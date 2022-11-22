using ArtNet.Enums;
using ArtNet.IO;

namespace ArtNet.Packets;

public class ArtPollPacket : ArtNetPacket
{
    public ArtPollPacket() : base(ArtNetOpCodes.Poll)
    {
    }

    public ArtPollPacket(ArtNetReceiveData data)
        : base(data)
    {

    }

    #region Packet Properties

    public byte TalkToMe { get; set; }

    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        TalkToMe = data.ReadByte();
    }

    public override void WriteData(ArtNetBinaryWriter data)
    {
        base.WriteData(data);

        data.Write(TalkToMe);
        data.Write((byte)0);
    }
}