using System.Net.Sockets;
using Common;
using Common.Objects;
using Packets.Client;
using Packets.Server;

public class ChatClient {
	private readonly TcpClient _client;

	private readonly NetworkStream _stream;
	private readonly BinaryReader _reader;
	private readonly BinaryWriter _writer;

	private Player _player;

	public ChatClient(string name, string ip, int port) {
		_client = new TcpClient();
		_client.Connect(ip, port);

		_stream = _client.GetStream();
		_writer = new BinaryWriter(_stream);
		_reader = new BinaryReader(_stream);

		// Start listening for incoming packets
		Task.Run(() => {
			while (true) {
				var packetID = _reader.ReadByte();
				var packetType = (PacketType) packetID;

				switch (packetType) {
					case PacketType.Client_Text: {
						var packet = new ClientTextPacket(_reader);
						Debug.Log($"[S -> C] {packet}");
						break;
					}
					case PacketType.Server_Text: {
						var packet = new ServerTextPacket(_reader);
						Console.WriteLine($"{packet.Player.Name}: {packet.Text}");
						break;
					}
					case PacketType.Server_Handshake: {
						var packet = new ServerHandshakePacket(_reader);
						_player = packet.Player;
						Debug.Log($"Setting Player to {packet.Player}");
						break;
					}
					case PacketType.Client_Handshake:
						break;
					case PacketType.Client_PlayerList:
						break;
					case PacketType.Server_PlayerList: {
						var packet = new ServerPlayerListPacket(_reader);
						Console.Out.WriteLine($"접속한 유저({packet.Players.Count}명) : ");
						packet.Players.ForEach(targetPlayer => { Console.Out.WriteLine($"- {targetPlayer.Name}"); });
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}).ContinueWith(t => {
			if (t.IsFaulted) throw t.Exception!;
		});

		SendPacket(new ClientHandshakePacket(name));

		while (true) {
			var line = Console.ReadLine();
			if (string.IsNullOrEmpty(line)) break;
			if (line.StartsWith("/")) {
				var command = line[1..];
				if (command.StartsWith("list")) {
					SendPacket(new ClientPlayerListPacket());
				}
			} else {
				var packet = new ClientTextPacket(line);
				SendPacket(packet);
			}
		}
	}

	~ChatClient() {
		_writer.Close();
		_reader.Close();
		_stream.Close();
		_client.Close();
	}

	private void SendPacket(IPacket packet) {
		Debug.Log($"[C -> S] {packet}");
		packet.Write(_writer);
	}
}
