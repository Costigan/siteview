using System;
using System.Drawing;
using System.Windows.Forms;
using TraversePlanner.views;

namespace siteview.mouse
{
    public class Pickable
    {
        protected PointF _location;
        protected float _spread = 3;
        public RectangleF Bounds = new RectangleF();

        public virtual PointF Location
        {
            get { return _location; }
            set
            {
                _location = value;
                Bounds = new RectangleF(_location.X - _spread, _location.Y - _spread, _spread + _spread + 1, +_spread + _spread + 1);
            }
        }

        //[JsonIgnore]
        public float X { get { return _location.X; } set { Location = new PointF(value, _location.Y); } }

        //[JsonIgnore]
        public float Y { get { return _location.Y; } set { Location = new PointF(_location.X, value); } }

        public virtual double PixelDistance(Pickable other)
        {
            float dx = Location.X - other.Location.X;
            float dy = Location.Y - other.Location.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        //public bool Pick(Point p) => Bounds.Contains(p);
        private const float PickDistance = 3f;
        public bool Pick(PointF p) => Math.Abs(p.X - Location.X) < PickDistance && Math.Abs(p.Y - Location.Y) < PickDistance;

        public float Spread
        {
            get { return Bounds.Height / 2; }
            set
            {
                _spread = value;
                Bounds.Location = new PointF(_location.X - _spread, _location.Y - _spread);
                Bounds.Width = Bounds.Height = _spread + _spread + 1;
            }
        }

        public virtual void Paint(PaintEventArgs e, PickablePanel p)
        {
            e.Graphics.DrawRectangle(Pens.Red, Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height);
        }

        public Pickable Copy() => new Pickable { Bounds = Bounds, Location = Location, Spread = Spread };

        public virtual void Select(PickablePanel panel) { }
        public virtual void Deselect(PickablePanel panel) { }
        public virtual void StartDragging(PickablePanel panel) { }  // not called yet.  Needs work.
        public virtual void Dragging(PickablePanel panel) { }
        public virtual void StopDragging(PickablePanel panel) { }
    }
}
