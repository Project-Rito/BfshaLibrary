using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BfshaLibrary.Core;
using System.Diagnostics;

namespace BfshaLibrary
{
    [DebuggerDisplay(nameof(ShaderProgram))]
    public class ShaderReflectionData : ShaderCodeData, IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public ResDict ShaderInputDictionary { get; set; }

        public ResDict ShaderOutputDictionary { get; set; }

        public ResDict ShaderSamplerDictionary { get; set; }

        public ResDict ShaderConstantBufferDictionary { get; set; }

        public ResDict ShaderUnorderedAccessBufferDictionary { get; set; }

        public int ShaderOutput { get; set; }
        public int ShaderSamplerOffset { get; set; }
        public int ConstBufferOffset { get; set; }
        public int SamplerSlotArrayOffset { get; set; }
        public int ComputeWorkGroupX { get; set; }
        public int ComputeWorkGroupY { get; set; }
        public int ComputeWorkGroupZ { get; set; }
        public int ImageOffset { get; set; }
        public long ImageDicOffset { get; set; }

        public int[] AttributeSlots { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            var loaderNX = loader as Switch.Core.BfshaFileSwitchLoader;

            ShaderInputDictionary = loaderNX.LoadDict<ResString>();
            ShaderOutputDictionary = loaderNX.LoadDict<ResString>();
            ShaderSamplerDictionary = loaderNX.LoadDict<ResString>();
            ShaderConstantBufferDictionary = loaderNX.LoadDict<ResString>();
            loader.ReadUInt64();

            // ShaderUnorderedAccessBufferDictionary = loaderNX.LoadDict<ResString>();
            ShaderOutput = loader.ReadInt32();
            ShaderSamplerOffset = loader.ReadInt32();
            ConstBufferOffset = loader.ReadInt32();
            uint numAttributeSlots = loader.ReadUInt32();
            uint attributeSlotArrayOffset = loader.ReadUInt32();
            ComputeWorkGroupX = loader.ReadInt32();
            ComputeWorkGroupY = loader.ReadInt32();
            ComputeWorkGroupZ = loader.ReadInt32();
            ImageOffset = loader.ReadInt32();
            ImageDicOffset = loader.ReadInt64();
            loader.ReadInt64(); //padding

            if (numAttributeSlots > 0) {
                AttributeSlots = loader.LoadCustom(() => loader.ReadInt32s((int)numAttributeSlots), attributeSlotArrayOffset);
            }
        }

        void IResData.Save(BfshaFileSaver saver)
        {
        }
    }
}
