namespace Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int UserId { get; }

    public Post(string title, string content, int userId)
    {
        Title = title;
        Content = content;
        UserId = userId;
    }
}