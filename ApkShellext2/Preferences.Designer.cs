namespace ApkShellext2 {
    partial class Preferences {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlLeft = new System.Windows.Forms.TableLayoutPanel();
            this.twLeft = new System.Windows.Forms.TreeView();
            this.lblRenamePattern = new System.Windows.Forms.Label();
            this.pnlRenaming = new System.Windows.Forms.TableLayoutPanel();
            this.txtRenamePattern = new System.Windows.Forms.TextBox();
            this.ckReplaceSpace = new System.Windows.Forms.CheckBox();
            this.llbPatternVariables = new System.Windows.Forms.LinkLabel();
            this.btnResetRenamePattern = new System.Windows.Forms.Button();
            this.pnlIcon = new System.Windows.Forms.TableLayoutPanel();
            this.ckShowIPA = new System.Windows.Forms.CheckBox();
            this.ckShowOverlay = new System.Windows.Forms.CheckBox();
            this.ckShowAppxIcon = new System.Windows.Forms.CheckBox();
            this.ckStretchThumbnail = new System.Windows.Forms.CheckBox();
            this.ckEnableThumbnail = new System.Windows.Forms.CheckBox();
            this.btnClearCache = new System.Windows.Forms.Button();
            this.pnlGeneral = new System.Windows.Forms.TableLayoutPanel();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.lblNewVer = new System.Windows.Forms.Label();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.combLanguage = new System.Windows.Forms.ComboBox();
            this.lblHelpTranslate = new System.Windows.Forms.LinkLabel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.ckAlwaysShowStore = new System.Windows.Forms.CheckBox();
            this.ckShowMenuIcon = new System.Windows.Forms.CheckBox();
            this.pnlRight = new System.Windows.Forms.TableLayoutPanel();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnResetInfoTipPattern = new System.Windows.Forms.Button();
            this.pnlContextMenu = new System.Windows.Forms.TableLayoutPanel();
            this.ckShowGoogle = new System.Windows.Forms.CheckBox();
            this.ckShowAmazon = new System.Windows.Forms.CheckBox();
            this.ckShowMS = new System.Windows.Forms.CheckBox();
            this.ckShowApple = new System.Windows.Forms.CheckBox();
            this.ckShowAM = new System.Windows.Forms.CheckBox();
            this.ckShowNewVersionInfo = new System.Windows.Forms.CheckBox();
            this.pnlInfoTip = new System.Windows.Forms.TableLayoutPanel();
            this.lblInfoTipPattern = new System.Windows.Forms.Label();
            this.txtToolTipPattern = new System.Windows.Forms.TextBox();
            this.llbInfoTipPattVar = new System.Windows.Forms.LinkLabel();
            this.pnlMain = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlLeft.SuspendLayout();
            this.pnlRenaming.SuspendLayout();
            this.pnlIcon.SuspendLayout();
            this.pnlGeneral.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlContextMenu.SuspendLayout();
            this.pnlInfoTip.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 0;
            this.toolTip1.AutoPopDelay = 0;
            this.toolTip1.InitialDelay = 0;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 0;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.StripAmpersands = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "ApkShellext2";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Image = global::ApkShellext2.Properties.NonLocalizeResources.logo;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(40, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // pnlLeft
            // 
            this.pnlLeft.AutoSize = true;
            this.pnlLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlLeft.ColumnCount = 1;
            this.pnlLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlLeft.Controls.Add(this.pictureBox1, 0, 0);
            this.pnlLeft.Controls.Add(this.twLeft, 0, 1);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeft.Location = new System.Drawing.Point(3, 3);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.RowCount = 2;
            this.pnlLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlLeft.Size = new System.Drawing.Size(144, 429);
            this.pnlLeft.TabIndex = 16;
            // 
            // twLeft
            // 
            this.twLeft.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.twLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.twLeft.HideSelection = false;
            this.twLeft.Location = new System.Drawing.Point(3, 73);
            this.twLeft.Name = "twLeft";
            this.twLeft.Scrollable = false;
            this.twLeft.ShowPlusMinus = false;
            this.twLeft.Size = new System.Drawing.Size(138, 353);
            this.twLeft.TabIndex = 7;
            this.twLeft.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.twLeft_AfterSelect);
            // 
            // lblRenamePattern
            // 
            this.lblRenamePattern.AutoSize = true;
            this.lblRenamePattern.Location = new System.Drawing.Point(3, 0);
            this.lblRenamePattern.Name = "lblRenamePattern";
            this.lblRenamePattern.Size = new System.Drawing.Size(87, 13);
            this.lblRenamePattern.TabIndex = 11;
            this.lblRenamePattern.Text = "Rename Pattern:";
            // 
            // pnlRenaming
            // 
            this.pnlRenaming.AutoSize = true;
            this.pnlRenaming.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlRenaming.ColumnCount = 1;
            this.pnlRenaming.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlRenaming.Controls.Add(this.txtRenamePattern, 0, 1);
            this.pnlRenaming.Controls.Add(this.lblRenamePattern, 0, 0);
            this.pnlRenaming.Controls.Add(this.ckReplaceSpace, 0, 3);
            this.pnlRenaming.Controls.Add(this.llbPatternVariables, 0, 4);
            this.pnlRenaming.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRenaming.Location = new System.Drawing.Point(3, 3);
            this.pnlRenaming.Name = "pnlRenaming";
            this.pnlRenaming.RowCount = 5;
            this.pnlRenaming.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRenaming.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRenaming.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRenaming.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRenaming.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRenaming.Size = new System.Drawing.Size(347, 75);
            this.pnlRenaming.TabIndex = 21;
            this.pnlRenaming.Paint += new System.Windows.Forms.PaintEventHandler(this.PnlRenaming_Paint);
            // 
            // txtRenamePattern
            // 
            this.txtRenamePattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRenamePattern.Location = new System.Drawing.Point(3, 16);
            this.txtRenamePattern.Name = "txtRenamePattern";
            this.txtRenamePattern.Size = new System.Drawing.Size(341, 20);
            this.txtRenamePattern.TabIndex = 15;
            this.txtRenamePattern.Text = "%label%_%versionName%_%versionCode%";
            this.txtRenamePattern.TextChanged += new System.EventHandler(this.txtRename_TextChanged);
            // 
            // ckReplaceSpace
            // 
            this.ckReplaceSpace.AutoSize = true;
            this.ckReplaceSpace.Location = new System.Drawing.Point(3, 42);
            this.ckReplaceSpace.Name = "ckReplaceSpace";
            this.ckReplaceSpace.Size = new System.Drawing.Size(109, 17);
            this.ckReplaceSpace.TabIndex = 20;
            this.ckReplaceSpace.Text = "ckReplaceSpace";
            this.ckReplaceSpace.UseVisualStyleBackColor = true;
            this.ckReplaceSpace.CheckedChanged += new System.EventHandler(this.ckReplaceSpace_CheckedChanged);
            // 
            // llbPatternVariables
            // 
            this.llbPatternVariables.AutoSize = true;
            this.llbPatternVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.llbPatternVariables.Location = new System.Drawing.Point(3, 62);
            this.llbPatternVariables.Name = "llbPatternVariables";
            this.llbPatternVariables.Size = new System.Drawing.Size(341, 13);
            this.llbPatternVariables.TabIndex = 21;
            this.llbPatternVariables.TabStop = true;
            this.llbPatternVariables.Text = "Refer to wiki";
            this.llbPatternVariables.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbPatternVariables_LinkClicked);
            // 
            // btnResetRenamePattern
            // 
            this.btnResetRenamePattern.AutoSize = true;
            this.btnResetRenamePattern.Location = new System.Drawing.Point(79, 3);
            this.btnResetRenamePattern.Name = "btnResetRenamePattern";
            this.btnResetRenamePattern.Size = new System.Drawing.Size(119, 23);
            this.btnResetRenamePattern.TabIndex = 17;
            this.btnResetRenamePattern.Text = "ResetRenamePattern";
            this.btnResetRenamePattern.Click += new System.EventHandler(this.btnResetRenamePattern_Click);
            // 
            // pnlIcon
            // 
            this.pnlIcon.AutoSize = true;
            this.pnlIcon.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlIcon.ColumnCount = 1;
            this.pnlIcon.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlIcon.Controls.Add(this.ckShowIPA, 0, 0);
            this.pnlIcon.Controls.Add(this.ckShowOverlay, 0, 2);
            this.pnlIcon.Controls.Add(this.ckShowAppxIcon, 0, 1);
            this.pnlIcon.Controls.Add(this.ckStretchThumbnail, 0, 4);
            this.pnlIcon.Controls.Add(this.ckEnableThumbnail, 0, 3);
            this.pnlIcon.Controls.Add(this.btnClearCache, 0, 5);
            this.pnlIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlIcon.Location = new System.Drawing.Point(3, 163);
            this.pnlIcon.Name = "pnlIcon";
            this.pnlIcon.RowCount = 6;
            this.pnlIcon.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlIcon.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlIcon.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlIcon.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlIcon.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlIcon.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlIcon.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlIcon.Size = new System.Drawing.Size(347, 144);
            this.pnlIcon.TabIndex = 22;
            // 
            // ckShowIPA
            // 
            this.ckShowIPA.AutoSize = true;
            this.ckShowIPA.Location = new System.Drawing.Point(3, 3);
            this.ckShowIPA.Name = "ckShowIPA";
            this.ckShowIPA.Size = new System.Drawing.Size(97, 17);
            this.ckShowIPA.TabIndex = 10;
            this.ckShowIPA.Text = "Icon--ShowIPA";
            this.ckShowIPA.UseVisualStyleBackColor = true;
            this.ckShowIPA.CheckedChanged += new System.EventHandler(this.ckShowIPA_CheckedChanged);
            // 
            // ckShowOverlay
            // 
            this.ckShowOverlay.AutoSize = true;
            this.ckShowOverlay.Location = new System.Drawing.Point(3, 49);
            this.ckShowOverlay.Name = "ckShowOverlay";
            this.ckShowOverlay.Size = new System.Drawing.Size(101, 17);
            this.ckShowOverlay.TabIndex = 18;
            this.ckShowOverlay.Text = "ckShowOverlay";
            this.ckShowOverlay.UseVisualStyleBackColor = true;
            this.ckShowOverlay.CheckedChanged += new System.EventHandler(this.ckShowOverlay_CheckedChanged);
            // 
            // ckShowAppxIcon
            // 
            this.ckShowAppxIcon.AutoSize = true;
            this.ckShowAppxIcon.Location = new System.Drawing.Point(3, 26);
            this.ckShowAppxIcon.Name = "ckShowAppxIcon";
            this.ckShowAppxIcon.Size = new System.Drawing.Size(77, 17);
            this.ckShowAppxIcon.TabIndex = 20;
            this.ckShowAppxIcon.Text = "ShowAppx";
            this.ckShowAppxIcon.UseVisualStyleBackColor = true;
            this.ckShowAppxIcon.CheckedChanged += new System.EventHandler(this.ckShowAppxIcon_CheckedChanged);
            // 
            // ckStretchThumbnail
            // 
            this.ckStretchThumbnail.AutoSize = true;
            this.ckStretchThumbnail.Location = new System.Drawing.Point(3, 95);
            this.ckStretchThumbnail.Name = "ckStretchThumbnail";
            this.ckStretchThumbnail.Size = new System.Drawing.Size(121, 17);
            this.ckStretchThumbnail.TabIndex = 21;
            this.ckStretchThumbnail.Text = "ckStretchThumbnail";
            this.ckStretchThumbnail.UseVisualStyleBackColor = true;
            this.ckStretchThumbnail.CheckedChanged += new System.EventHandler(this.ckStretchIcon_CheckedChanged);
            // 
            // ckEnableThumbnail
            // 
            this.ckEnableThumbnail.AutoSize = true;
            this.ckEnableThumbnail.Location = new System.Drawing.Point(3, 72);
            this.ckEnableThumbnail.Name = "ckEnableThumbnail";
            this.ckEnableThumbnail.Size = new System.Drawing.Size(120, 17);
            this.ckEnableThumbnail.TabIndex = 22;
            this.ckEnableThumbnail.Text = "ckEnableThumbnail";
            this.ckEnableThumbnail.UseVisualStyleBackColor = true;
            this.ckEnableThumbnail.CheckedChanged += new System.EventHandler(this.ckEnableThumbnail_CheckedChanged);
            // 
            // btnClearCache
            // 
            this.btnClearCache.AutoSize = true;
            this.btnClearCache.Location = new System.Drawing.Point(3, 118);
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(120, 23);
            this.btnClearCache.TabIndex = 23;
            this.btnClearCache.Text = "btnClearCache";
            this.btnClearCache.UseVisualStyleBackColor = true;
            this.btnClearCache.Click += new System.EventHandler(this.btnClearCache_Click);
            // 
            // pnlGeneral
            // 
            this.pnlGeneral.AutoSize = true;
            this.pnlGeneral.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlGeneral.ColumnCount = 1;
            this.pnlGeneral.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlGeneral.Controls.Add(this.lblCurrentVersion, 0, 4);
            this.pnlGeneral.Controls.Add(this.lblNewVer, 0, 3);
            this.pnlGeneral.Controls.Add(this.lblLanguage, 0, 0);
            this.pnlGeneral.Controls.Add(this.combLanguage, 0, 1);
            this.pnlGeneral.Controls.Add(this.lblHelpTranslate, 0, 2);
            this.pnlGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGeneral.Location = new System.Drawing.Point(0, 81);
            this.pnlGeneral.Margin = new System.Windows.Forms.Padding(0);
            this.pnlGeneral.Name = "pnlGeneral";
            this.pnlGeneral.RowCount = 5;
            this.pnlGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlGeneral.Size = new System.Drawing.Size(353, 79);
            this.pnlGeneral.TabIndex = 21;
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCurrentVersion.Location = new System.Drawing.Point(3, 66);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(347, 13);
            this.lblCurrentVersion.TabIndex = 11;
            this.lblCurrentVersion.Text = "Current version";
            this.lblCurrentVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNewVer
            // 
            this.lblNewVer.AutoSize = true;
            this.lblNewVer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNewVer.Location = new System.Drawing.Point(3, 53);
            this.lblNewVer.Name = "lblNewVer";
            this.lblNewVer.Size = new System.Drawing.Size(347, 13);
            this.lblNewVer.TabIndex = 10;
            this.lblNewVer.Text = "About--NewVersion";
            this.lblNewVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(3, 0);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(101, 13);
            this.lblLanguage.TabIndex = 9;
            this.lblLanguage.Text = "General--Language:";
            // 
            // combLanguage
            // 
            this.combLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.combLanguage.FormattingEnabled = true;
            this.combLanguage.Location = new System.Drawing.Point(3, 16);
            this.combLanguage.Name = "combLanguage";
            this.combLanguage.Size = new System.Drawing.Size(347, 21);
            this.combLanguage.TabIndex = 8;
            this.combLanguage.SelectedIndexChanged += new System.EventHandler(this.combLanguage_SelectedIndexChanged);
            // 
            // lblHelpTranslate
            // 
            this.lblHelpTranslate.AutoSize = true;
            this.lblHelpTranslate.Location = new System.Drawing.Point(3, 40);
            this.lblHelpTranslate.Name = "lblHelpTranslate";
            this.lblHelpTranslate.Size = new System.Drawing.Size(73, 13);
            this.lblHelpTranslate.TabIndex = 12;
            this.lblHelpTranslate.TabStop = true;
            this.lblHelpTranslate.Text = "HelpTranslate";
            this.lblHelpTranslate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblHelpTranslate_LinkClicked);
            // 
            // btnUpdate
            // 
            this.btnUpdate.AutoSize = true;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(204, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(67, 25);
            this.btnUpdate.TabIndex = 24;
            this.btnUpdate.Text = "update";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click_1);
            // 
            // ckAlwaysShowStore
            // 
            this.ckAlwaysShowStore.AutoSize = true;
            this.ckAlwaysShowStore.Location = new System.Drawing.Point(3, 49);
            this.ckAlwaysShowStore.Name = "ckAlwaysShowStore";
            this.ckAlwaysShowStore.Size = new System.Drawing.Size(115, 17);
            this.ckAlwaysShowStore.TabIndex = 17;
            this.ckAlwaysShowStore.Text = "alwaysShowStores";
            this.ckAlwaysShowStore.UseVisualStyleBackColor = true;
            this.ckAlwaysShowStore.CheckedChanged += new System.EventHandler(this.ckShowPlay_CheckedChanged);
            // 
            // ckShowMenuIcon
            // 
            this.ckShowMenuIcon.AutoSize = true;
            this.ckShowMenuIcon.Location = new System.Drawing.Point(3, 3);
            this.ckShowMenuIcon.Name = "ckShowMenuIcon";
            this.ckShowMenuIcon.Size = new System.Drawing.Size(151, 17);
            this.ckShowMenuIcon.TabIndex = 21;
            this.ckShowMenuIcon.Text = "ConMenu--showMenuIcon";
            this.ckShowMenuIcon.UseVisualStyleBackColor = true;
            this.ckShowMenuIcon.CheckedChanged += new System.EventHandler(this.ckShowMenuIcon_CheckedChanged);
            // 
            // pnlRight
            // 
            this.pnlRight.AutoSize = true;
            this.pnlRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlRight.ColumnCount = 1;
            this.pnlRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlRight.Controls.Add(this.pnlButtons, 0, 7);
            this.pnlRight.Controls.Add(this.pnlIcon, 0, 3);
            this.pnlRight.Controls.Add(this.pnlGeneral, 0, 2);
            this.pnlRight.Controls.Add(this.pnlRenaming, 0, 0);
            this.pnlRight.Controls.Add(this.pnlContextMenu, 0, 4);
            this.pnlRight.Controls.Add(this.pnlInfoTip, 0, 6);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(153, 3);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.RowCount = 8;
            this.pnlRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlRight.Size = new System.Drawing.Size(353, 429);
            this.pnlRight.TabIndex = 24;
            this.pnlRight.Visible = false;
            // 
            // pnlButtons
            // 
            this.pnlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlButtons.Controls.Add(this.btnOK);
            this.pnlButtons.Controls.Add(this.btnUpdate);
            this.pnlButtons.Controls.Add(this.btnResetRenamePattern);
            this.pnlButtons.Controls.Add(this.btnResetInfoTipPattern);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlButtons.Location = new System.Drawing.Point(3, 645);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(347, 31);
            this.pnlButtons.TabIndex = 21;
            // 
            // btnOK
            // 
            this.btnOK.AutoSize = true;
            this.btnOK.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOK.Location = new System.Drawing.Point(277, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(67, 25);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnResetInfoTipPattern
            // 
            this.btnResetInfoTipPattern.AutoSize = true;
            this.btnResetInfoTipPattern.Location = new System.Drawing.Point(229, 34);
            this.btnResetInfoTipPattern.Name = "btnResetInfoTipPattern";
            this.btnResetInfoTipPattern.Size = new System.Drawing.Size(115, 23);
            this.btnResetInfoTipPattern.TabIndex = 2;
            this.btnResetInfoTipPattern.Text = "ResetInfoTipPattern";
            this.btnResetInfoTipPattern.UseVisualStyleBackColor = true;
            this.btnResetInfoTipPattern.Click += new System.EventHandler(this.btnResetTooltipPattern_Click);
            // 
            // pnlContextMenu
            // 
            this.pnlContextMenu.AutoSize = true;
            this.pnlContextMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlContextMenu.ColumnCount = 1;
            this.pnlContextMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlContextMenu.Controls.Add(this.ckShowMenuIcon, 0, 0);
            this.pnlContextMenu.Controls.Add(this.ckAlwaysShowStore, 0, 2);
            this.pnlContextMenu.Controls.Add(this.ckShowGoogle, 0, 3);
            this.pnlContextMenu.Controls.Add(this.ckShowAmazon, 0, 4);
            this.pnlContextMenu.Controls.Add(this.ckShowMS, 0, 6);
            this.pnlContextMenu.Controls.Add(this.ckShowApple, 0, 5);
            this.pnlContextMenu.Controls.Add(this.ckShowAM, 0, 7);
            this.pnlContextMenu.Controls.Add(this.ckShowNewVersionInfo, 0, 1);
            this.pnlContextMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContextMenu.Location = new System.Drawing.Point(3, 313);
            this.pnlContextMenu.Name = "pnlContextMenu";
            this.pnlContextMenu.RowCount = 9;
            this.pnlContextMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlContextMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlContextMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlContextMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlContextMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlContextMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlContextMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlContextMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlContextMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlContextMenu.Size = new System.Drawing.Size(347, 204);
            this.pnlContextMenu.TabIndex = 24;
            // 
            // ckShowGoogle
            // 
            this.ckShowGoogle.AutoSize = true;
            this.ckShowGoogle.Location = new System.Drawing.Point(3, 72);
            this.ckShowGoogle.Name = "ckShowGoogle";
            this.ckShowGoogle.Size = new System.Drawing.Size(104, 17);
            this.ckShowGoogle.TabIndex = 22;
            this.ckShowGoogle.Text = "showGoogleplay";
            this.ckShowGoogle.UseVisualStyleBackColor = true;
            this.ckShowGoogle.CheckedChanged += new System.EventHandler(this.ckbShowGoogle_CheckedChanged);
            // 
            // ckShowAmazon
            // 
            this.ckShowAmazon.AutoSize = true;
            this.ckShowAmazon.Location = new System.Drawing.Point(3, 95);
            this.ckShowAmazon.Name = "ckShowAmazon";
            this.ckShowAmazon.Size = new System.Drawing.Size(63, 17);
            this.ckShowAmazon.TabIndex = 23;
            this.ckShowAmazon.Text = "amazon";
            this.ckShowAmazon.UseVisualStyleBackColor = true;
            this.ckShowAmazon.CheckedChanged += new System.EventHandler(this.ckShowAmazon_CheckedChanged);
            // 
            // ckShowMS
            // 
            this.ckShowMS.AutoSize = true;
            this.ckShowMS.Location = new System.Drawing.Point(3, 141);
            this.ckShowMS.Name = "ckShowMS";
            this.ckShowMS.Size = new System.Drawing.Size(39, 17);
            this.ckShowMS.TabIndex = 24;
            this.ckShowMS.Text = "ms";
            this.ckShowMS.UseVisualStyleBackColor = true;
            this.ckShowMS.CheckedChanged += new System.EventHandler(this.ckShowMS_CheckedChanged);
            // 
            // ckShowApple
            // 
            this.ckShowApple.AutoSize = true;
            this.ckShowApple.Location = new System.Drawing.Point(3, 118);
            this.ckShowApple.Name = "ckShowApple";
            this.ckShowApple.Size = new System.Drawing.Size(52, 17);
            this.ckShowApple.TabIndex = 25;
            this.ckShowApple.Text = "apple";
            this.ckShowApple.UseVisualStyleBackColor = true;
            this.ckShowApple.CheckedChanged += new System.EventHandler(this.ckShowApple_CheckedChanged);
            // 
            // ckShowAM
            // 
            this.ckShowAM.AutoSize = true;
            this.ckShowAM.Location = new System.Drawing.Point(3, 164);
            this.ckShowAM.Name = "ckShowAM";
            this.ckShowAM.Size = new System.Drawing.Size(96, 17);
            this.ckShowAM.TabIndex = 26;
            this.ckShowAM.Text = "showApkMirror";
            this.ckShowAM.UseVisualStyleBackColor = true;
            this.ckShowAM.Visible = false;
            this.ckShowAM.CheckedChanged += new System.EventHandler(this.ckShowAM_CheckedChanged);
            // 
            // ckShowNewVersionInfo
            // 
            this.ckShowNewVersionInfo.AutoSize = true;
            this.ckShowNewVersionInfo.Location = new System.Drawing.Point(3, 26);
            this.ckShowNewVersionInfo.Name = "ckShowNewVersionInfo";
            this.ckShowNewVersionInfo.Size = new System.Drawing.Size(128, 17);
            this.ckShowNewVersionInfo.TabIndex = 27;
            this.ckShowNewVersionInfo.Text = "ShowNewVersionInfo";
            this.ckShowNewVersionInfo.UseVisualStyleBackColor = true;
            this.ckShowNewVersionInfo.CheckedChanged += new System.EventHandler(this.CkShowNewVersionInfo_CheckedChanged);
            // 
            // pnlInfoTip
            // 
            this.pnlInfoTip.AutoSize = true;
            this.pnlInfoTip.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlInfoTip.ColumnCount = 1;
            this.pnlInfoTip.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlInfoTip.Controls.Add(this.lblInfoTipPattern, 0, 0);
            this.pnlInfoTip.Controls.Add(this.txtToolTipPattern, 0, 1);
            this.pnlInfoTip.Controls.Add(this.llbInfoTipPattVar, 0, 7);
            this.pnlInfoTip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInfoTip.Location = new System.Drawing.Point(3, 523);
            this.pnlInfoTip.Name = "pnlInfoTip";
            this.pnlInfoTip.RowCount = 8;
            this.pnlInfoTip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlInfoTip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlInfoTip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlInfoTip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlInfoTip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlInfoTip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlInfoTip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlInfoTip.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlInfoTip.Size = new System.Drawing.Size(347, 116);
            this.pnlInfoTip.TabIndex = 25;
            // 
            // lblInfoTipPattern
            // 
            this.lblInfoTipPattern.AutoSize = true;
            this.lblInfoTipPattern.Location = new System.Drawing.Point(3, 0);
            this.lblInfoTipPattern.Name = "lblInfoTipPattern";
            this.lblInfoTipPattern.Size = new System.Drawing.Size(104, 13);
            this.lblInfoTipPattern.TabIndex = 0;
            this.lblInfoTipPattern.Text = "Info Tip Info Pattern:";
            // 
            // txtToolTipPattern
            // 
            this.txtToolTipPattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtToolTipPattern.Location = new System.Drawing.Point(3, 16);
            this.txtToolTipPattern.Multiline = true;
            this.txtToolTipPattern.Name = "txtToolTipPattern";
            this.txtToolTipPattern.Size = new System.Drawing.Size(341, 84);
            this.txtToolTipPattern.TabIndex = 1;
            this.txtToolTipPattern.TextChanged += new System.EventHandler(this.txtToolTipPattern_TextChanged);
            // 
            // llbInfoTipPattVar
            // 
            this.llbInfoTipPattVar.AutoSize = true;
            this.llbInfoTipPattVar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.llbInfoTipPattVar.Location = new System.Drawing.Point(3, 103);
            this.llbInfoTipPattVar.Name = "llbInfoTipPattVar";
            this.llbInfoTipPattVar.Size = new System.Drawing.Size(341, 13);
            this.llbInfoTipPattVar.TabIndex = 3;
            this.llbInfoTipPattVar.TabStop = true;
            this.llbInfoTipPattVar.Text = "refer to wiki";
            this.llbInfoTipPattVar.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbInfoTipPattVar_LinkClicked);
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSize = true;
            this.pnlMain.ColumnCount = 2;
            this.pnlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.pnlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlMain.Controls.Add(this.pnlLeft, 0, 0);
            this.pnlMain.Controls.Add(this.pnlRight, 2, 0);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.RowCount = 1;
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlMain.Size = new System.Drawing.Size(509, 435);
            this.pnlMain.TabIndex = 25;
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(509, 435);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Preferences";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Preferences_FormClosed);
            this.Load += new System.EventHandler(this.Preferences_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            this.pnlRenaming.ResumeLayout(false);
            this.pnlRenaming.PerformLayout();
            this.pnlIcon.ResumeLayout(false);
            this.pnlIcon.PerformLayout();
            this.pnlGeneral.ResumeLayout(false);
            this.pnlGeneral.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.pnlContextMenu.ResumeLayout(false);
            this.pnlContextMenu.PerformLayout();
            this.pnlInfoTip.ResumeLayout(false);
            this.pnlInfoTip.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel pnlLeft;
        private System.Windows.Forms.TreeView twLeft;
        private System.Windows.Forms.Label lblRenamePattern;
        private System.Windows.Forms.TableLayoutPanel pnlRenaming;
        private System.Windows.Forms.TableLayoutPanel pnlIcon;
        private System.Windows.Forms.TableLayoutPanel pnlGeneral;
        private System.Windows.Forms.CheckBox ckShowIPA;
        private System.Windows.Forms.CheckBox ckShowOverlay;
        private System.Windows.Forms.CheckBox ckAlwaysShowStore;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox combLanguage;
        private System.Windows.Forms.CheckBox ckShowAppxIcon;
        private System.Windows.Forms.CheckBox ckShowMenuIcon;
        private System.Windows.Forms.TableLayoutPanel pnlRight;
        private System.Windows.Forms.FlowLayoutPanel pnlButtons;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TableLayoutPanel pnlContextMenu;
        private System.Windows.Forms.TableLayoutPanel pnlInfoTip;
        private System.Windows.Forms.TableLayoutPanel pnlMain;
        private System.Windows.Forms.CheckBox ckShowGoogle;
        private System.Windows.Forms.TextBox txtRenamePattern;
        private System.Windows.Forms.CheckBox ckShowAmazon;
        private System.Windows.Forms.CheckBox ckShowMS;
        private System.Windows.Forms.CheckBox ckShowApple;
        private System.Windows.Forms.Button btnResetRenamePattern;
        private System.Windows.Forms.Label lblInfoTipPattern;
        private System.Windows.Forms.TextBox txtToolTipPattern;
        private System.Windows.Forms.Button btnResetInfoTipPattern;
        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.Label lblNewVer;
        private System.Windows.Forms.CheckBox ckShowAM;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.CheckBox ckReplaceSpace;
        private System.Windows.Forms.LinkLabel llbPatternVariables;
        private System.Windows.Forms.LinkLabel llbInfoTipPattVar;
        private System.Windows.Forms.CheckBox ckShowNewVersionInfo;
        private System.Windows.Forms.CheckBox ckStretchThumbnail;
        private System.Windows.Forms.LinkLabel lblHelpTranslate;
        private System.Windows.Forms.CheckBox ckEnableThumbnail;
        private System.Windows.Forms.Button btnClearCache;
    }
}