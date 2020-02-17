using System;
using System.Collections.Generic;
using System.Text;

namespace Cecs475.BoardGames.Chess.Model {
	/// <summary>
	/// Represents the type of chess piece occupying a square on a chess board.
	/// </summary>
	public enum ChessPieceType : byte {
		/// <summary>
		/// An empty square.
		/// </summary>
		Empty = 0,
		Pawn = 1,
		Rook = 2,
		Knight = 3,
		Bishop = 4,
		Queen = 5,
		King = 6
	}
}
