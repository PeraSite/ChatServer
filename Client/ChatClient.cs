using System.Net.Sockets;
using Common;
using Packets.Client;

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

		var counter = 0;

		while (true) {
			var line = Console.ReadLine();
			if (string.IsNullOrEmpty(line)) break;
			var packet = new TextPacket(counter++, line);
			WritePacket(packet);
		}
	}

	~ChatClient() {
		_writer.Close();
		_reader.Close();
		_stream.Close();
		_client.Close();
	}

	private void WritePacket(IPacket packet, bool flush = true) {
		Console.WriteLine(packet);
		_writer.Write((int) packet.GetPacketType());
		packet.Serialize(_writer);
		if (flush)
			_writer.Flush();
	}
}
