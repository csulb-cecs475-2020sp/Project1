using System;
using System.Collections.Generic;

namespace Cecs475.BoardGames.Model {
	/// <summary>
	/// Represents a direction of movement on a rectangular game board grid.
	/// </summary>
	public struct BoardDirection : IEquatable<BoardDirection> {
		/// <summary>
		/// Negative means "up", positive means "down".
		/// </summary>
		public sbyte RowDelta { get; }
		/// <summary>
		/// Negative means "left", positive means "right".
		/// </summary>
		public sbyte ColDelta { get; }

		public BoardDirection(sbyte rowDelta, sbyte colDelta) {
			RowDelta = rowDelta;
			ColDelta = colDelta;
		}
		
		// An overridden ToString makes debugging easier.
		public override string ToString() =>
			"<" + RowDelta + ", " + ColDelta + ">";


		#region Equality methods and operators.
		/// <summary>
		/// True if the two objects have the same RowDelta and ColDelta.
		/// </summary>
		public bool Equals(BoardDirection other) =>
			RowDelta == other.RowDelta && ColDelta == other.ColDelta;

		public override bool Equals(object obj) =>
			Equals((BoardDirection)obj);

		public static bool operator ==(BoardDirection left, BoardDirection right) =>
			left.Equals(right);

		public static bool operator !=(BoardDirection left, BoardDirection right) =>
			!left.Equals(right);

		public override int GetHashCode() {
			unchecked {
				return (RowDelta.GetHashCode() * 397) ^ ColDelta.GetHashCode();
			}
		}
		#endregion

		/// <summary>
		/// Reverses a BoardDirection so that it points in the opposite direction.
		/// </summary>
		public BoardDirection Reverse() =>
			new BoardDirection((sbyte)-RowDelta, (sbyte)-ColDelta);

		/// <summary>
		/// Reverses a BoardDirection so that it points in the opposite direction.
		/// </summary>
		public static BoardDirection operator -(BoardDirection rhs) =>
			rhs.Reverse();



		/// <summary>
		/// A sequence of 1-square movements in the eight cardinal directions: 
		/// north-west, north, north-east, west, east, south-west, south, south-east.
		/// </summary>
		public static IEnumerable<BoardDirection> CardinalDirections { get; }
			= new BoardDirection[] {
				new BoardDirection(-1, -1),
				new BoardDirection(-1, 0),
				new BoardDirection(-1, 1),
				new BoardDirection(0, -1),
				new BoardDirection(0, 1),
				new BoardDirection(1, -1),
				new BoardDirection(1, 0),
				new BoardDirection(1, 1),
			};
	}
}
