using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Syroot.BinaryData;
using BfshaLibrary.Core;
using BfshaLibrary.Switch.Core;

namespace BfshaLibrary.Switch
{
    public class BfshaFileParser
    {
        public static void Load(BfshaFileSwitchLoader loader, BfshaFile resFile)
        {
            loader.CheckSignature("FSHA");
            uint padding = loader.ReadUInt32();
            resFile.Version = loader.ReadUInt32();
            resFile.SetVersionInfo(resFile.Version);
            resFile.ByteConverter = loader.ReadByteConverter();
            resFile.Alignment = (uint)loader.ReadByte();
            resFile.TargetAddressSize = (uint)loader.ReadByte(); //Thanks MasterF0X for pointing out the layout of the these
            uint OffsetToFileName = loader.ReadUInt32();
            resFile.Flag = loader.ReadUInt16();
            resFile.BlockOffset = loader.ReadUInt16();
            uint RelocationTableOffset = loader.ReadUInt32();
            uint sizFile = loader.ReadUInt32();
            ulong unk = loader.ReadUInt64();
            ulong stringPoolOffset = loader.ReadUInt64();
            ulong shaderModelOffset = loader.ReadUInt64();
            resFile.Name = loader.LoadString();
            resFile.Path = loader.LoadString();
            resFile.ShaderModels = loader.LoadDictValues<ShaderModel>();
            var userPointer = loader.ReadUInt32();
            var updateCallback = loader.ReadUInt32();
            var workMemPtr = loader.ReadUInt32();

            loader.ReadUInt32();
            loader.ReadUInt64();
            if (resFile.VersionMinor >= 7) //padding
            {
                loader.ReadUInt64();
            }

            ushort ModelCount = loader.ReadUInt16();
            ushort flag = loader.ReadUInt16();
            loader.ReadUInt16();
        }

        public static void Save(BfshaFileSwitchSaver saver, BfshaFile resFile)
        {
            saver.WriteSignature("FSHA");
            saver.Write(0x20202020);
            saver.Write(resFile.Version);
            saver.WriteByteConverter(resFile.ByteConverter);
            saver.Write((byte)resFile.Alignment);
            saver.Write((byte)resFile.TargetAddressSize);
        }
    }
}
