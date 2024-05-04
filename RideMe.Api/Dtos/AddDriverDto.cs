namespace RideMe.Api.Dtos
{
    public class AddDriverDto
    {
        public string Name { get; set; } 
        public string PhoneNumber { get; set; }
        public string Email { get; set; }    
        public string Password { get; set; } 
        public string CarType { get; set; }
        public bool Smoking { get; set; }
        public int CityId { get; set; }
        public string Region { get; set; }
    }
}
