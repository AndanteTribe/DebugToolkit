#if ENABLE_DEBUGTOOLKIT
#nullable enable

using System.Collections.Generic;
using System.Transactions;
using UnityEngine.UIElements;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace DebugToolkit
{
    public class HistoryTextField : TextField
    {
        private readonly Stack<string> _undoStack = new Stack<string>();
        private readonly Stack<string> _redoStack = new Stack<string>();

        private bool _isUndoRedoOperation;

#if ENABLE_INPUT_SYSTEM
        private IVisualElementScheduledItem? _pollingAction = null;
#endif

        public HistoryTextField(string label = "") : base(label)
        {
            this.RegisterValueChangedCallback(OnValueChanged);

#if ENABLE_INPUT_SYSTEM
            RegisterCallback<FocusInEvent>(OnFocusIn);
            RegisterCallback<FocusOutEvent>(OnFocusOut);
#else
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

#if ENABLE_INPUT_SYSTEM
        private void OnFocusIn(FocusInEvent evt) => _pollingAction = this.schedule.Execute(PollKeyboard).Every(0);

        private void OnFocusOut(FocusOutEvent evt) => _pollingAction?.Pause();

        private void PollKeyboard()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            var isCtrlOrCmd = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed ||
                              keyboard.leftCommandKey.isPressed || keyboard.rightCommandKey.isPressed;

            if (isCtrlOrCmd)
            {
                var isShift = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
                if (keyboard.zKey.wasPressedThisFrame && !isShift)
                {
                    Undo();
                }
                else if (keyboard.yKey.wasPressedThisFrame || (keyboard.zKey.wasPressedThisFrame && isShift))
                {
                    Redo();
                }
            }
        }
#else
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