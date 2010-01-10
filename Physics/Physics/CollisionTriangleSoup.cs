
namespace Physics
{
    /// <summary>
    /// Primitiva de colisión que representa una lista de triángulos
    /// </summary>
    public class CollisionTriangleSoup : CollisionPrimitive
    {
        /// <summary>
        /// Lista de triángulos
        /// </summary>
        private Triangle[] m_Triangles;
        /// <summary>
        /// Obtiene la lista de triángulos
        /// </summary>
        public Triangle[] Triangles
        {
            get
            {
                return this.m_Triangles;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="triangles">Lista de triángulos</param>
        /// <param name="mass">Masa</param>
        public CollisionTriangleSoup(Triangle[] triangles, float mass)
            : base(mass)
        {
            this.m_Triangles = triangles;
        }
    }
}
