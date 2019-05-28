using siteview.views;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace siteview.mouse
{
    public class MouseMode
    {
        public virtual void OnDragEnter(Object sender, DragEventArgs e) { }
        public virtual void OnDragLeave(Object sender, EventArgs e) { }
        public virtual void OnDragOver(Object sender, DragEventArgs e) { }
        public virtual void OnKeyDown(Object sender, KeyEventArgs e) { }
        public virtual void OnKeyPress(Object sender, KeyPressEventArgs e) { }
        public virtual void OnKeyUp(Object sender, KeyEventArgs e) { }
        public virtual void OnMouseClick(Object sender, MouseEventArgs e) { }
        public virtual void OnMouseDoubleClick(Object sender, MouseEventArgs e) { }
        public virtual void OnMouseDown(Object sender, MouseEventArgs e) { }
        public virtual void OnMouseEnter(Object sender, EventArgs e) { }
        public virtual void OnMouseHover(Object sender, EventArgs e) { }
        public virtual void OnMouseLeave(Object sender, EventArgs e) { }
        public virtual void OnMouseMove(Object sender, MouseEventArgs e) { }
        public virtual void OnMouseUp(Object sender, MouseEventArgs e) { }
        public virtual void OnMouseWheel(Object sender, MouseEventArgs e) { }

        public virtual Cursor GetCursor() => Cursors.Default;

        public void ShowStatus(string msg) { StatusManager.ShowStatus(msg); }

        public static int PointDistance(Point a, Point b) => Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));

        public static Rectangle Expand(Rectangle r, int expansion) => new Rectangle(r.Left - expansion, r.Top - expansion, r.Width + expansion + expansion, r.Height + expansion + expansion);

        public static SizeF Subtract(PointF a, PointF b) => new SizeF(a.X - b.X, a.Y - b.Y);
    }
}
