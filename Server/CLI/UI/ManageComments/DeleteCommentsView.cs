using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class DeleteCommentsView
{
    private readonly ICommentRepository commentRepository;

    public DeleteCommentsView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine();
        await DeleteCommentAsync();
    }

    private async Task DeleteCommentAsync()
    {
        while (true)
        {
            // Solicitar el ID del comentario a eliminar
            Console.WriteLine("Insert Comment ID to delete (or type 'Exit' to go back):");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Please insert a valid Comment ID.");
                continue;
            }

            if (input.ToLower() == "exit")
            {
                return;
            }

            if (int.TryParse(input, out int commentId))
            {
                try
                {
                    Comment comment = await commentRepository.GetSingleAsync(commentId);
                    Console.WriteLine($"Are you sure you want to delete the comment by user {comment.UserId}? (y/n)");

                    string? confirmation = Console.ReadLine()?.ToLower();
                    if (confirmation == "y")
                    {
                        await commentRepository.DeleteAsync(commentId);
                        Console.WriteLine($"Comment with ID {commentId} deleted successfully.");
                        return;
                    }
                    else if (confirmation == "n")
                    {
                        Console.WriteLine("Comment deletion canceled.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid option, please try again.");
                    }
                }
                catch (Exceptions.NotFoundException)
                {
                    Console.WriteLine($"Comment with ID {commentId} does not exist. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format. Please enter a numeric Comment ID.");
            }
        }
    }
}
