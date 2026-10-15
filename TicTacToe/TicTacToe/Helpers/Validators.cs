using TicTacToe.Enums;

namespace TicTacToe.Helpers
{
    public static class Validators
    {
        public static bool IsValidMoveNumber(string? data)
        {
            return !string.IsNullOrWhiteSpace(data)
                && int.TryParse(data, out int number)
                && number >= 1 && number <= 9;
        }

        public static bool IsPositionAvailable(Symbol[] board, int position)
        {
            return board[position] == Symbol.Empty;
        }
    }
}
