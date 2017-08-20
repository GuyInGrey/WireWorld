using System;
using System.Drawing;
using System.Windows.Forms;

using GdiApi;

namespace WireWorld
{
    public class Game
    {
        Context context;
        Context controlWindow;
        FrameRateManager frameRate = new FrameRateManager();
        WireWorldMap map;

        SolidBrush headColor = new SolidBrush(Color.Blue);
        SolidBrush tailColor = new SolidBrush(Color.Red);
        SolidBrush wireColor = new SolidBrush(Color.Yellow);
        SolidBrush blankColor = new SolidBrush(Color.Black);
        Pen borderColor = new Pen(Color.White, 1f);
        int squareSize = 15;
        WireWorldState selected = WireWorldState.Dead;

        Point[] borderLines = new Point[5];
        Point[] mouseLineMarkers = new Point[5];
        Point[] mouseLines = new Point[5];

        public Game(int squareSize, int width, int height)
        {
            this.squareSize = squareSize;
            map = new WireWorldMap(width, height);

            context = new Context(new Size(1000, 1000), "Fun Time!", false);
            controlWindow = new Context(new Size(300, 1000), "Controls", false);

            borderLines[0] = new Point(0, 0);
            borderLines[1] = new Point(map.Width * squareSize, 0);
            borderLines[2] = new Point(map.Width * squareSize, map.Height * squareSize);
            borderLines[3] = new Point(0, map.Height * squareSize);
            borderLines[4] = new Point(0, 0);

            mouseLineMarkers[0] = new Point(0,0);
            mouseLineMarkers[1] = new Point(squareSize, 0);
            mouseLineMarkers[2] = new Point(squareSize, squareSize);
            mouseLineMarkers[3] = new Point(0, squareSize);
            mouseLineMarkers[4] = new Point(0, 0);

            context.Load += Load;
            context.Render += Render;
            context.KeyDown += Context_KeyDown;

            controlWindow.ClearScreen = false;
            controlWindow.ManageFrameDraw = false;

            var cycleBtn = new Button()
            {
                Size = new Size(300, 20),
                Visible = true,
                Location = new Point(0,0),
                Text = "Cycle",
                Tag = "0.95|0.02",
            };
            cycleBtn.Click += CycleBtn_Click;
            controlWindow.Controls.Add(cycleBtn);

            controlWindow.Closing += ControlWindow_Closing;
            controlWindow.Resize += ControlWindow_Resize;

            context.Begin(false);
        }

        private void ControlWindow_Resize()
        {
            foreach (var c in controlWindow.Controls)
            {
                if (float.TryParse((c as Control).Tag.ToString().Split('|')[0], out var sizeWidth))
                {
                    if (float.TryParse((c as Control).Tag.ToString().Split('|')[1], out var sizeHeight))
                    {
                        (c as Control).Size = new Size((int)(controlWindow.Size.Width * sizeWidth), (int)(controlWindow.Size.Height * sizeHeight));
                    }
                }
            }
        }
        private void ControlWindow_Closing(FormClosingEventArgs fcea) => fcea.Cancel = true;
        private void CycleBtn_Click(object sender, EventArgs e) => map.Cycle();

        private void Context_KeyDown(KeyEventArgs kea)
        {
            switch (kea.KeyCode)
            {
                case Keys.Space:
                    map.Cycle();
                    break;
                case Keys.D1:
                    selected = WireWorldState.Dead;
                    break;
                case Keys.D3:
                    selected = WireWorldState.Head;
                    break;
                case Keys.D4:
                    selected = WireWorldState.Tail;
                    break;
                case Keys.D2:
                    selected = WireWorldState.Wire;
                    break;
            }
        }

        public void Load()
        {
            BitmapBuffer.Clear();
            BitmapBuffer.Load(@"assets\");

            controlWindow.Begin(true);
        }

        public void DrawState(Graphics g, Point p, WireWorldState state)
        {
            switch (state)
            {
                case WireWorldState.Head:
                    g.FillRectangle(headColor, new Rectangle(p.X * squareSize, p.Y * squareSize, squareSize, squareSize));
                    break;
                case WireWorldState.Tail:
                    g.FillRectangle(tailColor, new Rectangle(p.X * squareSize, p.Y * squareSize, squareSize, squareSize));
                    break;
                case WireWorldState.Wire:
                    g.FillRectangle(wireColor, new Rectangle(p.X * squareSize, p.Y * squareSize, squareSize, squareSize));
                    break;
                case WireWorldState.Dead:
                    g.FillRectangle(blankColor, new Rectangle(p.X * squareSize, p.Y * squareSize, squareSize, squareSize));
                    break;
            }
        }

        public void DrawStateLiteral(Graphics g, Point p, WireWorldState state)
        {
            switch (state)
            {
                case WireWorldState.Head:
                    g.FillRectangle(headColor, new Rectangle(p.X, p.Y, squareSize, squareSize));
                    break;
                case WireWorldState.Tail:
                    g.FillRectangle(tailColor, new Rectangle(p.X, p.Y, squareSize, squareSize));
                    break;
                case WireWorldState.Wire:
                    g.FillRectangle(wireColor, new Rectangle(p.X, p.Y, squareSize, squareSize));
                    break;
                case WireWorldState.Dead:
                    g.FillRectangle(blankColor, new Rectangle(p.X, p.Y, squareSize, squareSize));
                    break;
            }
        }

        public void Render(Graphics graphics, TimeSpan delta)
        {
            frameRate.Frame(delta);

            if (context.MouseClicked)
            {
                map.SetState(context.MouseLocation.X / squareSize,
                    context.MouseLocation.Y / squareSize, selected);
            }

            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var state = map.GetState(new Point(x, y));
                    DrawState(graphics, new Point(x, y), state);
                }
            }

            graphics.DrawLines(borderColor, borderLines);

            for (var x = 0; x < context.Size.Width; x += squareSize)
            {
                graphics.DrawLine(borderColor, x, 0, x, context.Size.Height);
            }

            for (var y = 0; y < context.Size.Height; y += squareSize)
            {
                graphics.DrawLine(borderColor, 0, y, context.Size.Width, y);
            }

            DrawStateLiteral(graphics, context.MouseLocation, selected);

            for (var i = 0; i < 5; i++)
            {
                mouseLines[i] = new Point(mouseLineMarkers[i].X + context.MouseLocation.X,
                    mouseLineMarkers[i].Y + context.MouseLocation.Y);
            }

            graphics.DrawLines(borderColor, mouseLines);

            context.Title = "Framerate: " + frameRate.ToString();
        }
    }
}
