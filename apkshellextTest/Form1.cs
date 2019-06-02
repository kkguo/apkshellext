using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApkShellext2;
using ApkQuickReader;
using ICSharpCode.SharpZipLib.Zip;
using ApkShellext2.ApkChunk;

namespace apkshellextTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "apkfile|*.apk";
            dialog.ShowDialog();
            textBox1.Text = dialog.FileName;
            parseapk();
        }

        private void Button2_Click(object sender, EventArgs e) {
            AppPackageReader reader = AppPackageReader.Read(textBox1.Text);
            reader.setFlag("ImageSize", 32);
            pictureBox1.Image = reader.Icon;
            
            parseapk();
        }

        private void parseapk() {
            if (File.Exists(textBox1.Text.Trim())) {
                ZipFile zip = new ZipFile(textBox1.Text);
                ZipEntry en = zip.GetEntry("androidmanifest.xml");
                BinaryReader s = new BinaryReader(zip.GetInputStream(en));
                byte[] bytes = s.ReadBytes((int)en.Size);
                ApkXMLChunk ck = new ApkXMLChunk(bytes);
                textBox2.Text = ck.OutputXML();
                
                //AppPackageReader reader  = AppPackageReader.Read(textBox1.Text);



            }

        }
    }
}
