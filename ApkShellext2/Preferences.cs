using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using SharpShell.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Reflection;

namespace ApkShellext2 {
    public partial class Preferences : Form {
        public Preferences() {
            InitializeComponent();
        }

        Thread getVersionTh;
        private void Settings_Load(object sender, EventArgs e) {
            this.Icon = Icon.FromHandle(Utility.ResizeBitmap(Properties.Resources.logo,16).GetHicon());
            btnUpdate.Image = Utility.ResizeBitmap(Properties.Resources.GitHub, 16);
            label1.Text = "Current Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            checkBox1.Checked = (Utility.getRegistrySetting("RenameWithVersionCode") == 1);
            checkBox2.Checked = (Utility.getRegistrySetting("AlwaysShowGooglePlay") == 1);
            getVersionTh = new Thread(new ThreadStart(getLatestVersion));
            getVersionTh.Start();
            btnCancel.Focus();
        }

        private void getLatestVersion() {
            try {
                lblNewVer.Text = "Checking newer version...";
                byte[] buf = new byte[1024];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://kkguo.github.io/apkshellext/latest");
                // execute the request
                HttpWebResponse response = (HttpWebResponse)
                    request.GetResponse();
                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();
                int count = resStream.Read(buf, 0, buf.Length);
                string s = "";
                if (count != 0) {
                    s = Encoding.ASCII.GetString(buf, 0, count);
                }                
                s = Regex.Replace(s, @"\t|\n|\r", "");
                Logging.Log("Get the latest version :" + s);
                string[] latestV = s.Split(new Char[] { '.' });
                string s1 = GetType().Assembly.GetName().Version.ToString();
                string[] curV = s1.Split(new Char[] { '.' });
                bool newAvail = false;
                // version number should be always 4 parts
                for (int i = 0; i < latestV.Length; i++) {
                    if (int.Parse(latestV[i]) > int.Parse(curV[i])) {
                        newAvail = true;
                        break;
                    }
                }
                if (newAvail) {
                    lblNewVer.Text= "Version " + s + " is availible online";
                    btnUpdate.Text = "Update";
                    btnUpdate.Image = Utility.ResizeBitmap(Properties.Resources.udpate,16);
                    btnUpdate.Focus();
                } else {
                    lblNewVer.Text = "Great! You got the latest version.";
                    btnUpdate.Text = "GitHub";
                }
            } catch (Exception ex) {
                lblNewVer.Text = "Check project site for update.";
                btnUpdate.Text = "GitHub";
                Logging.Log("Cannot access the web " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://kkguo.github.io/apkshellext/index.html?version=" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            Utility.setRegistrySetting("RenameWithVersionCode", checkBox1.Checked ? 1 : 0);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            Utility.setRegistrySetting("AlwaysShowGooglePlay", checkBox2.Checked ? 1 : 0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }

        private void lblNewVer_Click(object sender, EventArgs e)
        {

        }
    }
}
