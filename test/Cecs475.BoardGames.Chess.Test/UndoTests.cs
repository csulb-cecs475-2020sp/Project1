using Cecs475.BoardGames.Chess.Model;
using System.Linq;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class UndoTests : ChessTest {
		/// <summary>
		/// Test Case #4
		/// This test undoes Castle
		/// </summary>

		[Fact]
		public void UndoLastMoveTwo() {
			ChessBoard board = CreateBoardFromPositions(
				Pos("a1"), ChessPieceType.Rook, 1,
				Pos("h1"), ChessPieceType.Rook, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("c8"), ChessPieceType.King, 2
			);

			board.IsCheckmate.Should().BeFalse("the king has an escape");
			Apply(board, "e1,g1");
			board.UndoLastMove();
			board.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "There should be a " + ChessPieceType.King.ToString() + " at position " + Pos("e1").ToString());
			board.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Rook, "There should be a " + ChessPieceType.Rook.ToString() + " at position " + Pos("h1").ToString());
		}

		/// <summary>
		/// Test Case #3
		/// Moving Pawn by two spaces so that it can be captured and then undoing it
		/// </summary>

		[Fact]
		public void UndoLastMoveOne() {
			ChessBoard board = new ChessBoard();
			Apply(board, "c2,c4");
			Apply(board, "d7,d5");
			board.UndoLastMove();
			board.GetPieceAtPosition(Pos("d7")).PieceType.Should().Be(ChessPieceType.Pawn, "There should be a " + ChessPieceType.Pawn.ToString() + " at position " + Pos("d7").ToString());

		}
		//the following 2 test cases check for UndoLastMove
		//checks if the black piece returns to its original position
		[Fact]
		public void isBlackOriginalPosition() {
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",//move white piece out 2 spaces from in front of queen
				"c7, c5",//move black piece out 2 spaces from in front of bishop
				"e2, e3"//move white piece out 1 space from in front of king
				);
			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("c5"));
			expected.Should().Contain(Move("c5, d4"))//capture move on white pawn
				.And.Contain(Move("c5, c4"))//move forward 1
				.And.HaveCount(2, "a pawn can capture diagonally ahead or move forward");

			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no operations have changed the advantage");

			Apply(b, Move("c5, d4"));
			b.GetPieceAtPosition(Pos("d4")).Player.Should().Be(2, "Player 1 captured Player 2's white pawn diagonally");
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "White lost a single pawn of 1 value");

			b.UndoLastMove();

			b.GetPieceAtPosition(Pos("d4")).PieceType.Should().Be(ChessPieceType.Pawn, "The black pawn should be restored after undoing the capture");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "after undoing the pawn capture, advantage is neutral");
		}
		//checks if the white knight captured by the black knight previously is restored to its original state
		[Fact]
		public void isBlackCaptureRestored() {
			ChessBoard b = CreateBoardFromMoves(
				"b1, c3",//move white knight
				"g8, f6",//move black knight
				"c3, e4"//white knight in position to be captured
						  //black knight capture the white knight
				);
			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("f6"));
			expected.Should().Contain(Move("f6, e4"))//capture move on white pawn
				.And.Contain(Move("f6, d5")).And.Contain(Move("f6, g4")).And.Contain(Move("f6, h5")).And.Contain(Move("f6, g8"))//far left, inner right, far right, home position
				.And.HaveCount(5, "a black knight can capture or move to 4 other positions");

			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no operations have changed the advantage");

			Apply(b, Move("f6, e4"));
			b.GetPieceAtPosition(Pos("e4")).Player.Should().Be(2, "Player 2 captured Player 1's white knight");
			b.CurrentAdvantage.Should().Be(Advantage(2, 3), "White lost a single pawn of 1 value");

			b.UndoLastMove();

			b.GetPieceAtPosition(Pos("e4")).PieceType.Should().Be(ChessPieceType.Knight, "The white knight should be restored after undoing the capture");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "after undoing the knight capture, advantage is neutral");
		}

		/// <summary>
		/// checks that undoing the board will put the right pieces back on the board that were captured earlier
		/// </summary>
		[Fact]
		public void CheckUndoMove_CapturingPieces() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("h8"), ChessPieceType.King, 1,
				Pos("a1"), ChessPieceType.King, 2,
				Pos("g7"), ChessPieceType.Knight, 2,
				Pos("f5"), ChessPieceType.Rook, 1,
				Pos("e3"), ChessPieceType.Bishop, 1,
				Pos("c3"), ChessPieceType.Pawn, 1
				);

			Apply(b, "c3, c4");//move white pawn up 1

			Apply(b, "g7, f5");//black knight captures white rook

			Apply(b, "h8, h7");//move white King up 1

			Apply(b, "f5, e3");//Black knight captures white Bishop

			Apply(b, "h7, h6");//Move white King up 1

			Apply(b, "e3, c4");//Black Knight captures white pawn

			b.UndoLastMove(); //Undo Apply(b, "e3, c4");
			b.GetPieceAtPosition(Pos("c4")).PieceType.Should().Be(ChessPieceType.Pawn, "White's Pawn at position c4");
			b.UndoLastMove(); //Undo Apply(b, "h7, h6");
			b.UndoLastMove(); //Undo Apply(b, "f5, e3");
			b.GetPieceAtPosition(Pos("e3")).PieceType.Should().Be(ChessPieceType.Bishop, "White's Bishop at position e3");
			b.UndoLastMove(); //Undo Apply(b, "h8, h7");
			b.UndoLastMove(); //Undo Apply(b, "g7, f5");
			b.GetPieceAtPosition(Pos("f5")).PieceType.Should().Be(ChessPieceType.Rook, "White's Rook at position f5");

		}
		/// <summary>
		/// Checks that En Passant can happen before and after an undo
		/// </summary>
		[Fact]
		public void NoEnPassantTestwithUndos() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"h7, h5",
				"a4, a5"
			);

			// Move pawn forward twice, enabling en passant from a5
			Apply(b, "b7, b5");

			//moves a new different pawn to make bottom pass on the en passant
			Apply(b, "b2, b3");

			//makes top move the pawn on the side not involved in the en passant
			Apply(b, "h5, h4");

			var possMoves = b.GetPossibleMoves();
			var noEnPassant = GetMovesAtPosition(possMoves, Pos("a5"));
			noEnPassant.Should().HaveCount(1, "Pawn can only move forward and should not be able to do en passant")
				.And.Contain(Move("a5,a6"));

			b.UndoLastMove(); //Undos Apply(b, "h5, h4");

			b.UndoLastMove(); //Undos Apply(b, "b2, b3");

			possMoves = b.GetPossibleMoves();
			var EnPassant = GetMovesAtPosition(possMoves, Pos("a5"));
			EnPassant.Should().HaveCount(2, "Pawn Should be able to en Passant after all the undos")
				.And.Contain(Move("a5,a6"))
				.And.Contain(Move("a5,b6"));
		}

		/// <summary>
		/// After undoing an en passant from white, checks if advantage is neutral
		/// </summary>
		[Fact]
		public void UndoEnPassantAdvantage() {
			ChessBoard b = CreateBoardFromMoves(
			"d2, d4",   //white pawn forward 2
			"a7, a6",   //random move
			"d4, d5",   //white pawn forward 1
			"e7, e5"    //black pawn forward 2
			);
			Apply(b, Move(Pos("d5"), Pos("e6"), ChessMoveType.EnPassant));  //applies en passant capture
			b.UndoLastMove();   //undos en passant capture, returning neutral advantage
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "after undoing an en passant capture from white, advantage is neutral");
		}

		/// <summary>
		/// After undoing an irrelevant move, checks if black has advantage
		/// </summary>
		[Fact]
		public void UndoMoveAdvantageTest() {
			ChessBoard b = CreateBoardFromMoves(
			"b2, b4",
			"c7, c5",
			"b4, c5",   //white pawn captures black pawn (advantage is 1 for white)
			"d7, d6",
			"c5, d6",   //white pawn captures black pawn (advantage is 2 for white)
			"d8, d6",   //black queen captures white pawn (advantage is 1 for white)
			"b1, c3",
			"d6, h2",   //black queen captures white pawn (advantage is 0)
			"c3, b5",
			"h2, g2",   //black queen captures white pawn (advantage is 1 for black)
			"b5, a3"    //move to be undo'd (didn't change advantage)
			);
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "after undoing the last move, black should have an advantage of 1");
		}

		[Fact]

		// we can check that a move has been done succefully by checking if the piece (a pawn in this case) is at the destination. Then after
		// undoing the move we apply the same logic, but the new destination is actually the origin.
		public void undoMovementTest() {
			ChessBoard b = new ChessBoard();
			Apply(b, Move("a2,a4"));
			b.GetPieceAtPosition(Pos("a4")).PieceType.Should().Be(ChessPieceType.Pawn, "The pawn that moved should now be at this position");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("a2")).PieceType.Should().Be(ChessPieceType.Pawn, "The pawn that moved should be back at this position");
		}
		[Fact]
		public void undoCapturedMoveTest() {
			ChessBoard b = CreateBoardFromPositions(
					Pos("a1"), ChessPieceType.King, 1,
					Pos("a8"), ChessPieceType.King, 2,
					Pos("c3"), ChessPieceType.Pawn, 1,
					Pos("d5"), ChessPieceType.Pawn, 2
				);
			b.GetPieceAtPosition(Pos("c3")).PieceType.Should().Be(ChessPieceType.Pawn, "The pawn should be at this position");
			b.GetPieceAtPosition(Pos("d5")).PieceType.Should().Be(ChessPieceType.Pawn, "The pawn should be at this position");
			Apply(b, Move("c3,c4"));
			Apply(b, Move("d5,c4"));
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("c4")).PieceType.Should().Be(ChessPieceType.Pawn, "The pawn that moved should be back at this position");
			b.GetPieceAtPosition(Pos("d5")).PieceType.Should().Be(ChessPieceType.Pawn, "The pawn that got captured should be back at this position");
		}

		///<summary>
		///Test several undo moves
		///</summary>
		[Fact]
		public void CheckUndoMove() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"f7,f6",
				"d2,d4",
				"g7,g5",
				"d1,h5"
			);

			b.IsCheckmate.Should().BeTrue("The game is in checkmate");
			b.IsFinished.Should().BeTrue("The game is over");
			b.UndoLastMove();
			b.IsCheckmate.Should().BeFalse("The game is no longer in checkmate");
			b.CurrentPlayer.Should().Be(1, "It is white's turn");
			b.IsFinished.Should().BeFalse("The game is not over");

			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(2, "It is black's turn");

		}

		///<summary>
		/// Test to check another application of undo move
		///</summary>
		[Fact]
		public void CheckUndoMove_andCastlingandCheck() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("a8"), ChessPieceType.Rook, 2,
				Pos("e8"), ChessPieceType.King, 2,
				Pos("h8"), ChessPieceType.Rook, 2,
				Pos("a7"), ChessPieceType.Pawn, 2,
				Pos("b7"), ChessPieceType.Pawn, 2,
				Pos("d7"), ChessPieceType.Pawn, 2,
				Pos("f7"), ChessPieceType.Pawn, 2,
				Pos("g7"), ChessPieceType.Pawn, 2,
				Pos("h7"), ChessPieceType.Pawn, 2,
				Pos("d8"), ChessPieceType.Queen, 2,
				Pos("e5"), ChessPieceType.Pawn, 2,

				Pos("e4"), ChessPieceType.Pawn, 1,
				Pos("d3"), ChessPieceType.Pawn, 1,
				Pos("a2"), ChessPieceType.Pawn, 1,
				Pos("b2"), ChessPieceType.Pawn, 1,
				Pos("f2"), ChessPieceType.Pawn, 1,
				Pos("h2"), ChessPieceType.Pawn, 1,
				Pos("g2"), ChessPieceType.Pawn, 1,
				Pos("a1"), ChessPieceType.Rook, 1,
				Pos("a2"), ChessPieceType.Pawn, 1,
				Pos("d1"), ChessPieceType.Queen, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("h1"), ChessPieceType.Rook, 1
			);

			//Check that the white player can castle to the king side 
			var possMoves = b.GetPossibleMoves();
			var checkForCastling = GetMovesAtPosition(possMoves, Pos("e1"));
			checkForCastling.Should().HaveCount(4, "King can move forward , diagonally to the left, to the right, or to castle")
				.And.Contain(Move("e1, g1"), "King can castle")
				.And.Contain(Move("e1,d2"), "King can move diagonally right")
				.And.Contain(Move("e1,e2"), "King can move one forward")
				.And.Contain(Move("e1,f1"), "King can move one left")
				.And.Contain(Move(Pos("e1"), Pos("g1"), ChessMoveType.CastleKingSide));

			//Apply white's turn 
			b.CurrentPlayer.Should().Be(1, "It is white's turn");
			Apply(b, "f2,f3");

			//It is black's turn move the black queen to check 
			Apply(b, "d8, a5");

			//Check that the board is in check 
			b.IsCheck.Should().BeTrue("The board is in check");

			//Since there is a check, the king can't castle 
			var possMovesAfterCheck = b.GetPossibleMoves();
			var checkForCastlingAfterCheck = GetMovesAtPosition(possMovesAfterCheck, Pos("e1"));
			checkForCastlingAfterCheck.Should().HaveCount(3, "King can move forward,diagonally left, diagonally right or to the right ")
				.And.Contain(Move("e1, e2"), "one forward")
				.And.Contain(Move("e1,f2"), "diagonally right")
				.And.Contain(Move("e1,f1"), "one to the right")
				.And.NotContain(Move(Pos("e1"), Pos("g1"), ChessMoveType.CastleKingSide), "The board is in check so castling is not an option ");

			//Undo the check and apply a different black move 
			b.UndoLastMove();
			Apply(b, "d7,d6");

			//The board should not be in check 
			b.IsCheck.Should().BeFalse("The board is not in check");

			//The queen should be in the original position before the check 
			b.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Queen, "The queen is back to the original place before the check");


			//The castling move should be avilable for the white king 
			var possMovesAfterUndo = b.GetPossibleMoves();
			var varcheckForCastlingAfterUndo = GetMovesAtPosition(possMovesAfterUndo, Pos("e1"));
			varcheckForCastlingAfterUndo.Should().HaveCount(5, "King can move forward , diagonally to the left or to the right, to the right, or to castle")
				.And.Contain(Move("e1, g1"))
				.And.Contain(Move(Pos("e1"), Pos("g1"), ChessMoveType.CastleKingSide), "The king can perform castling");

		}

		/// <summary>
		/// Ensure that a killed piece comes back to the board if a move is undone
		/// </summary>
		[Fact]
		public void UndoKillingPiece() {
			ChessBoard b = CreateBoardFromMoves(
					 "b1, c3",
					 "h7, h6",
					 "d2, d3",
					 "e7, e6",
					 "c3, e4",
					 "f8, c5",
					 "e4, c5" // Knight kills Bishop
				);

			// Ensure that Knight is at c5 after killing Bishop
			b.GetPieceAtPosition(Pos("c5")).PieceType.Should().Be(ChessPieceType.Knight, "the Knight should be at c5 after killing the Bishop");

			// Undo the Knight killing the Bishop
			b.UndoLastMove();

			// After undoing the move, ensure that Bishop is back at c5 and Knight is back at e4
			b.GetPieceAtPosition(Pos("c5")).PieceType.Should().Be(ChessPieceType.Bishop, "the Bishop should be at c5 because the last move was undone");
			b.GetPieceAtPosition(Pos("e4")).PieceType.Should().Be(ChessPieceType.Knight, "the Knight should be at e4 where it was before it killed the Bishop");
		}

		/// <summary>
		/// Ensure that castle move can be done if a previous Rook move is undone
		/// </summary>
		[Fact]
		public void CastleAfterMoveUndone() {
			ChessBoard b = CreateBoardFromMoves(
					 "c2, c4",
					 "h7, h6",
					 "b1, c3",
					 "h6, h5",
					 "b2, b3",
					 "h5, h4",
					 "c1, b2",
					 "h4, h3",
					 "d1, c2",
					 "g7, g6"
			);

			// Move Rook
			Apply(b, "a1, c1");

			// Undo Rook move
			b.UndoLastMove();

			var PossMoves = b.GetPossibleMoves();

			// Get Kings possible moves
			var KingPossMoves = GetMovesAtPosition(PossMoves, Pos("e1"));

			// Ensure that Kingside castle is possible after previous Rook move is undone
			KingPossMoves.Should().HaveCount(2, "King at e1 should still be able to castle if Rook move was undone")
				.And.BeEquivalentTo(Move("e1, d1"), Move("e1, c1"));
		}

		// White bishop captures black queen
		// Check board state and white turn stats before and after undo
		[Fact]
		public void captureBlackQueenAndUndo() {
			ChessBoard cb = CreateBoardFromMoves(
				"d2, d4",
				"e7, e6",
				"c1, g5",
				"d7, d6",
				"g5, d8"
			);

			cb.CurrentPlayer.Should().Be(2, "black queen was captured, now black's turn");
			cb.CurrentAdvantage.Should().Be(Advantage(1, 9), "white advantage after taking black queen");
			cb.PositionIsEmpty(Pos("g5")).Should().BeTrue("white bishop left g5 to capture black queen");
			cb.PositionIsAttacked(Pos("d8"), 2).Should().BeTrue("black king can take white bishop");

			cb.UndoLastMove();
			cb.CurrentPlayer.Should().Be(1, "after undo, now back to white's turn");
			cb.CurrentAdvantage.Should().Be(Advantage(0, 0), "after undo, no pieces have been taken");
			cb.PositionIsAttacked(Pos("d8"), 1).Should().BeTrue("black queen threatened by white bishop");
			cb.PositionIsEmpty(Pos("d8")).Should().BeFalse("black queen is back after undo");
			cb.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Queen, "queen is back after undo");
		}

		[Fact]
		// involves undolastmove to prevent a queen from being eaten
		public void SaveTheQueen() {
			// failed fool's mate
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"e7, e5",
				"g2, g3"
			);

			var possMoves = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(possMoves, Pos("d8"));
			expected.Should().Contain(Move("d8, h4"))
				.And.Contain(Move("d8, g5"))
				.And.Contain(Move("d8, f6"))
				.And.Contain(Move("d8, e7"))
				.And.HaveCount(4, "The queen can only move diagonally from this position");

			// the queen moves to a position where a pawn can eat her
			Apply(b, Move("d8, h4"));

			// the pawn eats her!
			possMoves = b.GetPossibleMoves();
			expected = GetMovesAtPosition(possMoves, Pos("g3"));
			expected.Should().Contain(Move("g3, h4"))
				.And.HaveCount(1, "The pawn can only eat the queen to prevent a checkmate");
			Apply(b, Move("g3, h4"));

			// check everything
			b.GetPieceAtPosition(Pos("h4")).PieceType.Should().Be(ChessPieceType.Pawn, "The pawn ate the queen");
			b.CurrentAdvantage.Should().Be(Advantage(1, 9), "White ate a queen of value worth 9");

			// black starts crying so white agrees to undo all of it

			b.UndoLastMove(); // undo white's consumption
			b.GetPieceAtPosition(Pos("h4")).PieceType.Should().Be(ChessPieceType.Queen, "The pawn unate the queen");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "Undo the only consumption");

			b.UndoLastMove(); // undo black putting their queen there
			b.GetPieceAtPosition(Pos("h4")).Player.Should().Be(0, "The queen moved back to the initial position so this position is now empty");
			b.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Queen, "The queen went home");
		}



		// involves undolastmove to prevent a fools mate from happening
		[Fact]
		public void PreventFoolsMate() {
			// Fool's mate?
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4", // player 1
				"e7, e5" // player 2
			);

			var possMoves = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(possMoves, Pos("g2"));
			expected.Should().Contain(Move("g2, g3"))
				.And.Contain(Move("g2, g4"))
				.And.HaveCount(2, "a pawn can move forward 1 or 2 spaces");

			Apply(b, Move("g2, g4")); // player 1

			// now black should be in a position to checkmate
			possMoves = b.GetPossibleMoves();
			expected = GetMovesAtPosition(possMoves, Pos("d8"));
			expected.Should().Contain(Move("d8, h4"), "The queen should be able to checkmate with this move");

			// undo the 2nd white pawn move to protect the king by moving it somewhere else
			b.UndoLastMove();

			// check if the pawn moved back
			b.GetPieceAtPosition(Pos("g2")).PieceType.Should().Be(ChessPieceType.Pawn, "The pawn moved back to its original position");

			// the current player should still be player 1
			b.CurrentPlayer.Should().Be(1, "The current player should not have changed after undoing a move");
		}


		/// <summary>
		/// Restore taken pawn when undone
		/// </summary>
		[Fact]
		public void UndoRestoresTakenPawn() {
			// White pawn takes black pawn
			ChessBoard b = CreateBoardFromMoves(
				"a2, a3",
				"b7, b6",
				"a3, a4",
				"b6, b5",
				"a4, b5"
			);

			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Player 1 ahead by 1 pawn");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("a4")).PieceType.Should().Be(ChessPieceType.Pawn, "White pawn back at a4");
			b.GetPieceAtPosition(Pos("b5")).PieceType.Should().Be(ChessPieceType.Pawn, "Black pawn back at b5");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "No advantage, previously taken pawn is restored");
		}

		/// <summary>
		/// Running UndoMove for the total number of moves made in a game so far should restore the board to its spawn state
		/// </summary>
		[Fact]
		public void MassUndo() {

			var moves = new List<string> { "g2, g4",
				"f7, f5",
				"g4, f5",
				"g7, g5",
				"g1, f3",
				"g5, g4",
				"f1, h3",
				"g4, g3",
				"h3, g4",
				"g3, g2"};

			ChessBoard b = new ChessBoard();

			foreach (string move in moves) {
				Apply(b, move);
			}

			for (int i = 0; i < moves.Count; i++) {
				b.UndoLastMove();
			}

			//checks to make sure all pawns are back where they should be
			List<string> pawn_columns = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h" };
			List<string> pawn_rows = new List<string>() { "2", "7" };
			foreach (string row in pawn_rows) {
				foreach (string column in pawn_columns) {
					b.GetPieceAtPosition(Pos(column + row)).PieceType.Should().Be(ChessPieceType.Pawn, "pawns should spawn in all squares on rows 2 and 7");
				}
			}

			b.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "the black king should be at home position");

			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "the white king should be at home position");
		}

		/// <summary>
		/// Promote a pawn then undo the move
		/// </summary>
		[Fact]
		public void UndoPawnPromote() {
			ChessBoard b = CreateBoardFromMoves(
				"b2, b4",
				"a7, a5",
				"b4, b5",
				"a8, a6",
				"b5, a6", // capture rook with pawn
				"b8, c6",
				"a6, a7",
				"b7, b5",
				"c2, c3",
				"c8, b7",
				"c3, c4",
				"d7, d6",
				"c4, c5",
				"d8, d7"
			);

			// Apply the promotion move
			Apply(b, Move("(a7, a8, Rook)"));
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Rook, "the pawn was replaced by a rook");
			b.CurrentPlayer.Should().Be(2, "the current player is black");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("a7")).PieceType.Should().Be(ChessPieceType.Pawn, "the rook has reverted back to a pawn");
			b.CurrentPlayer.Should().Be(1, "the current player is white");
			b.GetPieceAtPosition(Pos("d7")).PieceType.Should().Be(ChessPieceType.Queen, "the move before the promotion is not effected by UndoLastMove");

		}

		/// <summary>
		/// Undo a first move made by white
		/// </summary>
		[Fact]
		public void UndoFirstMove() {
			//Initialize first move
			ChessBoard b = CreateBoardFromMoves(
					"g2, g4"

		  );
			//Undo move and start over
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no operations have changed the advantage");
			b.CurrentPlayer.Should().Be(1, "Player turn should be white, who takes the first turn");
			var empty = b.GetPieceAtPosition(Pos("g4"));
			empty.Player.Should().Be(0, "g4 should be empty again");
			var possible = b.GetPossibleMoves();
			possible.Should().HaveCount(20, "There should be 20 possible moves to start the game from a redo of the first turn, 16 from pawns, 2 from each knight");
			var forPawn = GetMovesAtPosition(possible, Pos("g2"));
			forPawn.Should().Contain(Move("g2, g3"))
				.And.Contain(Move("g2, g4"))
			 .And.HaveCount(2, "the orignal moved pawn should be placed back at it's orignal position and have its orginal 2 moves");

		}

		[Fact]
		//The purpose of this test is to undo the move when the white rook moves from 
		//a1 to a3.
		public void UndoLastMoveWRook() {
			ChessBoard board = CreateBoardFromMoves(
				"a2, a4",
				"a7, a5",
				"a1, a3");
			board.GetPieceAtPosition(Pos("a3")).PieceType.Should().Be(ChessPieceType.Rook, "This square should be rook");
			board.GetPieceAtPosition(Pos("a3")).Player.Should().Be(1, "This should be a white rook");
			board.UndoLastMove();

			board.GetPieceAtPosition(Pos("a3")).PieceType.Should().Be(ChessPieceType.Empty, "This square should be empty");
			board.GetPieceAtPosition(Pos("a1")).Player.Should().Be(1, "This should be the white player");
			board.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "This square should be rook");

			board.GetPieceAtPosition(Pos("a5")).PieceType.Should().Be(ChessPieceType.Pawn, "This piece should be a pawn");
			board.GetPieceAtPosition(Pos("a5")).Player.Should().Be(2, "This piece should be black");

			board.GetPieceAtPosition(Pos("a4")).PieceType.Should().Be(ChessPieceType.Pawn, "This piece should be a pawn");
			board.GetPieceAtPosition(Pos("a4")).Player.Should().Be(1, "This piece should be white");
		}

		[Fact]
		//The purpose of this test is to check if the black knight's move is undone
		//It should return to the starting position
		public void UndoLastMoveBKnight() {
			ChessBoard board = CreateBoardFromMoves(
				"b2, b4",
				"b7, b5",
				"c1, a3",
				"b8, c6");

			var bKnight = board.GetPieceAtPosition(Pos("c6"));
			bKnight.PieceType.Should().Be(ChessPieceType.Knight, "This piece should be a knight");
			bKnight.Player.Should().Be(2, "This piece should be black");

			board.UndoLastMove();

			board.GetPieceAtPosition(Pos("c6")).PieceType.Should().Be(ChessPieceType.Empty, "This square should be empty");

			board.GetPieceAtPosition(Pos("b8")).PieceType.Should().Be(ChessPieceType.Knight, "This square should have the knight");
			board.GetPieceAtPosition(Pos("b8")).Player.Should().Be(2, "This knight should be black");

			board.GetPieceAtPosition(Pos("a3")).PieceType.Should().Be(ChessPieceType.Bishop, "This square should have a bishop");
			board.GetPieceAtPosition(Pos("a3")).Player.Should().Be(1, "This piece should be white");


			board.GetPieceAtPosition(Pos("b5")).PieceType.Should().Be(ChessPieceType.Pawn, "This square should have a pawn");
			board.GetPieceAtPosition(Pos("b5")).Player.Should().Be(2, "This piece should be black");

			board.GetPieceAtPosition(Pos("b4")).PieceType.Should().Be(ChessPieceType.Pawn, "This square should have a pawn");
			board.GetPieceAtPosition(Pos("b4")).Player.Should().Be(1, "This piece should be white");


		}

		[Fact]
		public void CaptureUndoTest() {
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",
				"e7, e5",
				"a2, a4",
				"c7, c5",
				"d1, d3"

			);
			Apply(b, Move("e5, d4"));//black pawn captures white pawn
			b.GetPieceAtPosition(Pos("d4")).Player.Should().Be(2, "Player 2(Black) captured Player 1's (White) pawn");
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "Black captured a White pawn");
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "Captured White pawn should be on board after undo");
			b.GetPieceAtPosition(Pos("d4")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn should be at d4 after undoing last move");
			b.GetPlayerAtPosition(Pos("d4")).Should().Be(1, "White should be at d4 after undoing last move");
		}

		[Fact]
		public void MoveUndoTest() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"c7, c5",
				"e2, e3",
				"b8, a6",
				"f2, f4"
			);

			Apply(b, "d7, d6");
			b.UndoLastMove();
			b.GetPlayerAtPosition(Pos(5, 3)).Should().Be(0, "No players should be in this space");
		}

		// <summary>
		// Test 2 involving UndoLastMove (WITH BLACK)
		// This involves undoing a captured black rook
		//</summary>
		[Fact]
		public void UndoCapturedRook() {
			ChessBoard b = CreateBoardFromMoves(
				"h2, h3",
				"h7, h5",
				"h1, h2",
				"h8, h6",
				"g1, f3"
				);

			Apply(b, "h6, g6", "f3, h4", "f7,f6"); //use white knight to capture black's bishop 

			var possibleMoves = b.GetPossibleMoves();
			var whiteKnight = GetMovesAtPosition(possibleMoves, Pos("h4"));
			whiteKnight.Should().HaveCount(3, "knight can capture rook at g6 or move to two other empty spots")
			.And.Contain(Move("h4, f3"))
			.And.Contain(Move("h4, f5"))
			.And.Contain(Move("h4, g6"));

			//choose to take the bishop
			Apply(b, "h4, g6");
			var capturing = b.GetPieceAtPosition(Pos("g6"));
			capturing.Player.Should().Be(1, "knight captures rook");
			capturing.PieceType.Should().Be(ChessPieceType.Knight);
			var captured = b.GetPieceAtPosition(Pos("h4"));
			captured.Player.Should().Be(0, "spot should be empty");


			b.UndoLastMove();

			capturing = b.GetPieceAtPosition(Pos("h4"));
			capturing.Player.Should().Be(1, "knight back in its prev. pos");
			capturing.PieceType.Should().Be(ChessPieceType.Knight, "piece should be a white knight");
			captured = b.GetPieceAtPosition(Pos("g6"));
			captured.Player.Should().Be(2, "rook is placed back");
			captured.PieceType.Should().Be(ChessPieceType.Rook, "piece should be a black rook");

			b.CurrentPlayer.Should().Be(1, "Current Player is white");

		}

		//<summary>
		//Test 1 involving UndoLastMove (FOR WHITE)
		// This involves undoing a White Castle on the King's Side
		//</summary>
		[Fact]
		public void CastlingWhiteUndo() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"e7, e5",
				"f1, c4",
				"g8, f6",
				"g1, f3",
				"b7, b6"
				);

			Apply(b, "c4, d5", "g7,g5");

			var possibleMoves = b.GetPossibleMoves();
			var expectedCastle = GetMovesAtPosition(possibleMoves, Pos("e1"));
			expectedCastle.Should().HaveCount(3, "King can castle with rook or move up one or right one")
			.And.Contain(Move(Pos("e1"), Pos("g1"), ChessMoveType.CastleKingSide))
			.And.Contain(Move("e1,e2")) //move up one
			.And.Contain(Move("e1,f1")); // move to the right one

			//white chooses to do a castle
			Apply(b, Move(Pos("e1"), Pos("g1"), ChessMoveType.CastleKingSide));

			var king = b.GetPieceAtPosition(Pos("g1"));
			king.Player.Should().Be(1, "king switched with rook");
			king.PieceType.Should().Be(ChessPieceType.King, "type should now be a king");

			var rook = b.GetPieceAtPosition(Pos("f1"));
			rook.Player.Should().Be(1, "white rook protecting king");
			rook.PieceType.Should().Be(ChessPieceType.Rook, "type should now be a rook");

			var emptySpot1 = b.GetPieceAtPosition(Pos("h1"));
			emptySpot1.Player.Should().Be(0, "corner square should be empty");
			emptySpot1.PieceType.Should().Be(0, "empty space, no piece");

			var emptySpot2 = b.GetPieceAtPosition(Pos("e1"));
			emptySpot2.Player.Should().Be(0, "king's prev. spot should be empty");
			emptySpot2.PieceType.Should().Be(0, "empty space, no piece");

			//undo the castle
			b.UndoLastMove();

			king = b.GetPieceAtPosition(Pos("e1"));
			king.Player.Should().Be(1, "King back to its og spot");
			king.PieceType.Should().Be(ChessPieceType.King, "piece is a king");

			rook = b.GetPieceAtPosition(Pos("h1"));
			rook.Player.Should().Be(1, "Rook back to its corner");
			rook.PieceType.Should().Be(ChessPieceType.Rook, "piece is a rook");

			emptySpot1 = b.GetPieceAtPosition(Pos("f1"));
			emptySpot1.Player.Should().Be(0, "spot becomes empty");
			emptySpot1.PieceType.Should().Be(0, "no piece on the square");

			emptySpot2 = b.GetPieceAtPosition(Pos("g1"));
			emptySpot2.Player.Should().Be(0, "spot becomes empty");
			emptySpot2.PieceType.Should().Be(0, "no piece on the square");
		}

		//this sets up a pawn to be captured by a knight
		//the capture is undone and the state of the game board is checked for correctness
		[Fact]
		public void UndoBishopCaptureByQueen() {

			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"e7, e5",
				"d1, g4",
				"d7, d6"
			);

			//checks the current state of the board
			var possMoves = b.GetPossibleMoves();
			var queenMoves = GetMovesAtPosition(possMoves, Pos("g4"));
			queenMoves.Should().Contain(Move("g4, c8"), "THhe queen should be able to make the move g4, c3");

			//applies the capture
			Apply(b, "g4, c8");

			//checks the current state of the board
			var queen = b.GetPieceAtPosition(Pos("c8"));
			queen.PieceType.Should().Be(ChessPieceType.Queen, "The queen now takes the spot of the bishop");
			b.PositionIsAttacked(Pos("c8"), 2).Should().BeTrue("Player 2 attacked position c8");
			b.PositionIsEnemy(Pos("c8"), 2).Should().BeTrue("c8 should now be an enemy");
			b.CurrentAdvantage.Should().Be(Advantage(1, 3), "Player 1 should have an advantage of 3 because they captured a bishop");

			//undo the capture
			b.UndoLastMove();

			//checks the current state of the board
			var bishop = b.GetPieceAtPosition(Pos("c8"));
			bishop.PieceType.Should().Be(ChessPieceType.Bishop, "The bishop goes back to its last position");
			queen = b.GetPieceAtPosition(Pos("g4"));
			queen.PieceType.Should().Be(ChessPieceType.Queen, "The Queen should be back at its last position");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "Player 1 should have no advantage because the move was undone");
		}

		//this sets up a pawn to be captured by a knight
		//the capture is undone and the state of the game board is checked for correctness
		[Fact]
		public void UndoPawnCaptureByKnight() {

			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"c7, c5",
				"f4, f5",
				"c5, c4",
				"f5, f6"
			);

			//checks the current state of the board
			var possMoves = b.GetPossibleMoves();
			var knightMoves = GetMovesAtPosition(possMoves, Pos("g8"));
			knightMoves.Should().Contain(Move("g8, f6"), "This move should be possible in order to capture the pawn");

			//applies the capture
			Apply(b, "g8, f6");

			//checks the current state of the board
			var knight = b.GetPieceAtPosition(Pos("f6"));
			knight.PieceType.Should().Be(ChessPieceType.Knight, "The knight should now be in the position of the pawn");
			b.PositionIsAttacked(Pos("f6"), 2).Should().BeTrue("The position should be attacked by the enemy");
			b.PositionIsEnemy(Pos("f6"), 1).Should().BeTrue("That position should now be an enemy");
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "Player 2 should have an advantage of 1 because they captured a pawn");

			//undo the capture
			b.UndoLastMove();

			//checks the current state of the board
			var pawn = b.GetPieceAtPosition(Pos("f6"));
			pawn.PieceType.Should().Be(ChessPieceType.Pawn, "The pawn moved back to its last position");
			knight = b.GetPieceAtPosition(Pos("g8"));
			knight.PieceType.Should().Be(ChessPieceType.Knight, "The knight should be back at its starting position");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "Player 2 should have no advantage because the move was undone");
		}

		[Fact]
		public void UndoPawnQueenSide() {
			ChessBoard b = CreateBoardFromMoves(
				 "d2, d4",
				 "c7, c5");
			Apply(b, Move("d4, c5"));
			b.PositionIsEmpty(Pos("d4")).Should().BeTrue();
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("d4")).PieceType.Should().Be(ChessPieceType.Pawn, "Undo should put pawn back into position before taking other pawn");
			b.GetPieceAtPosition(Pos("c5")).PieceType.Should().Be(ChessPieceType.Pawn, "Undo should put taken pawn back into previous position");
		}

		[Fact]
		public void UndoKnightKingSide() {
			ChessBoard b = CreateBoardFromMoves(
			  "c2, c4",
			  "h7, h5",
			  "b1, c3",
			  "e7, e5");
			Apply(b, Move("g1, f3"));
			b.PositionIsEmpty(Pos("g1")).Should().BeTrue();
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "Undo should put knight back into starting position");


		}

		[Fact]
		public void check_correctPlayerTurn() {
			// create a board with two pawns on it
			ChessBoard b = CreateBoardFromMoves(
					"c2,c3"
				);
			b.CurrentPlayer.Should().Be(2, "moving the pawn will set the turn to player 2");
			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(1, "current player should go back to player 1 after un doing");


		}

		// checks that the pawn moved back to the appropriate position after undoing the move 
		[Fact]
		public void check_Advantage_after_undo() {
			// create a board with two pawns on it
			ChessBoard b = CreateBoardFromMoves(
					"c2, c4",
					"d7, d5"
				);
			var poss = b.GetPossibleMoves();
			Apply(b, Move("c4, d5"));
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Advantage should be 1 - 0");
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "there should be no advantage beacuse the capture was undone");
		}

		[Fact]
		//Use UndoLastMove #2
		public void undoSimpleCheck() {
			ChessBoard b = CreateBoardFromMoves(
				"h2, h4",
				"e7, e5",
				"g2, g4",
				"f8, c5",
				"d2, d4",
				"c5, b4"
			);

			b.IsCheck.Should().BeTrue("In check");
			var poss = b.GetPossibleMoves();
			poss.Should().HaveCount(5, "5 possible moves");
			b.UndoLastMove();
			b.IsCheck.Should().BeFalse("Not in check");
			Apply(b, "c5, b4");
			b.IsCheck.Should().BeTrue("In check");
		}

		[Fact]//Undo Last Move #1
				//King in check
		public void undoTakenPiece() {
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",
				"c7, c6",
				"e2, e3",
				"d8, a5"
			);

			b.IsCheck.Should().BeTrue("Check is true");

			Apply(b, "d1, d2");

			b.IsCheck.Should().BeFalse("Check is false");

			Apply(b, "a5, d2");

			b.IsCheck.Should().BeTrue("Check is True");

			b.UndoLastMove();

			b.IsCheck.Should().BeFalse("Check is False");

			b.GetPieceAtPosition(Pos("a5")).PieceType.Should().Be(ChessPieceType.Queen, "Queen moved back to A5");
			b.GetPieceAtPosition(Pos("d2")).PieceType.Should().Be(ChessPieceType.Queen, "Queen put back on D2");
		}

		[Fact]
		public void undoOneMove() {
			ChessBoard board = new ChessBoard();
			IEnumerable<ChessMove> original = board.GetPossibleMoves();
			Apply(board, "e2, e4");
			board.UndoLastMove();
			IEnumerable<ChessMove> original2 = board.GetPossibleMoves();

			original.Should().BeEquivalentTo(original2, "After undoing a pawn's move, the possible moves should be identical to before the move was applied.");
		}

		[Fact]
		public void undoAllMoves() {

			ChessBoard board = CreateBoardFromMoves
				(
					"a2, a4",
					"d7, d5",
					"h2, h4",
					"f7, f5"
				);

			board.UndoLastMove();
			board.UndoLastMove();
			board.UndoLastMove();
			board.UndoLastMove();

			var moves = board.GetPossibleMoves();

			moves.Should().Contain(Move("a2, a3")).And.Contain(Move("a2, a4")).And.Contain(Move("b2, b3")).And.Contain(Move("b2, b4")).And.Contain(Move("c2, c3")).And.
				Contain(Move("c2, c4")).And.Contain(Move("d2, d3")).And.Contain(Move("d2, d4")).And.Contain(Move("e2, e3")).And.Contain(Move("e2, e4")).And.Contain(Move("f2, f3"))
				.And.Contain(Move("f2, f4")).And.Contain(Move("g2, g3")).And.Contain(Move("g2, g4")).And.Contain(Move("h2, h3")).And.Contain(Move("h2, h4"), "This test undoes all the moves to return the board to the original state");

		}
	}
}
