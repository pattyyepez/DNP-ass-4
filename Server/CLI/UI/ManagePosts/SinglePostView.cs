namespace CLI.UI.ManagePosts;

public class SinglePostView
{
        private readonly IPostRepository postRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IUserRepository userRepository;
        private readonly int postId;
    
        public SinglePostView(IPostRepository postRepository, ICommentRepository commentRepository, IUserRepository userRepository, int postId)
        {
            this.postRepository = postRepository;
            this.postId = postId;
            this.userRepository = userRepository;
            this.commentRepository = commentRepository;
        }
        
        public async Task ShowAsync()
            {
                Post post = await postRepository.GetSingleAsync(postId);
                Console.WriteLine($"**************************************");
                Console.WriteLine($"Title: {post.Title}");
                Console.WriteLine($"**************************************");
                Console.WriteLine($"Content: {post.Body}");
                Console.WriteLine($"**************************************");
                
                List<Comment> comments = commentRepository.GetMany().Where(c => c.PostId == postId).ToList();
        
                foreach (Comment comment in comments)
                {
                    User user = await userRepository.GetSingleAsync(comment.UserId); 
                    Console.WriteLine($"{user.UserName}: {comment.Body}");
                }
        
                Console.WriteLine();
                const string options = """
                                       1) Add comment;
                                       <) Back
                                       """;
                Console.WriteLine("1) Add comment; \n Exit");
        
                while (true)
                {
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        Console.WriteLine("Please select a valid option.");
                        continue;
                    }
        
                    if ("Exit".Equals(input))
                    {
                        return;
                    }
        
                    Console.WriteLine("Not supported");
                }
            } 
}