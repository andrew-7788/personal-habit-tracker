using HabitTracker;

FileSaver fileSaver = new();
DataManager dataManager = new(fileSaver);

AuthManager authManager = new(dataManager);
HabitManager habitManager = new(dataManager);
Reporter reporter = new(dataManager);

ConsoleUI ui = new(authManager, habitManager, reporter);

ui.Run();