using System;
using System.Drawing;
using System.Windows.Forms;
using siteview.views;

namespace siteview.mouse
{
    public class RulerMeasureMode : MapIdleMode
    {
        protected bool MouseDragging = false;

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
                StartDragging(map, maploc);

            if (!MouseDragging)
                return;

            map.RulerMeasure = new RectangleF(mapDownPoint.X, mapDownPoint.Y, maploc.X - mapDownPoint.X, maploc.Y - mapDownPoint.Y);
        }

        protected virtual void StartDragging(MapView map, PointF maploc)
        {
            MouseDragging = true;
            map.RulerMeasure = new RectangleF(mapDownPoint.X, mapDownPoint.Y, maploc.X - mapDownPoint.X, maploc.Y - mapDownPoint.Y);
            map.RulerMeasureIsVisible = true;
        }

        public override void OnMouseUp(object sender, MouseEventArgs e)
        {
            var map = sender as MapView;
            map.RulerMeasureIsVisible = MouseDragging;
            MouseDragging = MouseDown = false;
        }

        public override void OnMouseLeave(object sender, EventArgs e) => MouseDragging = MouseDown = false;

    }
}
