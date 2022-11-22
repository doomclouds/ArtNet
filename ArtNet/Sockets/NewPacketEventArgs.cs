using System.Net;
using System;

namespace ArtNet.Sockets;

public class NewPacketEventArgs<TPacketType> : EventArgs
{
    public NewPacketEventArgs(IPEndPoint source, TPacketType packet)
    {
        Source = source;
        Packet = packet;
    }

    public IPEndPoint Source { get; protected set; }

    public TPacketType Packet { get; private set; }
}