using System;
using Cecs475.BoardGames.Model;
using FluentAssertions;
using System.Linq;
using System.Collections.Generic;
using Cecs475.BoardGames.View;

namespace Cecs475.BoardGames.Test {
	public abstract class BoardGameTest<TBoard, TMove, TView>
		where TBoard : IGameBoard, new()
		where TMove : IGameMove
		where TView : IConsoleView, new() {

		protected TView ConsoleView{ get; } = new TView();
		
		protected BoardPosition Pos(int row, int col) {
			return new BoardPosition(row, col);
		}

		protected GameAdvantage Advantage(int player, int advantage) {
			return new GameAdvantage(player, advantage);
		}

		private void ApplyToBoard(TBoard board, TMove move) {
			var currentAdvantage = board.CurrentAdvantage;
			var possMoves = board.GetPossibleMoves();
			board.CurrentAdvantage.Should().Be(currentAdvantage,
				"the board's value should not change after calling GetPossibleMoves");

			var toApply = possMoves.SingleOrDefault(move.Equals);
			if (toApply == null) {
				throw new InvalidOperationException($"Could not apply the move {ConsoleView.MoveToString(move)}"
					+ $" to the board\n{ConsoleView.BoardToString(board)}");
			}
			board.ApplyMove(toApply);
		}

		protected void Apply(TBoard board, params TMove[] moves) {
			foreach (var move in moves) {
				ApplyToBoard(board, move);
			}
		}

		protected TBoard CreateBoardFromMoves(params TMove[] moves) {
			TBoard board = new TBoard();
			Apply(board, moves);
			return board;
		}
	}
}
