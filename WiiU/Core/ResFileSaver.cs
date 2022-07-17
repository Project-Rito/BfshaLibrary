using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Syroot.BinaryData;
using Syroot.Maths;
using BfshaLibrary.Core;

namespace BfshaLibrary.WiiU.Core
{
    /// <summary>
    /// Saves the hierachy and data of a <see cref="Bfres.ResFile"/>.
    /// </summary>
    public class BfshaFileWiiUSaver : BfshaFileSaver
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets a data block alignment typically seen with <see cref="Buffer.Data"/>.
        /// </summary>
        internal const uint AlignmentSmall = 0x40;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private uint _ofsFileSize;
        private uint _ofsStringPool;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileSaver"/> class saving data from the given
        /// <paramref name="resFile"/> into the specified <paramref name="stream"/> which is optionally left open.
        /// </summary>
        /// <param name="resFile">The <see cref="Bfres.ResFile"/> instance to save data from.</param>
        /// <param name="stream">The <see cref="Stream"/> to save data into.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
        internal BfshaFileWiiUSaver(BfshaFile resFile, Stream stream, bool leaveOpen)
    : base(resFile, stream, leaveOpen)
        {
            ByteConverter = ByteConverter.Big;
        }

        internal BfshaFileWiiUSaver(IResData resData, BfshaFile resFile, Stream stream, bool leaveOpen)
    : base(resData, resFile, stream, leaveOpen)
        {
            ByteConverter = ByteConverter.Big;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileSaver"/> class for the file with the given
        /// <paramref name="fileName"/>.
        /// </summary>
        /// <param name="resFile">The <see cref="Bfres.ResFile"/> instance to save.</param>
        /// <param name="fileName">The name of the file to save the data into.</param>
        internal BfshaFileWiiUSaver(BfshaFile resFile, string fileName)
    : base(resFile, fileName)
        {
        }

        internal BfshaFileWiiUSaver(IResData resData, BfshaFile resFile, string fileName)
    : base(resData, resFile, fileName)
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        // ---- METHODS (PRIVAYE) -------------------------------------------------------------------------------------

        /// <summary>
        /// Starts serializing the data from the <see cref="ResFile"/> root.
        /// </summary>
        public override void Execute()
        {

        }

        public override void ExportSection()
        {

        }
    }
}
