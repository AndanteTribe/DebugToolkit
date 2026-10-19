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
        private static UnityEngine.Object[]? s_previousAssets;

#if UNITY_6000_3_OR_NEWER
        void IPreprocessBuildWithContext.OnPreprocessBuild(BuildCallbackContext context)
        {
            if (context.IsPlayerBuild)
            {
                Prepare(context.Report.summary.platform);
            }
        }

        void IPostprocessBuildWithContext.OnPostprocessBuild(BuildCallbackContext context)
        {
            if (context.IsPlayerBuild)
            {
                Restore();
            }
        }

#else
        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            Prepare(report.summary.platform);
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport _)
        {
            Restore();
        }
#endif

        private static void Prepare(BuildTarget target)
        {
            try
            {
                s_previousAssets = PlayerSettings.GetPreloadedAssets();
                Register(ShouldRegisterFontFallbacks(target) ? BuildFontFallbacks.CreateResources() : null);
            }
            catch
            {
                Restore();
                throw;
            }
        }

        // Keep the policy separate so additional platforms or user settings can be supported here later.
        private static bool ShouldRegisterFontFallbacks(BuildTarget target) => target == BuildTarget.WebGL;

        private static void Restore()
        {
            try
            {
                if (s_previousAssets != null)
                {
                    PlayerSettings.SetPreloadedAssets(s_previousAssets);
                }
            }
            finally
            {
                s_previousAssets = null;
                BuildFontFallbacks.Cleanup();
            }
        }

        private static void Register(UnityEngine.Object? resources = null)
        {
            var prevAssets = PlayerSettings.GetPreloadedAssets();
            var assets = new UnityEngine.Object[prevAssets.Length + 1];
            prevAssets.AsSpan().CopyTo(assets);
            assets[^1] = resources != null ? resources : AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ExternalResources.RootPath + ExternalResources.ManagerAssetPath);
            PlayerSettings.SetPreloadedAssets(assets);
        }
    }
}