using System;
using System.Collections.Generic;
using System.Text;
using BfshaLibrary.Core;

namespace BfshaLibrary
{
    public struct LocationInfo : IResData
    {
        public sbyte VertexLocation;
        public sbyte GeoemetryLocation;
        public sbyte FragmentLocation;
        public sbyte ComputeLocation;

        void IResData.Load(BfshaFileLoader loader)
        {
            if (loader.IsSwitch)
            {
                VertexLocation = (sbyte)loader.ReadInt32();
                GeoemetryLocation = (sbyte)loader.ReadInt32();
                FragmentLocation = (sbyte)loader.ReadInt32();
                ComputeLocation = (sbyte)loader.ReadInt32();
                //2 extra stages
                if (loader.BfshaFile.VersionMajor2 >= 8)
                {
                    loader.ReadInt32();
                    loader.ReadInt32();
                }
            }
            else
            {
                VertexLocation = loader.ReadSByte();
                GeoemetryLocation = loader.ReadSByte();
                FragmentLocation = loader.ReadSByte();
                ComputeLocation = -1;
            }
        }

        void IResData.Save(BfshaFileSaver saver)
        {
            if (saver.IsSwitch)
            {
                saver.Write((int)VertexLocation);
                saver.Write((int)GeoemetryLocation);
                saver.Write((int)FragmentLocation);
                saver.Write((int)ComputeLocation);
                //2 extra stages
                if (saver.BfshaFile.VersionMajor2 >= 8)
                {
                    saver.Write(0);
                    saver.Write(0);
                }
            }
            else
            {
                saver.Write((sbyte)VertexLocation);
                saver.Write((sbyte)GeoemetryLocation);
                saver.Write((sbyte)FragmentLocation);
            }
        }
    }
}
