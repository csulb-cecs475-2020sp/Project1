using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;


namespace Cecs475.BoardGames.Chess.Test {
	public class NewBoardMoveTests : ChessTest {
		/// <summary>
		/// Initial rook states should have no pieces moving and it should be Player One's turn
		/// </summary>
		[Fact]
		public void InitialRooksState() {
			ChessBoard b = new ChessBoard();
			//Pieces on the corners need to be Rooks
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "White's rook at position a1");
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Rook, "Black's rook at position a8");
			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Rook, "White's rook at position h1");
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Rook, "Black's rook at position h8");

			//get moves of the Rook
			var blockedRook = b.GetPossibleMoves();

			//Rooks cannot move because pawns and bishops are blocking
			GetMovesAtPosition(blockedRook, Pos("a1")).Should().HaveCount(0, "White's Rook('a1') cannot move in first turn because rooks and bishops are blocking");
			GetMovesAtPosition(blockedRook, Pos("a8")).Should().HaveCount(0, "Black's Rook('a8') cannot move in first turn because rooks and bishops are blocking");
			GetMovesAtPosition(blockedRook, Pos("h8")).Should().HaveCount(0, "White's Rook('h8') cannot move in first turn because rooks and bishops are blocking");
			GetMovesAtPosition(blockedRook, Pos("h1")).Should().HaveCount(0, "Black's Rook('h1') cannot move in first turn because rooks and bishops are blocking");
			//Game just started, should be Player one's turn
			b.CurrentPlayer.Should().Be(1, "It should be Player One's turn now");

		}

		/// <summary>
		/// Check a knights starting moves
		/// </summary>
		[Fact]
		public void KnightStart() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves();

			var movesAtPos = GetMovesAtPosition(possMoves, Pos("b1"));

			movesAtPos.Should().HaveCount(2, "A knight can move to two spaces at the start").And.Contain(Move("b1, c3")).And.Contain(Move("b1, a3"));

		}

		[Fact]
		public void InitialState() {
			ChessBoard b = new ChessBoard();
			var possibleMoves = b.GetPossibleMoves();
			foreach (var pos in GetPositionsInRank(2)) {
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Pawn, "Chess piece should be a pawn");
			}
			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Rook, "Chess piece should be a rook");
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "Chess piece should be a knight");
			b.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.Bishop, "Chess piece should be a bishop");
			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "Chess piece should be a king");
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Queen, "Chess piece should be a queen");
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Bishop, "Chess piece should be a bishop");
			b.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Knight, "Chess piece should be a knight");
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "Chess piece should be a rook");

			//Do other side of board
			foreach (var pos in GetPositionsInRank(7)) {
				b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Pawn, "Chess piece should be a pawn");
			}
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Rook, "Chess piece should be a rook");
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Knight, "Chess piece should be a knight");
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Bishop, "Chess piece should be a bishop");
			b.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "Chess piece should be a king");
			b.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Queen, "Chess piece should be a queen");
			b.GetPieceAtPosition(Pos("c8")).PieceType.Should().Be(ChessPieceType.Bishop, "Chess piece should be a bishop");
			b.GetPieceAtPosition(Pos("b8")).PieceType.Should().Be(ChessPieceType.Knight, "Chess piece should be a knight");
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Rook, "Chess piece should be a rook");

			for (int i = 3; i < 7; i++) {
				foreach (var pos in GetPositionsInRank(i)) {
					b.GetPieceAtPosition(pos).PieceType.Should().Be(ChessPieceType.Empty, "Square should be empty");
				}
			}
		}

		[Fact]//Initial State
		public void StartingNonPawnMoves() {
			ChessBoard b = new ChessBoard();

			Apply(b, "a2, a3");//Make it black pieces turn

			//Check all Non Pawn pieces number of moves at starting point
			var possMoves = b.GetPossibleMoves();
			var oneMove = GetMovesAtPosition(possMoves, Pos("h8"));
			oneMove.Should().HaveCount(0, "Rook has no moves");

			oneMove = GetMovesAtPosition(possMoves, Pos("g8"));
			oneMove.Should().HaveCount(2, "Knight has two moves");

			oneMove = GetMovesAtPosition(possMoves, Pos("f8"));
			oneMove.Should().HaveCount(0, "Bishop has no moves");

			oneMove = GetMovesAtPosition(possMoves, Pos("e8"));
			oneMove.Should().HaveCount(0, "King has no moves");

			oneMove = GetMovesAtPosition(possMoves, Pos("d8"));
			oneMove.Should().HaveCount(0, "Queen has no moves");

			oneMove = GetMovesAtPosition(possMoves, Pos("c8"));
			oneMove.Should().HaveCount(0, "Bishop has no moves");

			oneMove = GetMovesAtPosition(possMoves, Pos("b8"));
			oneMove.Should().HaveCount(2, "Knight has two moves");

			oneMove = GetMovesAtPosition(possMoves, Pos("a8"));
			oneMove.Should().HaveCount(0, "Rook has no moves");
		}

		/// <summary>
		/// Simple fact about the state of the board for a new chess game.
		/// </summary>
		[Fact]
		public void NewChessBoard() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves();
			var knightJumps = GetMovesAtPosition(possMoves, Pos("b1"));
			var bishopBlocked = GetMovesAtPosition(possMoves, Pos("f1"));

			// Checks for white queen at bottom of board
			b.GetPieceAtPosition(Pos(7, 3)).Player.Should().Be(1, "Player 1 (white) should be at the bottom of the board");
			b.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.Queen, "White queen at position (7, 3)");
			// Checks for black queen at top of board
			b.GetPieceAtPosition(Pos(0, 3)).Player.Should().Be(2, "Player 2 (black) should be at the bottom of the board");
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Black queen at position (0, 3)");
			// Checks possible movements of select pieces
			knightJumps.Should().HaveCount(2, "White knight should jump over pawn moving foward")
				.And.Contain(Move("b1, a3"))
				.And.Contain(Move("b1, c3"));
			bishopBlocked.Should().HaveCount(0, "White bishop should have no moves with pawns blocking forward movement");
		}

		[Fact]
		public void KnightsAtInitialBoard() {
			ChessBoard cBoard = new ChessBoard();

			var possMoves = cBoard.GetPossibleMoves();

			var initialWhiteKnightLocations = new List<BoardPosition> {
				Pos("b1"), Pos("g1") };

			var initialBlackKnightLocations = new List<BoardPosition> {
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
