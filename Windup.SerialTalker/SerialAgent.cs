using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace Windup.SerialTalker
{
    public sealed class SerialAgent
    {
        readonly SerialPort _serial;
        Thread _readRunner;
        public bool Enable = false;
        public event EventHandler<DataRxEventArgs> DataRxEvent;

        #region Define SerialAgent Attribute
        public string AgentPortName
        {
            set { if (!_serial.IsOpen) _serial.PortName = value; }
            get { return _serial.PortName; }
        }

        /// <summary>
        /// Set and get baudrate on running
        /// </summary>
        public int AgentBaudRate
        {
            set { if (!_serial.IsOpen) _serial.BaudRate = value; }
            get { return _serial.BaudRate; }
        }

        /// <summary>
        /// Set and get parity on running
        /// </summary>
        public Parity AgentParity
        {
            set { if (!_serial.IsOpen) _serial.Parity = value; }
            get { return _serial.Parity; }
        }

        /// <summary>
        /// Set and get databits on running
        /// </summary>
        public int AgentDataBits
        {
            set { if (!_serial.IsOpen) _serial.DataBits = value; }
            get { return _serial.DataBits; }
        }

        /// <summary>
        /// Set and get stopbits on running
        /// </summary>
        public StopBits AgentStopBits
        {
            set { if (!_serial.IsOpen) _serial.StopBits = value; }
            get { return _serial.StopBits; }
        }

        /// <summary>
        /// Set and get handshake on running
        /// </summary>
        public Handshake AgentHandshake
        {
            set { if (!_serial.IsOpen) _serial.Handshake = value; }
            get { return _serial.Handshake; }
        }

        /// <summary>
        /// Set and get read timeout on running
        /// </summary>
        public int AgentReadTimeout
        {
            set { if (!_serial.IsOpen) _serial.ReadTimeout = value; }
            get { return _serial.ReadTimeout; }
        }

        /// <summary>
        /// Set and get write timeout on running
        /// </summary>
        public int AgentWriteTimeout
        {
            set { if (!_serial.IsOpen) _serial.WriteTimeout = value; }
            get { return _serial.WriteTimeout; }
        }
        #endregion

        #region Define private member funtion
        /// <summary>
        /// Set read and write timeout
        /// </summary>
        void DefaultTimeoutSet()
        {
            _serial.ReadTimeout = 500;
            _serial.WriteTimeout = 500;
        }

        /// <summary>
        /// Thread function of read data buffer 
        /// </summary>
        void ReadThread()
        {
            do {
                if (_serial.BytesToRead > 0) {
                    var result = (byte)_serial.ReadByte();
                    DataRxEvent(this, new DataRxEventArgs(result));
                } else {
                    Thread.Sleep(10);
                }
            } while (true);
        }

        /// <summary>
        /// Write data to buffer
        /// </summary>
        /// <param name="what">Data of write to serialport</param>
        /// <returns>Write is successed status</returns>
        WriteFlagEnum Write(byte[] what)
        {
            var flag = WriteFlagEnum.Successed;
            if (!_serial.IsOpen)
                flag = WriteFlagEnum.NotOpen;
            else {
                try {
                    _serial.Write(what, 0, what.Length);
                } catch {
                    flag = WriteFlagEnum.Exception;
                }
            }
            return flag;
        }

        #endregion

        #region Custructor
        /// <summary>
        /// SerialAgent Class Constraction Function
        /// </summary>
        public SerialAgent()
            : this(portName: "", baudRate: 9600, parity: Parity.None, dataBits: 8, stopBits: StopBits.One)
        {
        }

        public SerialAgent(string portName)
            : this(portName: portName, baudRate: 9600, parity: Parity.None, dataBits: 8, stopBits: StopBits.One)
        {
        }

        public SerialAgent(string portName, int baudRate)
            : this(portName: portName, baudRate: baudRate, parity: Parity.None, dataBits: 8, stopBits: StopBits.One)
        {
        }

        public SerialAgent(string portName, int baudRate, Parity parity)
            : this(portName: portName, baudRate: baudRate, parity: parity, dataBits: 8, stopBits: StopBits.One)
        {
        }

        public SerialAgent(string portName, int baudRate, Parity parity, int dataBits)
            : this(portName:portName, baudRate: baudRate, parity: parity, dataBits: dataBits, stopBits: StopBits.One)
        {
        }

        public SerialAgent(string portName = "", int baudRate = 9600, Parity parity = Parity.None,
                           int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            Debug.WriteLine(portName);
            _serial = string.IsNullOrEmpty(portName) ? null : new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            if (null == _serial) {
                Enable = false;
            } else {
                DefaultTimeoutSet();
                Enable = true;
            }
        }
        #endregion

        #region Define public member funtion
        /// <summary>
        /// Test Serialport is open
        /// </summary>
        /// <param name="portName">Serialport name of will be test</param>
        /// <param name="baudRate">Serialport baudrate of will be test</param>
        /// <returns>true of false</returns>
        public static bool TouchAgentPort(string portName, int baudRate)
        {
            var result = true;
            try {
                var sp = new SerialPort(portName, baudRate);
                if (!sp.IsOpen)
                    sp.Open();
                sp.Close();
            } catch (Exception) {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Open SerialAgent
        /// Open Serialport and start read buffer not no Windows
        /// </summary>
        public void AgentOpen()
        {
            if (_serial != null) _serial.Open();
            _readRunner = new Thread(new ThreadStart(ReadThread));
            _readRunner.Start();
            Enable = true;
        }

        /// <summary>
        /// Juged write function on the basis of opration system
        /// </summary>
        /// <param name="what">Data of will be send to serialport</param>
        /// <returns>WriteFalgEnum(0,1,2)</returns>
        public WriteFlagEnum AgentWrite(byte[] what)
        {
            return Write(what);
        }

        /// <summary>
        /// Close serialport
        /// </summary>
        public void AgentClose()
        {
            if (null != _readRunner && _readRunner.IsAlive)
                _readRunner.Abort();
            if (_serial.IsOpen)
                _serial.Close();
        }

        public override string ToString()
        {
            return string.Format("[SerialAgent: AgentPortName={0}, AgentBaudRate={1}, AgentParity={2}, AgentDataBits={3}, AgentStopBits={4}, AgentHandshake={5}, AgentReadTimeout={6}, AgentWriteTimeout={7}]",
                                  AgentPortName, AgentBaudRate, AgentParity, AgentDataBits,
                                  AgentStopBits, AgentHandshake, AgentReadTimeout, AgentWriteTimeout);
        }
        #endregion
    }
}
