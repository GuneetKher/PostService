using Microsoft.AspNetCore.Mvc;
using PostService.Models;
using PostService.Services;

namespace ModService.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly DbService _postService;
    public PostController(DbService postService)
    {
        this._postService = postService;
    }

    /// <summary>
    /// Create a post.
    /// </summary>
    /// <returns>Create a post.</returns>
    [HttpPost]
    public async Task<ActionResult<Post>> CreatePostAsync([FromBody] Post post)
    {
        try
        {
            await _postService.CreatePostAsync(post);

            // Return a 201 (Created) response with the newly created post
            return Ok();
        }
        catch (Exception ex)
        {
            // Return a BadRequest response with an error message
            return BadRequest($"Failed to create post: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetAllPostsAsync(string userId)
    {
        var posts = await _postService.GetAllPostsAsync(userId);

        return Ok(posts);
    }

    [HttpGet("allposts")]
    public async Task<ActionResult<IEnumerable<Post>>> AllPostsAsync()
    {
        var posts = await _postService.AllPostsAsync();

        return Ok(posts);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Post>>> GetPostsByUserIdAsync(string userId)
    {
        var posts = await _postService.GetPostsByUserIdAsync(userId);
        if (posts == null)
        {
            return NotFound();
        }
        return Ok(posts);
    }

    [HttpPut("{id}/ismod")]
    public async Task<IActionResult> UpdatePostIsModAsync(string id)
    {
        var result = await _postService.UpdatePostIsModAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpGet("comments/{postId}")]
    public async Task<ActionResult<List<Post>>> GetCommentsByParentId(string postId)
    {
        var comments = await _postService.GetCommentsByParentIdAsync(postId);
        if (comments == null)
        {
            return NotFound();
        }
        return comments;
    }

    [HttpPost("flag/{id}/{userId}")]
    public async Task<IActionResult> Flag(string id, string userId)
    {
        var post = await _postService.GetPostByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        post.FlaggedBy = post.FlaggedBy.Concat(new[] { userId }).ToArray();

        await _postService.UpdatePostAsync(id, post);

        return Ok();
    }
}
