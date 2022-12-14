using System.Net;
using System.Net.Sockets;
using Common;
using Common.Objects;
using Packets.Client;
using Packets.Common;
using Packets.Server;

public class ChatServer {
	private readonly TcpListener _server;
	private readonly List<PlayerConnection> _playerConnections;

	public ChatServer(int port) {
		// TCP 서버 생성
		_server = new TcpListener(IPAddress.Any, port);

		// PlayerConnection 리스트 초기화
		_playerConnections = new List<PlayerConnection>();
	}

	~ChatServer() {
		Debug.Log("[TCP 서버] 서버 종료");
		_server.Stop();
	}

	public void Start() {
		// TCP 서버 시작
		_server.Start();

		Debug.Log($"[TCP 서버] 서버 시작");
		try {
			// 서버가 켜진 동안 클라이언트 접속 받기
			while (true) {
				// 새로운 TCP 클라이언트 접속 받기
				var client = _server.AcceptTcpClient();

				// 새 스레드에서 클라이언트 처리
				Task.Run(() => HandleNewClient(client))
					.ContinueWith(t => {
						if (t.IsFaulted) throw t.Exception!;
					});
			}
		} catch (Exception e) {
			Console.WriteLine(e);
			throw;
		}
	}

	private void HandleNewClient(TcpClient client) {
		// PlayerConnection 생성
		var playerConnection = new PlayerConnection(client);
		_playerConnections.Add(playerConnection);

		var ip = playerConnection.IP;
		var reader = playerConnection.Reader;

		Debug.Log("[TCP 서버] 클라이언트 접속: IP 주소={0}, 포트번호={1}", ip.Address, ip.Port);

		// 패킷 읽기
		try {
			while (client.Connected) {
				// 패킷 ID 읽기
				var packetID = reader.BaseStream.ReadByte();

				// 읽을 수 없다면(데이터가 끝났다면 리턴)
				if (packetID == -1) break;

				var packetType = (PacketType) packetID;

				// 타입에 맞는 패킷 객체 생성
				var basePacket = packetType.CreatePacket(reader);
				Debug.Log($"[C -> S] {basePacket}");

				// 패킷 타입에 맞게 처리
				switch (basePacket) {
					case ClientTextPacket packet: {
						HandleClientTextPacket(playerConnection, packet);
						break;
					}
					case ClientHandshakePacket packet: {
						HandleClientHandshakePacket(playerConnection, packet);
						break;
					}
					case ClientPlayerListPacket: {
						HandleClientPlayerListPacket(playerConnection);
						break;
					}
				}
			}
		} catch (Exception e) {
			Console.WriteLine(e);
			throw;
		} finally {
			// 클라이언트 접속 종료 처리
			HandleClientQuit(playerConnection);
		}
	}

	private void HandleClientQuit(PlayerConnection playerConnection) {
		var player = playerConnection.Player;
		if (player == null) return;

		// 플레이어 Quit broadcast
		Broadcast(new PlayerStatusPacket(player, PlayerStatusType.QUIT));

		var address = playerConnection.IP;

		// PlayerConnection Dictionary 에서 삭제
		_playerConnections.Remove(playerConnection);

		// 클라이언트 닫기
		playerConnection.Client.Close();

		Debug.Log("[TCP 서버] 클라이언트 종료: IP 주소={0}, 포트 번호={1}", address.Address, address.Port);
	}

#region Packet Handling
	private void HandleClientHandshakePacket(PlayerConnection playerConnection, ClientHandshakePacket packet) {
		// 새 Player 객체 생성 후 PlayerConnection에 할당
		var player = new Player(packet.Name, Guid.NewGuid());
		playerConnection.Player = player;

		// 클라이언트에게 플레이어 정보, 현재 접속해있는 플레이어 리스트 전송
		playerConnection.SendPacket(new ServerHandshakePacket(player));
		playerConnection.SendPacket(new ServerPlayerListPacket(GetPlayerList()));

		// 플레이어 Join Broadcast
		Broadcast(new PlayerStatusPacket(player, PlayerStatusType.JOIN));
		Debug.Log($"New player joined : {player}");
	}

	private void HandleClientTextPacket(PlayerConnection playerConnection, ClientTextPacket packet) {
		var player = playerConnection.Player;

		// Player가 할당되지 않았다면(Handshake가 없었다면) 리턴
		if (player == null) return;

		// 모든 클라이언트에게 Text 패킷 Broadcast
		foreach (var otherPlayerConnection in _playerConnections) {
			otherPlayerConnection.SendPacket(new ServerTextPacket(player, packet.Text));
		}
	}

	private void HandleClientPlayerListPacket(PlayerConnection playerConnection) {
		playerConnection.SendPacket(new ServerPlayerListPacket(GetPlayerList()));
	}
#endregion

#region Util
	private void Broadcast(IPacket packet) {
		foreach (var playerConnection in _playerConnections) {
			playerConnection.SendPacket(packet);
		}
	}

	private PlayerConnection? GetPlayerConnection(TcpClient client) {
		return _playerConnections.FirstOrDefault(x => x.Client == client);
	}

	private List<Player> GetPlayerList() {
		return _playerConnections
			.Select(connection => connection.Player ?? new Player(string.Empty, Guid.Empty))
			.Where(p => !string.IsNullOrEmpty(p.Name))
			.ToList();
	}
#endregion
}
