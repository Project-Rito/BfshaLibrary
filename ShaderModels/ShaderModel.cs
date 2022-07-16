using System.Collections.Generic;
using System.Diagnostics;
using BfshaLibrary.Core;
using System.ComponentModel;
using System;
using System.IO;

namespace BfshaLibrary
{
    /// <summary>
    /// Represents an FMDL subfile in a <see cref="ResFile"/>, storing model vertex data, skeletons and used materials.
    /// </summary>
    [DebuggerDisplay(nameof(ShaderModel) + " {" + nameof(Name) + "}")]
    public class ShaderModel : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in <see cref="ResDict{Model}"/>
        /// instances.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The parent bfsha binary of the file.
        /// </summary>
        public BfshaFile ParentFile { get; set; }

        /// <summary>
        /// The binary shader file used to store programs, variations and binary data
        /// </summary>
        public Stream BnshFileStream { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="ShaderOption"/> (FMDL) instances.
        /// </summary>
        public ResDict<ShaderOption> StaticOptions { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="DynamiOption"/> (FMDL) instances.
        /// </summary>
        public ResDict<ShaderOption> DynamiOptions { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="Attribute"/> (FMDL) instances.
        /// </summary>
        public ResDict<Attribute> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="Sampler"/> (FMDL) instances.
        /// </summary>
        public ResDict<Sampler> Samplers { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="UniformBlock"/> (FMDL) instances.
        /// </summary>
        public ResDict<UniformBlock> UniformBlocks { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="UniformVar"/> (FMDL) instances.
        /// </summary>
        public IList<UniformVar> UniformVars { get; set; }

        /// <summary>
        /// Gets or sets the program list of the shader model.
        /// </summary>
        public IList<ResShaderProgram> Programs { get; set; }

        /// <summary>
        /// Gets the program count of the shader model.
        /// </summary>
        public ushort ProgramCount => (ushort)Programs.Count;

        /// <summary>
        /// Gets or sets the key table used to lookup shader option choices used per program.
        /// </summary>
        public int[] KeyTable { get; set; }

        /// <summary>
        /// Gets the number of static keys used per program.
        /// </summary>
        public int StaticKeyLength { get; internal set; }

        /// <summary>
        /// Gets the number of dynamic keys used per program.
        /// </summary>
        public int DynamicKeyLength { get; internal set; }

        /// <summary>
        /// The max ring item size;
        /// </summary>
        public uint MaxRingItemSize { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the program index from a set of given option choices.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public int GetProgramIndex(Dictionary<string, string> options)
        {
            for (int i = 0; i < ProgramCount; i++)
            {
                if (IsValidProgram(i, options))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Prints all the program keys.
        /// </summary>
        public void PrintProgramKeys()
        {
            for (int i = 0; i < ProgramCount; i++)
                PrintProgramKeys(i);
        }

        /// <summary>
        /// Prints specificed program keys from the given index.
        /// </summary>
        /// <param name="programIndex"></param>
        public void PrintProgramKeys(int programIndex) {
            Console.WriteLine($"--------------------------------------------------------");

            int numKeysPerProgram = StaticKeyLength + DynamicKeyLength;
            int baseIndex = numKeysPerProgram * programIndex;
            for (int j = 0; j < this.StaticOptions.Count; j++) {
                var option = this.StaticOptions[j];
                int choiceIndex = option.GetChoiceIndex(KeyTable[baseIndex + option.bit32Index]);
                if (choiceIndex > option.choices.Length || choiceIndex == -1)
                    throw new Exception($"Invalid choice index in key table! {option.Name} index {choiceIndex}");

                Console.WriteLine($"{option.Name} index {choiceIndex} choice {option.choices[choiceIndex]}");
            }

            for (int j = 0; j < this.DynamiOptions.Count; j++) {
                var option = this.DynamiOptions[j];
                int ind = option.bit32Index - option.keyOffset;
                int choiceIndex = option.GetChoiceIndex(KeyTable[baseIndex + StaticKeyLength + ind]);
                if (choiceIndex > option.choices.Length || choiceIndex == -1)
                    throw new Exception($"Invalid choice index in key table! {option.Name} index {choiceIndex}");

                Console.WriteLine($"{option.Name} index {choiceIndex} choice {option.ChoiceDict.GetKey(choiceIndex)}");
            }
        }

        /// <summary>
        /// Determines if the specified program is valid with the given option choice combinations.
        /// This method checks the key table and determines if the choices match,
        /// </summary>
        /// <param name="programIndex"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool IsValidProgram(int programIndex, Dictionary<string, string> options)
        {
            int numKeysPerProgram = StaticKeyLength + DynamicKeyLength;

            //Static key (total * program index)
            int baseIndex = numKeysPerProgram * programIndex;

            for (int j = 0; j < this.StaticOptions.Count; j++)
            {
                var option = this.StaticOptions[j];
                //The options must be the same between bfres and bfsha
                if (!options.ContainsKey(option.Name))
                    continue;

                //Get key in table
                int choiceIndex = option.GetChoiceIndex(KeyTable[baseIndex + option.bit32Index]);
                if (choiceIndex > option.choices.Length)
                    throw new Exception($"Invalid choice index in key table!");

                //If the choice is not in the program, then skip the current program
                var choice = option.ChoiceDict.GetKey(choiceIndex);
                if (options[option.Name] != choice)
                    return false;
            }

            for (int j = 0; j < this.DynamiOptions.Count; j++)
            {
                var option = this.DynamiOptions[j];
                if (!options.ContainsKey(option.Name))
                    continue;

                int ind = option.bit32Index - option.keyOffset;
                int choiceIndex = option.GetChoiceIndex(KeyTable[baseIndex + StaticKeyLength + ind]);
                if (choiceIndex > option.choices.Length)
                    throw new Exception($"Invalid choice index in key table!");

                var choice = option.ChoiceDict.GetKey(choiceIndex);
                if (options[option.Name] != choice)
                    return false;
            }
            return true;
        }

        internal long shaderOffset;

        void IResData.Load(BfshaFileLoader loader)
        {
          if (loader.IsSwitch)
                Switch.ShaderModelParser.Load((Switch.Core.BfshaFileSwitchLoader)loader, this);
          else
                WiiU.ShaderModelParser.Load((WiiU.Core.BfshaFileWiiULoader)loader, this);
        }

        void IResData.Save(BfshaFileSaver saver)
        {
  
        }

        public ResShaderProgram GetShaderProgram(int index) {
            return Programs[index];
        }

        public ShaderVariation GetShaderVariation(ResShaderProgram program)
        {
            if (BnshFileStream == null)
                return null;

            using (var reader = new Switch.Core.BfshaFileSwitchLoader(ParentFile, BnshFileStream, true)) {
                return reader.Load<ShaderVariation>(program.variationOffset - shaderOffset);
            }
        }
    }
}