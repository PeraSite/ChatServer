using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using Common;
using Common.Objects;
using Packets.Client;
using Packets.Server;

public class TestClient {
	private readonly TcpClient _client;

	private readonly NetworkStream _stream;
	private readonly BinaryReader _reader;
	private readonly BinaryWriter _writer;

	private readonly string _name;
	public Player? _player;
	private Stopwatch _stopwatch;

	public TestClient(string name, string ip, int port) {
		_name = name;

		_client = new TcpClient();
		_client.Connect(ip, port);

		_stream = _client.GetStream();
		_writer = new BinaryWriter(_stream);
		_reader = new BinaryReader(_stream);

		_stopwatch = new Stopwatch();
		_stopwatch.Start();
	}

	~TestClient() {
		Stop();
	}

	public void Start() {
		// 패킷 리스너 스레드 시작
		StartListeningPackets();

		// 클라이언트의 name을 담은 Handshake 전송
		SendPacket(new ClientHandshakePacket(_name));
	}

	public void Stop() {
		_stopwatch.Stop();
		_writer.Close();
		_reader.Close();
		_stream.Close();
		_client.Close();
	}

	private void StartListeningPackets() {
		// Stream#Read는 Blocking Call이기 때문에, 비동기 Task로 처리
		Task.Run(() => {
			while (_client.Connected) {
				// 패킷 타입 읽어오기
				var packetID = _reader.ReadByte();
				var packetType = (PacketType) packetID;
				var basePacket = packetType.CreatePacket(_reader);

				switch (basePacket) {
					case ServerTextPacket packet: {
						//내가 보낸 패킷의 답장이 왔다면
						if (packet.Player == _player) {
							Console.Out.WriteLine($"Stopping client {_name}");
							Stop();
						}
						break;
					}
					case ServerHandshakePacket packet: {
						_player = packet.Player;
						Chat("SOme random message"); // Debug.Log($"Setting Player to {packet.Player}");
						Console.Out.WriteLine($"Server handshake takes {_stopwatch.ElapsedMilliseconds}ms");
						break;
					}
				}
			}
		}).ContinueWith(t => {
			if (t.IsFaulted) throw t.Exception!;
		});
	}

	public void Chat(string msg) {
		Console.Out.WriteLine($"{_name} send: {msg}");
		SendPacket(new ClientTextPacket(msg));
	}

	private void SendPacket(IPacket packet) {
		packet.Write(_writer);
	}
}
