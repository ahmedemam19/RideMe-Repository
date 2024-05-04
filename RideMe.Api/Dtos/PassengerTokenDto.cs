namespace RideMe.Api.Dtos
{
    public class PassengerTokenDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
    }
}