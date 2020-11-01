using System;
using System.Collections.Generic;
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
using NDB.Covid19.Base.AppleGoogle.Config;
using NDB.Covid19.Base.AppleGoogle.ExposureNotification;
using NDB.Covid19.Base.AppleGoogle.ExposureNotification.Helpers;
using NDB.Covid19.Base.AppleGoogle.ExposureNotification.Helpers.ExposureDetected;
using NDB.Covid19.Base.AppleGoogle.ExposureNotification.Helpers.FetchExposureKeys;
using NDB.Covid19.Base.AppleGoogle.Interfaces;
using NDB.Covid19.Base.AppleGoogle.Models;
using NDB.Covid19.Base.AppleGoogle.Models.SQLite;
using NDB.Covid19.Base.AppleGoogle.Models.UserDefinedExceptions;
using NDB.Covid19.Base.AppleGoogle.OAuth2;
using NDB.Covid19.Base.AppleGoogle.PersistedStorage.SQLite;
using NDB.Covid19.Base.AppleGoogle.ProtoModels;
using NDB.Covid19.Base.AppleGoogle.Utils;
using NDB.Covid19.Base.AppleGoogle.ViewModels;
using NDB.Covid19.Base.AppleGoogle.WebServices;
using NDB.Covid19.Base.AppleGoogle.WebServices.Helpers;
using NDB.Covid19.Configuration;
using NDB.Covid19.DeviceGuid;
using NDB.Covid19.Enums;
using NDB.Covid19.Models;
using NDB.Covid19.PersistedData.SecureStorage;
using NDB.Covid19.SecureStorage;
using NDB.Covid19.Utils;
using NDB.Covid19.ViewModels;
using NDB.Covid19.WebServices;
using NDB.Covid19.WebServices.ErrorHandlers;
using NDB.Covid19.WebServices.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLCrypto;
using SQLite;
using Unity;
using Unity.Injection;
using Xamarin.Auth;
using Xamarin.Auth.Presenters;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETStandard,Version=v2.0", FrameworkDisplayName = "")]
[assembly: AssemblyCompany("NDB.Covid19.Base.AppleGoogle")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("NDB.Covid19.Base.AppleGoogle")]
[assembly: AssemblyTitle("NDB.Covid19.Base.AppleGoogle")]
[assembly: AssemblyVersion("1.0.0.0")]
namespace NDB.Covid19.Base.AppleGoogle
{
	public static class BaseAppleGoogleDependencyInjectionConfig
	{
		public static void Init(UnityContainer unityContainer)
		{
			UnityContainerExtensions.RegisterType<ISharedConfInterface, Conf>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterType<IHeaderService, HeaderService>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterSingleton<MessagesManager>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
			UnityContainerExtensions.RegisterSingleton<SecureStorageService>((IUnityContainer)(object)unityContainer, Array.Empty<InjectionMember>());
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
				I18N.get_Current().SetNotFoundSymbol("$").SetFallbackLocale("dk")
					.AddLocaleReader((ILocaleReader)new JsonKvpReader(), ".json")
					.Init(typeof(LocalesService).GetTypeInfo().Assembly);
			}
		}

		public static void SetInternationalization(string conf)
		{
			I18N.get_Current().set_Locale(conf);
		}
	}
}
namespace NDB.Covid19.Base.AppleGoogle.WebServices
{
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
	public class ExposureNotificationWebService : BaseWebService
	{
		public async Task<bool> PostSelvExposureKeys(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
		{
			SelfDiagnosisSubmissionDTO selfDiagnosisSubmissionDTO = new SelfDiagnosisSubmissionDTO(temporaryExposureKeys);
			if (selfDiagnosisSubmissionDTO.DeviceVerificationPayload == null)
			{
				return false;
			}
			ApiResponse apiResponse = await Post(selfDiagnosisSubmissionDTO, Conf.URL_PUT_UPLOAD_DIAGNOSIS_KEYS);
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
			if (Conf.MOCK_EXPOSURE_CONFIGURATION)
			{
				return JsonConvert.DeserializeObject<Configuration>("{    'minimumRiskScore': 20,\n    'attenuationScores': [\n        1,\n        2,\n        3,\n        4,\n        5,\n        6,\n        7,\n        8\n    ],\n    'attenuationWeight': 50,\n    'daysSinceLastExposureScores': [\n        1,\n        2,\n        3,\n        4,\n        5,\n        6,\n        7,\n        8\n    ],\n    'daysSinceLastExposureWeight': 50,\n    'durationScores': [\n        1,\n        2,\n        3,\n        4,\n        5,\n        6,\n        7,\n        8\n    ],\n    'durationWeight': 50,\n    'transmissionRiskScores': [\n        1,\n        2,\n        3,\n        4,\n        5,\n        6,\n        7,\n        8\n    ],\n    'transmissionRiskWeight': 50,\n    'durationAtAttenuationThresholds': [\n        85,\n        170\n    ]\n}");
			}
			ApiResponse<Configuration> response = await Get<Configuration>(Conf.URL_GET_EXPOSURE_CONFIGURATION);
			BaseWebService.HandleErrorsSilently(response);
			if (response.IsSuccessfull && response.Data != null)
			{
				await LogUtils.SendAllLogs();
				return response.Data;
			}
			return null;
		}

		public async Task<ApiResponse<Stream>> GetDiagnosisKeys(string dateAndBatch, CancellationToken cancellationToken)
		{
			string url = Conf.URL_GET_DIAGNOSIS_KEYS + "/" + dateAndBatch + ".zip";
			ApiResponse<Stream> obj = await GetFileAsStreamAsync(url);
			BaseWebService.HandleErrorsSilently(obj);
			if (obj.IsSuccessfull)
			{
				MessageUtils.UpdateLastUpdatedDate();
			}
			return obj;
		}
	}
	public class HeaderService : IHeaderService
	{
		public void AddSecretToHeader(IHttpClientAccessor accessor)
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

		public void AddHostToHeader(IHttpClientAccessor accessor)
		{
		}
	}
}
namespace NDB.Covid19.Base.AppleGoogle.WebServices.Helpers
{
	public abstract class RedactedTekListHelper
	{
		public static string CreateRedactedTekList(IEnumerable<TemporaryExposureKey> teks)
		{
			return JsonConvert.SerializeObject((object)teks.Select((Func<TemporaryExposureKey, TemporaryExposureKey>)((TemporaryExposureKey tek) => new TemporaryExposureKey(new byte[1], tek.get_RollingStart(), tek.get_RollingDuration(), tek.get_TransmissionRiskLevel()))), BaseWebService.JsonSerializerSettings);
		}
	}
}
namespace NDB.Covid19.Base.AppleGoogle.ViewModels
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

		public async Task<bool> WithDrawConsents()
		{
			new DeviceUtils().CleanDataFromDevice();
			return true;
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
				new ConsentSectionTexts(CONSENT_SEVEN_TITLE, CONSENT_SEVEN_PARAGRAPH, null),
				new ConsentSectionTexts(CONSENT_EIGHT_TITLE, CONSENT_EIGHT_PARAGRAPH, null),
				new ConsentSectionTexts(CONSENT_NINE_TITLE, CONSENT_NINE_PARAGRAPH, null)
			};
		}
	}
	public class ENDeveloperToolsViewModel
	{
		public enum ENOperation
		{
			PUSH,
			PULL
		}

		private static bool LongRetentionTime = true;

		private DateTime MessageDateTime = DateTime.Now;

		public static string PushKeysInfo = "";

		public static string PullKeysInfo = "";

		private ExposureNotificationHandler handler;

		public Action devToolUpdateOutput;

		public string DEV_TOOLS_OUTPUT
		{
			get;
			set;
		}

		public ENDeveloperToolsViewModel()
		{
			handler = new ExposureNotificationHandler();
		}

		internal static void UpdatePushKeysInfo(ApiResponse response, SelfDiagnosisSubmissionDTO selfDiagnosisSubmissionDTO, JsonSerializerSettings settings)
		{
			PushKeysInfo = $"StatusCode: {response.StatusCode}, Time: {DateTime.Now}\n\n";
			ParseKeys(selfDiagnosisSubmissionDTO, settings, ENOperation.PUSH);
			PutInPushKeyInfoInSharedPrefs();
		}

		internal static void UpdatePullKeysInfo(ApiResponse response, SelfDiagnosisSubmissionDTO selfDiagnosisSubmissionDTO, JsonSerializerSettings settings)
		{
			PullKeysInfo = $"StatusCode: {response.StatusCode}, Time: {DateTime.Now}\n\n";
			ParseKeys(selfDiagnosisSubmissionDTO, settings, ENOperation.PULL);
			PutPullKeyInfoInSharedPrefs();
		}

		private static void ParseKeys(SelfDiagnosisSubmissionDTO selfDiagnosisSubmissionDTO, JsonSerializerSettings settings, ENOperation varAssignCheck)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			foreach (JObject item in (JArray)JObject.Parse(JsonConvert.SerializeObject((object)selfDiagnosisSubmissionDTO, settings)).get_Item("keys"))
			{
				JObject val = item;
				string text = string.Format("Key: {0} , rollingStart: {1}, rollingDuration: {2}, transmissionRiskLevel: {3}\n\n", val.get_Item("key"), val.get_Item("rollingStart"), val.get_Item("rollingDuration"), val.get_Item("transmissionRiskLevel"));
				if (varAssignCheck == ENOperation.PULL)
				{
					PullKeysInfo += text;
				}
				else
				{
					PushKeysInfo += text;
				}
			}
		}

		private static void PutPullKeyInfoInSharedPrefs()
		{
			PullKeysInfo = PullKeysInfo + "\n\n" + DeveloperToolsSingleton.Instance.PullKeyInfo;
			DeveloperToolsSingleton.Instance.PullKeyInfo = PullKeysInfo;
		}

		private static void PutInPushKeyInfoInSharedPrefs()
		{
			DeveloperToolsSingleton.Instance.LastKeyUploadInfo = PushKeysInfo;
		}

		public async Task<string> GetPushKeyInfoFromSharedPrefs()
		{
			string res = "Empty";
			PushKeysInfo = DeveloperToolsSingleton.Instance.LastKeyUploadInfo;
			if (PushKeysInfo != "")
			{
				res = PushKeysInfo;
			}
			await Clipboard.SetTextAsync(res);
			return res;
		}

		public async Task<string> GetPullKeyInfoFromSharedPrefs()
		{
			string res = "Empty";
			PullKeysInfo = DeveloperToolsSingleton.Instance.PullKeyInfo;
			if (PullKeysInfo != "")
			{
				res = PullKeysInfo;
			}
			await Clipboard.SetTextAsync(res);
			return res;
		}

		public static void SetLastPullResult(string result)
		{
			DeveloperToolsSingleton.Instance.PullKeyInfo = result;
		}

		public static string GetLastPullResult()
		{
			return DeveloperToolsSingleton.Instance.PullKeyInfo;
		}

		public string LastUsedExposureConfigurationAsync()
		{
			string lastUsedConfiguration = DeveloperToolsSingleton.Instance.LastUsedConfiguration;
			Clipboard.SetTextAsync(lastUsedConfiguration);
			return lastUsedConfiguration;
		}

		public async Task<bool> PullKeysFromServer()
		{
			DEV_TOOLS_OUTPUT = GetLastPullResult();
			bool processedAnyFiles = false;
			try
			{
				await ExposureNotification.UpdateKeysFromServer(default(CancellationToken));
				return processedAnyFiles;
			}
			catch (Exception ex)
			{
				LogUtils.LogException(LogSeverity.ERROR, ex, "PullKeysFromServer failed");
				Clipboard.SetTextAsync($"Pull keys failed:\n{ex}");
				SetLastPullResult($"Pull keys failed:\n{ex}");
				return processedAnyFiles;
			}
		}

		public async Task<bool> PullKeysFromServerAndGetExposureInfo()
		{
			DEV_TOOLS_OUTPUT = GetLastPullResult();
			bool processedAnyFiles = false;
			Preferences.Set(ExposureDetectedHelper.SHOULD_SAVE_EXPOSURE_INFOS_PREF, true);
			try
			{
				await ExposureNotification.UpdateKeysFromServer(default(CancellationToken));
				return processedAnyFiles;
			}
			catch (Exception ex)
			{
				LogUtils.LogException(LogSeverity.ERROR, ex, "PullKeysFromServer failed");
				Clipboard.SetTextAsync($"Pull keys failed:\n{ex}");
				SetLastPullResult($"Pull keys failed:\n{ex}");
				return processedAnyFiles;
			}
		}

		public string GetExposureInfosFromLastPull()
		{
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			string lastExposureInfos = DeveloperToolsSingleton.Instance.LastExposureInfos;
			string text = "";
			if (lastExposureInfos == "")
			{
				text = "We have not saved any ExposureInfos yet";
			}
			else
			{
				try
				{
					foreach (ExposureInfo item in ExposureInfoJsonHelper.ExposureInfosFromJsonCompatibleString(lastExposureInfos))
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
					LogUtils.LogException(LogSeverity.WARNING, e, "GetExposureInfosFromLastPull");
					text = "Failed at deserializing the saved ExposureInfos";
				}
			}
			string text2 = "These are the ExposureInfos we got the last time \"Pull keys and get exposure info\" was clicked:\n" + text;
			Clipboard.SetTextAsync(text2);
			return text2;
		}

		public async Task<string> FetchExposureConfigurationAsync()
		{
			Configuration val = await new ExposureNotificationHandler().GetConfigurationAsync();
			string text2 = (DEV_TOOLS_OUTPUT = $"Exposure Configuration: (mock: {Conf.MOCK_EXPOSURE_CONFIGURATION})\n" + $" AttenuationWeight: {val.get_AttenuationWeight()}, Values: {EnConfArrayString(val.get_AttenuationScores())} \n" + $" DaysSinceLastExposureWeight: {val.get_DaysSinceLastExposureWeight()}, Values: {EnConfArrayString(val.get_DaysSinceLastExposureScores())} \n" + $" DurationWeight: {val.get_DurationWeight()}, Values: {EnConfArrayString(val.get_DurationScores())} \n" + $" TransmissionWeight: {val.get_TransmissionWeight()}, Values: {EnConfArrayString(val.get_TransmissionRiskScores())} \n" + $" MinimumRiskScore: {val.get_MinimumRiskScore()}" + $" DurationAtAttenuationThresholds: [{val.get_DurationAtAttenuationThresholds()[0]},{val.get_DurationAtAttenuationThresholds()[1]}]");
			devToolUpdateOutput?.Invoke();
			Clipboard.SetTextAsync(text2);
			return text2;
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
			if (LongRetentionTime)
			{
				Conf.MAX_MESSAGE_RETENTION_TIME_IN_MINUTES = Conf.MESSAGE_RETENTION_TIME_IN_MINUTES_SHORT;
				LongRetentionTime = false;
			}
			else
			{
				Conf.MAX_MESSAGE_RETENTION_TIME_IN_MINUTES = Conf.MESSAGE_RETENTION_TIME_IN_MINUTES_LONG;
				LongRetentionTime = true;
			}
			return $"Message retention time minutes: \n{Conf.MAX_MESSAGE_RETENTION_TIME_IN_MINUTES}";
		}

		public string incementExposureDate()
		{
			MessageDateTime = MessageDateTime.AddDays(1.0);
			return $"Incremented date for Send Message function: \n{MessageDateTime}";
		}

		public string decrementExposureDate()
		{
			MessageDateTime = MessageDateTime.AddDays(-1.0);
			return $"Decremented date for Send Message function: \n{MessageDateTime}";
		}

		public string PrintLastSymptomOnsetDate()
		{
			PersonalDataModel personalData = AuthenticationState.PersonalData;
			return "Last Symptom Onset Date: " + QuestionnaireViewModel.DateLabel + ", " + $"Selection: {QuestionnaireViewModel.Selection}, " + "MiBaDate:" + personalData?.Covid19_smitte_start + ", " + $"Date used for risk calc:{personalData?.FinalMiBaDate}";
		}

		public string PrintLastPulledKeysAndTimestamp()
		{
			string text = DeveloperToolsSingleton.Instance.LastProvidedFiles;
			if (text == "")
			{
				text = "We have not saved any downloaded keys to the Storage Singleton";
			}
			string text2 = "These are the last TEK batch files provided to the EN API:\n" + text;
			Clipboard.SetTextAsync(text2);
			return text2;
		}

		public async Task SimulateExposureMessage(int notificationTriggerInSeconds = 0)
		{
			await Task.Delay(notificationTriggerInSeconds * 1000);
			await MessageUtils.CreateMessage(this, MessageDateTime);
		}

		public async Task SimulateExposureMessageAfter10Sec()
		{
			await SimulateExposureMessage(10);
		}

		public void CleanDevice()
		{
			new DeviceUtils().CleanDataFromDevice();
		}

		public string GetLastExposureSummary()
		{
			string text = ((!ServiceLocator.get_Current().GetInstance<SecureStorageService>().KeyExists(SecureStorageKeys.LAST_SUMMARY_KEY)) ? "No summary yet" : ("Last exposure summary: " + ServiceLocator.get_Current().GetInstance<SecureStorageService>().GetValue(SecureStorageKeys.LAST_SUMMARY_KEY)));
			Clipboard.SetTextAsync(text);
			return text;
		}

		public string GetLatestPullKeysTimesAndStatuses()
		{
			string latestPullKeysTimesAndStatuses = DeveloperToolsSingleton.Instance.LatestPullKeysTimesAndStatuses;
			string text;
			if (latestPullKeysTimesAndStatuses == "")
			{
				text = "We have not saved any \"pull keys\" times and statuses yet";
			}
			else
			{
				try
				{
					IEnumerable<Tuple<DateTime, string>> enumerable = JsonConvert.DeserializeObject<IEnumerable<Tuple<DateTime, string>>>(latestPullKeysTimesAndStatuses);
					text = "";
					foreach (Tuple<DateTime, string> item in enumerable)
					{
						string str = ((text == "") ? "" : "\n");
						text += str;
						text += $"[A call to \"pull keys\" at \"{item.Item1}\" UTC with status \"{item.Item2}\"]";
					}
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.ERROR, e, "Failed at deserialising pullKeysTimesAndStatusesString in GetLatestPullKeysTimesAndStatuses");
					text = "Failed at deserializing the saved pullKeysTimesAndStatusesString";
				}
			}
			string text2 = "These are the last 20 times the \"pull keys\" function was triggered, together with the statuses for the runs:\n" + text;
			Clipboard.SetTextAsync(text2);
			return text2;
		}
	}
	public static class ErrorViewModel
	{
		public static readonly string REGISTER_ERROR_NOMATCH_HEADER = Extensions.Translate("REGISTER_ERROR_NOMATCH_HEADER", Array.Empty<object>());

		public static readonly string REGISTER_ERROR_NOMATCH_DESCRIPTION = Extensions.Translate("REGISTER_ERROR_NOMATCH_DESCRIPTION", Array.Empty<object>());

		public static readonly string REGISTER_ERROR_TOOMANYTRIES_HEADER = Extensions.Translate("REGISTER_ERROR_TOOMANYTRIES_HEADER", Array.Empty<object>());

		public static readonly string REGISTER_ERROR_TOOMANYTRIES_DESCRIPTION = Extensions.Translate("REGISTER_ERROR_TOOMANYTRIES_DESCRIPTION", Array.Empty<object>());

		public static readonly string REGISTER_ERROR_HEADER = Extensions.Translate("REGISTER_ERROR_HEADER", Array.Empty<object>());

		public static readonly string REGISTER_ERROR_DESCRIPTION = Extensions.Translate("REGISTER_ERROR_DESCRIPTION", Array.Empty<object>());

		public static readonly string REGISTER_ERROR_DISMISS = Extensions.Translate("REGISTER_ERROR_DISMISS", Array.Empty<object>());

		public static readonly string REGISTER_LEAVE_HEADER = Extensions.Translate("REGISTER_LEAVE_HEADER", Array.Empty<object>());

		public static readonly string REGISTER_LEAVE_DESCRIPTION = Extensions.Translate("REGISTER_LEAVE_DESCRIPTION", Array.Empty<object>());

		public static readonly string REGISTER_LEAVE_CANCEL = Extensions.Translate("REGISTER_LEAVE_CANCEL", Array.Empty<object>());

		public static readonly string REGISTER_LEAVE_CONFIRM = Extensions.Translate("REGISTER_LEAVE_CONFIRM", Array.Empty<object>());

		public static readonly string REGISTER_ERROR_ACCESSIBILITY_CLOSE_BUTTON_TEXT = Extensions.Translate("REGISTER_ERROR_ACCESSIBILITY_CLOSE_BUTTON_TEXT", Array.Empty<object>());

		public static readonly string REGISTER_ERROR_ACCESSIBILITY_TOOMANYTRIES_HEADER = Extensions.Translate("REGISTER_ERROR_ACCESSIBILITY_TOOMANYTRIES_HEADER", Array.Empty<object>());

		public static readonly string REGISTER_ERROR_ACCESSIBILITY_TOOMANYTRIES_DESCRIPTION = Extensions.Translate("REGISTER_ERROR_ACCESSIBILITY_TOOMANYTRIES_DESCRIPTION", Array.Empty<object>());
	}
	public class InfectionStatusViewModel
	{
		private DateTime _latestMessageDateTime = DateTime.Today;

		private bool _isRunningExceptionWasLogged;

		public DialogViewModel OffDialogViewModel;

		public DialogViewModel OnDialogViewModel;

		public DialogViewModel PermissionViewModel;

		public DialogViewModel ReportingIllDialogViewModel;

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

		public EventHandler NewMessagesIconVisibilityChanged
		{
			get;
			set;
		}

		public async Task<string> StatusTxt()
		{
			return (await IsRunning()) ? INFECTION_STATUS_ACTIVE_TEXT : INFECTION_STATUS_INACTIVE_TEXT;
		}

		public async Task<string> StatusTxtDescription()
		{
			return (await IsRunning()) ? INFECTION_STATUS_ACTIVITY_STATUS_DESCRIPTION_TEXT : SMITTESPORING_INACTIVE_DESCRIPTION;
		}

		public async Task<bool> IsRunning()
		{
			try
			{
				return (int)(await ExposureNotification.GetStatusAsync()) == 2;
			}
			catch (Exception ex)
			{
				if (ex.ExposureNotificationApiNotAvailable())
				{
					if (!_isRunningExceptionWasLogged)
					{
						LogUtils.LogException(LogSeverity.ERROR, ex, "InfectionStatusViewModel.IsRunning: EN API was not available");
						_isRunningExceptionWasLogged = true;
					}
					return false;
				}
				throw ex;
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
				if (ex.ExposureNotificationApiNotAvailable())
				{
					LogUtils.LogException(LogSeverity.ERROR, ex, "InfectionStatusViewModel.IsEnabled: EN API was not available");
					return false;
				}
				throw ex;
			}
		}

		public InfectionStatusViewModel()
		{
			SubscribeMessages();
			SetDialogs();
		}

		public async Task<bool> StartBluetooth()
		{
			try
			{
				await ExposureNotification.StartAsync();
			}
			catch (Exception ex)
			{
				if (!ex.ExposureNotificationApiNotAvailable())
				{
					throw ex;
				}
				LogUtils.LogException(LogSeverity.ERROR, ex, "InfectionStatusViewModel.StartBluetooth: EN API was not available");
			}
			return await IsRunning();
		}

		public async Task<bool> StopBluetooth()
		{
			await ExposureNotification.StopAsync();
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

		private void SetDialogs()
		{
			OffDialogViewModel = new DialogViewModel
			{
				Title = Extensions.Translate("SMITTESPORING_TOGGLE_OFF_HEADER", Array.Empty<object>()),
				Body = Extensions.Translate("SMITTESPORING_TOGGLE_OFF_DESCRIPTION", Array.Empty<object>()),
				OkBtnTxt = Extensions.Translate("SMITTESPORING_TOGGLE_OFF_CONFIRM", Array.Empty<object>()),
				CancelbtnTxt = Extensions.Translate("SMITTESPORING_TOGGLE_OFF_CANCEL", Array.Empty<object>())
			};
			OnDialogViewModel = new DialogViewModel
			{
				Title = Extensions.Translate("SMITTESPORING_TOGGLE_ON_HEADER", Array.Empty<object>()),
				Body = Extensions.Translate("SMITTESPORING_TOGGLE_ON_DESCRIPTION", Array.Empty<object>()),
				OkBtnTxt = Extensions.Translate("SMITTESPORING_TOGGLE_ON_CONFIRM", Array.Empty<object>()),
				CancelbtnTxt = Extensions.Translate("SMITTESPORING_TOGGLE_ON_CANCEL", Array.Empty<object>())
			};
			PermissionViewModel = new DialogViewModel
			{
				Title = Extensions.Translate("SMITTESPORING_EN_PERMISSION_DENIED_HEADER", Array.Empty<object>()),
				Body = Extensions.Translate("SMITTESPORING_EN_PERMISSION_DENIED_BODY", Array.Empty<object>()),
				OkBtnTxt = Extensions.Translate("SMITTESPORING_EN_PERMISSION_DENIED_OK_BTN", Array.Empty<object>())
			};
			ReportingIllDialogViewModel = new DialogViewModel
			{
				Title = Extensions.Translate("SMITTESPORING_REPORTING_ILL_DIALOG_HEADER", Array.Empty<object>()),
				Body = Extensions.Translate("SMITTESPORING_REPORTING_ILL_DIALOG_BODY", Array.Empty<object>()),
				OkBtnTxt = Extensions.Translate("SMITTESPORING_REPORTING_ILL_DIALOG_OK_BTN", Array.Empty<object>())
			};
		}
	}
	public class InformationAndConsentViewModel
	{
		private NDB.Covid19.Base.AppleGoogle.OAuth2.AuthenticationManager _authManager;

		private EventHandler _onSuccess;

		private EventHandler<AuthErrorType> _onError;

		public static string INFORMATION_CONSENT_HEADER_TEXT => Extensions.Translate("INFOCONSENT_HEADER", Array.Empty<object>());

		public static string INFORMATION_CONSENT_CONTENT_TEXT => Extensions.Translate("INFOCONSENT_DESCRIPTION", Array.Empty<object>());

		public static string INFORMATION_CONSENT_NEMID_BUTTON_TEXT => Extensions.Translate("INFOCONSENT_LOGIN", Array.Empty<object>());

		public static string VERIFICATION_ERROR_TITLE => Extensions.Translate("BASE_ERROR_TITLE", Array.Empty<object>());

		public static string VERIFICATION_ERROR_MESSAGE => Extensions.Translate("BASE_ERROR_MESSAGE", Array.Empty<object>());

		public static string VERIFICATION_ERROR_BUTTON_TEXT => Extensions.Translate("ERROR_OK_BTN", Array.Empty<object>());

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
			_authManager = new NDB.Covid19.Base.AppleGoogle.OAuth2.AuthenticationManager();
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
			if (e != null && e.get_IsAuthenticated())
			{
				Account account = e.get_Account();
				if (((account != null) ? account.get_Properties() : null) != null && e.get_Account().get_Properties().ContainsKey("access_token"))
				{
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
						if (result > 0 && payloadValidateJWTToken != null)
						{
							payloadValidateJWTToken.TokenExpiration = DateTime.Now.AddSeconds(result);
						}
					}
					SaveCovidRelatedAttributes(payloadValidateJWTToken);
					if (AuthenticationState.PersonalData.Covid19_blokeret == "true")
					{
						this.OnError?.Invoke(this, AuthErrorType.MaxTriesExceeded);
					}
					else if (AuthenticationState.PersonalData.Covid19_status == "negativ")
					{
						this.OnError?.Invoke(this, AuthErrorType.NotInfected);
					}
					else if (!payloadValidateJWTToken.Validate() || AuthenticationState.PersonalData.Covid19_status == "ukendt")
					{
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
			_authManager = new NDB.Covid19.Base.AppleGoogle.OAuth2.AuthenticationManager();
			Init();
		}

		private void SaveCovidRelatedAttributes(PersonalDataModel payload)
		{
			AuthenticationState.PersonalData = payload;
		}
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
		public static readonly string MESSAGES_HEADER = Extensions.Translate("MESSAGES_HEADER", Array.Empty<object>());

		public static readonly string MESSAGES_NO_ITEMS_TITLE = Extensions.Translate("MESSAGES_NOMESSAGES_HEADER", Array.Empty<object>());

		public static readonly string MESSAGES_NO_ITEMS_DESCRIPTION = Extensions.Translate("MESSAGES_NOMESSAGES_LABEL", Array.Empty<object>());

		public static string MESSAGES_LAST_UPDATED_LABEL => Extensions.Translate("MESSAGES_LAST_UPDATED_LABEL", Array.Empty<object>());

		public static string MESSAGES_ACCESSIBILITY_CLOSE_BUTTON => Extensions.Translate("MESSAGES_ACCESSIBILITY_CLOSE_BUTTON", Array.Empty<object>());

		public static DateTime LastUpdateDateTime => MessageUtils.GetUpdatedDateTime();

		public static string LastUpdateString
		{
			get
			{
				if (!(LastUpdateDateTime != DateTime.MinValue))
				{
					return "";
				}
				return string.Format(MESSAGES_LAST_UPDATED_LABEL, DateUtils.GetDateFromDateTime(LastUpdateDateTime, "d. MMMMM") ?? "", DateUtils.GetDateFromDateTime(LastUpdateDateTime, "HH:mm") ?? "");
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
		public static string Title => Extensions.Translate("NOTIFICATION_HEADER", Array.Empty<object>());

		public static string Body => Extensions.Translate("NOTIFICATION_DESCRIPTION", Array.Empty<object>());
	}
	public class QuestionnaireViewModel
	{
		public static string REGISTER_QUESTIONAIRE_HEADER = Extensions.Translate("REGISTER_QUESTIONAIRE_HEADER", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_TEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YES = Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YES", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YESBUT = Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YESBUT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_NO = Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_NO", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_SKIP = Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_SKIP", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SYMPTOMONSET_HELP = Extensions.Translate("REGISTER_QUESTIONAIRE_SYMPTOMONSET_HELP", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_NEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_NEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SHARING_TEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_SHARING_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SHARING_ANSWER_YES = Extensions.Translate("REGISTER_QUESTIONAIRE_SHARING_ANSWER_YES", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SHARING_ANSWER_NO = Extensions.Translate("REGISTER_QUESTIONAIRE_SHARING_ANSWER_NO", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SHARING_ANSWER_SKIP = Extensions.Translate("REGISTER_QUESTIONAIRE_SHARING_ANSWER_SKIP", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_SUBMIT = Extensions.Translate("REGISTER_QUESTIONAIRE_SUBMIT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_HEADER = Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_HEADER", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_TEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_DESCRIPTION = Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_DESCRIPTION", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_DISMISS = Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_DISMISS", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_INNER_HEADER = Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_INNER_HEADER", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_INNER_READ_MORE = Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_INNER_READ_MORE", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_RECEIPT_LINK = Extensions.Translate("REGISTER_QUESTIONAIRE_RECEIPT_LINK", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_CLOSE_BUTTON_TEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_CLOSE_BUTTON_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_1_TEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_1_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_2_TEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_2_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_3_TEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_3_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_4_TEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_4_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_DATEPICKER_TEXT = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_DATEPICKER_TEXT", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_DATE_INFO_BUTTON = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_DATE_INFO_BUTTON", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_LOADING_PAGE_TITLE = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_LOADING_PAGE_TITLE", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_DATEPICKER = Extensions.Translate("REGISTER_QUESTIONAIRE_CHOOSE_DATE_POP_UP", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_HEADER = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_HEADER", Array.Empty<object>());

		public static string REGISTER_QUESTIONAIRE_ACCESSIBILITY_RECEIPT_HEADER = Extensions.Translate("REGISTER_QUESTIONAIRE_ACCESSIBILITY_RECEIPT_HEADER", Array.Empty<object>());

		public DialogViewModel CloseDialogViewModel;

		public static string DateLabel
		{
			get
			{
				if (!(_selectedDateUTC == DateTime.MinValue))
				{
					return _localSelectedDate.ToString("dd/MM/yyyy");
				}
				return "dd/mm/åååå";
			}
		}

		private static DateTime _selectedDateUTC
		{
			get;
			set;
		}

		private static DateTime _localSelectedDate => DateTime.SpecifyKind(_selectedDateUTC, DateTimeKind.Utc).ToLocalTime();

		private static QuestionaireSelection _selection
		{
			get;
			set;
		}

		public static QuestionaireSelection Selection => _selection;

		public DateTime MinimumDate
		{
			get;
			private set;
		}

		public DateTime MaximumDate
		{
			get;
			private set;
		}

		public string RadioButtonAccessibilityDatepicker => REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YES + ". " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_DATEPICKER_TEXT + ". " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_1_TEXT;

		public string RadioButtonAccessibilityYesDontRemember => REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YESBUT + ". " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_2_TEXT;

		public string RadioButtonAccessibilityNo => REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_NO + ". " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_3_TEXT;

		public string RadioButtonAccessibilitySkip => REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_SKIP + ". " + REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_4_TEXT;

		public string ReceipetPageReadMoreButtonAccessibility => REGISTER_QUESTIONAIRE_RECEIPT_INNER_READ_MORE;

		public QuestionnaireViewModel()
		{
			_selection = QuestionaireSelection.Skip;
			MinimumDate = new DateTime(2020, 1, 1, 0, 0, 0).ToUniversalTime();
			MaximumDate = DateTime.Today.ToUniversalTime();
			CloseDialogViewModel = new DialogViewModel
			{
				Title = ErrorViewModel.REGISTER_LEAVE_HEADER,
				Body = ErrorViewModel.REGISTER_LEAVE_DESCRIPTION,
				OkBtnTxt = ErrorViewModel.REGISTER_LEAVE_CONFIRM,
				CancelbtnTxt = ErrorViewModel.REGISTER_LEAVE_CANCEL
			};
		}

		public void SetSelectedDateUTC(DateTime newDate)
		{
			_selectedDateUTC = newDate;
		}

		public void SetSelection(QuestionaireSelection selection)
		{
			_selection = selection;
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

		~QuestionnaireViewModel()
		{
		}
	}
	public class WelcomeViewModel
	{
		private DeviceGuidService deviceGuidService;

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

		public static string ANNOUNCEMENT_PAGE_CHANGED_TO_FIVE => Extensions.Translate("WELCOME_PAGE_ACCESSIBILITY_ANNOUNCEMENT_PAGE_CHANGED_TO_FIVE", Array.Empty<object>());

		public static string CONTENT_DESCRIPTOR_NEXT_BUTTON_ENABLED => Extensions.Translate("WELCOME_PAGE_CONTENT_DESCRIPTOR_NEXT_BUTTON_ENABLED", Array.Empty<object>());

		public static string CONTENT_DESCRIPTOR_NEXT_BUTTON_DISABLED => Extensions.Translate("WELCOME_PAGE_CONTENT_DESCRIPTOR_NEXT_BUTTON_DISABLED", Array.Empty<object>());

		public static string TRANSMISSION_ERROR_MSG => Extensions.Translate("TRANSMISSION_ERROR_MSG", Array.Empty<object>());

		public static string TRANSMISSION_ERROR_MSG_NOTIFICATION_TEXT => Extensions.Translate("TRANSMISSION_ERROR_MSG_NOTIFICATION_TEXT", Array.Empty<object>());

		public static string WELCOME_PAGE_TWO_ACCESSIBILITY_TITLE => Extensions.Translate("WELCOME_PAGE_TWO_ACCESSIBILITY_TITLE", Array.Empty<object>());

		public static string WELCOME_PAGE_TWO_ACCESSIBILITY_BODY_ONE => Extensions.Translate("WELCOME_PAGE_TWO_ACCESSIBILITY_BODY_ONE", Array.Empty<object>());

		public WelcomeViewModel()
		{
			deviceGuidService = ServiceLocator.get_Current().GetInstance<DeviceGuidService>();
		}
	}
}
namespace NDB.Covid19.Base.AppleGoogle.PersistedStorage.SQLite
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
	public class MessagesManager : IMessagesManager
	{
		private readonly SQLiteAsyncConnection _database;

		private static readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

		public MessagesManager()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			_database = new SQLiteAsyncConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SharedConf.DB_NAME), true);
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
namespace NDB.Covid19.Base.AppleGoogle.Utils
{
	public class LocalPreferences
	{
		public static void SetIsOnboardingCompleted(bool isOnboardingCompleted)
		{
			Preferences.Set("isOnboardingCompleted", isOnboardingCompleted);
		}

		public static bool GetIsOnboardingCompleted()
		{
			return Preferences.Get("isOnboardingCompleted", false);
		}
	}
	public static class DateUtils
	{
		public static readonly CultureInfo cultureInfo = CultureInfo.CurrentCulture;

		public static string GetDateFromDateTime(this DateTime? date, string dateFormat)
		{
			if (date.HasValue)
			{
				return date.Value.ToString(dateFormat, cultureInfo);
			}
			return string.Empty;
		}
	}
	public sealed class DeveloperToolsSingleton
	{
		public string PullKeyInfo
		{
			get
			{
				return PullKeyInfo;
			}
			set
			{
				PullKeyInfo = value;
			}
		}

		public string LastKeyUploadInfo
		{
			get
			{
				return PullKeyInfo;
			}
			set
			{
				PullKeyInfo = value;
			}
		}

		public string LastUsedConfiguration
		{
			get
			{
				return PullKeyInfo;
			}
			set
			{
				PullKeyInfo = value;
			}
		}

		public string LastProvidedFiles
		{
			get
			{
				return PullKeyInfo;
			}
			set
			{
				PullKeyInfo = value;
			}
		}

		public string LatestPullKeysTimesAndStatuses
		{
			get
			{
				return PullKeyInfo;
			}
			set
			{
				PullKeyInfo = value;
			}
		}

		public string LastExposureInfos
		{
			get
			{
				return PullKeyInfo;
			}
			set
			{
				PullKeyInfo = value;
			}
		}

		public static DeveloperToolsSingleton Instance
		{
			get;
		}

		public void ClearAllFields()
		{
			PullKeyInfo = "";
			LastKeyUploadInfo = "";
			LastUsedConfiguration = "";
			LastProvidedFiles = "";
			LatestPullKeysTimesAndStatuses = "";
			LastExposureInfos = "";
		}

		static DeveloperToolsSingleton()
		{
			Instance = new DeveloperToolsSingleton();
		}

		private DeveloperToolsSingleton()
		{
		}
	}
	public class DeviceUtils : IDeviceUtils
	{
		public void CleanDataFromDevice()
		{
			ServiceLocator.get_Current().GetInstance<DeviceGuidService>().DeleteAll();
			DeveloperToolsSingleton.Instance.ClearAllFields();
			HttpClientManager.MakeNewInstance();
			Preferences.Clear();
			MessageUtils.RemoveAll();
			LocalPreferences.SetIsOnboardingCompleted(isOnboardingCompleted: false);
			foreach (string item in SecureStorageKeys.GetAllKeysForCleaningDevice())
			{
				ServiceLocator.get_Current().GetInstance<SecureStorageService>().Delete(item);
			}
		}

		public async void StopScanServices()
		{
			await ExposureNotification.StopAsync();
		}
	}
	public static class MessageUtils
	{
		public static readonly string MESSAGES_LAST_UPDATED_PREF = "MESSAGES_LAST_UPDATED_PREF";

		public static IMessagesManager Manager => ServiceLocator.get_Current().GetInstance<MessagesManager>();

		public static string MESSAGES_MESSAGE_HEADER => Extensions.Translate("MESSAGES_MESSAGE_HEADER", Array.Empty<object>());

		public static string INFECTION_MESSAGES_LINK => Extensions.Translate("MESSAGES_LINK", Array.Empty<object>());

		public static void CreateTestMessages()
		{
			MessageSQLiteModel log = new MessageSQLiteModel
			{
				IsRead = false,
				MessageLink = "https://www.netcompany.com",
				TimeStamp = DateTime.Now.Subtract(TimeSpan.FromDays(20.0)),
				Title = "Du har opholdt dig på tæt afstand af en COVID - 19 positiv"
			};
			MessageSQLiteModel log2 = new MessageSQLiteModel
			{
				IsRead = false,
				MessageLink = "https://www.netcompany.com",
				TimeStamp = DateTime.Now.Subtract(TimeSpan.FromDays(15.0)),
				Title = "Du har opholdt dig på tæt afstand af en COVID - 19 positiv"
			};
			MessageSQLiteModel log3 = new MessageSQLiteModel
			{
				IsRead = false,
				MessageLink = "https://www.netcompany.com",
				TimeStamp = DateTime.Now,
				Title = "Du har opholdt dig på tæt afstand af en COVID - 19 positiv"
			};
			Manager.SaveNewMessage(log);
			Manager.SaveNewMessage(log2);
			Manager.SaveNewMessage(log3);
		}

		private static async Task CreateAndSaveNewMessage(object Sender, DateTime? customTime = null, int triggerAfterNSec = 0)
		{
			MessageSQLiteModel log = new MessageSQLiteModel
			{
				IsRead = false,
				MessageLink = INFECTION_MESSAGES_LINK,
				TimeStamp = (customTime ?? DateTime.Now),
				Title = MESSAGES_MESSAGE_HEADER
			};
			await Manager.SaveNewMessage(log);
			MessagingCenter.Send(Sender, MessagingCenterKeys.KEY_MESSAGE_RECEIVED);
			ServiceLocator.get_Current().GetInstance<ILocalNotificationsManager>().GenerateLocalNotification(new NotificationViewModel(), triggerAfterNSec);
		}

		public static async Task CreateMessage(object Sender, DateTime? customTime = null, int triggerAfterNSec = 0)
		{
			await CreateAndSaveNewMessage(Sender, customTime, triggerAfterNSec);
		}

		public static async Task CreateTestMessage(object Sender, DateTime? customTime = null, int triggerNotificationInSeconds = 0)
		{
			ServiceLocator.get_Current().GetInstance<ILocalNotificationsManager>().GenerateLocalNotification(new NotificationViewModel(), triggerNotificationInSeconds);
			await Task.Delay(triggerNotificationInSeconds);
			await CreateAndSaveNewMessage(Sender, customTime);
		}

		public static async Task<int> SaveMessages(MessageSQLiteModel message)
		{
			return await Manager.SaveNewMessage(message);
		}

		public static async Task<List<MessageSQLiteModel>> GetMessages()
		{
			return (await Manager.GetMessages()).OrderByDescending((MessageSQLiteModel x) => x.TimeStamp).ToList();
		}

		public static async Task<bool> isTheLatestMessageFromToday()
		{
			return GetUpdatedDateTime().DayOfYear == DateTime.Now.DayOfYear;
		}

		public static async Task<List<MessageSQLiteModel>> GetAllUnreadMessages()
		{
			return await Manager.GetUnreadMessages();
		}

		public static void RemoveAll()
		{
			Manager.DeleteAll();
		}

		public static async Task RemoveMessages(List<MessageSQLiteModel> messages)
		{
			await Manager.DeleteMessages(messages);
		}

		public static async Task RemoveAllOlderThan(int minutes)
		{
			await RemoveMessages((await GetMessages()).FindAll((MessageSQLiteModel message) => DateTime.Now.Subtract(message.TimeStamp).TotalMinutes >= (double)minutes).ToList());
		}

		public static void MarkAsRead(MessageItemViewModel message, bool isRead)
		{
			Manager.MarkAsRead(new MessageSQLiteModel(message), isRead);
		}

		public static List<MessageItemViewModel> ToMessageItemViewModelList(List<MessageSQLiteModel> list)
		{
			return list.Select((MessageSQLiteModel model) => new MessageItemViewModel(model)).ToList();
		}

		public static void UpdateLastUpdatedDate()
		{
			Preferences.Set(MESSAGES_LAST_UPDATED_PREF, DateTime.Now);
		}

		public static DateTime GetUpdatedDateTime()
		{
			return Preferences.Get(MESSAGES_LAST_UPDATED_PREF, DateTime.MinValue);
		}
	}
}
namespace NDB.Covid19.Base.AppleGoogle.OAuth2
{
	public class AuthenticationManager
	{
		private OAuthLoginPresenter _presenter;

		public EventHandler<AuthenticatorCompletedEventArgs> _completedHandler;

		public EventHandler<AuthenticatorErrorEventArgs> _errorHandler;

		public static JsonSerializer JsonSerializer = new JsonSerializer();

		public void Setup(EventHandler<AuthenticatorCompletedEventArgs> completedHandler, EventHandler<AuthenticatorErrorEventArgs> errorHandler)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			_presenter = new OAuthLoginPresenter();
			AuthenticationState.Authenticator = new CustomOAuth2Authenticator(Conf.OAUTH2_CLIENT_ID, null, Conf.OAUTH2_SCOPE, new Uri(Conf.OAUTH2_AUTHORISE_URL), new Uri(Conf.OAUTH2_REDIRECT_URL), new Uri(Conf.OAUTH2_ACCESSTOKEN_URL), null, isUsingNativeUI: true);
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
			_presenter = null;
			AuthenticationState.Authenticator = null;
		}

		public PersonalDataModel GetPayloadValidateJWTToken(string accessToken)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			try
			{
				byte[] rawData = Convert.FromBase64String(Conf.OAUTH2_VERIFY_TOKEN_PUBLIC_KEY);
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

		public static string AuthLog = "";

		public static PersonalDataModel PersonalData;

		public static string DeviceVerificationToken = "";

		public static void AddAuthLog(string addString)
		{
			if (AuthLog != "")
			{
				AuthLog += " $$ ";
			}
			AuthLog += addString;
		}

		public static void LogAndResetAuth(bool success)
		{
			if (AuthLog != "")
			{
				string str = (success ? "Logged in with OAuth2" : "Failed to log in with OAuth2");
				LogUtils.LogMessage(LogSeverity.INFO, "OAuth2LoginFlow: " + str, AuthLog);
				AuthLog = "";
			}
		}
	}
	public enum AuthErrorType
	{
		Unknown,
		AuthenticationFailed,
		MaxTriesExceeded,
		NotInfected
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
			AuthenticationState.AddAuthLog("OnPageLoading: " + url?.AbsoluteUri);
			((WebRedirectAuthenticator)this).OnPageLoading(url);
		}

		protected override async void OnRedirectPageLoaded(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
		{
			AuthenticationState.AddAuthLog("OnRedirectPageLoaded: " + url);
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
				else
				{
					LogUtils.LogMessage(LogSeverity.INFO, "CustomOAuth2Authenticator successfully refreshed token");
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
namespace NDB.Covid19.Base.AppleGoogle.Models
{
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

		public bool Validate()
		{
			bool num = !string.IsNullOrEmpty(Covid19_smitte_start);
			bool flag = TokenExpiration.HasValue && TokenExpiration > DateTime.Now;
			return num && flag;
		}
	}
	public class SelfDiagnosisSubmissionDTO
	{
		public IEnumerable<TemporaryExposureKey> Keys
		{
			get;
			set;
		}

		public List<string> Regions
		{
			get;
			set;
		}

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

		public string DeviceVerificationPayload
		{
			get;
			set;
		}

		public string Padding
		{
			get;
			set;
		}

		public SelfDiagnosisSubmissionDTO(IEnumerable<TemporaryExposureKey> keys)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Keys = keys;
			AppPackageName = AppInfo.get_PackageName();
			DevicePlatform platform = DeviceInfo.get_Platform();
			Platform = ((object)(DevicePlatform)(ref platform)).ToString();
			Regions = Conf.SUPPORTED_REGIONS.ToList();
			DeviceVerificationPayload = AuthenticationState.DeviceVerificationToken;
			computePadding();
		}

		private void computePadding()
		{
			Padding = "";
			int num = new Random().Next(17, 35);
			for (int i = 1; i <= num; i++)
			{
				Padding += Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(AppPackageName + Platform + DeviceVerificationPayload + DateTime.UtcNow.Ticks)));
			}
		}
	}
}
namespace NDB.Covid19.Base.AppleGoogle.Models.UserDefinedExceptions
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
}
namespace NDB.Covid19.Base.AppleGoogle.Models.SQLite
{
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
namespace NDB.Covid19.Base.AppleGoogle.Interfaces
{
	public interface ILocalNotificationsManager
	{
		void GenerateLocalNotification(NotificationViewModel notificationViewModel, int triggerInSeconds);
	}
	public interface IStopBackgroundService
	{
		void StopBackgroundService();
	}
}
namespace NDB.Covid19.Base.AppleGoogle.ProtoModels
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
namespace NDB.Covid19.Base.AppleGoogle.ExposureNotification
{
	public class ExposureNotificationHandler : IExposureNotificationHandler
	{
		private ExposureNotificationWebService exposureNotificationWebService = new ExposureNotificationWebService();

		private DateTime? MiBaDate => AuthenticationState.PersonalData?.FinalMiBaDate;

		public string UserExplanation => "Saving ExposureInfos with \"Pull Keys and Save ExposureInfos\" causes the EN API to display this notification (not a bug)";

		public async Task ExposureDetectedAsync(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getExposureInfo)
		{
			await ExposureDetectedHelper.GenerateMessageIfAppropriate(summary, this);
			await ExposureDetectedHelper.SaveLastExposureInfosIfReleaseAndPrefIsTrue(getExposureInfo);
			ExposureDetectedHelper.SaveLastSummary(summary);
		}

		public async Task FetchExposureKeyBatchFilesFromServerAsync(Func<IEnumerable<string>, Task> submitBatches, CancellationToken cancellationToken)
		{
			await new FetchExposureKeysHelper().FetchExposureKeyBatchFilesFromServerAsync(submitBatches, cancellationToken);
		}

		public Task<Configuration> GetConfigurationAsync()
		{
			return Task.Run<Configuration>(async delegate
			{
				Configuration obj = await exposureNotificationWebService.GetExposureConfiguration();
				string arg = JsonConvert.SerializeObject((object)obj);
				DeveloperToolsSingleton.Instance.LastUsedConfiguration = $"Time used: {DateTime.Now}\n{arg}";
				return obj;
			});
		}

		public async Task UploadSelfExposureKeysToServerAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
		{
			if (DeviceInfo.get_Platform() == DevicePlatform.get_iOS())
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
			if (AuthenticationState.PersonalData?.Access_token == null)
			{
				throw new AccessTokenMissingFromNemIDException("The token from NemID is not set");
			}
			if (!MiBaDate.HasValue)
			{
				throw new MiBaDateMissingException("The symptom onset date is not set from the calling view model");
			}
			DateTime symptomsDate = MiBaDate.Value.ToUniversalTime();
			List<TemporaryExposureKey> keys = UploadDiagnosisKeysHelper.createAValidListOfTemporaryExposureKeys(temporaryExposureKeys);
			keys = UploadDiagnosisKeysHelper.SetTransmissionRiskLevel(keys, symptomsDate);
			if (!(await exposureNotificationWebService.PostSelvExposureKeys(keys)))
			{
				throw new FailedToPushToServerException("Failed to push keys to the server");
			}
		}
	}
	public abstract class BatchFileHelper
	{
		public static IEnumerable<string> SaveZipStreamToBinAndSig(Stream zipStream)
		{
			ZipArchive zipArchive = new ZipArchive(zipStream);
			ZipArchiveEntry entry = zipArchive.GetEntry("export.bin");
			ZipArchiveEntry entry2 = zipArchive.GetEntry("export.sig");
			Stream stream = entry.Open();
			Stream stream2 = entry2.Open();
			string text = Path.Combine(FileSystem.get_CacheDirectory(), Guid.NewGuid().ToString() + ".bin");
			string text2 = Path.Combine(FileSystem.get_CacheDirectory(), Guid.NewGuid().ToString() + ".sig");
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

		public static string TemporaryExposureKeyExportToPrettyString(TemporaryExposureKeyExport temporaryExposureKeyExport)
		{
			string str = "TEK batch, containing these keys:\n";
			string text;
			if (((IEnumerable<TemporaryExposureKey>)temporaryExposureKeyExport.Keys).Count() > 200)
			{
				text = "More than 200 keys in the batch. Displaying all of them would take too long";
			}
			else
			{
				text = "";
				foreach (TemporaryExposureKey key in temporaryExposureKeyExport.Keys)
				{
					string str2 = ((text == "") ? "--" : "\n--");
					text += str2;
					text = text + $"[TemporaryExposureKey with KeyData.ToBase64()={key.KeyData.ToBase64()}, TransmissionRiskLevel={key.TransmissionRiskLevel}, " + $"RollingStartIntervalNumber={DateTimeOffset.FromUnixTimeSeconds(key.RollingStartIntervalNumber * 600).UtcDateTime} UTC and RollingPeriod={key.RollingPeriod * 10} minutes";
				}
			}
			return str + text;
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
}
namespace NDB.Covid19.Base.AppleGoogle.ExposureNotification.Helpers
{
	public abstract class ExposureDetectedHelper
	{
		public static readonly string SHOULD_SAVE_EXPOSURE_INFOS_PREF = "SHOULD_SAVE_EXPOSURE_INFOS_PREF";

		private static readonly SecureStorageService _secureStorageService = ServiceLocator.get_Current().GetInstance<SecureStorageService>();

		public static async Task GenerateMessageIfAppropriate(ExposureDetectionSummary summary, object messageSender)
		{
			if (summary.get_MatchedKeyCount() != 0L && summary.get_HighestRiskScore() >= 1 && (HasNotSavedASummaryYet() || SummaryIsWorseThanTheSavedSummary(summary)))
			{
				if (summary.get_HighestRiskScore() >= Conf.RISK_SCORE_THRESHOLD_FOR_HIGH_RISK)
				{
					await AlertAboutHighRiskIfAppropriate(messageSender);
				}
				else
				{
					AlertAboutMediumRiskIfAppropriate(messageSender);
				}
			}
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
				LogUtils.LogException(LogSeverity.ERROR, e, "ExposureDetectedHelper.SaveLastSummary");
			}
		}

		public static async Task SaveLastExposureInfosIfReleaseAndPrefIsTrue(Func<Task<IEnumerable<ExposureInfo>>> getExposureInfo)
		{
			bool flag;
			try
			{
				flag = Preferences.Get(SHOULD_SAVE_EXPOSURE_INFOS_PREF, false);
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.WARNING, e, "SaveLastExposureInfosIfPrefIsTrue");
				flag = false;
			}
			if (flag)
			{
				Preferences.Set(SHOULD_SAVE_EXPOSURE_INFOS_PREF, false);
				string lastExposureInfos = ExposureInfoJsonHelper.ExposureInfosToJson(await getExposureInfo());
				DeveloperToolsSingleton.Instance.LastExposureInfos = lastExposureInfos;
			}
		}

		private static async Task AlertAboutHighRiskIfAppropriate(object messageSender)
		{
			if (!HasAlertedAboutHighRiskToday() && !HasAlertedAboutMediumRiskToday())
			{
				await MessageUtils.CreateMessage(messageSender);
				try
				{
					_secureStorageService.SaveValue(SecureStorageKeys.LAST_HIGH_RISK_ALERT_UTC_KEY, DateTime.UtcNow.ToString());
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.ERROR, e, "ExposureDetectedHelper.GenerateMessageIfAppropriate");
				}
			}
		}

		private static bool HasAlertedAboutHighRiskToday()
		{
			return UtcDateTimeToDenmarkDate(GetLastHighRiskAlertUtc()) == UtcDateTimeToDenmarkDate(DateTime.UtcNow);
		}

		private static DateTime GetLastHighRiskAlertUtc()
		{
			try
			{
				if (_secureStorageService.KeyExists(SecureStorageKeys.LAST_HIGH_RISK_ALERT_UTC_KEY))
				{
					return DateTime.Parse(_secureStorageService.GetValue(SecureStorageKeys.LAST_HIGH_RISK_ALERT_UTC_KEY));
				}
				return DateTime.UtcNow.AddDays(-100.0);
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, "ExposureDetectedHelper.GenerateMessageIfAppropriate");
				return DateTime.UtcNow.AddDays(-100.0);
			}
		}

		private static void AlertAboutMediumRiskIfAppropriate(object messageSender)
		{
			if (!HasAlertedAboutMediumRiskToday())
			{
				MessageUtils.CreateMessage(messageSender);
				try
				{
					_secureStorageService.SaveValue(SecureStorageKeys.LAST_MEDIUM_RISK_ALERT_UTC_KEY, DateTime.UtcNow.ToString());
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.ERROR, e, "ExposureDetectedHelper.GenerateMessageIfAppropriate");
				}
			}
		}

		private static bool HasAlertedAboutMediumRiskToday()
		{
			return UtcDateTimeToDenmarkDate(GetLastMediumRiskAlertUtc()) == UtcDateTimeToDenmarkDate(DateTime.UtcNow);
		}

		private static DateTime GetLastMediumRiskAlertUtc()
		{
			try
			{
				if (_secureStorageService.KeyExists(SecureStorageKeys.LAST_MEDIUM_RISK_ALERT_UTC_KEY))
				{
					return DateTime.Parse(_secureStorageService.GetValue(SecureStorageKeys.LAST_MEDIUM_RISK_ALERT_UTC_KEY));
				}
				return DateTime.UtcNow.AddDays(-100.0);
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, "ExposureDetectedHelper.GenerateMessageIfAppropriate");
				return DateTime.UtcNow.AddDays(-100.0);
			}
		}

		private static bool HasNotSavedASummaryYet()
		{
			return !_secureStorageService.KeyExists(SecureStorageKeys.LAST_SUMMARY_KEY);
		}

		private static bool SummaryIsWorseThanTheSavedSummary(ExposureDetectionSummary summary)
		{
			try
			{
				ExposureDetectionSummary val = ExposureDetectionSummaryJsonHelper.ExposureDetectionSummaryFromJsonCompatibleString(_secureStorageService.GetValue(SecureStorageKeys.LAST_SUMMARY_KEY));
				return summary.get_DaysSinceLastExposure() < val.get_DaysSinceLastExposure() || summary.get_MatchedKeyCount() > val.get_MatchedKeyCount() || summary.get_HighestRiskScore() > val.get_HighestRiskScore() || FirstHasAtLeastOneAttenuationDurationLargerThanSecond(summary, val) || summary.get_SummationRiskScore() > val.get_SummationRiskScore();
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, "ExposureDetectedHelper.HasSummaryGottenWorse");
				_secureStorageService.Delete(SecureStorageKeys.LAST_SUMMARY_KEY);
				return true;
			}
		}

		private static bool FirstHasAtLeastOneAttenuationDurationLargerThanSecond(ExposureDetectionSummary first, ExposureDetectionSummary second)
		{
			if (first.get_AttenuationDurations().Length != second.get_AttenuationDurations().Length)
			{
				throw new Exception("The AttenuationDuration arrays in the ExposureDetectionSummary objects given to FirstHasAtLeastOneAttenuationDurationLargerThanSecond did not have the same Length");
			}
			for (int i = 0; i < first.get_AttenuationDurations().Length; i++)
			{
				if (first.get_AttenuationDurations()[i] > second.get_AttenuationDurations()[i])
				{
					return true;
				}
			}
			return false;
		}

		private static DateTime UtcDateTimeToDenmarkDate(DateTime utcDateTime)
		{
			return utcDateTime.AddHours(2.0).Date;
		}
	}
	public abstract class ExposureDetectionSummaryJsonHelper
	{
		private class JsonCompatibleExposureDetectionSummary
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
	public class FetchExposureKeysHelper
	{
		public async Task FetchExposureKeyBatchFilesFromServerAsync(Func<IEnumerable<string>, Task> submitBatches, CancellationToken cancellationToken)
		{
			StoredValueHelper.SavePullCallTime();
			if (StoredValueHelper.LastDownloadZipsTooRecent())
			{
				SaveAndShowLastPullResult("Pull aborted. The last time we ran DownloadZips was too recent");
				return;
			}
			if (StoredValueHelper.TooManySubmitBatches())
			{
				SaveAndShowLastPullResult("Pull aborted. We have done submitBatches too many times in 24 hours");
				return;
			}
			IEnumerable<string> zips = await new ZipDownloader().DownloadZips(cancellationToken);
			await SubmitZips(zips, submitBatches);
			DeleteZips(zips);
		}

		private Task SubmitZips(IEnumerable<string> zips, Func<IEnumerable<string>, Task> submitBatches)
		{
			if (zips.Count() == 0)
			{
				SaveAndShowLastPullResult("No zips were given to SubmitZips. Probably because A: \"Our first request to the server got something other than response code Ok or response code No Content\" or B: \"We got response code No Content for all the requests we made to the server\"");
				return Task.CompletedTask;
			}
			StoredValueHelper.StoreLastProvidedFiles(zips);
			try
			{
				StoredValueHelper.AddSubmitBatchesCall();
				SaveAndShowLastPullResult("Zip files submitted");
				return submitBatches(zips);
			}
			catch (Exception ex)
			{
				LogUtils.LogException(LogSeverity.ERROR, ex, "FetchExposureKeyBatchFilesFromServerAsync, submitBatches failed when submitting the files to the EN API");
				SaveAndShowLastPullResult($"Pull keys failed:\n{ex}");
				return Task.CompletedTask;
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
					LogUtils.LogException(LogSeverity.WARNING, e, "Caught Exception when deleting temporary zip files in DownloadDiagnosisKeysHelper.DeleteZips");
				}
			}
		}

		private void SaveAndShowLastPullResult(string lastPullResult)
		{
			StoredValueHelper.SaveStatusOfLastPullCall(lastPullResult);
			ENDeveloperToolsViewModel.SetLastPullResult(lastPullResult);
		}
	}
	public abstract class UploadDiagnosisKeysHelper
	{
		public static List<TemporaryExposureKey> createAValidListOfTemporaryExposureKeys(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
		{
			List<TemporaryExposureKey> list = temporaryExposureKeys.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				TemporaryExposureKey val = list[i];
				for (int num = list.Count - 1; num > i; num--)
				{
					TemporaryExposureKey val2 = list[num];
					if (val.get_RollingStart() == val2.get_RollingStart())
					{
						list.RemoveAt(num);
					}
				}
			}
			list.Sort((TemporaryExposureKey x, TemporaryExposureKey y) => y.get_RollingStart().CompareTo(x.get_RollingStart()));
			for (int j = 0; j < list.Count - 1; j++)
			{
				if (list[j + 1].get_RollingStart() != list[j].get_RollingStart().AddDays(-1.0))
				{
					list = list.Take(j + 1).ToList();
					break;
				}
			}
			if (list.Count > 14)
			{
				list = list.Take(14).ToList();
			}
			foreach (TemporaryExposureKey item in list)
			{
				item.set_RollingDuration(new TimeSpan(1, 0, 0, 0));
			}
			return list;
		}

		public static List<TemporaryExposureKey> SetTransmissionRiskLevel(List<TemporaryExposureKey> keys, DateTime symptomsDate)
		{
			DateTimeOffset right = new DateTimeOffset(symptomsDate);
			foreach (TemporaryExposureKey key in keys)
			{
				int days = (key.get_RollingStart() - right).Days;
				for (int num = Conf.DAYS_SINCE_ONSET_FOR_TRANSMISSION_RISK_CALCULATION.Length - 1; num >= 0; num--)
				{
					if (days >= Conf.DAYS_SINCE_ONSET_FOR_TRANSMISSION_RISK_CALCULATION[num].Item1 && days <= Conf.DAYS_SINCE_ONSET_FOR_TRANSMISSION_RISK_CALCULATION[num].Item2)
					{
						key.set_TransmissionRiskLevel((RiskLevel)(num + 1));
						break;
					}
				}
			}
			return keys;
		}
	}
}
namespace NDB.Covid19.Base.AppleGoogle.ExposureNotification.Helpers.FetchExposureKeys
{
	public abstract class StoredValueHelper
	{
		public static readonly string LAST_DOWNLOAD_ZIPS_CALL_UTC_PREF = "LAST_DOWNLOAD_ZIPS_CALL_UTC_PREF";

		private static readonly string MOST_RECENT_15_CALLS_TO_SUBMIT_BATCHES_OLD_TO_NEW_UTC = "MOST_RECENT_15_CALLS_TO_SUBMIT_BATCHES_OLD_TO_NEW_UTC";

		public static bool LastDownloadZipsTooRecent()
		{
			string text = DateTime.UtcNow.AddDays(-123.0).Date.ToString();
			DateTime d = DateTime.Parse(Preferences.Get(LAST_DOWNLOAD_ZIPS_CALL_UTC_PREF, text));
			return DateTime.UtcNow - d < TimeSpan.FromHours(Conf.FETCH_MIN_HOURS_BETWEEN_PULL);
		}

		public static bool TooManySubmitBatches()
		{
			string text = Preferences.Get(MOST_RECENT_15_CALLS_TO_SUBMIT_BATCHES_OLD_TO_NEW_UTC, "");
			if (text == "")
			{
				return false;
			}
			IEnumerable<DateTime> source;
			try
			{
				source = JsonConvert.DeserializeObject<IEnumerable<DateTime>>(text);
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, "Failed at deserialising submitBatchesCallsString in TooManySubmitBatches");
				return false;
			}
			if (source.Count() < 15)
			{
				return false;
			}
			DateTime d = source.First();
			return source.Last() - d < TimeSpan.FromHours(24.0);
		}

		public static void AddSubmitBatchesCall()
		{
			string text = Preferences.Get(MOST_RECENT_15_CALLS_TO_SUBMIT_BATCHES_OLD_TO_NEW_UTC, "");
			List<DateTime> list;
			if (text == "")
			{
				list = new List<DateTime>();
			}
			else
			{
				try
				{
					list = JsonConvert.DeserializeObject<IEnumerable<DateTime>>(text).ToList();
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.ERROR, e, "Failed at deserialising submitBatchesCallsString in AddSubmitBatchesCall");
					list = new List<DateTime>();
				}
			}
			if (list.Count() < 15)
			{
				list.Add(DateTime.UtcNow);
			}
			else
			{
				list = list.Skip(1).ToList();
				list.Add(DateTime.UtcNow);
			}
			try
			{
				string text2 = JsonConvert.SerializeObject((object)list);
				Preferences.Set(MOST_RECENT_15_CALLS_TO_SUBMIT_BATCHES_OLD_TO_NEW_UTC, text2);
			}
			catch (Exception e2)
			{
				LogUtils.LogException(LogSeverity.ERROR, e2, "Failed at saving a JSON serialization of submitBatchesCalls in AddSubmitBatchesCall");
			}
		}

		public static void StoreLastProvidedFiles(IEnumerable<string> localFileUrls)
		{
			string str = $"TEK batch files downloaded at {DateTime.UtcNow} UTC:\n#######\n";
			foreach (string localFileUrl in localFileUrls)
			{
				string str2 = BatchFileHelper.TemporaryExposureKeyExportToPrettyString(BatchFileHelper.ZipToTemporaryExposureKeyExport(BatchFileHelper.UrlToZipArchive(localFileUrl)));
				str = str + str2 + "\n";
			}
			str += "#######";
			DeveloperToolsSingleton.Instance.LastProvidedFiles = str;
		}

		public static void SavePullCallTime()
		{
			string latestPullKeysTimesAndStatuses = DeveloperToolsSingleton.Instance.LatestPullKeysTimesAndStatuses;
			List<Tuple<DateTime, string>> list;
			if (latestPullKeysTimesAndStatuses == "")
			{
				list = new List<Tuple<DateTime, string>>();
			}
			else
			{
				try
				{
					list = JsonConvert.DeserializeObject<IEnumerable<Tuple<DateTime, string>>>(latestPullKeysTimesAndStatuses).ToList();
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.ERROR, e, "Failed at deserialising latestPullKeysTimesAndStatusesString in SavePullCallTime");
					list = new List<Tuple<DateTime, string>>();
				}
			}
			Tuple<DateTime, string> item = new Tuple<DateTime, string>(DateTime.UtcNow, "No status got saved for this call to \"pull keys\"");
			if (list.Count() < 20)
			{
				list.Add(item);
			}
			else
			{
				list = list.Skip(1).ToList();
				list.Add(item);
			}
			try
			{
				string latestPullKeysTimesAndStatuses2 = JsonConvert.SerializeObject((object)list);
				DeveloperToolsSingleton.Instance.LatestPullKeysTimesAndStatuses = latestPullKeysTimesAndStatuses2;
			}
			catch (Exception e2)
			{
				LogUtils.LogException(LogSeverity.ERROR, e2, "Failed at saving a JSON serialization of latestPullKeysTimesAndStatuses in SavePullCallTime");
			}
		}

		public static void SaveStatusOfLastPullCall(string status)
		{
			try
			{
				string latestPullKeysTimesAndStatuses = DeveloperToolsSingleton.Instance.LatestPullKeysTimesAndStatuses;
				if (latestPullKeysTimesAndStatuses == "")
				{
					throw new Exception("latestPullKeysTimesAndStatusesString == emptyString");
				}
				List<Tuple<DateTime, string>> source = JsonConvert.DeserializeObject<IEnumerable<Tuple<DateTime, string>>>(latestPullKeysTimesAndStatuses).ToList();
				Tuple<DateTime, string> tuple = source.Last();
				source = source.Take(source.Count() - 1).ToList();
				Tuple<DateTime, string> item = new Tuple<DateTime, string>(tuple.Item1, status);
				source.Add(item);
				string latestPullKeysTimesAndStatuses2 = JsonConvert.SerializeObject((object)source);
				DeveloperToolsSingleton.Instance.LatestPullKeysTimesAndStatuses = latestPullKeysTimesAndStatuses2;
			}
			catch (Exception e)
			{
				LogUtils.LogException(LogSeverity.ERROR, e, "Failed at adding a status to the latest tuple in SaveStatusOfLastPullCall");
			}
		}
	}
	public class ZipDownloader
	{
		private readonly string CURRENT_DOWNLOAD_DAY_BATCH_PREF = "CURRENT_DOWNLOAD_DAY_BATCH_PREF";

		private readonly string CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_PREF = "CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_PREF";

		private int fetchAttemptsLeft = Conf.FETCH_MAX_ATTEMPTS;

		private readonly ExposureNotificationWebService exposureNotificationWebService = new ExposureNotificationWebService();

		public async Task<IEnumerable<string>> DownloadZips(CancellationToken cancellationToken)
		{
			Preferences.Set(StoredValueHelper.LAST_DOWNLOAD_ZIPS_CALL_UTC_PREF, DateTime.UtcNow.ToString());
			List<string> zips = new List<string>();
			DateTime currentDownloadDayUtc = GetCurrentUtcDayToDownloadKeysFor();
			int currentDownloadDayUtcBatchNumber = Preferences.Get(CURRENT_DOWNLOAD_DAY_BATCH_PREF, 0);
			int iterations = 0;
			while (iterations < 1000 && !(DateTime.UtcNow.Date < currentDownloadDayUtc))
			{
				string arg = currentDownloadDayUtc.ToString("yyyy-MM-dd");
				string dateAndBatch = $"{arg}:{currentDownloadDayUtcBatchNumber}";
				ApiResponse<Stream> apiResponse = await exposureNotificationWebService.GetDiagnosisKeys(dateAndBatch, cancellationToken);
				if (apiResponse == null || (apiResponse.StatusCode != 200 && apiResponse.StatusCode != 204))
				{
					break;
				}
				if (apiResponse.StatusCode == 204)
				{
					if (!(currentDownloadDayUtc < DateTime.UtcNow.Date))
					{
						break;
					}
					currentDownloadDayUtc = currentDownloadDayUtc.AddDays(1.0);
					Preferences.Set(CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_PREF, currentDownloadDayUtc.ToString());
					currentDownloadDayUtcBatchNumber = 0;
					Preferences.Set(CURRENT_DOWNLOAD_DAY_BATCH_PREF, 0);
					continue;
				}
				try
				{
					string text = Path.Combine(FileSystem.get_CacheDirectory(), Guid.NewGuid().ToString() + ".zip");
					FileStream fileStream = File.Create(text);
					apiResponse.Data.CopyTo(fileStream);
					fileStream.Close();
					zips.Add(text);
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.ERROR, e, "Caught an Exception when trying to copy a zip stream in the HTTP response into a temp file in FetchExposureKeyBatchFilesFromServerAsync");
				}
				if (GetFinalBatchForDayBooleanFromHeader(apiResponse))
				{
					if (!(currentDownloadDayUtc < DateTime.UtcNow.Date))
					{
						break;
					}
					currentDownloadDayUtc = currentDownloadDayUtc.AddDays(1.0);
					Preferences.Set(CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_PREF, currentDownloadDayUtc.ToString());
					currentDownloadDayUtcBatchNumber = 0;
					Preferences.Set(CURRENT_DOWNLOAD_DAY_BATCH_PREF, 0);
				}
				else
				{
					currentDownloadDayUtcBatchNumber++;
					Preferences.Set(CURRENT_DOWNLOAD_DAY_BATCH_PREF, currentDownloadDayUtcBatchNumber);
				}
				iterations++;
			}
			return zips;
		}

		private DateTime GetCurrentUtcDayToDownloadKeysFor()
		{
			string text = DateTime.UtcNow.Date.ToString();
			DateTime dateTime = DateTime.Parse(Preferences.Get(CURRENT_DAY_TO_DOWNLOAD_KEYS_FOR_UTC_PREF, text));
			DateTime dateTime2 = DateTime.UtcNow.Date.AddDays(-14.0);
			if (dateTime < dateTime2)
			{
				return dateTime2;
			}
			return dateTime;
		}

		private bool GetFinalBatchForDayBooleanFromHeader(ApiResponse response)
		{
			HttpContentHeaders headers = response.Headers;
			string text = "FinalForTheDay";
			bool result = true;
			if (headers.Contains(text))
			{
				try
				{
					result = bool.Parse(headers.GetValues(text).First());
					return result;
				}
				catch (Exception e)
				{
					LogUtils.LogException(LogSeverity.ERROR, e, "Failed to parse " + text + " in HTTP response headers in GetFinalBatchForDayBooleanFromHeader");
					return result;
				}
			}
			return result;
		}
	}
}
namespace NDB.Covid19.Base.AppleGoogle.ExposureNotification.Helpers.ExposureDetected
{
	public abstract class ExposureInfoJsonHelper
	{
		private class JsonCompatibleExposureInfo
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
namespace NDB.Covid19.Base.AppleGoogle.Config
{
	public class Conf : ISharedConfInterface
	{
		public static readonly int APIVersion = 1;

		public static readonly string BaseUrl = "https://app.smittestop.dk/API/";

		public static int MAX_MESSAGE_RETENTION_TIME_IN_MINUTES = MESSAGE_RETENTION_TIME_IN_MINUTES_LONG;

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

		public static readonly int MINIMUM_RISK_SCORE = 125;

		public static readonly int RISK_SCORE_THRESHOLD_FOR_HIGH_RISK = 512;

		public static readonly string[] SUPPORTED_REGIONS = new string[1]
		{
			"dk"
		};

		public static string DEVELOPERS_CONSOLE_API_KEY = "AIzaSyAMsWk4-Fl5aPHXENTmON0GR4ihxQgRaaU";

		public static string GooglePlayAppLink = "https://play.google.com/store/apps/details?id=com.netcompany.smittestop_exposure_notification";

		public static string HuaweiAppGalleryLink = "https://appgallery.cloud.huawei.com/marketshare/app/C100879437";

		public static string IOSAppstoreAppLink = "itms-apps://itunes.apple.com/app/1516581736";

		public static readonly TimeSpan BACKGROUND_FETCH_REPEAT_INTERVAL_ANDROID = TimeSpan.FromMilliseconds(900000.0);

		public static string AuthorizationHeader => "68iXQyxZOy";

		public static string OAUTH2_CLIENT_ID => OAuthConf.OAUTH2_CLIENT_ID;

		public static string OAUTH2_SCOPE => OAuthConf.OAUTH2_SCOPE;

		public static string OAUTH2_AUTHORISE_URL => OAuthConf.OAUTH2_AUTHORISE_URL;

		public static string OAUTH2_ACCESSTOKEN_URL => OAuthConf.OAUTH2_ACCESSTOKEN_URL;

		public static string OAUTH2_REDIRECT_URL => OAuthConf.OAUTH2_REDIRECT_URL;

		public static string OAUTH2_VERIFY_TOKEN_PUBLIC_KEY => OAuthConf.OAUTH2_VERIFY_TOKEN_PUBLIC_KEY;

		public static string URL_PREFIX => $"{BaseUrl}v{APIVersion}/";

		public static string URL_LOG_MESSAGE => URL_PREFIX + "logging/logMessages";

		public static string URL_PUT_UPLOAD_DIAGNOSIS_KEYS => URL_PREFIX + "diagnostickeys";

		public static string URL_GET_EXPOSURE_CONFIGURATION => URL_PREFIX + "diagnostickeys/exposureconfiguration";

		public static string URL_GET_DIAGNOSIS_KEYS => URL_PREFIX + "diagnostickeys";

		public static bool MOCK_EXPOSURE_CONFIGURATION => false;

		public static int FETCH_RETRY_INTERVAL => 5000;

		public static int FETCH_MAX_ATTEMPTS => 1;

		public static int MOCK_FETCH_BATCHCOUNT => 20;

		public static double FETCH_MIN_HOURS_BETWEEN_PULL => 0.5;

		public static long FETCH_MIN_TICKS_BETWEEN_PULL => 7200 * TICKS_IN_SECOND;

		private static int TICKS_IN_SECOND => 10000000;

		public static int MAX_SUBMIT_BATCHES_PER_24_HOURS => 15;

		public static int MESSAGE_RETENTION_TIME_IN_MINUTES_SHORT => 15;

		public static int MESSAGE_RETENTION_TIME_IN_MINUTES_LONG => 20160;

		public static string DB_NAME => "Smittestop1.db3";

		string ISharedConfInterface.URL_PREFIX => URL_PREFIX;

		int ISharedConfInterface.APIVersion => APIVersion;

		string ISharedConfInterface.DB_NAME => DB_NAME;

		string ISharedConfInterface.URL_LOG_MESSAGE => URL_LOG_MESSAGE;

		string ISharedConfInterface.IOSAppstoreAppLink => IOSAppstoreAppLink;

		string ISharedConfInterface.GooglePlayAppLink => GooglePlayAppLink;

		string ISharedConfInterface.HuaweiAppGalleryLink => HuaweiAppGalleryLink;

		string ISharedConfInterface.AuthorizationHeader => AuthorizationHeader;
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
}
