using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    using Physics;

    public partial class Vehicle
    {
#if DEBUG
        private void DrawDebug(GameTime gameTime)
        {
            // Dibujar el OBB
            Debug.DebugDrawer.DrawDebugOBB(this.GraphicsDevice, this.m_CollisionPrimitive as CollisionBox);
            // Dibujar el AABB
            Debug.DebugDrawer.DrawDebugAABB(this.GraphicsDevice, this.AABB);
            // Dibujar el objeto físico
            Debug.DebugDrawer.DrawDebugPhysicObject(this.GraphicsDevice, this as IPhysicObject);
        }
#endif
    }
}
