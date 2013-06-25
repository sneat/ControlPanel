using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Overlay
{
    public partial class PlaylistForm : Form
    {
        private Dictionary<string, VideoFile> ParsedVideoFiles;
        private Form1 form1;
        private DragNDrop.DragAndDropListView Form1PlaylistDragAndDropListView;

        public PlaylistForm()
        {
            InitializeComponent();
        }

        public PlaylistForm(Form1 form1, Dictionary<string, VideoFile> ParsedVideoFiles, DragNDrop.DragAndDropListView Form1PlaylistDragAndDropListView)
        {
            InitializeComponent();
            this.form1 = form1;
            this.ParsedVideoFiles = ParsedVideoFiles;
            this.Form1PlaylistDragAndDropListView = Form1PlaylistDragAndDropListView;
            foreach (KeyValuePair<string, VideoFile> entry in ParsedVideoFiles)
            {
                PlaylistDragAndDropListView.Items.Add(new ListViewItem(new string[2] { entry.Value.CasparPath, entry.Value.DurationString }));
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.AutoValidate = AutoValidate.Disable;
                CancelButton.PerformClick();
                this.AutoValidate = AutoValidate.Inherit;
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (DescriptionTextBox.Text == "")
            {
                MessageBox.Show("Please enter a description", "Invalid description", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (PlaylistDragAndDropListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one video", "Invalid selections", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                List<KeyValuePair<string, long>> SelectedItems = new List<KeyValuePair<string, long>>();
                long TotalDuration = 0;
                foreach (ListViewItem item in PlaylistDragAndDropListView.SelectedItems)
                {
                    VideoFile VideoFile = ParsedVideoFiles[item.Text];
                    Console.WriteLine("File {0}, duration {1}", VideoFile.NetworkPath, VideoFile.DurationString);
                    SelectedItems.Add(new KeyValuePair<string, long>(VideoFile.CasparPath, VideoFile.Duration));
                    TotalDuration += VideoFile.Duration;
                }
                Playlist _playlist = new Playlist(DescriptionTextBox.Text, TotalDuration, SelectedItems);
                form1.Playlists.Add(DescriptionTextBox.Text, _playlist);
                Form1PlaylistDragAndDropListView.Items.Clear();
                foreach (KeyValuePair<string, Playlist> playlist in form1.Playlists)
                {
                    Form1PlaylistDragAndDropListView.Items.Add(new ListViewItem(new string[2] { playlist.Value.Name, playlist.Value.DurationString }));
                }

                this.Close();
            }
        }

        private void PlaylistDragAndDropListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            long TotalDuration = 0;
            foreach (ListViewItem item in PlaylistDragAndDropListView.SelectedItems)
            {
                VideoFile VideoFile = ParsedVideoFiles[item.Text];
                TotalDuration += VideoFile.Duration;
            }
            TotalDurationLabel.Text = "Total Duration: " + ConvertMilisecondsToString(TotalDuration);
        }

        public static string ConvertMilisecondsToString(long time)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(time);
            String[] parts = string
                .Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}",
                    ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds)
                .Split(':')
                .SkipWhile(s => Regex.Match(s, @"00").Success) // skip zero-valued components
                .ToArray();
            if (parts.Count() > 1)
            {
                return string.Join(":", parts);
            }
            else if (parts.Count() == 1)
            {
                return "00:" + parts[0].ToString();
            }
            else
            {
                return "00:00";
            }
        }
    }
}
