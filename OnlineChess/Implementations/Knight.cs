using OnlineChess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineChess.Implementations
{
    public class Knight : IPiece
    {
        public bool IsWhite { get; private set; }
        public Guid Id { get; private set; }
        public Point Point { get; set; }

        public Knight(Board board)
        {
            List<(Point point, bool isWhite)> spawnPoints =
            [
                (new Point(1, 0), true),
                (new Point(board.Spaces.GetLength(0) - 2, 0), true),
                (new Point(1, board.Spaces.GetLength(1) - 1), false),
                (new Point(board.Spaces.GetLength(0) - 2, board.Spaces.GetLength(1) - 1), false)
            ];

            foreach ((Point point, bool isWhite) in spawnPoints)
            {
                if (!board.Spaces[point.X, point.Y].IsOccupied)
                {
                    IsWhite = isWhite;
                    Point = point;
                    board.Spaces[point.X, point.Y].SetPiece(this);

                    return;
                }
            }

            throw new Exception("No room for new knight");
        }

        public Knight(Board board, Point point, bool isWhite)
        {
            Point = point;
            IsWhite = isWhite;
            board.Spaces[point.X, point.Y].SetPiece(this);
        }

        public List<(ISpace oldSpace, ISpace newSpace)> GetPossibleMoves(IBoard board, bool checkLegalMoves = true)
        {
            List<Point> movePoints =
            [
                new Point(Point.X + 2, Point.Y + 1),
                new Point(Point.X + 2, Point.Y - 1),
                new Point(Point.X - 2, Point.Y + 1),
                new Point(Point.X - 2, Point.Y - 1),
                new Point(Point.X + 1, Point.Y + 2),
                new Point(Point.X - 1, Point.Y + 2),
                new Point(Point.X + 1, Point.Y - 2),
                new Point(Point.X - 1, Point.Y - 2),
            ];

            List<ISpace> possibleMoves = [];

            foreach (var movePoint in movePoints)
            {
                if (!board.AsEnumerable().Any(x => x.Point == movePoint))
                    continue;

                ISpace? space = board.Spaces[movePoint.X, movePoint.Y];

                if (space.IsOccupied && space.GetPiece()!.IsWhite == IsWhite)
                    continue;

                possibleMoves.Add(space);
            }

            possibleMoves = checkLegalMoves ? possibleMoves.GetLegalMoves(board, GetKing(board), Point) : possibleMoves;

            List<(ISpace oldSpace, ISpace newSpace)> result = [];

            foreach (var move in possibleMoves)
            {
                result.Add((board.Spaces[Point.X, Point.Y], move));
            }

            return result;
        }

        public Bitmap GetSprite()
        {
            throw new NotImplementedException();
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
