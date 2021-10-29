using MyTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace The_Dark_Pictures
{
    class ToolB
    {
        public ToolB()
        {

        }
        public static void Export(int Ver, string UexpFileName, string TXTFileName)
        {
            BinaryReader bf = new BinaryReader(File.Open(UexpFileName, FileMode.Open));
            string Texts = "";
            bf.BaseStream.Position = 90;
            for (int i = 0; i < 25000; i++)
            {
                try
                {

                    bf.ReadBytes(bf.ReadInt32());
                    string d = BitConverter.ToString(bf.ReadBytes(1));
                    if (d == "12")
                    {
                        bf.ReadBytes(7);
                        bf.ReadBytes(bf.ReadInt32());
                        bf.ReadBytes(25);
                    }
                    else
                    {
                        bf.ReadBytes(24);
                    }
                    int StringSize = ToolClass.GetLittleEndian(bf.ReadBytes(4));
                    string Str;
                    if (StringSize < 0 || StringSize > 0)
                    {
                        if (StringSize < 0)
                        {
                            StringSize = -StringSize * 2;
                            Str = ToolClass.ReadString(bf.ReadBytes(StringSize), true, Encoding.Unicode);
                        }
                        else
                        {
                            Str = ToolClass.ReadString(bf.ReadBytes(StringSize), true, Encoding.ASCII);
                        }
                        Str = Str.Replace("\r\n", "<cf>");
                        Str = Str.Replace("\n", "<lf>");
                        Str = Str.Replace("\r", "<cr>");
                        Texts += Str + Environment.NewLine;
                    }
                    bf.ReadBytes(16);
                    string f = BitConverter.ToString(bf.ReadBytes(4)).Replace("-", "");
                    if (f == "04000000")
                    {
                        if (Ver == 2) { bf.ReadBytes(132); }
                        if (Ver == 3) { bf.ReadBytes(120); }

                    }
                    else
                    {
                        bf.ReadBytes(5);
                        bf.ReadBytes(bf.ReadInt32());
                        if (Ver == 2) { bf.ReadBytes(123); }
                        if (Ver == 3) { bf.ReadBytes(118); }
                    }
                }
                catch (Exception)
                {
                    break;
                }


            }
            File.WriteAllText(TXTFileName, Texts);
            bf.BaseStream.Close();
            MessageBox.Show("Export Done!" + Environment.NewLine + "Saved as " + Path.GetFileNameWithoutExtension(TXTFileName) + " in " + Path.GetDirectoryName(TXTFileName));
        }
        public static void Import(int Ver, string TXTFile, string UexpFileName, string UassetFileName)
        {
            string[] Texts = File.ReadAllLines(TXTFile);
            int k = 0;
            List<byte[]> NewFile = new List<byte[]>();
            List<byte[]> NewHex = new List<byte[]>();
            List<byte[]> NewHeader = new List<byte[]>();
            List<byte[]> NewHeader2 = new List<byte[]>();
            BinaryReader bf = new BinaryReader(File.Open(UexpFileName, FileMode.Open));
            NewHeader.Add(bf.ReadBytes(57));
            bf.ReadBytes(4);
            NewHeader2.Add(bf.ReadBytes(29));
            for (int i = 0; i < 30000; i++)
            {
                try
                {
                    byte[] MainIDSize = bf.ReadBytes(4);
                    NewHex.Add(MainIDSize);
                    NewHex.Add(bf.ReadBytes(ToolClass.GetLittleEndian(MainIDSize)));
                    byte[] Next = bf.ReadBytes(1);
                    string emptyOrnot = BitConverter.ToString(Next);
                    NewHex.Add(Next);
                    if (emptyOrnot == "12")
                    {
                        NewHex.Add(bf.ReadBytes(7));
                        byte[] SecendIDSize = bf.ReadBytes(4);
                        NewHex.Add(SecendIDSize);
                        NewHex.Add(bf.ReadBytes(ToolClass.GetLittleEndian(SecendIDSize)));
                        NewHex.Add(bf.ReadBytes(25));
                    }
                    else
                    {
                        NewHex.Add(bf.ReadBytes(24));
                    }
                    byte[] HexOldSize = bf.ReadBytes(4);
                    int OldStringSize = ToolClass.GetLittleEndian(HexOldSize);
                    if (OldStringSize < 0 || OldStringSize > 0)
                    {
                        bool unicode = false;
                        if (OldStringSize < 0)
                        {
                            OldStringSize = -OldStringSize * 2;
                            unicode = true;
                        }
                        bf.ReadBytes(OldStringSize);
                        Texts[k] = ToolClass.StringDeClear(Texts[k]);
                        byte[] NewText;
                        if (unicode)
                        {
                            NewText = ToolClass.AddBytesToArray(Encoding.Unicode.GetBytes(Texts[k]), new byte[] { 0x00, 0x00 });
                            NewHex.Add(ToolClass.ByteFromInt32(-Convert.ToInt32(NewText.Length / 2)));
                        }
                        else
                        {
                            NewText = ToolClass.AddBytesToArray(Encoding.ASCII.GetBytes(Texts[k]), new byte[] { 0x00 });
                            NewHex.Add(ToolClass.ByteFromInt32(Convert.ToInt32(NewText.Length)));
                        }
                        NewHex.Add(NewText);
                        k++;
                    }
                    else
                    {
                        NewHex.Add(HexOldSize);
                        NewHex.Add(bf.ReadBytes(OldStringSize));
                    }
                    NewHex.Add(bf.ReadBytes(16));
                    byte[] Next2 = bf.ReadBytes(4);
                    NewHex.Add(Next2);
                    string f = BitConverter.ToString(Next2).Replace("-", "");
                    if (f == "04000000")
                    {
                        if (Ver == 2) { NewHex.Add(bf.ReadBytes(132)); }
                        if (Ver == 3) { NewHex.Add(bf.ReadBytes(120)); }
                    }
                    else
                    {
                        NewHex.Add(bf.ReadBytes(5));
                        byte[] sizes = bf.ReadBytes(4);
                        NewHex.Add(bf.ReadBytes(ToolClass.GetLittleEndian(sizes)));
                        if (Ver == 2) { NewHex.Add(bf.ReadBytes(123)); }
                        if (Ver == 3) { NewHex.Add(bf.ReadBytes(118)); }
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
            NewFile.Add(NewHeader.SelectMany(byteArr => byteArr).ToArray());
            int fole = NewHeader.SelectMany(byteArr => byteArr).ToArray().Count() + 4 + NewHeader2.SelectMany(byteArr => byteArr).ToArray().Count() + NewHex.SelectMany(byteArr => byteArr).ToArray().Count();
            if (Ver == 3) { NewFile.Add(ToolClass.ByteFromInt32(fole - 98)); }
            if (Ver == 2) { NewFile.Add(ToolClass.ByteFromInt32(fole - 94)); }
            NewFile.Add(NewHeader2.SelectMany(byteArr => byteArr).ToArray());
            NewFile.Add(NewHex.SelectMany(byteArr => byteArr).ToArray());
            File.WriteAllBytes(UexpFileName + ".NEW", NewFile.SelectMany(byteArr => byteArr).ToArray());
            bf.BaseStream.Close();
            File.Delete(UassetFileName + ".NEW");
            File.Copy(UassetFileName, UassetFileName + ".NEW");
            BinaryWriter AssetFile = new BinaryWriter(File.Open(UassetFileName + ".NEW", FileMode.Open));
            AssetFile.BaseStream.Position = AssetFile.BaseStream.Length - 92;
            byte[] fo = BitConverter.GetBytes(NewFile.SelectMany(byteArr => byteArr).ToArray().Count() - 4);
            AssetFile.Write(fo, 0, fo.Length);
            AssetFile.BaseStream.Close();
            MessageBox.Show("Import Done!" + Environment.NewLine + " Saved as \"" + Path.GetFileNameWithoutExtension(UexpFileName) + ".NEW" + "\" in " + Path.GetDirectoryName(UexpFileName));
        }
    }
}
