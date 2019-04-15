using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System.IO;
using System.Collections;


public class ReporterEditor : Editor
{
	[MenuItem("Reporter/Create")]
	public static void CreateReporter()
	{
		const int ReporterExecOrder = -12000;
		GameObject reporterObj = new GameObject();
		reporterObj.name = "Reporter";
		Reporter reporter = reporterObj.AddComponent<Reporter>();
		reporterObj.AddComponent<ReporterMessageReceiver>();


        // Register root object for undo.
        Undo.RegisterCreatedObjectUndo(reporterObj, "Create Reporter Object");

		MonoScript reporterScript = MonoScript.FromMonoBehaviour(reporter);
		string reporterPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(reporterScript));

		if (MonoImporter.GetExecutionOrder(reporterScript) != ReporterExecOrder) {
			MonoImporter.SetExecutionOrder(reporterScript, ReporterExecOrder);
			//Debug.Log("Fixing exec order for " + reporterScript.name);
		}

		reporter.Images = new Images();
		reporter.Images.ClearImage           = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/clear.png"), typeof(Texture2D));
		reporter.Images.CollapseImage        = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/collapse.png"), typeof(Texture2D));
		reporter.Images.ClearOnNewSceneImage = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/clearOnSceneLoaded.png"), typeof(Texture2D));
		reporter.Images.ShowTimeImage        = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/timer_1.png"), typeof(Texture2D));
		reporter.Images.ShowSceneImage       = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/UnityIcon.png"), typeof(Texture2D));
		reporter.Images.UserImage            = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/user.png"), typeof(Texture2D));
		reporter.Images.ShowMemoryImage      = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/memory.png"), typeof(Texture2D));
		reporter.Images.SoftwareImage        = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/software.png"), typeof(Texture2D));
		reporter.Images.DateImage            = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/date.png"), typeof(Texture2D));
		reporter.Images.ShowFpsImage         = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/fps.png"), typeof(Texture2D));
		//reporter.images.graphImage           = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/chart.png"), typeof(Texture2D));
		reporter.Images.InfoImage            = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/info.png"), typeof(Texture2D));
		reporter.Images.SearchImage          = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/search.png"), typeof(Texture2D));
		reporter.Images.CloseImage           = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/close.png"), typeof(Texture2D));
		reporter.Images.BuildFromImage       = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/buildFrom.png"), typeof(Texture2D));
		reporter.Images.SystemInfoImage      = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/ComputerIcon.png"), typeof(Texture2D));
		reporter.Images.GraphicsInfoImage    = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/graphicCard.png"), typeof(Texture2D));
		reporter.Images.BackImage            = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/back.png"), typeof(Texture2D));
		reporter.Images.LogImage             = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/log_icon.png"), typeof(Texture2D));
		reporter.Images.WarningImage         = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/warning_icon.png"), typeof(Texture2D));
		reporter.Images.ErrorImage           = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/error_icon.png"), typeof(Texture2D));
		reporter.Images.BarImage             = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/bar.png"), typeof(Texture2D));
		reporter.Images.ButtonActiveImage   = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/button_active.png"), typeof(Texture2D));
		reporter.Images.EvenLogImage        = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/even_log.png"), typeof(Texture2D));
		reporter.Images.OddLogImage         = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/odd_log.png"), typeof(Texture2D));
		reporter.Images.SelectedImage        = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/selected.png"), typeof(Texture2D));

		reporter.Images.ReporterScrollerSkin = (GUISkin)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/reporterScrollerSkin.guiskin"), typeof(GUISkin));
	}
}

//public class ReporterModificationProcessor : UnityEditor.AssetModificationProcessor
//{
//	[InitializeOnLoad]
//	public class BuildInfo
//	{
//		static BuildInfo()
//		{
//			EditorApplication.update += Update;
//		}

//		static bool isCompiling = true;
//		static void Update()
//		{
//			if (!EditorApplication.isCompiling && isCompiling) {
//				//Debug.Log("Finish Compile");
//				if (!Directory.Exists(Application.dataPath + "/StreamingAssets")) {
//					Directory.CreateDirectory(Application.dataPath + "/StreamingAssets");
//				}
//				string info_path = Application.dataPath + "/StreamingAssets/build_info.txt";
//				StreamWriter build_info = new StreamWriter(info_path);
//				build_info.Write("Build from " + SystemInfo.deviceName + " at " + System.DateTime.Now.ToString());
//				build_info.Close();
//			}

//			isCompiling = EditorApplication.isCompiling;
//		}
//	}
//}
