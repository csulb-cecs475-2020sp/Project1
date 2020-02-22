using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class GameEndTests : ChessTest {
		//check for a checkmate on the black side of the board
		[Fact]
		public void blackCheckmate() {
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"e7, e5",
				"g2, g4",
				"d8, h4"
				);
			b.GetPieceAtPosition(Pos("h4")).PieceType.Should().Be(ChessPieceType.Queen, "the queen should place the king into checkmate");
			b.GetPieceAtPosition(Pos("h4")).Player.Should().Be(2, "the queen is controlled by player 2");
			b.IsCheckmate.Should().BeTrue("the king has no escape");
		}

		/// <summary>
		/// Black has White in check, White captures a piece to put it out of check, 
		/// Black captures a piece to place White in Checkmate
		/// </summary>
		[Fact]
		public void CheckMateAndCheck() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("e3"), ChessPieceType.King, 1,
				Pos("d8"), ChessPieceType.King, 2,
				Pos("b3"), ChessPieceType.Knight, 1,
				Pos("h2"), ChessPieceType.Bishop, 1,
				Pos("f8"), ChessPieceType.Rook, 2,
				Pos("a6"), ChessPieceType.Bishop, 2,
				Pos("d6"), ChessPieceType.Knight, 2,
				Pos("c5"), ChessPieceType.Queen, 2,
				Pos("d4"), ChessPieceType.Pawn, 2,
				Pos("h5"), ChessPieceType.Bishop, 2,
				Pos("c3"), ChessPieceType.Pawn, 2,
				Pos("c6"), ChessPieceType.Knight, 2
				);

			var possMove = b.GetPossibleMoves();
			var WhiteKnightsMove = GetMovesAtPosition(possMove, Pos("b3"));
			var WhiteKingsMove = GetMovesAtPosition(possMove, Pos("e3"));
			var WhiteBishopsMove = GetMovesAtPosition(possMove, Pos("h2"));
			b.IsCheck.Should().BeTrue("White King is in check by enemy pawn");
			b.IsCheckmate.Should().BeFalse("Board is not in checkmate because the White knight has to capture the pawn");
			WhiteKingsMove.Should().HaveCount(0, "White King cannot make any moves");
			WhiteKnightsMove.Should().HaveCount(1, "White Knight has to capture the Pawn")
				.And.Contain(Move("b3,d4"));
			WhiteBishopsMove.Should().HaveCount(0, "White Bishop cannot make any moves to save the King");
			Apply(b, "b3, d4");//White knight captures the pawn

			Apply(b, "c5, d4");//Black Queen captures the knight

			possMove = b.GetPossibleMoves();
			WhiteKingsMove = GetMovesAtPosition(possMove, Pos("e3"));
			WhiteKingsMove.Should().HaveCount(0, "White King cannot make any moves because of the checkmate");
			WhiteBishopsMove = GetMovesAtPosition(possMove, Pos("h2"));
			WhiteBishopsMove.Should().HaveCount(0, "White Bishop cannot make any moves because of the checkmate");
			b.IsCheck.Should().BeFalse("Situation is now checkmate");
			b.IsCheckmate.Should().BeTrue("White King is now in checkmate");
		}

		/// <summary>
		/// Checks that black only has three different possible moves in the case of a check
		/// </summary>
		[Fact]
		public void CheckMatePossibleMove() {
			ChessBoard b = CreateBoardFromMoves(
			"e2, e4",
			"d7, d5",
			"f1, c4",
			"e7, e6",
			"c4, b5"    //white bishop checkmates black king
			);

			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("c8"));

			//black bishop block
			expected.Should().Contain(Move("c8, d7"))
				 .And.HaveCount(1, "black bishop should only have one possible move, blocking the check");

			//black queen block
			expected = GetMovesAtPosition(poss, Pos("d8"));
			expected.Should().Contain(Move("d8, d7"))
				 .And.HaveCount(1, "black queen should only have one possible move, blocking the check");

			//black king avoid
			expected = GetMovesAtPosition(poss, Pos("e8"));
			expected.Should().Contain(Move("e8, e7"))
				 .And.HaveCount(1, "black king should only have one possible move, avoiding the check");
		}

		[Fact]
		// if there are only two kings in the game, they cannot capture each other. Therefore, at some point (100 moves in this case), the game
		// should end with a draw
		// It does not work, due to a bug in the the files, I tried to download the files again but it seemed that nothing changed. 
		// Even without this one there are still a total of 7 test cases
		public void DrawTest2() {
			ChessBoard b = CreateBoardFromPositions(
					Pos("c6"), ChessPieceType.King, 1,
					Pos("a6"), ChessPieceType.King, 2
				);
			bool c6 = true; bool a6 = true;
			for (int i = 0; i < 100; i++) {
				if (i % 2 == 0) {
					if (c6)
						Apply(b, Move("c6,c7"));
					else
						Apply(b, Move("c7,c6"));
					c6 = !c6;
					b.DrawCounter.Should().Be(1 +  i);
				}
				else {
					if (a6)
						Apply(b, Move("a6,a7"));
					else
						Apply(b, Move("a7,a6"));
					a6 = !a6;
				}

			}
			b.IsDraw.Should().BeTrue("because the value of the counter is: " + b.DrawCounter);
		}

		///<summary>
		/// Test stalemate situation 
		[Fact]
		public void CheckForStaleMate() {


			ChessBoard b = CreateBoardFromPositions(
				Pos("a8"), ChessPieceType.King, 2,
				Pos("b7"), ChessPieceType.Pawn, 2,
				Pos("f2"), ChessPieceType.Rook, 2,
				Pos("g3"), ChessPieceType.Queen, 2,
				Pos("h1"), ChessPieceType.King, 1
			);

			b.IsStalemate.Should().BeTrue("The game is in stalemae");

		}
		/// <summary>
		/// Place black king in check and checkmate. Verify that the board correctly differentiates between the two and calculates the correct moves for
		/// the king
		/// </summary>
		[Fact]
		public void InCheckMate() {
			ChessBoard board = CreateBoardFromPositions(Pos("g8"), ChessPieceType.King, 2,
				Pos("h6"), ChessPieceType.King, 1,
				Pos("f4"), ChessPieceType.Rook, 1,
				Pos("b3"), ChessPieceType.Queen, 1,
				Pos("a2"), ChessPieceType.Pawn, 1,
				Pos("a7"), ChessPieceType.Pawn, 2);
			Apply(board, Move(Pos("a2"), Pos("a3")));
			var poss = board.GetPossibleMoves();
			var bKing = GetMovesAtPosition(poss, Pos("g8"));
			bKing.Should().HaveCount(1, "Should only be able to move to the right since black king is in check");
			board.IsCheck.Should().Be(true, "Black king should be in check");
			board.IsCheckmate.Should().Be(false, "Black king still has possible moves");
			Apply(board, Move(Pos("g8"), Pos("h8")));
			Apply(board, Move(Pos("f4"), Pos("f8")));
			poss = board.GetPossibleMoves();
			bKing = GetMovesAtPosition(poss, Pos("g8"));
			board.IsCheck.Should().Be(false, "Check and checkmate are mutually exclusive");
			board.IsCheckmate.Should().Be(true, "Check and checkmate are mutually exclusive");
			board.IsStalemate.Should().Be(false, "Should not be a stalemate as one player won");
			bKing.Should().HaveCount(0, "Since in checkmate, black king should have no more moves left");
			board.IsFinished.Should().Be(true, "Game is finished since black king was placed in checkmate");
			board.UndoLastMove();
			board.IsFinished.Should().Be(false, "Game is still ongoing since there is no stalemate or checkmate");
			board.IsCheckmate.Should().Be(false, "Black king is no longer in checkmate");
			board.UndoLastMove();
			board.IsCheck.Should().Be(true, "King is no longer in checkmate since last two moves were undone");
			board.IsCheckmate.Should().Be(false, "King still has possible moves");
			board.IsStalemate.Should().Be(false, "Should not be a stalemate as game is still going");
			poss = board.GetPossibleMoves();
			bKing = GetMovesAtPosition(poss, Pos("g8"));
			bKing.Should().NotHaveCount(0, "King should still be able to move since last two moves were undone");
		}

		[Fact]
		// "Tricky" situation
		public void FoolsMate2() {
			// Fool's mate
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"e7, e5",
				"g2, g4",
				"d8, h4"
			);

			// When checkmate occurs, the possible moves list should be empty;
			// IsFinished should be true, as should IsCheckmate.
			var possMoves = b.GetPossibleMoves();
			var blockedKing = GetMovesAtPosition(possMoves, Pos("e1"));
			blockedKing.Should().BeEmpty("The white king is in checkmate");
			b.IsCheckmate.Should().BeTrue("The white king cannot move");
			b.IsFinished.Should().BeTrue("The game should be over");
		}

		/// <summary>
		/// A checkmate in 4 moves that is known as the "Scholar's mate"
		/// </summary>
		[Fact]
		public void ScholarsMate() {
			//Set board up for checkmate
			ChessBoard b = CreateBoardFromMoves(
					"e2, e4",
					 "e7, e5",
					 "f1, c4",
					 "b8, c6",
					 "d1, f3",
					 "d7, d6"
				);

			//Queen will capture white pawn at f7, thus causing checkmate and finishing game
			Apply(b, "f3, f7");
			var possible = b.GetPossibleMoves();
			possible.Count().Should().Be(0, "There should be 0 possible moves to cancel the checkmate from White Queen");
			b.IsCheckmate.Should().BeTrue("Black's king is in checkmate from White Queen at f7, and white bishop at c4 if black king attacks white queen");
			b.IsFinished.Should().BeTrue("The game should be over after checkmate");

		}


		/// <summary>
		/// A checkmate occurs when no moves can be made to protect a King in check
		/// </summary>
		[Fact]
		public void checkmate() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"f7, f6",
				"d2, d3",
				"g7, g5",
				"d1, h5");

			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(0, "The game is won by white");
			b.IsCheckmate.Should().BeTrue("Black can make no moves to protect the King");
			b.IsFinished.Should().BeTrue("The game is over");

			b.UndoLastMove();
			b.IsCheckmate.Should().BeFalse("Undoing the move also removes checkmate");
			b.IsFinished.Should().BeFalse("No player has won the game after undoing the move");
		}

		/// <summary>
		/// No moves can be made if there is a stalemate
		/// </summary>
		[Fact]
		public void stalemate() {
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
				"f7, f6",
				"c7, d7",
				"e8, f7",
				"d7, b7",
				"d8, d3",
				"b7, b8",
				"d3, h7",
				"b8, c8",
				"f7, g6"
				);

			Apply(b, "c8, e6");
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(0, "Current player should not be able to move any piece without leaving King in check");
			b.IsStalemate.Should().BeTrue();

			b.UndoLastMove();
			possMoves = b.GetPossibleMoves();
			possMoves.Should().NotBeEmpty("No player is in checkmate or stalemate");
			b.IsStalemate.Should().BeFalse("Undoing the last move removed the stalement");
		}

		/*2. Check tricky situation*/
		[Fact]
		public void CheckIfCheckmate() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"c7, c5",
				"e2, e3",
				"b8, a6",
				"f2, f4",
				"d7, d6",
				"d2, d4",
				"e8, d7",
				"c1, d2",
				"h7, h6",
				"f4, f5"

			);
			Apply(b, "h8, h7");
			Apply(b, "d2, a5");
			Apply(b, "a8, b8");
			Apply(b, "f1, b5");
			b.IsCheckmate.Should().BeTrue("the king can't escape");
		}



		/// <summary>
		/// Apply moves known as the Fool's move and test
		/// Also call undos and check board states
		/// </summary>
		[Fact]
		public void fastestCheckMate() {
			ChessBoard b = CreateBoardFromMoves(
				"f2, f3",
				"e7, e5",
				"g2, g4"
				);
			//it should be Player 2's turn
			b.CurrentPlayer.Should().Be(2, "Player's 2 turn");
			Apply(b, Move("d8, h4"));
			//after applying the move, then it should be Player 1's Turn
			b.CurrentPlayer.Should().Be(1, "Player's 1 turn");

			b.IsCheckmate.Should().BeTrue("CheckMate has occured, Player 2 Wins");
			b.IsFinished.Should().BeTrue("CheckMate has occured, the game isfinished");
			b.IsCheck.Should().BeFalse("CheckMate has occurred, Player 2 Wins");
			b.IsDraw.Should().BeFalse("CheckMate has occurred, there is no Draw");

			//undo lsat move
			b.UndoLastMove();

			//After undo, it should be Player 2's turn again
			b.CurrentPlayer.Should().Be(2, "Player's 2 turn");
			//Checkmate did not happen after undo
			b.IsCheckmate.Should().BeFalse("Undo happened, checkmate is not happened");
			b.IsFinished.Should().BeFalse("Undo happened, game is not finished");
			b.IsCheck.Should().BeFalse("Nothing is in check from undo");
			b.IsDraw.Should().BeFalse("Game is still going on from undo");
		}

		/// <summary>
		/// Apply fastest Stale Mate moves
		/// Then check for boolean conditions for
		/// isFinished, Stalemate, isCheckMate, and isCheck
		/// </summary>
		[Fact]
		public void FastestStaleMate() {
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
				"f7, f6",
				"c7, d7",
				"e8, f7",
				"d7, b7",
				"d8, d3",
				"b7, b8",
				"d3, h7",
				"b8, c8",
				"f7, g6"
			);
			//move queen to e6 to complete stalemate board
			Apply(b, Move("c8, e6"));
			var possMove = b.GetPossibleMoves();
			//test all possible cases
			possMove.Should().BeEmpty("Stale Mate has occured, no more possible moves can be made");
			b.IsFinished.Should().BeTrue("Stale Mate has occured, the game is finished");
			b.IsStalemate.Should().BeTrue("Stale Mate has occured, no more moves can be made by Player Two");
			b.IsCheckmate.Should().BeFalse("Stale Mate has occured, the match results is Draw");
			b.IsCheck.Should().BeFalse("Stale Mate has occurred, no Check");
		}

		//this puts the king in check mate by a sequence of moves called fool's mate 
		[Fact]
		public void KingInCheckMate() {

			//fool's mate
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"e7, e5",
				"g2, g4",
				"d8, h4"
			);
			b.IsCheckmate.Should().BeTrue("The King is in check mate.");
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().BeEmpty("There are no possible moves.");
			b.IsFinished.Should().BeTrue("The game is over because the king is in checkmate");

		}

		/// <summary>
		/// Black player causes white to be stalemated (not in check, but no legal move available).
		/// </summary>
		[Fact]
		public void Stalemate2() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("e8"), ChessPieceType.King, 2,
				Pos("h1"), ChessPieceType.King, 1,
				Pos("g3"), ChessPieceType.Queen, 2
			);

			b.IsCheck.Should().BeFalse("black king is not in check");
			b.IsStalemate.Should().BeTrue("Black queen threatens white king, but king has no legal moves although not in check");
			b.IsFinished.Should().BeTrue("stalemate. game over");
		}

		/// <summary>
		/// 50 full move rule results in a draw
		/// If both sides make 50 full consecutive moves without a capture or pawn move, player may claim a draw
		/// </summary>
		[Fact]
		public void FiftyFullMoveRule() {
			ChessBoard b = CreateBoardFromMoves(
				// move pawns out of the way
				"a2, a4",
				"a7, a6",
				"e2, e3",
				"g7, g5",
				"h2, h4",
				"d7, d5"
			);

			b.CurrentPlayer.Should().Be(1, "white should be next");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no captures made");
			b.DrawCounter.Should().Be(0, "only pawns have moved so far");

			Apply(b, Move("(a1, a2)")); // queenside white rook moves forward
			b.DrawCounter.Should().Be(1, "rook moved up");

			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "original black Rook position");
			b.GetPieceAtPosition(Pos("a2")).PieceType.Should().Be(ChessPieceType.Empty, "original empty space");
			b.CurrentPlayer.Should().Be(1, "undid last move, so it's still white player's turn");
			b.DrawCounter.Should().Be(0, "only pawns have moved so far since rook move was undone");

			Apply(b, Move("(h1, h3)")); // kingside white rook moves forwards
			b.DrawCounter.Should().Be(1, "rook moved forward");
			b.PositionIsAttacked(Pos("h3"), 2).Should().BeTrue();

			Apply(b, Move("(c8, h3)")); // black bishop captures rook
			b.CurrentAdvantage.Should().Be(Advantage(2, 5), "black bishop captured rook");
			b.DrawCounter.Should().Be(0, "a capture was made, so counter was reset");

			// Move black and white rooks
			Apply(b, Move("(a1, a2)")); // white piece
			Apply(b, Move("(a8, a7)")); // black piece
			Apply(b, Move("(a2, a1)")); // white piece
			Apply(b, Move("(a7, a8)")); // black piece
			Apply(b, Move("(a1, a2)")); // white piece
			Apply(b, Move("(a8, a7)")); // black piece
			Apply(b, Move("(a2, a1)")); // white piece
			Apply(b, Move("(a7, a8)")); // black piece
			Apply(b, Move("(a1, a2)")); // white piece
			Apply(b, Move("(a8, a7)")); // black piece
			b.DrawCounter.Should().Be(10, "10 moves made without pawn or capture");

			// Move black and white knights
			Apply(b, Move("(b1, a3)")); // white piece
			Apply(b, Move("(b8, d7)")); // black piece
			Apply(b, Move("(a3, b1)")); // white piece
			Apply(b, Move("(d7, b8)")); // black piece
			Apply(b, Move("(b1, a3)")); // white piece
			Apply(b, Move("(b8, d7)")); // black piece
			Apply(b, Move("(a3, b1)")); // white piece
			Apply(b, Move("(d7, b8)")); // black piece
			Apply(b, Move("(b1, a3)")); // white piece
			Apply(b, Move("(b8, d7)")); // black piece
			b.DrawCounter.Should().Be(20, "20 moves made without pawn or capture");

			// Move black and white queens
			Apply(b, Move("(d1, h5)")); // white piece
			Apply(b, Move("(d8, c8)")); // black piece
			Apply(b, Move("(h5, d1)")); // white piece
			Apply(b, Move("(c8, d8)")); // black piece
			Apply(b, Move("(d1, h5)")); // white piece
			Apply(b, Move("(d8, c8)")); // black piece
			Apply(b, Move("(h5, d1)")); // white piece
			Apply(b, Move("(c8, d8)")); // black piece
			Apply(b, Move("(d1, h5)")); // white piece
			Apply(b, Move("(d8, c8)")); // black piece
			b.DrawCounter.Should().Be(30, "30 moves made without pawn or capture");

			// Move black and white bishops
			Apply(b, Move("(f1, d3)")); // white piece
			Apply(b, Move("(f8, h6)")); // black piece
			Apply(b, Move("(d3, f1)")); // white piece
			Apply(b, Move("(h6, f8)")); // black piece
			Apply(b, Move("(f1, d3)")); // white piece
			Apply(b, Move("(f8, h6)")); // black piece
			Apply(b, Move("(d3, f1)")); // white piece
			Apply(b, Move("(h6, f8)")); // black piece
			Apply(b, Move("(f1, d3)")); // white piece
			Apply(b, Move("(f8, h6)")); // black piece
			b.DrawCounter.Should().Be(40, "40 moves made without pawn or capture");

			// Move black and white kings
			Apply(b, Move("(e1, d1)")); // white piece
			Apply(b, Move("(e8, d8)")); // black piece
			Apply(b, Move("(d1, e1)")); // white piece
			Apply(b, Move("(d8, e8)")); // black piece
			Apply(b, Move("(e1, d1)")); // white piece
			Apply(b, Move("(e8, d8)")); // black piece
			Apply(b, Move("(d1, e1)")); // white piece
			Apply(b, Move("(d8, e8)")); // black piece
			Apply(b, Move("(e1, d1)")); // white piece
			Apply(b, Move("(e8, d8)")); // black piece
			b.DrawCounter.Should().Be(50, "50 moves made without pawn or capture");

			// Move black and white rooks
			Apply(b, Move("(a2, a1)")); // white piece
			Apply(b, Move("(a7, a8)")); // black piece
			Apply(b, Move("(a1, a2)")); // white piece
			Apply(b, Move("(a8, a7)")); // black piece
			Apply(b, Move("(a2, a1)")); // white piece
			Apply(b, Move("(a7, a8)")); // black piece
			Apply(b, Move("(a1, a2)")); // white piece
			Apply(b, Move("(a8, a7)")); // black piece
			Apply(b, Move("(a2, a1)")); // white piece
			Apply(b, Move("(a7, a8)")); // black piece
			b.DrawCounter.Should().Be(60, "60 moves made without pawn or capture");

			// Move black and white queens
			Apply(b, Move("(h5, e2)")); // white piece
			Apply(b, Move("(c8, b8)")); // black piece
			Apply(b, Move("(e2, h5)")); // white piece
			Apply(b, Move("(b8, c8)")); // black piece
			Apply(b, Move("(h5, e2)")); // white piece
			Apply(b, Move("(c8, b8)")); // black piece
			Apply(b, Move("(e2, h5)")); // white piece
			Apply(b, Move("(b8, a7)")); // black piece
			Apply(b, Move("(h5, e2)")); // white piece
			Apply(b, Move("(a7, b6)")); // black piece
			b.DrawCounter.Should().Be(70, "70 moves made without pawn or capture");

			// Move black and white knights
			Apply(b, Move("(a3, b1)")); // white piece
			Apply(b, Move("(d7, b8)")); // black piece
			Apply(b, Move("(b1, a3)")); // white piece
			Apply(b, Move("(b8, d7)")); // black piece
			Apply(b, Move("(a3, b1)")); // white piece
			Apply(b, Move("(d7, b8)")); // black piece
			Apply(b, Move("(b1, a3)")); // white piece
			Apply(b, Move("(b8, d7)")); // black piece
			Apply(b, Move("(a3, b1)")); // white piece
			Apply(b, Move("(d7, b8)")); // black piece
			b.DrawCounter.Should().Be(80, "80 moves made without pawn or capture");

			// Move black and white bishops
			Apply(b, Move("(d3, b5)")); // white piece
			Apply(b, Move("(h6, f8)")); // black piece
			Apply(b, Move("(b5, d3)")); // white piece
			Apply(b, Move("(f8, h6)")); // black piece
			Apply(b, Move("(d3, b5)")); // white piece
			Apply(b, Move("(h6, f8)")); // black piece
			Apply(b, Move("(b5, d3)")); // white piece
			Apply(b, Move("(f8, h6)")); // black piece
			Apply(b, Move("(d3, b5)")); // white piece
			Apply(b, Move("(h6, f8)")); // black piece
			b.DrawCounter.Should().Be(90, "90 moves made without pawn or capture");

			// Move black and white kings
			Apply(b, Move("(d1, e1)")); // white piece
			Apply(b, Move("(d8, c8)")); // black piece
			Apply(b, Move("(e1, d1)")); // white piece
			Apply(b, Move("(c8, d8)")); // black piece
			Apply(b, Move("(d1, e1)")); // white piece
			Apply(b, Move("(d8, c8)")); // black piece
			Apply(b, Move("(e1, d1)")); // white piece
			Apply(b, Move("(c8, d8)")); // black piece
			Apply(b, Move("(d1, e1)")); // white piece
			Apply(b, Move("(d8, c8)")); // black piece

			b.DrawCounter.Should().Be(100, "100 moves made without pawn or capture");

			b.IsDraw.Should().BeTrue("50 (full) move rule results in a draw");
			b.IsFinished.Should().BeTrue("Draw. Game over.");
		}

		/// <summary>
		/// Two black bishops causing checkmate (white player).
		/// </summary>
		[Fact]
		public void BishopsCausingCheckmate() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",
				"d7, d6",
				"c2, c3",
				"c8, e6",
				"f2, f4",
				"g7, g5",
				"a2, a3",
				"f8, h6",
				"a3, a4",
				"e6, c4",
				"a4, a5",
				"g5, g4",
				"f4, f5",
				"d6, d5",
				"a5, a6",
				"h6, g5",
				"a6, b7", // white pawn captures black pawn
				"g5, h4"
			);

			b.IsCheck.Should().BeTrue("the white king is threatened by the bishop at h4");
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(1, "white pawn must protect king from being captured by black bishop")
				.And.BeEquivalentTo(Move("g2, g3"));
			Apply(b, "g2, g3");
			Apply(b, "d8, d6");
			Apply(b, "h2, h3");
			Apply(b, "h4, g3");  // black bishop captures white pawn
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "tie");

			possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(0, "checkmate");
			b.IsCheckmate.Should().BeTrue("white's king is in checkmate from bishop at h3 and bishop in c4");
			b.IsFinished.Should().BeTrue("checkmate. game over.");
		}

		[Fact]
		public void KingStalemate() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("a7"), ChessPieceType.King, 2,
				 Pos("b1"), ChessPieceType.King, 1,
				 Pos("b2"), ChessPieceType.Rook, 1,
				 Pos("c6"), ChessPieceType.Rook, 1);
			Apply(b, Move("c6, c7"));
			Apply(b, Move("a7, a8"));
			Apply(b, Move("b2, b7"));
			var pos = b.GetPossibleMoves();
			pos.Should().BeEmpty("Stalemate should not have possible moves for pieces");
			var KingMoves = GetMovesAtPosition(pos, Pos("a8"));
			KingMoves.Should().HaveCount(0, "King should not be able to move anywhere");
			b.PositionIsAttacked(Pos("a8"), 1).Should().BeFalse("The square that the king is on should be not attacked");
			b.IsFinished.Should().BeTrue("Game should be finished in the stalemate state");
			b.IsCheckmate.Should().BeFalse("King should not be in checkmate since the square the king is on is not attacked");
		}

		// checks that if the king is in checkmate that there are no possible moves and that the game is over
		[Fact]
		public void checkmate_moves() {
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"e7, e5",
				"g2, g4",
				"d8, h4"
			);
			var possible = b.GetPossibleMoves();
			b.IsFinished.Should().BeTrue("the game is finished");
			b.IsCheckmate.Should().BeTrue("the king has no escape");
			possible.Count().Should().Be(0, "There should be 0 possible moves to cancel the checking of black's king");

		}

		[Fact]
		public void FoolsMate() {
			ChessBoard b = CreateBoardFromMoves(
				"g2, g4",
				"e7, e5",
				"f2, f3",
				"d8, h4"
			);

			b.IsCheckmate.Should().BeTrue("Checkmate");
			b.IsFinished.Should().BeTrue("Finished");

		}

		[Fact]
		//Test case for checkmate
		public void CheckmateTest() {
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"e7, e6",
				"g2, g4"
			);

			//Check if the move is valid
			var posMoves = b.GetPossibleMoves();
			var checkMate = GetMovesAtPosition(posMoves, Pos("d8"));
			checkMate.Should().Contain(Move("d8, h4"))
				.And.HaveCount(4, "a king can move diagonal");

			//Apply the move to achieve checkmate
			Apply(b, "d8, h4");
			posMoves = b.GetPossibleMoves();
			posMoves.Should().BeEmpty("its a checkmate, queen cannot escape");
			b.IsCheckmate.Should().Be(true, "it is a checkmate");
			b.IsFinished.Should().Be(true, "checkmate, game over");

			//Undo the move and check board state
			b.UndoLastMove();
			posMoves = b.GetPossibleMoves();
			posMoves.Should().NotBeEmpty("undoing a checkmate should create moves");
			b.IsCheckmate.Should().Be(false, "undoing a checkmate");
			b.IsFinished.Should().Be(false, "undoing a checkmate, game still going");
		}

		[Fact]//Tricky Situation(Stalemate)
		public void RooksAndKnightSlatemate() {

			//Place King in stalemate position
			ChessBoard b = CreateBoardFromPositions(
				Pos("c7"), ChessPieceType.King, 1,
				Pos("e8"), ChessPieceType.Rook, 2,
				Pos("e6"), ChessPieceType.Rook, 2,
				Pos("c5"), ChessPieceType.Knight, 2,
				Pos("a1"), ChessPieceType.King, 2
			);


			var possMoves = b.GetPossibleMoves();
			var forKing = GetMovesAtPosition(possMoves, Pos("c7"));

			forKing.Should().HaveCount(0, "No move available");

			b.IsStalemate.Should().BeTrue("Stalemate");

			b.IsFinished.Should().BeTrue("Finished");

		}

		[Fact]
		public void StalemateTest() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("f6"), ChessPieceType.Queen, 2,
				Pos("g8"), ChessPieceType.King, 1,
				Pos("h6"), ChessPieceType.King, 2
			);

			var possMoves = b.GetPossibleMoves();
			GetMovesAtPosition(possMoves, Pos("g8")).Should().HaveCount(0, "king has no possible moves to escape");
			b.IsStalemate.Should().BeTrue("Game cannot be won and has entered stalemate");
			b.IsFinished.Should().BeTrue("No more possible moves");
			b.IsCheckmate.Should().BeFalse("Black pieces cannot checkmate White King");
			b.IsCheck.Should().BeFalse("White King is not threatened by opposing pieces");
		}

		/// <summary>
		/// Testing a draw.
		/// </summary>
		[Fact]
		public void DrawTest() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("b1"), ChessPieceType.King, 1,
				Pos("b8"), ChessPieceType.King, 2
			);

			for (int i = 0; i < 25; i++) {
				Apply(b, Move(Pos("b1"), Pos("b2"), ChessMoveType.Normal));
				Apply(b, Move(Pos("b8"), Pos("c8"), ChessMoveType.Normal));
				Apply(b, Move(Pos("b2"), Pos("b1"), ChessMoveType.Normal));
				Apply(b, Move(Pos("c8"), Pos("b8"), ChessMoveType.Normal));
			}

			var moves = b.DrawCounter;
			moves.Should().Be(100, "Once the total amount of moves is 100, (50 for White, 50 for Black) the game is a draw");
			b.IsDraw.Should().BeTrue("The game has passed 100 moves where no captures have been made");
		}

		/// <summary>
		/// Validate that the board has a stalemate and concludes the game
		/// </summary>
		[Fact]
		public void StalemateTest2() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("d5"), ChessPieceType.King, 1,
				Pos("d7"), ChessPieceType.Pawn, 1,
				Pos("d8"), ChessPieceType.King, 2
			);

			// Move so it's black's turn.
			Apply(b, "d5, d6");
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(0, "black king should have no legal move");
			b.IsStalemate.Should().BeTrue("there is a stalemate");
			b.IsCheckmate.Should().BeFalse("checkmate should be false");
			b.IsFinished.Should().BeTrue("no legal moves means the game is over");
		}

		/// <summary>
		/// TestCheckmateScenarios
		/// # Test Boden's Checkmate.
		/// The king is mated by the two criss-crossing bishops, and blocked by two friendly pieces
		/// 
		/// # Test Two Pawn Checkmate
		/// This is a popular common endgame, where one side has two pawns and the other side has none.
		/// In this scenario the white King has nowhere to move
		/// </summary>
		/// 
		[Fact]
		public void BodensCheckmateScenario() {
			/// Test Boden's Checkmate on Black Player
			ChessBoard cBoard = CreateBoardFromPositions(
				Pos("d1"), ChessPieceType.King, 1,
				Pos("a6"), ChessPieceType.Bishop, 1,
				Pos("h2"), ChessPieceType.Bishop, 1,
				Pos("d7"), ChessPieceType.Knight, 2,
				Pos("d8"), ChessPieceType.Rook, 2,
				Pos("c8"), ChessPieceType.King, 2
				);

			cBoard.IsCheckmate.Should().BeFalse("King should not be on checkmate");
			cBoard.IsFinished.Should().BeFalse();

			Apply(cBoard, "h2, g3");

			cBoard.IsCheckmate.Should().BeTrue("King should be on checkmate");
			cBoard.IsFinished.Should().BeTrue();
		}

		[Fact]
		public void QueenCausingCheckmate() {
			ChessBoard b = CreateBoardFromMoves(
				"g2, g4", //white pawn moves 2
				"e7, e6", //black pawn moves 1
				"f2, f4", //white pawn moves 2
				"d8, h4"); //black queen moves diagonal 
			var possMoves = b.GetPossibleMoves();
			GetMovesAtPosition(possMoves, Pos("0, 4")).Should().HaveCount(0, "king at D1 cannot move anywhere");
			b.IsCheckmate.Should().BeTrue("the king has no escape");
			b.IsFinished.Should().BeTrue("the game is over");
		}

		[Fact]
		private void TwoPawnCheckmateScenario() {
			// Test Two Pawn Checkmate on White player
			ChessBoard cBoard = CreateBoardFromPositions(
				Pos("d1"), ChessPieceType.King, 1,
				Pos("e3"), ChessPieceType.King, 2,
				Pos("e2"), ChessPieceType.Pawn, 2,
				Pos("f4"), ChessPieceType.Pawn, 2
				);
			Apply(cBoard, "d1, e1");
			Apply(cBoard, "f4, f3");

			cBoard.IsFinished.Should().BeTrue();
			// White King is not under threat, however has not moves left
			cBoard.IsCheckmate.Should().BeFalse();
			var availableMoves = GetMovesAtPosition(cBoard.GetPossibleMoves(), Pos("e1"));
			availableMoves.Should().HaveCount(0);
		}
	}
}
