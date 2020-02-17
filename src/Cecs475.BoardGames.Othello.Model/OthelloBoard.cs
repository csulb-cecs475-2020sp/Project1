using System;
using System.Collections.Generic;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Othello.Model {
	/// <summary>
	/// Implements the board model for a game of Othello. Tracks which squares of the 8x8 grid are occupied
	/// by which player, as well as state for the current player and move history.
	/// </summary>
	public class OthelloBoard : IGameBoard {
		/// <summary>
		/// Represents a memory of all flips made in one particular direction 
		/// by the application of a move.
		/// </summary>
		private struct FlipSet {
			/// <summary>
			/// The direction that flips were made.
			/// </summary>
			public BoardDirection Direction { get; set; }
			/// <summary>
			/// How many enemy pieces were flipped.
			/// </summary>
			public sbyte Count { get; set; }

			public FlipSet(BoardDirection direction, sbyte count) {
				Direction = direction;
				Count = count;
			}
		}

		#region Member variables
		public const int BOARD_SIZE = 8;
		private int mCurrentPlayer = 1;

		// The board is represented by two 64-bit bitboads. The bits store, in row-major order, the positions
		// of the black pieces and white pieces in two different variables.
		private ulong mBlackPieces = 0b00000000_00000000_00000000_00001000_00010000_00000000_00000000_00000000;
		private ulong mWhitePieces = 0b00000000_00000000_00000000_00010000_00001000_00000000_00000000_00000000;

		// Remembers the history of moves applied to the board, and the flips that resulted from those moves.
		private List<OthelloMove> mMoveHistory = new List<OthelloMove>();
		private List<List<FlipSet>> mFlipSets = new List<List<FlipSet>>();

		private int mAdvantageValue;
		#endregion

		#region Auto properties.
		/// <summary>
		/// How many "pass" moves have been applied in a row.
		/// </summary>
		public int PassCount { get; private set; }

		public GameAdvantage CurrentAdvantage { get; private set; }
		#endregion

		#region Computed properties.
		public IReadOnlyList<OthelloMove> MoveHistory {
			get {
				return mMoveHistory;
			}
		}

		public int CurrentPlayer {
			get {
				return mCurrentPlayer == 1 ? 1 : 2;
			}
		}
		
		public bool IsFinished {
			get {
				return PassCount == 2;
			}
		}
		#endregion

		#region Public methods
		// This is how we will expose the state of the gameboard in a way that reduces coupling.
		// No one needs to know HOW the data is represented; they simply need to know which player is
		// at which position.
		/// <summary>
		/// Returns an integer representing which player has a piece at the given position, or 0 if the position
		/// is empty.
		/// </summary>
		public int GetPlayerAtPosition(BoardPosition boardPosition) {
			// Get the bit position corresponding to this BoardPosition object.
			int index = GetBitIndexForPosition(boardPosition);

			// Create a bitmask with a 1 in the bit position for the calculated index.
			ulong mask = 1UL << index;

			// Check if black has a piece at the given position.
			var black = mask & mBlackPieces;
			if (black != 0) {
				return 1;
			}
			// Check if white has a piece at the given position.
			var white = mask & mWhitePieces;
			if (white != 0) {
				return 2;
			}

			// Otherwise it must be empty.
			return 0;
		}

		public void ApplyMove(OthelloMove? m) {
			if (m != null) {
				List<FlipSet> currentFlips = new List<FlipSet>();
				m.Player = CurrentPlayer;
				// If the move is a pass, then we do very little.
				if (m.IsPass) {
					PassCount++;
				}
				else {
					PassCount = 0;
					// Otherwise update the board at the move's position with the current player.
					SetPlayerAtPosition(m.Position, CurrentPlayer);
					mAdvantageValue += mCurrentPlayer;

					// Iterate through all 8 directions radially from the move's position.
					foreach (BoardDirection dir in BoardDirection.CardinalDirections) {
						// Repeatedly move in the selected direction, as long as we find "enemy" squares.
						BoardPosition newPos = m.Position;
						int steps = 0;
						do {
							newPos = newPos.Translate(dir);
							steps++;
						} while (PositionIsEnemy(newPos, CurrentPlayer));

						// This is a valid direction of flips if we moved at least 2 squares, and ended in bounds and on a
						// "friendly" square.
						if (steps > 1 && GetPlayerAtPosition(newPos) == CurrentPlayer) {
							// Record a FlipSet for this direction
							currentFlips.Add(new FlipSet(dir, (sbyte)(steps - 1)));

							var reverse = -dir;
							// Repeatedly walk back the way we came, updating the board with the current player's piece.
							do {
								newPos = newPos.Translate(reverse);
								SetPlayerAtPosition(newPos, CurrentPlayer);
								mAdvantageValue += 2 * mCurrentPlayer;

								steps--;
							}
							while (steps > 1);
						}
					}
				}

				// Update the rest of the board state.
				mCurrentPlayer = -mCurrentPlayer;
				SetAdvantage();
				mMoveHistory.Add(m);
				mFlipSets.Add(currentFlips);
			}
		}

		/// <summary>
		/// Returns an enumeration of moves that would be valid to apply to the current board state.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<OthelloMove> GetPossibleMoves() {
			var moves = new List<OthelloMove>();

			foreach (BoardPosition position in BoardPosition.GetRectangularPositions(BOARD_SIZE, BOARD_SIZE)) {
				if (!PositionIsEmpty(position)) {
					continue;
				}

				// Iterate through all 8 cardinal directions from the current position.
				foreach (BoardDirection dir in BoardDirection.CardinalDirections) {
					// Repeatedly move in the selected direction, as long as we find "enemy" squares.
					BoardPosition newPos = position;
					int steps = 0;
					do {
						newPos = newPos.Translate(dir);
						steps++;
					} while (PositionIsEnemy(newPos, CurrentPlayer));

					// This is a valid direction of flips if we moved at least 2 squares, and ended in bounds and on a
					// "friendly" square.
					if (steps > 1 && GetPlayerAtPosition(newPos) == CurrentPlayer) {
						moves.Add(new OthelloMove(CurrentPlayer, position));
						break;
					}
				}
				// If the current position is valid, yield a move at the position.
			}

			// If no positions were valid, return a "pass" move.
			if (moves.Count == 0) {
				moves.Add(new OthelloMove(CurrentPlayer, new BoardPosition(-1, -1)));
			}

			return moves;
		}

		/// <summary>
		/// Undoes the last move, restoring the game to its state before the move was applied.
		/// </summary>
		public void UndoLastMove() {
			OthelloMove m = mMoveHistory.Last();

			// Note: there is a bug in this code.
			if (!m.IsPass) {
				// Reset the board at the move's position.
				SetPlayerAtPosition(m.Position, 0);

				// Iterate through the move's recorded flipsets.
				foreach (var flipSet in mFlipSets.Last()) {
					BoardPosition pos = m.Position;
					// For each flipset, walk along the flipset's direction resetting pieces.
					for (int i = 0; i < flipSet.Count; i++) {
						pos = pos.Translate(flipSet.Direction);
						// At this moment, CurrentPlayer is actually the enemy of the move that
						// we are undoing, whose pieces we must restore.
						SetPlayerAtPosition(pos, CurrentPlayer);
					}
				}

				// Check to see if the second-to-last move was a pass; if so, set PassCount.
				if (mMoveHistory.Count > 1 && mMoveHistory[mMoveHistory.Count - 2].IsPass) {
					PassCount = 1;
				}
			}
			else {
				PassCount--;
			}
			// Reset the remaining game state.
			SetAdvantage();
			mCurrentPlayer = -mCurrentPlayer;
			mMoveHistory.RemoveAt(mMoveHistory.Count - 1);
			mFlipSets.RemoveAt(mFlipSets.Count - 1);
		}
		#endregion

		#region Private methods
		private void SetAdvantage() {
			CurrentAdvantage = new GameAdvantage(mAdvantageValue > 0 ? 1 : mAdvantageValue < 0 ? 2 : 0,
				Math.Abs(mAdvantageValue));
		}

		/// <summary>
		/// Returns the bit index corresponding to the given BoardPosition, with the LSB being index 0
		/// and the MSB being index 63.
		/// </summary>
		private static int GetBitIndexForPosition(BoardPosition boardPosition) =>
			63 - (boardPosition.Row * 8 + boardPosition.Col);

		private void SetPlayerAtPosition(BoardPosition position, int player) {
			// Construct a bitmask for the bit position corresponding to the BoardPosition.
			int index = GetBitIndexForPosition(position);
			ulong mask = 1UL << index;

			// To set a particular player at a given position, we must bitwise OR the mask
			// into the player's bitboard, and then remove that mask from the other player's
			// bitboard. 
			if (player == 1) {
				mBlackPieces |= mask;
				
				// ANDing with the NOT of a bitmask wipes that bit from the bitboard.
				mWhitePieces &= ~mask;
			}
			else if (player == 2) {
				mWhitePieces |= mask;
				mBlackPieces &= ~mask;
			}
			else {
				mBlackPieces &= ~mask;
				mWhitePieces &= ~mask;
			}
		}

		/// <summary>
		/// Returns true if the given in-bounds position is an enemy of the given player.
		/// </summary>
		/// <param name="pos">assumed to be in bounds</param>
		private bool PositionIsEnemy(BoardPosition pos, int player) => 
			// [player] 1 + [player] 2 == 3
			GetPlayerAtPosition(pos) + player == 3;

		private bool PositionIsEmpty(BoardPosition position) => 
			GetPlayerAtPosition(position) == 0;
		#endregion

		#region Explicit IGameBoard implementations.
		IEnumerable<IGameMove> IGameBoard.GetPossibleMoves() {
			IEnumerable<OthelloMove> moves = GetPossibleMoves();
			return moves;
		}

		IReadOnlyList<IGameMove> IGameBoard.MoveHistory {
			get {
				return mMoveHistory;
			}
		}

		void IGameBoard.ApplyMove(IGameMove move) {
			OthelloMove? casted = move as OthelloMove;
			if (casted != null) {
				ApplyMove(casted);
			}
			else {
				throw new ArgumentException($"Parameter {nameof(move)} must be of type {nameof(OthelloMove)}");
			}
		}
		#endregion
	}
}