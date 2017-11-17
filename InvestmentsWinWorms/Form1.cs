using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvestmentsWinWorms.OptimizationService;

namespace InvestmentsWinWorms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                OptimizationServiceClient client = new OptimizationServiceClient();
                Bionic bionic = new Bionic(double.Parse(textBox1.Text),
                        double.Parse(textBox4.Text),
                        double.Parse(textBox2.Text),
                        double.Parse(textBox3.Text),
                        double.Parse(textBox5.Text),
                        double.Parse(textBox6.Text), 
                        int.Parse(textBox7.Text),
                        int.Parse(textBox8.Text));
                double[] res = null;

                if (comboBox1.SelectedIndex == 0)
                {
                    res = client.DoSimplex(double.Parse(textBox1.Text),
                        double.Parse(textBox4.Text),
                        double.Parse(textBox2.Text),
                        double.Parse(textBox3.Text),
                        double.Parse(textBox5.Text),
                        double.Parse(textBox6.Text));
                }
                else
                {
                    res = bionic.DoBionic();
                }

                label7.Text = res[0].ToString();
                label8.Text = res[1].ToString();
                label9.Text = res[2].ToString();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;

            textBox1.Text = "400000";
            textBox4.Text = "3";
            textBox2.Text = "9";
            textBox3.Text = "11";
            textBox5.Text = "305000";
            textBox6.Text = "95000";
            
            textBox7.Text = "100";
            textBox8.Text = "5";
        }
    }

    public class RefElement
    {
        public double Sum { get; set; }
        public double Ratio { get; set; }
        public double DividentsA { get; set; }
        public double DividentsB { get; set; }
        public double LimitA { get; set; }
        public double LimitB { get; set; }
        public int DimensionPopulation { get; set; }
        public int CountPopulation { get; set; }

        public RefElement(double _sum, double _ratio, double _divA, double _divB, double _limA, double _limB, int _dP, int _cP)
        {
            this.Sum = _sum;
            this.Ratio = _ratio;
            this.DividentsA = _divA * 0.01;
            this.DividentsB = _divB * 0.01;
            this.LimitA = _limA;
            this.LimitB = _limB;
            this.DimensionPopulation = _dP;
            this.CountPopulation = _cP;
        }
    }

    public class Bionic
    {
        public double Sum { get; set; }
        public double Ratio { get; set; }
        public double DividentsA { get; set; }
        public double DividentsB { get; set; }
        public double LimitA { get; set; }
        public double LimitB { get; set; }
        public int DimensionPopulation { get; set; }
        public int CountPopulation { get; set; }
        public RefElement refElement { get; set; }

        public Bionic(double _sum, double _ratio, double _divA, double _divB, double _limA, double _limB, int _dP, int _cP)
        {
            this.Sum = _sum;
            this.Ratio = _ratio;
            this.DividentsA = _divA * 0.01;
            this.DividentsB = _divB * 0.01;
            this.LimitA = _limA;
            this.LimitB = _limB;
            this.DimensionPopulation = _dP;
            this.CountPopulation = _cP;
            refElement = new RefElement(_sum, _ratio, _divA, _divB, _limA, _limB, _dP, _cP);
        }

        List<Element> points;
        int size;

        int bottomLimit = 0;

        //List<int> res;
        Element answer;
        public double[] DoBionic()
        {
            bool generate_new_flag = false;
            
            size = DimensionPopulation;
            List<Element> mas = new List<Element>(size);
            points = new List<Element>(size * CountPopulation);
            List<Element> buf = new List<Element>(size);
            mas = generate(size);
            for (int i = 0; i < CountPopulation; i++)
            {
                addPoints(mas);
                if (buf.Count != 0)
                {
                    buf.Clear();
                }
                int kol = 0, flag = 0;
                for (int j = 0; j < size; j++)
                {
                    if (mas[j].elite)
                    {
                        buf.Add(mas[j]);
                        kol++;
                    }
                }
                mas.Clear();
                for (int j = 0; j < size; j++)
                {
                    double x1 = 0, x2 = 0;
                    if (flag == kol - 1)
                    {
                        flag = 0;
                        if (generate_new_flag)
                        {
                            generate_new_flag = false;
                        }
                        else
                        {
                            generate_new_flag = true;
                        }
                    }
                    if (buf.Count != 0) // amd mow exception
                    {
                        if (generate_new_flag)
                        {
                            x2 = buf[flag].x2;
                        }
                        else
                        {
                            x1 = buf[flag].x1;
                        }
                    }
                    flag++;
                    if (flag == kol)
                    {
                        flag = 0;
                        if (generate_new_flag)
                        {
                            x2 = buf[flag].x2;
                        }
                        else
                        {
                            x1 = buf[flag].x1;
                        }

                        if (generate_new_flag)
                        {
                            generate_new_flag = false;
                        }
                        else
                        {
                            generate_new_flag = true;
                        }
                    }
                    else
                    {
                        // now exception
                        if (generate_new_flag)
                        {
                            x1 = buf[flag].x1;
                        }
                        else
                        {
                            x2 = buf[flag].x2;
                        }
                    }
                    mas.Add(genereteNewElement(x1, x2));
                }
            }
            addPoints(mas);
            //label5.Text = find_best(mas).y.ToString() + ";" + find_best(mas, func).x1.ToString() + ";" + find_best(mas, func).x2.ToString();
            answer = find_best(mas);
            //button2_Click(new object(), new EventArgs());

            double[] Result = new double[] { answer.x1, answer.x2, answer.y };
            return Result;
        }
        private Element find_best(List<Element> mass)
        {
            double res = 0;
            Element result = new Element(0, 0, this.refElement);
            String str = "";

            res = 0;
            /*
            for (int i = 0; i < countPopulation; i++)
            {
                str += mass[i].y.ToString() + ";";
                if (mass[i].y > rez)
                {
                    rez = mass[i].y;
                    rezult = mass[i];
                }
            }
            */
            {
                for (int i = 0; i < CountPopulation; i++)
                {
                    str += mass[i].y.ToString() + ";";
                    if (mass[i].y > res)
                    {
                        res = mass[i].y;
                        result = mass[i];
                    }
                }
            }

            //label2.Text = rez.ToString(); // number of population
            return result;
        }
        private List<Element> generate(int size)
        {
            List<Element> mas = new List<Element>(size);
            Random rand = new Random();

            for (int i = 0; i < size; i++)
            {
                Element elem = new Element((double)rand.Next(bottomLimit * 100, (int)LimitA * 100) / 100, (double)rand.Next(bottomLimit * 100, (int)LimitB * 100) / 100, this.refElement);
                mas.Add(elem);
            }

            return mas;
        }
        private Element genereteNewElement(double x1, double x2)
        {
            Random rand = new Random();
            if (rand.Next(0, (int)Sum) == 1)
            {
                x1 = x1 * 1.2;
                x2 = x2 * 0.8;
            }
            return new Element(x1, x2, this.refElement);
        }
        private void addPoints(List<Element> mass)
        {
            for (int i = 0; i < mass.Count; i++)
            {
                points.Add(mass[i]);
            }
        }
        //private void addToChart(element el)
        //{
        //    chart1.Series["Точки"].Points.AddXY(el.x1, el.x2);
        //    chart1.Series["Значение"].Points.AddXY(el.y, el.y);
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    chart1.Series["Точки"].Points.Clear();
        //    chart1.Series["Значение"].Points.Clear();
        //    flag++;
        //    if (flag == 1)
        //        button3.Enabled = true;

        //    if (flag == size)
        //        button2.Enabled = false;
        //    for (int i = (flag - 1) * size; i < flag * size; i++)
        //    {
        //        addToChart(points[i]);
        //    }

        //    label2.Text = flag.ToString();
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    chart1.Series["Точки"].Points.Clear();
        //    chart1.Series["Значение"].Points.Clear();
        //    flag--;
        //    if (flag == 0)
        //        button3.Enabled = false;
        //    if (flag == size - 1)
        //        button2.Enabled = true;
        //    for (int i = (flag) * size; i < flag * size + size; i++)
        //    {
        //        addToChart(points[i]);
        //    }

        //    label2.Text = flag.ToString();
        //}
    }

    public class Element
    {
        public double x1 { get; set; }
        public double x2 { get; set; }
        public double y { get; set; }
        public bool elite { get; set; }
        public RefElement RE { get; set; }

        public Element(double x1, double x2, RefElement _RE)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.RE = _RE;
            //this.func = func;
            this.elite = if_elite();
            this.ObjectiveFunction();
        }

        private void ObjectiveFunction()
        {
            this.y = (double)(this.RE.DividentsA * x1 + this.RE.DividentsB * x2);
        }

        private bool if_elite()
        {
            bool result = true;

            if ((this.x1 + this.RE.Ratio * this.x2) <= 0)
            {
                result = false;
            }
            if (this.x1 <= 0 || this.x1 > this.RE.LimitA)
            {
                result = false;
            }
            if (this.x2 <= 0 || this.x2 > this.RE.LimitB)
            {
                result = false;
            }
            if ((this.x1 + this.x2 < this.RE.Sum - 100) || this.x1 + this.x2 > this.RE.Sum + 100)
            {
                result = false;
            }

            return result;
        }
    }

    /*
    public class Bionic
    {
        public double Sum { get; set; }
        public double Ratio { get; set; }
        public double DividentsA { get; set; }
        public double DividentsB { get; set; }
        public double LimitA { get; set; }
        public double LimitB { get; set; }

        public Bionic(double _sum, double _ratio, double _divA, double _divB, double _limA, double _limB)
        {
            this.Sum = _sum;
            this.Ratio = _ratio;
            this.DividentsA = _divA * 0.01;
            this.DividentsB = _divB * 0.01;
            this.LimitA = _limA;
            this.LimitB = _limB;
        }

        private double[,] fillMatrix()
        {
            double[,] mathModel = new double[4, 3]; // Выделяем память под входную матрицу

            try
            {
                mathModel[0, 0] = 1;
                mathModel[0, 1] = 1;
                mathModel[0, 2] = this.Sum;

                mathModel[1, 0] = 1;
                mathModel[1, 1] = -this.Ratio;
                mathModel[1, 2] = 0;

                mathModel[2, 0] = 0;
                mathModel[2, 1] = 1;
                mathModel[2, 2] = LimitB;

                mathModel[3, 0] = this.DividentsA;
                mathModel[3, 1] = this.DividentsB;
            }
            catch (Exception) { return null; }

            return mathModel;
        }

        public double[] DoBionic(int N, int G) // N - Количество особей, G - Количество поколений
        {
            double[,] InA = fillMatrix();
            double[][] Mat = new double[N << 1][]; // Выделяем память под особей в 2 раза больше N, т.к. выживет только половина более приспособленных
            for (int i = 0; i < N << 1; i++)
            {
                Mat[i] = new double[InA.GetLength(1)]; // Выделяем память под иксы и значение функции
            }
            Random R = new Random();
            // Инициализация начального поколения
            {
                double SR = 0; // Начальное приближение
                for (int i = 0; i < InA.GetLength(0) - 1; i++)
                {
                    double val = 0;
                    for (int j = 0; j < InA.GetLength(1) - 1; j++)
                    {
                        val += InA[i, j];
                    }
                    val = InA[i, InA.GetLength(1) - 1] / val;
                    if (val > SR) // Если для текущего ограничения требуется большее значение икса, то заменяем
                    {
                        SR = val;
                    }
                }
                for (int i = 0; i < N;)
                {
                    for (int j = 0; j < Mat[0].Length - 1; j++)
                    {
                        Mat[i][j] = SR * (1.1 - 0.2 * R.NextDouble());
                    }
                    if (getF(InA, ref Mat[i]))
                    {
                        i++;
                    }
                }
            }
            // Основной цикл
            for (int x = 0; x < G; x++)
            {
                //PF.setValue(x * 100 / G);
                // Скрещиваем каждую особь с случайной
                for (int i = 0; i < N; i++)
                {
                    int ind = R.Next(N);
                    do
                    {
                        for (int j = 0; j < Mat[0].Length - 1; j++)
                        {
                            Mat[N + i][j] = 0.8 * Mat[i][j] + 0.2 * Mat[ind][j]; // Скрещивание
                            Mat[N + i][j] += -1 + 2 * R.NextDouble(); // Мутация
                        }
                    }
                    while (!getF(InA, ref Mat[N + i]));
                }
                // Селекция (сортировка методом пузырька)
                for (int i = 1; i < N << 1; i++)
                {
                    for (int j = 0; j < (N << 1) - i; j++)
                    {
                        if (Mat[j][Mat[0].Length - 1] > Mat[j + 1][Mat[0].Length - 1])
                        {
                            double[] buf = Mat[j];
                            Mat[j] = Mat[j + 1];
                            Mat[j + 1] = buf;
                        }
                    }
                }
                //Application.DoEvents();
                //this.Text = x.ToString();
            }
            double[] OutA = new double[InA.GetLength(1) + 1]; // Иксы, ответ, время
            for (int i = 0; i < Mat[0].Length; i++)
            {
                OutA[i] = Mat[0][i];
            }
            return OutA;
        }

        private bool getF(double[,] InA, ref double[] A)
        {
            // --- Проверяем условия ---
            for (int i = 0; i < InA.GetLength(1) - 1; i++)
            {
                if (A[i] < 0)
                {
                    return false;
                }
            }
            for (int i = 0; i < InA.GetLength(0) - 1; i++)
            {
                double s = 0;
                for (int j = 0; j < InA.GetLength(1) - 1; j++)
                {
                    s += A[j] * InA[i, j];
                }
                if (s < InA[i, InA.GetLength(1) - 1])
                {
                    return false;
                }
            }
            // Считаем значение функции
            A[A.Length - 1] = 0;
            for (int i = 0; i < InA.GetLength(1) - 1; i++)
            {
                A[A.Length - 1] += A[i] * InA[InA.GetLength(0) - 1, i];
            }
            return true;
        }
    }
    */


}
