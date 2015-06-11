using System;
using System.Linq;
using System.IO.Ports;
using System.Collections.Generic;
using System.Diagnostics;

namespace Windup.SerialTalker
{
	public class Talker
	{
		//处理到达数据的回调函数，Talker的最核心的处理
		public delegate void ByteListProc (List<byte> bytes);
		public bool IsOpen = false;

		ByteListProc _byteListProc;

		SerialAgent _sa;
		Analyzer _analyzer;
		LineBreak _lineBreak;

		public Talker()
		{
			PortName = " "; //whitespace
			BaudRate = 9600;
			Parity = Parity.None;
			DataBits = 8;
			StopBits = StopBits.One;
			_lineBreak = new LineBreak{ Type = "linux"};
			_byteListProc += list => {
			};
		}

		/// <summary>
		/// Open this instance.
		/// </summary>
		public void Open(){
			_sa = new SerialAgent (PortName, BaudRate, Parity, DataBits, StopBits);
			_analyzer = new Analyzer ();
			_analyzer.SetLineBreak (_lineBreak);
			_analyzer.DataListReadyEvent += OnDataListReady;
			_analyzer.SetAgent (_sa);
			IsOpen = true;
		}

		/// <summary>
		/// Close this instance.
		/// </summary>
		public void Close()
		{
			if (!IsOpen) {
				return;
			}
			_sa.AgentClose ();
			IsOpen = false;
		}

		/// <summary>
		/// Gets or sets the name of the port.
		/// </summary>
		/// <value>The name of the port.</value>
		public string PortName {
			get;  
			set; 
		}

		/// <summary>
		/// Gets or sets the baud rate.
		/// </summary>
		/// <value>The baud rate.</value>
		public int BaudRate {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the parity.
		/// </summary>
		/// <value>The parity.</value>
		public Parity Parity {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the data bits.
		/// </summary>
		/// <value>The data bits.</value>
		public int DataBits {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the stop bits.
		/// </summary>
		/// <value>The stop bits.</value>
		public StopBits StopBits {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the proc.
		/// </summary>
		/// <value>The proc.</value>
		public ByteListProc Proc {
			get {
				return _byteListProc;
			}
			set { 
				_byteListProc += value;
			}
		}

		/// <summary>
		/// Gets or sets the line break.
		/// </summary>
		/// <value>The line break.</value>
		public LineBreak LineBreak {
			get {
				return _lineBreak;
			}
			set {
				_lineBreak = value;
			}
		}

		/// <summary>
		/// Write the specified what.
		/// </summary>
		/// <param name="what">What.</param>
		public bool Write (byte[] what)
		{
			if (IsOpen) {
				var status = _sa.AgentWrite (what);
				return 0 == status ? true : false;
			}
			return false;
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
		public static string[] GetSerialPorts ()
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
		public static bool TouchSerialPort (string portName, int baudRate)
		{
			return SerialAgent.TouchAgentPort (portName, baudRate);
		}
	}
}
