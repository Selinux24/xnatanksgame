using System;
using Microsoft.Xna.Framework;

namespace Common.Primitives
{
    /// <summary>
    /// Un tri�ngulo
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// Puntos del tri�ngulo
        /// </summary>
        public readonly Vector3 Point1, Point2, Point3;
        /// <summary>
        /// Centro del tri�ngulo
        /// </summary>
        public readonly Vector3 Center;
        /// <summary>
        /// Indices
        /// </summary>
        public readonly int I1, I2;
        /// <summary>
        /// Plano en el que est� contenido el tri�ngulo
        /// </summary>
        public readonly Plane Plane;
        /// <summary>
        /// Obtiene la normal del plano que contiene el tri�ngulo
        /// </summary>
        public Vector3 Normal
        {
            get
            {
                return this.Plane.Normal;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="point1">Punto 1</param>
        /// <param name="point2">Punto 2</param>
        /// <param name="point3">Punto 3</param>
        public Triangle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            this.Point1 = point1;
            this.Point2 = point2;
            this.Point3 = point3;
            this.Center = Vector3.Multiply(point1 + point2 + point3, 1.0f / 3.0f);
            this.Plane = new Plane(this.Point2, this.Point1, this.Point3);

            Vector3 n = this.Plane.Normal;
            float absX = (float)System.Math.Abs(n.X);
            float absY = (float)System.Math.Abs(n.Y);
            float absZ = (float)System.Math.Abs(n.Z);

            Vector3 a = new Vector3(absX, absY, absZ);
            if (a.X > a.Y)
            {
                if (a.X > a.Z)
                {
                    this.I1 = 1;
                    this.I2 = 2;
                }
                else
                {
                    this.I1 = 0;
                    this.I2 = 1;
                }
            }
            else
            {
                if (a.Y > a.Z)
                {
                    this.I1 = 0;
                    this.I2 = 2;
                }
                else
                {
                    this.I1 = 0;
                    this.I2 = 1;
                }
            }
        }

        /// <summary>
        /// Obtiene si el punto especificado est� contenido en el tri�ngulo
        /// </summary>
        /// <param name="point">Punto</param>
        /// <returns>Devuelve verdadero si el punto est� contenido en el tri�ngulo, o falso en el resto de los casos</returns>
        public static bool PointInTriangle(Triangle tri, Vector3 point)
        {
            Vector3 u = new Vector3(
                Triangle.PointFromVector(point, tri.I1) - Triangle.PointFromVector(tri.Point1, tri.I1),
                Triangle.PointFromVector(tri.Point2, tri.I1) - Triangle.PointFromVector(tri.Point1, tri.I1),
                Triangle.PointFromVector(tri.Point3, tri.I1) - Triangle.PointFromVector(tri.Point1, tri.I1));

            Vector3 v = new Vector3(
                Triangle.PointFromVector(point, tri.I2) - Triangle.PointFromVector(tri.Point1, tri.I2),
                Triangle.PointFromVector(tri.Point2, tri.I2) - Triangle.PointFromVector(tri.Point1, tri.I2),
                Triangle.PointFromVector(tri.Point3, tri.I2) - Triangle.PointFromVector(tri.Point1, tri.I2));

            float a, b;
            if (u.Y == 0.0f)
            {
                b = u.X / u.Z;
                if (b >= 0.0f && b <= 1.0f)
                {
                    a = (v.X - b * v.Z) / v.Y;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                b = (v.X * u.Y - u.X * v.Y) / (v.Z * u.Y - u.Z * v.Y);
                if (b >= 0.0f && b <= 1.0f)
                {
                    a = (u.X - b * u.Z) / u.Y;
                }
                else
                {
                    return false;
                }
            }

            return (a >= 0 && (a + b) <= 1);
        }
        /// <summary>
        /// Obtiene el valor de la componente del vector especificado por �ndice
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="index">Indice 0, 1 o 2 para obtener las componentes x, y o z</param>
        /// <returns>Devuelve la componente del vector especificada por el �ndice</returns>
        public static float PointFromVector(Vector3 vector, int index)
        {
            if (index == 0)
            {
                return vector.X;
            }
            else if (index == 1)
            {
                return vector.Y;
            }
            else if (index == 2)
            {
                return vector.Z;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Obtiene el punto m�s cercano al punto especificado en el tri�ngulo
        /// </summary>
        /// <param name="tri">Tri�ngulo</param>
        /// <param name="point">Punto</param>
        /// <returns>Devuelve el punto m�s cercano al punto especificado en el tri�ngulo</returns>
        public static Vector3 ClosestPointInTriangle(Triangle tri, Vector3 point)
        {
            Vector3 diff = tri.Point1 - point;
            Vector3 edge1 = tri.Point2 - tri.Point1;
            Vector3 edge2 = tri.Point3 - tri.Point1;
            
            float diffLength = diff.LengthSquared();
            float edge1Length = edge1.LengthSquared();
            float edge2Length = edge2.LengthSquared();

            float edgesProj = Vector3.Dot(edge1, edge2);
            float edge1Proj = Vector3.Dot(diff, edge1);
            float edge2Proj = Vector3.Dot(diff, edge2);

            float determinant = System.Math.Abs(edge1Length * edge2Length - edgesProj * edgesProj);
            float s = edgesProj * edge2Proj - edge2Length * edge1Proj;
            float t = edgesProj * edge1Proj - edge1Length * edge2Proj;

            if (s + t <= determinant)
            {
                if (s < 0.0f)
                {
                    if (t < 0.0f)
                    {
                        //Regi�n 4
                        if (edge1Proj < 0.0f)
                        {
                            t = 0.0f;
                            if (-edge1Proj >= edge1Length)
                            {
                                s = 1.0f;
                            }
                            else
                            {
                                s = -edge1Proj / edge1Length;
                            }
                        }
                        else
                        {
                            s = 0.0f;
                            if (edge2Proj >= 0.0f)
                            {
                                t = 0.0f;
                            }
                            else if (-edge2Proj >= edge2Length)
                            {
                                t = 1.0f;
                            }
                            else
                            {
                                t = -edge2Proj / edge2Length;
                            }
                        }
                    }
                    else  
                    {
                        //Regi�n 3
                        s = 0.0f;
                        if (edge2Proj >= 0.0f)
                        {
                            t = 0.0f;
                        }
                        else if (-edge2Proj >= edge2Length)
                        {
                            t = 0.0f;
                        }
                        else
                        {
                            t = -edge2Proj / edge2Length;
                        }
                    }
                }
                else if (t < 0.0f)
                {
                    //Regi�n 5
                    t = 0.0f;
                    if (edge1Proj >= 0.0f)
                    {
                        s = 0.0f;
                    }
                    else if (-edge1Proj >= edge1Length)
                    {
                        s = 1.0f;
                    }
                    else
                    {
                        s = -edge1Proj / edge1Length;
                    }
                }
                else
                {
                    //Regi�n 0
                    
                    //M�nimo punto interior
                    float fInvDet = 1.0f / determinant;
                    s *= fInvDet;
                    t *= fInvDet;
                }
            }
            else
            {
                if (s < 0.0f)  
                {
                    //Regi�n 2
                    float tmp0 = edgesProj + edge1Proj;
                    float tmp1 = edge2Length + edge2Proj;
                    if (tmp1 > tmp0)
                    {
                        float numer = tmp1 - tmp0;
                        float denom = edge1Length - 2.0f * edgesProj + edge2Length;
                        if (numer >= denom)
                        {
                            s = 1.0f;
                            t = 0.0f;
                        }
                        else
                        {
                            s = numer / denom;
                            t = 1.0f - s;
                        }
                    }
                    else
                    {
                        s = 0.0f;
                        if (tmp1 <= 0.0f)
                        {
                            t = 1.0f;
                        }
                        else if (edge2Proj >= 0.0f)
                        {
                            t = 0.0f;
                        }
                        else
                        {
                            t = -edge2Proj / edge2Length;
                        }
                    }
                }
                else if (t < 0.0f)
                {
                    //Regi�n 6
                    float tmp0 = edgesProj + edge2Proj;
                    float tmp1 = edge1Length + edge1Proj;
                    if (tmp1 > tmp0)
                    {
                        float numer = tmp1 - tmp0;
                        float denom = edge1Length - 2.0f * edgesProj + edge2Length;
                        if (numer >= denom)
                        {
                            t = 1.0f;
                            s = 0.0f;
                        }
                        else
                        {
                            t = numer / denom;
                            s = 1.0f - t;
                        }
                    }
                    else
                    {
                        t = 0.0f;
                        if (tmp1 <= 0.0f)
                        {
                            s = 1.0f;
                        }
                        else if (edge1Proj >= 0.0f)
                        {
                            s = 0.0f;
                        }
                        else
                        {
                            s = -edge1Proj / edge1Length;
                        }
                    }
                }
                else
                {
                    //Regi�n 1
                    float numer = edge2Length + edge2Proj - edgesProj - edge1Proj;
                    if (numer <= 0.0f)
                    {
                        s = 0.0f;
                        t = 1.0f;
                    }
                    else
                    {
                        float denom = edge1Length - 2.0f * edgesProj + edge2Length;
                        if (numer >= denom)
                        {
                            s = 1.0f;
                            t = 0.0f;
                        }
                        else
                        {
                            s = numer / denom;
                            t = 1.0f - s;
                        }
                    }
                }
            }

            return tri.Point1 + s * edge1 + t * edge2;
        }
    }
}
