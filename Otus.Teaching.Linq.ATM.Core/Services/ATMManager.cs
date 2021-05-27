using System.Collections.Generic;
using Otus.Teaching.Linq.ATM.Core.Entities;
using System.Linq;

namespace Otus.Teaching.Linq.ATM.Core.Services
{
    public class ATMManager
    {
        public IEnumerable<Account> Accounts { get; private set; }
        
        public IEnumerable<User> Users { get; private set; }
        
        public IEnumerable<OperationsHistory> History { get; private set; }
        
        public ATMManager(IEnumerable<Account> accounts, IEnumerable<User> users, IEnumerable<OperationsHistory> history)
        {
            Accounts = accounts;
            Users = users;
            History = history;
        }

        //TODO: Добавить методы получения данных для банкомата

        public User GetOwnerAccountHistoryOwner(OperationsHistory history)
        {
            return Users.Where(x => x.Id == history.AccountId)
                 .FirstOrDefault();
        }

    }
}