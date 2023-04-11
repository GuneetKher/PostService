using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PostService.Models;

namespace PostService.Services
{
    public class DbService
    {
        private readonly IMongoCollection<Post> _posts;
        private readonly IMongoCollection<Post> _comments;
        public DbService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _posts = database.GetCollection<Post>(settings.PostCollectionName);
            _comments = database.GetCollection<Post>(settings.CommentsCollectionName);
        }

        public async Task CreatePostAsync(Post post)
        {
            await _posts.InsertOneAsync(post);
        }

        public async Task<List<Post>> GetAllPostsAsync(string userId)
        {
            var filter = Builders<Post>.Filter.And(
                Builders<Post>.Filter.Eq(p => p.IsMod, false),
                Builders<Post>.Filter.Not(Builders<Post>.Filter.AnyIn(p => p.FlaggedBy, new[] { userId })),
                Builders<Post>.Filter.Eq(p => p.ParentID, null)
            );

            var projection = Builders<Post>.Projection.Exclude(p => p.FlaggedBy);
            var posts = await _posts.Find(filter).Project<Post>(projection).ToListAsync();

            return posts;
        }

        public async Task<List<Post>> GetPostsByUserIdAsync(string userId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Author, userId);
            var posts = await _posts.Find(filter).ToListAsync();
            return posts;
        }

        public async Task<bool> UpdatePostIsModAsync(string id)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, id);
            var update = Builders<Post>.Update.Set(p => p.IsMod, true);
            var result = await _posts.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<List<Post>> GetCommentsByParentIdAsync(string parentId)
        {
            var filter = Builders<Post>.Filter.Eq(c => c.ParentID, parentId);
            var comments = await _posts.Find(filter).ToListAsync();
            return comments;
        }

        public async Task<Post> GetPostByIdAsync(string id)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, id);
            return await _posts.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdatePostAsync(string id, Post post)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, id);
            await _posts.ReplaceOneAsync(filter, post);
        }

        public async Task<List<Post>> AllPostsAsync()
        {
            var filter = Builders<Post>.Filter.Eq(p => p.IsMod, false);
            var posts = await _posts.Find(filter).ToListAsync();
            return posts;
        }
    }
}