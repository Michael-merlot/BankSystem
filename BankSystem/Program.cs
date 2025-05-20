using BankSystem;
using System;
using System.ComponentModel.Design;

namespace BankSystem
{
    class MainProgram
    {
        static void Main()
        {
            Bank bank = new Bank();
            string saveFileName = "bank_data.json";

            if (File.Exists(saveFileName))
            {
                Console.WriteLine("Обнаружен файл сохранения. Загружаем данные...");
                bank = Bank.LoadFromFile(saveFileName);
            }
            else
            {
                Console.WriteLine("Файл сохранения не найден или поврежден. Создайте новый экземпляр банка.");
                bank = new Bank();
            }

            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey();

            bool exitProgram = false;

            while (!exitProgram)
            {
                Console.Clear();
                Console.WriteLine("=== БАНКОВСКАЯ СИСТЕМА ===");
                Console.WriteLine("1. Создать новый счёт");
                Console.WriteLine("2. Войти в существующий счёт");
                Console.WriteLine("3. Показать все счета в банке");
                Console.WriteLine("4. Сохранить данные");
                Console.WriteLine("5. Загрузить данные");
                Console.WriteLine("0. Выход из программы");
                Console.Write("\nВыберите действие (1-5): ");

                switch (Console.ReadLine())
                {
                    case "1":
                        CreateNewAccount(bank);
                        break;

                    case "2":
                        LoginToAccount(bank);
                        break;

                    case "3":
                        bank.DisplayAllAccount();
                        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Write("Введите имя файла для сохранения (или нажмите Enter для стандартного): ");
                        string saveFile = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(saveFile))
                        {
                            saveFile = saveFileName;
                        }
                        bank.SaveToFile(saveFile);
                        Console.WriteLine("\nНажмите на любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;

                    case "5":
                        Console.Write("Введите имя файла для загрузки (или нажмите Enter для стандартного): ");
                        string loadFile = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(loadFile))
                        {
                            loadFile = saveFileName;
                        }

                        if (File.Exists(loadFile))
                        {
                            Console.WriteLine("ВНИМАНИЕ: Все несохраненные данные будут потеряны");
                            Console.Write("Продолжить? (да/нет): ");

                            if (Console.ReadLine().ToLower() == "да")
                            {
                                bank = Bank.LoadFromFile(loadFile);
                                Console.WriteLine($"Данные из файла {loadFile} успешно загружены.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Файл {loadFile} не найден");
                        }

                        Console.WriteLine("\nНажмите на любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;

                    case "0":
                        Console.Write("Хотите сохранить данные перед выходом? (да/нет): ");
                        if (Console.ReadLine().ToLower() == "да")
                        {
                            bank.SaveToFile(saveFileName);
                        }
                        exitProgram = true;
                        break;

                    default:
                        Console.WriteLine("Неверный выбор! Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public static void CreateNewAccount(Bank bank)
        {
            Console.Clear();
            Console.WriteLine("=== СОЗДАНИЕ НОВОГО СЧЁТА ===");
            Console.WriteLine("1. Сберегательный счёт");
            Console.WriteLine("2. Кредитный счёт");
            Console.Write("Выберите действие (1-2): ");
            string accountType = Console.ReadLine();

            Console.Write("Введите имя пользователя: ");
            string owner = Console.ReadLine();

            Console.Write("Придумайте PIN-код (4 цифры): ");
            int pin;
            while (!int.TryParse(Console.ReadLine(), out pin) || pin < 1000 || pin > 9999)
            {
                Console.Write("Некорректный PIN-код! Введите 4 цифры: ");
            }

            if (accountType == "1")
            {
                decimal initialBalance = GetDecimalInpuy("Введите начальный баланс: ");
                decimal initialRate = GetInterestRateInput();

                IAccount newAccount = new SavingAccount(owner, initialBalance, initialRate, pin);
                bank.AddAccount(newAccount);

                SavingAccount account = new SavingAccount(owner, initialBalance, initialRate, pin);

                account.AddNotification($"Добро пожаловать в банк! Ваш сберегательный счёт создан с начальным балансом: {initialBalance}", NotificationType.SystemMessage);
                Console.WriteLine("Сберегательный счёт был успешно создан!");
            }
            else if (accountType == "2")
            {
                decimal creditLimit = GetDecimalInpuy("Введите кредитный лимит: ");

                IAccount newAccount = new CreditAccount(owner, creditLimit, pin);
                bank.AddAccount(newAccount);
                Console.WriteLine("Кредитный счёт успешно создан!");
            }
            else  
            {
                Console.WriteLine("Неверный выбор счёта");
            }

            Console.WriteLine("\nНажмите клавишу для продолжения...");
            Console.ReadKey();
        }

        static decimal GetDecimalInpuy(string prompt)
        {
            decimal value;
            Console.Write(prompt);

            while (!decimal.TryParse(Console.ReadLine(), out value) || value < 0)
            {
                Console.Write("Некорректное значение! Повторите ввод: ");
            }

            return value;
        }

        static decimal GetInterestRateInput()
        {
            decimal rate;
            Console.Write("Введите процентную ставку (от 0 до 1, например 0,05): ");

            while (!decimal.TryParse(Console.ReadLine(), out rate) || rate <= 0 || rate >= 1)
            {
                Console.Write("Некорректная ставка! Введите значение от 0 до 1: ");
            }

            return rate;
        }

        static void LoginToAccount(Bank bank)
        {
            Console.Clear();
            Console.WriteLine("=== ВХОД В СУЩЕСТВУЮЩИЙ СЧЁТ ===");
            Console.Write("Введите имя владельца счёта: ");

            string owner = Console.ReadLine();

            IAccount account = bank.FindAccount(owner);

            if (account == null)
            {
                Console.WriteLine("Счёт не найден");
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите PIN-код: ");
            int pin;
            int attempts = 3;

            while (attempts > 0)
            {
                if (int.TryParse(Console.ReadLine(), out pin))
                {
                    try
                    {
                        account.WithDraw(0, pin);
                        AccountOperationMenu(bank, account, pin);
                        return;
                    }
                    catch
                    {
                        attempts--;
                        if (attempts > 0)
                        {
                            Console.Write($"Неверный PIN-код! Осталось попыток: {attempts}. Введите PIN-код: ");
                        }
                    }
                }
                else
                {
                    attempts--;
                    if (attempts > 0)
                    {
                        Console.Write($"Некорректный ввод! Осталось попыток: {attempts}. Введите PIN-код: ");
                    }
                }
            }

            Console.WriteLine("\nСлишком много неудачных попыток. Доступ заблокирован.");
            Console.WriteLine("По соображениям безопасности счёт должен быть удален.");
            Console.Write($"Для подтвреждения удаления введите 'УДАЛИТЬ': ");

            string information = Console.ReadLine();

            if (information == "УДАЛИТЬ")
            {
                IAccount deletedAccount = bank.DeleteAccount(owner);
                if (deletedAccount != null)
                {
                    Console.WriteLine($"Счёт для {owner} успешно удален!");
                }
                else
                {
                    Console.WriteLine($"Произошла ошибка при удалении");
                }
            }
            else
            {
                Console.WriteLine("Вы не подтввердили удаление, но счёт всё равно будет заблокирован.");
                bank.DeleteAccount(owner);
            }

            Console.WriteLine("\nНажмите клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void AccountOperationMenu(Bank bank, IAccount account, int pin)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                account.DisplayInfo();

                Console.WriteLine("\n=== МЕНЮ ОПЕРАЦИЙ ===");
                Console.WriteLine("1. Внести деньги");
                Console.WriteLine("2. Снять деньги");
                Console.WriteLine("3. Перевести деньги другу");
                Console.WriteLine("4. История операций");
                Console.WriteLine("5. Рассчитать налог");
                Console.WriteLine("6. Уведомления и настройки");

                if (account is SavingAccount)
                {
                    Console.WriteLine("7. Начислить проценты");
                }
                Console.WriteLine("0. Выход в главное меню");

                Console.Write("\nВыберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        DepositMoney(account, pin);
                        break;
                    case "2":
                        WithDrawMoney(account, pin);
                        break;
                    case "3":
                        TransferMoney(bank, account, pin);
                        break;
                    case "4":
                        if (account is BankAccount bankAccount)
                        {
                            bankAccount.PrintTransaction();
                            Console.WriteLine("\nНажмите клавишу для продолжения...");
                            Console.ReadKey();
                        }
                        break;
                    case "5":
                        if (account is BankAccount bankAccount1)
                        {
                            decimal tax = bankAccount1.CalculateTax();
                            Console.WriteLine($"Сумма налога {tax}");
                            Console.WriteLine("\nНажмите клавишу для продолжения...");
                            Console.ReadKey();
                        }
                        break;
                    case "6":
                        NotificationMenu(account, pin);
                        break;
                    case "7":
                        if (account is SavingAccount savingAccount)
                        {
                            savingAccount.ApplyInterest();
                            Console.WriteLine("Проценты успешно начислены!");
                            Console.WriteLine("\nНажмите клавишу для продолжения...");
                            Console.ReadKey();
                        }
                        break;
                    default:
                        Console.WriteLine("Неверный выбор! Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void DepositMoney(IAccount account, int pin)
        {
            Console.Write("Введите сумму для внесения: ");

            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                try
                {
                    account.Deposit(amount, pin);
                    Console.WriteLine($"Внесено {amount}. Операция успешна!");
                }
                catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
            }
            else
            {
                Console.WriteLine("Введена некорректная сумма!");
            }
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void WithDrawMoney(IAccount account, int pin)
        {
            Console.Write("Введите сумму для снятия: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                try
                {
                    account.WithDraw(amount, pin);
                    Console.WriteLine($"Снято {amount}. Операция успешна!");
                }
                catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
            }
            else
            {
                Console.WriteLine("Введена некорректная сумма!");
            }
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void TransferMoney(Bank bank, IAccount fromAccount, int pin)
        {
            Console.Write("Введите имя получателя: ");
            string toOwner = Console.ReadLine();

            IAccount toAccount = bank.FindAccount(toOwner);
            if (toAccount == null)
            {
                Console.WriteLine("Получатель не найден");
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите сумму перевода: ");

            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                try
                {
                    bank.Transfer(fromAccount.GetOwner(), toOwner, amount, pin);
                    Console.WriteLine($"Перевод {amount} получателю {toOwner} выполнен успешно!");
                }
                catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
            }
            else
            {
                Console.WriteLine("Введена некорректная сумма!");
            }
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void ShowAvailableSaves()
        {
            Console.Clear();
            Console.WriteLine("=== ДОСТУПНЫЕ СОХРАНЕНИЯ === ");

            string saveFileName = "bank_data.json";

            if(File.Exists(saveFileName))
            {
                FileInfo fileInfo = new FileInfo(saveFileName);
                Console.WriteLine($"Основное сохранение: {saveFileName}");
                Console.WriteLine($"Дата изменения: {fileInfo.LastWriteTime}");
            }
        }
        private static void NotificationMenu(IAccount account, int pin)
        {
            if (account is BankAccount bankAccount)
            {
                bool exit = false;

                while (!exit)
                {
                    Console.Clear();
                    Console.WriteLine("=== МЕНЮ УВЕДОМЛЕНИЙ ===");
                    Console.WriteLine($"Уведомления: {(bankAccount.NotificationEnabled ? "Включены" : "Выключены")}");
                    Console.WriteLine($"Порог для крупных операций: {bankAccount.LargeTransactionThreshold}");
                    Console.WriteLine($"Минимальный баланс для уведомления: {bankAccount.MinBalanceThreshold}");
                    Console.WriteLine($"\n1. Просмотреть уведомления");
                    Console.WriteLine("2. Включить/Выключить уведомления");
                    Console.WriteLine("3. Изменить порог для крупных операций");
                    Console.WriteLine("4. Изменить минимальный баланс");
                    Console.WriteLine("0. Назад");

                    Console.Write("\nВыберите действие: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            bankAccount.ShowNotifications();
                            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                            break;
                        case "2":
                            bankAccount.NotificationEnabled = !bankAccount.NotificationEnabled;
                            Console.WriteLine($"Уведомления: {(bankAccount.NotificationEnabled ? "Включены" : "Выключены")}");
                            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                            break;
                        case "3":
                            Console.Write("Введите новый порог для крупных операций: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal value) && value > 0)
                            {
                                bankAccount.LargeTransactionThreshold = value;
                                Console.WriteLine($"Порог установлен на {value}");
                            }
                            else
                            {
                                Console.WriteLine("Введеное некорректное значние");
                            }
                            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                            break;
                        case "4":
                            Console.Write("Введите новый минимальный баланс для уведомления: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal minBalance) && minBalance >= 0)
                            {
                                bankAccount.MinBalanceThreshold = minBalance;
                                Console.WriteLine($"Минимальный баланс установлен на {minBalance}");
                            }
                            else
                            {
                                Console.WriteLine("Введено некорректное значение");
                            }
                            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Уведомления недоступны для этого типа счёта");
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }
    }
}