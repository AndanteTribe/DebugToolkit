#nullable enable

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;

namespace DebugToolkit.Editor
{
    internal static class BuildFontFallbacks
    {
        private const string Folder = "Assets/DebugToolkitGeneratedBuildAssets";
        private static bool s_createdFolder;

        internal static UnityEngine.Object CreateResources()
        {
            if (AssetDatabase.IsValidFolder(Folder))
            {
                throw new InvalidOperationException($"[DebugToolkit] {Folder} already exists. Refusing to overwrite it.");
            }

            var source = AssetDatabase.LoadAssetAtPath<ExternalResources>(ExternalResources.RootPath + ExternalResources.ManagerAssetPath);
            var fontPaths = AssetDatabase.FindAssets("t:Font glob:\"**/*.[tToO][tT][fF]\"");
            if (fontPaths.Length == 0)
            {
                return source;
            }

            for (var i = 0; i < fontPaths.Length; i++)
            {
                fontPaths[i] = AssetDatabase.GUIDToAssetPath(fontPaths[i]);
            }

            Array.Sort(fontPaths, StringComparer.Ordinal);

            AssetDatabase.CreateFolder("Assets", "DebugToolkitGeneratedBuildAssets");
            s_createdFolder = true;

            var resources = source.Clone();
            var settings = resources.FillPanelTextSettings(forceCreate: true);
            AssetDatabase.CreateAsset(resources, Folder + ExternalResources.ManagerAssetPath);
            AssetDatabase.CreateAsset(resources.PanelSettings, Folder + ExternalResources.PanelSettingsAssetPath);
            AssetDatabase.CreateAsset(settings, Folder + ExternalResources.TextSettingsAssetPath);

            for (var i = 0; i < fontPaths.Length; i++)
            {
                var font = AssetDatabase.LoadAssetAtPath<Font>(fontPaths[i]);
                if (font == null)
                {
                    continue;
                }

                var asset = FontAsset.CreateFontAsset(
                    font,
                    90,
                    9,
                    GlyphRenderMode.SDFAA,
                    1024,
                    1024,
                    AtlasPopulationMode.Dynamic,
                    true);
                if (asset == null)
                {
                    Debug.LogWarning("[DebugToolkit] could not create a dynamic font asset from " + fontPaths[i]);
                    continue;
                }

                AssetDatabase.CreateAsset(asset, Folder + "/Font" + i + ".asset");
                // CreateFontAsset creates these in memory; they must be sub-assets to survive a player build.
                AssetDatabase.AddObjectToAsset(asset.material, asset);
                foreach (var texture in asset.atlasTextures)
                {
                    AssetDatabase.AddObjectToAsset(texture, asset);
                }

                settings.fallbackFontAssets ??= new List<FontAsset>(fontPaths.Length);
                settings.fallbackFontAssets.Add(asset);
            }

            if (settings.fallbackFontAssets == null || settings.fallbackFontAssets.Count == 0)
            {
                Cleanup();
                return source;
            }

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            return resources;
        }

        internal static void Cleanup()
        {
            if (s_createdFolder)
            {
                AssetDatabase.DeleteAsset(Folder);
                s_createdFolder = false;
            }
        }
    }
}