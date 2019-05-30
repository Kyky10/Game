using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace Cells{
    
    [Serializable]
    public class Cell
    {
        
        private static Random r = new Random();

        public Cell(int size, Color color, Point? location = null, bool? infected = null, int? Protection = null, int? calories = null)
        {
            if (location == null)
            {
                location = new Point(r.Next(800), r.Next(450));
            }
            
            this.Size = size;
            this.Color = color;
            this.Location = location ?? new Point(r.Next(800), r.Next(450));
            this.Infected = infected ?? false;
            this.ProtInf = Protection ?? 0;
            this.Calories = calories ?? 20;
            for (int i = 0; i < (Protection ?? 0); i++)
            {
                Cicles.Add(r.Next(-9, 9));
            }

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var p1 = new Point(r.Next(Size / 2 * -1 - 1, Size / 2), r.Next(Size / 2 * -1 - 1, Size / 2));
                    Lines.Add(p1, p1 + (Size)new Point(r.Next(Size / 2 * -1 - 1, Size / 2)));
                }
                catch (Exception e)
                {
                    //
                }
            }
        }

        private Dictionary<Point, Point> Lines = new Dictionary<Point, Point>();

        private static readonly int namesL = new Names().Get().Length;
        public readonly string Name = new Names().Get()[r.Next(namesL)];
        public int Size { get; private set; }
        private List<Point> ToMove = new List<Point>();
        public Color Color { get; private set; }
        private bool GotoDone = true;
        private int foodTick = 60;
        public bool Dead = false;
        public bool Boomed = false;
        public int Calories { get; private set; }
        public int ProtInf = 0;
        public int InfAtta = 0;
        public Point Location { get; set; }
        public bool Infected { get; set; }
        private short infCount = 5;
        private List<int> Cicles = new List<int> {
            r.Next(-9, 9), r.Next(-9, 9), r.Next(-9, 9), r.Next(-9, 9),
            r.Next(-9, 9), r.Next(-9, 9), r.Next(-9, 9), r.Next(-9, 9),
            r.Next(-9, 9), r.Next(-9, 9), r.Next(-9, 9), r.Next(-9, 9)
        };

        public void Boom()
        {
            Boomed = true;
            Dead = true;

            //Location = Point.Empty;
        }

        public static explicit operator Point(Cell m)
        {
            return m.Location;
        }
        
        public void Draw(Bitmap image,
            System.Drawing.Drawing2D.SmoothingMode mode =
                System.Drawing.Drawing2D.SmoothingMode.HighSpeed)
        {
            var g = Graphics.FromImage(image);
            g.SmoothingMode = mode;

            var loc = new Point(Location.X - 5, Location.Y + 2);
            var t = false;
            var inf = infCount;

            if (!Dead)
            {
                Color color;
                if (Infected)
                {
                    g.FillCircle(Color.HotPink.SetTrans(60), new Point(Location.X + Rand(), Location.Y + Rand()),
                        Size / 3);
                    color = Color.HotPink.SetTrans(40);
                }
                else
                {
                    g.FillCircle(Color.Gold.SetTrans(220), new Point(Location.X + Rand(), Location.Y + Rand()),
                        Size / 3);
                    color = Color.LimeGreen.SetTrans(40);
                }


                if (infCount != 0)
                {
                    g.DrawCircle(Color, Location, Size + Calories / 5 + infCount / 10);
                    g.FillCircle(color, Location, Size + Calories / 5 + infCount / 10);
                }
                else
                {
                    g.DrawCircle(Color, Location, Size + Calories / 5);
                    g.FillCircle(color, Location, Size + Calories / 5);
                }

                foreach (var cicle in Cicles)
                {
                    if (t)
                    {
                        loc.Y = Location.Y + cicle;
                        var pen = new Pen(Color.ForestGreen);
                        var XRand = Rand();
                        var YRand = Rand();
                        if (inf < 0)
                        {
                            pen = new Pen(Color.DeepPink);
                        }


                        if (Infected)
                        {
                            g.DrawLine(new Pen(Color.DeepPink), Location,
                                new Point(loc.X + XRand + r.Next(-3, 1), loc.Y + YRand + r.Next(-1, 3)));
                        }

                        g.DrawEllipse(pen, loc.X + XRand, loc.Y + YRand, 6, 8);
                        inf--;
                        t = false;
                    }
                    else
                    {
                        loc.X = Location.X + cicle;
                        t = true;
                    }

                }

                foreach (var line in Lines)
                {
                    g.DrawLine(new Pen(Color.MediumPurple), Location + (Size) line.Key + new Size(Rand(), Rand()),
                        Location + (Size) line.Value + new Size(Rand(), Rand()));
                }
            }

            if(Dead & !Boomed)
            {
                g.DrawCircle(Color.Gray, Location, Size + Calories / 5 + infCount / 10);
                g.FillCircle(Color.DimGray.SetTrans(20), new Point(Location.X + Rand(), Location.Y + Rand()),
                    Size / 3);
                foreach (var cicle in Cicles)
                {
                    if (t)
                    {
                        loc.Y = Location.Y + cicle;
                        var pen = new Pen(Color.ForestGreen);
                        if (inf < 0)
                        {
                            pen = new Pen(Color.DeepPink);
                        }


                        if (Infected)
                        {
                            g.DrawLine(new Pen(Color.DeepPink), Location,
                                new Point(loc.X, loc.Y));
                        }

                        g.DrawEllipse(pen, loc.X, loc.Y, 6, 8);
                        inf--;
                        t = false;
                    }
                    else
                    {
                        loc.X = Location.X + cicle;
                        t = true;
                    }
                }

                foreach (var line in Lines)
                {
                    g.DrawLine(new Pen(Color.MediumPurple), Location + (Size)line.Key,
                        Location + (Size)line.Value);
                }
            }

            if (Boomed & Dead)
            {
                for (var i = 0; i < Cicles.Count; i++)
                {
                    var cicle = Cicles[i];

                    if (t)
                    {
                        loc.Y = Location.Y + cicle;
                        var pen = new Pen(Color.ForestGreen);
                        var XRand = Rand();
                        var YRand = Rand();

                        g.DrawEllipse(pen, loc.X + XRand, loc.Y + YRand, 6, 8);
                        inf--;
                        t = false;
                    }
                    else
                    {
                        loc.X = Location.X + cicle;
                        t = true;
                    }
                }
               
            }

            if (!Dead)
            {
                g.DrawString(Name,
                    new Font(FontFamily.GenericSansSerif, Size / 2, FontStyle.Bold),
                    new SolidBrush(Color.DimGray),
                    Location.X - Size,
                    Location.Y - Size / 4);
            }
        }

        public void Move(List<Object> gameArray, Graphics graphics, int? limitX = null, int? limitY = null)
        {
            if (!Dead)
            {
                if (foodTick < 1)
                {
                    foodTick = 60;
                    Calories--;
                }

                if (Infected)
                {
                    infCount--;
                    if (infCount < -80)
                    {
                        if (r.Next(200) > 50)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                foreach (var o in gameArray)
                                {
                                    if (o is Cell cell)
                                    {
                                        if (((cell.Location.X >= Location.X &
                                              cell.Location.X <= Location.X + 40) |

                                             (cell.Location.X <= Location.X &
                                              cell.Location.X >= Location.X - 40)) &

                                            /////////////////////////////////////

                                            ((cell.Location.Y >= Location.Y &
                                              cell.Location.Y <= Location.Y + 40) |

                                             (cell.Location.Y <= Location.Y &
                                              cell.Location.Y >= Location.Y - 40)))
                                        {
                                            if (cell.ProtInf > InfAtta)
                                            {
                                                if (r.Next(200) > 100)
                                                {
                                                    InfAtta++;
                                                }
                                            }
                                        }
                                    }
                                }
                                var loc = Location;
                                loc.X += Rand(false);
                                loc.Y += Rand(false);
                                gameArray.Add(new Virus(loc, InfAtta));
                            }

                            Boom();
                            return;
                        }
                        else
                        {
                            Infected = false;
                            Size = 20;
                            ProtInf++;
                            Cicles.Add(r.Next(-9, 9));
                        }
                    }
                }

                if (!Infected & infCount < 5)
                {
                    infCount++;
                }

                if (Calories < 1)
                {
                    Dead = true;
                    return;
                }
                

                if (gameArray != null)
                {
                    var foodList = new List<Location>();
                    foreach (var o in gameArray)
                    {
                        if (o is Food food)
                        {
                            foodList.Add(new Location(food, food.Location.X, food.Location.Y, 0));
                        }

                        if (o is Cell cell)
                        {
                            if (cell.Lines != Lines & cell.Location != Location & !cell.Boomed)
                            {
                                if (cell.Dead)
                                    foodList.Add(new Location(cell, cell.Location.X, cell.Location.Y, 1));

                                if (cell.Dead & cell.Infected)
                                    foodList.Add(new Location(cell, cell.Location.X, cell.Location.Y, 2));
                            }
                        }
                    }

                    var IfoodList = foodList.OrderBy(p => p.X).ThenBy(p => p.Y);

                    try
                    {
                        if (GotoDone)
                        {
                            Point GotoPoint = Location + new Size(r.Next(-100, 100), r.Next(-100, 100));

                            

                            if (IfoodList.Count() != 0)
                            {
                                var foodPoint = IfoodList.First();

                                //graphics.DrawLine(new Pen(Color.Black), Location, ToMove.Last());

                                var p1 = new System.Windows.Vector(Location.X, Location.Y);
                                var p2 = new System.Windows.Vector(foodPoint.X, foodPoint.Y);
                                if (System.Windows.Vector.Subtract(p1, p2).Length <= 500)
                                {
                                    GotoPoint = new Point((int) p2.X, (int) p2.Y);
                                }
                            }




                            List<Point> tomoveList;

                            //*** Calculates the distance and approximates hops number.
                            double count_per =
                                (Math.Pow(Location.X - GotoPoint.X, 2) + Math.Pow(Location.Y - Location.Y, 2));
                            var nrwithzero = new string('0', count_per.ToString().Length - 2);
                            var nr = int.Parse("1" + nrwithzero);
                            var count_per1 = count_per / nr;
                            var count = count_per1;
                            if (count > 40)
                            {
                                count /= 2;
                            }

                            // Convert hops number in hops, in points.
                            tomoveList = SplitLine(new Point(Location.X, Location.Y),
                                new Point(GotoPoint.X, GotoPoint.Y), (int) count);

                            tomoveList.Remove(tomoveList.First());
                            ToMove = tomoveList;
                            
                            GotoDone = false;
                            return;
                        }
                    }
                    catch (Exception e)
                    {

                    }


                }

                if (Calories >= 40)
                {
                    while (!(Calories <= 20))
                    {
                        if (Infected & r.Next(200) < 20)
                        {
                            gameArray.Add(new Cell(20, Color.Brown, new Point(Location.X + Rand(true), Location.Y + Rand(true)), false, ProtInf + 1));
                        }
                        else
                        {
                            gameArray.Add(new Cell(20, Color.Brown, new Point(Location.X + Rand(true), Location.Y + Rand(true)), Infected, ProtInf));
                        }
                        Calories -= 20;
                    }


                }

                if (!GotoDone)
                {
                    try
                    {
                        Location = new Point(ToMove.First().X + Rand(), ToMove.First().Y + Rand());
                        if (ToMove.Count == 1)
                        {
                            GotoDone = true;
                        }
                        ToMove.Remove(ToMove.First());

                        foreach (var o in gameArray)
                        {
                            if (o is Food food)
                            {
                                if (((food.Location.X >= Location.X &
                                      food.Location.X <= Location.X + Size) |

                                     (food.Location.X <= Location.X &
                                      food.Location.X >= Location.X - Size)) &

                                    /////////////////////////////////////

                                    ((food.Location.Y >= Location.Y &
                                      food.Location.Y <= Location.Y + Size) |

                                     (food.Location.Y <= Location.Y &
                                      food.Location.Y >= Location.Y - Size)))
                                {
                                    Calories += food.Calories;
                                    gameArray.Remove(food);
                                }
                            }

                            if (o is Cell cell)
                                if (cell.Dead)
                                {
                                    if (((cell.Location.X >= Location.X &
                                          cell.Location.X <= Location.X + Size + 2) |

                                         (cell.Location.X <= Location.X &
                                          cell.Location.X >= Location.X - Size + 2)) &

                                        /////////////////////////////////////

                                        ((cell.Location.Y >= Location.Y &
                                          cell.Location.Y <= Location.Y + Size + 2) |

                                         (cell.Location.Y <= Location.Y &
                                          cell.Location.Y >= Location.Y - Size + 2)))
                                    {
                                        if(Calories >= 20)
                                        Calories += 5;
                                        Infected = cell.Infected;
                                        gameArray.Remove(cell);
                                    }
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        
                    }
                }

                

                foodTick--;

                var t = false;
                var loc1 = new Point(Location.X - 5, Location.Y + 2);

                if (Boomed)
                {
                    var location = Location;
                    location.X += r.Next(-29, 30);
                    location.Y += r.Next(-29, 30);
                    Location = location;


                    for (var i = 0; i < Cicles.Count; i++)
                    {
                        var cicle = Cicles[i];
                        Cicles[i] = r.Next(cicle - 9, cicle + 10);

                        if (t)
                        {
                            loc1.Y = Location.Y + cicle;

                            if (limitX.HasValue & limitY.HasValue)
                            {
                                loc1.X += r.Next(-49, 50);
                                loc1.Y += r.Next(-49, 50);
                                if (loc1.X < 1)
                                {
                                    loc1.X = 1;
                                }

                                if (loc1.Y < 1)
                                {
                                    loc1.Y = 1;
                                }

                                if (loc1.X > limitX)
                                {
                                    loc1.X = (int) limitY - 1;
                                }

                                if (loc1.Y > limitY)
                                {
                                    loc1.Y = (int) limitY - 1;
                                }
                            }
                            else
                            {
                                loc1.X += r.Next(-49, 50);
                                loc1.Y += r.Next(-49, 50);
                                if (loc1.X < 1)
                                {
                                    loc1.X = 1;
                                }

                                if (loc1.Y < 1)
                                {
                                    loc1.Y = 1;
                                }

                                if (loc1.X > limitX)
                                {
                                    loc1.X = (int) limitY - 1;
                                }

                                if (loc1.Y > limitY)
                                {
                                    loc1.Y = (int) limitY - 1;
                                }
                            }

                            Cicles[i - 1] = loc1.X;
                            Cicles[i] = loc1.Y;

                            t = false;
                        }
                        else
                        {
                            loc1.X = Location.X + cicle;
                            t = true;
                        }


                    }
                }

                if (limitX.HasValue & limitY.HasValue)
                {
                    var location = Location;
                    location.X += Rand();
                    location.Y += Rand();
                    if (location.X < 1)
                    {
                        location.X = 1;
                    }

                    if (location.Y < 1)
                    {
                        location.Y = 1;
                    }

                    if (location.X > limitX)
                    {
                        location.X = (int) limitY - 1;
                    }

                    if (location.Y > limitY)
                    {
                        location.Y = (int) limitY - 1;
                    }

                    Location = location;
                }
                else
                {
                    var location = Location;
                    location.X += Rand();
                    location.Y += Rand();
                    if (location.X < 1)
                    {
                        location.X = 1;
                    }

                    if (location.Y < 1)
                    {
                        location.Y = 1;
                    }

                    if (location.X > limitX)
                    {
                        location.X = (int)limitY - 1;
                    }

                    if (location.Y > limitY)
                    {
                        location.Y = (int)limitY - 1;
                    }

                    Location = location;
                }
            }
        }


        private int Rand(bool? super = null)
        {
            if (super == true)
                return r.Next(-40, 40);

            if (super == false)
                return r.Next(-10, 10);

            return r.Next(-1, 2);
        }


        private static List<Point> SplitLine(
            Point p,
            Point p2,
            int count = 20)
        {
            Tuple<Double, Double> a = new Tuple<double, double>(p.X, p.Y);
            Tuple<Double, Double> b = new Tuple<double, double>(p2.X, p2.Y);

            count = count + 1;
            var sa = 0;
            Double d =
                Math.Sqrt((a.Item1 - b.Item1) * (a.Item1 - b.Item1) + (a.Item2 - b.Item2) * (a.Item2 - b.Item2)) /
                count;
            Double fi = Math.Atan2(b.Item2 - a.Item2, b.Item1 - a.Item1);

            List<Point> points = new List<Point>(count + 1);

            for (int i = 0; i <= count; ++i)
            {
                var item1 = (a.Item1 + (i * d * Math.Cos(fi)));
                var item2 = (a.Item2 + (i * d * Math.Sin(fi)));
                points.Add(new Point((int)item1, (int)item2));
            }
            
            //new Tuple<Double, Double>(, )
            return points;
        }
    }

    [Serializable]
    public class Food
    {
        private readonly Random r = new Random();
        public int Calories { get; private set; }
        public Point Location { get; set; }
        public bool Disposed { get; private set; }
        public Food(Point location, int calories)
        {
            Location = location;
            Calories = calories;
        }
        

        public static explicit operator Point(Food m)
        {
            return m.Location;
        }

        public void Draw(Bitmap image)
        {
            if (Disposed) throw new Exception("This Object is Disposed.");
            var g = Graphics.FromImage(image);

            for (int i = 0; i < Calories + 6; i++)
            {
                g.DrawLine(new Pen(Color.Yellow), Location.X + Rand(), Location.Y + Rand(), Location.X + Rand(),
                    Location.Y + Rand());
            }
        }

        public void Move(int? limitX, int? limitY)
        {
            if (Disposed) throw new Exception("This Object is Disposed.");
            if (limitX.HasValue & limitY.HasValue)
            {
                var location = Location;
                location.X += Rand();
                location.Y += Rand();
                if (location.X < 1)
                {
                    location.X = 1;
                }
                if (location.Y < 1)
                {
                    location.Y = 1;
                }
                if (location.X > limitX)
                {
                    location.X = (int)limitY - 1;
                }
                if (location.Y > limitY)
                {
                    location.Y = (int)limitY - 1;
                }
                Location = location;
            }
            else
            {
                var location = Location;
                location.X += Rand();
                location.Y += Rand();
                if (location.X < 1)
                {
                    location.X = 1;
                }
                if (location.Y < 1)
                {
                    location.Y = 1;
                }
                if (location.X > 800)
                {
                    location.X = 800;
                }
                if (location.Y > 450)
                {
                    location.Y = 450;
                }
                Location = location;
            }
        }

        private int Rand()
        {
            return r.Next(-4, 5);
        }
    }
    [Serializable]
    public class Virus
    {
        public Point Location { get; set; }
        public int Attack;
        private Random r = new Random();
        public Virus(Point location, int infAttac = 0)
        {
            Location = location;
            Attack = infAttac;
        }

        public static explicit operator Point(Virus m)
        {
            return m.Location;
        }

        private void drawGr(Graphics g)
        {
            g.DrawCircle(Color.DeepPink, Location, 2);
            for (int i = 0; i < 10; i++)
            {
                g.DrawLine(new Pen(Color.DeepPink), Location, new Point(Location.X + r.Next(-4, 4), Location.Y + r.Next(-4, 4)));
            }

            g.Flush();
        }

        public void Draw(Bitmap Image)
        {
            drawGr(Graphics.FromImage(Image));
        }

        public void Draw(Graphics graphics)
        {
            drawGr(graphics);
        }

        public void Move(List<object> gameList, int? limitX, int? limitY)
        {
            foreach (var o in gameList)
            {
                if (o is Cell cell)
                {
                    if (((cell.Location.X >= Location.X &
                        cell.Location.X <= Location.X + 10) |

                        (cell.Location.X <= Location.X &
                        cell.Location.X >= Location.X - 10)) &

                        /////////////////////////////////////

                        ( (cell.Location.Y >= Location.Y &
                        cell.Location.Y <= Location.Y + 10) |

                        (cell.Location.Y <= Location.Y &
                        cell.Location.Y >= Location.Y - 10)) )
                    {
                        if (!cell.Infected & cell.ProtInf <= Attack)
                        {
                            cell.Infected = true;
                            foreach (var obj in gameList)
                            {
                                if (obj is Virus)
                                {
                                    var virus = (Virus) obj;
                                    if (virus.Location == Location)
                                    {
                                        gameList.Remove(virus);
                                        break;
                                    }
                                }
                            }

                            return;
                        }
                    }
                }
            }


            if (r.Next(30000) <= 2)
            {
                Attack++;
            }

            if (limitX.HasValue & limitY.HasValue)
            {
                var location = Location;
                location.X += Rand();
                location.Y += Rand();
                if (location.X < 1)
                {
                    location.X = 1;
                }
                if (location.Y < 1)
                {
                    location.Y = 1;
                }
                if (location.X > limitX)
                {
                    location.X = (int)limitY - 1;
                }
                if (location.Y > limitY)
                {
                    location.Y = (int)limitY - 1;
                }
                Location = location;
            }
            else
            {
                var location = Location;
                location.X += Rand();
                location.Y += Rand();
                if (location.X < 1)
                {
                    location.X = 1;
                }
                if (location.Y < 1)
                {
                    location.Y = 1;
                }
                if (location.X > 800)
                {
                    location.X = 800;
                }
                if (location.Y > 450)
                {
                    location.Y = 450;
                }
                Location = location;
            }
        }

        private int Rand(bool? super = null)
        {
            if (super == true)
                return r.Next(-40, 40);

            if(super == false)
                return r.Next(-10, 10);

            return r.Next(-1, 2);
        }
    }
    
    [Serializable]
    public class Location
    {
        public int X;
        public int Y;
        public Type Type;
        public object o;
        public int Ifneeded;

        public Location(object Object, int x, int y, int ifneeded)
        {
            o = Object;
            Type = Object.GetType();
            X = x;
            Y = y;
            Ifneeded = ifneeded;
        }

        public Location(object Object, int x, int y)
        {
            o = Object;
            Type = Object.GetType();
            X = x;
            Y = y;
        }
    }
    
    public static class Extensions
    {
        public static void DrawCircle(this Graphics g, Color col,
            Point location, double Radius)
        {
            float radius = (float) Radius;
            var centerX = location.X;
            var centerY = location.Y;
            g.DrawEllipse(new Pen(col), centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }

        public static Color SetTrans(this Color col, int A)
        {
            return Color.FromArgb(A, col);
        }

        public static void FillCircle(this Graphics g, Color col,
            Point location, int radius)
        {
            var centerX = location.X;
            var centerY = location.Y;
            g.FillEllipse(new SolidBrush(col), centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }
    }
}
