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
        /// Distancias a cada cara desde el centro de la caja a lo largo de los tres ejes locales.
        /// </summary>
        private Vector3 m_HalfSize;
        /// <summary>
        /// Obtiene las distancias a cada cara desde el centro de la caja a lo largo de los tres ejes locales.
        /// </summary>
        public Vector3 HalfSize
        {
            get
            {
                return this.m_HalfSize;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="halfSize">Longitud del centro hasta las caras en los tres ejes</param>
        public CollisionBox(Vector3 halfSize)
            : this(halfSize, halfSize.X * halfSize.Y * halfSize.Z * 8.0f)
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="halfSize">Longitud del centro hasta las caras en los tres ejes</param>
        /// <param name="mass">Masa</param>
        public CollisionBox(Vector3 halfSize, float mass)
            : base(mass)
        {
            this.m_HalfSize = halfSize;
        }

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
     
        /// <summary>
        /// Establece el estado inicial de la caja en la posición y orientación indicadas
        /// </summary>
        /// <param name="position">Posición inicial</param>
        /// <param name="orientation">Orientación inicial</param>
        public override void SetState(Vector3 position, Quaternion orientation)
        {
            base.SetState(position, orientation);

            if (this.Body != null)
            {
                float mass = this.Body.Mass;
                Vector3 squares = Core.ComponentProductUpdate(this.HalfSize, this.HalfSize);
                this.Body.InertiaTensor = Core.SetInertiaTensorCoeffs(
                    0.3f * mass * (squares.Y + squares.Z),
                    0.3f * mass * (squares.X + squares.Z),
                    0.3f * mass * (squares.X + squares.Y));
            }
        }
    }
}