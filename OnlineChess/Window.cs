using FunctionalCore.Types;
using OnlineChess.Implementations;
using OnlineChess.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static FunctionalCore.Extensions.Option;

namespace OnlineChess
{
    public partial class Window : Form
    {
        public Board Board;
        public BoardHandler BoardHandler;

        public int FrameRate = 120;
        public bool CanPaint = true;

        public Option<Point> ClickPoint;

        public Window()
        {
            InitializeComponent();
            Board = new();
            BoardHandler = new(this);
            DoubleBuffered = true;
            ClickPoint = None<Point>();

            Task.Run(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                while (true)
                {
                    stopwatch.Restart();

                    while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(1000 / FrameRate))
                        ;

                    CanPaint = true;
                }
            });
        }

        private void Window_Load(object sender, EventArgs e)
        {
            return;

            bool isWhite = true;
            OutputBoard();
            bool checkNextRound = false;
            List<(Point oldSpace, Point newSpace)> playedMoves = [];

            while (true)
            {
                var moves = Board.GetAllLegalMoves(isWhite);

                if (moves.Count == 0)
                    break;

                int index;

                if (isWhite)
                    index = GetIndex(moves);
                else
                    index = new Random().Next(moves.Count);

                Board.MovePiece(moves[index].oldSpace, moves[index].newSpace);
                playedMoves.Add((moves[index].oldSpace.Point, moves[index].newSpace.Point));

                if (IsKingInCheck(isWhite))
                    Console.WriteLine("Error");

                OutputBoard();

                if (checkNextRound)
                {
                    checkNextRound = false;
                }

                isWhite = !isWhite;

                if (IsKingInCheck(isWhite))
                {
                    Console.WriteLine("Check");
                    checkNextRound = true;
                }

                int whiteValue = 0;
                for (int i = 0; i < Board.WhitePieces.Count; i++)
                {
                    if (Board.WhitePieces[i] is Pawn)
                        whiteValue = 4;

                    if (Board.WhitePieces[i] is Knight || Board.WhitePieces[i] is Bishop)
                        whiteValue += 3;

                    if (Board.WhitePieces[i] is Rook)
                        whiteValue += 5;

                    if (Board.WhitePieces[i] is Queen)
                        whiteValue += 9;
                }

                int blackValue = 0;
                for (int i = 0; i < Board.BlackPieces.Count; i++)
                {
                    if (Board.BlackPieces[i] is Pawn)
                        blackValue = 4;

                    if (Board.BlackPieces[i] is Knight || Board.BlackPieces[i] is Bishop)
                        blackValue += 3;

                    if (Board.BlackPieces[i] is Rook)
                        blackValue += 5;

                    if (Board.BlackPieces[i] is Queen)
                        blackValue += 9;
                }

                if (whiteValue < 4 && blackValue < 4)
                    break;
            }

            if (IsKingInCheck(isWhite))
                Console.WriteLine("Checkmate");
            else
                Console.WriteLine("Stalemate");
        }

        private bool IsKingInCheck(bool IsWhite)
        {
            foreach (ISpace space in Board.Spaces)
            {
                IPiece? piece = space.GetPiece();
                if (piece is null)
                    continue;

                if (piece is King king && king.IsWhite == IsWhite)
                    return king.IsInCheck(Board);
            }

            return false;
        }

        private void OutputBoard()
        {
            Console.Clear();

            for (int i = Board.Spaces.GetLength(0) - 1; i >= 0; i--)
            {
                if (i % 2 == 0)
                    Console.WriteLine("░░░░░█████░░░░░█████░░░░░█████░░░░░█████");
                else
                    Console.WriteLine("█████░░░░░█████░░░░░█████░░░░░█████░░░░░");

                for (int j = 0; j < Board.Spaces.GetLength(1); j++)
                {
                    string output = (i + j) % 2 == 0 ? "░" : "█";

                    Console.Write(output + output);

                    if (Board.Spaces[j, i].GetPiece() is Pawn)
                        output = "P";
                    else if (Board.Spaces[j, i].GetPiece() is King)
                        output = "K";
                    else if (Board.Spaces[j, i].GetPiece() is Knight)
                        output = "N";
                    else if (Board.Spaces[j, i].GetPiece() is Rook)
                        output = "R";
                    else if (Board.Spaces[j, i].GetPiece() is Bishop)
                        output = "B";
                    else if (Board.Spaces[j, i].GetPiece() is Queen)
                        output = "Q";

                    Console.ForegroundColor = ConsoleColor.White;

                    if (!Board.Spaces[j, i].GetPiece()?.IsWhite ?? false)
                        Console.ForegroundColor = ConsoleColor.Cyan;

                    Console.Write(output);
                    Console.ForegroundColor = ConsoleColor.White;

                    output = (i + j) % 2 == 0 ? "░" : "█";
                    Console.Write(output + output);
                }

                Console.WriteLine(i + 1);

                if (i % 2 == 0)
                    Console.WriteLine("░░░░░█████░░░░░█████░░░░░█████░░░░░█████");
                else
                    Console.WriteLine("█████░░░░░█████░░░░░█████░░░░░█████░░░░░");
            }

            Console.WriteLine("  A    B    C    D    E    F    G    H ");
        }

        private static int GetIndex(List<(ISpace oldSpace, ISpace newSpace)> moves)
        {
            List<(ISpace oldSpace, ISpace newSpace)> moveCount = [];
            string move;

            while (moveCount.Count == 0)
            {
                move = Console.ReadLine()!;
                moveCount = moves.Where(x => (x.newSpace.Point.X + 1 == Convert.ToInt32(move[0]) - 48 || x.newSpace.Point.X + 1 == Convert.ToInt32(move[0]) - 96) && x.newSpace.Point.Y + 1 == Convert.ToInt32(move[1]) - 48).ToList();

                if (moveCount.Count == 0 && (move == "0-0" || move.Equals("O-O", StringComparison.CurrentCultureIgnoreCase)))
                {
                    foreach (var x in moves)
                    {
                        if (x.oldSpace.GetPiece() is King king)
                        {
                            if (king.CastleSpaces.Any(space => space.kingSpace == x.newSpace))
                                moveCount.Add(x);
                        }
                    }
                }
            }

            if (moveCount.Count > 1)
            {
                Console.WriteLine("Multiple pieces can move there. Which piece would you like to move?");

                for (int i = 0; i < moveCount.Count; i++)
                {
                    string pieceName = "";

                    IPiece piece = moveCount[i].oldSpace.GetPiece()!;

                    if (piece is Pawn) pieceName = "P";
                    if (piece is Knight) pieceName = "N";
                    if (piece is Bishop) pieceName = "B";
                    if (piece is Rook) pieceName = "R";
                    if (piece is Queen) pieceName = "Q";
                    if (piece is King) pieceName = "K";

                    Console.WriteLine($"{i}: {pieceName}{(char)(moveCount[i].oldSpace.Point.X + 97)}{moveCount[i].oldSpace.Point.Y + 1}");
                }

                move = Console.ReadLine()!;

                return moves.IndexOf(moveCount[Convert.ToInt32(move[0] - 48)]);
            }

            return moves.IndexOf(moveCount[0]);
        }

        private void PaintWindow(object sender, PaintEventArgs e)
        {
            var image = BoardHandler.GetBoardImage();
            e.Graphics.DrawImage(image, 0, 0, image.Width, image.Height);
        }

        private void RefreshWindow()
        {
            if (!CanPaint)
                return;

            Refresh();
            CanPaint = false;
        }

        private void Window_ResizeBegin(object sender, EventArgs e)
        {
            RefreshWindow();
        }

        private void Window_ResizeEnd(object sender, EventArgs e)
        {
            RefreshWindow();
        }

        private void Window_SizeChanged(object sender, EventArgs e)
        {
            RefreshWindow();
        }

        private void Window_MouseClick(object sender, MouseEventArgs e)
        {
            ClickPoint = Some(new Point(e.X, e.Y));
            RefreshWindow();
        }

        private void Window_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                RefreshWindow();
                DoEvents();
            }
        }
        private void DoEvents()
        {
            while (!CanPaint)
            {
                Application.DoEvents();
            }
        }

    }
}
