﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NotWorldOfTanks.Model;
using NotWorldOfTanks.View;
using Point = System.Drawing.Point;

namespace NotWorldOfTanks
{

    public partial class Form1 : Form
    {
        private AreaView _area = default(AreaView);
        private WallView _wallView = default(WallView);
        private TankView _tankView = default(TankView);
        private List<Point> points = default(List<Point>);
        private Point tankPoint = default(Point);
        private direction _direction = default(direction);


        public Form1(AreaView area)
        {
            _area = area;

            InitializeComponent();

            this.Size = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2,
                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2);
            pictureBox1.Left = area.LocationArea(ClientSize.Width, pictureBox1.Width);
            pictureBox1.Top = area.LocationArea(ClientSize.Height, pictureBox1.Height);

            this.pictureBox1.Size = _area.AreaSize;
            this.pictureBox1.Size = _area.SetScale(this.Size, pictureBox1.Size);

            this.MinimumSize = new Size(pictureBox1.Width + 150, pictureBox1.Height + 150);

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _tankView = new TankView();
            _wallView = new WallView(_area.AreaSize.Height, _area.AreaSize.Width);
            SetWall();
            tankPoint = TankStart(1);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            pictureBox1.Left = ClientSize.Width / 2 - pictureBox1.Width / 2;
            pictureBox1.Top = ClientSize.Height / 2 - pictureBox1.Height / 2;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Random r = new Random();
            bool crashed = false;

            int dir = r.Next(1, 5);
            int res = r.Next(0, 2);


            for (int i = 0; i < _tankView._tank.Count; i++)
            {

                    if ((_tankView._tank[i].Direction is direction.Up) && _tankView._tank[i].Y > 30 &&
                        !points.Any(n => n.Equals(new Point(_tankView._tank[i].X, _tankView._tank[i].Y - 30))))
                    {
                        _tankView._tank[i].Y -= 1;
                        if (res == 1)
                        {
                            if (dir == 1) break;
                            else if (dir == 2) _tankView._tank[i].Direction = direction.Left;
                            else if (dir == 3) _tankView._tank[i].Direction = direction.Down;
                            else if (dir == 4) _tankView._tank[i].Direction = direction.Right;
                        }
                        break;

                }



                if ((_tankView._tank[i].Direction is direction.Down) && _tankView._tank[i].Y < _area.AreaSize.Width - 30 &&
                    !points.Any(n => n.Equals(new Point(_tankView._tank[i].X, _tankView._tank[i].Y + 30))))
                {
                    _tankView._tank[i].Y += 1;
                    if (res == 1)
                    {
                        if (dir == 1) break;
                        else if (dir == 2) _tankView._tank[i].Direction = direction.Left;
                        else if (dir == 3) _tankView._tank[i].Direction = direction.Up;
                        else if (dir == 4) _tankView._tank[i].Direction = direction.Right;
                    }
                    break;
                }


                if ((_tankView._tank[i].Direction is direction.Left) && _tankView._tank[i].X > 30 &&
                    !points.Any(n => n.Equals(new Point(_tankView._tank[i].X - 30, _tankView._tank[i].Y))))
                {
                    _tankView._tank[i].X -= 1;
                    if (res == 1)
                    {
                        if (dir == 1) break;
                        else if (dir == 2) _tankView._tank[i].Direction = direction.Down;
                        else if (dir == 3) _tankView._tank[i].Direction = direction.Up;
                        else if (dir == 4) _tankView._tank[i].Direction = direction.Right;
                    }
                    break;
                }

                if ((_tankView._tank[i].Direction is direction.Right) && _tankView._tank[i].X < _area.AreaSize.Height - 30 &&
                    !points.Any(n => n.Equals(new Point(_tankView._tank[i].X + 30, _tankView._tank[i].Y))))
                {
                    _tankView._tank[i].X += 1;
                    if (res == 1)
                    {
                        if (dir == 1) break;
                        else if (dir == 2) _tankView._tank[i].Direction = direction.Left;
                        else if (dir == 3) _tankView._tank[i].Direction = direction.Up;
                        else if (dir == 4) _tankView._tank[i].Direction = direction.Down;
                    }
                    break;
                }
            }

            Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int height = 0; height < _area.AreaSize.Width; height += 30)
            {
                g.DrawImage(Resource_NWoT.wall, new Rectangle(height, 0, 30, 30));
            }

            for (int width = 0; width < _area.AreaSize.Height; width += 30)
            {
                g.DrawImage(Resource_NWoT.wall, new Rectangle(0, width, 30, 30));
            }

            for (int height = 0; height < _area.AreaSize.Width; height += 30)
            {
                g.DrawImage(Resource_NWoT.wall, new Rectangle(height, _area.AreaSize.Height - 30, 30, 30));
            }

            for (int width = 0; width < _area.AreaSize.Height; width += 30)
            {
                g.DrawImage(Resource_NWoT.wall, new Rectangle(_area.AreaSize.Width - 30, width, 30, 30));
            }

            foreach (var point in points)
            {
                g.DrawImage(Resource_NWoT.wall, new Rectangle(point.X, point.Y, 30, 30));
            }
;
            for (int i = 0; i < _tankView._tank.Count; i++)
            {
                g.DrawImage(Resource_NWoT.Tank, new Rectangle(_tankView._tank[i].X, _tankView._tank[i].Y, 30, 30));

            }
        }

        private void SetWall()
        {
            Random r_h = new Random();

            int count = _area.AreaSize.Width / 10 - 10;
            points = new List<Point>();
            int a = 0, b = 0;
            for (int i = 0; i < count; i++)
            {
                a = r_h.Next(30, _area.AreaSize.Height - 30) / 30 * 30;
                b = r_h.Next(30, _area.AreaSize.Width - 30) / 30 * 30;

                if (!points.Any(n => new Point(a, b).Equals(n)))
                {
                    points.Add(new Point(a, b));

                }
                else i -= 1;

            }
        }

        private Point TankStart(int numTank)
        {
            Point point = default(Point);

            Random r_h = new Random();

            int count = _area.AreaSize.Width * _area.AreaSize.Height - _area.AreaSize.Width / 10 -
                        (_area.AreaSize.Width * 2 + _area.AreaSize.Height * 2) + 4;

            int a = 0, b = 0;
            for (int j = 0; j < numTank; j++)
            {
                for (int i = 0; i < count; i++)
                {
                    a = r_h.Next(30, _area.AreaSize.Height - 30) / 30 * 30;
                    b = r_h.Next(30, _area.AreaSize.Width - 30) / 30 * 30;

                    if (!points.Any(n => new Point(a, b).Equals(n)))
                    {
                        point = new Point(a, b);
                        _tankView.AddTank(point.X, point.Y);
                        break;

                    }
                    else i -= 1;

                }
            }


            return point;
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;

        }
    }
}
