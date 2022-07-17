using System;
using System.Collections.Generic;
using System.Linq;
using BfshaLibrary.WiiU.Core;

namespace BfshaLibrary.WiiU
{
    public class ResProgramParser
    {
        public static void Load(BfshaFileWiiULoader loader, ResShaderProgram shaderProgram)
        {
            if (loader.BfshaFile.VersionMajor >= 4)
            {

            }
            else
            {
                loader.ReadUInt16();
                byte samplerCount = (byte)loader.ReadByte();
                byte blockCount = (byte)loader.ReadByte();
                shaderProgram.UsedAttributeFlags = loader.ReadUInt32();
                loader.Seek(24);
                shaderProgram.SamplerLocations = loader.LoadList<LocationInfo>(samplerCount).ToArray();
                shaderProgram.UniformBlockLocations = loader.LoadList<LocationInfo>(blockCount).ToArray();
                shaderProgram.GX2VertexData = loader.Load<GX2VertexHeader>();
                uint gx2geometryShaderOffset = loader.ReadOffset();
                shaderProgram.GX2PixelData = loader.Load<GX2PixelHeader>();
                uint parentModelOffset = loader.ReadOffset();
            }
        }
    }
}
