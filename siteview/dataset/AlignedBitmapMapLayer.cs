using System;
using System.Drawing;
using System.Drawing.Imaging;
using siteview.utilities;
using siteview.views;

namespace siteview.dataset
{
    public class AlignedBitmapMapLayer : MapLayer
    {
        public string BitmapPath { get; set; }
        public string DataPath { get; set; }

        bool IsLoaded = false;

        public Bitmap Bitmap { get; set; }
        float[,] FloatData;
        //byte[,] ByteData;

        public AlignedBitmapMapLayer() { }
        public AlignedBitmapMapLayer(string name, string bitmap_path, string data_path)
        {
            Name = name;
            BitmapPath = bitmap_path;
            DataPath = data_path;
            if (BitmapPath == null)
                throw new Exception("AlignedBitmapMapLayers must be initialized with image paths");
        }

        void Load()
        {
            Bitmap = Image.FromFile(BitmapPath) as Bitmap;
            if (DataPath != null)
                FloatData = GeotiffHelper.ReadGeotiffAsFloatArray(DataPath, float.MinValue);
            IsLoaded = true;
        }

        public override void Draw(Graphics g, MapView.DisplayTransform t)
        {
            if (!IsLoaded)
                Load();
            var r = t.Transform(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height));

            Console.WriteLine($"destrect=[{r.X},{r.Y},{r.Width},{r.Height}]");

            var save = g.CompositingMode;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.DrawImage(Bitmap, r, 0, 0, Bitmap.Width, Bitmap.Height, GraphicsUnit.Pixel, _imageAttributes);
            g.CompositingMode = save;
        }


    }
}
