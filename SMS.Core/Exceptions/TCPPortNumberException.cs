using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Exceptions
{
    public class TcpPortNumberException : Exception
    {
        public TcpPortNumberException(short tcpPortNumber) : base($"TCPPortNumber cannot be less than 1. Current Value : {tcpPortNumber}")
        {

        }
    }
}
