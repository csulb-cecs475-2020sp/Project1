using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class NewBoardPositionTests : ChessTest {
		[Fact]
		public void CheckInitialgGamebaordState() {
			ChessBoard board = new ChessBoard();
			foreach (var pos in GetPositionsInRank(2)) {
				board.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Pawn, "There should be a " + ChessPieceType.Pawn.ToString() + " at position " + pos.ToString());

			}
		}

		//checks initial board state
		[Fact]
		public void initialWhiteBoardStateCheck() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "White Rook should be in the bottom left of the board");
			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Rook, "White Rook should be in the bottom right of the board");
			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "White King should be in the center left of the board");
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Queen, "White Queen should be in the center right of the board");
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Bishop, "White Bishop should be in the left of the King");
			b.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.Bishop, "White Bishop should be in the right of the Queen");
			b.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Knight, "White Knight should be in the left of the Bishop");
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "White Knight should be in the right of the Bishop");
		}

		/// <summary>
		/// Checking that the queens are across from each other on an initial board 
		/// (checks both white and black queens)
		/// </summary>
		[Fact]
		public void InitialQueenTest() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Queen, "White's queen at position d1");
			b.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Queen, "Black's queen at position d8");
		}

		//in a chess board all front pieces should be pawns, therefore by iterating through every column at row 2, all pieeces should be pawns
		[Fact]
		public void InitialStateTest() {
			ChessBoard b = new ChessBoard();
			/* cool but no used
			 * for (int i = 1; i <= 8; i++)
			{
				char a = (char)('a' + i);
				b.GetPieceAtPosition(Pos(a + "2")).PieceType.Should().Be(ChessPieceType.Pawn);
			}
			 */
			foreach (var pos in GetPositionsInRank(2))
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Pawn, "because this test case is during the initial state, this piece " +
					"should be a pawn");
		}


		/// <summary>
		/// Simple facts about "new" boards.
		/// </summary>
		[Fact]
		public void NewChessBoard5() {
			ChessBoard b = new ChessBoard();
			//Test for black's rooks starting positions
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Rook, "Black's Rook at position (0,0)");
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Rook, "Black's Rook at position (0,7)");

			//Test for white and black's Queen starting positions

			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Black's Rook at position (0,3)");
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "White's Rook at position (7,3)");

			//Other properties of a starting state 
			b.DrawCounter.Should().Be(0);

		}
		/// <summary>
		/// Check all starting positions of white pieces that are not pawns. Verify that the pieces are in the correct positions
		/// </summary>
		[Fact]
		public void CheckStartingPosition() {
			ChessBoard chess = new ChessBoard();
			chess.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "Piece at A1 should be a Rook");
			chess.GetPieceAtPosition(Pos("a1")).Player.Should().Be(1, "Rook piece at a1 must be a white player");
			chess.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Knight, "Piece at B1 should be a Knight");
			chess.GetPieceAtPosition(Pos("b1")).Player.Should().Be(1, "Knight piece at b1 must be a white player");
			chess.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Bishop, "Piece at C1 should be a Bishop");
			chess.GetPieceAtPosition(Pos("c1")).Player.Should().Be(1, "Bishop piece at c1 must be a white player");
			chess.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Queen, "Piece at D1 should be a Queen");
			chess.GetPieceAtPosition(Pos("d1")).Player.Should().Be(1, "Queen piece at d1 must be a white player");
			chess.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "Piece at E1 should be a King");
			chess.GetPieceAtPosition(Pos("e1")).Player.Should().Be(1, "King piece at e1 must be a white player");
			chess.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.Bishop, "Piece at F1 should be a Bishop");
			chess.GetPieceAtPosition(Pos("f1")).Player.Should().Be(1, "Bishop piece at f1 must be a white player");
			chess.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "Piece at G1 should be a Knight");
			chess.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "Piece at g1 should be a Knight");
			chess.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Rook, "Piece at H1 should be a Rook");
			chess.GetPieceAtPosition(Pos("h1")).Player.Should().Be(1, "Rook piece at h1 must be a white player");
		}

		[Fact]
		public void NewBoard() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos(7, 7)).Player.Should().Be(1, "Player 1 should be in lower left of board");
			b.GetPieceAtPosition(Pos(0, 7)).Player.Should().Be(2, "Player 2 should be in upper left of board");
			b.GetPieceAtPosition(Pos(4, 7)).Player.Should().Be(0, "Middle left of board should be empty");
			// Test a few select piece locations.
			b.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.Queen, "White's queen at position (7,4)");
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Black's queen at position (0,4)");
			b.GetPieceAtPosition(Pos(7, 1)).PieceType.Should().Be(ChessPieceType.Knight, "White's knight at position (0,4)");
			b.GetPieceAtPosition(Pos(0, 1)).PieceType.Should().Be(ChessPieceType.Knight, "Black's knight at position (0,4)");
		}

		/// <summary>
		/// Ensure all pieces are in their appropriate spots on board creation
		/// </summary>
		[Fact]
		public void InitialPiecePositions() {
			ChessBoard b = new ChessBoard();

			// List of positions where all Rooks start
			List<BoardPosition> RookPositions = new List<BoardPosition>
			{
				Pos("a1"),
				Pos("a8"),
				Pos("h1"),
				Pos("h8"),
			};

			// List of positions where all Knights start
			List<BoardPosition> KnightPositions = new List<BoardPosition>
			{
				Pos("b1"),
				Pos("b8"),
				Pos("g1"),
				Pos("g8"),
			};

			// List of positions where all Bishops start
			List<BoardPosition> BishopPositions = new List<BoardPosition>
			{
				Pos("c1"),
				Pos("c8"),
				Pos("f1"),
				Pos("f8"),
			};

			// Positions where the Queens start
			List<BoardPosition> QueenPositions = new List<BoardPosition>
			{
				Pos("d1"),
				Pos("d8"),
			};

			// Positions where the Kings start
			List<BoardPosition> KingPositions = new List<BoardPosition>
			{
				Pos("e1"),
				Pos("e8"),
			};

			// Check white Pawn starting positions
			foreach (var pos in GetPositionsInRank(2)) {
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Pawn, "the second rank should be Pawns");
			}

			// Check black Pawn starting positions
			foreach (var pos in GetPositionsInRank(7)) {
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Pawn, "the seventh rank should be Pawns");
			}

			// Check Rook starting positions
			foreach (var pos in RookPositions) {
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Rook, "the pieces at these positions should be Rooks");
			}

			// Check Knight starting positions
			foreach (var pos in KnightPositions) {
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Knight, "the pieces at these positions should be Knights");
			}

			// Check Bishop starting positions
			foreach (var pos in BishopPositions) {
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Bishop, "the pieces at these positions should be Bishops");
			}

			// Check Queen starting positions
			foreach (var pos in QueenPositions) {
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Queen, "the pieces at these positions should be Queens");
			}

			// Check King starting positions
			foreach (var pos in KingPositions) {
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.King, "the pieces at these positions should be Kings");
			}

			// Check the middle ranks to make sure they are Empty
			for (int i = 3; i <= 6; i++) {
				foreach (var pos in GetPositionsInRank(i)) {
					b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Empty, "the pieces at the middle of the bored should be Empty");
				}
			}
		}

		[Fact]
		// Initial starting state
		// Check if the queens are in the correct positions (black at 0,4 and white at 7,4)
		public void CheckInitialQueenPositions() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Black queen should be at position (0, 5)");
			b.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.Queen, "White queen should be at position (7, 5)");
		}


		/// <summary>
		/// Checking game start piece placements
		/// </summary>
		[Fact]
		public void Player1GameStartPiecePlacements() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "White rook at a1");
			b.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Knight, "White knight at b1");
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Bishop, "White bishop at c1");
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Queen, "White queen at d1");
			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "White King at e1");
			b.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.Bishop, "White bishop at f1");
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "White knight at g1");
			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Rook, "White rook at h1");
		}

		/// <summary>
		/// Checks all Pawn spawn placements on new board.
		/// </summary>
		[Fact]
		public void CheckPawnSpawn() {
			ChessBoard b = new ChessBoard();
			List<string> pawn_columns = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h" };
			List<string> pawn_rows = new List<string>() { "2", "7" };
			foreach (string row in pawn_rows) {
				foreach (string column in pawn_columns) {
					b.GetPieceAtPosition(Pos(column + row)).PieceType.Should().Be(ChessPieceType.Pawn, "pawns should spawn in all squares on rows 2 and 7");
				}
			}
		}

		/// <summary>
		/// Check initial starting board state, all black pieces in rank 8 are correct
		/// </summary>
		[Fact]
		public void NewChessBoard4() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos(0, 7)).Player.Should().Be(2, "Player 2 should be in upper right of board");
			b.GetPieceAtPosition(Pos(7, 7)).Player.Should().Be(1, "Player 1 should be in lower right of board");
			b.GetPieceAtPosition(Pos(4, 7)).Player.Should().Be(0, "Middle right of board should be empty");
			// Test all black pieces in rank 8 are correct
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Rook, "Black's rook at position (7,0)");
			b.GetPieceAtPosition(Pos(0, 1)).PieceType.Should().Be(ChessPieceType.Knight, "Black's knight at position (0,1)");
			b.GetPieceAtPosition(Pos(0, 2)).PieceType.Should().Be(ChessPieceType.Bishop, "Black's bishop at position (0,2)");
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Black's queen at position (0,3)");
			b.GetPieceAtPosition(Pos(0, 4)).PieceType.Should().Be(ChessPieceType.King, "Black's king at position (0,4)");
			b.GetPieceAtPosition(Pos(0, 5)).PieceType.Should().Be(ChessPieceType.Bishop, "Black's bishop at position (0,5)");
			b.GetPieceAtPosition(Pos(0, 6)).PieceType.Should().Be(ChessPieceType.Knight, "Black's king at position (0,6)");
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.Rook, "Black's rook at position (0,7)");
			// Test other properties
			b.DrawCounter.Should().Be(0, "no operations have changed the counter");
		}

		[Fact]
		//This will check the initial state of the board.
		//It will check if some pieces were initialized in the correct spot
		public void CheckInitialBoardState() {
			ChessBoard board = new ChessBoard();

			board.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "The piece should be a rook.");
			board.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Queen, "This piece should be a Queen");
			board.GetPieceAtPosition(Pos("d4")).PieceType.Should().Be(ChessPieceType.Empty, "This piece should be empty.");
		}

		/*1. Check Initial Board State*/
		[Fact]
		public void InitialState2() {
			ChessBoard b = new ChessBoard();
			b.GetPlayerAtPosition(Pos(4, 3)).Should().Be(0, "No players should be in this space at the start of the game");
			b.GetPlayerAtPosition(Pos(2, 6)).Should().Be(0, "No players should be in this space at the start of the game");
			b.GetPieceAtPosition(Pos(7, 0)).PieceType.Should().Be(ChessPieceType.Rook, "Black's rook start at position (7,0)");
			b.GetPieceAtPosition(Pos(7, 7)).PieceType.Should().Be(ChessPieceType.Rook, "Black's rook start at position (7,7)");
			b.GetPieceAtPosition(Pos(6, 4)).PieceType.Should().Be(ChessPieceType.Pawn, "Black's pawn start at position (6,4)");
			b.GetPieceAtPosition(Pos(6, 3)).PieceType.Should().Be(ChessPieceType.Pawn, "Black's pawn start at position (6,3)");
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Rook, "White's rook start at position (0,0)");
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.Rook, "White's rook start at position (0,7)");
			b.GetPieceAtPosition(Pos(1, 4)).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn start at position (1,4)");
			b.GetPieceAtPosition(Pos(1, 3)).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn start at position (1,3)");

		}

		[Fact]
		public void InitialBoardTest() {
			ChessBoard b = new ChessBoard();

			//check for rooks and knights at the corners and their players
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Rook);
			b.GetPieceAtPosition(Pos("h8")).Player.Should().Be(2, "Black Rook");
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Knight);
			b.GetPieceAtPosition(Pos("g8")).Player.Should().Be(2, "Black Knight");
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook);
			b.GetPieceAtPosition(Pos("a1")).Player.Should().Be(1, "White Rook");
			b.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Knight);
			b.GetPieceAtPosition(Pos("b1")).Player.Should().Be(1, "White Knight");

			//check for empty spots and pawns because pawns can never be in the first row...
			b.GetPieceAtPosition(Pos("e6")).PieceType.Should().Be(0, "This spot should be empty at the start");
			b.GetPieceAtPosition(Pos("e7")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn");
			b.GetPieceAtPosition(Pos("c3")).PieceType.Should().Be(0, "This spot should be empty at the start");
			b.GetPieceAtPosition(Pos("c2")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn");
		}

		

		//Checks to make sure the four middle rows of the starting board are empty
		[Fact]
		public void NewBoard_MiddleFourRowsEmpty() {

			ChessBoard b = new ChessBoard();

			for (int i = 2; i < 6; i++) {

				for (int j = 0; j < 8; j++) {

					b.GetPieceAtPosition(Pos(i, j)).Player.Should().Be(0, "Middle of the board should be empty");
				}
			}

		}


		/// <summary>
		/// Ensure that placement of pieces and pawns are correct. 
		/// </summary>
		[Fact]
		public void Placement() {
			ChessBoard b = CreateBoardFromMoves();
			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "white king position");
			b.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Queen, "black queen position");
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "white rook position");
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Bishop, "black bishop position");
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "white knight position");
			b.GetPieceAtPosition(Pos("b7")).PieceType.Should().Be(ChessPieceType.Pawn, "black pawn position");
		}

		[Fact]
		public void check_kingQueenPositions() {
			ChessBoard b = new ChessBoard();
			// the piece at this position should be a king
			b.GetPieceAtPosition(Pos(0, 4)).PieceType.Should().Be(ChessPieceType.King, "Black king should be at position (0,4)");
			// the piece at this position should be a queen
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Black Queen should be at position (0,3)");
			// the piece at this position should be a king
			b.GetPieceAtPosition(Pos(7, 4)).PieceType.Should().Be(ChessPieceType.King, "White king should be at position (7,4)");
			// the piece at this position should be a queen
			b.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.Queen, "White's Queen should be at position (7,3)");
		}


		/// <summary>
		/// Simple facts about "new" boards.
		/// </summary>
		[Fact]
		public void NewChessBoard3() {
			ChessBoard b = new ChessBoard();
			// Test position of the knights on both sides
			b.GetPieceAtPosition(Pos(7, 1)).PieceType.Should().Be(ChessPieceType.Knight, "White's knight at position (7,1)");
			b.GetPieceAtPosition(Pos(7, 1)).PieceType.Should().Be(ChessPieceType.Knight, "White's knight at position (7,6)");
			b.GetPieceAtPosition(Pos(0, 1)).PieceType.Should().Be(ChessPieceType.Knight, "Black's knight at position (0,1)");
			b.GetPieceAtPosition(Pos(0, 1)).PieceType.Should().Be(ChessPieceType.Knight, "Black's knight at position (0,6)");
		}

		/// <summary>
		/// Tests the initial board. Test both black and white.
		/// </summary>
		[Fact]
		public void InitialBoardStart() {
			ChessBoard b = new ChessBoard();

			var testCheck = b.IsCheck;
			testCheck.Should().BeFalse("The game has just STARTED, there should be NO Check!");

			var testCheckMate = b.IsCheckmate;
			testCheckMate.Should().BeFalse("The game has just STARTED, there should be NO Checkmate!");

			var testDraw = b.IsDraw;
			testDraw.Should().BeFalse("The game has just STARTED, there should be NO Draw!");

			var testGameFinish = b.IsFinished;
			testGameFinish.Should().BeFalse("The game has just STARTED, the game cannot be done already!");

			var testStalemate = b.IsStalemate;
			testStalemate.Should().BeFalse("The game has just STARTED, there should be NO Stalemate!");

			var testCurrentPlater = b.CurrentPlayer;
			testCurrentPlater.Should().Be(1, "White should be the starting player!");

			char[] files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

			foreach (var file in files) {
				var whitePawns = b.GetPieceAtPosition(Pos(file.ToString() + "2"));
				var blackPawns = b.GetPieceAtPosition(Pos(file.ToString() + "7"));

				whitePawns.PieceType.Should().Be(ChessPieceType.Pawn, "The pieces at rank 2 should be all White Pawns!");
				blackPawns.PieceType.Should().Be(ChessPieceType.Pawn, "The pieces at rank 7 should be all Black Pawns!");
			}
		}

	}


}
