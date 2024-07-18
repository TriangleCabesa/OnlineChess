using OnlineChess.Interfaces;

namespace OnlineChess.Implementations
{
    public class Bishop : IPiece
    {
        public bool IsWhite { get; private set; }
        public Guid Id { get; private set; }
        public Point Point { get; set; }

        public Bishop(Board board)
        {
            List<(Point point, bool isWhite)> spawnPoints =
            [
                (new Point(2, 0), true),
                (new Point(board.Spaces.GetLength(0) - 3, 0), true),
                (new Point(2, board.Spaces.GetLength(1) - 1), false),
                (new Point(board.Spaces.GetLength(0) - 3, board.Spaces.GetLength(1) - 1), false)
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

            throw new Exception("No room for new bishop");
        }

        public Bishop(Board board, Point point, bool isWhite)
        {
            Point = point;
            IsWhite = isWhite;
            board.Spaces[point.X, point.Y].SetPiece(this);
        }

        public List<(ISpace oldSpace, ISpace newSpace)> GetPossibleMoves(IBoard board, bool checkLegalMoves = true)
        {
            List<ISpace> possibleMoves = [];

            possibleMoves.AddRange(GetMovesInDirection(board, 1, 1));
            possibleMoves.AddRange(GetMovesInDirection(board, 1, -1));
            possibleMoves.AddRange(GetMovesInDirection(board, -1, 1));
            possibleMoves.AddRange(GetMovesInDirection(board, -1, -1));

            possibleMoves = checkLegalMoves ? possibleMoves.GetLegalMoves(board, GetKing(board), Point) : possibleMoves;

            List<(ISpace oldSpace, ISpace newSpace)> result = [];

            foreach (var move in possibleMoves)
            {
                result.Add((board.Spaces[Point.X, Point.Y], move));
            }

            return result;
        }

        private List<ISpace> GetMovesInDirection(IBoard board, int changeX, int changeY)
        {
            List<ISpace> possibleMoves = [];

            Point point = new(Point.X + changeX, Point.Y + changeY);

            while (IsPointInBoard(board, point))
            {
                ISpace space = board.Spaces[point.X, point.Y];

                if (space.IsOccupied && space.GetPiece()!.IsWhite == IsWhite)
                    break;

                possibleMoves.Add(space);

                if (space.IsOccupied && space.GetPiece()!.IsWhite != IsWhite)
                    break;

                point = new(point.X + changeX, point.Y + changeY);
            }

            return possibleMoves;
        }

        private bool IsPointInBoard(IBoard board, Point point) =>
            point.X >= 0 && point.Y >= 0 && point.X < board.Spaces.GetLength(0) && point.Y < board.Spaces.GetLength(1);
        

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
