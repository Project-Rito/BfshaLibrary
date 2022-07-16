using System.Linq;
using System.Diagnostics;
using BfshaLibrary.Core;

namespace BfshaLibrary
{
    [DebuggerDisplay(nameof(Attribute))]
    public class ResShaderProgram : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the location list of samplers for this program.
        /// </summary>
        public LocationInfo[] SamplerLocations { get; set; }

        /// <summary>
        /// Gets or sets the location list of uniforms for this program.
        /// </summary>
        public LocationInfo[] UniformBlockLocations { get; set; }

        internal uint UsedAttributeFlags;

        public bool HasAttribute(int index)
        {
            int value = (int)UsedAttributeFlags;
            for (int i = 0; i < 0x1F; i++)
            {
                bool set = (value & 0x1) != 0;
                if (index == i)
                    return set;

                value >>= 1;
            }
            return false;
        }

        internal long variationOffset;

        public WiiU.GX2VertexHeader GX2VertexData { get; set; }
        public WiiU.GX2PixelHeader GX2PixelData { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            if (loader.IsSwitch)
                Switch.ResProgramParser.Load((Switch.Core.BfshaFileSwitchLoader)loader, this);
            else
                WiiU.ResProgramParser.Load((WiiU.Core.BfshaFileWiiULoader)loader, this);
        }

        void IResData.Save(BfshaFileSaver saver)
        {
        }
    }
}