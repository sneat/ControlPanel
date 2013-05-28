using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Overlay
{
    public class CGDataItem
    {

        private String rawDataFile;

        public CGDataItem(string rawDataFile)
        {
            this.rawDataFile = rawDataFile;

            this.dataFileName = this.rawDataFile.Substring(1, this.rawDataFile.IndexOf("\"", 1) - 1);

        }

        public String dataFileName { get; private set; }


        public override string ToString()
        {
            return this.dataFileName;
        }

    }
}
