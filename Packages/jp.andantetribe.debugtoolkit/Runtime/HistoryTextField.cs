#if ENABLE_DEBUGTOOLKIT

using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

namespace DebugToolkit
{
    public class HistoryTextField : TextField
    {
        private readonly Stack<string> _undoStack = new Stack<string>();
        private readonly Stack<string> _redoStack = new Stack<string>();

        private bool _isUndoRedoOperation;

        public HistoryTextField(string label = null) : base(label)
        {
            this.RegisterValueChangedCallback(OnValueChanged);

            #if !ENABLE_INPUT_SYSTEM
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            #endif
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            if (_isUndoRedoOperation)
            {
                return;
            }

            _undoStack.Push(evt.previousValue);
            _redoStack.Clear();
        }

        #if !ENABLE_INPUT_SYSTEM
        private void OnKeyDown(KeyDownEvent evt)
        {
            var isCtrlOrCmd = evt.ctrlKey || evt.commandKey;

            if (isCtrlOrCmd)
            {
                if (evt.keyCode == KeyCode.Z && !evt.shiftKey)
                {
                    Undo();
                    evt.StopPropagation();
                }
                else if (evt.keyCode == KeyCode.Y || (evt.keyCode == KeyCode.Z && evt.shiftKey))
                {
                    Redo();
                    evt.StopPropagation();
                }
            }
        }
        #endif

        private void Undo()
        {
            if (_undoStack.Count > 0)
            {
                _redoStack.Push(value);

                _isUndoRedoOperation = true;
                this.SetValueWithoutNotify(_undoStack.Pop());
                _isUndoRedoOperation = false;
            }
        }

        private void Redo()
        {
            if (_redoStack.Count > 0)
            {
                _undoStack.Push(value);

                _isUndoRedoOperation = true;
                this.SetValueWithoutNotify(_redoStack.Pop());
                _isUndoRedoOperation = false;
            }
        }
    }
}

#endif