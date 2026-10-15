using TicTacToe.FileHandlers;
using TicTacToe.Models;

namespace TicTacToe.Repositories
{
    public class ScoreRepository
    {
        private readonly FileHandler<List<Score>> _fileHandler;

        public ScoreRepository(FileHandler<List<Score>> fileHandler)
        {
            _fileHandler = fileHandler;
        }

        public Score Get(string userName)
        {
            List<Score> scores = _fileHandler.Load() ?? new List<Score>();
            return scores.FirstOrDefault(score =>
                string.Equals(score.UserName, userName, StringComparison.OrdinalIgnoreCase))
                ?? new Score(userName);
        }

        public void RecordGame(string userName, bool won)
        {
            List<Score> scores = _fileHandler.Load() ?? new List<Score>();
            Score? score = scores.FirstOrDefault(item =>
                string.Equals(item.UserName, userName, StringComparison.OrdinalIgnoreCase));

            if (score == null)
            {
                score = new Score(userName);
                scores.Add(score);
            }

            score.TotalGamesPlayed++;
            if (won) score.ScoreValue++;
            _fileHandler.Save(scores);
        }
    }
}
