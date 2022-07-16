using System;
using System.Collections.Generic;
using System.IO;
using BfshaLibrary.Core;

namespace BfshaLibrary.WiiU
{
    public class GX2VertexHeader : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public byte[] Data { get; set; }
        public uint[] Regs { get; set; }
        public uint Mode { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            Regs = loader.ReadUInt32s(52);
            uint size = loader.ReadUInt32();
            Data = loader.LoadCustom(() => loader.ReadBytes((int)size));
            Mode = loader.ReadUInt32();
        }

        void IResData.Save(BfshaFileSaver saver)
        {

        }
    }
}
