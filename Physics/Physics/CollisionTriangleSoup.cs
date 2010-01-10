
namespace Physics
{
    /// <summary>
    /// Primitiva de colisi�n que representa una lista de tri�ngulos
    /// </summary>
    public class CollisionTriangleSoup : CollisionPrimitive
    {
        /// <summary>
        /// Lista de tri�ngulos
        /// </summary>
        private Triangle[] m_Triangles;
        /// <summary>
        /// Obtiene la lista de tri�ngulos
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
        /// <param name="triangles">Lista de tri�ngulos</param>
        /// <param name="mass">Masa</param>
        public CollisionTriangleSoup(Triangle[] triangles, float mass)
            : base(mass)
        {
            this.m_Triangles = triangles;
        }
    }
}
