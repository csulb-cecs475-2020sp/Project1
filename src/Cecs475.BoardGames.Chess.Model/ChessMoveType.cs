using System;
using System.Collections.Generic;
using System.Text;

namespace Cecs475.BoardGames.Chess.Model {
	/// <summary>
	/// Identifies the type of move represented by a ChessMove object, in particular highlighting 
	/// "special" moves.
	/// </summary>
	public enum ChessMoveType : byte {
		/// <summary>
		/// Moving one piece using its normal move rules
		/// </summary>
		Normal,
		/// <summary>
		/// Castling to the queen side
		/// </summary>
		CastleQueenSide,
		/// <summary>
		/// Castling to the king side
		/// </summary>
		CastleKingSide,
		/// <summary>
		/// Performing an en passant
		/// </summary>
		EnPassant,
		/// <summary>
		/// Promoting a pawn that is reaching the final rank
		/// </summary>
		PawnPromote
	}
}
