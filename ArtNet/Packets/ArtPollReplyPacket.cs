using ArtNet.Enums;
using ArtNet.IO;
using System;
using ArtNet.Sockets;

namespace ArtNet.Packets;

[Flags]
public enum PollReplyStatus
{
    None = 0,
    UBEA = 1,
    RdmCapable = 2,
    ROMBoot = 4
}

public class ArtPollReplyPacket : ArtNetPacket
{
    public ArtPollReplyPacket() : base(ArtNetOpCodes.PollReply)
    {
    }

    public ArtPollReplyPacket(ArtNetReceiveData data)
        : base(data)
    {
    }

    #region Packet Properties

    private byte[] _ipAddress = new byte[4];

    public byte[] IpAddress
    {
        get => _ipAddress;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The IP address must be an array of 4 bytes.");

            _ipAddress = value;
        }
    }

    public short Port { get; set; } = ArtNetSocket.Port;

    public short FirmwareVersion { get; set; }


    public short SubSwitch { get; set; }

    public short Oem { get; set; } = 0xff;

    public byte UbeaVersion { get; set; }

    public PollReplyStatus Status { get; set; } = 0;

    public short EstaCode { get; set; }

    public string ShortName { get; set; } = string.Empty;

    public string LongName { get; set; } = string.Empty;

    public string NodeReport { get; set; } = string.Empty;

    public short PortCount { get; set; }

    private byte[] _portTypes = new byte[4];

    public byte[] PortTypes
    {
        get => _portTypes;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The port types must be an array of 4 bytes.");

            _portTypes = value;
        }
    }

    private byte[] _goodInput = new byte[4];

    public byte[] GoodInput
    {
        get => _goodInput;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The good input must be an array of 4 bytes.");

            _goodInput = value;
        }
    }

    private byte[] _goodOutput = new byte[4];

    public byte[] GoodOutput
    {
        get => _goodOutput;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The good output must be an array of 4 bytes.");

            _goodOutput = value;
        }
    }

    public byte[] SwIn { get; set; } = new byte[4];

    public byte[] SwOut { get; set; } = new byte[4];

    public byte SwVideo { get; set; }

    public byte SwMacro { get; set; }

    public byte SwRemote { get; set; }

    public byte Style { get; set; }

    private byte[] _macAddress = new byte[6];

    public byte[] MacAddress
    {
        get => _macAddress;
        set
        {
            if (value.Length != 6)
                throw new ArgumentException("The mac address must be an array of 6 bytes.");

            _macAddress = value;
        }
    }

    private byte[] _bindIpAddress = new byte[4];

    public byte[] BindIpAddress
    {
        get => _bindIpAddress;
        set
        {
            if (value.Length != 4)
                throw new ArgumentException("The bind IP address must be an array of 4 bytes.");

            _bindIpAddress = value;
        }
    }

    public byte BindIndex { get; set; }

    public byte Status2 { get; set; }

    #endregion

    #region Packet Helpers

    /// <summary>
    /// Interprets the universe address to ensure compatibility with ArtNet I, II and III devices.
    /// </summary>
    /// <param name="outPorts">Whether to get the address for in or out ports.</param>
    /// <param name="portIndex">The port index to obtain the universe for.</param>
    /// <returns>The 15 Bit universe address</returns>
    public int UniverseAddress(bool outPorts, int portIndex)
    {
        int universe;

        if (SubSwitch > 0)
        {
            universe = (SubSwitch & 0x7F00);
            universe += (SubSwitch & 0x0F) << 4;
            universe += (outPorts ? SwOut[portIndex] : SwIn[portIndex]) & 0xF;
        }
        else
        {
            universe = (outPorts ? SwOut[portIndex] : SwIn[portIndex]);
        }

        return universe;
    }

    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        IpAddress = data.ReadBytes(4);
        Port = data.ReadInt16();
        FirmwareVersion = data.ReadNetwork16();
        SubSwitch = data.ReadNetwork16();
        Oem = data.ReadNetwork16();
        UbeaVersion = data.ReadByte();
        Status = (PollReplyStatus)data.ReadByte();
        EstaCode = data.ReadNetwork16();
        ShortName = data.ReadNetworkString(18);
        LongName = data.ReadNetworkString(64);
        NodeReport = data.ReadNetworkString(64);
        PortCount = data.ReadNetwork16();
        PortTypes = data.ReadBytes(4);
        GoodInput = data.ReadBytes(4);
        GoodOutput = data.ReadBytes(4);
        SwIn = data.ReadBytes(4);
        SwOut = data.ReadBytes(4);
        SwVideo = data.ReadByte();
        SwMacro = data.ReadByte();
        SwRemote = data.ReadByte();
        data.ReadBytes(3);
        Style = data.ReadByte();
        MacAddress = data.ReadBytes(6);
        BindIpAddress = data.ReadBytes(4);
        BindIndex = data.ReadByte();
        Status2 = data.ReadByte();
    }

    public override void WriteData(ArtNetBinaryWriter data)
    {
        base.WriteData(data);

        data.Write(IpAddress);
        data.Write(Port);
        data.WriteNetwork(FirmwareVersion);
        data.WriteNetwork(SubSwitch);
        data.WriteNetwork(Oem);
        data.Write(UbeaVersion);
        data.Write((byte)Status);
        data.Write(EstaCode);
        data.WriteNetwork(ShortName, 18);
        data.WriteNetwork(LongName, 64);
        data.WriteNetwork(NodeReport, 64);
        data.WriteNetwork(PortCount);
        data.Write(PortTypes);
        data.Write(GoodInput);
        data.Write(GoodOutput);
        data.Write(SwIn);
        data.Write(SwOut);
        data.Write(SwVideo);
        data.Write(SwMacro);
        data.Write(SwRemote);
        data.Write(new byte[3]);
        data.Write(Style);
        data.Write(MacAddress);
        data.Write(BindIpAddress);
        data.Write(BindIndex);
        data.Write(Status2);
        data.Write(new byte[208]);
    }
}