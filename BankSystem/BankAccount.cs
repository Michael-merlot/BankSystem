using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankSystem
{
    public interface IAccount
    {
        void Deposit(decimal amount, int pin);
        void WithDraw(decimal amount, int pin);
        void DisplayInfo();
        string GetOwner();
    }

    public class Bank
    {
        private List<IAccount> accounts = new List<IAccount>();

        public void AddAccount(IAccount account)
        {
            accounts.Add(account);
            Logger.Log($"Добавлен новый счёт для {account.GetOwner()}");
        }

        public IAccount DeleteAccount(string ownerName)
        {
            IAccount accountToDelete = FindAccount(ownerName);

            if (accountToDelete != null)
            {
                accounts.Remove(accountToDelete);
                Logger.Log($"Удален счёт для {accountToDelete.GetOwner()}");
                return accountToDelete;
            }
            else
            {
                Logger.Log($"Попытка удалить несуществующий счёт для {ownerName}");
                return null;
            }
        }

        public IAccount FindAccount(string ownerName)
        {
            return accounts.FirstOrDefault(account => string.Equals(account.GetOwner(), ownerName, StringComparison.OrdinalIgnoreCase));
        }

        public int GetAccountCount()
        {
            return accounts.Count;
        }

        public void Transfer(string fromOwner, string toOwner, decimal amount, int pin)
        {
            var fromAccount = FindAccount(fromOwner);
            var toAccount = FindAccount(toOwner);

            if (fromAccount == null || toAccount == null)
            {
                throw new Exception("Один из счетов не найден.");
            }

            fromAccount.WithDraw(amount, pin);
            toAccount.Deposit(amount, pin);
            Logger.Log($"Перевод {amount} от {fromOwner} к {toOwner}");
        }

        public void DisplayAllAccount()
        {
            Console.WriteLine("=== Все счета в банке ===");
            foreach (var account in accounts)
            {
                account.DisplayInfo();
            }
        }

        public BankData ToBankData()
        {
            BankData bankData = new BankData();

            foreach (var account in accounts)
            {
                AccountData accountData;

                if (account is SavingAccount savingAccount)
                {
                    accountData = new AccountData
                    {
                        AccountType = "Saving",
                        Owner = savingAccount.Owner,
                        Balance = savingAccount.Balance,
                        PinCode = savingAccount.GetPinCode(),
                        InterestRate = savingAccount.GetInterestRate(),
                        LargeTransactionThreshold = savingAccount.LargeTransactionThreshold,
                        MinBalanceThreshold = savingAccount.MinBalanceThreshold,
                        // NotificationsEnabled = savingAccount.NotificationsEnabled
                    };
                }
                else if (account is CreditAccount creditAccount)
                {
                    accountData = new AccountData
                    {
                        AccountType = "Credit",
                        Owner = creditAccount.Owner,
                        Balance = creditAccount.Balance,
                        PinCode = creditAccount.GetPinCode(),
                        CreditLimit = creditAccount.GetCreditLimit(),
                        LargeTransactionThreshold = creditAccount.LargeTransactionThreshold,
                        MinBalanceThreshold = creditAccount.MinBalanceThreshold,
                        // NotificationsEnabled = savingAccount.NotificationsEnabled
                    };
                }
                else
                {
                    continue;
                }

                if (account is BankAccount bankAccount)
                {
                    foreach (var transaction in bankAccount.GetTransactions())
                    {
                        accountData.Transactions.Add(new TransactionData
                        {
                            Date = transaction.Date,
                            Type = transaction.Type,
                            Amount = transaction.Amount,
                        });
                    }

                    foreach (var notification in bankAccount.GetNotifications())
                    {
                        accountData.Notifications.Add(new NotificationData
                        {
                            Date = notification.Date,
                            Message = notification.Message,
                            NotificationType = notification.Type.ToString(),
                            IsRead = notification.IsRead,
                        });
                    }
                }
                bankData.Accounts.Add(accountData);
            }
            return bankData;
        }

        public static Bank FromBankData(BankData bankData)
        {
            Bank bank = new Bank();
            int succesfullyAdded = 0;

            foreach (var accountData in bankData.Accounts)
            {
                try
                {
                    IAccount account = null;

                    if (accountData.AccountType == "Saving")
                    {
                        account = new SavingAccount(accountData.Owner, accountData.Balance, accountData.InterestRate, accountData.PinCode);

                        ((SavingAccount)account).LargeTransactionThreshold = accountData.LargeTransactionThreshold;
                        ((SavingAccount)account).MinBalanceThreshold = accountData.MinBalanceThreshold;
                        // ((SavingAccount)account).NotificationEnabled = accountData.NotificationsEnabled;

                        Console.WriteLine($"Создан сберегательный счёт для {accountData.Owner} с балансом: {accountData.Balance}");
                    }
                    else if (accountData.AccountType == "Credit")
                    {
                        account = new CreditAccount(accountData.Owner, accountData.CreditLimit, accountData.PinCode);

                        ((CreditAccount)account).Balance = accountData.Balance;
                        ((CreditAccount)account).LargeTransactionThreshold = accountData.LargeTransactionThreshold;
                        ((CreditAccount)account).MinBalanceThreshold = accountData.MinBalanceThreshold;
                        // ((CreditAccount)account).NotificationEnabled = accountData.NotificationsEnabled;

                        Console.WriteLine($"Создан кредитный счёт для {accountData.Owner} с балансом: {accountData.Balance}");
                    }

                    if (account != null)
                    {
                        bank.AddAccount(account);
                        succesfullyAdded++;

                        if (account is BankAccount BankAccount)
                        {
                            BankAccount.RestoreTransactions(accountData.Transactions);
                            BankAccount.RestoreNotifications(accountData.Notifications);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при создании сч`та: {ex.Message}");
                }
                
            }

            Console.WriteLine($"Успешно добавлено счетов: {succesfullyAdded} из {bankData.Accounts.Count}");
            return bank;
        }

        public void SaveToFile(string fileName)
        {
            try
            {
                Console.WriteLine($"Количество счетов в банке перед сохранением: {accounts.Count}");
                BankData data = ToBankData();

                Console.WriteLine($"Количество счетов в BankData: {data.Accounts.Count}");
                if (data.Accounts.Count > 0)
                {
                    Console.WriteLine("Детали счетов");

                    foreach (var account in data.Accounts)
                    {
                        Console.WriteLine($"- Тип: {account.AccountType}, Владелец: {account.Owner}, Баланс: {account.Balance} ");
                    }
                }

                string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

                Console.WriteLine("Первые 200 символов JSON: ");
                Console.WriteLine(jsonString.Length > 200 ? jsonString.Substring(0, 200) + "..." : jsonString);

                File.WriteAllText(fileName, jsonString);
                Console.WriteLine($"Данные успешно сохранены в файл {fileName}");
            }
            catch (Exception ex)  
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
                Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
            }
        }

        public static Bank LoadFromFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    Console.WriteLine($"Файл {fileName} найден. Читаем содержимое...");
                    string jsonString = File.ReadAllText(fileName);
                    Console.WriteLine($"Файл прочитан, длина содержимого: {jsonString.Length} символов");

                    BankData bankData = JsonSerializer.Deserialize<BankData>(jsonString);
                    Console.WriteLine($"Десериалзиация успешна, найдено счетов: {bankData.Accounts.Count}")
                        ;
                    Bank bank = FromBankData(bankData);
                    Console.WriteLine($"Данные успешно загружены из файла {fileName}");
                    Console.WriteLine($"Счетов в банке: {bank.GetAccountCount()}");
                    return bank;
                }
                else
                {
                    Console.WriteLine($"Файл {fileName} не найден. Создается новый счёт (банк)");
                    return new Bank();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
                Console.WriteLine($"Подробности: {ex.StackTrace}");
                return new Bank();
            }
        }

    }
    public abstract class BankAccount : IAccount
    {
        public string Owner;
        public decimal Balance;
        protected List<Transaction> TransactionList = new List<Transaction>();
        protected List<Notification> NotificationList = new List<Notification>();
        private int pinCode;

        public decimal LargeTransactionThreshold { get; set; } = 1000m;
        public decimal MinBalanceThreshold { get; set; } = 100m;
        public bool NotificationEnabled { get; set; } = true;

        public BankAccount(string owner, decimal initialBalance, int pin)
        {
            Owner = owner;
            Balance = initialBalance;
            pinCode = pin;
        }

        public virtual void Deposit(decimal amount, int pin)
        {
            if (!VerifyPin(pin))
            {
                throw new Exception("Неверный PIN-код!");
            }

            Balance += amount;
            TransactionList.Add(new Transaction("Депозит", amount));
            Logger.Log($"Счёт {Owner}: Пополнение на {amount}");

            if (NotificationEnabled && amount >= LargeTransactionThreshold)
            {
                AddNotification($"Крупное пополнение счёта на сумму {amount}", NotificationType.LargeDeposit);
            }
        }

        public virtual void WithDraw(decimal amount, int pin)
        {
            if (!VerifyPin(pin))
            {
                throw new Exception("Неверный PIN-код!");
            }
            else
            {
                if (amount <= Balance)
                {
                    Balance -= amount;
                    TransactionList.Add(new Transaction("Передача суммы", amount));
                    Logger.Log($"Счёт {Owner}: Передача суммы на {amount}");

                    if (NotificationEnabled && amount >= LargeTransactionThreshold)
                    {
                        AddNotification($"Крупное списание счёта на сумму {amount}", NotificationType.LargeWithdrawal);
                    }

                    if (NotificationEnabled && Balance <= MinBalanceThreshold)
                    {
                        AddNotification($"Баланс счёта достиг миниимального порога: {Balance}", NotificationType.LowBalance);
                    }
                }
                else
                {
                    throw new Exception($"Недостаточно средств. Баланс: {Balance}");
                }
            }
        }

        public void PrintTransaction() // надо будет доделать ИЛИ переделать
        {
            Console.WriteLine("----------");
            Console.WriteLine($"История операций для {Owner}");

            Console.WriteLine("|{0, -19}|{1, -15}|{2, 10}|", "Дата", "Тип операции", "Сумма");
            Console.WriteLine("|{0, -19}|{1, -15}|{2, 10}|", "-------------------", "--------------", "----------");

            foreach (Transaction transaction in TransactionList)
            {

                string symbol;

                if (transaction.Type == "Депозит")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    symbol = "⬆️";
                }
                else if (transaction.Type == "Снятие" || transaction.Type == "Передача суммы")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    symbol = "⬇️";
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    symbol = "💠";
                }

                Console.WriteLine($"{transaction.Date}: {transaction.Type} {transaction.Amount}");
                Console.ResetColor();
            }
            Console.WriteLine("-----------------------------------------------");
        }

        public int GetPinCode() => pinCode;
        public List<Transaction> GetTransactions() => TransactionList;
        public List<Notification> GetNotifications() => NotificationList;

        public void RestoreTransactions(List<TransactionData> transactionData)
        {
            TransactionList.Clear();

            foreach(var data in transactionData)
            {
                TransactionList.Add(new Transaction(data.Type, data.Amount)
                {
                    // Date = data.Date
                });
            }
        }

        public void RestoreNotifications(List<NotificationData> notificationData)
        {
            NotificationList.Clear();

            foreach (var data in notificationData)
            {
                NotificationType type = Enum.Parse<NotificationType>(data.NotificationType);
                Notification notification = (new Notification(data.Message, type)
                {
                    IsRead = data.IsRead
                });
                typeof(Notification).GetProperty("Date").SetValue(notification, data.Date);

                NotificationList.Add(notification);
            }
        }

        public bool CheckPin(int pin)
        {
            return VerifyPin(pin);
        }

        public bool VerifyPin(int pin)
        {
            return pin == pinCode;
        }

        public string GetOwner() => Owner;

        public virtual decimal CalculateTax()
        {
            return 0;
        }

        public virtual void DisplayInfo()
        {
            Console.WriteLine();
        }

        public void AddNotification(string message, NotificationType type)
        {
            Notification notification = new Notification(message, type);
            NotificationList.Add(notification);

            Console.WriteLine("\n=== НОВОЕ УВЕДОМЛЕНИЕ ===");
            notification.Display();
            Console.WriteLine("==========================\n");
        }

        public void ShowNotifications()
        {
            Console.WriteLine("\n=== УВЕДОМЛЕНИЯ ДЛЯ СЧЁТА ===");
            if (NotificationList.Count == 0)
            {
                Console.WriteLine("Нет уведомлений");
            }
            else
            {
                foreach (var notification in NotificationList)
                {
                    notification.Display();
                }
            }
            Console.WriteLine("=================================\n");
        }
    }

    public class SavingAccount : BankAccount
    {
        private decimal interestRate;

        public SavingAccount(string owner, decimal initialBalance, decimal interestRate, int pin) : base(owner, initialBalance, pin)
        {
            this.interestRate = interestRate;
        }
        public void ApplyInterest()
        {
            Balance = Balance * interestRate;
        }
        public override decimal CalculateTax()
        {
            decimal income = Balance * interestRate;
            return income * 0.13m;
        }
        public override void DisplayInfo()
        {
            Console.WriteLine("----------");
            Console.WriteLine($"Сберегательный счет. Владелец, {Owner}, Баланс: {Balance}, Процентная ставка: {interestRate:P}");
            Console.WriteLine("----------");
        }

        public decimal GetInterestRate() => interestRate;
    }

    public class CreditAccount : BankAccount
    {
        private decimal creditLimit;

        public CreditAccount(string owner, decimal creditLimit, int pin) : base(owner, 0, pin)
        {
            this.creditLimit = creditLimit;
        }

        public override void WithDraw(decimal amount, int pin)
        {
            if (!VerifyPin(pin))
            {
                throw new Exception("Неверный PIN-код!");
            }

            if (amount <= Balance + creditLimit)
            {
                Balance -= amount;
                TransactionList.Add(new Transaction("Снятие", amount));
                Logger.Log($"Счёт {Owner}: Снятие {amount}");
            }
            else
            {
                Console.WriteLine("Превышен кредитный лимит");
            }
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Кредитный счет. Владелец, {Owner}, Баланс: {Balance}, Кредитный лимит: {creditLimit}");
            Console.WriteLine("----------");
        }

        public decimal GetCreditLimit() => creditLimit;
    }

    public class BankData
    {
        public List<AccountData> Accounts { get; set; } = new List<AccountData>();
    }

    public class AccountData
    {
        public string AccountType { get; set; }
        public string Owner { get; set; }
        public decimal Balance { get; set; }
        public int PinCode { get; set; }
        public List<TransactionData> Transactions { get; set; } = new List<TransactionData>();
        public List<NotificationData> Notifications { get; set; } = new List<NotificationData>();

        public decimal InterestRate { get; set; }
        public decimal CreditLimit { get; set; }

        public decimal LargeTransactionThreshold { get; set; }
        public decimal MinBalanceThreshold { get; set; }
        public decimal NotificationsEnabled { get; set; }
    }

    public class TransactionData
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }

    public class NotificationData
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public bool IsRead { get; set; }
    }

    public enum NotificationType
    {
        LargeDeposit,
        LargeWithdrawal,
        LowBalance,
        AccountBlocked,
        SystemMessage
    }
    public class Notification
    {
        public DateTime Date { get; }
        public string Message { get; }
        public NotificationType Type { get; }
        public bool IsRead { get; set; } = false;

        public Notification(string Message, NotificationType Type)
        {
            Date = DateTime.Now;
            this.Message = Message;
            this.Type = Type;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }

        public void Display()
        {
            ConsoleColor color = ConsoleColor.White;

            switch (Type)
            {
                case NotificationType.LargeDeposit:
                    color = ConsoleColor.Green;
                    break;
                case NotificationType.LargeWithdrawal:
                    color = ConsoleColor.Red;
                    break;
                case NotificationType.LowBalance:
                    color = ConsoleColor.Yellow;
                    break;
                case NotificationType.SystemMessage:
                    color = ConsoleColor.Cyan;
                    break;
                case NotificationType.AccountBlocked:
                    color = ConsoleColor.Red;
                    break;
            }

            Console.ForegroundColor = color;
            Console.WriteLine($"{Date.ToString("dd.MM.yyyy HH:mm)")} - {Message}");
            Console.ResetColor();

            MarkAsRead();
        }
    }
    public class Transaction
    {
        public DateTime Date { get; }
        public string Type { get; }
        public decimal Amount { get; }

        public Transaction(string type, decimal amount)
        {
            Date = DateTime.Now;
            Type = type;
            Amount = amount;
        }
    }

    public class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine("----------");
            Console.WriteLine($"[LOG] {DateTime.Now}: {message}");
            Console.WriteLine("----------");
        }
    }
}
