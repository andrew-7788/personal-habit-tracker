using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HabitTracker;

namespace HabitTracker.Tests;

public class HabitManagerTests
{
    private readonly string _testDirectory;
    private readonly string _originalDirectory;
    private readonly FileSaver _fileSaver;
    private readonly DataManager _dataManager;
    private readonly HabitManager _habitManager;

    public HabitManagerTests()
    {
        _originalDirectory = Directory.GetCurrentDirectory();
        _testDirectory = Path.Combine(Path.GetTempPath(), "HabitManagerTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testDirectory);
        Directory.SetCurrentDirectory(_testDirectory);

        _fileSaver = new FileSaver();
        _dataManager = new DataManager(_fileSaver);
        _habitManager = new HabitManager(_dataManager);
    }

    [Fact]
    public void CreateHabit_Test()
    {
        bool result = _habitManager.CreateHabit("david", "Read", "daily", 1, out string message);

        Assert.True(result);
        Assert.Equal("Habit created.", message);

        List<Habit> habits = _habitManager.GetHabitsForUser("david");
        Assert.Contains(habits, h => h.Name == "Read");
    }

    [Fact]
    public void DeleteHabit_Test()
    {
        bool createResult = _habitManager.CreateHabit("david", "DeleteMe", "daily", 1, out _);
        Assert.True(createResult);

        List<Habit> habits = _habitManager.GetHabitsForUser("david");
        Habit? habit = habits.FirstOrDefault(h => h.Name == "DeleteMe");

        Assert.NotNull(habit);

        bool deleteResult = _habitManager.DeleteHabit("david", habit!.HabitId, out string message);

        Assert.True(deleteResult);
        Assert.Equal("Habit deleted.", message);

        List<Habit> updatedHabits = _habitManager.GetHabitsForUser("david");
        Assert.DoesNotContain(updatedHabits, h => h.Name == "DeleteMe");
    }
}