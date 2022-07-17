using System.Collections.Generic;
using System.Diagnostics;
using BfshaLibrary.Core;

namespace BfshaLibrary
{
    /// <summary>
    /// Represents an FMDL subfile in a <see cref="ShaderModel"/>, storing model vertex data, skeletons and used materials.
    /// </summary>
    [DebuggerDisplay(nameof(Sampler))]
    public class Sampler : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string AltAnnotation { get; set; }

        public byte Index { get; set; }

        public byte GX2Type { get; set; }
        public byte GX2Count { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            if (loader.IsSwitch)
            {
                AltAnnotation = loader.LoadString();
                Index = (byte)loader.ReadByte();
                loader.Seek(7);
            }
            else
            {
                Index = (byte)loader.ReadByte();
                GX2Type = (byte)loader.ReadByte(); //type
                GX2Count = (byte)loader.ReadByte(); //array
                loader.ReadByte();//padding
                AltAnnotation = loader.LoadString();
            }
        }

        void IResData.Save(BfshaFileSaver saver)
        {
            if (saver.IsSwitch)
            {
                saver.SaveString(AltAnnotation);
                saver.Write((byte)Index);
                saver.Seek(7);
            }
            else
            {
                saver.Write((byte)Index);
                saver.Write((byte)GX2Type);
                saver.Write((byte)GX2Count);
                saver.Write((byte)0);
                saver.SaveString(AltAnnotation);
            }
        }
    }
}