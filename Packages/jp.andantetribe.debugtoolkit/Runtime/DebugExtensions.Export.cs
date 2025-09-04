#nullable enable

using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    public static partial class DebugExtensions
    {
        /// <summary>
        /// Adds export buttons (JSON and XML) to a debug window.
        /// The buttons will export all debug values from the current window hierarchy to the clipboard.
        /// </summary>
        /// <param name="windowContent">The window content element to add export buttons to</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddExportButtons(this VisualElement windowContent)
        {
            var exportContainer = new VisualElement();
            exportContainer.style.flexDirection = FlexDirection.Row;
            exportContainer.style.marginTop = 10;
            exportContainer.style.marginBottom = 10;

            var exportLabel = new Label("Export:");
            exportLabel.style.marginRight = 10;
            exportLabel.style.alignSelf = Align.Center;
            exportContainer.Add(exportLabel);

            var jsonButton = new Button(() => ExportToJson(windowContent))
            {
                text = "JSON"
            };
            jsonButton.style.marginRight = 5;
            jsonButton.AddToClassList(DebugConst.ClassName + "__export-button");
            exportContainer.Add(jsonButton);

            var xmlButton = new Button(() => ExportToXml(windowContent))
            {
                text = "XML"
            };
            xmlButton.AddToClassList(DebugConst.ClassName + "__export-button");
            exportContainer.Add(xmlButton);

            windowContent.Add(exportContainer);
        }

        /// <summary>
        /// Adds an export button for JSON format to a debug window.
        /// </summary>
        /// <param name="windowContent">The window content element to add the export button to</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddJsonExportButton(this VisualElement windowContent)
        {
            var jsonButton = new Button(() => ExportToJson(windowContent))
            {
                text = "Export to JSON"
            };
            jsonButton.style.marginTop = 10;
            jsonButton.style.marginBottom = 10;
            jsonButton.AddToClassList(DebugConst.ClassName + "__export-button");
            windowContent.Add(jsonButton);
        }

        /// <summary>
        /// Adds an export button for XML format to a debug window.
        /// </summary>
        /// <param name="windowContent">The window content element to add the export button to</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddXmlExportButton(this VisualElement windowContent)
        {
            var xmlButton = new Button(() => ExportToXml(windowContent))
            {
                text = "Export to XML"
            };
            xmlButton.style.marginTop = 10;
            xmlButton.style.marginBottom = 10;
            xmlButton.AddToClassList(DebugConst.ClassName + "__export-button");
            windowContent.Add(xmlButton);
        }

        /// <summary>
        /// Exports debug values to JSON format via DebugExportUtils.
        /// </summary>
        private static void ExportToJson(VisualElement element)
        {
            var root = element.GetSafeAreaContainer();
            DebugExportUtils.ExportToJson(root);
        }

        /// <summary>
        /// Exports debug values to XML format via DebugExportUtils.
        /// </summary>
        private static void ExportToXml(VisualElement element)
        {
            var root = element.GetSafeAreaContainer();
            DebugExportUtils.ExportToXml(root);
        }
    }
}