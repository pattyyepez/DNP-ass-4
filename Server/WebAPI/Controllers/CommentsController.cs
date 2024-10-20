using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;

        public CommentsController(ICommentRepository commentRepository, IUserRepository userRepository, IPostRepository postRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _postRepository = postRepository;
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto request)
        {
            await VerifyUserExistsAsync(request.UserId);  // Verifica si el usuario existe
            await VerifyPostExistsAsync(request.PostId);  // Verifica si el post existe
            
            var comment = new Comment(request.Body, request.PostId, request.UserId);
            var createdComment = await _commentRepository.AddAsync(comment);

            var user = await _userRepository.GetSingleAsync(request.UserId);
            var commentDto = new CommentDto
            {
                Id = createdComment.Id,
                Body = createdComment.Body,
                UserName = user.Username
            };

            return CreatedAtAction(nameof(GetComment), new { id = commentDto.Id }, commentDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _commentRepository.GetSingleAsync(id);
            if (comment == null)
                return NotFound();

            var user = await _userRepository.GetSingleAsync(comment.UserId);
            return new CommentDto
            {
                Id = comment.Id,
                Body = comment.Body,
                UserName = user.Username
            };
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommentDto>> GetComments([FromQuery] int? postId = null, [FromQuery] int? userId = null)
        {
            var comments = _commentRepository.GetManyAsync()
                .Where(c => (!postId.HasValue || c.PostId == postId)
                         && (!userId.HasValue || c.UserId == userId))
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    UserName = _userRepository.GetSingleAsync(c.UserId).Result.Username
                });

            return Ok(comments);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await _commentRepository.DeleteAsync(id);
            return NoContent();
        }

        // Método para verificar si el usuario existe
        private async Task VerifyUserExistsAsync(int userId)
        {
            var user = await _userRepository.GetSingleAsync(userId);
            if (user == null)
            {
                throw new Exceptions.ValidationException($"The user with ID '{userId}' does not exist.");
            }
        }

        // Método para verificar si el post existe
        private async Task VerifyPostExistsAsync(int postId)
        {
            var post = await _postRepository.GetSingleAsync(postId);
            if (post == null)
            {
                throw new Exceptions.ValidationException($"The post with ID '{postId}' does not exist.");
            }
        }
    }