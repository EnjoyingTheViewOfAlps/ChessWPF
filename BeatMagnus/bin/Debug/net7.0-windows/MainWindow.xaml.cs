using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessWpf
{
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush lightSquareBrush = new SolidColorBrush(Colors.Beige);
        private readonly SolidColorBrush darkSquareBrush = new SolidColorBrush(Colors.SaddleBrown);
        private const int boardSize = 8;
        private Button[,] squares = new Button[boardSize, boardSize];
        private ChessPiece[,] pieces = new ChessPiece[boardSize, boardSize];
        private ChessSquare selectedSquare = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    var square = new Button
                    {
                        Background = (row + col) % 2 == 0 ? lightSquareBrush : darkSquareBrush,
                        Tag = new ChessSquare(row, col)
                    };

                    square.Click += Square_Click;
                    Grid.SetRow(square, row);
                    Grid.SetColumn(square, col);
                    chessBoard.Children.Add(square);
                    squares[row, col] = square;
                    pieces[row, col] = null;
                }
            }

            InitializePieces();
        }

        private void InitializePieces()
        {
            // Initialize white pawns
            for (int col = 0; col < boardSize; col++)
            {
                pieces[1, col] = new Pawn(ChessColor.White);
                AddPieceToSquare(pieces[1, col], 1, col);
            }

            // Initialize black pawns
            for (int col = 0; col < boardSize; col++)
            {
                pieces[6, col] = new Pawn(ChessColor.Black);
                AddPieceToSquare(pieces[6, col], 6, col);
            }
        }

        private void AddPieceToSquare(ChessPiece piece, int row, int col)
        {
            pieces[row, col] = piece;
            Button square = squares[row, col];
            square.Content = piece.Symbol;
        }

        private void Square_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var square = (ChessSquare)button.Tag;

            if (selectedSquare != null && (selectedSquare.Row != square.Row || selectedSquare.Column != square.Column))
            {
                MovePiece(square);
                selectedSquare = null;
            }
            else if (pieces[square.Row, square.Column] != null)
            {
                selectedSquare = square;
            }
        }

        private void MovePiece(ChessSquare toSquare)
        {
            if (selectedSquare == null)
                return;

            int fromRow = selectedSquare.Row;
            int fromCol = selectedSquare.Column;
            int toRow = toSquare.Row;
            int toCol = toSquare.Column;

            ChessPiece selectedPiece = pieces[fromRow, fromCol];

            if (selectedPiece != null && selectedPiece.IsValidMove(selectedSquare, toSquare))
            {
                pieces[fromRow, fromCol] = null;
                pieces[toRow, toCol] = selectedPiece;

                squares[fromRow, fromCol].Content = null;
                squares[toRow, toCol].Content = selectedPiece.Symbol;
            }
        }
    }

    public enum ChessColor
    {
        White,
        Black
    }

    public class ChessSquare
    {
        public int Row { get; }
        public int Column { get; }

        public ChessSquare(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }

    public abstract class ChessPiece
    {
        public abstract string Symbol { get; }
        public ChessColor Color { get; }

        protected ChessPiece(ChessColor color)
        {
            Color = color;
        }

        public abstract bool IsValidMove(ChessSquare fromSquare, ChessSquare toSquare);
    }

    public class Pawn : ChessPiece
    {
        public override string Symbol => Color == ChessColor.White ? "♙" : "♟";

        public Pawn(ChessColor color) : base(color) { }

        public override bool IsValidMove(ChessSquare fromSquare, ChessSquare toSquare)
        {
            int rowDifference = Math.Abs(toSquare.Row - fromSquare.Row);
            int colDifference = Math.Abs(toSquare.Column - fromSquare.Column);

            if (fromSquare.Column == toSquare.Column)
            {
                if (Color == ChessColor.White)
                {
                    return (toSquare.Row == fromSquare.Row + 1 && rowDifference == 1);
                }
                else
                {
                    return (toSquare.Row == fromSquare.Row - 1 && rowDifference == 1);
                }
            }
            return false;
        }
    }
}
