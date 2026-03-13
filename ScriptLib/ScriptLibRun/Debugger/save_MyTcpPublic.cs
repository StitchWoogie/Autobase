using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace ScriptLibRun.Debugger
{
    public class MyTcpPublic
    {
        protected void SendACK(Socket socket, int trans)
        {
            byte[] data = new byte[6];

            data[0] = (byte)EnumTcpCommand.ACK;
            data[1] = 0;
            data[2] = (byte)((trans >> 24) & 0x100);
            data[3] = (byte)((trans >> 16) & 0x100);
            data[4] = (byte)((trans >> 8) & 0x100);
            data[5] = (byte)((trans >> 0) & 0x100);

            socket.Send(data, 6, SocketFlags.None);
        }

        protected void SendNAK(Socket socket, int trans, EnumNakType error_type)
        {
            byte[] data = new byte[6];

            data[0] = (byte)EnumTcpCommand.NAK;
            data[1] = (byte)error_type;
            data[2] = (byte)((trans >> 24) & 0x100);
            data[3] = (byte)((trans >> 16) & 0x100);
            data[4] = (byte)((trans >> 8) & 0x100);
            data[5] = (byte)((trans >> 0) & 0x100);

            socket.Send(data, 6, SocketFlags.None);
        }
    }

    enum EnumTcpCommand : byte
    {
        MultiFrameStart = 1,
        MultiFrameContinue = 2,
        MultiFrameEnd = 3,           //
        SingleBlock = 4,
        Closed = 5,
        ACK = 6,
        NAK = 7,
    }

    public enum EnumNakType : byte
    {
        Unknown = 0,
        FirstFrameNoIsNotZero = 1,
        NextFrameNoMismatched = 2,
        MultiFrameNotStarted = 3,       // Mu
        MultiFrameOverTotalSize = 4,       // Mu
        DataSizeMismatched = 5,       // Mu
        UnKnownCommand = 6,
    }
}
