using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
// Very important point:
//Note that I made this tool by spending a lot of time and checking game files.
//There is nothing wrong with editing this tool for personal use,
//BUT DO NOT TRY TO COPY THE CODE AND SELL IT TO OTHERS!
//NoobInCoding :)
namespace The_Dark_Pictures
{
    public class TextTool
    {
        public struct Header
        {
            public byte[] UnknownFirst;
            public int FileSize;
            public byte[] UnknownSecond;
            public int TextCount;
        }
        public struct Texts
        {
            public int StartIDLen;
            public string StartID;
            public int TextByteSize;
            public int StrLen;
            public string Str;
            public int SpeakerByteSize;
            public int SpeakerLen;
            public string Speaker;
            public int EndIDByteSize;
            public int EndIDLen;
            public string EndID;
        }
        public struct UexpFile
        {
            public Header Header;
            public Texts[] Texts;
            public byte[] UnknownFirst;
            public byte[] FileFooter;
        }
        public static void Export(string Filename)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(Filename)))
            {
                UexpFile file = new UexpFile();
                file.Header.UnknownFirst = br.ReadBytes(57);
                file.Header.FileSize = br.ReadInt32();
                file.Header.UnknownSecond = br.ReadBytes(25);
                file.Header.TextCount = br.ReadInt32();
                file.Texts = new Texts[file.Header.TextCount];
                for (int i = 0; i < file.Header.TextCount; i++)
                {
                    file.Texts[i].StartIDLen = br.ReadInt32();
                    file.Texts[i].StartID = GetText(br, file.Texts[i].StartIDLen);
                    br.ReadBytes(16);
                    file.Texts[i].TextByteSize = br.ReadInt32();
                    br.ReadBytes(5);
                    file.Texts[i].StrLen = br.ReadInt32();
                    file.Texts[i].Str = GetText(br, file.Texts[i].StrLen);
                    br.ReadBytes(16);
                    file.Texts[i].SpeakerByteSize = br.ReadInt32();
                    br.ReadBytes(5);
                    file.Texts[i].SpeakerLen = br.ReadInt32();
                    file.Texts[i].Speaker = GetText(br, file.Texts[i].SpeakerLen);
                    br.ReadBytes(102);
                    file.Texts[i].EndIDByteSize = br.ReadInt32();
                    br.ReadBytes(5);
                    file.Texts[i].EndIDLen = br.ReadInt32();
                    file.Texts[i].EndID = GetText(br, file.Texts[i].EndIDLen);
                    br.ReadBytes(8);
                }
                br.ReadBytes(8);
                file.FileFooter = br.ReadBytes(8);
                string Strs = "";
                foreach (var Texts in file.Texts)
                {
                    if (Texts.StrLen != 0) Strs += Texts.Str + Environment.NewLine;
                }
                File.WriteAllText(Filename + ".txt", Strs);
            }
            
        }
        public static void Import(string TextFilename)
        {
            string[] TextFile = File.ReadAllLines(TextFilename);
            string uexpfilename = Path.GetDirectoryName(TextFilename) +"\\"+ Path.GetFileNameWithoutExtension(TextFilename);
            string uassetfilename = Path.GetDirectoryName(TextFilename) + "\\" + Path.GetFileNameWithoutExtension(uexpfilename)+".uasset";
            int l = 0;
            UexpFile file = new UexpFile();
            using (BinaryReader br = new BinaryReader(File.OpenRead(uexpfilename)))
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(uexpfilename + "_new")))
                {
                    file.Header.UnknownFirst = br.ReadBytes(57);
                    bw.Write(file.Header.UnknownFirst);
                    file.Header.FileSize = br.ReadInt32();
                    bw.Write(file.Header.FileSize);
                    file.Header.UnknownSecond = br.ReadBytes(25);
                    bw.Write(file.Header.UnknownSecond);
                    file.Header.TextCount = br.ReadInt32();
                    bw.Write(file.Header.TextCount);
                    file.Texts = new Texts[file.Header.TextCount];
                    long FileSize = bw.BaseStream.Position;
                    for (int i = 0; i < file.Header.TextCount; i++)
                    {
                        file.Texts[i].StartIDLen = br.ReadInt32();
                        file.Texts[i].StartID = GetText(br, file.Texts[i].StartIDLen);
                        bw.Write(GetBytes(file.Texts[i].StartID, Encoding.UTF8));
                        bw.Write(br.ReadBytes(16));
                        file.Texts[i].TextByteSize = br.ReadInt32();
                        br.ReadBytes(5);
                        file.Texts[i].StrLen = br.ReadInt32();
                        file.Texts[i].Str = GetText(br, file.Texts[i].StrLen);
                        if (file.Texts[i].StrLen != 0)
                        {
                            byte[] TextLine = GetBytes(TextFile[l], Encoding.Unicode);
                            bw.Write(TextLine.Length);
                            bw.Write(new byte[5]);
                            bw.Write(TextLine);
                            l++;
                        }
                        else
                        {
                            bw.Write(file.Texts[i].TextByteSize);
                            bw.Write(new byte[5]);
                            bw.Write(file.Texts[i].StrLen);
                        }
                        bw.Write(br.ReadBytes(16));
                        file.Texts[i].SpeakerByteSize = br.ReadInt32();
                        bw.Write(file.Texts[i].SpeakerByteSize);
                        bw.Write(br.ReadBytes(5));
                        file.Texts[i].SpeakerLen = br.ReadInt32();
                        file.Texts[i].Speaker = GetText(br, file.Texts[i].SpeakerLen);
                        bw.Write(GetBytes(file.Texts[i].Speaker, Encoding.UTF8));
                        bw.Write(br.ReadBytes(102));
                        file.Texts[i].EndIDByteSize = br.ReadInt32();
                        bw.Write(file.Texts[i].EndIDByteSize);
                        bw.Write(br.ReadBytes(5));
                        file.Texts[i].EndIDLen = br.ReadInt32();
                        file.Texts[i].EndID = GetText(br, file.Texts[i].EndIDLen);
                        bw.Write(GetBytes(file.Texts[i].EndID, Encoding.UTF8));
                        bw.Write(br.ReadBytes(8));
                    }
                    bw.Write(br.ReadBytes(8));
                    FileSize = bw.BaseStream.Position - FileSize;
                    file.FileFooter = br.ReadBytes(8);
                    bw.Write(file.FileFooter);
                    bw.BaseStream.Seek(57, SeekOrigin.Begin);
                    bw.Write(BitConverter.GetBytes((int)FileSize), 0, 4);
                    br.Close();
                    if (!File.Exists(uassetfilename)) { MessageBox.Show("Uasset File is missing! must be in :" + uassetfilename); Environment.Exit(0); }
                    File.WriteAllBytes(uassetfilename + "_new", File.ReadAllBytes(uassetfilename));
                    BinaryWriter AssetFile = new BinaryWriter(File.Open(uassetfilename + "_new", FileMode.Open));
                    AssetFile.BaseStream.Position = AssetFile.BaseStream.Length - 92;
                    byte[] fo = BitConverter.GetBytes(bw.BaseStream.Length - 4);
                    AssetFile.Write(fo, 0, fo.Length);
                    AssetFile.Close();
                }
            }
            
            
            
            
        }
        public static string GetText(BinaryReader br, int len)
        {

            string ret;
            if (len < 0)
            {
                len *= -2;
                ret = Encoding.Unicode.GetString(br.ReadBytes(len - 2)); // Get String Without a null at end
                br.ReadBytes(2);// Ignore Null
            }
            else if (len == 0)
            {
                br.ReadBytes(len);
                ret = "";
            }
            else
            {
                ret = Encoding.UTF8.GetString(br.ReadBytes(len - 1));// Get String Without a null at end
                br.ReadBytes(1);// Ignore Null
            }
            ret = ret.Replace("\r\n", "<cf>");
            ret = ret.Replace("\n", "<lf>");
            ret = ret.Replace("\r", "<cr>");
            return ret;
        }
        public static byte[] GetBytes(string Text, Encoding encoding)
        {
            Text = Text.Replace("<cf>", "\r\n");
            Text = Text.Replace("<lf>", "\n");
            Text = Text.Replace("<cr>", "\r");
            _ = Array.Empty<byte>();
            byte[] TextBytes;
            int Textlen;
            if (encoding == Encoding.Unicode)
            {
                TextBytes = CombBytes(encoding.GetBytes(Text), new byte[] { 0x00, 0x00 });
                Textlen = -Convert.ToInt32(TextBytes.Length / 2);
            }
            else
            {
                TextBytes = CombBytes(encoding.GetBytes(Text), new byte[] { 0x00 });
                Textlen = TextBytes.Length;
            }
            byte[] FinalBytes = CombBytes(BitConverter.GetBytes(Textlen), TextBytes);
            return FinalBytes;
        }
        public static byte[] CombBytes(byte[] bArray, byte[] newByte)
        {
            byte[] bytes = new byte[bArray.Length + newByte.Length];
            Buffer.BlockCopy(bArray, 0, bytes, 0, bArray.Length);
            Buffer.BlockCopy(newByte, 0, bytes, bArray.Length, newByte.Length);
            return bytes;
        }
    }
}
