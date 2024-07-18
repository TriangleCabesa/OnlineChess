using OnlineChess.Implementations;
using OnlineChess.Interfaces;

namespace OnlineChess
{
    public static class LegalMoveChecker
    {
        public static List<ISpace> GetLegalMoves(this List<ISpace> possibleMoves, IBoard board, King king, Point point)
        {
            List<ISpace> result = [];
            ISpace oldSpace = board.Spaces[point.X, point.Y];

            foreach (ISpace newSpace in possibleMoves)
            {
                IPiece? piece = newSpace.GetPiece();

                board.MovePiece(oldSpace, newSpace);

                if (!king.IsInCheck(board))
                    result.Add(newSpace);

                board.MovePiece(newSpace, oldSpace);

                if (piece is not null)
                    newSpace.SetPiece(piece);
            }

            return result;
        }
    }
}
