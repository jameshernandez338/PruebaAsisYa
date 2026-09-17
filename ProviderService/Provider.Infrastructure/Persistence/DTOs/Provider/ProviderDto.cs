namespace Provider.Infrastructure.Persistence.DTOs.Provider
{
    public class ProviderDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Lon { get; set; }
        public bool Available { get; set; }
        public double Rating { get; set; }
    }
}
