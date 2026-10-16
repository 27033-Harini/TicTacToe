using TicTacToe.FileHandlers;
using TicTacToe.Models;

namespace TicTacToe.Repositories
{
    public class UserRepository
    {
        private readonly FileHandler<List<User>> _fileHandler;

        public UserRepository(FileHandler<List<User>> fileHandler)
        {
            _fileHandler = fileHandler;
        }

        public List<User> GetAll() => _fileHandler.Load() ?? new List<User>();

        public User? FindByLogin(string login)
        {
            return GetAll().FirstOrDefault(user =>
                string.Equals(user.UserName, login, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(user.Email, login, StringComparison.OrdinalIgnoreCase));
        }

        public bool UserNameExists(string userName) => GetAll().Any(user =>
            string.Equals(user.UserName, userName, StringComparison.OrdinalIgnoreCase));

        public bool EmailExists(string email) => GetAll().Any(user =>
            string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase));

        public string EmailFromUserName(string userName)
        {
            User? user = GetAll().FirstOrDefault(u =>
                string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));
            return user?.Email ?? string.Empty;
        }

        public void Add(User user)
        {
            List<User> users = GetAll();
            users.Add(user);
            _fileHandler.Save(users);
        }
    }
}
