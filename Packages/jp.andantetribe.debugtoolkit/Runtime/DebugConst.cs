#nullable enable

using UnityEngine;

namespace DebugToolkit
{
    public static class DebugConst
    {
        public const string ClassName = "debug-toolkit";
        public const string WindowContentClassName = ClassName + "__window-content";
        public const string WindowLabelClassName = ClassName + "__window-label";
        public const string WindowHeaderClassName = ClassName + "__window-header";
        public const string ToggleWindowDisplayClassName = ClassName + "__toggle-window-display";
        public const string SafeAreaContainerClassName = ClassName + "__safe-area-container";
        public const string MasterWindowClassName = ClassName + "__master-window";
        public const string NormalWindowClassName = ClassName + "__normal-window";

        public static class StyleColor
        {
            public static readonly UnityEngine.UIElements.StyleColor White = Color.white;
            public static readonly UnityEngine.UIElements.StyleColor Warning = new Color(1, 1, 0.6f);
            public static readonly UnityEngine.UIElements.StyleColor Error = new Color(1, 0.4f, 0.4f);
        }
    }
}