using Microsoft.Xna.Framework;

namespace GameComponents.Buildings
{
    using Physics;

    public partial class Building
    {
#if DEBUG
        private void DrawDebug(GameTime gameTime)
        {
            // Dibujar el OBB
            Debug.DebugDrawer.DrawDebugOBB(this.GraphicsDevice, this.m_CollisionPrimitive as CollisionBox);
            // Dibujar el AABB
            Debug.DebugDrawer.DrawDebugAABB(this.GraphicsDevice, this.AABB);
        }
#endif
    }
}
