using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BfshaLibrary.Core;
using System.Diagnostics;

namespace BfshaLibrary
{
    [DebuggerDisplay(nameof(ShaderInfoData))]
    public class ShaderReflection : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public ShaderReflectionData VertexShaderCode { get; set; }
        public ShaderReflectionData HullShaderCode { get; set; }
        public ShaderReflectionData DomainShaderCode { get; set; }
        public ShaderReflectionData GeometryShaderCode { get; set; }
        public ShaderReflectionData PixelShaderCode { get; set; }
        public ShaderReflectionData ComputeShaderCode { get; set; }

        public byte Type { get; set; }
        public ShaderFormat Format { get; set; }

        public uint BinaryFormat { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            VertexShaderCode = loader.Load<ShaderReflectionData>();
            HullShaderCode = loader.Load<ShaderReflectionData>();
            DomainShaderCode = loader.Load<ShaderReflectionData>();
            GeometryShaderCode = loader.Load<ShaderReflectionData>();
            PixelShaderCode = loader.Load<ShaderReflectionData>();
            ComputeShaderCode = loader.Load<ShaderReflectionData>();


            if (VertexShaderCode != null)
                VertexShaderCode.ShaderType = ShaderType.VERTEX;
            if (HullShaderCode != null)
                HullShaderCode.ShaderType = ShaderType.HULL;
            if (DomainShaderCode != null)
                DomainShaderCode.ShaderType = ShaderType.DOMAIN;
            if (GeometryShaderCode != null)
                GeometryShaderCode.ShaderType = ShaderType.GEOMETRY;
            if (PixelShaderCode != null)
                PixelShaderCode.ShaderType = ShaderType.PIXEL;
            if (ComputeShaderCode != null)
                ComputeShaderCode.ShaderType = ShaderType.COMPUTE;
        }

        internal long PtrVertexShaderCodePos;
        internal long PtrNullShaderCodePos;
        internal long PtrDomainShaderCodePos;
        internal long PtrGeometryShaderCodePos;
        internal long PtrPixelShaderCodePos;
        internal long PtrComputeShaderCodePos;

        void IResData.Save(BfshaFileSaver saver)
        {
            PtrVertexShaderCodePos = saver.SaveOffset();
            PtrNullShaderCodePos = saver.SaveOffset();
            PtrDomainShaderCodePos = saver.SaveOffset();
            PtrGeometryShaderCodePos = saver.SaveOffset();
            PtrPixelShaderCodePos = saver.SaveOffset();
            PtrComputeShaderCodePos = saver.SaveOffset();
        }

        public enum ShaderFormat : byte
        {
            Binary = 0x00,
            Source = 0x03,
        }
    }
}
