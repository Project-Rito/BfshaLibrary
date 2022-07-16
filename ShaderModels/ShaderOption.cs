using System.Collections.Generic;
using System.Diagnostics;
using BfshaLibrary.Core;

namespace BfshaLibrary
{
    /// <summary>
    /// Represents an FMDL subfile in a <see cref="ShaderModel"/>, storing model vertex data, skeletons and used materials.
    /// </summary>
    [DebuggerDisplay(nameof(Attribute))]
    public class ShaderOption : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in <see cref="ResDict{StaticOption}"/>
        /// instances.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets attached <see cref="UniformBlock"/> names
        /// </summary>
        public ResDict<ResUint32> ChoiceDict { get; set; }

        /// <summary>
        /// A list of choice value indices.
        /// </summary>
        public uint[] ChoiceValues { get; set; }

        /// <summary>
        /// Gets or sets attached <see cref="UniformBlock"/> names
        /// </summary>
        public string[] choices
        {
            get
            {
                return ChoiceDict.GetKeys();
            }
        }

        /// <summary>
        /// Gets or sets attached <see cref="UniformBlock"/> names
        /// </summary>
        public string defaultChoice
        {
            get
            {
                return ChoiceDict.GetKey(defaultIndex);
            }
        }

        public byte defaultIndex { get; set; }
        public ushort branchOffset { get; set; }
        public byte flag { get; set; }
        public byte keyOffset { get; set; }
        public byte bit32Index { get; set; }
        public byte bit32Shift { get; set; }
        public uint bit32Mask { get; set; }

        public int GetChoiceIndex(int key)
        {
            //Find choice index with mask and shift
            return (int)((key & bit32Mask) >> bit32Shift);
        }


        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            if (loader.IsSwitch)
            {
                Name = loader.LoadString();
                ChoiceDict = loader.LoadDict<ResUint32>();
                uint choiceValuesOffset = loader.ReadOffset();
                byte choiceCount = loader.ReadByte();
                defaultIndex = loader.ReadByte();
                branchOffset = loader.ReadUInt16(); // Uniform block offset.
                flag = loader.ReadByte();
                keyOffset = loader.ReadByte();
                bit32Index = loader.ReadByte();
                bit32Shift = loader.ReadByte();
                bit32Mask = loader.ReadUInt32();
                uint padding = loader.ReadUInt32();

                ChoiceValues = loader.LoadCustom( () =>
                {
                    return loader.ReadUInt32s(choiceCount);
                }, choiceValuesOffset);
            }
            else
            {
                byte choiceCount = loader.ReadByte();
                defaultIndex = loader.ReadByte();
                branchOffset = loader.ReadUInt16(); // Uniform block offset.
                flag = loader.ReadByte();
                keyOffset = loader.ReadByte();
                bit32Index = loader.ReadByte();
                bit32Shift = loader.ReadByte();
                bit32Mask = loader.ReadUInt32();
                Name = loader.LoadString();
                ChoiceDict = loader.LoadDict<ResUint32>();
                uint choiceValuesOffset = loader.ReadOffset();

                ChoiceValues = loader.LoadCustom(() =>
                {
                    return loader.ReadUInt32s(choiceCount);
                }, choiceValuesOffset);
            }
        }

        void IResData.Save(BfshaFileSaver saver)
        {
            if (saver.IsSwitch)
            {
                saver.SaveString(Name);
                saver.SaveDict(ChoiceDict);
                saver.Write((byte)ChoiceValues?.Length);
                saver.Write((byte)defaultIndex);
                saver.Write((ushort)branchOffset);
                saver.Write((byte)flag);
                saver.Write((byte)keyOffset);
                saver.Write((byte)bit32Index);
                saver.Write((byte)bit32Shift);
                saver.Write((byte)bit32Mask);
                saver.SaveString(Name);
                saver.Write(0);
            }
            else
            {
                saver.Write((byte)ChoiceValues?.Length);
                saver.Write((byte)defaultIndex);
                saver.Write((ushort)branchOffset);
                saver.Write((byte)flag);
                saver.Write((byte)keyOffset);
                saver.Write((byte)bit32Index);
                saver.Write((byte)bit32Shift);
                saver.Write((byte)bit32Mask);
                saver.SaveString(Name);
                saver.SaveDict(ChoiceDict);
                saver.SaveCustom(ChoiceValues, () => saver.Write(ChoiceValues));
            }
        }

        public int GetStaticKey()
        {
            var key = bit32Index;
            return (int)((key & bit32Mask) >> bit32Shift);
        }

        public int GetDynamicKey()
        {
            var key = bit32Index - keyOffset;
            return (int)((key & bit32Mask) >> bit32Shift);
        }
    }
}