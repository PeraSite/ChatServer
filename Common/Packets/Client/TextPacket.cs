using Common;

namespace Packets.Client;

public record TextPacket(int Index, string Text) : IPacket {
	public int Index { get; private set; } = Index;
	public string Text { get; private set; } = Text;

	public TextPacket() : this(0, string.Empty) { }

	public PacketType GetPacketType() {
		return PacketType.TEXT;
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(Index);
		writer.Write(Text);
	}

	public void Deserialize(BinaryReader reader) {
		Index = reader.ReadInt32();
		Text = reader.ReadString();
	}
}
