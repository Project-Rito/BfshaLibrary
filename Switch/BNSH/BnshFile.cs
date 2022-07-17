﻿using System.Diagnostics;
using System.Linq;
using System.IO;
using Syroot.BinaryData;
using BfshaLibrary.Core;
using System.ComponentModel;
using Syroot.BinaryData.Core;

namespace BfshaLibrary
{
    /// <summary>
    /// Represents a NintendoWare for Cafe (NW4F) shader file.
    /// </summary>
    [DebuggerDisplay(nameof(BnshFile) + " {" + nameof(Name) + "}")]
    public class BnshFile : IResData
    {
        internal ulong SizeOfGRSC => 96;

        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "BNSH";
        private const string _grscSignature = "grsc";

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BfshaFile"/> class.
        /// </summary>
        public BnshFile()
        {
            Name = "dummy.bnsh";
            VersionMajor = 0;
            VersionMajor2 = 2;
            VersionMinor = 1;
            VersionMinor2 = 11;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BfshaFile"/> class from the given <paramref name="stream"/> which
        /// is optionally left open.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
        public BnshFile(Stream stream, bool leaveOpen = false)
        {
            using (BfshaFileLoader loader = new Switch.Core.BfshaFileSwitchLoader(this, stream, leaveOpen))
            {
                loader.Execute();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BfshaFile"/> class from the file with the given
        /// <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public BnshFile(string fileName)
        {
            using (BfshaFileLoader loader = new Switch.Core.BfshaFileSwitchLoader(this, fileName))
            {
                loader.Execute();
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the major revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Version")]
        [DisplayName("Version Full")]
        public string VersionFull
        {
            get
            {
                return $"{VersionMajor},{VersionMajor2},{VersionMinor},{VersionMinor2}";
            }
        }

        internal uint Version { get; set; }

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
        public Syroot.BinaryData.ByteConverter ByteConverter { get; private set; }


        /// <summary>
        /// Gets or sets the alignment to use for raw data blocks in the file.
        /// </summary>
        public uint Alignment { get; set; }

        /// <summary>
        /// Gets or sets the data alignment to use for raw data blocks in the file.
        /// </summary>
        public uint DataAlignment
        {
            get
            {
                return (uint)(1 << (int)Alignment);
            }
        }


        /// <summary>
        /// Gets or sets the target adress size to use for raw data blocks in the file.
        /// </summary>
        public uint TargetAddressSize { get; set; }

        /// <summary>
        /// Gets or sets the flag. Unknown purpose.
        /// </summary>
        public uint Flag { get; set; }

        /// <summary>
        /// Gets or sets the BlockOffset. 
        /// </summary>
        public uint BlockOffset { get; set; }

        /// <summary>
        /// Gets or sets a name describing the contents.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="RelocationTable"/> (_RLT) instance.
        /// </summary>
        public RelocationTable RelocationTable { get; set; }


        /// <summary>
        /// Saves the contents in the given <paramref name="stream"/> and optionally leaves it open
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the contents into.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
        public void Save(Stream stream, bool leaveOpen = false)
        {
            using (BfshaFileSaver saver = new Switch.Core.BfshaFileSwitchSaver(this, stream, leaveOpen))
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
            using (BfshaFileSaver saver = new Switch.Core.BfshaFileSwitchSaver(this, fileName))
            {
                saver.Execute();
            }
        }

        public ShaderVariation[] ShaderVariations { get; set; }

        public StringTable StringTable;

        public ushort ApiType { get; set; }
        public ushort ApiVersion { get; set; }
        public byte TargetCode { get; set; }

        public uint CompilerVersion { get; set; }

        public ulong LlCompileVersion { get; set; }

        public long VariationStartOffset { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        public static ShaderVariation GetShaderVariation(Stream stream, int index)
        {
            using (var reader = new Switch.Core.BfshaFileSwitchLoader(new BnshFile(), stream, true)) {
                return reader.Load<ShaderVariation>(192 + (index * 64));
            }
        }

        internal uint SaveVersion()
        {
            return VersionMajor << 24 | VersionMajor2 << 16 | VersionMinor << 8 | VersionMinor2;
        }

        internal void SetVersionInfo(uint Version)
        {
            VersionMajor = Version >> 24;
            VersionMajor2 = Version >> 16 & 0xFF;
            VersionMinor = Version >> 8 & 0xFF;
            VersionMinor2 = Version & 0xFF;
        }


        void IResData.Load(BfshaFileLoader loader)
        {
            StringTable = new StringTable();

            loader.CheckSignature(_signature);
            uint padding = loader.ReadUInt32();
            SetVersionInfo(loader.ReadUInt32());
            ByteConverter = loader.ReadEnum<Endian>(false) == Endian.Little ? Syroot.BinaryData.ByteConverter.Little : Syroot.BinaryData.ByteConverter.Big;
            Alignment = (uint)loader.ReadByte();
            TargetAddressSize = (uint)loader.ReadByte(); //Thanks MasterF0X for pointing out the layout of the these
            uint OffsetToFileName = loader.ReadUInt32();
            ushort flag = loader.ReadUInt16();
            ushort blockOffset = loader.ReadUInt16();
            uint RelocationTableOffset = loader.ReadUInt32();
            uint sizFile = loader.ReadUInt32();
            loader.Seek(64); //Padding

            if (OffsetToFileName != 0)
            {
                using (loader.TemporarySeek(OffsetToFileName, SeekOrigin.Begin))
                {
                    loader.PushStringCoding(StringCoding.ZeroTerminated);
                    Name = loader.ReadString();
                    loader.PopStringCoding();
                }
            }

            //GRSC Section
            loader.CheckSignature(_grscSignature);
            loader.Seek(12); //Block header
            ApiType = loader.ReadUInt16();
            ApiVersion = loader.ReadUInt16();
            TargetCode = (byte)loader.ReadByte();
            loader.Seek(3); //reserved
            CompilerVersion = loader.ReadUInt32();
            uint VariationCount = loader.ReadUInt32();
            VariationStartOffset = loader.ReadInt64();
            loader.ReadInt64();
            LlCompileVersion = loader.ReadUInt64();

            ShaderVariations = loader.LoadList<ShaderVariation>((int)VariationCount, (uint)VariationStartOffset).ToArray();
        }

        internal long shaderVariationArrayOffset;

        void IResData.Save(BfshaFileSaver saver)
        {
            var saverNX = saver as Switch.Core.BfshaFileSwitchSaver;

            Version = SaveVersion();
            saver.WriteSignature(_signature);
            saver.Write(0x20202020);
            saver.Write(Version);
            saver.ByteConverter = ByteConverter;
            saver.Write(true);
            saver.Write((byte)Alignment);
            saver.Write((byte)TargetAddressSize);
        //    saverNX.SaveFileNameString(Name);
            saver.Write((ushort)Flag);
           // saverNX.SaveHeaderBlock(true);
           // saverNX.SaveRelocationTablePointerPointer();
           // saverNX.SaveFieldFileSize();
            saver.Write(new byte[64]);

            //GRSC
            saver.WriteSignature(_grscSignature);
           // saverNX.SaveHeaderBlock();
            saver.Write(ApiType);
            saver.Write(ApiVersion);
            saver.Write(TargetCode);
            saver.Seek(3);
            saver.Write(CompilerVersion);
            saver.Write(ShaderVariations.Length);
            shaderVariationArrayOffset = saver.SaveOffset();
            saver.Write(0L);
            saver.Write(LlCompileVersion);
            saver.Write(new byte[40]); //reserved
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void PreSave()
        {
            // Update Shape instances.

        }
    }
}
