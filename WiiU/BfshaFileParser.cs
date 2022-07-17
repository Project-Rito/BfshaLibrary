using System;
using System.Collections.Generic;
using System.Linq;
using BfshaLibrary.WiiU.Core;

namespace BfshaLibrary.WiiU
{
    public class BfshaFileParser
    {
        public static void Load(BfshaFileWiiULoader loader, BfshaFile resFile)
        {
            loader.CheckSignature("FSHA");
            resFile.Version = loader.ReadUInt32();
            resFile.SetVersionInfo(resFile.Version);
            resFile.ByteConverter = loader.ReadByteConverter();
            ushort headerSize = loader.ReadUInt16();
            uint sizFile = loader.ReadUInt32();
            resFile.Alignment = loader.ReadUInt32();
            resFile.Name = loader.LoadString();
            uint stringPoolSize = loader.ReadUInt32();
            uint stringPoolOffset = loader.ReadOffset();
            resFile.Path = loader.LoadString();
            ushort modelCount = loader.ReadUInt16();
            ushort flag = loader.ReadUInt16();
            loader.ReadUInt32();
            resFile.ShaderModels = loader.LoadDict<ShaderModel>();
            uint userPointer = loader.ReadUInt32();
            uint updateCallback = loader.ReadUInt32();
        }
    }
}
