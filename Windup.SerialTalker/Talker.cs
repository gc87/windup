using System;
using System.Linq;
using System.IO.Ports;
using System.Collections.Generic;

namespace Windup.SerialTalker
{
	public class Talker
	{
		//处理到达数据的回调函数，Talker的最核心的处理
		public delegate void ByteListProc(List<byte> bytes);
		public ByteListProc byteListProc;

		public Talker ()
		{
		}

		public Talker(string portName, int baudRate, LineBreak lineBreak)
		{
			var sa = new SerialAgent (portName, baudRate);
			CreateAnalyzer (sa, lineBreak);
		}

		public Talker(SPSetting setting, LineBreak lineBreak)
		{
			var sa = CreateAgent(setting);
			CreateAnalyzer (sa, lineBreak);
		}

		/// <summary>
		/// Creates the analyzer.
		/// </summary>
		/// <returns>The analyzer.</returns>
		/// <param name="sa">Sa.</param>
		/// <param name="lineBreak">Line break.</param>
		Analyzer CreateAnalyzer(SerialAgent sa, LineBreak lineBreak)
		{
			var analyzer = new Analyzer ();
			analyzer.SetAgent (sa);
			analyzer.SetLineBreak (lineBreak);
			analyzer.DataListReadyEvent += OnDataListReady;
			return analyzer;
		}

		/// <summary>
		/// Analyzer中的串口返回的字节列表到达时的数据处理函数.
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="e">E.</param>
		void OnDataListReady(object o, DataListReadyEventArgs e)
		{
			byteListProc (e.Bytes);
		}

		/// <summary>
		/// Creates the agent.
		/// </summary>
		/// <returns>The agent.</returns>
		/// <param name="setting">Setting.</param>
		SerialAgent CreateAgent(SPSetting setting)
		{
			var sa = new SerialAgent() {
				AgentPortName = setting.PortName,
				AgentBaudRate = setting.BaudRate,
				AgentParity = setting.Parity,
				AgentDataBits = setting.DataBits,
				AgentStopBits = setting.StopBits
			};
			return sa;
		}

		/// <summary>
		/// Creates the agents.
		/// </summary>
		/// <returns>The agents.</returns>
		/// <param name="settings">Settings.</param>
		IEnumerable<SerialAgent> CreateAgents(IEnumerable<SPSetting> settings)
		{
			var list = new List<SerialAgent>();
			foreach (var sa in settings.Select(setting => new SerialAgent(
				portName: setting.PortName,
				baudRate: setting.BaudRate,
				parity: setting.Parity,
				dataBits: setting.DataBits,
				stopBits: setting.StopBits
			))) {
				list.Add(sa);
			}
			return list;
		}

		/// <summary>
		/// Gets the serial ports.
		/// </summary>
		/// <returns>The serial ports.</returns>
		public string[] GetSerialPorts()
		{
			string[] allSerial = null;
			try
			{
				allSerial = SerialPort.GetPortNames();
			}
			catch /*(Exception e)*/
			{
				//throw new Exception(e.Message);
				return null;
			}
			return allSerial;
		}
	}
}

