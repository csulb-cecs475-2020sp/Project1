using System;
using System.Collections.Generic;
using System.Text;
using Cecs475.BoardGames.Model;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Model {
	/// <summary>
	/// Represents the board state of a game of chess. Tracks which squares of the 8x8 board are occupied
	/// by which player's pieces.
	/// </summary>
	public class ChessBoard : IGameBoard {
		#region Member fields.
		// The history of moves applied to the board.
		private List<ChessMove> mMoveHistory = new List<ChessMove>();

		public const int BoardSize = 8;

		// TODO: create a field for the board position array. You can hand-initialize
		// the starting entries of the array, or set them in the constructor.





		// TODO: Add a means of tracking miscellaneous board state, like captured pieces and the 50-move rule.




		// TODO: add a field for tracking the current player and the board advantage.		




		#endregion

		#region Properties.
		// TODO: implement these properties.
		// You can choose to use auto properties, computed properties, or normal properties 
		// using a private field to back the property.

		// You can add set bodies if you think that is appropriate, as long as you justify
		// the access level (public, private).

		public bool IsFinished { get { throw new NotImplementedException("You must implement this property."); } }

		public int CurrentPlayer { get { throw new NotImplementedException("You must implement this property."); } }

		public GameAdvantage CurrentAdvantage { get { throw new NotImplementedException("You must implement this property."); } }

		public IReadOnlyList<ChessMove> MoveHistory => mMoveHistory;

		// TODO: implement IsCheck, IsCheckmate, IsStalemate
		public bool IsCheck {
			get { throw new NotImplementedException("You must implement this property."); }
		}

		public bool IsCheckmate {
			get { throw new NotImplementedException("You must implement this property."); }
		}

		public bool IsStalemate {
			get { throw new NotImplementedException("You must implement this property."); }
		}

		public bool IsDraw {
			get { throw new NotImplementedException("You must implement this property."); }
		}
		
		/// <summary>
		/// Tracks the current draw counter, which goes up by 1 for each non-capturing, non-pawn move, and resets to 0
		/// for other moves. If the counter reaches 100 (50 full turns), the game is a draw.
		/// </summary>
		public int DrawCounter {
			get { throw new NotImplementedException("You must implement this property."); }
		}
		#endregion


		#region Public methods.
		public IEnumerable<ChessMove> GetPossibleMoves() {
			throw new NotImplementedException("You must implement this method.");
		}

		public void ApplyMove(ChessMove m) {
			// STRONG RECOMMENDATION: any mutation to the board state should be run
			// through the method SetPieceAtPosition.
			throw new NotImplementedException("You must implement this method.");
		}

		public void UndoLastMove() {
			throw new NotImplementedException("You must implement this method.");
		}
		
		/// <summary>
		/// Returns whatever chess piece is occupying the given position.
		/// </summary>
		public ChessPiece GetPieceAtPosition(BoardPosition position) {
			throw new NotImplementedException("You must implement this method.");
		}

		/// <summary>
		/// Returns whatever player is occupying the given position.
		/// </summary>
		public int GetPlayerAtPosition(BoardPosition pos) {
			// As a hint, you should call GetPieceAtPosition.
			throw new NotImplementedException("You must implement this method.");
		}

		/// <summary>
		/// Returns true if the given position on the board is empty.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEmpty(BoardPosition pos) {
			throw new NotImplementedException("You must implement this method.");
		}

		/// <summary>
		/// Returns true if the given position contains a piece that is the enemy of the given player.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEnemy(BoardPosition pos, int player) {
			throw new NotImplementedException("You must implement this method.");
		}

		/// <summary>
		/// Returns true if the given position is in the bounds of the board.
		/// </summary>
		public static bool PositionInBounds(BoardPosition pos) {
			throw new NotImplementedException("You must implement this method.");
		}

		/// <summary>
		/// Returns all board positions where the given piece can be found.
		/// </summary>
		public IEnumerable<BoardPosition> GetPositionsOfPiece(ChessPieceType piece, int player) {
			throw new NotImplementedException("You must implement this method.");
		}

		/// <summary>
		/// Returns true if the given player's pieces are attacking the given position.
		/// </summary>
		public bool PositionIsAttacked(BoardPosition position, int byPlayer) {
			throw new NotImplementedException("You must implement this method.");
		}

		/// <summary>
		/// Returns a set of all BoardPositions that are attacked by the given player.
		/// </summary>
		public ISet<BoardPosition> GetAttackedPositions(int byPlayer) {
			throw new NotImplementedException("You must implement this method.");
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// Mutates the board state so that the given piece is at the given position.
		/// </summary>
		private void SetPieceAtPosition(BoardPosition position, ChessPiece piece) {
			throw new NotImplementedException("You must implement this method.");
		}

		#endregion

		#region Explicit IGameBoard implementations.
		IEnumerable<IGameMove> IGameBoard.GetPossibleMoves() {
			return GetPossibleMoves();
		}
		void IGameBoard.ApplyMove(IGameMove m) {
			ApplyMove(m as ChessMove);
		}
		IReadOnlyList<IGameMove> IGameBoard.MoveHistory => mMoveHistory;
		#endregion

		// You may or may not need to add code to this constructor.
		public ChessBoard() {

		}

		public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiece>> startingPositions)
			: this() {
			var king1 = startingPositions.Where(t => t.Item2.Player == 1 && t.Item2.PieceType == ChessPieceType.King);
			var king2 = startingPositions.Where(t => t.Item2.Player == 2 && t.Item2.PieceType == ChessPieceType.King);
			if (king1.Count() != 1 || king2.Count() != 1) {
				throw new ArgumentException("A chess board must have a single king for each player");
			}

			foreach (var position in BoardPosition.GetRectangularPositions(8, 8)) {
				SetPieceAtPosition(position, ChessPiece.Empty);
			}

			int[] values = { 0, 0 };
			foreach (var pos in startingPositions) {
				SetPieceAtPosition(pos.Item1, pos.Item2);
				// TODO: you must calculate the overall advantage for this board, in terms of the pieces
				// that the board has started with. "pos.Item2" will give you the chess piece being placed
				// on this particular position.
			}
		}
	}
}
