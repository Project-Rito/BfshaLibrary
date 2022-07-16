using System.Diagnostics;
using System.Text;
using Syroot.BinaryData;
using BfshaLibrary.Core;

namespace BfshaLibrary
{
    /// <summary>
    /// Represents a <see cref="String"/> which is stored in a <see cref="BfshaFile"/>.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ResUint32) + "}")]
    public class ResUint32 : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// The textual <see cref="System.UInt32"/> represented by this instance.
        /// </summary>
        public uint Value
        {
            get; set;
        }

        /// <summary>
        /// The <see cref="Encoding"/> with which this string was read or will be written.
        /// </summary>
        public Encoding Encoding
        {
            get; set;
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Converts the given <paramref name="value"/> value to a <see cref="ResString"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to represent in the new <see cref="ResString"/> instance.
        /// </param>
        public static implicit operator ResUint32(uint value)
        {
            return new ResUint32() { Value = value };
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> value to an <see cref="System.UInt32"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="ResUint32"/> value to represent in the new <see cref="System.UInt32"/> instance.
        /// </param>
        public static implicit operator uint(ResUint32 value)
        {
            return value.Value;
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Returns the value of the <see cref="String"/> property.
        /// </summary>
        /// <returns>The value of the <see cref="String"/> property.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(BfshaFileLoader loader)
        {
            Value = loader.ReadUInt32();
        }

        void IResData.Save(BfshaFileSaver saver)
        {
            saver.Write(Value);
        }
    }
}
