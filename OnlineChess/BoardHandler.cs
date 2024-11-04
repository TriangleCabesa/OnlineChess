using FunctionalCore.Types;
using OnlineChess.Implementations;
using OnlineChess.Interfaces;
using System.ComponentModel;
using static FunctionalCore.Extensions.Option;

namespace OnlineChess;

public class BoardHandler
{
    private static readonly int _windowYOffset = 45;
    private static readonly int _windowXOffset = 19;
    private Board _board;
    private Bitmap _bitmap;
    private Graphics _graphics;
    private Window _window;
    private int _boardSize;
    private Option<ISpace> _lastSelectedSpace;
    private bool _isLoadDrawn = false;
    private bool _isWhiteTurn = true;
    private bool _playerIsWhite = true;

    private static Image _boardImage = Image.FromFile(Directory.GetCurrentDirectory().Split("OnlineChess").First() + @"OnlineChess\OnlineChess\Images\ChessBoardImage.png");

    public BoardHandler(Window window)
    {
        _board = new();
        _window = window;
        _bitmap = new Bitmap(_window.Width, _window.Height);
        _graphics = Graphics.FromImage(_bitmap);
        _boardSize = Math.Min(_window.Width, _window.Height);
        _lastSelectedSpace = None<ISpace>();
    }

    public Bitmap GetBoardImage()
    {
        if (_isLoadDrawn && _window.ClickPoint.IsNone)
            return _bitmap;

        _isLoadDrawn = true;

        int windowWidth = _window.Width - _windowXOffset;
        int windowHeight = _window.Height - _windowYOffset;
        Option<ISpace> selectedSpace = None<ISpace>();

        _boardSize = Math.Min(windowWidth, windowHeight);
        int squareSize = _boardSize / 8;
        Bitmap background = new(_boardImage, new Size(_boardSize, _boardSize));
        _graphics = Graphics.FromImage(_bitmap);
        _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

        _graphics.FillRectangle(Brushes.White, 0, 0, windowWidth, windowHeight);
        _graphics.DrawImage(background, 0, 0);

        if (_window.ClickPoint.TryGetValue(out Point clickedPoint))
        {
            selectedSpace = Some(_board.Spaces[clickedPoint.X / squareSize, clickedPoint.Y / squareSize]);
        }

        if (selectedSpace.TryGetValue(out var clickedSpace))
        {
            if (clickedSpace.IsOccupied)
                HandleOccupiedSpace(clickedSpace, squareSize);

            if (_lastSelectedSpace.TryGetValue(out var lastSelectedSpace))
                HandlePieceMove(clickedSpace, lastSelectedSpace, squareSize);
        }

        if (IsCheckMate())
        {
            ColorKingRed(squareSize);
        }

        foreach (var space in _board.Spaces)
        {
            if (space.IsOccupied)
            {
                var piece = space.GetPiece();

                if (piece is not null)
                {
                    var pieceImage = piece.GetSprite();
                    pieceImage.MakeTransparent(Color.White);

                    _graphics.DrawImage(new Bitmap(pieceImage, squareSize, squareSize), new Point(piece.Point.X * squareSize, piece.Point.Y * squareSize));
                }
            }
        }

        _window.ClickPoint = None<Point>();
        _lastSelectedSpace = selectedSpace;

        return _bitmap;
    }

    private void ColorKingRed(int squareSize)
    {
        foreach (ISpace space in _board.Spaces)
        {
            IPiece? piece = space.GetPiece();

            if (piece is null)
                continue;

            if (piece is King king && king.IsWhite == _isWhiteTurn)
                _graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 0, 0)),
                new Rectangle(king.Point.X * squareSize + squareSize / 5,
                    king.Point.Y * squareSize + squareSize / 5,
                    squareSize - (squareSize / 5) * 2,
                    squareSize - (squareSize / 5) * 2));
        }
    }

    private bool IsCheckMate()
    {
        var moves = _board.GetAllLegalMoves(_isWhiteTurn);

        return moves.Count == 0;
    }

    private void HandlePieceMove(ISpace clickedSpace, ISpace lastSelectedSpace, int squareSize)
    {
        var piece = lastSelectedSpace.GetPiece();

        if (piece is not null)
        {
            if (piece.IsWhite != _isWhiteTurn)
                return;

            

            var pieceMoves = piece.GetPossibleMoves(_board, true);

            foreach ((var oldSpace, var newSpace) in pieceMoves)
            {
                if (newSpace == clickedSpace)
                {
                    _board.MovePiece(oldSpace, newSpace);
                    _isWhiteTurn = !_isWhiteTurn;
                    return;
                }
            }
        }
    }

    private void HandleOccupiedSpace(ISpace space, int squareSize)
    {
        var piece = space.GetPiece()!;

        if (piece.IsWhite != _isWhiteTurn)
            return;

        var legalMoves = piece.GetPossibleMoves(_board, true);

        foreach (var move in legalMoves)
        {
            var newSpace = move.newSpace;
            _graphics.FillEllipse(new SolidBrush(Color.FromArgb(50, 0, 0, 0)),
                new Rectangle(newSpace.Point.X * squareSize + squareSize / 5,
                    newSpace.Point.Y * squareSize + squareSize / 5,
                    squareSize - (squareSize / 5) * 2,
                    squareSize - (squareSize / 5) * 2));
        }
    }

    private bool IsKingInCheck()
    {
        foreach (ISpace space in _board.Spaces)
        {
            IPiece? piece = space.GetPiece();
            if (piece is null)
                continue;

            if (piece is King king && king.IsWhite == _isWhiteTurn)
                return king.IsInCheck(_board);
        }

        return false;
    }
}
