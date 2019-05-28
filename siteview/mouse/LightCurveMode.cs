using siteview.views;
using siteview.viz;
using System.Windows.Forms;

namespace siteview.mouse
{
    public class LightCurveMode : MouseMode
    {
        public LightCurveForm LightCurveForm;
        private bool _isDown;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            var map = sender as MapView;
            var pointf = map.TransformMouse(e.Location);
            LightCurveForm.Point = new System.Drawing.Point((int)pointf.X, (int)pointf.Y);
            _isDown = true;
        }

        public override void OnMouseUp(object sender, MouseEventArgs e)
        {
            _isDown = false;
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDown) return;
            var map = sender as MapView;
            var pointf = map.TransformMouse(e.Location);
            LightCurveForm.Point = new System.Drawing.Point((int)pointf.X, (int)pointf.Y);
        }
    }
}
