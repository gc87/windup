using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windup.SerialTalker
{
    class Analyzer
    {
        public enum NewLine
        {
            Linux, //n
            Mac, //r
            Nt, //rn
            Length, //length
            Char //char
        };

        public enum ChangeFlag
        {
            Succeed,
            Failed
        }

        private delegate bool IsLineBreak(byte what);
        private IsLineBreak _isLineBreak;
        private SerialAgent _sa;
        private NewLine _newLine;
        private byte? _preByte;
        private int _index = 0;
        private long? _length = null;
        private char? _breakChar = null;
        private List<byte> _list = new List<byte>();

        public bool IsReady
        {
            get {
                return null != _sa && null != _isLineBreak;
            }
        }

        public event EventHandler<DataListReadyEventArgs> DataListReadyEvent;

        protected virtual void OnDataListReadyEvent(DataListReadyEventArgs e)
        {
            EventHandler<DataListReadyEventArgs> handler = DataListReadyEvent;
            if (handler != null) handler(this, e);
        }

        public Analyzer()
        {
        }

        void OnDataRxEvent(object o, DataRxEventArgs e)
        {
            if (_isLineBreak(e.Data)) {
                _index = 0;
                _preByte = null;
                if (NewLine.Nt == _newLine)
                    _list.RemoveAt(_list.Count - 1);
                if (NewLine.Length == _newLine)
                    _list.Add(e.Data);
                DataListReadyEvent(this, new DataListReadyEventArgs(_list));
                _list = new List<byte>();
            } else {
                _index++;
                _preByte = e.Data;
                _list.Add(e.Data);
            }
        }

        public ChangeFlag SetAgent(SerialAgent sa)
        {
            try {
                if (null != _sa) {
                    _sa.DataRxEvent -= OnDataRxEvent;
                    _sa.AgentClose();
                }
                _sa = sa;
                sa.AgentOpen();
                sa.DataRxEvent += OnDataRxEvent;
            } catch (Exception ex) {
                return ChangeFlag.Failed;
            }
            return ChangeFlag.Succeed;
        }

        public ChangeFlag SetNewLine(NewLine newLine, long? length, char? breakChar)
        {
            _newLine = newLine;
            return SetBreakLineMethod(newLine, length, breakChar);
        }

        ChangeFlag SetBreakLineMethod(NewLine newLine, long? length, char? breakChar)
        {
            var flag = ChangeFlag.Succeed;
            switch (newLine) {
                case NewLine.Nt: //Windows Nt
                    _isLineBreak = IsLineBreakForNt;
                    break;

                case NewLine.Linux: //Linux
                    _isLineBreak = IsLineBreakForLinux;
                    break;

                case NewLine.Mac: //Mac
                    _isLineBreak = IsLineBreakForMac;
                    break;

                case NewLine.Length: //stream length
                    if (null != length) {
                        _index = 0;
                        _length = length.Value;
                        _isLineBreak = IsLineBreakForLength;
                    } else {
                        flag = ChangeFlag.Failed;
                    }
                    break;

                case NewLine.Char: //break char
                    if (null != breakChar) {
                        _breakChar = breakChar;
                        _isLineBreak = IsLineBreakForChar;
                    } else {
                        flag = ChangeFlag.Failed;
                    }
                    break;

                default:
                    flag = ChangeFlag.Failed;
                    break;
            }
            return flag;
        }


        bool IsLineBreakForNt(byte what)
        {
            if (_preByte == null || 13 != (int)_preByte) return false;
            return 10 == what;
        }

        bool IsLineBreakForLinux(byte what)
        {
            return 10 == what;
        }

        bool IsLineBreakForMac(byte what)
        {
            return 13 == what;
        }

        bool IsLineBreakForLength(byte what)
        {
            return _length == _index;
        }

        bool IsLineBreakForChar(byte what)
        {
            return what == _breakChar;
        }
    }
}
