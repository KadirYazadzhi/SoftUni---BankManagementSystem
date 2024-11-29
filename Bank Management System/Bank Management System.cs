using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Program {
    public static List<User> users = new List<User>();

    private const int MAX_LOAN_AMOUNT = 10000;
    private const double INTEREST_RATE = 0.03; 
    
    public static void Main() {
        ChooseFunction();
    }

    private static void ChooseFunction() {
        while (true) {
            switch (DisplayMenu()) {
                case 1:
                    CreateAccount();
                    break;
                case 2:
                    ChangePassword();
                    break;
                case 3:
                    Deposit();
                    break;
                case 4:
                    Withdraw();
                    break;
                case 5:
                    CheckBalance();
                    break;
                case 6:
                    ListAccounts();
                    break;
                case 7:
                    Transfer();
                    break;
                case 8:
                    ViewTransactionHistory();
                    break;
                case 9:
                    ApplyForLoan();
                    break;
                case 10:
                    RepayLoan();
                    break;
                case 11:
                    EndAccount();
                    break;
                case 12:
                    Console.WriteLine($"Exiting the system. Goodbye!");
                    return;
            }
        }
    }

    private static int DisplayMenu() {
        Console.WriteLine("\n--- Bank Account Management System ---");
        Console.WriteLine("1. Create Account");
        Console.WriteLine("2. Change Account Password");
        Console.WriteLine("3. Deposit");
        Console.WriteLine("4. Withdraw");
        Console.WriteLine("5. Check Balance");
        Console.WriteLine("6. List Accounts");
        Console.WriteLine("7. Transfer Funds");
        Console.WriteLine("8. View Transaction History");
        Console.WriteLine("9. Apply for Loan");
        Console.WriteLine("10. Repay Loan");
        Console.WriteLine("11. End Account");
        Console.WriteLine("12. Exit");
        
        Console.Write("Enter your choice: ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 12) {
            Console.Write("Invalid input. Please enter a number between 1 and 12: ");
        }

        return choice;
    }

    private static void CreateAccount() {
        string name = IsUserNotExist("Write your username: ", "Username already exists");
        
        Console.WriteLine("Write your password: ");
        string password = Console.ReadLine();

        User user = new User(name, password, 0, new List<string>(), 0);
        users.Add(user);
        Console.WriteLine("Account created successfully");
    }

    private static void ChangePassword() {
        string name = IsUserExist("Write your username: ", "User does not exist");
        
        Console.WriteLine("Enter your current password: ");
        string oldPassword = Console.ReadLine();
        
        Console.WriteLine("Enter your new password: ");
        string newPassword = Console.ReadLine();

        if (newPassword == oldPassword) {
            Console.WriteLine("Your current password is the same");
            return;
        }
        
        users[FindIndexProfile(name)].ChangePassword(newPassword);
        Console.WriteLine("Password changed successfully");
    }

    private static void Deposit() {
        string name = IsUserExist("Write your username: ", "User does not exist");
        if (!IsValidPassword(name)) {
            return;
        }
        
        Console.WriteLine("Write amount to deposit: ");
        int amount = int.Parse(Console.ReadLine());

        if (amount < 0) {
            Console.WriteLine("Amount cannot be negative");
            return;
        }

        if (amount > MAX_LOAN_AMOUNT) {
            Console.WriteLine("Amount cannot be greater than {0}", MAX_LOAN_AMOUNT);
            return;
        }
        
        users[FindIndexProfile(name)].ChangeBalance(amount, '+');
        users[FindIndexProfile(name)].AddTransaction($"Deposit {amount} to {name}");
        Console.WriteLine("Deposit successful");
        Console.WriteLine($"You added {amount} to your account balance");
    }
    
    private static void Withdraw() {
        string name = IsUserExist("Write your username: ", "User does not exist");
        if (!IsValidPassword(name)) {
            return;
        }
        
        Console.WriteLine("Write amount to withdraw: ");
        int amount = int.Parse(Console.ReadLine());

        if (amount < 0) {
            Console.WriteLine("Amount cannot be negative");
            return;
        }

        if (amount > MAX_LOAN_AMOUNT) {
            Console.WriteLine("Amount cannot be greater than {0}", MAX_LOAN_AMOUNT);
            return;
        }
        
        users[FindIndexProfile(name)].ChangeBalance(amount, '-');
        users[FindIndexProfile(name)].AddTransaction($"Withdraw {amount} from {name}");
        Console.WriteLine("Withdraw successful");
        Console.WriteLine($"You withdrew {amount} from your account balance");
    }
    
    private static void CheckBalance() {
        string name = IsUserExist("Write your username: ", "User does not exist");
        if (!IsValidPassword(name)) {
            return;
        }
        
        Console.WriteLine($"You have {users[FindIndexProfile(name)].GetBalances()} in your account balance");
    }
    
    private static void ListAccounts() {
        int num = 1;
        
        foreach (var user in users) {
            Console.WriteLine($"#{num}. {user.UserName} has {user.GetBalances()} balance with {user.TransactionHistory.Count} transactions.");
            Console.WriteLine($"All transaction history: ");
            Console.WriteLine(string.Join('\n', user.TransactionHistory));
            num++;
        }
    }

    private static void Transfer() {
        string senderName = IsUserExist("Write your username: ", "User does not exist");
        if (!IsValidPassword(senderName)) {
            return;
        }
        
        string receiverName = IsUserExist("Write receiver username: ", "Receiver does not exist");
        
        Console.WriteLine("Write amount to transfer: ");
        double amount = double.Parse(Console.ReadLine());

        if (amount < 0 || amount > MAX_LOAN_AMOUNT || amount > users[FindIndexProfile(receiverName)].GetBalances()) {
            Console.WriteLine("Amount is invalid");
            return;
        }
        
        users[FindIndexProfile(senderName)].ChangeBalance(amount, '-');
        users[FindIndexProfile(receiverName)].ChangeBalance(amount, '+');
        
        users[FindIndexProfile(senderName)].AddTransaction($"You transfer {amount} to {receiverName}");
        users[FindIndexProfile(receiverName)].AddTransaction($"You receive {amount} from {senderName}");
        
        Console.WriteLine("Transfer successful");
        Console.WriteLine($"You transferred {amount} from your account balance to {receiverName}.");
    }

    private static void ViewTransactionHistory() {
        string name = IsUserExist("Write your username: ", "User does not exist");
        if (!IsValidPassword(name)) {
            return;
        }
        
        Console.WriteLine(users[FindIndexProfile(name)].ReturnTransactionHistory());
    }

    private static void ApplyForLoan() {
        string name = IsUserExist("Write your username: ", "User does not exist");
        if (!IsValidPassword(name)) {
            return;
        }

        Console.WriteLine($"Enter the loan amount (max {MAX_LOAN_AMOUNT} leva): ");
        double amount = double.Parse(Console.ReadLine());

        if (amount < 0 || amount > MAX_LOAN_AMOUNT) {
            Console.WriteLine("Amount is invalid");
            return;
        }
        
        users[FindIndexProfile(name)].ChangeBalance(amount, '+');
        users[FindIndexProfile(name)].ChangeLoans(amount * (1 + INTEREST_RATE), '+');
        
        Console.WriteLine($"Loan of {amount:F2} leva approved for {name}. New balance: {users[FindIndexProfile(name)].GetBalances():F2} leva.");
    }

    private static void RepayLoan() {
        string name = IsUserExist("Write your username: ", "User does not exist");
        if (!IsValidPassword(name)) {
            return;
        }
        
        Console.WriteLine($"Enter repayment amount (Outstanding loan: {users[FindIndexProfile(name)].GetLoans():F2} leva): ");
        double amount = double.Parse(Console.ReadLine());
        if (amount < 0 || amount > MAX_LOAN_AMOUNT) {
            Console.WriteLine("Amount is invalid");
            return;
        }
        
        users[FindIndexProfile(name)].ChangeBalance(amount, '-');
        users[FindIndexProfile(name)].ChangeLoans(amount, '-'); 
    }

    private static void EndAccount() {
        string name = IsUserExist("Write your username: ", "User does not exist");
        if (!IsValidPassword(name)) {
            return;
        }

        if (users[FindIndexProfile(name)].GetBalances() > 0) {
            Console.WriteLine($"You have money in your account. Please withdraw them before closing the account.");
            return;
        }

        if (users[FindIndexProfile(name)].GetLoans() > 0) {
            Console.WriteLine("You have outstanding loans on this account. Please repay them before you can close the account.");
            return;
        }

        Console.WriteLine("Are you sure you want to end the account?(Y/N)");
        string answer = Console.ReadLine().ToUpper();
        if (answer == "Y") {
            users.Remove(users[FindIndexProfile(name)]);
        }
        Console.WriteLine("You have ended the account successfully");
    }

    private static string IsUserExist(string message, string errorMessage) {
        Console.Write(message);
        string name = Console.ReadLine();
        
        if (!DoesUserExist(name)) {
            Console.WriteLine(errorMessage);
            return IsUserExist(message, errorMessage);
        }

        return name;
    }

    private static string IsUserNotExist(string message, string errorMessage) {
        Console.Write(message);
        string name = Console.ReadLine();

        if (DoesUserExist(name)) {
            Console.WriteLine(errorMessage);
            return IsUserNotExist(message, errorMessage);
        }

        return name;
    }

    private static bool DoesUserExist(string name) {
        return users.Any(user => user.UserName == name);
    }

    private static bool IsValidPassword(string name) {
        Console.WriteLine("Write your password: ");
        string password = Console.ReadLine();

        if (users[FindIndexProfile(name)].Password != password) {
            Console.WriteLine("Password is incorrect.");
            return false;
        }
        
        return true;
    }

    private static int FindIndexProfile(string name) {
        return users.FindIndex(user => user.UserName == name);
    }
}

public class User {
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public double Balance { get; private set; }
    public List<string> TransactionHistory { get; private set; }
    public double LoanAmount { get; private set; }

    public User(string userName, string password, double balance, List<string> transactionHistory, double loanAmount) {
        UserName = userName;
        Password = password;
        Balance = balance;
        TransactionHistory = transactionHistory;
        LoanAmount = loanAmount;
    }

    public void ChangePassword(string newPassword) {
        Password = newPassword;
    }

    public void ChangeBalance(double amount, char operation) {
        if (operation == '+') Balance += amount;
        if (operation == '-') Balance -= amount;
    }

    public void AddTransaction(string transaction) {
        TransactionHistory.Add(transaction);
    }

    public void ChangeLoans(double amount, char operation) {
        if (operation == '+') LoanAmount += amount;
        if (operation == '-') LoanAmount -= amount;
    }

    public double GetBalances() {
        return Balance;
    }

    public double GetLoans() {
        return LoanAmount;
    }

    public string ReturnTransactionHistory() {
        return string.Join("\n", TransactionHistory);
    }
}
