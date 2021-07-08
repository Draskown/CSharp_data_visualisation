using System.Collections.Generic;
using SharpGL.SceneGraph.Assets;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.IO;
using SharpGL;
using System;

namespace DVT_LR4
{

    public partial class DVT_LR4_Form : Form
    {
        #region Initialization

        private readonly FileStream stream;

        private readonly List<float> valuesX, valuesY, valuesZ;

        private readonly List<Point3F> pointsReg, pointsAvg;

        private readonly OpenGL GL;

        private Point3F[] operatingPoints;
        private PointF initPoint, angle, delta;

        private float[][] calculatedArrays, colours;

        private float[] avgX, avgY, avgZ, recalculatedY, vertices;

        private readonly float angleDelta, bitmapsOffset,
                               cubeOffset, axisSize, axisArrowSize;
        private float distance, minX, maxX,
                      minZ, maxZ, minY, maxY,
                      xDelta, xOffset, yDelta, yOffset,
                      zDelta, zOffset, pminX, pminY, pminZ,
                      pmaxX, pmaxY, pmaxZ, width, height,
                      depth, centreX, centreY, centreZ, a;

        private bool firstMessage;

        private Int16 startMs;

        private readonly int[] xFreq, yFreq, zFreq;

        private readonly int scatPointSize;
        private int bitmapDelta;

        private Bitmap xyHistBmp, xzHistBmp, yzHistBmp,
                       xyScatBmp, xzScatBmp, yzScatBmp;

        private readonly Texture[] textures;

        private readonly Color histColor;
        private readonly Brush scatBrush;


        public DVT_LR4_Form()
        {
            InitializeComponent();

            stream = new FileStream("dump.dmp", FileMode.Open);

            GL = graphMain.OpenGL;

            valuesX = new List<float>();
            valuesY = new List<float>();
            valuesZ = new List<float>();

            initPoint = new PointF();
            angle = new PointF();
            delta = new PointF(-7.39f, -5.79f);

            pointsReg = new List<Point3F>();
            pointsAvg = new List<Point3F>();

            angleDelta = 0.2f;
            cubeOffset = 0.4f;
            bitmapsOffset = 10.0f;
            axisSize = 3.0f;
            axisArrowSize = 0.4f;
            distance = 55;
            scatPointSize = 4;

            firstMessage = true;

            xFreq = new int[10];
            yFreq = new int[10];
            zFreq = new int[10];

            bitmapDelta = 0;

            textures = new Texture[6];
            for (int i = 0; i < textures.Length; i++)
                textures[i] = new Texture();

            histColor = Color.Fuchsia;
            scatBrush = Brushes.Bisque;

            FirstMoves();
        }


        private void DVT_LR4_Form_Load(object sender, EventArgs e)
        {
            this.Click += (ss, ee) => this.ActiveControl = null;

            this.graphMain.MouseDown += (ss, ee) => initPoint = ee.Location;
            this.graphMain.MouseWheel += ChangeDistance;
            this.graphMain.MouseMove += ChangePosition;

            this.graphMain.OpenGLDraw += Draw;

            this.btnSave.Click += SaveImage;

            this.switchDisplay.CheckedChanged += (ss, ee) =>
            {
                CalculateHistograms();
                FormBitmaps();
                InitializeTextures();
            };

            this.numDepth.ValueChanged += (ss, ee) => { FirstMoves(); };

            this.numLength.ValueChanged += (ss, ee) => { FirstMoves(); };
        }


        private void FirstMoves()
        {
            ReadData();

            CalculateAvgs(valuesX, "X");
            CalculateAvgs(valuesZ, "Z");

            RecalculateY(valuesX.ToArray(), valuesZ.ToArray(), 0);
            RecalculateY(avgX, avgZ, 1);

            pointsAvg.Clear();
            for (int i = 0; i < avgX.Length; i++)
                pointsAvg.Add(new Point3F(avgX[i] / 10.0f, avgY[i] / 10.0f, avgZ[i] / 10.0f));

            CalculateHistograms();

            FormBitmaps();

            Random r = new Random();
            colours = new float[][] {
                new float[] {(float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()},
                new float[] {(float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()},
                new float[] {(float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()},
                new float[] {(float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()}
            };

            InitializeTextures();
        }


        private void InitializeTextures()
        {
            textures[0].Create(GL, xyHistBmp);
            textures[1].Create(GL, xyScatBmp);
            textures[2].Create(GL, xzHistBmp);
            textures[3].Create(GL, yzScatBmp);
            textures[4].Create(GL, yzHistBmp);
            textures[5].Create(GL, xzScatBmp);
        }

        #endregion



        #region Data Reading

        private void ReadData()
        {
            pointsReg.Clear();
            valuesX.Clear();
            valuesY.Clear();
            valuesZ.Clear();

            do
            {
                byte[] message = new byte[19];

                if (stream.Position == 58900)
                {
                    stream.Position = 0;
                    break;
                }

                stream.Read(message, 0, message.Length);

                var str = $"0x{message[6]:X2} -> 0x{message[7]:X2}";

                var ms = BitConverter.ToInt16(new byte[] { message[0], message[1] }, 0);

                if (firstMessage)
                {
                    startMs = ms;
                    firstMessage = false;
                }

                if (ms - startMs <= (int)this.numLength.Value)
                {
                    if (str == labelX.Text)
                        valuesX.Add(message[9]);
                    if (str == labelY.Text)
                        valuesY.Add(message[9]);
                    if (str == labelZ.Text)
                        valuesZ.Add(message[9]);
                }
                else
                {
                    if (str == labelX.Text)
                        valuesX.Add(0);
                    if (str == labelY.Text)
                        valuesY.Add(0);
                    if (str == labelZ.Text)
                        valuesZ.Add(0);
                }

                startMs = ms;
            }
            while (true);

            for (int i = 0; i < valuesX.Count; i++)
            {
                if (pointsReg.Any(p => float.Equals(p.X, valuesX[i] / 10.0f) &&
                                    p.Y == valuesY[i] / 10.0f &&
                                    p.Z == valuesZ[i] / 10.0f))
                {
                    valuesX.RemoveAt(i);
                    valuesY.RemoveAt(i);
                    valuesZ.RemoveAt(i);
                    i = 0;
                    continue;
                }

                pointsReg.Add(new Point3F(valuesX[i] / 10.0f, valuesY[i] / 10.0f, valuesZ[i] / 10.0f));
            }

            avgX = new float[valuesX.Count];
            avgY = new float[valuesY.Count];
            avgZ = new float[valuesZ.Count];
        }

        #endregion



        #region Calculations

        private void CalculateAvgs(List<float> lst, string key)
        {
            if (lst.Count == 0)
                return;

            for (int j = 1; j < lst.Count; j++)
            {
                var depth = (int)numDepth.Value;
                var sum = 0.0f;
                var count = 0;

                for (int i = lst.Count - j - 1; i >= 0; i--)
                {
                    if (lst.Count - j - i > depth)
                        continue;

                    sum += lst[i];
                    count++;
                }

                switch (key)
                {
                    case "X":
                        avgX[avgX.Length - j] = sum / count;
                        break;
                    case "Y":
                        avgY[avgY.Length - j] = sum / count;
                        break;
                    case "Z":
                        avgZ[avgZ.Length - j] = sum / count;
                        break;
                }
            }
        }


        private void RecalculateY(float[] arrayX, float[] arrayZ, int key)
        {
            minX = arrayX.Min();
            maxX = arrayX.Max();
            minZ = arrayZ.Min();
            maxZ = arrayZ.Max();

            xDelta = (maxX - minX) / 10;
            xOffset = 0 - minX / xDelta;
            zDelta = (maxZ - minZ) / 10;
            zOffset = 0 - minZ / zDelta;

            var ys = new int[10, 10];

            if (key == 0)
                recalculatedY = new float[valuesY.Count];

            for (int k = 0; k < arrayX.Length; k++)
            {
                int xIndex = (int)Math.Round(arrayX[k] / xDelta + xOffset);
                int zIndex = (int)Math.Round(arrayZ[k] / zDelta + zOffset);

                for (int i = 0; i < 10; i++)
                    if (xIndex == i)
                        for (int j = 0; j < 10; j++)
                            if (zIndex == j)
                                ys[xIndex, zIndex]++;
            }

            for (int k = 0; k < arrayX.Length; k++)
            {
                int xIndex = (int)Math.Round(arrayX[k] / xDelta + xOffset);
                int zIndex = (int)Math.Round(arrayZ[k] / zDelta + zOffset);

                for (int i = 0; i < 10; i++)
                    if (xIndex == i)
                        for (int j = 0; j < 10; j++)
                            if (zIndex == j)
                                if (key == 0)
                                    recalculatedY[k] = ys[xIndex, zIndex];
                                else
                                    avgY[k] = ys[xIndex, zIndex];
            }

            if (key == 0)
                for (int i = 0; i < pointsReg.Count; i++)
                    pointsReg[i].Y = recalculatedY[i] / 10.0f;
            else
                for (int i = 0; i < pointsAvg.Count; i++)
                    pointsAvg[i].Y = avgY[i] / 10.0f;
        }


        private void CalculateHistograms()
        {
            if (this.switchDisplay.Checked)
                calculatedArrays = new float[][] { avgX, avgY, avgZ };
            else
                calculatedArrays = new float[][] { valuesX.ToArray(), recalculatedY.ToArray(), valuesZ.ToArray() };

            minX = calculatedArrays[0].Min();
            maxX = calculatedArrays[0].Max();
            minY = calculatedArrays[1].Min();
            maxY = calculatedArrays[1].Max();
            minZ = calculatedArrays[2].Min();
            maxZ = calculatedArrays[2].Max();

            xDelta = (maxX - minX) / 10;
            xOffset = 0 - minX / xDelta;
            yDelta = (maxY - minY) / 10;
            yOffset = 0 - minY / xDelta;
            zDelta = (maxZ - minZ) / 10;
            zOffset = 0 - minZ / zDelta;

            for (int k = 0; k < calculatedArrays[0].Length; k++)
            {
                int xIndex = (int)(calculatedArrays[0][k] / xDelta + xOffset);
                int yIndex = (int)(calculatedArrays[1][k] / yDelta + yOffset);
                int zIndex = (int)(calculatedArrays[2][k] / zDelta + zOffset);

                for (int i = 0; i < 10; i++)
                {
                    if (xIndex == i)
                        xFreq[i]++;
                    if (yIndex == i)
                        yFreq[i]++;
                    if (zIndex == i)
                        zFreq[i]++;
                }
            }
        }


        private void FormBitmaps()
        {
            if (this.switchDisplay.Checked)
                operatingPoints = (Point3F[])pointsAvg.ToArray().Clone();
            else
                operatingPoints = (Point3F[])pointsReg.ToArray().Clone();

            pminX = operatingPoints.Min(p => p.X);
            pmaxX = operatingPoints.Max(p => p.X);
            pminY = operatingPoints.Min(p => p.Y);
            pmaxY = operatingPoints.Max(p => p.Y);
            pminZ = operatingPoints.Min(p => p.Z);
            pmaxZ = operatingPoints.Max(p => p.Z);

            width = pmaxX - pminX;
            height = pmaxY - pminY;
            depth = pmaxZ - pminZ;

            a = new[] { width, height, depth }.Max();

            centreX = (pmaxX + pminX) / 2;
            centreY = (pmaxY + pminY) / 2;
            centreZ = (pmaxZ + pminZ) / 2;

            pminX = centreX - a / 2 - cubeOffset;
            pmaxX = centreX + a / 2 + cubeOffset;
            pminY = centreY - a / 2 - cubeOffset;
            pmaxY = centreY + a / 2 + cubeOffset;
            pminZ = centreZ - a / 2 - cubeOffset;
            pmaxZ = centreZ + a / 2 + cubeOffset;

            xyHistBmp = new Bitmap(Map(width, pminX, pmaxX, 0, 500),
                               Map(height, pminY, pmaxY, 0, 500));
            xzHistBmp = new Bitmap(Map(width, pminX, pmaxX, 0, 500),
                               Map(depth, pminZ, pmaxZ, 0, 500));
            yzHistBmp = new Bitmap(Map(height, pminY, pmaxY, 0, 500),
                               Map(depth, pminZ, pmaxZ, 0, 500));

            xyScatBmp = new Bitmap(xyHistBmp);
            xzScatBmp = new Bitmap(xzHistBmp);
            yzScatBmp = new Bitmap(yzHistBmp);

            vertices = new float[operatingPoints.Length * 3];
            for (int i = 0; i < vertices.Length; i += 3)
            {
                vertices[i] = operatingPoints[i / 3].X;
                vertices[i + 1] = operatingPoints[i / 3].Y;
                vertices[i + 2] = operatingPoints[i / 3].Z;
            }

            using (Graphics g = Graphics.FromImage(xyHistBmp))
            {
                bitmapDelta = xyHistBmp.Width / xFreq.Length;

                for (int i = 0; i < xFreq.Length; i++)
                {
                    g.DrawRectangle(
                        new Pen(histColor),
                        i * bitmapDelta,
                        xyHistBmp.Height - Map(xFreq[i], 0, xFreq.Max(), 0, xyHistBmp.Height),
                        bitmapDelta,
                        Map(xFreq[i], 0, xFreq.Max(), 0, xyHistBmp.Height) - 1
                    );
                }
            }

            using (Graphics g = Graphics.FromImage(xzHistBmp))
            {
                bitmapDelta = xzHistBmp.Width / zFreq.Length;

                for (int i = 0; i < zFreq.Length; i++)
                {
                    g.DrawRectangle(
                        new Pen(histColor),
                        i * bitmapDelta,
                        0,
                        bitmapDelta,
                        Map(zFreq[zFreq.Length - 1 - i], 0, zFreq.Max(), 0, xzHistBmp.Height) - 1
                    );
                }
            }

            using (Graphics g = Graphics.FromImage(yzHistBmp))
            {
                bitmapDelta = yzHistBmp.Height / yFreq.Length;

                for (int i = 0; i < yFreq.Length; i++)
                {
                    g.DrawRectangle(
                        new Pen(histColor),
                        0,
                        i * bitmapDelta,
                        Map(yFreq[zFreq.Length - 1 - i], 0, yFreq.Max(), 0, yzHistBmp.Width) - 1,
                        bitmapDelta
                    );
                }
            }

            int crutches1 = 0, crutches2 = 0;

            if (this.switchDisplay.Checked)
            {
                calculatedArrays = new float[][] { avgX, avgY, avgZ };
                crutches1 = xzScatBmp.Height / 2;
                crutches2 = xyScatBmp.Height / 2;
            }
            else
                calculatedArrays = new float[][] { valuesX.ToArray(), valuesY.ToArray(), valuesZ.ToArray() };

            using (Graphics g = Graphics.FromImage(xyScatBmp))
            {
                for (int i = 0; i < calculatedArrays[0].Length; i++)
                {
                    g.FillEllipse(
                        scatBrush,
                        Map(calculatedArrays[0][i], minX, maxX, 0, xyScatBmp.Width - scatPointSize),
                        Map(calculatedArrays[1][i] / 10.0f, minY, maxY, 0, xyScatBmp.Height - scatPointSize) + (float)crutches1 * 0.6f,
                        scatPointSize, scatPointSize
                    );
                }
            }

            using (Graphics g = Graphics.FromImage(xzScatBmp))
            {
                for (int i = 0; i < calculatedArrays[0].Length; i++)
                {
                    g.FillEllipse(
                        scatBrush,
                        Map(calculatedArrays[0][i], minX, maxX, 0, xzScatBmp.Width - scatPointSize),
                        Map(calculatedArrays[2][i], minZ, maxZ, 0, xzScatBmp.Height - scatPointSize),
                        scatPointSize, scatPointSize
                    );
                }
            }

            using (Graphics g = Graphics.FromImage(yzScatBmp))
            {
                for (int i = 0; i < calculatedArrays[1].Length; i++)
                {
                    g.FillEllipse(
                        scatBrush,
                        Map(calculatedArrays[2][i], minZ, maxZ, 0, yzScatBmp.Height - scatPointSize),
                        Map(calculatedArrays[1][i] / 10.0f, minY, maxY, 0, yzScatBmp.Width - scatPointSize) + (float)crutches2 * 1.5f,
                        scatPointSize, scatPointSize
                    );
                }
            }
        }

        #endregion



        #region Drawing

        private void Draw(object sender, RenderEventArgs args)
        {
            if (valuesX.Count == 0 || valuesY.Count == 0 || valuesZ.Count == 0)
                return;

            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            GL.LoadIdentity();

            GL.Translate(delta.X, delta.Y, -distance);
            GL.Rotate(angle.Y, angle.X, 0.0f);

            GL.Enable(OpenGL.GL_DEPTH_TEST);
            GL.Enable(OpenGL.GL_BLEND);
            GL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            if (this.showPoints.Checked)
            {
                GL.PointSize(2.0f);
                GL.Begin(OpenGL.GL_POINTS);
                GL.Color(1.0f, 1.0f, 1.0f, 0.5f);

                foreach (var p in operatingPoints)
                    GL.Vertex(p.X, p.Y, p.Z);

                GL.End();
            }

            GL.Disable(OpenGL.GL_BLEND);

            if (!this.hideCube.Checked)
            {
                GL.Color(1.0f, 1.0f, 1.0f, 1.0f);

                DrawSurface();

                GL.LineWidth(2.0f);
                GL.Begin(OpenGL.GL_LINES);
                {
                    GL.Color(colours[0]);
                    GL.Vertex(pminX, pminY, pminZ);
                    GL.Vertex(pmaxX, pminY, pminZ);
                    GL.Vertex(pmaxX, pminY, pminZ);
                    GL.Vertex(pmaxX, pminY, pmaxZ);
                    GL.Vertex(pmaxX, pminY, pmaxZ);
                    GL.Vertex(pminX, pminY, pmaxZ);
                    GL.Color(colours[1]);
                    GL.Vertex(pminX, pminY, pmaxZ);
                    GL.Vertex(pminX, pminY, pminZ);
                    GL.Vertex(pminX, pminY, pminZ);
                    GL.Vertex(pminX, pmaxY, pminZ);
                    GL.Vertex(pminX, pmaxY, pminZ);
                    GL.Vertex(pmaxX, pmaxY, pminZ);
                    GL.Color(colours[2]);
                    GL.Vertex(pmaxX, pmaxY, pminZ);
                    GL.Vertex(pmaxX, pminY, pminZ);
                    GL.Vertex(pmaxX, pmaxY, pminZ);
                    GL.Vertex(pmaxX, pmaxY, pmaxZ);
                    GL.Vertex(pmaxX, pmaxY, pmaxZ);
                    GL.Vertex(pmaxX, pminY, pmaxZ);
                    GL.Color(colours[3]);
                    GL.Vertex(pmaxX, pmaxY, pmaxZ);
                    GL.Vertex(pminX, pmaxY, pmaxZ);
                    GL.Vertex(pminX, pminY, pmaxZ);
                    GL.Vertex(pminX, pmaxY, pmaxZ);
                    GL.Vertex(pminX, pmaxY, pmaxZ);
                    GL.Vertex(pminX, pmaxY, pminZ);

                    GL.Color(1.0f, 0.0f, 0.0f);
                    GL.Vertex(centreX, centreY, centreZ);
                    GL.Vertex(centreX + axisSize, centreY, centreZ);
                    GL.Vertex(centreX + axisSize, centreY, centreZ);
                    GL.Vertex(centreX + axisSize - axisArrowSize, centreY, centreZ - axisArrowSize / 2.0f);
                    GL.Vertex(centreX + axisSize, centreY, centreZ);
                    GL.Vertex(centreX + axisSize - axisArrowSize, centreY, centreZ + axisArrowSize / 2.0f);

                    GL.Color(0.0f, 0.0f, 1.0f);
                    GL.Vertex(centreX, centreY, centreZ);
                    GL.Vertex(centreX, centreY, centreZ + axisSize);
                    GL.Vertex(centreX, centreY, centreZ + axisSize);
                    GL.Vertex(centreX + axisArrowSize / 2.0f, centreY, centreZ + axisSize - axisArrowSize);
                    GL.Vertex(centreX, centreY, centreZ + axisSize);
                    GL.Vertex(centreX - axisArrowSize / 2.0f, centreY, centreZ + axisSize - axisArrowSize);

                    GL.Color(1.0f, 1.0f, 0.0f);
                    GL.Vertex(centreX, centreY, centreZ);
                    GL.Vertex(centreX, centreY + axisSize, centreZ);
                    GL.Vertex(centreX, centreY + axisSize, centreZ);
                    GL.Vertex(centreX + axisArrowSize / 2.0f, centreY + axisSize - axisArrowSize, centreZ);
                    GL.Vertex(centreX, centreY + axisSize, centreZ);
                    GL.Vertex(centreX - axisArrowSize / 2.0f, centreY + axisSize - axisArrowSize, centreZ);
                }
                GL.End();

                GL.DrawText(0, 0, 0.0f, 0.0f, 0.0f, "Gilroy", 1, ".");

                var sv = SharpGL.SceneGraph.OpenGLSceneGraphExtensions.Project(GL, new SharpGL.SceneGraph.Vertex(centreX + axisSize, centreY, centreZ));
                GL.DrawText((int)sv.X, (int)sv.Y, 1f, 1f, 1f, "Gilroy", 12, labelX.Text);

                sv = SharpGL.SceneGraph.OpenGLSceneGraphExtensions.Project(GL, new SharpGL.SceneGraph.Vertex(centreX, centreY + axisSize, centreZ));
                GL.DrawText((int)sv.X, (int)sv.Y, 1f, 1f, 1f, "Gilroy", 12, labelY.Text);

                sv = SharpGL.SceneGraph.OpenGLSceneGraphExtensions.Project(GL, new SharpGL.SceneGraph.Vertex(centreX, centreY, centreZ + axisSize));
                GL.DrawText((int)sv.X, (int)sv.Y, 1f, 1f, 1f, "Gilroy", 12, labelZ.Text);
            }

            GL.Enable(OpenGL.GL_TEXTURE_2D);

            if (!this.hideXYHist.Checked)
            {
                textures[0].Bind(GL);
                GL.Color(1.0f, 1.0f, 1.0f, 0.7f);
                GL.Begin(SharpGL.Enumerations.BeginMode.Quads);
                {
                    GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX, pmaxY, pmaxZ + bitmapsOffset);
                    GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX, pmaxY, pmaxZ + bitmapsOffset);
                    GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX, pminY, pmaxZ + bitmapsOffset);
                    GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX, pminY, pmaxZ + bitmapsOffset);
                }
                GL.End();
            }

            if (!this.hideXYScat.Checked)
            {
                textures[1].Bind(GL);

                GL.Begin(SharpGL.Enumerations.BeginMode.Quads);
                {
                    GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX, pminY, pminZ - bitmapsOffset);
                    GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX, pminY, pminZ - bitmapsOffset);
                    GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX, pmaxY, pminZ - bitmapsOffset);
                    GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX, pmaxY, pminZ - bitmapsOffset);
                }
                GL.End();
            }

            if (!hideXZHist.Checked)
            {
                textures[2].Bind(GL);

                GL.Begin(SharpGL.Enumerations.BeginMode.Quads);
                {
                    GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX - bitmapsOffset, pminY, pmaxZ);
                    GL.TexCoord(1.0f, 0.0f); GL.Vertex(pminX - bitmapsOffset, pminY, pminZ);
                    GL.TexCoord(1.0f, 1.0f); GL.Vertex(pminX - bitmapsOffset, pmaxY, pminZ);
                    GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX - bitmapsOffset, pmaxY, pmaxZ);
                }
                GL.End();
            }

            if (!this.hideYZScat.Checked)
            {
                textures[3].Bind(GL);
                GL.Begin(SharpGL.Enumerations.BeginMode.Quads);
                {
                    GL.TexCoord(0.0f, 0.0f); GL.Vertex(pmaxX + bitmapsOffset, pminY, pminZ);
                    GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX + bitmapsOffset, pminY, pmaxZ);
                    GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX + bitmapsOffset, pmaxY, pmaxZ);
                    GL.TexCoord(0.0f, 1.0f); GL.Vertex(pmaxX + bitmapsOffset, pmaxY, pminZ);
                }
                GL.End();
            }

            if (!this.hideYZHist.Checked)
            {
                textures[4].Bind(GL);

                GL.Begin(SharpGL.Enumerations.BeginMode.Quads);
                {
                    GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX, pminY - bitmapsOffset, pmaxZ);
                    GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX, pminY - bitmapsOffset, pmaxZ);
                    GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX, pminY - bitmapsOffset, pminZ);
                    GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX, pminY - bitmapsOffset, pminZ);
                }
                GL.End();
            }

            if (!this.hideXZScat.Checked)
            {
                textures[5].Bind(GL);

                GL.Begin(SharpGL.Enumerations.BeginMode.Quads);

                GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX, pmaxY + bitmapsOffset, pminZ);
                GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX, pmaxY + bitmapsOffset, pminZ);
                GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX, pmaxY + bitmapsOffset, pmaxZ);
                GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX, pmaxY + bitmapsOffset, pmaxZ);

                GL.End();
            }

            GL.Disable(OpenGL.GL_TEXTURE_2D);
            GL.Disable(OpenGL.GL_DEPTH_TEST);
            GL.Flush();
        }


        private void DrawSurface()
        {
            var yMaxs = new float[10, 10];
            var yMaxMax = 0.0f;

            for (int k = 0; k < valuesX.Count; k++)
            {
                int xIndex = (int)(valuesX[k] / xDelta + xOffset);
                int zIndex = (int)(valuesZ[k] / zDelta + zOffset);

                for (int i = 0; i < 10; i++)
                {
                    if (xIndex == i)
                        for (int j = 0; j < 10; j++)
                            if (zIndex == j && recalculatedY[k] / 10.0f > yMaxs[i, j])
                            {
                                yMaxs[i, j] = recalculatedY[k] / 10.0f;
                                if (yMaxs[i, j] > yMaxMax)
                                    yMaxMax = yMaxs[i, j];
                            }
                }
            }

            GL.Begin(OpenGL.GL_TRIANGLES);
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        GL.Color(0.0f, Map(yMaxs[i, j], 0.0f, yMaxMax, 0.4f, 1.0f), 0.0f);
                        GL.Vertex(xDelta / 10.0f * i + xDelta / 10.0f / 2,
                                  yMaxs[i, j],
                                  zDelta / 10.0f * j + zDelta / 10.0f / 2);
                        GL.Color(0.0f, Map(yMaxs[i, j], 0.0f, yMaxMax, 0.4f, 1.0f), 0.0f);
                        GL.Vertex(xDelta / 10.0f * i + xDelta / 10.0f / 2,
                                  yMaxs[i, j + 1],
                                  zDelta / 10.0f * (j + 1) + zDelta / 10.0f / 2);
                        GL.Color(0.0f, Map(yMaxs[i, j], 0.0f, yMaxMax, 0.4f, 1.0f), 0.0f);
                        GL.Vertex(xDelta / 10.0f * (i + 1) + xDelta / 10.0f / 2,
                                  yMaxs[i + 1, j],
                                  zDelta / 10.0f * (j) + zDelta / 10.0f / 2);
                        GL.Color(0.0f, Map(yMaxs[i, j], 0.0f, yMaxMax, 0.4f, 1.0f), 0.0f);
                        GL.Vertex(xDelta / 10.0f * i + xDelta / 10.0f / 2,
                                  yMaxs[i, j + 1],
                                  zDelta / 10.0f * (j + 1) + zDelta / 10.0f / 2);
                        GL.Color(0.0f, Map(yMaxs[i, j], 0.0f, yMaxMax, 0.4f, 1.0f), 0.0f);
                        GL.Vertex(xDelta / 10.0f * (i + 1) + xDelta / 10.0f / 2,
                                  yMaxs[i + 1, j],
                                  zDelta / 10.0f * j + zDelta / 10.0f / 2);
                        GL.Color(0.0f, Map(yMaxs[i, j], 0.0f, yMaxMax, 0.4f, 1.0f), 0.0f);
                        GL.Vertex(xDelta / 10.0f * (i + 1) + xDelta / 10.0f / 2,
                                  yMaxs[i + 1, j + 1],
                                  zDelta / 10.0f * (j + 1) + zDelta / 10.0f / 2);
                    }
                }
            }
            GL.End();
        }

        #endregion



        #region Mouse moving

        private void ChangePosition(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                valuesX.Count == 0 || valuesY.Count == 0 || valuesZ.Count == 0)
                return;

            PointF finalPoint = e.Location;

            if (ModifierKeys == Keys.Shift)
            {
                angle.X += (finalPoint.X - initPoint.X) * angleDelta;
                angle.Y += (finalPoint.Y - initPoint.Y) * angleDelta;
            }
            else
            {
                delta.X += (finalPoint.X - initPoint.X) / 100;
                delta.Y -= (finalPoint.Y - initPoint.Y) / 100;
            }

            initPoint = finalPoint;
        }


        private void ChangeDistance(object sender, MouseEventArgs e)
        {
            distance -= e.Delta / 120;
        }

        #endregion



        #region Saving an image

        private void SaveImage(object sender, EventArgs e)
        {
            var bmp = new Bitmap(
                this.graphMain.Width,
                this.graphMain.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.ReadPixels(0, 0, bmp.Width, bmp.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bmpData.Scan0);

            bmp.UnlockBits(bmpData);
            bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            bmp.Save("Image.bmp");
        }

        #endregion



        #region Map methods

        private int Map(float value, float inMin, float inMax, int outMin, int outMax)
        {
            return (int)((value - inMin) * (outMax - outMin) / (inMax - inMin)) + outMin;
        }


        private int Map(int value, int inMin, int inMax, int outMin, int outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        private float Map(float value, float inMin, float inMax, float outMin, float outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        #endregion
    }



    #region Additional class Point3F

    public partial class Point3F
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Point3F(float x, float y, float z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }
    }
    #endregion
}
