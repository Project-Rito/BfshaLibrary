using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Syroot.Maths;
using Syroot.BinaryData;
using Syroot.BinaryData.Core;

namespace BfshaLibrary.Core
{
    /// <summary>
    /// Loads the hierachy and data of a <see cref="Bfres.ResFile"/>.
    /// </summary>
    public class BfshaFileLoader : BinaryStream
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private IDictionary<uint, IResData> _dataMap;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileLoader"/> class loading data into the given
        /// <paramref name="resFile"/> from the specified <paramref name="stream"/> which is optionally left open.
        /// </summary>
        /// <param name="resFile">The <see cref="Bfres.ResFile"/> instance to load data into.</param>
        /// <param name="stream">The <see cref="Stream"/> to read data from.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
        internal BfshaFileLoader(BfshaFile resFile, Stream stream, bool leaveOpen = false)
            : base(stream, null, null, BooleanCoding.Byte, DateTimeCoding.NetTicks, StringCoding.ZeroTerminated, leaveOpen)
        {
            BfshaFile = resFile;
            _dataMap = new Dictionary<uint, IResData>();
        }

        internal BfshaFileLoader(IResData resData, BfshaFile resFile, Stream stream, bool leaveOpen = false)
    : base(stream, null, null, BooleanCoding.Byte, DateTimeCoding.NetTicks, StringCoding.ZeroTerminated, leaveOpen)
        {
            BfshaFile = resFile;
            _dataMap = new Dictionary<uint, IResData>();
            ImportableFile = resData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFileLoader"/> class from the file with the given
        /// <paramref name="fileName"/>.
        /// </summary>
        /// <param name="resFile">The <see cref="Bfres.ResFile"/> instance to load data into.</param>
        /// <param name="fileName">The name of the file to load the data from.</param>
        internal BfshaFileLoader(BfshaFile resFile, string fileName)
            : this(resFile, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        internal BfshaFileLoader(IResData resData, BfshaFile resFile, string fileName)
    : this(resData, resFile, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        internal bool IsSwitch { get; set; }

        /// <summary>
        /// Gets the loaded <see cref="Bfres.ResFile"/> instance.
        /// </summary>
        internal BfshaFile BfshaFile { get; }

        internal IResData ImportableFile { get; }

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        static internal void ImportSection(string fileName, IResData resData, BfshaFile resFile)
        {
            bool platformSwitch = false;
            using (var reader = new BinaryStream(File.OpenRead(fileName))) {
                reader.Seek(24, SeekOrigin.Begin);
                platformSwitch = reader.ReadUInt32() != 0;
            }

            if (platformSwitch)
            {
                using (var reader = new Switch.Core.BfshaFileSwitchLoader(resData, resFile, fileName)) {
                    reader.ImportSection();
                }
            }
            else
            {
                using (var reader = new WiiU.Core.BfshaFileWiiULoader(resData, resFile, fileName)) {
                    reader.ImportSection();
                }
            }
        }

        internal void ImportSection()
        {
            ReadImportedFileHeader();
            ((IResData)ImportableFile).Load(this);
        }

        private void ReadImportedFileHeader()
        {
            this.ByteConverter = ByteConverter.Big;

            Seek(8, SeekOrigin.Begin); //SUB MAGIC
            BfshaFile.Version = ReadUInt32();

            PushStringCoding(StringCoding.Raw);
            string sectionMagic = ReadString(8);
            PopStringCoding();
            uint offset = ReadUInt32();
            uint platformFlag = ReadUInt32();
            ReadUInt32();
            this.ByteConverter = ByteConverter.Big;

            if (platformFlag != 0)
            {
                Seek(0x30, SeekOrigin.Begin);
                this.ByteConverter = ByteConverter.Little;
            }
        }

        internal ByteConverter ReadByteConverter()
        {
            this.ByteConverter = ByteConverter.Big;
            var bom = ReadEnum<Syroot.BinaryData.Core.Endian>(true);
            this.ByteConverter = ByteConverter.GetConverter(bom);
            return this.ByteConverter;
        }

        /// <summary>
        /// Starts deserializing the data from the <see cref="ResFile"/> root.
        /// </summary>
         virtual internal void Execute()
        {
            ((IResData)BfshaFile).Load(this);
        }
        
        /// <summary>
        /// Reads and returns an <see cref="IResData"/> instance of type <typeparamref name="T"/> from the following
        /// offset or returns <c>null</c> if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> to read.</typeparam>
        /// <returns>The <see cref="IResData"/> instance or <c>null</c>.</returns>
        [DebuggerStepThrough]
        internal T Load<T>()
            where T : IResData, new()
        {
            uint offset = ReadOffset();
            if (offset == 0) return default(T);

            // Seek to the instance data and load it.
            using (TemporarySeek(offset, SeekOrigin.Begin))
            {
                return ReadResData<T>();
            }
        }

        /// <summary>
        /// Reads and returns an <see cref="IResData"/> instance of type <typeparamref name="T"/> from the following
        /// offset or returns <c>null</c> if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> to read.</typeparam>
        /// <returns>The <see cref="IResData"/> instance or <c>null</c>.</returns>
        [DebuggerStepThrough]
        internal T LoadSection<T>()
            where T : IResData, new()
        {
            T instance = new T();
            instance.Load(this);
            return instance;
        }

        /// <summary>
        /// Reads and returns an <see cref="IResData"/> instance of type <typeparamref name="T"/> from the following
        /// offset or returns <c>null</c> if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> to read.</typeparam>
        /// <returns>The <see cref="IResData"/> instance or <c>null</c>.</returns>
        internal T Load<T>(long offset)
    where T : IResData, new()
        {
            if (offset == 0) return default(T);

            // Seek to the instance data and load it.
            using (TemporarySeek(offset, SeekOrigin.Begin))
            {
                return ReadResData<T>();
            }
        }

        /// <summary>
        /// Reads and returns an instance of arbitrary type <typeparamref name="T"/> from the following offset with the
        /// given <paramref name="callback"/> or returns <c>null</c> if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the data to read.</typeparam>
        /// <param name="callback">The callback to read the instance data with.</param>
        /// <param name="offset">The optional offset to use instead of reading a following one.</param>
        /// <returns>The data instance or <c>null</c>.</returns>
        /// <remarks>Offset required for ExtFile header (offset specified before size).</remarks>
        [DebuggerStepThrough]
        public virtual T LoadCustom<T>(Func<T> callback, uint? offset = null)
        {
            offset = offset ?? ReadOffset();
            if (offset == 0) return default(T);

            using (TemporarySeek(offset.Value, SeekOrigin.Begin))
            {
                return callback.Invoke();
            }
        }

        /// <summary>
        /// Reads and returns an <see cref="ResDict{T}"/> instance with elements of type <typeparamref name="T"/> from
        /// the following offset or returns an empty instance if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> elements.</typeparam>
        /// <returns>The <see cref="ResDict{T}"/> instance.</returns>
        [DebuggerStepThrough]
        internal ResDict<T> LoadDict<T>()
            where T : IResData, new()
        {
            uint offset = ReadOffset();
            if (offset == 0) return new ResDict<T>();

            using (TemporarySeek(offset, SeekOrigin.Begin))
            {
                ResDict<T> dict = new ResDict<T>();
                ((IResData)dict).Load(this);
                return dict;
            }
        }

        /// <summary>
        /// Reads and returns an <see cref="IList{T}"/> instance with <paramref name="count"/> elements of type
        /// <typeparamref name="T"/> from the following offset or returns <c>null</c> if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> elements.</typeparam>
        /// <param name="count">The number of elements to expect for the list.</param>
        /// <param name="offset">The optional offset to use instead of reading a following one.</param>
        /// <returns>The <see cref="IList{T}"/> instance or <c>null</c>.</returns>
        /// <remarks>Offset required for FMDL FVTX lists (offset specified before count).</remarks>
        [DebuggerStepThrough]
        internal IList<T> LoadList<T>(int count, uint? offset = null)
            where T : IResData, new()
        {
            List<T> list = new List<T>(count);
            offset = offset ?? ReadOffset();
            if (offset == 0 || count == 0) return new List<T>();

            // Seek to the list start and read it.
            using (TemporarySeek(offset.Value, SeekOrigin.Begin))
            {
                for (; count > 0; count--)
                {
                    list.Add(ReadResData<T>());
                }
                return list;
            }
        }

        /// <summary>
        /// Reads and returns a <see cref="String"/> instance from the following offset or <c>null</c> if the read
        /// offset is 0.
        /// </summary>
        /// <param name="encoding">The optional encoding of the text.</param>
        /// <returns>The read text.</returns>
        [DebuggerStepThrough]
        internal string LoadString(Encoding encoding = null)
        {
            uint offset = ReadOffset();
            if (offset == 0) return null;

            Encoding = encoding ?? Encoding;
            using (TemporarySeek(offset, SeekOrigin.Begin))
            {
                return ReadString(Encoding);
            }
        }

        /// <summary>
        /// Reads and returns <paramref name="count"/> <see cref="String"/> instances from the following offsets.
        /// </summary>
        /// <param name="count">The number of instances to read.</param>
        /// <param name="encoding">The optional encoding of the texts.</param>
        /// <returns>The read texts.</returns>
        [DebuggerStepThrough]
        internal IList<string> LoadStrings(int count, Encoding encoding = null)
        {
            uint[] offsets = ReadOffsets(count);

            Encoding = encoding ?? Encoding;
            string[] names = new string[offsets.Length];
            using (TemporarySeek())
            {
                for (int i = 0; i < offsets.Length; i++)
                {
                    uint offset = offsets[i];
                    if (offset == 0) continue;

                    Position = offset;
                    names[i] = ReadString(Encoding);
                }
                return names;
            }
        }

        /// <summary>
        /// Reads and returns an <see cref="ResDict{T}"/> instance with elements of type <typeparamref name="T"/> from
        /// the following offset or returns an empty instance if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> elements.</typeparam>
        /// <returns>The <see cref="ResDict{T}"/> instance.</returns>
        [DebuggerStepThrough]
        internal virtual ResDict<T> LoadDictValues<T>()
            where T : IResData, new()
        {
            uint valuesOffset = ReadOffset();
            uint offset = ReadOffset();
            return LoadDictValues<T>(offset, valuesOffset);
        }

        /// <summary>
        /// Reads and returns an <see cref="ResDict{T}"/> instance with elements of type <typeparamref name="T"/> from
        /// the following offset or returns an empty instance if the read offset is 0.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IResData"/> elements.</typeparam>
        /// <returns>The <see cref="ResDict{T}"/> instance.</returns>
        [DebuggerStepThrough]
        internal virtual ResDict<T> LoadDictValues<T>(long dictionaryOffset, long valueOffset)
            where T : IResData, new()
        {
            if (dictionaryOffset == 0) return new ResDict<T>();

            using (TemporarySeek(dictionaryOffset, SeekOrigin.Begin))
            {
                ResDict<T> dict = new ResDict<T>();
                ((IResData)dict).Load(this);

                //Load data via list next to dictionary
                var keys = dict.Keys.ToList();
                var values = LoadList<T>(dict.Count, (uint)valueOffset);

                dict.Clear();
                for (int i = 0; i < keys.Count; i++)
                    dict.Add(keys[i], values[i]);
                return dict;
            }
        }

        public virtual uint ReadSize() => ReadUInt32();

        public virtual string ReadString(Encoding encoding = null) {
            PushStringCoding(StringCoding.ZeroTerminated);
            PushEncoding(encoding);
            string output = base.ReadString();
            PopEncoding();
            PopStringCoding();
            return output;
        }

        /// <summary>
        /// Reads a BFRES signature consisting of 4 ASCII characters encoded as an <see cref="UInt32"/> and checks for
        /// validity.
        /// </summary>
        /// <param name="validSignature">A valid signature.</param>
        internal void CheckSignature(string validSignature)
        {
            // Read the actual signature and compare it.
            PushEncoding(Encoding.ASCII);
            string signature = ReadString(sizeof(uint));
            PopEncoding();
            if (signature != validSignature)
            {
                 System.Console.WriteLine($"Invalid signature, expected '{validSignature}' but got '{signature}' at position {Position}.");

                // throw new ResException($"Invalid signature, expected '{validSignature}' but got '{signature}' at position {Position}.");
            }
        }

        /// <summary>
        /// Reads a BFRES offset which is relative to itself, and returns the absolute address.
        /// </summary>
        /// <returns>The absolute address of the offset.</returns>
        public virtual uint ReadOffset()
        {
            uint offset = ReadUInt32();
            return offset == 0 ? 0 : (uint)Position - sizeof(uint) + offset;
        }

        /// <summary>
        /// Reads BFRES offsets which are relative to themselves, and returns the absolute addresses.
        /// </summary>
        /// <param name="count">The number of offsets to read.</param>
        /// <returns>The absolute addresses of the offsets.</returns>
        internal uint[] ReadOffsets(int count)
        {
            uint[] values = new uint[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadOffset();
            }
            return values;
        }

        private Stack<StringCoding> _stringCodingStack = new Stack<StringCoding>();
        /// <summary>
        /// Pushes a string coding in a way that previous codings can be restored.
        /// </summary>
        /// <param name="coding">The coding to push.</param>
        public void PushStringCoding(StringCoding coding)
        {
            _stringCodingStack.Push(StringCoding);
            StringCoding = coding;
        }
        /// <summary>
        /// Pops a string coding.
        /// </summary>
        public void PopStringCoding()
        {
            StringCoding = _stringCodingStack.Pop();
        }

        private Stack<Encoding> _encodingStack = new Stack<Encoding>();
        /// <summary>
        /// Pushes an encoding in a way that previous encodings can be restored.
        /// </summary>
        /// <param name="coding">The coding to push.</param>
        public void PushEncoding(Encoding encoding)
        {
            _encodingStack.Push(Encoding);
            Encoding = encoding;
        }
        /// <summary>
        /// Pops an encoding.
        /// </summary>
        public void PopEncoding()
        {
            Encoding = _encodingStack.Pop();
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        [DebuggerStepThrough]
        private T ReadResData<T>()
            where T : IResData, new()
        {
            uint offset = (uint)Position;

            // Same data can be referenced multiple times. Load it in any case to move in the stream, needed for lists.
            T instance = new T();
            instance.Load(this);

            // If possible, return an instance already representing the data.
            if (_dataMap.TryGetValue(offset, out IResData existingInstance))
            {
                return (T)existingInstance;
            }
            else
            {
                _dataMap.Add(offset, instance);
                return instance;
            }
        }
    }
}
