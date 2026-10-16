namespace TicTacToe.Models
{
    public class Score
    {
        public string UserName { get; set; } = string.Empty;
        public int ScoreValue { get; set; }
        public int TotalGamesInDraw { get; set; }
        public int TotalGamesPlayed { get; set; }

        public Score() { }

        public Score(string userName)
        {
            UserName = userName;
        }
    }
}
