using SharpShell.Diagnostics;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ApkShellext2.Properties;
using System.Globalization;

namespace ApkShellext2 {

    public partial class Preferences : Form {
        public Preferences() {
            InitializeComponent();
        }

        private bool formLoaded = false;
        private bool updateChecked = false;

        private void Preferences_Load(object sender, EventArgs e) {
            Utility.Localize();

            if (!formLoaded) {
                CultureInfo[] culs = Utility.getSupportedLanguages();
                combLanguage.Text = culs[0].NativeName;
                foreach (var l in culs) {
                    combLanguage.Items.Add(l.NativeName);
                    if (l.LCID == Thread.CurrentThread.CurrentUICulture.LCID) {
                        combLanguage.Text = l.NativeName;
                    }
                }
            }

            lblLanguage.Text = Resources.strLanguages;

            this.Text = Resources.strPreferencesCaption;
            this.Icon = Icon.FromHandle(Utility.ResizeBitmap(Properties.Resources.logo, 16).GetHicon());
            btnUpdate.Image = Utility.ResizeBitmap(Properties.Resources.GitHub, 16);

            label1.Text =  string.Format(Resources.strCurrVersion, Assembly.GetExecutingAssembly().GetName().Version.ToString());

            btnCancel.Text = Resources.btnCancel;
            btnUpdate.Text = Resources.btnGitHub;
            toolTip1.SetToolTip(btnUpdate, Resources.strGotoProjectSite);
            checkBox1.Text = Resources.strRenameWithVersionCode;
            toolTip1.SetToolTip(checkBox1, Resources.strRenameToolTip);
            checkBox2.Text = Resources.strAlwaysShowGooglePlay;
            toolTip1.SetToolTip(checkBox2, Resources.strAlwaysShowGooglePlayToolTip);
            checkBox3.Text = Resources.strShowOverlayIcon;
            toolTip1.SetToolTip(checkBox3, Resources.strShowOverlayIconToolTip);

            lblNewVer.Text = Resources.strCheckingNewVersion;

            checkBox1.Checked = (Utility.getRegistrySetting(Utility.keyRenameWithVersionCode) == 1);
            checkBox2.Checked = (Utility.getRegistrySetting(Utility.keyAlwaysShowGooglePlay) == 1);
            checkBox3.Checked = (Utility.getRegistrySetting(Utility.keyShowOverlay) == 1);

            if (!updateChecked) {
                Thread getVersionTh = new Thread(new ThreadStart(getLatestVersion));
                getVersionTh.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                getVersionTh.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                getVersionTh.Start();
            }
            btnCancel.Focus();
            formLoaded = true;

            // Secret garden, disable show ipa icon for now, as it's a beta
            int ipa = Utility.getRegistrySetting("ShowIpaIcon", 100);
            checkBox4.Text = Resources.strShowIpaIcon;
            checkBox4.Visible = (ipa != 100);
            checkBox4.Checked = (ipa == 1);
        }

        private void getLatestVersion() {
            try {
                byte[] buf = new byte[1024];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Properties.Resources.urlGithubHomeLatest);
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

                // version number should be always 4 parts
                for (int i = 0; i < latestV.Length; i++) {
                    if (latestV[i] != curV[i]) {
                        if (int.Parse(latestV[i]) > int.Parse(curV[i])) {
                            this.Invoke((Action)(() => {
                                lblNewVer.Text = string.Format(Resources.strNewVersionAvailible, s);
                                btnUpdate.Text = Resources.btnUpdate;
                                btnUpdate.Image = Utility.ResizeBitmap(Properties.Resources.udpate, 16);
                                btnUpdate.Focus();
                                toolTip1.SetToolTip(btnUpdate, Resources.btnUpdateToolTip);
                            }));
                            return;
                        } else {
                            break;
                        }
                    }
                }
                this.Invoke((Action)(() => {
                    lblNewVer.Text = Resources.strGotLatest;
                    btnUpdate.Text = Resources.btnGitHub;
                }));
                return;

            } catch (Exception ex) {
                lblNewVer.Invoke((Action)(()=>lblNewVer.Text = Resources.strGotLatest));
                btnUpdate.Invoke((Action)(()=>btnUpdate.Text = Resources.btnGitHub));
                Logging.Log("Cannot access the web " + ex.Message);
            }
        }

         private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e) {
            if ((ModifierKeys & Keys.Shift) != 0) checkBox4.Visible = true;
            System.Diagnostics.Process.Start(string.Format(Resources.urlGithubHomeWithVersion, Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            Utility.setRegistrySetting(Utility.keyRenameWithVersionCode, checkBox1.Checked ? 1 : 0);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            Utility.setRegistrySetting(Utility.keyAlwaysShowGooglePlay, checkBox2.Checked ? 1 : 0);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) {
            if (Utility.getRegistrySetting(Utility.keyShowOverlay) != (checkBox3.Checked ? 1 : 0)) {
                Utility.setRegistrySetting(Utility.keyShowOverlay, checkBox3.Checked ? 1 : 0);
                SharpShell.Interop.Shell32.SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void combLanguage_SelectedIndexChanged(object sender, EventArgs e) {
            if (formLoaded && Utility.getSupportedLanguages()[combLanguage.SelectedIndex].LCID != Thread.CurrentThread.CurrentCulture.LCID) {
                Utility.setRegistrySetting("language", Utility.getSupportedLanguages()[combLanguage.SelectedIndex].LCID);
                this.OnLoad(e);
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e) {
            if (Utility.getRegistrySetting(Utility.keyShowIpaIcon) != (checkBox4.Checked ? 1 : 0)) {
                Utility.setRegistrySetting(Utility.keyShowIpaIcon, checkBox4.Checked ? 1 : 0);
                SharpShell.Interop.Shell32.SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}
