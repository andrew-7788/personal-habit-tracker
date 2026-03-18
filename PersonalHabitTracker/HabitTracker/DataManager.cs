using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HabitTracker;

public class DataManager
{
    private readonly FileSaver _fileSaver;

    private readonly string _usersPath = "users.txt";
    private readonly string _habitsPath = "habits.txt";
    private readonly string _habitEntriesPath = "habitEntries.txt";
    private readonly string _habitSchedulesPath = "habitSchedules.txt";
    private readonly string _remindersPath = "reminders.txt";

    public DataManager(FileSaver fileSaver)
    {
        _fileSaver = fileSaver;
    }

    public List<UserAccount> LoadUsers()
    {
        List<string> lines = _fileSaver.ReadLines(_usersPath);
        List<UserAccount> users = new();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("username|"))
            {
                continue;
            }

            string[] parts = line.Split('|');

            if (parts.Length != 5)
            {
                continue;
            }

            users.Add(new UserAccount
            {
                Username = parts[0],
                PasswordHash = parts[1],
                SecretHash = parts[2],
                FailedLoginAttempts = int.Parse(parts[3]),
                IsLocked = bool.Parse(parts[4])
            });
        }

        return users;
    }

    public void SaveUsers(List<UserAccount> users)
    {
        List<string> lines = new()
        {
            "username|passwordHash|secretHash|failedLoginAttempts|isLocked"
        };

        foreach (UserAccount user in users)
        {
            lines.Add(
                $"{user.Username}|{user.PasswordHash}|{user.SecretHash}|{user.FailedLoginAttempts}|{user.IsLocked}");
        }

        _fileSaver.WriteLines(_usersPath, lines);
    }

    public List<Habit> LoadHabits()
    {
        List<string> lines = _fileSaver.ReadLines(_habitsPath);
        List<Habit> habits = new();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("habitId|"))
            {
                continue;
            }

            string[] parts = line.Split('|');

            if (parts.Length != 5)
            {
                continue;
            }

            habits.Add(new Habit
            {
                HabitId = int.Parse(parts[0]),
                Username = parts[1],
                Name = parts[2],
                IsActive = bool.Parse(parts[3]),
                CreatedOn = DateTime.Parse(parts[4], CultureInfo.InvariantCulture)
            });
        }

        return habits;
    }

    public void SaveHabits(List<Habit> habits)
    {
        List<string> lines = new()
        {
            "habitId|username|name|isActive|createdOn"
        };

        foreach (Habit habit in habits)
        {
            lines.Add(
                $"{habit.HabitId}|{habit.Username}|{habit.Name}|{habit.IsActive}|{habit.CreatedOn:o}");
        }

        _fileSaver.WriteLines(_habitsPath, lines);
    }

    public List<HabitEntry> LoadHabitEntries()
    {
        List<string> lines = _fileSaver.ReadLines(_habitEntriesPath);
        List<HabitEntry> entries = new();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("entryId|"))
            {
                continue;
            }

            string[] parts = line.Split('|');

            if (parts.Length != 4)
            {
                continue;
            }

            entries.Add(new HabitEntry
            {
                EntryId = int.Parse(parts[0]),
                HabitId = int.Parse(parts[1]),
                EntryDate = DateTime.Parse(parts[2], CultureInfo.InvariantCulture),
                Value = int.Parse(parts[3])
            });
        }

        return entries;
    }

    public void SaveHabitEntries(List<HabitEntry> entries)
    {
        List<string> lines = new()
        {
            "entryId|habitId|entryDate|value"
        };

        foreach (HabitEntry entry in entries)
        {
            lines.Add(
                $"{entry.EntryId}|{entry.HabitId}|{entry.EntryDate:o}|{entry.Value}");
        }

        _fileSaver.WriteLines(_habitEntriesPath, lines);
    }

    public List<HabitSchedule> LoadHabitSchedules()
    {
        List<string> lines = _fileSaver.ReadLines(_habitSchedulesPath);
        List<HabitSchedule> schedules = new();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("habitId|"))
            {
                continue;
            }

            string[] parts = line.Split('|');

            if (parts.Length != 3)
            {
                continue;
            }

            schedules.Add(new HabitSchedule
            {
                HabitId = int.Parse(parts[0]),
                FrequencyType = parts[1],
                TargetPerPeriod = int.Parse(parts[2])
            });
        }

        return schedules;
    }

    public void SaveHabitSchedules(List<HabitSchedule> schedules)
    {
        List<string> lines = new()
        {
            "habitId|frequencyType|targetPerPeriod"
        };

        foreach (HabitSchedule schedule in schedules)
        {
            lines.Add(
                $"{schedule.HabitId}|{schedule.FrequencyType}|{schedule.TargetPerPeriod}");
        }

        _fileSaver.WriteLines(_habitSchedulesPath, lines);
    }

    public List<Reminder> LoadReminders()
    {
        List<string> lines = _fileSaver.ReadLines(_remindersPath);
        List<Reminder> reminders = new();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("reminderId|"))
            {
                continue;
            }

            string[] parts = line.Split('|');

            if (parts.Length != 4)
            {
                continue;
            }

            reminders.Add(new Reminder
            {
                ReminderId = int.Parse(parts[0]),
                HabitId = int.Parse(parts[1]),
                TimeOfDay = DateTime.Parse(parts[2], CultureInfo.InvariantCulture),
                IsEnabled = bool.Parse(parts[3])
            });
        }

        return reminders;
    }

    public void SaveReminders(List<Reminder> reminders)
    {
        List<string> lines = new()
        {
            "reminderId|habitId|timeOfDay|isEnabled"
        };

        foreach (Reminder reminder in reminders)
        {
            lines.Add(
                $"{reminder.ReminderId}|{reminder.HabitId}|{reminder.TimeOfDay:o}|{reminder.IsEnabled}");
        }

        _fileSaver.WriteLines(_remindersPath, lines);
    }

    public int GetNextHabitId()
    {
        List<Habit> habits = LoadHabits();

        if (!habits.Any())
        {
            return 1;
        }

        return habits.Max(h => h.HabitId) + 1;
    }

    public int GetNextHabitEntryId()
    {
        List<HabitEntry> entries = LoadHabitEntries();

        if (!entries.Any())
        {
            return 1;
        }

        return entries.Max(e => e.EntryId) + 1;
    }

    public int GetNextReminderId()
    {
        List<Reminder> reminders = LoadReminders();

        if (!reminders.Any())
        {
            return 1;
        }

        return reminders.Max(r => r.ReminderId) + 1;
    }
}
