using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windup.SerialTalker
{
    abstract class AbAnalyzer
    {
        void OnDataRxEvent(object o, DataRxEventArgs e)
        {
            if (IsLineBreak(e.Data)) {

            }
        }

        abstract public string GetAnalyzerName();

        abstract public bool IsLineBreak(byte what);
    }
}
