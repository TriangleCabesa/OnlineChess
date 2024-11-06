using OnlineChess.Interfaces;
using System.Collections.Generic;

namespace OnlineChess.Implementations
{
    public class Pawn : IPiece
    {
        public (ISpace? space, Pawn? pawn) EnPessantSpace { get; set; }
        public bool IsWhite { get; private set; }
        public Guid Id { get; private set; }
        public Point Point { get; set; }

        private bool FirstMove { get => IsWhite ? Point.Y == 6 : Point.Y == 1; }
        private int YDirection { get => IsWhite ? -1 : 1; }

        public List<(ISpace oldSpace, ISpace newSpace)> GetPossibleMoves(IBoard board, bool checkLegalMoves = true)
        {
            if (Point.Y == 0 || Point.Y == board.Spaces.GetLength(1) - 1)
                return [];

            List<ISpace> possibleMoves = [..AddForwardMoves(board)];
            possibleMoves.AddRange(AddNormalCaptures(board));
            possibleMoves.AddRange(AddEnPassant(board));

            possibleMoves = checkLegalMoves ? possibleMoves.GetLegalMoves(board, GetKing(board), Point) : possibleMoves;

            List<(ISpace oldSpace, ISpace newSpace)> result = [];

            foreach (var move in possibleMoves)
            {
                result.Add((board.Spaces[Point.X, Point.Y], move));
            }

            return result;
        }

        private List<ISpace> AddForwardMoves(IBoard board)
        {
            List<ISpace> result = [];

            if (!board.Spaces[Point.X, Point.Y + YDirection].IsOccupied)
                result.Add(board.Spaces[Point.X, Point.Y + YDirection]);

            if (FirstMove && !board.Spaces[Point.X, Point.Y + YDirection * 2].IsOccupied)
                result.Add(board.Spaces[Point.X, Point.Y + YDirection * 2]);

            return result;
        }

        private List<ISpace> AddNormalCaptures(IBoard board)
        {
            List<ISpace> result = [];
            bool spaceContainsEnemyPiece = Point.X > 0
                && board.Spaces[Point.X - 1, Point.Y + YDirection].IsOccupied
                && board.Spaces[Point.X - 1, Point.Y + YDirection].GetPiece()?.IsWhite != IsWhite;

            if (spaceContainsEnemyPiece)
                result.Add(board.Spaces[Point.X - 1, Point.Y + YDirection]);

            spaceContainsEnemyPiece = Point.X < board.Spaces.GetLength(0) - 1
                && board.Spaces[Point.X + 1, Point.Y + YDirection].IsOccupied
                && board.Spaces[Point.X + 1, Point.Y + YDirection].GetPiece()?.IsWhite != IsWhite;

            if (spaceContainsEnemyPiece)
                result.Add(board.Spaces[Point.X + 1, Point.Y + YDirection]);

            return result;
        }

        private List<ISpace> AddEnPassant(IBoard board)
        {
            var (oldSpace, newSpace) = board.GetLastMove();

            bool isPawnOnEnPassantSquare = IsWhite ? Point.Y == 3 : Point.Y == 4;
            bool isMoveOnEnPassantSquare = newSpace?.Point.Y == Point.Y;

            if (!isPawnOnEnPassantSquare || !isMoveOnEnPassantSquare)
                return [];

            Pawn? pawn = newSpace?.GetPiece() as Pawn;
            bool isLastMoveAnEnemyPawn = pawn?.IsWhite != IsWhite;
            bool didPawnMoveTwoSpaces = Math.Abs(oldSpace?.Point.Y - newSpace?.Point.Y ?? 0) == 2;

            if (!isLastMoveAnEnemyPawn || !didPawnMoveTwoSpaces)
                return [];

            EnPessantSpace = (board.Spaces[pawn!.Point.X, Point.Y + YDirection], pawn);

            return [board.Spaces[pawn!.Point.X, Point.Y + YDirection]];
        }

        public Bitmap GetSprite()
        {
            string color = IsWhite ? "White" : "Black";

            return new Bitmap(Directory.GetCurrentDirectory().Split("OnlineChess").First() + $@"OnlineChess\OnlineChess\Images\{color}Pawn.png");
        }

        public Pawn(IBoard board)
        {
            Id = Guid.NewGuid();

            for (int i = 0; i < board.Spaces.GetLength(0); i++)
            {
                if (board.Spaces[i, 1].IsOccupied)
                    continue;

                IsWhite = false;
                Point = new Point(i, 1);
                board.Spaces[i, 1].SetPiece(this);

                return;
            }

            for (int i = 0; i < board.Spaces.GetLength(0); i++)
            {
                if (board.Spaces[i, 6].IsOccupied)
                    continue;

                IsWhite = true;
                Point = new Point(i, 6);
                board.Spaces[i, 6].SetPiece(this);

                return;
            }

            throw new Exception("No room for new pawn");
        }

        private King GetKing(IBoard board)
        {
            foreach (ISpace space in board.Spaces)
            {
                IPiece? piece = space.GetPiece();

                if (piece is null)
                    continue;

                if (piece is King king && king.IsWhite == IsWhite)
                    return king;
            }

            throw new Exception("King not found");
        }
    }
}
