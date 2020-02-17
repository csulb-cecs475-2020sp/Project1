using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.View {
	/// <summary>
	/// Supports console (text)-based input and output for a particular game. Converts game objects
	/// to strings for output, and parses string inputs to game objects.
	/// </summary>
	public interface IConsoleView {
		/// <summary>
		/// Returns a string suitable for representing the given board on a console output window.
		/// </summary>
		string BoardToString(IGameBoard board);

		/// <summary>
		/// Returns a string suitable for representing the given move on a console output window.
		/// </summary>
		string MoveToString(IGameMove move);

		/// <summary>
		/// Parses input from a command line into a game move object appropriate to this game type.
		/// </summary>
		IGameMove ParseMove(string moveText);

		/// <summary>
		/// Returns a game-appropriate description for the given player number.
		/// </summary>
		string PlayerToString(int player);
	}
}
