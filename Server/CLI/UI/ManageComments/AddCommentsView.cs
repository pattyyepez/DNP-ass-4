using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class AddCommentsView
{
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository; 
    private readonly int postId;

    public AddCommentsView(ICommentRepository commentRepository, IUserRepository userRepository, IPostRepository postRepository, int postId)
    {
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.postId = postId;
    }

    public async Task ShowAsync()
    {
        try
        {
            Post post = await postRepository.GetSingleAsync(postId);
        }
        catch (Exceptions.NotFoundException)
        {
            Console.WriteLine($"Post with ID {postId} does not exist. Comment cannot be added.");
            return;
        }
        
        int userId;
        while (true)
        {
            Console.WriteLine("Insert your User ID:");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out userId) || userId <= 0)
            {
                Console.WriteLine("Invalid User ID. Please try again.");
                continue;
            }
            try
            {
                await userRepository.GetSingleAsync(userId);
                break; 
            }
            catch (Exceptions.NotFoundException)
            {
                Console.WriteLine($"User with ID {userId} does not exist. Please try again.");
            }
        }

        Console.WriteLine("Insert your comment:");
        string? commentBody = Console.ReadLine();
        if (string.IsNullOrEmpty(commentBody))
        {
            Console.WriteLine("Comment cannot be empty.");
            return;
        }

        Comment comment = new Comment(commentBody, postId, userId);

        await commentRepository.AddAsync(comment);
        Console.WriteLine("Comment added successfully.");
    }
}