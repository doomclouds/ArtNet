using ArtNet.Packets;
using ArtNet.Sockets;

var artNet = new ArtNetSocket();
artNet.EnableBroadcast = true;

Console.WriteLine(artNet.BroadcastAddress.ToString());
// artNet.Open(IPAddress.Parse("192.168.1.58"), IPAddress.Parse("255.255.255.0"));
var dmxData = new byte[512];
for (var i = 0; i < dmxData.Length; i++)
{
    dmxData[i] = 0xff;
}

for (var i = 0; i < 10000; i++)
{
    Thread.Sleep(100);
    var dmx = new ArtNetDmxPacket
    {
        DmxData = dmxData,
        Universe = (short)i
    };
    artNet.Send(dmx);
}

Console.ReadKey();