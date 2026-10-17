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

        internal PanelSettings? PanelSettings => _panelSettings;

        public const string RootPath = "Packages/jp.andantetribe.debugtoolkit/ExternalResources";
        public const string ManagerAssetPath = "/ExternalResourcesManager.asset";
        public const string PanelSettingsAssetPath = "/PanelSettings.asset";
        public const string TextSettingsAssetPath = "/TextSettings.asset";

#if !UNITY_EDITOR
        private static ExternalResources s_instance;

        private void OnEnable()
        {
            s_instance = this;
        }
#endif

        public ExternalResources Clone()
        {
            var resources = Instantiate(this);
            resources._panelSettings = _panelSettings == null ? null : Instantiate(_panelSettings);
            return resources;
        }

        public PanelTextSettings FillPanelTextSettings(bool forceCreate = false)
        {
            if (_panelSettings == null)
            {
                _panelSettings = CreateInstance<PanelSettings>();
            }

            if (_panelSettings.textSettings == null)
            {
                _panelSettings.textSettings = CreateInstance<PanelTextSettings>();
            }
            else if (forceCreate)
            {
                _panelSettings.textSettings = Instantiate(_panelSettings.textSettings);
            }
            return _panelSettings.textSettings;
        }

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
            return UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>(RootPath + PanelSettingsAssetPath);
#else
            return s_instance._panelSettings;
#endif
        }
    }
}