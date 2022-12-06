using System.Text;

const string SERVERIP = "127.0.0.1";
const int SERVER_PORT = 9000;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

Console.Out.Write("닉네임을 적어주세요 : ");
var name = Console.ReadLine();
if (string.IsNullOrEmpty(name)) {
	Console.Out.WriteLine("올바르지 않은 이름입니다.");
	return;
}

var client = new ChatClient(name, SERVERIP, SERVER_PORT);
