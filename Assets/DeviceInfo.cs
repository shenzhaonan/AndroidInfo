using UnityEngine;

public class DeviceInfo : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    void OnGUI()
    {
        if (GUILayout.Button("Device Info"))
        {
            Debug.LogError("System Info Unique Id = " + SystemInfo.deviceUniqueIdentifier);
            Debug.LogError("System Info Battery Level = " + SystemInfo.batteryLevel);
            Debug.LogError("System Info Battery Status = " + SystemInfo.batteryStatus);
            Debug.LogError("System Info Device Model = " + SystemInfo.deviceModel);
            Debug.LogError("System Info Device Level = " + SystemInfo.deviceName);
            Debug.LogError("System Info Device Type = " + SystemInfo.deviceType);
            Debug.LogError("System Info Operating System = " + SystemInfo.operatingSystem);
            Debug.LogError("System Info System Memory Size = " + SystemInfo.systemMemorySize);
            Debug.LogError("System Info Unsupported Identifier = " + SystemInfo.unsupportedIdentifier);
        }

        if (GUILayout.Button("Android Info"))
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

                Debug.LogError("Android Info = "+ context.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getDisplayMetrics").Call<int>("heightPixels"));
                //getApplicationContext().getResources().getDisplayMetrics().heightPixels;
            }

            using (AndroidJavaClass locale = new AndroidJavaClass("java.util.Locale"))
            {
                Debug.LogError("get Country = " + locale.Call<AndroidJavaObject>("getDefault").Call<string>("getCountry"));
                   // .getDefault().getCountry()
            }
        }

            if (GUILayout.Button("Show Toast"))
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

                AndroidJavaClass toast = new AndroidJavaClass("android.widget.Toast");
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject javaStr = new AndroidJavaObject("java.lang.String", "Test Toast");
                    toast.Call<AndroidJavaObject>("makeText", context, javaStr, toast.GetStatic<int>("LENGTH_SHORT")).Call("show");
                }));

            }
        }
    }
}
