namespace ArtNet.IO;

public class ArtNetReceiveData
{
    public byte[] Buffer = new byte[1500];
    public int BufferSize = 1500;
    public int DataLength = 0;

    public bool Valid => DataLength > 12;

    public int OpCode => Buffer[9] + (Buffer[8] << 8);
}