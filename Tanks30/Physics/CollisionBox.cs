using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Caja
    /// </summary>
    public class CollisionBox : CollisionPrimitive
    {
        /// <summary>
        /// Conjunto de vectores para calcular la posición de los vértices de una caja.
        /// </summary>
        private static readonly float[,] _Mults = new float[8, 3] { { 1, 1, 1 }, { -1, 1, 1 }, { 1, -1, 1 }, { -1, -1, 1 }, { 1, 1, -1 }, { -1, 1, -1 }, { 1, -1, -1 }, { -1, -1, -1 } };
    
        /// <summary>
        /// Crea una caja a partir de una caja alineada con los ejes de coordenadas
        /// </summary>
        /// <param name="aabb">Caja alineada con los ejes de coordenadas</param>
        /// <returns>Devuelve la caja creada</returns>
        public static CollisionBox CreateFromBoundingBox(BoundingBox aabb)
        {
            return new CollisionBox()
            {
                HalfSize = (aabb.Max - aabb.Min) * 0.5f,
            };
        }

        /// <summary>
        /// Distancias a cada cara desde el centro de la caja a lo largo de los tres ejes locales.
        /// </summary>
        public Vector3 HalfSize;

        /// <summary>
        /// Obtiene la esquina especificada en coordenadas del mundo
        /// </summary>
        /// <param name="index">Índice de esquina</param>
        /// <returns>Devuelve la posición de la esquina en coordenadas del mundo</returns>
        public Vector3 GetCorner(int index)
        {
            Vector3 result = new Vector3(CollisionBox._Mults[index, 0], CollisionBox._Mults[index, 1], CollisionBox._Mults[index, 2]);
            result = Core.ComponentProductUpdate(result, this.HalfSize);
            result = Vector3.Transform(result, this.Transform);

            return result;
        }

    }
}