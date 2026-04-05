using System;
using System.IO;
using HabitTracker;

namespace HabitTracker.Tests;

public class AuthManagerTests
{
    private readonly string _testDirectory;
    private readonly string _originalDirectory;
    private readonly FileSaver _fileSaver;
    private readonly DataManager _dataManager;
    private readonly AuthManager _authManager;

    public AuthManagerTests()
    {
        _originalDirectory = Directory.GetCurrentDirectory();
        _testDirectory = Path.Combine(Path.GetTempPath(), "AuthManagerTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testDirectory);
        Directory.SetCurrentDirectory(_testDirectory);

        _fileSaver = new FileSaver();
        _dataManager = new DataManager(_fileSaver);
        _authManager = new AuthManager(_dataManager);
    }

    [Fact]
    public void Register_Test()
    {
        bool result = _authManager.Register("registeruser", "pass123", "blue", out string message);

        Assert.True(result);
        Assert.Equal("Account created.", message);
    }

    [Fact]
    public void Login_Test()
    {
        bool registerResult = _authManager.Register("loginuser", "pass123", "green", out _);
        Assert.True(registerResult);

        bool loginResult = _authManager.Login("loginuser", "pass123", out string message);

        Assert.True(loginResult);
        Assert.Equal("Login successful.", message);
        Assert.NotNull(_authManager.CurrentUser);
        Assert.Equal("loginuser", _authManager.CurrentUser!.Username);
    }
}