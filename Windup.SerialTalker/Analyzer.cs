using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windup.SerialTalker
{
	public enum LineBreak
	{
		Linux, //"\n"
		Mac, //"\r"
		Nt, //"\r\n"
		Length, //length
		Char //char
	};

	public enum ChangeFlag
	{
		Succeed,
		Failed
	}

    class Analyzer
    {
        private delegate bool IsLineBreak(byte what);
        private IsLineBreak _isLineBreak;
        private SerialAgent _sa;
        private LineBreak _lineBreak = 0;
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

		/// <summary>
		/// 数据到达时候的事件处理函数.
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="e">E.</param>
        void OnDataRxEvent(object o, DataRxEventArgs e)
        {
            if (_isLineBreak(e.Data)) {
                _index = 0;
                _preByte = null;
                if (LineBreak.Nt == _lineBreak)
                    _list.RemoveAt(_list.Count - 1);
                if (LineBreak.Length == _lineBreak)
                    _list.Add(e.Data);
                DataListReadyEvent(this, new DataListReadyEventArgs(_list));
                _list = new List<byte>();
            } else {
                _index++;
                _preByte = e.Data;
                _list.Add(e.Data);
            }
        }

		/// <summary>
		/// 设置SerialAgent
		/// </summary>
		/// <returns>The agent.</returns>
		/// <param name="sa">Sa.</param>
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
            } catch /*(Exception ex)*/ {
                return ChangeFlag.Failed;
            }
            return ChangeFlag.Succeed;
        }

		/// <summary>
		/// 设置断句函数.
		/// </summary>
		/// <returns>The break line.</returns>
		/// <param name="LineBreak">New line.</param>
		/// <param name="length">Length.</param>
		/// <param name="breakChar">Break char.</param>
        public ChangeFlag SetLineBreak(LineBreak LineBreak, long? length, char? breakChar)
        {
            var flag = ChangeFlag.Succeed;
            switch (LineBreak) {
                case LineBreak.Nt: //Windows Nt
                    _isLineBreak = IsLineBreakForNt;
                    break;

                case LineBreak.Linux: //Linux
                    _isLineBreak = IsLineBreakForLinux;
                    break;

                case LineBreak.Mac: //Mac
                    _isLineBreak = IsLineBreakForMac;
                    break;

                case LineBreak.Length: //stream length
                    if (null != length) {
                        _index = 0;
                        _length = length.Value;
                        _isLineBreak = IsLineBreakForLength;
                    } else {
                        flag = ChangeFlag.Failed;
                    }
                    break;

                case LineBreak.Char: //break char
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


		/// <summary>
		/// 是否通过NT系统的回车符断句.
		/// </summary>
		/// <returns><c>true</c> if this instance is line break for nt the specified what; otherwise, <c>false</c>.</returns>
		/// <param name="what">What.</param>
        bool IsLineBreakForNt(byte what)
        {
            if (_preByte == null || 13 != (int)_preByte) return false;
            return 10 == what;
        }

		/// <summary>
		/// 是否通过linux的回车符断句.
		/// </summary>
		/// <returns><c>true</c> if this instance is line break for linux the specified what; otherwise, <c>false</c>.</returns>
		/// <param name="what">What.</param>
        bool IsLineBreakForLinux(byte what)
        {
            return 10 == what;
        }

		/// <summary>
		///  是否通过Mac的回车符断句.
		/// </summary>
		/// <returns><c>true</c> if this instance is line break for mac the specified what; otherwise, <c>false</c>.</returns>
		/// <param name="what">What.</param>
        bool IsLineBreakForMac(byte what)
        {
            return 13 == what;
        }

		/// <summary>
		/// 是否通过字节长度断句.
		/// </summary>
		/// <returns><c>true</c> if this instance is line break for length the specified what; otherwise, <c>false</c>.</returns>
		/// <param name="what">What.</param>
        bool IsLineBreakForLength(byte what)
        {
            return _length == _index;
        }

		/// <summary>
		/// 是否通过字符标志断句.
		/// </summary>
		/// <returns><c>true</c> if this instance is line break for char the specified what; otherwise, <c>false</c>.</returns>
		/// <param name="what">What.</param>
        bool IsLineBreakForChar(byte what)
        {
            return what == _breakChar;
        }
    }
}
