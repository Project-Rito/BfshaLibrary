using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Maths;
using BfshaLibrary.Core;

namespace BfshaLibrary.WiiU.Core
{
    /// <summary>
    /// Loads the hierachy and data of a <see cref="Bfres.ResFile"/>.
    /// </summary>
    public class BfshaFileWiiULoader : BfshaFileLoader
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------



        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileLoader"/> class loading data into the given
        /// <paramref name="resFile"/> from the specified <paramref name="stream"/> which is optionally left open.
        /// </summary>
        /// <param name="resFile">The <see cref="Bfres.ResFile"/> instance to load data into.</param>
        /// <param name="stream">The <see cref="Stream"/> to read data from.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
        internal BfshaFileWiiULoader(BfshaFile resFile, Stream stream, bool leaveOpen = false)
            : base(resFile, stream, leaveOpen)
        {
            ByteConverter = ByteConverter.Big;
        }

        internal BfshaFileWiiULoader(IResData resData, BfshaFile resFile, Stream stream, bool leaveOpen = false)
    : base(resData, resFile, stream, leaveOpen)
        {
            ByteConverter = ByteConverter.Big;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileLoader"/> class from the file with the given
        /// <paramref name="fileName"/>.
        /// </summary>
        /// <param name="resFile">The <see cref="Bfres.ResFile"/> instance to load data into.</param>
        /// <param name="fileName">The name of the file to load the data from.</param>
        internal BfshaFileWiiULoader(BfshaFile resFile, string fileName)
            : base(resFile, fileName)
        {
            ByteConverter = ByteConverter.Big;
        }

        internal BfshaFileWiiULoader(IResData resData, BfshaFile resFile, string fileName)
            : base(resData, resFile, fileName)
        {
            ByteConverter = ByteConverter.Big;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Reads and returns an <see cref="ResDict{T}"/> instance with elements of type <typeparamref name="T"/> from
        /// the following offset or returns an empty instance if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> elements.</typeparam>
        /// <returns>The <see cref="ResDict{T}"/> instance.</returns>
        [DebuggerStepThrough]
        internal override ResDict<T> LoadDictValues<T>(long dictionaryOffset, long valueOffset)
        {
            if (dictionaryOffset == 0) return new ResDict<T>();

            using (TemporarySeek(dictionaryOffset, SeekOrigin.Begin))
            {
                ResDict<T> dict = new ResDict<T>();
                ((IResData)dict).Load(this);
                return dict;
            }
        }
    }
}
