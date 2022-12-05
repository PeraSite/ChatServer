using System.Text;

const int SERVER_PORT = 9000;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

var server = new ChatServer(SERVER_PORT);
