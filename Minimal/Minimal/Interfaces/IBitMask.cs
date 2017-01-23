using System;

namespace Minimal.Interfaces
{
    /// <summary>
    /// The interface for BitMap class implementations
    /// </summary>
    public interface IBitMask
    {
        /// <summary>
        /// The base value of the bitmap
        /// </summary>
        UInt64 BaseValue { get; set; }
        /// <summary>
        /// Get the value of the bit at the supplied position position in the bitmask
        /// </summary>
        /// <param name="bitPosition"></param>
        /// <returns><code>true</code> if the bit is on and <code>false</code> if it is off</returns>
        bool GetBitValue(Int32 bitPosition);
        /// <summary>
        /// Turn on bit at the supplied position in the bitmask
        /// </summary>
        /// <param name="bitPosition"></param>
        void SetBit(Int32 bitPosition);
        /// <summary>
        /// Turn off bit at the supplied position in the bitmask
        /// </summary>
        /// <param name="bitPosition"></param>
        void ClearBit(Int32 bitPosition);
        /// <summary>
        /// Turn on all bits in the bitmask
        /// </summary>
        void SetAll();
        /// <summary>
        /// Turn off all bits in the bitmask
        /// </summary>
        void ClearAll();
    }
}
