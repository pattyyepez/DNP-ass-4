namespace DTOs;

public class CreateUserDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}