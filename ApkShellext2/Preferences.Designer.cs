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
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblNewVer = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.combLanguage = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblRenamePattern = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.ckRename = new System.Windows.Forms.CheckBox();
            this.ckShowPlay = new System.Windows.Forms.CheckBox();
            this.ckShowOverlay = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.ckShowAppxIcon = new System.Windows.Forms.CheckBox();
            this.ckShowMenuIcon = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(163, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 25);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "OK";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblNewVer
            // 
            this.lblNewVer.AutoSize = true;
            this.lblNewVer.Location = new System.Drawing.Point(3, 74);
            this.lblNewVer.Name = "lblNewVer";
            this.lblNewVer.Size = new System.Drawing.Size(64, 13);
            this.lblNewVer.TabIndex = 0;
            this.lblNewVer.Text = "NewVersion";
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
            this.pictureBox1.Image = global::ApkShellext2.Properties.Resources.logo;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(55, 55);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(3, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current version";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblNewVer, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(101, 287);
            this.tableLayoutPanel2.TabIndex = 16;
            // 
            // combLanguage
            // 
            this.combLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.combLanguage.FormattingEnabled = true;
            this.combLanguage.Location = new System.Drawing.Point(3, 154);
            this.combLanguage.Name = "combLanguage";
            this.combLanguage.Size = new System.Drawing.Size(233, 21);
            this.combLanguage.TabIndex = 8;
            this.combLanguage.SelectedIndexChanged += new System.EventHandler(this.combLanguage_SelectedIndexChanged);
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(3, 138);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(58, 13);
            this.lblLanguage.TabIndex = 9;
            this.lblLanguage.Text = "Language:";
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 233);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(233, 20);
            this.textBox1.TabIndex = 12;
            this.textBox1.Text = "%label%_%versionName%_%versionCode%.apk";
            this.textBox1.Visible = false;
            // 
            // lblRenamePattern
            // 
            this.lblRenamePattern.AutoSize = true;
            this.lblRenamePattern.Location = new System.Drawing.Point(3, 178);
            this.lblRenamePattern.Name = "lblRenamePattern";
            this.lblRenamePattern.Size = new System.Drawing.Size(87, 13);
            this.lblRenamePattern.TabIndex = 11;
            this.lblRenamePattern.Text = "Rename Pattern:";
            this.lblRenamePattern.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 39);
            this.label3.TabIndex = 14;
            this.label3.Text = "%label%: Label\r\n%versionName%: VersionName\r\n%versionCode%: VersionCode\r\n";
            this.label3.Visible = false;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(3, 72);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(70, 17);
            this.checkBox4.TabIndex = 10;
            this.checkBox4.Text = "ShowIPA";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // ckRename
            // 
            this.ckRename.AutoSize = true;
            this.ckRename.Location = new System.Drawing.Point(3, 3);
            this.ckRename.Name = "ckRename";
            this.ckRename.Size = new System.Drawing.Size(124, 17);
            this.ckRename.TabIndex = 1;
            this.ckRename.Text = "renameWithVerCode";
            this.ckRename.UseVisualStyleBackColor = true;
            this.ckRename.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ckShowPlay
            // 
            this.ckShowPlay.AutoSize = true;
            this.ckShowPlay.Location = new System.Drawing.Point(3, 26);
            this.ckShowPlay.Name = "ckShowPlay";
            this.ckShowPlay.Size = new System.Drawing.Size(85, 17);
            this.ckShowPlay.TabIndex = 17;
            this.ckShowPlay.Text = "ckShowPlay";
            this.ckShowPlay.UseVisualStyleBackColor = true;
            this.ckShowPlay.CheckedChanged += new System.EventHandler(this.ckShowPlay_CheckedChanged);
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
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.ckRename, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.checkBox4, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.textBox1, 0, 12);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 11);
            this.tableLayoutPanel3.Controls.Add(this.ckShowOverlay, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.lblRenamePattern, 0, 10);
            this.tableLayoutPanel3.Controls.Add(this.ckShowPlay, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblLanguage, 0, 8);
            this.tableLayoutPanel3.Controls.Add(this.combLanguage, 0, 9);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 13);
            this.tableLayoutPanel3.Controls.Add(this.ckShowAppxIcon, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.ckShowMenuIcon, 0, 6);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(101, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 14;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(239, 287);
            this.tableLayoutPanel3.TabIndex = 19;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnUpdate);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 259);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(233, 31);
            this.flowLayoutPanel1.TabIndex = 19;
            // 
            // btnUpdate
            // 
            this.btnUpdate.AutoSize = true;
            this.btnUpdate.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(90, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(67, 25);
            this.btnUpdate.TabIndex = 21;
            this.btnUpdate.Text = "update";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // ckShowAppxIcon
            // 
            this.ckShowAppxIcon.AutoSize = true;
            this.ckShowAppxIcon.Location = new System.Drawing.Point(3, 95);
            this.ckShowAppxIcon.Name = "ckShowAppxIcon";
            this.ckShowAppxIcon.Size = new System.Drawing.Size(77, 17);
            this.ckShowAppxIcon.TabIndex = 20;
            this.ckShowAppxIcon.Text = "ShowAppx";
            this.ckShowAppxIcon.UseVisualStyleBackColor = true;
            this.ckShowAppxIcon.CheckedChanged += new System.EventHandler(this.ckShowAppxIcon_CheckedChanged);
            // 
            // ckShowMenuIcon
            // 
            this.ckShowMenuIcon.AutoSize = true;
            this.ckShowMenuIcon.Location = new System.Drawing.Point(3, 118);
            this.ckShowMenuIcon.Name = "ckShowMenuIcon";
            this.ckShowMenuIcon.Size = new System.Drawing.Size(99, 17);
            this.ckShowMenuIcon.TabIndex = 21;
            this.ckShowMenuIcon.Text = "showMenuIcon";
            this.ckShowMenuIcon.UseVisualStyleBackColor = true;
            this.ckShowMenuIcon.CheckedChanged += new System.EventHandler(this.ckShowMenuIcon_CheckedChanged);
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(354, 287);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Preferences";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Preferences_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblNewVer;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ComboBox combLanguage;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblRenamePattern;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox ckRename;
        private System.Windows.Forms.CheckBox ckShowPlay;
        private System.Windows.Forms.CheckBox ckShowOverlay;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.CheckBox ckShowAppxIcon;
        private System.Windows.Forms.CheckBox ckShowMenuIcon;
    }
}