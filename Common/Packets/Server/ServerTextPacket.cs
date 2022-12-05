using Common;
using Common.Objects;

namespace Packets.Server;

public record ServerTextPacket(Player Player, string Text) : IPacket {
	public ServerTextPacket(BinaryReader reader) : this(reader.ReadPlayer(), reader.ReadString()) { }

	public PacketType GetPacketType() {
		return PacketType.Server_Text;
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(Player);
		writer.Write(Text);
	}
}
