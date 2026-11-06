using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AsunderScreenshotCapture
{
    [MenuItem("Asunder/Capture MainScene Screenshot")]
    public static void CaptureScreenshot()
    {
        string scenePath = "Assets/SCENES/Main/MainScene.unity";
        Debug.Log("[Screenshot] Opening scene: " + scenePath);
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        Camera targetCam = null;
        Camera[] cameras = GameObject.FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (var cam in cameras)
        {
            if (cam.gameObject.name.Contains("Camera") || cam.gameObject.name.Contains("Eye") || cam.CompareTag("MainCamera"))
            {
                if (targetCam == null || cam.gameObject.name.Contains("Center") || cam.gameObject.name.Contains("LeftEye"))
                {
                    targetCam = cam;
                }
            }
        }

        if (targetCam == null)
        {
            targetCam = Camera.main;
        }

        if (targetCam == null && cameras.Length > 0)
        {
            targetCam = cameras[0];
        }

        if (targetCam == null)
        {
            Debug.LogError("[Screenshot] No camera found in scene!");
            return;
        }

        Debug.Log("[Screenshot] Using camera: " + targetCam.gameObject.name + " at position " + targetCam.transform.position);

        int width = 1920;
        int height = 1080;
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture oldRt = targetCam.targetTexture;
        RenderTexture oldActive = RenderTexture.active;

        targetCam.targetTexture = rt;
        targetCam.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        targetCam.targetTexture = oldRt;
        RenderTexture.active = oldActive;
        GameObject.DestroyImmediate(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        GameObject.DestroyImmediate(screenShot);

        string outDir = Path.Combine(Directory.GetCurrentDirectory(), "DOCS", "images");
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
        string outPath = Path.Combine(outDir, "asunder_ingame_cabin.png");
        File.WriteAllBytes(outPath, bytes);
        Debug.Log("[Screenshot] Saved in-game screenshot to: " + outPath + " (" + bytes.Length + " bytes)");
    }
}
