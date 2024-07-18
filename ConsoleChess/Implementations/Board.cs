using OnlineChess.Interfaces;
using System.Linq;

namespace OnlineChess.Implementations
{
    public class Board : IBoard
    {
        public ISpace[,] Spaces { get; private set; }

        public Board()
        {
            Spaces = new ISpace[8, 8];

            SetUpPieces();
        }

        public void MovePiece(ISpace oldSpace, ISpace newSpace)
        {
            newSpace.SetPiece(oldSpace.GetPiece());
            oldSpace.SetPiece(null);

            for (int i = 0; i < Spaces.GetLength(0); i++)
            {
                for (int j = 0; j < Spaces.GetLength(1); j++)
                {
                    if (Spaces[i, j] == newSpace)
                    {
                        newSpace.GetPiece()!.Point = new Point(i, j);
                    }
                }
            }

            foreach (ISpace space in Spaces)
            {
                if (space.GetPiece() is Pawn pawn)
                    if (pawn.IsWhite == newSpace.GetPiece()!.IsWhite)
                        pawn.EnPessantSpace = null;
            }
        }

        private void SetUpPieces()
        {
            for (int i = 0; i < Spaces.GetLength(0); i++)
            {
                for (int j = 0; j < Spaces.GetLength(1); j++)
                {
                    Spaces[i, j] = new Space();
                }
            }

            for (int i = 0; i < 16; i++)
                _ = new Pawn(this); // The piece handles getting its own space, and its own memory.

            _ = new King(this);
            _ = new King(this);
        }
    }
}
