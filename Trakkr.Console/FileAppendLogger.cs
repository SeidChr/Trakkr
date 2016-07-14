using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Console
{
    internal class FileAppendLogger
    {
        private readonly string fileName;
        private readonly string fileHeader;
        private bool fileHeaderWritten = false;

        public FileAppendLogger(string fileName, string fileHeader)
        {
            this.fileName = fileName;
            this.fileHeader = fileHeader;
        }

        private void WriteFileHeader()
        {
            File.AppendAllLines(fileName, new[] { fileHeader });
            fileHeaderWritten = true;
        }

        public void Log(params string[] lines)
        {
            if (!fileHeaderWritten)
            {
                WriteFileHeader();
            }

            File.AppendAllLines(fileName, lines);
        }
    }
}
