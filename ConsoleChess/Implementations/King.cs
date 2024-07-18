using OnlineChess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<ISpace> GetMoveableSpaces(IBoard board, bool checkLegalMoves = true)
        {
            List<ISpace> result = [];

            for (int i = Point.X - 1; i <= Point.X + 1; i++)
            {
                if (i < 0 || i > board.Spaces.GetLength(0) - 1)
                    continue;

                for (int j = Point.Y - 1; j <= Point.Y + 1; j++)
                {
                    if (j < 0 || j > board.Spaces.GetLength(1) - 1)
                        continue;

                    result.Add(board.Spaces[i, j]);
                }
            }

            return checkLegalMoves ? result.GetLegalMoves(board, this, Point) : result;
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

                spaces.AddRange(piece.GetMoveableSpaces(board, false));
            }

            return spaces.Any(x => x.GetPiece() == this);
        }
    }
}
