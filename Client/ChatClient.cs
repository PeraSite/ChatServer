using System.Net.Sockets;
using Common;
using Packets.Client;
using Packets.Server;

public class ChatClient {
	private readonly TcpClient _client;

	private readonly NetworkStream _stream;
	private readonly BinaryReader _reader;
	private readonly BinaryWriter _writer;

	public ChatClient(string ip, int port) {
		_client = new TcpClient();
		_client.Connect(ip, port);

		_stream = _client.GetStream();
		_writer = new BinaryWriter(_stream);
		_reader = new BinaryReader(_stream);

		// Start listening for incoming packets
		Task.Factory.StartNew(() => {
			while (true) {
				var packetID = _reader.ReadByte();
				var packetType = (PacketType) packetID;

				switch (packetType) {
					case PacketType.Client_Text: {
						var packet = new ClientTextPacket(_reader);
						Console.WriteLine($"[S -> C] {packet}");
						break;
					}
					case PacketType.Server_Text: {
						var packet = new ServerTextPacket(_reader);
						Console.WriteLine($"[{packet.Player}] {packet.Text}");
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		});

		while (true) {
			var line = Console.ReadLine();
			if (string.IsNullOrEmpty(line)) break;
			var packet = new ClientTextPacket(line);
			SendPacket(packet);
		}
	}

	~ChatClient() {
		_writer.Close();
		_reader.Close();
		_stream.Close();
		_client.Close();
	}

	private void SendPacket(IPacket packet) {
		Console.Out.WriteLine($"[C -> S] {packet}");
		packet.Write(_writer);
	}
}
