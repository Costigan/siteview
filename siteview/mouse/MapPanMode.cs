using siteview.views;
using System.Drawing;
using System.Windows.Forms;

namespace siteview.mouse
{
    public class MapPanMode : MouseMode
    {
        public MapView Map;
        public Point MouseDownPoint;
        int offsetX, offsetY;

        public MapPanMode(MapView map, Point mouseDown)
        {
            Map = map;
            MouseDownPoint = mouseDown;
            offsetX = map.Transform.OffsetX;
            offsetY = map.Transform.OffsetY;
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            MapView.DisplayTransform transform = Map.Transform;
            transform.OffsetX = e.Location.X - MouseDownPoint.X + offsetX;
            transform.OffsetY = e.Location.Y - MouseDownPoint.Y + offsetY;
            Map.Transform = transform;
        }

        public override void OnMouseUp(object sender, MouseEventArgs e)
        {
            Map.EndPanZoom();
            Map.ReplaceMouseMode(new MapIdleMode());
        }
    }
}
