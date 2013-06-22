using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MediaInfoNET;

namespace Overlay
{
    class MediaParser
    {
        private List<FileInfo> VideoFiles;

        public MediaParser(List<FileInfo> VideoFiles)
        {
            this.VideoFiles = VideoFiles;
        }

        public void ParseFiles()
        {
            Console.WriteLine("Starting parsing");
            foreach (FileInfo file in VideoFiles)
            {
                MediaFile mFile = new MediaFile(file.FullName);
                //VideoListProgressBar.Value++;
                //Thread.Sleep(1000);
                
                Console.WriteLine("File Name   : {0}", mFile.Name);
                Console.WriteLine("Format      : {0}", mFile.General.Format);
                Console.WriteLine("Duration    : {0}", mFile.General.DurationString);
                Console.WriteLine("Bitrate     : {0}", mFile.General.Bitrate);
                 
            }
        }
    }
}
