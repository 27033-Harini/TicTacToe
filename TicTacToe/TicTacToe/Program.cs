using TicTacToe.Repositories;
using TicTacToe.Models;
using TicTacToe.Services;
using TicTacToe.Views;
using TicTacToe.FileHandlers;

class Program
{
    public static void Main(string[] args)
    {
        string dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string completedGameFilePath = Path.Combine(dataDirectory, "completedGames.json");
        string scoreFilePath = Path.Combine(dataDirectory, "scores.json");

        FileHandler<List<CompletedGame>> completedGameHandler = new(completedGameFilePath);
        FileHandler<List<Score>> scoreHandler = new(scoreFilePath);
        GameRepository gameRepository = new();
        HistoryRepository historyRepository = new(completedGameHandler);
        ScoreRepository scoreRepository = new(scoreHandler);
        GameService gameService = new(gameRepository);
        HistoryService historyService = new(historyRepository, scoreRepository);
        ConsoleUI consoleUI = new(gameService, historyService);
        consoleUI.HandleMainMenuChoice();
    }
}
