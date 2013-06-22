using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Overlay
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            CasparCGHostnameTextBox.Text = Properties.Settings.Default.CasparCGHostname;
            CasparCGPortTextBox.Text = Properties.Settings.Default.CasparCGPort.ToString();
            CasparCGNetworkVideoFolderTextBox.Text = Properties.Settings.Default.NetworkVideoFolder;
            MediaTransitionComboBox.Text = Properties.Settings.Default.MediaTransitionType;
            TransitionDurationTextBox.Text = Properties.Settings.Default.MediaTransitionDuration.ToString();
        }

        private void CasparCGPortTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.Match(CasparCGPortTextBox.Text, @"^\d+$").Success) {
                MessageBox.Show("Please enter a valid port. Default is 5250", "Invalid port", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
            }
        }

        private void SettingFormSaveButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CasparCGHostname = CasparCGHostnameTextBox.Text;
            Properties.Settings.Default.CasparCGPort = Convert.ToInt32(CasparCGPortTextBox.Text);
            Properties.Settings.Default.NetworkVideoFolder = CasparCGNetworkVideoFolderTextBox.Text;
            Properties.Settings.Default.MediaTransitionType = MediaTransitionComboBox.Text;
            Properties.Settings.Default.MediaTransitionDuration = Convert.ToInt32(TransitionDurationTextBox.Text);

            Properties.Settings.Default.Save();

            this.Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.AutoValidate = AutoValidate.Disable;
                SettingsFormCancelButton.PerformClick();
                this.AutoValidate = AutoValidate.Inherit;
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void CasparCGNetworkVideoFolderBrowseButton_Click(object sender, EventArgs e)
        {
            var FolderBrowser = new Ionic.Utils.FolderBrowserDialogEx();
            FolderBrowser.Description = "Please select the CasparCG media folder (e.g. \\\\CasparServer\\CasparCG\\media):";
            FolderBrowser.SelectedPath = CasparCGNetworkVideoFolderTextBox.Text;

            // Show the FolderBrowserDialog.
            if (FolderBrowser.ShowDialog() == DialogResult.OK)
            {
                CasparCGNetworkVideoFolderTextBox.Text = FolderBrowser.SelectedPath;
            }
        }

        private void TransitionDurationTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.Match(TransitionDurationTextBox.Text, @"^\d+$").Success)
            {
                MessageBox.Show("Please enter a valid duration (in milliseconds).", "Invalid duration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
            }
            else if (Convert.ToInt32(TransitionDurationTextBox.Text) < 1)
            {
                TransitionDurationTextBox.Text = "1";
            }
        }
    }
}
