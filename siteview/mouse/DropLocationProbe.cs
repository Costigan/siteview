using siteview.views;
using System.Windows.Forms;

namespace siteview.mouse
{
    public class DropLocationProbe : MapIdleMode
    {
        public LocationProbe Probe;

        public override void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!(sender is MapView map)) return;
            Probe.Location = map.TransformMouse(e.Location);
            if (map.Probes != null)
                map.Probes.Add(Probe);
            Probe.StopDragging(sender as PickablePanel);
            map.Invalidate();
            ((PickablePanel)sender).ReplaceMouseMode(map.GetIdleMode());
        }

    }
}
