using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cecs475.BoardGames.Chess.Model;
using Cecs475.BoardGames.Chess.View;
using Cecs475.BoardGames.Model;
using Cecs475.BoardGames.Test;

namespace Cecs475.BoardGames.Chess.Test {
	public class ChessTest : BoardGameTest<ChessBoard, ChessMove, ChessConsoleView> {

		/// <summary>
		/// Creates a ChessMove using the given start and end positions.
		/// </summary>
		protected ChessMove Move(BoardPosition start, BoardPosition end, ChessMoveType type = ChessMoveType.Normal) =>
			new ChessMove(start, end, type);

		/// <summary>
		/// Creates a ChessMove from a string involving two positions on the board, e.g., "a2, a4".
		/// </summary>
		protected ChessMove Move(string moveString) =>
			ConsoleView.ParseMove(moveString);

		/// <summary>
		/// Creates a BoardPosition from a rank/file string like "a2"
		/// </summary>
		protected BoardPosition Pos(string algebraicPosition) =>
			ChessConsoleView.ParsePosition(algebraicPosition);

		/// <summary>
		/// Creates a new chess board and applies the given list of moves to it immediately.
		/// </summary>
		protected ChessBoard CreateBoardFromMoves(params string[] moveStrings) {
			var board = new ChessBoard();
			Apply(board, moveStrings);
			return board;
		}

		/// <summary>
		/// Creates a new chess board that is empty, except for the given list of pieces
		/// to be manually placed. Three values must be given for each piece: a BoardPosition,
		/// a ChessPieceType, and a player integer.
		/// </summary>
		protected ChessBoard CreateBoardFromPositions(params object[] positions) {
			List<Tuple<BoardPosition, ChessPiece>> pieces = new List<Tuple<BoardPosition, ChessPiece>>();
			for (int i = 0; i < positions.Length; i+= 3) {
				pieces.Add(Tuple.Create((BoardPosition)positions[i],
					new ChessPiece((ChessPieceType)positions[i + 1], (int)positions[i + 2])));
			}
			return new ChessBoard(pieces);
		}

		/// <summary>
		/// Applies one or more moves in sequence to the given board.
		/// </summary>
		protected void Apply(ChessBoard board, params string[] moveStrings) {
			Apply(board, moveStrings.Select(ConsoleView.ParseMove).ToArray());
		}

		/// <summary>
		/// Filters the given list of moves to only those that start at the given position.
		/// </summary>
		protected IEnumerable<ChessMove> GetMovesAtPosition(IEnumerable<ChessMove> moves, BoardPosition start) =>
			moves.Where(m => m.StartPosition == start);



		/// <summary>
		/// Returns all chess piece positions controlled by the given player
		/// </summary>
		protected IEnumerable<ChessPiece> GetAllPiecesForPlayer(ChessBoard b, int player) =>
			BoardPosition.GetRectangularPositions(8, 8)
				.Select(b.GetPieceAtPosition)
				.Where(piece => piece.Player == player);

		/// <summary>
		/// Returns BoardPosition objects representing all squares in the given rank (row).
		/// </summary>
		protected IEnumerable<BoardPosition> GetPositionsInRank(int rank) {
			BoardPosition start = new BoardPosition(8 - rank, 0); // The leftmost position in the given rank.
			return Enumerable.Range(0, 8)
				.Select(c => start.Translate(0, c));
		}

		/// <summary>
		/// Returns BoardPosition objects representing all squares in the given file (column).
		/// </summary>
		protected IEnumerable<BoardPosition> GetPositionsInFile(int file) {
			BoardPosition start = new BoardPosition(0, file); // The topmost position in the given rank.
			return Enumerable.Range(0, 8)
				.Select(r => start.Translate(r, 0));
		}
	}
}
