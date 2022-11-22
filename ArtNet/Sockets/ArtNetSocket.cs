using ArtNet.IO;
using ArtNet.Packets;
using System.Net.Sockets;
using System.Net;
using System;

namespace ArtNet.Sockets;

public class ArtNetSocket : Socket
{
    public const int Port = 6454;

    public event UnhandledExceptionEventHandler UnhandledException;
    public event EventHandler<NewPacketEventArgs<ArtNetPacket>> NewPacket;


    public ArtNetSocket()
        : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
    {
    }

    #region Information

    public bool PortOpen { get; set; }

    public IPAddress LocalIp { get; protected set; }

    public IPAddress LocalSubnetMask { get; protected set; }

    private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
    {
        var ipAddressBytes = address.GetAddressBytes();
        var subnetMaskBytes = subnetMask.GetAddressBytes();

        if (ipAddressBytes.Length != subnetMaskBytes.Length)
            throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

        var broadcastAddress = new byte[ipAddressBytes.Length];
        for (var i = 0; i < broadcastAddress.Length; i++)
        {
            broadcastAddress[i] = (byte)(ipAddressBytes[i] | (subnetMaskBytes[i] ^ 255));
        }

        return new IPAddress(broadcastAddress);
    }

    public IPAddress BroadcastAddress =>
        LocalSubnetMask == null ? IPAddress.Broadcast : GetBroadcastAddress(LocalIp, LocalSubnetMask);

    public DateTime? LastPacket { get; protected set; }

    #endregion

    public void Open(IPAddress localIp, IPAddress localSubnetMask)
    {
        LocalIp = localIp;
        LocalSubnetMask = localSubnetMask;

        SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Bind(new IPEndPoint(LocalIp, Port));
        SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        PortOpen = true;

        StartReceive();
    }

    public void StartReceive()
    {
        try
        {
            EndPoint localPort = new IPEndPoint(IPAddress.Any, Port);
            var receiveState = new ArtNetReceiveData();
            BeginReceiveFrom(receiveState.Buffer, 0, receiveState.BufferSize, SocketFlags.None, ref localPort,
                OnReceive, receiveState);
        }
        catch (Exception ex)
        {
            OnUnhandledException(new ApplicationException("An error ocurred while trying to start recieving ArtNet.",
                ex));
        }
    }

    private void OnReceive(IAsyncResult state)
    {
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        if (PortOpen)
        {
            try
            {
                var receiveState = (ArtNetReceiveData)(state.AsyncState);

                if (receiveState != null)
                {
                    receiveState.DataLength = EndReceiveFrom(state, ref remoteEndPoint);

                    //Protect against UDP loopback where we receive our own packets.
                    if (LocalEndPoint != remoteEndPoint && receiveState.Valid)
                    {
                        LastPacket = DateTime.Now;

                        ProcessPacket((IPEndPoint)remoteEndPoint, ArtNetPacket.Create(receiveState));
                    }
                }
            }
            catch (Exception ex)
            {
                OnUnhandledException(ex);
            }
            finally
            {
                //Attempt to receive another packet.
                StartReceive();
            }
        }
    }

    private void ProcessPacket(IPEndPoint source, ArtNetPacket packet)
    {
        if (packet != null)
        {
            NewPacket?.Invoke(this, new NewPacketEventArgs<ArtNetPacket>(source, packet));
        }
    }

    protected void OnUnhandledException(Exception ex)
    {
        UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
    }

    #region Sending

    public void Send(ArtNetPacket packet)
    {
        SendTo(packet.ToArray(), new IPEndPoint(BroadcastAddress, Port));
    }

    #endregion

    protected override void Dispose(bool disposing)
    {
        PortOpen = false;

        base.Dispose(disposing);
    }
}