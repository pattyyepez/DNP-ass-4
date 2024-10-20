using CLI.UI.ManageComments;
using Entities;
using RepositoryContracts; 
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
                Console.WriteLine($"Content: {post.Content}");
                Console.WriteLine($"**************************************");
                
                List<Comment> comments = commentRepository.GetManyAsync().Where(c => c.PostId == postId).ToList();
        
                foreach (Comment comment in comments)
                {
                    User user = await userRepository.GetSingleAsync(comment.UserId); 
                    Console.WriteLine($"{user.Username}: {comment.Body}");
                }
                
                Console.WriteLine("1) Add comment; \n 2) Delete comment; \n 3) Exit");
        
                while (true)
                {
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        Console.WriteLine("Please select a valid option.");
                        continue;
                    }
        
                    if ("1".Equals(input))
                    {
                        AddCommentsView addCommentView = new AddCommentsView(commentRepository, userRepository, postRepository, postId);
                        await addCommentView.ShowAsync();
                        return;
                    }
                    if ("2".Equals(input))
                    {
                        DeleteCommentsView deleteCommentView = new DeleteCommentsView(commentRepository);
                        await deleteCommentView.ShowAsync();
                        return;
                    }
                    if ("3".Equals(input))
                    {
                        return;
                    }
        
                    
                }
            } 
}