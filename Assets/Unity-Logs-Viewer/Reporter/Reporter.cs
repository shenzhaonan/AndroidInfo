#if UNITY_CHANGE1 || UNITY_CHANGE2 || UNITY_CHANGE3
#warning UNITY_CHANGE has been set manually
#elif UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_CHANGE1
#elif UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define UNITY_CHANGE2
#else
#define UNITY_CHANGE3
#endif
//use UNITY_CHANGE1 for unity older than "unity 5"
//use UNITY_CHANGE2 for unity 5.0 -> 5.3 
//use UNITY_CHANGE3 for unity 5.3 (fix for new SceneManger system  )


using System;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
#if UNITY_CHANGE3
using UnityEngine.SceneManagement;
#endif


[Serializable]
public class Images
{
    public Texture2D ClearImage;
    public Texture2D CollapseImage;
    public Texture2D ClearOnNewSceneImage;
    public Texture2D ShowTimeImage;
    public Texture2D ShowSceneImage;
    public Texture2D UserImage;
    public Texture2D ShowMemoryImage;
    public Texture2D SoftwareImage;
    public Texture2D DateImage;
    public Texture2D ShowFpsImage;
    public Texture2D InfoImage;
    public Texture2D SearchImage;
    public Texture2D CloseImage;

    public Texture2D BuildFromImage;
    public Texture2D SystemInfoImage;
    public Texture2D GraphicsInfoImage;
    public Texture2D BackImage;

    public Texture2D LogImage;
    public Texture2D WarningImage;
    public Texture2D ErrorImage;

    public Texture2D BarImage;
    public Texture2D ButtonActiveImage;
    public Texture2D EvenLogImage;
    public Texture2D OddLogImage;
    public Texture2D SelectedImage;

    public GUISkin ReporterScrollerSkin;
}

//To use Reporter just create reporter from menu (Reporter->Create) at first scene your game start.
//then set the ” Scrip execution order ” in (Edit -> Project Settings ) of Reporter.cs to be the highest.

//Now to view logs all what you have to do is to make a circle gesture using your mouse (click and drag) 
//or your finger (touch and drag) on the screen to show all these logs
//no coding is required 

public class Reporter : MonoBehaviour
{

    public enum LogViewerType
    {
        Assert,
        Error,
        Exception,
        Log,
        Warning
    }

    public class Sample
    {
        public float Time;
        public byte LoadedScene;
        public float Memory;
        public float Fps;
        public string FpsText;
        public static float MemSize()
        {
            float s = sizeof(float) + sizeof(byte) + sizeof(float) + sizeof(float);
            return s;
        }

        public string GetSceneName()
        {
            return scenes[LoadedScene];
        }
    }

    readonly List<Sample> samples = new List<Sample>(60 * 60 * 60);

    public class Log
    {
        public int Count = 1;
        public LogViewerType LogViewerType;
        public string Condition;
        public string Stacktrace;
        public int SampleId;
        //public string   objectName="" ;//object who send error
        //public string   rootName =""; //root of object send error

        public Log CreateCopy()
        {
            return (Log)MemberwiseClone();
        }
        public float GetMemoryUsage()
        {
            return sizeof(int) +
                   sizeof(LogViewerType) +
                   Condition.Length * sizeof(char) +
                   Stacktrace.Length * sizeof(char) +
                   sizeof(int);
        }
    }
    //contains all uncollapsed log
    List<Log> logs = new List<Log>();
    //contains all collapsed logs
    List<Log> collapsedLogs = new List<Log>();
    //contain logs which should only appear to user , for example if you switch off show logs + switch off show warnings
    //and your mode is collapse,then this list will contains only collapsed errors
    List<Log> currentLog = new List<Log>();

    //used to check if the new coming logs is already exist or new one
    MultiKeyDictionary<string, string, Log> logsDic = new MultiKeyDictionary<string, string, Log>();
    //to save memory
    Dictionary<string, string> cachedString = new Dictionary<string, string>();

    [HideInInspector]
    //show hide In Game Logs
    public bool Show;
    //collapse logs
    bool collapse;
    //to decide if you want to clean logs for new loaded scene
    bool clearOnNewSceneLoaded;

    bool showTime;

    bool showScene;

    bool showMemory;

    bool showFps;

    bool showGraph;

    //show or hide logs
    bool showLog = true;
    //show or hide warnings
    bool showWarning = true;
    //show or hide errors
    bool showError = true;

    //total number of logs
    int numOfLogs;
    //total number of warnings
    int numOfLogsWarning;
    //total number of errors
    int numOfLogsError;
    //total number of collapsed logs
    int numOfCollapsedLogs;
    //total number of collapsed warnings
    int numOfCollapsedLogsWarning;
    //total number of collapsed errors
    int numOfCollapsedLogsError;

    //maximum number of allowed logs to view
    //public int maxAllowedLog = 1000 ;

    bool showClearOnNewSceneLoadedButton = true;
    bool showTimeButton = true;
    bool showSceneButton = true;
    bool showMemButton = true;
    bool showFpsButton = true;
    bool showSearchText = true;

    //string buildDate;
    string logDate;
    float logsMemUsage;
    float graphMemUsage;
    public float TotalMemUsage
    {
        get
        {
            return logsMemUsage + graphMemUsage;
        }
    }
    float gcTotalMemory;
    public string UserData = "";
    //frame rate per second
    public float Fps;
    public string FpsText;

    //List<Texture2D> snapshots = new List<Texture2D>() ;

    enum ReportView
    {
        None,
        Logs,
        Info,
        Snapshot,
    }
    ReportView currentView = ReportView.Logs;
    enum DetailView
    {
        None,
        StackTrace,
        Graph,
    }

    //used to check if you have In Game Logs multiple time in different scene
    //only one should work and other should be deleted
    static bool created;
    //public delegate void OnLogHandler( string condition, string stack-trace, LogType type );
    //public event OnLogHandler OnLog ;

    public Images Images;
    // gui
    GUIContent clearContent;
    GUIContent collapseContent;
    GUIContent clearOnNewSceneContent;
    GUIContent showTimeContent;
    GUIContent showSceneContent;
    GUIContent userContent;
    GUIContent showMemoryContent;
    GUIContent softwareContent;
    GUIContent dateContent;
    GUIContent showFpsContent;
    //GUIContent graphContent;
    GUIContent infoContent;
    GUIContent searchContent;
    GUIContent closeContent;

    GUIContent buildFromContent;
    GUIContent systemInfoContent;
    GUIContent graphicsInfoContent;
    GUIContent backContent;

    //GUIContent cameraContent;

    GUIContent logContent;
    GUIContent warningContent;
    GUIContent errorContent;
    GUIStyle barStyle;
    GUIStyle buttonActiveStyle;

    GUIStyle nonStyle;
    GUIStyle lowerLeftFontStyle;
    GUIStyle backStyle;
    GUIStyle evenLogStyle;
    GUIStyle oddLogStyle;
    GUIStyle logButtonStyle;
    GUIStyle selectedLogStyle;
    GUIStyle selectedLogFontStyle;
    GUIStyle stackLabelStyle;
    GUIStyle scrollerStyle;
    GUIStyle searchStyle;
    GUIStyle sliderBackStyle;
    GUIStyle sliderThumbStyle;
    GUISkin toolbarScrollerSkin;
    GUISkin logScrollerSkin;
    GUISkin graphScrollerSkin;

    public Vector2 Size = new Vector2(32, 32);
    public float MaxSize = 20;
    public int NumOfCircleToShow = 10;
    static string[] scenes;
    string currentScene;
    string filterText = "";

    string deviceModel;
    string deviceType;
    string deviceName;
    string graphicsMemorySize;
#if !UNITY_CHANGE1
    string maxTextureSize;
#endif
    string systemMemorySize;

    void Awake()
    {
        if (!Initialized)
            Initialize();
    }

    void OnEnable()
    {
        if (logs.Count == 0)//if recompile while in play mode
            Clear();
    }

    void AddSample()
    {
        Sample sample = new Sample();
        sample.Fps = Fps;
        sample.FpsText = FpsText;
#if UNITY_CHANGE3
        sample.LoadedScene = (byte)SceneManager.GetActiveScene().buildIndex;
#else
		sample.loadedScene = (byte)Application.loadedLevel;
#endif
        sample.Time = Time.realtimeSinceStartup;
        sample.Memory = gcTotalMemory;
        samples.Add(sample);

        graphMemUsage = (samples.Count * Sample.MemSize()) / 1024 / 1024;
    }

    public bool Initialized;
    public void Initialize()
    {
        if (!created)
        {
            try
            {
                gameObject.SendMessage("OnPreStart");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#if UNITY_CHANGE3
            scenes = new string[SceneManager.sceneCountInBuildSettings];
            currentScene = SceneManager.GetActiveScene().name;
#else
			scenes = new string[Application.levelCount];
			currentScene = Application.loadedLevelName;
#endif
            DontDestroyOnLoad(gameObject);
#if UNITY_CHANGE1
			Application.RegisterLogCallback (new Application.LogCallback (CaptureLog));
			Application.RegisterLogCallbackThreaded (new Application.LogCallback (CaptureLogThread));
#else
            //Application.logMessageReceived += CaptureLog ;
            Application.logMessageReceivedThreaded += CaptureLogThread;
#endif
            created = true;
            //addSample();
        }
        else
        {
            Debug.LogWarning("tow manager is exists delete the second");
            DestroyImmediate(gameObject, true);
            return;
        }


        //initialize gui and styles for gui purpose

        clearContent = new GUIContent("", Images.ClearImage, "Clear logs");
        collapseContent = new GUIContent("", Images.CollapseImage, "Collapse logs");
        clearOnNewSceneContent = new GUIContent("", Images.ClearOnNewSceneImage, "Clear logs on new scene loaded");
        showTimeContent = new GUIContent("", Images.ShowTimeImage, "Show Hide Time");
        showSceneContent = new GUIContent("", Images.ShowSceneImage, "Show Hide Scene");
        showMemoryContent = new GUIContent("", Images.ShowMemoryImage, "Show Hide Memory");
        softwareContent = new GUIContent("", Images.SoftwareImage, "Software");
        dateContent = new GUIContent("", Images.DateImage, "Date");
        showFpsContent = new GUIContent("", Images.ShowFpsImage, "Show Hide fps");
        infoContent = new GUIContent("", Images.InfoImage, "Information about application");
        searchContent = new GUIContent("", Images.SearchImage, "Search for logs");
        closeContent = new GUIContent("", Images.CloseImage, "Hide logs");
        userContent = new GUIContent("", Images.UserImage, "User");

        buildFromContent = new GUIContent("", Images.BuildFromImage, "Build From");
        systemInfoContent = new GUIContent("", Images.SystemInfoImage, "System Info");
        graphicsInfoContent = new GUIContent("", Images.GraphicsInfoImage, "Graphics Info");
        backContent = new GUIContent("", Images.BackImage, "Back");


        //snapshotContent = new GUIContent("",images.cameraImage,"show or hide logs");
        logContent = new GUIContent("", Images.LogImage, "show or hide logs");
        warningContent = new GUIContent("", Images.WarningImage, "show or hide warnings");
        errorContent = new GUIContent("", Images.ErrorImage, "show or hide errors");


        currentView = (ReportView)PlayerPrefs.GetInt("Reporter_currentView", 1);
        Show = PlayerPrefs.GetInt("Reporter_show") == 1;
        collapse = PlayerPrefs.GetInt("Reporter_collapse") == 1;
        clearOnNewSceneLoaded = PlayerPrefs.GetInt("Reporter_clearOnNewSceneLoaded") == 1;
        showTime = PlayerPrefs.GetInt("Reporter_showTime") == 1;
        showScene = PlayerPrefs.GetInt("Reporter_showScene") == 1;
        showMemory = PlayerPrefs.GetInt("Reporter_showMemory") == 1;
        showFps = PlayerPrefs.GetInt("Reporter_showFps") == 1;
        showGraph = PlayerPrefs.GetInt("Reporter_showGraph") == 1;
        showLog = PlayerPrefs.GetInt("Reporter_showLog", 1) == 1;
        showWarning = PlayerPrefs.GetInt("Reporter_showWarning", 1) == 1;
        showError = PlayerPrefs.GetInt("Reporter_showError", 1) == 1;
        filterText = PlayerPrefs.GetString("Reporter_filterText");
        Size.x = Size.y = PlayerPrefs.GetFloat("Reporter_size", 32);


        showClearOnNewSceneLoadedButton = PlayerPrefs.GetInt("Reporter_showClearOnNewSceneLoadedButton", 1) == 1;
        showTimeButton = PlayerPrefs.GetInt("Reporter_showTimeButton", 1) == 1;
        showSceneButton = PlayerPrefs.GetInt("Reporter_showSceneButton", 1) == 1;
        showMemButton = PlayerPrefs.GetInt("Reporter_showMemButton", 1) == 1;
        showFpsButton = PlayerPrefs.GetInt("Reporter_showFpsButton", 1) == 1;
        showSearchText = PlayerPrefs.GetInt("Reporter_showSearchText", 1) == 1;


        InitializeStyle();

        Initialized = true;

        if (Show)
        {
            DoShow();
        }

        deviceModel = SystemInfo.deviceModel;
        deviceType = SystemInfo.deviceType.ToString();
        deviceName = SystemInfo.deviceName;
        graphicsMemorySize = SystemInfo.graphicsMemorySize.ToString();
#if !UNITY_CHANGE1
        maxTextureSize = SystemInfo.maxTextureSize.ToString();
#endif
        systemMemorySize = SystemInfo.systemMemorySize.ToString();

    }

    void InitializeStyle()
    {
        int paddingX = (int)(Size.x * 0.2f);
        int paddingY = (int)(Size.y * 0.2f);
        nonStyle = new GUIStyle();
        nonStyle.clipping = TextClipping.Clip;
        nonStyle.border = new RectOffset(0, 0, 0, 0);
        nonStyle.normal.background = null;
        nonStyle.fontSize = (int)(Size.y / 2);
        nonStyle.alignment = TextAnchor.MiddleCenter;

        lowerLeftFontStyle = new GUIStyle();
        lowerLeftFontStyle.clipping = TextClipping.Clip;
        lowerLeftFontStyle.border = new RectOffset(0, 0, 0, 0);
        lowerLeftFontStyle.normal.background = null;
        lowerLeftFontStyle.fontSize = (int)(Size.y / 2);
        lowerLeftFontStyle.fontStyle = FontStyle.Bold;
        lowerLeftFontStyle.alignment = TextAnchor.LowerLeft;


        barStyle = new GUIStyle();
        barStyle.border = new RectOffset(1, 1, 1, 1);
        barStyle.normal.background = Images.BarImage;
        barStyle.active.background = Images.ButtonActiveImage;
        barStyle.alignment = TextAnchor.MiddleCenter;
        barStyle.margin = new RectOffset(1, 1, 1, 1);

        //barStyle.padding = new RectOffset(paddingX,paddingX,paddingY,paddingY); 
        //barStyle.wordWrap = true ;
        barStyle.clipping = TextClipping.Clip;
        barStyle.fontSize = (int)(Size.y / 2);


        buttonActiveStyle = new GUIStyle();
        buttonActiveStyle.border = new RectOffset(1, 1, 1, 1);
        buttonActiveStyle.normal.background = Images.ButtonActiveImage;
        buttonActiveStyle.alignment = TextAnchor.MiddleCenter;
        buttonActiveStyle.margin = new RectOffset(1, 1, 1, 1);
        //buttonActiveStyle.padding = new RectOffset(4,4,4,4);
        buttonActiveStyle.fontSize = (int)(Size.y / 2);

        backStyle = new GUIStyle();
        backStyle.normal.background = Images.EvenLogImage;
        backStyle.clipping = TextClipping.Clip;
        backStyle.fontSize = (int)(Size.y / 2);

        evenLogStyle = new GUIStyle();
        evenLogStyle.normal.background = Images.EvenLogImage;
        evenLogStyle.fixedHeight = Size.y;
        evenLogStyle.clipping = TextClipping.Clip;
        evenLogStyle.alignment = TextAnchor.UpperLeft;
        evenLogStyle.imagePosition = ImagePosition.ImageLeft;
        evenLogStyle.fontSize = (int)(Size.y / 2);
        //evenLogStyle.wordWrap = true;

        oddLogStyle = new GUIStyle();
        oddLogStyle.normal.background = Images.OddLogImage;
        oddLogStyle.fixedHeight = Size.y;
        oddLogStyle.clipping = TextClipping.Clip;
        oddLogStyle.alignment = TextAnchor.UpperLeft;
        oddLogStyle.imagePosition = ImagePosition.ImageLeft;
        oddLogStyle.fontSize = (int)(Size.y / 2);
        //oddLogStyle.wordWrap = true ;

        logButtonStyle = new GUIStyle();
        //logButtonStyle.wordWrap = true;
        logButtonStyle.fixedHeight = Size.y;
        logButtonStyle.clipping = TextClipping.Clip;
        logButtonStyle.alignment = TextAnchor.UpperLeft;
        //logButtonStyle.imagePosition = ImagePosition.ImageLeft ;
        //logButtonStyle.wordWrap = true;
        logButtonStyle.fontSize = (int)(Size.y / 2);
        logButtonStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        selectedLogStyle = new GUIStyle();
        selectedLogStyle.normal.background = Images.SelectedImage;
        selectedLogStyle.fixedHeight = Size.y;
        selectedLogStyle.clipping = TextClipping.Clip;
        selectedLogStyle.alignment = TextAnchor.UpperLeft;
        selectedLogStyle.normal.textColor = Color.white;
        //selectedLogStyle.wordWrap = true;
        selectedLogStyle.fontSize = (int)(Size.y / 2);

        selectedLogFontStyle = new GUIStyle();
        selectedLogFontStyle.normal.background = Images.SelectedImage;
        selectedLogFontStyle.fixedHeight = Size.y;
        selectedLogFontStyle.clipping = TextClipping.Clip;
        selectedLogFontStyle.alignment = TextAnchor.UpperLeft;
        selectedLogFontStyle.normal.textColor = Color.white;
        //selectedLogStyle.wordWrap = true;
        selectedLogFontStyle.fontSize = (int)(Size.y / 2);
        selectedLogFontStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        stackLabelStyle = new GUIStyle();
        stackLabelStyle.wordWrap = true;
        stackLabelStyle.fontSize = (int)(Size.y / 2);
        stackLabelStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        scrollerStyle = new GUIStyle();
        scrollerStyle.normal.background = Images.BarImage;

        searchStyle = new GUIStyle();
        searchStyle.clipping = TextClipping.Clip;
        searchStyle.alignment = TextAnchor.LowerCenter;
        searchStyle.fontSize = (int)(Size.y / 2);
        searchStyle.wordWrap = true;


        sliderBackStyle = new GUIStyle();
        sliderBackStyle.normal.background = Images.BarImage;
        sliderBackStyle.fixedHeight = Size.y;
        sliderBackStyle.border = new RectOffset(1, 1, 1, 1);

        sliderThumbStyle = new GUIStyle();
        sliderThumbStyle.normal.background = Images.SelectedImage;
        sliderThumbStyle.fixedWidth = Size.x;

        GUISkin skin = Images.ReporterScrollerSkin;

        toolbarScrollerSkin = Instantiate(skin);
        toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        toolbarScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        toolbarScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
        toolbarScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

        logScrollerSkin = Instantiate(skin);
        logScrollerSkin.verticalScrollbar.fixedWidth = Size.x * 2f;
        logScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        logScrollerSkin.verticalScrollbarThumb.fixedWidth = Size.x * 2f;
        logScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

        graphScrollerSkin = Instantiate(skin);
        graphScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        graphScrollerSkin.horizontalScrollbar.fixedHeight = Size.x * 2f;
        graphScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
        graphScrollerSkin.horizontalScrollbarThumb.fixedHeight = Size.x * 2f;
        //inGameLogsScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2;
        //inGameLogsScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2;
    }

    private void Start()
    {
        logDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        //StartCoroutine("readInfo");

        SceneManager.sceneLoaded += (InScene, InMode) =>
        {
            if (clearOnNewSceneLoaded)
                Clear();

            currentScene = InScene.name;
            Debug.Log("Scene " + InScene + " is loaded");
        };
    }

    //clear all logs
    void Clear()
    {
        logs.Clear();
        collapsedLogs.Clear();
        currentLog.Clear();
        logsDic.Clear();
        //selectedIndex = -1;
        selectedLog = null;
        numOfLogs = 0;
        numOfLogsWarning = 0;
        numOfLogsError = 0;
        numOfCollapsedLogs = 0;
        numOfCollapsedLogsWarning = 0;
        numOfCollapsedLogsError = 0;
        logsMemUsage = 0;
        graphMemUsage = 0;
        samples.Clear();
        GC.Collect();
        selectedLog = null;
    }

    //private Rect screenRect;
    //private readonly Vector3 tempVector1;
    Rect toolBarRect;
    Rect logsRect;
    Rect stackRect;
    Rect graphRect;
    Rect graphMinRect;
    Rect graphMaxRect;
    Rect buttonRect;
    //private Vector2 stackRectTopLeft;
    //Rect detailRect;

    Vector2 scrollPosition;
    Vector2 scrollPosition2;
    Vector2 toolbarScrollPosition;

    //int 	selectedIndex = -1;
    Log selectedLog;

    float toolbarOldDrag;
    float oldDrag;
    float oldDrag2;
    float oldDrag3;
    int startIndex;

    //calculate what is the currentLog : collapsed or not , hide or view warnings ......
    void CalculateCurrentLog()
    {
        bool filter = !string.IsNullOrEmpty(filterText);
        string lower = string.Empty;
        if (filter)
            lower = filterText.ToLower();
        currentLog.Clear();
        if (collapse)
        {
            for (int i = 0; i < collapsedLogs.Count; i++)
            {
                Log log = collapsedLogs[i];
                if (log.LogViewerType == LogViewerType.Log && !showLog)
                    continue;
                if (log.LogViewerType == LogViewerType.Warning && !showWarning)
                    continue;
                if (log.LogViewerType == LogViewerType.Error && !showError)
                    continue;
                if (log.LogViewerType == LogViewerType.Assert && !showError)
                    continue;
                if (log.LogViewerType == LogViewerType.Exception && !showError)
                    continue;

                if (filter)
                {
                    if (log.Condition.ToLower().Contains(lower))
                        currentLog.Add(log);
                }
                else
                {
                    currentLog.Add(log);
                }
            }
        }
        else
        {
            for (int i = 0; i < logs.Count; i++)
            {
                Log log = logs[i];
                if (log.LogViewerType == LogViewerType.Log && !showLog)
                    continue;
                if (log.LogViewerType == LogViewerType.Warning && !showWarning)
                    continue;
                if (log.LogViewerType == LogViewerType.Error && !showError)
                    continue;
                if (log.LogViewerType == LogViewerType.Assert && !showError)
                    continue;
                if (log.LogViewerType == LogViewerType.Exception && !showError)
                    continue;

                if (filter)
                {
                    if (log.Condition.ToLower().Contains(lower))
                        currentLog.Add(log);
                }
                else
                {
                    currentLog.Add(log);
                }
            }
        }

        if (selectedLog != null)
        {
            int newSelectedIndex = currentLog.IndexOf(selectedLog);
            if (newSelectedIndex == -1)
            {
                Log collapsedSelected = logsDic[selectedLog.Condition][selectedLog.Stacktrace];
                newSelectedIndex = currentLog.IndexOf(collapsedSelected);
                if (newSelectedIndex != -1)
                    scrollPosition.y = newSelectedIndex * Size.y;
            }
            else
            {
                scrollPosition.y = newSelectedIndex * Size.y;
            }
        }
    }

    Rect countRect;
    Rect timeRect;
    Rect timeLabelRect;
    Rect sceneRect;
    Rect sceneLabelRect;
    Rect memoryRect;
    Rect memoryLabelRect;
    Rect fpsRect;
    Rect fpsLabelRect;
    GUIContent tempContent = new GUIContent();


    Vector2 infoScrollPosition;
    Vector2 oldInfoDrag;
    void DrawInfo()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height), backStyle);

        Vector2 drag = GetDrag();
        if ((Math.Abs(drag.x) > float.Epsilon) && (downPos != Vector2.zero))
        {
            infoScrollPosition.x -= (drag.x - oldInfoDrag.x);
        }
        if ((Math.Abs(drag.y) > float.Epsilon) && (downPos != Vector2.zero))
        {
            infoScrollPosition.y += (drag.y - oldInfoDrag.y);
        }
        oldInfoDrag = drag;

        GUI.skin = toolbarScrollerSkin;
        infoScrollPosition = GUILayout.BeginScrollView(infoScrollPosition);
        GUILayout.Space(Size.x);
        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(buildFromContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(/*buildDate*/string.Empty, nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(systemInfoContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(deviceModel, nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(deviceType, nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(deviceName, nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(graphicsInfoContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(SystemInfo.graphicsDeviceName, nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(graphicsMemorySize, nonStyle, GUILayout.Height(Size.y));
#if !UNITY_CHANGE1
        GUILayout.Space(Size.x);
        GUILayout.Label(maxTextureSize, nonStyle, GUILayout.Height(Size.y));
#endif
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Space(Size.x);
        GUILayout.Space(Size.x);
        GUILayout.Label("Screen Width " + Screen.width, nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label("Screen Height " + Screen.height, nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(systemMemorySize + " mb", nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Space(Size.x);
        GUILayout.Space(Size.x);
        GUILayout.Label("Mem Usage Of Logs " + logsMemUsage.ToString("0.000") + " mb", nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        //GUILayout.Label( "Mem Usage Of Graph " + graphMemUsage.ToString("0.000")  + " mb", nonStyle , GUILayout.Height(size.y));
        //GUILayout.Space( size.x);
        GUILayout.Label("GC Memory " + gcTotalMemory.ToString("0.000") + " mb", nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(softwareContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(SystemInfo.operatingSystem, nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(dateContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(DateTime.Now.ToString(CultureInfo.InvariantCulture), nonStyle, GUILayout.Height(Size.y));
        GUILayout.Label(" - Application Started At " + logDate, nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(showTimeContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(Time.realtimeSinceStartup.ToString("000"), nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(FpsText, nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(userContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(UserData, nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(currentScene, nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label("Unity Version = " + Application.unityVersion, nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        /*GUILayout.BeginHorizontal();
		GUILayout.Space( size.x);
		GUILayout.Box( graphContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
		GUILayout.Space( size.x);
		GUILayout.Label( "frame " + samples.Count , nonStyle , GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();*/

        drawInfo_enableDisableToolBarButtons();

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Label("Size = " + Size.x.ToString("0.0"), nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        float sizeY = GUILayout.HorizontalSlider(Size.x, 16, 64, sliderBackStyle, sliderThumbStyle, GUILayout.Width(Screen.width * 0.5f));
        if (Math.Abs(Size.x - sizeY) > float.Epsilon)
        {
            Size.x = Size.y = sizeY;
            InitializeStyle();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        if (GUILayout.Button(backContent, barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            currentView = ReportView.Logs;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();



        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }


    void drawInfo_enableDisableToolBarButtons()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Label("Hide or Show tool bar buttons", nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);

        if (GUILayout.Button(clearOnNewSceneContent, (showClearOnNewSceneLoadedButton) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showClearOnNewSceneLoadedButton = !showClearOnNewSceneLoadedButton;
        }

        if (GUILayout.Button(showTimeContent, (showTimeButton) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showTimeButton = !showTimeButton;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(tempRect, Time.realtimeSinceStartup.ToString("0.0"), lowerLeftFontStyle);
        if (GUILayout.Button(showSceneContent, (showSceneButton) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showSceneButton = !showSceneButton;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(tempRect, currentScene, lowerLeftFontStyle);
        if (GUILayout.Button(showMemoryContent, (showMemButton) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showMemButton = !showMemButton;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(tempRect, gcTotalMemory.ToString("0.0"), lowerLeftFontStyle);

        if (GUILayout.Button(showFpsContent, (showFpsButton) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showFpsButton = !showFpsButton;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(tempRect, FpsText, lowerLeftFontStyle);
        /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
		{
			showGraph = !showGraph ;
		}
		tempRect = GUILayoutUtility.GetLastRect();
		GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/
        if (GUILayout.Button(searchContent, (showSearchText) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showSearchText = !showSearchText;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.TextField(tempRect, filterText, searchStyle);


        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void DrawReport()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height), backStyle);
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        /*GUILayout.Box( cameraContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();*/
        GUILayout.Label("Select Photo", nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Coming Soon", nonStyle, GUILayout.Height(Size.y));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(backContent, barStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y)))
        {
            currentView = ReportView.Logs;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void DrawToolBar()
    {

        toolBarRect = new Rect
        {
            x = 0f,
            y = 0f,
            width = Screen.width,
            height = Size.y * 2f
        };

        //toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        //toolbarScrollerSkin.horizontalScrollbar.fixedHeight= 0f  ;

        GUI.skin = toolbarScrollerSkin;
        Vector2 drag = GetDrag();
        if (Math.Abs(drag.x) > float.Epsilon && downPos != Vector2.zero && downPos.y > Screen.height - Size.y * 2f)
        {
            toolbarScrollPosition.x -= (drag.x - toolbarOldDrag);
        }
        toolbarOldDrag = drag.x;
        GUILayout.BeginArea(toolBarRect);
        toolbarScrollPosition = GUILayout.BeginScrollView(toolbarScrollPosition);
        GUILayout.BeginHorizontal(barStyle);

        if (GUILayout.Button(clearContent, barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            Clear();
        }
        if (GUILayout.Button(collapseContent, (collapse) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            collapse = !collapse;
            CalculateCurrentLog();
        }
        if (showClearOnNewSceneLoadedButton && GUILayout.Button(clearOnNewSceneContent, (clearOnNewSceneLoaded) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            clearOnNewSceneLoaded = !clearOnNewSceneLoaded;
        }

        if (showTimeButton && GUILayout.Button(showTimeContent, (showTime) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showTime = !showTime;
        }
        if (showSceneButton)
        {
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, Time.realtimeSinceStartup.ToString("0.0"), lowerLeftFontStyle);
            if (GUILayout.Button(showSceneContent, (showScene) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            {
                showScene = !showScene;
            }
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, currentScene, lowerLeftFontStyle);
        }
        if (showMemButton)
        {
            if (GUILayout.Button(showMemoryContent, (showMemory) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            {
                showMemory = !showMemory;
            }
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, gcTotalMemory.ToString("0.0"), lowerLeftFontStyle);
        }
        if (showFpsButton)
        {
            if (GUILayout.Button(showFpsContent, (showFps) ? buttonActiveStyle : barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            {
                showFps = !showFps;
            }
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, FpsText, lowerLeftFontStyle);
        }
        /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
		{
			showGraph = !showGraph ;
		}
		tempRect = GUILayoutUtility.GetLastRect();
		GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/

        if (showSearchText)
        {
            GUILayout.Box(searchContent, barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2));
            tempRect = GUILayoutUtility.GetLastRect();
            string newFilterText = GUI.TextField(tempRect, filterText, searchStyle);
            if (newFilterText != filterText)
            {
                filterText = newFilterText;
                CalculateCurrentLog();
            }
        }

        if (GUILayout.Button(infoContent, barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            currentView = ReportView.Info;
        }



        GUILayout.FlexibleSpace();


        string logsText = " ";
        if (collapse)
        {
            logsText += numOfCollapsedLogs;
        }
        else
        {
            logsText += numOfLogs;
        }
        string logsWarningText = " ";
        if (collapse)
        {
            logsWarningText += numOfCollapsedLogsWarning;
        }
        else
        {
            logsWarningText += numOfLogsWarning;
        }
        string logsErrorText = " ";
        if (collapse)
        {
            logsErrorText += numOfCollapsedLogsError;
        }
        else
        {
            logsErrorText += numOfLogsError;
        }

        GUILayout.BeginHorizontal((showLog) ? buttonActiveStyle : barStyle);
        if (GUILayout.Button(logContent, nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showLog = !showLog;
            CalculateCurrentLog();
        }
        if (GUILayout.Button(logsText, nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showLog = !showLog;
            CalculateCurrentLog();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal((showWarning) ? buttonActiveStyle : barStyle);
        if (GUILayout.Button(warningContent, nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showWarning = !showWarning;
            CalculateCurrentLog();
        }
        if (GUILayout.Button(logsWarningText, nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showWarning = !showWarning;
            CalculateCurrentLog();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal((showError) ? buttonActiveStyle : nonStyle);
        if (GUILayout.Button(errorContent, nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showError = !showError;
            CalculateCurrentLog();
        }
        if (GUILayout.Button(logsErrorText, nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            showError = !showError;
            CalculateCurrentLog();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button(closeContent, barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            Show = false;
            ReporterGUI gui = gameObject.GetComponent<ReporterGUI>();
            DestroyImmediate(gui);

            try
            {
                gameObject.SendMessage("OnHideReporter");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }


    Rect tempRect;
    void DrawLogs()
    {

        GUILayout.BeginArea(logsRect, backStyle);

        GUI.skin = logScrollerSkin;
        //setStartPos();
        Vector2 drag = GetDrag();

        if (Math.Abs(drag.y) > float.Epsilon && logsRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y)))
        {
            scrollPosition.y += (drag.y - oldDrag);
        }
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        oldDrag = drag.y;


        int totalVisibleCount = (int)(Screen.height * 0.75f / Size.y);
        int totalCount = currentLog.Count;
        /*if( totalCount < 100 )
			inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 0;
		else 
			inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 64;*/

        totalVisibleCount = Mathf.Min(totalVisibleCount, totalCount - startIndex);
        int index = 0;
        int beforeHeight = (int)(startIndex * Size.y);
        //selectedIndex = Mathf.Clamp( selectedIndex , -1 , totalCount -1);
        if (beforeHeight > 0)
        {
            //fill invisible gap before scroller to make proper scroller pos
            GUILayout.BeginHorizontal(GUILayout.Height(beforeHeight));
            GUILayout.Label("---");
            GUILayout.EndHorizontal();
        }

        int endIndex = startIndex + totalVisibleCount;
        endIndex = Mathf.Clamp(endIndex, 0, totalCount);
        bool scrollerVisible = (totalVisibleCount < totalCount);
        for (int i = startIndex; (startIndex + index) < endIndex; i++)
        {

            if (i >= currentLog.Count)
                break;
            Log log = currentLog[i];

            if (log.LogViewerType == LogViewerType.Log && !showLog)
                continue;
            if (log.LogViewerType == LogViewerType.Warning && !showWarning)
                continue;
            if (log.LogViewerType == LogViewerType.Error && !showError)
                continue;
            if (log.LogViewerType == LogViewerType.Assert && !showError)
                continue;
            if (log.LogViewerType == LogViewerType.Exception && !showError)
                continue;

            if (index >= totalVisibleCount)
            {
                break;
            }

            GUIContent content;
            if (log.LogViewerType == LogViewerType.Log)
                content = logContent;
            else if (log.LogViewerType == LogViewerType.Warning)
                content = warningContent;
            else
                content = errorContent;
            //content.text = log.condition ;

            GUIStyle currentLogStyle = ((startIndex + index) % 2 == 0) ? evenLogStyle : oddLogStyle;
            if (log == selectedLog)
            {
                //selectedLog = log ;
                currentLogStyle = selectedLogStyle;
            }
            else
            {
            }

            tempContent.text = log.Count.ToString();
            float w = 0f;
            if (collapse)
                w = barStyle.CalcSize(tempContent).x + 3;
            countRect = new Rect
            {
                x = Screen.width - w,
                y = Size.y * i,
                width = w,
                height = Size.y
            };
            if (beforeHeight > 0)
                countRect.y += 8;//i will check later why

            if (scrollerVisible)
                countRect.x -= Size.x * 2;

            Sample sample = samples[log.SampleId];
            fpsRect = countRect;
            if (showFps)
            {
                tempContent.text = sample.FpsText;
                w = currentLogStyle.CalcSize(tempContent).x + Size.x;
                fpsRect.x -= w;
                fpsRect.width = Size.x;
                fpsLabelRect = fpsRect;
                fpsLabelRect.x += Size.x;
                fpsLabelRect.width = w - Size.x;
            }


            memoryRect = fpsRect;
            if (showMemory)
            {
                tempContent.text = sample.Memory.ToString("0.000");
                w = currentLogStyle.CalcSize(tempContent).x + Size.x;
                memoryRect.x -= w;
                memoryRect.width = Size.x;
                memoryLabelRect = memoryRect;
                memoryLabelRect.x += Size.x;
                memoryLabelRect.width = w - Size.x;
            }
            sceneRect = memoryRect;
            if (showScene)
            {

                tempContent.text = sample.GetSceneName();
                w = currentLogStyle.CalcSize(tempContent).x + Size.x;
                sceneRect.x -= w;
                sceneRect.width = Size.x;
                sceneLabelRect = sceneRect;
                sceneLabelRect.x += Size.x;
                sceneLabelRect.width = w - Size.x;
            }
            timeRect = sceneRect;
            if (showTime)
            {
                tempContent.text = sample.Time.ToString("0.000");
                w = currentLogStyle.CalcSize(tempContent).x + Size.x;
                timeRect.x -= w;
                timeRect.width = Size.x;
                timeLabelRect = timeRect;
                timeLabelRect.x += Size.x;
                timeLabelRect.width = w - Size.x;
            }



            GUILayout.BeginHorizontal(currentLogStyle);
            if (log == selectedLog)
            {
                GUILayout.Box(content, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
                GUILayout.Label(log.Condition, selectedLogFontStyle);
                //GUILayout.FlexibleSpace();
                if (showTime)
                {
                    GUI.Box(timeRect, showTimeContent, currentLogStyle);
                    GUI.Label(timeLabelRect, sample.Time.ToString("0.000"), currentLogStyle);
                }
                if (showScene)
                {
                    GUI.Box(sceneRect, showSceneContent, currentLogStyle);
                    GUI.Label(sceneLabelRect, sample.GetSceneName(), currentLogStyle);
                }
                if (showMemory)
                {
                    GUI.Box(memoryRect, showMemoryContent, currentLogStyle);
                    GUI.Label(memoryLabelRect, sample.Memory.ToString("0.000") + " mb", currentLogStyle);
                }
                if (showFps)
                {
                    GUI.Box(fpsRect, showFpsContent, currentLogStyle);
                    GUI.Label(fpsLabelRect, sample.FpsText, currentLogStyle);
                }


            }
            else
            {
                if (GUILayout.Button(content, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y)))
                {
                    //selectedIndex = startIndex + index ;
                    selectedLog = log;
                }
                if (GUILayout.Button(log.Condition, logButtonStyle))
                {
                    //selectedIndex = startIndex + index ;
                    selectedLog = log;
                }
                //GUILayout.FlexibleSpace();
                if (showTime)
                {
                    GUI.Box(timeRect, showTimeContent, currentLogStyle);
                    GUI.Label(timeLabelRect, sample.Time.ToString("0.000"), currentLogStyle);
                }
                if (showScene)
                {
                    GUI.Box(sceneRect, showSceneContent, currentLogStyle);
                    GUI.Label(sceneLabelRect, sample.GetSceneName(), currentLogStyle);
                }
                if (showMemory)
                {
                    GUI.Box(memoryRect, showMemoryContent, currentLogStyle);
                    GUI.Label(memoryLabelRect, sample.Memory.ToString("0.000") + " mb", currentLogStyle);
                }
                if (showFps)
                {
                    GUI.Box(fpsRect, showFpsContent, currentLogStyle);
                    GUI.Label(fpsLabelRect, sample.FpsText, currentLogStyle);
                }
            }
            if (collapse)
                GUI.Label(countRect, log.Count.ToString(), barStyle);
            GUILayout.EndHorizontal();
            index++;
        }

        int afterHeight = (int)((totalCount - (startIndex + totalVisibleCount)) * Size.y);
        if (afterHeight > 0)
        {
            //fill invisible gap after scroller to make proper scroller pos
            GUILayout.BeginHorizontal(GUILayout.Height(afterHeight));
            GUILayout.Label(" ");
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        buttonRect = new Rect { x = 0f, y = Screen.height - Size.y, width = Screen.width, height = Size.y };

        if (showGraph)
            DrawGraph();
        else
            DrawStack();
    }


    float graphSize = 4f;
    int startFrame;
    int currentFrame;
    //Vector3 tempVector1;
    //Vector3 tempVector2;
    Vector2 graphScrollerPos;
    float maxFpsValue;
    float minFpsValue;
    float maxMemoryValue;
    float minMemoryValue;

    void DrawGraph()
    {

        graphRect = stackRect;
        graphRect.height = Screen.height * 0.25f;//- size.y ;



        //startFrame = samples.Count - (int)(Screen.width / graphSize) ;
        //if( startFrame < 0 ) startFrame = 0 ;
        GUI.skin = graphScrollerSkin;

        Vector2 drag = GetDrag();
        if (graphRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y)))
        {
            if (Math.Abs(drag.x) > float.Epsilon)
            {
                graphScrollerPos.x -= drag.x - oldDrag3;
                graphScrollerPos.x = Mathf.Max(0, graphScrollerPos.x);
            }

            Vector2 p = downPos;
            if (p != Vector2.zero)
            {
                currentFrame = startFrame + (int)(p.x / graphSize);
            }
        }

        oldDrag3 = drag.x;
        GUILayout.BeginArea(graphRect, backStyle);

        graphScrollerPos = GUILayout.BeginScrollView(graphScrollerPos);
        startFrame = (int)(graphScrollerPos.x / graphSize);
        if (graphScrollerPos.x >= (samples.Count * graphSize - Screen.width))
            graphScrollerPos.x += graphSize;

        GUILayout.Label(" ", GUILayout.Width(samples.Count * graphSize));
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        maxFpsValue = 0;
        minFpsValue = 100000;
        maxMemoryValue = 0;
        minMemoryValue = 100000;
        for (int i = 0; i < Screen.width / graphSize; i++)
        {
            int index = startFrame + i;
            if (index >= samples.Count)
                break;
            Sample s = samples[index];
            if (maxFpsValue < s.Fps) maxFpsValue = s.Fps;
            if (minFpsValue > s.Fps) minFpsValue = s.Fps;
            if (maxMemoryValue < s.Memory) maxMemoryValue = s.Memory;
            if (minMemoryValue > s.Memory) minMemoryValue = s.Memory;
        }

        //GUI.BeginGroup(graphRect);


        if (currentFrame != -1 && currentFrame < samples.Count)
        {
            Sample selectedSample = samples[currentFrame];
            GUILayout.BeginArea(buttonRect, backStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Box(showTimeContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.Time.ToString("0.0"), nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.GetSceneName(), nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.Memory.ToString("0.000"), nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.FpsText, nonStyle);
            GUILayout.Space(Size.x);

            /*GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
			GUILayout.Label( currentFrame.ToString() ,nonStyle  );*/
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        graphMaxRect = stackRect;
        graphMaxRect.height = Size.y;
        GUILayout.BeginArea(graphMaxRect);
        GUILayout.BeginHorizontal();

        GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Label(maxMemoryValue.ToString("0.000"), nonStyle);

        GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Label(maxFpsValue.ToString("0.000"), nonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        graphMinRect = stackRect;
        graphMinRect.y = stackRect.y + stackRect.height - Size.y;
        graphMinRect.height = Size.y;
        GUILayout.BeginArea(graphMinRect);
        GUILayout.BeginHorizontal();

        GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));

        GUILayout.Label(minMemoryValue.ToString("0.000"), nonStyle);


        GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));

        GUILayout.Label(minFpsValue.ToString("0.000"), nonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //GUI.EndGroup();
    }

    void DrawStack()
    {

        if (selectedLog != null)
        {
            Vector2 drag = GetDrag();
            if (Math.Abs(drag.y) > float.Epsilon && stackRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y)))
            {
                scrollPosition2.y += drag.y - oldDrag2;
            }
            oldDrag2 = drag.y;



            GUILayout.BeginArea(stackRect, backStyle);
            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);
            Sample selectedSample = null;
            try
            {
                selectedSample = samples[selectedLog.SampleId];
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(selectedLog.Condition, stackLabelStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(Size.y * 0.25f);
            GUILayout.BeginHorizontal();
            GUILayout.Label(selectedLog.Stacktrace, stackLabelStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(Size.y);
            GUILayout.EndScrollView();
            GUILayout.EndArea();


            GUILayout.BeginArea(buttonRect, backStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Box(showTimeContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.Time.ToString("0.000"), nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.GetSceneName(), nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.Memory.ToString("0.000"), nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.FpsText, nonStyle);
            /*GUILayout.Space( size.x );
			GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
			GUILayout.Label( selectedLog.sampleId.ToString() ,nonStyle  );*/
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();



        }
        else
        {
            GUILayout.BeginArea(stackRect, backStyle);
            GUILayout.EndArea();
            GUILayout.BeginArea(buttonRect, backStyle);
            GUILayout.EndArea();
        }

    }


    public void OnGUIDraw()
    {

        if (!Show)
        {
            return;
        }

        sceneRect = new Rect { x = 0, y = 0, width = Screen.width, height = Screen.height };

        GetDownPos();


        logsRect.x = 0f;
        logsRect.y = Size.y * 2f;
        logsRect.width = Screen.width;
        logsRect.height = Screen.height * 0.75f - Size.y * 2f;

        //stackRectTopLeft.x = 0f;
        stackRect.x = 0f;
        //stackRectTopLeft.y = Screen.height * 0.75f;
        stackRect.y = Screen.height * 0.75f;
        stackRect.width = Screen.width;
        stackRect.height = Screen.height * 0.25f - Size.y;

        //detailRect.x = 0f;
        //detailRect.y = Screen.height - Size.y * 3;
        //detailRect.width = Screen.width;
        //detailRect.height = Size.y * 3;

        if (currentView == ReportView.Info)
            DrawInfo();
        else if (currentView == ReportView.Logs)
        {
            DrawToolBar();
            DrawLogs();
        }


    }

    List<Vector2> gestureDetector = new List<Vector2>();
    Vector2 gestureSum = Vector2.zero;
    float gestureLength;
    int gestureCount;
    bool IsGestureDone()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1)
            {
                gestureDetector.Clear();
                gestureCount = 0;
            }
            else
            {
                if (Input.touches[0].phase == TouchPhase.Canceled || Input.touches[0].phase == TouchPhase.Ended)
                    gestureDetector.Clear();
                else if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    Vector2 p = Input.touches[0].position;
                    if (gestureDetector.Count == 0 || (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
                        gestureDetector.Add(p);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                gestureDetector.Clear();
                gestureCount = 0;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    Vector2 p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (gestureDetector.Count == 0 || (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
                        gestureDetector.Add(p);
                }
            }
        }

        if (gestureDetector.Count < 10)
            return false;

        gestureSum = Vector2.zero;
        gestureLength = 0;
        Vector2 prevDelta = Vector2.zero;
        for (int i = 0; i < gestureDetector.Count - 2; i++)
        {

            Vector2 delta = gestureDetector[i + 1] - gestureDetector[i];
            float deltaLength = delta.magnitude;
            gestureSum += delta;
            gestureLength += deltaLength;

            float dot = Vector2.Dot(delta, prevDelta);
            if (dot < 0f)
            {
                gestureDetector.Clear();
                gestureCount = 0;
                return false;
            }

            prevDelta = delta;
        }

        int gestureBase = (Screen.width + Screen.height) / 4;

        if (gestureLength > gestureBase && gestureSum.magnitude < gestureBase / 2)
        {
            gestureDetector.Clear();
            gestureCount++;
            if (gestureCount >= NumOfCircleToShow)
                return true;
        }

        return false;
    }

    float lastClickTime = -1;
    bool IsDoubleClickDone()
    {
        if (Application.platform == RuntimePlatform.Android ||
           Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1)
            {
                lastClickTime = -1;
            }
            else
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    if (Math.Abs(lastClickTime + 1) < float.Epsilon)
                        lastClickTime = Time.realtimeSinceStartup;
                    else if (Time.realtimeSinceStartup - lastClickTime < 0.2f)
                    {
                        lastClickTime = -1;
                        return true;
                    }
                    else
                    {
                        lastClickTime = Time.realtimeSinceStartup;
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Math.Abs(lastClickTime + 1) < float.Epsilon)
                    lastClickTime = Time.realtimeSinceStartup;
                else if (Time.realtimeSinceStartup - lastClickTime < 0.2f)
                {
                    lastClickTime = -1;
                    return true;
                }
                else
                {
                    lastClickTime = Time.realtimeSinceStartup;
                }
            }
        }
        return false;
    }

    //calculate  pos of first click on screen
    //Vector2 startPos;

    Vector2 downPos;
    void GetDownPos()
    {
        if (Application.platform == RuntimePlatform.Android ||
           Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began)
            {
                downPos = Input.touches[0].position;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                downPos.x = Input.mousePosition.x;
                downPos.y = Input.mousePosition.y;
            }
        }
    }
    //calculate drag amount , this is used for scrolling

    Vector2 mousePosition;
    Vector2 GetDrag()
    {

        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1)
            {
                return Vector2.zero;
            }
            return Input.touches[0].position - downPos;
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                mousePosition = Input.mousePosition;
                return mousePosition - downPos;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

    //calculate the start index of visible log
    void CalculateStartIndex()
    {
        startIndex = (int)(scrollPosition.y / Size.y);
        startIndex = Mathf.Clamp(startIndex, 0, currentLog.Count);
    }

    // For FPS Counter
    private int frames;
    private bool firstTime = true;
    private float lastUpdate;
    private const int RequiredFrames = 10;
    private const float UpdateInterval = 0.25f;

#if UNITY_CHANGE1
	float lastUpdate2 = 0;
#endif

    void DoShow()
    {
        Show = true;
        currentView = ReportView.Logs;
        gameObject.AddComponent<ReporterGUI>();


        try
        {
            gameObject.SendMessage("OnShowReporter");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    void Update()
    {
        FpsText = Fps.ToString("0.000");
        gcTotalMemory = (float)GC.GetTotalMemory(false) / 1024 / 1024;
        //addSample();

#if UNITY_CHANGE3
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex != -1 && string.IsNullOrEmpty(scenes[sceneIndex]))
            scenes[SceneManager.GetActiveScene().buildIndex] = SceneManager.GetActiveScene().name;
#else
		int sceneIndex = Application.loadedLevel;
		if (sceneIndex != -1 && string.IsNullOrEmpty(scenes[Application.loadedLevel]))
			scenes[Application.loadedLevel] = Application.loadedLevelName;
#endif

        CalculateStartIndex();
        if (!Show && IsGestureDone())
        {
            DoShow();
        }


        lock (threadedLogs)
        {
            if (threadedLogs.Count > 0)
            {
                lock (threadedLogs)
                {
                    for (int i = 0; i < threadedLogs.Count; i++)
                    {
                        Log l = threadedLogs[i];
                        AddLog(l.Condition, l.Stacktrace, (LogType)l.LogViewerType);
                    }
                    threadedLogs.Clear();
                }
            }
        }

#if UNITY_CHANGE1
		float elapsed2 = Time.realtimeSinceStartup - lastUpdate2;
		if (elapsed2 > 1) {
			lastUpdate2 = Time.realtimeSinceStartup;
			//be sure no body else take control of log 
			Application.RegisterLogCallback (new Application.LogCallback (CaptureLog));
			Application.RegisterLogCallbackThreaded (new Application.LogCallback (CaptureLogThread));
		}
#endif

        // FPS Counter
        if (firstTime)
        {
            firstTime = false;
            lastUpdate = Time.realtimeSinceStartup;
            frames = 0;
            return;
        }
        frames++;
        float dt = Time.realtimeSinceStartup - lastUpdate;
        if (dt > UpdateInterval && frames > RequiredFrames)
        {
            Fps = frames / dt;
            lastUpdate = Time.realtimeSinceStartup;
            frames = 0;
        }
    }


    void CaptureLog(string InCondition, string InStacktrace, LogType InType)
    {
        AddLog(InCondition, InStacktrace, InType);
    }

    void AddLog(string InCondition, string InStacktrace, LogType InType)
    {
        float memUsage = 0f;
        string condition;
        if (cachedString.ContainsKey(InCondition))
        {
            condition = cachedString[InCondition];
        }
        else
        {
            condition = InCondition;
            cachedString.Add(condition, condition);
            memUsage += string.IsNullOrEmpty(condition) ? 0 : condition.Length * sizeof(char);
            memUsage += IntPtr.Size;
        }
        string stacktrace;
        if (cachedString.ContainsKey(InStacktrace))
        {
            stacktrace = cachedString[InStacktrace];
        }
        else
        {
            stacktrace = InStacktrace;
            cachedString.Add(stacktrace, stacktrace);
            memUsage += string.IsNullOrEmpty(stacktrace) ? 0 : stacktrace.Length * sizeof(char);
            memUsage += IntPtr.Size;
        }
        bool newLogAdded = false;

        AddSample();
        Log log = new Log() { LogViewerType = (LogViewerType)InType, Condition = condition, Stacktrace = stacktrace, SampleId = samples.Count - 1 };
        memUsage += log.GetMemoryUsage();
        //memUsage += samples.Count * 13 ;

        logsMemUsage += memUsage / 1024 / 1024;

        if (TotalMemUsage > MaxSize)
        {
            Clear();
            Debug.Log("Memory Usage Reach" + MaxSize + " mb So It is Cleared");
            return;
        }

        bool isNew = false;
        //string key = _condition;// + "_!_" + _stacktrace ;
        if (logsDic.ContainsKey(condition, InStacktrace))
        {
            logsDic[condition][InStacktrace].Count++;
        }
        else
        {
            isNew = true;
            collapsedLogs.Add(log);
            logsDic[condition][InStacktrace] = log;

            if (InType == LogType.Log)
                numOfCollapsedLogs++;
            else if (InType == LogType.Warning)
                numOfCollapsedLogsWarning++;
            else
                numOfCollapsedLogsError++;
        }

        if (InType == LogType.Log)
            numOfLogs++;
        else if (InType == LogType.Warning)
            numOfLogsWarning++;
        else
            numOfLogsError++;


        logs.Add(log);
        if (!collapse || isNew)
        {
            bool skip = false;
            if (log.LogViewerType == LogViewerType.Log && !showLog)
                skip = true;
            if (log.LogViewerType == LogViewerType.Warning && !showWarning)
                skip = true;
            if (log.LogViewerType == LogViewerType.Error && !showError)
                skip = true;
            if (log.LogViewerType == LogViewerType.Assert && !showError)
                skip = true;
            if (log.LogViewerType == LogViewerType.Exception && !showError)
                skip = true;

            if (!skip)
            {
                if (string.IsNullOrEmpty(filterText) || log.Condition.ToLower().Contains(filterText.ToLower()))
                {
                    currentLog.Add(log);
                    newLogAdded = true;
                }
            }
        }

        if (newLogAdded)
        {
            CalculateStartIndex();
            int totalCount = currentLog.Count;
            int totalVisibleCount = (int)(Screen.height * 0.75f / Size.y);
            if (startIndex >= (totalCount - totalVisibleCount))
                scrollPosition.y += Size.y;
        }

        try
        {
            gameObject.SendMessage("OnLog", log);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    List<Log> threadedLogs = new List<Log>();

    //public Reporter(Rect InScreenRect, Vector3 InTempVector1, Vector3 InTempVector2)
    //{
    //    screenRect = InScreenRect;
    //    tempVector1 = InTempVector1;
    //    //tempVector1 = InTempVector1;
    //    //tempVector2 = InTempVector2;
    //    currentFrame = 0;
    //}

    void CaptureLogThread(string InCondition, string InStacktrace, LogType InType)
    {
        Log log = new Log() { Condition = InCondition, Stacktrace = InStacktrace, LogViewerType = (LogViewerType)InType };
        lock (threadedLogs)
        {
            threadedLogs.Add(log);
        }
    }

    //save user config
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Reporter_currentView", (int)currentView);
        PlayerPrefs.SetInt("Reporter_show", Show  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_collapse", collapse  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_clearOnNewSceneLoaded", clearOnNewSceneLoaded  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showTime", showTime  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showScene", showScene  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showMemory", showMemory  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showFps", showFps  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showGraph", showGraph  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showLog", showLog  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showWarning", showWarning  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showError", showError  ? 1 : 0);
        PlayerPrefs.SetString("Reporter_filterText", filterText);
        PlayerPrefs.SetFloat("Reporter_size", Size.x);

        PlayerPrefs.SetInt("Reporter_showClearOnNewSceneLoadedButton", showClearOnNewSceneLoadedButton  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showTimeButton", showTimeButton  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showSceneButton", showSceneButton  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showMemButton", showMemButton  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showFpsButton", showFpsButton  ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showSearchText", showSearchText  ? 1 : 0);

        PlayerPrefs.Save();
    }

    //read build information 
    //IEnumerator readInfo()
    //{
    //	string prefFile = "build_info.txt";
    //	string url = prefFile;

    //	if (prefFile.IndexOf("://") == -1) {
    //		string streamingAssetsPath = Application.streamingAssetsPath;
    //		if (streamingAssetsPath == "")
    //			streamingAssetsPath = Application.dataPath + "/StreamingAssets/";
    //		url = System.IO.Path.Combine(streamingAssetsPath, prefFile);
    //	}

    //	if (Application.platform != RuntimePlatform.WebGLPlayer && Application.platform != RuntimePlatform.WebGLPlayer)
    //		if (!url.Contains("://"))
    //			url = "file://" + url;


    //	// float startTime = Time.realtimeSinceStartup;
    //	WWW www = new WWW(url);
    //	yield return www;

    //	if (!string.IsNullOrEmpty(www.error)) {
    //		Debug.LogError(www.error);
    //	}
    //	else {
    //		buildDate = www.text;
    //	}

    //	yield break;
    //}
}


