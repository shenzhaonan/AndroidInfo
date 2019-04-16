using System;
using UnityEngine;

namespace Framework.UtilPackage
{
    public sealed class AndroidTools
    {
        private static AndroidTools instance;

        public static AndroidTools Instance
        {
            get { return instance ?? (instance = new AndroidTools()); }
        }

        private AndroidJavaClass unityPlayer, toast;
        private AndroidJavaObject activity, context;
        private int showToastTime;

        private static readonly string isGetDeviceInfo = "IsGetDeviceInfo";

        public static readonly string UNIQUE_ID = "UniqueId";
        public static readonly string COUNTRY = "Country";
        public static readonly string LANGUAGE = "Language";
        public static readonly string OPERATING_SYSTEM = "OperatingSystem";
        public static readonly string MODEL = "Model";
        public static readonly string SCREEN_SIZE = "ScreenSize";
        public static readonly string MEMORY = "Memory";

        public AndroidTools()
        {
            if (PlayerPrefs.GetInt(isGetDeviceInfo, 0) == 0) GetDeviceInfo();
        }

        private void Initialize()
        {
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            context = activity.Call<AndroidJavaObject>("getApplicationContext");
            toast = new AndroidJavaClass("android.widget.Toast");
            showToastTime = toast.GetStatic<int>("LENGTH_SHORT");
        }

        private void GetDeviceInfo()
        {
            if (PlayerPrefs.GetInt(isGetDeviceInfo, 0) == 0)
            {
                try
                {
                    PlayerPrefs.SetString(SCREEN_SIZE, string.Format("{0}x{1}", Screen.width, Screen.height));
                    PlayerPrefs.SetString(MODEL, SystemInfo.deviceModel);
                    PlayerPrefs.SetString(OPERATING_SYSTEM, SystemInfo.operatingSystem);
                    PlayerPrefs.SetString(MEMORY, SystemInfo.systemMemorySize.ToString());

                    if (null == context) Initialize();

                    using (AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure"))
                    {
                        PlayerPrefs.SetString(UNIQUE_ID, secure.CallStatic<string>("getString",
                            context.Call<AndroidJavaObject>("getContentResolver"),
                            secure.GetStatic<string>("ANDROID_ID")));
                    }

                    using (AndroidJavaClass locale = new AndroidJavaClass("java.util.Locale"))
                    {
                        using (AndroidJavaObject defaultLocale = locale.CallStatic<AndroidJavaObject>("getDefault"))
                        {
                            PlayerPrefs.SetString(COUNTRY, defaultLocale.Call<string>("getCountry"));
                            PlayerPrefs.SetString(LANGUAGE, defaultLocale.Call<string>("getLanguage"));
                        }
                    }

                    PlayerPrefs.SetInt(isGetDeviceInfo, 1);
                }
                catch (Exception exception)
                {
                    PlayerPrefs.SetInt(isGetDeviceInfo, 0);
                    Debug.LogError(string.Format("Msg:\n{0}\nStack Trace:\n{1}", exception.Message,
                        exception.StackTrace));
                }
            }
        }

        public void ShowDeviceInfo()
        {
            Debug.LogError(string.Format(
                "Unique Id = {0}\nCountry = {1}\nLanguage = {2}\nOperatingSystem = {3}\nModel = {4}\nScreenSize = {5}\nMemory = {6}",
                PlayerPrefs.GetString(UNIQUE_ID),
                PlayerPrefs.GetString(COUNTRY),
                PlayerPrefs.GetString(LANGUAGE),
                PlayerPrefs.GetString(OPERATING_SYSTEM),
                PlayerPrefs.GetString(MODEL),
                PlayerPrefs.GetString(SCREEN_SIZE),
                PlayerPrefs.GetString(MEMORY)));
        }

        public void ShowToast(string InContent)
        {
            if (null == context) Initialize();

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                toast.CallStatic<AndroidJavaObject>("makeText",
                    context,
                    InContent,
                    showToastTime).Call("show");
            }));
        }
    }
}
