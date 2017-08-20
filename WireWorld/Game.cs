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
        bool autoRunning = false;
        float cyclesPerSecond = 3;

        Point[] borderLines = new Point[5];
        Point[] mouseLineMarkers = new Point[5];
        Point[] mouseLines = new Point[5];

        public Game(int squareSize, int width, int height)
        {
            this.squareSize = squareSize;
            map = new WireWorldMap(width, height);

            context = new Context(new Size(1000, 1000), "Fun Time!", false);
            controlWindow = new Context(new Size(200, 1000), "Controls", false);

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
                Size = new Size(200, 20),
                Visible = true,
                Location = new Point(0,0),
                Text = "Cycle",
            };
            cycleBtn.Click += CycleBtn_Click;
            cycleBtn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            controlWindow.Controls.Add(cycleBtn);

            var autoToggle = new Button()
            {
                Size = new Size(200, 20),
                Visible = true,
                Location = new Point(0, 25),
                Text = "Toggle Auto",
            };
            autoToggle.Click += AutoToggle_Click;
            autoToggle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            controlWindow.Controls.Add(autoToggle);

            var increaseAuto = new Button()
            {
                Size = new Size(200, 20),
                Visible = true,
                Location = new Point(0, 50),
                Text = "Increase Auto Speed",
            };
            increaseAuto.Click += IncreaseAuto_Click;
            increaseAuto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            controlWindow.Controls.Add(increaseAuto);

            var decreaseAuto = new Button()
            {
                Size = new Size(200, 20),
                Visible = true,
                Location = new Point(0, 75),
                Text = "Decrease Auto Speed",
            };
            decreaseAuto.Click += DecreaseAuto_Click;
            decreaseAuto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            controlWindow.Controls.Add(decreaseAuto);

            controlWindow.Closing += ControlWindow_Closing;
            controlWindow.Resize += ControlWindow_Resize;

            context.Begin(false);
        }

        private void DecreaseAuto_Click(object sender, EventArgs e) => cyclesPerSecond *= 0.7f;
        private void IncreaseAuto_Click(object sender, EventArgs e) => cyclesPerSecond *= 1.3f;
        private void AutoToggle_Click(object sender, EventArgs e) => autoRunning = !autoRunning;

        private void ControlWindow_Resize()
        {

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
                case Keys.NumPad1:
                case Keys.D1:
                    selected = WireWorldState.Dead;
                    break;
                case Keys.NumPad3:
                case Keys.D3:
                    selected = WireWorldState.Head;
                    break;
                case Keys.NumPad4:
                case Keys.D4:
                    selected = WireWorldState.Tail;
                    break;
                case Keys.NumPad2:
                case Keys.D2:
                    selected = WireWorldState.Wire;
                    break;
                case Keys.A:
                    autoRunning = !autoRunning;
                    break;
                case Keys.OemPeriod:
                    cyclesPerSecond *= 1.3f;
                    break;
                case Keys.Oemcomma:
                    cyclesPerSecond *= 0.7f;
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

        int MilliSinceLastAuto = 0;

        public void Render(Graphics graphics, TimeSpan delta)
        {
            //Auto
            if (autoRunning)
            {
                MilliSinceLastAuto += (int)delta.TotalMilliseconds;
                if (MilliSinceLastAuto > (int)(1000f / cyclesPerSecond))
                {
                    MilliSinceLastAuto = 0;
                    map.Cycle();
                }
            }
            
            //Get Framerate
            frameRate.Frame(delta);

            //Set square if mouse clicked
            if (context.MouseClicked)
            {
                map.SetState(context.MouseLocation.X / squareSize,
                    context.MouseLocation.Y / squareSize, selected);
            }

            //Draw Map
            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var state = map.GetState(new Point(x, y));
                    DrawState(graphics, new Point(x, y), state);
                }
            }

            //Draw border around map
            graphics.DrawLines(borderColor, borderLines);

            //Draw tile grid
            for (var x = 0; x < context.Size.Width; x += squareSize)
            {
                graphics.DrawLine(borderColor, x, 0, x, context.Size.Height);
            }
            for (var y = 0; y < context.Size.Height; y += squareSize)
            {
                graphics.DrawLine(borderColor, 0, y, context.Size.Width, y);
            }

            //Cursor tile
            DrawStateLiteral(graphics, context.MouseLocation, selected);

            //Cursor tile border
            for (var i = 0; i < 5; i++)
            {
                mouseLines[i] = new Point(mouseLineMarkers[i].X + context.MouseLocation.X,
                    mouseLineMarkers[i].Y + context.MouseLocation.Y);
            }
            graphics.DrawLines(borderColor, mouseLines);

            //Set title to framerate
            context.Title = "Framerate: " + frameRate.ToString();
        }
    }
}