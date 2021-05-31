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

        private User _currentUser;

        private User GetAccountHistoryOwner(OperationsHistory history)
        {
            //TODO:
            var userId = Accounts.Where(x => x.Id == history.AccountId)
                    .FirstOrDefault().UserId;

            return Users.Where(x => x.Id == userId)
                        .FirstOrDefault();
        }

        /// <summary>
        /// 1. Вывод информации о заданном аккаунте по логину и паролю;
        /// </summary>
        public void UserLogIn(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                _currentUser = null;
                return;
            }

            _currentUser = Users
                .Where(x => x.Login == login && x.Password == password)
                .FirstOrDefault();

            System.Console.WriteLine($"1. Login info: {_currentUser}");
        }

        /// <summary>
        /// 2. Вывод данных о всех счетах заданного пользователя;
        /// </summary>
        public void PrintUserAccountsInfo()
        {
            if (_currentUser is null)
                throw new System.ArgumentNullException("user");

            System.Console.WriteLine($"2. Accounts info");

            var userAccountsList = Accounts
                .Where(x => x.UserId == _currentUser.Id).ToList();

            var accountNo = 1;
            foreach (var account in userAccountsList)
            {
                System.Console.WriteLine($"\t acc#{accountNo}. {account}");
                accountNo++;
            }
        }

        /// <summary>
        /// 3. Вывод данных о всех счетах заданного пользователя, включая историю по каждому счёту;
        /// </summary>
        public void PrintUserAccountsInfoWithHistory()
        {
            if (_currentUser is null)
                throw new System.ArgumentNullException("user");

            System.Console.WriteLine($"3. Accounts history");

            var userAccountsList = Accounts
                .Where(x => x.UserId == _currentUser.Id)
                .Select(x => new
                {
                    openingDate = x.OpeningDate,
                    cashAll = x.CashAll,
                    history = History
                    .Where(y => y.AccountId == x.Id)
                    .Select(y => new
                    {
                        operationType = y.OperationType,
                        operationDate = y.OperationDate,
                        cashSum = y.CashSum
                    })
                    .ToList()
                })
                .ToList();

            var accountNo = 1;
            foreach (var account in userAccountsList)
            {
                System.Console.WriteLine($"\t acc#{accountNo}. Opening date: {account.openingDate} - cash: {account.cashAll}");

                var historyRecNo = 1;
                foreach (var tx in account.history)
                {
                    System.Console.WriteLine($"\t\t3.{historyRecNo}. Operation type: {tx.operationType} - opening date: {tx.operationDate} - cash: {tx.cashSum}");
                    historyRecNo++;
                }

                accountNo++;
            }
        }

        /// <summary>
        /// 4. Вывод данных о всех операциях пополнения счёта с указанием владельца каждого счёта;
        /// </summary>
        public void PrintInputAccountOperations()
        {
            System.Console.WriteLine($"4. Accounts input history");

          /*  var operationHistoryWithOwner = History
                .Where(x => x.OperationType == OperationType.InputCash)
                .ToList();

            var operationNo = 1;
            foreach (var historyRecord in operationHistoryWithOwner)
            {
                System.Console.WriteLine($"\t4.{ operationNo}. {historyRecord} => owner: " +
                                        //TODO:
                                         $"{GetAccountHistoryOwner(historyRecord)}");
                operationNo++;
            }
            */

            var operationHistoryWithOwner =
                from tx in History
                where tx.OperationType == OperationType.InputCash
                join acc in Accounts on tx.AccountId equals acc.Id
                join user in Users on acc.UserId equals user.Id
                select new
                {
                    operationDate = tx.OperationDate,
                    cashSum = tx.CashSum,
                    owner = new
                    {
                        firstName = user.FirstName,
                        middleName = user.MiddleName,
                        surName = user.SurName
                    }
                };

            var operationNo = 1;
            foreach (var historyRecord in operationHistoryWithOwner)
            {
                System.Console.WriteLine($"\t4.{ operationNo}. opening date: {historyRecord.operationDate} - cash: {historyRecord.cashSum} => owner: {historyRecord.owner.firstName} {historyRecord.owner.middleName} {historyRecord.owner.surName}");
                operationNo++;
            }

        }

        /// <summary>
        /// 5. Вывод данных о всех пользователях у которых на счёте сумма больше N(N задаётся из вне и может быть любой);
        /// </summary>
        public void PrintUserBalanceInquery(decimal balanceInquaryAmount)
        {
            System.Console.WriteLine($"5. Account balance info:");
            System.Console.WriteLine($"Showing all accounts balance with greater than: {balanceInquaryAmount}");

            var userAccounts = Accounts.Where(x => x.CashAll > balanceInquaryAmount)
                                       .Join(Users,
                                           acc => acc.UserId,
                                           user => user.Id,
                                           (acc, user) => new
                                           {
                                               UserName = $"{user.FirstName} {user.SurName} {user.MiddleName}",
                                               Balance = acc.CashAll
                                           }
                                           );

            foreach (var userAccount in userAccounts)
            {
                System.Console.WriteLine($"{userAccount.UserName} has balance: {userAccount.Balance}");
            }
        }
    }
}