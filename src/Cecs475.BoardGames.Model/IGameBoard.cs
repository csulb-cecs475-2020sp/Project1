using System;
using System.Collections.Generic;

namespace Cecs475.BoardGames.Model {
	/// <summary>
	/// Represents the board model for a particular board game. Can apply moves, get all possible moves, undo moves,
	/// and report other state information. 
	/// </summary>
	public interface IGameBoard {
		/// <summary>
		/// Gets a sequence of all possible moves for the current game state.
		/// </summary>
		IEnumerable<IGameMove> GetPossibleMoves();

		/// <summary>
		/// Applies a valid move to the current game state.
		/// </summary>
		/// <param name="move">assumed to be a value from the sequence returned by GetPossibleMoves.</param>
		/// <see cref="GetPossibleMoves"/>
		void ApplyMove(IGameMove move);

		/// <summary>
		/// Undoes the most recent move, restoring the game state to the moment when the move was applied.
		/// </summary>
		void UndoLastMove();

		/// <summary>
		/// A list of all moves applied to the game, in the order they were applied.
		/// </summary>
		IReadOnlyList<IGameMove> MoveHistory { get; }

		/// <summary>
		/// The player whose turn it currently is, counting from 1 for the first player.
		/// </summary>
		int CurrentPlayer { get; }

		/// <summary>
		/// True if the game has finished and a winner has been determined.
		/// </summary>
		bool IsFinished { get; }

		/// <summary>
		/// A value indicating which player is winning the game. When IsFinished is true, this value indicates
		/// the winner of the game.
		/// </summary>
		/// <see cref="IsFinished"/>
		GameAdvantage CurrentAdvantage { get; }
	}
}
