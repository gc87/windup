using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Windup.SerialTalker
{
	enum ChangeFlag
	{
		Succeed,
		Failed
	}

    class Analyzer
    {
        private delegate bool IsLineBreak(byte what);
        private IsLineBreak _isLineBreak;
        private SerialAgent _sa;
        private LineBreak _lineBreak;
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

		/*
        protected virtual void OnDataListReadyEvent(DataListReadyEventArgs e)
        {
            EventHandler<DataListReadyEventArgs> handler = DataListReadyEvent;
            if (handler != null) handler(this, e);
        }
*/

        public Analyzer()
        {
        }

		public Analyzer(SerialAgent sa)
		{
			if (null == sa) {
				throw new ArgumentNullException();
			}
			_sa = sa;
			SetAgent(_sa);
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
				if ("nt" == _lineBreak.Type)
                    _list.RemoveAt(_list.Count - 1);
				if ("length" == _lineBreak.Type)
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
		public ChangeFlag SetLineBreak(LineBreak lineBreak)
		{
			var flag = ChangeFlag.Succeed;
			_lineBreak = lineBreak;
			switch (lineBreak.Type) {
			case "nt": //Windows Nt
				_isLineBreak = IsLineBreakForNt;
				break;

			case "linux": //Linux
				_isLineBreak = IsLineBreakForLinux;
				break;

			case "mac": //Mac
				_isLineBreak = IsLineBreakForMac;
				break;

			case "length": //stream length
				if (null != lineBreak.Length) {
					_index = 0;
					_length = lineBreak.Length.Value;
					_isLineBreak = IsLineBreakForLength;
				} else {
					flag = ChangeFlag.Failed;
				}
				break;

			case "char": //break char
				if (null != lineBreak.Char) {
					_breakChar = lineBreak.Char;
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
