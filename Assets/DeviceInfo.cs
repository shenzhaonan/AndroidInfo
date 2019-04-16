using Framework.UtilPackage;
using UnityEngine;

public class DeviceInfo : MonoBehaviour
{
    void OnGUI()
    {
        GUI.skin.button.fontSize = 64;

        if (GUILayout.Button("Show Device Info"))
        {
            AndroidTools.Instance.ShowDeviceInfo();
        }

        if (GUILayout.Button("Show Device Info"))
        {
            AndroidTools.Instance.ShowToast("I am a toast msg.");
        }

        //    if (GUILayout.Button("Device Info"))
        //    {
        //        Debug.LogError("System Info Unique Id = " + SystemInfo.deviceUniqueIdentifier);
        //        Debug.LogError("System Info Battery Level = " + SystemInfo.batteryLevel);
        //        Debug.LogError("System Info Battery Status = " + SystemInfo.batteryStatus);
        //        Debug.LogError("System Info Device Model = " + SystemInfo.deviceModel);
        //        Debug.LogError("System Info Device Level = " + SystemInfo.deviceName);
        //        Debug.LogError("System Info Device Type = " + SystemInfo.deviceType);
        //        Debug.LogError("System Info Operating System = " + SystemInfo.operatingSystem);
        //        Debug.LogError("System Info System Memory Size = " + SystemInfo.systemMemorySize);
        //        Debug.LogError("System Info Unsupported Identifier = " + SystemInfo.unsupportedIdentifier);
        //    }

        //    if (GUILayout.Button("Android Height Pixels"))
        //    {
        //        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        //        {
        //            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        //            Debug.LogError("Android Info = "
        //                           + context.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getDisplayMetrics").Get<int>("heightPixels")
        //                           + "," +
        //                           +context.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getDisplayMetrics").Get<int>("widthPixels"));
        //            //getApplicationContext().getResources().getDisplayMetrics().heightPixels;

        //            Debug.LogError("Android Info = "
        //                           + context.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getDisplayMetrics").Get<float>("ydpi")
        //                           + "," +
        //                           +context.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getDisplayMetrics").Get<float>("xdpi"));

        //            Debug.LogError(Screen.width + "," + Screen.height);
        //        }
        //    }

        //    if (GUILayout.Button("Android Country"))
        //    {
        //        using (AndroidJavaClass locale = new AndroidJavaClass("java.util.Locale"))
        //        {

        //            Debug.LogError("get Default Country = " + locale.CallStatic<AndroidJavaObject>("getDefault").Call<string>("getCountry"));
        //            Debug.LogError("get Default Language = " + locale.CallStatic<AndroidJavaObject>("getDefault").Call<string>("getLanguage"));

        //            Debug.LogError("get Country = " + locale.Call<string>("getCountry"));
        //            Debug.LogError("get Language = " + locale.Call<string>("getLanguage"));

        //            Debug.LogError("get Show Country = " + locale.Call<string>("getDisplayCountry"));
        //            Debug.LogError("get Show Language = " + locale.Call<string>("getDisplayLanguage"));
        //            Debug.LogError("get Show Name = " + locale.Call<string>("getDisplayName"));
        //            // .getDefault().getCountry()
        //        }
        //    }

        //    if (GUILayout.Button("Android Os"))
        //    {
        //        using (AndroidJavaClass locale = new AndroidJavaClass("android.os.Build"))
        //        {
        //            Debug.LogError("get Country = " + locale.GetStatic<string>("BRAND"));
        //            Debug.LogError("get Language = " + locale.GetStatic<string>("CPU_ABI"));
        //            Debug.LogError("get Language = " + locale.GetStatic<string>("CPU_ABI2"));
        //        }

        //        using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
        //        {
        //            Debug.LogError("get Language = " + version.GetStatic<int>("SDK_INT"));
        //            Debug.LogError("get Language = " + version.GetStatic<string>("RELEASE"));
        //        }
        //    }

        //    if (GUILayout.Button("Android Id"))
        //    {
        //        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        //        {
        //            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        //            using (AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure"))
        //            {
        //                Debug.LogError("Android id = " + secure.CallStatic<string>("getString",
        //                                   context.Call<AndroidJavaObject>("getContentResolver"),
        //                                   secure.GetStatic<string>("ANDROID_ID")));
        //            }
        //        }
        //    }

        //    if (GUILayout.Button("Show Toast"))
        //    {
        //        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        //        {
        //            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

        //            AndroidJavaClass toast = new AndroidJavaClass("android.widget.Toast");
        //            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        //            {
        //                AndroidJavaObject javaStr = new AndroidJavaObject("java.lang.String", "Test Toast");
        //                toast.CallStatic<AndroidJavaObject>("makeText", context, javaStr, toast.GetStatic<int>("LENGTH_SHORT")).Call("show");
        //            }));

        //        }
        //    }
        //}
    }
}
