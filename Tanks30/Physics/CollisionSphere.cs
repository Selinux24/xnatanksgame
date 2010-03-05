using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Cuerpo rígido que puede ser tratado como una esfera para colisión
    /// </summary>
    public class CollisionSphere : CollisionPrimitive
    {
        /// <summary>
        /// Radio de la esfera
        /// </summary>
        public float Radius;
        /// <summary>
        /// Obtiene el AABB de la esfera
        /// </summary>
        public override BoundingBox AABB 
        {
            get
            {
                return BoundingBox.CreateFromSphere(this.SPH);
            }
            protected set
            {

            }
        }
        /// <summary>
        /// Obtiene el SPH de la esfera
        /// </summary>
        public override BoundingSphere SPH
        {
            get
            {
                return new BoundingSphere(this.Position, this.Radius);
            }
            protected set
            {

            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="radius">Radio</param>
        /// <param name="mass">Masa</param>
        public CollisionSphere(float radius, float mass)
            : base(mass)
        {
            this.Radius = radius;
        }
    }
}