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
                        double.Parse(textBox6.Text));
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
                    res = bionic.DoBionic(int.Parse(textBox7.Text),
                        int.Parse(textBox8.Text));
                }

                label7.Text = res[0].ToString();
                label8.Text = res[1].ToString();
                label9.Text = res[2].ToString();
            }
            catch (Exception) { }
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
}
