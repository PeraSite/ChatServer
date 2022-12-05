using System.Text;

const string SERVERIP = "127.0.0.1";
const int SERVER_PORT = 9000;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

var client = new ChatClient($"Client #{Random.Shared.Next(100)}", SERVERIP, SERVER_PORT);
