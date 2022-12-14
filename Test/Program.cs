// 설정

const string SERVERIP = "127.0.0.1";
const int SERVER_PORT = 9000;

const int CLIENT_AMOUNT = 10;

await Task.WhenAll(Enumerable.Range(0, CLIENT_AMOUNT)
	.Select(index => {
		return Task.Run(() => {
			var name = $"Client #{index}";
			var client = new TestClient(name, SERVERIP, SERVER_PORT);
			Console.Out.WriteLine($"Client ready: {name}");

			client.Start();
		});
	}));
