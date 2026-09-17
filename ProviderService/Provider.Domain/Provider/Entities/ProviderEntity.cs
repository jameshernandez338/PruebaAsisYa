namespace Provider.Domain.Provider.Entities;

public class ProviderEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public bool Available { get; private set; }
    public double Rating { get; set; }

    private ProviderEntity(
            Guid id,
            string name,
            double lat,
            double lon,
            bool available,
            double rating)
    {
        Id = id;
        Name = name;
        Lat = lat;
        Lon = lon;
        Available = available;
        Rating = rating;
    }

    public static ProviderEntity Reconstitute(Guid id, string name, double lat, double lon, bool available, double rating)
    {
        return new ProviderEntity
        (
            id,
            name,
            lat,
            lon,
            available,
            rating
        );
    }

    public void MarkUnavailable()
    {
        Available = false;
    }

    public void MarkAvailable()
    {
        Available = true;
    }
}
