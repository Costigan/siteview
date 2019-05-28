using siteview.views;
using System.Drawing;
using System.Windows.Forms;
using TraversePlanner.views;

namespace siteview.mouse
{
    public class DragPickableInMap : MouseMode
    {
        public Pickable Pickable;
        public PointF DownPoint;
        public SizeF Offset;

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Pickable == null) return;
            var map = sender as MapView;
            Pickable.Location = new PointF(map.TransformMouseX(e.X) + Offset.Width, map.TransformMouseY(e.Y) + Offset.Height);
            map.Invalidate();
            Pickable?.Dragging(map);
        }

        public override void OnMouseUp(object sender, MouseEventArgs e)
        {
            var panel = (PickablePanel)sender;
            Pickable?.StopDragging(panel);
            ((PickablePanel)sender).ReplaceMouseMode(panel.GetIdleMode());
        }
    }
}
