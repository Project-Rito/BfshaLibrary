using System.Collections.Generic;
using System.Diagnostics;
using BfshaLibrary.Core;

namespace BfshaLibrary
{
    /// <summary>
    /// </summary>
    [DebuggerDisplay(nameof(UniformVar))]
    public class UniformVar : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        public string Name { get; set; }

        public int Index { get; set; }

        public ushort Offset { get; set; }

        public byte BlockIndex { get; set; }

        public ushort GX2Count { get; set; }
        public byte GX2Type { get; set; }
        public byte GX2ParamType { get; set; }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            if (loader.IsSwitch)
            {
                Name = loader.LoadString();
                Index = loader.ReadInt32();
                Offset = loader.ReadUInt16();
                BlockIndex = loader.ReadByte();
                byte padding = loader.ReadByte();
            }
            else
            {
                Index = loader.ReadInt32();
                GX2Count = loader.ReadUInt16();
                GX2Type = loader.ReadByte();
                BlockIndex = loader.ReadByte();
                Offset = loader.ReadUInt16();
                GX2ParamType = loader.ReadByte();
                loader.ReadByte();
                Name = loader.LoadString();
            }
        }

        void IResData.Save(BfshaFileSaver saver)
        {
            if (saver.IsSwitch)
            {
                saver.SaveString(Name);
                saver.Write(Index);
                saver.Write((ushort)Offset);
                saver.Write((byte)BlockIndex);
                saver.Seek(1);
            }
            else
            {
                saver.Write(Index);
                saver.Write(GX2Count);
                saver.Write(GX2Type);
                saver.Write((byte)BlockIndex);
                saver.Write((ushort)Offset);
                saver.Write((byte)GX2ParamType);
                saver.Seek(1);
                saver.SaveString(Name);
            }
        }
    }
}