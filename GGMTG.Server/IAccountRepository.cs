using Microsoft.EntityFrameworkCore;

namespace GGMTG.Server
{
    public interface IAccountRepository
    {
        // must be awaited immediately
        //Account? FindAccountByUsername(string username);
        Account? FindAccountByEmail(string email);
        Account? FindAccountById(int id);
        bool AddAccount(Account account);
        bool UpdateAccount(Account account);
        bool DeleteAccount(Account account);

    }
}
