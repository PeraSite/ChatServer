using JetBrains.Annotations;

public static class Debug {
	private static readonly bool Enabled = Environment.GetEnvironmentVariable("DEBUG") == "1";

	public static void Log(string message) {
		if (Enabled) {
			Console.WriteLine(message);
		}
	}

	[StringFormatMethod("format")]
	public static void Log(string format, params object?[] arg0) => Log(string.Format(format, arg0));
}
