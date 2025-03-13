using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    public class DragManipulator : MouseManipulator
    {
        readonly VisualElement moveTarget;
        Vector2 targetStartPosition;
        Vector3 pointerStartPosition;
        bool enabled;
        
        public DragManipulator(VisualElement moveTarget)
        {
            this.moveTarget = moveTarget;
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

        void PointerDownHandler(PointerDownEvent e)
        {
            targetStartPosition = moveTarget.transform.position;
            pointerStartPosition = e.position;
            target.CapturePointer(e.pointerId);
            enabled = true;
        }

        void PointerMoveHandler(PointerMoveEvent e)
        {
            if (enabled && target.HasPointerCapture(e.pointerId))
            {
                var pointerDelta = e.position - pointerStartPosition;
                moveTarget.transform.position = new Vector2(targetStartPosition.x + pointerDelta.x, targetStartPosition.y + pointerDelta.y);
            }
        }

        void PointerUpHandler(PointerUpEvent e)
        {
            if (enabled && target.HasPointerCapture(e.pointerId))
            {
                target.ReleasePointer(e.pointerId);
            }
        }

        void PointerCaptureOutHandler(PointerCaptureOutEvent e)
        {
            enabled = false;
        }
    }
}