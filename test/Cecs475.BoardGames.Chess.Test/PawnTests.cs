using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using Cecs475.BoardGames.Model;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Test {
	public class PawnTests : ChessTest {
		/// <summary>
		/// Tests that an en passant is possible (white side)
		/// </summary>
		[Fact]
		public void EnPassantTest() {
			ChessBoard b = CreateBoardFromMoves(
			"d2, d4",   //white pawn forward 2
			"a7, a6",   //random move
			"d4, d5",   //white pawn forward 1
			"e7, e5"    //black pawn forward 2
			);
			var possMoves = b.GetPossibleMoves();
			var enPassantExpected = GetMovesAtPosition(possMoves, Pos("d5"));
			//Checks if the white pawn has two possible moves:
			//forward one or an en passant
			enPassantExpected.Should().HaveCount(2, "white pawn can move forward one or en passant")
				 .And.Contain(Move("d5, d6"))
				 .And.Contain(Move(Pos("d5"), Pos("e6"), ChessMoveType.EnPassant));
		}

		/// <summary>
		/// Perform an En Passant move from the white side. Make sure all conditions are met for the conditions to perform the move and the pawn is in the
		/// correct position after the move
		/// </summary>
		[Fact]
		public void checkEnPassant() {
			ChessBoard board = new ChessBoard();
			Apply(board, Move(Pos("d2"), Pos("d4")));
			Apply(board, Move(Pos("a7"), Pos("a5")));
			Apply(board, Move(Pos("d4"), Pos("d5")));
			Apply(board, Move("h7, h5"));
			Apply(board, Move(Pos("b2"), Pos("b4")));
			Apply(board, Move("h5, h4"));
			Apply(board, Move("b4, b5"));
			Apply(board, Move(Pos("e7"), Pos("e5")));
			var moves = board.GetPossibleMoves();
			var pMove = GetMovesAtPosition(moves, Pos("d5"));
			var pMove2 = GetMovesAtPosition(moves, Pos("b5"));
			pMove.Should().HaveCount(2, "2 moves. One forward, and one diagonal due to en passant possible move");
			pMove2.Should().HaveCount(1, "Only able to move forward because en passant conditions are not fulfilled");
			Apply(board, Move(Pos("d5"), Pos("e6"), ChessMoveType.EnPassant));
			board.GetPieceAtPosition(Pos("e6")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn has moved diagonally to the right");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Empty, "There is no pawn on e5 since it was taken");
		}

		/// <summary>
		/// Take a pawn piece from a position in the board. Verify taken pawn is properly registered. Test primarily the black pawns on the board
		/// </summary>
		[Fact]
		public void checkTaken() {
			ChessBoard board = CreateBoardFromMoves("d2, d4", "e7, e5");
			Apply(board, Move(Pos("d4"), Pos("e5")));
			board.GetPieceAtPosition(Pos("d4")).PieceType.Should().Be(ChessPieceType.Empty, "No chess piece at d4 since pawn was moved");
			board.GetPlayerAtPosition(Pos("e5")).Should().Be(1, "First player pawn should be at position e5 since pawn was taken");
			board.UndoLastMove();
			board.GetPieceAtPosition(Pos("d4")).PieceType.Should().Be(ChessPieceType.Pawn, "White pawn should be back at position since last move was undone");
			board.GetPlayerAtPosition(Pos("e5")).Should().Be(2, "Second player pawn should be at position e5 since previous move was undone");
			Apply(board, Move(Pos("f2"), Pos("f4")));
			Apply(board, Move(Pos("e5"), Pos("f4")));
			board.GetPlayerAtPosition(Pos("f4")).Should().Be(2, "Black pawn took white pawn");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Empty, "There should be no chess piece in previous position");
		}

		/// <summary>
		/// Promote a White pawn to queen.
		/// </summary>
		[Fact]
		public void PawnPromotion_WhitePawnToQueen() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"b7, b5",
				"a4, b5", //capture black pawn with white pawn
				"b8, c6",
				"b5, b6",
				"c6, b4",
				"b6, b7",
				"b4, d5"
			);
			Apply(b, Move("(b7, b8, Queen"));
			b.GetPieceAtPosition(Pos("b8")).PieceType.Should().Be(ChessPieceType.Queen, "Rhe pawn was replaced by a queen");
			b.GetPieceAtPosition(Pos("b8")).Player.Should().Be(1, "the queen is controlled by player 1");
		}

		/// <summary>
		/// En passant test for black pawn to capture white pawn
		/// </summary>
		[Fact]
		public void EnPassantTest_BlackPawnCaptureWhitePawn() {
			ChessBoard b = CreateBoardFromMoves(
				"h2, h3",
				"b7, b5",
				"h3, h4",
				"b5, b4"
			);
			// Move white pawn forward twice, enabling en passant from black pawn from b4
			Apply(b, "a2, a4");

			var possMoves = b.GetPossibleMoves();
			var enPassantExpected = GetMovesAtPosition(possMoves, Pos("b4"));
			enPassantExpected.Should().HaveCount(2, "Black pawn can move forward one or en passant")
				.And.Contain(Move("b4, b3"))
				.And.Contain(Move(Pos("b4"), Pos("a3"), ChessMoveType.EnPassant));

			// Apply the en passant
			Apply(b, Move(Pos("b4"), Pos("a3"), ChessMoveType.EnPassant));
			var pawn = b.GetPieceAtPosition(Pos("a3"));
			pawn.Player.Should().Be(2, "Pawn performed en passant move");
			pawn.PieceType.Should().Be(ChessPieceType.Pawn);
			var captured = b.GetPieceAtPosition(Pos("a4"));
			captured.Player.Should().Be(0, "Rhe pawn that moved to b5 was captured by en passant");
			b.CurrentAdvantage.Should().Be(Advantage(2, 1));
		}


		// Move white pawn to end of board to promote it to queen
		// Capture black pawn and bishop on the way
		[Fact]
		public void whitePawnPromotionAndAdvantage() {
			ChessBoard cb = CreateBoardFromMoves(
				"e2, e4",
				"d7, d5",
				"e4, d5",
				"d8, d6",
				"d2, d3",
				"d6, c5",
				"d5, d6",
				"c7, c6",
				"d6, d7",
				"e8, d8"
			);

			Apply(cb, Move("(d7, c8, Queen)"));
			cb.GetPieceAtPosition(Pos("c8")).PieceType.Should().Be(ChessPieceType.Queen, "the pawn was replaced by a queen");
			cb.GetPieceAtPosition(Pos("c8")).Player.Should().Be(1, "the queen is controlled by player 1");
			cb.CurrentAdvantage.Should().Be(Advantage(1, 12), "a black pawn and bishop were captured," +
				" while white gained a queen");
		}

		[Fact]
		public void boardStateAfterThreeSinglePawnMoves() {
			ChessBoard cb = CreateBoardFromMoves(
				"e2, e3",
				"d7, d6",
				"h2, h3"
			);

			cb.CurrentPlayer.Should().Be(2, "it should be black's turn after 3 moves.");
			var possMoves = cb.GetPossibleMoves();
			possMoves.Should().HaveCount(27, "there should be 27 possible moves for the black player.");
			possMoves.Should().Contain(Move("e8, d7"), "the black king should be able to move south-west");
			possMoves.Should().NotContain(Move("d8, d6"), "black queen should not be able to move to d6 since pawn is there");
		}
		/// <summary>
		/// Test an En Passant for a black pawn and Undo Move
		/// </summary>
		[Fact]
		public void BlackEnPassantTestAndUndo() {
			ChessBoard b = CreateBoardFromMoves(
					 "b1, a3",
					 "e7, e5",
					 "a3, b5",
					 "e5, e4"
			);

			// Move pawn forward twice, enabling en passant from e4
			Apply(b, "d2, d4");

			var possible = b.GetPossibleMoves();
			var enPassantExpected = GetMovesAtPosition(possible, Pos("e4"));
			enPassantExpected.Should().HaveCount(2, "pawn can move forward one or en passant")
				.And.Contain(Move("e4, e3"))
				.And.Contain(Move(Pos("e4"), Pos("d3"), ChessMoveType.EnPassant));

			// Apply the en passant
			Apply(b, Move(Pos("e4"), Pos("d3"), ChessMoveType.EnPassant));
			var pawn = b.GetPieceAtPosition(Pos("d3"));
			pawn.Player.Should().Be(2, "pawn performed en passant move");
			pawn.PieceType.Should().Be(ChessPieceType.Pawn, "The piece type should be a pawn");
			var captured = b.GetPieceAtPosition(Pos("d4"));
			captured.Player.Should().Be(0, "the pawn that moved to d4 was captured by en passant");
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "Black took white pawn of value 1");

			// Undo the move and check the board state
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "The advantage should be reset after undo");
			pawn = b.GetPieceAtPosition(Pos("e4"));
			pawn.Player.Should().Be(2, "Pawn should belong to player 2, black");
			captured = b.GetPieceAtPosition(Pos("d4"));
			captured.Player.Should().Be(1, "Originally captured piece should belong to player 1, white");
			var empty = b.GetPieceAtPosition(Pos("d3"));
			empty.Player.Should().Be(0, "there should be no piece at d3, should be empty");
		}

		[Fact]
		//This test will check for an enpassant
		public void CheckPassant() {
			ChessBoard board = CreateBoardFromMoves(
				"d2, d4",
				"h7, h5",
				"d4, d5");

			Apply(board, "e7, e5");

			Apply(board, Move(Pos("d5"), Pos("e6"), ChessMoveType.EnPassant));
			var wPlayerPawn = board.GetPieceAtPosition(Pos("e6"));
			wPlayerPawn.Player.Should().Be(1, "This should be white's piece");
			wPlayerPawn.PieceType.Should().Be(ChessPieceType.Pawn, "This should be a pawn");

			var bPlayerPawn = board.GetPieceAtPosition(Pos("e5"));
			bPlayerPawn.Player.Should().Be(0, "This should be black's piece");
			bPlayerPawn.PieceType.Should().Be(0, "This should be captured");
		}


		//<summary>
		//Test involving "tricky" situation 
		//Create a case wherein pawn can perform en passant on a black pawn whilst being blocked by a 
		//black pawn in front of it (WITH WHITE)
		//</summary>
		[Fact]
		public void MyEnPassantTest() {
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",
				"d7, d5",
				"e2, e4",
				"e7, e6"
				);

			Apply(b, "e4, e5", "f7, f5");

			var possibleMoves = b.GetPossibleMoves();
			var enPassant = GetMovesAtPosition(possibleMoves, Pos("e5"));
			enPassant.Should().HaveCount(1, "pawn at e5 can en passant, capturing pawn at f5")
			.And.Contain(Move(Pos("e5"), Pos("f6"), ChessMoveType.EnPassant));

			Apply(b, Move(Pos("e5"), Pos("f6"), ChessMoveType.EnPassant));
			var capturing = b.GetPieceAtPosition(Pos("f6"));
			capturing.Player.Should().Be(1, "white pawn performed en passant move");
			capturing.PieceType.Should().Be(ChessPieceType.Pawn, "white pawn catured black pawn");
			var captured = b.GetPieceAtPosition(Pos("f5"));
			captured.Player.Should().Be(0, "black pawn that moved to f5 was captured by en passant");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Black lost a pawn due to en passant ");
		}

		/// <summary>
		/// Check pawn promotion with two diagonals
		/// </summary>
		[Fact]
		public void PawnPromotionWithDiagonals() {
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"g7, g5",
				"f4, g5",
				"b7, b6",
				"g5, g6",
				"g8, f6",
				"g6, g7",
				"a7, a6"
				);

			var possMoves = b.GetPossibleMoves();
			var movesAtPos = GetMovesAtPosition(possMoves, Pos("g7"));

			movesAtPos.Should().HaveCount(12, "Pawn can move forward and promote or diagonal to promote")
				.And.OnlyContain(m => m.MoveType == ChessMoveType.PawnPromote, "the only moves available are PawnPromote moves");
		}

		[Fact]
		//Testing pawn promotion
		public void PawnPromotionTest() {
			ChessBoard b = CreateBoardFromMoves(
				"c2, c4",
				"h7, h5",
				"h2, h4",
				"a7, a5",
				"d1, a4",
				"a8, a6",
				"a4, a5", //Capture black pawn
				"a6, h6",
				"a5, c7", //Capture black pawn
				"d7, d5",
				"a2, a4",
				"d5, c4", //Capture white pawn
				"a4, a5",
				"c4, c3",
				"a5, a6",
				"e7, e5",
				"a6, a7",
				"h6, d6"
			);
			//Check for pawn promotion moves
			var possMoves = b.GetPossibleMoves();
			var pawnMoves = GetMovesAtPosition(possMoves, Pos("a7"));
			pawnMoves.Should().HaveCount(8, "there are eight possible promotion moves")
				.And.OnlyContain(m => m.MoveType == ChessMoveType.PawnPromote);

			//Apply promotion move
			Apply(b, Move("(a7, a8, Queen)"));
			b.GetPositionsOfPiece(ChessPieceType.Queen, 1).Should().Contain(Pos("a8"), "pawn promoted to queen");
			b.CurrentPlayer.Should().Be(2, "white just promoted pawn, black's turn");
		}


		[Fact]
		//Test case for an en passant move
		public void EnPassantMove() {
			//Create a board with en passant move option
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",
				"b7, b6",
				"d4, d5",
				"e7, e5"
			);

			//Check for possible en passant move
			var posMoves = b.GetPossibleMoves();
			var enPassant = GetMovesAtPosition(posMoves, Pos("d5"));
			enPassant.Should().Contain(Move("d5, e6"))
				.And.HaveCount(2, "a pawn can move diagonal to capture enemy pawn if that had just moved two spaces or move up one space")
				.And.Contain(Move(Pos("d5"), Pos("e6"), ChessMoveType.EnPassant));

			//Apply the en passant move
			Apply(b, "d5, e6");
			b.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Empty, "the pawn was captured by en passant move");
			b.GetPieceAtPosition(Pos("e5")).Player.Should().Be(0, "empty position, should not have any player");
			b.GetPieceAtPosition(Pos("e6")).Player.Should().Be(1, "pawn performed en passant move");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "the player captured a pawn");

			//Undo the en passant move and check if the pawn is restored
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Pawn, "the pawn that was captured by en passant move should have been restored");
			b.GetPieceAtPosition(Pos("e5")).Player.Should().Be(2, "pawn was restored");
			b.GetPieceAtPosition(Pos("e6")).Player.Should().Be(0, "pawn returned to original location");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "the pawn was restored");
		}

		/// <summary>
		/// Tests a pawn being promoted to a queen. Tests black pawn.
		/// </summary>
		[Fact]
		public void PawnBeingPromoted() {
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"g7, g5",
				"h2, h4",
				"g5, g4",
				"h1, h3",
				"g4, h3", // captures white rook with pawn black
				"g1, f3", //moves white knight 
				"h3, h2",
				"g2, g4"
			);

			Apply(b, Move("(h2, h1, Queen)"));
			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Queen, "The Black Pawn should be a Black Queen");
			b.GetPieceAtPosition(Pos("h1")).Player.Should().Be(2, "The Queen should be controlled by player 2 (Black)");
		}
		[Fact]
		public void pawnCount() {
			ChessBoard board = new ChessBoard();

			IEnumerable<BoardPosition> list = board.GetPositionsOfPiece(ChessPieceType.Pawn, 1);
			list.Should().HaveCount(8, "This test makes sure the board begins with the right amount of pawns");
		}
		/// <summary>
		/// TestEnPassantScenario
		/// Test special pawn move in which it can only occur immediately after a pawn makes a
		/// move of two squares from its starting square, and it could have been captured 
		/// by an enemy pawn had it advanced only one position.
		/// </summary>
		[Fact]
		public void EnPassantScenario() {
			ChessBoard cBoard = CreateBoardFromMoves(
				"a2, a4", // white pawn's first move = two positions
				"h7, h5", // black pawn's first move = two positions
				"a4, a5",
				"h5, h4",
				"g2, g4" // white pawn's first move = two positions
				);

			// White Player Test
			var blackPawnMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("h4"));
			blackPawnMoves.Should().Contain(Move(Pos("h4"), Pos("g3"), ChessMoveType.EnPassant), "black player pawn can catch white player pawn at g4");

			// Proceed with the En Passant Move, capturing and relocating according to En Passant rules
			Apply(cBoard, Move(Pos("h4"), Pos("g3"), ChessMoveType.EnPassant));

			// Black player pawn has been captured so g4 should be empty
			cBoard.GetPieceAtPosition(Pos("g4")).Player.Should().Be(0, "Enemy pawn has been captured which means that this position is empty");

			// Black Player Test
			Apply(cBoard, "d2, d3");
			Apply(cBoard, "b7, b5");  // black pawn's first move = two positions

			var whitePawnMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("a5"));
			whitePawnMoves.Should().Contain(Move(Pos("a5"), Pos("b6"), ChessMoveType.EnPassant), "white player pawn can catch black player pawn at b6");

			// Proceed with the En Passant Move, capturing and relocating according to En Passant rules
			Apply(cBoard, Move(Pos("a5"), Pos("b6"), ChessMoveType.EnPassant));

			// Black player pawn has been captured so b5 should be empty
			cBoard.GetPieceAtPosition(Pos("b5")).Player.Should().Be(0, "Enemy pawn has been captured which means that this position is empty");
		}
	}
}
