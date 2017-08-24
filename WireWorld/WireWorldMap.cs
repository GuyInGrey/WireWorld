using System;
using System.Drawing;

namespace WireWorld
{
    public class WireWorldMap
    {
        //Base Map
        private WireWorldState[,] map;
        private WireWorldState[,] tempMap;
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Square Size
        public int BootingSquareSize { get; set; } = 15;
        public int SquareSize { get; set; } = 15;

        //Colors
        public SolidBrush HeadColor { get; set; } = new SolidBrush(Color.CornflowerBlue);
        public SolidBrush TailColor { get; set; } = new SolidBrush(Color.Red);
        public SolidBrush WireColor { get; set; } = new SolidBrush(Color.Yellow);
        public SolidBrush BlankColor { get; set; } = new SolidBrush(Color.Black);
        public Pen GridColor { get; set; } = new Pen(Color.Gray);

        //Auto Cycler
        public bool AutoCycling { get; set; } = false;
        public float AutoCyclesPerSecond { get; set; } = 3;
        private int MilliSinceLastAuto { get; set; } = 0;

        //Surrounding Tiles
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
                        switch (this[x,y])
                        {
                            case WireWorldState.Dead:
                                continue;
                            case WireWorldState.Head:
                                tempMap[x, y] = WireWorldState.Tail;
                                continue;
                            case WireWorldState.Tail:
                                tempMap[x, y] = WireWorldState.Wire;
                                continue;
                            case WireWorldState.Wire:
                                var headCnt = 0;
                                for (var i = 0; i < 8; i++)
                                {
                                    var _x = x + surrounding[i].X;
                                    var _y = y + surrounding[i].Y;

                                    if (_x >= 0 && _x < map.GetUpperBound(0) + 1)
                                    {
                                        if (_y >= 0 && _y < map.GetUpperBound(1) + 1)
                                        {
                                            if (this[_x, _y] == WireWorldState.Head)
                                            {
                                                headCnt++;
                                            }
                                        }
                                    }
                                }

                                if (headCnt == 1 || headCnt == 2)
                                {
                                    tempMap[x, y] = WireWorldState.Head;
                                }
                                else
                                {
                                    tempMap[x, y] = WireWorldState.Wire;
                                }
                                continue;
                        }
                    }
                }

                map = tempMap;
            }
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

        public void DrawState(Graphics g, Point location, WireWorldState state, StateDrawMode drawMode)
        {
            var toUse = new Rectangle();

            if (drawMode == StateDrawMode.Literal)
            {
                toUse = new Rectangle(location.X, location.Y, SquareSize, SquareSize);
            }
            else if (drawMode == StateDrawMode.Tile)
            {
                toUse = new Rectangle(location.X * SquareSize, location.Y * SquareSize, SquareSize, SquareSize);
            }
            else if (drawMode == StateDrawMode.LiteralMatched)
            {
                toUse = new Rectangle((location.X/SquareSize) * SquareSize, (location.Y / SquareSize) * SquareSize, SquareSize, SquareSize);
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

        public void Frame(Graphics g, TimeSpan delta)
        {
            if (AutoCycling)
            {
                MilliSinceLastAuto += (int)delta.TotalMilliseconds;
                if (MilliSinceLastAuto > (int)(1000f / AutoCyclesPerSecond))
                {
                    MilliSinceLastAuto = 0;
                    Cycle();
                }
            }

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var state = GetState(new Point(x, y));
                    DrawState(g, new Point(x, y), state, StateDrawMode.Tile);
                }
            }

            //Draw tile grid
            var width = (map.GetUpperBound(0) + 1) * SquareSize;
            var height = (map.GetUpperBound(1) + 1) * SquareSize;

            for (var x = 0; x < width; x += SquareSize)
            {
                g.DrawLine(GridColor, x, 0, x, height);
            }
            for (var y = 0; y < height; y += SquareSize)
            {
                g.DrawLine(GridColor, 0, y, width, y);
            }
        }
    }

    public enum StateDrawMode
    {
        Tile,
        Literal,
        LiteralMatched,
    }

    public enum ColorDrawMode
    {
        AlwaysGray,
        StateMatch,
    }
}