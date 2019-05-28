using System;
using System.Drawing;
using System.Windows.Forms;
using siteview.views;

namespace siteview.mouse
{
    public class DragRectangleMode : MouseMode
    {
        public const int StartDragThreshold = 2;

        public int Modulus = 1;

        protected bool MouseDown = false;
        protected bool MouseDragging = false;
        protected Point mouseDownPoint;
        protected PointF mapDownPoint;

        public override void OnMouseDown(Object sender, MouseEventArgs e)
        {
            var map = sender as MapView;
            mouseDownPoint = e.Location;
            mapDownPoint = map.TransformMouse(mouseDownPoint);
            map.Select();
            MouseDown = true;
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!MouseDown) return;
            var map = sender as MapView;
            var maploc = map.Transform.MouseToMap(e.Location);

            if (!MouseDragging && PointDistance(mouseDownPoint, e.Location) > StartDragThreshold)
            {
                MouseDragging = true;
                map.SelectionRectangle = new RectangleF(Floor(maploc), new SizeF(Modulus, Modulus));
                map.SelectionRectangleIsVisible = true;
            }

            if (!MouseDragging)
                return;

            var ul = map.SelectionRectangle.Location;
            var br = Ceiling(maploc);
            var size = new SizeF(br.X - ul.X, br.Y - ul.Y);

            map.SelectionRectangle = new RectangleF(ul, size);
        }

        public override void OnMouseUp(object sender, MouseEventArgs e) => MouseDragging = MouseDown = false;
        public override void OnMouseLeave(object sender, EventArgs e) => MouseDragging = MouseDown = false;

        protected PointF Floor(PointF loc)
        {
            var x = Modulus * (int)(loc.X / Modulus);
            var y = Modulus * (int)(loc.Y / Modulus);
            return new PointF(x, y);
        }

        protected PointF Ceiling(PointF loc)
        {
            var x = Modulus + Modulus * (int)(loc.X / Modulus);
            var y = Modulus + Modulus * (int)(loc.Y / Modulus);
            return new PointF(x, y);
        }
    }
}
