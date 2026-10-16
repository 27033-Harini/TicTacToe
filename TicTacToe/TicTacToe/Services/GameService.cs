using TicTacToe.Enums;
using TicTacToe.Models;
using TicTacToe.Repositories;

namespace TicTacToe.Services
{
    public class GameService
    {
        private readonly GameRepository _gameRepository;
        public GameService(GameRepository gameRepository)
        {
            this._gameRepository = gameRepository;
        }

        public void UpdateBoard(Game game, int position, Symbol symbol, string player)
        {
            this._gameRepository.UpdateBoard(game, position, symbol);
            this._gameRepository.AddMove(position, game, player);
        }

        public Symbol[] GetBoard(Game game)
        {
            return this._gameRepository.GetBoard(game);
        }

        public Symbol GetByPosition(Game game, int index)
        {
            return this._gameRepository.GetByPosition(game, index);
        }

        public int GenerateEasyMove(Game game)
        {
            int position;
            bool isBlockingAndChangeRequested = false;
            Symbol[] board = this.GetBoard(game);
            do
            {
                Random random = new Random();
                position = random.Next(0, 9);
                if (Random.Shared.Next(1, 101) > 50)
                {
                    isBlockingAndChangeRequested = this.CheckIfMoveIsBlocking(game, position);
                }
                else
                {
                    isBlockingAndChangeRequested = false;
                }
            }
            while (board[position] != Symbol.Empty || isBlockingAndChangeRequested);

            this.UpdateBoard(game, position, game.Computer, "Computer");
            return position;
        }

        public bool CheckIfMoveIsBlocking(Game game, int position)
        {
            Symbol[]? tempBoard = this.GetBoard(game).Clone() as Symbol[];
            if (tempBoard == null)
            {
                return false;
            }

            tempBoard[position] = game.Player;
            Tuple<bool, Symbol, int[]?> result = HasWon(tempBoard);
            return result.Item1 && result.Item2 == tempBoard[position];
        }

        public int GenerateMediumMove(Game game)
        {
            Symbol[] board = this.GetBoard(game);
            int move = GetWinningMove(game);
            if (move == -1)
            {
                move = GetBlockingMove(game);
            }
            if (move == -1)
            {
                move = GenerateRandomMove(board);
            }

            
            board[move] = game.Computer;
            this.UpdateBoard(game, move, game.Computer, "Computer");
            return move;
        }

        public int GetBlockingMove(Game game)
        {
            Symbol[] board = game.Board;
            Symbol[]? tempBoard = board.Clone() as Symbol[];
            if (tempBoard == null)
            {
                return -1;
            }

            Tuple<bool, Symbol, int[]?> result;
            for (int i = 0; i < tempBoard.Length; i++)
            {
                if (tempBoard[i] != Symbol.Empty)
                {
                    continue;
                }

                tempBoard[i] = game.Player;
                result = this.HasWon(tempBoard);
                if (result.Item1)
                {
                    if (game.Board[i] != Symbol.Empty)
                    {
                        continue;
                    }
                    else
                    {
                        return i;
                    }
                }
                else
                {
                    tempBoard[i] = Symbol.Empty;
                }
            }

            return -1;
        }

        public int GetWinningMove(Game game)
        {
            Symbol[] board = game.Board;
            Symbol[]? tempBoard = board.Clone() as Symbol[];
            if (tempBoard == null)
            {
                return -1;
            }

            Tuple<bool, Symbol, int[]?> result;
            for (int i = 0; i < tempBoard.Length; i++)
            {
                if (tempBoard[i] != Symbol.Empty)
                {
                    continue;
                }

                tempBoard[i] = game.Computer;
                result = this.HasWon(tempBoard);
                if (result.Item1)
                {
                    return i;
                }
                else
                {
                    tempBoard[i] = Symbol.Empty;
                }
            }

            return -1;
        }

        public static int GenerateRandomMove(Symbol[] board)
        {
            int position;
            do
            {
                Random random = new Random();
                position = random.Next(0, 9);
            }
            while (board[position] != Symbol.Empty);

            return position;
        }

        public int GenerateImpossibleToWinMove(Game game)
        {
            int bestScore = int.MinValue;
            int bestMove = -1;
            Symbol[] board = game.Board;
            foreach (int move in GetEmptyCells(game))
            {
                board[move] = game.Computer;

                int score = this.Minimax(game, false);
                board[move] = Symbol.Empty;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            if (Random.Shared.Next(1, 101) <= 15)
            {
                if (Random.Shared.Next(1, 101) <= 15)
                {
                    int blockingMove = GetBlockingMove(game);

                    if (blockingMove >= 0)
                    {
                        bestMove = blockingMove;
                    }
                }
            }

            if (bestMove >= 0)
            {
                this.UpdateBoard(game, bestMove, game.Computer, "Computer");
            }
            return bestMove;
        }

        public int Minimax(Game game, bool isMaximizing)
        {
            Tuple<bool, Symbol, int[]?> result = this.HasWon(game.Board);
            if (result.Item1 && result.Item2 == game.Computer)
            {
                return 10;
            }

            if (result.Item1 && result.Item2 == game.Player)
            {
                return -10;
            }

            if (GetEmptyCells(game).Count == 0)
            {
                return 0;
            }

            if (isMaximizing)
            {
                int bestScore = int.MinValue;

                foreach (int move in GetEmptyCells(game))
                {
                    game.Board[move] = game.Computer;
                    int score = Minimax(game, false);
                    game.Board[move] = Symbol.Empty;
                    bestScore = Math.Max(bestScore, score);
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                foreach (int move in GetEmptyCells(game))
                {
                    game.Board[move] = game.Player;
                    int score = Minimax(game, true);
                    game.Board[move] = Symbol.Empty;
                    bestScore = Math.Min(bestScore, score);
                }

                return bestScore;
            }
        }

        public static List<int> GetEmptyCells(Game game)
        {
            List<int> emptyCells = new List<int>();
            for (int i = 0; i < game.Board.Length; i++)
            {
                if (game.Board[i] == Symbol.Empty)
                {
                    emptyCells.Add(i);
                }
            }
            return emptyCells;
        }


        public Tuple<bool, Symbol, int[]?> HasWon(Symbol[] board)
        {
            int[][] winningLines =
            {
                new int[] {0, 1, 2 },
                new int[] {3, 4, 5 },
                new int[] {6, 7, 8 },
                new int[] {0, 4, 8 },
                new int[] {2, 4, 6 },
                new int[] {0, 3, 6 },
                new int[] {1, 4, 7 },
                new int[] {2, 5, 8 },
            };

            foreach (int[] line in winningLines)
            {
                if (board[line[0]] != Symbol.Empty && board[line[0]] == board[line[1]] && board[line[1]] == board[line[2]])
                {
                    return new Tuple<bool, Symbol, int[]?>(true, board[line[0]], line);
                }
            }

            return new Tuple<bool, Symbol, int[]?>(false, Symbol.Empty, null);
        }

        public bool HasDraw(Game game)
        {
            return !game.Board.Contains(Symbol.Empty);
        }
    }
}
