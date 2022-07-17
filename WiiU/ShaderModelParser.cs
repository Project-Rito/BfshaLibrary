using System;
using System.Collections.Generic;
using BfshaLibrary.Core;
using BfshaLibrary;
using BfshaLibrary.WiiU.Core;

namespace BfshaLibrary.WiiU
{
    public class ShaderModelParser
    {
        public static void Load(BfshaFileWiiULoader loader, ShaderModel shaderModel)
        {
            shaderModel.StaticKeyLength = loader.ReadByte();
            shaderModel.DynamicKeyLength = loader.ReadByte();
            ushort staticOptionCount = loader.ReadUInt16();
            ushort dynamicOptionCount = loader.ReadUInt16();
            ushort programCount = loader.ReadUInt16();
            byte attributeCount = (byte)loader.ReadByte();
            byte samplerCount = (byte)loader.ReadByte();
            byte uniformBlockCount = (byte)loader.ReadByte();
            shaderModel.MaxRingItemSize = (uint)loader.ReadByte();
            loader.ReadUInt16();
            loader.ReadUInt16();
            loader.ReadUInt32();
            uint uniformCount = loader.ReadUInt32();
            loader.ReadInt32();
            int defaultProgramIndex = loader.ReadInt32();
            shaderModel.Name = loader.LoadString();
            loader.ReadUInt32();
            loader.ReadUInt32();

            shaderModel.StaticOptions = loader.LoadDictValues<ShaderOption>();
            shaderModel.DynamiOptions = loader.LoadDictValues<ShaderOption>();
            shaderModel.Attributes = loader.LoadDictValues<Attribute>();
            shaderModel.Samplers = loader.LoadDictValues<Sampler>();
            shaderModel.UniformBlocks = loader.LoadDictValues<UniformBlock>();
            uint uniformArrayOffset = loader.ReadOffset();
            shaderModel.Programs = loader.LoadList<ResShaderProgram>(programCount);
            uint keyTableOffset = loader.ReadOffset();
            uint shaderArchiveOffset = loader.ReadOffset();
            loader.ReadUInt32();
            loader.ReadUInt32();
            uint infoOffset = loader.ReadUInt32();


            //Static and dynamic keys for each program
            shaderModel.KeyTable = loader.LoadCustom(() => loader.ReadInt32s(
                (shaderModel.StaticKeyLength + shaderModel.DynamicKeyLength) * shaderModel.ProgramCount),
                keyTableOffset);
        }
    }
}
