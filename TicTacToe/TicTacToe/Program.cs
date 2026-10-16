using TicTacToe.Repositories;
using TicTacToe.Models;
using TicTacToe.Services;
using TicTacToe.Views;
using TicTacToe.Models;
using TicTacToe.FileHandlers;

class Program
{
    public static void Main(string[] args)
    {
        string dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string completedGameFilePath = Path.Combine(dataDirectory, "completedGames.json");
        string scoreFilePath = Path.Combine(dataDirectory, "scores.json");
        string userFilePath = Path.Combine(dataDirectory, "users.json");

        FileHandler<List<CompletedGame>> completedGameHandler = new(completedGameFilePath);
        FileHandler<List<Score>> scoreHandler = new(scoreFilePath);
        FileHandler<List<User>> userHandler = new(userFilePath);
        GameRepository gameRepository = new();
        HistoryRepository historyRepository = new(completedGameHandler);
        ScoreRepository scoreRepository = new(scoreHandler);
        UserRepository userRepository = new(userHandler);
        GameService gameService = new(gameRepository);
        HistoryService historyService = new(historyRepository, scoreRepository);
        UserService userService = new(userRepository);
        ConsoleUI consoleUI = new(gameService, historyService, userService);
        consoleUI.HandleMenuChoice();
    }
}
