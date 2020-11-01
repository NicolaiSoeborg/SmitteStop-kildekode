using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using I18NPortable;
using Microsoft.Security.Application;
using NDB.Covid19.Configuration;
using NDB.Covid19.DeviceGuid;
using NDB.Covid19.Enums;
using NDB.Covid19.HardwareServices.SupportServices;
using NDB.Covid19.Models;
using NDB.Covid19.Models.DTOsForServer;
using NDB.Covid19.Models.Logging;
using NDB.Covid19.Models.SQLite;
using NDB.Covid19.PersistedData.SecureStorage;
using NDB.Covid19.PersistedData.SQLite;
using NDB.Covid19.SecureStorage;
using NDB.Covid19.Utils;
using NDB.Covid19.WebServices;
using NDB.Covid19.WebServices.ErrorHandlers;
using NDB.Covid19.WebServices.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Plugin.SecureStorage;
using Plugin.SecureStorage.Abstractions;
using SQLite;
using Unity;
using Unity.Injection;
using Xamarin.Essentials;

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
			unityContainer.RegisterSingleton<DeviceGuidService>(Array.Empty<InjectionMember>());
			unityContainer.RegisterType<ILoggingManager, LoggingSQLiteManager>(Array.Empty<InjectionMember>());
		}
	}
	public static class ExceptionExtensions
	{
		public static bool ExposureNotificationApiNotAvailable(this Exception e)
		{
			return e.ToString().Contains("Android.Gms.Common.Apis.ApiException: 17");
		}
	}
}
namespace NDB.Covid19.WebServices
{
	public class BaseWebService
	{
		private BadConnectionErrorHandler _badConnectionErrorHandler = new BadConnectionErrorHandler();

		public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			DateFormatHandling = DateFormatHandling.IsoDateFormat,
			DateTimeZoneHandling = DateTimeZoneHandling.Local
		};

		public static JsonSerializer JsonSerializer = new JsonSerializer();

		public object Data
		{
			get;
			set;
		}

		private HttpClientManager _httpClientManager
		{
			get
			{
				HttpClientManager instance = HttpClientManager.Instance;
				instance.AddSecretToHeaderIfMissing();
				instance.AddHostHeaderIfMissing();
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
			ApiResponse<M> result = new ApiResponse<M>(url);
			try
			{
				HttpResponseMessage response = await _client.GetAsync(url);
				result.StatusCode = (int)response.StatusCode;
				if (response.IsSuccessStatusCode)
				{
					string text = await response.Content.ReadAsStringAsync();
					if (!string.IsNullOrEmpty(text))
					{
						result.ResponseText = text;
						MapData(result, text);
					}
					result.Headers = response.Content.Headers;
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

		public async Task<ApiResponse<Stream>> GetFileAsStreamAsync(string url)
		{
			ApiResponse<Stream> result = new ApiResponse<Stream>(url);
			try
			{
				HttpResponseMessage response = await _client.GetAsync(url);
				result.StatusCode = (int)response.StatusCode;
				if (response.IsSuccessStatusCode)
				{
					Stream stream = await response.Content.ReadAsStreamAsync();
					result.Headers = response.Content.Headers;
					if (stream.Length > 0)
					{
						result.Data = stream;
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

		public async Task<ApiResponse> Post(string url)
		{
			return await Post<object>(null, url);
		}

		public async Task<ApiResponse> Post<T>(T t, string url)
		{
			string content2 = JsonConvert.SerializeObject(t, JsonSerializerSettings);
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
			ApiResponse result = new ApiResponse(url);
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
			JObject jObject = JObject.Parse(content);
			if (jObject != null)
			{
				resultObj.Data = jObject.ToObject<M>(JsonSerializer);
			}
		}

		public static void HandleErrors(ApiResponse response, params IErrorHandler[] extraErrorHandlers)
		{
			List<IErrorHandler> second = new List<IErrorHandler>
			{
				new ApiDeprecatedErrorHandler(),
				new NoInternetErrorHandler(),
				new BadConnectionErrorHandler(),
				new DefaultErrorHandler()
			};
			List<IErrorHandler> errorHandlers = extraErrorHandlers.Concat(second).ToList();
			Handle(response, errorHandlers);
		}

		public static void HandleErrorsSilently(ApiResponse response, params IErrorHandler[] extraErrorHandlers)
		{
			List<IErrorHandler> second = new List<IErrorHandler>
			{
				new ApiDeprecatedErrorHandler(),
				new NoInternetErrorHandler(IsSilent: true),
				new BadConnectionErrorHandler(IsSilent: true),
				new DefaultErrorHandler(IsSilent: true)
			};
			List<IErrorHandler> errorHandlers = extraErrorHandlers.Concat(second).ToList();
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
	}
	public class LoggingService : BaseWebService
	{
		public async Task<bool> PostAllLogs(List<LogDTO> dtos)
		{
			object t = new
			{
				Logs = dtos.ToArray()
			};
			return (await Post(t, SharedConf.URL_LOG_MESSAGE)).IsSuccessfull;
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
			HttpClientAccessor = new DefaultHttpClientAccessor();
			HttpClientAccessor.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			HttpClientAccessor.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
			HttpClientAccessor.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/zip"));
			HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("Authorization_Mobile", SharedConf.AuthorizationHeader);
			if (DeviceInfo.Platform == DevicePlatform.Unknown)
			{
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("Manufacturer", "Unknown");
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("OSVersion", "Unknown");
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("OS", "Unknown");
			}
			else
			{
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("Manufacturer", DeviceInfo.Manufacturer);
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("OSVersion", DeviceInfo.VersionString);
				HttpClientAccessor.HttpClient.DefaultRequestHeaders.Add("OS", GetOS());
			}
			HttpClientAccessor.HttpClient.MaxResponseContentBufferSize = 3000000L;
			HttpClientAccessor.HttpClient.Timeout = TimeSpan.FromSeconds(10.0);
		}

		private string GetOS()
		{
			string result = "Unknown";
			if (DeviceInfo.Platform == DevicePlatform.Android)
			{
				result = ServiceLocator.Current.GetInstance<ApiDataHelper>().DeviceType;
			}
			else if (DeviceInfo.Platform == DevicePlatform.iOS)
			{
				result = "IOS";
			}
			return result;
		}

		public void AddSecretToHeaderIfMissing()
		{
			ServiceLocator.Current.GetInstance<IHeaderService>().AddSecretToHeader(HttpClientAccessor);
		}

		public void AddHostHeaderIfMissing()
		{
			ServiceLocator.Current.GetInstance<IHeaderService>().AddHostToHeader(HttpClientAccessor);
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
			if (Connectivity.NetworkAccess == NetworkAccess.None)
			{
				return false;
			}
			return true;
		}

		public bool CheckInternetPoorConnection()
		{
			if (Connectivity.NetworkAccess == NetworkAccess.Local)
			{
				return false;
			}
			return true;
		}
	}
	public interface IHeaderService
	{
		void AddSecretToHeader(IHttpClientAccessor accessor);

		void AddHostToHeader(IHttpClientAccessor accessor);
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

		public override string ErrorMessageTitle => "CONNECTION_ERROR_TITLE".Translate();

		public override string ErrorMessage => "BAD_CONNECTION_ERROR_MESSAGE".Translate();

		public BadConnectionErrorHandler(bool IsSilent)
		{
			this.IsSilent = IsSilent;
		}

		public BadConnectionErrorHandler()
		{
		}

		public bool IsResponsible(ApiResponse apiResponse)
		{
			bool num = apiResponse.Exception?.InnerException is IOException && (apiResponse.Exception.InnerException.Message.Contains("the transport connection") || apiResponse.Exception.InnerException.Message.Contains("The server returned an invalid or unrecognized response"));
			bool flag = apiResponse.Exception is WebException;
			return num || flag;
		}

		public void HandleError(ApiResponse apiResponse)
		{
			LogUtils.LogApiError(LogSeverity.WARNING, apiResponse, IsSilent);
			if (!IsSilent)
			{
				ShowErrorToUser();
			}
		}
	}
	public class BaseErrorHandler
	{
		public IDialogService DialogServiceInstance => ServiceLocator.Current.GetInstance<IDialogService>();

		public virtual string ErrorMessageTitle => "BASE_ERROR_TITLE".Translate();

		public virtual string ErrorMessage => "BASE_ERROR_MESSAGE".Translate();

		public virtual string OkBtnText => "ERROR_OK_BTN".Translate();

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

		public override string ErrorMessageTitle => "CONNECTION_ERROR_TITLE".Translate();

		public override string ErrorMessage => "NO_INTERNET_ERROR_MESSAGE".Translate();

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
			LogUtils.LogApiError(LogSeverity.WARNING, apiResponse, IsSilent);
			if (!IsSilent)
			{
				ShowErrorToUser();
			}
		}
	}
}
namespace NDB.Covid19.ViewModels
{
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
	public class ForceUpdateViewModel
	{
		public static string FORCE_UPDATE_BUTTON_GOOGLE_ANDROID = "FORCE_UPDATE_BUTTON_GOOGLE_ANDROID".Translate();

		public static string FORCE_UPDATE_BUTTON_HUAWEI_ANDROID = "FORCE_UPDATE_BUTTON_HUAWEI_ANDROID".Translate();

		public static string FORCE_UPDATE_BUTTON_APPSTORE_IOS = "FORCE_UPDATE_BUTTON_APPSTORE_IOS".Translate();

		public static string FORCE_UPDATE_MESSAGE => "FORCE_UPDATE_MESSAGE".Translate();
	}
	public class InitializerViewModel
	{
		public static string LAUNCHER_PAGE_START_BTN => "LAUNCHER_PAGE_START_BTN".Translate();
	}
	public class SettingsPage2ViewModel
	{
		public static string SETTINGS_PAGE_2_HEADER => "SETTINGS_PAGE_2_HEADER_TEXT".Translate();

		public static string SETTINGS_PAGE_2_CONTENT => "SETTINGS_PAGE_2_CONTENT_TEXT".Translate();

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_INTRO => "SETTINGS_PAGE_2_CONTENT_TEXT_INTRO".Translate();

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_1_TITLE => "SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_1_TITLE".Translate();

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_1_CONTENT => "SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_1_CONTENT".Translate();

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_2_TITLE => "SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_2_TITLE".Translate();

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_2_CONTENT => "SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_2_CONTENT".Translate();

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_TITLE => "SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_TITLE".Translate();

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_CONTENT => "SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_CONTENT".Translate();

		public static string SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_CONTENT_LINK => "SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_3_CONTENT_LINK".Translate();
	}
	public class SettingsPage4ViewModel
	{
		public static string HEADER => "SETTINGS_PAGE_4_HEADER_TEXT".Translate();

		public static string CONTENT_TEXT_BEFORE_SUPPORT_LINK => "SETTINGS_PAGE_4_CONTENT_TEXT_BEFORE_SUPPORT_LINK".Translate();

		public static string SUPPORT_LINK_SHOWN_TEXT => "SETTINGS_PAGE_4_SUPPORT_LINK_SHOWN_TEXT".Translate();

		public static string SUPPORT_LINK => "SETTINGS_PAGE_4_SUPPORT_LINK".Translate();

		public static string EMAIL_TEXT => "SETTINGS_PAGE_4_EMAIL_TEXT".Translate();

		public static string EMAIL => "SETTINGS_PAGE_4_EMAIL".Translate();

		public static string PHONE_NUM_Text => "SETTINGS_PAGE_4_PHONE_NUM_TEXT".Translate();

		public static string PHONE_NUM => "SETTINGS_PAGE_4_PHONE_NUM".Translate();

		public static string PHONE_OPEN_TEXT => "SETTINGS_PAGE_4_PHONE_OPEN_TEXT".Translate();

		public static string PHONE_OPEN_MON_THU => "SETTINGS_PAGE_4_PHONE_OPEN_MON_THU".Translate();

		public static string PHONE_OPEN_FRE => "SETTINGS_PAGE_4_PHONE_OPEN_FRE".Translate();

		public static string PHONE_OPEN_SAT_SUN_HOLY => "SETTINGS_PAGE_4_PHONE_OPEN_SAT_SUN_HOLY".Translate();

		public static string PHONE_NUM_ACCESSIBILITY => "SETTINGS_PAGE_4_ACCESSIBILITY_PHONE_NUM".Translate();

		public static string PHONE_OPEN_MON_THU_ACCESSIBILITY => "SETTINGS_PAGE_4_ACCESSIBILITY_PHONE_OPEN_MON_THU".Translate();

		public static string PHONE_OPEN_FRE_ACCESSIBILITY => "SETTINGS_PAGE_4_ACCESSIBILITY_PHONE_OPEN_FRE".Translate();
	}
	public class SettingsPage5ViewModel
	{
		public static string SETTINGS_PAGE_5_HEADER => "SETTINGS_PAGE_5_HEADER_TEXT".Translate();

		public static string SETTINGS_PAGE_5_CONTENT => "SETTINGS_PAGE_5_CONTENT_TEXT".Translate();

		public static string SETTINGS_PAGE_5_LINK => "SETTINGS_PAGE_5_LINK".Translate();

		public static string GetVersionInfo()
		{
			return $"V{AppInfo.VersionString} B{AppInfo.BuildString} A{SharedConf.APIVersion} {GetPartialUrlFromConf()} ";
		}

		public static string GetPartialUrlFromConf()
		{
			try
			{
				string uRL_PREFIX = SharedConf.URL_PREFIX;
				int length = "Https://".Length;
				int length2 = uRL_PREFIX.IndexOf(".smittestop") - length;
				return uRL_PREFIX.Substring(length, length2);
			}
			catch
			{
				return "u";
			}
		}
	}
	public class SettingsViewModel
	{
		public static string SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON => "SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON".Translate();

		public static string SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON => "SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON".Translate();

		public bool ShowDebugItem => SettingItemList.Any((SettingItem i) => i.Type == SettingItemType.Debug);

		public List<SettingItem> SettingItemList
		{
			get;
			private set;
		}

		public SettingsViewModel()
		{
			SettingItemList = GetSettingsItemList();
		}

		private static List<SettingItem> GetSettingsItemList()
		{
			return new List<SettingItem>
			{
				new SettingItem(SettingItemType.Intro),
				new SettingItem(SettingItemType.HowItWorks),
				new SettingItem(SettingItemType.Consent),
				new SettingItem(SettingItemType.Help),
				new SettingItem(SettingItemType.About)
			};
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
			return Microsoft.Security.Application.Encoder.HtmlEncode(Sanitizer.GetSafeHtmlFragment(ReplaceMacAddress(ReplaceEmailAddress(ReplacePhoneNumber(ReplaceCpr(ReplaceIMEI(input)))))));
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
	public interface IDeviceUtils
	{
		void StopScanServices();

		void CleanDataFromDevice();
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
			ServiceLocator.Current.GetInstance<ILoggingManager>().SaveNewLog(log);
		}

		public static void LogException(LogSeverity severity, Exception e, string contextDescription, string additionalInfo = "")
		{
			LogDeviceDetails info = new LogDeviceDetails(severity, contextDescription, additionalInfo);
			LogExceptionDetails e2 = new LogExceptionDetails(e);
			LogSQLiteModel log = new LogSQLiteModel(info, null, e2);
			ServiceLocator.Current.GetInstance<ILoggingManager>().SaveNewLog(log);
		}

		public static void LogApiError(LogSeverity severity, ApiResponse apiResponse, bool erroredSilently, string additionalInfo = "")
		{
			string logMessage = apiResponse.ErrorLogMessage + (erroredSilently ? "(silent)" : "(error shown)");
			LogDeviceDetails info = new LogDeviceDetails(severity, logMessage, additionalInfo);
			LogApiDetails apiDetails = new LogApiDetails(apiResponse);
			LogExceptionDetails e = null;
			if (apiResponse.Exception != null)
			{
				e = new LogExceptionDetails(apiResponse.Exception);
			}
			LogSQLiteModel log = new LogSQLiteModel(info, apiDetails, e);
			ServiceLocator.Current.GetInstance<ILoggingManager>().SaveNewLog(log);
		}

		public static async Task SendAllLogs()
		{
			ILoggingManager manager = ServiceLocator.Current.GetInstance<ILoggingManager>();
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
			ILoggingManager manager = ServiceLocator.Current.GetInstance<ILoggingManager>();
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
	public interface IMessagingCenter
	{
		void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class;

		void Send<TSender>(TSender sender, string message) where TSender : class;

		void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class;

		void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class;

		void Unsubscribe<TSender, TArgs>(object subscriber, string message) where TSender : class;

		void Unsubscribe<TSender>(object subscriber, string message) where TSender : class;
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
		public static readonly string KEY_FORCE_UPDATE = "KEY_FORCE_UPDATE";

		public static readonly string KEY_APP_RETURNS_FROM_BACKGROUND = "KEY_APP_RETURNS_FROM_BACKGROUND";

		public static readonly string KEY_APP_WILL_ENTER_BACKGROUND = "KEY_APP_WILL_ENTER_BACKGROUND";

		public static readonly string KEY_MESSAGE_RECEIVED = "KEY_MESSAGE_RECEIVED";
	}
}
namespace NDB.Covid19.PersistedData.SQLite
{
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
			string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SharedConf.DB_NAME);
			_database = new SQLiteAsyncConnection(databasePath);
			_database.CreateTableAsync<LogSQLiteModel>().Wait();
		}

		public async void SaveNewLog(LogSQLiteModel log)
		{
			await _syncLock.WaitAsync();
			try
			{
				await _database.InsertAsync(log);
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
					await _database.Table<LogSQLiteModel>().DeleteAsync((LogSQLiteModel it) => it.ID == log.ID);
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
}
namespace NDB.Covid19.PersistedData.SecureStorage
{
	public static class SecureStorageKeys
	{
		public static readonly string CURRENT_DEVICE_ID = "SS_DEVICE_ID";

		public static readonly string CURRENT_DEVICE_TOKEN = "SS_DEVICE_TOKEN";

		public static readonly string CURRENT_DEVICE_ID_CREATED_TIMESTAMP = "SS_DEVICE_ID_CREATED_TIMESTAMP";

		public static readonly string LAST_HIGH_RISK_ALERT_UTC_KEY = "LAST_HIGH_RISK_ALERT_UTC_KEY";

		public static readonly string LAST_MEDIUM_RISK_ALERT_UTC_KEY = "LAST_MEDIUM_RISK_ALERT_UTC_KEY";

		public static readonly string LAST_SUMMARY_KEY = "LAST_SUMMARY_KEY";

		public static IEnumerable<string> GetAllKeysForCleaningDevice()
		{
			return from field in typeof(SecureStorageKeys).GetFields()
				select field.GetValue(typeof(SecureStorageKeys)) into field
				where field.GetType() == typeof(string)
				select field.ToString();
		}
	}
}
namespace NDB.Covid19.SecureStorage
{
	public interface ISecureStorageService
	{
		ISecureStorage SecureStorage
		{
			get;
		}
	}
	public class SecureStorageService : ISecureStorageService
	{
		private ISecureStorage _secureStorage;

		public ISecureStorage SecureStorage
		{
			get
			{
				if (_secureStorage == null)
				{
					_secureStorage = CrossSecureStorage.Current;
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
				return SecureStorage.GetValue(key);
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
namespace NDB.Covid19.DeviceGuid
{
	public class DeviceGuidService
	{
		private SecureStorageService _secureStorageService;

		public DeviceGuidService()
		{
			_secureStorageService = ServiceLocator.Current.GetInstance<SecureStorageService>();
		}

		public DeviceGuidService(SecureStorageService secureStorageService)
		{
			_secureStorageService = secureStorageService;
		}

		public string GetDeviceGuid()
		{
			if (_secureStorageService.KeyExists(SecureStorageKeys.CURRENT_DEVICE_ID))
			{
				string value = _secureStorageService.GetValue(SecureStorageKeys.CURRENT_DEVICE_ID);
				if (!string.IsNullOrEmpty(value))
				{
					return value;
				}
			}
			return null;
		}

		public string GetDeviceTimestamp()
		{
			if (_secureStorageService.KeyExists(SecureStorageKeys.CURRENT_DEVICE_ID_CREATED_TIMESTAMP))
			{
				string value = _secureStorageService.GetValue(SecureStorageKeys.CURRENT_DEVICE_ID_CREATED_TIMESTAMP);
				if (!string.IsNullOrEmpty(value))
				{
					return value;
				}
			}
			return null;
		}

		public bool SaveNewGuidAndToken(DeviceIdentityDTO newIdentity)
		{
			bool num = _secureStorageService.SaveValue(SecureStorageKeys.CURRENT_DEVICE_ID, newIdentity.DeviceId);
			bool flag = _secureStorageService.SaveValue(SecureStorageKeys.CURRENT_DEVICE_TOKEN, newIdentity.Token);
			bool flag2 = _secureStorageService.SaveValue(SecureStorageKeys.CURRENT_DEVICE_ID_CREATED_TIMESTAMP, DateTime.Today.ToString());
			return num && flag && flag2;
		}

		public string FetchToken()
		{
			if (_secureStorageService.KeyExists(SecureStorageKeys.CURRENT_DEVICE_TOKEN))
			{
				return _secureStorageService.GetValue(SecureStorageKeys.CURRENT_DEVICE_TOKEN);
			}
			return null;
		}

		public void DeleteAll()
		{
			_secureStorageService.Delete(SecureStorageKeys.CURRENT_DEVICE_ID);
			_secureStorageService.Delete(SecureStorageKeys.CURRENT_DEVICE_TOKEN);
			_secureStorageService.Delete(SecureStorageKeys.CURRENT_DEVICE_ID_CREATED_TIMESTAMP);
		}

		public string GetDeviceId()
		{
			return _secureStorageService.GetValue(SecureStorageKeys.CURRENT_DEVICE_ID);
		}

		public string GetTokenKey()
		{
			return _secureStorageService.GetValue(SecureStorageKeys.CURRENT_DEVICE_TOKEN);
		}

		public void SetDeviceCreatedTimestamp(DateTime dt)
		{
			if (_secureStorageService.KeyExists(SecureStorageKeys.CURRENT_DEVICE_ID_CREATED_TIMESTAMP))
			{
				_secureStorageService.Delete(SecureStorageKeys.CURRENT_DEVICE_ID_CREATED_TIMESTAMP);
			}
			_secureStorageService.SaveValue(SecureStorageKeys.CURRENT_DEVICE_ID_CREATED_TIMESTAMP, dt.ToString());
		}
	}
}
namespace NDB.Covid19.Models
{
	public class ApiResponse
	{
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

		public HttpContentHeaders Headers
		{
			get;
			set;
		}

		public bool IsSuccessfull
		{
			get
			{
				if (StatusCode == 200 || StatusCode == 201 || StatusCode == 204)
				{
					return Exception == null;
				}
				return false;
			}
		}

		public string ErrorLogMessage
		{
			get
			{
				string text = "API " + Endpoint + " failed";
				if (!new int[2]
				{
					200,
					201
				}.Contains(StatusCode))
				{
					text += $" with HttpStatusCode {StatusCode}";
				}
				else if (Exception != null)
				{
					text = text + " with " + Exception.GetType().Name;
				}
				return text;
			}
		}

		public ApiResponse(string url)
		{
			try
			{
				Endpoint = url.Split(new string[1]
				{
					SharedConf.URL_PREFIX
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

		public ApiResponse(string url)
			: base(url)
		{
			Data = default(T);
		}
	}
	public class DeviceIdentityDTO
	{
		public string DeviceId
		{
			get;
			set;
		}

		public string Token
		{
			get;
			set;
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
				SettingItemType.About => "SETTINGS_ITEM_ABOUT".Translate(), 
				SettingItemType.Consent => "SETTINGS_ITEM_CONSENT".Translate(), 
				SettingItemType.Help => "SETTINGS_ITEM_HELP".Translate(), 
				SettingItemType.HowItWorks => "SETTINGS_ITEM_HOW_IT_WORKS".Translate(), 
				SettingItemType.Intro => "SETTINGS_ITEM_INTRO".Translate(), 
				SettingItemType.Messages => "SETTINGS_ITEM_MESSAGES".Translate(), 
				SettingItemType.Debug => "Developer Tools", 
				_ => string.Empty, 
			};
		}
	}
	public class OccouranceSQliteModel
	{
		[PrimaryKey]
		[AutoIncrement]
		public int ID
		{
			get;
			set;
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		public string DeviceID
		{
			get;
			set;
		}

		public int CountLessThanOneMeter
		{
			get;
			set;
		}

		public int CountOneToTwoMeters
		{
			get;
			set;
		}

		public int CountTwoToThreeMeters
		{
			get;
			set;
		}

		public int CountThreeToFourMeters
		{
			get;
			set;
		}

		public int CountMoreThanFourMeters
		{
			get;
			set;
		}

		public string ApiVersion
		{
			get;
			set;
		}

		public string OperationMode
		{
			get;
			set;
		}

		public int TimeIntervalPerCount
		{
			get;
			set;
		}

		public int NumberOfMeasures
		{
			get;
			set;
		}

		public bool MarkedForDeletion
		{
			get;
			set;
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
	public class RawBLEMeasurementSQLiteModel
	{
		[PrimaryKey]
		[AutoIncrement]
		public int ID
		{
			get;
			set;
		}

		public DateTime Time
		{
			get;
			set;
		}

		public int DeviceIdMajor
		{
			get;
			set;
		}

		public int DeviceIdMinor
		{
			get;
			set;
		}

		public int Rssi
		{
			get;
			set;
		}

		public int TxPower
		{
			get;
			set;
		}

		public int BulkId
		{
			get;
			set;
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
			Api = apiResponse.Endpoint;
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
			ApiVersion = SharedConf.APIVersion;
			AdditionalInfo = Anonymizer.RedactText(additionalInfo);
			BuildNumber = AppInfo.BuildString;
			BuildVersion = AppInfo.VersionString;
			DeviceOSVersion = DeviceInfo.VersionString;
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
					InnerExceptionType = e.InnerException.GetType().Name;
					InnerExceptionMessage = Anonymizer.RedactText(e.InnerException.Message);
					InnerExceptionStackTrace = Anonymizer.RedactText(ShortenedText(e.InnerException.StackTrace));
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
			DeviceType = ServiceLocator.Current.GetInstance<ApiDataHelper>().DeviceType;
			DeviceDescription = ServiceLocator.Current.GetInstance<ApiDataHelper>().DeviceModel;
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
namespace NDB.Covid19.HardwareServices.SupportServices
{
	public class ApiDataHelper
	{
		public IApiDataHelper ApiDataHelperInstance => ServiceLocator.Current.GetInstance<IApiDataHelper>();

		public string DeviceModel
		{
			get
			{
				if (DeviceInfo.Platform == DevicePlatform.Android)
				{
					return DeviceInfo.Model;
				}
				return IOSHardwareMapper.GetModel(DeviceInfo.Model);
			}
		}

		public string DeviceType
		{
			get
			{
				if (DeviceInfo.Platform == DevicePlatform.Android)
				{
					if (!(DeviceInfo.Manufacturer.ToLower() == "huawei") || ApiDataHelperInstance.IsGoogleServiceEnabled())
					{
						return "Android-Google";
					}
					return "Android-Huawei";
				}
				return "IOS";
			}
		}
	}
	public interface IApiDataHelper
	{
		OperationModeEnum GetOperationMode();

		bool IsGoogleServiceEnabled();
	}
}
namespace NDB.Covid19.Enums
{
	public enum LogSeverity
	{
		INFO,
		WARNING,
		ERROR
	}
	public static class LogSeverityExtensions
	{
		public static string ToString(this LogSeverity severity)
		{
			return severity switch
			{
				LogSeverity.INFO => "INFO", 
				LogSeverity.WARNING => "WARNING", 
				LogSeverity.ERROR => "ERROR", 
				_ => null, 
			};
		}
	}
	public enum OperationModeEnum
	{
		Stopped,
		Background,
		Foreground
	}
	public enum QuestionaireSelection
	{
		YesSince,
		YesBut,
		No,
		Skip
	}
	public enum RadioButtonEnum
	{
		YES,
		YES_BUT,
		NO,
		SKIP
	}
	public enum SettingItemType
	{
		Intro,
		HowItWorks,
		Consent,
		Help,
		About,
		Messages,
		Debug
	}
}
namespace NDB.Covid19.Configuration
{
	public interface ISharedConfInterface
	{
		string URL_PREFIX
		{
			get;
		}

		int APIVersion
		{
			get;
		}

		string DB_NAME
		{
			get;
		}

		string AuthorizationHeader
		{
			get;
		}

		string URL_LOG_MESSAGE
		{
			get;
		}

		string IOSAppstoreAppLink
		{
			get;
		}

		string GooglePlayAppLink
		{
			get;
		}

		string HuaweiAppGalleryLink
		{
			get;
		}
	}
	public class SharedConf
	{
		private static ISharedConfInterface PlatformConf => ServiceLocator.Current.GetInstance<ISharedConfInterface>();

		public static string URL_PREFIX => PlatformConf.URL_PREFIX;

		public static int APIVersion => PlatformConf.APIVersion;

		public static string DB_NAME => PlatformConf.DB_NAME;

		public static string AuthorizationHeader => PlatformConf.AuthorizationHeader;

		public static string URL_LOG_MESSAGE => PlatformConf.URL_LOG_MESSAGE;

		public static string IOSAppstoreAppLink => PlatformConf.IOSAppstoreAppLink;

		public static string GooglePlayAppLink => PlatformConf.GooglePlayAppLink;

		public static string HuaweiAppGalleryLink => PlatformConf.HuaweiAppGalleryLink;
	}
}
