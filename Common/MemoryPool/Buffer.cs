﻿using BfshaLibrary.Core;

namespace BfshaLibrary
{
    /// <summary>
    /// Represents a buffer of data uploaded to the  GPU which can hold arbitrary data.
    /// </summary>
    public class Buffer : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// The size of a full vertex in bytes.
        /// </summary>
        public ushort Stride { get; set; }

        /// <summary>
        /// The raw bytes stored for each buffering.
        /// </summary>
        public byte[][] Data { get; set; }
        
        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            uint dataPointer = loader.ReadUInt32();
            uint size = loader.ReadUInt32();
            uint handle = loader.ReadUInt32();
            Stride = loader.ReadUInt16();
            ushort numBuffering = loader.ReadUInt16();
            uint contextPointer = loader.ReadUInt32();
            Data = loader.LoadCustom(() =>
            {
                byte[][] data = new byte[numBuffering][];
                for (int i = 0; i < numBuffering; i++)
                {
                    data[i] = loader.ReadBytes((int)size);
                }
                return data;
            });
        }

        void IResData.Save(BfshaFileSaver saver)
        {
            saver.Write(0); // DataPointer
            saver.Write(Data[0].Length); // Size
            saver.Write(0); // Handle
            saver.Write(Stride);
            saver.Write((ushort)Data.Length); // NumBuffering
            saver.Write(0); // ContextPointer
            saver.SaveBlock(Data, BfshaFileSaver.AlignmentSmall, () =>
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    saver.Write(Data[i]);
                }
            });
        }
    }
}