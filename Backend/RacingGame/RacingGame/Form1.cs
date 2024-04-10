using GameLogic;
using System.Diagnostics;
using System.Numerics;

namespace RacingGame
{
    public partial class Form1 : Form
    {
        readonly GameController GameController = new();
        readonly GameAgent GameAgent = new();

        public Vector2 CameraPosition { get; set; } = new Vector2(0, 0);

        public bool Running { get; set; } = true;

        public PictureBox pb = new();

        public Form1()
        {
            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            this.Controls.Add(pb);
            pb.Dock = DockStyle.Fill;

            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            GameAgent.Position = new Vector2(30, 0);

            GameController.AddAgent(GameAgent);
        }

        public void Render()
        {
            try
            {
                Vector2 offset = new(Width / 2 - GameController.TrackWidth / 2, Height / 2);
                Vector2 multiplier = new(1, -1);
                int cnt = 0;

                int width = Width;
                int height = Height;

                Bitmap bitmap = new Bitmap(width, height);
                var g = Graphics.FromImage(bitmap);
                g.Clear(Color.SaddleBrown);

                Vector2 previousTrackPiece = GameController.Track.First() - CameraPosition;
                previousTrackPiece *= multiplier;
                previousTrackPiece += offset;
                var trackParts = GameController.Track.Where(x => Vector2.Distance(x, CameraPosition) < 900);
                int trackPartsSkipped = GameController.Track.ToList().IndexOf(trackParts.First());
                cnt += trackPartsSkipped;
                foreach (var item in trackParts)
                {
                    cnt++;
                    bool drawCool = ((cnt % (199 * 8)) / 199) % 2 == 0;

                    var pos = item - CameraPosition;
                    pos *= multiplier;
                    pos += offset;
                    var trackWidth = GameController.TrackWidth;

                    if (drawCool)
                    {
                        Vector2 centerPointOfLastPoint = previousTrackPiece + new Vector2(trackWidth / 2, 0);
                        Vector2 centerPointOfCurrentPoint = pos + new Vector2(trackWidth / 2, 0);

                        Vector2 direction = centerPointOfCurrentPoint - centerPointOfLastPoint;

                        Vector2 normal = new Vector2(direction.Y, -direction.X);

                        normal = Vector2.Normalize(normal);

                        Vector2[] points = new Vector2[]
                        {
                            centerPointOfLastPoint + normal * 10,
                            centerPointOfLastPoint - normal * 10,
                            centerPointOfCurrentPoint - normal * 10,
                            centerPointOfCurrentPoint + normal * 10
                        };

                        g.FillPolygon(Brushes.Gray, points.Select(x => new PointF(x.X, x.Y)).ToArray());
                    }

                    //g.FillRectangle(Brushes.Gray, pos.X, pos.Y, trackWidth, 10);

                    g.DrawLine(Pens.White, previousTrackPiece.X, previousTrackPiece.Y, pos.X, pos.Y);

                    g.DrawLine(Pens.White, previousTrackPiece.X + trackWidth, previousTrackPiece.Y, pos.X + trackWidth, pos.Y);

                    previousTrackPiece = pos;
                }
                //previousTrackPiece = GameController.Track.First() - CameraPosition;
                //previousTrackPiece *= multiplier;
                //previousTrackPiece += offset;
                //foreach (var item in GameController.TrackRaw)
                //{
                //    var pos = item - CameraPosition;
                //    pos *= multiplier;
                //    pos += offset;
                //    var trackWidth = GameController.TrackWidth;

                //    g.FillRectangle(Brushes.Gray, pos.X, pos.Y, trackWidth, 10);

                //    g.DrawLine(Pens.Magenta, previousTrackPiece.X, previousTrackPiece.Y, pos.X, pos.Y);

                //    g.DrawLine(Pens.Magenta, previousTrackPiece.X + trackWidth, previousTrackPiece.Y, pos.X + trackWidth, pos.Y);

                //    previousTrackPiece = pos;
                //}

                foreach (var agent in GameController.Agents)
                {
                    var pos = agent.Position - CameraPosition;
                    pos *= multiplier;
                    pos += offset;

                    Vector2 forward = new((float)Math.Cos(agent.Rotation), (float)Math.Sin(agent.Rotation));
                    forward = new Vector2(forward.Y, forward.X);

                    var frontCarPos = pos;
                    frontCarPos += forward * multiplier * agent.FrontCarPosition.Length();

                    var backCarPos = pos;
                    backCarPos -= forward * multiplier * agent.FrontCarPosition.Length();

                    // draw steering direction
                    var steeringDirection = agent.SteeringDirection;
                    var steeringDirectionNormal = new Vector2(steeringDirection.Y, -steeringDirection.X);
                    var steeringPos = frontCarPos + steeringDirection * multiplier * 20;

                    //g.DrawLine(Pens.Yellow, frontCarPos.X, frontCarPos.Y, steeringPos.X, steeringPos.Y);

                    int carWidth = 20;
                    int carHeight = (int)(agent.FrontCarPosition.Length() * 2);
                    Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(agent.Rotation);
                    Vector2[] carPoints = new Vector2[]
                    {
                        new(-carWidth / 2, -carHeight / 2),
                        new(carWidth / 2, -carHeight / 2),
                        new(carWidth / 2, carHeight / 2),
                        new(-carWidth / 2, carHeight / 2)
                    };

                    for (int i = 0; i < carPoints.Length; i++)
                    {
                        carPoints[i] = Vector2.Transform(carPoints[i], rotationMatrix);
                        carPoints[i] += pos;
                    }


                    Vector2[] frontWheelPositions = new Vector2[]
                    {
                        frontCarPos + Vector2.Transform(new Vector2(10, 10), rotationMatrix),
                        frontCarPos - Vector2.Transform(new Vector2(10, -10), rotationMatrix)
                    };
                    Vector2[] backWheelPositions = new Vector2[]
                    {
                        backCarPos + Vector2.Transform(new Vector2(10, -10), rotationMatrix),
                        backCarPos - Vector2.Transform(new Vector2(10, 10), rotationMatrix)
                    };

                    Vector2[] wheelPoints = new Vector2[]
                    {
                        new(-3, -7),
                        new(3, -7),
                        new(3, 7),
                        new(-3, 7)
                    };

                    foreach (var item in backWheelPositions)
                    {
                        var copyWheels = wheelPoints.ToArray();
                        for (int i = 0; i < wheelPoints.Length; i++)
                        {
                            copyWheels[i] = Vector2.Transform(copyWheels[i], rotationMatrix);
                            copyWheels[i] += item;
                        }

                        g.FillPolygon(Brushes.Black, copyWheels.Select(x => new PointF(x.X, x.Y)).ToArray());
                    }

                    Matrix3x2 steeringMatrix = Matrix3x2.CreateRotation(agent.SteeringAngle);

                    foreach (var item in frontWheelPositions)
                    {
                        var copyWheels = wheelPoints.ToArray();
                        for (int i = 0; i < wheelPoints.Length; i++)
                        {
                            copyWheels[i] = Vector2.Transform(copyWheels[i], rotationMatrix);
                            copyWheels[i] = Vector2.Transform(copyWheels[i], steeringMatrix);
                            copyWheels[i] += item;
                        }

                        g.FillPolygon(Brushes.Black, copyWheels.Select(x => new PointF(x.X, x.Y)).ToArray());
                    }

                    g.FillPolygon(Brushes.Red, carPoints.Select(x => new PointF(x.X, x.Y)).ToArray());

                }

                g.Dispose();
                renderSemaphore.WaitOne();
                pb.Image?.Dispose();
                pb.Image = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Running = false;
                Close();
            }
            finally
            {
                renderSemaphore.Release();
            }
        }
        Semaphore renderSemaphore = new Semaphore(1, 1);
        public void Loop()
        {
            Stopwatch w = new Stopwatch();
            while (Running)
            {
                w.Stop();
                GameController.Update((float)w.Elapsed.TotalSeconds);
                w.Restart();
                // lerp camera
                CameraPosition = Vector2.Lerp(CameraPosition, GameAgent.Position, 0.1f);
                try
                {
                    Invoke(new MethodInvoker(Render));
                    //Render();
                }
                catch (Exception)
                {

                }
                Application.DoEvents();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GameController.GenerateTrack(100, 0);

            var thread = new Thread(Loop)
            {
                IsBackground = true
            };
            thread.Start();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                GameAgent.ForwardControl = true;
            }
            if (e.KeyCode == Keys.S)
            {
                GameAgent.BackwardControl = true;
            }
            if (e.KeyCode == Keys.A)
            {
                GameAgent.LeftControl = true;
            }
            if (e.KeyCode == Keys.D)
            {
                GameAgent.RightControl = true;
            }
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                GameAgent.ForwardControl = false;
            }
            if (e.KeyCode == Keys.S)
            {
                GameAgent.BackwardControl = false;
            }
            if (e.KeyCode == Keys.A)
            {
                GameAgent.LeftControl = false;
            }
            if (e.KeyCode == Keys.D)
            {
                GameAgent.RightControl = false;
            }
        }
    }
}
