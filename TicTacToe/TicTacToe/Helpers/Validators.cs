using System.Net.Mail;
using System.Text.RegularExpressions;
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

        public static bool IsValidUsername(string? userName)
        {
            return !string.IsNullOrWhiteSpace(userName)
                && userName.Trim().Length <= 30
                && userName.All(c => char.IsLetterOrDigit(c) || c == '_');
        }

        public static bool IsValidEmail(string? email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return false;
                }

                MailAddress mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidLoginIdentifier(string? login)
        {
            return IsValidUsername(login) || IsValidEmail(login);
        }

        public static bool IsValidLoginPassword(string? password)
        {
            return !string.IsNullOrEmpty(password);
        }

        public static bool IsValidPassword(string? password)
        {
            return !string.IsNullOrWhiteSpace(password)
            && password.Length >= 8
            && password.Length <= 30
            && !password.Any(char.IsWhiteSpace)
            && password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(char.IsDigit)
            && password.Any(c => !char.IsLetterOrDigit(c));
        }
    }
}
