#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    internal sealed class BindListViewController<TItem, TState> : ListViewController
    {
        private readonly TState _state;
        private readonly Action<TState, VisualElement, TItem> _onBindItem;
        private readonly bool _usePlaceholder;

        public BindListViewController(TState state, Action<TState, VisualElement, TItem> onBindItem, bool usePlaceholder = true)
        {
            _state = state;
            _onBindItem = onBindItem;
            _usePlaceholder = usePlaceholder;
        }

        protected override void BindItem(VisualElement element, int index)
        {
            base.BindItem(element, index);

            if (listView?.itemsSource is IReadOnlyList<TItem> source && 0 <= index && index < source.Count)
            {
                _onBindItem(_state, element, source[index]);
                return;
            }

            if (_usePlaceholder)
            {
                element.style.backgroundColor = DebugConst.StyleColor.White;
                if (element is Label label)
                {
                    label.text = "";
                }
                foreach (var childLabel in element.Query<Label>().Build())
                {
                    childLabel.text = "";
                }
            }
        }
    }
}