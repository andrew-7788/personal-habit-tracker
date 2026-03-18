using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HabitTracker;

public class AuthManager
{
    private readonly DataManager _dataManager;

    public UserAccount? CurrentUser { get; private set; }

    public AuthManager(DataManager dataManager)
    {
        _dataManager = dataManager;
    }

    public bool Register(string username, string password, string secretWord, out string message)
    {
        List<UserAccount> users = _dataManager.LoadUsers();

        if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
        {
            message = "Username already exists.";
            return false;
        }

        UserAccount user = new()
        {
            Username = username,
            PasswordHash = Hash(password),
            SecretHash = Hash(secretWord),
            FailedLoginAttempts = 0,
            IsLocked = false
        };

        users.Add(user);
        _dataManager.SaveUsers(users);

        message = "Account created.";
        return true;
    }

    public bool Login(string username, string password, out string message)
    {
        List<UserAccount> users = _dataManager.LoadUsers();

        UserAccount? user = users
            .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            message = "User not found.";
            return false;
        }

        if (user.IsLocked)
        {
            message = "Account is locked.";
            return false;
        }

        if (user.PasswordHash == Hash(password))
        {
            user.FailedLoginAttempts = 0;
            CurrentUser = user;
            _dataManager.SaveUsers(users);

            message = "Login successful.";
            return true;
        }

        user.FailedLoginAttempts++;

        if (user.FailedLoginAttempts >= 5)
        {
            user.IsLocked = true;
            message = "Account locked after too many failed attempts.";
        }
        else
        {
            message = "Invalid password.";
        }

        _dataManager.SaveUsers(users);
        return false;
    }

    public bool ResetPassword(string username, string secretWord, string newPassword, out string message)
    {
        List<UserAccount> users = _dataManager.LoadUsers();

        UserAccount? user = users
            .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            message = "User not found.";
            return false;
        }

        if (user.SecretHash != Hash(secretWord))
        {
            message = "Secret word incorrect.";
            return false;
        }

        user.PasswordHash = Hash(newPassword);
        user.FailedLoginAttempts = 0;
        user.IsLocked = false;

        _dataManager.SaveUsers(users);

        message = "Password reset successful.";
        return true;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    private string Hash(string input)
    {
        using SHA256 sha = SHA256.Create();
        byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

        StringBuilder builder = new();

        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
