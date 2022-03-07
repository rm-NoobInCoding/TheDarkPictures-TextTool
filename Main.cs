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

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Uexp File (*.uexp) | *.uexp"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                button2.Enabled = false;
                button1.Enabled = false;
                button1.Text = "Working...";
                TextTool.Export(ofd.FileName);
                button1.Text = "Export";
                button2.Enabled = true;
                button1.Enabled = true;
            }


        }

        private void Button2_Click(object sender, EventArgs e)
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
                TextTool.Import(OpenTXT.FileName);
                button2.Text = "Import";
                button2.Enabled = true;
                button1.Enabled = true;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
