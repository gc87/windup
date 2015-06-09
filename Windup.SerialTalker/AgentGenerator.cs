using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Windup.SerialTalker
{
    static class AgentGenerator
    {
		
		public static SerialAgent CreateAgent(SPSetting setting)
        {
            Debug.Assert(null != setting);
            var sa = new SerialAgent() {
                AgentPortName = setting.PortName,
                AgentBaudRate = setting.BaudRate,
                AgentParity = setting.Parity,
                AgentDataBits = setting.DataBits,
                AgentStopBits = setting.StopBits
            };
            return sa;
        }

		public static IEnumerable<SerialAgent> CreateAgents(IEnumerable<SPSetting> settings)
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
    }
}
