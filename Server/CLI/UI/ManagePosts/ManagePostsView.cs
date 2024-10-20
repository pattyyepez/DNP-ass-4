using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public ManagePostsView(IPostRepository postRepository, ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine();
        while (true)
        {
            Console.WriteLine("Please select:\n" +
                              "1) Create new post\n" +
                              "2) Update post\n" +
                              "3) Delete post\n" +
                              "4) View all posts\n" +
                              "5) Exit");
            string input = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Please select an option.\n\n");
                continue;
            }

            if ("Exit".Equals(input))
            {
                return;
            }

            switch (input)
            {
                case "1":
                    await new CreatePostView(postRepository, userRepository).ShowAsync();
                    break;
                case "2":
                    await new UpdatePostsView(postRepository).ShowAsync();
                    break;
                case "3":
                    await new DeletePostsView(postRepository).ShowAsync();
                    break;
                case "4":
                    await new ListPostsView(postRepository, commentRepository, userRepository).ShowAsync();
                    break;
                case "5": return;
                default:
                    Console.WriteLine("Invalid option, please try again.\n\n");
                    break;
            }
        }
    }
    
}