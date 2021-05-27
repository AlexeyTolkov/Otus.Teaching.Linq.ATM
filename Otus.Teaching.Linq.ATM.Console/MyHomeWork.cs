using System;
using System.IO;
using System.Linq;
using Otus.Teaching.Linq.ATM.Core.Entities;
using Otus.Teaching.Linq.ATM.Core.Services;
using Otus.Teaching.Linq.ATM.DataAccess;

namespace Otus.Teaching.Linq.ATM.Console
{
    public class MyHomeWork
    {
        public ATMManager ATMManager { get; private set; }

        public MyHomeWork(ATMManager atmManager)
        {
            ATMManager = atmManager;
        }

        public void Run()
        {
            System.Console.Write("Login: ");
            var login = System.Console.ReadLine();

            System.Console.Write("Password: ");
            var password = System.Console.ReadLine();

            FindUserAndProcees(login, password);
        }

        private void FindUserAndProcees(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
                return;

            var userList = ATMManager.Users
                .Where(x => x.Login == login)
                .Where(y => y.Password == password)
                .ToList();

            foreach (var user in userList)
            {
                // 1,2,3
                PrintUserDetailedInfo(user);
            }

            // 4
            PrintInputAccountOperations();

            // 5
            PrintUserBalanceInquery();
        }

        private void PrintUserBalanceInquery()
        {
            // 5. Вывод данных о всех пользователях у которых на счёте сумма больше N(N задаётся из вне и может быть любой);
            System.Console.WriteLine($"5. Account balance info:");
            System.Console.WriteLine($"enter the amount to get accounts with greater balance:");

            var balanceInquaryAmount = decimal.Parse(System.Console.ReadLine());

            var userAccounts = ATMManager.Accounts.Where(x => x.CashAll > balanceInquaryAmount)
                                                  .Join(ATMManager.Users,
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

        private void PrintInputAccountOperations()
        {
            // 4. Вывод данных о всех операциях пополнения счёта с указанием владельца каждого счёта;
            System.Console.WriteLine($"4. Accounts input history");

            var operationHistoryWithOwner = ATMManager.History
                .Where(x => x.OperationType == OperationType.InputCash)
                .ToList();

            var operationNo = 1;
            foreach (var historyRecord in operationHistoryWithOwner)
            {
                System.Console.WriteLine($"\t4.{ operationNo}. {historyRecord}; owner: " +
                                         $"{ATMManager.GetOwnerAccountHistoryOwner(historyRecord)}");
                operationNo++;
            }
        }

        private void PrintUserDetailedInfo(User user)
        {
            // 1. Вывод информации о заданном аккаунте по логину и паролю;
            System.Console.WriteLine($"1. Login info: {user}");

            // 2. Вывод данных о всех счетах заданного пользователя;
            System.Console.WriteLine($"2. Accounts info");
            PrintUserAccountsInfo(user);

            // 3. Вывод данных о всех счетах заданного пользователя, включая историю по каждому счёту;
            System.Console.WriteLine($"3. Accounts history");
            PrintUserAccountsInfo(user, true);
        }

        private void PrintUserAccountsInfo(User user, bool includeAccountsHistory = false)
        {
            var userAccountsList = ATMManager.Accounts
                .Where(x => x.UserId == user.Id).ToList();

            var accountNo = 1;
            foreach (var account in userAccountsList)
            {
                System.Console.WriteLine($"\t acc#{accountNo}. {account}");

                if (includeAccountsHistory)
                {
                    PrintUserAccountsHistory(account);
                }

                accountNo++;
            }
        }

        private void PrintUserAccountsHistory(Account account)
        {
            var operationHistory = ATMManager.History
                .Where(x => x.AccountId == account.Id).ToList();

            var historyRecNo = 1;
            foreach (var historyRecord in operationHistory)
            {
                System.Console.WriteLine($"\t\t3.{historyRecNo}. {historyRecord}");
            }
        }
    }   
}
