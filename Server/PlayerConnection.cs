using System.Net;
using System.Net.Sockets;
using Common;
using Common.Objects;

public class PlayerConnection {
	public Player Player { get; }
	public TcpClient Client { get; }

	public NetworkStream Stream { get; }
	public BinaryReader Reader { get; }
	public BinaryWriter Writer { get; }

	public IPEndPoint IP => (IPEndPoint) Client.Client.RemoteEndPoint!;

	public PlayerConnection(Player player, TcpClient client) {
		Player = player;
		Client = client;

		Stream = Client.GetStream();
		Writer = new BinaryWriter(Stream);
		Reader = new BinaryReader(Stream);
	}

	public void SendPacket(IPacket packet) {
		Console.Out.WriteLine($"[S -> C] {IP.Address} {packet}");
		packet.Write(Writer);
	}
}
