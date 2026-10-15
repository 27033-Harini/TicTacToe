using TicTacToe.Enums;

namespace TicTacToe.Models
{
    public class Move
    {
        public Move(int position, string name)
        {
            Position = position;
            Name = name;
        }

        public int Position { get; set; }
        public string Name { get; set; }
        public Symbol[] BoardAfterMove { get; set; } = new Symbol[9];
    }
}
