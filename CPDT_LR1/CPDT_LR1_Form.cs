using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace CSharp_data_visualisation
{
    public partial class Application : Form
    {
        bool pause_flag = false;

        private readonly int trail_length = 60;
        private readonly int centroid_thresh = 100;
        private readonly int min_points = 7;

        private readonly List<FoundObject> frame_objects = new List<FoundObject>();
        private readonly List<FoundObject> found_objects = new List<FoundObject>();
        private readonly List<Point> frame_centroids = new List<Point>();
        private readonly List<Point> frame_points = new List<Point>();
        private readonly List<List<int>> data = new List<List<int>>();

        private int current_value = 0, current_line = 0;
        private List<string> time = new List<string>();
        private Bitmap frame;

        public Application()
        {
            InitializeComponent();
        }


        private void Application_Load(object sender, EventArgs e)
        {
            Read_Log();

            this.framerate.Interval = 100;
            this.framerate.Start();
        }


        private void Application_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            this.objects_grid.ClearSelection();
        }


        private void Btn_pp_Click(object sender, EventArgs e)
        {
            if (pause_flag)
            {
                this.framerate.Start();
                this.btn_pp.Text = "Pause";
            }
            else
            {
                this.framerate.Stop();
                this.btn_pp.Text = "Resume";
            }

            pause_flag = !pause_flag;
        }


        private void Btn_pp_MouseDown(object sender, MouseEventArgs e)
        {
            this.btn_pp.BackColor = Color.Fuchsia;
        }


        private void Btn_pp_MouseUp(object sender, MouseEventArgs e)
        {
            this.btn_pp.BackColor = Color.FromArgb(64, 64, 64);
        }



        private void Framerate_Tick(object sender, EventArgs e)
        {
            Draw_Data();
            Find_Objects();
        }


        private void Read_Log()
        {
            try
            {
                using (StreamReader reader = new StreamReader("dump.txt"))
                {
                    string message = reader.ReadToEnd();

                    foreach (string line in message.Split('\n'))
                    {
                        if (line == "")
                            continue;

                        int l_index = line.LastIndexOf('>') + 2,
                            e_index = line.Count() - 2 - l_index;

                        data.Add(line.Substring(l_index, e_index).Split(' ').Select(Int32.Parse).ToList());
                        time.Add(line.Substring(0, l_index - 2));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application did an oopsie", ex.Message);
            }
        }


        private void Draw_Data()
        {
            if (current_value >= data[current_line].Count)
            {
                current_line++;
                current_value = 0;
                frame_points.Clear();
            }

            if (current_line >= data.Count)
            {
                frame_points.Clear();
                current_line = 0;
            }

            frame = new Bitmap(this.frame_box.Width, this.frame_box.Height);

            while (current_value < data[current_line].Count)
            {
                double angle = (60.0 + current_value * 0.36) * Math.PI / 180;

                int x = (int)(data[current_line][current_value] *
                    Math.Sin(angle));

                int y = (int)(data[current_line][current_value] *
                    Math.Cos(angle));

                x = (int)(x * 0.08 + this.frame_box.Width / 2);
                y = (int)(y * 0.08 + this.frame_box.Height / 1.2);

                frame_points.Add(new Point(x, y));

                if (data[current_line][current_value] <= 19)
                {
                    current_value++;
                    continue;
                }

                using (Graphics g = Graphics.FromImage(frame))
                    g.FillEllipse(Brushes.Fuchsia, x, y, 3, 3);

                current_value++;
            }

            this.frame_box.Image = frame;
        }


        private void Find_Objects()
        {
            frame_objects.Clear();
            frame_centroids.Clear();
            var processed_frame = data[current_line];
            FoundObject fo = new FoundObject();
            FoundObject comparableFo = new FoundObject();
            bool isInComparableFo = false;

            for (int i = 1; i < processed_frame.Count; i++)
            {
                if (Math.Abs(processed_frame[i] - processed_frame[i - 1]) >= 100)
                {
                    if (isInComparableFo)
                    {
                        if (comparableFo.points.Count > min_points)
                            frame_objects.Add(comparableFo);

                        if (fo.values.Count > 0 && Math.Abs(processed_frame[i] - fo.values.Last()) <= 20)
                        {
                            fo.points.Add(frame_points[i]);
                            fo.values.Add(processed_frame[i]);
                        }
                        else
                        {
                            if (fo.points.Count > min_points)
                                frame_objects.Add(fo);

                            fo = new FoundObject(processed_frame[i], frame_points[i]);
                        }

                        isInComparableFo = false;
                    }
                    else
                    {
                        comparableFo = new FoundObject(processed_frame[i], frame_points[i]);
                        isInComparableFo = true;
                    }
                }
                else
                {
                    if (!isInComparableFo)
                    {
                        if (fo.points.Count >= (int)num_max_points.Value)
                        {
                            if (fo.points.Count > min_points)
                                frame_objects.Add(fo);

                            fo = new FoundObject(processed_frame[i], frame_points[i]);
                        }
                        else
                        {
                            fo.points.Add(frame_points[i]);
                            fo.values.Add(processed_frame[i]);
                        }
                    }
                    else
                    {
                        if (comparableFo.points.Count >= (int)num_max_points.Value)
                        {
                            if (comparableFo.points.Count > min_points)
                                frame_objects.Add(comparableFo);

                            comparableFo = new FoundObject(processed_frame[i], frame_points[i]);
                        }
                        else
                        {
                            comparableFo.points.Add(frame_points[i]);
                            comparableFo.values.Add(processed_frame[i]);
                        }
                    }
                }
            }

            frame = new Bitmap(frame);
            this.objects_grid.Rows.Clear();

            frame_objects.RemoveAll(o => o.points.Count(p => (Math.Abs(p.X - o.points[0].X) < 3) || (Math.Abs(p.Y - o.points[0].Y) < 3)) > o.points.Count * 5 / 6);

            if (found_objects.Count == 0)
                frame_objects.ForEach(o => found_objects.Add(o));

            foreach (var obj in frame_objects)
            {
                int min_x = obj.points.Min(p => p.X),
                    min_y = obj.points.Min(p => p.Y),
                    max_x = obj.points.Max(p => p.X),
                    max_y = obj.points.Max(p => p.Y);

                Point centroid = new Point((min_x + max_x) / 2, (min_y + max_y) / 2),
                      endpoint = obj.points.Last(),
                      startpoint = obj.points.First();

                frame_centroids.Add(centroid);

                if (obj.centroid_frames.Count >= trail_length)
                    obj.centroid_frames.RemoveAt(0);
                obj.centroid_frames.Add(centroid);

                using (Graphics g = Graphics.FromImage(frame))
                {
                    if (startpoint.X == endpoint.X)
                    {
                        startpoint = obj.points.Last(p => p.X == min_x);
                        endpoint = obj.points.Last(p => p.X == max_x);
                        g.DrawLine(new Pen(Color.Blue, 4), startpoint, endpoint);
                    }
                    else
                    {
                        double slope = (startpoint.Y - endpoint.Y) / (double)(startpoint.X - endpoint.X);
                        double y_intersect = -slope * startpoint.X + startpoint.Y;

                        if (obj.points.Count(p => Math.Abs(p.Y - (slope * p.X) - y_intersect) <= 5) >= obj.points.Count * 5/6)
                            g.DrawLine(new Pen(Color.Blue, 4), startpoint, endpoint);
                        else
                        {
                            g.DrawRectangle(Pens.Fuchsia, min_x, min_y, max_x - min_x, max_y - min_y);
                            Find_Closest_Centroid(obj, centroid);

                            this.objects_grid.Rows.Add(new object[] {
                                time[current_line],
                                frame_objects.IndexOf(obj),
                                obj.centroid_frames.Last()
                            });

                            g.FillEllipse(Brushes.Red, centroid.X, centroid.Y, 5, 5);

                            found_objects.ForEach(o =>
                            {
                                if (o.centroid_frames.Count > 1)
                                    g.DrawLines(new Pen(Color.Green, 2), o.centroid_frames.ToArray());
                            });
                        }
                    }
                }
            }

            found_objects.RemoveAll(foo => !frame_centroids.Contains(foo.centroid_frames.Last()));

            this.frame_box.Image = frame;
        }


        private void Find_Closest_Centroid(FoundObject o, Point c)
        {
            int goal_distance = (int)Math.Sqrt(Math.Pow(found_objects[0].centroid_frames.Last().X - c.X, 2) +
                                               Math.Pow(found_objects[0].centroid_frames.Last().Y - c.Y, 2));

            var closest_object = found_objects[0];

            foreach (var obj_j in found_objects)
            {
                int obj_distance = (int)Math.Sqrt(Math.Pow(obj_j.centroid_frames.Last().X - c.X, 2) +
                                        Math.Pow(obj_j.centroid_frames.Last().Y - c.Y, 2));

                if (obj_distance < goal_distance)
                {
                    goal_distance = obj_distance;
                    closest_object = obj_j;
                }
            }

            if (goal_distance <= centroid_thresh)
            {
                if (o.centroid_frames.Count >= trail_length)
                    o.centroid_frames.RemoveAt(0);
                o.centroid_frames.Add(c);

                found_objects.Remove(closest_object);
                found_objects.Add(o);
            }
            else if (!found_objects.Contains(o))
                found_objects.Add(o);
        }


        private void Objects_Grid_SelectionChanged(object sender, EventArgs e)
        {
            var bmp = new Bitmap(frame);

            if (pause_flag && objects_grid.SelectedCells.Count != 0)
            {
                var index = objects_grid.SelectedCells[0].RowIndex;
                var obj = frame_objects.ElementAt((int)objects_grid.Rows[index].Cells[1].Value);

                int min_x = obj.points.Min(p => p.X),
                    min_y = obj.points.Min(p => p.Y),
                    max_x = obj.points.Max(p => p.X),
                    max_y = obj.points.Max(p => p.Y);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawRectangle(new Pen(Color.Yellow, 2),
                                    min_x,
                                    min_y,
                                    max_x - min_x,
                                    max_y - min_y
                        );
                }

                this.frame_box.Image = bmp;
            }
        }


        private void objects_grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                this.btn_pp.PerformClick();
            if (e.KeyCode == Keys.W && ModifierKeys == Keys.Control)
                this.Close();
        }


        private class FoundObject
        {
            public List<Point> points, centroid_frames;
            public readonly List<int> values;

            public FoundObject()
            {
                this.points = new List<Point>();
                this.centroid_frames = new List<Point>();
                this.values = new List<int>();
            }

            public FoundObject(int v, Point p)
            {
                this.points = new List<Point> { p };
                this.centroid_frames = new List<Point> { p };
                this.values = new List<int> { v };
            }
        }
    }
}
