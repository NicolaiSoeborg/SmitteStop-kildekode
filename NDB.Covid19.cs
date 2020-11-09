using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using I18NPortable;
using I18NPortable.JsonReader;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Security.Application;
using MoreLinq;
using NDB.Covid19.Config;
using NDB.Covid19.Enums;
using NDB.Covid19.ExposureNotification;
using NDB.Covid19.ExposureNotification.Helpers;
using NDB.Covid19.ExposureNotification.Helpers.ExposureDetected;
using NDB.Covid19.ExposureNotification.Helpers.FetchExposureKeys;
using NDB.Covid19.ExposureNotifications.Helpers.ExposureDetected;
using NDB.Covid19.ExposureNotifications.Helpers.FetchExposureKeys;
using NDB.Covid19.Implementation;
using NDB.Covid19.Interfaces;
using NDB.Covid19.Models;
using NDB.Covid19.Models.DTOsForServer;
using NDB.Covid19.Models.Logging;
using NDB.Covid19.Models.SQLite;
using NDB.Covid19.Models.UserDefinedExceptions;
using NDB.Covid19.OAuth2;
using NDB.Covid19.PersistedData;
using NDB.Covid19.PersistedData.SecureStorage;
using NDB.Covid19.PersistedData.SQLite;
using NDB.Covid19.ProtoModels;
using NDB.Covid19.SecureStorage;
using NDB.Covid19.Utils;
using NDB.Covid19.ViewModels;
using NDB.Covid19.WebServices;
using NDB.Covid19.WebServices.ErrorHandlers;
using NDB.Covid19.WebServices.ExposureNotification;
using NDB.Covid19.WebServices.Helpers;
using NDB.Covid19.WebServices.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PCLCrypto;
using Plugin.SecureStorage;
using Plugin.SecureStorage.Abstractions;
using SQLite;
using Unity;
using Unity.Injection;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETStandard,Version=v2.0", FrameworkDisplayName = "")]
[assembly: AssemblyCompany("NDB.Covid19")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("NDB.Covid19")]
[assembly: AssemblyTitle("NDB.Covid19")]
[assembly: AssemblyVersion("1.0.0.0")]
namespace NDB.Covid19
{
	public static class CommonDependencyInjectionConfig
	{
		public static void Init(UnityContainer unityContainer)
		{
			UnityContainerExtensions.RegisterType<ILoggingManager, LoggingSQLiteManager>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterType<IMessagesManager, MessagesManager>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterSingleton<SecureStorageService>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			if (Conf.UseDeveloperTools)
			{
				UnityContainerExtensions.RegisterType<IDeveloperToolsService, DeveloperToolsService>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			}
			else
			{
				UnityContainerExtensions.RegisterType<IDeveloperToolsService, ReleaseToolsService>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			}
			XamarinEssentialsRegister(unityContainer);
		}

		private static void XamarinEssentialsRegister(UnityContainer unityContainer)
		{
			UnityContainerExtensions.RegisterType<IConnectivity, ConnectivityImplementation>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterType<IAppInfo, AppInfoImplementation>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterType<IBrowser, BrowserImplementation>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterType<IClipboard, ClipboardImplementation>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterType<IDeviceInfo, DeviceInfoImplementation>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterType<IFileSystem, FileSystemImplementation>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterType<IPreferences, PreferencesImplementation>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterType<IShare, ShareImplementation>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
		}
	}
	public static class ExceptionExtensions
	{
		public static bool HandleExposureNotificationException(this Exception e, string className, string methodName)
		{
			if (e.ToString().Contains("Android.Gms.Common.Apis.ApiException: 17"))
			{
				LogUtils.LogException(LogSeverity.ERROR, e, className + "." + methodName + ": EN API was not available");
				return true;
			}
			if (e.ToString().Contains("com.google.android.gms.common.api.UnsupportedApiCallException"))
			{
				LogUtils.LogException(LogSeverity.ERROR, e, className + "." + methodName + ": EN API call is not supported");
				return true;
			}
			return false;
		}
	}
	public static class LocalesService
	{
		public static void Initialize()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			II18N current = I18N.get_Current();
			if (((current != null) ? current.get_Locale() : null) == null)
			{
				I18N.get_Current().SetNotFoundSymbol("$").SetFallbackLocale(Conf.DEFAULT_LANGUAGE)
					.AddLocaleReader((ILocaleReader)new JsonKvpReader(), ".json")
					.Init(typeof(LocalesService).GetTypeInfo().Assembly);
			}
			SetInternationalization();
		}

		public static string GetLanguage()
		{
			if (LocalPreferencesHelper.GetAppLanguage() != null)
			{
				return LocalPreferencesHelper.GetAppLanguage();
			}
			if (!Conf.SUPPORTED_LANGUAGES.Contains<string>(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
			{
				return Conf.DEFAULT_LANGUAGE;
			}
			return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
		}

		public static void SetInternationalization()
		{
			I18N.get_Current().set_Locale(GetLanguage());
		}
	}
	public class MigrationService
	{
		public int CurrentMigrationVersion = 2;

		private static IPreferences _preferences => ServiceLocator.get_Current().GetInstance<IPreferences>();

		public void Migrate()
		{
			for (int num = LocalPreferencesHelper.MigrationCount; num < CurrentMigrationVersion; num = (LocalPreferencesHelper.MigrationCount = num + 1))
			{
				DoTheMigrationToVersion(num + 1);
			}
		}

		private void DoTheMigrationToVersion(int versionToMigrateTo)
		{
			switch (versionToMigrateTo)
			{
			case 1:
				MigrateToVersion1();
				break;
			case 2:
				MigrateToVersion2();
				break;
			}
		}

		private void MigrateToVersion1()
		{
			DateTime value;
			try
			{
				value = DateTime.Parse(_preferences.Get(PreferencesKeys.LAST_DOWNLOAD_ZIPS_CALL_UTC_PREF, "defaultvalue"));
			}
			catch (Exception)
			{
				value = SystemTime.Now().AddDays(-14.0);
			}
			_preferences.Set(PreferencesKeys.LAST_DOWNLOAD_ZIPS_CALL_UTC_DATETIME_PREF, value);
			_preferences.Remove(PreferencesKeys.LAST_DOWNLOAD_ZIPS_CALL_UTC_PREF);
			try
			{
				value = DateTime.Parse(_preferences.Get(PreferencesKeys.CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_PREF, "defaultvalue"));
			}
			catch (Exception)
			{
				value = SystemTime.Now().Date;
			}
			_preferences.Set(PreferencesKeys.CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_DATETIME_PREF, value);
			_preferences.Remove(PreferencesKeys.CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_PREF);
		}

		private void MigrateToVersion2()
		{
			DateTime dateTime = _preferences.Get(PreferencesKeys.CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_DATETIME_PREF, DateTime.MinValue);
			if (dateTime != DateTime.MinValue)
			{
				_preferences.Set(PreferencesKeys.LAST_PULL_KEYS_SUCCEEDED_DATE_TIME, dateTime);
			}
			_preferences.Remove(PreferencesKeys.LAST_DOWNLOAD_ZIPS_CALL_UTC_DATETIME_PREF);
			_preferences.Remove(PreferencesKeys.CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_DATETIME_PREF);
			_preferences.Remove(PreferencesKeys.CURRENT_DOWNLOAD_DAY_BATCH_PREF);
		}
	}
}
namespace NDB.Covid19.WebServices
{
	public class BaseWebService
	{
		private readonly BadConnectionErrorHandler _badConnectionErrorHandler = new BadConnectionErrorHandler();

		public static JsonSerializerSettings JsonSerializerSettings;

		public static JsonSerializer JsonSerializer;

		private HttpClientManager _httpClientManager
		{
			get
			{
				HttpClientManager instance = HttpClientManager.Instance;
				instance.AddSecretToHeaderIfMissing();
				return instance;
			}
		}

		private HttpClient _client => _httpClientManager.HttpClientAccessor.HttpClient;

		public async Task<ApiResponse<M>> Get<M>(string url)
		{
			ApiResponse<M> apiResponse = await InnerGet<M>(url);
			if (!apiResponse.IsSuccessfull && _badConnectionErrorHandler.IsResponsible(apiResponse))
			{
				apiResponse = await InnerGet<M>(url);
			}
			return apiResponse;
		}

		private async Task<ApiResponse<M>> InnerGet<M>(string url)
		{
			ApiResponse<M> result = new ApiResponse<M>(url, HttpMethod.Get);
			try
			{
				HttpResponseMessage httpResponseMessage = await _client.GetAsync(url);
				result.StatusCode = (int)httpResponseMessage.StatusCode;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string text = await httpResponseMessage.Content.ReadAsStringAsync();
					if (!string.IsNullOrEmpty(text))
					{
						result.ResponseText = text;
						MapData(result, text);
						return result;
					}
					return result;
				}
				result.ResponseText = httpResponseMessage.ReasonPhrase;
				return result;
			}
			catch (Exception ex)
			{
				Exception ex3 = (result.Exception = ex);
				return result;
			}
		}

		public virtual async Task<ApiResponse<Stream>> GetFileAsStreamAsync(string url)
		{
			ApiResponse<Stream> result = new ApiResponse<Stream>(url, HttpMethod.Get);
			try
			{
				HttpResponseMessage response = await _client.GetAsync(url);
				result.StatusCode = (int)response.StatusCode;
				if (response.IsSuccessStatusCode)
				{
					Stream stream = await response.Content.ReadAsStreamAsync();
					result.Headers = response.Headers;
					if (stream.Length > 0)
					{
						result.Data = stream;
						PrintFileSize(response);
					}
				}
				else
				{
					result.ResponseText = response.ReasonPhrase;
				}
				return result;
			}
			catch (Exception ex)
			{
				Exception ex3 = (result.Exception = ex);
				return result;
			}
		}

		private void PrintFileSize(HttpResponseMessage response)
		{
		}

		public async Task<ApiResponse> Post(string url)
		{
			return await Post<object>(null, url);
		}

		public virtual async Task<ApiResponse> Post<T>(T t, string url)
		{
			string content2 = JsonConvert.SerializeObject((object)t, JsonSerializerSettings);
			StringContent content = new StringContent(content2, Encoding.UTF8, "application/json");
			ApiResponse apiResponse = await InnerPost(content, url);
			if (!apiResponse.IsSuccessfull && _badConnectionErrorHandler.IsResponsible(apiResponse))
			{
				apiResponse = await InnerPost(content, url);
			}
			return apiResponse;
		}

		private async Task<ApiResponse> InnerPost(StringContent content, string url)
		{
			ApiResponse result = new ApiResponse(url, HttpMethod.Post);
			try
			{
				HttpResponseMessage httpResponseMessage = await _client.PostAsync(url, content);
				result.StatusCode = (int)httpResponseMessage.StatusCode;
				PrintHeadersToConsole(httpResponseMessage);
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					ApiResponse apiResponse = result;
					apiResponse.ResponseText = await httpResponseMessage.Content.ReadAsStringAsync();
					return result;
				}
				try
				{
					result.ResponseText = httpResponseMessage.ReasonPhrase;
					return result;
				}
				catch (Exception)
				{
					return result;
				}
			}
			catch (Exception ex2)
			{
				Exception ex4 = (result.Exception = ex2);
				return result;
			}
		}

		private void PrintHeadersToConsole(HttpResponseMessage response)
		{
			try
			{
				if (response.RequestMessage?.Headers == null)
				{
					return;
				}
				foreach (KeyValuePair<string, IEnumerable<string>> item in response.RequestMessage?.Headers)
				{
					string.Join("; ", item.Value);
				}
			}
			catch (Exception)
			{
			}
		}

		public void MapData<M>(ApiResponse<M> resultObj, string content)
		{
			JObject val = JObject.Parse(content);
			if (val != null)
			{
				resultObj.Data = ((JToken)val).ToObject<M>(JsonSerializer);
			}
		}

		public static void HandleErrors(ApiResponse response, params IErrorHandler[] extraErrorHandlers)
		{
			HandleErrors(response, silently: false, extraErrorHandlers);
		}

		public static void HandleErrorsSilently(ApiResponse response, params IErrorHandler[] extraErrorHandlers)
		{
			HandleErrors(response, silently: true, extraErrorHandlers);
		}

		private static void HandleErrors(ApiResponse response, bool silently, params IErrorHandler[] extraErrorHandlers)
		{
			List<IErrorHandler> first = new List<IErrorHandler>
			{
				new ApiDeprecatedErrorHandler(),
				new NoInternetErrorHandler(silently),
				new BadConnectionErrorHandler(silently),
				new TimeoutErrorHandler(silently)
			};
			List<IErrorHandler> errorHandlers = Enumerable.Concat(second: new List<IErrorHandler>
			{
				new DefaultErrorHandler(silently)
			}, first: first.Concat(extraErrorHandlers)).ToList();
			Handle(response, errorHandlers);
		}

		private static void Handle(ApiResponse response, List<IErrorHandler> errorHandlers)
		{
			foreach (IErrorHandler errorHandler in errorHandlers)
			{
				if (errorHandler.IsResponsible(response))
				{
					errorHandler.HandleError(response);
					break;
				}
			}
		}

		static BaseWebService()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			JsonSerializerSettings val = new JsonSerializerSettings();
			val.set_ContractResolver((IContractResolver)new CamelCasePropertyNamesContractResolver());
			val.set_DateFormatHandling((DateFormatHandling)0);
			val.set_DateTimeZoneHandling((DateTimeZoneHandling)0);
			JsonSerializerSettings = val;
			JsonSerializer = new JsonSerializer();
		}
	}
	public class CountryListService : BaseWebService
	{
		public async Task<CountryListDTO> GetCountryList()
		{
			ApiResponse<CountryListDTO> obj = await Get<CountryListDTO>(Conf.URL_GET_COUNTRY_LIST);
			BaseWebService.HandleErrorsSilently(obj);
			return obj?.Data;
		}
	}
	public class ExposureNotificationWebService : BaseWebService
	{
		public async Task<bool> PostSelvExposureKeys(IEnumerable<ExposureKeyModel> temporaryExposureKeys)
		{
			return await PostSelvExposureKeys(new SelfDiagnosisSubmissionDTO(temporaryExposureKeys), temporaryExposureKeys);
		}

		public async Task<bool> PostSelvExposureKeys(SelfDiagnosisSubmissionDTO selfDiagnosisSubmissionDTO, IEnumerable<ExposureKeyModel> temporaryExposureKeys)
		{
			return await PostSelvExposureKeys(selfDiagnosisSubmissionDTO, temporaryExposureKeys, this);
		}

		public async Task<bool> PostSelvExposureKeys(SelfDiagnosisSubmissionDTO selfDiagnosisSubmissionDTO, IEnumerable<ExposureKeyModel> temporaryExposureKeys, BaseWebService service)
		{
			ApiResponse apiResponse = await service.Post(selfDiagnosisSubmissionDTO, Conf.URL_PUT_UPLOAD_DIAGNOSIS_KEYS);
			if (!apiResponse.IsSuccessfull)
			{
				string redactedKeys = RedactedTekListHelper.CreateRedactedTekList(temporaryExposureKeys);
				BaseWebService.HandleErrorsSilently(apiResponse, new PostExposureKeysErrorHandler(redactedKeys));
			}
			else
			{
				BaseWebService.HandleErrorsSilently(apiResponse);
			}
			ENDeveloperToolsViewModel.UpdatePushKeysInfo(apiResponse, selfDiagnosisSubmissionDTO, BaseWebService.JsonSerializerSettings);
			return apiResponse.IsSuccessfull;
		}

		public async Task<Configuration> GetExposureConfiguration()
		{
			ApiResponse<AttenuationBucketsConfigurationDTO> apiResponse = await Get<AttenuationBucketsConfigurationDTO>(Conf.URL_GET_EXPOSURE_CONFIGURATION);
			BaseWebService.HandleErrorsSilently(apiResponse);
			LogUtils.SendAllLogs();
			if (apiResponse.IsSuccessfull && apiResponse.Data != null && apiResponse.Data.Configuration != null)
			{
				if (apiResponse.Data.AttenuationBucketsParams != null)
				{
					LocalPreferencesHelper.ExposureTimeThreshold = apiResponse.Data.AttenuationBucketsParams.ExposureTimeThreshold;
					LocalPreferencesHelper.LowAttenuationDurationMultiplier = apiResponse.Data.AttenuationBucketsParams.LowAttenuationBucketMultiplier;
					LocalPreferencesHelper.MiddleAttenuationDurationMultiplier = apiResponse.Data.AttenuationBucketsParams.MiddleAttenuationBucketMultiplier;
					LocalPreferencesHelper.HighAttenuationDurationMultiplier = apiResponse.Data.AttenuationBucketsParams.HighAttenuationBucketMultiplier;
				}
				return apiResponse.Data.Configuration;
			}
			return null;
		}

		public virtual async Task<ApiResponse<Stream>> GetDiagnosisKeys(string batchRequestString, CancellationToken cancellationToken)
		{
			string url = Conf.URL_GET_DIAGNOSIS_KEYS + "/" + batchRequestString;
			ApiResponse<Stream> obj = await GetFileAsStreamAsync(url);
			BaseWebService.HandleErrorsSilently(obj);
			if (obj.IsSuccessfull)
			{
				LocalPreferencesHelper.UpdateLastUpdatedDate();
			}
			return obj;
		}
	}
	internal class FakeGatewayWebService : BaseWebService
	{
		public async Task<ApiResponse> UploadKeys(SelfDiagnosisSubmissionDTO selfDiagnosisSubmissionDto)
		{
			ApiResponse obj = await new BaseWebService().Post(selfDiagnosisSubmissionDto, Conf.URL_GATEWAY_STUB_UPLOAD);
			BaseWebService.HandleErrorsSilently(obj);
			return obj;
		}
	}
	public class LoggingService : BaseWebService
	{
		public async Task<bool> PostAllLogs(List<LogDTO> dtos)
		{
			object t = new
			{
				Logs = dtos.ToArray()
			};
			return (await Post(t, Conf.URL_LOG_MESSAGE)).IsSuccessfull;
		}
	}
	public static class HeaderUtils
	{
		public static void AddSecretToHeader(IHttpClientAccessor accessor)
		{
			if (accessor.HttpClient.DefaultRequestHeaders.Contains("Authorization"))
			{
				accessor.HttpClient.DefaultRequestHeaders.Remove("Authorization");
			}
			if (AuthenticationState.PersonalData != null && AuthenticationState.PersonalData.Validate())
			{
				string str = AuthenticationState.PersonalData?.Access_token;
				accessor.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + str);
			}
		}
	}
}
namespace NDB.Covid19.WebServices.Utils
{
	public interface IHttpClientAccessor
	{
		HttpClient HttpClient
		{
			get;
		}

		CookieContainer Cookies
		{
			get;
		}
	}
	public class DefaultHttpClientAccessor : IHttpClientAccessor
	{
		public HttpClient HttpClient
		{
			get;
		}

		public CookieContainer Cookies
		{
			get;
		}

		public DefaultHttpClientAccessor()
		{
			Cookies = new CookieContainer();
			HttpClient = new HttpClient(new HttpClientHandler
			{
				CookieContainer = Cookies
			});
			HttpClient.Timeout = TimeSpan.FromSeconds(10.0);
		}
	}
	public class HttpClientManager
	{
		public static string CsrfpTokenCookieName = "Csrfp-Token";

		public static string CsrfpTokenHeader = "csrfp-token";

		public IHttpClientAccessor HttpClientAccessor;

		private static HttpClientManager _instance;

		public static HttpClientManager Instance
		{
			get
			{
				if (_instance == null)
				{
					MakeNewInstance();
				}
				return _instance;
			}
		}

		private HttpClientManager()
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			HttpClientAccessor = new DefaultHttpClientAccessor();
			HttpClientAccessor.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			HttpClientAccessor.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
			HttpClientAccessor.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/zip"));
			HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("Authorization_Mobile", Conf.AuthorizationHeader);
			IDeviceInfo instance = ServiceLocator.get_Current().GetInstance<IDeviceInfo>();
			if (instance.Platform == DevicePlatform.get_Unknown())
			{
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("Manufacturer", "Unknown");
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("OSVersion", "Unknown");
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("OS", "Unknown");
			}
			else
			{
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("Manufacturer", instance.Manufacturer);
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("OSVersion", instance.VersionString);
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("OS", DeviceUtils.DeviceType);
			}
			HttpClientAccessor.HttpClient.MaxResponseContentBufferSize = 3000000L;
			HttpClientAccessor.HttpClient.Timeout = TimeSpan.FromSeconds(Conf.DEFAULT_TIMEOUT_SERVICECALLS_SECONDS);
		}

		public void AddSecretToHeaderIfMissing()
		{
			HeaderUtils.AddSecretToHeader(HttpClientAccessor);
		}

		public static void MakeNewInstance()
		{
			if (_instance?.HttpClientAccessor?.HttpClient != null)
			{
				_instance.HttpClientAccessor.HttpClient.CancelPendingRequests();
			}
			_instance = new HttpClientManager();
		}

		public bool CheckInternetConnection()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			return (int)ServiceLocator.get_Current().GetInstance<IConnectivity>().NetworkAccess != 1;
		}
	}
}
namespace NDB.Covid19.WebServices.Helpers
{
	public abstract class RedactedTekListHelper
	{
		public static string CreateRedactedTekList(IEnumerable<ExposureKeyModel> teks)
		{
			return JsonConvert.SerializeObject((object)teks.Select((ExposureKeyModel tek) => new ExposureKeyModel(new byte[1], ((TemporaryExposureKey)tek).get_RollingStart(), ((TemporaryExposureKey)tek).get_RollingDuration(), ((TemporaryExposureKey)tek).get_TransmissionRiskLevel())), BaseWebService.JsonSerializerSettings);
		}
	}
}
namespace NDB.Covid19.WebServices.ExposureNotification
{
	public enum BatchType
	{
		ALL,
		DK
	}
	public static class BatchTypeExtensions
	{
		public static string ToTypeString(this BatchType type)
		{
			if (type == BatchType.ALL)
			{
				return "all";
			}
			return "dk";
		}

		public static BatchType ToBatchType(this string type)
		{
			if (type == "all")
			{
				return BatchType.ALL;
			}
			return BatchType.DK;
		}
	}
	public class PullKeysParams
	{
		public DateTime Date
		{
			get;
			set;
		}

		public BatchType BatchType
		{
			get;
			set;
		}

		public int BatchNumber
		{
			get;
			set;
		}

		public string ToBatchFileRequest()
		{
			return Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + $"_{BatchNumber}" + "_" + BatchType.ToTypeString() + ".zip";
		}

		public static PullKeysParams GenerateParams()
		{
			DateTime date = SystemTime.Now().ToUniversalTime().Date;
			bool num = OnboardingStatusHelper.Status == OnboardingStatus.CountriesOnboardingCompleted;
			BatchType batchType = ((!num) ? BatchType.DK : BatchType.ALL);
			DateTime date2 = LocalPreferencesHelper.GetLastPullKeysSucceededDateTime();
			DateTime date3 = date2.Date;
			DateTime minValue = DateTime.MinValue;
			if (date3.Equals(minValue.Date))
			{
				date2 = date;
			}
			int lastPullKeysBatchNumberSuccessfullySubmitted = LocalPreferencesHelper.LastPullKeysBatchNumberSuccessfullySubmitted;
			lastPullKeysBatchNumberSuccessfullySubmitted++;
			if (date2.Date <= date.AddDays(-14.0))
			{
				date2 = date.AddDays(-13.0);
				lastPullKeysBatchNumberSuccessfullySubmitted = 1;
			}
			if (num && LocalPreferencesHelper.LastPulledBatchType == BatchType.DK)
			{
				batchType = BatchType.ALL;
				lastPullKeysBatchNumberSuccessfullySubmitted = 1;
			}
			return new PullKeysParams
			{
				BatchType = batchType,
				Date = date2,
				BatchNumber = lastPullKeysBatchNumberSuccessfullySubmitted
			};
		}
	}
}
namespace NDB.Covid19.WebServices.ErrorHandlers
{
	public class ApiDeprecatedErrorHandler : BaseErrorHandler, IErrorHandler
	{
		public bool IsResponsible(ApiResponse apiResponse)
		{
			return apiResponse.StatusCode == 410;
		}

		public void HandleError(ApiResponse apiResponse)
		{
			LogUtils.LogApiError(LogSeverity.WARNING, apiResponse, erroredSilently: false);
			MessagingCenter.Send((object)this, MessagingCenterKeys.KEY_FORCE_UPDATE);
		}
	}
	public class BadConnectionErrorHandler : BaseErrorHandler, IErrorHandler
	{
		public bool IsSilent;

		public override string ErrorMessageTitle => Extensions.Translate("CONNECTION_ERROR_TITLE", Array.Empty<object>());

		public override string ErrorMessage => Extensions.Translate("BAD_CONNECTION_ERROR_MESSAGE", Array.Empty<object>());

		public BadConnectionErrorHandler(bool IsSilent)
		{
			this.IsSilent = IsSilent;
		}

		public BadConnectionErrorHandler()
		{
		}

		public bool IsResponsible(ApiResponse apiResponse)
		{
			bool num = apiResponse.Exception?.InnerException is IOException && (apiResponse.Exception.InnerException!.Message.Contains("the transport connection") || apiResponse.Exception.InnerException!.Message.Contains("The server returned an invalid or unrecognized response"));
			bool flag = apiResponse.Exception is WebException;
			return num || flag;
		}

		public void HandleError(ApiResponse apiResponse)
		{
			LogUtils.LogApiError(LogSeverity.WARNING, apiResponse, IsSilent, "", "Failed contact to server: Bad connection");
			if (!IsSilent)
			{
				ShowErrorToUser();
			}
		}
	}
	public class BaseErrorHandler
	{
		public IDialogService DialogServiceInstance => ServiceLocator.get_Current().GetInstance<IDialogService>();

		public virtual string ErrorMessageTitle => Extensions.Translate("BASE_ERROR_TITLE", Array.Empty<object>());

		public virtual string ErrorMessage => Extensions.Translate("BASE_ERROR_MESSAGE", Array.Empty<object>());

		public virtual string OkBtnText => Extensions.Translate("ERROR_OK_BTN", Array.Empty<object>());

		public void ShowErrorToUser()
		{
			DialogServiceInstance.ShowMessageDialog(ErrorMessageTitle, ErrorMessage, OkBtnText);
		}
	}
	public class DefaultErrorHandler : BaseErrorHandler, IErrorHandler
	{
		public bool IsSilent;

		public DefaultErrorHandler()
		{
		}

		public DefaultErrorHandler(bool IsSilent)
		{
			this.IsSilent = IsSilent;
		}

		public bool IsResponsible(ApiResponse apiResponse)
		{
			return !apiResponse.IsSuccessfull;
		}

		public void HandleError(ApiResponse apiResponse)
		{
			LogUtils.LogApiError(LogSeverity.ERROR, apiResponse, IsSilent);
			if (!IsSilent)
			{
				ShowErrorToUser();
			}
		}
	}
	public interface IDialogService
	{
		void ShowMessageDialog(string title, string message, string okBtn, PlatformDialogServiceArguments platformArguments = null);
	}
	public class PlatformDialogServiceArguments
	{
		public object Context
		{
			get;
			set;
		}
	}
	public interface IErrorHandler
	{
		bool IsResponsible(ApiResponse apiResponse);

		void HandleError(ApiResponse apiResponse);
	}
	public class NoInternetErrorHandler : BaseErrorHandler, IErrorHandler
	{
		public bool IsSilent;

		public override string ErrorMessageTitle => Extensions.Translate("CONNECTION_ERROR_TITLE", Array.Empty<object>());

		public override string ErrorMessage => Extensions.Translate("NO_INTERNET_ERROR_MESSAGE", Array.Empty<object>());

		public NoInternetErrorHandler(bool IsSilent)
		{
			this.IsSilent = IsSilent;
		}

		public NoInternetErrorHandler()
		{
		}

		public bool IsResponsible(ApiResponse apiResponse)
		{
			return !HttpClientManager.Instance.CheckInternetConnection();
		}

		public void HandleError(ApiResponse apiResponse)
		{
			LogUtils.LogApiError(LogSeverity.WARNING, apiResponse, IsSilent, "", "Failed contact to server: No internet");
			if (!IsSilent)
			{
				ShowErrorToUser();
			}
		}
	}
	public class PostExposureKeysErrorHandler : BaseErrorHandler, IErrorHandler
	{
		private readonly string RedactedKeys;

		public PostExposureKeysErrorHandler(string RedactedKeys)
		{
			this.RedactedKeys = RedactedKeys;
		}

		public bool IsResponsible(ApiResponse apiResponse)
		{
			return !apiResponse.IsSuccessfull;
		}

		public void HandleError(ApiResponse apiResponse)
		{
			LogUtils.LogApiError(LogSeverity.ERROR, apiResponse, erroredSilently: true, RedactedKeys);
		}
	}
	public class TimeoutErrorHandler : BaseErrorHandler, IErrorHandler
	{
		public bool IsSilent;

		public override string ErrorMessageTitle => Extensions.Translate("CONNECTION_ERROR_TITLE", Array.Empty<object>());

		public override string ErrorMessage => Extensions.Translate("BAD_CONNECTION_ERROR_MESSAGE", Array.Empty<object>());

		public TimeoutErrorHandler(bool IsSilent)
		{
			this.IsSilent = IsSilent;
		}

		public TimeoutErrorHandler()
		{
		}

		public bool IsResponsible(ApiResponse apiResponse)
		{
			if (apiResponse.Exception != null)
			{
				return apiResponse.Exception is TaskCanceledException;
			}
			return false;
		}

		public void HandleError(ApiResponse apiResponse)
		{
			string overwriteMessage = $"{apiResponse.ErrorLogMessage}. Timed out after {Conf.DEFAULT_TIMEOUT_SERVICECALLS_SECONDS} seconds because of bad connection.";
			LogUtils.LogApiError(LogSeverity.WARNING, apiResponse, IsSilent, "", overwriteMessage);
			if (!IsSilent)
			{
				ShowErrorToUser();
			}
		}
	}
}
namespace NDB.Covid19.Utils
{
	public static class Anonymizer
	{
		public static string ReplaceCpr(string input)
		{
			string input2 = input;
			Regex regex = new Regex("[0-3][0-9][0-1][1-9]\\d{2}-\\d{4}?[^0-9]*?");
			Regex regex2 = new Regex("[0-3][0-9][0-1][1-9]\\d{2}\\d{4}?[^0-9]*?");
			Regex regex3 = new Regex("[0-3][0-9][0-1][1-9]\\d{2} \\d{4}?[^0-9]*?");
			input2 = regex.Replace(input2, "xxxxxx-xxxx");
			input2 = regex2.Replace(input2, "xxxxxxxxxx");
			return regex3.Replace(input2, "xxxxxx xxxx");
		}

		public static string ReplacePhoneNumber(string input)
		{
			string input2 = input;
			Regex regex = new Regex("((\\+[0-9]{2})|([0]{2}[0-9]{2}))", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
			Regex regex2 = new Regex("([0-9]{2}(\\ |\\-)[0-9]{2}(\\ |\\-)[0-9]{2}(\\ |\\-)[0-9]{2})", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
			Regex regex3 = new Regex("([0-9]{4}(\\ |\\-)[0-9]{4})", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
			Regex regex4 = new Regex("([0-9]{8})", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
			input2 = regex.Replace(input2, "+xx");
			input2 = regex2.Replace(input2, "xx-xx-xx-xx");
			input2 = regex3.Replace(input2, "xxxx-xxxx");
			return regex4.Replace(input2, "xxxxxxxx");
		}

		public static string ReplaceEmailAddress(string input)
		{
			string pattern = "(?<=[\\w]{0})[\\w-\\._\\+%\\\\]*(?=[\\w]{0}@)|(?<=@[\\w]{0})[\\w-_\\+%]*(?=\\.)";
			return Regex.Replace(input, pattern, (Match m) => new string('*', m.Length));
		}

		public static string ReplaceMacAddress(string input)
		{
			return new Regex("(?:[0-9a-fA-F]{2}[:.-]){5}[0-9a-fA-F]{2}|\n                        (?:[0-9a-fA-F]{2}-){5}[0-9a-fA-F]{2}|\n                        (?:[0-9a-fA-F]{2}){5}[0-9a-fA-F]{2}$|\n                        (?:[0-9a-fA-F]{3}[:.-]){5}[0-9a-fA-F]{3}|\n                        (?:[0-9a-fA-F]{3}-){4}[0-9a-fA-F]{3}|\n                        (?:[0-9a-fA-F]{3}){5}[0-9a-fA-F]{3}$|\n                        (?:[0-9a-fA-F]{3}[:.-]){3}[0-9a-fA-F]{3}|\n                        (?:[0-9a-fA-F]{3}-){3}[0-9a-fA-F]{3}|\n                        (?:[0-9a-fA-F]{3}){3}[0-9a-fA-F]{3}$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace).Replace(input, "xx:xx:xx:xx");
		}

		public static string ReplaceIMEI(string input)
		{
			return new Regex("([0-9]{15})", RegexOptions.Multiline).Replace(input, (Match match) => (!Luhn(match.ToString())) ? match.ToString() : "xxxxxxxxxxxxxxx");
		}

		public static string RedactText(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			return Encoder.HtmlEncode(Sanitizer.GetSafeHtmlFragment(ReplaceMacAddress(ReplaceEmailAddress(ReplacePhoneNumber(ReplaceCpr(ReplaceIMEI(input)))))));
		}

		private static bool Luhn(string digits)
		{
			if (digits.All(char.IsDigit))
			{
				return (from c in digits.Reverse()
					select c - 48).Select((int thisNum, int i) => (i % 2 != 0) ? (((thisNum *= 2) <= 9) ? thisNum : (thisNum - 9)) : thisNum).Sum() % 10 == 0;
			}
			return false;
		}
	}
	public class ConnectivityHelper
	{
		private static IEnumerable<ConnectionProfile> _connectionProfiles;

		private static IConnectivity _connectivity => ServiceLocator.get_Current().GetInstance<IConnectivity>();

		public static NetworkAccess NetworkAccess => _connectivity.NetworkAccess;

		public static IEnumerable<ConnectionProfile> ConnectionProfiles => _connectionProfiles ?? _connectivity.ConnectionProfiles;

		public static void MockConnectionProfiles(List<ConnectionProfile> mockedConnectionProfiles)
		{
			_connectionProfiles = mockedConnectionProfiles;
		}

		public static void ResetConnectionProfiles()
		{
			_connectionProfiles = null;
		}
	}
	public static class DateUtils
	{
		public static string GetDateFromDateTime(this DateTime? date, string dateFormat)
		{
			if (date.HasValue)
			{
				DateTime value = date.Value;
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(Conf.DEFAULT_LANGUAGE);
				bool flag = Conf.SUPPORTED_LANGUAGES.Contains<string>(currentCulture.TwoLetterISOLanguageName);
				DateTime dateTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, flag ? CultureInfo.CurrentCulture.Calendar : new GregorianCalendar());
				return dateTime.ToString(dateFormat, flag ? currentCulture : cultureInfo).Replace("-", "/");
			}
			return string.Empty;
		}

		public static string ReplaceAndInsertNewlineiOS(string text, string sizeCategory)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			bool flag = sizeCategory switch
			{
				"UICTContentSizeCategoryXS" => false, 
				"UICTContentSizeCategoryS" => false, 
				"UICTContentSizeCategoryM" => false, 
				"UICTContentSizeCategoryL" => false, 
				_ => true, 
			};
			DisplayInfo mainDisplayInfo = DeviceDisplay.get_MainDisplayInfo();
			text = ((!(((DisplayInfo)(ref mainDisplayInfo)).get_Width() <= 750.0 && flag)) ? text.Replace("-", "/") : ReplaceLastOccurrence(text.Replace("-", "/"), "/", "/" + Environment.NewLine));
			return text;
		}

		private static string ReplaceLastOccurrence(string Source, string Find, string Replace)
		{
			int num = Source.LastIndexOf(Find);
			if (num == -1)
			{
				return Source;
			}
			return Source.Remove(num, Find.Length).Insert(num, Replace);
		}

		public static string ToGreGorianUtcString(this DateTime dateTime, string format)
		{
			return dateTime.ToString(format, CultureInfo.InvariantCulture);
		}

		public static DateTime TrimMilliseconds(this DateTime dt)
		{
			return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
		}
	}
	public sealed class DeveloperToolsService : IDeveloperToolsService
	{
		public static readonly string DEV_TOOLS_LAST_PROVIDED_FILES_PREF = "DEV_TOOLS_LAST_PROVIDED_FILES_PREF";

		public static readonly string DEV_TOOLS_SHOULD_SAVE_EXPOSURE_INFOS_PREF = "DEV_TOOLS_SHOULD_SAVE_EXPOSURE_INFOS_PREF";

		public static readonly string DEV_TOOLS_LAST_EXPOSURE_INFOS_PREF = "DEV_TOOLS_LAST_EXPOSURE_INFOS_PREF";

		public static readonly string DEV_TOOLS_LAST_KEY_UPLOAD_INFO = "LastKeyUploadInfo";

		public static readonly string DEV_TOOLS_LAST_USED_CONFIGURATION = "LastUsedConfiguration";

		private static IPreferences _preferences => ServiceLocator.get_Current().GetInstance<IPreferences>();

		public string LastKeyUploadInfo
		{
			get
			{
				return _preferences.Get(DEV_TOOLS_LAST_KEY_UPLOAD_INFO, "");
			}
			set
			{
				_preferences.Set(DEV_TOOLS_LAST_KEY_UPLOAD_INFO, value);
			}
		}

		public string LastUsedConfiguration
		{
			get
			{
				return _preferences.Get(DEV_TOOLS_LAST_USED_CONFIGURATION, "");
			}
			set
			{
				_preferences.Set(DEV_TOOLS_LAST_USED_CONFIGURATION, value);
			}
		}

		public bool ShouldSaveExposureInfo
		{
			get
			{
				return _preferences.Get(DEV_TOOLS_SHOULD_SAVE_EXPOSURE_INFOS_PREF, defaultValue: false);
			}
			set
			{
				_preferences.Set(DEV_TOOLS_SHOULD_SAVE_EXPOSURE_INFOS_PREF, value);
			}
		}

		public string LastProvidedFilesPref
		{
			get
			{
				return _preferences.Get(DEV_TOOLS_LAST_PROVIDED_FILES_PREF, "");
			}
			set
			{
				_preferences.Set(DEV_TOOLS_LAST_PROVIDED_FILES_PREF, value);
			}
		}

		public string PersistedExposureInfo
		{
			get
			{
				return _preferences.Get(DEV_TOOLS_LAST_EXPOSURE_INFOS_PREF, "");
			}
			set
			{
				_preferences.Set(DEV_TOOLS_LAST_EXPOSURE_INFOS_PREF, value);
			}
		}

		public string AllPullHistory
		{
			get
			{
				return _preferences.Get(PreferencesKeys.DEV_TOOLS_PULL_KEYS_HISTORY, "");
			}
			set
			{
				_preferences.Set(PreferencesKeys.DEV_TOOLS_PULL_KEYS_HISTORY, value);
			}
		}

		public string LastPullHistory
		{
			get
			{
				return _preferences.Get(PreferencesKeys.DEV_TOOLS_PULL_KEYS_HISTORY_LAST_RECORD, "");
			}
			set
			{
				_preferences.Set(PreferencesKeys.DEV_TOOLS_PULL_KEYS_HISTORY_LAST_RECORD, value);
			}
		}

		public void ClearAllFields()
		{
			_preferences.Set(DEV_TOOLS_LAST_KEY_UPLOAD_INFO, "");
			_preferences.Set(DEV_TOOLS_LAST_USED_CONFIGURATION, "");
			_preferences.Set(DEV_TOOLS_SHOULD_SAVE_EXPOSURE_INFOS_PREF, "");
			_preferences.Set(DEV_TOOLS_LAST_EXPOSURE_INFOS_PREF, "");
			_preferences.Set(DEV_TOOLS_LAST_PROVIDED_FILES_PREF, "");
		}

		public void StoreLastProvidedFiles(IEnumerable<string> localFileUrls)
		{
			string str = "TEK batch files downloaded at " + SystemTime.Now().ToGreGorianUtcString("yyyy-MM-dd HH:mm:ss") + " UTC:\n#######\n";
			foreach (string localFileUrl in localFileUrls)
			{
				TemporaryExposureKeyExport temporaryExposureKeyExport = BatchFileHelper.ZipToTemporaryExposureKeyExport(BatchFileHelper.UrlToZipArchive(localFileUrl));
				string str2 = TemporaryExposureKeyExportToPrettyString(temporaryExposureKeyExport);
				str = str + str2 + "\n";
			}
			str = (LastProvidedFilesPref = str + "#######");
		}

		public async Task SaveLastExposureInfos(Func<Task<IEnumerable<ExposureInfo>>> getExposureInfo)
		{
			bool flag;
			try
			{
				flag = ShouldSaveExposureInfo;
			}
			catch (Exception)
			{
				flag = false;
			}
			if (flag)
			{
				try
				{
					ShouldSaveExposureInfo = false;
					string text2 = (PersistedExposureInfo = ExposureInfoJsonHelper.ExposureInfosToJson(await getExposureInfo()));
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.WARNING, e, "ExposureDetectedHelper.DevToolsSaveLastExposureInfos");
				}
			}
		}

		public string TemporaryExposureKeyExportToPrettyString(TemporaryExposureKeyExport temporaryExposureKeyExport)
		{
			try
			{
				string str = "TEK batch, containing these keys:\n";
				str = str + "Regions: " + temporaryExposureKeyExport.Region + "\n";
				string text = "";
				int num = 0;
				foreach (TemporaryExposureKey key in temporaryExposureKeyExport.Keys)
				{
					string str2 = ((text == "") ? "--" : "\n--");
					text += str2;
					text = text + "[TemporaryExposureKey with KeyData.ToBase64()=" + key.KeyData.ToBase64() + ", <In DB format: " + EncodingUtils.ConvertByteArrayToString(key.KeyData.ToByteArray()) + "> " + $"TransmissionRiskLevel={key.TransmissionRiskLevel}, " + "RollingStartIntervalNumber=" + DateTimeOffset.FromUnixTimeSeconds(key.RollingStartIntervalNumber * 600).UtcDateTime.ToGreGorianUtcString("yyyy-MM-dd HH:mm:ss") + " UTC and " + $"RollingPeriod={key.RollingPeriod * 10} minutes]";
					num++;
					if (num == 200)
					{
						break;
					}
				}
				return str + text;
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, "DeveloperToolsService.TemporaryExposureKeyExportToPrettyString");
				return "";
			}
		}

		public void StartPullHistoryRecord()
		{
			string str = SystemTime.Now().ToGreGorianUtcString("yyyy-MM-dd HH:mm");
			string text2 = (LastPullHistory = "Pulled the following keys (batches) at " + str + " UTC:");
			string allPullHistory = AllPullHistory;
			AllPullHistory = (string.IsNullOrEmpty(allPullHistory) ? text2 : (allPullHistory + "\n\n" + text2));
		}

		public void AddToPullHistoryRecord(string message, string requestUrl = null)
		{
			string text = ((requestUrl == null) ? ("\n* " + message) : ("\n* " + requestUrl + ": " + message));
			AllPullHistory += text;
			LastPullHistory += text;
		}
	}
	public interface IDeveloperToolsService
	{
		string LastKeyUploadInfo
		{
			get;
			set;
		}

		string LastUsedConfiguration
		{
			get;
			set;
		}

		bool ShouldSaveExposureInfo
		{
			get;
			set;
		}

		string LastProvidedFilesPref
		{
			get;
			set;
		}

		string PersistedExposureInfo
		{
			get;
			set;
		}

		string LastPullHistory
		{
			get;
			set;
		}

		string AllPullHistory
		{
			get;
			set;
		}

		void ClearAllFields();

		void StoreLastProvidedFiles(IEnumerable<string> localFileUrls);

		Task SaveLastExposureInfos(Func<Task<IEnumerable<ExposureInfo>>> getExposureInfo);

		string TemporaryExposureKeyExportToPrettyString(TemporaryExposureKeyExport temporaryExposureKeyExport);

		void StartPullHistoryRecord();

		void AddToPullHistoryRecord(string message, string requestUrl = null);
	}
	public class ReleaseToolsService : IDeveloperToolsService
	{
		public string LastKeyUploadInfo
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public string LastUsedConfiguration
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public bool ShouldSaveExposureInfo
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public string LastProvidedFilesPref
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public string PersistedExposureInfo
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public string AllPullHistory
		{
			get;
			set;
		}

		public string LastPullHistory
		{
			get;
			set;
		}

		public void ClearAllFields()
		{
		}

		public void StoreLastProvidedFiles(IEnumerable<string> localFileUrls)
		{
		}

		public Task SaveLastExposureInfos(Func<Task<IEnumerable<ExposureInfo>>> getExposureInfo)
		{
			return Task.FromResult(result: true);
		}

		public string TemporaryExposureKeyExportToPrettyString(TemporaryExposureKeyExport temporaryExposureKeyExport)
		{
			return "";
		}

		public void StartPullHistoryRecord()
		{
		}

		public void AddToPullHistoryRecord(string message, string requestUrl)
		{
		}
	}
	public static class DeviceUtils
	{
		public static string DeviceModel
		{
			get
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				IDeviceInfo instance = ServiceLocator.get_Current().GetInstance<IDeviceInfo>();
				if (!(ServiceLocator.get_Current().GetInstance<IDeviceInfo>().Platform == DevicePlatform.get_Android()))
				{
					return IOSHardwareMapper.GetModel(instance.Model);
				}
				return instance.Model;
			}
		}

		public static string DeviceType
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				if (!(ServiceLocator.get_Current().GetInstance<IDeviceInfo>().Platform == DevicePlatform.get_Android()))
				{
					return "IOS";
				}
				return "Android-Google";
			}
		}

		public static void CleanDataFromDevice()
		{
			ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>().ClearAllFields();
			HttpClientManager.MakeNewInstance();
			ServiceLocator.get_Current().GetInstance<IPreferences>().Clear();
			MessageUtils.RemoveAll();
			OnboardingStatusHelper.Status = OnboardingStatus.NoConsentsGiven;
			foreach (string item in SecureStorageKeys.GetAllKeysForCleaningDevice())
			{
				ServiceLocator.get_Current().GetInstance<SecureStorageService>().Delete(item);
			}
		}

		public static async void StopScanServices()
		{
			try
			{
				await ExposureNotification.StopAsync();
			}
			catch (Exception ex)
			{
				if (!ex.HandleExposureNotificationException("DeviceUtils", "StopScanServices"))
				{
					throw ex;
				}
			}
		}
	}
	public class EncodingUtils
	{
		public static string ConvertByteArrayToString(byte[] array)
		{
			return BitConverter.ToString(array).Replace("-", "");
		}
	}
	public static class FakeGatewayUtils
	{
		public static IEnumerable<ExposureKeyModel> LastPulledExposureKeys = new List<ExposureKeyModel>();

		public static bool IsFakeGatewayTest
		{
			get;
			set;
		} = false;


		private static SelfDiagnosisSubmissionDTO CreateFakeDataForRegions(List<string> regions)
		{
			MoreEnumerable.ForEach<ExposureKeyModel>(LastPulledExposureKeys, (Action<ExposureKeyModel>)delegate(ExposureKeyModel key)
			{
				key.DaysSinceOnsetOfSymptoms = 2;
			});
			return new SelfDiagnosisSubmissionDTO(LastPulledExposureKeys)
			{
				Regions = regions
			};
		}

		public static async Task<ApiResponse> PostKeysToFakeGateway(string region)
		{
			_ = 1;
			try
			{
				IsFakeGatewayTest = true;
				await ExposureNotification.SubmitSelfDiagnosisAsync();
				return await new FakeGatewayWebService().UploadKeys(CreateFakeDataForRegions(new List<string>
				{
					region
				}));
			}
			finally
			{
				IsFakeGatewayTest = false;
			}
		}
	}
	public class HtmlWrapper
	{
		public static string HtmlForLabelWithText(string body, float fontSize, bool bold, bool isAndroid, string textColor = null)
		{
			string style = StyleForLabelWithFontSize(fontSize, textColor);
			return WrapHtmlWithBodyAndStyle(body, style, bold, isAndroid);
		}

		public static string StyleForLabelWithFontSize(float fontSize, string textColor = null)
		{
			string text = "#ffffff";
			return "\n                span.wrap\n                {\n                    display:inline-block;\n                    width:100%;\n                    white-space: nowrap;\n                    overflow:hidden !important;\n                    text-overflow: ellipsis;\n                }\n                body {\n                    margin: 0; \n                    padding: 0;\n                    font-family: Raleway;\n                    font-size: " + fontSize + "px;\n                    text-align: justify;\n                    color: " + ((textColor == null) ? text : textColor) + ";\n                    text-align: left;\n                    -webkit-font-smoothing: none;\n                    -webkit-text-size-adjust: none;\n                }\n                .ios-bold {\n                    font-family: Raleway-Bold;\n                }\n                .draft {\n                    background-color: #B50050;\n                    color: white;\n                    font-size: 11px;\n                    padding: 1px 4px 1px 4px;\n                }\n                ";
		}

		public static string WrapHtmlWithBodyAndStyle(string body, string style, bool bold, bool isAndroid)
		{
			string text = (isAndroid ? "" : "<meta name=\"viewport\" content=\"initial-scale=1.0\" />");
			string text2 = body;
			if (bold)
			{
				text2 = (isAndroid ? ("<strong>" + body + "</strong>") : ("<span class='ios-bold'>" + body + "</span>"));
			}
			else if (isAndroid)
			{
				text2 = text2.Replace("<em>", "<strong>");
				text2 = text2.Replace("</em>", "</strong>");
			}
			else
			{
				text2 = text2.Replace("<em>", "<span class='ios-bold'>");
				text2 = text2.Replace("</em>", "</span>");
			}
			text2 = "<span class=\"wrap\">" + text2 + "</span>";
			return "\n                <html>\n                <head>\n                " + text + "\n                <style type=\"text/css\">\n                " + style + "\n                </style>\n                </head>\n                <body>\n                " + text2 + "\n                </body>\n                </html>            \n                ";
		}
	}
	public static class IOSHardwareMapper
	{
		public static string GetModel(string hardware)
		{
			if (hardware.StartsWith("iPhone"))
			{
				switch (hardware)
				{
				case "iPhone1,1":
					return "iPhone";
				case "iPhone1,2":
					return "iPhone 3G";
				case "iPhone2,1":
					return "iPhone 3GS";
				case "iPhone3,1":
				case "iPhone3,2":
					return "iPhone 4 GSM";
				case "iPhone3,3":
					return "iPhone 4 CDMA";
				case "iPhone4,1":
					return "iPhone 4S";
				case "iPhone5,1":
					return "iPhone 5 GSM";
				case "iPhone5,2":
					return "iPhone 5 Global";
				case "iPhone5,3":
					return "iPhone 5C GSM";
				case "iPhone5,4":
					return "iPhone 5C Global";
				case "iPhone6,1":
					return "iPhone 5S GSM";
				case "iPhone6,2":
					return "iPhone 5S Global";
				case "iPhone7,2":
					return "iPhone 6";
				case "iPhone7,1":
					return "iPhone 6 Plus";
				case "iPhone8,1":
					return "iPhone 6S";
				case "iPhone8,2":
					return "iPhone 6S Plus";
				case "iPhone8,4":
					return "iPhone SE";
				case "iPhone12,8":
					return "iPhone SE (2nd generation)";
				case "iPhone9,1":
				case "iPhone9,3":
					return "iPhone 7";
				case "iPhone9,2":
				case "iPhone9,4":
					return "iPhone 7 Plus";
				case "iPhone10,1":
				case "iPhone10,4":
					return "iPhone 8";
				case "iPhone10,2":
				case "iPhone10,5":
					return "iPhone 8 Plus";
				case "iPhone10,3":
				case "iPhone10,6":
					return "iPhone X";
				case "iPhone11,8":
					return "iPhone XR";
				case "iPhone11,4":
				case "iPhone11,6":
					return "iPhone XS Max";
				case "iPhone11,2":
					return "iPhone XS";
				case "iPhone12,1":
					return "iPhone 11";
				case "iPhone12,3":
					return "iPhone 11 Pro";
				case "iPhone12,5":
					return "iPhone 11 Pro Max";
				}
			}
			if (hardware.StartsWith("iPad"))
			{
				switch (hardware)
				{
				case "iPad1,1":
					return "iPad";
				case "iPad2,1":
					return "iPad 2 Wi-Fi";
				case "iPad2,2":
					return "iPad 2 GSM";
				case "iPad2,3":
					return "iPad 2 CDMA";
				case "iPad2,4":
					return "iPad 2 Wi-Fi";
				case "iPad3,1":
					return "iPad 3 Wi-Fi";
				case "iPad3,2":
					return "iPad 3 Wi-Fi + Cellular (VZ)";
				case "iPad3,3":
					return "iPad 3 Wi-Fi + Cellular";
				case "iPad3,4":
					return "iPad 4 Wi-Fi";
				case "iPad3,5":
					return "iPad 4 Wi-Fi + Cellular";
				case "iPad3,6":
					return "iPad 4 Wi-Fi + Cellular (MM)";
				case "iPad6,11":
					return "iPad 5 Wi-Fi";
				case "iPad6,12":
					return "iPad 5 Wi-Fi + Cellular";
				case "iPad7,5":
					return "iPad 6 Wi-Fi";
				case "iPad7,6":
					return "iPad 6 Wi-Fi + Cellular";
				case "iPad7,11":
					return "iPad 7 Wi-Fi";
				case "iPad7,12":
					return "iPad 7 Wi-Fi + Cellular";
				case "iPad4,1":
					return "iPad Air Wi-Fi";
				case "iPad4,2":
					return "iPad Air Wi-Fi + Cellular";
				case "iPad4,3":
					return "iPad Air Wi-Fi + Cellular (TD-LTE)";
				case "iPad5,3":
					return "iPad Air 2 Wi-Fi";
				case "iPad5,4":
					return "iPad Air 2 Wi-Fi + Cellular";
				case "iPad11,3":
					return "iPad Air 3 Wi-Fi";
				case "iPad11,4":
					return "iPad Air 3 Wi-Fi + Cellular";
				case "iPad2,5":
					return "iPad mini Wi-Fi";
				case "iPad2,6":
					return "iPad mini Wi-Fi + Cellular";
				case "iPad2,7":
					return "iPad mini Wi-Fi + Cellular (MM)";
				case "iPad4,4":
					return "iPad mini 2 Wi-Fi";
				case "iPad4,5":
					return "iPad mini 2 Wi-Fi + Cellular";
				case "iPad4,6":
					return "iPad mini 2 Wi-Fi + Cellular (TD-LTE)";
				case "iPad4,7":
					return "iPad mini 3 Wi-Fi";
				case "iPad4,8":
					return "iPad mini 3 Wi-Fi + Cellular";
				case "iPad4,9":
					return "iPad mini 3 Wi-Fi + Cellular (TD-LTE)";
				case "iPad5,1":
					return "iPad mini 4";
				case "iPad5,2":
					return "iPad mini 4 Wi-Fi + Cellular";
				case "iPad11,1":
					return "iPad mini 5 Wi-Fi";
				case "iPad11,2":
					return "iPad mini 5 Wi-Fi + Cellular";
				case "iPad6,3":
					return "iPad Pro (9.7-inch)";
				case "iPad6,4":
					return "iPad Pro (9.7-inch) Wi-Fi + Cellular";
				case "iPad7,3":
					return "iPad Pro (10.5-inch)";
				case "iPad7,4":
					return "iPad Pro (10.5-inch) Wi-Fi + Cellular";
				case "iPad6,7":
					return "iPad Pro 12.9-inch";
				case "iPad6,8":
					return "iPad Pro 12.9-inch Wi-Fi + Cellular";
				case "iPad7,1":
					return "iPad Pro 12.9-inch (2nd generation)";
				case "iPad7,2":
					return "iPad Pro 12.9-inch (2nd generation) Wi-Fi + Cellular";
				case "iPad8,5":
				case "iPad8,6":
					return "iPad Pro 12.9-inch (3rd generation)";
				case "iPad8,7":
				case "iPad8,8":
					return "iPad Pro 12.9-inch (3rd generation Wi-Fi + Cellular)";
				case "iPad8,11":
					return "iPad Pro 12.9-inch (4th generation)";
				case "iPad8,12":
					return "iPad Pro 12.9-inch (4th generation Wi-Fi + Cellular)";
				case "iPad8,1":
				case "iPad8,2":
					return "iPad Pro 11-inch";
				case "iPad8,3":
				case "iPad8,4":
					return "iPad Pro 11-inch Wi-Fi + Cellular";
				case "iPad8,9":
					return "iPad Pro 11-inch (2nd generation)";
				case "iPad8,10":
					return "iPad Pro 11-inch (2nd generation) Wi-Fi + Cellular";
				}
			}
			switch (hardware)
			{
			case "i386":
			case "x86_64":
				return "Simulator";
			default:
				return hardware;
			case "":
				return "Unknown";
			}
		}
	}
	public static class LogUtils
	{
		private static readonly int _numLogsToSendAtATime = 100;

		private static readonly int _maxNumOfPersistedLogsOnSendError = 200;

		public static void LogMessage(LogSeverity severity, string message, string additionalInfo = "")
		{
			LogSQLiteModel log = new LogSQLiteModel(new LogDeviceDetails(severity, message, additionalInfo));
			ServiceLocator.get_Current().GetInstance<ILoggingManager>().SaveNewLog(log);
		}

		public static void LogException(LogSeverity severity, Exception e, string contextDescription, string additionalInfo = "")
		{
			LogDeviceDetails info = new LogDeviceDetails(severity, contextDescription, additionalInfo);
			LogExceptionDetails e2 = new LogExceptionDetails(e);
			LogSQLiteModel log = new LogSQLiteModel(info, null, e2);
			ServiceLocator.get_Current().GetInstance<ILoggingManager>().SaveNewLog(log);
		}

		public static void LogApiError(LogSeverity severity, ApiResponse apiResponse, bool erroredSilently, string additionalInfo = "", string overwriteMessage = null)
		{
			string logMessage = (overwriteMessage ?? apiResponse.ErrorLogMessage) + (erroredSilently ? " (silent)" : " (error shown)");
			LogDeviceDetails info = new LogDeviceDetails(severity, logMessage, additionalInfo);
			LogApiDetails apiDetails = new LogApiDetails(apiResponse);
			LogExceptionDetails e = null;
			if (apiResponse.Exception != null)
			{
				e = new LogExceptionDetails(apiResponse.Exception);
			}
			LogSQLiteModel log = new LogSQLiteModel(info, apiDetails, e);
			ServiceLocator.get_Current().GetInstance<ILoggingManager>().SaveNewLog(log);
		}

		public static async void SendAllLogs()
		{
			ILoggingManager manager = ServiceLocator.get_Current().GetInstance<ILoggingManager>();
			try
			{
				bool flag = false;
				while (!flag)
				{
					List<LogSQLiteModel> logs = await manager.GetLogs(_numLogsToSendAtATime);
					if (logs == null || !logs.Any())
					{
						break;
					}
					List<LogDTO> dtos = logs.Select((LogSQLiteModel l) => new LogDTO(l)).ToList();
					if (!(await new LoggingService().PostAllLogs(dtos)))
					{
						DeleteLogsIfTooMany();
						break;
					}
					await manager.DeleteLogs(logs);
					flag = logs.Count < _numLogsToSendAtATime;
				}
			}
			catch (Exception ex)
			{
				_ = ex;
				await manager.DeleteAll();
			}
		}

		private static async void DeleteLogsIfTooMany()
		{
			ILoggingManager manager = ServiceLocator.get_Current().GetInstance<ILoggingManager>();
			bool tooManyPersistedLogs = true;
			while (tooManyPersistedLogs)
			{
				List<LogSQLiteModel> source = await manager.GetLogs(_maxNumOfPersistedLogsOnSendError * 2);
				tooManyPersistedLogs = source.Count() > _maxNumOfPersistedLogsOnSendError;
				if (tooManyPersistedLogs)
				{
					int count = source.Count() - _maxNumOfPersistedLogsOnSendError;
					List<LogSQLiteModel> logs = source.Take(count).ToList();
					await manager.DeleteLogs(logs);
				}
			}
		}

		public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e != null && e.ExceptionObject != null)
			{
				Exception ex = e?.ExceptionObject as Exception;
				if (ex != null)
				{
					string contextDescription = "LogUtils.OnUnhandledException: " + (e.IsTerminating ? "Native unhandled crash" : "Native unhandled exception - not crashing");
					LogException((!e.IsTerminating) ? LogSeverity.WARNING : LogSeverity.ERROR, ex, contextDescription);
				}
			}
		}
	}
	public static class MessageUtils
	{
		private static string _logPrefix = "MessageUtils";

		private static SecureStorageService _secureStorageService => ServiceLocator.get_Current().GetInstance<SecureStorageService>();

		private static IMessagesManager _manager => ServiceLocator.get_Current().GetInstance<IMessagesManager>();

		private static string DEFAULT_MESSAGE_TITLE => Extensions.Translate("MESSAGES_MESSAGE_HEADER", Array.Empty<object>());

		private static string DEFAULT_MESSAGE_LINK => Extensions.Translate("MESSAGES_LINK", Array.Empty<object>());

		private static async Task CreateAndSaveNewMessage(object Sender, DateTime? customTime = null, int triggerAfterNSec = 0)
		{
			MessageSQLiteModel log = new MessageSQLiteModel
			{
				IsRead = false,
				MessageLink = DEFAULT_MESSAGE_LINK,
				TimeStamp = (customTime ?? DateTime.Now),
				Title = DEFAULT_MESSAGE_TITLE
			};
			if (!HasCreatedMessageAndNotificationToday())
			{
				await _manager.SaveNewMessage(log);
				MessagingCenter.Send(Sender, MessagingCenterKeys.KEY_MESSAGE_RECEIVED);
				NotificationsHelper.CreateNotification(NotificationsEnum.NewMessageReceived, triggerAfterNSec);
				SaveDateTimeToSecureStorageForKey(SecureStorageKeys.LAST_HIGH_RISK_ALERT_UTC_KEY, customTime ?? SystemTime.Now(), "CreateAndSaveNewMessage");
				SaveDateTimeToSecureStorageForKey(SecureStorageKeys.LAST_SENT_NOTIFICATION_UTC_KEY, customTime ?? SystemTime.Now(), "CreateAndSaveNewMessage");
			}
		}

		public static async Task CreateMessage(object Sender, DateTime? customTime = null, int triggerAfterNSec = 0)
		{
			await CreateAndSaveNewMessage(Sender, customTime, triggerAfterNSec);
		}

		public static async Task<int> SaveMessages(MessageSQLiteModel message)
		{
			return await _manager.SaveNewMessage(message);
		}

		public static async Task<List<MessageSQLiteModel>> GetMessages()
		{
			return (await _manager.GetMessages()).OrderByDescending((MessageSQLiteModel x) => x.TimeStamp).ToList();
		}

		public static async Task<List<MessageSQLiteModel>> GetAllUnreadMessages()
		{
			return await _manager.GetUnreadMessages();
		}

		public static void RemoveAll()
		{
			_manager.DeleteAll();
		}

		public static async Task RemoveMessages(List<MessageSQLiteModel> messages)
		{
			await _manager.DeleteMessages(messages);
		}

		public static async Task RemoveAllOlderThan(int minutes)
		{
			await RemoveMessages((await GetMessages()).FindAll((MessageSQLiteModel message) => DateTime.Now.Subtract(message.TimeStamp).TotalMinutes >= (double)minutes).ToList());
		}

		public static void MarkAsRead(MessageItemViewModel message, bool isRead)
		{
			_manager.MarkAsRead(new MessageSQLiteModel(message), isRead);
		}

		public static List<MessageItemViewModel> ToMessageItemViewModelList(List<MessageSQLiteModel> list)
		{
			return list.Select((MessageSQLiteModel model) => new MessageItemViewModel(model)).ToList();
		}

		public static bool HasCreatedMessageAndNotificationToday()
		{
			return GetDateTimeFromSecureStorageForKey(SecureStorageKeys.LAST_HIGH_RISK_ALERT_UTC_KEY, "HasCreatedMessageAndNotificationToday").Date == SystemTime.Now().Date;
		}

		public static void SaveDateTimeToSecureStorageForKey(string SecureStorageKey, DateTime DateTimeToSave, string CallerMethodToLogError)
		{
			try
			{
				_secureStorageService.SaveValue(SecureStorageKey, DateTimeToSave.ToString());
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, _logPrefix + "." + CallerMethodToLogError);
			}
		}

		public static DateTime GetDateTimeFromSecureStorageForKey(string SecureStorageKey, string CallerMethodToLogError)
		{
			try
			{
				if (_secureStorageService.KeyExists(SecureStorageKey))
				{
					return DateTime.Parse(_secureStorageService.GetValue(SecureStorageKey));
				}
				return SystemTime.Now().AddDays(-100.0);
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, _logPrefix + "." + CallerMethodToLogError);
				return DateTime.UtcNow.AddDays(-100.0);
			}
		}
	}
	public class MessagingCenter : IMessagingCenter
	{
		private class Sender : Tuple<string, Type, Type>
		{
			public Sender(string message, Type senderType, Type argType)
				: base(message, senderType, argType)
			{
			}
		}

		private delegate bool Filter(object sender);

		private class MaybeWeakReference
		{
			private readonly bool _isStrongReference;

			private WeakReference DelegateWeakReference
			{
				get;
			}

			private object DelegateStrongReference
			{
				get;
			}

			public object Target
			{
				get
				{
					if (!_isStrongReference)
					{
						return DelegateWeakReference.Target;
					}
					return DelegateStrongReference;
				}
			}

			public bool IsAlive
			{
				get
				{
					if (!_isStrongReference)
					{
						return DelegateWeakReference.IsAlive;
					}
					return true;
				}
			}

			public MaybeWeakReference(object subscriber, object delegateSource)
			{
				if (subscriber.Equals(delegateSource))
				{
					DelegateWeakReference = new WeakReference(delegateSource);
					_isStrongReference = false;
				}
				else
				{
					DelegateStrongReference = delegateSource;
					_isStrongReference = true;
				}
			}
		}

		private class Subscription : Tuple<WeakReference, MaybeWeakReference, MethodInfo, Filter>
		{
			public WeakReference Subscriber => base.Item1;

			private MaybeWeakReference DelegateSource => base.Item2;

			private MethodInfo MethodInfo => base.Item3;

			private Filter Filter => base.Item4;

			public Subscription(object subscriber, object delegateSource, MethodInfo methodInfo, Filter filter)
				: base(new WeakReference(subscriber), new MaybeWeakReference(subscriber, delegateSource), methodInfo, filter)
			{
			}

			public void InvokeCallback(object sender, object args)
			{
				if (!Filter(sender))
				{
					return;
				}
				if (MethodInfo.IsStatic)
				{
					MethodInfo.Invoke(null, (MethodInfo.GetParameters().Length != 1) ? new object[2]
					{
						sender,
						args
					} : new object[1]
					{
						sender
					});
					return;
				}
				object target = DelegateSource.Target;
				if (target != null)
				{
					MethodInfo.Invoke(target, (MethodInfo.GetParameters().Length != 1) ? new object[2]
					{
						sender,
						args
					} : new object[1]
					{
						sender
					});
				}
			}

			public bool CanBeRemoved()
			{
				if (Subscriber.IsAlive)
				{
					return !DelegateSource.IsAlive;
				}
				return true;
			}
		}

		private readonly Dictionary<Sender, List<Subscription>> _subscriptions = new Dictionary<Sender, List<Subscription>>();

		public static IMessagingCenter Instance
		{
			get;
		} = new MessagingCenter();


		public static void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class
		{
			Instance.Send(sender, message, args);
		}

		void IMessagingCenter.Send<TSender, TArgs>(TSender sender, string message, TArgs args)
		{
			if (sender == null)
			{
				throw new ArgumentNullException("sender");
			}
			InnerSend(message, typeof(TSender), typeof(TArgs), sender, args);
		}

		public static void Send<TSender>(TSender sender, string message) where TSender : class
		{
			Instance.Send(sender, message);
		}

		void IMessagingCenter.Send<TSender>(TSender sender, string message)
		{
			if (sender == null)
			{
				throw new ArgumentNullException("sender");
			}
			InnerSend(message, typeof(TSender), null, sender, null);
		}

		public static void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class
		{
			Instance.Subscribe(subscriber, message, callback, source);
		}

		void IMessagingCenter.Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source)
		{
			if (subscriber == null)
			{
				throw new ArgumentNullException("subscriber");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			object target = callback.Target;
			Filter filter = delegate(object sender)
			{
				TSender val = (TSender)sender;
				return source == null || val == source;
			};
			InnerSubscribe(subscriber, message, typeof(TSender), typeof(TArgs), target, callback.GetMethodInfo(), filter);
		}

		public static void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class
		{
			Instance.Subscribe(subscriber, message, callback, source);
		}

		void IMessagingCenter.Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source)
		{
			if (subscriber == null)
			{
				throw new ArgumentNullException("subscriber");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			object target = callback.Target;
			Filter filter = delegate(object sender)
			{
				TSender val = (TSender)sender;
				return source == null || val == source;
			};
			InnerSubscribe(subscriber, message, typeof(TSender), null, target, callback.GetMethodInfo(), filter);
		}

		public static void Unsubscribe<TSender, TArgs>(object subscriber, string message) where TSender : class
		{
			Instance.Unsubscribe<TSender, TArgs>(subscriber, message);
		}

		void IMessagingCenter.Unsubscribe<TSender, TArgs>(object subscriber, string message)
		{
			InnerUnsubscribe(message, typeof(TSender), typeof(TArgs), subscriber);
		}

		public static void Unsubscribe<TSender>(object subscriber, string message) where TSender : class
		{
			Instance.Unsubscribe<TSender>(subscriber, message);
		}

		void IMessagingCenter.Unsubscribe<TSender>(object subscriber, string message)
		{
			InnerUnsubscribe(message, typeof(TSender), null, subscriber);
		}

		private void InnerSend(string message, Type senderType, Type argType, object sender, object args)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			Sender key = new Sender(message, senderType, argType);
			if (!_subscriptions.ContainsKey(key))
			{
				return;
			}
			List<Subscription> list = _subscriptions[key];
			if (list == null || !list.Any())
			{
				return;
			}
			foreach (Subscription item in list.ToList())
			{
				if (item.Subscriber.Target != null && list.Contains(item))
				{
					item.InvokeCallback(sender, args);
				}
			}
		}

		private void InnerSubscribe(object subscriber, string message, Type senderType, Type argType, object target, MethodInfo methodInfo, Filter filter)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			Sender key = new Sender(message, senderType, argType);
			Subscription item = new Subscription(subscriber, target, methodInfo, filter);
			if (_subscriptions.ContainsKey(key))
			{
				_subscriptions[key].Add(item);
				return;
			}
			List<Subscription> value = new List<Subscription>
			{
				item
			};
			_subscriptions[key] = value;
		}

		private void InnerUnsubscribe(string message, Type senderType, Type argType, object subscriber)
		{
			if (subscriber == null)
			{
				throw new ArgumentNullException("subscriber");
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			Sender key = new Sender(message, senderType, argType);
			if (_subscriptions.ContainsKey(key))
			{
				_subscriptions[key].RemoveAll((Subscription sub) => sub.CanBeRemoved() || sub.Subscriber.Target == subscriber);
				if (!_subscriptions[key].Any())
				{
					_subscriptions.Remove(key);
				}
			}
		}

		internal static void ClearSubscribers()
		{
			(Instance as MessagingCenter)?._subscriptions.Clear();
		}
	}
	public static class MessagingCenterKeys
	{
		public static string KEY_FORCE_UPDATE => "KEY_FORCE_UPDATE";

		public static string KEY_APP_RETURNS_FROM_BACKGROUND => "KEY_APP_RETURNS_FROM_BACKGROUND";

		public static string KEY_APP_WILL_ENTER_BACKGROUND => "KEY_APP_WILL_ENTER_BACKGROUND";

		public static string KEY_MESSAGE_RECEIVED => "KEY_MESSAGE_RECEIVED";

		public static string KEY_PERMISSIONS_CHANGED => "PermissionsChangedKey";
	}
	public class NotificationsHelper
	{
		public static readonly ILocalNotificationsManager LocalNotificationsManager = ServiceLocator.get_Current().GetInstance<ILocalNotificationsManager>();

		public static void CreateNotification(NotificationsEnum notificationType, int triggerInSeconds)
		{
			LocalNotificationsManager.GenerateLocalNotification(notificationType.Data(), triggerInSeconds);
		}

		public static void CreateNotificationOnlyIfInBackground(NotificationsEnum notificationType)
		{
			LocalNotificationsManager.GenerateLocalNotificationOnlyIfInBackground(notificationType.Data());
		}
	}
	public static class OnboardingStatusHelper
	{
		public static OnboardingStatus Status
		{
			get
			{
				if (!LocalPreferencesHelper.IsOnboardingCompleted && !LocalPreferencesHelper.IsOnboardingCountriesCompleted)
				{
					return OnboardingStatus.NoConsentsGiven;
				}
				if (LocalPreferencesHelper.IsOnboardingCompleted && !LocalPreferencesHelper.IsOnboardingCountriesCompleted)
				{
					return OnboardingStatus.OnlyMainOnboardingCompleted;
				}
				return OnboardingStatus.CountriesOnboardingCompleted;
			}
			set
			{
				switch (value)
				{
				case OnboardingStatus.NoConsentsGiven:
					LocalPreferencesHelper.IsOnboardingCompleted = false;
					LocalPreferencesHelper.IsOnboardingCountriesCompleted = false;
					break;
				case OnboardingStatus.OnlyMainOnboardingCompleted:
					LocalPreferencesHelper.IsOnboardingCompleted = true;
					LocalPreferencesHelper.IsOnboardingCountriesCompleted = false;
					break;
				case OnboardingStatus.CountriesOnboardingCompleted:
					LocalPreferencesHelper.IsOnboardingCompleted = true;
					LocalPreferencesHelper.IsOnboardingCountriesCompleted = true;
					break;
				}
			}
		}
	}
}
namespace NDB.Covid19.SecureStorage
{
	public class SecureStorageService : ISecureStorageService
	{
		private ISecureStorage _secureStorage;

		public ISecureStorage SecureStorage
		{
			get
			{
				if (_secureStorage == null)
				{
					_secureStorage = CrossSecureStorage.get_Current();
				}
				return _secureStorage;
			}
		}

		public bool SaveValue(string key, string value)
		{
			return SecureStorage.SetValue(key, value);
		}

		public string GetValue(string key)
		{
			try
			{
				return SecureStorage.GetValue(key, (string)null);
			}
			catch (Exception)
			{
				SecureStorage.DeleteKey(key);
				return null;
			}
		}

		public bool KeyExists(string key)
		{
			return SecureStorage.HasKey(key);
		}

		public void Delete(string key)
		{
			if (KeyExists(key))
			{
				SecureStorage.DeleteKey(key);
			}
		}

		public void SetSecureStorageInstance(ISecureStorage instance)
		{
			_secureStorage = instance;
		}
	}
}
namespace NDB.Covid19.PersistedData
{
	public class LocalPreferencesHelper
	{
		private static IPreferences _preferences => ServiceLocator.get_Current().GetInstance<IPreferences>();

		public static int MigrationCount
		{
			get
			{
				return _preferences.Get(PreferencesKeys.MIGRATION_COUNT, 0);
			}
			set
			{
				_preferences.Set(PreferencesKeys.MIGRATION_COUNT, value);
			}
		}

		public static bool IsOnboardingCompleted
		{
			get
			{
				return _preferences.Get(PreferencesKeys.IS_ONBOARDING_COMPLETED_PREF, defaultValue: false);
			}
			set
			{
				_preferences.Set(PreferencesKeys.IS_ONBOARDING_COMPLETED_PREF, value);
			}
		}

		public static bool IsOnboardingCountriesCompleted
		{
			get
			{
				return _preferences.Get(PreferencesKeys.IS_ONBOARDING_COUNTRIES_COMPLETED_PREF, defaultValue: false);
			}
			set
			{
				_preferences.Set(PreferencesKeys.IS_ONBOARDING_COUNTRIES_COMPLETED_PREF, value);
			}
		}

		public static int LastPullKeysBatchNumberNotSubmitted
		{
			get
			{
				return _preferences.Get(PreferencesKeys.LAST_PULLED_BATCH_NUMBER_NOT_SUBMITTED, 0);
			}
			set
			{
				_preferences.Set(PreferencesKeys.LAST_PULLED_BATCH_NUMBER_NOT_SUBMITTED, value);
			}
		}

		public static int LastPullKeysBatchNumberSuccessfullySubmitted
		{
			get
			{
				return _preferences.Get(PreferencesKeys.LAST_PULLED_BATCH_NUMBER_SUBMITTED, 0);
			}
			set
			{
				_preferences.Set(PreferencesKeys.LAST_PULLED_BATCH_NUMBER_SUBMITTED, value);
			}
		}

		public static BatchType LastPulledBatchType
		{
			get
			{
				return _preferences.Get(PreferencesKeys.LAST_PULLED_BATCH_TYPE, "dk").ToBatchType();
			}
			set
			{
				_preferences.Set(PreferencesKeys.LAST_PULLED_BATCH_TYPE, value.ToTypeString());
			}
		}

		public static bool TermsNotificationWasShown
		{
			get
			{
				return _preferences.Get(PreferencesKeys.TERMS_NOTIFICATION_WAS_SENT, defaultValue: false);
			}
			set
			{
				_preferences.Set(PreferencesKeys.TERMS_NOTIFICATION_WAS_SENT, value);
			}
		}

		public static double ExposureTimeThreshold
		{
			get
			{
				return _preferences.Get(PreferencesKeys.EXPOSURE_TIME_THRESHOLD, Conf.EXPOSURE_TIME_THRESHOLD);
			}
			set
			{
				_preferences.Set(PreferencesKeys.EXPOSURE_TIME_THRESHOLD, value);
			}
		}

		public static double LowAttenuationDurationMultiplier
		{
			get
			{
				return _preferences.Get(PreferencesKeys.LOW_ATTENUATION_DURATION_MULTIPLIER, Conf.LOW_ATTENUATION_DURATION_MULTIPLIER);
			}
			set
			{
				_preferences.Set(PreferencesKeys.LOW_ATTENUATION_DURATION_MULTIPLIER, value);
			}
		}

		public static double MiddleAttenuationDurationMultiplier
		{
			get
			{
				return _preferences.Get(PreferencesKeys.MIDDLE_ATTENUATION_DURATION_MULTIPLIER, Conf.MIDDLE_ATTENUATION_DURATION_MULTIPLIER);
			}
			set
			{
				_preferences.Set(PreferencesKeys.MIDDLE_ATTENUATION_DURATION_MULTIPLIER, value);
			}
		}

		public static double HighAttenuationDurationMultiplier
		{
			get
			{
				return _preferences.Get(PreferencesKeys.HIGH_ATTENUATION_DURATION_MULTIPLIER, Conf.HIGH_ATTENUATION_DURATION_MULTIPLIER);
			}
			set
			{
				_preferences.Set(PreferencesKeys.HIGH_ATTENUATION_DURATION_MULTIPLIER, value);
			}
		}

		public static bool GetIsDownloadWithMobileDataEnabled()
		{
			return _preferences.Get(PreferencesKeys.USE_MOBILE_DATA_PREF, defaultValue: true);
		}

		public static void SetIsDownloadWithMobileDataEnabled(bool isDownloadWithMobileDataEnabled)
		{
			_preferences.Set(PreferencesKeys.USE_MOBILE_DATA_PREF, isDownloadWithMobileDataEnabled);
		}

		public static DateTime GetUpdatedDateTime()
		{
			return _preferences.Get(PreferencesKeys.MESSAGES_LAST_UPDATED_PREF, DateTime.MinValue);
		}

		public static void UpdateLastUpdatedDate()
		{
			_preferences.Set(PreferencesKeys.MESSAGES_LAST_UPDATED_PREF, SystemTime.Now());
		}

		public static DateTime GetLastPullKeysSucceededDateTime()
		{
			return _preferences.Get(PreferencesKeys.LAST_PULL_KEYS_SUCCEEDED_DATE_TIME, DateTime.MinValue);
		}

		public static void UpdateLastPullKeysSucceededDateTime()
		{
			_preferences.Set(PreferencesKeys.LAST_PULL_KEYS_SUCCEEDED_DATE_TIME, SystemTime.Now());
			LastPullKeysBatchNumberSuccessfullySubmitted = LastPullKeysBatchNumberNotSubmitted;
		}

		public static string GetAppLanguage()
		{
			return _preferences.Get(PreferencesKeys.APP_LANGUAGE, null);
		}

		public static void SetAppLanguage(string language)
		{
			_preferences.Set(PreferencesKeys.APP_LANGUAGE, language);
		}
	}
}
namespace NDB.Covid19.PersistedData.SQLite
{
	public interface IMessagesManager
	{
		Task<int> SaveNewMessage(MessageSQLiteModel log);

		Task<List<MessageSQLiteModel>> GetMessages();

		Task<List<MessageSQLiteModel>> GetUnreadMessages();

		Task DeleteMessages(List<MessageSQLiteModel> logs);

		Task DeleteAll();

		Task MarkAsRead(MessageSQLiteModel message, bool isRead);
	}
	public interface ILoggingManager
	{
		void SaveNewLog(LogSQLiteModel log);

		Task<List<LogSQLiteModel>> GetLogs(int amount);

		Task DeleteLogs(List<LogSQLiteModel> logs);

		Task DeleteAll();
	}
	public class LoggingSQLiteManager : ILoggingManager
	{
		private readonly SQLiteAsyncConnection _database;

		private static readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

		public LoggingSQLiteManager()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Conf.DB_NAME);
			_database = new SQLiteAsyncConnection(text, true);
			_database.CreateTableAsync<LogSQLiteModel>((CreateFlags)0).Wait();
		}

		public async void SaveNewLog(LogSQLiteModel log)
		{
			await _syncLock.WaitAsync();
			try
			{
				await _database.InsertAsync((object)log);
				_ = log.ExceptionMessage;
			}
			catch (Exception)
			{
			}
			finally
			{
				_syncLock.Release();
			}
		}

		public async Task<List<LogSQLiteModel>> GetLogs(int amount)
		{
			await _syncLock.WaitAsync();
			try
			{
				return await _database.Table<LogSQLiteModel>().Take(amount).ToListAsync();
			}
			catch (Exception)
			{
				return new List<LogSQLiteModel>();
			}
			finally
			{
				_syncLock.Release();
			}
		}

		public async Task DeleteLogs(List<LogSQLiteModel> logs)
		{
			await _syncLock.WaitAsync();
			try
			{
				foreach (LogSQLiteModel log in logs)
				{
					await _database.Table<LogSQLiteModel>().DeleteAsync((Expression<Func<LogSQLiteModel, bool>>)((LogSQLiteModel it) => it.ID == log.ID));
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				_syncLock.Release();
			}
		}

		public async Task DeleteAll()
		{
			await _syncLock.WaitAsync();
			try
			{
				await _database.DeleteAllAsync<LogSQLiteModel>();
			}
			catch (Exception)
			{
			}
			finally
			{
				_syncLock.Release();
			}
		}
	}
	public class MessagesManager : IMessagesManager
	{
		private readonly SQLiteAsyncConnection _database;

		private static readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

		public MessagesManager()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			_database = new SQLiteAsyncConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Conf.DB_NAME), true);
			_database.CreateTableAsync<MessageSQLiteModel>((CreateFlags)0).Wait();
		}

		public async Task<int> SaveNewMessage(MessageSQLiteModel message)
		{
			await _syncLock.WaitAsync();
			try
			{
				return await _database.InsertAsync((object)message);
			}
			finally
			{
				_syncLock.Release();
			}
		}

		public async Task<List<MessageSQLiteModel>> GetMessages()
		{
			await _syncLock.WaitAsync();
			try
			{
				return await _database.Table<MessageSQLiteModel>().ToListAsync();
			}
			catch
			{
				return new List<MessageSQLiteModel>();
			}
			finally
			{
				_syncLock.Release();
			}
		}

		public async Task<List<MessageSQLiteModel>> GetUnreadMessages()
		{
			await _syncLock.WaitAsync();
			try
			{
				return await _database.Table<MessageSQLiteModel>().Where((Expression<Func<MessageSQLiteModel, bool>>)((MessageSQLiteModel message) => message.IsRead == false)).ToListAsync();
			}
			catch
			{
				return new List<MessageSQLiteModel>();
			}
			finally
			{
				_syncLock.Release();
			}
		}

		public async Task DeleteMessages(List<MessageSQLiteModel> messages)
		{
			await _syncLock.WaitAsync();
			try
			{
				foreach (MessageSQLiteModel message in messages)
				{
					await _database.Table<MessageSQLiteModel>().DeleteAsync((Expression<Func<MessageSQLiteModel, bool>>)((MessageSQLiteModel it) => it.ID == message.ID));
				}
			}
			finally
			{
				_syncLock.Release();
			}
		}

		public async Task DeleteAll()
		{
			await _syncLock.WaitAsync();
			try
			{
				await _database.DeleteAllAsync<MessageSQLiteModel>();
			}
			finally
			{
				_syncLock.Release();
			}
		}

		public async Task MarkAsRead(MessageSQLiteModel message, bool isRead)
		{
			await _syncLock.WaitAsync();
			try
			{
				message.IsRead = isRead;
				await _database.UpdateAsync((object)message);
			}
			finally
			{
				_syncLock.Release();
			}
		}
	}
}
namespace NDB.Covid19.PersistedData.SecureStorage
{
	public static class SecureStorageKeys
	{
		public static readonly string LAST_HIGH_RISK_ALERT_UTC_KEY = "LAST_HIGH_RISK_ALERT_UTC_KEY";

		public static readonly string LAST_SENT_NOTIFICATION_UTC_KEY = "LAST_SENT_NOTIFICATION_UTC_KEY";

		public static readonly string LAST_SUMMARY_KEY = "LAST_SUMMARY_KEY";

		[Obsolete]
		public static readonly string LAST_MEDIUM_RISK_ALERT_UTC_KEY = "LAST_MEDIUM_RISK_ALERT_UTC_KEY";

		public static IEnumerable<string> GetAllKeysForCleaningDevice()
		{
			return from field in typeof(SecureStorageKeys).GetFields()
				select field.GetValue(typeof(SecureStorageKeys)) into field
				where field.GetType() == typeof(string)
				select field.ToString();
		}
	}
}
namespace NDB.Covid19.OAuth2
{
	public class AuthenticationManager
	{
		public EventHandler<AuthenticatorCompletedEventArgs> _completedHandler;

		public EventHandler<AuthenticatorErrorEventArgs> _errorHandler;

		public static JsonSerializer JsonSerializer = new JsonSerializer();

		public void Setup(EventHandler<AuthenticatorCompletedEventArgs> completedHandler, EventHandler<AuthenticatorErrorEventArgs> errorHandler)
		{
			AuthenticationState.Authenticator = new CustomOAuth2Authenticator(OAuthConf.OAUTH2_CLIENT_ID, null, OAuthConf.OAUTH2_SCOPE, new Uri(OAuthConf.OAUTH2_AUTHORISE_URL), new Uri(OAuthConf.OAUTH2_REDIRECT_URL), new Uri(OAuthConf.OAUTH2_ACCESSTOKEN_URL), null, isUsingNativeUI: true);
			((WebAuthenticator)AuthenticationState.Authenticator).set_ClearCookiesBeforeLogin(true);
			((Authenticator)AuthenticationState.Authenticator).set_ShowErrors(true);
			((Authenticator)AuthenticationState.Authenticator).set_AllowCancel(true);
			_completedHandler = completedHandler;
			((Authenticator)AuthenticationState.Authenticator).add_Completed(_completedHandler);
			_errorHandler = errorHandler;
			((Authenticator)AuthenticationState.Authenticator).add_Error(_errorHandler);
		}

		public void Cleanup()
		{
			if (AuthenticationState.Authenticator != null)
			{
				((Authenticator)AuthenticationState.Authenticator).remove_Completed(_completedHandler);
				((Authenticator)AuthenticationState.Authenticator).remove_Error(_errorHandler);
			}
			AuthenticationState.Authenticator = null;
		}

		public PersonalDataModel GetPayloadValidateJWTToken(string accessToken)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			try
			{
				byte[] rawData = Convert.FromBase64String(OAuthConf.OAUTH2_VERIFY_TOKEN_PUBLIC_KEY);
				JObject val = JObject.Parse(new JwtBuilder().WithAlgorithm((IJwtAlgorithm)new RS256Algorithm(new X509Certificate2(rawData))).MustVerifySignature().Decode(accessToken));
				PersonalDataModel personalDataModel = new PersonalDataModel();
				if (val != null)
				{
					personalDataModel = ((JToken)val).ToObject<PersonalDataModel>(JsonSerializer);
				}
				personalDataModel.Access_token = accessToken;
				return personalDataModel;
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, "AuthenticationManager.GetPayloadValidateJWTToken failed.");
				return null;
			}
		}
	}
	public static class AuthenticationState
	{
		public static CustomOAuth2Authenticator Authenticator;

		public static PersonalDataModel PersonalData;
	}
	public class CustomOAuth2Authenticator : OAuth2Authenticator
	{
		private Uri _accessTokenUrl;

		private string _redirectUrl;

		private string _codeVerifier;

		public CustomOAuth2Authenticator(string clientId, string scope, Uri authorizeUrl, Uri redirectUrl, GetUsernameAsyncFunc getUsernameAsync = null, bool isUsingNativeUI = false)
			: this(clientId, scope, authorizeUrl, redirectUrl, getUsernameAsync, isUsingNativeUI)
		{
		}

		public CustomOAuth2Authenticator(string clientId, string clientSecret, string scope, Uri authorizeUrl, Uri redirectUrl, Uri accessTokenUrl, GetUsernameAsyncFunc getUsernameAsync = null, bool isUsingNativeUI = false)
			: this(clientId, clientSecret, scope, authorizeUrl, redirectUrl, accessTokenUrl, getUsernameAsync, isUsingNativeUI)
		{
			_accessTokenUrl = accessTokenUrl;
		}

		public override void OnPageLoading(Uri url)
		{
			((WebRedirectAuthenticator)this).OnPageLoading(url);
		}

		protected override async void OnRedirectPageLoaded(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
		{
			query["code_verifier"] = _codeVerifier;
			query["client_id"] = ((OAuth2Authenticator)this).get_ClientId();
			query["grant_type"] = "authorization_code";
			query["redirect_uri"] = _redirectUrl;
			try
			{
				foreach (KeyValuePair<string, string> item in await CustomRequestAccessTokenAsync(query))
				{
					fragment.Add(item);
				}
				<>n__0(url, query, fragment);
			}
			catch (AuthException val)
			{
				AuthException val2 = val;
				LogUtils.LogException(LogSeverity.ERROR, (Exception)(object)val2, "CustomOAuth2Authenticator-OnRedirectPageLoaded threw an exception");
				((Authenticator)this).OnError((Exception)(object)val2);
			}
		}

		public async Task<IDictionary<string, string>> CustomRequestAccessTokenAsync(IDictionary<string, string> queryValues)
		{
			FormUrlEncodedContent content = new FormUrlEncodedContent(queryValues);
			HttpResponseMessage response = await new HttpClient().PostAsync(_accessTokenUrl, content).ConfigureAwait(continueOnCapturedContext: false);
			string text = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				if (response.StatusCode != HttpStatusCode.OK)
				{
					LogUtils.LogMessage(LogSeverity.WARNING, "CustomOAuth2Authenticator failed to refresh token.", "Error from service: " + text);
				}
			}
			catch
			{
			}
			IDictionary<string, string> dictionary;
			if (!text.Contains("{"))
			{
				dictionary = WebEx.FormDecode(text);
			}
			else
			{
				IDictionary<string, string> dictionary2 = WebEx.JsonDecode(text);
				dictionary = dictionary2;
			}
			IDictionary<string, string> dictionary3 = dictionary;
			if (dictionary3.ContainsKey("error"))
			{
				throw new AuthException("Error authenticating: " + dictionary3["error"]);
			}
			if (!dictionary3.ContainsKey(((OAuth2Authenticator)this).get_AccessTokenName()))
			{
				throw new AuthException("Expected " + ((OAuth2Authenticator)this).get_AccessTokenName() + " in access token response, but did not receive one.");
			}
			return dictionary3;
		}

		protected override void OnCreatingInitialUrl(IDictionary<string, string> query)
		{
			_redirectUrl = Uri.UnescapeDataString(query["redirect_uri"]);
			_codeVerifier = CreateCodeVerifier();
			query["response_type"] = "code";
			query["nonce"] = Guid.NewGuid().ToString("N");
			query["code_challenge"] = CreateChallenge(_codeVerifier);
			query["code_challenge_method"] = "S256";
			((OAuth2Authenticator)this).OnCreatingInitialUrl(query);
		}

		private string CreateCodeVerifier()
		{
			return Convert.ToBase64String(WinRTCrypto.get_CryptographicBuffer().GenerateRandom(64)).Replace("+", "-").Replace("/", "_")
				.Replace("=", "");
		}

		private string CreateChallenge(string code)
		{
			byte[] array = WinRTCrypto.get_HashAlgorithmProvider().OpenAlgorithm((HashAlgorithm)2).HashData(WinRTCrypto.get_CryptographicBuffer().CreateFromByteArray(Encoding.UTF8.GetBytes(code)));
			byte[] inArray = default(byte[]);
			WinRTCrypto.get_CryptographicBuffer().CopyToByteArray(array, ref inArray);
			return Convert.ToBase64String(inArray).Replace("+", "-").Replace("/", "_")
				.Replace("=", "");
		}
	}
}
namespace NDB.Covid19.ViewModels
{
	public class ConsentViewModel
	{
		public class ConsentSectionTexts
		{
			public string Title
			{
				get;
				private set;
			}

			public string Paragraph
			{
				get;
				private set;
			}

			public string ParagraphAccessibilityText
			{
				get;
				private set;
			}

			public ConsentSectionTexts(string title, string paragraph, string paragraphAccessibilityText)
			{
				Title = title;
				Paragraph = paragraph;
				ParagraphAccessibilityText = paragraphAccessibilityText;
			}
		}

		public bool ConsentIsGiven;

		public static string WELCOME_PAGE_CONSENT_TITLE => Extensions.Translate("WELCOME_PAGE_FIVE_TITLE", Array.Empty<object>());

		public static string CONSENT_ONE_TITLE => Extensions.Translate("CONSENT_ONE_TITLE", Array.Empty<object>());

		public static string CONSENT_ONE_PARAGRAPH => Extensions.Translate("CONSENT_ONE_PARAGRAPH", Array.Empty<object>());

		public static string CONSENT_TWO_TITLE => Extensions.Translate("CONSENT_TWO_TITLE", Array.Empty<object>());

		public static string CONSENT_TWO_PARAGRAPH => Extensions.Translate("CONSENT_TWO_PARAGRAPH", Array.Empty<object>());

		public static string CONSENT_THREE_TITLE => Extensions.Translate("CONSENT_THREE_TITLE", Array.Empty<object>());

		public static string CONSENT_THREE_PARAGRAPH => Extensions.Translate("CONSENT_THREE_PARAGRAPH", Array.Empty<object>());

		public static string CONSENT_FOUR_TITLE => Extensions.Translate("CONSENT_FOUR_TITLE", Array.Empty<object>());

		public static string CONSENT_FOUR_PARAGRAPH => Extensions.Translate("CONSENT_FOUR_PARAGRAPH", Array.Empty<object>());

		public static string CONSENT_FIVE_TITLE => Extensions.Translate("CONSENT_FIVE_TITLE", Array.Empty<object>());

		public static string CONSENT_FIVE_PARAGRAPH => Extensions.Translate("CONSENT_FIVE_PARAGRAPH", Array.Empty<object>());

		public static string CONSENT_SIX_TITLE => Extensions.Translate("CONSENT_SIX_TITLE", Array.Empty<object>());

		public static string CONSENT_SIX_PARAGRAPH => Extensions.Translate("CONSENT_SIX_PARAGRAPH", Array.Empty<object>());

		public static string CONSENT_SEVEN_TITLE => Extensions.Translate("CONSENT_SEVEN_TITLE", Array.Empty<object>());

		public static string CONSENT_SEVEN_PARAGRAPH => Extensions.Translate("CONSENT_SEVEN_PARAGRAPH", Array.Empty<object>());

		public static string CONSENT_SEVEN_BUTTON_TEXT => Extensions.Translate("CONSENT_SEVEN_BUTTON_TEXT", Array.Empty<object>());

		public static string CONSENT_SEVEN_BUTTON_URL => Extensions.Translate("CONSENT_SEVEN_BUTTON_URL", Array.Empty<object>());

		public static string CONSENT_EIGHT_TITLE => Extensions.Translate("CONSENT_EIGHT_TITLE", Array.Empty<object>());

		public static string CONSENT_EIGHT_PARAGRAPH => Extensions.Translate("CONSENT_EIGHT_PARAGRAPH", Array.Empty<object>());

		public static string CONSENT_NINE_TITLE => Extensions.Translate("CONSENT_NINE_TITLE", Array.Empty<object>());

		public static string CONSENT_NINE_PARAGRAPH => Extensions.Translate("CONSENT_NINE_PARAGRAPH", Array.Empty<object>());

		public static string CONSENT_REMOVE_TITLE => Extensions.Translate("CONSENT_REMOVE_TITLE", Array.Empty<object>());

		public static string CONSENT_REMOVE_MESSAGE => Extensions.Translate("CONSENT_REMOVE_MESSAGE", Array.Empty<object>());

		public static string CONSENT_OK_BUTTON_TEXT => Extensions.Translate("CONSENT_OK_BUTTON_TEXT", Array.Empty<object>());

		public static string CONSENT_NO_BUTTON_TEXT => Extensions.Translate("CONSENT_NO_BUTTON_TEXT", Array.Empty<object>());

		public static string GIVE_CONSENT_TEXT => Extensions.Translate("CONSENT_GIVE_CONSENT", Array.Empty<object>());

		public static string WITHDRAW_CONSENT_BUTTON_TEXT => Extensions.Translate("CONSENT_WITHDRAW_BUTTON_TEXT", Array.Empty<object>());

		public static string WITHDRAW_CONSENT_SUCCESS_TITLE => Extensions.Translate("CONSENT_WITHDRAW_SUCCES_TITLE", Array.Empty<object>());

		public static string WITHDRAW_CONSENT_SUCCESS_TEXT => Extensions.Translate("CONSENT_WITHDRAW_SUCCES_BODY", Array.Empty<object>());

		public static string SWITCH_ACCESSIBILITY_CONSENT_SWITCH_DESCRIPTOR => Extensions.Translate("WELCOME_PAGE_FIVE_ACCESSIBILITY_CONSENT_SWITCH", Array.Empty<object>());

		public static string SWITCH_ACCESSIBILITY_ANNOUNCEMENT_CONSENT_GIVEN => Extensions.Translate("WELCOME_PAGE_FIVE_SWITCH_ACCESSIBILITY_ANNOUNCEMENT_CONSENT_GIVEN", Array.Empty<object>());

		public static string SWITCH_ACCESSIBILITY_ANNOUNCEMENT_CONSENT_NOT_GIVEN => Extensions.Translate("WELCOME_PAGE_FIVE_SWITCH_ACCESSIBILITY_ANNOUNCEMENT_CONSENT_NOT_GIVEN", Array.Empty<object>());

		public static string CONSENT_THREE_PARAGRAPH_ACCESSIBILITY => Extensions.Translate("CONSENT_THREE_PARAGRAPH_ACCESSIBILITY", Array.Empty<object>());

		public static string CONSENT_REQUIRED => Extensions.Translate("CONSENT_REQUIRED", Array.Empty<object>());

		public static void OpenPrivacyPolicyLink()
		{
			try
			{
				ServiceLocator.get_Current().GetInstance<IBrowser>().OpenAsync(CONSENT_SEVEN_BUTTON_URL);
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, "Failed to open Privacy policy");
			}
		}

		public List<ConsentSectionTexts> GetConsentSectionsTexts()
		{
			return new List<ConsentSectionTexts>
			{
				new ConsentSectionTexts(CONSENT_ONE_TITLE, CONSENT_ONE_PARAGRAPH, null),
				new ConsentSectionTexts(CONSENT_TWO_TITLE, CONSENT_TWO_PARAGRAPH, null),
				new ConsentSectionTexts(CONSENT_THREE_TITLE, CONSENT_THREE_PARAGRAPH, CONSENT_THREE_PARAGRAPH_ACCESSIBILITY),
				new ConsentSectionTexts(CONSENT_FOUR_TITLE, CONSENT_FOUR_PARAGRAPH, null),
				new ConsentSectionTexts(CONSENT_FIVE_TITLE, CONSENT_FIVE_PARAGRAPH, null),
				new ConsentSectionTexts(CONSENT_SIX_TITLE, CONSENT_SIX_PARAGRAPH, null),
				new ConsentSectionTexts(CONSENT_SEVEN_TITLE, CONSENT_SEVEN_PARAGRAPH, CONSENT_SEVEN_PARAGRAPH.Replace("|", "")),
				new ConsentSectionTexts(CONSENT_EIGHT_TITLE, CONSENT_EIGHT_PARAGRAPH, null),
				new ConsentSectionTexts(CONSENT_NINE_TITLE, CONSENT_NINE_PARAGRAPH, null)
			};
		}
	}
	public class CountryDetailsViewModel
	{
		public string Name
		{
			get;
			set;
		}

		public string Code
		{
			get;
			set;
		}

		public bool Checked
		{
			get;
			set;
		}
	}
	public class DialogViewModel
	{
		public string Title
		{
			get;
			set;
		}

		public string Body
		{
			get;
			set;
		}

		public string OkBtnTxt
		{
			get;
			set;
		}

		public string CancelbtnTxt
		{
			get;
			set;
		}
	}
	public class ENDeveloperToolsViewModel
	{
		private string _logPrefix = "ENDeveloperToolsViewModel: ";

		private static bool _longRetentionTime = true;

		private DateTime _messageDateTime = DateTime.Now;

		public static string PushKeysInfo = "";

		public Action DevToolUpdateOutput;

		public string DevToolsOutput
		{
			get;
			set;
		}

		private static IDeveloperToolsService _devTools => ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>();

		private IClipboard _clipboard => ServiceLocator.get_Current().GetInstance<IClipboard>();

		public void PullWithDelay(Func<Task<bool>> action)
		{
			Task.Run(async delegate
			{
				LocalPreferencesHelper.TermsNotificationWasShown = false;
				await Task.Delay(10000);
				if (action != null)
				{
					await action();
				}
			});
		}

		internal static void UpdatePushKeysInfo(ApiResponse response, SelfDiagnosisSubmissionDTO selfDiagnosisSubmissionDTO, JsonSerializerSettings settings)
		{
			PushKeysInfo = string.Format("StatusCode: {0}, Time (UTC): {1}\n\n", response.StatusCode, DateTime.UtcNow.ToGreGorianUtcString("yyyy-MM-dd HH:mm:ss"));
			ParseKeys(selfDiagnosisSubmissionDTO, settings, ENOperation.PUSH);
			PutInPushKeyInfoInSharedPrefs();
		}

		private static void ParseKeys(SelfDiagnosisSubmissionDTO selfDiagnosisSubmissionDTO, JsonSerializerSettings settings, ENOperation varAssignCheck)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			JObject obj = JObject.Parse(JsonConvert.SerializeObject((object)selfDiagnosisSubmissionDTO, settings));
			JArray val = (JArray)obj.get_Item("keys");
			JArray arg = (JArray)obj.get_Item("visitedCountries");
			JArray arg2 = (JArray)obj.get_Item("regions");
			PushKeysInfo += $"visitedCountries: {arg}\n";
			PushKeysInfo += $"regions: {arg2}\n";
			if (val != null)
			{
				MoreEnumerable.ForEach<JToken>((IEnumerable<JToken>)val, (Action<JToken>)delegate(JToken key)
				{
					string text = "Key: " + EncodingUtils.ConvertByteArrayToString((byte[])key.get_Item((object)"key")) + " ,\n" + string.Format("rollingStart: {0},\n", key.get_Item((object)"rollingStart")) + string.Format("rollingDuration: {0},\n", key.get_Item((object)"rollingDuration")) + string.Format("transmissionRiskLevel: {0}\n\n", key.get_Item((object)"transmissionRiskLevel"));
					PushKeysInfo += text;
				});
			}
		}

		private static void PutInPushKeyInfoInSharedPrefs()
		{
			ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>().LastKeyUploadInfo = PushKeysInfo;
		}

		public async Task<string> GetPushKeyInfoFromSharedPrefs()
		{
			string res = "Empty";
			PushKeysInfo = _devTools.LastKeyUploadInfo;
			if (PushKeysInfo != "")
			{
				res = PushKeysInfo;
			}
			await _clipboard.SetTextAsync(res);
			return res;
		}

		public async Task<string> GetFormattedPreferences()
		{
			int migrationCount = LocalPreferencesHelper.MigrationCount;
			int lastPullKeysBatchNumberNotSubmitted = LocalPreferencesHelper.LastPullKeysBatchNumberNotSubmitted;
			int lastPullKeysBatchNumberSuccessfullySubmitted = LocalPreferencesHelper.LastPullKeysBatchNumberSuccessfullySubmitted;
			BatchType lastPulledBatchType = LocalPreferencesHelper.LastPulledBatchType;
			bool isOnboardingCompleted = LocalPreferencesHelper.IsOnboardingCompleted;
			bool isOnboardingCountriesCompleted = LocalPreferencesHelper.IsOnboardingCountriesCompleted;
			bool isDownloadWithMobileDataEnabled = LocalPreferencesHelper.GetIsDownloadWithMobileDataEnabled();
			DateTime updatedDateTime = LocalPreferencesHelper.GetUpdatedDateTime();
			DateTime lastPullKeysSucceededDateTime = LocalPreferencesHelper.GetLastPullKeysSucceededDateTime();
			string appLanguage = LocalPreferencesHelper.GetAppLanguage();
			string formattedString = $"EXPOSURE_TIME_THRESHOLD: {LocalPreferencesHelper.ExposureTimeThreshold}\n" + $"LOW_ATTENUATION_DURATION_MULTIPLIER: {LocalPreferencesHelper.LowAttenuationDurationMultiplier}\n" + $"MIDDLE_ATTENUATION_DURATION_MULTIPLIER: {LocalPreferencesHelper.MiddleAttenuationDurationMultiplier}\n" + $"HIGH_ATTENUATION_DURATION_MULTIPLIER: {LocalPreferencesHelper.HighAttenuationDurationMultiplier}\n\n" + $"MIGRATION_COUNT: {migrationCount}\n " + $"LAST_PULLED_BATCH_NUMBER_NOT_SUBMITTED: {lastPullKeysBatchNumberNotSubmitted}\n " + $"LAST_PULLED_BATCH_NUMBER_SUBMITTED: {lastPullKeysBatchNumberSuccessfullySubmitted}\n " + $"LAST_PULLED_BATCH_TYPE: {lastPulledBatchType}\n " + $"IS_ONBOARDING_COMPLETED_PREF: {isOnboardingCompleted}\n " + $"IS_ONBOARDING_COUNTRIES_COMPLETED_PREF: {isOnboardingCountriesCompleted}\n" + $"USE_MOBILE_DATA_PREF: {isDownloadWithMobileDataEnabled}\n" + $"MESSAGES_LAST_UPDATED_PREF: {updatedDateTime}\n" + $"LAST_PULL_KEYS_SUCCEEDED_DATE_TIME: {lastPullKeysSucceededDateTime}\n" + $"TERMS_NOTIFICATION_WAS_SENT: {LocalPreferencesHelper.TermsNotificationWasShown}\n" + "APP_LANGUAGE: " + appLanguage + "\n\n";
			await _clipboard.SetTextAsync(formattedString);
			return formattedString;
		}

		public static string GetLastPullResult()
		{
			return _devTools.LastPullHistory;
		}

		public string LastUsedExposureConfigurationAsync()
		{
			string lastUsedConfiguration = _devTools.LastUsedConfiguration;
			_clipboard.SetTextAsync(lastUsedConfiguration);
			return lastUsedConfiguration;
		}

		public async Task<ApiResponse> FakeGateway(string region)
		{
			ApiResponse result = default(ApiResponse);
			object obj;
			int num;
			try
			{
				if (string.IsNullOrEmpty(region))
				{
					region = "dk";
				}
				result = await FakeGatewayUtils.PostKeysToFakeGateway(region);
				return result;
			}
			catch (Exception ex)
			{
				obj = ex;
				num = 1;
			}
			if (num != 1)
			{
				return result;
			}
			Exception ex2 = (Exception)obj;
			LogUtils.LogException(LogSeverity.ERROR, ex2, _logPrefix + "Fake gateway upload failed");
			await _clipboard.SetTextAsync($"Push keys failed:\n{ex2}");
			return null;
		}

		public async Task<bool> PullKeysFromServer()
		{
			DevToolsOutput = GetLastPullResult();
			bool processedAnyFiles = false;
			try
			{
				await ExposureNotification.UpdateKeysFromServer(default(CancellationToken));
			}
			catch (Exception arg)
			{
				string error = $"Pull keys failed:\n{arg}";
				await _clipboard.SetTextAsync(error);
				ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>().AddToPullHistoryRecord(error);
			}
			return processedAnyFiles;
		}

		public async Task<bool> PullKeysFromServerAndGetExposureInfo()
		{
			DevToolsOutput = GetLastPullResult();
			bool processedAnyFiles = false;
			_devTools.ShouldSaveExposureInfo = true;
			try
			{
				await ExposureNotification.UpdateKeysFromServer(default(CancellationToken));
			}
			catch (Exception arg)
			{
				string error = $"Pull keys failed:\n{arg}";
				await _clipboard.SetTextAsync(error);
				ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>().AddToPullHistoryRecord(error);
			}
			return processedAnyFiles;
		}

		public string GetExposureInfosFromLastPull()
		{
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			string persistedExposureInfo = _devTools.PersistedExposureInfo;
			string text = "";
			if (persistedExposureInfo == "")
			{
				text = "We have not saved any ExposureInfos yet";
			}
			else
			{
				try
				{
					foreach (ExposureInfo item in ExposureInfoJsonHelper.ExposureInfosFromJsonCompatibleString(persistedExposureInfo))
					{
						string str = ((text == "") ? "" : "\n");
						text += str;
						text += "[ExposureInfo with ";
						text += $"AttenuationValue: {item.get_AttenuationValue()},";
						text += $"Duration: {item.get_Duration()},";
						text += $"Timestamp: {item.get_Timestamp()},";
						text += $"TotalRiskScore: {item.get_TotalRiskScore()},";
						text += $"TransmissionRiskLevel: {item.get_TransmissionRiskLevel()}";
						text += "]";
					}
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.WARNING, e, _logPrefix + "GetExposureInfosFromLastPull");
					text = "Failed at deserializing the saved ExposureInfos";
				}
			}
			string text2 = "These are the ExposureInfos we got the last time \"Pull keys and get exposure info\" was clicked:\n" + text;
			_clipboard.SetTextAsync(text2);
			return text2;
		}

		public async Task<string> FetchExposureConfigurationAsync()
		{
			Configuration val = await new ExposureNotificationHandler().GetConfigurationAsync();
			string res = (DevToolsOutput = $" AttenuationWeight: {val.get_AttenuationWeight()}, Values: {EnConfArrayString(val.get_AttenuationScores())} \n" + $" DaysSinceLastExposureWeight: {val.get_DaysSinceLastExposureWeight()}, Values: {EnConfArrayString(val.get_DaysSinceLastExposureScores())} \n" + $" DurationWeight: {val.get_DurationWeight()}, Values: {EnConfArrayString(val.get_DurationScores())} \n" + $" TransmissionWeight: {val.get_TransmissionWeight()}, Values: {EnConfArrayString(val.get_TransmissionRiskScores())} \n" + $" MinimumRiskScore: {val.get_MinimumRiskScore()}" + $" DurationAtAttenuationThresholds: [{val.get_DurationAtAttenuationThresholds()[0]},{val.get_DurationAtAttenuationThresholds()[1]}]");
			DevToolUpdateOutput?.Invoke();
			await _clipboard.SetTextAsync(res);
			return res;
		}

		private string EnConfArrayString(int[] values)
		{
			string text = "";
			for (int i = 0; i < 8; i++)
			{
				text = ((i == 7) ? (text + values[i]) : (text + values[i] + ", "));
			}
			return text;
		}

		public string ToggleMessageRetentionTime()
		{
			if (_longRetentionTime)
			{
				Conf.MAX_MESSAGE_RETENTION_TIME_IN_MINUTES = Conf.MESSAGE_RETENTION_TIME_IN_MINUTES_SHORT;
				_longRetentionTime = false;
			}
			else
			{
				Conf.MAX_MESSAGE_RETENTION_TIME_IN_MINUTES = Conf.MESSAGE_RETENTION_TIME_IN_MINUTES_LONG;
				_longRetentionTime = true;
			}
			return $"Message retention time minutes: \n{Conf.MAX_MESSAGE_RETENTION_TIME_IN_MINUTES}";
		}

		public string IncementExposureDate()
		{
			_messageDateTime = _messageDateTime.AddDays(1.0);
			return "Incremented date for Send Message function: \n" + _messageDateTime.ToGreGorianUtcString("yyyy-MM-dd HH:mm:ss");
		}

		public string DecrementExposureDate()
		{
			_messageDateTime = _messageDateTime.AddDays(-1.0);
			return "Decremented date for Send Message function: \n" + _messageDateTime.ToGreGorianUtcString("yyyy-MM-dd HH:mm:ss");
		}

		public string PrintLastSymptomOnsetDate()
		{
			PersonalDataModel personalData = AuthenticationState.PersonalData;
			return "Last Symptom Onset Date: " + QuestionnaireViewModel.DateLabel + ", " + $"Selection: {QuestionnaireViewModel.Selection}, " + "MiBaDate:" + personalData?.Covid19_smitte_start + ", " + $"Date used for risk calc:{personalData?.FinalMiBaDate}";
		}

		public string PrintLastPulledKeysAndTimestamp()
		{
			string text = _devTools.LastProvidedFilesPref;
			if (text == "")
			{
				text = "We have not saved any downloaded keys yet";
			}
			string text2 = "These are the last TEK batch files provided to the EN API:\n" + text;
			_clipboard.SetTextAsync(text2);
			return text2;
		}

		public async Task SimulateExposureMessage(int notificationTriggerInSeconds = 0)
		{
			await Task.Delay(notificationTriggerInSeconds * 1000);
			await MessageUtils.CreateMessage(this, _messageDateTime);
		}

		public async Task SimulateExposureMessageAfter10Sec()
		{
			await SimulateExposureMessage(10);
		}

		public string GetLastExposureSummary()
		{
			string text = ((!ServiceLocator.get_Current().GetInstance<SecureStorageService>().KeyExists(SecureStorageKeys.LAST_SUMMARY_KEY)) ? "No summary yet" : ("Last exposure summary: " + ServiceLocator.get_Current().GetInstance<SecureStorageService>().GetValue(SecureStorageKeys.LAST_SUMMARY_KEY)));
			_clipboard.SetTextAsync(text);
			return text;
		}

		public string GetPullHistory()
		{
			string allPullHistory = _devTools.AllPullHistory;
			if (allPullHistory == "")
			{
				return "No pull history";
			}
			_clipboard.SetTextAsync(allPullHistory);
			return allPullHistory;
		}
	}
	public static class ErrorViewModel
	{
		public static string REGISTER_ERROR_NOMATCH_HEADER => Extensions.Translate("REGISTER_ERROR_NOMATCH_HEADER", Array.Empty<object>());

		public static string REGISTER_ERROR_NOMATCH_DESCRIPTION => Extensions.Translate("REGISTER_ERROR_NOMATCH_DESCRIPTION", Array.Empty<object>());

		public static string REGISTER_ERROR_TOOMANYTRIES_HEADER => Extensions.Translate("REGISTER_ERROR_TOOMANYTRIES_HEADER", Array.Empty<object>());

		public static string REGISTER_ERROR_TOOMANYTRIES_DESCRIPTION => Extensions.Translate("REGISTER_ERROR_TOOMANYTRIES_DESCRIPTION", Array.Empty<object>());

		public static string REGISTER_ERROR_HEADER => Extensions.Translate("REGISTER_ERROR_HEADER", Array.Empty<object>());

		public static string REGISTER_ERROR_DESCRIPTION => Extensions.Translate("REGISTER_ERROR_DESCRIPTION", Array.Empty<object>());

		public static string REGISTER_ERROR_DISMISS => Extensions.Translate("REGISTER_ERROR_DISMISS", Array.Empty<object>());

		public static string REGISTER_LEAVE_HEADER => Extensions.Translate("REGISTER_LEAVE_HEADER", Array.Empty<object>());

		public static string REGISTER_LEAVE_DESCRIPTION => Extensions.Translate("REGISTER_LEAVE_DESCRIPTION", Array.Empty<object>());

		public static string REGISTER_LEAVE_CANCEL => Extensions.Translate("REGISTER_LEAVE_CANCEL", Array.Empty<object>());

		public static string REGISTER_LEAVE_CONFIRM => Extensions.Translate("REGISTER_LEAVE_CONFIRM", Array.Empty<object>());

		public static string REGISTER_ERROR_ACCESSIBILITY_CLOSE_BUTTON_TEXT => Extensions.Translate("REGISTER_ERROR_ACCESSIBILITY_CLOSE_BUTTON_TEXT", Array.Empty<object>());

		public static string REGISTER_ERROR_ACCESSIBILITY_TOOMANYTRIES_HEADER => Extensions.Translate("REGISTER_ERROR_ACCESSIBILITY_TOOMANYTRIES_HEADER", Array.Empty<object>());

		public static string REGISTER_ERROR_ACCESSIBILITY_TOOMANYTRIES_DESCRIPTION => Extensions.Translate("REGISTER_ERROR_ACCESSIBILITY_TOOMANYTRIES_DESCRIPTION", Array.Empty<object>());
	}
	public class ForceUpdateViewModel
	{
		public static string FORCE_UPDATE_MESSAGE => Extensions.Translate("FORCE_UPDATE_MESSAGE", Array.Empty<object>());

		public static string FORCE_UPDATE_BUTTON_GOOGLE_ANDROID => Extensions.Translate("FORCE_UPDATE_BUTTON_GOOGLE_ANDROID", Array.Empty<object>());

		public static string FORCE_UPDATE_BUTTON_HUAWEI_ANDROID => Extensions.Translate("FORCE_UPDATE_BUTTON_HUAWEI_ANDROID", Array.Empty<object>());

		public static string FORCE_UPDATE_BUTTON_APPSTORE_IOS => Extensions.Translate("FORCE_UPDATE_BUTTON_APPSTORE_IOS", Array.Empty<object>());
	}
	public class InfectionStatusViewModel
	{
		private DateTime _latestMessageDateTime = DateTime.Today;

		public static string INFECTION_STATUS_PAGE_TITLE => Extensions.Translate("SMITTESPORING_PAGE_TITLE", Array.Empty<object>());

		public static string INFECTION_STATUS_ACTIVE_TEXT => Extensions.Translate("SMITTESPORING_ACTIVE_HEADER", Array.Empty<object>());

		public static string INFECTION_STATUS_INACTIVE_TEXT => Extensions.Translate("SMITTESPORING_INACTIVE_HEADER", Array.Empty<object>());

		public static string INFECTION_STATUS_ACTIVITY_STATUS_DESCRIPTION_TEXT => Extensions.Translate("SMITTESPORING_ACTIVE_DESCRIPTION", Array.Empty<object>());

		public static string SMITTESPORING_INACTIVE_DESCRIPTION => Extensions.Translate("SMITTESPORING_INACTIVE_DESCRIPTION", Array.Empty<object>());

		public static string INFECTION_STATUS_MESSAGE_HEADER_TEXT => Extensions.Translate("SMITTESPORING_MESSAGE_HEADER", Array.Empty<object>());

		public static string INFECTION_STATUS_MESSAGE_ACCESSIBILITY_TEXT => Extensions.Translate("SMITTESPORING_MESSAGE_HEADER_ACCESSIBILITY", Array.Empty<object>());

		public static string INFECTION_STATUS_MESSAGE_SUBHEADER_TEXT => Extensions.Translate("SMITTESPORING_MESSAGE_DESCRIPTION", Array.Empty<object>());

		public static string INFECTION_STATUS_NO_NEW_MESSAGE_SUBHEADER_TEXT => Extensions.Translate("SMITTESPORING_NO_NEW_MESSAGE_DESCRIPTION", Array.Empty<object>());

		public static string INFECTION_STATUS_REGISTRATION_HEADER_TEXT => Extensions.Translate("SMITTESPORING_REGISTER_HEADER", Array.Empty<object>());

		public static string INFECTION_STATUS_REGISTRATION_SUBHEADER_TEXT => Extensions.Translate("SMITTESPORING_REGISTER_DESCRIPTION", Array.Empty<object>());

		public static string INFECTION_STATUS_MENU_ACCESSIBILITY_TEXT => Extensions.Translate("MENU_TEXT", Array.Empty<object>());

		public static string INFECTION_STATUS_NEW_MESSAGE_NOTIFICATION_DOT_ACCESSIBILITY_TEXT => Extensions.Translate("SMITTESPORING_NEW_MESSAGE_NOTIFICATION_DOT_ACCESSIBILITY", Array.Empty<object>());

		public static string INFECTION_STATUS_START_BUTTON_ACCESSIBILITY_TEXT => Extensions.Translate("SMITTESPORING_START_BUTTON_ACCESSIBILITY", Array.Empty<object>());

		public static string INFECTION_STATUS_STOP_BUTTON_ACCESSIBILITY_TEXT => Extensions.Translate("SMITTESPORING_STOP_BUTTON_ACCESSIBILITY", Array.Empty<object>());

		public bool ShowNewMessageIcon
		{
			get;
			private set;
		}

		public EventHandler NewMessagesIconVisibilityChanged
		{
			get;
			set;
		}

		public string NewMessageSubheaderTxt
		{
			get
			{
				if (!ShowNewMessageIcon)
				{
					return INFECTION_STATUS_NO_NEW_MESSAGE_SUBHEADER_TEXT;
				}
				return INFECTION_STATUS_MESSAGE_SUBHEADER_TEXT + " " + DateUtils.GetDateFromDateTime(_latestMessageDateTime, "d. MMMMM");
			}
		}

		public string NewMessageAccessibilityText => INFECTION_STATUS_MESSAGE_ACCESSIBILITY_TEXT + ". " + NewMessageSubheaderTxt;

		public string NewRegistrationAccessibilityText => INFECTION_STATUS_REGISTRATION_HEADER_TEXT + ". " + INFECTION_STATUS_REGISTRATION_SUBHEADER_TEXT;

		public DialogViewModel OffDialogViewModel => new DialogViewModel
		{
			Title = Extensions.Translate("SMITTESPORING_TOGGLE_OFF_HEADER", Array.Empty<object>()),
			Body = Extensions.Translate("SMITTESPORING_TOGGLE_OFF_DESCRIPTION", Array.Empty<object>()),
			OkBtnTxt = Extensions.Translate("SMITTESPORING_TOGGLE_OFF_CONFIRM", Array.Empty<object>()),
			CancelbtnTxt = Extensions.Translate("SMITTESPORING_TOGGLE_OFF_CANCEL", Array.Empty<object>())
		};

		public DialogViewModel OnDialogViewModel => new DialogViewModel
		{
			Title = Extensions.Translate("SMITTESPORING_TOGGLE_ON_HEADER", Array.Empty<object>()),
			Body = Extensions.Translate("SMITTESPORING_TOGGLE_ON_DESCRIPTION", Array.Empty<object>()),
			OkBtnTxt = Extensions.Translate("SMITTESPORING_TOGGLE_ON_CONFIRM", Array.Empty<object>()),
			CancelbtnTxt = Extensions.Translate("SMITTESPORING_TOGGLE_ON_CANCEL", Array.Empty<object>())
		};

		public DialogViewModel PermissionViewModel => new DialogViewModel
		{
			Title = Extensions.Translate("SMITTESPORING_EN_PERMISSION_DENIED_HEADER", Array.Empty<object>()),
			Body = Extensions.Translate("SMITTESPORING_EN_PERMISSION_DENIED_BODY", Array.Empty<object>()),
			OkBtnTxt = Extensions.Translate("SMITTESPORING_EN_PERMISSION_DENIED_OK_BTN", Array.Empty<object>())
		};

		public DialogViewModel ReportingIllDialogViewModel => new DialogViewModel
		{
			Title = Extensions.Translate("SMITTESPORING_REPORTING_ILL_DIALOG_HEADER", Array.Empty<object>()),
			Body = Extensions.Translate("SMITTESPORING_REPORTING_ILL_DIALOG_BODY", Array.Empty<object>()),
			OkBtnTxt = Extensions.Translate("SMITTESPORING_REPORTING_ILL_DIALOG_OK_BTN", Array.Empty<object>())
		};

		public async Task<string> StatusTxt()
		{
			return (await IsRunning()) ? INFECTION_STATUS_ACTIVE_TEXT : INFECTION_STATUS_INACTIVE_TEXT;
		}

		public async Task<string> StatusTxtDescription()
		{
			return (await IsRunning()) ? INFECTION_STATUS_ACTIVITY_STATUS_DESCRIPTION_TEXT : SMITTESPORING_INACTIVE_DESCRIPTION;
		}

		public InfectionStatusViewModel()
		{
			SubscribeMessages();
		}

		public async Task<bool> IsRunning()
		{
			try
			{
				return (int)(await ExposureNotification.GetStatusAsync()) == 2;
			}
			catch (Exception ex)
			{
				if (!ex.HandleExposureNotificationException("InfectionStatusViewModel", "IsRunning"))
				{
					throw ex;
				}
				return false;
			}
		}

		public async Task<bool> IsEnabled()
		{
			try
			{
				return await ExposureNotification.IsEnabledAsync();
			}
			catch (Exception ex)
			{
				if (!ex.HandleExposureNotificationException("InfectionStatusViewModel", "IsEnabled"))
				{
					throw ex;
				}
				return false;
			}
		}

		public async Task<bool> StartENService()
		{
			try
			{
				await ExposureNotification.StartAsync();
			}
			catch (Exception ex)
			{
				if (!ex.HandleExposureNotificationException("InfectionStatusViewModel", "StartENService"))
				{
					throw ex;
				}
			}
			return await IsRunning();
		}

		public async Task<bool> StopENService()
		{
			try
			{
				await ExposureNotification.StopAsync();
			}
			catch (Exception ex)
			{
				if (!ex.HandleExposureNotificationException("InfectionStatusViewModel", "StopENService"))
				{
					throw ex;
				}
			}
			return await IsRunning();
		}

		private async Task NewMessagesFetched()
		{
			List<MessageItemViewModel> list = MessageUtils.ToMessageItemViewModelList((await MessageUtils.GetMessages()).OrderByDescending((MessageSQLiteModel message) => message.TimeStamp).ToList());
			if (list.Any())
			{
				_latestMessageDateTime = list[0].TimeStamp;
			}
			ShowNewMessageIcon = (await MessageUtils.GetAllUnreadMessages()).Any();
			NewMessagesIconVisibilityChanged?.Invoke(this, null);
		}

		public void SubscribeMessages()
		{
			MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.KEY_MESSAGE_RECEIVED, async delegate
			{
				await NewMessagesFetched();
			});
		}

		public async void UpdateNotificationDot()
		{
			await NewMessagesFetched();
		}
	}
	public class InformationAndConsentViewModel
	{
		private NDB.Covid19.OAuth2.AuthenticationManager _authManager;

		private EventHandler _onSuccess;

		private EventHandler<AuthErrorType> _onError;

		public static string INFORMATION_CONSENT_HEADER_TEXT => Extensions.Translate("INFOCONSENT_HEADER", Array.Empty<object>());

		public static string INFORMATION_CONSENT_CONTENT_TEXT => Extensions.Translate("INFOCONSENT_DESCRIPTION", Array.Empty<object>());

		public static string INFORMATION_CONSENT_NEMID_BUTTON_TEXT => Extensions.Translate("INFOCONSENT_LOGIN", Array.Empty<object>());

		public static string CLOSE_BUTTON_ACCESSIBILITY_LABEL => Extensions.Translate("SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON", Array.Empty<object>());

		public static string INFOCONSENT_TITLE => Extensions.Translate("INFOCONSENT_TITLE", Array.Empty<object>());

		public static string INFOCONSENT_BODY_ONE => Extensions.Translate("INFOCONSENT_BODY_ONE", Array.Empty<object>());

		public static string INFOCONSENT_BODY_TWO => Extensions.Translate("INFOCONSENT_BODY_TWO", Array.Empty<object>());

		public static string INFOCONSENT_DESCRIPTION_ONE => Extensions.Translate("INFOCONSENT_DESCRIPTION_ONE", Array.Empty<object>());

		public event EventHandler<AuthErrorType> OnError;

		public event EventHandler OnSuccess;

		public InformationAndConsentViewModel(EventHandler onSuccess, EventHandler<AuthErrorType> onError)
		{
			_onSuccess = onSuccess;
			_onError = onError;
			_authManager = new NDB.Covid19.OAuth2.AuthenticationManager();
		}

		public void Init()
		{
			OnError += _onError;
			OnSuccess += _onSuccess;
			_authManager.Setup(OnAuthCompleted, OnAuthError);
		}

		public void Cleanup()
		{
			if (this.OnError != null)
			{
				OnError -= _onError;
			}
			if (this.OnSuccess != null)
			{
				OnSuccess -= _onSuccess;
			}
			_onError = null;
			_onSuccess = null;
			if (_authManager != null)
			{
				_authManager.Cleanup();
			}
		}

		private void Unsubscribe()
		{
			if (this.OnError != null)
			{
				OnError -= _onError;
			}
			if (this.OnSuccess != null)
			{
				OnSuccess -= _onSuccess;
			}
		}

		private void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			this.OnError?.Invoke(this, AuthErrorType.Unknown);
		}

		private void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			string str = "InformationAndConsentViewModel.OnAuthCompleted: ";
			if (e != null && e.get_IsAuthenticated())
			{
				Account account = e.get_Account();
				if (((account != null) ? account.get_Properties() : null) != null && e.get_Account().get_Properties().ContainsKey("access_token"))
				{
					LogUtils.LogMessage(LogSeverity.INFO, str + "User returned from NemID after authentication and access_token exists.");
					Account account2 = e.get_Account();
					string accessToken = ((account2 != null) ? account2.get_Properties()["access_token"] : null);
					PersonalDataModel payloadValidateJWTToken = _authManager.GetPayloadValidateJWTToken(accessToken);
					if (payloadValidateJWTToken == null)
					{
						this.OnError?.Invoke(this, AuthErrorType.Unknown);
						return;
					}
					if (e.get_Account().get_Properties().TryGetValue("expires_in", out var value))
					{
						int.TryParse(value, out var result);
						if (result > 0)
						{
							payloadValidateJWTToken.TokenExpiration = DateTime.Now.AddSeconds(result);
							LogUtils.LogMessage(LogSeverity.INFO, str + "Access-token expires timestamp", payloadValidateJWTToken.TokenExpiration.ToString());
						}
					}
					else
					{
						LogUtils.LogMessage(LogSeverity.ERROR, str + "'expires_in' value does not exist");
					}
					SaveCovidRelatedAttributes(payloadValidateJWTToken);
					if (AuthenticationState.PersonalData.IsBlocked)
					{
						this.OnError?.Invoke(this, AuthErrorType.MaxTriesExceeded);
					}
					else if (AuthenticationState.PersonalData.IsNotInfected)
					{
						this.OnError?.Invoke(this, AuthErrorType.NotInfected);
					}
					else if (!payloadValidateJWTToken.Validate() || AuthenticationState.PersonalData.UnknownStatus)
					{
						if (AuthenticationState.PersonalData.UnknownStatus)
						{
							LogUtils.LogMessage(LogSeverity.ERROR, str + "Value Covid19_status = ukendt");
						}
						this.OnError?.Invoke(this, AuthErrorType.Unknown);
					}
					else
					{
						this.OnSuccess?.Invoke(this, null);
					}
					return;
				}
			}
			Restart();
		}

		private void Restart()
		{
			Unsubscribe();
			if (_authManager != null)
			{
				_authManager.Cleanup();
			}
			_authManager = new NDB.Covid19.OAuth2.AuthenticationManager();
			Init();
		}

		private void SaveCovidRelatedAttributes(PersonalDataModel payload)
		{
			AuthenticationState.PersonalData = payload;
		}
	}
	public class InitializerViewModel
	{
		public static string LAUNCHER_PAGE_START_BTN => Extensions.Translate("LAUNCHER_PAGE_START_BTN", Array.Empty<object>());

		public static string LAUNCHER_PAGE_CONTINUE_IN_ENG => Extensions.Translate("LAUNCHER_PAGE_CONTINUE_IN_ENG", Array.Empty<object>());
	}
	public class MessageItemViewModel
	{
		public static readonly string MESSAGES_RECOMMENDATIONS = Extensions.Translate("MESSAGES_RECOMMENDATIONS_", Array.Empty<object>());

		public static readonly string MESSAGES_MESSAGE_HEADER = Extensions.Translate("MESSAGES_MESSAGE_HEADER", Array.Empty<object>());

		private bool _isRead;

		public int ID
		{
			get;
		}

		public string Title
		{
			get;
		}

		public DateTime TimeStamp
		{
			get;
		}

		public string MessageLink
		{
			get;
		}

		public string DayAndMonthString => DateUtils.GetDateFromDateTime(TimeStamp, "d. MMMMM") ?? "";

		public bool IsRead
		{
			get
			{
				return _isRead;
			}
			set
			{
				MessageUtils.MarkAsRead(this, value);
				_isRead = value;
			}
		}

		public MessageItemViewModel(MessageSQLiteModel model)
		{
			ID = model.ID;
			Title = model.Title;
			TimeStamp = model.TimeStamp;
			MessageLink = model.MessageLink;
			IsRead = model.IsRead;
		}

		public MessageItemViewModel()
		{
		}
	}
	public class MessagesViewModel
	{
		public static string MESSAGES_HEADER => Extensions.Translate("MESSAGES_HEADER", Array.Empty<object>());

		public static string MESSAGES_NO_ITEMS_TITLE => Extensions.Translate("MESSAGES_NOMESSAGES_HEADER", Array.Empty<object>());

		public static string MESSAGES_NO_ITEMS_DESCRIPTION => Extensions.Translate("MESSAGES_NOMESSAGES_LABEL", Array.Empty<object>());

		public static string MESSAGES_LAST_UPDATED_LABEL => Extensions.Translate("MESSAGES_LAST_UPDATED_LABEL", Array.Empty<object>());

		public static string MESSAGES_ACCESSIBILITY_CLOSE_BUTTON => Extensions.Translate("MESSAGES_ACCESSIBILITY_CLOSE_BUTTON", Array.Empty<object>());

		public static DateTime LastUpdateDateTime => LocalPreferencesHelper.GetUpdatedDateTime().ToLocalTime();

		public static string LastUpdateString
		{
			get
			{
				DateTime lastUpdateDateTime = LastUpdateDateTime;
				DateTime minValue = DateTime.MinValue;
				if (!(lastUpdateDateTime != minValue.ToLocalTime()))
				{
					return "";
				}
				return string.Format(MESSAGES_LAST_UPDATED_LABEL, DateUtils.GetDateFromDateTime(LastUpdateDateTime, "m") ?? "", DateUtils.GetDateFromDateTime(LastUpdateDateTime, "t") ?? "");
			}
		}

		public static void SubscribeMessages(object subscriber, Action<List<MessageItemViewModel>> action)
		{
			MessagingCenter.Subscribe<object>(subscriber, MessagingCenterKeys.KEY_MESSAGE_RECEIVED, async delegate
			{
				action?.Invoke(await GetMessages());
			});
		}

		public static void UnsubscribeMessages(object subscriber)
		{
			MessagingCenter.Unsubscribe<object>(subscriber, MessagingCenterKeys.KEY_MESSAGE_RECEIVED);
		}

		public static async Task<List<MessageItemViewModel>> GetMessages()
		{
			return MessageUtils.ToMessageItemViewModelList(await MessageUtils.GetMessages());
		}

		public static async Task MarkAllMessagesAsRead()
		{
			foreach (MessageItemViewModel item in await GetMessages())
			{
				if (!item.IsRead)
				{
					item.IsRead = true;
				}
			}
		}
	}
	public class NotificationViewModel
	{
		public NotificationsEnum Type
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Body
		{
			get;
			set;
		}
	}
	public class QuestionnaireCountriesViewModel
	{
		public static string COUNTRY_QUESTIONAIRE_HEADER_TEXT => Extensions.Translate("REGISTER_COUNTRY_QUESTIONAIRE_HEADER_TEXT", Array.Empty<object>());

		public static string COUNTRY_QUESTIONAIRE_INFORMATION_TEXT => Extensions.Translate("REGISTER_COUNTRY_QUESTIONAIRE_INFORMATION_TEXT", Array.Empty<object>());

		public static string COUNTRY_QUESTIONAIRE_BUTTON_TEXT => Extensions.Translate("REGISTER_COUNTRY_QUESTIONAIRE_BUTTON_TEXT", Array.Empty<object>());

		public static string COUNTRY_QUESTIONAIRE_FOOTER => Extensions.Translate("REGISTER_COUNTRY_QUESTIONAIRE_FOOTER", Array.Empty<object>());

		public DialogViewModel CloseDialogViewModel => new DialogViewModel
		{
			Title = ErrorViewModel.REGISTER_LEAVE_HEADER,
			Body = ErrorViewModel.REGISTER_LEAVE_DESCRIPTION,
			OkBtnTxt = ErrorViewModel.REGISTER_LEAVE_CONFIRM,
			CancelbtnTxt = ErrorViewModel.REGISTER_LEAVE_CANCEL
		};

		public async Task<List<CountryDetailsViewModel>> GetListOfCountriesAsync()
		{
			return (from model in (await new CountryListService().GetCountryList())?.CountryCollection?.Select((CountryDetailsDTO x) => new CountryDetailsViewModel
				{
					Name = x.GetName(),
					Code = x.Code
				})
				orderby model.Name
				select model).ToList() ?? new List<CountryDetailsViewModel>();
		}

		public void InvokeNextButtonClick(Action onSuccess, Action onFail, List<CountryDetailsViewModel> selectedCountriesList)
		{
			if (AuthenticationState.PersonalData != null)
			{
				AuthenticationState.PersonalData.VisitedCountries = (from x in selectedCountriesList
					where x.Checked
					select x.Code).ToList();
				onSuccess?.Invoke();
			}
			else
			{
				onFail?.Invoke();
			}
		}
	}
	public class QuestionnaireViewModel
	{
		public static bool DateHasBeenSet;

		public static string REGISTER_QUESTIONAIRE_HEADER => Extensions.Translate("REGISTER_QUESTIONAIRE_HEADER", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_TEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YES => Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YES", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YESBUT => Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YESBUT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_NO => Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_NO", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_SKIP => Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_SKIP", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_HELP => Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_HELP", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_NEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_NEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SHARING_TEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_SHARING_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SHARING_ANSWER_YES => Extensions.Translate("REGISTER_QUESTIONAIRE_SHARING_ANSWER_YES", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SHARING_ANSWER_NO => Extensions.Translate("REGISTER_QUESTIONAIRE_SHARING_ANSWER_NO", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SHARING_ANSWER_SKIP => Extensions.Translate("REGISTER_QUESTIONAIRE_SHARING_ANSWER_SKIP", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SUBMIT => Extensions.Translate("REGISTER_QUESTIONAIRE_SUBMIT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_HEADER => Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_HEADER", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_TEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_DESCRIPTION => Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_DESCRIPTION", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_DISMISS => Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_DISMISS", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_INNER_HEADER => Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_INNER_HEADER", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_INNER_READ_MORE => Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_INNER_READ_MORE", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_LINK => Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_LINK", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_CLOSE_BUTTON_TEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_CLOSE_BUTTON_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_1_TEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_1_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_2_TEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_2_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_3_TEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_3_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_4_TEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_4_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_DATEPICKER_TEXT => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_DATEPICKER_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_DATE_INFO_BUTTON => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_DATE_INFO_BUTTON", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_LOADING_PAGE_TITLE => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_LOADING_PAGE_TITLE", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_DATEPICKER => Extensions.Translate("REGISTER_QUESTIONAIRE_CHOOSE_DATE_POP_UP", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_HEADER => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_HEADER", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RECEIPT_HEADER => Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RECEIPT_HEADER", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_DATE_LABEL_FORMAT => Extensions.Translate("REGISTER_QUESTIONAIRE_DATE_LABEL_FORMAT", Array.Empty<object>());

		public DialogViewModel CloseDialogViewModel => new DialogViewModel
		{
			Title = ErrorViewModel.REGISTER_LEAVE_HEADER,
			Body = ErrorViewModel.REGISTER_LEAVE_DESCRIPTION,
			OkBtnTxt = ErrorViewModel.REGISTER_LEAVE_CONFIRM,
			CancelbtnTxt = ErrorViewModel.REGISTER_LEAVE_CANCEL
		};

		public static string DateLabel
		{
			get
			{
				if (!(_selectedDateUTC == DateTime.MinValue))
				{
					return DateUtils.GetDateFromDateTime(_localSelectedDate, "d");
				}
				return REGISTER_QUESTIONAIRE_DATE_LABEL_FORMAT;
			}
		}

		private static DateTime _selectedDateUTC
		{
			get;
			set;
		}

		private static DateTime _localSelectedDate => DateTime.SpecifyKind(_selectedDateUTC, DateTimeKind.Utc).ToLocalTime();

		public static QuestionaireSelection Selection
		{
			get;
			private set;
		} = QuestionaireSelection.Skip;


		public DateTime MinimumDate
		{
			get;
			private set;
		} = new DateTime(2020, 1, 1, 0, 0, 0).ToUniversalTime();


		public DateTime MaximumDate
		{
			get;
			private set;
		} = DateTime.Today.ToUniversalTime();


		public string RadioButtonAccessibilityDatepicker => REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YES + ". " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_DATEPICKER_TEXT + ". " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_1_TEXT;

		public string RadioButtonAccessibilityYesDontRemember => REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YESBUT + ". " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_2_TEXT;

		public string RadioButtonAccessibilityNo => REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_NO + "\n " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_3_TEXT;

		public string RadioButtonAccessibilitySkip => REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_SKIP + ". " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_4_TEXT;

		public string ReceipetPageReadMoreButtonAccessibility => REGISTER_QUESTIONAIRE_RECEIPT_INNER_READ_MORE;

		public void SetSelectedDateUTC(DateTime newDate)
		{
			_selectedDateUTC = newDate;
			DateHasBeenSet = true;
		}

		public static DateTime GetLocalSelectedDate()
		{
			return _localSelectedDate;
		}

		public void SetSelection(QuestionaireSelection selection)
		{
			Selection = selection;
		}

		public void InvokeNextButtonClick(Action onSuccess, Action onFail, Action onValidationFail, PlatformDialogServiceArguments platformDialogServiceArguments = null)
		{
			if (Selection == QuestionaireSelection.YesSince)
			{
				if (_selectedDateUTC == DateTime.MinValue)
				{
					ServiceLocator.get_Current().GetInstance<IDialogService>().ShowMessageDialog(null, Extensions.Translate("REGISTER_QUESTIONAIRE_CHOOSE_DATE_POP_UP", Array.Empty<object>()), Extensions.Translate("ERROR_OK_BTN", Array.Empty<object>()), platformDialogServiceArguments);
					onValidationFail?.Invoke();
					return;
				}
				AuthenticationState.PersonalData.FinalMiBaDate = _localSelectedDate;
			}
			else
			{
				if (!AuthenticationState.PersonalData.Validate())
				{
					onFail?.Invoke();
					LogUtils.LogMessage(LogSeverity.ERROR, "Validation of personaldata failed because of miba data was null or accesstoken expired");
					return;
				}
				try
				{
					AuthenticationState.PersonalData.FinalMiBaDate = Convert.ToDateTime(AuthenticationState.PersonalData.Covid19_smitte_start);
				}
				catch
				{
					onFail?.Invoke();
					LogUtils.LogMessage(LogSeverity.ERROR, "Miba data can't be parsed into datetime");
					return;
				}
			}
			onSuccess?.Invoke();
		}
	}
	public class SettingsGeneralViewModel
	{
		public static string SETTINGS_GENERAL_TITLE = Extensions.Translate("SETTINGS_GENERAL_TITLE", Array.Empty<object>());

		public static string SETTINGS_GENERAL_EXPLANATION_ONE = Extensions.Translate("SETTINGS_GENERAL_EXPLANATION_ONE", Array.Empty<object>());

		public static string SETTINGS_GENERAL_EXPLANATION_TWO = Extensions.Translate("SETTINGS_GENERAL_EXPLANATION_TWO", Array.Empty<object>());

		public static string SETTINGS_GENERAL_MOBILE_DATA_HEADER = Extensions.Translate("SETTINGS_GENERAL_MOBILE_DATA_HEADER", Array.Empty<object>());

		public static string SETTINGS_GENERAL_MOBILE_DATA_DESC = Extensions.Translate("SETTINGS_GENERAL_MOBILE_DATA_DESC", Array.Empty<object>());

		public static string SETTINGS_GENERAL_CHOOSE_LANGUAGE_HEADER = Extensions.Translate("SETTINGS_GENERAL_CHOOSE_LANGUAGE_HEADER", Array.Empty<object>());

		public static string SETTINGS_GENERAL_RESTART_REQUIRED_TEXT = Extensions.Translate("SETTINGS_GENERAL_RESTART_REQUIRED_TEXT", Array.Empty<object>());

		public static string SETTINGS_GENERAL_MORE_INFO_LINK = Extensions.Translate("SETTINGS_GENERAL_MORE_INFO_LINK", Array.Empty<object>());

		public static string SETTINGS_GENERAL_MORE_INFO_BUTTON_TEXT = Extensions.Translate("SETTINGS_GENERAL_MORE_INFO_BUTTON_TEXT", Array.Empty<object>());

		public static string SETTINGS_GENERAL_ACCESSIBILITY_MORE_INFO_BUTTON_TEXT = Extensions.Translate("SETTINGS_GENERAL_ACCESSIBILITY_MORE_INFO_BUTTON_TEXT", Array.Empty<object>());

		public static string SETTINGS_GENERAL_DA = Extensions.Translate("SETTINGS_GENERAL_DA", Array.Empty<object>());

		public static string SETTINGS_GENERAL_EN = Extensions.Translate("SETTINGS_GENERAL_EN", Array.Empty<object>());

		public static DialogViewModel AreYouSureDialogViewModel = new DialogViewModel
		{
			Body = Extensions.Translate("SETTINGS_GENERAL_DIALOG_BODY", Array.Empty<object>()),
			CancelbtnTxt = Extensions.Translate("SETTINGS_GENERAL_DIALOG_CANCEL", Array.Empty<object>()),
			OkBtnTxt = Extensions.Translate("SETTINGS_GENERAL_DIALOG_OK", Array.Empty<object>()),
			Title = Extensions.Translate("SETTINGS_GENERAL_DIALOG_TITLE", Array.Empty<object>())
		};

		public static string SETTINGS_GENERAL_EXPLANATION => SETTINGS_GENERAL_EXPLANATION_ONE + "\n\n" + SETTINGS_GENERAL_EXPLANATION_TWO;

		public static SettingsLanguageSelection Selection
		{
			get;
			private set;
		}

		public static DialogViewModel GetChangeLanguageViewModel => new DialogViewModel
		{
			Title = Extensions.Translate("SETTINGS_GENERAL_CHOOSE_LANGUAGE_HEADER", Array.Empty<object>()),
			Body = Extensions.Translate("SETTINGS_GENERAL_RESTART_REQUIRED_TEXT", Array.Empty<object>()),
			OkBtnTxt = Extensions.Translate("SETTINGS_GENERAL_DIALOG_OK", Array.Empty<object>())
		};

		public bool GetStoredCheckedState()
		{
			return LocalPreferencesHelper.GetIsDownloadWithMobileDataEnabled();
		}

		public void OnCheckedChange(bool isChecked)
		{
			LocalPreferencesHelper.SetIsDownloadWithMobileDataEnabled(isChecked);
		}

		public static void OpenSmitteStopLink()
		{
			try
			{
				ServiceLocator.get_Current().GetInstance<IBrowser>().OpenAsync(SETTINGS_GENERAL_MORE_INFO_LINK);
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, "Failed to open smittestop.dk link on general settings page");
			}
		}

		public void SetSelection(SettingsLanguageSelection selection)
		{
			Selection = selection;
		}
	}
	public class SettingsPage2ViewModel
	{
		public static string SETTINGS_PAGE_2_HEADER => Extensions.Translate("SETTINGS_PAGE_2_HEADER_TEXT", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_INTRO => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_INTRO", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_1_TITLE => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_1_TITLE", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_1_CONTENT => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_1_CONTENT", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_2_TITLE => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_2_TITLE", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_2_CONTENT => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_2_CONTENT", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_TITLE => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_TITLE", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_CONTENT => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_CONTENT", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_TITLE => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_TITLE", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_CONTENT => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_CONTENT", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_LINK_TEXT => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_LINK_TEXT", Array.Empty<object>());

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_LINK => Extensions.Translate("SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_LINK", Array.Empty<object>());
	}
	public class SettingsPage4ViewModel
	{
		public static string HEADER => Extensions.Translate("SETTINGS_PAGE_4_HEADER_TEXT", Array.Empty<object>());

		public static string CONTENT_TEXT_BEFORE_SUPPORT_LINK => Extensions.Translate("SETTINGS_PAGE_4_CONTENT_TEXT_BEFORE_SUPPORT_LINK", Array.Empty<object>());

		public static string SUPPORT_LINK_SHOWN_TEXT => Extensions.Translate("SETTINGS_PAGE_4_SUPPORT_LINK_SHOWN_TEXT", Array.Empty<object>());

		public static string SUPPORT_LINK => Extensions.Translate("SETTINGS_PAGE_4_SUPPORT_LINK", Array.Empty<object>());

		public static string EMAIL_TEXT => Extensions.Translate("SETTINGS_PAGE_4_EMAIL_TEXT", Array.Empty<object>());

		public static string EMAIL => Extensions.Translate("SETTINGS_PAGE_4_EMAIL", Array.Empty<object>());

		public static string PHONE_NUM_Text => Extensions.Translate("SETTINGS_PAGE_4_PHONE_NUM_TEXT", Array.Empty<object>());

		public static string PHONE_NUM => Extensions.Translate("SETTINGS_PAGE_4_PHONE_NUM", Array.Empty<object>());

		public static string PHONE_OPEN_TEXT => Extensions.Translate("SETTINGS_PAGE_4_PHONE_OPEN_TEXT", Array.Empty<object>());

		public static string PHONE_OPEN_MON_THU => Extensions.Translate("SETTINGS_PAGE_4_PHONE_OPEN_MON_THU", Array.Empty<object>());

		public static string PHONE_OPEN_FRE => Extensions.Translate("SETTINGS_PAGE_4_PHONE_OPEN_FRE", Array.Empty<object>());

		public static string PHONE_OPEN_SAT_SUN_HOLY => Extensions.Translate("SETTINGS_PAGE_4_PHONE_OPEN_SAT_SUN_HOLY", Array.Empty<object>());

		public static string PHONE_NUM_ACCESSIBILITY => Extensions.Translate("SETTINGS_PAGE_4_ACCESSIBILITY_PHONE_NUM", Array.Empty<object>());

		public static string PHONE_OPEN_MON_THU_ACCESSIBILITY => Extensions.Translate("SETTINGS_PAGE_4_ACCESSIBILITY_PHONE_OPEN_MON_THU", Array.Empty<object>());

		public static string PHONE_OPEN_FRE_ACCESSIBILITY => Extensions.Translate("SETTINGS_PAGE_4_ACCESSIBILITY_PHONE_OPEN_FRE", Array.Empty<object>());
	}
	public class SettingsPage5ViewModel
	{
		public static string SETTINGS_PAGE_5_HEADER => Extensions.Translate("SETTINGS_PAGE_5_HEADER_TEXT", Array.Empty<object>());

		public static string SETTINGS_PAGE_5_CONTENT => Extensions.Translate("SETTINGS_PAGE_5_CONTENT_TEXT", Array.Empty<object>());

		public static string SETTINGS_PAGE_5_LINK => Extensions.Translate("SETTINGS_PAGE_5_LINK", Array.Empty<object>());

		public static string GetVersionInfo()
		{
			IAppInfo instance = ServiceLocator.get_Current().GetInstance<IAppInfo>();
			return $"V{instance.VersionString} B{instance.BuildString} A{Conf.APIVersion} {GetPartialUrlFromConf()} ";
		}

		public static string GetPartialUrlFromConf()
		{
			try
			{
				string uRL_PREFIX = Conf.URL_PREFIX;
				return uRL_PREFIX["Https://".Length..uRL_PREFIX.IndexOf(".smittestop")];
			}
			catch
			{
				return "u";
			}
		}
	}
	public class SettingsViewModel
	{
		public static string SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON => Extensions.Translate("SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON", Array.Empty<object>());

		public static string SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON => Extensions.Translate("SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON", Array.Empty<object>());

		public bool ShowDebugItem => Conf.UseDeveloperTools;

		public List<SettingItem> SettingItemList
		{
			get;
			private set;
		}

		public SettingsViewModel()
		{
			SettingItemList = new List<SettingItem>
			{
				new SettingItem(SettingItemType.Intro),
				new SettingItem(SettingItemType.HowItWorks),
				new SettingItem(SettingItemType.Consent),
				new SettingItem(SettingItemType.Help),
				new SettingItem(SettingItemType.About),
				new SettingItem(SettingItemType.Settings)
			};
			if (Conf.UseDeveloperTools)
			{
				SettingItemList.Add(new SettingItem(SettingItemType.Debug));
			}
		}
	}
	public class WelcomePageWhatIsNewViewModel
	{
		public static string WELCOME_PAGE_WHATS_NEW_TITLE => Extensions.Translate("WELCOME_PAGE_WHATS_NEW_TITLE", Array.Empty<object>());

		public static string WELCOME_PAGE_WHATS_NEW_BULLET_ONE => Extensions.Translate("WELCOME_PAGE_WHATS_NEW_BULLET_ONE", Array.Empty<object>());

		public static string WELCOME_PAGE_WHATS_NEW_BULLET_TWO => Extensions.Translate("WELCOME_PAGE_WHATS_NEW_BULLET_TWO", Array.Empty<object>());

		public static string WELCOME_PAGE_WHATS_NEW_BULLET_THREE => Extensions.Translate("WELCOME_PAGE_WHATS_NEW_BULLET_THREE", Array.Empty<object>());

		public static string WELCOME_PAGE_WHATS_NEW_BUTTON => Extensions.Translate("WELCOME_PAGE_WHATS_NEW_BUTTON", Array.Empty<object>());

		public static string WELCOME_PAGE_WHATS_NEW_FOOTER => Extensions.Translate("WELCOME_PAGE_WHATS_NEW_FOOTER", Array.Empty<object>());
	}
	public class WelcomeViewModel
	{
		public static string NEXT_PAGE_BUTTON_TEXT => Extensions.Translate("WELCOME_PAGE_NEXT_BUTTON_TEXT", Array.Empty<object>());

		public static string PREVIOUS_PAGE_BUTTON_TEXT => Extensions.Translate("WELCOME_PAGE_PREVIOUS_BUTTON_TEXT", Array.Empty<object>());

		public static string WELCOME_PAGE_ONE_TITLE => Extensions.Translate("WELCOME_PAGE_ONE_TITLE", Array.Empty<object>());

		public static string WELCOME_PAGE_ONE_BODY_ONE => Extensions.Translate("WELCOME_PAGE_ONE_BODY_ONE", Array.Empty<object>());

		public static string WELCOME_PAGE_ONE_BODY_TWO => Extensions.Translate("WELCOME_PAGE_ONE_BODY_TWO", Array.Empty<object>());

		public static string WELCOME_PAGE_TWO_TITLE => Extensions.Translate("WELCOME_PAGE_TWO_TITLE", Array.Empty<object>());

		public static string WELCOME_PAGE_TWO_BODY_ONE => Extensions.Translate("WELCOME_PAGE_TWO_BODY_ONE", Array.Empty<object>());

		public static string WELCOME_PAGE_TWO_BODY_TWO => Extensions.Translate("WELCOME_PAGE_TWO_BODY_TWO", Array.Empty<object>());

		public static string WELCOME_PAGE_THREE_TITLE => Extensions.Translate("WELCOME_PAGE_THREE_TITLE", Array.Empty<object>());

		public static string WELCOME_PAGE_THREE_BODY_ONE => Extensions.Translate("WELCOME_PAGE_THREE_BODY_ONE", Array.Empty<object>());

		public static string WELCOME_PAGE_THREE_BODY_TWO => Extensions.Translate("WELCOME_PAGE_THREE_BODY_TWO", Array.Empty<object>());

		public static string WELCOME_PAGE_THREE_INFOBOX_BODY => Extensions.Translate("WELCOME_PAGE_THREE_INFOBOX_BODY", Array.Empty<object>());

		public static string WELCOME_PAGE_FOUR_TITLE => Extensions.Translate("WELCOME_PAGE_FOUR_TITLE", Array.Empty<object>());

		public static string WELCOME_PAGE_FOUR_BODY_ONE => Extensions.Translate("WELCOME_PAGE_FOUR_BODY_ONE", Array.Empty<object>());

		public static string WELCOME_PAGE_FOUR_BODY_TWO => Extensions.Translate("WELCOME_PAGE_FOUR_BODY_TWO", Array.Empty<object>());

		public static string WELCOME_PAGE_FOUR_BODY_THREE => Extensions.Translate("WELCOME_PAGE_FOUR_BODY_THREE", Array.Empty<object>());

		public static string WELCOME_PAGE_BACKGROUND_LIMITATIONS_TITLE => Extensions.Translate("WELCOME_PAGE_BACKGROUND_LIMITATIONS_TITLE", Array.Empty<object>());

		public static string WELCOME_PAGE_BACKGROUND_LIMITATIONS_BODY_ONE => Extensions.Translate("WELCOME_PAGE_BACKGROUND_LIMITATIONS_BODY_ONE", Array.Empty<object>());

		public static string WELCOME_PAGE_BACKGROUND_LIMITATIONS_BODY_TWO => Extensions.Translate("WELCOME_PAGE_BACKGROUND_LIMITATIONS_BODY_TWO", Array.Empty<object>());

		public static string WELCOME_PAGE_BACKGROUND_LIMITATIONS_NEXT_BUTTON => Extensions.Translate("WELCOME_PAGE_BACKGROUND_LIMITATIONS_NEXT_BUTTON", Array.Empty<object>());

		public static string ANNOUNCEMENT_PAGE_CHANGED_TO_ONE => Extensions.Translate("WELCOME_PAGE_ACCESSIBILITY_ANNOUNCEMENT_PAGE_CHANGED_TO_ONE", Array.Empty<object>());

		public static string ANNOUNCEMENT_PAGE_CHANGED_TO_TWO => Extensions.Translate("WELCOME_PAGE_ACCESSIBILITY_ANNOUNCEMENT_PAGE_CHANGED_TO_TWO", Array.Empty<object>());

		public static string ANNOUNCEMENT_PAGE_CHANGED_TO_THREE => Extensions.Translate("WELCOME_PAGE_ACCESSIBILITY_ANNOUNCEMENT_PAGE_CHANGED_TO_THREE", Array.Empty<object>());

		public static string ANNOUNCEMENT_PAGE_CHANGED_TO_FOUR => Extensions.Translate("WELCOME_PAGE_ACCESSIBILITY_ANNOUNCEMENT_PAGE_CHANGED_TO_FOUR", Array.Empty<object>());

		public static string TRANSMISSION_ERROR_MSG => Extensions.Translate("TRANSMISSION_ERROR_MSG", Array.Empty<object>());

		public static string WELCOME_PAGE_TWO_ACCESSIBILITY_TITLE => Extensions.Translate("WELCOME_PAGE_TWO_ACCESSIBILITY_TITLE", Array.Empty<object>());

		public static string WELCOME_PAGE_TWO_ACCESSIBILITY_BODY_ONE => Extensions.Translate("WELCOME_PAGE_TWO_ACCESSIBILITY_BODY_ONE", Array.Empty<object>());
	}
}
namespace NDB.Covid19.Models
{
	public class ApiResponse
	{
		public HttpMethod HttpMethod
		{
			get;
			set;
		}

		public string Endpoint
		{
			get;
			set;
		}

		public string ResponseText
		{
			get;
			set;
		}

		public int StatusCode
		{
			get;
			set;
		}

		public Exception Exception
		{
			get;
			set;
		}

		public HttpHeaders Headers
		{
			get;
			set;
		}

		public bool IsSuccessfull
		{
			get
			{
				if (HasSuccessfullStatusCode)
				{
					return Exception == null;
				}
				return false;
			}
		}

		public bool HasSuccessfullStatusCode
		{
			get
			{
				if (StatusCode != 200 && StatusCode != 201)
				{
					return StatusCode == 204;
				}
				return true;
			}
		}

		public string ErrorLogMessage
		{
			get
			{
				string text = Endpoint;
				if (text.EndsWith(".zip"))
				{
					int num = text.IndexOf(text.Split(new char[1]
					{
						'/'
					}).Last());
					if (num > 1)
					{
						text = text.Remove(num - 1);
					}
				}
				string text2 = $"API {HttpMethod} /{text} failed";
				if (!HasSuccessfullStatusCode && StatusCode != 0)
				{
					text2 += $" with HttpStatusCode {StatusCode}";
				}
				else if (Exception != null)
				{
					text2 = text2 + " with " + Exception.GetType().Name;
				}
				return text2;
			}
		}

		public ApiResponse(string url, HttpMethod method)
		{
			HttpMethod = method;
			try
			{
				Endpoint = url.Split(new string[1]
				{
					Conf.BaseUrl
				}, StringSplitOptions.None).Last();
			}
			catch
			{
			}
		}
	}
	public class ApiResponse<T> : ApiResponse
	{
		public T Data
		{
			get;
			set;
		}

		public ApiResponse(string url, HttpMethod method)
			: base(url, method)
		{
			Data = default(T);
		}
	}
	public class AttenuationBucketsConfigurationDTO
	{
		public Configuration Configuration
		{
			get;
			set;
		}

		public AttenuationBucketsParametersDTO AttenuationBucketsParams
		{
			get;
			set;
		}
	}
	public class AttenuationBucketsParametersDTO
	{
		public double ExposureTimeThreshold
		{
			get;
			set;
		}

		public double LowAttenuationBucketMultiplier
		{
			get;
			set;
		}

		public double MiddleAttenuationBucketMultiplier
		{
			get;
			set;
		}

		public double HighAttenuationBucketMultiplier
		{
			get;
			set;
		}
	}
	public class ExposureKeyModel : TemporaryExposureKey
	{
		public int DaysSinceOnsetOfSymptoms
		{
			get;
			set;
		}

		public ExposureKeyModel(TemporaryExposureKey tek)
			: this(tek.get_Key(), tek.get_RollingStart(), tek.get_RollingDuration(), tek.get_TransmissionRiskLevel())
		{
		}//IL_0014: Unknown result type (might be due to invalid IL or missing references)


		public ExposureKeyModel(byte[] keyData, DateTimeOffset rollingStart, TimeSpan rollingDuration, RiskLevel transmissionRisk)
			: this(keyData, rollingStart, rollingDuration, transmissionRisk)
		{
		}//IL_0004: Unknown result type (might be due to invalid IL or missing references)


		public ExposureKeyModel(byte[] keyData, DateTimeOffset rollingStart, TimeSpan rollingDuration, RiskLevel transmissionRisk, int daysSinceOnsetOfSymptoms)
			: this(keyData, rollingStart, rollingDuration, transmissionRisk)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			DaysSinceOnsetOfSymptoms = daysSinceOnsetOfSymptoms;
		}
	}
	public class PersonalDataModel
	{
		public string Covid19_smitte_start
		{
			get;
			set;
		}

		public string Covid19_blokeret
		{
			get;
			set;
		}

		public string Covid19_smitte_stop
		{
			get;
			set;
		}

		public string Covid19_status
		{
			get;
			set;
		}

		[JsonIgnore]
		public string Access_token
		{
			get;
			set;
		}

		[JsonIgnore]
		public DateTime? TokenExpiration
		{
			get;
			set;
		}

		[JsonIgnore]
		public DateTime? FinalMiBaDate
		{
			get;
			set;
		}

		[JsonIgnore]
		public List<string> VisitedCountries
		{
			get;
			set;
		} = new List<string>();


		[JsonIgnore]
		public bool IsBlocked => Covid19_blokeret == "true";

		[JsonIgnore]
		public bool IsNotInfected => Covid19_status == "negativ";

		[JsonIgnore]
		public bool UnknownStatus => Covid19_status == "ukendt";

		public bool Validate()
		{
			string str = "PersonalDataModel.Validate: ";
			bool num = !string.IsNullOrEmpty(Covid19_smitte_start);
			if (!num)
			{
				LogUtils.LogMessage(LogSeverity.ERROR, str + "Covid19_smitte_start value was null or empty");
			}
			bool flag = TokenExpiration.HasValue && TokenExpiration > DateTime.Now;
			if (!flag)
			{
				LogUtils.LogMessage(LogSeverity.ERROR, str + "Access token was expired");
			}
			return num && flag;
		}
	}
	public class SelfDiagnosisSubmissionDTO
	{
		public IEnumerable<ExposureKeyModel> Keys
		{
			get;
			set;
		}

		public List<string> Regions
		{
			get;
			set;
		}

		public List<string> VisitedCountries
		{
			get;
			set;
		} = new List<string>();


		public string AppPackageName
		{
			get;
			set;
		}

		public string Platform
		{
			get;
			set;
		}

		public string Padding
		{
			get;
			set;
		}

		public SelfDiagnosisSubmissionDTO()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			AppPackageName = ServiceLocator.get_Current().GetInstance<IAppInfo>().PackageName;
			DevicePlatform platform = ServiceLocator.get_Current().GetInstance<IDeviceInfo>().Platform;
			Platform = ((object)(DevicePlatform)(ref platform)).ToString();
			Regions = Conf.SUPPORTED_REGIONS.ToList();
			VisitedCountries.AddRange(AuthenticationState.PersonalData?.VisitedCountries ?? new List<string>());
		}

		public SelfDiagnosisSubmissionDTO(IEnumerable<ExposureKeyModel> keys)
			: this()
		{
			Keys = keys;
			ComputePadding();
		}

		public void ComputePadding()
		{
			Padding = "";
			int num = new Random().Next(12, 24);
			for (int i = 1; i <= num; i++)
			{
				string text = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(AppPackageName + Platform + DateTime.UtcNow.Ticks)));
				if (Padding.Length * 2 < 1024 || (text + Padding).Length * 2 <= 2048)
				{
					Padding += text;
					continue;
				}
				break;
			}
		}
	}
	public class SettingItem
	{
		public SettingItemType Type
		{
			get;
			private set;
		}

		public string Text => GetFriendlyTextFromSettingItemType();

		public SettingItem(SettingItemType type)
		{
			Type = type;
		}

		private string GetFriendlyTextFromSettingItemType()
		{
			return Type switch
			{
				SettingItemType.About => Extensions.Translate("SETTINGS_ITEM_ABOUT", Array.Empty<object>()), 
				SettingItemType.Consent => Extensions.Translate("SETTINGS_ITEM_CONSENT", Array.Empty<object>()), 
				SettingItemType.Help => Extensions.Translate("SETTINGS_ITEM_HELP", Array.Empty<object>()), 
				SettingItemType.HowItWorks => Extensions.Translate("SETTINGS_ITEM_HOW_IT_WORKS", Array.Empty<object>()), 
				SettingItemType.Intro => Extensions.Translate("SETTINGS_ITEM_INTRO", Array.Empty<object>()), 
				SettingItemType.Settings => Extensions.Translate("SETTINGS_ITEM_GENERAL", Array.Empty<object>()), 
				SettingItemType.Messages => Extensions.Translate("SETTINGS_ITEM_MESSAGES", Array.Empty<object>()), 
				SettingItemType.Debug => "Developer Tools", 
				_ => string.Empty, 
			};
		}
	}
}
namespace NDB.Covid19.Models.UserDefinedExceptions
{
	[Serializable]
	public class AccessTokenMissingFromNemIDException : Exception
	{
		public AccessTokenMissingFromNemIDException()
		{
		}

		public AccessTokenMissingFromNemIDException(string message)
			: base(message)
		{
		}

		public AccessTokenMissingFromNemIDException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
	[Serializable]
	public class DeviceVerificationException : Exception
	{
		public DeviceVerificationException()
		{
		}

		public DeviceVerificationException(string message)
			: base(message)
		{
		}

		public DeviceVerificationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
	[Serializable]
	public class FailedToFetchConfigurationException : Exception
	{
		public FailedToFetchConfigurationException()
		{
		}

		public FailedToFetchConfigurationException(string message)
			: base(message)
		{
		}

		public FailedToFetchConfigurationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
	[Serializable]
	public class FailedToPushToServerException : Exception
	{
		public FailedToPushToServerException()
		{
		}

		public FailedToPushToServerException(string message)
			: base(message)
		{
		}

		public FailedToPushToServerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
	[Serializable]
	public class MiBaDateMissingException : Exception
	{
		public MiBaDateMissingException()
		{
		}

		public MiBaDateMissingException(string message)
			: base(message)
		{
		}

		public MiBaDateMissingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
	[Serializable]
	public class VisitedCountriesMissingException : Exception
	{
		public VisitedCountriesMissingException()
		{
		}

		public VisitedCountriesMissingException(string message)
			: base(message)
		{
		}

		public VisitedCountriesMissingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
namespace NDB.Covid19.Models.SQLite
{
	public class LogSQLiteModel
	{
		[PrimaryKey]
		[AutoIncrement]
		public int ID
		{
			get;
			set;
		}

		public DateTime ReportedTime
		{
			get;
			set;
		}

		public string Severity
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public int ApiVersion
		{
			get;
			set;
		}

		public string BuildVersion
		{
			get;
			set;
		}

		public string BuildNumber
		{
			get;
			set;
		}

		public string DeviceOSVersion
		{
			get;
			set;
		}

		public string ExceptionType
		{
			get;
			set;
		}

		public string ExceptionMessage
		{
			get;
			set;
		}

		public string ExceptionStackTrace
		{
			get;
			set;
		}

		public string InnerExceptionType
		{
			get;
			set;
		}

		public string InnerExceptionMessage
		{
			get;
			set;
		}

		public string InnerExceptionStackTrace
		{
			get;
			set;
		}

		public string Api
		{
			get;
			set;
		}

		public int? ApiErrorCode
		{
			get;
			set;
		}

		public string ApiErrorMessage
		{
			get;
			set;
		}

		public string AdditionalInfo
		{
			get;
			set;
		}

		public LogSQLiteModel()
		{
		}

		public LogSQLiteModel(LogDeviceDetails info, LogApiDetails apiDetails = null, LogExceptionDetails e = null)
		{
			ReportedTime = info.ReportedTime;
			Severity = info.Severity.ToString();
			Description = info.Description;
			ApiVersion = info.ApiVersion;
			BuildVersion = info.BuildVersion;
			BuildNumber = info.BuildNumber;
			DeviceOSVersion = info.DeviceOSVersion;
			AdditionalInfo = info.AdditionalInfo;
			if (apiDetails != null)
			{
				Api = apiDetails.Api;
				ApiErrorCode = apiDetails.ApiErrorCode;
				ApiErrorMessage = apiDetails.ApiErrorMessage;
			}
			if (e != null)
			{
				ExceptionType = e.ExceptionType;
				ExceptionMessage = e.ExceptionMessage;
				ExceptionStackTrace = e.ExceptionStackTrace;
				InnerExceptionType = e.InnerExceptionType;
				InnerExceptionMessage = e.InnerExceptionMessage;
				InnerExceptionStackTrace = e.InnerExceptionStackTrace;
			}
		}

		public override string ToString()
		{
			return Severity + " Log: " + Description;
		}
	}
	public class MessageSQLiteModel
	{
		[PrimaryKey]
		[AutoIncrement]
		public int ID
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public DateTime TimeStamp
		{
			get;
			set;
		}

		public string MessageLink
		{
			get;
			set;
		}

		public bool IsRead
		{
			get;
			set;
		}

		public MessageSQLiteModel()
		{
		}

		public MessageSQLiteModel(MessageItemViewModel model)
		{
			ID = model.ID;
			Title = model.Title;
			TimeStamp = model.TimeStamp;
			MessageLink = model.MessageLink;
			IsRead = model.IsRead;
		}
	}
}
namespace NDB.Covid19.Models.Logging
{
	public class LogApiDetails
	{
		public string Api
		{
			get;
			private set;
		}

		public int? ApiErrorCode
		{
			get;
			private set;
		}

		public string ApiErrorMessage
		{
			get;
			private set;
		}

		public LogApiDetails(ApiResponse apiResponse)
		{
			Api = "/" + apiResponse.Endpoint;
			ApiErrorCode = ((apiResponse.StatusCode > 0) ? new int?(apiResponse.StatusCode) : null);
			ApiErrorMessage = (new int?[2]
			{
				200,
				201
			}.Contains(ApiErrorCode) ? null : Anonymizer.RedactText(apiResponse.ResponseText));
		}
	}
	public class LogDeviceDetails
	{
		public LogSeverity Severity
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public DateTime ReportedTime
		{
			get;
			private set;
		}

		public int ApiVersion
		{
			get;
			private set;
		}

		public string BuildVersion
		{
			get;
			private set;
		}

		public string BuildNumber
		{
			get;
			private set;
		}

		public string DeviceOSVersion
		{
			get;
			private set;
		}

		public string AdditionalInfo
		{
			get;
			set;
		}

		public LogDeviceDetails(LogSeverity severity, string logMessage, string additionalInfo = "")
		{
			Severity = severity;
			Description = Anonymizer.RedactText(logMessage);
			ReportedTime = DateTime.Now;
			ApiVersion = Conf.APIVersion;
			string backGroundServicVersionLogString = ServiceLocator.get_Current().GetInstance<IApiDataHelper>().GetBackGroundServicVersionLogString();
			AdditionalInfo = Anonymizer.RedactText(additionalInfo) + backGroundServicVersionLogString;
			BuildNumber = AppInfo.get_BuildString();
			BuildVersion = AppInfo.get_VersionString();
			DeviceOSVersion = DeviceInfo.get_VersionString();
		}
	}
	public class LogExceptionDetails
	{
		private readonly int _maxLengthOfStacktrace = 1000;

		public string ExceptionType
		{
			get;
			private set;
		}

		public string ExceptionMessage
		{
			get;
			private set;
		}

		public string ExceptionStackTrace
		{
			get;
			private set;
		}

		public string InnerExceptionType
		{
			get;
			private set;
		}

		public string InnerExceptionMessage
		{
			get;
			private set;
		}

		public string InnerExceptionStackTrace
		{
			get;
			private set;
		}

		public LogExceptionDetails(Exception e)
		{
			if (e != null)
			{
				ExceptionType = e.GetType().Name;
				ExceptionMessage = Anonymizer.RedactText(e.Message);
				ExceptionStackTrace = Anonymizer.RedactText(ShortenedText(e.StackTrace));
				if (e.InnerException != null)
				{
					InnerExceptionType = e.InnerException!.GetType().Name;
					InnerExceptionMessage = Anonymizer.RedactText(e.InnerException!.Message);
					InnerExceptionStackTrace = Anonymizer.RedactText(ShortenedText(e.InnerException!.StackTrace));
				}
			}
		}

		private string ShortenedText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			if (text.Length <= _maxLengthOfStacktrace)
			{
				return text;
			}
			return text.Substring(0, _maxLengthOfStacktrace);
		}
	}
}
namespace NDB.Covid19.Models.DTOsForServer
{
	public class CountryDetailsDTO
	{
		public string Name_DA
		{
			get;
			set;
		}

		public string Name_EN
		{
			get;
			set;
		}

		public string Code
		{
			get;
			set;
		}

		public string GetName()
		{
			return LocalesService.GetLanguage() switch
			{
				"da" => Name_DA, 
				"en" => Name_EN, 
				_ => Name_DA, 
			};
		}
	}
	public class CountryListDTO
	{
		public List<CountryDetailsDTO> CountryCollection
		{
			get;
			set;
		}
	}
	public class LogDTO
	{
		public DateTime ReportedTime
		{
			get;
			private set;
		}

		public string Severity
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public int ApiVersion
		{
			get;
			private set;
		}

		public string BuildVersion
		{
			get;
			private set;
		}

		public string BuildNumber
		{
			get;
			private set;
		}

		public string DeviceOSVersion
		{
			get;
			private set;
		}

		public string DeviceCorrelationId
		{
			get;
			private set;
		}

		public string DeviceType
		{
			get;
			private set;
		}

		public string DeviceDescription
		{
			get;
			private set;
		}

		public string ExceptionType
		{
			get;
			private set;
		}

		public string ExceptionMessage
		{
			get;
			private set;
		}

		public string ExceptionStackTrace
		{
			get;
			private set;
		}

		public string InnerExceptionType
		{
			get;
			private set;
		}

		public string InnerExceptionMessage
		{
			get;
			private set;
		}

		public string InnerExceptionStackTrace
		{
			get;
			private set;
		}

		public string Api
		{
			get;
			set;
		}

		public int? ApiErrorCode
		{
			get;
			private set;
		}

		public string ApiErrorMessage
		{
			get;
			private set;
		}

		public string AdditionalInfo
		{
			get;
			private set;
		}

		public LogDTO(LogSQLiteModel log)
		{
			DeviceType = DeviceUtils.DeviceType;
			DeviceDescription = DeviceUtils.DeviceModel;
			ReportedTime = log.ReportedTime;
			Severity = log.Severity;
			Description = log.Description;
			ApiVersion = log.ApiVersion;
			BuildVersion = log.BuildVersion;
			BuildNumber = log.BuildNumber;
			DeviceOSVersion = log.DeviceOSVersion;
			DeviceCorrelationId = "";
			ExceptionType = log.ExceptionType;
			ExceptionMessage = log.ExceptionMessage;
			ExceptionStackTrace = log.ExceptionStackTrace;
			InnerExceptionType = log.InnerExceptionType;
			InnerExceptionMessage = log.InnerExceptionMessage;
			InnerExceptionStackTrace = log.InnerExceptionStackTrace;
			Api = log.Api;
			ApiErrorCode = log.ApiErrorCode;
			ApiErrorMessage = log.ApiErrorMessage;
			AdditionalInfo = log.AdditionalInfo;
		}

		public override string ToString()
		{
			return Severity + " Log: " + Description;
		}
	}
}
namespace NDB.Covid19.Implementation
{
	public class AccelerometerImplementation : IEssentialsImplementation, IAccelerometer
	{
		bool IAccelerometer.IsMonitoring => Accelerometer.get_IsMonitoring();

		event EventHandler<AccelerometerChangedEventArgs> IAccelerometer.ReadingChanged
		{
			add
			{
				Accelerometer.add_ReadingChanged(value);
			}
			remove
			{
				Accelerometer.remove_ReadingChanged(value);
			}
		}

		event EventHandler IAccelerometer.ShakeDetected
		{
			add
			{
				Accelerometer.add_ShakeDetected(value);
			}
			remove
			{
				Accelerometer.remove_ShakeDetected(value);
			}
		}

		[Preserve(Conditional = true)]
		public AccelerometerImplementation()
		{
		}

		void IAccelerometer.Start(SensorSpeed sensorSpeed)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Accelerometer.Start(sensorSpeed);
		}

		void IAccelerometer.Stop()
		{
			Accelerometer.Stop();
		}
	}
	public class AppInfoImplementation : IEssentialsImplementation, IAppInfo
	{
		string IAppInfo.PackageName => AppInfo.get_PackageName();

		string IAppInfo.Name => AppInfo.get_Name();

		string IAppInfo.VersionString => AppInfo.get_VersionString();

		Version IAppInfo.Version => AppInfo.get_Version();

		string IAppInfo.BuildString => AppInfo.get_BuildString();

		AppTheme IAppInfo.RequestedTheme => AppInfo.get_RequestedTheme();

		[Preserve(Conditional = true)]
		public AppInfoImplementation()
		{
		}

		void IAppInfo.ShowSettingsUI()
		{
			AppInfo.ShowSettingsUI();
		}
	}
	public class BarometerImplementation : IEssentialsImplementation, IBarometer
	{
		bool IBarometer.IsMonitoring => Barometer.get_IsMonitoring();

		event EventHandler<BarometerChangedEventArgs> IBarometer.ReadingChanged
		{
			add
			{
				Barometer.add_ReadingChanged(value);
			}
			remove
			{
				Barometer.remove_ReadingChanged(value);
			}
		}

		[Preserve(Conditional = true)]
		public BarometerImplementation()
		{
		}

		void IBarometer.Start(SensorSpeed sensorSpeed)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Barometer.Start(sensorSpeed);
		}

		void IBarometer.Stop()
		{
			Barometer.Stop();
		}
	}
	public class BatteryImplementation : IEssentialsImplementation, IBattery
	{
		double IBattery.ChargeLevel => Battery.get_ChargeLevel();

		BatteryState IBattery.State => Battery.get_State();

		BatteryPowerSource IBattery.PowerSource => Battery.get_PowerSource();

		EnergySaverStatus IBattery.EnergySaverStatus => Battery.get_EnergySaverStatus();

		event EventHandler<BatteryInfoChangedEventArgs> IBattery.BatteryInfoChanged
		{
			add
			{
				Battery.add_BatteryInfoChanged(value);
			}
			remove
			{
				Battery.remove_BatteryInfoChanged(value);
			}
		}

		event EventHandler<EnergySaverStatusChangedEventArgs> IBattery.EnergySaverStatusChanged
		{
			add
			{
				Battery.add_EnergySaverStatusChanged(value);
			}
			remove
			{
				Battery.remove_EnergySaverStatusChanged(value);
			}
		}

		[Preserve(Conditional = true)]
		public BatteryImplementation()
		{
		}
	}
	public class BrowserImplementation : IEssentialsImplementation, IBrowser
	{
		[Preserve(Conditional = true)]
		public BrowserImplementation()
		{
		}

		Task IBrowser.OpenAsync(string uri)
		{
			return Browser.OpenAsync(uri);
		}

		Task IBrowser.OpenAsync(string uri, BrowserLaunchMode launchMode)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Browser.OpenAsync(uri, launchMode);
		}

		Task IBrowser.OpenAsync(string uri, BrowserLaunchOptions options)
		{
			return Browser.OpenAsync(uri, options);
		}

		Task IBrowser.OpenAsync(Uri uri)
		{
			return Browser.OpenAsync(uri);
		}

		Task IBrowser.OpenAsync(Uri uri, BrowserLaunchMode launchMode)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Browser.OpenAsync(uri, launchMode);
		}

		Task<bool> IBrowser.OpenAsync(Uri uri, BrowserLaunchOptions options)
		{
			return Browser.OpenAsync(uri, options);
		}
	}
	public class ClipboardImplementation : IEssentialsImplementation, IClipboard
	{
		bool IClipboard.HasText => Clipboard.get_HasText();

		event EventHandler<EventArgs> IClipboard.ClipboardContentChanged
		{
			add
			{
				Clipboard.add_ClipboardContentChanged(value);
			}
			remove
			{
				Clipboard.remove_ClipboardContentChanged(value);
			}
		}

		[Preserve(Conditional = true)]
		public ClipboardImplementation()
		{
		}

		Task IClipboard.SetTextAsync(string text)
		{
			return Clipboard.SetTextAsync(text);
		}

		Task<string> IClipboard.GetTextAsync()
		{
			return Clipboard.GetTextAsync();
		}
	}
	public class CompassImplementation : IEssentialsImplementation, ICompass
	{
		bool ICompass.IsMonitoring => Compass.get_IsMonitoring();

		event EventHandler<CompassChangedEventArgs> ICompass.ReadingChanged
		{
			add
			{
				Compass.add_ReadingChanged(value);
			}
			remove
			{
				Compass.remove_ReadingChanged(value);
			}
		}

		[Preserve(Conditional = true)]
		public CompassImplementation()
		{
		}

		void ICompass.Start(SensorSpeed sensorSpeed)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Compass.Start(sensorSpeed);
		}

		void ICompass.Start(SensorSpeed sensorSpeed, bool applyLowPassFilter)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Compass.Start(sensorSpeed, applyLowPassFilter);
		}

		void ICompass.Stop()
		{
			Compass.Stop();
		}
	}
	public class ConnectivityImplementation : IEssentialsImplementation, IConnectivity
	{
		NetworkAccess IConnectivity.NetworkAccess => Connectivity.get_NetworkAccess();

		IEnumerable<ConnectionProfile> IConnectivity.ConnectionProfiles => Connectivity.get_ConnectionProfiles();

		event EventHandler<ConnectivityChangedEventArgs> IConnectivity.ConnectivityChanged
		{
			add
			{
				Connectivity.add_ConnectivityChanged(value);
			}
			remove
			{
				Connectivity.remove_ConnectivityChanged(value);
			}
		}

		[Preserve(Conditional = true)]
		public ConnectivityImplementation()
		{
		}
	}
	public class DeviceDisplayImplementation : IEssentialsImplementation, IDeviceDisplay
	{
		bool IDeviceDisplay.KeepScreenOn
		{
			get
			{
				return DeviceDisplay.get_KeepScreenOn();
			}
			set
			{
				DeviceDisplay.set_KeepScreenOn(value);
			}
		}

		DisplayInfo IDeviceDisplay.MainDisplayInfo => DeviceDisplay.get_MainDisplayInfo();

		event EventHandler<DisplayInfoChangedEventArgs> IDeviceDisplay.MainDisplayInfoChanged
		{
			add
			{
				DeviceDisplay.add_MainDisplayInfoChanged(value);
			}
			remove
			{
				DeviceDisplay.remove_MainDisplayInfoChanged(value);
			}
		}

		[Preserve(Conditional = true)]
		public DeviceDisplayImplementation()
		{
		}
	}
	public class DeviceInfoImplementation : IEssentialsImplementation, IDeviceInfo
	{
		string IDeviceInfo.Model => DeviceInfo.get_Model();

		string IDeviceInfo.Manufacturer => DeviceInfo.get_Manufacturer();

		string IDeviceInfo.Name => DeviceInfo.get_Name();

		string IDeviceInfo.VersionString => DeviceInfo.get_VersionString();

		Version IDeviceInfo.Version => DeviceInfo.get_Version();

		DevicePlatform IDeviceInfo.Platform => DeviceInfo.get_Platform();

		DeviceIdiom IDeviceInfo.Idiom => DeviceInfo.get_Idiom();

		DeviceType IDeviceInfo.DeviceType => DeviceInfo.get_DeviceType();

		[Preserve(Conditional = true)]
		public DeviceInfoImplementation()
		{
		}
	}
	public class EmailImplementation : IEssentialsImplementation, IEmail
	{
		[Preserve(Conditional = true)]
		public EmailImplementation()
		{
		}

		Task IEmail.ComposeAsync()
		{
			return Email.ComposeAsync();
		}

		Task IEmail.ComposeAsync(string subject, string body, params string[] to)
		{
			return Email.ComposeAsync(subject, body, to);
		}

		Task IEmail.ComposeAsync(EmailMessage message)
		{
			return Email.ComposeAsync(message);
		}
	}
	public class FileSystemImplementation : IEssentialsImplementation, IFileSystem
	{
		string IFileSystem.CacheDirectory => FileSystem.get_CacheDirectory();

		string IFileSystem.AppDataDirectory => FileSystem.get_AppDataDirectory();

		[Preserve(Conditional = true)]
		public FileSystemImplementation()
		{
		}

		Task<Stream> IFileSystem.OpenAppPackageFileAsync(string filename)
		{
			return FileSystem.OpenAppPackageFileAsync(filename);
		}
	}
	public class FlashlightImplementation : IEssentialsImplementation, IFlashlight
	{
		[Preserve(Conditional = true)]
		public FlashlightImplementation()
		{
		}

		Task IFlashlight.TurnOnAsync()
		{
			return Flashlight.TurnOnAsync();
		}

		Task IFlashlight.TurnOffAsync()
		{
			return Flashlight.TurnOffAsync();
		}
	}
	public class GeocodingImplementation : IEssentialsImplementation, IGeocoding
	{
		[Preserve(Conditional = true)]
		public GeocodingImplementation()
		{
		}

		Task<IEnumerable<Placemark>> IGeocoding.GetPlacemarksAsync(Location location)
		{
			return Geocoding.GetPlacemarksAsync(location);
		}

		Task<IEnumerable<Placemark>> IGeocoding.GetPlacemarksAsync(double latitude, double longitude)
		{
			return Geocoding.GetPlacemarksAsync(latitude, longitude);
		}

		Task<IEnumerable<Location>> IGeocoding.GetLocationsAsync(string address)
		{
			return Geocoding.GetLocationsAsync(address);
		}
	}
	public class GeolocationImplementation : IEssentialsImplementation, IGeolocation
	{
		[Preserve(Conditional = true)]
		public GeolocationImplementation()
		{
		}

		Task<Location> IGeolocation.GetLastKnownLocationAsync()
		{
			return Geolocation.GetLastKnownLocationAsync();
		}

		Task<Location> IGeolocation.GetLocationAsync()
		{
			return Geolocation.GetLocationAsync();
		}

		Task<Location> IGeolocation.GetLocationAsync(GeolocationRequest request)
		{
			return Geolocation.GetLocationAsync(request);
		}

		Task<Location> IGeolocation.GetLocationAsync(GeolocationRequest request, CancellationToken cancelToken)
		{
			return Geolocation.GetLocationAsync(request, cancelToken);
		}
	}
	public class GyroscopeImplementation : IEssentialsImplementation, IGyroscope
	{
		bool IGyroscope.IsMonitoring => Gyroscope.get_IsMonitoring();

		event EventHandler<GyroscopeChangedEventArgs> IGyroscope.ReadingChanged
		{
			add
			{
				Gyroscope.add_ReadingChanged(value);
			}
			remove
			{
				Gyroscope.remove_ReadingChanged(value);
			}
		}

		[Preserve(Conditional = true)]
		public GyroscopeImplementation()
		{
		}

		void IGyroscope.Start(SensorSpeed sensorSpeed)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Gyroscope.Start(sensorSpeed);
		}

		void IGyroscope.Stop()
		{
			Gyroscope.Stop();
		}
	}
	public class LauncherImplementation : IEssentialsImplementation, ILauncher
	{
		[Preserve(Conditional = true)]
		public LauncherImplementation()
		{
		}

		Task<bool> ILauncher.CanOpenAsync(string uri)
		{
			return Launcher.CanOpenAsync(uri);
		}

		Task<bool> ILauncher.CanOpenAsync(Uri uri)
		{
			return Launcher.CanOpenAsync(uri);
		}

		Task ILauncher.OpenAsync(string uri)
		{
			return Launcher.OpenAsync(uri);
		}

		Task ILauncher.OpenAsync(Uri uri)
		{
			return Launcher.OpenAsync(uri);
		}

		Task ILauncher.OpenAsync(OpenFileRequest request)
		{
			return Launcher.OpenAsync(request);
		}

		Task<bool> ILauncher.TryOpenAsync(string uri)
		{
			return Launcher.TryOpenAsync(uri);
		}

		Task<bool> ILauncher.TryOpenAsync(Uri uri)
		{
			return Launcher.TryOpenAsync(uri);
		}
	}
	public class MagnetometerImplementation : IEssentialsImplementation, IMagnetometer
	{
		bool IMagnetometer.IsMonitoring => Magnetometer.get_IsMonitoring();

		event EventHandler<MagnetometerChangedEventArgs> IMagnetometer.ReadingChanged
		{
			add
			{
				Magnetometer.add_ReadingChanged(value);
			}
			remove
			{
				Magnetometer.remove_ReadingChanged(value);
			}
		}

		[Preserve(Conditional = true)]
		public MagnetometerImplementation()
		{
		}

		void IMagnetometer.Start(SensorSpeed sensorSpeed)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Magnetometer.Start(sensorSpeed);
		}

		void IMagnetometer.Stop()
		{
			Magnetometer.Stop();
		}
	}
	public class MainThreadImplementation : IEssentialsImplementation, IMainThread
	{
		bool IMainThread.IsMainThread => MainThread.get_IsMainThread();

		[Preserve(Conditional = true)]
		public MainThreadImplementation()
		{
		}

		void IMainThread.BeginInvokeOnMainThread(Action action)
		{
			MainThread.BeginInvokeOnMainThread(action);
		}

		Task IMainThread.InvokeOnMainThreadAsync(Action action)
		{
			return MainThread.InvokeOnMainThreadAsync(action);
		}

		Task<T> IMainThread.InvokeOnMainThreadAsync<T>(Func<T> func)
		{
			return MainThread.InvokeOnMainThreadAsync<T>(func);
		}

		Task IMainThread.InvokeOnMainThreadAsync(Func<Task> funcTask)
		{
			return MainThread.InvokeOnMainThreadAsync(funcTask);
		}

		Task<T> IMainThread.InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
		{
			return MainThread.InvokeOnMainThreadAsync<T>(funcTask);
		}

		Task<SynchronizationContext> IMainThread.GetMainThreadSynchronizationContextAsync()
		{
			return MainThread.GetMainThreadSynchronizationContextAsync();
		}
	}
	public class MapImplementation : IEssentialsImplementation, IMap
	{
		[Preserve(Conditional = true)]
		public MapImplementation()
		{
		}

		Task IMap.OpenAsync(Location location)
		{
			return Map.OpenAsync(location);
		}

		Task IMap.OpenAsync(Location location, MapLaunchOptions options)
		{
			return Map.OpenAsync(location, options);
		}

		Task IMap.OpenAsync(double latitude, double longitude)
		{
			return Map.OpenAsync(latitude, longitude);
		}

		Task IMap.OpenAsync(double latitude, double longitude, MapLaunchOptions options)
		{
			return Map.OpenAsync(latitude, longitude, options);
		}

		Task IMap.OpenAsync(Placemark placemark)
		{
			return Map.OpenAsync(placemark);
		}

		Task IMap.OpenAsync(Placemark placemark, MapLaunchOptions options)
		{
			return Map.OpenAsync(placemark, options);
		}
	}
	public class OrientationSensorImplementation : IEssentialsImplementation, IOrientationSensor
	{
		bool IOrientationSensor.IsMonitoring => OrientationSensor.get_IsMonitoring();

		event EventHandler<OrientationSensorChangedEventArgs> IOrientationSensor.ReadingChanged
		{
			add
			{
				OrientationSensor.add_ReadingChanged(value);
			}
			remove
			{
				OrientationSensor.remove_ReadingChanged(value);
			}
		}

		[Preserve(Conditional = true)]
		public OrientationSensorImplementation()
		{
		}

		void IOrientationSensor.Start(SensorSpeed sensorSpeed)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			OrientationSensor.Start(sensorSpeed);
		}

		void IOrientationSensor.Stop()
		{
			OrientationSensor.Stop();
		}
	}
	public class PermissionsImplementation : IEssentialsImplementation, IPermissions
	{
		[Preserve(Conditional = true)]
		public PermissionsImplementation()
		{
		}

		Task<PermissionStatus> IPermissions.CheckStatusAsync<TPermission>()
		{
			return Permissions.CheckStatusAsync<TPermission>();
		}

		Task<PermissionStatus> IPermissions.RequestAsync<TPermission>()
		{
			return Permissions.RequestAsync<TPermission>();
		}
	}
	public class PhoneDialerImplementation : IEssentialsImplementation, IPhoneDialer
	{
		[Preserve(Conditional = true)]
		public PhoneDialerImplementation()
		{
		}

		void IPhoneDialer.Open(string number)
		{
			PhoneDialer.Open(number);
		}
	}
	public class PreferencesImplementation : IEssentialsImplementation, IPreferences
	{
		[Preserve(Conditional = true)]
		public PreferencesImplementation()
		{
		}

		bool IPreferences.ContainsKey(string key)
		{
			return Preferences.ContainsKey(key);
		}

		void IPreferences.Remove(string key)
		{
			Preferences.Remove(key);
		}

		void IPreferences.Clear()
		{
			Preferences.Clear();
		}

		string IPreferences.Get(string key, string defaultValue)
		{
			return Preferences.Get(key, defaultValue);
		}

		bool IPreferences.Get(string key, bool defaultValue)
		{
			return Preferences.Get(key, defaultValue);
		}

		int IPreferences.Get(string key, int defaultValue)
		{
			return Preferences.Get(key, defaultValue);
		}

		double IPreferences.Get(string key, double defaultValue)
		{
			return Preferences.Get(key, defaultValue);
		}

		float IPreferences.Get(string key, float defaultValue)
		{
			return Preferences.Get(key, defaultValue);
		}

		long IPreferences.Get(string key, long defaultValue)
		{
			return Preferences.Get(key, defaultValue);
		}

		void IPreferences.Set(string key, string value)
		{
			Preferences.Set(key, value);
		}

		void IPreferences.Set(string key, bool value)
		{
			Preferences.Set(key, value);
		}

		void IPreferences.Set(string key, int value)
		{
			Preferences.Set(key, value);
		}

		void IPreferences.Set(string key, double value)
		{
			Preferences.Set(key, value);
		}

		void IPreferences.Set(string key, float value)
		{
			Preferences.Set(key, value);
		}

		void IPreferences.Set(string key, long value)
		{
			Preferences.Set(key, value);
		}

		bool IPreferences.ContainsKey(string key, string sharedName)
		{
			return Preferences.ContainsKey(key, sharedName);
		}

		void IPreferences.Remove(string key, string sharedName)
		{
			Preferences.Remove(key, sharedName);
		}

		void IPreferences.Clear(string sharedName)
		{
			Preferences.Clear(sharedName);
		}

		string IPreferences.Get(string key, string defaultValue, string sharedName)
		{
			return Preferences.Get(key, defaultValue, sharedName);
		}

		bool IPreferences.Get(string key, bool defaultValue, string sharedName)
		{
			return Preferences.Get(key, defaultValue, sharedName);
		}

		int IPreferences.Get(string key, int defaultValue, string sharedName)
		{
			return Preferences.Get(key, defaultValue, sharedName);
		}

		double IPreferences.Get(string key, double defaultValue, string sharedName)
		{
			return Preferences.Get(key, defaultValue, sharedName);
		}

		float IPreferences.Get(string key, float defaultValue, string sharedName)
		{
			return Preferences.Get(key, defaultValue, sharedName);
		}

		long IPreferences.Get(string key, long defaultValue, string sharedName)
		{
			return Preferences.Get(key, defaultValue, sharedName);
		}

		void IPreferences.Set(string key, string value, string sharedName)
		{
			Preferences.Set(key, value, sharedName);
		}

		void IPreferences.Set(string key, bool value, string sharedName)
		{
			Preferences.Set(key, value, sharedName);
		}

		void IPreferences.Set(string key, int value, string sharedName)
		{
			Preferences.Set(key, value, sharedName);
		}

		void IPreferences.Set(string key, double value, string sharedName)
		{
			Preferences.Set(key, value, sharedName);
		}

		void IPreferences.Set(string key, float value, string sharedName)
		{
			Preferences.Set(key, value, sharedName);
		}

		void IPreferences.Set(string key, long value, string sharedName)
		{
			Preferences.Set(key, value, sharedName);
		}

		DateTime IPreferences.Get(string key, DateTime defaultValue)
		{
			return Preferences.Get(key, defaultValue);
		}

		void IPreferences.Set(string key, DateTime value)
		{
			Preferences.Set(key, value);
		}

		DateTime IPreferences.Get(string key, DateTime defaultValue, string sharedName)
		{
			return Preferences.Get(key, defaultValue, sharedName);
		}

		void IPreferences.Set(string key, DateTime value, string sharedName)
		{
			Preferences.Set(key, value, sharedName);
		}
	}
	public class SecureStorageImplementation : IEssentialsImplementation, ISecureStorage
	{
		[Preserve(Conditional = true)]
		public SecureStorageImplementation()
		{
		}

		Task<string> ISecureStorage.GetAsync(string key)
		{
			return SecureStorage.GetAsync(key);
		}

		Task ISecureStorage.SetAsync(string key, string value)
		{
			return SecureStorage.SetAsync(key, value);
		}

		bool ISecureStorage.Remove(string key)
		{
			return SecureStorage.Remove(key);
		}

		void ISecureStorage.RemoveAll()
		{
			SecureStorage.RemoveAll();
		}
	}
	public class ShareImplementation : IEssentialsImplementation, IShare
	{
		[Preserve(Conditional = true)]
		public ShareImplementation()
		{
		}

		Task IShare.RequestAsync(string text)
		{
			return Share.RequestAsync(text);
		}

		Task IShare.RequestAsync(string text, string title)
		{
			return Share.RequestAsync(text, title);
		}

		Task IShare.RequestAsync(ShareTextRequest request)
		{
			return Share.RequestAsync(request);
		}

		Task IShare.RequestAsync(ShareFileRequest request)
		{
			return Share.RequestAsync(request);
		}
	}
	public class SmsImplementation : IEssentialsImplementation, ISms
	{
		[Preserve(Conditional = true)]
		public SmsImplementation()
		{
		}

		Task ISms.ComposeAsync()
		{
			return Sms.ComposeAsync();
		}

		Task ISms.ComposeAsync(SmsMessage message)
		{
			return Sms.ComposeAsync(message);
		}
	}
	public class TextToSpeechImplementation : IEssentialsImplementation, ITextToSpeech
	{
		[Preserve(Conditional = true)]
		public TextToSpeechImplementation()
		{
		}

		Task<IEnumerable<Locale>> ITextToSpeech.GetLocalesAsync()
		{
			return TextToSpeech.GetLocalesAsync();
		}

		Task ITextToSpeech.SpeakAsync(string text, CancellationToken cancelToken = default(CancellationToken))
		{
			return TextToSpeech.SpeakAsync(text, cancelToken);
		}

		Task ITextToSpeech.SpeakAsync(string text, SpeechOptions options, CancellationToken cancelToken = default(CancellationToken))
		{
			return TextToSpeech.SpeakAsync(text, options, cancelToken);
		}
	}
	public class VersionTrackingImplementation : IEssentialsImplementation, IVersionTracking
	{
		bool IVersionTracking.IsFirstLaunchEver => VersionTracking.get_IsFirstLaunchEver();

		bool IVersionTracking.IsFirstLaunchForCurrentVersion => VersionTracking.get_IsFirstLaunchForCurrentVersion();

		bool IVersionTracking.IsFirstLaunchForCurrentBuild => VersionTracking.get_IsFirstLaunchForCurrentBuild();

		string IVersionTracking.CurrentVersion => VersionTracking.get_CurrentVersion();

		string IVersionTracking.CurrentBuild => VersionTracking.get_CurrentBuild();

		string IVersionTracking.PreviousVersion => VersionTracking.get_PreviousVersion();

		string IVersionTracking.PreviousBuild => VersionTracking.get_PreviousBuild();

		string IVersionTracking.FirstInstalledVersion => VersionTracking.get_FirstInstalledVersion();

		string IVersionTracking.FirstInstalledBuild => VersionTracking.get_FirstInstalledBuild();

		IEnumerable<string> IVersionTracking.VersionHistory => VersionTracking.get_VersionHistory();

		IEnumerable<string> IVersionTracking.BuildHistory => VersionTracking.get_BuildHistory();

		[Preserve(Conditional = true)]
		public VersionTrackingImplementation()
		{
		}

		void IVersionTracking.Track()
		{
			VersionTracking.Track();
		}

		bool IVersionTracking.IsFirstLaunchForVersion(string version)
		{
			return VersionTracking.IsFirstLaunchForVersion(version);
		}

		bool IVersionTracking.IsFirstLaunchForBuild(string build)
		{
			return VersionTracking.IsFirstLaunchForBuild(build);
		}
	}
	public class VibrationImplementation : IEssentialsImplementation, IVibration
	{
		[Preserve(Conditional = true)]
		public VibrationImplementation()
		{
		}

		void IVibration.Vibrate()
		{
			Vibration.Vibrate();
		}

		void IVibration.Vibrate(double duration)
		{
			Vibration.Vibrate(duration);
		}

		void IVibration.Vibrate(TimeSpan duration)
		{
			Vibration.Vibrate(duration);
		}

		void IVibration.Cancel()
		{
			Vibration.Cancel();
		}
	}
	public class WebAuthenticatorImplementation : IEssentialsImplementation, IWebAuthenticator
	{
		[Preserve(Conditional = true)]
		public WebAuthenticatorImplementation()
		{
		}

		Task<WebAuthenticatorResult> IWebAuthenticator.AuthenticateAsync(Uri url, Uri callbackUrl)
		{
			return WebAuthenticator.AuthenticateAsync(url, callbackUrl);
		}
	}
}
namespace NDB.Covid19.Interfaces
{
	public interface IApiDataHelper
	{
		bool IsGoogleServiceEnabled();

		string GetBackGroundServicVersionLogString();
	}
	public interface ILocalNotificationsManager
	{
		void GenerateLocalNotification(NotificationViewModel notificationViewModel, int triggerInSeconds);

		void GenerateLocalNotificationOnlyIfInBackground(NotificationViewModel viewModel);
	}
	public interface IMessagingCenter
	{
		void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class;

		void Send<TSender>(TSender sender, string message) where TSender : class;

		void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class;

		void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class;

		void Unsubscribe<TSender, TArgs>(object subscriber, string message) where TSender : class;

		void Unsubscribe<TSender>(object subscriber, string message) where TSender : class;
	}
	public interface ISecureStorageService
	{
		ISecureStorage SecureStorage
		{
			get;
		}
	}
	public interface IEssentialsImplementation
	{
	}
	public interface IAccelerometer
	{
		bool IsMonitoring
		{
			get;
		}

		event EventHandler<AccelerometerChangedEventArgs> ReadingChanged;

		event EventHandler ShakeDetected;

		void Start(SensorSpeed sensorSpeed);

		void Stop();
	}
	public interface IAppInfo
	{
		string PackageName
		{
			get;
		}

		string Name
		{
			get;
		}

		string VersionString
		{
			get;
		}

		Version Version
		{
			get;
		}

		string BuildString
		{
			get;
		}

		AppTheme RequestedTheme
		{
			get;
		}

		void ShowSettingsUI();
	}
	public interface IBarometer
	{
		bool IsMonitoring
		{
			get;
		}

		event EventHandler<BarometerChangedEventArgs> ReadingChanged;

		void Start(SensorSpeed sensorSpeed);

		void Stop();
	}
	public interface IBattery
	{
		double ChargeLevel
		{
			get;
		}

		BatteryState State
		{
			get;
		}

		BatteryPowerSource PowerSource
		{
			get;
		}

		EnergySaverStatus EnergySaverStatus
		{
			get;
		}

		event EventHandler<BatteryInfoChangedEventArgs> BatteryInfoChanged;

		event EventHandler<EnergySaverStatusChangedEventArgs> EnergySaverStatusChanged;
	}
	public interface IBrowser
	{
		Task OpenAsync(string uri);

		Task OpenAsync(string uri, BrowserLaunchMode launchMode);

		Task OpenAsync(string uri, BrowserLaunchOptions options);

		Task OpenAsync(Uri uri);

		Task OpenAsync(Uri uri, BrowserLaunchMode launchMode);

		Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options);
	}
	public interface IClipboard
	{
		bool HasText
		{
			get;
		}

		event EventHandler<EventArgs> ClipboardContentChanged;

		Task SetTextAsync(string text);

		Task<string> GetTextAsync();
	}
	public interface ICompass
	{
		bool IsMonitoring
		{
			get;
		}

		event EventHandler<CompassChangedEventArgs> ReadingChanged;

		void Start(SensorSpeed sensorSpeed);

		void Start(SensorSpeed sensorSpeed, bool applyLowPassFilter);

		void Stop();
	}
	public interface IConnectivity
	{
		NetworkAccess NetworkAccess
		{
			get;
		}

		IEnumerable<ConnectionProfile> ConnectionProfiles
		{
			get;
		}

		event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;
	}
	public interface IDeviceDisplay
	{
		bool KeepScreenOn
		{
			get;
			set;
		}

		DisplayInfo MainDisplayInfo
		{
			get;
		}

		event EventHandler<DisplayInfoChangedEventArgs> MainDisplayInfoChanged;
	}
	public interface IDeviceInfo
	{
		string Model
		{
			get;
		}

		string Manufacturer
		{
			get;
		}

		string Name
		{
			get;
		}

		string VersionString
		{
			get;
		}

		Version Version
		{
			get;
		}

		DevicePlatform Platform
		{
			get;
		}

		DeviceIdiom Idiom
		{
			get;
		}

		DeviceType DeviceType
		{
			get;
		}
	}
	public interface IEmail
	{
		Task ComposeAsync();

		Task ComposeAsync(string subject, string body, params string[] to);

		Task ComposeAsync(EmailMessage message);
	}
	public interface IFileSystem
	{
		string CacheDirectory
		{
			get;
		}

		string AppDataDirectory
		{
			get;
		}

		Task<Stream> OpenAppPackageFileAsync(string filename);
	}
	public interface IFlashlight
	{
		Task TurnOnAsync();

		Task TurnOffAsync();
	}
	public interface IGeocoding
	{
		Task<IEnumerable<Placemark>> GetPlacemarksAsync(Location location);

		Task<IEnumerable<Placemark>> GetPlacemarksAsync(double latitude, double longitude);

		Task<IEnumerable<Location>> GetLocationsAsync(string address);
	}
	public interface IGeolocation
	{
		Task<Location> GetLastKnownLocationAsync();

		Task<Location> GetLocationAsync();

		Task<Location> GetLocationAsync(GeolocationRequest request);

		Task<Location> GetLocationAsync(GeolocationRequest request, CancellationToken cancelToken);
	}
	public interface IGyroscope
	{
		bool IsMonitoring
		{
			get;
		}

		event EventHandler<GyroscopeChangedEventArgs> ReadingChanged;

		void Start(SensorSpeed sensorSpeed);

		void Stop();
	}
	public interface ILauncher
	{
		Task<bool> CanOpenAsync(string uri);

		Task<bool> CanOpenAsync(Uri uri);

		Task OpenAsync(string uri);

		Task OpenAsync(Uri uri);

		Task OpenAsync(OpenFileRequest request);

		Task<bool> TryOpenAsync(string uri);

		Task<bool> TryOpenAsync(Uri uri);
	}
	public interface IMagnetometer
	{
		bool IsMonitoring
		{
			get;
		}

		event EventHandler<MagnetometerChangedEventArgs> ReadingChanged;

		void Start(SensorSpeed sensorSpeed);

		void Stop();
	}
	public interface IMainThread
	{
		bool IsMainThread
		{
			get;
		}

		void BeginInvokeOnMainThread(Action action);

		Task InvokeOnMainThreadAsync(Action action);

		Task<T> InvokeOnMainThreadAsync<T>(Func<T> func);

		Task InvokeOnMainThreadAsync(Func<Task> funcTask);

		Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask);

		Task<SynchronizationContext> GetMainThreadSynchronizationContextAsync();
	}
	public interface IMap
	{
		Task OpenAsync(Location location);

		Task OpenAsync(Location location, MapLaunchOptions options);

		Task OpenAsync(double latitude, double longitude);

		Task OpenAsync(double latitude, double longitude, MapLaunchOptions options);

		Task OpenAsync(Placemark placemark);

		Task OpenAsync(Placemark placemark, MapLaunchOptions options);
	}
	public interface IOrientationSensor
	{
		bool IsMonitoring
		{
			get;
		}

		event EventHandler<OrientationSensorChangedEventArgs> ReadingChanged;

		void Start(SensorSpeed sensorSpeed);

		void Stop();
	}
	public interface IPermissions
	{
		Task<PermissionStatus> CheckStatusAsync<TPermission>() where TPermission : BasePermission, new();

		Task<PermissionStatus> RequestAsync<TPermission>() where TPermission : BasePermission, new();
	}
	public interface IPhoneDialer
	{
		void Open(string number);
	}
	public interface IPreferences
	{
		bool ContainsKey(string key);

		void Remove(string key);

		void Clear();

		string Get(string key, string defaultValue);

		bool Get(string key, bool defaultValue);

		int Get(string key, int defaultValue);

		double Get(string key, double defaultValue);

		float Get(string key, float defaultValue);

		long Get(string key, long defaultValue);

		void Set(string key, string value);

		void Set(string key, bool value);

		void Set(string key, int value);

		void Set(string key, double value);

		void Set(string key, float value);

		void Set(string key, long value);

		bool ContainsKey(string key, string sharedName);

		void Remove(string key, string sharedName);

		void Clear(string sharedName);

		string Get(string key, string defaultValue, string sharedName);

		bool Get(string key, bool defaultValue, string sharedName);

		int Get(string key, int defaultValue, string sharedName);

		double Get(string key, double defaultValue, string sharedName);

		float Get(string key, float defaultValue, string sharedName);

		long Get(string key, long defaultValue, string sharedName);

		void Set(string key, string value, string sharedName);

		void Set(string key, bool value, string sharedName);

		void Set(string key, int value, string sharedName);

		void Set(string key, double value, string sharedName);

		void Set(string key, float value, string sharedName);

		void Set(string key, long value, string sharedName);

		DateTime Get(string key, DateTime defaultValue);

		void Set(string key, DateTime value);

		DateTime Get(string key, DateTime defaultValue, string sharedName);

		void Set(string key, DateTime value, string sharedName);
	}
	public interface ISecureStorage
	{
		Task<string> GetAsync(string key);

		Task SetAsync(string key, string value);

		bool Remove(string key);

		void RemoveAll();
	}
	public interface IShare
	{
		Task RequestAsync(string text);

		Task RequestAsync(string text, string title);

		Task RequestAsync(ShareTextRequest request);

		Task RequestAsync(ShareFileRequest request);
	}
	public interface ISms
	{
		Task ComposeAsync();

		Task ComposeAsync(SmsMessage message);
	}
	public interface ITextToSpeech
	{
		Task<IEnumerable<Locale>> GetLocalesAsync();

		Task SpeakAsync(string text, CancellationToken cancelToken = default(CancellationToken));

		Task SpeakAsync(string text, SpeechOptions options, CancellationToken cancelToken = default(CancellationToken));
	}
	public interface IVersionTracking
	{
		bool IsFirstLaunchEver
		{
			get;
		}

		bool IsFirstLaunchForCurrentVersion
		{
			get;
		}

		bool IsFirstLaunchForCurrentBuild
		{
			get;
		}

		string CurrentVersion
		{
			get;
		}

		string CurrentBuild
		{
			get;
		}

		string PreviousVersion
		{
			get;
		}

		string PreviousBuild
		{
			get;
		}

		string FirstInstalledVersion
		{
			get;
		}

		string FirstInstalledBuild
		{
			get;
		}

		IEnumerable<string> VersionHistory
		{
			get;
		}

		IEnumerable<string> BuildHistory
		{
			get;
		}

		void Track();

		bool IsFirstLaunchForVersion(string version);

		bool IsFirstLaunchForBuild(string build);
	}
	public interface IVibration
	{
		void Vibrate();

		void Vibrate(double duration);

		void Vibrate(TimeSpan duration);

		void Cancel();
	}
	public interface IWebAuthenticator
	{
		Task<WebAuthenticatorResult> AuthenticateAsync(Uri url, Uri callbackUrl);
	}
}
namespace NDB.Covid19.ProtoModels
{
	public static class TemporaryExposureKeyBatchReflection
	{
		private static FileDescriptor descriptor;

		public static FileDescriptor Descriptor => descriptor;

		static TemporaryExposureKeyBatchReflection()
		{
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Expected O, but got Unknown
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Expected O, but got Unknown
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Expected O, but got Unknown
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Expected O, but got Unknown
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Expected O, but got Unknown
			descriptor = FileDescriptor.FromGeneratedCode(Convert.FromBase64String("Ch9UZW1wb3JhcnlFeHBvc3VyZUtleUJhdGNoLnByb3RvItEBChpUZW1wb3Jh" + "cnlFeHBvc3VyZUtleUV4cG9ydBIXCg9zdGFydF90aW1lc3RhbXAYASABKAYS" + "FQoNZW5kX3RpbWVzdGFtcBgCIAEoBhIOCgZyZWdpb24YAyABKAkSEQoJYmF0" + "Y2hfbnVtGAQgASgFEhIKCmJhdGNoX3NpemUYBSABKAUSJwoPc2lnbmF0dXJl" + "X2luZm9zGAYgAygLMg4uU2lnbmF0dXJlSW5mbxIjCgRrZXlzGAcgAygLMhUu" + "VGVtcG9yYXJ5RXhwb3N1cmVLZXkimwEKDVNpZ25hdHVyZUluZm8SFQoNYXBw" + "X2J1bmRsZV9pZBgBIAEoCRIXCg9hbmRyb2lkX3BhY2thZ2UYAiABKAkSIAoY" + "dmVyaWZpY2F0aW9uX2tleV92ZXJzaW9uGAMgASgJEhsKE3ZlcmlmaWNhdGlv" + "bl9rZXlfaWQYBCABKAkSGwoTc2lnbmF0dXJlX2FsZ29yaXRobRgFIAEoCSKN" + "AQoUVGVtcG9yYXJ5RXhwb3N1cmVLZXkSEAoIa2V5X2RhdGEYASABKAwSHwoX" + "dHJhbnNtaXNzaW9uX3Jpc2tfbGV2ZWwYAiABKAUSJQodcm9sbGluZ19zdGFy" + "dF9pbnRlcnZhbF9udW1iZXIYAyABKAUSGwoOcm9sbGluZ19wZXJpb2QYBCAB" + "KAU6AzE0NCI1ChBURUtTaWduYXR1cmVMaXN0EiEKCnNpZ25hdHVyZXMYASAD" + "KAsyDS5URUtTaWduYXR1cmUicAoMVEVLU2lnbmF0dXJlEiYKDnNpZ25hdHVy" + "ZV9pbmZvGAEgASgLMg4uU2lnbmF0dXJlSW5mbxIRCgliYXRjaF9udW0YAiAB" + "KAUSEgoKYmF0Y2hfc2l6ZRgDIAEoBRIRCglzaWduYXR1cmUYBCABKAxCKaoC" + "JkV4cG9zdXJlTm90aWZpY2F0aW9uLkJhY2tlbmQuRnVuY3Rpb25z"), (FileDescriptor[])(object)new FileDescriptor[0], new GeneratedClrTypeInfo((Type[])null, (Extension[])null, (GeneratedClrTypeInfo[])(object)new GeneratedClrTypeInfo[5]
			{
				new GeneratedClrTypeInfo(typeof(TemporaryExposureKeyExport), (MessageParser)(object)TemporaryExposureKeyExport.Parser, new string[7]
				{
					"StartTimestamp",
					"EndTimestamp",
					"Region",
					"BatchNum",
					"BatchSize",
					"SignatureInfos",
					"Keys"
				}, (string[])null, (Type[])null, (Extension[])null, (GeneratedClrTypeInfo[])null),
				new GeneratedClrTypeInfo(typeof(SignatureInfo), (MessageParser)(object)SignatureInfo.Parser, new string[5]
				{
					"AppBundleId",
					"AndroidPackage",
					"VerificationKeyVersion",
					"VerificationKeyId",
					"SignatureAlgorithm"
				}, (string[])null, (Type[])null, (Extension[])null, (GeneratedClrTypeInfo[])null),
				new GeneratedClrTypeInfo(typeof(TemporaryExposureKey), (MessageParser)(object)TemporaryExposureKey.Parser, new string[4]
				{
					"KeyData",
					"TransmissionRiskLevel",
					"RollingStartIntervalNumber",
					"RollingPeriod"
				}, (string[])null, (Type[])null, (Extension[])null, (GeneratedClrTypeInfo[])null),
				new GeneratedClrTypeInfo(typeof(TEKSignatureList), (MessageParser)(object)TEKSignatureList.Parser, new string[1]
				{
					"Signatures"
				}, (string[])null, (Type[])null, (Extension[])null, (GeneratedClrTypeInfo[])null),
				new GeneratedClrTypeInfo(typeof(TEKSignature), (MessageParser)(object)TEKSignature.Parser, new string[4]
				{
					"SignatureInfo",
					"BatchNum",
					"BatchSize",
					"Signature"
				}, (string[])null, (Type[])null, (Extension[])null, (GeneratedClrTypeInfo[])null)
			}));
		}
	}
	public sealed class TemporaryExposureKeyExport : IMessage<TemporaryExposureKeyExport>, IMessage, IEquatable<TemporaryExposureKeyExport>, IDeepCloneable<TemporaryExposureKeyExport>
	{
		private static readonly MessageParser<TemporaryExposureKeyExport> _parser = new MessageParser<TemporaryExposureKeyExport>((Func<TemporaryExposureKeyExport>)(() => new TemporaryExposureKeyExport()));

		private UnknownFieldSet _unknownFields;

		private int _hasBits0;

		public const int StartTimestampFieldNumber = 1;

		private static readonly ulong StartTimestampDefaultValue = 0uL;

		private ulong startTimestamp_;

		public const int EndTimestampFieldNumber = 2;

		private static readonly ulong EndTimestampDefaultValue = 0uL;

		private ulong endTimestamp_;

		public const int RegionFieldNumber = 3;

		private static readonly string RegionDefaultValue = "";

		private string region_;

		public const int BatchNumFieldNumber = 4;

		private static readonly int BatchNumDefaultValue = 0;

		private int batchNum_;

		public const int BatchSizeFieldNumber = 5;

		private static readonly int BatchSizeDefaultValue = 0;

		private int batchSize_;

		public const int SignatureInfosFieldNumber = 6;

		private static readonly FieldCodec<SignatureInfo> _repeated_signatureInfos_codec = FieldCodec.ForMessage<SignatureInfo>(50u, SignatureInfo.Parser);

		private readonly RepeatedField<SignatureInfo> signatureInfos_ = new RepeatedField<SignatureInfo>();

		public const int KeysFieldNumber = 7;

		private static readonly FieldCodec<TemporaryExposureKey> _repeated_keys_codec = FieldCodec.ForMessage<TemporaryExposureKey>(58u, TemporaryExposureKey.Parser);

		private readonly RepeatedField<TemporaryExposureKey> keys_ = new RepeatedField<TemporaryExposureKey>();

		[DebuggerNonUserCode]
		public static MessageParser<TemporaryExposureKeyExport> Parser => _parser;

		[DebuggerNonUserCode]
		public static MessageDescriptor Descriptor => TemporaryExposureKeyBatchReflection.Descriptor.get_MessageTypes()[0];

		[DebuggerNonUserCode]
		MessageDescriptor Descriptor => Descriptor;

		[DebuggerNonUserCode]
		public ulong StartTimestamp
		{
			get
			{
				if (((uint)_hasBits0 & (true ? 1u : 0u)) != 0)
				{
					return startTimestamp_;
				}
				return StartTimestampDefaultValue;
			}
			set
			{
				_hasBits0 |= 1;
				startTimestamp_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool HasStartTimestamp => (_hasBits0 & 1) != 0;

		[DebuggerNonUserCode]
		public ulong EndTimestamp
		{
			get
			{
				if (((uint)_hasBits0 & 2u) != 0)
				{
					return endTimestamp_;
				}
				return EndTimestampDefaultValue;
			}
			set
			{
				_hasBits0 |= 2;
				endTimestamp_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool HasEndTimestamp => (_hasBits0 & 2) != 0;

		[DebuggerNonUserCode]
		public string Region
		{
			get
			{
				return region_ ?? RegionDefaultValue;
			}
			set
			{
				region_ = ProtoPreconditions.CheckNotNull<string>(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool HasRegion => region_ != null;

		[DebuggerNonUserCode]
		public int BatchNum
		{
			get
			{
				if (((uint)_hasBits0 & 4u) != 0)
				{
					return batchNum_;
				}
				return BatchNumDefaultValue;
			}
			set
			{
				_hasBits0 |= 4;
				batchNum_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool HasBatchNum => (_hasBits0 & 4) != 0;

		[DebuggerNonUserCode]
		public int BatchSize
		{
			get
			{
				if (((uint)_hasBits0 & 8u) != 0)
				{
					return batchSize_;
				}
				return BatchSizeDefaultValue;
			}
			set
			{
				_hasBits0 |= 8;
				batchSize_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool HasBatchSize => (_hasBits0 & 8) != 0;

		[DebuggerNonUserCode]
		public RepeatedField<SignatureInfo> SignatureInfos => signatureInfos_;

		[DebuggerNonUserCode]
		public RepeatedField<TemporaryExposureKey> Keys => keys_;

		[DebuggerNonUserCode]
		public TemporaryExposureKeyExport()
		{
		}

		[DebuggerNonUserCode]
		public TemporaryExposureKeyExport(TemporaryExposureKeyExport other)
			: this()
		{
			_hasBits0 = other._hasBits0;
			startTimestamp_ = other.startTimestamp_;
			endTimestamp_ = other.endTimestamp_;
			region_ = other.region_;
			batchNum_ = other.batchNum_;
			batchSize_ = other.batchSize_;
			signatureInfos_ = other.signatureInfos_.Clone();
			keys_ = other.keys_.Clone();
			_unknownFields = UnknownFieldSet.Clone(other._unknownFields);
		}

		[DebuggerNonUserCode]
		public TemporaryExposureKeyExport Clone()
		{
			return new TemporaryExposureKeyExport(this);
		}

		[DebuggerNonUserCode]
		public void ClearStartTimestamp()
		{
			_hasBits0 &= -2;
		}

		[DebuggerNonUserCode]
		public void ClearEndTimestamp()
		{
			_hasBits0 &= -3;
		}

		[DebuggerNonUserCode]
		public void ClearRegion()
		{
			region_ = null;
		}

		[DebuggerNonUserCode]
		public void ClearBatchNum()
		{
			_hasBits0 &= -5;
		}

		[DebuggerNonUserCode]
		public void ClearBatchSize()
		{
			_hasBits0 &= -9;
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as TemporaryExposureKeyExport);
		}

		[DebuggerNonUserCode]
		public bool Equals(TemporaryExposureKeyExport other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (StartTimestamp != other.StartTimestamp)
			{
				return false;
			}
			if (EndTimestamp != other.EndTimestamp)
			{
				return false;
			}
			if (Region != other.Region)
			{
				return false;
			}
			if (BatchNum != other.BatchNum)
			{
				return false;
			}
			if (BatchSize != other.BatchSize)
			{
				return false;
			}
			if (!signatureInfos_.Equals(other.signatureInfos_))
			{
				return false;
			}
			if (!keys_.Equals(other.keys_))
			{
				return false;
			}
			return object.Equals(_unknownFields, other._unknownFields);
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (HasStartTimestamp)
			{
				num ^= StartTimestamp.GetHashCode();
			}
			if (HasEndTimestamp)
			{
				num ^= EndTimestamp.GetHashCode();
			}
			if (HasRegion)
			{
				num ^= Region.GetHashCode();
			}
			if (HasBatchNum)
			{
				num ^= BatchNum.GetHashCode();
			}
			if (HasBatchSize)
			{
				num ^= BatchSize.GetHashCode();
			}
			num ^= ((object)signatureInfos_).GetHashCode();
			num ^= ((object)keys_).GetHashCode();
			if (_unknownFields != null)
			{
				num ^= ((object)_unknownFields).GetHashCode();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString((IMessage)(object)this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (HasStartTimestamp)
			{
				output.WriteRawTag((byte)9);
				output.WriteFixed64(StartTimestamp);
			}
			if (HasEndTimestamp)
			{
				output.WriteRawTag((byte)17);
				output.WriteFixed64(EndTimestamp);
			}
			if (HasRegion)
			{
				output.WriteRawTag((byte)26);
				output.WriteString(Region);
			}
			if (HasBatchNum)
			{
				output.WriteRawTag((byte)32);
				output.WriteInt32(BatchNum);
			}
			if (HasBatchSize)
			{
				output.WriteRawTag((byte)40);
				output.WriteInt32(BatchSize);
			}
			signatureInfos_.WriteTo(output, _repeated_signatureInfos_codec);
			keys_.WriteTo(output, _repeated_keys_codec);
			if (_unknownFields != null)
			{
				_unknownFields.WriteTo(output);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (HasStartTimestamp)
			{
				num += 9;
			}
			if (HasEndTimestamp)
			{
				num += 9;
			}
			if (HasRegion)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Region);
			}
			if (HasBatchNum)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(BatchNum);
			}
			if (HasBatchSize)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(BatchSize);
			}
			num += signatureInfos_.CalculateSize(_repeated_signatureInfos_codec);
			num += keys_.CalculateSize(_repeated_keys_codec);
			if (_unknownFields != null)
			{
				num += _unknownFields.CalculateSize();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(TemporaryExposureKeyExport other)
		{
			if (other != null)
			{
				if (other.HasStartTimestamp)
				{
					StartTimestamp = other.StartTimestamp;
				}
				if (other.HasEndTimestamp)
				{
					EndTimestamp = other.EndTimestamp;
				}
				if (other.HasRegion)
				{
					Region = other.Region;
				}
				if (other.HasBatchNum)
				{
					BatchNum = other.BatchNum;
				}
				if (other.HasBatchSize)
				{
					BatchSize = other.BatchSize;
				}
				signatureInfos_.Add((IEnumerable<SignatureInfo>)other.signatureInfos_);
				keys_.Add((IEnumerable<TemporaryExposureKey>)other.keys_);
				_unknownFields = UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				switch (num)
				{
				default:
					_unknownFields = UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
					break;
				case 9u:
					StartTimestamp = input.ReadFixed64();
					break;
				case 17u:
					EndTimestamp = input.ReadFixed64();
					break;
				case 26u:
					Region = input.ReadString();
					break;
				case 32u:
					BatchNum = input.ReadInt32();
					break;
				case 40u:
					BatchSize = input.ReadInt32();
					break;
				case 50u:
					signatureInfos_.AddEntriesFrom(input, _repeated_signatureInfos_codec);
					break;
				case 58u:
					keys_.AddEntriesFrom(input, _repeated_keys_codec);
					break;
				}
			}
		}
	}
	public sealed class SignatureInfo : IMessage<SignatureInfo>, IMessage, IEquatable<SignatureInfo>, IDeepCloneable<SignatureInfo>
	{
		private static readonly MessageParser<SignatureInfo> _parser = new MessageParser<SignatureInfo>((Func<SignatureInfo>)(() => new SignatureInfo()));

		private UnknownFieldSet _unknownFields;

		public const int AppBundleIdFieldNumber = 1;

		private static readonly string AppBundleIdDefaultValue = "";

		private string appBundleId_;

		public const int AndroidPackageFieldNumber = 2;

		private static readonly string AndroidPackageDefaultValue = "";

		private string androidPackage_;

		public const int VerificationKeyVersionFieldNumber = 3;

		private static readonly string VerificationKeyVersionDefaultValue = "";

		private string verificationKeyVersion_;

		public const int VerificationKeyIdFieldNumber = 4;

		private static readonly string VerificationKeyIdDefaultValue = "";

		private string verificationKeyId_;

		public const int SignatureAlgorithmFieldNumber = 5;

		private static readonly string SignatureAlgorithmDefaultValue = "";

		private string signatureAlgorithm_;

		[DebuggerNonUserCode]
		public static MessageParser<SignatureInfo> Parser => _parser;

		[DebuggerNonUserCode]
		public static MessageDescriptor Descriptor => TemporaryExposureKeyBatchReflection.Descriptor.get_MessageTypes()[1];

		[DebuggerNonUserCode]
		MessageDescriptor Descriptor => Descriptor;

		[DebuggerNonUserCode]
		public string AppBundleId
		{
			get
			{
				return appBundleId_ ?? AppBundleIdDefaultValue;
			}
			set
			{
				appBundleId_ = ProtoPreconditions.CheckNotNull<string>(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool HasAppBundleId => appBundleId_ != null;

		[DebuggerNonUserCode]
		public string AndroidPackage
		{
			get
			{
				return androidPackage_ ?? AndroidPackageDefaultValue;
			}
			set
			{
				androidPackage_ = ProtoPreconditions.CheckNotNull<string>(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool HasAndroidPackage => androidPackage_ != null;

		[DebuggerNonUserCode]
		public string VerificationKeyVersion
		{
			get
			{
				return verificationKeyVersion_ ?? VerificationKeyVersionDefaultValue;
			}
			set
			{
				verificationKeyVersion_ = ProtoPreconditions.CheckNotNull<string>(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool HasVerificationKeyVersion => verificationKeyVersion_ != null;

		[DebuggerNonUserCode]
		public string VerificationKeyId
		{
			get
			{
				return verificationKeyId_ ?? VerificationKeyIdDefaultValue;
			}
			set
			{
				verificationKeyId_ = ProtoPreconditions.CheckNotNull<string>(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool HasVerificationKeyId => verificationKeyId_ != null;

		[DebuggerNonUserCode]
		public string SignatureAlgorithm
		{
			get
			{
				return signatureAlgorithm_ ?? SignatureAlgorithmDefaultValue;
			}
			set
			{
				signatureAlgorithm_ = ProtoPreconditions.CheckNotNull<string>(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool HasSignatureAlgorithm => signatureAlgorithm_ != null;

		[DebuggerNonUserCode]
		public SignatureInfo()
		{
		}

		[DebuggerNonUserCode]
		public SignatureInfo(SignatureInfo other)
			: this()
		{
			appBundleId_ = other.appBundleId_;
			androidPackage_ = other.androidPackage_;
			verificationKeyVersion_ = other.verificationKeyVersion_;
			verificationKeyId_ = other.verificationKeyId_;
			signatureAlgorithm_ = other.signatureAlgorithm_;
			_unknownFields = UnknownFieldSet.Clone(other._unknownFields);
		}

		[DebuggerNonUserCode]
		public SignatureInfo Clone()
		{
			return new SignatureInfo(this);
		}

		[DebuggerNonUserCode]
		public void ClearAppBundleId()
		{
			appBundleId_ = null;
		}

		[DebuggerNonUserCode]
		public void ClearAndroidPackage()
		{
			androidPackage_ = null;
		}

		[DebuggerNonUserCode]
		public void ClearVerificationKeyVersion()
		{
			verificationKeyVersion_ = null;
		}

		[DebuggerNonUserCode]
		public void ClearVerificationKeyId()
		{
			verificationKeyId_ = null;
		}

		[DebuggerNonUserCode]
		public void ClearSignatureAlgorithm()
		{
			signatureAlgorithm_ = null;
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as SignatureInfo);
		}

		[DebuggerNonUserCode]
		public bool Equals(SignatureInfo other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (AppBundleId != other.AppBundleId)
			{
				return false;
			}
			if (AndroidPackage != other.AndroidPackage)
			{
				return false;
			}
			if (VerificationKeyVersion != other.VerificationKeyVersion)
			{
				return false;
			}
			if (VerificationKeyId != other.VerificationKeyId)
			{
				return false;
			}
			if (SignatureAlgorithm != other.SignatureAlgorithm)
			{
				return false;
			}
			return object.Equals(_unknownFields, other._unknownFields);
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (HasAppBundleId)
			{
				num ^= AppBundleId.GetHashCode();
			}
			if (HasAndroidPackage)
			{
				num ^= AndroidPackage.GetHashCode();
			}
			if (HasVerificationKeyVersion)
			{
				num ^= VerificationKeyVersion.GetHashCode();
			}
			if (HasVerificationKeyId)
			{
				num ^= VerificationKeyId.GetHashCode();
			}
			if (HasSignatureAlgorithm)
			{
				num ^= SignatureAlgorithm.GetHashCode();
			}
			if (_unknownFields != null)
			{
				num ^= ((object)_unknownFields).GetHashCode();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString((IMessage)(object)this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (HasAppBundleId)
			{
				output.WriteRawTag((byte)10);
				output.WriteString(AppBundleId);
			}
			if (HasAndroidPackage)
			{
				output.WriteRawTag((byte)18);
				output.WriteString(AndroidPackage);
			}
			if (HasVerificationKeyVersion)
			{
				output.WriteRawTag((byte)26);
				output.WriteString(VerificationKeyVersion);
			}
			if (HasVerificationKeyId)
			{
				output.WriteRawTag((byte)34);
				output.WriteString(VerificationKeyId);
			}
			if (HasSignatureAlgorithm)
			{
				output.WriteRawTag((byte)42);
				output.WriteString(SignatureAlgorithm);
			}
			if (_unknownFields != null)
			{
				_unknownFields.WriteTo(output);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (HasAppBundleId)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(AppBundleId);
			}
			if (HasAndroidPackage)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(AndroidPackage);
			}
			if (HasVerificationKeyVersion)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(VerificationKeyVersion);
			}
			if (HasVerificationKeyId)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(VerificationKeyId);
			}
			if (HasSignatureAlgorithm)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(SignatureAlgorithm);
			}
			if (_unknownFields != null)
			{
				num += _unknownFields.CalculateSize();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(SignatureInfo other)
		{
			if (other != null)
			{
				if (other.HasAppBundleId)
				{
					AppBundleId = other.AppBundleId;
				}
				if (other.HasAndroidPackage)
				{
					AndroidPackage = other.AndroidPackage;
				}
				if (other.HasVerificationKeyVersion)
				{
					VerificationKeyVersion = other.VerificationKeyVersion;
				}
				if (other.HasVerificationKeyId)
				{
					VerificationKeyId = other.VerificationKeyId;
				}
				if (other.HasSignatureAlgorithm)
				{
					SignatureAlgorithm = other.SignatureAlgorithm;
				}
				_unknownFields = UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				switch (num)
				{
				default:
					_unknownFields = UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
					break;
				case 10u:
					AppBundleId = input.ReadString();
					break;
				case 18u:
					AndroidPackage = input.ReadString();
					break;
				case 26u:
					VerificationKeyVersion = input.ReadString();
					break;
				case 34u:
					VerificationKeyId = input.ReadString();
					break;
				case 42u:
					SignatureAlgorithm = input.ReadString();
					break;
				}
			}
		}
	}
	public sealed class TemporaryExposureKey : IMessage<TemporaryExposureKey>, IMessage, IEquatable<TemporaryExposureKey>, IDeepCloneable<TemporaryExposureKey>
	{
		private static readonly MessageParser<TemporaryExposureKey> _parser = new MessageParser<TemporaryExposureKey>((Func<TemporaryExposureKey>)(() => new TemporaryExposureKey()));

		private UnknownFieldSet _unknownFields;

		private int _hasBits0;

		public const int KeyDataFieldNumber = 1;

		private static readonly ByteString KeyDataDefaultValue = ByteString.get_Empty();

		private ByteString keyData_;

		public const int TransmissionRiskLevelFieldNumber = 2;

		private static readonly int TransmissionRiskLevelDefaultValue = 0;

		private int transmissionRiskLevel_;

		public const int RollingStartIntervalNumberFieldNumber = 3;

		private static readonly int RollingStartIntervalNumberDefaultValue = 0;

		private int rollingStartIntervalNumber_;

		public const int RollingPeriodFieldNumber = 4;

		private static readonly int RollingPeriodDefaultValue = 144;

		private int rollingPeriod_;

		[DebuggerNonUserCode]
		public static MessageParser<TemporaryExposureKey> Parser => _parser;

		[DebuggerNonUserCode]
		public static MessageDescriptor Descriptor => TemporaryExposureKeyBatchReflection.Descriptor.get_MessageTypes()[2];

		[DebuggerNonUserCode]
		MessageDescriptor Descriptor => Descriptor;

		[DebuggerNonUserCode]
		public ByteString KeyData
		{
			get
			{
				return keyData_ ?? KeyDataDefaultValue;
			}
			set
			{
				keyData_ = ProtoPreconditions.CheckNotNull<ByteString>(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool HasKeyData => keyData_ != (ByteString)null;

		[DebuggerNonUserCode]
		public int TransmissionRiskLevel
		{
			get
			{
				if (((uint)_hasBits0 & (true ? 1u : 0u)) != 0)
				{
					return transmissionRiskLevel_;
				}
				return TransmissionRiskLevelDefaultValue;
			}
			set
			{
				_hasBits0 |= 1;
				transmissionRiskLevel_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool HasTransmissionRiskLevel => (_hasBits0 & 1) != 0;

		[DebuggerNonUserCode]
		public int RollingStartIntervalNumber
		{
			get
			{
				if (((uint)_hasBits0 & 2u) != 0)
				{
					return rollingStartIntervalNumber_;
				}
				return RollingStartIntervalNumberDefaultValue;
			}
			set
			{
				_hasBits0 |= 2;
				rollingStartIntervalNumber_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool HasRollingStartIntervalNumber => (_hasBits0 & 2) != 0;

		[DebuggerNonUserCode]
		public int RollingPeriod
		{
			get
			{
				if (((uint)_hasBits0 & 4u) != 0)
				{
					return rollingPeriod_;
				}
				return RollingPeriodDefaultValue;
			}
			set
			{
				_hasBits0 |= 4;
				rollingPeriod_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool HasRollingPeriod => (_hasBits0 & 4) != 0;

		[DebuggerNonUserCode]
		public TemporaryExposureKey()
		{
		}

		[DebuggerNonUserCode]
		public TemporaryExposureKey(TemporaryExposureKey other)
			: this()
		{
			_hasBits0 = other._hasBits0;
			keyData_ = other.keyData_;
			transmissionRiskLevel_ = other.transmissionRiskLevel_;
			rollingStartIntervalNumber_ = other.rollingStartIntervalNumber_;
			rollingPeriod_ = other.rollingPeriod_;
			_unknownFields = UnknownFieldSet.Clone(other._unknownFields);
		}

		[DebuggerNonUserCode]
		public TemporaryExposureKey Clone()
		{
			return new TemporaryExposureKey(this);
		}

		[DebuggerNonUserCode]
		public void ClearKeyData()
		{
			keyData_ = null;
		}

		[DebuggerNonUserCode]
		public void ClearTransmissionRiskLevel()
		{
			_hasBits0 &= -2;
		}

		[DebuggerNonUserCode]
		public void ClearRollingStartIntervalNumber()
		{
			_hasBits0 &= -3;
		}

		[DebuggerNonUserCode]
		public void ClearRollingPeriod()
		{
			_hasBits0 &= -5;
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as TemporaryExposureKey);
		}

		[DebuggerNonUserCode]
		public bool Equals(TemporaryExposureKey other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (KeyData != other.KeyData)
			{
				return false;
			}
			if (TransmissionRiskLevel != other.TransmissionRiskLevel)
			{
				return false;
			}
			if (RollingStartIntervalNumber != other.RollingStartIntervalNumber)
			{
				return false;
			}
			if (RollingPeriod != other.RollingPeriod)
			{
				return false;
			}
			return object.Equals(_unknownFields, other._unknownFields);
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (HasKeyData)
			{
				num ^= ((object)KeyData).GetHashCode();
			}
			if (HasTransmissionRiskLevel)
			{
				num ^= TransmissionRiskLevel.GetHashCode();
			}
			if (HasRollingStartIntervalNumber)
			{
				num ^= RollingStartIntervalNumber.GetHashCode();
			}
			if (HasRollingPeriod)
			{
				num ^= RollingPeriod.GetHashCode();
			}
			if (_unknownFields != null)
			{
				num ^= ((object)_unknownFields).GetHashCode();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString((IMessage)(object)this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (HasKeyData)
			{
				output.WriteRawTag((byte)10);
				output.WriteBytes(KeyData);
			}
			if (HasTransmissionRiskLevel)
			{
				output.WriteRawTag((byte)16);
				output.WriteInt32(TransmissionRiskLevel);
			}
			if (HasRollingStartIntervalNumber)
			{
				output.WriteRawTag((byte)24);
				output.WriteInt32(RollingStartIntervalNumber);
			}
			if (HasRollingPeriod)
			{
				output.WriteRawTag((byte)32);
				output.WriteInt32(RollingPeriod);
			}
			if (_unknownFields != null)
			{
				_unknownFields.WriteTo(output);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (HasKeyData)
			{
				num += 1 + CodedOutputStream.ComputeBytesSize(KeyData);
			}
			if (HasTransmissionRiskLevel)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(TransmissionRiskLevel);
			}
			if (HasRollingStartIntervalNumber)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(RollingStartIntervalNumber);
			}
			if (HasRollingPeriod)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(RollingPeriod);
			}
			if (_unknownFields != null)
			{
				num += _unknownFields.CalculateSize();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(TemporaryExposureKey other)
		{
			if (other != null)
			{
				if (other.HasKeyData)
				{
					KeyData = other.KeyData;
				}
				if (other.HasTransmissionRiskLevel)
				{
					TransmissionRiskLevel = other.TransmissionRiskLevel;
				}
				if (other.HasRollingStartIntervalNumber)
				{
					RollingStartIntervalNumber = other.RollingStartIntervalNumber;
				}
				if (other.HasRollingPeriod)
				{
					RollingPeriod = other.RollingPeriod;
				}
				_unknownFields = UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				switch (num)
				{
				default:
					_unknownFields = UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
					break;
				case 10u:
					KeyData = input.ReadBytes();
					break;
				case 16u:
					TransmissionRiskLevel = input.ReadInt32();
					break;
				case 24u:
					RollingStartIntervalNumber = input.ReadInt32();
					break;
				case 32u:
					RollingPeriod = input.ReadInt32();
					break;
				}
			}
		}
	}
	public sealed class TEKSignatureList : IMessage<TEKSignatureList>, IMessage, IEquatable<TEKSignatureList>, IDeepCloneable<TEKSignatureList>
	{
		private static readonly MessageParser<TEKSignatureList> _parser = new MessageParser<TEKSignatureList>((Func<TEKSignatureList>)(() => new TEKSignatureList()));

		private UnknownFieldSet _unknownFields;

		public const int SignaturesFieldNumber = 1;

		private static readonly FieldCodec<TEKSignature> _repeated_signatures_codec = FieldCodec.ForMessage<TEKSignature>(10u, TEKSignature.Parser);

		private readonly RepeatedField<TEKSignature> signatures_ = new RepeatedField<TEKSignature>();

		[DebuggerNonUserCode]
		public static MessageParser<TEKSignatureList> Parser => _parser;

		[DebuggerNonUserCode]
		public static MessageDescriptor Descriptor => TemporaryExposureKeyBatchReflection.Descriptor.get_MessageTypes()[3];

		[DebuggerNonUserCode]
		MessageDescriptor Descriptor => Descriptor;

		[DebuggerNonUserCode]
		public RepeatedField<TEKSignature> Signatures => signatures_;

		[DebuggerNonUserCode]
		public TEKSignatureList()
		{
		}

		[DebuggerNonUserCode]
		public TEKSignatureList(TEKSignatureList other)
			: this()
		{
			signatures_ = other.signatures_.Clone();
			_unknownFields = UnknownFieldSet.Clone(other._unknownFields);
		}

		[DebuggerNonUserCode]
		public TEKSignatureList Clone()
		{
			return new TEKSignatureList(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as TEKSignatureList);
		}

		[DebuggerNonUserCode]
		public bool Equals(TEKSignatureList other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!signatures_.Equals(other.signatures_))
			{
				return false;
			}
			return object.Equals(_unknownFields, other._unknownFields);
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			num ^= ((object)signatures_).GetHashCode();
			if (_unknownFields != null)
			{
				num ^= ((object)_unknownFields).GetHashCode();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString((IMessage)(object)this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			signatures_.WriteTo(output, _repeated_signatures_codec);
			if (_unknownFields != null)
			{
				_unknownFields.WriteTo(output);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			num += signatures_.CalculateSize(_repeated_signatures_codec);
			if (_unknownFields != null)
			{
				num += _unknownFields.CalculateSize();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(TEKSignatureList other)
		{
			if (other != null)
			{
				signatures_.Add((IEnumerable<TEKSignature>)other.signatures_);
				_unknownFields = UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				if (num != 10)
				{
					_unknownFields = UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
				}
				else
				{
					signatures_.AddEntriesFrom(input, _repeated_signatures_codec);
				}
			}
		}
	}
	public sealed class TEKSignature : IMessage<TEKSignature>, IMessage, IEquatable<TEKSignature>, IDeepCloneable<TEKSignature>
	{
		private static readonly MessageParser<TEKSignature> _parser = new MessageParser<TEKSignature>((Func<TEKSignature>)(() => new TEKSignature()));

		private UnknownFieldSet _unknownFields;

		private int _hasBits0;

		public const int SignatureInfoFieldNumber = 1;

		private SignatureInfo signatureInfo_;

		public const int BatchNumFieldNumber = 2;

		private static readonly int BatchNumDefaultValue = 0;

		private int batchNum_;

		public const int BatchSizeFieldNumber = 3;

		private static readonly int BatchSizeDefaultValue = 0;

		private int batchSize_;

		public const int SignatureFieldNumber = 4;

		private static readonly ByteString SignatureDefaultValue = ByteString.get_Empty();

		private ByteString signature_;

		[DebuggerNonUserCode]
		public static MessageParser<TEKSignature> Parser => _parser;

		[DebuggerNonUserCode]
		public static MessageDescriptor Descriptor => TemporaryExposureKeyBatchReflection.Descriptor.get_MessageTypes()[4];

		[DebuggerNonUserCode]
		MessageDescriptor Descriptor => Descriptor;

		[DebuggerNonUserCode]
		public SignatureInfo SignatureInfo
		{
			get
			{
				return signatureInfo_;
			}
			set
			{
				signatureInfo_ = value;
			}
		}

		[DebuggerNonUserCode]
		public int BatchNum
		{
			get
			{
				if (((uint)_hasBits0 & (true ? 1u : 0u)) != 0)
				{
					return batchNum_;
				}
				return BatchNumDefaultValue;
			}
			set
			{
				_hasBits0 |= 1;
				batchNum_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool HasBatchNum => (_hasBits0 & 1) != 0;

		[DebuggerNonUserCode]
		public int BatchSize
		{
			get
			{
				if (((uint)_hasBits0 & 2u) != 0)
				{
					return batchSize_;
				}
				return BatchSizeDefaultValue;
			}
			set
			{
				_hasBits0 |= 2;
				batchSize_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool HasBatchSize => (_hasBits0 & 2) != 0;

		[DebuggerNonUserCode]
		public ByteString Signature
		{
			get
			{
				return signature_ ?? SignatureDefaultValue;
			}
			set
			{
				signature_ = ProtoPreconditions.CheckNotNull<ByteString>(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool HasSignature => signature_ != (ByteString)null;

		[DebuggerNonUserCode]
		public TEKSignature()
		{
		}

		[DebuggerNonUserCode]
		public TEKSignature(TEKSignature other)
			: this()
		{
			_hasBits0 = other._hasBits0;
			signatureInfo_ = ((other.signatureInfo_ != null) ? other.signatureInfo_.Clone() : null);
			batchNum_ = other.batchNum_;
			batchSize_ = other.batchSize_;
			signature_ = other.signature_;
			_unknownFields = UnknownFieldSet.Clone(other._unknownFields);
		}

		[DebuggerNonUserCode]
		public TEKSignature Clone()
		{
			return new TEKSignature(this);
		}

		[DebuggerNonUserCode]
		public void ClearBatchNum()
		{
			_hasBits0 &= -2;
		}

		[DebuggerNonUserCode]
		public void ClearBatchSize()
		{
			_hasBits0 &= -3;
		}

		[DebuggerNonUserCode]
		public void ClearSignature()
		{
			signature_ = null;
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as TEKSignature);
		}

		[DebuggerNonUserCode]
		public bool Equals(TEKSignature other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!object.Equals(SignatureInfo, other.SignatureInfo))
			{
				return false;
			}
			if (BatchNum != other.BatchNum)
			{
				return false;
			}
			if (BatchSize != other.BatchSize)
			{
				return false;
			}
			if (Signature != other.Signature)
			{
				return false;
			}
			return object.Equals(_unknownFields, other._unknownFields);
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (signatureInfo_ != null)
			{
				num ^= SignatureInfo.GetHashCode();
			}
			if (HasBatchNum)
			{
				num ^= BatchNum.GetHashCode();
			}
			if (HasBatchSize)
			{
				num ^= BatchSize.GetHashCode();
			}
			if (HasSignature)
			{
				num ^= ((object)Signature).GetHashCode();
			}
			if (_unknownFields != null)
			{
				num ^= ((object)_unknownFields).GetHashCode();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString((IMessage)(object)this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (signatureInfo_ != null)
			{
				output.WriteRawTag((byte)10);
				output.WriteMessage((IMessage)(object)SignatureInfo);
			}
			if (HasBatchNum)
			{
				output.WriteRawTag((byte)16);
				output.WriteInt32(BatchNum);
			}
			if (HasBatchSize)
			{
				output.WriteRawTag((byte)24);
				output.WriteInt32(BatchSize);
			}
			if (HasSignature)
			{
				output.WriteRawTag((byte)34);
				output.WriteBytes(Signature);
			}
			if (_unknownFields != null)
			{
				_unknownFields.WriteTo(output);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (signatureInfo_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize((IMessage)(object)SignatureInfo);
			}
			if (HasBatchNum)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(BatchNum);
			}
			if (HasBatchSize)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(BatchSize);
			}
			if (HasSignature)
			{
				num += 1 + CodedOutputStream.ComputeBytesSize(Signature);
			}
			if (_unknownFields != null)
			{
				num += _unknownFields.CalculateSize();
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(TEKSignature other)
		{
			if (other == null)
			{
				return;
			}
			if (other.signatureInfo_ != null)
			{
				if (signatureInfo_ == null)
				{
					SignatureInfo = new SignatureInfo();
				}
				SignatureInfo.MergeFrom(other.SignatureInfo);
			}
			if (other.HasBatchNum)
			{
				BatchNum = other.BatchNum;
			}
			if (other.HasBatchSize)
			{
				BatchSize = other.BatchSize;
			}
			if (other.HasSignature)
			{
				Signature = other.Signature;
			}
			_unknownFields = UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				switch (num)
				{
				default:
					_unknownFields = UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
					break;
				case 10u:
					if (signatureInfo_ == null)
					{
						SignatureInfo = new SignatureInfo();
					}
					input.ReadMessage((IMessage)(object)SignatureInfo);
					break;
				case 16u:
					BatchNum = input.ReadInt32();
					break;
				case 24u:
					BatchSize = input.ReadInt32();
					break;
				case 34u:
					Signature = input.ReadBytes();
					break;
				}
			}
		}
	}
}
namespace NDB.Covid19.ExposureNotifications.Helpers.FetchExposureKeys
{
	public class PullRules
	{
		private IDeveloperToolsService _developerTools => ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>();

		private IPreferences _preferences => ServiceLocator.get_Current().GetInstance<IPreferences>();

		public bool ShouldAbortPull()
		{
			string logPrefix = "PullRules.ShouldAbortPull: ";
			if (!LocalPreferencesHelper.GetIsDownloadWithMobileDataEnabled() && !ConnectivityHelper.ConnectionProfiles.Contains((ConnectionProfile)4))
			{
				return PrepareAbortMessage(logPrefix, "Pull aborted. Mobile connectivity has been disabled in general settings and WiFi connection is not available. ", $"Last pull: {LocalPreferencesHelper.GetLastPullKeysSucceededDateTime()}");
			}
			if (LastDownloadZipsTooRecent())
			{
				return PrepareAbortMessage(logPrefix, "Pull aborted. The last time we ran DownloadZips was too recent. ", $"Last pull: {LocalPreferencesHelper.GetLastPullKeysSucceededDateTime()}");
			}
			return false;
		}

		private bool PrepareAbortMessage(string logPrefix, string msg, string additionalData = null)
		{
			string text = logPrefix + msg;
			if (additionalData != null)
			{
				text += additionalData;
			}
			_developerTools.AddToPullHistoryRecord(text);
			LogUtils.LogMessage(LogSeverity.WARNING, text);
			return true;
		}

		public bool LastDownloadZipsTooRecent()
		{
			DateTime dateTime = LocalPreferencesHelper.GetLastPullKeysSucceededDateTime();
			if (dateTime.Equals(DateTime.MinValue))
			{
				dateTime = DateTime.UtcNow.AddDays(-14.0).Date;
			}
			TimeSpan t = DateTime.UtcNow - dateTime;
			if (dateTime > DateTime.UtcNow)
			{
				return false;
			}
			return t < Conf.FETCH_MIN_HOURS_BETWEEN_PULL;
		}
	}
}
namespace NDB.Covid19.ExposureNotifications.Helpers.ExposureDetected
{
	public class JsonCompatibleExposureDetectionSummary
	{
		public int DaysSinceLastExposure
		{
			get;
			set;
		}

		public ulong MatchedKeyCount
		{
			get;
			set;
		}

		public int HighestRiskScore
		{
			get;
			set;
		}

		public TimeSpan[] AttenuationDurations
		{
			get;
			set;
		}

		public int SummationRiskScore
		{
			get;
			set;
		}

		public JsonCompatibleExposureDetectionSummary()
		{
		}

		public JsonCompatibleExposureDetectionSummary(ExposureDetectionSummary exposureDetectionSummary)
		{
			DaysSinceLastExposure = exposureDetectionSummary.get_DaysSinceLastExposure();
			MatchedKeyCount = exposureDetectionSummary.get_MatchedKeyCount();
			HighestRiskScore = exposureDetectionSummary.get_HighestRiskScore();
			AttenuationDurations = exposureDetectionSummary.get_AttenuationDurations();
			SummationRiskScore = exposureDetectionSummary.get_SummationRiskScore();
		}
	}
	public class JsonCompatibleExposureInfo
	{
		public DateTime Timestamp
		{
			get;
			set;
		}

		public TimeSpan Duration
		{
			get;
			set;
		}

		public int AttenuationValue
		{
			get;
			set;
		}

		public int TotalRiskScore
		{
			get;
			set;
		}

		public RiskLevel TransmissionRiskLevel
		{
			get;
			set;
		}

		public JsonCompatibleExposureInfo()
		{
		}

		public JsonCompatibleExposureInfo(ExposureInfo exposureInfo)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			Timestamp = exposureInfo.get_Timestamp();
			Duration = exposureInfo.get_Duration();
			AttenuationValue = exposureInfo.get_AttenuationValue();
			TotalRiskScore = exposureInfo.get_TotalRiskScore();
			TransmissionRiskLevel = exposureInfo.get_TransmissionRiskLevel();
		}
	}
}
namespace NDB.Covid19.ExposureNotification
{
	public class ExposureNotificationHandler : IExposureNotificationHandler
	{
		private ExposureNotificationWebService exposureNotificationWebService = new ExposureNotificationWebService();

		private DateTime? MiBaDate => AuthenticationState.PersonalData?.FinalMiBaDate;

		public string UserExplanation => "Saving ExposureInfos with \"Pull Keys and Save ExposureInfos\" causes the EN API to display this notification (not a bug)";

		public Task<Configuration> GetConfigurationAsync()
		{
			return Task.Run<Configuration>(async delegate
			{
				Configuration obj = (await exposureNotificationWebService.GetExposureConfiguration()) ?? throw new FailedToFetchConfigurationException("Aborting pull because configuration was not fetched from server. See corresponding server error log");
				string str = JsonConvert.SerializeObject((object)obj);
				ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>().LastUsedConfiguration = "Time used (UTC): " + DateTime.UtcNow.ToGreGorianUtcString("yyyy-MM-dd HH:mm:ss") + "\n" + str;
				return obj;
			});
		}

		public async Task FetchExposureKeyBatchFilesFromServerAsync(Func<IEnumerable<string>, Task> submitBatches, CancellationToken cancellationToken)
		{
			await new FetchExposureKeysHelper().FetchExposureKeyBatchFilesFromServerAsync(submitBatches, cancellationToken);
		}

		public async Task ExposureDetectedAsync(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getExposureInfo)
		{
			await ExposureDetectedHelper.EvaluateRiskInSummaryAndCreateMessage(summary, this);
			await ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>().SaveLastExposureInfos(getExposureInfo);
			ExposureDetectedHelper.SaveLastSummary(summary);
		}

		public async Task UploadSelfExposureKeysToServerAsync(IEnumerable<TemporaryExposureKey> tempKeys)
		{
			IEnumerable<ExposureKeyModel> temporaryExposureKeys = tempKeys?.Select((TemporaryExposureKey key) => new ExposureKeyModel(key)) ?? new List<ExposureKeyModel>();
			try
			{
				if (ServiceLocator.get_Current().GetInstance<IDeviceInfo>().Platform == DevicePlatform.get_iOS())
				{
					if (!(await ExposureNotification.IsEnabledAsync()))
					{
						await ExposureNotification.StartAsync();
						await ExposureNotification.StopAsync();
					}
					else
					{
						await ExposureNotification.StopAsync();
						await ExposureNotification.StartAsync();
					}
				}
			}
			catch (Exception ex)
			{
				if (!ex.HandleExposureNotificationException("ExposureNotificationHandler", "UploadSelfExposureKeysToServerAsync"))
				{
					throw ex;
				}
			}
			if (FakeGatewayUtils.IsFakeGatewayTest)
			{
				FakeGatewayUtils.LastPulledExposureKeys = temporaryExposureKeys;
				return;
			}
			if (AuthenticationState.PersonalData?.Access_token == null)
			{
				throw new AccessTokenMissingFromNemIDException("The token from NemID is not set");
			}
			if (AuthenticationState.PersonalData?.VisitedCountries == null)
			{
				throw new VisitedCountriesMissingException("The visited countries list is missing. Possibly garbage collection removed it.");
			}
			if (!MiBaDate.HasValue)
			{
				throw new MiBaDateMissingException("The symptom onset date is not set from the calling view model");
			}
			DateTime symptomsDate = MiBaDate.Value.ToUniversalTime();
			List<ExposureKeyModel> keys = UploadDiagnosisKeysHelper.CreateAValidListOfTemporaryExposureKeys(temporaryExposureKeys);
			keys = UploadDiagnosisKeysHelper.SetTransmissionRiskLevel(keys, symptomsDate);
			if (await exposureNotificationWebService.PostSelvExposureKeys(keys))
			{
				return;
			}
			throw new FailedToPushToServerException("Failed to push keys to the server");
		}
	}
}
namespace NDB.Covid19.ExposureNotification.Helpers
{
	public abstract class BatchFileHelper
	{
		public static IEnumerable<string> SaveZipStreamToBinAndSig(Stream zipStream)
		{
			ZipArchive zipArchive = new ZipArchive(zipStream);
			ZipArchiveEntry entry = zipArchive.GetEntry("export.bin");
			ZipArchiveEntry entry2 = zipArchive.GetEntry("export.sig");
			Stream stream = entry.Open();
			Stream stream2 = entry2.Open();
			string text = Path.Combine(ServiceLocator.get_Current().GetInstance<IFileSystem>().CacheDirectory, Guid.NewGuid().ToString() + ".bin");
			string text2 = Path.Combine(ServiceLocator.get_Current().GetInstance<IFileSystem>().CacheDirectory, Guid.NewGuid().ToString() + ".sig");
			FileStream fileStream = File.Create(text);
			FileStream fileStream2 = File.Create(text2);
			stream.CopyTo(fileStream);
			stream2.CopyTo(fileStream2);
			fileStream.Close();
			fileStream2.Close();
			return new List<string>
			{
				text,
				text2
			};
		}

		public static ZipArchive UrlToZipArchive(string localFileUrl)
		{
			return new ZipArchive(new FileStream(localFileUrl, FileMode.Open, FileAccess.Read));
		}

		public static TemporaryExposureKeyExport ZipToTemporaryExposureKeyExport(ZipArchive zipArchive)
		{
			IEnumerable<byte> source = ReadToEnd(zipArchive.GetEntry("export.bin").Open()).Skip(16);
			return TemporaryExposureKeyExport.Parser.ParseFrom(source.ToArray());
		}

		private static byte[] ReadToEnd(Stream stream)
		{
			long position = 0L;
			if (stream.CanSeek)
			{
				position = stream.Position;
				stream.Position = 0L;
			}
			try
			{
				byte[] array = new byte[4096];
				int num = 0;
				int num2;
				while ((num2 = stream.Read(array, num, array.Length - num)) > 0)
				{
					num += num2;
					if (num == array.Length)
					{
						int num3 = stream.ReadByte();
						if (num3 != -1)
						{
							byte[] array2 = new byte[array.Length * 2];
							Buffer.BlockCopy(array, 0, array2, 0, array.Length);
							Buffer.SetByte(array2, num, (byte)num3);
							array = array2;
							num++;
						}
					}
				}
				byte[] array3 = array;
				if (array.Length != num)
				{
					array3 = new byte[num];
					Buffer.BlockCopy(array, 0, array3, 0, num);
				}
				return array3;
			}
			finally
			{
				if (stream.CanSeek)
				{
					stream.Position = position;
				}
			}
		}
	}
	public static class SystemTime
	{
		public static Func<DateTime> Now = () => DateTime.UtcNow;

		public static void SetDateTime(DateTime dateTimeNow)
		{
			Now = dateTimeNow.ToUniversalTime;
		}

		public static void ResetDateTime()
		{
			Now = () => DateTime.UtcNow;
		}
	}
	public abstract class UploadDiagnosisKeysHelper
	{
		public static List<ExposureKeyModel> CreateAValidListOfTemporaryExposureKeys(IEnumerable<ExposureKeyModel> temporaryExposureKeys)
		{
			List<ExposureKeyModel> list = temporaryExposureKeys.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				ExposureKeyModel exposureKeyModel = list[i];
				for (int num = list.Count - 1; num > i; num--)
				{
					ExposureKeyModel exposureKeyModel2 = list[num];
					if (((TemporaryExposureKey)exposureKeyModel).get_RollingStart() == ((TemporaryExposureKey)exposureKeyModel2).get_RollingStart())
					{
						list.RemoveAt(num);
					}
				}
			}
			list.Sort((ExposureKeyModel x, ExposureKeyModel y) => ((TemporaryExposureKey)y).get_RollingStart().CompareTo(((TemporaryExposureKey)x).get_RollingStart()));
			for (int j = 0; j < list.Count - 1; j++)
			{
				if (((TemporaryExposureKey)list[j + 1]).get_RollingStart() != ((TemporaryExposureKey)list[j]).get_RollingStart().AddDays(-1.0))
				{
					list = list.Take(j + 1).ToList();
					break;
				}
			}
			if (list.Count > 14)
			{
				list = list.Take(14).ToList();
			}
			foreach (ExposureKeyModel item in list)
			{
				((TemporaryExposureKey)item).set_RollingDuration(new TimeSpan(1, 0, 0, 0));
			}
			return list;
		}

		public static List<ExposureKeyModel> SetTransmissionRiskLevel(List<ExposureKeyModel> keys, DateTime symptomsDate)
		{
			DateTimeOffset right = new DateTimeOffset(symptomsDate);
			foreach (ExposureKeyModel key in keys)
			{
				int num = (key.DaysSinceOnsetOfSymptoms = (((TemporaryExposureKey)key).get_RollingStart() - right).Days);
				for (int num2 = Conf.DAYS_SINCE_ONSET_FOR_TRANSMISSION_RISK_CALCULATION.Length - 1; num2 >= 0; num2--)
				{
					if (num >= Conf.DAYS_SINCE_ONSET_FOR_TRANSMISSION_RISK_CALCULATION[num2].Item1 && num <= Conf.DAYS_SINCE_ONSET_FOR_TRANSMISSION_RISK_CALCULATION[num2].Item2)
					{
						((TemporaryExposureKey)key).set_TransmissionRiskLevel((RiskLevel)(num2 + 1));
						break;
					}
				}
			}
			return keys;
		}
	}
}
namespace NDB.Covid19.ExposureNotification.Helpers.FetchExposureKeys
{
	public class FetchExposureKeysHelper
	{
		private static string _logPrefix = "FetchExposureKeysHelper";

		private PullRules _pullRules = new PullRules();

		private IDeveloperToolsService _developerTools => ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>();

		public async Task FetchExposureKeyBatchFilesFromServerAsync(Func<IEnumerable<string>, Task> submitBatches, CancellationToken cancellationToken)
		{
			_developerTools.StartPullHistoryRecord();
			SendReApproveConsentsNotificationIfNeeded();
			ResendMessageIfNeeded();
			if (!_pullRules.ShouldAbortPull())
			{
				IEnumerable<string> zipsLocation = await new ZipDownloader().DownloadZips(cancellationToken);
				if (await SubmitZips(zipsLocation, submitBatches))
				{
					LocalPreferencesHelper.UpdateLastPullKeysSucceededDateTime();
					_developerTools.AddToPullHistoryRecord("Zips were successfully submitted to EN API.");
				}
				DeleteZips(zipsLocation);
			}
		}

		private async void ResendMessageIfNeeded()
		{
			DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(SystemTime.Now(), TimeZoneInfo.Local);
			DateTime date = SystemTime.Now().Date;
			DateTime dateTimeFromSecureStorageForKey = MessageUtils.GetDateTimeFromSecureStorageForKey(SecureStorageKeys.LAST_SENT_NOTIFICATION_UTC_KEY, "ResendMessageIfNeeded");
			DateTime dateTime2 = dateTimeFromSecureStorageForKey.ToLocalTime();
			if (dateTimeFromSecureStorageForKey < date && dateTime.Date.Subtract(dateTime2.Date).TotalHours >= (double)Conf.HOURS_UNTIL_RESEND_MESSAGES && dateTime.Hour >= Conf.HOUR_WHEN_MESSAGE_SHOULD_BE_RESEND && (await MessageUtils.GetAllUnreadMessages()).FindAll((MessageSQLiteModel message) => SystemTime.Now().Subtract(message.TimeStamp).TotalMinutes < (double)Conf.MAX_MESSAGE_RETENTION_TIME_IN_MINUTES).ToList().Count > 0)
			{
				NotificationsHelper.CreateNotification(NotificationsEnum.NewMessageReceived, 0);
				MessageUtils.SaveDateTimeToSecureStorageForKey(SecureStorageKeys.LAST_SENT_NOTIFICATION_UTC_KEY, SystemTime.Now(), "ResendMessageIfNeeded");
			}
		}

		private void SendReApproveConsentsNotificationIfNeeded()
		{
			if (OnboardingStatusHelper.Status == OnboardingStatus.OnlyMainOnboardingCompleted && !LocalPreferencesHelper.TermsNotificationWasShown)
			{
				NotificationsHelper.CreateNotificationOnlyIfInBackground(NotificationsEnum.ReApproveConsents);
			}
		}

		private async Task<bool> SubmitZips(IEnumerable<string> zips, Func<IEnumerable<string>, Task> submitBatches)
		{
			if (zips == null || !zips.Any())
			{
				return false;
			}
			_developerTools.StoreLastProvidedFiles(zips);
			try
			{
				await submitBatches(zips);
				return true;
			}
			catch (FailedToFetchConfigurationException ex)
			{
				string message = ex.Message;
				LogUtils.LogException(LogSeverity.WARNING, ex, _logPrefix + ".SubmitZips: " + message);
				_developerTools.AddToPullHistoryRecord(message);
				return false;
			}
			catch (Exception e)
			{
				string text = "submitBatches() failed when submitting the files to the EN API";
				LogUtils.LogException(LogSeverity.ERROR, e, _logPrefix + ".SubmitZips: " + text);
				_developerTools.AddToPullHistoryRecord(text);
				return false;
			}
		}

		private void DeleteZips(IEnumerable<string> zips)
		{
			foreach (string zip in zips)
			{
				try
				{
					File.Delete(zip);
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.WARNING, e, _logPrefix + ".DeleteZips: Caught Exception when deleting temporary zip files");
				}
			}
		}
	}
	public class ZipDownloader
	{
		private readonly ExposureNotificationWebService _exposureNotificationWebService = new ExposureNotificationWebService();

		private string _logPrefix = "ZipDownloader";

		public static readonly string MoreBatchesExistHeader = "nextBatchExists";

		public static readonly string LastBatchReturnedHeader = "lastBatchReturned";

		private IDeveloperToolsService _developerTools => ServiceLocator.get_Current().GetInstance<IDeveloperToolsService>();

		private static IPreferences _preferences => ServiceLocator.get_Current().GetInstance<IPreferences>();

		public async Task<IEnumerable<string>> DownloadZips(CancellationToken cancellationToken)
		{
			return await PullNewKeys(_exposureNotificationWebService, cancellationToken);
		}

		public async Task<IEnumerable<string>> PullNewKeys(ExposureNotificationWebService service, CancellationToken cancellationToken)
		{
			PullKeysParams requestParams = PullKeysParams.GenerateParams();
			List<string> zipLocations = new List<string>();
			bool lastPull = false;
			int? lastBatchReceived = null;
			while (!lastPull)
			{
				string requestUrl = requestParams.ToBatchFileRequest();
				ApiResponse<Stream> apiResponse = await service.GetDiagnosisKeys(requestUrl, cancellationToken);
				HttpHeaders headers = apiResponse.Headers;
				bool flag = true;
				if (apiResponse == null || !apiResponse.IsSuccessfull)
				{
					if (apiResponse != null && apiResponse.StatusCode == 410)
					{
						NotificationsHelper.CreateNotification(NotificationsEnum.ApiDeprecated, 0);
						string text = "410 Api was deprecated";
						_developerTools.AddToPullHistoryRecord(text, requestUrl);
						LogUtils.LogMessage(LogSeverity.WARNING, _logPrefix + ".DownloadZips: " + text);
					}
					else
					{
						_developerTools.AddToPullHistoryRecord($"{apiResponse.StatusCode} Server Error", requestUrl);
					}
					break;
				}
				if (apiResponse.StatusCode == 204)
				{
					if (requestParams.Date.Date < SystemTime.Now().Date)
					{
						requestParams.Date = requestParams.Date.AddDays(1.0);
						requestParams.BatchNumber = 1;
						lastPull = false;
					}
					else
					{
						_developerTools.AddToPullHistoryRecord("204 No Content - No new keys", requestUrl);
						string str = "API " + apiResponse.Endpoint + " returned 204 No Content - No new keys since last pull";
						LogUtils.LogMessage(LogSeverity.WARNING, _logPrefix + ".DownloadZips: " + str);
						lastPull = true;
					}
				}
				else
				{
					try
					{
						int value = int.Parse(headers.GetValues(LastBatchReturnedHeader).First());
						bool num = bool.Parse(headers.GetValues(MoreBatchesExistHeader).First());
						lastBatchReceived = value;
						if (num)
						{
							requestParams.BatchNumber = lastBatchReceived.Value + 1;
							lastPull = false;
						}
						else if (requestParams.Date.Date < SystemTime.Now().Date)
						{
							requestParams.Date = requestParams.Date.AddDays(1.0);
							requestParams.BatchNumber = 1;
							lastPull = false;
						}
						else
						{
							lastPull = true;
						}
					}
					catch (Exception e)
					{
						HandleErrorWhenPulling(e, "Failed to parse " + MoreBatchesExistHeader + " or " + LastBatchReturnedHeader + " header.", requestUrl);
						break;
					}
				}
				if (apiResponse.StatusCode == 200 && flag)
				{
					try
					{
						_developerTools.AddToPullHistoryRecord("200 OK", requestUrl);
						string tmpFile = Path.Combine(ServiceLocator.get_Current().GetInstance<IFileSystem>().CacheDirectory, Guid.NewGuid().ToString() + ".zip");
						FileStream tmpFileStream = File.Create(tmpFile);
						await apiResponse.Data.CopyToAsync(tmpFileStream);
						tmpFileStream.Close();
						zipLocations.Add(tmpFile);
					}
					catch (Exception e2)
					{
						HandleErrorWhenPulling(e2, "Failed to save zip locally", requestUrl);
						break;
					}
				}
			}
			if (zipLocations.Any() && lastBatchReceived.HasValue)
			{
				LocalPreferencesHelper.LastPullKeysBatchNumberNotSubmitted = lastBatchReceived.Value;
				LocalPreferencesHelper.LastPulledBatchType = requestParams.BatchType;
			}
			return zipLocations;
		}

		public void HandleErrorWhenPulling(Exception e, string errorMessage, string requestUrl)
		{
			_developerTools.AddToPullHistoryRecord(errorMessage, requestUrl);
			LogUtils.LogException(LogSeverity.ERROR, e, _logPrefix + ".DownloadZips: " + errorMessage);
		}
	}
}
namespace NDB.Covid19.ExposureNotification.Helpers.ExposureDetected
{
	public abstract class ExposureDetectedHelper
	{
		private static string _logPrefix = "ExposureDetectedHelper";

		private static SecureStorageService _secureStorageService => ServiceLocator.get_Current().GetInstance<SecureStorageService>();

		public static async Task EvaluateRiskInSummaryAndCreateMessage(ExposureDetectionSummary summary, object messageSender)
		{
			if (summary.get_MatchedKeyCount() != 0L && summary.get_HighestRiskScore() >= 1 && IsAttenuationDurationOverThreshold(summary))
			{
				await MessageUtils.CreateMessage(messageSender);
			}
		}

		public static bool IsAttenuationDurationOverThreshold(ExposureDetectionSummary summary)
		{
			double exposureTimeThreshold = LocalPreferencesHelper.ExposureTimeThreshold;
			double lowAttenuationDurationMultiplier = LocalPreferencesHelper.LowAttenuationDurationMultiplier;
			double middleAttenuationDurationMultiplier = LocalPreferencesHelper.MiddleAttenuationDurationMultiplier;
			double highAttenuationDurationMultiplier = LocalPreferencesHelper.HighAttenuationDurationMultiplier;
			return (double)summary.get_AttenuationDurations()[0].Minutes * lowAttenuationDurationMultiplier + (double)summary.get_AttenuationDurations()[1].Minutes * middleAttenuationDurationMultiplier + (double)summary.get_AttenuationDurations()[2].Minutes * highAttenuationDurationMultiplier >= exposureTimeThreshold;
		}

		public static void SaveLastSummary(ExposureDetectionSummary summary)
		{
			try
			{
				string value = ExposureDetectionSummaryJsonHelper.ExposureDectionSummaryToJson(summary);
				_secureStorageService.SaveValue(SecureStorageKeys.LAST_SUMMARY_KEY, value);
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, _logPrefix + ".SaveLastSummary");
			}
		}
	}
	public abstract class ExposureDetectionSummaryJsonHelper
	{
		public static string ExposureDectionSummaryToJson(ExposureDetectionSummary exposureDetectionSummary)
		{
			return JsonConvert.SerializeObject((object)new JsonCompatibleExposureDetectionSummary(exposureDetectionSummary));
		}

		public static ExposureDetectionSummary ExposureDetectionSummaryFromJsonCompatibleString(string jsonCompatibleExposureDetectionSummaryJson)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			JsonCompatibleExposureDetectionSummary jsonCompatibleExposureDetectionSummary = JsonConvert.DeserializeObject<JsonCompatibleExposureDetectionSummary>(jsonCompatibleExposureDetectionSummaryJson);
			return new ExposureDetectionSummary(jsonCompatibleExposureDetectionSummary.DaysSinceLastExposure, jsonCompatibleExposureDetectionSummary.MatchedKeyCount, jsonCompatibleExposureDetectionSummary.HighestRiskScore, jsonCompatibleExposureDetectionSummary.AttenuationDurations, jsonCompatibleExposureDetectionSummary.SummationRiskScore);
		}
	}
	public abstract class ExposureInfoJsonHelper
	{
		public static string ExposureInfosToJson(IEnumerable<ExposureInfo> exposureInfos)
		{
			return JsonConvert.SerializeObject((object)exposureInfos.Select((ExposureInfo exposureInfo) => new JsonCompatibleExposureInfo(exposureInfo)));
		}

		public static IEnumerable<ExposureInfo> ExposureInfosFromJsonCompatibleString(string jsonCompatibleExposureInfosJson)
		{
			return JsonConvert.DeserializeObject<IEnumerable<JsonCompatibleExposureInfo>>(jsonCompatibleExposureInfosJson).Select((Func<JsonCompatibleExposureInfo, ExposureInfo>)((JsonCompatibleExposureInfo jsonCompatibleExposureInfo) => new ExposureInfo(jsonCompatibleExposureInfo.Timestamp, jsonCompatibleExposureInfo.Duration, jsonCompatibleExposureInfo.AttenuationValue, jsonCompatibleExposureInfo.TotalRiskScore, jsonCompatibleExposureInfo.TransmissionRiskLevel)));
		}
	}
}
namespace NDB.Covid19.Enums
{
	public enum AppState
	{
		IsAlive,
		IsDestroyed
	}
	public enum AuthErrorType
	{
		Unknown,
		MaxTriesExceeded,
		NotInfected
	}
	public enum ENOperation
	{
		PUSH,
		PULL
	}
	public enum LogSeverity
	{
		INFO,
		WARNING,
		ERROR
	}
	public enum NotificationsEnum
	{
		NewMessageReceived,
		ApiDeprecated,
		ConsentNeeded,
		ReApproveConsents,
		BackgroundFetch
	}
	public static class NotificationsEnumExtensions
	{
		public static NotificationViewModel Data(this NotificationsEnum notificationType)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			return notificationType switch
			{
				NotificationsEnum.NewMessageReceived => new NotificationViewModel
				{
					Type = NotificationsEnum.NewMessageReceived,
					Title = Extensions.Translate("NOTIFICATION_HEADER", Array.Empty<object>()),
					Body = Extensions.Translate("NOTIFICATION_DESCRIPTION", Array.Empty<object>())
				}, 
				NotificationsEnum.ApiDeprecated => new NotificationViewModel
				{
					Type = NotificationsEnum.ApiDeprecated,
					Title = Extensions.Translate("NOTIFICATION_UPDATE_HEADER", Array.Empty<object>()),
					Body = Extensions.Translate((ServiceLocator.get_Current().GetInstance<IDeviceInfo>().Platform == DevicePlatform.get_iOS()) ? "NOTIFICATION_iOS_UPDATE_DESCRIPTION" : "NOTIFICATION_ANDROID_UPDATE_DESCRIPTION", Array.Empty<object>())
				}, 
				NotificationsEnum.ConsentNeeded => new NotificationViewModel
				{
					Type = NotificationsEnum.ConsentNeeded,
					Title = Extensions.Translate("NOTIFICATION_UPDATE_HEADER", Array.Empty<object>()),
					Body = Extensions.Translate("NOTIFICATION_CONSENT_DESCRIPTION", Array.Empty<object>())
				}, 
				NotificationsEnum.ReApproveConsents => new NotificationViewModel
				{
					Type = NotificationsEnum.ReApproveConsents,
					Title = Extensions.Translate("NOTIFICATION_CONSENT_HEADER", Array.Empty<object>()),
					Body = Extensions.Translate("NOTIFICATION_CONSENT_DESCRIPTION", Array.Empty<object>())
				}, 
				NotificationsEnum.BackgroundFetch => new NotificationViewModel
				{
					Type = NotificationsEnum.BackgroundFetch,
					Title = Extensions.Translate("NOTIFICATION_BACKGROUND_FETCH_HEADER", Array.Empty<object>()),
					Body = Extensions.Translate("NOTIFICATION_BACKGROUND_FETCH_DESCRIPTION", Array.Empty<object>())
				}, 
				_ => throw new InvalidEnumArgumentException("Notification type does not exist"), 
			};
		}
	}
	public enum OnboardingStatus
	{
		NoConsentsGiven,
		OnlyMainOnboardingCompleted,
		CountriesOnboardingCompleted
	}
	public enum QuestionaireSelection
	{
		YesSince,
		YesBut,
		No,
		Skip
	}
	public enum SettingItemType
	{
		Intro,
		HowItWorks,
		Consent,
		Settings,
		Help,
		About,
		Messages,
		Debug
	}
	public enum SettingsLanguageSelection
	{
		Danish,
		English
	}
}
namespace NDB.Covid19.Config
{
	public class Conf
	{
		public static readonly string BaseUrl = "https://app.smittestop.dk/API/";

		public static readonly TimeSpan FETCH_MIN_HOURS_BETWEEN_PULL = TimeSpan.FromMinutes(120.0);

		public static readonly int APIVersion = 2;

		public static string DEFAULT_LANGUAGE = "da";

		public static string[] SUPPORTED_LANGUAGES = new string[2]
		{
			"da",
			"en"
		};

		public static int MAX_MESSAGE_RETENTION_TIME_IN_MINUTES = MESSAGE_RETENTION_TIME_IN_MINUTES_LONG;

		public static int HOURS_UNTIL_RESEND_MESSAGES = 48;

		public static int HOUR_WHEN_MESSAGE_SHOULD_BE_RESEND = 21;

		public static readonly TimeSpan BACKGROUND_FETCH_REPEAT_INTERVAL_ANDROID = TimeSpan.FromHours(1.0);

		public static readonly int FETCH_MAX_ATTEMPTS = 1;

		public static readonly Tuple<int, int>[] DAYS_SINCE_ONSET_FOR_TRANSMISSION_RISK_CALCULATION = new Tuple<int, int>[8]
		{
			Tuple.Create(int.MinValue, int.MaxValue),
			Tuple.Create(-3, -3),
			Tuple.Create(-2, -2),
			Tuple.Create(-1, 2),
			Tuple.Create(3, 6),
			Tuple.Create(7, 8),
			Tuple.Create(9, 10),
			Tuple.Create(11, 12)
		};

		public static readonly int RISK_SCORE_THRESHOLD_FOR_HIGH_RISK = 512;

		public static readonly double LOW_ATTENUATION_DURATION_MULTIPLIER = 1.0;

		public static readonly double MIDDLE_ATTENUATION_DURATION_MULTIPLIER = 0.5;

		public static readonly double HIGH_ATTENUATION_DURATION_MULTIPLIER = 0.0;

		public static readonly double EXPOSURE_TIME_THRESHOLD = 15.0;

		public static readonly string[] SUPPORTED_REGIONS = new string[1]
		{
			"dk"
		};

		public static string GooglePlayAppLink = "https://play.google.com/store/apps/details?id=com.netcompany.smittestop_exposure_notification";

		public static string IOSAppstoreAppLink = "itms-apps://itunes.apple.com/app/1516581736";

		public static string AuthorizationHeader => "68iXQyxZOy";

		public static bool UseDeveloperTools => false;

		public static int DEFAULT_TIMEOUT_SERVICECALLS_SECONDS => 40;

		public static int MESSAGE_RETENTION_TIME_IN_MINUTES_SHORT => 15;

		public static int MESSAGE_RETENTION_TIME_IN_MINUTES_LONG => 20160;

		public static string URL_PREFIX => $"{BaseUrl}v{APIVersion}/";

		public static string URL_LOG_MESSAGE => URL_PREFIX + "logging/logMessages";

		public static string URL_PUT_UPLOAD_DIAGNOSIS_KEYS => URL_PREFIX + "diagnostickeys";

		public static string URL_GET_EXPOSURE_CONFIGURATION => URL_PREFIX + "diagnostickeys/exposureconfiguration";

		public static string URL_GET_DIAGNOSIS_KEYS => URL_PREFIX + "diagnostickeys";

		public static string URL_GET_COUNTRY_LIST => URL_PREFIX + "countries";

		public static string URL_GATEWAY_STUB_UPLOAD => URL_PREFIX + "diagnosiskeys/upload";

		public static string DB_NAME => "Smittestop1.db3";
	}
	public static class OAuthConf
	{
		public static string OAUTH2_CLIENT_ID = "smittestop";

		public static string OAUTH2_SCOPE = "openid";

		public static string OAUTH2_REDIRECT_URL = "com.netcompany.smittestop:/oauth2redirect";

		public static string OAUTH2_AUTHORISE_URL = "https://smittestop.sundhedsdatastyrelsen.dk/auth/realms/smittestop/protocol/openid-connect/auth";

		public static string OAUTH2_ACCESSTOKEN_URL = "https://smittestop.sundhedsdatastyrelsen.dk/auth/realms/smittestop/protocol/openid-connect/token";

		public static string OAUTH2_VERIFY_TOKEN_PUBLIC_KEY = "MIICozCCAYsCBgFyUF+KrzANBgkqhkiG9w0BAQsFADAVMRMwEQYDVQQDDApzbWl0dGVzdG9wMB4XDTIwMDUyNjA5NDM1OFoXDTMwMDUyNjA5NDUzOFowFTETMBEGA1UEAwwKc21pdHRlc3RvcDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKfvyy3eqyn9A4s5cuyzNJyJQsrWkyN2aNzwq8s1Gd9WUsFe6RKwKHAMQqRzSVzDS6cgxN9MWbsyLZymxNxtDFvILxOTKvzxJsbDwcD2vAiLu7raRTP0e2WyST8UdSS+ZT69yIKqWXqjwtz/KMgFD9FaWhj/xLf34RiLP6qysEYNGaBnKONoajjvo5+WzXkvX5vkhx2dWajikk0wxbLhKskwr41yw2xa6fsBmhzigZjGkzAoXoLeYGx/EbnpRSoadFatagMemWUXe1Nw8AGYpamqAuvuHRUTAYtyJChZZuXbtm8hygj05oTOfPOlE3E7dL0MGhe91BgyKmEjGeHRNBcCAwEAATANBgkqhkiG9w0BAQsFAAOCAQEANhvGRFjAyTewVu4/gy234d5qGadET5y4OR3uUPyySM2gMhkmrlUyOxWr82UunRLCVs+S0N1AEVt4/sAFn4q5zopyDMHhYUpi9L5X32Q8E1qHRFgwjfMIGq6kiU6qZld3zOvBlu9A4mgsxm23JhCx+kYoDN4FSsoyIiyQX9wxHj9VXA7exlK/nTlk4aPTPPH/ukY1isZmdPXfKUiLocikDReFffqy8gaG081Jqjpp04HmJWX+ryOYI1oKB88ZNkadpadA1z7nHBTF1k+KIhB+0vFCBjGVXeC7KSJQkJck8mfG/JKNFgnKGt4x2PzHGJYOQewJxhDXWXyRGtIwmUzH3w==";
	}
	public class PreferencesKeys
	{
		public static readonly string MIGRATION_COUNT = "MIGRATION_COUNT";

		public static readonly string MESSAGES_LAST_UPDATED_PREF = "MESSAGES_LAST_UPDATED_PREF";

		public static readonly string IS_ONBOARDING_COMPLETED_PREF = "isOnboardingCompleted";

		public static readonly string IS_ONBOARDING_COUNTRIES_COMPLETED_PREF = "isOnboardingCountriesCompleted";

		public static readonly string USE_MOBILE_DATA_PREF = "USE_MOBILE_DATA_PREF";

		public static readonly string LAST_PULL_KEYS_SUCCEEDED_DATE_TIME = "LAST_PULL_KEYS_SUCCEEDED_DATE_TIME";

		public static readonly string LAST_PULLED_BATCH_NUMBER_NOT_SUBMITTED = "LAST_PULLED_BATCH_NUMBER_NOT_SUBMITTED";

		public static readonly string LAST_PULLED_BATCH_NUMBER_SUBMITTED = "LAST_PULLED_BATCH_NUMBER_SUBMITTED";

		public static readonly string LAST_PULLED_BATCH_TYPE = "LAST_PULLED_BATCH_TYPE";

		public static readonly string APP_LANGUAGE = "APP_LANGUAGE";

		public static readonly string DEV_TOOLS_PULL_KEYS_HISTORY = "DEV_TOOLS_PULL_KEYS_HISTORY";

		public static readonly string DEV_TOOLS_PULL_KEYS_HISTORY_LAST_RECORD = "DEV_TOOLS_PULL_KEYS_HISTORY_LAST_RECORD";

		public static readonly string TERMS_NOTIFICATION_WAS_SENT = "TERMS_NOTIFICATION_WAS_SENT";

		public static readonly string LAST_MESSAGE_DATE_TIME = "LAST_MESSAGE_DATE_TIME";

		public static readonly string EXPOSURE_TIME_THRESHOLD = "EXPOSURE_TIME_THRESHOLD";

		public static readonly string LOW_ATTENUATION_DURATION_MULTIPLIER = "LOW_ATTENUATION_DURATION_MULTIPLIER";

		public static readonly string MIDDLE_ATTENUATION_DURATION_MULTIPLIER = "MIDDLE_ATTENUATION_DURATION_MULTIPLIER";

		public static readonly string HIGH_ATTENUATION_DURATION_MULTIPLIER = "HIGH_ATTENUATION_DURATION_MULTIPLIER";

		[Obsolete]
		public static readonly string LAST_DOWNLOAD_ZIPS_CALL_UTC_PREF = "LAST_DOWNLOAD_ZIPS_CALL_UTC_PREF";

		[Obsolete]
		public static readonly string LAST_DOWNLOAD_ZIPS_CALL_UTC_DATETIME_PREF = "LAST_DOWNLOAD_ZIPS_CALL_UTC_DATETIME_PREF";

		[Obsolete]
		public static readonly string CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_DATETIME_PREF = "CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_DATETIME_PREF";

		[Obsolete]
		public static readonly string CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_PREF = "CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_PREF";

		[Obsolete]
		public static readonly string CURRENT_DOWNLOAD_DAY_BATCH_PREF = "CURRENT_DOWNLOAD_DAY_BATCH_PREF";
	}
}
