#pragma warning disable CS0618
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AsunderFixer
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

    [MenuItem("Asunder/Apply All Fixes and Enhancements")]
    public static void ApplyFixes()
    {
        Debug.Log("[AsunderFixer] Starting automated fixes and graphics enhancement pass...");

        int fixedListeners = 0;
        int fixedAudioSources = 0;
        int removedEmptyAnimations = 0;
        int enhancedLights = 0;

        for (int s = 0; s < ExpectedScenes.Length; s++)
        {
            string scenePath = ExpectedScenes[s];
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            bool sceneDirty = false;

            GameObject[] roots = scene.GetRootGameObjects();

            // 1. AudioListener resolution: Find VR listener on CameraRight or primary camera
            AudioListener primaryListener = null;
            List<AudioListener> otherListeners = new List<AudioListener>();

            foreach (GameObject root in roots)
            {
                AudioListener[] listeners = root.GetComponentsInChildren<AudioListener>(true);
                foreach (AudioListener al in listeners)
                {
                    if (al.gameObject.name == "CameraRight")
                    {
                        primaryListener = al;
                    }
                    else
                    {
                        otherListeners.Add(al);
                    }
                }
            }

            if (primaryListener == null && otherListeners.Count > 0)
            {
                // Fallback to first camera listener (e.g. Loading scene)
                primaryListener = otherListeners[0];
                otherListeners.RemoveAt(0);
            }

            if (primaryListener != null)
            {
                if (!primaryListener.enabled)
                {
                    primaryListener.enabled = true;
                    EditorUtility.SetDirty(primaryListener);
                    sceneDirty = true;
                }

                foreach (AudioListener other in otherListeners)
                {
                    if (other != null && other.enabled)
                    {
                        other.enabled = false;
                        EditorUtility.SetDirty(other);
                        fixedListeners++;
                        sceneDirty = true;
                        Debug.Log($"[AsunderFixer] Disabled duplicate AudioListener in {scene.name} on '{other.gameObject.name}'");
                    }
                }
            }

            // 2. AudioSources: Fix PlayOnAwake with null clips
            foreach (GameObject root in roots)
            {
                AudioSource[] sources = root.GetComponentsInChildren<AudioSource>(true);
                foreach (AudioSource src in sources)
                {
                    if (src != null && src.clip == null && src.playOnAwake)
                    {
                        src.playOnAwake = false;
                        EditorUtility.SetDirty(src);
                        fixedAudioSources++;
                        sceneDirty = true;
                        Debug.Log($"[AsunderFixer] Fixed AudioSource PlayOnAwake=false on null clip in {scene.name} on '{src.gameObject.name}'");
                    }
                }
            }

            // 3. Remove empty legacy Animation components with 0 clips
            foreach (GameObject root in roots)
            {
                Animation[] anims = root.GetComponentsInChildren<Animation>(true);
                foreach (Animation anim in anims)
                {
                    if (anim != null && anim.clip == null && anim.GetClipCount() == 0)
                    {
                        // Check if object is a static tree or prop
                        string goName = anim.gameObject.name;
                        UnityEngine.Object.DestroyImmediate(anim, true);
                        removedEmptyAnimations++;
                        sceneDirty = true;
                        Debug.Log($"[AsunderFixer] Removed redundant empty Animation component on '{goName}' in {scene.name}");
                    }
                }
            }

            // 4. Boost Lighting & Shadows
            foreach (GameObject root in roots)
            {
                Light[] lights = root.GetComponentsInChildren<Light>(true);
                foreach (Light l in lights)
                {
                    if (l != null && l.enabled)
                    {
                        if (l.shadows != LightShadows.None)
                        {
                            // Boost to soft shadows for high visual fidelity in VR
                            if (l.shadows == LightShadows.Hard)
                            {
                                l.shadows = LightShadows.Soft;
                                l.shadowNormalBias = 0.4f;
                                l.shadowBias = 0.05f;
                                EditorUtility.SetDirty(l);
                                enhancedLights++;
                                sceneDirty = true;
                            }
                        }
                    }
                }
            }

            if (sceneDirty)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[AsunderFixer] Saved modifications to scene: {scenePath}");
            }
        }

        AssetDatabase.SaveAssets();

        Debug.Log($"[AsunderFixer] PASS COMPLETE! Fixed Listeners: {fixedListeners}, Fixed AudioSources: {fixedAudioSources}, Removed Empty Animations: {removedEmptyAnimations}, Enhanced Lights: {enhancedLights}");

        // Return to first scene
        EditorSceneManager.OpenScene(ExpectedScenes[0], OpenSceneMode.Single);

        if (Application.isBatchMode)
        {
            EditorApplication.Exit(0);
        }
    }
}
