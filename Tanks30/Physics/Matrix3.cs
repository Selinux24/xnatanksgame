using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Matriz de 3x3
    /// </summary>
    public struct Matrix3
    {
        /// <summary>
        /// Matriz identidad
        /// </summary>
        private static Matrix3 _identity;

        /// <summary>
        /// Obtiene o establece el elemento 11
        /// </summary>
        public float M11;
        /// <summary>
        /// Obtiene o establece el elemento 12
        /// </summary>
        public float M12;
        /// <summary>
        /// Obtiene o establece el elemento 13
        /// </summary>
        public float M13;
        /// <summary>
        /// Obtiene o establece el elemento 21
        /// </summary>
        public float M21;
        /// <summary>
        /// Obtiene o establece el elemento 22
        /// </summary>
        public float M22;
        /// <summary>
        /// Obtiene o establece el elemento 23
        /// </summary>
        public float M23;
        /// <summary>
        /// Obtiene o establece el elemento 31
        /// </summary>
        public float M31;
        /// <summary>
        /// Obtiene o establece el elemento 32
        /// </summary>
        public float M32;
        /// <summary>
        /// Obtiene o establece el elemento 33
        /// </summary>
        public float M33;

        /// <summary>
        /// Obtiene o establece el vector arriba
        /// </summary>
        public Vector3 Up
        {
            get
            {
                Vector3 vector;
                vector.X = this.M21;
                vector.Y = this.M22;
                vector.Z = this.M23;
                return vector;
            }
            set
            {
                this.M21 = value.X;
                this.M22 = value.Y;
                this.M23 = value.Z;
            }
        }
        /// <summary>
        /// Obtiene o establece el vector abajo
        /// </summary>
        public Vector3 Down
        {
            get
            {
                Vector3 vector;
                vector.X = -this.M21;
                vector.Y = -this.M22;
                vector.Z = -this.M23;
                return vector;
            }
            set
            {
                this.M21 = -value.X;
                this.M22 = -value.Y;
                this.M23 = -value.Z;
            }
        }
        /// <summary>
        /// Obtiene o establece el vector adelante
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                Vector3 vector;
                vector.X = -this.M31;
                vector.Y = -this.M32;
                vector.Z = -this.M33;
                return vector;
            }
            set
            {
                this.M31 = -value.X;
                this.M32 = -value.Y;
                this.M33 = -value.Z;
            }
        }
        /// <summary>
        /// Obtiene o establece el vector atrás
        /// </summary>
        public Vector3 Backward
        {
            get
            {
                Vector3 vector;
                vector.X = this.M31;
                vector.Y = this.M32;
                vector.Z = this.M33;
                return vector;
            }
            set
            {
                this.M31 = value.X;
                this.M32 = value.Y;
                this.M33 = value.Z;
            }
        }
        /// <summary>
        /// Obtiene o establece el vector izquierda
        /// </summary>
        public Vector3 Left
        {
            get
            {
                Vector3 vector;
                vector.X = -this.M11;
                vector.Y = -this.M12;
                vector.Z = -this.M13;
                return vector;
            }
            set
            {
                this.M11 = -value.X;
                this.M12 = -value.Y;
                this.M13 = -value.Z;
            }
        }
        /// <summary>
        /// Obtiene o establece el vector derecha
        /// </summary>
        public Vector3 Right
        {
            get
            {
                Vector3 vector;
                vector.X = this.M11;
                vector.Y = this.M12;
                vector.Z = this.M13;
                return vector;
            }
            set
            {
                this.M11 = value.X;
                this.M12 = value.Y;
                this.M13 = value.Z;
            }
        }

        /// <summary>
        /// Obtiene la matriz identidad
        /// </summary>
        public static Matrix3 Identity
        {
            get
            {
                return _identity;
            }
        }

        /// <summary>
        /// Obtiene la matriz de coheficientes de tensor de inercia
        /// </summary>
        /// <param name="ix">Coheficiente en X</param>
        /// <param name="iy">Coheficiente en Y</param>
        /// <param name="iz">Coheficiente en Z</param>
        /// <returns>Devuelve la matriz de coheficientes de tensor de inercia</returns>
        public static Matrix3 CreateFromInertiaTensorCoeffs(float ix, float iy, float iz)
        {
            return Matrix3.CreateFromInertiaTensorCoeffs(ix, iy, iz, 0f, 0f, 0f);
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
        public static Matrix3 CreateFromInertiaTensorCoeffs(float ix, float iy, float iz, float ixy, float ixz, float iyz)
        {
            return new Matrix3()
            {
                M11 = ix,
                M12 = -ixy,
                M13 = -ixz,

                M21 = -ixy,
                M22 = iy,
                M23 = -iyz,

                M31 = -ixz,
                M32 = -iyz,
                M33 = iz,
            };
        }

        /// <summary>
        /// Constructo éstático
        /// </summary>
        static Matrix3()
        {
            _identity = new Matrix3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="f">Número al que se inicializan todos los elementos de la matriz</param>
        public Matrix3(float f)
        {
            this.M11 = f;
            this.M12 = f;
            this.M13 = f;

            this.M21 = f;
            this.M22 = f;
            this.M23 = f;

            this.M31 = f;
            this.M32 = f;
            this.M33 = f;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="m11">Valor del elemento 11</param>
        /// <param name="m12">Valor del elemento 12</param>
        /// <param name="m13">Valor del elemento 13</param>
        /// <param name="m21">Valor del elemento 21</param>
        /// <param name="m22">Valor del elemento 22</param>
        /// <param name="m23">Valor del elemento 23</param>
        /// <param name="m31">Valor del elemento 31</param>
        /// <param name="m32">Valor del elemento 32</param>
        /// <param name="m33">Valor del elemento 33</param>
        public Matrix3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;

            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;

            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="m">Matriz 4x4</param>
        /// <remarks>La matriz 3x3 se inicializa con los elementos 11, 12, 13, 21, 22, 23, 31, 32, 33 de la matriz 4x4</remarks>
        public Matrix3(Matrix m)
        {
            this.M11 = m.M11;
            this.M12 = m.M12;
            this.M13 = m.M13;

            this.M21 = m.M21;
            this.M22 = m.M22;
            this.M23 = m.M23;

            this.M31 = m.M31;
            this.M32 = m.M32;
            this.M33 = m.M33;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="m">Matriz 3x3</param>
        /// <remarks>La matriz 3x3 se inicializa con los valores de la matriz 3x3 especificada</remarks>
        public Matrix3(Matrix3 m)
        {
            this.M11 = m.M11;
            this.M12 = m.M12;
            this.M13 = m.M13;

            this.M21 = m.M21;
            this.M22 = m.M22;
            this.M23 = m.M23;

            this.M31 = m.M31;
            this.M32 = m.M32;
            this.M33 = m.M33;
        }
        
        /// <summary>
        /// Establece los componentes de la matriz
        /// </summary>
        /// <param name="compOne">Componente 1</param>
        /// <param name="compTwo">Componente 2</param>
        /// <param name="compThree">Componente3</param>
        public void SetComponents(Vector3 compOne, Vector3 compTwo, Vector3 compThree)
        {
            this.M11 = compOne.X;
            this.M12 = compTwo.X;
            this.M13 = compThree.X;
            this.M21 = compOne.Y;
            this.M22 = compTwo.Y;
            this.M23 = compThree.Y;
            this.M31 = compOne.Z;
            this.M32 = compTwo.Z;
            this.M33 = compThree.Z;
        }
        /// <summary>
        /// Obtiene la representación del objeto actual
        /// </summary>
        /// <returns>Cadena que representa el objeto</returns>
        public override string ToString()
        {
            return (
                "{ " +
                string.Format("{{M11:{0} M12:{1} M13:{2}}} ", new object[] { this.M11.ToString(), this.M12.ToString(), this.M13.ToString() }) +
                string.Format("{{M21:{0} M22:{1} M23:{2}}} ", new object[] { this.M21.ToString(), this.M22.ToString(), this.M23.ToString() }) +
                string.Format("{{M31:{0} M32:{1} M33:{2}}} ", new object[] { this.M31.ToString(), this.M32.ToString(), this.M33.ToString() }) +
                "}");
        }
        /// <summary>
        /// Devuelve un valor que indica si la instancia actual es igual al objeto especificado.
        /// </summary>
        /// <param name="obj">Objeto</param>
        /// <returns>Devuelve verdadero si la instancia actual es igual al objeto especificado</returns>
        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is Matrix3)
            {
                flag = this.Equals((Matrix3)obj);
            }
            return flag;
        }
        /// <summary>
        /// Determina si el System.Object especificado es igual a la Matriz actual
        /// </summary>
        /// <param name="other">El System.Object a comparar con la Matrix actual</param>
        /// <returns>Devuelve verdadero si el System.Object es igual a la matriz actual</returns>
        public bool Equals(Matrix3 other)
        {
            return (
                (this.M11 == other.M11) && (this.M12 == other.M12) && (this.M13 == other.M13) &&
                (this.M21 == other.M21) && (this.M22 == other.M22) && (this.M23 == other.M23) &&
                (this.M31 == other.M31) && (this.M32 == other.M32) && (this.M33 == other.M33));
        }
        /// <summary>
        /// Obtiene el código hash del objeto
        /// </summary>
        /// <returns>Devuelve el código hash del objeto</returns>
        public override int GetHashCode()
        {
            return (
                this.M11.GetHashCode() + this.M12.GetHashCode() + this.M13.GetHashCode() +
                this.M21.GetHashCode() + this.M22.GetHashCode() + this.M23.GetHashCode() +
                this.M31.GetHashCode() + this.M32.GetHashCode() + this.M33.GetHashCode());
        }

        /// <summary>
        /// Obtiene la matriz inversa de la matriz especificada
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <returns>Devuelve la matriz inversa de la matriz especificada</returns>
        public static Matrix3 Invert(Matrix3 matrix)
        {
            Matrix3 result;
            Invert(ref matrix, out result);
            return result;
        }
        /// <summary>
        /// Obtiene la matriz inversa de la matriz especificada
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="result">Matriz inversa</param>
        public static void Invert(ref Matrix3 matrix, out Matrix3 result)
        {
            float t4 = matrix.M11 * matrix.M22;
            float t6 = matrix.M11 * matrix.M23;
            float t8 = matrix.M12 * matrix.M21;
            float t10 = matrix.M13 * matrix.M21;
            float t12 = matrix.M12 * matrix.M31;
            float t14 = matrix.M13 * matrix.M31;

            // Calculate the determinant.
            float t16 = (t4 * matrix.M33 - t6 * matrix.M32 - t8 * matrix.M33 + t10 * matrix.M32 + t12 * matrix.M23 - t14 * matrix.M22);

            // Make sure the determinant is non-zero.
            if (t16 != (float)0.0f)
            {
                float t17 = 1 / t16;
                result.M11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * t17;
                result.M12 = -(matrix.M12 * matrix.M33 - matrix.M13 * matrix.M32) * t17;
                result.M13 = (matrix.M12 * matrix.M23 - matrix.M13 * matrix.M22) * t17;
                result.M21 = -(matrix.M21 * matrix.M33 - matrix.M23 * matrix.M31) * t17;
                result.M22 = (matrix.M11 * matrix.M33 - t14) * t17;
                result.M23 = -(t6 - t10) * t17;
                result.M31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * t17;
                result.M32 = -(matrix.M11 * matrix.M32 - t12) * t17;
                result.M33 = (t4 - t8) * t17;
            }
            else
            {
                result.M11 = matrix.M11;
                result.M12 = matrix.M12;
                result.M13 = matrix.M13;
                result.M21 = matrix.M21;
                result.M22 = matrix.M22;
                result.M23 = matrix.M23;
                result.M31 = matrix.M31;
                result.M32 = matrix.M32;
                result.M33 = matrix.M33;
            }
        }
        /// <summary>
        /// Obtiene la matriz traspuesta de la matriz especificada
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <returns>Devuelve la matriz traspuesta de la matriz especificada</returns>
        public static Matrix3 Transpose(Matrix3 matrix)
        {
            Matrix3 result;
            Transpose(ref matrix, out result);
            return result;
        }
        /// <summary>
        /// Obtiene la matriz traspuesta de la matriz especificada
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="result">Matriz traspuesta</param>
        public static void Transpose(ref Matrix3 matrix, out Matrix3 result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
        }
        /// <summary>
        /// Obtiene la matriz con los componentes cambiados de signo de la matriz especificada
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <returns>Matriz negativa</returns>
        public static Matrix3 Negate(Matrix3 matrix)
        {
            Matrix3 result;
            Negate(ref matrix, out result);
            return result;
        }
        /// <summary>
        /// Obtiene la matriz con los componentes cambiados de signo de la matriz especificada
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="result">Matriz negativa</param>
        public static void Negate(ref Matrix3 matrix, out Matrix3 result)
        {
            result.M11 = -matrix.M11;
            result.M12 = -matrix.M12;
            result.M13 = -matrix.M13;
            result.M21 = -matrix.M21;
            result.M22 = -matrix.M22;
            result.M23 = -matrix.M23;
            result.M31 = -matrix.M31;
            result.M32 = -matrix.M32;
            result.M33 = -matrix.M33;
        }
        /// <summary>
        /// Transforma el vector al espacio que representa la matriz
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="vector">Vector</param>
        /// <returns>Devuelve el vector en el espacio representado por esta matriz</returns>
        public static Vector3 Transform(Matrix3 matrix, Vector3 vector)
        {
            return matrix * vector;
        }
        /// <summary>
        /// Transforma el vector al espacio que representa la matriz
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="vector">Vector</param>
        /// <param name="result">Vector en el espacio representado por esta matriz</param>
        public static void Transform(ref Matrix3 matrix, ref Vector3 vector, out Vector3 result)
        {
            result = matrix * vector;
        }
        /// <summary>
        /// Transforma la matriz 3x3 especificada usando la matriz 4x4
        /// </summary>
        /// <param name="matrix">Matriz 3x3</param>
        /// <param name="transfom">Matriz 4x4</param>
        /// <returns>Devuelve la matriz 3x3 transformada</returns>
        public static Matrix3 Transform(Matrix3 matrix, Matrix transfom)
        {
            Matrix3 tranformedMatrix = Matrix3.Identity;

            float t4 = transfom.M11 * matrix.M11 + transfom.M12 * matrix.M21 + transfom.M13 * matrix.M31;
            float t9 = transfom.M11 * matrix.M12 + transfom.M12 * matrix.M22 + transfom.M13 * matrix.M32;
            float t14 = transfom.M11 * matrix.M13 + transfom.M12 * matrix.M23 + transfom.M13 * matrix.M33;
            float t28 = transfom.M21 * matrix.M11 + transfom.M22 * matrix.M21 + transfom.M23 * matrix.M31;
            float t33 = transfom.M21 * matrix.M12 + transfom.M22 * matrix.M22 + transfom.M23 * matrix.M32;
            float t38 = transfom.M21 * matrix.M13 + transfom.M22 * matrix.M23 + transfom.M23 * matrix.M33;
            float t52 = transfom.M31 * matrix.M11 + transfom.M32 * matrix.M21 + transfom.M33 * matrix.M31;
            float t57 = transfom.M31 * matrix.M12 + transfom.M32 * matrix.M22 + transfom.M33 * matrix.M32;
            float t62 = transfom.M31 * matrix.M13 + transfom.M32 * matrix.M23 + transfom.M33 * matrix.M33;

            tranformedMatrix.M11 = t4 * transfom.M11 + t9 * transfom.M12 + t14 * transfom.M13;
            tranformedMatrix.M12 = t4 * transfom.M21 + t9 * transfom.M22 + t14 * transfom.M23;
            tranformedMatrix.M13 = t4 * transfom.M31 + t9 * transfom.M32 + t14 * transfom.M33;
            tranformedMatrix.M21 = t28 * transfom.M11 + t33 * transfom.M12 + t38 * transfom.M13;
            tranformedMatrix.M22 = t28 * transfom.M21 + t33 * transfom.M22 + t38 * transfom.M23;
            tranformedMatrix.M23 = t28 * transfom.M31 + t33 * transfom.M32 + t38 * transfom.M33;
            tranformedMatrix.M31 = t52 * transfom.M11 + t57 * transfom.M12 + t62 * transfom.M13;
            tranformedMatrix.M32 = t52 * transfom.M21 + t57 * transfom.M22 + t62 * transfom.M23;
            tranformedMatrix.M33 = t52 * transfom.M31 + t57 * transfom.M32 + t62 * transfom.M33;

            return tranformedMatrix;
        }
        /// <summary>
        /// Transforma la matriz 3x3 especificada usando la matriz 4x4
        /// </summary>
        /// <param name="matrix">Matriz 3x3</param>
        /// <param name="transfom">Matriz 4x4</param>
        /// <param name="tranformedMatrix">Devuelve la matriz 3x3 transformada</param>
        public static void Transform(ref Matrix3 matrix, ref Matrix transfom, out Matrix3 tranformedMatrix)
        {
            tranformedMatrix = Transform(matrix, transfom);;

            float t4 = transfom.M11 * matrix.M11 + transfom.M12 * matrix.M21 + transfom.M13 * matrix.M31;
            float t9 = transfom.M11 * matrix.M12 + transfom.M12 * matrix.M22 + transfom.M13 * matrix.M32;
            float t14 = transfom.M11 * matrix.M13 + transfom.M12 * matrix.M23 + transfom.M13 * matrix.M33;
            float t28 = transfom.M21 * matrix.M11 + transfom.M22 * matrix.M21 + transfom.M23 * matrix.M31;
            float t33 = transfom.M21 * matrix.M12 + transfom.M22 * matrix.M22 + transfom.M23 * matrix.M32;
            float t38 = transfom.M21 * matrix.M13 + transfom.M22 * matrix.M23 + transfom.M23 * matrix.M33;
            float t52 = transfom.M31 * matrix.M11 + transfom.M32 * matrix.M21 + transfom.M33 * matrix.M31;
            float t57 = transfom.M31 * matrix.M12 + transfom.M32 * matrix.M22 + transfom.M33 * matrix.M32;
            float t62 = transfom.M31 * matrix.M13 + transfom.M32 * matrix.M23 + transfom.M33 * matrix.M33;

            tranformedMatrix.M11 = t4 * transfom.M11 + t9 * transfom.M12 + t14 * transfom.M13;
            tranformedMatrix.M12 = t4 * transfom.M21 + t9 * transfom.M22 + t14 * transfom.M23;
            tranformedMatrix.M13 = t4 * transfom.M31 + t9 * transfom.M32 + t14 * transfom.M33;
            tranformedMatrix.M21 = t28 * transfom.M11 + t33 * transfom.M12 + t38 * transfom.M13;
            tranformedMatrix.M22 = t28 * transfom.M21 + t33 * transfom.M22 + t38 * transfom.M23;
            tranformedMatrix.M23 = t28 * transfom.M31 + t33 * transfom.M32 + t38 * transfom.M33;
            tranformedMatrix.M31 = t52 * transfom.M11 + t57 * transfom.M12 + t62 * transfom.M13;
            tranformedMatrix.M32 = t52 * transfom.M21 + t57 * transfom.M22 + t62 * transfom.M23;
            tranformedMatrix.M33 = t52 * transfom.M31 + t57 * transfom.M32 + t62 * transfom.M33;
        }
        /// <summary>
        /// Transforma el vector al espacio que representa la traspuesta de la matriz
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="vector">Vector</param>
        /// <returns>Devuelve el vector en el espacio representado por la traspuesta de la matriz</returns>
        public static Vector3 TransformTranspose(Matrix3 matrix, Vector3 vector)
        {
            Vector3 result;
            TransformTranspose(ref matrix, ref vector, out result);
            return result;
        }
        /// <summary>
        /// Transforma el vector al espacio que representa la traspuesta de la matriz
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="vector">Vector</param>
        /// <param name="result">Vector en el espacio representado por la traspuesta de la matriz</param>
        public static void TransformTranspose(ref Matrix3 matrix, ref Vector3 vector, out Vector3 result)
        {
            result.X = vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31;
            result.Y = vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32;
            result.Z = vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33;
        }
        /// <summary>
        /// Obtiene la matriz simétrica basada en el vector especificado
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Devuelve la matriz simétrica basada en el vector especificado</returns>
        /// <remarks>La matriz simétrica es el equivalente al producto vectorial del vector especificado, tal que a x b = A_s b, donde a y b son vectores y A_s es la forma simétrica de a</remarks>
        public static Matrix3 SkewSymmetric(Vector3 vector)
        {
            Matrix3 result;
            SkewSymmetric(ref vector, out result);
            return result;
        }
        /// <summary>
        /// Obtiene la matriz simétrica basada en el vector especificado
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="result">Matriz simétrica basada en el vector especificado</param>
        /// <remarks>La matriz simétrica es el equivalente al producto vectorial del vector especificado, tal que a x b = A_s b, donde a y b son vectores y A_s es la forma simétrica de a</remarks>
        public static void SkewSymmetric(ref Vector3 vector, out Matrix3 result)
        {
            result.M11 = result.M22 = result.M33 = 0f;
            result.M12 = -vector.Z;
            result.M13 = vector.Y;
            result.M21 = vector.Z;
            result.M23 = -vector.X;
            result.M31 = -vector.Y;
            result.M32 = vector.X;
        }

        /// <summary>
        /// Calcula la operación de suma entre dos matrices
        /// </summary>
        /// <param name="one">Matriz</param>
        /// <param name="two">Matriz</param>
        /// <returns>Devuelve la matriz resultante de sumar las matrices especificadas</returns>
        public static Matrix3 operator +(Matrix3 one, Matrix3 two)
        {
            Matrix3 result = new Matrix3();

            result.M11 = one.M11 + two.M11;
            result.M12 = one.M12 + two.M12;
            result.M13 = one.M13 + two.M13;

            result.M21 = one.M21 + two.M21;
            result.M22 = one.M22 + two.M22;
            result.M23 = one.M23 + two.M23;

            result.M31 = one.M31 + two.M31;
            result.M32 = one.M32 + two.M32;
            result.M33 = one.M33 + two.M33;

            return result;
        }
        /// <summary>
        /// Calcula la operación de multiplicación entre una matriz y un escalar
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="scalar">Escalar</param>
        /// <returns>Devuelve la matriz resultante de multiplicar una matriz por un escalar</returns>
        public static Matrix3 operator *(Matrix3 matrix, float scalar)
        {
            Matrix3 result = new Matrix3();

            result.M11 = matrix.M11 * scalar;
            result.M12 = matrix.M12 * scalar;
            result.M13 = matrix.M13 * scalar;

            result.M21 = matrix.M21 * scalar;
            result.M22 = matrix.M22 * scalar;
            result.M23 = matrix.M23 * scalar;

            result.M31 = matrix.M31 * scalar;
            result.M32 = matrix.M32 * scalar;
            result.M33 = matrix.M33 * scalar;

            return result;
        }
        /// <summary>
        /// Calcula la operación de multiplicación entre una matriz y un escalar
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="scalar">Escalar</param>
        /// <returns>Devuelve la matriz resultante de multiplicar una matriz por un escalar</returns>
        public static Matrix3 operator *(float scalar, Matrix3 matrix)
        {
            return matrix * scalar;
        }
        /// <summary>
        /// Calcula la operación de multiplicación entre una matriz y un vector
        /// </summary>
        /// <param name="matrix">Matriz</param>
        /// <param name="vector">Vector</param>
        /// <returns>Devuelve el vector resultante de la multiplicación entre una matriz y un vector</returns>
        public static Vector3 operator *(Matrix3 matrix, Vector3 vector)
        {
            return new Vector3(
                vector.X * matrix.M11 + vector.Y * matrix.M12 + vector.Z * matrix.M13,
                vector.X * matrix.M21 + vector.Y * matrix.M22 + vector.Z * matrix.M23,
                vector.X * matrix.M31 + vector.Y * matrix.M32 + vector.Z * matrix.M33);
        }
        /// <summary>
        /// Calcula la operación de multiplicación entre una matriz y un vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="matrix">Matriz</param>
        /// <returns>Devuelve el vector resultante de la multiplicación entre una matriz y un vector</returns>
        public static Vector3 operator *(Vector3 vector, Matrix3 matrix)
        {
            return matrix * vector;
        }
        /// <summary>
        /// Calcula la operación de multiplicación entre dos matrices
        /// </summary>
        /// <param name="one">Matriz</param>
        /// <param name="two">Matriz</param>
        /// <returns>Devuelve la matriz resultante de multiplicar las matrices especificadas</returns>
        public static Matrix3 operator *(Matrix3 one, Matrix3 two)
        {
            return new Matrix3(
                one.M11 * two.M11 + one.M12 * two.M21 + one.M13 * two.M31,
                one.M11 * two.M12 + one.M12 * two.M22 + one.M13 * two.M32,
                one.M11 * two.M13 + one.M12 * two.M23 + one.M13 * two.M33,

                one.M21 * two.M11 + one.M22 * two.M21 + one.M23 * two.M31,
                one.M21 * two.M12 + one.M22 * two.M22 + one.M23 * two.M32,
                one.M21 * two.M13 + one.M22 * two.M23 + one.M23 * two.M33,

                one.M31 * two.M11 + one.M32 * two.M21 + one.M33 * two.M31,
                one.M31 * two.M12 + one.M32 * two.M22 + one.M33 * two.M32,
                one.M31 * two.M13 + one.M32 * two.M23 + one.M33 * two.M33
                );
        }
    }
}