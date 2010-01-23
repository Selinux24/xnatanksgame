using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace Physics
{
    public abstract class PhysicsMathHelper
    {
        public static bool IsZero(float number)
        {
            return (Math.Abs(number) < float.Epsilon);
        }

        public static float Angle(Vector3 vector1, Vector3 vector2)
        {
            Vector3 nVector1 = Vector3.Normalize(vector1);
            Vector3 nVector2 = Vector3.Normalize(vector2);

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

        public static Quaternion EulerToQuaternion(float roll, float pitch, float yaw)
        {
            // calculate trig identities
            double cr = Math.Cos(roll / 2);
            double cp = Math.Cos(pitch / 2);
            double cy = Math.Cos(yaw / 2);


            double sr = Math.Sin(roll / 2);
            double sp = Math.Sin(pitch / 2);
            double sy = Math.Sin(yaw / 2);

            double cpcy = cp * cy;
            double spsy = sp * sy;

            Quaternion quat;
            quat.W = Convert.ToSingle(cr * cpcy + sr * spsy);
            quat.X = Convert.ToSingle(sr * cpcy - cr * spsy);
            quat.Y = Convert.ToSingle(cr * sp * cy + sr * cp * sy);
            quat.Z = Convert.ToSingle(cr * cp * sy - sr * sp * cy);

            return quat;
        }

        public static void MatrixDecompose(Matrix mat, out Vector3 vTrans, out Vector3 vScale, out Matrix mRot)
        {
            vTrans = Vector3.Zero;
            vScale = Vector3.Zero;
            mRot = Matrix.Identity;

            Vector3[] vCols = new Vector3[]{
                    new Vector3(mat.M11,mat.M12,mat.M13),
                    new Vector3(mat.M21,mat.M22,mat.M23),
                    new Vector3(mat.M31,mat.M32,mat.M33)  
                };

            vScale.X = vCols[0].Length();
            vScale.Y = vCols[1].Length();
            vScale.Z = vCols[2].Length();


            vTrans.X = mat.M41 / (vScale.X == 0 ? 1 : vScale.X);
            vTrans.Y = mat.M42 / (vScale.Y == 0 ? 1 : vScale.Y);
            vTrans.Z = mat.M43 / (vScale.Z == 0 ? 1 : vScale.Z);

            if (vScale.X != 0)
            {
                vCols[0].X /= vScale.X;
                vCols[0].Y /= vScale.X;
                vCols[0].Z /= vScale.X;
            }
            if (vScale.Y != 0)
            {
                vCols[1].X /= vScale.Y;
                vCols[1].Y /= vScale.Y;
                vCols[1].Z /= vScale.Y;
            }
            if (vScale.Z != 0)
            {
                vCols[2].X /= vScale.Z;
                vCols[2].Y /= vScale.Z;
                vCols[2].Z /= vScale.Z;
            }

            mRot.M11 = vCols[0].X;
            mRot.M12 = vCols[0].Y;
            mRot.M13 = vCols[0].Z;
            mRot.M14 = 0;
            mRot.M41 = 0;
            mRot.M21 = vCols[1].X;
            mRot.M22 = vCols[1].Y;
            mRot.M23 = vCols[1].Z;
            mRot.M24 = 0;
            mRot.M42 = 0;
            mRot.M31 = vCols[2].X;
            mRot.M32 = vCols[2].Y;
            mRot.M33 = vCols[2].Z;
            mRot.M34 = 0;
            mRot.M43 = 0;
            mRot.M44 = 1;
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
