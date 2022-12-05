using Common;

namespace Packets.Client;

public record ClientTextPacket(string Text) : IPacket {
	public ClientTextPacket(BinaryReader reader) : this(reader.ReadString()) { }

	public PacketType GetPacketType() {
		return PacketType.Client_Text;
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(Text);
	}
}
