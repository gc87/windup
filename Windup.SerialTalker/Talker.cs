using System;
using System.Linq;
using System.IO.Ports;
using System.Collections.Generic;

namespace Windup.SerialTalker
{
	public class Talker
	{
		//处理到达数据的回调函数，Talker的最核心的处理
		public delegate void ByteListProc (List<byte> bytes);

		ByteListProc _byteListProc;

		SerialAgent _sa;
		Analyzer _analyzer;

		public Talker ()
		{
			LineBreak = new LineBreak () {
				Type = "linux"
			};

			_sa = new SerialAgent ("INIT");
			_analyzer = new Analyzer ();
			_analyzer.SetLineBreak (LineBreak);
			_analyzer.DataListReadyEvent += OnDataListReady;
		}

		public string PortName {
			get { 
				return _sa.AgentPortName;
			}
			set { 
				_sa.AgentPortName = value;
			}
		}

		public int BaudRate {
			get {
				return _sa.AgentBaudRate;
			}
			set {
				_sa.AgentBaudRate = value;
				_analyzer.SetAgent (_sa);
			}
		}

		public Parity Parity {
			get {
				return _sa.AgentParity;
			}
			set {
				_sa.AgentParity = value;
				_analyzer.SetAgent (_sa);
			}
		}

		public int DataBits {
			get {
				return _sa.AgentDataBits;
			}
			set {
				_sa.AgentDataBits = value;
				_analyzer.SetAgent (_sa);
			}
		}

		public StopBits StopBits {
			get {
				return _sa.AgentStopBits;
			}
			set {
				_sa.AgentStopBits = value;
				_analyzer.SetAgent (_sa);
			}
		}

		public ByteListProc Proc {
			get {
				return _byteListProc;
			}
			set { 
				_byteListProc += value;
			}
		}

		public LineBreak LineBreak {
			get;
			set;
		}

		/// <summary>
		/// Write the specified what.
		/// </summary>
		/// <param name="what">What.</param>
		public bool Write (byte[] what)
		{
			var status = _sa.AgentWrite (what);
			return 0 == status ? true : false;
		}

		/// <summary>
		/// Analyzer中的串口返回的字节列表到达时的数据处理函数.
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="e">E.</param>
		void OnDataListReady (object o, DataListReadyEventArgs e)
		{
			_byteListProc (e.Bytes);
		}

		/// <summary>
		/// Gets the serial ports.
		/// </summary>
		/// <returns>The serial ports.</returns>
		public string[] GetSerialPorts ()
		{
			string[] allSerial = null;
			try {
				allSerial = SerialPort.GetPortNames ();
			} catch { /*(Exception e)*/
				//throw new Exception(e.Message);
				return null;
			}
			return allSerial;
		}

		/// <summary>
		/// Touchs the agent port.
		/// </summary>
		/// <returns><c>true</c>, if agent port was touched, <c>false</c> otherwise.</returns>
		/// <param name="portName">Port name.</param>
		/// <param name="baudRate">Baud rate.</param>
		public static bool TouchAgentPort(string portName, int baudRate)
		{
			return SerialAgent.TouchAgentPort (portName, baudRate);
		}
	}
}
