using System;
using System.Collections.Generic;
using System.IO;
using HabitTracker;

namespace HabitTracker.Tests;

public class FileSaverTests
{
    private readonly string _testDirectory;
    private readonly string _originalDirectory;
    private readonly FileSaver _fileSaver;

    public FileSaverTests()
    {
        _originalDirectory = Directory.GetCurrentDirectory();
        _testDirectory = Path.Combine(Path.GetTempPath(), "FileSaverTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testDirectory);
        Directory.SetCurrentDirectory(_testDirectory);

        _fileSaver = new FileSaver();
    }

    [Fact]
    public void EnsureFileExists_Test()
    {
        string path = "testfile.txt";

        _fileSaver.EnsureFileExists(path);

        Assert.True(File.Exists(path));
    }

    [Fact]
    public void WriteAndReadLines_Test()
    {
        string path = "testlines.txt";
        List<string> lines = new() { "first line", "second line" };

        _fileSaver.WriteLines(path, lines);
        List<string> result = _fileSaver.ReadLines(path);

        Assert.Equal(2, result.Count);
        Assert.Equal("first line", result[0]);
        Assert.Equal("second line", result[1]);
    }
}