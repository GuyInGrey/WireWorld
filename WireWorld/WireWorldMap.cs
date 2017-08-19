using System;
using System.Drawing;

namespace WireWorld
{
    public class WireWorldMap
    {
        private WireWorldState[,] Map;
        private WireWorldState[,] tempMap;
        public int Width { get; private set; }
        public int Height { get; private set; }

        Point[] surrounding;

        public WireWorldMap(Size size) : this(size.Width, size.Height) { }
        public WireWorldMap(int width, int height)
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

            Map = new WireWorldState[width, height];
            Width = width;
            Height = height;
        }

        public void Cycle()
        {
            if (Map != null)
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

                            if (_x > 0 && _x < Map.GetUpperBound(0) + 1)
                            {
                                if (_y > 0 && _y < Map.GetUpperBound(1) + 1)
                                {
                                    if (Map[_x, _y] == WireWorldState.Head)
                                    {
                                        headCnt++;
                                    }
                                }
                            }
                        }

                        tempMap[x, y] = TileCycle(headCnt, Map[x, y]);
                    }
                }

                Map = tempMap;
            }
        }

        public void SetState(int x, int y, WireWorldState state)
        {
            try
            {
                Map[x, y] = state;
            }
            catch
            {

            }
        }
        public WireWorldState GetState(Point point) => Map[point.X, point.Y];

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
    }
}
