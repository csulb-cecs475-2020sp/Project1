using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Test {
	public class KnightTests : ChessTest {
		/// <summary>
		/// test for knights possible moves
		/// </summary>
		[Fact]
		public void KnightsPossibleMoves() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("a1"), ChessPieceType.King, 1,
				Pos("b8"), ChessPieceType.King, 2,
				Pos("c7"), ChessPieceType.Knight, 2,
				Pos("h6"), ChessPieceType.Pawn, 2,
				Pos("e6"), ChessPieceType.Pawn, 1,
				Pos("d6"), ChessPieceType.Bishop, 1
				);

			Apply(b, "e6, e7"); //move white pawn up 1
			var possMove = b.GetPossibleMoves();
			var KnightsMove = GetMovesAtPosition(possMove, Pos("c7"));
			KnightsMove.Should().HaveCount(0, "This Knight cannot move because the bishop will put the king in check");
			Apply(b, "h6, h5"); //move black pawn up 1
			Apply(b, "d6, a3"); //move white bishop away from a possible future check
			possMove = b.GetPossibleMoves();
			KnightsMove = GetMovesAtPosition(possMove, Pos("c7"));
			KnightsMove.Should().HaveCount(6, "This Knight can now move to a6, b5, d5, e6, e8 and a8")
				.And.Contain(Move("c7,a6"))
				.And.Contain(Move("c7,b5"))
				.And.Contain(Move("c7,d5"))
				.And.Contain(Move("c7,e6"))
				.And.Contain(Move("c7,e8"))
				.And.Contain(Move("c7,a8"));

		}

		/// <summary>
		/// Ensure that all the possible moves of a black Knight are listed
		/// </summary>
		[Fact]
		public void KnightPossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(
					 "b1, c3",
					 "g8, f6",
					 "f2, f3",
					 "f6, d5",
					 "h2, h4"
			);

			var PossMoves = b.GetPossibleMoves();

			// Get Knights possible moves
			var KnightPossMoves = GetMovesAtPosition(PossMoves, Pos("d5"));

			// Ensure all possible moves for Knight exists
			KnightPossMoves.Should().HaveCount(6, "Knight at d5 should be able to go to b6, b4, c3, e3, f4, f6")
				.And.BeEquivalentTo(Move("d5, b6"), Move("d5, b4"), Move("d5, c3"), Move("d5, e3"), Move("d5, f4"), Move("d5, f6"));
		}

		[Fact]
		public void CheckReportedMoves() {
			ChessBoard b = CreateBoardFromMoves(
				"f2, f4",
				"e7, e5",
				"g2, g3",
				"d8, h4",
				"g3, h4",
				"g8, f6",
				"f4, e5"
			);

			// at this point, the knight can move into 5 positions
			var possMoves = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(possMoves, Pos("f6"));
			expected.Should().Contain(Move("f6, g8"))
				.And.Contain(Move("f6, h5"))
				.And.Contain(Move("f6, g4"))
				.And.Contain(Move("f6, e4"))
				.And.Contain(Move("f6, d5"))
				.And.HaveCount(5, "The knight can move into 5 different places");
		}

		/// <summary>
		/// Verify knight moves
		/// </summary>
		[Fact]
		public void KnightMoves() {
			// Move white knight to center of board
			ChessBoard b = CreateBoardFromMoves(
				"g1, f3",
				"a7, a6",
				"f3, e5",
				"b7, b6"
			);

			var possMoves = b.GetPossibleMoves();
			var knightPossibleMoves = GetMovesAtPosition(possMoves, Pos("e5"));
			knightPossibleMoves.Should().Contain(Move("e5, d7")).And.Contain(Move("e5, f7")).And.Contain(Move("e5, g6")).
				And.Contain(Move("e5, g4")).And.Contain(Move("e5, f3")).And.Contain(Move("e5, d3")).And.Contain(Move("e5, c4")).
				And.Contain(Move("e5, c6"), "Knight has 8 possible moves");
		}


		/// <summary>
		/// Knights may still move when completely surrounded
		/// </summary>
		[Fact]
		public void KnightJump() {
			ChessBoard b = CreateBoardFromPositions(
				Pos("e4"), ChessPieceType.Knight, 2,
				Pos("e5"), ChessPieceType.Pawn, 1,
				Pos("f5"), ChessPieceType.Pawn, 1,
				Pos("f4"), ChessPieceType.Pawn, 1,
				Pos("f2"), ChessPieceType.Pawn, 1,
				Pos("e3"), ChessPieceType.Pawn, 1,
				Pos("d3"), ChessPieceType.Pawn, 1,
				Pos("d4"), ChessPieceType.Pawn, 1,
				Pos("d5"), ChessPieceType.Pawn, 1,
				Pos("a8"), ChessPieceType.King, 1,
				Pos("h1"), ChessPieceType.King, 2

			);

			Apply(b, "f2, f3"); //blacks turn
			var possMoves = b.GetPossibleMoves();

			var knightMoves = GetMovesAtPosition(possMoves, Pos("e4"));

			knightMoves.Should().Contain(Move("e4, f6"));//checks to make sure knight can still jump while surrounded
			knightMoves.Should().Contain(Move("e4, g5"));
			knightMoves.Should().Contain(Move("e4, g3"));
			knightMoves.Should().Contain(Move("e4, f2"));
			knightMoves.Should().Contain(Move("e4, d2"));
			knightMoves.Should().Contain(Move("e4, c3"));
			knightMoves.Should().Contain(Move("e4, c5"));
			knightMoves.Should().Contain(Move("e4, d6"));
		}

		[Fact]
		public void BlackKnightCaptureAndPossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(
				"h2, h4",
				"g8, f6",
					 "h4, h5"
			);

			var possible = b.GetPossibleMoves();
			var forKnight = GetMovesAtPosition(possible, Pos("f6"));
			forKnight.Should().HaveCount(5, "a knight can capture white pawn or move to four other positions")
				 .And.BeEquivalentTo(Move("f6, h5"), Move("f6, g8"), Move("f6, d5"), Move("f6, e4"), Move("f6, g4"));

			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no operations have changed the advantage");

			//Capture piece
			Apply(b, Move("f6, h5"));
			b.GetPieceAtPosition(Pos("h5")).Player.Should().Be(2, "Player 2 captured Player 1's pawn from knight");
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "White lost a single pawn of 1 value");
		}

		/// <summary>
		/// Check all the Movement of the Knights 
		/// Depending on the Player's turn
		/// Then, apply one move, and see if Player's turn is changed
		/// And knight's possible moves
		/// </summary>
		[Fact]
		public void AllKnightMovement() {
			ChessBoard b = CreateBoardFromMoves(
				"g1, h3",
				"e7, e5",
				"b1, c3",
				"d7, d5",
				"h1, g1",
				"g8, f6"
			);

			var possMoves = b.GetPossibleMoves();

			///Check all of the horses possible moves

			//WHITE PIECES PLAYER 1
			//Player 1 horse: c3
			b.GetPieceAtPosition(Pos("c3")).PieceType.Should().Be(ChessPieceType.Knight, "Knight is at position c3");
			var c3HorsePossMoves = GetMovesAtPosition(possMoves, Pos("c3"));
			c3HorsePossMoves.Should().Contain(Move("c3, b5"))
				.And.Contain(Move("c3, d5"))
				.And.Contain(Move("c3, e4"))
				.And.Contain(Move("c3, b1"))
				.And.Contain(Move("c3, a4"))
				.And.HaveCount(5, "Horse at c3 has 5 possible moves");
			//Player 1 horse: h3
			b.GetPieceAtPosition(Pos("h3")).PieceType.Should().Be(ChessPieceType.Knight, "Knight is at position h3");
			var h3HorsePossMoves = GetMovesAtPosition(possMoves, Pos("h3"));
			h3HorsePossMoves.Should().Contain(Move("h3, g5"))
				.And.Contain(Move("h3, f4"))
				.And.HaveCount(2, "Horse at h3 has 3 possible moves");


			//BLACK PIECES PLAYER 2
			//Player 1 horse: c8
			b.GetPieceAtPosition(Pos("b8")).PieceType.Should().Be(ChessPieceType.Knight, "Knight is at position b8");
			var b8HorsePossMoves = GetMovesAtPosition(possMoves, Pos("b8"));
			b8HorsePossMoves.Should().HaveCount(0, "Player 1 has not made his turn yet");

			//Player 1 horse: f6
			b.GetPieceAtPosition(Pos("f6")).PieceType.Should().Be(ChessPieceType.Knight, "Knight is at position f6");
			var f6HorsePossMoves = GetMovesAtPosition(possMoves, Pos("f6"));
			f6HorsePossMoves.Should().HaveCount(0, "Player 1 has not made his turn yet");

			//apply move so that next player's turn
			Apply(b, Move("g2, g3"));
			//need to get the new possible move set
			possMoves = b.GetPossibleMoves();
			b.CurrentPlayer.Should().Be(2, "Player 1 moved pawns: it should not be Player 2's turn");

			//c3 knights cannot move because it is Player 2's turn
			c3HorsePossMoves = GetMovesAtPosition(possMoves, Pos("c3"));
			c3HorsePossMoves.Should().BeEmpty().And.HaveCount(0, "It is currently Player 2's turn");
			//c3 knights cannot move because it is Player 2's turn
			h3HorsePossMoves = GetMovesAtPosition(possMoves, Pos("h3"));
			h3HorsePossMoves.Should().BeEmpty().And.HaveCount(0, "It is currently Player 2's turn");

			//Player 2 knights: b8 
			//Check for Player 2 knight's possible moves
			b8HorsePossMoves = GetMovesAtPosition(possMoves, Pos("b8"));
			b8HorsePossMoves.Should().Contain(Move("b8, a6"))
				.And.Contain(Move("b8,c6"))
				.And.Contain(Move("b8,d7"))
				.And.HaveCount(3, "Horse at b8 has Two possible moves");
			//Player 2 knights: 6
			f6HorsePossMoves = GetMovesAtPosition(possMoves, Pos("f6"));
			f6HorsePossMoves.Should().Contain(Move("f6, g8"))
				.And.Contain(Move("f6, d7"))
				.And.Contain(Move("f6, e4"))
				.And.Contain(Move("f6, g4"))
				.And.Contain(Move("f6, h5"))
				.And.HaveCount(5, "Horse at f6 has Two possible moves");
		}

		/// <summary>
		/// Make somme moves so that Horse captures Queen
		/// and thenn undo to check for the correct board States
		/// </summary>
		[Fact]
		public void HorseCaptureQueen() {
			ChessBoard b = CreateBoardFromMoves(
				"b1, a3",
				"d7, d5",
				"a3, c4",
				"d8, d6"
			);

			//get moves of the horse
			var poss = b.GetPossibleMoves();
			var horseMoves = GetMovesAtPosition(poss, Pos("c4"));

			//horse should have about 6 moves that it can make
			horseMoves.Should().Contain(Move("c4, d6"))
				.And.Contain(Move("c4, b6"))
				.And.Contain(Move("c4, a5"))
				.And.Contain(Move("c4, e3"))
				.And.Contain(Move("c4, e5"))
				.And.Contain(Move("c4, a3"))
				.And.HaveCount(6, "a horse moves in a L shape, can capture Queen");

			//horse will make a move and capture Queen
			Apply(b, Move("c4, d6"));

			//Check the advantage, as only queen is capturedmm, then Player 1 needs to have an advantage of 1
			b.CurrentAdvantage.Should().Be(Advantage(1, 9), "Queen has a value of 9");
			//Now It should be Player 2's turn
			b.CurrentPlayer.Should().Be(2, "It should be Player 2's turn now");

			//after capturing check if the horse piece has moved
			b.GetPieceAtPosition(Pos("d6")).PieceType.Should().Be(ChessPieceType.Knight, "Knight Captured Queen, takes over d6 spot");
			b.GetPieceAtPosition(Pos("c4")).Player.Should().Be(0, "Horse has moved away from c4, should be empty");

			//undo the move where it captured the Queen
			b.UndoLastMove();

			//check the state before the first move
			b.CurrentPlayer.Should().Be(1, "It should be Player 1's turn now");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "Undo happened, Queen has yet to be take, no advantage has been created");
			b.GetPieceAtPosition(Pos("c4")).PieceType.Should().Be(ChessPieceType.Knight, "Undo happened, Knight should now be in c4 spot");
			b.GetPieceAtPosition(Pos("d6")).PieceType.Should().Be(ChessPieceType.Queen, "Undo happened, Queen should now be in d6 spot");
		}

		[Fact]
		//Test GetPossibleMoves using knight's moves
		public void TestGetPossibleMoves() {
			ChessBoard b = new ChessBoard();

			//Check the initial board state possible moves for knights
			var posMoves = b.GetPossibleMoves();
			posMoves.Should().HaveCount(20);
			var movesAtPos = GetMovesAtPosition(posMoves, Pos("b1"));
			movesAtPos.Should().Contain(Move("b1, a3"), "knight can move in L shape to open positions")
				.And.Contain(Move("b1, c3"), "knight can move in L shape to open positions");
			movesAtPos = GetMovesAtPosition(posMoves, Pos("g1"));
			movesAtPos.Should().Contain(Move("g1, f3"), "knight can move in L shape to open positions")
				.And.Contain(Move("g1, h3"), "knight can move in L shape to open positions");

			//Apply a knight move
			Apply(b, "b1, c3");

			//Check if the turn is marked correctly
			b.CurrentPlayer.Should().Be(2, "white made a move, its black turn");

			//Move a pawn in a location where it can be captured by the knight
			Apply(b, "d7, d5");
			posMoves = b.GetPossibleMoves();
			movesAtPos = GetMovesAtPosition(posMoves, Pos("c3"));
			movesAtPos.Should().HaveCount(5, "knight can capture pawn or make 4 other L shape moves, including moving back")
				.And.BeEquivalentTo(Move("c3, b1"), Move("c3, a4"), Move("c3, b5"), Move("c3, d5"), Move("c3, e4"));
		}

		[Fact]
		public void knightMovementValidation() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e3",
				"f7, f6"
				);

			//test ability to jump over blocking pieces
			var possMoves = b.GetPossibleMoves();
			GetMovesAtPosition(possMoves, Pos("g1")).Should().HaveCount(3, "knight has 3 possible moves ")
								.And.Contain(Move("g1, h3"))
								.And.Contain(Move("g1, e2"))
								.And.Contain(Move("g1, f3"));
			Apply(b, "g1, h3");
			Apply(b, "h7, h5");

			possMoves = b.GetPossibleMoves();
			//test backwards movement
			//should have 3 potential moves but resulted in 0
			GetMovesAtPosition(possMoves, Pos("h3")).Should().HaveCount(3, "knight has 3 possible moves ")
								.And.Contain(Move("h3, f4"))
								.And.Contain(Move("h3, g1"))
								.And.Contain(Move("h3, g5"));
			Apply(b, "h3, g1");
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "White's Knight at position (6, 0)");
			b.CurrentPlayer.Should().Be(2, "undoing the Knight's move from h3 to g1 will allow player two to make the next move");

		}

		/// <summary>
		/// Checking the possible moves of a knight. Tests white knight.
		/// </summary>
		// 017844266 % 5 + 1 = 2 -> knight
		[Fact]
		public void ValidateKnightPossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(
				"b1, c3",
				"c7, c5",
				"c3, b5",
				"e7, e5",
				"b5, d4",
				"g7, g5"
			);

			var whiteKnight = b.GetPieceAtPosition(Pos("d4"));
			whiteKnight.PieceType.Should().Be(ChessPieceType.Knight, "A Knight should be at d4.");
			whiteKnight.Player.Should().Be(1, "Should be a White Knight at d4.");

			var possMoves = b.GetPossibleMoves();
			var knightMoves = GetMovesAtPosition(possMoves, Pos("d4"));

			//c2 and e2 should be blocked by friendly pawns
			knightMoves.Should().HaveCount(6, "The White Knight should have 6 possible moves.")
				.And.Contain(Move("d4, e6")).And.Contain(Move("d4, f5"))
				.And.Contain(Move("d4, f3"))
				.And.Contain(Move("d4, b3"))
				.And.Contain(Move("d4, b5")).And.Contain(Move("d4, c6"));

		}

		//Checks the knight moves for black pieces
		[Fact]
		public void KnightsAtInitialBoard() {
			ChessBoard cBoard = new ChessBoard();

			var possMoves = cBoard.GetPossibleMoves();

			var initialWhiteKnightLocations = new List<Cecs475.BoardGames.Model.BoardPosition> {
				Pos("b1"), Pos("g1") };

			var initialBlackKnightLocations = new List<Cecs475.BoardGames.Model.BoardPosition> {
				Pos("b8"), Pos("g8") };

			foreach (var boardPosition in initialWhiteKnightLocations) {
				// check for chess piece type to be  a knight
				cBoard.GetPieceAtPosition(boardPosition).PieceType.Should().Be(ChessPieceType.Knight);

				// get available moves for the position elements
				var availableMoves = GetMovesAtPosition(possMoves, boardPosition);
				availableMoves.Should().BeEquivalentTo(
						Move(boardPosition, boardPosition.Translate(-2, -1)),
						Move(boardPosition, boardPosition.Translate(-2, 1))
					).And.HaveCount(2, "a white knight should be able to jump over pawns and get only two possible moves in the initial board");
			}

			// White Player makes the first move 
			Apply(cBoard, "a2, a4");

			// Update possible moves list
			possMoves = cBoard.GetPossibleMoves();

			foreach (var boardPosition in initialBlackKnightLocations) {
				// check for chess piece type to be  a knight
				cBoard.GetPieceAtPosition(boardPosition).PieceType.Should().Be(ChessPieceType.Knight);

				// get available moves for the position elements
				var availableMoves = GetMovesAtPosition(possMoves, boardPosition);
				availableMoves.Should().BeEquivalentTo(
						Move(boardPosition, boardPosition.Translate(2, -1)),
						Move(boardPosition, boardPosition.Translate(2, 1))
					).And.HaveCount(2, "a black knight should be able to jump over pawns and get only two possible moves in the initial board");
			}
		}
	}
}
