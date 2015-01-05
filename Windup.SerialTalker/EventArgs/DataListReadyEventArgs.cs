using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windup.SerialTalker
{
    public class DataListReadyEventArgs : EventArgs
    {
        private List<byte> _bytes;
        public List<byte> Bytes { get { return _bytes; } }
        public DataListReadyEventArgs(List<byte> bytes)
        {
            _bytes = bytes;
        }
    }
}
