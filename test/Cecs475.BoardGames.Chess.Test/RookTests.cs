using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Test {
	public class RookTests : ChessTest {
		/// <summary>
		/// checks that Castling cannot happen if the rook has moved from and to its starting position
		/// </summary>
		[Fact]
		public void noCastlingFromNonNeutralWithUndos() {
			ChessBoard b = CreateBoardFromMoves(
					"g1, f3",
					"h7, h6",
					"e2, e3",
					"g7, g6",
					"f1, e2",
					"f7, f6"
				);

			var possMove = b.GetPossibleMoves();
			var Castling = GetMovesAtPosition(possMove, Pos("e1"));
			Castling.Should().HaveCount(2, "King can only move one space to the right, and be able to castle")
				.And.Contain(Move("e1,f1"))
				.And.Contain(Move("e1,g1"));

			//move bottom right rook one space to the left
			Apply(b, "h1, g1");

			//move another top pawn
			Apply(b, "e7, e6");

			//move rook back to its original space
			Apply(b, "g1, h1");

			//move another top pawn
			Apply(b, "d7, d6");

			possMove = b.GetPossibleMoves();
			var noCastling = GetMovesAtPosition(possMove, Pos("e1"));
			noCastling.Should().HaveCount(1, "King can only move one space to the right, and not be able to castle")
				.And.Contain(Move("e1,f1"));

			b.UndoLastMove(); //Undo Apply(b, "d7, d6");
			b.UndoLastMove(); //Undo Apply(b, "g1, h1");
			b.UndoLastMove(); //Undo Apply(b, "e7, e6");
			b.UndoLastMove(); //Undo Apply(b, "h1, g1");

			possMove = b.GetPossibleMoves();
			var CastlingAgain = GetMovesAtPosition(possMove, Pos("e1"));
			CastlingAgain.Should().HaveCount(2, "King can only move one space to the right, and be able to castle after all the Undos")
				.And.Contain(Move("e1,f1"))
				.And.Contain(Move("e1,g1"));
		}

		/// <summary>
		/// Verify castling move king side. King should move two spaces to the left and rook should be on its right side after castling.
		/// Test on both sides of the board
		/// </summary>
		[Fact]
		public void CheckCastling() {
			ChessBoard chess = CreateBoardFromMoves("b1, a3", "b8, a6", "b2, b3", "b7, b6", "c1, b2", "c8, b7", "d2, d3", "c7, c6", "d1, d2", "d8,  c7");
			Apply(chess, Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleKingSide));
			chess.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.King, "Piece should be white King since castling. King should have moved left by two spaces");
			chess.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Rook, "Piece should be white Rook on right side of king");
			Apply(chess, Move(Pos("e8"), Pos("c8"), ChessMoveType.CastleKingSide));
			chess.GetPieceAtPosition(Pos("c8")).PieceType.Should().Be(ChessPieceType.King, "Piece should be black King since castling. King should have moved left by two spaces");
			chess.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Rook, "Piece should be black Rook on right side of king");
			chess.UndoLastMove();
			chess.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "Black king piece should be at the starting position");
			chess.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Rook, "Black rook piece should be at the starting position");
		}

		[Fact]
		/// <summary>
		/// White rook should have 6 possible moves, two of them can capture: one black pawn or one black knight.
		/// </summary>
		public void WhiteRook_2PossibleCaptures() {
			ChessBoard b = CreateBoardFromMoves(
				"b1, c3",
				"f7, f5",
				"b2, b4",
				"g8, f6",
				"a1, b1",
				"c7, c6",
				"b1, b3",
				"e7, e5",
				"e2, e4",
				"f5, e4",
				"c3, e4",
				"f6, e4",
				"b4, b5",
				"c6, b5",
				"c2, c3",
				"e4, c3");

			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("b3"));
			expected.Should().Contain(Move("b3, b5"))
				.And.Contain(Move("b3, c3"))
				.And.HaveCount(6, "A rook in this position can move forward to eat the pawn or right to eat the knight or can move left or down.");

			b.CurrentAdvantage.Should().Be(Advantage(2, 5), "Black should have a 5 point advantage");

			Apply(b, Move("b3, b5"));
			b.GetPieceAtPosition(Pos("b5")).Player.Should().Be(1, "Player 1 captured Player 2's pawn vertically");
			b.CurrentAdvantage.Should().Be(Advantage(2, 4), "Black lost a single pawn of 1 value");

			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(2, 5), "After undoing the pawn capture, advantage is back to 5");

			Apply(b, Move("b3, c3"));
			b.GetPieceAtPosition(Pos("c3")).Player.Should().Be(1, "Player 1 captured Player 2's knight horizontally");
			b.CurrentAdvantage.Should().Be(Advantage(2, 2), "Black lost a single knight of 3 value");
		}

		/// <summary>
		/// Checks Black Rook possible turns.
		/// </summary>
		[Fact]
		public void CheckAllRookMoves() {
			List<string> cols = new List<string>() { "b", "c", "d", "e", "f", "g" };
			List<string> rows = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };

			ChessBoard b = CreateBoardFromPositions(
				Pos("a8"), ChessPieceType.Rook, 2,
				Pos("h8"), ChessPieceType.Rook, 2,
				Pos("b1"), ChessPieceType.King, 1,
				Pos("g1"), ChessPieceType.King, 2
			);

			Apply(b, "b1, c1");//blacks turn
			var possMoves = b.GetPossibleMoves();

			var aMoves = GetMovesAtPosition(possMoves, Pos("a8"));
			var bMoves = GetMovesAtPosition(possMoves, Pos("h8"));

			foreach (string row in rows) {
				aMoves.Should().Contain(Move("a8, a" + row), "Top Left rook should be able to move to any square in 'a' "); // all A vertical moves
			}
			foreach (string col in cols) {
				aMoves.Should().Contain(Move("a8, " + col + "8"), "Top Left rook should be able to move to any square on '8' (except for ah and h8)"); // all B horizontal moves
			}

			foreach (string row in rows) {
				bMoves.Should().Contain(Move("h8, h" + row), "Top Right rook should be able to move to any square in 'h' "); // all B vertical moves
			}
			foreach (string col in cols) {
				bMoves.Should().Contain(Move("h8, " + col + "8"), "Top Right rook should be able to move to any square on '8' (except for a8 and h8)"); // all B horizontal moves
			}
		}


		/// <summary>
		/// Check that a rook at b1 has all moves available
		/// </summary>
		[Fact]
		public void RookMovement() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("b1"), ChessPieceType.Rook, 1,
				Pos("c5"), ChessPieceType.King, 2,
				Pos("e3"), ChessPieceType.King, 1
			);

			var possMoves = b.GetPossibleMoves();
			//the a column seems to be bugged?
			var pos = Pos("b1");
			var movesAtPos = GetMovesAtPosition(possMoves, pos);

			movesAtPos.Should().HaveCount(14, "A rook in a corner has 14 possible moves")
					.And.BeEquivalentTo(
						Move(pos, pos.Translate(0, 1)),
						Move(pos, pos.Translate(0, 2)),
						Move(pos, pos.Translate(0, 3)),
						Move(pos, pos.Translate(0, 4)),
						Move(pos, pos.Translate(0, 5)),
						Move(pos, pos.Translate(0, 6)),
						Move(pos, pos.Translate(-1, 0)),
						Move(pos, pos.Translate(0, -1)),
						Move(pos, pos.Translate(-2, 0)),
						Move(pos, pos.Translate(-3, 0)),
						Move(pos, pos.Translate(-4, 0)),
						Move(pos, pos.Translate(-5, 0)),
						Move(pos, pos.Translate(-6, 0)),
						Move(pos, pos.Translate(-7, 0))
					);
		}

		[Fact]
		public void verify_rook() {
			ChessBoard b = CreateBoardFromMoves(
					"a2, a4",
					"h7, h6",
					"a1, a3",
					"h6, h5",
					"a3, b3",
					"h5, h4",
					"c2, c3",
					"h4, h3"
				);

			// rook should be able to move straight up the b row or one space to the left
			var possMoves = b.GetPossibleMoves();
			var rmoves = GetMovesAtPosition(possMoves, Pos("b3"));

			rmoves.Should().HaveCount(5, "rook has 5 possible moves").And.BeEquivalentTo(
				Move("b3, b4"), Move("b3, b5"), Move("b3, b6"), Move("b3, a3"), Move("b3, b7"));
		}

		[Fact]
		public void KingMoveswithTwoCastleOptions() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("c8"), ChessPieceType.King, 2,
				Pos("a1"), ChessPieceType.Rook, 1,
				Pos("h1"), ChessPieceType.Rook, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("a2"), ChessPieceType.Pawn, 1,
				Pos("h2"), ChessPieceType.Pawn, 1
			);

			var possMoves = b.GetPossibleMoves();
			var oneMove = GetMovesAtPosition(possMoves, Pos("e1"));
			oneMove.Should().HaveCount(7, "7 possible moves");

			oneMove = GetMovesAtPosition(possMoves, Pos("a1"));
			oneMove.Should().HaveCount(3, "3 possible moves");
			oneMove = GetMovesAtPosition(possMoves, Pos("h1"));
			oneMove.Should().HaveCount(2, "2 possible moves");

		}

		[Fact]
		public void RookCausingCheck() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4", //white pawn moves 2 forward
				"e7, e5", //black pawn moves 2 forward
				"a4, a5", //white pawn moves 1 forward
				"e8, e7", //black king moves 1 forward
				"a1, a4", //white rook moves 3 forward
				"e7, d6", //black king moves 1 right
				"a4, d4" //white rook moves 4 left
				);
			b.IsCheck.Should().BeTrue("the king is threatened by the rook at d4");
			b.IsCheckmate.Should().BeFalse("the king has an escape");
			var possMoves = b.GetPossibleMoves();
			GetMovesAtPosition(possMoves, Pos("d6")).Should().HaveCount(4, "king has 4 possible moves to escape")
								.And.Contain(Move("d6, c6"))
								.And.Contain(Move("d6, e6"))
								.And.Contain(Move("d6, c5"))
								.And.Contain(Move("d6, e7"));
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0));
			b.GetPieceAtPosition(Pos("d6")).PieceType.Should().Be(ChessPieceType.King, "Black's king at position (3, 5)");
			b.GetPieceAtPosition(Pos("a5")).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn at position (0, 4)");
			b.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Pawn, "Black's pawn at position (4, 4)");
			b.CurrentPlayer.Should().Be(1, "undoing the Rook's move from a4 to d4 will allow player one to make the next move");

		}

		/// <summary>
		/// Undoing a move after a capture. Tests black rook.
		/// </summary>
		[Fact]
		public void UndoRookCaptureMove() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"a7, a5",
				"b1, c3",
				"a8, a6",
				"c3, b5",
				"a6, b6",
				"b2, b4"
			);

			var whiteKnight = b.GetPieceAtPosition(Pos("b5"));
			whiteKnight.PieceType.Should().Be(ChessPieceType.Knight, "A White Knight should be at b5, waiting to be captured.");
			whiteKnight.Player.Should().Be(1, "Confirms that it is players 1 piece.");

			var blackRook = b.GetPieceAtPosition(Pos("b6"));
			blackRook.PieceType.Should().Be(ChessPieceType.Rook, "A Black Rook should be at b6.");
			blackRook.Player.Should().Be(2, "Confirms that it is players 2 piece.");

			Apply(b, Move(Pos("b6"), Pos("b5"), ChessMoveType.Normal));
			blackRook = b.GetPieceAtPosition(Pos("b5"));
			blackRook.PieceType.Should().Be(ChessPieceType.Rook, "A Black Rook captured a White Knight at b5.");
			blackRook.Player.Should().Be(2, "Confirms that it is players 2 piece.");


			b.UndoLastMove();
			blackRook = b.GetPieceAtPosition(Pos("b6"));
			blackRook.PieceType.Should().Be(ChessPieceType.Rook, "After Undo, A Black Rook should be at b6.");
			blackRook.Player.Should().Be(2, "After Undo, Confirms that it is players 2 piece.");

			whiteKnight.PieceType.Should().Be(ChessPieceType.Knight, "After Undo, A White Knight should be back at b5");
			whiteKnight.Player.Should().Be(1, "After Undo, Confirms that it is players 1 piece.");

		}
		[Fact]
		public void getRooksPossibleMoves() {
			ChessBoard board = CreateBoardFromMoves
				(
					"a2, a3",
					"a7, a6",
					"a3, a4",
					"a6, a5",
					"a1, a3",
					"b7, b6"
				);

			var moves = board.GetPossibleMoves();

			moves.Should().Contain(Move("a3, b3")).And.Contain(Move("a3, c3")).And.Contain(Move("a3, d3")).And.Contain(Move("a3, e3"))
				.And.Contain(Move("a3, f3")).And.Contain(Move("a3, g3")).And.Contain(Move("a3, h3"), "This test makes sure the moves reported by a Rook are correct");
		}
	}
}
