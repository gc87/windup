using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Windup.SerialTalker
{
    public class DataHub
    {
        public delegate void TransferData(string port, object data);

        TransferData _transfer;
        readonly Thread _readThread;
        readonly List<SerialAgent> _saList;

        public DataHub()
        {
            _saList = new List<SerialAgent>();
            _readThread = new Thread(new ThreadStart(ReadQueue));
        }

        public void SetProcessMothod(TransferData transfer)
        {
            _transfer += transfer;
        }

        public void AddSerialAgent(SerialAgent sa)
        {
            _saList.Add(sa);
        }

        public void AddSerialAgents(IEnumerable<SerialAgent> list)
        {
            foreach (var sa in list) {
                _saList.Add(sa);
            }
        }

        public List<string> GetOnlineList()
        {
            return _saList.Where(sa => false != sa.Enable).Select(sa => sa.AgentPortName).ToList();
        }

        public List<string> AllOnLine()
        {
            foreach (var sa in _saList) {
                sa.AgentOpen();
            }
            return GetOnlineList();
        }

        public List<string> TakeOnLineByPortName(string portName)
        {
            var list = _saList.Where(sa => portName.Equals(sa.AgentPortName)).ToList();
            if (list.Any())
                list.First().AgentOpen();
            return GetOnlineList();
        }

        public void AllOffLine()
        {
            if (null != _readThread && _readThread.IsAlive)
                _readThread.Abort();
            foreach (var sa in _saList) {
                sa.AgentClose();
            }
        }

        public List<string> TakeOffLineByPortName(string portName)
        {
            var list = _saList.Where(sa => portName == sa.AgentPortName).ToList();
            if (list.Any())
                list.First().AgentClose();
            return GetOnlineList();
        }


        void ReadQueue()
        {
            do {
                foreach (var sa in _saList.Where(sa => false != sa.Enable)) {
                }
            } while (true);
        }

        public WriteFlagEnum WriteByPortName(string portName, byte[] what)
        {
            var list = _saList.Where(sa => sa.AgentPortName.Equals(portName) && true == sa.Enable);
            var serialAgents = list as SerialAgent[] ?? list.ToArray();
            return serialAgents.Any() ? serialAgents.First().AgentWrite(what) : WriteFlagEnum.Exception;
        }
    }
}
