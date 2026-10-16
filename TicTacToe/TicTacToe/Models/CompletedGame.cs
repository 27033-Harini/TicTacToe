using TicTacToe.Enums;

namespace TicTacToe.Models
{
    public class CompletedGame
    {
        public CompletedGame(string playerName, DateTime time, List<Move> moves, GameMode mode, string? winner = null, int[]? winningPositions = null)
        {
            this.PlayerName = playerName;
            this.Time = time;
            this.Moves = moves;
            this.Mode = mode;
            this.Winner = winner;
            this.WinningPositions = winningPositions ?? new int[3];
        }
        public string PlayerName { get; set; }
        public string GameId { get; set; } = Guid.NewGuid().ToString("N");
        public string? Winner { get; set; }
        public DateTime Time { get; set; }
        public List<Move> Moves { get; set; }
        public GameMode Mode { get; set; }
        public int[] WinningPositions { get; set; }
    }
}
