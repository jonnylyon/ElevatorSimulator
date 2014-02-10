using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ElevatorSimulator.Logger
{
    class Logger
    {
        private string file;
        private bool consolePrint;

        public Logger(string file, bool consolePrint = false)
        {
            this.file = file;
            this.consolePrint = consolePrint;
        }

        public void logLine(string line)
        {
            using (StreamWriter w = File.AppendText(this.file))
            {
                w.WriteLine(line);
            }

            if (this.consolePrint)
            {
                Console.WriteLine(line);
            }
        }
    }
}
