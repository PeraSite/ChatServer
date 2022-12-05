using System.Net;
using System.Net.Sockets;
using Common.Objects;
using Packets.Client;
using Packets.Server;

public class ChatServer {
	private readonly TcpListener _server;
	private Dictionary<Player, PlayerConnection> _playerConnection;

	public ChatServer(int port) {
		_server = new TcpListener(IPAddress.Any, port);
		_playerConnection = new Dictionary<Player, PlayerConnection>();

		_server.Start();
		Console.WriteLine("[TCP 서버] 서버 시작: 포트 번호 = {0}", port);
		try {
			while (true) {
				var tcpClient = _server.AcceptTcpClient();
				Task.Run(() => HandleNewClient(tcpClient));
			}
		} catch (Exception e) {
			Console.WriteLine(e);
			throw;
		}
	}

	~ChatServer() {
		Console.WriteLine("[TCP 서버] 서버 종료");
		_server.Stop();
	}

	private void HandleNewClient(TcpClient tcpClient) {
		var player = new Player(Guid.NewGuid());
		var playerConnection = new PlayerConnection(player, tcpClient);
		_playerConnection[player] = playerConnection;

		var address = playerConnection.IP;

		var reader = playerConnection.Reader;

		Console.WriteLine("\n[TCP 서버] 클라이언트 접속: IP 주소={0}, 포트번호 = {1}", address.Address, address.Port);
		try {
			while (true) {
				var packetID = reader.ReadByte();
				var packetType = (PacketType) packetID;

				//TODO: Dictionary<PacketType, PacketHandler>로 쉽게 register/unregister하게. 아래 switch 부분은 패킷 추가할때마다 기존 코드를 수정할 부분이 생김

				switch (packetType) {
					case PacketType.Client_Text:
						var packet = new ClientTextPacket(reader);
						HandleClientTextPacket(player, packet);
						break;
					case PacketType.Server_Text:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		} catch (Exception e) {
			Console.WriteLine(e);
			throw;
		} finally {
			OnPlayerQuit(tcpClient, playerConnection);
		}
	}

	private void OnPlayerQuit(TcpClient tcpClient, PlayerConnection playerConnection) {
		var player = playerConnection.Player;
		var address = playerConnection.IP;

		_playerConnection.Remove(player);
		tcpClient.Close();
		Console.WriteLine("[TCP 서버] 클라이언트 종료: IP 주소={0}, 포트 번호={1}", address.Address, address.Port);
	}

	private void HandleClientTextPacket(Player player, ClientTextPacket packet) {
		Console.Out.WriteLine($"[C -> S] {packet}");

		// Broadcast to other players
		foreach (var (otherPlayer, otherPlayerConnection) in
		         _playerConnection.Where(pair => !pair.Key.Equals(player))) {
			otherPlayerConnection.SendPacket(new ServerTextPacket(player, packet.Text));
		}
	}
}
