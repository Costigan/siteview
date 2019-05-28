using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace siteview.utilities
{
    /// <summary>
    /// Description of BitmapHelper.
    /// </summary>
    public static unsafe class BitmapHelper
    {
        /// <summary>
        /// gets the number of Bits Per Pixel (BPP)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static int BitsPerPixel(System.Drawing.Bitmap bitmap)
        {
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed: return 1;
                case PixelFormat.Format4bppIndexed: return 4;
                case PixelFormat.Format8bppIndexed: return 8;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565: return 16;
                case PixelFormat.Format24bppRgb: return 24;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb: return 32;
                case PixelFormat.Format48bppRgb: return 48;
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb: return 64;
                default: throw new ArgumentException(String.Format("The bitmap's pixel format of {0} was not recognised.", bitmap.PixelFormat), "bitmap");
            }
        }

        /// <summary>
        /// Bytes per row for a bitmap, assuming 1 bit per pixel
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public static int BytesPerRow(int width) => 4 * ((((width + 7) / 8) + 3) / 4);

        /*
        public static int BytesPerRow(int width)
        {
            int rem;
            int stride = Math.DivRem(width, 8, out rem);  // divide 8bpp by 8 to get  1bpp
            if (rem > 0) stride += 1;
            Math.DivRem(stride, 4, out rem); // round newStride up to multiple of 4 bytes
            Math.DivRem(4 - rem, 4, out rem);
            stride += rem;
            return stride;
        }
        */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitsPerRow(int width) => 8 * BytesPerRow(width);

        #region Bitmap Data Access

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataBit(byte* data, int index) => (byte)((*(data + (index >> 3)) >> (index & 0x7)) & 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataBit(byte[] data, int bitindex) => (byte)((data[bitindex >> 3] >> (7 - (bitindex & 0x7))) & 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataBit(byte[] data, int imageno, int height, int width, int row, int col)
        {
            int bitindex = imageno * (height * width) + row * width + col;
            return GetDataBit(data, bitindex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataBit(byte* data, int index, byte value)
        {
            byte* wordPtr = data + (index >> 3);
            int v = *wordPtr & (byte)~(0x80 >> (index & 7)); 			// clear bit, note first pixel in the byte is most significant (1000 0000)
            *wordPtr = (byte)(v | ((value & 1) << (7 - (index & 7))));       // set bit, if value is 1
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataBit(byte[] data, int bitindex, byte value)
        {
            int wordPtr = bitindex >> 3;
            int v = data[wordPtr] & (byte)~(1 << (7 - (bitindex & 7))); 			// clear bit, note first pixel in the byte is most significant (1000 0000)
            data[wordPtr] = (byte)(v | ((value & 1) << (7 - (bitindex & 7))));                // set bit, if value is 1  (TODO WORKING HERE!!!!)
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataBit(byte[] data, int imageno, int height, int width, int row, int col, byte value)
        {
            int bitindex = imageno * (height * width) + row * width + col;
            SetDataBit(data, bitindex, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataQBit(byte* data, int index) => (byte)((*(data + (index >> 1)) >> (4 * (index & 1))) & 0xF);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataQBit(byte* data, int index, byte value)
        {
            byte* wordPtr = data + (index >> 1);
            *wordPtr &= (byte)~(0xF0 >> (4 * (index & 1))); // clears qbit located at index, note like bit the qbit corresponding to the first pixel is the most significant (0xF0)
            *wordPtr |= (byte)((value & 0x0F) << (4 - (4 * (index & 1)))); // applys qbit to n
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataByte(byte* data, int index) => *(data + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataByte(byte* data, int index, byte value) => *(data + index) = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetDataUInt16(ushort* data, int index) => *(data + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataUInt16(ushort* data, int index, ushort value) => *(data + index) = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetDataUInt32(uint* data, int index) => *(data + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataUInt32(uint* data, int index, uint value) => *(data + index) = value;

        #endregion

        #region PixelFormat conversion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertRgb555ToRGBA(uint val)
        {
            uint red = ((val & 0x7C00) >> 10);
            uint green = ((val & 0x3E0) >> 5);
            uint blue = (val & 0x1F);

            return ((red << 3 | red >> 2) << 24) |
                ((green << 3 | green >> 2) << 16) |
                ((blue << 3 | blue >> 2) << 8) |
                0xFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertRgb565ToRGBA(uint val)
        {
            uint red = ((val & 0xF800) >> 11);
            uint green = ((val & 0x7E0) >> 5);
            uint blue = (val & 0x1F);

            return ((red << 3 | red >> 2) << 24) |
                ((green << 2 | green >> 4) << 16) |
                ((blue << 3 | blue >> 2) << 8) |
                0xFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertArgb1555ToRGBA(uint val)
        {
            uint alpha = ((val & 0x8000) >> 15);
            uint red = ((val & 0x7C00) >> 10);
            uint green = ((val & 0x3E0) >> 5);
            uint blue = (val & 0x1F);

            return ((red << 3 | red >> 2) << 24) |
                ((green << 3 | green >> 2) << 16) |
                ((blue << 3 | blue >> 2) << 8) |
                ((alpha << 8) - alpha); // effectively alpha * 255, only works as alpha will be either 0 or 1
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint EncodeAsRGBA(byte red, byte green, byte blue, byte alpha)
        {
            return (uint)((red << 24) |
                (green << 16) |
                (blue << 8) |
                alpha);
        }

        public static void HSVToRGB(double H, double S, double V, out double R, out double G, out double B)
        {
            if (H == 1.0)
                H = 0.0;

            double step = 1.0 / 6.0;
            double vh = H / step;

            var i = (int)Math.Floor(vh);

            double f = vh - i;
            double p = V * (1.0 - S);
            double q = V * (1.0 - (S * f));
            double t = V * (1.0 - (S * (1.0 - f)));

            switch (i)
            {
                case 0:
                    {
                        R = V;
                        G = t;
                        B = p;
                        break;
                    }
                case 1:
                    {
                        R = q;
                        G = V;
                        B = p;
                        break;
                    }
                case 2:
                    {
                        R = p;
                        G = V;
                        B = t;
                        break;
                    }
                case 3:
                    {
                        R = p;
                        G = q;
                        B = V;
                        break;
                    }
                case 4:
                    {
                        R = t;
                        G = p;
                        B = V;
                        break;
                    }
                case 5:
                    {
                        R = V;
                        G = p;
                        B = q;
                        break;
                    }
                default:
                    {
                        // not possible - if we get here it is an internal error
                        throw new ArgumentException();
                    }
            }
        }

        #endregion

        #region working with drawing bitmaps

        public static Bitmap ToFormat32bppArgb(this Bitmap source)
        {
            if (source.PixelFormat == PixelFormat.Format32bppArgb)
                return source;
            var bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
                g.DrawImageUnscaled(source, 0, 0);
            return bmp;
        }

        public static unsafe Bitmap ToFormat8bppIndexed(byte[,] ary)
        {
            var height = ary.GetLength(0);
            var width = ary.GetLength(1);
            var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            for (var row = 0; row < height; row++)
            {
                var rowptr = (byte*)(bmpdata.Scan0 + row * bmpdata.Stride);
                for (var col = 0; col < width; col++)
                    rowptr[col] = ary[row, col];
            }
            bmp.UnlockBits(bmpdata);
            return bmp;
        }

        public static unsafe Bitmap ToFormat8bppIndexed(float[,] ary, Func<float,byte> conversion = null)
        {
            var f = conversion ?? (v => (byte)v);
            var height = ary.GetLength(0);
            var width = ary.GetLength(1);
            var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            for (var row=0;row<height;row++)
            {
                var rowptr = (byte*)(bmpdata.Scan0 + row * bmpdata.Stride);
                for (var col = 0; col < width; col++)
                    rowptr[col] = f(ary[row, col]);
            }
            bmp.UnlockBits(bmpdata);
            return bmp;
        }

        public static unsafe Bitmap ToFormat8bppIndexed(Bitmap src, Func<byte, byte> conversion = null)
        {
            var f = conversion ?? (v => (byte)v);
            var height = src.Height;
            var width = src.Width;
            System.Diagnostics.Debug.Assert(src.PixelFormat == PixelFormat.Format8bppIndexed);
            var src_data = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, src.PixelFormat);
            var dst = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            var dst_data = dst.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, dst.PixelFormat);
            for (var row = 0; row < height; row++)
            {
                var dst_ptr = (byte*)(dst_data.Scan0 + row * dst_data.Stride);
                var src_ptr = (byte*)(src_data.Scan0 + row * src_data.Stride);
                for (var col = 0; col < width; col++)
                    dst_ptr[col] = f(src_ptr[col]);
            }
            dst.UnlockBits(dst_data);
            src.UnlockBits(src_data);
            dst.Palette = src.Palette;
            return dst;
        }

        /// <summary>
        /// Returns a 2D byte array containing the pixel data or null if the pixel format isn't 8 bit indexed
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[,] Image8bppToByteArray(string path)
        {
            using (var bmp = Image.FromFile(path) as Bitmap)
                return bmp.ToByteArray2();
        }

        #endregion
    }

}
