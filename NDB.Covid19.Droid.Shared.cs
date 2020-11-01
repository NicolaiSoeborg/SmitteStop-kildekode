using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Text.Util;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Text;
using CommonServiceLocator;
using Java.Util.Regex;
using NDB.Covid19.Configuration;
using NDB.Covid19.Droid.Shared.Utils;
using NDB.Covid19.Droid.Shared.Utils.Navigation;
using NDB.Covid19.Droid.Shared.Views.Settings;
using NDB.Covid19.HardwareServices.SupportServices;
using NDB.Covid19.Utils;
using NDB.Covid19.ViewModels;
using NDB.Covid19.WebServices.ErrorHandlers;
using Plugin.CurrentActivity;
using RSG;
using Unity;
using Unity.ServiceLocation;
using Xamarin.Essentials;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: ResourceDesigner("NDB.Covid19.Droid.Shared.Resource", IsApplication = false)]
[assembly: AssemblyTitle("NDB.Covid19.Droid.Shared")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("NDB.Covid19.Droid.Shared")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: TargetFramework("MonoAndroid,Version=v10.0", FrameworkDisplayName = "Xamarin.Android v10.0 Support")]
[assembly: AssemblyVersion("1.0.0.0")]
namespace NDB.Covid19.Droid.Shared
{
	[GeneratedCode("Xamarin.Android.Build.Tasks", "1.0.0.0")]
	public class Resource
	{
		public class Animation
		{
			public static int abc_fade_in;

			public static int abc_fade_out;

			public static int abc_grow_fade_in_from_bottom;

			public static int abc_popup_enter;

			public static int abc_popup_exit;

			public static int abc_shrink_fade_out_from_bottom;

			public static int abc_slide_in_bottom;

			public static int abc_slide_in_top;

			public static int abc_slide_out_bottom;

			public static int abc_slide_out_top;

			public static int abc_tooltip_enter;

			public static int abc_tooltip_exit;

			public static int btn_checkbox_to_checked_box_inner_merged_animation;

			public static int btn_checkbox_to_checked_box_outer_merged_animation;

			public static int btn_checkbox_to_checked_icon_null_animation;

			public static int btn_checkbox_to_unchecked_box_inner_merged_animation;

			public static int btn_checkbox_to_unchecked_check_path_merged_animation;

			public static int btn_checkbox_to_unchecked_icon_null_animation;

			public static int btn_radio_to_off_mtrl_dot_group_animation;

			public static int btn_radio_to_off_mtrl_ring_outer_animation;

			public static int btn_radio_to_off_mtrl_ring_outer_path_animation;

			public static int btn_radio_to_on_mtrl_dot_group_animation;

			public static int btn_radio_to_on_mtrl_ring_outer_animation;

			public static int btn_radio_to_on_mtrl_ring_outer_path_animation;

			public static int design_bottom_sheet_slide_in;

			public static int design_bottom_sheet_slide_out;

			public static int design_snackbar_in;

			public static int design_snackbar_out;

			public static int fragment_close_enter;

			public static int fragment_close_exit;

			public static int fragment_fade_enter;

			public static int fragment_fade_exit;

			public static int fragment_fast_out_extra_slow_in;

			public static int fragment_open_enter;

			public static int fragment_open_exit;

			public static int mtrl_bottom_sheet_slide_in;

			public static int mtrl_bottom_sheet_slide_out;

			public static int mtrl_card_lowers_interpolator;

			static Animation()
			{
				abc_fade_in = 2130771968;
				abc_fade_out = 2130771969;
				abc_grow_fade_in_from_bottom = 2130771970;
				abc_popup_enter = 2130771971;
				abc_popup_exit = 2130771972;
				abc_shrink_fade_out_from_bottom = 2130771973;
				abc_slide_in_bottom = 2130771974;
				abc_slide_in_top = 2130771975;
				abc_slide_out_bottom = 2130771976;
				abc_slide_out_top = 2130771977;
				abc_tooltip_enter = 2130771978;
				abc_tooltip_exit = 2130771979;
				btn_checkbox_to_checked_box_inner_merged_animation = 2130771980;
				btn_checkbox_to_checked_box_outer_merged_animation = 2130771981;
				btn_checkbox_to_checked_icon_null_animation = 2130771982;
				btn_checkbox_to_unchecked_box_inner_merged_animation = 2130771983;
				btn_checkbox_to_unchecked_check_path_merged_animation = 2130771984;
				btn_checkbox_to_unchecked_icon_null_animation = 2130771985;
				btn_radio_to_off_mtrl_dot_group_animation = 2130771986;
				btn_radio_to_off_mtrl_ring_outer_animation = 2130771987;
				btn_radio_to_off_mtrl_ring_outer_path_animation = 2130771988;
				btn_radio_to_on_mtrl_dot_group_animation = 2130771989;
				btn_radio_to_on_mtrl_ring_outer_animation = 2130771990;
				btn_radio_to_on_mtrl_ring_outer_path_animation = 2130771991;
				design_bottom_sheet_slide_in = 2130771992;
				design_bottom_sheet_slide_out = 2130771993;
				design_snackbar_in = 2130771994;
				design_snackbar_out = 2130771995;
				fragment_close_enter = 2130771996;
				fragment_close_exit = 2130771997;
				fragment_fade_enter = 2130771998;
				fragment_fade_exit = 2130771999;
				fragment_fast_out_extra_slow_in = 2130772000;
				fragment_open_enter = 2130772001;
				fragment_open_exit = 2130772002;
				mtrl_bottom_sheet_slide_in = 2130772003;
				mtrl_bottom_sheet_slide_out = 2130772004;
				mtrl_card_lowers_interpolator = 2130772005;
				ResourceIdManager.UpdateIdValues();
			}

			private Animation()
			{
			}
		}

		public class Animator
		{
			public static int design_appbar_state_list_animator;

			public static int design_fab_hide_motion_spec;

			public static int design_fab_show_motion_spec;

			public static int mtrl_btn_state_list_anim;

			public static int mtrl_btn_unelevated_state_list_anim;

			public static int mtrl_card_state_list_anim;

			public static int mtrl_chip_state_list_anim;

			public static int mtrl_extended_fab_change_size_motion_spec;

			public static int mtrl_extended_fab_hide_motion_spec;

			public static int mtrl_extended_fab_show_motion_spec;

			public static int mtrl_extended_fab_state_list_animator;

			public static int mtrl_fab_hide_motion_spec;

			public static int mtrl_fab_show_motion_spec;

			public static int mtrl_fab_transformation_sheet_collapse_spec;

			public static int mtrl_fab_transformation_sheet_expand_spec;

			static Animator()
			{
				design_appbar_state_list_animator = 2130837504;
				design_fab_hide_motion_spec = 2130837505;
				design_fab_show_motion_spec = 2130837506;
				mtrl_btn_state_list_anim = 2130837507;
				mtrl_btn_unelevated_state_list_anim = 2130837508;
				mtrl_card_state_list_anim = 2130837509;
				mtrl_chip_state_list_anim = 2130837510;
				mtrl_extended_fab_change_size_motion_spec = 2130837511;
				mtrl_extended_fab_hide_motion_spec = 2130837512;
				mtrl_extended_fab_show_motion_spec = 2130837513;
				mtrl_extended_fab_state_list_animator = 2130837514;
				mtrl_fab_hide_motion_spec = 2130837515;
				mtrl_fab_show_motion_spec = 2130837516;
				mtrl_fab_transformation_sheet_collapse_spec = 2130837517;
				mtrl_fab_transformation_sheet_expand_spec = 2130837518;
				ResourceIdManager.UpdateIdValues();
			}

			private Animator()
			{
			}
		}

		public class Attribute
		{
			public static int actionBarDivider;

			public static int actionBarItemBackground;

			public static int actionBarPopupTheme;

			public static int actionBarSize;

			public static int actionBarSplitStyle;

			public static int actionBarStyle;

			public static int actionBarTabBarStyle;

			public static int actionBarTabStyle;

			public static int actionBarTabTextStyle;

			public static int actionBarTheme;

			public static int actionBarWidgetTheme;

			public static int actionButtonStyle;

			public static int actionDropDownStyle;

			public static int actionLayout;

			public static int actionMenuTextAppearance;

			public static int actionMenuTextColor;

			public static int actionModeBackground;

			public static int actionModeCloseButtonStyle;

			public static int actionModeCloseDrawable;

			public static int actionModeCopyDrawable;

			public static int actionModeCutDrawable;

			public static int actionModeFindDrawable;

			public static int actionModePasteDrawable;

			public static int actionModePopupWindowStyle;

			public static int actionModeSelectAllDrawable;

			public static int actionModeShareDrawable;

			public static int actionModeSplitBackground;

			public static int actionModeStyle;

			public static int actionModeWebSearchDrawable;

			public static int actionOverflowButtonStyle;

			public static int actionOverflowMenuStyle;

			public static int actionProviderClass;

			public static int actionTextColorAlpha;

			public static int actionViewClass;

			public static int activityChooserViewStyle;

			public static int alertDialogButtonGroupStyle;

			public static int alertDialogCenterButtons;

			public static int alertDialogStyle;

			public static int alertDialogTheme;

			public static int allowStacking;

			public static int alpha;

			public static int alphabeticModifiers;

			public static int animationMode;

			public static int appBarLayoutStyle;

			public static int arrowHeadLength;

			public static int arrowShaftLength;

			public static int autoCompleteTextViewStyle;

			public static int autoSizeMaxTextSize;

			public static int autoSizeMinTextSize;

			public static int autoSizePresetSizes;

			public static int autoSizeStepGranularity;

			public static int autoSizeTextType;

			public static int background;

			public static int backgroundColor;

			public static int backgroundInsetBottom;

			public static int backgroundInsetEnd;

			public static int backgroundInsetStart;

			public static int backgroundInsetTop;

			public static int backgroundOverlayColorAlpha;

			public static int backgroundSplit;

			public static int backgroundStacked;

			public static int backgroundTint;

			public static int backgroundTintMode;

			public static int badgeGravity;

			public static int badgeStyle;

			public static int badgeTextColor;

			public static int barLength;

			public static int barrierAllowsGoneWidgets;

			public static int barrierDirection;

			public static int behavior_autoHide;

			public static int behavior_autoShrink;

			public static int behavior_expandedOffset;

			public static int behavior_fitToContents;

			public static int behavior_halfExpandedRatio;

			public static int behavior_hideable;

			public static int behavior_overlapTop;

			public static int behavior_peekHeight;

			public static int behavior_saveFlags;

			public static int behavior_skipCollapsed;

			public static int borderlessButtonStyle;

			public static int borderWidth;

			public static int bottomAppBarStyle;

			public static int bottomNavigationStyle;

			public static int bottomSheetDialogTheme;

			public static int bottomSheetStyle;

			public static int boxBackgroundColor;

			public static int boxBackgroundMode;

			public static int boxCollapsedPaddingTop;

			public static int boxCornerRadiusBottomEnd;

			public static int boxCornerRadiusBottomStart;

			public static int boxCornerRadiusTopEnd;

			public static int boxCornerRadiusTopStart;

			public static int boxStrokeColor;

			public static int boxStrokeWidth;

			public static int boxStrokeWidthFocused;

			public static int buttonBarButtonStyle;

			public static int buttonBarNegativeButtonStyle;

			public static int buttonBarNeutralButtonStyle;

			public static int buttonBarPositiveButtonStyle;

			public static int buttonBarStyle;

			public static int buttonCompat;

			public static int buttonGravity;

			public static int buttonIconDimen;

			public static int buttonPanelSideLayout;

			public static int buttonSize;

			public static int buttonStyle;

			public static int buttonStyleSmall;

			public static int buttonTint;

			public static int buttonTintMode;

			public static int cardBackgroundColor;

			public static int cardCornerRadius;

			public static int cardElevation;

			public static int cardForegroundColor;

			public static int cardMaxElevation;

			public static int cardPreventCornerOverlap;

			public static int cardUseCompatPadding;

			public static int cardViewStyle;

			public static int chainUseRtl;

			public static int checkboxStyle;

			public static int checkedButton;

			public static int checkedChip;

			public static int checkedIcon;

			public static int checkedIconEnabled;

			public static int checkedIconTint;

			public static int checkedIconVisible;

			public static int checkedTextViewStyle;

			public static int chipBackgroundColor;

			public static int chipCornerRadius;

			public static int chipEndPadding;

			public static int chipGroupStyle;

			public static int chipIcon;

			public static int chipIconEnabled;

			public static int chipIconSize;

			public static int chipIconTint;

			public static int chipIconVisible;

			public static int chipMinHeight;

			public static int chipMinTouchTargetSize;

			public static int chipSpacing;

			public static int chipSpacingHorizontal;

			public static int chipSpacingVertical;

			public static int chipStandaloneStyle;

			public static int chipStartPadding;

			public static int chipStrokeColor;

			public static int chipStrokeWidth;

			public static int chipStyle;

			public static int chipSurfaceColor;

			public static int circleCrop;

			public static int closeIcon;

			public static int closeIconEnabled;

			public static int closeIconEndPadding;

			public static int closeIconSize;

			public static int closeIconStartPadding;

			public static int closeIconTint;

			public static int closeIconVisible;

			public static int closeItemLayout;

			public static int collapseContentDescription;

			public static int collapsedTitleGravity;

			public static int collapsedTitleTextAppearance;

			public static int collapseIcon;

			public static int color;

			public static int colorAccent;

			public static int colorBackgroundFloating;

			public static int colorButtonNormal;

			public static int colorControlActivated;

			public static int colorControlHighlight;

			public static int colorControlNormal;

			public static int colorError;

			public static int colorOnBackground;

			public static int colorOnError;

			public static int colorOnPrimary;

			public static int colorOnPrimarySurface;

			public static int colorOnSecondary;

			public static int colorOnSurface;

			public static int colorPrimary;

			public static int colorPrimaryDark;

			public static int colorPrimarySurface;

			public static int colorPrimaryVariant;

			public static int colorScheme;

			public static int colorSecondary;

			public static int colorSecondaryVariant;

			public static int colorSurface;

			public static int colorSwitchThumbNormal;

			public static int commitIcon;

			public static int constraintSet;

			public static int constraint_referenced_ids;

			public static int content;

			public static int contentDescription;

			public static int contentInsetEnd;

			public static int contentInsetEndWithActions;

			public static int contentInsetLeft;

			public static int contentInsetRight;

			public static int contentInsetStart;

			public static int contentInsetStartWithNavigation;

			public static int contentPadding;

			public static int contentPaddingBottom;

			public static int contentPaddingLeft;

			public static int contentPaddingRight;

			public static int contentPaddingTop;

			public static int contentScrim;

			public static int controlBackground;

			public static int coordinatorLayoutStyle;

			public static int cornerFamily;

			public static int cornerFamilyBottomLeft;

			public static int cornerFamilyBottomRight;

			public static int cornerFamilyTopLeft;

			public static int cornerFamilyTopRight;

			public static int cornerRadius;

			public static int cornerSize;

			public static int cornerSizeBottomLeft;

			public static int cornerSizeBottomRight;

			public static int cornerSizeTopLeft;

			public static int cornerSizeTopRight;

			public static int counterEnabled;

			public static int counterMaxLength;

			public static int counterOverflowTextAppearance;

			public static int counterOverflowTextColor;

			public static int counterTextAppearance;

			public static int counterTextColor;

			public static int customNavigationLayout;

			public static int dayInvalidStyle;

			public static int daySelectedStyle;

			public static int dayStyle;

			public static int dayTodayStyle;

			public static int defaultQueryHint;

			public static int dialogCornerRadius;

			public static int dialogPreferredPadding;

			public static int dialogTheme;

			public static int displayOptions;

			public static int divider;

			public static int dividerHorizontal;

			public static int dividerPadding;

			public static int dividerVertical;

			public static int drawableBottomCompat;

			public static int drawableEndCompat;

			public static int drawableLeftCompat;

			public static int drawableRightCompat;

			public static int drawableSize;

			public static int drawableStartCompat;

			public static int drawableTint;

			public static int drawableTintMode;

			public static int drawableTopCompat;

			public static int drawerArrowStyle;

			public static int dropdownListPreferredItemHeight;

			public static int dropDownListViewStyle;

			public static int editTextBackground;

			public static int editTextColor;

			public static int editTextStyle;

			public static int elevation;

			public static int elevationOverlayColor;

			public static int elevationOverlayEnabled;

			public static int emptyVisibility;

			public static int endIconCheckable;

			public static int endIconContentDescription;

			public static int endIconDrawable;

			public static int endIconMode;

			public static int endIconTint;

			public static int endIconTintMode;

			public static int enforceMaterialTheme;

			public static int enforceTextAppearance;

			public static int ensureMinTouchTargetSize;

			public static int errorEnabled;

			public static int errorIconDrawable;

			public static int errorIconTint;

			public static int errorIconTintMode;

			public static int errorTextAppearance;

			public static int errorTextColor;

			public static int expandActivityOverflowButtonDrawable;

			public static int expanded;

			public static int expandedTitleGravity;

			public static int expandedTitleMargin;

			public static int expandedTitleMarginBottom;

			public static int expandedTitleMarginEnd;

			public static int expandedTitleMarginStart;

			public static int expandedTitleMarginTop;

			public static int expandedTitleTextAppearance;

			public static int extendedFloatingActionButtonStyle;

			public static int extendMotionSpec;

			public static int fabAlignmentMode;

			public static int fabAnimationMode;

			public static int fabCradleMargin;

			public static int fabCradleRoundedCornerRadius;

			public static int fabCradleVerticalOffset;

			public static int fabCustomSize;

			public static int fabSize;

			public static int fastScrollEnabled;

			public static int fastScrollHorizontalThumbDrawable;

			public static int fastScrollHorizontalTrackDrawable;

			public static int fastScrollVerticalThumbDrawable;

			public static int fastScrollVerticalTrackDrawable;

			public static int firstBaselineToTopHeight;

			public static int floatingActionButtonStyle;

			public static int font;

			public static int fontFamily;

			public static int fontProviderAuthority;

			public static int fontProviderCerts;

			public static int fontProviderFetchStrategy;

			public static int fontProviderFetchTimeout;

			public static int fontProviderPackage;

			public static int fontProviderQuery;

			public static int fontStyle;

			public static int fontVariationSettings;

			public static int fontWeight;

			public static int foregroundInsidePadding;

			public static int gapBetweenBars;

			public static int goIcon;

			public static int headerLayout;

			public static int height;

			public static int helperText;

			public static int helperTextEnabled;

			public static int helperTextTextAppearance;

			public static int helperTextTextColor;

			public static int hideMotionSpec;

			public static int hideOnContentScroll;

			public static int hideOnScroll;

			public static int hintAnimationEnabled;

			public static int hintEnabled;

			public static int hintTextAppearance;

			public static int hintTextColor;

			public static int homeAsUpIndicator;

			public static int homeLayout;

			public static int hoveredFocusedTranslationZ;

			public static int icon;

			public static int iconEndPadding;

			public static int iconGravity;

			public static int iconifiedByDefault;

			public static int iconPadding;

			public static int iconSize;

			public static int iconStartPadding;

			public static int iconTint;

			public static int iconTintMode;

			public static int imageAspectRatio;

			public static int imageAspectRatioAdjust;

			public static int imageButtonStyle;

			public static int indeterminateProgressStyle;

			public static int initialActivityCount;

			public static int insetForeground;

			public static int isLightTheme;

			public static int isMaterialTheme;

			public static int itemBackground;

			public static int itemFillColor;

			public static int itemHorizontalPadding;

			public static int itemHorizontalTranslationEnabled;

			public static int itemIconPadding;

			public static int itemIconSize;

			public static int itemIconTint;

			public static int itemMaxLines;

			public static int itemPadding;

			public static int itemRippleColor;

			public static int itemShapeAppearance;

			public static int itemShapeAppearanceOverlay;

			public static int itemShapeFillColor;

			public static int itemShapeInsetBottom;

			public static int itemShapeInsetEnd;

			public static int itemShapeInsetStart;

			public static int itemShapeInsetTop;

			public static int itemSpacing;

			public static int itemStrokeColor;

			public static int itemStrokeWidth;

			public static int itemTextAppearance;

			public static int itemTextAppearanceActive;

			public static int itemTextAppearanceInactive;

			public static int itemTextColor;

			public static int keylines;

			public static int labelVisibilityMode;

			public static int lastBaselineToBottomHeight;

			public static int layout;

			public static int layoutManager;

			public static int layout_anchor;

			public static int layout_anchorGravity;

			public static int layout_behavior;

			public static int layout_collapseMode;

			public static int layout_collapseParallaxMultiplier;

			public static int layout_constrainedHeight;

			public static int layout_constrainedWidth;

			public static int layout_constraintBaseline_creator;

			public static int layout_constraintBaseline_toBaselineOf;

			public static int layout_constraintBottom_creator;

			public static int layout_constraintBottom_toBottomOf;

			public static int layout_constraintBottom_toTopOf;

			public static int layout_constraintCircle;

			public static int layout_constraintCircleAngle;

			public static int layout_constraintCircleRadius;

			public static int layout_constraintDimensionRatio;

			public static int layout_constraintEnd_toEndOf;

			public static int layout_constraintEnd_toStartOf;

			public static int layout_constraintGuide_begin;

			public static int layout_constraintGuide_end;

			public static int layout_constraintGuide_percent;

			public static int layout_constraintHeight_default;

			public static int layout_constraintHeight_max;

			public static int layout_constraintHeight_min;

			public static int layout_constraintHeight_percent;

			public static int layout_constraintHorizontal_bias;

			public static int layout_constraintHorizontal_chainStyle;

			public static int layout_constraintHorizontal_weight;

			public static int layout_constraintLeft_creator;

			public static int layout_constraintLeft_toLeftOf;

			public static int layout_constraintLeft_toRightOf;

			public static int layout_constraintRight_creator;

			public static int layout_constraintRight_toLeftOf;

			public static int layout_constraintRight_toRightOf;

			public static int layout_constraintStart_toEndOf;

			public static int layout_constraintStart_toStartOf;

			public static int layout_constraintTop_creator;

			public static int layout_constraintTop_toBottomOf;

			public static int layout_constraintTop_toTopOf;

			public static int layout_constraintVertical_bias;

			public static int layout_constraintVertical_chainStyle;

			public static int layout_constraintVertical_weight;

			public static int layout_constraintWidth_default;

			public static int layout_constraintWidth_max;

			public static int layout_constraintWidth_min;

			public static int layout_constraintWidth_percent;

			public static int layout_dodgeInsetEdges;

			public static int layout_editor_absoluteX;

			public static int layout_editor_absoluteY;

			public static int layout_goneMarginBottom;

			public static int layout_goneMarginEnd;

			public static int layout_goneMarginLeft;

			public static int layout_goneMarginRight;

			public static int layout_goneMarginStart;

			public static int layout_goneMarginTop;

			public static int layout_insetEdge;

			public static int layout_keyline;

			public static int layout_optimizationLevel;

			public static int layout_scrollFlags;

			public static int layout_scrollInterpolator;

			public static int liftOnScroll;

			public static int liftOnScrollTargetViewId;

			public static int lineHeight;

			public static int lineSpacing;

			public static int listChoiceBackgroundIndicator;

			public static int listChoiceIndicatorMultipleAnimated;

			public static int listChoiceIndicatorSingleAnimated;

			public static int listDividerAlertDialog;

			public static int listItemLayout;

			public static int listLayout;

			public static int listMenuViewStyle;

			public static int listPopupWindowStyle;

			public static int listPreferredItemHeight;

			public static int listPreferredItemHeightLarge;

			public static int listPreferredItemHeightSmall;

			public static int listPreferredItemPaddingEnd;

			public static int listPreferredItemPaddingLeft;

			public static int listPreferredItemPaddingRight;

			public static int listPreferredItemPaddingStart;

			public static int logo;

			public static int logoDescription;

			public static int materialAlertDialogBodyTextStyle;

			public static int materialAlertDialogTheme;

			public static int materialAlertDialogTitleIconStyle;

			public static int materialAlertDialogTitlePanelStyle;

			public static int materialAlertDialogTitleTextStyle;

			public static int materialButtonOutlinedStyle;

			public static int materialButtonStyle;

			public static int materialButtonToggleGroupStyle;

			public static int materialCalendarDay;

			public static int materialCalendarFullscreenTheme;

			public static int materialCalendarHeaderConfirmButton;

			public static int materialCalendarHeaderDivider;

			public static int materialCalendarHeaderLayout;

			public static int materialCalendarHeaderSelection;

			public static int materialCalendarHeaderTitle;

			public static int materialCalendarHeaderToggleButton;

			public static int materialCalendarStyle;

			public static int materialCalendarTheme;

			public static int materialCardViewStyle;

			public static int materialThemeOverlay;

			public static int maxActionInlineWidth;

			public static int maxButtonHeight;

			public static int maxCharacterCount;

			public static int maxImageSize;

			public static int measureWithLargestChild;

			public static int menu;

			public static int minTouchTargetSize;

			public static int multiChoiceItemLayout;

			public static int navigationContentDescription;

			public static int navigationIcon;

			public static int navigationMode;

			public static int navigationViewStyle;

			public static int number;

			public static int numericModifiers;

			public static int overlapAnchor;

			public static int paddingBottomNoButtons;

			public static int paddingEnd;

			public static int paddingStart;

			public static int paddingTopNoTitle;

			public static int panelBackground;

			public static int panelMenuListTheme;

			public static int panelMenuListWidth;

			public static int passwordToggleContentDescription;

			public static int passwordToggleDrawable;

			public static int passwordToggleEnabled;

			public static int passwordToggleTint;

			public static int passwordToggleTintMode;

			public static int popupMenuBackground;

			public static int popupMenuStyle;

			public static int popupTheme;

			public static int popupWindowStyle;

			public static int preserveIconSpacing;

			public static int pressedTranslationZ;

			public static int progressBarPadding;

			public static int progressBarStyle;

			public static int queryBackground;

			public static int queryHint;

			public static int radioButtonStyle;

			public static int rangeFillColor;

			public static int ratingBarStyle;

			public static int ratingBarStyleIndicator;

			public static int ratingBarStyleSmall;

			public static int recyclerViewStyle;

			public static int reverseLayout;

			public static int rippleColor;

			public static int scopeUris;

			public static int scrimAnimationDuration;

			public static int scrimBackground;

			public static int scrimVisibleHeightTrigger;

			public static int searchHintIcon;

			public static int searchIcon;

			public static int searchViewStyle;

			public static int seekBarStyle;

			public static int selectableItemBackground;

			public static int selectableItemBackgroundBorderless;

			public static int shapeAppearance;

			public static int shapeAppearanceLargeComponent;

			public static int shapeAppearanceMediumComponent;

			public static int shapeAppearanceOverlay;

			public static int shapeAppearanceSmallComponent;

			public static int showAsAction;

			public static int showDividers;

			public static int showMotionSpec;

			public static int showText;

			public static int showTitle;

			public static int shrinkMotionSpec;

			public static int singleChoiceItemLayout;

			public static int singleLine;

			public static int singleSelection;

			public static int snackbarButtonStyle;

			public static int snackbarStyle;

			public static int spanCount;

			public static int spinBars;

			public static int spinnerDropDownItemStyle;

			public static int spinnerStyle;

			public static int splitTrack;

			public static int srcCompat;

			public static int stackFromEnd;

			public static int startIconCheckable;

			public static int startIconContentDescription;

			public static int startIconDrawable;

			public static int startIconTint;

			public static int startIconTintMode;

			public static int state_above_anchor;

			public static int state_collapsed;

			public static int state_collapsible;

			public static int state_dragged;

			public static int state_liftable;

			public static int state_lifted;

			public static int statusBarBackground;

			public static int statusBarForeground;

			public static int statusBarScrim;

			public static int strokeColor;

			public static int strokeWidth;

			public static int subMenuArrow;

			public static int submitBackground;

			public static int subtitle;

			public static int subtitleTextAppearance;

			public static int subtitleTextColor;

			public static int subtitleTextStyle;

			public static int suggestionRowLayout;

			public static int switchMinWidth;

			public static int switchPadding;

			public static int switchStyle;

			public static int switchTextAppearance;

			public static int tabBackground;

			public static int tabContentStart;

			public static int tabGravity;

			public static int tabIconTint;

			public static int tabIconTintMode;

			public static int tabIndicator;

			public static int tabIndicatorAnimationDuration;

			public static int tabIndicatorColor;

			public static int tabIndicatorFullWidth;

			public static int tabIndicatorGravity;

			public static int tabIndicatorHeight;

			public static int tabInlineLabel;

			public static int tabMaxWidth;

			public static int tabMinWidth;

			public static int tabMode;

			public static int tabPadding;

			public static int tabPaddingBottom;

			public static int tabPaddingEnd;

			public static int tabPaddingStart;

			public static int tabPaddingTop;

			public static int tabRippleColor;

			public static int tabSelectedTextColor;

			public static int tabStyle;

			public static int tabTextAppearance;

			public static int tabTextColor;

			public static int tabUnboundedRipple;

			public static int textAllCaps;

			public static int textAppearanceBody1;

			public static int textAppearanceBody2;

			public static int textAppearanceButton;

			public static int textAppearanceCaption;

			public static int textAppearanceHeadline1;

			public static int textAppearanceHeadline2;

			public static int textAppearanceHeadline3;

			public static int textAppearanceHeadline4;

			public static int textAppearanceHeadline5;

			public static int textAppearanceHeadline6;

			public static int textAppearanceLargePopupMenu;

			public static int textAppearanceLineHeightEnabled;

			public static int textAppearanceListItem;

			public static int textAppearanceListItemSecondary;

			public static int textAppearanceListItemSmall;

			public static int textAppearanceOverline;

			public static int textAppearancePopupMenuHeader;

			public static int textAppearanceSearchResultSubtitle;

			public static int textAppearanceSearchResultTitle;

			public static int textAppearanceSmallPopupMenu;

			public static int textAppearanceSubtitle1;

			public static int textAppearanceSubtitle2;

			public static int textColorAlertDialogListItem;

			public static int textColorSearchUrl;

			public static int textEndPadding;

			public static int textInputStyle;

			public static int textLocale;

			public static int textStartPadding;

			public static int theme;

			public static int themeLineHeight;

			public static int thickness;

			public static int thumbTextPadding;

			public static int thumbTint;

			public static int thumbTintMode;

			public static int tickMark;

			public static int tickMarkTint;

			public static int tickMarkTintMode;

			public static int tint;

			public static int tintMode;

			public static int title;

			public static int titleEnabled;

			public static int titleMargin;

			public static int titleMarginBottom;

			public static int titleMarginEnd;

			public static int titleMargins;

			public static int titleMarginStart;

			public static int titleMarginTop;

			public static int titleTextAppearance;

			public static int titleTextColor;

			public static int titleTextStyle;

			public static int toolbarId;

			public static int toolbarNavigationButtonStyle;

			public static int toolbarStyle;

			public static int tooltipForegroundColor;

			public static int tooltipFrameBackground;

			public static int tooltipText;

			public static int track;

			public static int trackTint;

			public static int trackTintMode;

			public static int ttcIndex;

			public static int useCompatPadding;

			public static int useMaterialThemeColors;

			public static int viewInflaterClass;

			public static int voiceIcon;

			public static int windowActionBar;

			public static int windowActionBarOverlay;

			public static int windowActionModeOverlay;

			public static int windowFixedHeightMajor;

			public static int windowFixedHeightMinor;

			public static int windowFixedWidthMajor;

			public static int windowFixedWidthMinor;

			public static int windowMinWidthMajor;

			public static int windowMinWidthMinor;

			public static int windowNoTitle;

			public static int yearSelectedStyle;

			public static int yearStyle;

			public static int yearTodayStyle;

			static Attribute()
			{
				actionBarDivider = 2130903040;
				actionBarItemBackground = 2130903041;
				actionBarPopupTheme = 2130903042;
				actionBarSize = 2130903043;
				actionBarSplitStyle = 2130903044;
				actionBarStyle = 2130903045;
				actionBarTabBarStyle = 2130903046;
				actionBarTabStyle = 2130903047;
				actionBarTabTextStyle = 2130903048;
				actionBarTheme = 2130903049;
				actionBarWidgetTheme = 2130903050;
				actionButtonStyle = 2130903051;
				actionDropDownStyle = 2130903052;
				actionLayout = 2130903053;
				actionMenuTextAppearance = 2130903054;
				actionMenuTextColor = 2130903055;
				actionModeBackground = 2130903056;
				actionModeCloseButtonStyle = 2130903057;
				actionModeCloseDrawable = 2130903058;
				actionModeCopyDrawable = 2130903059;
				actionModeCutDrawable = 2130903060;
				actionModeFindDrawable = 2130903061;
				actionModePasteDrawable = 2130903062;
				actionModePopupWindowStyle = 2130903063;
				actionModeSelectAllDrawable = 2130903064;
				actionModeShareDrawable = 2130903065;
				actionModeSplitBackground = 2130903066;
				actionModeStyle = 2130903067;
				actionModeWebSearchDrawable = 2130903068;
				actionOverflowButtonStyle = 2130903069;
				actionOverflowMenuStyle = 2130903070;
				actionProviderClass = 2130903071;
				actionTextColorAlpha = 2130903072;
				actionViewClass = 2130903073;
				activityChooserViewStyle = 2130903074;
				alertDialogButtonGroupStyle = 2130903075;
				alertDialogCenterButtons = 2130903076;
				alertDialogStyle = 2130903077;
				alertDialogTheme = 2130903078;
				allowStacking = 2130903079;
				alpha = 2130903080;
				alphabeticModifiers = 2130903081;
				animationMode = 2130903082;
				appBarLayoutStyle = 2130903083;
				arrowHeadLength = 2130903084;
				arrowShaftLength = 2130903085;
				autoCompleteTextViewStyle = 2130903086;
				autoSizeMaxTextSize = 2130903087;
				autoSizeMinTextSize = 2130903088;
				autoSizePresetSizes = 2130903089;
				autoSizeStepGranularity = 2130903090;
				autoSizeTextType = 2130903091;
				background = 2130903092;
				backgroundColor = 2130903093;
				backgroundInsetBottom = 2130903094;
				backgroundInsetEnd = 2130903095;
				backgroundInsetStart = 2130903096;
				backgroundInsetTop = 2130903097;
				backgroundOverlayColorAlpha = 2130903098;
				backgroundSplit = 2130903099;
				backgroundStacked = 2130903100;
				backgroundTint = 2130903101;
				backgroundTintMode = 2130903102;
				badgeGravity = 2130903103;
				badgeStyle = 2130903104;
				badgeTextColor = 2130903105;
				barLength = 2130903106;
				barrierAllowsGoneWidgets = 2130903107;
				barrierDirection = 2130903108;
				behavior_autoHide = 2130903109;
				behavior_autoShrink = 2130903110;
				behavior_expandedOffset = 2130903111;
				behavior_fitToContents = 2130903112;
				behavior_halfExpandedRatio = 2130903113;
				behavior_hideable = 2130903114;
				behavior_overlapTop = 2130903115;
				behavior_peekHeight = 2130903116;
				behavior_saveFlags = 2130903117;
				behavior_skipCollapsed = 2130903118;
				borderlessButtonStyle = 2130903120;
				borderWidth = 2130903119;
				bottomAppBarStyle = 2130903121;
				bottomNavigationStyle = 2130903122;
				bottomSheetDialogTheme = 2130903123;
				bottomSheetStyle = 2130903124;
				boxBackgroundColor = 2130903125;
				boxBackgroundMode = 2130903126;
				boxCollapsedPaddingTop = 2130903127;
				boxCornerRadiusBottomEnd = 2130903128;
				boxCornerRadiusBottomStart = 2130903129;
				boxCornerRadiusTopEnd = 2130903130;
				boxCornerRadiusTopStart = 2130903131;
				boxStrokeColor = 2130903132;
				boxStrokeWidth = 2130903133;
				boxStrokeWidthFocused = 2130903134;
				buttonBarButtonStyle = 2130903135;
				buttonBarNegativeButtonStyle = 2130903136;
				buttonBarNeutralButtonStyle = 2130903137;
				buttonBarPositiveButtonStyle = 2130903138;
				buttonBarStyle = 2130903139;
				buttonCompat = 2130903140;
				buttonGravity = 2130903141;
				buttonIconDimen = 2130903142;
				buttonPanelSideLayout = 2130903143;
				buttonSize = 2130903144;
				buttonStyle = 2130903145;
				buttonStyleSmall = 2130903146;
				buttonTint = 2130903147;
				buttonTintMode = 2130903148;
				cardBackgroundColor = 2130903149;
				cardCornerRadius = 2130903150;
				cardElevation = 2130903151;
				cardForegroundColor = 2130903152;
				cardMaxElevation = 2130903153;
				cardPreventCornerOverlap = 2130903154;
				cardUseCompatPadding = 2130903155;
				cardViewStyle = 2130903156;
				chainUseRtl = 2130903157;
				checkboxStyle = 2130903158;
				checkedButton = 2130903159;
				checkedChip = 2130903160;
				checkedIcon = 2130903161;
				checkedIconEnabled = 2130903162;
				checkedIconTint = 2130903163;
				checkedIconVisible = 2130903164;
				checkedTextViewStyle = 2130903165;
				chipBackgroundColor = 2130903166;
				chipCornerRadius = 2130903167;
				chipEndPadding = 2130903168;
				chipGroupStyle = 2130903169;
				chipIcon = 2130903170;
				chipIconEnabled = 2130903171;
				chipIconSize = 2130903172;
				chipIconTint = 2130903173;
				chipIconVisible = 2130903174;
				chipMinHeight = 2130903175;
				chipMinTouchTargetSize = 2130903176;
				chipSpacing = 2130903177;
				chipSpacingHorizontal = 2130903178;
				chipSpacingVertical = 2130903179;
				chipStandaloneStyle = 2130903180;
				chipStartPadding = 2130903181;
				chipStrokeColor = 2130903182;
				chipStrokeWidth = 2130903183;
				chipStyle = 2130903184;
				chipSurfaceColor = 2130903185;
				circleCrop = 2130903186;
				closeIcon = 2130903187;
				closeIconEnabled = 2130903188;
				closeIconEndPadding = 2130903189;
				closeIconSize = 2130903190;
				closeIconStartPadding = 2130903191;
				closeIconTint = 2130903192;
				closeIconVisible = 2130903193;
				closeItemLayout = 2130903194;
				collapseContentDescription = 2130903195;
				collapsedTitleGravity = 2130903197;
				collapsedTitleTextAppearance = 2130903198;
				collapseIcon = 2130903196;
				color = 2130903199;
				colorAccent = 2130903200;
				colorBackgroundFloating = 2130903201;
				colorButtonNormal = 2130903202;
				colorControlActivated = 2130903203;
				colorControlHighlight = 2130903204;
				colorControlNormal = 2130903205;
				colorError = 2130903206;
				colorOnBackground = 2130903207;
				colorOnError = 2130903208;
				colorOnPrimary = 2130903209;
				colorOnPrimarySurface = 2130903210;
				colorOnSecondary = 2130903211;
				colorOnSurface = 2130903212;
				colorPrimary = 2130903213;
				colorPrimaryDark = 2130903214;
				colorPrimarySurface = 2130903215;
				colorPrimaryVariant = 2130903216;
				colorScheme = 2130903217;
				colorSecondary = 2130903218;
				colorSecondaryVariant = 2130903219;
				colorSurface = 2130903220;
				colorSwitchThumbNormal = 2130903221;
				commitIcon = 2130903222;
				constraintSet = 2130903223;
				constraint_referenced_ids = 2130903224;
				content = 2130903225;
				contentDescription = 2130903226;
				contentInsetEnd = 2130903227;
				contentInsetEndWithActions = 2130903228;
				contentInsetLeft = 2130903229;
				contentInsetRight = 2130903230;
				contentInsetStart = 2130903231;
				contentInsetStartWithNavigation = 2130903232;
				contentPadding = 2130903233;
				contentPaddingBottom = 2130903234;
				contentPaddingLeft = 2130903235;
				contentPaddingRight = 2130903236;
				contentPaddingTop = 2130903237;
				contentScrim = 2130903238;
				controlBackground = 2130903239;
				coordinatorLayoutStyle = 2130903240;
				cornerFamily = 2130903241;
				cornerFamilyBottomLeft = 2130903242;
				cornerFamilyBottomRight = 2130903243;
				cornerFamilyTopLeft = 2130903244;
				cornerFamilyTopRight = 2130903245;
				cornerRadius = 2130903246;
				cornerSize = 2130903247;
				cornerSizeBottomLeft = 2130903248;
				cornerSizeBottomRight = 2130903249;
				cornerSizeTopLeft = 2130903250;
				cornerSizeTopRight = 2130903251;
				counterEnabled = 2130903252;
				counterMaxLength = 2130903253;
				counterOverflowTextAppearance = 2130903254;
				counterOverflowTextColor = 2130903255;
				counterTextAppearance = 2130903256;
				counterTextColor = 2130903257;
				customNavigationLayout = 2130903258;
				dayInvalidStyle = 2130903259;
				daySelectedStyle = 2130903260;
				dayStyle = 2130903261;
				dayTodayStyle = 2130903262;
				defaultQueryHint = 2130903263;
				dialogCornerRadius = 2130903264;
				dialogPreferredPadding = 2130903265;
				dialogTheme = 2130903266;
				displayOptions = 2130903267;
				divider = 2130903268;
				dividerHorizontal = 2130903269;
				dividerPadding = 2130903270;
				dividerVertical = 2130903271;
				drawableBottomCompat = 2130903272;
				drawableEndCompat = 2130903273;
				drawableLeftCompat = 2130903274;
				drawableRightCompat = 2130903275;
				drawableSize = 2130903276;
				drawableStartCompat = 2130903277;
				drawableTint = 2130903278;
				drawableTintMode = 2130903279;
				drawableTopCompat = 2130903280;
				drawerArrowStyle = 2130903281;
				dropdownListPreferredItemHeight = 2130903283;
				dropDownListViewStyle = 2130903282;
				editTextBackground = 2130903284;
				editTextColor = 2130903285;
				editTextStyle = 2130903286;
				elevation = 2130903287;
				elevationOverlayColor = 2130903288;
				elevationOverlayEnabled = 2130903289;
				emptyVisibility = 2130903290;
				endIconCheckable = 2130903291;
				endIconContentDescription = 2130903292;
				endIconDrawable = 2130903293;
				endIconMode = 2130903294;
				endIconTint = 2130903295;
				endIconTintMode = 2130903296;
				enforceMaterialTheme = 2130903297;
				enforceTextAppearance = 2130903298;
				ensureMinTouchTargetSize = 2130903299;
				errorEnabled = 2130903300;
				errorIconDrawable = 2130903301;
				errorIconTint = 2130903302;
				errorIconTintMode = 2130903303;
				errorTextAppearance = 2130903304;
				errorTextColor = 2130903305;
				expandActivityOverflowButtonDrawable = 2130903306;
				expanded = 2130903307;
				expandedTitleGravity = 2130903308;
				expandedTitleMargin = 2130903309;
				expandedTitleMarginBottom = 2130903310;
				expandedTitleMarginEnd = 2130903311;
				expandedTitleMarginStart = 2130903312;
				expandedTitleMarginTop = 2130903313;
				expandedTitleTextAppearance = 2130903314;
				extendedFloatingActionButtonStyle = 2130903316;
				extendMotionSpec = 2130903315;
				fabAlignmentMode = 2130903317;
				fabAnimationMode = 2130903318;
				fabCradleMargin = 2130903319;
				fabCradleRoundedCornerRadius = 2130903320;
				fabCradleVerticalOffset = 2130903321;
				fabCustomSize = 2130903322;
				fabSize = 2130903323;
				fastScrollEnabled = 2130903324;
				fastScrollHorizontalThumbDrawable = 2130903325;
				fastScrollHorizontalTrackDrawable = 2130903326;
				fastScrollVerticalThumbDrawable = 2130903327;
				fastScrollVerticalTrackDrawable = 2130903328;
				firstBaselineToTopHeight = 2130903329;
				floatingActionButtonStyle = 2130903330;
				font = 2130903331;
				fontFamily = 2130903332;
				fontProviderAuthority = 2130903333;
				fontProviderCerts = 2130903334;
				fontProviderFetchStrategy = 2130903335;
				fontProviderFetchTimeout = 2130903336;
				fontProviderPackage = 2130903337;
				fontProviderQuery = 2130903338;
				fontStyle = 2130903339;
				fontVariationSettings = 2130903340;
				fontWeight = 2130903341;
				foregroundInsidePadding = 2130903342;
				gapBetweenBars = 2130903343;
				goIcon = 2130903344;
				headerLayout = 2130903345;
				height = 2130903346;
				helperText = 2130903347;
				helperTextEnabled = 2130903348;
				helperTextTextAppearance = 2130903349;
				helperTextTextColor = 2130903350;
				hideMotionSpec = 2130903351;
				hideOnContentScroll = 2130903352;
				hideOnScroll = 2130903353;
				hintAnimationEnabled = 2130903354;
				hintEnabled = 2130903355;
				hintTextAppearance = 2130903356;
				hintTextColor = 2130903357;
				homeAsUpIndicator = 2130903358;
				homeLayout = 2130903359;
				hoveredFocusedTranslationZ = 2130903360;
				icon = 2130903361;
				iconEndPadding = 2130903362;
				iconGravity = 2130903363;
				iconifiedByDefault = 2130903369;
				iconPadding = 2130903364;
				iconSize = 2130903365;
				iconStartPadding = 2130903366;
				iconTint = 2130903367;
				iconTintMode = 2130903368;
				imageAspectRatio = 2130903370;
				imageAspectRatioAdjust = 2130903371;
				imageButtonStyle = 2130903372;
				indeterminateProgressStyle = 2130903373;
				initialActivityCount = 2130903374;
				insetForeground = 2130903375;
				isLightTheme = 2130903376;
				isMaterialTheme = 2130903377;
				itemBackground = 2130903378;
				itemFillColor = 2130903379;
				itemHorizontalPadding = 2130903380;
				itemHorizontalTranslationEnabled = 2130903381;
				itemIconPadding = 2130903382;
				itemIconSize = 2130903383;
				itemIconTint = 2130903384;
				itemMaxLines = 2130903385;
				itemPadding = 2130903386;
				itemRippleColor = 2130903387;
				itemShapeAppearance = 2130903388;
				itemShapeAppearanceOverlay = 2130903389;
				itemShapeFillColor = 2130903390;
				itemShapeInsetBottom = 2130903391;
				itemShapeInsetEnd = 2130903392;
				itemShapeInsetStart = 2130903393;
				itemShapeInsetTop = 2130903394;
				itemSpacing = 2130903395;
				itemStrokeColor = 2130903396;
				itemStrokeWidth = 2130903397;
				itemTextAppearance = 2130903398;
				itemTextAppearanceActive = 2130903399;
				itemTextAppearanceInactive = 2130903400;
				itemTextColor = 2130903401;
				keylines = 2130903402;
				labelVisibilityMode = 2130903403;
				lastBaselineToBottomHeight = 2130903404;
				layout = 2130903405;
				layoutManager = 2130903406;
				layout_anchor = 2130903407;
				layout_anchorGravity = 2130903408;
				layout_behavior = 2130903409;
				layout_collapseMode = 2130903410;
				layout_collapseParallaxMultiplier = 2130903411;
				layout_constrainedHeight = 2130903412;
				layout_constrainedWidth = 2130903413;
				layout_constraintBaseline_creator = 2130903414;
				layout_constraintBaseline_toBaselineOf = 2130903415;
				layout_constraintBottom_creator = 2130903416;
				layout_constraintBottom_toBottomOf = 2130903417;
				layout_constraintBottom_toTopOf = 2130903418;
				layout_constraintCircle = 2130903419;
				layout_constraintCircleAngle = 2130903420;
				layout_constraintCircleRadius = 2130903421;
				layout_constraintDimensionRatio = 2130903422;
				layout_constraintEnd_toEndOf = 2130903423;
				layout_constraintEnd_toStartOf = 2130903424;
				layout_constraintGuide_begin = 2130903425;
				layout_constraintGuide_end = 2130903426;
				layout_constraintGuide_percent = 2130903427;
				layout_constraintHeight_default = 2130903428;
				layout_constraintHeight_max = 2130903429;
				layout_constraintHeight_min = 2130903430;
				layout_constraintHeight_percent = 2130903431;
				layout_constraintHorizontal_bias = 2130903432;
				layout_constraintHorizontal_chainStyle = 2130903433;
				layout_constraintHorizontal_weight = 2130903434;
				layout_constraintLeft_creator = 2130903435;
				layout_constraintLeft_toLeftOf = 2130903436;
				layout_constraintLeft_toRightOf = 2130903437;
				layout_constraintRight_creator = 2130903438;
				layout_constraintRight_toLeftOf = 2130903439;
				layout_constraintRight_toRightOf = 2130903440;
				layout_constraintStart_toEndOf = 2130903441;
				layout_constraintStart_toStartOf = 2130903442;
				layout_constraintTop_creator = 2130903443;
				layout_constraintTop_toBottomOf = 2130903444;
				layout_constraintTop_toTopOf = 2130903445;
				layout_constraintVertical_bias = 2130903446;
				layout_constraintVertical_chainStyle = 2130903447;
				layout_constraintVertical_weight = 2130903448;
				layout_constraintWidth_default = 2130903449;
				layout_constraintWidth_max = 2130903450;
				layout_constraintWidth_min = 2130903451;
				layout_constraintWidth_percent = 2130903452;
				layout_dodgeInsetEdges = 2130903453;
				layout_editor_absoluteX = 2130903454;
				layout_editor_absoluteY = 2130903455;
				layout_goneMarginBottom = 2130903456;
				layout_goneMarginEnd = 2130903457;
				layout_goneMarginLeft = 2130903458;
				layout_goneMarginRight = 2130903459;
				layout_goneMarginStart = 2130903460;
				layout_goneMarginTop = 2130903461;
				layout_insetEdge = 2130903462;
				layout_keyline = 2130903463;
				layout_optimizationLevel = 2130903464;
				layout_scrollFlags = 2130903465;
				layout_scrollInterpolator = 2130903466;
				liftOnScroll = 2130903467;
				liftOnScrollTargetViewId = 2130903468;
				lineHeight = 2130903469;
				lineSpacing = 2130903470;
				listChoiceBackgroundIndicator = 2130903471;
				listChoiceIndicatorMultipleAnimated = 2130903472;
				listChoiceIndicatorSingleAnimated = 2130903473;
				listDividerAlertDialog = 2130903474;
				listItemLayout = 2130903475;
				listLayout = 2130903476;
				listMenuViewStyle = 2130903477;
				listPopupWindowStyle = 2130903478;
				listPreferredItemHeight = 2130903479;
				listPreferredItemHeightLarge = 2130903480;
				listPreferredItemHeightSmall = 2130903481;
				listPreferredItemPaddingEnd = 2130903482;
				listPreferredItemPaddingLeft = 2130903483;
				listPreferredItemPaddingRight = 2130903484;
				listPreferredItemPaddingStart = 2130903485;
				logo = 2130903486;
				logoDescription = 2130903487;
				materialAlertDialogBodyTextStyle = 2130903488;
				materialAlertDialogTheme = 2130903489;
				materialAlertDialogTitleIconStyle = 2130903490;
				materialAlertDialogTitlePanelStyle = 2130903491;
				materialAlertDialogTitleTextStyle = 2130903492;
				materialButtonOutlinedStyle = 2130903493;
				materialButtonStyle = 2130903494;
				materialButtonToggleGroupStyle = 2130903495;
				materialCalendarDay = 2130903496;
				materialCalendarFullscreenTheme = 2130903497;
				materialCalendarHeaderConfirmButton = 2130903498;
				materialCalendarHeaderDivider = 2130903499;
				materialCalendarHeaderLayout = 2130903500;
				materialCalendarHeaderSelection = 2130903501;
				materialCalendarHeaderTitle = 2130903502;
				materialCalendarHeaderToggleButton = 2130903503;
				materialCalendarStyle = 2130903504;
				materialCalendarTheme = 2130903505;
				materialCardViewStyle = 2130903506;
				materialThemeOverlay = 2130903507;
				maxActionInlineWidth = 2130903508;
				maxButtonHeight = 2130903509;
				maxCharacterCount = 2130903510;
				maxImageSize = 2130903511;
				measureWithLargestChild = 2130903512;
				menu = 2130903513;
				minTouchTargetSize = 2130903514;
				multiChoiceItemLayout = 2130903515;
				navigationContentDescription = 2130903516;
				navigationIcon = 2130903517;
				navigationMode = 2130903518;
				navigationViewStyle = 2130903519;
				number = 2130903520;
				numericModifiers = 2130903521;
				overlapAnchor = 2130903522;
				paddingBottomNoButtons = 2130903523;
				paddingEnd = 2130903524;
				paddingStart = 2130903525;
				paddingTopNoTitle = 2130903526;
				panelBackground = 2130903527;
				panelMenuListTheme = 2130903528;
				panelMenuListWidth = 2130903529;
				passwordToggleContentDescription = 2130903530;
				passwordToggleDrawable = 2130903531;
				passwordToggleEnabled = 2130903532;
				passwordToggleTint = 2130903533;
				passwordToggleTintMode = 2130903534;
				popupMenuBackground = 2130903535;
				popupMenuStyle = 2130903536;
				popupTheme = 2130903537;
				popupWindowStyle = 2130903538;
				preserveIconSpacing = 2130903539;
				pressedTranslationZ = 2130903540;
				progressBarPadding = 2130903541;
				progressBarStyle = 2130903542;
				queryBackground = 2130903543;
				queryHint = 2130903544;
				radioButtonStyle = 2130903545;
				rangeFillColor = 2130903546;
				ratingBarStyle = 2130903547;
				ratingBarStyleIndicator = 2130903548;
				ratingBarStyleSmall = 2130903549;
				recyclerViewStyle = 2130903550;
				reverseLayout = 2130903551;
				rippleColor = 2130903552;
				scopeUris = 2130903553;
				scrimAnimationDuration = 2130903554;
				scrimBackground = 2130903555;
				scrimVisibleHeightTrigger = 2130903556;
				searchHintIcon = 2130903557;
				searchIcon = 2130903558;
				searchViewStyle = 2130903559;
				seekBarStyle = 2130903560;
				selectableItemBackground = 2130903561;
				selectableItemBackgroundBorderless = 2130903562;
				shapeAppearance = 2130903563;
				shapeAppearanceLargeComponent = 2130903564;
				shapeAppearanceMediumComponent = 2130903565;
				shapeAppearanceOverlay = 2130903566;
				shapeAppearanceSmallComponent = 2130903567;
				showAsAction = 2130903568;
				showDividers = 2130903569;
				showMotionSpec = 2130903570;
				showText = 2130903571;
				showTitle = 2130903572;
				shrinkMotionSpec = 2130903573;
				singleChoiceItemLayout = 2130903574;
				singleLine = 2130903575;
				singleSelection = 2130903576;
				snackbarButtonStyle = 2130903577;
				snackbarStyle = 2130903578;
				spanCount = 2130903579;
				spinBars = 2130903580;
				spinnerDropDownItemStyle = 2130903581;
				spinnerStyle = 2130903582;
				splitTrack = 2130903583;
				srcCompat = 2130903584;
				stackFromEnd = 2130903585;
				startIconCheckable = 2130903586;
				startIconContentDescription = 2130903587;
				startIconDrawable = 2130903588;
				startIconTint = 2130903589;
				startIconTintMode = 2130903590;
				state_above_anchor = 2130903591;
				state_collapsed = 2130903592;
				state_collapsible = 2130903593;
				state_dragged = 2130903594;
				state_liftable = 2130903595;
				state_lifted = 2130903596;
				statusBarBackground = 2130903597;
				statusBarForeground = 2130903598;
				statusBarScrim = 2130903599;
				strokeColor = 2130903600;
				strokeWidth = 2130903601;
				subMenuArrow = 2130903602;
				submitBackground = 2130903603;
				subtitle = 2130903604;
				subtitleTextAppearance = 2130903605;
				subtitleTextColor = 2130903606;
				subtitleTextStyle = 2130903607;
				suggestionRowLayout = 2130903608;
				switchMinWidth = 2130903609;
				switchPadding = 2130903610;
				switchStyle = 2130903611;
				switchTextAppearance = 2130903612;
				tabBackground = 2130903613;
				tabContentStart = 2130903614;
				tabGravity = 2130903615;
				tabIconTint = 2130903616;
				tabIconTintMode = 2130903617;
				tabIndicator = 2130903618;
				tabIndicatorAnimationDuration = 2130903619;
				tabIndicatorColor = 2130903620;
				tabIndicatorFullWidth = 2130903621;
				tabIndicatorGravity = 2130903622;
				tabIndicatorHeight = 2130903623;
				tabInlineLabel = 2130903624;
				tabMaxWidth = 2130903625;
				tabMinWidth = 2130903626;
				tabMode = 2130903627;
				tabPadding = 2130903628;
				tabPaddingBottom = 2130903629;
				tabPaddingEnd = 2130903630;
				tabPaddingStart = 2130903631;
				tabPaddingTop = 2130903632;
				tabRippleColor = 2130903633;
				tabSelectedTextColor = 2130903634;
				tabStyle = 2130903635;
				tabTextAppearance = 2130903636;
				tabTextColor = 2130903637;
				tabUnboundedRipple = 2130903638;
				textAllCaps = 2130903639;
				textAppearanceBody1 = 2130903640;
				textAppearanceBody2 = 2130903641;
				textAppearanceButton = 2130903642;
				textAppearanceCaption = 2130903643;
				textAppearanceHeadline1 = 2130903644;
				textAppearanceHeadline2 = 2130903645;
				textAppearanceHeadline3 = 2130903646;
				textAppearanceHeadline4 = 2130903647;
				textAppearanceHeadline5 = 2130903648;
				textAppearanceHeadline6 = 2130903649;
				textAppearanceLargePopupMenu = 2130903650;
				textAppearanceLineHeightEnabled = 2130903651;
				textAppearanceListItem = 2130903652;
				textAppearanceListItemSecondary = 2130903653;
				textAppearanceListItemSmall = 2130903654;
				textAppearanceOverline = 2130903655;
				textAppearancePopupMenuHeader = 2130903656;
				textAppearanceSearchResultSubtitle = 2130903657;
				textAppearanceSearchResultTitle = 2130903658;
				textAppearanceSmallPopupMenu = 2130903659;
				textAppearanceSubtitle1 = 2130903660;
				textAppearanceSubtitle2 = 2130903661;
				textColorAlertDialogListItem = 2130903662;
				textColorSearchUrl = 2130903663;
				textEndPadding = 2130903664;
				textInputStyle = 2130903665;
				textLocale = 2130903666;
				textStartPadding = 2130903667;
				theme = 2130903668;
				themeLineHeight = 2130903669;
				thickness = 2130903670;
				thumbTextPadding = 2130903671;
				thumbTint = 2130903672;
				thumbTintMode = 2130903673;
				tickMark = 2130903674;
				tickMarkTint = 2130903675;
				tickMarkTintMode = 2130903676;
				tint = 2130903677;
				tintMode = 2130903678;
				title = 2130903679;
				titleEnabled = 2130903680;
				titleMargin = 2130903681;
				titleMarginBottom = 2130903682;
				titleMarginEnd = 2130903683;
				titleMargins = 2130903686;
				titleMarginStart = 2130903684;
				titleMarginTop = 2130903685;
				titleTextAppearance = 2130903687;
				titleTextColor = 2130903688;
				titleTextStyle = 2130903689;
				toolbarId = 2130903690;
				toolbarNavigationButtonStyle = 2130903691;
				toolbarStyle = 2130903692;
				tooltipForegroundColor = 2130903693;
				tooltipFrameBackground = 2130903694;
				tooltipText = 2130903695;
				track = 2130903696;
				trackTint = 2130903697;
				trackTintMode = 2130903698;
				ttcIndex = 2130903699;
				useCompatPadding = 2130903700;
				useMaterialThemeColors = 2130903701;
				viewInflaterClass = 2130903702;
				voiceIcon = 2130903703;
				windowActionBar = 2130903704;
				windowActionBarOverlay = 2130903705;
				windowActionModeOverlay = 2130903706;
				windowFixedHeightMajor = 2130903707;
				windowFixedHeightMinor = 2130903708;
				windowFixedWidthMajor = 2130903709;
				windowFixedWidthMinor = 2130903710;
				windowMinWidthMajor = 2130903711;
				windowMinWidthMinor = 2130903712;
				windowNoTitle = 2130903713;
				yearSelectedStyle = 2130903714;
				yearStyle = 2130903715;
				yearTodayStyle = 2130903716;
				ResourceIdManager.UpdateIdValues();
			}

			private Attribute()
			{
			}
		}

		public class Boolean
		{
			public static int abc_action_bar_embed_tabs;

			public static int abc_allow_stacked_button_bar;

			public static int abc_config_actionMenuItemAllCaps;

			public static int mtrl_btn_textappearance_all_caps;

			static Boolean()
			{
				abc_action_bar_embed_tabs = 2130968576;
				abc_allow_stacked_button_bar = 2130968577;
				abc_config_actionMenuItemAllCaps = 2130968578;
				mtrl_btn_textappearance_all_caps = 2130968579;
				ResourceIdManager.UpdateIdValues();
			}

			private Boolean()
			{
			}
		}

		public class Color
		{
			public static int abc_background_cache_hint_selector_material_dark;

			public static int abc_background_cache_hint_selector_material_light;

			public static int abc_btn_colored_borderless_text_material;

			public static int abc_btn_colored_text_material;

			public static int abc_color_highlight_material;

			public static int abc_hint_foreground_material_dark;

			public static int abc_hint_foreground_material_light;

			public static int abc_input_method_navigation_guard;

			public static int abc_primary_text_disable_only_material_dark;

			public static int abc_primary_text_disable_only_material_light;

			public static int abc_primary_text_material_dark;

			public static int abc_primary_text_material_light;

			public static int abc_search_url_text;

			public static int abc_search_url_text_normal;

			public static int abc_search_url_text_pressed;

			public static int abc_search_url_text_selected;

			public static int abc_secondary_text_material_dark;

			public static int abc_secondary_text_material_light;

			public static int abc_tint_btn_checkable;

			public static int abc_tint_default;

			public static int abc_tint_edittext;

			public static int abc_tint_seek_thumb;

			public static int abc_tint_spinner;

			public static int abc_tint_switch_track;

			public static int accent_material_dark;

			public static int accent_material_light;

			public static int activated_color;

			public static int backgroundColor;

			public static int background_floating_material_dark;

			public static int background_floating_material_light;

			public static int background_material_dark;

			public static int background_material_light;

			public static int bright_foreground_disabled_material_dark;

			public static int bright_foreground_disabled_material_light;

			public static int bright_foreground_inverse_material_dark;

			public static int bright_foreground_inverse_material_light;

			public static int bright_foreground_material_dark;

			public static int bright_foreground_material_light;

			public static int browser_actions_bg_grey;

			public static int browser_actions_divider_color;

			public static int browser_actions_text_color;

			public static int browser_actions_title_color;

			public static int buttonOnGreen;

			public static int button_material_dark;

			public static int button_material_light;

			public static int cardview_dark_background;

			public static int cardview_light_background;

			public static int cardview_shadow_end_color;

			public static int cardview_shadow_start_color;

			public static int checkbox_themeable_attribute_color;

			public static int colorAccent;

			public static int colorControlActivated;

			public static int colorPrimary;

			public static int colorPrimaryDark;

			public static int colorPrimaryMedium;

			public static int common_google_signin_btn_text_dark;

			public static int common_google_signin_btn_text_dark_default;

			public static int common_google_signin_btn_text_dark_disabled;

			public static int common_google_signin_btn_text_dark_focused;

			public static int common_google_signin_btn_text_dark_pressed;

			public static int common_google_signin_btn_text_light;

			public static int common_google_signin_btn_text_light_default;

			public static int common_google_signin_btn_text_light_disabled;

			public static int common_google_signin_btn_text_light_focused;

			public static int common_google_signin_btn_text_light_pressed;

			public static int common_google_signin_btn_tint;

			public static int counterExplainText;

			public static int counterLayoutBackgroundColor;

			public static int design_bottom_navigation_shadow_color;

			public static int design_box_stroke_color;

			public static int design_dark_default_color_background;

			public static int design_dark_default_color_error;

			public static int design_dark_default_color_on_background;

			public static int design_dark_default_color_on_error;

			public static int design_dark_default_color_on_primary;

			public static int design_dark_default_color_on_secondary;

			public static int design_dark_default_color_on_surface;

			public static int design_dark_default_color_primary;

			public static int design_dark_default_color_primary_dark;

			public static int design_dark_default_color_primary_variant;

			public static int design_dark_default_color_secondary;

			public static int design_dark_default_color_secondary_variant;

			public static int design_dark_default_color_surface;

			public static int design_default_color_background;

			public static int design_default_color_error;

			public static int design_default_color_on_background;

			public static int design_default_color_on_error;

			public static int design_default_color_on_primary;

			public static int design_default_color_on_secondary;

			public static int design_default_color_on_surface;

			public static int design_default_color_primary;

			public static int design_default_color_primary_dark;

			public static int design_default_color_primary_variant;

			public static int design_default_color_secondary;

			public static int design_default_color_secondary_variant;

			public static int design_default_color_surface;

			public static int design_error;

			public static int design_fab_shadow_end_color;

			public static int design_fab_shadow_mid_color;

			public static int design_fab_shadow_start_color;

			public static int design_fab_stroke_end_inner_color;

			public static int design_fab_stroke_end_outer_color;

			public static int design_fab_stroke_top_inner_color;

			public static int design_fab_stroke_top_outer_color;

			public static int design_icon_tint;

			public static int design_snackbar_background_color;

			public static int dim_foreground_disabled_material_dark;

			public static int dim_foreground_disabled_material_light;

			public static int dim_foreground_material_dark;

			public static int dim_foreground_material_light;

			public static int divider;

			public static int dividerBlue;

			public static int dividerWhite;

			public static int errorColor;

			public static int error_color_material_dark;

			public static int error_color_material_light;

			public static int foreground_material_dark;

			public static int foreground_material_light;

			public static int greyedOut;

			public static int highlighted_text_material_dark;

			public static int highlighted_text_material_light;

			public static int ic_launcher_background;

			public static int infectionStatusBackgroundGreen;

			public static int infectionStatusBackgroundRed;

			public static int infectionStatusButtonOffRed;

			public static int infectionStatusButtonOnGreen;

			public static int infectionStatusLayoutButtonArrowBackground;

			public static int infectionStatusLayoutButtonBackground;

			public static int lightBlueDivider;

			public static int lightPrimary;

			public static int linkColor;

			public static int material_blue_grey_800;

			public static int material_blue_grey_900;

			public static int material_blue_grey_950;

			public static int material_deep_teal_200;

			public static int material_deep_teal_500;

			public static int material_grey_100;

			public static int material_grey_300;

			public static int material_grey_50;

			public static int material_grey_600;

			public static int material_grey_800;

			public static int material_grey_850;

			public static int material_grey_900;

			public static int material_on_background_disabled;

			public static int material_on_background_emphasis_high_type;

			public static int material_on_background_emphasis_medium;

			public static int material_on_primary_disabled;

			public static int material_on_primary_emphasis_high_type;

			public static int material_on_primary_emphasis_medium;

			public static int material_on_surface_disabled;

			public static int material_on_surface_emphasis_high_type;

			public static int material_on_surface_emphasis_medium;

			public static int mtrl_bottom_nav_colored_item_tint;

			public static int mtrl_bottom_nav_colored_ripple_color;

			public static int mtrl_bottom_nav_item_tint;

			public static int mtrl_bottom_nav_ripple_color;

			public static int mtrl_btn_bg_color_selector;

			public static int mtrl_btn_ripple_color;

			public static int mtrl_btn_stroke_color_selector;

			public static int mtrl_btn_text_btn_bg_color_selector;

			public static int mtrl_btn_text_btn_ripple_color;

			public static int mtrl_btn_text_color_disabled;

			public static int mtrl_btn_text_color_selector;

			public static int mtrl_btn_transparent_bg_color;

			public static int mtrl_calendar_item_stroke_color;

			public static int mtrl_calendar_selected_range;

			public static int mtrl_card_view_foreground;

			public static int mtrl_card_view_ripple;

			public static int mtrl_chip_background_color;

			public static int mtrl_chip_close_icon_tint;

			public static int mtrl_chip_ripple_color;

			public static int mtrl_chip_surface_color;

			public static int mtrl_chip_text_color;

			public static int mtrl_choice_chip_background_color;

			public static int mtrl_choice_chip_ripple_color;

			public static int mtrl_choice_chip_text_color;

			public static int mtrl_error;

			public static int mtrl_extended_fab_bg_color_selector;

			public static int mtrl_extended_fab_ripple_color;

			public static int mtrl_extended_fab_text_color_selector;

			public static int mtrl_fab_ripple_color;

			public static int mtrl_filled_background_color;

			public static int mtrl_filled_icon_tint;

			public static int mtrl_filled_stroke_color;

			public static int mtrl_indicator_text_color;

			public static int mtrl_navigation_item_background_color;

			public static int mtrl_navigation_item_icon_tint;

			public static int mtrl_navigation_item_text_color;

			public static int mtrl_on_primary_text_btn_text_color_selector;

			public static int mtrl_outlined_icon_tint;

			public static int mtrl_outlined_stroke_color;

			public static int mtrl_popupmenu_overlay_color;

			public static int mtrl_scrim_color;

			public static int mtrl_tabs_colored_ripple_color;

			public static int mtrl_tabs_icon_color_selector;

			public static int mtrl_tabs_icon_color_selector_colored;

			public static int mtrl_tabs_legacy_text_color_selector;

			public static int mtrl_tabs_ripple_color;

			public static int mtrl_textinput_default_box_stroke_color;

			public static int mtrl_textinput_disabled_color;

			public static int mtrl_textinput_filled_box_default_background_color;

			public static int mtrl_textinput_focused_box_stroke_color;

			public static int mtrl_textinput_hovered_box_stroke_color;

			public static int mtrl_text_btn_text_color_selector;

			public static int notification_action_color_filter;

			public static int notification_icon_bg_color;

			public static int notification_material_background_media_default_color;

			public static int primaryText;

			public static int primary_dark_material_dark;

			public static int primary_dark_material_light;

			public static int primary_material_dark;

			public static int primary_material_light;

			public static int primary_text_default_material_dark;

			public static int primary_text_default_material_light;

			public static int primary_text_disabled_material_dark;

			public static int primary_text_disabled_material_light;

			public static int ripple_material_dark;

			public static int ripple_material_light;

			public static int secondaryText;

			public static int secondary_text_default_material_dark;

			public static int secondary_text_default_material_light;

			public static int secondary_text_disabled_material_dark;

			public static int secondary_text_disabled_material_light;

			public static int selectedDot;

			public static int splashBackground;

			public static int switchSelectedThumb;

			public static int switchSelectedTrack;

			public static int switchUnselectedThumb;

			public static int switchUnselectedTrack;

			public static int switch_thumb_disabled_material_dark;

			public static int switch_thumb_disabled_material_light;

			public static int switch_thumb_material_dark;

			public static int switch_thumb_material_light;

			public static int switch_thumb_normal_material_dark;

			public static int switch_thumb_normal_material_light;

			public static int test_mtrl_calendar_day;

			public static int test_mtrl_calendar_day_selected;

			public static int textIcon;

			public static int tooltip_background_dark;

			public static int tooltip_background_light;

			public static int topbar;

			public static int topbarDevicer;

			public static int unselectedDot;

			public static int warningColor;

			static Color()
			{
				abc_background_cache_hint_selector_material_dark = 2131034112;
				abc_background_cache_hint_selector_material_light = 2131034113;
				abc_btn_colored_borderless_text_material = 2131034114;
				abc_btn_colored_text_material = 2131034115;
				abc_color_highlight_material = 2131034116;
				abc_hint_foreground_material_dark = 2131034117;
				abc_hint_foreground_material_light = 2131034118;
				abc_input_method_navigation_guard = 2131034119;
				abc_primary_text_disable_only_material_dark = 2131034120;
				abc_primary_text_disable_only_material_light = 2131034121;
				abc_primary_text_material_dark = 2131034122;
				abc_primary_text_material_light = 2131034123;
				abc_search_url_text = 2131034124;
				abc_search_url_text_normal = 2131034125;
				abc_search_url_text_pressed = 2131034126;
				abc_search_url_text_selected = 2131034127;
				abc_secondary_text_material_dark = 2131034128;
				abc_secondary_text_material_light = 2131034129;
				abc_tint_btn_checkable = 2131034130;
				abc_tint_default = 2131034131;
				abc_tint_edittext = 2131034132;
				abc_tint_seek_thumb = 2131034133;
				abc_tint_spinner = 2131034134;
				abc_tint_switch_track = 2131034135;
				accent_material_dark = 2131034136;
				accent_material_light = 2131034137;
				activated_color = 2131034138;
				backgroundColor = 2131034139;
				background_floating_material_dark = 2131034140;
				background_floating_material_light = 2131034141;
				background_material_dark = 2131034142;
				background_material_light = 2131034143;
				bright_foreground_disabled_material_dark = 2131034144;
				bright_foreground_disabled_material_light = 2131034145;
				bright_foreground_inverse_material_dark = 2131034146;
				bright_foreground_inverse_material_light = 2131034147;
				bright_foreground_material_dark = 2131034148;
				bright_foreground_material_light = 2131034149;
				browser_actions_bg_grey = 2131034150;
				browser_actions_divider_color = 2131034151;
				browser_actions_text_color = 2131034152;
				browser_actions_title_color = 2131034153;
				buttonOnGreen = 2131034154;
				button_material_dark = 2131034155;
				button_material_light = 2131034156;
				cardview_dark_background = 2131034157;
				cardview_light_background = 2131034158;
				cardview_shadow_end_color = 2131034159;
				cardview_shadow_start_color = 2131034160;
				checkbox_themeable_attribute_color = 2131034161;
				colorAccent = 2131034162;
				colorControlActivated = 2131034163;
				colorPrimary = 2131034164;
				colorPrimaryDark = 2131034165;
				colorPrimaryMedium = 2131034166;
				common_google_signin_btn_text_dark = 2131034167;
				common_google_signin_btn_text_dark_default = 2131034168;
				common_google_signin_btn_text_dark_disabled = 2131034169;
				common_google_signin_btn_text_dark_focused = 2131034170;
				common_google_signin_btn_text_dark_pressed = 2131034171;
				common_google_signin_btn_text_light = 2131034172;
				common_google_signin_btn_text_light_default = 2131034173;
				common_google_signin_btn_text_light_disabled = 2131034174;
				common_google_signin_btn_text_light_focused = 2131034175;
				common_google_signin_btn_text_light_pressed = 2131034176;
				common_google_signin_btn_tint = 2131034177;
				counterExplainText = 2131034178;
				counterLayoutBackgroundColor = 2131034179;
				design_bottom_navigation_shadow_color = 2131034180;
				design_box_stroke_color = 2131034181;
				design_dark_default_color_background = 2131034182;
				design_dark_default_color_error = 2131034183;
				design_dark_default_color_on_background = 2131034184;
				design_dark_default_color_on_error = 2131034185;
				design_dark_default_color_on_primary = 2131034186;
				design_dark_default_color_on_secondary = 2131034187;
				design_dark_default_color_on_surface = 2131034188;
				design_dark_default_color_primary = 2131034189;
				design_dark_default_color_primary_dark = 2131034190;
				design_dark_default_color_primary_variant = 2131034191;
				design_dark_default_color_secondary = 2131034192;
				design_dark_default_color_secondary_variant = 2131034193;
				design_dark_default_color_surface = 2131034194;
				design_default_color_background = 2131034195;
				design_default_color_error = 2131034196;
				design_default_color_on_background = 2131034197;
				design_default_color_on_error = 2131034198;
				design_default_color_on_primary = 2131034199;
				design_default_color_on_secondary = 2131034200;
				design_default_color_on_surface = 2131034201;
				design_default_color_primary = 2131034202;
				design_default_color_primary_dark = 2131034203;
				design_default_color_primary_variant = 2131034204;
				design_default_color_secondary = 2131034205;
				design_default_color_secondary_variant = 2131034206;
				design_default_color_surface = 2131034207;
				design_error = 2131034208;
				design_fab_shadow_end_color = 2131034209;
				design_fab_shadow_mid_color = 2131034210;
				design_fab_shadow_start_color = 2131034211;
				design_fab_stroke_end_inner_color = 2131034212;
				design_fab_stroke_end_outer_color = 2131034213;
				design_fab_stroke_top_inner_color = 2131034214;
				design_fab_stroke_top_outer_color = 2131034215;
				design_icon_tint = 2131034216;
				design_snackbar_background_color = 2131034217;
				dim_foreground_disabled_material_dark = 2131034218;
				dim_foreground_disabled_material_light = 2131034219;
				dim_foreground_material_dark = 2131034220;
				dim_foreground_material_light = 2131034221;
				divider = 2131034222;
				dividerBlue = 2131034223;
				dividerWhite = 2131034224;
				errorColor = 2131034225;
				error_color_material_dark = 2131034226;
				error_color_material_light = 2131034227;
				foreground_material_dark = 2131034228;
				foreground_material_light = 2131034229;
				greyedOut = 2131034230;
				highlighted_text_material_dark = 2131034231;
				highlighted_text_material_light = 2131034232;
				ic_launcher_background = 2131034233;
				infectionStatusBackgroundGreen = 2131034234;
				infectionStatusBackgroundRed = 2131034235;
				infectionStatusButtonOffRed = 2131034236;
				infectionStatusButtonOnGreen = 2131034237;
				infectionStatusLayoutButtonArrowBackground = 2131034238;
				infectionStatusLayoutButtonBackground = 2131034239;
				lightBlueDivider = 2131034240;
				lightPrimary = 2131034241;
				linkColor = 2131034242;
				material_blue_grey_800 = 2131034243;
				material_blue_grey_900 = 2131034244;
				material_blue_grey_950 = 2131034245;
				material_deep_teal_200 = 2131034246;
				material_deep_teal_500 = 2131034247;
				material_grey_100 = 2131034248;
				material_grey_300 = 2131034249;
				material_grey_50 = 2131034250;
				material_grey_600 = 2131034251;
				material_grey_800 = 2131034252;
				material_grey_850 = 2131034253;
				material_grey_900 = 2131034254;
				material_on_background_disabled = 2131034255;
				material_on_background_emphasis_high_type = 2131034256;
				material_on_background_emphasis_medium = 2131034257;
				material_on_primary_disabled = 2131034258;
				material_on_primary_emphasis_high_type = 2131034259;
				material_on_primary_emphasis_medium = 2131034260;
				material_on_surface_disabled = 2131034261;
				material_on_surface_emphasis_high_type = 2131034262;
				material_on_surface_emphasis_medium = 2131034263;
				mtrl_bottom_nav_colored_item_tint = 2131034264;
				mtrl_bottom_nav_colored_ripple_color = 2131034265;
				mtrl_bottom_nav_item_tint = 2131034266;
				mtrl_bottom_nav_ripple_color = 2131034267;
				mtrl_btn_bg_color_selector = 2131034268;
				mtrl_btn_ripple_color = 2131034269;
				mtrl_btn_stroke_color_selector = 2131034270;
				mtrl_btn_text_btn_bg_color_selector = 2131034271;
				mtrl_btn_text_btn_ripple_color = 2131034272;
				mtrl_btn_text_color_disabled = 2131034273;
				mtrl_btn_text_color_selector = 2131034274;
				mtrl_btn_transparent_bg_color = 2131034275;
				mtrl_calendar_item_stroke_color = 2131034276;
				mtrl_calendar_selected_range = 2131034277;
				mtrl_card_view_foreground = 2131034278;
				mtrl_card_view_ripple = 2131034279;
				mtrl_chip_background_color = 2131034280;
				mtrl_chip_close_icon_tint = 2131034281;
				mtrl_chip_ripple_color = 2131034282;
				mtrl_chip_surface_color = 2131034283;
				mtrl_chip_text_color = 2131034284;
				mtrl_choice_chip_background_color = 2131034285;
				mtrl_choice_chip_ripple_color = 2131034286;
				mtrl_choice_chip_text_color = 2131034287;
				mtrl_error = 2131034288;
				mtrl_extended_fab_bg_color_selector = 2131034289;
				mtrl_extended_fab_ripple_color = 2131034290;
				mtrl_extended_fab_text_color_selector = 2131034291;
				mtrl_fab_ripple_color = 2131034292;
				mtrl_filled_background_color = 2131034293;
				mtrl_filled_icon_tint = 2131034294;
				mtrl_filled_stroke_color = 2131034295;
				mtrl_indicator_text_color = 2131034296;
				mtrl_navigation_item_background_color = 2131034297;
				mtrl_navigation_item_icon_tint = 2131034298;
				mtrl_navigation_item_text_color = 2131034299;
				mtrl_on_primary_text_btn_text_color_selector = 2131034300;
				mtrl_outlined_icon_tint = 2131034301;
				mtrl_outlined_stroke_color = 2131034302;
				mtrl_popupmenu_overlay_color = 2131034303;
				mtrl_scrim_color = 2131034304;
				mtrl_tabs_colored_ripple_color = 2131034305;
				mtrl_tabs_icon_color_selector = 2131034306;
				mtrl_tabs_icon_color_selector_colored = 2131034307;
				mtrl_tabs_legacy_text_color_selector = 2131034308;
				mtrl_tabs_ripple_color = 2131034309;
				mtrl_textinput_default_box_stroke_color = 2131034311;
				mtrl_textinput_disabled_color = 2131034312;
				mtrl_textinput_filled_box_default_background_color = 2131034313;
				mtrl_textinput_focused_box_stroke_color = 2131034314;
				mtrl_textinput_hovered_box_stroke_color = 2131034315;
				mtrl_text_btn_text_color_selector = 2131034310;
				notification_action_color_filter = 2131034316;
				notification_icon_bg_color = 2131034317;
				notification_material_background_media_default_color = 2131034318;
				primaryText = 2131034319;
				primary_dark_material_dark = 2131034320;
				primary_dark_material_light = 2131034321;
				primary_material_dark = 2131034322;
				primary_material_light = 2131034323;
				primary_text_default_material_dark = 2131034324;
				primary_text_default_material_light = 2131034325;
				primary_text_disabled_material_dark = 2131034326;
				primary_text_disabled_material_light = 2131034327;
				ripple_material_dark = 2131034328;
				ripple_material_light = 2131034329;
				secondaryText = 2131034330;
				secondary_text_default_material_dark = 2131034331;
				secondary_text_default_material_light = 2131034332;
				secondary_text_disabled_material_dark = 2131034333;
				secondary_text_disabled_material_light = 2131034334;
				selectedDot = 2131034335;
				splashBackground = 2131034336;
				switchSelectedThumb = 2131034337;
				switchSelectedTrack = 2131034338;
				switchUnselectedThumb = 2131034339;
				switchUnselectedTrack = 2131034340;
				switch_thumb_disabled_material_dark = 2131034341;
				switch_thumb_disabled_material_light = 2131034342;
				switch_thumb_material_dark = 2131034343;
				switch_thumb_material_light = 2131034344;
				switch_thumb_normal_material_dark = 2131034345;
				switch_thumb_normal_material_light = 2131034346;
				test_mtrl_calendar_day = 2131034347;
				test_mtrl_calendar_day_selected = 2131034348;
				textIcon = 2131034349;
				tooltip_background_dark = 2131034350;
				tooltip_background_light = 2131034351;
				topbar = 2131034352;
				topbarDevicer = 2131034353;
				unselectedDot = 2131034354;
				warningColor = 2131034355;
				ResourceIdManager.UpdateIdValues();
			}

			private Color()
			{
			}
		}

		public class Dimension
		{
			public static int abc_action_bar_content_inset_material;

			public static int abc_action_bar_content_inset_with_nav;

			public static int abc_action_bar_default_height_material;

			public static int abc_action_bar_default_padding_end_material;

			public static int abc_action_bar_default_padding_start_material;

			public static int abc_action_bar_elevation_material;

			public static int abc_action_bar_icon_vertical_padding_material;

			public static int abc_action_bar_overflow_padding_end_material;

			public static int abc_action_bar_overflow_padding_start_material;

			public static int abc_action_bar_stacked_max_height;

			public static int abc_action_bar_stacked_tab_max_width;

			public static int abc_action_bar_subtitle_bottom_margin_material;

			public static int abc_action_bar_subtitle_top_margin_material;

			public static int abc_action_button_min_height_material;

			public static int abc_action_button_min_width_material;

			public static int abc_action_button_min_width_overflow_material;

			public static int abc_alert_dialog_button_bar_height;

			public static int abc_alert_dialog_button_dimen;

			public static int abc_button_inset_horizontal_material;

			public static int abc_button_inset_vertical_material;

			public static int abc_button_padding_horizontal_material;

			public static int abc_button_padding_vertical_material;

			public static int abc_cascading_menus_min_smallest_width;

			public static int abc_config_prefDialogWidth;

			public static int abc_control_corner_material;

			public static int abc_control_inset_material;

			public static int abc_control_padding_material;

			public static int abc_dialog_corner_radius_material;

			public static int abc_dialog_fixed_height_major;

			public static int abc_dialog_fixed_height_minor;

			public static int abc_dialog_fixed_width_major;

			public static int abc_dialog_fixed_width_minor;

			public static int abc_dialog_list_padding_bottom_no_buttons;

			public static int abc_dialog_list_padding_top_no_title;

			public static int abc_dialog_min_width_major;

			public static int abc_dialog_min_width_minor;

			public static int abc_dialog_padding_material;

			public static int abc_dialog_padding_top_material;

			public static int abc_dialog_title_divider_material;

			public static int abc_disabled_alpha_material_dark;

			public static int abc_disabled_alpha_material_light;

			public static int abc_dropdownitem_icon_width;

			public static int abc_dropdownitem_text_padding_left;

			public static int abc_dropdownitem_text_padding_right;

			public static int abc_edit_text_inset_bottom_material;

			public static int abc_edit_text_inset_horizontal_material;

			public static int abc_edit_text_inset_top_material;

			public static int abc_floating_window_z;

			public static int abc_list_item_height_large_material;

			public static int abc_list_item_height_material;

			public static int abc_list_item_height_small_material;

			public static int abc_list_item_padding_horizontal_material;

			public static int abc_panel_menu_list_width;

			public static int abc_progress_bar_height_material;

			public static int abc_search_view_preferred_height;

			public static int abc_search_view_preferred_width;

			public static int abc_seekbar_track_background_height_material;

			public static int abc_seekbar_track_progress_height_material;

			public static int abc_select_dialog_padding_start_material;

			public static int abc_switch_padding;

			public static int abc_text_size_body_1_material;

			public static int abc_text_size_body_2_material;

			public static int abc_text_size_button_material;

			public static int abc_text_size_caption_material;

			public static int abc_text_size_display_1_material;

			public static int abc_text_size_display_2_material;

			public static int abc_text_size_display_3_material;

			public static int abc_text_size_display_4_material;

			public static int abc_text_size_headline_material;

			public static int abc_text_size_large_material;

			public static int abc_text_size_medium_material;

			public static int abc_text_size_menu_header_material;

			public static int abc_text_size_menu_material;

			public static int abc_text_size_small_material;

			public static int abc_text_size_subhead_material;

			public static int abc_text_size_subtitle_material_toolbar;

			public static int abc_text_size_title_material;

			public static int abc_text_size_title_material_toolbar;

			public static int action_bar_size;

			public static int appcompat_dialog_background_inset;

			public static int browser_actions_context_menu_max_width;

			public static int browser_actions_context_menu_min_padding;

			public static int cardview_compat_inset_shadow;

			public static int cardview_default_elevation;

			public static int cardview_default_radius;

			public static int compat_button_inset_horizontal_material;

			public static int compat_button_inset_vertical_material;

			public static int compat_button_padding_horizontal_material;

			public static int compat_button_padding_vertical_material;

			public static int compat_control_corner_material;

			public static int compat_notification_large_icon_max_height;

			public static int compat_notification_large_icon_max_width;

			public static int default_dimension;

			public static int design_appbar_elevation;

			public static int design_bottom_navigation_active_item_max_width;

			public static int design_bottom_navigation_active_item_min_width;

			public static int design_bottom_navigation_active_text_size;

			public static int design_bottom_navigation_elevation;

			public static int design_bottom_navigation_height;

			public static int design_bottom_navigation_icon_size;

			public static int design_bottom_navigation_item_max_width;

			public static int design_bottom_navigation_item_min_width;

			public static int design_bottom_navigation_margin;

			public static int design_bottom_navigation_shadow_height;

			public static int design_bottom_navigation_text_size;

			public static int design_bottom_sheet_elevation;

			public static int design_bottom_sheet_modal_elevation;

			public static int design_bottom_sheet_peek_height_min;

			public static int design_fab_border_width;

			public static int design_fab_elevation;

			public static int design_fab_image_size;

			public static int design_fab_size_mini;

			public static int design_fab_size_normal;

			public static int design_fab_translation_z_hovered_focused;

			public static int design_fab_translation_z_pressed;

			public static int design_navigation_elevation;

			public static int design_navigation_icon_padding;

			public static int design_navigation_icon_size;

			public static int design_navigation_item_horizontal_padding;

			public static int design_navigation_item_icon_padding;

			public static int design_navigation_max_width;

			public static int design_navigation_padding_bottom;

			public static int design_navigation_separator_vertical_padding;

			public static int design_snackbar_action_inline_max_width;

			public static int design_snackbar_action_text_color_alpha;

			public static int design_snackbar_background_corner_radius;

			public static int design_snackbar_elevation;

			public static int design_snackbar_extra_spacing_horizontal;

			public static int design_snackbar_max_width;

			public static int design_snackbar_min_width;

			public static int design_snackbar_padding_horizontal;

			public static int design_snackbar_padding_vertical;

			public static int design_snackbar_padding_vertical_2lines;

			public static int design_snackbar_text_size;

			public static int design_tab_max_width;

			public static int design_tab_scrollable_min_width;

			public static int design_tab_text_size;

			public static int design_tab_text_size_2line;

			public static int design_textinput_caption_translate_y;

			public static int disabled_alpha_material_dark;

			public static int disabled_alpha_material_light;

			public static int fab_margin;

			public static int fastscroll_default_thickness;

			public static int fastscroll_margin;

			public static int fastscroll_minimum_range;

			public static int highlight_alpha_material_colored;

			public static int highlight_alpha_material_dark;

			public static int highlight_alpha_material_light;

			public static int hint_alpha_material_dark;

			public static int hint_alpha_material_light;

			public static int hint_pressed_alpha_material_dark;

			public static int hint_pressed_alpha_material_light;

			public static int item_touch_helper_max_drag_scroll_per_frame;

			public static int item_touch_helper_swipe_escape_max_velocity;

			public static int item_touch_helper_swipe_escape_velocity;

			public static int material_emphasis_disabled;

			public static int material_emphasis_high_type;

			public static int material_emphasis_medium;

			public static int material_text_view_test_line_height;

			public static int material_text_view_test_line_height_override;

			public static int mtrl_alert_dialog_background_inset_bottom;

			public static int mtrl_alert_dialog_background_inset_end;

			public static int mtrl_alert_dialog_background_inset_start;

			public static int mtrl_alert_dialog_background_inset_top;

			public static int mtrl_alert_dialog_picker_background_inset;

			public static int mtrl_badge_horizontal_edge_offset;

			public static int mtrl_badge_long_text_horizontal_padding;

			public static int mtrl_badge_radius;

			public static int mtrl_badge_text_horizontal_edge_offset;

			public static int mtrl_badge_text_size;

			public static int mtrl_badge_with_text_radius;

			public static int mtrl_bottomappbar_fabOffsetEndMode;

			public static int mtrl_bottomappbar_fab_bottom_margin;

			public static int mtrl_bottomappbar_fab_cradle_margin;

			public static int mtrl_bottomappbar_fab_cradle_rounded_corner_radius;

			public static int mtrl_bottomappbar_fab_cradle_vertical_offset;

			public static int mtrl_bottomappbar_height;

			public static int mtrl_btn_corner_radius;

			public static int mtrl_btn_dialog_btn_min_width;

			public static int mtrl_btn_disabled_elevation;

			public static int mtrl_btn_disabled_z;

			public static int mtrl_btn_elevation;

			public static int mtrl_btn_focused_z;

			public static int mtrl_btn_hovered_z;

			public static int mtrl_btn_icon_btn_padding_left;

			public static int mtrl_btn_icon_padding;

			public static int mtrl_btn_inset;

			public static int mtrl_btn_letter_spacing;

			public static int mtrl_btn_padding_bottom;

			public static int mtrl_btn_padding_left;

			public static int mtrl_btn_padding_right;

			public static int mtrl_btn_padding_top;

			public static int mtrl_btn_pressed_z;

			public static int mtrl_btn_stroke_size;

			public static int mtrl_btn_text_btn_icon_padding;

			public static int mtrl_btn_text_btn_padding_left;

			public static int mtrl_btn_text_btn_padding_right;

			public static int mtrl_btn_text_size;

			public static int mtrl_btn_z;

			public static int mtrl_calendar_action_height;

			public static int mtrl_calendar_action_padding;

			public static int mtrl_calendar_bottom_padding;

			public static int mtrl_calendar_content_padding;

			public static int mtrl_calendar_days_of_week_height;

			public static int mtrl_calendar_day_corner;

			public static int mtrl_calendar_day_height;

			public static int mtrl_calendar_day_horizontal_padding;

			public static int mtrl_calendar_day_today_stroke;

			public static int mtrl_calendar_day_vertical_padding;

			public static int mtrl_calendar_day_width;

			public static int mtrl_calendar_dialog_background_inset;

			public static int mtrl_calendar_header_content_padding;

			public static int mtrl_calendar_header_content_padding_fullscreen;

			public static int mtrl_calendar_header_divider_thickness;

			public static int mtrl_calendar_header_height;

			public static int mtrl_calendar_header_height_fullscreen;

			public static int mtrl_calendar_header_selection_line_height;

			public static int mtrl_calendar_header_text_padding;

			public static int mtrl_calendar_header_toggle_margin_bottom;

			public static int mtrl_calendar_header_toggle_margin_top;

			public static int mtrl_calendar_landscape_header_width;

			public static int mtrl_calendar_maximum_default_fullscreen_minor_axis;

			public static int mtrl_calendar_month_horizontal_padding;

			public static int mtrl_calendar_month_vertical_padding;

			public static int mtrl_calendar_navigation_bottom_padding;

			public static int mtrl_calendar_navigation_height;

			public static int mtrl_calendar_navigation_top_padding;

			public static int mtrl_calendar_pre_l_text_clip_padding;

			public static int mtrl_calendar_selection_baseline_to_top_fullscreen;

			public static int mtrl_calendar_selection_text_baseline_to_bottom;

			public static int mtrl_calendar_selection_text_baseline_to_bottom_fullscreen;

			public static int mtrl_calendar_selection_text_baseline_to_top;

			public static int mtrl_calendar_text_input_padding_top;

			public static int mtrl_calendar_title_baseline_to_top;

			public static int mtrl_calendar_title_baseline_to_top_fullscreen;

			public static int mtrl_calendar_year_corner;

			public static int mtrl_calendar_year_height;

			public static int mtrl_calendar_year_horizontal_padding;

			public static int mtrl_calendar_year_vertical_padding;

			public static int mtrl_calendar_year_width;

			public static int mtrl_card_checked_icon_margin;

			public static int mtrl_card_checked_icon_size;

			public static int mtrl_card_corner_radius;

			public static int mtrl_card_dragged_z;

			public static int mtrl_card_elevation;

			public static int mtrl_card_spacing;

			public static int mtrl_chip_pressed_translation_z;

			public static int mtrl_chip_text_size;

			public static int mtrl_exposed_dropdown_menu_popup_elevation;

			public static int mtrl_exposed_dropdown_menu_popup_vertical_offset;

			public static int mtrl_exposed_dropdown_menu_popup_vertical_padding;

			public static int mtrl_extended_fab_bottom_padding;

			public static int mtrl_extended_fab_corner_radius;

			public static int mtrl_extended_fab_disabled_elevation;

			public static int mtrl_extended_fab_disabled_translation_z;

			public static int mtrl_extended_fab_elevation;

			public static int mtrl_extended_fab_end_padding;

			public static int mtrl_extended_fab_end_padding_icon;

			public static int mtrl_extended_fab_icon_size;

			public static int mtrl_extended_fab_icon_text_spacing;

			public static int mtrl_extended_fab_min_height;

			public static int mtrl_extended_fab_min_width;

			public static int mtrl_extended_fab_start_padding;

			public static int mtrl_extended_fab_start_padding_icon;

			public static int mtrl_extended_fab_top_padding;

			public static int mtrl_extended_fab_translation_z_base;

			public static int mtrl_extended_fab_translation_z_hovered_focused;

			public static int mtrl_extended_fab_translation_z_pressed;

			public static int mtrl_fab_elevation;

			public static int mtrl_fab_min_touch_target;

			public static int mtrl_fab_translation_z_hovered_focused;

			public static int mtrl_fab_translation_z_pressed;

			public static int mtrl_high_ripple_default_alpha;

			public static int mtrl_high_ripple_focused_alpha;

			public static int mtrl_high_ripple_hovered_alpha;

			public static int mtrl_high_ripple_pressed_alpha;

			public static int mtrl_large_touch_target;

			public static int mtrl_low_ripple_default_alpha;

			public static int mtrl_low_ripple_focused_alpha;

			public static int mtrl_low_ripple_hovered_alpha;

			public static int mtrl_low_ripple_pressed_alpha;

			public static int mtrl_min_touch_target_size;

			public static int mtrl_navigation_elevation;

			public static int mtrl_navigation_item_horizontal_padding;

			public static int mtrl_navigation_item_icon_padding;

			public static int mtrl_navigation_item_icon_size;

			public static int mtrl_navigation_item_shape_horizontal_margin;

			public static int mtrl_navigation_item_shape_vertical_margin;

			public static int mtrl_shape_corner_size_large_component;

			public static int mtrl_shape_corner_size_medium_component;

			public static int mtrl_shape_corner_size_small_component;

			public static int mtrl_snackbar_action_text_color_alpha;

			public static int mtrl_snackbar_background_corner_radius;

			public static int mtrl_snackbar_background_overlay_color_alpha;

			public static int mtrl_snackbar_margin;

			public static int mtrl_switch_thumb_elevation;

			public static int mtrl_textinput_box_corner_radius_medium;

			public static int mtrl_textinput_box_corner_radius_small;

			public static int mtrl_textinput_box_label_cutout_padding;

			public static int mtrl_textinput_box_stroke_width_default;

			public static int mtrl_textinput_box_stroke_width_focused;

			public static int mtrl_textinput_end_icon_margin_start;

			public static int mtrl_textinput_outline_box_expanded_padding;

			public static int mtrl_textinput_start_icon_margin_end;

			public static int mtrl_toolbar_default_height;

			public static int notification_action_icon_size;

			public static int notification_action_text_size;

			public static int notification_big_circle_margin;

			public static int notification_content_margin_start;

			public static int notification_large_icon_height;

			public static int notification_large_icon_width;

			public static int notification_main_column_padding_top;

			public static int notification_media_narrow_margin;

			public static int notification_right_icon_size;

			public static int notification_right_side_padding_top;

			public static int notification_small_icon_background_padding;

			public static int notification_small_icon_size_as_large;

			public static int notification_subtext_size;

			public static int notification_top_pad;

			public static int notification_top_pad_large_text;

			public static int subtitle_corner_radius;

			public static int subtitle_outline_width;

			public static int subtitle_shadow_offset;

			public static int subtitle_shadow_radius;

			public static int test_mtrl_calendar_day_cornerSize;

			public static int tooltip_corner_radius;

			public static int tooltip_horizontal_padding;

			public static int tooltip_margin;

			public static int tooltip_precise_anchor_extra_offset;

			public static int tooltip_precise_anchor_threshold;

			public static int tooltip_vertical_padding;

			public static int tooltip_y_offset_non_touch;

			public static int tooltip_y_offset_touch;

			static Dimension()
			{
				abc_action_bar_content_inset_material = 2131099648;
				abc_action_bar_content_inset_with_nav = 2131099649;
				abc_action_bar_default_height_material = 2131099650;
				abc_action_bar_default_padding_end_material = 2131099651;
				abc_action_bar_default_padding_start_material = 2131099652;
				abc_action_bar_elevation_material = 2131099653;
				abc_action_bar_icon_vertical_padding_material = 2131099654;
				abc_action_bar_overflow_padding_end_material = 2131099655;
				abc_action_bar_overflow_padding_start_material = 2131099656;
				abc_action_bar_stacked_max_height = 2131099657;
				abc_action_bar_stacked_tab_max_width = 2131099658;
				abc_action_bar_subtitle_bottom_margin_material = 2131099659;
				abc_action_bar_subtitle_top_margin_material = 2131099660;
				abc_action_button_min_height_material = 2131099661;
				abc_action_button_min_width_material = 2131099662;
				abc_action_button_min_width_overflow_material = 2131099663;
				abc_alert_dialog_button_bar_height = 2131099664;
				abc_alert_dialog_button_dimen = 2131099665;
				abc_button_inset_horizontal_material = 2131099666;
				abc_button_inset_vertical_material = 2131099667;
				abc_button_padding_horizontal_material = 2131099668;
				abc_button_padding_vertical_material = 2131099669;
				abc_cascading_menus_min_smallest_width = 2131099670;
				abc_config_prefDialogWidth = 2131099671;
				abc_control_corner_material = 2131099672;
				abc_control_inset_material = 2131099673;
				abc_control_padding_material = 2131099674;
				abc_dialog_corner_radius_material = 2131099675;
				abc_dialog_fixed_height_major = 2131099676;
				abc_dialog_fixed_height_minor = 2131099677;
				abc_dialog_fixed_width_major = 2131099678;
				abc_dialog_fixed_width_minor = 2131099679;
				abc_dialog_list_padding_bottom_no_buttons = 2131099680;
				abc_dialog_list_padding_top_no_title = 2131099681;
				abc_dialog_min_width_major = 2131099682;
				abc_dialog_min_width_minor = 2131099683;
				abc_dialog_padding_material = 2131099684;
				abc_dialog_padding_top_material = 2131099685;
				abc_dialog_title_divider_material = 2131099686;
				abc_disabled_alpha_material_dark = 2131099687;
				abc_disabled_alpha_material_light = 2131099688;
				abc_dropdownitem_icon_width = 2131099689;
				abc_dropdownitem_text_padding_left = 2131099690;
				abc_dropdownitem_text_padding_right = 2131099691;
				abc_edit_text_inset_bottom_material = 2131099692;
				abc_edit_text_inset_horizontal_material = 2131099693;
				abc_edit_text_inset_top_material = 2131099694;
				abc_floating_window_z = 2131099695;
				abc_list_item_height_large_material = 2131099696;
				abc_list_item_height_material = 2131099697;
				abc_list_item_height_small_material = 2131099698;
				abc_list_item_padding_horizontal_material = 2131099699;
				abc_panel_menu_list_width = 2131099700;
				abc_progress_bar_height_material = 2131099701;
				abc_search_view_preferred_height = 2131099702;
				abc_search_view_preferred_width = 2131099703;
				abc_seekbar_track_background_height_material = 2131099704;
				abc_seekbar_track_progress_height_material = 2131099705;
				abc_select_dialog_padding_start_material = 2131099706;
				abc_switch_padding = 2131099707;
				abc_text_size_body_1_material = 2131099708;
				abc_text_size_body_2_material = 2131099709;
				abc_text_size_button_material = 2131099710;
				abc_text_size_caption_material = 2131099711;
				abc_text_size_display_1_material = 2131099712;
				abc_text_size_display_2_material = 2131099713;
				abc_text_size_display_3_material = 2131099714;
				abc_text_size_display_4_material = 2131099715;
				abc_text_size_headline_material = 2131099716;
				abc_text_size_large_material = 2131099717;
				abc_text_size_medium_material = 2131099718;
				abc_text_size_menu_header_material = 2131099719;
				abc_text_size_menu_material = 2131099720;
				abc_text_size_small_material = 2131099721;
				abc_text_size_subhead_material = 2131099722;
				abc_text_size_subtitle_material_toolbar = 2131099723;
				abc_text_size_title_material = 2131099724;
				abc_text_size_title_material_toolbar = 2131099725;
				action_bar_size = 2131099726;
				appcompat_dialog_background_inset = 2131099727;
				browser_actions_context_menu_max_width = 2131099728;
				browser_actions_context_menu_min_padding = 2131099729;
				cardview_compat_inset_shadow = 2131099730;
				cardview_default_elevation = 2131099731;
				cardview_default_radius = 2131099732;
				compat_button_inset_horizontal_material = 2131099733;
				compat_button_inset_vertical_material = 2131099734;
				compat_button_padding_horizontal_material = 2131099735;
				compat_button_padding_vertical_material = 2131099736;
				compat_control_corner_material = 2131099737;
				compat_notification_large_icon_max_height = 2131099738;
				compat_notification_large_icon_max_width = 2131099739;
				default_dimension = 2131099740;
				design_appbar_elevation = 2131099741;
				design_bottom_navigation_active_item_max_width = 2131099742;
				design_bottom_navigation_active_item_min_width = 2131099743;
				design_bottom_navigation_active_text_size = 2131099744;
				design_bottom_navigation_elevation = 2131099745;
				design_bottom_navigation_height = 2131099746;
				design_bottom_navigation_icon_size = 2131099747;
				design_bottom_navigation_item_max_width = 2131099748;
				design_bottom_navigation_item_min_width = 2131099749;
				design_bottom_navigation_margin = 2131099750;
				design_bottom_navigation_shadow_height = 2131099751;
				design_bottom_navigation_text_size = 2131099752;
				design_bottom_sheet_elevation = 2131099753;
				design_bottom_sheet_modal_elevation = 2131099754;
				design_bottom_sheet_peek_height_min = 2131099755;
				design_fab_border_width = 2131099756;
				design_fab_elevation = 2131099757;
				design_fab_image_size = 2131099758;
				design_fab_size_mini = 2131099759;
				design_fab_size_normal = 2131099760;
				design_fab_translation_z_hovered_focused = 2131099761;
				design_fab_translation_z_pressed = 2131099762;
				design_navigation_elevation = 2131099763;
				design_navigation_icon_padding = 2131099764;
				design_navigation_icon_size = 2131099765;
				design_navigation_item_horizontal_padding = 2131099766;
				design_navigation_item_icon_padding = 2131099767;
				design_navigation_max_width = 2131099768;
				design_navigation_padding_bottom = 2131099769;
				design_navigation_separator_vertical_padding = 2131099770;
				design_snackbar_action_inline_max_width = 2131099771;
				design_snackbar_action_text_color_alpha = 2131099772;
				design_snackbar_background_corner_radius = 2131099773;
				design_snackbar_elevation = 2131099774;
				design_snackbar_extra_spacing_horizontal = 2131099775;
				design_snackbar_max_width = 2131099776;
				design_snackbar_min_width = 2131099777;
				design_snackbar_padding_horizontal = 2131099778;
				design_snackbar_padding_vertical = 2131099779;
				design_snackbar_padding_vertical_2lines = 2131099780;
				design_snackbar_text_size = 2131099781;
				design_tab_max_width = 2131099782;
				design_tab_scrollable_min_width = 2131099783;
				design_tab_text_size = 2131099784;
				design_tab_text_size_2line = 2131099785;
				design_textinput_caption_translate_y = 2131099786;
				disabled_alpha_material_dark = 2131099787;
				disabled_alpha_material_light = 2131099788;
				fab_margin = 2131099789;
				fastscroll_default_thickness = 2131099790;
				fastscroll_margin = 2131099791;
				fastscroll_minimum_range = 2131099792;
				highlight_alpha_material_colored = 2131099793;
				highlight_alpha_material_dark = 2131099794;
				highlight_alpha_material_light = 2131099795;
				hint_alpha_material_dark = 2131099796;
				hint_alpha_material_light = 2131099797;
				hint_pressed_alpha_material_dark = 2131099798;
				hint_pressed_alpha_material_light = 2131099799;
				item_touch_helper_max_drag_scroll_per_frame = 2131099800;
				item_touch_helper_swipe_escape_max_velocity = 2131099801;
				item_touch_helper_swipe_escape_velocity = 2131099802;
				material_emphasis_disabled = 2131099803;
				material_emphasis_high_type = 2131099804;
				material_emphasis_medium = 2131099805;
				material_text_view_test_line_height = 2131099806;
				material_text_view_test_line_height_override = 2131099807;
				mtrl_alert_dialog_background_inset_bottom = 2131099808;
				mtrl_alert_dialog_background_inset_end = 2131099809;
				mtrl_alert_dialog_background_inset_start = 2131099810;
				mtrl_alert_dialog_background_inset_top = 2131099811;
				mtrl_alert_dialog_picker_background_inset = 2131099812;
				mtrl_badge_horizontal_edge_offset = 2131099813;
				mtrl_badge_long_text_horizontal_padding = 2131099814;
				mtrl_badge_radius = 2131099815;
				mtrl_badge_text_horizontal_edge_offset = 2131099816;
				mtrl_badge_text_size = 2131099817;
				mtrl_badge_with_text_radius = 2131099818;
				mtrl_bottomappbar_fabOffsetEndMode = 2131099819;
				mtrl_bottomappbar_fab_bottom_margin = 2131099820;
				mtrl_bottomappbar_fab_cradle_margin = 2131099821;
				mtrl_bottomappbar_fab_cradle_rounded_corner_radius = 2131099822;
				mtrl_bottomappbar_fab_cradle_vertical_offset = 2131099823;
				mtrl_bottomappbar_height = 2131099824;
				mtrl_btn_corner_radius = 2131099825;
				mtrl_btn_dialog_btn_min_width = 2131099826;
				mtrl_btn_disabled_elevation = 2131099827;
				mtrl_btn_disabled_z = 2131099828;
				mtrl_btn_elevation = 2131099829;
				mtrl_btn_focused_z = 2131099830;
				mtrl_btn_hovered_z = 2131099831;
				mtrl_btn_icon_btn_padding_left = 2131099832;
				mtrl_btn_icon_padding = 2131099833;
				mtrl_btn_inset = 2131099834;
				mtrl_btn_letter_spacing = 2131099835;
				mtrl_btn_padding_bottom = 2131099836;
				mtrl_btn_padding_left = 2131099837;
				mtrl_btn_padding_right = 2131099838;
				mtrl_btn_padding_top = 2131099839;
				mtrl_btn_pressed_z = 2131099840;
				mtrl_btn_stroke_size = 2131099841;
				mtrl_btn_text_btn_icon_padding = 2131099842;
				mtrl_btn_text_btn_padding_left = 2131099843;
				mtrl_btn_text_btn_padding_right = 2131099844;
				mtrl_btn_text_size = 2131099845;
				mtrl_btn_z = 2131099846;
				mtrl_calendar_action_height = 2131099847;
				mtrl_calendar_action_padding = 2131099848;
				mtrl_calendar_bottom_padding = 2131099849;
				mtrl_calendar_content_padding = 2131099850;
				mtrl_calendar_days_of_week_height = 2131099857;
				mtrl_calendar_day_corner = 2131099851;
				mtrl_calendar_day_height = 2131099852;
				mtrl_calendar_day_horizontal_padding = 2131099853;
				mtrl_calendar_day_today_stroke = 2131099854;
				mtrl_calendar_day_vertical_padding = 2131099855;
				mtrl_calendar_day_width = 2131099856;
				mtrl_calendar_dialog_background_inset = 2131099858;
				mtrl_calendar_header_content_padding = 2131099859;
				mtrl_calendar_header_content_padding_fullscreen = 2131099860;
				mtrl_calendar_header_divider_thickness = 2131099861;
				mtrl_calendar_header_height = 2131099862;
				mtrl_calendar_header_height_fullscreen = 2131099863;
				mtrl_calendar_header_selection_line_height = 2131099864;
				mtrl_calendar_header_text_padding = 2131099865;
				mtrl_calendar_header_toggle_margin_bottom = 2131099866;
				mtrl_calendar_header_toggle_margin_top = 2131099867;
				mtrl_calendar_landscape_header_width = 2131099868;
				mtrl_calendar_maximum_default_fullscreen_minor_axis = 2131099869;
				mtrl_calendar_month_horizontal_padding = 2131099870;
				mtrl_calendar_month_vertical_padding = 2131099871;
				mtrl_calendar_navigation_bottom_padding = 2131099872;
				mtrl_calendar_navigation_height = 2131099873;
				mtrl_calendar_navigation_top_padding = 2131099874;
				mtrl_calendar_pre_l_text_clip_padding = 2131099875;
				mtrl_calendar_selection_baseline_to_top_fullscreen = 2131099876;
				mtrl_calendar_selection_text_baseline_to_bottom = 2131099877;
				mtrl_calendar_selection_text_baseline_to_bottom_fullscreen = 2131099878;
				mtrl_calendar_selection_text_baseline_to_top = 2131099879;
				mtrl_calendar_text_input_padding_top = 2131099880;
				mtrl_calendar_title_baseline_to_top = 2131099881;
				mtrl_calendar_title_baseline_to_top_fullscreen = 2131099882;
				mtrl_calendar_year_corner = 2131099883;
				mtrl_calendar_year_height = 2131099884;
				mtrl_calendar_year_horizontal_padding = 2131099885;
				mtrl_calendar_year_vertical_padding = 2131099886;
				mtrl_calendar_year_width = 2131099887;
				mtrl_card_checked_icon_margin = 2131099888;
				mtrl_card_checked_icon_size = 2131099889;
				mtrl_card_corner_radius = 2131099890;
				mtrl_card_dragged_z = 2131099891;
				mtrl_card_elevation = 2131099892;
				mtrl_card_spacing = 2131099893;
				mtrl_chip_pressed_translation_z = 2131099894;
				mtrl_chip_text_size = 2131099895;
				mtrl_exposed_dropdown_menu_popup_elevation = 2131099896;
				mtrl_exposed_dropdown_menu_popup_vertical_offset = 2131099897;
				mtrl_exposed_dropdown_menu_popup_vertical_padding = 2131099898;
				mtrl_extended_fab_bottom_padding = 2131099899;
				mtrl_extended_fab_corner_radius = 2131099900;
				mtrl_extended_fab_disabled_elevation = 2131099901;
				mtrl_extended_fab_disabled_translation_z = 2131099902;
				mtrl_extended_fab_elevation = 2131099903;
				mtrl_extended_fab_end_padding = 2131099904;
				mtrl_extended_fab_end_padding_icon = 2131099905;
				mtrl_extended_fab_icon_size = 2131099906;
				mtrl_extended_fab_icon_text_spacing = 2131099907;
				mtrl_extended_fab_min_height = 2131099908;
				mtrl_extended_fab_min_width = 2131099909;
				mtrl_extended_fab_start_padding = 2131099910;
				mtrl_extended_fab_start_padding_icon = 2131099911;
				mtrl_extended_fab_top_padding = 2131099912;
				mtrl_extended_fab_translation_z_base = 2131099913;
				mtrl_extended_fab_translation_z_hovered_focused = 2131099914;
				mtrl_extended_fab_translation_z_pressed = 2131099915;
				mtrl_fab_elevation = 2131099916;
				mtrl_fab_min_touch_target = 2131099917;
				mtrl_fab_translation_z_hovered_focused = 2131099918;
				mtrl_fab_translation_z_pressed = 2131099919;
				mtrl_high_ripple_default_alpha = 2131099920;
				mtrl_high_ripple_focused_alpha = 2131099921;
				mtrl_high_ripple_hovered_alpha = 2131099922;
				mtrl_high_ripple_pressed_alpha = 2131099923;
				mtrl_large_touch_target = 2131099924;
				mtrl_low_ripple_default_alpha = 2131099925;
				mtrl_low_ripple_focused_alpha = 2131099926;
				mtrl_low_ripple_hovered_alpha = 2131099927;
				mtrl_low_ripple_pressed_alpha = 2131099928;
				mtrl_min_touch_target_size = 2131099929;
				mtrl_navigation_elevation = 2131099930;
				mtrl_navigation_item_horizontal_padding = 2131099931;
				mtrl_navigation_item_icon_padding = 2131099932;
				mtrl_navigation_item_icon_size = 2131099933;
				mtrl_navigation_item_shape_horizontal_margin = 2131099934;
				mtrl_navigation_item_shape_vertical_margin = 2131099935;
				mtrl_shape_corner_size_large_component = 2131099936;
				mtrl_shape_corner_size_medium_component = 2131099937;
				mtrl_shape_corner_size_small_component = 2131099938;
				mtrl_snackbar_action_text_color_alpha = 2131099939;
				mtrl_snackbar_background_corner_radius = 2131099940;
				mtrl_snackbar_background_overlay_color_alpha = 2131099941;
				mtrl_snackbar_margin = 2131099942;
				mtrl_switch_thumb_elevation = 2131099943;
				mtrl_textinput_box_corner_radius_medium = 2131099944;
				mtrl_textinput_box_corner_radius_small = 2131099945;
				mtrl_textinput_box_label_cutout_padding = 2131099946;
				mtrl_textinput_box_stroke_width_default = 2131099947;
				mtrl_textinput_box_stroke_width_focused = 2131099948;
				mtrl_textinput_end_icon_margin_start = 2131099949;
				mtrl_textinput_outline_box_expanded_padding = 2131099950;
				mtrl_textinput_start_icon_margin_end = 2131099951;
				mtrl_toolbar_default_height = 2131099952;
				notification_action_icon_size = 2131099953;
				notification_action_text_size = 2131099954;
				notification_big_circle_margin = 2131099955;
				notification_content_margin_start = 2131099956;
				notification_large_icon_height = 2131099957;
				notification_large_icon_width = 2131099958;
				notification_main_column_padding_top = 2131099959;
				notification_media_narrow_margin = 2131099960;
				notification_right_icon_size = 2131099961;
				notification_right_side_padding_top = 2131099962;
				notification_small_icon_background_padding = 2131099963;
				notification_small_icon_size_as_large = 2131099964;
				notification_subtext_size = 2131099965;
				notification_top_pad = 2131099966;
				notification_top_pad_large_text = 2131099967;
				subtitle_corner_radius = 2131099968;
				subtitle_outline_width = 2131099969;
				subtitle_shadow_offset = 2131099970;
				subtitle_shadow_radius = 2131099971;
				test_mtrl_calendar_day_cornerSize = 2131099972;
				tooltip_corner_radius = 2131099973;
				tooltip_horizontal_padding = 2131099974;
				tooltip_margin = 2131099975;
				tooltip_precise_anchor_extra_offset = 2131099976;
				tooltip_precise_anchor_threshold = 2131099977;
				tooltip_vertical_padding = 2131099978;
				tooltip_y_offset_non_touch = 2131099979;
				tooltip_y_offset_touch = 2131099980;
				ResourceIdManager.UpdateIdValues();
			}

			private Dimension()
			{
			}
		}

		public class Drawable
		{
			public static int abc_ab_share_pack_mtrl_alpha;

			public static int abc_action_bar_item_background_material;

			public static int abc_btn_borderless_material;

			public static int abc_btn_check_material;

			public static int abc_btn_check_material_anim;

			public static int abc_btn_check_to_on_mtrl_000;

			public static int abc_btn_check_to_on_mtrl_015;

			public static int abc_btn_colored_material;

			public static int abc_btn_default_mtrl_shape;

			public static int abc_btn_radio_material;

			public static int abc_btn_radio_material_anim;

			public static int abc_btn_radio_to_on_mtrl_000;

			public static int abc_btn_radio_to_on_mtrl_015;

			public static int abc_btn_switch_to_on_mtrl_00001;

			public static int abc_btn_switch_to_on_mtrl_00012;

			public static int abc_cab_background_internal_bg;

			public static int abc_cab_background_top_material;

			public static int abc_cab_background_top_mtrl_alpha;

			public static int abc_control_background_material;

			public static int abc_dialog_material_background;

			public static int abc_edit_text_material;

			public static int abc_ic_ab_back_material;

			public static int abc_ic_arrow_drop_right_black_24dp;

			public static int abc_ic_clear_material;

			public static int abc_ic_commit_search_api_mtrl_alpha;

			public static int abc_ic_go_search_api_material;

			public static int abc_ic_menu_copy_mtrl_am_alpha;

			public static int abc_ic_menu_cut_mtrl_alpha;

			public static int abc_ic_menu_overflow_material;

			public static int abc_ic_menu_paste_mtrl_am_alpha;

			public static int abc_ic_menu_selectall_mtrl_alpha;

			public static int abc_ic_menu_share_mtrl_alpha;

			public static int abc_ic_search_api_material;

			public static int abc_ic_star_black_16dp;

			public static int abc_ic_star_black_36dp;

			public static int abc_ic_star_black_48dp;

			public static int abc_ic_star_half_black_16dp;

			public static int abc_ic_star_half_black_36dp;

			public static int abc_ic_star_half_black_48dp;

			public static int abc_ic_voice_search_api_material;

			public static int abc_item_background_holo_dark;

			public static int abc_item_background_holo_light;

			public static int abc_list_divider_material;

			public static int abc_list_divider_mtrl_alpha;

			public static int abc_list_focused_holo;

			public static int abc_list_longpressed_holo;

			public static int abc_list_pressed_holo_dark;

			public static int abc_list_pressed_holo_light;

			public static int abc_list_selector_background_transition_holo_dark;

			public static int abc_list_selector_background_transition_holo_light;

			public static int abc_list_selector_disabled_holo_dark;

			public static int abc_list_selector_disabled_holo_light;

			public static int abc_list_selector_holo_dark;

			public static int abc_list_selector_holo_light;

			public static int abc_menu_hardkey_panel_mtrl_mult;

			public static int abc_popup_background_mtrl_mult;

			public static int abc_ratingbar_indicator_material;

			public static int abc_ratingbar_material;

			public static int abc_ratingbar_small_material;

			public static int abc_scrubber_control_off_mtrl_alpha;

			public static int abc_scrubber_control_to_pressed_mtrl_000;

			public static int abc_scrubber_control_to_pressed_mtrl_005;

			public static int abc_scrubber_primary_mtrl_alpha;

			public static int abc_scrubber_track_mtrl_alpha;

			public static int abc_seekbar_thumb_material;

			public static int abc_seekbar_tick_mark_material;

			public static int abc_seekbar_track_material;

			public static int abc_spinner_mtrl_am_alpha;

			public static int abc_spinner_textfield_background_material;

			public static int abc_switch_thumb_material;

			public static int abc_switch_track_mtrl_alpha;

			public static int abc_tab_indicator_material;

			public static int abc_tab_indicator_mtrl_alpha;

			public static int abc_textfield_activated_mtrl_alpha;

			public static int abc_textfield_default_mtrl_alpha;

			public static int abc_textfield_search_activated_mtrl_alpha;

			public static int abc_textfield_search_default_mtrl_alpha;

			public static int abc_textfield_search_material;

			public static int abc_text_cursor_material;

			public static int abc_text_select_handle_left_mtrl_dark;

			public static int abc_text_select_handle_left_mtrl_light;

			public static int abc_text_select_handle_middle_mtrl_dark;

			public static int abc_text_select_handle_middle_mtrl_light;

			public static int abc_text_select_handle_right_mtrl_dark;

			public static int abc_text_select_handle_right_mtrl_light;

			public static int abc_vector_test;

			public static int anonymus;

			public static int avd_hide_password;

			public static int avd_show_password;

			public static int bluetooth_icon;

			public static int btn_checkbox_checked_mtrl;

			public static int btn_checkbox_checked_to_unchecked_mtrl_animation;

			public static int btn_checkbox_unchecked_mtrl;

			public static int btn_checkbox_unchecked_to_checked_mtrl_animation;

			public static int btn_radio_off_mtrl;

			public static int btn_radio_off_to_on_mtrl_animation;

			public static int btn_radio_on_mtrl;

			public static int btn_radio_on_to_off_mtrl_animation;

			public static int bubble;

			public static int checkmark;

			public static int circle;

			public static int circle_greyed_out;

			public static int circle_textview;

			public static int color_gradient;

			public static int common_full_open_on_phone;

			public static int common_google_signin_btn_icon_dark;

			public static int common_google_signin_btn_icon_dark_focused;

			public static int common_google_signin_btn_icon_dark_normal;

			public static int common_google_signin_btn_icon_dark_normal_background;

			public static int common_google_signin_btn_icon_disabled;

			public static int common_google_signin_btn_icon_light;

			public static int common_google_signin_btn_icon_light_focused;

			public static int common_google_signin_btn_icon_light_normal;

			public static int common_google_signin_btn_icon_light_normal_background;

			public static int common_google_signin_btn_text_dark;

			public static int common_google_signin_btn_text_dark_focused;

			public static int common_google_signin_btn_text_dark_normal;

			public static int common_google_signin_btn_text_dark_normal_background;

			public static int common_google_signin_btn_text_disabled;

			public static int common_google_signin_btn_text_light;

			public static int common_google_signin_btn_text_light_focused;

			public static int common_google_signin_btn_text_light_normal;

			public static int common_google_signin_btn_text_light_normal_background;

			public static int counter_background;

			public static int default_button;

			public static int default_button_green;

			public static int default_button_no_border;

			public static int default_button_white;

			public static int default_dot;

			public static int design_bottom_navigation_item_background;

			public static int design_fab_background;

			public static int design_ic_visibility;

			public static int design_ic_visibility_off;

			public static int design_password_eye;

			public static int design_snackbar_background;

			public static int dotselector;

			public static int ellipse;

			public static int googleg_disabled_color_18;

			public static int googleg_standard_color_18;

			public static int gradientBackground;

			public static int health_department_logo;

			public static int ic_back_arrow;

			public static int ic_back_icon;

			public static int ic_calendar_black_24dp;

			public static int ic_clear_black_24dp;

			public static int ic_close_white;

			public static int ic_edit_black_24dp;

			public static int ic_help;

			public static int ic_information;

			public static int ic_keyboard_arrow_left_black_24dp;

			public static int ic_keyboard_arrow_right_black_24dp;

			public static int ic_logo_no_chain;

			public static int ic_menu_arrow_down_black_24dp;

			public static int ic_menu_arrow_up_black_24dp;

			public static int ic_mtrl_checked_circle;

			public static int ic_mtrl_chip_checked_black;

			public static int ic_mtrl_chip_checked_circle;

			public static int ic_mtrl_chip_close_circle;

			public static int ic_person;

			public static int ic_settings;

			public static int ic_smittestop;

			public static int ic_smittestop_small;

			public static int ic_sst_crown_white;

			public static int ic_start_logo;

			public static int menu;

			public static int mtrl_dialog_background;

			public static int mtrl_dropdown_arrow;

			public static int mtrl_ic_arrow_drop_down;

			public static int mtrl_ic_arrow_drop_up;

			public static int mtrl_ic_cancel;

			public static int mtrl_ic_error;

			public static int mtrl_popupmenu_background;

			public static int mtrl_popupmenu_background_dark;

			public static int mtrl_tabs_default_indicator;

			public static int navigation_empty_icon;

			public static int notification_action_background;

			public static int notification_bg;

			public static int notification_bg_low;

			public static int notification_bg_low_normal;

			public static int notification_bg_low_pressed;

			public static int notification_bg_normal;

			public static int notification_bg_normal_pressed;

			public static int notification_icon_background;

			public static int notification_template_icon_bg;

			public static int notification_template_icon_low_bg;

			public static int notification_tile_bg;

			public static int notify_panel_notification_icon_bg;

			public static int on_off_button;

			public static int on_off_button_green;

			public static int patient_logo;

			public static int rectangle;

			public static int selected_dot;

			public static int sundhedLogo;

			public static int technology_background;

			public static int test_custom_background;

			public static int thumb_selector;

			public static int tooltip_frame_dark;

			public static int tooltip_frame_light;

			public static int track_selector;

			public static int working_schema;

			static Drawable()
			{
				abc_ab_share_pack_mtrl_alpha = 2131165191;
				abc_action_bar_item_background_material = 2131165192;
				abc_btn_borderless_material = 2131165193;
				abc_btn_check_material = 2131165194;
				abc_btn_check_material_anim = 2131165195;
				abc_btn_check_to_on_mtrl_000 = 2131165196;
				abc_btn_check_to_on_mtrl_015 = 2131165197;
				abc_btn_colored_material = 2131165198;
				abc_btn_default_mtrl_shape = 2131165199;
				abc_btn_radio_material = 2131165200;
				abc_btn_radio_material_anim = 2131165201;
				abc_btn_radio_to_on_mtrl_000 = 2131165202;
				abc_btn_radio_to_on_mtrl_015 = 2131165203;
				abc_btn_switch_to_on_mtrl_00001 = 2131165204;
				abc_btn_switch_to_on_mtrl_00012 = 2131165205;
				abc_cab_background_internal_bg = 2131165206;
				abc_cab_background_top_material = 2131165207;
				abc_cab_background_top_mtrl_alpha = 2131165208;
				abc_control_background_material = 2131165209;
				abc_dialog_material_background = 2131165210;
				abc_edit_text_material = 2131165211;
				abc_ic_ab_back_material = 2131165212;
				abc_ic_arrow_drop_right_black_24dp = 2131165213;
				abc_ic_clear_material = 2131165214;
				abc_ic_commit_search_api_mtrl_alpha = 2131165215;
				abc_ic_go_search_api_material = 2131165216;
				abc_ic_menu_copy_mtrl_am_alpha = 2131165217;
				abc_ic_menu_cut_mtrl_alpha = 2131165218;
				abc_ic_menu_overflow_material = 2131165219;
				abc_ic_menu_paste_mtrl_am_alpha = 2131165220;
				abc_ic_menu_selectall_mtrl_alpha = 2131165221;
				abc_ic_menu_share_mtrl_alpha = 2131165222;
				abc_ic_search_api_material = 2131165223;
				abc_ic_star_black_16dp = 2131165224;
				abc_ic_star_black_36dp = 2131165225;
				abc_ic_star_black_48dp = 2131165226;
				abc_ic_star_half_black_16dp = 2131165227;
				abc_ic_star_half_black_36dp = 2131165228;
				abc_ic_star_half_black_48dp = 2131165229;
				abc_ic_voice_search_api_material = 2131165230;
				abc_item_background_holo_dark = 2131165231;
				abc_item_background_holo_light = 2131165232;
				abc_list_divider_material = 2131165233;
				abc_list_divider_mtrl_alpha = 2131165234;
				abc_list_focused_holo = 2131165235;
				abc_list_longpressed_holo = 2131165236;
				abc_list_pressed_holo_dark = 2131165237;
				abc_list_pressed_holo_light = 2131165238;
				abc_list_selector_background_transition_holo_dark = 2131165239;
				abc_list_selector_background_transition_holo_light = 2131165240;
				abc_list_selector_disabled_holo_dark = 2131165241;
				abc_list_selector_disabled_holo_light = 2131165242;
				abc_list_selector_holo_dark = 2131165243;
				abc_list_selector_holo_light = 2131165244;
				abc_menu_hardkey_panel_mtrl_mult = 2131165245;
				abc_popup_background_mtrl_mult = 2131165246;
				abc_ratingbar_indicator_material = 2131165247;
				abc_ratingbar_material = 2131165248;
				abc_ratingbar_small_material = 2131165249;
				abc_scrubber_control_off_mtrl_alpha = 2131165250;
				abc_scrubber_control_to_pressed_mtrl_000 = 2131165251;
				abc_scrubber_control_to_pressed_mtrl_005 = 2131165252;
				abc_scrubber_primary_mtrl_alpha = 2131165253;
				abc_scrubber_track_mtrl_alpha = 2131165254;
				abc_seekbar_thumb_material = 2131165255;
				abc_seekbar_tick_mark_material = 2131165256;
				abc_seekbar_track_material = 2131165257;
				abc_spinner_mtrl_am_alpha = 2131165258;
				abc_spinner_textfield_background_material = 2131165259;
				abc_switch_thumb_material = 2131165260;
				abc_switch_track_mtrl_alpha = 2131165261;
				abc_tab_indicator_material = 2131165262;
				abc_tab_indicator_mtrl_alpha = 2131165263;
				abc_textfield_activated_mtrl_alpha = 2131165271;
				abc_textfield_default_mtrl_alpha = 2131165272;
				abc_textfield_search_activated_mtrl_alpha = 2131165273;
				abc_textfield_search_default_mtrl_alpha = 2131165274;
				abc_textfield_search_material = 2131165275;
				abc_text_cursor_material = 2131165264;
				abc_text_select_handle_left_mtrl_dark = 2131165265;
				abc_text_select_handle_left_mtrl_light = 2131165266;
				abc_text_select_handle_middle_mtrl_dark = 2131165267;
				abc_text_select_handle_middle_mtrl_light = 2131165268;
				abc_text_select_handle_right_mtrl_dark = 2131165269;
				abc_text_select_handle_right_mtrl_light = 2131165270;
				abc_vector_test = 2131165276;
				anonymus = 2131165277;
				avd_hide_password = 2131165278;
				avd_show_password = 2131165279;
				bluetooth_icon = 2131165280;
				btn_checkbox_checked_mtrl = 2131165281;
				btn_checkbox_checked_to_unchecked_mtrl_animation = 2131165282;
				btn_checkbox_unchecked_mtrl = 2131165283;
				btn_checkbox_unchecked_to_checked_mtrl_animation = 2131165284;
				btn_radio_off_mtrl = 2131165285;
				btn_radio_off_to_on_mtrl_animation = 2131165286;
				btn_radio_on_mtrl = 2131165287;
				btn_radio_on_to_off_mtrl_animation = 2131165288;
				bubble = 2131165289;
				checkmark = 2131165290;
				circle = 2131165291;
				circle_greyed_out = 2131165292;
				circle_textview = 2131165293;
				color_gradient = 2131165294;
				common_full_open_on_phone = 2131165295;
				common_google_signin_btn_icon_dark = 2131165296;
				common_google_signin_btn_icon_dark_focused = 2131165297;
				common_google_signin_btn_icon_dark_normal = 2131165298;
				common_google_signin_btn_icon_dark_normal_background = 2131165299;
				common_google_signin_btn_icon_disabled = 2131165300;
				common_google_signin_btn_icon_light = 2131165301;
				common_google_signin_btn_icon_light_focused = 2131165302;
				common_google_signin_btn_icon_light_normal = 2131165303;
				common_google_signin_btn_icon_light_normal_background = 2131165304;
				common_google_signin_btn_text_dark = 2131165305;
				common_google_signin_btn_text_dark_focused = 2131165306;
				common_google_signin_btn_text_dark_normal = 2131165307;
				common_google_signin_btn_text_dark_normal_background = 2131165308;
				common_google_signin_btn_text_disabled = 2131165309;
				common_google_signin_btn_text_light = 2131165310;
				common_google_signin_btn_text_light_focused = 2131165311;
				common_google_signin_btn_text_light_normal = 2131165312;
				common_google_signin_btn_text_light_normal_background = 2131165313;
				counter_background = 2131165314;
				default_button = 2131165315;
				default_button_green = 2131165316;
				default_button_no_border = 2131165317;
				default_button_white = 2131165318;
				default_dot = 2131165319;
				design_bottom_navigation_item_background = 2131165320;
				design_fab_background = 2131165321;
				design_ic_visibility = 2131165322;
				design_ic_visibility_off = 2131165323;
				design_password_eye = 2131165324;
				design_snackbar_background = 2131165325;
				dotselector = 2131165326;
				ellipse = 2131165327;
				googleg_disabled_color_18 = 2131165328;
				googleg_standard_color_18 = 2131165329;
				gradientBackground = 2131165330;
				health_department_logo = 2131165331;
				ic_back_arrow = 2131165332;
				ic_back_icon = 2131165333;
				ic_calendar_black_24dp = 2131165334;
				ic_clear_black_24dp = 2131165335;
				ic_close_white = 2131165336;
				ic_edit_black_24dp = 2131165337;
				ic_help = 2131165338;
				ic_information = 2131165339;
				ic_keyboard_arrow_left_black_24dp = 2131165340;
				ic_keyboard_arrow_right_black_24dp = 2131165341;
				ic_logo_no_chain = 2131165342;
				ic_menu_arrow_down_black_24dp = 2131165343;
				ic_menu_arrow_up_black_24dp = 2131165344;
				ic_mtrl_checked_circle = 2131165345;
				ic_mtrl_chip_checked_black = 2131165346;
				ic_mtrl_chip_checked_circle = 2131165347;
				ic_mtrl_chip_close_circle = 2131165348;
				ic_person = 2131165349;
				ic_settings = 2131165350;
				ic_smittestop = 2131165351;
				ic_smittestop_small = 2131165352;
				ic_sst_crown_white = 2131165353;
				ic_start_logo = 2131165354;
				menu = 2131165355;
				mtrl_dialog_background = 2131165356;
				mtrl_dropdown_arrow = 2131165357;
				mtrl_ic_arrow_drop_down = 2131165358;
				mtrl_ic_arrow_drop_up = 2131165359;
				mtrl_ic_cancel = 2131165360;
				mtrl_ic_error = 2131165361;
				mtrl_popupmenu_background = 2131165362;
				mtrl_popupmenu_background_dark = 2131165363;
				mtrl_tabs_default_indicator = 2131165364;
				navigation_empty_icon = 2131165365;
				notification_action_background = 2131165366;
				notification_bg = 2131165367;
				notification_bg_low = 2131165368;
				notification_bg_low_normal = 2131165369;
				notification_bg_low_pressed = 2131165370;
				notification_bg_normal = 2131165371;
				notification_bg_normal_pressed = 2131165372;
				notification_icon_background = 2131165373;
				notification_template_icon_bg = 2131165374;
				notification_template_icon_low_bg = 2131165375;
				notification_tile_bg = 2131165376;
				notify_panel_notification_icon_bg = 2131165377;
				on_off_button = 2131165378;
				on_off_button_green = 2131165379;
				patient_logo = 2131165380;
				rectangle = 2131165381;
				selected_dot = 2131165382;
				sundhedLogo = 2131165383;
				technology_background = 2131165384;
				test_custom_background = 2131165385;
				thumb_selector = 2131165386;
				tooltip_frame_dark = 2131165387;
				tooltip_frame_light = 2131165388;
				track_selector = 2131165389;
				working_schema = 2131165390;
				ResourceIdManager.UpdateIdValues();
			}

			private Drawable()
			{
			}
		}

		public class Font
		{
			public static int IBMPlexSans;

			public static int ibmplexsans_bold;

			public static int ibmplexsans_bolditalic;

			public static int ibmplexsans_extralightitalic;

			public static int ibmplexsans_extralightt;

			public static int ibmplexsans_italic;

			public static int ibmplexsans_light;

			public static int ibmplexsans_lightitalic;

			public static int ibmplexsans_medium;

			public static int ibmplexsans_mediumitalic;

			public static int ibmplexsans_regular;

			public static int ibmplexsans_semibold;

			public static int ibmplexsans_semibolditalic;

			public static int ibmplexsans_thin;

			public static int ibmplexsans_thinitalic;

			public static int raleway;

			public static int raleway_black;

			public static int raleway_blackitalic;

			public static int raleway_bold;

			public static int raleway_bolditalic;

			public static int raleway_extrabold;

			public static int raleway_extrabolditalic;

			public static int raleway_extralight;

			public static int raleway_extralightitalic;

			public static int raleway_italic;

			public static int raleway_light;

			public static int raleway_lightitalic;

			public static int raleway_medium;

			public static int raleway_mediumitalic;

			public static int raleway_regular;

			public static int raleway_semibold;

			public static int raleway_semibolditalic;

			public static int raleway_thin;

			public static int raleway_thinitalic;

			static Font()
			{
				IBMPlexSans = 2131230720;
				ibmplexsans_bold = 2131230721;
				ibmplexsans_bolditalic = 2131230722;
				ibmplexsans_extralightitalic = 2131230723;
				ibmplexsans_extralightt = 2131230724;
				ibmplexsans_italic = 2131230725;
				ibmplexsans_light = 2131230726;
				ibmplexsans_lightitalic = 2131230727;
				ibmplexsans_medium = 2131230728;
				ibmplexsans_mediumitalic = 2131230729;
				ibmplexsans_regular = 2131230730;
				ibmplexsans_semibold = 2131230731;
				ibmplexsans_semibolditalic = 2131230732;
				ibmplexsans_thin = 2131230733;
				ibmplexsans_thinitalic = 2131230734;
				raleway = 2131230735;
				raleway_black = 2131230736;
				raleway_blackitalic = 2131230737;
				raleway_bold = 2131230738;
				raleway_bolditalic = 2131230739;
				raleway_extrabold = 2131230740;
				raleway_extrabolditalic = 2131230741;
				raleway_extralight = 2131230742;
				raleway_extralightitalic = 2131230743;
				raleway_italic = 2131230744;
				raleway_light = 2131230745;
				raleway_lightitalic = 2131230746;
				raleway_medium = 2131230747;
				raleway_mediumitalic = 2131230748;
				raleway_regular = 2131230749;
				raleway_semibold = 2131230750;
				raleway_semibolditalic = 2131230751;
				raleway_thin = 2131230752;
				raleway_thinitalic = 2131230753;
				ResourceIdManager.UpdateIdValues();
			}

			private Font()
			{
			}
		}

		public class Id
		{
			public static int accessibility_action_clickable_span;

			public static int accessibility_custom_action_0;

			public static int accessibility_custom_action_1;

			public static int accessibility_custom_action_10;

			public static int accessibility_custom_action_11;

			public static int accessibility_custom_action_12;

			public static int accessibility_custom_action_13;

			public static int accessibility_custom_action_14;

			public static int accessibility_custom_action_15;

			public static int accessibility_custom_action_16;

			public static int accessibility_custom_action_17;

			public static int accessibility_custom_action_18;

			public static int accessibility_custom_action_19;

			public static int accessibility_custom_action_2;

			public static int accessibility_custom_action_20;

			public static int accessibility_custom_action_21;

			public static int accessibility_custom_action_22;

			public static int accessibility_custom_action_23;

			public static int accessibility_custom_action_24;

			public static int accessibility_custom_action_25;

			public static int accessibility_custom_action_26;

			public static int accessibility_custom_action_27;

			public static int accessibility_custom_action_28;

			public static int accessibility_custom_action_29;

			public static int accessibility_custom_action_3;

			public static int accessibility_custom_action_30;

			public static int accessibility_custom_action_31;

			public static int accessibility_custom_action_4;

			public static int accessibility_custom_action_5;

			public static int accessibility_custom_action_6;

			public static int accessibility_custom_action_7;

			public static int accessibility_custom_action_8;

			public static int accessibility_custom_action_9;

			public static int action0;

			public static int actions;

			public static int action_bar;

			public static int action_bar_activity_content;

			public static int action_bar_container;

			public static int action_bar_root;

			public static int action_bar_spinner;

			public static int action_bar_subtitle;

			public static int action_bar_title;

			public static int action_container;

			public static int action_context_bar;

			public static int action_divider;

			public static int action_image;

			public static int action_menu_divider;

			public static int action_menu_presenter;

			public static int action_mode_bar;

			public static int action_mode_bar_stub;

			public static int action_mode_close_button;

			public static int action_text;

			public static int activity_chooser_view_content;

			public static int add;

			public static int adjust_height;

			public static int adjust_width;

			public static int alertTitle;

			public static int all;

			public static int ALT;

			public static int always;

			public static int arrow_back;

			public static int arrow_back_1;

			public static int arrow_back_1_view;

			public static int arrow_back_about;

			public static int arrow_back_help;

			public static int async;

			public static int auto;

			public static int barrier;

			public static int beginning;

			public static int blocking;

			public static int bottom;

			public static int BOTTOM_END;

			public static int BOTTOM_START;

			public static int browser_actions_header_text;

			public static int browser_actions_menu_items;

			public static int browser_actions_menu_item_icon;

			public static int browser_actions_menu_item_text;

			public static int browser_actions_menu_view;

			public static int bubble_layout;

			public static int bubble_message;

			public static int buttonBubble;

			public static int buttonPanel;

			public static int buttonResetConsents;

			public static int cancel_action;

			public static int cancel_button;

			public static int center;

			public static int center_horizontal;

			public static int center_vertical;

			public static int chains;

			public static int checkbox;

			public static int @checked;

			public static int chip;

			public static int chip_group;

			public static int chronometer;

			public static int clear_text;

			public static int clip_horizontal;

			public static int clip_vertical;

			public static int collapseActionView;

			public static int confirm_button;

			public static int consentActivityIndicator;

			public static int consent_info;

			public static int consent_info_view;

			public static int consent_page_text;

			public static int consent_page_title;

			public static int consent_paragraph_aendringer;

			public static int consent_paragraph_behandlingen;

			public static int consent_paragraph_frivillig_brug;

			public static int consent_paragraph_hvad_registreres;

			public static int consent_paragraph_hvordan_accepterer;

			public static int consent_paragraph_kontaktregistringer;

			public static int consent_paragraph_mere;

			public static int consent_paragraph_policy_btn;

			public static int consent_paragraph_ret;

			public static int consent_paragraph_sadan_fungerer_appen;

			public static int container;

			public static int content;

			public static int contentPanel;

			public static int coordinator;

			public static int CTRL;

			public static int custom;

			public static int customPanel;

			public static int cut;

			public static int dark;

			public static int date_picker_actions;

			public static int decor_content_parent;

			public static int default_activity_button;

			public static int design_bottom_sheet;

			public static int design_menu_item_action_area;

			public static int design_menu_item_action_area_stub;

			public static int design_menu_item_text;

			public static int design_navigation_view;

			public static int dialog_button;

			public static int dimensions;

			public static int direct;

			public static int disableHome;

			public static int dropdown_menu;

			public static int edit_query;

			public static int end;

			public static int end_padder;

			public static int enterAlways;

			public static int enterAlwaysCollapsed;

			public static int exitUntilCollapsed;

			public static int expanded_menu;

			public static int expand_activities_button;

			public static int fab;

			public static int fade;

			public static int fill;

			public static int filled;

			public static int fill_horizontal;

			public static int fill_vertical;

			public static int filter_chip;

			public static int fitToContents;

			public static int @fixed;

			public static int force_update_button;

			public static int force_update_label;

			public static int forever;

			public static int fragment_container_view_tag;

			public static int FUNCTION;

			public static int ghost_view;

			public static int ghost_view_holder;

			public static int gone;

			public static int groups;

			public static int group_divider;

			public static int guideline;

			public static int guideline_about_left;

			public static int guideline_about_right;

			public static int guideline_help_left;

			public static int guideline_help_right;

			public static int guideline_left;

			public static int guideline_right;

			public static int hideable;

			public static int home;

			public static int homeAsUp;

			public static int icon;

			public static int icon_group;

			public static int icon_only;

			public static int ic_close_white;

			public static int ic_start_logo;

			public static int ifRoom;

			public static int image;

			public static int info;

			public static int invisible;

			public static int italic;

			public static int item_touch_helper_previous_elevation;

			public static int labeled;

			public static int largeLabel;

			public static int launcer_icon_imageview;

			public static int launcher_button;

			public static int left;

			public static int light;

			public static int line1;

			public static int line3;

			public static int listMode;

			public static int list_item;

			public static int masked;

			public static int media_actions;

			public static int message;

			public static int META;

			public static int middle;

			public static int mini;

			public static int month_grid;

			public static int month_navigation_bar;

			public static int month_navigation_fragment_toggle;

			public static int month_navigation_next;

			public static int month_navigation_previous;

			public static int month_title;

			public static int mtrl_calendar_days_of_week;

			public static int mtrl_calendar_day_selector_frame;

			public static int mtrl_calendar_frame;

			public static int mtrl_calendar_main_pane;

			public static int mtrl_calendar_months;

			public static int mtrl_calendar_selection_frame;

			public static int mtrl_calendar_text_input_frame;

			public static int mtrl_calendar_year_selector_frame;

			public static int mtrl_card_checked_layer_id;

			public static int mtrl_child_content_container;

			public static int mtrl_internal_children_alpha_tag;

			public static int mtrl_picker_fullscreen;

			public static int mtrl_picker_header;

			public static int mtrl_picker_header_selection_text;

			public static int mtrl_picker_header_title_and_selection;

			public static int mtrl_picker_header_toggle;

			public static int mtrl_picker_text_input_date;

			public static int mtrl_picker_text_input_range_end;

			public static int mtrl_picker_text_input_range_start;

			public static int mtrl_picker_title_text;

			public static int multiply;

			public static int navigation_header_container;

			public static int never;

			public static int none;

			public static int normal;

			public static int noScroll;

			public static int notification_background;

			public static int notification_main_column;

			public static int notification_main_column_container;

			public static int off;

			public static int om_frame;

			public static int on;

			public static int outline;

			public static int packed;

			public static int parallax;

			public static int parent;

			public static int parentPanel;

			public static int parent_matrix;

			public static int password_toggle;

			public static int peekHeight;

			public static int percent;

			public static int pin;

			public static int progress_circular;

			public static int progress_horizontal;

			public static int radio;

			public static int right;

			public static int right_icon;

			public static int right_side;

			public static int rounded;

			public static int save_non_transition_alpha;

			public static int save_overlay_view;

			public static int scale;

			public static int screen;

			public static int scroll;

			public static int scrollable;

			public static int scrollIndicatorDown;

			public static int scrollIndicatorUp;

			public static int scrollView;

			public static int search_badge;

			public static int search_bar;

			public static int search_button;

			public static int search_close_btn;

			public static int search_edit_frame;

			public static int search_go_btn;

			public static int search_mag_icon;

			public static int search_plate;

			public static int search_src_text;

			public static int search_voice_btn;

			public static int selected;

			public static int select_dialog_listview;

			public static int settings_about_link;

			public static int settings_about_scroll_layout;

			public static int settings_about_text;

			public static int settings_about_text_layout;

			public static int settings_about_title;

			public static int settings_about_version_info_textview;

			public static int settings_behandling_frame;

			public static int settings_consents_layout;

			public static int settings_general_text;

			public static int settings_general_text_layout;

			public static int settings_general_title;

			public static int settings_help_link;

			public static int settings_help_scroll_layout;

			public static int settings_help_text;

			public static int settings_help_text_layout;

			public static int settings_help_title;

			public static int settings_hjaelp_frame;

			public static int settings_intro_frame;

			public static int settings_links_layout;

			public static int settings_link_text;

			public static int settings_saddan_frame;

			public static int settings_scroll_frame;

			public static int settings_scroll_help_frame;

			public static int settings_version_info_textview;

			public static int SHIFT;

			public static int shortcut;

			public static int showCustom;

			public static int showHome;

			public static int showTitle;

			public static int skipCollapsed;

			public static int slide;

			public static int smallLabel;

			public static int snackbar_action;

			public static int snackbar_text;

			public static int snap;

			public static int snapMargins;

			public static int spacer;

			public static int split_action_bar;

			public static int spread;

			public static int spread_inside;

			public static int src_atop;

			public static int src_in;

			public static int src_over;

			public static int standard;

			public static int start;

			public static int status_bar_latest_event_content;

			public static int stretch;

			public static int submenuarrow;

			public static int submit_area;

			public static int SYM;

			public static int tabMode;

			public static int tag_accessibility_actions;

			public static int tag_accessibility_clickable_spans;

			public static int tag_accessibility_heading;

			public static int tag_accessibility_pane_title;

			public static int tag_screen_reader_focusable;

			public static int tag_transition_group;

			public static int tag_unhandled_key_event_manager;

			public static int tag_unhandled_key_listeners;

			public static int test_checkbox_android_button_tint;

			public static int test_checkbox_app_button_tint;

			public static int test_frame;

			public static int text;

			public static int text2;

			public static int textEnd;

			public static int textinput_counter;

			public static int textinput_error;

			public static int textinput_helper_text;

			public static int textSpacerNoButtons;

			public static int textSpacerNoTitle;

			public static int textStart;

			public static int text_input_end_icon;

			public static int text_input_start_icon;

			public static int time;

			public static int title;

			public static int titleDividerNoCustom;

			public static int title_template;

			public static int top;

			public static int topPanel;

			public static int TOP_END;

			public static int TOP_START;

			public static int touch_outside;

			public static int transition_current_scene;

			public static int transition_layout_save;

			public static int transition_position;

			public static int transition_scene_layoutid_cache;

			public static int transition_transform;

			public static int @unchecked;

			public static int uniform;

			public static int unlabeled;

			public static int up;

			public static int useLogo;

			public static int view_offset_helper;

			public static int visible;

			public static int visible_removing_fragment_view_tag;

			public static int wide;

			public static int withText;

			public static int wrap;

			public static int wrap_content;

			static Id()
			{
				accessibility_action_clickable_span = 2131296266;
				accessibility_custom_action_0 = 2131296267;
				accessibility_custom_action_1 = 2131296268;
				accessibility_custom_action_10 = 2131296269;
				accessibility_custom_action_11 = 2131296270;
				accessibility_custom_action_12 = 2131296271;
				accessibility_custom_action_13 = 2131296272;
				accessibility_custom_action_14 = 2131296273;
				accessibility_custom_action_15 = 2131296274;
				accessibility_custom_action_16 = 2131296275;
				accessibility_custom_action_17 = 2131296276;
				accessibility_custom_action_18 = 2131296277;
				accessibility_custom_action_19 = 2131296278;
				accessibility_custom_action_2 = 2131296279;
				accessibility_custom_action_20 = 2131296280;
				accessibility_custom_action_21 = 2131296281;
				accessibility_custom_action_22 = 2131296282;
				accessibility_custom_action_23 = 2131296283;
				accessibility_custom_action_24 = 2131296284;
				accessibility_custom_action_25 = 2131296285;
				accessibility_custom_action_26 = 2131296286;
				accessibility_custom_action_27 = 2131296287;
				accessibility_custom_action_28 = 2131296288;
				accessibility_custom_action_29 = 2131296289;
				accessibility_custom_action_3 = 2131296290;
				accessibility_custom_action_30 = 2131296291;
				accessibility_custom_action_31 = 2131296292;
				accessibility_custom_action_4 = 2131296293;
				accessibility_custom_action_5 = 2131296294;
				accessibility_custom_action_6 = 2131296295;
				accessibility_custom_action_7 = 2131296296;
				accessibility_custom_action_8 = 2131296297;
				accessibility_custom_action_9 = 2131296298;
				action0 = 2131296299;
				actions = 2131296317;
				action_bar = 2131296300;
				action_bar_activity_content = 2131296301;
				action_bar_container = 2131296302;
				action_bar_root = 2131296303;
				action_bar_spinner = 2131296304;
				action_bar_subtitle = 2131296305;
				action_bar_title = 2131296306;
				action_container = 2131296307;
				action_context_bar = 2131296308;
				action_divider = 2131296309;
				action_image = 2131296310;
				action_menu_divider = 2131296311;
				action_menu_presenter = 2131296312;
				action_mode_bar = 2131296313;
				action_mode_bar_stub = 2131296314;
				action_mode_close_button = 2131296315;
				action_text = 2131296316;
				activity_chooser_view_content = 2131296318;
				add = 2131296319;
				adjust_height = 2131296320;
				adjust_width = 2131296321;
				alertTitle = 2131296322;
				all = 2131296323;
				ALT = 2131296256;
				always = 2131296324;
				arrow_back = 2131296325;
				arrow_back_1 = 2131296326;
				arrow_back_1_view = 2131296327;
				arrow_back_about = 2131296328;
				arrow_back_help = 2131296329;
				async = 2131296330;
				auto = 2131296331;
				barrier = 2131296332;
				beginning = 2131296333;
				blocking = 2131296334;
				bottom = 2131296335;
				BOTTOM_END = 2131296257;
				BOTTOM_START = 2131296258;
				browser_actions_header_text = 2131296336;
				browser_actions_menu_items = 2131296339;
				browser_actions_menu_item_icon = 2131296337;
				browser_actions_menu_item_text = 2131296338;
				browser_actions_menu_view = 2131296340;
				bubble_layout = 2131296341;
				bubble_message = 2131296342;
				buttonBubble = 2131296343;
				buttonPanel = 2131296344;
				buttonResetConsents = 2131296345;
				cancel_action = 2131296346;
				cancel_button = 2131296347;
				center = 2131296348;
				center_horizontal = 2131296349;
				center_vertical = 2131296350;
				chains = 2131296351;
				checkbox = 2131296352;
				@checked = 2131296353;
				chip = 2131296354;
				chip_group = 2131296355;
				chronometer = 2131296356;
				clear_text = 2131296357;
				clip_horizontal = 2131296358;
				clip_vertical = 2131296359;
				collapseActionView = 2131296360;
				confirm_button = 2131296361;
				consentActivityIndicator = 2131296362;
				consent_info = 2131296363;
				consent_info_view = 2131296364;
				consent_page_text = 2131296365;
				consent_page_title = 2131296366;
				consent_paragraph_aendringer = 2131296367;
				consent_paragraph_behandlingen = 2131296368;
				consent_paragraph_frivillig_brug = 2131296369;
				consent_paragraph_hvad_registreres = 2131296370;
				consent_paragraph_hvordan_accepterer = 2131296371;
				consent_paragraph_kontaktregistringer = 2131296372;
				consent_paragraph_mere = 2131296373;
				consent_paragraph_policy_btn = 2131296374;
				consent_paragraph_ret = 2131296375;
				consent_paragraph_sadan_fungerer_appen = 2131296376;
				container = 2131296377;
				content = 2131296378;
				contentPanel = 2131296379;
				coordinator = 2131296380;
				CTRL = 2131296259;
				custom = 2131296381;
				customPanel = 2131296382;
				cut = 2131296383;
				dark = 2131296384;
				date_picker_actions = 2131296385;
				decor_content_parent = 2131296386;
				default_activity_button = 2131296387;
				design_bottom_sheet = 2131296388;
				design_menu_item_action_area = 2131296389;
				design_menu_item_action_area_stub = 2131296390;
				design_menu_item_text = 2131296391;
				design_navigation_view = 2131296392;
				dialog_button = 2131296393;
				dimensions = 2131296394;
				direct = 2131296395;
				disableHome = 2131296396;
				dropdown_menu = 2131296397;
				edit_query = 2131296398;
				end = 2131296399;
				end_padder = 2131296400;
				enterAlways = 2131296401;
				enterAlwaysCollapsed = 2131296402;
				exitUntilCollapsed = 2131296403;
				expanded_menu = 2131296405;
				expand_activities_button = 2131296404;
				fab = 2131296406;
				fade = 2131296407;
				fill = 2131296408;
				filled = 2131296411;
				fill_horizontal = 2131296409;
				fill_vertical = 2131296410;
				filter_chip = 2131296412;
				fitToContents = 2131296413;
				@fixed = 2131296414;
				force_update_button = 2131296415;
				force_update_label = 2131296416;
				forever = 2131296417;
				fragment_container_view_tag = 2131296418;
				FUNCTION = 2131296260;
				ghost_view = 2131296419;
				ghost_view_holder = 2131296420;
				gone = 2131296421;
				groups = 2131296423;
				group_divider = 2131296422;
				guideline = 2131296424;
				guideline_about_left = 2131296425;
				guideline_about_right = 2131296426;
				guideline_help_left = 2131296427;
				guideline_help_right = 2131296428;
				guideline_left = 2131296429;
				guideline_right = 2131296430;
				hideable = 2131296431;
				home = 2131296432;
				homeAsUp = 2131296433;
				icon = 2131296436;
				icon_group = 2131296437;
				icon_only = 2131296438;
				ic_close_white = 2131296434;
				ic_start_logo = 2131296435;
				ifRoom = 2131296439;
				image = 2131296440;
				info = 2131296441;
				invisible = 2131296442;
				italic = 2131296443;
				item_touch_helper_previous_elevation = 2131296444;
				labeled = 2131296445;
				largeLabel = 2131296446;
				launcer_icon_imageview = 2131296447;
				launcher_button = 2131296448;
				left = 2131296449;
				light = 2131296450;
				line1 = 2131296451;
				line3 = 2131296452;
				listMode = 2131296453;
				list_item = 2131296454;
				masked = 2131296455;
				media_actions = 2131296456;
				message = 2131296457;
				META = 2131296261;
				middle = 2131296458;
				mini = 2131296459;
				month_grid = 2131296460;
				month_navigation_bar = 2131296461;
				month_navigation_fragment_toggle = 2131296462;
				month_navigation_next = 2131296463;
				month_navigation_previous = 2131296464;
				month_title = 2131296465;
				mtrl_calendar_days_of_week = 2131296467;
				mtrl_calendar_day_selector_frame = 2131296466;
				mtrl_calendar_frame = 2131296468;
				mtrl_calendar_main_pane = 2131296469;
				mtrl_calendar_months = 2131296470;
				mtrl_calendar_selection_frame = 2131296471;
				mtrl_calendar_text_input_frame = 2131296472;
				mtrl_calendar_year_selector_frame = 2131296473;
				mtrl_card_checked_layer_id = 2131296474;
				mtrl_child_content_container = 2131296475;
				mtrl_internal_children_alpha_tag = 2131296476;
				mtrl_picker_fullscreen = 2131296477;
				mtrl_picker_header = 2131296478;
				mtrl_picker_header_selection_text = 2131296479;
				mtrl_picker_header_title_and_selection = 2131296480;
				mtrl_picker_header_toggle = 2131296481;
				mtrl_picker_text_input_date = 2131296482;
				mtrl_picker_text_input_range_end = 2131296483;
				mtrl_picker_text_input_range_start = 2131296484;
				mtrl_picker_title_text = 2131296485;
				multiply = 2131296486;
				navigation_header_container = 2131296487;
				never = 2131296488;
				none = 2131296490;
				normal = 2131296491;
				noScroll = 2131296489;
				notification_background = 2131296492;
				notification_main_column = 2131296493;
				notification_main_column_container = 2131296494;
				off = 2131296495;
				om_frame = 2131296496;
				on = 2131296497;
				outline = 2131296498;
				packed = 2131296499;
				parallax = 2131296500;
				parent = 2131296501;
				parentPanel = 2131296502;
				parent_matrix = 2131296503;
				password_toggle = 2131296504;
				peekHeight = 2131296505;
				percent = 2131296506;
				pin = 2131296507;
				progress_circular = 2131296508;
				progress_horizontal = 2131296509;
				radio = 2131296510;
				right = 2131296511;
				right_icon = 2131296512;
				right_side = 2131296513;
				rounded = 2131296514;
				save_non_transition_alpha = 2131296515;
				save_overlay_view = 2131296516;
				scale = 2131296517;
				screen = 2131296518;
				scroll = 2131296519;
				scrollable = 2131296523;
				scrollIndicatorDown = 2131296520;
				scrollIndicatorUp = 2131296521;
				scrollView = 2131296522;
				search_badge = 2131296524;
				search_bar = 2131296525;
				search_button = 2131296526;
				search_close_btn = 2131296527;
				search_edit_frame = 2131296528;
				search_go_btn = 2131296529;
				search_mag_icon = 2131296530;
				search_plate = 2131296531;
				search_src_text = 2131296532;
				search_voice_btn = 2131296533;
				selected = 2131296535;
				select_dialog_listview = 2131296534;
				settings_about_link = 2131296536;
				settings_about_scroll_layout = 2131296537;
				settings_about_text = 2131296538;
				settings_about_text_layout = 2131296539;
				settings_about_title = 2131296540;
				settings_about_version_info_textview = 2131296541;
				settings_behandling_frame = 2131296542;
				settings_consents_layout = 2131296543;
				settings_general_text = 2131296544;
				settings_general_text_layout = 2131296545;
				settings_general_title = 2131296546;
				settings_help_link = 2131296547;
				settings_help_scroll_layout = 2131296548;
				settings_help_text = 2131296549;
				settings_help_text_layout = 2131296550;
				settings_help_title = 2131296551;
				settings_hjaelp_frame = 2131296552;
				settings_intro_frame = 2131296553;
				settings_links_layout = 2131296555;
				settings_link_text = 2131296554;
				settings_saddan_frame = 2131296556;
				settings_scroll_frame = 2131296557;
				settings_scroll_help_frame = 2131296558;
				settings_version_info_textview = 2131296559;
				SHIFT = 2131296262;
				shortcut = 2131296560;
				showCustom = 2131296561;
				showHome = 2131296562;
				showTitle = 2131296563;
				skipCollapsed = 2131296564;
				slide = 2131296565;
				smallLabel = 2131296566;
				snackbar_action = 2131296567;
				snackbar_text = 2131296568;
				snap = 2131296569;
				snapMargins = 2131296570;
				spacer = 2131296571;
				split_action_bar = 2131296572;
				spread = 2131296573;
				spread_inside = 2131296574;
				src_atop = 2131296575;
				src_in = 2131296576;
				src_over = 2131296577;
				standard = 2131296578;
				start = 2131296579;
				status_bar_latest_event_content = 2131296580;
				stretch = 2131296581;
				submenuarrow = 2131296582;
				submit_area = 2131296583;
				SYM = 2131296263;
				tabMode = 2131296584;
				tag_accessibility_actions = 2131296585;
				tag_accessibility_clickable_spans = 2131296586;
				tag_accessibility_heading = 2131296587;
				tag_accessibility_pane_title = 2131296588;
				tag_screen_reader_focusable = 2131296589;
				tag_transition_group = 2131296590;
				tag_unhandled_key_event_manager = 2131296591;
				tag_unhandled_key_listeners = 2131296592;
				test_checkbox_android_button_tint = 2131296593;
				test_checkbox_app_button_tint = 2131296594;
				test_frame = 2131296595;
				text = 2131296596;
				text2 = 2131296597;
				textEnd = 2131296598;
				textinput_counter = 2131296604;
				textinput_error = 2131296605;
				textinput_helper_text = 2131296606;
				textSpacerNoButtons = 2131296599;
				textSpacerNoTitle = 2131296600;
				textStart = 2131296601;
				text_input_end_icon = 2131296602;
				text_input_start_icon = 2131296603;
				time = 2131296607;
				title = 2131296608;
				titleDividerNoCustom = 2131296609;
				title_template = 2131296610;
				top = 2131296611;
				topPanel = 2131296612;
				TOP_END = 2131296264;
				TOP_START = 2131296265;
				touch_outside = 2131296613;
				transition_current_scene = 2131296614;
				transition_layout_save = 2131296615;
				transition_position = 2131296616;
				transition_scene_layoutid_cache = 2131296617;
				transition_transform = 2131296618;
				@unchecked = 2131296619;
				uniform = 2131296620;
				unlabeled = 2131296621;
				up = 2131296622;
				useLogo = 2131296623;
				view_offset_helper = 2131296624;
				visible = 2131296625;
				visible_removing_fragment_view_tag = 2131296626;
				wide = 2131296627;
				withText = 2131296628;
				wrap = 2131296629;
				wrap_content = 2131296630;
				ResourceIdManager.UpdateIdValues();
			}

			private Id()
			{
			}
		}

		public class Integer
		{
			public static int abc_config_activityDefaultDur;

			public static int abc_config_activityShortDur;

			public static int app_bar_elevation_anim_duration;

			public static int bottom_sheet_slide_duration;

			public static int cancel_button_image_alpha;

			public static int config_tooltipAnimTime;

			public static int design_snackbar_text_max_lines;

			public static int design_tab_indicator_anim_duration_ms;

			public static int google_play_services_version;

			public static int hide_password_duration;

			public static int mtrl_badge_max_character_count;

			public static int mtrl_btn_anim_delay_ms;

			public static int mtrl_btn_anim_duration_ms;

			public static int mtrl_calendar_header_orientation;

			public static int mtrl_calendar_selection_text_lines;

			public static int mtrl_calendar_year_selector_span;

			public static int mtrl_card_anim_delay_ms;

			public static int mtrl_card_anim_duration_ms;

			public static int mtrl_chip_anim_duration;

			public static int mtrl_tab_indicator_anim_duration_ms;

			public static int show_password_duration;

			public static int status_bar_notification_info_maxnum;

			static Integer()
			{
				abc_config_activityDefaultDur = 2131361792;
				abc_config_activityShortDur = 2131361793;
				app_bar_elevation_anim_duration = 2131361794;
				bottom_sheet_slide_duration = 2131361795;
				cancel_button_image_alpha = 2131361796;
				config_tooltipAnimTime = 2131361797;
				design_snackbar_text_max_lines = 2131361798;
				design_tab_indicator_anim_duration_ms = 2131361799;
				google_play_services_version = 2131361800;
				hide_password_duration = 2131361801;
				mtrl_badge_max_character_count = 2131361802;
				mtrl_btn_anim_delay_ms = 2131361803;
				mtrl_btn_anim_duration_ms = 2131361804;
				mtrl_calendar_header_orientation = 2131361805;
				mtrl_calendar_selection_text_lines = 2131361806;
				mtrl_calendar_year_selector_span = 2131361807;
				mtrl_card_anim_delay_ms = 2131361808;
				mtrl_card_anim_duration_ms = 2131361809;
				mtrl_chip_anim_duration = 2131361810;
				mtrl_tab_indicator_anim_duration_ms = 2131361811;
				show_password_duration = 2131361812;
				status_bar_notification_info_maxnum = 2131361813;
				ResourceIdManager.UpdateIdValues();
			}

			private Integer()
			{
			}
		}

		public class Interpolator
		{
			public static int btn_checkbox_checked_mtrl_animation_interpolator_0;

			public static int btn_checkbox_checked_mtrl_animation_interpolator_1;

			public static int btn_checkbox_unchecked_mtrl_animation_interpolator_0;

			public static int btn_checkbox_unchecked_mtrl_animation_interpolator_1;

			public static int btn_radio_to_off_mtrl_animation_interpolator_0;

			public static int btn_radio_to_on_mtrl_animation_interpolator_0;

			public static int fast_out_slow_in;

			public static int mtrl_fast_out_linear_in;

			public static int mtrl_fast_out_slow_in;

			public static int mtrl_linear;

			public static int mtrl_linear_out_slow_in;

			static Interpolator()
			{
				btn_checkbox_checked_mtrl_animation_interpolator_0 = 2131427328;
				btn_checkbox_checked_mtrl_animation_interpolator_1 = 2131427329;
				btn_checkbox_unchecked_mtrl_animation_interpolator_0 = 2131427330;
				btn_checkbox_unchecked_mtrl_animation_interpolator_1 = 2131427331;
				btn_radio_to_off_mtrl_animation_interpolator_0 = 2131427332;
				btn_radio_to_on_mtrl_animation_interpolator_0 = 2131427333;
				fast_out_slow_in = 2131427334;
				mtrl_fast_out_linear_in = 2131427335;
				mtrl_fast_out_slow_in = 2131427336;
				mtrl_linear = 2131427337;
				mtrl_linear_out_slow_in = 2131427338;
				ResourceIdManager.UpdateIdValues();
			}

			private Interpolator()
			{
			}
		}

		public class Layout
		{
			public static int abc_action_bar_title_item;

			public static int abc_action_bar_up_container;

			public static int abc_action_menu_item_layout;

			public static int abc_action_menu_layout;

			public static int abc_action_mode_bar;

			public static int abc_action_mode_close_item_material;

			public static int abc_activity_chooser_view;

			public static int abc_activity_chooser_view_list_item;

			public static int abc_alert_dialog_button_bar_material;

			public static int abc_alert_dialog_material;

			public static int abc_alert_dialog_title_material;

			public static int abc_cascading_menu_item_layout;

			public static int abc_dialog_title_material;

			public static int abc_expanded_menu_layout;

			public static int abc_list_menu_item_checkbox;

			public static int abc_list_menu_item_icon;

			public static int abc_list_menu_item_layout;

			public static int abc_list_menu_item_radio;

			public static int abc_popup_menu_header_item_layout;

			public static int abc_popup_menu_item_layout;

			public static int abc_screen_content_include;

			public static int abc_screen_simple;

			public static int abc_screen_simple_overlay_action_mode;

			public static int abc_screen_toolbar;

			public static int abc_search_dropdown_item_icons_2line;

			public static int abc_search_view;

			public static int abc_select_dialog_material;

			public static int abc_tooltip;

			public static int activity_main;

			public static int browser_actions_context_menu_page;

			public static int browser_actions_context_menu_row;

			public static int bubble_layout;

			public static int consent_info;

			public static int consent_paragraph;

			public static int consent_settings_page_body;

			public static int content_main;

			public static int custom_dialog;

			public static int design_bottom_navigation_item;

			public static int design_bottom_sheet_dialog;

			public static int design_layout_snackbar;

			public static int design_layout_snackbar_include;

			public static int design_layout_tab_icon;

			public static int design_layout_tab_text;

			public static int design_menu_item_action_area;

			public static int design_navigation_item;

			public static int design_navigation_item_header;

			public static int design_navigation_item_separator;

			public static int design_navigation_item_subheader;

			public static int design_navigation_menu;

			public static int design_navigation_menu_item;

			public static int design_text_input_end_icon;

			public static int design_text_input_start_icon;

			public static int force_update;

			public static int layout_with_launcher_button;

			public static int mtrl_alert_dialog;

			public static int mtrl_alert_dialog_actions;

			public static int mtrl_alert_dialog_title;

			public static int mtrl_alert_select_dialog_item;

			public static int mtrl_alert_select_dialog_multichoice;

			public static int mtrl_alert_select_dialog_singlechoice;

			public static int mtrl_calendar_day;

			public static int mtrl_calendar_days_of_week;

			public static int mtrl_calendar_day_of_week;

			public static int mtrl_calendar_horizontal;

			public static int mtrl_calendar_month;

			public static int mtrl_calendar_months;

			public static int mtrl_calendar_month_labeled;

			public static int mtrl_calendar_month_navigation;

			public static int mtrl_calendar_vertical;

			public static int mtrl_calendar_year;

			public static int mtrl_layout_snackbar;

			public static int mtrl_layout_snackbar_include;

			public static int mtrl_picker_actions;

			public static int mtrl_picker_dialog;

			public static int mtrl_picker_fullscreen;

			public static int mtrl_picker_header_dialog;

			public static int mtrl_picker_header_fullscreen;

			public static int mtrl_picker_header_selection_text;

			public static int mtrl_picker_header_title_text;

			public static int mtrl_picker_header_toggle;

			public static int mtrl_picker_text_input_date;

			public static int mtrl_picker_text_input_date_range;

			public static int notification_action;

			public static int notification_action_tombstone;

			public static int notification_media_action;

			public static int notification_media_cancel_action;

			public static int notification_template_big_media;

			public static int notification_template_big_media_custom;

			public static int notification_template_big_media_narrow;

			public static int notification_template_big_media_narrow_custom;

			public static int notification_template_custom_big;

			public static int notification_template_icon_group;

			public static int notification_template_lines_media;

			public static int notification_template_media;

			public static int notification_template_media_custom;

			public static int notification_template_part_chronometer;

			public static int notification_template_part_time;

			public static int select_dialog_item_material;

			public static int select_dialog_multichoice_material;

			public static int select_dialog_singlechoice_material;

			public static int settings_about;

			public static int settings_about_scroll;

			public static int settings_consents;

			public static int settings_general_page;

			public static int settings_help;

			public static int settings_help_scroll;

			public static int settings_link;

			public static int settings_page;

			public static int support_simple_spinner_dropdown_item;

			public static int test_action_chip;

			public static int test_design_checkbox;

			public static int test_reflow_chipgroup;

			public static int test_toolbar;

			public static int test_toolbar_custom_background;

			public static int test_toolbar_elevation;

			public static int test_toolbar_surface;

			public static int text_view_without_line_height;

			public static int text_view_with_line_height_from_appearance;

			public static int text_view_with_line_height_from_layout;

			public static int text_view_with_line_height_from_style;

			public static int text_view_with_theme_line_height;

			static Layout()
			{
				abc_action_bar_title_item = 2131492864;
				abc_action_bar_up_container = 2131492865;
				abc_action_menu_item_layout = 2131492866;
				abc_action_menu_layout = 2131492867;
				abc_action_mode_bar = 2131492868;
				abc_action_mode_close_item_material = 2131492869;
				abc_activity_chooser_view = 2131492870;
				abc_activity_chooser_view_list_item = 2131492871;
				abc_alert_dialog_button_bar_material = 2131492872;
				abc_alert_dialog_material = 2131492873;
				abc_alert_dialog_title_material = 2131492874;
				abc_cascading_menu_item_layout = 2131492875;
				abc_dialog_title_material = 2131492876;
				abc_expanded_menu_layout = 2131492877;
				abc_list_menu_item_checkbox = 2131492878;
				abc_list_menu_item_icon = 2131492879;
				abc_list_menu_item_layout = 2131492880;
				abc_list_menu_item_radio = 2131492881;
				abc_popup_menu_header_item_layout = 2131492882;
				abc_popup_menu_item_layout = 2131492883;
				abc_screen_content_include = 2131492884;
				abc_screen_simple = 2131492885;
				abc_screen_simple_overlay_action_mode = 2131492886;
				abc_screen_toolbar = 2131492887;
				abc_search_dropdown_item_icons_2line = 2131492888;
				abc_search_view = 2131492889;
				abc_select_dialog_material = 2131492890;
				abc_tooltip = 2131492891;
				activity_main = 2131492892;
				browser_actions_context_menu_page = 2131492893;
				browser_actions_context_menu_row = 2131492894;
				bubble_layout = 2131492895;
				consent_info = 2131492896;
				consent_paragraph = 2131492897;
				consent_settings_page_body = 2131492898;
				content_main = 2131492899;
				custom_dialog = 2131492900;
				design_bottom_navigation_item = 2131492901;
				design_bottom_sheet_dialog = 2131492902;
				design_layout_snackbar = 2131492903;
				design_layout_snackbar_include = 2131492904;
				design_layout_tab_icon = 2131492905;
				design_layout_tab_text = 2131492906;
				design_menu_item_action_area = 2131492907;
				design_navigation_item = 2131492908;
				design_navigation_item_header = 2131492909;
				design_navigation_item_separator = 2131492910;
				design_navigation_item_subheader = 2131492911;
				design_navigation_menu = 2131492912;
				design_navigation_menu_item = 2131492913;
				design_text_input_end_icon = 2131492914;
				design_text_input_start_icon = 2131492915;
				force_update = 2131492916;
				layout_with_launcher_button = 2131492917;
				mtrl_alert_dialog = 2131492918;
				mtrl_alert_dialog_actions = 2131492919;
				mtrl_alert_dialog_title = 2131492920;
				mtrl_alert_select_dialog_item = 2131492921;
				mtrl_alert_select_dialog_multichoice = 2131492922;
				mtrl_alert_select_dialog_singlechoice = 2131492923;
				mtrl_calendar_day = 2131492924;
				mtrl_calendar_days_of_week = 2131492926;
				mtrl_calendar_day_of_week = 2131492925;
				mtrl_calendar_horizontal = 2131492927;
				mtrl_calendar_month = 2131492928;
				mtrl_calendar_months = 2131492931;
				mtrl_calendar_month_labeled = 2131492929;
				mtrl_calendar_month_navigation = 2131492930;
				mtrl_calendar_vertical = 2131492932;
				mtrl_calendar_year = 2131492933;
				mtrl_layout_snackbar = 2131492934;
				mtrl_layout_snackbar_include = 2131492935;
				mtrl_picker_actions = 2131492936;
				mtrl_picker_dialog = 2131492937;
				mtrl_picker_fullscreen = 2131492938;
				mtrl_picker_header_dialog = 2131492939;
				mtrl_picker_header_fullscreen = 2131492940;
				mtrl_picker_header_selection_text = 2131492941;
				mtrl_picker_header_title_text = 2131492942;
				mtrl_picker_header_toggle = 2131492943;
				mtrl_picker_text_input_date = 2131492944;
				mtrl_picker_text_input_date_range = 2131492945;
				notification_action = 2131492946;
				notification_action_tombstone = 2131492947;
				notification_media_action = 2131492948;
				notification_media_cancel_action = 2131492949;
				notification_template_big_media = 2131492950;
				notification_template_big_media_custom = 2131492951;
				notification_template_big_media_narrow = 2131492952;
				notification_template_big_media_narrow_custom = 2131492953;
				notification_template_custom_big = 2131492954;
				notification_template_icon_group = 2131492955;
				notification_template_lines_media = 2131492956;
				notification_template_media = 2131492957;
				notification_template_media_custom = 2131492958;
				notification_template_part_chronometer = 2131492959;
				notification_template_part_time = 2131492960;
				select_dialog_item_material = 2131492961;
				select_dialog_multichoice_material = 2131492962;
				select_dialog_singlechoice_material = 2131492963;
				settings_about = 2131492964;
				settings_about_scroll = 2131492965;
				settings_consents = 2131492966;
				settings_general_page = 2131492967;
				settings_help = 2131492968;
				settings_help_scroll = 2131492969;
				settings_link = 2131492970;
				settings_page = 2131492971;
				support_simple_spinner_dropdown_item = 2131492972;
				test_action_chip = 2131492973;
				test_design_checkbox = 2131492974;
				test_reflow_chipgroup = 2131492975;
				test_toolbar = 2131492976;
				test_toolbar_custom_background = 2131492977;
				test_toolbar_elevation = 2131492978;
				test_toolbar_surface = 2131492979;
				text_view_without_line_height = 2131492984;
				text_view_with_line_height_from_appearance = 2131492980;
				text_view_with_line_height_from_layout = 2131492981;
				text_view_with_line_height_from_style = 2131492982;
				text_view_with_theme_line_height = 2131492983;
				ResourceIdManager.UpdateIdValues();
			}

			private Layout()
			{
			}
		}

		public class Plurals
		{
			public static int mtrl_badge_content_description;

			static Plurals()
			{
				mtrl_badge_content_description = 2131558400;
				ResourceIdManager.UpdateIdValues();
			}

			private Plurals()
			{
			}
		}

		public class String
		{
			public static int abc_action_bar_home_description;

			public static int abc_action_bar_up_description;

			public static int abc_action_menu_overflow_description;

			public static int abc_action_mode_done;

			public static int abc_activitychooserview_choose_application;

			public static int abc_activity_chooser_view_see_all;

			public static int abc_capital_off;

			public static int abc_capital_on;

			public static int abc_menu_alt_shortcut_label;

			public static int abc_menu_ctrl_shortcut_label;

			public static int abc_menu_delete_shortcut_label;

			public static int abc_menu_enter_shortcut_label;

			public static int abc_menu_function_shortcut_label;

			public static int abc_menu_meta_shortcut_label;

			public static int abc_menu_shift_shortcut_label;

			public static int abc_menu_space_shortcut_label;

			public static int abc_menu_sym_shortcut_label;

			public static int abc_prepend_shortcut_label;

			public static int abc_searchview_description_clear;

			public static int abc_searchview_description_query;

			public static int abc_searchview_description_search;

			public static int abc_searchview_description_submit;

			public static int abc_searchview_description_voice;

			public static int abc_search_hint;

			public static int abc_shareactionprovider_share_with;

			public static int abc_shareactionprovider_share_with_application;

			public static int abc_toolbar_collapse_description;

			public static int appbar_scrolling_view_behavior;

			public static int bottom_sheet_behavior;

			public static int character_counter_content_description;

			public static int character_counter_overflowed_content_description;

			public static int character_counter_pattern;

			public static int chip_text;

			public static int clear_text_end_icon_content_description;

			public static int common_google_play_services_enable_button;

			public static int common_google_play_services_enable_text;

			public static int common_google_play_services_enable_title;

			public static int common_google_play_services_install_button;

			public static int common_google_play_services_install_text;

			public static int common_google_play_services_install_title;

			public static int common_google_play_services_notification_channel_name;

			public static int common_google_play_services_notification_ticker;

			public static int common_google_play_services_unknown_issue;

			public static int common_google_play_services_unsupported_text;

			public static int common_google_play_services_update_button;

			public static int common_google_play_services_update_text;

			public static int common_google_play_services_update_title;

			public static int common_google_play_services_updating_text;

			public static int common_google_play_services_wear_update_text;

			public static int common_open_on_phone;

			public static int common_signin_button_text;

			public static int common_signin_button_text_long;

			public static int copy_toast_msg;

			public static int error_icon_content_description;

			public static int exposed_dropdown_menu_content_description;

			public static int fab_transformation_scrim_behavior;

			public static int fab_transformation_sheet_behavior;

			public static int fallback_menu_item_copy_link;

			public static int fallback_menu_item_open_in_browser;

			public static int fallback_menu_item_share_link;

			public static int hide_bottom_view_on_scroll_behavior;

			public static int icon_content_description;

			public static int mtrl_badge_numberless_content_description;

			public static int mtrl_chip_close_icon_content_description;

			public static int mtrl_exceed_max_badge_number_suffix;

			public static int mtrl_picker_a11y_next_month;

			public static int mtrl_picker_a11y_prev_month;

			public static int mtrl_picker_announce_current_selection;

			public static int mtrl_picker_cancel;

			public static int mtrl_picker_confirm;

			public static int mtrl_picker_date_header_selected;

			public static int mtrl_picker_date_header_title;

			public static int mtrl_picker_date_header_unselected;

			public static int mtrl_picker_day_of_week_column_header;

			public static int mtrl_picker_invalid_format;

			public static int mtrl_picker_invalid_format_example;

			public static int mtrl_picker_invalid_format_use;

			public static int mtrl_picker_invalid_range;

			public static int mtrl_picker_navigate_to_year_description;

			public static int mtrl_picker_out_of_range;

			public static int mtrl_picker_range_header_only_end_selected;

			public static int mtrl_picker_range_header_only_start_selected;

			public static int mtrl_picker_range_header_selected;

			public static int mtrl_picker_range_header_title;

			public static int mtrl_picker_range_header_unselected;

			public static int mtrl_picker_save;

			public static int mtrl_picker_text_input_date_hint;

			public static int mtrl_picker_text_input_date_range_end_hint;

			public static int mtrl_picker_text_input_date_range_start_hint;

			public static int mtrl_picker_text_input_day_abbr;

			public static int mtrl_picker_text_input_month_abbr;

			public static int mtrl_picker_text_input_year_abbr;

			public static int mtrl_picker_toggle_to_calendar_input_mode;

			public static int mtrl_picker_toggle_to_day_selection;

			public static int mtrl_picker_toggle_to_text_input_mode;

			public static int mtrl_picker_toggle_to_year_selection;

			public static int password_toggle_content_description;

			public static int path_password_eye;

			public static int path_password_eye_mask_strike_through;

			public static int path_password_eye_mask_visible;

			public static int path_password_strike_through;

			public static int search_menu_title;

			public static int status_bar_notification_info_overflow;

			static String()
			{
				abc_action_bar_home_description = 2131623936;
				abc_action_bar_up_description = 2131623937;
				abc_action_menu_overflow_description = 2131623938;
				abc_action_mode_done = 2131623939;
				abc_activitychooserview_choose_application = 2131623941;
				abc_activity_chooser_view_see_all = 2131623940;
				abc_capital_off = 2131623942;
				abc_capital_on = 2131623943;
				abc_menu_alt_shortcut_label = 2131623944;
				abc_menu_ctrl_shortcut_label = 2131623945;
				abc_menu_delete_shortcut_label = 2131623946;
				abc_menu_enter_shortcut_label = 2131623947;
				abc_menu_function_shortcut_label = 2131623948;
				abc_menu_meta_shortcut_label = 2131623949;
				abc_menu_shift_shortcut_label = 2131623950;
				abc_menu_space_shortcut_label = 2131623951;
				abc_menu_sym_shortcut_label = 2131623952;
				abc_prepend_shortcut_label = 2131623953;
				abc_searchview_description_clear = 2131623955;
				abc_searchview_description_query = 2131623956;
				abc_searchview_description_search = 2131623957;
				abc_searchview_description_submit = 2131623958;
				abc_searchview_description_voice = 2131623959;
				abc_search_hint = 2131623954;
				abc_shareactionprovider_share_with = 2131623960;
				abc_shareactionprovider_share_with_application = 2131623961;
				abc_toolbar_collapse_description = 2131623962;
				appbar_scrolling_view_behavior = 2131623963;
				bottom_sheet_behavior = 2131623964;
				character_counter_content_description = 2131623965;
				character_counter_overflowed_content_description = 2131623966;
				character_counter_pattern = 2131623967;
				chip_text = 2131623968;
				clear_text_end_icon_content_description = 2131623969;
				common_google_play_services_enable_button = 2131623970;
				common_google_play_services_enable_text = 2131623971;
				common_google_play_services_enable_title = 2131623972;
				common_google_play_services_install_button = 2131623973;
				common_google_play_services_install_text = 2131623974;
				common_google_play_services_install_title = 2131623975;
				common_google_play_services_notification_channel_name = 2131623976;
				common_google_play_services_notification_ticker = 2131623977;
				common_google_play_services_unknown_issue = 2131623978;
				common_google_play_services_unsupported_text = 2131623979;
				common_google_play_services_update_button = 2131623980;
				common_google_play_services_update_text = 2131623981;
				common_google_play_services_update_title = 2131623982;
				common_google_play_services_updating_text = 2131623983;
				common_google_play_services_wear_update_text = 2131623984;
				common_open_on_phone = 2131623985;
				common_signin_button_text = 2131623986;
				common_signin_button_text_long = 2131623987;
				copy_toast_msg = 2131623988;
				error_icon_content_description = 2131623989;
				exposed_dropdown_menu_content_description = 2131623990;
				fab_transformation_scrim_behavior = 2131623991;
				fab_transformation_sheet_behavior = 2131623992;
				fallback_menu_item_copy_link = 2131623993;
				fallback_menu_item_open_in_browser = 2131623994;
				fallback_menu_item_share_link = 2131623995;
				hide_bottom_view_on_scroll_behavior = 2131623996;
				icon_content_description = 2131623997;
				mtrl_badge_numberless_content_description = 2131623998;
				mtrl_chip_close_icon_content_description = 2131623999;
				mtrl_exceed_max_badge_number_suffix = 2131624000;
				mtrl_picker_a11y_next_month = 2131624001;
				mtrl_picker_a11y_prev_month = 2131624002;
				mtrl_picker_announce_current_selection = 2131624003;
				mtrl_picker_cancel = 2131624004;
				mtrl_picker_confirm = 2131624005;
				mtrl_picker_date_header_selected = 2131624006;
				mtrl_picker_date_header_title = 2131624007;
				mtrl_picker_date_header_unselected = 2131624008;
				mtrl_picker_day_of_week_column_header = 2131624009;
				mtrl_picker_invalid_format = 2131624010;
				mtrl_picker_invalid_format_example = 2131624011;
				mtrl_picker_invalid_format_use = 2131624012;
				mtrl_picker_invalid_range = 2131624013;
				mtrl_picker_navigate_to_year_description = 2131624014;
				mtrl_picker_out_of_range = 2131624015;
				mtrl_picker_range_header_only_end_selected = 2131624016;
				mtrl_picker_range_header_only_start_selected = 2131624017;
				mtrl_picker_range_header_selected = 2131624018;
				mtrl_picker_range_header_title = 2131624019;
				mtrl_picker_range_header_unselected = 2131624020;
				mtrl_picker_save = 2131624021;
				mtrl_picker_text_input_date_hint = 2131624022;
				mtrl_picker_text_input_date_range_end_hint = 2131624023;
				mtrl_picker_text_input_date_range_start_hint = 2131624024;
				mtrl_picker_text_input_day_abbr = 2131624025;
				mtrl_picker_text_input_month_abbr = 2131624026;
				mtrl_picker_text_input_year_abbr = 2131624027;
				mtrl_picker_toggle_to_calendar_input_mode = 2131624028;
				mtrl_picker_toggle_to_day_selection = 2131624029;
				mtrl_picker_toggle_to_text_input_mode = 2131624030;
				mtrl_picker_toggle_to_year_selection = 2131624031;
				password_toggle_content_description = 2131624032;
				path_password_eye = 2131624033;
				path_password_eye_mask_strike_through = 2131624034;
				path_password_eye_mask_visible = 2131624035;
				path_password_strike_through = 2131624036;
				search_menu_title = 2131624037;
				status_bar_notification_info_overflow = 2131624038;
				ResourceIdManager.UpdateIdValues();
			}

			private String()
			{
			}
		}

		public class Style
		{
			public static int AlertDialog_AppCompat;

			public static int AlertDialog_AppCompat_Light;

			public static int Animation_AppCompat_Dialog;

			public static int Animation_AppCompat_DropDownUp;

			public static int Animation_AppCompat_Tooltip;

			public static int Animation_Design_BottomSheetDialog;

			public static int Animation_MaterialComponents_BottomSheetDialog;

			public static int AppTheme;

			public static int AppTheme_AppBarOverlay;

			public static int AppTheme_Launcher;

			public static int AppTheme_PopupOverlay;

			public static int Base_AlertDialog_AppCompat;

			public static int Base_AlertDialog_AppCompat_Light;

			public static int Base_Animation_AppCompat_Dialog;

			public static int Base_Animation_AppCompat_DropDownUp;

			public static int Base_Animation_AppCompat_Tooltip;

			public static int Base_CardView;

			public static int Base_DialogWindowTitleBackground_AppCompat;

			public static int Base_DialogWindowTitle_AppCompat;

			public static int Base_MaterialAlertDialog_MaterialComponents_Title_Icon;

			public static int Base_MaterialAlertDialog_MaterialComponents_Title_Panel;

			public static int Base_MaterialAlertDialog_MaterialComponents_Title_Text;

			public static int Base_TextAppearance_AppCompat;

			public static int Base_TextAppearance_AppCompat_Body1;

			public static int Base_TextAppearance_AppCompat_Body2;

			public static int Base_TextAppearance_AppCompat_Button;

			public static int Base_TextAppearance_AppCompat_Caption;

			public static int Base_TextAppearance_AppCompat_Display1;

			public static int Base_TextAppearance_AppCompat_Display2;

			public static int Base_TextAppearance_AppCompat_Display3;

			public static int Base_TextAppearance_AppCompat_Display4;

			public static int Base_TextAppearance_AppCompat_Headline;

			public static int Base_TextAppearance_AppCompat_Inverse;

			public static int Base_TextAppearance_AppCompat_Large;

			public static int Base_TextAppearance_AppCompat_Large_Inverse;

			public static int Base_TextAppearance_AppCompat_Light_Widget_PopupMenu_Large;

			public static int Base_TextAppearance_AppCompat_Light_Widget_PopupMenu_Small;

			public static int Base_TextAppearance_AppCompat_Medium;

			public static int Base_TextAppearance_AppCompat_Medium_Inverse;

			public static int Base_TextAppearance_AppCompat_Menu;

			public static int Base_TextAppearance_AppCompat_SearchResult;

			public static int Base_TextAppearance_AppCompat_SearchResult_Subtitle;

			public static int Base_TextAppearance_AppCompat_SearchResult_Title;

			public static int Base_TextAppearance_AppCompat_Small;

			public static int Base_TextAppearance_AppCompat_Small_Inverse;

			public static int Base_TextAppearance_AppCompat_Subhead;

			public static int Base_TextAppearance_AppCompat_Subhead_Inverse;

			public static int Base_TextAppearance_AppCompat_Title;

			public static int Base_TextAppearance_AppCompat_Title_Inverse;

			public static int Base_TextAppearance_AppCompat_Tooltip;

			public static int Base_TextAppearance_AppCompat_Widget_ActionBar_Menu;

			public static int Base_TextAppearance_AppCompat_Widget_ActionBar_Subtitle;

			public static int Base_TextAppearance_AppCompat_Widget_ActionBar_Subtitle_Inverse;

			public static int Base_TextAppearance_AppCompat_Widget_ActionBar_Title;

			public static int Base_TextAppearance_AppCompat_Widget_ActionBar_Title_Inverse;

			public static int Base_TextAppearance_AppCompat_Widget_ActionMode_Subtitle;

			public static int Base_TextAppearance_AppCompat_Widget_ActionMode_Title;

			public static int Base_TextAppearance_AppCompat_Widget_Button;

			public static int Base_TextAppearance_AppCompat_Widget_Button_Borderless_Colored;

			public static int Base_TextAppearance_AppCompat_Widget_Button_Colored;

			public static int Base_TextAppearance_AppCompat_Widget_Button_Inverse;

			public static int Base_TextAppearance_AppCompat_Widget_DropDownItem;

			public static int Base_TextAppearance_AppCompat_Widget_PopupMenu_Header;

			public static int Base_TextAppearance_AppCompat_Widget_PopupMenu_Large;

			public static int Base_TextAppearance_AppCompat_Widget_PopupMenu_Small;

			public static int Base_TextAppearance_AppCompat_Widget_Switch;

			public static int Base_TextAppearance_AppCompat_Widget_TextView_SpinnerItem;

			public static int Base_TextAppearance_MaterialComponents_Badge;

			public static int Base_TextAppearance_MaterialComponents_Button;

			public static int Base_TextAppearance_MaterialComponents_Headline6;

			public static int Base_TextAppearance_MaterialComponents_Subtitle2;

			public static int Base_TextAppearance_Widget_AppCompat_ExpandedMenu_Item;

			public static int Base_TextAppearance_Widget_AppCompat_Toolbar_Subtitle;

			public static int Base_TextAppearance_Widget_AppCompat_Toolbar_Title;

			public static int Base_ThemeOverlay_AppCompat;

			public static int Base_ThemeOverlay_AppCompat_ActionBar;

			public static int Base_ThemeOverlay_AppCompat_Dark;

			public static int Base_ThemeOverlay_AppCompat_Dark_ActionBar;

			public static int Base_ThemeOverlay_AppCompat_Dialog;

			public static int Base_ThemeOverlay_AppCompat_Dialog_Alert;

			public static int Base_ThemeOverlay_AppCompat_Light;

			public static int Base_ThemeOverlay_MaterialComponents_Dialog;

			public static int Base_ThemeOverlay_MaterialComponents_Dialog_Alert;

			public static int Base_ThemeOverlay_MaterialComponents_MaterialAlertDialog;

			public static int Base_Theme_AppCompat;

			public static int Base_Theme_AppCompat_CompactMenu;

			public static int Base_Theme_AppCompat_Dialog;

			public static int Base_Theme_AppCompat_DialogWhenLarge;

			public static int Base_Theme_AppCompat_Dialog_Alert;

			public static int Base_Theme_AppCompat_Dialog_FixedSize;

			public static int Base_Theme_AppCompat_Dialog_MinWidth;

			public static int Base_Theme_AppCompat_Light;

			public static int Base_Theme_AppCompat_Light_DarkActionBar;

			public static int Base_Theme_AppCompat_Light_Dialog;

			public static int Base_Theme_AppCompat_Light_DialogWhenLarge;

			public static int Base_Theme_AppCompat_Light_Dialog_Alert;

			public static int Base_Theme_AppCompat_Light_Dialog_FixedSize;

			public static int Base_Theme_AppCompat_Light_Dialog_MinWidth;

			public static int Base_Theme_MaterialComponents;

			public static int Base_Theme_MaterialComponents_Bridge;

			public static int Base_Theme_MaterialComponents_CompactMenu;

			public static int Base_Theme_MaterialComponents_Dialog;

			public static int Base_Theme_MaterialComponents_DialogWhenLarge;

			public static int Base_Theme_MaterialComponents_Dialog_Alert;

			public static int Base_Theme_MaterialComponents_Dialog_Bridge;

			public static int Base_Theme_MaterialComponents_Dialog_FixedSize;

			public static int Base_Theme_MaterialComponents_Dialog_MinWidth;

			public static int Base_Theme_MaterialComponents_Light;

			public static int Base_Theme_MaterialComponents_Light_Bridge;

			public static int Base_Theme_MaterialComponents_Light_DarkActionBar;

			public static int Base_Theme_MaterialComponents_Light_DarkActionBar_Bridge;

			public static int Base_Theme_MaterialComponents_Light_Dialog;

			public static int Base_Theme_MaterialComponents_Light_DialogWhenLarge;

			public static int Base_Theme_MaterialComponents_Light_Dialog_Alert;

			public static int Base_Theme_MaterialComponents_Light_Dialog_Bridge;

			public static int Base_Theme_MaterialComponents_Light_Dialog_FixedSize;

			public static int Base_Theme_MaterialComponents_Light_Dialog_MinWidth;

			public static int Base_V14_ThemeOverlay_MaterialComponents_Dialog;

			public static int Base_V14_ThemeOverlay_MaterialComponents_Dialog_Alert;

			public static int Base_V14_ThemeOverlay_MaterialComponents_MaterialAlertDialog;

			public static int Base_V14_Theme_MaterialComponents;

			public static int Base_V14_Theme_MaterialComponents_Bridge;

			public static int Base_V14_Theme_MaterialComponents_Dialog;

			public static int Base_V14_Theme_MaterialComponents_Dialog_Bridge;

			public static int Base_V14_Theme_MaterialComponents_Light;

			public static int Base_V14_Theme_MaterialComponents_Light_Bridge;

			public static int Base_V14_Theme_MaterialComponents_Light_DarkActionBar_Bridge;

			public static int Base_V14_Theme_MaterialComponents_Light_Dialog;

			public static int Base_V14_Theme_MaterialComponents_Light_Dialog_Bridge;

			public static int Base_V21_ThemeOverlay_AppCompat_Dialog;

			public static int Base_V21_Theme_AppCompat;

			public static int Base_V21_Theme_AppCompat_Dialog;

			public static int Base_V21_Theme_AppCompat_Light;

			public static int Base_V21_Theme_AppCompat_Light_Dialog;

			public static int Base_V22_Theme_AppCompat;

			public static int Base_V22_Theme_AppCompat_Light;

			public static int Base_V23_Theme_AppCompat;

			public static int Base_V23_Theme_AppCompat_Light;

			public static int Base_V26_Theme_AppCompat;

			public static int Base_V26_Theme_AppCompat_Light;

			public static int Base_V26_Widget_AppCompat_Toolbar;

			public static int Base_V28_Theme_AppCompat;

			public static int Base_V28_Theme_AppCompat_Light;

			public static int Base_V7_ThemeOverlay_AppCompat_Dialog;

			public static int Base_V7_Theme_AppCompat;

			public static int Base_V7_Theme_AppCompat_Dialog;

			public static int Base_V7_Theme_AppCompat_Light;

			public static int Base_V7_Theme_AppCompat_Light_Dialog;

			public static int Base_V7_Widget_AppCompat_AutoCompleteTextView;

			public static int Base_V7_Widget_AppCompat_EditText;

			public static int Base_V7_Widget_AppCompat_Toolbar;

			public static int Base_Widget_AppCompat_ActionBar;

			public static int Base_Widget_AppCompat_ActionBar_Solid;

			public static int Base_Widget_AppCompat_ActionBar_TabBar;

			public static int Base_Widget_AppCompat_ActionBar_TabText;

			public static int Base_Widget_AppCompat_ActionBar_TabView;

			public static int Base_Widget_AppCompat_ActionButton;

			public static int Base_Widget_AppCompat_ActionButton_CloseMode;

			public static int Base_Widget_AppCompat_ActionButton_Overflow;

			public static int Base_Widget_AppCompat_ActionMode;

			public static int Base_Widget_AppCompat_ActivityChooserView;

			public static int Base_Widget_AppCompat_AutoCompleteTextView;

			public static int Base_Widget_AppCompat_Button;

			public static int Base_Widget_AppCompat_ButtonBar;

			public static int Base_Widget_AppCompat_ButtonBar_AlertDialog;

			public static int Base_Widget_AppCompat_Button_Borderless;

			public static int Base_Widget_AppCompat_Button_Borderless_Colored;

			public static int Base_Widget_AppCompat_Button_ButtonBar_AlertDialog;

			public static int Base_Widget_AppCompat_Button_Colored;

			public static int Base_Widget_AppCompat_Button_Small;

			public static int Base_Widget_AppCompat_CompoundButton_CheckBox;

			public static int Base_Widget_AppCompat_CompoundButton_RadioButton;

			public static int Base_Widget_AppCompat_CompoundButton_Switch;

			public static int Base_Widget_AppCompat_DrawerArrowToggle;

			public static int Base_Widget_AppCompat_DrawerArrowToggle_Common;

			public static int Base_Widget_AppCompat_DropDownItem_Spinner;

			public static int Base_Widget_AppCompat_EditText;

			public static int Base_Widget_AppCompat_ImageButton;

			public static int Base_Widget_AppCompat_Light_ActionBar;

			public static int Base_Widget_AppCompat_Light_ActionBar_Solid;

			public static int Base_Widget_AppCompat_Light_ActionBar_TabBar;

			public static int Base_Widget_AppCompat_Light_ActionBar_TabText;

			public static int Base_Widget_AppCompat_Light_ActionBar_TabText_Inverse;

			public static int Base_Widget_AppCompat_Light_ActionBar_TabView;

			public static int Base_Widget_AppCompat_Light_PopupMenu;

			public static int Base_Widget_AppCompat_Light_PopupMenu_Overflow;

			public static int Base_Widget_AppCompat_ListMenuView;

			public static int Base_Widget_AppCompat_ListPopupWindow;

			public static int Base_Widget_AppCompat_ListView;

			public static int Base_Widget_AppCompat_ListView_DropDown;

			public static int Base_Widget_AppCompat_ListView_Menu;

			public static int Base_Widget_AppCompat_PopupMenu;

			public static int Base_Widget_AppCompat_PopupMenu_Overflow;

			public static int Base_Widget_AppCompat_PopupWindow;

			public static int Base_Widget_AppCompat_ProgressBar;

			public static int Base_Widget_AppCompat_ProgressBar_Horizontal;

			public static int Base_Widget_AppCompat_RatingBar;

			public static int Base_Widget_AppCompat_RatingBar_Indicator;

			public static int Base_Widget_AppCompat_RatingBar_Small;

			public static int Base_Widget_AppCompat_SearchView;

			public static int Base_Widget_AppCompat_SearchView_ActionBar;

			public static int Base_Widget_AppCompat_SeekBar;

			public static int Base_Widget_AppCompat_SeekBar_Discrete;

			public static int Base_Widget_AppCompat_Spinner;

			public static int Base_Widget_AppCompat_Spinner_Underlined;

			public static int Base_Widget_AppCompat_TextView;

			public static int Base_Widget_AppCompat_TextView_SpinnerItem;

			public static int Base_Widget_AppCompat_Toolbar;

			public static int Base_Widget_AppCompat_Toolbar_Button_Navigation;

			public static int Base_Widget_Design_TabLayout;

			public static int Base_Widget_MaterialComponents_AutoCompleteTextView;

			public static int Base_Widget_MaterialComponents_CheckedTextView;

			public static int Base_Widget_MaterialComponents_Chip;

			public static int Base_Widget_MaterialComponents_PopupMenu;

			public static int Base_Widget_MaterialComponents_PopupMenu_ContextMenu;

			public static int Base_Widget_MaterialComponents_PopupMenu_ListPopupWindow;

			public static int Base_Widget_MaterialComponents_PopupMenu_Overflow;

			public static int Base_Widget_MaterialComponents_TextInputEditText;

			public static int Base_Widget_MaterialComponents_TextInputLayout;

			public static int Base_Widget_MaterialComponents_TextView;

			public static int BubbleText;

			public static int CardView;

			public static int CardView_Dark;

			public static int CardView_Light;

			public static int CheckmarkText;

			public static int ConsentButton;

			public static int DefaultButton;

			public static int DefaultButtonGreen;

			public static int DefaultButtonNoBorder;

			public static int DefaultButtonWhite;

			public static int Divider;

			public static int Divider_Horizontal;

			public static int EmptyTheme;

			public static int ErrorText;

			public static int ExplanationTextHeader;

			public static int HeaderText;

			public static int HelpText;

			public static int LauncherAppName;

			public static int LauncherHealthAuth;

			public static int LauncherSubtitle;

			public static int MaterialAlertDialog_MaterialComponents;

			public static int MaterialAlertDialog_MaterialComponents_Body_Text;

			public static int MaterialAlertDialog_MaterialComponents_Picker_Date_Calendar;

			public static int MaterialAlertDialog_MaterialComponents_Picker_Date_Spinner;

			public static int MaterialAlertDialog_MaterialComponents_Title_Icon;

			public static int MaterialAlertDialog_MaterialComponents_Title_Icon_CenterStacked;

			public static int MaterialAlertDialog_MaterialComponents_Title_Panel;

			public static int MaterialAlertDialog_MaterialComponents_Title_Panel_CenterStacked;

			public static int MaterialAlertDialog_MaterialComponents_Title_Text;

			public static int MaterialAlertDialog_MaterialComponents_Title_Text_CenterStacked;

			public static int Platform_AppCompat;

			public static int Platform_AppCompat_Light;

			public static int Platform_MaterialComponents;

			public static int Platform_MaterialComponents_Dialog;

			public static int Platform_MaterialComponents_Light;

			public static int Platform_MaterialComponents_Light_Dialog;

			public static int Platform_ThemeOverlay_AppCompat;

			public static int Platform_ThemeOverlay_AppCompat_Dark;

			public static int Platform_ThemeOverlay_AppCompat_Light;

			public static int Platform_V21_AppCompat;

			public static int Platform_V21_AppCompat_Light;

			public static int Platform_V25_AppCompat;

			public static int Platform_V25_AppCompat_Light;

			public static int Platform_Widget_AppCompat_Spinner;

			public static int PrimaryText;

			public static int PrimaryTextBold;

			public static int PrimaryTextItalic;

			public static int PrimaryTextLight;

			public static int PrimaryTextRegular;

			public static int PrimaryTextSemiBold;

			public static int RectangleBox;

			public static int RtlOverlay_DialogWindowTitle_AppCompat;

			public static int RtlOverlay_Widget_AppCompat_ActionBar_TitleItem;

			public static int RtlOverlay_Widget_AppCompat_DialogTitle_Icon;

			public static int RtlOverlay_Widget_AppCompat_PopupMenuItem;

			public static int RtlOverlay_Widget_AppCompat_PopupMenuItem_InternalGroup;

			public static int RtlOverlay_Widget_AppCompat_PopupMenuItem_Shortcut;

			public static int RtlOverlay_Widget_AppCompat_PopupMenuItem_SubmenuArrow;

			public static int RtlOverlay_Widget_AppCompat_PopupMenuItem_Text;

			public static int RtlOverlay_Widget_AppCompat_PopupMenuItem_Title;

			public static int RtlOverlay_Widget_AppCompat_SearchView_MagIcon;

			public static int RtlOverlay_Widget_AppCompat_Search_DropDown;

			public static int RtlOverlay_Widget_AppCompat_Search_DropDown_Icon1;

			public static int RtlOverlay_Widget_AppCompat_Search_DropDown_Icon2;

			public static int RtlOverlay_Widget_AppCompat_Search_DropDown_Query;

			public static int RtlOverlay_Widget_AppCompat_Search_DropDown_Text;

			public static int RtlUnderlay_Widget_AppCompat_ActionButton;

			public static int RtlUnderlay_Widget_AppCompat_ActionButton_Overflow;

			public static int ScrollbarConsent;

			public static int ScrollScreen;

			public static int SecondaryText;

			public static int settings;

			public static int settings_general;

			public static int ShapeAppearanceOverlay;

			public static int ShapeAppearanceOverlay_BottomLeftDifferentCornerSize;

			public static int ShapeAppearanceOverlay_BottomRightCut;

			public static int ShapeAppearanceOverlay_Cut;

			public static int ShapeAppearanceOverlay_DifferentCornerSize;

			public static int ShapeAppearanceOverlay_MaterialComponents_BottomSheet;

			public static int ShapeAppearanceOverlay_MaterialComponents_Chip;

			public static int ShapeAppearanceOverlay_MaterialComponents_ExtendedFloatingActionButton;

			public static int ShapeAppearanceOverlay_MaterialComponents_FloatingActionButton;

			public static int ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Day;

			public static int ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Window_Fullscreen;

			public static int ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Year;

			public static int ShapeAppearanceOverlay_MaterialComponents_TextInputLayout_FilledBox;

			public static int ShapeAppearanceOverlay_TopLeftCut;

			public static int ShapeAppearanceOverlay_TopRightDifferentCornerSize;

			public static int ShapeAppearance_MaterialComponents;

			public static int ShapeAppearance_MaterialComponents_LargeComponent;

			public static int ShapeAppearance_MaterialComponents_MediumComponent;

			public static int ShapeAppearance_MaterialComponents_SmallComponent;

			public static int ShapeAppearance_MaterialComponents_Test;

			public static int SwitchPlaneStyle;

			public static int SwitchTextStyle;

			public static int TestStyleWithLineHeight;

			public static int TestStyleWithLineHeightAppearance;

			public static int TestStyleWithoutLineHeight;

			public static int TestStyleWithThemeLineHeightAttribute;

			public static int TestThemeWithLineHeight;

			public static int TestThemeWithLineHeightDisabled;

			public static int Test_ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Day;

			public static int Test_Theme_MaterialComponents_MaterialCalendar;

			public static int Test_Widget_MaterialComponents_MaterialCalendar;

			public static int Test_Widget_MaterialComponents_MaterialCalendar_Day;

			public static int Test_Widget_MaterialComponents_MaterialCalendar_Day_Selected;

			public static int TextAppearance_AppCompat;

			public static int TextAppearance_AppCompat_Body1;

			public static int TextAppearance_AppCompat_Body2;

			public static int TextAppearance_AppCompat_Button;

			public static int TextAppearance_AppCompat_Caption;

			public static int TextAppearance_AppCompat_Display1;

			public static int TextAppearance_AppCompat_Display2;

			public static int TextAppearance_AppCompat_Display3;

			public static int TextAppearance_AppCompat_Display4;

			public static int TextAppearance_AppCompat_Headline;

			public static int TextAppearance_AppCompat_Inverse;

			public static int TextAppearance_AppCompat_Large;

			public static int TextAppearance_AppCompat_Large_Inverse;

			public static int TextAppearance_AppCompat_Light_SearchResult_Subtitle;

			public static int TextAppearance_AppCompat_Light_SearchResult_Title;

			public static int TextAppearance_AppCompat_Light_Widget_PopupMenu_Large;

			public static int TextAppearance_AppCompat_Light_Widget_PopupMenu_Small;

			public static int TextAppearance_AppCompat_Medium;

			public static int TextAppearance_AppCompat_Medium_Inverse;

			public static int TextAppearance_AppCompat_Menu;

			public static int TextAppearance_AppCompat_SearchResult_Subtitle;

			public static int TextAppearance_AppCompat_SearchResult_Title;

			public static int TextAppearance_AppCompat_Small;

			public static int TextAppearance_AppCompat_Small_Inverse;

			public static int TextAppearance_AppCompat_Subhead;

			public static int TextAppearance_AppCompat_Subhead_Inverse;

			public static int TextAppearance_AppCompat_Title;

			public static int TextAppearance_AppCompat_Title_Inverse;

			public static int TextAppearance_AppCompat_Tooltip;

			public static int TextAppearance_AppCompat_Widget_ActionBar_Menu;

			public static int TextAppearance_AppCompat_Widget_ActionBar_Subtitle;

			public static int TextAppearance_AppCompat_Widget_ActionBar_Subtitle_Inverse;

			public static int TextAppearance_AppCompat_Widget_ActionBar_Title;

			public static int TextAppearance_AppCompat_Widget_ActionBar_Title_Inverse;

			public static int TextAppearance_AppCompat_Widget_ActionMode_Subtitle;

			public static int TextAppearance_AppCompat_Widget_ActionMode_Subtitle_Inverse;

			public static int TextAppearance_AppCompat_Widget_ActionMode_Title;

			public static int TextAppearance_AppCompat_Widget_ActionMode_Title_Inverse;

			public static int TextAppearance_AppCompat_Widget_Button;

			public static int TextAppearance_AppCompat_Widget_Button_Borderless_Colored;

			public static int TextAppearance_AppCompat_Widget_Button_Colored;

			public static int TextAppearance_AppCompat_Widget_Button_Inverse;

			public static int TextAppearance_AppCompat_Widget_DropDownItem;

			public static int TextAppearance_AppCompat_Widget_PopupMenu_Header;

			public static int TextAppearance_AppCompat_Widget_PopupMenu_Large;

			public static int TextAppearance_AppCompat_Widget_PopupMenu_Small;

			public static int TextAppearance_AppCompat_Widget_Switch;

			public static int TextAppearance_AppCompat_Widget_TextView_SpinnerItem;

			public static int TextAppearance_Compat_Notification;

			public static int TextAppearance_Compat_Notification_Info;

			public static int TextAppearance_Compat_Notification_Info_Media;

			public static int TextAppearance_Compat_Notification_Line2;

			public static int TextAppearance_Compat_Notification_Line2_Media;

			public static int TextAppearance_Compat_Notification_Media;

			public static int TextAppearance_Compat_Notification_Time;

			public static int TextAppearance_Compat_Notification_Time_Media;

			public static int TextAppearance_Compat_Notification_Title;

			public static int TextAppearance_Compat_Notification_Title_Media;

			public static int TextAppearance_Design_CollapsingToolbar_Expanded;

			public static int TextAppearance_Design_Counter;

			public static int TextAppearance_Design_Counter_Overflow;

			public static int TextAppearance_Design_Error;

			public static int TextAppearance_Design_HelperText;

			public static int TextAppearance_Design_Hint;

			public static int TextAppearance_Design_Snackbar_Message;

			public static int TextAppearance_Design_Tab;

			public static int TextAppearance_MaterialComponents_Badge;

			public static int TextAppearance_MaterialComponents_Body1;

			public static int TextAppearance_MaterialComponents_Body2;

			public static int TextAppearance_MaterialComponents_Button;

			public static int TextAppearance_MaterialComponents_Caption;

			public static int TextAppearance_MaterialComponents_Chip;

			public static int TextAppearance_MaterialComponents_Headline1;

			public static int TextAppearance_MaterialComponents_Headline2;

			public static int TextAppearance_MaterialComponents_Headline3;

			public static int TextAppearance_MaterialComponents_Headline4;

			public static int TextAppearance_MaterialComponents_Headline5;

			public static int TextAppearance_MaterialComponents_Headline6;

			public static int TextAppearance_MaterialComponents_Overline;

			public static int TextAppearance_MaterialComponents_Subtitle1;

			public static int TextAppearance_MaterialComponents_Subtitle2;

			public static int TextAppearance_Widget_AppCompat_ExpandedMenu_Item;

			public static int TextAppearance_Widget_AppCompat_Toolbar_Subtitle;

			public static int TextAppearance_Widget_AppCompat_Toolbar_Title;

			public static int ThemeOverlay_AppCompat;

			public static int ThemeOverlay_AppCompat_ActionBar;

			public static int ThemeOverlay_AppCompat_Dark;

			public static int ThemeOverlay_AppCompat_Dark_ActionBar;

			public static int ThemeOverlay_AppCompat_DayNight;

			public static int ThemeOverlay_AppCompat_DayNight_ActionBar;

			public static int ThemeOverlay_AppCompat_Dialog;

			public static int ThemeOverlay_AppCompat_Dialog_Alert;

			public static int ThemeOverlay_AppCompat_Light;

			public static int ThemeOverlay_Design_TextInputEditText;

			public static int ThemeOverlay_MaterialComponents;

			public static int ThemeOverlay_MaterialComponents_ActionBar;

			public static int ThemeOverlay_MaterialComponents_ActionBar_Primary;

			public static int ThemeOverlay_MaterialComponents_ActionBar_Surface;

			public static int ThemeOverlay_MaterialComponents_AutoCompleteTextView;

			public static int ThemeOverlay_MaterialComponents_AutoCompleteTextView_FilledBox;

			public static int ThemeOverlay_MaterialComponents_AutoCompleteTextView_FilledBox_Dense;

			public static int ThemeOverlay_MaterialComponents_AutoCompleteTextView_OutlinedBox;

			public static int ThemeOverlay_MaterialComponents_AutoCompleteTextView_OutlinedBox_Dense;

			public static int ThemeOverlay_MaterialComponents_BottomAppBar_Primary;

			public static int ThemeOverlay_MaterialComponents_BottomAppBar_Surface;

			public static int ThemeOverlay_MaterialComponents_BottomSheetDialog;

			public static int ThemeOverlay_MaterialComponents_Dark;

			public static int ThemeOverlay_MaterialComponents_Dark_ActionBar;

			public static int ThemeOverlay_MaterialComponents_DayNight_BottomSheetDialog;

			public static int ThemeOverlay_MaterialComponents_Dialog;

			public static int ThemeOverlay_MaterialComponents_Dialog_Alert;

			public static int ThemeOverlay_MaterialComponents_Light;

			public static int ThemeOverlay_MaterialComponents_Light_BottomSheetDialog;

			public static int ThemeOverlay_MaterialComponents_MaterialAlertDialog;

			public static int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Centered;

			public static int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date;

			public static int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Calendar;

			public static int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Header_Text;

			public static int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Header_Text_Day;

			public static int ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Spinner;

			public static int ThemeOverlay_MaterialComponents_MaterialCalendar;

			public static int ThemeOverlay_MaterialComponents_MaterialCalendar_Fullscreen;

			public static int ThemeOverlay_MaterialComponents_TextInputEditText;

			public static int ThemeOverlay_MaterialComponents_TextInputEditText_FilledBox;

			public static int ThemeOverlay_MaterialComponents_TextInputEditText_FilledBox_Dense;

			public static int ThemeOverlay_MaterialComponents_TextInputEditText_OutlinedBox;

			public static int ThemeOverlay_MaterialComponents_TextInputEditText_OutlinedBox_Dense;

			public static int ThemeOverlay_MaterialComponents_Toolbar_Primary;

			public static int ThemeOverlay_MaterialComponents_Toolbar_Surface;

			public static int Theme_AppCompat;

			public static int Theme_AppCompat_CompactMenu;

			public static int Theme_AppCompat_DayNight;

			public static int Theme_AppCompat_DayNight_DarkActionBar;

			public static int Theme_AppCompat_DayNight_Dialog;

			public static int Theme_AppCompat_DayNight_DialogWhenLarge;

			public static int Theme_AppCompat_DayNight_Dialog_Alert;

			public static int Theme_AppCompat_DayNight_Dialog_MinWidth;

			public static int Theme_AppCompat_DayNight_NoActionBar;

			public static int Theme_AppCompat_Dialog;

			public static int Theme_AppCompat_DialogWhenLarge;

			public static int Theme_AppCompat_Dialog_Alert;

			public static int Theme_AppCompat_Dialog_MinWidth;

			public static int Theme_AppCompat_Light;

			public static int Theme_AppCompat_Light_DarkActionBar;

			public static int Theme_AppCompat_Light_Dialog;

			public static int Theme_AppCompat_Light_DialogWhenLarge;

			public static int Theme_AppCompat_Light_Dialog_Alert;

			public static int Theme_AppCompat_Light_Dialog_MinWidth;

			public static int Theme_AppCompat_Light_NoActionBar;

			public static int Theme_AppCompat_NoActionBar;

			public static int Theme_Design;

			public static int Theme_Design_BottomSheetDialog;

			public static int Theme_Design_Light;

			public static int Theme_Design_Light_BottomSheetDialog;

			public static int Theme_Design_Light_NoActionBar;

			public static int Theme_Design_NoActionBar;

			public static int Theme_MaterialComponents;

			public static int Theme_MaterialComponents_BottomSheetDialog;

			public static int Theme_MaterialComponents_Bridge;

			public static int Theme_MaterialComponents_CompactMenu;

			public static int Theme_MaterialComponents_DayNight;

			public static int Theme_MaterialComponents_DayNight_BottomSheetDialog;

			public static int Theme_MaterialComponents_DayNight_Bridge;

			public static int Theme_MaterialComponents_DayNight_DarkActionBar;

			public static int Theme_MaterialComponents_DayNight_DarkActionBar_Bridge;

			public static int Theme_MaterialComponents_DayNight_Dialog;

			public static int Theme_MaterialComponents_DayNight_DialogWhenLarge;

			public static int Theme_MaterialComponents_DayNight_Dialog_Alert;

			public static int Theme_MaterialComponents_DayNight_Dialog_Alert_Bridge;

			public static int Theme_MaterialComponents_DayNight_Dialog_Bridge;

			public static int Theme_MaterialComponents_DayNight_Dialog_FixedSize;

			public static int Theme_MaterialComponents_DayNight_Dialog_FixedSize_Bridge;

			public static int Theme_MaterialComponents_DayNight_Dialog_MinWidth;

			public static int Theme_MaterialComponents_DayNight_Dialog_MinWidth_Bridge;

			public static int Theme_MaterialComponents_DayNight_NoActionBar;

			public static int Theme_MaterialComponents_DayNight_NoActionBar_Bridge;

			public static int Theme_MaterialComponents_Dialog;

			public static int Theme_MaterialComponents_DialogWhenLarge;

			public static int Theme_MaterialComponents_Dialog_Alert;

			public static int Theme_MaterialComponents_Dialog_Alert_Bridge;

			public static int Theme_MaterialComponents_Dialog_Bridge;

			public static int Theme_MaterialComponents_Dialog_FixedSize;

			public static int Theme_MaterialComponents_Dialog_FixedSize_Bridge;

			public static int Theme_MaterialComponents_Dialog_MinWidth;

			public static int Theme_MaterialComponents_Dialog_MinWidth_Bridge;

			public static int Theme_MaterialComponents_Light;

			public static int Theme_MaterialComponents_Light_BarSize;

			public static int Theme_MaterialComponents_Light_BottomSheetDialog;

			public static int Theme_MaterialComponents_Light_Bridge;

			public static int Theme_MaterialComponents_Light_DarkActionBar;

			public static int Theme_MaterialComponents_Light_DarkActionBar_Bridge;

			public static int Theme_MaterialComponents_Light_Dialog;

			public static int Theme_MaterialComponents_Light_DialogWhenLarge;

			public static int Theme_MaterialComponents_Light_Dialog_Alert;

			public static int Theme_MaterialComponents_Light_Dialog_Alert_Bridge;

			public static int Theme_MaterialComponents_Light_Dialog_Bridge;

			public static int Theme_MaterialComponents_Light_Dialog_FixedSize;

			public static int Theme_MaterialComponents_Light_Dialog_FixedSize_Bridge;

			public static int Theme_MaterialComponents_Light_Dialog_MinWidth;

			public static int Theme_MaterialComponents_Light_Dialog_MinWidth_Bridge;

			public static int Theme_MaterialComponents_Light_LargeTouch;

			public static int Theme_MaterialComponents_Light_NoActionBar;

			public static int Theme_MaterialComponents_Light_NoActionBar_Bridge;

			public static int Theme_MaterialComponents_NoActionBar;

			public static int Theme_MaterialComponents_NoActionBar_Bridge;

			public static int TopbarText;

			public static int WarningText;

			public static int Widget_AppCompat_ActionBar;

			public static int Widget_AppCompat_ActionBar_Solid;

			public static int Widget_AppCompat_ActionBar_TabBar;

			public static int Widget_AppCompat_ActionBar_TabText;

			public static int Widget_AppCompat_ActionBar_TabView;

			public static int Widget_AppCompat_ActionButton;

			public static int Widget_AppCompat_ActionButton_CloseMode;

			public static int Widget_AppCompat_ActionButton_Overflow;

			public static int Widget_AppCompat_ActionMode;

			public static int Widget_AppCompat_ActivityChooserView;

			public static int Widget_AppCompat_AutoCompleteTextView;

			public static int Widget_AppCompat_Button;

			public static int Widget_AppCompat_ButtonBar;

			public static int Widget_AppCompat_ButtonBar_AlertDialog;

			public static int Widget_AppCompat_Button_Borderless;

			public static int Widget_AppCompat_Button_Borderless_Colored;

			public static int Widget_AppCompat_Button_ButtonBar_AlertDialog;

			public static int Widget_AppCompat_Button_Colored;

			public static int Widget_AppCompat_Button_Small;

			public static int Widget_AppCompat_CompoundButton_CheckBox;

			public static int Widget_AppCompat_CompoundButton_RadioButton;

			public static int Widget_AppCompat_CompoundButton_Switch;

			public static int Widget_AppCompat_DrawerArrowToggle;

			public static int Widget_AppCompat_DropDownItem_Spinner;

			public static int Widget_AppCompat_EditText;

			public static int Widget_AppCompat_ImageButton;

			public static int Widget_AppCompat_Light_ActionBar;

			public static int Widget_AppCompat_Light_ActionBar_Solid;

			public static int Widget_AppCompat_Light_ActionBar_Solid_Inverse;

			public static int Widget_AppCompat_Light_ActionBar_TabBar;

			public static int Widget_AppCompat_Light_ActionBar_TabBar_Inverse;

			public static int Widget_AppCompat_Light_ActionBar_TabText;

			public static int Widget_AppCompat_Light_ActionBar_TabText_Inverse;

			public static int Widget_AppCompat_Light_ActionBar_TabView;

			public static int Widget_AppCompat_Light_ActionBar_TabView_Inverse;

			public static int Widget_AppCompat_Light_ActionButton;

			public static int Widget_AppCompat_Light_ActionButton_CloseMode;

			public static int Widget_AppCompat_Light_ActionButton_Overflow;

			public static int Widget_AppCompat_Light_ActionMode_Inverse;

			public static int Widget_AppCompat_Light_ActivityChooserView;

			public static int Widget_AppCompat_Light_AutoCompleteTextView;

			public static int Widget_AppCompat_Light_DropDownItem_Spinner;

			public static int Widget_AppCompat_Light_ListPopupWindow;

			public static int Widget_AppCompat_Light_ListView_DropDown;

			public static int Widget_AppCompat_Light_PopupMenu;

			public static int Widget_AppCompat_Light_PopupMenu_Overflow;

			public static int Widget_AppCompat_Light_SearchView;

			public static int Widget_AppCompat_Light_Spinner_DropDown_ActionBar;

			public static int Widget_AppCompat_ListMenuView;

			public static int Widget_AppCompat_ListPopupWindow;

			public static int Widget_AppCompat_ListView;

			public static int Widget_AppCompat_ListView_DropDown;

			public static int Widget_AppCompat_ListView_Menu;

			public static int Widget_AppCompat_PopupMenu;

			public static int Widget_AppCompat_PopupMenu_Overflow;

			public static int Widget_AppCompat_PopupWindow;

			public static int Widget_AppCompat_ProgressBar;

			public static int Widget_AppCompat_ProgressBar_Horizontal;

			public static int Widget_AppCompat_RatingBar;

			public static int Widget_AppCompat_RatingBar_Indicator;

			public static int Widget_AppCompat_RatingBar_Small;

			public static int Widget_AppCompat_SearchView;

			public static int Widget_AppCompat_SearchView_ActionBar;

			public static int Widget_AppCompat_SeekBar;

			public static int Widget_AppCompat_SeekBar_Discrete;

			public static int Widget_AppCompat_Spinner;

			public static int Widget_AppCompat_Spinner_DropDown;

			public static int Widget_AppCompat_Spinner_DropDown_ActionBar;

			public static int Widget_AppCompat_Spinner_Underlined;

			public static int Widget_AppCompat_TextView;

			public static int Widget_AppCompat_TextView_SpinnerItem;

			public static int Widget_AppCompat_Toolbar;

			public static int Widget_AppCompat_Toolbar_Button_Navigation;

			public static int Widget_Compat_NotificationActionContainer;

			public static int Widget_Compat_NotificationActionText;

			public static int Widget_Design_AppBarLayout;

			public static int Widget_Design_BottomNavigationView;

			public static int Widget_Design_BottomSheet_Modal;

			public static int Widget_Design_CollapsingToolbar;

			public static int Widget_Design_FloatingActionButton;

			public static int Widget_Design_NavigationView;

			public static int Widget_Design_ScrimInsetsFrameLayout;

			public static int Widget_Design_Snackbar;

			public static int Widget_Design_TabLayout;

			public static int Widget_Design_TextInputLayout;

			public static int Widget_MaterialComponents_ActionBar_Primary;

			public static int Widget_MaterialComponents_ActionBar_PrimarySurface;

			public static int Widget_MaterialComponents_ActionBar_Solid;

			public static int Widget_MaterialComponents_ActionBar_Surface;

			public static int Widget_MaterialComponents_AppBarLayout_Primary;

			public static int Widget_MaterialComponents_AppBarLayout_PrimarySurface;

			public static int Widget_MaterialComponents_AppBarLayout_Surface;

			public static int Widget_MaterialComponents_AutoCompleteTextView_FilledBox;

			public static int Widget_MaterialComponents_AutoCompleteTextView_FilledBox_Dense;

			public static int Widget_MaterialComponents_AutoCompleteTextView_OutlinedBox;

			public static int Widget_MaterialComponents_AutoCompleteTextView_OutlinedBox_Dense;

			public static int Widget_MaterialComponents_Badge;

			public static int Widget_MaterialComponents_BottomAppBar;

			public static int Widget_MaterialComponents_BottomAppBar_Colored;

			public static int Widget_MaterialComponents_BottomAppBar_PrimarySurface;

			public static int Widget_MaterialComponents_BottomNavigationView;

			public static int Widget_MaterialComponents_BottomNavigationView_Colored;

			public static int Widget_MaterialComponents_BottomNavigationView_PrimarySurface;

			public static int Widget_MaterialComponents_BottomSheet;

			public static int Widget_MaterialComponents_BottomSheet_Modal;

			public static int Widget_MaterialComponents_Button;

			public static int Widget_MaterialComponents_Button_Icon;

			public static int Widget_MaterialComponents_Button_OutlinedButton;

			public static int Widget_MaterialComponents_Button_OutlinedButton_Icon;

			public static int Widget_MaterialComponents_Button_TextButton;

			public static int Widget_MaterialComponents_Button_TextButton_Dialog;

			public static int Widget_MaterialComponents_Button_TextButton_Dialog_Flush;

			public static int Widget_MaterialComponents_Button_TextButton_Dialog_Icon;

			public static int Widget_MaterialComponents_Button_TextButton_Icon;

			public static int Widget_MaterialComponents_Button_TextButton_Snackbar;

			public static int Widget_MaterialComponents_Button_UnelevatedButton;

			public static int Widget_MaterialComponents_Button_UnelevatedButton_Icon;

			public static int Widget_MaterialComponents_CardView;

			public static int Widget_MaterialComponents_CheckedTextView;

			public static int Widget_MaterialComponents_ChipGroup;

			public static int Widget_MaterialComponents_Chip_Action;

			public static int Widget_MaterialComponents_Chip_Choice;

			public static int Widget_MaterialComponents_Chip_Entry;

			public static int Widget_MaterialComponents_Chip_Filter;

			public static int Widget_MaterialComponents_CompoundButton_CheckBox;

			public static int Widget_MaterialComponents_CompoundButton_RadioButton;

			public static int Widget_MaterialComponents_CompoundButton_Switch;

			public static int Widget_MaterialComponents_ExtendedFloatingActionButton;

			public static int Widget_MaterialComponents_ExtendedFloatingActionButton_Icon;

			public static int Widget_MaterialComponents_FloatingActionButton;

			public static int Widget_MaterialComponents_Light_ActionBar_Solid;

			public static int Widget_MaterialComponents_MaterialButtonToggleGroup;

			public static int Widget_MaterialComponents_MaterialCalendar;

			public static int Widget_MaterialComponents_MaterialCalendar_Day;

			public static int Widget_MaterialComponents_MaterialCalendar_DayTextView;

			public static int Widget_MaterialComponents_MaterialCalendar_Day_Invalid;

			public static int Widget_MaterialComponents_MaterialCalendar_Day_Selected;

			public static int Widget_MaterialComponents_MaterialCalendar_Day_Today;

			public static int Widget_MaterialComponents_MaterialCalendar_Fullscreen;

			public static int Widget_MaterialComponents_MaterialCalendar_HeaderConfirmButton;

			public static int Widget_MaterialComponents_MaterialCalendar_HeaderDivider;

			public static int Widget_MaterialComponents_MaterialCalendar_HeaderLayout;

			public static int Widget_MaterialComponents_MaterialCalendar_HeaderSelection;

			public static int Widget_MaterialComponents_MaterialCalendar_HeaderSelection_Fullscreen;

			public static int Widget_MaterialComponents_MaterialCalendar_HeaderTitle;

			public static int Widget_MaterialComponents_MaterialCalendar_HeaderToggleButton;

			public static int Widget_MaterialComponents_MaterialCalendar_Item;

			public static int Widget_MaterialComponents_MaterialCalendar_Year;

			public static int Widget_MaterialComponents_MaterialCalendar_Year_Selected;

			public static int Widget_MaterialComponents_MaterialCalendar_Year_Today;

			public static int Widget_MaterialComponents_NavigationView;

			public static int Widget_MaterialComponents_PopupMenu;

			public static int Widget_MaterialComponents_PopupMenu_ContextMenu;

			public static int Widget_MaterialComponents_PopupMenu_ListPopupWindow;

			public static int Widget_MaterialComponents_PopupMenu_Overflow;

			public static int Widget_MaterialComponents_Snackbar;

			public static int Widget_MaterialComponents_Snackbar_FullWidth;

			public static int Widget_MaterialComponents_TabLayout;

			public static int Widget_MaterialComponents_TabLayout_Colored;

			public static int Widget_MaterialComponents_TabLayout_PrimarySurface;

			public static int Widget_MaterialComponents_TextInputEditText_FilledBox;

			public static int Widget_MaterialComponents_TextInputEditText_FilledBox_Dense;

			public static int Widget_MaterialComponents_TextInputEditText_OutlinedBox;

			public static int Widget_MaterialComponents_TextInputEditText_OutlinedBox_Dense;

			public static int Widget_MaterialComponents_TextInputLayout_FilledBox;

			public static int Widget_MaterialComponents_TextInputLayout_FilledBox_Dense;

			public static int Widget_MaterialComponents_TextInputLayout_FilledBox_Dense_ExposedDropdownMenu;

			public static int Widget_MaterialComponents_TextInputLayout_FilledBox_ExposedDropdownMenu;

			public static int Widget_MaterialComponents_TextInputLayout_OutlinedBox;

			public static int Widget_MaterialComponents_TextInputLayout_OutlinedBox_Dense;

			public static int Widget_MaterialComponents_TextInputLayout_OutlinedBox_Dense_ExposedDropdownMenu;

			public static int Widget_MaterialComponents_TextInputLayout_OutlinedBox_ExposedDropdownMenu;

			public static int Widget_MaterialComponents_TextView;

			public static int Widget_MaterialComponents_Toolbar;

			public static int Widget_MaterialComponents_Toolbar_Primary;

			public static int Widget_MaterialComponents_Toolbar_PrimarySurface;

			public static int Widget_MaterialComponents_Toolbar_Surface;

			public static int Widget_Support_CoordinatorLayout;

			static Style()
			{
				AlertDialog_AppCompat = 2131689472;
				AlertDialog_AppCompat_Light = 2131689473;
				Animation_AppCompat_Dialog = 2131689474;
				Animation_AppCompat_DropDownUp = 2131689475;
				Animation_AppCompat_Tooltip = 2131689476;
				Animation_Design_BottomSheetDialog = 2131689477;
				Animation_MaterialComponents_BottomSheetDialog = 2131689478;
				AppTheme = 2131689479;
				AppTheme_AppBarOverlay = 2131689480;
				AppTheme_Launcher = 2131689481;
				AppTheme_PopupOverlay = 2131689482;
				Base_AlertDialog_AppCompat = 2131689483;
				Base_AlertDialog_AppCompat_Light = 2131689484;
				Base_Animation_AppCompat_Dialog = 2131689485;
				Base_Animation_AppCompat_DropDownUp = 2131689486;
				Base_Animation_AppCompat_Tooltip = 2131689487;
				Base_CardView = 2131689488;
				Base_DialogWindowTitleBackground_AppCompat = 2131689490;
				Base_DialogWindowTitle_AppCompat = 2131689489;
				Base_MaterialAlertDialog_MaterialComponents_Title_Icon = 2131689491;
				Base_MaterialAlertDialog_MaterialComponents_Title_Panel = 2131689492;
				Base_MaterialAlertDialog_MaterialComponents_Title_Text = 2131689493;
				Base_TextAppearance_AppCompat = 2131689494;
				Base_TextAppearance_AppCompat_Body1 = 2131689495;
				Base_TextAppearance_AppCompat_Body2 = 2131689496;
				Base_TextAppearance_AppCompat_Button = 2131689497;
				Base_TextAppearance_AppCompat_Caption = 2131689498;
				Base_TextAppearance_AppCompat_Display1 = 2131689499;
				Base_TextAppearance_AppCompat_Display2 = 2131689500;
				Base_TextAppearance_AppCompat_Display3 = 2131689501;
				Base_TextAppearance_AppCompat_Display4 = 2131689502;
				Base_TextAppearance_AppCompat_Headline = 2131689503;
				Base_TextAppearance_AppCompat_Inverse = 2131689504;
				Base_TextAppearance_AppCompat_Large = 2131689505;
				Base_TextAppearance_AppCompat_Large_Inverse = 2131689506;
				Base_TextAppearance_AppCompat_Light_Widget_PopupMenu_Large = 2131689507;
				Base_TextAppearance_AppCompat_Light_Widget_PopupMenu_Small = 2131689508;
				Base_TextAppearance_AppCompat_Medium = 2131689509;
				Base_TextAppearance_AppCompat_Medium_Inverse = 2131689510;
				Base_TextAppearance_AppCompat_Menu = 2131689511;
				Base_TextAppearance_AppCompat_SearchResult = 2131689512;
				Base_TextAppearance_AppCompat_SearchResult_Subtitle = 2131689513;
				Base_TextAppearance_AppCompat_SearchResult_Title = 2131689514;
				Base_TextAppearance_AppCompat_Small = 2131689515;
				Base_TextAppearance_AppCompat_Small_Inverse = 2131689516;
				Base_TextAppearance_AppCompat_Subhead = 2131689517;
				Base_TextAppearance_AppCompat_Subhead_Inverse = 2131689518;
				Base_TextAppearance_AppCompat_Title = 2131689519;
				Base_TextAppearance_AppCompat_Title_Inverse = 2131689520;
				Base_TextAppearance_AppCompat_Tooltip = 2131689521;
				Base_TextAppearance_AppCompat_Widget_ActionBar_Menu = 2131689522;
				Base_TextAppearance_AppCompat_Widget_ActionBar_Subtitle = 2131689523;
				Base_TextAppearance_AppCompat_Widget_ActionBar_Subtitle_Inverse = 2131689524;
				Base_TextAppearance_AppCompat_Widget_ActionBar_Title = 2131689525;
				Base_TextAppearance_AppCompat_Widget_ActionBar_Title_Inverse = 2131689526;
				Base_TextAppearance_AppCompat_Widget_ActionMode_Subtitle = 2131689527;
				Base_TextAppearance_AppCompat_Widget_ActionMode_Title = 2131689528;
				Base_TextAppearance_AppCompat_Widget_Button = 2131689529;
				Base_TextAppearance_AppCompat_Widget_Button_Borderless_Colored = 2131689530;
				Base_TextAppearance_AppCompat_Widget_Button_Colored = 2131689531;
				Base_TextAppearance_AppCompat_Widget_Button_Inverse = 2131689532;
				Base_TextAppearance_AppCompat_Widget_DropDownItem = 2131689533;
				Base_TextAppearance_AppCompat_Widget_PopupMenu_Header = 2131689534;
				Base_TextAppearance_AppCompat_Widget_PopupMenu_Large = 2131689535;
				Base_TextAppearance_AppCompat_Widget_PopupMenu_Small = 2131689536;
				Base_TextAppearance_AppCompat_Widget_Switch = 2131689537;
				Base_TextAppearance_AppCompat_Widget_TextView_SpinnerItem = 2131689538;
				Base_TextAppearance_MaterialComponents_Badge = 2131689539;
				Base_TextAppearance_MaterialComponents_Button = 2131689540;
				Base_TextAppearance_MaterialComponents_Headline6 = 2131689541;
				Base_TextAppearance_MaterialComponents_Subtitle2 = 2131689542;
				Base_TextAppearance_Widget_AppCompat_ExpandedMenu_Item = 2131689543;
				Base_TextAppearance_Widget_AppCompat_Toolbar_Subtitle = 2131689544;
				Base_TextAppearance_Widget_AppCompat_Toolbar_Title = 2131689545;
				Base_ThemeOverlay_AppCompat = 2131689579;
				Base_ThemeOverlay_AppCompat_ActionBar = 2131689580;
				Base_ThemeOverlay_AppCompat_Dark = 2131689581;
				Base_ThemeOverlay_AppCompat_Dark_ActionBar = 2131689582;
				Base_ThemeOverlay_AppCompat_Dialog = 2131689583;
				Base_ThemeOverlay_AppCompat_Dialog_Alert = 2131689584;
				Base_ThemeOverlay_AppCompat_Light = 2131689585;
				Base_ThemeOverlay_MaterialComponents_Dialog = 2131689586;
				Base_ThemeOverlay_MaterialComponents_Dialog_Alert = 2131689587;
				Base_ThemeOverlay_MaterialComponents_MaterialAlertDialog = 2131689588;
				Base_Theme_AppCompat = 2131689546;
				Base_Theme_AppCompat_CompactMenu = 2131689547;
				Base_Theme_AppCompat_Dialog = 2131689548;
				Base_Theme_AppCompat_DialogWhenLarge = 2131689552;
				Base_Theme_AppCompat_Dialog_Alert = 2131689549;
				Base_Theme_AppCompat_Dialog_FixedSize = 2131689550;
				Base_Theme_AppCompat_Dialog_MinWidth = 2131689551;
				Base_Theme_AppCompat_Light = 2131689553;
				Base_Theme_AppCompat_Light_DarkActionBar = 2131689554;
				Base_Theme_AppCompat_Light_Dialog = 2131689555;
				Base_Theme_AppCompat_Light_DialogWhenLarge = 2131689559;
				Base_Theme_AppCompat_Light_Dialog_Alert = 2131689556;
				Base_Theme_AppCompat_Light_Dialog_FixedSize = 2131689557;
				Base_Theme_AppCompat_Light_Dialog_MinWidth = 2131689558;
				Base_Theme_MaterialComponents = 2131689560;
				Base_Theme_MaterialComponents_Bridge = 2131689561;
				Base_Theme_MaterialComponents_CompactMenu = 2131689562;
				Base_Theme_MaterialComponents_Dialog = 2131689563;
				Base_Theme_MaterialComponents_DialogWhenLarge = 2131689568;
				Base_Theme_MaterialComponents_Dialog_Alert = 2131689564;
				Base_Theme_MaterialComponents_Dialog_Bridge = 2131689565;
				Base_Theme_MaterialComponents_Dialog_FixedSize = 2131689566;
				Base_Theme_MaterialComponents_Dialog_MinWidth = 2131689567;
				Base_Theme_MaterialComponents_Light = 2131689569;
				Base_Theme_MaterialComponents_Light_Bridge = 2131689570;
				Base_Theme_MaterialComponents_Light_DarkActionBar = 2131689571;
				Base_Theme_MaterialComponents_Light_DarkActionBar_Bridge = 2131689572;
				Base_Theme_MaterialComponents_Light_Dialog = 2131689573;
				Base_Theme_MaterialComponents_Light_DialogWhenLarge = 2131689578;
				Base_Theme_MaterialComponents_Light_Dialog_Alert = 2131689574;
				Base_Theme_MaterialComponents_Light_Dialog_Bridge = 2131689575;
				Base_Theme_MaterialComponents_Light_Dialog_FixedSize = 2131689576;
				Base_Theme_MaterialComponents_Light_Dialog_MinWidth = 2131689577;
				Base_V14_ThemeOverlay_MaterialComponents_Dialog = 2131689598;
				Base_V14_ThemeOverlay_MaterialComponents_Dialog_Alert = 2131689599;
				Base_V14_ThemeOverlay_MaterialComponents_MaterialAlertDialog = 2131689600;
				Base_V14_Theme_MaterialComponents = 2131689589;
				Base_V14_Theme_MaterialComponents_Bridge = 2131689590;
				Base_V14_Theme_MaterialComponents_Dialog = 2131689591;
				Base_V14_Theme_MaterialComponents_Dialog_Bridge = 2131689592;
				Base_V14_Theme_MaterialComponents_Light = 2131689593;
				Base_V14_Theme_MaterialComponents_Light_Bridge = 2131689594;
				Base_V14_Theme_MaterialComponents_Light_DarkActionBar_Bridge = 2131689595;
				Base_V14_Theme_MaterialComponents_Light_Dialog = 2131689596;
				Base_V14_Theme_MaterialComponents_Light_Dialog_Bridge = 2131689597;
				Base_V21_ThemeOverlay_AppCompat_Dialog = 2131689605;
				Base_V21_Theme_AppCompat = 2131689601;
				Base_V21_Theme_AppCompat_Dialog = 2131689602;
				Base_V21_Theme_AppCompat_Light = 2131689603;
				Base_V21_Theme_AppCompat_Light_Dialog = 2131689604;
				Base_V22_Theme_AppCompat = 2131689606;
				Base_V22_Theme_AppCompat_Light = 2131689607;
				Base_V23_Theme_AppCompat = 2131689608;
				Base_V23_Theme_AppCompat_Light = 2131689609;
				Base_V26_Theme_AppCompat = 2131689610;
				Base_V26_Theme_AppCompat_Light = 2131689611;
				Base_V26_Widget_AppCompat_Toolbar = 2131689612;
				Base_V28_Theme_AppCompat = 2131689613;
				Base_V28_Theme_AppCompat_Light = 2131689614;
				Base_V7_ThemeOverlay_AppCompat_Dialog = 2131689619;
				Base_V7_Theme_AppCompat = 2131689615;
				Base_V7_Theme_AppCompat_Dialog = 2131689616;
				Base_V7_Theme_AppCompat_Light = 2131689617;
				Base_V7_Theme_AppCompat_Light_Dialog = 2131689618;
				Base_V7_Widget_AppCompat_AutoCompleteTextView = 2131689620;
				Base_V7_Widget_AppCompat_EditText = 2131689621;
				Base_V7_Widget_AppCompat_Toolbar = 2131689622;
				Base_Widget_AppCompat_ActionBar = 2131689623;
				Base_Widget_AppCompat_ActionBar_Solid = 2131689624;
				Base_Widget_AppCompat_ActionBar_TabBar = 2131689625;
				Base_Widget_AppCompat_ActionBar_TabText = 2131689626;
				Base_Widget_AppCompat_ActionBar_TabView = 2131689627;
				Base_Widget_AppCompat_ActionButton = 2131689628;
				Base_Widget_AppCompat_ActionButton_CloseMode = 2131689629;
				Base_Widget_AppCompat_ActionButton_Overflow = 2131689630;
				Base_Widget_AppCompat_ActionMode = 2131689631;
				Base_Widget_AppCompat_ActivityChooserView = 2131689632;
				Base_Widget_AppCompat_AutoCompleteTextView = 2131689633;
				Base_Widget_AppCompat_Button = 2131689634;
				Base_Widget_AppCompat_ButtonBar = 2131689640;
				Base_Widget_AppCompat_ButtonBar_AlertDialog = 2131689641;
				Base_Widget_AppCompat_Button_Borderless = 2131689635;
				Base_Widget_AppCompat_Button_Borderless_Colored = 2131689636;
				Base_Widget_AppCompat_Button_ButtonBar_AlertDialog = 2131689637;
				Base_Widget_AppCompat_Button_Colored = 2131689638;
				Base_Widget_AppCompat_Button_Small = 2131689639;
				Base_Widget_AppCompat_CompoundButton_CheckBox = 2131689642;
				Base_Widget_AppCompat_CompoundButton_RadioButton = 2131689643;
				Base_Widget_AppCompat_CompoundButton_Switch = 2131689644;
				Base_Widget_AppCompat_DrawerArrowToggle = 2131689645;
				Base_Widget_AppCompat_DrawerArrowToggle_Common = 2131689646;
				Base_Widget_AppCompat_DropDownItem_Spinner = 2131689647;
				Base_Widget_AppCompat_EditText = 2131689648;
				Base_Widget_AppCompat_ImageButton = 2131689649;
				Base_Widget_AppCompat_Light_ActionBar = 2131689650;
				Base_Widget_AppCompat_Light_ActionBar_Solid = 2131689651;
				Base_Widget_AppCompat_Light_ActionBar_TabBar = 2131689652;
				Base_Widget_AppCompat_Light_ActionBar_TabText = 2131689653;
				Base_Widget_AppCompat_Light_ActionBar_TabText_Inverse = 2131689654;
				Base_Widget_AppCompat_Light_ActionBar_TabView = 2131689655;
				Base_Widget_AppCompat_Light_PopupMenu = 2131689656;
				Base_Widget_AppCompat_Light_PopupMenu_Overflow = 2131689657;
				Base_Widget_AppCompat_ListMenuView = 2131689658;
				Base_Widget_AppCompat_ListPopupWindow = 2131689659;
				Base_Widget_AppCompat_ListView = 2131689660;
				Base_Widget_AppCompat_ListView_DropDown = 2131689661;
				Base_Widget_AppCompat_ListView_Menu = 2131689662;
				Base_Widget_AppCompat_PopupMenu = 2131689663;
				Base_Widget_AppCompat_PopupMenu_Overflow = 2131689664;
				Base_Widget_AppCompat_PopupWindow = 2131689665;
				Base_Widget_AppCompat_ProgressBar = 2131689666;
				Base_Widget_AppCompat_ProgressBar_Horizontal = 2131689667;
				Base_Widget_AppCompat_RatingBar = 2131689668;
				Base_Widget_AppCompat_RatingBar_Indicator = 2131689669;
				Base_Widget_AppCompat_RatingBar_Small = 2131689670;
				Base_Widget_AppCompat_SearchView = 2131689671;
				Base_Widget_AppCompat_SearchView_ActionBar = 2131689672;
				Base_Widget_AppCompat_SeekBar = 2131689673;
				Base_Widget_AppCompat_SeekBar_Discrete = 2131689674;
				Base_Widget_AppCompat_Spinner = 2131689675;
				Base_Widget_AppCompat_Spinner_Underlined = 2131689676;
				Base_Widget_AppCompat_TextView = 2131689677;
				Base_Widget_AppCompat_TextView_SpinnerItem = 2131689678;
				Base_Widget_AppCompat_Toolbar = 2131689679;
				Base_Widget_AppCompat_Toolbar_Button_Navigation = 2131689680;
				Base_Widget_Design_TabLayout = 2131689681;
				Base_Widget_MaterialComponents_AutoCompleteTextView = 2131689682;
				Base_Widget_MaterialComponents_CheckedTextView = 2131689683;
				Base_Widget_MaterialComponents_Chip = 2131689684;
				Base_Widget_MaterialComponents_PopupMenu = 2131689685;
				Base_Widget_MaterialComponents_PopupMenu_ContextMenu = 2131689686;
				Base_Widget_MaterialComponents_PopupMenu_ListPopupWindow = 2131689687;
				Base_Widget_MaterialComponents_PopupMenu_Overflow = 2131689688;
				Base_Widget_MaterialComponents_TextInputEditText = 2131689689;
				Base_Widget_MaterialComponents_TextInputLayout = 2131689690;
				Base_Widget_MaterialComponents_TextView = 2131689691;
				BubbleText = 2131689692;
				CardView = 2131689693;
				CardView_Dark = 2131689694;
				CardView_Light = 2131689695;
				CheckmarkText = 2131689696;
				ConsentButton = 2131689697;
				DefaultButton = 2131689698;
				DefaultButtonGreen = 2131689699;
				DefaultButtonNoBorder = 2131689700;
				DefaultButtonWhite = 2131689701;
				Divider = 2131689702;
				Divider_Horizontal = 2131689703;
				EmptyTheme = 2131689704;
				ErrorText = 2131689705;
				ExplanationTextHeader = 2131689706;
				HeaderText = 2131689707;
				HelpText = 2131689708;
				LauncherAppName = 2131689709;
				LauncherHealthAuth = 2131689710;
				LauncherSubtitle = 2131689711;
				MaterialAlertDialog_MaterialComponents = 2131689712;
				MaterialAlertDialog_MaterialComponents_Body_Text = 2131689713;
				MaterialAlertDialog_MaterialComponents_Picker_Date_Calendar = 2131689714;
				MaterialAlertDialog_MaterialComponents_Picker_Date_Spinner = 2131689715;
				MaterialAlertDialog_MaterialComponents_Title_Icon = 2131689716;
				MaterialAlertDialog_MaterialComponents_Title_Icon_CenterStacked = 2131689717;
				MaterialAlertDialog_MaterialComponents_Title_Panel = 2131689718;
				MaterialAlertDialog_MaterialComponents_Title_Panel_CenterStacked = 2131689719;
				MaterialAlertDialog_MaterialComponents_Title_Text = 2131689720;
				MaterialAlertDialog_MaterialComponents_Title_Text_CenterStacked = 2131689721;
				Platform_AppCompat = 2131689722;
				Platform_AppCompat_Light = 2131689723;
				Platform_MaterialComponents = 2131689724;
				Platform_MaterialComponents_Dialog = 2131689725;
				Platform_MaterialComponents_Light = 2131689726;
				Platform_MaterialComponents_Light_Dialog = 2131689727;
				Platform_ThemeOverlay_AppCompat = 2131689728;
				Platform_ThemeOverlay_AppCompat_Dark = 2131689729;
				Platform_ThemeOverlay_AppCompat_Light = 2131689730;
				Platform_V21_AppCompat = 2131689731;
				Platform_V21_AppCompat_Light = 2131689732;
				Platform_V25_AppCompat = 2131689733;
				Platform_V25_AppCompat_Light = 2131689734;
				Platform_Widget_AppCompat_Spinner = 2131689735;
				PrimaryText = 2131689736;
				PrimaryTextBold = 2131689737;
				PrimaryTextItalic = 2131689738;
				PrimaryTextLight = 2131689739;
				PrimaryTextRegular = 2131689740;
				PrimaryTextSemiBold = 2131689741;
				RectangleBox = 2131689742;
				RtlOverlay_DialogWindowTitle_AppCompat = 2131689743;
				RtlOverlay_Widget_AppCompat_ActionBar_TitleItem = 2131689744;
				RtlOverlay_Widget_AppCompat_DialogTitle_Icon = 2131689745;
				RtlOverlay_Widget_AppCompat_PopupMenuItem = 2131689746;
				RtlOverlay_Widget_AppCompat_PopupMenuItem_InternalGroup = 2131689747;
				RtlOverlay_Widget_AppCompat_PopupMenuItem_Shortcut = 2131689748;
				RtlOverlay_Widget_AppCompat_PopupMenuItem_SubmenuArrow = 2131689749;
				RtlOverlay_Widget_AppCompat_PopupMenuItem_Text = 2131689750;
				RtlOverlay_Widget_AppCompat_PopupMenuItem_Title = 2131689751;
				RtlOverlay_Widget_AppCompat_SearchView_MagIcon = 2131689757;
				RtlOverlay_Widget_AppCompat_Search_DropDown = 2131689752;
				RtlOverlay_Widget_AppCompat_Search_DropDown_Icon1 = 2131689753;
				RtlOverlay_Widget_AppCompat_Search_DropDown_Icon2 = 2131689754;
				RtlOverlay_Widget_AppCompat_Search_DropDown_Query = 2131689755;
				RtlOverlay_Widget_AppCompat_Search_DropDown_Text = 2131689756;
				RtlUnderlay_Widget_AppCompat_ActionButton = 2131689758;
				RtlUnderlay_Widget_AppCompat_ActionButton_Overflow = 2131689759;
				ScrollbarConsent = 2131689761;
				ScrollScreen = 2131689760;
				SecondaryText = 2131689762;
				settings = 2131690181;
				settings_general = 2131690182;
				ShapeAppearanceOverlay = 2131689768;
				ShapeAppearanceOverlay_BottomLeftDifferentCornerSize = 2131689769;
				ShapeAppearanceOverlay_BottomRightCut = 2131689770;
				ShapeAppearanceOverlay_Cut = 2131689771;
				ShapeAppearanceOverlay_DifferentCornerSize = 2131689772;
				ShapeAppearanceOverlay_MaterialComponents_BottomSheet = 2131689773;
				ShapeAppearanceOverlay_MaterialComponents_Chip = 2131689774;
				ShapeAppearanceOverlay_MaterialComponents_ExtendedFloatingActionButton = 2131689775;
				ShapeAppearanceOverlay_MaterialComponents_FloatingActionButton = 2131689776;
				ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Day = 2131689777;
				ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Window_Fullscreen = 2131689778;
				ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Year = 2131689779;
				ShapeAppearanceOverlay_MaterialComponents_TextInputLayout_FilledBox = 2131689780;
				ShapeAppearanceOverlay_TopLeftCut = 2131689781;
				ShapeAppearanceOverlay_TopRightDifferentCornerSize = 2131689782;
				ShapeAppearance_MaterialComponents = 2131689763;
				ShapeAppearance_MaterialComponents_LargeComponent = 2131689764;
				ShapeAppearance_MaterialComponents_MediumComponent = 2131689765;
				ShapeAppearance_MaterialComponents_SmallComponent = 2131689766;
				ShapeAppearance_MaterialComponents_Test = 2131689767;
				SwitchPlaneStyle = 2131689783;
				SwitchTextStyle = 2131689784;
				TestStyleWithLineHeight = 2131689790;
				TestStyleWithLineHeightAppearance = 2131689791;
				TestStyleWithoutLineHeight = 2131689793;
				TestStyleWithThemeLineHeightAttribute = 2131689792;
				TestThemeWithLineHeight = 2131689794;
				TestThemeWithLineHeightDisabled = 2131689795;
				Test_ShapeAppearanceOverlay_MaterialComponents_MaterialCalendar_Day = 2131689785;
				Test_Theme_MaterialComponents_MaterialCalendar = 2131689786;
				Test_Widget_MaterialComponents_MaterialCalendar = 2131689787;
				Test_Widget_MaterialComponents_MaterialCalendar_Day = 2131689788;
				Test_Widget_MaterialComponents_MaterialCalendar_Day_Selected = 2131689789;
				TextAppearance_AppCompat = 2131689796;
				TextAppearance_AppCompat_Body1 = 2131689797;
				TextAppearance_AppCompat_Body2 = 2131689798;
				TextAppearance_AppCompat_Button = 2131689799;
				TextAppearance_AppCompat_Caption = 2131689800;
				TextAppearance_AppCompat_Display1 = 2131689801;
				TextAppearance_AppCompat_Display2 = 2131689802;
				TextAppearance_AppCompat_Display3 = 2131689803;
				TextAppearance_AppCompat_Display4 = 2131689804;
				TextAppearance_AppCompat_Headline = 2131689805;
				TextAppearance_AppCompat_Inverse = 2131689806;
				TextAppearance_AppCompat_Large = 2131689807;
				TextAppearance_AppCompat_Large_Inverse = 2131689808;
				TextAppearance_AppCompat_Light_SearchResult_Subtitle = 2131689809;
				TextAppearance_AppCompat_Light_SearchResult_Title = 2131689810;
				TextAppearance_AppCompat_Light_Widget_PopupMenu_Large = 2131689811;
				TextAppearance_AppCompat_Light_Widget_PopupMenu_Small = 2131689812;
				TextAppearance_AppCompat_Medium = 2131689813;
				TextAppearance_AppCompat_Medium_Inverse = 2131689814;
				TextAppearance_AppCompat_Menu = 2131689815;
				TextAppearance_AppCompat_SearchResult_Subtitle = 2131689816;
				TextAppearance_AppCompat_SearchResult_Title = 2131689817;
				TextAppearance_AppCompat_Small = 2131689818;
				TextAppearance_AppCompat_Small_Inverse = 2131689819;
				TextAppearance_AppCompat_Subhead = 2131689820;
				TextAppearance_AppCompat_Subhead_Inverse = 2131689821;
				TextAppearance_AppCompat_Title = 2131689822;
				TextAppearance_AppCompat_Title_Inverse = 2131689823;
				TextAppearance_AppCompat_Tooltip = 2131689824;
				TextAppearance_AppCompat_Widget_ActionBar_Menu = 2131689825;
				TextAppearance_AppCompat_Widget_ActionBar_Subtitle = 2131689826;
				TextAppearance_AppCompat_Widget_ActionBar_Subtitle_Inverse = 2131689827;
				TextAppearance_AppCompat_Widget_ActionBar_Title = 2131689828;
				TextAppearance_AppCompat_Widget_ActionBar_Title_Inverse = 2131689829;
				TextAppearance_AppCompat_Widget_ActionMode_Subtitle = 2131689830;
				TextAppearance_AppCompat_Widget_ActionMode_Subtitle_Inverse = 2131689831;
				TextAppearance_AppCompat_Widget_ActionMode_Title = 2131689832;
				TextAppearance_AppCompat_Widget_ActionMode_Title_Inverse = 2131689833;
				TextAppearance_AppCompat_Widget_Button = 2131689834;
				TextAppearance_AppCompat_Widget_Button_Borderless_Colored = 2131689835;
				TextAppearance_AppCompat_Widget_Button_Colored = 2131689836;
				TextAppearance_AppCompat_Widget_Button_Inverse = 2131689837;
				TextAppearance_AppCompat_Widget_DropDownItem = 2131689838;
				TextAppearance_AppCompat_Widget_PopupMenu_Header = 2131689839;
				TextAppearance_AppCompat_Widget_PopupMenu_Large = 2131689840;
				TextAppearance_AppCompat_Widget_PopupMenu_Small = 2131689841;
				TextAppearance_AppCompat_Widget_Switch = 2131689842;
				TextAppearance_AppCompat_Widget_TextView_SpinnerItem = 2131689843;
				TextAppearance_Compat_Notification = 2131689844;
				TextAppearance_Compat_Notification_Info = 2131689845;
				TextAppearance_Compat_Notification_Info_Media = 2131689846;
				TextAppearance_Compat_Notification_Line2 = 2131689847;
				TextAppearance_Compat_Notification_Line2_Media = 2131689848;
				TextAppearance_Compat_Notification_Media = 2131689849;
				TextAppearance_Compat_Notification_Time = 2131689850;
				TextAppearance_Compat_Notification_Time_Media = 2131689851;
				TextAppearance_Compat_Notification_Title = 2131689852;
				TextAppearance_Compat_Notification_Title_Media = 2131689853;
				TextAppearance_Design_CollapsingToolbar_Expanded = 2131689854;
				TextAppearance_Design_Counter = 2131689855;
				TextAppearance_Design_Counter_Overflow = 2131689856;
				TextAppearance_Design_Error = 2131689857;
				TextAppearance_Design_HelperText = 2131689858;
				TextAppearance_Design_Hint = 2131689859;
				TextAppearance_Design_Snackbar_Message = 2131689860;
				TextAppearance_Design_Tab = 2131689861;
				TextAppearance_MaterialComponents_Badge = 2131689862;
				TextAppearance_MaterialComponents_Body1 = 2131689863;
				TextAppearance_MaterialComponents_Body2 = 2131689864;
				TextAppearance_MaterialComponents_Button = 2131689865;
				TextAppearance_MaterialComponents_Caption = 2131689866;
				TextAppearance_MaterialComponents_Chip = 2131689867;
				TextAppearance_MaterialComponents_Headline1 = 2131689868;
				TextAppearance_MaterialComponents_Headline2 = 2131689869;
				TextAppearance_MaterialComponents_Headline3 = 2131689870;
				TextAppearance_MaterialComponents_Headline4 = 2131689871;
				TextAppearance_MaterialComponents_Headline5 = 2131689872;
				TextAppearance_MaterialComponents_Headline6 = 2131689873;
				TextAppearance_MaterialComponents_Overline = 2131689874;
				TextAppearance_MaterialComponents_Subtitle1 = 2131689875;
				TextAppearance_MaterialComponents_Subtitle2 = 2131689876;
				TextAppearance_Widget_AppCompat_ExpandedMenu_Item = 2131689877;
				TextAppearance_Widget_AppCompat_Toolbar_Subtitle = 2131689878;
				TextAppearance_Widget_AppCompat_Toolbar_Title = 2131689879;
				ThemeOverlay_AppCompat = 2131689956;
				ThemeOverlay_AppCompat_ActionBar = 2131689957;
				ThemeOverlay_AppCompat_Dark = 2131689958;
				ThemeOverlay_AppCompat_Dark_ActionBar = 2131689959;
				ThemeOverlay_AppCompat_DayNight = 2131689960;
				ThemeOverlay_AppCompat_DayNight_ActionBar = 2131689961;
				ThemeOverlay_AppCompat_Dialog = 2131689962;
				ThemeOverlay_AppCompat_Dialog_Alert = 2131689963;
				ThemeOverlay_AppCompat_Light = 2131689964;
				ThemeOverlay_Design_TextInputEditText = 2131689965;
				ThemeOverlay_MaterialComponents = 2131689966;
				ThemeOverlay_MaterialComponents_ActionBar = 2131689967;
				ThemeOverlay_MaterialComponents_ActionBar_Primary = 2131689968;
				ThemeOverlay_MaterialComponents_ActionBar_Surface = 2131689969;
				ThemeOverlay_MaterialComponents_AutoCompleteTextView = 2131689970;
				ThemeOverlay_MaterialComponents_AutoCompleteTextView_FilledBox = 2131689971;
				ThemeOverlay_MaterialComponents_AutoCompleteTextView_FilledBox_Dense = 2131689972;
				ThemeOverlay_MaterialComponents_AutoCompleteTextView_OutlinedBox = 2131689973;
				ThemeOverlay_MaterialComponents_AutoCompleteTextView_OutlinedBox_Dense = 2131689974;
				ThemeOverlay_MaterialComponents_BottomAppBar_Primary = 2131689975;
				ThemeOverlay_MaterialComponents_BottomAppBar_Surface = 2131689976;
				ThemeOverlay_MaterialComponents_BottomSheetDialog = 2131689977;
				ThemeOverlay_MaterialComponents_Dark = 2131689978;
				ThemeOverlay_MaterialComponents_Dark_ActionBar = 2131689979;
				ThemeOverlay_MaterialComponents_DayNight_BottomSheetDialog = 2131689980;
				ThemeOverlay_MaterialComponents_Dialog = 2131689981;
				ThemeOverlay_MaterialComponents_Dialog_Alert = 2131689982;
				ThemeOverlay_MaterialComponents_Light = 2131689983;
				ThemeOverlay_MaterialComponents_Light_BottomSheetDialog = 2131689984;
				ThemeOverlay_MaterialComponents_MaterialAlertDialog = 2131689985;
				ThemeOverlay_MaterialComponents_MaterialAlertDialog_Centered = 2131689986;
				ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date = 2131689987;
				ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Calendar = 2131689988;
				ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Header_Text = 2131689989;
				ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Header_Text_Day = 2131689990;
				ThemeOverlay_MaterialComponents_MaterialAlertDialog_Picker_Date_Spinner = 2131689991;
				ThemeOverlay_MaterialComponents_MaterialCalendar = 2131689992;
				ThemeOverlay_MaterialComponents_MaterialCalendar_Fullscreen = 2131689993;
				ThemeOverlay_MaterialComponents_TextInputEditText = 2131689994;
				ThemeOverlay_MaterialComponents_TextInputEditText_FilledBox = 2131689995;
				ThemeOverlay_MaterialComponents_TextInputEditText_FilledBox_Dense = 2131689996;
				ThemeOverlay_MaterialComponents_TextInputEditText_OutlinedBox = 2131689997;
				ThemeOverlay_MaterialComponents_TextInputEditText_OutlinedBox_Dense = 2131689998;
				ThemeOverlay_MaterialComponents_Toolbar_Primary = 2131689999;
				ThemeOverlay_MaterialComponents_Toolbar_Surface = 2131690000;
				Theme_AppCompat = 2131689880;
				Theme_AppCompat_CompactMenu = 2131689881;
				Theme_AppCompat_DayNight = 2131689882;
				Theme_AppCompat_DayNight_DarkActionBar = 2131689883;
				Theme_AppCompat_DayNight_Dialog = 2131689884;
				Theme_AppCompat_DayNight_DialogWhenLarge = 2131689887;
				Theme_AppCompat_DayNight_Dialog_Alert = 2131689885;
				Theme_AppCompat_DayNight_Dialog_MinWidth = 2131689886;
				Theme_AppCompat_DayNight_NoActionBar = 2131689888;
				Theme_AppCompat_Dialog = 2131689889;
				Theme_AppCompat_DialogWhenLarge = 2131689892;
				Theme_AppCompat_Dialog_Alert = 2131689890;
				Theme_AppCompat_Dialog_MinWidth = 2131689891;
				Theme_AppCompat_Light = 2131689893;
				Theme_AppCompat_Light_DarkActionBar = 2131689894;
				Theme_AppCompat_Light_Dialog = 2131689895;
				Theme_AppCompat_Light_DialogWhenLarge = 2131689898;
				Theme_AppCompat_Light_Dialog_Alert = 2131689896;
				Theme_AppCompat_Light_Dialog_MinWidth = 2131689897;
				Theme_AppCompat_Light_NoActionBar = 2131689899;
				Theme_AppCompat_NoActionBar = 2131689900;
				Theme_Design = 2131689901;
				Theme_Design_BottomSheetDialog = 2131689902;
				Theme_Design_Light = 2131689903;
				Theme_Design_Light_BottomSheetDialog = 2131689904;
				Theme_Design_Light_NoActionBar = 2131689905;
				Theme_Design_NoActionBar = 2131689906;
				Theme_MaterialComponents = 2131689907;
				Theme_MaterialComponents_BottomSheetDialog = 2131689908;
				Theme_MaterialComponents_Bridge = 2131689909;
				Theme_MaterialComponents_CompactMenu = 2131689910;
				Theme_MaterialComponents_DayNight = 2131689911;
				Theme_MaterialComponents_DayNight_BottomSheetDialog = 2131689912;
				Theme_MaterialComponents_DayNight_Bridge = 2131689913;
				Theme_MaterialComponents_DayNight_DarkActionBar = 2131689914;
				Theme_MaterialComponents_DayNight_DarkActionBar_Bridge = 2131689915;
				Theme_MaterialComponents_DayNight_Dialog = 2131689916;
				Theme_MaterialComponents_DayNight_DialogWhenLarge = 2131689924;
				Theme_MaterialComponents_DayNight_Dialog_Alert = 2131689917;
				Theme_MaterialComponents_DayNight_Dialog_Alert_Bridge = 2131689918;
				Theme_MaterialComponents_DayNight_Dialog_Bridge = 2131689919;
				Theme_MaterialComponents_DayNight_Dialog_FixedSize = 2131689920;
				Theme_MaterialComponents_DayNight_Dialog_FixedSize_Bridge = 2131689921;
				Theme_MaterialComponents_DayNight_Dialog_MinWidth = 2131689922;
				Theme_MaterialComponents_DayNight_Dialog_MinWidth_Bridge = 2131689923;
				Theme_MaterialComponents_DayNight_NoActionBar = 2131689925;
				Theme_MaterialComponents_DayNight_NoActionBar_Bridge = 2131689926;
				Theme_MaterialComponents_Dialog = 2131689927;
				Theme_MaterialComponents_DialogWhenLarge = 2131689935;
				Theme_MaterialComponents_Dialog_Alert = 2131689928;
				Theme_MaterialComponents_Dialog_Alert_Bridge = 2131689929;
				Theme_MaterialComponents_Dialog_Bridge = 2131689930;
				Theme_MaterialComponents_Dialog_FixedSize = 2131689931;
				Theme_MaterialComponents_Dialog_FixedSize_Bridge = 2131689932;
				Theme_MaterialComponents_Dialog_MinWidth = 2131689933;
				Theme_MaterialComponents_Dialog_MinWidth_Bridge = 2131689934;
				Theme_MaterialComponents_Light = 2131689936;
				Theme_MaterialComponents_Light_BarSize = 2131689937;
				Theme_MaterialComponents_Light_BottomSheetDialog = 2131689938;
				Theme_MaterialComponents_Light_Bridge = 2131689939;
				Theme_MaterialComponents_Light_DarkActionBar = 2131689940;
				Theme_MaterialComponents_Light_DarkActionBar_Bridge = 2131689941;
				Theme_MaterialComponents_Light_Dialog = 2131689942;
				Theme_MaterialComponents_Light_DialogWhenLarge = 2131689950;
				Theme_MaterialComponents_Light_Dialog_Alert = 2131689943;
				Theme_MaterialComponents_Light_Dialog_Alert_Bridge = 2131689944;
				Theme_MaterialComponents_Light_Dialog_Bridge = 2131689945;
				Theme_MaterialComponents_Light_Dialog_FixedSize = 2131689946;
				Theme_MaterialComponents_Light_Dialog_FixedSize_Bridge = 2131689947;
				Theme_MaterialComponents_Light_Dialog_MinWidth = 2131689948;
				Theme_MaterialComponents_Light_Dialog_MinWidth_Bridge = 2131689949;
				Theme_MaterialComponents_Light_LargeTouch = 2131689951;
				Theme_MaterialComponents_Light_NoActionBar = 2131689952;
				Theme_MaterialComponents_Light_NoActionBar_Bridge = 2131689953;
				Theme_MaterialComponents_NoActionBar = 2131689954;
				Theme_MaterialComponents_NoActionBar_Bridge = 2131689955;
				TopbarText = 2131690001;
				WarningText = 2131690002;
				Widget_AppCompat_ActionBar = 2131690003;
				Widget_AppCompat_ActionBar_Solid = 2131690004;
				Widget_AppCompat_ActionBar_TabBar = 2131690005;
				Widget_AppCompat_ActionBar_TabText = 2131690006;
				Widget_AppCompat_ActionBar_TabView = 2131690007;
				Widget_AppCompat_ActionButton = 2131690008;
				Widget_AppCompat_ActionButton_CloseMode = 2131690009;
				Widget_AppCompat_ActionButton_Overflow = 2131690010;
				Widget_AppCompat_ActionMode = 2131690011;
				Widget_AppCompat_ActivityChooserView = 2131690012;
				Widget_AppCompat_AutoCompleteTextView = 2131690013;
				Widget_AppCompat_Button = 2131690014;
				Widget_AppCompat_ButtonBar = 2131690020;
				Widget_AppCompat_ButtonBar_AlertDialog = 2131690021;
				Widget_AppCompat_Button_Borderless = 2131690015;
				Widget_AppCompat_Button_Borderless_Colored = 2131690016;
				Widget_AppCompat_Button_ButtonBar_AlertDialog = 2131690017;
				Widget_AppCompat_Button_Colored = 2131690018;
				Widget_AppCompat_Button_Small = 2131690019;
				Widget_AppCompat_CompoundButton_CheckBox = 2131690022;
				Widget_AppCompat_CompoundButton_RadioButton = 2131690023;
				Widget_AppCompat_CompoundButton_Switch = 2131690024;
				Widget_AppCompat_DrawerArrowToggle = 2131690025;
				Widget_AppCompat_DropDownItem_Spinner = 2131690026;
				Widget_AppCompat_EditText = 2131690027;
				Widget_AppCompat_ImageButton = 2131690028;
				Widget_AppCompat_Light_ActionBar = 2131690029;
				Widget_AppCompat_Light_ActionBar_Solid = 2131690030;
				Widget_AppCompat_Light_ActionBar_Solid_Inverse = 2131690031;
				Widget_AppCompat_Light_ActionBar_TabBar = 2131690032;
				Widget_AppCompat_Light_ActionBar_TabBar_Inverse = 2131690033;
				Widget_AppCompat_Light_ActionBar_TabText = 2131690034;
				Widget_AppCompat_Light_ActionBar_TabText_Inverse = 2131690035;
				Widget_AppCompat_Light_ActionBar_TabView = 2131690036;
				Widget_AppCompat_Light_ActionBar_TabView_Inverse = 2131690037;
				Widget_AppCompat_Light_ActionButton = 2131690038;
				Widget_AppCompat_Light_ActionButton_CloseMode = 2131690039;
				Widget_AppCompat_Light_ActionButton_Overflow = 2131690040;
				Widget_AppCompat_Light_ActionMode_Inverse = 2131690041;
				Widget_AppCompat_Light_ActivityChooserView = 2131690042;
				Widget_AppCompat_Light_AutoCompleteTextView = 2131690043;
				Widget_AppCompat_Light_DropDownItem_Spinner = 2131690044;
				Widget_AppCompat_Light_ListPopupWindow = 2131690045;
				Widget_AppCompat_Light_ListView_DropDown = 2131690046;
				Widget_AppCompat_Light_PopupMenu = 2131690047;
				Widget_AppCompat_Light_PopupMenu_Overflow = 2131690048;
				Widget_AppCompat_Light_SearchView = 2131690049;
				Widget_AppCompat_Light_Spinner_DropDown_ActionBar = 2131690050;
				Widget_AppCompat_ListMenuView = 2131690051;
				Widget_AppCompat_ListPopupWindow = 2131690052;
				Widget_AppCompat_ListView = 2131690053;
				Widget_AppCompat_ListView_DropDown = 2131690054;
				Widget_AppCompat_ListView_Menu = 2131690055;
				Widget_AppCompat_PopupMenu = 2131690056;
				Widget_AppCompat_PopupMenu_Overflow = 2131690057;
				Widget_AppCompat_PopupWindow = 2131690058;
				Widget_AppCompat_ProgressBar = 2131690059;
				Widget_AppCompat_ProgressBar_Horizontal = 2131690060;
				Widget_AppCompat_RatingBar = 2131690061;
				Widget_AppCompat_RatingBar_Indicator = 2131690062;
				Widget_AppCompat_RatingBar_Small = 2131690063;
				Widget_AppCompat_SearchView = 2131690064;
				Widget_AppCompat_SearchView_ActionBar = 2131690065;
				Widget_AppCompat_SeekBar = 2131690066;
				Widget_AppCompat_SeekBar_Discrete = 2131690067;
				Widget_AppCompat_Spinner = 2131690068;
				Widget_AppCompat_Spinner_DropDown = 2131690069;
				Widget_AppCompat_Spinner_DropDown_ActionBar = 2131690070;
				Widget_AppCompat_Spinner_Underlined = 2131690071;
				Widget_AppCompat_TextView = 2131690072;
				Widget_AppCompat_TextView_SpinnerItem = 2131690073;
				Widget_AppCompat_Toolbar = 2131690074;
				Widget_AppCompat_Toolbar_Button_Navigation = 2131690075;
				Widget_Compat_NotificationActionContainer = 2131690076;
				Widget_Compat_NotificationActionText = 2131690077;
				Widget_Design_AppBarLayout = 2131690078;
				Widget_Design_BottomNavigationView = 2131690079;
				Widget_Design_BottomSheet_Modal = 2131690080;
				Widget_Design_CollapsingToolbar = 2131690081;
				Widget_Design_FloatingActionButton = 2131690082;
				Widget_Design_NavigationView = 2131690083;
				Widget_Design_ScrimInsetsFrameLayout = 2131690084;
				Widget_Design_Snackbar = 2131690085;
				Widget_Design_TabLayout = 2131690086;
				Widget_Design_TextInputLayout = 2131690087;
				Widget_MaterialComponents_ActionBar_Primary = 2131690088;
				Widget_MaterialComponents_ActionBar_PrimarySurface = 2131690089;
				Widget_MaterialComponents_ActionBar_Solid = 2131690090;
				Widget_MaterialComponents_ActionBar_Surface = 2131690091;
				Widget_MaterialComponents_AppBarLayout_Primary = 2131690092;
				Widget_MaterialComponents_AppBarLayout_PrimarySurface = 2131690093;
				Widget_MaterialComponents_AppBarLayout_Surface = 2131690094;
				Widget_MaterialComponents_AutoCompleteTextView_FilledBox = 2131690095;
				Widget_MaterialComponents_AutoCompleteTextView_FilledBox_Dense = 2131690096;
				Widget_MaterialComponents_AutoCompleteTextView_OutlinedBox = 2131690097;
				Widget_MaterialComponents_AutoCompleteTextView_OutlinedBox_Dense = 2131690098;
				Widget_MaterialComponents_Badge = 2131690099;
				Widget_MaterialComponents_BottomAppBar = 2131690100;
				Widget_MaterialComponents_BottomAppBar_Colored = 2131690101;
				Widget_MaterialComponents_BottomAppBar_PrimarySurface = 2131690102;
				Widget_MaterialComponents_BottomNavigationView = 2131690103;
				Widget_MaterialComponents_BottomNavigationView_Colored = 2131690104;
				Widget_MaterialComponents_BottomNavigationView_PrimarySurface = 2131690105;
				Widget_MaterialComponents_BottomSheet = 2131690106;
				Widget_MaterialComponents_BottomSheet_Modal = 2131690107;
				Widget_MaterialComponents_Button = 2131690108;
				Widget_MaterialComponents_Button_Icon = 2131690109;
				Widget_MaterialComponents_Button_OutlinedButton = 2131690110;
				Widget_MaterialComponents_Button_OutlinedButton_Icon = 2131690111;
				Widget_MaterialComponents_Button_TextButton = 2131690112;
				Widget_MaterialComponents_Button_TextButton_Dialog = 2131690113;
				Widget_MaterialComponents_Button_TextButton_Dialog_Flush = 2131690114;
				Widget_MaterialComponents_Button_TextButton_Dialog_Icon = 2131690115;
				Widget_MaterialComponents_Button_TextButton_Icon = 2131690116;
				Widget_MaterialComponents_Button_TextButton_Snackbar = 2131690117;
				Widget_MaterialComponents_Button_UnelevatedButton = 2131690118;
				Widget_MaterialComponents_Button_UnelevatedButton_Icon = 2131690119;
				Widget_MaterialComponents_CardView = 2131690120;
				Widget_MaterialComponents_CheckedTextView = 2131690121;
				Widget_MaterialComponents_ChipGroup = 2131690126;
				Widget_MaterialComponents_Chip_Action = 2131690122;
				Widget_MaterialComponents_Chip_Choice = 2131690123;
				Widget_MaterialComponents_Chip_Entry = 2131690124;
				Widget_MaterialComponents_Chip_Filter = 2131690125;
				Widget_MaterialComponents_CompoundButton_CheckBox = 2131690127;
				Widget_MaterialComponents_CompoundButton_RadioButton = 2131690128;
				Widget_MaterialComponents_CompoundButton_Switch = 2131690129;
				Widget_MaterialComponents_ExtendedFloatingActionButton = 2131690130;
				Widget_MaterialComponents_ExtendedFloatingActionButton_Icon = 2131690131;
				Widget_MaterialComponents_FloatingActionButton = 2131690132;
				Widget_MaterialComponents_Light_ActionBar_Solid = 2131690133;
				Widget_MaterialComponents_MaterialButtonToggleGroup = 2131690134;
				Widget_MaterialComponents_MaterialCalendar = 2131690135;
				Widget_MaterialComponents_MaterialCalendar_Day = 2131690136;
				Widget_MaterialComponents_MaterialCalendar_DayTextView = 2131690140;
				Widget_MaterialComponents_MaterialCalendar_Day_Invalid = 2131690137;
				Widget_MaterialComponents_MaterialCalendar_Day_Selected = 2131690138;
				Widget_MaterialComponents_MaterialCalendar_Day_Today = 2131690139;
				Widget_MaterialComponents_MaterialCalendar_Fullscreen = 2131690141;
				Widget_MaterialComponents_MaterialCalendar_HeaderConfirmButton = 2131690142;
				Widget_MaterialComponents_MaterialCalendar_HeaderDivider = 2131690143;
				Widget_MaterialComponents_MaterialCalendar_HeaderLayout = 2131690144;
				Widget_MaterialComponents_MaterialCalendar_HeaderSelection = 2131690145;
				Widget_MaterialComponents_MaterialCalendar_HeaderSelection_Fullscreen = 2131690146;
				Widget_MaterialComponents_MaterialCalendar_HeaderTitle = 2131690147;
				Widget_MaterialComponents_MaterialCalendar_HeaderToggleButton = 2131690148;
				Widget_MaterialComponents_MaterialCalendar_Item = 2131690149;
				Widget_MaterialComponents_MaterialCalendar_Year = 2131690150;
				Widget_MaterialComponents_MaterialCalendar_Year_Selected = 2131690151;
				Widget_MaterialComponents_MaterialCalendar_Year_Today = 2131690152;
				Widget_MaterialComponents_NavigationView = 2131690153;
				Widget_MaterialComponents_PopupMenu = 2131690154;
				Widget_MaterialComponents_PopupMenu_ContextMenu = 2131690155;
				Widget_MaterialComponents_PopupMenu_ListPopupWindow = 2131690156;
				Widget_MaterialComponents_PopupMenu_Overflow = 2131690157;
				Widget_MaterialComponents_Snackbar = 2131690158;
				Widget_MaterialComponents_Snackbar_FullWidth = 2131690159;
				Widget_MaterialComponents_TabLayout = 2131690160;
				Widget_MaterialComponents_TabLayout_Colored = 2131690161;
				Widget_MaterialComponents_TabLayout_PrimarySurface = 2131690162;
				Widget_MaterialComponents_TextInputEditText_FilledBox = 2131690163;
				Widget_MaterialComponents_TextInputEditText_FilledBox_Dense = 2131690164;
				Widget_MaterialComponents_TextInputEditText_OutlinedBox = 2131690165;
				Widget_MaterialComponents_TextInputEditText_OutlinedBox_Dense = 2131690166;
				Widget_MaterialComponents_TextInputLayout_FilledBox = 2131690167;
				Widget_MaterialComponents_TextInputLayout_FilledBox_Dense = 2131690168;
				Widget_MaterialComponents_TextInputLayout_FilledBox_Dense_ExposedDropdownMenu = 2131690169;
				Widget_MaterialComponents_TextInputLayout_FilledBox_ExposedDropdownMenu = 2131690170;
				Widget_MaterialComponents_TextInputLayout_OutlinedBox = 2131690171;
				Widget_MaterialComponents_TextInputLayout_OutlinedBox_Dense = 2131690172;
				Widget_MaterialComponents_TextInputLayout_OutlinedBox_Dense_ExposedDropdownMenu = 2131690173;
				Widget_MaterialComponents_TextInputLayout_OutlinedBox_ExposedDropdownMenu = 2131690174;
				Widget_MaterialComponents_TextView = 2131690175;
				Widget_MaterialComponents_Toolbar = 2131690176;
				Widget_MaterialComponents_Toolbar_Primary = 2131690177;
				Widget_MaterialComponents_Toolbar_PrimarySurface = 2131690178;
				Widget_MaterialComponents_Toolbar_Surface = 2131690179;
				Widget_Support_CoordinatorLayout = 2131690180;
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

			public static int ActionBarLayout_android_layout_gravity;

			public static int ActionBar_background;

			public static int ActionBar_backgroundSplit;

			public static int ActionBar_backgroundStacked;

			public static int ActionBar_contentInsetEnd;

			public static int ActionBar_contentInsetEndWithActions;

			public static int ActionBar_contentInsetLeft;

			public static int ActionBar_contentInsetRight;

			public static int ActionBar_contentInsetStart;

			public static int ActionBar_contentInsetStartWithNavigation;

			public static int ActionBar_customNavigationLayout;

			public static int ActionBar_displayOptions;

			public static int ActionBar_divider;

			public static int ActionBar_elevation;

			public static int ActionBar_height;

			public static int ActionBar_hideOnContentScroll;

			public static int ActionBar_homeAsUpIndicator;

			public static int ActionBar_homeLayout;

			public static int ActionBar_icon;

			public static int ActionBar_indeterminateProgressStyle;

			public static int ActionBar_itemPadding;

			public static int ActionBar_logo;

			public static int ActionBar_navigationMode;

			public static int ActionBar_popupTheme;

			public static int ActionBar_progressBarPadding;

			public static int ActionBar_progressBarStyle;

			public static int ActionBar_subtitle;

			public static int ActionBar_subtitleTextStyle;

			public static int ActionBar_title;

			public static int ActionBar_titleTextStyle;

			public static int[] ActionMenuItemView;

			public static int ActionMenuItemView_android_minWidth;

			public static int[] ActionMenuView;

			public static int[] ActionMode;

			public static int ActionMode_background;

			public static int ActionMode_backgroundSplit;

			public static int ActionMode_closeItemLayout;

			public static int ActionMode_height;

			public static int ActionMode_subtitleTextStyle;

			public static int ActionMode_titleTextStyle;

			public static int[] ActivityChooserView;

			public static int ActivityChooserView_expandActivityOverflowButtonDrawable;

			public static int ActivityChooserView_initialActivityCount;

			public static int[] AlertDialog;

			public static int AlertDialog_android_layout;

			public static int AlertDialog_buttonIconDimen;

			public static int AlertDialog_buttonPanelSideLayout;

			public static int AlertDialog_listItemLayout;

			public static int AlertDialog_listLayout;

			public static int AlertDialog_multiChoiceItemLayout;

			public static int AlertDialog_showTitle;

			public static int AlertDialog_singleChoiceItemLayout;

			public static int[] AnimatedStateListDrawableCompat;

			public static int AnimatedStateListDrawableCompat_android_constantSize;

			public static int AnimatedStateListDrawableCompat_android_dither;

			public static int AnimatedStateListDrawableCompat_android_enterFadeDuration;

			public static int AnimatedStateListDrawableCompat_android_exitFadeDuration;

			public static int AnimatedStateListDrawableCompat_android_variablePadding;

			public static int AnimatedStateListDrawableCompat_android_visible;

			public static int[] AnimatedStateListDrawableItem;

			public static int AnimatedStateListDrawableItem_android_drawable;

			public static int AnimatedStateListDrawableItem_android_id;

			public static int[] AnimatedStateListDrawableTransition;

			public static int AnimatedStateListDrawableTransition_android_drawable;

			public static int AnimatedStateListDrawableTransition_android_fromId;

			public static int AnimatedStateListDrawableTransition_android_reversible;

			public static int AnimatedStateListDrawableTransition_android_toId;

			public static int[] AppBarLayout;

			public static int[] AppBarLayoutStates;

			public static int AppBarLayoutStates_state_collapsed;

			public static int AppBarLayoutStates_state_collapsible;

			public static int AppBarLayoutStates_state_liftable;

			public static int AppBarLayoutStates_state_lifted;

			public static int AppBarLayout_android_background;

			public static int AppBarLayout_android_keyboardNavigationCluster;

			public static int AppBarLayout_android_touchscreenBlocksFocus;

			public static int AppBarLayout_elevation;

			public static int AppBarLayout_expanded;

			public static int[] AppBarLayout_Layout;

			public static int AppBarLayout_Layout_layout_scrollFlags;

			public static int AppBarLayout_Layout_layout_scrollInterpolator;

			public static int AppBarLayout_liftOnScroll;

			public static int AppBarLayout_liftOnScrollTargetViewId;

			public static int AppBarLayout_statusBarForeground;

			public static int[] AppCompatImageView;

			public static int AppCompatImageView_android_src;

			public static int AppCompatImageView_srcCompat;

			public static int AppCompatImageView_tint;

			public static int AppCompatImageView_tintMode;

			public static int[] AppCompatSeekBar;

			public static int AppCompatSeekBar_android_thumb;

			public static int AppCompatSeekBar_tickMark;

			public static int AppCompatSeekBar_tickMarkTint;

			public static int AppCompatSeekBar_tickMarkTintMode;

			public static int[] AppCompatTextHelper;

			public static int AppCompatTextHelper_android_drawableBottom;

			public static int AppCompatTextHelper_android_drawableEnd;

			public static int AppCompatTextHelper_android_drawableLeft;

			public static int AppCompatTextHelper_android_drawableRight;

			public static int AppCompatTextHelper_android_drawableStart;

			public static int AppCompatTextHelper_android_drawableTop;

			public static int AppCompatTextHelper_android_textAppearance;

			public static int[] AppCompatTextView;

			public static int AppCompatTextView_android_textAppearance;

			public static int AppCompatTextView_autoSizeMaxTextSize;

			public static int AppCompatTextView_autoSizeMinTextSize;

			public static int AppCompatTextView_autoSizePresetSizes;

			public static int AppCompatTextView_autoSizeStepGranularity;

			public static int AppCompatTextView_autoSizeTextType;

			public static int AppCompatTextView_drawableBottomCompat;

			public static int AppCompatTextView_drawableEndCompat;

			public static int AppCompatTextView_drawableLeftCompat;

			public static int AppCompatTextView_drawableRightCompat;

			public static int AppCompatTextView_drawableStartCompat;

			public static int AppCompatTextView_drawableTint;

			public static int AppCompatTextView_drawableTintMode;

			public static int AppCompatTextView_drawableTopCompat;

			public static int AppCompatTextView_firstBaselineToTopHeight;

			public static int AppCompatTextView_fontFamily;

			public static int AppCompatTextView_fontVariationSettings;

			public static int AppCompatTextView_lastBaselineToBottomHeight;

			public static int AppCompatTextView_lineHeight;

			public static int AppCompatTextView_textAllCaps;

			public static int AppCompatTextView_textLocale;

			public static int[] AppCompatTheme;

			public static int AppCompatTheme_actionBarDivider;

			public static int AppCompatTheme_actionBarItemBackground;

			public static int AppCompatTheme_actionBarPopupTheme;

			public static int AppCompatTheme_actionBarSize;

			public static int AppCompatTheme_actionBarSplitStyle;

			public static int AppCompatTheme_actionBarStyle;

			public static int AppCompatTheme_actionBarTabBarStyle;

			public static int AppCompatTheme_actionBarTabStyle;

			public static int AppCompatTheme_actionBarTabTextStyle;

			public static int AppCompatTheme_actionBarTheme;

			public static int AppCompatTheme_actionBarWidgetTheme;

			public static int AppCompatTheme_actionButtonStyle;

			public static int AppCompatTheme_actionDropDownStyle;

			public static int AppCompatTheme_actionMenuTextAppearance;

			public static int AppCompatTheme_actionMenuTextColor;

			public static int AppCompatTheme_actionModeBackground;

			public static int AppCompatTheme_actionModeCloseButtonStyle;

			public static int AppCompatTheme_actionModeCloseDrawable;

			public static int AppCompatTheme_actionModeCopyDrawable;

			public static int AppCompatTheme_actionModeCutDrawable;

			public static int AppCompatTheme_actionModeFindDrawable;

			public static int AppCompatTheme_actionModePasteDrawable;

			public static int AppCompatTheme_actionModePopupWindowStyle;

			public static int AppCompatTheme_actionModeSelectAllDrawable;

			public static int AppCompatTheme_actionModeShareDrawable;

			public static int AppCompatTheme_actionModeSplitBackground;

			public static int AppCompatTheme_actionModeStyle;

			public static int AppCompatTheme_actionModeWebSearchDrawable;

			public static int AppCompatTheme_actionOverflowButtonStyle;

			public static int AppCompatTheme_actionOverflowMenuStyle;

			public static int AppCompatTheme_activityChooserViewStyle;

			public static int AppCompatTheme_alertDialogButtonGroupStyle;

			public static int AppCompatTheme_alertDialogCenterButtons;

			public static int AppCompatTheme_alertDialogStyle;

			public static int AppCompatTheme_alertDialogTheme;

			public static int AppCompatTheme_android_windowAnimationStyle;

			public static int AppCompatTheme_android_windowIsFloating;

			public static int AppCompatTheme_autoCompleteTextViewStyle;

			public static int AppCompatTheme_borderlessButtonStyle;

			public static int AppCompatTheme_buttonBarButtonStyle;

			public static int AppCompatTheme_buttonBarNegativeButtonStyle;

			public static int AppCompatTheme_buttonBarNeutralButtonStyle;

			public static int AppCompatTheme_buttonBarPositiveButtonStyle;

			public static int AppCompatTheme_buttonBarStyle;

			public static int AppCompatTheme_buttonStyle;

			public static int AppCompatTheme_buttonStyleSmall;

			public static int AppCompatTheme_checkboxStyle;

			public static int AppCompatTheme_checkedTextViewStyle;

			public static int AppCompatTheme_colorAccent;

			public static int AppCompatTheme_colorBackgroundFloating;

			public static int AppCompatTheme_colorButtonNormal;

			public static int AppCompatTheme_colorControlActivated;

			public static int AppCompatTheme_colorControlHighlight;

			public static int AppCompatTheme_colorControlNormal;

			public static int AppCompatTheme_colorError;

			public static int AppCompatTheme_colorPrimary;

			public static int AppCompatTheme_colorPrimaryDark;

			public static int AppCompatTheme_colorSwitchThumbNormal;

			public static int AppCompatTheme_controlBackground;

			public static int AppCompatTheme_dialogCornerRadius;

			public static int AppCompatTheme_dialogPreferredPadding;

			public static int AppCompatTheme_dialogTheme;

			public static int AppCompatTheme_dividerHorizontal;

			public static int AppCompatTheme_dividerVertical;

			public static int AppCompatTheme_dropdownListPreferredItemHeight;

			public static int AppCompatTheme_dropDownListViewStyle;

			public static int AppCompatTheme_editTextBackground;

			public static int AppCompatTheme_editTextColor;

			public static int AppCompatTheme_editTextStyle;

			public static int AppCompatTheme_homeAsUpIndicator;

			public static int AppCompatTheme_imageButtonStyle;

			public static int AppCompatTheme_listChoiceBackgroundIndicator;

			public static int AppCompatTheme_listChoiceIndicatorMultipleAnimated;

			public static int AppCompatTheme_listChoiceIndicatorSingleAnimated;

			public static int AppCompatTheme_listDividerAlertDialog;

			public static int AppCompatTheme_listMenuViewStyle;

			public static int AppCompatTheme_listPopupWindowStyle;

			public static int AppCompatTheme_listPreferredItemHeight;

			public static int AppCompatTheme_listPreferredItemHeightLarge;

			public static int AppCompatTheme_listPreferredItemHeightSmall;

			public static int AppCompatTheme_listPreferredItemPaddingEnd;

			public static int AppCompatTheme_listPreferredItemPaddingLeft;

			public static int AppCompatTheme_listPreferredItemPaddingRight;

			public static int AppCompatTheme_listPreferredItemPaddingStart;

			public static int AppCompatTheme_panelBackground;

			public static int AppCompatTheme_panelMenuListTheme;

			public static int AppCompatTheme_panelMenuListWidth;

			public static int AppCompatTheme_popupMenuStyle;

			public static int AppCompatTheme_popupWindowStyle;

			public static int AppCompatTheme_radioButtonStyle;

			public static int AppCompatTheme_ratingBarStyle;

			public static int AppCompatTheme_ratingBarStyleIndicator;

			public static int AppCompatTheme_ratingBarStyleSmall;

			public static int AppCompatTheme_searchViewStyle;

			public static int AppCompatTheme_seekBarStyle;

			public static int AppCompatTheme_selectableItemBackground;

			public static int AppCompatTheme_selectableItemBackgroundBorderless;

			public static int AppCompatTheme_spinnerDropDownItemStyle;

			public static int AppCompatTheme_spinnerStyle;

			public static int AppCompatTheme_switchStyle;

			public static int AppCompatTheme_textAppearanceLargePopupMenu;

			public static int AppCompatTheme_textAppearanceListItem;

			public static int AppCompatTheme_textAppearanceListItemSecondary;

			public static int AppCompatTheme_textAppearanceListItemSmall;

			public static int AppCompatTheme_textAppearancePopupMenuHeader;

			public static int AppCompatTheme_textAppearanceSearchResultSubtitle;

			public static int AppCompatTheme_textAppearanceSearchResultTitle;

			public static int AppCompatTheme_textAppearanceSmallPopupMenu;

			public static int AppCompatTheme_textColorAlertDialogListItem;

			public static int AppCompatTheme_textColorSearchUrl;

			public static int AppCompatTheme_toolbarNavigationButtonStyle;

			public static int AppCompatTheme_toolbarStyle;

			public static int AppCompatTheme_tooltipForegroundColor;

			public static int AppCompatTheme_tooltipFrameBackground;

			public static int AppCompatTheme_viewInflaterClass;

			public static int AppCompatTheme_windowActionBar;

			public static int AppCompatTheme_windowActionBarOverlay;

			public static int AppCompatTheme_windowActionModeOverlay;

			public static int AppCompatTheme_windowFixedHeightMajor;

			public static int AppCompatTheme_windowFixedHeightMinor;

			public static int AppCompatTheme_windowFixedWidthMajor;

			public static int AppCompatTheme_windowFixedWidthMinor;

			public static int AppCompatTheme_windowMinWidthMajor;

			public static int AppCompatTheme_windowMinWidthMinor;

			public static int AppCompatTheme_windowNoTitle;

			public static int[] Badge;

			public static int Badge_backgroundColor;

			public static int Badge_badgeGravity;

			public static int Badge_badgeTextColor;

			public static int Badge_maxCharacterCount;

			public static int Badge_number;

			public static int[] BottomAppBar;

			public static int BottomAppBar_backgroundTint;

			public static int BottomAppBar_elevation;

			public static int BottomAppBar_fabAlignmentMode;

			public static int BottomAppBar_fabAnimationMode;

			public static int BottomAppBar_fabCradleMargin;

			public static int BottomAppBar_fabCradleRoundedCornerRadius;

			public static int BottomAppBar_fabCradleVerticalOffset;

			public static int BottomAppBar_hideOnScroll;

			public static int[] BottomNavigationView;

			public static int BottomNavigationView_backgroundTint;

			public static int BottomNavigationView_elevation;

			public static int BottomNavigationView_itemBackground;

			public static int BottomNavigationView_itemHorizontalTranslationEnabled;

			public static int BottomNavigationView_itemIconSize;

			public static int BottomNavigationView_itemIconTint;

			public static int BottomNavigationView_itemRippleColor;

			public static int BottomNavigationView_itemTextAppearanceActive;

			public static int BottomNavigationView_itemTextAppearanceInactive;

			public static int BottomNavigationView_itemTextColor;

			public static int BottomNavigationView_labelVisibilityMode;

			public static int BottomNavigationView_menu;

			public static int[] BottomSheetBehavior_Layout;

			public static int BottomSheetBehavior_Layout_android_elevation;

			public static int BottomSheetBehavior_Layout_backgroundTint;

			public static int BottomSheetBehavior_Layout_behavior_expandedOffset;

			public static int BottomSheetBehavior_Layout_behavior_fitToContents;

			public static int BottomSheetBehavior_Layout_behavior_halfExpandedRatio;

			public static int BottomSheetBehavior_Layout_behavior_hideable;

			public static int BottomSheetBehavior_Layout_behavior_peekHeight;

			public static int BottomSheetBehavior_Layout_behavior_saveFlags;

			public static int BottomSheetBehavior_Layout_behavior_skipCollapsed;

			public static int BottomSheetBehavior_Layout_shapeAppearance;

			public static int BottomSheetBehavior_Layout_shapeAppearanceOverlay;

			public static int[] ButtonBarLayout;

			public static int ButtonBarLayout_allowStacking;

			public static int[] CardView;

			public static int CardView_android_minHeight;

			public static int CardView_android_minWidth;

			public static int CardView_cardBackgroundColor;

			public static int CardView_cardCornerRadius;

			public static int CardView_cardElevation;

			public static int CardView_cardMaxElevation;

			public static int CardView_cardPreventCornerOverlap;

			public static int CardView_cardUseCompatPadding;

			public static int CardView_contentPadding;

			public static int CardView_contentPaddingBottom;

			public static int CardView_contentPaddingLeft;

			public static int CardView_contentPaddingRight;

			public static int CardView_contentPaddingTop;

			public static int[] Chip;

			public static int[] ChipGroup;

			public static int ChipGroup_checkedChip;

			public static int ChipGroup_chipSpacing;

			public static int ChipGroup_chipSpacingHorizontal;

			public static int ChipGroup_chipSpacingVertical;

			public static int ChipGroup_singleLine;

			public static int ChipGroup_singleSelection;

			public static int Chip_android_checkable;

			public static int Chip_android_ellipsize;

			public static int Chip_android_maxWidth;

			public static int Chip_android_text;

			public static int Chip_android_textAppearance;

			public static int Chip_android_textColor;

			public static int Chip_checkedIcon;

			public static int Chip_checkedIconEnabled;

			public static int Chip_checkedIconVisible;

			public static int Chip_chipBackgroundColor;

			public static int Chip_chipCornerRadius;

			public static int Chip_chipEndPadding;

			public static int Chip_chipIcon;

			public static int Chip_chipIconEnabled;

			public static int Chip_chipIconSize;

			public static int Chip_chipIconTint;

			public static int Chip_chipIconVisible;

			public static int Chip_chipMinHeight;

			public static int Chip_chipMinTouchTargetSize;

			public static int Chip_chipStartPadding;

			public static int Chip_chipStrokeColor;

			public static int Chip_chipStrokeWidth;

			public static int Chip_chipSurfaceColor;

			public static int Chip_closeIcon;

			public static int Chip_closeIconEnabled;

			public static int Chip_closeIconEndPadding;

			public static int Chip_closeIconSize;

			public static int Chip_closeIconStartPadding;

			public static int Chip_closeIconTint;

			public static int Chip_closeIconVisible;

			public static int Chip_ensureMinTouchTargetSize;

			public static int Chip_hideMotionSpec;

			public static int Chip_iconEndPadding;

			public static int Chip_iconStartPadding;

			public static int Chip_rippleColor;

			public static int Chip_shapeAppearance;

			public static int Chip_shapeAppearanceOverlay;

			public static int Chip_showMotionSpec;

			public static int Chip_textEndPadding;

			public static int Chip_textStartPadding;

			public static int[] CollapsingToolbarLayout;

			public static int CollapsingToolbarLayout_collapsedTitleGravity;

			public static int CollapsingToolbarLayout_collapsedTitleTextAppearance;

			public static int CollapsingToolbarLayout_contentScrim;

			public static int CollapsingToolbarLayout_expandedTitleGravity;

			public static int CollapsingToolbarLayout_expandedTitleMargin;

			public static int CollapsingToolbarLayout_expandedTitleMarginBottom;

			public static int CollapsingToolbarLayout_expandedTitleMarginEnd;

			public static int CollapsingToolbarLayout_expandedTitleMarginStart;

			public static int CollapsingToolbarLayout_expandedTitleMarginTop;

			public static int CollapsingToolbarLayout_expandedTitleTextAppearance;

			public static int[] CollapsingToolbarLayout_Layout;

			public static int CollapsingToolbarLayout_Layout_layout_collapseMode;

			public static int CollapsingToolbarLayout_Layout_layout_collapseParallaxMultiplier;

			public static int CollapsingToolbarLayout_scrimAnimationDuration;

			public static int CollapsingToolbarLayout_scrimVisibleHeightTrigger;

			public static int CollapsingToolbarLayout_statusBarScrim;

			public static int CollapsingToolbarLayout_title;

			public static int CollapsingToolbarLayout_titleEnabled;

			public static int CollapsingToolbarLayout_toolbarId;

			public static int[] ColorStateListItem;

			public static int ColorStateListItem_alpha;

			public static int ColorStateListItem_android_alpha;

			public static int ColorStateListItem_android_color;

			public static int[] CompoundButton;

			public static int CompoundButton_android_button;

			public static int CompoundButton_buttonCompat;

			public static int CompoundButton_buttonTint;

			public static int CompoundButton_buttonTintMode;

			public static int[] ConstraintLayout_Layout;

			public static int ConstraintLayout_Layout_android_maxHeight;

			public static int ConstraintLayout_Layout_android_maxWidth;

			public static int ConstraintLayout_Layout_android_minHeight;

			public static int ConstraintLayout_Layout_android_minWidth;

			public static int ConstraintLayout_Layout_android_orientation;

			public static int ConstraintLayout_Layout_barrierAllowsGoneWidgets;

			public static int ConstraintLayout_Layout_barrierDirection;

			public static int ConstraintLayout_Layout_chainUseRtl;

			public static int ConstraintLayout_Layout_constraintSet;

			public static int ConstraintLayout_Layout_constraint_referenced_ids;

			public static int ConstraintLayout_Layout_layout_constrainedHeight;

			public static int ConstraintLayout_Layout_layout_constrainedWidth;

			public static int ConstraintLayout_Layout_layout_constraintBaseline_creator;

			public static int ConstraintLayout_Layout_layout_constraintBaseline_toBaselineOf;

			public static int ConstraintLayout_Layout_layout_constraintBottom_creator;

			public static int ConstraintLayout_Layout_layout_constraintBottom_toBottomOf;

			public static int ConstraintLayout_Layout_layout_constraintBottom_toTopOf;

			public static int ConstraintLayout_Layout_layout_constraintCircle;

			public static int ConstraintLayout_Layout_layout_constraintCircleAngle;

			public static int ConstraintLayout_Layout_layout_constraintCircleRadius;

			public static int ConstraintLayout_Layout_layout_constraintDimensionRatio;

			public static int ConstraintLayout_Layout_layout_constraintEnd_toEndOf;

			public static int ConstraintLayout_Layout_layout_constraintEnd_toStartOf;

			public static int ConstraintLayout_Layout_layout_constraintGuide_begin;

			public static int ConstraintLayout_Layout_layout_constraintGuide_end;

			public static int ConstraintLayout_Layout_layout_constraintGuide_percent;

			public static int ConstraintLayout_Layout_layout_constraintHeight_default;

			public static int ConstraintLayout_Layout_layout_constraintHeight_max;

			public static int ConstraintLayout_Layout_layout_constraintHeight_min;

			public static int ConstraintLayout_Layout_layout_constraintHeight_percent;

			public static int ConstraintLayout_Layout_layout_constraintHorizontal_bias;

			public static int ConstraintLayout_Layout_layout_constraintHorizontal_chainStyle;

			public static int ConstraintLayout_Layout_layout_constraintHorizontal_weight;

			public static int ConstraintLayout_Layout_layout_constraintLeft_creator;

			public static int ConstraintLayout_Layout_layout_constraintLeft_toLeftOf;

			public static int ConstraintLayout_Layout_layout_constraintLeft_toRightOf;

			public static int ConstraintLayout_Layout_layout_constraintRight_creator;

			public static int ConstraintLayout_Layout_layout_constraintRight_toLeftOf;

			public static int ConstraintLayout_Layout_layout_constraintRight_toRightOf;

			public static int ConstraintLayout_Layout_layout_constraintStart_toEndOf;

			public static int ConstraintLayout_Layout_layout_constraintStart_toStartOf;

			public static int ConstraintLayout_Layout_layout_constraintTop_creator;

			public static int ConstraintLayout_Layout_layout_constraintTop_toBottomOf;

			public static int ConstraintLayout_Layout_layout_constraintTop_toTopOf;

			public static int ConstraintLayout_Layout_layout_constraintVertical_bias;

			public static int ConstraintLayout_Layout_layout_constraintVertical_chainStyle;

			public static int ConstraintLayout_Layout_layout_constraintVertical_weight;

			public static int ConstraintLayout_Layout_layout_constraintWidth_default;

			public static int ConstraintLayout_Layout_layout_constraintWidth_max;

			public static int ConstraintLayout_Layout_layout_constraintWidth_min;

			public static int ConstraintLayout_Layout_layout_constraintWidth_percent;

			public static int ConstraintLayout_Layout_layout_editor_absoluteX;

			public static int ConstraintLayout_Layout_layout_editor_absoluteY;

			public static int ConstraintLayout_Layout_layout_goneMarginBottom;

			public static int ConstraintLayout_Layout_layout_goneMarginEnd;

			public static int ConstraintLayout_Layout_layout_goneMarginLeft;

			public static int ConstraintLayout_Layout_layout_goneMarginRight;

			public static int ConstraintLayout_Layout_layout_goneMarginStart;

			public static int ConstraintLayout_Layout_layout_goneMarginTop;

			public static int ConstraintLayout_Layout_layout_optimizationLevel;

			public static int[] ConstraintLayout_placeholder;

			public static int ConstraintLayout_placeholder_content;

			public static int ConstraintLayout_placeholder_emptyVisibility;

			public static int[] ConstraintSet;

			public static int ConstraintSet_android_alpha;

			public static int ConstraintSet_android_elevation;

			public static int ConstraintSet_android_id;

			public static int ConstraintSet_android_layout_height;

			public static int ConstraintSet_android_layout_marginBottom;

			public static int ConstraintSet_android_layout_marginEnd;

			public static int ConstraintSet_android_layout_marginLeft;

			public static int ConstraintSet_android_layout_marginRight;

			public static int ConstraintSet_android_layout_marginStart;

			public static int ConstraintSet_android_layout_marginTop;

			public static int ConstraintSet_android_layout_width;

			public static int ConstraintSet_android_maxHeight;

			public static int ConstraintSet_android_maxWidth;

			public static int ConstraintSet_android_minHeight;

			public static int ConstraintSet_android_minWidth;

			public static int ConstraintSet_android_orientation;

			public static int ConstraintSet_android_rotation;

			public static int ConstraintSet_android_rotationX;

			public static int ConstraintSet_android_rotationY;

			public static int ConstraintSet_android_scaleX;

			public static int ConstraintSet_android_scaleY;

			public static int ConstraintSet_android_transformPivotX;

			public static int ConstraintSet_android_transformPivotY;

			public static int ConstraintSet_android_translationX;

			public static int ConstraintSet_android_translationY;

			public static int ConstraintSet_android_translationZ;

			public static int ConstraintSet_android_visibility;

			public static int ConstraintSet_barrierAllowsGoneWidgets;

			public static int ConstraintSet_barrierDirection;

			public static int ConstraintSet_chainUseRtl;

			public static int ConstraintSet_constraint_referenced_ids;

			public static int ConstraintSet_layout_constrainedHeight;

			public static int ConstraintSet_layout_constrainedWidth;

			public static int ConstraintSet_layout_constraintBaseline_creator;

			public static int ConstraintSet_layout_constraintBaseline_toBaselineOf;

			public static int ConstraintSet_layout_constraintBottom_creator;

			public static int ConstraintSet_layout_constraintBottom_toBottomOf;

			public static int ConstraintSet_layout_constraintBottom_toTopOf;

			public static int ConstraintSet_layout_constraintCircle;

			public static int ConstraintSet_layout_constraintCircleAngle;

			public static int ConstraintSet_layout_constraintCircleRadius;

			public static int ConstraintSet_layout_constraintDimensionRatio;

			public static int ConstraintSet_layout_constraintEnd_toEndOf;

			public static int ConstraintSet_layout_constraintEnd_toStartOf;

			public static int ConstraintSet_layout_constraintGuide_begin;

			public static int ConstraintSet_layout_constraintGuide_end;

			public static int ConstraintSet_layout_constraintGuide_percent;

			public static int ConstraintSet_layout_constraintHeight_default;

			public static int ConstraintSet_layout_constraintHeight_max;

			public static int ConstraintSet_layout_constraintHeight_min;

			public static int ConstraintSet_layout_constraintHeight_percent;

			public static int ConstraintSet_layout_constraintHorizontal_bias;

			public static int ConstraintSet_layout_constraintHorizontal_chainStyle;

			public static int ConstraintSet_layout_constraintHorizontal_weight;

			public static int ConstraintSet_layout_constraintLeft_creator;

			public static int ConstraintSet_layout_constraintLeft_toLeftOf;

			public static int ConstraintSet_layout_constraintLeft_toRightOf;

			public static int ConstraintSet_layout_constraintRight_creator;

			public static int ConstraintSet_layout_constraintRight_toLeftOf;

			public static int ConstraintSet_layout_constraintRight_toRightOf;

			public static int ConstraintSet_layout_constraintStart_toEndOf;

			public static int ConstraintSet_layout_constraintStart_toStartOf;

			public static int ConstraintSet_layout_constraintTop_creator;

			public static int ConstraintSet_layout_constraintTop_toBottomOf;

			public static int ConstraintSet_layout_constraintTop_toTopOf;

			public static int ConstraintSet_layout_constraintVertical_bias;

			public static int ConstraintSet_layout_constraintVertical_chainStyle;

			public static int ConstraintSet_layout_constraintVertical_weight;

			public static int ConstraintSet_layout_constraintWidth_default;

			public static int ConstraintSet_layout_constraintWidth_max;

			public static int ConstraintSet_layout_constraintWidth_min;

			public static int ConstraintSet_layout_constraintWidth_percent;

			public static int ConstraintSet_layout_editor_absoluteX;

			public static int ConstraintSet_layout_editor_absoluteY;

			public static int ConstraintSet_layout_goneMarginBottom;

			public static int ConstraintSet_layout_goneMarginEnd;

			public static int ConstraintSet_layout_goneMarginLeft;

			public static int ConstraintSet_layout_goneMarginRight;

			public static int ConstraintSet_layout_goneMarginStart;

			public static int ConstraintSet_layout_goneMarginTop;

			public static int[] CoordinatorLayout;

			public static int CoordinatorLayout_keylines;

			public static int[] CoordinatorLayout_Layout;

			public static int CoordinatorLayout_Layout_android_layout_gravity;

			public static int CoordinatorLayout_Layout_layout_anchor;

			public static int CoordinatorLayout_Layout_layout_anchorGravity;

			public static int CoordinatorLayout_Layout_layout_behavior;

			public static int CoordinatorLayout_Layout_layout_dodgeInsetEdges;

			public static int CoordinatorLayout_Layout_layout_insetEdge;

			public static int CoordinatorLayout_Layout_layout_keyline;

			public static int CoordinatorLayout_statusBarBackground;

			public static int[] DrawerArrowToggle;

			public static int DrawerArrowToggle_arrowHeadLength;

			public static int DrawerArrowToggle_arrowShaftLength;

			public static int DrawerArrowToggle_barLength;

			public static int DrawerArrowToggle_color;

			public static int DrawerArrowToggle_drawableSize;

			public static int DrawerArrowToggle_gapBetweenBars;

			public static int DrawerArrowToggle_spinBars;

			public static int DrawerArrowToggle_thickness;

			public static int[] ExtendedFloatingActionButton;

			public static int[] ExtendedFloatingActionButton_Behavior_Layout;

			public static int ExtendedFloatingActionButton_Behavior_Layout_behavior_autoHide;

			public static int ExtendedFloatingActionButton_Behavior_Layout_behavior_autoShrink;

			public static int ExtendedFloatingActionButton_elevation;

			public static int ExtendedFloatingActionButton_extendMotionSpec;

			public static int ExtendedFloatingActionButton_hideMotionSpec;

			public static int ExtendedFloatingActionButton_showMotionSpec;

			public static int ExtendedFloatingActionButton_shrinkMotionSpec;

			public static int[] FloatingActionButton;

			public static int FloatingActionButton_backgroundTint;

			public static int FloatingActionButton_backgroundTintMode;

			public static int[] FloatingActionButton_Behavior_Layout;

			public static int FloatingActionButton_Behavior_Layout_behavior_autoHide;

			public static int FloatingActionButton_borderWidth;

			public static int FloatingActionButton_elevation;

			public static int FloatingActionButton_ensureMinTouchTargetSize;

			public static int FloatingActionButton_fabCustomSize;

			public static int FloatingActionButton_fabSize;

			public static int FloatingActionButton_hideMotionSpec;

			public static int FloatingActionButton_hoveredFocusedTranslationZ;

			public static int FloatingActionButton_maxImageSize;

			public static int FloatingActionButton_pressedTranslationZ;

			public static int FloatingActionButton_rippleColor;

			public static int FloatingActionButton_shapeAppearance;

			public static int FloatingActionButton_shapeAppearanceOverlay;

			public static int FloatingActionButton_showMotionSpec;

			public static int FloatingActionButton_useCompatPadding;

			public static int[] FlowLayout;

			public static int FlowLayout_itemSpacing;

			public static int FlowLayout_lineSpacing;

			public static int[] FontFamily;

			public static int[] FontFamilyFont;

			public static int FontFamilyFont_android_font;

			public static int FontFamilyFont_android_fontStyle;

			public static int FontFamilyFont_android_fontVariationSettings;

			public static int FontFamilyFont_android_fontWeight;

			public static int FontFamilyFont_android_ttcIndex;

			public static int FontFamilyFont_font;

			public static int FontFamilyFont_fontStyle;

			public static int FontFamilyFont_fontVariationSettings;

			public static int FontFamilyFont_fontWeight;

			public static int FontFamilyFont_ttcIndex;

			public static int FontFamily_fontProviderAuthority;

			public static int FontFamily_fontProviderCerts;

			public static int FontFamily_fontProviderFetchStrategy;

			public static int FontFamily_fontProviderFetchTimeout;

			public static int FontFamily_fontProviderPackage;

			public static int FontFamily_fontProviderQuery;

			public static int[] ForegroundLinearLayout;

			public static int ForegroundLinearLayout_android_foreground;

			public static int ForegroundLinearLayout_android_foregroundGravity;

			public static int ForegroundLinearLayout_foregroundInsidePadding;

			public static int[] Fragment;

			public static int[] FragmentContainerView;

			public static int FragmentContainerView_android_name;

			public static int FragmentContainerView_android_tag;

			public static int Fragment_android_id;

			public static int Fragment_android_name;

			public static int Fragment_android_tag;

			public static int[] GradientColor;

			public static int[] GradientColorItem;

			public static int GradientColorItem_android_color;

			public static int GradientColorItem_android_offset;

			public static int GradientColor_android_centerColor;

			public static int GradientColor_android_centerX;

			public static int GradientColor_android_centerY;

			public static int GradientColor_android_endColor;

			public static int GradientColor_android_endX;

			public static int GradientColor_android_endY;

			public static int GradientColor_android_gradientRadius;

			public static int GradientColor_android_startColor;

			public static int GradientColor_android_startX;

			public static int GradientColor_android_startY;

			public static int GradientColor_android_tileMode;

			public static int GradientColor_android_type;

			public static int[] LinearConstraintLayout;

			public static int LinearConstraintLayout_android_orientation;

			public static int[] LinearLayoutCompat;

			public static int LinearLayoutCompat_android_baselineAligned;

			public static int LinearLayoutCompat_android_baselineAlignedChildIndex;

			public static int LinearLayoutCompat_android_gravity;

			public static int LinearLayoutCompat_android_orientation;

			public static int LinearLayoutCompat_android_weightSum;

			public static int LinearLayoutCompat_divider;

			public static int LinearLayoutCompat_dividerPadding;

			public static int[] LinearLayoutCompat_Layout;

			public static int LinearLayoutCompat_Layout_android_layout_gravity;

			public static int LinearLayoutCompat_Layout_android_layout_height;

			public static int LinearLayoutCompat_Layout_android_layout_weight;

			public static int LinearLayoutCompat_Layout_android_layout_width;

			public static int LinearLayoutCompat_measureWithLargestChild;

			public static int LinearLayoutCompat_showDividers;

			public static int[] ListPopupWindow;

			public static int ListPopupWindow_android_dropDownHorizontalOffset;

			public static int ListPopupWindow_android_dropDownVerticalOffset;

			public static int[] LoadingImageView;

			public static int LoadingImageView_circleCrop;

			public static int LoadingImageView_imageAspectRatio;

			public static int LoadingImageView_imageAspectRatioAdjust;

			public static int[] MaterialAlertDialog;

			public static int[] MaterialAlertDialogTheme;

			public static int MaterialAlertDialogTheme_materialAlertDialogBodyTextStyle;

			public static int MaterialAlertDialogTheme_materialAlertDialogTheme;

			public static int MaterialAlertDialogTheme_materialAlertDialogTitleIconStyle;

			public static int MaterialAlertDialogTheme_materialAlertDialogTitlePanelStyle;

			public static int MaterialAlertDialogTheme_materialAlertDialogTitleTextStyle;

			public static int MaterialAlertDialog_backgroundInsetBottom;

			public static int MaterialAlertDialog_backgroundInsetEnd;

			public static int MaterialAlertDialog_backgroundInsetStart;

			public static int MaterialAlertDialog_backgroundInsetTop;

			public static int[] MaterialButton;

			public static int[] MaterialButtonToggleGroup;

			public static int MaterialButtonToggleGroup_checkedButton;

			public static int MaterialButtonToggleGroup_singleSelection;

			public static int MaterialButton_android_checkable;

			public static int MaterialButton_android_insetBottom;

			public static int MaterialButton_android_insetLeft;

			public static int MaterialButton_android_insetRight;

			public static int MaterialButton_android_insetTop;

			public static int MaterialButton_backgroundTint;

			public static int MaterialButton_backgroundTintMode;

			public static int MaterialButton_cornerRadius;

			public static int MaterialButton_elevation;

			public static int MaterialButton_icon;

			public static int MaterialButton_iconGravity;

			public static int MaterialButton_iconPadding;

			public static int MaterialButton_iconSize;

			public static int MaterialButton_iconTint;

			public static int MaterialButton_iconTintMode;

			public static int MaterialButton_rippleColor;

			public static int MaterialButton_shapeAppearance;

			public static int MaterialButton_shapeAppearanceOverlay;

			public static int MaterialButton_strokeColor;

			public static int MaterialButton_strokeWidth;

			public static int[] MaterialCalendar;

			public static int[] MaterialCalendarItem;

			public static int MaterialCalendarItem_android_insetBottom;

			public static int MaterialCalendarItem_android_insetLeft;

			public static int MaterialCalendarItem_android_insetRight;

			public static int MaterialCalendarItem_android_insetTop;

			public static int MaterialCalendarItem_itemFillColor;

			public static int MaterialCalendarItem_itemShapeAppearance;

			public static int MaterialCalendarItem_itemShapeAppearanceOverlay;

			public static int MaterialCalendarItem_itemStrokeColor;

			public static int MaterialCalendarItem_itemStrokeWidth;

			public static int MaterialCalendarItem_itemTextColor;

			public static int MaterialCalendar_android_windowFullscreen;

			public static int MaterialCalendar_dayInvalidStyle;

			public static int MaterialCalendar_daySelectedStyle;

			public static int MaterialCalendar_dayStyle;

			public static int MaterialCalendar_dayTodayStyle;

			public static int MaterialCalendar_rangeFillColor;

			public static int MaterialCalendar_yearSelectedStyle;

			public static int MaterialCalendar_yearStyle;

			public static int MaterialCalendar_yearTodayStyle;

			public static int[] MaterialCardView;

			public static int MaterialCardView_android_checkable;

			public static int MaterialCardView_cardForegroundColor;

			public static int MaterialCardView_checkedIcon;

			public static int MaterialCardView_checkedIconTint;

			public static int MaterialCardView_rippleColor;

			public static int MaterialCardView_shapeAppearance;

			public static int MaterialCardView_shapeAppearanceOverlay;

			public static int MaterialCardView_state_dragged;

			public static int MaterialCardView_strokeColor;

			public static int MaterialCardView_strokeWidth;

			public static int[] MaterialCheckBox;

			public static int MaterialCheckBox_buttonTint;

			public static int MaterialCheckBox_useMaterialThemeColors;

			public static int[] MaterialRadioButton;

			public static int MaterialRadioButton_useMaterialThemeColors;

			public static int[] MaterialShape;

			public static int MaterialShape_shapeAppearance;

			public static int MaterialShape_shapeAppearanceOverlay;

			public static int[] MaterialTextAppearance;

			public static int MaterialTextAppearance_android_lineHeight;

			public static int MaterialTextAppearance_lineHeight;

			public static int[] MaterialTextView;

			public static int MaterialTextView_android_lineHeight;

			public static int MaterialTextView_android_textAppearance;

			public static int MaterialTextView_lineHeight;

			public static int[] MenuGroup;

			public static int MenuGroup_android_checkableBehavior;

			public static int MenuGroup_android_enabled;

			public static int MenuGroup_android_id;

			public static int MenuGroup_android_menuCategory;

			public static int MenuGroup_android_orderInCategory;

			public static int MenuGroup_android_visible;

			public static int[] MenuItem;

			public static int MenuItem_actionLayout;

			public static int MenuItem_actionProviderClass;

			public static int MenuItem_actionViewClass;

			public static int MenuItem_alphabeticModifiers;

			public static int MenuItem_android_alphabeticShortcut;

			public static int MenuItem_android_checkable;

			public static int MenuItem_android_checked;

			public static int MenuItem_android_enabled;

			public static int MenuItem_android_icon;

			public static int MenuItem_android_id;

			public static int MenuItem_android_menuCategory;

			public static int MenuItem_android_numericShortcut;

			public static int MenuItem_android_onClick;

			public static int MenuItem_android_orderInCategory;

			public static int MenuItem_android_title;

			public static int MenuItem_android_titleCondensed;

			public static int MenuItem_android_visible;

			public static int MenuItem_contentDescription;

			public static int MenuItem_iconTint;

			public static int MenuItem_iconTintMode;

			public static int MenuItem_numericModifiers;

			public static int MenuItem_showAsAction;

			public static int MenuItem_tooltipText;

			public static int[] MenuView;

			public static int MenuView_android_headerBackground;

			public static int MenuView_android_horizontalDivider;

			public static int MenuView_android_itemBackground;

			public static int MenuView_android_itemIconDisabledAlpha;

			public static int MenuView_android_itemTextAppearance;

			public static int MenuView_android_verticalDivider;

			public static int MenuView_android_windowAnimationStyle;

			public static int MenuView_preserveIconSpacing;

			public static int MenuView_subMenuArrow;

			public static int[] NavigationView;

			public static int NavigationView_android_background;

			public static int NavigationView_android_fitsSystemWindows;

			public static int NavigationView_android_maxWidth;

			public static int NavigationView_elevation;

			public static int NavigationView_headerLayout;

			public static int NavigationView_itemBackground;

			public static int NavigationView_itemHorizontalPadding;

			public static int NavigationView_itemIconPadding;

			public static int NavigationView_itemIconSize;

			public static int NavigationView_itemIconTint;

			public static int NavigationView_itemMaxLines;

			public static int NavigationView_itemShapeAppearance;

			public static int NavigationView_itemShapeAppearanceOverlay;

			public static int NavigationView_itemShapeFillColor;

			public static int NavigationView_itemShapeInsetBottom;

			public static int NavigationView_itemShapeInsetEnd;

			public static int NavigationView_itemShapeInsetStart;

			public static int NavigationView_itemShapeInsetTop;

			public static int NavigationView_itemTextAppearance;

			public static int NavigationView_itemTextColor;

			public static int NavigationView_menu;

			public static int[] PopupWindow;

			public static int[] PopupWindowBackgroundState;

			public static int PopupWindowBackgroundState_state_above_anchor;

			public static int PopupWindow_android_popupAnimationStyle;

			public static int PopupWindow_android_popupBackground;

			public static int PopupWindow_overlapAnchor;

			public static int[] RecycleListView;

			public static int RecycleListView_paddingBottomNoButtons;

			public static int RecycleListView_paddingTopNoTitle;

			public static int[] RecyclerView;

			public static int RecyclerView_android_clipToPadding;

			public static int RecyclerView_android_descendantFocusability;

			public static int RecyclerView_android_orientation;

			public static int RecyclerView_fastScrollEnabled;

			public static int RecyclerView_fastScrollHorizontalThumbDrawable;

			public static int RecyclerView_fastScrollHorizontalTrackDrawable;

			public static int RecyclerView_fastScrollVerticalThumbDrawable;

			public static int RecyclerView_fastScrollVerticalTrackDrawable;

			public static int RecyclerView_layoutManager;

			public static int RecyclerView_reverseLayout;

			public static int RecyclerView_spanCount;

			public static int RecyclerView_stackFromEnd;

			public static int[] ScrimInsetsFrameLayout;

			public static int ScrimInsetsFrameLayout_insetForeground;

			public static int[] ScrollingViewBehavior_Layout;

			public static int ScrollingViewBehavior_Layout_behavior_overlapTop;

			public static int[] SearchView;

			public static int SearchView_android_focusable;

			public static int SearchView_android_imeOptions;

			public static int SearchView_android_inputType;

			public static int SearchView_android_maxWidth;

			public static int SearchView_closeIcon;

			public static int SearchView_commitIcon;

			public static int SearchView_defaultQueryHint;

			public static int SearchView_goIcon;

			public static int SearchView_iconifiedByDefault;

			public static int SearchView_layout;

			public static int SearchView_queryBackground;

			public static int SearchView_queryHint;

			public static int SearchView_searchHintIcon;

			public static int SearchView_searchIcon;

			public static int SearchView_submitBackground;

			public static int SearchView_suggestionRowLayout;

			public static int SearchView_voiceIcon;

			public static int[] ShapeAppearance;

			public static int ShapeAppearance_cornerFamily;

			public static int ShapeAppearance_cornerFamilyBottomLeft;

			public static int ShapeAppearance_cornerFamilyBottomRight;

			public static int ShapeAppearance_cornerFamilyTopLeft;

			public static int ShapeAppearance_cornerFamilyTopRight;

			public static int ShapeAppearance_cornerSize;

			public static int ShapeAppearance_cornerSizeBottomLeft;

			public static int ShapeAppearance_cornerSizeBottomRight;

			public static int ShapeAppearance_cornerSizeTopLeft;

			public static int ShapeAppearance_cornerSizeTopRight;

			public static int[] SignInButton;

			public static int SignInButton_buttonSize;

			public static int SignInButton_colorScheme;

			public static int SignInButton_scopeUris;

			public static int[] Snackbar;

			public static int[] SnackbarLayout;

			public static int SnackbarLayout_actionTextColorAlpha;

			public static int SnackbarLayout_android_maxWidth;

			public static int SnackbarLayout_animationMode;

			public static int SnackbarLayout_backgroundOverlayColorAlpha;

			public static int SnackbarLayout_elevation;

			public static int SnackbarLayout_maxActionInlineWidth;

			public static int Snackbar_snackbarButtonStyle;

			public static int Snackbar_snackbarStyle;

			public static int[] Spinner;

			public static int Spinner_android_dropDownWidth;

			public static int Spinner_android_entries;

			public static int Spinner_android_popupBackground;

			public static int Spinner_android_prompt;

			public static int Spinner_popupTheme;

			public static int[] StateListDrawable;

			public static int[] StateListDrawableItem;

			public static int StateListDrawableItem_android_drawable;

			public static int StateListDrawable_android_constantSize;

			public static int StateListDrawable_android_dither;

			public static int StateListDrawable_android_enterFadeDuration;

			public static int StateListDrawable_android_exitFadeDuration;

			public static int StateListDrawable_android_variablePadding;

			public static int StateListDrawable_android_visible;

			public static int[] SwitchCompat;

			public static int SwitchCompat_android_textOff;

			public static int SwitchCompat_android_textOn;

			public static int SwitchCompat_android_thumb;

			public static int SwitchCompat_showText;

			public static int SwitchCompat_splitTrack;

			public static int SwitchCompat_switchMinWidth;

			public static int SwitchCompat_switchPadding;

			public static int SwitchCompat_switchTextAppearance;

			public static int SwitchCompat_thumbTextPadding;

			public static int SwitchCompat_thumbTint;

			public static int SwitchCompat_thumbTintMode;

			public static int SwitchCompat_track;

			public static int SwitchCompat_trackTint;

			public static int SwitchCompat_trackTintMode;

			public static int[] SwitchMaterial;

			public static int SwitchMaterial_useMaterialThemeColors;

			public static int[] TabItem;

			public static int TabItem_android_icon;

			public static int TabItem_android_layout;

			public static int TabItem_android_text;

			public static int[] TabLayout;

			public static int TabLayout_tabBackground;

			public static int TabLayout_tabContentStart;

			public static int TabLayout_tabGravity;

			public static int TabLayout_tabIconTint;

			public static int TabLayout_tabIconTintMode;

			public static int TabLayout_tabIndicator;

			public static int TabLayout_tabIndicatorAnimationDuration;

			public static int TabLayout_tabIndicatorColor;

			public static int TabLayout_tabIndicatorFullWidth;

			public static int TabLayout_tabIndicatorGravity;

			public static int TabLayout_tabIndicatorHeight;

			public static int TabLayout_tabInlineLabel;

			public static int TabLayout_tabMaxWidth;

			public static int TabLayout_tabMinWidth;

			public static int TabLayout_tabMode;

			public static int TabLayout_tabPadding;

			public static int TabLayout_tabPaddingBottom;

			public static int TabLayout_tabPaddingEnd;

			public static int TabLayout_tabPaddingStart;

			public static int TabLayout_tabPaddingTop;

			public static int TabLayout_tabRippleColor;

			public static int TabLayout_tabSelectedTextColor;

			public static int TabLayout_tabTextAppearance;

			public static int TabLayout_tabTextColor;

			public static int TabLayout_tabUnboundedRipple;

			public static int[] TextAppearance;

			public static int TextAppearance_android_fontFamily;

			public static int TextAppearance_android_shadowColor;

			public static int TextAppearance_android_shadowDx;

			public static int TextAppearance_android_shadowDy;

			public static int TextAppearance_android_shadowRadius;

			public static int TextAppearance_android_textColor;

			public static int TextAppearance_android_textColorHint;

			public static int TextAppearance_android_textColorLink;

			public static int TextAppearance_android_textFontWeight;

			public static int TextAppearance_android_textSize;

			public static int TextAppearance_android_textStyle;

			public static int TextAppearance_android_typeface;

			public static int TextAppearance_fontFamily;

			public static int TextAppearance_fontVariationSettings;

			public static int TextAppearance_textAllCaps;

			public static int TextAppearance_textLocale;

			public static int[] TextInputLayout;

			public static int TextInputLayout_android_hint;

			public static int TextInputLayout_android_textColorHint;

			public static int TextInputLayout_boxBackgroundColor;

			public static int TextInputLayout_boxBackgroundMode;

			public static int TextInputLayout_boxCollapsedPaddingTop;

			public static int TextInputLayout_boxCornerRadiusBottomEnd;

			public static int TextInputLayout_boxCornerRadiusBottomStart;

			public static int TextInputLayout_boxCornerRadiusTopEnd;

			public static int TextInputLayout_boxCornerRadiusTopStart;

			public static int TextInputLayout_boxStrokeColor;

			public static int TextInputLayout_boxStrokeWidth;

			public static int TextInputLayout_boxStrokeWidthFocused;

			public static int TextInputLayout_counterEnabled;

			public static int TextInputLayout_counterMaxLength;

			public static int TextInputLayout_counterOverflowTextAppearance;

			public static int TextInputLayout_counterOverflowTextColor;

			public static int TextInputLayout_counterTextAppearance;

			public static int TextInputLayout_counterTextColor;

			public static int TextInputLayout_endIconCheckable;

			public static int TextInputLayout_endIconContentDescription;

			public static int TextInputLayout_endIconDrawable;

			public static int TextInputLayout_endIconMode;

			public static int TextInputLayout_endIconTint;

			public static int TextInputLayout_endIconTintMode;

			public static int TextInputLayout_errorEnabled;

			public static int TextInputLayout_errorIconDrawable;

			public static int TextInputLayout_errorIconTint;

			public static int TextInputLayout_errorIconTintMode;

			public static int TextInputLayout_errorTextAppearance;

			public static int TextInputLayout_errorTextColor;

			public static int TextInputLayout_helperText;

			public static int TextInputLayout_helperTextEnabled;

			public static int TextInputLayout_helperTextTextAppearance;

			public static int TextInputLayout_helperTextTextColor;

			public static int TextInputLayout_hintAnimationEnabled;

			public static int TextInputLayout_hintEnabled;

			public static int TextInputLayout_hintTextAppearance;

			public static int TextInputLayout_hintTextColor;

			public static int TextInputLayout_passwordToggleContentDescription;

			public static int TextInputLayout_passwordToggleDrawable;

			public static int TextInputLayout_passwordToggleEnabled;

			public static int TextInputLayout_passwordToggleTint;

			public static int TextInputLayout_passwordToggleTintMode;

			public static int TextInputLayout_shapeAppearance;

			public static int TextInputLayout_shapeAppearanceOverlay;

			public static int TextInputLayout_startIconCheckable;

			public static int TextInputLayout_startIconContentDescription;

			public static int TextInputLayout_startIconDrawable;

			public static int TextInputLayout_startIconTint;

			public static int TextInputLayout_startIconTintMode;

			public static int[] ThemeEnforcement;

			public static int ThemeEnforcement_android_textAppearance;

			public static int ThemeEnforcement_enforceMaterialTheme;

			public static int ThemeEnforcement_enforceTextAppearance;

			public static int[] Toolbar;

			public static int Toolbar_android_gravity;

			public static int Toolbar_android_minHeight;

			public static int Toolbar_buttonGravity;

			public static int Toolbar_collapseContentDescription;

			public static int Toolbar_collapseIcon;

			public static int Toolbar_contentInsetEnd;

			public static int Toolbar_contentInsetEndWithActions;

			public static int Toolbar_contentInsetLeft;

			public static int Toolbar_contentInsetRight;

			public static int Toolbar_contentInsetStart;

			public static int Toolbar_contentInsetStartWithNavigation;

			public static int Toolbar_logo;

			public static int Toolbar_logoDescription;

			public static int Toolbar_maxButtonHeight;

			public static int Toolbar_menu;

			public static int Toolbar_navigationContentDescription;

			public static int Toolbar_navigationIcon;

			public static int Toolbar_popupTheme;

			public static int Toolbar_subtitle;

			public static int Toolbar_subtitleTextAppearance;

			public static int Toolbar_subtitleTextColor;

			public static int Toolbar_title;

			public static int Toolbar_titleMargin;

			public static int Toolbar_titleMarginBottom;

			public static int Toolbar_titleMarginEnd;

			public static int Toolbar_titleMargins;

			public static int Toolbar_titleMarginStart;

			public static int Toolbar_titleMarginTop;

			public static int Toolbar_titleTextAppearance;

			public static int Toolbar_titleTextColor;

			public static int[] View;

			public static int[] ViewBackgroundHelper;

			public static int ViewBackgroundHelper_android_background;

			public static int ViewBackgroundHelper_backgroundTint;

			public static int ViewBackgroundHelper_backgroundTintMode;

			public static int[] ViewPager2;

			public static int ViewPager2_android_orientation;

			public static int[] ViewStubCompat;

			public static int ViewStubCompat_android_id;

			public static int ViewStubCompat_android_inflatedId;

			public static int ViewStubCompat_android_layout;

			public static int View_android_focusable;

			public static int View_android_theme;

			public static int View_paddingEnd;

			public static int View_paddingStart;

			public static int View_theme;

			static Styleable()
			{
				ActionBar = new int[29]
				{
					2130903092,
					2130903099,
					2130903100,
					2130903227,
					2130903228,
					2130903229,
					2130903230,
					2130903231,
					2130903232,
					2130903258,
					2130903267,
					2130903268,
					2130903287,
					2130903346,
					2130903352,
					2130903358,
					2130903359,
					2130903361,
					2130903373,
					2130903386,
					2130903486,
					2130903518,
					2130903537,
					2130903541,
					2130903542,
					2130903604,
					2130903607,
					2130903679,
					2130903689
				};
				ActionBarLayout = new int[1]
				{
					16842931
				};
				ActionBarLayout_android_layout_gravity = 0;
				ActionBar_background = 0;
				ActionBar_backgroundSplit = 1;
				ActionBar_backgroundStacked = 2;
				ActionBar_contentInsetEnd = 3;
				ActionBar_contentInsetEndWithActions = 4;
				ActionBar_contentInsetLeft = 5;
				ActionBar_contentInsetRight = 6;
				ActionBar_contentInsetStart = 7;
				ActionBar_contentInsetStartWithNavigation = 8;
				ActionBar_customNavigationLayout = 9;
				ActionBar_displayOptions = 10;
				ActionBar_divider = 11;
				ActionBar_elevation = 12;
				ActionBar_height = 13;
				ActionBar_hideOnContentScroll = 14;
				ActionBar_homeAsUpIndicator = 15;
				ActionBar_homeLayout = 16;
				ActionBar_icon = 17;
				ActionBar_indeterminateProgressStyle = 18;
				ActionBar_itemPadding = 19;
				ActionBar_logo = 20;
				ActionBar_navigationMode = 21;
				ActionBar_popupTheme = 22;
				ActionBar_progressBarPadding = 23;
				ActionBar_progressBarStyle = 24;
				ActionBar_subtitle = 25;
				ActionBar_subtitleTextStyle = 26;
				ActionBar_title = 27;
				ActionBar_titleTextStyle = 28;
				ActionMenuItemView = new int[1]
				{
					16843071
				};
				ActionMenuItemView_android_minWidth = 0;
				ActionMenuView = new int[1]
				{
					-1
				};
				ActionMode = new int[6]
				{
					2130903092,
					2130903099,
					2130903194,
					2130903346,
					2130903607,
					2130903689
				};
				ActionMode_background = 0;
				ActionMode_backgroundSplit = 1;
				ActionMode_closeItemLayout = 2;
				ActionMode_height = 3;
				ActionMode_subtitleTextStyle = 4;
				ActionMode_titleTextStyle = 5;
				ActivityChooserView = new int[2]
				{
					2130903306,
					2130903374
				};
				ActivityChooserView_expandActivityOverflowButtonDrawable = 0;
				ActivityChooserView_initialActivityCount = 1;
				AlertDialog = new int[8]
				{
					16842994,
					2130903142,
					2130903143,
					2130903475,
					2130903476,
					2130903515,
					2130903572,
					2130903574
				};
				AlertDialog_android_layout = 0;
				AlertDialog_buttonIconDimen = 1;
				AlertDialog_buttonPanelSideLayout = 2;
				AlertDialog_listItemLayout = 3;
				AlertDialog_listLayout = 4;
				AlertDialog_multiChoiceItemLayout = 5;
				AlertDialog_showTitle = 6;
				AlertDialog_singleChoiceItemLayout = 7;
				AnimatedStateListDrawableCompat = new int[6]
				{
					16843036,
					16843156,
					16843157,
					16843158,
					16843532,
					16843533
				};
				AnimatedStateListDrawableCompat_android_constantSize = 3;
				AnimatedStateListDrawableCompat_android_dither = 0;
				AnimatedStateListDrawableCompat_android_enterFadeDuration = 4;
				AnimatedStateListDrawableCompat_android_exitFadeDuration = 5;
				AnimatedStateListDrawableCompat_android_variablePadding = 2;
				AnimatedStateListDrawableCompat_android_visible = 1;
				AnimatedStateListDrawableItem = new int[2]
				{
					16842960,
					16843161
				};
				AnimatedStateListDrawableItem_android_drawable = 1;
				AnimatedStateListDrawableItem_android_id = 0;
				AnimatedStateListDrawableTransition = new int[4]
				{
					16843161,
					16843849,
					16843850,
					16843851
				};
				AnimatedStateListDrawableTransition_android_drawable = 0;
				AnimatedStateListDrawableTransition_android_fromId = 2;
				AnimatedStateListDrawableTransition_android_reversible = 3;
				AnimatedStateListDrawableTransition_android_toId = 1;
				AppBarLayout = new int[8]
				{
					16842964,
					16843919,
					16844096,
					2130903287,
					2130903307,
					2130903467,
					2130903468,
					2130903598
				};
				AppBarLayoutStates = new int[4]
				{
					2130903592,
					2130903593,
					2130903595,
					2130903596
				};
				AppBarLayoutStates_state_collapsed = 0;
				AppBarLayoutStates_state_collapsible = 1;
				AppBarLayoutStates_state_liftable = 2;
				AppBarLayoutStates_state_lifted = 3;
				AppBarLayout_android_background = 0;
				AppBarLayout_android_keyboardNavigationCluster = 2;
				AppBarLayout_android_touchscreenBlocksFocus = 1;
				AppBarLayout_elevation = 3;
				AppBarLayout_expanded = 4;
				AppBarLayout_Layout = new int[2]
				{
					2130903465,
					2130903466
				};
				AppBarLayout_Layout_layout_scrollFlags = 0;
				AppBarLayout_Layout_layout_scrollInterpolator = 1;
				AppBarLayout_liftOnScroll = 5;
				AppBarLayout_liftOnScrollTargetViewId = 6;
				AppBarLayout_statusBarForeground = 7;
				AppCompatImageView = new int[4]
				{
					16843033,
					2130903584,
					2130903677,
					2130903678
				};
				AppCompatImageView_android_src = 0;
				AppCompatImageView_srcCompat = 1;
				AppCompatImageView_tint = 2;
				AppCompatImageView_tintMode = 3;
				AppCompatSeekBar = new int[4]
				{
					16843074,
					2130903674,
					2130903675,
					2130903676
				};
				AppCompatSeekBar_android_thumb = 0;
				AppCompatSeekBar_tickMark = 1;
				AppCompatSeekBar_tickMarkTint = 2;
				AppCompatSeekBar_tickMarkTintMode = 3;
				AppCompatTextHelper = new int[7]
				{
					16842804,
					16843117,
					16843118,
					16843119,
					16843120,
					16843666,
					16843667
				};
				AppCompatTextHelper_android_drawableBottom = 2;
				AppCompatTextHelper_android_drawableEnd = 6;
				AppCompatTextHelper_android_drawableLeft = 3;
				AppCompatTextHelper_android_drawableRight = 4;
				AppCompatTextHelper_android_drawableStart = 5;
				AppCompatTextHelper_android_drawableTop = 1;
				AppCompatTextHelper_android_textAppearance = 0;
				AppCompatTextView = new int[21]
				{
					16842804,
					2130903087,
					2130903088,
					2130903089,
					2130903090,
					2130903091,
					2130903272,
					2130903273,
					2130903274,
					2130903275,
					2130903277,
					2130903278,
					2130903279,
					2130903280,
					2130903329,
					2130903332,
					2130903340,
					2130903404,
					2130903469,
					2130903639,
					2130903666
				};
				AppCompatTextView_android_textAppearance = 0;
				AppCompatTextView_autoSizeMaxTextSize = 1;
				AppCompatTextView_autoSizeMinTextSize = 2;
				AppCompatTextView_autoSizePresetSizes = 3;
				AppCompatTextView_autoSizeStepGranularity = 4;
				AppCompatTextView_autoSizeTextType = 5;
				AppCompatTextView_drawableBottomCompat = 6;
				AppCompatTextView_drawableEndCompat = 7;
				AppCompatTextView_drawableLeftCompat = 8;
				AppCompatTextView_drawableRightCompat = 9;
				AppCompatTextView_drawableStartCompat = 10;
				AppCompatTextView_drawableTint = 11;
				AppCompatTextView_drawableTintMode = 12;
				AppCompatTextView_drawableTopCompat = 13;
				AppCompatTextView_firstBaselineToTopHeight = 14;
				AppCompatTextView_fontFamily = 15;
				AppCompatTextView_fontVariationSettings = 16;
				AppCompatTextView_lastBaselineToBottomHeight = 17;
				AppCompatTextView_lineHeight = 18;
				AppCompatTextView_textAllCaps = 19;
				AppCompatTextView_textLocale = 20;
				AppCompatTheme = new int[125]
				{
					16842839,
					16842926,
					2130903040,
					2130903041,
					2130903042,
					2130903043,
					2130903044,
					2130903045,
					2130903046,
					2130903047,
					2130903048,
					2130903049,
					2130903050,
					2130903051,
					2130903052,
					2130903054,
					2130903055,
					2130903056,
					2130903057,
					2130903058,
					2130903059,
					2130903060,
					2130903061,
					2130903062,
					2130903063,
					2130903064,
					2130903065,
					2130903066,
					2130903067,
					2130903068,
					2130903069,
					2130903070,
					2130903074,
					2130903075,
					2130903076,
					2130903077,
					2130903078,
					2130903086,
					2130903120,
					2130903135,
					2130903136,
					2130903137,
					2130903138,
					2130903139,
					2130903145,
					2130903146,
					2130903158,
					2130903165,
					2130903200,
					2130903201,
					2130903202,
					2130903203,
					2130903204,
					2130903205,
					2130903206,
					2130903213,
					2130903214,
					2130903221,
					2130903239,
					2130903264,
					2130903265,
					2130903266,
					2130903269,
					2130903271,
					2130903282,
					2130903283,
					2130903284,
					2130903285,
					2130903286,
					2130903358,
					2130903372,
					2130903471,
					2130903472,
					2130903473,
					2130903474,
					2130903477,
					2130903478,
					2130903479,
					2130903480,
					2130903481,
					2130903482,
					2130903483,
					2130903484,
					2130903485,
					2130903527,
					2130903528,
					2130903529,
					2130903536,
					2130903538,
					2130903545,
					2130903547,
					2130903548,
					2130903549,
					2130903559,
					2130903560,
					2130903561,
					2130903562,
					2130903581,
					2130903582,
					2130903611,
					2130903650,
					2130903652,
					2130903653,
					2130903654,
					2130903656,
					2130903657,
					2130903658,
					2130903659,
					2130903662,
					2130903663,
					2130903691,
					2130903692,
					2130903693,
					2130903694,
					2130903702,
					2130903704,
					2130903705,
					2130903706,
					2130903707,
					2130903708,
					2130903709,
					2130903710,
					2130903711,
					2130903712,
					2130903713
				};
				AppCompatTheme_actionBarDivider = 2;
				AppCompatTheme_actionBarItemBackground = 3;
				AppCompatTheme_actionBarPopupTheme = 4;
				AppCompatTheme_actionBarSize = 5;
				AppCompatTheme_actionBarSplitStyle = 6;
				AppCompatTheme_actionBarStyle = 7;
				AppCompatTheme_actionBarTabBarStyle = 8;
				AppCompatTheme_actionBarTabStyle = 9;
				AppCompatTheme_actionBarTabTextStyle = 10;
				AppCompatTheme_actionBarTheme = 11;
				AppCompatTheme_actionBarWidgetTheme = 12;
				AppCompatTheme_actionButtonStyle = 13;
				AppCompatTheme_actionDropDownStyle = 14;
				AppCompatTheme_actionMenuTextAppearance = 15;
				AppCompatTheme_actionMenuTextColor = 16;
				AppCompatTheme_actionModeBackground = 17;
				AppCompatTheme_actionModeCloseButtonStyle = 18;
				AppCompatTheme_actionModeCloseDrawable = 19;
				AppCompatTheme_actionModeCopyDrawable = 20;
				AppCompatTheme_actionModeCutDrawable = 21;
				AppCompatTheme_actionModeFindDrawable = 22;
				AppCompatTheme_actionModePasteDrawable = 23;
				AppCompatTheme_actionModePopupWindowStyle = 24;
				AppCompatTheme_actionModeSelectAllDrawable = 25;
				AppCompatTheme_actionModeShareDrawable = 26;
				AppCompatTheme_actionModeSplitBackground = 27;
				AppCompatTheme_actionModeStyle = 28;
				AppCompatTheme_actionModeWebSearchDrawable = 29;
				AppCompatTheme_actionOverflowButtonStyle = 30;
				AppCompatTheme_actionOverflowMenuStyle = 31;
				AppCompatTheme_activityChooserViewStyle = 32;
				AppCompatTheme_alertDialogButtonGroupStyle = 33;
				AppCompatTheme_alertDialogCenterButtons = 34;
				AppCompatTheme_alertDialogStyle = 35;
				AppCompatTheme_alertDialogTheme = 36;
				AppCompatTheme_android_windowAnimationStyle = 1;
				AppCompatTheme_android_windowIsFloating = 0;
				AppCompatTheme_autoCompleteTextViewStyle = 37;
				AppCompatTheme_borderlessButtonStyle = 38;
				AppCompatTheme_buttonBarButtonStyle = 39;
				AppCompatTheme_buttonBarNegativeButtonStyle = 40;
				AppCompatTheme_buttonBarNeutralButtonStyle = 41;
				AppCompatTheme_buttonBarPositiveButtonStyle = 42;
				AppCompatTheme_buttonBarStyle = 43;
				AppCompatTheme_buttonStyle = 44;
				AppCompatTheme_buttonStyleSmall = 45;
				AppCompatTheme_checkboxStyle = 46;
				AppCompatTheme_checkedTextViewStyle = 47;
				AppCompatTheme_colorAccent = 48;
				AppCompatTheme_colorBackgroundFloating = 49;
				AppCompatTheme_colorButtonNormal = 50;
				AppCompatTheme_colorControlActivated = 51;
				AppCompatTheme_colorControlHighlight = 52;
				AppCompatTheme_colorControlNormal = 53;
				AppCompatTheme_colorError = 54;
				AppCompatTheme_colorPrimary = 55;
				AppCompatTheme_colorPrimaryDark = 56;
				AppCompatTheme_colorSwitchThumbNormal = 57;
				AppCompatTheme_controlBackground = 58;
				AppCompatTheme_dialogCornerRadius = 59;
				AppCompatTheme_dialogPreferredPadding = 60;
				AppCompatTheme_dialogTheme = 61;
				AppCompatTheme_dividerHorizontal = 62;
				AppCompatTheme_dividerVertical = 63;
				AppCompatTheme_dropdownListPreferredItemHeight = 65;
				AppCompatTheme_dropDownListViewStyle = 64;
				AppCompatTheme_editTextBackground = 66;
				AppCompatTheme_editTextColor = 67;
				AppCompatTheme_editTextStyle = 68;
				AppCompatTheme_homeAsUpIndicator = 69;
				AppCompatTheme_imageButtonStyle = 70;
				AppCompatTheme_listChoiceBackgroundIndicator = 71;
				AppCompatTheme_listChoiceIndicatorMultipleAnimated = 72;
				AppCompatTheme_listChoiceIndicatorSingleAnimated = 73;
				AppCompatTheme_listDividerAlertDialog = 74;
				AppCompatTheme_listMenuViewStyle = 75;
				AppCompatTheme_listPopupWindowStyle = 76;
				AppCompatTheme_listPreferredItemHeight = 77;
				AppCompatTheme_listPreferredItemHeightLarge = 78;
				AppCompatTheme_listPreferredItemHeightSmall = 79;
				AppCompatTheme_listPreferredItemPaddingEnd = 80;
				AppCompatTheme_listPreferredItemPaddingLeft = 81;
				AppCompatTheme_listPreferredItemPaddingRight = 82;
				AppCompatTheme_listPreferredItemPaddingStart = 83;
				AppCompatTheme_panelBackground = 84;
				AppCompatTheme_panelMenuListTheme = 85;
				AppCompatTheme_panelMenuListWidth = 86;
				AppCompatTheme_popupMenuStyle = 87;
				AppCompatTheme_popupWindowStyle = 88;
				AppCompatTheme_radioButtonStyle = 89;
				AppCompatTheme_ratingBarStyle = 90;
				AppCompatTheme_ratingBarStyleIndicator = 91;
				AppCompatTheme_ratingBarStyleSmall = 92;
				AppCompatTheme_searchViewStyle = 93;
				AppCompatTheme_seekBarStyle = 94;
				AppCompatTheme_selectableItemBackground = 95;
				AppCompatTheme_selectableItemBackgroundBorderless = 96;
				AppCompatTheme_spinnerDropDownItemStyle = 97;
				AppCompatTheme_spinnerStyle = 98;
				AppCompatTheme_switchStyle = 99;
				AppCompatTheme_textAppearanceLargePopupMenu = 100;
				AppCompatTheme_textAppearanceListItem = 101;
				AppCompatTheme_textAppearanceListItemSecondary = 102;
				AppCompatTheme_textAppearanceListItemSmall = 103;
				AppCompatTheme_textAppearancePopupMenuHeader = 104;
				AppCompatTheme_textAppearanceSearchResultSubtitle = 105;
				AppCompatTheme_textAppearanceSearchResultTitle = 106;
				AppCompatTheme_textAppearanceSmallPopupMenu = 107;
				AppCompatTheme_textColorAlertDialogListItem = 108;
				AppCompatTheme_textColorSearchUrl = 109;
				AppCompatTheme_toolbarNavigationButtonStyle = 110;
				AppCompatTheme_toolbarStyle = 111;
				AppCompatTheme_tooltipForegroundColor = 112;
				AppCompatTheme_tooltipFrameBackground = 113;
				AppCompatTheme_viewInflaterClass = 114;
				AppCompatTheme_windowActionBar = 115;
				AppCompatTheme_windowActionBarOverlay = 116;
				AppCompatTheme_windowActionModeOverlay = 117;
				AppCompatTheme_windowFixedHeightMajor = 118;
				AppCompatTheme_windowFixedHeightMinor = 119;
				AppCompatTheme_windowFixedWidthMajor = 120;
				AppCompatTheme_windowFixedWidthMinor = 121;
				AppCompatTheme_windowMinWidthMajor = 122;
				AppCompatTheme_windowMinWidthMinor = 123;
				AppCompatTheme_windowNoTitle = 124;
				Badge = new int[5]
				{
					2130903093,
					2130903103,
					2130903105,
					2130903510,
					2130903520
				};
				Badge_backgroundColor = 0;
				Badge_badgeGravity = 1;
				Badge_badgeTextColor = 2;
				Badge_maxCharacterCount = 3;
				Badge_number = 4;
				BottomAppBar = new int[8]
				{
					2130903101,
					2130903287,
					2130903317,
					2130903318,
					2130903319,
					2130903320,
					2130903321,
					2130903353
				};
				BottomAppBar_backgroundTint = 0;
				BottomAppBar_elevation = 1;
				BottomAppBar_fabAlignmentMode = 2;
				BottomAppBar_fabAnimationMode = 3;
				BottomAppBar_fabCradleMargin = 4;
				BottomAppBar_fabCradleRoundedCornerRadius = 5;
				BottomAppBar_fabCradleVerticalOffset = 6;
				BottomAppBar_hideOnScroll = 7;
				BottomNavigationView = new int[12]
				{
					2130903101,
					2130903287,
					2130903378,
					2130903381,
					2130903383,
					2130903384,
					2130903387,
					2130903399,
					2130903400,
					2130903401,
					2130903403,
					2130903513
				};
				BottomNavigationView_backgroundTint = 0;
				BottomNavigationView_elevation = 1;
				BottomNavigationView_itemBackground = 2;
				BottomNavigationView_itemHorizontalTranslationEnabled = 3;
				BottomNavigationView_itemIconSize = 4;
				BottomNavigationView_itemIconTint = 5;
				BottomNavigationView_itemRippleColor = 6;
				BottomNavigationView_itemTextAppearanceActive = 7;
				BottomNavigationView_itemTextAppearanceInactive = 8;
				BottomNavigationView_itemTextColor = 9;
				BottomNavigationView_labelVisibilityMode = 10;
				BottomNavigationView_menu = 11;
				BottomSheetBehavior_Layout = new int[11]
				{
					16843840,
					2130903101,
					2130903111,
					2130903112,
					2130903113,
					2130903114,
					2130903116,
					2130903117,
					2130903118,
					2130903563,
					2130903566
				};
				BottomSheetBehavior_Layout_android_elevation = 0;
				BottomSheetBehavior_Layout_backgroundTint = 1;
				BottomSheetBehavior_Layout_behavior_expandedOffset = 2;
				BottomSheetBehavior_Layout_behavior_fitToContents = 3;
				BottomSheetBehavior_Layout_behavior_halfExpandedRatio = 4;
				BottomSheetBehavior_Layout_behavior_hideable = 5;
				BottomSheetBehavior_Layout_behavior_peekHeight = 6;
				BottomSheetBehavior_Layout_behavior_saveFlags = 7;
				BottomSheetBehavior_Layout_behavior_skipCollapsed = 8;
				BottomSheetBehavior_Layout_shapeAppearance = 9;
				BottomSheetBehavior_Layout_shapeAppearanceOverlay = 10;
				ButtonBarLayout = new int[1]
				{
					2130903079
				};
				ButtonBarLayout_allowStacking = 0;
				CardView = new int[13]
				{
					16843071,
					16843072,
					2130903149,
					2130903150,
					2130903151,
					2130903153,
					2130903154,
					2130903155,
					2130903233,
					2130903234,
					2130903235,
					2130903236,
					2130903237
				};
				CardView_android_minHeight = 1;
				CardView_android_minWidth = 0;
				CardView_cardBackgroundColor = 2;
				CardView_cardCornerRadius = 3;
				CardView_cardElevation = 4;
				CardView_cardMaxElevation = 5;
				CardView_cardPreventCornerOverlap = 6;
				CardView_cardUseCompatPadding = 7;
				CardView_contentPadding = 8;
				CardView_contentPaddingBottom = 9;
				CardView_contentPaddingLeft = 10;
				CardView_contentPaddingRight = 11;
				CardView_contentPaddingTop = 12;
				Chip = new int[40]
				{
					16842804,
					16842904,
					16842923,
					16843039,
					16843087,
					16843237,
					2130903161,
					2130903162,
					2130903164,
					2130903166,
					2130903167,
					2130903168,
					2130903170,
					2130903171,
					2130903172,
					2130903173,
					2130903174,
					2130903175,
					2130903176,
					2130903181,
					2130903182,
					2130903183,
					2130903185,
					2130903187,
					2130903188,
					2130903189,
					2130903190,
					2130903191,
					2130903192,
					2130903193,
					2130903299,
					2130903351,
					2130903362,
					2130903366,
					2130903552,
					2130903563,
					2130903566,
					2130903570,
					2130903664,
					2130903667
				};
				ChipGroup = new int[6]
				{
					2130903160,
					2130903177,
					2130903178,
					2130903179,
					2130903575,
					2130903576
				};
				ChipGroup_checkedChip = 0;
				ChipGroup_chipSpacing = 1;
				ChipGroup_chipSpacingHorizontal = 2;
				ChipGroup_chipSpacingVertical = 3;
				ChipGroup_singleLine = 4;
				ChipGroup_singleSelection = 5;
				Chip_android_checkable = 5;
				Chip_android_ellipsize = 2;
				Chip_android_maxWidth = 3;
				Chip_android_text = 4;
				Chip_android_textAppearance = 0;
				Chip_android_textColor = 1;
				Chip_checkedIcon = 6;
				Chip_checkedIconEnabled = 7;
				Chip_checkedIconVisible = 8;
				Chip_chipBackgroundColor = 9;
				Chip_chipCornerRadius = 10;
				Chip_chipEndPadding = 11;
				Chip_chipIcon = 12;
				Chip_chipIconEnabled = 13;
				Chip_chipIconSize = 14;
				Chip_chipIconTint = 15;
				Chip_chipIconVisible = 16;
				Chip_chipMinHeight = 17;
				Chip_chipMinTouchTargetSize = 18;
				Chip_chipStartPadding = 19;
				Chip_chipStrokeColor = 20;
				Chip_chipStrokeWidth = 21;
				Chip_chipSurfaceColor = 22;
				Chip_closeIcon = 23;
				Chip_closeIconEnabled = 24;
				Chip_closeIconEndPadding = 25;
				Chip_closeIconSize = 26;
				Chip_closeIconStartPadding = 27;
				Chip_closeIconTint = 28;
				Chip_closeIconVisible = 29;
				Chip_ensureMinTouchTargetSize = 30;
				Chip_hideMotionSpec = 31;
				Chip_iconEndPadding = 32;
				Chip_iconStartPadding = 33;
				Chip_rippleColor = 34;
				Chip_shapeAppearance = 35;
				Chip_shapeAppearanceOverlay = 36;
				Chip_showMotionSpec = 37;
				Chip_textEndPadding = 38;
				Chip_textStartPadding = 39;
				CollapsingToolbarLayout = new int[16]
				{
					2130903197,
					2130903198,
					2130903238,
					2130903308,
					2130903309,
					2130903310,
					2130903311,
					2130903312,
					2130903313,
					2130903314,
					2130903554,
					2130903556,
					2130903599,
					2130903679,
					2130903680,
					2130903690
				};
				CollapsingToolbarLayout_collapsedTitleGravity = 0;
				CollapsingToolbarLayout_collapsedTitleTextAppearance = 1;
				CollapsingToolbarLayout_contentScrim = 2;
				CollapsingToolbarLayout_expandedTitleGravity = 3;
				CollapsingToolbarLayout_expandedTitleMargin = 4;
				CollapsingToolbarLayout_expandedTitleMarginBottom = 5;
				CollapsingToolbarLayout_expandedTitleMarginEnd = 6;
				CollapsingToolbarLayout_expandedTitleMarginStart = 7;
				CollapsingToolbarLayout_expandedTitleMarginTop = 8;
				CollapsingToolbarLayout_expandedTitleTextAppearance = 9;
				CollapsingToolbarLayout_Layout = new int[2]
				{
					2130903410,
					2130903411
				};
				CollapsingToolbarLayout_Layout_layout_collapseMode = 0;
				CollapsingToolbarLayout_Layout_layout_collapseParallaxMultiplier = 1;
				CollapsingToolbarLayout_scrimAnimationDuration = 10;
				CollapsingToolbarLayout_scrimVisibleHeightTrigger = 11;
				CollapsingToolbarLayout_statusBarScrim = 12;
				CollapsingToolbarLayout_title = 13;
				CollapsingToolbarLayout_titleEnabled = 14;
				CollapsingToolbarLayout_toolbarId = 15;
				ColorStateListItem = new int[3]
				{
					16843173,
					16843551,
					2130903080
				};
				ColorStateListItem_alpha = 2;
				ColorStateListItem_android_alpha = 1;
				ColorStateListItem_android_color = 0;
				CompoundButton = new int[4]
				{
					16843015,
					2130903140,
					2130903147,
					2130903148
				};
				CompoundButton_android_button = 0;
				CompoundButton_buttonCompat = 1;
				CompoundButton_buttonTint = 2;
				CompoundButton_buttonTintMode = 3;
				ConstraintLayout_Layout = new int[60]
				{
					16842948,
					16843039,
					16843040,
					16843071,
					16843072,
					2130903107,
					2130903108,
					2130903157,
					2130903223,
					2130903224,
					2130903412,
					2130903413,
					2130903414,
					2130903415,
					2130903416,
					2130903417,
					2130903418,
					2130903419,
					2130903420,
					2130903421,
					2130903422,
					2130903423,
					2130903424,
					2130903425,
					2130903426,
					2130903427,
					2130903428,
					2130903429,
					2130903430,
					2130903431,
					2130903432,
					2130903433,
					2130903434,
					2130903435,
					2130903436,
					2130903437,
					2130903438,
					2130903439,
					2130903440,
					2130903441,
					2130903442,
					2130903443,
					2130903444,
					2130903445,
					2130903446,
					2130903447,
					2130903448,
					2130903449,
					2130903450,
					2130903451,
					2130903452,
					2130903454,
					2130903455,
					2130903456,
					2130903457,
					2130903458,
					2130903459,
					2130903460,
					2130903461,
					2130903464
				};
				ConstraintLayout_Layout_android_maxHeight = 2;
				ConstraintLayout_Layout_android_maxWidth = 1;
				ConstraintLayout_Layout_android_minHeight = 4;
				ConstraintLayout_Layout_android_minWidth = 3;
				ConstraintLayout_Layout_android_orientation = 0;
				ConstraintLayout_Layout_barrierAllowsGoneWidgets = 5;
				ConstraintLayout_Layout_barrierDirection = 6;
				ConstraintLayout_Layout_chainUseRtl = 7;
				ConstraintLayout_Layout_constraintSet = 8;
				ConstraintLayout_Layout_constraint_referenced_ids = 9;
				ConstraintLayout_Layout_layout_constrainedHeight = 10;
				ConstraintLayout_Layout_layout_constrainedWidth = 11;
				ConstraintLayout_Layout_layout_constraintBaseline_creator = 12;
				ConstraintLayout_Layout_layout_constraintBaseline_toBaselineOf = 13;
				ConstraintLayout_Layout_layout_constraintBottom_creator = 14;
				ConstraintLayout_Layout_layout_constraintBottom_toBottomOf = 15;
				ConstraintLayout_Layout_layout_constraintBottom_toTopOf = 16;
				ConstraintLayout_Layout_layout_constraintCircle = 17;
				ConstraintLayout_Layout_layout_constraintCircleAngle = 18;
				ConstraintLayout_Layout_layout_constraintCircleRadius = 19;
				ConstraintLayout_Layout_layout_constraintDimensionRatio = 20;
				ConstraintLayout_Layout_layout_constraintEnd_toEndOf = 21;
				ConstraintLayout_Layout_layout_constraintEnd_toStartOf = 22;
				ConstraintLayout_Layout_layout_constraintGuide_begin = 23;
				ConstraintLayout_Layout_layout_constraintGuide_end = 24;
				ConstraintLayout_Layout_layout_constraintGuide_percent = 25;
				ConstraintLayout_Layout_layout_constraintHeight_default = 26;
				ConstraintLayout_Layout_layout_constraintHeight_max = 27;
				ConstraintLayout_Layout_layout_constraintHeight_min = 28;
				ConstraintLayout_Layout_layout_constraintHeight_percent = 29;
				ConstraintLayout_Layout_layout_constraintHorizontal_bias = 30;
				ConstraintLayout_Layout_layout_constraintHorizontal_chainStyle = 31;
				ConstraintLayout_Layout_layout_constraintHorizontal_weight = 32;
				ConstraintLayout_Layout_layout_constraintLeft_creator = 33;
				ConstraintLayout_Layout_layout_constraintLeft_toLeftOf = 34;
				ConstraintLayout_Layout_layout_constraintLeft_toRightOf = 35;
				ConstraintLayout_Layout_layout_constraintRight_creator = 36;
				ConstraintLayout_Layout_layout_constraintRight_toLeftOf = 37;
				ConstraintLayout_Layout_layout_constraintRight_toRightOf = 38;
				ConstraintLayout_Layout_layout_constraintStart_toEndOf = 39;
				ConstraintLayout_Layout_layout_constraintStart_toStartOf = 40;
				ConstraintLayout_Layout_layout_constraintTop_creator = 41;
				ConstraintLayout_Layout_layout_constraintTop_toBottomOf = 42;
				ConstraintLayout_Layout_layout_constraintTop_toTopOf = 43;
				ConstraintLayout_Layout_layout_constraintVertical_bias = 44;
				ConstraintLayout_Layout_layout_constraintVertical_chainStyle = 45;
				ConstraintLayout_Layout_layout_constraintVertical_weight = 46;
				ConstraintLayout_Layout_layout_constraintWidth_default = 47;
				ConstraintLayout_Layout_layout_constraintWidth_max = 48;
				ConstraintLayout_Layout_layout_constraintWidth_min = 49;
				ConstraintLayout_Layout_layout_constraintWidth_percent = 50;
				ConstraintLayout_Layout_layout_editor_absoluteX = 51;
				ConstraintLayout_Layout_layout_editor_absoluteY = 52;
				ConstraintLayout_Layout_layout_goneMarginBottom = 53;
				ConstraintLayout_Layout_layout_goneMarginEnd = 54;
				ConstraintLayout_Layout_layout_goneMarginLeft = 55;
				ConstraintLayout_Layout_layout_goneMarginRight = 56;
				ConstraintLayout_Layout_layout_goneMarginStart = 57;
				ConstraintLayout_Layout_layout_goneMarginTop = 58;
				ConstraintLayout_Layout_layout_optimizationLevel = 59;
				ConstraintLayout_placeholder = new int[2]
				{
					2130903225,
					2130903290
				};
				ConstraintLayout_placeholder_content = 0;
				ConstraintLayout_placeholder_emptyVisibility = 1;
				ConstraintSet = new int[80]
				{
					16842948,
					16842960,
					16842972,
					16842996,
					16842997,
					16842999,
					16843000,
					16843001,
					16843002,
					16843039,
					16843040,
					16843071,
					16843072,
					16843551,
					16843552,
					16843553,
					16843554,
					16843555,
					16843556,
					16843557,
					16843558,
					16843559,
					16843560,
					16843701,
					16843702,
					16843770,
					16843840,
					2130903107,
					2130903108,
					2130903157,
					2130903224,
					2130903412,
					2130903413,
					2130903414,
					2130903415,
					2130903416,
					2130903417,
					2130903418,
					2130903419,
					2130903420,
					2130903421,
					2130903422,
					2130903423,
					2130903424,
					2130903425,
					2130903426,
					2130903427,
					2130903428,
					2130903429,
					2130903430,
					2130903431,
					2130903432,
					2130903433,
					2130903434,
					2130903435,
					2130903436,
					2130903437,
					2130903438,
					2130903439,
					2130903440,
					2130903441,
					2130903442,
					2130903443,
					2130903444,
					2130903445,
					2130903446,
					2130903447,
					2130903448,
					2130903449,
					2130903450,
					2130903451,
					2130903452,
					2130903454,
					2130903455,
					2130903456,
					2130903457,
					2130903458,
					2130903459,
					2130903460,
					2130903461
				};
				ConstraintSet_android_alpha = 13;
				ConstraintSet_android_elevation = 26;
				ConstraintSet_android_id = 1;
				ConstraintSet_android_layout_height = 4;
				ConstraintSet_android_layout_marginBottom = 8;
				ConstraintSet_android_layout_marginEnd = 24;
				ConstraintSet_android_layout_marginLeft = 5;
				ConstraintSet_android_layout_marginRight = 7;
				ConstraintSet_android_layout_marginStart = 23;
				ConstraintSet_android_layout_marginTop = 6;
				ConstraintSet_android_layout_width = 3;
				ConstraintSet_android_maxHeight = 10;
				ConstraintSet_android_maxWidth = 9;
				ConstraintSet_android_minHeight = 12;
				ConstraintSet_android_minWidth = 11;
				ConstraintSet_android_orientation = 0;
				ConstraintSet_android_rotation = 20;
				ConstraintSet_android_rotationX = 21;
				ConstraintSet_android_rotationY = 22;
				ConstraintSet_android_scaleX = 18;
				ConstraintSet_android_scaleY = 19;
				ConstraintSet_android_transformPivotX = 14;
				ConstraintSet_android_transformPivotY = 15;
				ConstraintSet_android_translationX = 16;
				ConstraintSet_android_translationY = 17;
				ConstraintSet_android_translationZ = 25;
				ConstraintSet_android_visibility = 2;
				ConstraintSet_barrierAllowsGoneWidgets = 27;
				ConstraintSet_barrierDirection = 28;
				ConstraintSet_chainUseRtl = 29;
				ConstraintSet_constraint_referenced_ids = 30;
				ConstraintSet_layout_constrainedHeight = 31;
				ConstraintSet_layout_constrainedWidth = 32;
				ConstraintSet_layout_constraintBaseline_creator = 33;
				ConstraintSet_layout_constraintBaseline_toBaselineOf = 34;
				ConstraintSet_layout_constraintBottom_creator = 35;
				ConstraintSet_layout_constraintBottom_toBottomOf = 36;
				ConstraintSet_layout_constraintBottom_toTopOf = 37;
				ConstraintSet_layout_constraintCircle = 38;
				ConstraintSet_layout_constraintCircleAngle = 39;
				ConstraintSet_layout_constraintCircleRadius = 40;
				ConstraintSet_layout_constraintDimensionRatio = 41;
				ConstraintSet_layout_constraintEnd_toEndOf = 42;
				ConstraintSet_layout_constraintEnd_toStartOf = 43;
				ConstraintSet_layout_constraintGuide_begin = 44;
				ConstraintSet_layout_constraintGuide_end = 45;
				ConstraintSet_layout_constraintGuide_percent = 46;
				ConstraintSet_layout_constraintHeight_default = 47;
				ConstraintSet_layout_constraintHeight_max = 48;
				ConstraintSet_layout_constraintHeight_min = 49;
				ConstraintSet_layout_constraintHeight_percent = 50;
				ConstraintSet_layout_constraintHorizontal_bias = 51;
				ConstraintSet_layout_constraintHorizontal_chainStyle = 52;
				ConstraintSet_layout_constraintHorizontal_weight = 53;
				ConstraintSet_layout_constraintLeft_creator = 54;
				ConstraintSet_layout_constraintLeft_toLeftOf = 55;
				ConstraintSet_layout_constraintLeft_toRightOf = 56;
				ConstraintSet_layout_constraintRight_creator = 57;
				ConstraintSet_layout_constraintRight_toLeftOf = 58;
				ConstraintSet_layout_constraintRight_toRightOf = 59;
				ConstraintSet_layout_constraintStart_toEndOf = 60;
				ConstraintSet_layout_constraintStart_toStartOf = 61;
				ConstraintSet_layout_constraintTop_creator = 62;
				ConstraintSet_layout_constraintTop_toBottomOf = 63;
				ConstraintSet_layout_constraintTop_toTopOf = 64;
				ConstraintSet_layout_constraintVertical_bias = 65;
				ConstraintSet_layout_constraintVertical_chainStyle = 66;
				ConstraintSet_layout_constraintVertical_weight = 67;
				ConstraintSet_layout_constraintWidth_default = 68;
				ConstraintSet_layout_constraintWidth_max = 69;
				ConstraintSet_layout_constraintWidth_min = 70;
				ConstraintSet_layout_constraintWidth_percent = 71;
				ConstraintSet_layout_editor_absoluteX = 72;
				ConstraintSet_layout_editor_absoluteY = 73;
				ConstraintSet_layout_goneMarginBottom = 74;
				ConstraintSet_layout_goneMarginEnd = 75;
				ConstraintSet_layout_goneMarginLeft = 76;
				ConstraintSet_layout_goneMarginRight = 77;
				ConstraintSet_layout_goneMarginStart = 78;
				ConstraintSet_layout_goneMarginTop = 79;
				CoordinatorLayout = new int[2]
				{
					2130903402,
					2130903597
				};
				CoordinatorLayout_keylines = 0;
				CoordinatorLayout_Layout = new int[7]
				{
					16842931,
					2130903407,
					2130903408,
					2130903409,
					2130903453,
					2130903462,
					2130903463
				};
				CoordinatorLayout_Layout_android_layout_gravity = 0;
				CoordinatorLayout_Layout_layout_anchor = 1;
				CoordinatorLayout_Layout_layout_anchorGravity = 2;
				CoordinatorLayout_Layout_layout_behavior = 3;
				CoordinatorLayout_Layout_layout_dodgeInsetEdges = 4;
				CoordinatorLayout_Layout_layout_insetEdge = 5;
				CoordinatorLayout_Layout_layout_keyline = 6;
				CoordinatorLayout_statusBarBackground = 1;
				DrawerArrowToggle = new int[8]
				{
					2130903084,
					2130903085,
					2130903106,
					2130903199,
					2130903276,
					2130903343,
					2130903580,
					2130903670
				};
				DrawerArrowToggle_arrowHeadLength = 0;
				DrawerArrowToggle_arrowShaftLength = 1;
				DrawerArrowToggle_barLength = 2;
				DrawerArrowToggle_color = 3;
				DrawerArrowToggle_drawableSize = 4;
				DrawerArrowToggle_gapBetweenBars = 5;
				DrawerArrowToggle_spinBars = 6;
				DrawerArrowToggle_thickness = 7;
				ExtendedFloatingActionButton = new int[5]
				{
					2130903287,
					2130903315,
					2130903351,
					2130903570,
					2130903573
				};
				ExtendedFloatingActionButton_Behavior_Layout = new int[2]
				{
					2130903109,
					2130903110
				};
				ExtendedFloatingActionButton_Behavior_Layout_behavior_autoHide = 0;
				ExtendedFloatingActionButton_Behavior_Layout_behavior_autoShrink = 1;
				ExtendedFloatingActionButton_elevation = 0;
				ExtendedFloatingActionButton_extendMotionSpec = 1;
				ExtendedFloatingActionButton_hideMotionSpec = 2;
				ExtendedFloatingActionButton_showMotionSpec = 3;
				ExtendedFloatingActionButton_shrinkMotionSpec = 4;
				FloatingActionButton = new int[16]
				{
					2130903101,
					2130903102,
					2130903119,
					2130903287,
					2130903299,
					2130903322,
					2130903323,
					2130903351,
					2130903360,
					2130903511,
					2130903540,
					2130903552,
					2130903563,
					2130903566,
					2130903570,
					2130903700
				};
				FloatingActionButton_backgroundTint = 0;
				FloatingActionButton_backgroundTintMode = 1;
				FloatingActionButton_Behavior_Layout = new int[1]
				{
					2130903109
				};
				FloatingActionButton_Behavior_Layout_behavior_autoHide = 0;
				FloatingActionButton_borderWidth = 2;
				FloatingActionButton_elevation = 3;
				FloatingActionButton_ensureMinTouchTargetSize = 4;
				FloatingActionButton_fabCustomSize = 5;
				FloatingActionButton_fabSize = 6;
				FloatingActionButton_hideMotionSpec = 7;
				FloatingActionButton_hoveredFocusedTranslationZ = 8;
				FloatingActionButton_maxImageSize = 9;
				FloatingActionButton_pressedTranslationZ = 10;
				FloatingActionButton_rippleColor = 11;
				FloatingActionButton_shapeAppearance = 12;
				FloatingActionButton_shapeAppearanceOverlay = 13;
				FloatingActionButton_showMotionSpec = 14;
				FloatingActionButton_useCompatPadding = 15;
				FlowLayout = new int[2]
				{
					2130903395,
					2130903470
				};
				FlowLayout_itemSpacing = 0;
				FlowLayout_lineSpacing = 1;
				FontFamily = new int[6]
				{
					2130903333,
					2130903334,
					2130903335,
					2130903336,
					2130903337,
					2130903338
				};
				FontFamilyFont = new int[10]
				{
					16844082,
					16844083,
					16844095,
					16844143,
					16844144,
					2130903331,
					2130903339,
					2130903340,
					2130903341,
					2130903699
				};
				FontFamilyFont_android_font = 0;
				FontFamilyFont_android_fontStyle = 2;
				FontFamilyFont_android_fontVariationSettings = 4;
				FontFamilyFont_android_fontWeight = 1;
				FontFamilyFont_android_ttcIndex = 3;
				FontFamilyFont_font = 5;
				FontFamilyFont_fontStyle = 6;
				FontFamilyFont_fontVariationSettings = 7;
				FontFamilyFont_fontWeight = 8;
				FontFamilyFont_ttcIndex = 9;
				FontFamily_fontProviderAuthority = 0;
				FontFamily_fontProviderCerts = 1;
				FontFamily_fontProviderFetchStrategy = 2;
				FontFamily_fontProviderFetchTimeout = 3;
				FontFamily_fontProviderPackage = 4;
				FontFamily_fontProviderQuery = 5;
				ForegroundLinearLayout = new int[3]
				{
					16843017,
					16843264,
					2130903342
				};
				ForegroundLinearLayout_android_foreground = 0;
				ForegroundLinearLayout_android_foregroundGravity = 1;
				ForegroundLinearLayout_foregroundInsidePadding = 2;
				Fragment = new int[3]
				{
					16842755,
					16842960,
					16842961
				};
				FragmentContainerView = new int[2]
				{
					16842755,
					16842961
				};
				FragmentContainerView_android_name = 0;
				FragmentContainerView_android_tag = 1;
				Fragment_android_id = 1;
				Fragment_android_name = 0;
				Fragment_android_tag = 2;
				GradientColor = new int[12]
				{
					16843165,
					16843166,
					16843169,
					16843170,
					16843171,
					16843172,
					16843265,
					16843275,
					16844048,
					16844049,
					16844050,
					16844051
				};
				GradientColorItem = new int[2]
				{
					16843173,
					16844052
				};
				GradientColorItem_android_color = 0;
				GradientColorItem_android_offset = 1;
				GradientColor_android_centerColor = 7;
				GradientColor_android_centerX = 3;
				GradientColor_android_centerY = 4;
				GradientColor_android_endColor = 1;
				GradientColor_android_endX = 10;
				GradientColor_android_endY = 11;
				GradientColor_android_gradientRadius = 5;
				GradientColor_android_startColor = 0;
				GradientColor_android_startX = 8;
				GradientColor_android_startY = 9;
				GradientColor_android_tileMode = 6;
				GradientColor_android_type = 2;
				LinearConstraintLayout = new int[1]
				{
					16842948
				};
				LinearConstraintLayout_android_orientation = 0;
				LinearLayoutCompat = new int[9]
				{
					16842927,
					16842948,
					16843046,
					16843047,
					16843048,
					2130903268,
					2130903270,
					2130903512,
					2130903569
				};
				LinearLayoutCompat_android_baselineAligned = 2;
				LinearLayoutCompat_android_baselineAlignedChildIndex = 3;
				LinearLayoutCompat_android_gravity = 0;
				LinearLayoutCompat_android_orientation = 1;
				LinearLayoutCompat_android_weightSum = 4;
				LinearLayoutCompat_divider = 5;
				LinearLayoutCompat_dividerPadding = 6;
				LinearLayoutCompat_Layout = new int[4]
				{
					16842931,
					16842996,
					16842997,
					16843137
				};
				LinearLayoutCompat_Layout_android_layout_gravity = 0;
				LinearLayoutCompat_Layout_android_layout_height = 2;
				LinearLayoutCompat_Layout_android_layout_weight = 3;
				LinearLayoutCompat_Layout_android_layout_width = 1;
				LinearLayoutCompat_measureWithLargestChild = 7;
				LinearLayoutCompat_showDividers = 8;
				ListPopupWindow = new int[2]
				{
					16843436,
					16843437
				};
				ListPopupWindow_android_dropDownHorizontalOffset = 0;
				ListPopupWindow_android_dropDownVerticalOffset = 1;
				LoadingImageView = new int[3]
				{
					2130903186,
					2130903370,
					2130903371
				};
				LoadingImageView_circleCrop = 0;
				LoadingImageView_imageAspectRatio = 1;
				LoadingImageView_imageAspectRatioAdjust = 2;
				MaterialAlertDialog = new int[4]
				{
					2130903094,
					2130903095,
					2130903096,
					2130903097
				};
				MaterialAlertDialogTheme = new int[5]
				{
					2130903488,
					2130903489,
					2130903490,
					2130903491,
					2130903492
				};
				MaterialAlertDialogTheme_materialAlertDialogBodyTextStyle = 0;
				MaterialAlertDialogTheme_materialAlertDialogTheme = 1;
				MaterialAlertDialogTheme_materialAlertDialogTitleIconStyle = 2;
				MaterialAlertDialogTheme_materialAlertDialogTitlePanelStyle = 3;
				MaterialAlertDialogTheme_materialAlertDialogTitleTextStyle = 4;
				MaterialAlertDialog_backgroundInsetBottom = 0;
				MaterialAlertDialog_backgroundInsetEnd = 1;
				MaterialAlertDialog_backgroundInsetStart = 2;
				MaterialAlertDialog_backgroundInsetTop = 3;
				MaterialButton = new int[20]
				{
					16843191,
					16843192,
					16843193,
					16843194,
					16843237,
					2130903101,
					2130903102,
					2130903246,
					2130903287,
					2130903361,
					2130903363,
					2130903364,
					2130903365,
					2130903367,
					2130903368,
					2130903552,
					2130903563,
					2130903566,
					2130903600,
					2130903601
				};
				MaterialButtonToggleGroup = new int[2]
				{
					2130903159,
					2130903576
				};
				MaterialButtonToggleGroup_checkedButton = 0;
				MaterialButtonToggleGroup_singleSelection = 1;
				MaterialButton_android_checkable = 4;
				MaterialButton_android_insetBottom = 3;
				MaterialButton_android_insetLeft = 0;
				MaterialButton_android_insetRight = 1;
				MaterialButton_android_insetTop = 2;
				MaterialButton_backgroundTint = 5;
				MaterialButton_backgroundTintMode = 6;
				MaterialButton_cornerRadius = 7;
				MaterialButton_elevation = 8;
				MaterialButton_icon = 9;
				MaterialButton_iconGravity = 10;
				MaterialButton_iconPadding = 11;
				MaterialButton_iconSize = 12;
				MaterialButton_iconTint = 13;
				MaterialButton_iconTintMode = 14;
				MaterialButton_rippleColor = 15;
				MaterialButton_shapeAppearance = 16;
				MaterialButton_shapeAppearanceOverlay = 17;
				MaterialButton_strokeColor = 18;
				MaterialButton_strokeWidth = 19;
				MaterialCalendar = new int[9]
				{
					16843277,
					2130903259,
					2130903260,
					2130903261,
					2130903262,
					2130903546,
					2130903714,
					2130903715,
					2130903716
				};
				MaterialCalendarItem = new int[10]
				{
					16843191,
					16843192,
					16843193,
					16843194,
					2130903379,
					2130903388,
					2130903389,
					2130903396,
					2130903397,
					2130903401
				};
				MaterialCalendarItem_android_insetBottom = 3;
				MaterialCalendarItem_android_insetLeft = 0;
				MaterialCalendarItem_android_insetRight = 1;
				MaterialCalendarItem_android_insetTop = 2;
				MaterialCalendarItem_itemFillColor = 4;
				MaterialCalendarItem_itemShapeAppearance = 5;
				MaterialCalendarItem_itemShapeAppearanceOverlay = 6;
				MaterialCalendarItem_itemStrokeColor = 7;
				MaterialCalendarItem_itemStrokeWidth = 8;
				MaterialCalendarItem_itemTextColor = 9;
				MaterialCalendar_android_windowFullscreen = 0;
				MaterialCalendar_dayInvalidStyle = 1;
				MaterialCalendar_daySelectedStyle = 2;
				MaterialCalendar_dayStyle = 3;
				MaterialCalendar_dayTodayStyle = 4;
				MaterialCalendar_rangeFillColor = 5;
				MaterialCalendar_yearSelectedStyle = 6;
				MaterialCalendar_yearStyle = 7;
				MaterialCalendar_yearTodayStyle = 8;
				MaterialCardView = new int[10]
				{
					16843237,
					2130903152,
					2130903161,
					2130903163,
					2130903552,
					2130903563,
					2130903566,
					2130903594,
					2130903600,
					2130903601
				};
				MaterialCardView_android_checkable = 0;
				MaterialCardView_cardForegroundColor = 1;
				MaterialCardView_checkedIcon = 2;
				MaterialCardView_checkedIconTint = 3;
				MaterialCardView_rippleColor = 4;
				MaterialCardView_shapeAppearance = 5;
				MaterialCardView_shapeAppearanceOverlay = 6;
				MaterialCardView_state_dragged = 7;
				MaterialCardView_strokeColor = 8;
				MaterialCardView_strokeWidth = 9;
				MaterialCheckBox = new int[2]
				{
					2130903147,
					2130903701
				};
				MaterialCheckBox_buttonTint = 0;
				MaterialCheckBox_useMaterialThemeColors = 1;
				MaterialRadioButton = new int[1]
				{
					2130903701
				};
				MaterialRadioButton_useMaterialThemeColors = 0;
				MaterialShape = new int[2]
				{
					2130903563,
					2130903566
				};
				MaterialShape_shapeAppearance = 0;
				MaterialShape_shapeAppearanceOverlay = 1;
				MaterialTextAppearance = new int[2]
				{
					16844159,
					2130903469
				};
				MaterialTextAppearance_android_lineHeight = 0;
				MaterialTextAppearance_lineHeight = 1;
				MaterialTextView = new int[3]
				{
					16842804,
					16844159,
					2130903469
				};
				MaterialTextView_android_lineHeight = 1;
				MaterialTextView_android_textAppearance = 0;
				MaterialTextView_lineHeight = 2;
				MenuGroup = new int[6]
				{
					16842766,
					16842960,
					16843156,
					16843230,
					16843231,
					16843232
				};
				MenuGroup_android_checkableBehavior = 5;
				MenuGroup_android_enabled = 0;
				MenuGroup_android_id = 1;
				MenuGroup_android_menuCategory = 3;
				MenuGroup_android_orderInCategory = 4;
				MenuGroup_android_visible = 2;
				MenuItem = new int[23]
				{
					16842754,
					16842766,
					16842960,
					16843014,
					16843156,
					16843230,
					16843231,
					16843233,
					16843234,
					16843235,
					16843236,
					16843237,
					16843375,
					2130903053,
					2130903071,
					2130903073,
					2130903081,
					2130903226,
					2130903367,
					2130903368,
					2130903521,
					2130903568,
					2130903695
				};
				MenuItem_actionLayout = 13;
				MenuItem_actionProviderClass = 14;
				MenuItem_actionViewClass = 15;
				MenuItem_alphabeticModifiers = 16;
				MenuItem_android_alphabeticShortcut = 9;
				MenuItem_android_checkable = 11;
				MenuItem_android_checked = 3;
				MenuItem_android_enabled = 1;
				MenuItem_android_icon = 0;
				MenuItem_android_id = 2;
				MenuItem_android_menuCategory = 5;
				MenuItem_android_numericShortcut = 10;
				MenuItem_android_onClick = 12;
				MenuItem_android_orderInCategory = 6;
				MenuItem_android_title = 7;
				MenuItem_android_titleCondensed = 8;
				MenuItem_android_visible = 4;
				MenuItem_contentDescription = 17;
				MenuItem_iconTint = 18;
				MenuItem_iconTintMode = 19;
				MenuItem_numericModifiers = 20;
				MenuItem_showAsAction = 21;
				MenuItem_tooltipText = 22;
				MenuView = new int[9]
				{
					16842926,
					16843052,
					16843053,
					16843054,
					16843055,
					16843056,
					16843057,
					2130903539,
					2130903602
				};
				MenuView_android_headerBackground = 4;
				MenuView_android_horizontalDivider = 2;
				MenuView_android_itemBackground = 5;
				MenuView_android_itemIconDisabledAlpha = 6;
				MenuView_android_itemTextAppearance = 1;
				MenuView_android_verticalDivider = 3;
				MenuView_android_windowAnimationStyle = 0;
				MenuView_preserveIconSpacing = 7;
				MenuView_subMenuArrow = 8;
				NavigationView = new int[21]
				{
					16842964,
					16842973,
					16843039,
					2130903287,
					2130903345,
					2130903378,
					2130903380,
					2130903382,
					2130903383,
					2130903384,
					2130903385,
					2130903388,
					2130903389,
					2130903390,
					2130903391,
					2130903392,
					2130903393,
					2130903394,
					2130903398,
					2130903401,
					2130903513
				};
				NavigationView_android_background = 0;
				NavigationView_android_fitsSystemWindows = 1;
				NavigationView_android_maxWidth = 2;
				NavigationView_elevation = 3;
				NavigationView_headerLayout = 4;
				NavigationView_itemBackground = 5;
				NavigationView_itemHorizontalPadding = 6;
				NavigationView_itemIconPadding = 7;
				NavigationView_itemIconSize = 8;
				NavigationView_itemIconTint = 9;
				NavigationView_itemMaxLines = 10;
				NavigationView_itemShapeAppearance = 11;
				NavigationView_itemShapeAppearanceOverlay = 12;
				NavigationView_itemShapeFillColor = 13;
				NavigationView_itemShapeInsetBottom = 14;
				NavigationView_itemShapeInsetEnd = 15;
				NavigationView_itemShapeInsetStart = 16;
				NavigationView_itemShapeInsetTop = 17;
				NavigationView_itemTextAppearance = 18;
				NavigationView_itemTextColor = 19;
				NavigationView_menu = 20;
				PopupWindow = new int[3]
				{
					16843126,
					16843465,
					2130903522
				};
				PopupWindowBackgroundState = new int[1]
				{
					2130903591
				};
				PopupWindowBackgroundState_state_above_anchor = 0;
				PopupWindow_android_popupAnimationStyle = 1;
				PopupWindow_android_popupBackground = 0;
				PopupWindow_overlapAnchor = 2;
				RecycleListView = new int[2]
				{
					2130903523,
					2130903526
				};
				RecycleListView_paddingBottomNoButtons = 0;
				RecycleListView_paddingTopNoTitle = 1;
				RecyclerView = new int[12]
				{
					16842948,
					16842987,
					16842993,
					2130903324,
					2130903325,
					2130903326,
					2130903327,
					2130903328,
					2130903406,
					2130903551,
					2130903579,
					2130903585
				};
				RecyclerView_android_clipToPadding = 1;
				RecyclerView_android_descendantFocusability = 2;
				RecyclerView_android_orientation = 0;
				RecyclerView_fastScrollEnabled = 3;
				RecyclerView_fastScrollHorizontalThumbDrawable = 4;
				RecyclerView_fastScrollHorizontalTrackDrawable = 5;
				RecyclerView_fastScrollVerticalThumbDrawable = 6;
				RecyclerView_fastScrollVerticalTrackDrawable = 7;
				RecyclerView_layoutManager = 8;
				RecyclerView_reverseLayout = 9;
				RecyclerView_spanCount = 10;
				RecyclerView_stackFromEnd = 11;
				ScrimInsetsFrameLayout = new int[1]
				{
					2130903375
				};
				ScrimInsetsFrameLayout_insetForeground = 0;
				ScrollingViewBehavior_Layout = new int[1]
				{
					2130903115
				};
				ScrollingViewBehavior_Layout_behavior_overlapTop = 0;
				SearchView = new int[17]
				{
					16842970,
					16843039,
					16843296,
					16843364,
					2130903187,
					2130903222,
					2130903263,
					2130903344,
					2130903369,
					2130903405,
					2130903543,
					2130903544,
					2130903557,
					2130903558,
					2130903603,
					2130903608,
					2130903703
				};
				SearchView_android_focusable = 0;
				SearchView_android_imeOptions = 3;
				SearchView_android_inputType = 2;
				SearchView_android_maxWidth = 1;
				SearchView_closeIcon = 4;
				SearchView_commitIcon = 5;
				SearchView_defaultQueryHint = 6;
				SearchView_goIcon = 7;
				SearchView_iconifiedByDefault = 8;
				SearchView_layout = 9;
				SearchView_queryBackground = 10;
				SearchView_queryHint = 11;
				SearchView_searchHintIcon = 12;
				SearchView_searchIcon = 13;
				SearchView_submitBackground = 14;
				SearchView_suggestionRowLayout = 15;
				SearchView_voiceIcon = 16;
				ShapeAppearance = new int[10]
				{
					2130903241,
					2130903242,
					2130903243,
					2130903244,
					2130903245,
					2130903247,
					2130903248,
					2130903249,
					2130903250,
					2130903251
				};
				ShapeAppearance_cornerFamily = 0;
				ShapeAppearance_cornerFamilyBottomLeft = 1;
				ShapeAppearance_cornerFamilyBottomRight = 2;
				ShapeAppearance_cornerFamilyTopLeft = 3;
				ShapeAppearance_cornerFamilyTopRight = 4;
				ShapeAppearance_cornerSize = 5;
				ShapeAppearance_cornerSizeBottomLeft = 6;
				ShapeAppearance_cornerSizeBottomRight = 7;
				ShapeAppearance_cornerSizeTopLeft = 8;
				ShapeAppearance_cornerSizeTopRight = 9;
				SignInButton = new int[3]
				{
					2130903144,
					2130903217,
					2130903553
				};
				SignInButton_buttonSize = 0;
				SignInButton_colorScheme = 1;
				SignInButton_scopeUris = 2;
				Snackbar = new int[2]
				{
					2130903577,
					2130903578
				};
				SnackbarLayout = new int[6]
				{
					16843039,
					2130903072,
					2130903082,
					2130903098,
					2130903287,
					2130903508
				};
				SnackbarLayout_actionTextColorAlpha = 1;
				SnackbarLayout_android_maxWidth = 0;
				SnackbarLayout_animationMode = 2;
				SnackbarLayout_backgroundOverlayColorAlpha = 3;
				SnackbarLayout_elevation = 4;
				SnackbarLayout_maxActionInlineWidth = 5;
				Snackbar_snackbarButtonStyle = 0;
				Snackbar_snackbarStyle = 1;
				Spinner = new int[5]
				{
					16842930,
					16843126,
					16843131,
					16843362,
					2130903537
				};
				Spinner_android_dropDownWidth = 3;
				Spinner_android_entries = 0;
				Spinner_android_popupBackground = 1;
				Spinner_android_prompt = 2;
				Spinner_popupTheme = 4;
				StateListDrawable = new int[6]
				{
					16843036,
					16843156,
					16843157,
					16843158,
					16843532,
					16843533
				};
				StateListDrawableItem = new int[1]
				{
					16843161
				};
				StateListDrawableItem_android_drawable = 0;
				StateListDrawable_android_constantSize = 3;
				StateListDrawable_android_dither = 0;
				StateListDrawable_android_enterFadeDuration = 4;
				StateListDrawable_android_exitFadeDuration = 5;
				StateListDrawable_android_variablePadding = 2;
				StateListDrawable_android_visible = 1;
				SwitchCompat = new int[14]
				{
					16843044,
					16843045,
					16843074,
					2130903571,
					2130903583,
					2130903609,
					2130903610,
					2130903612,
					2130903671,
					2130903672,
					2130903673,
					2130903696,
					2130903697,
					2130903698
				};
				SwitchCompat_android_textOff = 1;
				SwitchCompat_android_textOn = 0;
				SwitchCompat_android_thumb = 2;
				SwitchCompat_showText = 3;
				SwitchCompat_splitTrack = 4;
				SwitchCompat_switchMinWidth = 5;
				SwitchCompat_switchPadding = 6;
				SwitchCompat_switchTextAppearance = 7;
				SwitchCompat_thumbTextPadding = 8;
				SwitchCompat_thumbTint = 9;
				SwitchCompat_thumbTintMode = 10;
				SwitchCompat_track = 11;
				SwitchCompat_trackTint = 12;
				SwitchCompat_trackTintMode = 13;
				SwitchMaterial = new int[1]
				{
					2130903701
				};
				SwitchMaterial_useMaterialThemeColors = 0;
				TabItem = new int[3]
				{
					16842754,
					16842994,
					16843087
				};
				TabItem_android_icon = 0;
				TabItem_android_layout = 1;
				TabItem_android_text = 2;
				TabLayout = new int[25]
				{
					2130903613,
					2130903614,
					2130903615,
					2130903616,
					2130903617,
					2130903618,
					2130903619,
					2130903620,
					2130903621,
					2130903622,
					2130903623,
					2130903624,
					2130903625,
					2130903626,
					2130903627,
					2130903628,
					2130903629,
					2130903630,
					2130903631,
					2130903632,
					2130903633,
					2130903634,
					2130903636,
					2130903637,
					2130903638
				};
				TabLayout_tabBackground = 0;
				TabLayout_tabContentStart = 1;
				TabLayout_tabGravity = 2;
				TabLayout_tabIconTint = 3;
				TabLayout_tabIconTintMode = 4;
				TabLayout_tabIndicator = 5;
				TabLayout_tabIndicatorAnimationDuration = 6;
				TabLayout_tabIndicatorColor = 7;
				TabLayout_tabIndicatorFullWidth = 8;
				TabLayout_tabIndicatorGravity = 9;
				TabLayout_tabIndicatorHeight = 10;
				TabLayout_tabInlineLabel = 11;
				TabLayout_tabMaxWidth = 12;
				TabLayout_tabMinWidth = 13;
				TabLayout_tabMode = 14;
				TabLayout_tabPadding = 15;
				TabLayout_tabPaddingBottom = 16;
				TabLayout_tabPaddingEnd = 17;
				TabLayout_tabPaddingStart = 18;
				TabLayout_tabPaddingTop = 19;
				TabLayout_tabRippleColor = 20;
				TabLayout_tabSelectedTextColor = 21;
				TabLayout_tabTextAppearance = 22;
				TabLayout_tabTextColor = 23;
				TabLayout_tabUnboundedRipple = 24;
				TextAppearance = new int[16]
				{
					16842901,
					16842902,
					16842903,
					16842904,
					16842906,
					16842907,
					16843105,
					16843106,
					16843107,
					16843108,
					16843692,
					16844165,
					2130903332,
					2130903340,
					2130903639,
					2130903666
				};
				TextAppearance_android_fontFamily = 10;
				TextAppearance_android_shadowColor = 6;
				TextAppearance_android_shadowDx = 7;
				TextAppearance_android_shadowDy = 8;
				TextAppearance_android_shadowRadius = 9;
				TextAppearance_android_textColor = 3;
				TextAppearance_android_textColorHint = 4;
				TextAppearance_android_textColorLink = 5;
				TextAppearance_android_textFontWeight = 11;
				TextAppearance_android_textSize = 0;
				TextAppearance_android_textStyle = 2;
				TextAppearance_android_typeface = 1;
				TextAppearance_fontFamily = 12;
				TextAppearance_fontVariationSettings = 13;
				TextAppearance_textAllCaps = 14;
				TextAppearance_textLocale = 15;
				TextInputLayout = new int[50]
				{
					16842906,
					16843088,
					2130903125,
					2130903126,
					2130903127,
					2130903128,
					2130903129,
					2130903130,
					2130903131,
					2130903132,
					2130903133,
					2130903134,
					2130903252,
					2130903253,
					2130903254,
					2130903255,
					2130903256,
					2130903257,
					2130903291,
					2130903292,
					2130903293,
					2130903294,
					2130903295,
					2130903296,
					2130903300,
					2130903301,
					2130903302,
					2130903303,
					2130903304,
					2130903305,
					2130903347,
					2130903348,
					2130903349,
					2130903350,
					2130903354,
					2130903355,
					2130903356,
					2130903357,
					2130903530,
					2130903531,
					2130903532,
					2130903533,
					2130903534,
					2130903563,
					2130903566,
					2130903586,
					2130903587,
					2130903588,
					2130903589,
					2130903590
				};
				TextInputLayout_android_hint = 1;
				TextInputLayout_android_textColorHint = 0;
				TextInputLayout_boxBackgroundColor = 2;
				TextInputLayout_boxBackgroundMode = 3;
				TextInputLayout_boxCollapsedPaddingTop = 4;
				TextInputLayout_boxCornerRadiusBottomEnd = 5;
				TextInputLayout_boxCornerRadiusBottomStart = 6;
				TextInputLayout_boxCornerRadiusTopEnd = 7;
				TextInputLayout_boxCornerRadiusTopStart = 8;
				TextInputLayout_boxStrokeColor = 9;
				TextInputLayout_boxStrokeWidth = 10;
				TextInputLayout_boxStrokeWidthFocused = 11;
				TextInputLayout_counterEnabled = 12;
				TextInputLayout_counterMaxLength = 13;
				TextInputLayout_counterOverflowTextAppearance = 14;
				TextInputLayout_counterOverflowTextColor = 15;
				TextInputLayout_counterTextAppearance = 16;
				TextInputLayout_counterTextColor = 17;
				TextInputLayout_endIconCheckable = 18;
				TextInputLayout_endIconContentDescription = 19;
				TextInputLayout_endIconDrawable = 20;
				TextInputLayout_endIconMode = 21;
				TextInputLayout_endIconTint = 22;
				TextInputLayout_endIconTintMode = 23;
				TextInputLayout_errorEnabled = 24;
				TextInputLayout_errorIconDrawable = 25;
				TextInputLayout_errorIconTint = 26;
				TextInputLayout_errorIconTintMode = 27;
				TextInputLayout_errorTextAppearance = 28;
				TextInputLayout_errorTextColor = 29;
				TextInputLayout_helperText = 30;
				TextInputLayout_helperTextEnabled = 31;
				TextInputLayout_helperTextTextAppearance = 32;
				TextInputLayout_helperTextTextColor = 33;
				TextInputLayout_hintAnimationEnabled = 34;
				TextInputLayout_hintEnabled = 35;
				TextInputLayout_hintTextAppearance = 36;
				TextInputLayout_hintTextColor = 37;
				TextInputLayout_passwordToggleContentDescription = 38;
				TextInputLayout_passwordToggleDrawable = 39;
				TextInputLayout_passwordToggleEnabled = 40;
				TextInputLayout_passwordToggleTint = 41;
				TextInputLayout_passwordToggleTintMode = 42;
				TextInputLayout_shapeAppearance = 43;
				TextInputLayout_shapeAppearanceOverlay = 44;
				TextInputLayout_startIconCheckable = 45;
				TextInputLayout_startIconContentDescription = 46;
				TextInputLayout_startIconDrawable = 47;
				TextInputLayout_startIconTint = 48;
				TextInputLayout_startIconTintMode = 49;
				ThemeEnforcement = new int[3]
				{
					16842804,
					2130903297,
					2130903298
				};
				ThemeEnforcement_android_textAppearance = 0;
				ThemeEnforcement_enforceMaterialTheme = 1;
				ThemeEnforcement_enforceTextAppearance = 2;
				Toolbar = new int[30]
				{
					16842927,
					16843072,
					2130903141,
					2130903195,
					2130903196,
					2130903227,
					2130903228,
					2130903229,
					2130903230,
					2130903231,
					2130903232,
					2130903486,
					2130903487,
					2130903509,
					2130903513,
					2130903516,
					2130903517,
					2130903537,
					2130903604,
					2130903605,
					2130903606,
					2130903679,
					2130903681,
					2130903682,
					2130903683,
					2130903684,
					2130903685,
					2130903686,
					2130903687,
					2130903688
				};
				Toolbar_android_gravity = 0;
				Toolbar_android_minHeight = 1;
				Toolbar_buttonGravity = 2;
				Toolbar_collapseContentDescription = 3;
				Toolbar_collapseIcon = 4;
				Toolbar_contentInsetEnd = 5;
				Toolbar_contentInsetEndWithActions = 6;
				Toolbar_contentInsetLeft = 7;
				Toolbar_contentInsetRight = 8;
				Toolbar_contentInsetStart = 9;
				Toolbar_contentInsetStartWithNavigation = 10;
				Toolbar_logo = 11;
				Toolbar_logoDescription = 12;
				Toolbar_maxButtonHeight = 13;
				Toolbar_menu = 14;
				Toolbar_navigationContentDescription = 15;
				Toolbar_navigationIcon = 16;
				Toolbar_popupTheme = 17;
				Toolbar_subtitle = 18;
				Toolbar_subtitleTextAppearance = 19;
				Toolbar_subtitleTextColor = 20;
				Toolbar_title = 21;
				Toolbar_titleMargin = 22;
				Toolbar_titleMarginBottom = 23;
				Toolbar_titleMarginEnd = 24;
				Toolbar_titleMargins = 27;
				Toolbar_titleMarginStart = 25;
				Toolbar_titleMarginTop = 26;
				Toolbar_titleTextAppearance = 28;
				Toolbar_titleTextColor = 29;
				View = new int[5]
				{
					16842752,
					16842970,
					2130903524,
					2130903525,
					2130903668
				};
				ViewBackgroundHelper = new int[3]
				{
					16842964,
					2130903101,
					2130903102
				};
				ViewBackgroundHelper_android_background = 0;
				ViewBackgroundHelper_backgroundTint = 1;
				ViewBackgroundHelper_backgroundTintMode = 2;
				ViewPager2 = new int[1]
				{
					16842948
				};
				ViewPager2_android_orientation = 0;
				ViewStubCompat = new int[3]
				{
					16842960,
					16842994,
					16842995
				};
				ViewStubCompat_android_id = 0;
				ViewStubCompat_android_inflatedId = 2;
				ViewStubCompat_android_layout = 1;
				View_android_focusable = 1;
				View_android_theme = 0;
				View_paddingEnd = 2;
				View_paddingStart = 3;
				View_theme = 4;
				ResourceIdManager.UpdateIdValues();
			}

			private Styleable()
			{
			}
		}

		public class Xml
		{
			public static int image_share_filepaths;

			public static int standalone_badge;

			public static int standalone_badge_gravity_bottom_end;

			public static int standalone_badge_gravity_bottom_start;

			public static int standalone_badge_gravity_top_start;

			public static int xamarin_essentials_fileprovider_file_paths;

			static Xml()
			{
				image_share_filepaths = 2131820544;
				standalone_badge = 2131820545;
				standalone_badge_gravity_bottom_end = 2131820546;
				standalone_badge_gravity_bottom_start = 2131820547;
				standalone_badge_gravity_top_start = 2131820548;
				xamarin_essentials_fileprovider_file_paths = 2131820549;
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
				throw new Exception("Invalid hex string");
			}
			int r = int.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
			int g = int.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
			int b = int.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);
			return new Color(r, g, b);
		}
	}
	public class DroidDependencyInjectionConfig
	{
		public static UnityContainer unityContainer;

		public static void Init()
		{
			unityContainer = new UnityContainer();
			CommonDependencyInjectionConfig.Init(unityContainer);
			UnityServiceLocator unityServiceLocalter = new UnityServiceLocator(unityContainer);
			ServiceLocator.SetLocatorProvider(() => unityServiceLocalter);
		}
	}
}
namespace NDB.Covid19.Droid.Shared.Services
{
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
					goto IL_002c;
				}
			}
			current = CrossCurrentActivity.Current.Activity;
			goto IL_002c;
			IL_002c:
			DialogUtils.DisplayDialog(current, title, message, okBtn);
		}
	}
}
namespace NDB.Covid19.Droid.Shared.Views
{
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	public class ForceUpdateActivity : AppCompatActivity
	{
		private bool IsGoogleHuawei
		{
			get
			{
				if (DeviceInfo.Manufacturer != null && DeviceInfo.Manufacturer.ToLower() == "huawei")
				{
					return !ServiceLocator.Current.GetInstance<ApiDataHelper>().ApiDataHelperInstance.IsGoogleServiceEnabled();
				}
				return false;
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(Resource.Layout.force_update);
			base.FindViewById<TextView>(Resource.Id.force_update_label).Text = ForceUpdateViewModel.FORCE_UPDATE_MESSAGE;
			Button button = base.FindViewById<Button>(Resource.Id.force_update_button);
			if (IsGoogleHuawei)
			{
				button.Text = ForceUpdateViewModel.FORCE_UPDATE_BUTTON_HUAWEI_ANDROID;
				button.ContentDescription = ForceUpdateViewModel.FORCE_UPDATE_BUTTON_HUAWEI_ANDROID;
				button.Click += new StressUtils.SingleClick(GoToHuaweiAppGallery).Run;
			}
			else
			{
				button.Text = ForceUpdateViewModel.FORCE_UPDATE_BUTTON_GOOGLE_ANDROID;
				button.ContentDescription = ForceUpdateViewModel.FORCE_UPDATE_BUTTON_GOOGLE_ANDROID;
				button.Click += new StressUtils.SingleClick(GoToGooglePlay).Run;
			}
		}

		private void GoToGooglePlay(object o, EventArgs eventArgs)
		{
			Intent intent = new Intent("android.intent.action.VIEW");
			intent.SetData(Android.Net.Uri.Parse(SharedConf.GooglePlayAppLink));
			((Context)(object)this).StartActivity(intent);
		}

		private void GoToHuaweiAppGallery(object o, EventArgs eventArgs)
		{
			Intent intent = new Intent("android.intent.action.VIEW");
			intent.SetData(Android.Net.Uri.Parse(SharedConf.HuaweiAppGalleryLink));
			((Context)(object)this).StartActivity(intent);
		}
	}
	public class BaseAppCompatActivity : AppCompatActivity
	{
		public enum AppState
		{
			IsAlive,
			IsDestroyed
		}

		protected bool? AreYouStillAlive;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			if (State(savedInstanceState) == AppState.IsDestroyed)
			{
				base.OnCreate((Bundle)null);
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
				base.OnCreate(savedInstanceState);
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
			base.OnSaveInstanceState(outState);
			outState.PutInt("SavedInstance", 1);
		}
	}
}
namespace NDB.Covid19.Droid.Shared.Views.Settings
{
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class SettingsActivity : AppCompatActivity
	{
		private static SettingsViewModel _settingsViewModel = new SettingsViewModel();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			((Activity)(object)this).SetContentView(Resource.Layout.settings_page);
			Init();
		}

		private void Init()
		{
			ConstraintLayout constraintLayout = base.FindViewById<ConstraintLayout>(Resource.Id.settings_intro_frame);
			ConstraintLayout constraintLayout2 = base.FindViewById<ConstraintLayout>(Resource.Id.settings_saddan_frame);
			ConstraintLayout constraintLayout3 = base.FindViewById<ConstraintLayout>(Resource.Id.settings_behandling_frame);
			ConstraintLayout constraintLayout4 = base.FindViewById<ConstraintLayout>(Resource.Id.settings_hjaelp_frame);
			ConstraintLayout constraintLayout5 = base.FindViewById<ConstraintLayout>(Resource.Id.om_frame);
			ConstraintLayout constraintLayout6 = base.FindViewById<ConstraintLayout>(Resource.Id.test_frame);
			constraintLayout.FindViewById<TextView>(Resource.Id.settings_link_text).Text = _settingsViewModel.SettingItemList[0].Text;
			constraintLayout2.FindViewById<TextView>(Resource.Id.settings_link_text).Text = _settingsViewModel.SettingItemList[1].Text;
			constraintLayout3.FindViewById<TextView>(Resource.Id.settings_link_text).Text = _settingsViewModel.SettingItemList[2].Text;
			constraintLayout4.FindViewById<TextView>(Resource.Id.settings_link_text).Text = _settingsViewModel.SettingItemList[3].Text;
			constraintLayout5.FindViewById<TextView>(Resource.Id.settings_link_text).Text = _settingsViewModel.SettingItemList[4].Text;
			if (_settingsViewModel.ShowDebugItem)
			{
				constraintLayout6.FindViewById<TextView>(Resource.Id.settings_link_text).Text = _settingsViewModel.SettingItemList[5].Text;
				constraintLayout6.Visibility = ViewStates.Visible;
			}
			ViewGroup viewGroup = base.FindViewById<ViewGroup>(Resource.Id.ic_close_white);
			viewGroup.ContentDescription = SettingsViewModel.SETTINGS_ITEM_ACCESSIBILITY_CLOSE_BUTTON;
			viewGroup.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
			constraintLayout.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToOnBoarding((Activity)(object)this, isOnBoarding: false);
			}).Run;
			constraintLayout2.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToSettingsHowItWorksPage((Activity)(object)this);
			}).Run;
			constraintLayout4.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToSettingsHelpPage((Activity)(object)this);
			}).Run;
			constraintLayout5.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToSettingsAboutPage((Activity)(object)this);
			}).Run;
			constraintLayout3.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToConsentsWithdrawPage((Activity)(object)this);
			}).Run;
			constraintLayout6.Click += new StressUtils.SingleClick(delegate
			{
				NavigationHelper.GoToDebugPage((Activity)(object)this);
			}).Run;
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class SettingsHelpActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			base.Title = SettingsPage4ViewModel.HEADER;
			((Activity)(object)this).SetContentView(Resource.Layout.settings_help);
			Init();
		}

		private void Init()
		{
			Button button = base.FindViewById<Button>(Resource.Id.arrow_back_help);
			button.ContentDescription = SettingsViewModel.SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON;
			TextView textView = base.FindViewById<TextView>(Resource.Id.settings_help_text);
			TextView textView2 = base.FindViewById<TextView>(Resource.Id.settings_help_title);
			TextView textView3 = base.FindViewById<TextView>(Resource.Id.settings_help_link);
			textView2.Text = SettingsPage4ViewModel.HEADER;
			textView.TextFormatted = HtmlCompat.FromHtml(SettingsPage4ViewModel.CONTENT_TEXT_BEFORE_SUPPORT_LINK + " <a href=\"https://" + SettingsPage4ViewModel.SUPPORT_LINK + "\">" + SettingsPage4ViewModel.SUPPORT_LINK_SHOWN_TEXT + "</a><br><br>" + SettingsPage4ViewModel.EMAIL_TEXT + " <a href=\"mailto:" + SettingsPage4ViewModel.EMAIL + "\">" + SettingsPage4ViewModel.EMAIL + "</a> " + SettingsPage4ViewModel.PHONE_NUM_Text + " <a href=\"tel:" + SettingsPage4ViewModel.PHONE_NUM + "\">" + SettingsPage4ViewModel.PHONE_NUM + "</a>.<br><br>" + SettingsPage4ViewModel.PHONE_OPEN_TEXT + "<br><br>" + SettingsPage4ViewModel.PHONE_OPEN_MON_THU + "<br>" + SettingsPage4ViewModel.PHONE_OPEN_FRE + "<br><br>" + SettingsPage4ViewModel.PHONE_OPEN_SAT_SUN_HOLY, 0);
			textView.ContentDescriptionFormatted = HtmlCompat.FromHtml(SettingsPage4ViewModel.CONTENT_TEXT_BEFORE_SUPPORT_LINK + " <a href=\"https://" + SettingsPage4ViewModel.SUPPORT_LINK + "\">" + SettingsPage4ViewModel.SUPPORT_LINK_SHOWN_TEXT + "</a><br><br>" + SettingsPage4ViewModel.EMAIL_TEXT + " <a href=\"mailto:" + SettingsPage4ViewModel.EMAIL + "\">" + SettingsPage4ViewModel.EMAIL + "</a> " + SettingsPage4ViewModel.PHONE_NUM_Text + " <a href=\"tel:" + SettingsPage4ViewModel.PHONE_NUM + "\">" + SettingsPage4ViewModel.PHONE_NUM_ACCESSIBILITY + "</a>.<br><br>" + SettingsPage4ViewModel.PHONE_OPEN_TEXT + "<br><br>" + SettingsPage4ViewModel.PHONE_OPEN_MON_THU_ACCESSIBILITY + "<br>" + SettingsPage4ViewModel.PHONE_OPEN_FRE_ACCESSIBILITY + "<br><br>" + SettingsPage4ViewModel.PHONE_OPEN_SAT_SUN_HOLY, 0);
			textView.MovementMethod = LinkMovementMethod.Instance;
			button.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
			textView3.Text = SettingsPage4ViewModel.SUPPORT_LINK;
			textView3.ContentDescription = SettingsPage4ViewModel.SUPPORT_LINK_SHOWN_TEXT;
			LinkUtil.LinkifyTextView(textView3);
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop, WindowSoftInputMode = SoftInput.AdjustResize)]
	internal class SettingsAbout : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			base.Title = SettingsPage5ViewModel.SETTINGS_PAGE_5_HEADER;
			((Activity)(object)this).SetContentView(Resource.Layout.settings_about);
			Init();
		}

		private void Init()
		{
			Button button = base.FindViewById<Button>(Resource.Id.arrow_back_about);
			button.ContentDescription = SettingsViewModel.SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON;
			TextView textView = base.FindViewById<TextView>(Resource.Id.settings_about_title);
			TextView textView2 = base.FindViewById<TextView>(Resource.Id.settings_about_text);
			TextView textView3 = base.FindViewById<TextView>(Resource.Id.settings_about_link);
			base.FindViewById<TextView>(Resource.Id.settings_about_version_info_textview).Text = SettingsPage5ViewModel.GetVersionInfo();
			textView.Text = SettingsPage5ViewModel.SETTINGS_PAGE_5_HEADER;
			textView2.Text = SettingsPage5ViewModel.SETTINGS_PAGE_5_CONTENT + " " + SettingsPage5ViewModel.SETTINGS_PAGE_5_LINK;
			button.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
			textView3.Text = SettingsPage5ViewModel.SETTINGS_PAGE_5_LINK;
			LinkUtil.LinkifyTextView(textView3);
		}

		private void GoToUrl()
		{
			((Context)(object)this).StartActivity(new Intent("android.intent.action.VIEW", Android.Net.Uri.Parse("https://" + SettingsPage5ViewModel.SETTINGS_PAGE_5_LINK)));
		}
	}
	[Activity(Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
	internal class SettingsHowItWorksActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			base.Title = SettingsPage2ViewModel.SETTINGS_PAGE_2_HEADER;
			((Activity)(object)this).SetContentView(Resource.Layout.settings_general_page);
			Init();
		}

		private void Init()
		{
			Button button = base.FindViewById<Button>(Resource.Id.arrow_back);
			button.ContentDescription = SettingsViewModel.SETTINGS_CHILD_PAGE_ACCESSIBILITY_BACK_BUTTON;
			TextView textView = base.FindViewById<TextView>(Resource.Id.settings_general_text);
			base.FindViewById<TextView>(Resource.Id.settings_general_title).Text = SettingsPage2ViewModel.SETTINGS_PAGE_2_HEADER;
			textView.TextFormatted = HtmlCompat.FromHtml(SettingsPage2ViewModel.SETTINGS_PAGE_2_CONTENT, 0);
			button.Click += new StressUtils.SingleClick(delegate
			{
				((Activity)(object)this).Finish();
			}).Run;
		}
	}
}
namespace NDB.Covid19.Droid.Shared.Utils
{
	public static class DialogUtils
	{
		[Obsolete("Use DisplayDialogAsync instead")]
		public static void DisplayDialog(Activity current, string title, string message, string okBtnText, Action action = null)
		{
			new AlertDialog.Builder(current, 16974126).SetTitle(title).SetMessage(message).SetPositiveButton(okBtnText, delegate
			{
				action?.Invoke();
			})
				.SetCancelable(cancelable: false)
				.Show();
		}

		public static void DisplayBubbleDialog(Activity current, string message, string buttonText)
		{
			View view = LayoutInflater.From(current).Inflate(Resource.Layout.bubble_layout, null);
			TextView textView = view.FindViewById<TextView>(Resource.Id.bubble_message);
			Button button = view.FindViewById<Button>(Resource.Id.buttonBubble);
			textView.Text = message;
			button.Text = buttonText;
			AlertDialog dialog = new AlertDialog.Builder(current).SetView(view).SetCancelable(cancelable: true).Create();
			button.Click += new StressUtils.SingleClick(delegate
			{
				dialog.Dismiss();
			}).Run;
			dialog.Window.SetLayout(-2, -2);
			dialog.Show();
		}

		public static Task<bool> DisplayDialogAsync(Activity activity, string title, string message, string okBtnText = null, string noBtnText = null, int? okBtnTextResourceId = null, int? noBtnTextResourceId = null)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			AlertDialog.Builder builder = new AlertDialog.Builder(activity, 16974126).SetTitle(title).SetMessage(message).SetCancelable(cancelable: false);
			if (!string.IsNullOrEmpty(okBtnText) || okBtnTextResourceId.HasValue)
			{
				if (!string.IsNullOrEmpty(okBtnText))
				{
					builder.SetPositiveButton(okBtnText, delegate
					{
						tcs.SetResult(result: true);
					});
				}
				if (okBtnTextResourceId.HasValue)
				{
					builder.SetPositiveButton(okBtnTextResourceId.Value, delegate
					{
						tcs.SetResult(result: true);
					});
				}
			}
			if (!string.IsNullOrEmpty(noBtnText) || noBtnTextResourceId.HasValue)
			{
				if (!string.IsNullOrEmpty(noBtnText))
				{
					builder.SetNegativeButton(noBtnText, delegate
					{
						tcs.SetResult(result: false);
					});
				}
				if (noBtnTextResourceId.HasValue)
				{
					builder.SetNegativeButton(noBtnTextResourceId.Value, delegate
					{
						tcs.SetResult(result: false);
					});
				}
			}
			builder.Show();
			return tcs.Task;
		}

		[Obsolete("Use DisplayDialogAsync instead")]
		public static void DisplayDialogExtended(Activity current, string title, string message, string okBtnText, string noBtnText, Action actionOk = null, Action actionNotOk = null)
		{
			new AlertDialog.Builder(current, 16974126).SetTitle(title).SetMessage(message).SetPositiveButton(okBtnText, delegate
			{
				actionOk?.Invoke();
			})
				.SetNegativeButton(noBtnText, delegate
				{
					actionNotOk?.Invoke();
				})
				.SetCancelable(cancelable: false)
				.Show();
		}

		[Obsolete("Use DisplayDialogAsync instead")]
		public static void DisplayDialogExtended(Activity current, DialogViewModel viewModel, Action actionOk = null, Action actionNotOk = null)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(current, 16974126).SetTitle(viewModel.Title).SetMessage(viewModel.Body).SetCancelable(!string.IsNullOrEmpty(viewModel.OkBtnTxt) || !string.IsNullOrEmpty(viewModel.CancelbtnTxt));
			if (!string.IsNullOrEmpty(viewModel.OkBtnTxt))
			{
				builder.SetPositiveButton(viewModel.OkBtnTxt, delegate
				{
					actionOk?.Invoke();
				});
			}
			if (!string.IsNullOrEmpty(viewModel.CancelbtnTxt))
			{
				builder.SetNegativeButton(viewModel.CancelbtnTxt, delegate
				{
					actionNotOk?.Invoke();
				});
			}
			builder.Show();
		}

		public static async Task<bool> DisplayDialogAsync(Activity current, DialogViewModel viewModel, Action actionOk = null, Action actionNotOk = null)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
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
	public class StressUtils
	{
		public class GenericSingleAction<T1, T2>
		{
			private bool _hasStarted;

			private readonly Action<T1, T2> _setOnAction;

			private int _delayMilliseconds;

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

		public class SingleAction
		{
			private bool _hasStarted;

			private readonly Action _setOnAction;

			private int _delayMilliseconds;

			public SingleAction(Action setOnAction, int delayMilliseconds = 1000)
			{
				_setOnAction = setOnAction;
				_delayMilliseconds = delayMilliseconds;
			}

			public void Run()
			{
				if (!_hasStarted)
				{
					_hasStarted = true;
					_setOnAction?.Invoke();
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

		public class SingleClick : SingleAction<EventArgs>
		{
			private bool _hasClicked;

			private readonly Action<object, EventArgs> _setOnClick;

			public SingleClick(Action<object, EventArgs> setOnAction, int delayMilliseconds = 1000)
				: base(setOnAction, delayMilliseconds)
			{
			}
		}

		public class SinglePress : SingleAction<bool>
		{
			public SinglePress(Action<object, bool> setOnAction, int delayMilliseconds = 1000)
				: base(setOnAction, delayMilliseconds)
			{
			}
		}
	}
	public class DroidRequestCodes
	{
		public const int BluetoothRequestCode = 1;

		public const int LocationRequestCode = 2;

		public const int EnApiRequestCode = 1111;

		public const string isOnBoardinIntentExtra = "isOnBoarding";
	}
	public static class NavigationHelper
	{
		public static void GoToStartPage(Activity parent)
		{
			ServiceLocator.Current.GetInstance<INavigationServiceDroid>().GoToStartPageIfIsOnboarded(parent);
		}

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

		public static void GoToOnBoarding(Activity parent, bool isOnBoarding)
		{
			ServiceLocator.Current.GetInstance<INavigationServiceDroid>().GoToOnBoarding(parent, isOnBoarding);
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

		public static void GoToConsentsWithdrawPage(Activity parent)
		{
			ServiceLocator.Current.GetInstance<INavigationServiceDroid>().GoToConsentsWithdrawPage(parent);
		}

		public static void GoToResultPage(Activity parent)
		{
			ServiceLocator.Current.GetInstance<INavigationServiceDroid>().GoToResultPage(parent);
		}

		public static void GoToDebugPage(Activity parent)
		{
			ServiceLocator.Current.GetInstance<INavigationServiceDroid>().GoToDebugPage(parent);
		}
	}
	public interface IPermissionUtils
	{
		Task<bool> HasBluetoothSupportAsync();

		Task<bool> HasPermissions();

		bool IsLocationEnabled();

		Task<bool> HasLocationPermissionsAsync();

		bool HasPermissionsWithoutDialogs();

		void SubscribePermissionsMessagingCenter(object subscriber, Action<object> action);

		void UnsubscribePErmissionsMessagingCenter(object subscriber);

		bool HasBluetoothSupport();

		bool DoesNotHavePermissions(bool withBluetoothAdapterCheck = true);

		void CheckMyOwnPermissions();

		Task<IPromise<bool>> CheckMyOwnPermissionsPromise();

		bool HasLocationPermissions();

		void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults);

		void OnActivityResult(int requestCode, Result resultCode, Intent data);

		Task<bool> CheckPermissionsIfChangedWhileIdle();
	}
	public static class MeasureUnitUtils
	{
		public static int ConvertDpToPixels(float dp)
		{
			return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Application.Context.Resources.DisplayMetrics);
		}

		public static int ConvertSpToPixels(float sp)
		{
			return (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, sp, Application.Context.Resources.DisplayMetrics);
		}
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
}
namespace NDB.Covid19.Droid.Shared.Utils.Navigation
{
	public interface INavigationServiceDroid
	{
		void GoToResultPage(Activity parent);

		void GoToDebugPage(Activity parent);

		void GoToConsentsWithdrawPage(Activity parent);

		void GoToOnBoarding(Activity parent, bool isOnBoarding);

		void GoToStartPageIfIsOnboarded(Activity parent);
	}
}
namespace NDB.Covid19.Droid.Shared.Utils.MessagingCenter
{
	public class PermissionsMessagingCenter
	{
		public static bool PermissionsChanged
		{
			get;
			set;
		}

		private static string PermissionsChangedKey => "PermissionsChangedKey";

		public static void SubscribeForPermissionsChanged(object sender, Action<object> action)
		{
			NDB.Covid19.Utils.MessagingCenter.Subscribe(sender, PermissionsChangedKey, action);
		}

		public static void Unsubscribe(object sender)
		{
			NDB.Covid19.Utils.MessagingCenter.Unsubscribe<object>(sender, PermissionsChangedKey);
		}

		public static void NotifyPermissionsChanged(object sender)
		{
			PermissionsChanged = true;
			NDB.Covid19.Utils.MessagingCenter.Send(sender, PermissionsChangedKey);
		}
	}
}
