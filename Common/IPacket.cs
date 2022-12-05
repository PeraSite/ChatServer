namespace Common;

public interface IPacket {
	public PacketType GetPacketType();
	public void Serialize(BinaryWriter writer);
	public void Deserialize(BinaryReader reader);
}
