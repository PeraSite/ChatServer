using Common.Objects;

public static class BinaryWriterExtensions {
	public static void Write(this BinaryWriter writer, Guid guid) {
		writer.Write(guid.ToByteArray());
	}

	public static void Write(this BinaryWriter writer, Player player) {
		writer.Write(player.Name);
		writer.Write(player.Guid);
	}
}
