namespace OnlineChess.Interfaces
{
    public interface IBoard
    {
        ISpace[,] Spaces { get; }

        void MovePiece(ISpace oldSpace, ISpace newSpace, bool resetSpecialMoves = true);

        IEnumerable<ISpace> AsEnumerable();

        ISpace this[int i, int j] { get; }
    }
}
