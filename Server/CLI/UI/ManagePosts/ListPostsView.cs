using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public ListPostsView(IPostRepository postRepository, ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }
    public Task ShowAsync()
    {
        Console.WriteLine();
        return ViewPostsAsync();
    }

    public async Task ViewPostsAsync()
    {
        List<Post> posts = postRepository.GetManyAsync().OrderBy(p => p.Id).ToList();
        Console.WriteLine("Posts:");
        
        foreach (Post post in posts)
        {
            Console.WriteLine($"\t({post.Id}): {post.Title}");
        }
        

        Console.WriteLine("Insert Post ID too see more info \n Exit ");
        
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

            int postId;
            if (int.TryParse(input, out postId))
            {
                SinglePostView singlePostView = new(postRepository, commentRepository, userRepository, postId);
                await singlePostView.ShowAsync();
                Console.WriteLine("Insert Post ID too see more info \n Exit ");
            }
            else
            {
                Console.WriteLine("Invalid option, please try again.");
            }
        }
    }
}