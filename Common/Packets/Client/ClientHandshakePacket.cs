using Common;

namespace Packets.Client;

public record ClientHandshakePacket(string Name) : IPacket {
	public ClientHandshakePacket(BinaryReader reader) : this(
		reader.ReadString()
	) { }

	public PacketType GetPacketType() {
		return PacketType.Client_Handshake;
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(Name);
	}
}
