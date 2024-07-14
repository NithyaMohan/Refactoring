namespace LegacyApp
{
    public record Client
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public ClientStatus ClientStatus { get; init; }
    }
}
