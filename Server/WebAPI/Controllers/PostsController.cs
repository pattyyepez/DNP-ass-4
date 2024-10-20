using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

 [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommentRepository _commentRepository;

        public PostsController(IPostRepository postRepository, IUserRepository userRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto request)
        {
            await VerifyUserExistsAsync(request.UserId);  

            var post = new Post(request.Title, request.Content, request.UserId);
            var createdPost = await _postRepository.AddAsync(post);
            var user = await _userRepository.GetSingleAsync(request.UserId);

            var postDto = new PostDto
            {
                Id = createdPost.Id,
                Title = createdPost.Title,
                Content = createdPost.Content,
                UserName = user.Username
            };

            return CreatedAtAction(nameof(GetPost), new { id = postDto.Id }, postDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var post = await _postRepository.GetSingleAsync(id);
            if (post == null)
                return NotFound();

            var user = await _userRepository.GetSingleAsync(post.UserId);
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UserName = user.Username
            };
        }

        [HttpGet]
        public ActionResult<IEnumerable<PostDto>> GetPosts([FromQuery] string? titleContains = null, [FromQuery] string? userName = null)
        {
            var posts = _postRepository.GetManyAsync()
                .Where(p => (string.IsNullOrEmpty(titleContains) || p.Title.Contains(titleContains))
                         && (string.IsNullOrEmpty(userName) || _userRepository.GetSingleAsync(p.UserId).Result.Username == userName))
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    UserName = _userRepository.GetSingleAsync(p.UserId).Result.Username
                });

            return Ok(posts);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] CreatePostDto request)
        {
            var post = await _postRepository.GetSingleAsync(id);
            if (post == null)
                return NotFound();

            post.Title = request.Title;
            post.Content = request.Content;
            await _postRepository.UpdateAsync(post);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _postRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task VerifyUserExistsAsync(int userId)
        {
            var user = await _userRepository.GetSingleAsync(userId);
            if (user == null)
            {
                throw new Exceptions.ValidationException($"The user with ID '{userId}' does not exist.");
            }
        }
    }