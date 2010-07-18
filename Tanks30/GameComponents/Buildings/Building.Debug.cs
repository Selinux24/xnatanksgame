using Microsoft.Xna.Framework;

namespace GameComponents.Buildings
{
    public partial class Building
    {
#if DEBUG
        private void DrawDebug(GameTime gameTime)
        {
            // Dibujar el OBB
            Debug.DebugDrawer.DrawDebugOBB(this.GraphicsDevice, this.m_OBB);
            // Dibujar el AABB
            Debug.DebugDrawer.DrawDebugAABB(this.GraphicsDevice, this.GetAABB());
        }
#endif
    }
}
