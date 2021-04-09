using System;
using System.IO;
using System.Text;
using Accord.Math;

namespace laba5_jpeg
{
    class Program
    {
        static void Main(string[] args)
        {
            int q = 3;//коефіцієнт ущільнення

            double[,] matrix =
            {
                {71, 61, 59, 59, 68, 70, 93, 73},
                {68, 67, 71, 69, 76, 76, 91, 80},
                {80, 77, 81, 83, 88, 84, 84, 90},
                {93, 83, 84, 91, 101, 91, 97, 97},
                {85, 80, 80, 87, 94, 91, 97, 101},
                {77, 76, 78, 78, 76, 76, 93, 99},
                {70, 73, 79, 76, 67, 65, 85, 95},
                {74, 80, 91, 83, 65, 56, 79, 102},
            };


            double[,] DctMatrix = new double[matrix.GetLength(0), matrix.GetLength(1)];

            Array.Copy(matrix, DctMatrix, DctMatrix.Length);

            Console.WriteLine("\n                  original");
            WriteArrayToConsole(DctMatrix);

            CosineTransform.DCT(DctMatrix);
            Console.WriteLine("\n                  DCT");
            WriteArrayToConsole(DctMatrix);

            Console.WriteLine("\n                  quantization Matrix");
            int[,] quantizationMatrix = Quantization(q, DctMatrix.GetLength(0));
            WriteArrayToConsole(quantizationMatrix);

            Console.WriteLine("\n                  division of matrices");
            int[,] quantizationMatrixDividedDct = MatrixDivision(DctMatrix, quantizationMatrix);
            WriteArrayToConsole(quantizationMatrixDividedDct);

            Console.WriteLine("\n                  multiply of matrices");
            double[,] quantizationMatrixDividedDctMultiplicationOfquantizationMatrix = MatrixMultiplyDouble(quantizationMatrix, quantizationMatrixDividedDct);
            WriteArrayToConsole(quantizationMatrixDividedDctMultiplicationOfquantizationMatrix);

            double[,] IDEctMatrix = new double[quantizationMatrixDividedDctMultiplicationOfquantizationMatrix.GetLength(0), quantizationMatrixDividedDctMultiplicationOfquantizationMatrix.GetLength(1)];
            Array.Copy(quantizationMatrixDividedDctMultiplicationOfquantizationMatrix, IDEctMatrix, IDEctMatrix.Length);

            Console.WriteLine("\n                  contrary DCT of matrices");
            CosineTransform.IDCT(IDEctMatrix);
            WriteArrayToConsole(IDEctMatrix);

            //збереженні в файл

            string data = FillInTheVariable(matrix, DctMatrix, quantizationMatrix, quantizationMatrixDividedDct, quantizationMatrixDividedDctMultiplicationOfquantizationMatrix, IDEctMatrix);

            // создаем каталог для файла
            string path = @"C:\DCT";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            SaveFile(@"C:\DCT\Dct.txt", data);

            Console.Read();
        }

        private static string FillInTheVariable(double[,] matrix, double[,] dctMatrix, int[,] quantizationMatrix, int[,] quantizationMatrixDividedDct, double[,] quantizationMatrixDividedDctMultiplicationOfquantizationMatrix, double[,] iDEctMatrix)
        {
            string data = String.Empty;

            data += "\r\n                  original\r\n";
            Set2DArrayForData(matrix,ref data);

            data += "\r\n\r\n                  DCT\r\n";
            Set2DArrayForData(dctMatrix, ref data);

            data += "\r\n\r\n                  quantization Matrix\r\n";
            Set2DArrayForData(quantizationMatrix, ref data);

            data += "\r\n\r\n                  division of matrices\r\n";
            Set2DArrayForData(quantizationMatrixDividedDct, ref data);

            data += "\r\n\r\n                  multiply of matrices\r\n";
            Set2DArrayForData(quantizationMatrixDividedDctMultiplicationOfquantizationMatrix, ref data);

            data += "\r\n\r\n                  contrary DCT of matrices\r\n";
            Set2DArrayForData(iDEctMatrix, ref data);

            return data;
        }


        /// <summary>
        /// множення матриць
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        private static int[,] MatrixMultiply(int[,] m1, int[,] m2)
        {
            if (m1.Length != m2.Length)
                return null;
            int[,] returnMatrix = new int[m1.GetLength(0), m1.GetLength(1)];

            for (int i = 0; i < returnMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < returnMatrix.GetLength(1); j++)
                {
                    returnMatrix[i, j] = m1[i, j] * m2[i, j];
                }
            }
            return returnMatrix;
        }
        private static double[,] MatrixMultiplyDouble(int[,] m1, int[,] m2)
        {
            if (m1.Length != m2.Length)
                return null;
            double[,] returnMatrix = new double[m1.GetLength(0), m1.GetLength(1)];

            for (int i = 0; i < returnMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < returnMatrix.GetLength(1); j++)
                {
                    returnMatrix[i, j] = m1[i, j] * m2[i, j];
                }
            }
            return returnMatrix;
        }


        /// <summary>
        /// Ділить матриці
        /// </summary>
        /// <param name="divided">ділене</param>
        /// <param name="divider">дільник</param>
        /// <returns></returns>
        private static int[,] MatrixDivision(double[,] divided, int[,] divider)
        {
            if (divided.Length != divider.Length)
                return null;

            int[,] returnMatrix = new int[divided.GetLength(0), divided.GetLength(0)];

           

            for (int i = 0; i < divided.GetLength(0); i++)
            {
                for (int j = 0; j < divided.GetLength(0); j++)
                {
                    returnMatrix[i, j] = (int)Math.Round((double)(divided[i, j] / divider[i, j]));
                }
            }

            return returnMatrix;
        }


        private static int[,] Quantization(int q, int n)
        {
            int[,] resultMatrix = new int[n, n];

            for (int i = 0; i < resultMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < resultMatrix.GetLength(1); j++)
                {
                    resultMatrix[i, j] = 1 + ((1 + i + j) * q); 
                }
            }

            return resultMatrix;
        }

        static void WriteArrayToConsole(double[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    Console.Write(Math.Round(array[i, j]) + "\t");
                }
                Console.WriteLine();
            }
        }
        static void WriteArrayToConsole(int[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    Console.Write(array[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
        static void WriteArrayToConsole(byte[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    Console.Write(array[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }



        static void SaveFile(string path, string data)
        {
            // сохраняем текст в файл
            using (FileStream fstream = new FileStream($@"{path}", FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = Encoding.Default.GetBytes(data);
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Console.WriteLine($"Файл з iнформацiєю було створено за наступним шляхом - {path}");
            }
        }

        static void Set2DArrayForData<T>(T[,] array, ref string data)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    data += Convert.ToString(Convert.ToInt32(array[i, j]));
                    data += "\t";
                }
                data += "\r\n";
            }
        }
    }
}

// формули з лаби
/*/// <summary>
        /// дискретне косинусне перетворення
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        static double[,] DCT(byte[,] matrix)
        {
            double[,] resultMatrix = new double[matrix.GetLength(0), matrix.GetLength(1)];

            int n = matrix.GetLength(0);
            double f = 0;
            double cU = 0;
            double cV = 0;

            for (int u = 0; u < matrix.GetLength(0); u++)
            {
                for (int v = 0; v < matrix.GetLength(1); v++)
                {
                    f = 0;
                    
                    for (int x = 0; x < matrix.GetLength(0); x++)
                    {
                        for (int y = 0; y < matrix.GetLength(1); y++)
                        {
                            double cos1 = Math.Cos((Math.PI * u * (2 * x + 1)) / (2 * n));
                            double cos2 = Math.Cos((Math.PI * v * (2 * y + 1)) / (2 * n));
                            f += matrix[x, y] * cos1 * cos2;
                        }
                    }

                    if (u == 0)
                        cU = 1 / Math.Sqrt(2);
                    else
                        cU = 1;
                    if (v == 0)
                        cV = 1 / Math.Sqrt(2);
                    else
                        cV = 1;

                    double w = (cU * cV) / Math.Sqrt(2 * n);

                    resultMatrix[u, v] = Math.Round(w * f);
                }
            }

            return resultMatrix;
        }

        /// <summary>
        /// зворотнє ДКП
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static int[,] ContraryDCT(int[,] matrix)
        {
            int[,] resultMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];

            int n = matrix.GetLength(0);
            double f;
            double cU;
            double cV;

            for (int u = 0; u < matrix.GetLength(0); u++)
            {
                for (int v = 0; v < matrix.GetLength(1); v++)
                {
                    f = 0;

                    if (u == 0)
                        cU = 1 / Math.Sqrt(2);
                    else
                        cU = 1;
                    if (v == 0)
                        cV = 1 / Math.Sqrt(2);
                    else
                        cV = 1;

                    for (int x = 0; x < resultMatrix.GetLength(0); x++)
                    {
                        for (int y = 0; y < resultMatrix.GetLength(1); y++)
                        {
                            double cos1 = Math.Cos((Math.PI * u * (2 * x + 1)) / (2 * n));
                            double cos2 = Math.Cos((Math.PI * v * (2 * y + 1)) / (2 * n));
                            f += cU * cV * matrix[x, y] * cos1 * cos2;
                        }
                    }


                    double w = 1 / Math.Sqrt(2 * n);

                    resultMatrix[u, v] = (int)Math.Round(w * f);
                }
            }

            return resultMatrix;
        }*/


/*/// <summary>
        /// дискретне косинусне перетворення
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        static double[,] DCT(int[,] matrix)
        {
            double[,] resultMatrix = new double[matrix.GetLength(0), matrix.GetLength(1)];

            int n = matrix.GetLength(0);
            double f = 0;
            double cU = 0;
            double cV = 0;

            for (int u = 0; u < matrix.GetLength(0); u++)
            {
                for (int v = 0; v < matrix.GetLength(1); v++)
                {
                    f = 0;

                    if (u == 0)
                        cU = 1 / Math.Sqrt(n);
                    else
                        cU = Math.Sqrt(2 / n);

                    if (v == 0)
                        cV = 1 / Math.Sqrt(n);
                    else
                        cV = Math.Sqrt(2 / n);

                    for (int x = 0; x < matrix.GetLength(0); x++)
                    {
                        for (int y = 0; y < matrix.GetLength(1); y++)
                        {
                            double cos1 = Math.Cos((Math.PI * u * (2 * x + 1)) / (2 * n));
                            double cos2 = Math.Cos((Math.PI * v * (2 * y + 1)) / (2 * n));
                            f += matrix[x, y] * cos1 * cos2;
                        }
                    }

                    double w = cU * cV;

                    resultMatrix[u, v] = Math.Round(w * f);
                }
            }

            return resultMatrix;
        }

        /// <summary>
        /// зворотнє ДКП
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static int[,] ContraryDCT(int[,] matrix)
        {
            int[,] resultMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];

            int n = matrix.GetLength(0);
            double f;
            double cU;
            double cV;

            for (int u = 0; u < matrix.GetLength(0); u++)
            {
                for (int v = 0; v < matrix.GetLength(1); v++)
                {
                    f = 0;

                    if (u == 0)
                        cU = 1 / Math.Sqrt(n);
                    else
                        cU = Math.Sqrt(2 / n);

                    if (v == 0)
                        cV = 1 / Math.Sqrt(n);
                    else
                        cV = Math.Sqrt(2 / n);

                    for (int x = 0; x < resultMatrix.GetLength(0); x++)
                    {
                        for (int y = 0; y < resultMatrix.GetLength(1); y++)
                        {
                            double cos1 = Math.Cos((Math.PI * u * (2 * x + 1)) / (2 * n));
                            double cos2 = Math.Cos((Math.PI * v * (2 * y + 1)) / (2 * n));
                            f += cU * cV * matrix[x, y] * cos1 * cos2;
                        }
                    }


                    resultMatrix[u, v] = (int)Math.Round(f);
                }
            }

            return resultMatrix;
        }*/
