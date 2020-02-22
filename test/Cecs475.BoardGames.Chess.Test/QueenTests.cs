using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class QueenBoardTests : ChessTest {
		[Fact]
		public void QueenAvailableMovesTest() {
			ChessBoard b = new ChessBoard();
			IEnumerable<ChessMove> possMoves = b.GetPossibleMoves();
			IEnumerable<ChessMove> surroundedQueen = GetMovesAtPosition(possMoves, Pos("e8"));
			surroundedQueen.Should().BeEmpty("The queen is surround by all the other pieces so it cannot move");

		}

		///<summary>
		///Test to check available moves for queens(black and white) 
		///</summary>
		[Fact]
		public void QueensPossibleMoves() {
			//Creating a new board 
			ChessBoard b = new ChessBoard();

			var posMoves = b.GetPossibleMoves();
			var checkWhiteQueensMoves = GetMovesAtPosition(posMoves, Pos("d1"));
			checkWhiteQueensMoves.Should().HaveCount(0, "When the game starts the queen shouldn't have possible moves");

			//apply white's turn
			Apply(b, "c2,c3");

			//first turn of the black, the black queen should have 0 possible moves 
			posMoves = b.GetPossibleMoves();
			var checkBlackQueensMoves = GetMovesAtPosition(posMoves, Pos("d8"));
			checkBlackQueensMoves.Should().HaveCount(0, "On first move of black the black queen shouldn't have possible moves");

			//Since black didn't perform his turn, white's queen should still have 0 available moves even though it's left upper diagonal is open
			posMoves = b.GetPossibleMoves();
			checkWhiteQueensMoves = GetMovesAtPosition(posMoves, Pos("d1"));
			checkWhiteQueensMoves.Should().HaveCount(0, "This is black's turn, white queen should have 0 possible moves");

			//apply black's turn 
			Apply(b, "d7,d6");

			//Whites turn, after moving the pawn in c2, the white queen should have 3 moves 
			posMoves = b.GetPossibleMoves();
			checkWhiteQueensMoves = GetMovesAtPosition(posMoves, Pos("d1"));
			checkWhiteQueensMoves.Should().HaveCount(3, "3 possible moves on the left upper diagonal");

			//Apply white's turn - move the queen to the boarder of the board -- put the black king in check
			Apply(b, "d1, a4");

			//Black's queen should have only one possible move after the pawn opened d7
			posMoves = b.GetPossibleMoves();
			checkBlackQueensMoves = GetMovesAtPosition(posMoves, Pos("d8"));
			checkBlackQueensMoves.Should().HaveCount(1, "On first move of black the black queen shouldn't have possible moves");

			//Board should be in check 
			b.IsCheck.Should().BeTrue("The black king is in check");

			//Apply black's turn 
			Apply(b, "c7,c6");

			//Board should not be in check 
			b.IsCheck.Should().BeFalse("The black king is not in check");

			//Whites turn, since the white queen is at the boarder 
			posMoves = b.GetPossibleMoves();
			checkWhiteQueensMoves = GetMovesAtPosition(posMoves, Pos("a4"));
			checkWhiteQueensMoves.Should().HaveCount(16, "");


		}

		/// <summary>
		/// A pinned Queen can only moe along the file or rank it is currently pinned on
		/// </summary>
		[Fact]
		public void PartialPinOnQueen() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"d7, d5",
				"e4, d5",
				"d8, d6",
				"d1, e2"
				);

			//Move black's Queen to pin white's Queen
			Apply(b, "d6, e6");

			var possMoves = b.GetPossibleMoves();
			var pinnedQueenExpected = GetMovesAtPosition(possMoves, Pos("e2"));
			pinnedQueenExpected.Should().HaveCount(4, "Queen can move 3 empty spaces forward, and capture the opposing Queen in the 4th space forward")
				.And.Contain(Move("e2, e3"))
				.And.Contain(Move("e2, e4"))
				.And.Contain(Move("e2, e5"))
				.And.Contain(Move("e2, e6"));
		}

		/// <summary>
		/// Move Queen Into Check 
		/// Test possible moves that Can escape Check
		/// </summary>
		[Fact]
		public void QueenMove_IntoCheck() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e3",
				"a7, a5",
				"d1, h5",
				"a8, a6",
				"h5, a5",
				"h7, h5",
				"h2, h4",
				"a6, h6",
				"a5, c7",
				"f7, f6"
			);

			//Move queen and force a Check
			Apply(b, Move("c7, d7"));

			//Check board states
			b.IsCheck.Should().BeTrue("The king is threatened by the Queen at d7");
			b.IsCheckmate.Should().BeFalse("The kind is not in check mate by Queen at d7");
			b.CurrentPlayer.Should().Be(2, "Player 2 should have the next move, Player one put Player 2 in Check, and King has an escape");

			//get the moves of all possible moves
			var possMoves = b.GetPossibleMoves();

			//get the moves of the Player 2's Queen
			var queenMoves = GetMovesAtPosition(possMoves, Pos("d8"));
			queenMoves.Should().Contain(Move("d8, d7")).And.HaveCount(1, "Queen can only attack Queen to save King from Check");

			//get the moves of the Player 2's King
			var kingMoves = GetMovesAtPosition(possMoves, Pos("e8"));
			kingMoves.Should().Contain(Move("e8, d7"))
				.And.Contain(Move("e8, f7"))
				.And.HaveCount(2, "King has 2 moves, attack Queen at d7 or run away to f7");
		}

		//applies certain moves to the chess board and makes sure the number of moves the queen can do is correct
		[Fact]
		public void GetPossibleMoves_Queen() {

			ChessBoard b = new ChessBoard();

			//checks the initial state of the board to make sure the queen has no moves
			var possMoves = b.GetPossibleMoves();
			var noMoves = GetMovesAtPosition(possMoves, Pos("d1"));
			noMoves.Should().BeEmpty("The queen should not be able to move at all at the beginning of the game");

			//move the white pawn in the next column to the queen two spaces forward
			Apply(b,
				"e2, e4",
				"c7, c5");

			//The queen should now be able to move diagonally with 4 possible positions
			possMoves = b.GetPossibleMoves();
			var oneMove = GetMovesAtPosition(possMoves, Pos("d1"));
			oneMove.Should().HaveCount(4, "The queen can move diagonally").And.BeEquivalentTo(Move("d1, e2"),
				Move("d1, f3"), Move("d1, g4"), Move("d1, h5"));

			//move the queen diagonally to the right 
			Apply(b,
				"d1, h5",
				"g7, g6");

			//The queen should now be able to move in multiple directions
			possMoves = b.GetPossibleMoves();
			var multipleMoves = GetMovesAtPosition(possMoves, Pos("h5"));
			multipleMoves.Should().HaveCount(14, "The queen can move in multiple directions");
		}

		/// <summary>
		/// Check all possible moves of white queen.
		/// </summary>
		[Fact]
		public void QueenPossibleMoves4() {
			ChessBoard b = CreateBoardFromMoves();

			var possMoves = b.GetPossibleMoves();
			var queenMoves = GetMovesAtPosition(possMoves, Pos("d1"));
			queenMoves.Should().HaveCount(0, "the queen at d1 is blocked by surrounding pieces and pawns");
			Apply(b, "d2, d4"); // move white pawn out of the way
			Apply(b, "a7, a6"); // black move
			possMoves = b.GetPossibleMoves();
			queenMoves = GetMovesAtPosition(possMoves, Pos("d1"));
			queenMoves.Should().HaveCount(2, "the queen at d1 is no longer blocked by pawn and can move forward");

			Apply(b, "d1, d2"); // move queen forward one square
			Apply(b, "a6, a5"); // black move
			possMoves = b.GetPossibleMoves();
			queenMoves = GetMovesAtPosition(possMoves, Pos("d2"));
			queenMoves.Should().HaveCount(9, "the queen at d2 can either move forwards, backwards, or diagonally left and can capture pawn, or diagonally right");

			Apply(b, "d2, d3"); // move queen forward one square
			Apply(b, "a5, a4"); // black move
			possMoves = b.GetPossibleMoves();
			queenMoves = GetMovesAtPosition(possMoves, Pos("d3"));
			queenMoves.Should().HaveCount(16, "the queen at d2 can move backwards, or diagonally left, or diagonally right and can capture pawn");

			Apply(b, "d3, f5"); // move queen forward one square
			Apply(b, "a4, a3"); // black move
			possMoves = b.GetPossibleMoves();
			queenMoves = GetMovesAtPosition(possMoves, Pos("f5"));
			queenMoves.Should().HaveCount(19, "the queen at f5 can move in all directions");

			Apply(b, "d4, d5"); // white move
			Apply(b, "e7, e6"); // black move
			Apply(b, "d5, e6"); // white pawn capture black pawn
			Apply(b, "a8, a7"); // black move
			Apply(b, "f5, e5"); // white move
			Apply(b, "a7, a8"); // black move
			Apply(b, "e6, d7"); //white queen and pawn put king in check

			b.IsCheck.Should().BeTrue("the black king is threatened by the pawn at d7 and queen at e5");
			possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(1, "black king must capture white pawn to avoid capture by white queen")
				.And.BeEquivalentTo(Move("e8, d7"));

			Apply(b, "e8, d7"); // black move
			possMoves = b.GetPossibleMoves();
			queenMoves = GetMovesAtPosition(possMoves, Pos("e5"));
			queenMoves.Should().HaveCount(20, "the queen at e5 can move in all directions");

			Apply(b, "c1, g5"); // white move
			Apply(b, "a8, a7"); // black move
			Apply(b, "e5, d5"); // white queen and bishop puts king in check

			b.IsCheck.Should().BeTrue("the black king is threatened by the pawn at d7 and queen at e5");
			possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(2, "black king can move or black bishop can move in front of king to avoid king's capture by white queen")
				.And.BeEquivalentTo(Move("d7, e8"), Move("f8, d6"));

			Apply(b, "d7, e8"); // black move
			Apply(b, "d5, b7"); // queen captures black pawn
			Apply(b, "h7, h6"); // black move
			Apply(b, "b7, a8"); // queen moves into a corner
			Apply(b, "h6, h5"); // black move

			possMoves = b.GetPossibleMoves();
			queenMoves = GetMovesAtPosition(possMoves, Pos("a8"));
			queenMoves.Should().HaveCount(7, "the queen at a8 can only move right diagonally or downwards (capturing a rook) or right (capturing a knight");
		}

		[Fact]
		public void QueenPossibleMoves3() {
			ChessBoard b = CreateBoardFromMoves(
				 "d2, d4",
				 "b8, c6",
				 "d1, d3",
				 "c6, b8",
				 "d4, d5",
				 "b8, c6",
				 "d5, c6",
				 "b7, c6"
				 );
			var possibleMoves = b.GetPossibleMoves();
			var QueenMoves = GetMovesAtPosition(possibleMoves, Pos("d3"));
			QueenMoves.Should().HaveCount(20, "Queen can move in 20 different squares this turn");
		}


		[Fact]
		public void ValidatingGetPossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a3",
				"d7, d5",
				"b2, b3",
				"d8, d6",
				"c2, c3"
				);
			var possMoves = b.GetPossibleMoves();
			GetMovesAtPosition(possMoves, Pos("d6")).Should().HaveCount(16, "Queen has 16 possible moves")
								.And.Contain(Move("d6, d7"))
								.And.Contain(Move("d6, d8"))
								.And.Contain(Move("d6, c6"))
								.And.Contain(Move("d6, b6"))
								.And.Contain(Move("d6, a6"))
								.And.Contain(Move("d6, e6"))
								.And.Contain(Move("d6, f6"))
								.And.Contain(Move("d6, g6"))
								.And.Contain(Move("d6, h6"))
								.And.Contain(Move("d6, e5"))
								.And.Contain(Move("d6, f4"))
								.And.Contain(Move("d6, g3"))
								.And.Contain(Move("d6, h2"))
								.And.Contain(Move("d6, a3"))
								.And.Contain(Move("d6, b4"))
								.And.Contain(Move("d6, c5"));
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0));
			b.CurrentPlayer.Should().Be(1, "undoing the Pawn's move from c2 to c3 will allow player one to make the next move");
			b.GetPieceAtPosition(Pos("d6")).PieceType.Should().Be(ChessPieceType.Queen, "Black's Queen at position (3, 5)");
			b.GetPieceAtPosition(Pos("d5")).PieceType.Should().Be(ChessPieceType.Pawn, "Black's pawn at position (3, 4)");
			b.GetPieceAtPosition(Pos("c2")).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn at position (2, 1)");
			b.GetPieceAtPosition(Pos("b3")).PieceType.Should().Be(ChessPieceType.Pawn, "White's king at position (1, 2)");
			b.GetPieceAtPosition(Pos("a3")).PieceType.Should().Be(ChessPieceType.Pawn, "White's king at position (0, 2)");
		}


		/// <summary>
		/// Queen gets captured then capture is undone
		/// </summary>
		[Fact]
		public void QueenCaptureUndo() {
			ChessBoard b = CreateBoardFromMoves(
				"c2, c4",
				"c7, c5",
				"d2, d4",
				"b8, c6",
				"c1, e3",
				"d8, a5",
				"e3, d2"
			);

			Apply(b, Move("a5, d2")); // black queen capture white bishop
			b.PositionIsAttacked(Pos("d2"), 1).Should().BeTrue("Player 1's queen should be threatened by Player 2's queen (d1)");
			b.IsCheck.Should().BeTrue("Player 1's king should be in check by Player 2's black queen");
			b.IsCheckmate.Should().BeFalse("Player 1's king should not be in checkmate");

			Apply(b, Move("d1, d2")); // white queen captures black queen
			b.IsCheck.Should().BeFalse("Player 1's king should not be in check");
			b.IsCheckmate.Should().BeFalse("Player 1's king should not be in checkmate");

			b.UndoLastMove(); // black queen returns to previous state
			b.PositionIsAttacked(Pos("d2"), 1).Should().BeTrue("Player 2's queen should be threatened by Player 1's queen (d1) and king (e1)");
		}

		[Fact]
		public void QueenPossibleMoves2() {
			ChessBoard cBoard = CreateBoardFromMoves(
				"c2, c4",
				"e7, e5",
				"d1, b3", // white queen
				"d8, f6", // black queen
				"b1, c3", // white knight
				"g7, g6",
				"c4, c5",
				"h7, h6",
				"c5, c6",
				"f8, b4" // black bishop
			);

			var possibleMoves = cBoard.GetPossibleMoves();
			var whiteQueenPossibleMoves = GetMovesAtPosition(possibleMoves, Pos("b3"));
			whiteQueenPossibleMoves.Should().HaveCount(9, " ..").
				And.Contain(Move("b3, a3")).
				And.Contain(Move("b3, a4")).
				And.Contain(Move("b3, b4")).
				And.Contain(Move("b3, c4")).
				And.Contain(Move("b3, d5")).
				And.Contain(Move("b3, e6")).
				And.Contain(Move("b3, f7")).
				And.Contain(Move("b3, c2")).
				And.Contain(Move("b3, d1")).

				And.NotContain(Move("b3, b2")). // Forbidden move
				And.NotContain(Move("b3, a2"))  // Forbidden move
				;
			// White queen makes a move
			Apply(cBoard, "b3, b4");

			possibleMoves = cBoard.GetPossibleMoves();
			var blackQueenPossibleMoves = GetMovesAtPosition(possibleMoves, Pos("f6"));
			blackQueenPossibleMoves.Should().HaveCount(12, "Total of possible moves for black queen should be 12").
				And.Contain(Move("f6, e6")).
				And.Contain(Move("f6, d6")).
				And.Contain(Move("f6, c6")).
				And.Contain(Move("f6, e7")).
				And.Contain(Move("f6, d8")).
				And.Contain(Move("f6, g7")).
				And.Contain(Move("f6, g5")).
				And.Contain(Move("f6, h4")).
				And.Contain(Move("f6, f5")).
				And.Contain(Move("f6, f4")).
				And.Contain(Move("f6, f3")).

				And.NotContain(Move("f6, f7")). // Forbidden move
				And.NotContain(Move("f6, g6")). // Forbidden move
				And.NotContain(Move("f6, e5"))  // Forbidden move
				;
		}
	}
}
