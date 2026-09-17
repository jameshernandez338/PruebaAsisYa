namespace Provider.Application.Provider.Requests
{
    public sealed record UserLocationRequest(
        double Lat, 
        double Lon
     );
}
