// Author: Kevin Rucker
// License: BSD 3-Clause
// Copyright (c) 2014 - 2015, Kevin Rucker
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its contributors
//    may be used to endorse or promote products derived from this software without
//    specific prior written permission.
//
// Disclaimer:
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace Minimal.Imaging
{
    /// <summary>
    /// Provides method for creating thumbnail bitmaps
    /// </summary>
    public static class Thumbnail
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Creates a thumbnail from an image stream
        /// </summary>
        /// <param name="source">The image stream</param>
        /// <param name="size">Size in pixels (largest source dimention will be this size in the thumbnail image)</param>
        /// <returns>Resized <see cref="Bitmap"/></returns>
        public static Bitmap Create(Stream source, int size)
        {
            var lockTaken = false;
            Bitmap output = null;
            try 
            {
                Monitor.Enter(_lock, ref lockTaken);
                using (var workingBitmap = new Bitmap(source))
                {
                    // Determine scale based on requested size (this preserves aspect ratio)
                    Decimal scale;
                    if (((Decimal)workingBitmap.Width / (Decimal)size) > ((Decimal)workingBitmap.Height / (Decimal)size))
                    {
                        scale = (Decimal)workingBitmap.Width / (Decimal)size;
                    }
                    else 
                    {
                        scale = (Decimal)workingBitmap.Height / (Decimal)size;
                    }

                    // Calculate new height/width
                    var newHeight = (Int32)((Decimal)workingBitmap.Height / scale);
                    var newWidth = (Int32)((Decimal)workingBitmap.Width / scale);

                    // Create blank BitMap of appropriate size
                    output = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);

                    // Create Graphics surface
                    using (var g = Graphics.FromImage(output))
                    {
                        g.CompositingMode = CompositingMode.SourceCopy;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        var destRectangle = new Rectangle(0, 0, newWidth, newHeight);

                        // Use Graphics surface to draw resized BitMap to blank BitMap
                        g.DrawImage(workingBitmap, destRectangle, 0, 0, workingBitmap.Width, workingBitmap.Height, GraphicsUnit.Pixel);
                    }
                }
            }
            catch 
            {
                output = null;
            }
            finally 
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }

            return output;
        }
    }
}
