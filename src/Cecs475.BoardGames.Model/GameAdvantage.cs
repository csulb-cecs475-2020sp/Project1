using System;
using System.Collections.Generic;
using System.Text;

namespace Cecs475.BoardGames.Model {
	/// <summary>
	/// Represents an advantage in a board game, indicating which player is currently "winning", or, 
	/// if the game is finished, which player has won the game.
	/// </summary>
	public struct GameAdvantage : IEquatable<GameAdvantage> {
		/// <summary>
		/// Which player is winning / has won.
		/// </summary>
		public int Player { get; }
		/// <summary>
		/// A game-specific amount indicating "how much" the player is winning / has won by.
		/// </summary>
		public int Advantage { get; }

		public GameAdvantage(int player, int advantage) {
			Player = player;
			Advantage = advantage;
		}

		public bool Equals(GameAdvantage other) =>
			Player == other.Player && Advantage == other.Advantage;

		public override bool Equals(object obj) =>
			Equals((GameAdvantage)obj);

		public override int GetHashCode() {
			unchecked {
				return (Player * 397) ^ Advantage;
			}
		}

		public static bool operator ==(GameAdvantage left, GameAdvantage right) =>
			left.Equals(right);

		public static bool operator !=(GameAdvantage left, GameAdvantage right) =>
			!left.Equals(right);
	
	}
}
