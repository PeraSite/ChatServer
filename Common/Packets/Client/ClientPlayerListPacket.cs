using Common;

namespace Packets.Client;

public record ClientRequestPlayerListPacket : IPacket {
	public PacketType GetPacketType() {
		return PacketType.Client_PlayerList;
	}

	public void Serialize(BinaryWriter writer) { }
}
