namespace DTOs;

public class CreatePostDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int UserId { get; set; }
}