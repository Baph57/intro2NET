using System;
using System.Threading.Tasks;
using intro2NET.API.Models;
using Microsoft.EntityFrameworkCore;

namespace intro2NET.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null) return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (System.Security.Cryptography.HMACSHA512 hmacSHA =
                new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                byte[] computedHash = hmacSHA.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }

            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            //out keyword specifies a reference
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //using allows for the information to be 'released' or deleted after use.
            using (System.Security.Cryptography.HMACSHA512 hmacSHA =
                new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmacSHA.Key;
                passwordHash = hmacSHA.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExistsCheck(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username)) return true;

            return false;
        }
    }
}
