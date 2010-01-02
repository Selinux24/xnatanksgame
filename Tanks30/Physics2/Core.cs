using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    public abstract class Core
    {
        public static float SleepEpsilon = 1f;

        public static float Pow(float a, float b)
        {
            return Convert.ToSingle(Math.Pow(a, b));
        }

        public static void TransformInertiaTensor(ref Quaternion q, ref Matrix3 iitBody, ref Matrix rotmat, out Matrix3 iitWorld)
        {
            float t4 = rotmat.M11 * iitBody.M11 + rotmat.M12 * iitBody.M21 + rotmat.M13 * iitBody.M31;
            float t9 = rotmat.M11 * iitBody.M12 + rotmat.M12 * iitBody.M22 + rotmat.M13 * iitBody.M32;
            float t14 = rotmat.M11 * iitBody.M13 + rotmat.M12 * iitBody.M23 + rotmat.M13 * iitBody.M33;
            float t28 = rotmat.M21 * iitBody.M11 + rotmat.M22 * iitBody.M21 + rotmat.M23 * iitBody.M31;
            float t33 = rotmat.M21 * iitBody.M12 + rotmat.M22 * iitBody.M22 + rotmat.M23 * iitBody.M32;
            float t38 = rotmat.M21 * iitBody.M13 + rotmat.M22 * iitBody.M23 + rotmat.M23 * iitBody.M33;
            float t52 = rotmat.M31 * iitBody.M11 + rotmat.M32 * iitBody.M21 + rotmat.M33 * iitBody.M31;
            float t57 = rotmat.M31 * iitBody.M12 + rotmat.M32 * iitBody.M22 + rotmat.M33 * iitBody.M32;
            float t62 = rotmat.M31 * iitBody.M13 + rotmat.M32 * iitBody.M23 + rotmat.M33 * iitBody.M33;

            iitWorld.M11 = t4 * rotmat.M11 + t9 * rotmat.M12 + t14 * rotmat.M13;
            iitWorld.M12 = t4 * rotmat.M21 + t9 * rotmat.M22 + t14 * rotmat.M23;
            iitWorld.M13 = t4 * rotmat.M31 + t9 * rotmat.M32 + t14 * rotmat.M33;
            iitWorld.M21 = t28 * rotmat.M11 + t33 * rotmat.M12 + t38 * rotmat.M13;
            iitWorld.M22 = t28 * rotmat.M21 + t33 * rotmat.M22 + t38 * rotmat.M23;
            iitWorld.M23 = t28 * rotmat.M31 + t33 * rotmat.M32 + t38 * rotmat.M33;
            iitWorld.M31 = t52 * rotmat.M11 + t57 * rotmat.M12 + t62 * rotmat.M13;
            iitWorld.M32 = t52 * rotmat.M21 + t57 * rotmat.M22 + t62 * rotmat.M23;
            iitWorld.M33 = t52 * rotmat.M31 + t57 * rotmat.M32 + t62 * rotmat.M33;
        }
        /// <summary>
        /// Obtiene la matriz de coheficientes de tensor de inercia
        /// </summary>
        /// <param name="ix">Coheficiente en X</param>
        /// <param name="iy">Coheficiente en Y</param>
        /// <param name="iz">Coheficiente en Z</param>
        /// <returns>Devuelve la matriz de coheficientes de tensor de inercia</returns>
        public static Matrix3 SetInertiaTensorCoeffs(float ix, float iy, float iz)
        {
            return SetInertiaTensorCoeffs(ix, iy, iz, 0f, 0f, 0f);
        }
        /// <summary>
        /// Obtiene la matriz de coheficientes de tensor de inercia
        /// </summary>
        /// <param name="ix">Coheficiente en X</param>
        /// <param name="iy">Coheficiente en Y</param>
        /// <param name="iz">Coheficiente en Z</param>
        /// <param name="ixy"></param>
        /// <param name="ixz"></param>
        /// <param name="iyz"></param>
        /// <returns>Devuelve la matriz de coheficientes de tensor de inercia</returns>
        public static Matrix3 SetInertiaTensorCoeffs(float ix, float iy, float iz, float ixy, float ixz, float iyz)
        {
            Matrix3 matrix = new Matrix3();

            matrix.M11 = ix;
            matrix.M12 = matrix.M21 = -ixy;
            matrix.M13 = matrix.M31 = -ixz;
            matrix.M22 = iy;
            matrix.M23 = matrix.M32 = -iyz;
            matrix.M33 = iz;

            return matrix;
        }

        public static Matrix3 SetInertiaTensorToBox(Vector3 halfSizes, float mass)
        {
            Vector3 squares = ComponentProductUpdate(halfSizes, halfSizes);

            return SetInertiaTensorCoeffs(
                0.3f * mass * (squares.Y + squares.Z),
                0.3f * mass * (squares.X + squares.Z),
                0.3f * mass * (squares.X + squares.Y));
        }

        public static Quaternion AddScaledVector(Vector3 vector, float scale, Quaternion q)
        {
            Quaternion newQuaternion = new Quaternion(vector * scale, 0f);

            newQuaternion *= q;
            q.W += newQuaternion.W * (0.5f);
            q.X += newQuaternion.X * (0.5f);
            q.Y += newQuaternion.Y * (0.5f);
            q.Z += newQuaternion.Z * (0.5f);

            return newQuaternion;
        }

        public static void AddScaledVector(ref Quaternion rotation, Vector3 vector, float scale)
        {
            Quaternion q = new Quaternion(vector.X * scale, vector.Y * scale, vector.Z * scale, 0f);

            q *= rotation;
            rotation.W += q.W * 0.5f;
            rotation.X += q.X * 0.5f;
            rotation.Y += q.Y * 0.5f;
            rotation.Z += q.Z * 0.5f;
        }

        public static Vector3 ComponentProductUpdate(Vector3 one, Vector3 two)
        {
            return new Vector3(
                one.X * two.X,
                one.Y * two.Y,
                one.Z * two.Z);
        }
    }
}