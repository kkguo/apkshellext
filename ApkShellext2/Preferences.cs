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
using System.Configuration;
using System.Diagnostics;

namespace ApkShellext2 {

    public partial class Preferences : Form {
        public Preferences() {
            InitializeComponent();
        }

        public string currentFile = "";
        private bool formLoaded = false;
        private bool needClearThumbnailCache = false;

        private void Preferences_Load(object sender, EventArgs e) {
            Utility.Localize();

            Log("Using setting file from: " + ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);

            #region Initialize text
            this.Text = Resources.strPreferencesCaption;
            this.Icon = Icon.FromHandle(Utility.ResizeBitmap(Properties.NonLocalizeResources.logo, 16).GetHicon());
            
            btnOK.Text = Resources.btnOK;

            // Tree view on the left
            twLeft.BeginUpdate();
            twLeft.Nodes.Clear();
            twLeft.Nodes.Add(new TreeNode(Resources.twGeneral));
            twLeft.Nodes.Add(new TreeNode(Resources.twIcon));
            twLeft.Nodes.Add(new TreeNode(Resources.twContextMenu));
            twLeft.Nodes.Add(new TreeNode(Resources.twRename));
            twLeft.Nodes.Add(new TreeNode(Resources.twInfotip));
            //twLeft.SelectedNode = twLeft.Nodes[Int16.Parse(Utility.GetSetting("LastPanel","0"))];
            twLeft.ExpandAll();
            twLeft.EndUpdate();

            #region General Panel
            // Dropdown
            if (!formLoaded) {
                CultureInfo[] culs = Utility.getSupportedLanguages();
                foreach (var l in culs) {
                    combLanguage.Items.Add(l.NativeName);
                    if (l.LCID == Thread.CurrentThread.CurrentUICulture.LCID) {
                        combLanguage.Text = l.NativeName;
                    }
                }
                if (combLanguage.Text == "")
                    combLanguage.Text = culs[0].NativeName;
            }

            lblLanguage.Text = Resources.strLanguages;
            //lblHelpTranslate.Text;
            lblCurrentVersion.Text = string.Format(Resources.strCurrVersion, Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (Utility.NewVersionAvailible()) {
                lblNewVer.Text = string.Format(Resources.strNewVersionAvailible, Utility.GetSetting("LatestVersion"));
                btnUpdate.Text = Resources.btnUpdate;
                btnUpdate.Image = Utility.ResizeBitmap(Properties.NonLocalizeResources.iconUpdate, 16);
                toolTip1.SetToolTip(btnUpdate, Resources.btnUpdateToolTip);
            } else {
                lblNewVer.Text = Resources.strGotLatest;
                btnUpdate.Text = Resources.btnGitHub;
                btnUpdate.Image = Utility.ResizeBitmap(Properties.NonLocalizeResources.iconGitHub, 16);
            }

            //btnUpdate.Visible = false;
            toolTip1.SetToolTip(btnUpdate, Resources.strGotoProjectSite);

            lblHelpTranslate.Text = Resources.strHelpTranslate;
            #endregion

            #region Icon Panel
            ckShowOverlay.Text = Resources.strShowOverlayIcon;
            toolTip1.SetToolTip(ckShowOverlay, Resources.strShowOverlayIconToolTip);
            ckShowOverlay.Checked = Utility.GetSetting("ShowOverLayIcon")=="True";
            ckShowIPA.Text = Resources.strShowIpaIcon;
            ckShowIPA.Checked = Utility.GetSetting("ShowIpaIcon") == "True";
            ckShowAppxIcon.Text = Resources.strShowAppxIcon;
            ckShowAppxIcon.Checked = Utility.GetSetting("ShowAppxIcon") == "True";
            ckStretchThumbnail.Text = Resources.strStretchThumbnail;
            ckStretchThumbnail.Checked = Utility.GetSetting("StretchThumbnail") == "True";
            ckEnableThumbnail.Text = Resources.strEnableThumbnail;
            ckEnableThumbnail.Checked = Utility.GetSetting("EnableThumbnail") == "True";
            btnClearCache.Text = Resources.strClearCache;
            #endregion

            #region ContextMenu Panel
            ckAlwaysShowStore.Text = Resources.strAlwaysShowGooglePlay;
            toolTip1.SetToolTip(ckAlwaysShowStore, Resources.strAlwaysShowGooglePlayToolTip);
            ckAlwaysShowStore.Checked = Utility.GetSetting("ShowAppStoreWhenMultiSelected") == "True";
            ckShowMenuIcon.Checked = Utility.GetSetting("ShowMenuIcon") == "True";
            ckShowMenuIcon.Text = Resources.strShowContextMenuIcon;
            ckShowNewVersionInfo.Checked = Utility.GetSetting("ShowNewVersion") == "True";
            ckShowNewVersionInfo.Text = Resources.strShowNewVerInfo;
            ckShowGoogle.Checked = Utility.GetSetting("ShowGooglePlay") == "True";
            ckShowGoogle.Text = Resources.strShowGooglePlay;
            ckShowAM.Checked = Utility.GetSetting("ShowApkMirror") == "True";
            ckShowAM.Text = Resources.strShowApkMirror;
            ckShowAmazon.Checked = Utility.GetSetting("ShowAmazonStore") == "True";
            ckShowAmazon.Text = Resources.strShowAmazonStore;
            ckShowApple.Checked = Utility.GetSetting("ShowAppleStore") == "True";
            ckShowApple.Text = Resources.strShowAppleStore;
            ckShowMS.Checked = Utility.GetSetting("ShowMSStore") == "True";
            ckShowMS.Text = Resources.strShowMSStore;
            ckShowAM.Checked = Utility.GetSetting("ShowApkMirror") == "True";
            ckShowAM.Text = Resources.strShowApkMirror;
            #endregion

            #region Renaming Panel
            lblRenamePattern.Text = Resources.strRenamePattern;
            llbPatternVariables.Text = Resources.strReferToWiki;

            btnResetRenamePattern.Text = Resources.btnResetPattern;
            btnResetRenamePattern_Click(this, new EventArgs());

            ckReplaceSpace.Text = Resources.strReplaceSpaceWith_;
            ckReplaceSpace.Checked = Utility.GetSetting("ReplaceSpace") == "True";
            #endregion

            #region ToolTip Panel
            lblInfoTipPattern.Text = Resources.strInfoTipPattern;
            string pattern = Utility.GetSetting("ToolTipPattern");
            if (pattern == "") {
                txtToolTipPattern.Text = Resources.strInfoTipDefault;
            } else {
                txtToolTipPattern.Text = pattern;
            }
            btnResetInfoTipPattern.Text = Resources.btnResetPattern;

            llbInfoTipPattVar.Text = Resources.strReferToWiki;
            #endregion

            #endregion

            twLeft.Select();
            pnlRight.Visible = true;
            btnOK.Focus();
            formLoaded = true;
        }

        private void DisablePenels() {
            pnlContextMenu.Visible = false;
            pnlGeneral.Visible = false;
            pnlIcon.Visible = false;
            pnlRenaming.Visible = false;
            pnlInfoTip.Visible = false;
            btnResetInfoTipPattern.Visible = false;
            btnResetRenamePattern.Visible = false;
            btnUpdate.Visible = false;
        }

        private void combLanguage_SelectedIndexChanged(object sender, EventArgs e) {
            CultureInfo[] culs = Utility.getSupportedLanguages();
            CultureInfo cul = culs[combLanguage.SelectedIndex];
            Utility.SaveSetting("Language", cul.Name);
            if (formLoaded)
                this.OnLoad(e);
        }

        private void ckShowIPA_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowIpaIcon", ckShowIPA.Checked);
            Utility.refreshShell();
        }

        private void ckShowAppxIcon_CheckedChanged(object sender, EventArgs e) {

            Utility.SaveSetting("ShowAppxIcon",ckShowAppxIcon.Checked);       
            SharpShell.Interop.Shell32.SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
        }

        private void ckShowOverlay_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowOverLayIcon", ckShowOverlay.Checked);
            if (formLoaded) {
                SharpShell.Interop.Shell32.SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
                if (Utility.GetSetting("EnableThumbnail")=="True")
                    needClearThumbnailCache = true;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(string.Format(NonLocalizeResources.urlGithubHomeWithVersion, Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }

        private void ckShowMenuIcon_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowMenuIcon", ckShowMenuIcon.Checked);
        }

        private void ckShowPlay_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowAppStoreWhenMultiSelected", ckAlwaysShowStore.Checked);
        }

        private void twLeft_AfterSelect(object sender, TreeViewEventArgs e) {
            DisablePenels();
            if (twLeft.SelectedNode.Text == Resources.twGeneral) {
                pnlGeneral.Visible = true;
                btnUpdate.Visible = true;
            } else if (twLeft.SelectedNode.Text == Resources.twIcon) {
                pnlIcon.Visible = true;
            } else if (twLeft.SelectedNode.Text == Resources.twRename) {
                pnlRenaming.Visible = true;
                btnResetRenamePattern.Visible = true;
            } else if (twLeft.SelectedNode.Text == Resources.twContextMenu) {
                pnlContextMenu.Visible = true;
            } else if (twLeft.SelectedNode.Text == Resources.twInfotip) {
                pnlInfoTip.Visible = true;
                btnResetInfoTipPattern.Visible = true;
            }
            Utility.SaveSetting("LastPanel", twLeft.SelectedNode.Index);
        }

        private void btnOK_Click(object sender, EventArgs e) {
            if (RenamePatternIsDirty)
                Utility.SaveSetting("RenamePattern", txtRenamePattern.Text);
            if (ToolTipPatternIsDirty)
                Utility.SaveSetting("ToolTipPattern", txtToolTipPattern.Text);
            this.Close();
        }

        private void ckShowAmazon_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowAmazonStore", ckShowAmazon.Checked);
        }

        private void ckbShowGoogle_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowGooglePlay", ckShowGoogle.Checked);
        }

        private void btnResetTooltipPattern_Click(object sender, EventArgs e) {
            txtToolTipPattern.Text = Resources.strInfoTipDefault;
        }

        private void btnResetRenamePattern_Click(object sender, EventArgs e) {
            string pattern = Utility.GetSetting("RenamePattern").ToString();
            if (pattern == "")
                txtRenamePattern.Text = Resources.strRenamePatternDefault;                
            else 
                txtRenamePattern.Text = pattern;
        }

        private void ckShowApple_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowAppleStore",ckShowGoogle.Checked);
        }

        private void ckShowMS_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowMSStore", ckShowGoogle.Checked);
        }

        private void ckShowAM_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowApkMirror", ckShowGoogle.Checked);
        }

        private bool RenamePatternIsDirty = false;
        private void txtRename_TextChanged(object sender, EventArgs e) {
                RenamePatternIsDirty = formLoaded;
        }

        private bool ToolTipPatternIsDirty = false;
        private void txtToolTipPattern_TextChanged(object sender, EventArgs e) {
                ToolTipPatternIsDirty = formLoaded;
        }

        private void Preferences_FormClosed(object sender, FormClosedEventArgs e) {
            if (needClearThumbnailCache) {
                if (System.Windows.Forms.MessageBox.Show(
                    Resources.dialogNeedClearCache,
                    Resources.strClearCache,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                        btnClearCache_Click(sender, new EventArgs());
                }
            }
        }

        private void ckReplaceSpace_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ReplaceSpace", ckReplaceSpace.Checked);
        }

        private void llbPatternVariables_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(NonLocalizeResources.urlPatternVarWiki);
        }

        private void btnUpdate_Click_1(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(string.Format(Properties.NonLocalizeResources.urlGithubHomeWithVersion,Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }

        private void llbInfoTipPattVar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(NonLocalizeResources.urlPatternVarWiki);
        }

        private void ckStretchIcon_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("StretchThumbnail", ckStretchThumbnail.Checked);
            needClearThumbnailCache = formLoaded;
        }

        private void lblHelpTranslate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(NonLocalizeResources.urlTranslate);
        }

        private void ckEnableThumbnail_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("EnableThumbnail",ckEnableThumbnail.Checked);
            needClearThumbnailCache = formLoaded;
        }

        private void Log(string message) {
            Utility.Log(this, "", message);
        }

        private void btnClearCache_Click(object sender, EventArgs e) {
            if (System.Windows.Forms.MessageBox.Show(
                    Resources.dialogClearCache,
                    Resources.strClearCache,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                Process process = new Process();
                string path = Path.GetTempPath() + "\\clearcache.bat";
                File.WriteAllText(path, NonLocalizeResources.cmdClearCache);
                process.StartInfo.FileName = path;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
            }
        }

        private void CkShowNewVersionInfo_CheckedChanged(object sender, EventArgs e) {
            Utility.SaveSetting("ShowNewVersion", ckShowNewVersionInfo.Checked);
        }

        private void PnlRenaming_Paint(object sender, PaintEventArgs e) {

        }
    }
}
