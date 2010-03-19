using System;
using System.IO;
using Microsoft.Xna.Framework;
using Common.Primitives;
using System.Collections.Generic;

namespace Physics
{
    public static class PhysicsMathHelper
    {
        /// <summary>
        /// Obtiene si el número está cercano a cero o es 0
        /// </summary>
        /// <param name="number">Número</param>
        /// <returns>Devuelve verdadero si el número está cercano a cero o es 0</returns>
        public static bool IsZero(this float number)
        {
            return (Math.Abs(number) < float.Epsilon);
        }
        /// <summary>
        /// Obtiene el ángulo en radianes entre los vectore
        /// </summary>
        /// <param name="thisVector">Vector actual</param>
        /// <param name="vector">Vector</param>
        /// <returns>Devuelve el ángulo que forman los vectores especificados</returns>
        public static float Angle(this Vector3 thisVector, Vector3 vector)
        {
            Vector3 nVector1 = Vector3.Normalize(thisVector);
            Vector3 nVector2 = Vector3.Normalize(vector);

            if (!nVector1.Equals(nVector2))
            {
                float dot = Vector3.Dot(nVector1, nVector2);
                double acos = Math.Acos(dot);
                if (!double.IsNaN(acos))
                {
                    return Convert.ToSingle(acos);
                }
            }

            return 0f;
        }
        /// <summary>
        /// Descompone la matriz en los componentes de escala, rotación y traslación
        /// </summary>
        /// <param name="thisMatrix">Matriz</param>
        /// <param name="scale">Escala</param>
        /// <param name="rotation">Rotación</param>
        /// <param name="translation">Traslación</param>
        public static void Decompose(this Matrix thisMatrix, out Vector3 scale, out Matrix rotation, out Vector3 translation)
        {
            translation = Vector3.Zero;
            scale = Vector3.Zero;
            rotation = Matrix.Identity;

            Vector3[] vCols = new Vector3[]
            {
                new Vector3(thisMatrix.M11,thisMatrix.M12,thisMatrix.M13),
                new Vector3(thisMatrix.M21,thisMatrix.M22,thisMatrix.M23),
                new Vector3(thisMatrix.M31,thisMatrix.M32,thisMatrix.M33)  
            };

            scale.X = vCols[0].Length();
            scale.Y = vCols[1].Length();
            scale.Z = vCols[2].Length();

            translation.X = thisMatrix.M41 / (scale.X == 0 ? 1 : scale.X);
            translation.Y = thisMatrix.M42 / (scale.Y == 0 ? 1 : scale.Y);
            translation.Z = thisMatrix.M43 / (scale.Z == 0 ? 1 : scale.Z);

            if (scale.X != 0)
            {
                vCols[0].X /= scale.X;
                vCols[0].Y /= scale.X;
                vCols[0].Z /= scale.X;
            }
            if (scale.Y != 0)
            {
                vCols[1].X /= scale.Y;
                vCols[1].Y /= scale.Y;
                vCols[1].Z /= scale.Y;
            }
            if (scale.Z != 0)
            {
                vCols[2].X /= scale.Z;
                vCols[2].Y /= scale.Z;
                vCols[2].Z /= scale.Z;
            }

            rotation.M11 = vCols[0].X;
            rotation.M12 = vCols[0].Y;
            rotation.M13 = vCols[0].Z;
            rotation.M14 = 0;
            rotation.M41 = 0;
            rotation.M21 = vCols[1].X;
            rotation.M22 = vCols[1].Y;
            rotation.M23 = vCols[1].Z;
            rotation.M24 = 0;
            rotation.M42 = 0;
            rotation.M31 = vCols[2].X;
            rotation.M32 = vCols[2].Y;
            rotation.M33 = vCols[2].Z;
            rotation.M34 = 0;
            rotation.M43 = 0;
            rotation.M44 = 1;
        }
        /// <summary>
        /// Descompone el Quaternion en los componentes de rotación
        /// </summary>
        /// <param name="thisQuaternion">Quaternion</param>
        /// <param name="yaw">Rotación en el eje vertical</param>
        /// <param name="pitch">Rotación en el eje horizontal</param>
        /// <param name="roll">Rotación en el eje de visión</param>
        public static void Decompose(this Quaternion thisQuaternion, out float yaw, out float pitch, out float roll)
        {
            yaw = (float)Math.Atan2
            (
                2 * thisQuaternion.Y * thisQuaternion.W - 2 * thisQuaternion.X * thisQuaternion.Z,
                1 - 2 * Math.Pow(thisQuaternion.Y, 2) - 2 * Math.Pow(thisQuaternion.Z, 2)
            );

            roll = (float)Math.Asin
            (
                2 * thisQuaternion.X * thisQuaternion.Y + 2 * thisQuaternion.Z * thisQuaternion.W
            );

            pitch = (float)Math.Atan2
            (
                2 * thisQuaternion.X * thisQuaternion.W - 2 * thisQuaternion.Y * thisQuaternion.Z,
                1 - 2 * Math.Pow(thisQuaternion.X, 2) - 2 * Math.Pow(thisQuaternion.Z, 2)
            );

            if (thisQuaternion.X * thisQuaternion.Y + thisQuaternion.Z * thisQuaternion.W == 0.5)
            {
                yaw = (float)(2 * Math.Atan2(thisQuaternion.X, thisQuaternion.W));
                pitch = 0;
            }
            else if (thisQuaternion.X * thisQuaternion.Y + thisQuaternion.Z * thisQuaternion.W == -0.5)
            {
                yaw = (float)(-2 * Math.Atan2(thisQuaternion.X, thisQuaternion.W));
                pitch = 0;
            }
        }
        /// <summary>
        /// Obtiene la distancia del punto al plano especificado
        /// </summary>
        /// <param name="plane">Plano</param>
        /// <param name="point">Punto</param>
        /// <returns>Devuelve la distancia del punto al plano</returns>
        public static float Distance(this Plane plane, Vector3 point)
        {
            return Vector3.Dot(point, plane.Normal) + plane.D;
        }
        /// <summary>
        /// Obtiene los valores máximo y mínimo de los tres valores pasados
        /// </summary>
        /// <param name="x0">Valor 1</param>
        /// <param name="x1">Valor 2</param>
        /// <param name="x2">Valor 3</param>
        /// <param name="min">Valor mínimo obtenido</param>
        /// <param name="max">Valor máximo obtenido</param>
        public static void FindMinMax(float x0, float x1, float x2, out float min, out float max)
        {
            min = max = x0;

            if (x1 < min) min = x1;
            if (x1 > max) max = x1;
            if (x2 < min) min = x2;
            if (x2 > max) max = x2;
        }
        /// <summary>
        /// Obtiene x elevado a y
        /// </summary>
        /// <param name="x">Número</param>
        /// <param name="b">Potencia</param>
        /// <returns>Devuelve x elevado a y</returns>
        public static float Pow(float x, float y)
        {
            return Convert.ToSingle(Math.Pow(x, y));
        }
        /// <summary>
        /// Obtiene el producto por componentes de dos vectores
        /// </summary>
        /// <param name="thisVector">Vector</param>
        /// <param name="vector">Vector</param>
        /// <returns>Devuelve el vector resultante de multiplicar los componentes de los vectores especificados</returns>
        public static Vector3 ComponentProduct(this Vector3 thisVector, Vector3 vector)
        {
            return new Vector3(
                thisVector.X * vector.X,
                thisVector.Y * vector.Y,
                thisVector.Z * vector.Z);
        }
        /// <summary>
        /// Obtiene el volúmen de la esfera especificada
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <returns>Devuelve el volúmen de la esfera especificada</returns>
        public static float Volume(this BoundingSphere sphere)
        {
            // 4/3 de PI por el radio al cubo
            return 1.333333f * MathHelper.Pi * sphere.Radius * sphere.Radius * sphere.Radius;
        }

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
    }
}
