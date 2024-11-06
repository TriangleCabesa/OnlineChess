namespace OnlineChess.Interfaces
{
    public interface IBoard
    {
        ISpace[,] Spaces { get; }

        void SimulatePieceMove(ISpace oldSpace, ISpace newSpace, bool resetSpecialMoves = true);

        void MovePiece(ISpace oldSpace, ISpace newSpace, bool resetSpecialMoves = true);

        (ISpace oldSpace, ISpace newSpace) GetLastMove();

        IEnumerable<ISpace> AsEnumerable();

        ISpace this[int i, int j] { get; }
    }
}
