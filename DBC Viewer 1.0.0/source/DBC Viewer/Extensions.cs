using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DBCViewer
{
    #region Coords3
    /// <summary>
    ///  Represents a coordinates of WoW object without orientation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct Coords3
    {
        public float X, Y, Z;

        /// <summary>
        ///  Converts the numeric values of this instance to its equivalent string representations, separator is space.
        /// </summary>
        public string GetCoords()
        {
            string coords = string.Empty;

            coords += X.ToString(CultureInfo.InvariantCulture);
            coords += " ";
            coords += Y.ToString(CultureInfo.InvariantCulture);
            coords += " ";
            coords += Z.ToString(CultureInfo.InvariantCulture);

            return coords;
        }
    }
    #endregion

    #region Coords4
    /// <summary>
    ///  Represents a coordinates of WoW object with specified orientation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct Coords4
    {
        public float X, Y, Z, O;

        /// <summary>
        ///  Converts the numeric values of this instance to its equivalent string representations, separator is space.
        /// </summary>
        public string GetCoordsAsString()
        {
            string coords = string.Empty;

            coords += X.ToString(CultureInfo.InvariantCulture);
            coords += " ";
            coords += Y.ToString(CultureInfo.InvariantCulture);
            coords += " ";
            coords += Z.ToString(CultureInfo.InvariantCulture);
            coords += " ";
            coords += O.ToString(CultureInfo.InvariantCulture);

            return coords;
        }
    }
    #endregion

    static class Extensions
    {
        public static BinaryReader FromFile(string fileName)
        {
            return new BinaryReader(new FileStream(fileName, FileMode.Open), Encoding.UTF8);
        }

        #region ReadPackedGuid
        /// <summary>
        ///  Reads the packed guid from the current stream and advances the current position of the stream by packed guid size.
        /// </summary>
        public static ulong ReadPackedGuid(this BinaryReader reader)
        {
            ulong res = 0;
            byte mask = reader.ReadByte();

            if (mask == 0)
                return res;

            int i = 0;

            while (i < 9)
            {
                if ((mask & 1 << i) != 0)
                    res += (ulong)reader.ReadByte() << (i * 8);
                i++;
            }
            return res;
        }
        #endregion

        #region ReadStringNumber
        /// <summary>
        ///  Reads the string with known length from the current stream and advances the current position of the stream by string length.
        /// <seealso cref="GenericReader.ReadStringNull"/>
        /// </summary>
        public static string ReadStringNumber(this BinaryReader reader)
        {
            string text = string.Empty;
            uint num = reader.ReadUInt32(); // string length

            for (uint i = 0; i < num; i++)
            {
                text += (char)reader.ReadByte();
            }
            return text;
        }
        #endregion

        #region ReadStringNull
        /// <summary>
        ///  Reads the NULL terminated string from the current stream and advances the current position of the stream by string length + 1.
        /// <seealso cref="GenericReader.ReadStringNumber"/>
        /// </summary>
        public static string ReadStringNull(this BinaryReader reader)
        {
            byte num;
            string text = string.Empty;
            System.Collections.Generic.List<byte> temp = new System.Collections.Generic.List<byte>();

            while ((num = reader.ReadByte()) != 0)
                temp.Add(num);

            text = Encoding.UTF8.GetString(temp.ToArray());

            return text;
        }
        #endregion

        #region ReadCoords3
        /// <summary>
        ///  Reads the object coordinates from the current stream and advances the current position of the stream by 12 bytes.
        /// </summary>
        public static Coords3 ReadCoords3(this BinaryReader reader)
        {
            Coords3 v;

            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();

            return v;
        }
        #endregion

        #region ReadCoords4
        /// <summary>
        ///  Reads the object coordinates and orientation from the current stream and advances the current position of the stream by 16 bytes.
        /// </summary>
        public static Coords4 ReadCoords4(this BinaryReader reader)
        {
            Coords4 v;

            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();
            v.O = reader.ReadSingle();

            return v;
        }
        #endregion

        #region ReadStruct
        /// <summary>
        /// Reads struct from the current stream and advances the current position if the stream by SizeOf(T) bytes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binReader"></param>
        /// <returns></returns>
        public static T ReadStruct<T>(this BinaryReader reader) where T : struct
        {
            byte[] rawData = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            T returnObject = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return returnObject;
        }
        #endregion

        #region ReadInt8
        /// <summary>
        ///  Reads the Int8 from the current stream and advances the current position of the stream by Int8 size.
        /// </summary>
        public static sbyte ReadInt8(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta != null && meta.Bits != 0x18)
                throw new Exception("TypeCode.SByte Unknown meta.Bits");
            return reader.ReadSByte();
            //if (meta == null)
            //    return reader.ReadSByte();
            //else
            //{
            //    byte[] b = reader.ReadBytes((32 - meta.Bits) >> 3);

            //    sbyte i8 = 0;
            //    for (int i = 0; i < b.Length; i++)
            //        i8 |= (sbyte)(b[i] << i * 8);

            //    return i8;
            //}
        }
        #endregion

        #region ReadUInt8
        /// <summary>
        ///  Reads the UInt8 from the current stream and advances the current position of the stream by UInt8 size.
        /// </summary>
        public static byte ReadUInt8(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta != null && meta.Bits != 0x18)
                throw new Exception("TypeCode.Byte Unknown meta.Bits");
            return reader.ReadByte();
            //if (meta == null)
            //    return reader.ReadByte();
            //else
            //{
            //    byte[] b = reader.ReadBytes((32 - meta.Bits) >> 3);

            //    byte u8 = 0;
            //    for (int i = 0; i < b.Length; i++)
            //        u8 |= (byte)(b[i] << i * 8);

            //    return u8;
            //}
        }
        #endregion

        #region ReadInt16
        /// <summary>
        ///  Reads the Int16 from the current stream and advances the current position of the stream by Int16 size.
        /// </summary>
        public static short ReadInt16(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta != null && meta.Bits != 0x10)
                throw new Exception("TypeCode.Int16 Unknown meta.Bits");
            return reader.ReadInt16();
            //if (meta == null)
            //    return reader.ReadInt16();
            //else
            //{
            //    byte[] b = reader.ReadBytes((32 - meta.Bits) >> 3);

            //    short i16 = 0;
            //    for (int i = 0; i < b.Length; i++)
            //        i16 |= (short)(b[i] << i * 8);

            //    return i16;
            //}
        }
        #endregion

        #region ReadUInt16
        /// <summary>
        ///  Reads the UInt16 from the current stream and advances the current position of the stream by UInt16 size.
        /// </summary>
        public static ushort ReadUInt16(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta != null && meta.Bits != 0x10)
                throw new Exception("TypeCode.UInt16 Unknown meta.Bits");
            return reader.ReadUInt16();
            //if (meta == null)
            //    return reader.ReadUInt16();
            //else
            //{
            //    byte[] b = reader.ReadBytes((32 - meta.Bits) >> 3);

            //    ushort u16 = 0;
            //    for (int i = 0; i < b.Length; i++)
            //        u16 |= (ushort)(b[i] << i * 8);

            //    return u16;
            //}
        }
        #endregion

        #region ReadInt32
        /// <summary>
        ///  Reads the Int32 from the current stream and advances the current position of the stream by Int32 size.
        /// </summary>
        public static int ReadInt32(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta == null)
                return reader.ReadInt32();
            else
            {
                byte[] b = reader.ReadBytes((32 - meta.Bits) >> 3);

                int i32 = 0;
                for (int i = 0; i < b.Length; i++)
                    i32 |= (b[i] << i * 8);

                return i32;
            }
        }
        #endregion

        #region ReadUInt32
        /// <summary>
        ///  Reads the UInt32 from the current stream and advances the current position of the stream by UInt32 size.
        /// </summary>
        public static uint ReadUInt32(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta == null)
                return reader.ReadUInt32();
            else
            {
                byte[] b = reader.ReadBytes((32 - meta.Bits) >> 3);

                uint u32 = 0;
                for (int i = 0; i < b.Length; i++)
                    u32 |= ((uint)b[i] << i * 8);

                return u32;
            }
        }
        #endregion

        #region ReadInt64
        /// <summary>
        ///  Reads the Int64 from the current stream and advances the current position of the stream by Int64 size.
        /// </summary>
        public static long ReadInt64(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta == null)
                return reader.ReadInt64();
            else
            {
                byte[] b = reader.ReadBytes((32 - meta.Bits) >> 3);

                long i64 = 0;
                for (int i = 0; i < b.Length; i++)
                    i64 |= ((long)b[i] << i * 8);

                return i64;
            }
        }
        #endregion

        #region ReadUInt64
        /// <summary>
        ///  Reads the UInt64 from the current stream and advances the current position of the stream by UInt64 size.
        /// </summary>
        public static ulong ReadUInt64(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta == null)
                return reader.ReadUInt64();
            else
            {
                byte[] b = reader.ReadBytes((32 - meta.Bits) >> 3);

                ulong u64 = 0;
                for (int i = 0; i < b.Length; i++)
                    u64 |= ((ulong)b[i] << i * 8);

                return u64;
            }
        }
        #endregion

        #region ReadSingle
        /// <summary>
        ///  Reads the Single from the current stream and advances the current position of the stream by Single size.
        /// </summary>
        public static float ReadSingle(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta != null && meta.Bits != 0x00)
                throw new Exception("TypeCode.Single Unknown meta.Bits");
            return reader.ReadSingle();
        }
        #endregion

        #region ReadDouble
        /// <summary>
        ///  Reads the Double from the current stream and advances the current position of the stream by Double size.
        /// </summary>
        public static double ReadDouble(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta != null && meta.Bits != -32)
                throw new Exception("TypeCode.Double Unknown meta.Bits");
            return reader.ReadDouble();
        }
        #endregion

        #region ReadString
        /// <summary>
        ///  Reads the String from the current stream and advances the current position of the stream by String size.
        /// </summary>
        public static string ReadString(this BinaryReader reader, ColumnMeta meta)
        {
            if (meta != null && meta.Bits != 0x00)
                throw new Exception("TypeCode.String Unknown meta.Bits");
            return reader.ReadStringNull();
        }
        #endregion

        public static void AppendFormatLine(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendFormat(format, args);
            sb.AppendLine();
        }

        public static void AppendFormatLine(this StringBuilder sb, IFormatProvider provider, string format, params object[] args)
        {
            sb.AppendFormat(provider, format, args);
            sb.AppendLine();
        }
    }
}
