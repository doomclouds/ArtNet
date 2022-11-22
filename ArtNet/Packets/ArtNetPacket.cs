using ArtNet.Enums;
using ArtNet.IO;
using System.IO;

namespace ArtNet.Packets;

public abstract class ArtNetPacket
{
    protected ArtNetPacket(ArtNetOpCodes opCode)
    {
        OpCode = opCode;
    }

    protected ArtNetPacket(ArtNetReceiveData data)
    {
        var packetReader = new ArtNetBinaryReader(new MemoryStream(data.Buffer));
        StartReadData(packetReader);
    }

    private void StartReadData(ArtNetBinaryReader packetReader)
    {
        ReadData(packetReader);
    }

    public byte[] ToArray()
    {
        var stream = new MemoryStream();
        WriteData(new ArtNetBinaryWriter(stream));
        return stream.ToArray();
    }

    #region Packet Properties

    private string _protocol = "Art-Net";

    public string Protocol
    {
        get => _protocol;
        protected set => _protocol = value.Length > 8
            ? value.Substring(0, 8)
            : value;
    }


    public short Version { get; protected set; } = 14;

    public ArtNetOpCodes OpCode { get; protected set; } = ArtNetOpCodes.None;

    #endregion

    public virtual void ReadData(ArtNetBinaryReader data)
    {
        Protocol = data.ReadNetworkString(8);
        OpCode = (ArtNetOpCodes)data.ReadNetwork16();

        //For some reason the poll packet header does not include the version.
        if (OpCode != ArtNetOpCodes.PollReply)
            Version = data.ReadNetwork16();
    }

    public virtual void WriteData(ArtNetBinaryWriter data)
    {
        data.WriteNetwork(Protocol, 8);
        data.WriteNetwork((short)OpCode);

        //For some reason the poll packet header does not include the version.
        if (OpCode != ArtNetOpCodes.PollReply)
            data.WriteNetwork(Version);
    }

    public static ArtNetPacket Create(ArtNetReceiveData data)
    {
        return (ArtNetOpCodes)data.OpCode switch
        {
            ArtNetOpCodes.Poll => new ArtPollPacket(data),
            ArtNetOpCodes.PollReply => new ArtPollReplyPacket(data),
            ArtNetOpCodes.Dmx => new ArtNetDmxPacket(data),
            ArtNetOpCodes.TodRequest => new ArtTodRequestPacket(data),
            ArtNetOpCodes.TodData => new ArtTodDataPacket(data),
            ArtNetOpCodes.TodControl => new ArtTodControlPacket(data),
            _ => null
        };
    }
}