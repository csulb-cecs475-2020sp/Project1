using System;
using System.Collections.Generic;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Othello.Model {
	/// <summary>
	/// Represents a single move that can be or has been applied to an OthelloBoard object.
	/// </summary>
	public class OthelloMove : IGameMove, IEquatable<OthelloMove> {
		public int Player { get; set; }

		/// <summary>
		/// The position of the move.
		/// </summary>
		public BoardPosition Position { get; }

		/// <summary>
		/// Initializes a new OthelloMove instance representing the given board position.
		/// </summary>
		public OthelloMove(BoardPosition pos) {
			Position = pos;
		}

		public OthelloMove(int player, BoardPosition pos) {
			Player = player;
			Position = pos;
		}

		public override bool Equals(object obj) {
			OthelloMove? casted = obj as OthelloMove;
			return casted != null && Equals(casted);
		}

		/// <summary>
		/// Returns true if the two objects have the same position.
		/// </summary>
		public bool Equals(IGameMove obj) {
			OthelloMove? casted = obj as OthelloMove;
			return casted != null && Equals(casted);
		}

		public bool Equals(OthelloMove other) {
			return Position.Equals(other.Position);
		}


		public override int GetHashCode() =>
			Position.GetHashCode();

		/// <summary>
		/// True if the move represents a "pass".
		/// </summary>
		public bool IsPass =>
			Position.Row == -1 && Position.Col == -1;


		// For debugging.
		public override string ToString() {
			return Position.ToString();
		}
	}
}
