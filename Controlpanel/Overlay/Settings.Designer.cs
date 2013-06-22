namespace Overlay
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.CasparCGPortTextBox = new System.Windows.Forms.TextBox();
            this.SettingFormSaveButton = new System.Windows.Forms.Button();
            this.SettingsFormCancelButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CasparCGNetworkVideoFolderTextBox = new System.Windows.Forms.TextBox();
            this.CasparCGNetworkVideoFolderBrowseButton = new System.Windows.Forms.Button();
            this.RemoteAccessibleToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.LocalPathToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.CasparCGHostnameTextBox = new System.Windows.Forms.TextBox();
            this.MediaTransitionComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.GeneralToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.TransitionDurationTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "CasparCG Port";
            // 
            // CasparCGPortTextBox
            // 
            this.CasparCGPortTextBox.Location = new System.Drawing.Point(92, 121);
            this.CasparCGPortTextBox.Name = "CasparCGPortTextBox";
            this.CasparCGPortTextBox.Size = new System.Drawing.Size(45, 20);
            this.CasparCGPortTextBox.TabIndex = 2;
            this.CasparCGPortTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.CasparCGPortTextBox_Validating);
            // 
            // SettingFormSaveButton
            // 
            this.SettingFormSaveButton.Location = new System.Drawing.Point(473, 343);
            this.SettingFormSaveButton.Name = "SettingFormSaveButton";
            this.SettingFormSaveButton.Size = new System.Drawing.Size(75, 23);
            this.SettingFormSaveButton.TabIndex = 6;
            this.SettingFormSaveButton.Text = "&Save";
            this.SettingFormSaveButton.UseVisualStyleBackColor = true;
            this.SettingFormSaveButton.Click += new System.EventHandler(this.SettingFormSaveButton_Click);
            // 
            // SettingsFormCancelButton
            // 
            this.SettingsFormCancelButton.CausesValidation = false;
            this.SettingsFormCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.SettingsFormCancelButton.Location = new System.Drawing.Point(392, 342);
            this.SettingsFormCancelButton.Name = "SettingsFormCancelButton";
            this.SettingsFormCancelButton.Size = new System.Drawing.Size(75, 23);
            this.SettingsFormCancelButton.TabIndex = 7;
            this.SettingsFormCancelButton.Text = "Cancel";
            this.SettingsFormCancelButton.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Overlay.Properties.Resources.CasparCG_Logo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(302, 77);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Media Full Path";
            this.RemoteAccessibleToolTip.SetToolTip(this.label2, "e.g. \\\\CasparCGServer\\CasparCG\\media\r\nor\r\nC:\\CasparCG\\server\\media if running loc" +
        "ally");
            // 
            // CasparCGNetworkVideoFolderTextBox
            // 
            this.CasparCGNetworkVideoFolderTextBox.Location = new System.Drawing.Point(117, 181);
            this.CasparCGNetworkVideoFolderTextBox.Name = "CasparCGNetworkVideoFolderTextBox";
            this.CasparCGNetworkVideoFolderTextBox.Size = new System.Drawing.Size(197, 20);
            this.CasparCGNetworkVideoFolderTextBox.TabIndex = 4;
            this.RemoteAccessibleToolTip.SetToolTip(this.CasparCGNetworkVideoFolderTextBox, "e.g. \\\\CasparCGServer\\CasparCG\\media\r\nor\r\nC:\\CasparCG\\server\\media if running loc" +
        "ally");
            // 
            // CasparCGNetworkVideoFolderBrowseButton
            // 
            this.CasparCGNetworkVideoFolderBrowseButton.Location = new System.Drawing.Point(320, 179);
            this.CasparCGNetworkVideoFolderBrowseButton.Name = "CasparCGNetworkVideoFolderBrowseButton";
            this.CasparCGNetworkVideoFolderBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.CasparCGNetworkVideoFolderBrowseButton.TabIndex = 5;
            this.CasparCGNetworkVideoFolderBrowseButton.Text = "Browse";
            this.CasparCGNetworkVideoFolderBrowseButton.UseVisualStyleBackColor = true;
            this.CasparCGNetworkVideoFolderBrowseButton.Click += new System.EventHandler(this.CasparCGNetworkVideoFolderBrowseButton_Click);
            // 
            // RemoteAccessibleToolTip
            // 
            this.RemoteAccessibleToolTip.ToolTipTitle = "Remote accessible paths must be accessible via this machine";
            // 
            // LocalPathToolTip
            // 
            this.LocalPathToolTip.ToolTipTitle = "Local paths are relative to the machine running CasparCG";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Remote Accessible Paths:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "CasparCG Hostname";
            // 
            // CasparCGHostnameTextBox
            // 
            this.CasparCGHostnameTextBox.Location = new System.Drawing.Point(121, 96);
            this.CasparCGHostnameTextBox.Name = "CasparCGHostnameTextBox";
            this.CasparCGHostnameTextBox.Size = new System.Drawing.Size(100, 20);
            this.CasparCGHostnameTextBox.TabIndex = 1;
            // 
            // MediaTransitionComboBox
            // 
            this.MediaTransitionComboBox.FormattingEnabled = true;
            this.MediaTransitionComboBox.Items.AddRange(new object[] {
            "CUT",
            "MIX",
            "PUSH",
            "WIPE",
            "SLIDE"});
            this.MediaTransitionComboBox.Location = new System.Drawing.Point(71, 232);
            this.MediaTransitionComboBox.Name = "MediaTransitionComboBox";
            this.MediaTransitionComboBox.Size = new System.Drawing.Size(121, 21);
            this.MediaTransitionComboBox.TabIndex = 13;
            this.GeneralToolTip.SetToolTip(this.MediaTransitionComboBox, "This is the type of transition that will be used.");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 215);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Media Transitions:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(34, 235);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Type";
            this.GeneralToolTip.SetToolTip(this.label8, "This is the type of transition that will be used.");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(34, 263);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Duration";
            this.GeneralToolTip.SetToolTip(this.label9, "The duration that the transition should take. Minimum of 1.\r\nUse 1 if you are usi" +
        "ng CUT.");
            // 
            // TransitionDurationTextBox
            // 
            this.TransitionDurationTextBox.Location = new System.Drawing.Point(87, 259);
            this.TransitionDurationTextBox.Name = "TransitionDurationTextBox";
            this.TransitionDurationTextBox.Size = new System.Drawing.Size(56, 20);
            this.TransitionDurationTextBox.TabIndex = 17;
            this.GeneralToolTip.SetToolTip(this.TransitionDurationTextBox, "The duration that the transition should take. Minimum of 1.\r\nUse 1 if you are usi" +
        "ng CUT.");
            this.TransitionDurationTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TransitionDurationTextBox_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(149, 263);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "ms";
            // 
            // Settings
            // 
            this.AcceptButton = this.SettingFormSaveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.SettingsFormCancelButton;
            this.ClientSize = new System.Drawing.Size(560, 378);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TransitionDurationTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.MediaTransitionComboBox);
            this.Controls.Add(this.CasparCGHostnameTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.CasparCGNetworkVideoFolderBrowseButton);
            this.Controls.Add(this.CasparCGNetworkVideoFolderTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.SettingsFormCancelButton);
            this.Controls.Add(this.SettingFormSaveButton);
            this.Controls.Add(this.CasparCGPortTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Name = "Settings";
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CasparCGPortTextBox;
        private System.Windows.Forms.Button SettingFormSaveButton;
        private System.Windows.Forms.Button SettingsFormCancelButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox CasparCGNetworkVideoFolderTextBox;
        private System.Windows.Forms.Button CasparCGNetworkVideoFolderBrowseButton;
        private System.Windows.Forms.ToolTip RemoteAccessibleToolTip;
        private System.Windows.Forms.ToolTip LocalPathToolTip;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox CasparCGHostnameTextBox;
        private System.Windows.Forms.ComboBox MediaTransitionComboBox;
        private System.Windows.Forms.ToolTip GeneralToolTip;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TransitionDurationTextBox;
        private System.Windows.Forms.Label label3;
    }
}