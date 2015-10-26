using ApkShellext2;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;

namespace ApkQuickReader {
    public enum RES_TYPE : ushort {
        RES_NULL_TYPE = 0x0000,
        RES_STRING_POOL_TYPE = 0x0001,
        RES_TABLE_TYPE = 0x0002,
        RES_XML_TYPE = 0x0003,

        // Chunk types in RES_XML_TYPE
        RES_XML_FIRST_CHUNK_TYPE = 0x0100,
        RES_XML_START_NAMESPACE_TYPE = 0x0100,
        RES_XML_END_NAMESPACE_TYPE = 0x0101,
        RES_XML_START_ELEMENT_TYPE = 0x0102,
        RES_XML_END_ELEMENT_TYPE = 0x0103,
        RES_XML_CDATA_TYPE = 0x0104,
        RES_XML_LAST_CHUNK_TYPE = 0x017f,
        // This contains a uint32_t array mapping strings in the string
        // pool back to resource identifiers.  It is optional.
        RES_XML_RESOURCE_MAP_TYPE = 0x0180,

        // Chunk types in RES_TABLE_TYPE
        RES_TABLE_PACKAGE_TYPE = 0x0200,
        RES_TABLE_TYPE_TYPE = 0x0201,
        RES_TABLE_TYPE_SPEC_TYPE = 0x0202
    };

    public enum DATA_TYPE : byte {
        // Contains no data.
        TYPE_NULL = 0x00,
        // The 'data' holds an attribute resource identifier.
        TYPE_ATTRIBUTE = 0x02,
        // The 'data' holds a single-precision floating point number.
        TYPE_FLOAT = 0x04,
        // The 'data' holds a complex number encoding a dimension value,
        // such as "100in".
        TYPE_DIMENSION = 0x05,
        // The 'data' holds a complex number encoding a fraction of a
        // container.
        TYPE_FRACTION = 0x06,
        // The 'data' is a raw integer value of the form n..n.
        TYPE_INT_DEC = 0x10,
        // The 'data' is a raw integer value of the form 0xn..n.
        TYPE_INT_HEX = 0x11,
        // The 'data' is either 0 or 1, for input "false" or "true" respectively.
        TYPE_INT_BOOLEAN = 0x12,
        // The 'data' is a raw integer value of the form #aarrggbb.
        TYPE_INT_COLOR_ARGB8 = 0x1c,
        // The 'data' is a raw integer value of the form #rrggbb.
        TYPE_INT_COLOR_RGB8 = 0x1d,
        // The 'data' is a raw integer value of the form #argb.
        TYPE_INT_COLOR_ARGB4 = 0x1e,
        // The 'data' is a raw integer value of the form #rgb.
        TYPE_INT_COLOR_RGB4 = 0x1f,
        TYPE_REFERENCE = 0x01,
        TYPE_STRING = 0x03,
    }

    public enum RDefaultString : uint {
        absListViewStyle = 16842858, // 0x101006a
	    accessibilityEventTypes = 16843648, // 0x1010380
	    accessibilityFeedbackType = 16843650, // 0x1010382
	    accessibilityFlags = 16843652, // 0x1010384
	    accountPreferences = 16843423, // 0x101029f
	    accountType = 16843407, // 0x101028f
	    action = 16842797, // 0x101002d
	    actionBarDivider = 16843675, // 0x101039b
	    actionBarItemBackground = 16843676, // 0x101039c
	    actionBarSize = 16843499, // 0x10102eb
	    actionBarSplitStyle = 16843656, // 0x1010388
	    actionBarStyle = 16843470, // 0x10102ce
	    actionBarTabBarStyle = 16843508, // 0x10102f4
	    actionBarTabStyle = 16843507, // 0x10102f3
	    actionBarTabTextStyle = 16843509, // 0x10102f5
	    actionBarWidgetTheme = 16843671, // 0x1010397
	    actionButtonStyle = 16843480, // 0x10102d8
	    actionDropDownStyle = 16843479, // 0x10102d7
	    actionLayout = 16843515, // 0x10102fb
	    actionMenuTextAppearance = 16843616, // 0x1010360
	    actionMenuTextColor = 16843617, // 0x1010361
	    actionModeBackground = 16843483, // 0x10102db
	    actionModeCloseButtonStyle = 16843511, // 0x10102f7
	    actionModeCloseDrawable = 16843484, // 0x10102dc
	    actionModeCopyDrawable = 16843538, // 0x1010312
	    actionModeCutDrawable = 16843537, // 0x1010311
	    actionModePasteDrawable = 16843539, // 0x1010313
	    actionModeSelectAllDrawable = 16843646, // 0x101037e
	    actionModeSplitBackground = 16843677, // 0x101039d
	    actionModeStyle = 16843668, // 0x1010394
	    actionOverflowButtonStyle = 16843510, // 0x10102f6
	    actionProviderClass = 16843657, // 0x1010389
	    actionViewClass = 16843516, // 0x10102fc
	    activatedBackgroundIndicator = 16843517, // 0x10102fd
	    activityCloseEnterAnimation = 16842938, // 0x10100ba
	    activityCloseExitAnimation = 16842939, // 0x10100bb
	    activityOpenEnterAnimation = 16842936, // 0x10100b8
	    activityOpenExitAnimation = 16842937, // 0x10100b9
	    addStatesFromChildren = 16842992, // 0x10100f0
	    adjustViewBounds = 16843038, // 0x101011e
	    alertDialogIcon = 16843605, // 0x1010355
	    alertDialogStyle = 16842845, // 0x101005d
	    alertDialogTheme = 16843529, // 0x1010309
	    alignmentMode = 16843642, // 0x101037a
	    allContactsName = 16843468, // 0x10102cc
	    allowBackup = 16843392, // 0x1010280
	    allowClearUserData = 16842757, // 0x1010005
	    allowParallelSyncs = 16843570, // 0x1010332
	    allowSingleTap = 16843353, // 0x1010259
	    allowTaskReparenting = 16843268, // 0x1010204
	    alpha = 16843551, // 0x101031f
	    alphabeticShortcut = 16843235, // 0x10101e3
	    alwaysDrawnWithCache = 16842991, // 0x10100ef
	    alwaysRetainTaskState = 16843267, // 0x1010203
	    angle = 16843168, // 0x10101a0
	    animateFirstView = 16843477, // 0x10102d5
	    animateLayoutChanges = 16843506, // 0x10102f2
	    animateOnClick = 16843356, // 0x101025c
	    animation = 16843213, // 0x10101cd
	    animationCache = 16842989, // 0x10100ed
	    animationDuration = 16843026, // 0x1010112
	    animationOrder = 16843214, // 0x10101ce
	    animationResolution = 16843546, // 0x101031a
	    antialias = 16843034, // 0x101011a
	    anyDensity = 16843372, // 0x101026c
	    apiKey = 16843281, // 0x1010211
	    author = 16843444, // 0x10102b4
	    authorities = 16842776, // 0x1010018
	    autoAdvanceViewId = 16843535, // 0x101030f
	    autoCompleteTextViewStyle = 16842859, // 0x101006b
	    autoLink = 16842928, // 0x10100b0
	    autoStart = 16843445, // 0x10102b5
	    autoText = 16843114, // 0x101016a
	    autoUrlDetect = 16843404, // 0x101028c
	    background = 16842964, // 0x10100d4
	    backgroundDimAmount = 16842802, // 0x1010032
	    backgroundDimEnabled = 16843295, // 0x101021f
	    backgroundSplit = 16843659, // 0x101038b
	    backgroundStacked = 16843658, // 0x101038a
	    backupAgent = 16843391, // 0x101027f
	    baseline = 16843548, // 0x101031c
	    baselineAlignBottom = 16843042, // 0x1010122
	    baselineAligned = 16843046, // 0x1010126
	    baselineAlignedChildIndex = 16843047, // 0x1010127
	    borderlessButtonStyle = 16843563, // 0x101032b
	    bottom = 16843184, // 0x10101b0
	    bottomBright = 16842957, // 0x10100cd
	    bottomDark = 16842953, // 0x10100c9
	    bottomLeftRadius = 16843179, // 0x10101ab
	    bottomMedium = 16842958, // 0x10100ce
	    bottomOffset = 16843351, // 0x1010257
	    bottomRightRadius = 16843180, // 0x10101ac
	    breadCrumbShortTitle = 16843524, // 0x1010304
	    breadCrumbTitle = 16843523, // 0x1010303
	    bufferType = 16843086, // 0x101014e
	    button = 16843015, // 0x1010107
	    buttonBarButtonStyle = 16843567, // 0x101032f
	    buttonBarStyle = 16843566, // 0x101032e
	    buttonStyle = 16842824, // 0x1010048
	    buttonStyleInset = 16842826, // 0x101004a
	    buttonStyleSmall = 16842825, // 0x1010049
	    buttonStyleToggle = 16842827, // 0x101004b
	    cacheColorHint = 16843009, // 0x1010101
	    calendarViewShown = 16843596, // 0x101034c
	    calendarViewStyle = 16843613, // 0x101035d
	    canRetrieveWindowContent = 16843653, // 0x1010385
	    candidatesTextStyleSpans = 16843312, // 0x1010230
	    capitalize = 16843113, // 0x1010169
	    centerBright = 16842956, // 0x10100cc
	    centerColor = 16843275, // 0x101020b
	    centerDark = 16842952, // 0x10100c8
	    centerMedium = 16842959, // 0x10100cf
	    centerX = 16843170, // 0x10101a2
	    centerY = 16843171, // 0x10101a3
	    checkBoxPreferenceStyle = 16842895, // 0x101008f
	    checkMark = 16843016, // 0x1010108
	    checkable = 16843237, // 0x10101e5
	    checkableBehavior = 16843232, // 0x10101e0
	    checkboxStyle = 16842860, // 0x101006c
	    _checked = 16843014, // 0x1010106
	    checkedButton = 16843080, // 0x1010148
	    childDivider = 16843025, // 0x1010111
	    childIndicator = 16843020, // 0x101010c
	    childIndicatorLeft = 16843023, // 0x101010f
	    childIndicatorRight = 16843024, // 0x1010110
	    choiceMode = 16843051, // 0x101012b
	    clearTaskOnLaunch = 16842773, // 0x1010015
	    clickable = 16842981, // 0x10100e5
	    clipChildren = 16842986, // 0x10100ea
	    clipOrientation = 16843274, // 0x101020a
	    clipToPadding = 16842987, // 0x10100eb
	    codes = 16843330, // 0x1010242
	    collapseColumns = 16843083, // 0x101014b
	    color = 16843173, // 0x10101a5
	    colorActivatedHighlight = 16843664, // 0x1010390
	    colorBackground = 16842801, // 0x1010031
	    colorBackgroundCacheHint = 16843435, // 0x10102ab
	    colorFocusedHighlight = 16843663, // 0x101038f
	    colorForeground = 16842800, // 0x1010030
	    colorForegroundInverse = 16843270, // 0x1010206
	    colorLongPressedHighlight = 16843662, // 0x101038e
	    colorMultiSelectHighlight = 16843665, // 0x1010391
	    colorPressedHighlight = 16843661, // 0x101038d
	    columnCount = 16843639, // 0x1010377
	    columnDelay = 16843215, // 0x10101cf
	    columnOrderPreserved = 16843640, // 0x1010378
	    columnWidth = 16843031, // 0x1010117
	    compatibleWidthLimitDp = 16843621, // 0x1010365
	    completionHint = 16843122, // 0x1010172
	    completionHintView = 16843123, // 0x1010173
	    completionThreshold = 16843124, // 0x1010174
	    configChanges = 16842783, // 0x101001f
	    configure = 16843357, // 0x101025d
	    constantSize = 16843158, // 0x1010196
	    content = 16843355, // 0x101025b
	    contentAuthority = 16843408, // 0x1010290
	    contentDescription = 16843379, // 0x1010273
	    cropToPadding = 16843043, // 0x1010123
	    cursorVisible = 16843090, // 0x1010152
	    customNavigationLayout = 16843474, // 0x10102d2
	    customTokens = 16843579, // 0x101033b
	    cycles = 16843220, // 0x10101d4
	    dashGap = 16843175, // 0x10101a7
	    dashWidth = 16843174, // 0x10101a6
	    data = 16842798, // 0x101002e
	    datePickerStyle = 16843612, // 0x101035c
	    dateTextAppearance = 16843593, // 0x1010349
	    debuggable = 16842767, // 0x101000f
	    defaultValue = 16843245, // 0x10101ed
	    delay = 16843212, // 0x10101cc
	    dependency = 16843244, // 0x10101ec
	    descendantFocusability = 16842993, // 0x10100f1
	    description = 16842784, // 0x1010020
	    detachWallpaper = 16843430, // 0x10102a6
	    detailColumn = 16843427, // 0x10102a3
	    detailSocialSummary = 16843428, // 0x10102a4
	    detailsElementBackground = 16843598, // 0x101034e
	    dial = 16843010, // 0x1010102
	    dialogIcon = 16843252, // 0x10101f4
	    dialogLayout = 16843255, // 0x10101f7
	    dialogMessage = 16843251, // 0x10101f3
	    dialogPreferenceStyle = 16842897, // 0x1010091
	    dialogTheme = 16843528, // 0x1010308
	    dialogTitle = 16843250, // 0x10101f2
	    digits = 16843110, // 0x1010166
	    direction = 16843217, // 0x10101d1
	    directionDescriptions = 16843681, // 0x10103a1
	    directionPriority = 16843218, // 0x10101d2
	    disableDependentsState = 16843249, // 0x10101f1
	    disabledAlpha = 16842803, // 0x1010033
	    displayOptions = 16843472, // 0x10102d0
	    dither = 16843036, // 0x101011c
	    divider = 16843049, // 0x1010129
	    dividerHeight = 16843050, // 0x101012a
	    dividerHorizontal = 16843564, // 0x101032c
	    dividerPadding = 16843562, // 0x101032a
	    dividerVertical = 16843530, // 0x101030a
	    drawSelectorOnTop = 16843004, // 0x10100fc
	    drawable = 16843161, // 0x1010199
	    drawableBottom = 16843118, // 0x101016e
	    drawableEnd = 16843667, // 0x1010393
	    drawableLeft = 16843119, // 0x101016f
	    drawablePadding = 16843121, // 0x1010171
	    drawableRight = 16843120, // 0x1010170
	    drawableStart = 16843666, // 0x1010392
	    drawableTop = 16843117, // 0x101016d
	    drawingCacheQuality = 16842984, // 0x10100e8
	    dropDownAnchor = 16843363, // 0x1010263
	    dropDownHeight = 16843395, // 0x1010283
	    dropDownHintAppearance = 16842888, // 0x1010088
	    dropDownHorizontalOffset = 16843436, // 0x10102ac
	    dropDownItemStyle = 16842886, // 0x1010086
	    dropDownListViewStyle = 16842861, // 0x101006d
	    dropDownSelector = 16843125, // 0x1010175
	    dropDownSpinnerStyle = 16843478, // 0x10102d6
	    dropDownVerticalOffset = 16843437, // 0x10102ad
	    dropDownWidth = 16843362, // 0x1010262
	    duplicateParentState = 16842985, // 0x10100e9
	    duration = 16843160, // 0x1010198
	    editTextBackground = 16843602, // 0x1010352
	    editTextColor = 16843601, // 0x1010351
	    editTextPreferenceStyle = 16842898, // 0x1010092
	    editTextStyle = 16842862, // 0x101006e
	    editable = 16843115, // 0x101016b
	    editorExtras = 16843300, // 0x1010224
	    ellipsize = 16842923, // 0x10100ab
	    ems = 16843096, // 0x1010158
	    enabled = 16842766, // 0x101000e
	    endColor = 16843166, // 0x101019e
	    endYear = 16843133, // 0x101017d
	    enterFadeDuration = 16843532, // 0x101030c
	    entries = 16842930, // 0x10100b2
	    entryValues = 16843256, // 0x10101f8
	    eventsInterceptionEnabled = 16843389, // 0x101027d
	    excludeFromRecents = 16842775, // 0x1010017
	    exitFadeDuration = 16843533, // 0x101030d
	    expandableListPreferredChildIndicatorLeft = 16842834, // 0x1010052
	    expandableListPreferredChildIndicatorRight = 16842835, // 0x1010053
	    expandableListPreferredChildPaddingLeft = 16842831, // 0x101004f
	    expandableListPreferredItemIndicatorLeft = 16842832, // 0x1010050
	    expandableListPreferredItemIndicatorRight = 16842833, // 0x1010051
	    expandableListPreferredItemPaddingLeft = 16842830, // 0x101004e
	    expandableListViewStyle = 16842863, // 0x101006f
	    expandableListViewWhiteStyle = 16843446, // 0x10102b6
	    exported = 16842768, // 0x1010010
	    extraTension = 16843371, // 0x101026b
	    factor = 16843219, // 0x10101d3
	    fadeDuration = 16843384, // 0x1010278
	    fadeEnabled = 16843390, // 0x101027e
	    fadeOffset = 16843383, // 0x1010277
	    fadeScrollbars = 16843434, // 0x10102aa
	    fadingEdge = 16842975, // 0x10100df
	    fadingEdgeLength = 16842976, // 0x10100e0
	    fastScrollAlwaysVisible = 16843573, // 0x1010335
	    fastScrollEnabled = 16843302, // 0x1010226
	    fastScrollOverlayPosition = 16843578, // 0x101033a
	    fastScrollPreviewBackgroundLeft = 16843575, // 0x1010337
	    fastScrollPreviewBackgroundRight = 16843576, // 0x1010338
	    fastScrollTextColor = 16843609, // 0x1010359
	    fastScrollThumbDrawable = 16843574, // 0x1010336
	    fastScrollTrackDrawable = 16843577, // 0x1010339
	    fillAfter = 16843197, // 0x10101bd
	    fillBefore = 16843196, // 0x10101bc
	    fillEnabled = 16843343, // 0x101024f
	    fillViewport = 16843130, // 0x101017a
	    filter = 16843035, // 0x101011b
	    filterTouchesWhenObscured = 16843460, // 0x10102c4
	    finishOnCloseSystemDialogs = 16843431, // 0x10102a7
	    finishOnTaskLaunch = 16842772, // 0x1010014
	    firstDayOfWeek = 16843581, // 0x101033d
	    fitsSystemWindows = 16842973, // 0x10100dd
	    flipInterval = 16843129, // 0x1010179
	    focusable = 16842970, // 0x10100da
	    focusableInTouchMode = 16842971, // 0x10100db
	    focusedMonthDateColor = 16843587, // 0x1010343
	    fontFamily = 16843692, // 0x10103ac
	    footerDividersEnabled = 16843311, // 0x101022f
	    foreground = 16843017, // 0x1010109
	    foregroundGravity = 16843264, // 0x1010200
	    format = 16843013, // 0x1010105
	    fragment = 16843491, // 0x10102e3
	    fragmentCloseEnterAnimation = 16843495, // 0x10102e7
	    fragmentCloseExitAnimation = 16843496, // 0x10102e8
	    fragmentFadeEnterAnimation = 16843497, // 0x10102e9
	    fragmentFadeExitAnimation = 16843498, // 0x10102ea
	    fragmentOpenEnterAnimation = 16843493, // 0x10102e5
	    fragmentOpenExitAnimation = 16843494, // 0x10102e6
	    freezesText = 16843116, // 0x101016c
	    fromAlpha = 16843210, // 0x10101ca
	    fromDegrees = 16843187, // 0x10101b3
	    fromXDelta = 16843206, // 0x10101c6
	    fromXScale = 16843202, // 0x10101c2
	    fromYDelta = 16843208, // 0x10101c8
	    fromYScale = 16843204, // 0x10101c4
	    fullBright = 16842954, // 0x10100ca
	    fullDark = 16842950, // 0x10100c6
	    functionalTest = 16842787, // 0x1010023
	    galleryItemBackground = 16842828, // 0x101004c
	    galleryStyle = 16842864, // 0x1010070
	    gestureColor = 16843381, // 0x1010275
	    gestureStrokeAngleThreshold = 16843388, // 0x101027c
	    gestureStrokeLengthThreshold = 16843386, // 0x101027a
	    gestureStrokeSquarenessThreshold = 16843387, // 0x101027b
	    gestureStrokeType = 16843385, // 0x1010279
	    gestureStrokeWidth = 16843380, // 0x1010274
	    glEsVersion = 16843393, // 0x1010281
	    gradientRadius = 16843172, // 0x10101a4
	    grantUriPermissions = 16842779, // 0x101001b
	    gravity = 16842927, // 0x10100af
	    gridViewStyle = 16842865, // 0x1010071
	    groupIndicator = 16843019, // 0x101010b
	    hand_hour = 16843011, // 0x1010103
	    hand_minute = 16843012, // 0x1010104
	    handle = 16843354, // 0x101025a
	    handleProfiling = 16842786, // 0x1010022
	    hapticFeedbackEnabled = 16843358, // 0x101025e
	    hardwareAccelerated = 16843475, // 0x10102d3
	    hasCode = 16842764, // 0x101000c
	    headerBackground = 16843055, // 0x101012f
	    headerDividersEnabled = 16843310, // 0x101022e
	    height = 16843093, // 0x1010155
	    hint = 16843088, // 0x1010150
	    homeAsUpIndicator = 16843531, // 0x101030b
	    homeLayout = 16843549, // 0x101031d
	    horizontalDivider = 16843053, // 0x101012d
	    horizontalGap = 16843327, // 0x101023f
	    horizontalScrollViewStyle = 16843603, // 0x1010353
	    horizontalSpacing = 16843028, // 0x1010114
	    host = 16842792, // 0x1010028
	    icon = 16842754, // 0x1010002
	    iconPreview = 16843337, // 0x1010249
	    iconifiedByDefault = 16843514, // 0x10102fa
	    id = 16842960, // 0x10100d0
	    ignoreGravity = 16843263, // 0x10101ff
	    imageButtonStyle = 16842866, // 0x1010072
	    imageWellStyle = 16842867, // 0x1010073
	    imeActionId = 16843366, // 0x1010266
	    imeActionLabel = 16843365, // 0x1010265
	    imeExtractEnterAnimation = 16843368, // 0x1010268
	    imeExtractExitAnimation = 16843369, // 0x1010269
	    imeFullscreenBackground = 16843308, // 0x101022c
	    imeOptions = 16843364, // 0x1010264
	    imeSubtypeExtraValue = 16843502, // 0x10102ee
	    imeSubtypeLocale = 16843500, // 0x10102ec
	    imeSubtypeMode = 16843501, // 0x10102ed
	    immersive = 16843456, // 0x10102c0
	    importantForAccessibility = 16843690, // 0x10103aa
	    inAnimation = 16843127, // 0x1010177
	    includeFontPadding = 16843103, // 0x101015f
	    includeInGlobalSearch = 16843374, // 0x101026e
	    indeterminate = 16843065, // 0x1010139
	    indeterminateBehavior = 16843070, // 0x101013e
	    indeterminateDrawable = 16843067, // 0x101013b
	    indeterminateDuration = 16843069, // 0x101013d
	    indeterminateOnly = 16843066, // 0x101013a
	    indeterminateProgressStyle = 16843544, // 0x1010318
	    indicatorLeft = 16843021, // 0x101010d
	    indicatorRight = 16843022, // 0x101010e
	    inflatedId = 16842995, // 0x10100f3
	    initOrder = 16842778, // 0x101001a
	    initialLayout = 16843345, // 0x1010251
	    innerRadius = 16843359, // 0x101025f
	    innerRadiusRatio = 16843163, // 0x101019b
	    inputMethod = 16843112, // 0x1010168
	    inputType = 16843296, // 0x1010220
	    insetBottom = 16843194, // 0x10101ba
	    insetLeft = 16843191, // 0x10101b7
	    insetRight = 16843192, // 0x10101b8
	    insetTop = 16843193, // 0x10101b9
	    installLocation = 16843447, // 0x10102b7
	    interpolator = 16843073, // 0x1010141
	    isAlwaysSyncable = 16843571, // 0x1010333
	    isAuxiliary = 16843647, // 0x101037f
	    isDefault = 16843297, // 0x1010221
	    isIndicator = 16843079, // 0x1010147
	    isModifier = 16843334, // 0x1010246
	    isRepeatable = 16843336, // 0x1010248
	    isScrollContainer = 16843342, // 0x101024e
	    isSticky = 16843335, // 0x1010247
	    isolatedProcess = 16843689, // 0x10103a9
	    itemBackground = 16843056, // 0x1010130
	    itemIconDisabledAlpha = 16843057, // 0x1010131
	    itemPadding = 16843565, // 0x101032d
	    itemTextAppearance = 16843052, // 0x101012c
	    keepScreenOn = 16843286, // 0x1010216
	    key = 16843240, // 0x10101e8
	    keyBackground = 16843315, // 0x1010233
	    keyEdgeFlags = 16843333, // 0x1010245
	    keyHeight = 16843326, // 0x101023e
	    keyIcon = 16843340, // 0x101024c
	    keyLabel = 16843339, // 0x101024b
	    keyOutputText = 16843338, // 0x101024a
	    keyPreviewHeight = 16843321, // 0x1010239
	    keyPreviewLayout = 16843319, // 0x1010237
	    keyPreviewOffset = 16843320, // 0x1010238
	    keyTextColor = 16843318, // 0x1010236
	    keyTextSize = 16843316, // 0x1010234
	    keyWidth = 16843325, // 0x101023d
	    keyboardLayout = 16843691, // 0x10103ab
	    keyboardMode = 16843341, // 0x101024d
	    keycode = 16842949, // 0x10100c5
	    killAfterRestore = 16843420, // 0x101029c
	    label = 16842753, // 0x1010001
	    labelTextSize = 16843317, // 0x1010235
	    largeHeap = 16843610, // 0x101035a
	    largeScreens = 16843398, // 0x1010286
	    largestWidthLimitDp = 16843622, // 0x1010366
	    launchMode = 16842781, // 0x101001d
	    layerType = 16843604, // 0x1010354
	    layout = 16842994, // 0x10100f2
	    layoutAnimation = 16842988, // 0x10100ec
	    layoutDirection = 16843698, // 0x10103b2
	    layout_above = 16843140, // 0x1010184
	    layout_alignBaseline = 16843142, // 0x1010186
	    layout_alignBottom = 16843146, // 0x101018a
	    layout_alignEnd = 16843706, // 0x10103ba
	    layout_alignLeft = 16843143, // 0x1010187
	    layout_alignParentBottom = 16843150, // 0x101018e
	    layout_alignParentEnd = 16843708, // 0x10103bc
	    layout_alignParentLeft = 16843147, // 0x101018b
	    layout_alignParentRight = 16843149, // 0x101018d
	    layout_alignParentStart = 16843707, // 0x10103bb
	    layout_alignParentTop = 16843148, // 0x101018c
	    layout_alignRight = 16843145, // 0x1010189
	    layout_alignStart = 16843705, // 0x10103b9
	    layout_alignTop = 16843144, // 0x1010188
	    layout_alignWithParentIfMissing = 16843154, // 0x1010192
	    layout_below = 16843141, // 0x1010185
	    layout_centerHorizontal = 16843152, // 0x1010190
	    layout_centerInParent = 16843151, // 0x101018f
	    layout_centerVertical = 16843153, // 0x1010191
	    layout_column = 16843084, // 0x101014c
	    layout_columnSpan = 16843645, // 0x101037d
	    layout_gravity = 16842931, // 0x10100b3
	    layout_height = 16842997, // 0x10100f5
	    layout_margin = 16842998, // 0x10100f6
	    layout_marginBottom = 16843002, // 0x10100fa
	    layout_marginEnd = 16843702, // 0x10103b6
	    layout_marginLeft = 16842999, // 0x10100f7
	    layout_marginRight = 16843001, // 0x10100f9
	    layout_marginStart = 16843701, // 0x10103b5
	    layout_marginTop = 16843000, // 0x10100f8
	    layout_row = 16843643, // 0x101037b
	    layout_rowSpan = 16843644, // 0x101037c
	    layout_scale = 16843155, // 0x1010193
	    layout_span = 16843085, // 0x101014d
	    layout_toEndOf = 16843704, // 0x10103b8
	    layout_toLeftOf = 16843138, // 0x1010182
	    layout_toRightOf = 16843139, // 0x1010183
	    layout_toStartOf = 16843703, // 0x10103b7
	    layout_weight = 16843137, // 0x1010181
	    layout_width = 16842996, // 0x10100f4
	    layout_x = 16843135, // 0x101017f
	    layout_y = 16843136, // 0x1010180
	    left = 16843181, // 0x10101ad
	    lineSpacingExtra = 16843287, // 0x1010217
	    lineSpacingMultiplier = 16843288, // 0x1010218
	    lines = 16843092, // 0x1010154
	    linksClickable = 16842929, // 0x10100b1
	    listChoiceBackgroundIndicator = 16843504, // 0x10102f0
	    listChoiceIndicatorMultiple = 16843290, // 0x101021a
	    listChoiceIndicatorSingle = 16843289, // 0x1010219
	    listDivider = 16843284, // 0x1010214
	    listDividerAlertDialog = 16843525, // 0x1010305
	    listPopupWindowStyle = 16843519, // 0x10102ff
	    listPreferredItemHeight = 16842829, // 0x101004d
	    listPreferredItemHeightLarge = 16843654, // 0x1010386
	    listPreferredItemHeightSmall = 16843655, // 0x1010387
	    listPreferredItemPaddingEnd = 16843710, // 0x10103be
	    listPreferredItemPaddingLeft = 16843683, // 0x10103a3
	    listPreferredItemPaddingRight = 16843684, // 0x10103a4
	    listPreferredItemPaddingStart = 16843709, // 0x10103bd
	    listSelector = 16843003, // 0x10100fb
	    listSeparatorTextViewStyle = 16843272, // 0x1010208
	    listViewStyle = 16842868, // 0x1010074
	    listViewWhiteStyle = 16842869, // 0x1010075
	    logo = 16843454, // 0x10102be
	    longClickable = 16842982, // 0x10100e6
	    loopViews = 16843527, // 0x1010307
	    manageSpaceActivity = 16842756, // 0x1010004
	    mapViewStyle = 16842890, // 0x101008a
	    marqueeRepeatLimit = 16843293, // 0x101021d
	    max = 16843062, // 0x1010136
	    maxDate = 16843584, // 0x1010340
	    maxEms = 16843095, // 0x1010157
	    maxHeight = 16843040, // 0x1010120
	    maxItemsPerRow = 16843060, // 0x1010134
	    maxLength = 16843104, // 0x1010160
	    maxLevel = 16843186, // 0x10101b2
	    maxLines = 16843091, // 0x1010153
	    maxRows = 16843059, // 0x1010133
	    maxSdkVersion = 16843377, // 0x1010271
	    maxWidth = 16843039, // 0x101011f
	    measureAllChildren = 16843018, // 0x101010a
	    measureWithLargestChild = 16843476, // 0x10102d4
	    mediaRouteButtonStyle = 16843693, // 0x10103ad
	    mediaRouteTypes = 16843694, // 0x10103ae
	    menuCategory = 16843230, // 0x10101de
	    mimeType = 16842790, // 0x1010026
	    minDate = 16843583, // 0x101033f
	    minEms = 16843098, // 0x101015a
	    minHeight = 16843072, // 0x1010140
	    minLevel = 16843185, // 0x10101b1
	    minLines = 16843094, // 0x1010156
	    minResizeHeight = 16843670, // 0x1010396
	    minResizeWidth = 16843669, // 0x1010395
	    minSdkVersion = 16843276, // 0x101020c
	    minWidth = 16843071, // 0x101013f
	    mode = 16843134, // 0x101017e
	    moreIcon = 16843061, // 0x1010135
	    multiprocess = 16842771, // 0x1010013
	    name = 16842755, // 0x1010003
	    navigationMode = 16843471, // 0x10102cf
	    negativeButtonText = 16843254, // 0x10101f6
	    nextFocusDown = 16842980, // 0x10100e4
	    nextFocusForward = 16843580, // 0x101033c
	    nextFocusLeft = 16842977, // 0x10100e1
	    nextFocusRight = 16842978, // 0x10100e2
	    nextFocusUp = 16842979, // 0x10100e3
	    noHistory = 16843309, // 0x101022d
	    normalScreens = 16843397, // 0x1010285
	    notificationTimeout = 16843651, // 0x1010383
	    numColumns = 16843032, // 0x1010118
	    numStars = 16843076, // 0x1010144
	    numeric = 16843109, // 0x1010165
	    numericShortcut = 16843236, // 0x10101e4
	    onClick = 16843375, // 0x101026f
	    oneshot = 16843159, // 0x1010197
	    opacity = 16843550, // 0x101031e
	    order = 16843242, // 0x10101ea
	    orderInCategory = 16843231, // 0x10101df
	    ordering = 16843490, // 0x10102e2
	    orderingFromXml = 16843239, // 0x10101e7
	    orientation = 16842948, // 0x10100c4
	    outAnimation = 16843128, // 0x1010178
	    overScrollFooter = 16843459, // 0x10102c3
	    overScrollHeader = 16843458, // 0x10102c2
	    overScrollMode = 16843457, // 0x10102c1
	    overridesImplicitlyEnabledSubtype = 16843682, // 0x10103a2
	    packageNames = 16843649, // 0x1010381
	    padding = 16842965, // 0x10100d5
	    paddingBottom = 16842969, // 0x10100d9
	    paddingEnd = 16843700, // 0x10103b4
	    paddingLeft = 16842966, // 0x10100d6
	    paddingRight = 16842968, // 0x10100d8
	    paddingStart = 16843699, // 0x10103b3
	    paddingTop = 16842967, // 0x10100d7
	    panelBackground = 16842846, // 0x101005e
	    panelColorBackground = 16842849, // 0x1010061
	    panelColorForeground = 16842848, // 0x1010060
	    panelFullBackground = 16842847, // 0x101005f
	    panelTextAppearance = 16842850, // 0x1010062
	    parentActivityName = 16843687, // 0x10103a7
	    password = 16843100, // 0x101015c
	    path = 16842794, // 0x101002a
	    pathPattern = 16842796, // 0x101002c
	    pathPrefix = 16842795, // 0x101002b
	    permission = 16842758, // 0x1010006
	    permissionGroup = 16842762, // 0x101000a
	    persistent = 16842765, // 0x101000d
	    persistentDrawingCache = 16842990, // 0x10100ee
	    phoneNumber = 16843111, // 0x1010167
	    pivotX = 16843189, // 0x10101b5
	    pivotY = 16843190, // 0x10101b6
	    popupAnimationStyle = 16843465, // 0x10102c9
	    popupBackground = 16843126, // 0x1010176
	    popupCharacters = 16843332, // 0x1010244
	    popupKeyboard = 16843331, // 0x1010243
	    popupLayout = 16843323, // 0x101023b
	    popupMenuStyle = 16843520, // 0x1010300
	    popupWindowStyle = 16842870, // 0x1010076
	    port = 16842793, // 0x1010029
	    positiveButtonText = 16843253, // 0x10101f5
	    preferenceCategoryStyle = 16842892, // 0x101008c
	    preferenceInformationStyle = 16842893, // 0x101008d
	    preferenceLayoutChild = 16842900, // 0x1010094
	    preferenceScreenStyle = 16842891, // 0x101008b
	    preferenceStyle = 16842894, // 0x101008e
	    previewImage = 16843482, // 0x10102da
	    priority = 16842780, // 0x101001c
	    privateImeOptions = 16843299, // 0x1010223
	    process = 16842769, // 0x1010011
	    progress = 16843063, // 0x1010137
	    progressBarPadding = 16843545, // 0x1010319
	    progressBarStyle = 16842871, // 0x1010077
	    progressBarStyleHorizontal = 16842872, // 0x1010078
	    progressBarStyleInverse = 16843399, // 0x1010287
	    progressBarStyleLarge = 16842874, // 0x101007a
	    progressBarStyleLargeInverse = 16843401, // 0x1010289
	    progressBarStyleSmall = 16842873, // 0x1010079
	    progressBarStyleSmallInverse = 16843400, // 0x1010288
	    progressBarStyleSmallTitle = 16843279, // 0x101020f
	    progressDrawable = 16843068, // 0x101013c
	    prompt = 16843131, // 0x101017b
	    propertyName = 16843489, // 0x10102e1
	    protectionLevel = 16842761, // 0x1010009
	    publicKey = 16843686, // 0x10103a6
	    queryActionMsg = 16843227, // 0x10101db
	    queryAfterZeroResults = 16843394, // 0x1010282
	    queryHint = 16843608, // 0x1010358
	    quickContactBadgeStyleSmallWindowLarge = 16843443, // 0x10102b3
	    quickContactBadgeStyleSmallWindowMedium = 16843442, // 0x10102b2
	    quickContactBadgeStyleSmallWindowSmall = 16843441, // 0x10102b1
	    quickContactBadgeStyleWindowLarge = 16843440, // 0x10102b0
	    quickContactBadgeStyleWindowMedium = 16843439, // 0x10102af
	    quickContactBadgeStyleWindowSmall = 16843438, // 0x10102ae
	    radioButtonStyle = 16842878, // 0x101007e
	    radius = 16843176, // 0x10101a8
	    rating = 16843077, // 0x1010145
	    ratingBarStyle = 16842876, // 0x101007c
	    ratingBarStyleIndicator = 16843280, // 0x1010210
	    ratingBarStyleSmall = 16842877, // 0x101007d
	    readPermission = 16842759, // 0x1010007
	    repeatCount = 16843199, // 0x10101bf
	    repeatMode = 16843200, // 0x10101c0
	    reqFiveWayNav = 16843314, // 0x1010232
	    reqHardKeyboard = 16843305, // 0x1010229
	    reqKeyboardType = 16843304, // 0x1010228
	    reqNavigation = 16843306, // 0x101022a
	    reqTouchScreen = 16843303, // 0x1010227
	    required = 16843406, // 0x101028e
	    requiresFadingEdge = 16843685, // 0x10103a5
	    requiresSmallestWidthDp = 16843620, // 0x1010364
	    resizeMode = 16843619, // 0x1010363
	    resizeable = 16843405, // 0x101028d
	    resource = 16842789, // 0x1010025
	    restoreAnyVersion = 16843450, // 0x10102ba
	    restoreNeedsApplication = 16843421, // 0x101029d
	    right = 16843183, // 0x10101af
	    ringtonePreferenceStyle = 16842899, // 0x1010093
	    ringtoneType = 16843257, // 0x10101f9
	    rotation = 16843558, // 0x1010326
	    rotationX = 16843559, // 0x1010327
	    rotationY = 16843560, // 0x1010328
	    rowCount = 16843637, // 0x1010375
	    rowDelay = 16843216, // 0x10101d0
	    rowEdgeFlags = 16843329, // 0x1010241
	    rowHeight = 16843058, // 0x1010132
	    rowOrderPreserved = 16843638, // 0x1010376
	    saveEnabled = 16842983, // 0x10100e7
	    scaleGravity = 16843262, // 0x10101fe
	    scaleHeight = 16843261, // 0x10101fd
	    scaleType = 16843037, // 0x101011d
	    scaleWidth = 16843260, // 0x10101fc
	    scaleX = 16843556, // 0x1010324
	    scaleY = 16843557, // 0x1010325
	    scheme = 16842791, // 0x1010027
	    screenDensity = 16843467, // 0x10102cb
	    screenOrientation = 16842782, // 0x101001e
	    screenSize = 16843466, // 0x10102ca
	    scrollHorizontally = 16843099, // 0x101015b
	    scrollViewStyle = 16842880, // 0x1010080
	    scrollX = 16842962, // 0x10100d2
	    scrollY = 16842963, // 0x10100d3
	    scrollbarAlwaysDrawHorizontalTrack = 16842856, // 0x1010068
	    scrollbarAlwaysDrawVerticalTrack = 16842857, // 0x1010069
	    scrollbarDefaultDelayBeforeFade = 16843433, // 0x10102a9
	    scrollbarFadeDuration = 16843432, // 0x10102a8
	    scrollbarSize = 16842851, // 0x1010063
	    scrollbarStyle = 16842879, // 0x101007f
	    scrollbarThumbHorizontal = 16842852, // 0x1010064
	    scrollbarThumbVertical = 16842853, // 0x1010065
	    scrollbarTrackHorizontal = 16842854, // 0x1010066
	    scrollbarTrackVertical = 16842855, // 0x1010067
	    scrollbars = 16842974, // 0x10100de
	    scrollingCache = 16843006, // 0x10100fe
	    searchButtonText = 16843269, // 0x1010205
	    searchMode = 16843221, // 0x10101d5
	    searchSettingsDescription = 16843402, // 0x101028a
	    searchSuggestAuthority = 16843222, // 0x10101d6
	    searchSuggestIntentAction = 16843225, // 0x10101d9
	    searchSuggestIntentData = 16843226, // 0x10101da
	    searchSuggestPath = 16843223, // 0x10101d7
	    searchSuggestSelection = 16843224, // 0x10101d8
	    searchSuggestThreshold = 16843373, // 0x101026d
	    secondaryProgress = 16843064, // 0x1010138
	    seekBarStyle = 16842875, // 0x101007b
	    segmentedButtonStyle = 16843568, // 0x1010330
	    selectAllOnFocus = 16843102, // 0x101015e
	    selectable = 16843238, // 0x10101e6
	    selectableItemBackground = 16843534, // 0x101030e
	    selectedDateVerticalBar = 16843591, // 0x1010347
	    selectedWeekBackgroundColor = 16843586, // 0x1010342
	    settingsActivity = 16843301, // 0x1010225
	    shadowColor = 16843105, // 0x1010161
	    shadowDx = 16843106, // 0x1010162
	    shadowDy = 16843107, // 0x1010163
	    shadowRadius = 16843108, // 0x1010164
	    shape = 16843162, // 0x101019a
	    shareInterpolator = 16843195, // 0x10101bb
	    sharedUserId = 16842763, // 0x101000b
	    sharedUserLabel = 16843361, // 0x1010261
	    shouldDisableView = 16843246, // 0x10101ee
	    showAsAction = 16843481, // 0x10102d9
	    showDefault = 16843258, // 0x10101fa
	    showDividers = 16843561, // 0x1010329
	    showSilent = 16843259, // 0x10101fb
	    showWeekNumber = 16843582, // 0x101033e
	    shownWeekCount = 16843585, // 0x1010341
	    shrinkColumns = 16843082, // 0x101014a
	    singleLine = 16843101, // 0x101015d
	    smallIcon = 16843422, // 0x101029e
	    smallScreens = 16843396, // 0x1010284
	    smoothScrollbar = 16843313, // 0x1010231
	    soundEffectsEnabled = 16843285, // 0x1010215
	    spacing = 16843027, // 0x1010113
	    spinnerDropDownItemStyle = 16842887, // 0x1010087
	    spinnerItemStyle = 16842889, // 0x1010089
	    spinnerMode = 16843505, // 0x10102f1
	    spinnerStyle = 16842881, // 0x1010081
	    spinnersShown = 16843595, // 0x101034b
	    splitMotionEvents = 16843503, // 0x10102ef
	    src = 16843033, // 0x1010119
	    stackFromBottom = 16843005, // 0x10100fd
	    starStyle = 16842882, // 0x1010082
	    startColor = 16843165, // 0x101019d
	    startOffset = 16843198, // 0x10101be
	    startYear = 16843132, // 0x101017c
	    stateNotNeeded = 16842774, // 0x1010016
	    state_above_anchor = 16842922, // 0x10100aa
	    state_accelerated = 16843547, // 0x101031b
	    state_activated = 16843518, // 0x10102fe
	    state_active = 16842914, // 0x10100a2
	    state_checkable = 16842911, // 0x101009f
	    state_checked = 16842912, // 0x10100a0
	    state_drag_can_accept = 16843624, // 0x1010368
	    state_drag_hovered = 16843625, // 0x1010369
	    state_empty = 16842921, // 0x10100a9
	    state_enabled = 16842910, // 0x101009e
	    state_expanded = 16842920, // 0x10100a8
	    state_first = 16842916, // 0x10100a4
	    state_focused = 16842908, // 0x101009c
	    state_hovered = 16843623, // 0x1010367
	    state_last = 16842918, // 0x10100a6
	    state_long_pressable = 16843324, // 0x101023c
	    state_middle = 16842917, // 0x10100a5
	    state_multiline = 16843597, // 0x101034d
	    state_pressed = 16842919, // 0x10100a7
	    state_selected = 16842913, // 0x10100a1
	    state_single = 16842915, // 0x10100a3
	    state_window_focused = 16842909, // 0x101009d
	    staticWallpaperPreview = 16843569, // 0x1010331
	    stepSize = 16843078, // 0x1010146
	    stopWithTask = 16843626, // 0x101036a
	    streamType = 16843273, // 0x1010209
	    stretchColumns = 16843081, // 0x1010149
	    stretchMode = 16843030, // 0x1010116
	    subtitle = 16843473, // 0x10102d1
	    subtitleTextStyle = 16843513, // 0x10102f9
	    subtypeExtraValue = 16843674, // 0x101039a
	    subtypeLocale = 16843673, // 0x1010399
	    suggestActionMsg = 16843228, // 0x10101dc
	    suggestActionMsgColumn = 16843229, // 0x10101dd
	    summary = 16843241, // 0x10101e9
	    summaryColumn = 16843426, // 0x10102a2
	    summaryOff = 16843248, // 0x10101f0
	    summaryOn = 16843247, // 0x10101ef
	    supportsRtl = 16843695, // 0x10103af
	    supportsUploading = 16843419, // 0x101029b
	    switchMinWidth = 16843632, // 0x1010370
	    switchPadding = 16843633, // 0x1010371
	    switchPreferenceStyle = 16843629, // 0x101036d
	    switchTextAppearance = 16843630, // 0x101036e
	    switchTextOff = 16843628, // 0x101036c
	    switchTextOn = 16843627, // 0x101036b
	    syncable = 16842777, // 0x1010019
	    tabStripEnabled = 16843453, // 0x10102bd
	    tabStripLeft = 16843451, // 0x10102bb
	    tabStripRight = 16843452, // 0x10102bc
	    tabWidgetStyle = 16842883, // 0x1010083
	    tag = 16842961, // 0x10100d1
	    targetActivity = 16843266, // 0x1010202
	    targetClass = 16842799, // 0x101002f
	    targetDescriptions = 16843680, // 0x10103a0
	    targetPackage = 16842785, // 0x1010021
	    targetSdkVersion = 16843376, // 0x1010270
	    taskAffinity = 16842770, // 0x1010012
	    taskCloseEnterAnimation = 16842942, // 0x10100be
	    taskCloseExitAnimation = 16842943, // 0x10100bf
	    taskOpenEnterAnimation = 16842940, // 0x10100bc
	    taskOpenExitAnimation = 16842941, // 0x10100bd
	    taskToBackEnterAnimation = 16842946, // 0x10100c2
	    taskToBackExitAnimation = 16842947, // 0x10100c3
	    taskToFrontEnterAnimation = 16842944, // 0x10100c0
	    taskToFrontExitAnimation = 16842945, // 0x10100c1
	    tension = 16843370, // 0x101026a
	    testOnly = 16843378, // 0x1010272
	    text = 16843087, // 0x101014f
	    textAlignment = 16843697, // 0x10103b1
	    textAllCaps = 16843660, // 0x101038c
	    textAppearance = 16842804, // 0x1010034
	    textAppearanceButton = 16843271, // 0x1010207
	    textAppearanceInverse = 16842805, // 0x1010035
	    textAppearanceLarge = 16842816, // 0x1010040
	    textAppearanceLargeInverse = 16842819, // 0x1010043
	    textAppearanceLargePopupMenu = 16843521, // 0x1010301
	    textAppearanceListItem = 16843678, // 0x101039e
	    textAppearanceListItemSmall = 16843679, // 0x101039f
	    textAppearanceMedium = 16842817, // 0x1010041
	    textAppearanceMediumInverse = 16842820, // 0x1010044
	    textAppearanceSearchResultSubtitle = 16843424, // 0x10102a0
	    textAppearanceSearchResultTitle = 16843425, // 0x10102a1
	    textAppearanceSmall = 16842818, // 0x1010042
	    textAppearanceSmallInverse = 16842821, // 0x1010045
	    textAppearanceSmallPopupMenu = 16843522, // 0x1010302
	    textCheckMark = 16842822, // 0x1010046
	    textCheckMarkInverse = 16842823, // 0x1010047
	    textColor = 16842904, // 0x1010098
	    textColorAlertDialogListItem = 16843526, // 0x1010306
	    textColorHighlight = 16842905, // 0x1010099
	    textColorHighlightInverse = 16843599, // 0x101034f
	    textColorHint = 16842906, // 0x101009a
	    textColorHintInverse = 16842815, // 0x101003f
	    textColorLink = 16842907, // 0x101009b
	    textColorLinkInverse = 16843600, // 0x1010350
	    textColorPrimary = 16842806, // 0x1010036
	    textColorPrimaryDisableOnly = 16842807, // 0x1010037
	    textColorPrimaryInverse = 16842809, // 0x1010039
	    textColorPrimaryInverseDisableOnly = 16843403, // 0x101028b
	    textColorPrimaryInverseNoDisable = 16842813, // 0x101003d
	    textColorPrimaryNoDisable = 16842811, // 0x101003b
	    textColorSecondary = 16842808, // 0x1010038
	    textColorSecondaryInverse = 16842810, // 0x101003a
	    textColorSecondaryInverseNoDisable = 16842814, // 0x101003e
	    textColorSecondaryNoDisable = 16842812, // 0x101003c
	    textColorTertiary = 16843282, // 0x1010212
	    textColorTertiaryInverse = 16843283, // 0x1010213
	    textCursorDrawable = 16843618, // 0x1010362
	    textDirection = 16843696, // 0x10103b0
	    textEditNoPasteWindowLayout = 16843541, // 0x1010315
	    textEditPasteWindowLayout = 16843540, // 0x1010314
	    textEditSideNoPasteWindowLayout = 16843615, // 0x101035f
	    textEditSidePasteWindowLayout = 16843614, // 0x101035e
	    textEditSuggestionItemLayout = 16843636, // 0x1010374
	    textFilterEnabled = 16843007, // 0x10100ff
	    textIsSelectable = 16843542, // 0x1010316
	    textOff = 16843045, // 0x1010125
	    textOn = 16843044, // 0x1010124
	    textScaleX = 16843089, // 0x1010151
	    textSelectHandle = 16843463, // 0x10102c7
	    textSelectHandleLeft = 16843461, // 0x10102c5
	    textSelectHandleRight = 16843462, // 0x10102c6
	    textSelectHandleWindowStyle = 16843464, // 0x10102c8
	    textSize = 16842901, // 0x1010095
	    textStyle = 16842903, // 0x1010097
	    textSuggestionsWindowStyle = 16843635, // 0x1010373
	    textViewStyle = 16842884, // 0x1010084
	    theme = 16842752, // 0x1010000
	    thickness = 16843360, // 0x1010260
	    thicknessRatio = 16843164, // 0x101019c
	    thumb = 16843074, // 0x1010142
	    thumbOffset = 16843075, // 0x1010143
	    thumbTextPadding = 16843634, // 0x1010372
	    thumbnail = 16843429, // 0x10102a5
	    tileMode = 16843265, // 0x1010201
	    tint = 16843041, // 0x1010121
	    title = 16843233, // 0x10101e1
	    titleCondensed = 16843234, // 0x10101e2
	    titleTextStyle = 16843512, // 0x10102f8
	    toAlpha = 16843211, // 0x10101cb
	    toDegrees = 16843188, // 0x10101b4
	    toXDelta = 16843207, // 0x10101c7
	    toXScale = 16843203, // 0x10101c3
	    toYDelta = 16843209, // 0x10101c9
	    toYScale = 16843205, // 0x10101c5
	    top = 16843182, // 0x10101ae
	    topBright = 16842955, // 0x10100cb
	    topDark = 16842951, // 0x10100c7
	    topLeftRadius = 16843177, // 0x10101a9
	    topOffset = 16843352, // 0x1010258
	    topRightRadius = 16843178, // 0x10101aa
	    track = 16843631, // 0x101036f
	    transcriptMode = 16843008, // 0x1010100
	    transformPivotX = 16843552, // 0x1010320
	    transformPivotY = 16843553, // 0x1010321
	    translationX = 16843554, // 0x1010322
	    translationY = 16843555, // 0x1010323
	    type = 16843169, // 0x10101a1
	    typeface = 16842902, // 0x1010096
	    uiOptions = 16843672, // 0x1010398
	    uncertainGestureColor = 16843382, // 0x1010276
	    unfocusedMonthDateColor = 16843588, // 0x1010344
	    unselectedAlpha = 16843278, // 0x101020e
	    updatePeriodMillis = 16843344, // 0x1010250
	    useDefaultMargins = 16843641, // 0x1010379
	    useIntrinsicSizeAsMinimum = 16843536, // 0x1010310
	    useLevel = 16843167, // 0x101019f
	    userVisible = 16843409, // 0x1010291
	    value = 16842788, // 0x1010024
	    valueFrom = 16843486, // 0x10102de
	    valueTo = 16843487, // 0x10102df
	    valueType = 16843488, // 0x10102e0
	    variablePadding = 16843157, // 0x1010195
	    versionCode = 16843291, // 0x101021b
	    versionName = 16843292, // 0x101021c
	    verticalCorrection = 16843322, // 0x101023a
	    verticalDivider = 16843054, // 0x101012e
	    verticalGap = 16843328, // 0x1010240
	    verticalScrollbarPosition = 16843572, // 0x1010334
	    verticalSpacing = 16843029, // 0x1010115
	    visibility = 16842972, // 0x10100dc
	    visible = 16843156, // 0x1010194
	    vmSafeMode = 16843448, // 0x10102b8
	    voiceLanguage = 16843349, // 0x1010255
	    voiceLanguageModel = 16843347, // 0x1010253
	    voiceMaxResults = 16843350, // 0x1010256
	    voicePromptText = 16843348, // 0x1010254
	    voiceSearchMode = 16843346, // 0x1010252
	    wallpaperCloseEnterAnimation = 16843413, // 0x1010295
	    wallpaperCloseExitAnimation = 16843414, // 0x1010296
	    wallpaperIntraCloseEnterAnimation = 16843417, // 0x1010299
	    wallpaperIntraCloseExitAnimation = 16843418, // 0x101029a
	    wallpaperIntraOpenEnterAnimation = 16843415, // 0x1010297
	    wallpaperIntraOpenExitAnimation = 16843416, // 0x1010298
	    wallpaperOpenEnterAnimation = 16843411, // 0x1010293
	    wallpaperOpenExitAnimation = 16843412, // 0x1010294
	    webTextViewStyle = 16843449, // 0x10102b9
	    webViewStyle = 16842885, // 0x1010085
	    weekDayTextAppearance = 16843592, // 0x1010348
	    weekNumberColor = 16843589, // 0x1010345
	    weekSeparatorLineColor = 16843590, // 0x1010346
	    weightSum = 16843048, // 0x1010128
	    widgetLayout = 16843243, // 0x10101eb
	    width = 16843097, // 0x1010159
	    windowActionBar = 16843469, // 0x10102cd
	    windowActionBarOverlay = 16843492, // 0x10102e4
	    windowActionModeOverlay = 16843485, // 0x10102dd
	    windowAnimationStyle = 16842926, // 0x10100ae
	    windowBackground = 16842836, // 0x1010054
	    windowCloseOnTouchOutside = 16843611, // 0x101035b
	    windowContentOverlay = 16842841, // 0x1010059
	    windowDisablePreview = 16843298, // 0x1010222
	    windowEnableSplitTouch = 16843543, // 0x1010317
	    windowEnterAnimation = 16842932, // 0x10100b4
	    windowExitAnimation = 16842933, // 0x10100b5
	    windowFrame = 16842837, // 0x1010055
	    windowFullscreen = 16843277, // 0x101020d
	    windowHideAnimation = 16842935, // 0x10100b7
	    windowIsFloating = 16842839, // 0x1010057
	    windowIsTranslucent = 16842840, // 0x1010058
	    windowMinWidthMajor = 16843606, // 0x1010356
	    windowMinWidthMinor = 16843607, // 0x1010357
	    windowNoDisplay = 16843294, // 0x101021e
	    windowNoTitle = 16842838, // 0x1010056
	    windowShowAnimation = 16842934, // 0x10100b6
	    windowShowWallpaper = 16843410, // 0x1010292
	    windowSoftInputMode = 16843307, // 0x101022b
	    windowTitleBackgroundStyle = 16842844, // 0x101005c
	    windowTitleSize = 16842842, // 0x101005a
	    windowTitleStyle = 16842843, // 0x101005b
	    writePermission = 16842760, // 0x1010008
	    x = 16842924, // 0x10100ac
	    xlargeScreens = 16843455, // 0x10102bf
	    y = 16842925, // 0x10100ad
	    yesNoPreferenceStyle = 16842896, // 0x1010090
	    zAdjustment = 16843201, // 0x10101c1
    }

    public class ApkReader : AppPackageReader {
        private ZipFile zip;
        private byte[] resources;
        private byte[] manifest;

        private const string AndroidManifextXML = @"androidmanifest.xml";
        private const string Resources_arsc = @"resources.arsc";
        private const string TagApplication = @"application";
        private const string TagManifest = @"manifest";
        private const string AttrLabel = @"label";
        private const string AttrVersionName = @"versionName";
        private const string AttrVersionCode = @"versionCode";
        private const string AttrIcon = @"icon";
        private const string AttrPackage = @"package";
        //private const string AttrName = @"name";
        private const int ConfigurationDensityPosition = 10;

        private byte[] use_config = null;

        /// <summary>
        /// extract the manifext
        /// </summary>
        /// <param name="filename">full path to the file</param>
        /// <param name="culture"></param>
        public ApkReader(string filename, string culture = "") {
            FileName = filename;
            openStream(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public ApkReader(Stream stream, string culture = "") {
            Log("Opening apk from stream");
            openStream(stream);
        }

        private void openStream(Stream stream) {
            zip = new ZipFile(stream);
            ZipEntry en = zip.GetEntry(AndroidManifextXML);
            BinaryReader s = new BinaryReader(zip.GetInputStream(en));
            manifest = s.ReadBytes((int)en.Size);

            en = zip.GetEntry(Resources_arsc);
            s = new BinaryReader(zip.GetInputStream(en));
            resources = s.ReadBytes((int)en.Size);
        }

        public override AppPackageReader.AppType Type {
            get {
                return AppType.AndroidApp;
            }
        }

        public override string AppName {
            get {
                return getAttribute(TagApplication, AttrLabel);
            }
        }

        public override string Version {
            get {
                return getAttribute(TagManifest, AttrVersionName);
            }
        }
        public override string Revision {
            get {
                return getAttribute(TagManifest, AttrVersionCode);
            }
        }

        public override string Publisher {
            get {
                string[] names = PackageName.Split(new char[] {'.'});
                string[] Slice = new List<string>(names).GetRange(0, names.Length-1).ToArray();
                return string.Join(".", Slice);
            }
        }

        public override Bitmap Icon {
            get {
                return getImage(TagApplication, AttrIcon);
            }
        }

        public override string PackageName {
            get {
                return getAttribute(TagManifest, AttrPackage);
            }
        }

        /// <summary>
        /// get a string from manifest.xml
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public string getAttribute(string tag, string attr) {
            return QuickSearchManifestXml(tag, attr);
        }

        /// <summary>
        /// Get a Image object from manifest and resources
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public Bitmap getImage(string tag, string attr) {
            // get the biggest density config
            List<byte[]> configs = getResourceConfigs();
            int bestDesityIndex = 0;
            int bestDensity = configs[0][ConfigurationDensityPosition] + configs[0][ConfigurationDensityPosition + 1] * 256;
            for (int i = 1; i < configs.Count; i++) {
                int density = configs[i][ConfigurationDensityPosition] + configs[i][ConfigurationDensityPosition + 1] * 256;
                if (density > bestDensity) {
                    bestDesityIndex = i;
                    bestDensity = density;
                }
            }
            use_config = configs[bestDesityIndex];
            ZipEntry en = zip.GetEntry(QuickSearchManifestXml(tag, attr));
            use_config = null;
            if (en != null) {
                try {
                    return (Bitmap)Bitmap.FromStream(zip.GetInputStream(en));
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        private uint imageSize;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public void setImageSize(uint size) {
            imageSize = size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private string QuickSearchManifestXml(string tag, string attribute) {
            using (MemoryStream ms = new MemoryStream(manifest))
            using (BinaryReader br = new BinaryReader(ms)) {
                ms.Seek(8, SeekOrigin.Begin); // skip header

                long stringPoolPos = ms.Position;
                ms.Seek(4, SeekOrigin.Current);
                ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // skip string pool chunk

                long resourceMapPos = ms.Position;
                ms.Seek(4, SeekOrigin.Current);
                ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // skip resourceMap

                ms.Seek(4, SeekOrigin.Current);
                ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // skip StartNamespaceChunk

                // XML_START_ELEMENT CHUNK
                while (true) {
                    long chunkPos = ms.Position;
                    short chunkType = br.ReadInt16();
                    short headerSize = br.ReadInt16();
                    int chunkSize = br.ReadInt32();
                    if (chunkType != (short)RES_TYPE.RES_XML_START_ELEMENT_TYPE) {
                        ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin); // skip current chunk
                        continue;
                    }
                    ms.Seek(8 + 4, SeekOrigin.Current); // skip line number & comment / namespace
                    string tag_s = QuickSearchManifestStringPool(br.ReadUInt32());
                    if (tag_s.ToUpper() != tag.ToUpper()) {
                        ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);// to next tag
                        continue;
                    }
                    int attributeStart = br.ReadInt16();
                    int attributeSize = br.ReadInt16();
                    int attributeCount = br.ReadInt16();
                    for (int i = 0; i < attributeSize; i++) {
                        int offset = headerSize + attributeStart + attributeSize * i + 4;
                        if (offset >= chunkSize) { // Error: comes to out of chunk
                            return null;
                        }
                        ms.Seek(chunkPos + offset, SeekOrigin.Begin); // ignore the ns                            
                        uint ind = br.ReadUInt32();
                        string name = QuickSearchManifestStringPool(ind);
                        if (name == "")
                            name = QuickSearchManifestResMap(ind);
                        if (name.ToUpper() == attribute.ToUpper()) {
                            ms.Seek(4 + 2 + 1, SeekOrigin.Current); // skip rawValue/size/0/
                            byte dataType = br.ReadByte();
                            uint data = br.ReadUInt32();
                            if (dataType == (byte)DATA_TYPE.TYPE_STRING) {
                                return QuickSearchManifestStringPool(data);
                            } else if (dataType == (byte)DATA_TYPE.TYPE_REFERENCE) {
                                return QuickSearchResource((UInt32)data,use_config);
                            } else { // I would like to expect we only will recieve TYPE_STRING/TYPE_REFERENCE/any integer type, complex is not considering here,yet
                                return data.ToString();
                            }
                        }
                    }
                    return null;
                }
            }

        }

        /// <summary>
        /// Search in Manifest string pool
        /// </summary>
        /// <param name="stringID"></param>
        /// <returns></returns>
        private string QuickSearchManifestStringPool(uint stringID) {
            using (MemoryStream ms = new MemoryStream(manifest))
            using (BinaryReader br = new BinaryReader(ms)) {
                // the first chunk is always stringpool for manifest and resources
                ms.Seek(2, SeekOrigin.Begin);
                short headerSize = br.ReadInt16();
                ms.Seek(headerSize, SeekOrigin.Begin);
                return QuickSearchStringPool(ms, stringID);
            }

        }

        private string QuickSearchManifestResMap(uint stringID) {
            using (MemoryStream ms = new MemoryStream(manifest))
            using (BinaryReader br = new BinaryReader(ms)) {
                ms.Seek(8, SeekOrigin.Begin); // skip header

                long stringPoolPos = ms.Position;
                ms.Seek(4, SeekOrigin.Current);
                ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // skip string pool chunk

                //Resource map
                ms.Seek(2+2+4 + stringID * 4, SeekOrigin.Current);
                return Enum.GetName(typeof(RDefaultString),br.ReadUInt32());
            }
        }

        /// <summary>
        /// Search in Resources string pool
        /// </summary>
        /// <param name="stringID"></param>
        /// <returns></returns>
        private string QuickSearchResourcesStringPool(uint stringID) {
            using (MemoryStream ms = new MemoryStream(resources))
            using (BinaryReader br = new BinaryReader(ms)) {
                // the first chunk is always stringpool for manifest and resources
                ms.Seek(2, SeekOrigin.Begin);
                short headerSize = br.ReadInt16();
                ms.Seek(headerSize, SeekOrigin.Begin);
                return QuickSearchStringPool(ms, stringID);
            }

        }

        /// <summary>
        /// Search a string pool within existing stream, the stream need to be at the 
        /// start of string pool
        /// </summary>
        /// <param name="ms">Stream, need to be at start of stringPool</param>
        /// <param name="stringID"></param>
        /// <returns></returns>
        private string QuickSearchStringPool(MemoryStream ms, uint stringID) {
            using (BinaryReader br = new BinaryReader(ms)) {
                long poolPos = ms.Position; // record start of pool
                ms.Seek(8 + 4 + 4, SeekOrigin.Current); //skip the header/stringCount/styleCount

                // comes to the start of string pool chunk body, 
                int flags = br.ReadInt32();
                bool isUTF_8 = (flags & (1 << 8)) != 0;
                int stringStart = br.ReadInt32();
                ms.Seek(4, SeekOrigin.Current);
                ms.Seek(stringID * 4, SeekOrigin.Current);
                int stringPos = br.ReadInt32();
                ms.Seek(poolPos + stringStart + stringPos, SeekOrigin.Begin);
                if (isUTF_8) {
                    int u16len = br.ReadByte(); // u16len
                    if ((u16len & 0x80) != 0) {// larger than 128
                        u16len = ((u16len & 0x7F) << 8) + br.ReadByte();
                    }

                    int u8len = br.ReadByte(); // u8len
                    if ((u8len & 0x80) != 0) {// larger than 128
                        u8len = ((u8len & 0x7F) << 8) + br.ReadByte();
                    }
                    return Encoding.UTF8.GetString(br.ReadBytes(u8len));
                } else // UTF_16
                {
                    int u16len = br.ReadUInt16();
                    if ((u16len & 0x8000) != 0) {// larger than 32768
                        u16len = ((u16len & 0x7FFF) << 16) + br.ReadUInt16();
                    }

                    return Encoding.Unicode.GetString(br.ReadBytes(u16len * 2));
                }
            }
        }

        /// <summary>
        /// Get culture info from string
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private CultureInfo getCulture(byte[] code) {
            string language;
            string country;
            byte[] decode;
            if (code[1] > 0x80) { // ISO-639-2
                decode = new byte[3];
                decode[0] = (byte)(code[0] & 0x1F);
                decode[1] = (byte)(((code[1] & 0x3) << 3) + (code[0] & 0xE0) >> 1);
                decode[2] = (byte)((code[1] & 0x7C) >> 2);
            } else { //ISO-639-1
                decode = new byte[2];
                decode[0] = code[0];
                decode[1] = code[1];
            }
            language = System.Text.Encoding.ASCII.GetString(decode);
            decode = new byte[2];
            if (code[3] > 0x80) {
                decode[0] = (byte)(code[2] & 0x1F);
                decode[1] = (byte)(((code[3] & 0x3) << 3) + (code[2] & 0xE0) >> 1);
            } else {
                decode[0] = code[0];
                decode[1] = code[1];
            }
            country = System.Text.Encoding.ASCII.GetString(decode);            
            return new CultureInfo(country + "-" + language);
        }

        /// <summary>
        /// get all configuations
        /// </summary>
        /// <returns>list of configurations</returns>
        private List<byte[]> getResourceConfigs() {
            List<byte[]> result = new List<byte[]>();

            using (MemoryStream ms = new MemoryStream(resources))
            using (BinaryReader br = new BinaryReader(ms)) {
                ms.Seek(8, SeekOrigin.Begin); // jump type/headersize/chunksize
                int packageCount = br.ReadInt32();
                // comes to stringpool chunk, skipit
                long stringPoolPos = ms.Position;
                ms.Seek(4, SeekOrigin.Current);
                int stringPoolSize = br.ReadInt32();
                ms.Seek(stringPoolSize - 8, SeekOrigin.Current); // jump to the end

                //Package chunk now
                for (int pack = 0; pack < packageCount; pack++) {
                    long PackChunkPos = ms.Position;
                    ms.Seek(2, SeekOrigin.Current); // jump type/headersize
                    int headerSize = br.ReadInt16();
                    int PackChunkSize = br.ReadInt32();
                    int packID = br.ReadInt32();

                    ms.Seek(PackChunkPos + headerSize, SeekOrigin.Begin);

                    // skip typestring chunk
                    ms.Seek(4, SeekOrigin.Current);
                    ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end
                    // skip keystring chunk
                    ms.Seek(4, SeekOrigin.Current);
                    ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end

                    do {
                        long chunkPos = ms.Position;
                        short chunkType = br.ReadInt16();
                        headerSize = br.ReadInt16();
                        int chunkSize = br.ReadInt32();
                        if (chunkType == (short)RES_TYPE.RES_TABLE_TYPE_TYPE) {
                            ms.Seek(4 + 4 + 4, SeekOrigin.Current); // skip typeid ,0, entrycount, entrystart

                            // read the config section
                            int config_size = br.ReadInt32();
                            byte[] config = br.ReadBytes(config_size - 4);
                            bool match = false;
                            foreach (byte[] conf in result) {
                                match = true;
                                for (int i = 0; i < conf.Length; i++) {
                                    if (conf[i] != config[i]) {
                                        match = false;
                                        break;
                                    }
                                }
                                if (match)
                                    break;
                            };
                            if (match == false)
                                result.Add(config);
                        }
                        ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin); // skip this chunk
                    } while (ms.Position < PackChunkPos + PackChunkSize);
                }
            }
            return result;
        }

        /// <summary>
        /// Find the requested resource, according to config setting, if the config was set.
        /// This method is NOT HANDLING ANY ERROR, yet!!!!
        /// </summary>
        /// <param name="id">resourceID</param>
        /// <returns>the resource, in string format, if resource id not found, return null value</returns>
        private string QuickSearchResource(UInt32 id, byte[] config = null) {
            uint PackageID = (id & 0xff000000) >> 24;
            uint TypeID = (id & 0x00ff0000) >> 16;
            uint EntryID = (id & 0x0000ffff);

            using (MemoryStream ms = new MemoryStream(resources))
            using (BinaryReader br = new BinaryReader(ms)) {
                ms.Seek(8, SeekOrigin.Begin); // jump type/headersize/chunksize
                int packageCount = br.ReadInt32();
                // comes to stringpool chunk, skipit
                long stringPoolPos = ms.Position;
                ms.Seek(4, SeekOrigin.Current);
                int stringPoolSize = br.ReadInt32();
                ms.Seek(stringPoolSize - 8, SeekOrigin.Current); // jump to the end

                //Package chunk now
                for (int pack = 0; pack < packageCount; pack++) {
                    long PackChunkPos = ms.Position;
                    ms.Seek(2, SeekOrigin.Current); // jump type/headersize
                    int headerSize = br.ReadInt16();
                    int PackChunkSize = br.ReadInt32();
                    int packID = br.ReadInt32();

                    if (packID != PackageID) { // check if the resource is in this pack
                        // goto next chunk
                        ms.Seek(PackChunkPos + PackChunkSize, SeekOrigin.Begin);
                        continue;
                    } else {
                        //ms.Seek(128*2, SeekOrigin.Current); // skip name
                        //int typeStringsPos = br.ReadInt32();
                        //ms.Seek(4,SeekOrigin.Current);    // skip lastpublictype
                        //int keyStringsPos = br.ReadInt32();
                        //ms.Seek(4, SeekOrigin.Current);  // skip lastpublickey
                        ms.Seek(PackChunkPos + headerSize, SeekOrigin.Begin);

                        // skip typestring chunk
                        ms.Seek(4, SeekOrigin.Current);
                        ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end
                        // skip keystring chunk
                        ms.Seek(4, SeekOrigin.Current);
                        ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end

                        // come to typespec chunks and type chunks
                        // typespec and type chunks may happen in a row.
                        do {
                            long chunkPos = ms.Position;
                            short chunkType = br.ReadInt16();
                            headerSize = br.ReadInt16();
                            int chunkSize = br.ReadInt32();
                            byte typeid;
                            //if (chunkType == (short)RES_TYPE.RES_TABLE_TYPE_SPEC_TYPE) {
                            //    typeid = br.ReadByte();
                            //    if (typeid == TypeID) {
                            //        // todo: get the flags
                            //    }
                            //} else 
                            if (chunkType == (short)RES_TYPE.RES_TABLE_TYPE_TYPE) {
                                typeid = br.ReadByte();
                                if (typeid == TypeID) {
                                    ms.Seek(3, SeekOrigin.Current); // skip 0
                                    int entryCount = br.ReadInt32();
                                    int entryStart = br.ReadInt32();

                                    // read the config section
                                    int config_size = br.ReadInt32();
                                    byte[] conf = br.ReadBytes(config_size - 4);

                                    if (config != null) {
                                        bool match = true;
                                        for (int i = 0; i < config.Length; i++) {
                                            if (conf[i] != config[i]) {
                                                match = false;
                                                break;
                                            }
                                        }
                                        if (match == false) {// config does not fit, jump to next chunk
                                            ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);
                                            continue;
                                        }
                                    }

                                    ms.Seek(chunkPos + headerSize + EntryID *4, SeekOrigin.Begin);
                                    //ms.Seek(EntryID * 4, SeekOrigin.Current); // goto index
                                    uint entryIndic = br.ReadUInt32();
                                    if (entryIndic == 0xffffffff) {
                                        ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);
                                        continue; //no entry here, go to next chunk
                                    }
                                    ms.Seek(chunkPos + entryStart + entryIndic, SeekOrigin.Begin);

                                    // get to the entry
                                    ms.Seek(11, SeekOrigin.Current); // skip entry size, flags, key, size, 0
                                    byte dataType = br.ReadByte();
                                    uint data = br.ReadUInt32();
                                    if (config != null)
                                        Log(string.Format("Extracting Resource {0} using density {1}:",id,config[ConfigurationDensityPosition]));
                                    if (dataType == (byte)DATA_TYPE.TYPE_STRING) {
                                        return QuickSearchResourcesStringPool(data);
                                    } else if (dataType == (byte)DATA_TYPE.TYPE_REFERENCE) {
                                        // the entry is null, or it's referencing itself, go to next chunk
                                        if (data == 0x00000000 || data == id) {
                                            ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);
                                            continue;
                                        }
                                        return QuickSearchResource((UInt32)data, config);
                                    } else { // I would like to expect we only will recieve TYPE_STRING/TYPE_REFERENCE/any integer type, complex is not considering here,yet
                                        return data.ToString();
                                    }
                                }
                            //} else {
                            //    // chunk Type is not what we want.
                            }
                            ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin); // skip this chunk
                        } while (ms.Position < PackChunkPos + PackChunkSize);
                        if (config != null) // no config fits, search default
                            return QuickSearchResource(id);
                    }
                }
            }
            return null;
        }

        private bool disposed = false;
        protected override void Dispose(bool disposing) {
            if (disposed) return;
            if (disposing) {
                resources = null;
                manifest = null;
                if (zip != null)
                    zip.Close();
            }
            disposed = true;
            base.Dispose(disposing);
        }

        public void Close() {
            Dispose(true);
        }

        ~ApkReader() {
            Dispose(true);
        }
    }

    /// <summary>
    /// ChunkInfo class
    /// </summary>
    //[DebuggerDisplay("{Type}")]
    //public class ApkChunkInfo : IEnumerable {
    //    public long Offset { get; private set; }
    //    public RES_TYPE Type { get; private set; }
    //    public UInt32 Size;
    //    public List<ApkChunkInfo> subChunks = new List<ApkChunkInfo>();
    //    public ApkChunkInfo parentChunk = null;
    //    public UInt16 headerSize;

    //    public MemoryStream baseStream { get; private set; }

    //    public static ApkChunkInfo FromMemoryStream(MemoryStream stream) {
    //        return new ApkChunkInfo(stream);
    //    }

    //    public void AttachStream(MemoryStream stream) {
    //        if (stream != null) {
    //            baseStream = stream;
    //            baseStream.Seek(Offset, SeekOrigin.Begin);
    //        }
    //    }

    //    private ApkChunkInfo(MemoryStream stream) {
    //        Offset = stream.Position;
    //        AttachStream(stream);
    //        using (BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true)) {

    //            Type = (RES_TYPE)br.ReadUInt16();
    //            headerSize = br.ReadUInt16();
    //            Size = br.ReadUInt32();

    //            // skip whole header
    //            stream.Seek(Offset + headerSize, SeekOrigin.Begin);
    //            // and come to the chunk body

    //            switch (Type) {
    //                case RES_TYPE.RES_TABLE_TYPE:
    //                case RES_TYPE.RES_TABLE_PACKAGE_TYPE:
    //                case RES_TYPE.RES_XML_TYPE:
    //                    // Get subChunks
    //                    while (stream.Position < (Offset + Size)) {
    //                        ApkChunkInfo sub = new ApkChunkInfo(stream);
    //                        sub.parentChunk = this;
    //                        subChunks.Add(sub);
    //                    }
    //                    break;
    //                default:
    //                    stream.Seek(Offset + Size, SeekOrigin.Begin);
    //                    break;
    //            }
    //            if (stream.Position != Offset + Size) {
    //                throw new Exception("Read through the end of chunk, but not match the ChunkSize");
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// find the first chunk fits specific type in sub chunks 
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public ApkChunkInfo findFirstSubChunk(RES_TYPE type) {
    //        foreach (ApkChunkInfo chunk in subChunks) {
    //            if (chunk.Type == type) {
    //                chunk.AttachStream(baseStream);
    //                return chunk;
    //            }
    //        }
    //        return null;
    //    }

    //    /// <summary>
    //    ///  get the next chunk at the same level
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public ApkChunkInfo findNextChunk(RES_TYPE type) {
    //        foreach (ApkChunkInfo chunk in parentChunk.subChunks) {
    //            if (chunk.Offset > Offset && chunk.Type == type) {
    //                return chunk;
    //            }
    //        }
    //        return null;
    //    }

    //    public IEnumerator GetEnumerator() {
    //        throw new NotImplementedException();
    //        //return new ApkChunkEnumerator(BaseStream);
    //    }
    //}
}
