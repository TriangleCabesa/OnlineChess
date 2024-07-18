using OnlineChess.Interfaces;

namespace OnlineChess.Implementations
{
    public class King : IPiece
    {
        public bool IsWhite { get; private set; }
        public Point Point { get; set; }

        public Guid Id { get; private set; }

        public King(IBoard board)
        {
            Id = Guid.NewGuid();

            if (!board.Spaces[4, 0].IsOccupied)
            {
                IsWhite = true;
                Point = new(4, 0);
                board.Spaces[4, 0].SetPiece(this);

                return;
            }

            if (!board.Spaces[4, 7].IsOccupied)
            {
                IsWhite = false;
                Point = new(4, 7);
                board.Spaces[4, 7].SetPiece(this);

                return;
            }

            throw new Exception("No room for new king");
        }

        public List<(ISpace oldSpace, ISpace newSpace)> GetPossibleMoves(IBoard board, bool checkLegalMoves = true)
        {
            List<ISpace> possibleMoves = [];

            for (int i = Point.X - 1; i <= Point.X + 1; i++)
            {
                if (i < 0 || i > board.Spaces.GetLength(0) - 1)
                    continue;

                for (int j = Point.Y - 1; j <= Point.Y + 1; j++)
                {
                    if (j < 0 || j > board.Spaces.GetLength(1) - 1)
                        continue;

                    if (i == Point.X && j == Point.Y)
                        continue;

                    if (board.Spaces[i, j].IsOccupied && board.Spaces[i, j].GetPiece()!.IsWhite == IsWhite)
                        continue;

                    possibleMoves.Add(board.Spaces[i, j]);
                }
            }

            possibleMoves = checkLegalMoves ? possibleMoves.GetLegalMoves(board, this, Point) : possibleMoves;

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

        public bool IsInCheck(IBoard board)
        {
            List<ISpace> spaces = [];

            foreach (ISpace space in board.Spaces)
            {
                IPiece? piece = space.GetPiece();

                if (piece is null)
                    continue;

                if (piece.IsWhite == IsWhite)
                    continue;

                spaces.AddRange(piece.GetPossibleMoves(board, false).Select(x => x.newSpace));
            }

            return spaces.Any(x => x.GetPiece() == this);
        }
    }
}
