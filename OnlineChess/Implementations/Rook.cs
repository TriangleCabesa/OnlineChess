using OnlineChess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineChess.Implementations
{
    public class Rook : IPiece
    {
        public bool IsWhite { get; private set; }
        public Guid Id { get; private set; }
        public Point Point { get; set; }

        public Rook(Board board)
        {
            List<(Point point, bool isWhite)> spawnPoints =
            [
                (new Point(0, 0), true),
                (new Point(board.Spaces.GetLength(0) - 1, 0), true),
                (new Point(0, board.Spaces.GetLength(1) - 1), false),
                (new Point(board.Spaces.GetLength(0) - 1, board.Spaces.GetLength(1) - 1), false)
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

        public Rook(Board board, Point point, bool isWhite)
        {
            Point = point;
            IsWhite = isWhite;
            board.Spaces[point.X, point.Y].SetPiece(this);
        }

        public List<(ISpace oldSpace, ISpace newSpace)> GetPossibleMoves(IBoard board, bool checkLegalMoves = true)
        {
            List<ISpace> possibleMoves = [];

            possibleMoves.AddRange(GetMovesInDirection(startValue: Point.X - 1, condition: i => i >= 0, increment: -1, getSpace: i => board.Spaces[i, Point.Y]));
            possibleMoves.AddRange(GetMovesInDirection(startValue: Point.X + 1, condition: i => i < board.Spaces.GetLength(0), increment: 1, getSpace: i => board.Spaces[i, Point.Y]));
            possibleMoves.AddRange(GetMovesInDirection(startValue: Point.Y - 1, condition: i => i >= 0, increment: -1, getSpace: i => board.Spaces[Point.X, i]));
            possibleMoves.AddRange(GetMovesInDirection(startValue: Point.Y + 1, condition: i => i < board.Spaces.GetLength(1), increment: 1, getSpace: i => board.Spaces[Point.X, i]));

            possibleMoves = checkLegalMoves ? possibleMoves.GetLegalMoves(board, GetKing(board), Point) : possibleMoves;

            List<(ISpace oldSpace, ISpace newSpace)> result = [];

            foreach (var move in possibleMoves)
            {
                result.Add((board.Spaces[Point.X, Point.Y], move));
            }

            return result;
        }

        private List<ISpace> GetMovesInDirection(int startValue, Predicate<int> condition, int increment, Func<int, ISpace> getSpace)
        {
            List<ISpace> possibleMoves = [];

            for (int i = startValue; condition(i); i += increment)
            {
                ISpace space = getSpace(i);

                if (space.IsOccupied && space.GetPiece()!.IsWhite == IsWhite)
                    break;

                possibleMoves.Add(space);

                if (space.IsOccupied && space.GetPiece()!.IsWhite != IsWhite)
                    break;
            }

            return possibleMoves;
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
