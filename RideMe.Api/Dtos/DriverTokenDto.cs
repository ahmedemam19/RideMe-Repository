namespace RideMe.Api.Dtos
{
    public class DriverTokenDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CarType { get; set; }
        public bool Smoking { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public double Rating { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public bool Available { get; set; }
    }
}