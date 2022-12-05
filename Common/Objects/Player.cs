namespace Common.Objects;

public record Player(string Name, Guid Guid) {
	public Player(Guid guid) : this(string.Empty, guid) { }
}
