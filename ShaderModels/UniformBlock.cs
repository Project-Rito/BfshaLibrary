using System.Collections.Generic;
using System.Diagnostics;
using BfshaLibrary.Core;

namespace BfshaLibrary
{
    /// <summary>
    /// Represents an FMDL subfile in a <see cref="ShaderModel"/>, storing model vertex data, skeletons and used materials.
    /// </summary>
    [DebuggerDisplay(nameof(UniformBlock))]
    public class UniformBlock : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the stored <see cref="UniformBlock"/> (FMDL) instances.
        /// </summary>
        public ResDict<UniformVar> Uniforms { get; set; }

        public byte Index { get; set; }

        public BlockType Type { get; set; }

        public ushort Size { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            if (loader.IsSwitch)
            {
                Uniforms = loader.LoadDictValues<UniformVar>();
                long defaultOffset = loader.ReadInt64();
                Index = (byte)loader.ReadByte();
                Type = loader.ReadEnum<BlockType>(true);
                Size = loader.ReadUInt16();
                ushort uniformCount = loader.ReadUInt16();
                ushort padding = loader.ReadUInt16();
            }
            else
            {
                Index = (byte)loader.ReadByte();
                Type = loader.ReadEnum<BlockType>(true);
                Size = loader.ReadUInt16();
                ushort uniformCount = loader.ReadUInt16();
                loader.ReadUInt16();
                Uniforms = loader.LoadDict<UniformVar>();
                long defaultOffset = loader.ReadOffset();
            }
        }

        void IResData.Save(BfshaFileSaver saver)
        {
            if (saver.IsSwitch)
            {

            }
            else
            {
                saver.Write(Index);
                saver.WriteEnum(Type, false);
                saver.Write(Size);
                saver.Write((ushort)Uniforms.Count);
                saver.Write((ushort)0);
                saver.SaveDict(Uniforms);
                saver.Write(0);
            }
        }


        public enum BlockType : byte
        {
            None,
            Material,
            Shape,
            Option,
            Num,
        }

    }
}