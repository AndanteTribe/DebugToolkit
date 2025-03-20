#nullable enable

using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    internal sealed class DragManipulator : MouseManipulator
    {
        private readonly VisualElement _moveTarget;
        private Vector2 _targetStartPosition;
        private Vector3 _pointerStartPosition;
        private bool _enabled;
        
        public DragManipulator(VisualElement moveTarget)
        {
            this._moveTarget = moveTarget;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler, TrickleDown.TrickleDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler, TrickleDown.TrickleDown);
        }

        private void PointerDownHandler(PointerDownEvent e)
        {
            _targetStartPosition = _moveTarget.transform.position;
            _pointerStartPosition = e.position;
            target.CapturePointer(e.pointerId);
            _enabled = true;
        }

        private void PointerMoveHandler(PointerMoveEvent e)
        {
            if (_enabled && target.HasPointerCapture(e.pointerId))
            {
                var pointerDelta = e.position - _pointerStartPosition;
                _moveTarget.transform.position = new Vector2(_targetStartPosition.x + pointerDelta.x, _targetStartPosition.y + pointerDelta.y);
            }
        }

        private void PointerUpHandler(PointerUpEvent e)
        {
            if (_enabled && target.HasPointerCapture(e.pointerId))
            {
                target.ReleasePointer(e.pointerId);
            }
        }

        private void PointerCaptureOutHandler(PointerCaptureOutEvent e)
        {
            _enabled = false;
        }
    }
}
