using System;
using System.Collections.Generic;
using System.Linq;
using BfshaLibrary.Switch.Core;

namespace BfshaLibrary.Switch
{
    public class ResProgramParser
    {
        public static void Load(BfshaFileSwitchLoader loader, ResShaderProgram shaderProgram)
        {
            long samplerTableOffset = loader.ReadOffset();
            if (loader.BfshaFile.VersionMajor2 >= 8)
                loader.ReadOffset();
            long uniformBlockOffset = loader.ReadOffset();
            if (loader.BfshaFile.VersionMajor2 >= 7)
                loader.ReadOffset();
            shaderProgram.variationOffset = loader.ReadOffset();
            long parentModelOffset = loader.ReadOffset();
            uint unk = loader.ReadUInt32();
            ushort unk2 = loader.ReadUInt16();
            ushort samplerCount = loader.ReadUInt16();
            ushort blockCount = 0;
            if (loader.BfshaFile.VersionMajor2 >= 8)
            {
                loader.ReadUInt16();
                blockCount = loader.ReadUInt16();
                loader.ReadUInt16();
                loader.Seek(2);
            }
            else
            {
                blockCount = loader.ReadUInt16();
                loader.Seek(6);
            }

            shaderProgram.SamplerLocations = loader.LoadList<LocationInfo>(samplerCount, (uint)samplerTableOffset).ToArray();
            shaderProgram.UniformBlockLocations = loader.LoadList<LocationInfo>(blockCount, (uint)uniformBlockOffset).ToArray();
        }
    }
}
