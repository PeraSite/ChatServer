using System.Net;
using System.Net.Sockets;
using System.Text;
using Packets.Client;

const int SERVER_PORT = 9000;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

var server = new TcpListener(IPAddress.Any, SERVER_PORT);

try {
	server.Start();
	Console.WriteLine("[TCP 서버] 서버 시작: 포트 번호 = {0}", SERVER_PORT);

	while (true) {
		var tcpClient = server.AcceptTcpClient();
		Task.Run(() => ProcessClient(tcpClient));
	}
} finally {
	Console.WriteLine("[TCP 서버] 서버 종료");
	server.Stop();
}

static void ProcessClient(TcpClient tcpClient) {
	var clientSocket = tcpClient.Client;
	var clientAddress = (IPEndPoint) clientSocket.RemoteEndPoint!;
	var stream = tcpClient.GetStream();
	var reader = new BinaryReader(stream);

	Console.WriteLine("\n[TCP 서버] 클라이언트 접속: IP 주소={0}, 포트번호 = {1}", clientAddress.Address, clientAddress.Port);
	try {
		while (true) {
			var packetID = reader.ReadInt32();
			var packetType = (PacketType) packetID;

			//TODO: Dictionary<PacketType, PacketHandler>로 쉽게 register/unregister하게. 아래 switch 부분은 패킷 추가할때마다 기존 코드를 수정할 부분이 생김

			switch (packetType) {
				case PacketType.TEXT:
					var packet = new TextPacket();
					packet.Deserialize(reader);
					Console.WriteLine("[TCP/{0}:{1}] {2}th: {3}", clientAddress.Address, clientAddress.Port,
						packet.Index,
						packet.Text);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	} finally {
		tcpClient.Close();
		Console.WriteLine("[TCP 서버] 클라이언트 종료: IP 주소={0}, 포트 번호={1}", clientAddress.Address, clientAddress.Port);
	}
}
