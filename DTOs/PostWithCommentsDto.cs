namespace DTOs
{
    public class PostWithCommentsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }  
        public List<CommentDto> Comments { get; set; } 
    }
}