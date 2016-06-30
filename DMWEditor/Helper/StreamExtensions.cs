using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DMWEditor
{
    public static class StreamExtensions
    {
        private static byte[] ReadIntoBuffer(Stream ms, int length)
        {
            if (length < 0)
                return null;

            byte[] buf = new byte[length];
            ms.Read(buf, 0, buf.Length);
            return buf;
        }

        public static void WriteString(this Stream ms, string str)
        {
            byte[] strBuf = Encoding.UTF8.GetBytes(str);
            ms.WriteInt16((short)strBuf.Length);
            ms.Write(strBuf, 0, strBuf.Length);
        }

        public static string ReadString(this Stream ms)
        {
            short strLen = ms.ReadInt16();
            if (strLen == 0)
                return null;

            return Encoding.UTF8.GetString(ReadIntoBuffer(ms, strLen));
        }

        public static void WriteInt16(this Stream ms, short val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            ms.Write(buf, 0, buf.Length);
        }

        public static short ReadInt16(this Stream ms)
        {
            return BitConverter.ToInt16(ReadIntoBuffer(ms, 2), 0);
        }

        public static void WriteInt32(this Stream ms, int val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            ms.Write(buf, 0, buf.Length);
        }

        public static int ReadInt32(this Stream ms)
        {
            return BitConverter.ToInt32(ReadIntoBuffer(ms, 4), 0);
        }

        public static void WriteFloat(this Stream ms, float val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            ms.Write(buf, 0, buf.Length);
        }

        public static float ReadFloat(this Stream ms)
        {
            return BitConverter.ToSingle(ReadIntoBuffer(ms, 4), 0);
        }

        public static void WriteDouble(this Stream ms, double val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            ms.Write(buf, 0, buf.Length);
        }

        public static double ReadDouble(this Stream ms)
        {
            return BitConverter.ToDouble(ReadIntoBuffer(ms, 4), 0);
        }
    }
}