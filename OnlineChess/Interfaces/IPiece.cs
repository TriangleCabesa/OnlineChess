namespace OnlineChess.Interfaces
{
    public interface IPiece
    {
        Point Point { get; set; }
        bool IsWhite { get; }
        Guid Id { get; }
        List<(ISpace oldSpace, ISpace newSpace)> GetPossibleMoves(IBoard board, bool checkLegalMoves = true);
        Bitmap GetSprite();
    }
}
