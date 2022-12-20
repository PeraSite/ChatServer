using Common;
using Packets.Client;
using Packets.Common;
using Packets.Server;

public enum PacketType : byte {
	Client_Handshake,
	Server_Handshake,

	Client_Text,
	Server_Text,

	Client_PlayerList,
	Server_PlayerList,

	Common_PlayerStatus,
}

public static class PacketTypes {
	public static IPacket CreatePacket(this PacketType type, BinaryReader reader) {
		return type switch {
			PacketType.Client_Handshake => new ClientHandshakePacket(reader),
			PacketType.Server_Handshake => new ServerHandshakePacket(reader),

			PacketType.Client_Text => new ClientTextPacket(reader),
			PacketType.Server_Text => new ServerTextPacket(reader),

			PacketType.Client_PlayerList => new ClientPlayerListPacket(),
			PacketType.Server_PlayerList => new ServerPlayerListPacket(reader),

			PacketType.Common_PlayerStatus => new PlayerStatusPacket(reader),

			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}

	public static TPacket CreatePacket<TPacket>(this PacketType type, BinaryReader reader) where TPacket : IPacket {
		return (TPacket) type.CreatePacket(reader);
	}
}
