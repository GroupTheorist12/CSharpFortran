using System;
using System.Runtime.InteropServices;

namespace CSharpFortran
{
    class Program
    {
        [DllImport("csharp_module.so", CallingConvention = CallingConvention.Cdecl)]
        static extern void PassString(string str, ref int n);

        [DllImport("csharp_module.so", CallingConvention = CallingConvention.Cdecl)]
        static extern double DegCtoF(double[] degC, double[] degF, ref int n);

        [StructLayout(LayoutKind.Sequential)]
        struct point3d
        {
            public float x;
            public float y;
            public float z;

        };

        [DllImport("csharp_module.so", CallingConvention = CallingConvention.Cdecl)]
        static extern void fflip(ref point3d p);


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

        }
    }
}
