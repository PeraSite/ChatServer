using Common;
using Common.Objects;

namespace Packets.Server;

public record ServerPlayerListPacket(List<Player> Players) : IPacket {
	public ServerPlayerListPacket(BinaryReader reader) : this(
		Enumerable.Range(0, reader.ReadInt32())
			.Select(_ => reader.ReadPlayer())
			.ToList()
	) { }

	public PacketType GetPacketType() {
		return PacketType.Server_PlayerList;
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(Players.Count);
		Players.ForEach(writer.Write);
	}
}
