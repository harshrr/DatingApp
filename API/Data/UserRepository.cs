using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using API.Helpers;
using System;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _conecxt;
        private readonly IMapper _mapper;
        public UserRepository(DataContext conecxt, IMapper mapper)
        {
            _mapper = mapper;
            _conecxt = conecxt;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {

            return await _conecxt.Users
                    .Where(x => x.UserName == username)
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync();

        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _conecxt.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUserName);
            query = query.Where(u => u.Gender == userParams.Gender);
            
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            
            //above code generate expression tree

            query = userParams.OrderBy switch 
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.
                            ConfigurationProvider).AsNoTracking(), 
                            userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _conecxt.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _conecxt.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _conecxt.Users
                    .Include(p => p.Photos)
                    .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _conecxt.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _conecxt.Entry(user).State = EntityState.Modified;
        }
    }
}