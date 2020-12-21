using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

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

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _conecxt.Users
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

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