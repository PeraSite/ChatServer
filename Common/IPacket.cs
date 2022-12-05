namespace Common;

public interface IPacket {
	public PacketType GetPacketType();
	public void Serialize(BinaryWriter writer);

	public void Write(BinaryWriter writer, bool flush = true) {
		writer.Write((byte) GetPacketType());
		Serialize(writer);
		if (flush)
			writer.Flush();
	}
}
