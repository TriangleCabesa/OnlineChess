using OnlineChess.Interfaces;

namespace OnlineChess.Implementations
{
    public class Pawn : IPiece
    {
        public (ISpace? space, Pawn? pawn) EnPessantSpace { get; set; }
        public bool IsWhite { get; private set; }
        public Guid Id { get; private set; }
        public Point Point { get; set; }

        public List<(ISpace oldSpace, ISpace newSpace)> GetPossibleMoves(IBoard board, bool checkLegalMoves = true)
        {
            if (Point.Y == 0 || Point.Y == board.Spaces.GetLength(1) - 1)
                return [];

            int y = IsWhite ? Point.Y + 1 : Point.Y - 1;
            int x = Point.X;
            bool firstMove = IsWhite ? Point.Y == 1 : Point.Y == 6;

            List<ISpace> possibleMoves = [];

            if (!board.Spaces[x, y].IsOccupied)
            {
                possibleMoves.Add(board.Spaces[x, y]);
                int secondY = IsWhite ? Point.Y + 2 : Point.Y - 2;

                if (firstMove && !board.Spaces[x, secondY].IsOccupied)
                {
                    possibleMoves.Add(board.Spaces[x, secondY]);

                    if (x > 0 && board.Spaces[x - 1, secondY].GetPiece() is Pawn && board.Spaces[x - 1, secondY].GetPiece()!.IsWhite != IsWhite)
                        (board.Spaces[x - 1, secondY].GetPiece() as Pawn)!.EnPessantSpace = (board.Spaces[x, y], this);

                    if (x < board.Spaces.GetLength(0) - 1 && board.Spaces[x + 1, secondY].GetPiece() is Pawn && board.Spaces[x + 1, secondY].GetPiece()!.IsWhite != IsWhite)
                        (board.Spaces[x + 1, secondY].GetPiece() as Pawn)!.EnPessantSpace = (board.Spaces[x, y], this);
                }
            }

            if (x > 0 && board.Spaces[x - 1, y].IsOccupied && board.Spaces[x - 1, y].GetPiece()!.IsWhite != IsWhite)
                possibleMoves.Add(board.Spaces[x - 1, y]);

            if (x < 7 && board.Spaces[x + 1, y].IsOccupied && board.Spaces[x + 1, y].GetPiece()!.IsWhite != IsWhite)
                possibleMoves.Add(board.Spaces[x + 1, y]);

            if (EnPessantSpace.space is not null)
                possibleMoves.Add(EnPessantSpace.space);

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
            return new Bitmap("");
        }

        public Pawn(IBoard board)
        {
            Id = Guid.NewGuid();

            for (int i = 0; i < board.Spaces.GetLength(0); i++)
            {
                if (board.Spaces[i, 1].IsOccupied)
                    continue;

                IsWhite = true;
                Point = new Point(i, 1);
                board.Spaces[i, 1].SetPiece(this);

                return;
            }

            for (int i = 0; i < board.Spaces.GetLength(0); i++)
            {
                if (board.Spaces[i, 6].IsOccupied)
                    continue;

                IsWhite = false;
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
