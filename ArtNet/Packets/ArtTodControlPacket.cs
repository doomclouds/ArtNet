using ArtNet.Enums;
using ArtNet.IO;

namespace ArtNet.Packets;

public class ArtTodControlPacket : ArtNetPacket
{
    public ArtTodControlPacket() : base(ArtNetOpCodes.TodControl)
    {
    }

    public ArtTodControlPacket(ArtNetReceiveData data)
        : base(data)
    {

    }

    #region Packet Properties

    public byte Net { get; set; }

    public ArtTodControlCommand Command { get; set; }

    public byte Address { get; set; }


    #endregion

    public override void ReadData(ArtNetBinaryReader data)
    {
        base.ReadData(data);

        data.BaseStream.Seek(9, System.IO.SeekOrigin.Current);
        Net = data.ReadByte();
        Command = (ArtTodControlCommand)data.ReadByte();
        Address = data.ReadByte();
    }

    public override void WriteData(ArtNetBinaryWriter data)
    {
        base.WriteData(data);

        data.Write(new byte[9]);
        data.Write(Net);
        data.Write((byte)Command);
        data.Write(Address);
    }
}