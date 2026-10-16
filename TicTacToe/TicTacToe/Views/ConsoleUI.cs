using Spectre.Console;
using TicTacToe.Enums;
using TicTacToe.Helpers;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Views
{
    public class ConsoleUI
    {
        private readonly GameService _gameService;
        private readonly HistoryService _historyService;
        private readonly UserService _userService;
        private string _userName = string.Empty;
        private string _opponentName = string.Empty;

        public ConsoleUI(GameService gameService, HistoryService historyService, UserService userService)
        {
            _gameService = gameService;
            _historyService = historyService;
            _userService = userService;
        }

        public void HandleMainMenuChoice()
        {
            HandleMenuChoice();
        }

        public void HandleMenuChoice()
        {
            while (true)
            {
                if (!Login()) return;
                bool loggedIn = true;
                while (loggedIn)
                {
                    Console.Clear();
                    WriteScoreTopLeft();
                    AnsiConsole.Write(Align.Center(new FigletText("TIC TAC TOE").Color(Color.Cyan1)));
                    var prompt = new SelectionPrompt<string>()
                        .Title("[bold cyan]MAIN MENU[/]")
                        .HighlightStyle(new Style(Color.Black, Color.Cyan1))
                        .AddChoices("Play New Game", "View History / Replay", "View Total Score", "Logout", "Close Application");
                    switch (AnsiConsole.Prompt(prompt))
                    {
                        case "Play New Game": HandleModeChoice(); break;
                        case "View History / Replay": ViewHistory(); break;
                        case "View Total Score":
                            Console.Clear();
                            AnsiConsole.Write(Align.Center(new Rule("[yellow]YOUR SCORE[/]").RuleStyle("yellow")));
                            Score score = _historyService.GetScoreDetails(_userName);
                            AnsiConsole.MarkupLine($"[bold green]Player: {Markup.Escape(_userName)}[/]");
                            AnsiConsole.MarkupLine($"[bold green]Total wins: {score.ScoreValue}[/]");
                            AnsiConsole.MarkupLine($"[cyan]Games played: {score.TotalGamesPlayed}[/]");
                            Pause();
                            break;
                        case "Logout":
                            AnsiConsole.MarkupLine("[cyan]Logging out...[/]");
                            Thread.Sleep(500);
                            loggedIn = false;
                            break;
                        default:
                            AnsiConsole.MarkupLine("[cyan]Thanks for playing. See you next time![/]");
                            return;
                    }
                }
            }
        }

        public void HandleModeChoice()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(Align.Center(new Rule("[cyan]CHOOSE YOUR GAME MODE[/]")));
                var prompt = new SelectionPrompt<GameMode>()
                    .HighlightStyle(new Style(Color.Black, Color.Cyan1))
                    .PageSize(8)
                    .AddChoices(Enum.GetValues<GameMode>());
                GameMode choice = AnsiConsole.Prompt(prompt);
                if (choice == GameMode.Back) return;
                if (choice == GameMode.Manual) StartManualGame();
                else StartGame(choice);
                return;
            }
        }

        public static void DisplayExitMessage() => AnsiConsole.MarkupLine("[cyan]Exiting the game. Thanks for playing![/]");

        public static void Pause()
        {
            AnsiConsole.MarkupLine("\n[grey]Press any key to continue, or X to exit...[/]");
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.X) Environment.Exit(0);
        }

        private bool Login()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(Align.Center(new FigletText("TIC TAC TOE").Color(Color.Cyan1)));
                AnsiConsole.Write(Align.Center(new Rule("[cyan]WELCOME[/]")));
                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("[bold cyan]ACCOUNT[/]")
                    .HighlightStyle(new Style(Color.Black, Color.Cyan1))
                    .AddChoices("Login", "Sign-up", "Close Application"));

                if (choice == "Close Application") return false;
                if (choice == "Sign-up")
                {
                    SignUp();
                    continue;
                }

                Console.Clear();
                AnsiConsole.Write(Align.Center(new Rule("[cyan]LOGIN[/]")));
                AnsiConsole.MarkupLine("[grey]Enter your username or email address. Press Escape to go back.[/]");
                AnsiConsole.Markup("[cyan]Username or email: [/]");
                string? login = ReadTextInput();
                if (login == null) continue;

                login = login.Trim();
                if (!Validators.IsValidLoginIdentifier(login))
                {
                    AnsiConsole.MarkupLine("[red]Enter a valid username.[/]");
                    Pause();
                    continue;
                }

                AnsiConsole.Markup("[cyan]Password: [/]");
                string? password = ReadPasswordInput();
                if (password == null) continue;
                if (!Validators.IsValidLoginPassword(password))
                {
                    AnsiConsole.MarkupLine("[red]Password cannot be empty.[/]");
                    Pause();
                    continue;
                }

                User? user = _userService.Login(login, password);
                if (user == null)
                {
                    AnsiConsole.MarkupLine("[red]Invalid username/email or password.[/]");
                    Pause();
                    continue;
                }

                _userName = user.UserName;
                return true;
            }
        }

        private void SignUp()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(Align.Center(new Rule("[cyan]CREATE ACCOUNT[/]")));
                AnsiConsole.MarkupLine("[grey]Press Escape at any input to return.[/]");
                AnsiConsole.Markup("[cyan]Username: [/]");
                string? userName = ReadTextInput();
                if (userName == null) return;
                if (!Validators.IsValidUsername(userName))
                {
                    AnsiConsole.MarkupLine("[red]Username is not valid." +
                        " It should be 3-30 characters long and contain only letters, " +
                        "numbers, and underscores.[/]");
                    Pause();
                    continue;
                }
                string? email;
                while (true)
                {
                    AnsiConsole.Markup("[cyan]Email: [/]");
                    email = ReadTextInput();
                    if (email == null) return;
                    if (!Validators.IsValidEmail(email))
                    {
                        AnsiConsole.MarkupLine("[cyan] Email is not valid. [/]");
                        Pause();
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                string? password;
                while (true)
                {
                    AnsiConsole.Markup("[cyan]Password (minimum 8 characters): [/]");
                    password = ReadPasswordInput();
                    if (password == null) return;
                    if (!Validators.IsValidPassword(password))
                    {
                        AnsiConsole.MarkupLine("[cyan]Password must be 8-30 characters long and contain " +
                            "at least:\r\n• One uppercase letter (A-Z)\r\n• One lowercase letter (a-z)" +
                            "\r\n• One number (0-9)\r\n• One special character (e.g., @, #, $, %, &)" +
                            "\r\n• No spaces[/]");
                        Pause();
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                string? confirmPassword;
                while (true)
                {
                    AnsiConsole.Markup("[cyan]Confirm password: [/]");
                    confirmPassword = ReadPasswordInput();
                    if (confirmPassword == null) return;
                    if (password != confirmPassword)
                    {
                        AnsiConsole.MarkupLine("[red]Passwords do not match.[/]");
                        Pause();
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                var result = _userService.SignUp(userName, email, password);
                AnsiConsole.MarkupLine(result.Success
                    ? $"[green]{Markup.Escape(result.Message)}[/]"
                    : $"[red]{Markup.Escape(result.Message)}[/]");
                Pause();
                if (result.Success) return;
            }
        }

        private static string? ReadPasswordInput()
        {
            var input = new System.Text.StringBuilder();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape) return null;
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input.ToString();
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input.Length--;
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
        }

        private bool AskForOpponentName()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(Align.Center(new FigletText("TIC TAC TOE").Color(Color.Cyan1)));
                AnsiConsole.Write(Align.Center(new Rule("[cyan]WELCOME[/]")));
                AnsiConsole.Write(Align.Right(new Markup("[grey]Press Escape to close application.[/]")));
                AnsiConsole.MarkupLine("[white]Enter Player 2's name[/]");
                AnsiConsole.Markup("[cyan]Name: [/]");
                string? name = ReadTextInput();
                if (name == null) return false;
                name = name.Trim();
                if (name.Length == 0)
                {
                    AnsiConsole.MarkupLine("[red]Name cannot be empty.[/]");
                    Pause();
                    continue;
                }

                if (name.Length > 30)
                {
                    AnsiConsole.MarkupLine("[red]Name should not exceed the length of 30.[/]");
                    Pause();
                    continue;
                }

                if (string.Equals(name, this._userName, StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine("[red]Player 2's name cannot be the same as Player 1's name.[/]");
                    Pause();
                    continue;
                }

                _opponentName = name;
                return true;
            }
        }

        private static string? ReadTextInput()
        {
            var input = new System.Text.StringBuilder();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape) return null;
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input.ToString();
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input.Length--;
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
            }
        }

        private void WriteScoreTopLeft()
        {
            Score score = _historyService.GetScoreDetails(_userName);
            AnsiConsole.MarkupLine($"[bold yellow]{Markup.Escape(_userName)} | Score: {score.ScoreValue} | Games: {score.TotalGamesPlayed} | Draws: {score.TotalGamesInDraw}[/]");
        }

        public void StartGame(GameMode mode)
        {
            Symbol choice = GetSymbolChoice();
            if (choice == Symbol.Empty) return;

            Game game = new Game(choice, computer: choice == Symbol.X ? Symbol.O : Symbol.X);
            bool playerTurn = true;
            while (true)
            {
                Thread.Sleep(650);
                DisplayBoard(game, playerTurn ? $"Your turn ({game.Player})" : "Computer's turn...");
                if (playerTurn)
                {
                    int move = GetPlayerMove(game, game.Player, this._userName);
                    if (move == -1) return;
                }
                else
                {
                    HandleGenerationOfMoves(game, mode);
                }

                Tuple<bool, Symbol, int[]?> result = _gameService.HasWon(game.Board);
                if (result.Item1)
                {
                    DisplayBoard(game, "GAME OVER", result.Item3);
                    string? winner = DisplayWin(game, result.Item2);
                    _historyService.AddGame(_userName, DateTime.Now, game.Moves, mode, winner, result.Item3);
                    _historyService.RecordCompletedGame(_userName, winner, false);
                    Pause();
                    return;
                }
                if (_gameService.HasDraw(game))
                {
                    DisplayBoard(game, "GAME OVER");
                    AnsiConsole.MarkupLine("[yellow]It's a draw![/]");
                    _historyService.AddGame(_userName, DateTime.Now, game.Moves, mode);
                    _historyService.RecordCompletedGame(_userName, null, true);
                    Pause();
                    return;
                }
                playerTurn = !playerTurn;
            }
        }

        public void StartManualGame()
        {
            Symbol choice = GetSymbolChoice();
            Console.WriteLine("Enter the second player's name: ");
            if (!AskForOpponentName()) return;
            if (choice == Symbol.Empty) return;
            Game game = new Game(choice, opponent: choice == Symbol.X ? Symbol.O : Symbol.X);
            bool player1Turn = true;
            int move = -1;
            while (true)
            {
                AnsiConsole.Write(Align.Right(new Markup("[grey]Press Escape to go back to main manu. Press [[X]] to Quit[/]")));
                Symbol symbol = player1Turn ? game.Player : game.Opponent;
                PlayerName name = player1Turn ? PlayerName.Player : PlayerName.Opponent;
                DisplayBoard(game, $"Player {(player1Turn ? "1" : "2")}'s turn ({symbol})");
                if (player1Turn)
                {
                    move = GetPlayerMove(game, symbol, this._userName);
                }
                else
                {
                    move = GetPlayerMove(game, symbol, this._opponentName);
                }
                if (move == -1) return;

                Tuple<bool, Symbol, int[]?> result = _gameService.HasWon(game.Board);
                if (result.Item1)
                {
                    DisplayBoard(game, "GAME OVER", result.Item3);
                    string? winner = DisplayManualWin(game, result.Item2);
                    _historyService.AddGame(_userName, DateTime.Now, game.Moves, GameMode.Manual, winner, result.Item3);
                    _historyService.RecordCompletedGame(_userName, winner, false);
                    Pause();
                    return;
                }
                if (_gameService.HasDraw(game))
                {
                    DisplayBoard(game, "GAME OVER");
                    AnsiConsole.MarkupLine("[yellow]It's a draw![/]");
                    _historyService.AddGame(_userName, DateTime.Now, game.Moves, GameMode.Manual);
                    _historyService.RecordCompletedGame(_userName, null, true);
                    Pause();
                    return;
                }
                player1Turn = !player1Turn;
            }
        }

        public void ViewHistory()
        {
            List<CompletedGame> games = _historyService.GetAll(this._userName);
            Console.Clear();
            AnsiConsole.Write(Align.Center(new Rule("[cyan]GAME HISTORY[/]")));
            if (games.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No completed games yet. Play a game to see it here![/]");
                Pause();
                return;
            }

            var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Cyan1);
            table.AddColumn("Completed");
            table.AddColumn("Result");
            table.AddColumn("Mode");
            table.AddColumn("Moves");
            table.AddColumn("Final board");
            foreach (CompletedGame game in games.OrderByDescending(g => g.Time))
            {
                table.AddRow(
                    game.Time.ToString("dd MMM yyyy HH:mm"),
                    game.Winner?.ToString() ?? "Draw",
                    game.Mode.ToString(),
                    game.Moves.Count.ToString(),
                    FormatBoard(game.Moves.LastOrDefault()?.BoardAfterMove));
            }
            AnsiConsole.Write(Align.Center(table));

            string action = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("\n[cyan]What would you like to do?[/]")
                .HighlightStyle(new Style(Color.Black, Color.Cyan1))
                .AddChoices("Replay a game", "Back to main menu"));
            if (action == "Replay a game")
            {
                int index = 1;
                var choices = games.OrderByDescending(g => g.Time)
                    .Select(g => $"{index++}: {g.Time:dd MMM HH:mm} | {g.Winner?.ToString() ?? "Draw"}")
                    .ToList();
                choices.Insert(0, "Cancel");
                string selected = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("[cyan]Choose the game to replay[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Black, Color.Cyan1))
                    .AddChoices(choices));
                if (selected != "Cancel")
                {
                    string id = selected.Split(':')[0].Trim();
                    if (int.TryParse(id, out int choiceIndex))
                    {
                        CompletedGame? game = games.OrderByDescending(g => g.Time).ElementAtOrDefault(choiceIndex - 1);
                        if (game != null) ReplayGame(game);
                    }
                }
            }
        }

        private void ReplayGame(CompletedGame game)
        {
            Console.Clear();
            AnsiConsole.Write(Align.Center(new Rule($"[cyan]REPLAY — GAME {game.GameId[..Math.Min(8, game.GameId.Length)]}[/]")));
            AnsiConsole.MarkupLine("[grey]Replaying saved moves. Press any key to skip the animation.[/]");
            bool skip = false;
            foreach (Move move in game.Moves)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey pressed = Console.ReadKey(true).Key;
                    if (pressed == ConsoleKey.X) Environment.Exit(0);
                    if (pressed == ConsoleKey.Escape) return;
                    skip = true;
                }
                Console.Clear();
                AnsiConsole.Write(Align.Center(new Rule($"[cyan]REPLAY — MOVE {game.Moves.IndexOf(move) + 1}/{game.Moves.Count}[/]")));
                DisplaySnapshot(move.BoardAfterMove, $"{move.Name} played position {move.Position + 1}");
                if (!skip) Thread.Sleep(1500);
            }
            Console.Clear();
            AnsiConsole.Write(Align.Center(new Rule("[green]REPLAY COMPLETE[/]")));
            DisplaySnapshot(game.Moves.LastOrDefault()?.BoardAfterMove ?? new Symbol[9], $"Result: {game.Winner?.ToString() ?? "Draw"}");
            Pause();
        }

        private static string FormatBoard(Symbol[]? board)
        {
            if (board == null || board.Length != 9) return "—";
            string Cell(int i) => board[i] == Symbol.Empty ? " " : board[i].ToString();
            return $"{Cell(0)} {Cell(1)} {Cell(2)} \n{Cell(3)} {Cell(4)} {Cell(5)} \n{Cell(6)} {Cell(7)} {Cell(8)}\n";
        }

        private void DisplaySnapshot(Symbol[] board, string subtitle, int[]? winningPositions = null)
        {
            AnsiConsole.MarkupLine($"[grey]{Markup.Escape(subtitle)}[/]");
            var table = new Table().HideHeaders().Border(TableBorder.Heavy).BorderColor(Color.Cyan1).ShowRowSeparators();
            // Define the three columns before adding the three cells in each board row.
            table.AddColumn(" ");
            table.AddColumn(" ");
            table.AddColumn(" ");
            // AnsiConsole.Write(Align.Center(table));
            for (int i = 0; i < 3; i++)
            {
                var row = new List<string>();
                for (int j = 0; j < 3; j++)
                {
                    int position = i * 3 + j;
                    Symbol symbol = board[position];
                    string cell = symbol == Symbol.Empty ? " " : symbol == Symbol.X ? "[bold dodgerblue1] X [/]" : "[bold hotpink] O [/]";
                    if (winningPositions != null && winningPositions.Contains(position))
                    {
                        cell = symbol == Symbol.X ? "[white on bold dodgerblue1] X [/]" : "[white on bold hotpink] O [/]";
                    }
                    row.Add(cell);
                }
                table.AddRow(row.Select(x => new Markup(x)));
            }
            AnsiConsole.Write(Align.Center(table));
        }

        public Symbol GetSymbolChoice()
        {
            string choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"Choose your symbol, {this._userName}")
                .HighlightStyle(new Style(Color.Black, Color.Cyan1))
                .AddChoices("X", "O", "Back"));
            return choice == "X" ? Symbol.X : choice == "O" ? Symbol.O : Symbol.Empty;
        }

        public string DisplayWin(Game game, Symbol symbol)
        {
            bool won = symbol == game.Player;
            AnsiConsole.MarkupLine(won ? "[bold green]You won! Great game![/]" : "[bold red]The computer won this round.[/]");
            return won ? this._userName : "Computer";
        }

        public string DisplayManualWin(Game game, Symbol symbol)
        {
            bool won = symbol == game.Player;
            AnsiConsole.MarkupLine(won ? $"[bold green]{this._userName} wins![/]" : $"[bold hotpink]{this._opponentName} wins![/]");
            return won ? this._userName : this._opponentName;
        }

        public static void DisplayChoice(Symbol symbol) =>
            AnsiConsole.MarkupLine($"[cyan]You are {symbol}; computer is {(symbol == Symbol.X ? Symbol.O : Symbol.X)}.[/]");

        public static void DisplayManualChoice(Symbol symbol) =>
            AnsiConsole.MarkupLine($"[cyan]Player 1 is {symbol}; Player 2 is {(symbol == Symbol.X ? Symbol.O : Symbol.X)}.[/]");

        public void DisplayBoard(Game game, string subtitle = "", int[]? winningPositions = null)
        {
            Console.Clear();
            AnsiConsole.Write(Align.Right(new Markup("[grey]Press Escape to go back to main manu. Press [[X]] to Quit[/]")));
            WriteScoreTopLeft();
            AnsiConsole.Write(Align.Center(new FigletText("TIC TAC TOE").Color(Color.Cyan1)));
            if (!string.IsNullOrWhiteSpace(subtitle)) AnsiConsole.MarkupLine($"[bold yellow]{Markup.Escape(subtitle)}[/]");
            DisplaySnapshot(game.Board, "Use Arrows to select a cell, Enter to place your mark, Esc to quit this game.", winningPositions);
        }

        public string GetCell(Game game, int index)
        {
            Symbol value = _gameService.GetByPosition(game, index);
            return value == Symbol.Empty ? (index + 1).ToString() : value.ToString();
        }

        public int GetPlayerMove(Game game, Symbol symbol, string name)
        {
            int selected = 4;
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(Align.Center(new FigletText("TIC TAC TOE").Color(Color.Cyan1)));
                AnsiConsole.MarkupLine($"[bold yellow]{Markup.Escape($"{name} ({symbol}) — choose a square")}[/]");
                // Draw a numbered board with the current cursor highlighted.
                var table = new Table().Border(TableBorder.Heavy).BorderColor(Color.Cyan1).HideHeaders().ShowRowSeparators();
                // Each board row contains three cells, so declare three columns first.
                table.AddColumn(new TableColumn(" "));
                table.AddColumn(new TableColumn(" "));
                table.AddColumn(new TableColumn(" "));
                for (int row = 0; row < 3; row++)
                {
                    var cells = new List<string>();
                    for (int col = 0; col < 3; col++)
                    {
                        int i = row * 3 + col;
                        Symbol value = game.Board[i];
                        string text = value == Symbol.Empty ? " " : value.ToString();
                        if (i == selected) text = $"[black on yellow] {text} [/]";
                        else if (value == Symbol.X) text = $"[bold dodgerblue1] {text} [/]";
                        else if (value == Symbol.O) text = $"[bold hotpink] {text} [/]";
                        else text = $"[grey] {text} [/]";
                        cells.Add(text);
                    }
                    table.AddRow(cells.Select(c => new Markup(c)));
                }
                AnsiConsole.Write(Align.Center(table));
                AnsiConsole.MarkupLine("[grey]Arrow keys: move   Enter: place mark   Esc: cancel[/]");

                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.X) Environment.Exit(0);
                if (key == ConsoleKey.Escape) return -1;
                if (key == ConsoleKey.LeftArrow && selected % 3 > 0) selected--;
                else if (key == ConsoleKey.RightArrow && selected % 3 < 2) selected++;
                else if (key == ConsoleKey.UpArrow && selected >= 3) selected -= 3;
                else if (key == ConsoleKey.DownArrow && selected < 6) selected += 3;
                else if (key == ConsoleKey.Enter)
                {
                    if (game.Board[selected] != Symbol.Empty)
                    {
                        AnsiConsole.MarkupLine("[red]That square is already taken. Choose an empty one.[/]");
                        Thread.Sleep(700);
                        continue;
                    }
                    _gameService.UpdateBoard(game, selected, symbol, name);
                    return selected;
                }
            }
        }

        public int HandleGenerationOfMoves(Game game, GameMode mode) => mode switch
        {
            GameMode.Easy => _gameService.GenerateEasyMove(game),
            GameMode.Medium => _gameService.GenerateMediumMove(game),
            GameMode.ImpossibleToWin => _gameService.GenerateImpossibleToWinMove(game),
            _ => -1
        };
    }
}
