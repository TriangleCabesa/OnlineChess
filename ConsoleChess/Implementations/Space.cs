using OnlineChess.Interfaces;

namespace OnlineChess.Implementations
{
    public class Space : ISpace
    {
        private IPiece? Piece;

        public bool IsOccupied { get => GetPiece() is not null; }

        public IPiece? GetPiece()
        {
            return Piece;
        }

        public void SetPiece(IPiece? piece)
        {
            Piece = piece;
        }
    }
}
