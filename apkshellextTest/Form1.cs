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
using System.Drawing.Drawing2D;

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
            //parseapk();
        }

        private void Button2_Click(object sender, EventArgs e) {
            AppPackageReader reader = AppPackageReader.Read(textBox1.Text);
            reader.setFlag("ImageSize", 48);
            pictureBox2.Image = reader.Icon;

            //parseapk();

            Bitmap b = new Bitmap(1000, 1000);
            using (Graphics g = Graphics.FromImage(b)) {
                //g.DrawPath(new Pen(Color.Black,1), VectorDrawableRender.Convert2Path("M 100 100 L 300 100 L 200 300 z"));
                g.FillPath(new SolidBrush(Color.Brown), VectorDrawableRender.Convert2Path("M0,0h108v108h-108z"));
                //g.FillPath(new SolidBrush(Color.Green), VectorDrawableRender.Convert2Path("M100,200 C100,100 250,100 250,200S400,300 400,200"));
                //g.FillPath(new SolidBrush(Color.Red), VectorDrawableRender.Convert2Path("M200,300 Q400,50 600,300 T1000,300"));
                //g.DrawPath(new Pen(Color.Purple, 2), VectorDrawableRender.Convert2Path("M15.67,4H14V2h-4v2H8.33C7.6,4 7,4.6 7,5.33V9h4.93L13,7v2h4V5.33C17,4.6 16.4,4 15.67,4z"));    
                //g.DrawPath(new Pen(Color.Purple, 2), VectorDrawableRender.Convert2Path("M600,350 l 50,-25           a25, 25 -30 0, 1 50, -25 l 50, -25           a25, 50 -30 0, 1 50, -25 l 50, -25          a25, 75 -30 0, 1 50, -25 l 50, -25           a25, 100 -30 0, 1 50, -25 l 50, -25"));
                if (textBox1.Text != "") {
                    g.DrawPath(new Pen(Color.Brown, 2),
                        VectorDrawableRender.Convert2Path(textBox1.Text));
                }
                
            }
            pictureBox1.Size = b.Size;
            pictureBox1.Image = b;
            pictureBox1.Size = new Size(100, 100);
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
                s.Close();
                zip.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

        }


    }
}
