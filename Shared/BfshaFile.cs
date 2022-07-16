using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Syroot.BinaryData;
using BfshaLibrary.Core;
using System.ComponentModel;

namespace BfshaLibrary
{
    /// <summary>
    /// Represents a NintendoWare for Cafe (NW4F) graphics shader archive file.
    /// </summary>
    [DebuggerDisplay(nameof(BfshaFile) + " {" + nameof(Name) + "}")]
    public class BfshaFile : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSHA";

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BfshaFile"/> class.
        /// </summary>
        public BfshaFile()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BfshaFile"/> class from the given <paramref name="stream"/> which
        /// is optionally left open.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
        public BfshaFile(Stream stream)
        {
            if (IsSwitchBinary(stream))
            {
                using (BfshaFileLoader loader = new Switch.Core.BfshaFileSwitchLoader(this, stream, true)) {
                    loader.Execute();
                }
            }
            else
            {
                using (BfshaFileLoader loader = new WiiU.Core.BfshaFileWiiULoader(this, stream, true)) {
                    loader.Execute();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BfshaFile"/> class from the file with the given
        /// <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public BfshaFile(string fileName)
        {
            if (IsSwitchBinary(fileName)) {
                using (BfshaFileLoader loader = new Switch.Core.BfshaFileSwitchLoader(this, fileName)) {
                    loader.Execute();
                }
            }
            else
            {
                using (BfshaFileLoader loader = new WiiU.Core.BfshaFileWiiULoader(this, fileName)) {
                    loader.Execute();
                }
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public bool IsPlatformSwitch { get; set; }

        /// <summary>
        /// Gets or sets the alignment to use for raw data blocks in the file.
        /// </summary>
        [Browsable(true)]
        [Category("Binary Info")]
        [DisplayName("Alignment")]
        public uint Alignment { get; set; } = 0xC;

        public int DataAlignment
        {
            get
            {
                if (IsPlatformSwitch)
                    return (1 << (int)Alignment);
                else
                    return (int)Alignment;
            }
            set
            {
                if (IsPlatformSwitch)
                    Alignment = (uint)(value >> 7);
                else
                    Alignment = (uint)value;
            }
        }

        /// <summary>
        /// Gets or sets the target adress size to use for raw data blocks in the file.
        /// </summary>
        [Browsable(false)]
        public uint TargetAddressSize { get; set; }

        /// <summary>
        /// Gets or sets a name describing the contents.
        /// </summary>
        [Browsable(true)]
        [Category("Binary Info")]
        [DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the revision of the BFRES structure formats.
        /// </summary>
        internal uint Version { get; set; }

        /// <summary>
        /// Gets or sets the flag. Unknown purpose.
        /// </summary>
        internal uint Flag { get; set; }

        /// <summary>
        /// Gets or sets the BlockOffset. 
        /// </summary>
        internal uint BlockOffset { get; set; }

        /// <summary>
        /// Gets or sets the major revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Version")]
        [DisplayName("Version Major")]
        public string VersioFull
        {
            get
            {
                return $"{VersionMajor},{VersionMajor2},{VersionMinor},{VersionMinor2}";
            }
        }

        /// <summary>
        /// Gets or sets the major revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [Category("Version")]
        [DisplayName("Version Major")]
        public uint VersionMajor { get; set; }
        /// <summary>
        /// Gets or sets the second major revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [Category("Version")]
        [DisplayName("Version Major 2")]
        public uint VersionMajor2 { get; set; }
        /// <summary>
        /// Gets or sets the minor revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [Category("Version")]
        [DisplayName("Version Minor")]
        public uint VersionMinor { get; set; }
        /// <summary>
        /// Gets or sets the second minor revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [Category("Version")]
        [DisplayName("Version Minor 2")]
        public uint VersionMinor2 { get; set; }

        /// <summary>
        /// Gets the byte order in which data is stored. Must be the endianness of the target platform.
        /// </summary>
        public ByteOrder ByteOrder { get; internal set; }

        /// <summary>
        /// Gets or sets the stored <see cref="RelocationTable"/> (_RLT) instance.
        /// </summary>
        public RelocationTable RelocationTable { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="ShaderModel"/> instances.
        /// </summary>
        public ResDict<ShaderModel> ShaderModels { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Saves the contents in the given <paramref name="stream"/> and optionally leaves it open
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the contents into.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
        public void Save(Stream stream, bool leaveOpen = false)
        {
            using (BfshaFileSaver saver = new BfshaFileSaver(this, stream, leaveOpen))
            {
                saver.Execute();
            }
        }

        /// <summary>
        /// Saves the contents in the file with the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to save the contents into.</param>
        public void Save(string fileName)
        {
            using (BfshaFileSaver saver = new BfshaFileSaver(this, fileName))
            {
                saver.Execute();
            }
        }

        // ---- METHODS (INTERNAL) ---------------------------------------------------------------------------------------

        internal void SetVersionInfo(uint Version)
        {
            VersionMajor = Version >> 24;
            VersionMajor2 = Version >> 16 & 0xFF;
            VersionMinor = Version >> 8 & 0xFF;
            VersionMinor2 = Version & 0xFF;
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            IsPlatformSwitch = loader.IsSwitch;
            if (loader.IsSwitch)
                Switch.BfshaFileParser.Load((Switch.Core.BfshaFileSwitchLoader)loader, this);
            else
                WiiU.BfshaFileParser.Load((WiiU.Core.BfshaFileWiiULoader)loader, this);
        }

        void IResData.Save(BfshaFileSaver saver)
        {
 
        }

        public static bool IsSwitchBinary(string fileName) {
            return IsSwitchBinary(File.OpenRead(fileName));
        }

        public static bool IsSwitchBinary(Stream stream)
        {
            using (var reader = new BinaryDataReader(stream, true))
            {
                reader.ByteOrder = ByteOrder.LittleEndian;

                reader.Seek(4, SeekOrigin.Begin);
                uint paddingCheck = reader.ReadUInt32();
                reader.Position = 0;

                return paddingCheck == 0x20202020;
            }
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void PreSave()
        {
            // Update Shape instances.
       
        }
    }
}
