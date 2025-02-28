#nullable enable

using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    /// <summary>
    /// 実機デバッグメニューの実装をするための基底クラス.
    /// </summary>
    public abstract class DebugViewerBase
    {
        /// <summary>
        /// パネル設定.
        /// </summary>
        public PanelSettings? panelSettings { get; set; }

        /// <summary>
        /// tss.
        /// </summary>
        public ThemeStyleSheet? themeStyleSheet { get; set; }

        /// <summary>
        /// エントリーポイント.
        /// </summary>
        /// <returns>ルート要素.</returns>
        public virtual VisualElement Start()
        {
            var obj = new GameObject("DebugToolkit");
            Object.DontDestroyOnLoad(obj);
            var uiDocument = obj.AddComponent<UIDocument>();
            if (panelSettings == null)
            {
                panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                if (themeStyleSheet == null)
                {
                    themeStyleSheet = Resources.Load<ThemeStyleSheet>("DefaultRuntimeTheme");
                }
                panelSettings.themeStyleSheet = themeStyleSheet;
                panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                panelSettings.screenMatchMode = PanelScreenMatchMode.Expand;
            }
            uiDocument.panelSettings = panelSettings;

            var root = uiDocument.rootVisualElement;
            var safeAreaContainer = new SafeAreaContainer();
            root.Add(safeAreaContainer);
            return safeAreaContainer;
        }
    }
}
