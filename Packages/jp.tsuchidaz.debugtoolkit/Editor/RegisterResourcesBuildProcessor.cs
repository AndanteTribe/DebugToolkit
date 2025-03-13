#nullable enable

using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace DebugToolkit.Editor
{
    internal sealed class RegisterResourcesBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 0;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport _)
        {
            var prevAssets = PlayerSettings.GetPreloadedAssets();
            var assets = new UnityEngine.Object[prevAssets.Length + 1];
            prevAssets.AsSpan().CopyTo(assets);
            assets[^1] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ExternalResources.rootPath + "/ExternalResourcesManager.asset");
            PlayerSettings.SetPreloadedAssets(assets);
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport _)
        {
            var prevAssets = PlayerSettings.GetPreloadedAssets();
            var assets = new UnityEngine.Object[prevAssets.Length - 1];
            var index = Array.FindIndex(prevAssets, static asset => asset.name == "ExternalResourcesManager");
            prevAssets.AsSpan(0, index).CopyTo(assets);
            prevAssets.AsSpan(index + 1).CopyTo(assets.AsSpan(index));
            PlayerSettings.SetPreloadedAssets(assets);
        }
    }
}