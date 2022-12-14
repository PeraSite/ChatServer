using Common;
using Common.Objects;

namespace Packets.Common;

public enum PlayerStatusType : byte {
	JOIN,
	QUIT
}

public record PlayerStatusPacket(Player Player, PlayerStatusType Type) : IPacket {
	public PlayerStatusPacket(BinaryReader reader) : this(
		reader.ReadPlayer(),
		(PlayerStatusType) reader.ReadByte()
	) { }

	public PacketType GetPacketType() {
		return PacketType.Common_PlayerStatus;
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(Player);
		writer.Write((byte) Type);
	}
}
