namespace OnlineChess.Interfaces
{
    public interface ISpace
    {
        Point Point { get; }
        bool IsOccupied { get; }
        IPiece? GetPiece();
        void SetPiece(IPiece? piece);
    }
}
