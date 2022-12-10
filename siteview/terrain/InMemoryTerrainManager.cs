
using siteview.math;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace siteview.terrain
{
    public class InMemoryTerrainManager : IDisposable
    {
        //private const string TerrainImageFile = @"C:\UVS\svn\src\TestData\Terrain\ldem_64.img";
        public static string NorthDEM = @"C:\RP\maps\DEM\ldem_80n_20m.horizon.img";

        //public static string SouthDEM = @"C:\RP\maps\DEM\ldem_80s_20m.img";  // Not right yet
        public static string SouthDEM = @"C:\RP\maps\DEM\ldem_80s_20m_fixed.horizon.img";  // Not right yet

        public static string PolarStereographic = @"PROJCS[""Moon_North_Pole_Stereographic"",
    GEOGCS[""Moon 2000"",
        DATUM[""D_Moon_2000"",
            SPHEROID[""Moon_2000_IAU_IAG"",1737400.0,0.0]],
        PRIMEM[""Greenwich"",0],
        UNIT[""Decimal_Degree"",0.0174532925199433]],
    PROJECTION[""Stereographic""],
    PARAMETER[""False_Easting"",0],
    PARAMETER[""False_Northing"",0],
    PARAMETER[""Central_Meridian"",0],
    PARAMETER[""Scale_Factor"",1],
    PARAMETER[""Latitude_Of_Origin"",90],
    UNIT[""Meter"",1]]";

        public static InMemoryTerrainManager Singleton;

        public short[] Data { get; private set; }

        // Conversion to lat/lon
        public const double S0 = 15199.5d;             // PDS SAMPLE_PROJECTION_OFFSET
        public const double L0 = 15199.5d;             // PDS LINE_PROJECTION_OFFSET
        public static double LonFactor = 1d;  // appropriate for south
        public const double Scale = 20d / 1000d;
        public const double LonP = 0d;
        public static double LatP;
        public const double MoonRadius = 1737.4d;
        public const double RadiusInMeters = 1737400.0d;
        public const int Samples = 30400;
        public bool IsNorth = false;

        // Constants

        public virtual InMemoryTerrainManager LoadNorth()
        {
            IsNorth = true;
            LonFactor = -1d;
            LatP = Math.PI / 2;
            Open(NorthDEM);
            Singleton = this;
            return this;
        }

        public virtual InMemoryTerrainManager LoadSouth()
        {
            IsNorth = false;
            LonFactor = 1d;
            LatP = -Math.PI / 2;
            Open(SouthDEM);
            Singleton = this;
            return this;
        }

        protected short _min = short.MaxValue;
        protected short _max = short.MinValue;
        protected bool _minMaxCalculated;

        public unsafe void Open(string lola_dem_path)
        {
            Console.WriteLine($"Loading terrain {lola_dem_path}");
            var fi = new FileInfo(lola_dem_path);
            var byte_count = fi.Length;
            Data = new short[byte_count / 2];
            fixed (short* shortptr = &Data[0])
            {
                var ptr = (byte*)shortptr;
                using (var memory_stream = new UnmanagedMemoryStream(ptr, 0, byte_count, FileAccess.Write))
                using (var file_stream = new FileStream(lola_dem_path, FileMode.Open, FileAccess.Read))
                    file_stream.CopyTo(memory_stream);
            }
        }

        public void Close() => Dispose(true);

        public short Min
        {
            get
            {
                if (_minMaxCalculated) return _min;
                GetMinMax();
                return _min;
            }
        }

        public short Max
        {
            get
            {
                if (_minMaxCalculated) return _max;
                GetMinMax();
                return _max;
            }
        }

        /// <summary>
        /// Return latitude and longitude in radians
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sample"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /*public static void GetLatLon(int line, int sample, out double latitude, out double longitude)
        {
            const double R = MoonRadius;
            var x = (sample - S0 - 1d) * Scale;
            var y = (1d - L0 - line) * Scale;
            var P = Math.Sqrt(x * x + y * y);
            var C = 2 * Math.Atan2(P, 2 * R);
            latitude = Math.Asin(Math.Cos(C) * Math.Sin(LatP) + y * Math.Sin(C) * Math.Cos(LatP) / P);
            longitude = LonP + Math.Atan2(x, y * LonFactor);
        }*/

        /// <summary>
        /// Return latitude and longitude in radians
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sample"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public static void GetLatLon(double line, double sample, out double latitude, out double longitude)
        {
            const double R = MoonRadius;
            var x = (sample - S0) * Scale;
            var y = (L0 - line) * Scale;
            var P = Math.Sqrt(x * x + y * y);
            var C = 2d * Math.Atan2(P, 2 * R);
            latitude = Math.Asin(Math.Cos(C) * Math.Sin(LatP) + (y == 0 ? 0 : y * Math.Sin(C) * Math.Cos(LatP) / P));
            longitude = LonP + Math.Atan2(x, y * LonFactor);
            //Console.WriteLine($"Line={line} Sample={sample} x={x} y={y} lat={latitude} lon={longitude}");
        }

        public static void GetLatLonDegrees(double line, double sample, out double latitude, out double longitude)
        {
            GetLatLon(line, sample, out double lat, out double lon);
            latitude = lat * 180d / Math.PI;
            longitude = lon * 180d / Math.PI;
        }

        public Vector3d GetVector3d(int line, int sample, float height)
        {
            var radius = MoonRadius + height / 1000d;
            GetLatLon(line, sample, out double lat, out double lon);
            var z = radius * Math.Sin(lat);
            var c = radius * Math.Cos(lat);
            var x = c * Math.Cos(lon);  // TODO: Not sure about these
            var y = c * Math.Sin(lon);
            return new Vector3d(x, y, z);
        }

        public static Point GetLineSampleDegrees(double lat_deg, double lon_deg)
        {
            GetLineSampleDegrees(lat_deg, lon_deg, out int line, out int sample);
            return new Point(sample, line);
        }

        public static void GetLineSampleDegrees(double lat_deg, double lon_deg, out int line, out int sample) =>
            GetLineSample(lat_deg * Math.PI / 180d, lon_deg * Math.PI / 180d, out line, out sample);

        // Checked against GetLatLon
        public static void GetLineSample(double lat_rad, double lon_rad, out int line, out int sample)
        {
            double x, y;
            if (InMemoryTerrainManager.Singleton.IsNorth)
            {
                var temp1 = MoonRadius * Math.Tan(Math.PI / 4d - lat_rad / 2d);
                var temp2 = lon_rad - LonP;
                x = 2d * temp1 * Math.Sin(temp2);
                y = -2d * temp1 * Math.Cos(temp2);
            }
            else
            {
                var temp1 = MoonRadius * Math.Tan(Math.PI / 4d + lat_rad / 2d);
                var temp2 = lon_rad - LonP;
                x = 2d * temp1 * Math.Sin(temp2);
                y = 2d * temp1 * Math.Cos(temp2);
            }

            //x = (Sample - S0 - 1) * Scale
            //y = (1 - L0 - Line) * Scale => Line = 1 - L0 - y/Scale
            sample = (int)(x / Scale + S0 + 1d + 0.5d);

            // line = (int)(1d - L0 - y / Scale);   // Should be this per the notes
            line = -(int)(1d - L0 - y / Scale);  //NOTE: I'm not sure why there's a - sign here

            // I don't understand this correction
            const int mid = 30400 / 2;
            line = mid - (line - mid);
        }

        public static void GetLineSampleD(double lat_rad, double lon_rad, out double line, out double sample)
        {
            double x, y;
            if (InMemoryTerrainManager.Singleton.IsNorth)
            {
                var temp1 = MoonRadius * Math.Tan(Math.PI / 4d - lat_rad / 2d);
                var temp2 = lon_rad - LonP;
                x = 2d * temp1 * Math.Sin(temp2);
                y = -2d * temp1 * Math.Cos(temp2);
            }
            else
            {
                var temp1 = MoonRadius * Math.Tan(Math.PI / 4d + lat_rad / 2d);
                var temp2 = lon_rad - LonP;
                x = 2d * temp1 * Math.Sin(temp2);
                y = 2d * temp1 * Math.Cos(temp2);
            }

            //x = (Sample - S0 - 1) * Scale
            //y = (1 - L0 - Line) * Scale => Line = 1 - L0 - y/Scale
            sample = x / Scale + S0;

            // line = (int)(1d - L0 - y / Scale);   // Should be this per the notes
            //line = -(1d - L0 - y / Scale);  //NOTE: I'm not sure why there's a - sign here
            line = -y / Scale + L0;

            // I don't understand this correction
            //const int mid = 30400 / 2;
            //line = mid - (line - mid);
        }

        public float InterpolatedElevation(float line, float sample)
        {
            var x1 = (int)sample;
            var y1 = (int)line;
            int x2 = x1 + 1;
            int y2 = y1 + 1;
            float q11 = LineSampleToTerrainOffset(y1, x1);
            float q12 = LineSampleToTerrainOffset(y2, x1);
            float q21 = LineSampleToTerrainOffset(y1, x2);
            float q22 = LineSampleToTerrainOffset(y2, x2);
            // From https://en.wikipedia.org/wiki/Bilinear_interpolation, note denominator is 1 in this case, so is omitted
            float r = (q11 * (x2 - sample) * (y2 - line) + q21 * (sample - x1) * (y2 - line) + q12 * (x2 - sample) * (line - y1) + q22 * (sample - x1) * (line - y1));
            //Console.WriteLine(@"  x={0:G7} y={1:G7} x1={2} y1={3} x2={4} y2={5} q11={6:G7} q12={7:G7} q21={8:G7} q22={9:G7}", x, y, x1, y1, x2, y2, q11, q12, q21, q22);
            return r;
        }

        public short[] GenerateHeightField(int line, int sample, int height, int width)
        {
            var r = new short[height * width];
            for (var h = 0; h < height; h++)
            {
                var row_offset = h * width;
                for (var w = 0; w < width; w++)
                    r[row_offset + w] = LineSampleToRawTerrainOffset(line + h, sample + w);
            }
            return r;
        }

        //public short[] GenerateHeightField(patches.TerrainPatch p) => GenerateHeightField(p.Line, p.Sample, p.Height, p.Width);

        public Vector3d GetPointInME(int line, int sample)
        {
            var relz = LineSampleToTerrainOffset(line, sample);
            var radius = MoonRadius + relz / 1000d;
            GetLatLon(line, sample, out double lat, out double lon);
            var z = radius * Math.Sin(lat);
            var c = radius * Math.Cos(lat);
            var x = c * Math.Cos(lon);  // TODO: Not sure about these
            var y = c * Math.Sin(lon);
            return new Vector3d(x, y, z);
        }

        public Vector3d GetPointInME(float line, float sample)
        {
            var relz = InterpolatedElevation(line, sample);
            var radius = MoonRadius + relz / 1000d;
            GetLatLon(line, sample, out double lat, out double lon);
            var z = radius * Math.Sin(lat);
            var c = radius * Math.Cos(lat);
            var x = c * Math.Cos(lon);  // TODO: Not sure about these
            var y = c * Math.Sin(lon);
            return new Vector3d(x, y, z);
        }

        public float LineSampleToTerrainOffset(int line, int sample)
        {
            const int height = 30400;
            const int width = 30400;
            const int stride = width;

            System.Diagnostics.Debug.Assert(line >= 0 && line < height && sample >= 0 && sample < width);
            //if (!(line >= 0 && line < height && sample >= 0 && sample < width))
            //    Console.WriteLine("here");

            int index = line * stride + sample;
            short h = Data[index];
            return 0.5f * h;
        }

        public short LineSampleToRawTerrainOffset(int line, int sample)
        {
            const int height = 30400;
            const int width = 30400;
            const int stride = width;

            System.Diagnostics.Debug.Assert(line >= 0 && line < height && sample >= 0 && sample < width);

            int index = line * stride + sample;
            return Data[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short Fetch(int line, int sample) => Data[line * 30400 + sample];

        public void GetMinMax()
        {
            var min = short.MaxValue;
            var max = short.MinValue;
            var length = Data.Length;
            for (var i = 0; i < length; i++)
            {
                var v = Data[i];
                if (v < min) min = v;
                if (v > max) max = v;
            }
            _max = max;
            _min = min;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                Data = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~InMemoryTerrainManager() {
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

        /*
        #region South Accessors (untested)

        public void GetSouthIndices(double lat, double lon, out int i, out int j)
        {
            var r = 2d * MoonRadius * Math.Tan((90d + lat) * Math.PI / 360d);
            var x = r * Math.Sin(lon * Math.PI / 180d);
            var y = r * Math.Cos(lon * Math.PI / 180d);
            i = (int)(x / Scale + Samples / 2d + 0.5d);
            j = (int)(y / Scale + Samples / 2d + 0.5d);
        }

        public void GetSouthLatLon(int i, int j, out double lat, out double lon)
        {
            var x = (i - Samples / 2 - 0.5d) * Scale;
            var y = (j - Samples / 2 - 0.5d) * Scale;
            var r = Math.Sqrt(x * x + y * y);
            lon = Math.Atan2(x, y) * 180 / Math.PI;
            lat = -90d + 180d / Math.PI * 2 * Math.Atan(0.5d * r / MoonRadius);
        }

        public void GetSouthPosition(int line, int sample, out double px, out double py, out double pz)
        {
            var terrainHeight = LineSampleToTerrainOffset(line,sample) / 1000d;  // terrain height in km
            double lat, lon;
            GetSouthLatLon(line, sample, out lat, out lon);
            var latInRadians = lat * Math.PI / 180d;
            var z = MoonRadius * Math.Sin(latInRadians);
            var r1 = MoonRadius * Math.Cos(latInRadians);
            var lonInRadians = lon * Math.PI / 180d;
            var x = r1 * Math.Cos(lonInRadians);
            var y = r1 * Math.Sin(lonInRadians);

            var len = Math.Sqrt(x * x + y * y + z * z);

            px = -(x + terrainHeight * (x / len));  // This - sign is odd.  I don't understand why it's necessary.
            py = y + terrainHeight * (y / len);
            pz = z + terrainHeight * (z / len);
        }

        public void GetSouthPosition(double lat, double lon, double alt, out double px, out double py, out double pz)
        {
            lon = 180d - lon;
            int i, j;
            GetSouthIndices(lat, lon, out i, out j);
            var terrainHeight = this[i, j] / 1000d;  // terrain height in km
            var latInRadians = lat * Math.PI / 180d;
            var z = MoonRadius * Math.Sin(latInRadians);
            var r1 = MoonRadius * Math.Cos(latInRadians);
            var lonInRadians = lon * Math.PI / 180d;
            var x = r1 * Math.Cos(lonInRadians);
            var y = r1 * Math.Sin(lonInRadians);

            var len = Math.Sqrt(x * x + y * y + z * z);

            terrainHeight += alt / 1000d;

            px = -(x + terrainHeight * (x / len));  // This - sign is odd.  I don't understand why it's necessary.
            py = y + terrainHeight * (y / len);
            pz = z + terrainHeight * (z / len);
        }

        /// <summary>
        /// Returns the position in Moon ME frame.  Units are km from the moon's center.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="px">Position in ME frame in km</param>
        /// <param name="py">Position in ME frame in km</param>
        /// <param name="pz">Position in ME frame in km</param>
        public void GetSouthPositionME(int i, int j, out double px, out double py, out double pz)
        {
            var x = (i - Samples / 2 - 0.5d) * Scale;
            var y = (j - Samples / 2 - 0.5d) * Scale;
            var r1 = Math.Sqrt(x * x + y * y);
            var z = -Math.Sqrt(MoonRadius * MoonRadius - r1 * r1);

            var temp = x;  // wrong
            x = -y;
            y = temp;

            var vx = x / MoonRadius;  // - signs here are wrong
            var vy = y / MoonRadius;  // swapping x and y is wrong
            var vz = z / MoonRadius;

            var terrainHeight = this[i, j] / 1000d;  // terrain height in km

            px = x + vx * terrainHeight;
            py = y + vy * terrainHeight;
            pz = z + vz * terrainHeight;
        }

        /// <summary>
        /// Return position above surface
        /// </summary>
        /// <param name="lat">Latitude in deg</param>
        /// <param name="lon">Longitude in deg</param>
        /// <param name="alt">Altitude in km above terrain</param>
        /// <param name="px">X position in Moon ME in km</param>
        /// <param name="py">Y position in Moon ME in km</param>
        /// <param name="pz">Z position in Moon ME in km</param>
        public void GetSouthPositionME(double lat, double lon, double alt, out double px, out double py, out double pz)
        {
            int i, j;
            GetSouthIndices(lat, lon, out i, out j);
            var x = (i - Samples / 2 - 0.5d) * Scale;
            var y = (j - Samples / 2 - 0.5d) * Scale;
            var r1 = Math.Sqrt(x * x + y * y);
            var z = -Math.Sqrt(MoonRadius * MoonRadius - r1 * r1);

            var temp = x;  // wrong
            x = y;
            y = temp;

            var vx = y / MoonRadius;  // - signs here are wrong
            var vy = x / MoonRadius;  // swapping x and y is wrong
            var vz = z / MoonRadius;

            var terrainHeight = this[i, j] / 1000d;  // terrain height in km
            terrainHeight += alt;
            px = x + vx * terrainHeight;
            py = y + vy * terrainHeight;
            pz = z + vz * terrainHeight;
        }

        #endregion South Accessors

        #region North Accessors

        public void GetNorthIndices(double lat, double lon, out int i, out int j)
        {
            var r = 2d * RadiusInMeters * Math.Tan((90d - lat) * Math.PI / 360d);
            var x = r * Math.Sin(lon * Math.PI / 180d);
            var y = r * Math.Cos(lon * Math.PI / 180d);
            i = (int)(x / ScaleInMeters + Samples / 2d + 0.5d);
            j = (int)(y / ScaleInMeters + Samples / 2d + 0.5d);
        }

        public void GetNorthLatLon(int i, int j, out double lat, out double lon)
        {
            var x = (i - Samples / 2 - 0.5d) * ScaleInMeters;
            var y = (j - Samples / 2 - 0.5d) * ScaleInMeters;
            var r = Math.Sqrt(x * x + y * y);
            lon = Math.Atan2(x, y) * 180 / Math.PI;
            lat = 90d - 180d / Math.PI * 2 * Math.Atan(0.5d * r / RadiusInMeters);
        }

        public void GetNorthPosition(int i, int j, out double px, out double py, out double pz)
        {
            var terrainHeight = this[i, j] / 1000d;  // terrain height in km
            double lat, lon;
            GetNorthLatLon(i, j, out lat, out lon);
            var latInRadians = lat * Math.PI / 180d;
            var z = MoonRadius * Math.Sin(latInRadians);
            var r1 = MoonRadius * Math.Cos(latInRadians);
            var lonInRadians = lon * Math.PI / 180d;
            var x = r1 * Math.Cos(lonInRadians);
            var y = r1 * Math.Sin(lonInRadians);

            var len = Math.Sqrt(x * x + y * y + z * z);

            px = x + terrainHeight * (x / len);
            py = y + terrainHeight * (y / len);
            pz = z + terrainHeight * (z / len);
        }

        public void GetNorthPositionME(int i, int j, out double px, out double py, out double pz)
        {
            var x = (i - Samples / 2 - 0.5d) * Scale;
            var y = (j - Samples / 2 - 0.5d) * Scale;
            var r1 = Math.Sqrt(x * x + y * y);
            var z = Math.Sqrt(MoonRadius * MoonRadius - r1 * r1);

            var vx = x / MoonRadius;
            var vy = y / MoonRadius;
            var vz = z / MoonRadius;

            var terrainHeight = this[i, j] / 1000d;  // terrain height in km

            px = x + vx * terrainHeight;
            py = y + vy * terrainHeight;
            pz = z + vz * terrainHeight;
        }

        public void GetNorthPositionME(double lat, double lon, double alt, out double px, out double py, out double pz)
        {
            int i, j;
            GetNorthIndices(lat, lon, out i, out j);
            var x = (i - Samples / 2 - 0.5d) * Scale;
            var y = (j - Samples / 2 - 0.5d) * Scale;
            var r1 = Math.Sqrt(x * x + y * y);
            var z = Math.Sqrt(MoonRadius * MoonRadius - r1 * r1);

            var vx = y / MoonRadius;  // - signs here are wrong
            var vy = x / MoonRadius;  // swapping x and y is wrong
            var vz = z / MoonRadius;

            var terrainHeight = this[i, j] / 1000d;  // terrain height in km
            terrainHeight += alt;
            px = x + vx * terrainHeight;
            py = y + vy * terrainHeight;
            pz = z + vz * terrainHeight;
        }

        #endregion
        */

    }
}
