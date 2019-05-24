using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        Bitmap image = new Bitmap(800, 450);

        public Form3()
        {
            InitializeComponent();

            chart.Series["Cells"].Points.Add(3);
            chart.Series["Food"].Points.Add(2);
            chart.Series["Virus"].Points.Add(1);

            chart.Series["Cells"].Points[0] = new DataPoint(0, 2);
            chart.Series["Food"].Points[0] = new DataPoint(0, 2);
            chart.Series["Virus"].Points[0] = new DataPoint(0, 2);

            g = Graphics.FromImage(image);
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

                    for(var i = 0; i < Game.Count; i++)
                    {
                        var o = Game[i];
                        if (o is Cell cell)
                        {
                            cell.Draw(image);
                            cell.Move(Game);
                            
                            cellsNr++;
                            if (cell.Infected) cellInf++;
                            if (cell.ProtInf > mcp) mcp = cell.ProtInf;
                        }
                        if (o is Food food)
                        {
                            food.Draw(image);
                            food.Move();
                            
                            foodNr++;
                        }
                        if (o is Virus virus)
                        {
                            virus.Draw(g);
                            virus.Move(Game);
                            
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
            }
            catch (Exception exception)
            {
                
            }
           
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            Game = new List<object>();
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Rand_food_Tick(object sender, EventArgs e)
        {
            Game.Add(new Food(Random(true), 20));
            Game.Add(new Food(Random(true), 20));
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
    }
}

