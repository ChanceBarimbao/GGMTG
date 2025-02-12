using GGMTG.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GGMTG.Server.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        Context _context;
        public AccountRepository(Context context) {
            _context = context;
        }
        /// <inheritdoc/>
        public Account? FindAccountByUsername(string username)
        {
            return _context.Accounts.Where(account => account.Username == username).FirstOrDefault();
        }
        // Finds account by email. Returns an account if found, otherwise null.
        public Account? FindAccountByEmail(string email)
        {
            return _context.Accounts.Where(account => account.Email == email).FirstOrDefault();
        }

        // Finds an account by Id. This should be considered the fastest
        public Account? FindAccountById(int id)
        {
            return _context.Accounts.Find(id);
        }

        // returns true if success, otherwise false.
        public bool AddAccount(Account account)
        {
            var result = _context.Accounts.Add(account);
            if (result != null && result.State == EntityState.Added)
            {
                return (_context.SaveChanges()) > 0;
            }
            return false;
        }
        public bool UpdateAccount(Account account)
        {
            var result = _context.Accounts.Update(account);
            if (result != null && result.State == EntityState.Modified)
            {
                return (_context.SaveChanges()) > 0;
            }
            return false;
        }
        public bool DeleteAccount(Account account)
        {
            //if (account.Projects != null)
            //{
            //    _context.Projects.RemoveRange(account.Projects);
            //}
            var result = _context.Accounts.Remove(account);
            if (result != null && result.State == EntityState.Deleted)
            {
                return (_context.SaveChanges()) > 0;
            }
            return false;
        }
    }
}
