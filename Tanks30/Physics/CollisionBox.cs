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
        public Vector3 HalfSize;
        /// <summary>
        /// Esfera circundante
        /// </summary>
        private BoundingSphere m_SPH;
        /// <summary>
        /// Obtiene el AABB de la caja
        /// </summary>
        public override BoundingBox AABB
        {
            get
            {
                return BoundingBox.CreateFromPoints(this.GetCorners());
            }
            protected set
            {

            }
        }
        /// <summary>
        /// Obtiene la esfera circundate del OBB
        /// </summary>
        public override BoundingSphere SPH
        {
            get
            {
                this.m_SPH.Center = this.Position;

                return this.m_SPH;
            }
            protected set
            {

            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aabb">AABB</param>
        /// <param name="mass">Masa</param>
        public CollisionBox(BoundingBox aabb, float mass)
            : this(aabb.Max, aabb.Min, mass)
        {
            
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="max">Punto máximo de un AABB</param>
        /// <param name="min">Punto mínimo de un AABB</param>
        /// <param name="mass">Masa</param>
        public CollisionBox(Vector3 max, Vector3 min, float mass)
            : base(mass)
        {
            this.HalfSize = (max - min) * 0.5f;

            this.m_SPH = BoundingSphere.CreateFromPoints(this.GetCorners());
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="halfSize">Medias longitudes en los ejes de coordenadas</param>
        /// <param name="mass">Masa</param>
        public CollisionBox(Vector3 halfSize, float mass)
            : base(mass)
        {
            this.HalfSize = halfSize;

            this.m_SPH = BoundingSphere.CreateFromPoints(this.GetCorners());
        }

        /// <summary>
        /// Obtiene la esquina especificada en coordenadas del mundo
        /// </summary>
        /// <param name="index">Índice de esquina</param>
        /// <returns>Devuelve la posición de la esquina en coordenadas del mundo</returns>
        public Vector3 GetCorner(int index)
        {
            Vector3 result = new Vector3(CollisionBox._Mults[index, 0], CollisionBox._Mults[index, 1], CollisionBox._Mults[index, 2]);
            result = result.ComponentProduct(this.HalfSize);
            result = Vector3.Transform(result, this.Transform);

            return result;
        }
        /// <summary>
        /// Obtiene las esquinas del OBB
        /// </summary>
        /// <returns>Devuelve una lista de vectores con las esquinas del OBB</returns>
        public Vector3[] GetCorners()
        {
            Vector3[] corners = new Vector3[8];

            for (int i = 0; i < 8; i++)
            {
                corners[i] = this.GetCorner(i);
            }

            return corners;
        }

        /// <summary>
        /// Establece el estado inicial de la caja en la posición y orientación indicadas
        /// </summary>
        /// <param name="position">Posición inicial</param>
        /// <param name="orientation">Orientación inicial</param>
        public override void SetInitialState(Vector3 position, Quaternion orientation)
        {
            base.SetInitialState(position, orientation);

            float mass = this.Mass;
            Vector3 squares = this.HalfSize.ComponentProduct(this.HalfSize);
            this.InertiaTensor = Matrix3.CreateFromInertiaTensorCoeffs(
                0.3f * mass * (squares.Y + squares.Z),
                0.3f * mass * (squares.X + squares.Z),
                0.3f * mass * (squares.X + squares.Y));
        }
    }
}