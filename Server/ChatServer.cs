using System.Net;
using System.Net.Sockets;
using Common.Objects;
using Packets.Client;
using Packets.Server;

public class ChatServer {
	private readonly TcpListener _server;
	private List<PlayerConnection> _playerConnections;

	public ChatServer(int port) {
		_server = new TcpListener(IPAddress.Any, port);
		_playerConnections = new List<PlayerConnection>();

		_server.Start();
		Debug.Log("[TCP 서버] 서버 시작: 포트 번호 = {0}", port);
		try {
			while (true) {
				var client = _server.AcceptTcpClient();
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

	~ChatServer() {
		Debug.Log("[TCP 서버] 서버 종료");
		_server.Stop();
	}

	private void HandleNewClient(TcpClient client) {
		var playerConnection = new PlayerConnection(client);
		_playerConnections.Add(playerConnection);

		var ip = playerConnection.IP;
		var reader = playerConnection.Reader;

		Debug.Log("[TCP 서버] 클라이언트 접속: IP 주소={0}, 포트번호 = {1}", ip.Address, ip.Port);
		try {
			while (true) {
				var packetID = reader.ReadByte();
				var packetType = (PacketType) packetID;

				Debug.Log("[C -> S] 패킷 ID: {0}", packetType);
				//TODO: Dictionary<PacketType, PacketHandler>로 쉽게 register/unregister하게. 아래 switch 부분은 패킷 추가할때마다 기존 코드를 수정할 부분이 생김

				switch (packetType) {
					case PacketType.Client_Text: {
						var packet = new ClientTextPacket(reader);
						HandleClientTextPacket(client, packet);
						break;
					}
					case PacketType.Client_Handshake: {
						var packet = new ClientHandshakePacket(reader);
						HandleClientHandshakePacket(playerConnection, packet);
						break;
					}
					case PacketType.Server_Handshake:
						break;
					case PacketType.Server_Text:
						break;
					case PacketType.Client_PlayerList:
						playerConnection.SendPacket(
							new ServerPlayerListPacket(_playerConnections.Select(connection => connection.Player)
								.ToList()));
						break;
					case PacketType.Server_PlayerList:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		} catch (Exception e) {
			Console.WriteLine(e);
			throw;
		} finally {
			OnPlayerQuit(client);
		}
	}

	private void OnPlayerQuit(TcpClient client) {
		var playerConnection = GetPlayerConnection(client);
		if (playerConnection == null) return;

		var address = playerConnection.IP;

		_playerConnections.Remove(playerConnection);
		client.Close();
		Debug.Log("[TCP 서버] 클라이언트 종료: IP 주소={0}, 포트 번호={1}", address.Address, address.Port);
	}

	private void HandleClientHandshakePacket(PlayerConnection playerConnection, ClientHandshakePacket packet) {
		var player = new Player(packet.Name, Guid.NewGuid());
		Debug.Log("New player joined : {0}", player);
		playerConnection.Player = player;
		playerConnection.SendPacket(new ServerHandshakePacket(player));
		playerConnection.SendPacket(
			new ServerPlayerListPacket(_playerConnections.Select(connection => connection.Player).ToList()));
	}

	private void HandleClientTextPacket(TcpClient client, ClientTextPacket packet) {
		var player = GetPlayerConnection(client)?.Player;
		if (player == null) return;

		// Broadcast to other players
		foreach (var otherPlayerConnection in _playerConnections) {
			otherPlayerConnection.SendPacket(new ServerTextPacket(player, packet.Text));
		}
	}

	private PlayerConnection? GetPlayerConnection(TcpClient client) {
		return _playerConnections.FirstOrDefault(x => x.Client == client);
	}
}
