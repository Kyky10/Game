using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Cells;

namespace App
{
    public partial class Form3 : Form
    {
        Graphics g;
        List<Object> Game = new List<object>();
        Random r = new Random();
        private Bitmap image = null;

        public Form3()
        {
            InitializeComponent();

            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            chart.Series["Cells"].Points.Add(3);
            chart.Series["Food"].Points.Add(2);
            chart.Series["Virus"].Points.Add(1);

            chart.Series["Cells"].Points[0] = new DataPoint(0, 2);
            chart.Series["Food"].Points[0] = new DataPoint(0, 2);
            chart.Series["Virus"].Points[0] = new DataPoint(0, 2);

            var imgIcon = new Bitmap(40, 40);
            var gr = Graphics.FromImage(imgIcon);
            gr.Clear(Color.Aquamarine);
            new Cell(19, Color.Brown, new Point(20, 20), false, 0, 1).Draw(imgIcon, SmoothingMode.HighQuality);
            Icon = Icon.FromHandle(imgIcon.GetHicon());

            g = Graphics.FromImage((Bitmap)image);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Game.Add(new Cell(20, Color.Brown, null));
            Game.Add(new Food(Random(), 1));
            Game.Add(new Food(Random(), 1));
            Game.Add(new Food(Random(), 1));
        }

        private void Game_timer_Tick(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    var cellsNr = 0;
                    var cellInf = 0;
                    var foodNr = 0;
                    var virusNr = 0;
                    var mcp = 0;
                    var mvi = 0;
                    g.Clear(Color.LightSkyBlue);
                   

                    for (var i = 0; i < Game.Count; i++)
                    {
                        var o = Game[i];
                        if (o is Cell cell)
                        {
                            cell.Draw((Bitmap)image);
                            cell.Move(Game, pictureBox1.Width, pictureBox1.Height);
                            
                            cellsNr++;
                            if (cell.Infected) cellInf++;
                            if (cell.ProtInf > mcp) mcp = cell.ProtInf;
                        }
                        if (o is Food food)
                        {
                            food.Draw((Bitmap)image);
                            food.Move(pictureBox1.Width, pictureBox1.Height);
                            
                            foodNr++;
                        }
                        if (o is Virus virus)
                        {
                            virus.Draw(g);
                            virus.Move(Game, pictureBox1.Width, pictureBox1.Height);
                            
                            virusNr++;
                            if (virus.Attack > mvi) mvi = virus.Attack;
                        }


                    }

                 

                    label1.Text = $"Cells: {cellsNr} ({cellInf} Infected)";
                    label2.Text = $"Food: {foodNr}";
                    label3.Text = $"Virus: {virusNr}";
                    label4.Text = $"Max Cell Protection: {mcp}";
                    label5.Text = $"Max Virus Infection: {mvi}";

                    chart.Series["Cells"].Points[0] = new DataPoint(0, cellsNr);
                    chart.Series["Food"].Points[0] = new DataPoint(0, foodNr);
                    chart.Series["Virus"].Points[0] = new DataPoint(0, virusNr);

                    var listToSort = new List<int> {cellsNr, foodNr, virusNr};
                    listToSort.Sort();
                    var greater = listToSort.Last();
                    chart.ChartAreas[0].AxisY.Maximum = greater + 10;

                    UpdateImage();

                    Random rnd = new Random();

                    for (int i = 0; i < 2; i++)
                        Game = Game.OrderBy(x => rnd.Next(Game.Count)).ToList();


                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    //Game_timer_Tick(null, null);
                    continue;
                }
            }
        }

        private Point Random(bool super = true)
        {
            if(super)
                return new Point(r.Next(800), r.Next(450));
            return new Point(r.Next(-10, 10), r.Next(-10, 10));
        }

        private void Try(Action ac)
        {
            try
            {
                ac();
            }
            catch
            {

            }
        }

        private void UpdateImage()
        {
            pictureBox1.Image = image;
            pictureBox1.Invalidate();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                var m = (MouseEventArgs)e;
                int i = 1;
                if (Control.ModifierKeys == Keys.Shift) i = 10;
                for (int j = 0; j < i; j++)
                {
                    if (m.Button == MouseButtons.Left)
                    {
                        Game.Add(new Food(m.Location + new Size(Random(false)), 16));
                    }

                    if (m.Button == MouseButtons.Right)
                    {
                        Game.Add(new Cell(20, Color.Brown, m.Location));
                    }

                    if (m.Button == MouseButtons.Middle)
                    {
                        Game.Add(new Virus(m.Location));
                    }
                }

                UpdateIf();
            }
            catch (Exception exception)
            {
                
            }
           
        }

        private void UpdateIf()
        {
            if (!game_timer.Enabled)
            {
                var cellsNr = 0;
                var cellInf = 0;
                var foodNr = 0;
                var virusNr = 0;
                var mcp = 0;
                var mvi = 0;
                g.Clear(Color.LightSkyBlue);

                {
                    /*int i = 0;
                    List<object[]> twoArr = Game.GroupBy(x => i++ % Environment.ProcessorCount).Select(g => g.ToArray()).ToList();
    
                    Thread[] threads = new Thread[Environment.ProcessorCount];
                    i = 0;
                    foreach (var objectse in twoArr)
                    {
                        var Gamee = objectse;
                        var t = new Thread((() =>
                        {
                            for (var j = 0; j < Gamee.Length; j++)
                            {
                                var o = Gamee[j];
                                if (o is Cell cell)
                                {
                                    cell.Draw((Bitmap)image);
    
                                    cellsNr++;
                                    if (cell.Infected) cellInf++;
                                    if (cell.ProtInf > mcp) mcp = cell.ProtInf;
                                }
                                if (o is Food food)
                                {
                                    food.Draw((Bitmap)image);
    
                                    foodNr++;
                                }
                                if (o is Virus virus)
                                {
                                    virus.Draw(g);
    
                                    virusNr++;
                                    if (virus.Attack > mvi) mvi = virus.Attack;
                                }
    
    
                            }
                        }));
                        threads[i] = t;
                        t.Start();
                        i++;
                    }
    
                    foreach (var thread in threads)
                    {
                        thread?.Join();
                    }*/

                }


                for (var j = 0; j < Game.Count; j++)
                {
                    var o = Game[j];
                    if (o is Cell cell)
                    {
                        cell.Draw((Bitmap)image);

                        cellsNr++;
                        if (cell.Infected) cellInf++;
                        if (cell.ProtInf > mcp) mcp = cell.ProtInf;
                    }
                    if (o is Food food)
                    {
                        food.Draw((Bitmap)image);

                        foodNr++;
                    }
                    if (o is Virus virus)
                    {
                        virus.Draw(g);

                        virusNr++;
                        if (virus.Attack > mvi) mvi = virus.Attack;
                    }


                }
                    
                  
                
                label1.Text = $"Cells: {cellsNr} ({cellInf} Infected)";
                label2.Text = $"Food: {foodNr}";
                label3.Text = $"Virus: {virusNr}";
                label4.Text = $"Max Cell Protection: {mcp}";
                label5.Text = $"Max Virus Infection: {mvi}";

                chart.Series["Cells"].Points[0] = new DataPoint(0, cellsNr);
                chart.Series["Food"].Points[0] = new DataPoint(0, foodNr);
                chart.Series["Virus"].Points[0] = new DataPoint(0, virusNr);

                var listToSort = new List<int> { cellsNr, foodNr, virusNr };
                listToSort.Sort();
                var greater = listToSort.Last();
                chart.ChartAreas[0].AxisY.Maximum = greater + 10;

                UpdateImage();
            }
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            Game = new List<object>();
            UpdateIf();
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Rand_food_Tick(object sender, EventArgs e)
        {
            var cellCount = 0;
            for (int i = 0; i < Game.Count; i++)
            {
                if (Game[i] is Cell)
                {
                    cellCount++;
                }
            }

            if (cellCount < 400)
            {
                for (int i = 0; i < cellCount / 4; i++)
                {
                    Game.Add(new Food(new Point(r.Next(pictureBox1.Width), r.Next(pictureBox1.Height)), 20));
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (button2.Text.ToLower() == "stop random food")
            {
                rand_food.Stop();
                button2.Text = "Start random food";
            }
            else
            {
                rand_food.Start();
                button2.Text = "Stop random food";
            }
            
        }

        private void Icon_change_Tick(object sender, EventArgs e)
        {
            try
            {
                var imgIcon = new Bitmap(40, 40, PixelFormat.Format32bppArgb);
                var gr = Graphics.FromImage(imgIcon);
                gr.Clear(Color.Aquamarine);
                new Cell(19, Color.Brown, new Point(20, 20), false, 0, 1).Draw(imgIcon, SmoothingMode.AntiAlias);
                var handle = imgIcon.GetHicon();
                Icon = Icon.FromHandle(handle);
            }
            catch (Exception exception)
            {
                
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (button3.Text.ToLower() == "pause")
            {
                button3.Text = "Start";
                game_timer.Stop();
            }
            else
            {
                button3.Text = "Pause";
                game_timer.Start();
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Game_timer_Tick(null, null);
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 1000; i++)
            {
                Game.Add(new Cell(20, Color.Brown, null));
                Game.Add(new Food(Random(), 1));
                Game.Add(new Food(Random(), 1));
                Game.Add(new Food(Random(), 1));
            }
            UpdateIf();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            FileStream fs = null;
            try
            {
                var sfd = new SaveFileDialog();
                sfd.OverwritePrompt = false;
                sfd.Filter = "Game Save|*.sg|Files |*.";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    fs = new FileStream(sfd.FileName, FileMode.Create);
                    bf.Serialize(fs, Game);
                    fs.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Try(() => fs.Close());
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            FileStream fs = null;
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "Game Save|*.sg|Files |*.";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    fs = new FileStream(ofd.FileName, FileMode.Open);
                    Game = (List<Object>)bf.Deserialize(fs);
                    fs.Close();
                }
                UpdateIf();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Try(() => fs.Close());
            }
        }

        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Button3_Click(null, null);
            }
        }

        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
            var m = (MouseEventArgs)e;
            foreach (var o in Game)
            {
                if (o is Cell cell)
                {
                    if (((m.Location.X >= cell.Location.X &
                          m.Location.X <= cell.Location.X + cell.Size) |

                         (m.Location.X <= cell.Location.X &
                          m.Location.X >= cell.Location.X - cell.Size)) &

                        /////////////////////////////////////

                        ((m.Location.Y >= cell.Location.Y &
                          m.Location.Y <= cell.Location.Y + cell.Size) |

                         (m.Location.Y <= cell.Location.Y &
                          m.Location.Y >= cell.Location.Y - cell.Size)))
                    {
                        Game.Remove(cell);
                        return;
                    }
                }
            }
        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            try
            {
                image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                g = Graphics.FromImage(image);
            }
            catch (Exception exception)
            {
                
            }
        }
    }
}

