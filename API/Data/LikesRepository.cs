using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _dataContext;
        public LikesRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _dataContext.Likes.FindAsync(sourceUserId,likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParms)
        {
            var users = _dataContext.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _dataContext.Likes.AsQueryable();

            if(likesParms.Predicate == "liked") //Users which currently logged in user has liked
            {
                likes = likes.Where(like => like.SourceUserId == likesParms.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            if(likesParms.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParms.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers,
                likesParms.PageNumber,likesParms.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _dataContext.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}