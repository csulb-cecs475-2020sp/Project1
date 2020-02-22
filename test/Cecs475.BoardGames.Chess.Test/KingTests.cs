using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class KingTests : ChessTest {
		/// <summary>
		/// Test Case #6
		/// Checks all possible moves for the King when in Check
		/// </summary>

		[Fact]
		public void PossibleMovesInCheck() {
			ChessBoard board = CreateBoardFromMoves(
				 Move("f2,f3"),
				 Move("e7,e6"),
			Move("h2,h3"),
			Move("f8,c5"),
			Move("h3,h4"),
			Move("c5,f2"));
			var allPossibleMoves = GetMovesAtPosition(board.GetPossibleMoves(), Pos("e1"));
			allPossibleMoves.Should().HaveCount(1, "These are not the possible moves for the king in check position");
		}

		/// <summary>
		/// Test Case #2
		/// Moving the King to castle
		/// </summary>

		[Fact]
		public void CastlingTest() {
			ChessBoard board = CreateBoardFromPositions(
				Pos("a1"), ChessPieceType.Rook, 1,
				Pos("h1"), ChessPieceType.Rook, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("c8"), ChessPieceType.King, 2
			);

			board.IsCheckmate.Should().BeFalse("the king has an escape");
			Apply(board, "e1,g1");
			board.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.King, "There should be a " + ChessPieceType.King.ToString() + " at position " + Pos("g1").ToString());
			board.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.Rook, "There should be a " + ChessPieceType.Rook.ToString() + " at position " + Pos("f1").ToString());
		}

		//check possible moves for a white king that's in check
		[Fact]
		public void checkWhiteKing() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"d7, d5",
				"e1, e2",
				"d8, d6",
				"e2, e3",//moves white king up by 1 into check position
				"d6, f4"
				);
			b.GetPieceAtPosition(Pos("e3")).PieceType.Should().Be(ChessPieceType.King, "the white king should be in check");
			b.GetPieceAtPosition(Pos("e3")).Player.Should().Be(1, "the king is controlled by player 1");
			b.IsCheck.Should().BeTrue("the king is threatened by the black queen controlled at f4");
			b.IsCheckmate.Should().BeFalse("the king has an escape");

			var possible = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(possible, Pos("e3"));
			kingMoves.Count().Should().Be(4, "There should be 4 possible moves to move the white king");
			kingMoves.Should().Contain(Move("e3, d4")).And.Contain(Move("e3, d3")).And.
				Contain(Move("e3, e2")).And.Contain(Move("e3, f4")).And.
				HaveCount(4, "a white king can capture diagonally ahead or go back or go the left in either d3 or d4");
		}

		/// <summary>
		/// Checks that Black King is in check and the possible moves that can remove it from Check
		/// </summary>
		[Fact]
		public void BlackKingInCheck2() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("d2"), ChessPieceType.King, 1,
				Pos("e7"), ChessPieceType.King, 2,
				Pos("g8"), ChessPieceType.Knight, 2,
				Pos("a6"), ChessPieceType.Rook, 1,
				Pos("g5"), ChessPieceType.Knight, 1,
				Pos("e5"), ChessPieceType.Bishop, 1
				);
			Apply(b, "e5, f6");//move Bishop to place Black King in check

			var possMove = b.GetPossibleMoves();
			var KnightsMove = GetMovesAtPosition(possMove, Pos("g8"));
			var KingsMove = GetMovesAtPosition(possMove, Pos("e7"));
			b.IsCheck.Should().BeTrue("Black King is in check by enemy Bishop");
			b.IsCheckmate.Should().BeFalse("The knight has to capture the Bishop or the King has to move");
			KingsMove.Should().HaveCount(3, "Black King can only make three moves away and cannot capture the bishop")
				.And.Contain(Move("e7,e8"))
				.And.Contain(Move("e7,d7"))
				.And.Contain(Move("e7,f8"));
			KnightsMove.Should().HaveCount(1, "Knight has to capture the Bishop")
				.And.Contain(Move("g8,f6"));

		}

		// In this test case, the king is in a corner (a1) and the rows 1 and 3 are controlled by two rooks, just like the columns b and c. For this
		//reason, the king is in check and the only possible solutions is to move one square up (a2)
		[Fact]
		public void KingInCheckTest2() {
			ChessBoard b = CreateBoardFromPositions(
					Pos("a1"), ChessPieceType.King, 1,
					Pos("a8"), ChessPieceType.King, 2,
					Pos("b3"), ChessPieceType.Rook, 2,
					Pos("c1"), ChessPieceType.Rook, 2
			);

			b.IsCheck.Should().BeTrue("At this moment the king should be in check, and there should be just one possible move to do");

			IEnumerable<ChessMove> possMoves = b.GetPossibleMoves();
			IEnumerable<ChessMove> surroundedQueen = GetMovesAtPosition(possMoves, Pos("a1"));
			surroundedQueen.Should().HaveCount(1, "the count should be one because all the other possible moves" +
				" would result in the king being captured");
		}
		///<summary>
		///Placing the black king in check and checking the possible moves
		///</summary>

		[Fact]
		public void BlackKingInCheck() {
			ChessBoard b = CreateBoardFromMoves(
				"c2, c3",
				"d7,d6",
				"e2,e3",
				"f7,f5",
				"f2,f3",
				"e7,e6",
				"g2,g3"
			);

			b.CurrentPlayer.Should().Be(2, "It is black's turn");

			//This is black's turn 
			//Black King is open and has 3 avilable move choices
			var posMoves = b.GetPossibleMoves();
			var checkPosMoves = GetMovesAtPosition(posMoves, Pos("e8"));
			checkPosMoves.Should().HaveCount(3, "one down, one diaganol right, one diaganol left");

			//Apply black's turn 
			Apply(b, "g7,g6");

			//Apply white's move- put the black king in check
			Apply(b, "d1,a4");

			b.IsCheck.Should().BeTrue("The black king is in check");

			posMoves = b.GetPossibleMoves();
			checkPosMoves = GetMovesAtPosition(posMoves, Pos("e8"));
			checkPosMoves.Should().HaveCount(2, "one down, one diaganol right");
		}


		///<summary>
		///Placing the white king in check 
		///</summary>
		[Fact]
		public void WhiteKingInCheck() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e3",
				"f7,f6",
				"f2,f4",
				"c7,c6",
				"d2,d4",
				"g7,g6"

			);

			b.CurrentPlayer.Should().Be(1, "It is white's turn");

			var posMoves = b.GetPossibleMoves();
			var checkPosMoves = GetMovesAtPosition(posMoves, Pos("e1"));
			checkPosMoves.Should().HaveCount(3, "one diagonal left, one diaganol right, one forward")
				.And.Contain(Move("e1, d2"), "one diagonal left")
				.And.Contain(Move("e1, e2"), "one forward")
				.And.Contain(Move("e1, f2"), "one diagonal right");

			//Apply white's move
			Apply(b, "g2,g3");

			//Apply black's move- put white's king in check 
			Apply(b, "d8,a5");


			b.IsCheck.Should().BeTrue("The white king is in check");


			//Check possible move for white's king 
			posMoves = b.GetPossibleMoves();
			checkPosMoves = GetMovesAtPosition(posMoves, Pos("e1"));
			checkPosMoves.Should().HaveCount(2, "one diaganol right, one forward")
				.And.NotContain(Move("e1, d2"), "one diagonal left")
				.And.Contain(Move("e1, e2"), "one forward")
				.And.Contain(Move("e1, f2"), "one diagonal right");

		}


		/// <summary>
		/// Check all possible moves of the black king piece at the start of the game and during the proceedings of a game. Verify that black king has possible
		/// moves left when in check
		/// </summary>
		[Fact]
		public void CheckKing() {
			ChessBoard chess = CreateBoardFromMoves("d2, d4", "e7, e6", "d1, d2");
			var moves = chess.GetPossibleMoves();
			var kingMove = GetMovesAtPosition(moves, Pos("e8"));
			kingMove.Should().HaveCount(1, "Black king should have 1 possible move since pawn in front moved");
			Apply(chess, Move(Pos("e8"), Pos("e7")));
			moves = chess.GetPossibleMoves();
			kingMove = GetMovesAtPosition(moves, Pos("e7"));
			kingMove.Should().HaveCount(0, "Black king should have 0 moves since it is the white player's turn");
			Apply(chess, Move(Pos("d4"), Pos("d5")));
			moves = chess.GetPossibleMoves();
			kingMove = GetMovesAtPosition(moves, Pos("e7"));
			kingMove.Should().HaveCount(3, "Black king should have 3 moves : diagonally to the left, diagonally to the right, and back to its original position");
			Apply(chess, Move(Pos("h7"), Pos("h5")));
			Apply(chess, Move(Pos("d2"), Pos("g5")));
			moves = chess.GetPossibleMoves();
			kingMove = GetMovesAtPosition(moves, Pos("e7"));
			kingMove.Should().HaveCount(2, "Black king should have 2 moves : diagonally to the left,and back to its original position. Cannot move to the right since it is now in check");
			chess.IsCheck.Should().Be(true, "Black king shoud be in check");
			chess.IsCheckmate.Should().Be(false, "Black king shoud not be in checkmate");
		}
		/// <summary>
		/// Ensure that all possible moves are valid when white King is in check
		/// </summary>
		[Fact]
		public void KingInCheckTest() {
			ChessBoard b = CreateBoardFromMoves(
					 "d2, d4",
					 "e7 ,e5",
					 "a2, a3",
					 "f8, b4" // Bishop puts white King in check
				);
			// Get white players possible moves with King in check
			var WhitePossMoves = b.GetPossibleMoves();

			// Ensure all possible moves for white player are valid when King is in check
			WhitePossMoves.Should().HaveCount(6, "Pawn at a3, Pawn at c2, Knight at b1, Bishop at c1, Queen at d1 can save King")
				.And.BeEquivalentTo(Move("a3, b4"), Move("c2, c3"), Move("b1, c3"), Move("b1, d2"), Move("c1, d2"), Move("d1, d2"));
		}

		/// <summary>
		/// Ensure that Kingside castling works properly on the black side
		/// </summary>
		[Fact]
		public void KingsideCastlingTest() {
			// Chess board setup for a Kingside castle on the black side
			ChessBoard b = CreateBoardFromMoves(
					 "a2, a3",
					 "e7, e6",
					 "a3, a4",
					 "f8, e7",
					 "a4, a5",
					 "g8, f6",
					 "a5, a6"
			);

			var PossMoves = b.GetPossibleMoves();
			var KingPossMoves = GetMovesAtPosition(PossMoves, Pos("e8"));
			KingPossMoves.Should().HaveCount(2, "King at e8 can move to f8 or castle to g8")
				.And.BeEquivalentTo(Move("e8, f8"), Move("e8, g8"));

			// Apply Kingside castle move
			Apply(b, "e8, g8");

			// Ensure that the Rook and King made it to the correct spots after a castle move
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.King, "the King should be at g8 after successful castle");
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Rook, "the Rook should be at f8 after successful castle");
		}

		/// <summary>
		/// Ensure that Queenside castling works properly on white side
		/// </summary>
		[Fact]
		public void QueensideCastlingTest() {
			// Chess board setup for a Queenside castle
			ChessBoard b = CreateBoardFromMoves(
				"b2, b4",
				"h7, h6",
				"c2, c3",
				"h6, h5",
				"b1, a3",
				"h5, h4",
				"c1, b2",
				"h4, h3",
				"d1, c2",
				"g7, g6"
			);

			var PossMoves = b.GetPossibleMoves();
			var KingPossMoves = GetMovesAtPosition(PossMoves, Pos("e1"));
			KingPossMoves.Should().HaveCount(2, "King at e1 can move to d1 or castle to c1")
				.And.BeEquivalentTo(Move("e1, d1"), Move("e1, c1"));

			// Apply Queenside castle move
			Apply(b, "e1, c1");

			// Ensure that the Rook and King made it to the correct spots after a castle move
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.King, "the King should be at c1 after successful castle");
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Rook, "the Rook should be at d1 after successful castle");
		}

		// Check that a black king can castle correctly
		[Fact]
		public void validBlackCastling() {
			ChessBoard cb = CreateBoardFromPositions(
				Pos("e8"), ChessPieceType.King, 2,
				Pos("h8"), ChessPieceType.Rook, 2,
				Pos("f7"), ChessPieceType.Pawn, 2,
				Pos("a1"), ChessPieceType.Pawn, 1,
				Pos("e1"), ChessPieceType.King, 1
			);

			Apply(cb, "a1, a2");  // black's turn
			var possMoves = cb.GetPossibleMoves();
			possMoves.Should().Contain(Move("e8, g8"), "the black king can castle");
			Apply(cb, "e8, g8");  // castle
			cb.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Rook, "rook should move appropriately" +
				" after castle");
		}

		// Place the black king in check, escape, undo, and go back into check
		// Check various aspects of board before and after undo
		[Fact]
		public void checkBlackKingUndoCheck() {
			ChessBoard cb = CreateBoardFromMoves(
					"e2, e4",
					"d7, d5",
					"e4, d5",
					"d8, d6",
					"d2, d3",
					"d6, c5",
					"d5, d6",
					"c7, c6",
					"d6, d7"  // white pawn puts black king in check
			);

			cb.CurrentPlayer.Should().Be(2, "black king is in check");
			cb.IsCheck.Should().BeTrue("the black king is threatened by white pawn at d7");
			var possMoves = cb.GetPossibleMoves();
			possMoves.Should().HaveCount(4, "black king must move out of check");
			Apply(cb, Move("(e8, d8)"));  // black king escapes check
			cb.IsCheck.Should().BeFalse("the black king is no longer threatened");
			cb.CurrentAdvantage.Should().Be(Advantage(1, 1), "a black pawn was captured");
			possMoves = cb.GetPossibleMoves();
			possMoves.Should().HaveCount(36, "white should be able to move in 36 different ways");

			// undo and test
			cb.UndoLastMove();
			cb.IsCheck.Should().BeTrue("the black king is back in check after undoing the last move");
			cb.CurrentAdvantage.Should().Be(Advantage(1, 1), "advantage should stay the same");
			cb.CurrentPlayer.Should().Be(2, "black's turn after undo move");
			cb.GetPieceAtPosition(Pos("d7")).PieceType.Should().Be(ChessPieceType.Pawn, "the pawn does not move");
			cb.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "the king moves back after undo");
			possMoves = cb.GetPossibleMoves();
			possMoves.Should().HaveCount(4, "black king can move 2 places, bishop can move 1, knight can move 1");
		}

		[Fact]
		// places a king in check, then validates all possible moves
		public void BlackCheck() {
			// referenced game link: https://lichess.org/ggkFK1GN#8
			ChessBoard b = CreateBoardFromMoves(
				 "d2, d4",
				 "b7, b5",
				 "c1, g5",
				 "g7, g6",
				 "b1, c3",
				 "b5, b4",
				 "c3, b5",
				 "a7, a6",
				 "b5, c7"
			);

			b.IsCheck.Should().BeTrue("Black king should be in check");
			// validate all possible moves
			// the only move should be that the black queen eats the white horsie
			var possMoves = b.GetPossibleMoves();
			var queenEatHorse = GetMovesAtPosition(possMoves, Pos("d8"));
			queenEatHorse.Should().Contain(Move("d8, c7")).And.HaveCount(1, "The queen is blocked everywhere except where the white horse is");
		}

		/// <summary>
		/// Verify that king must escape check
		/// </summary>
		[Fact]
		public void EscapeCheck() {
			// Place black king in check
			ChessBoard b = CreateBoardFromMoves(
				"g2, g3",
				"a7, a6",
				"f1, h3",
				"b7, b6",
				"h3, f5",
				"f7, f6",
				"g3, g4",
				"e7, e6",
				"f5, g6"
			);

			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(2, "King is in check and must escape");
			var kingPossibleMoves = GetMovesAtPosition(possMoves, Pos("e8"));
			kingPossibleMoves.Should().HaveCount(1).And.Contain(Move("e8, e7"), "King has 1 move, can escape check");
			var pawnPossibleMoves = GetMovesAtPosition(possMoves, Pos("h7"));
			pawnPossibleMoves.Should().HaveCount(1).And.Contain(Move("h7, g6"), "Pawn has 1 move and can take bishop, escaping check");
		}

		/// <summary>
		/// No castling if rook has moved
		/// </summary>
		[Fact]
		public void CannotCastleIfRookHasMoved() {
			// Move black pieces out of the way to allow king side castling
			ChessBoard b = CreateBoardFromMoves(
				"a2, a3",
				"h7, h6",
				"b2, b3",
				"g7, g6",
				"c2, c3",
				"g8, f6",
				"d2, d3",
				"f8, g7",
				"e2, e3"
			);

			var possMoves = b.GetPossibleMoves();
			var castlingPossible = GetMovesAtPosition(possMoves, Pos("e8"));
			castlingPossible.Should().Contain(Move("e8, g8"), "Castling possible because king side rook hasn't moved");

			//Moving king side rook forward and back to make king side castling illegal
			Apply(b, Move("h8, h7"));
			Apply(b, Move("f2, f3"));
			Apply(b, Move("h7, h8"));
			Apply(b, Move("g2, g3"));
			possMoves = b.GetPossibleMoves();
			var castlingImpossible = GetMovesAtPosition(possMoves, Pos("e8"));
			castlingImpossible.Should().NotContain(Move("e8, g8"), "Castling impossible because king side rook has moved");
		}
		/// <summary>
		/// Undo rook move re-enables castling
		/// </summary>
		[Fact]
		public void UndoReEnablesCastling() {
			// Move black pieces out of the way for king side castling
			ChessBoard b = CreateBoardFromMoves(
				"a2, a3",
				"h7, h6",
				"b2, b3",
				"g7, g6",
				"c2, c3",
				"g8, f6",
				"d2, d3",
				"f8, g7",
				"e2, e3",
				"h8, h7", // Move rook forward and back to disable castling
				"f2, f3",
				"h7, h8",
				"g2, g3"
			);

			var possMoves = b.GetPossibleMoves();
			var castlingImpossible = GetMovesAtPosition(possMoves, Pos("e8"));
			castlingImpossible.Should().NotContain(Move("e8, g8"), "Castling impossible because king side rook has moved");

			// Undoing last 4 moves to re-enable castling
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			possMoves = b.GetPossibleMoves();
			var castlingPossible = GetMovesAtPosition(possMoves, Pos("e8"));
			castlingPossible.Should().Contain(Move("e8, g8"), "Undo re-enables castling");
			castlingPossible = GetMovesAtPosition(possMoves, Pos("h8"));
			castlingPossible.Should().Contain(Move("h8, f8"), "Undo re-enables castling");
		}

		/// <summary>
		/// Queens can place enemy in check and enemy can get themselves out of check with possible moves
		/// </summary>
		[Fact]
		public void WhiteQueenCausingCheckAndValidMoves() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e3",
					 "e7, e5",
					 "d1, h5",
					 "e8, e7"
				);


			// Move queen to g5 so it's black's turn, putting king in e6 in check
			Apply(b, "h5, g5");
			var possible = b.GetPossibleMoves();
			b.IsCheck.Should().BeTrue("black's king is in check from White Queen at g5");
			possible.Count().Should().Be(5, "There should be 5 possible moves to cancel the checking of black's king");
			possible.Should().Contain(Move("e7, e6")).And.Contain(Move("e7, d6")).And.Contain(Move("e7, e8"))
				 .And.Contain(Move("f7, f6")).And.Contain(Move("g8, f6"), "the king cannot move into check, 3 moves from king, one from pawn f7, and one from knight g8");
		}


		[Fact]
		//This checks if the white king is in check by the queen
		public void CheckWhiteKingTest() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("e2"), ChessPieceType.Queen, 2,
				Pos("b8"), ChessPieceType.King, 2,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("a2"), ChessPieceType.Pawn, 1);

			var possMoves = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			b.IsCheck.Should().BeTrue("White King should be in check from the queen at e1");
			kingMoves.Should().HaveCount(1, "The white king's only move is to capture the black queen");
		}

		[Fact]
		public void KingCheckTest() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"d7, d6",
				"f1, b5"

			);
			b.IsCheck.Should().BeTrue("Black king is threatened by white bishop at b5");
			b.IsCheckmate.Should().BeFalse("Black king can escape");
			var possible = b.GetPossibleMoves();
			possible.Count().Should().Be(5, "There should be 5 possible moves to cancel the checking of black's king");
			possible.Should().Contain(Move("d8, d7")).And.Contain(Move("c8, d7")).And.Contain(Move("b8, d7")).And.Contain(Move("b8, c6")).And.Contain(Move("c7, c6"), "5 moves to save king");
		}

		[Fact]
		public void KingInCheck3() {

			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"e7, e5",
				"d2, d3",
				"f8, b4"
			);
			b.IsCheck.Should().BeTrue("The king is now in check by the bishop");
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(6, "The King can move out of check through only six moves").And.BeEquivalentTo(
				Move("e1, e2"), Move("d1, d2"), Move("c2, c3"), Move("c1, d2"), Move("b1, c3"), Move("b1, d2"));

			//apply one of the available moves and the king should be out of check
			Apply(b, Move("c2, c3"));
			b.IsCheck.Should().BeFalse("The king in not in check anymore");
		}

		/// <summary>
		/// Ensure that kingside castling is still allowed (black player). 
		/// </summary>
		[Fact]
		public void KingsideCastling() {
			ChessBoard b = CreateBoardFromMoves(
				"g2, g3",
				"b7, b5",
				"a2, a4",
				"g7, g6",
				"f1, h3",
				"f8, g7", // move bishop out of the way
				"a4, b5", // white capture black pawn
				"g8, f6",  // move knight out of the way
				"b2, b3",
				"h7, h6",
				"c2, c4"
			);
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "a Black pawn was captured");

			var possMoves = b.GetPossibleMoves();
			var forKing = GetMovesAtPosition(possMoves, Pos("e8"));
			forKing.Should().HaveCount(2, "king at e8 can castle kingside")
				.And.BeEquivalentTo(Move("e8, f8"), Move("e8, g8"));

			var forRook = GetMovesAtPosition(possMoves, Pos("h8"));
			forRook.Should().HaveCount(3, "rook at e1 can castle kingside")
				.And.BeEquivalentTo(Move("h8, f8"), Move("h8, g8"), Move("h8, h7"));

			// Apply the castling move
			Apply(b, Move("(e8,g8)"));
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.King, "the empty space replaced by a black King");
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Rook, "the empty space replaced by a black Rook");

			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(2, "undoing kingside castling should change the current player");
			// Check that black king and rook are where they were before
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Rook, "original black Rook position");
			b.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "original black King position");
		}

		/// <summary>
		/// Check that a king at an edge in check by a rook can move three places 
		/// </summary>
		[Fact]
		public void KingInCheckMovement() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("c8"), ChessPieceType.King, 1,
				Pos("f8"), ChessPieceType.Rook, 2,
				Pos("e1"), ChessPieceType.King, 2
			);

			b.IsCheck.Should().BeTrue();
			var possMoves = b.GetPossibleMoves();
			var pos = Pos("c8");
			var movesAtPos = GetMovesAtPosition(possMoves, pos);
			movesAtPos.Should().HaveCount(3, "The king has three moves avaialble when on an edge threated by a rook")
		.And.BeEquivalentTo(
			Move(pos, pos.Translate(1, -1)),
			Move(pos, pos.Translate(1, 1)),
			Move(pos, pos.Translate(1, 0))
		);
		}

		[Fact]
		public void BishopKingCheck() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "d7, d6",
				 "f1, b5"
				 );
			//put black king in check
			var possibleMoves = b.GetPossibleMoves();
			var KingMoves = GetMovesAtPosition(possibleMoves, Pos("e8"));
			KingMoves.Should().BeEmpty("King cannot move while in check in this position");
			b.IsCheck.Should().BeTrue("The black king should be in check");
			b.IsCheckmate.Should().BeFalse("The black king should not be in checkmate");
		}


		[Fact]
		// moves the rook into position that puts the king in check and verifies the moves the king can use to get out of it
		public void check_kingInCheck() {
			ChessBoard b = CreateBoardFromMoves(
				"d2,d4",
				"e7,e5",
				"d1,d3",
				"e8,e7",
				"d3,g6",
				"d7,d6",
				"g6,d6"
			);

			b.IsCheck.Should().BeTrue("The king is now in check by the rook");
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(4, "The King can move out of check through 1 move").And.BeEquivalentTo(
				Move("e7, e8"), Move("e7,d6"), Move("d8, d6"), Move("c7,d6"));

			Apply(b, Move("e7, e8"));
			b.IsCheck.Should().BeFalse("The king in not in check");

			b.UndoLastMove();

			Apply(b, Move("e7, d6"));
			b.IsCheck.Should().BeFalse("The king in not in check");

			b.UndoLastMove();

			Apply(b, Move("d8, d6"));
			b.IsCheck.Should().BeFalse("The king in not in check");

			b.UndoLastMove();

			Apply(b, Move("c7, d6"));
			b.IsCheck.Should().BeFalse("The king in not in check");

		}

		[Fact]
		//Testing placing a king in check
		public void KingInCheck() {
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"e7, e6",
				"e2, e4"
			);

			//Apply the move to achieve check
			Apply(b, "d8, h4");

			//Check the allowed moves
			var posMoves = b.GetPossibleMoves();
			posMoves.Should().HaveCount(2, "king is in check and there are two moves to save the queen")
				.And.BeEquivalentTo(Move("e1, e2"), Move("g2, g3"));
			b.IsCheckmate.Should().Be(false, "it is check, queen can be saved");
			b.IsFinished.Should().Be(false, "it is check, queen can be saved");

			//Block the queen from getting captured with a pawn
			Apply(b, "g2, g3");

			//Capture the blocking pawn putting the king in check again
			Apply(b, "h4, g3");
			posMoves = b.GetPossibleMoves();
			posMoves.Should().HaveCount(2, "king is in check and there are two moves to save the queen")
				.And.BeEquivalentTo(Move("e1, e2"), Move("h2, g3"));
			b.IsCheckmate.Should().Be(false, "it is check, queen can be saved");
			b.IsFinished.Should().Be(false, "it is check, queen can be saved");

		}

		/// <summary>
		/// Checking the ways to get out of a check. Checks black putting white into check.
		/// </summary>
		[Fact]
		public void KingInCheck2() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"b8, c6",
				"e2, e4",
				"c6, b4",
				"c2, c4",
				"b4, d3"
			);
			var testCheck = b.IsCheck;
			testCheck.Should().BeTrue("The Black Knight at d3 has the White King in Check!");

			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(2, "There should only be two ways to get out of check.")
				.And.Contain(Move("e1, e2"))    //Moves the king up one spot and out of check
				.And.Contain(Move("f1, d3")); //Moves the White bishop at f1 to capture the Black Knight at d3
		}

		/// <summary>
		/// Validates that King is threatened in check
		/// </summary>
		[Fact]
		public void KingCheckThreats() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("d4"), ChessPieceType.King, 1,
				Pos("e8"), ChessPieceType.King, 2,
				Pos("d8"), ChessPieceType.Queen, 2,
				Pos("d5"), ChessPieceType.Pawn, 2,
				Pos("h8"), ChessPieceType.Bishop, 2,
				Pos("a4"), ChessPieceType.Rook, 2,
				Pos("b3"), ChessPieceType.Pawn, 1
			);

			b.PositionIsAttacked(Pos("d4"), 2).Should().BeTrue("Player 1's king should be threatened by Player 2's bishop (h8) and rook (a4)");
			b.IsCheck.Should().BeTrue("Player 1's king should be in check");
			b.IsCheckmate.Should().BeFalse("Player 1's king should not be in checkmate");

			Apply(b, Move("d4, d3")); // white king moves out of check
			b.PositionIsAttacked(Pos("d3"), 2).Should().BeFalse("Player 1's king should not be threatened");
			b.IsCheck.Should().BeFalse("Player 1's king should not be in check");
			b.IsCheckmate.Should().BeFalse("Player 1's king should not be in checkmate");

			b.UndoLastMove(); // white king returns to previous state
			b.PositionIsAttacked(Pos("d4"), 2).Should().BeTrue("Player 1's king should be threatened by Player 2's bishop (h8) and rook (a4)");
			b.IsCheck.Should().BeTrue("Player 1's king should be in check");
			b.IsCheckmate.Should().BeFalse("Player 1's king should not be in checkmate");

			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(3);
			b.IsCheck.Should().BeTrue("Player 1's king should be in check");
		}

		[Fact]
		public void KingInCheckScenarios() {
			ChessBoard cBoard = CreateBoardFromMoves(
				"d2, d4",
				"d7, d6",
				"b1, c3",
				"c8, f5",
				"c3, b5",
				"f7, f6"
			);

			// White player's turn
			cBoard.CurrentPlayer.Should().Be(1, "It's white player's turn");
			Apply(cBoard, "b5, c7");
			// Black player's turn
			cBoard.CurrentPlayer.Should().Be(2, "It's black player's turn");

			var possibleMoves = cBoard.GetPossibleMoves();
			var whiteKingPossibleMoves = GetMovesAtPosition(possibleMoves, Pos("e8"));

			cBoard.IsCheckmate.Should().BeFalse("Black king can escape to d7 or f7 ");
			cBoard.IsCheck.Should().BeTrue("");
			whiteKingPossibleMoves.Should().HaveCount(2, "").
				And.Contain(Move("e8, d7"), "").
				And.Contain(Move("e8, f7"), "");

			// Undo white knigth's last move
			cBoard.UndoLastMove();

			// Since last move was undone, then it is white player's turn again
			cBoard.CurrentPlayer.Should().Be(1, "It's white player's turn");
			Apply(cBoard, "b5, a7");

			// Black player's turn
			cBoard.CurrentPlayer.Should().Be(2, "It's black player's turn");
			cBoard.IsCheck.Should().BeFalse("");

			// Update possible moves after a play
			possibleMoves = cBoard.GetPossibleMoves();

			// Black rook at a8 can now attack white knight at a7, it is indeed his only possible move
			GetMovesAtPosition(possibleMoves, Pos("a8")).Should().HaveCount(1, "Black rook can attack white knight at a7");
			GetMovesAtPosition(possibleMoves, Pos("a8")).Should().Contain(Move("a8, a7"));
		}

		/// <summary>
		/// TestKingCastlingScenarios
		/// King castles King's side. It moves two positions to the right and the Rook comes to 
		/// stand on the position immediately next to the King on its opposite side
		/// Castling is not possible if either the King or the Rook has moved.
		/// All of the postions between the King and the Rook must be empty.
		/// </summary>
		[Fact]
		public void KingCastlingScenarios() {
			// ------------------------------------
			// TestKingSideCastlingScenario
			ChessBoard cBoard = CreateBoardFromPositions(
				Pos("a1"), ChessPieceType.Rook, 1,
				Pos("h1"), ChessPieceType.Rook, 1,
				Pos("g1"), ChessPieceType.Knight, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("c8"), ChessPieceType.Bishop, 2,
				Pos("d7"), ChessPieceType.King, 2
				);

			// Get Possible moves for White King
			var whiteKingPossibleMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("e1"));
			var whiteRookKingSidePossibleMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("h1"));

			whiteRookKingSidePossibleMoves.Should().NotContain(Move(Pos("h1"), Pos("f1"), ChessMoveType.CastleKingSide), "there is a knight between the rook and king");
			whiteKingPossibleMoves.Should().NotContain(Move(Pos("e1"), Pos("g1"), ChessMoveType.CastleKingSide), "there is a knight between the king and rook");

			// Remove the knight at g1 out of the way to allow castling
			Apply(cBoard, "g1, h3");
			Apply(cBoard, "d7, d8");

			// Check for positions between the King anf rook to be empty
			cBoard.GetPieceAtPosition(Pos("f1")).Player.Should().Be(0, "0 since this position is empty");
			cBoard.GetPieceAtPosition(Pos("g1")).Player.Should().Be(0, "0 since this position is empty");

			// Update both king and rook possible moves
			whiteKingPossibleMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("e1"));
			whiteRookKingSidePossibleMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("h1"));
			whiteRookKingSidePossibleMoves.Should().Contain(Move(Pos("h1"), Pos("f1"), ChessMoveType.CastleKingSide),
				"there is not an chess piece between the rook and king. Also the rook hasn't made a move yet");
			whiteKingPossibleMoves.Should().Contain(Move(Pos("e1"), Pos("g1"), ChessMoveType.CastleKingSide),
				"there is not an chess piece between the rook and king. Also the king hasn't made a move yet.");
		}

		[Fact]
		private void TestQueenSideCastling() {
			// ------------------------------------
			// TestQueenSideCastlingScenario
			ChessBoard cBoard = CreateBoardFromPositions(
				Pos("a8"), ChessPieceType.Rook, 2,
				Pos("h8"), ChessPieceType.Rook, 2,
				Pos("e8"), ChessPieceType.King, 2,
				Pos("c1"), ChessPieceType.Bishop, 1,
				Pos("d2"), ChessPieceType.King, 1
				);
			Apply(cBoard, "d2, d1");

			// Get Possible moves for Black King
			var blackKingPossibleMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("e8"));
			var blackRookQueenSidePossibleMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("a8"));

			// Check for positions between the King anf rook to be empty
			cBoard.GetPieceAtPosition(Pos("b8")).Player.Should().Be(0, "0 since this position is empty");
			cBoard.GetPieceAtPosition(Pos("c8")).Player.Should().Be(0, "0 since this position is empty");
			cBoard.GetPieceAtPosition(Pos("d8")).Player.Should().Be(0, "0 since this position is empty");

			// Black king will be allowed to move 2 positions to the left and rook queen side will be on the king's right side
			blackKingPossibleMoves.Should().Contain(Move(Pos("e8"), Pos("c8"), ChessMoveType.CastleKingSide), "there is not another chess piece between king and rook");
			blackRookQueenSidePossibleMoves.Should().Contain(Move(Pos("a8"), Pos("d8"), ChessMoveType.CastleKingSide), "there is not rook chess piece between king and king");

			// Relocate the pieces at these locations
			Apply(cBoard, "e8, d8");
			Apply(cBoard, "d1, d2");

			blackKingPossibleMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("d8"));
			blackKingPossibleMoves.Should().NotContain(Move(Pos("d8"), Pos("b8"), ChessMoveType.CastleKingSide),
				"the knight has moved from its originial position");

			// Undo last two moves on the board 
			cBoard.UndoLastMove();
			cBoard.UndoLastMove();

			// Update possible moves for the specified position
			blackKingPossibleMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("e8"));
			blackKingPossibleMoves.Should().Contain(Move(Pos("e8"), Pos("c8"), ChessMoveType.CastleKingSide), "there is not another chess piece between king and the rook");
			blackRookQueenSidePossibleMoves.Should().Contain(Move(Pos("a8"), Pos("d8"), ChessMoveType.CastleKingSide), "there is not rook chess piece between king and the king");
		}



	}
}
