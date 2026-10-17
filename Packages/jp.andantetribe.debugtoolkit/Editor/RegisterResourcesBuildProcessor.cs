#nullable enable

using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace DebugToolkit.Editor
{
    internal sealed class RegisterResourcesBuildProcessor :
#if UNITY_6000_3_OR_NEWER
        IPreprocessBuildWithContext, IPostprocessBuildWithContext
#else
        IPreprocessBuildWithReport, IPostprocessBuildWithReport
#endif
    {
        int IOrderedCallback.callbackOrder => 0;

#if UNITY_6000_3_OR_NEWER
        void IPreprocessBuildWithContext.OnPreprocessBuild(BuildCallbackContext context)
        {
            if (context.IsPlayerBuild)
            {
                Register();
            }
        }

        void IPostprocessBuildWithContext.OnPostprocessBuild(BuildCallbackContext context)
        {
            if (context.IsPlayerBuild)
            {
                Unregister();
            }
        }
#else
        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport _)
        {
            Register();
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport _)
        {
            Unregister();
        }
#endif

        private static void Register()
        {
            var prevAssets = PlayerSettings.GetPreloadedAssets();
            var assets = new UnityEngine.Object[prevAssets.Length + 1];
            prevAssets.AsSpan().CopyTo(assets);
            assets[^1] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ExternalResources.RootPath + "/ExternalResourcesManager.asset");
            PlayerSettings.SetPreloadedAssets(assets);
        }

        private static void Unregister()
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