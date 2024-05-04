using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace Basic_Shadows_Test
{
    public partial class Form1 : Form
    {
        PointF lightSource;
        List<Rectangle> objects = new List<Rectangle>();
        List<PointF[]> objectPoints = new List<PointF[]>();
        List<PointF[]> objectShadowPoints = new List<PointF[]>();
        List<PointF[]> testPoints = new List<PointF[]>();

        const double LIGHT_STRENGTH = 100;
        double currentLightStrength;
        const int lightSize = 10;
        int time = 0;

        Region shadowRegion = new Region();
        GraphicsPath gp = new GraphicsPath(FillMode.Winding);
        public Form1()
        {
            InitializeComponent();
            lightSource = new Point(this.Width / 2, this.Height / 2);

            foreach (System.Windows.Forms.Label l in this.Controls.OfType<System.Windows.Forms.Label>())
            {
                objects.Add(new Rectangle(l.Location, l.Size));
                l.Visible = false;
            }

        }

        double getTheta(PointF one, PointF two)
        {
            double deltaX = (two.X - one.X);
            double deltaY = ((two.Y - one.Y) == 0) ? 0.001 : (two.Y - one.Y);
            double theta = Math.Atan(deltaX / deltaY);
            return Math.Abs(theta);
        }


        int positiveOrNegative(float one, float two)
        {
            int delta = (two >= one) ? -1 : 1;
            return delta;
        }

        PointF getShadowPoint(PointF p, double ls)
        {
            double specificPointLS = ls;
            double theta = getTheta(p, lightSource);

            double deltaX = specificPointLS * Math.Sin(theta);
            deltaX *= positiveOrNegative(p.X, lightSource.X);

            double deltaY = specificPointLS * Math.Cos(theta);
            deltaY *= positiveOrNegative(p.Y, lightSource.Y);

            PointF shadowedPoint = new PointF(p.X + (float)deltaX, p.Y + (float)deltaY);

            return shadowedPoint;
        }
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // time++;
            currentLightStrength = LIGHT_STRENGTH + Math.Sin((double)time / (double)100) * (double)30;
            objectPoints.Clear();
            objectShadowPoints.Clear();
            testPoints.Clear();
            gp.Reset();
            gp.FillMode = FillMode.Winding;

            foreach (Rectangle rect in objects)
            {
                PointF[] rp = new PointF[]
                {
                new PointF(rect.X, rect.Y), //Left Up
                new PointF(rect.X, rect.Y + rect.Height), //Left Down
                new PointF(rect.X + rect.Width, rect.Y + rect.Height), //Right Down
                new PointF(rect.X + rect.Width, rect.Y), //Right Up
                };

                PointF[] sp = new PointF[]
                {
                    getShadowPoint(rp[0], currentLightStrength),
                    getShadowPoint(rp[1], currentLightStrength),
                    getShadowPoint(rp[2], currentLightStrength),
                    getShadowPoint(rp[3], currentLightStrength),
                };

                List<PointF> eightPoints = new List<PointF>();
                eightPoints.AddRange(rp);
                eightPoints.AddRange(sp);
                List<PointF> sixPoints = new List<PointF>();

                PointF[] allPoints = new PointF[]
                {
                rp[0],
                rp[1],
                rp[2],
                rp[3],
                sp[0],
                sp[1],
                sp[2],
                sp[3],
                };


                PointF[][] ghostShadows = new PointF[][]
                {
                    new PointF[]
                    {
                    rp[0],
                    sp[0],
                    sp[1],
                    rp[1],
                    },
                    new PointF[]
                    {
                    rp[1],
                    sp[1],
                    sp[2],
                    rp[2],
                    },
                    new PointF[]
                    {
                    rp[2],
                    sp[2],
                    sp[3],
                    rp[3],
                    },
                    new PointF[]
                    {
                    rp[3],
                    sp[3],
                    sp[0],
                    rp[0],
                    },
                    //new PointF[]
                    //{
                    //rp[0],
                    //rp[1],
                    //rp[2],
                    //rp[3],
                    //},
                    //new PointF[]
                    //{
                    //sp[0],
                    //sp[1],
                    //sp[2],
                    //sp[3],
                    //},
                };

                #region test stuff
                //    int arrayChange = 0;
                //    arrayChange += (positiveOrNegative(rect.X, lightSource.X) == -1) ? 2 : 0; //ADD 2 FOR RIGHT
                //    arrayChange += (positiveOrNegative(rect.Y, lightSource.Y) == -1) ? 1 : 0; //ADD 1 FOR DOWN

                //    PointF[][] allOptions = new PointF[][] {
                //    //LS is UP LEFT
                //    new PointF[]
                //    {
                //        ghostPointArray[0],
                //        ghostPointArray[1],
                //        ghostShadowPointArray[1],
                //        ghostShadowPointArray[2],
                //        ghostShadowPointArray[3],
                //        ghostPointArray[3],
                //    },

                //    //LS is DOWN LEFT
                //    new PointF[]
                //    {
                //        ghostPointArray[1],
                //        ghostPointArray[2],
                //        ghostShadowPointArray[2],
                //        ghostShadowPointArray[3],
                //        ghostShadowPointArray[0],
                //        ghostPointArray[0],
                //    },

                //    //LS is UP RIGHT
                //    new PointF[]
                //    {
                //        ghostPointArray[3],
                //        ghostPointArray[0],
                //        ghostShadowPointArray[0],
                //        ghostShadowPointArray[1],
                //        ghostShadowPointArray[2],
                //        ghostPointArray[2],
                //    },

                //    //LS is DOWN RIGHT
                //    new PointF[]
                //    {
                //        ghostPointArray[2],
                //        ghostPointArray[3],
                //        ghostShadowPointArray[3],
                //        ghostShadowPointArray[0],
                //        ghostShadowPointArray[1],
                //        ghostPointArray[1],
                //    },

                //};
                #endregion
                testPoints.Add(rp);
                testPoints.Add(sp);
                objectPoints.Add(rp);
                objectShadowPoints.AddRange(ghostShadows);
                lightSource = new PointF(Cursor.Position.X, Cursor.Position.Y);
            }

            Refresh();
        }

        List<PointF[]> polygonClump(List<PointF[]> beforeList)
        {
            //List for holding all the lines
            List<PointF[]> lineListTwo = new List<PointF[]>();
            List<PointF[]> lineList = new List<PointF[]>();
            foreach (PointF[] p in beforeList)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    PointF[] ghostLine = new PointF[]
                    {
                       p[i],
                       p[(i == p.Length -1) ? 0 : i + 1]
                    };

                    lineList.Add(ghostLine);
                }
            }

            //Now we have a list of all lines in the polygons, Check if a line is shared more than once, delete it if it is
            for (int i = 0; i < lineList.Count; i++)
            {
                PointF[] pinnedItem = lineList[i];
                lineListTwo.Add(pinnedItem);
            }


            //Create the new polygons


            List<PointF[]> returnMe = lineListTwo;

            //Return new polygons
            return returnMe;
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(new SolidBrush(Color.Red), new Rectangle((int)lightSource.X - (lightSize / 2), (int)lightSource.Y - (lightSize / 2), lightSize, lightSize));

            GraphicsPath gpTwo = new GraphicsPath(FillMode.Winding);
            shadowRegion = new Region(gpTwo);

            List<PointF[]> drawLines = polygonClump(objectShadowPoints);

            foreach (PointF[] p in objectShadowPoints)
            {
                gp.Reset();
                gp.AddPolygon(p);
                shadowRegion.Union(gp);
            }

            Region lightRegion = new Region();
            lightRegion.MakeInfinite();
            lightRegion.Exclude(shadowRegion);

            GraphicsPath sunPath = new GraphicsPath();
            RectangleF[] rects = lightRegion.GetRegionScans(new Matrix());
            foreach (RectangleF r in rects)
            {
                sunPath.AddRectangle(new Rectangle((int)r.Location.X, (int)r.Location.Y, (int)r.Width, (int)r.Height));
            }
            e.Graphics.FillRegion(new SolidBrush(Color.FromArgb(30, 0, 0, 0)), shadowRegion);
            e.Graphics.FillRegion(new SolidBrush(Color.FromArgb(30, 250, 250, 250)), lightRegion);

            gpTwo.Dispose();
            GraphicsPath gpThree = new GraphicsPath();
            gpTwo.Widen(new Pen(new SolidBrush(Color.Black), 3));
            //shadowRegion = new Region(gpTwo);
            e.Graphics.FillRegion(new SolidBrush(Color.FromArgb(30, 0, 0, 0)), shadowRegion);


            foreach (PointF[] p in objectPoints)
            {
                e.Graphics.FillPolygon(new SolidBrush(Color.FromArgb(255, 200, 200, 200)), p);
            }

            PointF[] rp = new PointF[]
          {
                new PointF(objects[0].X, objects[0].Y), //Left Up
                new PointF(objects[0].X, objects[0].Y + objects[0].Height), //Left Down
                new PointF(objects[0].X + objects[0].Width, objects[0].Y + objects[0].Height), //Right Down
                new PointF(objects[0].X + objects[0].Width, objects[0].Y), //Right Up
          };

            PointF[] sp = new PointF[]
            {
                    getShadowPoint(rp[0], currentLightStrength),
                    getShadowPoint(rp[1], currentLightStrength),
                    getShadowPoint(rp[2], currentLightStrength),
                    getShadowPoint(rp[3], currentLightStrength),
            };

            PointF[] cp = new PointF[]
           {
                    getShadowPoint(rp[0], 1),
                    getShadowPoint(rp[1], 1),
                    getShadowPoint(rp[2], 1),
                    getShadowPoint(rp[3], 1),
           };
            for (int i = 0; i < 4; i++)
            {
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Lime), 2), rp[i], sp[i]);
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.FromArgb(255, 255, 0, 0)), 2), sp[i], (i == 3) ? sp[0] : sp[i + 1]);
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0, 255)), 2), rp[i], (i == 3) ? rp[0] : rp[i + 1]);
            }
            int check = 5;
            for (int i = 0; i < 4; i++)
            {
                Rectangle box = new Rectangle(label3.Location, label3.Size);
                if (box.Contains((int)cp[i].X, (int)cp[i].Y))
                {
                    check = i;
                }
            }
            if (check != 5)
            {
                int i = check;
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black), 3), rp[i], sp[i]);
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), 3), sp[i], (i == 3) ? sp[0] : sp[i + 1]);
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), 3), sp[i], (i == 0) ? sp[3] : sp[i - 1]);
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black), 3), (i > 1) ? rp[i - 2] : rp[i + 2], (i > 1) ? sp[i - 2] : sp[i + 2]);

                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black), 3), (i > 1) ? rp[i - 2] : rp[i + 2], (i > 1) ? rp[i - 1] : rp[i + 1]);
                // e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black), 3), (i > 1) ? rp[i - 2] : rp[i + 2], (i > 0) ? rp[(i + 3) - 4] : rp[(i + 3)]);
                // e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black), 3), (i + 2 > 3) ? rp[(i + 2) - 4] : rp[(i + 2)], (i + 3 > 3) ? rp[(i + 3) - 4] : rp[(i + 3)]);
            }
            else 
            {
            
            }

        }
    }
}
