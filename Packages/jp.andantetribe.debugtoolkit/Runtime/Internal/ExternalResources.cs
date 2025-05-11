#nullable enable

using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    internal sealed class ExternalResources : ScriptableObject
    {
        [SerializeField]
        private ThemeStyleSheet? _themeStyleSheet;
        [SerializeField]
        private PanelSettings? _panelSettings;

        public const string RootPath = "Packages/jp.andantetribe.debugtoolkit/ExternalResources";

#if !UNITY_EDITOR
        private static ExternalResources s_instance;

        private void OnEnable()
        {
            s_instance = this;
        }
#endif

        public static ThemeStyleSheet LoadThemeStyleSheet()
        {
#if UNITY_EDITOR

            return UnityEditor.AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(RootPath + "/DefaultRuntimeTheme.tss");
#else
            return s_instance._themeStyleSheet;
#endif
        }

        public static PanelSettings LoadPanelSettings()
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>(RootPath + "/PanelSettings.asset");
#else
            return s_instance._panelSettings;
#endif
        }
    }
}