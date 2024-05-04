namespace RideMe.Api.Dtos
{
    public class RequestRideDto
    {
        public int PassengerId { get; set; }

        public int DriverId { get; set; }

        public string RideSource { get; set; }

        public string RideDestination { get; set; }

        public double Price { get; set; }

    }
}
