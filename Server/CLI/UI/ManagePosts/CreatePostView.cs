using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public CreatePostView(IPostRepository postRepo, IUserRepository userRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }
    public Task ShowAsync()
    {
        Console.WriteLine();
        return CreatePostAsync();
        
    }

    private async Task CreatePostAsync()
    {

        string? title = null;
        while (string.IsNullOrEmpty(title))
        {
            Console.WriteLine("Please insert post title:");
            title = Console.ReadLine();
            if (string.IsNullOrEmpty(title))
            {
                Console.WriteLine("Title cannot be empty.");
            }
        }
        string? content = null;
        while (string.IsNullOrEmpty(content))
        {
            Console.WriteLine("Please insert post content:");
            content = Console.ReadLine();
            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine("Content cannot be empty.");
            }
        }

        int userId = 0;
        while (userId <= 0)
        {
            Console.WriteLine("Please insert the ID of the user:");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out userId) || userId <= 0)
            {
                Console.WriteLine("Invalid ID. Please try again.");
            }
            try
            {
                // Verifica si el usuario existe
                var user = await userRepo.GetSingleAsync(userId); 
            }
            catch (Exceptions.NotFoundException)
            {
                Console.WriteLine($"User with ID {userId} does not exist. Please try again.");
                userId = 0; 
            }
        }

        Post post = new Post(title, content, userId);
        Post addedPost = await postRepo.AddAsync(post);

        Console.WriteLine($"Post created successfully with Id: {addedPost.Id}");
    }
}