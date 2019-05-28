using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace siteview.dataset
{
    public class SiteDataset : IDisposable
    {
        public string Directory = null;
        public string SiteName = null;

        public float[,] DEM;   // elevation in meters from reference
        string DEM_filename = @"dem.tif";
        public byte[,] Slope;  // slope in deg
        string Slope_filename = @"slope.tif";

        public List<IMapLayer> Layers;

        public SiteDataset() { }
        public SiteDataset(string path)
        {
            Load(path);
        }

        public void Load(string path)
        {
            Directory = path;
            SiteName = Path.GetFileNameWithoutExtension(path);

            // Load Layers
            EmptyLayers();
            var di = new DirectoryInfo(path);
            var png_names = di.GetFiles("*.png").Select(fi => Path.GetFileNameWithoutExtension(fi.Name)).ToList();
            png_names = png_names.Where(n => !n.EndsWith("_legend")).ToList();
            var tiff_names = di.GetFiles("*.tif").Select(fi => Path.GetFileNameWithoutExtension(fi.Name)).ToList();

            png_names = RemovePrefix(SiteName + "_", png_names);
            tiff_names = RemovePrefix(SiteName + "_", tiff_names);

            tiff_names.Remove(DEM_filename);
            tiff_names.Remove(Slope_filename);

            png_names = png_names.OrderBy(name => name).ToList();
            foreach (var pngname in png_names)
            {
                var with_site_name = SiteName + "_" + pngname;
                var pngpath = Path.Combine(Directory, with_site_name + ".png");
                var datapath = tiff_names.Contains(pngname) ? Path.Combine(Directory, with_site_name + ".tif") : null;

                var layer = new AlignedBitmapMapLayer(pngname, pngpath, datapath);
                Layers.Add(layer);
            }

            Layers.Add(new AlignedBitmapMapLayer("Hillshade", Path.Combine(Directory, SiteName + "_hillshade.tif"), null));
        }

        List<string> RemovePrefix(string prefix, List<string> names)
        {
            var result = new List<string>();
            foreach (var name in names)
            {
                if (name.Length <= prefix.Length || !prefix.Equals(name.Substring(0, prefix.Length)))
                {
                    Console.WriteLine($"Site {prefix} contains an invalid filename: {name}");
                    continue;
                }
                result.Add(name.Substring(prefix.Length));
            }
            return result;
        }

        internal void GetLatLon(float y, float x, out double lat, out double lon)
        {
            lat = lon = 0d;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EmptyLayers();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        void EmptyLayers()
        {
            if (Layers == null)
                Layers = new List<IMapLayer>();
            else
            {
                foreach (var layer in Layers)
                    layer.Dispose();
                Layers.Clear();
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SiteDataset() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
