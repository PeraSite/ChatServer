using System.Net.Sockets;
using System.Text;
using Common;
using Packets.Client;

const string SERVERIP = "127.0.0.1";
const int SERVER_PORT = 9000;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

var client = new ChatClient(SERVERIP, SERVER_PORT);
