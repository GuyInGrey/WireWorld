using System;
using System.Drawing;

namespace WireWorld
{
    public class WireWorldMap
    {
        private WireWorldState[,] map;
        private WireWorldState[,] tempMap;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int BootingSquareSize { get; set; } = 15;
        public int SquareSize { get; set; } = 15;

        public SolidBrush HeadColor { get; set; } = new SolidBrush(Color.CornflowerBlue);
        public SolidBrush TailColor { get; set; } = new SolidBrush(Color.White);
        public SolidBrush WireColor { get; set; } = new SolidBrush(Color.LightGray);
        public SolidBrush BlankColor { get; set; } = new SolidBrush(Color.Black);

        Point[] surrounding;

        public WireWorldMap(int width, int height, WireWorldState defaultState)
        {
            surrounding = new Point[8]
            {
                new Point(-1, 0),
                new Point(1, 0),
                new Point(0, -1),
                new Point(0, 1),
                new Point(-1, -1),
                new Point(-1, 1),
                new Point(1, -1),
                new Point(1, 1),
            };

            map = new WireWorldState[width, height];
            Width = width;
            Height = height;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    this[x, y] = defaultState;
                }
            }
        }

        public WireWorldState this[int x, int y]
        {
            get => map[x,y];
            set => map[x, y] = value;
        }

        public void Cycle()
        {
            if (map != null)
            {
                tempMap = new WireWorldState[Width, Height];

                for (var y = 0; y < Height; y++)
                {
                    for (var x = 0; x < Width; x++)
                    {
                        var headCnt = 0;
                        for (var i = 0; i < 8; i++)
                        {
                            var _x = x + surrounding[i].X;
                            var _y = y + surrounding[i].Y;

                            if (_x >= 0 && _x < map.GetUpperBound(0) + 1)
                            {
                                if (_y >= 0 && _y < map.GetUpperBound(1) + 1)
                                {
                                    if (map[_x, _y] == WireWorldState.Head)
                                    {
                                        headCnt++;
                                    }
                                }
                            }
                        }

                        tempMap[x, y] = TileCycle(headCnt, map[x, y]);
                    }
                }

                map = tempMap;
            }
        }

        public WireWorldState TileCycle(int headCnt, WireWorldState currentState)
        {

            if (currentState == WireWorldState.Dead)
            {
                return WireWorldState.Dead;
            }

            if (currentState == WireWorldState.Head)
            {
                return WireWorldState.Tail;
            }

            if (currentState == WireWorldState.Tail)
            {
                return WireWorldState.Wire;
            }

            if (currentState == WireWorldState.Wire)
            {
                if (headCnt == 1 || headCnt == 2)
                {
                    return WireWorldState.Head;
                }
                else
                {
                    return WireWorldState.Wire;
                }
            }

            return currentState;
        }

        public void SetState(int x, int y, WireWorldState state)
        {
            try
            {
                map[x, y] = state;
            }
            catch
            {

            }
        }
        public WireWorldState GetState(Point point) => map[point.X, point.Y];

        public void DrawState(Graphics g, Point p, WireWorldState state, bool literal)
        {
            var toUse = new Rectangle(p.X, p.Y, SquareSize, SquareSize);
            if (!literal)
            {
                toUse = new Rectangle(p.X * SquareSize, p.Y * SquareSize, SquareSize, SquareSize);
            }

            switch (state)
            {
                case WireWorldState.Head:
                    g.FillRectangle(HeadColor, toUse);
                    break;
                case WireWorldState.Tail:
                    g.FillRectangle(TailColor, toUse);
                    break;
                case WireWorldState.Wire:
                    g.FillRectangle(WireColor, toUse);
                    break;
                case WireWorldState.Dead:
                    g.FillRectangle(BlankColor, toUse);
                    break;
            }
        }
    }
}