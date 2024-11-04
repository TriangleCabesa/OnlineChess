using OnlineChess.Interfaces;
using System.Collections.Generic;

namespace OnlineChess.Implementations
{
    public class King : IPiece
    {
        public bool IsWhite { get; private set; }
        public bool CanCastle { get; set; } = true;
        public List<(ISpace kingSpace, ISpace oldRookSpace, ISpace newRookSpace)> CastleSpaces { get; private set; } = [];

        public Point Point { get; set; }

        public Guid Id { get; private set; }

        public King(IBoard board)
        {
            Id = Guid.NewGuid();

            if (!board.Spaces[4, 0].IsOccupied)
            {
                IsWhite = false;
                Point = new(4, 0);
                board.Spaces[4, 0].SetPiece(this);

                return;
            }

            if (!board.Spaces[4, 7].IsOccupied)
            {
                IsWhite = true;
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

            possibleMoves.AddRange(GetCastleSpaces(board, checkLegalMoves));

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
            string color = IsWhite ? "White" : "Black";

            return new Bitmap(Directory.GetCurrentDirectory().Split("OnlineChess").First() + $@"OnlineChess\OnlineChess\Images\{color}King.png");
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

        private List<ISpace> GetCastleSpaces(IBoard board, bool checkLegalMoves)
        {
            CastleSpaces = [];
            List<Rook> rooks = GetRooks(board).Where(x => x.CanCastle).ToList();

            if (!(rooks.Count > 0 && CanCastle))
                return [];

            foreach (var rook in rooks)
            {
                int incrementer = rook.Point.X > Point.X ? 1 : -1;

                int x = Point.X + incrementer;
                int y = Point.Y;
                List<ISpace> spaces = [];

                while (rook.Point.X != x)
                {
                    if (board[x, y].GetPiece() is not null)
                        break;

                    spaces.Add(board[x, y]);
                    x += incrementer;
                }

                if (rook.Point.X != x)
                    continue;

                if (checkLegalMoves && board is Board b)
                {
                    bool isSpaceCovered = false;

                    foreach (var space in spaces)
                    {
                        if (b.GetAllLegalMoves(!IsWhite).Any(x => x.newSpace == space))
                            isSpaceCovered = true;
                    }

                    if (isSpaceCovered)
                        continue;
                }

                CastleSpaces.Add((spaces[1], board[rook.Point.X, rook.Point.Y], spaces[0]));
            }

            return CastleSpaces.Select(x => x.kingSpace).ToList();
        }

        private List<Rook> GetRooks(IBoard board)
        {
            List<Rook> rooks = [];

            foreach (ISpace space in board.Spaces)
            {
                IPiece? piece = space.GetPiece();

                if (piece is null)
                    continue;

                if (piece is Rook rook && rook.IsWhite == IsWhite)
                    rooks.Add(rook);
            }

            return rooks;
        }
    }
}
