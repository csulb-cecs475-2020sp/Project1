using System;

namespace Cecs475.BoardGames.Model {
	/// <summary>
	/// Represents a move that can be (or has been) applied to a game board.
	/// </summary>
	public interface IGameMove : IEquatable<IGameMove> {
		/// <summary>
		/// The player that applied this move, if the move has been applied to a board. If it has not,
		/// this property is undefined.
		/// </summary>
		int Player { get; }
	}
}
