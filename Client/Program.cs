﻿using System.Net.Sockets;
using System.Text;

const string SERVERIP = "127.0.0.1";
const int SERVER_PORT = 9000;
const int BUFFER_SIZE = 512;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

var buffer = new byte[BUFFER_SIZE];

using var client = new TcpClient();
client.Connect(SERVERIP, SERVER_PORT);
var stream = client.GetStream();

while (true) {
	var line = Console.ReadLine();
	if (string.IsNullOrEmpty(line)) break;
	var bytes = Encoding.Default.GetBytes(line);
	stream.Write(bytes, 0, bytes.Length);

	var bytesRead = stream.Read(buffer);
	var response = Encoding.Default.GetString(buffer, 0, bytesRead);
	Console.WriteLine(response);
}

client.Close();
