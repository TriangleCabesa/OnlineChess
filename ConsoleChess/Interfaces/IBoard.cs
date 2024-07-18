namespace OnlineChess.Interfaces
{
    public interface IBoard
    {
        ISpace[,] Spaces { get; }

        void MovePiece(ISpace oldSpace, ISpace newSpace);
    }
}
