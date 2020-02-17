using System;
using System.Text;
using Cecs475.BoardGames.Chess.Model;
using Cecs475.BoardGames.Model;
using Cecs475.BoardGames.View;

namespace Cecs475.BoardGames.Chess.View {
	/// <summary>
	/// A chess game view for string-based console input and output.
	/// </summary>
	public class ChessConsoleView : IConsoleView {
		private static char[] LABELS = { '.', 'P', 'R', 'N', 'B', 'Q', 'K' };
		
		// Public methods.
		public string BoardToString(ChessBoard board) {
			StringBuilder str = new StringBuilder();

			for (int i = 0; i < ChessBoard.BoardSize; i++) {
				str.Append(8 - i);
				str.Append(" ");
				for (int j = 0; j < ChessBoard.BoardSize; j++) {
					var space = board.GetPieceAtPosition(new BoardPosition(i, j));
					if (space.PieceType == ChessPieceType.Empty)
						str.Append(". ");
					else if (space.Player == 1)
						str.Append($"{LABELS[(int)space.PieceType]} ");
					else
						str.Append($"{char.ToLower(LABELS[(int)space.PieceType])} ");
				}
				str.AppendLine();
			}
			str.AppendLine("  a b c d e f g h");
			return str.ToString();
		}

		/// <summary>
		/// Converts the given ChessMove to a string representation in the form
		/// "(start, end)", where start and end are board positions in algebraic
		/// notation (e.g., "a5").
		/// 
		/// If this move is a pawn promotion move, the selected promotion piece 
		/// must also be in parentheses after the end position, as in 
		/// "(a7, a8, Queen)".
		/// </summary>
		public string MoveToString(ChessMove move) {
			throw new NotImplementedException("You are responsible for implementing this method.");
		}

		public string PlayerToString(int player) {
			return player == 1 ? "White" : "Black";
		}

		/// <summary>
		/// Converts a string representation of a move into a ChessMove object.
		/// Must work with any string representation created by MoveToString.
		/// </summary>
		public ChessMove ParseMove(string moveText) {
			throw new NotImplementedException("You are responsible for implementing this method.");
		}

		public static BoardPosition ParsePosition(string pos) {
			return new BoardPosition(8 - (pos[1] - '0'), pos[0] - 'a');
		}

		public static string PositionToString(BoardPosition pos) {
			return $"{(char)(pos.Col + 'a')}{8 - pos.Row}";
		}

		#region Explicit interface implementations
		// Explicit method implementations. Do not modify these.
		string IConsoleView.BoardToString(IGameBoard board) {
			return BoardToString(board as ChessBoard);
		}

		string IConsoleView.MoveToString(IGameMove move) {
			return MoveToString(move as ChessMove);
		}

		IGameMove IConsoleView.ParseMove(string moveText) {
			return ParseMove(moveText);
		}
		#endregion
	}
}
