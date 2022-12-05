using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program {
	private const int SERVER_PORT = 9000;
	private const int BUFFER_SIZE = 512;

	private static void ProcessClient(object? arg) {
		if (arg == null) return;
		var clientSocket = (Socket) arg;
		if (clientSocket.RemoteEndPoint is not IPEndPoint clientAddress) return;
		var buffer = new byte[BUFFER_SIZE];

		Console.WriteLine("\n[TCP 서버] 클라이언트 접속: IP 주소={0}, 포트번호 = {1}", clientAddress.Address, clientAddress.Port);
		while (true) {
			try {
				var textLength = clientSocket.Receive(buffer, BUFFER_SIZE, SocketFlags.None);
				if (textLength == 0) break;
				var text = Encoding.Default.GetString(buffer, 0, textLength);
				Console.WriteLine("[TCP/{0}:{1}] {2}", clientAddress.Address, clientAddress.Port, text);

				clientSocket.Send(buffer, textLength, SocketFlags.None);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				break;
			}
		}
		clientSocket.Close();
		Console.WriteLine("[TCP 서버] 클라이언트 종료: IP 주소={0}, 포트 번호={1}", clientAddress.Address, clientAddress.Port);
	}

	private static void Main() {
		Console.InputEncoding = Encoding.Unicode;
		Console.OutputEncoding = Encoding.Unicode;

		var server = new TcpListener(IPAddress.Any, SERVER_PORT);

		try {
			server.Start();
			Console.WriteLine("[TCP 서버] 서버 시작: 포트 번호 = {0}", SERVER_PORT);

			while (true) {
				using var tcpClient = server.AcceptTcpClient();
				var clientSocket = tcpClient.Client;
				Task.Factory.StartNew(ProcessClient, clientSocket);
			}
		} finally {
			Console.WriteLine("[TCP 서버] 서버 종료");
			server.Stop();
		}
	}
}
