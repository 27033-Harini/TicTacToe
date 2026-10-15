using TicTacToe.Enums;

namespace TicTacToe.Models
{
    public class Game
    {
        public Game(Symbol player, Symbol computer = Symbol.Empty, Symbol opponent = Symbol.Empty)
        {
            
            this.Player = player;
            this.Computer = computer;
            this.Opponent = opponent;
        }
        public Symbol[] Board { get; set; } = new Symbol[9];
        public Symbol Player { get; set; }
        public Symbol Computer { get; set; }
        public Symbol Opponent { get; set; }
        public List<Move> Moves { get; set; } = new List<Move>();
    }
}
