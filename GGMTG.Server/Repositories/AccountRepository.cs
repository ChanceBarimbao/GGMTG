using GGMTG.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GGMTG.Server.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly Context _context;

        public AccountRepository(Context context)
        {
            _context = context;
        }

        public Account? FindAccountByEmail(string email)
        {
            return _context.Accounts.AsNoTracking().FirstOrDefault(account => account.Email == email);
        }

        public Account? FindAccountById(int id)
        {
            return _context.Accounts.AsNoTracking().FirstOrDefault(account => account.Id == id);
        }

        public bool AddAccount(Account account)
        {
            _context.Accounts.Add(account);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteAccount(Account account)
        {
            _context.Accounts.Remove(account);
            return _context.SaveChanges() > 0;
        }
    }
}
