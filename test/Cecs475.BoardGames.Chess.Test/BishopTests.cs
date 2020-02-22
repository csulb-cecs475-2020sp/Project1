using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class BishopTests : ChessTest {
		/// <summary>
		/// Test Case #5
		/// Checks all possible moves for the Bishop
		/// </summary>

		[Fact]
		public void GetPossibleMovesBishop() {
			ChessBoard board = CreateBoardFromMoves(
					 Move("h2,h3"),
					 Move("d7,d5"));

			Apply(board, "a2,a3");
			var allPossibleMoves = GetMovesAtPosition(board.GetPossibleMoves(), Pos("c8"));
			allPossibleMoves.Should().HaveCount(5, "These are not all the possible moves for the Bishop")
				.And.Contain(Move("c8,d7"))
				.And.Contain(Move("c8,e6"))
				.And.Contain(Move("c8,f5"))
				.And.Contain(Move("c8,g4"))
				.And.Contain(Move("c8,h3"));
		}
		//check possible moves for bishop
		[Fact]
		public void checkWhiteBishopMoves() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"c7, c5",
				"f1, c4",
				"a7, a6"
				);
			var possible = b.GetPossibleMoves();
			var bishopMoves = GetMovesAtPosition(possible, Pos("c4"));
			bishopMoves.Count().Should().Be(9, "There should be 9 possible moves to move the white bishop");
			bishopMoves.Should().Contain(Move("c4, b3")).And.Contain(Move("c4, a6")).And.
				Contain(Move("c4, f7")).And.Contain(Move("c4, f1")).And.Contain(Move("c4, b5")).And.Contain(Move("c4, d3")).And.
				Contain(Move("c4, e2")).And.Contain(Move("c4, d5")).And.Contain(Move("c4, e6")).And.
				HaveCount(9, "a white bishop can capture diagonally ahead, move forward diagonally in either left/right direction or backwards diagonally in either left/right direction");
		}

		/// <summary>
		/// Checks that black's bishop only has one possible move in the case of a check
		/// </summary>
		[Fact]
		public void BishopCheckBlock() {
			ChessBoard b = CreateBoardFromMoves(
			"e2, e4",
			"d7, d5",
			"f1, b5"   //white bishop checks black king
			);

			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("c8"));
			expected.Should().Contain(Move("c8, d7"))
				 .And.HaveCount(1, "black bishop should only have one possible move, blocking the check");
		}


		/// <summary>
		/// A knight and bishop cannot move if there are pawns of the same color occupying its possible moves.
		/// </summary>
		[Fact]
		public void BlackKnightBishop_TrappedbyPawns() {
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",
				"a7, a6",
				"d1, d2",
				"c7, c6",
				"d2, f4"
			);

			//Possible moves for black knight should be 0
			var possMoves = b.GetPossibleMoves();
			var blackKnight = GetMovesAtPosition(possMoves, Pos("b8"));
			blackKnight.Should().HaveCount(0, "The black knight should have 0 moves because it is blocked");

			//Possible moves for black bishop should be 0
			var blackBishop = GetMovesAtPosition(possMoves, Pos("c8"));
			blackBishop.Should().HaveCount(0, "The black bishop should have 0 moves because it is blocked");
		}

		[Fact]
		/// <summary>
		/// Black bishop has the White King in check but the King has nowhere to move. A sacrifice must be made to protect the king.
		/// </summary>
		public void BlackBishop_ChecksWhiteKing_WhiteSacrifice() {
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",
				"e7, e5",
				"d4, e5",
				"f8, b4");

			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Black should have a 1 point advantage");
			b.IsCheck.Should().BeTrue("White's king is in check from bishop at b4");

			var poss = b.GetPossibleMoves();
			var expectedKing = GetMovesAtPosition(poss, Pos("b4"));
			expectedKing.Should().HaveCount(0, "A king in this position cannot move because it is blocked.");

			Apply(b, Move("d1, d2"));
			b.GetPieceAtPosition(Pos("d2")).Player.Should().Be(1, "Player 1 Queen has moved to block the king from the black bishop");

			b.UndoLastMove(); //But we don't want to sacrifice the queen so lets do bishop instead

			var expectedQueen = GetMovesAtPosition(poss, Pos("d1"));
			expectedQueen.Should().HaveCount(1, "The white queen possible moves should still be able to move to block even if undone")
				.And.Contain(Move("d1, d2"));

			Apply(b, Move("c1, d2"));
			b.GetPieceAtPosition(Pos("d2")).Player.Should().Be(1, "Player 1 Bishop has moved to block the king from the black bishop");

			Apply(b, Move("b4, d2"));
			b.CurrentAdvantage.Should().Be(Advantage(2, 2), "Black bishop has taken white bishop of 3 value, making black advantage 2");

			Apply(b, Move("d1, d2"));
			b.GetPieceAtPosition(Pos("d2")).Player.Should().Be(1, "Player 1 Queen captured Player 2's bishop by moving up one square");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Black lost a single bishop of 3 value");
		}


		// Surround white bishop by 4 pieces and test it can capture any of them
		[Fact]
		public void whiteBishopPossibleMoves() {
			ChessBoard cb = CreateBoardFromMoves(
				"c2, c3",
				"d7, d5",
				"d2, d3",
				"d5, d4",
				"c1, f4",
				"g8, f6",
				"f4, e5",
				"d8, d6",
				"h2, h3",
				"h7, h5",
				"g2, g3",
				"h5, h4",
				"a2, a3",
				"h8, h5",
				"f2, f3",
				"h5, f5",
				"e2, e3",
				"f5, f4"  // white bishop is surrounded by black pieces in all 4 directions
			);

			var possMoves = cb.GetPossibleMoves();
			var surroundedBishop = GetMovesAtPosition(possMoves, Pos("e5"));
			surroundedBishop.Should().HaveCount(4, "bishop can move in 4 directions");
			possMoves.Should().Contain(Move("e5, d6")).And.Contain(Move("e5, f6")).And.Contain(
				Move("e5, d4")).And.Contain(Move("e5, f4"), "white bishop can capture black queen, " +
				" knight, pawn, or rook");
			possMoves.Should().NotContain(Move("e5, e6"));
		}

		[Fact]
		//This will check for possible moves of a bishop
		public void PossibleMovesTest() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"d7, d5",
				"b2, b4");

			Apply(b, "c8, f5");


			b.GetPieceAtPosition(Pos("f5")).PieceType.Should().Be
				(ChessPieceType.Bishop, "This should be a bishop.");

			var possMoves = b.GetPossibleMoves();
			var bishopMoves = GetMovesAtPosition(possMoves, Pos("f5"));
			bishopMoves.Should().HaveCount(0, "This bishop should have 0 moves because it isn't black's turn");

			Apply(b, "c2, c4");
			GetMovesAtPosition(b.GetPossibleMoves(), Pos("f5")).Should().HaveCount(10, "This bishop should have 10 moves").
				And.Contain(Move("f5, e6")).
				And.Contain(Move("f5, d7")).
				And.Contain(Move("f5, c8")).
				And.Contain(Move("f5, g6")).
				And.Contain(Move("f5, e4")).
				And.Contain(Move("f5, d3")).
				And.Contain(Move("f5, c2")).
				And.Contain(Move("f5, b1")).
				And.Contain(Move("f5, g4")).
				And.Contain(Move("f5, h3"));
		}
		// <summary>
		// Test must place a king in check, and validate the possible moves that result
		// Test allows BLACK'S bishop to put white's king in check  (BLACK)
		// </summary>
		[Fact]
		public void BishopCausingCheck() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"e7, e5",
				"d2, d3"
			);

			//move black's bishop so now it can have its sights on white's king
			Apply(b, "f8, b4");

			var possibleMoves = b.GetPossibleMoves();
			possibleMoves.Count().Should().Be(6, "There should be 6 possible moves to cancel the check");
			possibleMoves.Should().Contain(Move("d1, d2")) //move white queen up one
				.And.Contain(Move("e1, e2")) //move white king up one
				.And.Contain(Move("c1, d2")) //move white bishop up one and left one
				.And.Contain(Move("b1, d2")) //move knight to directly block the king
				.And.Contain(Move("c2, c3")) //move white pawn up one to block bishop across
				.And.Contain(Move("b1, c3")); //move knight to directly across from attacking bishop  

			b.IsCheck.Should().BeTrue("white's king is now in check from black's bishop");
		}

		// <summary>
		//Test involving validating GetPossibleMoves to see that a particular piece has correctly
		//reported all its possible moves
		//TEST BISHOPS (WITH BLACK)
		// </summary>
		[Fact]
		public void ValidateBishopMoves() {
			ChessBoard b = CreateBoardFromMoves(
				"c2, c4",
				"e7, e5",
				"d2, d4",
				"b7, b6"
				);

			Apply(b, "e2, e4", "c8, a6", "a2, a3");

			var possMoves = b.GetPossibleMoves();

			var blackBishop1 = GetMovesAtPosition(possMoves, Pos("a6"));

			blackBishop1.Count().Should().Be(4, "Left black bishop can move forward diagonally across " +
				"2 squares (capturing a white pawn) or it can move back diagonally 2 squares ");
			blackBishop1.Should().Contain
				(Move("a6, b5")) //move into empty space
				.And.Contain(Move("a6, b7")) //move into empty space
				.And.Contain(Move("a6, c8")) //move back to original starting pos
				.And.Contain(Move("a6,c4"));

			var blackBishop2 = GetMovesAtPosition(possMoves, Pos("f8"));

			blackBishop2.Should().HaveCount(5, "Right black bishop can move across diagonally to the left " +
				"up to 5 spaces")
				.And.Contain(Move("f8, e7"))
				.And.Contain(Move("f8, d6"))
				.And.Contain(Move("f8, c5"))
				.And.Contain(Move("f8, b4"))
				.And.Contain(Move("f8, a3"));
		}

		/// <summary>
		/// Undoing a simple move. Tests white bishop.
		/// </summary>
		[Fact]
		public void UndoBishopMove() {
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",
				"d7, d5",
				"c1, f4",  //Move white bishop from c1 to f4
				"c7, c5",
				"f4, c7"  //move white bishop from f4 to c7 to take black pawn spot
			);

			var whiteBishop = b.GetPieceAtPosition(Pos("c7"));
			whiteBishop.Player.Should().Be(1, "There should be a White piece at c7.");
			whiteBishop.PieceType.Should().Be(ChessPieceType.Bishop, "There should be a White Bishop at c7.");


			b.UndoLastMove();
			var oldWhiteBishop = b.GetPieceAtPosition(Pos("c7"));
			oldWhiteBishop.PieceType.Should().Be(ChessPieceType.Empty, "Move Undone. There should be an empty space at c7.");

			whiteBishop = b.GetPieceAtPosition(Pos("f4"));
			whiteBishop.PieceType.Should().Be(ChessPieceType.Bishop, "Move Undone. The White Bishop is back at its spot at f4.");
			whiteBishop.Player.Should().Be(1, "The piece at f4 should be player 1's (white)");

		}
		/// <summary>
		/// Validates bishop cannot move with pieces blocking
		/// </summary>
		[Fact]
		public void BishopMovement() {
			ChessBoard b = new ChessBoard();

			var possMoves = b.GetPossibleMoves();
			var blockedBishop = GetMovesAtPosition(possMoves, Pos("f1"));
			blockedBishop.Should().BeEmpty("Bishop should be blocked by own player's pawns");
			Apply(b, Move("g2, g4")); // moves white pawn
			Apply(b, Move("g8, f6")); // moves black knight
			possMoves = b.GetPossibleMoves();
			blockedBishop = GetMovesAtPosition(possMoves, Pos("f1"));
			blockedBishop.Should().HaveCount(2, "Bishop should be able to move to g2 or h3");
		}

		/// <summary>
		/// Bishop diagonal capture
		/// </summary>
		[Fact]
		public void BishopCapture() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"g8, f6",
				"f1, d3",
				"f6, e4" // black knight captures white pawn
			);

			// Current advantage
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "White lost a single pawn of 1 value");

			Apply(b, Move("d3, e4")); // white bishop captures black knight
			b.GetPieceAtPosition(Pos("e4")).Player.Should().Be(1, "Player 1 captured Player 2's knight diagonally");
			b.CurrentAdvantage.Should().Be(Advantage(1, 2), "Black lost a single knight of 3 value");

			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "after undoing the knight capture, advantage returned to (0, 1) where Player 2 +1");
		}

		/// <summary>
		/// Test possible moves for two Bishops belonging to White Player
		/// </summary>
		[Fact]
		public void WhiteBishopTest() {
			List<Tuple<BoardPosition, ChessPiece>> startingPositions = new List<Tuple<BoardPosition, ChessPiece>>();

			// White Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 4), new ChessPiece(ChessPieceType.King, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(3, 1), new ChessPiece(ChessPieceType.Bishop, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(3, 6), new ChessPiece(ChessPieceType.Bishop, 1)));

			// Black Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(0, 4), new ChessPiece(ChessPieceType.King, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 5), new ChessPiece(ChessPieceType.Queen, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 0), new ChessPiece(ChessPieceType.Rook, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 2), new ChessPiece(ChessPieceType.Knight, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 7), new ChessPiece(ChessPieceType.Bishop, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(4, 0), new ChessPiece(ChessPieceType.Pawn, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(4, 2), new ChessPiece(ChessPieceType.Pawn, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(4, 5), new ChessPiece(ChessPieceType.Pawn, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(4, 7), new ChessPiece(ChessPieceType.Pawn, 2)));

			ChessBoard b = new ChessBoard(startingPositions);

			var possMoves = b.GetPossibleMoves();

			var bishopOneMoves = GetMovesAtPosition(possMoves, Pos("b5"));
			var bishopTwoMoves = GetMovesAtPosition(possMoves, Pos("g5"));

			bishopOneMoves.Should().HaveCount(4, "Bishop at B5 (White Space) has Four Possible Capture Moves in Total")
				 .And.Contain(Move("b5, a6"), "Bishop (b5) takes Rook (a6)")
				 .And.Contain(Move("b5, c6"), "Bishop (b5) takes Knight (a6)")
				 .And.Contain(Move("b5, a4"), "Bishop (b5) takes Pawn (a4)")
				 .And.Contain(Move("b5, c4"), "Bishop (b5) takes Pawn (c4)");

			bishopTwoMoves.Should().HaveCount(4, "Bishop at G5 has Four Possible Capture Moves in Total")
				 .And.Contain(Move("g5, f6"), "Bishop (g5) takes Queen (f6)")
				 .And.Contain(Move("g5, h6"), "Bishop (g5) takes Bishop (h6)")
				 .And.Contain(Move("g5, f4"), "Bishop (g5) takes Pawn (f4)")
				 .And.Contain(Move("g5, h4"), "Bishop (g5) takes Pawn (h4)");
		}

		[Fact]

		public void WhiteUndoMove() {
			ChessBoard board = CreateBoardFromMoves(
				"e2, e4",
				"e7, e5",
				"d2, d4",
				"d7, d6",
				"c2, c3",
				"h7, h5",
				"d4, e5",
				"c8, f5",
				"g2, g3",
				"f5, e4",
				"f1, g2",
				"e4, g2",
				"g1, h3",
				"g2, h1"
			);

			board.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Bishop, "Black's bishop took rook on h1");
			board.GetPieceAtPosition(Pos("h1")).Player.Should().Be(2, "Black's bishop is at position h1");
			board.CurrentAdvantage.Should().Be(Advantage(2, 8), "Black has a bishop and rook advantage");

			board.UndoLastMove();
			board.UndoLastMove();
			board.UndoLastMove();

			board.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Rook, "White's rook has not been moved or captured");
			board.GetPieceAtPosition(Pos("g2")).PieceType.Should().Be(ChessPieceType.Bishop, "White's bishop has not been captured at g2");
			board.GetPieceAtPosition(Pos("g2")).Player.Should().Be(1, "The bishop on g2 should belong to white");
			board.GetPieceAtPosition(Pos("e4")).PieceType.Should().Be(ChessPieceType.Bishop, "The piece at e4 should be a black bishop");
			board.GetPieceAtPosition(Pos("e4")).Player.Should().Be(2, "The bishop at e4 should belong to black");
			board.CurrentAdvantage.Should().Be(Advantage(0, 0), "There should currently be no advantage on the board");
		}

		/// <summary>
		/// Testing bishop possible moves
		/// </summary>
		[Fact]
		public void BishopPossibleMoves2() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"d7, d5",
				"b2, b4"
			);
			var possMoves = b.GetPossibleMoves();
			var bishopMoves = GetMovesAtPosition(possMoves, Pos("c8"));
			bishopMoves.Should().HaveCount(5, "kight can move in one diagonal from b8")
				.And.Contain(Move("c8, d7"))
				.And.Contain(Move("c8, e6"))
				.And.Contain(Move("c8, f5"))
				.And.Contain(Move("c8, g4"))
				.And.Contain(Move("c8, h3"));
		}

		/// <summary>
		/// Check a lot of different possible moves for bishop (player 1 and 2)
		/// </summary>
		[Fact]
		public void checkBishopPossibleMoves() {
			ChessBoard b = new ChessBoard();

			var bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("c1"));
			bishopMoves.Should().HaveCount(0, "there is no possible move at start for p1's bishop");
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("f1"));
			bishopMoves.Should().HaveCount(0, "there is no possible move at start for p1's bishop");

			Apply(b, "b2, b3"); // one space pawn's move from p1
			Apply(b, "e7, e5"); // two space pawn's move from p2
			Apply(b, "d2, d3"); // one space pawn's move from p1
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("f8"));
			bishopMoves.Should().HaveCount(5, "there is only 5 possible moves for bishop at this stage of game")
				.And.OnlyContain(m => m.MoveType == ChessMoveType.Normal);

			Apply(b, "f8, d6"); // two space bishop's move from p2
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("d6"));
			bishopMoves.Should().HaveCount(0, "there is only 0 possible moves for p2's bishop at this stage of game, because it's not p2's turn.");

			Apply(b, "c1, a3"); // two space bishop's move from p1
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("d6"));
			bishopMoves.Should().HaveCount(5, "there is only 5 possible moves for p2's bishop at this stage of game.");
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("c3"));
			bishopMoves.Should().HaveCount(0, "there is only 0 possible moves for p1's bishop at this stage of game, because it's not p1's turn.");

			Apply(b, "d6, a3"); // Take p1's bishop with p2's bishop
			Apply(b, "g2, g3"); // one space move from p1's pawn
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("a3"));
			bishopMoves.Should().HaveCount(7, "there is only 07possible moves for p2's bishop at this stage of game.");

			Apply(b, "a3, c1"); // move p2's bishop
			Apply(b, "f1, h3"); // move p1's bishop
			Apply(b, "h7, h6"); // move p2's pawn
			Apply(b, "h3, f5"); // move p1's bishop
			Apply(b, "d7, d6"); // move p2's pawn
			Apply(b, "d3, d4"); // move p1's pawn
			Apply(b, "g7, g6"); // move p2's pawn
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("f5"));
			bishopMoves.Should().HaveCount(8, "there is only 8 possible moves for p1's bishop at this stage of game.");
		}

		///<summary>
		/// Attempts a white bishop to capture black queen
		///</summary>
		[Fact]
		public void BlackQueenCapture() {
			ChessBoard cb = CreateBoardFromPositions(
				 Pos(1, 4), ChessPieceType.Queen, 2, //black queen to be captured
				 Pos(2, 3), ChessPieceType.Bishop, 1,
				 Pos(2, 6), ChessPieceType.King, 2,
				 Pos(5, 6), ChessPieceType.Bishop, 2, //white bishop to capture
				 Pos(6, 1), ChessPieceType.Rook, 1,
				 Pos(6, 4), ChessPieceType.King, 1);

			//check if queen can be captured
			var poss = cb.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos(2, 3));
			expected.Should().Contain(Move("d6, e7")).And.HaveCount(2, "Bishop can take Queen or not take Queen");

			//capture queen
			Apply(cb, Move("d6, e7"));
			cb.GetPieceAtPosition(Pos("e7")).Player.Should().Be(1, "P1 Bishop captured P2 Queen diagonally");
		}

		///<summary>
		/// Checks possibles moves for black bishop
		///</summary>
		[Fact]
		public void MovesForBlackBishop() {
			ChessBoard cb = CreateBoardFromPositions(
				 Pos(1, 1), ChessPieceType.King, 2,
				 Pos(1, 5), ChessPieceType.Rook, 2,
				 Pos(2, 4), ChessPieceType.Bishop, 2, //black bishop to move
				 Pos(4, 2), ChessPieceType.Rook, 1,
				 Pos(3, 5), ChessPieceType.Pawn, 1,
				 Pos(6, 5), ChessPieceType.King, 1);

			//check all positions for which a black bishop can move
			var moves = cb.GetPossibleMoves();
			var expected = GetMovesAtPosition(moves, Pos(2, 4));
			expected.Should().BeEmpty("Black Bishop cannot move");
			expected.Should().NotContain(Move("e6, c4")).And.HaveCount(0, "Black Bishop cannot take White Rook");
		}

		[Fact]
		public void BishopPossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(
					  "d2, d3",
					  "g8, f6", // black knight 
					  "b2, b4",
					  "c7, c5",
					  "c1, b2",
					  "e7, e6",
					  "b2, e5", // white bishop
					  "g7, g5");

			var possMoves = b.GetPossibleMoves();
			var whiteBishop = GetMovesAtPosition(possMoves, Pos("e5"));
			whiteBishop.Should().HaveCount(9, "White Bishop should have 9 moves, two of them should capture one of the black's knights")
			.And.Contain(Move("e5,d4")).And.Contain(Move("e5,c3")).And.Contain(Move("e5,b2"))
			.And.Contain(Move("e5,f6")).And.Contain(Move("e5,g3")).And.Contain(Move("e5,f6"))
			.And.Contain(Move("e5,d6")).And.Contain(Move("e5,c7")).And.Contain(Move("e5,b8"));
		}
	}
}
