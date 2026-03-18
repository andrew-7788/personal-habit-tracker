using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HabitTracker;

public class ConsoleUI
{
    private readonly AuthManager _auth;
    private readonly HabitManager _habitManager;
    private readonly Reporter _reporter;

    public ConsoleUI(AuthManager auth, HabitManager habitManager, Reporter reporter)
    {
        _auth = auth;
        _habitManager = habitManager;
        _reporter = reporter;
    }

    public void Run()
    {
        while (true)
        {
            if (_auth.CurrentUser == null)
            {
                ShowAccountMenu();
            }
            else
            {
                ShowDashboard();
            }
        }
    }

    private void ShowAccountMenu()
    {
        while (_auth.CurrentUser == null)
        {
            Console.Clear();
            PrintHeader("ACCOUNT MENU");

            Console.WriteLine("1) Login");
            Console.WriteLine("2) Register");
            Console.WriteLine("3) Reset Password");
            Console.WriteLine("4) Logout");
            Console.WriteLine("[X] Exit");
            PrintDivider();
            Console.Write("Select option: ");

            string choice = ReadInput().ToUpperInvariant();

            switch (choice)
            {
                case "1":
                    Login();
                    break;

                case "2":
                    Register();
                    break;

                case "3":
                    ResetPassword();
                    break;

                case "4":
                    if (_auth.CurrentUser == null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("No user is currently logged in.");
                        Pause();
                    }
                    else
                    {
                        _auth.Logout();
                    }
                    break;

                case "X":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid option.");
                    Pause();
                    break;
            }
        }
    }

    private void Login()
    {
        Console.Clear();
        PrintHeader("LOGIN");

        Console.Write("Username: ");
        string username = ReadInput();

        Console.Write("Password: ");
        string password = ReadInput();

        bool success = _auth.Login(username, password, out string message);

        Console.WriteLine();
        Console.WriteLine(message);

        if (!success)
        {
            Pause();
        }
    }

    private void Register()
    {
        Console.Clear();
        PrintHeader("REGISTER");

        Console.Write("Username: ");
        string username = ReadInput();

        Console.Write("Password: ");
        string password = ReadInput();

        Console.Write("Secret Word: ");
        string secretWord = ReadInput();

        bool success = _auth.Register(username, password, secretWord, out string message);

        Console.WriteLine();
        Console.WriteLine(message);

        if (success)
        {
            Console.WriteLine("You may now log in.");
        }

        Pause();
    }

    private void ResetPassword()
    {
        Console.Clear();
        PrintHeader("RESET PASSWORD");

        Console.Write("Enter Username: ");
        string username = ReadInput();

        Console.Write("Enter Secret Word: ");
        string secretWord = ReadInput();

        Console.Write("Enter New Password: ");
        string newPassword = ReadInput();

        bool success = _auth.ResetPassword(username, secretWord, newPassword, out string message);

        Console.WriteLine();
        Console.WriteLine(message);

        if (success)
        {
            Console.WriteLine("Please log in with your new password.");
        }

        Pause();
    }

    private void ShowDashboard()
    {
        while (_auth.CurrentUser != null)
        {
            Console.Clear();

            string username = _auth.CurrentUser!.Username;
            List<Reminder> reminders = _habitManager.GetEnabledRemindersForUser(username);
            List<Habit> habits = _habitManager.GetHabitsForUser(username);
            Dictionary<int, Habit> habitMap = habits.ToDictionary(h => h.HabitId, h => h);
            List<Habit> pending = _habitManager.GetPendingHabits(username);

            PrintHeader("MAIN DASHBOARD");
            Console.WriteLine("                      HABIT TRACKER");
            PrintHeaderSpacer();

            Console.WriteLine($"User: {username,-32} Today: {DateTime.Today:yyyy-MM-dd}");
            Console.WriteLine();
            Console.WriteLine("Reminders (next):");

            if (reminders.Count == 0)
            {
                Console.WriteLine("- none");
            }
            else
            {
                foreach (Reminder reminder in reminders.Take(5))
                {
                    string habitName = habitMap.ContainsKey(reminder.HabitId)
                        ? habitMap[reminder.HabitId].Name
                        : $"Habit {reminder.HabitId}";

                    Console.WriteLine($"- {habitName} at {reminder.TimeOfDay:hh:mm tt}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Pending habits today: {pending.Count}");
            Console.WriteLine();
            PrintDivider();
            Console.WriteLine("1) Log Daily Habits");
            Console.WriteLine("2) Manage Habits");
            Console.WriteLine("3) View Progress");
            Console.WriteLine("4) Set Reminders");
            Console.WriteLine("5) Account Menu");
            Console.WriteLine("[X] Exit");
            PrintDivider();
            Console.Write("Select option: ");

            string choice = ReadInput().ToUpperInvariant();

            switch (choice)
            {
                case "1":
                    ShowLogDailyHabitsMenu();
                    break;

                case "2":
                    ShowManageHabitsMenu();
                    break;

                case "3":
                    ShowViewProgressMenu();
                    break;

                case "4":
                    ShowRemindersMenu();
                    break;

                case "5":
                    ShowLoggedInAccountMenu();
                    break;

                case "X":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid option.");
                    Pause();
                    break;
            }
        }
    }

    private void ShowLoggedInAccountMenu()
    {
        while (_auth.CurrentUser != null)
        {
            Console.Clear();
            PrintHeader("ACCOUNT MENU");

            Console.WriteLine("1) Logout");
            Console.WriteLine("[B] Back to Dashboard");
            Console.WriteLine("[X] Exit");
            PrintDivider();
            Console.Write("Select option: ");

            string choice = ReadInput().ToUpperInvariant();

            switch (choice)
            {
                case "1":
                    _auth.Logout();
                    Console.WriteLine();
                    Console.WriteLine("Logged out.");
                    Pause();
                    return;

                case "B":
                    return;

                case "X":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid option.");
                    Pause();
                    break;
            }
        }
    }

    private void ShowManageHabitsMenu()
    {
        while (_auth.CurrentUser != null)
        {
            Console.Clear();
            PrintHeader("MANAGE HABITS");

            Console.WriteLine("1) List Habits");
            Console.WriteLine("2) Create Habit");
            Console.WriteLine("3) Edit Habit");
            Console.WriteLine("4) Delete Habit");
            Console.WriteLine("[B] Back to Dashboard");
            Console.WriteLine("[X] Exit");
            PrintDivider();
            Console.Write("Select option: ");

            string choice = ReadInput().ToUpperInvariant();

            switch (choice)
            {
                case "1":
                    ListHabits();
                    break;

                case "2":
                    CreateHabit();
                    break;

                case "3":
                    EditHabit();
                    break;

                case "4":
                    DeleteHabit();
                    break;

                case "B":
                    return;

                case "X":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid option.");
                    Pause();
                    break;
            }
        }
    }

    private void ListHabits()
    {
        Console.Clear();
        PrintHeader("LIST HABITS");

        string username = _auth.CurrentUser!.Username;
        List<Habit> habits = _habitManager.GetHabitsForUser(username);

        if (habits.Count == 0)
        {
            Console.WriteLine("No habits found.");
            Pause();
            return;
        }

        foreach (Habit habit in habits)
        {
            HabitSchedule? schedule = _habitManager.GetScheduleByHabitId(habit.HabitId);
            int target = schedule?.TargetPerPeriod ?? 0;
            string frequency = schedule?.FrequencyType ?? "n/a";

            Console.WriteLine(
                $"{habit.HabitId}) {habit.Name} | {frequency} | target {target} | {(habit.IsActive ? "Active" : "Inactive")}");
        }

        Pause();
    }

    private void CreateHabit()
    {
        Console.Clear();
        PrintHeader("CREATE HABIT");

        string username = _auth.CurrentUser!.Username;

        Console.Write("Habit Name: ");
        string name = ReadInput();

        Console.Write("Frequency Type (daily): ");
        string frequencyType = ReadInput();
        if (string.IsNullOrWhiteSpace(frequencyType))
        {
            frequencyType = "daily";
        }

        Console.Write("Target Per Period: ");
        if (!int.TryParse(ReadInput(), out int targetPerPeriod))
        {
            Console.WriteLine();
            Console.WriteLine("Invalid target.");
            Pause();
            return;
        }

        bool success = _habitManager.CreateHabit(
            username,
            name,
            frequencyType,
            targetPerPeriod,
            out string message);

        Console.WriteLine();
        Console.WriteLine(message);

        if (success)
        {
            Console.WriteLine("Habit saved.");
        }

        Pause();
    }

    private void EditHabit()
    {
        Console.Clear();
        PrintHeader("EDIT HABIT");

        string username = _auth.CurrentUser!.Username;
        List<Habit> habits = _habitManager.GetHabitsForUser(username);

        if (habits.Count == 0)
        {
            Console.WriteLine("No habits found.");
            Pause();
            return;
        }

        foreach (Habit habit in habits)
        {
            HabitSchedule? schedule = _habitManager.GetScheduleByHabitId(habit.HabitId);
            int target = schedule?.TargetPerPeriod ?? 0;
            string frequency = schedule?.FrequencyType ?? "n/a";

            Console.WriteLine($"{habit.HabitId}) {habit.Name} | {frequency} | target {target}");
        }

        Console.WriteLine();
        Console.Write("Habit ID: ");
        if (!int.TryParse(ReadInput(), out int habitId))
        {
            Console.WriteLine();
            Console.WriteLine("Invalid habit ID.");
            Pause();
            return;
        }

        Habit? existingHabit = _habitManager.GetHabitById(username, habitId);
        HabitSchedule? existingSchedule = _habitManager.GetScheduleByHabitId(habitId);

        if (existingHabit == null || existingSchedule == null)
        {
            Console.WriteLine();
            Console.WriteLine("Habit not found.");
            Pause();
            return;
        }

        Console.Write($"New Habit Name ({existingHabit.Name}): ");
        string newName = ReadInput();
        if (string.IsNullOrWhiteSpace(newName))
        {
            newName = existingHabit.Name;
        }

        Console.Write($"New Frequency Type ({existingSchedule.FrequencyType}): ");
        string newFrequency = ReadInput();
        if (string.IsNullOrWhiteSpace(newFrequency))
        {
            newFrequency = existingSchedule.FrequencyType;
        }

        Console.Write($"New Target Per Period ({existingSchedule.TargetPerPeriod}): ");
        string targetInput = ReadInput();

        int newTarget = existingSchedule.TargetPerPeriod;
        if (!string.IsNullOrWhiteSpace(targetInput))
        {
            if (!int.TryParse(targetInput, out newTarget))
            {
                Console.WriteLine();
                Console.WriteLine("Invalid target.");
                Pause();
                return;
            }
        }

        bool success = _habitManager.EditHabit(
            username,
            habitId,
            newName,
            newFrequency,
            newTarget,
            out string message);

        Console.WriteLine();
        Console.WriteLine(message);
        Pause();
    }

    private void DeleteHabit()
    {
        Console.Clear();
        PrintHeader("DELETE HABIT");

        string username = _auth.CurrentUser!.Username;
        List<Habit> habits = _habitManager.GetHabitsForUser(username);

        if (habits.Count == 0)
        {
            Console.WriteLine("No habits found.");
            Pause();
            return;
        }

        foreach (Habit habit in habits)
        {
            Console.WriteLine($"{habit.HabitId}) {habit.Name}");
        }

        Console.WriteLine();
        Console.Write("Habit ID: ");
        if (!int.TryParse(ReadInput(), out int habitId))
        {
            Console.WriteLine();
            Console.WriteLine("Invalid habit ID.");
            Pause();
            return;
        }

        Console.Write("Confirm delete? (Y/N): ");
        string confirm = ReadInput().ToUpperInvariant();

        if (confirm != "Y")
        {
            Console.WriteLine();
            Console.WriteLine("Delete cancelled.");
            Pause();
            return;
        }

        bool success = _habitManager.DeleteHabit(username, habitId, out string message);

        Console.WriteLine();
        Console.WriteLine(message);

        if (!success)
        {
            Pause();
            return;
        }

        Pause();
    }

    private void ShowLogDailyHabitsMenu()
    {
        while (_auth.CurrentUser != null)
        {
            Console.Clear();
            PrintHeader("LOG DAILY HABITS");

            string username = _auth.CurrentUser!.Username;
            List<Habit> habits = _habitManager.GetActiveHabitsForUser(username);

            Console.WriteLine($"Today: {DateTime.Today:yyyy-MM-dd}");
            Console.WriteLine();
            Console.WriteLine("Pending Habits:");

            if (habits.Count == 0)
            {
                Console.WriteLine("No active habits.");
            }
            else
            {
                foreach (Habit habit in habits)
                {
                    int logged = _habitManager.GetTodayLoggedAmount(habit.HabitId);
                    int target = _habitManager.GetTargetPerPeriod(habit.HabitId);
                    string status = _habitManager.GetStatusText(habit.HabitId);

                    Console.WriteLine($"{habit.HabitId}) {habit.Name,-16} [{logged}/{target}]   Status: {status}");
                }
            }

            PrintDivider();
            Console.WriteLine("[L] Log a Habit Entry");
            Console.WriteLine("[B] Back to Dashboard");
            Console.WriteLine("[X] Exit");
            PrintDivider();
            Console.Write("Select option: ");

            string choice = ReadInput().ToUpperInvariant();

            switch (choice)
            {
                case "L":
                    LogHabitEntry();
                    break;

                case "B":
                    return;

                case "X":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid option.");
                    Pause();
                    break;
            }
        }
    }

    private void LogHabitEntry()
    {
        Console.Clear();
        PrintHeader("LOG A HABIT ENTRY");

        string username = _auth.CurrentUser!.Username;
        List<Habit> habits = _habitManager.GetActiveHabitsForUser(username);

        if (habits.Count == 0)
        {
            Console.WriteLine("No active habits found.");
            Pause();
            return;
        }

        foreach (Habit habit in habits)
        {
            int logged = _habitManager.GetTodayLoggedAmount(habit.HabitId);
            int target = _habitManager.GetTargetPerPeriod(habit.HabitId);
            Console.WriteLine($"{habit.HabitId}) {habit.Name} [{logged}/{target}]");
        }

        Console.WriteLine();
        Console.Write("Habit ID: ");
        if (!int.TryParse(ReadInput(), out int habitId))
        {
            Console.WriteLine();
            Console.WriteLine("Invalid habit ID.");
            Pause();
            return;
        }

        Console.Write("Value: ");
        if (!int.TryParse(ReadInput(), out int value))
        {
            Console.WriteLine();
            Console.WriteLine("Invalid value.");
            Pause();
            return;
        }

        bool success = _habitManager.LogHabitEntry(username, habitId, value, out string message);

        Console.WriteLine();
        Console.WriteLine(message);

        if (success)
        {
            Console.WriteLine("Entry saved.");
        }

        Pause();
    }

    private void ShowViewProgressMenu()
    {
        while (_auth.CurrentUser != null)
        {
            Console.Clear();
            PrintHeader("VIEW PROGRESS");

            Console.WriteLine("1) View Progress Summary");
            Console.WriteLine("2) View Habit Log");
            Console.WriteLine("[B] Back to Dashboard");
            Console.WriteLine("[X] Exit");
            PrintDivider();
            Console.Write("Select option: ");

            string choice = ReadInput().ToUpperInvariant();

            switch (choice)
            {
                case "1":
                    ViewProgressSummary();
                    break;

                case "2":
                    ViewHabitLog();
                    break;

                case "B":
                    return;

                case "X":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid option.");
                    Pause();
                    break;
            }
        }
    }

    private void ViewProgressSummary()
    {
        Console.Clear();
        PrintHeader("VIEW PROGRESS SUMMARY");

        string username = _auth.CurrentUser!.Username;
        Console.WriteLine(_reporter.BuildProgressSummary(username));
        Pause();
    }

    private void ViewHabitLog()
    {
        Console.Clear();
        PrintHeader("VIEW HABIT LOG");

        string username = _auth.CurrentUser!.Username;
        Console.WriteLine(_reporter.BuildHabitLogReport(username));
        Pause();
    }

    private void ShowRemindersMenu()
    {
        while (_auth.CurrentUser != null)
        {
            Console.Clear();
            PrintHeader("REMINDERS");

            Console.WriteLine("1) View Reminders");
            Console.WriteLine("2) Create Reminder");
            Console.WriteLine("3) Enable/Disable Reminder");
            Console.WriteLine("[B] Back to Dashboard");
            Console.WriteLine("[X] Exit");
            PrintDivider();
            Console.Write("Select option: ");

            string choice = ReadInput().ToUpperInvariant();

            switch (choice)
            {
                case "1":
                    ViewReminders();
                    break;

                case "2":
                    CreateReminder();
                    break;

                case "3":
                    ToggleReminder();
                    break;

                case "B":
                    return;

                case "X":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Invalid option.");
                    Pause();
                    break;
            }
        }
    }

    private void ViewReminders()
    {
        Console.Clear();
        PrintHeader("VIEW REMINDERS");

        string username = _auth.CurrentUser!.Username;
        List<Habit> habits = _habitManager.GetHabitsForUser(username);
        List<Reminder> reminders = _habitManager.GetRemindersForUser(username);

        Dictionary<int, Habit> habitMap = habits.ToDictionary(h => h.HabitId, h => h);

        if (reminders.Count == 0)
        {
            Console.WriteLine("No reminders found.");
            Pause();
            return;
        }

        foreach (Reminder reminder in reminders)
        {
            string habitName = habitMap.ContainsKey(reminder.HabitId)
                ? habitMap[reminder.HabitId].Name
                : $"Habit {reminder.HabitId}";

            string status = reminder.IsEnabled ? "Enabled" : "Disabled";
            Console.WriteLine($"{reminder.ReminderId}) {habitName,-16} {reminder.TimeOfDay:hh:mm tt}   {status}");
        }

        Pause();
    }

    private void CreateReminder()
    {
        Console.Clear();
        PrintHeader("CREATE REMINDER");

        string username = _auth.CurrentUser!.Username;
        List<Habit> habits = _habitManager.GetHabitsForUser(username);

        if (habits.Count == 0)
        {
            Console.WriteLine("No habits found.");
            Pause();
            return;
        }

        foreach (Habit habit in habits)
        {
            Console.WriteLine($"{habit.HabitId}) {habit.Name}");
        }

        Console.WriteLine();
        Console.Write("Choose Habit ID: ");
        if (!int.TryParse(ReadInput(), out int habitId))
        {
            Console.WriteLine();
            Console.WriteLine("Invalid habit ID.");
            Pause();
            return;
        }

        Console.Write("Enter Time (HH:mm): ");
        string timeInput = ReadInput();

        if (!DateTime.TryParseExact(
                timeInput,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedTime))
        {
            Console.WriteLine();
            Console.WriteLine("Invalid time format. Use HH:mm.");
            Pause();
            return;
        }

        DateTime reminderTime = DateTime.Today.AddHours(parsedTime.Hour).AddMinutes(parsedTime.Minute);

        bool success = _habitManager.CreateReminder(username, habitId, reminderTime, out string message);

        Console.WriteLine();
        Console.WriteLine(message);

        if (success)
        {
            Console.WriteLine("Reminder saved.");
        }

        Pause();
    }

    private void ToggleReminder()
    {
        Console.Clear();
        PrintHeader("ENABLE / DISABLE REMINDER");

        string username = _auth.CurrentUser!.Username;
        List<Habit> habits = _habitManager.GetHabitsForUser(username);
        List<Reminder> reminders = _habitManager.GetRemindersForUser(username);

        Dictionary<int, Habit> habitMap = habits.ToDictionary(h => h.HabitId, h => h);

        if (reminders.Count == 0)
        {
            Console.WriteLine("No reminders found.");
            Pause();
            return;
        }

        foreach (Reminder reminder in reminders)
        {
            string habitName = habitMap.ContainsKey(reminder.HabitId)
                ? habitMap[reminder.HabitId].Name
                : $"Habit {reminder.HabitId}";

            string status = reminder.IsEnabled ? "Enabled" : "Disabled";
            Console.WriteLine($"{reminder.ReminderId}) {habitName,-16} {reminder.TimeOfDay:hh:mm tt}   {status}");
        }

        Console.WriteLine();
        Console.Write("Reminder ID: ");
        if (!int.TryParse(ReadInput(), out int reminderId))
        {
            Console.WriteLine();
            Console.WriteLine("Invalid reminder ID.");
            Pause();
            return;
        }

        bool success = _habitManager.ToggleReminder(username, reminderId, out string message);

        Console.WriteLine();
        Console.WriteLine(message);

        if (!success)
        {
            Pause();
            return;
        }

        Pause();
    }

    private static void PrintHeader(string title)
    {
        Console.WriteLine("============================================================");
        Console.WriteLine($"{title,34}");
        Console.WriteLine("============================================================");
    }

    private static void PrintHeaderSpacer()
    {
        Console.WriteLine("============================================================");
    }

    private static void PrintDivider()
    {
        Console.WriteLine("------------------------------------------------------------");
    }

    private static string ReadInput()
    {
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}