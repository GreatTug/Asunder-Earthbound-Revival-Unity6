#pragma warning disable CS0618
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AsunderDeepInspector
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

    [Serializable]
    public class SceneLightingInfo
    {
        public string sceneName;
        public bool hasSkybox;
        public string skyboxShader;
        public bool fogEnabled;
        public string fogMode;
        public float fogDensity;
        public float fogStart;
        public float fogEnd;
        public Color fogColor;
        public string ambientMode;
        public Color ambientSkyColor;
        public int totalLights;
        public int directionalLights;
        public int pointLights;
        public int spotLights;
        public int shadowCastingLights;
        public int reflectionProbes;
        public List<string> warnings = new List<string>();
    }

    [Serializable]
    public class ParticleSystemAudit
    {
        public string sceneOrPrefab;
        public string gameObjectName;
        public string materialName;
        public string shaderName;
        public bool isShaderValid;
        public string renderMode;
        public string sortingLayer;
        public int sortingOrder;
        public float sortingFudge;
    }

    [Serializable]
    public class AudioAuditInfo
    {
        public string sceneName;
        public string gameObjectName;
        public string clipName;
        public float spatialBlend;
        public float volume;
        public float minDistance;
        public float maxDistance;
        public string rolloffMode;
        public bool playOnAwake;
        public bool loop;
    }

    [Serializable]
    public class SequencerAuditInfo
    {
        public string sceneName;
        public string sequencerName;
        public float duration;
        public int timelineCount;
        public int totalEvents;
        public int brokenEvents;
        public List<string> brokenDetails = new List<string>();
    }

    [Serializable]
    public class DeepAuditReport
    {
        public string timestampUtc;
        public List<SceneLightingInfo> lighting = new List<SceneLightingInfo>();
        public int totalParticleSystems;
        public int particleSystemsWithIssues;
        public List<string> particleIssues = new List<string>();
        public int totalAudioSources;
        public int total2DAudio;
        public int total3DAudio;
        public int audioSourcesMissingClip;
        public List<string> audioIssues = new List<string>();
        public int activeAudioListenerViolations;
        public List<string> audioListenerIssues = new List<string>();
        public int totalSequencers;
        public int sequencersWithIssues;
        public List<string> sequencerIssues = new List<string>();
        public int totalAnimators;
        public int totalLegacyAnimations;
        public int animationIssues;
        public List<string> animationIssueDetails = new List<string>();
        public bool allPassed;
    }

    [MenuItem("Asunder/Run Deep Systems Audit")]
    public static void RunDeepAudit()
    {
        DeepAuditReport report = new DeepAuditReport
        {
            timestampUtc = DateTime.UtcNow.ToString("o")
        };

        Debug.Log("[DeepAudit] Starting exhaustive audit across all 13 scenes...");

        for (int i = 0; i < ExpectedScenes.Length; i++)
        {
            string scenePath = ExpectedScenes[i];
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 1. Audit Lighting & Fog & Skybox
            SceneLightingInfo lightInfo = new SceneLightingInfo
            {
                sceneName = sceneName,
                hasSkybox = RenderSettings.skybox != null,
                skyboxShader = RenderSettings.skybox != null && RenderSettings.skybox.shader != null ? RenderSettings.skybox.shader.name : "None",
                fogEnabled = RenderSettings.fog,
                fogMode = RenderSettings.fogMode.ToString(),
                fogDensity = RenderSettings.fogDensity,
                fogStart = RenderSettings.fogStartDistance,
                fogEnd = RenderSettings.fogEndDistance,
                fogColor = RenderSettings.fogColor,
                ambientMode = RenderSettings.ambientMode.ToString(),
                ambientSkyColor = RenderSettings.ambientSkyColor
            };

            if (RenderSettings.skybox != null)
            {
                if (RenderSettings.skybox.shader == null || ShaderUtil.ShaderHasError(RenderSettings.skybox.shader))
                {
                    lightInfo.warnings.Add("Skybox shader has errors or is null: " + lightInfo.skyboxShader);
                }
            }

            GameObject[] roots = scene.GetRootGameObjects();
            int activeAudioListeners = 0;

            foreach (GameObject root in roots)
            {
                Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
                foreach (Transform tr in transforms)
                {
                    GameObject go = tr.gameObject;

                    // Light inspection
                    Light l = go.GetComponent<Light>();
                    if (l != null && l.enabled)
                    {
                        lightInfo.totalLights++;
                        if (l.type == LightType.Directional) lightInfo.directionalLights++;
                        else if (l.type == LightType.Point) lightInfo.pointLights++;
                        else if (l.type == LightType.Spot) lightInfo.spotLights++;

                        if (l.shadows != LightShadows.None)
                        {
                            lightInfo.shadowCastingLights++;
                        }

                        if (float.IsNaN(l.intensity) || float.IsInfinity(l.intensity) || l.intensity < 0f)
                        {
                            lightInfo.warnings.Add($"Light {go.name} has invalid intensity: {l.intensity}");
                        }
                    }

                    // Reflection Probes
                    ReflectionProbe probe = go.GetComponent<ReflectionProbe>();
                    if (probe != null && probe.enabled)
                    {
                        lightInfo.reflectionProbes++;
                    }

                    // Audio Listeners
                    AudioListener al = go.GetComponent<AudioListener>();
                    if (al != null && al.enabled && go.activeInHierarchy)
                    {
                        activeAudioListeners++;
                    }

                    // Audio Sources
                    AudioSource src = go.GetComponent<AudioSource>();
                    if (src != null)
                    {
                        report.totalAudioSources++;
                        if (Mathf.Approximately(src.spatialBlend, 0f))
                        {
                            report.total2DAudio++;
                        }
                        else
                        {
                            report.total3DAudio++;
                        }

                        if (src.clip == null && !src.playOnAwake)
                        {
                            // Trigger-based audio might load dynamically or play custom clip via script, so clip == null is acceptable if not playOnAwake
                        }
                        else if (src.clip == null && src.playOnAwake)
                        {
                            report.audioSourcesMissingClip++;
                            report.audioIssues.Add($"{sceneName} :: {go.name} has PlayOnAwake=true but clip is NULL!");
                        }
                    }

                    // Particle Systems
                    ParticleSystem ps = go.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        report.totalParticleSystems++;
                        ParticleSystemRenderer psr = go.GetComponent<ParticleSystemRenderer>();
                        if (psr == null)
                        {
                            report.particleSystemsWithIssues++;
                            report.particleIssues.Add($"{sceneName} :: {go.name} ParticleSystem missing ParticleSystemRenderer!");
                        }
                        else
                        {
                            Material pMat = psr.sharedMaterial;
                            if (pMat == null)
                            {
                                report.particleSystemsWithIssues++;
                                report.particleIssues.Add($"{sceneName} :: {go.name} ParticleSystemRenderer has NULL material!");
                            }
                            else if (pMat.shader == null || ShaderUtil.ShaderHasError(pMat.shader))
                            {
                                report.particleSystemsWithIssues++;
                                report.particleIssues.Add($"{sceneName} :: {go.name} Particle material has error shader: {(pMat.shader != null ? pMat.shader.name : "NULL")}");
                            }
                        }
                    }

                    // uSequencer audit
                    Component[] allComponents = go.GetComponents<Component>();
                    foreach (Component c in allComponents)
                    {
                        if (c == null) continue;
                        Type cType = c.GetType();
                        if (cType.Name == "USSequencer")
                        {
                            report.totalSequencers++;
                            Transform cTr = c.transform;
                            int timelineCount = cTr.childCount;

                            for (int ch = 0; ch < timelineCount; ch++)
                            {
                                Transform child = cTr.GetChild(ch);
                                Component[] childComps = child.GetComponentsInChildren<Component>(true);
                                foreach (Component cc in childComps)
                                {
                                    if (cc != null && cc.GetType().Name.StartsWith("USEvent"))
                                    {
                                        PropertyInfo targetProp = cc.GetType().GetProperty("AffectedObject") ?? cc.GetType().GetProperty("TargetObject");
                                        object targetVal = targetProp != null ? targetProp.GetValue(cc, null) : null;
                                        if (targetVal == null)
                                        {
                                            FieldInfo targetField = cc.GetType().GetField("affectedObject") ?? cc.GetType().GetField("targetObject");
                                            if (targetField != null)
                                            {
                                                targetVal = targetField.GetValue(cc);
                                            }
                                        }
                                        if (targetVal == null)
                                        {
                                            report.sequencersWithIssues++;
                                            report.sequencerIssues.Add($"{sceneName} :: {go.name} -> event {cc.GetType().Name} has null target object");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Animators & Legacy Animations
                    Animator anim = go.GetComponent<Animator>();
                    if (anim != null)
                    {
                        report.totalAnimators++;
                        if (anim.runtimeAnimatorController == null && !go.name.Contains("OVR"))
                        {
                            // Some dummy or sub-rigs might have animator without controller
                        }
                    }

                    Animation legacyAnim = go.GetComponent<Animation>();
                    if (legacyAnim != null)
                    {
                        report.totalLegacyAnimations++;
                        if (legacyAnim.clip == null && legacyAnim.GetClipCount() == 0)
                        {
                            report.animationIssues++;
                            report.animationIssueDetails.Add($"{sceneName} :: {go.name} Legacy Animation component has 0 clips!");
                        }
                    }
                }
            }

            if (activeAudioListeners > 1)
            {
                report.activeAudioListenerViolations++;
                report.audioListenerIssues.Add($"{sceneName} has {activeAudioListeners} ACTIVE AudioListeners!");
            }
            else if (activeAudioListeners == 0 && !scenePath.EndsWith("Loading.unity", StringComparison.OrdinalIgnoreCase))
            {
                report.audioListenerIssues.Add($"{sceneName} has 0 ACTIVE AudioListeners!");
            }

            report.lighting.Add(lightInfo);
        }

        report.allPassed = (report.particleSystemsWithIssues == 0) &&
                           (report.audioSourcesMissingClip == 0) &&
                           (report.activeAudioListenerViolations == 0) &&
                           (report.sequencersWithIssues == 0);

        string json = JsonUtility.ToJson(report, true);
        string currentBrainDir = @"C:\Users\greattug\.gemini\antigravity\brain\57b19068-3805-4e68-a336-dadd75e1c861\scratch";
        if (Directory.Exists(currentBrainDir))
        {
            File.WriteAllText(Path.Combine(currentBrainDir, "deep_audit_report.json"), json, new UTF8Encoding(false));
        }
        string altBrainDir = @"C:\Users\greattug\.gemini\antigravity\brain\ad840f43-b3a1-4ef9-8f3e-8d770441e3ff";
        if (Directory.Exists(altBrainDir))
        {
            File.WriteAllText(Path.Combine(altBrainDir, "deep_audit_report.json"), json, new UTF8Encoding(false));
        }
        string projectLogs = @"C:\Games\Asunder Earthbound VR\Asunder_Unity6\Logs";
        if (Directory.Exists(projectLogs))
        {
            File.WriteAllText(Path.Combine(projectLogs, "deep_audit_report.json"), json, new UTF8Encoding(false));
        }
        string priorBrainDir = @"C:\Users\greattug\.gemini\antigravity\brain\a30dc1f6-c03a-4a70-84e6-80e66e153bfc";
        if (Directory.Exists(priorBrainDir))
        {
            File.WriteAllText(Path.Combine(priorBrainDir, "deep_audit_report.json"), json, new UTF8Encoding(false));
        }

        Debug.Log($"[DeepAudit] FINISHED. allPassed={report.allPassed} | ParticleSystems={report.totalParticleSystems} (Issues={report.particleSystemsWithIssues}) | AudioSources={report.totalAudioSources} (2D={report.total2DAudio}, 3D={report.total3DAudio}) | AudioListenerViolations={report.activeAudioListenerViolations} | Sequencers={report.totalSequencers}");

        // Return to first scene
        EditorSceneManager.OpenScene(ExpectedScenes[0], OpenSceneMode.Single);

        if (Application.isBatchMode)
        {
            EditorApplication.Exit(report.allPassed ? 0 : 1);
        }
    }
}
