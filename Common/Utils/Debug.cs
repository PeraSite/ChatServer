using JetBrains.Annotations;

public static class Debug {
	public static bool Enabled;

	public static void Log(string message) {
		if (Enabled) {
			Console.WriteLine(message);
		}
	}

	[StringFormatMethod("format")]
	public static void Log(string format, params object?[] arg0) => Log(string.Format(format, arg0));
}
