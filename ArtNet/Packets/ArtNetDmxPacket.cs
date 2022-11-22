using ArtNet.Enums;
using ArtNet.IO;

namespace ArtNet.Packets;

public class ArtNetDmxPacket : ArtNetPacket
{
    public ArtNetDmxPacket() : base(ArtNetOpCodes.Dmx)
    {
    }

    public ArtNetDmxPacket(ArtNetReceiveData data)
        : base(data)
    {

    }

    #region Packet Properties

    public byte Sequence { get; set; }

    public byte Physical { get; set; }

    public short Universe { get; set; }

    public short Length
    {
        get
        {
            if (DmxData == null)
                return 0;
            return (short)DmxData.Length;
        }
    }

    public byte[] DmxData { get; set; }

    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        Sequence = data.ReadByte();
        Physical = data.ReadByte();
        Universe = data.ReadInt16();
        int length = data.ReadNetwork16();
        DmxData = data.ReadBytes(length);
    }

    public override void WriteData(ArtNetBinaryWriter data)
    {
        base.WriteData(data);

        data.Write(Sequence);
        data.Write(Physical);
        data.Write(Universe);
        data.WriteNetwork(Length);
        data.Write(DmxData);
    }
}