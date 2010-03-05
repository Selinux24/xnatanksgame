using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Scenery
{
    public partial class Scenery
    {
#if DEBUG
        public void DrawDebug(GraphicsDevice device, GameTime gameTime)
        {
            // Dibujar el AABB
            Debug.DebugDrawer.DrawDebugAABB(device, this.Root.NorthEast.AABB);
            Debug.DebugDrawer.DrawDebugAABB(device, this.Root.NorthWest.AABB);
            Debug.DebugDrawer.DrawDebugAABB(device, this.Root.SouthEast.AABB);
            Debug.DebugDrawer.DrawDebugAABB(device, this.Root.SouthWest.AABB);
        }
#endif
    }
}
