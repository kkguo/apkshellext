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
        public event EventHandler newVersionEvent;
        public event EventHandler networkIssueEvent;

        private void Preferences_Load(object sender, EventArgs e) {
            int lang = Utility.getRegistrySetting("language", -1);
            if (lang != -1) { // language set
                Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }

            if (!formLoaded) {
                combLanguage.Text = Utility.SupportedLanguages[0].DisplayName;
                foreach (var l in Utility.SupportedLanguages) {
                    combLanguage.Items.Add(l.NativeName);
                    if (l.LCID == Thread.CurrentThread.CurrentUICulture.LCID) {
                        combLanguage.Text = l.DisplayName;
                    }
                }
            }

            lblLanguage.Text = Resources.strLanguages;

            this.Text = Resources.strPreferencesCation;
            this.Icon = Icon.FromHandle(Utility.ResizeBitmap(Properties.Resources.logo, 16).GetHicon());
            btnUpdate.Image = Utility.ResizeBitmap(Properties.Resources.GitHub, 16);
            linkLabel1.Text = Resources.urlGithubHome;
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
            toolTip1.SetToolTip(linkLabel1, Resources.strGotoProjectSite);

            lblNewVer.Text = Resources.strCheckingNewVersion;

            checkBox1.Checked = (Utility.getRegistrySetting(Properties.Resources.optRenameWithVersionCode) == 1);
            checkBox2.Checked = (Utility.getRegistrySetting(Properties.Resources.optAlwaysShowGooglePlay) == 1);
            checkBox3.Checked = (Utility.getRegistrySetting(Properties.Resources.optShowOverlay) == 1);

            newVersionEvent += OnNewVersionEvent;
            networkIssueEvent += OnNetworkIssue;
            Thread getVersionTh = new Thread(new ThreadStart(getLatestVersion));
            getVersionTh.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            getVersionTh.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            getVersionTh.Start();
            btnCancel.Focus();
            formLoaded = true;
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

                NewVersionEventArgs args = new NewVersionEventArgs();
                // version number should be always 4 parts
                for (int i = 0; i < latestV.Length; i++) {
                    if (int.Parse(latestV[i]) > int.Parse(curV[i])) {                        
                        args.newVersion = true;
                        args.version = s;
                        newVersionEvent(this, args);
                        return;
                    }
                }

                args.newVersion = false;
                args.version = s;
                newVersionEvent(this, args);
                return;

            } catch (Exception ex) {
                networkIssueEvent(this, new EventArgs());
                Logging.Log("Cannot access the web " + ex.Message);
            }
        }

        private void OnNewVersionEvent(object sender, EventArgs e) {
            NewVersionEventArgs arg = (NewVersionEventArgs)e;
            if (arg.newVersion) {
                lblNewVer.Text = string.Format(Resources.strNewVersionAvailible,arg.version);
                btnUpdate.Text = Resources.btnUpdate;
                btnUpdate.Image = Utility.ResizeBitmap(Properties.Resources.udpate, 16);
                toolTip1.SetToolTip(btnUpdate, Resources.btnUpdateToolTip);                
                btnUpdate.Focus();
            } else {
                lblNewVer.Text = Resources.strGotLatest;
                btnUpdate.Text = Resources.btnGitHub;
            }
        }

        private void OnNetworkIssue(object sender, EventArgs e) {
            lblNewVer.Text = Resources.strCheckProjectSite;
            btnUpdate.Text = Resources.btnGitHub;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(string.Format(Resources.urlGithubHomeWithVersion, Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            Utility.setRegistrySetting(Properties.Resources.optRenameWithVersionCode, checkBox1.Checked ? 1 : 0);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            Utility.setRegistrySetting(Properties.Resources.optAlwaysShowGooglePlay, checkBox2.Checked ? 1 : 0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }

        private void lblNewVer_Click(object sender, EventArgs e) {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) {
            if (Utility.getRegistrySetting(Properties.Resources.optShowOverlay) != (checkBox3.Checked ? 1 : 0)) {
                Utility.setRegistrySetting(Properties.Resources.optShowOverlay, checkBox3.Checked ? 1 : 0);
                SharpShell.Interop.Shell32.SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void combLanguage_SelectedIndexChanged(object sender, EventArgs e) {
            if (formLoaded && Utility.SupportedLanguages[combLanguage.SelectedIndex].LCID != Thread.CurrentThread.CurrentCulture.LCID) {
                Utility.setRegistrySetting("language", Utility.SupportedLanguages[combLanguage.SelectedIndex].LCID);
                this.OnLoad(e);
            }
        }
    }

    public class NewVersionEventArgs : EventArgs {
        public bool newVersion { get; set; }
        public string version { get; set; }
    }
}
