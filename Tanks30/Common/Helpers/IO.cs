using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Common.Helpers
{
    using Common.Math;

    /// <summary>
    /// Funciones de lectura y escritura
    /// </summary>
    public static class IO
    {
        /// <summary>
        /// Almacena en un fichero el Quaternion especificado
        /// </summary>
        /// <param name="q">Quaternion</param>
        /// <param name="filename">Fichero</param>
        public static void SaveToFile(Quaternion q, string filename)
        {
            StreamWriter wr = new StreamWriter(filename);
            try
            {
                wr.WriteLine(q.X);
                wr.WriteLine(q.Y);
                wr.WriteLine(q.Z);
                wr.WriteLine(q.W);
            }
            finally
            {
                wr.Close();
            }
        }
        /// <summary>
        /// Almacena en un fichero la Matriz especificada
        /// </summary>
        /// <param name="m">Matriz</param>
        /// <param name="filename">Fichero</param>
        public static void SaveToFile(Matrix m, string filename)
        {
            StreamWriter wr = new StreamWriter(filename);
            try
            {
                wr.WriteLine(m.M11);
                wr.WriteLine(m.M12);
                wr.WriteLine(m.M13);
                wr.WriteLine(m.M14);

                wr.WriteLine(m.M21);
                wr.WriteLine(m.M22);
                wr.WriteLine(m.M23);
                wr.WriteLine(m.M24);

                wr.WriteLine(m.M31);
                wr.WriteLine(m.M32);
                wr.WriteLine(m.M33);
                wr.WriteLine(m.M34);

                wr.WriteLine(m.M41);
                wr.WriteLine(m.M42);
                wr.WriteLine(m.M43);
                wr.WriteLine(m.M44);
            }
            finally
            {
                wr.Close();
            }
        }
        /// <summary>
        /// Almacena en un fichero la Matriz especificada
        /// </summary>
        /// <param name="m">Matriz</param>
        /// <param name="filename">Fichero</param>
        public static void SaveToFile(Matrix3 m, string filename)
        {
            StreamWriter wr = new StreamWriter(filename);
            try
            {
                wr.WriteLine(m.M11);
                wr.WriteLine(m.M12);
                wr.WriteLine(m.M13);

                wr.WriteLine(m.M21);
                wr.WriteLine(m.M22);
                wr.WriteLine(m.M23);

                wr.WriteLine(m.M31);
                wr.WriteLine(m.M32);
                wr.WriteLine(m.M33);
            }
            finally
            {
                wr.Close();
            }
        }
        /// <summary>
        /// Almacena en un fichero el Vector especificado
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="filename">Fichero</param>
        public static void SaveToFile(Vector3 v, string filename)
        {
            StreamWriter wr = new StreamWriter(filename);
            try
            {
                wr.WriteLine(v.X);
                wr.WriteLine(v.Y);
                wr.WriteLine(v.Z);
            }
            finally
            {
                wr.Close();
            }
        }
        /// <summary>
        /// Carga desde un fichero un Quaternion
        /// </summary>
        /// <param name="filename">Fichero</param>
        /// <param name="q">Quaternion leído</param>
        public static void LoadFromFile(string filename, out Quaternion q)
        {
            q = new Quaternion();

            StreamReader rd = new StreamReader(filename);
            try
            {
                q.X = Convert.ToSingle(rd.ReadLine());
                q.Y = Convert.ToSingle(rd.ReadLine());
                q.Z = Convert.ToSingle(rd.ReadLine());
                q.W = Convert.ToSingle(rd.ReadLine());
            }
            finally
            {
                rd.Close();
            }
        }
        /// <summary>
        /// Carga desde un fichero una Matriz
        /// </summary>
        /// <param name="filename">Fichero</param>
        /// <param name="m">Matriz leída</param>
        public static void LoadFromFile(string filename, out Matrix m)
        {
            m = new Matrix();

            StreamReader rd = new StreamReader(filename);
            try
            {
                m.M11 = Convert.ToSingle(rd.ReadLine());
                m.M12 = Convert.ToSingle(rd.ReadLine());
                m.M13 = Convert.ToSingle(rd.ReadLine());
                m.M14 = Convert.ToSingle(rd.ReadLine());

                m.M21 = Convert.ToSingle(rd.ReadLine());
                m.M22 = Convert.ToSingle(rd.ReadLine());
                m.M23 = Convert.ToSingle(rd.ReadLine());
                m.M24 = Convert.ToSingle(rd.ReadLine());

                m.M31 = Convert.ToSingle(rd.ReadLine());
                m.M32 = Convert.ToSingle(rd.ReadLine());
                m.M33 = Convert.ToSingle(rd.ReadLine());
                m.M34 = Convert.ToSingle(rd.ReadLine());

                m.M41 = Convert.ToSingle(rd.ReadLine());
                m.M42 = Convert.ToSingle(rd.ReadLine());
                m.M43 = Convert.ToSingle(rd.ReadLine());
                m.M44 = Convert.ToSingle(rd.ReadLine());
            }
            finally
            {
                rd.Close();
            }
        }
        /// <summary>
        /// Carga desde un fichero una Matriz
        /// </summary>
        /// <param name="filename">Fichero</param>
        /// <param name="m">Matriz leída</param>
        public static void LoadFromFile(string filename, out Matrix3 m)
        {
            m = new Matrix3();

            StreamReader rd = new StreamReader(filename);
            try
            {
                m.M11 = Convert.ToSingle(rd.ReadLine());
                m.M12 = Convert.ToSingle(rd.ReadLine());
                m.M13 = Convert.ToSingle(rd.ReadLine());

                m.M21 = Convert.ToSingle(rd.ReadLine());
                m.M22 = Convert.ToSingle(rd.ReadLine());
                m.M23 = Convert.ToSingle(rd.ReadLine());

                m.M31 = Convert.ToSingle(rd.ReadLine());
                m.M32 = Convert.ToSingle(rd.ReadLine());
                m.M33 = Convert.ToSingle(rd.ReadLine());
            }
            finally
            {
                rd.Close();
            }
        }
        /// <summary>
        /// Carga desde un fichero un Vector
        /// </summary>
        /// <param name="filename">Fichero</param>
        /// <param name="v">Vector leído</param>
        public static void LoadFromFile(string filename, out Vector3 v)
        {
            v = new Vector3();

            StreamReader rd = new StreamReader(filename);
            try
            {
                v.X = Convert.ToSingle(rd.ReadLine());
                v.Y = Convert.ToSingle(rd.ReadLine());
                v.Z = Convert.ToSingle(rd.ReadLine());
            }
            finally
            {
                rd.Close();
            }
        }
    }
}
