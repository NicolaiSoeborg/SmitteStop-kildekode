using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Common;
using Android.Gms.Nearby;
using Android.Gms.Nearby.ExposureNotification;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Text.Util;
using Android.Util;
using Android.Views;
using Android.Views.Accessibility;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content.PM;
using AndroidX.Core.Text;
using AndroidX.Fragment.App;
using AndroidX.LocalBroadcastManager.Content;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using AndroidX.Work;
using CommonServiceLocator;
using Google.Android.Material.Tabs;
using I18NPortable;
using Java.Interop;
using Java.Lang;
using Java.Nio.FileNio;
using Java.Util;
using Java.Util.Regex;
using MoreLinq.Extensions;
using NDB.Covid19.Configuration;
using NDB.Covid19.Droid.Services;
using NDB.Covid19.Droid.Utils;
using NDB.Covid19.Droid.Utils.MessagingCenter;
using NDB.Covid19.Droid.Views;
using NDB.Covid19.Droid.Views.AuthenticationFlow;
using NDB.Covid19.Droid.Views.AuthenticationFlow.ErrorActivities;
using NDB.Covid19.Droid.Views.AuthenticationFlow.QuestionnaireAdapters;
using NDB.Covid19.Droid.Views.DiseaseRate;
using NDB.Covid19.Droid.Views.ENDeveloperTools;
using NDB.Covid19.Droid.Views.InfectionStatus;
using NDB.Covid19.Droid.Views.Messages;
using NDB.Covid19.Droid.Views.Settings;
using NDB.Covid19.Droid.Views.Welcome;
using NDB.Covid19.Enums;
using NDB.Covid19.Interfaces;
using NDB.Covid19.Models;
using NDB.Covid19.OAuth2;
using NDB.Covid19.PersistedData;
using NDB.Covid19.Utils;
using NDB.Covid19.ViewModels;
using NDB.Covid19.WebServices.ErrorHandlers;
using PCLCrypto;
using Plugin.CurrentActivity;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.ServiceLocation;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.ExposureNotification;
using Xamarin.ExposureNotifications;
using XamarinShortcutBadger;
using Yort.Ntp;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: ResourceDesigner("NDB.Covid19.Droid.Resource", IsApplication = true)]
[assembly: AssemblyTitle("NDB.Covid19.Droid")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("NDB.Covid19.Droid")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: TargetFramework("MonoAndroid,Version=v10.0", FrameworkDisplayName = "Xamarin.Android v10.0 Support")]
[assembly: AssemblyVersion("1.0.0.0")]
namespace NDB.Covid19.Droid
{
	[Application]
	internal class MainApplication : Application, Application.IActivityLifecycleCallbacks, IJavaObject, IDisposable, IJavaPeerable
	{
		private int _activityReferences;

		private BackgroundNotificationBroadcastReceiver _backgroundNotificationBroadcastReceiver;

		private IntentFilter _filter;

		private FlightModeHandlerBroadcastReceiver _flightModeBroadcastReceiver;

		private bool _isActivityChangingConfigurations;

		private BroadcastReceiver _permissionsBroadcastReceiver;

		public MainApplication(IntPtr handle, JniHandleOwnership transer)
			: base(handle, transer)
		{
		}

		public MainApplication()
		{
		}

		public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
			AccessibilityUtils.AdjustFontScale(activity);
			MessagingCenter.Subscribe<object>(activity, MessagingCenterKeys.KEY_FORCE_UPDATE, delegate
			{
				OnForceUpdate(activity);
			});
		}

		public void OnActivityDestroyed(Activity activity)
		{
			MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.KEY_FORCE_UPDATE);
		}

		public void OnActivityPaused(Activity activity)
		{
		}

		public void OnActivityResumed(Activity activity)
		{
		}

		public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
		{
		}

		public void OnActivityStarted(Activity activity)
		{
			_activityReferences++;
			if (_activityReferences == 1 && !_isActivityChangingConfigurations)
			{
				LogUtils.LogMessage(LogSeverity.INFO, "The user has opened the app", null);
				if (activity is LoadingPageActivity)
				{
					LogUtils.LogMessage(LogSeverity.INFO, "The user is seeing Loading Page", null, LocalPreferencesHelper.GetCorrelationId());
				}
			}
		}

		public void OnActivityStopped(Activity activity)
		{
			_isActivityChangingConfigurations = activity.IsChangingConfigurations;
			if (--_activityReferences == 0 && !_isActivityChangingConfigurations)
			{
				if (activity is QuestionnairePageActivity)
				{
					LogUtils.LogMessage(LogSeverity.INFO, "The user left Questionnaire", null, LocalPreferencesHelper.GetCorrelationId());
				}
				else if (activity is QuestionnaireCountriesSelectionActivity)
				{
					LogUtils.LogMessage(LogSeverity.INFO, "The user left Questionnaire Countries Selection", null, LocalPreferencesHelper.GetCorrelationId());
				}
				else if (activity is LoadingPageActivity)
				{
					LogUtils.LogMessage(LogSeverity.INFO, "The user left Loading Page", null, LocalPreferencesHelper.GetCorrelationId());
				}
				else if (activity is QuestionnaireConfirmLeaveActivity)
				{
					LogUtils.LogMessage(LogSeverity.INFO, "The user left Questionnaire Confirm Leave", null, LocalPreferencesHelper.GetCorrelationId());
				}
				else if (activity is QuestionnairePreShareActivity)
				{
					LogUtils.LogMessage(LogSeverity.INFO, "The user left Questionnaire Pre Share", null, LocalPreferencesHelper.GetCorrelationId());
				}
			}
		}

		private void Init()
		{
			_filter = new IntentFilter();
			_filter.AddAction("android.bluetooth.adapter.action.STATE_CHANGED");
			_filter.AddAction("android.location.PROVIDERS_CHANGED");
			AppDomain.CurrentDomain.UnhandledException += LogUtils.OnUnhandledException;
			AndroidEnvironment.UnhandledExceptionRaiser += OnUnhandledAndroidException;
			DroidDependencyInjectionConfig.Init();
			Platform.Init(this);
			CrossCurrentActivity.Current.Init(this);
			LocalesService.Initialize();
			new MigrationService().Migrate();
			_permissionsBroadcastReceiver = new PermissionsBroadcastReceiver();
			_flightModeBroadcastReceiver = new FlightModeHandlerBroadcastReceiver();
			_backgroundNotificationBroadcastReceiver = new BackgroundNotificationBroadcastReceiver();
			LogUtils.SendAllLogs();
			if (PlayServicesVersionUtils.PlayServicesVersionNumberIsLargeEnough(PackageManager))
			{
				BackgroundFetchScheduler.ScheduleBackgroundFetch();
			}
		}

		private void OnUnhandledAndroidException(object sender, RaiseThrowableEventArgs e)
		{
			if (e?.Exception != null)
			{
				string correlationId = LocalPreferencesHelper.GetCorrelationId();
				if (!string.IsNullOrEmpty(correlationId))
				{
					LogUtils.LogMessage(LogSeverity.INFO, "The user has experienced native Android crash", null, correlationId);
				}
				string contextDescription = "MainApplication.OnUnhandledAndroidException: " + ((!e.Handled) ? "Native unhandled crash" : "Native unhandled exception - not crashing");
				LogUtils.LogException(e.Handled ? LogSeverity.WARNING : LogSeverity.ERROR, e.Exception, contextDescription);
			}
		}

		public override void OnCreate()
		{
			base.OnCreate();
			Init();
			RegisterActivityLifecycleCallbacks(this);
			ManualGarbageCollectionTool();
			RegisterReceiver(_permissionsBroadcastReceiver, _filter);
			RegisterReceiver(_flightModeBroadcastReceiver, new IntentFilter("android.intent.action.AIRPLANE_MODE"));
			LocalBroadcastManager.GetInstance(ApplicationContext).RegisterReceiver(_backgroundNotificationBroadcastReceiver, new IntentFilter("com.netcompany.smittestop_exposure_notification.background_notification"));
		}

		public override void OnTerminate()
		{
			UnregisterReceiver(_permissionsBroadcastReceiver);
			UnregisterReceiver(_flightModeBroadcastReceiver);
			LocalBroadcastManager.GetInstance(ApplicationContext).UnregisterReceiver(_backgroundNotificationBroadcastReceiver);
			base.OnTerminate();
		}

		private void ManualGarbageCollectionTool()
		{
		}

		private void OnForceUpdate(Activity activity)
		{
			activity.RunOnUiThread(delegate
			{
				Intent intent = new Intent(this, typeof(ForceUpdateActivity));
				intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
				StartActivity(intent);
			});
			activity.Finish();
		}
	}
	[GeneratedCode("Xamarin.Android.Build.Tasks", "1.0.0.0")]
	public class Resource
	{
		public class Animation
		{
			public const int abc_fade_in = 2130771968;

			public const int abc_fade_out = 2130771969;

			public const int abc_grow_fade_in_from_bottom = 2130771970;

			public const int abc_popup_enter = 2130771971;

			public const int abc_popup_exit = 2130771972;

			public const int abc_shrink_fade_out_from_bottom = 2130771973;

			public const int abc_slide_in_bottom = 2130771974;

			public const int abc_slide_in_top = 2130771975;

			public const int abc_slide_out_bottom = 2130771976;

			public const int abc_slide_out_top = 2130771977;

			public const int abc_tooltip_enter = 2130771978;

			public const int abc_tooltip_exit = 2130771979;

			public const int background_circle_anim = 2130771980;

			public const int btn_checkbox_to_checked_box_inner_merged_animation = 2130771981;

			public const int btn_checkbox_to_checked_box_outer_merged_animation = 2130771982;

			public const int btn_checkbox_to_checked_icon_null_animation = 2130771983;

			public const int btn_checkbox_to_unchecked_box_inner_merged_animation = 2130771984;

			public const int btn_checkbox_to_unchecked_check_path_merged_animation = 2130771985;

			public const int btn_checkbox_to_unchecked_icon_null_animation = 2130771986;

			public const int btn_radio_to_off_mtrl_dot_group_animation = 2130771987;

			public const int btn_radio_to_off_mtrl_ring_outer_animation = 2130771988;

			public const int btn_radio_to_off_mtrl_ring_outer_path_animation = 2130771989;

			public const int btn_radio_to_on_mtrl_dot_group_animation = 2130771990;

			public const int btn_radio_to_on_mtrl_ring_outer_animation = 2130771991;

			public const int btn_radio_to_on_mtrl_ring_outer_path_animation = 2130771992;

			public const int design_bottom_sheet_slide_in = 2130771993;

			public const int design_bottom_sheet_slide_out = 2130771994;

			public const int design_snackbar_in = 2130771995;

			public const int design_snackbar_out = 2130771996;

			public const int fragment_close_enter = 2130771997;

			public const int fragment_close_exit = 2130771998;

			public const int fragment_fade_enter = 2130771999;

			public const int fragment_fade_exit = 2130772000;

			public const int fragment_fast_out_extra_slow_in = 2130772001;

			public const int fragment_open_enter = 2130772002;

			public const int fragment_open_exit = 2130772003;

			public const int mtrl_bottom_sheet_slide_in = 2130772004;

			public const int mtrl_bottom_sheet_slide_out = 2130772005;

			public const int mtrl_card_lowers_interpolator = 2130772006;

			public const int slide_in_right = 2130772007;

			public const int slide_out_left = 2130772008;

			static Animation()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Animation()
			{
			}
		}

		public class Animator
		{
			public const int design_appbar_state_list_animator = 2130837504;

			public const int design_fab_hide_motion_spec = 2130837505;

			public const int design_fab_show_motion_spec = 2130837506;

			public const int mtrl_btn_state_list_anim = 2130837507;

			public const int mtrl_btn_unelevated_state_list_anim = 2130837508;

			public const int mtrl_card_state_list_anim = 2130837509;

			public const int mtrl_chip_state_list_anim = 2130837510;

			public const int mtrl_extended_fab_change_size_motion_spec = 2130837511;

			public const int mtrl_extended_fab_hide_motion_spec = 2130837512;

			public const int mtrl_extended_fab_show_motion_spec = 2130837513;

			public const int mtrl_extended_fab_state_list_animator = 2130837514;

			public const int mtrl_fab_hide_motion_spec = 2130837515;

			public const int mtrl_fab_show_motion_spec = 2130837516;

			public const int mtrl_fab_transformation_sheet_collapse_spec = 2130837517;

			public const int mtrl_fab_transformation_sheet_expand_spec = 2130837518;

			static Animator()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Animator()
			{
			}
		}

		public class Attribute
		{
			public const int actionBarDivider = 2130903040;

			public const int actionBarItemBackground = 2130903041;

			public const int actionBarPopupTheme = 2130903042;

			public const int actionBarSize = 2130903043;

			public const int actionBarSplitStyle = 2130903044;

			public const int actionBarStyle = 2130903045;

			public const int actionBarTabBarStyle = 2130903046;

			public const int actionBarTabStyle = 2130903047;

			public const int actionBarTabTextStyle = 2130903048;

			public const int actionBarTheme = 2130903049;

			public const int actionBarWidgetTheme = 2130903050;

			public const int actionButtonStyle = 2130903051;

			public const int actionDropDownStyle = 2130903052;

			public const int actionLayout = 2130903053;

			public const int actionMenuTextAppearance = 2130903054;

			public const int actionMenuTextColor = 2130903055;

			public const int actionModeBackground = 2130903056;

			public const int actionModeCloseButtonStyle = 2130903057;

			public const int actionModeCloseDrawable = 2130903058;

			public const int actionModeCopyDrawable = 2130903059;

			public const int actionModeCutDrawable = 2130903060;

			public const int actionModeFindDrawable = 2130903061;

			public const int actionModePasteDrawable = 2130903062;

			public const int actionModePopupWindowStyle = 2130903063;

			public const int actionModeSelectAllDrawable = 2130903064;

			public const int actionModeShareDrawable = 2130903065;

			public const int actionModeSplitBackground = 2130903066;

			public const int actionModeStyle = 2130903067;

			public const int actionModeWebSearchDrawable = 2130903068;

			public const int actionOverflowButtonStyle = 2130903069;

			public const int actionOverflowMenuStyle = 2130903070;

			public const int actionProviderClass = 2130903071;

			public const int actionTextColorAlpha = 2130903072;

			public const int actionViewClass = 2130903073;

			public const int activityChooserViewStyle = 2130903074;

			public const int alertDialogButtonGroupStyle = 2130903075;

			public const int alertDialogCenterButtons = 2130903076;

			public const int alertDialogStyle = 2130903077;

			public const int alertDialogTheme = 2130903078;

			public const int alignContent = 2130903079;

			public const int alignItems = 2130903080;

			public const int allowStacking = 2130903081;

			public const int alpha = 2130903082;

			public const int alphabeticModifiers = 2130903083;

			public const int animationMode = 2130903084;

			public const int appBarLayoutStyle = 2130903085;

			public const int arrowHeadLength = 2130903086;

			public const int arrowShaftLength = 2130903087;

			public const int autoCompleteTextViewStyle = 2130903088;

			public const int autoSizeMaxTextSize = 2130903089;

			public const int autoSizeMinTextSize = 2130903090;

			public const int autoSizePresetSizes = 2130903091;

			public const int autoSizeStepGranularity = 2130903092;

			public const int autoSizeTextType = 2130903093;

			public const int background = 2130903094;

			public const int backgroundColor = 2130903095;

			public const int backgroundInsetBottom = 2130903096;

			public const int backgroundInsetEnd = 2130903097;

			public const int backgroundInsetStart = 2130903098;

			public const int backgroundInsetTop = 2130903099;

			public const int backgroundOverlayColorAlpha = 2130903100;

			public const int backgroundSplit = 2130903101;

			public const int backgroundStacked = 2130903102;

			public const int backgroundTint = 2130903103;

			public const int backgroundTintMode = 2130903104;

			public const int badgeGravity = 2130903105;

			public const int badgeStyle = 2130903106;

			public const int badgeTextColor = 2130903107;

			public const int barLength = 2130903108;

			public const int barrierAllowsGoneWidgets = 2130903109;

			public const int barrierDirection = 2130903110;

			public const int behavior_autoHide = 2130903111;

			public const int behavior_autoShrink = 2130903112;

			public const int behavior_expandedOffset = 2130903113;

			public const int behavior_fitToContents = 2130903114;

			public const int behavior_halfExpandedRatio = 2130903115;

			public const int behavior_hideable = 2130903116;

			public const int behavior_overlapTop = 2130903117;

			public const int behavior_peekHeight = 2130903118;

			public const int behavior_saveFlags = 2130903119;

			public const int behavior_skipCollapsed = 2130903120;

			public const int borderlessButtonStyle = 2130903122;

			public const int borderWidth = 2130903121;

			public const int bottomAppBarStyle = 2130903123;

			public const int bottomNavigationStyle = 2130903124;

			public const int bottomSheetDialogTheme = 2130903125;

			public const int bottomSheetStyle = 2130903126;

			public const int boxBackgroundColor = 2130903127;

			public const int boxBackgroundMode = 2130903128;

			public const int boxCollapsedPaddingTop = 2130903129;

			public const int boxCornerRadiusBottomEnd = 2130903130;

			public const int boxCornerRadiusBottomStart = 2130903131;

			public const int boxCornerRadiusTopEnd = 2130903132;

			public const int boxCornerRadiusTopStart = 2130903133;

			public const int boxStrokeColor = 2130903134;

			public const int boxStrokeWidth = 2130903135;

			public const int boxStrokeWidthFocused = 2130903136;

			public const int buttonBarButtonStyle = 2130903137;

			public const int buttonBarNegativeButtonStyle = 2130903138;

			public const int buttonBarNeutralButtonStyle = 2130903139;

			public const int buttonBarPositiveButtonStyle = 2130903140;

			public const int buttonBarStyle = 2130903141;

			public const int buttonCompat = 2130903142;

			public const int buttonGravity = 2130903143;

			public const int buttonIconDimen = 2130903144;

			public const int buttonPanelSideLayout = 2130903145;

			public const int buttonSize = 2130903146;

			public const int buttonStyle = 2130903147;

			public const int buttonStyleSmall = 2130903148;

			public const int buttonTint = 2130903149;

			public const int buttonTintMode = 2130903150;

			public const int cardBackgroundColor = 2130903151;

			public const int cardCornerRadius = 2130903152;

			public const int cardElevation = 2130903153;

			public const int cardForegroundColor = 2130903154;

			public const int cardMaxElevation = 2130903155;

			public const int cardPreventCornerOverlap = 2130903156;

			public const int cardUseCompatPadding = 2130903157;

			public const int cardViewStyle = 2130903158;

			public const int chainUseRtl = 2130903159;

			public const int checkboxStyle = 2130903160;

			public const int checkedButton = 2130903161;

			public const int checkedChip = 2130903162;

			public const int checkedIcon = 2130903163;

			public const int checkedIconEnabled = 2130903164;

			public const int checkedIconTint = 2130903165;

			public const int checkedIconVisible = 2130903166;

			public const int checkedTextViewStyle = 2130903167;

			public const int chipBackgroundColor = 2130903168;

			public const int chipCornerRadius = 2130903169;

			public const int chipEndPadding = 2130903170;

			public const int chipGroupStyle = 2130903171;

			public const int chipIcon = 2130903172;

			public const int chipIconEnabled = 2130903173;

			public const int chipIconSize = 2130903174;

			public const int chipIconTint = 2130903175;

			public const int chipIconVisible = 2130903176;

			public const int chipMinHeight = 2130903177;

			public const int chipMinTouchTargetSize = 2130903178;

			public const int chipSpacing = 2130903179;

			public const int chipSpacingHorizontal = 2130903180;

			public const int chipSpacingVertical = 2130903181;

			public const int chipStandaloneStyle = 2130903182;

			public const int chipStartPadding = 2130903183;

			public const int chipStrokeColor = 2130903184;

			public const int chipStrokeWidth = 2130903185;

			public const int chipStyle = 2130903186;

			public const int chipSurfaceColor = 2130903187;

			public const int circleCrop = 2130903188;

			public const int closeIcon = 2130903189;

			public const int closeIconEnabled = 2130903190;

			public const int closeIconEndPadding = 2130903191;

			public const int closeIconSize = 2130903192;

			public const int closeIconStartPadding = 2130903193;

			public const int closeIconTint = 2130903194;

			public const int closeIconVisible = 2130903195;

			public const int closeItemLayout = 2130903196;

			public const int collapseContentDescription = 2130903197;

			public const int collapsedTitleGravity = 2130903199;

			public const int collapsedTitleTextAppearance = 2130903200;

			public const int collapseIcon = 2130903198;

			public const int color = 2130903201;

			public const int colorAccent = 2130903202;

			public const int colorBackgroundFloating = 2130903203;

			public const int colorButtonNormal = 2130903204;

			public const int colorControlActivated = 2130903205;

			public const int colorControlHighlight = 2130903206;

			public const int colorControlNormal = 2130903207;

			public const int colorError = 2130903208;

			public const int colorOnBackground = 2130903209;

			public const int colorOnError = 2130903210;

			public const int colorOnPrimary = 2130903211;

			public const int colorOnPrimarySurface = 2130903212;

			public const int colorOnSecondary = 2130903213;

			public const int colorOnSurface = 2130903214;

			public const int colorPrimary = 2130903215;

			public const int colorPrimaryDark = 2130903216;

			public const int colorPrimarySurface = 2130903217;

			public const int colorPrimaryVariant = 2130903218;

			public const int colorScheme = 2130903219;

			public const int colorSecondary = 2130903220;

			public const int colorSecondaryVariant = 2130903221;

			public const int colorSurface = 2130903222;

			public const int colorSwitchThumbNormal = 2130903223;

			public const int commitIcon = 2130903224;

			public const int constraintSet = 2130903225;

			public const int constraint_referenced_ids = 2130903226;

			public const int content = 2130903227;

			public const int contentDescription = 2130903228;

			public const int contentInsetEnd = 2130903229;

			public const int contentInsetEndWithActions = 2130903230;

			public const int contentInsetLeft = 2130903231;

			public const int contentInsetRight = 2130903232;

			public const int contentInsetStart = 2130903233;

			public const int contentInsetStartWithNavigation = 2130903234;

			public const int contentPadding = 2130903235;

			public const int contentPaddingBottom = 2130903236;

			public const int contentPaddingLeft = 2130903237;

			public const int contentPaddingRight = 2130903238;

			public const int contentPaddingTop = 2130903239;

			public const int contentScrim = 2130903240;

			public const int controlBackground = 2130903241;

			public const int coordinatorLayoutStyle = 2130903242;

			public const int cornerFamily = 2130903243;

			public const int cornerFamilyBottomLeft = 2130903244;

			public const int cornerFamilyBottomRight = 2130903245;

			public const int cornerFamilyTopLeft = 2130903246;

			public const int cornerFamilyTopRight = 2130903247;

			public const int cornerRadius = 2130903248;

			public const int cornerSize = 2130903249;

			public const int cornerSizeBottomLeft = 2130903250;

			public const int cornerSizeBottomRight = 2130903251;

			public const int cornerSizeTopLeft = 2130903252;

			public const int cornerSizeTopRight = 2130903253;

			public const int counterEnabled = 2130903254;

			public const int counterMaxLength = 2130903255;

			public const int counterOverflowTextAppearance = 2130903256;

			public const int counterOverflowTextColor = 2130903257;

			public const int counterTextAppearance = 2130903258;

			public const int counterTextColor = 2130903259;

			public const int customNavigationLayout = 2130903260;

			public const int dayInvalidStyle = 2130903261;

			public const int daySelectedStyle = 2130903262;

			public const int dayStyle = 2130903263;

			public const int dayTodayStyle = 2130903264;

			public const int defaultQueryHint = 2130903265;

			public const int dialogCornerRadius = 2130903266;

			public const int dialogPreferredPadding = 2130903267;

			public const int dialogTheme = 2130903268;

			public const int displayOptions = 2130903269;

			public const int divider = 2130903270;

			public const int dividerDrawable = 2130903271;

			public const int dividerDrawableHorizontal = 2130903272;

			public const int dividerDrawableVertical = 2130903273;

			public const int dividerHorizontal = 2130903274;

			public const int dividerPadding = 2130903275;

			public const int dividerVertical = 2130903276;

			public const int drawableBottomCompat = 2130903277;

			public const int drawableEndCompat = 2130903278;

			public const int drawableLeftCompat = 2130903279;

			public const int drawableRightCompat = 2130903280;

			public const int drawableSize = 2130903281;

			public const int drawableStartCompat = 2130903282;

			public const int drawableTint = 2130903283;

			public const int drawableTintMode = 2130903284;

			public const int drawableTopCompat = 2130903285;

			public const int drawerArrowStyle = 2130903286;

			public const int dropdownListPreferredItemHeight = 2130903288;

			public const int dropDownListViewStyle = 2130903287;

			public const int editTextBackground = 2130903289;

			public const int editTextColor = 2130903290;

			public const int editTextStyle = 2130903291;

			public const int elevation = 2130903292;

			public const int elevationOverlayColor = 2130903293;

			public const int elevationOverlayEnabled = 2130903294;

			public const int emptyVisibility = 2130903295;

			public const int endIconCheckable = 2130903296;

			public const int endIconContentDescription = 2130903297;

			public const int endIconDrawable = 2130903298;

			public const int endIconMode = 2130903299;

			public const int endIconTint = 2130903300;

			public const int endIconTintMode = 2130903301;

			public const int enforceMaterialTheme = 2130903302;

			public const int enforceTextAppearance = 2130903303;

			public const int ensureMinTouchTargetSize = 2130903304;

			public const int errorEnabled = 2130903305;

			public const int errorIconDrawable = 2130903306;

			public const int errorIconTint = 2130903307;

			public const int errorIconTintMode = 2130903308;

			public const int errorTextAppearance = 2130903309;

			public const int errorTextColor = 2130903310;

			public const int expandActivityOverflowButtonDrawable = 2130903311;

			public const int expanded = 2130903312;

			public const int expandedTitleGravity = 2130903313;

			public const int expandedTitleMargin = 2130903314;

			public const int expandedTitleMarginBottom = 2130903315;

			public const int expandedTitleMarginEnd = 2130903316;

			public const int expandedTitleMarginStart = 2130903317;

			public const int expandedTitleMarginTop = 2130903318;

			public const int expandedTitleTextAppearance = 2130903319;

			public const int extendedFloatingActionButtonStyle = 2130903321;

			public const int extendMotionSpec = 2130903320;

			public const int fabAlignmentMode = 2130903322;

			public const int fabAnimationMode = 2130903323;

			public const int fabCradleMargin = 2130903324;

			public const int fabCradleRoundedCornerRadius = 2130903325;

			public const int fabCradleVerticalOffset = 2130903326;

			public const int fabCustomSize = 2130903327;

			public const int fabSize = 2130903328;

			public const int fastScrollEnabled = 2130903329;

			public const int fastScrollHorizontalThumbDrawable = 2130903330;

			public const int fastScrollHorizontalTrackDrawable = 2130903331;

			public const int fastScrollVerticalThumbDrawable = 2130903332;

			public const int fastScrollVerticalTrackDrawable = 2130903333;

			public const int firstBaselineToTopHeight = 2130903334;

			public const int flexDirection = 2130903335;

			public const int flexWrap = 2130903336;

			public const int floatingActionButtonStyle = 2130903337;

			public const int font = 2130903338;

			public const int fontFamily = 2130903339;

			public const int fontProviderAuthority = 2130903340;

			public const int fontProviderCerts = 2130903341;

			public const int fontProviderFetchStrategy = 2130903342;

			public const int fontProviderFetchTimeout = 2130903343;

			public const int fontProviderPackage = 2130903344;

			public const int fontProviderQuery = 2130903345;

			public const int fontStyle = 2130903346;

			public const int fontVariationSettings = 2130903347;

			public const int fontWeight = 2130903348;

			public const int foregroundInsidePadding = 2130903349;

			public const int gapBetweenBars = 2130903350;

			public const int goIcon = 2130903351;

			public const int headerLayout = 2130903352;

			public const int height = 2130903353;

			public const int helperText = 2130903354;

			public const int helperTextEnabled = 2130903355;

			public const int helperTextTextAppearance = 2130903356;

			public const int helperTextTextColor = 2130903357;

			public const int hideMotionSpec = 2130903358;

			public const int hideOnContentScroll = 2130903359;

			public const int hideOnScroll = 2130903360;

			public const int hintAnimationEnabled = 2130903361;

			public const int hintEnabled = 2130903362;

			public const int hintTextAppearance = 2130903363;

			public const int hintTextColor = 2130903364;

			public const int homeAsUpIndicator = 2130903365;

			public const int homeLayout = 2130903366;

			public const int hoveredFocusedTranslationZ = 2130903367;

			public const int icon = 2130903368;

			public const int iconEndPadding = 2130903369;

			public const int iconGravity = 2130903370;

			public const int iconifiedByDefault = 2130903376;

			public const int iconPadding = 2130903371;

			public const int iconSize = 2130903372;

			public const int iconStartPadding = 2130903373;

			public const int iconTint = 2130903374;

			public const int iconTintMode = 2130903375;

			public const int imageAspectRatio = 2130903377;

			public const int imageAspectRatioAdjust = 2130903378;

			public const int imageButtonStyle = 2130903379;

			public const int indeterminateProgressStyle = 2130903380;

			public const int initialActivityCount = 2130903381;

			public const int insetForeground = 2130903382;

			public const int isLightTheme = 2130903383;

			public const int isMaterialTheme = 2130903384;

			public const int itemBackground = 2130903385;

			public const int itemFillColor = 2130903386;

			public const int itemHorizontalPadding = 2130903387;

			public const int itemHorizontalTranslationEnabled = 2130903388;

			public const int itemIconPadding = 2130903389;

			public const int itemIconSize = 2130903390;

			public const int itemIconTint = 2130903391;

			public const int itemMaxLines = 2130903392;

			public const int itemPadding = 2130903393;

			public const int itemRippleColor = 2130903394;

			public const int itemShapeAppearance = 2130903395;

			public const int itemShapeAppearanceOverlay = 2130903396;

			public const int itemShapeFillColor = 2130903397;

			public const int itemShapeInsetBottom = 2130903398;

			public const int itemShapeInsetEnd = 2130903399;

			public const int itemShapeInsetStart = 2130903400;

			public const int itemShapeInsetTop = 2130903401;

			public const int itemSpacing = 2130903402;

			public const int itemStrokeColor = 2130903403;

			public const int itemStrokeWidth = 2130903404;

			public const int itemTextAppearance = 2130903405;

			public const int itemTextAppearanceActive = 2130903406;

			public const int itemTextAppearanceInactive = 2130903407;

			public const int itemTextColor = 2130903408;

			public const int justifyContent = 2130903409;

			public const int keylines = 2130903410;

			public const int labelVisibilityMode = 2130903411;

			public const int lastBaselineToBottomHeight = 2130903412;

			public const int layout = 2130903413;

			public const int layoutManager = 2130903414;

			public const int layout_alignSelf = 2130903415;

			public const int layout_anchor = 2130903416;

			public const int layout_anchorGravity = 2130903417;

			public const int layout_behavior = 2130903418;

			public const int layout_collapseMode = 2130903419;

			public const int layout_collapseParallaxMultiplier = 2130903420;

			public const int layout_constrainedHeight = 2130903421;

			public const int layout_constrainedWidth = 2130903422;

			public const int layout_constraintBaseline_creator = 2130903423;

			public const int layout_constraintBaseline_toBaselineOf = 2130903424;

			public const int layout_constraintBottom_creator = 2130903425;

			public const int layout_constraintBottom_toBottomOf = 2130903426;

			public const int layout_constraintBottom_toTopOf = 2130903427;

			public const int layout_constraintCircle = 2130903428;

			public const int layout_constraintCircleAngle = 2130903429;

			public const int layout_constraintCircleRadius = 2130903430;

			public const int layout_constraintDimensionRatio = 2130903431;

			public const int layout_constraintEnd_toEndOf = 2130903432;

			public const int layout_constraintEnd_toStartOf = 2130903433;

			public const int layout_constraintGuide_begin = 2130903434;

			public const int layout_constraintGuide_end = 2130903435;

			public const int layout_constraintGuide_percent = 2130903436;

			public const int layout_constraintHeight_default = 2130903437;

			public const int layout_constraintHeight_max = 2130903438;

			public const int layout_constraintHeight_min = 2130903439;

			public const int layout_constraintHeight_percent = 2130903440;

			public const int layout_constraintHorizontal_bias = 2130903441;

			public const int layout_constraintHorizontal_chainStyle = 2130903442;

			public const int layout_constraintHorizontal_weight = 2130903443;

			public const int layout_constraintLeft_creator = 2130903444;

			public const int layout_constraintLeft_toLeftOf = 2130903445;

			public const int layout_constraintLeft_toRightOf = 2130903446;

			public const int layout_constraintRight_creator = 2130903447;

			public const int layout_constraintRight_toLeftOf = 2130903448;

			public const int layout_constraintRight_toRightOf = 2130903449;

			public const int layout_constraintStart_toEndOf = 2130903450;

			public const int layout_constraintStart_toStartOf = 2130903451;

			public const int layout_constraintTop_creator = 2130903452;

			public const int layout_constraintTop_toBottomOf = 2130903453;

			public const int layout_constraintTop_toTopOf = 2130903454;

			public const int layout_constraintVertical_bias = 2130903455;

			public const int layout_constraintVertical_chainStyle = 2130903456;

			public const int layout_constraintVertical_weight = 2130903457;

			public const int layout_constraintWidth_default = 2130903458;

			public const int layout_constraintWidth_max = 2130903459;

			public const int layout_constraintWidth_min = 2130903460;

			public const int layout_constraintWidth_percent = 2130903461;

			public const int layout_dodgeInsetEdges = 2130903462;

			public const int layout_editor_absoluteX = 2130903463;

			public const int layout_editor_absoluteY = 2130903464;

			public const int layout_flexBasisPercent = 2130903465;

			public const int layout_flexGrow = 2130903466;

			public const int layout_flexShrink = 2130903467;

			public const int layout_goneMarginBottom = 2130903468;

			public const int layout_goneMarginEnd = 2130903469;

			public const int layout_goneMarginLeft = 2130903470;

			public const int layout_goneMarginRight = 2130903471;

			public const int layout_goneMarginStart = 2130903472;

			public const int layout_goneMarginTop = 2130903473;

			public const int layout_insetEdge = 2130903474;

			public const int layout_keyline = 2130903475;

			public const int layout_maxHeight = 2130903476;

			public const int layout_maxWidth = 2130903477;

			public const int layout_minHeight = 2130903478;

			public const int layout_minWidth = 2130903479;

			public const int layout_optimizationLevel = 2130903480;

			public const int layout_order = 2130903481;

			public const int layout_scrollFlags = 2130903482;

			public const int layout_scrollInterpolator = 2130903483;

			public const int layout_wrapBefore = 2130903484;

			public const int liftOnScroll = 2130903485;

			public const int liftOnScrollTargetViewId = 2130903486;

			public const int lineHeight = 2130903487;

			public const int lineSpacing = 2130903488;

			public const int listChoiceBackgroundIndicator = 2130903489;

			public const int listChoiceIndicatorMultipleAnimated = 2130903490;

			public const int listChoiceIndicatorSingleAnimated = 2130903491;

			public const int listDividerAlertDialog = 2130903492;

			public const int listItemLayout = 2130903493;

			public const int listLayout = 2130903494;

			public const int listMenuViewStyle = 2130903495;

			public const int listPopupWindowStyle = 2130903496;

			public const int listPreferredItemHeight = 2130903497;

			public const int listPreferredItemHeightLarge = 2130903498;

			public const int listPreferredItemHeightSmall = 2130903499;

			public const int listPreferredItemPaddingEnd = 2130903500;

			public const int listPreferredItemPaddingLeft = 2130903501;

			public const int listPreferredItemPaddingRight = 2130903502;

			public const int listPreferredItemPaddingStart = 2130903503;

			public const int logo = 2130903504;

			public const int logoDescription = 2130903505;

			public const int materialAlertDialogBodyTextStyle = 2130903506;

			public const int materialAlertDialogTheme = 2130903507;

			public const int materialAlertDialogTitleIconStyle = 2130903508;

			public const int materialAlertDialogTitlePanelStyle = 2130903509;

			public const int materialAlertDialogTitleTextStyle = 2130903510;

			public const int materialButtonOutlinedStyle = 2130903511;

			public const int materialButtonStyle = 2130903512;

			public const int materialButtonToggleGroupStyle = 2130903513;

			public const int materialCalendarDay = 2130903514;

			public const int materialCalendarFullscreenTheme = 2130903515;

			public const int materialCalendarHeaderConfirmButton = 2130903516;

			public const int materialCalendarHeaderDivider = 2130903517;

			public const int materialCalendarHeaderLayout = 2130903518;

			public const int materialCalendarHeaderSelection = 2130903519;

			public const int materialCalendarHeaderTitle = 2130903520;

			public const int materialCalendarHeaderToggleButton = 2130903521;

			public const int materialCalendarStyle = 2130903522;

			public const int materialCalendarTheme = 2130903523;

			public const int materialCardViewStyle = 2130903524;

			public const int materialThemeOverlay = 2130903525;

			public const int maxActionInlineWidth = 2130903526;

			public const int maxButtonHeight = 2130903527;

			public const int maxCharacterCount = 2130903528;

			public const int maxImageSize = 2130903529;

			public const int maxLine = 2130903530;

			public const int measureWithLargestChild = 2130903531;

			public const int menu = 2130903532;

			public const int minTouchTargetSize = 2130903533;

			public const int multiChoiceItemLayout = 2130903534;

			public const int navigationContentDescription = 2130903535;

			public const int navigationIcon = 2130903536;

			public const int navigationMode = 2130903537;

			public const int navigationViewStyle = 2130903538;

			public const int number = 2130903539;

			public const int numericModifiers = 2130903540;

			public const int overlapAnchor = 2130903541;

			public const int paddingBottomNoButtons = 2130903542;

			public const int paddingEnd = 2130903543;

			public const int paddingStart = 2130903544;

			public const int paddingTopNoTitle = 2130903545;

			public const int panelBackground = 2130903546;

			public const int panelMenuListTheme = 2130903547;

			public const int panelMenuListWidth = 2130903548;

			public const int passwordToggleContentDescription = 2130903549;

			public const int passwordToggleDrawable = 2130903550;

			public const int passwordToggleEnabled = 2130903551;

			public const int passwordToggleTint = 2130903552;

			public const int passwordToggleTintMode = 2130903553;

			public const int popupMenuBackground = 2130903554;

			public const int popupMenuStyle = 2130903555;

			public const int popupTheme = 2130903556;

			public const int popupWindowStyle = 2130903557;

			public const int preserveIconSpacing = 2130903558;

			public const int pressedTranslationZ = 2130903559;

			public const int progressBarPadding = 2130903560;

			public const int progressBarStyle = 2130903561;

			public const int queryBackground = 2130903562;

			public const int queryHint = 2130903563;

			public const int radioButtonStyle = 2130903564;

			public const int rangeFillColor = 2130903565;

			public const int ratingBarStyle = 2130903566;

			public const int ratingBarStyleIndicator = 2130903567;

			public const int ratingBarStyleSmall = 2130903568;

			public const int recyclerViewStyle = 2130903569;

			public const int reverseLayout = 2130903570;

			public const int rippleColor = 2130903571;

			public const int scopeUris = 2130903572;

			public const int scrimAnimationDuration = 2130903573;

			public const int scrimBackground = 2130903574;

			public const int scrimVisibleHeightTrigger = 2130903575;

			public const int searchHintIcon = 2130903576;

			public const int searchIcon = 2130903577;

			public const int searchViewStyle = 2130903578;

			public const int seekBarStyle = 2130903579;

			public const int selectableItemBackground = 2130903580;

			public const int selectableItemBackgroundBorderless = 2130903581;

			public const int shapeAppearance = 2130903582;

			public const int shapeAppearanceLargeComponent = 2130903583;

			public const int shapeAppearanceMediumComponent = 2130903584;

			public const int shapeAppearanceOverlay = 2130903585;

			public const int shapeAppearanceSmallComponent = 2130903586;

			public const int showAsAction = 2130903587;

			public const int showDivider = 2130903588;

			public const int showDividerHorizontal = 2130903589;

			public const int showDividers = 2130903591;

			public const int showDividerVertical = 2130903590;

			public const int showMotionSpec = 2130903592;

			public const int showText = 2130903593;

			public const int showTitle = 2130903594;

			public const int shrinkMotionSpec = 2130903595;

			public const int singleChoiceItemLayout = 2130903596;

			public const int singleLine = 2130903597;

			public const int singleSelection = 2130903598;

			public const int snackbarButtonStyle = 2130903599;

			public const int snackbarStyle = 2130903600;

			public const int spanCount = 2130903601;

			public const int spinBars = 2130903602;

			public const int spinnerDropDownItemStyle = 2130903603;

			public const int spinnerStyle = 2130903604;

			public const int splitTrack = 2130903605;

			public const int srcCompat = 2130903606;

			public const int stackFromEnd = 2130903607;

			public const int startIconCheckable = 2130903608;

			public const int startIconContentDescription = 2130903609;

			public const int startIconDrawable = 2130903610;

			public const int startIconTint = 2130903611;

			public const int startIconTintMode = 2130903612;

			public const int state_above_anchor = 2130903613;

			public const int state_collapsed = 2130903614;

			public const int state_collapsible = 2130903615;

			public const int state_dragged = 2130903616;

			public const int state_liftable = 2130903617;

			public const int state_lifted = 2130903618;

			public const int statusBarBackground = 2130903619;

			public const int statusBarForeground = 2130903620;

			public const int statusBarScrim = 2130903621;

			public const int strokeColor = 2130903622;

			public const int strokeWidth = 2130903623;

			public const int subMenuArrow = 2130903624;

			public const int submitBackground = 2130903625;

			public const int subtitle = 2130903626;

			public const int subtitleTextAppearance = 2130903627;

			public const int subtitleTextColor = 2130903628;

			public const int subtitleTextStyle = 2130903629;

			public const int suggestionRowLayout = 2130903630;

			public const int switchMinWidth = 2130903631;

			public const int switchPadding = 2130903632;

			public const int switchStyle = 2130903633;

			public const int switchTextAppearance = 2130903634;

			public const int tabBackground = 2130903635;

			public const int tabContentStart = 2130903636;

			public const int tabGravity = 2130903637;

			public const int tabIconTint = 2130903638;

			public const int tabIconTintMode = 2130903639;

			public const int tabIndicator = 2130903640;

			public const int tabIndicatorAnimationDuration = 2130903641;

			public const int tabIndicatorColor = 2130903642;

			public const int tabIndicatorFullWidth = 2130903643;

			public const int tabIndicatorGravity = 2130903644;

			public const int tabIndicatorHeight = 2130903645;

			public const int tabInlineLabel = 2130903646;

			public const int tabMaxWidth = 2130903647;

			public const int tabMinWidth = 2130903648;

			public const int tabMode = 2130903649;

			public const int tabPadding = 2130903650;

			public const int tabPaddingBottom = 2130903651;

			public const int tabPaddingEnd = 2130903652;

			public const int tabPaddingStart = 2130903653;

			public const int tabPaddingTop = 2130903654;

			public const int tabRippleColor = 2130903655;

			public const int tabSelectedTextColor = 2130903656;

			public const int tabStyle = 2130903657;

			public const int tabTextAppearance = 2130903658;

			public const int tabTextColor = 2130903659;

			public const int tabUnboundedRipple = 2130903660;

			public const int textAllCaps = 2130903661;

			public const int textAppearanceBody1 = 2130903662;

			public const int textAppearanceBody2 = 2130903663;

			public const int textAppearanceButton = 2130903664;

			public const int textAppearanceCaption = 2130903665;

			public const int textAppearanceHeadline1 = 2130903666;

			public const int textAppearanceHeadline2 = 2130903667;

			public const int textAppearanceHeadline3 = 2130903668;

			public const int textAppearanceHeadline4 = 2130903669;

			public const int textAppearanceHeadline5 = 2130903670;

			public const int textAppearanceHeadline6 = 2130903671;

			public const int textAppearanceLargePopupMenu = 2130903672;

			public const int textAppearanceLineHeightEnabled = 2130903673;

			public const int textAppearanceListItem = 2130903674;

			public const int textAppearanceListItemSecondary = 2130903675;

			public const int textAppearanceListItemSmall = 2130903676;

			public const int textAppearanceOverline = 2130903677;

			public const int textAppearancePopupMenuHeader = 2130903678;

			public const int textAppearanceSearchResultSubtitle = 2130903679;

			public const int textAppearanceSearchResultTitle = 2130903680;

			public const int textAppearanceSmallPopupMenu = 2130903681;

			public const int textAppearanceSubtitle1 = 2130903682;

			public const int textAppearanceSubtitle2 = 2130903683;

			public const int textColorAlertDialogListItem = 2130903684;

			public const int textColorSearchUrl = 2130903685;

			public const int textEndPadding = 2130903686;

			public const int textInputStyle = 2130903687;

			public const int textLocale = 2130903688;

			public const int textStartPadding = 2130903689;

			public const int theme = 2130903690;

			public const int themeLineHeight = 2130903691;

			public const int thickness = 2130903692;

			public const int thumbTextPadding = 2130903693;

			public const int thumbTint = 2130903694;

			public const int thumbTintMode = 2130903695;

			public const int tickMark = 2130903696;

			public const int tickMarkTint = 2130903697;

			public const int tickMarkTintMode = 2130903698;

			public const int tint = 2130903699;

			public const int tintMode = 2130903700;

			public const int title = 2130903701;

			public const int titleEnabled = 2130903702;

			public const int titleMargin = 2130903703;

			public const int titleMarginBottom = 2130903704;

			public const int titleMarginEnd = 2130903705;

			public const int titleMargins = 2130903708;

			public const int titleMarginStart = 2130903706;

			public const int titleMarginTop = 2130903707;

			public const int titleTextAppearance = 2130903709;

			public const int titleTextColor = 2130903710;

			public const int titleTextStyle = 2130903711;

			public const int toolbarId = 2130903712;

			public const int toolbarNavigationButtonStyle = 2130903713;

			public const int toolbarStyle = 2130903714;

			public const int tooltipForegroundColor = 2130903715;

			public const int tooltipFrameBackground = 2130903716;

			public const int tooltipText = 2130903717;

			public const int track = 2130903718;

			public const int trackTint = 2130903719;

			public const int trackTintMode = 2130903720;

			public const int ttcIndex = 2130903721;

			public const int useCompatPadding = 2130903722;

			public const int useMaterialThemeColors = 2130903723;

			public const int viewInflaterClass = 2130903724;

			public const int voiceIcon = 2130903725;

			public const int windowActionBar = 2130903726;

			public const int windowActionBarOverlay = 2130903727;

			public const int windowActionModeOverlay = 2130903728;

			public const int windowFixedHeightMajor = 2130903729;

			public const int windowFixedHeightMinor = 2130903730;

			public const int windowFixedWidthMajor = 2130903731;

			public const int windowFixedWidthMinor = 2130903732;

			public const int windowMinWidthMajor = 2130903733;

			public const int windowMinWidthMinor = 2130903734;

			public const int windowNoTitle = 2130903735;

			public const int yearSelectedStyle = 2130903736;

			public const int yearStyle = 2130903737;

			public const int yearTodayStyle = 2130903738;

			static Attribute()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Attribute()
			{
			}
		}

		public class Boolean
		{
			public const int abc_action_bar_embed_tabs = 2130968576;

			public const int abc_allow_stacked_button_bar = 2130968577;

			public const int abc_config_actionMenuItemAllCaps = 2130968578;

			public const int enable_system_alarm_service_default = 2130968579;

			public const int enable_system_foreground_service_default = 2130968580;

			public const int enable_system_job_service_default = 2130968581;

			public const int mtrl_btn_textappearance_all_caps = 2130968582;

			public const int workmanager_test_configuration = 2130968583;

			static Boolean()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Boolean()
			{
			}
		}

		public class Color
		{
			public const int abc_background_cache_hint_selector_material_dark = 2131034112;

			public const int abc_background_cache_hint_selector_material_light = 2131034113;

			public const int abc_btn_colored_borderless_text_material = 2131034114;

			public const int abc_btn_colored_text_material = 2131034115;

			public const int abc_color_highlight_material = 2131034116;

			public const int abc_hint_foreground_material_dark = 2131034117;

			public const int abc_hint_foreground_material_light = 2131034118;

			public const int abc_input_method_navigation_guard = 2131034119;

			public const int abc_primary_text_disable_only_material_dark = 2131034120;

			public const int abc_primary_text_disable_only_material_light = 2131034121;

			public const int abc_primary_text_material_dark = 2131034122;

			public const int abc_primary_text_material_light = 2131034123;

			public const int abc_search_url_text = 2131034124;

			public const int abc_search_url_text_normal = 2131034125;

			public const int abc_search_url_text_pressed = 2131034126;

			public const int abc_search_url_text_selected = 2131034127;

			public const int abc_secondary_text_material_dark = 2131034128;

			public const int abc_secondary_text_material_light = 2131034129;

			public const int abc_tint_btn_checkable = 2131034130;

			public const int abc_tint_default = 2131034131;

			public const int abc_tint_edittext = 2131034132;

			public const int abc_tint_seek_thumb = 2131034133;

			public const int abc_tint_spinner = 2131034134;

			public const int abc_tint_switch_track = 2131034135;

			public const int accent_material_dark = 2131034136;

			public const int accent_material_light = 2131034137;

			public const int activated_color = 2131034138;

			public const int backgroundColor = 2131034139;

			public const int background_floating_material_dark = 2131034140;

			public const int background_floating_material_light = 2131034141;

			public const int background_material_dark = 2131034142;

			public const int background_material_light = 2131034143;

			public const int bright_foreground_disabled_material_dark = 2131034144;

			public const int bright_foreground_disabled_material_light = 2131034145;

			public const int bright_foreground_inverse_material_dark = 2131034146;

			public const int bright_foreground_inverse_material_light = 2131034147;

			public const int bright_foreground_material_dark = 2131034148;

			public const int bright_foreground_material_light = 2131034149;

			public const int browser_actions_bg_grey = 2131034150;

			public const int browser_actions_divider_color = 2131034151;

			public const int browser_actions_text_color = 2131034152;

			public const int browser_actions_title_color = 2131034153;

			public const int buttonOnGreen = 2131034154;

			public const int button_material_dark = 2131034155;

			public const int button_material_light = 2131034156;

			public const int cardview_dark_background = 2131034157;

			public const int cardview_light_background = 2131034158;

			public const int cardview_shadow_end_color = 2131034159;

			public const int cardview_shadow_start_color = 2131034160;

			public const int checkbox_themeable_attribute_color = 2131034161;

			public const int colorAccent = 2131034162;

			public const int colorControlActivated = 2131034163;

			public const int colorPrimary = 2131034164;

			public const int colorPrimaryDark = 2131034165;

			public const int colorPrimaryMedium = 2131034166;

			public const int common_google_signin_btn_text_dark = 2131034167;

			public const int common_google_signin_btn_text_dark_default = 2131034168;

			public const int common_google_signin_btn_text_dark_disabled = 2131034169;

			public const int common_google_signin_btn_text_dark_focused = 2131034170;

			public const int common_google_signin_btn_text_dark_pressed = 2131034171;

			public const int common_google_signin_btn_text_light = 2131034172;

			public const int common_google_signin_btn_text_light_default = 2131034173;

			public const int common_google_signin_btn_text_light_disabled = 2131034174;

			public const int common_google_signin_btn_text_light_focused = 2131034175;

			public const int common_google_signin_btn_text_light_pressed = 2131034176;

			public const int common_google_signin_btn_tint = 2131034177;

			public const int counterExplainText = 2131034178;

			public const int counterLayoutBackgroundColor = 2131034179;

			public const int design_bottom_navigation_shadow_color = 2131034180;

			public const int design_box_stroke_color = 2131034181;

			public const int design_dark_default_color_background = 2131034182;

			public const int design_dark_default_color_error = 2131034183;

			public const int design_dark_default_color_on_background = 2131034184;

			public const int design_dark_default_color_on_error = 2131034185;

			public const int design_dark_default_color_on_primary = 2131034186;

			public const int design_dark_default_color_on_secondary = 2131034187;

			public const int design_dark_default_color_on_surface = 2131034188;

			public const int design_dark_default_color_primary = 2131034189;

			public const int design_dark_default_color_primary_dark = 2131034190;

			public const int design_dark_default_color_primary_variant = 2131034191;

			public const int design_dark_default_color_secondary = 2131034192;

			public const int design_dark_default_color_secondary_variant = 2131034193;

			public const int design_dark_default_color_surface = 2131034194;

			public const int design_default_color_background = 2131034195;

			public const int design_default_color_error = 2131034196;

			public const int design_default_color_on_background = 2131034197;

			public const int design_default_color_on_error = 2131034198;

			public const int design_default_color_on_primary = 2131034199;

			public const int design_default_color_on_secondary = 2131034200;

			public const int design_default_color_on_surface = 2131034201;

			public const int design_default_color_primary = 2131034202;

			public const int design_default_color_primary_dark = 2131034203;

			public const int design_default_color_primary_variant = 2131034204;

			public const int design_default_color_secondary = 2131034205;

			public const int design_default_color_secondary_variant = 2131034206;

			public const int design_default_color_surface = 2131034207;

			public const int design_error = 2131034208;

			public const int design_fab_shadow_end_color = 2131034209;

			public const int design_fab_shadow_mid_color = 2131034210;

			public const int design_fab_shadow_start_color = 2131034211;

			public const int design_fab_stroke_end_inner_color = 2131034212;

			public const int design_fab_stroke_end_outer_color = 2131034213;

			public const int design_fab_stroke_top_inner_color = 2131034214;

			public const int design_fab_stroke_top_outer_color = 2131034215;

			public const int design_icon_tint = 2131034216;

			public const int design_snackbar_background_color = 2131034217;

			public const int dim_foreground_disabled_material_dark = 2131034218;

			public const int dim_foreground_disabled_material_light = 2131034219;

			public const int dim_foreground_material_dark = 2131034220;

			public const int dim_foreground_material_light = 2131034221;

			public const int diseaseRateCounterLayoutBackground = 2131034222;

			public const int divider = 2131034223;

			public const int dividerBlue = 2131034224;

			public const int dividerWhite = 2131034225;

			public const int errorColor = 2131034226;

			public const int error_color_material_dark = 2131034227;

			public const int error_color_material_light = 2131034228;

			public const int foreground_material_dark = 2131034229;

			public const int foreground_material_light = 2131034230;

			public const int greyedOut = 2131034231;

			public const int highlighted_text_material_dark = 2131034232;

			public const int highlighted_text_material_light = 2131034233;

			public const int ic_launcher_background = 2131034234;

			public const int infectionStatusBackgroundGreen = 2131034235;

			public const int infectionStatusBackgroundRed = 2131034236;

			public const int infectionStatusButtonMark = 2131034237;

			public const int infectionStatusButtonOffRed = 2131034238;

			public const int infectionStatusButtonOnGreen = 2131034239;

			public const int infectionStatusLayoutButtonArrowBackground = 2131034240;

			public const int infectionStatusLayoutButtonBackground = 2131034241;

			public const int lightBlueDivider = 2131034242;

			public const int lightPrimary = 2131034243;

			public const int linkColor = 2131034244;

			public const int material_blue_grey_800 = 2131034245;

			public const int material_blue_grey_900 = 2131034246;

			public const int material_blue_grey_950 = 2131034247;

			public const int material_deep_teal_200 = 2131034248;

			public const int material_deep_teal_500 = 2131034249;

			public const int material_grey_100 = 2131034250;

			public const int material_grey_300 = 2131034251;

			public const int material_grey_50 = 2131034252;

			public const int material_grey_600 = 2131034253;

			public const int material_grey_800 = 2131034254;

			public const int material_grey_850 = 2131034255;

			public const int material_grey_900 = 2131034256;

			public const int material_on_background_disabled = 2131034257;

			public const int material_on_background_emphasis_high_type = 2131034258;

			public const int material_on_background_emphasis_medium = 2131034259;

			public const int material_on_primary_disabled = 2131034260;

			public const int material_on_primary_emphasis_high_type = 2131034261;

			public const int material_on_primary_emphasis_medium = 2131034262;

			public const int material_on_surface_disabled = 2131034263;

			public const int material_on_surface_emphasis_high_type = 2131034264;

			public const int material_on_surface_emphasis_medium = 2131034265;

			public const int mtrl_bottom_nav_colored_item_tint = 2131034266;

			public const int mtrl_bottom_nav_colored_ripple_color = 2131034267;

			public const int mtrl_bottom_nav_item_tint = 2131034268;

			public const int mtrl_bottom_nav_ripple_color = 2131034269;

			public const int mtrl_btn_bg_color_selector = 2131034270;

			public const int mtrl_btn_ripple_color = 2131034271;

			public const int mtrl_btn_stroke_color_selector = 2131034272;

			public const int mtrl_btn_text_btn_bg_color_selector = 2131034273;

			public const int mtrl_btn_text_btn_ripple_color = 2131034274;

			public const int mtrl_btn_text_color_disabled = 2131034275;

			public const int mtrl_btn_text_color_selector = 2131034276;

			public const int mtrl_btn_transparent_bg_color = 2131034277;

			public const int mtrl_calendar_item_stroke_color = 2131034278;

			public const int mtrl_calendar_selected_range = 2131034279;

			public const int mtrl_card_view_foreground = 2131034280;

			public const int mtrl_card_view_ripple = 2131034281;

			public const int mtrl_chip_background_color = 2131034282;

			public const int mtrl_chip_close_icon_tint = 2131034283;

			public const int mtrl_chip_ripple_color = 2131034284;

			public const int mtrl_chip_surface_color = 2131034285;

			public const int mtrl_chip_text_color = 2131034286;

			public const int mtrl_choice_chip_background_color = 2131034287;

			public const int mtrl_choice_chip_ripple_color = 2131034288;

			public const int mtrl_choice_chip_text_color = 2131034289;

			public const int mtrl_error = 2131034290;

			public const int mtrl_extended_fab_bg_color_selector = 2131034291;

			public const int mtrl_extended_fab_ripple_color = 2131034292;

			public const int mtrl_extended_fab_text_color_selector = 2131034293;

			public const int mtrl_fab_ripple_color = 2131034294;

			public const int mtrl_filled_background_color = 2131034295;

			public const int mtrl_filled_icon_tint = 2131034296;

			public const int mtrl_filled_stroke_color = 2131034297;

			public const int mtrl_indicator_text_color = 2131034298;

			public const int mtrl_navigation_item_background_color = 2131034299;

			public const int mtrl_navigation_item_icon_tint = 2131034300;

			public const int mtrl_navigation_item_text_color = 2131034301;

			public const int mtrl_on_primary_text_btn_text_color_selector = 2131034302;

			public const int mtrl_outlined_icon_tint = 2131034303;

			public const int mtrl_outlined_stroke_color = 2131034304;

			public const int mtrl_popupmenu_overlay_color = 2131034305;

			public const int mtrl_scrim_color = 2131034306;

			public const int mtrl_tabs_colored_ripple_color = 2131034307;

			public const int mtrl_tabs_icon_color_selector = 2131034308;

			public const int mtrl_tabs_icon_color_selector_colored = 2131034309;

			public const int mtrl_tabs_legacy_text_color_selector = 2131034310;

			public const int mtrl_tabs_ripple_color = 2131034311;

			public const int mtrl_textinput_default_box_stroke_color = 2131034313;

			public const int mtrl_textinput_disabled_color = 2131034314;

			public const int mtrl_textinput_filled_box_default_background_color = 2131034315;

			public const int mtrl_textinput_focused_box_stroke_color = 2131034316;

			public const int mtrl_textinput_hovered_box_stroke_color = 2131034317;

			public const int mtrl_text_btn_text_color_selector = 2131034312;

			public const int notification_action_color_filter = 2131034318;

			public const int notification_icon_bg_color = 2131034319;

			public const int notification_material_background_media_default_color = 2131034320;

			public const int primaryText = 2131034321;

			public const int primary_dark_material_dark = 2131034322;

			public const int primary_dark_material_light = 2131034323;

			public const int primary_material_dark = 2131034324;

			public const int primary_material_light = 2131034325;

			public const int primary_text_default_material_dark = 2131034326;

			public const int primary_text_default_material_light = 2131034327;

			public const int primary_text_disabled_material_dark = 2131034328;

			public const int primary_text_disabled_material_light = 2131034329;

			public const int ripple_material_dark = 2131034330;

			public const int ripple_material_light = 2131034331;

			public const int secondaryText = 2131034332;

			public const int secondary_text_default_material_dark = 2131034333;

			public const int secondary_text_default_material_light = 2131034334;

			public const int secondary_text_disabled_material_dark = 2131034335;

			public const int secondary_text_disabled_material_light = 2131034336;

			public const int selectedDot = 2131034337;

			public const int splashBackground = 2131034338;

			public const int switchSelectedThumb = 2131034339;

			public const int switchSelectedTrack = 2131034340;

			public const int switchUnselectedThumb = 2131034341;

			public const int switchUnselectedTrack = 2131034342;

			public const int switch_thumb_disabled_material_dark = 2131034343;

			public const int switch_thumb_disabled_material_light = 2131034344;

			public const int switch_thumb_material_dark = 2131034345;

			public const int switch_thumb_material_light = 2131034346;

			public const int switch_thumb_normal_material_dark = 2131034347;

			public const int switch_thumb_normal_material_light = 2131034348;

			public const int test_mtrl_calendar_day = 2131034349;

			public const int test_mtrl_calendar_day_selected = 2131034350;

			public const int textIcon = 2131034351;

			public const int tooltip_background_dark = 2131034352;

			public const int tooltip_background_light = 2131034353;

			public const int topbar = 2131034354;

			public const int topbarDevicer = 2131034355;

			public const int unselectedDot = 2131034356;

			public const int warningColor = 2131034357;

			static Color()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Color()
			{
			}
		}

		public class Dimension
		{
			public const int abc_action_bar_content_inset_material = 2131099648;

			public const int abc_action_bar_content_inset_with_nav = 2131099649;

			public const int abc_action_bar_default_height_material = 2131099650;

			public const int abc_action_bar_default_padding_end_material = 2131099651;

			public const int abc_action_bar_default_padding_start_material = 2131099652;

			public const int abc_action_bar_elevation_material = 2131099653;

			public const int abc_action_bar_icon_vertical_padding_material = 2131099654;

			public const int abc_action_bar_overflow_padding_end_material = 2131099655;

			public const int abc_action_bar_overflow_padding_start_material = 2131099656;

			public const int abc_action_bar_stacked_max_height = 2131099657;

			public const int abc_action_bar_stacked_tab_max_width = 2131099658;

			public const int abc_action_bar_subtitle_bottom_margin_material = 2131099659;

			public const int abc_action_bar_subtitle_top_margin_material = 2131099660;

			public const int abc_action_button_min_height_material = 2131099661;

			public const int abc_action_button_min_width_material = 2131099662;

			public const int abc_action_button_min_width_overflow_material = 2131099663;

			public const int abc_alert_dialog_button_bar_height = 2131099664;

			public const int abc_alert_dialog_button_dimen = 2131099665;

			public const int abc_button_inset_horizontal_material = 2131099666;

			public const int abc_button_inset_vertical_material = 2131099667;

			public const int abc_button_padding_horizontal_material = 2131099668;

			public const int abc_button_padding_vertical_material = 2131099669;

			public const int abc_cascading_menus_min_smallest_width = 2131099670;

			public const int abc_config_prefDialogWidth = 2131099671;

			public const int abc_control_corner_material = 2131099672;

			public const int abc_control_inset_material = 2131099673;

			public const int abc_control_padding_material = 2131099674;

			public const int abc_dialog_corner_radius_material = 2131099675;

			public const int abc_dialog_fixed_height_major = 2131099676;

			public const int abc_dialog_fixed_height_minor = 2131099677;

			public const int abc_dialog_fixed_width_major = 2131099678;

			public const int abc_dialog_fixed_width_minor = 2131099679;

			public const int abc_dialog_list_padding_bottom_no_buttons = 2131099680;

			public const int abc_dialog_list_padding_top_no_title = 2131099681;

			public const int abc_dialog_min_width_major = 2131099682;

			public const int abc_dialog_min_width_minor = 2131099683;

			public const int abc_dialog_padding_material = 2131099684;

			public const int abc_dialog_padding_top_material = 2131099685;

			public const int abc_dialog_title_divider_material = 2131099686;

			public const int abc_disabled_alpha_material_dark = 2131099687;

			public const int abc_disabled_alpha_material_light = 2131099688;

			public const int abc_dropdownitem_icon_width = 2131099689;

			public const int abc_dropdownitem_text_padding_left = 2131099690;

			public const int abc_dropdownitem_text_padding_right = 2131099691;

			public const int abc_edit_text_inset_bottom_material = 2131099692;

			public const int abc_edit_text_inset_horizontal_material = 2131099693;

			public const int abc_edit_text_inset_top_material = 2131099694;

			public const int abc_floating_window_z = 2131099695;

			public const int abc_list_item_height_large_material = 2131099696;

			public const int abc_list_item_height_material = 2131099697;

			public const int abc_list_item_height_small_material = 2131099698;

			public const int abc_list_item_padding_horizontal_material = 2131099699;

			public const int abc_panel_menu_list_width = 2131099700;

			public const int abc_progress_bar_height_material = 2131099701;

			public const int abc_search_view_preferred_height = 2131099702;

			public const int abc_search_view_preferred_width = 2131099703;

			public const int abc_seekbar_track_background_height_material = 2131099704;

			public const int abc_seekbar_track_progress_height_material = 2131099705;

			public const int abc_select_dialog_padding_start_material = 2131099706;

			public const int abc_switch_padding = 2131099707;

			public const int abc_text_size_body_1_material = 2131099708;

			public const int abc_text_size_body_2_material = 2131099709;

			public const int abc_text_size_button_material = 2131099710;

			public const int abc_text_size_caption_material = 2131099711;

			public const int abc_text_size_display_1_material = 2131099712;

			public const int abc_text_size_display_2_material = 2131099713;

			public const int abc_text_size_display_3_material = 2131099714;

			public const int abc_text_size_display_4_material = 2131099715;

			public const int abc_text_size_headline_material = 2131099716;

			public const int abc_text_size_large_material = 2131099717;

			public const int abc_text_size_medium_material = 2131099718;

			public const int abc_text_size_menu_header_material = 2131099719;

			public const int abc_text_size_menu_material = 2131099720;

			public const int abc_text_size_small_material = 2131099721;

			public const int abc_text_size_subhead_material = 2131099722;

			public const int abc_text_size_subtitle_material_toolbar = 2131099723;

			public const int abc_text_size_title_material = 2131099724;

			public const int abc_text_size_title_material_toolbar = 2131099725;

			public const int action_bar_size = 2131099726;

			public const int appcompat_dialog_background_inset = 2131099727;

			public const int browser_actions_context_menu_max_width = 2131099728;

			public const int browser_actions_context_menu_min_padding = 2131099729;

			public const int cardview_compat_inset_shadow = 2131099730;

			public const int cardview_default_elevation = 2131099731;

			public const int cardview_default_radius = 2131099732;

			public const int compat_button_inset_horizontal_material = 2131099733;

			public const int compat_button_inset_vertical_material = 2131099734;

			public const int compat_button_padding_horizontal_material = 2131099735;

			public const int compat_button_padding_vertical_material = 2131099736;

			public const int compat_control_corner_material = 2131099737;

			public const int compat_notification_large_icon_max_height = 2131099738;

			public const int compat_notification_large_icon_max_width = 2131099739;

			public const int default_dimension = 2131099740;

			public const int design_appbar_elevation = 2131099741;

			public const int design_bottom_navigation_active_item_max_width = 2131099742;

			public const int design_bottom_navigation_active_item_min_width = 2131099743;

			public const int design_bottom_navigation_active_text_size = 2131099744;

			public const int design_bottom_navigation_elevation = 2131099745;

			public const int design_bottom_navigation_height = 2131099746;

			public const int design_bottom_navigation_icon_size = 2131099747;

			public const int design_bottom_navigation_item_max_width = 2131099748;

			public const int design_bottom_navigation_item_min_width = 2131099749;

			public const int design_bottom_navigation_margin = 2131099750;

			public const int design_bottom_navigation_shadow_height = 2131099751;

			public const int design_bottom_navigation_text_size = 2131099752;

			public const int design_bottom_sheet_elevation = 2131099753;

			public const int design_bottom_sheet_modal_elevation = 2131099754;

			public const int design_bottom_sheet_peek_height_min = 2131099755;

			public const int design_fab_border_width = 2131099756;

			public const int design_fab_elevation = 2131099757;

			public const int design_fab_image_size = 2131099758;

			public const int design_fab_size_mini = 2131099759;

			public const int design_fab_size_normal = 2131099760;

			public const int design_fab_translation_z_hovered_focused = 2131099761;

			public const int design_fab_translation_z_pressed = 2131099762;

			public const int design_navigation_elevation = 2131099763;

			public const int design_navigation_icon_padding = 2131099764;

			public const int design_navigation_icon_size = 2131099765;

			public const int design_navigation_item_horizontal_padding = 2131099766;

			public const int design_navigation_item_icon_padding = 2131099767;

			public const int design_navigation_max_width = 2131099768;

			public const int design_navigation_padding_bottom = 2131099769;

			public const int design_navigation_separator_vertical_padding = 2131099770;

			public const int design_snackbar_action_inline_max_width = 2131099771;

			public const int design_snackbar_action_text_color_alpha = 2131099772;

			public const int design_snackbar_background_corner_radius = 2131099773;

			public const int design_snackbar_elevation = 2131099774;

			public const int design_snackbar_extra_spacing_horizontal = 2131099775;

			public const int design_snackbar_max_width = 2131099776;

			public const int design_snackbar_min_width = 2131099777;

			public const int design_snackbar_padding_horizontal = 2131099778;

			public const int design_snackbar_padding_vertical = 2131099779;

			public const int design_snackbar_padding_vertical_2lines = 2131099780;

			public const int design_snackbar_text_size = 2131099781;

			public const int design_tab_max_width = 2131099782;

			public const int design_tab_scrollable_min_width = 2131099783;

			public const int design_tab_text_size = 2131099784;

			public const int design_tab_text_size_2line = 2131099785;

			public const int design_textinput_caption_translate_y = 2131099786;

			public const int disabled_alpha_material_dark = 2131099787;

			public const int disabled_alpha_material_light = 2131099788;

			public const int fab_margin = 2131099789;

			public const int fastscroll_default_thickness = 2131099790;

			public const int fastscroll_margin = 2131099791;

			public const int fastscroll_minimum_range = 2131099792;

			public const int highlight_alpha_material_colored = 2131099793;

			public const int highlight_alpha_material_dark = 2131099794;

			public const int highlight_alpha_material_light = 2131099795;

			public const int hint_alpha_material_dark = 2131099796;

			public const int hint_alpha_material_light = 2131099797;

			public const int hint_pressed_alpha_material_dark = 2131099798;

			public const int hint_pressed_alpha_material_light = 2131099799;

			public const int item_touch_helper_max_drag_scroll_per_frame = 2131099800;

			public const int item_touch_helper_swipe_escape_max_velocity = 2131099801;

			public const int item_touch_helper_swipe_escape_velocity = 2131099802;

			public const int material_emphasis_disabled = 2131099803;

			public const int material_emphasis_high_type = 2131099804;

			public const int material_emphasis_medium = 2131099805;

			public const int material_text_view_test_line_height = 2131099806;

			public const int material_text_view_test_line_height_override = 2131099807;

			public const int mtrl_alert_dialog_background_inset_bottom = 2131099808;

			public const int mtrl_alert_dialog_background_inset_end = 2131099809;

			public const int mtrl_alert_dialog_background_inset_start = 2131099810;

			public const int mtrl_alert_dialog_background_inset_top = 2131099811;

			public const int mtrl_alert_dialog_picker_background_inset = 2131099812;

			public const int mtrl_badge_horizontal_edge_offset = 2131099813;

			public const int mtrl_badge_long_text_horizontal_padding = 2131099814;

			public const int mtrl_badge_radius = 2131099815;

			public const int mtrl_badge_text_horizontal_edge_offset = 2131099816;

			public const int mtrl_badge_text_size = 2131099817;

			public const int mtrl_badge_with_text_radius = 2131099818;

			public const int mtrl_bottomappbar_fabOffsetEndMode = 2131099819;

			public const int mtrl_bottomappbar_fab_bottom_margin = 2131099820;

			public const int mtrl_bottomappbar_fab_cradle_margin = 2131099821;

			public const int mtrl_bottomappbar_fab_cradle_rounded_corner_radius = 2131099822;

			public const int mtrl_bottomappbar_fab_cradle_vertical_offset = 2131099823;

			public const int mtrl_bottomappbar_height = 2131099824;

			public const int mtrl_btn_corner_radius = 2131099825;

			public const int mtrl_btn_dialog_btn_min_width = 2131099826;

			public const int mtrl_btn_disabled_elevation = 2131099827;

			public const int mtrl_btn_disabled_z = 2131099828;

			public const int mtrl_btn_elevation = 2131099829;

			public const int mtrl_btn_focused_z = 2131099830;

			public const int mtrl_btn_hovered_z = 2131099831;

			public const int mtrl_btn_icon_btn_padding_left = 2131099832;

			public const int mtrl_btn_icon_padding = 2131099833;

			public const int mtrl_btn_inset = 2131099834;

			public const int mtrl_btn_letter_spacing = 2131099835;

			public const int mtrl_btn_padding_bottom = 2131099836;

			public const int mtrl_btn_padding_left = 2131099837;

			public const int mtrl_btn_padding_right = 2131099838;

			public const int mtrl_btn_padding_top = 2131099839;

			public const int mtrl_btn_pressed_z = 2131099840;

			public const int mtrl_btn_stroke_size = 2131099841;

			public const int mtrl_btn_text_btn_icon_padding = 2131099842;

			public const int mtrl_btn_text_btn_padding_left = 2131099843;

			public const int mtrl_btn_text_btn_padding_right = 2131099844;

			public const int mtrl_btn_text_size = 2131099845;

			public const int mtrl_btn_z = 2131099846;

			public const int mtrl_calendar_action_height = 2131099847;

			public const int mtrl_calendar_action_padding = 2131099848;

			public const int mtrl_calendar_bottom_padding = 2131099849;

			public const int mtrl_calendar_content_padding = 2131099850;

			public const int mtrl_calendar_days_of_week_height = 2131099857;

			public const int mtrl_calendar_day_corner = 2131099851;

			public const int mtrl_calendar_day_height = 2131099852;

			public const int mtrl_calendar_day_horizontal_padding = 2131099853;

			public const int mtrl_calendar_day_today_stroke = 2131099854;

			public const int mtrl_calendar_day_vertical_padding = 2131099855;

			public const int mtrl_calendar_day_width = 2131099856;

			public const int mtrl_calendar_dialog_background_inset = 2131099858;

			public const int mtrl_calendar_header_content_padding = 2131099859;

			public const int mtrl_calendar_header_content_padding_fullscreen = 2131099860;

			public const int mtrl_calendar_header_divider_thickness = 2131099861;

			public const int mtrl_calendar_header_height = 2131099862;

			public const int mtrl_calendar_header_height_fullscreen = 2131099863;

			public const int mtrl_calendar_header_selection_line_height = 2131099864;

			public const int mtrl_calendar_header_text_padding = 2131099865;

			public const int mtrl_calendar_header_toggle_margin_bottom = 2131099866;

			public const int mtrl_calendar_header_toggle_margin_top = 2131099867;

			public const int mtrl_calendar_landscape_header_width = 2131099868;

			public const int mtrl_calendar_maximum_default_fullscreen_minor_axis = 2131099869;

			public const int mtrl_calendar_month_horizontal_padding = 2131099870;

			public const int mtrl_calendar_month_vertical_padding = 2131099871;

			public const int mtrl_calendar_navigation_bottom_padding = 2131099872;

			public const int mtrl_calendar_navigation_height = 2131099873;

			public const int mtrl_calendar_navigation_top_padding = 2131099874;

			public const int mtrl_calendar_pre_l_text_clip_padding = 2131099875;

			public const int mtrl_calendar_selection_baseline_to_top_fullscreen = 2131099876;

			public const int mtrl_calendar_selection_text_baseline_to_bottom = 2131099877;

			public const int mtrl_calendar_selection_text_baseline_to_bottom_fullscreen = 2131099878;

			public const int mtrl_calendar_selection_text_baseline_to_top = 2131099879;

			public const int mtrl_calendar_text_input_padding_top = 2131099880;

			public const int mtrl_calendar_title_baseline_to_top = 2131099881;

			public const int mtrl_calendar_title_baseline_to_top_fullscreen = 2131099882;

			public const int mtrl_calendar_year_corner = 2131099883;

			public const int mtrl_calendar_year_height = 2131099884;

			public const int mtrl_calendar_year_horizontal_padding = 2131099885;

			public const int mtrl_calendar_year_vertical_padding = 2131099886;

			public const int mtrl_calendar_year_width = 2131099887;

			public const int mtrl_card_checked_icon_margin = 2131099888;

			public const int mtrl_card_checked_icon_size = 2131099889;

			public const int mtrl_card_corner_radius = 2131099890;

			public const int mtrl_card_dragged_z = 2131099891;

			public const int mtrl_card_elevation = 2131099892;

			public const int mtrl_card_spacing = 2131099893;

			public const int mtrl_chip_pressed_translation_z = 2131099894;

			public const int mtrl_chip_text_size = 2131099895;

			public const int mtrl_exposed_dropdown_menu_popup_elevation = 2131099896;

			public const int mtrl_exposed_dropdown_menu_popup_vertical_offset = 2131099897;

			public const int mtrl_exposed_dropdown_menu_popup_vertical_padding = 2131099898;

			public const int mtrl_extended_fab_bottom_padding = 2131099899;

			public const int mtrl_extended_fab_corner_radius = 2131099900;

			public const int mtrl_extended_fab_disabled_elevation = 2131099901;

			public const int mtrl_extended_fab_disabled_translation_z = 2131099902;

			public const int mtrl_extended_fab_elevation = 2131099903;

			public const int mtrl_extended_fab_end_padding = 2131099904;

			public const int mtrl_extended_fab_end_padding_icon = 2131099905;

			public const int mtrl_extended_fab_icon_size = 2131099906;

			public const int mtrl_extended_fab_icon_text_spacing = 2131099907;

			public const int mtrl_extended_fab_min_height = 2131099908;

			public const int mtrl_extended_fab_min_width = 2131099909;

			public const int mtrl_extended_fab_start_padding = 2131099910;

			public const int mtrl_extended_fab_start_padding_icon = 2131099911;

			public const int mtrl_extended_fab_top_padding = 2131099912;

			public const int mtrl_extended_fab_translation_z_base = 2131099913;

			public const int mtrl_extended_fab_translation_z_hovered_focused = 2131099914;

			public const int mtrl_extended_fab_translation_z_pressed = 2131099915;

			public const int mtrl_fab_elevation = 2131099916;

			public const int mtrl_fab_min_touch_target = 2131099917;

			public const int mtrl_fab_translation_z_hovered_focused = 2131099918;

			public const int mtrl_fab_translation_z_pressed = 2131099919;

			public const int mtrl_high_ripple_default_alpha = 2131099920;

			public const int mtrl_high_ripple_focused_alpha = 2131099921;

			public const int mtrl_high_ripple_hovered_alpha = 2131099922;

			public const int mtrl_high_ripple_pressed_alpha = 2131099923;

			public const int mtrl_large_touch_target = 2131099924;

			public const int mtrl_low_ripple_default_alpha = 2131099925;

			public const int mtrl_low_ripple_focused_alpha = 2131099926;

			public const int mtrl_low_ripple_hovered_alpha = 2131099927;

			public const int mtrl_low_ripple_pressed_alpha = 2131099928;

			public const int mtrl_min_touch_target_size = 2131099929;

			public const int mtrl_navigation_elevation = 2131099930;

			public const int mtrl_navigation_item_horizontal_padding = 2131099931;

			public const int mtrl_navigation_item_icon_padding = 2131099932;

			public const int mtrl_navigation_item_icon_size = 2131099933;

			public const int mtrl_navigation_item_shape_horizontal_margin = 2131099934;

			public const int mtrl_navigation_item_shape_vertical_margin = 2131099935;

			public const int mtrl_shape_corner_size_large_component = 2131099936;

			public const int mtrl_shape_corner_size_medium_component = 2131099937;

			public const int mtrl_shape_corner_size_small_component = 2131099938;

			public const int mtrl_snackbar_action_text_color_alpha = 2131099939;

			public const int mtrl_snackbar_background_corner_radius = 2131099940;

			public const int mtrl_snackbar_background_overlay_color_alpha = 2131099941;

			public const int mtrl_snackbar_margin = 2131099942;

			public const int mtrl_switch_thumb_elevation = 2131099943;

			public const int mtrl_textinput_box_corner_radius_medium = 2131099944;

			public const int mtrl_textinput_box_corner_radius_small = 2131099945;

			public const int mtrl_textinput_box_label_cutout_padding = 2131099946;

			public const int mtrl_textinput_box_stroke_width_default = 2131099947;

			public const int mtrl_textinput_box_stroke_width_focused = 2131099948;

			public const int mtrl_textinput_end_icon_margin_start = 2131099949;

			public const int mtrl_textinput_outline_box_expanded_padding = 2131099950;

			public const int mtrl_textinput_start_icon_margin_end = 2131099951;

			public const int mtrl_toolbar_default_height = 2131099952;

			public const int notification_action_icon_size = 2131099953;

			public const int notification_action_text_size = 2131099954;

			public const int notification_big_circle_margin = 2131099955;

			public const int notification_content_margin_start = 2131099956;

			public const int notification_large_icon_height = 2131099957;

			public const int notification_large_icon_width = 2131099958;

			public const int notification_main_column_padding_top = 2131099959;

			public const int notification_media_narrow_margin = 2131099960;

			public const int notification_right_icon_size = 2131099961;

			public const int notification_right_side_padding_top = 2131099962;

			public const int notification_small_icon_background_padding = 2131099963;

			public const int notification_small_icon_size_as_large = 2131099964;

			public const int notification_subtext_size = 2131099965;

			public const int notification_top_pad = 2131099966;

			public const int notification_top_pad_large_text = 2131099967;

			public const int subtitle_corner_radius = 2131099968;

			public const int subtitle_outline_width = 2131099969;

			public const int subtitle_shadow_offset = 2131099970;

			public const int subtitle_shadow_radius = 2131099971;

			public const int test_mtrl_calendar_day_cornerSize = 2131099972;

			public const int tooltip_corner_radius = 2131099973;

			public const int tooltip_horizontal_padding = 2131099974;

			public const int tooltip_margin = 2131099975;

			public const int tooltip_precise_anchor_extra_offset = 2131099976;

			public const int tooltip_precise_anchor_threshold = 2131099977;

			public const int tooltip_vertical_padding = 2131099978;

			public const int tooltip_y_offset_non_touch = 2131099979;

			public const int tooltip_y_offset_touch = 2131099980;

			static Dimension()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Dimension()
			{
			}
		}

		public class Drawable
		{
			public const int abc_ab_share_pack_mtrl_alpha = 2131165191;

			public const int abc_action_bar_item_background_material = 2131165192;

			public const int abc_btn_borderless_material = 2131165193;

			public const int abc_btn_check_material = 2131165194;

			public const int abc_btn_check_material_anim = 2131165195;

			public const int abc_btn_check_to_on_mtrl_000 = 2131165196;

			public const int abc_btn_check_to_on_mtrl_015 = 2131165197;

			public const int abc_btn_colored_material = 2131165198;

			public const int abc_btn_default_mtrl_shape = 2131165199;

			public const int abc_btn_radio_material = 2131165200;

			public const int abc_btn_radio_material_anim = 2131165201;

			public const int abc_btn_radio_to_on_mtrl_000 = 2131165202;

			public const int abc_btn_radio_to_on_mtrl_015 = 2131165203;

			public const int abc_btn_switch_to_on_mtrl_00001 = 2131165204;

			public const int abc_btn_switch_to_on_mtrl_00012 = 2131165205;

			public const int abc_cab_background_internal_bg = 2131165206;

			public const int abc_cab_background_top_material = 2131165207;

			public const int abc_cab_background_top_mtrl_alpha = 2131165208;

			public const int abc_control_background_material = 2131165209;

			public const int abc_dialog_material_background = 2131165210;

			public const int abc_edit_text_material = 2131165211;

			public const int abc_ic_ab_back_material = 2131165212;

			public const int abc_ic_arrow_drop_right_black_24dp = 2131165213;

			public const int abc_ic_clear_material = 2131165214;

			public const int abc_ic_commit_search_api_mtrl_alpha = 2131165215;

			public const int abc_ic_go_search_api_material = 2131165216;

			public const int abc_ic_menu_copy_mtrl_am_alpha = 2131165217;

			public const int abc_ic_menu_cut_mtrl_alpha = 2131165218;

			public const int abc_ic_menu_overflow_material = 2131165219;

			public const int abc_ic_menu_paste_mtrl_am_alpha = 2131165220;

			public const int abc_ic_menu_selectall_mtrl_alpha = 2131165221;

			public const int abc_ic_menu_share_mtrl_alpha = 2131165222;

			public const int abc_ic_search_api_material = 2131165223;

			public const int abc_ic_star_black_16dp = 2131165224;

			public const int abc_ic_star_black_36dp = 2131165225;

			public const int abc_ic_star_black_48dp = 2131165226;

			public const int abc_ic_star_half_black_16dp = 2131165227;

			public const int abc_ic_star_half_black_36dp = 2131165228;

			public const int abc_ic_star_half_black_48dp = 2131165229;

			public const int abc_ic_voice_search_api_material = 2131165230;

			public const int abc_item_background_holo_dark = 2131165231;

			public const int abc_item_background_holo_light = 2131165232;

			public const int abc_list_divider_material = 2131165233;

			public const int abc_list_divider_mtrl_alpha = 2131165234;

			public const int abc_list_focused_holo = 2131165235;

			public const int abc_list_longpressed_holo = 2131165236;

			public const int abc_list_pressed_holo_dark = 2131165237;

			public const int abc_list_pressed_holo_light = 2131165238;

			public const int abc_list_selector_background_transition_holo_dark = 2131165239;

			public const int abc_list_selector_background_transition_holo_light = 2131165240;

			public const int abc_list_selector_disabled_holo_dark = 2131165241;

			public const int abc_list_selector_disabled_holo_light = 2131165242;

			public const int abc_list_selector_holo_dark = 2131165243;

			public const int abc_list_selector_holo_light = 2131165244;

			public const int abc_menu_hardkey_panel_mtrl_mult = 2131165245;

			public const int abc_popup_background_mtrl_mult = 2131165246;

			public const int abc_ratingbar_indicator_material = 2131165247;

			public const int abc_ratingbar_material = 2131165248;

			public const int abc_ratingbar_small_material = 2131165249;

			public const int abc_scrubber_control_off_mtrl_alpha = 2131165250;

			public const int abc_scrubber_control_to_pressed_mtrl_000 = 2131165251;

			public const int abc_scrubber_control_to_pressed_mtrl_005 = 2131165252;

			public const int abc_scrubber_primary_mtrl_alpha = 2131165253;

			public const int abc_scrubber_track_mtrl_alpha = 2131165254;

			public const int abc_seekbar_thumb_material = 2131165255;

			public const int abc_seekbar_tick_mark_material = 2131165256;

			public const int abc_seekbar_track_material = 2131165257;

			public const int abc_spinner_mtrl_am_alpha = 2131165258;

			public const int abc_spinner_textfield_background_material = 2131165259;

			public const int abc_switch_thumb_material = 2131165260;

			public const int abc_switch_track_mtrl_alpha = 2131165261;

			public const int abc_tab_indicator_material = 2131165262;

			public const int abc_tab_indicator_mtrl_alpha = 2131165263;

			public const int abc_textfield_activated_mtrl_alpha = 2131165271;

			public const int abc_textfield_default_mtrl_alpha = 2131165272;

			public const int abc_textfield_search_activated_mtrl_alpha = 2131165273;

			public const int abc_textfield_search_default_mtrl_alpha = 2131165274;

			public const int abc_textfield_search_material = 2131165275;

			public const int abc_text_cursor_material = 2131165264;

			public const int abc_text_select_handle_left_mtrl_dark = 2131165265;

			public const int abc_text_select_handle_left_mtrl_light = 2131165266;

			public const int abc_text_select_handle_middle_mtrl_dark = 2131165267;

			public const int abc_text_select_handle_middle_mtrl_light = 2131165268;

			public const int abc_text_select_handle_right_mtrl_dark = 2131165269;

			public const int abc_text_select_handle_right_mtrl_light = 2131165270;

			public const int abc_vector_test = 2131165276;

			public const int anonymus = 2131165277;

			public const int avd_hide_password = 2131165278;

			public const int avd_show_password = 2131165279;

			public const int background_circle_green = 2131165280;

			public const int background_circle_red = 2131165281;

			public const int bluetooth = 2131165282;

			public const int bluetooth_icon = 2131165283;

			public const int btn_checkbox_checked_mtrl = 2131165284;

			public const int btn_checkbox_checked_to_unchecked_mtrl_animation = 2131165285;

			public const int btn_checkbox_unchecked_mtrl = 2131165286;

			public const int btn_checkbox_unchecked_to_checked_mtrl_animation = 2131165287;

			public const int btn_radio_off_mtrl = 2131165288;

			public const int btn_radio_off_to_on_mtrl_animation = 2131165289;

			public const int btn_radio_on_mtrl = 2131165290;

			public const int btn_radio_on_to_off_mtrl_animation = 2131165291;

			public const int bubble = 2131165292;

			public const int checkmark = 2131165293;

			public const int circle = 2131165294;

			public const int circle_greyed_out = 2131165295;

			public const int circle_textview = 2131165296;

			public const int color_gradient = 2131165297;

			public const int common_full_open_on_phone = 2131165298;

			public const int common_google_signin_btn_icon_dark = 2131165299;

			public const int common_google_signin_btn_icon_dark_focused = 2131165300;

			public const int common_google_signin_btn_icon_dark_normal = 2131165301;

			public const int common_google_signin_btn_icon_dark_normal_background = 2131165302;

			public const int common_google_signin_btn_icon_disabled = 2131165303;

			public const int common_google_signin_btn_icon_light = 2131165304;

			public const int common_google_signin_btn_icon_light_focused = 2131165305;

			public const int common_google_signin_btn_icon_light_normal = 2131165306;

			public const int common_google_signin_btn_icon_light_normal_background = 2131165307;

			public const int common_google_signin_btn_text_dark = 2131165308;

			public const int common_google_signin_btn_text_dark_focused = 2131165309;

			public const int common_google_signin_btn_text_dark_normal = 2131165310;

			public const int common_google_signin_btn_text_dark_normal_background = 2131165311;

			public const int common_google_signin_btn_text_disabled = 2131165312;

			public const int common_google_signin_btn_text_light = 2131165313;

			public const int common_google_signin_btn_text_light_focused = 2131165314;

			public const int common_google_signin_btn_text_light_normal = 2131165315;

			public const int common_google_signin_btn_text_light_normal_background = 2131165316;

			public const int counter_background = 2131165317;

			public const int counter_background_disease_rate_bottom = 2131165318;

			public const int counter_background_disease_rate_mid = 2131165319;

			public const int counter_background_disease_rate_top = 2131165320;

			public const int date_background = 2131165321;

			public const int default_button = 2131165322;

			public const int default_button_green = 2131165323;

			public const int default_button_no_border = 2131165324;

			public const int default_button_white = 2131165325;

			public const int default_dot = 2131165326;

			public const int design_bottom_navigation_item_background = 2131165327;

			public const int design_fab_background = 2131165328;

			public const int design_ic_visibility = 2131165329;

			public const int design_ic_visibility_off = 2131165330;

			public const int design_password_eye = 2131165331;

			public const int design_snackbar_background = 2131165332;

			public const int dotselector = 2131165333;

			public const int ellipse = 2131165334;

			public const int googleg_disabled_color_18 = 2131165335;

			public const int googleg_standard_color_18 = 2131165336;

			public const int gradientBackground = 2131165337;

			public const int health_department_da = 2131165338;

			public const int health_department_en = 2131165339;

			public const int ic_arrow_back = 2131165340;

			public const int ic_arrow_back_right = 2131165341;

			public const int ic_back_arrow = 2131165342;

			public const int ic_back_icon = 2131165343;

			public const int ic_bg_two = 2131165344;

			public const int ic_calendar = 2131165345;

			public const int ic_calendar_black_24dp = 2131165346;

			public const int ic_clear_black_24dp = 2131165347;

			public const int ic_close_white = 2131165348;

			public const int ic_covid_virus = 2131165349;

			public const int ic_disease_rate = 2131165350;

			public const int ic_down_arrow = 2131165351;

			public const int ic_edit_black_24dp = 2131165352;

			public const int ic_green_tick = 2131165353;

			public const int ic_help = 2131165354;

			public const int ic_info = 2131165355;

			public const int ic_information = 2131165356;

			public const int ic_keyboard_arrow_left_black_24dp = 2131165357;

			public const int ic_keyboard_arrow_right_black_24dp = 2131165358;

			public const int ic_logo_no_chain = 2131165359;

			public const int ic_menu_arrow_down_black_24dp = 2131165360;

			public const int ic_menu_arrow_up_black_24dp = 2131165361;

			public const int ic_mtrl_checked_circle = 2131165362;

			public const int ic_mtrl_chip_checked_black = 2131165363;

			public const int ic_mtrl_chip_checked_circle = 2131165364;

			public const int ic_mtrl_chip_close_circle = 2131165365;

			public const int ic_nemid_white = 2131165366;

			public const int ic_notification = 2131165367;

			public const int ic_notification_dot = 2131165368;

			public const int ic_pause = 2131165369;

			public const int ic_person = 2131165370;

			public const int ic_play = 2131165371;

			public const int ic_settings = 2131165372;

			public const int ic_smittestop = 2131165373;

			public const int ic_smittestop_small = 2131165374;

			public const int ic_sst_crown_white = 2131165375;

			public const int ic_start_logo = 2131165376;

			public const int ic_start_logo_ag_api = 2131165377;

			public const int ic_warning_inverted = 2131165378;

			public const int ic_warning_orange = 2131165379;

			public const int infection_status_layout_button = 2131165380;

			public const int infection_status_on_off_button_green = 2131165381;

			public const int Infection_status_on_off_button_red = 2131165382;

			public const int infection_status_scroll_down_background = 2131165383;

			public const int logo_small = 2131165384;

			public const int logo_small_en = 2131165385;

			public const int menu = 2131165386;

			public const int message_item_normal = 2131165387;

			public const int message_item_pressed = 2131165388;

			public const int mtrl_dialog_background = 2131165389;

			public const int mtrl_dropdown_arrow = 2131165390;

			public const int mtrl_ic_arrow_drop_down = 2131165391;

			public const int mtrl_ic_arrow_drop_up = 2131165392;

			public const int mtrl_ic_cancel = 2131165393;

			public const int mtrl_ic_error = 2131165394;

			public const int mtrl_popupmenu_background = 2131165395;

			public const int mtrl_popupmenu_background_dark = 2131165396;

			public const int mtrl_tabs_default_indicator = 2131165397;

			public const int navigation_empty_icon = 2131165398;

			public const int notification_action_background = 2131165399;

			public const int notification_bg = 2131165400;

			public const int notification_bg_low = 2131165401;

			public const int notification_bg_low_normal = 2131165402;

			public const int notification_bg_low_pressed = 2131165403;

			public const int notification_bg_normal = 2131165404;

			public const int notification_bg_normal_pressed = 2131165405;

			public const int notification_icon_background = 2131165406;

			public const int notification_template_icon_bg = 2131165407;

			public const int notification_template_icon_low_bg = 2131165408;

			public const int notification_tile_bg = 2131165409;

			public const int notify_panel_notification_icon_bg = 2131165410;

			public const int onboarding02left = 2131165413;

			public const int onboarding02right = 2131165414;

			public const int on_off_button = 2131165411;

			public const int on_off_button_green = 2131165412;

			public const int patient_logo = 2131165415;

			public const int patient_logo_da = 2131165416;

			public const int patient_logo_en = 2131165417;

			public const int progress_bar = 2131165418;

			public const int recipe_bg = 2131165419;

			public const int rectangle = 2131165420;

			public const int selected_dot = 2131165421;

			public const int spinner = 2131165422;

			public const int sundhedLogo = 2131165423;

			public const int switch_thumb = 2131165424;

			public const int switch_track = 2131165425;

			public const int technology_background = 2131165426;

			public const int technology_background_ag_api = 2131165427;

			public const int test_custom_background = 2131165428;

			public const int thumb_selector = 2131165429;

			public const int tooltip_frame_dark = 2131165430;

			public const int tooltip_frame_light = 2131165431;

			public const int track_selector = 2131165432;

			public const int working_schema = 2131165433;

			public const int working_schema_ag_api = 2131165434;

			static Drawable()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Drawable()
			{
			}
		}

		public class Font
		{
			public const int IBMPlexSans = 2131230720;

			public const int ibmplexsans_bold = 2131230721;

			public const int ibmplexsans_bolditalic = 2131230722;

			public const int ibmplexsans_extralightitalic = 2131230723;

			public const int ibmplexsans_extralightt = 2131230724;

			public const int ibmplexsans_italic = 2131230725;

			public const int ibmplexsans_light = 2131230726;

			public const int ibmplexsans_lightitalic = 2131230727;

			public const int ibmplexsans_medium = 2131230728;

			public const int ibmplexsans_mediumitalic = 2131230729;

			public const int ibmplexsans_regular = 2131230730;

			public const int ibmplexsans_semibold = 2131230731;

			public const int ibmplexsans_semibolditalic = 2131230732;

			public const int ibmplexsans_thin = 2131230733;

			public const int ibmplexsans_thinitalic = 2131230734;

			public const int raleway = 2131230735;

			public const int raleway_black = 2131230736;

			public const int raleway_blackitalic = 2131230737;

			public const int raleway_bold = 2131230738;

			public const int raleway_bolditalic = 2131230739;

			public const int raleway_extrabold = 2131230740;

			public const int raleway_extrabolditalic = 2131230741;

			public const int raleway_extralight = 2131230742;

			public const int raleway_extralightitalic = 2131230743;

			public const int raleway_italic = 2131230744;

			public const int raleway_light = 2131230745;

			public const int raleway_lightitalic = 2131230746;

			public const int raleway_medium = 2131230747;

			public const int raleway_mediumitalic = 2131230748;

			public const int raleway_regular = 2131230749;

			public const int raleway_semibold = 2131230750;

			public const int raleway_semibolditalic = 2131230751;

			public const int raleway_thin = 2131230752;

			public const int raleway_thinitalic = 2131230753;

			static Font()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Font()
			{
			}
		}

		public class Id
		{
			public const int accessibility_action_clickable_span = 2131296266;

			public const int accessibility_custom_action_0 = 2131296267;

			public const int accessibility_custom_action_1 = 2131296268;

			public const int accessibility_custom_action_10 = 2131296269;

			public const int accessibility_custom_action_11 = 2131296270;

			public const int accessibility_custom_action_12 = 2131296271;

			public const int accessibility_custom_action_13 = 2131296272;

			public const int accessibility_custom_action_14 = 2131296273;

			public const int accessibility_custom_action_15 = 2131296274;

			public const int accessibility_custom_action_16 = 2131296275;

			public const int accessibility_custom_action_17 = 2131296276;

			public const int accessibility_custom_action_18 = 2131296277;

			public const int accessibility_custom_action_19 = 2131296278;

			public const int accessibility_custom_action_2 = 2131296279;

			public const int accessibility_custom_action_20 = 2131296280;

			public const int accessibility_custom_action_21 = 2131296281;

			public const int accessibility_custom_action_22 = 2131296282;

			public const int accessibility_custom_action_23 = 2131296283;

			public const int accessibility_custom_action_24 = 2131296284;

			public const int accessibility_custom_action_25 = 2131296285;

			public const int accessibility_custom_action_26 = 2131296286;

			public const int accessibility_custom_action_27 = 2131296287;

			public const int accessibility_custom_action_28 = 2131296288;

			public const int accessibility_custom_action_29 = 2131296289;

			public const int accessibility_custom_action_3 = 2131296290;

			public const int accessibility_custom_action_30 = 2131296291;

			public const int accessibility_custom_action_31 = 2131296292;

			public const int accessibility_custom_action_4 = 2131296293;

			public const int accessibility_custom_action_5 = 2131296294;

			public const int accessibility_custom_action_6 = 2131296295;

			public const int accessibility_custom_action_7 = 2131296296;

			public const int accessibility_custom_action_8 = 2131296297;

			public const int accessibility_custom_action_9 = 2131296298;

			public const int action0 = 2131296299;

			public const int actions = 2131296317;

			public const int action_bar = 2131296300;

			public const int action_bar_activity_content = 2131296301;

			public const int action_bar_container = 2131296302;

			public const int action_bar_root = 2131296303;

			public const int action_bar_spinner = 2131296304;

			public const int action_bar_subtitle = 2131296305;

			public const int action_bar_title = 2131296306;

			public const int action_container = 2131296307;

			public const int action_context_bar = 2131296308;

			public const int action_divider = 2131296309;

			public const int action_image = 2131296310;

			public const int action_menu_divider = 2131296311;

			public const int action_menu_presenter = 2131296312;

			public const int action_mode_bar = 2131296313;

			public const int action_mode_bar_stub = 2131296314;

			public const int action_mode_close_button = 2131296315;

			public const int action_text = 2131296316;

			public const int activityIndicator = 2131296318;

			public const int activity_chooser_view_content = 2131296319;

			public const int activity_feed_send_logfile = 2131296320;

			public const int add = 2131296321;

			public const int adjust_height = 2131296322;

			public const int adjust_width = 2131296323;

			public const int alertTitle = 2131296324;

			public const int all = 2131296325;

			public const int allUsersData = 2131296326;

			public const int all_consents = 2131296327;

			public const int all_consents_no_restart = 2131296328;

			public const int ALT = 2131296256;

			public const int always = 2131296329;

			public const int anonymus = 2131296330;

			public const int app_icon_imageView = 2131296331;

			public const int app_warning_icon = 2131296332;

			public const int arrow_back = 2131296333;

			public const int arrow_back_1 = 2131296334;

			public const int arrow_back_1_view = 2131296335;

			public const int arrow_back_about = 2131296336;

			public const int arrow_back_general = 2131296337;

			public const int arrow_back_help = 2131296338;

			public const int arrow_back_testmode = 2131296339;

			public const int async = 2131296340;

			public const int auto = 2131296341;

			public const int average_contacts_relativeLayout = 2131296342;

			public const int barrier = 2131296343;

			public const int baseline = 2131296344;

			public const int beginning = 2131296345;

			public const int blocking = 2131296346;

			public const int bottom = 2131296347;

			public const int BOTTOM_END = 2131296257;

			public const int BOTTOM_START = 2131296258;

			public const int browser_actions_header_text = 2131296348;

			public const int browser_actions_menu_items = 2131296351;

			public const int browser_actions_menu_item_icon = 2131296349;

			public const int browser_actions_menu_item_text = 2131296350;

			public const int browser_actions_menu_view = 2131296352;

			public const int bubble_layout = 2131296353;

			public const int bubble_message = 2131296354;

			public const int bulletText = 2131296355;

			public const int bullet_one = 2131296356;

			public const int bullet_three = 2131296357;

			public const int bullet_two = 2131296358;

			public const int buttonBubble = 2131296359;

			public const int buttonGetStarted = 2131296360;

			public const int buttonPanel = 2131296361;

			public const int buttonPlane = 2131296362;

			public const int buttonPrev = 2131296363;

			public const int buttonResetConsents = 2131296364;

			public const int buttons = 2131296366;

			public const int buttons_layout = 2131296367;

			public const int button_error = 2131296365;

			public const int cancel_action = 2131296368;

			public const int cancel_button = 2131296369;

			public const int center = 2131296370;

			public const int center_horizontal = 2131296371;

			public const int center_vertical = 2131296372;

			public const int chains = 2131296373;

			public const int checkbox = 2131296374;

			public const int checkbox_layout = 2131296375;

			public const int @checked = 2131296376;

			public const int chip = 2131296377;

			public const int chip_group = 2131296378;

			public const int chronometer = 2131296379;

			public const int clear_text = 2131296380;

			public const int clip_horizontal = 2131296381;

			public const int clip_vertical = 2131296382;

			public const int closeEncountersAmount = 2131296383;

			public const int close_cross_btn = 2131296384;

			public const int collapseActionView = 2131296385;

			public const int column = 2131296386;

			public const int column_reverse = 2131296387;

			public const int confirm_button = 2131296388;

			public const int consentActivityIndicator = 2131296389;

			public const int consent_header = 2131296390;

			public const int consent_info = 2131296391;

			public const int consent_info_layout = 2131296392;

			public const int consent_info_view = 2131296393;

			public const int consent_page_text = 2131296394;

			public const int consent_page_title = 2131296395;

			public const int consent_paragraph_aendringer = 2131296396;

			public const int consent_paragraph_behandlingen = 2131296397;

			public const int consent_paragraph_frivillig_brug = 2131296398;

			public const int consent_paragraph_hvad_registreres = 2131296399;

			public const int consent_paragraph_hvordan_accepterer = 2131296400;

			public const int consent_paragraph_kontaktregistringer = 2131296401;

			public const int consent_paragraph_mere = 2131296402;

			public const int consent_paragraph_policy_btn = 2131296403;

			public const int consent_paragraph_ret = 2131296404;

			public const int consent_paragraph_sadan_fungerer_appen = 2131296405;

			public const int consent_text_layout = 2131296406;

			public const int container = 2131296407;

			public const int content = 2131296408;

			public const int contentPanel = 2131296409;

			public const int continue_in_en_layout = 2131296410;

			public const int continue_in_en_text = 2131296411;

			public const int coordinator = 2131296412;

			public const int counter_explained_text_textView = 2131296413;

			public const int counter_off_text_textView = 2131296414;

			public const int countries_button = 2131296415;

			public const int countries_button_layout = 2131296416;

			public const int countries_layout = 2131296417;

			public const int countries_list = 2131296418;

			public const int countries_subtitle = 2131296419;

			public const int countries_title = 2131296420;

			public const int countries_visibility_layout = 2131296421;

			public const int country_item_caption = 2131296422;

			public const int country_item_checkbox = 2131296423;

			public const int CTRL = 2131296259;

			public const int currentDeviceUUID = 2131296424;

			public const int custom = 2131296425;

			public const int customPanel = 2131296426;

			public const int cut = 2131296427;

			public const int daily_average_number_textView = 2131296428;

			public const int daily_average_text_help_imageButton = 2131296429;

			public const int daily_average_text_layout = 2131296430;

			public const int daily_average_text_textView = 2131296431;

			public const int dark = 2131296432;

			public const int date_picker = 2131296433;

			public const int date_picker_actions = 2131296434;

			public const int decor_content_parent = 2131296435;

			public const int default_activity_button = 2131296436;

			public const int description = 2131296437;

			public const int design_bottom_sheet = 2131296438;

			public const int design_menu_item_action_area = 2131296439;

			public const int design_menu_item_action_area_stub = 2131296440;

			public const int design_menu_item_text = 2131296441;

			public const int design_navigation_view = 2131296442;

			public const int deviceCorrelationID = 2131296443;

			public const int dialog_button = 2131296444;

			public const int dimensions = 2131296445;

			public const int direct = 2131296446;

			public const int disableHome = 2131296447;

			public const int disease_rate_close_cross_btn = 2131296448;

			public const int disease_rate_counter_on_scrollView = 2131296449;

			public const int disease_rate_death_header_text = 2131296450;

			public const int disease_rate_death_number_text = 2131296451;

			public const int disease_rate_death_total_text = 2131296452;

			public const int disease_rate_downloads_header_text = 2131296453;

			public const int disease_rate_downloads_icon = 2131296454;

			public const int disease_rate_downloads_number_text = 2131296455;

			public const int disease_rate_header_textView = 2131296456;

			public const int disease_rate_infected_header_text = 2131296457;

			public const int disease_rate_infected_number_text = 2131296458;

			public const int disease_rate_infected_total_text = 2131296459;

			public const int disease_rate_linear_layout = 2131296460;

			public const int disease_rate_positive_header_text = 2131296461;

			public const int disease_rate_positive_icon = 2131296462;

			public const int disease_rate_positive_number_text = 2131296463;

			public const int disease_rate_positive_total_text = 2131296464;

			public const int disease_rate_relativeLayout = 2131296465;

			public const int disease_rate_sub_header_textView = 2131296466;

			public const int disease_rate_sub_text = 2131296467;

			public const int disease_rate_tested_header_text = 2131296468;

			public const int disease_rate_tested_number_empty_two_text = 2131296469;

			public const int disease_rate_tested_number_text = 2131296470;

			public const int disease_rate_tested_total_text = 2131296471;

			public const int disease_rate_vaccinated_header_text = 2131296472;

			public const int disease_rate_vaccinated_number = 2131296473;

			public const int disease_rate_vaccinated_number_label = 2131296474;

			public const int disease_rate_vaccinated_second_number = 2131296475;

			public const int disease_rate_vaccinated_second_number_label = 2131296476;

			public const int divider = 2131296477;

			public const int dropdown_menu = 2131296478;

			public const int edit_query = 2131296479;

			public const int ellipsis = 2131296480;

			public const int end = 2131296503;

			public const int enDeveloperTools_button_back = 2131296481;

			public const int enDeveloperTools_button_fetchExposureConfiguration = 2131296482;

			public const int enDeveloperTools_button_lastUsedExposureConfiguration = 2131296483;

			public const int enDeveloperTools_button_printLastKeysPulledAndTimestamp = 2131296484;

			public const int enDeveloperTools_button_printLastSymptomOnsetDate = 2131296485;

			public const int enDeveloperTools_button_pullKeys = 2131296486;

			public const int enDeveloperTools_button_pullKeysAndGetExposureInfo = 2131296487;

			public const int enDeveloperTools_button_pushKeys = 2131296488;

			public const int enDeveloperTools_button_resetLocalData = 2131296489;

			public const int enDeveloperTools_button_sendExposureMessage = 2131296490;

			public const int enDeveloperTools_button_sendExposureMessage_after_10_sec = 2131296491;

			public const int enDeveloperTools_button_sendExposureMessage_decrement = 2131296492;

			public const int enDeveloperTools_button_sendExposureMessage_increment = 2131296493;

			public const int enDeveloperTools_button_showLastExposureInfo = 2131296494;

			public const int enDeveloperTools_button_showLastSummary = 2131296495;

			public const int enDeveloperTools_button_showLatestPullKeysTimesAndStatuses = 2131296496;

			public const int enDeveloperTools_button_toggleMessageRetentionLength = 2131296497;

			public const int enDeveloperTools_constraintLayout_betweenGuidelines = 2131296498;

			public const int enDeveloperTools_guideline_left = 2131296499;

			public const int enDeveloperTools_guideline_right = 2131296500;

			public const int enDeveloperTools_textView_devOutput = 2131296501;

			public const int enDeveloperTools_textView_hello = 2131296502;

			public const int end_padder = 2131296504;

			public const int enterAlways = 2131296505;

			public const int enterAlwaysCollapsed = 2131296506;

			public const int error_button = 2131296507;

			public const int error_description = 2131296508;

			public const int error_page_scrollview = 2131296509;

			public const int error_subtitle = 2131296510;

			public const int error_title = 2131296511;

			public const int exitUntilCollapsed = 2131296512;

			public const int expanded_menu = 2131296514;

			public const int expand_activities_button = 2131296513;

			public const int fab = 2131296515;

			public const int fade = 2131296516;

			public const int fake_gateway = 2131296517;

			public const int fill = 2131296518;

			public const int filled = 2131296521;

			public const int fill_horizontal = 2131296519;

			public const int fill_vertical = 2131296520;

			public const int filter_chip = 2131296522;

			public const int firstRadioButton = 2131296523;

			public const int fitToContents = 2131296524;

			public const int @fixed = 2131296525;

			public const int flex_end = 2131296526;

			public const int flex_start = 2131296527;

			public const int footer = 2131296528;

			public const int force_update_button = 2131296529;

			public const int force_update_label = 2131296530;

			public const int forever = 2131296531;

			public const int fourthRadioButton = 2131296532;

			public const int fragment = 2131296533;

			public const int fragment_container_view_tag = 2131296534;

			public const int FUNCTION = 2131296260;

			public const int general_settings = 2131296535;

			public const int ghost_view = 2131296536;

			public const int ghost_view_holder = 2131296537;

			public const int gone = 2131296538;

			public const int groups = 2131296540;

			public const int group_divider = 2131296539;

			public const int guideline = 2131296541;

			public const int guideline_left = 2131296542;

			public const int guideline_right = 2131296543;

			public const int guideline_testmode_left = 2131296544;

			public const int guideline_testmode_right = 2131296545;

			public const int hideable = 2131296546;

			public const int home = 2131296547;

			public const int homeAsUp = 2131296548;

			public const int horizontal_devider = 2131296549;

			public const int icon = 2131296552;

			public const int icon_group = 2131296553;

			public const int icon_only = 2131296554;

			public const int icon_settin = 2131296555;

			public const int ic_close_white = 2131296550;

			public const int ic_start_logo = 2131296551;

			public const int ifRoom = 2131296556;

			public const int image = 2131296557;

			public const int infection_status_activity_status_textView = 2131296558;

			public const int infection_status_activivity_status_description_textView = 2131296559;

			public const int infection_status_app_icon_imageView = 2131296560;

			public const int infection_status_background = 2131296561;

			public const int infection_status_circle_background = 2131296562;

			public const int infection_status_disease_rate_arrow_imageView = 2131296563;

			public const int infection_status_disease_rate_button_relativeLayout = 2131296564;

			public const int infection_status_disease_rate_button_relativeLayout_button = 2131296565;

			public const int infection_status_disease_rate_button_relativeLayout_inner = 2131296566;

			public const int infection_status_disease_rate_imageView = 2131296567;

			public const int infection_status_disease_rate_text_textView = 2131296568;

			public const int infection_status_down_arrow = 2131296569;

			public const int infection_status_down_text = 2131296570;

			public const int infection_status_main_layout = 2131296571;

			public const int infection_status_menu_icon_relativeLayout = 2131296572;

			public const int infection_status_messages_button_relativeLayout = 2131296577;

			public const int infection_status_messages_button_relativeLayout_button = 2131296578;

			public const int infection_status_messages_button_relativeLayout_inner = 2131296579;

			public const int infection_status_message_arrow_imageView = 2131296573;

			public const int infection_status_message_bell_imageView = 2131296574;

			public const int infection_status_message_new_message_imageView = 2131296575;

			public const int infection_status_message_text_textView = 2131296576;

			public const int infection_status_new_disease_rate_text_textView = 2131296580;

			public const int infection_status_new_message_text_textView = 2131296581;

			public const int infection_status_on_off_button = 2131296582;

			public const int infection_status_registration_arrow_imageView = 2131296583;

			public const int infection_status_registration_button_relativeLayout = 2131296584;

			public const int infection_status_registration_button_relativeLayout_button = 2131296585;

			public const int infection_status_registration_button_relativeLayout_inner = 2131296586;

			public const int infection_status_registration_login_text_textView = 2131296587;

			public const int infection_status_registration_text_textView = 2131296588;

			public const int infection_status_registration_virus_imageView = 2131296589;

			public const int infection_status_relativeLayout = 2131296590;

			public const int infection_status_scrollView = 2131296591;

			public const int infection_status_scroll_down_layout = 2131296592;

			public const int info = 2131296593;

			public const int information_consent_body_one_bullet_linearLayout = 2131296594;

			public const int information_consent_body_one_textView = 2131296595;

			public const int information_consent_body_two_bullet_linearLayout = 2131296596;

			public const int information_consent_body_two_textView = 2131296597;

			public const int information_consent_content_textView = 2131296598;

			public const int information_consent_content_two_textView = 2131296599;

			public const int information_consent_header_textView = 2131296600;

			public const int information_consent_nemid_button = 2131296601;

			public const int information_consent_progress_bar = 2131296602;

			public const int information_consent_relativeLayout = 2131296603;

			public const int information_consent_scrollView = 2131296604;

			public const int information_consent_subtitle_textView = 2131296605;

			public const int invisible = 2131296606;

			public const int italic = 2131296607;

			public const int item_touch_helper_previous_elevation = 2131296608;

			public const int labeled = 2131296609;

			public const int largeLabel = 2131296610;

			public const int last_updated_textView = 2131296611;

			public const int launcer_icon_imageview = 2131296612;

			public const int launcher_button = 2131296613;

			public const int left = 2131296614;

			public const int light = 2131296615;

			public const int line1 = 2131296616;

			public const int line3 = 2131296617;

			public const int linearLayout = 2131296618;

			public const int linearLayout4 = 2131296619;

			public const int listMode = 2131296620;

			public const int listViewActivityFeed = 2131296621;

			public const int listViewActivityFeedProximity = 2131296622;

			public const int listViewActivityFeedRssi = 2131296623;

			public const int listViewActivityFeedTimestamp = 2131296624;

			public const int listViewActivityFeedUUID = 2131296625;

			public const int list_item = 2131296626;

			public const int loading_text = 2131296627;

			public const int masked = 2131296628;

			public const int media_actions = 2131296629;

			public const int message = 2131296630;

			public const int messages_devider = 2131296637;

			public const int messages_item_date = 2131296638;

			public const int messages_item_description = 2131296639;

			public const int messages_item_tile = 2131296640;

			public const int messages_list = 2131296641;

			public const int messages_page_title = 2131296642;

			public const int message_article_image = 2131296631;

			public const int message_article_tile = 2131296632;

			public const int message_last_update = 2131296633;

			public const int message_layout = 2131296634;

			public const int message_logo = 2131296635;

			public const int message_tile_title = 2131296636;

			public const int META = 2131296261;

			public const int middle = 2131296643;

			public const int mini = 2131296644;

			public const int month_grid = 2131296645;

			public const int month_navigation_bar = 2131296646;

			public const int month_navigation_fragment_toggle = 2131296647;

			public const int month_navigation_next = 2131296648;

			public const int month_navigation_previous = 2131296649;

			public const int month_title = 2131296650;

			public const int mtrl_calendar_days_of_week = 2131296652;

			public const int mtrl_calendar_day_selector_frame = 2131296651;

			public const int mtrl_calendar_frame = 2131296653;

			public const int mtrl_calendar_main_pane = 2131296654;

			public const int mtrl_calendar_months = 2131296655;

			public const int mtrl_calendar_selection_frame = 2131296656;

			public const int mtrl_calendar_text_input_frame = 2131296657;

			public const int mtrl_calendar_year_selector_frame = 2131296658;

			public const int mtrl_card_checked_layer_id = 2131296659;

			public const int mtrl_child_content_container = 2131296660;

			public const int mtrl_internal_children_alpha_tag = 2131296661;

			public const int mtrl_picker_fullscreen = 2131296662;

			public const int mtrl_picker_header = 2131296663;

			public const int mtrl_picker_header_selection_text = 2131296664;

			public const int mtrl_picker_header_title_and_selection = 2131296665;

			public const int mtrl_picker_header_toggle = 2131296666;

			public const int mtrl_picker_text_input_date = 2131296667;

			public const int mtrl_picker_text_input_range_end = 2131296668;

			public const int mtrl_picker_text_input_range_start = 2131296669;

			public const int mtrl_picker_title_text = 2131296670;

			public const int multiply = 2131296671;

			public const int navigation_header_container = 2131296672;

			public const int nested_scroll = 2131296673;

			public const int never = 2131296674;

			public const int next_button = 2131296675;

			public const int none = 2131296682;

			public const int normal = 2131296683;

			public const int noScroll = 2131296676;

			public const int notification_background = 2131296684;

			public const int notification_main_column = 2131296685;

			public const int notification_main_column_container = 2131296686;

			public const int nowrap = 2131296687;

			public const int no_consents = 2131296677;

			public const int no_consents_no_restart = 2131296678;

			public const int no_items_description = 2131296679;

			public const int no_items_message = 2131296680;

			public const int no_items_title = 2131296681;

			public const int number_counter_imageView = 2131296688;

			public const int number_counter_textView = 2131296689;

			public const int number_counter_text_help_imageButton = 2131296690;

			public const int number_counter_text_textView = 2131296691;

			public const int off = 2131296692;

			public const int ok_button = 2131296693;

			public const int om_frame = 2131296694;

			public const int on = 2131296695;

			public const int only_v1 = 2131296697;

			public const int only_v1_no_restart = 2131296698;

			public const int on_off_button = 2131296696;

			public const int outline = 2131296699;

			public const int packed = 2131296700;

			public const int parallax = 2131296701;

			public const int parent = 2131296702;

			public const int parentPanel = 2131296703;

			public const int parent_matrix = 2131296704;

			public const int password_toggle = 2131296705;

			public const int peekHeight = 2131296706;

			public const int percent = 2131296707;

			public const int picker = 2131296708;

			public const int pin = 2131296709;

			public const int print_actual_preferences = 2131296710;

			public const int progress_bar = 2131296711;

			public const int progress_circular = 2131296712;

			public const int progress_horizontal = 2131296713;

			public const int proximity_status_page_counters_relativeLayout = 2131296717;

			public const int proximity_status_page_counter_off_relativeLayout = 2131296714;

			public const int proximity_status_page_counter_on_relativeLayout = 2131296715;

			public const int proximity_status_page_counter_on_scrollView = 2131296716;

			public const int proximity_status_page_scrollView_relativeLayout = 2131296718;

			public const int proximity_sublayout = 2131296719;

			public const int pull_with_delay = 2131296720;

			public const int questionnaire_button = 2131296721;

			public const int questionnaire_countries_footer = 2131296722;

			public const int questionnaire_info_button = 2131296723;

			public const int questionnaire_subtitle = 2131296724;

			public const int questionnaire_title = 2131296725;

			public const int radio = 2131296726;

			public const int radio_group = 2131296727;

			public const int radio_layout = 2131296728;

			public const int radio_no = 2131296729;

			public const int radio_scroll = 2131296730;

			public const int radio_yes = 2131296731;

			public const int recipe_divider = 2131296732;

			public const int recipe_header = 2131296733;

			public const int recipe_logo = 2131296734;

			public const int recipe_small_text = 2131296735;

			public const int recipe_small_text_layout = 2131296736;

			public const int registered_button = 2131296737;

			public const int registered_content = 2131296738;

			public const int registered_description = 2131296739;

			public const int registered_tick_text = 2131296740;

			public const int registered_title = 2131296741;

			public const int relativeLayout1 = 2131296742;

			public const int right = 2131296743;

			public const int right_icon = 2131296744;

			public const int right_side = 2131296745;

			public const int rounded = 2131296746;

			public const int row = 2131296747;

			public const int row_reverse = 2131296748;

			public const int save_non_transition_alpha = 2131296749;

			public const int save_overlay_view = 2131296750;

			public const int scale = 2131296751;

			public const int screen = 2131296752;

			public const int scroll = 2131296753;

			public const int scrollable = 2131296758;

			public const int scrollIndicatorDown = 2131296754;

			public const int scrollIndicatorUp = 2131296755;

			public const int scrollView = 2131296756;

			public const int scroll_view = 2131296757;

			public const int search_badge = 2131296759;

			public const int search_bar = 2131296760;

			public const int search_button = 2131296761;

			public const int search_close_btn = 2131296762;

			public const int search_edit_frame = 2131296763;

			public const int search_go_btn = 2131296764;

			public const int search_mag_icon = 2131296765;

			public const int search_plate = 2131296766;

			public const int search_src_text = 2131296767;

			public const int search_voice_btn = 2131296768;

			public const int secondRadioButton = 2131296769;

			public const int selected = 2131296771;

			public const int select_dialog_listview = 2131296770;

			public const int settings_about_link = 2131296772;

			public const int settings_about_scroll_layout = 2131296773;

			public const int settings_about_text = 2131296774;

			public const int settings_about_text_layout = 2131296775;

			public const int settings_about_title = 2131296776;

			public const int settings_about_version_info_textview = 2131296777;

			public const int settings_behandling_frame = 2131296778;

			public const int settings_consents_layout = 2131296779;

			public const int settings_general_danish = 2131296780;

			public const int settings_general_divider_bottom = 2131296781;

			public const int settings_general_divider_top = 2131296782;

			public const int settings_general_english = 2131296783;

			public const int settings_general_explanation = 2131296784;

			public const int settings_general_explanation_two = 2131296785;

			public const int settings_general_link = 2131296786;

			public const int settings_general_main_content_layout = 2131296787;

			public const int settings_general_mobile_data_desc = 2131296788;

			public const int settings_general_mobile_data_header = 2131296789;

			public const int settings_general_mobile_data_layout = 2131296790;

			public const int settings_general_scroll_view = 2131296791;

			public const int settings_general_select_lang_desc_one = 2131296792;

			public const int settings_general_select_lang_divider_bottom = 2131296793;

			public const int settings_general_select_lang_divider_top = 2131296794;

			public const int settings_general_select_lang_header = 2131296795;

			public const int settings_general_select_lang_radio_group = 2131296796;

			public const int settings_general_switch = 2131296797;

			public const int settings_general_text_layout = 2131296798;

			public const int settings_general_title = 2131296799;

			public const int settings_help_link = 2131296800;

			public const int settings_help_scroll_layout = 2131296801;

			public const int settings_help_text = 2131296802;

			public const int settings_help_text_layout = 2131296803;

			public const int settings_help_title = 2131296804;

			public const int settings_hjaelp_frame = 2131296805;

			public const int settings_how_it_works_link = 2131296806;

			public const int settings_how_it_works_text = 2131296807;

			public const int settings_how_it_works_text_layout = 2131296808;

			public const int settings_how_it_works_title = 2131296809;

			public const int settings_intro_frame = 2131296810;

			public const int settings_links_layout = 2131296812;

			public const int settings_link_text = 2131296811;

			public const int settings_saddan_frame = 2131296813;

			public const int settings_scroll_frame = 2131296814;

			public const int settings_scroll_help_frame = 2131296815;

			public const int settings_testmode_text_layout = 2131296816;

			public const int settings_version_info_textview = 2131296817;

			public const int SHIFT = 2131296262;

			public const int shortcut = 2131296818;

			public const int showCustom = 2131296819;

			public const int showHome = 2131296820;

			public const int showTitle = 2131296821;

			public const int skipCollapsed = 2131296822;

			public const int slide = 2131296823;

			public const int smallLabel = 2131296824;

			public const int snackbar_action = 2131296825;

			public const int snackbar_text = 2131296826;

			public const int snap = 2131296827;

			public const int snapMargins = 2131296828;

			public const int spacer = 2131296832;

			public const int space_around = 2131296829;

			public const int space_between = 2131296830;

			public const int space_evenly = 2131296831;

			public const int split_action_bar = 2131296833;

			public const int spread = 2131296834;

			public const int spread_inside = 2131296835;

			public const int src_atop = 2131296836;

			public const int src_in = 2131296837;

			public const int src_over = 2131296838;

			public const int standard = 2131296839;

			public const int start = 2131296840;

			public const int status_bar_latest_event_content = 2131296841;

			public const int stretch = 2131296842;

			public const int submenuarrow = 2131296843;

			public const int submit_area = 2131296844;

			public const int switchbar = 2131296845;

			public const int SYM = 2131296263;

			public const int tabDots = 2131296846;

			public const int table1 = 2131296848;

			public const int table2 = 2131296849;

			public const int table3 = 2131296850;

			public const int tabMode = 2131296847;

			public const int tag_accessibility_actions = 2131296851;

			public const int tag_accessibility_clickable_spans = 2131296852;

			public const int tag_accessibility_heading = 2131296853;

			public const int tag_accessibility_pane_title = 2131296854;

			public const int tag_screen_reader_focusable = 2131296855;

			public const int tag_transition_group = 2131296856;

			public const int tag_unhandled_key_event_manager = 2131296857;

			public const int tag_unhandled_key_listeners = 2131296858;

			public const int technology_background = 2131296859;

			public const int test_checkbox_android_button_tint = 2131296860;

			public const int test_checkbox_app_button_tint = 2131296861;

			public const int test_frame = 2131296862;

			public const int text = 2131296863;

			public const int text2 = 2131296864;

			public const int textEnd = 2131296865;

			public const int textinput_counter = 2131296871;

			public const int textinput_error = 2131296872;

			public const int textinput_helper_text = 2131296873;

			public const int textSpacerNoButtons = 2131296866;

			public const int textSpacerNoTitle = 2131296867;

			public const int textStart = 2131296868;

			public const int text_input_end_icon = 2131296869;

			public const int text_input_start_icon = 2131296870;

			public const int thirdRadioButton = 2131296874;

			public const int time = 2131296875;

			public const int title = 2131296876;

			public const int titleDividerNoCustom = 2131296877;

			public const int title_and_updated_date = 2131296878;

			public const int title_layout = 2131296879;

			public const int title_template = 2131296880;

			public const int top = 2131296881;

			public const int topPanel = 2131296882;

			public const int TOP_END = 2131296264;

			public const int top_layout = 2131296883;

			public const int TOP_START = 2131296265;

			public const int touch_outside = 2131296884;

			public const int transition_current_scene = 2131296885;

			public const int transition_layout_save = 2131296886;

			public const int transition_position = 2131296887;

			public const int transition_scene_layoutid_cache = 2131296888;

			public const int transition_transform = 2131296889;

			public const int @unchecked = 2131296890;

			public const int uniform = 2131296891;

			public const int unlabeled = 2131296892;

			public const int unsupported_Transmit_text_textView = 2131296893;

			public const int up = 2131296894;

			public const int useLogo = 2131296895;

			public const int userData = 2131296896;

			public const int vertical_devider = 2131296897;

			public const int view_offset_helper = 2131296898;

			public const int visible = 2131296899;

			public const int visible_removing_fragment_view_tag = 2131296900;

			public const int warningBar = 2131296901;

			public const int warning_layout = 2131296902;

			public const int warning_textView = 2131296903;

			public const int webview = 2131296904;

			public const int welcome_bullet = 2131296905;

			public const int welcome_page_five_button_next = 2131296906;

			public const int welcome_page_five_consent_warning = 2131296907;

			public const int welcome_page_five_consent_warning_text = 2131296908;

			public const int welcome_page_five_prev_button = 2131296909;

			public const int welcome_page_five_switch = 2131296910;

			public const int welcome_page_five_switch_text = 2131296911;

			public const int welcome_page_five_title = 2131296912;

			public const int welcome_page_five_title_layout = 2131296913;

			public const int welcome_page_four_body_one = 2131296914;

			public const int welcome_page_four_body_three = 2131296915;

			public const int welcome_page_four_body_two = 2131296916;

			public const int welcome_page_four_bullet_one_layout = 2131296917;

			public const int welcome_page_four_bullet_three_layout = 2131296918;

			public const int welcome_page_four_bullet_two_layout = 2131296919;

			public const int welcome_page_four_title = 2131296920;

			public const int welcome_page_four_title_layout = 2131296921;

			public const int welcome_page_one_body_one = 2131296922;

			public const int welcome_page_one_body_two = 2131296923;

			public const int welcome_page_one_bullet_one_layout = 2131296924;

			public const int welcome_page_one_bullet_two_layout = 2131296925;

			public const int welcome_page_one_title = 2131296926;

			public const int welcome_page_one_title_layout = 2131296927;

			public const int welcome_page_three_body_one = 2131296928;

			public const int welcome_page_three_body_two = 2131296929;

			public const int welcome_page_three_infobox_body = 2131296930;

			public const int welcome_page_three_title = 2131296931;

			public const int welcome_page_two_body_one = 2131296932;

			public const int welcome_page_two_body_two = 2131296933;

			public const int welcome_page_two_bullet_one_layout = 2131296934;

			public const int welcome_page_two_bullet_two_layout = 2131296935;

			public const int welcome_page_two_title = 2131296936;

			public const int welcome_page_two_title_layout = 2131296937;

			public const int welcome_what_is_new_title = 2131296938;

			public const int wide = 2131296939;

			public const int withText = 2131296940;

			public const int working_schema = 2131296941;

			public const int wrap = 2131296942;

			public const int wrap_content = 2131296943;

			public const int wrap_reverse = 2131296944;

			static Id()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Id()
			{
			}
		}

		public class Integer
		{
			public const int abc_config_activityDefaultDur = 2131361792;

			public const int abc_config_activityShortDur = 2131361793;

			public const int app_bar_elevation_anim_duration = 2131361794;

			public const int bottom_sheet_slide_duration = 2131361795;

			public const int cancel_button_image_alpha = 2131361796;

			public const int config_tooltipAnimTime = 2131361797;

			public const int design_snackbar_text_max_lines = 2131361798;

			public const int design_tab_indicator_anim_duration_ms = 2131361799;

			public const int google_play_services_version = 2131361800;

			public const int hide_password_duration = 2131361801;

			public const int mtrl_badge_max_character_count = 2131361802;

			public const int mtrl_btn_anim_delay_ms = 2131361803;

			public const int mtrl_btn_anim_duration_ms = 2131361804;

			public const int mtrl_calendar_header_orientation = 2131361805;

			public const int mtrl_calendar_selection_text_lines = 2131361806;

			public const int mtrl_calendar_year_selector_span = 2131361807;

			public const int mtrl_card_anim_delay_ms = 2131361808;

			public const int mtrl_card_anim_duration_ms = 2131361809;

			public const int mtrl_chip_anim_duration = 2131361810;

			public const int mtrl_tab_indicator_anim_duration_ms = 2131361811;

			public const int show_password_duration = 2131361812;

			public const int status_bar_notification_info_maxnum = 2131361813;

			static Integer()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Integer()
			{
			}
		}

		public class Interpolator
		{
			public const int btn_checkbox_checked_mtrl_animation_interpolator_0 = 2131427328;

			public const int btn_checkbox_checked_mtrl_animation_interpolator_1 = 2131427329;

			public const int btn_checkbox_unchecked_mtrl_animation_interpolator_0 = 2131427330;

			public const int btn_checkbox_unchecked_mtrl_animation_interpolator_1 = 2131427331;

			public const int btn_radio_to_off_mtrl_animation_interpolator_0 = 2131427332;

			public const int btn_radio_to_on_mtrl_animation_interpolator_0 = 2131427333;

			public const int fast_out_slow_in = 2131427334;

			public const int mtrl_fast_out_linear_in = 2131427335;

			public const int mtrl_fast_out_slow_in = 2131427336;

			public const int mtrl_linear = 2131427337;

			public const int mtrl_linear_out_slow_in = 2131427338;

			static Interpolator()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Interpolator()
			{
			}
		}

		public class Layout
		{
			public const int abc_action_bar_title_item = 2131492864;

			public const int abc_action_bar_up_container = 2131492865;

			public const int abc_action_menu_item_layout = 2131492866;

			public const int abc_action_menu_layout = 2131492867;

			public const int abc_action_mode_bar = 2131492868;

			public const int abc_action_mode_close_item_material = 2131492869;

			public const int abc_activity_chooser_view = 2131492870;

			public const int abc_activity_chooser_view_list_item = 2131492871;

			public const int abc_alert_dialog_button_bar_material = 2131492872;

			public const int abc_alert_dialog_material = 2131492873;

			public const int abc_alert_dialog_title_material = 2131492874;

			public const int abc_cascading_menu_item_layout = 2131492875;

			public const int abc_dialog_title_material = 2131492876;

			public const int abc_expanded_menu_layout = 2131492877;

			public const int abc_list_menu_item_checkbox = 2131492878;

			public const int abc_list_menu_item_icon = 2131492879;

			public const int abc_list_menu_item_layout = 2131492880;

			public const int abc_list_menu_item_radio = 2131492881;

			public const int abc_popup_menu_header_item_layout = 2131492882;

			public const int abc_popup_menu_item_layout = 2131492883;

			public const int abc_screen_content_include = 2131492884;

			public const int abc_screen_simple = 2131492885;

			public const int abc_screen_simple_overlay_action_mode = 2131492886;

			public const int abc_screen_toolbar = 2131492887;

			public const int abc_search_dropdown_item_icons_2line = 2131492888;

			public const int abc_search_view = 2131492889;

			public const int abc_select_dialog_material = 2131492890;

			public const int abc_tooltip = 2131492891;

			public const int activity_disease_rate = 2131492892;

			public const int activity_feed = 2131492893;

			public const int activity_feed_list_item = 2131492894;

			public const int activity_main = 2131492895;

			public const int activity_webview = 2131492896;

			public const int browser_actions_context_menu_page = 2131492897;

			public const int browser_actions_context_menu_row = 2131492898;

			public const int bubble_layout = 2131492899;

			public const int consent_header = 2131492900;

			public const int consent_info = 2131492901;

			public const int consent_paragraph = 2131492902;

			public const int consent_settings_page_body = 2131492903;

			public const int content_main = 2131492904;

			public const int country_view = 2131492905;

			public const int custom_dialog = 2131492906;

			public const int design_bottom_navigation_item = 2131492907;

			public const int design_bottom_sheet_dialog = 2131492908;

			public const int design_layout_snackbar = 2131492909;

			public const int design_layout_snackbar_include = 2131492910;

			public const int design_layout_tab_icon = 2131492911;

			public const int design_layout_tab_text = 2131492912;

			public const int design_menu_item_action_area = 2131492913;

			public const int design_navigation_item = 2131492914;

			public const int design_navigation_item_header = 2131492915;

			public const int design_navigation_item_separator = 2131492916;

			public const int design_navigation_item_subheader = 2131492917;

			public const int design_navigation_menu = 2131492918;

			public const int design_navigation_menu_item = 2131492919;

			public const int design_text_input_end_icon = 2131492920;

			public const int design_text_input_start_icon = 2131492921;

			public const int en_developer_tools = 2131492922;

			public const int error_page = 2131492923;

			public const int force_update = 2131492924;

			public const int infection_status = 2131492925;

			public const int information_and_consent = 2131492926;

			public const int layout_with_launcher_button = 2131492927;

			public const int layout_with_launcher_button_ag_api = 2131492928;

			public const int loading_page = 2131492929;

			public const int messages_list_element = 2131492931;

			public const int messages_page = 2131492932;

			public const int message_title = 2131492930;

			public const int mtrl_alert_dialog = 2131492933;

			public const int mtrl_alert_dialog_actions = 2131492934;

			public const int mtrl_alert_dialog_title = 2131492935;

			public const int mtrl_alert_select_dialog_item = 2131492936;

			public const int mtrl_alert_select_dialog_multichoice = 2131492937;

			public const int mtrl_alert_select_dialog_singlechoice = 2131492938;

			public const int mtrl_calendar_day = 2131492939;

			public const int mtrl_calendar_days_of_week = 2131492941;

			public const int mtrl_calendar_day_of_week = 2131492940;

			public const int mtrl_calendar_horizontal = 2131492942;

			public const int mtrl_calendar_month = 2131492943;

			public const int mtrl_calendar_months = 2131492946;

			public const int mtrl_calendar_month_labeled = 2131492944;

			public const int mtrl_calendar_month_navigation = 2131492945;

			public const int mtrl_calendar_vertical = 2131492947;

			public const int mtrl_calendar_year = 2131492948;

			public const int mtrl_layout_snackbar = 2131492949;

			public const int mtrl_layout_snackbar_include = 2131492950;

			public const int mtrl_picker_actions = 2131492951;

			public const int mtrl_picker_dialog = 2131492952;

			public const int mtrl_picker_fullscreen = 2131492953;

			public const int mtrl_picker_header_dialog = 2131492954;

			public const int mtrl_picker_header_fullscreen = 2131492955;

			public const int mtrl_picker_header_selection_text = 2131492956;

			public const int mtrl_picker_header_title_text = 2131492957;

			public const int mtrl_picker_header_toggle = 2131492958;

			public const int mtrl_picker_text_input_date = 2131492959;

			public const int mtrl_picker_text_input_date_range = 2131492960;

			public const int notification_action = 2131492961;

			public const int notification_action_tombstone = 2131492962;

			public const int notification_media_action = 2131492963;

			public const int notification_media_cancel_action = 2131492964;

			public const int notification_template_big_media = 2131492965;

			public const int notification_template_big_media_custom = 2131492966;

			public const int notification_template_big_media_narrow = 2131492967;

			public const int notification_template_big_media_narrow_custom = 2131492968;

			public const int notification_template_custom_big = 2131492969;

			public const int notification_template_icon_group = 2131492970;

			public const int notification_template_lines_media = 2131492971;

			public const int notification_template_media = 2131492972;

			public const int notification_template_media_custom = 2131492973;

			public const int notification_template_part_chronometer = 2131492974;

			public const int notification_template_part_time = 2131492975;

			public const int proximity_status = 2131492976;

			public const int proximity_sublayout = 2131492977;

			public const int questionnaire_confirm_leave = 2131492978;

			public const int questionnaire_countries = 2131492979;

			public const int questionnaire_page = 2131492980;

			public const int questionnaire_pre_share = 2131492981;

			public const int questionnare = 2131492982;

			public const int registered_page = 2131492983;

			public const int select_dialog_item_material = 2131492984;

			public const int select_dialog_multichoice_material = 2131492985;

			public const int select_dialog_singlechoice_material = 2131492986;

			public const int settings_about = 2131492987;

			public const int settings_about_scroll = 2131492988;

			public const int settings_consents = 2131492989;

			public const int settings_general = 2131492990;

			public const int settings_help = 2131492991;

			public const int settings_help_scroll = 2131492992;

			public const int settings_how_it_works = 2131492993;

			public const int settings_link = 2131492994;

			public const int settings_page = 2131492995;

			public const int spinner_dialog = 2131492996;

			public const int spinner_item = 2131492997;

			public const int support_simple_spinner_dropdown_item = 2131492998;

			public const int test_action_chip = 2131492999;

			public const int test_design_checkbox = 2131493000;

			public const int test_reflow_chipgroup = 2131493001;

			public const int test_toolbar = 2131493002;

			public const int test_toolbar_custom_background = 2131493003;

			public const int test_toolbar_elevation = 2131493004;

			public const int test_toolbar_surface = 2131493005;

			public const int text_view_without_line_height = 2131493010;

			public const int text_view_with_line_height_from_appearance = 2131493006;

			public const int text_view_with_line_height_from_layout = 2131493007;

			public const int text_view_with_line_height_from_style = 2131493008;

			public const int text_view_with_theme_line_height = 2131493009;

			public const int warningbar = 2131493011;

			public const int welcome = 2131493012;

			public const int welcome_bullet = 2131493013;

			public const int welcome_page_five = 2131493014;

			public const int welcome_page_four = 2131493015;

			public const int welcome_page_one = 2131493016;

			public const int welcome_page_three = 2131493017;

			public const int welcome_page_two = 2131493018;

			public const int welcome_what_is_new = 2131493019;

			static Layout()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Layout()
			{
			}
		}

		public class Mipmap
		{
			public const int anonymus = 2131558400;

			public const int bluetooth_icon = 2131558401;

			public const int checkmark = 2131558402;

			public const int health_department_logo = 2131558403;

			public const int ic_help = 2131558404;

			public const int ic_information = 2131558405;

			public const int ic_launcher = 2131558406;

			public const int ic_launcher_foreground = 2131558407;

			public const int ic_launcher_round = 2131558408;

			public const int ic_logo_no_chain = 2131558409;

			public const int ic_person = 2131558410;

			public const int ic_sst_crown_white = 2131558411;

			public const int ic_start_logo = 2131558412;

			public const int sundhedLogo = 2131558413;

			public const int technology_background = 2131558414;

			public const int working_schema = 2131558415;

			static Mipmap()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Mipmap()
			{
			}
		}

		public class Plurals
		{
			public const int mtrl_badge_content_description = 2131623936;

			static Plurals()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Plurals()
			{
			}
		}

		public class String
		{
			public const int abc_action_bar_home_description = 2131689474;

			public const int abc_action_bar_up_description = 2131689475;

			public const int abc_action_menu_overflow_description = 2131689476;

			public const int abc_action_mode_done = 2131689477;

			public const int abc_activitychooserview_choose_application = 2131689479;

			public const int abc_activity_chooser_view_see_all = 2131689478;

			public const int abc_capital_off = 2131689480;

			public const int abc_capital_on = 2131689481;

			public const int abc_menu_alt_shortcut_label = 2131689482;

			public const int abc_menu_ctrl_shortcut_label = 2131689483;

			public const int abc_menu_delete_shortcut_label = 2131689484;

			public const int abc_menu_enter_shortcut_label = 2131689485;

			public const int abc_menu_function_shortcut_label = 2131689486;

			public const int abc_menu_meta_shortcut_label = 2131689487;

			public const int abc_menu_shift_shortcut_label = 2131689488;

			public const int abc_menu_space_shortcut_label = 2131689489;

			public const int abc_menu_sym_shortcut_label = 2131689490;

			public const int abc_prepend_shortcut_label = 2131689491;

			public const int abc_searchview_description_clear = 2131689493;

			public const int abc_searchview_description_query = 2131689494;

			public const int abc_searchview_description_search = 2131689495;

			public const int abc_searchview_description_submit = 2131689496;

			public const int abc_searchview_description_voice = 2131689497;

			public const int abc_search_hint = 2131689492;

			public const int abc_shareactionprovider_share_with = 2131689498;

			public const int abc_shareactionprovider_share_with_application = 2131689499;

			public const int abc_toolbar_collapse_description = 2131689500;

			public const int action_settings = 2131689501;

			public const int appbar_scrolling_view_behavior = 2131689503;

			public const int ApplicationName = 2131689472;

			public const int app_name = 2131689502;

			public const int bottom_sheet_behavior = 2131689504;

			public const int channel_description = 2131689505;

			public const int channel_name = 2131689506;

			public const int character_counter_content_description = 2131689507;

			public const int character_counter_overflowed_content_description = 2131689508;

			public const int character_counter_pattern = 2131689509;

			public const int chip_text = 2131689510;

			public const int clear_text_end_icon_content_description = 2131689511;

			public const int common_google_play_services_enable_button = 2131689512;

			public const int common_google_play_services_enable_text = 2131689513;

			public const int common_google_play_services_enable_title = 2131689514;

			public const int common_google_play_services_install_button = 2131689515;

			public const int common_google_play_services_install_text = 2131689516;

			public const int common_google_play_services_install_title = 2131689517;

			public const int common_google_play_services_notification_channel_name = 2131689518;

			public const int common_google_play_services_notification_ticker = 2131689519;

			public const int common_google_play_services_unknown_issue = 2131689520;

			public const int common_google_play_services_unsupported_text = 2131689521;

			public const int common_google_play_services_update_button = 2131689522;

			public const int common_google_play_services_update_text = 2131689523;

			public const int common_google_play_services_update_title = 2131689524;

			public const int common_google_play_services_updating_text = 2131689525;

			public const int common_google_play_services_wear_update_text = 2131689526;

			public const int common_open_on_phone = 2131689527;

			public const int common_signin_button_text = 2131689528;

			public const int common_signin_button_text_long = 2131689529;

			public const int copy_toast_msg = 2131689530;

			public const int error_icon_content_description = 2131689531;

			public const int exposed_dropdown_menu_content_description = 2131689532;

			public const int fab_transformation_scrim_behavior = 2131689533;

			public const int fab_transformation_sheet_behavior = 2131689534;

			public const int fallback_menu_item_copy_link = 2131689535;

			public const int fallback_menu_item_open_in_browser = 2131689536;

			public const int fallback_menu_item_share_link = 2131689537;

			public const int Hello = 2131689473;

			public const int hide_bottom_view_on_scroll_behavior = 2131689538;

			public const int icon_content_description = 2131689539;

			public const int mtrl_badge_numberless_content_description = 2131689540;

			public const int mtrl_chip_close_icon_content_description = 2131689541;

			public const int mtrl_exceed_max_badge_number_suffix = 2131689542;

			public const int mtrl_picker_a11y_next_month = 2131689543;

			public const int mtrl_picker_a11y_prev_month = 2131689544;

			public const int mtrl_picker_announce_current_selection = 2131689545;

			public const int mtrl_picker_cancel = 2131689546;

			public const int mtrl_picker_confirm = 2131689547;

			public const int mtrl_picker_date_header_selected = 2131689548;

			public const int mtrl_picker_date_header_title = 2131689549;

			public const int mtrl_picker_date_header_unselected = 2131689550;

			public const int mtrl_picker_day_of_week_column_header = 2131689551;

			public const int mtrl_picker_invalid_format = 2131689552;

			public const int mtrl_picker_invalid_format_example = 2131689553;

			public const int mtrl_picker_invalid_format_use = 2131689554;

			public const int mtrl_picker_invalid_range = 2131689555;

			public const int mtrl_picker_navigate_to_year_description = 2131689556;

			public const int mtrl_picker_out_of_range = 2131689557;

			public const int mtrl_picker_range_header_only_end_selected = 2131689558;

			public const int mtrl_picker_range_header_only_start_selected = 2131689559;

			public const int mtrl_picker_range_header_selected = 2131689560;

			public const int mtrl_picker_range_header_title = 2131689561;

			public const int mtrl_picker_range_header_unselected = 2131689562;

			public const int mtrl_picker_save = 2131689563;

			public const int mtrl_picker_text_input_date_hint = 2131689564;

			public const int mtrl_picker_text_input_date_range_end_hint = 2131689565;

			public const int mtrl_picker_text_input_date_range_start_hint = 2131689566;

			public const int mtrl_picker_text_input_day_abbr = 2131689567;

			public const int mtrl_picker_text_input_month_abbr = 2131689568;

			public const int mtrl_picker_text_input_year_abbr = 2131689569;

			public const int mtrl_picker_toggle_to_calendar_input_mode = 2131689570;

			public const int mtrl_picker_toggle_to_day_selection = 2131689571;

			public const int mtrl_picker_toggle_to_text_input_mode = 2131689572;

			public const int mtrl_picker_toggle_to_year_selection = 2131689573;

			public const int package_name = 2131689574;

			public const int password_toggle_content_description = 2131689575;

			public const int path_password_eye = 2131689576;

			public const int path_password_eye_mask_strike_through = 2131689577;

			public const int path_password_eye_mask_visible = 2131689578;

			public const int path_password_strike_through = 2131689579;

			public const int search_menu_title = 2131689580;

			public const int status_bar_notification_info_overflow = 2131689581;

			public const int title_activity_webview = 2131689582;

			static String()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private String()
			{
			}
		}

		public class Style
		{
			public const int AlertDialog_AppCompat = 2131755008;

			public const int AlertDialog_AppCompat_Light = 2131755009;

			public const int Animation_AppCompat_Dialog = 2131755010;

			public const int Animation_AppCompat_DropDownUp = 2131755011;

			public const int Animation_AppCompat_Tooltip = 2131755012;

			public const int Animation_Design_BottomSheetDialog = 2131755013;

			public const int Animation_MaterialComponents_BottomSheetDialog = 2131755014;

			public const int AppTheme = 2131755015;

			public const int AppTheme_AppBarOverlay = 2131755016;

			public const int AppTheme_Launcher = 2131755017;

			public const int AppTheme_PopupOverlay = 2131755018;

			public const int AverageNumber = 2131755019;

			public const int AverageText = 2131755020;

			public const int Base_AlertDialog_AppCompat = 2131755021;

			public const int Base_AlertDialog_AppCompat_Light = 2131755022;

			public const int Base_Animation_AppCompat_Dialog = 2131755023;

			public const int Base_Animation_AppCompat_DropDownUp = 2131755024;

			public const int Base_Animation_AppCompat_Tooltip = 2131755025;

			public const int Base_CardView = 2131755026;

			public const int Base_DialogWindowTitleBackground_AppCompat = 2131755028;

			public const int Base_DialogWindowTitle_AppCompat = 2131755027;

			public const int Base_MaterialAlertDialog_MaterialComponents_Title_Icon = 2131755029;

			public const int Base_MaterialAlertDialog_MaterialComponents_Title_Panel = 2131755030;

			public const int Base_MaterialAlertDialog_MaterialComponents_Title_Text = 2131755031;

			public const int Base_TextAppearance_AppCompat = 2131755032;

			public const int Base_TextAppearance_AppCompat_Body1 = 2131755033;

			public const int Base_TextAppearance_AppCompat_Body2 = 2131755034;

			public const int Base_TextAppearance_AppCompat_Button = 2131755035;

			public const int Base_TextAppearance_AppCompat_Caption = 2131755036;

			public const int Base_TextAppearance_AppCompat_Display1 = 2131755037;

			public const int Base_TextAppearance_AppCompat_Display2 = 2131755038;

			public const int Base_TextAppearance_AppCompat_Display3 = 2131755039;

			public const int Base_TextAppearance_AppCompat_Display4 = 2131755040;

			public const int Base_TextAppearance_AppCompat_Headline = 2131755041;

			public const int Base_TextAppearance_AppCompat_Inverse = 2131755042;

			public const int Base_TextAppearance_AppCompat_Large = 2131755043;

			public const int Base_TextAppearance_AppCompat_Large_Inverse = 2131755044;

			public const int Base_TextAppearance_AppCompat_Light_Widget_PopupMenu_Large = 2131755045;

			public const int Base_TextAppearance_AppCompat_Light_Widget_PopupMenu_Small = 2131755046;

			public const int Base_TextAppearance_AppCompat_Medium = 2131755047;

			public const int Base_TextAppearance_AppCompat_Medium_Inverse = 2131755048;

			public const int Base_TextAppearance_AppCompat_Menu = 2131755049;

			public const int Base_TextAppearance_AppCompat_SearchResult = 2131755050;

			public const int Base_TextAppearance_AppCompat_SearchResult_Subtitle = 2131755051;

			public const int Base_TextAppearance_AppCompat_SearchResult_Title = 2131755052;

			public const int Base_TextAppearance_AppCompat_Small = 2131755053;

			public const int Base_TextAppearance_AppCompat_Small_Inverse = 2131755054;

			public const int Base_TextAppearance_AppCompat_Subhead = 2131755055;

			public const int Base_TextAppearance_AppCompat_Subhead_Inverse = 2131755056;

			public const int Base_TextAppearance_AppCompat_Title = 2131755057;

			public const int Base_TextAppearance_AppCompat_Title_Inverse = 2131755058;

			public const int Base_TextAppearance_AppCompat_Tooltip = 2131755059;

			public const int Base_TextAppearance_AppCompat_Widget_ActionBar_Menu = 2131755060;

			public const int Base_TextAppearance_AppCompat_Widget_ActionBar_Subtitle = 2131755061;

			public const int Base_TextAppearance_AppCompat_Widget_ActionBar_Subtitle_Inverse = 2131755062;

			public const int Base_TextAppearance_AppCompat_Widget_ActionBar_Title = 2131755063;

			public const int Base_TextAppearance_AppCompat_Widget_ActionBar_Title_Inverse = 2131755064;

			public const int Base_TextAppearance_AppCompat_Widget_ActionMode_Subtitle = 2131755065;

			public const int Base_TextAppearance_AppCompat_Widget_ActionMode_Title = 2131755066;

			public const int Base_TextAppearance_AppCompat_Widget_Button = 2131755067;

			public const int Base_TextAppearance_AppCompat_Widget_Button_Borderless_Colored = 2131755068;

			public const int Base_TextAppearance_AppCompat_Widget_Button_Colored = 2131755069;

			public const int Base_TextAppearance_AppCompat_Widget_Button_Inverse = 2131755070;

			public const int Base_TextAppearance_AppCompat_Widget_DropDownItem = 2131755071;

			public const int Base_TextAppearance_AppCompat_Widget_PopupMenu_Header = 2131755072;

			public const int Base_TextAppearance_AppCompat_Widget_PopupMenu_Large = 2131755073;

			public const int Base_TextAppearance_AppCompat_Widget_PopupMenu_Small = 2131755074;

			public const int Base_TextAppearance_AppCompat_Widget_Switch = 2131755075;

			public const int Base_TextAppearance_AppCompat_Widget_TextView_SpinnerItem = 2131755076;

			public const int Base_TextAppearance_MaterialComponents_Badge = 2131755077;

			public const int Base_TextAppearance_MaterialComponents_Button = 2131755078;

			public const int Base_TextAppearance_MaterialComponents_Headline6 = 2131755079;

			public const int Base_TextAppearance_MaterialComponents_Subtitle2 = 2131755080;

			public const int Base_TextAppearance_Widget_AppCompat_ExpandedMenu_Item = 2131755081;

			public const int Base_TextAppearance_Widget_AppCompat_Toolbar_Subtitle = 2131755082;

			public const int Base_TextAppearance_Widget_AppCompat_Toolbar_Title = 2131755083;

			public const int Base_ThemeOverlay_AppCompat = 2131755117;

			public const int Base_ThemeOverlay_AppCompat_ActionBar = 2131755118;

			public const int Base_ThemeOverlay_AppCompat_Dark = 2131755119;

			public const int Base_ThemeOverlay_AppCompat_Dark_ActionBar = 2131755120;

			public const int Base_ThemeOverlay_AppCompat_Dialog = 2131755121;

			public const int Base_ThemeOverlay_AppCompat_Dialog_Alert = 2131755122;

			public const int Base_ThemeOverlay_AppCompat_Light = 2131755123;

			public const int Base_ThemeOverlay_MaterialComponents_Dialog = 2131755124;

			public const int Base_ThemeOverlay_MaterialComponents_Dialog_Alert = 2131755125;

			public const int Base_ThemeOverlay_MaterialComponents_MaterialAlertDialog = 2131755126;

			public const int Base_Theme_AppCompat = 2131755084;

			public const int Base_Theme_AppCompat_CompactMenu = 2131755085;

			public const int Base_Theme_AppCompat_Dialog = 2131755086;

			public const int Base_Theme_AppCompat_DialogWhenLarge = 2131755090;

			public const int Base_Theme_AppCompat_Dialog_Alert = 2131755087;

			public const int Base_Theme_AppCompat_Dialog_FixedSize = 2131755088;

			public const int Base_Theme_AppCompat_Dialog_MinWidth = 2131755089;

			public const int Base_Theme_AppCompat_Light = 2131755091;

			public const int Base_Theme_AppCompat_Light_DarkActionBar = 2131755092;

			public const int Base_Theme_AppCompat_Light_Dialog = 2131755093;

			public const int Base_Theme_AppCompat_Light_DialogWhenLarge = 2131755097;

			public const int Base_Theme_AppCompat_Light_Dialog_Alert = 2131755094;

			public const int Base_Theme_AppCompat_Light_Dialog_FixedSize = 2131755095;

			public const int Base_Theme_AppCompat_Light_Dialog_MinWidth = 2131755096;

			public const int Base_Theme_MaterialComponents = 2131755098;

			public const int Base_Theme_MaterialComponents_Bridge = 2131755099;

			public const int Base_Theme_MaterialComponents_CompactMenu = 2131755100;

			public const int Base_Theme_MaterialComponents_Dialog = 2131755101;

			public const int Base_Theme_MaterialComponents_DialogWhenLarge = 2131755106;

			public const int Base_Theme_MaterialComponents_Dialog_Alert = 2131755102;

			public const int Base_Theme_MaterialComponents_Dialog_Bridge = 2131755103;

			public const int Base_Theme_MaterialComponents_Dialog_FixedSize = 2131755104;

			public const int Base_Theme_MaterialComponents_Dialog_MinWidth = 2131755105;

			public const int Base_Theme_MaterialComponents_Light = 2131755107;

			public const int Base_Theme_MaterialComponents_Light_Bridge = 2131755108;

			public const int Base_Theme_MaterialComponents_Light_DarkActionBar = 2131755109;

			public const int Base_Theme_MaterialComponents_Light_DarkActionBar_Bridge = 2131755110;

			public const int Base_Theme_MaterialComponents_Light_Dialog = 2131755111;

			public const int Base_Theme_MaterialComponents_Light_DialogWhenLarge = 2131755116;

			public const int Base_Theme_MaterialComponents_Light_Dialog_Alert = 2131755112;

			public const int Base_Theme_MaterialComponents_Light_Dialog_Bridge = 2131755113;

			public const int Base_Theme_MaterialComponents_Light_Dialog_FixedSize = 2131755114;

			public const int Base_Theme_MaterialComponents_Light_Dialog_MinWidth = 2131755115;

			public const int Base_V14_ThemeOverlay_MaterialComponents_Dialog = 2131755136;

			public const int Base_V14_ThemeOverlay_MaterialComponents_Dialog_Alert = 2131755137;

			public const int Base_V14_ThemeOverlay_MaterialComponents_MaterialAlertDialog = 2131755138;

			public const int Base_V14_Theme_MaterialComponents = 2131755127;

			public const int Base_V14_Theme_MaterialComponents_Bridge = 2131755128;

			public const int Base_V14_Theme_MaterialComponents_Dialog = 2131755129;

			public const int Base_V14_Theme_MaterialComponents_Dialog_Bridge = 2131755130;

			public const int Base_V14_Theme_MaterialComponents_Light = 2131755131;

			public const int Base_V14_Theme_MaterialComponents_Light_Bridge = 2131755132;

			public const int Base_V14_Theme_MaterialComponents_Light_DarkActionBar_Bridge = 2131755133;

			public const int Base_V14_Theme_MaterialComponents_Light_Dialog = 2131755134;

			public const int Base_V14_Theme_MaterialComponents_Light_Dialog_Bridge = 2131755135;

			public const int Base_V21_ThemeOverlay_AppCompat_Dialog = 2131755143;

			public const int Base_V21_Theme_AppCompat = 2131755139;

			public const int Base_V21_Theme_AppCompat_Dialog = 2131755140;

			public const int Base_V21_Theme_AppCompat_Light = 2131755141;

			public const int Base_V21_Theme_AppCompat_Light_Dialog = 2131755142;

			public const int Base_V22_Theme_AppCompat = 2131755144;

			public const int Base_V22_Theme_AppCompat_Light = 2131755145;

			public const int Base_V23_Theme_AppCompat = 2131755146;

			public const int Base_V23_Theme_AppCompat_Light = 2131755147;

			public const int Base_V26_Theme_AppCompat = 2131755148;

			public const int Base_V26_Theme_AppCompat_Light = 2131755149;

			public const int Base_V26_Widget_AppCompat_Toolbar = 2131755150;

			public const int Base_V28_Theme_AppCompat = 2131755151;

			public const int Base_V28_Theme_AppCompat_Light = 2131755152;

			public const int Base_V7_ThemeOverlay_AppCompat_Dialog = 2131755157;

			public const int Base_V7_Theme_AppCompat = 2131755153;

			public const int Base_V7_Theme_AppCompat_Dialog = 2131755154;

			public const int Base_V7_Theme_AppCompat_Light = 2131755155;

			public const int Base_V7_Theme_AppCompat_Light_Dialog = 2131755156;

			public const int Base_V7_Widget_AppCompat_AutoCompleteTextView = 2131755158;

			public const int Base_V7_Widget_AppCompat_EditText = 2131755159;

			public const int Base_V7_Widget_AppCompat_Toolbar = 2131755160;

			public const int Base_Widget_AppCompat_ActionBar = 2131755161;

			public const int Base_Widget_AppCompat_ActionBar_Solid = 2131755162;

			public const int Base_Widget_AppCompat_ActionBar_TabBar = 2131755163;

			public const int Base_Widget_AppCompat_ActionBar_TabText = 2131755164;

			public const int Base_Widget_AppCompat_ActionBar_TabView = 2131755165;

			public const int Base_Widget_AppCompat_ActionButton = 2131755166;

			public const int Base_Widget_AppCompat_ActionButton_CloseMode = 2131755167;

			public const int Base_Widget_AppCompat_ActionButton_Overflow = 2131755168;

			public const int Base_Widget_AppCompat_ActionMode = 2131755169;

			public const int Base_Widget_AppCompat_ActivityChooserView = 2131755170;

			public const int Base_Widget_AppCompat_AutoCompleteTextView = 2131755171;

			public const int Base_Widget_AppCompat_Button = 2131755172;

			public const int Base_Widget_AppCompat_ButtonBar = 2131755178;

			public const int Base_Widget_AppCompat_ButtonBar_AlertDialog = 2131755179;

			public const int Base_Widget_AppCompat_Button_Borderless = 2131755173;

			public const int Base_Widget_AppCompat_Button_Borderless_Colored = 2131755174;

			public const int Base_Widget_AppCompat_Button_ButtonBar_AlertDialog = 2131755175;

			public const int Base_Widget_AppCompat_Button_Colored = 2131755176;

			public const int Base_Widget_AppCompat_Button_Small = 2131755177;

			public const int Base_Widget_AppCompat_CompoundButton_CheckBox = 2131755180;

			public const int Base_Widget_AppCompat_CompoundButton_RadioButton = 2131755181;

			public const int Base_Widget_AppCompat_CompoundButton_Switch = 2131755182;

			public const int Base_Widget_AppCompat_DrawerArrowToggle = 2131755183;

			public const int Base_Widget_AppCompat_DrawerArrowToggle_Common = 2131755184;

			public const int Base_Widget_AppCompat_DropDownItem_Spinner = 2131755185;

			public const int Base_Widget_AppCompat_EditText = 2131755186;

			public const int Base_Widget_AppCompat_ImageButton = 2131755187;

			public const int Base_Widget_AppCompat_Light_ActionBar = 2131755188;

			public const int Base_Widget_AppCompat_Light_ActionBar_Solid = 2131755189;

			public const int Base_Widget_AppCompat_Light_ActionBar_TabBar = 2131755190;

			public const int Base_Widget_AppCompat_Light_ActionBar_TabText = 2131755191;

			public const int Base_Widget_AppCompat_Light_ActionBar_TabText_Inverse = 2131755192;

			public const int Base_Widget_AppCompat_Light_ActionBar_TabView = 2131755193;

			public const int Base_Widget_AppCompat_Light_PopupMenu = 2131755194;

			public const int Base_Widget_AppCompat_Light_PopupMenu_Overflow = 2131755195;

			public const int Base_Widget_AppCompat_ListMenuView = 2131755196;

			public const int Base_Widget_AppCompat_ListPopupWindow = 2131755197;

			public const int Base_Widget_AppCompat_ListView = 2131755198;

			public const int Base_Widget_AppCompat_ListView_DropDown = 2131755199;

			public const int Base_Widget_AppCompat_ListView_Menu = 2131755200;

			public const int Base_Widget_AppCompat_PopupMenu = 2131755201;

			public const int Base_Widget_AppCompat_PopupMenu_Overflow = 2131755202;

			public const int Base_Widget_AppCompat_PopupWindow = 2131755203;

			public const int Base_Widget_AppCompat_ProgressBar = 2131755204;

			public const int Base_Widget_AppCompat_ProgressBar_Horizontal = 2131755205;

			public const int Base_Widget_AppCompat_RatingBar = 2131755206;

			public const int Base_Widget_AppCompat_RatingBar_Indicator = 2131755207;

			public const int Base_Widget_AppCompat_RatingBar_Small = 2131755208;

			public const int Base_Widget_AppCompat_SearchView = 2131755209;

			public const int Base_Widget_AppCompat_SearchView_ActionBar = 2131755210;

			public const int Base_Widget_AppCompat_SeekBar = 2131755211;

			public const int Base_Widget_AppCompat_SeekBar_Discrete = 2131755212;

			public const int Base_Widget_AppCompat_Spinner = 2131755213;

			public const int Base_Widget_AppCompat_Spinner_Underlined = 2131755214;

			public const int Base_Widget_AppCompat_TextView = 2131755215;

			public const int Base_Widget_AppCompat_TextView_SpinnerItem = 2131755216;

			public const int Base_Widget_AppCompat_Toolbar = 2131755217;

			public const int Base_Widget_AppCompat_Toolbar_Button_Navigation = 2131755218;

			public const int Base_Widget_Design_TabLayout = 2131755219;

			public const int Base_Widget_MaterialComponents_AutoCompleteTextView = 2131755220;

			public const int Base_Widget_MaterialComponents_CheckedTextView = 2131755221;

			public const int Base_Widget_MaterialComponents_Chip = 2131755222;

			public const int Base_Widget_MaterialComponents_PopupMenu = 2131755223;

			public const int Base_Widget_MaterialComponents_PopupMenu_ContextMenu = 2131755224;

			public const int Base_Widget_MaterialComponents_PopupMenu_ListPopupWindow = 2131755225;

			public const int Base_Widget_MaterialComponents_PopupMenu_Overflow = 2131755226;

			public const int Base_Widget_MaterialComponents_TextInputEditText = 2131755227;

			public const int Base_Widget_MaterialComponents_TextInputLayout = 2131755228;

			public const int Base_Widget_MaterialComponents_TextView = 2131755229;

			public const int BubbleText = 2131755230;

			public const int CardView = 2131755231;

			public const int CardView_Dark = 2131755232;

			public const int CardView_Light = 2131755233;

			public const int Checkmark = 2131755234;

			public const int CheckmarkContainer = 2131755235;

			public const int CheckmarkText = 2131755236;

			public const int ConsentButton = 2131755237;

			public const int ConsentButtonNoBorder = 2131755238;

			public const int contactsTodayText = 2131755744;

			public const int CounterBackground = 2131755239;

			public const int counterCircle = 2131755745;

			public const int CounterExplainText = 2131755240;

			public const int counterNumber = 2131755746;

			public const int DefaultButton = 2131755241;

			public const int DefaultButtonGreen = 2131755242;

			public const int DefaultButtonNoBorder = 2131755243;

			public const int DefaultButtonWhite = 2131755244;

			public const int DiseaseRateItemHeaderText = 2131755245;

			public const int DiseaseRateItemNumber = 2131755246;

			public const int DiseaseRateSubText = 2131755247;

			public const int DiseaseRateVaccinationLabel = 2131755248;

			public const int Divider = 2131755249;

			public const int Divider_Horizontal = 2131755250;

			public const int EmptyTheme = 2131755251;

			public const int ErrorText = 2131755252;

			public const int ExplanationTextHeader = 2131755253;

			public const int Footer = 2131755254;

			public const int HeaderSmallText = 2131755255;

			public const int HeaderText = 2131755256;

			public const int HelpText = 2131755257;

			public const int InfectionStatusLayoutButton = 2131755258;

			public const int InfectionStatusOnOffButtonGreen = 2131755259;

			public const int InfectionStatusOnOffButtonRed = 2131755260;

			public const int LastUpdatedText = 2131755261;

			public const int LauncherAppName = 2131755262;

			public const int LauncherHealthAuth = 2131755263;

			public const int LauncherSubtitle = 2131755264;

			public const int MaterialAlertDialog_MaterialComponents = 2131755265;

			public const int MaterialAlertDialog_MaterialComponents_Body_Text = 2131755266;

			public const int MaterialAlertDialog_MaterialComponents_Picker_Date_Calendar = 2131755267;

			public const int MaterialAlertDialog_MaterialComponents_Picker_Date_Spinner = 2131755268;

			public const int MaterialAlertDialog_MaterialComponents_Title_Icon = 2131755269;

			public const int MaterialAlertDialog_MaterialComponents_Title_Icon_CenterStacked = 2131755270;

			public const int MaterialAlertDialog_MaterialComponents_Title_Panel = 2131755271;

			public const int MaterialAlertDialog_MaterialComponents_Title_Panel_CenterStacked = 2131755272;

			public const int MaterialAlertDialog_MaterialComponents_Title_Text = 2131755273;

			public const int MaterialAlertDialog_MaterialComponents_Title_Text_CenterStacked = 2131755274;

			public const int MessageListItemDateText = 2131755275;

			public const int MessageListItemDescriptionText = 2131755276;

			public const int MessageListItemTitleText = 2131755277;

			public const int MessageListLastUpdateText = 2131755278;

			public const int NemIDButton = 2131755279;

			public const int OnOffButton = 2131755280;

			public const int OnOffButtonGreen = 2131755281;

			public const int Platform_AppCompat = 2131755282;

			public const int Platform_AppCompat_Light = 2131755283;

			public const int Platform_MaterialComponents = 2131755284;

			public const int Platform_MaterialComponents_Dialog = 2131755285;

			public const int Platform_MaterialComponents_Light = 2131755286;

			public const int Platform_MaterialComponents_Light_Dialog = 2131755287;

			public const int Platform_ThemeOverlay_AppCompat = 2131755288;

			public const int Platform_ThemeOverlay_AppCompat_Dark = 2131755289;

			public const int Platform_ThemeOverlay_AppCompat_Light = 2131755290;

			public const int Platform_V21_AppCompat = 2131755291;

			public const int Platform_V21_AppCompat_Light = 2131755292;

			public const int Platform_V25_AppCompat = 2131755293;

			public const int Platform_V25_AppCompat_Light = 2131755294;

			public const int Platform_Widget_AppCompat_Spinner = 2131755295;

			public const int PrimaryText = 2131755296;

			public const int PrimaryTextBold = 2131755297;

			public const int PrimaryTextItalic = 2131755298;

			public const int PrimaryTextLight = 2131755299;

			public const int PrimaryTextRegular = 2131755300;

			public const int PrimaryTextSemiBold = 2131755301;

			public const int QuestionnaireDateText = 2131755302;

			public const int RectangleBox = 2131755303;

			public const int RtlOverlay_DialogWindowTitle_AppCompat = 2131755304;

			public const int RtlOverlay_Widget_AppCompat_ActionBar_TitleItem = 2131755305;

			public const int RtlOverlay_Widget_AppCompat_DialogTitle_Icon = 2131755306;

			public const int RtlOverlay_Widget_AppCompat_PopupMenuItem = 2131755307;

			public const int RtlOverlay_Widget_AppCompat_PopupMenuItem_InternalGroup = 2131755308;

			public const int RtlOverlay_Widget_AppCompat_PopupMenuItem_Shortcut = 2131755309;

			public const int RtlOverlay_Widget_AppCompat_PopupMenuItem_SubmenuArrow = 2131755310;

			public const int RtlOverlay_Widget_AppCompat_PopupMenuItem_Text = 2131755311;

			public const int RtlOverlay_Widget_AppCompat_PopupMenuItem_Title = 2131755312;

			public const int RtlOverlay_Widget_AppCompat_SearchView_MagIcon = 2131755318;

			public const int RtlOverlay_Widget_AppCompat_Search_DropDown = 2131755313;

			public const int RtlOverlay_Widget_AppCompat_Search_DropDown_Icon1 = 2131755314;

			public const int RtlOverlay_Widget_AppCompat_Search_DropDown_Icon2 = 2131755315;

			public const int RtlOverlay_Widget_AppCompat_Search_DropDown_Query = 2131755316;

			public const int RtlOverlay_Widget_AppCompat_Search_DropDown_Text = 2131755317;

			public const int RtlUnderlay_Widget_AppCompat_ActionButton = 2131755319;

			public const int RtlUnderlay_Widget_AppCompat_ActionButton_Overflow = 2131755320;

			public const int ScrollbarConsent = 2131755322;

			public const int ScrollScreen = 2131755321;

			public const int SecondaryText = 2131755323;

			public const int settings = 2131755747;

			public const int settings_general = 2131755748;

			public const int ShapeAppearanceOverlay = 2131755329;

			public const int ShapeAppearanceOverlay_BottomLeftDifferentCornerSize = 2131755330;

			public const int ShapeAppearanceOverlay_BottomRightCut = 2131755331;

			public const int ShapeAppearanceOverlay_Cut = 2131755332;

			public const int ShapeAppearanceOverlay_DifferentCornerSize = 2131755333;

			public const int ShapeAppearanceOverlay_MaterialComponents_BottomSheet = 2131755334;

			public const int ShapeAppearanceOverlay_MaterialComponents_Chip = 2131755335;

			public const int ShapeAppearanceOverlay_MaterialComponents_ExtendedFloatingActionButton = 2131755336;

			public const int ShapeAppearanceOverlay_MaterialComponents_FloatingActionButton = 2131755337;

			public const int ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Day = 2131755338;

			public const int ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Window_Fullscreen = 2131755339;

			public const int ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Year = 2131755340;

			public const int ShapeAppearanceOverlay_MaterialComponents_TextInputLayout_FilledBox = 2131755341;

			public const int ShapeAppearanceOverlay_TopLeftCut = 2131755342;

			public const int ShapeAppearanceOverlay_TopRightDifferentCornerSize = 2131755343;

			public const int ShapeAppearance_MaterialComponents = 2131755324;

			public const int ShapeAppearance_MaterialComponents_LargeComponent = 2131755325;

			public const int ShapeAppearance_MaterialComponents_MediumComponent = 2131755326;

			public const int ShapeAppearance_MaterialComponents_SmallComponent = 2131755327;

			public const int ShapeAppearance_MaterialComponents_Test = 2131755328;

			public const int SwitchPlaneStyle = 2131755344;

			public const int SwitchTextStyle = 2131755345;

			public const int TestStyleWithLineHeight = 2131755351;

			public const int TestStyleWithLineHeightAppearance = 2131755352;

			public const int TestStyleWithoutLineHeight = 2131755354;

			public const int TestStyleWithThemeLineHeightAttribute = 2131755353;

			public const int TestThemeWithLineHeight = 2131755355;

			public const int TestThemeWithLineHeightDisabled = 2131755356;

			public const int Test_ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Day = 2131755346;

			public const int Test_Theme_MaterialComponents_MaterialCalendar = 2131755347;

			public const int Test_Widget_MaterialComponents_MaterialCalendar = 2131755348;

			public const int Test_Widget_MaterialComponents_MaterialCalendar_Day = 2131755349;

			public const int Test_Widget_MaterialComponents_MaterialCalendar_Day_Selected = 2131755350;

			public const int TextAppearance_AppCompat = 2131755357;

			public const int TextAppearance_AppCompat_Body1 = 2131755358;

			public const int TextAppearance_AppCompat_Body2 = 2131755359;

			public const int TextAppearance_AppCompat_Button = 2131755360;

			public const int TextAppearance_AppCompat_Caption = 2131755361;

			public const int TextAppearance_AppCompat_Display1 = 2131755362;

			public const int TextAppearance_AppCompat_Display2 = 2131755363;

			public const int TextAppearance_AppCompat_Display3 = 2131755364;

			public const int TextAppearance_AppCompat_Display4 = 2131755365;

			public const int TextAppearance_AppCompat_Headline = 2131755366;

			public const int TextAppearance_AppCompat_Inverse = 2131755367;

			public const int TextAppearance_AppCompat_Large = 2131755368;

			public const int TextAppearance_AppCompat_Large_Inverse = 2131755369;

			public const int TextAppearance_AppCompat_Light_SearchResult_Subtitle = 2131755370;

			public const int TextAppearance_AppCompat_Light_SearchResult_Title = 2131755371;

			public const int TextAppearance_AppCompat_Light_Widget_PopupMenu_Large = 2131755372;

			public const int TextAppearance_AppCompat_Light_Widget_PopupMenu_Small = 2131755373;

			public const int TextAppearance_AppCompat_Medium = 2131755374;

			public const int TextAppearance_AppCompat_Medium_Inverse = 2131755375;

			public const int TextAppearance_AppCompat_Menu = 2131755376;

			public const int TextAppearance_AppCompat_SearchResult_Subtitle = 2131755377;

			public const int TextAppearance_AppCompat_SearchResult_Title = 2131755378;

			public const int TextAppearance_AppCompat_Small = 2131755379;

			public const int TextAppearance_AppCompat_Small_Inverse = 2131755380;

			public const int TextAppearance_AppCompat_Subhead = 2131755381;

			public const int TextAppearance_AppCompat_Subhead_Inverse = 2131755382;

			public const int TextAppearance_AppCompat_Title = 2131755383;

			public const int TextAppearance_AppCompat_Title_Inverse = 2131755384;

			public const int TextAppearance_AppCompat_Tooltip = 2131755385;

			public const int TextAppearance_AppCompat_Widget_ActionBar_Menu = 2131755386;

			public const int TextAppearance_AppCompat_Widget_ActionBar_Subtitle = 2131755387;

			public const int TextAppearance_AppCompat_Widget_ActionBar_Subtitle_Inverse = 2131755388;

			public const int TextAppearance_AppCompat_Widget_ActionBar_Title = 2131755389;

			public const int TextAppearance_AppCompat_Widget_ActionBar_Title_Inverse = 2131755390;

			public const int TextAppearance_AppCompat_Widget_ActionMode_Subtitle = 2131755391;

			public const int TextAppearance_AppCompat_Widget_ActionMode_Subtitle_Inverse = 2131755392;

			public const int TextAppearance_AppCompat_Widget_ActionMode_Title = 2131755393;

			public const int TextAppearance_AppCompat_Widget_ActionMode_Title_Inverse = 2131755394;

			public const int TextAppearance_AppCompat_Widget_Button = 2131755395;

			public const int TextAppearance_AppCompat_Widget_Button_Borderless_Colored = 2131755396;

			public const int TextAppearance_AppCompat_Widget_Button_Colored = 2131755397;

			public const int TextAppearance_AppCompat_Widget_Button_Inverse = 2131755398;

			public const int TextAppearance_AppCompat_Widget_DropDownItem = 2131755399;

			public const int TextAppearance_AppCompat_Widget_PopupMenu_Header = 2131755400;

			public const int TextAppearance_AppCompat_Widget_PopupMenu_Large = 2131755401;

			public const int TextAppearance_AppCompat_Widget_PopupMenu_Small = 2131755402;

			public const int TextAppearance_AppCompat_Widget_Switch = 2131755403;

			public const int TextAppearance_AppCompat_Widget_TextView_SpinnerItem = 2131755404;

			public const int TextAppearance_Compat_Notification = 2131755405;

			public const int TextAppearance_Compat_Notification_Info = 2131755406;

			public const int TextAppearance_Compat_Notification_Info_Media = 2131755407;

			public const int TextAppearance_Compat_Notification_Line2 = 2131755408;

			public const int TextAppearance_Compat_Notification_Line2_Media = 2131755409;

			public const int TextAppearance_Compat_Notification_Media = 2131755410;

			public const int TextAppearance_Compat_Notification_Time = 2131755411;

			public const int TextAppearance_Compat_Notification_Time_Media = 2131755412;

			public const int TextAppearance_Compat_Notification_Title = 2131755413;

			public const int TextAppearance_Compat_Notification_Title_Media = 2131755414;

			public const int TextAppearance_Design_CollapsingToolbar_Expanded = 2131755415;

			public const int TextAppearance_Design_Counter = 2131755416;

			public const int TextAppearance_Design_Counter_Overflow = 2131755417;

			public const int TextAppearance_Design_Error = 2131755418;

			public const int TextAppearance_Design_HelperText = 2131755419;

			public const int TextAppearance_Design_Hint = 2131755420;

			public const int TextAppearance_Design_Snackbar_Message = 2131755421;

			public const int TextAppearance_Design_Tab = 2131755422;

			public const int TextAppearance_MaterialComponents_Badge = 2131755423;

			public const int TextAppearance_MaterialComponents_Body1 = 2131755424;

			public const int TextAppearance_MaterialComponents_Body2 = 2131755425;

			public const int TextAppearance_MaterialComponents_Button = 2131755426;

			public const int TextAppearance_MaterialComponents_Caption = 2131755427;

			public const int TextAppearance_MaterialComponents_Chip = 2131755428;

			public const int TextAppearance_MaterialComponents_Headline1 = 2131755429;

			public const int TextAppearance_MaterialComponents_Headline2 = 2131755430;

			public const int TextAppearance_MaterialComponents_Headline3 = 2131755431;

			public const int TextAppearance_MaterialComponents_Headline4 = 2131755432;

			public const int TextAppearance_MaterialComponents_Headline5 = 2131755433;

			public const int TextAppearance_MaterialComponents_Headline6 = 2131755434;

			public const int TextAppearance_MaterialComponents_Overline = 2131755435;

			public const int TextAppearance_MaterialComponents_Subtitle1 = 2131755436;

			public const int TextAppearance_MaterialComponents_Subtitle2 = 2131755437;

			public const int TextAppearance_Widget_AppCompat_ExpandedMenu_Item = 2131755438;

			public const int TextAppearance_Widget_AppCompat_Toolbar_Subtitle = 2131755439;

			public const int TextAppearance_Widget_AppCompat_Toolbar_Title = 2131755440;

			public const int ThemeOverlay_AppCompat = 2131755517;

			public const int ThemeOverlay_AppCompat_ActionBar = 2131755518;

			public const int ThemeOverlay_AppCompat_Dark = 2131755519;

			public const int ThemeOverlay_AppCompat_Dark_ActionBar = 2131755520;

			public const int ThemeOverlay_AppCompat_DayNight = 2131755521;

			public const int ThemeOverlay_AppCompat_DayNight_ActionBar = 2131755522;

			public const int ThemeOverlay_AppCompat_Dialog = 2131755523;

			public const int ThemeOverlay_AppCompat_Dialog_Alert = 2131755524;

			public const int ThemeOverlay_AppCompat_Light = 2131755525;

			public const int ThemeOverlay_Design_TextInputEditText = 2131755526;

			public const int ThemeOverlay_MaterialComponents = 2131755527;

			public const int ThemeOverlay_MaterialComponents_ActionBar = 2131755528;

			public const int ThemeOverlay_MaterialComponents_ActionBar_Primary = 2131755529;

			public const int ThemeOverlay_MaterialComponents_ActionBar_Surface = 2131755530;

			public const int ThemeOverlay_MaterialComponents_AutoCompleteTextView = 2131755531;

			public const int ThemeOverlay_MaterialComponents_AutoCompleteTextView_FilledBox = 2131755532;

			public const int ThemeOverlay_MaterialComponents_AutoCompleteTextView_FilledBox_Dense = 2131755533;

			public const int ThemeOverlay_MaterialComponents_AutoCompleteTextView_OutlinedBox = 2131755534;

			public const int ThemeOverlay_MaterialComponents_AutoCompleteTextView_OutlinedBox_Dense = 2131755535;

			public const int ThemeOverlay_MaterialComponents_BottomAppBar_Primary = 2131755536;

			public const int ThemeOverlay_MaterialComponents_BottomAppBar_Surface = 2131755537;

			public const int ThemeOverlay_MaterialComponents_BottomSheetDialog = 2131755538;

			public const int ThemeOverlay_MaterialComponents_Dark = 2131755539;

			public const int ThemeOverlay_MaterialComponents_Dark_ActionBar = 2131755540;

			public const int ThemeOverlay_MaterialComponents_DayNight_BottomSheetDialog = 2131755541;

			public const int ThemeOverlay_MaterialComponents_Dialog = 2131755542;

			public const int ThemeOverlay_MaterialComponents_Dialog_Alert = 2131755543;

			public const int ThemeOverlay_MaterialComponents_Light = 2131755544;

			public const int ThemeOverlay_MaterialComponents_Light_BottomSheetDialog = 2131755545;

			public const int ThemeOverlay_MaterialComponents_MaterialAlertDialog = 2131755546;

			public const int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Centered = 2131755547;

			public const int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date = 2131755548;

			public const int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Calendar = 2131755549;

			public const int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Header_Text = 2131755550;

			public const int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Header_Text_Day = 2131755551;

			public const int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Spinner = 2131755552;

			public const int ThemeOverlay_MaterialComponents_MaterialCalendar = 2131755553;

			public const int ThemeOverlay_MaterialComponents_MaterialCalendar_Fullscreen = 2131755554;

			public const int ThemeOverlay_MaterialComponents_TextInputEditText = 2131755555;

			public const int ThemeOverlay_MaterialComponents_TextInputEditText_FilledBox = 2131755556;

			public const int ThemeOverlay_MaterialComponents_TextInputEditText_FilledBox_Dense = 2131755557;

			public const int ThemeOverlay_MaterialComponents_TextInputEditText_OutlinedBox = 2131755558;

			public const int ThemeOverlay_MaterialComponents_TextInputEditText_OutlinedBox_Dense = 2131755559;

			public const int ThemeOverlay_MaterialComponents_Toolbar_Primary = 2131755560;

			public const int ThemeOverlay_MaterialComponents_Toolbar_Surface = 2131755561;

			public const int Theme_AppCompat = 2131755441;

			public const int Theme_AppCompat_CompactMenu = 2131755442;

			public const int Theme_AppCompat_DayNight = 2131755443;

			public const int Theme_AppCompat_DayNight_DarkActionBar = 2131755444;

			public const int Theme_AppCompat_DayNight_Dialog = 2131755445;

			public const int Theme_AppCompat_DayNight_DialogWhenLarge = 2131755448;

			public const int Theme_AppCompat_DayNight_Dialog_Alert = 2131755446;

			public const int Theme_AppCompat_DayNight_Dialog_MinWidth = 2131755447;

			public const int Theme_AppCompat_DayNight_NoActionBar = 2131755449;

			public const int Theme_AppCompat_Dialog = 2131755450;

			public const int Theme_AppCompat_DialogWhenLarge = 2131755453;

			public const int Theme_AppCompat_Dialog_Alert = 2131755451;

			public const int Theme_AppCompat_Dialog_MinWidth = 2131755452;

			public const int Theme_AppCompat_Light = 2131755454;

			public const int Theme_AppCompat_Light_DarkActionBar = 2131755455;

			public const int Theme_AppCompat_Light_Dialog = 2131755456;

			public const int Theme_AppCompat_Light_DialogWhenLarge = 2131755459;

			public const int Theme_AppCompat_Light_Dialog_Alert = 2131755457;

			public const int Theme_AppCompat_Light_Dialog_MinWidth = 2131755458;

			public const int Theme_AppCompat_Light_NoActionBar = 2131755460;

			public const int Theme_AppCompat_NoActionBar = 2131755461;

			public const int Theme_Design = 2131755462;

			public const int Theme_Design_BottomSheetDialog = 2131755463;

			public const int Theme_Design_Light = 2131755464;

			public const int Theme_Design_Light_BottomSheetDialog = 2131755465;

			public const int Theme_Design_Light_NoActionBar = 2131755466;

			public const int Theme_Design_NoActionBar = 2131755467;

			public const int Theme_MaterialComponents = 2131755468;

			public const int Theme_MaterialComponents_BottomSheetDialog = 2131755469;

			public const int Theme_MaterialComponents_Bridge = 2131755470;

			public const int Theme_MaterialComponents_CompactMenu = 2131755471;

			public const int Theme_MaterialComponents_DayNight = 2131755472;

			public const int Theme_MaterialComponents_DayNight_BottomSheetDialog = 2131755473;

			public const int Theme_MaterialComponents_DayNight_Bridge = 2131755474;

			public const int Theme_MaterialComponents_DayNight_DarkActionBar = 2131755475;

			public const int Theme_MaterialComponents_DayNight_DarkActionBar_Bridge = 2131755476;

			public const int Theme_MaterialComponents_DayNight_Dialog = 2131755477;

			public const int Theme_MaterialComponents_DayNight_DialogWhenLarge = 2131755485;

			public const int Theme_MaterialComponents_DayNight_Dialog_Alert = 2131755478;

			public const int Theme_MaterialComponents_DayNight_Dialog_Alert_Bridge = 2131755479;

			public const int Theme_MaterialComponents_DayNight_Dialog_Bridge = 2131755480;

			public const int Theme_MaterialComponents_DayNight_Dialog_FixedSize = 2131755481;

			public const int Theme_MaterialComponents_DayNight_Dialog_FixedSize_Bridge = 2131755482;

			public const int Theme_MaterialComponents_DayNight_Dialog_MinWidth = 2131755483;

			public const int Theme_MaterialComponents_DayNight_Dialog_MinWidth_Bridge = 2131755484;

			public const int Theme_MaterialComponents_DayNight_NoActionBar = 2131755486;

			public const int Theme_MaterialComponents_DayNight_NoActionBar_Bridge = 2131755487;

			public const int Theme_MaterialComponents_Dialog = 2131755488;

			public const int Theme_MaterialComponents_DialogWhenLarge = 2131755496;

			public const int Theme_MaterialComponents_Dialog_Alert = 2131755489;

			public const int Theme_MaterialComponents_Dialog_Alert_Bridge = 2131755490;

			public const int Theme_MaterialComponents_Dialog_Bridge = 2131755491;

			public const int Theme_MaterialComponents_Dialog_FixedSize = 2131755492;

			public const int Theme_MaterialComponents_Dialog_FixedSize_Bridge = 2131755493;

			public const int Theme_MaterialComponents_Dialog_MinWidth = 2131755494;

			public const int Theme_MaterialComponents_Dialog_MinWidth_Bridge = 2131755495;

			public const int Theme_MaterialComponents_Light = 2131755497;

			public const int Theme_MaterialComponents_Light_BarSize = 2131755498;

			public const int Theme_MaterialComponents_Light_BottomSheetDialog = 2131755499;

			public const int Theme_MaterialComponents_Light_Bridge = 2131755500;

			public const int Theme_MaterialComponents_Light_DarkActionBar = 2131755501;

			public const int Theme_MaterialComponents_Light_DarkActionBar_Bridge = 2131755502;

			public const int Theme_MaterialComponents_Light_Dialog = 2131755503;

			public const int Theme_MaterialComponents_Light_DialogWhenLarge = 2131755511;

			public const int Theme_MaterialComponents_Light_Dialog_Alert = 2131755504;

			public const int Theme_MaterialComponents_Light_Dialog_Alert_Bridge = 2131755505;

			public const int Theme_MaterialComponents_Light_Dialog_Bridge = 2131755506;

			public const int Theme_MaterialComponents_Light_Dialog_FixedSize = 2131755507;

			public const int Theme_MaterialComponents_Light_Dialog_FixedSize_Bridge = 2131755508;

			public const int Theme_MaterialComponents_Light_Dialog_MinWidth = 2131755509;

			public const int Theme_MaterialComponents_Light_Dialog_MinWidth_Bridge = 2131755510;

			public const int Theme_MaterialComponents_Light_LargeTouch = 2131755512;

			public const int Theme_MaterialComponents_Light_NoActionBar = 2131755513;

			public const int Theme_MaterialComponents_Light_NoActionBar_Bridge = 2131755514;

			public const int Theme_MaterialComponents_NoActionBar = 2131755515;

			public const int Theme_MaterialComponents_NoActionBar_Bridge = 2131755516;

			public const int TopbarText = 2131755562;

			public const int UnsupportedText = 2131755563;

			public const int WarningText = 2131755564;

			public const int WhatIsNewFooter = 2131755565;

			public const int Widget_AppCompat_ActionBar = 2131755566;

			public const int Widget_AppCompat_ActionBar_Solid = 2131755567;

			public const int Widget_AppCompat_ActionBar_TabBar = 2131755568;

			public const int Widget_AppCompat_ActionBar_TabText = 2131755569;

			public const int Widget_AppCompat_ActionBar_TabView = 2131755570;

			public const int Widget_AppCompat_ActionButton = 2131755571;

			public const int Widget_AppCompat_ActionButton_CloseMode = 2131755572;

			public const int Widget_AppCompat_ActionButton_Overflow = 2131755573;

			public const int Widget_AppCompat_ActionMode = 2131755574;

			public const int Widget_AppCompat_ActivityChooserView = 2131755575;

			public const int Widget_AppCompat_AutoCompleteTextView = 2131755576;

			public const int Widget_AppCompat_Button = 2131755577;

			public const int Widget_AppCompat_ButtonBar = 2131755583;

			public const int Widget_AppCompat_ButtonBar_AlertDialog = 2131755584;

			public const int Widget_AppCompat_Button_Borderless = 2131755578;

			public const int Widget_AppCompat_Button_Borderless_Colored = 2131755579;

			public const int Widget_AppCompat_Button_ButtonBar_AlertDialog = 2131755580;

			public const int Widget_AppCompat_Button_Colored = 2131755581;

			public const int Widget_AppCompat_Button_Small = 2131755582;

			public const int Widget_AppCompat_CompoundButton_CheckBox = 2131755585;

			public const int Widget_AppCompat_CompoundButton_RadioButton = 2131755586;

			public const int Widget_AppCompat_CompoundButton_Switch = 2131755587;

			public const int Widget_AppCompat_DrawerArrowToggle = 2131755588;

			public const int Widget_AppCompat_DropDownItem_Spinner = 2131755589;

			public const int Widget_AppCompat_EditText = 2131755590;

			public const int Widget_AppCompat_ImageButton = 2131755591;

			public const int Widget_AppCompat_Light_ActionBar = 2131755592;

			public const int Widget_AppCompat_Light_ActionBar_Solid = 2131755593;

			public const int Widget_AppCompat_Light_ActionBar_Solid_Inverse = 2131755594;

			public const int Widget_AppCompat_Light_ActionBar_TabBar = 2131755595;

			public const int Widget_AppCompat_Light_ActionBar_TabBar_Inverse = 2131755596;

			public const int Widget_AppCompat_Light_ActionBar_TabText = 2131755597;

			public const int Widget_AppCompat_Light_ActionBar_TabText_Inverse = 2131755598;

			public const int Widget_AppCompat_Light_ActionBar_TabView = 2131755599;

			public const int Widget_AppCompat_Light_ActionBar_TabView_Inverse = 2131755600;

			public const int Widget_AppCompat_Light_ActionButton = 2131755601;

			public const int Widget_AppCompat_Light_ActionButton_CloseMode = 2131755602;

			public const int Widget_AppCompat_Light_ActionButton_Overflow = 2131755603;

			public const int Widget_AppCompat_Light_ActionMode_Inverse = 2131755604;

			public const int Widget_AppCompat_Light_ActivityChooserView = 2131755605;

			public const int Widget_AppCompat_Light_AutoCompleteTextView = 2131755606;

			public const int Widget_AppCompat_Light_DropDownItem_Spinner = 2131755607;

			public const int Widget_AppCompat_Light_ListPopupWindow = 2131755608;

			public const int Widget_AppCompat_Light_ListView_DropDown = 2131755609;

			public const int Widget_AppCompat_Light_PopupMenu = 2131755610;

			public const int Widget_AppCompat_Light_PopupMenu_Overflow = 2131755611;

			public const int Widget_AppCompat_Light_SearchView = 2131755612;

			public const int Widget_AppCompat_Light_Spinner_DropDown_ActionBar = 2131755613;

			public const int Widget_AppCompat_ListMenuView = 2131755614;

			public const int Widget_AppCompat_ListPopupWindow = 2131755615;

			public const int Widget_AppCompat_ListView = 2131755616;

			public const int Widget_AppCompat_ListView_DropDown = 2131755617;

			public const int Widget_AppCompat_ListView_Menu = 2131755618;

			public const int Widget_AppCompat_PopupMenu = 2131755619;

			public const int Widget_AppCompat_PopupMenu_Overflow = 2131755620;

			public const int Widget_AppCompat_PopupWindow = 2131755621;

			public const int Widget_AppCompat_ProgressBar = 2131755622;

			public const int Widget_AppCompat_ProgressBar_Horizontal = 2131755623;

			public const int Widget_AppCompat_RatingBar = 2131755624;

			public const int Widget_AppCompat_RatingBar_Indicator = 2131755625;

			public const int Widget_AppCompat_RatingBar_Small = 2131755626;

			public const int Widget_AppCompat_SearchView = 2131755627;

			public const int Widget_AppCompat_SearchView_ActionBar = 2131755628;

			public const int Widget_AppCompat_SeekBar = 2131755629;

			public const int Widget_AppCompat_SeekBar_Discrete = 2131755630;

			public const int Widget_AppCompat_Spinner = 2131755631;

			public const int Widget_AppCompat_Spinner_DropDown = 2131755632;

			public const int Widget_AppCompat_Spinner_DropDown_ActionBar = 2131755633;

			public const int Widget_AppCompat_Spinner_Underlined = 2131755634;

			public const int Widget_AppCompat_TextView = 2131755635;

			public const int Widget_AppCompat_TextView_SpinnerItem = 2131755636;

			public const int Widget_AppCompat_Toolbar = 2131755637;

			public const int Widget_AppCompat_Toolbar_Button_Navigation = 2131755638;

			public const int Widget_Compat_NotificationActionContainer = 2131755639;

			public const int Widget_Compat_NotificationActionText = 2131755640;

			public const int Widget_Design_AppBarLayout = 2131755641;

			public const int Widget_Design_BottomNavigationView = 2131755642;

			public const int Widget_Design_BottomSheet_Modal = 2131755643;

			public const int Widget_Design_CollapsingToolbar = 2131755644;

			public const int Widget_Design_FloatingActionButton = 2131755645;

			public const int Widget_Design_NavigationView = 2131755646;

			public const int Widget_Design_ScrimInsetsFrameLayout = 2131755647;

			public const int Widget_Design_Snackbar = 2131755648;

			public const int Widget_Design_TabLayout = 2131755649;

			public const int Widget_Design_TextInputLayout = 2131755650;

			public const int Widget_MaterialComponents_ActionBar_Primary = 2131755651;

			public const int Widget_MaterialComponents_ActionBar_PrimarySurface = 2131755652;

			public const int Widget_MaterialComponents_ActionBar_Solid = 2131755653;

			public const int Widget_MaterialComponents_ActionBar_Surface = 2131755654;

			public const int Widget_MaterialComponents_AppBarLayout_Primary = 2131755655;

			public const int Widget_MaterialComponents_AppBarLayout_PrimarySurface = 2131755656;

			public const int Widget_MaterialComponents_AppBarLayout_Surface = 2131755657;

			public const int Widget_MaterialComponents_AutoCompleteTextView_FilledBox = 2131755658;

			public const int Widget_MaterialComponents_AutoCompleteTextView_FilledBox_Dense = 2131755659;

			public const int Widget_MaterialComponents_AutoCompleteTextView_OutlinedBox = 2131755660;

			public const int Widget_MaterialComponents_AutoCompleteTextView_OutlinedBox_Dense = 2131755661;

			public const int Widget_MaterialComponents_Badge = 2131755662;

			public const int Widget_MaterialComponents_BottomAppBar = 2131755663;

			public const int Widget_MaterialComponents_BottomAppBar_Colored = 2131755664;

			public const int Widget_MaterialComponents_BottomAppBar_PrimarySurface = 2131755665;

			public const int Widget_MaterialComponents_BottomNavigationView = 2131755666;

			public const int Widget_MaterialComponents_BottomNavigationView_Colored = 2131755667;

			public const int Widget_MaterialComponents_BottomNavigationView_PrimarySurface = 2131755668;

			public const int Widget_MaterialComponents_BottomSheet = 2131755669;

			public const int Widget_MaterialComponents_BottomSheet_Modal = 2131755670;

			public const int Widget_MaterialComponents_Button = 2131755671;

			public const int Widget_MaterialComponents_Button_Icon = 2131755672;

			public const int Widget_MaterialComponents_Button_OutlinedButton = 2131755673;

			public const int Widget_MaterialComponents_Button_OutlinedButton_Icon = 2131755674;

			public const int Widget_MaterialComponents_Button_TextButton = 2131755675;

			public const int Widget_MaterialComponents_Button_TextButton_Dialog = 2131755676;

			public const int Widget_MaterialComponents_Button_TextButton_Dialog_Flush = 2131755677;

			public const int Widget_MaterialComponents_Button_TextButton_Dialog_Icon = 2131755678;

			public const int Widget_MaterialComponents_Button_TextButton_Icon = 2131755679;

			public const int Widget_MaterialComponents_Button_TextButton_Snackbar = 2131755680;

			public const int Widget_MaterialComponents_Button_UnelevatedButton = 2131755681;

			public const int Widget_MaterialComponents_Button_UnelevatedButton_Icon = 2131755682;

			public const int Widget_MaterialComponents_CardView = 2131755683;

			public const int Widget_MaterialComponents_CheckedTextView = 2131755684;

			public const int Widget_MaterialComponents_ChipGroup = 2131755689;

			public const int Widget_MaterialComponents_Chip_Action = 2131755685;

			public const int Widget_MaterialComponents_Chip_Choice = 2131755686;

			public const int Widget_MaterialComponents_Chip_Entry = 2131755687;

			public const int Widget_MaterialComponents_Chip_Filter = 2131755688;

			public const int Widget_MaterialComponents_CompoundButton_CheckBox = 2131755690;

			public const int Widget_MaterialComponents_CompoundButton_RadioButton = 2131755691;

			public const int Widget_MaterialComponents_CompoundButton_Switch = 2131755692;

			public const int Widget_MaterialComponents_ExtendedFloatingActionButton = 2131755693;

			public const int Widget_MaterialComponents_ExtendedFloatingActionButton_Icon = 2131755694;

			public const int Widget_MaterialComponents_FloatingActionButton = 2131755695;

			public const int Widget_MaterialComponents_Light_ActionBar_Solid = 2131755696;

			public const int Widget_MaterialComponents_MaterialButtonToggleGroup = 2131755697;

			public const int Widget_MaterialComponents_MaterialCalendar = 2131755698;

			public const int Widget_MaterialComponents_MaterialCalendar_Day = 2131755699;

			public const int Widget_MaterialComponents_MaterialCalendar_DayTextView = 2131755703;

			public const int Widget_MaterialComponents_MaterialCalendar_Day_Invalid = 2131755700;

			public const int Widget_MaterialComponents_MaterialCalendar_Day_Selected = 2131755701;

			public const int Widget_MaterialComponents_MaterialCalendar_Day_Today = 2131755702;

			public const int Widget_MaterialComponents_MaterialCalendar_Fullscreen = 2131755704;

			public const int Widget_MaterialComponents_MaterialCalendar_HeaderConfirmButton = 2131755705;

			public const int Widget_MaterialComponents_MaterialCalendar_HeaderDivider = 2131755706;

			public const int Widget_MaterialComponents_MaterialCalendar_HeaderLayout = 2131755707;

			public const int Widget_MaterialComponents_MaterialCalendar_HeaderSelection = 2131755708;

			public const int Widget_MaterialComponents_MaterialCalendar_HeaderSelection_Fullscreen = 2131755709;

			public const int Widget_MaterialComponents_MaterialCalendar_HeaderTitle = 2131755710;

			public const int Widget_MaterialComponents_MaterialCalendar_HeaderToggleButton = 2131755711;

			public const int Widget_MaterialComponents_MaterialCalendar_Item = 2131755712;

			public const int Widget_MaterialComponents_MaterialCalendar_Year = 2131755713;

			public const int Widget_MaterialComponents_MaterialCalendar_Year_Selected = 2131755714;

			public const int Widget_MaterialComponents_MaterialCalendar_Year_Today = 2131755715;

			public const int Widget_MaterialComponents_NavigationView = 2131755716;

			public const int Widget_MaterialComponents_PopupMenu = 2131755717;

			public const int Widget_MaterialComponents_PopupMenu_ContextMenu = 2131755718;

			public const int Widget_MaterialComponents_PopupMenu_ListPopupWindow = 2131755719;

			public const int Widget_MaterialComponents_PopupMenu_Overflow = 2131755720;

			public const int Widget_MaterialComponents_Snackbar = 2131755721;

			public const int Widget_MaterialComponents_Snackbar_FullWidth = 2131755722;

			public const int Widget_MaterialComponents_TabLayout = 2131755723;

			public const int Widget_MaterialComponents_TabLayout_Colored = 2131755724;

			public const int Widget_MaterialComponents_TabLayout_PrimarySurface = 2131755725;

			public const int Widget_MaterialComponents_TextInputEditText_FilledBox = 2131755726;

			public const int Widget_MaterialComponents_TextInputEditText_FilledBox_Dense = 2131755727;

			public const int Widget_MaterialComponents_TextInputEditText_OutlinedBox = 2131755728;

			public const int Widget_MaterialComponents_TextInputEditText_OutlinedBox_Dense = 2131755729;

			public const int Widget_MaterialComponents_TextInputLayout_FilledBox = 2131755730;

			public const int Widget_MaterialComponents_TextInputLayout_FilledBox_Dense = 2131755731;

			public const int Widget_MaterialComponents_TextInputLayout_FilledBox_Dense_ExposedDropdownMenu = 2131755732;

			public const int Widget_MaterialComponents_TextInputLayout_FilledBox_ExposedDropdownMenu = 2131755733;

			public const int Widget_MaterialComponents_TextInputLayout_OutlinedBox = 2131755734;

			public const int Widget_MaterialComponents_TextInputLayout_OutlinedBox_Dense = 2131755735;

			public const int Widget_MaterialComponents_TextInputLayout_OutlinedBox_Dense_ExposedDropdownMenu = 2131755736;

			public const int Widget_MaterialComponents_TextInputLayout_OutlinedBox_ExposedDropdownMenu = 2131755737;

			public const int Widget_MaterialComponents_TextView = 2131755738;

			public const int Widget_MaterialComponents_Toolbar = 2131755739;

			public const int Widget_MaterialComponents_Toolbar_Primary = 2131755740;

			public const int Widget_MaterialComponents_Toolbar_PrimarySurface = 2131755741;

			public const int Widget_MaterialComponents_Toolbar_Surface = 2131755742;

			public const int Widget_Support_CoordinatorLayout = 2131755743;

			static Style()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Style()
			{
			}
		}

		public class Styleable
		{
			public static int[] ActionBar;

			public static int[] ActionBarLayout;

			public const int ActionBarLayout_android_layout_gravity = 0;

			public const int ActionBar_background = 0;

			public const int ActionBar_backgroundSplit = 1;

			public const int ActionBar_backgroundStacked = 2;

			public const int ActionBar_contentInsetEnd = 3;

			public const int ActionBar_contentInsetEndWithActions = 4;

			public const int ActionBar_contentInsetLeft = 5;

			public const int ActionBar_contentInsetRight = 6;

			public const int ActionBar_contentInsetStart = 7;

			public const int ActionBar_contentInsetStartWithNavigation = 8;

			public const int ActionBar_customNavigationLayout = 9;

			public const int ActionBar_displayOptions = 10;

			public const int ActionBar_divider = 11;

			public const int ActionBar_elevation = 12;

			public const int ActionBar_height = 13;

			public const int ActionBar_hideOnContentScroll = 14;

			public const int ActionBar_homeAsUpIndicator = 15;

			public const int ActionBar_homeLayout = 16;

			public const int ActionBar_icon = 17;

			public const int ActionBar_indeterminateProgressStyle = 18;

			public const int ActionBar_itemPadding = 19;

			public const int ActionBar_logo = 20;

			public const int ActionBar_navigationMode = 21;

			public const int ActionBar_popupTheme = 22;

			public const int ActionBar_progressBarPadding = 23;

			public const int ActionBar_progressBarStyle = 24;

			public const int ActionBar_subtitle = 25;

			public const int ActionBar_subtitleTextStyle = 26;

			public const int ActionBar_title = 27;

			public const int ActionBar_titleTextStyle = 28;

			public static int[] ActionMenuItemView;

			public const int ActionMenuItemView_android_minWidth = 0;

			public static int[] ActionMenuView;

			public static int[] ActionMode;

			public const int ActionMode_background = 0;

			public const int ActionMode_backgroundSplit = 1;

			public const int ActionMode_closeItemLayout = 2;

			public const int ActionMode_height = 3;

			public const int ActionMode_subtitleTextStyle = 4;

			public const int ActionMode_titleTextStyle = 5;

			public static int[] ActivityChooserView;

			public const int ActivityChooserView_expandActivityOverflowButtonDrawable = 0;

			public const int ActivityChooserView_initialActivityCount = 1;

			public static int[] AlertDialog;

			public const int AlertDialog_android_layout = 0;

			public const int AlertDialog_buttonIconDimen = 1;

			public const int AlertDialog_buttonPanelSideLayout = 2;

			public const int AlertDialog_listItemLayout = 3;

			public const int AlertDialog_listLayout = 4;

			public const int AlertDialog_multiChoiceItemLayout = 5;

			public const int AlertDialog_showTitle = 6;

			public const int AlertDialog_singleChoiceItemLayout = 7;

			public static int[] AnimatedStateListDrawableCompat;

			public const int AnimatedStateListDrawableCompat_android_constantSize = 3;

			public const int AnimatedStateListDrawableCompat_android_dither = 0;

			public const int AnimatedStateListDrawableCompat_android_enterFadeDuration = 4;

			public const int AnimatedStateListDrawableCompat_android_exitFadeDuration = 5;

			public const int AnimatedStateListDrawableCompat_android_variablePadding = 2;

			public const int AnimatedStateListDrawableCompat_android_visible = 1;

			public static int[] AnimatedStateListDrawableItem;

			public const int AnimatedStateListDrawableItem_android_drawable = 1;

			public const int AnimatedStateListDrawableItem_android_id = 0;

			public static int[] AnimatedStateListDrawableTransition;

			public const int AnimatedStateListDrawableTransition_android_drawable = 0;

			public const int AnimatedStateListDrawableTransition_android_fromId = 2;

			public const int AnimatedStateListDrawableTransition_android_reversible = 3;

			public const int AnimatedStateListDrawableTransition_android_toId = 1;

			public static int[] AppBarLayout;

			public static int[] AppBarLayoutStates;

			public const int AppBarLayoutStates_state_collapsed = 0;

			public const int AppBarLayoutStates_state_collapsible = 1;

			public const int AppBarLayoutStates_state_liftable = 2;

			public const int AppBarLayoutStates_state_lifted = 3;

			public const int AppBarLayout_android_background = 0;

			public const int AppBarLayout_android_keyboardNavigationCluster = 2;

			public const int AppBarLayout_android_touchscreenBlocksFocus = 1;

			public const int AppBarLayout_elevation = 3;

			public const int AppBarLayout_expanded = 4;

			public static int[] AppBarLayout_Layout;

			public const int AppBarLayout_Layout_layout_scrollFlags = 0;

			public const int AppBarLayout_Layout_layout_scrollInterpolator = 1;

			public const int AppBarLayout_liftOnScroll = 5;

			public const int AppBarLayout_liftOnScrollTargetViewId = 6;

			public const int AppBarLayout_statusBarForeground = 7;

			public static int[] AppCompatImageView;

			public const int AppCompatImageView_android_src = 0;

			public const int AppCompatImageView_srcCompat = 1;

			public const int AppCompatImageView_tint = 2;

			public const int AppCompatImageView_tintMode = 3;

			public static int[] AppCompatSeekBar;

			public const int AppCompatSeekBar_android_thumb = 0;

			public const int AppCompatSeekBar_tickMark = 1;

			public const int AppCompatSeekBar_tickMarkTint = 2;

			public const int AppCompatSeekBar_tickMarkTintMode = 3;

			public static int[] AppCompatTextHelper;

			public const int AppCompatTextHelper_android_drawableBottom = 2;

			public const int AppCompatTextHelper_android_drawableEnd = 6;

			public const int AppCompatTextHelper_android_drawableLeft = 3;

			public const int AppCompatTextHelper_android_drawableRight = 4;

			public const int AppCompatTextHelper_android_drawableStart = 5;

			public const int AppCompatTextHelper_android_drawableTop = 1;

			public const int AppCompatTextHelper_android_textAppearance = 0;

			public static int[] AppCompatTextView;

			public const int AppCompatTextView_android_textAppearance = 0;

			public const int AppCompatTextView_autoSizeMaxTextSize = 1;

			public const int AppCompatTextView_autoSizeMinTextSize = 2;

			public const int AppCompatTextView_autoSizePresetSizes = 3;

			public const int AppCompatTextView_autoSizeStepGranularity = 4;

			public const int AppCompatTextView_autoSizeTextType = 5;

			public const int AppCompatTextView_drawableBottomCompat = 6;

			public const int AppCompatTextView_drawableEndCompat = 7;

			public const int AppCompatTextView_drawableLeftCompat = 8;

			public const int AppCompatTextView_drawableRightCompat = 9;

			public const int AppCompatTextView_drawableStartCompat = 10;

			public const int AppCompatTextView_drawableTint = 11;

			public const int AppCompatTextView_drawableTintMode = 12;

			public const int AppCompatTextView_drawableTopCompat = 13;

			public const int AppCompatTextView_firstBaselineToTopHeight = 14;

			public const int AppCompatTextView_fontFamily = 15;

			public const int AppCompatTextView_fontVariationSettings = 16;

			public const int AppCompatTextView_lastBaselineToBottomHeight = 17;

			public const int AppCompatTextView_lineHeight = 18;

			public const int AppCompatTextView_textAllCaps = 19;

			public const int AppCompatTextView_textLocale = 20;

			public static int[] AppCompatTheme;

			public const int AppCompatTheme_actionBarDivider = 2;

			public const int AppCompatTheme_actionBarItemBackground = 3;

			public const int AppCompatTheme_actionBarPopupTheme = 4;

			public const int AppCompatTheme_actionBarSize = 5;

			public const int AppCompatTheme_actionBarSplitStyle = 6;

			public const int AppCompatTheme_actionBarStyle = 7;

			public const int AppCompatTheme_actionBarTabBarStyle = 8;

			public const int AppCompatTheme_actionBarTabStyle = 9;

			public const int AppCompatTheme_actionBarTabTextStyle = 10;

			public const int AppCompatTheme_actionBarTheme = 11;

			public const int AppCompatTheme_actionBarWidgetTheme = 12;

			public const int AppCompatTheme_actionButtonStyle = 13;

			public const int AppCompatTheme_actionDropDownStyle = 14;

			public const int AppCompatTheme_actionMenuTextAppearance = 15;

			public const int AppCompatTheme_actionMenuTextColor = 16;

			public const int AppCompatTheme_actionModeBackground = 17;

			public const int AppCompatTheme_actionModeCloseButtonStyle = 18;

			public const int AppCompatTheme_actionModeCloseDrawable = 19;

			public const int AppCompatTheme_actionModeCopyDrawable = 20;

			public const int AppCompatTheme_actionModeCutDrawable = 21;

			public const int AppCompatTheme_actionModeFindDrawable = 22;

			public const int AppCompatTheme_actionModePasteDrawable = 23;

			public const int AppCompatTheme_actionModePopupWindowStyle = 24;

			public const int AppCompatTheme_actionModeSelectAllDrawable = 25;

			public const int AppCompatTheme_actionModeShareDrawable = 26;

			public const int AppCompatTheme_actionModeSplitBackground = 27;

			public const int AppCompatTheme_actionModeStyle = 28;

			public const int AppCompatTheme_actionModeWebSearchDrawable = 29;

			public const int AppCompatTheme_actionOverflowButtonStyle = 30;

			public const int AppCompatTheme_actionOverflowMenuStyle = 31;

			public const int AppCompatTheme_activityChooserViewStyle = 32;

			public const int AppCompatTheme_alertDialogButtonGroupStyle = 33;

			public const int AppCompatTheme_alertDialogCenterButtons = 34;

			public const int AppCompatTheme_alertDialogStyle = 35;

			public const int AppCompatTheme_alertDialogTheme = 36;

			public const int AppCompatTheme_android_windowAnimationStyle = 1;

			public const int AppCompatTheme_android_windowIsFloating = 0;

			public const int AppCompatTheme_autoCompleteTextViewStyle = 37;

			public const int AppCompatTheme_borderlessButtonStyle = 38;

			public const int AppCompatTheme_buttonBarButtonStyle = 39;

			public const int AppCompatTheme_buttonBarNegativeButtonStyle = 40;

			public const int AppCompatTheme_buttonBarNeutralButtonStyle = 41;

			public const int AppCompatTheme_buttonBarPositiveButtonStyle = 42;

			public const int AppCompatTheme_buttonBarStyle = 43;

			public const int AppCompatTheme_buttonStyle = 44;

			public const int AppCompatTheme_buttonStyleSmall = 45;

			public const int AppCompatTheme_checkboxStyle = 46;

			public const int AppCompatTheme_checkedTextViewStyle = 47;

			public const int AppCompatTheme_colorAccent = 48;

			public const int AppCompatTheme_colorBackgroundFloating = 49;

			public const int AppCompatTheme_colorButtonNormal = 50;

			public const int AppCompatTheme_colorControlActivated = 51;

			public const int AppCompatTheme_colorControlHighlight = 52;

			public const int AppCompatTheme_colorControlNormal = 53;

			public const int AppCompatTheme_colorError = 54;

			public const int AppCompatTheme_colorPrimary = 55;

			public const int AppCompatTheme_colorPrimaryDark = 56;

			public const int AppCompatTheme_colorSwitchThumbNormal = 57;

			public const int AppCompatTheme_controlBackground = 58;

			public const int AppCompatTheme_dialogCornerRadius = 59;

			public const int AppCompatTheme_dialogPreferredPadding = 60;

			public const int AppCompatTheme_dialogTheme = 61;

			public const int AppCompatTheme_dividerHorizontal = 62;

			public const int AppCompatTheme_dividerVertical = 63;

			public const int AppCompatTheme_dropdownListPreferredItemHeight = 65;

			public const int AppCompatTheme_dropDownListViewStyle = 64;

			public const int AppCompatTheme_editTextBackground = 66;

			public const int AppCompatTheme_editTextColor = 67;

			public const int AppCompatTheme_editTextStyle = 68;

			public const int AppCompatTheme_homeAsUpIndicator = 69;

			public const int AppCompatTheme_imageButtonStyle = 70;

			public const int AppCompatTheme_listChoiceBackgroundIndicator = 71;

			public const int AppCompatTheme_listChoiceIndicatorMultipleAnimated = 72;

			public const int AppCompatTheme_listChoiceIndicatorSingleAnimated = 73;

			public const int AppCompatTheme_listDividerAlertDialog = 74;

			public const int AppCompatTheme_listMenuViewStyle = 75;

			public const int AppCompatTheme_listPopupWindowStyle = 76;

			public const int AppCompatTheme_listPreferredItemHeight = 77;

			public const int AppCompatTheme_listPreferredItemHeightLarge = 78;

			public const int AppCompatTheme_listPreferredItemHeightSmall = 79;

			public const int AppCompatTheme_listPreferredItemPaddingEnd = 80;

			public const int AppCompatTheme_listPreferredItemPaddingLeft = 81;

			public const int AppCompatTheme_listPreferredItemPaddingRight = 82;

			public const int AppCompatTheme_listPreferredItemPaddingStart = 83;

			public const int AppCompatTheme_panelBackground = 84;

			public const int AppCompatTheme_panelMenuListTheme = 85;

			public const int AppCompatTheme_panelMenuListWidth = 86;

			public const int AppCompatTheme_popupMenuStyle = 87;

			public const int AppCompatTheme_popupWindowStyle = 88;

			public const int AppCompatTheme_radioButtonStyle = 89;

			public const int AppCompatTheme_ratingBarStyle = 90;

			public const int AppCompatTheme_ratingBarStyleIndicator = 91;

			public const int AppCompatTheme_ratingBarStyleSmall = 92;

			public const int AppCompatTheme_searchViewStyle = 93;

			public const int AppCompatTheme_seekBarStyle = 94;

			public const int AppCompatTheme_selectableItemBackground = 95;

			public const int AppCompatTheme_selectableItemBackgroundBorderless = 96;

			public const int AppCompatTheme_spinnerDropDownItemStyle = 97;

			public const int AppCompatTheme_spinnerStyle = 98;

			public const int AppCompatTheme_switchStyle = 99;

			public const int AppCompatTheme_textAppearanceLargePopupMenu = 100;

			public const int AppCompatTheme_textAppearanceListItem = 101;

			public const int AppCompatTheme_textAppearanceListItemSecondary = 102;

			public const int AppCompatTheme_textAppearanceListItemSmall = 103;

			public const int AppCompatTheme_textAppearancePopupMenuHeader = 104;

			public const int AppCompatTheme_textAppearanceSearchResultSubtitle = 105;

			public const int AppCompatTheme_textAppearanceSearchResultTitle = 106;

			public const int AppCompatTheme_textAppearanceSmallPopupMenu = 107;

			public const int AppCompatTheme_textColorAlertDialogListItem = 108;

			public const int AppCompatTheme_textColorSearchUrl = 109;

			public const int AppCompatTheme_toolbarNavigationButtonStyle = 110;

			public const int AppCompatTheme_toolbarStyle = 111;

			public const int AppCompatTheme_tooltipForegroundColor = 112;

			public const int AppCompatTheme_tooltipFrameBackground = 113;

			public const int AppCompatTheme_viewInflaterClass = 114;

			public const int AppCompatTheme_windowActionBar = 115;

			public const int AppCompatTheme_windowActionBarOverlay = 116;

			public const int AppCompatTheme_windowActionModeOverlay = 117;

			public const int AppCompatTheme_windowFixedHeightMajor = 118;

			public const int AppCompatTheme_windowFixedHeightMinor = 119;

			public const int AppCompatTheme_windowFixedWidthMajor = 120;

			public const int AppCompatTheme_windowFixedWidthMinor = 121;

			public const int AppCompatTheme_windowMinWidthMajor = 122;

			public const int AppCompatTheme_windowMinWidthMinor = 123;

			public const int AppCompatTheme_windowNoTitle = 124;

			public static int[] Badge;

			public const int Badge_backgroundColor = 0;

			public const int Badge_badgeGravity = 1;

			public const int Badge_badgeTextColor = 2;

			public const int Badge_maxCharacterCount = 3;

			public const int Badge_number = 4;

			public static int[] BottomAppBar;

			public const int BottomAppBar_backgroundTint = 0;

			public const int BottomAppBar_elevation = 1;

			public const int BottomAppBar_fabAlignmentMode = 2;

			public const int BottomAppBar_fabAnimationMode = 3;

			public const int BottomAppBar_fabCradleMargin = 4;

			public const int BottomAppBar_fabCradleRoundedCornerRadius = 5;

			public const int BottomAppBar_fabCradleVerticalOffset = 6;

			public const int BottomAppBar_hideOnScroll = 7;

			public static int[] BottomNavigationView;

			public const int BottomNavigationView_backgroundTint = 0;

			public const int BottomNavigationView_elevation = 1;

			public const int BottomNavigationView_itemBackground = 2;

			public const int BottomNavigationView_itemHorizontalTranslationEnabled = 3;

			public const int BottomNavigationView_itemIconSize = 4;

			public const int BottomNavigationView_itemIconTint = 5;

			public const int BottomNavigationView_itemRippleColor = 6;

			public const int BottomNavigationView_itemTextAppearanceActive = 7;

			public const int BottomNavigationView_itemTextAppearanceInactive = 8;

			public const int BottomNavigationView_itemTextColor = 9;

			public const int BottomNavigationView_labelVisibilityMode = 10;

			public const int BottomNavigationView_menu = 11;

			public static int[] BottomSheetBehavior_Layout;

			public const int BottomSheetBehavior_Layout_android_elevation = 0;

			public const int BottomSheetBehavior_Layout_backgroundTint = 1;

			public const int BottomSheetBehavior_Layout_behavior_expandedOffset = 2;

			public const int BottomSheetBehavior_Layout_behavior_fitToContents = 3;

			public const int BottomSheetBehavior_Layout_behavior_halfExpandedRatio = 4;

			public const int BottomSheetBehavior_Layout_behavior_hideable = 5;

			public const int BottomSheetBehavior_Layout_behavior_peekHeight = 6;

			public const int BottomSheetBehavior_Layout_behavior_saveFlags = 7;

			public const int BottomSheetBehavior_Layout_behavior_skipCollapsed = 8;

			public const int BottomSheetBehavior_Layout_shapeAppearance = 9;

			public const int BottomSheetBehavior_Layout_shapeAppearanceOverlay = 10;

			public static int[] ButtonBarLayout;

			public const int ButtonBarLayout_allowStacking = 0;

			public static int[] CardView;

			public const int CardView_android_minHeight = 1;

			public const int CardView_android_minWidth = 0;

			public const int CardView_cardBackgroundColor = 2;

			public const int CardView_cardCornerRadius = 3;

			public const int CardView_cardElevation = 4;

			public const int CardView_cardMaxElevation = 5;

			public const int CardView_cardPreventCornerOverlap = 6;

			public const int CardView_cardUseCompatPadding = 7;

			public const int CardView_contentPadding = 8;

			public const int CardView_contentPaddingBottom = 9;

			public const int CardView_contentPaddingLeft = 10;

			public const int CardView_contentPaddingRight = 11;

			public const int CardView_contentPaddingTop = 12;

			public static int[] Chip;

			public static int[] ChipGroup;

			public const int ChipGroup_checkedChip = 0;

			public const int ChipGroup_chipSpacing = 1;

			public const int ChipGroup_chipSpacingHorizontal = 2;

			public const int ChipGroup_chipSpacingVertical = 3;

			public const int ChipGroup_singleLine = 4;

			public const int ChipGroup_singleSelection = 5;

			public const int Chip_android_checkable = 5;

			public const int Chip_android_ellipsize = 2;

			public const int Chip_android_maxWidth = 3;

			public const int Chip_android_text = 4;

			public const int Chip_android_textAppearance = 0;

			public const int Chip_android_textColor = 1;

			public const int Chip_checkedIcon = 6;

			public const int Chip_checkedIconEnabled = 7;

			public const int Chip_checkedIconVisible = 8;

			public const int Chip_chipBackgroundColor = 9;

			public const int Chip_chipCornerRadius = 10;

			public const int Chip_chipEndPadding = 11;

			public const int Chip_chipIcon = 12;

			public const int Chip_chipIconEnabled = 13;

			public const int Chip_chipIconSize = 14;

			public const int Chip_chipIconTint = 15;

			public const int Chip_chipIconVisible = 16;

			public const int Chip_chipMinHeight = 17;

			public const int Chip_chipMinTouchTargetSize = 18;

			public const int Chip_chipStartPadding = 19;

			public const int Chip_chipStrokeColor = 20;

			public const int Chip_chipStrokeWidth = 21;

			public const int Chip_chipSurfaceColor = 22;

			public const int Chip_closeIcon = 23;

			public const int Chip_closeIconEnabled = 24;

			public const int Chip_closeIconEndPadding = 25;

			public const int Chip_closeIconSize = 26;

			public const int Chip_closeIconStartPadding = 27;

			public const int Chip_closeIconTint = 28;

			public const int Chip_closeIconVisible = 29;

			public const int Chip_ensureMinTouchTargetSize = 30;

			public const int Chip_hideMotionSpec = 31;

			public const int Chip_iconEndPadding = 32;

			public const int Chip_iconStartPadding = 33;

			public const int Chip_rippleColor = 34;

			public const int Chip_shapeAppearance = 35;

			public const int Chip_shapeAppearanceOverlay = 36;

			public const int Chip_showMotionSpec = 37;

			public const int Chip_textEndPadding = 38;

			public const int Chip_textStartPadding = 39;

			public static int[] CollapsingToolbarLayout;

			public const int CollapsingToolbarLayout_collapsedTitleGravity = 0;

			public const int CollapsingToolbarLayout_collapsedTitleTextAppearance = 1;

			public const int CollapsingToolbarLayout_contentScrim = 2;

			public const int CollapsingToolbarLayout_expandedTitleGravity = 3;

			public const int CollapsingToolbarLayout_expandedTitleMargin = 4;

			public const int CollapsingToolbarLayout_expandedTitleMarginBottom = 5;

			public const int CollapsingToolbarLayout_expandedTitleMarginEnd = 6;

			public const int CollapsingToolbarLayout_expandedTitleMarginStart = 7;

			public const int CollapsingToolbarLayout_expandedTitleMarginTop = 8;

			public const int CollapsingToolbarLayout_expandedTitleTextAppearance = 9;

			public static int[] CollapsingToolbarLayout_Layout;

			public const int CollapsingToolbarLayout_Layout_layout_collapseMode = 0;

			public const int CollapsingToolbarLayout_Layout_layout_collapseParallaxMultiplier = 1;

			public const int CollapsingToolbarLayout_scrimAnimationDuration = 10;

			public const int CollapsingToolbarLayout_scrimVisibleHeightTrigger = 11;

			public const int CollapsingToolbarLayout_statusBarScrim = 12;

			public const int CollapsingToolbarLayout_title = 13;

			public const int CollapsingToolbarLayout_titleEnabled = 14;

			public const int CollapsingToolbarLayout_toolbarId = 15;

			public static int[] ColorStateListItem;

			public const int ColorStateListItem_alpha = 2;

			public const int ColorStateListItem_android_alpha = 1;

			public const int ColorStateListItem_android_color = 0;

			public static int[] CompoundButton;

			public const int CompoundButton_android_button = 0;

			public const int CompoundButton_buttonCompat = 1;

			public const int CompoundButton_buttonTint = 2;

			public const int CompoundButton_buttonTintMode = 3;

			public static int[] ConstraintLayout_Layout;

			public const int ConstraintLayout_Layout_android_maxHeight = 2;

			public const int ConstraintLayout_Layout_android_maxWidth = 1;

			public const int ConstraintLayout_Layout_android_minHeight = 4;

			public const int ConstraintLayout_Layout_android_minWidth = 3;

			public const int ConstraintLayout_Layout_android_orientation = 0;

			public const int ConstraintLayout_Layout_barrierAllowsGoneWidgets = 5;

			public const int ConstraintLayout_Layout_barrierDirection = 6;

			public const int ConstraintLayout_Layout_chainUseRtl = 7;

			public const int ConstraintLayout_Layout_constraintSet = 8;

			public const int ConstraintLayout_Layout_constraint_referenced_ids = 9;

			public const int ConstraintLayout_Layout_layout_constrainedHeight = 10;

			public const int ConstraintLayout_Layout_layout_constrainedWidth = 11;

			public const int ConstraintLayout_Layout_layout_constraintBaseline_creator = 12;

			public const int ConstraintLayout_Layout_layout_constraintBaseline_toBaselineOf = 13;

			public const int ConstraintLayout_Layout_layout_constraintBottom_creator = 14;

			public const int ConstraintLayout_Layout_layout_constraintBottom_toBottomOf = 15;

			public const int ConstraintLayout_Layout_layout_constraintBottom_toTopOf = 16;

			public const int ConstraintLayout_Layout_layout_constraintCircle = 17;

			public const int ConstraintLayout_Layout_layout_constraintCircleAngle = 18;

			public const int ConstraintLayout_Layout_layout_constraintCircleRadius = 19;

			public const int ConstraintLayout_Layout_layout_constraintDimensionRatio = 20;

			public const int ConstraintLayout_Layout_layout_constraintEnd_toEndOf = 21;

			public const int ConstraintLayout_Layout_layout_constraintEnd_toStartOf = 22;

			public const int ConstraintLayout_Layout_layout_constraintGuide_begin = 23;

			public const int ConstraintLayout_Layout_layout_constraintGuide_end = 24;

			public const int ConstraintLayout_Layout_layout_constraintGuide_percent = 25;

			public const int ConstraintLayout_Layout_layout_constraintHeight_default = 26;

			public const int ConstraintLayout_Layout_layout_constraintHeight_max = 27;

			public const int ConstraintLayout_Layout_layout_constraintHeight_min = 28;

			public const int ConstraintLayout_Layout_layout_constraintHeight_percent = 29;

			public const int ConstraintLayout_Layout_layout_constraintHorizontal_bias = 30;

			public const int ConstraintLayout_Layout_layout_constraintHorizontal_chainStyle = 31;

			public const int ConstraintLayout_Layout_layout_constraintHorizontal_weight = 32;

			public const int ConstraintLayout_Layout_layout_constraintLeft_creator = 33;

			public const int ConstraintLayout_Layout_layout_constraintLeft_toLeftOf = 34;

			public const int ConstraintLayout_Layout_layout_constraintLeft_toRightOf = 35;

			public const int ConstraintLayout_Layout_layout_constraintRight_creator = 36;

			public const int ConstraintLayout_Layout_layout_constraintRight_toLeftOf = 37;

			public const int ConstraintLayout_Layout_layout_constraintRight_toRightOf = 38;

			public const int ConstraintLayout_Layout_layout_constraintStart_toEndOf = 39;

			public const int ConstraintLayout_Layout_layout_constraintStart_toStartOf = 40;

			public const int ConstraintLayout_Layout_layout_constraintTop_creator = 41;

			public const int ConstraintLayout_Layout_layout_constraintTop_toBottomOf = 42;

			public const int ConstraintLayout_Layout_layout_constraintTop_toTopOf = 43;

			public const int ConstraintLayout_Layout_layout_constraintVertical_bias = 44;

			public const int ConstraintLayout_Layout_layout_constraintVertical_chainStyle = 45;

			public const int ConstraintLayout_Layout_layout_constraintVertical_weight = 46;

			public const int ConstraintLayout_Layout_layout_constraintWidth_default = 47;

			public const int ConstraintLayout_Layout_layout_constraintWidth_max = 48;

			public const int ConstraintLayout_Layout_layout_constraintWidth_min = 49;

			public const int ConstraintLayout_Layout_layout_constraintWidth_percent = 50;

			public const int ConstraintLayout_Layout_layout_editor_absoluteX = 51;

			public const int ConstraintLayout_Layout_layout_editor_absoluteY = 52;

			public const int ConstraintLayout_Layout_layout_goneMarginBottom = 53;

			public const int ConstraintLayout_Layout_layout_goneMarginEnd = 54;

			public const int ConstraintLayout_Layout_layout_goneMarginLeft = 55;

			public const int ConstraintLayout_Layout_layout_goneMarginRight = 56;

			public const int ConstraintLayout_Layout_layout_goneMarginStart = 57;

			public const int ConstraintLayout_Layout_layout_goneMarginTop = 58;

			public const int ConstraintLayout_Layout_layout_optimizationLevel = 59;

			public static int[] ConstraintLayout_placeholder;

			public const int ConstraintLayout_placeholder_content = 0;

			public const int ConstraintLayout_placeholder_emptyVisibility = 1;

			public static int[] ConstraintSet;

			public const int ConstraintSet_android_alpha = 13;

			public const int ConstraintSet_android_elevation = 26;

			public const int ConstraintSet_android_id = 1;

			public const int ConstraintSet_android_layout_height = 4;

			public const int ConstraintSet_android_layout_marginBottom = 8;

			public const int ConstraintSet_android_layout_marginEnd = 24;

			public const int ConstraintSet_android_layout_marginLeft = 5;

			public const int ConstraintSet_android_layout_marginRight = 7;

			public const int ConstraintSet_android_layout_marginStart = 23;

			public const int ConstraintSet_android_layout_marginTop = 6;

			public const int ConstraintSet_android_layout_width = 3;

			public const int ConstraintSet_android_maxHeight = 10;

			public const int ConstraintSet_android_maxWidth = 9;

			public const int ConstraintSet_android_minHeight = 12;

			public const int ConstraintSet_android_minWidth = 11;

			public const int ConstraintSet_android_orientation = 0;

			public const int ConstraintSet_android_rotation = 20;

			public const int ConstraintSet_android_rotationX = 21;

			public const int ConstraintSet_android_rotationY = 22;

			public const int ConstraintSet_android_scaleX = 18;

			public const int ConstraintSet_android_scaleY = 19;

			public const int ConstraintSet_android_transformPivotX = 14;

			public const int ConstraintSet_android_transformPivotY = 15;

			public const int ConstraintSet_android_translationX = 16;

			public const int ConstraintSet_android_translationY = 17;

			public const int ConstraintSet_android_translationZ = 25;

			public const int ConstraintSet_android_visibility = 2;

			public const int ConstraintSet_barrierAllowsGoneWidgets = 27;

			public const int ConstraintSet_barrierDirection = 28;

			public const int ConstraintSet_chainUseRtl = 29;

			public const int ConstraintSet_constraint_referenced_ids = 30;

			public const int ConstraintSet_layout_constrainedHeight = 31;

			public const int ConstraintSet_layout_constrainedWidth = 32;

			public const int ConstraintSet_layout_constraintBaseline_creator = 33;

			public const int ConstraintSet_layout_constraintBaseline_toBaselineOf = 34;

			public const int ConstraintSet_layout_constraintBottom_creator = 35;

			public const int ConstraintSet_layout_constraintBottom_toBottomOf = 36;

			public const int ConstraintSet_layout_constraintBottom_toTopOf = 37;

			public const int ConstraintSet_layout_constraintCircle = 38;

			public const int ConstraintSet_layout_constraintCircleAngle = 39;

			public const int ConstraintSet_layout_constraintCircleRadius = 40;

			public const int ConstraintSet_layout_constraintDimensionRatio = 41;

			public const int ConstraintSet_layout_constraintEnd_toEndOf = 42;

			public const int ConstraintSet_layout_constraintEnd_toStartOf = 43;

			public const int ConstraintSet_layout_constraintGuide_begin = 44;

			public const int ConstraintSet_layout_constraintGuide_end = 45;

			public const int ConstraintSet_layout_constraintGuide_percent = 46;

			public const int ConstraintSet_layout_constraintHeight_default = 47;

			public const int ConstraintSet_layout_constraintHeight_max = 48;

			public const int ConstraintSet_layout_constraintHeight_min = 49;

			public const int ConstraintSet_layout_constraintHeight_percent = 50;

			public const int ConstraintSet_layout_constraintHorizontal_bias = 51;

			public const int ConstraintSet_layout_constraintHorizontal_chainStyle = 52;

			public const int ConstraintSet_layout_constraintHorizontal_weight = 53;

			public const int ConstraintSet_layout_constraintLeft_creator = 54;

			public const int ConstraintSet_layout_constraintLeft_toLeftOf = 55;

			public const int ConstraintSet_layout_constraintLeft_toRightOf = 56;

			public const int ConstraintSet_layout_constraintRight_creator = 57;

			public const int ConstraintSet_layout_constraintRight_toLeftOf = 58;

			public const int ConstraintSet_layout_constraintRight_toRightOf = 59;

			public const int ConstraintSet_layout_constraintStart_toEndOf = 60;

			public const int ConstraintSet_layout_constraintStart_toStartOf = 61;

			public const int ConstraintSet_layout_constraintTop_creator = 62;

			public const int ConstraintSet_layout_constraintTop_toBottomOf = 63;

			public const int ConstraintSet_layout_constraintTop_toTopOf = 64;

			public const int ConstraintSet_layout_constraintVertical_bias = 65;

			public const int ConstraintSet_layout_constraintVertical_chainStyle = 66;

			public const int ConstraintSet_layout_constraintVertical_weight = 67;

			public const int ConstraintSet_layout_constraintWidth_default = 68;

			public const int ConstraintSet_layout_constraintWidth_max = 69;

			public const int ConstraintSet_layout_constraintWidth_min = 70;

			public const int ConstraintSet_layout_constraintWidth_percent = 71;

			public const int ConstraintSet_layout_editor_absoluteX = 72;

			public const int ConstraintSet_layout_editor_absoluteY = 73;

			public const int ConstraintSet_layout_goneMarginBottom = 74;

			public const int ConstraintSet_layout_goneMarginEnd = 75;

			public const int ConstraintSet_layout_goneMarginLeft = 76;

			public const int ConstraintSet_layout_goneMarginRight = 77;

			public const int ConstraintSet_layout_goneMarginStart = 78;

			public const int ConstraintSet_layout_goneMarginTop = 79;

			public static int[] CoordinatorLayout;

			public const int CoordinatorLayout_keylines = 0;

			public static int[] CoordinatorLayout_Layout;

			public const int CoordinatorLayout_Layout_android_layout_gravity = 0;

			public const int CoordinatorLayout_Layout_layout_anchor = 1;

			public const int CoordinatorLayout_Layout_layout_anchorGravity = 2;

			public const int CoordinatorLayout_Layout_layout_behavior = 3;

			public const int CoordinatorLayout_Layout_layout_dodgeInsetEdges = 4;

			public const int CoordinatorLayout_Layout_layout_insetEdge = 5;

			public const int CoordinatorLayout_Layout_layout_keyline = 6;

			public const int CoordinatorLayout_statusBarBackground = 1;

			public static int[] DrawerArrowToggle;

			public const int DrawerArrowToggle_arrowHeadLength = 0;

			public const int DrawerArrowToggle_arrowShaftLength = 1;

			public const int DrawerArrowToggle_barLength = 2;

			public const int DrawerArrowToggle_color = 3;

			public const int DrawerArrowToggle_drawableSize = 4;

			public const int DrawerArrowToggle_gapBetweenBars = 5;

			public const int DrawerArrowToggle_spinBars = 6;

			public const int DrawerArrowToggle_thickness = 7;

			public static int[] ExtendedFloatingActionButton;

			public static int[] ExtendedFloatingActionButton_Behavior_Layout;

			public const int ExtendedFloatingActionButton_Behavior_Layout_behavior_autoHide = 0;

			public const int ExtendedFloatingActionButton_Behavior_Layout_behavior_autoShrink = 1;

			public const int ExtendedFloatingActionButton_elevation = 0;

			public const int ExtendedFloatingActionButton_extendMotionSpec = 1;

			public const int ExtendedFloatingActionButton_hideMotionSpec = 2;

			public const int ExtendedFloatingActionButton_showMotionSpec = 3;

			public const int ExtendedFloatingActionButton_shrinkMotionSpec = 4;

			public static int[] FlexboxLayout;

			public const int FlexboxLayout_alignContent = 0;

			public const int FlexboxLayout_alignItems = 1;

			public const int FlexboxLayout_dividerDrawable = 2;

			public const int FlexboxLayout_dividerDrawableHorizontal = 3;

			public const int FlexboxLayout_dividerDrawableVertical = 4;

			public const int FlexboxLayout_flexDirection = 5;

			public const int FlexboxLayout_flexWrap = 6;

			public const int FlexboxLayout_justifyContent = 7;

			public static int[] FlexboxLayout_Layout;

			public const int FlexboxLayout_Layout_layout_alignSelf = 0;

			public const int FlexboxLayout_Layout_layout_flexBasisPercent = 1;

			public const int FlexboxLayout_Layout_layout_flexGrow = 2;

			public const int FlexboxLayout_Layout_layout_flexShrink = 3;

			public const int FlexboxLayout_Layout_layout_maxHeight = 4;

			public const int FlexboxLayout_Layout_layout_maxWidth = 5;

			public const int FlexboxLayout_Layout_layout_minHeight = 6;

			public const int FlexboxLayout_Layout_layout_minWidth = 7;

			public const int FlexboxLayout_Layout_layout_order = 8;

			public const int FlexboxLayout_Layout_layout_wrapBefore = 9;

			public const int FlexboxLayout_maxLine = 8;

			public const int FlexboxLayout_showDivider = 9;

			public const int FlexboxLayout_showDividerHorizontal = 10;

			public const int FlexboxLayout_showDividerVertical = 11;

			public static int[] FloatingActionButton;

			public const int FloatingActionButton_backgroundTint = 0;

			public const int FloatingActionButton_backgroundTintMode = 1;

			public static int[] FloatingActionButton_Behavior_Layout;

			public const int FloatingActionButton_Behavior_Layout_behavior_autoHide = 0;

			public const int FloatingActionButton_borderWidth = 2;

			public const int FloatingActionButton_elevation = 3;

			public const int FloatingActionButton_ensureMinTouchTargetSize = 4;

			public const int FloatingActionButton_fabCustomSize = 5;

			public const int FloatingActionButton_fabSize = 6;

			public const int FloatingActionButton_hideMotionSpec = 7;

			public const int FloatingActionButton_hoveredFocusedTranslationZ = 8;

			public const int FloatingActionButton_maxImageSize = 9;

			public const int FloatingActionButton_pressedTranslationZ = 10;

			public const int FloatingActionButton_rippleColor = 11;

			public const int FloatingActionButton_shapeAppearance = 12;

			public const int FloatingActionButton_shapeAppearanceOverlay = 13;

			public const int FloatingActionButton_showMotionSpec = 14;

			public const int FloatingActionButton_useCompatPadding = 15;

			public static int[] FlowLayout;

			public const int FlowLayout_itemSpacing = 0;

			public const int FlowLayout_lineSpacing = 1;

			public static int[] FontFamily;

			public static int[] FontFamilyFont;

			public const int FontFamilyFont_android_font = 0;

			public const int FontFamilyFont_android_fontStyle = 2;

			public const int FontFamilyFont_android_fontVariationSettings = 4;

			public const int FontFamilyFont_android_fontWeight = 1;

			public const int FontFamilyFont_android_ttcIndex = 3;

			public const int FontFamilyFont_font = 5;

			public const int FontFamilyFont_fontStyle = 6;

			public const int FontFamilyFont_fontVariationSettings = 7;

			public const int FontFamilyFont_fontWeight = 8;

			public const int FontFamilyFont_ttcIndex = 9;

			public const int FontFamily_fontProviderAuthority = 0;

			public const int FontFamily_fontProviderCerts = 1;

			public const int FontFamily_fontProviderFetchStrategy = 2;

			public const int FontFamily_fontProviderFetchTimeout = 3;

			public const int FontFamily_fontProviderPackage = 4;

			public const int FontFamily_fontProviderQuery = 5;

			public static int[] ForegroundLinearLayout;

			public const int ForegroundLinearLayout_android_foreground = 0;

			public const int ForegroundLinearLayout_android_foregroundGravity = 1;

			public const int ForegroundLinearLayout_foregroundInsidePadding = 2;

			public static int[] Fragment;

			public static int[] FragmentContainerView;

			public const int FragmentContainerView_android_name = 0;

			public const int FragmentContainerView_android_tag = 1;

			public const int Fragment_android_id = 1;

			public const int Fragment_android_name = 0;

			public const int Fragment_android_tag = 2;

			public static int[] GradientColor;

			public static int[] GradientColorItem;

			public const int GradientColorItem_android_color = 0;

			public const int GradientColorItem_android_offset = 1;

			public const int GradientColor_android_centerColor = 7;

			public const int GradientColor_android_centerX = 3;

			public const int GradientColor_android_centerY = 4;

			public const int GradientColor_android_endColor = 1;

			public const int GradientColor_android_endX = 10;

			public const int GradientColor_android_endY = 11;

			public const int GradientColor_android_gradientRadius = 5;

			public const int GradientColor_android_startColor = 0;

			public const int GradientColor_android_startX = 8;

			public const int GradientColor_android_startY = 9;

			public const int GradientColor_android_tileMode = 6;

			public const int GradientColor_android_type = 2;

			public static int[] LinearConstraintLayout;

			public const int LinearConstraintLayout_android_orientation = 0;

			public static int[] LinearLayoutCompat;

			public const int LinearLayoutCompat_android_baselineAligned = 2;

			public const int LinearLayoutCompat_android_baselineAlignedChildIndex = 3;

			public const int LinearLayoutCompat_android_gravity = 0;

			public const int LinearLayoutCompat_android_orientation = 1;

			public const int LinearLayoutCompat_android_weightSum = 4;

			public const int LinearLayoutCompat_divider = 5;

			public const int LinearLayoutCompat_dividerPadding = 6;

			public static int[] LinearLayoutCompat_Layout;

			public const int LinearLayoutCompat_Layout_android_layout_gravity = 0;

			public const int LinearLayoutCompat_Layout_android_layout_height = 2;

			public const int LinearLayoutCompat_Layout_android_layout_weight = 3;

			public const int LinearLayoutCompat_Layout_android_layout_width = 1;

			public const int LinearLayoutCompat_measureWithLargestChild = 7;

			public const int LinearLayoutCompat_showDividers = 8;

			public static int[] ListPopupWindow;

			public const int ListPopupWindow_android_dropDownHorizontalOffset = 0;

			public const int ListPopupWindow_android_dropDownVerticalOffset = 1;

			public static int[] LoadingImageView;

			public const int LoadingImageView_circleCrop = 0;

			public const int LoadingImageView_imageAspectRatio = 1;

			public const int LoadingImageView_imageAspectRatioAdjust = 2;

			public static int[] MaterialAlertDialog;

			public static int[] MaterialAlertDialogTheme;

			public const int MaterialAlertDialogTheme_materialAlertDialogBodyTextStyle = 0;

			public const int MaterialAlertDialogTheme_materialAlertDialogTheme = 1;

			public const int MaterialAlertDialogTheme_materialAlertDialogTitleIconStyle = 2;

			public const int MaterialAlertDialogTheme_materialAlertDialogTitlePanelStyle = 3;

			public const int MaterialAlertDialogTheme_materialAlertDialogTitleTextStyle = 4;

			public const int MaterialAlertDialog_backgroundInsetBottom = 0;

			public const int MaterialAlertDialog_backgroundInsetEnd = 1;

			public const int MaterialAlertDialog_backgroundInsetStart = 2;

			public const int MaterialAlertDialog_backgroundInsetTop = 3;

			public static int[] MaterialButton;

			public static int[] MaterialButtonToggleGroup;

			public const int MaterialButtonToggleGroup_checkedButton = 0;

			public const int MaterialButtonToggleGroup_singleSelection = 1;

			public const int MaterialButton_android_checkable = 4;

			public const int MaterialButton_android_insetBottom = 3;

			public const int MaterialButton_android_insetLeft = 0;

			public const int MaterialButton_android_insetRight = 1;

			public const int MaterialButton_android_insetTop = 2;

			public const int MaterialButton_backgroundTint = 5;

			public const int MaterialButton_backgroundTintMode = 6;

			public const int MaterialButton_cornerRadius = 7;

			public const int MaterialButton_elevation = 8;

			public const int MaterialButton_icon = 9;

			public const int MaterialButton_iconGravity = 10;

			public const int MaterialButton_iconPadding = 11;

			public const int MaterialButton_iconSize = 12;

			public const int MaterialButton_iconTint = 13;

			public const int MaterialButton_iconTintMode = 14;

			public const int MaterialButton_rippleColor = 15;

			public const int MaterialButton_shapeAppearance = 16;

			public const int MaterialButton_shapeAppearanceOverlay = 17;

			public const int MaterialButton_strokeColor = 18;

			public const int MaterialButton_strokeWidth = 19;

			public static int[] MaterialCalendar;

			public static int[] MaterialCalendarItem;

			public const int MaterialCalendarItem_android_insetBottom = 3;

			public const int MaterialCalendarItem_android_insetLeft = 0;

			public const int MaterialCalendarItem_android_insetRight = 1;

			public const int MaterialCalendarItem_android_insetTop = 2;

			public const int MaterialCalendarItem_itemFillColor = 4;

			public const int MaterialCalendarItem_itemShapeAppearance = 5;

			public const int MaterialCalendarItem_itemShapeAppearanceOverlay = 6;

			public const int MaterialCalendarItem_itemStrokeColor = 7;

			public const int MaterialCalendarItem_itemStrokeWidth = 8;

			public const int MaterialCalendarItem_itemTextColor = 9;

			public const int MaterialCalendar_android_windowFullscreen = 0;

			public const int MaterialCalendar_dayInvalidStyle = 1;

			public const int MaterialCalendar_daySelectedStyle = 2;

			public const int MaterialCalendar_dayStyle = 3;

			public const int MaterialCalendar_dayTodayStyle = 4;

			public const int MaterialCalendar_rangeFillColor = 5;

			public const int MaterialCalendar_yearSelectedStyle = 6;

			public const int MaterialCalendar_yearStyle = 7;

			public const int MaterialCalendar_yearTodayStyle = 8;

			public static int[] MaterialCardView;

			public const int MaterialCardView_android_checkable = 0;

			public const int MaterialCardView_cardForegroundColor = 1;

			public const int MaterialCardView_checkedIcon = 2;

			public const int MaterialCardView_checkedIconTint = 3;

			public const int MaterialCardView_rippleColor = 4;

			public const int MaterialCardView_shapeAppearance = 5;

			public const int MaterialCardView_shapeAppearanceOverlay = 6;

			public const int MaterialCardView_state_dragged = 7;

			public const int MaterialCardView_strokeColor = 8;

			public const int MaterialCardView_strokeWidth = 9;

			public static int[] MaterialCheckBox;

			public const int MaterialCheckBox_buttonTint = 0;

			public const int MaterialCheckBox_useMaterialThemeColors = 1;

			public static int[] MaterialRadioButton;

			public const int MaterialRadioButton_useMaterialThemeColors = 0;

			public static int[] MaterialShape;

			public const int MaterialShape_shapeAppearance = 0;

			public const int MaterialShape_shapeAppearanceOverlay = 1;

			public static int[] MaterialTextAppearance;

			public const int MaterialTextAppearance_android_lineHeight = 0;

			public const int MaterialTextAppearance_lineHeight = 1;

			public static int[] MaterialTextView;

			public const int MaterialTextView_android_lineHeight = 1;

			public const int MaterialTextView_android_textAppearance = 0;

			public const int MaterialTextView_lineHeight = 2;

			public static int[] MenuGroup;

			public const int MenuGroup_android_checkableBehavior = 5;

			public const int MenuGroup_android_enabled = 0;

			public const int MenuGroup_android_id = 1;

			public const int MenuGroup_android_menuCategory = 3;

			public const int MenuGroup_android_orderInCategory = 4;

			public const int MenuGroup_android_visible = 2;

			public static int[] MenuItem;

			public const int MenuItem_actionLayout = 13;

			public const int MenuItem_actionProviderClass = 14;

			public const int MenuItem_actionViewClass = 15;

			public const int MenuItem_alphabeticModifiers = 16;

			public const int MenuItem_android_alphabeticShortcut = 9;

			public const int MenuItem_android_checkable = 11;

			public const int MenuItem_android_checked = 3;

			public const int MenuItem_android_enabled = 1;

			public const int MenuItem_android_icon = 0;

			public const int MenuItem_android_id = 2;

			public const int MenuItem_android_menuCategory = 5;

			public const int MenuItem_android_numericShortcut = 10;

			public const int MenuItem_android_onClick = 12;

			public const int MenuItem_android_orderInCategory = 6;

			public const int MenuItem_android_title = 7;

			public const int MenuItem_android_titleCondensed = 8;

			public const int MenuItem_android_visible = 4;

			public const int MenuItem_contentDescription = 17;

			public const int MenuItem_iconTint = 18;

			public const int MenuItem_iconTintMode = 19;

			public const int MenuItem_numericModifiers = 20;

			public const int MenuItem_showAsAction = 21;

			public const int MenuItem_tooltipText = 22;

			public static int[] MenuView;

			public const int MenuView_android_headerBackground = 4;

			public const int MenuView_android_horizontalDivider = 2;

			public const int MenuView_android_itemBackground = 5;

			public const int MenuView_android_itemIconDisabledAlpha = 6;

			public const int MenuView_android_itemTextAppearance = 1;

			public const int MenuView_android_verticalDivider = 3;

			public const int MenuView_android_windowAnimationStyle = 0;

			public const int MenuView_preserveIconSpacing = 7;

			public const int MenuView_subMenuArrow = 8;

			public static int[] NavigationView;

			public const int NavigationView_android_background = 0;

			public const int NavigationView_android_fitsSystemWindows = 1;

			public const int NavigationView_android_maxWidth = 2;

			public const int NavigationView_elevation = 3;

			public const int NavigationView_headerLayout = 4;

			public const int NavigationView_itemBackground = 5;

			public const int NavigationView_itemHorizontalPadding = 6;

			public const int NavigationView_itemIconPadding = 7;

			public const int NavigationView_itemIconSize = 8;

			public const int NavigationView_itemIconTint = 9;

			public const int NavigationView_itemMaxLines = 10;

			public const int NavigationView_itemShapeAppearance = 11;

			public const int NavigationView_itemShapeAppearanceOverlay = 12;

			public const int NavigationView_itemShapeFillColor = 13;

			public const int NavigationView_itemShapeInsetBottom = 14;

			public const int NavigationView_itemShapeInsetEnd = 15;

			public const int NavigationView_itemShapeInsetStart = 16;

			public const int NavigationView_itemShapeInsetTop = 17;

			public const int NavigationView_itemTextAppearance = 18;

			public const int NavigationView_itemTextColor = 19;

			public const int NavigationView_menu = 20;

			public static int[] PopupWindow;

			public static int[] PopupWindowBackgroundState;

			public const int PopupWindowBackgroundState_state_above_anchor = 0;

			public const int PopupWindow_android_popupAnimationStyle = 1;

			public const int PopupWindow_android_popupBackground = 0;

			public const int PopupWindow_overlapAnchor = 2;

			public static int[] RecycleListView;

			public const int RecycleListView_paddingBottomNoButtons = 0;

			public const int RecycleListView_paddingTopNoTitle = 1;

			public static int[] RecyclerView;

			public const int RecyclerView_android_clipToPadding = 1;

			public const int RecyclerView_android_descendantFocusability = 2;

			public const int RecyclerView_android_orientation = 0;

			public const int RecyclerView_fastScrollEnabled = 3;

			public const int RecyclerView_fastScrollHorizontalThumbDrawable = 4;

			public const int RecyclerView_fastScrollHorizontalTrackDrawable = 5;

			public const int RecyclerView_fastScrollVerticalThumbDrawable = 6;

			public const int RecyclerView_fastScrollVerticalTrackDrawable = 7;

			public const int RecyclerView_layoutManager = 8;

			public const int RecyclerView_reverseLayout = 9;

			public const int RecyclerView_spanCount = 10;

			public const int RecyclerView_stackFromEnd = 11;

			public static int[] ScrimInsetsFrameLayout;

			public const int ScrimInsetsFrameLayout_insetForeground = 0;

			public static int[] ScrollingViewBehavior_Layout;

			public const int ScrollingViewBehavior_Layout_behavior_overlapTop = 0;

			public static int[] SearchView;

			public const int SearchView_android_focusable = 0;

			public const int SearchView_android_imeOptions = 3;

			public const int SearchView_android_inputType = 2;

			public const int SearchView_android_maxWidth = 1;

			public const int SearchView_closeIcon = 4;

			public const int SearchView_commitIcon = 5;

			public const int SearchView_defaultQueryHint = 6;

			public const int SearchView_goIcon = 7;

			public const int SearchView_iconifiedByDefault = 8;

			public const int SearchView_layout = 9;

			public const int SearchView_queryBackground = 10;

			public const int SearchView_queryHint = 11;

			public const int SearchView_searchHintIcon = 12;

			public const int SearchView_searchIcon = 13;

			public const int SearchView_submitBackground = 14;

			public const int SearchView_suggestionRowLayout = 15;

			public const int SearchView_voiceIcon = 16;

			public static int[] ShapeAppearance;

			public const int ShapeAppearance_cornerFamily = 0;

			public const int ShapeAppearance_cornerFamilyBottomLeft = 1;

			public const int ShapeAppearance_cornerFamilyBottomRight = 2;

			public const int ShapeAppearance_cornerFamilyTopLeft = 3;

			public const int ShapeAppearance_cornerFamilyTopRight = 4;

			public const int ShapeAppearance_cornerSize = 5;

			public const int ShapeAppearance_cornerSizeBottomLeft = 6;

			public const int ShapeAppearance_cornerSizeBottomRight = 7;

			public const int ShapeAppearance_cornerSizeTopLeft = 8;

			public const int ShapeAppearance_cornerSizeTopRight = 9;

			public static int[] SignInButton;

			public const int SignInButton_buttonSize = 0;

			public const int SignInButton_colorScheme = 1;

			public const int SignInButton_scopeUris = 2;

			public static int[] Snackbar;

			public static int[] SnackbarLayout;

			public const int SnackbarLayout_actionTextColorAlpha = 1;

			public const int SnackbarLayout_android_maxWidth = 0;

			public const int SnackbarLayout_animationMode = 2;

			public const int SnackbarLayout_backgroundOverlayColorAlpha = 3;

			public const int SnackbarLayout_elevation = 4;

			public const int SnackbarLayout_maxActionInlineWidth = 5;

			public const int Snackbar_snackbarButtonStyle = 0;

			public const int Snackbar_snackbarStyle = 1;

			public static int[] Spinner;

			public const int Spinner_android_dropDownWidth = 3;

			public const int Spinner_android_entries = 0;

			public const int Spinner_android_popupBackground = 1;

			public const int Spinner_android_prompt = 2;

			public const int Spinner_popupTheme = 4;

			public static int[] StateListDrawable;

			public static int[] StateListDrawableItem;

			public const int StateListDrawableItem_android_drawable = 0;

			public const int StateListDrawable_android_constantSize = 3;

			public const int StateListDrawable_android_dither = 0;

			public const int StateListDrawable_android_enterFadeDuration = 4;

			public const int StateListDrawable_android_exitFadeDuration = 5;

			public const int StateListDrawable_android_variablePadding = 2;

			public const int StateListDrawable_android_visible = 1;

			public static int[] SwitchCompat;

			public const int SwitchCompat_android_textOff = 1;

			public const int SwitchCompat_android_textOn = 0;

			public const int SwitchCompat_android_thumb = 2;

			public const int SwitchCompat_showText = 3;

			public const int SwitchCompat_splitTrack = 4;

			public const int SwitchCompat_switchMinWidth = 5;

			public const int SwitchCompat_switchPadding = 6;

			public const int SwitchCompat_switchTextAppearance = 7;

			public const int SwitchCompat_thumbTextPadding = 8;

			public const int SwitchCompat_thumbTint = 9;

			public const int SwitchCompat_thumbTintMode = 10;

			public const int SwitchCompat_track = 11;

			public const int SwitchCompat_trackTint = 12;

			public const int SwitchCompat_trackTintMode = 13;

			public static int[] SwitchMaterial;

			public const int SwitchMaterial_useMaterialThemeColors = 0;

			public static int[] TabItem;

			public const int TabItem_android_icon = 0;

			public const int TabItem_android_layout = 1;

			public const int TabItem_android_text = 2;

			public static int[] TabLayout;

			public const int TabLayout_tabBackground = 0;

			public const int TabLayout_tabContentStart = 1;

			public const int TabLayout_tabGravity = 2;

			public const int TabLayout_tabIconTint = 3;

			public const int TabLayout_tabIconTintMode = 4;

			public const int TabLayout_tabIndicator = 5;

			public const int TabLayout_tabIndicatorAnimationDuration = 6;

			public const int TabLayout_tabIndicatorColor = 7;

			public const int TabLayout_tabIndicatorFullWidth = 8;

			public const int TabLayout_tabIndicatorGravity = 9;

			public const int TabLayout_tabIndicatorHeight = 10;

			public const int TabLayout_tabInlineLabel = 11;

			public const int TabLayout_tabMaxWidth = 12;

			public const int TabLayout_tabMinWidth = 13;

			public const int TabLayout_tabMode = 14;

			public const int TabLayout_tabPadding = 15;

			public const int TabLayout_tabPaddingBottom = 16;

			public const int TabLayout_tabPaddingEnd = 17;

			public const int TabLayout_tabPaddingStart = 18;

			public const int TabLayout_tabPaddingTop = 19;

			public const int TabLayout_tabRippleColor = 20;

			public const int TabLayout_tabSelectedTextColor = 21;

			public const int TabLayout_tabTextAppearance = 22;

			public const int TabLayout_tabTextColor = 23;

			public const int TabLayout_tabUnboundedRipple = 24;

			public static int[] TextAppearance;

			public const int TextAppearance_android_fontFamily = 10;

			public const int TextAppearance_android_shadowColor = 6;

			public const int TextAppearance_android_shadowDx = 7;

			public const int TextAppearance_android_shadowDy = 8;

			public const int TextAppearance_android_shadowRadius = 9;

			public const int TextAppearance_android_textColor = 3;

			public const int TextAppearance_android_textColorHint = 4;

			public const int TextAppearance_android_textColorLink = 5;

			public const int TextAppearance_android_textFontWeight = 11;

			public const int TextAppearance_android_textSize = 0;

			public const int TextAppearance_android_textStyle = 2;

			public const int TextAppearance_android_typeface = 1;

			public const int TextAppearance_fontFamily = 12;

			public const int TextAppearance_fontVariationSettings = 13;

			public const int TextAppearance_textAllCaps = 14;

			public const int TextAppearance_textLocale = 15;

			public static int[] TextInputLayout;

			public const int TextInputLayout_android_hint = 1;

			public const int TextInputLayout_android_textColorHint = 0;

			public const int TextInputLayout_boxBackgroundColor = 2;

			public const int TextInputLayout_boxBackgroundMode = 3;

			public const int TextInputLayout_boxCollapsedPaddingTop = 4;

			public const int TextInputLayout_boxCornerRadiusBottomEnd = 5;

			public const int TextInputLayout_boxCornerRadiusBottomStart = 6;

			public const int TextInputLayout_boxCornerRadiusTopEnd = 7;

			public const int TextInputLayout_boxCornerRadiusTopStart = 8;

			public const int TextInputLayout_boxStrokeColor = 9;

			public const int TextInputLayout_boxStrokeWidth = 10;

			public const int TextInputLayout_boxStrokeWidthFocused = 11;

			public const int TextInputLayout_counterEnabled = 12;

			public const int TextInputLayout_counterMaxLength = 13;

			public const int TextInputLayout_counterOverflowTextAppearance = 14;

			public const int TextInputLayout_counterOverflowTextColor = 15;

			public const int TextInputLayout_counterTextAppearance = 16;

			public const int TextInputLayout_counterTextColor = 17;

			public const int TextInputLayout_endIconCheckable = 18;

			public const int TextInputLayout_endIconContentDescription = 19;

			public const int TextInputLayout_endIconDrawable = 20;

			public const int TextInputLayout_endIconMode = 21;

			public const int TextInputLayout_endIconTint = 22;

			public const int TextInputLayout_endIconTintMode = 23;

			public const int TextInputLayout_errorEnabled = 24;

			public const int TextInputLayout_errorIconDrawable = 25;

			public const int TextInputLayout_errorIconTint = 26;

			public const int TextInputLayout_errorIconTintMode = 27;

			public const int TextInputLayout_errorTextAppearance = 28;

			public const int TextInputLayout_errorTextColor = 29;

			public const int TextInputLayout_helperText = 30;

			public const int TextInputLayout_helperTextEnabled = 31;

			public const int TextInputLayout_helperTextTextAppearance = 32;

			public const int TextInputLayout_helperTextTextColor = 33;

			public const int TextInputLayout_hintAnimationEnabled = 34;

			public const int TextInputLayout_hintEnabled = 35;

			public const int TextInputLayout_hintTextAppearance = 36;

			public const int TextInputLayout_hintTextColor = 37;

			public const int TextInputLayout_passwordToggleContentDescription = 38;

			public const int TextInputLayout_passwordToggleDrawable = 39;

			public const int TextInputLayout_passwordToggleEnabled = 40;

			public const int TextInputLayout_passwordToggleTint = 41;

			public const int TextInputLayout_passwordToggleTintMode = 42;

			public const int TextInputLayout_shapeAppearance = 43;

			public const int TextInputLayout_shapeAppearanceOverlay = 44;

			public const int TextInputLayout_startIconCheckable = 45;

			public const int TextInputLayout_startIconContentDescription = 46;

			public const int TextInputLayout_startIconDrawable = 47;

			public const int TextInputLayout_startIconTint = 48;

			public const int TextInputLayout_startIconTintMode = 49;

			public static int[] ThemeEnforcement;

			public const int ThemeEnforcement_android_textAppearance = 0;

			public const int ThemeEnforcement_enforceMaterialTheme = 1;

			public const int ThemeEnforcement_enforceTextAppearance = 2;

			public static int[] Toolbar;

			public const int Toolbar_android_gravity = 0;

			public const int Toolbar_android_minHeight = 1;

			public const int Toolbar_buttonGravity = 2;

			public const int Toolbar_collapseContentDescription = 3;

			public const int Toolbar_collapseIcon = 4;

			public const int Toolbar_contentInsetEnd = 5;

			public const int Toolbar_contentInsetEndWithActions = 6;

			public const int Toolbar_contentInsetLeft = 7;

			public const int Toolbar_contentInsetRight = 8;

			public const int Toolbar_contentInsetStart = 9;

			public const int Toolbar_contentInsetStartWithNavigation = 10;

			public const int Toolbar_logo = 11;

			public const int Toolbar_logoDescription = 12;

			public const int Toolbar_maxButtonHeight = 13;

			public const int Toolbar_menu = 14;

			public const int Toolbar_navigationContentDescription = 15;

			public const int Toolbar_navigationIcon = 16;

			public const int Toolbar_popupTheme = 17;

			public const int Toolbar_subtitle = 18;

			public const int Toolbar_subtitleTextAppearance = 19;

			public const int Toolbar_subtitleTextColor = 20;

			public const int Toolbar_title = 21;

			public const int Toolbar_titleMargin = 22;

			public const int Toolbar_titleMarginBottom = 23;

			public const int Toolbar_titleMarginEnd = 24;

			public const int Toolbar_titleMargins = 27;

			public const int Toolbar_titleMarginStart = 25;

			public const int Toolbar_titleMarginTop = 26;

			public const int Toolbar_titleTextAppearance = 28;

			public const int Toolbar_titleTextColor = 29;

			public static int[] View;

			public static int[] ViewBackgroundHelper;

			public const int ViewBackgroundHelper_android_background = 0;

			public const int ViewBackgroundHelper_backgroundTint = 1;

			public const int ViewBackgroundHelper_backgroundTintMode = 2;

			public static int[] ViewPager2;

			public const int ViewPager2_android_orientation = 0;

			public static int[] ViewStubCompat;

			public const int ViewStubCompat_android_id = 0;

			public const int ViewStubCompat_android_inflatedId = 2;

			public const int ViewStubCompat_android_layout = 1;

			public const int View_android_focusable = 1;

			public const int View_android_theme = 0;

			public const int View_paddingEnd = 2;

			public const int View_paddingStart = 3;

			public const int View_theme = 4;

			static Styleable()
			{
				ActionBar = new int[29]
				{
					2130903094, 2130903101, 2130903102, 2130903229, 2130903230, 2130903231, 2130903232, 2130903233, 2130903234, 2130903260,
					2130903269, 2130903270, 2130903292, 2130903353, 2130903359, 2130903365, 2130903366, 2130903368, 2130903380, 2130903393,
					2130903504, 2130903537, 2130903556, 2130903560, 2130903561, 2130903626, 2130903629, 2130903701, 2130903711
				};
				ActionBarLayout = new int[1] { 16842931 };
				ActionMenuItemView = new int[1] { 16843071 };
				ActionMenuView = new int[1] { -1 };
				ActionMode = new int[6] { 2130903094, 2130903101, 2130903196, 2130903353, 2130903629, 2130903711 };
				ActivityChooserView = new int[2] { 2130903311, 2130903381 };
				AlertDialog = new int[8] { 16842994, 2130903144, 2130903145, 2130903493, 2130903494, 2130903534, 2130903594, 2130903596 };
				AnimatedStateListDrawableCompat = new int[6] { 16843036, 16843156, 16843157, 16843158, 16843532, 16843533 };
				AnimatedStateListDrawableItem = new int[2] { 16842960, 16843161 };
				AnimatedStateListDrawableTransition = new int[4] { 16843161, 16843849, 16843850, 16843851 };
				AppBarLayout = new int[8] { 16842964, 16843919, 16844096, 2130903292, 2130903312, 2130903485, 2130903486, 2130903620 };
				AppBarLayoutStates = new int[4] { 2130903614, 2130903615, 2130903617, 2130903618 };
				AppBarLayout_Layout = new int[2] { 2130903482, 2130903483 };
				AppCompatImageView = new int[4] { 16843033, 2130903606, 2130903699, 2130903700 };
				AppCompatSeekBar = new int[4] { 16843074, 2130903696, 2130903697, 2130903698 };
				AppCompatTextHelper = new int[7] { 16842804, 16843117, 16843118, 16843119, 16843120, 16843666, 16843667 };
				AppCompatTextView = new int[21]
				{
					16842804, 2130903089, 2130903090, 2130903091, 2130903092, 2130903093, 2130903277, 2130903278, 2130903279, 2130903280,
					2130903282, 2130903283, 2130903284, 2130903285, 2130903334, 2130903339, 2130903347, 2130903412, 2130903487, 2130903661,
					2130903688
				};
				AppCompatTheme = new int[125]
				{
					16842839, 16842926, 2130903040, 2130903041, 2130903042, 2130903043, 2130903044, 2130903045, 2130903046, 2130903047,
					2130903048, 2130903049, 2130903050, 2130903051, 2130903052, 2130903054, 2130903055, 2130903056, 2130903057, 2130903058,
					2130903059, 2130903060, 2130903061, 2130903062, 2130903063, 2130903064, 2130903065, 2130903066, 2130903067, 2130903068,
					2130903069, 2130903070, 2130903074, 2130903075, 2130903076, 2130903077, 2130903078, 2130903088, 2130903122, 2130903137,
					2130903138, 2130903139, 2130903140, 2130903141, 2130903147, 2130903148, 2130903160, 2130903167, 2130903202, 2130903203,
					2130903204, 2130903205, 2130903206, 2130903207, 2130903208, 2130903215, 2130903216, 2130903223, 2130903241, 2130903266,
					2130903267, 2130903268, 2130903274, 2130903276, 2130903287, 2130903288, 2130903289, 2130903290, 2130903291, 2130903365,
					2130903379, 2130903489, 2130903490, 2130903491, 2130903492, 2130903495, 2130903496, 2130903497, 2130903498, 2130903499,
					2130903500, 2130903501, 2130903502, 2130903503, 2130903546, 2130903547, 2130903548, 2130903555, 2130903557, 2130903564,
					2130903566, 2130903567, 2130903568, 2130903578, 2130903579, 2130903580, 2130903581, 2130903603, 2130903604, 2130903633,
					2130903672, 2130903674, 2130903675, 2130903676, 2130903678, 2130903679, 2130903680, 2130903681, 2130903684, 2130903685,
					2130903713, 2130903714, 2130903715, 2130903716, 2130903724, 2130903726, 2130903727, 2130903728, 2130903729, 2130903730,
					2130903731, 2130903732, 2130903733, 2130903734, 2130903735
				};
				Badge = new int[5] { 2130903095, 2130903105, 2130903107, 2130903528, 2130903539 };
				BottomAppBar = new int[8] { 2130903103, 2130903292, 2130903322, 2130903323, 2130903324, 2130903325, 2130903326, 2130903360 };
				BottomNavigationView = new int[12]
				{
					2130903103, 2130903292, 2130903385, 2130903388, 2130903390, 2130903391, 2130903394, 2130903406, 2130903407, 2130903408,
					2130903411, 2130903532
				};
				BottomSheetBehavior_Layout = new int[11]
				{
					16843840, 2130903103, 2130903113, 2130903114, 2130903115, 2130903116, 2130903118, 2130903119, 2130903120, 2130903582,
					2130903585
				};
				ButtonBarLayout = new int[1] { 2130903081 };
				CardView = new int[13]
				{
					16843071, 16843072, 2130903151, 2130903152, 2130903153, 2130903155, 2130903156, 2130903157, 2130903235, 2130903236,
					2130903237, 2130903238, 2130903239
				};
				Chip = new int[40]
				{
					16842804, 16842904, 16842923, 16843039, 16843087, 16843237, 2130903163, 2130903164, 2130903166, 2130903168,
					2130903169, 2130903170, 2130903172, 2130903173, 2130903174, 2130903175, 2130903176, 2130903177, 2130903178, 2130903183,
					2130903184, 2130903185, 2130903187, 2130903189, 2130903190, 2130903191, 2130903192, 2130903193, 2130903194, 2130903195,
					2130903304, 2130903358, 2130903369, 2130903373, 2130903571, 2130903582, 2130903585, 2130903592, 2130903686, 2130903689
				};
				ChipGroup = new int[6] { 2130903162, 2130903179, 2130903180, 2130903181, 2130903597, 2130903598 };
				CollapsingToolbarLayout = new int[16]
				{
					2130903199, 2130903200, 2130903240, 2130903313, 2130903314, 2130903315, 2130903316, 2130903317, 2130903318, 2130903319,
					2130903573, 2130903575, 2130903621, 2130903701, 2130903702, 2130903712
				};
				CollapsingToolbarLayout_Layout = new int[2] { 2130903419, 2130903420 };
				ColorStateListItem = new int[3] { 16843173, 16843551, 2130903082 };
				CompoundButton = new int[4] { 16843015, 2130903142, 2130903149, 2130903150 };
				ConstraintLayout_Layout = new int[60]
				{
					16842948, 16843039, 16843040, 16843071, 16843072, 2130903109, 2130903110, 2130903159, 2130903225, 2130903226,
					2130903421, 2130903422, 2130903423, 2130903424, 2130903425, 2130903426, 2130903427, 2130903428, 2130903429, 2130903430,
					2130903431, 2130903432, 2130903433, 2130903434, 2130903435, 2130903436, 2130903437, 2130903438, 2130903439, 2130903440,
					2130903441, 2130903442, 2130903443, 2130903444, 2130903445, 2130903446, 2130903447, 2130903448, 2130903449, 2130903450,
					2130903451, 2130903452, 2130903453, 2130903454, 2130903455, 2130903456, 2130903457, 2130903458, 2130903459, 2130903460,
					2130903461, 2130903463, 2130903464, 2130903468, 2130903469, 2130903470, 2130903471, 2130903472, 2130903473, 2130903480
				};
				ConstraintLayout_placeholder = new int[2] { 2130903227, 2130903295 };
				ConstraintSet = new int[80]
				{
					16842948, 16842960, 16842972, 16842996, 16842997, 16842999, 16843000, 16843001, 16843002, 16843039,
					16843040, 16843071, 16843072, 16843551, 16843552, 16843553, 16843554, 16843555, 16843556, 16843557,
					16843558, 16843559, 16843560, 16843701, 16843702, 16843770, 16843840, 2130903109, 2130903110, 2130903159,
					2130903226, 2130903421, 2130903422, 2130903423, 2130903424, 2130903425, 2130903426, 2130903427, 2130903428, 2130903429,
					2130903430, 2130903431, 2130903432, 2130903433, 2130903434, 2130903435, 2130903436, 2130903437, 2130903438, 2130903439,
					2130903440, 2130903441, 2130903442, 2130903443, 2130903444, 2130903445, 2130903446, 2130903447, 2130903448, 2130903449,
					2130903450, 2130903451, 2130903452, 2130903453, 2130903454, 2130903455, 2130903456, 2130903457, 2130903458, 2130903459,
					2130903460, 2130903461, 2130903463, 2130903464, 2130903468, 2130903469, 2130903470, 2130903471, 2130903472, 2130903473
				};
				CoordinatorLayout = new int[2] { 2130903410, 2130903619 };
				CoordinatorLayout_Layout = new int[7] { 16842931, 2130903416, 2130903417, 2130903418, 2130903462, 2130903474, 2130903475 };
				DrawerArrowToggle = new int[8] { 2130903086, 2130903087, 2130903108, 2130903201, 2130903281, 2130903350, 2130903602, 2130903692 };
				ExtendedFloatingActionButton = new int[5] { 2130903292, 2130903320, 2130903358, 2130903592, 2130903595 };
				ExtendedFloatingActionButton_Behavior_Layout = new int[2] { 2130903111, 2130903112 };
				FlexboxLayout = new int[12]
				{
					2130903079, 2130903080, 2130903271, 2130903272, 2130903273, 2130903335, 2130903336, 2130903409, 2130903530, 2130903588,
					2130903589, 2130903590
				};
				FlexboxLayout_Layout = new int[10] { 2130903415, 2130903465, 2130903466, 2130903467, 2130903476, 2130903477, 2130903478, 2130903479, 2130903481, 2130903484 };
				FloatingActionButton = new int[16]
				{
					2130903103, 2130903104, 2130903121, 2130903292, 2130903304, 2130903327, 2130903328, 2130903358, 2130903367, 2130903529,
					2130903559, 2130903571, 2130903582, 2130903585, 2130903592, 2130903722
				};
				FloatingActionButton_Behavior_Layout = new int[1] { 2130903111 };
				FlowLayout = new int[2] { 2130903402, 2130903488 };
				FontFamily = new int[6] { 2130903340, 2130903341, 2130903342, 2130903343, 2130903344, 2130903345 };
				FontFamilyFont = new int[10] { 16844082, 16844083, 16844095, 16844143, 16844144, 2130903338, 2130903346, 2130903347, 2130903348, 2130903721 };
				ForegroundLinearLayout = new int[3] { 16843017, 16843264, 2130903349 };
				Fragment = new int[3] { 16842755, 16842960, 16842961 };
				FragmentContainerView = new int[2] { 16842755, 16842961 };
				GradientColor = new int[12]
				{
					16843165, 16843166, 16843169, 16843170, 16843171, 16843172, 16843265, 16843275, 16844048, 16844049,
					16844050, 16844051
				};
				GradientColorItem = new int[2] { 16843173, 16844052 };
				LinearConstraintLayout = new int[1] { 16842948 };
				LinearLayoutCompat = new int[9] { 16842927, 16842948, 16843046, 16843047, 16843048, 2130903270, 2130903275, 2130903531, 2130903591 };
				LinearLayoutCompat_Layout = new int[4] { 16842931, 16842996, 16842997, 16843137 };
				ListPopupWindow = new int[2] { 16843436, 16843437 };
				LoadingImageView = new int[3] { 2130903188, 2130903377, 2130903378 };
				MaterialAlertDialog = new int[4] { 2130903096, 2130903097, 2130903098, 2130903099 };
				MaterialAlertDialogTheme = new int[5] { 2130903506, 2130903507, 2130903508, 2130903509, 2130903510 };
				MaterialButton = new int[20]
				{
					16843191, 16843192, 16843193, 16843194, 16843237, 2130903103, 2130903104, 2130903248, 2130903292, 2130903368,
					2130903370, 2130903371, 2130903372, 2130903374, 2130903375, 2130903571, 2130903582, 2130903585, 2130903622, 2130903623
				};
				MaterialButtonToggleGroup = new int[2] { 2130903161, 2130903598 };
				MaterialCalendar = new int[9] { 16843277, 2130903261, 2130903262, 2130903263, 2130903264, 2130903565, 2130903736, 2130903737, 2130903738 };
				MaterialCalendarItem = new int[10] { 16843191, 16843192, 16843193, 16843194, 2130903386, 2130903395, 2130903396, 2130903403, 2130903404, 2130903408 };
				MaterialCardView = new int[10] { 16843237, 2130903154, 2130903163, 2130903165, 2130903571, 2130903582, 2130903585, 2130903616, 2130903622, 2130903623 };
				MaterialCheckBox = new int[2] { 2130903149, 2130903723 };
				MaterialRadioButton = new int[1] { 2130903723 };
				MaterialShape = new int[2] { 2130903582, 2130903585 };
				MaterialTextAppearance = new int[2] { 16844159, 2130903487 };
				MaterialTextView = new int[3] { 16842804, 16844159, 2130903487 };
				MenuGroup = new int[6] { 16842766, 16842960, 16843156, 16843230, 16843231, 16843232 };
				MenuItem = new int[23]
				{
					16842754, 16842766, 16842960, 16843014, 16843156, 16843230, 16843231, 16843233, 16843234, 16843235,
					16843236, 16843237, 16843375, 2130903053, 2130903071, 2130903073, 2130903083, 2130903228, 2130903374, 2130903375,
					2130903540, 2130903587, 2130903717
				};
				MenuView = new int[9] { 16842926, 16843052, 16843053, 16843054, 16843055, 16843056, 16843057, 2130903558, 2130903624 };
				NavigationView = new int[21]
				{
					16842964, 16842973, 16843039, 2130903292, 2130903352, 2130903385, 2130903387, 2130903389, 2130903390, 2130903391,
					2130903392, 2130903395, 2130903396, 2130903397, 2130903398, 2130903399, 2130903400, 2130903401, 2130903405, 2130903408,
					2130903532
				};
				PopupWindow = new int[3] { 16843126, 16843465, 2130903541 };
				PopupWindowBackgroundState = new int[1] { 2130903613 };
				RecycleListView = new int[2] { 2130903542, 2130903545 };
				RecyclerView = new int[12]
				{
					16842948, 16842987, 16842993, 2130903329, 2130903330, 2130903331, 2130903332, 2130903333, 2130903414, 2130903570,
					2130903601, 2130903607
				};
				ScrimInsetsFrameLayout = new int[1] { 2130903382 };
				ScrollingViewBehavior_Layout = new int[1] { 2130903117 };
				SearchView = new int[17]
				{
					16842970, 16843039, 16843296, 16843364, 2130903189, 2130903224, 2130903265, 2130903351, 2130903376, 2130903413,
					2130903562, 2130903563, 2130903576, 2130903577, 2130903625, 2130903630, 2130903725
				};
				ShapeAppearance = new int[10] { 2130903243, 2130903244, 2130903245, 2130903246, 2130903247, 2130903249, 2130903250, 2130903251, 2130903252, 2130903253 };
				SignInButton = new int[3] { 2130903146, 2130903219, 2130903572 };
				Snackbar = new int[2] { 2130903599, 2130903600 };
				SnackbarLayout = new int[6] { 16843039, 2130903072, 2130903084, 2130903100, 2130903292, 2130903526 };
				Spinner = new int[5] { 16842930, 16843126, 16843131, 16843362, 2130903556 };
				StateListDrawable = new int[6] { 16843036, 16843156, 16843157, 16843158, 16843532, 16843533 };
				StateListDrawableItem = new int[1] { 16843161 };
				SwitchCompat = new int[14]
				{
					16843044, 16843045, 16843074, 2130903593, 2130903605, 2130903631, 2130903632, 2130903634, 2130903693, 2130903694,
					2130903695, 2130903718, 2130903719, 2130903720
				};
				SwitchMaterial = new int[1] { 2130903723 };
				TabItem = new int[3] { 16842754, 16842994, 16843087 };
				TabLayout = new int[25]
				{
					2130903635, 2130903636, 2130903637, 2130903638, 2130903639, 2130903640, 2130903641, 2130903642, 2130903643, 2130903644,
					2130903645, 2130903646, 2130903647, 2130903648, 2130903649, 2130903650, 2130903651, 2130903652, 2130903653, 2130903654,
					2130903655, 2130903656, 2130903658, 2130903659, 2130903660
				};
				TextAppearance = new int[16]
				{
					16842901, 16842902, 16842903, 16842904, 16842906, 16842907, 16843105, 16843106, 16843107, 16843108,
					16843692, 16844165, 2130903339, 2130903347, 2130903661, 2130903688
				};
				TextInputLayout = new int[50]
				{
					16842906, 16843088, 2130903127, 2130903128, 2130903129, 2130903130, 2130903131, 2130903132, 2130903133, 2130903134,
					2130903135, 2130903136, 2130903254, 2130903255, 2130903256, 2130903257, 2130903258, 2130903259, 2130903296, 2130903297,
					2130903298, 2130903299, 2130903300, 2130903301, 2130903305, 2130903306, 2130903307, 2130903308, 2130903309, 2130903310,
					2130903354, 2130903355, 2130903356, 2130903357, 2130903361, 2130903362, 2130903363, 2130903364, 2130903549, 2130903550,
					2130903551, 2130903552, 2130903553, 2130903582, 2130903585, 2130903608, 2130903609, 2130903610, 2130903611, 2130903612
				};
				ThemeEnforcement = new int[3] { 16842804, 2130903302, 2130903303 };
				Toolbar = new int[30]
				{
					16842927, 16843072, 2130903143, 2130903197, 2130903198, 2130903229, 2130903230, 2130903231, 2130903232, 2130903233,
					2130903234, 2130903504, 2130903505, 2130903527, 2130903532, 2130903535, 2130903536, 2130903556, 2130903626, 2130903627,
					2130903628, 2130903701, 2130903703, 2130903704, 2130903705, 2130903706, 2130903707, 2130903708, 2130903709, 2130903710
				};
				View = new int[5] { 16842752, 16842970, 2130903543, 2130903544, 2130903690 };
				ViewBackgroundHelper = new int[3] { 16842964, 2130903103, 2130903104 };
				ViewPager2 = new int[1] { 16842948 };
				ViewStubCompat = new int[3] { 16842960, 16842994, 16842995 };
				ResourceIdManager.UpdateIdValues();
			}

			private Styleable()
			{
			}
		}

		public class Xml
		{
			public const int image_share_filepaths = 2131886080;

			public const int standalone_badge = 2131886081;

			public const int standalone_badge_gravity_bottom_end = 2131886082;

			public const int standalone_badge_gravity_bottom_start = 2131886083;

			public const int standalone_badge_gravity_top_start = 2131886084;

			public const int xamarin_essentials_fileprovider_file_paths = 2131886085;

			static Xml()
			{
				ResourceIdManager.UpdateIdValues();
			}

			private Xml()
			{
			}
		}

		static Resource()
		{
			ResourceIdManager.UpdateIdValues();
		}

		public static void UpdateIdValues()
		{
			PCLCrypto.Resource.String.ApplicationName = 2131689472;
			PCLCrypto.Resource.String.Hello = 2131689473;
			Xamarin.Auth.Resource.Animation.slide_in_right = 2130772007;
			Xamarin.Auth.Resource.Animation.slide_out_left = 2130772008;
			Xamarin.Auth.Resource.Attribute.alpha = 2130903082;
			Xamarin.Auth.Resource.Attribute.font = 2130903338;
			Xamarin.Auth.Resource.Attribute.fontProviderAuthority = 2130903340;
			Xamarin.Auth.Resource.Attribute.fontProviderCerts = 2130903341;
			Xamarin.Auth.Resource.Attribute.fontProviderFetchStrategy = 2130903342;
			Xamarin.Auth.Resource.Attribute.fontProviderFetchTimeout = 2130903343;
			Xamarin.Auth.Resource.Attribute.fontProviderPackage = 2130903344;
			Xamarin.Auth.Resource.Attribute.fontProviderQuery = 2130903345;
			Xamarin.Auth.Resource.Attribute.fontStyle = 2130903346;
			Xamarin.Auth.Resource.Attribute.fontVariationSettings = 2130903347;
			Xamarin.Auth.Resource.Attribute.fontWeight = 2130903348;
			Xamarin.Auth.Resource.Attribute.ttcIndex = 2130903721;
			Xamarin.Auth.Resource.Color.browser_actions_bg_grey = 2131034150;
			Xamarin.Auth.Resource.Color.browser_actions_divider_color = 2131034151;
			Xamarin.Auth.Resource.Color.browser_actions_text_color = 2131034152;
			Xamarin.Auth.Resource.Color.browser_actions_title_color = 2131034153;
			Xamarin.Auth.Resource.Color.notification_action_color_filter = 2131034318;
			Xamarin.Auth.Resource.Color.notification_icon_bg_color = 2131034319;
			Xamarin.Auth.Resource.Color.ripple_material_light = 2131034331;
			Xamarin.Auth.Resource.Color.secondary_text_default_material_light = 2131034334;
			Xamarin.Auth.Resource.Dimension.browser_actions_context_menu_max_width = 2131099728;
			Xamarin.Auth.Resource.Dimension.browser_actions_context_menu_min_padding = 2131099729;
			Xamarin.Auth.Resource.Dimension.compat_button_inset_horizontal_material = 2131099733;
			Xamarin.Auth.Resource.Dimension.compat_button_inset_vertical_material = 2131099734;
			Xamarin.Auth.Resource.Dimension.compat_button_padding_horizontal_material = 2131099735;
			Xamarin.Auth.Resource.Dimension.compat_button_padding_vertical_material = 2131099736;
			Xamarin.Auth.Resource.Dimension.compat_control_corner_material = 2131099737;
			Xamarin.Auth.Resource.Dimension.compat_notification_large_icon_max_height = 2131099738;
			Xamarin.Auth.Resource.Dimension.compat_notification_large_icon_max_width = 2131099739;
			Xamarin.Auth.Resource.Dimension.notification_action_icon_size = 2131099953;
			Xamarin.Auth.Resource.Dimension.notification_action_text_size = 2131099954;
			Xamarin.Auth.Resource.Dimension.notification_big_circle_margin = 2131099955;
			Xamarin.Auth.Resource.Dimension.notification_content_margin_start = 2131099956;
			Xamarin.Auth.Resource.Dimension.notification_large_icon_height = 2131099957;
			Xamarin.Auth.Resource.Dimension.notification_large_icon_width = 2131099958;
			Xamarin.Auth.Resource.Dimension.notification_main_column_padding_top = 2131099959;
			Xamarin.Auth.Resource.Dimension.notification_media_narrow_margin = 2131099960;
			Xamarin.Auth.Resource.Dimension.notification_right_icon_size = 2131099961;
			Xamarin.Auth.Resource.Dimension.notification_right_side_padding_top = 2131099962;
			Xamarin.Auth.Resource.Dimension.notification_small_icon_background_padding = 2131099963;
			Xamarin.Auth.Resource.Dimension.notification_small_icon_size_as_large = 2131099964;
			Xamarin.Auth.Resource.Dimension.notification_subtext_size = 2131099965;
			Xamarin.Auth.Resource.Dimension.notification_top_pad = 2131099966;
			Xamarin.Auth.Resource.Dimension.notification_top_pad_large_text = 2131099967;
			Xamarin.Auth.Resource.Drawable.ic_arrow_back = 2131165340;
			Xamarin.Auth.Resource.Drawable.notification_action_background = 2131165399;
			Xamarin.Auth.Resource.Drawable.notification_bg = 2131165400;
			Xamarin.Auth.Resource.Drawable.notification_bg_low = 2131165401;
			Xamarin.Auth.Resource.Drawable.notification_bg_low_normal = 2131165402;
			Xamarin.Auth.Resource.Drawable.notification_bg_low_pressed = 2131165403;
			Xamarin.Auth.Resource.Drawable.notification_bg_normal = 2131165404;
			Xamarin.Auth.Resource.Drawable.notification_bg_normal_pressed = 2131165405;
			Xamarin.Auth.Resource.Drawable.notification_icon_background = 2131165406;
			Xamarin.Auth.Resource.Drawable.notification_template_icon_bg = 2131165407;
			Xamarin.Auth.Resource.Drawable.notification_template_icon_low_bg = 2131165408;
			Xamarin.Auth.Resource.Drawable.notification_tile_bg = 2131165409;
			Xamarin.Auth.Resource.Drawable.notify_panel_notification_icon_bg = 2131165410;
			Xamarin.Auth.Resource.Id.action_container = 2131296307;
			Xamarin.Auth.Resource.Id.action_divider = 2131296309;
			Xamarin.Auth.Resource.Id.action_image = 2131296310;
			Xamarin.Auth.Resource.Id.action_text = 2131296316;
			Xamarin.Auth.Resource.Id.actions = 2131296317;
			Xamarin.Auth.Resource.Id.async = 2131296340;
			Xamarin.Auth.Resource.Id.blocking = 2131296346;
			Xamarin.Auth.Resource.Id.browser_actions_header_text = 2131296348;
			Xamarin.Auth.Resource.Id.browser_actions_menu_item_icon = 2131296349;
			Xamarin.Auth.Resource.Id.browser_actions_menu_item_text = 2131296350;
			Xamarin.Auth.Resource.Id.browser_actions_menu_items = 2131296351;
			Xamarin.Auth.Resource.Id.browser_actions_menu_view = 2131296352;
			Xamarin.Auth.Resource.Id.chronometer = 2131296379;
			Xamarin.Auth.Resource.Id.forever = 2131296531;
			Xamarin.Auth.Resource.Id.icon = 2131296552;
			Xamarin.Auth.Resource.Id.icon_group = 2131296553;
			Xamarin.Auth.Resource.Id.info = 2131296593;
			Xamarin.Auth.Resource.Id.italic = 2131296607;
			Xamarin.Auth.Resource.Id.line1 = 2131296616;
			Xamarin.Auth.Resource.Id.line3 = 2131296617;
			Xamarin.Auth.Resource.Id.normal = 2131296683;
			Xamarin.Auth.Resource.Id.notification_background = 2131296684;
			Xamarin.Auth.Resource.Id.notification_main_column = 2131296685;
			Xamarin.Auth.Resource.Id.notification_main_column_container = 2131296686;
			Xamarin.Auth.Resource.Id.right_icon = 2131296744;
			Xamarin.Auth.Resource.Id.right_side = 2131296745;
			Xamarin.Auth.Resource.Id.tag_transition_group = 2131296856;
			Xamarin.Auth.Resource.Id.tag_unhandled_key_event_manager = 2131296857;
			Xamarin.Auth.Resource.Id.tag_unhandled_key_listeners = 2131296858;
			Xamarin.Auth.Resource.Id.text = 2131296863;
			Xamarin.Auth.Resource.Id.text2 = 2131296864;
			Xamarin.Auth.Resource.Id.time = 2131296875;
			Xamarin.Auth.Resource.Id.title = 2131296876;
			Xamarin.Auth.Resource.Id.webview = 2131296904;
			Xamarin.Auth.Resource.Integer.status_bar_notification_info_maxnum = 2131361813;
			Xamarin.Auth.Resource.Layout.activity_webview = 2131492896;
			Xamarin.Auth.Resource.Layout.browser_actions_context_menu_page = 2131492897;
			Xamarin.Auth.Resource.Layout.browser_actions_context_menu_row = 2131492898;
			Xamarin.Auth.Resource.Layout.notification_action = 2131492961;
			Xamarin.Auth.Resource.Layout.notification_action_tombstone = 2131492962;
			Xamarin.Auth.Resource.Layout.notification_template_custom_big = 2131492969;
			Xamarin.Auth.Resource.Layout.notification_template_icon_group = 2131492970;
			Xamarin.Auth.Resource.Layout.notification_template_part_chronometer = 2131492974;
			Xamarin.Auth.Resource.Layout.notification_template_part_time = 2131492975;
			Xamarin.Auth.Resource.String.status_bar_notification_info_overflow = 2131689581;
			Xamarin.Auth.Resource.String.title_activity_webview = 2131689582;
			Xamarin.Auth.Resource.Style.TextAppearance_Compat_Notification = 2131755405;
			Xamarin.Auth.Resource.Style.TextAppearance_Compat_Notification_Info = 2131755406;
			Xamarin.Auth.Resource.Style.TextAppearance_Compat_Notification_Line2 = 2131755408;
			Xamarin.Auth.Resource.Style.TextAppearance_Compat_Notification_Time = 2131755411;
			Xamarin.Auth.Resource.Style.TextAppearance_Compat_Notification_Title = 2131755413;
			Xamarin.Auth.Resource.Style.Widget_Compat_NotificationActionContainer = 2131755639;
			Xamarin.Auth.Resource.Style.Widget_Compat_NotificationActionText = 2131755640;
			Xamarin.Auth.Resource.Styleable.ColorStateListItem = Styleable.ColorStateListItem;
			Xamarin.Auth.Resource.Styleable.ColorStateListItem_alpha = 2;
			Xamarin.Auth.Resource.Styleable.ColorStateListItem_android_alpha = 1;
			Xamarin.Auth.Resource.Styleable.ColorStateListItem_android_color = 0;
			Xamarin.Auth.Resource.Styleable.FontFamily = Styleable.FontFamily;
			Xamarin.Auth.Resource.Styleable.FontFamily_fontProviderAuthority = 0;
			Xamarin.Auth.Resource.Styleable.FontFamily_fontProviderCerts = 1;
			Xamarin.Auth.Resource.Styleable.FontFamily_fontProviderFetchStrategy = 2;
			Xamarin.Auth.Resource.Styleable.FontFamily_fontProviderFetchTimeout = 3;
			Xamarin.Auth.Resource.Styleable.FontFamily_fontProviderPackage = 4;
			Xamarin.Auth.Resource.Styleable.FontFamily_fontProviderQuery = 5;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont = Styleable.FontFamilyFont;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_android_font = 0;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_android_fontStyle = 2;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_android_fontVariationSettings = 4;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_android_fontWeight = 1;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_android_ttcIndex = 3;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_font = 5;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_fontStyle = 6;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_fontVariationSettings = 7;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_fontWeight = 8;
			Xamarin.Auth.Resource.Styleable.FontFamilyFont_ttcIndex = 9;
			Xamarin.Auth.Resource.Styleable.GradientColor = Styleable.GradientColor;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_centerColor = 7;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_centerX = 3;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_centerY = 4;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_endColor = 1;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_endX = 10;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_endY = 11;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_gradientRadius = 5;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_startColor = 0;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_startX = 8;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_startY = 9;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_tileMode = 6;
			Xamarin.Auth.Resource.Styleable.GradientColor_android_type = 2;
			Xamarin.Auth.Resource.Styleable.GradientColorItem = Styleable.GradientColorItem;
			Xamarin.Auth.Resource.Styleable.GradientColorItem_android_color = 0;
			Xamarin.Auth.Resource.Styleable.GradientColorItem_android_offset = 1;
			Xamarin.Essentials.Resource.Attribute.alpha = 2130903082;
			Xamarin.Essentials.Resource.Attribute.coordinatorLayoutStyle = 2130903242;
			Xamarin.Essentials.Resource.Attribute.font = 2130903338;
			Xamarin.Essentials.Resource.Attribute.fontProviderAuthority = 2130903340;
			Xamarin.Essentials.Resource.Attribute.fontProviderCerts = 2130903341;
			Xamarin.Essentials.Resource.Attribute.fontProviderFetchStrategy = 2130903342;
			Xamarin.Essentials.Resource.Attribute.fontProviderFetchTimeout = 2130903343;
			Xamarin.Essentials.Resource.Attribute.fontProviderPackage = 2130903344;
			Xamarin.Essentials.Resource.Attribute.fontProviderQuery = 2130903345;
			Xamarin.Essentials.Resource.Attribute.fontStyle = 2130903346;
			Xamarin.Essentials.Resource.Attribute.fontVariationSettings = 2130903347;
			Xamarin.Essentials.Resource.Attribute.fontWeight = 2130903348;
			Xamarin.Essentials.Resource.Attribute.keylines = 2130903410;
			Xamarin.Essentials.Resource.Attribute.layout_anchor = 2130903416;
			Xamarin.Essentials.Resource.Attribute.layout_anchorGravity = 2130903417;
			Xamarin.Essentials.Resource.Attribute.layout_behavior = 2130903418;
			Xamarin.Essentials.Resource.Attribute.layout_dodgeInsetEdges = 2130903462;
			Xamarin.Essentials.Resource.Attribute.layout_insetEdge = 2130903474;
			Xamarin.Essentials.Resource.Attribute.layout_keyline = 2130903475;
			Xamarin.Essentials.Resource.Attribute.statusBarBackground = 2130903619;
			Xamarin.Essentials.Resource.Attribute.ttcIndex = 2130903721;
			Xamarin.Essentials.Resource.Color.browser_actions_bg_grey = 2131034150;
			Xamarin.Essentials.Resource.Color.browser_actions_divider_color = 2131034151;
			Xamarin.Essentials.Resource.Color.browser_actions_text_color = 2131034152;
			Xamarin.Essentials.Resource.Color.browser_actions_title_color = 2131034153;
			Xamarin.Essentials.Resource.Color.notification_action_color_filter = 2131034318;
			Xamarin.Essentials.Resource.Color.notification_icon_bg_color = 2131034319;
			Xamarin.Essentials.Resource.Color.ripple_material_light = 2131034331;
			Xamarin.Essentials.Resource.Color.secondary_text_default_material_light = 2131034334;
			Xamarin.Essentials.Resource.Dimension.browser_actions_context_menu_max_width = 2131099728;
			Xamarin.Essentials.Resource.Dimension.browser_actions_context_menu_min_padding = 2131099729;
			Xamarin.Essentials.Resource.Dimension.compat_button_inset_horizontal_material = 2131099733;
			Xamarin.Essentials.Resource.Dimension.compat_button_inset_vertical_material = 2131099734;
			Xamarin.Essentials.Resource.Dimension.compat_button_padding_horizontal_material = 2131099735;
			Xamarin.Essentials.Resource.Dimension.compat_button_padding_vertical_material = 2131099736;
			Xamarin.Essentials.Resource.Dimension.compat_control_corner_material = 2131099737;
			Xamarin.Essentials.Resource.Dimension.compat_notification_large_icon_max_height = 2131099738;
			Xamarin.Essentials.Resource.Dimension.compat_notification_large_icon_max_width = 2131099739;
			Xamarin.Essentials.Resource.Dimension.notification_action_icon_size = 2131099953;
			Xamarin.Essentials.Resource.Dimension.notification_action_text_size = 2131099954;
			Xamarin.Essentials.Resource.Dimension.notification_big_circle_margin = 2131099955;
			Xamarin.Essentials.Resource.Dimension.notification_content_margin_start = 2131099956;
			Xamarin.Essentials.Resource.Dimension.notification_large_icon_height = 2131099957;
			Xamarin.Essentials.Resource.Dimension.notification_large_icon_width = 2131099958;
			Xamarin.Essentials.Resource.Dimension.notification_main_column_padding_top = 2131099959;
			Xamarin.Essentials.Resource.Dimension.notification_media_narrow_margin = 2131099960;
			Xamarin.Essentials.Resource.Dimension.notification_right_icon_size = 2131099961;
			Xamarin.Essentials.Resource.Dimension.notification_right_side_padding_top = 2131099962;
			Xamarin.Essentials.Resource.Dimension.notification_small_icon_background_padding = 2131099963;
			Xamarin.Essentials.Resource.Dimension.notification_small_icon_size_as_large = 2131099964;
			Xamarin.Essentials.Resource.Dimension.notification_subtext_size = 2131099965;
			Xamarin.Essentials.Resource.Dimension.notification_top_pad = 2131099966;
			Xamarin.Essentials.Resource.Dimension.notification_top_pad_large_text = 2131099967;
			Xamarin.Essentials.Resource.Drawable.notification_action_background = 2131165399;
			Xamarin.Essentials.Resource.Drawable.notification_bg = 2131165400;
			Xamarin.Essentials.Resource.Drawable.notification_bg_low = 2131165401;
			Xamarin.Essentials.Resource.Drawable.notification_bg_low_normal = 2131165402;
			Xamarin.Essentials.Resource.Drawable.notification_bg_low_pressed = 2131165403;
			Xamarin.Essentials.Resource.Drawable.notification_bg_normal = 2131165404;
			Xamarin.Essentials.Resource.Drawable.notification_bg_normal_pressed = 2131165405;
			Xamarin.Essentials.Resource.Drawable.notification_icon_background = 2131165406;
			Xamarin.Essentials.Resource.Drawable.notification_template_icon_bg = 2131165407;
			Xamarin.Essentials.Resource.Drawable.notification_template_icon_low_bg = 2131165408;
			Xamarin.Essentials.Resource.Drawable.notification_tile_bg = 2131165409;
			Xamarin.Essentials.Resource.Drawable.notify_panel_notification_icon_bg = 2131165410;
			Xamarin.Essentials.Resource.Id.accessibility_action_clickable_span = 2131296266;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_0 = 2131296267;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_1 = 2131296268;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_10 = 2131296269;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_11 = 2131296270;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_12 = 2131296271;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_13 = 2131296272;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_14 = 2131296273;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_15 = 2131296274;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_16 = 2131296275;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_17 = 2131296276;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_18 = 2131296277;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_19 = 2131296278;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_2 = 2131296279;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_20 = 2131296280;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_21 = 2131296281;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_22 = 2131296282;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_23 = 2131296283;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_24 = 2131296284;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_25 = 2131296285;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_26 = 2131296286;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_27 = 2131296287;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_28 = 2131296288;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_29 = 2131296289;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_3 = 2131296290;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_30 = 2131296291;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_31 = 2131296292;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_4 = 2131296293;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_5 = 2131296294;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_6 = 2131296295;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_7 = 2131296296;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_8 = 2131296297;
			Xamarin.Essentials.Resource.Id.accessibility_custom_action_9 = 2131296298;
			Xamarin.Essentials.Resource.Id.actions = 2131296317;
			Xamarin.Essentials.Resource.Id.action_container = 2131296307;
			Xamarin.Essentials.Resource.Id.action_divider = 2131296309;
			Xamarin.Essentials.Resource.Id.action_image = 2131296310;
			Xamarin.Essentials.Resource.Id.action_text = 2131296316;
			Xamarin.Essentials.Resource.Id.all = 2131296325;
			Xamarin.Essentials.Resource.Id.async = 2131296340;
			Xamarin.Essentials.Resource.Id.blocking = 2131296346;
			Xamarin.Essentials.Resource.Id.bottom = 2131296347;
			Xamarin.Essentials.Resource.Id.browser_actions_header_text = 2131296348;
			Xamarin.Essentials.Resource.Id.browser_actions_menu_items = 2131296351;
			Xamarin.Essentials.Resource.Id.browser_actions_menu_item_icon = 2131296349;
			Xamarin.Essentials.Resource.Id.browser_actions_menu_item_text = 2131296350;
			Xamarin.Essentials.Resource.Id.browser_actions_menu_view = 2131296352;
			Xamarin.Essentials.Resource.Id.center = 2131296370;
			Xamarin.Essentials.Resource.Id.center_horizontal = 2131296371;
			Xamarin.Essentials.Resource.Id.center_vertical = 2131296372;
			Xamarin.Essentials.Resource.Id.chronometer = 2131296379;
			Xamarin.Essentials.Resource.Id.clip_horizontal = 2131296381;
			Xamarin.Essentials.Resource.Id.clip_vertical = 2131296382;
			Xamarin.Essentials.Resource.Id.dialog_button = 2131296444;
			Xamarin.Essentials.Resource.Id.end = 2131296503;
			Xamarin.Essentials.Resource.Id.fill = 2131296518;
			Xamarin.Essentials.Resource.Id.fill_horizontal = 2131296519;
			Xamarin.Essentials.Resource.Id.fill_vertical = 2131296520;
			Xamarin.Essentials.Resource.Id.forever = 2131296531;
			Xamarin.Essentials.Resource.Id.icon = 2131296552;
			Xamarin.Essentials.Resource.Id.icon_group = 2131296553;
			Xamarin.Essentials.Resource.Id.info = 2131296593;
			Xamarin.Essentials.Resource.Id.italic = 2131296607;
			Xamarin.Essentials.Resource.Id.left = 2131296614;
			Xamarin.Essentials.Resource.Id.line1 = 2131296616;
			Xamarin.Essentials.Resource.Id.line3 = 2131296617;
			Xamarin.Essentials.Resource.Id.none = 2131296682;
			Xamarin.Essentials.Resource.Id.normal = 2131296683;
			Xamarin.Essentials.Resource.Id.notification_background = 2131296684;
			Xamarin.Essentials.Resource.Id.notification_main_column = 2131296685;
			Xamarin.Essentials.Resource.Id.notification_main_column_container = 2131296686;
			Xamarin.Essentials.Resource.Id.right = 2131296743;
			Xamarin.Essentials.Resource.Id.right_icon = 2131296744;
			Xamarin.Essentials.Resource.Id.right_side = 2131296745;
			Xamarin.Essentials.Resource.Id.start = 2131296840;
			Xamarin.Essentials.Resource.Id.tag_accessibility_actions = 2131296851;
			Xamarin.Essentials.Resource.Id.tag_accessibility_clickable_spans = 2131296852;
			Xamarin.Essentials.Resource.Id.tag_accessibility_heading = 2131296853;
			Xamarin.Essentials.Resource.Id.tag_accessibility_pane_title = 2131296854;
			Xamarin.Essentials.Resource.Id.tag_screen_reader_focusable = 2131296855;
			Xamarin.Essentials.Resource.Id.tag_transition_group = 2131296856;
			Xamarin.Essentials.Resource.Id.tag_unhandled_key_event_manager = 2131296857;
			Xamarin.Essentials.Resource.Id.tag_unhandled_key_listeners = 2131296858;
			Xamarin.Essentials.Resource.Id.text = 2131296863;
			Xamarin.Essentials.Resource.Id.text2 = 2131296864;
			Xamarin.Essentials.Resource.Id.time = 2131296875;
			Xamarin.Essentials.Resource.Id.title = 2131296876;
			Xamarin.Essentials.Resource.Id.top = 2131296881;
			Xamarin.Essentials.Resource.Integer.status_bar_notification_info_maxnum = 2131361813;
			Xamarin.Essentials.Resource.Layout.browser_actions_context_menu_page = 2131492897;
			Xamarin.Essentials.Resource.Layout.browser_actions_context_menu_row = 2131492898;
			Xamarin.Essentials.Resource.Layout.custom_dialog = 2131492906;
			Xamarin.Essentials.Resource.Layout.notification_action = 2131492961;
			Xamarin.Essentials.Resource.Layout.notification_action_tombstone = 2131492962;
			Xamarin.Essentials.Resource.Layout.notification_template_custom_big = 2131492969;
			Xamarin.Essentials.Resource.Layout.notification_template_icon_group = 2131492970;
			Xamarin.Essentials.Resource.Layout.notification_template_part_chronometer = 2131492974;
			Xamarin.Essentials.Resource.Layout.notification_template_part_time = 2131492975;
			Xamarin.Essentials.Resource.String.status_bar_notification_info_overflow = 2131689581;
			Xamarin.Essentials.Resource.Style.TextAppearance_Compat_Notification = 2131755405;
			Xamarin.Essentials.Resource.Style.TextAppearance_Compat_Notification_Info = 2131755406;
			Xamarin.Essentials.Resource.Style.TextAppearance_Compat_Notification_Line2 = 2131755408;
			Xamarin.Essentials.Resource.Style.TextAppearance_Compat_Notification_Time = 2131755411;
			Xamarin.Essentials.Resource.Style.TextAppearance_Compat_Notification_Title = 2131755413;
			Xamarin.Essentials.Resource.Style.Widget_Compat_NotificationActionContainer = 2131755639;
			Xamarin.Essentials.Resource.Style.Widget_Compat_NotificationActionText = 2131755640;
			Xamarin.Essentials.Resource.Style.Widget_Support_CoordinatorLayout = 2131755743;
			Xamarin.Essentials.Resource.Styleable.ColorStateListItem = Styleable.ColorStateListItem;
			Xamarin.Essentials.Resource.Styleable.ColorStateListItem_alpha = 2;
			Xamarin.Essentials.Resource.Styleable.ColorStateListItem_android_alpha = 1;
			Xamarin.Essentials.Resource.Styleable.ColorStateListItem_android_color = 0;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout = Styleable.CoordinatorLayout;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_keylines = 0;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_Layout = Styleable.CoordinatorLayout_Layout;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_Layout_android_layout_gravity = 0;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_Layout_layout_anchor = 1;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_Layout_layout_anchorGravity = 2;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_Layout_layout_behavior = 3;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_Layout_layout_dodgeInsetEdges = 4;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_Layout_layout_insetEdge = 5;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_Layout_layout_keyline = 6;
			Xamarin.Essentials.Resource.Styleable.CoordinatorLayout_statusBarBackground = 1;
			Xamarin.Essentials.Resource.Styleable.FontFamily = Styleable.FontFamily;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont = Styleable.FontFamilyFont;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_android_font = 0;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_android_fontStyle = 2;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_android_fontVariationSettings = 4;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_android_fontWeight = 1;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_android_ttcIndex = 3;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_font = 5;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_fontStyle = 6;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_fontVariationSettings = 7;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_fontWeight = 8;
			Xamarin.Essentials.Resource.Styleable.FontFamilyFont_ttcIndex = 9;
			Xamarin.Essentials.Resource.Styleable.FontFamily_fontProviderAuthority = 0;
			Xamarin.Essentials.Resource.Styleable.FontFamily_fontProviderCerts = 1;
			Xamarin.Essentials.Resource.Styleable.FontFamily_fontProviderFetchStrategy = 2;
			Xamarin.Essentials.Resource.Styleable.FontFamily_fontProviderFetchTimeout = 3;
			Xamarin.Essentials.Resource.Styleable.FontFamily_fontProviderPackage = 4;
			Xamarin.Essentials.Resource.Styleable.FontFamily_fontProviderQuery = 5;
			Xamarin.Essentials.Resource.Styleable.GradientColor = Styleable.GradientColor;
			Xamarin.Essentials.Resource.Styleable.GradientColorItem = Styleable.GradientColorItem;
			Xamarin.Essentials.Resource.Styleable.GradientColorItem_android_color = 0;
			Xamarin.Essentials.Resource.Styleable.GradientColorItem_android_offset = 1;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_centerColor = 7;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_centerX = 3;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_centerY = 4;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_endColor = 1;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_endX = 10;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_endY = 11;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_gradientRadius = 5;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_startColor = 0;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_startX = 8;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_startY = 9;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_tileMode = 6;
			Xamarin.Essentials.Resource.Styleable.GradientColor_android_type = 2;
			Xamarin.Essentials.Resource.Xml.xamarin_essentials_fileprovider_file_paths = 2131886085;
			Xamarin.ExposureNotification.Resource.Attribute.alpha = 2130903082;
			Xamarin.ExposureNotification.Resource.Attribute.buttonSize = 2130903146;
			Xamarin.ExposureNotification.Resource.Attribute.circleCrop = 2130903188;
			Xamarin.ExposureNotification.Resource.Attribute.colorScheme = 2130903219;
			Xamarin.ExposureNotification.Resource.Attribute.coordinatorLayoutStyle = 2130903242;
			Xamarin.ExposureNotification.Resource.Attribute.font = 2130903338;
			Xamarin.ExposureNotification.Resource.Attribute.fontProviderAuthority = 2130903340;
			Xamarin.ExposureNotification.Resource.Attribute.fontProviderCerts = 2130903341;
			Xamarin.ExposureNotification.Resource.Attribute.fontProviderFetchStrategy = 2130903342;
			Xamarin.ExposureNotification.Resource.Attribute.fontProviderFetchTimeout = 2130903343;
			Xamarin.ExposureNotification.Resource.Attribute.fontProviderPackage = 2130903344;
			Xamarin.ExposureNotification.Resource.Attribute.fontProviderQuery = 2130903345;
			Xamarin.ExposureNotification.Resource.Attribute.fontStyle = 2130903346;
			Xamarin.ExposureNotification.Resource.Attribute.fontVariationSettings = 2130903347;
			Xamarin.ExposureNotification.Resource.Attribute.fontWeight = 2130903348;
			Xamarin.ExposureNotification.Resource.Attribute.imageAspectRatio = 2130903377;
			Xamarin.ExposureNotification.Resource.Attribute.imageAspectRatioAdjust = 2130903378;
			Xamarin.ExposureNotification.Resource.Attribute.keylines = 2130903410;
			Xamarin.ExposureNotification.Resource.Attribute.layout_anchor = 2130903416;
			Xamarin.ExposureNotification.Resource.Attribute.layout_anchorGravity = 2130903417;
			Xamarin.ExposureNotification.Resource.Attribute.layout_behavior = 2130903418;
			Xamarin.ExposureNotification.Resource.Attribute.layout_dodgeInsetEdges = 2130903462;
			Xamarin.ExposureNotification.Resource.Attribute.layout_insetEdge = 2130903474;
			Xamarin.ExposureNotification.Resource.Attribute.layout_keyline = 2130903475;
			Xamarin.ExposureNotification.Resource.Attribute.scopeUris = 2130903572;
			Xamarin.ExposureNotification.Resource.Attribute.statusBarBackground = 2130903619;
			Xamarin.ExposureNotification.Resource.Attribute.ttcIndex = 2130903721;
			Xamarin.ExposureNotification.Resource.Boolean.enable_system_alarm_service_default = 2130968579;
			Xamarin.ExposureNotification.Resource.Boolean.enable_system_foreground_service_default = 2130968580;
			Xamarin.ExposureNotification.Resource.Boolean.enable_system_job_service_default = 2130968581;
			Xamarin.ExposureNotification.Resource.Boolean.workmanager_test_configuration = 2130968583;
			Xamarin.ExposureNotification.Resource.Color.browser_actions_bg_grey = 2131034150;
			Xamarin.ExposureNotification.Resource.Color.browser_actions_divider_color = 2131034151;
			Xamarin.ExposureNotification.Resource.Color.browser_actions_text_color = 2131034152;
			Xamarin.ExposureNotification.Resource.Color.browser_actions_title_color = 2131034153;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_dark = 2131034167;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_dark_default = 2131034168;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_dark_disabled = 2131034169;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_dark_focused = 2131034170;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_dark_pressed = 2131034171;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_light = 2131034172;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_light_default = 2131034173;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_light_disabled = 2131034174;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_light_focused = 2131034175;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_text_light_pressed = 2131034176;
			Xamarin.ExposureNotification.Resource.Color.common_google_signin_btn_tint = 2131034177;
			Xamarin.ExposureNotification.Resource.Color.notification_action_color_filter = 2131034318;
			Xamarin.ExposureNotification.Resource.Color.notification_icon_bg_color = 2131034319;
			Xamarin.ExposureNotification.Resource.Color.ripple_material_light = 2131034331;
			Xamarin.ExposureNotification.Resource.Color.secondary_text_default_material_light = 2131034334;
			Xamarin.ExposureNotification.Resource.Dimension.browser_actions_context_menu_max_width = 2131099728;
			Xamarin.ExposureNotification.Resource.Dimension.browser_actions_context_menu_min_padding = 2131099729;
			Xamarin.ExposureNotification.Resource.Dimension.compat_button_inset_horizontal_material = 2131099733;
			Xamarin.ExposureNotification.Resource.Dimension.compat_button_inset_vertical_material = 2131099734;
			Xamarin.ExposureNotification.Resource.Dimension.compat_button_padding_horizontal_material = 2131099735;
			Xamarin.ExposureNotification.Resource.Dimension.compat_button_padding_vertical_material = 2131099736;
			Xamarin.ExposureNotification.Resource.Dimension.compat_control_corner_material = 2131099737;
			Xamarin.ExposureNotification.Resource.Dimension.compat_notification_large_icon_max_height = 2131099738;
			Xamarin.ExposureNotification.Resource.Dimension.compat_notification_large_icon_max_width = 2131099739;
			Xamarin.ExposureNotification.Resource.Dimension.notification_action_icon_size = 2131099953;
			Xamarin.ExposureNotification.Resource.Dimension.notification_action_text_size = 2131099954;
			Xamarin.ExposureNotification.Resource.Dimension.notification_big_circle_margin = 2131099955;
			Xamarin.ExposureNotification.Resource.Dimension.notification_content_margin_start = 2131099956;
			Xamarin.ExposureNotification.Resource.Dimension.notification_large_icon_height = 2131099957;
			Xamarin.ExposureNotification.Resource.Dimension.notification_large_icon_width = 2131099958;
			Xamarin.ExposureNotification.Resource.Dimension.notification_main_column_padding_top = 2131099959;
			Xamarin.ExposureNotification.Resource.Dimension.notification_media_narrow_margin = 2131099960;
			Xamarin.ExposureNotification.Resource.Dimension.notification_right_icon_size = 2131099961;
			Xamarin.ExposureNotification.Resource.Dimension.notification_right_side_padding_top = 2131099962;
			Xamarin.ExposureNotification.Resource.Dimension.notification_small_icon_background_padding = 2131099963;
			Xamarin.ExposureNotification.Resource.Dimension.notification_small_icon_size_as_large = 2131099964;
			Xamarin.ExposureNotification.Resource.Dimension.notification_subtext_size = 2131099965;
			Xamarin.ExposureNotification.Resource.Dimension.notification_top_pad = 2131099966;
			Xamarin.ExposureNotification.Resource.Dimension.notification_top_pad_large_text = 2131099967;
			Xamarin.ExposureNotification.Resource.Drawable.common_full_open_on_phone = 2131165298;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_icon_dark = 2131165299;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_icon_dark_focused = 2131165300;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_icon_dark_normal = 2131165301;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_icon_dark_normal_background = 2131165302;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_icon_disabled = 2131165303;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_icon_light = 2131165304;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_icon_light_focused = 2131165305;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_icon_light_normal = 2131165306;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_icon_light_normal_background = 2131165307;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_text_dark = 2131165308;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_text_dark_focused = 2131165309;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_text_dark_normal = 2131165310;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_text_dark_normal_background = 2131165311;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_text_disabled = 2131165312;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_text_light = 2131165313;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_text_light_focused = 2131165314;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_text_light_normal = 2131165315;
			Xamarin.ExposureNotification.Resource.Drawable.common_google_signin_btn_text_light_normal_background = 2131165316;
			Xamarin.ExposureNotification.Resource.Drawable.googleg_disabled_color_18 = 2131165335;
			Xamarin.ExposureNotification.Resource.Drawable.googleg_standard_color_18 = 2131165336;
			Xamarin.ExposureNotification.Resource.Drawable.notification_action_background = 2131165399;
			Xamarin.ExposureNotification.Resource.Drawable.notification_bg = 2131165400;
			Xamarin.ExposureNotification.Resource.Drawable.notification_bg_low = 2131165401;
			Xamarin.ExposureNotification.Resource.Drawable.notification_bg_low_normal = 2131165402;
			Xamarin.ExposureNotification.Resource.Drawable.notification_bg_low_pressed = 2131165403;
			Xamarin.ExposureNotification.Resource.Drawable.notification_bg_normal = 2131165404;
			Xamarin.ExposureNotification.Resource.Drawable.notification_bg_normal_pressed = 2131165405;
			Xamarin.ExposureNotification.Resource.Drawable.notification_icon_background = 2131165406;
			Xamarin.ExposureNotification.Resource.Drawable.notification_template_icon_bg = 2131165407;
			Xamarin.ExposureNotification.Resource.Drawable.notification_template_icon_low_bg = 2131165408;
			Xamarin.ExposureNotification.Resource.Drawable.notification_tile_bg = 2131165409;
			Xamarin.ExposureNotification.Resource.Drawable.notify_panel_notification_icon_bg = 2131165410;
			Xamarin.ExposureNotification.Resource.Id.accessibility_action_clickable_span = 2131296266;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_0 = 2131296267;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_1 = 2131296268;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_10 = 2131296269;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_11 = 2131296270;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_12 = 2131296271;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_13 = 2131296272;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_14 = 2131296273;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_15 = 2131296274;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_16 = 2131296275;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_17 = 2131296276;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_18 = 2131296277;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_19 = 2131296278;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_2 = 2131296279;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_20 = 2131296280;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_21 = 2131296281;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_22 = 2131296282;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_23 = 2131296283;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_24 = 2131296284;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_25 = 2131296285;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_26 = 2131296286;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_27 = 2131296287;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_28 = 2131296288;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_29 = 2131296289;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_3 = 2131296290;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_30 = 2131296291;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_31 = 2131296292;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_4 = 2131296293;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_5 = 2131296294;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_6 = 2131296295;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_7 = 2131296296;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_8 = 2131296297;
			Xamarin.ExposureNotification.Resource.Id.accessibility_custom_action_9 = 2131296298;
			Xamarin.ExposureNotification.Resource.Id.actions = 2131296317;
			Xamarin.ExposureNotification.Resource.Id.action_container = 2131296307;
			Xamarin.ExposureNotification.Resource.Id.action_divider = 2131296309;
			Xamarin.ExposureNotification.Resource.Id.action_image = 2131296310;
			Xamarin.ExposureNotification.Resource.Id.action_text = 2131296316;
			Xamarin.ExposureNotification.Resource.Id.adjust_height = 2131296322;
			Xamarin.ExposureNotification.Resource.Id.adjust_width = 2131296323;
			Xamarin.ExposureNotification.Resource.Id.all = 2131296325;
			Xamarin.ExposureNotification.Resource.Id.async = 2131296340;
			Xamarin.ExposureNotification.Resource.Id.auto = 2131296341;
			Xamarin.ExposureNotification.Resource.Id.blocking = 2131296346;
			Xamarin.ExposureNotification.Resource.Id.bottom = 2131296347;
			Xamarin.ExposureNotification.Resource.Id.browser_actions_header_text = 2131296348;
			Xamarin.ExposureNotification.Resource.Id.browser_actions_menu_items = 2131296351;
			Xamarin.ExposureNotification.Resource.Id.browser_actions_menu_item_icon = 2131296349;
			Xamarin.ExposureNotification.Resource.Id.browser_actions_menu_item_text = 2131296350;
			Xamarin.ExposureNotification.Resource.Id.browser_actions_menu_view = 2131296352;
			Xamarin.ExposureNotification.Resource.Id.center = 2131296370;
			Xamarin.ExposureNotification.Resource.Id.center_horizontal = 2131296371;
			Xamarin.ExposureNotification.Resource.Id.center_vertical = 2131296372;
			Xamarin.ExposureNotification.Resource.Id.chronometer = 2131296379;
			Xamarin.ExposureNotification.Resource.Id.clip_horizontal = 2131296381;
			Xamarin.ExposureNotification.Resource.Id.clip_vertical = 2131296382;
			Xamarin.ExposureNotification.Resource.Id.dark = 2131296432;
			Xamarin.ExposureNotification.Resource.Id.dialog_button = 2131296444;
			Xamarin.ExposureNotification.Resource.Id.end = 2131296503;
			Xamarin.ExposureNotification.Resource.Id.fill = 2131296518;
			Xamarin.ExposureNotification.Resource.Id.fill_horizontal = 2131296519;
			Xamarin.ExposureNotification.Resource.Id.fill_vertical = 2131296520;
			Xamarin.ExposureNotification.Resource.Id.forever = 2131296531;
			Xamarin.ExposureNotification.Resource.Id.icon = 2131296552;
			Xamarin.ExposureNotification.Resource.Id.icon_group = 2131296553;
			Xamarin.ExposureNotification.Resource.Id.icon_only = 2131296554;
			Xamarin.ExposureNotification.Resource.Id.info = 2131296593;
			Xamarin.ExposureNotification.Resource.Id.italic = 2131296607;
			Xamarin.ExposureNotification.Resource.Id.left = 2131296614;
			Xamarin.ExposureNotification.Resource.Id.light = 2131296615;
			Xamarin.ExposureNotification.Resource.Id.line1 = 2131296616;
			Xamarin.ExposureNotification.Resource.Id.line3 = 2131296617;
			Xamarin.ExposureNotification.Resource.Id.none = 2131296682;
			Xamarin.ExposureNotification.Resource.Id.normal = 2131296683;
			Xamarin.ExposureNotification.Resource.Id.notification_background = 2131296684;
			Xamarin.ExposureNotification.Resource.Id.notification_main_column = 2131296685;
			Xamarin.ExposureNotification.Resource.Id.notification_main_column_container = 2131296686;
			Xamarin.ExposureNotification.Resource.Id.right = 2131296743;
			Xamarin.ExposureNotification.Resource.Id.right_icon = 2131296744;
			Xamarin.ExposureNotification.Resource.Id.right_side = 2131296745;
			Xamarin.ExposureNotification.Resource.Id.standard = 2131296839;
			Xamarin.ExposureNotification.Resource.Id.start = 2131296840;
			Xamarin.ExposureNotification.Resource.Id.tag_accessibility_actions = 2131296851;
			Xamarin.ExposureNotification.Resource.Id.tag_accessibility_clickable_spans = 2131296852;
			Xamarin.ExposureNotification.Resource.Id.tag_accessibility_heading = 2131296853;
			Xamarin.ExposureNotification.Resource.Id.tag_accessibility_pane_title = 2131296854;
			Xamarin.ExposureNotification.Resource.Id.tag_screen_reader_focusable = 2131296855;
			Xamarin.ExposureNotification.Resource.Id.tag_transition_group = 2131296856;
			Xamarin.ExposureNotification.Resource.Id.tag_unhandled_key_event_manager = 2131296857;
			Xamarin.ExposureNotification.Resource.Id.tag_unhandled_key_listeners = 2131296858;
			Xamarin.ExposureNotification.Resource.Id.text = 2131296863;
			Xamarin.ExposureNotification.Resource.Id.text2 = 2131296864;
			Xamarin.ExposureNotification.Resource.Id.time = 2131296875;
			Xamarin.ExposureNotification.Resource.Id.title = 2131296876;
			Xamarin.ExposureNotification.Resource.Id.top = 2131296881;
			Xamarin.ExposureNotification.Resource.Id.wide = 2131296939;
			Xamarin.ExposureNotification.Resource.Integer.google_play_services_version = 2131361800;
			Xamarin.ExposureNotification.Resource.Integer.status_bar_notification_info_maxnum = 2131361813;
			Xamarin.ExposureNotification.Resource.Layout.browser_actions_context_menu_page = 2131492897;
			Xamarin.ExposureNotification.Resource.Layout.browser_actions_context_menu_row = 2131492898;
			Xamarin.ExposureNotification.Resource.Layout.custom_dialog = 2131492906;
			Xamarin.ExposureNotification.Resource.Layout.notification_action = 2131492961;
			Xamarin.ExposureNotification.Resource.Layout.notification_action_tombstone = 2131492962;
			Xamarin.ExposureNotification.Resource.Layout.notification_template_custom_big = 2131492969;
			Xamarin.ExposureNotification.Resource.Layout.notification_template_icon_group = 2131492970;
			Xamarin.ExposureNotification.Resource.Layout.notification_template_part_chronometer = 2131492974;
			Xamarin.ExposureNotification.Resource.Layout.notification_template_part_time = 2131492975;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_enable_button = 2131689512;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_enable_text = 2131689513;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_enable_title = 2131689514;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_install_button = 2131689515;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_install_text = 2131689516;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_install_title = 2131689517;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_notification_channel_name = 2131689518;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_notification_ticker = 2131689519;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_unknown_issue = 2131689520;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_unsupported_text = 2131689521;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_update_button = 2131689522;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_update_text = 2131689523;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_update_title = 2131689524;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_updating_text = 2131689525;
			Xamarin.ExposureNotification.Resource.String.common_google_play_services_wear_update_text = 2131689526;
			Xamarin.ExposureNotification.Resource.String.common_open_on_phone = 2131689527;
			Xamarin.ExposureNotification.Resource.String.common_signin_button_text = 2131689528;
			Xamarin.ExposureNotification.Resource.String.common_signin_button_text_long = 2131689529;
			Xamarin.ExposureNotification.Resource.String.status_bar_notification_info_overflow = 2131689581;
			Xamarin.ExposureNotification.Resource.Style.TextAppearance_Compat_Notification = 2131755405;
			Xamarin.ExposureNotification.Resource.Style.TextAppearance_Compat_Notification_Info = 2131755406;
			Xamarin.ExposureNotification.Resource.Style.TextAppearance_Compat_Notification_Line2 = 2131755408;
			Xamarin.ExposureNotification.Resource.Style.TextAppearance_Compat_Notification_Time = 2131755411;
			Xamarin.ExposureNotification.Resource.Style.TextAppearance_Compat_Notification_Title = 2131755413;
			Xamarin.ExposureNotification.Resource.Style.Widget_Compat_NotificationActionContainer = 2131755639;
			Xamarin.ExposureNotification.Resource.Style.Widget_Compat_NotificationActionText = 2131755640;
			Xamarin.ExposureNotification.Resource.Style.Widget_Support_CoordinatorLayout = 2131755743;
			Xamarin.ExposureNotification.Resource.Styleable.ColorStateListItem = Styleable.ColorStateListItem;
			Xamarin.ExposureNotification.Resource.Styleable.ColorStateListItem_alpha = 2;
			Xamarin.ExposureNotification.Resource.Styleable.ColorStateListItem_android_alpha = 1;
			Xamarin.ExposureNotification.Resource.Styleable.ColorStateListItem_android_color = 0;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout = Styleable.CoordinatorLayout;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_keylines = 0;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_Layout = Styleable.CoordinatorLayout_Layout;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_Layout_android_layout_gravity = 0;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_Layout_layout_anchor = 1;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_Layout_layout_anchorGravity = 2;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_Layout_layout_behavior = 3;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_Layout_layout_dodgeInsetEdges = 4;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_Layout_layout_insetEdge = 5;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_Layout_layout_keyline = 6;
			Xamarin.ExposureNotification.Resource.Styleable.CoordinatorLayout_statusBarBackground = 1;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamily = Styleable.FontFamily;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont = Styleable.FontFamilyFont;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_android_font = 0;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_android_fontStyle = 2;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_android_fontVariationSettings = 4;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_android_fontWeight = 1;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_android_ttcIndex = 3;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_font = 5;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_fontStyle = 6;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_fontVariationSettings = 7;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_fontWeight = 8;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamilyFont_ttcIndex = 9;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamily_fontProviderAuthority = 0;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamily_fontProviderCerts = 1;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamily_fontProviderFetchStrategy = 2;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamily_fontProviderFetchTimeout = 3;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamily_fontProviderPackage = 4;
			Xamarin.ExposureNotification.Resource.Styleable.FontFamily_fontProviderQuery = 5;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor = Styleable.GradientColor;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColorItem = Styleable.GradientColorItem;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColorItem_android_color = 0;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColorItem_android_offset = 1;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_centerColor = 7;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_centerX = 3;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_centerY = 4;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_endColor = 1;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_endX = 10;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_endY = 11;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_gradientRadius = 5;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_startColor = 0;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_startX = 8;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_startY = 9;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_tileMode = 6;
			Xamarin.ExposureNotification.Resource.Styleable.GradientColor_android_type = 2;
			Xamarin.ExposureNotification.Resource.Styleable.LoadingImageView = Styleable.LoadingImageView;
			Xamarin.ExposureNotification.Resource.Styleable.LoadingImageView_circleCrop = 0;
			Xamarin.ExposureNotification.Resource.Styleable.LoadingImageView_imageAspectRatio = 1;
			Xamarin.ExposureNotification.Resource.Styleable.LoadingImageView_imageAspectRatioAdjust = 2;
			Xamarin.ExposureNotification.Resource.Styleable.SignInButton = Styleable.SignInButton;
			Xamarin.ExposureNotification.Resource.Styleable.SignInButton_buttonSize = 0;
			Xamarin.ExposureNotification.Resource.Styleable.SignInButton_colorScheme = 1;
			Xamarin.ExposureNotification.Resource.Styleable.SignInButton_scopeUris = 2;
			Xamarin.ExposureNotification.Resource.Xml.xamarin_essentials_fileprovider_file_paths = 2131886085;
			Yort.Ntp.Resource.String.ApplicationName = 2131689472;
			Yort.Ntp.Resource.String.Hello = 2131689473;
		}
	}
	public static class DroidDependencyInjectionConfig
	{
		public static UnityContainer unityContainer;

		public static void Init()
		{
			unityContainer = new UnityContainer();
			unityContainer.RegisterType<IApiDataHelper, DroidApiDataHelperHandler>(Array.Empty<InjectionMember>());
			unityContainer.RegisterType<IDialogService, DroidDialogService>(Array.Empty<InjectionMember>());
			unityContainer.RegisterType<ILocalNotificationsManager, LocalNotificationsManager>(new ContainerControlledLifetimeManager(), Array.Empty<InjectionMember>());
			unityContainer.RegisterType<IPermissionsHelper, PermissionUtils>(Array.Empty<InjectionMember>());
			CommonDependencyInjectionConfig.Init(unityContainer);
			UnityServiceLocator unityServiceLocalter = new UnityServiceLocator(unityContainer);
			ServiceLocator.SetLocatorProvider(() => unityServiceLocalter);
		}
	}
	public static class StringExtensions
	{
		public static Color ToColor(this string hexString)
		{
			hexString = hexString.Replace("#", "");
			if (hexString.Length == 3)
			{
				hexString += hexString;
			}
			if (hexString.Length != 6)
			{
				throw new System.Exception("Invalid hex string");
			}
			int r = int.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
			int g = int.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
			int b = int.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);
			return new Color(r, g, b);
		}
	}
}
namespace NDB.Covid19.Droid.OAuth2
{
	[Activity(Label = "AuthUrlSchemeInterceptorActivity", LaunchMode = LaunchMode.SingleTop, NoHistory = true, Name = "md52ecc484fd43c6baf7f3301c3ba1d0d0c.AuthUrlSchemeInterceptorActivity")]
	[IntentFilter(new string[] { "android.intent.action.VIEW" }, Categories = new string[] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" }, DataSchemes = new string[] { "com.netcompany.smittestop" }, DataPath = "/oauth2redirect")]
	public class AuthUrlSchemeInterceptorActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			try
			{
				base.OnCreate(savedInstanceState);
				System.Uri url = new System.Uri(Intent.Data.ToString());
				AuthenticationState.Authenticator.OnPageLoading(url);
				Finish();
			}
			catch (System.Exception e)
			{
				string additionalInfo = ((Intent == null) ? "Intent was null" : ((Intent.Data == null) ? "Intent.Data was null" : ("Intent.Data: " + Intent.Data.ToString())));
				LogUtils.LogException(LogSeverity.WARNING, e, "AuthUrlSchemeInterceptorActivity OnCreate error when redirectin to app after NemID validation", additionalInfo);
				AuthenticationState.Authenticator.OnPageLoading(new System.Uri("com.netcompany.smittestop:/oauth2redirect"));
				Finish();
			}
		}
	}
}
namespace NDB.Covid19.Droid.Views
{
	[Activity(MainLauncher = true, Theme = "@style/AppTheme.Launcher", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class InitializerActivity : Activity
	{
		private RelativeLayout _continueInEnRelativeLayoutButton;

		private TextView _continueInEnTextView;

		private Button _launcherButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			if (!IsTaskRoot)
			{
				Finish();
			}
			base.OnCreate(savedInstanceState);
			if (OnboardingStatusHelper.Status == OnboardingStatus.OnlyMainOnboardingCompleted)
			{
				NavigationHelper.GoToWelcomeWhatsNewPage(this);
				Finish();
				return;
			}
			SetContentView(2131492928);
			_launcherButton = FindViewById<Button>(2131296613);
			_continueInEnRelativeLayoutButton = FindViewById<RelativeLayout>(2131296410);
			_continueInEnTextView = FindViewById<TextView>(2131296411);
			_launcherButton.Text = InitializerViewModel.LAUNCHER_PAGE_START_BTN;
			_continueInEnTextView.Text = InitializerViewModel.LAUNCHER_PAGE_CONTINUE_IN_ENG;
			_launcherButton.Click += new StressUtils.SingleClick(LauncherButton_Click).Run;
			_continueInEnRelativeLayoutButton.Click += new StressUtils.SingleClick(ContinueInEnButton_Click).Run;
			SetLogoBasedOnAppLanguage();
		}

		private void SetLogoBasedOnAppLanguage()
		{
			ImageView imageView = FindViewById<ImageView>(2131296612);
			string language = LocalesService.GetLanguage();
			imageView?.SetImageResource((language != null && language.ToLower() == "en") ? 2131165339 : 2131165338);
			if (imageView?.LayoutParameters != null)
			{
				DisplayMetrics displayMetrics = new DisplayMetrics();
				WindowManager?.DefaultDisplay?.GetMetrics(displayMetrics);
				float density = displayMetrics.Density;
				int width = (int)System.Math.Ceiling((float)((language != null && language.ToLower() == "en") ? 120 : 180) * density);
				imageView.LayoutParameters.Width = width;
			}
			imageView?.RequestLayout();
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (PlayServicesVersionUtils.PlayServicesVersionNumberIsLargeEnough(PackageManager))
			{
				NavigationHelper.GoToStartPageIfIsOnboarded(this);
			}
			else
			{
				ShowOutdatedGPSDialog();
			}
		}

		private void LauncherButton_Click(object sender, EventArgs e)
		{
			LocalPreferencesHelper.SetAppLanguage("da");
			LocalesService.Initialize();
			Continue();
		}

		private void ShowOutdatedGPSDialog()
		{
			DialogUtils.DisplayDialogAsync(this, "BASE_ERROR_TITLE".Translate(), "LAUNCHER_PAGE_GPS_VERSION_DIALOG_MESSAGE_ANDROID".Translate(), "ERROR_OK_BTN".Translate());
		}

		private void Continue()
		{
			if (PlayServicesVersionUtils.PlayServicesVersionNumberIsLargeEnough(PackageManager))
			{
				NavigationHelper.GoToOnBoarding(this, isOnBoarding: true);
			}
			else
			{
				ShowOutdatedGPSDialog();
			}
		}

		private void ContinueInEnButton_Click(object sender, EventArgs e)
		{
			LocalPreferencesHelper.SetAppLanguage("en");
			LocalesService.Initialize();
			Continue();
		}
	}
	public class BaseAppCompatActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			if (State(savedInstanceState) == AppState.IsDestroyed)
			{
				((Activity)(object)this).OnCreate((Bundle)null);
				Intent startingNewIntent = GetStartingNewIntent();
				if (startingNewIntent != null)
				{
					startingNewIntent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
					((Context)(object)this).StartActivity(startingNewIntent);
				}
				((Activity)(object)this).Finish();
			}
			else
			{
				((Activity)(object)this).OnCreate(savedInstanceState);
			}
		}

		protected virtual Intent GetStartingNewIntent()
		{
			return null;
		}

		public AppState State(Bundle savedInstanceState)
		{
			if (savedInstanceState != null && savedInstanceState.GetInt("SavedInstance") > 0)
			{
				return AppState.IsDestroyed;
			}
			return AppState.IsAlive;
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			((Activity)(object)this).OnSaveInstanceState(outState);
			outState.PutInt("SavedInstance", 1);
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class ForceUpdateActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(2131492924);
			((Activity)(object)this).FindViewById<TextView>(2131296530).TextFormatted = HtmlCompat.FromHtml(ForceUpdateViewModel.FORCE_UPDATE_MESSAGE, 0);
			Button button = ((Activity)(object)this).FindViewById<Button>(2131296529);
			button.Text = ForceUpdateViewModel.FORCE_UPDATE_BUTTON_GOOGLE_ANDROID;
			button.ContentDescription = ForceUpdateViewModel.FORCE_UPDATE_BUTTON_GOOGLE_ANDROID;
			button.Click += new StressUtils.SingleClick(GoToGooglePlay).Run;
		}

		private void GoToGooglePlay(object o, EventArgs eventArgs)
		{
			Intent intent = new Intent("android.intent.action.VIEW");
			intent.SetData(Android.Net.Uri.Parse(Conf.GooglePlayAppLink));
			((Context)(object)this).StartActivity(intent);
		}

		public override void OnBackPressed()
		{
		}
	}
}
namespace NDB.Covid19.Droid.Views.Welcome
{
	public class NonSwipeableViewPager : ViewPager
	{
		private bool IsEnabled;

		public NonSwipeableViewPager(IntPtr handle, JniHandleOwnership transfer)
			: base(handle, transfer)
		{
		}

		public NonSwipeableViewPager(Context context, IAttributeSet attrs)
			: base(context, attrs)
		{
			IsEnabled = true;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (IsEnabled)
			{
				return base.OnTouchEvent(e);
			}
			return false;
		}

		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			if (IsEnabled)
			{
				return base.OnInterceptTouchEvent(ev);
			}
			return false;
		}

		public void SetPagingEnabled(bool enabled)
		{
			IsEnabled = enabled;
		}
	}
	[Activity(Label = "", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class WelcomeActivity : BaseAppCompatActivity, ViewPager.IOnPageChangeListener, IJavaObject, IDisposable, IJavaPeerable
	{
		private readonly WelcomePageFourFragment _welcomePageFour = new WelcomePageFourFragment();

		private readonly WelcomePageOneFragment _welcomePageOne = new WelcomePageOneFragment();

		private readonly WelcomePageThreeFragment _welcomePageThree = new WelcomePageThreeFragment();

		private readonly WelcomePageTwoFragment _welcomePageTwo = new WelcomePageTwoFragment();

		private Button _button;

		private TabLayout _dotLayout;

		private int _numPages;

		private NonSwipeableViewPager _pager;

		private List<Fragment> _pages;

		private Button _previousButton;

		public bool IsOnBoarding;

		public void OnPageScrollStateChanged(int state)
		{
			Console.WriteLine("OnPageScrollStateChanged  " + state);
		}

		public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{
			Console.WriteLine("OnPageScrolled  " + position);
		}

		public void OnPageSelected(int position)
		{
			ScrollToTop();
			_previousButton.Visibility = ((position == 0) ? ViewStates.Invisible : ViewStates.Visible);
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			if (State(savedInstanceState) != AppState.IsDestroyed)
			{
				IsOnBoarding = ((Activity)(object)this).Intent.GetBooleanExtra("isOnBoarding", defaultValue: false);
				if (IsOnBoarding && OnboardingStatusHelper.Status == OnboardingStatus.OnlyMainOnboardingCompleted)
				{
					GoToConsents();
					return;
				}
				_pages = new List<Fragment>(new Fragment[4] { _welcomePageOne, _welcomePageTwo, _welcomePageFour, _welcomePageThree });
				_numPages = _pages.Count;
				((Activity)(object)this).SetContentView(2131493012);
				_button = ((Activity)(object)this).FindViewById<Button>(2131296360);
				_previousButton = ((Activity)(object)this).FindViewById<Button>(2131296363);
				_previousButton.Text = WelcomeViewModel.PREVIOUS_PAGE_BUTTON_TEXT;
				_button.Text = WelcomeViewModel.NEXT_PAGE_BUTTON_TEXT;
				_button.Click += new StressUtils.SingleClick(GetNextButton_Click, 500).Run;
				_previousButton.Click += new StressUtils.SingleClick(GetPreviousButton_Click, 500).Run;
				_previousButton.Visibility = ViewStates.Invisible;
				WelcomePagerAdapter adapter = new WelcomePagerAdapter(SupportFragmentManager, _pages);
				_pager = ((Activity)(object)this).FindViewById<NonSwipeableViewPager>(2131296533);
				_pager.Adapter = adapter;
				_pager.SetPagingEnabled(enabled: false);
				_pager.AddOnPageChangeListener(this);
				_pager.AnnounceForAccessibility(IsOnBoarding ? WelcomeViewModel.ANNOUNCEMENT_PAGE_CHANGED_TO_ONE : WelcomeViewModel.ANNOUNCEMENT_PAGE_CHANGED_TO_ONE);
				_dotLayout = ((Activity)(object)this).FindViewById<TabLayout>(2131296846);
				_dotLayout.SetupWithViewPager(_pager, autoRefresh: true);
			}
		}

		protected override Intent GetStartingNewIntent()
		{
			return NavigationHelper.GetStartPageIntent((Activity)(object)this);
		}

		private void GetNextButton_Click(object sender, EventArgs e)
		{
			ScrollToTop();
			if (_numPages == _pager.CurrentItem + 1)
			{
				if (IsOnBoarding)
				{
					GoToConsents();
				}
				else
				{
					((Activity)(object)this).RunOnUiThread((System.Action)((Activity)(object)this).Finish);
				}
			}
			else
			{
				_pager.SetCurrentItem(_pager.CurrentItem + 1, smoothScroll: true);
				AnnouncePageChangesForScreenReaders();
			}
		}

		private void GoToConsents()
		{
			Intent intent = new Intent((Context)(object)this, typeof(WelcomePageConsentsActivity));
			((Context)(object)this).StartActivity(intent);
		}

		private void AnnouncePageChangesForScreenReaders()
		{
			_pager.PerformAccessibilityAction(Android.Views.Accessibility.Action.AccessibilityFocus, null);
			Fragment fragment = _pages[_pager.CurrentItem];
			if (fragment == _welcomePageOne)
			{
				_pager.AnnounceForAccessibility(WelcomeViewModel.ANNOUNCEMENT_PAGE_CHANGED_TO_ONE);
			}
			else if (fragment == _welcomePageTwo)
			{
				_pager.AnnounceForAccessibility(WelcomeViewModel.ANNOUNCEMENT_PAGE_CHANGED_TO_TWO);
			}
			else if (fragment == _welcomePageThree)
			{
				_pager.AnnounceForAccessibility(WelcomeViewModel.ANNOUNCEMENT_PAGE_CHANGED_TO_THREE);
			}
			else if (fragment == _welcomePageFour)
			{
				_pager.AnnounceForAccessibility(WelcomeViewModel.ANNOUNCEMENT_PAGE_CHANGED_TO_FOUR);
			}
		}

		private void GetPreviousButton_Click(object sender, EventArgs e)
		{
			ScrollToTop();
			_pager.SetCurrentItem(_pager.CurrentItem - 1, smoothScroll: true);
			_button.Visibility = ViewStates.Visible;
			AnnouncePageChangesForScreenReaders();
		}

		private void ScrollToTop()
		{
			(_pager.Adapter as WelcomePagerAdapter)?.GetItem(_pager.CurrentItem)?.View.ScrollTo(0, 0);
		}
	}
	public class WelcomePageFourFragment : Fragment
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(2131493015, container, attachToRoot: false);
			TextView textView = view.FindViewById<TextView>(2131296914);
			TextView textView2 = view.FindViewById<TextView>(2131296916);
			TextView textView3 = view.FindViewById<TextView>(2131296915);
			TextView textView4 = view.FindViewById<TextView>(2131296920);
			textView.Text = WelcomeViewModel.WELCOME_PAGE_FOUR_BODY_ONE;
			textView2.Text = WelcomeViewModel.WELCOME_PAGE_FOUR_BODY_TWO;
			textView3.Text = WelcomeViewModel.WELCOME_PAGE_FOUR_BODY_THREE;
			textView4.Text = WelcomeViewModel.WELCOME_PAGE_FOUR_TITLE;
			WelcomePageTools.SetArrowVisibility(view);
			return view;
		}
	}
	public class WelcomePageOneFragment : Fragment
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(2131493016, container, attachToRoot: false);
			TextView textView = view.FindViewById<TextView>(2131296922);
			TextView textView2 = view.FindViewById<TextView>(2131296923);
			TextView textView3 = view.FindViewById<TextView>(2131296926);
			textView.Text = WelcomeViewModel.WELCOME_PAGE_ONE_BODY_ONE;
			textView2.Text = WelcomeViewModel.WELCOME_PAGE_ONE_BODY_TWO;
			textView3.Text = WelcomeViewModel.WELCOME_PAGE_ONE_TITLE;
			WelcomePageTools.SetArrowVisibility(view);
			return view;
		}
	}
	public class WelcomePagerAdapter : FragmentPagerAdapter
	{
		private readonly List<Fragment> pages;

		public override int Count => pages.Count;

		public WelcomePagerAdapter(FragmentManager fm, List<Fragment> pages)
			: base(fm)
		{
			this.pages = pages;
		}

		public override Fragment GetItem(int position)
		{
			return pages[position];
		}
	}
	public class WelcomePageThreeFragment : Fragment
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(2131493017, container, attachToRoot: false);
			TextView textView = view.FindViewById<TextView>(2131296928);
			TextView textView2 = view.FindViewById<TextView>(2131296929);
			TextView textView3 = view.FindViewById<TextView>(2131296931);
			TextView textView4 = view.FindViewById<TextView>(2131296930);
			textView.Text = WelcomeViewModel.WELCOME_PAGE_THREE_BODY_ONE;
			textView.ContentDescription = WelcomeViewModel.WELCOME_PAGE_THREE_BODY_ONE_ACCESSIBILITY;
			textView2.Text = WelcomeViewModel.WELCOME_PAGE_THREE_BODY_TWO;
			textView3.Text = WelcomeViewModel.WELCOME_PAGE_THREE_TITLE;
			textView4.Text = WelcomeViewModel.WELCOME_PAGE_THREE_INFOBOX_BODY;
			textView4.ContentDescription = WelcomeViewModel.WELCOME_PAGE_THREE_INFOBOX_BODY;
			WelcomePageTools.SetArrowVisibility(view);
			return view;
		}
	}
	public class WelcomePageTwoFragment : Fragment
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(2131493018, container, attachToRoot: false);
			TextView textView = view.FindViewById<TextView>(2131296932);
			TextView textView2 = view.FindViewById<TextView>(2131296933);
			TextView textView3 = view.FindViewById<TextView>(2131296936);
			textView.Text = WelcomeViewModel.WELCOME_PAGE_TWO_BODY_ONE;
			textView2.Text = WelcomeViewModel.WELCOME_PAGE_TWO_BODY_TWO;
			textView3.Text = WelcomeViewModel.WELCOME_PAGE_TWO_TITLE;
			WelcomePageTools.SetArrowVisibility(view);
			return view;
		}
	}
	[Activity(Label = "", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class WelcomePageConsentsActivity : AppCompatActivity
	{
		private LinearLayout _consentWarning;

		private TextView _consentWarningTextView;

		private SwitchCompat _switchCustom;

		public event EventHandler<bool> ButtonPressed;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(2131493014);
			((Activity)(object)this).FindViewById<LinearLayout>(2131296390).FindViewById<TextView>(2131296912).Text = NDB.Covid19.ViewModels.ConsentViewModel.WELCOME_PAGE_CONSENT_TITLE;
			Button button = ((Activity)(object)this).FindViewById<Button>(2131296909);
			button.Click += new StressUtils.SingleClick(PreviousButtonPressed, 500).Run;
			button.Text = WelcomeViewModel.PREVIOUS_PAGE_BUTTON_TEXT;
			Button button2 = ((Activity)(object)this).FindViewById<Button>(2131296906);
			button2.Click += new StressUtils.SingleClick(NextButtonPressed, 500).Run;
			button2.Text = WelcomeViewModel.NEXT_PAGE_BUTTON_TEXT;
			_switchCustom = ((Activity)(object)this).FindViewById<SwitchCompat>(2131296910);
			_switchCustom.CheckedChange += OnCheckedChange;
			_switchCustom.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.SWITCH_ACCESSIBILITY_CONSENT_SWITCH_DESCRIPTOR;
			_consentWarning = ((Activity)(object)this).FindViewById<LinearLayout>(2131296907);
			SetConsentWarningShown(shown: false);
			_consentWarningTextView = ((Activity)(object)this).FindViewById<TextView>(2131296908);
			_consentWarningTextView.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_REQUIRED;
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296911);
			textView.Text = NDB.Covid19.ViewModels.ConsentViewModel.GIVE_CONSENT_TEXT;
			textView.LabelFor = _switchCustom.Id;
			RelativeLayout relativeLayout = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296398);
			RelativeLayout relativeLayout2 = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296405);
			RelativeLayout relativeLayout3 = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296399);
			RelativeLayout relativeLayout4 = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296400);
			RelativeLayout relativeLayout5 = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296404);
			RelativeLayout relativeLayout6 = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296401);
			RelativeLayout relativeLayout7 = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296402);
			RelativeLayout relativeLayout8 = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296397);
			RelativeLayout relativeLayout9 = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296396);
			TextView textView2 = relativeLayout.FindViewById<TextView>(2131296395);
			TextView textView3 = relativeLayout2.FindViewById<TextView>(2131296395);
			TextView textView4 = relativeLayout3.FindViewById<TextView>(2131296395);
			TextView textView5 = relativeLayout4.FindViewById<TextView>(2131296395);
			TextView textView6 = relativeLayout5.FindViewById<TextView>(2131296395);
			TextView textView7 = relativeLayout6.FindViewById<TextView>(2131296395);
			TextView textView8 = relativeLayout7.FindViewById<TextView>(2131296395);
			TextView textView9 = relativeLayout8.FindViewById<TextView>(2131296395);
			TextView textView10 = relativeLayout9.FindViewById<TextView>(2131296395);
			textView2.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_ONE_TITLE;
			textView3.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_TWO_TITLE;
			textView4.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_THREE_TITLE;
			textView5.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FOUR_TITLE;
			textView6.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FIVE_TITLE;
			textView7.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SIX_TITLE;
			textView8.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SEVEN_TITLE;
			textView9.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_EIGHT_TITLE;
			textView10.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_NINE_TITLE;
			textView2.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_ONE_TITLE.ToLower();
			textView3.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_TWO_TITLE.ToLower();
			textView4.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_THREE_TITLE.ToLower();
			textView5.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FOUR_TITLE.ToLower();
			textView6.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FIVE_TITLE.ToLower();
			textView7.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SIX_TITLE.ToLower();
			textView8.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SEVEN_TITLE.ToLower();
			textView9.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_EIGHT_TITLE.ToLower();
			textView10.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_NINE_TITLE.ToLower();
			relativeLayout.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_ONE_PARAGRAPH, 0);
			relativeLayout2.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_TWO_PARAGRAPH, 0);
			relativeLayout3.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_THREE_PARAGRAPH, 0);
			relativeLayout4.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FOUR_PARAGRAPH, 0);
			relativeLayout5.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FIVE_PARAGRAPH, 0);
			relativeLayout6.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SIX_PARAGRAPH, 0);
			relativeLayout7.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SEVEN_PARAGRAPH, 0);
			relativeLayout8.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_EIGHT_PARAGRAPH, 0);
			relativeLayout9.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_NINE_PARAGRAPH, 0);
			Button button3 = ((Activity)(object)this).FindViewById<Button>(2131296403);
			button3.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SEVEN_BUTTON_TEXT;
			button3.Click += PolicyLinkBtn_Click;
		}

		private void PolicyLinkBtn_Click(object sender, EventArgs e)
		{
			NDB.Covid19.ViewModels.ConsentViewModel.OpenPrivacyPolicyLink();
		}

		protected override void OnResume()
		{
			((Activity)(object)this).OnResume();
			UpdatePadding();
		}

		private void UpdatePadding()
		{
			ConstraintLayout checkboxLayout = ((Activity)(object)this).FindViewById<ConstraintLayout>(2131296375);
			LinearLayout consentInfoLayout = ((Activity)(object)this).FindViewById<LinearLayout>(2131296392);
			consentInfoLayout.Post(delegate
			{
				consentInfoLayout.SetPadding(consentInfoLayout.PaddingLeft, consentInfoLayout.PaddingTop, consentInfoLayout.PaddingRight, checkboxLayout.Height);
			});
		}

		public bool IsChecked()
		{
			if (_switchCustom != null)
			{
				return _switchCustom.Checked;
			}
			return false;
		}

		private void OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			this.ButtonPressed?.Invoke(this, e.IsChecked);
			_switchCustom.AnnounceForAccessibility(_switchCustom.Checked ? NDB.Covid19.ViewModels.ConsentViewModel.SWITCH_ACCESSIBILITY_ANNOUNCEMENT_CONSENT_GIVEN : NDB.Covid19.ViewModels.ConsentViewModel.SWITCH_ACCESSIBILITY_ANNOUNCEMENT_CONSENT_NOT_GIVEN);
			if (_switchCustom.Checked)
			{
				SetConsentWarningShown(shown: false);
			}
		}

		private void SetConsentWarningShown(bool shown)
		{
			_consentWarning.Visibility = ((!shown) ? ViewStates.Gone : ViewStates.Visible);
			if (shown)
			{
				_consentWarningTextView.SendAccessibilityEvent(EventTypes.ViewAccessibilityFocused);
			}
			UpdatePadding();
		}

		private void PreviousButtonPressed(object sender, EventArgs eventArgs)
		{
			if (OnboardingStatusHelper.Status == OnboardingStatus.OnlyMainOnboardingCompleted)
			{
				((Activity)(object)this).OnBackPressed();
			}
			else
			{
				((Activity)(object)this).Finish();
			}
		}

		private void NextButtonPressed(object sender, EventArgs eventArgs)
		{
			if (IsChecked())
			{
				OnboardingStatusHelper.Status = OnboardingStatus.CountriesOnboardingCompleted;
				NavigationHelper.GoToResultPageAndClearTop((Activity)(object)this);
			}
			else
			{
				SetConsentWarningShown(shown: true);
			}
		}

		public override void OnBackPressed()
		{
			if (OnboardingStatusHelper.Status == OnboardingStatus.OnlyMainOnboardingCompleted)
			{
				NavigationHelper.GoToWelcomeWhatsNewPage((Activity)(object)this);
			}
			else
			{
				((Activity)(object)this).OnBackPressed();
			}
		}
	}
	[Activity(Label = "WelcomePageWhatIsNewActivity", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class WelcomePageWhatIsNewActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(2131493019);
			((Activity)(object)this).FindViewById<TextView>(2131296938).Text = WelcomePageWhatIsNewViewModel.WELCOME_PAGE_WHATS_NEW_TITLE;
			SetBulletText(2131296356, WelcomePageWhatIsNewViewModel.WELCOME_PAGE_WHATS_NEW_BULLET_ONE);
			SetBulletText(2131296358, WelcomePageWhatIsNewViewModel.WELCOME_PAGE_WHATS_NEW_BULLET_TWO);
			SetBulletText(2131296357, WelcomePageWhatIsNewViewModel.WELCOME_PAGE_WHATS_NEW_BULLET_THREE);
			Button button = ((Activity)(object)this).FindViewById<Button>(2131296693);
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296528);
			button.Text = WelcomePageWhatIsNewViewModel.WELCOME_PAGE_WHATS_NEW_BUTTON;
			textView.Text = WelcomePageWhatIsNewViewModel.WELCOME_PAGE_WHATS_NEW_FOOTER;
			button.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToOnBoarding((Activity)(object)this, isOnBoarding: true);
			}).Run;
		}

		private void SetBulletText(int resourceId, string textContent)
		{
			CheckBox checkBox = ((Activity)(object)this).FindViewById<LinearLayout>(resourceId).FindViewById<CheckBox>(2131296355);
			if (checkBox != null)
			{
				checkBox.TextFormatted = HtmlCompat.FromHtml(textContent, 0);
			}
		}
	}
}
namespace NDB.Covid19.Droid.Views.Settings
{
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class SettingsGeneralActivity : AppCompatActivity
	{
		private class OnCheckedChangeListener : Java.Lang.Object, RadioGroup.IOnCheckedChangeListener, IJavaObject, IDisposable, IJavaPeerable
		{
			private readonly SettingsGeneralActivity _self;

			public OnCheckedChangeListener(SettingsGeneralActivity self)
			{
				_self = self;
			}

			public async void OnCheckedChanged(RadioGroup group, int checkedId)
			{
				switch (checkedId)
				{
				case 2131296783:
					await DialogUtils.DisplayDialogAsync((Activity)(object)_self, SettingsGeneralViewModel.GetChangeLanguageViewModel);
					LocalPreferencesHelper.SetAppLanguage("en");
					break;
				case 2131296780:
					await DialogUtils.DisplayDialogAsync((Activity)(object)_self, SettingsGeneralViewModel.GetChangeLanguageViewModel);
					LocalPreferencesHelper.SetAppLanguage("da");
					break;
				}
				LocalesService.SetInternationalization();
			}
		}

		private readonly SettingsGeneralViewModel _viewModel = new SettingsGeneralViewModel();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(2131492990);
			Init();
			LogUtils.LogMessage(LogSeverity.INFO, "User opened Settings General", null);
		}

		private void Init()
		{
			Button button = ((Activity)(object)this).FindViewById<Button>(2131296337);
			button.ContentDescription = SettingsViewModel.SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON;
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296799);
			TextView textView2 = ((Activity)(object)this).FindViewById<TextView>(2131296784);
			TextView textView3 = ((Activity)(object)this).FindViewById<TextView>(2131296785);
			TextView textView4 = ((Activity)(object)this).FindViewById<TextView>(2131296789);
			TextView textView5 = ((Activity)(object)this).FindViewById<TextView>(2131296788);
			TextView textView6 = ((Activity)(object)this).FindViewById<TextView>(2131296795);
			TextView textView7 = ((Activity)(object)this).FindViewById<TextView>(2131296792);
			TextView textView8 = ((Activity)(object)this).FindViewById<TextView>(2131296786);
			TextView textView9 = ((Activity)(object)this).FindViewById<TextView>(2131296786);
			textView.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_TITLE;
			textView2.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_EXPLANATION_ONE;
			textView3.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_EXPLANATION_TWO;
			textView4.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_MOBILE_DATA_HEADER;
			textView5.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_MOBILE_DATA_DESC;
			textView6.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_CHOOSE_LANGUAGE_HEADER;
			textView7.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_RESTART_REQUIRED_TEXT;
			textView8.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_MORE_INFO_BUTTON_TEXT;
			textView8.ContentDescription = SettingsGeneralViewModel.SETTINGS_GENERAL_ACCESSIBILITY_MORE_INFO_BUTTON_TEXT;
			textView9.Click += new StressUtils.SingleClick(delegate
			{
				SettingsGeneralViewModel.OpenSmitteStopLink();
			}).Run;
			RadioGroup radioGroup = ((Activity)(object)this).FindViewById<RadioGroup>(2131296796);
			RadioButton radioButton = ((Activity)(object)this).FindViewById<RadioButton>(2131296783);
			RadioButton radioButton2 = ((Activity)(object)this).FindViewById<RadioButton>(2131296780);
			radioButton.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_EN;
			radioButton2.Text = SettingsGeneralViewModel.SETTINGS_GENERAL_DA;
			if (LocalesService.GetLanguage() == "en")
			{
				radioButton.Checked = true;
			}
			else
			{
				radioButton2.Checked = true;
			}
			radioGroup.SetOnCheckedChangeListener(new OnCheckedChangeListener(this));
			SwitchCompat switchCompat = ((Activity)(object)this).FindViewById<SwitchCompat>(2131296797);
			switchCompat.Checked = _viewModel.GetStoredCheckedState();
			switchCompat.CheckedChange += OnCheckedChange;
			button.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
		}

		private async void OnCheckedChange(object obj, EventArgs args)
		{
			SwitchCompat switchButton = (SwitchCompat)obj;
			bool flag = !switchButton.Checked;
			if (flag)
			{
				flag = !(await DialogUtils.DisplayDialogAsync((Activity)(object)this, SettingsGeneralViewModel.AreYouSureDialogViewModel));
			}
			if (flag)
			{
				switchButton.Checked = true;
			}
			_viewModel.OnCheckedChange(switchButton.Checked);
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class SettingsWithdrawConsentsActivity : AppCompatActivity
	{
		private ProgressBar _progressBar;

		private Button _resetConsentsButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = NDB.Covid19.ViewModels.ConsentViewModel.WELCOME_PAGE_CONSENT_TITLE;
			((Activity)(object)this).SetContentView(2131492989);
			Init();
			LogUtils.LogMessage(LogSeverity.INFO, "User opened Settings Withdraw Consents", null);
		}

		private void Init()
		{
			ImageButton imageButton = ((Activity)(object)this).FindViewById<ImageButton>(2131296333);
			imageButton.ContentDescription = SettingsViewModel.SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON;
			_resetConsentsButton = ((Activity)(object)this).FindViewById<Button>(2131296364);
			_progressBar = ((Activity)(object)this).FindViewById<ProgressBar>(2131296389);
			((Activity)(object)this).FindViewById<TextView>(2131296912).Text = NDB.Covid19.ViewModels.ConsentViewModel.WELCOME_PAGE_CONSENT_TITLE;
			_resetConsentsButton.Text = NDB.Covid19.ViewModels.ConsentViewModel.WITHDRAW_CONSENT_BUTTON_TEXT;
			imageButton.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
			_resetConsentsButton.Click += new StressUtils.SingleClick(ResetButtonToggled).Run;
		}

		private async void ResetButtonToggled(object sender, EventArgs e)
		{
			ShowSpinner(show: true);
			await DialogUtils.DisplayDialogAsync((Activity)(object)this, new DialogViewModel
			{
				Title = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_REMOVE_TITLE,
				Body = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_REMOVE_MESSAGE,
				OkBtnTxt = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_OK_BUTTON_TEXT,
				CancelbtnTxt = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_NO_BUTTON_TEXT
			}, PerformWithdrawAsync, delegate
			{
				ShowSpinner(show: false);
			});
		}

		private void PerformWithdrawAsync()
		{
			DeviceUtils.StopScanServices();
			DeviceUtils.CleanDataFromDevice();
			NavigationHelper.RestartApp((Activity)(object)this);
			ShowSpinner(show: false);
		}

		private void ShowSpinner(bool show)
		{
			_resetConsentsButton.Enabled = !show;
			_resetConsentsButton.Visibility = (show ? ViewStates.Invisible : ViewStates.Visible);
			_progressBar.Visibility = ((!show) ? ViewStates.Gone : ViewStates.Visible);
		}
	}
	public class ConsentSettingPageFragment : Fragment
	{
		public event EventHandler<bool> ButtonPressed;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(2131492903, container, attachToRoot: false);
			RelativeLayout relativeLayout = view.FindViewById<RelativeLayout>(2131296398);
			RelativeLayout relativeLayout2 = view.FindViewById<RelativeLayout>(2131296405);
			RelativeLayout relativeLayout3 = view.FindViewById<RelativeLayout>(2131296399);
			RelativeLayout relativeLayout4 = view.FindViewById<RelativeLayout>(2131296400);
			RelativeLayout relativeLayout5 = view.FindViewById<RelativeLayout>(2131296404);
			RelativeLayout relativeLayout6 = view.FindViewById<RelativeLayout>(2131296401);
			RelativeLayout relativeLayout7 = view.FindViewById<RelativeLayout>(2131296402);
			RelativeLayout relativeLayout8 = view.FindViewById<RelativeLayout>(2131296397);
			RelativeLayout relativeLayout9 = view.FindViewById<RelativeLayout>(2131296396);
			TextView textView = relativeLayout.FindViewById<TextView>(2131296395);
			TextView textView2 = relativeLayout2.FindViewById<TextView>(2131296395);
			TextView textView3 = relativeLayout3.FindViewById<TextView>(2131296395);
			TextView textView4 = relativeLayout4.FindViewById<TextView>(2131296395);
			TextView textView5 = relativeLayout5.FindViewById<TextView>(2131296395);
			TextView textView6 = relativeLayout6.FindViewById<TextView>(2131296395);
			TextView textView7 = relativeLayout7.FindViewById<TextView>(2131296395);
			TextView textView8 = relativeLayout8.FindViewById<TextView>(2131296395);
			TextView textView9 = relativeLayout9.FindViewById<TextView>(2131296395);
			textView.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_ONE_TITLE;
			textView2.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_TWO_TITLE;
			textView3.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_THREE_TITLE;
			textView4.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FOUR_TITLE;
			textView5.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FIVE_TITLE;
			textView6.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SIX_TITLE;
			textView7.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SEVEN_TITLE;
			textView8.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_EIGHT_TITLE;
			textView9.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_NINE_TITLE;
			textView.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_ONE_TITLE.ToLower();
			textView2.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_TWO_TITLE.ToLower();
			textView3.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_THREE_TITLE.ToLower();
			textView4.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FOUR_TITLE.ToLower();
			textView5.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FIVE_TITLE.ToLower();
			textView6.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SIX_TITLE.ToLower();
			textView7.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SEVEN_TITLE.ToLower();
			textView8.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_EIGHT_TITLE.ToLower();
			textView9.ContentDescription = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_NINE_TITLE.ToLower();
			relativeLayout.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_ONE_PARAGRAPH, 0);
			relativeLayout2.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_TWO_PARAGRAPH, 0);
			relativeLayout3.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_THREE_PARAGRAPH, 0);
			relativeLayout4.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FOUR_PARAGRAPH, 0);
			relativeLayout5.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_FIVE_PARAGRAPH, 0);
			relativeLayout6.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SIX_PARAGRAPH, 0);
			relativeLayout7.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SEVEN_PARAGRAPH, 0);
			relativeLayout8.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_EIGHT_PARAGRAPH, 0);
			relativeLayout9.FindViewById<TextView>(2131296394).TextFormatted = HtmlCompat.FromHtml(NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_NINE_PARAGRAPH, 0);
			Button button = view.FindViewById<Button>(2131296403);
			button.Text = NDB.Covid19.ViewModels.ConsentViewModel.CONSENT_SEVEN_BUTTON_TEXT;
			button.Click += PolicyLinkBtn_Click;
			return view;
		}

		private void PolicyLinkBtn_Click(object sender, EventArgs e)
		{
			NDB.Covid19.ViewModels.ConsentViewModel.OpenPrivacyPolicyLink();
		}

		private void OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			this.ButtonPressed?.Invoke(this, e.IsChecked);
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop, WindowSoftInputMode = SoftInput.AdjustResize)]
	internal class SettingsAbout : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = SettingsPage5ViewModel.SETTINGS_PAGE_5_HEADER;
			((Activity)(object)this).SetContentView(2131492987);
			Init();
			LogUtils.LogMessage(LogSeverity.INFO, "User opened Settings About", null);
		}

		private void Init()
		{
			Button button = ((Activity)(object)this).FindViewById<Button>(2131296336);
			button.ContentDescription = SettingsViewModel.SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON;
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296776);
			TextView textView2 = ((Activity)(object)this).FindViewById<TextView>(2131296774);
			TextView textView3 = ((Activity)(object)this).FindViewById<TextView>(2131296772);
			((Activity)(object)this).FindViewById<TextView>(2131296777).Text = SettingsPage5ViewModel.GetVersionInfo();
			textView.Text = SettingsPage5ViewModel.SETTINGS_PAGE_5_HEADER;
			textView2.Text = SettingsPage5ViewModel.SETTINGS_PAGE_5_CONTENT + " " + SettingsPage5ViewModel.SETTINGS_PAGE_5_LINK;
			button.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
			textView3.Text = SettingsPage5ViewModel.SETTINGS_PAGE_5_LINK;
			LinkUtil.LinkifyTextView(textView3);
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class SettingsActivity : AppCompatActivity
	{
		private static readonly SettingsViewModel _settingsViewModel = new SettingsViewModel();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(2131492995);
			Init();
			LogUtils.LogMessage(LogSeverity.INFO, "User opened Settings", null);
		}

		private void Init()
		{
			ConstraintLayout constraintLayout = ((Activity)(object)this).FindViewById<ConstraintLayout>(2131296810);
			ConstraintLayout constraintLayout2 = ((Activity)(object)this).FindViewById<ConstraintLayout>(2131296813);
			ConstraintLayout constraintLayout3 = ((Activity)(object)this).FindViewById<ConstraintLayout>(2131296778);
			ConstraintLayout constraintLayout4 = ((Activity)(object)this).FindViewById<ConstraintLayout>(2131296805);
			ConstraintLayout constraintLayout5 = ((Activity)(object)this).FindViewById<ConstraintLayout>(2131296694);
			ConstraintLayout constraintLayout6 = ((Activity)(object)this).FindViewById<ConstraintLayout>(2131296862);
			ConstraintLayout constraintLayout7 = ((Activity)(object)this).FindViewById<ConstraintLayout>(2131296535);
			Button button = constraintLayout.FindViewById<Button>(2131296811);
			Button button2 = constraintLayout2.FindViewById<Button>(2131296811);
			Button button3 = constraintLayout3.FindViewById<Button>(2131296811);
			Button button4 = constraintLayout4.FindViewById<Button>(2131296811);
			Button button5 = constraintLayout5.FindViewById<Button>(2131296811);
			Button button6 = constraintLayout7.FindViewById<Button>(2131296811);
			Button button7 = constraintLayout6.FindViewById<Button>(2131296811);
			button.Text = _settingsViewModel.SettingItemList[0].Text;
			button2.Text = _settingsViewModel.SettingItemList[1].Text;
			button3.Text = _settingsViewModel.SettingItemList[2].Text;
			button4.Text = _settingsViewModel.SettingItemList[3].Text;
			button5.Text = _settingsViewModel.SettingItemList[4].Text;
			button6.Text = _settingsViewModel.SettingItemList[5].Text;
			if (_settingsViewModel.ShowDebugItem)
			{
				button7.Text = _settingsViewModel.SettingItemList[6].Text;
				constraintLayout6.Visibility = ViewStates.Visible;
			}
			ViewGroup viewGroup = ((Activity)(object)this).FindViewById<ViewGroup>(2131296550);
			viewGroup.ContentDescription = SettingsViewModel.SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON;
			viewGroup.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
			button.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToOnBoarding((Activity)(object)this, isOnBoarding: false);
			}).Run;
			button2.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToSettingsHowItWorksPage((Activity)(object)this);
			}).Run;
			button4.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToSettingsHelpPage((Activity)(object)this);
			}).Run;
			button5.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToSettingsAboutPage((Activity)(object)this);
			}).Run;
			button3.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToConsentsWithdrawPage((Activity)(object)this);
			}).Run;
			button7.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToDebugPage((Activity)(object)this);
			}).Run;
			button6.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToGenetalSettingsPage((Activity)(object)this);
			}).Run;
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class SettingsHelpActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = SettingsPage4ViewModel.HEADER;
			((Activity)(object)this).SetContentView(2131492991);
			Init();
			LogUtils.LogMessage(LogSeverity.INFO, "User opened Settings Help", null);
		}

		private void Init()
		{
			ImageButton imageButton = ((Activity)(object)this).FindViewById<ImageButton>(2131296338);
			imageButton.ContentDescription = SettingsViewModel.SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON;
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296802);
			TextView textView2 = ((Activity)(object)this).FindViewById<TextView>(2131296804);
			TextView textView3 = ((Activity)(object)this).FindViewById<TextView>(2131296800);
			textView2.Text = SettingsPage4ViewModel.HEADER;
			textView.TextFormatted = HtmlCompat.FromHtml(SettingsPage4ViewModel.CONTENT_TEXT_BEFORE_SUPPORT_LINK + "<br><br>" + SettingsPage4ViewModel.EMAIL_TEXT + "<br><a href=\"mailto:" + SettingsPage4ViewModel.EMAIL + "\">" + SettingsPage4ViewModel.EMAIL + "</a><br>" + SettingsPage4ViewModel.PHONE_NUM_Text + "<br><a href=\"tel:" + SettingsPage4ViewModel.PHONE_NUM + "\">" + SettingsPage4ViewModel.PHONE_NUM + "</a><br><br>" + SettingsPage4ViewModel.SUPPORT_TEXT, 0);
			textView.ContentDescriptionFormatted = HtmlCompat.FromHtml(SettingsPage4ViewModel.CONTENT_TEXT_BEFORE_SUPPORT_LINK + "<br><br>" + SettingsPage4ViewModel.EMAIL_TEXT + "<br><a href=\"mailto:" + SettingsPage4ViewModel.EMAIL + "\">" + SettingsPage4ViewModel.EMAIL + "</a><br>" + SettingsPage4ViewModel.PHONE_NUM_Text + "<br><a href=\"tel:" + SettingsPage4ViewModel.PHONE_NUM + "\">" + SettingsPage4ViewModel.PHONE_NUM_ACCESSIBILITY + "</a><br><br>" + SettingsPage4ViewModel.ACCESSIBILITY_SUPPORT_TEXT, 0);
			textView.MovementMethod = LinkMovementMethod.Instance;
			imageButton.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
			textView3.Text = SettingsPage4ViewModel.SUPPORT_LINK;
			textView3.ContentDescription = SettingsPage4ViewModel.SUPPORT_LINK_SHOWN_TEXT;
			LinkUtil.LinkifyTextView(textView3);
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class SettingsHowItWorksActivity : AppCompatActivity
	{
		private class OnClickListener : Java.Lang.Object, View.IOnClickListener, IJavaObject, IDisposable, IJavaPeerable
		{
			private readonly string _link;

			private readonly SettingsHowItWorksActivity _self;

			public OnClickListener(SettingsHowItWorksActivity self, string link)
			{
				_self = self;
				_link = link;
			}

			public void OnClick(View v)
			{
				Intent intent = new Intent("android.intent.action.VIEW");
				intent.SetData(Android.Net.Uri.Parse(_link));
				((Context)(object)_self).StartActivity(intent);
			}
		}

		private class URLSpanNoUnderline : URLSpan
		{
			public URLSpanNoUnderline(string url)
				: base(url)
			{
			}

			public override void UpdateDrawState(TextPaint ds)
			{
				base.UpdateDrawState(ds);
				ds.UnderlineText = false;
				ds.Color = Color.White;
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = SettingsPage2ViewModel.SETTINGS_PAGE_2_HEADER;
			((Activity)(object)this).SetContentView(2131492993);
			Init();
			LogUtils.LogMessage(LogSeverity.INFO, "User opened Settings How App Works", null);
		}

		private void Init()
		{
			Button button = ((Activity)(object)this).FindViewById<Button>(2131296333);
			button.ContentDescription = SettingsViewModel.SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON;
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296807);
			TextView textView2 = ((Activity)(object)this).FindViewById<TextView>(2131296809);
			TextView textView3 = ((Activity)(object)this).FindViewById<TextView>(2131296806);
			textView2.Text = SettingsPage2ViewModel.SETTINGS_PAGE_2_HEADER;
			textView.TextFormatted = HtmlCompat.FromHtml(SettingsPage2ViewModel.SETTINGS_PAGE_2_CONTENT, 0);
			string source = "<a href=\"" + SettingsPage2ViewModel.SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_LINK + "\">" + SettingsPage2ViewModel.SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_LINK_TEXT + "</a>";
			textView3.TextFormatted = HtmlCompat.FromHtml(source, 0);
			FormatLink(textView3);
			textView3.SetOnClickListener(new OnClickListener(this, SettingsPage2ViewModel.SETTINGS_PAGE_2_CONTENT_TEXT_PARAGRAPH_4_LINK));
			button.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
		}

		private void FormatLink(TextView textView)
		{
			SpannableStringBuilder spannableStringBuilder = new SpannableStringBuilder(textView.TextFormatted);
			Java.Lang.Object[] spans = spannableStringBuilder.GetSpans(0, spannableStringBuilder.Length(), Class.FromType(typeof(URLSpan)));
			for (int i = 0; i < spans.Length; i++)
			{
				URLSpan uRLSpan = (URLSpan)spans[i];
				int spanStart = spannableStringBuilder.GetSpanStart(uRLSpan);
				int spanEnd = spannableStringBuilder.GetSpanEnd(uRLSpan);
				spannableStringBuilder.RemoveSpan(uRLSpan);
				URLSpanNoUnderline what = new URLSpanNoUnderline(uRLSpan.URL);
				spannableStringBuilder.SetSpan(what, spanStart, spanEnd, (SpanTypes)0);
			}
			textView.TextFormatted = spannableStringBuilder;
		}
	}
}
namespace NDB.Covid19.Droid.Views.ENDeveloperTools
{
	[Activity(Label = "ENDeveloperToolsActivity", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class ENDeveloperToolsActivity : AppCompatActivity
	{
		private Button _buttonAllConsentNoRestarts;

		private Button _buttonAllConsents;

		private Button _buttonBack;

		private Button _buttonFakeGateway;

		private Button _buttonFetchExposureConfiguration;

		private Button _buttonLastUsedExposureConfiguration;

		private Button _buttonNoConsents;

		private Button _buttonNoConsentsNoRestart;

		private Button _buttonOnlyV1Consents;

		private Button _buttonOnlyV1ConsentsNoRestart;

		private Button _buttonPrintActualPreferences;

		private Button _buttonPrintLastKeysPulledAndTimestamp;

		private Button _buttonPrintLastSymptomOnsetDate;

		private Button _buttonPrintLastUploadedKeys;

		private Button _buttonPullKeys;

		private Button _buttonPullKeysAndGetExposureInfo;

		private Button _buttonPushKeys;

		private Button _buttonResetLocalData;

		private Button _buttonSendExposureMessage;

		private Button _buttonSendExposureMessageAfter10Sec;

		private Button _buttonSendExposureMessageDecrement;

		private Button _buttonSendExposureMessageIncrement;

		private Button _buttonShowLastExposureInfo;

		private Button _buttonShowLastSummary;

		private Button _buttonShowLatestPullKeysTimesAndStatuses;

		private Button _buttonToggleMessageRetentionLength;

		private Button _pullWithDelay;

		private TextView _textViewDevOutput;

		private ENDeveloperToolsViewModel _viewModel;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(2131492922);
			_viewModel = new ENDeveloperToolsViewModel();
			InitLayout();
		}

		private void InitLayout()
		{
			_buttonBack = ((Activity)(object)this).FindViewById<Button>(2131296481);
			_buttonBack.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
			_buttonPullKeys = ((Activity)(object)this).FindViewById<Button>(2131296486);
			_buttonPullKeys.Click += new StressUtils.SingleClick(delegate
			{
				PullKeys();
			}).Run;
			_buttonPullKeysAndGetExposureInfo = ((Activity)(object)this).FindViewById<Button>(2131296487);
			_buttonPullKeysAndGetExposureInfo.Click += new StressUtils.SingleClick(delegate
			{
				PullKeysAndGetExposureInfo();
			}).Run;
			_buttonPushKeys = ((Activity)(object)this).FindViewById<Button>(2131296488);
			_buttonPushKeys.Click += new StressUtils.SingleClick(delegate
			{
				GetPushKeyInfo();
			}).Run;
			_buttonSendExposureMessage = ((Activity)(object)this).FindViewById<Button>(2131296490);
			_buttonSendExposureMessage.Click += new StressUtils.SingleClick(async delegate
			{
				await SendExposureMessage();
			}).Run;
			_buttonSendExposureMessageIncrement = ((Activity)(object)this).FindViewById<Button>(2131296493);
			_buttonSendExposureMessageIncrement.Click += new StressUtils.SingleClick(delegate
			{
				SendExposureMessageIncrement();
			}).Run;
			_buttonSendExposureMessageDecrement = ((Activity)(object)this).FindViewById<Button>(2131296492);
			_buttonSendExposureMessageDecrement.Click += new StressUtils.SingleClick(delegate
			{
				SendExposureMessageDecrement();
			}).Run;
			_buttonSendExposureMessageAfter10Sec = ((Activity)(object)this).FindViewById<Button>(2131296491);
			_buttonSendExposureMessageAfter10Sec.Click += new StressUtils.SingleClick(async delegate
			{
				await SendExposureMessageAfter10Sec();
			}).Run;
			_buttonFetchExposureConfiguration = ((Activity)(object)this).FindViewById<Button>(2131296482);
			_buttonFetchExposureConfiguration.Click += new StressUtils.SingleClick(delegate
			{
				FetchExposureConfiguration();
			}).Run;
			_buttonLastUsedExposureConfiguration = ((Activity)(object)this).FindViewById<Button>(2131296483);
			_buttonLastUsedExposureConfiguration.Click += new StressUtils.SingleClick(delegate
			{
				LastUsedExposureConfiguration();
			}).Run;
			_buttonResetLocalData = ((Activity)(object)this).FindViewById<Button>(2131296489);
			_buttonResetLocalData.Click += new StressUtils.SingleClick(delegate
			{
				ResetLocalData();
			}).Run;
			_buttonToggleMessageRetentionLength = ((Activity)(object)this).FindViewById<Button>(2131296497);
			_buttonToggleMessageRetentionLength.Click += new StressUtils.SingleClick(delegate
			{
				ToggleRetentionTime();
			}).Run;
			_buttonPrintLastSymptomOnsetDate = ((Activity)(object)this).FindViewById<Button>(2131296485);
			_buttonPrintLastSymptomOnsetDate.Click += new StressUtils.SingleClick(delegate
			{
				PrintLastSymptomsOnsetDate();
			}).Run;
			_buttonPrintLastKeysPulledAndTimestamp = ((Activity)(object)this).FindViewById<Button>(2131296484);
			_buttonPrintLastKeysPulledAndTimestamp.Click += new StressUtils.SingleClick(delegate
			{
				PrintLastPulledKeysAndTimestamp();
			}).Run;
			_buttonShowLastSummary = ((Activity)(object)this).FindViewById<Button>(2131296495);
			_buttonShowLastSummary.Click += new StressUtils.SingleClick(delegate
			{
				PrintLastSummary();
			}).Run;
			_buttonShowLastExposureInfo = ((Activity)(object)this).FindViewById<Button>(2131296494);
			_buttonShowLastExposureInfo.Click += new StressUtils.SingleClick(delegate
			{
				PrintLastExposureInfo();
			}).Run;
			_buttonShowLatestPullKeysTimesAndStatuses = ((Activity)(object)this).FindViewById<Button>(2131296496);
			_buttonShowLatestPullKeysTimesAndStatuses.Click += new StressUtils.SingleClick(delegate
			{
				ShowLatestPullKeysTimesAndStatuses();
			}).Run;
			_textViewDevOutput = ((Activity)(object)this).FindViewById<TextView>(2131296501);
			_buttonNoConsents = ((Activity)(object)this).FindViewById<Button>(2131296677);
			_buttonOnlyV1Consents = ((Activity)(object)this).FindViewById<Button>(2131296697);
			_buttonAllConsents = ((Activity)(object)this).FindViewById<Button>(2131296327);
			_buttonNoConsents.Click += new StressUtils.SingleClick(delegate
			{
				ChangeConsentsAndRestart(OnboardingStatus.NoConsentsGiven);
			}).Run;
			_buttonOnlyV1Consents.Click += new StressUtils.SingleClick(delegate
			{
				ChangeConsentsAndRestart(OnboardingStatus.OnlyMainOnboardingCompleted);
			}).Run;
			_buttonAllConsents.Click += new StressUtils.SingleClick(delegate
			{
				ChangeConsentsAndRestart(OnboardingStatus.CountriesOnboardingCompleted);
			}).Run;
			_buttonPrintActualPreferences = ((Activity)(object)this).FindViewById<Button>(2131296710);
			_buttonPrintActualPreferences.Click += OnPrintActualPreferences().Run;
			_buttonFakeGateway = ((Activity)(object)this).FindViewById<Button>(2131296517);
			_buttonFakeGateway.Click += OnFakeGateway().Run;
			_pullWithDelay = ((Activity)(object)this).FindViewById<Button>(2131296720);
			_pullWithDelay.Click += OnPullWithDelay().Run;
			_buttonNoConsentsNoRestart = ((Activity)(object)this).FindViewById<Button>(2131296678);
			_buttonOnlyV1ConsentsNoRestart = ((Activity)(object)this).FindViewById<Button>(2131296698);
			_buttonAllConsentNoRestarts = ((Activity)(object)this).FindViewById<Button>(2131296328);
			_buttonNoConsentsNoRestart.Click += new StressUtils.SingleClick(delegate
			{
				OnboardingStatusHelper.Status = OnboardingStatus.NoConsentsGiven;
			}).Run;
			_buttonOnlyV1ConsentsNoRestart.Click += new StressUtils.SingleClick(delegate
			{
				OnboardingStatusHelper.Status = OnboardingStatus.OnlyMainOnboardingCompleted;
			}).Run;
			_buttonAllConsentNoRestarts.Click += new StressUtils.SingleClick(delegate
			{
				OnboardingStatusHelper.Status = OnboardingStatus.CountriesOnboardingCompleted;
			}).Run;
		}

		private StressUtils.SingleClick OnPullWithDelay()
		{
			return new StressUtils.SingleClick(delegate
			{
				_viewModel.PullWithDelay(_viewModel.PullKeysFromServer);
			});
		}

		private StressUtils.SingleClick OnFakeGateway()
		{
			return new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).RunOnUiThread((System.Action)delegate
				{
					EditText input = new EditText((Context)(object)this);
					InputFilterLengthFilter inputFilterLengthFilter = new InputFilterLengthFilter(2);
					input.SetFilters(new IInputFilter[1] { inputFilterLengthFilter });
					input.LayoutParameters = new LinearLayout.LayoutParams(-1, -1);
					new AndroidX.AppCompat.App.AlertDialog.Builder((Context)(object)this).SetMessage("Enter region code (2 chars country code)").SetTitle("Fake Gateway").SetView(input)
						.SetPositiveButton("OK", async delegate
						{
							ApiResponse apiResponse = await _viewModel.FakeGateway(input.Text);
							if (apiResponse != null)
							{
								UpdateText("Pushed keys to region: " + input.Text + "\n" + $"isSuccessful: {apiResponse.IsSuccessfull}\n" + $"StatusCode: {apiResponse.StatusCode}\n" + "Error Message: " + apiResponse.ErrorLogMessage + "\n\n");
							}
							else
							{
								UpdateText("Pushing of keys failed with unknown error");
							}
						})
						.SetNegativeButton("Cancel", delegate
						{
						})
						.Show();
				});
			});
		}

		private StressUtils.SingleClick OnPrintActualPreferences()
		{
			return new StressUtils.SingleClick(async delegate
			{
				UpdateText(await _viewModel.GetFormattedPreferences());
			});
		}

		private void ChangeConsentsAndRestart(OnboardingStatus status)
		{
			OnboardingStatusHelper.Status = status;
			NavigationHelper.RestartApp((Activity)(object)this);
		}

		private void SendExposureMessageDecrement()
		{
			UpdateText(_viewModel.DecrementExposureDate());
		}

		private void SendExposureMessageIncrement()
		{
			UpdateText(_viewModel.IncementExposureDate());
		}

		private void UpdateText(string text)
		{
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				_textViewDevOutput.Text = text;
			});
		}

		private void ToggleRetentionTime()
		{
			UpdateText(_viewModel.ToggleMessageRetentionTime());
		}

		private void PrintLastSymptomsOnsetDate()
		{
			UpdateText(_viewModel.PrintLastSymptomOnsetDate());
		}

		private void PrintLastPulledKeysAndTimestamp()
		{
			UpdateText(_viewModel.PrintLastPulledKeysAndTimestamp());
		}

		private async void PullKeys()
		{
			await _viewModel.PullKeysFromServer();
			UpdateText(ENDeveloperToolsViewModel.GetLastPullResult() ?? "");
		}

		private async void PullKeysAndGetExposureInfo()
		{
			await _viewModel.PullKeysFromServerAndGetExposureInfo();
			UpdateText(ENDeveloperToolsViewModel.GetLastPullResult() ?? "");
		}

		private async void GetPushKeyInfo()
		{
			UpdateText("Copied to clipboard: \n" + await _viewModel.GetPushKeyInfoFromSharedPrefs());
		}

		private async System.Threading.Tasks.Task SendExposureMessage()
		{
			UpdateText("Sending Exposure Message");
			try
			{
				await _viewModel.SimulateExposureMessage();
			}
			catch (System.Exception)
			{
				UpdateText("Test method: _viewModel.SimulateExposureMessage() failed on android");
			}
		}

		private async System.Threading.Tasks.Task SendExposureMessageAfter10Sec()
		{
			UpdateText("Sending Exposure Message in 10 sec");
			try
			{
				await _viewModel.SimulateExposureMessageAfter10Sec();
			}
			catch (System.Exception)
			{
				UpdateText("Test method: _viewModel.SimulateExposureMessageAfter10Sec() failed on android");
			}
		}

		private void FetchExposureConfiguration()
		{
			System.Threading.Tasks.Task.Run(async delegate
			{
				string res = await _viewModel.FetchExposureConfigurationAsync();
				((Activity)(object)this).RunOnUiThread((System.Action)delegate
				{
					UpdateText("Copied to clipboard:\n" + res);
				});
			});
		}

		private void LastUsedExposureConfiguration()
		{
			string res = _viewModel.LastUsedExposureConfigurationAsync();
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				UpdateText("Copied to clipboard:\n" + res);
			});
		}

		private void ResetLocalData()
		{
			DeviceUtils.CleanDataFromDevice();
			UpdateText("Device cleaned");
			DialogUtils.DisplayDialogAsync((Activity)(object)this, "Local data partially deleted", "Delete successful. You still need to reinstall the app to delete Exposure Notification history and to avoid bugs where the app doesn't know we have exceeded the 'provide keys' quota for the past 24 hours", "OK");
		}

		private void PrintLastSummary()
		{
			UpdateText(_viewModel.GetLastExposureSummary());
		}

		private void PrintLastExposureInfo()
		{
			UpdateText(_viewModel.GetExposureInfosFromLastPull());
		}

		private void ShowLatestPullKeysTimesAndStatuses()
		{
			UpdateText(_viewModel.GetPullHistory());
		}
	}
}
namespace NDB.Covid19.Droid.Views.Messages
{
	[Activity(Theme = "@style/AppTheme", ParentActivity = typeof(InfectionStatusActivity), ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class MessagesActivity : AppCompatActivity
	{
		private class ItemClickListener : Java.Lang.Object, AdapterView.IOnItemClickListener, IJavaObject, IDisposable, IJavaPeerable
		{
			private readonly MessagesAdapter _adapterMessages;

			public ItemClickListener(MessagesAdapter adapterMessages)
			{
				_adapterMessages = adapterMessages;
			}

			public async void OnItemClick(AdapterView parent, View view, int position, long id)
			{
				await ServiceLocator.Current.GetInstance<IBrowser>().OpenAsync(_adapterMessages[position].MessageLink, BrowserLaunchMode.SystemPreferred);
				_adapterMessages[position].IsRead = true;
				_adapterMessages.NotifyDataSetChanged();
			}
		}

		private MessagesAdapter _adapterMessages;

		private ViewGroup _closeButton;

		private ListView _messagesList;

		private LinearLayout _noItemsLayout;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = MessagesViewModel.MESSAGES_HEADER;
			((Activity)(object)this).SetContentView(2131492932);
			Init();
			LogUtils.LogMessage(LogSeverity.INFO, "User opened Messages", null);
		}

		protected override void OnDestroy()
		{
			MessagesViewModel.UnsubscribeMessages(this);
			System.Threading.Tasks.Task.Run(delegate
			{
				MessagesViewModel.MarkAllMessagesAsRead();
			});
			((Activity)(object)this).OnDestroy();
		}

		protected override async void OnResume()
		{
			((Activity)(object)this).OnResume();
			await MessageUtils.RemoveAllOlderThan(Conf.MAX_MESSAGE_RETENTION_TIME_IN_MINUTES);
			CloseLocalNotification();
			Update();
			RemoveBadgeCounterOnOlderPlatforms();
		}

		private void RemoveBadgeCounterOnOlderPlatforms()
		{
			bool num = Build.VERSION.SdkInt < BuildVersionCodes.O;
			bool flag = ShortcutBadger.IsBadgeCounterSupported(((Context)(object)this).ApplicationContext);
			if (num && flag)
			{
				ShortcutBadger.RemoveCount(((Context)(object)this).ApplicationContext);
			}
		}

		private void CloseLocalNotification()
		{
			NotificationManagerCompat.From(CrossCurrentActivity.Current.Activity).Cancel(0);
		}

		private void Init()
		{
			SetLogoBasedOnAppLanguage();
			MessagesViewModel.SubscribeMessages(this, ClearAndAddNewMessages);
			((Activity)(object)this).FindViewById<TextView>(2131296642).Text = MessagesViewModel.MESSAGES_HEADER;
			((Activity)(object)this).FindViewById<TextView>(2131296633).Text = MessagesViewModel.LastUpdateString;
			((Activity)(object)this).FindViewById<TextView>(2131296681).Text = MessagesViewModel.MESSAGES_NO_ITEMS_TITLE;
			((Activity)(object)this).FindViewById<TextView>(2131296679).Text = MessagesViewModel.MESSAGES_NO_ITEMS_DESCRIPTION;
			_messagesList = ((Activity)(object)this).FindViewById<ListView>(2131296641);
			_noItemsLayout = ((Activity)(object)this).FindViewById<LinearLayout>(2131296680);
			_closeButton = ((Activity)(object)this).FindViewById<ViewGroup>(2131296384);
			_closeButton.Click += new StressUtils.SingleClick(OnCloseBtnClicked).Run;
			_closeButton.ContentDescription = MessagesViewModel.MESSAGES_ACCESSIBILITY_CLOSE_BUTTON;
			_adapterMessages = new MessagesAdapter((Activity)(object)this, new MessageItemViewModel[0]);
			_messagesList.Adapter = _adapterMessages;
			_messagesList.OnItemClickListener = new ItemClickListener(_adapterMessages);
			ShowList(isShown: false);
		}

		private void SetLogoBasedOnAppLanguage()
		{
			View view = ((Activity)(object)this).FindViewById<View>(2131296635);
			string language = LocalesService.GetLanguage();
			view?.SetBackgroundResource((language != null && language.ToLower() == "en") ? 2131165417 : 2131165416);
		}

		private void HandleBeforeActivityClose()
		{
			MessagesViewModel.MarkAllMessagesAsRead();
		}

		public override void OnBackPressed()
		{
			HandleBeforeActivityClose();
			((Activity)(object)this).OnBackPressed();
		}

		private void OnCloseBtnClicked(object arg1, EventArgs arg2)
		{
			HandleBeforeActivityClose();
			((Activity)(object)this).Finish();
		}

		public void ClearAndAddNewMessages(List<MessageItemViewModel> messages)
		{
			_adapterMessages.ClearList();
			ShowList(messages.Count > 0);
			_adapterMessages.AddItems(messages);
			((Activity)(object)this).FindViewById<TextView>(2131296633).Text = MessagesViewModel.LastUpdateString;
		}

		public async void Update()
		{
			ClearAndAddNewMessages(await MessagesViewModel.GetMessages());
		}

		private void ShowList(bool isShown)
		{
			_messagesList.Visibility = ((!isShown) ? ViewStates.Invisible : ViewStates.Visible);
			_noItemsLayout.Visibility = (isShown ? ViewStates.Invisible : ViewStates.Visible);
		}
	}
	internal class MessagesAdapter : BaseAdapter<MessageItemViewModel>
	{
		private readonly Activity _context;

		private readonly List<MessageItemViewModel> _items;

		public override MessageItemViewModel this[int position] => _items[position];

		public override int Count => _items.Count;

		public MessagesAdapter(Activity context, MessageItemViewModel[] items)
		{
			_context = context;
			_items = items.ToList();
		}

		public void AddItems(List<MessageItemViewModel> messages)
		{
			_items.AddRange(messages);
			NotifyDataSetChanged();
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View obj = convertView ?? _context.LayoutInflater.Inflate(2131492931, null);
			obj.FindViewById<TextView>(2131296640).Text = _items[position].Title;
			obj.FindViewById<TextView>(2131296638).Text = _items[position].DayAndMonthString;
			obj.FindViewById<TextView>(2131296639).Text = MessageItemViewModel.MESSAGES_RECOMMENDATIONS;
			obj.FindViewById<View>(2131296480).Visibility = (_items[position].IsRead ? ViewStates.Invisible : ViewStates.Visible);
			obj.SetBackgroundResource(_items[position].IsRead ? 2131165387 : 2131165388);
			return obj;
		}

		public void ClearList()
		{
			_items.Clear();
			NotifyDataSetChanged();
		}
	}
}
namespace NDB.Covid19.Droid.Views.InfectionStatus
{
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class LoadingPageDiseaseRateActivity : AppCompatActivity
	{
		private bool _isRunning;

		private DiseaseRateViewModel _viewModel;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_ACCESSIBILITY_LOADING_PAGE_TITLE;
			((Activity)(object)this).SetContentView(2131492929);
			_viewModel = new DiseaseRateViewModel();
			((Activity)(object)this).FindViewById<ProgressBar>(2131296711).Visibility = ViewStates.Visible;
		}

		protected override void OnResume()
		{
			((Activity)(object)this).OnResume();
			if (!_isRunning)
			{
				LoadDataAndStartDiseaseRateActivity();
				_isRunning = true;
			}
		}

		private async void LoadDataAndStartDiseaseRateActivity()
		{
			try
			{
				if (!(await DiseaseRateViewModel.UpdateSSIDataAsync()) && LocalPreferencesHelper.HasNeverSuccessfullyFetchedSSIData)
				{
					OnError(new NullReferenceException("No SSI data"));
					return;
				}
				LogUtils.LogMessage(LogSeverity.INFO, "Data for the disease rate of the day is loaded");
				OnActivityFinished();
			}
			catch (System.Exception e)
			{
				if (!((Activity)(object)this).IsFinishing)
				{
					OnError(e);
				}
			}
		}

		private void OnActivityFinished()
		{
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				((Context)(object)this).StartActivity(new Intent((Context)(object)this, typeof(DiseaseRateActivity)));
			});
		}

		private void OnError(System.Exception e)
		{
			if (LocalPreferencesHelper.HasNeverSuccessfullyFetchedSSIData)
			{
				((Activity)(object)this).RunOnUiThread((System.Action)delegate
				{
					AuthErrorUtils.GoToTechnicalErrorSSINumbers((Activity)(object)this, LogSeverity.ERROR, e, "Could not load data for disease rate of the day, showing technical error page");
				});
				return;
			}
			LogUtils.LogException(LogSeverity.ERROR, e, "Could not load data for disease rate of the day, showing old data");
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				((Context)(object)this).StartActivity(new Intent((Context)(object)this, typeof(DiseaseRateActivity)));
			});
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class InfectionStatusActivity : AppCompatActivity
	{
		private class OnDrawScrollView : Java.Lang.Object, ViewTreeObserver.IOnDrawListener, IJavaObject, IDisposable, IJavaPeerable
		{
			private readonly InfectionStatusActivity _self;

			public OnDrawScrollView(InfectionStatusActivity self)
			{
				_self = self;
			}

			public void OnDraw()
			{
				_self.CheckAndShowScrollDownText();
			}
		}

		private class OnScrollListener : Java.Lang.Object, ViewTreeObserver.IOnScrollChangedListener, IJavaObject, IDisposable, IJavaPeerable
		{
			private readonly ConstraintLayout _layout;

			private readonly ScrollView _scrollView;

			private readonly Activity _self;

			public OnScrollListener(Activity self, ConstraintLayout layout)
			{
				_self = self;
				_layout = layout;
				_scrollView = self.FindViewById<ScrollView>(2131296591);
			}

			public void OnScrollChanged()
			{
				_self.RunOnUiThread(delegate
				{
					if (!_scrollView.CanScrollVertically(1))
					{
						_layout.Visibility = ViewStates.Gone;
						InfectionStatusViewModel.IsScrollDownShown = false;
					}
				});
			}
		}

		private readonly PermissionUtils _permissionUtils = ServiceLocator.Current.GetInstance<PermissionUtils>();

		private TextView _activityStatusDescription;

		private TextView _activityStatusText;

		private ImageView _buttonBackgroundAnimated;

		private bool _dialogDisplayed;

		private Button _diseaseRateCoverButton;

		private TextView _diseaseRateHeader;

		private TextView _diseaseRateLastUpdated;

		private ImageButton _menuIcon;

		private Button _messageCoverButton;

		private RelativeLayout _messageRelativeLayout;

		private TextView _messageSubHeader;

		private TextView _messeageHeader;

		private ImageView _notificationDot;

		private ImageButton _onOffButton;

		private NumberPicker _picker;

		private Button _registrationCoverButton;

		private TextView _registrationHeader;

		private RelativeLayout _registrationRelativeLayout;

		private TextView _registrationSubheader;

		private ConstraintLayout _scrollDown;

		private TextView _scrollDownHeader;

		private ScrollView _scrollView;

		private InfectionStatusViewModel _viewModel;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = InfectionStatusViewModel.INFECTION_STATUS_PAGE_TITLE;
			((Activity)(object)this).SetContentView(2131492925);
			_viewModel = new InfectionStatusViewModel();
			InitLayout();
			UpdateMessagesStatus();
			MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.KEY_MESSAGE_STATUS_UPDATED, OnMessageStatusChanged);
			MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.KEY_UPDATE_DISEASE_RATE, OnAppDiseaseRateChanged);
		}

		private void OnAppDiseaseRateChanged(object _ = null)
		{
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				_diseaseRateLastUpdated.Text = InfectionStatusViewModel.LastUpdateString;
				_diseaseRateCoverButton.ContentDescription = InfectionStatusViewModel.INFECTION_STATUS_DISEASE_RATE_HEADER_TEXT + " " + InfectionStatusViewModel.LastUpdateAccessibilityString;
			});
		}

		protected override void OnDestroy()
		{
			MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.KEY_MESSAGE_STATUS_UPDATED);
			MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.KEY_UPDATE_DISEASE_RATE);
			((Activity)(object)this).OnDestroy();
		}

		private void OnMessageStatusChanged(object _ = null)
		{
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				_viewModel.UpdateNotificationDot();
			});
		}

		protected override void OnResume()
		{
			((Activity)(object)this).OnResume();
			LocalPreferencesHelper.UpdateCorrelationId(null);
			LogUtils.LogMessage(LogSeverity.INFO, "User opened InfectionStatus", null);
			InfectionStatusViewModel.RequestSSIUpdate();
			_scrollView?.ScrollTo(0, 0);
			_permissionUtils.SubscribePermissionsMessagingCenter(this, delegate
			{
				PreventMultiplePermissionsDialogsForAction(_permissionUtils.HasPermissions);
			});
			ShowPermissionsDialogIfTheyHavChangedWhileInIdle();
			UpdateUI();
			InfectionStatusViewModel viewModel = _viewModel;
			viewModel.NewMessagesIconVisibilityChanged = (EventHandler)System.Delegate.Combine(viewModel.NewMessagesIconVisibilityChanged, new EventHandler(OnNewMessagesIconVisibilityChanged));
			OnMessageStatusChanged();
		}

		private void ShowPermissionsDialogIfTheyHavChangedWhileInIdle()
		{
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				PreventMultiplePermissionsDialogsForAction(_permissionUtils.CheckPermissionsIfChangedWhileIdle);
			});
		}

		private async void PreventMultiplePermissionsDialogsForAction(Func<Task<bool>> action)
		{
			bool flag = !(await _viewModel.IsRunning());
			if (!flag)
			{
				flag = !(await _permissionUtils.HasPermissionsWithoutDialogs());
			}
			if (flag && !_dialogDisplayed)
			{
				_dialogDisplayed = true;
				if (action != null)
				{
					await action();
				}
				_dialogDisplayed = false;
				await BluetoothStateBroadcastReceiver.GetBluetoothState(UpdateUI);
			}
			UpdateUI();
		}

		protected override void OnPause()
		{
			((Activity)(object)this).OnPause();
			_permissionUtils.UnsubscribePErmissionsMessagingCenter(this);
			InfectionStatusViewModel viewModel = _viewModel;
			viewModel.NewMessagesIconVisibilityChanged = (EventHandler)System.Delegate.Remove(viewModel.NewMessagesIconVisibilityChanged, new EventHandler(OnNewMessagesIconVisibilityChanged));
		}

		private async void InitLayout()
		{
			_activityStatusText = ((Activity)(object)this).FindViewById<TextView>(2131296558);
			_activityStatusDescription = ((Activity)(object)this).FindViewById<TextView>(2131296559);
			_diseaseRateHeader = ((Activity)(object)this).FindViewById<TextView>(2131296568);
			_diseaseRateLastUpdated = ((Activity)(object)this).FindViewById<TextView>(2131296580);
			_messeageHeader = ((Activity)(object)this).FindViewById<TextView>(2131296576);
			_messageSubHeader = ((Activity)(object)this).FindViewById<TextView>(2131296581);
			_registrationHeader = ((Activity)(object)this).FindViewById<TextView>(2131296588);
			_registrationSubheader = ((Activity)(object)this).FindViewById<TextView>(2131296587);
			_scrollDownHeader = ((Activity)(object)this).FindViewById<TextView>(2131296570);
			_onOffButton = ((Activity)(object)this).FindViewById<ImageButton>(2131296582);
			_messageRelativeLayout = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296577);
			_registrationRelativeLayout = ((Activity)(object)this).FindViewById<RelativeLayout>(2131296584);
			_menuIcon = ((Activity)(object)this).FindViewById<ImageButton>(2131296572);
			_diseaseRateCoverButton = ((Activity)(object)this).FindViewById<Button>(2131296565);
			_messageCoverButton = ((Activity)(object)this).FindViewById<Button>(2131296578);
			_registrationCoverButton = ((Activity)(object)this).FindViewById<Button>(2131296585);
			_notificationDot = ((Activity)(object)this).FindViewById<ImageView>(2131296575);
			_activityStatusText.Text = InfectionStatusViewModel.INFECTION_STATUS_ACTIVE_TEXT;
			_activityStatusDescription.Text = InfectionStatusViewModel.INFECTION_STATUS_ACTIVITY_STATUS_DESCRIPTION_TEXT;
			_diseaseRateHeader.Text = InfectionStatusViewModel.INFECTION_STATUS_DISEASE_RATE_HEADER_TEXT;
			_diseaseRateLastUpdated.Text = InfectionStatusViewModel.LastUpdateString;
			_messeageHeader.Text = InfectionStatusViewModel.INFECTION_STATUS_MESSAGE_HEADER_TEXT;
			_messageSubHeader.Text = InfectionStatusViewModel.INFECTION_STATUS_MESSAGE_SUBHEADER_TEXT;
			_registrationHeader.Text = InfectionStatusViewModel.INFECTION_STATUS_REGISTRATION_HEADER_TEXT;
			_registrationSubheader.Text = InfectionStatusViewModel.INFECTION_STATUS_REGISTRATION_SUBHEADER_TEXT;
			_scrollDownHeader.Text = InfectionStatusViewModel.SCROLL_DOWN_HEADER_TEXT;
			_menuIcon.ContentDescription = InfectionStatusViewModel.INFECTION_STATUS_MENU_ACCESSIBILITY_TEXT;
			_notificationDot.ContentDescription = InfectionStatusViewModel.INFECTION_STATUS_NEW_MESSAGE_NOTIFICATION_DOT_ACCESSIBILITY_TEXT;
			_diseaseRateCoverButton.ContentDescription = InfectionStatusViewModel.INFECTION_STATUS_DISEASE_RATE_HEADER_TEXT + " " + InfectionStatusViewModel.LastUpdateAccessibilityString;
			_messageCoverButton.ContentDescription = InfectionStatusViewModel.INFECTION_STATUS_MESSAGE_HEADER_TEXT + " " + InfectionStatusViewModel.INFECTION_STATUS_MESSAGE_SUBHEADER_TEXT;
			_registrationCoverButton.ContentDescription = InfectionStatusViewModel.INFECTION_STATUS_REGISTRATION_HEADER_TEXT + " " + InfectionStatusViewModel.INFECTION_STATUS_REGISTRATION_SUBHEADER_TEXT;
			_onOffButton.Click += new StressUtils.SingleClick(StartStopButton_Click, 500).Run;
			_messageRelativeLayout.Click += new StressUtils.SingleClick(MessageLayoutButton_Click, 500).Run;
			_diseaseRateCoverButton.Click += new StressUtils.SingleClick(GoToLoadingPage, 500).Run;
			_messageCoverButton.Click += new StressUtils.SingleClick(MessageLayoutButton_Click, 500).Run;
			_registrationRelativeLayout.Click += new StressUtils.SingleClick(RegistrationLayoutButton_Click, 500).Run;
			_registrationCoverButton.Click += new StressUtils.SingleClick(RegistrationLayoutButton_Click, 500).Run;
			_menuIcon.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToSettingsPage((Activity)(object)this);
			}, 500).Run;
			_buttonBackgroundAnimated = ((Activity)(object)this).FindViewById<ImageView>(2131296561);
			if (!(await _viewModel.IsRunning()))
			{
				_onOffButton.PerformClick();
			}
			_scrollView = ((Activity)(object)this).FindViewById<ScrollView>(2131296591);
			_scrollDown = ((Activity)(object)this).FindViewById<ConstraintLayout>(2131296592);
			_scrollDown.Click += delegate
			{
				_scrollView.FullScroll(FocusSearchDirection.Down);
			};
			_scrollView?.ViewTreeObserver?.AddOnScrollChangedListener(new OnScrollListener((Activity)(object)this, _scrollDown));
			_scrollView?.ViewTreeObserver?.AddOnDrawListener(new OnDrawScrollView(this));
			UpdateUI();
			FlightModeHandlerBroadcastReceiver.OnFlightModeChange = (System.Action)System.Delegate.Combine(FlightModeHandlerBroadcastReceiver.OnFlightModeChange, new System.Action(UpdateUI));
		}

		private void CheckAndShowScrollDownText()
		{
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				bool num = _scrollView.CanScrollVertically(1);
				bool isScrollDownShown = InfectionStatusViewModel.IsScrollDownShown;
				if (num && !isScrollDownShown)
				{
					_scrollDown.Visibility = ViewStates.Visible;
				}
			});
		}

		private void CreatePulseAnimation(ImageView buttonBackgroundAnimated, bool isRunning)
		{
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				if (isRunning)
				{
					Animation animation = AnimationUtils.LoadAnimation((Context)(object)this, 2130771980);
					buttonBackgroundAnimated.Visibility = ViewStates.Visible;
					buttonBackgroundAnimated.StartAnimation(animation);
				}
				else
				{
					buttonBackgroundAnimated.Visibility = ViewStates.Invisible;
					buttonBackgroundAnimated.ClearAnimation();
				}
			});
		}

		private void UpdateUI()
		{
			((Activity)(object)this).RunOnUiThread((System.Action)async delegate
			{
				bool isLocationEnabled = await _permissionUtils.IsLocationEnabled();
				bool isRunning = await _viewModel.IsRunning(isLocationEnabled);
				_diseaseRateLastUpdated.Text = InfectionStatusViewModel.LastUpdateString;
				TextView activityStatusText = _activityStatusText;
				activityStatusText.Text = await _viewModel.StatusTxt(isLocationEnabled);
				activityStatusText = _activityStatusDescription;
				activityStatusText.Text = await _viewModel.StatusTxtDescription(isLocationEnabled);
				_onOffButton.SetBackgroundResource(isRunning ? 2131165381 : 2131165382);
				_onOffButton.SetImageResource(isRunning ? 2131165369 : 2131165371);
				_onOffButton.ContentDescription = (isRunning ? InfectionStatusViewModel.INFECTION_STATUS_STOP_BUTTON_ACCESSIBILITY_TEXT : InfectionStatusViewModel.INFECTION_STATUS_START_BUTTON_ACCESSIBILITY_TEXT);
				CreatePulseAnimation(_buttonBackgroundAnimated, isRunning);
			});
		}

		private void UpdateMessagesStatus()
		{
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				_notificationDot.Visibility = ((!_viewModel.ShowNewMessageIcon) ? ViewStates.Gone : ViewStates.Visible);
				_messageSubHeader.Text = _viewModel.NewMessageSubheaderTxt;
				_messageCoverButton.ContentDescription = InfectionStatusViewModel.INFECTION_STATUS_MESSAGE_HEADER_TEXT + " " + _viewModel.NewMessageSubheaderTxt;
			});
		}

		private void OnNewMessagesIconVisibilityChanged(object sender, EventArgs e)
		{
			UpdateMessagesStatus();
		}

		private async void StartStopButton_Click(object sender, EventArgs e)
		{
			bool isRunning = await _viewModel.IsRunning();
			bool flag = await _permissionUtils.HasPermissionsWithoutDialogs();
			if (isRunning && !flag)
			{
				await _permissionUtils.HasPermissions();
				return;
			}
			if (!isRunning)
			{
				await DialogUtils.DisplayDialogAsync((Activity)(object)this, _viewModel.OnDialogViewModel, StartGoogleAPI);
			}
			else
			{
				await DialogUtils.DisplayDialogAsync((Activity)(object)this, _viewModel.OffDialogViewModel, ShowSpinnerDialog);
			}
			UpdateUI();
		}

		public override void OnWindowFocusChanged(bool hasFocus)
		{
			((Activity)(object)this).OnWindowFocusChanged(hasFocus);
			if (hasFocus)
			{
				ShowPermissionsDialogIfTheyHavChangedWhileInIdle();
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			((Activity)(object)this).OnActivityResult(requestCode, resultCode, data);
			try
			{
				if (resultCode == Result.Ok)
				{
					ExposureNotification.OnActivityResult(requestCode, resultCode, data);
				}
			}
			catch (System.Exception e)
			{
				e.HandleExposureNotificationException("InfectionStatusActivity", "OnActivityResult");
			}
			finally
			{
				_permissionUtils.OnActivityResult(requestCode, resultCode, data);
				UpdateUI();
			}
		}

		private async void StartGoogleAPI()
		{
			_ = 6;
			try
			{
				CloseReminderNotifications();
				await _viewModel.StartENService();
				if (await _viewModel.IsRunning())
				{
					BackgroundFetchScheduler.ScheduleBackgroundFetch();
				}
				bool flag = await _viewModel.IsEnabled();
				if (flag)
				{
					flag = !(await _viewModel.IsRunning());
				}
				bool flag2 = flag;
				if (flag2)
				{
					flag2 = await BluetoothStateBroadcastReceiver.GetBluetoothState(UpdateUI) == BluetoothState.OFF;
				}
				if (flag2)
				{
					await _permissionUtils.HasPermissions();
					await BluetoothStateBroadcastReceiver.GetBluetoothState(UpdateUI);
				}
			}
			finally
			{
				UpdateUI();
			}
		}

		private async void StopGoogleAPI()
		{
			try
			{
				await _viewModel.StopENService();
			}
			finally
			{
				UpdateUI();
			}
		}

		private void MessageLayoutButton_Click(object sender, EventArgs e)
		{
			((Context)(object)this).StartActivity(new Intent((Context)(object)this, typeof(MessagesActivity)));
		}

		private void ShowSpinnerDialog()
		{
			View view = ((Activity)(object)this).LayoutInflater.Inflate(2131492996, null);
			Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder((Context)(object)this);
			builder.SetCancelable(cancelable: false);
			builder.SetTitle(InfectionStatusViewModel.INFECTION_STATUS_SPINNER_DIALOG_TITLE);
			builder.SetMessage(InfectionStatusViewModel.INFECTION_STATUS_SPINNER_DIALOG_MESSAGE);
			_picker = view.FindViewById(2131296708) as NumberPicker;
			_picker.MaxValue = 5;
			_picker.MinValue = 0;
			_picker.DescendantFocusability = DescendantFocusability.BlockDescendants;
			_picker.SetDisplayedValues(new string[6]
			{
				InfectionStatusViewModel.INFECTION_STATUS_SPINNER_DIALOG_OPTION_NO_REMINDER,
				InfectionStatusViewModel.INFECTION_STATUS_SPINNER_DIALOG_OPTION_ONE_HOUR,
				InfectionStatusViewModel.INFECTION_STATUS_SPINNER_DIALOG_OPTION_TWO_HOURS,
				InfectionStatusViewModel.INFECTION_STATUS_SPINNER_DIALOG_OPTION_FOUR_HOURS,
				InfectionStatusViewModel.INFECTION_STATUS_SPINNER_DIALOG_OPTION_EIGHT_HOURS,
				InfectionStatusViewModel.INFECTION_STATUS_SPINNER_DIALOG_OPTION_TWELVE_HOURS
			});
			builder.SetPositiveButton(InfectionStatusViewModel.INFECTION_STATUS_SPINNER_DIALOG_OK_BUTTON, delegate(object sender, DialogClickEventArgs args)
			{
				switch (_picker.Value)
				{
				case 1:
				case 2:
				case 3:
				case 4:
					StartReminderService((int)System.Math.Pow(2.0, _picker.Value - 1));
					break;
				case 5:
					StartReminderService(12L);
					break;
				}
				StopGoogleAPI();
				(sender as Android.App.AlertDialog)?.Dismiss();
			});
			builder.SetView(view);
			builder.Create()?.Show();
		}

		private void CloseReminderNotifications()
		{
			(((Context)(object)this).GetSystemService("notification") as NotificationManager)?.Cancel(10);
			StopReminderService();
		}

		private void StartReminderService(long hourMultiplier)
		{
			Bundle bundle = new Bundle();
			long value = 3600000 * hourMultiplier;
			bundle.PutLong("ticks", value);
			ForegroundServiceHelper.StartForegroundServiceCompat<TimedReminderForegroundService>((Context)(object)this, bundle);
		}

		private void StopReminderService()
		{
			ForegroundServiceHelper.StopForegroundServiceCompat<TimedReminderForegroundService>((Context)(object)this);
		}

		private async void RegistrationLayoutButton_Click(object sender, EventArgs e)
		{
			InfectionStatusViewModel viewModel = _viewModel;
			if (!(await viewModel.IsRunning(await _permissionUtils.IsLocationEnabled())))
			{
				await DialogUtils.DisplayDialogAsync((Activity)(object)this, _viewModel.ReportingIllDialogViewModel);
				return;
			}
			Intent intent = new Intent((Context)(object)this, typeof(InformationAndConsentActivity));
			((Context)(object)this).StartActivity(intent);
		}

		private void GoToLoadingPage(object sender, EventArgs e)
		{
			((Context)(object)this).StartActivity(new Intent((Context)(object)this, typeof(LoadingPageDiseaseRateActivity)));
		}
	}
}
namespace NDB.Covid19.Droid.Views.DiseaseRate
{
	[Activity(Theme = "@style/AppTheme", ParentActivity = typeof(InfectionStatusActivity), ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class DiseaseRateActivity : AppCompatActivity
	{
		private ViewGroup _closeButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = DiseaseRateViewModel.DISEASE_RATE_HEADER;
			((Activity)(object)this).SetContentView(2131492892);
			Init();
		}

		protected override void OnResume()
		{
			((Activity)(object)this).OnResume();
			LogUtils.LogMessage(LogSeverity.INFO, "User opened DiseaseRate", null);
			InfectionStatusViewModel.RequestSSIUpdate(delegate
			{
				((Activity)(object)this).RunOnUiThread((System.Action)UpdateUI);
			});
		}

		private void Init()
		{
			UpdateUI();
			_closeButton = ((Activity)(object)this).FindViewById<ViewGroup>(2131296448);
			_closeButton.Click += new StressUtils.SingleClick(OnCloseBtnClicked).Run;
			_closeButton.ContentDescription = MessagesViewModel.MESSAGES_ACCESSIBILITY_CLOSE_BUTTON;
		}

		private void UpdateUI()
		{
			((Activity)(object)this).FindViewById<TextView>(2131296456).Text = DiseaseRateViewModel.DISEASE_RATE_HEADER;
			((Activity)(object)this).FindViewById<TextView>(2131296466).Text = DiseaseRateViewModel.LastUpdateStringSubHeader;
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296467);
			ISpanned spanned = (ISpanned)(textView.ContentDescriptionFormatted = (textView.TextFormatted = HtmlCompat.FromHtml(DiseaseRateViewModel.LastUpdateStringSubSubHeader, 0)));
			textView.MovementMethod = LinkMovementMethod.Instance;
			textView.SetLinkTextColor(new Color(250, 220, 93));
			((Activity)(object)this).FindViewById<TextView>(2131296457).Text = DiseaseRateViewModel.KEY_FEATURE_ONE_LABEL;
			((Activity)(object)this).FindViewById<TextView>(2131296458).Text = DiseaseRateViewModel.ConfirmedCasesToday;
			((Activity)(object)this).FindViewById<TextView>(2131296459).Text = DiseaseRateViewModel.ConfirmedCasesTotal;
			((Activity)(object)this).FindViewById<TextView>(2131296450).Text = DiseaseRateViewModel.KEY_FEATURE_TWO_LABEL;
			((Activity)(object)this).FindViewById<TextView>(2131296451).Text = DiseaseRateViewModel.DeathsToday;
			((Activity)(object)this).FindViewById<TextView>(2131296452).Text = DiseaseRateViewModel.DeathsTotal;
			((Activity)(object)this).FindViewById<TextView>(2131296468).Text = DiseaseRateViewModel.KEY_FEATURE_THREE_LABEL;
			((Activity)(object)this).FindViewById<TextView>(2131296470).Text = DiseaseRateViewModel.TestsConductedToday;
			((Activity)(object)this).FindViewById<TextView>(2131296471).Text = DiseaseRateViewModel.TestsConductedTotal;
			((Activity)(object)this).FindViewById<TextView>(2131296472).Text = DiseaseRateViewModel.KEY_FEATURE_FOUR_LABEL;
			((Activity)(object)this).FindViewById<TextView>(2131296473).Text = DiseaseRateViewModel.VaccinatedFirst;
			((Activity)(object)this).FindViewById<TextView>(2131296474).Text = DiseaseRateViewModel.KEY_FEATURE_FOUR_FIRST_VACCINATION_LABEL;
			((Activity)(object)this).FindViewById<TextView>(2131296475).Text = DiseaseRateViewModel.VaccinatedSecond;
			((Activity)(object)this).FindViewById<TextView>(2131296476).Text = DiseaseRateViewModel.KEY_FEATURE_FOUR_SECOND_VACCINATION_LABEL;
			((Activity)(object)this).FindViewById<TextView>(2131296453).Text = DiseaseRateViewModel.KEY_FEATURE_SIX_LABEL + " \n";
			((Activity)(object)this).FindViewById<TextView>(2131296455).Text = DiseaseRateViewModel.SmittestopDownloadsTotal;
			((Activity)(object)this).FindViewById<TextView>(2131296461).Text = DiseaseRateViewModel.KEY_FEATURE_FIVE_LABEL;
			((Activity)(object)this).FindViewById<TextView>(2131296463).Text = DiseaseRateViewModel.NumberOfPositiveTestsResultsLast7Days;
			((Activity)(object)this).FindViewById<TextView>(2131296464).Text = DiseaseRateViewModel.NumberOfPositiveTestsResultsTotal;
		}

		private void OnCloseBtnClicked(object arg1, EventArgs arg2)
		{
			GoToInfectionStatusActivity();
		}

		public override void OnBackPressed()
		{
			GoToInfectionStatusActivity();
		}

		private void GoToInfectionStatusActivity()
		{
			NavigationHelper.GoToResultPageAndClearTop((Activity)(object)this);
		}
	}
}
namespace NDB.Covid19.Droid.Views.AuthenticationFlow
{
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class QuestionnaireConfirmLeaveActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(2131492978);
			Init();
		}

		protected override void OnResume()
		{
			((Activity)(object)this).OnResume();
			LogUtils.LogMessage(LogSeverity.INFO, "The user is seeing Questionnaire Confirm Leave", null, LocalPreferencesHelper.GetCorrelationId());
		}

		private void Init()
		{
			TextView obj = ((Activity)(object)this).FindViewById(2131296876) as TextView;
			TextView textView = ((Activity)(object)this).FindViewById(2131296437) as TextView;
			Button button = ((Activity)(object)this).FindViewById(2131296693) as Button;
			Button button2 = ((Activity)(object)this).FindViewById(2131296369) as Button;
			obj.TextFormatted = HtmlCompat.FromHtml(QuestionnaireConfirmLeaveViewModel.QUESTIONNAIRE_CONFIRM_LEAVE_TITLE, 0);
			textView.TextFormatted = HtmlCompat.FromHtml(QuestionnaireConfirmLeaveViewModel.QUESTIONNAIRE_CONFIRM_LEAVE_DESCRIPTION, 0);
			button.Text = QuestionnaireConfirmLeaveViewModel.QUESTIONNAIRE_CONFIRM_LEAVE_BUTTON_OK;
			button2.Text = QuestionnaireConfirmLeaveViewModel.QUESTIONNAIRE_CONFIRM_LEAVE_BUTTON_CANCEL;
			button2.Click += new StressUtils.SingleClick(delegate
			{
				LogUtils.LogMessage(LogSeverity.INFO, "The user confirmed not sharing keys", null, LocalPreferencesHelper.GetCorrelationId());
				GoToInfectionStatusPage();
			}).Run;
			button.Click += new StressUtils.SingleClick(delegate
			{
				QuestionnaireConfirmLeaveViewModel.ValidateData(GoToLoadingPage, OnFail);
			}).Run;
			Button button3 = ((Activity)(object)this).FindViewById<Button>(2131296384);
			button3.ContentDescription = SettingsViewModel.SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON;
			button3.Click += new StressUtils.SingleClick(delegate
			{
				ShowAreYouSureToExitDialog();
			}).Run;
		}

		private void OnFail()
		{
			AuthErrorUtils.GoToTechnicalError((Activity)(object)this, LogSeverity.ERROR, null, "QuestionnaireConfirmLeaveActivity.OnFail: No miba data or token expired");
		}

		private async void ShowAreYouSureToExitDialog()
		{
			if (await DialogUtils.DisplayDialogAsync((Activity)(object)this, ErrorViewModel.REGISTER_LEAVE_HEADER, ErrorViewModel.REGISTER_LEAVE_DESCRIPTION, ErrorViewModel.REGISTER_LEAVE_CONFIRM, ErrorViewModel.REGISTER_LEAVE_CANCEL))
			{
				LogUtils.LogMessage(LogSeverity.INFO, "The user is returning to Infection Status", null, LocalPreferencesHelper.GetCorrelationId());
				GoToInfectionStatusPage();
			}
		}

		private void GoToInfectionStatusPage()
		{
			NavigationHelper.GoToResultPageAndClearTop((Activity)(object)this);
		}

		private void GoToLoadingPage()
		{
			((Context)(object)this).StartActivity(new Intent((Context)(object)this, typeof(LoadingPageActivity)));
		}

		public override void OnBackPressed()
		{
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class QuestionnaireCountriesSelectionActivity : Activity
	{
		private readonly QuestionnaireCountriesViewModel _viewModel = new QuestionnaireCountriesViewModel();

		private Button _closeButton;

		private List<CountryDetailsViewModel> _countries = new List<CountryDetailsViewModel>();

		private TextView _footer;

		private Button _nextButton;

		private ProgressBar _progressBar;

		private RecyclerView _recyclerView;

		private TextView _subtitle;

		private TextView _title;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			base.Title = QuestionnaireCountriesViewModel.COUNTRY_QUESTIONAIRE_HEADER_TEXT;
			InitView();
		}

		protected override void OnResume()
		{
			base.OnResume();
			LogUtils.LogMessage(LogSeverity.INFO, "The user is seeing Questionnaire Countries Selection", null, LocalPreferencesHelper.GetCorrelationId());
		}

		private async void InitView()
		{
			SetContentView(2131492979);
			_title = FindViewById<TextView>(2131296420);
			_subtitle = FindViewById<TextView>(2131296419);
			_footer = FindViewById<TextView>(2131296722);
			_nextButton = FindViewById<Button>(2131296415);
			_recyclerView = FindViewById<RecyclerView>(2131296418);
			_progressBar = FindViewById<ProgressBar>(2131296711);
			_closeButton = FindViewById<Button>(2131296384);
			PrepareRadioGroup();
			_closeButton.ContentDescription = InformationAndConsentViewModel.CLOSE_BUTTON_ACCESSIBILITY_LABEL;
			_closeButton.Click += new StressUtils.SingleClick(OnExitClick).Run;
			_nextButton.Click += new StressUtils.SingleClick(OnNextButtonClick).Run;
			_title.Text = QuestionnaireCountriesViewModel.COUNTRY_QUESTIONAIRE_HEADER_TEXT;
			_subtitle.Text = QuestionnaireCountriesViewModel.COUNTRY_QUESTIONAIRE_INFORMATION_TEXT;
			_footer.Text = QuestionnaireCountriesViewModel.COUNTRY_QUESTIONAIRE_FOOTER;
			_nextButton.Text = QuestionnaireCountriesViewModel.COUNTRY_QUESTIONAIRE_BUTTON_TEXT;
			RunOnUiThread(delegate
			{
				ShowSpinner(show: true);
			});
			List<CountryDetailsViewModel> countries = _countries;
			countries.AddRange((await _viewModel.GetListOfCountriesAsync()) ?? new List<CountryDetailsViewModel>());
			if (!_countries.Any())
			{
				RunOnUiThread(delegate
				{
					ShowSpinner(show: false);
				});
				OnServerError();
				return;
			}
			RunOnUiThread(delegate
			{
				ShowSpinner(show: false);
				QuestionnaireCountriesSelectionAdapter adapter = new QuestionnaireCountriesSelectionAdapter(_countries);
				LinearLayoutManager layoutManager = new LinearLayoutManager(this);
				_recyclerView.SetLayoutManager(layoutManager);
				_recyclerView.SetAdapter(adapter);
			});
		}

		private void PrepareRadioGroup()
		{
			RadioGroup radioGroup = FindViewById<RadioGroup>(2131296727);
			radioGroup.CheckedChange += delegate
			{
				ConstraintLayout constraintLayout = FindViewById<ConstraintLayout>(2131296421);
				ConstraintLayout constraintLayout2 = constraintLayout;
				constraintLayout2.Visibility = radioGroup.CheckedRadioButtonId switch
				{
					2131296731 => ViewStates.Visible, 
					2131296729 => ViewStates.Gone, 
					_ => constraintLayout.Visibility, 
				};
			};
			RadioButton radioButton = FindViewById<RadioButton>(2131296729);
			RadioButton radioButton2 = FindViewById<RadioButton>(2131296731);
			radioButton.Text = QuestionnaireCountriesViewModel.COUNTRY_QUESTIONAIRE_NO_RADIO_BUTTON;
			radioButton2.Text = QuestionnaireCountriesViewModel.COUNTRY_QUESTIONAIRE_YES_RADIO_BUTTON;
			radioButton.Toggle();
		}

		private async void OnExitClick(object sender, EventArgs args)
		{
			if (await DialogUtils.DisplayDialogAsync(this, _viewModel.CloseDialogViewModel))
			{
				LogUtils.LogMessage(LogSeverity.INFO, "The user is returning to Infection Status", null, LocalPreferencesHelper.GetCorrelationId());
				GoToInfectionStatusPage();
			}
		}

		private void GoToInfectionStatusPage()
		{
			NavigationHelper.GoToResultPageAndClearTop(this);
		}

		private void GoToPreSharePage()
		{
			StartActivity(new Intent(this, typeof(QuestionnairePreShareActivity)));
		}

		private void OnNextButtonClick(object sender, EventArgs args)
		{
			List<CountryDetailsViewModel> selectedCountriesList = (FindViewById<RadioButton>(2131296731).Checked ? _countries : new List<CountryDetailsViewModel>());
			_viewModel.InvokeNextButtonClick(GoToPreSharePage, OnFail, selectedCountriesList);
		}

		private void OnServerError()
		{
			LogUtils.LogMessage(LogSeverity.ERROR, "QuestionnaireCountriesSelectionActivity.OnServerError: Skipping language selection because countries failed to be fetched. (Android)");
			_countries = new List<CountryDetailsViewModel>();
			ShowSpinner(show: false);
			OnNextButtonClick(null, null);
		}

		private void OnFail()
		{
			ShowSpinner(show: false);
			AuthErrorUtils.GoToTechnicalError(this, LogSeverity.ERROR, null, "QuestionnaireCountriesSelectionActivity.OnFail: AuthenticationState.personaldata was garbage collected (Android)");
		}

		private void ShowSpinner(bool show)
		{
			if (show)
			{
				_nextButton.Enabled = false;
				_nextButton.Visibility = ViewStates.Invisible;
				_progressBar.Visibility = ViewStates.Visible;
				_recyclerView.Visibility = ViewStates.Invisible;
			}
			else
			{
				_nextButton.Enabled = true;
				_nextButton.Visibility = ViewStates.Visible;
				_progressBar.Visibility = ViewStates.Gone;
				_recyclerView.Visibility = ViewStates.Visible;
			}
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class QuestionnairePreShareActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(2131492981);
			Init();
		}

		protected override void OnResume()
		{
			((Activity)(object)this).OnResume();
			LogUtils.LogMessage(LogSeverity.INFO, "The user is seeing Questionnaire Pre Share", null, LocalPreferencesHelper.GetCorrelationId());
		}

		private void Init()
		{
			TextView obj = ((Activity)(object)this).FindViewById(2131296876) as TextView;
			TextView textView = ((Activity)(object)this).FindViewById(2131296437) as TextView;
			Button button = ((Activity)(object)this).FindViewById(2131296675) as Button;
			obj.TextFormatted = HtmlCompat.FromHtml(QuestionnairePreShareViewModel.QUESTIONNAIRE_PRE_SHARE_TITLE, 0);
			textView.TextFormatted = HtmlCompat.FromHtml(QuestionnairePreShareViewModel.QUESTIONNAIRE_PRE_SHARE_DESCRIPTION, 0);
			button.Text = QuestionnairePreShareViewModel.QUESTIONNAIRE_PRE_SHARE_NEXT_BUTTON;
			button.Click += new StressUtils.SingleClick(delegate
			{
				QuestionnairePreShareViewModel.InvokeNextButtonClick(GoToLoadingPage, OnFail);
			}).Run;
			Button button2 = ((Activity)(object)this).FindViewById<Button>(2131296384);
			button2.ContentDescription = SettingsViewModel.SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON;
			button2.Click += new StressUtils.SingleClick(delegate
			{
				ShowAreYouSureToExitDialog();
			}).Run;
		}

		private void OnFail()
		{
			AuthErrorUtils.GoToTechnicalError((Activity)(object)this, LogSeverity.ERROR, null, "QuestionnairePreShareActivity.OnFail: No miba data or token expired");
		}

		private async void ShowAreYouSureToExitDialog()
		{
			if (await DialogUtils.DisplayDialogAsync((Activity)(object)this, ErrorViewModel.REGISTER_LEAVE_HEADER, ErrorViewModel.REGISTER_LEAVE_DESCRIPTION, ErrorViewModel.REGISTER_LEAVE_CONFIRM, ErrorViewModel.REGISTER_LEAVE_CANCEL))
			{
				LogUtils.LogMessage(LogSeverity.INFO, "The user is returning to Infection Status", null, LocalPreferencesHelper.GetCorrelationId());
				GoToInfectionStatusPage();
			}
		}

		private void GoToInfectionStatusPage()
		{
			NavigationHelper.GoToResultPageAndClearTop((Activity)(object)this);
		}

		private void GoToLoadingPage()
		{
			((Context)(object)this).StartActivity(new Intent((Context)(object)this, typeof(LoadingPageActivity)));
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class InformationAndConsentActivity : AppCompatActivity
	{
		private TextView _bodyOneText;

		private TextView _bodyTwoText;

		private Button _closeButton;

		private TextView _contentText;

		private TextView _contentTwoText;

		private TextView _header;

		private Button _nemIdButton;

		private ProgressBar _progressBar;

		private TextView _subtitleText;

		private InformationAndConsentViewModel _viewModel;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = InformationAndConsentViewModel.INFORMATION_CONSENT_HEADER_TEXT;
			((Activity)(object)this).SetContentView(2131492926);
			CustomTabsConfiguration.CustomTabsClosingMessage = null;
			_viewModel = new InformationAndConsentViewModel(OnAuthSuccess, OnAuthError);
			_viewModel.Init();
			InitLayout();
		}

		private void OnAuthError(object sender, AuthErrorType e)
		{
			GoToErrorPage(e);
		}

		private void OnAuthSuccess(object sender, EventArgs e)
		{
			LogUtils.LogMessage(LogSeverity.INFO, "Successfully authenticated and verified user. Navigation to QuestionnairePageActivity");
			GoToQuestionnairePage();
		}

		private void InitLayout()
		{
			_closeButton = ((Activity)(object)this).FindViewById<Button>(2131296384);
			_nemIdButton = ((Activity)(object)this).FindViewById<Button>(2131296601);
			_header = ((Activity)(object)this).FindViewById<TextView>(2131296600);
			_contentText = ((Activity)(object)this).FindViewById<TextView>(2131296598);
			_subtitleText = ((Activity)(object)this).FindViewById<TextView>(2131296605);
			_bodyOneText = ((Activity)(object)this).FindViewById<TextView>(2131296595);
			_bodyTwoText = ((Activity)(object)this).FindViewById<TextView>(2131296597);
			_contentTwoText = ((Activity)(object)this).FindViewById<TextView>(2131296599);
			_nemIdButton.Text = InformationAndConsentViewModel.INFORMATION_CONSENT_NEMID_BUTTON_TEXT;
			_header.Text = InformationAndConsentViewModel.INFORMATION_CONSENT_HEADER_TEXT;
			_contentText.TextFormatted = HtmlCompat.FromHtml(InformationAndConsentViewModel.INFORMATION_CONSENT_CONTENT_TEXT ?? "", 0);
			_subtitleText.Text = InformationAndConsentViewModel.INFOCONSENT_TITLE;
			_bodyOneText.Text = InformationAndConsentViewModel.INFOCONSENT_BODY_ONE;
			_bodyTwoText.Text = InformationAndConsentViewModel.INFOCONSENT_BODY_TWO;
			_contentTwoText.Text = InformationAndConsentViewModel.INFOCONSENT_DESCRIPTION_ONE;
			_closeButton.ContentDescription = InformationAndConsentViewModel.CLOSE_BUTTON_ACCESSIBILITY_LABEL;
			_closeButton.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}, 500).Run;
			_nemIdButton.Click += new StressUtils.SingleClick(NemIdButton_Click, 500).Run;
			_progressBar = ((Activity)(object)this).FindViewById<ProgressBar>(2131296602);
		}

		private void NemIdButton_Click(object sender, EventArgs e)
		{
			LogUtils.LogMessage(LogSeverity.INFO, "Startet login with nemid");
			Intent uI = AuthenticationState.Authenticator.GetUI((Context)(object)this);
			((Context)(object)this).StartActivity(uI);
		}

		private void GoToErrorPage(AuthErrorType error)
		{
			_viewModel.Cleanup();
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				switch (error)
				{
				case AuthErrorType.MaxTriesExceeded:
					AuthErrorUtils.GoToManyTriesError((Activity)(object)this, LogSeverity.WARNING, null, "Max number of tries was exceeded");
					break;
				case AuthErrorType.NotInfected:
					AuthErrorUtils.GoToNotInfectedError((Activity)(object)this, LogSeverity.WARNING, null, "User is not infected");
					break;
				case AuthErrorType.Unknown:
					AuthErrorUtils.GoToTechnicalError((Activity)(object)this, LogSeverity.WARNING, null, "User sees Technical error page after NemID login: Unknown auth error or user press backbtn");
					break;
				}
			});
		}

		private void GoToQuestionnairePage()
		{
			_viewModel.Cleanup();
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				Intent intent = new Intent((Context)(object)this, typeof(QuestionnairePageActivity));
				((Context)(object)this).StartActivity(intent);
			});
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class LoadingPageActivity : AppCompatActivity
	{
		private static int? refusedCount = 0;

		private bool _isRunning;

		private TextView _loadingText;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_ACCESSIBILITY_LOADING_PAGE_TITLE;
			((Activity)(object)this).SetContentView(2131492929);
			((Activity)(object)this).FindViewById<ProgressBar>(2131296711).Visibility = ViewStates.Visible;
			_loadingText = ((Activity)(object)this).FindViewById<TextView>(2131296627);
			_loadingText.Text = LoadingPageViewModel.LOADING_PAGE_TEXT_NORMAL;
			LogUtils.LogMessage(LogSeverity.INFO, "The user is seeing Loading Page", null, LocalPreferencesHelper.GetCorrelationId());
		}

		protected override void OnResume()
		{
			((Activity)(object)this).OnResume();
			if (!_isRunning)
			{
				LoadingPageViewModel.StartTimer(OnFinished);
				LoadingPageViewModel.ValidateData(StartPushActivity, OnFail);
				_isRunning = true;
			}
		}

		private void OnFinished()
		{
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				_loadingText.Text = LoadingPageViewModel.LOADING_PAGE_TEXT_TIME_EXTENDED;
			});
		}

		private async void StartPushActivity()
		{
			try
			{
				await ExposureNotification.SubmitSelfDiagnosisAsync();
				LogUtils.LogMessage(LogSeverity.INFO, "The user agreed to share keys", null, LocalPreferencesHelper.GetCorrelationId());
				OnActivityFinished();
			}
			catch (System.Exception e)
			{
				e.HandleExposureNotificationException("LoadingPageActivity", "StartPushActivity");
				OnError(e);
			}
		}

		private void OnFail()
		{
			OnError(new System.Exception("Validation Failed"), isOnFail: true);
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			((Activity)(object)this).OnActivityResult(requestCode, resultCode, data);
			try
			{
				ExposureNotification.OnActivityResult(requestCode, resultCode, data);
			}
			catch (System.Exception e)
			{
				e.HandleExposureNotificationException("LoadingPageActivity", "OnActivityResult");
			}
		}

		private void OnActivityFinished()
		{
			refusedCount = 0;
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				((Context)(object)this).StartActivity(new Intent((Context)(object)this, typeof(RegisteredActivity)));
			});
		}

		private void OnError(System.Exception e, bool isOnFail = false)
		{
			if (e is AccessDeniedException)
			{
				if (!refusedCount.HasValue)
				{
					refusedCount = 0;
				}
				LogUtils.LogMessage(LogSeverity.INFO, "The user refused to share keys", refusedCount++.ToString(), LocalPreferencesHelper.GetCorrelationId());
				GoToConfirmLeavePage();
				return;
			}
			if (!isOnFail)
			{
				LogUtils.LogMessage(LogSeverity.INFO, "Something went wrong during key sharing (INFO with correlation id)", e.Message, LocalPreferencesHelper.GetCorrelationId());
			}
			refusedCount = 0;
			((Activity)(object)this).RunOnUiThread((System.Action)delegate
			{
				AuthErrorUtils.GoToTechnicalError((Activity)(object)this, LogSeverity.ERROR, e, "Pushing keys failed");
			});
		}

		private void GoToConfirmLeavePage()
		{
			((Context)(object)this).StartActivity(new Intent((Context)(object)this, typeof(QuestionnaireConfirmLeaveActivity)));
		}

		public override void OnBackPressed()
		{
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class RegisteredActivity : AppCompatActivity
	{
		private Button _closeButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_HEADER;
			((Activity)(object)this).SetContentView(2131492983);
			Init();
		}

		private void Init()
		{
			_closeButton = ((Activity)(object)this).FindViewById<Button>(2131296384);
			_closeButton.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_ACCESSIBILITY_CLOSE_BUTTON_TEXT;
			_closeButton.Click += new StressUtils.SingleClick(delegate
			{
				GoToInfectionStatusActivity();
			}).Run;
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296741);
			TextView textView2 = ((Activity)(object)this).FindViewById<TextView>(2131296740);
			TextView textView3 = ((Activity)(object)this).FindViewById<TextView>(2131296739);
			TextView textView4 = ((Activity)(object)this).FindViewById<TextView>(2131296733);
			TextView textView5 = ((Activity)(object)this).FindViewById<TextView>(2131296735);
			SetLogoBasedOnAppLanguage();
			textView.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_HEADER;
			textView2.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_TEXT;
			textView3.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_DESCRIPTION;
			textView4.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_INNER_HEADER;
			textView5.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_INNER_READ_MORE;
			textView.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_HEADER;
			textView2.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_TEXT;
			textView3.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_DESCRIPTION;
			textView4.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_INNER_HEADER;
			textView5.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_INNER_READ_MORE;
			Button button = ((Activity)(object)this).FindViewById<Button>(2131296737);
			button.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_DISMISS;
			button.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_DISMISS;
			button.Click += new StressUtils.SingleClick(delegate
			{
				GoToInfectionStatusActivity();
			}).Run;
			((Activity)(object)this).FindViewById<RelativeLayout>(2131296736).Click += async delegate
			{
				await ServiceLocator.Current.GetInstance<IBrowser>().OpenAsync(QuestionnaireViewModel.REGISTER_QUESTIONAIRE_RECEIPT_LINK, BrowserLaunchMode.SystemPreferred);
			};
			LogUtils.LogMessage(LogSeverity.INFO, "The user has successfully shared their keys", null, LocalPreferencesHelper.GetCorrelationId());
			LocalPreferencesHelper.UpdateCorrelationId(null);
		}

		private void SetLogoBasedOnAppLanguage()
		{
			ImageView imageView = ((Activity)(object)this).FindViewById<ImageView>(2131296734);
			string language = LocalesService.GetLanguage();
			imageView?.SetBackgroundResource((language != null && language.ToLower() == "en") ? 2131165385 : 2131165384);
		}

		public override void OnBackPressed()
		{
			GoToInfectionStatusActivity();
		}

		private void GoToInfectionStatusActivity()
		{
			NavigationHelper.GoToResultPageAndClearTop((Activity)(object)this);
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class QuestionnairePageActivity : AppCompatActivity
	{
		private class CustomRadioOnClickListener : Java.Lang.Object, View.IOnClickListener, IJavaObject, IDisposable, IJavaPeerable
		{
			private readonly Action<RadioButton> _action;

			private readonly List<RadioButton> _radioButtonList;

			private readonly int _radioButtonToCheck;

			public CustomRadioOnClickListener(List<RadioButton> radioButtons, int radioButtonToCheck, Action<RadioButton> onCheckChange)
			{
				_action = onCheckChange;
				_radioButtonList = radioButtons;
				_radioButtonToCheck = radioButtonToCheck;
			}

			public void OnClick(View v)
			{
				_radioButtonList[_radioButtonToCheck].Checked = true;
				_radioButtonList.Where((RadioButton button, int i) => i != _radioButtonToCheck).ForEach(delegate(RadioButton button)
				{
					button.Checked = false;
				});
				_action?.Invoke(_radioButtonList[_radioButtonToCheck]);
			}
		}

		private Button _closeButton;

		private DatePickerDialog _datePicker;

		private TextView _datePickerTextView;

		private RadioButton _firstRadioButton;

		private RadioButton _fourthRadioButton;

		private ImageButton _infoButton;

		private bool _isChangedFromDatePicker;

		private Button _questionnaireButton;

		private QuestionnaireViewModel _questionnaireViewModel;

		private RadioButton _secondRadioButton;

		private RadioButton _thirdRadioButton;

		private ISpanned GetFormattedText => HtmlCompat.FromHtml(QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YES + " <input type=\"date\">" + _datePickerTextView.ContentDescription + "</input>", 0);

		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).Title = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_HEADER;
			((Activity)(object)this).SetContentView(2131492980);
			Init();
			if (!RemovedFromRecentDetectorService.IsRunning)
			{
				((Context)(object)this).StartService(new Intent((Context)(object)this, typeof(RemovedFromRecentDetectorService)));
			}
		}

		protected override void OnResume()
		{
			((Activity)(object)this).OnResume();
			LogUtils.LogMessage(LogSeverity.INFO, "The user is seeing Questionnaire", null, LocalPreferencesHelper.GetCorrelationId());
		}

		private void Init()
		{
			LocalPreferencesHelper.UpdateCorrelationId(LogUtils.GenerateCorrelationId());
			_questionnaireViewModel = new QuestionnaireViewModel();
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296725);
			textView.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_HEADER;
			textView.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_HEADER;
			TextView textView2 = ((Activity)(object)this).FindViewById<TextView>(2131296724);
			ISpanned spanned = (ISpanned)(textView2.ContentDescriptionFormatted = (textView2.TextFormatted = HtmlCompat.FromHtml(QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_TEXT, 0)));
			_questionnaireButton = ((Activity)(object)this).FindViewById<Button>(2131296721);
			_questionnaireButton.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_NEXT;
			_questionnaireButton.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_NEXT;
			_questionnaireButton.Click += OnNextButtonClick;
			_infoButton = ((Activity)(object)this).FindViewById<ImageButton>(2131296723);
			_infoButton.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_ACCESSIBILITY_DATE_INFO_BUTTON;
			_infoButton.Click += OnInfoButtonPressed;
			_closeButton = ((Activity)(object)this).FindViewById<Button>(2131296384);
			_closeButton.ContentDescription = SettingsViewModel.SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON;
			_closeButton.Click += new StressUtils.SingleClick(delegate
			{
				ShowAreYouSureToExitDialog();
			}).Run;
			PrepareRadioButtons();
		}

		private void PrepareRadioButtons()
		{
			_firstRadioButton = ((Activity)(object)this).FindViewById<RadioButton>(2131296523);
			_secondRadioButton = ((Activity)(object)this).FindViewById<RadioButton>(2131296769);
			_thirdRadioButton = ((Activity)(object)this).FindViewById<RadioButton>(2131296874);
			_fourthRadioButton = ((Activity)(object)this).FindViewById<RadioButton>(2131296532);
			_fourthRadioButton.Checked = true;
			_datePickerTextView = ((Activity)(object)this).FindViewById<TextView>(2131296433);
			_datePickerTextView.Text = QuestionnaireViewModel.DateLabel;
			_datePickerTextView.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_ACCESSIBILITY_RADIO_BUTTON_DATEPICKER_TEXT;
			_firstRadioButton.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YES;
			_secondRadioButton.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YESBUT;
			_thirdRadioButton.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_NO;
			_fourthRadioButton.Text = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_SKIP;
			_firstRadioButton.ContentDescriptionFormatted = GetFormattedText;
			_secondRadioButton.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_YESBUT;
			_thirdRadioButton.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_NO;
			_fourthRadioButton.ContentDescription = QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_ANSWER_SKIP;
			List<RadioButton> radioButtons = new List<RadioButton> { _firstRadioButton, _secondRadioButton, _thirdRadioButton, _fourthRadioButton };
			radioButtons.ForEach(delegate(RadioButton button, int i)
			{
				button.SetOnClickListener(new CustomRadioOnClickListener(radioButtons, i, OnCheckChange));
			});
			_datePickerTextView.SetOnClickListener(new CustomRadioOnClickListener(radioButtons, 0, OnDateEditTextClick));
		}

		private void OnCheckChange(RadioButton radioButton)
		{
			switch (radioButton.Id)
			{
			case 2131296523:
				_questionnaireViewModel.SetSelection(QuestionaireSelection.YesSince);
				if (!_isChangedFromDatePicker)
				{
					ShowDatePickerDialog();
				}
				else if (_isChangedFromDatePicker)
				{
					_isChangedFromDatePicker = false;
				}
				break;
			case 2131296769:
				_questionnaireViewModel.SetSelection(QuestionaireSelection.YesBut);
				if (_isChangedFromDatePicker)
				{
					_isChangedFromDatePicker = false;
				}
				break;
			case 2131296874:
				_questionnaireViewModel.SetSelection(QuestionaireSelection.No);
				if (_isChangedFromDatePicker)
				{
					_isChangedFromDatePicker = false;
				}
				break;
			case 2131296532:
				_questionnaireViewModel.SetSelection(QuestionaireSelection.Skip);
				if (_isChangedFromDatePicker)
				{
					_isChangedFromDatePicker = false;
				}
				break;
			}
		}

		private void OnDateEditTextClick(RadioButton button)
		{
			ShowDatePickerDialog();
			_isChangedFromDatePicker = true;
			OnCheckChange(button);
		}

		private void ShowDatePickerDialog()
		{
			DateTime localSelectedDate = QuestionnaireViewModel.GetLocalSelectedDate();
			int dayOfMonth = (QuestionnaireViewModel.DateHasBeenSet ? localSelectedDate.Day : DateTime.Now.Day);
			int monthOfYear = (QuestionnaireViewModel.DateHasBeenSet ? (localSelectedDate.Month - 1) : (DateTime.Now.Month - 1));
			int year = (QuestionnaireViewModel.DateHasBeenSet ? localSelectedDate.Year : DateTime.Now.Year);
			_datePicker = new DatePickerDialog(CrossCurrentActivity.Current.Activity, delegate(object sender, DatePickerDialog.DateSetEventArgs args)
			{
				_questionnaireViewModel.SetSelectedDateUTC(args.Date.ToUniversalTime());
				_firstRadioButton.Checked = true;
				_firstRadioButton.ContentDescriptionFormatted = GetFormattedText;
				_datePickerTextView.Text = QuestionnaireViewModel.DateLabel;
				_datePickerTextView.Ellipsize = TextUtils.TruncateAt.End;
			}, year, monthOfYear, dayOfMonth);
			_datePicker.DatePicker.MinDate = DateTimeToAndroidDatePickerLong(_questionnaireViewModel.MinimumDate.ToLocalTime());
			_datePicker.DatePicker.MaxDate = DateTimeToAndroidDatePickerLong(DateTime.Now);
			_datePicker.Show();
		}

		private long DateTimeToAndroidDatePickerLong(DateTime dateTime)
		{
			return (long)dateTime.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
		}

		public override void OnBackPressed()
		{
			ShowAreYouSureToExitDialog();
		}

		private async void ShowAreYouSureToExitDialog()
		{
			if (await DialogUtils.DisplayDialogAsync((Activity)(object)this, ErrorViewModel.REGISTER_LEAVE_HEADER, ErrorViewModel.REGISTER_LEAVE_DESCRIPTION, ErrorViewModel.REGISTER_LEAVE_CONFIRM, ErrorViewModel.REGISTER_LEAVE_CANCEL))
			{
				LogUtils.LogMessage(LogSeverity.INFO, "The user is returning to Infection Status", null, LocalPreferencesHelper.GetCorrelationId());
				GoToInfectionStatusPage();
			}
		}

		private void OnNextButtonClick(object o, EventArgs args)
		{
			if (_fourthRadioButton.Checked)
			{
				LogUtils.LogMessage(LogSeverity.INFO, "The user does not want to provide health information", null, LocalPreferencesHelper.GetCorrelationId());
			}
			_questionnaireViewModel.InvokeNextButtonClick(GoToQuestionnaireCountriesSelectionPage, OnFail, OnValidationFail);
		}

		private void OnFail()
		{
			AuthErrorUtils.GoToTechnicalError((Activity)(object)this, LogSeverity.ERROR, null, "QuestionnairePageActivity.OnFail: No miba data or token expired");
		}

		private void OnValidationFail()
		{
			AuthErrorUtils.GoToTechnicalError((Activity)(object)this, LogSeverity.ERROR, null, "QuestionnairePageActivity.OnValidationFail: Data validation failed");
		}

		private void OnInfoButtonPressed(object o, EventArgs args)
		{
			DialogUtils.DisplayBubbleDialog((Activity)(object)this, QuestionnaireViewModel.REGISTER_QUESTIONAIRE_SYMPTOMONSET_HELP, "ERROR_OK_BTN".Translate());
		}

		private void GoToQuestionnaireCountriesSelectionPage()
		{
			((Context)(object)this).StartActivity(new Intent((Context)(object)this, typeof(QuestionnaireCountriesSelectionActivity)));
		}

		private void GoToInfectionStatusPage()
		{
			NavigationHelper.GoToResultPageAndClearTop((Activity)(object)this);
		}
	}
}
namespace NDB.Covid19.Droid.Views.AuthenticationFlow.ErrorActivities
{
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class GeneralErrorActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			((Activity)(object)this).OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(2131492923);
			Init();
		}

		private void Init()
		{
			Bundle extras = ((Activity)(object)this).Intent.Extras;
			string @string = extras.GetString("title");
			string string2 = extras.GetString("description");
			string string3 = extras.GetString("button");
			TextView textView = ((Activity)(object)this).FindViewById<TextView>(2131296510);
			if (extras.ContainsKey("subtitle"))
			{
				string text2 = (textView.ContentDescription = (textView.Text = extras.GetString("subtitle")));
				textView.Visibility = ViewStates.Visible;
			}
			else
			{
				textView.Visibility = ViewStates.Gone;
			}
			TextView textView2 = ((Activity)(object)this).FindViewById<TextView>(2131296511);
			textView2.Text = @string;
			textView2.ContentDescription = @string;
			TextView textView3 = ((Activity)(object)this).FindViewById<TextView>(2131296508);
			ISpanned spanned = (ISpanned)(textView3.ContentDescriptionFormatted = (textView3.TextFormatted = HtmlCompat.FromHtml(string2, 0)));
			textView3.MovementMethod = LinkMovementMethod.Instance;
			ViewGroup viewGroup = ((Activity)(object)this).FindViewById<ViewGroup>(2131296384);
			viewGroup.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToResultPageAndClearTop((Activity)(object)this);
			}).Run;
			viewGroup.ContentDescription = SettingsViewModel.SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON;
			Button button = ((Activity)(object)this).FindViewById<Button>(2131296507);
			button.Text = string3;
			button.ContentDescription = string3;
			button.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToResultPageAndClearTop((Activity)(object)this);
			}).Run;
			((Activity)(object)this).Title = extras.GetString("title");
		}

		public override void OnBackPressed()
		{
			Intent startPageIntent = NavigationHelper.GetStartPageIntent((Activity)(object)this);
			if (startPageIntent != null)
			{
				startPageIntent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
				((Context)(object)this).StartActivity(startPageIntent);
			}
		}
	}
}
namespace NDB.Covid19.Droid.Views.AuthenticationFlow.QuestionnaireAdapters
{
	internal class QuestionnaireCountriesSelectionAdapter : RecyclerView.Adapter
	{
		private class CheckedChangeListener : Java.Lang.Object, View.IOnClickListener, IJavaObject, IDisposable, IJavaPeerable
		{
			private readonly RecyclerView.ViewHolder _holder;

			private readonly int _position;

			private readonly QuestionnaireCountriesSelectionAdapter _self;

			public CheckedChangeListener(QuestionnaireCountriesSelectionAdapter self, RecyclerView.ViewHolder holder, int position)
			{
				_holder = holder;
				_self = self;
				_position = position;
			}

			public void OnClick(View v)
			{
				QuestionnaireCountriesSelectionAdapterViewHolder questionnaireCountriesSelectionAdapterViewHolder = (QuestionnaireCountriesSelectionAdapterViewHolder)_holder;
				questionnaireCountriesSelectionAdapterViewHolder.Check.Checked = !questionnaireCountriesSelectionAdapterViewHolder.Check.Checked;
				_self.Data[_position].Checked = questionnaireCountriesSelectionAdapterViewHolder.Check.Checked;
			}
		}

		private readonly List<int> _selectedItems = new List<int>();

		private List<CountryDetailsViewModel> _countryList;

		private List<CountryDetailsViewModel> Data
		{
			get
			{
				return _countryList;
			}
			set
			{
				_countryList = value;
				NotifyDataSetChanged();
			}
		}

		public override int ItemCount => Data.Count;

		public QuestionnaireCountriesSelectionAdapter(List<CountryDetailsViewModel> countryList)
		{
			_countryList = countryList;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			CountryDetailsViewModel countryDetailsViewModel = Data[position];
			QuestionnaireCountriesSelectionAdapterViewHolder questionnaireCountriesSelectionAdapterViewHolder = holder as QuestionnaireCountriesSelectionAdapterViewHolder;
			if (questionnaireCountriesSelectionAdapterViewHolder != null)
			{
				questionnaireCountriesSelectionAdapterViewHolder.Caption.Text = countryDetailsViewModel.Name;
				questionnaireCountriesSelectionAdapterViewHolder.Check.Checked = _selectedItems.Contains(position);
				questionnaireCountriesSelectionAdapterViewHolder.SetOnClickListener(new CheckedChangeListener(this, holder, position));
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			return new QuestionnaireCountriesSelectionAdapterViewHolder(LayoutInflater.From(parent.Context).Inflate(2131492905, parent, attachToRoot: false) as LinearLayout);
		}
	}
	internal class QuestionnaireCountriesSelectionAdapterViewHolder : RecyclerView.ViewHolder
	{
		private readonly View _itemView;

		public CheckBox Check { get; set; }

		public TextView Caption { get; set; }

		public QuestionnaireCountriesSelectionAdapterViewHolder(View item)
			: base(item)
		{
			_itemView = item;
			Check = item.FindViewById<CheckBox>(2131296423);
			Caption = item.FindViewById<TextView>(2131296422);
			Check.Clickable = false;
		}

		public void SetOnClickListener(View.IOnClickListener onClickListener)
		{
			_itemView.SetOnClickListener(onClickListener);
		}
	}
}
namespace NDB.Covid19.Droid.Utils
{
	public static class AccessibilityUtils
	{
		public static void AdjustFontScale(Activity context)
		{
			Android.Content.Res.Configuration configuration = context.Resources.Configuration;
			if ((double)configuration.FontScale > 1.25)
			{
				configuration.FontScale = 1.25f;
				DisplayMetrics displayMetrics = context.Resources.DisplayMetrics;
				Android.Runtime.Extensions.JavaCast<IWindowManager>(Application.Context.GetSystemService("window")).DefaultDisplay.GetMetrics(displayMetrics);
				displayMetrics.ScaledDensity = configuration.FontScale * displayMetrics.Density;
				context.CreateConfigurationContext(configuration);
			}
		}
	}
	public static class AuthErrorUtils
	{
		public static void GoToNotInfectedError(Activity parent, LogSeverity severity, System.Exception e, string errorMessage)
		{
			GoToErrorPage(parent, ErrorViewModel.REGISTER_ERROR_NOMATCH_HEADER, ErrorViewModel.REGISTER_ERROR_NOMATCH_DESCRIPTION, ErrorViewModel.REGISTER_ERROR_DISMISS);
			LogUtils.LogException(severity, e, errorMessage);
		}

		public static void GoToManyTriesError(Activity parent, LogSeverity severity, System.Exception e, string errorMessage)
		{
			GoToErrorPage(parent, ErrorViewModel.REGISTER_ERROR_TOOMANYTRIES_HEADER, ErrorViewModel.REGISTER_ERROR_TOOMANYTRIES_DESCRIPTION, ErrorViewModel.REGISTER_ERROR_DISMISS);
			LogUtils.LogException(severity, e, errorMessage);
		}

		public static void GoToTechnicalError(Activity parent, LogSeverity severity, System.Exception e, string errorMessage)
		{
			GoToErrorPage(parent, ErrorViewModel.REGISTER_ERROR_HEADER, ErrorViewModel.REGISTER_ERROR_DESCRIPTION, ErrorViewModel.REGISTER_ERROR_DISMISS);
			LogUtils.LogException(severity, e, errorMessage);
		}

		public static void GoToTechnicalErrorSSINumbers(Activity parent, LogSeverity severity, System.Exception e, string errorMessage)
		{
			GoToErrorPage(parent, ErrorViewModel.REGISTER_ERROR_FETCH_SSI_DATA_HEADER, ErrorViewModel.REGISTER_ERROR_FETCH_SSI_DATA_DESCRIPTION, ErrorViewModel.REGISTER_ERROR_DISMISS);
			LogUtils.LogException(severity, e, errorMessage);
		}

		public static void GoToErrorPage(Activity parent, string title, string description, string button, string subtitle = null)
		{
			LocalPreferencesHelper.UpdateCorrelationId(null);
			Intent intent = new Intent(parent, typeof(GeneralErrorActivity));
			Bundle bundle = new Bundle();
			bundle.PutString("title", title);
			bundle.PutString("description", description);
			bundle.PutString("button", button);
			if (subtitle != null)
			{
				bundle.PutString("subtitle", subtitle);
			}
			intent.PutExtras(bundle);
			parent.StartActivity(intent);
		}
	}
	internal class BackgroundFetchScheduler
	{
		private class BackgroundFetchWorker : Worker
		{
			private static int _runAttemptCount;

			private static readonly int _minimalDisplayTime = 3000;

			public BackgroundFetchWorker(Context context, WorkerParameters workerParameters)
				: base(context, workerParameters)
			{
			}

			public override Result DoWork()
			{
				try
				{
					SetForegroundAsync(CreateForegroundInfo());
					System.Threading.Tasks.Task.Run((Func<System.Threading.Tasks.Task>)DoAsyncWork).GetAwaiter().GetResult();
					return Result.InvokeSuccess();
				}
				catch (System.Exception e)
				{
					if (_runAttemptCount < 3)
					{
						LogUtils.LogException(LogSeverity.WARNING, e, "BackgroundFetchScheduler.BackgroundFetchWorker.DoWork: Failed to perform key background fetch. Retrying now.");
						_runAttemptCount++;
						return Result.InvokeRetry();
					}
					LogUtils.LogException(LogSeverity.WARNING, e, "BackgroundFetchScheduler.BackgroundFetchWorker.DoWork: Failed to perform key background fetch. Pull aborted. BG Task is rescheduled");
					_runAttemptCount = 0;
					return Result.InvokeFailure();
				}
			}

			private ForegroundInfo CreateForegroundInfo()
			{
				return new ForegroundInfo(4, new LocalNotificationsManager().CreateNotification(NotificationsEnum.BackgroundFetch.Data()).GetAwaiter().GetResult());
			}

			private async System.Threading.Tasks.Task DoAsyncWork()
			{
				Stopwatch timer = new Stopwatch();
				timer.Start();
				try
				{
					if (await ExposureNotification.IsEnabledAsync())
					{
						await ExposureNotification.UpdateKeysFromServer();
					}
					else
					{
						LogUtils.LogMessage(LogSeverity.WARNING, "BackgroundFetchScheduler.DoAsyncWork (Android): EN API is not enabled. Aborting pull.");
					}
				}
				catch (System.Exception e)
				{
					e.HandleExposureNotificationException("BackgroundFetchScheduler", "DoAsyncWork");
				}
				timer.Stop();
				if (timer.ElapsedMilliseconds < _minimalDisplayTime)
				{
					await System.Threading.Tasks.Task.Delay((int)(_minimalDisplayTime - timer.ElapsedMilliseconds));
				}
			}
		}

		public static void ScheduleBackgroundFetch()
		{
			PeriodicWorkRequest.Builder builder = new PeriodicWorkRequest.Builder(typeof(BackgroundFetchWorker), Conf.BACKGROUND_FETCH_REPEAT_INTERVAL_ANDROID);
			builder.SetPeriodStartTime(TimeSpan.FromSeconds(1.0)).SetBackoffCriteria(BackoffPolicy.Linear, TimeSpan.FromSeconds(10.0)).SetConstraints(new AndroidX.Work.Constraints.Builder().SetRequiredNetworkType(NetworkType.Connected).Build());
			PeriodicWorkRequest p = builder.Build();
			WorkManager.GetInstance(Platform.AppContext).EnqueueUniquePeriodicWork("exposurenotification", ExistingPeriodicWorkPolicy.Keep, p);
		}
	}
	public static class DroidExposureNotificationsStatusHelper
	{
		private static IExposureNotificationClient Instance => NearbyClass.GetExposureNotificationClient(Application.Context);

		public static async Task<bool> IsNearbyExposureNotificationBluetoothAndLocationDisabled()
		{
			try
			{
				return (await Instance.NativeGetStatus().CastTask()).ToArray().ToList().Any((Java.Lang.Object s) => s == ExposureNotificationStatus.BluetoothDisabled || s == ExposureNotificationStatus.LocationDisabled || s == ExposureNotificationStatus.BluetoothSupportUnknown);
			}
			catch (System.Exception)
			{
				return true;
			}
		}

		public static async Task<bool> HasNearbyExposureNotificationStatus(ExposureNotificationStatus status)
		{
			try
			{
				return (await Instance.NativeGetStatus().CastTask()).ToArray().ToList().Any((Java.Lang.Object s) => s == status);
			}
			catch (System.Exception)
			{
				return true;
			}
		}
	}
	internal static class ExposureNotificationStatusExtensions
	{
		private class MyCompleteListener : Java.Lang.Object, IOnCompleteListener, IJavaObject, IDisposable, IJavaPeerable
		{
			private Action<Android.Gms.Tasks.Task> OnCompleteHandler { get; }

			public MyCompleteListener(Action<Android.Gms.Tasks.Task> onComplete)
			{
				OnCompleteHandler = onComplete;
			}

			public void OnComplete(Android.Gms.Tasks.Task task)
			{
				OnCompleteHandler?.Invoke(task);
			}
		}

		public static Task<AbstractCollection> CastTask(this Android.Gms.Tasks.Task androidTask)
		{
			TaskCompletionSource<AbstractCollection> tcs = new TaskCompletionSource<AbstractCollection>();
			androidTask.AddOnCompleteListener(new MyCompleteListener(delegate(Android.Gms.Tasks.Task t)
			{
				if (t.Exception == null)
				{
					tcs.TrySetResult(Android.Runtime.Extensions.JavaCast<AbstractCollection>(t.Result));
				}
				else
				{
					tcs.TrySetException(t.Exception);
				}
			}));
			return tcs.Task;
		}
	}
	public class ForegroundServiceHelper
	{
		public static void StartForegroundServiceCompat<T>(Context context, Bundle args = null) where T : Service
		{
			Intent intent = new Intent(context, typeof(T));
			if (args != null)
			{
				intent.PutExtras(args);
			}
			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				context.StartForegroundService(intent);
			}
			else
			{
				context.StartService(intent);
			}
		}

		public static void StopForegroundServiceCompat<T>(Context context) where T : Service
		{
			Intent service = new Intent(context, typeof(T));
			context.StopService(service);
		}
	}
	public class PermissionUtils : IPermissionsHelper
	{
		private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

		public Task<bool> IsBluetoothEnabled()
		{
			return System.Threading.Tasks.Task.Run(async delegate
			{
				bool flag = !(await DroidExposureNotificationsStatusHelper.HasNearbyExposureNotificationStatus(ExposureNotificationStatus.BluetoothDisabled));
				if (flag)
				{
					flag = !(await DroidExposureNotificationsStatusHelper.HasNearbyExposureNotificationStatus(ExposureNotificationStatus.BluetoothSupportUnknown));
				}
				return flag;
			});
		}

		public Task<bool> IsLocationEnabled()
		{
			if (Build.VERSION.SdkInt >= (BuildVersionCodes)30)
			{
				return System.Threading.Tasks.Task.FromResult(result: true);
			}
			return System.Threading.Tasks.Task.Run(async () => !(await DroidExposureNotificationsStatusHelper.HasNearbyExposureNotificationStatus(ExposureNotificationStatus.LocationDisabled)));
		}

		public async Task<bool> AreAllPermissionsGranted()
		{
			return !(await DroidExposureNotificationsStatusHelper.IsNearbyExposureNotificationBluetoothAndLocationDisabled());
		}

		public async Task<bool> HasPermissions()
		{
			_tcs = new TaskCompletionSource<bool>();
			if (await HasLocationPermissionsAsync() & await HasBluetoothSupportAsync())
			{
				_tcs.TrySetResult(result: true);
			}
			bool result = await _tcs.Task;
			PermissionsMessagingCenter.PermissionsChanged = false;
			return result;
		}

		public async Task<bool> CheckPermissionsIfChangedWhileIdle()
		{
			if (PermissionsMessagingCenter.PermissionsChanged)
			{
				return await HasPermissions();
			}
			return true;
		}

		public void SubscribePermissionsMessagingCenter(object subscriber, Action<object> action)
		{
			PermissionsMessagingCenter.SubscribeForPermissionsChanged(subscriber, action);
		}

		public void UnsubscribePErmissionsMessagingCenter(object subscriber)
		{
			PermissionsMessagingCenter.Unsubscribe(subscriber);
		}

		public async Task<bool> HasPermissionsWithoutDialogs()
		{
			bool flag = await IsBluetoothEnabled();
			if (flag)
			{
				flag = await IsLocationEnabled();
			}
			return flag;
		}

		public async Task<bool> HasBluetoothSupportAsync()
		{
			if (await HasBluetoothAdapter() && BluetoothAdapter.DefaultAdapter.IsEnabled)
			{
				return true;
			}
			await DialogUtils.DisplayDialogAsync(CrossCurrentActivity.Current.Activity, new DialogViewModel
			{
				Title = "PERMISSION_BLUETOOTH_NEEDED_TITLE".Translate(),
				Body = "PERMISSION_ENABLE_LOCATION_AND_BLUETOOTH".Translate(),
				OkBtnTxt = CrossCurrentActivity.Current.Activity.Resources.GetString(17039370),
				CancelbtnTxt = CrossCurrentActivity.Current.Activity.Resources.GetString(17039360)
			}, GoToBluetoothSettings, CancelTask);
			return false;
		}

		public async Task<bool> HasBluetoothAdapter()
		{
			if (BluetoothAdapter.DefaultAdapter != null)
			{
				return true;
			}
			await DialogUtils.DisplayDialogAsync(CrossCurrentActivity.Current.Activity, new DialogViewModel
			{
				Title = "NO_BLUETOOTH_TITLE".Translate(),
				Body = "NO_BLUETOOTH_MSG".Translate(),
				OkBtnTxt = CrossCurrentActivity.Current.Activity.Resources.GetString(17039370)
			});
			return false;
		}

		public async Task<bool> HasLocationPermissionsAsync()
		{
			if (await IsLocationEnabled())
			{
				return true;
			}
			await DialogUtils.DisplayDialogAsync(CrossCurrentActivity.Current.Activity, new DialogViewModel
			{
				Title = "PERMISSION_LOCATION_NEEDED_TITLE".Translate(),
				Body = "PERMISSION_ENABLE_LOCATION_AND_BLUETOOTH".Translate(),
				OkBtnTxt = CrossCurrentActivity.Current.Activity.Resources.GetString(17039370)
			}, GoToLocationSettings);
			return false;
		}

		private void CancelTask()
		{
			_tcs.TrySetResult(result: false);
		}

		private void GoToBluetoothSettings()
		{
			try
			{
				CrossCurrentActivity.Current.Activity.StartActivityForResult(new Intent().SetAction("android.settings.BLUETOOTH_SETTINGS"), 1);
			}
			catch (System.Exception e)
			{
				LogUtils.LogException(LogSeverity.WARNING, e, "PermissionUtils.GoToBluetoothSettings: Failed to go to bluetooth settings");
			}
		}

		private void GoToLocationSettings()
		{
			try
			{
				CrossCurrentActivity.Current.Activity.StartActivityForResult(new Intent().SetAction("android.settings.LOCATION_SOURCE_SETTINGS"), 2);
			}
			catch (System.Exception e)
			{
				LogUtils.LogException(LogSeverity.WARNING, e, "GoToLocationSettings");
			}
		}

		public async void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if ((requestCode == 1 || requestCode == 2) && resultCode != Result.FirstUser)
			{
				TaskCompletionSource<bool> tcs = _tcs;
				tcs.TrySetResult(await HasPermissionsWithoutDialogs());
			}
		}
	}
	public class LocalNotificationsManager : ILocalNotificationsManager
	{
		private const string _broadcastName = "com.netcompany.smittestop_exposure_notification.background_notification";

		private readonly string _backgroundFetchChannelId = "4_background_channel";

		private readonly Context _context;

		private readonly string _countdownChannelId = "3_countdown_channel";

		private readonly string _exposureChannelId = "0_exposure_channel";

		private readonly string _permissionsChannelId = "1_permissions_channel";

		private readonly string _reminderChannelId = "2_reminder_channel";

		private Context NotificationContext => _context ?? CrossCurrentActivity.Current.Activity ?? CrossCurrentActivity.Current.AppContext;

		public LocalNotificationsManager(Context context = null)
		{
			_context = context;
			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				CreateChannels();
			}
		}

		public void GenerateLocalNotification(NotificationViewModel notificationViewModel, long triggerInSeconds)
		{
			BroadcastNotification(notificationViewModel, NotificationType.Local);
		}

		public void GenerateLocalNotificationOnlyIfInBackground(NotificationViewModel viewModel)
		{
			BroadcastNotification(viewModel, NotificationType.InBackground);
		}

		public void GenerateLocalPermissionsNotification(NotificationViewModel viewModel)
		{
			BroadcastNotification(viewModel, NotificationType.Permissions);
		}

		public void GenerateDelayedNotification(NotificationViewModel viewModel, long ticks)
		{
			Intent intent = new Intent();
			intent.SetAction("com.netcompany.smittestop_exposure_notification.background_notification");
			intent.PutExtra("type", 3);
			intent.PutExtra("data", (int)viewModel.Type);
			intent.PutExtra("ticks", ticks);
			LocalBroadcastManager.GetInstance(CrossCurrentActivity.Current.Activity ?? CrossCurrentActivity.Current.AppContext).SendBroadcast(intent);
		}

		private void CreateChannels()
		{
			NotificationManager obj = (NotificationManager)NotificationContext.GetSystemService("notification");
			NotificationChannel notificationChannel = new NotificationChannel(_exposureChannelId, NotificationChannelsViewModel.NOTIFICATION_CHANNEL_EXPOSURE_NAME, NotificationImportance.High)
			{
				Description = NotificationChannelsViewModel.NOTIFICATION_CHANNEL_EXPOSURE_DESCRIPTION
			};
			notificationChannel.SetShowBadge(showBadge: true);
			NotificationChannel notificationChannel2 = new NotificationChannel(_permissionsChannelId, NotificationChannelsViewModel.NOTIFICATION_CHANNEL_PERMISSIONS_NAME, NotificationImportance.High)
			{
				Description = NotificationChannelsViewModel.NOTIFICATION_CHANNEL_PERMISSIONS_DESCRIPTION
			};
			notificationChannel2.SetShowBadge(showBadge: true);
			NotificationChannel notificationChannel3 = new NotificationChannel(_backgroundFetchChannelId, NotificationChannelsViewModel.NOTIFICATION_CHANNEL_BACKGROUND_FETCH_NAME, NotificationImportance.Low)
			{
				Description = NotificationChannelsViewModel.NOTIFICATION_CHANNEL_BACKGROUND_FETCH_DESCRIPTION
			};
			notificationChannel3.SetShowBadge(showBadge: false);
			NotificationChannel item = new NotificationChannel(_countdownChannelId, NotificationChannelsViewModel.NOTIFICATION_CHANNEL_COUNTDOWN_NAME, NotificationImportance.Low)
			{
				Description = NotificationChannelsViewModel.NOTIFICATION_CHANNEL_COUNTDOWN_DESCRIPTION
			};
			notificationChannel3.SetShowBadge(showBadge: false);
			NotificationChannel item2 = new NotificationChannel(_reminderChannelId, NotificationChannelsViewModel.NOTIFICATION_CHANNEL_REMINDER_NAME, NotificationImportance.High)
			{
				Description = NotificationChannelsViewModel.NOTIFICATION_CHANNEL_REMINDER_DESCRIPTION
			};
			notificationChannel2.SetShowBadge(showBadge: true);
			obj?.CreateNotificationChannels(new List<NotificationChannel> { notificationChannel, notificationChannel2, notificationChannel3, item, item2 });
		}

		private string SelectChannel(NotificationsEnum type)
		{
			return type switch
			{
				NotificationsEnum.NewMessageReceived => _exposureChannelId, 
				NotificationsEnum.BackgroundFetch => _backgroundFetchChannelId, 
				NotificationsEnum.TimedReminder => _countdownChannelId, 
				NotificationsEnum.TimedReminderFinished => _reminderChannelId, 
				_ => _permissionsChannelId, 
			};
		}

		private NotificationPriority SelectPriorityForLowerVersions(NotificationsEnum type)
		{
			return type switch
			{
				NotificationsEnum.NewMessageReceived => NotificationPriority.Default, 
				NotificationsEnum.BackgroundFetch => NotificationPriority.Low, 
				_ => NotificationPriority.Default, 
			};
		}

		public Task<Notification> CreateNotification(NotificationViewModel notificationViewModel)
		{
			PendingIntent contentIntent = InitResultIntentBasingOnViewModel(notificationViewModel);
			NotificationCompat.Builder builder = new NotificationCompat.Builder(NotificationContext, SelectChannel(notificationViewModel.Type)).SetAutoCancel(autoCancel: true).SetContentTitle(notificationViewModel.Title).SetStyle(new NotificationCompat.BigTextStyle().BigText(notificationViewModel.Body))
				.SetContentText(notificationViewModel.Body)
				.SetContentIntent(contentIntent)
				.SetVibrate(null)
				.SetSound(null)
				.SetNumber(1)
				.SetCategory("msg")
				.SetOnlyAlertOnce(onlyAlertOnce: true);
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
			{
				builder.SetColor(2131034164);
			}
			builder.SetSmallIcon(2131165373);
			bool num = Build.VERSION.SdkInt < BuildVersionCodes.O;
			if (num)
			{
				builder.SetPriority((int)SelectPriorityForLowerVersions(notificationViewModel.Type));
			}
			Notification notification = builder.Build();
			bool flag = ShortcutBadger.IsBadgeCounterSupported(NotificationContext);
			bool flag2 = notificationViewModel.Type == NotificationsEnum.NewMessageReceived;
			bool flag3 = NotificationManagerCompat.From(NotificationContext).AreNotificationsEnabled();
			if (num && flag && flag2 && flag3)
			{
				ShortcutBadger.ApplyNotification(NotificationContext, notification, 1);
			}
			return System.Threading.Tasks.Task.FromResult(notification);
		}

		public Notification CreateNotificationWithExtraLongData(NotificationViewModel notificationViewModel, long ticks = 0L)
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(ticks);
			PendingIntent contentIntent = InitResultIntentBasingOnViewModel(notificationViewModel);
			NotificationCompat.Builder builder = new NotificationCompat.Builder(NotificationContext, SelectChannel(notificationViewModel.Type)).SetContentTitle(notificationViewModel.Title).SetStyle(new NotificationCompat.BigTextStyle().BigText(string.Format(notificationViewModel.Body, timeSpan.ToString("hh':'mm':'ss")))).SetContentText(string.Format(notificationViewModel.Body, timeSpan.ToString("hh':'mm':'ss")))
				.SetContentIntent(contentIntent)
				.SetVibrate(null)
				.SetSound(null)
				.SetCategory("status")
				.SetOnlyAlertOnce(onlyAlertOnce: true);
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
			{
				builder.SetColor(2131034164);
			}
			builder.SetSmallIcon(2131165373);
			return builder.Build();
		}

		private PendingIntent InitResultIntentBasingOnViewModel(NotificationViewModel notificationViewModel)
		{
			Android.App.TaskStackBuilder taskStackBuilder = Android.App.TaskStackBuilder.Create(NotificationContext);
			Intent nextIntent;
			if (notificationViewModel.Type == NotificationsEnum.NewMessageReceived.Data().Type)
			{
				nextIntent = new Intent(NotificationContext, typeof(MessagesActivity));
				taskStackBuilder.AddParentStack(Class.FromType(typeof(MessagesActivity)));
			}
			else
			{
				nextIntent = new Intent(NotificationContext, typeof(InitializerActivity));
				taskStackBuilder.AddParentStack(Class.FromType(typeof(InitializerActivity)));
			}
			taskStackBuilder.AddNextIntent(nextIntent);
			return taskStackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);
		}

		private static void BroadcastNotification(NotificationViewModel viewModel, NotificationType type)
		{
			Intent intent = new Intent();
			intent.SetAction("com.netcompany.smittestop_exposure_notification.background_notification");
			intent.PutExtra("type", (int)type);
			intent.PutExtra("data", (int)viewModel.Type);
			LocalBroadcastManager.GetInstance(CrossCurrentActivity.Current.Activity ?? CrossCurrentActivity.Current.AppContext).SendBroadcast(intent);
		}
	}
	public static class WelcomePageTools
	{
		public static void SetArrowVisibility(View view)
		{
			bool flag = (CrossCurrentActivity.Current.Activity as WelcomeActivity)?.IsOnBoarding ?? false;
			Button button = view.FindViewById<Button>(2131296333);
			button.ContentDescription = SettingsViewModel.SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON;
			button.Visibility = (flag ? ViewStates.Gone : ViewStates.Visible);
			if (!flag)
			{
				button.Click += new StressUtils.SingleClick(delegate
				{
					CrossCurrentActivity.Current.Activity.Finish();
				}).Run;
			}
		}
	}
	public abstract class PlayServicesVersionUtils
	{
		public static bool PlayServicesVersionNumberIsLargeEnough(PackageManager packageManager)
		{
			long num = 201300000L;
			bool num2 = PackageInfoCompat.GetLongVersionCode(CrossCurrentActivity.Current.AppContext.PackageManager.GetPackageInfo("com.google.android.gms", (PackageInfoFlags)0)) >= num;
			if (!num2)
			{
				LogUtils.LogMessage(LogSeverity.INFO, "PlayServicesVersionUtils: User is prevented from using the app because of too low GPS version");
			}
			return num2;
		}
	}
	public static class DialogUtils
	{
		private static readonly string _logPrefix = "Android DialogUtils: ";

		[Obsolete("Use DisplayDialogAsync instead")]
		public static void DisplayDialog(Activity current, string title, string message, string okBtnText, System.Action action = null)
		{
			if (current != null && !current.IsFinishing)
			{
				new Android.App.AlertDialog.Builder(current, 16974126).SetTitle(title).SetMessage(message).SetPositiveButton(okBtnText, delegate
				{
					action?.Invoke();
				})
					.SetCancelable(cancelable: false)
					.Show();
			}
		}

		public static void DisplayBubbleDialog(Activity current, string message, string buttonText)
		{
			if (current != null && !current.IsFinishing)
			{
				View view = LayoutInflater.From(current).Inflate(2131492899, null);
				TextView textView = view.FindViewById<TextView>(2131296354);
				Button button = view.FindViewById<Button>(2131296359);
				textView.Text = message;
				button.Text = buttonText;
				Android.App.AlertDialog dialog = new Android.App.AlertDialog.Builder(current).SetView(view).SetCancelable(cancelable: true).Create();
				button.Click += new StressUtils.SingleClick(delegate
				{
					dialog.Dismiss();
				}).Run;
				dialog.Window.SetLayout(-2, -2);
				dialog.Show();
			}
		}

		public static Task<bool> DisplayDialogAsync(Activity activity, string title, string message, string okBtnText = null, string noBtnText = null, int? okBtnTextResourceId = null, int? noBtnTextResourceId = null)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			if (activity == null || activity.IsFinishing)
			{
				tcs.TrySetResult(result: false);
				return tcs.Task;
			}
			Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(activity, 16974126).SetTitle(title).SetMessage(message).SetCancelable(cancelable: false);
			string additionalInfo = AdditionalDialogInfoForLogging(activity, title, message, okBtnText, noBtnText);
			if (!string.IsNullOrEmpty(okBtnText) || okBtnTextResourceId.HasValue)
			{
				if (!string.IsNullOrEmpty(okBtnText))
				{
					builder.SetPositiveButton(okBtnText, delegate
					{
						SetResultWithCatch(tcs, result: true, additionalInfo);
					});
				}
				if (okBtnTextResourceId.HasValue)
				{
					builder.SetPositiveButton(okBtnTextResourceId.Value, delegate
					{
						SetResultWithCatch(tcs, result: true, additionalInfo);
					});
				}
			}
			if (!string.IsNullOrEmpty(noBtnText) || noBtnTextResourceId.HasValue)
			{
				if (!string.IsNullOrEmpty(noBtnText))
				{
					builder.SetNegativeButton(noBtnText, delegate
					{
						SetResultWithCatch(tcs, result: false, additionalInfo);
					});
				}
				if (noBtnTextResourceId.HasValue)
				{
					builder.SetNegativeButton(noBtnTextResourceId.Value, delegate
					{
						SetResultWithCatch(tcs, result: false, additionalInfo);
					});
				}
			}
			builder.Show();
			return tcs.Task;
		}

		public static Task<bool> DisplayDialogAsync(Activity activity, DialogViewModel dialogViewModel)
		{
			return DisplayDialogAsync(activity, dialogViewModel.Title, dialogViewModel.Body, dialogViewModel.OkBtnTxt, dialogViewModel.CancelbtnTxt);
		}

		private static string AdditionalDialogInfoForLogging(Activity current, string title, string message, string okBtnText, string noBtnText)
		{
			return "Exception occured in Activity " + current.Class.SimpleName + ". dialog title: " + title + ", dialog message: " + message + ", dialog okBtnText: " + okBtnText + ", dialog noBtnText: " + noBtnText;
		}

		private static void SetResultWithCatch(TaskCompletionSource<bool> tcs, bool result, string additionalInfo)
		{
			try
			{
				tcs.SetResult(result);
			}
			catch (System.Exception ex)
			{
				if (ex is InvalidOperationException)
				{
					LogUtils.LogException(LogSeverity.WARNING, ex, _logPrefix + "InvalidOperationException, this indicates the task was already in a final state when SetResult was called, (User most likely pressed both button at the same time)", additionalInfo);
				}
				else if (ex is ObjectDisposedException)
				{
					LogUtils.LogException(LogSeverity.ERROR, ex, _logPrefix + "ObjectDisposedException, this indicates the task was disposed when SetResult was called.", additionalInfo);
				}
				else
				{
					LogUtils.LogException(LogSeverity.ERROR, ex, _logPrefix + "Some Exception from SetResult operation.", additionalInfo);
				}
			}
		}

		public static async Task<bool> DisplayDialogAsync(Activity current, DialogViewModel viewModel, System.Action actionOk = null, System.Action actionNotOk = null)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			if (current == null || current.IsFinishing)
			{
				tcs.TrySetResult(result: false);
				return await tcs.Task;
			}
			if (await DisplayDialogAsync(current, viewModel.Title, viewModel.Body, viewModel.OkBtnTxt, viewModel.CancelbtnTxt))
			{
				actionOk?.Invoke();
				tcs.TrySetResult(result: true);
			}
			else
			{
				actionNotOk?.Invoke();
				tcs.TrySetResult(result: false);
			}
			return await tcs.Task;
		}
	}
	public class DroidRequestCodes
	{
		public const int BluetoothRequestCode = 1;

		public const int LocationRequestCode = 2;

		public const string isOnBoardinIntentExtra = "isOnBoarding";
	}
	public class LinkUtil
	{
		public static void LinkifyTextView(TextView textView)
		{
			Pattern pattern = Pattern.Compile("(?:^|[\\W])((ht|f)tp(s?):\\/\\/|www\\.)(([\\w\\-]+\\.){1,}?([\\w\\-.~]+\\/?)*[\\p{Alnum}.,%_=?&#\\-+()\\[\\]\\*$~@!:/{};']*)");
			Linkify.AddLinks(textView, pattern, "https://");
			textView.MovementMethod = LinkMovementMethod.Instance;
		}
	}
	public static class NavigationHelper
	{
		public static void GoToSettingsPage(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(SettingsActivity));
			parent.StartActivity(intent);
		}

		public static void GoToSettingsHowItWorksPage(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(SettingsHowItWorksActivity));
			parent.StartActivity(intent);
		}

		public static void GoToSettingsHelpPage(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(SettingsHelpActivity));
			parent.StartActivity(intent);
		}

		public static void GoToSettingsAboutPage(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(SettingsAbout));
			parent.StartActivity(intent);
		}

		public static void GoToDebugPage(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(ENDeveloperToolsActivity));
			parent.StartActivity(intent);
		}

		public static void GoToGenetalSettingsPage(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(SettingsGeneralActivity));
			parent.StartActivity(intent);
		}

		public static void GoToResultPageAndClearTop(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(InfectionStatusActivity));
			intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
			parent.StartActivity(intent);
		}

		public static void GoToWelcomeWhatsNewPage(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(WelcomePageWhatIsNewActivity));
			intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
			parent.StartActivity(intent);
		}

		public static void RestartApp(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(InitializerActivity));
			intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
			parent.StartActivity(intent);
		}

		public static void GoToConsentsWithdrawPage(Activity parent)
		{
			Intent intent = new Intent(parent, typeof(SettingsWithdrawConsentsActivity));
			parent.StartActivity(intent);
		}

		public static void GoToOnBoarding(Activity parent, bool isOnBoarding)
		{
			Intent intent = new Intent(parent, typeof(WelcomeActivity));
			intent.PutExtra("isOnBoarding", isOnBoarding);
			parent.StartActivity(intent);
		}

		public static Intent GetStartPageIntent(Activity parent)
		{
			if (LocalPreferencesHelper.IsOnboardingCompleted)
			{
				return new Intent(parent, typeof(InfectionStatusActivity));
			}
			return new Intent(parent, typeof(InitializerActivity));
		}

		public static void GoToStartPageIfIsOnboarded(Activity parent)
		{
			if (OnboardingStatusHelper.Status == OnboardingStatus.CountriesOnboardingCompleted)
			{
				GoToResultPageAndClearTop(parent);
			}
		}
	}
	public class StressUtils
	{
		public class GenericSingleAction<T1, T2>
		{
			private readonly int _delayMilliseconds;

			private readonly Action<T1, T2> _setOnAction;

			private bool _hasStarted;

			public GenericSingleAction(Action<T1, T2> setOnAction, int delayMilliseconds = 1000)
			{
				_setOnAction = setOnAction;
				_delayMilliseconds = delayMilliseconds;
			}

			public void Run(T1 v, T2 e)
			{
				if (!_hasStarted)
				{
					_hasStarted = true;
					_setOnAction?.Invoke(v, e);
				}
				Reset();
			}

			private void Reset()
			{
				new Handler().PostDelayed(delegate
				{
					_hasStarted = false;
				}, _delayMilliseconds);
			}
		}

		public class SingleAction<T> : GenericSingleAction<object, T>
		{
			public SingleAction(Action<object, T> setOnAction, int delayMilliseconds = 1000)
				: base(setOnAction, delayMilliseconds)
			{
			}
		}

		public class SingleClick : SingleAction<EventArgs>
		{
			public SingleClick(Action<object, EventArgs> setOnAction, int delayMilliseconds = 1000)
				: base(setOnAction, delayMilliseconds)
			{
			}
		}
	}
}
namespace NDB.Covid19.Droid.Utils.MessagingCenter
{
	public class PermissionsMessagingCenter
	{
		public static bool PermissionsChanged { get; set; }

		public static void SubscribeForPermissionsChanged(object sender, Action<object> action)
		{
			NDB.Covid19.Utils.MessagingCenter.Subscribe(sender, MessagingCenterKeys.KEY_PERMISSIONS_CHANGED, action);
		}

		public static void Unsubscribe(object sender)
		{
			NDB.Covid19.Utils.MessagingCenter.Unsubscribe<object>(sender, MessagingCenterKeys.KEY_PERMISSIONS_CHANGED);
		}

		public static void NotifyPermissionsChanged(object sender)
		{
			PermissionsChanged = true;
			NDB.Covid19.Utils.MessagingCenter.Send(sender, MessagingCenterKeys.KEY_PERMISSIONS_CHANGED);
		}
	}
}
namespace NDB.Covid19.Droid.Services
{
	public enum BluetoothState
	{
		NO_ADAPTER,
		ON,
		OFF
	}
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new string[] { "android.bluetooth.adapter.action.STATE_CHANGED" })]
	internal class BluetoothStateBroadcastReceiver : BroadcastReceiver
	{
		private TaskCompletionSource<bool> _isBluetoothOnAsync;

		public System.Action OnBluetoothStateChange { get; set; }

		public override void OnReceive(Context context, Intent intent)
		{
			string action = intent.Action;
			if (action != null && (action.Equals("android.location.PROVIDERS_CHANGED") || action.Equals("android.bluetooth.adapter.action.STATE_CHANGED")))
			{
				switch (intent.GetIntExtra("android.bluetooth.adapter.extra.STATE", -2147483648))
				{
				case 12:
					_isBluetoothOnAsync?.TrySetResult(result: true);
					OnBluetoothStateChange?.Invoke();
					break;
				case 10:
					_isBluetoothOnAsync?.TrySetResult(result: false);
					OnBluetoothStateChange?.Invoke();
					break;
				}
			}
		}

		private Task<bool> GetBluetoothStateAsync()
		{
			_isBluetoothOnAsync = new TaskCompletionSource<bool>();
			return _isBluetoothOnAsync.Task;
		}

		public static async Task<BluetoothState> GetBluetoothState(System.Action callbackAction)
		{
			BluetoothStateBroadcastReceiver bluetoothStateBroadcastReceiver = new BluetoothStateBroadcastReceiver();
			BluetoothStateBroadcastReceiver bluetoothStateBroadcastReceiver2 = bluetoothStateBroadcastReceiver;
			bluetoothStateBroadcastReceiver2.OnBluetoothStateChange = (System.Action)Delegate.Combine(bluetoothStateBroadcastReceiver2.OnBluetoothStateChange, callbackAction);
			CrossCurrentActivity.Current.AppContext.RegisterReceiver(bluetoothStateBroadcastReceiver, new IntentFilter("android.bluetooth.adapter.action.STATE_CHANGED"));
			new CancellationTokenSource(2000).Token.Register(delegate
			{
				bluetoothStateBroadcastReceiver._isBluetoothOnAsync?.TrySetResult(BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled);
				bluetoothStateBroadcastReceiver.OnBluetoothStateChange?.Invoke();
			}, useSynchronizationContext: false);
			_ = BluetoothAdapter.DefaultAdapter;
			int result = ((await bluetoothStateBroadcastReceiver.GetBluetoothStateAsync()) ? 1 : 2);
			CrossCurrentActivity.Current.AppContext.UnregisterReceiver(bluetoothStateBroadcastReceiver);
			return (BluetoothState)result;
		}
	}
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new string[] { "android.intent.action.AIRPLANE_MODE" })]
	internal class FlightModeHandlerBroadcastReceiver : BroadcastReceiver
	{
		public static bool IsFlightModeOn { get; set; }

		public static System.Action OnFlightModeChange { get; set; }

		public override async void OnReceive(Context context, Intent intent)
		{
			IsFlightModeOn = intent.GetBooleanExtra("state", defaultValue: false);
			await BluetoothStateBroadcastReceiver.GetBluetoothState(OnFlightModeChange);
			OnFlightModeChange?.Invoke();
		}
	}
	[BroadcastReceiver]
	[IntentFilter(new string[] { "com.netcompany.smittestop_exposure_notification.background_notification" })]
	public class BackgroundNotificationBroadcastReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			NotificationType intExtra = (NotificationType)intent.GetIntExtra("type", 0);
			NotificationsEnum intExtra2 = (NotificationsEnum)intent.GetIntExtra("data", 0);
			switch (intExtra)
			{
			case NotificationType.Local:
				GenerateLocalNotificationBroadcasted(context, intExtra2.Data(), 0);
				break;
			case NotificationType.Permissions:
				GenerateLocalPermissionsNotificationBroadcasted(context, intExtra2.Data());
				break;
			case NotificationType.InBackground:
				GenerateLocalNotificationOnlyIfInBackgroundBroadcasted(context, intExtra2.Data());
				break;
			case NotificationType.ForegroundWithUpdates:
			{
				long longExtra = intent.GetLongExtra("ticks", 0L);
				GenerateForegroundServiceNotificationWithUpdatesBroadcasted(context, intExtra2.Data(), longExtra);
				break;
			}
			}
		}

		private void GenerateLocalPermissionsNotificationBroadcasted(Context context, NotificationViewModel viewModel)
		{
			System.Threading.Tasks.Task.Run(async delegate
			{
				NotificationManagerCompat notificationManagerCompat = NotificationManagerCompat.From(context);
				NotificationManagerCompat notificationManagerCompat2 = notificationManagerCompat;
				int type = (int)viewModel.Type;
				notificationManagerCompat2.Notify(type, await new LocalNotificationsManager(context).CreateNotification(viewModel));
			});
		}

		private void GenerateLocalNotificationOnlyIfInBackgroundBroadcasted(Context context, NotificationViewModel viewModel)
		{
			ActivityManager.RunningAppProcessInfo runningAppProcessInfo = new ActivityManager.RunningAppProcessInfo();
			ActivityManager.GetMyMemoryState(runningAppProcessInfo);
			if (runningAppProcessInfo.Importance != Importance.Foreground)
			{
				new LocalNotificationsManager(context).GenerateLocalNotification(viewModel, 0L);
				LocalPreferencesHelper.TermsNotificationWasShown = true;
			}
		}

		private void GenerateLocalNotificationBroadcasted(Context context, NotificationViewModel notificationViewModel, int triggerInSeconds)
		{
			System.Threading.Tasks.Task.Run(async delegate
			{
				NotificationManagerCompat notificationManagerCompat = NotificationManagerCompat.From(context);
				await System.Threading.Tasks.Task.Delay(triggerInSeconds * 1000);
				NotificationManagerCompat notificationManagerCompat2 = notificationManagerCompat;
				int type = (int)notificationViewModel.Type;
				notificationManagerCompat2.Notify(type, await new LocalNotificationsManager(context).CreateNotification(notificationViewModel));
			});
		}

		private void GenerateForegroundServiceNotificationWithUpdatesBroadcasted(Context context, NotificationViewModel notificationViewModel, long ticksLeft)
		{
			System.Threading.Tasks.Task.Run(delegate
			{
				NotificationManagerCompat.From(context).Notify((int)notificationViewModel.Type, new LocalNotificationsManager(context).CreateNotificationWithExtraLongData(notificationViewModel, ticksLeft));
			});
		}
	}
	[Service(Enabled = true, Exported = false)]
	public class RemovedFromRecentDetectorService : Service
	{
		public static bool IsRunning { get; set; }

		public override void OnCreate()
		{
			base.OnCreate();
			IsRunning = true;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			IsRunning = false;
		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override void OnTaskRemoved(Intent rootIntent)
		{
			base.OnTaskRemoved(rootIntent);
			try
			{
				string correlationId = LocalPreferencesHelper.GetCorrelationId();
				if (!string.IsNullOrEmpty(correlationId))
				{
					LogUtils.LogMessage(LogSeverity.INFO, "The user has closed the app", null, correlationId);
				}
				LogUtils.SendAllLogs();
			}
			catch
			{
			}
		}
	}
	[Service]
	public class TimedReminderForegroundService : Service
	{
		private class TimedReminderCountdownTimer : CountDownTimer
		{
			private readonly LocalNotificationsManager _lnm = new LocalNotificationsManager();

			private readonly Service _service;

			public TimedReminderCountdownTimer(Service self, long millisInFuture, long countDownInterval)
				: base(millisInFuture, countDownInterval)
			{
				_service = self;
			}

			public TimedReminderCountdownTimer(IntPtr javaReference, JniHandleOwnership transfer)
				: base(javaReference, transfer)
			{
			}

			public TimedReminderCountdownTimer(long millisInFuture, long countDownInterval)
				: base(millisInFuture, countDownInterval)
			{
			}

			public override void OnFinish()
			{
				_lnm.GenerateLocalNotification(NotificationsEnum.TimedReminderFinished.Data(), 0L);
				_service.StopForeground(removeNotification: true);
				_service.StopSelf();
			}

			public override void OnTick(long millisUntilFinished)
			{
				_lnm.GenerateDelayedNotification(NotificationsEnum.TimedReminder.Data(), millisUntilFinished);
			}
		}

		private TimedReminderCountdownTimer _timedReminderCountdownTimer;

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			if (intent.GetBooleanExtra("close", defaultValue: false))
			{
				StopForeground(removeNotification: true);
				StopSelf();
				return base.OnStartCommand(intent, flags, startId);
			}
			long longExtra = intent.GetLongExtra("ticks", 0L);
			Notification notification = new LocalNotificationsManager().CreateNotificationWithExtraLongData(NotificationsEnum.TimedReminder.Data(), longExtra);
			StartForeground(9, notification);
			try
			{
				_timedReminderCountdownTimer = new TimedReminderCountdownTimer(this, longExtra, 1000L);
				_timedReminderCountdownTimer.Start();
			}
			catch (System.Exception e)
			{
				LogUtils.LogException(LogSeverity.WARNING, e, "TimedReminderForegroundService-StartCommandResult: Timer exception");
			}
			return base.OnStartCommand(intent, flags, startId);
		}

		public override void OnDestroy()
		{
			_timedReminderCountdownTimer.Cancel();
			if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
			{
				StopForeground(StopForegroundFlags.Remove);
			}
			else
			{
				StopForeground(removeNotification: true);
			}
			StopSelf();
			base.OnDestroy();
		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}
	}
	public class DroidDialogService : IDialogService
	{
		public void ShowMessageDialog(string title, string message, string okBtn, PlatformDialogServiceArguments platformArguments = null)
		{
			Activity current;
			if (platformArguments != null && platformArguments.Context != null)
			{
				Activity activity = platformArguments.Context as Activity;
				if (activity != null)
				{
					current = activity;
					goto IL_002a;
				}
			}
			current = CrossCurrentActivity.Current.Activity;
			goto IL_002a;
			IL_002a:
			DialogUtils.DisplayDialog(current, title, message, okBtn);
		}
	}
	internal class DroidApiDataHelperHandler : IApiDataHelper
	{
		public bool IsGoogleServiceEnabled()
		{
			return GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(CrossCurrentActivity.Current.AppContext) == 0;
		}

		public string GetBackGroundServicVersionLogString()
		{
			return " (GPS: " + GetBackGroudServiceVersion() + ")";
		}

		public string GetBackGroudServiceVersion()
		{
			string result = "";
			try
			{
				result = PackageInfoCompat.GetLongVersionCode(CrossCurrentActivity.Current.AppContext.PackageManager.GetPackageInfo("com.google.android.gms", (PackageInfoFlags)0)).ToString();
				return result;
			}
			catch (Java.Lang.Exception)
			{
				return result;
			}
		}
	}
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new string[] { "android.bluetooth.adapter.action.STATE_CHANGED", "android.location.PROVIDERS_CHANGED" })]
	internal class PermissionsBroadcastReceiver : BroadcastReceiver
	{
		private readonly PermissionUtils _permissionsUtils = ServiceLocator.Current.GetInstance<PermissionUtils>();

		public override void OnReceive(Context context, Intent intent)
		{
			if (BluetoothAdapter.DefaultAdapter != null)
			{
				string action = intent.Action;
				if (action.Equals("android.location.PROVIDERS_CHANGED") || (action.Equals("android.bluetooth.adapter.action.STATE_CHANGED") && intent.GetIntExtra("android.bluetooth.adapter.extra.STATE", -2147483648) == 10))
				{
					NotifyAboutPermissionsChange();
				}
			}
		}

		private async void NotifyAboutPermissionsChange()
		{
			bool isEnabled;
			try
			{
				isEnabled = await ExposureNotification.IsEnabledAsync();
			}
			catch (System.Exception e)
			{
				if (e.HandleExposureNotificationException("PermissionsBroadcastReceiver", "NotifyAboutPermissionsChange"))
				{
				}
				isEnabled = false;
			}
			if (!(await _permissionsUtils.HasPermissionsWithoutDialogs()) && isEnabled)
			{
				PermissionsMessagingCenter.NotifyPermissionsChanged(this);
			}
		}
	}
}
