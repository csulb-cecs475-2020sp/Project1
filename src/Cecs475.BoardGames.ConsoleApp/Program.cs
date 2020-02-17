using System;
using System.Collections.Generic;
using System.Linq;
using Cecs475.BoardGames.Model;
using Cecs475.BoardGames.Othello.View;
using Cecs475.BoardGames.Othello.Model;
using Cecs475.BoardGames.Chess.View;
using Cecs475.BoardGames.Chess.Model;
using Cecs475.BoardGames.View;

namespace Cecs475.BoardGames.ConsoleApp {
	class Program {
		public static void Main(string[] args) {
			OthelloBoard b = new OthelloBoard();
			b.MoveHistory;


			IGameBoard board = null!;
			IConsoleView view = null!;

			// Use a game name from the command line, or default to chess.
			string gameName = args.Length == 1 ? args[0] : "othello";

			switch (gameName) {
				case "othello":
				default:
					board = new OthelloBoard();
					view = new OthelloConsoleView();
					break;
				case "chess":
					board = new ChessBoard();
					view = new ChessConsoleView();
					break;
			}

			while (!board.IsFinished) {
				Console.WriteLine(view.BoardToString(board));

				Console.WriteLine();
				Console.WriteLine("Possible moves:");
				IEnumerable<IGameMove> possMoves = board.GetPossibleMoves();
				Console.WriteLine(string.Join(", ",
					possMoves.Select(view.MoveToString)));

				Console.WriteLine($"It is {view.PlayerToString(board.CurrentPlayer)}'s turn.");
				Console.WriteLine("Enter a command: ");
				string input = Console.ReadLine();

				if (input.StartsWith("move")) {
					IGameMove toApply = view.ParseMove(input.Substring(5));
					IGameMove foundMove = possMoves.SingleOrDefault(toApply.Equals);
					if (foundMove == null) {
						Console.WriteLine("Sorry, that move is invalid.");
					}
					else {
						board.ApplyMove(foundMove);
					}
				}
				else if (input.StartsWith("undo")) {
					if (!int.TryParse(input.Split(' ')[1], out int undoCount)) {
						undoCount = 1;
					}
					for (int i = 0; i < undoCount && board.MoveHistory.Count > 0; i++) {
						board.UndoLastMove();
					}
				}
				else if (input.StartsWith("history")) {
					Console.WriteLine("Move history:");
					Console.WriteLine(string.Join(Environment.NewLine,
						board.MoveHistory.Reverse().Select(
							m => view.PlayerToString(m.Player) + ":" + view.MoveToString(m))));
				}
				else if (input.StartsWith("advantage")) {
					var adv = board.CurrentAdvantage;
					if (adv.Player == 0) {
						Console.WriteLine("No player has an advantage.");
					}
					else {
						Console.WriteLine($"{view.PlayerToString(adv.Player)} has an advantage of {adv.Advantage}.");
					}
				}

				Console.WriteLine();
				Console.WriteLine();
			}
		}
	}
}