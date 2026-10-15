using TicTacToe.Enums;
using TicTacToe.Models;

namespace TicTacToe.Repositories
{
    public class GameRepository
    {
        public void UpdateBoard(Game game, int position, Symbol symbol) => game.Board[position] = symbol;
        public Symbol[] GetBoard(Game game) => game.Board;
        public Symbol GetByPosition(Game game, int index) => game.Board[index];

        public void AddMove(int position, Game game, string name)
        {
            game.Moves.Add(new Move(position, name)
            {
                BoardAfterMove = (Symbol[])game.Board.Clone()
            });
        }
    }
}
