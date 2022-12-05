using Common;
using Common.Objects;

namespace Packets.Server;

public record ServerHandshakePacket(Player Player) : IPacket {
	public ServerHandshakePacket(BinaryReader reader) : this(
		reader.ReadPlayer()
	) { }

	public PacketType GetPacketType() {
		return PacketType.Server_Handshake;
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(Player);
	}
}
