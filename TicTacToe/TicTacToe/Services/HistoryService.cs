using TicTacToe.Enums;
using TicTacToe.Models;
using TicTacToe.Repositories;

namespace TicTacToe.Services
{
    public class HistoryService
    {
        private readonly HistoryRepository _historyRepository;
        private readonly ScoreRepository _scoreRepository;

        public HistoryService(HistoryRepository historyRepository, ScoreRepository scoreRepository)
        {
            _historyRepository = historyRepository;
            _scoreRepository = scoreRepository;
        }

        public void AddGame(string name, DateTime completionTime, List<Move> moves, GameMode mode, string? winner = null, int[]? winningPositions = null)
        {
            if (winner != null)
            {
                _historyRepository.Add(new CompletedGame(name, completionTime, moves, mode, winner, winningPositions));
            }
            _historyRepository.Add(new CompletedGame(name, completionTime, moves, mode, winner));
        }

        public List<CompletedGame> GetAll(string userName) => _historyRepository.GetAll(userName);

        public void RecordCompletedGame(string userName, string? winner, bool isDraw)
        {
            _scoreRepository.RecordGame(userName, winner == userName, isDraw);
        }

        public Score GetScoreDetails(string userName) => _scoreRepository.Get(userName);
    }
}
