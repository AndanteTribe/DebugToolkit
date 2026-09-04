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

        /// <summary>
        /// Colors that have to be applied from C# because they depend on runtime state.
        /// These mirror the tokens in ExternalResources/Parts/Variables.uss:
        /// change one and the other has to follow.
        /// </summary>
        public static class StyleColor
        {
            /// <summary>Background of a normal log row. Transparent, so the row striping shows through. (--debug-toolkit-color-log)</summary>
            public static readonly UnityEngine.UIElements.StyleColor Log = new Color(0f, 0f, 0f, 0f);

            /// <summary>Background of a warning log row. (--debug-toolkit-color-log-warning)</summary>
            public static readonly UnityEngine.UIElements.StyleColor LogWarning = new Color(107f / 255f, 89f / 255f, 41f / 255f, 0.55f);

            /// <summary>Background of an error log row. (--debug-toolkit-color-log-error)</summary>
            public static readonly UnityEngine.UIElements.StyleColor LogError = new Color(115f / 255f, 51f / 255f, 51f / 255f, 0.60f);

            /// <summary>Accent color, used for transient feedback. (--debug-toolkit-color-accent)</summary>
            public static readonly UnityEngine.UIElements.StyleColor Accent = new Color(120f / 255f, 160f / 255f, 185f / 255f);

            /// <summary>Toggle color of a window that is currently shown. (--debug-toolkit-color-success)</summary>
            public static readonly UnityEngine.UIElements.StyleColor WindowVisible = new Color(102f / 255f, 143f / 255f, 110f / 255f);

            /// <summary>Toggle color of a window that is currently hidden. (--debug-toolkit-color-danger)</summary>
            public static readonly UnityEngine.UIElements.StyleColor WindowHidden = new Color(153f / 255f, 92f / 255f, 92f / 255f);
        }
    }
}
