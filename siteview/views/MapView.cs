using siteview.dataset;
using siteview.math;
using siteview.mouse;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TraversePlanner.views;

namespace siteview.views
{
    public class MapView : PickablePanel
    {
        const float MetersPerPixel = 20f;

        public MapView()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            Transform = new DisplayTransform { OffsetX = 0, OffsetY = 0, Scale = 1f };

            MouseModeStack = new Stack<MouseMode>(new[] { GetIdleMode() });

            InitializeContextMenu();
        }

        public SiteView MainWindow;

        public static Font TimestampFont = new Font("Arial", 32, FontStyle.Regular);
        public static Font RulerMeasureFont = new Font("Arial", 16, FontStyle.Regular);
        protected readonly Pen _selectionPen = new Pen(Color.LightCyan, 2);
        protected readonly Brush _selectionBrush = new SolidBrush(Color.LightCyan);

        public override IEnumerable<Pickable> Pickables() => Enumerable.Empty<Pickable>();

        protected ContextMenu _contextMenu;

        protected bool _RulerMeasureIsVisible = false;
        public bool RulerMeasureIsVisible
        {
            get { return _RulerMeasureIsVisible; }
            set { _RulerMeasureIsVisible = value; Invalidate(); }
        }

        protected RectangleF _RulerMeasure = new RectangleF(0, 0, 0, 0);
        public RectangleF RulerMeasure
        {
            get { return _RulerMeasure; }
            set { _RulerMeasure = value; if (_RulerMeasureIsVisible) Invalidate(); }
        }

        protected List<IMapLayer> _Layers;
        public List<IMapLayer> Layers
        {
            get { return _Layers; }
            set
            {
                _Layers = value;
                Invalidate();
            }
        }

        private DisplayTransform _Transform;
        public DisplayTransform Transform
        {
            get { return _Transform; }
            set
            {
                _Transform = value;
                ClampTransform();
                Invalidate();
            }
        }

        private void ClampTransform()
        {
            _Transform.OffsetX = Math.Min(0, _Transform.OffsetX);
            _Transform.OffsetY = Math.Min(0, _Transform.OffsetY);
        }

        protected bool _SelectionRectangleIsVisible = false;
        public bool SelectionRectangleIsVisible
        {
            get { return _SelectionRectangleIsVisible; }
            set { _SelectionRectangleIsVisible = value; Invalidate(); }
        }

        protected RectangleF _SelectionRectangle = new RectangleF(0, 0, 0, 0);
        public RectangleF SelectionRectangle
        {
            get { return _SelectionRectangle; }
            set { _SelectionRectangle = value; if (SelectionRectangleIsVisible) Invalidate(); }
        }

        public Vector3d SunVector;
        public Vector3d EarthVector;
        public MouseMessage MouseMessage;
        public float SunAzimuth;  // Radians
        public float SunElevation;
        public float EarthAzimuth;
        public float EarthElevation;

        public PointF TransformMouse(Point l) => Transform.MouseToMap(l);
        public float TransformMouseX(int x) => Transform.MouseToMapX(x);
        public float TransformMouseY(int y) => Transform.MouseToMapY(y);

        public struct DisplayTransform
        {
            public int OffsetX;
            public int OffsetY;
            public float Scale;

            public override string ToString() => $"<DisplayTransform Scale={Scale} OffsetX={OffsetX} OffsetY={OffsetY}";

            public PointF MouseToMap(Point m) => new PointF((m.X - OffsetX) * Scale, (m.Y - OffsetY) * Scale);
            public float MouseToMapX(int mx) => (mx - OffsetX) * Scale;
            public float MouseToMapY(int my) => (my - OffsetY) * Scale;

            /*
            public PointF MapToMouse(Point p)
            {
                float cellSize;
                int level, stride;
                TileTree.DrawCellSize(this, out cellSize, out stride, out level);
                var 
            }*/

            public float X(float mx) => mx / Scale + OffsetX;
            public float Y(float my) => my / Scale + OffsetY;
            public PointF MapToMouse(PointF p) => new PointF(X(p.X), Y(p.Y));

            public PointF this[PointF p] => new PointF(p.X / Scale + OffsetX, p.Y / Scale + OffsetY);
            public SizeF this[SizeF s] => new SizeF(s.Width / Scale, s.Height / Scale);

            public RectangleF this[Rectangle r] => new RectangleF(this[r.Location], this[r.Size]);
            public RectangleF this[RectangleF r] => new RectangleF(this[r.Location], this[r.Size]);

            //TODO: Change these?
            public RectangleF Transform(RectangleF r) => new RectangleF(r.Left * Scale + OffsetX, r.Top * Scale + OffsetY, r.Width * Scale, r.Height * Scale);
            public Rectangle Transform(Rectangle r) => new Rectangle((int)(r.Left * Scale + OffsetX), (int)(r.Top * Scale + OffsetY), (int)(r.Width * Scale), (int)(r.Height * Scale));
            public Rectangle InvTransform(int Stride, Rectangle r) => new Rectangle(Stride * (int)((r.Left - OffsetX) / Scale), Stride * (int)((r.Y - OffsetY) / Scale), Stride * (int)(r.Width / Scale), Stride * (int)(r.Height / Scale));
            public PointF this[System.Windows.Vector p] => new PointF((float)(p.X * Scale + OffsetX), (float)(p.Y * Scale + OffsetY));
        }

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            if (DesignMode) return;
            PaintMap(e, e.Graphics);
        }

        public void PaintMap(PaintEventArgs e, Graphics g)
        {
            try
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                foreach (var layer in Layers)
                    layer.Draw(g, Transform);

                // Draw other pickables

                foreach (Pickable p in Pickables())
                    p.Paint(e, this);

                if (MouseMessage != null && MouseMessage.Message != null)
                    g.DrawString(MouseMessage.Message, MouseMessage.Font, MouseMessage.Brush, MouseMessage.X, MouseMessage.Y);

                if (SelectionRectangleIsVisible)
                    DrawSelectionRectangle(g, Transform, SelectionRectangle);

                if (RulerMeasureIsVisible)
                    DrawRulerMeasure(g, Transform, RulerMeasure);

                // Changes clipping region, so do this last, for now
                // DrawLegend(g);
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }
        }

        private void DrawSelectionRectangle(Graphics g, DisplayTransform t, RectangleF r)
        {
            var loc = t[r.Location];
            var siz = t[r.Size];
            g.DrawRectangle(_selectionPen, loc.X, loc.Y, siz.Width, siz.Height);
        }

        private void DrawRectangle(Graphics g, DisplayTransform t, RectangleF r, Pen pen)
        {
            var loc = t[r.Location];
            var siz = t[r.Size];
            g.DrawRectangle(pen, loc.X, loc.Y, siz.Width, siz.Height);
        }

        private void DrawRulerMeasure(Graphics g, DisplayTransform t, RectangleF r)
        {
            var m1 = r.Location;
            var m2 = new PointF(m1.X + r.Width, m1.Y + r.Height);
            var p1 = t[m1];
            var p2 = t[m2];
            g.DrawLine(_selectionPen, p1.X, p1.Y, p2.X, p2.Y);
            var d = MetersPerPixel * m1.Distance(m2);
            string str = d > 1000f ? $"{(d / 1000f).ToString("F2")} km" : $"{d.ToString("F0")} m";
            g.DrawString(str, RulerMeasureFont, _selectionBrush, p2.X + 4f, p2.Y + 4f, StringFormat.GenericDefault);
        }

        private readonly Brush _LegendBrush = new SolidBrush(Color.WhiteSmoke);
        private readonly Point[] _transformedPoints = new Point[1];
        private void DrawLegend(Graphics g)
        {
            const float LegendSize = 5000f;   // 5 km
            const float LegendY = 10;
            const float LegendX = 5;
            const float LegendTickHeight = 5f;

            const int rTop = 2;
            const int rLeft = 2;
            const int rWidth = 600;
            const int rHeight = 30;

            float _mapScale = Transform.Scale;

            const float targetLength = LegendSize / MetersPerPixel;
            float targetMeters = targetLength * MetersPerPixel / _mapScale;
            double legendMeters = Math.Pow(10d, Math.Ceiling(Math.Log10(targetMeters)));
            double legendStep = legendMeters / 10d;

            float factor = _mapScale / MetersPerPixel;

            g.Clip = new Region(new Rectangle(rTop, rLeft, rWidth, rHeight));
            g.FillRegion(Brushes.DarkGray, g.Clip);

            g.DrawLine(Pens.WhiteSmoke, LegendX, LegendY, LegendX + (float)(legendMeters * factor), LegendY);
            for (int i = 0; i < 11; i++)
            {
                float x = LegendX + (float)(legendStep * i * factor);
                g.DrawLine(Pens.WhiteSmoke, x, LegendY, x, LegendY - LegendTickHeight);
                //g.DrawString((i * legendStep).ToString() + " m", DrawingHelper.DrawFont, _LegendBrush, x, LegendY);
            }
        }

        /// <summary>
        /// Draw a compass face with vectors to the sun and earth
        /// Note that azimuth has 0 due north and increases clockwise
        /// </summary>
        /// <param name="g"></param>
        private void DrawSunEarthAngles(Graphics g)
        {
            const int size = 100;
            const int size2 = size / 2;
            const int border = 5;
            const int tick = 5;

            Rectangle c = ClientRectangle;

            float cx = c.Width - border - size2;
            const float cy = border + size2;
            g.FillEllipse(Brushes.LightSlateGray, cx - size2, cy - size2, size, size);
            const float vector_length = .8f * size2;

            g.DrawLine(Pens.Black, cx + size2 - tick, cy, cx + size2, cy);
            g.DrawLine(Pens.Black, cx - size2, cy, cx - size2 + tick, cy);
            g.DrawLine(Pens.Black, cx, cy - size2, cx, cy - size2 + tick);
            g.DrawLine(Pens.Black, cx, cy + size2 - tick, cx, cy + size2);

            g.DrawLine(Pens.Blue, cx, cy, cx + vector_length * (float)Math.Sin(EarthAzimuth), cy - vector_length * (float)Math.Cos(EarthAzimuth));
            g.DrawLine(Pens.Orange, cx, cy, cx + vector_length * (float)Math.Sin(SunAzimuth), cy - vector_length * (float)Math.Cos(SunAzimuth));

            /*
            if (false)
            {
                cy += size + border;
                g.FillEllipse(Brushes.LightSlateGray, cx - size2, cy - size2, size, size);
                g.DrawLine(Pens.Black, cx + size2 - tick, cy, cx + size2, cy);
                g.DrawLine(Pens.Black, cx - size2, cy, cx - size2 + tick, cy);
                g.DrawLine(Pens.Black, cx, cy - size2, cx, cy - size2 + tick);
                g.DrawLine(Pens.Black, cx, cy + size2 - tick, cx, cy + size2);
                g.DrawLine(Pens.Blue, cx, cy, cx + vector_length * (float)Math.Cos(EarthElevation), cy - vector_length * (float)Math.Sin(EarthElevation));
                g.DrawLine(Pens.Orange, cx, cy, cx + vector_length * (float)Math.Cos(SunElevation), cy - vector_length * (float)Math.Sin(SunElevation));
            }*/
        }

        public void DrawLine(Graphics g, Pen pen, System.Windows.Vector p1, System.Windows.Vector p2) => g.DrawLine(pen, _Transform[p1], _Transform[p2]);
        public void DrawLineToward(Graphics g, Pen pen, System.Windows.Vector p1, System.Windows.Vector p2, double d) => g.DrawLine(pen, _Transform[p1], _Transform[p1 + d * p2]);
        public void DrawPoint(Graphics g, Brush b, System.Windows.Vector v)
        {
            var m = _Transform[v];
            g.FillEllipse(b, m.X - 1, m.Y - 1, 3, 3);
        }

        public void WriteMapImage(string filename)
        {
            var bmp = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                var e = new PaintEventArgs(g, Bounds);
                PaintMap(e, g);
            }
            bmp.Save(filename);
        }

        public void WriteMapImageToBitmap(Bitmap bmp)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                var e = new PaintEventArgs(g, Bounds);
                PaintMap(e, g);
            }
        }

        #endregion Paint

        #region Context Menu

        private void InitializeContextMenu()
        {
            var cm = _contextMenu = new ContextMenu();

            var item1 = new MenuItem { Text = "Fill in later" };
            item1.Click += (sender, e) => FillInLater();
            cm.MenuItems.Add(item1);


        }

        public void ShowContextMenu(Point p)
        {
            _contextMenu.Show(this, p);
        }

        private void FillInLater()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        public event EventHandler PanZoomEnded;
        internal void EndPanZoom()
        {
            PanZoomEnded?.Invoke(this, new EventArgs());  // This isn't right yet, but it's sufficient for my needs
        }

        #endregion
    }

    public class MouseMessage
    {
        public int X;
        public int Y;
        public Brush Brush = DefaultBrush;
        public Font Font = DefaultFont;
        public string Message;

        public static Brush DefaultBrush = Brushes.Red;
        public static Font DefaultFont = new Font("Arial", 18, FontStyle.Regular);
    }
}
