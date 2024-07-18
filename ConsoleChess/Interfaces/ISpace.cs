namespace OnlineChess.Interfaces
{
    public interface ISpace
    {
        bool IsOccupied { get; }
        IPiece? GetPiece();
        void SetPiece(IPiece? piece);
    }
}
