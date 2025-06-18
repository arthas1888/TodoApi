namespace TodoApi.Models.ViewModels
{
    public class UserDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }

       
    }
  
}