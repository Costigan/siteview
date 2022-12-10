using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ILGPU;
using ILGPU.Runtime;
using System.Drawing;
using siteview.terrain;
using siteview.math;

/*
namespace siteview.gpu
{
    public class GenerateSiteHorizons
    {
        public const float MoonRadius = 1737.4f;
        //public const float MetersToMoonRadiusUnits = 1f / MoonRadiusUnitsToMeters;
        //public const float MoonRadiusUnitsToMeters = 1737400f;
        //public const float PixelsToMoonRadiusUnits = 20f / MoonRadiusUnitsToMeters;
        public const float PixelsToKM = 0.020f;

        public const float S0 = 15199.5f;             // PDS SAMPLE_PROJECTION_OFFSET
        public const float L0 = 15199.5f;

        public const float Scale = 20f / 1000f;
        public const float LonP = 0f;

        //public static float LonFactor = 1f;  
        //public static float LatP;
        public const float LonFactor = 1f;           // appropriate for south
        public const float LatP = -GPUMath.PI / 2f;

        public const int TerrainSize = InMemoryTerrainManager.Samples;
        public const int ChunkSize = 262144;  // 512 pixels x 512 pixels maximum

        public AcceleratorType DefaultAcceleratorType = AcceleratorType.Cuda;
        public Rectangle? BoundingBox = null;

        List<Vector3d> _sunvecs = null;

        const int small_range_step_count = 1024;

        InMemoryTerrainManager Terrain => SiteView.Singleton.Terrain;

        public const int HorizonSamples = 360 * 4;
        public const int PixelsPerKernel = 32;
        public const int RayCount = 360 * 8;
        static readonly Index2 Dimension = new Index2(PixelsPerKernel, RayCount);

        public void Generate(Rectangle rect, string horizons_path)
        {
            Debug.Assert(LonFactor == (float)InMemoryTerrainManager.LonFactor);
            Debug.Assert(LatP == (float)InMemoryTerrainManager.LatP);
            var terrain = Terrain;
            var cpu_chunk_array = new short[262144 * HorizonSamples];

            BoundingBox = null;

            var cpu_range = FillRangeArray();
            var cpu_range_limit = new int[cpu_range.Length];

            using (var context = new Context())
            {
                if (!GetAccelerator(AcceleratorType.Cuda, out AcceleratorId aid))  // DefaultAcceleratorType
                    return;
                using (var accelerator = Accelerator.Create(context, aid))
                {
                    using (var gpu_terrain = accelerator.Allocate<short>(terrain.Data.Length))
                    using (var gpu_range = accelerator.Allocate<float>(cpu_range.Length))
                    using (var gpu_range_limit = accelerator.Allocate<int>(cpu_range_limit.Length))
                    using (var gpu_chunk_array = accelerator.Allocate<short>(cpu_chunk_array.Length))
                    {
                        gpu_terrain.CopyFrom(terrain.Data, 0, 0, terrain.Data.Length);
                        gpu_range.CopyFrom(cpu_range, 0, 0, cpu_range.Length);

                        var index = new GroupedIndex()

                        var kernel1 = accelerator.LoadAutoGroupedStreamKernel<Index2, int, int, int, int, ArrayView<short>, ArrayView<float>, ArrayView<float>, ArrayView<float>, ArrayView<int>>(NearFieldKernel1);

                        while (queue.Count > 0)
                        {
                            var center = queue[0];
                            Console.WriteLine($"Starting patch id=[{center.Id.X},{center.Id.Y}] ...");
                            try
                            {
                                var stopwatch = Stopwatch.StartNew();

                                for (var i = 0; i < cpu_rise.Length; i++)
                                    cpu_rise[i] = float.MinValue;
                                FillRangeLimit(center, cpu_range_limit);
                                gpu_range_limit.CopyFrom(cpu_range_limit, 0, 0, cpu_range_limit.Length);
                                gpu_rise.CopyFrom(cpu_rise, 0, 0, cpu_rise.Length);


                                var batch = 32;
                                for (var slope_idx_start = 0; slope_idx_start < cpu_slope.Length; slope_idx_start += batch)
                                {
                                    var slope_idx_stop = Math.Min(slope_idx_start + batch, cpu_slope.Length);
                                    kernel1(Dimension, center.Line, center.Sample, slope_idx_start, slope_idx_stop, gpu_terrain, gpu_range, gpu_slope, gpu_rise, gpu_range_limit);
                                    accelerator.Synchronize();  // Needed?
                                }

                                gpu_rise.CopyTo(cpu_rise, 0, 0, cpu_rise.Length);

                                stopwatch.Stop();

                                DumpRise(center, cpu_rise);
                                WriteSlopeArray();

                                // Update queue on disk
                                queue.RemoveAt(0);
                                queue_saver?.Invoke(queue);
                                Console.WriteLine($"  {queue.Count} patches remaining.  Est. completion at {DateTime.Now.AddMinutes(queue.Count * stopwatch.Elapsed.TotalMinutes)}");
                            }
                            catch (Exception e1)
                            {
                                Console.WriteLine(e1);
                                Console.WriteLine(e1.StackTrace);
                            }
                            LunarHorizon.Singleton?.SetStatus(false);
                        }
                    }
                    Console.WriteLine($"Finished queue.  time={stopwatchOuter.Elapsed}.");
                }
            }
        }

        bool GetAccelerator(AcceleratorType t, out AcceleratorId aid)
        {
            aid = Accelerator.Accelerators.Where(id => id.AcceleratorType == t).FirstOrDefault();
            if (aid.AcceleratorType != t)
                Console.WriteLine(@"There is accelerator present of the desired type.  Doing nothing.");
            return aid.AcceleratorType == t;
        }

        float[] FillRangeArray()
        {
            var step = 0.70710710678f;
            var lst = Enumerable.Range(0, small_range_step_count).Select(i => i * step).ToList();

            // Fill this list, then copy to the second range array

            // the largest range in _range1 is d.  Go from there to 2*d, taking a larger step.  Then bump the range and the step again.
            var bump_step = 4f;
            var bump_fence = 2f;
            var max = (float)InMemoryTerrainManager.Samples;

            step *= bump_step;
            var d = lst[lst.Count - 1] + step;
            var fence = d * bump_fence;
            while (d < max)
            {
                lst.Add(d);
                d += step;
                if (d > fence)
                {
                    step *= bump_step;
                    fence *= bump_fence;
                }
            }
            return lst.ToArray();
        }

        void FillRangeLimit(Point pt, int[] cpu_range_limit)
        {
            var center_line = pt.Y;
            var center_sample = pt.X;
            Debug.Assert(cpu_range_limit.Length == cpu_slope_size);
            var min = (double)(2 * TerrainPatch.DefaultSize);
            var max = (double)(TerrainPatch.DEM_size - 2 * TerrainPatch.DefaultSize);
            for (var i = 0; i < cpu_range_limit.Length; i++)
            {
                var ray_rad = Math.PI * 2f * i / cpu_slope_size;  // 0 deg in ME frame points toward the earth
                var ray_cos = Math.Cos(ray_rad);  // varies the line
                var ray_sin = Math.Sin(ray_rad);  // varies the sample
                var farthest = 0;
                for (var j = 0; j < cpu_range.Length; j++)
                {
                    var range = cpu_range[j];
                    var caster_line = center_line + ray_sin * range;
                    if (caster_line < min || caster_line > max)
                        break;
                    var caster_sample = center_sample + ray_cos * range;
                    if (caster_sample < min || caster_sample > max)
                        break;
                    farthest = j;
                }
                cpu_range_limit[i] = farthest;
            }
        }

        static void NearFieldKernel1(
            Index2 index,      // Contains (sample, line)  First dimension changes fastest
            int target_line,   // row within the 128 x 128 target patch
            int target_sample, // column
            int slope_idx_start,
            int slope_idx_stop,

            ArrayView<short> gpu_terrain,
            ArrayView<float> gpu_range,
            ArrayView<float> gpu_slope,
            ArrayView<float> gpu_rise,
            ArrayView<int> gpu_range_limit)
        {
            var flip = -LonFactor;

            // From ILGPU source code: public int ComputeLinearIndex(Index2 dimension) => Y * dimension.X + X;
            var rise_idx = index.ComputeLinearIndex(Dimension);
            var rise = gpu_rise[rise_idx];

            var center_line = target_line + index.Y;
            var center_sample = target_sample + index.X;
            var center_idx = center_line * TerrainSize + center_sample;
            var center_height = 0.5f * gpu_terrain[center_idx];
            GetVector3d(center_line, center_sample, center_height, out float center_x, out float center_y, out float center_z);
            center_z *= flip;

            for (var slope_idx = slope_idx_start; slope_idx < slope_idx_stop; slope_idx++)
            {
                var slope = gpu_slope[slope_idx];

                // Work out the direction vector
                var ray_rad = GPUMath.PI * 2f * slope_idx / cpu_slope_size;  // 0 deg in ME frame points toward the earth
                var ray_cos = GPUMath.Cos(ray_rad);  // varies the line
                var ray_sin = GPUMath.Sin(ray_rad);  // varies the sample

                // iterate over the ranges
                var range_limit = gpu_range_limit[slope_idx];
                for (var range_idx = 0; range_idx < range_limit; range_idx++)
                {
                    var range = gpu_range[range_idx];
                    var caster_line = center_line + ray_sin * range;
                    var caster_sample = center_sample + ray_cos * range;

                    var x1 = (int)caster_sample;  // Calculate the caster point by interpolating between four points from the points array
                    var y1 = (int)caster_line;
                    int x2 = x1 + 1;
                    int y2 = y1 + 1;

                    var q11_idx = y1 * TerrainSize + x1;
                    var q11 = gpu_terrain[q11_idx];

                    var q12_idx = q11_idx + TerrainSize;
                    var q12 = gpu_terrain[q12_idx];

                    // First interpolation across rows (line)
                    var q1_line = q11 + (caster_line - y1) * (q12 - q11);

                    var q21_idx = q11_idx + 1;
                    var q21 = gpu_terrain[q21_idx];

                    var q22_idx = q11_idx + TerrainSize + 1;
                    var q22 = gpu_terrain[q22_idx];

                    // Second interpolation across rows
                    var q2_line = q21 + (caster_line - y1) * (q22 - q21);

                    // Interpolate across samples and convert to meters
                    var caster_height = q1_line + (caster_sample - x1) * (q2_line - q1_line);
                    caster_height *= 0.5f;

                    GetVector3d(caster_line, caster_sample, caster_height, out float caster_x, out float caster_y, out float caster_z);
                    caster_z *= flip;

                    var dx = caster_x - center_x;
                    var dy = caster_y - center_y;
                    var d = GPUMath.Sqrt(dx * dx + dy * dy); // horizontal distance in moon radius units

                    var light_ray_height = caster_z - slope * d;  // negative slope gets higher as light ray goes toward the center
                    var ray_rise_height = light_ray_height - center_z;  // moon radius units
                    var ray_rise_meters = ray_rise_height * 1000f;

                    // Alternative
                    //var dInMeters = d * 1000f;
                    //var deltaHeightInMeters = (caster_z - center_z) * 1000f;
                    //var rise2 = deltaHeightInMeters - dInMeters * slope;
                    
                    rise = GPUMath.Max(rise, ray_rise_meters);
                }
            }

            gpu_rise[rise_idx] = rise;
        }

        // Returns the position in km
        static void GetVector3d(float line, float sample, float height_meters, out float x, out float y, out float z)
        {
            var radius = MoonRadius + height_meters / 1000f;
            GetLatLon(line, sample, out float lat, out float lon);
            z = radius * GPUMath.Sin(lat);
            var c = radius * GPUMath.Cos(lat);
            x = c * GPUMath.Cos(lon);  // TODO: Not sure about these
            y = c * GPUMath.Sin(lon);
        }

        static void GetLatLon(float line, float sample, out float latitude, out float longitude)
        {
            var x = (sample - S0) * Scale;
            var y = (L0 - line) * Scale;
            var P = GPUMath.Sqrt(x * x + y * y);
            var C = 2f * GPUMath.Atan2(P, 2f * MoonRadius);
            latitude = GPUMath.Asin(GPUMath.Cos(C) * GPUMath.Sin(LatP) + (y == 0 ? 0 : y * GPUMath.Sin(C) * GPUMath.Cos(LatP) / P));
            longitude = LonP + GPUMath.Atan2(x, y * LonFactor);
        }

        #region testing

        #endregion

        #region Generate Rise Height Dataset

        public List<(TerrainPatch,float[])> GenerateRiseHeightPatches(List<TerrainPatch> queue)
        {
            var result = new List<(TerrainPatch, float[])>();

            Debug.Assert(LonFactor == (float)InMemoryTerrainManager.LonFactor);
            Debug.Assert(LatP == (float)InMemoryTerrainManager.LatP);
            Terrain = LunarHorizon.Terrain;

            var stopwatchOuter = new Stopwatch();
            stopwatchOuter.Start();
            LunarHorizon.Singleton?.SetStatus(true);
            Debug.Assert(LunarHorizon.Terrain != null);
            if (queue == null)
                queue = ReadShadowCalculationQueue();

            var patchDictionary = new Dictionary<int, TerrainPatch>();
            BoundingBox = null;

            var interval = LightingInterval.MetonicCycle;
            interval.Step = TimeSpan.FromMinutes(20);
            PrepareSlopeArray(interval);
            WriteSlopeArray();
            FillRangeArray();
            cpu_range_limit = new int[cpu_slope.Length];

            using (var context = new Context())
            {
                if (!GetAccelerator(AcceleratorType.Cuda, out AcceleratorId aid))  // DefaultAcceleratorType
                    return result;
                using (var accelerator = Accelerator.Create(context, aid))
                {
                    using (var gpu_terrain = accelerator.Allocate<short>(LunarHorizon.Terrain.Data.Length))
                    using (var gpu_range = accelerator.Allocate<float>(cpu_range.Length))
                    using (var gpu_range_limit = accelerator.Allocate<int>(cpu_range_limit.Length))
                    using (var gpu_slope = accelerator.Allocate<float>(cpu_slope.Length))
                    using (var gpu_rise = accelerator.Allocate<float>(cpu_rise.Length))
                    {
                        gpu_terrain.CopyFrom(LunarHorizon.Terrain.Data, 0, 0, LunarHorizon.Terrain.Data.Length);
                        gpu_range.CopyFrom(cpu_range, 0, 0, cpu_range.Length);
                        gpu_slope.CopyFrom(cpu_slope, 0, 0, cpu_slope.Length);

                        var kernel1 = accelerator.LoadAutoGroupedStreamKernel<Index2, int, int, int, int, ArrayView<short>, ArrayView<float>, ArrayView<float>, ArrayView<float>, ArrayView<int>>(NearFieldKernel1);

                        while (queue.Count > 0)
                        {
                            var center = queue[0];
                            Console.WriteLine($"Starting patch id=[{center.Id.X},{center.Id.Y}] ...");
                            try
                            {
                                var stopwatch = Stopwatch.StartNew();

                                for (var i = 0; i < cpu_rise.Length; i++)
                                    cpu_rise[i] = float.MinValue;
                                FillRangeLimit(center);
                                gpu_range_limit.CopyFrom(cpu_range_limit, 0, 0, cpu_range_limit.Length);
                                gpu_rise.CopyFrom(cpu_rise, 0, 0, cpu_rise.Length);

                                var launchDimension = new Index2(TerrainPatch.DefaultSize, TerrainPatch.DefaultSize);
                                //var launchDimension = new Index2(1, 1);

                                var batch = 32;
                                for (var slope_idx_start = 0; slope_idx_start < cpu_slope.Length; slope_idx_start += batch)
                                {
                                    var slope_idx_stop = Math.Min(slope_idx_start + batch, cpu_slope.Length);
                                    kernel1(launchDimension, center.Line, center.Sample, slope_idx_start, slope_idx_stop, gpu_terrain, gpu_range, gpu_slope, gpu_rise, gpu_range_limit);
                                    accelerator.Synchronize();  // Needed?  Note location.  Was right after kernel call
                                }
                                gpu_rise.CopyTo(cpu_rise, 0, 0, cpu_rise.Length);

                                stopwatch.Stop();

                                var copy = new float[cpu_rise.Length];
                                Array.Copy(cpu_rise, copy, cpu_rise.Length);
                                result.Add((center, copy));

                                // Update queue on disk
                                queue.RemoveAt(0);

                                Console.WriteLine($"  {queue.Count} patches remaining.  Est. completion at {DateTime.Now.AddMinutes(queue.Count * stopwatch.Elapsed.TotalMinutes)}");
                            }
                            catch (Exception e1)
                            {
                                Console.WriteLine(e1);
                                Console.WriteLine(e1.StackTrace);
                            }
                            LunarHorizon.Singleton?.SetStatus(false);
                        }
                    }
                    Console.WriteLine($"Finished queue.  time={stopwatchOuter.Elapsed}.");
                }
            }
            return result;
        }



        #endregion
    }
}
*/