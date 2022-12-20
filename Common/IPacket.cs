namespace Common;

public interface IPacket {
	public PacketType GetPacketType();
	public void Serialize(BinaryWriter writer);
}
