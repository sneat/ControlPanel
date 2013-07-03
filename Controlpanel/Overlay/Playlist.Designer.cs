namespace Overlay
{
    partial class PlaylistForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.TotalDurationLabel = new System.Windows.Forms.Label();
            this.PlaylistDragAndDropListView = new DragNDrop.DragAndDropListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Playlist Name";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Location = new System.Drawing.Point(15, 26);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(290, 20);
            this.DescriptionTextBox.TabIndex = 1;
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(497, 354);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 3;
            this.OkButton.Text = "Ok";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(416, 354);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 4;
            this.CloseButton.Text = "Cancel";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // TotalDurationLabel
            // 
            this.TotalDurationLabel.AutoSize = true;
            this.TotalDurationLabel.Location = new System.Drawing.Point(12, 359);
            this.TotalDurationLabel.Name = "TotalDurationLabel";
            this.TotalDurationLabel.Size = new System.Drawing.Size(107, 13);
            this.TotalDurationLabel.TabIndex = 5;
            this.TotalDurationLabel.Text = "Total Duration: 00:00";
            // 
            // PlaylistDragAndDropListView
            // 
            this.PlaylistDragAndDropListView.AllowDrop = true;
            this.PlaylistDragAndDropListView.AllowReorder = true;
            this.PlaylistDragAndDropListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlaylistDragAndDropListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.PlaylistDragAndDropListView.FullRowSelect = true;
            this.PlaylistDragAndDropListView.HideSelection = false;
            this.PlaylistDragAndDropListView.LabelWrap = false;
            this.PlaylistDragAndDropListView.LineColor = System.Drawing.Color.LightGray;
            this.PlaylistDragAndDropListView.Location = new System.Drawing.Point(15, 57);
            this.PlaylistDragAndDropListView.Name = "PlaylistDragAndDropListView";
            this.PlaylistDragAndDropListView.Size = new System.Drawing.Size(557, 291);
            this.PlaylistDragAndDropListView.TabIndex = 6;
            this.PlaylistDragAndDropListView.UseCompatibleStateImageBehavior = false;
            this.PlaylistDragAndDropListView.View = System.Windows.Forms.View.Details;
            this.PlaylistDragAndDropListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.PlaylistDragAndDropListView_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 472;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Duration";
            // 
            // PlaylistForm
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 382);
            this.Controls.Add(this.PlaylistDragAndDropListView);
            this.Controls.Add(this.TotalDurationLabel);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.DescriptionTextBox);
            this.Controls.Add(this.label1);
            this.Name = "PlaylistForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Playlist";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox DescriptionTextBox;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label TotalDurationLabel;
        private DragNDrop.DragAndDropListView PlaylistDragAndDropListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}