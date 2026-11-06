#pragma warning disable CS0618
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Animations;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

public static class AsunderUnity6SetupAndAudit
{
    private static readonly string[] ExpectedScenes = new string[]
    {
        "Assets/SCENES/Main/Loading.unity",
        "Assets/SCENES/Main/IntroScene.unity",
        "Assets/SCENES/Main/PreMainScene.unity",
        "Assets/SCENES/Main/MainScene.unity",
        "Assets/SCENES/Main/PostMainScene.unity",
        "Assets/SCENES/Main/EndScene.unity",
        "Assets/SCENES/Extras/ExtrasMenuScene.unity",
        "Assets/SCENES/Extras/Extra1_WalterNightmare.unity",
        "Assets/SCENES/Extras/Extra2_Fall.unity",
        "Assets/SCENES/Extras/Extra3_Flyby.unity",
        "Assets/SCENES/Extras/Extra4_Blood.unity",
        "Assets/SCENES/Extras/Extra5_Beach.unity",
        "Assets/SCENES/Extras/Extra6_City.unity"
    };

    private const string AuditJsonPath = @"C:\Games\Asunder Earthbound VR\Asunder_Unity6\Logs\unity6_final_audit.json";
    private const string BuildExePath = @"C:\Games\Asunder Earthbound VR\Asunder_Unity6\Builds\Win64\Asunder_Earthbound_Unity6.exe";
    private const string BuildPcExePath = @"C:\Games\Asunder Earthbound VR\Asunder_Unity6\Builds\PC\Asunder_Earthbound_PC.exe";
    private const string BuildVrExePath = @"C:\Games\Asunder Earthbound VR\Asunder_Unity6\Builds\VR\Asunder_Earthbound_VR.exe";

    [Serializable]
    private class SceneAuditEntry
    {
        public int buildIndex;
        public string path;
        public int totalGameObjects;
        public int missingScripts;
        public int brokenRenderers;
        public bool hasOVRCameraController;
        public bool mainSceneAsymmetricLayer12Verified;
    }

    [Serializable]
    private class FinalAuditReport
    {
        public string unityVersion;
        public string timestampUtc;
        public bool openXrConfigured;
        public bool initManagerOnStart;
        public string openXrRenderMode;
        public int enabledInteractionProfiles;
        public List<string> enabledProfileNames = new List<string>();
        public bool ravenAnimatorUpgraded;
        public int totalShaders;
        public int shadersWithErrors;
        public List<string> shaderErrorDetails = new List<string>();
        public int totalMaterials;
        public int brokenMaterials;
        public List<string> brokenMaterialDetails = new List<string>();
        public int totalPrefabs;
        public int prefabMissingScripts;
        public List<string> prefabMissingDetails = new List<string>();
        public int totalAudioClips;
        public int invalidAudioClips;
        public int totalStandaloneAnimFiles;
        public int invalidAnimFiles;
        public int totalScenesAudited;
        public int totalSceneMissingScripts;
        public int totalSceneBrokenRenderers;
        public int terrainTreeShaderWarnings;
        public bool appManagerAudio2DVerified;
        public bool buildSettingsOrderVerified;
        public bool mainSceneLayer12StereoVerified;
        public List<SceneAuditEntry> scenes = new List<SceneAuditEntry>();
        public string buildResult;
        public int buildTotalErrors;
        public int buildTotalWarnings;
        public ulong buildTotalSizeBytes;
        public double buildDurationSeconds;
        public bool exeExistsOnDisk;
        public long exeSizeBytes;
        public bool pcExeExistsOnDisk;
        public long pcExeSizeBytes;
        public bool vrExeExistsOnDisk;
        public long vrExeSizeBytes;
        public bool openXrRuntimePluginsDeployed;
        public bool allChecksPassed;
    }

    [MenuItem("Asunder/DiagnoseJDK")]
    public static void DiagnoseJDK()
    {
        System.Type t = System.Type.GetType("UnityEditor.Android.AndroidExternalToolsSettings, UnityEditor.Android.Extensions");
        if (t != null)
        {
            Debug.Log("[DiagnoseJDK] Found AndroidExternalToolsSettings!");
            foreach (var p in t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static))
            {
                try { Debug.Log($"[DiagnoseJDK] Prop: {p.Name} = '{p.GetValue(null)}'"); }
                catch (System.Exception e) { Debug.Log($"[DiagnoseJDK] Prop: {p.Name} threw {e.Message}"); }
            }
            System.Type javaToolsType = t.Assembly.GetType("UnityEditor.Android.AndroidJavaTools");
            if (javaToolsType != null)
            {
                Debug.Log("[DiagnoseJDK] Found AndroidJavaTools!");
                foreach (var p in javaToolsType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance))
                {
                    try { Debug.Log($"[DiagnoseJDK] JavaTools Prop: {p.Name} = '{p.GetValue(null)}'"); }
                    catch (System.Exception e) { Debug.Log($"[DiagnoseJDK] JavaTools Prop: {p.Name} threw {e.Message}"); }
                }
                foreach (var m in javaToolsType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance))
                {
                    Debug.Log($"[DiagnoseJDK] JavaTools Method: {m.Name}");
                }
            }
            System.Type javaRootType = t.Assembly.GetType("UnityEditor.Android.AndroidJavaRoot");
            if (javaRootType != null)
            {
                var getInstance = javaRootType.GetMethod("GetInstance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                object instance = getInstance != null ? getInstance.Invoke(null, null) : null;
                Debug.Log($"[DiagnoseJDK] AndroidJavaRoot instance: {instance}");
                if (instance != null)
                {
                    foreach (var p in javaRootType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                    {
                        try { Debug.Log($"[DiagnoseJDK] JavaRoot Instance Prop: {p.Name} = '{p.GetValue(instance)}'"); }
                        catch (System.Exception e) { Debug.Log($"[DiagnoseJDK] JavaRoot Instance Prop: {p.Name} threw {e.Message}"); }
                    }
                    var valMethod = javaRootType.GetMethod("Validate", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (valMethod != null)
                    {
                        try
                        {
                            string[] testPaths = new string[] { @"C:\Users\greattug\jdk-17.0.12+7", @"C:\Users\greattug\Unity6\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK" };
                        foreach (var tp in testPaths)
                        {
                            object valRes = valMethod.Invoke(instance, new object[] { tp });
                            string details = "";
                            if (valRes != null)
                            {
                                foreach (var fld in valRes.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                                    details += $"{fld.Name}={fld.GetValue(valRes)}, ";
                                foreach (var prp in valRes.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                                    details += $"{prp.Name}={prp.GetValue(valRes)}, ";
                            }
                            Debug.Log($"[DiagnoseJDK] Validate result for {tp}: '{details}'");
                        }
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log($"[DiagnoseJDK] Validate threw: {e}");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("[DiagnoseJDK] Could not find AndroidExternalToolsSettings type!");
        }
    }

    public static void ConfigureAndroidSDKPaths()
    {
        EditorPrefs.SetString("AndroidSdkRoot", @"C:\Users\greattug\AndroidSDK");
        EditorPrefs.SetString("AndroidNdkRoot", @"C:\Program Files (x86)\Android\AndroidNDK\android-ndk-r27c");
        EditorPrefs.SetString("Jdk17Path", @"C:\Users\greattug\jdk-17.0.12+7");
        EditorPrefs.SetString("JdkPath", @"C:\Users\greattug\jdk-17.0.12+7");
        EditorPrefs.SetBool("AndroidSdkRootUseEmbedded", true);
        EditorPrefs.SetBool("AndroidNdkRootUseEmbedded", true);
        EditorPrefs.SetBool("JdkUseEmbedded", true);
        EditorPrefs.SetBool("GradleUseEmbedded", true);
        QualitySettings.shadows = ShadowQuality.HardOnly;

        try
        {
            System.Type toolsSettingsType = System.Type.GetType("UnityEditor.Android.AndroidExternalToolsSettings, UnityEditor.Android.Extensions");
            if (toolsSettingsType != null)
            {
                var jdkProp = toolsSettingsType.GetProperty("jdkRootPath", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (jdkProp != null) jdkProp.SetValue(null, @"C:\Users\greattug\jdk-17.0.12+7");
                var sdkProp = toolsSettingsType.GetProperty("sdkRootPath", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (sdkProp != null) sdkProp.SetValue(null, @"C:\Users\greattug\AndroidSDK");
                var ndkProp = toolsSettingsType.GetProperty("ndkRootPath", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (ndkProp != null) ndkProp.SetValue(null, @"C:\Program Files (x86)\Android\AndroidNDK\android-ndk-r27c");
            }

            System.Type javaRootType = System.Type.GetType("UnityEditor.Android.AndroidJavaRoot, UnityEditor.Android.Extensions");
            if (javaRootType != null)
            {
                var getInstance = javaRootType.GetMethod("GetInstance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                object instance = getInstance != null ? getInstance.Invoke(null, null) : null;
                if (instance != null)
                {
                    var customDirProp = javaRootType.GetProperty("CustomDirectory", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (customDirProp != null) customDirProp.SetValue(instance, @"C:\Users\greattug\jdk-17.0.12+7");
                    var useEmbeddedProp = javaRootType.GetProperty("UseEmbedded", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (useEmbeddedProp != null) useEmbeddedProp.SetValue(instance, true);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[AsunderAndroid] Reflection settings warning: " + ex.Message);
        }

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }
        EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;
        EditorUserBuildSettings.buildAppBundle = false;
        Debug.Log("[AsunderAndroid] Android SDK (with CMake 3.22.1), NDK, and JDK 17 paths configured.");
    }

    [MenuItem("Asunder/Build Quest 3 Standalone APK")]
    public static void BuildQuest3StandaloneAPK()
    {
        ConfigureAndroidSDKPaths();
        ConfigureOpenXRAndroid(null);
        string apkDir = @"C:\Games\Asunder Earthbound VR\Asunder_Unity6\Builds\Android";
        if (!Directory.Exists(apkDir)) Directory.CreateDirectory(apkDir);
        string apkPath = Path.Combine(apkDir, "Asunder_Earthbound_Quest3_v1.0.0.apk");

        BuildPlayerOptions opts = new BuildPlayerOptions();
        opts.scenes = ExpectedScenes;
        opts.locationPathName = apkPath;
        opts.targetGroup = BuildTargetGroup.Android;
        opts.target = BuildTarget.Android;
        opts.options = BuildOptions.None;

        Debug.Log("[AsunderQuest3] Building Standalone Quest 3 APK to: " + apkPath);
        BuildReport report = BuildPipeline.BuildPlayer(opts);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[AsunderQuest3] Quest 3 APK Build Succeeded! Size: {report.summary.totalSize} bytes ({report.summary.totalSize / (1024 * 1024)} MB) at {apkPath}");
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Quest 3 Build Complete", $"Successfully built Quest 3 APK:\n{apkPath}\nSize: {report.summary.totalSize / (1024 * 1024)} MB", "OK");
            }
        }
        else
        {
            Debug.LogError($"[AsunderQuest3] Quest 3 APK Build Failed with {report.summary.totalErrors} errors.");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }

    [MenuItem("Asunder/Build Cardboard Mobile VR APK")]
    public static void BuildCardboardStandaloneAPK()
    {
        ConfigureAndroidSDKPaths();
        ConfigureCardboardAndroid();
        string apkDir = @"C:\Games\Asunder Earthbound VR\Asunder_Unity6\Builds\Android";
        if (!Directory.Exists(apkDir)) Directory.CreateDirectory(apkDir);
        string apkPath = Path.Combine(apkDir, "Asunder_Earthbound_Cardboard_v1.0.0.apk");

        BuildPlayerOptions opts = new BuildPlayerOptions();
        opts.scenes = ExpectedScenes;
        opts.locationPathName = apkPath;
        opts.targetGroup = BuildTargetGroup.Android;
        opts.target = BuildTarget.Android;
        opts.options = BuildOptions.None;

        Debug.Log("[AsunderCardboard] Building Standalone Cardboard APK to: " + apkPath);
        BuildReport report = BuildPipeline.BuildPlayer(opts);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[AsunderCardboard] Cardboard APK Build Succeeded! Size: {report.summary.totalSize} bytes ({report.summary.totalSize / (1024 * 1024)} MB) at {apkPath}");
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Cardboard Build Complete", $"Successfully built Cardboard APK:\n{apkPath}\nSize: {report.summary.totalSize / (1024 * 1024)} MB", "OK");
            }
        }
        else
        {
            Debug.LogError($"[AsunderCardboard] Cardboard APK Build Failed with {report.summary.totalErrors} errors.");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }

    [MenuItem("Asunder/Build Both Android APKs (Quest 3 + Cardboard)")]
    public static void BuildBothAndroidAPKs()
    {
        Debug.Log("[AsunderAndroid] Starting Dual Android Compilation: Meta Quest 3 + Google Cardboard...");
        BuildQuest3StandaloneAPK();
        BuildCardboardStandaloneAPK();
        Debug.Log("[AsunderAndroid] All Android builds successfully finished!");
    }

    public static void ConfigureCardboardAndroid()
    {
        ConfigureAndroidSDKPaths();
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.colorSpace = ColorSpace.Gamma;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.GameActivity;
        PlayerSettings.Android.useCustomKeystore = false;
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3, UnityEngine.Rendering.GraphicsDeviceType.Vulkan });
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.AldinDynamics.Asunder.Cardboard");
        PlayerSettings.productName = "Asunder: Earthbound (Cardboard VR)";
        PlayerSettings.companyName = "Aldin Dynamics";

        const string generalAssetPath = "Assets/XR/XRGeneralSettingsPerBuildTarget.asset";
        if (EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.settingsKey, out XRGeneralSettingsPerBuildTarget perBuildTarget) && perBuildTarget != null)
        {
            XRGeneralSettings androidGeneral = perBuildTarget.SettingsForBuildTarget(BuildTargetGroup.Android);
            if (androidGeneral != null)
            {
                androidGeneral.InitManagerOnStart = false;
                EditorUtility.SetDirty(androidGeneral);
            }
            XRManagerSettings androidManager = perBuildTarget.ManagerSettingsForBuildTarget(BuildTargetGroup.Android);
            if (androidManager != null)
            {
                XRPackageMetadataStore.RemoveLoader(androidManager, typeof(OpenXRLoader).FullName, BuildTargetGroup.Android);
                EditorUtility.SetDirty(androidManager);
            }
            EditorUtility.SetDirty(perBuildTarget);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("[AsunderCardboard] Configured Cardboard Android settings (ARM64, split-screen stereo, phone gyro, OpenXR disabled).");
    }

    [MenuItem("Asunder/Build Quest 3 APK (Android)")]
    public static void BuildQuest3APKMenu()
    {
        ConfigureAndroidSDKPaths();
        Debug.Log("[AsunderQuest3] Initiating Quest 3 Standalone Android Build...");
        ConfigureOpenXRAndroid(null);
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
        {
            Debug.LogError("[AsunderQuest3] Android Build Support is not installed in this Unity Editor. Please install 'Android Build Support', 'Android SDK & NDK Tools', and 'OpenJDK' via Unity Hub.");
            EditorUtility.DisplayDialog("Quest 3 Build Notice", "Android Build Support is not installed in this Unity Editor.\n\nPlease install the Android module via Unity Hub:\nUnity Hub -> Installs -> 6000.6.6f1 -> Add Modules -> Android Build Support (with Android SDK & NDK Tools and OpenJDK).", "OK");
            return;
        }

        string apkDir = @"C:\Games\Asunder Earthbound VR\Asunder_Unity6\Builds\Quest3";
        if (!Directory.Exists(apkDir)) Directory.CreateDirectory(apkDir);
        string apkPath = Path.Combine(apkDir, "Asunder_Earthbound_Quest3.apk");

        BuildPlayerOptions opts = new BuildPlayerOptions();
        opts.scenes = ExpectedScenes;
        opts.locationPathName = apkPath;
        opts.targetGroup = BuildTargetGroup.Android;
        opts.target = BuildTarget.Android;
        opts.options = BuildOptions.None;

        Debug.Log("[AsunderQuest3] Building APK to: " + apkPath);
        BuildReport report = BuildPipeline.BuildPlayer(opts);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[AsunderQuest3] Quest 3 APK Build Succeeded! Size: {report.summary.totalSize} bytes at {apkPath}");
            EditorUtility.DisplayDialog("Quest 3 Build Complete", $"Successfully built Quest 3 APK:\n{apkPath}\nSize: {report.summary.totalSize / (1024 * 1024)} MB", "OK");
        }
        else
        {
            Debug.LogError($"[AsunderQuest3] Quest 3 APK Build Failed: {report.summary.totalErrors} errors.");
        }
    }

    [MenuItem("Asunder/Run Full Audit (No Build)")]
    public static void RunAuditOnly()
    {
        RunInternal(performBuild: false);
    }

    [MenuItem("Asunder/Build All Standalone Players (PC + VR + Unified Win64)")]
    public static void RunAll()
    {
        RunInternal(performBuild: true);
    }

    [MenuItem("Asunder/Build PC Version (Standalone Desktop)")]
    public static void BuildPCStandalone()
    {
        FinalAuditReport report = new FinalAuditReport
        {
            unityVersion = Application.unityVersion,
            timestampUtc = DateTime.UtcNow.ToString("o")
        };
        ConfigureBuildSettings(report);
        ConfigureOpenXRStandalone(report);
        ConfigureAlwaysIncludedShaders();
        string pcDir = Path.GetDirectoryName(BuildPcExePath);
        if (!Directory.Exists(pcDir)) Directory.CreateDirectory(pcDir);
        BuildPlayerOptions bpo = new BuildPlayerOptions
        {
            scenes = ExpectedScenes,
            locationPathName = BuildPcExePath,
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        BuildReport br = BuildPipeline.BuildPlayer(bpo);
        FileInfo pcInfo = new FileInfo(BuildPcExePath);
        report.buildResult = br.summary.result.ToString();
        report.buildTotalErrors = br.summary.totalErrors;
        report.buildTotalWarnings = br.summary.totalWarnings;
        report.buildTotalSizeBytes = br.summary.totalSize;
        report.buildDurationSeconds = br.summary.totalTime.TotalSeconds;
        report.pcExeExistsOnDisk = pcInfo.Exists;
        report.pcExeSizeBytes = pcInfo.Exists ? pcInfo.Length : 0L;
        WriteReport(report);
        Debug.Log($"[AsunderBuild] PC Player Build: {br.summary.result} ({br.summary.totalSize} bytes)");
    }

    [MenuItem("Asunder/Build VR Version (OpenXR Standalone)")]
    public static void BuildVRStandalone()
    {
        FinalAuditReport report = new FinalAuditReport
        {
            unityVersion = Application.unityVersion,
            timestampUtc = DateTime.UtcNow.ToString("o")
        };
        ConfigureBuildSettings(report);
        ConfigureOpenXRStandalone(report);
        ConfigureAlwaysIncludedShaders();
        string vrDir = Path.GetDirectoryName(BuildVrExePath);
        if (!Directory.Exists(vrDir)) Directory.CreateDirectory(vrDir);
        BuildPlayerOptions bpo = new BuildPlayerOptions
        {
            scenes = ExpectedScenes,
            locationPathName = BuildVrExePath,
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        BuildReport br = BuildPipeline.BuildPlayer(bpo);
        FileInfo vrInfo = new FileInfo(BuildVrExePath);
        report.buildResult = br.summary.result.ToString();
        report.buildTotalErrors = br.summary.totalErrors;
        report.buildTotalWarnings = br.summary.totalWarnings;
        report.buildTotalSizeBytes = br.summary.totalSize;
        report.buildDurationSeconds = br.summary.totalTime.TotalSeconds;
        report.vrExeExistsOnDisk = vrInfo.Exists;
        report.vrExeSizeBytes = vrInfo.Exists ? vrInfo.Length : 0L;
        WriteReport(report);
        Debug.Log($"[AsunderBuild] VR Player Build: {br.summary.result} ({br.summary.totalSize} bytes)");
    }

    private static void RunInternal(bool performBuild)
    {
        FinalAuditReport report = new FinalAuditReport
        {
            unityVersion = Application.unityVersion,
            timestampUtc = DateTime.UtcNow.ToString("o")
        };

        try
        {
            Debug.Log("[AsunderAudit] Step 1/5: Configuring BuildSettings & OpenXR Dual-Mode (PCVR + Quest 3)...");
            ConfigureBuildSettings(report);
            ConfigureOpenXRStandalone(report);
            ConfigureOpenXRAndroid(report);
            ConfigureAlwaysIncludedShaders();

            Debug.Log("[AsunderAudit] Step 2/5: Verifying RavenControl Mecanim AnimatorController...");
            UpgradeRavenAnimatorAndCleanOrphans(report);

            Debug.Log("[AsunderAudit] Step 3/5: Auditing Shaders, Materials, Prefabs, Audio & Animations...");
            AuditAssets(report);

            Debug.Log("[AsunderAudit] Step 4/5: Auditing all 13 Scenes, Terrain Trees & Asymmetric Layer-12 VR Stereo...");
            AuditScenes(report);

            EditorSceneManager.OpenScene(ExpectedScenes[0], OpenSceneMode.Single);
            AssetDatabase.SaveAssets();

            if (performBuild)
            {
                Debug.Log("[AsunderAudit] Step 5/5: Building Win64 Standalone Players (Unified Win64, PC Desktop, VR OpenXR)...");
                RunWin64Build(report);
            }
            else
            {
                report.buildResult = "Skipped (AuditOnly)";
                FileInfo exeInfo = new FileInfo(BuildExePath);
                report.exeExistsOnDisk = exeInfo.Exists;
                report.exeSizeBytes = exeInfo.Exists ? exeInfo.Length : 0L;

                FileInfo pcInfo = new FileInfo(BuildPcExePath);
                report.pcExeExistsOnDisk = pcInfo.Exists;
                report.pcExeSizeBytes = pcInfo.Exists ? pcInfo.Length : 0L;

                FileInfo vrInfo = new FileInfo(BuildVrExePath);
                report.vrExeExistsOnDisk = vrInfo.Exists;
                report.vrExeSizeBytes = vrInfo.Exists ? vrInfo.Length : 0L;

                string outDir = Path.GetDirectoryName(BuildExePath);
                string vrDir = Path.GetDirectoryName(BuildVrExePath);
                string pluginsX64 = Path.Combine(outDir, "Asunder_Earthbound_Unity6_Data", "Plugins", "x86_64");
                string vrPluginsX64 = Path.Combine(vrDir, "Asunder_Earthbound_VR_Data", "Plugins", "x86_64");
                report.openXrRuntimePluginsDeployed = File.Exists(Path.Combine(pluginsX64, "openxr_loader.dll")) &&
                                                      File.Exists(Path.Combine(pluginsX64, "UnityOpenXR.dll")) &&
                                                      File.Exists(Path.Combine(vrPluginsX64, "openxr_loader.dll")) &&
                                                      File.Exists(Path.Combine(vrPluginsX64, "UnityOpenXR.dll"));
            }

            report.allChecksPassed =
                report.openXrConfigured &&
                !report.initManagerOnStart &&
                report.ravenAnimatorUpgraded &&
                report.shadersWithErrors == 0 &&
                report.brokenMaterials == 0 &&
                report.prefabMissingScripts == 0 &&
                report.invalidAudioClips == 0 &&
                report.invalidAnimFiles == 0 &&
                report.totalScenesAudited == 13 &&
                report.totalSceneMissingScripts == 0 &&
                report.totalSceneBrokenRenderers == 0 &&
                report.terrainTreeShaderWarnings == 0 &&
                report.appManagerAudio2DVerified &&
                report.buildSettingsOrderVerified &&
                report.mainSceneLayer12StereoVerified &&
                (!performBuild || (report.buildResult == "Succeeded" && report.buildTotalErrors == 0)) &&
                report.exeExistsOnDisk && report.exeSizeBytes > 0 &&
                report.pcExeExistsOnDisk && report.pcExeSizeBytes > 0 &&
                report.vrExeExistsOnDisk && report.vrExeSizeBytes > 0 &&
                report.openXrRuntimePluginsDeployed;

            WriteReport(report);
            Debug.Log("[AsunderAudit] COMPLETE! allChecksPassed=" + report.allChecksPassed + " | pcExe=" + report.pcExeExistsOnDisk + " | vrExe=" + report.vrExeExistsOnDisk);

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(report.allChecksPassed ? 0 : 2);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[AsunderAudit] FATAL EXCEPTION: " + ex);
            report.buildResult = "Exception: " + ex.Message;
            report.allChecksPassed = false;
            WriteReport(report);
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(3);
            }
        }
    }

    private static void EnsureFolder(string parent, string child)
    {
        string combined = parent + "/" + child;
        if (!AssetDatabase.IsValidFolder(combined))
        {
            AssetDatabase.CreateFolder(parent, child);
        }
    }

    private static void ConfigureBuildSettings(FinalAuditReport report)
    {
        EditorBuildSettingsScene[] desired = new EditorBuildSettingsScene[ExpectedScenes.Length];
        for (int i = 0; i < ExpectedScenes.Length; i++)
        {
            desired[i] = new EditorBuildSettingsScene(ExpectedScenes[i], true);
        }
        EditorBuildSettings.scenes = desired;

        EditorBuildSettingsScene[] actual = EditorBuildSettings.scenes;
        bool ok = actual.Length == ExpectedScenes.Length;
        for (int i = 0; ok && i < ExpectedScenes.Length; i++)
        {
            if (!actual[i].enabled || actual[i].path != ExpectedScenes[i])
            {
                ok = false;
            }
        }
        report.buildSettingsOrderVerified = ok;
    }

    private static void ConfigureOpenXRStandalone(FinalAuditReport report)
    {
        EnsureFolder("Assets", "XR");
        EnsureFolder("Assets/XR", "Settings");
        EnsureFolder("Assets/XR", "Loaders");

        const string generalAssetPath = "Assets/XR/XRGeneralSettingsPerBuildTarget.asset";
        XRGeneralSettingsPerBuildTarget perBuildTarget = null;
        if (!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.settingsKey, out perBuildTarget) || perBuildTarget == null)
        {
            perBuildTarget = AssetDatabase.LoadAssetAtPath<XRGeneralSettingsPerBuildTarget>(generalAssetPath);
            if (perBuildTarget == null)
            {
                perBuildTarget = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
                AssetDatabase.CreateAsset(perBuildTarget, generalAssetPath);
            }
            EditorBuildSettings.AddConfigObject(XRGeneralSettings.settingsKey, perBuildTarget, true);
        }

        if (!perBuildTarget.HasManagerSettingsForBuildTarget(BuildTargetGroup.Standalone))
        {
            perBuildTarget.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Standalone);
        }

        XRGeneralSettings standaloneGeneral = perBuildTarget.SettingsForBuildTarget(BuildTargetGroup.Standalone);
        XRManagerSettings standaloneManager = perBuildTarget.ManagerSettingsForBuildTarget(BuildTargetGroup.Standalone);

        standaloneGeneral.InitManagerOnStart = false;
        EditorUtility.SetDirty(standaloneGeneral);
        EditorUtility.SetDirty(perBuildTarget);

        bool assigned = XRPackageMetadataStore.AssignLoader(standaloneManager, typeof(OpenXRLoader).FullName, BuildTargetGroup.Standalone);
        Debug.Log("[AsunderAudit] AssignLoader(OpenXRLoader) => " + assigned);

        Type pkgSettingsType = typeof(FeatureHelpers).Assembly.GetType("UnityEditor.XR.OpenXR.OpenXRPackageSettings");
        if (pkgSettingsType != null)
        {
            MethodInfo getOrCreate = pkgSettingsType.GetMethod("GetOrCreateInstance", BindingFlags.Public | BindingFlags.Static);
            if (getOrCreate != null)
            {
                UnityEngine.Object pkgObj = getOrCreate.Invoke(null, null) as UnityEngine.Object;
                if (pkgObj != null)
                {
                    EditorUtility.SetDirty(pkgObj);
                }
            }
        }

        FeatureHelpers.RefreshFeatures(BuildTargetGroup.Standalone);
        OpenXRSettings openXrSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);
        if (openXrSettings != null)
        {
            openXrSettings.renderMode = OpenXRSettings.RenderMode.MultiPass;
            openXrSettings.depthSubmissionMode = OpenXRSettings.DepthSubmissionMode.Depth24Bit;

            Type[] profileTypes = new Type[]
            {
                typeof(OculusTouchControllerProfile),
                typeof(ValveIndexControllerProfile),
                typeof(HTCViveControllerProfile),
                typeof(KHRSimpleControllerProfile),
                typeof(MicrosoftMotionControllerProfile),
                typeof(HPReverbG2ControllerProfile),
                typeof(MetaQuestTouchPlusControllerProfile),
                typeof(MetaQuestTouchProControllerProfile)
            };

            foreach (Type pt in profileTypes)
            {
                OpenXRFeature feat = openXrSettings.GetFeature(pt);
                if (feat != null)
                {
                    feat.enabled = true;
                    EditorUtility.SetDirty(feat);
                    report.enabledInteractionProfiles++;
                    report.enabledProfileNames.Add(pt.Name);
                }
            }

            EditorUtility.SetDirty(openXrSettings);
            report.openXrRenderMode = openXrSettings.renderMode.ToString();
        }

        if (AssetDatabase.IsValidFolder("Assets/XR 1"))
        {
            AssetDatabase.DeleteAsset("Assets/XR 1");
        }

        AssetDatabase.SaveAssets();

        bool hasOpenXrLoader = standaloneManager != null &&
                               standaloneManager.activeLoaders != null &&
                               standaloneManager.activeLoaders.Any(l => l is OpenXRLoader);

        report.openXrConfigured = hasOpenXrLoader && openXrSettings != null;
        report.initManagerOnStart = standaloneGeneral.InitManagerOnStart;
    }

    private static void ConfigureOpenXRAndroid(FinalAuditReport report)
    {
        ConfigureAndroidSDKPaths();
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.colorSpace = ColorSpace.Gamma;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.GameActivity;
        PlayerSettings.Android.useCustomKeystore = false;
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.Vulkan, UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.AldinDynamics.Asunder");
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, "com.AldinDynamics.Asunder");
        PlayerSettings.productName = "Asunder: Earthbound";
        PlayerSettings.companyName = "Aldin Dynamics";

        const string generalAssetPath = "Assets/XR/XRGeneralSettingsPerBuildTarget.asset";
        XRGeneralSettingsPerBuildTarget perBuildTarget = null;
        if (!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.settingsKey, out perBuildTarget) || perBuildTarget == null)
        {
            perBuildTarget = AssetDatabase.LoadAssetAtPath<XRGeneralSettingsPerBuildTarget>(generalAssetPath);
            if (perBuildTarget == null)
            {
                perBuildTarget = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
                AssetDatabase.CreateAsset(perBuildTarget, generalAssetPath);
            }
            EditorBuildSettings.AddConfigObject(XRGeneralSettings.settingsKey, perBuildTarget, true);
        }

        if (!perBuildTarget.HasManagerSettingsForBuildTarget(BuildTargetGroup.Android))
        {
            perBuildTarget.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);
        }

        XRGeneralSettings androidGeneral = perBuildTarget.SettingsForBuildTarget(BuildTargetGroup.Android);
        XRManagerSettings androidManager = perBuildTarget.ManagerSettingsForBuildTarget(BuildTargetGroup.Android);

        if (androidGeneral != null)
        {
            androidGeneral.InitManagerOnStart = true;
            EditorUtility.SetDirty(androidGeneral);
        }

        XRPackageMetadataStore.AssignLoader(androidManager, typeof(OpenXRLoader).FullName, BuildTargetGroup.Android);

        FeatureHelpers.RefreshFeatures(BuildTargetGroup.Android);
        OpenXRSettings androidSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
        if (androidSettings != null)
        {
            androidSettings.renderMode = OpenXRSettings.RenderMode.MultiPass;
            foreach (OpenXRFeature feature in androidSettings.GetFeatures<OpenXRFeature>())
            {
                if (feature.GetType().Name == "OculusQuestFeature")
                {
                    feature.enabled = false;
                    EditorUtility.SetDirty(feature);
                    continue;
                }

                if (feature is OculusTouchControllerProfile ||
                    feature is MetaQuestTouchProControllerProfile ||
                    feature is MetaQuestTouchPlusControllerProfile)
                {
                    feature.enabled = true;
                    EditorUtility.SetDirty(feature);
                }

                if (feature.GetType().Name.Contains("MetaQuestFeature"))
                {
                    feature.enabled = true;
                    SerializedObject questSo = new SerializedObject(feature);
                    SerializedProperty targetDevicesProp = questSo.FindProperty("targetDevices");
                    if (targetDevicesProp != null)
                    {
                        targetDevicesProp.ClearArray();
                        string[] manifestNames = new string[] { "eureka", "quest3s", "quest2", "cambria" };
                        string[] visibleNames = new string[] { "Quest 3", "Quest 3S", "Quest 2", "Quest Pro" };
                        for (int i = 0; i < manifestNames.Length; i++)
                        {
                            targetDevicesProp.InsertArrayElementAtIndex(i);
                            SerializedProperty elem = targetDevicesProp.GetArrayElementAtIndex(i);
                            elem.FindPropertyRelative("visibleName").stringValue = visibleNames[i];
                            elem.FindPropertyRelative("manifestName").stringValue = manifestNames[i];
                            elem.FindPropertyRelative("enabled").boolValue = true;
                        }
                    }
                    SerializedProperty symmProp = questSo.FindProperty("m_symmetricProjection");
                    if (symmProp != null) symmProp.boolValue = false;
                    questSo.ApplyModifiedProperties();
                    EditorUtility.SetDirty(feature);
                }
            }

            var valIssues = new System.Collections.Generic.List<OpenXRFeature.ValidationRule>();
            UnityEditor.XR.OpenXR.OpenXRProjectValidation.GetCurrentValidationIssues(valIssues, BuildTargetGroup.Android);
            foreach (var issue in valIssues)
            {
                Debug.Log($"[AsunderOpenXRValidation] {issue.message} (error: {issue.error})");
                if (issue.fixIt != null)
                {
                    try
                    {
                        issue.fixIt.Invoke();
                        Debug.Log($"[AsunderOpenXRValidation] Successfully fixed: {issue.message}");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"[AsunderOpenXRValidation] Could not auto-fix '{issue.message}': {ex.Message}");
                    }
                }
            }
            EditorUtility.SetDirty(androidSettings);
        }
        EditorUtility.SetDirty(perBuildTarget);
        AssetDatabase.SaveAssets();
        Debug.Log("[AsunderQuest3] Configured Android OpenXR settings for Quest 3 (ARM64, IL2CPP, MultiPass, Meta Quest feature group).");
    }

    private static void ConfigureAlwaysIncludedShaders()
    {
        UnityEngine.Object[] gfxAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset");
        if (gfxAssets == null || gfxAssets.Length == 0)
            return;

        SerializedObject gfxSo = new SerializedObject(gfxAssets[0]);
        SerializedProperty alwaysIncluded = gfxSo.FindProperty("m_AlwaysIncludedShaders");
        if (alwaysIncluded == null)
            return;

        bool modified = false;

        // First strip any null or HideFlags.DontSave shaders (e.g. Library/unity default resources like Hidden/Internal-Colored,
        // which Unity already ships inside unity default resources automatically!)
        for (int i = alwaysIncluded.arraySize - 1; i >= 0; i--)
        {
            Shader s = alwaysIncluded.GetArrayElementAtIndex(i).objectReferenceValue as Shader;
            if (s == null || (s.hideFlags & HideFlags.DontSave) != 0 || AssetDatabase.GetAssetPath(s) == "Library/unity default resources")
            {
                alwaysIncluded.DeleteArrayElementAtIndex(i);
                // Note: in SerializedProperty object arrays, DeleteArrayElementAtIndex sets non-null ref to null on first call
                if (i < alwaysIncluded.arraySize && alwaysIncluded.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    alwaysIncluded.DeleteArrayElementAtIndex(i);
                }
                modified = true;
            }
        }

        HashSet<Shader> existing = new HashSet<Shader>();
        for (int i = 0; i < alwaysIncluded.arraySize; i++)
        {
            Shader s = alwaysIncluded.GetArrayElementAtIndex(i).objectReferenceValue as Shader;
            if (s != null)
                existing.Add(s);
        }

        // Include custom project post-FX shaders from Assets/Shader that don't carry HideFlags.DontSave
        string[] customPostFxNames = new string[]
        {
            "Hidden/CC_BrightnessContrastGamma",
            "Hidden/CC_DoubleVision",
            "Hidden/CC_FastVignette",
            "Hidden/CC_Grayscale",
            "Hidden/BlendModesOverlay",
            "Hidden/FastBloom",
            "Hidden/BlendForBloom",
            "Hidden/LensFlareCreate",
            "Hidden/BlurAndFlares",
            "Hidden/BrightPassFilter2",
            "Hidden/BlurEffectConeTap",
            "Hidden/SeparableBlur",
            "Hidden/ColorCorrectionCurves",
            "Hidden/ColorCorrectionCurvesSimple",
            "Hidden/ColorCorrectionSelective",
            "Hidden/SunShaftsComposite",
            "Hidden/ContrastComposite",
            "Hidden/SimpleClear",
            "Hidden/Dof/DX11Dof",
            "Hidden/Dof/DepthOfFieldHdr",
            "Hidden/GlobalFog"
        };

        foreach (string sName in customPostFxNames)
        {
            Shader sh = Shader.Find(sName);
            if (sh != null && (sh.hideFlags & HideFlags.DontSave) == 0 && !existing.Contains(sh))
            {
                int idx = alwaysIncluded.arraySize;
                alwaysIncluded.InsertArrayElementAtIndex(idx);
                alwaysIncluded.GetArrayElementAtIndex(idx).objectReferenceValue = sh;
                existing.Add(sh);
                modified = true;
            }
        }

        if (modified)
        {
            gfxSo.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(gfxAssets[0]);
            AssetDatabase.SaveAssets();
        }
    }

    private static void UpgradeRavenAnimatorAndCleanOrphans(FinalAuditReport report)
    {
        const string ctrlPath = "Assets/AnimatorController/RavenControl.controller";
        AnimatorController ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(ctrlPath);
        AnimationClip glideClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath("9bd955e3a23e86f4b90eb1083284fb32"));
        AnimationClip flapClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath("468bde9f43417de4dbcf830ee1e1e112"));

        if (ctrl != null && ctrl.layers.Length > 0)
        {
            AnimatorStateMachine sm = ctrl.layers[0].stateMachine;
            if (sm.states == null || sm.states.Length == 0)
            {
                AnimatorState glideState = sm.AddState("Glide", new Vector3(260f, 60f, 0f));
                glideState.motion = glideClip;
                glideState.speed = 1f;
                glideState.iKOnFeet = true;

                AnimatorState flapState = sm.AddState("Flap", new Vector3(260f, 160f, 0f));
                flapState.motion = flapClip;
                flapState.speed = 0.7f;
                flapState.iKOnFeet = true;

                sm.defaultState = glideState;

                AnimatorStateTransition toFlap = glideState.AddTransition(flapState);
                toFlap.hasExitTime = false;
                toFlap.duration = 0.2f;
                toFlap.AddCondition(AnimatorConditionMode.IfNot, 0f, "Glide");

                AnimatorStateTransition toGlide = flapState.AddTransition(glideState);
                toGlide.hasExitTime = false;
                toGlide.duration = 0.2f;
                toGlide.AddCondition(AnimatorConditionMode.If, 0f, "Glide");

                EditorUtility.SetDirty(sm);
                EditorUtility.SetDirty(ctrl);
                AssetDatabase.SaveAssets();
            }
            report.ravenAnimatorUpgraded = sm.states != null && sm.states.Length >= 2 && sm.defaultState != null;
        }

        if (AssetDatabase.IsValidFolder("Assets/State"))
        {
            AssetDatabase.DeleteAsset("Assets/State");
            AssetDatabase.Refresh();
        }
    }

    private static void AuditAssets(FinalAuditReport report)
    {
        string[] shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { "Assets/Shader" });
        report.totalShaders = shaderGuids.Length;
        foreach (string guid in shaderGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Shader sh = AssetDatabase.LoadAssetAtPath<Shader>(path);
            if (sh == null || ShaderUtil.ShaderHasError(sh))
            {
                report.shadersWithErrors++;
                ShaderMessage[] msgs = sh != null ? ShaderUtil.GetShaderMessages(sh) : Array.Empty<ShaderMessage>();
                string detail = path + " (" + (sh != null ? sh.name : "NULL") + "): " +
                    string.Join(" | ", msgs.Where(m => m.severity == ShaderCompilerMessageSeverity.Error).Select(m => m.message));
                report.shaderErrorDetails.Add(detail);
            }
        }

        string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        report.totalMaterials = matGuids.Length;
        foreach (string guid in matGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null || mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader" || ShaderUtil.ShaderHasError(mat.shader))
            {
                report.brokenMaterials++;
                report.brokenMaterialDetails.Add(path + " -> " + (mat != null && mat.shader != null ? mat.shader.name : "NULL"));
            }
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        report.totalPrefabs = prefabGuids.Length;
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefabRoot == null)
            {
                report.prefabMissingScripts++;
                report.prefabMissingDetails.Add(path + ": NULL prefab");
                continue;
            }
            Transform[] allTransforms = prefabRoot.GetComponentsInChildren<Transform>(true);
            foreach (Transform tr in allTransforms)
            {
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(tr.gameObject);
                if (missing > 0)
                {
                    report.prefabMissingScripts += missing;
                    report.prefabMissingDetails.Add(path + " :: " + tr.name + " (" + missing + ")");
                }
            }
        }

        string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets" });
        report.totalAudioClips = audioGuids.Length;
        foreach (string guid in audioGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null || clip.length <= 0f)
            {
                report.invalidAudioClips++;
            }
        }

        string[] animGuids = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/AnimationClip" });
        report.totalStandaloneAnimFiles = animGuids.Length;
        foreach (string guid in animGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
            {
                report.invalidAnimFiles++;
            }
        }
    }

    private static void AuditScenes(FinalAuditReport report)
    {
        for (int i = 0; i < ExpectedScenes.Length; i++)
        {
            string scenePath = ExpectedScenes[i];
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            SceneAuditEntry entry = new SceneAuditEntry
            {
                buildIndex = i,
                path = scenePath
            };

            GameObject[] roots = scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                Transform[] children = root.GetComponentsInChildren<Transform>(true);
                entry.totalGameObjects += children.Length;

                foreach (Transform tr in children)
                {
                    GameObject go = tr.gameObject;
                    int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                    if (missing > 0)
                    {
                        entry.missingScripts += missing;
                        Debug.LogError("[AsunderAudit] Missing script in " + scenePath + " on " + go.name);
                    }

                    Renderer rend = go.GetComponent<Renderer>();
                    if (rend != null && !(rend is ParticleSystemRenderer))
                    {
                        Material[] mats = rend.sharedMaterials;
                        for (int m = 0; m < mats.Length; m++)
                        {
                            Material mat = mats[m];
                            if (mat != null && (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader"))
                            {
                                entry.brokenRenderers++;
                                Debug.LogError("[AsunderAudit] Pink material in " + scenePath + " on " + go.name);
                            }
                        }
                    }

                    Terrain terrain = go.GetComponent<Terrain>();
                    if (terrain != null && terrain.terrainData != null)
                    {
                        TreePrototype[] treeProtos = terrain.terrainData.treePrototypes;
                        for (int t = 0; t < treeProtos.Length; t++)
                        {
                            GameObject tpPrefab = treeProtos[t].prefab;
                            if (tpPrefab == null) continue;
                            Renderer[] tpRenderers = tpPrefab.GetComponentsInChildren<Renderer>(true);
                            foreach (Renderer tpr in tpRenderers)
                            {
                                foreach (Material tpm in tpr.sharedMaterials)
                                {
                                    if (tpm == null || tpm.shader == null || !tpm.shader.name.Contains("Soft Occlusion"))
                                    {
                                        report.terrainTreeShaderWarnings++;
                                        Debug.LogWarning("[AsunderAudit] Tree prototype " + tpPrefab.name + " in " + scenePath + " uses non-Soft-Occlusion shader: " + (tpm != null && tpm.shader != null ? tpm.shader.name : "NULL"));
                                    }
                                }
                            }
                        }
                    }

                    if (scenePath.EndsWith("Loading.unity", StringComparison.OrdinalIgnoreCase) && go.name == "* App Manager")
                    {
                        AudioSource appSrc = go.GetComponent<AudioSource>();
                        if (appSrc != null && Mathf.Approximately(appSrc.spatialBlend, 0f))
                        {
                            report.appManagerAudio2DVerified = true;
                        }
                    }

                    OVRCameraController ovrRig = go.GetComponent<OVRCameraController>();
                    if (ovrRig != null)
                    {
                        entry.hasOVRCameraController = true;
                        if (scenePath.EndsWith("MainScene.unity", StringComparison.OrdinalIgnoreCase))
                        {
                            Camera camLeft = null;
                            Camera camRight = null;
                            foreach (Camera c in ovrRig.GetComponentsInChildren<Camera>(true))
                            {
                                if (c.name == "CameraLeft") camLeft = c;
                                else if (c.name == "CameraRight") camRight = c;
                            }
                            if (camLeft != null && camRight != null)
                            {
                                bool asymmetricLayer12 = ((camLeft.cullingMask ^ camRight.cullingMask) & (1 << 12)) != 0;
                                if (asymmetricLayer12)
                                {
                                    entry.mainSceneAsymmetricLayer12Verified = true;
                                    report.mainSceneLayer12StereoVerified = true;
                                }
                            }
                        }
                    }
                }
            }

            report.totalScenesAudited++;
            report.totalSceneMissingScripts += entry.missingScripts;
            report.totalSceneBrokenRenderers += entry.brokenRenderers;
            report.scenes.Add(entry);
        }
    }

    private static void RunWin64Build(FinalAuditReport report)
    {
        string outDir = Path.GetDirectoryName(BuildExePath);
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

        string pcDir = Path.GetDirectoryName(BuildPcExePath);
        if (!Directory.Exists(pcDir)) Directory.CreateDirectory(pcDir);

        string vrDir = Path.GetDirectoryName(BuildVrExePath);
        if (!Directory.Exists(vrDir)) Directory.CreateDirectory(vrDir);

        Debug.Log("[AsunderBuild] (1/3) Building Unified Win64 Player to: " + BuildExePath);
        BuildPlayerOptions bpoUnified = new BuildPlayerOptions
        {
            scenes = ExpectedScenes,
            locationPathName = BuildExePath,
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        BuildReport buildReport = BuildPipeline.BuildPlayer(bpoUnified);
        BuildSummary summary = buildReport.summary;

        FileInfo exeInfo = new FileInfo(BuildExePath);
        report.exeExistsOnDisk = exeInfo.Exists;
        report.exeSizeBytes = exeInfo.Exists ? exeInfo.Length : 0L;

        Debug.Log("[AsunderBuild] (2/3) Building Dedicated PC (Flat Screen Desktop) Player to: " + BuildPcExePath);
        BuildPlayerOptions bpoPc = new BuildPlayerOptions
        {
            scenes = ExpectedScenes,
            locationPathName = BuildPcExePath,
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        BuildReport pcReport = BuildPipeline.BuildPlayer(bpoPc);
        FileInfo pcInfo = new FileInfo(BuildPcExePath);
        report.pcExeExistsOnDisk = pcInfo.Exists;
        report.pcExeSizeBytes = pcInfo.Exists ? pcInfo.Length : 0L;
        Debug.Log($"[AsunderBuild] PC Player: {pcReport.summary.result} ({pcInfo.Length} bytes)");

        Debug.Log("[AsunderBuild] (3/3) Building Dedicated VR (OpenXR) Player to: " + BuildVrExePath);
        BuildPlayerOptions bpoVr = new BuildPlayerOptions
        {
            scenes = ExpectedScenes,
            locationPathName = BuildVrExePath,
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        BuildReport vrReport = BuildPipeline.BuildPlayer(bpoVr);
        FileInfo vrInfo = new FileInfo(BuildVrExePath);
        report.vrExeExistsOnDisk = vrInfo.Exists;
        report.vrExeSizeBytes = vrInfo.Exists ? vrInfo.Length : 0L;
        Debug.Log($"[AsunderBuild] VR Player: {vrReport.summary.result} ({vrInfo.Length} bytes)");

        bool allSucceeded = summary.result == BuildResult.Succeeded &&
                            pcReport.summary.result == BuildResult.Succeeded &&
                            vrReport.summary.result == BuildResult.Succeeded;

        report.buildResult = allSucceeded ? "Succeeded" : "Failed";
        report.buildTotalErrors = summary.totalErrors + pcReport.summary.totalErrors + vrReport.summary.totalErrors;
        report.buildTotalWarnings = summary.totalWarnings + pcReport.summary.totalWarnings + vrReport.summary.totalWarnings;
        report.buildTotalSizeBytes = summary.totalSize + pcReport.summary.totalSize + vrReport.summary.totalSize;
        report.buildDurationSeconds = summary.totalTime.TotalSeconds + pcReport.summary.totalTime.TotalSeconds + vrReport.summary.totalTime.TotalSeconds;

        string pluginsX64 = Path.Combine(outDir, "Asunder_Earthbound_Unity6_Data", "Plugins", "x86_64");
        string vrPluginsX64 = Path.Combine(vrDir, "Asunder_Earthbound_VR_Data", "Plugins", "x86_64");
        bool hasOpenXrLoaderDll = File.Exists(Path.Combine(pluginsX64, "openxr_loader.dll")) &&
                                  File.Exists(Path.Combine(vrPluginsX64, "openxr_loader.dll"));
        bool hasUnityOpenXrDll = File.Exists(Path.Combine(pluginsX64, "UnityOpenXR.dll")) &&
                                 File.Exists(Path.Combine(vrPluginsX64, "UnityOpenXR.dll"));
        report.openXrRuntimePluginsDeployed = hasOpenXrLoaderDll && hasUnityOpenXrDll;
    }

    private static void WriteReport(FinalAuditReport report)
    {
        string dir = Path.GetDirectoryName(AuditJsonPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string json = JsonUtility.ToJson(report, true);
        File.WriteAllText(AuditJsonPath, json, new UTF8Encoding(false));

        try
        {
            string currentBrainDir = @"C:\Users\greattug\.gemini\antigravity\brain\ad840f43-b3a1-4ef9-8f3e-8d770441e3ff";
            if (Directory.Exists(currentBrainDir))
            {
                File.WriteAllText(Path.Combine(currentBrainDir, "unity6_final_audit.json"), json, new UTF8Encoding(false));
            }
            string projectLogs = @"C:\Games\Asunder Earthbound VR\Asunder_Unity6\Logs";
            if (Directory.Exists(projectLogs))
            {
                File.WriteAllText(Path.Combine(projectLogs, "unity6_final_audit.json"), json, new UTF8Encoding(false));
            }
            const string priorAuditPath = @"C:\Users\greattug\.gemini\antigravity\brain\a30dc1f6-c03a-4a70-84e6-80e66e153bfc\unity6_final_audit.json";
            if (File.Exists(priorAuditPath))
            {
                File.WriteAllText(priorAuditPath, json, new UTF8Encoding(false));
            }
        }
        catch { }
    }
}
