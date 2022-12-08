using System.Text;

// 설정
const int SERVER_PORT = 9000;

// 콘솔 입출력 한글 깨짐 수정
Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

// 서버 시작
var server = new ChatServer(SERVER_PORT);
server.Start();
