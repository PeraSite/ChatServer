using Common;
using Packets.Client;
using Packets.Server;

public enum PacketType : byte {
	Client_Handshake,
	Server_Handshake,

	Client_Text,
	Server_Text,

	Client_PlayerList,
	Server_PlayerList
}

public static class PacketTypes {
	private static readonly Dictionary<PacketType, Func<BinaryReader, IPacket>> PACKET_CONSTRUCTORS = new() {
		{PacketType.Client_Handshake, reader => new ClientHandshakePacket(reader)},
		{PacketType.Server_Handshake, reader => new ServerHandshakePacket(reader)},

		{PacketType.Client_Text, reader => new ClientTextPacket(reader)},
		{PacketType.Server_Text, reader => new ServerTextPacket(reader)},

		{PacketType.Client_PlayerList, _ => new ClientPlayerListPacket()},
		{PacketType.Server_PlayerList, reader => new ServerPlayerListPacket(reader)},
	};

	public static IPacket CreatePacket(this PacketType type, BinaryReader reader) {
		return PACKET_CONSTRUCTORS[type](reader);
	}

	public static TPacket CreatePacket<TPacket>(this PacketType type, BinaryReader reader) where TPacket : IPacket {
		return (TPacket) PACKET_CONSTRUCTORS[type](reader);
	}
}
