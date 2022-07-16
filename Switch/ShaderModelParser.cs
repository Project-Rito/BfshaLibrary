using System;
using System.Collections.Generic;
using BfshaLibrary.Core;
using BfshaLibrary;
using BfshaLibrary.Switch.Core;

namespace BfshaLibrary.Switch
{
    public class ShaderModelParser
    {
        public static void Load(BfshaFileSwitchLoader loader, ShaderModel shaderModel)
        {
            shaderModel.Name = loader.LoadString();
            shaderModel.StaticOptions = loader.LoadDictValues<ShaderOption>();
            shaderModel.DynamiOptions = loader.LoadDictValues<ShaderOption>();
            shaderModel.Attributes = loader.LoadDictValues<Attribute>();
            shaderModel.Samplers = loader.LoadDictValues<Sampler>();

            if (loader.BfshaFile.VersionMajor2 >= 8)
            {
                loader.ReadInt64();
                loader.ReadInt64();
            }

            shaderModel.UniformBlocks = loader.LoadDictValues<UniformBlock>();
            long uniformArrayOffset = loader.ReadOffset();

            if (loader.BfshaFile.VersionMajor2 >= 7)
            {
                loader.ReadInt64();
                loader.ReadInt64();
                loader.ReadInt64();
            }

            long shaderProgramArrayOffset = loader.ReadOffset();
            long keyTableOffset = loader.ReadOffset();
            long shaderArchiveOffset = loader.ReadOffset();
            long glShaderInfoOffset = loader.ReadOffset();
            long shaderFileOffset = loader.ReadOffset();
            long MutexTypePtr = loader.ReadOffset();
            long userPointer = loader.ReadOffset();
            long updateCallback = loader.ReadOffset();

            if (loader.BfshaFile.VersionMajor2 >= 7)
            {
                //padding
                loader.ReadInt64();
                loader.ReadInt64();
            }

            uint uniformCount = loader.ReadUInt32();
            if (loader.BfshaFile.VersionMajor2 >= 7) {
                uint shaderStorageCount = loader.ReadUInt32();
            }
            int defaultProgramIndex = loader.ReadInt32();
            ushort staticOptionCount = loader.ReadUInt16();
            ushort dynamicOptionCount = loader.ReadUInt16();
            ushort programCount = loader.ReadUInt16();

            if (loader.BfshaFile.VersionMajor2 < 7)
                loader.ReadUInt16();

            shaderModel.StaticKeyLength = loader.ReadByte();
            shaderModel.DynamicKeyLength = loader.ReadByte();

            byte attribCount = loader.ReadByte();
            byte samplerCount = loader.ReadByte();

            if (loader.BfshaFile.VersionMajor2 >= 8)
                loader.ReadByte();

            byte uniformBlockCount = loader.ReadByte();
            loader.ReadByte();
            byte[] systemBlockIndices = loader.ReadBytes(4);
            if (loader.BfshaFile.VersionMajor2 >= 8)
                loader.ReadBytes(11);
            else if (loader.BfshaFile.VersionMajor2 >= 7)
                loader.ReadBytes(4);
            else
                loader.ReadBytes(6);

            int bnshSize = 0;
            if (shaderFileOffset != 0)
            {
                //Go into the bnsh file and get the file size
                using (loader.TemporarySeek(shaderFileOffset + 0x1C, System.IO.SeekOrigin.Begin)) {
                    bnshSize = (int)loader.ReadUInt32();
                }
            }

            shaderModel.BnshFileStream = new SubStream(loader.BaseStream, shaderFileOffset, bnshSize);
            shaderModel.UniformVars = loader.LoadList<UniformVar>((int)uniformCount, (uint)uniformArrayOffset);
            shaderModel.Programs = loader.LoadList<ResShaderProgram>(programCount, (uint)shaderProgramArrayOffset);

            shaderModel.shaderOffset = shaderFileOffset;

            //Static and dynamic keys for each program
            shaderModel.KeyTable = loader.LoadCustom(() => loader.ReadInt32s(
                (shaderModel.StaticKeyLength + shaderModel.DynamicKeyLength) * shaderModel.ProgramCount),
                (uint)keyTableOffset);
        }
    }
}
