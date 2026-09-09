using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class FolderSpriteWhiteBGEditor : EditorWindow
{
    private DefaultAsset spriteFolder;
    private int padding = 0;

    [MenuItem("Tools/Icon/Folder ? Overwrite White BG")]
    public static void ShowWindow()
    {
        GetWindow<FolderSpriteWhiteBGEditor>("Folder White BG");
    }

    private void OnGUI()
    {
        GUILayout.Label("Overwrite Sprites in Folder (Solid White BG)", EditorStyles.boldLabel);

        spriteFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Sprite Folder",
            spriteFolder,
            typeof(DefaultAsset),
            false
        );

        padding = EditorGUILayout.IntField("Extra Padding (px)", padding);

        EditorGUILayout.HelpBox(
            "? This will permanently modify ALL sprites in the selected folder.\n" +
            "Use version control or backup before running.",
            MessageType.Warning
        );

        GUI.enabled = spriteFolder != null;
        if (GUILayout.Button("PROCESS ALL SPRITES IN FOLDER"))
        {
            ProcessFolder();
        }
        GUI.enabled = true;
    }

    private void ProcessFolder()
    {
        string folderPath = AssetDatabase.GetAssetPath(spriteFolder);

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("? Selected asset is not a valid folder.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

        if (guids.Length == 0)
        {
            Debug.LogWarning("No sprites found in folder.");
            return;
        }

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
                OverwriteSprite(sprite);
        }

        AssetDatabase.Refresh();
        Debug.Log($"? Processed {guids.Length} sprites in folder.");
    }

    private void OverwriteSprite(Sprite sprite)
    {
        string path = AssetDatabase.GetAssetPath(sprite);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        bool wasReadable = importer.isReadable;
        if (!wasReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        Texture2D srcTex = sprite.texture;
        Rect r = sprite.rect;

        int w = (int)r.width + padding * 2;
        int h = (int)r.height + padding * 2;

        Texture2D newTex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        // Fill full texture white
        Color[] pixels = new Color[w * h];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;

        Color[] sp = srcTex.GetPixels(
            (int)r.x,
            (int)r.y,
            (int)r.width,
            (int)r.height
        );

        int sw = (int)r.width;
        int sh = (int)r.height;

        // Alpha flatten
        for (int y = 0; y < sh; y++)
        {
            for (int x = 0; x < sw; x++)
            {
                Color c = sp[y * sw + x];
                Color final = Color.Lerp(Color.white, c, c.a);
                final.a = 1f;

                int fx = x + padding;
                int fy = y + padding;
                pixels[fy * w + fx] = final;
            }
        }

        newTex.SetPixels(pixels);
        newTex.Apply();

        // Overwrite original PNG
        File.WriteAllBytes(path, newTex.EncodeToPNG());

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();

        if (!wasReadable)
        {
            importer.isReadable = false;
            importer.SaveAndReimport();
        }
    }
}
