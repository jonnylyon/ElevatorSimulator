using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ElevatorSimulator.Logger
{
    class Logger : IDisposable
    {
        private string file;
        private bool consolePrint;

        private StreamWriter w;

        public Logger(string file, bool consolePrint = false)
        {
            this.file = file;
            this.consolePrint = consolePrint;

            using (FileStream fs = File.Create(file))
            {
                // do nothing; clears file
            }

            w = File.AppendText(this.file);
        }

        public void logLine(string line)
        {
            w.WriteLine(line);

            if (this.consolePrint)
            {
                Console.WriteLine(line);
            }
        }

        public void Dispose()
        {
            w.Close();
        }
    }
}
