using Common.Objects;

public static class BinaryReaderExtensions {
	public static Guid ReadGuid(this BinaryReader reader) {
		return new Guid(reader.ReadBytes(16));
	}

	public static Player ReadPlayer(this BinaryReader reader) {
		var name = reader.ReadString();
		var guid = reader.ReadGuid();
		return new Player(name, guid);
	}
}
