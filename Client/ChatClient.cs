using System.Net.Sockets;
using Common;
using Common.Objects;
using Packets.Client;
using Packets.Common;
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

		// TcpClient 생성 및 접속
		_client = new TcpClient();
		_client.Connect(ip, port);

		// BinaryWriter, BinaryReader을 NetworkStream으로 생성
		_stream = _client.GetStream();
		_writer = new BinaryWriter(_stream);
		_reader = new BinaryReader(_stream);
	}

	~ChatClient() {
		CloseClient();
	}

	private void CloseClient() {
		// TCP 클라이언트, Stream 닫기
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
						CloseClient();
						break;
				}
				continue;
			}

			// 텍스트가 명령어가 아니라면 채팅 패킷 전송
			SendPacket(new ClientTextPacket(line));
		}
	}

	private void StartListeningPackets() {
		// Stream#Read는 Blocking Call이므로 별도의 스레드에서 실행
		var listeningThread = new Thread(() => {
			while (_client.Connected) {
				// 패킷 타입 읽어오기
				var packetID = _reader.ReadByte();
				var packetType = (PacketType) packetID;

				// 패킷 객체 생성
				var basePacket = packetType.CreatePacket(_reader);

				// 패킷 핸들링
				HandlePacket(basePacket);
			}
		});
		listeningThread.Start();
	}

	private void HandlePacket(IPacket basePacket) {
		switch (basePacket) {
			case ClientTextPacket packet: {
				HandleClientTextPacket(packet);
				break;
			}
			case ServerTextPacket packet: {
				HandleServerTextPacket(packet);
				break;
			}
			case ServerHandshakePacket packet: {
				HandleServerHandshakePacket(packet);
				break;
			}
			case ServerPlayerListPacket packet: {
				HandleServerPlayerListPacket(packet);
				break;
			}
			case PlayerStatusPacket packet: {
				HandlePlayerStatusPacket(packet);
				break;
			}
		}
	}

	private void HandleClientTextPacket(ClientTextPacket packet) {
		Debug.Log($"[S -> C] {packet.Text}");
	}

	private void HandleServerTextPacket(ServerTextPacket packet) {
		Console.WriteLine($"{packet.Player.Name}: {packet.Text}");
	}

	private void HandleServerHandshakePacket(ServerHandshakePacket packet) {
		_player = packet.Player;
		Debug.Log($"Setting Player to {packet.Player}");
	}

	private void HandleServerPlayerListPacket(ServerPlayerListPacket packet) {
		Console.Out.WriteLine($"접속한 유저({packet.Players.Count}명) : ");
		packet.Players.ForEach(targetPlayer => { Console.Out.WriteLine($"- {targetPlayer.Name}"); });
	}

	private void HandlePlayerStatusPacket(PlayerStatusPacket packet) {
		var statusType = packet.Type;
		var statusName = statusType switch {
			PlayerStatusType.JOIN => "접속",
			PlayerStatusType.QUIT => "퇴장",
			_ => "알 수 없음"
		};
		Console.Out.WriteLine($"[{packet.Player.Name}] {statusName}");
	}

	private void SendPacket(IPacket packet) {
		Debug.Log($"[C -> S] {packet}");
		packet.Write(_writer);
	}
}
