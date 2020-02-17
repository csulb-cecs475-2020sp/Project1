using System;
using System.Collections.Generic;
using System.Linq;

namespace Cecs475.BoardGames.Model {
	/// <summary>
	/// Represents a row/column position on a 2D grid board.
	/// </summary>
	public struct BoardPosition : IEquatable<BoardPosition> {
		/// <summary>
		/// The row of the position.
		/// </summary>
		public int Row { get; }
		/// <summary>
		/// The column of the position.
		/// </summary>
		public int Col { get; }

		public BoardPosition(int row, int col) {
			Row = row;
			Col = col;
		}

		/// <summary>
		/// Translates the BoardPosition by the given amount in the row and column directions, returning a new
		/// position.
		/// </summary>
		/// <param name="rDelta">the amount to change the new position's row by</param>
		/// <param name="cDelta">the amount to change the new position's column by</param>
		/// <returns>a new BoardPosition object that has been translated from the source</returns>
		public BoardPosition Translate(int rDelta, int cDelta) =>
			new BoardPosition(Row + rDelta, Col + cDelta);

		/// <summary>
		/// Translates the BoardPosition by the given amount in the row and column directions, returning a new
		/// position.
		/// </summary>
		/// <param name="direction">a BoardDirection object giving the amount to change the new position's row and column by</param>
		/// <returns>a new BoardPosition object that has been translated from the source</returns>
		public BoardPosition Translate(BoardDirection direction) =>
			Translate(direction.RowDelta, direction.ColDelta);

		// An overridden ToString makes debugging easier.
		public override string ToString() =>
			"(" + Row + ", " + Col + ")";

		#region Equality methods and operators
		/// <summary>
		/// Two board positions are equal if they have the same row and column.
		/// </summary>
		public bool Equals(BoardPosition other) =>
			Row == other.Row && Col == other.Col;

		public override bool Equals(object obj) =>
			Equals((BoardPosition) obj);

		public static bool operator ==(BoardPosition left, BoardPosition right) =>
			left.Equals(right);

		public static bool operator !=(BoardPosition left, BoardPosition right) =>
			!left.Equals(right);

		public override int GetHashCode() {
			unchecked {
				return (Row * 397) ^ Col;
			}
		}
		#endregion

		/// <summary>
		/// Returns a sequence of BoardPosition objects representing each square on a given rectangular
		/// game board, in row-major order.
		/// </summary>
		/// <param name="rows">the number of horizontal rows on the board</param>
		/// <param name="cols">the number of vertical columns on the board</param>
		public static IEnumerable<BoardPosition> GetRectangularPositions(int rows, int cols) {
			return Enumerable.Range(0, 8).SelectMany(
				r => Enumerable.Range(0, 8),
				(r, c) => new BoardPosition(r, c));
		}

		public static BoardPosition operator +(BoardPosition lhs, BoardDirection rhs) =>
			lhs.Translate(rhs);
	}
}
