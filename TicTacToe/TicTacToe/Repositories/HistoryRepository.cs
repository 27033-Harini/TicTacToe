using TicTacToe.FileHandlers;
using TicTacToe.Models;

namespace TicTacToe.Repositories
{
    public class HistoryRepository
    {
        private readonly FileHandler<List<CompletedGame>> _fileHandler;
        public HistoryRepository(FileHandler<List<CompletedGame>> fileHandler)
        {
            this._fileHandler = fileHandler;
        }

        public void Add(CompletedGame completedGame)
        {
            List<CompletedGame> history = this._fileHandler.Load() ?? new List<CompletedGame>();
            history.Add(completedGame);
            this._fileHandler.Save(history);
        }

        public List<CompletedGame> GetAll(string userName)
        {
            List<CompletedGame>? completedGames = this._fileHandler.Load()?.Where(g => g.PlayerName == userName).ToList();
            return completedGames ?? new List<CompletedGame>();
        }
    }
}
