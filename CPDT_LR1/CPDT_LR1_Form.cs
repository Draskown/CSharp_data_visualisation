using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace LRs
{
    public partial class Application : Form
    {
        bool pause_flag = false;

        readonly List<List<int>> data = new List<List<int>>();
        readonly List<FoundObject> found_objects = new List<FoundObject>();
        public List<Point> current_points = new List<Point>();

        int current_value = 0, current_line = 0;
        Bitmap frame;

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
                current_points.Clear();
            }

            if (current_line >= data.Count)
            {
                current_points.Clear();
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

                current_points.Add(new Point(x, y));

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
            found_objects.Clear();
            var processed_frame = data[current_line];
            FoundObject fo;

            for (int i = 1; i < processed_frame.Count; i++)
            {
                if (Math.Abs(processed_frame[i] - processed_frame[i - 1]) >= 100)
                {
                    fo = new FoundObject(processed_frame[i], current_points[i]);
                    fo.points.Add(current_points[i]);
                    fo.values.Add(processed_frame[i]);

                    found_objects.Add(fo);
                }

                if (found_objects.Count > 0)
                {
                    if (found_objects.Last().values.Count < this.num_max_points.Value)
                    {
                        var last_obj = found_objects.Last();

                        last_obj.points.Add(current_points[i]);
                        last_obj.values.Add(processed_frame[i]);

                        if (found_objects.Last().values.Count > found_objects.Last().values.Count - 6)
                        last_obj.centroid_frames.Add( new Point(
                                current_points[i].X + last_obj.centroid_frames.Last().X / 2,
                                current_points[i].Y + last_obj.centroid_frames.Last().Y / 2
                            ));
                    }
                    else
                    {
                        fo = new FoundObject(processed_frame[i], current_points[i]);
                        fo.points.Add(current_points[i]);
                        fo.values.Add(processed_frame[i]);
                        found_objects.Add(fo);
                    }
                }
            }

            frame = new Bitmap(frame);
            this.objects_grid.Rows.Clear();

            found_objects.RemoveAll(o => o.values.Count < 5);

            if (found_objects.Count == 0)
                found_objects.ForEach(o => found_objects.Add(o));

            foreach (var obj in found_objects)
            {
                int min_x = obj.points.Min(p => p.X),
                    min_y = obj.points.Min(p => p.Y),
                    max_x = obj.points.Max(p => p.X),
                    max_y = obj.points.Max(p => p.Y);

                Point centroid = new Point((min_x + max_x) / 2, (min_y + max_y) / 2),
                      endpoint = obj.points.Last(p => p.X == min_x),
                      startpoint = obj.points.Last(p => p.X == max_x);

                Find_Closest_Centroid(obj, centroid);

                this.objects_grid.Rows.Add(new object[] { found_objects.IndexOf(obj), obj.centroid_frames.Last() });

                using (Graphics g = Graphics.FromImage(frame))
                {
                    if (min_x == max_x)
                    {
                        startpoint = obj.points.Last(p => p.Y == min_y);
                        endpoint = obj.points.Last(p => p.Y == max_y);
                        g.DrawLine(new Pen(Color.Blue, 4), startpoint, endpoint);
                    }
                    else
                    {
                        double slope = (startpoint.Y - endpoint.Y) / (startpoint.X - endpoint.X);
                        int y_intersect = (int)(-slope * startpoint.X + startpoint.Y);

                        if (obj.points.Count > 50 &&
                            obj.points.Count - obj.points.Count(p => Math.Abs(p.Y - slope * p.X - y_intersect) <= 60) < 3)
                            g.DrawLine(new Pen(Color.Blue, 4), startpoint, endpoint);
                        else
                            g.DrawRectangle(Pens.Fuchsia, min_x, min_y, max_x - min_x, max_y - min_y);

                        g.FillEllipse(
                            Brushes.Red,
                            centroid.X,
                            centroid.Y,
                            5,
                            5
                            );
                    }

                    obj.centroid_frames.ForEach(cf => g.FillEllipse(Brushes.Green, cf.X, cf.Y, 2, 2));
                }
            }

            this.frame_box.Image = frame;
        }


        private void Find_Closest_Centroid(FoundObject o, Point c)
        {
            int goal_distance = (o.centroid_frames.Last().X + c.X) / 2 +
                                (o.centroid_frames.Last().Y + c.Y) / 2;

            for (int i = 0; i < found_objects.Count; i++)
            {
                var obj_j = found_objects[i];

                int obj_distance = (obj_j.centroid_frames.Last().X + c.X) / 2 +
                                   (obj_j.centroid_frames.Last().Y - c.Y) / 2;

                if (Math.Abs(obj_distance - goal_distance) <= 100)
                {
                    obj_j = o;

                    if (obj_j.centroid_frames.Count == 5)
                        obj_j.centroid_frames.RemoveAt(0);

                    obj_j.centroid_frames.Add(c);
                }
                else
                    obj_j.centroid_frames.Add(c);
            }
        }


        private class FoundObject
        {
            public readonly List<Point> points, centroid_frames;
            public readonly List<int> values;

            public FoundObject(int v, Point p)
            {
                this.points = new List<Point> { p };
                this.centroid_frames = new List<Point> { p };
                this.values = new List<int> { v };
            }
        }
    }
}
