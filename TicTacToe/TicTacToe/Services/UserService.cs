using System.Security.Cryptography;
using System.Text;
using TicTacToe.Models;
using TicTacToe.Repositories;

namespace TicTacToe.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public (bool Success, string Message) SignUp(string userName, string email, string password)
        {
            userName = userName.Trim();
            email = email.Trim();
            if (_userRepository.EmailFromUserName(userName) == email)
                return (false, "That username is already registered with this email.");
            if (_userRepository.UserNameExists(userName))
                return (false, "That username is already registered. Try another one.");
            if (_userRepository.EmailExists(email))
                return (false, "That email address is already registered.");

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

            _userRepository.Add(new User
            {
                UserName = userName,
                Email = email,
                PasswordSalt = Convert.ToBase64String(salt),
                PasswordHash = Convert.ToBase64String(hash)
            });

            return (true, "Account created successfully. You can now log in.");
        }

        public User? Login(string login, string password)
        {
            User? user = _userRepository.FindByLogin(login.Trim());
            if (user == null) return null;

            try
            {
                byte[] salt = Convert.FromBase64String(user.PasswordSalt);
                byte[] expectedHash = Convert.FromBase64String(user.PasswordHash);
                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

                return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash) ? user : null;
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
