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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LocalVideoFolderTextBox = new System.Windows.Forms.TextBox();
            this.LocalPathToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.CasparCGHostnameTextBox = new System.Windows.Forms.TextBox();
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
            this.label2.Location = new System.Drawing.Point(31, 238);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Media Full Path";
            this.RemoteAccessibleToolTip.SetToolTip(this.label2, "e.g. \\\\CasparCGServer\\CasparCG\\media\r\nor\r\nC:\\CasparCG\\server\\media if running loc" +
        "ally");
            // 
            // CasparCGNetworkVideoFolderTextBox
            // 
            this.CasparCGNetworkVideoFolderTextBox.Location = new System.Drawing.Point(117, 235);
            this.CasparCGNetworkVideoFolderTextBox.Name = "CasparCGNetworkVideoFolderTextBox";
            this.CasparCGNetworkVideoFolderTextBox.Size = new System.Drawing.Size(197, 20);
            this.CasparCGNetworkVideoFolderTextBox.TabIndex = 4;
            this.RemoteAccessibleToolTip.SetToolTip(this.CasparCGNetworkVideoFolderTextBox, "e.g. \\\\CasparCGServer\\CasparCG\\media\r\nor\r\nC:\\CasparCG\\server\\media if running loc" +
        "ally");
            // 
            // CasparCGNetworkVideoFolderBrowseButton
            // 
            this.CasparCGNetworkVideoFolderBrowseButton.Location = new System.Drawing.Point(320, 233);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Local Paths:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Media Full Path";
            this.LocalPathToolTip.SetToolTip(this.label4, "e.g. C:\\CasparCG\\Server\\media");
            // 
            // LocalVideoFolderTextBox
            // 
            this.LocalVideoFolderTextBox.Location = new System.Drawing.Point(117, 176);
            this.LocalVideoFolderTextBox.Name = "LocalVideoFolderTextBox";
            this.LocalVideoFolderTextBox.Size = new System.Drawing.Size(223, 20);
            this.LocalVideoFolderTextBox.TabIndex = 3;
            this.LocalPathToolTip.SetToolTip(this.LocalVideoFolderTextBox, "e.g. C:\\CasparCG\\Server\\media");
            // 
            // LocalPathToolTip
            // 
            this.LocalPathToolTip.ToolTipTitle = "Local paths are relative to the machine running CasparCG";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 216);
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
            // Settings
            // 
            this.AcceptButton = this.SettingFormSaveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.SettingsFormCancelButton;
            this.ClientSize = new System.Drawing.Size(560, 378);
            this.Controls.Add(this.CasparCGHostnameTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.LocalVideoFolderTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox LocalVideoFolderTextBox;
        private System.Windows.Forms.ToolTip LocalPathToolTip;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox CasparCGHostnameTextBox;
    }
}