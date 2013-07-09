using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TangramTheatre.SharedObjects
{
    public class Robot
    {
        private readonly IPAddress _ip;
        private readonly IPEndPoint _endPoint;

        public Robot(string ip)
        {
            this._ip = IPAddress.Parse(ip);
            _endPoint = new IPEndPoint(this._ip, Port);
        }

        public IPAddress IP
        {
            get { return _ip; }
        }

        public int Port
        {
            get { return 5000; }
        }

        public IPEndPoint EndPoint
        {
            get { return _endPoint; }
        }

        public static void Move(Robot robot, int direction, int velocity)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var sendBuffer = Encoding.ASCII.GetBytes(String.Format("move {0} {1}", direction, velocity));
            socket.SendTo(sendBuffer, robot.EndPoint);
        }

        public static void Stop(Robot robot)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var sendBuffer = Encoding.ASCII.GetBytes("move 0 0");
            socket.SendTo(sendBuffer, robot.EndPoint);
        }

        public static void Turn(Robot robot, int direction, int velocity)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var sendBuffer = Encoding.ASCII.GetBytes(String.Format("turn {0} {1}", direction, velocity));
            socket.SendTo(sendBuffer, robot.EndPoint);
        }
    }
}
