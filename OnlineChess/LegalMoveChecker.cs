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

                board.SimulatePieceMove(oldSpace, newSpace, false);

                if (!king.IsInCheck(board))
                    result.Add(newSpace);

                board.SimulatePieceMove(newSpace, oldSpace, false);

                if (piece is not null)
                    newSpace.SetPiece(piece);
            }

            return result;
        }

        public static List<(ISpace oldSpace, ISpace newSpace)> GetLegalMoves(this List<(ISpace oldSpace, ISpace newSpace)> possibleMoves, IBoard board, King king)
        {
            List<(ISpace oldSpace, ISpace newSpace)> result = [];

            foreach ((ISpace oldSpace, ISpace newSpace) in possibleMoves)
            {
                IPiece? piece = newSpace.GetPiece();

                board.SimulatePieceMove(oldSpace, newSpace, false);

                if (!king.IsInCheck(board))
                    result.Add((oldSpace, newSpace));

                board.SimulatePieceMove(newSpace, oldSpace, false);
                newSpace.SetPiece(piece);
            }

            return result;
        }
    }
}
