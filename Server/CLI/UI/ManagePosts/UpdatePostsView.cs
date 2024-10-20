using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class UpdatePostsView
{
    private readonly IPostRepository postRepository;

    public UpdatePostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine();
        await UpdatePostAsync();
    }
    private async Task UpdatePostAsync()
    {
        while (true)
        {
            Console.WriteLine("Insert Post ID to update (or type 'Exit' to go back):");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Please insert a valid Post ID.");
                continue;
            }

            if (input.ToLower() == "exit")
            {
                return;
            }

            if (int.TryParse(input, out int postId))
            {
                try
                {
                    Post post = await postRepository.GetSingleAsync(postId);

                    Console.WriteLine($"Current Title: {post.Title}");
                    Console.WriteLine($"Current Content: {post.Content}");

                    Console.WriteLine("Enter new title (or press Enter to keep the current title):");
                    string? newTitle = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newTitle))
                    {
                        post.Title = newTitle; 
                    }

                    Console.WriteLine("Enter new content (or press Enter to keep the current content):");
                    string? newContent = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newContent))
                    {
                        post.Content = newContent; 
                    }
                    await postRepository.UpdateAsync(post);
                    Console.WriteLine($"Post with ID {postId} updated successfully.");

                    return; 
                }
                catch (Exceptions.NotFoundException)
                {
                    Console.WriteLine($"Post with ID {postId} does not exist. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID.");
            }
        }
    }
    
}