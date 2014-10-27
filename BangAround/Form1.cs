using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BangAround
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //DoIt();
            do5();

            chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart1.ChartAreas["ChartArea1"].AxisX.Maximum = 100;
            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Interval = 5;
            chart1.ChartAreas["ChartArea1"].AxisY.Minimum = 0;
            chart1.ChartAreas["ChartArea1"].AxisY.Maximum = 15;
            chart1.ChartAreas["ChartArea1"].AxisY.Interval = 1;
        }

        private void DoIt()
        {
            // f(x) = f(0)e^(-rx)
            // y = e^-(K*X)
            double X = 4.0d; // count of engines
            double Y = 0.85d; // left side
            double logLeft = Math.Log(Y, Math.E);
            //logleft = -(K*X)

            double logLeftNegative = logLeft * -1;
            //logleftnegative = K*X;

            double K = logLeftNegative / X;
            //result = K

            string super = "Numbers:";
            for (int i = 0; i < 100; i++)
            {
                double t = Math.Pow(Math.E, (-1 * (K * i)));
                super = super + Environment.NewLine+i.ToString()+": "+Math.Round(t*i).ToString();
            }

            label2.Text = super;

            //label1.Text = logLeft.ToString()+Environment.NewLine+logLeftNegative.ToString()+Environment.NewLine+K.ToString();
        }

        private void do2()
        {
            int last = 1;
            int counter = 0;
            int next = 0;
            string super = "2:";
            for (int i = 0; i < 100; i++)
            {
                if (counter==last)
                {
                    next++;
                    last = next;
                    counter=0;
                }
                super = super + Environment.NewLine + i.ToString() + ": " + next.ToString();
                counter++;
            }
            label1.Text = super;

        }

        private void do3()
        {
            double count = 0;
            string super = "Numbers:";
            for (int i = 0; i < 100; i++)
            {
                count = Math.Pow(2, i) - 1;
                super = super + Environment.NewLine + i.ToString() + ": " + count.ToString();
            }
            label1.Text = super;
        }

        private void do4()
        {
            double count = 0;
            string super = "4:";
            for (int i = 0; i < 100; i++)
            {
                count = (i*(i+1))/2;
                super = super + Environment.NewLine + i.ToString() + ": " + count.ToString();
            }
            label2.Text = super;
        }

        private void do5()
        {
            Ship currentShip;
            
            DataTable table = new DataTable();
            double[][] data = new double[6][];

            for (int countOfColumns = 0; countOfColumns < 6; countOfColumns++)
            {
                double power = Math.Pow(4,countOfColumns+1);
                double mass = 50 * power;

                table.Columns.Add(new DataColumn(string.Format("Mass: {0}", mass ),typeof(string)));
                data[countOfColumns] = new double[100];
            }
            
            for (int countOfEngines = 0; countOfEngines < 100; countOfEngines++)
            {
                DataRow row = table.NewRow();
                for (int countOfMass = 0; countOfMass < 6; countOfMass++)
                {
                    double scale = Convert.ToDouble(tbxScale.Text);
                    currentShip = new Ship(countOfEngines, 50 * Math.Pow(4,countOfMass+1),scale);
                    row[countOfMass] = "Eng: " + countOfEngines.ToString() + " MP: " + currentShip.GetMP() + " Mass: " + currentShip.TotalMass.ToString()+":"+ Math.Log((currentShip.TotalMass/50),4);;
                    data[countOfMass][countOfEngines] = currentShip.GetMP();
                }
                table.Rows.Add(row);
                
                
            }
            dgvResults.DataSource = table;

            //chart
            chart1.Series.Clear();
            
            for (int i = 0; i < 6; i++)
			{
			    Series series = chart1.Series.Add(string.Format("Mass: {0}",50*Math.Pow(4,i+1)));
                series.BorderWidth = 3;
                series.ChartType = SeriesChartType.SplineArea;
                for (int j = 0; j < 100; j++)
                {
                    series.Points.AddXY(j, data[i][j]);
                }
			}
                
            
        }

        private void do6()
        {
            Ship currentShip;

            DataTable table = new DataTable();
            double[][] data = new double[6][];

            for (int countOfColumns = 0; countOfColumns < 6; countOfColumns++)
            {
                table.Columns.Add(new DataColumn(string.Format("scale: {0}", countOfColumns+2), typeof(string)));
                data[countOfColumns] = new double[100];
            }

            for (int countOfEngines = 0; countOfEngines < 100; countOfEngines++)
            {
                DataRow row = table.NewRow();
                for (int scale = 0; scale < 6; scale++)
                {
                    currentShip = new Ship(countOfEngines, Convert.ToDouble(tbxMass.Text),scale+2);
                    row[scale] = "Eng: "+ countOfEngines.ToString() + " MP: " + currentShip.GetMP()+" Mass: "+currentShip.TotalMass.ToString();
                    data[scale][countOfEngines] = currentShip.GetMP();
                }
                table.Rows.Add(row);


            }
            dgvResults.DataSource = table;

            //chart
            chart1.Series.Clear();
            

            for (int i = 0; i < 6; i++)
            {
                Series series = chart1.Series.Add(string.Format("Scale: {0}", i+2));
                series.BorderWidth = 3;
                series.ChartType = SeriesChartType.SplineArea;
                for (int j = 0; j < 100; j++)
                {
                    series.Points.AddXY(j, data[i][j]);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            do5();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            do6();
        }
    }
    class EnginePart
    {
        public double Thrust = 100d;
        public double Mass = 50d;

        public EnginePart(double thrust)
        {
            this.Thrust = thrust;
        }
    }
    class ShipHull
    {
        
        public double Mass = 500d;
        public ShipHull(double mass)
        {
            this.Mass = mass;
        }
    }
    class Ship
    {
        public List<EnginePart> Equipment = new List<EnginePart>();
        public ShipHull HullType;
        public double scale = 2;
        
        public double TotalMass
        {
            get
            {
                double mass = HullType.Mass+50*8;
                foreach (var part in Equipment)
                    mass += part.Mass;
                return mass;
            }
        }
        public int GetMP()
        {
            double totalThrust = 0;
            foreach (EnginePart part in this.Equipment.Where(f => f is EnginePart))
                totalThrust += part.Thrust;
            if (totalThrust <= 0)
                return 0;
            double SizeCategory = Math.Log((TotalMass/50),4);
            
            double EngineCount = totalThrust / 100;
            
            double MP = (-0.0332 * SizeCategory + 0.405) * EngineCount + (-0.29093 * SizeCategory + 1.7867);

            return Convert.ToInt32(Math.Round(MP));
        }

        public static double diminishing_returns(double val, double scale)
        {
            if (val < 0)
                return -diminishing_returns(-val, scale);
            double mult = val / scale;
            double trinum = (Math.Sqrt(8.0 * mult + 1.0) - 1.0) / 2.0;
            return trinum * scale;
        }
                
        public Ship(int countEngines, double mass)
        {
            for (int i = 0; i < countEngines; i++)
            {
                Equipment.Add(new EnginePart(100));
            }
            this.HullType = new ShipHull(mass);
        }
        public Ship(int countEngines, double mass, double scale)
        {
            for (int i = 0; i < countEngines; i++)
            {
                Equipment.Add(new EnginePart(100));
            }
            this.HullType = new ShipHull(mass);
            this.scale = scale;
        }
    }
}
