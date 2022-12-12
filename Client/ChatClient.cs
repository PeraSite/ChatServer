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

	private readonly string _name;
	private Player? _player;

	public ChatClient(string name, string ip, int port) {
		_name = name;

		_client = new TcpClient();
		_client.Connect(ip, port);

		_stream = _client.GetStream();
		_writer = new BinaryWriter(_stream);
		_reader = new BinaryReader(_stream);
	}

	~ChatClient() {
		_writer.Close();
		_reader.Close();
		_stream.Close();
		_client.Close();
	}

	public void Start() {
		// 패킷 리스너 스레드 시작
		StartListeningPackets();

		// 클라이언트의 name을 담은 Handshake 전송
		SendPacket(new ClientHandshakePacket(_name));

		// 클라이언트 메인 루프
		while (_client.Connected) {
			// 유저 텍스트 입력받기
			var line = Console.ReadLine();
			if (string.IsNullOrEmpty(line)) continue;

			// 텍스트가 명령어인지 체크
			if (line.StartsWith("/")) {
				var command = line[1..].Trim().ToLower();

				switch (command) {
					case "list":
						// Player List 요청 패킷 전송
						SendPacket(new ClientPlayerListPacket());
						break;
					case "exit":
						// 클라이언트 종료
						_client.Close();
						break;
				}
				continue;
			}

			// 텍스트가 명령어가 아니라면 채팅 패킷 전송
			SendPacket(new ClientTextPacket(line));
		}
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
					case ClientTextPacket packet: {
						Debug.Log($"[S -> C] {packet}");
						break;
					}
					case ServerTextPacket packet: {
						Console.WriteLine($"{packet.Player.Name}: {packet.Text}");
						break;
					}
					case ServerHandshakePacket packet: {
						_player = packet.Player;
						Debug.Log($"Setting Player to {packet.Player}");
						break;
					}
					case ServerPlayerListPacket packet: {
						Console.Out.WriteLine($"접속한 유저({packet.Players.Count}명) : ");
						packet.Players.ForEach(targetPlayer => { Console.Out.WriteLine($"- {targetPlayer.Name}"); });
						break;
					}
				}
			}
		}).ContinueWith(t => {
			if (t.IsFaulted) throw t.Exception!;
		});
	}

	private void SendPacket(IPacket packet) {
		Debug.Log($"[C -> S] {packet}");
		packet.Write(_writer);
	}
}
