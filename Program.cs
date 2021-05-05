using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using System.Linq;

using System.Runtime.InteropServices;

namespace CSharpFortran
{
    class Program
    {
        [DllImport("csharp_module.so", CallingConvention = CallingConvention.Cdecl)]
        static extern void PassString(string str, ref int n);

        [DllImport("csharp_module.so", CallingConvention = CallingConvention.Cdecl)]
        static extern double DegCtoF(double[] degC, double[] degF, ref int n);

        [DllImport("csharp_module.so", CallingConvention = CallingConvention.Cdecl)]
        static extern void PassMatrix(double[,] matrix, ref int cols, ref int rows);

        [StructLayout(LayoutKind.Sequential)]
        struct point3d
        {
            public float x;
            public float y;
            public float z;

        };

        [DllImport("csharp_module.so", CallingConvention = CallingConvention.Cdecl)]
        static extern void fflip(ref point3d p);

        [DllImport("csharp_module.so", CallingConvention = CallingConvention.Cdecl)]
        static extern void ffliparr([In, Out] point3d[] p, ref int n);

        static double[,] GetTestMatrix(int Cols, int Rows)
        {
            double[,] matrix = new double[Cols, Rows];

            Random rnd = new Random(12);

            for(int i = 0; i < Cols; i++)
            {
                for(int j = 0; j < Rows; j++)
                {
                    matrix[i, j] = rnd.NextDouble();
                }
            }
            return matrix;
        }

        static void Main(string[] args)
        {
            string tofort = "Howdy Doody Man";
            int n = tofort.Length;
            PassString(tofort, ref n);

            double[] DegreesC = { 32, 64 };
            double[] DegreesF = { 0, 0 };

            n = 2;

            DegCtoF(DegreesC, DegreesF, ref n);

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine("{0} : {1:0.00} [C] = {2:0.00} [F]\n", i, DegreesC[i], DegreesF[i]);
            }

            point3d d  = new point3d{x = 3.0f, y = 7.0f, z = 9.0f};
            Console.WriteLine($"values before fortran call: x = {d.x:0.00}, y = {d.y:0.00}, z = {d.z:0.00}");

            fflip(ref d);

            Console.WriteLine($"values after fortran call: x = {d.x:0.00}, y = {d.y:0.00}, z = {d.z:0.00}");

            Console.WriteLine();

            point3d[] arr = 
            {
                new point3d{x = 3.0f, y = 7.0f, z = 9.0f},
                new point3d{x = 56.0f, y = 27.0f, z = 91.0f},
                new point3d{x = 45.3f, y = 22.7f, z = 71.0f}
            };
            foreach(point3d pd in arr)
            {
                Console.WriteLine($"values before fortran call to flipparr: x = {pd.x:0.00}, y = {pd.y:0.00}, z = {pd.z:0.00}");
                
            }

            int nn = arr.Length;
            ffliparr(arr, ref nn);

            Console.WriteLine();

            foreach(point3d pd in arr)
            {
                Console.WriteLine($"values after fortran call to flipparr: x = {pd.x:0.00}, y = {pd.y:0.00}, z = {pd.z:0.00}");
                
            }

            Console.WriteLine();
            int rows = 2;
            int cols = 2;

            double[,] matrix = GetTestMatrix(cols, rows);

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < cols; i++)
            {
                for(int j = 0; j < rows; j++)
                {
                    //matrix[i, j] = rnd.NextDouble();
                    sb.AppendFormat("{0}\t", matrix[i, j]);

                }

                sb.Append(Environment.NewLine);
            }

            Console.WriteLine("From C#");
            Console.Write(sb.ToString());
            Console.WriteLine("From Fortran");
            PassMatrix(matrix, ref cols, ref rows);

        }
    }
}
