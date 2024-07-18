using OnlineChess.Interfaces;

namespace OnlineChess.Implementations
{
    public class Space(Point point) : ISpace
    {
        private IPiece? Piece;
        public Point Point { get; private set; } = point;
        public bool IsOccupied { get => Piece is not null; }

        public IPiece? GetPiece()
        {
            return Piece;
        }

        public void SetPiece(IPiece? piece)
        {
            Piece = piece;

            if (piece is not null)
                piece.Point = Point;
        }
    }
}
