using System;
using System.Windows.Forms;

namespace The_Dark_Pictures
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a version");
            }
            else
            {
                OpenFileDialog OpenUexp = new OpenFileDialog
                {
                    Filter = "UEXP File (*.uexp) | *.uexp"
                };
                if (OpenUexp.ShowDialog() == DialogResult.OK)
                {
                    SaveFileDialog SaveTXT = new SaveFileDialog
                    {
                        Filter = "Text File (*.txt) | *.txt"
                    };
                    if (SaveTXT.ShowDialog() == DialogResult.OK)
                    {
                        button2.Enabled = false;
                        button1.Enabled = false;
                        button1.Text = "Working...";
                        ToolB.Export(comboBox1.SelectedIndex + 1, OpenUexp.FileName, SaveTXT.FileName);
                        button1.Text = "Export";
                        button2.Enabled = true;
                        button1.Enabled = true;
                    }
                }
            }

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a version");
            }
            else
            {
                OpenFileDialog OpenUexp = new OpenFileDialog
                {
                    Filter = "Orginal UEXP File (*.uexp) | *.uexp"
                };
                if (OpenUexp.ShowDialog() == DialogResult.OK)
                {
                    OpenFileDialog OpenOpenUasset = new OpenFileDialog
                    {
                        Filter = "Orginal Uasset File (*.uasset) | *.uasset"
                    };
                    if (OpenOpenUasset.ShowDialog() == DialogResult.OK)
                    {
                        OpenFileDialog OpenTXT = new OpenFileDialog
                        {
                            Filter = "Exported Text File (*.txt) | *.txt"
                        };
                        if (OpenTXT.ShowDialog() == DialogResult.OK)
                        {
                            button2.Enabled = false;
                            button1.Enabled = false;
                            button2.Text = "Working...";
                            ToolB.Import(comboBox1.SelectedIndex + 1, OpenTXT.FileName, OpenUexp.FileName, OpenOpenUasset.FileName);
                            button2.Text = "Import";
                            button2.Enabled = true;
                            button1.Enabled = true;
                        }
                    }
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = 0;
        }
    }
}
