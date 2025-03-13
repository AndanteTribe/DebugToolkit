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
        /// タブが追加される親要素.
        /// </summary>
        private VisualElement _tabView;

        public void Start() => Setup();

        /// <summary>
        /// エントリーポイント.
        /// </summary>
        /// <returns>ルート要素.</returns>
        protected virtual VisualElement Setup()
        {
            var obj = new GameObject("DebugToolkit");
            Object.DontDestroyOnLoad(obj);
            var uiDocument = obj.AddComponent<UIDocument>();
            if (panelSettings == null)
            {
                panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                if (themeStyleSheet == null)
                {
                    themeStyleSheet = ExternalResources.LoadThemeStyleSheet();
                }
                panelSettings.themeStyleSheet = themeStyleSheet;
                panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                panelSettings.screenMatchMode = PanelScreenMatchMode.Expand;
            }
            uiDocument.panelSettings = panelSettings;

            var root = uiDocument.rootVisualElement;
            var safeAreaContainer = new SafeAreaContainer();
            root.Add(safeAreaContainer);

            var window = new VisualElement();
            window.AddToClassList("debug-toolkit-master");
            safeAreaContainer.Add(window);

            var manipulator = new DragManipulator(window);
            var dragArea = new VisualElement(){ name = "drag-area" };
            dragArea.AddToClassList("unity-foldout__drag-area");
            dragArea.AddManipulator(manipulator);

            var foldout = new Foldout { text = "DebugMenu", value = true, name = "DebugMenuHandler" };
            foldout.Q<Toggle>().Add(dragArea);
            window.Add(foldout);

            _tabView = new TabView();
            foldout.Add(_tabView);

            return _tabView;
        }

        /// <summary>
        /// デバックメニューにタブを追加するメソッド.
        /// </summary>
        /// <param name="label">
        /// タブのラベル.
        /// </param>
        /// <returns>
        /// ユーザが要素を追加するルート要素.
        /// </returns>
        protected VisualElement AddTab(string label)
        {
            var tab = new Tab { label = label };
            _tabView.Add(tab);
                
            var scrollView = new ScrollView();
            tab.Add(scrollView);

            return scrollView;
        }
    }
}
