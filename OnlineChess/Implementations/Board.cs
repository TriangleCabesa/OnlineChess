using OnlineChess.Interfaces;
using System.ComponentModel;
using System.Linq;

namespace OnlineChess.Implementations
{
    public class Board : IBoard
    {
        public ISpace[,] Spaces { get; private set; }
        public List<IPiece> WhitePieces { get; set; }
        public List<IPiece> BlackPieces { get; set; }

        public ISpace this[int i, int j]
        {
            get
            {
                return Spaces[i, j];
            }
            set
            {
                Spaces[i, j] = value;
            }
        }

        public Board()
        {
            Spaces = new ISpace[8, 8];
            WhitePieces = [];
            BlackPieces = [];

            SetUpPieces();
        }

        public void MovePiece(ISpace oldSpace, ISpace newSpace, bool runSpecialMoves = true)
        {
            IPiece? piece = newSpace.GetPiece();
            IPiece? currentPiece = oldSpace.GetPiece();

            newSpace.SetPiece(oldSpace.GetPiece());
            oldSpace.SetPiece(null);

            if (!runSpecialMoves)
                return;

            if (newSpace.GetPiece() is Rook rook)
                rook.CanCastle = false;

            if (newSpace.GetPiece() is King king && king.CastleSpaces.Any(x => x.kingSpace == newSpace))
            {
                king.CanCastle = false;
                (ISpace oldSpace, ISpace newSpace) move = king.CastleSpaces.Where(x => x.kingSpace == newSpace).Select(x => (x.oldRookSpace, x.newRookSpace)).First();
                MovePiece(move.oldSpace, move.newSpace);
            }

            if (piece is not null)
            {
                if (piece.IsWhite)
                    WhitePieces.Remove(piece);
                else
                    BlackPieces.Remove(piece);
            }

            HandlePawnPromotions(newSpace);

            if (newSpace.GetPiece() is Pawn)
                foreach (var x in AsEnumerable())
                    if (x.GetPiece() is Pawn y && y.EnPessantSpace is not (null, null) && y.EnPessantSpace.space == newSpace)
                        Spaces[y.EnPessantSpace.pawn!.Point.X, y.EnPessantSpace.pawn!.Point.Y].SetPiece(null);

            if (newSpace.GetPiece() is null)
                newSpace.SetPiece(currentPiece);

            foreach (ISpace space in Spaces)
            {
                if (space.GetPiece() is Pawn pawn)
                    if (pawn.IsWhite == newSpace.GetPiece()!.IsWhite)
                        pawn.EnPessantSpace = (null, null);
            }
        }

        public List<(ISpace oldSpace, ISpace newSpace)> GetAllLegalMoves(bool isWhite)
        {
            List<(ISpace oldSpace, ISpace newSpace)> result = [];
            King? king = null;

            foreach (ISpace space in Spaces)
            {
                if (!space.IsOccupied)
                    continue;

                if (space.GetPiece()!.IsWhite != isWhite)
                    continue;

                if (space.GetPiece() is King k)
                    king = k;

                result.AddRange(space.GetPiece()!.GetPossibleMoves(this, false));
            }

            return result.GetLegalMoves(this, king!).Distinct().ToList();
        }

        public List<(ISpace oldSpace, ISpace newSpace)> GetAllLegalMovesFromSpace(bool isWhite, ISpace space)
        {
            List<(ISpace oldSpace, ISpace newSpace)> result = [];

            if (!space.IsOccupied)
                return [];

            if (space.GetPiece()!.IsWhite != isWhite)
                return [];

            result.AddRange(space.GetPiece()!.GetPossibleMoves(this, false));
            
            return result.GetLegalMoves(this, GetKing(isWhite)).Distinct().ToList();
        }

        private void SetUpPieces()
        {
            for (int i = 0; i < Spaces.GetLength(0); i++)
            {
                for (int j = 0; j < Spaces.GetLength(1); j++)
                {
                    Spaces[i, j] = new Space(new Point(i, j));
                }
            }

            for (int i = 0; i < 16; i++)
            {
                var x = new Pawn(this); // The piece handles getting its own space, and its own memory.

                if (x.IsWhite)
                    WhitePieces.Add(x);
                else
                    BlackPieces.Add(x);
            }

            for (int i = 0; i < 2; i++)
            {
                var a = new King(this);
                var b = new Queen(this);

                if (a.IsWhite)
                    WhitePieces.Add(a);
                else
                    BlackPieces.Add(a);

                if (b.IsWhite)
                    WhitePieces.Add(b);
                else
                    BlackPieces.Add(b);
            }

            for (int i = 0; i < 4; i++)
            {
                var a = new Knight(this);
                var b = new Rook(this);
                var c = new Bishop(this);

                if (a.IsWhite)
                    WhitePieces.Add(a);
                else
                    BlackPieces.Add(a);

                if (b.IsWhite)
                    WhitePieces.Add(b);
                else
                    BlackPieces.Add(b);

                if (c.IsWhite)
                    WhitePieces.Add(c);
                else
                    BlackPieces.Add(c);
            }
        }

        private void HandlePawnPromotions(ISpace space)
        {
            if (space.GetPiece() is not Pawn pawn)
                return;

            if ((pawn.IsWhite && pawn.Point.Y == Spaces.GetLength(1) - 1)
             || (!pawn.IsWhite && pawn.Point.Y == 0))
            {
                IPiece piece = new Queen(this, pawn.Point, pawn.IsWhite);
                space.SetPiece(piece);

                if (pawn.IsWhite)
                {
                    WhitePieces.Add(piece);
                    WhitePieces.Remove(pawn);
                }
                else
                {
                    BlackPieces.Add(piece);
                    BlackPieces.Remove(pawn);
                }
            }
        }

        public IEnumerable<ISpace> AsEnumerable()
        {
            for (int row = 0; row < Spaces.GetLength(0); row++)
            {
                for (int col = 0; col < Spaces.GetLength(1); col++)
                {
                    yield return Spaces[row, col];
                }
            }
        }

        private King GetKing(bool isWhite)
        {
            foreach (ISpace space in Spaces)
            {
                IPiece? piece = space.GetPiece();

                if (piece is null)
                    continue;

                if (piece is King king && king.IsWhite == isWhite)
                    return king;
            }

            throw new Exception("King not found");
        }
    }
}
