using System.Collections.Generic;
using System.Diagnostics;
using BfshaLibrary.Core;

namespace BfshaLibrary
{
    /// <summary>
    /// Represents an FMDL subfile in a <see cref="ShaderModel"/>, storing model vertex data, skeletons and used materials.
    /// </summary>
    [DebuggerDisplay(nameof(Attribute))]
    public class Attribute : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public byte Index { get; set; }

        public byte Location { get; set; }

        public byte GX2Type { get; set; }
        public byte GX2Count { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            Index = loader.ReadByte();
            if (!loader.IsSwitch)
            {
                GX2Type = loader.ReadByte();
                GX2Count = loader.ReadByte();
            }
            Location = loader.ReadByte();
        }

        void IResData.Save(BfshaFileSaver saver)
        {
            saver.Write((byte)Index);
            if (!saver.IsSwitch)
            {
                saver.Write((byte)GX2Type);
                saver.Write((byte)GX2Count);
            }
            saver.Write((byte)Location);
        }
    }
}