using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool
{
    class ToolClass
    {
        public ToolClass()
        {

        }
        public static int GetLittleEndian(byte[] data)
        {
            int length = data.Length;
            int result = 0;
            for (int i = length - 1; i >= 0; i--)
            {
                result |= data[i] << i * 8;
            }
            return result;
        }
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        public static string StringTrimRight(string str, int number)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str.TrimEnd(str[str.Length - number]);
            }
        }
        public static string ReadString(byte[] strbyte,bool trailingNull,System.Text.Encoding encoding)
        {
            if (trailingNull)
            {
                return StringTrimRight(encoding.GetString(strbyte),1);
            }
            else
            {
                return encoding.GetString(strbyte);
            }
            
        }
        public static string DeleteNull(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str.TrimEnd(str[str.Length - 1]);
            }
        }
        public static string StringClear(string str)
        {
            str = str.Replace("\r\n", "<cf>");
            str = str.Replace("\n", "<lf>");
            str = str.Replace("\r", "<cr>");
            return str;

        }
        public static string StringDeClear(string str)
        {
            str = str.Replace("<cf>","\r\n");
            str = str.Replace("<lf>", "\n");
            str = str.Replace("<cr>", "\r");
            return str;

        }
        public static byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static String ToBinary(Byte[] data)
        {
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }

        /// <summary>
        /// add a new byte to end or start of a byte array
        /// </summary>
        /// <param name="_input_bArray"></param>
        /// <param name="_newByte"></param>
        /// <param name="_add_to_start_of_array">if this parameter is True then the byte will be added to the beginning of array otherwise
        /// to the end of the array</param>
        /// <returns>result byte array</returns>
        public static byte[] AddByteToArray(byte[] _input_bArray, byte _newByte, Boolean _add_to_start_of_array)
        {
            byte[] newArray;
            if (_add_to_start_of_array)
            {
                newArray = new byte[_input_bArray.Length + 1];
                _input_bArray.CopyTo(newArray, 1);
                newArray[0] = _newByte;
            }
            else
            {
                newArray = new byte[_input_bArray.Length + 1];
                _input_bArray.CopyTo(newArray, 0);
                newArray[_input_bArray.Length] = _newByte;
            }
            return newArray;
        }
        public static byte[] AddBytesToArray(byte[] bArray, byte[] newByte)
        {
            byte[] bytes = new byte[bArray.Length + newByte.Length];
            Buffer.BlockCopy(bArray, 0, bytes, 0, bArray.Length);
            Buffer.BlockCopy(newByte, 0, bytes, bArray.Length, newByte.Length);
            return bytes;
        }
        public static byte[] ByteFromInt32(Int32 I32)

        {

            return BitConverter.GetBytes(I32);

        }
        public static void ReplaceData(string filename, int position, byte[] data)
        {
            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                stream.Position = position;
                stream.Write(data, 0, data.Length);
            }
        }
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
