using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class MiscTests : ChessTest {
		/// <summary>
		/// The piece shielding the King cannot be moved
		/// </summary>
		[Fact]
		public void AbsolutePin() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"e7, e5",
				"a4, a5"
			);

			//Move black's Queen to pin pawn
			Apply(b, "d8, h4");

			var possMoves = b.GetPossibleMoves();
			var pinnedExpected = GetMovesAtPosition(possMoves, Pos("f2"));
			pinnedExpected.Should().BeEmpty("the pawn cannot move and leave the King in check");
		}

		/// <summary>
		/// Check queen advantage
		/// </summary>
		[Fact]
		public void QueenAdvantage() {
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",
				"e7, e5",
				"d4, e5",
				"e8, e7",
				"d1, d7",
				"d8, d7"
				);

			b.CurrentAdvantage.Should().Be(Advantage(2, 7), "Black has taken a queen and lost two pawns");
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(1, 2), "White is two pawns up");
		}
		
		/// <summary>
		/// Check bishop advantage
		/// </summary>
		[Fact]
		public void BishopAdvantage() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"b7, b6",
				"f1, e2",
				"c8, a6",
				"e4, e5",
				"a6, e2"
				);

			b.CurrentAdvantage.Should().Be(Advantage(2, 3), "Black is a bishop up");
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "No pieces have been taken");
		}

		[Fact]
		//Testing DrawCounter
		public void DrawCounterTest2() {
			ChessBoard b = CreateBoardFromMoves(
				"b2, b4",
				"g8, f6"
			);

			b.DrawCounter.Should().Be(1, "black knight was moved");
			Apply(b, "b1, c3");
			b.DrawCounter.Should().Be(2, "white knight was moved");
			Apply(b, "b8, c6");
			b.DrawCounter.Should().Be(3, "black knight was moved");

			Apply(b, "c3, d5");
			b.DrawCounter.Should().Be(4, "white knight was moved");
			Apply(b, "f6, d5"); //Capture white knight
			b.DrawCounter.Should().Be(0, "black knight captured white knight");

			Apply(b, "g1, f3");
			b.DrawCounter.Should().Be(1, "white knight was moved");
			Apply(b, "d5, f4");
			b.DrawCounter.Should().Be(2, "black knight was moved");
			Apply(b, "e2, e4");
			b.DrawCounter.Should().Be(0, "white pawn was moved");
		}

		/// <summary>
		/// Test the method GetPossibleMoves()
		/// </summary>
		[Fact]
		public void TestPossibleMoves() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e4"), ChessPieceType.Bishop, 1,
				 Pos("a1"), ChessPieceType.King, 1,
				 Pos("h8"), ChessPieceType.King, 2,
				 Pos("b7"), ChessPieceType.Pawn, 2,
				 Pos("c2"), ChessPieceType.Pawn, 2
			);

			var possMoves = b.GetPossibleMoves();
			possMoves.Count().Should().Be(13, "There should be 3 moves from a1 King + 10 moves from e4 Bishop");
			possMoves.Should().Contain(Move("e4, d5"), "e4 to d5 should be a valid move")
			.And.Contain(Move("e4, b7"), "e4 to b7 should be a valid move")
			.And.Contain(Move("e4, h1"), "e4 to h1 should be a valid move")
			.And.Contain(Move("e4, f5"), "e4 to f5 should be a valid move")
			.And.NotContain(Move("e4, a8"), "e4 to a8 should not be a valid move because of a blocked pawn")
			.And.NotContain(Move("e4, e2"), "e4 to a2 should not be a valid move because of a Bishops can move diagonal");

		}
		[Fact] //w
		public void UndoMultiplePieces() {
			ChessBoard b = CreateBoardFromMoves(
				 "a2, a4",
				 "a7, a5",
				 "a1, a3",
				 "h7, h5",
				 "a3, h3",
				 "h5, h4",
				 "h3, h4"
			);

			b.GetPieceAtPosition(Pos("h4")).PieceType.Should().Be(ChessPieceType.Rook, "Rook should be at h4");

			b.UndoLastMove();

			b.GetPieceAtPosition(Pos("h3")).PieceType.Should().Be(ChessPieceType.Rook, "Rook should be back at h3");
			b.GetPieceAtPosition(Pos("h4")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn should be back at h4");
		}

		/// <summary>
		/// At least 2 tests must involve Undo Last Move.
		/// </summary>
		[Fact]
		public void checkingScoreUndoingThreeMoves() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "d7, d5",
				 "f2, f3"
			);

			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("d5"));
			expected.Should().Contain(Move("d5, d4"))
				 .And.Contain(Move("d5, e4"))
				 .And.HaveCount(2, "a pawn can capture diagonally ahead or move forward");

			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no operations have changed the advantage");

			Apply(b, Move("d5, e4"));
			b.GetPieceAtPosition(Pos("e4")).Player.Should().Be(2, "Player 2 captured Player 1's pawn diagonally");
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "White lost a single pawn of 1 value");

			Apply(b, Move("g1, h3"));

			Apply(b, Move("e4, f3"));
			b.GetPieceAtPosition(Pos("f3")).Player.Should().Be(2, "Player 2 captured Player 1's pawn diagonally");
			b.CurrentAdvantage.Should().Be(Advantage(2, 2), "White lost a single pawn of 1 value");

			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();

			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("d5"));
			expected.Should().Contain(Move("d5, d4"))
				 .And.Contain(Move("d5, e4"))
				 .And.HaveCount(2, "a pawn can capture diagonally ahead or move forward");

			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "board should be reverted back to when no pieces were captured");


		}

		/// <summary>
		///  Check undo untill undo is no longer possible and check illegal play after undoing
		/// </summary>
		[Fact]
		public void CheckTrickyUndo() {
			ChessBoard b = new ChessBoard();
			var pieces = GetAllPiecesForPlayer(b, 1).ToList();
			var possMoves = b.GetPossibleMoves();
			Apply(b, "b2, b3"); // one space move from p1
			Apply(b, "a7, a6"); // one space move from p2
			Apply(b, "b3, b4"); // one space move from p1
			Apply(b, "a6, a5"); // one space move from p2
			Apply(b, "b4, a5"); // Pawn from p1 take pawn from p2

			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "player 1 should have the advantage over player 2");
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "draw situation (back at starting state)");

			// Test undoing when the list of undo move should be empty
			b.Invoking(y => y.UndoLastMove())
			  .Should().Throw<InvalidOperationException>("Undoing a move when there is no more move to undo should be illegal");

			// Checking position of the pawns after multiple undoing moves
			b.PositionIsEmpty(Pos("b3")).Should().BeTrue("previous piece position should contain piece again after undo a move");
			b.PositionIsEmpty(Pos("b2")).Should().BeFalse("new piece position on board should not contain the moved piece anymore after an undo");
			b.PositionIsEmpty(Pos("a6")).Should().BeTrue("previous piece position should contain piece again after undo a move");
			b.PositionIsEmpty(Pos("a7")).Should().BeFalse("new piece position on board should not contain the moved piece anymore after an undo");

			// trying to play when it's not the player turn after undoing move
			b.Invoking(y => Apply(b, "a7, a6"))
			  .Should().Throw<InvalidOperationException>("Moving a pawn when its not player's turn after undoing move should be illegal");
		}

		///<summary>
		/// Determines board state before and after a white-to-black capture
		///</summary>
		[Fact]
		public void UndoWhiteCapturesBlack() {
			ChessBoard cb = CreateBoardFromPositions(
				 Pos(0, 4), ChessPieceType.King, 2,
				 Pos(2, 2), ChessPieceType.Knight, 2,
				 Pos(3, 0), ChessPieceType.Queen, 2,
				 Pos(3, 5), ChessPieceType.Pawn, 2,  // black pawn to be captured
				 Pos(4, 2), ChessPieceType.Bishop, 1,
				 Pos(4, 6), ChessPieceType.Pawn, 1,  // white pawn to capture
				 Pos(5, 4), ChessPieceType.Rook, 1,
				 Pos(6, 2), ChessPieceType.King, 1);

			//white pawn captures black pawn
			Apply(cb, Move("g4, f5"));
			cb.GetPieceAtPosition(Pos(3, 5)).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn is at f5");
			cb.GetPieceAtPosition(Pos(3, 5)).Player.Should().Be(1, "Pawn at f5 is white");
			cb.CurrentAdvantage.Should().Be(Advantage(2, 3), "P2 loses black pawn");

			//board returns to state in which black pawn is not captured
			cb.UndoLastMove();
			cb.GetPieceAtPosition(Pos(3, 5)).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn is at f5");
			cb.GetPieceAtPosition(Pos(3, 5)).Player.Should().Be(2, "Pawn at f5 is black");
			cb.CurrentAdvantage.Should().Be(Advantage(2, 4), "P2 gains back black pawn");
		}

		/// <summary>
		/// White Pawn capture black bishop and undo it.
		/// </summary>
		[Fact]
		public void UndoCapture() {
			ChessBoard b = CreateBoardFromMoves(
				 "a2, a4",
				 "b7, b6",
				 "b2, b4",
				 "c8, a6",
				 "c2, c3",
				 "a6, b5"
			);
			//Check if capture is possible
			var possMoves = b.GetPossibleMoves();
			var forPawn = GetMovesAtPosition(possMoves, Pos("a4"));
			forPawn.Should().HaveCount(2, "White Pawn can move forward or capture Black's bishop")
				 .And.BeEquivalentTo(Move("a4, a5"), Move("a4, b5"));

			//Check if capture is done
			Apply(b, "a4, b5");
			var pawn = b.GetPieceAtPosition(Pos("b5"));
			pawn.Player.Should().Be(1, "white pawn performed capture on bishop");
			pawn.PieceType.Should().Be(ChessPieceType.Pawn);

			//Check if undo a capture is done correctly
			b.UndoLastMove();
			pawn = b.GetPieceAtPosition(Pos("a4"));
			pawn.Player.Should().Be(1, "white pawn returned to original spot");
			pawn.PieceType.Should().Be(ChessPieceType.Pawn);
			var captured = b.GetPieceAtPosition(Pos("b5"));
			captured.Player.Should().Be(2, "black bishop returned to original spot");
			captured.PieceType.Should().Be(ChessPieceType.Bishop);

		}

		/*
		3. Checks if Moves history is working with undo
		Checks if a pawn is unable to move after first move 
		 */
		[Fact]
		public void CheckHistoryWithUndo_CheckFirstTwoMovePawn() {
			ChessBoard board = new ChessBoard();
			IReadOnlyList<ChessMove> history = board.MoveHistory;
			// check inital moves list
			history.Should().HaveCount(0, "Moves History should have size zero because there has not been any moves");

			Apply(board, "a2, a4"); //move  pawn 2 spaces forward
			board.GetPieceAtPosition(Pos("a4")).PieceType.Should().Be(ChessPieceType.Pawn, "there should be a pawn at a4 after moving from a2");
			history.Should().HaveCount(1, "Moves History should have size 1 after 1 move");
			board.UndoLastMove();
			board.GetPieceAtPosition(Pos("a2")).PieceType.Should().Be(ChessPieceType.Pawn, "there should be a pawn at a2 after undo");
			history.Should().HaveCount(0, "Moves History should have size after undo");
			Apply(board, "a2, a4"); //move  pawn 2 spaces forward
			board.GetPieceAtPosition(Pos("a4")).PieceType.Should().Be(ChessPieceType.Pawn, "there should be a pawn at a4 after moving from a2");
			Apply(board, "h7, h6");

			//check that the pawn cannot move 2 spaces anymore
			var oneMoveExpected = GetMovesAtPosition(board.GetPossibleMoves(), Pos("a4"));
			oneMoveExpected.Should().HaveCount(1, "the pawn cannot move 2 spaces anymore after moving 2 spaces on its first move")
				.And.Contain(Move("a4, a5")).And.NotContain(Move("a4, a6"));
		}

		[Fact]
		public void DrawCounterTest() {
			ChessBoard b = new ChessBoard();
			Apply(b, "b1, a3");
			b.DrawCounter.Should().Be(1, "a knight moved without capture");
			Apply(b, "a7, a5");
			b.DrawCounter.Should().Be(0, "a pawn moved");
			Apply(b, "a3, c4");
			b.DrawCounter.Should().Be(1, "a knight moved without capture");
			Apply(b, "b8, a6");
			b.DrawCounter.Should().Be(2, "two knights moved without capture");
			Apply(b, "c4, a5");
			b.DrawCounter.Should().Be(0, "a knight captured a pawn");

			b.UndoLastMove();
			b.DrawCounter.Should().Be(2, "undid a capture move; DrawCounter should be restored to its value before the capture, which was 2");
			b.UndoLastMove();
			b.DrawCounter.Should().Be(1, "undid a knight move; DrawCounter should go down by 1");
			b.UndoLastMove();
			b.DrawCounter.Should().Be(0, "undid a knight move; DrawCounter should go down by 1");
			b.UndoLastMove();
			b.DrawCounter.Should().Be(1, "undid a pawn move; DrawCounter should be restored to its value before the move, which was 1");
		}
	}
}
