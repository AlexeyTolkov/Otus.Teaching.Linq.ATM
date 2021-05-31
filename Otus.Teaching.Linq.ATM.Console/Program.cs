using System;
using System.Linq;
using Otus.Teaching.Linq.ATM.Core.Services;
using Otus.Teaching.Linq.ATM.DataAccess;

namespace Otus.Teaching.Linq.ATM.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Старт приложения-банкомата...");

            var atmManager = CreateATMManager();

            var login = "lee";
            var password = "222";

            // 1
            atmManager.UserLogIn(login, password);
            // 2
            atmManager.PrintUserAccountsInfo();
            // 3
            atmManager.PrintUserAccountsInfoWithHistory();
            // 4
            atmManager.PrintInputAccountOperations();
            // 5
            atmManager.PrintUserBalanceInquery(100000m);

            System.Console.WriteLine("Завершение работы приложения-банкомата...");
        }

        static ATMManager CreateATMManager()
        {
            using var dataContext = new ATMDataContext();
            var users = dataContext.Users.ToList();
            var accounts = dataContext.Accounts.ToList();
            var history = dataContext.History.ToList();
                
            return new ATMManager(accounts, users, history);
        }
    }
}