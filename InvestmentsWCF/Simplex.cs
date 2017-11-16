using System;

namespace InvestmentsWCF
{
    public class Simplex
    {
        public double Sum { get; set; }
        public double Ratio { get; set; }
        public double DividentsA { get; set; }
        public double DividentsB { get; set; }
        public double LimitA { get; set; }
        public double LimitB { get; set; }

        public Simplex(double _sum, double _ratio, double _divA, double _divB, double _limA, double _limB)
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

        public double[] DoSimplex()
        {
            double[,] InA = fillMatrix();
            double[,] Mat = new double[InA.GetLength(0), InA.GetLength(1) + InA.GetLength(0)]; // Выделяем память для матрицы симплекс метода

            // --- Заполняем матрицу симплекс метода ---
            for (int i = 0; i < InA.GetLength(0); i++)
            {
                Mat[i, 0] = i + InA.GetLength(1); // Базис
                Mat[i, 1] = -InA[i, InA.GetLength(1) - 1]; // B
                for (int j = 0; j < InA.GetLength(1) - 1; j++)
                    Mat[i, j + 2] = -InA[i, j]; // Иксы и коэффициенты целевой функции
                for (int j = 0; j < InA.GetLength(0) - 1; j++) // Базисные иксы и коэффициенты целевой функции
                    if (i == j)
                        Mat[i, j + 4] = 1;
                    else
                        Mat[i, j + 4] = 0;
            }

            // Основной цикл симплекс метода
            do
            {
                // Среди отрицательных значений базисных переменных выбираем наибольший по модулю
                int indI = -1;
                for (int i = 0; i < Mat.GetLength(0); i++)
                {
                    if (Mat[i, 1] < 0)
                    {
                        if (indI == -1)
                        {
                            indI = i;
                        }
                        else
                        {
                            if (Mat[i, 1] < Mat[indI, 1])
                            {
                                indI = i;
                            }
                        }
                    }
                }

                int indJ = 0;
                if (indI != -1)
                {
                    // Находим θ
                    double[] Teta = new double[InA.GetLength(1) + InA.GetLength(0) - 2];
                    for (int j = 0; j < InA.GetLength(1) + InA.GetLength(0) - 2; j++)
                    {
                        if (Mat[indI, j + 2] != 0 && Mat[InA.GetLength(0) - 1, j + 2] != 0)
                        {
                            Teta[j] = Mat[InA.GetLength(0) - 1, j + 2] / Mat[indI, j + 2];
                        }
                        else
                        {
                            Teta[j] = double.MaxValue;
                        }
                    }

                    // Находим минимум θ
                    for (int j = 1; j < Teta.Length; j++)
                    {
                        if (Teta[j] < Teta[indJ])
                        {
                            indJ = j;
                        }
                    }
                    indJ += 2; // Смещаем на 2 элемента чтобы индексы соответствовали матрице Mat
                }

                // Завершаем цикл если все базисные переменные положительны
                if (indI == -1)
                {
                    break;
                }

                // Заменяем базис
                Mat[indI, 0] = indJ - 1; // Т.к. индексация базиса от 1

                // Выполняем преобразование методом Жордано-Гаусса
                double val = Mat[indI, indJ];
                for (int j = 1; j < Mat.GetLength(1); j++)
                {
                    Mat[indI, j] /= val;
                }

                for (int i = 0; i < Mat.GetLength(0); i++)
                {
                    if (i != indI)
                    {
                        double k = Mat[i, indJ] / Mat[indI, indJ]; // Коэффициент для текущей строки
                        for (int j = 1; j < Mat.GetLength(1); j++)
                        {
                            Mat[i, j] -= k * Mat[indI, j];
                        }
                    }
                }
            } while (true);

            double[] OutA = new double[InA.GetLength(1) + 1]; // Иксы, ответ, время

            for (int x = 1; x < InA.GetLength(1); x++) // Находим иксы
            {
                for (int i = 0; i < InA.GetLength(0) - 1; i++)
                {
                    if (Mat[i, 0] == x)
                    {
                        OutA[x - 1] = Mat[i, 1];
                    }
                }
            }

            OutA[InA.GetLength(1) - 1] = Mat[InA.GetLength(0) - 1, 1];
            return OutA;
        }
    }


    /*
    public class Simplex
    {
        //source - симплекс таблица без базисных переменных
        double[,] table; //симплекс таблица

        int m, n;

        List<int> basis; //список базисных переменных

        public Simplex(double[,] source)
        {
            m = source.GetLength(0);
            n = source.GetLength(1);
            table = new double[m, n + m - 1];
            basis = new List<int>();

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (j < n)
                        table[i, j] = source[i, j];
                    else
                        table[i, j] = 0;
                }
                //выставляем коэффициент 1 перед базисной переменной в строке
                if ((n + i) < table.GetLength(1))
                {
                    table[i, n + i] = 1;
                    basis.Add(n + i);
                }
            }

            n = table.GetLength(1);
        }

        //result - в этот массив будут записаны полученные значения X
        public double[,] Calculate(double[] result)
        {
            int mainCol, mainRow; //ведущие столбец и строка

            while (!IsItEnd())
            {
                mainCol = findMainCol();
                mainRow = findMainRow(mainCol);
                basis[mainRow] = mainCol;

                double[,] new_table = new double[m, n];

                for (int j = 0; j < n; j++)
                    new_table[mainRow, j] = table[mainRow, j] / table[mainRow, mainCol];

                for (int i = 0; i < m; i++)
                {
                    if (i == mainRow)
                        continue;

                    for (int j = 0; j < n; j++)
                        new_table[i, j] = table[i, j] - table[i, mainCol] * new_table[mainRow, j];
                }
                table = new_table;
            }

            //заносим в result найденные значения X
            for (int i = 0; i < result.Length; i++)
            {
                int k = basis.IndexOf(i + 1);
                if (k != -1)
                    result[i] = table[k, 0];
                else
                    result[i] = 0;
            }

            return table;
        }

        private bool IsItEnd()
        {
            bool flag = true;

            for (int j = 1; j < n; j++)
            {
                if (table[m - 1, j] < 0)
                {
                    flag = false;
                    break;
                }
            }

            return flag;
        }

        private int findMainCol()
        {
            int mainCol = 1;

            for (int j = 2; j < n; j++)
                if (table[m - 1, j] < table[m - 1, mainCol])
                    mainCol = j;

            return mainCol;
        }

        private int findMainRow(int mainCol)
        {
            int mainRow = 0;

            for (int i = 0; i < m - 1; i++)
                if (table[i, mainCol] > 0)
                {
                    mainRow = i;
                    break;
                }

            for (int i = mainRow + 1; i < m - 1; i++)
                if ((table[i, mainCol] > 0) && ((table[i, 0] / table[i, mainCol]) < (table[mainRow, 0] / table[mainRow, mainCol])))
                    mainRow = i;

            return mainRow;
        }
    }
    */
}
