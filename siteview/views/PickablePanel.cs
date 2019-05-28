using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using siteview.mouse;

namespace TraversePlanner.views
{
    public class PickablePanel : Panel
    {
        protected const bool _gridSnap = false;
        internal Stack<MouseMode> MouseModeStack = new Stack<MouseMode>();
        public Pickable Selection = null;

        public PickablePanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
            MouseModeStack.Push(new MouseMode());
        }

        #region Pickable Handling

        public virtual IEnumerable<Pickable> Pickables() => Enumerable.Empty<Pickable>();

        #endregion Pickable Handling

        #region Mouse Handling

        public void PushMouseMode(MouseMode m)
        {
            //Console.WriteLine("Enter mouse mode " + m);
            MouseModeStack.Push(m);
            Cursor = m.GetCursor();
        }

        /// <summary>
        /// Replace the top of the mouse mode stack
        /// </summary>
        /// <param name="m"></param>
        public void ReplaceMouseMode(MouseMode m)
        {
            Console.WriteLine("Replace mouse mode " + m);
            MouseModeStack.Pop();
            MouseModeStack.Push(m);
            Cursor = m.GetCursor();
        }

        public MouseMode PopMouseMode()
        {
            MouseMode m = MouseModeStack.Pop();
            //Console.WriteLine("Exit mouse mode " + m);
            MouseMode top = MouseModeStack.Peek();
            if (top != null)
                Cursor = top.GetCursor();
            return m;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            MouseModeStack.Peek().OnKeyDown(this, e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            MouseModeStack.Peek().OnKeyPress(this, e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            MouseModeStack.Peek().OnKeyUp(this, e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            MouseModeStack.Peek().OnMouseClick(this, e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            MouseModeStack.Peek().OnMouseDoubleClick(this, e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseModeStack.Peek().OnMouseDown(this, e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            MouseModeStack.Peek().OnMouseEnter(this, e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            MouseModeStack.Peek().OnMouseHover(this, e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            MouseModeStack.Peek().OnMouseLeave(this, e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            try
            {
                //if (_gridSnap)
                //    MouseModeStack.Peek().OnMouseMove(this, SnappedMouseEvent(e));
                //else
                MouseModeStack.Peek().OnMouseMove(this, e);
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }
        }

        protected MouseEventArgs SnappedMouseEvent(MouseEventArgs e)
        {
            const int resolution = 5;
            int x1 = ((e.X + resolution / 2) / resolution) * resolution;
            int y1 = ((e.Y + resolution / 2) / resolution) * resolution;
            return new MouseEventArgs(e.Button, e.Clicks, x1, y1, e.Delta);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            //if (_gridSnap)
            //    MouseModeStack.Peek().OnMouseUp(this, SnappedMouseEvent(e));
            //else
            MouseModeStack.Peek().OnMouseUp(this, e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            MouseModeStack.Peek().OnMouseWheel(this, e);
        }

        public virtual MouseMode GetIdleMode() => new MapIdleMode();

        #endregion Mouse Handling

        #region Picking and Selection

        public virtual Pickable Pick(PointF pt)
        {
            Pickable r = null;
            const float threshold = 25f;
            float mind2 = float.MaxValue;
            foreach (Pickable p in Pickables())
            {
                float d2 = (p.X - pt.X) * (p.X - pt.X) + (p.Y - pt.Y) * (p.Y - pt.Y);
                if (d2 <= mind2)
                {
                    r = p;
                    mind2 = d2;
                }
            }
            return mind2 <= threshold ? r : null;
        }

        public virtual void Select(Pickable p)
        {
            if (Selection == p)
                return;
            Selection?.Deselect(this);
            Selection = p;
            Selection?.Select(this);
            Selected?.Invoke(this, p);
        }

        public delegate void SelectionEventHandler(PickablePanel panel, Pickable picked);
        public event SelectionEventHandler Selected;

        #endregion Picking and Selection
    }
}
