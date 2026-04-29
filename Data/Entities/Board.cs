namespace Data
{
    internal sealed class Board : IBoard
    {
        public double Width { get; }
        public double Height { get; }
        
        public Board(double width, double height)
        {
            Height = height;
            Width = width;
        }
    }
}

