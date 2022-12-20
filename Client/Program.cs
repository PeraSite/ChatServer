using System.Text;

// 설정
const int SERVER_PORT = 9000;

// 서버 IP 입력 받기
Console.Out.Write("서버 IP : ");
var serverIP = Console.ReadLine();
if (string.IsNullOrEmpty(serverIP)) {
	Console.Out.WriteLine("올바르지 않은 IP입니다.");
	return;
}

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
var client = new ChatClient(name, serverIP, SERVER_PORT);
client.Start();
