using DebugToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class DebugViewHoge : DebugViewerBase
    {
        protected override VisualElement CreateViewGUI()
        {
            var root = base.CreateViewGUI();
            var conWin = root.AddWindow("Console");
            var logWin = root.AddWindow("LogButtons");
            conWin.AddConsoleView();
            var logButton = new Button() { text = "Log" };
            logButton.RegisterCallback<FocusEvent>(evt => Debug.Log("Button Clicked"));
            var warningButton = new Button() { text = "Warning" };
            warningButton.RegisterCallback<FocusEvent>(evt => Debug.LogWarning("Warning Clicked"));
            var errorButton = new Button() { text = "Error" };
            errorButton.RegisterCallback<FocusEvent>(evt => Debug.LogError("Error Clicked"));
            logWin.Add(logButton);
            logWin.Add(warningButton);
            logWin.Add(errorButton);

            return root;
        }
    }
}