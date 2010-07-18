﻿using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    public partial class Vehicle
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
