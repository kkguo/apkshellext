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

namespace ApkShellext2 {

    public partial class Preferences : Form {
        public Preferences() {
            InitializeComponent();
        }

        public string currentFile = "";
        private bool formLoaded = false;

        private void Preferences_Load(object sender, EventArgs e) {
            Utility.Localize();

            Log("Using setting file from: " + ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);

            #region Initialize text
            this.Text = Resources.strPreferencesCaption;
            this.Icon = Icon.FromHandle(Utility.ResizeBitmap(Properties.Resources.logo, 16).GetHicon());
            
            btnOK.Text = Resources.btnOK;

            // Tree view on the left
            twLeft.BeginUpdate();
            twLeft.Nodes.Clear();
            twLeft.Nodes.Add(new TreeNode(Resources.twGeneral));
            twLeft.Nodes.Add(new TreeNode(Resources.twIcon));
            twLeft.Nodes.Add(new TreeNode(Resources.twContextMenu));
            twLeft.Nodes.Add(new TreeNode(Resources.twRename));
            twLeft.Nodes.Add(new TreeNode(Resources.twInfotip));
            twLeft.SelectedNode = twLeft.Nodes[Settings.Default.LastPanel];
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
                lblNewVer.Text = string.Format(Resources.strNewVersionAvailible, Settings.Default.LatestVersion);
                btnUpdate.Text = Resources.btnUpdate;
                btnUpdate.Image = Utility.ResizeBitmap(Properties.Resources.iconUpdate, 16);
                toolTip1.SetToolTip(btnUpdate, Resources.btnUpdateToolTip);
            } else {
                lblNewVer.Text = Resources.strGotLatest;
                btnUpdate.Text = Resources.btnGitHub;
                btnUpdate.Image = Utility.ResizeBitmap(Properties.Resources.iconGitHub, 16);
            }

            //btnUpdate.Visible = false;
            toolTip1.SetToolTip(btnUpdate, Resources.strGotoProjectSite);

            lblHelpTranslate.Text = Resources.strHelpTranslate;
            #endregion

            #region Icon Panel
            ckShowOverlay.Text = Resources.strShowOverlayIcon;
            toolTip1.SetToolTip(ckShowOverlay, Resources.strShowOverlayIconToolTip);
            ckShowOverlay.Checked = Settings.Default.ShowOverLayIcon;
            ckShowIPA.Text = Resources.strShowIpaIcon;
            ckShowIPA.Checked = Settings.Default.ShowIpaIcon;
            ckShowAppxIcon.Text = Resources.strShowAppxIcon;
            ckShowAppxIcon.Checked = Settings.Default.ShowAppxIcon;
            ckStretchThumbnail.Text = Resources.strStretchThumbnail;
            ckStretchThumbnail.Checked = Settings.Default.StretchThumbnail;
            #endregion

            #region ContextMenu Panel
            ckAlwaysShowStore.Text = Resources.strAlwaysShowGooglePlay;
            toolTip1.SetToolTip(ckAlwaysShowStore, Resources.strAlwaysShowGooglePlayToolTip);
            ckAlwaysShowStore.Checked = Settings.Default.ShowAppStoreWhenMultiSelected;
            ckShowMenuIcon.Checked = Settings.Default.ShowMenuIcon;
            ckShowMenuIcon.Text = Resources.strShowContextMenuIcon;
            ckShowNewVersionInfo.Checked = Settings.Default.ShowNewVersion;
            ckShowNewVersionInfo.Text = Resources.strShowNewVerInfo;
            ckShowGoogle.Checked = Settings.Default.ShowGooglePlay;
            ckShowGoogle.Text = Resources.strShowGooglePlay;
            ckShowAM.Checked = Settings.Default.ShowApkMirror;
            ckShowAM.Text = Resources.strShowApkMirror;
            ckShowAmazon.Checked = Settings.Default.ShowAmazonStore;
            ckShowAmazon.Text = Resources.strShowAmazonStore;
            ckShowApple.Checked = Settings.Default.ShowAppleStore;
            ckShowApple.Text = Resources.strShowAppleStore;
            ckShowMS.Checked = Settings.Default.ShowMSStore;
            ckShowMS.Text = Resources.strShowMSStore;
            ckShowAM.Checked = Settings.Default.ShowApkMirror;
            ckShowAM.Text = Resources.strShowApkMirror;
            #endregion

            #region Renaming Panel
            lblRenamePattern.Text = Resources.strRenamePattern;
            llbPatternVariables.Text = Resources.strReferToWiki;

            btnResetRenamePattern.Text = Resources.btnResetPattern;
            btnResetRenamePattern_Click(this, new EventArgs());

            ckReplaceSpace.Text = Resources.strReplaceSpaceWith_;
            ckReplaceSpace.Checked = Settings.Default.ReplaceSpace;
            #endregion

            #region ToolTip Panel
            lblInfoTipPattern.Text = Resources.strInfoTipPattern;
            string pattern = Settings.Default.ToolTipPattern;
            if (pattern == "") {
                txtToolTipPattern.Text = Resources.strInfoTipDefault;
            } else {
                txtToolTipPattern.Text = pattern;
            }
            btnResetInfoTipPattern.Text = Resources.btnResetPattern;

            llbInfoTipPattVar.Text = Resources.strReferToWiki;
            #endregion

            #endregion

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
            Settings.Default.Language = cul.LCID;
            if (formLoaded)
                this.OnLoad(e);
        }

        private void ckShowIPA_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ShowIpaIcon = ckShowIPA.Checked;
            Utility.refreshShell();
        }

        private void ckShowAppxIcon_CheckedChanged(object sender, EventArgs e) {

            Settings.Default.ShowAppxIcon = ckShowAppxIcon.Checked;       
            SharpShell.Interop.Shell32.SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
        }

        private void ckShowOverlay_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ShowOverLayIcon = ckShowOverlay.Checked;
            SharpShell.Interop.Shell32.SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
        }

        private void btnUpdate_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(string.Format(Resources.urlGithubHomeWithVersion, Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }

        private void ckShowMenuIcon_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ShowMenuIcon = ckShowMenuIcon.Checked;
        }

        private void ckShowPlay_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ShowAppStoreWhenMultiSelected = ckAlwaysShowStore.Checked;
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
            Settings.Default.LastPanel = twLeft.SelectedNode.Index;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            if (RenamePatternIsDirty)
                Settings.Default.RenamePattern = txtRenamePattern.Text;
            if (ToolTipPatternIsDirty)
                Settings.Default.ToolTipPattern = txtToolTipPattern.Text;
            this.Close();
        }

        private void ckShowAmazon_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ShowAmazonStore = ckShowAmazon.Checked;
        }

        private void ckbShowGoogle_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ShowGooglePlay = ckShowGoogle.Checked;
        }

        private void btnResetTooltipPattern_Click(object sender, EventArgs e) {
            txtToolTipPattern.Text = Resources.strInfoTipDefault;
        }

        private void btnResetRenamePattern_Click(object sender, EventArgs e) {
            string pattern = Settings.Default.RenamePattern;
            if (pattern == "")
                txtRenamePattern.Text = Resources.strRenamePatternDefault;                
            else 
                txtRenamePattern.Text = pattern;
        }

        private void ckShowApple_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ShowAppleStore = ckShowGoogle.Checked;
        }

        private void ckShowMS_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ShowMSStore = ckShowGoogle.Checked;
        }

        private void ckShowAM_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ShowApkMirror = ckShowGoogle.Checked;
        }

        private bool RenamePatternIsDirty = false;
        private void txtRename_TextChanged(object sender, EventArgs e) {
            if (formLoaded)
                RenamePatternIsDirty = true;
        }

        private bool ToolTipPatternIsDirty = false;
        private void txtToolTipPattern_TextChanged(object sender, EventArgs e) {
            if (formLoaded)
                ToolTipPatternIsDirty = true;
        }

        private void Preferences_FormClosed(object sender, FormClosedEventArgs e) {
            Settings.Default.Save();
        }

        private void ckReplaceSpace_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.ReplaceSpace = ckReplaceSpace.Checked;
        }

        private void llbPatternVariables_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(Resources.urlPatternVarWiki);
        }

        private void btnUpdate_Click_1(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(string.Format(Properties.Resources.urlGithubHomeWithVersion,Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }

        private void llbInfoTipPattVar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(Resources.urlPatternVarWiki);
        }

        private void ckStretchIcon_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.StretchThumbnail = ckStretchThumbnail.Checked;
        }

        private void lblHelpTranslate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(Resources.urlTranslate);
        }

        private void ckEnableThumbnail_CheckedChanged(object sender, EventArgs e) {

        }

        private void Log(string message) {
            Utility.Log(this, "", message);
        }

    }
}
