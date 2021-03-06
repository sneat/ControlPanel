﻿#region includes
//.NET Framework
using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Threading;
using System.Net.Sockets;

//Caspar API
using Svt.Caspar;
using Svt.Network;

//Interop
using System.Runtime.InteropServices;

//BMD API
using BMDSwitcherAPI;
using MediaInfoNET;
using Microsoft.Win32;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using PIEHidDotNet;
#endregion

namespace Overlay
{
    public partial class Form1 : Form
    {
        #region variables
        //Delegates
        private delegate void UpdateGUI(object parameter);
        private delegate void del();

        //CasparCG Objects
        CasparDevice caspar_ = new CasparDevice();
        CasparCGDataCollection cgData = new CasparCGDataCollection();
        CasparItem decklink = new CasparItem("DECKLINK 1");
        private int _initMaps;
        private int _initLowerThird;
        private int _initAnnouncement;
        private int _initSchedule;
        private int[] _initPreset = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private string flashCmd;
        private System.Timers.Timer stingerTimer;

        //BMD Objects
        private IBMDSwitcherDiscovery m_switcherDiscovery;
        private IBMDSwitcher m_switcher;
        private IBMDSwitcherMixEffectBlock m_mixEffectBlock1;

        private SwitcherMonitor m_switcherMonitor;
        private MixEffectBlockMonitor m_mixEffectBlockMonitor;

        private bool m_moveSliderDownwards = false;
        private bool m_currentTransitionReachedHalfway = false;

        private List<InputMonitor> m_inputMonitors = new List<InputMonitor>();

        //Video parsing
        private List<FileInfo> VideoFiles;
        private Dictionary<string, VideoFile> ParsedVideoFiles;
        private String SelectedSingleVideo;
        public Dictionary<string, Playlist> Playlists;
        private String CurrentlyPlayingPlaylist = null;
        private String CurrentlyPlayingVideo = null;
        private String NextPlayingVideo = null;
        private static long playlistCountdown = 0;
        private static long playlistCountdownCurrentVideoDuration = 0;
        private static long playlistCountdownNextVideoDuration = 0;
        private static System.Windows.Forms.Timer playlistTimer;
        private static ConcurrentQueue<VideoFile> VideoQueue;
        public List<VideoFormat> VideoFormats;

        //X-Keys
        public PIEDevice[] devices;
        public int[] cbotodevice = null; //for each item in the CboDevice list maps this index to the device index.  Max devices =100 
        public byte[] wData = null; //write data buffer
        public int selecteddevice = -1; //set to the index of CboDevice
        #endregion

        #region constructor
        //Initializing the form
        public Form1()
        {
            InitializeComponent();
            VideoListStatusLabel.Visible = false;
            VideoListProgressBar.Visible = false;

            //Init values
            _initMaps = 0;
            _initLowerThird = 0;
            _initAnnouncement = 0;
            _initSchedule = 0;

            //CasparCG Events.
            caspar_.Connected += new EventHandler<NetworkEventArgs>(caspar__Connected);
            caspar_.FailedConnect += new EventHandler<NetworkEventArgs>(caspar__FailedConnected);
            caspar_.Disconnected += new EventHandler<NetworkEventArgs>(caspar__Disconnected);

            //Disable all controls
            disableControls();

            //Creates the first part of every CG command ("CG CC-FL")
            flashCmd = "CG " + Properties.Settings.Default.CasparChannel + "-" + Properties.Settings.Default.FlashLayer;

            //Initializing BMD ATEM
            // note: this invoke pattern ensures our callback is called in the main thread. We are making double
            // use of lambda expressions here to achieve this.
            // Essentially, the events will arrive at the callback class (implemented by our monitor classes)
            // on a separate thread. We must marshell these to the main thread, and we're doing this by calling
            // invoke on the Windows Forms object. The lambda expression is just a simplification.
            m_switcherMonitor = new SwitcherMonitor();
            m_switcherMonitor.SwitcherDisconnected += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => SwitcherDisconnected())));

            m_mixEffectBlockMonitor = new MixEffectBlockMonitor();
            m_mixEffectBlockMonitor.ProgramInputChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => UpdateProgramButtonSelection())));
            m_mixEffectBlockMonitor.PreviewInputChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => UpdatePreviewButtonSelection())));
            m_mixEffectBlockMonitor.TransitionFramesRemainingChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => UpdateTransitionFramesRemaining())));
            m_mixEffectBlockMonitor.TransitionPositionChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => UpdateSliderPosition())));
            m_mixEffectBlockMonitor.InTransitionChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => OnInTransitionChanged())));

            m_switcherDiscovery = new CBMDSwitcherDiscovery();
            if (m_switcherDiscovery == null)
            {
                MessageBox.Show("Could not create Switcher Discovery Instance.\nATEM Switcher Software may not be installed.", "Error");
                Environment.Exit(1);
            }

            SwitcherDisconnected();		// start with switcher disconnected

            slcTeam1.SelectedIndex = 1;
            slcTeam2.SelectedIndex = 0;

            stingDelay.Text = Properties.Settings.Default.StingCutTime.ToString();

            chkCount9.Checked = true;

            int idx = VideoTabControl.TabPages.IndexOf(PlaylistsTab);
            VideoTabControl.TabPages.RemoveAt(idx);
            tbCasparServer.Text = Properties.Settings.Default.CasparCGHostname;
            textBoxIP.Text = Properties.Settings.Default.ATEMHost;
        }
        #endregion

        #region populate video formats
        private void populateVideoFormats()
        {
            VideoFormats = new List<VideoFormat>();
            VideoFormats.Add(new VideoFormat("PAL", 50, 1024, 576));
            VideoFormats.Add(new VideoFormat("NTSC", 59.94, 720, 480));
            VideoFormats.Add(new VideoFormat("576p2500", 25, 1024, 576));
            VideoFormats.Add(new VideoFormat("720p2398", 23.976, 1280, 720));
            VideoFormats.Add(new VideoFormat("720p2400", 24, 1280, 720));
            VideoFormats.Add(new VideoFormat("720p2500", 25, 1280, 720));
            VideoFormats.Add(new VideoFormat("720p2997", 29.976, 1280, 720));
            VideoFormats.Add(new VideoFormat("720p3000", 30, 1280, 720));
            VideoFormats.Add(new VideoFormat("720p5000", 50, 1280, 720));
            VideoFormats.Add(new VideoFormat("720p5994", 59.94, 1280, 720));
            VideoFormats.Add(new VideoFormat("720p6000", 60, 1280, 720));
            VideoFormats.Add(new VideoFormat("1080p2397", 23.976, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080p2400", 24, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080p2500", 25, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080p2997", 29.976, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080p3000", 30, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080p5000", 50, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080i5000", 50, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080p5994", 59.94, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080i5994", 59.94, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080p6000", 60, 1920, 1080));
            VideoFormats.Add(new VideoFormat("1080i6000", 60, 1920, 1080));
            VideoFormats.Add(new VideoFormat("2160p2397", 23.976, 3840, 2160));
            VideoFormats.Add(new VideoFormat("2160p2400", 24, 3840, 2160));
            VideoFormats.Add(new VideoFormat("2160p2500", 25, 3840, 2160));
            VideoFormats.Add(new VideoFormat("2160p2997", 29.976, 3840, 2160));
            VideoFormats.Add(new VideoFormat("2160p3000", 30, 3840, 2160));
        }
        #endregion

        #region caspar connection
        //Button handlers
        private void buttonConnect_Click_1(object sender, EventArgs e)
        {
            buttonConnect.Enabled = false;

            if (!caspar_.IsConnected)
            {
                caspar_.Settings.Hostname = this.tbCasparServer.Text; // Properties.Settings.Default.CasparCGHostname;
                caspar_.Settings.Port = Properties.Settings.Default.CasparCGPort;
                caspar_.Connect();
            }
            else
            {
                caspar_.Disconnect();
            }
        }

        //caspar event - connected
        void caspar__Connected(object sender, NetworkEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new UpdateGUI(OnCasparConnected), e);
            else
                OnCasparConnected(e);
        }
        void OnCasparConnected(object param)
        {
            buttonConnect.Enabled = true;
            updateConnectButtonText();

            caspar_.RefreshMediafiles();
            caspar_.RefreshDatalist();

            if (caspar_.Mediafiles.Count < 1)
            {
                // Couldn't get the media list, wait and then try again
                Thread.Sleep(500);
                caspar_.RefreshMediafiles();
                caspar_.RefreshDatalist();
            }

            NetworkEventArgs e = (NetworkEventArgs)param;
            statusStrip1.BackColor = Color.LightGreen;
            toolStripStatusLabel1.Text = "Connected to " + caspar_.Settings.Hostname; // Properties.Settings.Default.CasparCGHostname;
            Properties.Settings.Default.CasparCGHostname = caspar_.Settings.Hostname;
            Properties.Settings.Default.Save();

            VideoFiles = new List<FileInfo>();
            ParsedVideoFiles = new Dictionary<string, VideoFile>();
            Playlists = new Dictionary<string, Playlist>();
            CurrentlyPlayingPlaylist = null;
            CurrentlyPlayingVideo = null;
            NextPlayingVideo = null;
            playlistCountdown = 0;
            playlistCountdownCurrentVideoDuration = 0;
            VideoQueue = new ConcurrentQueue<VideoFile>();

            enableControls();
            popVidBox();
        }



        //caspar event - failed connect
        void caspar__FailedConnected(object sender, NetworkEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new UpdateGUI(OnCasparFailedConnect), e);
            else
                OnCasparFailedConnect(e);
        }
        void OnCasparFailedConnect(object param)
        {
            buttonConnect.Enabled = true;
            updateConnectButtonText();

            NetworkEventArgs e = (NetworkEventArgs)param;
            statusStrip1.BackColor = Color.LightCoral;
            toolStripStatusLabel1.Text = "Failed to connect to " + caspar_.Settings.Hostname; // Properties.Settings.Default.CasparCGHostname;

            disableControls();
        }

        //caspar event - disconnected
        void caspar__Disconnected(object sender, NetworkEventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new UpdateGUI(OnCasparDisconnected), e);
            else
                OnCasparDisconnected(e);
        }

        //When caspar is disconnected
        void OnCasparDisconnected(object param)
        {
            buttonConnect.Enabled = true;
            updateConnectButtonText();

            NetworkEventArgs e = (NetworkEventArgs)param;
            statusStrip1.BackColor = Color.LightCoral;
            toolStripStatusLabel1.Text = "Disconnected from " + caspar_.Settings.Hostname; // Properties.Settings.Default.CasparCGHostname;

            disableControls();
            // TODO Media list info causes error when trying to re-load the information. Need to clear out the lists of media files etc.
        }

        // update text on button
        private void updateConnectButtonText()
        {
            if (!caspar_.IsConnected)
            {
                buttonConnect.Text = "Connect";// to " + Properties.Settings.Default.Hostname;
            }
            else
            {
                buttonConnect.Text = "Disconnect"; // from " + Properties.Settings.Default.Hostname;
            }
        }

        #endregion

        #region ATEMControls
        private void OnInputLongNameChanged(object sender, object args)
        {
            this.Invoke((Action)(() => UpdatePopupItems()));
        }

        //When switcher is connected
        private void SwitcherConnected()
        {
            Properties.Settings.Default.ATEMHost = textBoxIP.Text;
            Properties.Settings.Default.Save();

            bmdConnect.Enabled = false;
            groupBox4.Enabled = true;

            // Install SwitcherMonitor callbacks:
            m_switcher.AddCallback(m_switcherMonitor);

            // We create input monitors for each input. To do this we iterator over all inputs:
            // This will allow us to update the combo boxes when input names change:
            IBMDSwitcherInputIterator inputIterator;
            if (SwitcherAPIHelper.CreateIterator(m_switcher, out inputIterator))
            {
                IBMDSwitcherInput input;
                inputIterator.Next(out input);
                while (input != null)
                {
                    InputMonitor newInputMonitor = new InputMonitor(input);
                    input.AddCallback(newInputMonitor);
                    newInputMonitor.LongNameChanged += new SwitcherEventHandler(OnInputLongNameChanged);

                    m_inputMonitors.Add(newInputMonitor);

                    inputIterator.Next(out input);
                }
            }

            // We want to get the first Mix Effect block (ME 1). We create a ME iterator,
            // and then get the first one:
            m_mixEffectBlock1 = null;
            IBMDSwitcherMixEffectBlockIterator meIterator;
            SwitcherAPIHelper.CreateIterator(m_switcher, out meIterator);

            if (meIterator != null)
            {
                meIterator.Next(out m_mixEffectBlock1);
            }

            if (m_mixEffectBlock1 == null)
            {
                MessageBox.Show("Unexpected: Could not get first mix effect block", "Error");
                return;
            }

            // Install MixEffectBlockMonitor callbacks:
            m_mixEffectBlock1.AddCallback(m_mixEffectBlockMonitor);

            MixEffectBlockSetEnable(true);
            UpdatePopupItems();
            UpdateTransitionFramesRemaining();
            UpdateSliderPosition();
        }

        //When switcher is disconnected
        private void SwitcherDisconnected()
        {
            bmdConnect.Enabled = true;
            groupBox4.Enabled = false;


            MixEffectBlockSetEnable(false);

            // Remove all input monitors, remove callbacks
            foreach (InputMonitor inputMon in m_inputMonitors)
            {
                inputMon.Input.RemoveCallback(inputMon);
                inputMon.LongNameChanged -= new SwitcherEventHandler(OnInputLongNameChanged);
            }
            m_inputMonitors.Clear();

            if (m_mixEffectBlock1 != null)
            {
                // Remove callback
                m_mixEffectBlock1.RemoveCallback(m_mixEffectBlockMonitor);

                // Release reference
                m_mixEffectBlock1 = null;
            }

            if (m_switcher != null)
            {
                // Remove callback:
                m_switcher.RemoveCallback(m_switcherMonitor);

                // release reference:
                m_switcher = null;
            }
        }

        //Enable M/E
        private void MixEffectBlockSetEnable(bool enable)
        {
            //ENABLE
            buttonAuto.Enabled = enable;
            buttonCut.Enabled = enable;
            trackBarTransitionPos.Enabled = enable;
            for (int i = 1; i <= 8; i++)
            {
                panelPrev.Controls["prevBtn" + i].Enabled = false;
                panelProg.Controls["progBtn" + i].Enabled = false;
            }
        }

        //Update the buttons with the text
        private void UpdatePopupItems()
        {
            // Clear the combo boxes:
            

            // Get an input iterator. We use the SwitcherAPIHelper to create the iterator for us:
            IBMDSwitcherInputIterator inputIterator;
            if (!SwitcherAPIHelper.CreateIterator(m_switcher, out inputIterator))
                return;

            string[] ignore = { "Black", "Color Bars", "Color 1", "Color 2", "Media Player 1", "Media Player 1 Key", "Media Player 2", "Media Player 2 Key", "Program", "Preview", "Clean Feed 1", "Clean Feed 2" };

            IBMDSwitcherInput input;
            inputIterator.Next(out input);
            while (input != null)
            {
                string inputName;
                long inputId;

                input.GetInputId(out inputId);
                input.GetString(_BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdLongName, out inputName);

                // Add items to list
                if (!ignore.Contains(inputName))
                {
                    panelPrev.Controls["prevBtn" + inputId].Text = inputName;
                    panelProg.Controls["progBtn" + inputId].Text = inputName;

                    groupBox5.Controls["btnLogo" + inputId].Text = inputName;

                    panelPrev.Controls["prevBtn" + inputId].Tag = inputId;
                    panelProg.Controls["progBtn" + inputId].Tag = inputId;

                    panelPrev.Controls["prevBtn" + inputId].Enabled = true;
                    panelProg.Controls["progBtn" + inputId].Enabled = true;
                }

                inputIterator.Next(out input);
            }

            UpdateProgramButtonSelection();
            UpdatePreviewButtonSelection();
        }

        //Reset all program buttons to non-active colors
        private void resetProgBtns()
        {
            for (int i = 1; i <= 8; i++)
            {
                panelProg.Controls["progBtn" + i].BackColor = System.Drawing.SystemColors.Control;
            }
        }

        //Reset all preview buttons to non-active colors
        private void resetPrevBtns()
        {
            for (int i = 1; i <= 8; i++)
            {
                panelPrev.Controls["prevBtn" + i].BackColor = System.Drawing.SystemColors.Control;
            }
        }

        //Update program buttons
        private void UpdateProgramButtonSelection()
        {
            long programId;

            m_mixEffectBlock1.GetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput, out programId);

            resetProgBtns();

            panelProg.Controls["progBtn" + programId].BackColor = System.Drawing.Color.Red;

            if (checkLogoAuto.Checked)
            {
                changeLogo((int) programId);
            }
        }

        //Update preview buttons
        private void UpdatePreviewButtonSelection()
        {
            long previewId;

            m_mixEffectBlock1.GetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewInput, out previewId);

            resetPrevBtns();

            panelPrev.Controls["prevBtn" + previewId].BackColor = System.Drawing.Color.LawnGreen;
        }

        //Update the frames remaining text field
        private void UpdateTransitionFramesRemaining()
        {
            long framesRemaining;

            m_mixEffectBlock1.GetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdTransitionFramesRemaining, out framesRemaining);

            textBoxTransFramesRemaining.Text = String.Format("{0}", framesRemaining);
        }

        //Constantly update the slider when the auto has been used
        private void UpdateSliderPosition()
        {
            double transitionPos;

            m_mixEffectBlock1.GetFloat(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdTransitionPosition, out transitionPos);

            m_currentTransitionReachedHalfway = (transitionPos >= 0.50);

            if (m_moveSliderDownwards)
                trackBarTransitionPos.Value = 100 - (int)(transitionPos * 100);
            else
                trackBarTransitionPos.Value = (int)(transitionPos * 100);
        }

        //When a transition has been completed
        private void OnInTransitionChanged()
        {
            int inTransition;

            m_mixEffectBlock1.GetFlag(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdInTransition, out inTransition);

            if (inTransition == 0)
            {
                // Toggle the starting orientation of slider handle if a transition has passed through halfway
                if (m_currentTransitionReachedHalfway)
                {
                    m_moveSliderDownwards = !m_moveSliderDownwards;
                    UpdateSliderPosition();
                }
                m_currentTransitionReachedHalfway = false;
            }
        }

        //Change a preview button
        private void changePrev(object sender, EventArgs e)
        {
            long inputId = (long)Convert.ToDouble(((Button)sender).Tag);

            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.SetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewInput,
                    inputId);
            }
        }

        //Change a program button
        private void changeProg(object sender, EventArgs e)
        {
            long inputId = (long)Convert.ToDouble(((Button)sender).Tag);

            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.SetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput,
                    inputId);
            }
        }

        //When connect is pressed on the ATEM connect
        private void bmdConnect_Click(object sender, EventArgs e)
        {
            _BMDSwitcherConnectToFailure failReason = 0;
            string address = textBoxIP.Text;

            try
            {
                // Note that ConnectTo() can take several seconds to return, both for success or failure,
                // depending upon hostname resolution and network response times, so it may be best to
                // do this in a separate thread to prevent the main GUI thread blocking.
                m_switcherDiscovery.ConnectTo(address, out m_switcher, out failReason);
            }
            catch (COMException)
            {
                // An exception will be thrown if ConnectTo fails. For more information, see failReason.
                switch (failReason)
                {
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureNoResponse:
                        MessageBox.Show("No response from Switcher", "Error");
                        break;
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureIncompatibleFirmware:
                        MessageBox.Show("Switcher has incompatible firmware", "Error");
                        break;
                    default:
                        MessageBox.Show("Connection failed for unknown reason", "Error");
                        break;
                }
                return;
            }

            SwitcherConnected();
        }

        //Perform auto
        private void buttonAuto_Click(object sender, EventArgs e)
        {
            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.PerformAutoTransition();
            }
        }

        //Perform cut
        private void buttonCut_Click(object sender, EventArgs e)
        {
            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.PerformCut();
            }
        }

        //Scrolling the track bar (fade)
        private void trackBarTransitionPos_Scroll(object sender, EventArgs e)
        {
            if (m_mixEffectBlock1 != null)
            {
                double position = trackBarTransitionPos.Value / 100.0;
                if (m_moveSliderDownwards)
                    position = (100 - trackBarTransitionPos.Value) / 100.0;

                m_mixEffectBlock1.SetFloat(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdTransitionPosition,
                    position);
            }
        }

        /// <summary>
        /// Used for putting other object types into combo boxes.
        /// </summary>
        struct StringObjectPair<T>
        {
            public string name;
            public T value;

            public StringObjectPair(string name, T value)
            {
                this.name = name;
                this.value = value;
            }

            public override string ToString()
            {
                return name;
            }
        }
        #endregion

        #region controls
        //Disable CasparCG controls
        private void disableControls()
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            groupBox5.Enabled = false;
            groupBox6.Enabled = false;
        }

        //Enable CasparCG controls
        private void enableControls()
        {
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            groupBox5.Enabled = true;
            groupBox6.Enabled = true;
        }
        #endregion

        #region actions

        //Populate video comboboxes
        private void popVidBox()
        {
            int range = caspar_.Mediafiles.Count;

            videoBox.AutoGenerateColumns = false;
            videoBox.Columns.Clear();
            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.DataPropertyName = "CasparPath";
            nameColumn.HeaderText = "Name";
            nameColumn.Name = "VideoBoxName";
            nameColumn.ReadOnly = true;
            DataGridViewTextBoxColumn durationColumn = new DataGridViewTextBoxColumn();
            durationColumn.DataPropertyName = "DurationString";
            durationColumn.HeaderText = "Duration";
            durationColumn.ReadOnly = true;
            durationColumn.FillWeight = 40;
            videoBox.Columns.Add(nameColumn);
            videoBox.Columns.Add(durationColumn);

            for (int i = 0; i < range; i++)
            {
                Svt.Caspar.MediaInfo item = caspar_.Mediafiles[i];

                string filename = item.ToString().Replace("\\", "/");
                string filetype = item.Type.ToString();

                if (filetype == "MOVIE")
                {
                    VideoFile VideoFile = new VideoFile();
                    VideoFile.CasparPath = filename;
                    ParsedVideoFiles.Add(filename, VideoFile);
                    Console.WriteLine("Adding {0}", filename);
                }
                else
                {
                    imageBox.Items.Add(filename);
                }
            }

            videoBox.DataSource = ParsedVideoFiles.Values.ToArray();

            //videoBox.ResetBindings();
            try
            {
                DirectoryInfo dinfo = new DirectoryInfo(@Properties.Settings.Default.NetworkVideoFolder);
                FileInfo[] Files = dinfo.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (FileInfo file in Files)
                {
                    String name = ConvertNetworkPathToCasparPath(file);
                    // Check to make sure it is a video
                    if (GetMimeType(file).StartsWith("video/"))
                    {
                        VideoFiles.Add(file);
                    }
                }

                if (VideoFiles.Count > 0)
                {
                    VideoListStatusLabel.Visible = true;
                    VideoListProgressBar.Visible = true;

                    VideoParserBackgroundWorker.DoWork += ParseVideoFiles;
                    VideoParserBackgroundWorker.ProgressChanged += VideoParserBackgroundWorker_ProgressChanged;
                    VideoParserBackgroundWorker.RunWorkerCompleted += VideoParserBackgroundWorker_Completed;
                    VideoParserBackgroundWorker.WorkerReportsProgress = true;
                    VideoParserBackgroundWorker.RunWorkerAsync();
                }
            }
            catch
            {
                MessageBox.Show("Could not read " + Properties.Settings.Default.NetworkVideoFolder + ". Please ensure path is accessible.", "Error");
            }
        }

        //Change logo action
        private void changeLogo(int id)
        {
            for (int i = 1; i < _initPreset.Length; i++)
            {
                if (_initPreset[i] == 1 && i != id)
                {
                    toggleLogo(i);
                    groupBox5.Controls["btnLogo" + i].BackColor = System.Drawing.Color.LawnGreen;
                    _initPreset[i] = 0;
                }
            }

            if (_initPreset[id] == 1)
            {
                toggleLogo(id);
                groupBox5.Controls["btnLogo" + id].BackColor = System.Drawing.Color.LawnGreen;
                _initPreset[id] = 0;
            }
            else
            {
                startLogo(id);
                toggleLogo(id);
                groupBox5.Controls["btnLogo" + id].BackColor = System.Drawing.Color.Red;
                _initPreset[id] = 1;
            }
        }

        //Adds the lowerthird template with data
        private void startLowerthird()
        {
            try
            {
                // Clear old data
                cgData.Clear();

                // build data
                if (chkCa1.Checked)
                {
                    cgData.SetData("nick1", cnick1.Text);
                    cgData.SetData("name1", cname1.Text);
                    cgData.SetData("title1", ctitle1.Text);
                }
                else
                {
                    cgData.SetData("nick1", "");
                    cgData.SetData("name1", "");
                    cgData.SetData("title1", "");
                }

                if (chkCa2.Checked)
                {
                    cgData.SetData("nick2", cnick2.Text);
                    cgData.SetData("name2", cname2.Text);
                    cgData.SetData("title2", ctitle2.Text);
                }
                else
                {
                    cgData.SetData("nick2", "");
                    cgData.SetData("name2", "");
                    cgData.SetData("title2", "");
                }

                if (chkCa3.Checked)
                {
                    cgData.SetData("nick3", cnick3.Text);
                    cgData.SetData("name3", cname3.Text);
                    cgData.SetData("title3", ctitle3.Text);
                }
                else
                {
                    cgData.SetData("nick3", "");
                    cgData.SetData("name3", "");
                    cgData.SetData("title3", "");
                }
            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " ADD " + Properties.Settings.Default.LayerLowerthird + " " + Properties.Settings.Default.TemplateLowerthird + " 1 \"" + cgData.ToAMCPEscapedXml() + "\"");
                }
            }
        }

        //Adds the specific logo template (id: 1-8)
        private void startLogo(int id)
        {
            try
            {
                // Clear old data
                cgData.Clear();
            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " ADD " + Convert.ToInt32(Properties.Settings.Default["LayerPreset" + id].ToString()) + " " + Properties.Settings.Default["TemplatePreset" + id].ToString() + " 1");
                }
            }
        }

        //Shows/hides a specific logo (id: 1-8)
        private void toggleLogo(int id)
        {
            try
            {
                // Clear old data
                cgData.Clear();
            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " NEXT " + Properties.Settings.Default.LayerPreset1 + " " + Properties.Settings.Default.TemplatePreset1 + " 1");
                }
            }
        }

        //Adds announcement template with data
        private void startAnnouncement()
        {
            try
            {
                // Clear old data
                cgData.Clear();

                // build data
                cgData.SetData("scrollText", txtAnnounce.Text);
            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " ADD " + Properties.Settings.Default.LayerAnnouncement + " " + Properties.Settings.Default.TemplateAnnouncement + " 1 \"" + cgData.ToAMCPEscapedXml() + "\"");
                }
            }
        }

        //Adds the schedule with data
        private void startSchedule()
        {
            try
            {
                // Clear old data
                cgData.Clear();

                // build data
                cgData.SetData("time1", schT1.Text);
                cgData.SetData("text1", schH1.Text);

                cgData.SetData("time2", schT2.Text);
                cgData.SetData("text2", schH2.Text);

                cgData.SetData("time3", schT3.Text);
                cgData.SetData("text3", schH3.Text);

                cgData.SetData("time4", schT4.Text);
                cgData.SetData("text4", schH4.Text);

                cgData.SetData("time5", schT5.Text);
                cgData.SetData("text5", schH5.Text);

                cgData.SetData("time6", schT6.Text);
                cgData.SetData("text6", schH6.Text);

                cgData.SetData("time7", schT7.Text);
                cgData.SetData("text7", schH7.Text);

                cgData.SetData("time8", schT8.Text);
                cgData.SetData("text8", schH8.Text);

                cgData.SetData("txtminute", cdMM.Text);
                cgData.SetData("txthour", cdH.Text);
                cgData.SetData("txtday", cdD.Text);
                cgData.SetData("txtmonth", cdM.Text);
                cgData.SetData("txtyear", cdY.Text);

                if (chkCount1.Checked)
                {
                    cgData.SetData("countDown", "1");
                }
                else if (chkCount2.Checked)
                {
                    cgData.SetData("countDown", "2");
                }
                else if (chkCount3.Checked)
                {
                    cgData.SetData("countDown", "3");
                }
                else if (chkCount4.Checked)
                {
                    cgData.SetData("countDown", "4");
                }
                else if (chkCount5.Checked)
                {
                    cgData.SetData("countDown", "5");
                }
                else if (chkCount6.Checked)
                {
                    cgData.SetData("countDown", "6");
                }
                else if (chkCount7.Checked)
                {
                    cgData.SetData("countDown", "7");
                }
                else if (chkCount8.Checked)
                {
                    cgData.SetData("countDown", "8");
                }
                else
                {
                    cgData.SetData("countDown", "9");
                }

            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString("PLAY " + Properties.Settings.Default.CasparChannel + "-" + Properties.Settings.Default.LayerScheduleBG + " " + Properties.Settings.Default.TemplateScheduleBG + " MIX 50 EASEINSINE LOOP");
                    caspar_.SendString(flashCmd + " ADD " + Properties.Settings.Default.LayerSchedule + " " + Properties.Settings.Default.TemplateSchedule + " 1 \"" + cgData.ToAMCPEscapedXml() + "\"");
                }
            }
        }

        //Reload the content of the schedule rows (will instantly show/hide rows that go from zero to non-zero or non-zero to zero length content in the text field)
        private void reloadSchedule()
        {
            try
            {
                // Clear old data
                cgData.Clear();

                // build data
                cgData.SetData("time1", schT1.Text);
                cgData.SetData("text1", schH1.Text);

                cgData.SetData("time2", schT2.Text);
                cgData.SetData("text2", schH2.Text);

                cgData.SetData("time3", schT3.Text);
                cgData.SetData("text3", schH3.Text);

                cgData.SetData("time4", schT4.Text);
                cgData.SetData("text4", schH4.Text);

                cgData.SetData("time5", schT5.Text);
                cgData.SetData("text5", schH5.Text);

                cgData.SetData("time6", schT6.Text);
                cgData.SetData("text6", schH6.Text);

                cgData.SetData("time7", schT7.Text);
                cgData.SetData("text7", schH7.Text);

                cgData.SetData("time8", schT8.Text);
                cgData.SetData("text8", schH8.Text);

                cgData.SetData("txtminute", cdMM.Text);
                cgData.SetData("txthour", cdH.Text);
                cgData.SetData("txtday", cdD.Text);
                cgData.SetData("txtmonth", cdM.Text);
                cgData.SetData("txtyear", cdY.Text);

                if (chkCount1.Checked)
                {
                    cgData.SetData("countDown", "1");
                }
                else if (chkCount2.Checked)
                {
                    cgData.SetData("countDown", "2");
                }
                else if (chkCount3.Checked)
                {
                    cgData.SetData("countDown", "3");
                }
                else if (chkCount4.Checked)
                {
                    cgData.SetData("countDown", "4");
                }
                else if (chkCount5.Checked)
                {
                    cgData.SetData("countDown", "5");
                }
                else if (chkCount6.Checked)
                {
                    cgData.SetData("countDown", "6");
                }
                else if (chkCount7.Checked)
                {
                    cgData.SetData("countDown", "7");
                }
                else if (chkCount8.Checked)
                {
                    cgData.SetData("countDown", "8");
                }
                else
                {
                    cgData.SetData("countDown", "9");
                }
            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " UPDATE " + Properties.Settings.Default.LayerSchedule + " \"" + cgData.ToAMCPEscapedXml() + "\"");
                }
            }
        }

        //Adds the maps template to the scene and fills in content
        private void startMaps(object sender, EventArgs e)
        {
            try
            {
                // Clear old data
                cgData.Clear();

                // build data
                cgData.SetData("slc1", slcTeam1.Text);
                cgData.SetData("slc2", slcTeam2.Text);


                if (chkShowM1.Checked)
                {
                    cgData.SetData("mappet1", map1.Text);
                }
                else
                {
                    cgData.SetData("mappet1", "");
                }

                if (chkShowM2.Checked)
                {
                    cgData.SetData("mappet2", map2.Text);
                }
                else
                {
                    cgData.SetData("mappet2", "");
                }

                if (chkShowM3.Checked)
                {
                    cgData.SetData("mappet3", map3.Text);
                }
                else
                {
                    cgData.SetData("mappet3", "");
                }

                if (chkShowM4.Checked)
                {
                    cgData.SetData("mappet4", map4.Text);
                }
                else
                {
                    cgData.SetData("mappet4", "");
                }

                cgData.SetData("team11", txtTeam1.Text);
                cgData.SetData("team21", txtTeam2.Text);
                cgData.SetData("team12", txtTeam1.Text);
                cgData.SetData("team22", txtTeam2.Text);
                cgData.SetData("team13", txtTeam1.Text);
                cgData.SetData("team23", txtTeam2.Text);
                cgData.SetData("team14", txtTeam1.Text);
                cgData.SetData("team24", txtTeam2.Text);

                string mscore1 = m1t1.Text + "-" + m1t2.Text;
                string mscore2 = m2t1.Text + "-" + m2t2.Text;
                string mscore3 = m3t1.Text + "-" + m3t2.Text;
                string mscore4 = m4t1.Text + "-" + m4t2.Text;

                cgData.SetData("score1", mscore1);
                cgData.SetData("score2", mscore2);
                cgData.SetData("score3", mscore3);
                cgData.SetData("score4", mscore4);
            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " ADD " + Properties.Settings.Default.LayerMaps + " " + Properties.Settings.Default.TemplateMaps + " 1 \"" + cgData.ToAMCPEscapedXml() + "\"");
                }
            }
        }

        //Shows/hides lowerthird template
        private void toggleLowerthird()
        {
            try
            {

            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " NEXT " + Properties.Settings.Default.LayerLowerthird);
                }
            }
        }

        //Shows/hides schedule template
        private void toggleSchedule()
        {
            try
            {

            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " NEXT " + Properties.Settings.Default.LayerSchedule);
                }
            }
        }

        //Shows/hides announcement template
        private void toggleAnnouncement()
        {
            try
            {

            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " NEXT " + Properties.Settings.Default.LayerAnnouncement);
                }
            }
        }

        //Shows/hides map template
        private void toggleMaps()
        {
            try
            {
                
            }
            catch
            {

            }
            finally
            {
                if (caspar_.IsConnected && caspar_.Channels.Count > 0)
                {
                    caspar_.SendString(flashCmd + " NEXT " + Properties.Settings.Default.LayerMaps);
                }
            }
        }

        //Swaps teams - scores, names, types etc.
        private void swapTeams()
        {
            try
            {
                string slc1 = slcTeam1.Text;
                string slc2 = slcTeam2.Text;
                string txt1 = txtTeam1.Text;
                string txt2 = txtTeam2.Text;

                string map1t1 = m1t1.Text;
                string map1t2 = m1t2.Text;
                string map2t1 = m2t1.Text;
                string map2t2 = m2t2.Text;
                string map3t1 = m3t1.Text;
                string map3t2 = m3t2.Text;
                string map4t1 = m4t1.Text;
                string map4t2 = m4t2.Text;

                slcTeam1.Text = slc2;
                slcTeam2.Text = slc1;
                txtTeam1.Text = txt2;
                txtTeam2.Text = txt1;

                m1t1.Text = map1t2;
                m1t2.Text = map1t1;
                m2t1.Text = map2t2;
                m2t2.Text = map2t1;
                m3t1.Text = map3t2;
                m3t2.Text = map3t1;
                m4t1.Text = map4t2;
                m4t2.Text = map4t1;
            }
            catch
            {

            }
        }

        //Swaps CT/T
        private void swapType()
        {
            try
            {
                string slc1 = slcTeam1.Text;
                string slc2 = slcTeam2.Text;

                slcTeam1.Text = slc2;
                slcTeam2.Text = slc1;
            }
            catch
            {

            }
        }

        /* FOR FUTURE USE - Can browse files on HDD */
        private string browseFile() {
            string txtt = "";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                txtt = openFileDialog1.FileName;
            }
            return txtt;
        }

        //Perform the cut action on ATEM and stop timer
        private void endSting(object sender, EventArgs e)
        {
            //Perform in main thread
            del doCut = new del(m_mixEffectBlock1.PerformCut);
            Invoke(doCut);
            stingerTimer.Enabled = false;
        }
        #endregion

        #region xkeys
        public void SetAllXKeysBacklights(bool on) {
            //Turns on or off, ALL bank 1 BLs using current intensity
            if (selecteddevice != -1) //do nothing if not enumerated
            {
                byte sl = 0;

                if (on == true)
                {
                    sl = 255;
                }

                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }

                wData[0] = 0;
                wData[1] = 182; //0xb6
                wData[2] = 0;  //0 for bank 1, 1 for bank 2
                wData[3] = (byte)sl; //OR turn individual rows on or off using bits.  1st bit=row 1, 2nd bit=row 2, 3rd bit =row 3, etc

                int result = devices[selecteddevice].WriteData(wData);
                if (result != 0)
                {
                    Console.WriteLine( "Write Fail: " + result);
                }
                else
                {
                    Console.WriteLine( "Write Success-All bank 1 BL {0}", on ? "on" : "off");
                }
            }
        }
        #endregion

        #region buttonactions

        private void ConnectXKeysButton_Click(object sender, EventArgs e)
        {
            cbotodevice = new int[128]; //128=max # of devices
            //enumerate and setupinterfaces for all devices
            devices = PIEHidDotNet.PIEDevice.EnumeratePIE();
            if (devices.Length == 0)
            {
                toolStripStatusLabel1.Text = "No Devices Found";
            }
            else
            {
                //System.Media.SystemSounds.Beep.Play(); 
                int cbocount = 0; //keeps track of how many valid devices were added to the CboDevice box
                for (int i = 0; i < devices.Length; i++)
                {
                    //information about device
                    //PID = devices[i].Pid);
                    //HID Usage = devices[i].HidUsage);
                    //HID Usage Page = devices[i].HidUsagePage);
                    //HID Version = devices[i].Version);
                    int hidusagepg = devices[i].HidUsagePage;
                    int hidusage = devices[i].HidUsage;
                    if (devices[i].HidUsagePage == 0xc)
                    {
                        switch (devices[i].Pid)
                        {
                            case 1027:
                                //Device 2 Keyboard, Joystick, Input and Output endpoints
                                GeneralStatusMessage.Text = "Connected: X-keys XK-24 (" + devices[i].Pid + "=PID #2)";
                                GeneralStatusMessage.Visible = true;
                                cbotodevice[cbocount] = i;
                                cbocount++;
                                break;
                            case 1028:
                                //Device 1 Keyboard, Joystick, Mouse and Output endpoints
                                GeneralStatusMessage.Text = "Connected: X-keys XK-24 (" + devices[i].Pid + ")";
                                GeneralStatusMessage.Visible = true;
                                cbotodevice[cbocount] = i;
                                cbocount++;
                                break;
                            case 1029:
                                //Device 0 Keyboard, Mouse, Input and Output endpoints (factory default)
                                GeneralStatusMessage.Text = "Connected: X-keys XK-24 (" + devices[i].Pid + "=PID #1)";
                                GeneralStatusMessage.Visible = true;
                                cbotodevice[cbocount] = i;
                                cbocount++;
                                break;
                            default:
                                break;
                        }
                        devices[i].SetupInterface(false);
                    }
                }
                if (cbotodevice.Count() > 0)
                {
                    System.Timers.Timer aTimer = new System.Timers.Timer(1000 * 10);
                    aTimer.Elapsed += new ElapsedEventHandler(ClearGeneralStatusMessageText);
                    aTimer.Enabled = true; 
                    selecteddevice = cbotodevice[0];
                    wData = new byte[devices[selecteddevice].WriteLength];//go ahead and setup for write

                    // Turn the Green LED on
                    for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                    {
                        wData[j] = 0;
                    }
                    wData[1] = 179; //0xb3
                    wData[2] = 6; //6 for green, 7 for red
                    wData[3] = 1; //0=off, 1=on, 2=flash
                    int result = devices[selecteddevice].WriteData(wData);

                    // Flash the lights to show that we're ok
                    SetAllXKeysBacklights(false);
                    SetAllXKeysBacklights(true);
                    SetAllXKeysBacklights(false);
                }
            }
        }

        //Clear ALL!
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (caspar_.IsConnected && caspar_.Channels.Count > 0)
            {
                caspar_.SendString("CLEAR " + Properties.Settings.Default.CasparChannel);
            }
        }

        //Show/hide lowerthird
        private void btnShowLowerthird_Click(object sender, EventArgs e)
        {
            if (_initLowerThird == 0)
            {
                startLowerthird();
                this.btnShowLowerthird.Text = "Hide";
                this.btnShowLowerthird.BackColor = System.Drawing.Color.Red;
                _initLowerThird = 1;
            }
            else
            {
                this.btnShowLowerthird.Text = "Show";
                this.btnShowLowerthird.BackColor = System.Drawing.Color.LawnGreen;
                _initLowerThird = 0;
            }

            toggleLowerthird();
        }

        //Show/hide announcement topbar
        private void btnShowAnnounce_Click(object sender, EventArgs e)
        {
            if (_initAnnouncement == 0)
            {
                startAnnouncement();
                this.btnShowAnnounce.Text = "Hide";
                this.btnShowAnnounce.BackColor = System.Drawing.Color.Red;
                _initAnnouncement = 1;
            }
            else
            {
                this.btnShowAnnounce.Text = "Show";
                this.btnShowAnnounce.BackColor = System.Drawing.Color.LawnGreen;
                _initAnnouncement = 0;
            }

            toggleAnnouncement();
        }
        
        //Swap teams + scores (both swaps type, names and mapscores)
        private void btnSwap1_Click(object sender, EventArgs e)
        {
            swapTeams();
        }
        
        //Change logo button
        private void changeLogo_Click(object sender, EventArgs e)
        {
            Button btn = ((Button)sender);
            int id = Convert.ToInt32(btn.Tag);
            changeLogo(id);
        }

        //Swap CT/T 
        private void btnSwap2_Click(object sender, EventArgs e)
        {
            swapType();
        }

        //Show/hide maps template
        private void btnShowMaps_Click(object sender, EventArgs e)
        {
            if(_initMaps == 0) {
                startMaps(sender, e);
                this.btnShowMaps.Text = "Hide";
                this.btnShowMaps.BackColor = System.Drawing.Color.Red;
                _initMaps = 1;
            } else {
                this.btnShowMaps.Text = "Show";
                this.btnShowMaps.BackColor = System.Drawing.Color.LawnGreen;
                _initMaps = 0;
            }

            toggleMaps();
        }

        //Show/hide schedule
        private void btnShowSchedule_Click(object sender, EventArgs e)
        {
            if (_initSchedule == 0)
            {
                startSchedule();
                this.btnShowSchedule.Text = "Hide";
                this.btnShowSchedule.BackColor = System.Drawing.Color.Red;
                _initSchedule = 1;
            }
            else
            {
                caspar_.SendString("PLAY " + Properties.Settings.Default.CasparChannel + "-" + Properties.Settings.Default.LayerScheduleBG + " EMPTY MIX 20 EASEINSINE AUTO");
                this.btnShowSchedule.Text = "Show";
                this.btnShowSchedule.BackColor = System.Drawing.Color.LawnGreen;
                _initSchedule = 0;
            }

            toggleSchedule();
        }

        //Exit button top right
        private void Exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
            Application.Exit();
        }

        //Click the sting button - play sting template
        private void buttonSting_Click(object sender, EventArgs e)
        {
            try
            {
                //Set delay and end-action
                stingerTimer = new System.Timers.Timer(Convert.ToInt32(stingDelay.Text));
                stingerTimer.Elapsed += new ElapsedEventHandler(this.endSting);

                caspar_.SendString(flashCmd + " ADD " + Properties.Settings.Default.LayerSting + " " + Properties.Settings.Default.TemplateSting + " 1");
            }
            catch
            {
            }
            finally
            {
                stingerTimer.Enabled = true;
            }
        }

        //Reload content in schedule rows
        private void btnReload_Click(object sender, EventArgs e)
        {
            reloadSchedule();
        }

        //Play selected video - fades to EMPTY after ended
        private void buttonPlayVid_Click(object sender, EventArgs e)
        {
            if (SelectedSingleVideo != null)
            {
                try
                {
                    caspar_.SendString(String.Format("PLAY {0}-{1} \"{2}\" {3} {4} {5}", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer, SelectedSingleVideo.Replace("\\", "/"), Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
                    caspar_.SendString(String.Format("LOADBG {0}-{1} EMPTY {2} {3} {4} AUTO", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer, Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
                }
                catch { }
            }
        }

        //Stop current video
        private void buttonStopVid_Click(object sender, EventArgs e)
        {
            try
            {
                caspar_.SendString(String.Format("STOP {0}-{1}", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer));
            } catch { }
        }

        //Removes current image on the scene
        private void buttonStopImg_Click(object sender, EventArgs e)
        {
            try
            {
                caspar_.SendString(String.Format("PLAY {0}-{1} EMPTY {2} {3} {4} AUTO", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.ImgLayer, Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
            }
            catch { }
        }

        //Adds marked image to the scene
        private void buttonPlayImg_Click(object sender, EventArgs e)
        {
            try
            {
                caspar_.SendString(String.Format("PLAY {0}-{1} \"{2}\" {3} {4} {5}", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.ImgLayer, imageBox.Text.Replace("\\", "/"), Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
                caspar_.SendString(String.Format("LOADBG {0}-{1} EMPTY {2} {3} {4} AUTO", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.ImgLayer, Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
            }
            catch { }
        }

        //Loops the marked video
        private void buttonVidLoop_Click(object sender, EventArgs e)
        {
            if (SelectedSingleVideo != null)
            {
                try
                {
                    caspar_.SendString(String.Format("PLAY {0}-{1} \"{2}\" CUT 1 LINEAR LOOP", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer, SelectedSingleVideo.Replace("\\", "/")));    
                }
                catch { }
            }
        }

        //Opens the settings dialog
        private void settings_button_Click(object sender, EventArgs e)
        {
            Settings settingsForm = new Settings();
            settingsForm.ShowDialog();
        }
        #endregion

        #region Video/playlist functionality

        //Convert UNC file path to filename for use within CasparCG
        public String ConvertNetworkPathToCasparPath(FileInfo file)
        {
            return file.FullName.Replace(Properties.Settings.Default.NetworkVideoFolder + "\\", "").Replace(file.Extension, "").Replace("\\", "/").ToUpper();
        }

        //Read video files over network to get duration
        private void ParseVideoFiles(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int total = VideoFiles.Count();
            int i = 0;
            foreach (FileInfo file in VideoFiles)
            {
                i++;
                Console.WriteLine("Checking {0}", file.FullName);
                MediaFile mFile = new MediaFile(@file.FullName);

                VideoFile VideoFile = ParsedVideoFiles[ConvertNetworkPathToCasparPath(file)];
                VideoFile.NetworkPath = file.FullName;
                VideoFile.Duration = mFile.General.DurationMillis;
                VideoFile.DurationString = TimeSpan.FromMilliseconds(VideoFile.Duration).ToString(@"mm\:ss");
                Console.WriteLine("{0} {1} {2}fps", VideoFile.CasparPath, VideoFile.DurationString, mFile.Video[0].FrameRate);

                ParsedVideoFiles[ConvertNetworkPathToCasparPath(file)] = VideoFile;
                videoBox.NotifyCurrentCellDirty(true);

                int progress = Convert.ToInt32(Math.Round(((double)i / (double)total) * 100d));
                VideoParserBackgroundWorker.ReportProgress(progress);
            }
        }

        //Called when background video parser completed
        private void VideoParserBackgroundWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            VideoListStatusLabel.Visible = false;
            VideoListProgressBar.Visible = false;
            VideoListProgressBar.Value = 0;

            if (e.Error != null || e.Cancelled)
            {
                GeneralStatusMessage.Text = "Error occurred while parsing video files.";
                GeneralStatusMessage.Visible = true;
            }
            else
            {
                VideoTabControl.TabPages.Add(PlaylistsTab);
                videoBox.Update();
            }
        }

        public void ClearGeneralStatusMessageText()
        {
            GeneralStatusMessage.Text = "";
            GeneralStatusMessage.Visible = false;
        }

        public void ClearGeneralStatusMessageText(object sender, ElapsedEventArgs e)
        {
            GeneralStatusMessage.Text = "";
            GeneralStatusMessage.Visible = false;
        }

        //Called when a video has finished being parsed
        private void VideoParserBackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            VideoListProgressBar.Value = e.ProgressPercentage;
        }

        public string GetMimeType(FileInfo fileInfo)
        {
            string mimeType = "application/unknown";

            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(
                fileInfo.Extension.ToLower()
                );

            if (regKey != null)
            {
                object contentType = regKey.GetValue("Content Type");

                if (contentType != null)
                    mimeType = contentType.ToString();
            }

            return mimeType;
        }

        // Keep track of which single video is selected for playing
        private void videoBox_SelectionChanged(object sender, EventArgs e)
        {
            //SelectedSingleVideo
            if (videoBox.SelectedCells.Count == 1)
            {
                int selectedrowindex = videoBox.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = videoBox.Rows[selectedrowindex];

                SelectedSingleVideo = Convert.ToString(selectedRow.Cells["VideoBoxName"].Value);
            }
            else
            {
                SelectedSingleVideo = null;
            }
        }

        private void PlaylistAddButton_Click(object sender, EventArgs e)
        {
            PlaylistForm Playlist = new PlaylistForm(this, ParsedVideoFiles, PlaylistDragAndDropListView);
            Playlist.ShowDialog();
        }

        private void PlaylistDragAndDropListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (PlaylistDragAndDropListView.SelectedItems.Count == 1)
            {
                PlaylistPlay.Enabled = true;
                if (PlaylistDragAndDropListView.SelectedItems[0].Text != CurrentlyPlayingPlaylist)
                {
                    PlaylistEditButton.Enabled = true;
                    PlaylistDeleteButton.Enabled = true;
                }
                else
                {
                    PlaylistEditButton.Enabled = false;
                    PlaylistDeleteButton.Enabled = false;
                }
            }
            else
            {
                PlaylistPlay.Enabled = false;
                PlaylistEditButton.Enabled = false;
                PlaylistDeleteButton.Enabled = false;
            }
        }

        private void PlaylistPlay_Click(object sender, EventArgs e)
        {
            if (PlaylistDragAndDropListView.SelectedItems.Count == 1)
            {
                if (CurrentlyPlayingPlaylist != null)
                {
                    // Already playing something. Stop it.
                    PlaylistStop.PerformClick();
                }

                Playlist _playlist = Playlists[PlaylistDragAndDropListView.SelectedItems[0].Text];
                foreach (KeyValuePair<string, long> item in _playlist.Items)
                {
                    VideoFile VideoFile = ParsedVideoFiles[item.Key];
                    VideoQueue.Enqueue(VideoFile);
                }
                CurrentlyPlayingPlaylist = _playlist.Name;
                playlistCountdown = _playlist.Duration;
                Console.WriteLine("Time starting is {0}ms", playlistCountdown);
                VideoCountdownTimer.ForeColor = SystemColors.ControlText;
                VideoCountdownTimer.Text = _playlist.DurationString;
                VideoCountdownTimer.Visible = true;
                PlaylistStop.Enabled = true;
                playlistTimer = new System.Windows.Forms.Timer();
                playlistTimer.Interval = 1000;
                playlistTimer.Tick += new EventHandler(playlistTimerTick);

                VideoFile CurrentVideoFile;
                if (VideoQueue.TryDequeue(out CurrentVideoFile))
                {
                    try
                    {
                        // Send our first video to be played
                        playlistCountdownCurrentVideoDuration = CurrentVideoFile.Duration;
                        CurrentlyPlayingVideo = CurrentVideoFile.CasparPath;
                        caspar_.SendString(String.Format("PLAY {0}-{1} \"{2}\" {3} {4} {5}", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer, @CurrentlyPlayingVideo, Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
                    }
                    catch { }
                }
                if (VideoQueue.TryDequeue(out CurrentVideoFile))
                {
                    try
                    {
                        playlistCountdownNextVideoDuration = CurrentVideoFile.Duration;
                        NextPlayingVideo = CurrentVideoFile.CasparPath;
                        caspar_.SendString(String.Format("LOADBG {0}-{1} \"{2}\" {3} {4} {5} AUTO", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer, @NextPlayingVideo, Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
                    }
                    catch { }
                }
                else
                {
                    playlistCountdownNextVideoDuration = 0;
                    NextPlayingVideo = null;
                    caspar_.SendString(String.Format("LOADBG {0}-{1} EMPTY {2} {3} {4} AUTO", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer, Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
                }
                playlistTimer.Start();
            }
        }

        private void playlistTimerTick(object source, EventArgs e)
        {
            playlistCountdown -= 1000;
            playlistCountdownCurrentVideoDuration -= 1000;
            Console.WriteLine("Time left is {0}ms", playlistCountdown);
            if (playlistCountdownCurrentVideoDuration < 0)
            {
                // Queue the next video
                playlistCountdownCurrentVideoDuration = playlistCountdownNextVideoDuration;
                CurrentlyPlayingVideo = NextPlayingVideo;
                VideoFile NextVideoFile;
                if (VideoQueue.TryDequeue(out NextVideoFile))
                {
                    try
                    {
                        playlistCountdownNextVideoDuration = NextVideoFile.Duration;
                        NextPlayingVideo = NextVideoFile.CasparPath;
                        caspar_.SendString(String.Format("LOADBG {0}-{1} \"{2}\" {3} {4} {5} AUTO", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer, @NextPlayingVideo, Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
                    }
                    catch { }
                }
                else
                {
                    playlistCountdownNextVideoDuration = 0;
                    NextPlayingVideo = null;
                    caspar_.SendString(String.Format("LOADBG {0}-{1} EMPTY {2} {3} {4} AUTO", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer, Properties.Settings.Default.MediaTransitionType, Properties.Settings.Default.MediaTransitionDuration, Properties.Settings.Default.MediaTransitionAnimationType));
                }
            }
            if (playlistCountdown < 0)
            {
                PlaylistStop.PerformClick();
                playlistCountdownCurrentVideoDuration = 0;
            }
            else
            {
                VideoCountdownTimer.Text = PlaylistForm.ConvertMilisecondsToString(playlistCountdown);
            }
        }

        private void PlaylistStop_Click(object sender, EventArgs e)
        {
            try
            {
                VideoQueue = new ConcurrentQueue<VideoFile>();
                CurrentlyPlayingPlaylist = null;
                PlaylistStop.Enabled = false;
                playlistTimer.Enabled = false;
                VideoCountdownTimer.ForeColor = SystemColors.ControlText;
                VideoCountdownTimer.Text = "00:00";
                VideoCountdownTimer.Visible = false;
                caspar_.SendString(String.Format("STOP {0}-{1}", Properties.Settings.Default.CasparChannel, Properties.Settings.Default.VideoLayer));
            }
            catch { }
        }

        private void PlaylistCountdownTimer_TextChanged(object sender, EventArgs e)
        {
            long seconds = 0;
            if (VideoCountdownTimer.Text != "")
            {
                String[] parts = VideoCountdownTimer.Text.Split(':');
                if (parts.Count() > 0)
                {
                    int minutes = Convert.ToInt32(parts[0]);
                    seconds += minutes * 60;
                    seconds += Convert.ToInt32(parts[1]);
                    if (seconds <= 10)
                    {
                        VideoCountdownTimer.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }
        #endregion

    }

    public class VideoFile : INotifyPropertyChanged
    {
        private string _CasparPath;
        private string _NetworkPath;
        private long _Duration; // in ms
        private string _DurationString;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string CasparPath
        {
            get { return _CasparPath; }
            set
            {
                _CasparPath = value;
                OnPropertyChanged("CasparPath");
            }
        }

        public string NetworkPath
        {
            get { return _NetworkPath; }
            set
            {
                _NetworkPath = value;
                OnPropertyChanged("NetworkPath");
            }
        }

        public long Duration
        {
            get { return _Duration; }
            set
            {
                _Duration = value;
                OnPropertyChanged("Duration");
            }
        }

        public string DurationString
        {
            get { return _DurationString; }
            set
            {
                _DurationString = value;
                OnPropertyChanged("DurationString");
            }
        }
    }

    public class Playlist
    {
        // key properties
        public String Name { get; private set; }
        public long Duration { get; private set; }
        public String DurationString { get; private set; }
        public List<KeyValuePair<string, long>> Items { get; private set; }

        public Playlist(String name, long duration, List<KeyValuePair<string, long>> items)
        {
            Name = name;
            Duration = duration;
            DurationString = PlaylistForm.ConvertMilisecondsToString(duration);
            Items = items;
        }
    }

    public class VideoFormat : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.String)]
        [DefaultSettingValue("1080i5994")]
        public String Name { get; private set; }
        public double FrameRate { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public String Resolution { get { return String.Format("{0}x{1}", X.ToString(), Y.ToString()); } }


        public VideoFormat(String Name, double FrameRate, int X, int Y) { }
    }
}