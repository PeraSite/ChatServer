using System.Text;

// 설정
const string SERVERIP = "127.0.0.1";
const int SERVER_PORT = 9000;

// 콘솔 입출력 한글 깨짐 수정
Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

// 닉네임 설정
Console.Out.Write("닉네임을 적어주세요 : ");
var name = Console.ReadLine();
if (string.IsNullOrEmpty(name)) {
	Console.Out.WriteLine("올바르지 않은 이름입니다.");
	return;
}

// 클라이언트 시작
var client = new ChatClient(name, SERVERIP, SERVER_PORT);
client.Start();
