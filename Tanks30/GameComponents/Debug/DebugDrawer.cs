using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Debug
{
    using Common;
    using Physics;

    public abstract class DebugDrawer
    {
        static BasicEffect Effect;

        static VertexDeclaration VertexDeclaration;

        static VertexPositionColor[] m_DebugBoxVertexes = new VertexPositionColor[24];

        public static void Initialize(GraphicsDevice device)
        {
            Effect = new BasicEffect(device, null);
            VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
        }

        public static void DrawLines(GraphicsDevice device, VertexPositionColor[] points)
        {
            device.VertexDeclaration = VertexDeclaration;

            DebugDrawer.Effect.VertexColorEnabled = true;
            DebugDrawer.Effect.LightingEnabled = true;
            DebugDrawer.Effect.PreferPerPixelLighting = true;
            DebugDrawer.Effect.TextureEnabled = false;

            DebugDrawer.Effect.World = GlobalMatrices.gWorldMatrix;
            DebugDrawer.Effect.View = GlobalMatrices.gViewMatrix;
            DebugDrawer.Effect.Projection = GlobalMatrices.gProjectionMatrix;

            DebugDrawer.Effect.Begin();

            foreach (EffectPass pass in DebugDrawer.Effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                device.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    points,
                    0,
                    points.Length / 2);

                pass.End();
            }

            DebugDrawer.Effect.End();

            device.VertexDeclaration = null;
        }

        public static void DrawDebugAABB(GraphicsDevice device, BoundingBox aabb)
        {
            Vector3[] corners = new Vector3[8];
            corners[0] = aabb.Min;

            corners[1].X = aabb.Max.X;
            corners[1].Y = aabb.Min.Y;
            corners[1].Z = aabb.Min.Z;

            corners[2].X = aabb.Min.X;
            corners[2].Y = aabb.Max.Y;
            corners[2].Z = aabb.Min.Z;

            corners[3].X = aabb.Max.X;
            corners[3].Y = aabb.Max.Y;
            corners[3].Z = aabb.Min.Z;

            corners[4].X = aabb.Min.X;
            corners[4].Y = aabb.Min.Y;
            corners[4].Z = aabb.Max.Z;

            corners[5].X = aabb.Max.X;
            corners[5].Y = aabb.Min.Y;
            corners[5].Z = aabb.Max.Z;

            corners[6].X = aabb.Min.X;
            corners[6].Y = aabb.Max.Y;
            corners[6].Z = aabb.Max.Z;

            corners[7] = aabb.Max;

            m_DebugBoxVertexes[0].Position = corners[0];
            m_DebugBoxVertexes[0].Color = Color.Yellow;
            m_DebugBoxVertexes[1].Position = corners[1];
            m_DebugBoxVertexes[1].Color = Color.Yellow;

            m_DebugBoxVertexes[2].Position = corners[1];
            m_DebugBoxVertexes[2].Color = Color.Yellow;
            m_DebugBoxVertexes[3].Position = corners[3];
            m_DebugBoxVertexes[3].Color = Color.Yellow;

            m_DebugBoxVertexes[4].Position = corners[0];
            m_DebugBoxVertexes[4].Color = Color.Yellow;
            m_DebugBoxVertexes[5].Position = corners[2];
            m_DebugBoxVertexes[5].Color = Color.Yellow;

            m_DebugBoxVertexes[6].Position = corners[2];
            m_DebugBoxVertexes[6].Color = Color.Yellow;
            m_DebugBoxVertexes[7].Position = corners[3];
            m_DebugBoxVertexes[7].Color = Color.Yellow;

            m_DebugBoxVertexes[8].Position = corners[4];
            m_DebugBoxVertexes[8].Color = Color.Yellow;
            m_DebugBoxVertexes[9].Position = corners[5];
            m_DebugBoxVertexes[9].Color = Color.Yellow;

            m_DebugBoxVertexes[10].Position = corners[4];
            m_DebugBoxVertexes[10].Color = Color.Yellow;
            m_DebugBoxVertexes[11].Position = corners[6];
            m_DebugBoxVertexes[11].Color = Color.Yellow;

            m_DebugBoxVertexes[12].Position = corners[5];
            m_DebugBoxVertexes[12].Color = Color.Yellow;
            m_DebugBoxVertexes[13].Position = corners[7];
            m_DebugBoxVertexes[13].Color = Color.Yellow;

            m_DebugBoxVertexes[14].Position = corners[6];
            m_DebugBoxVertexes[14].Color = Color.Yellow;
            m_DebugBoxVertexes[15].Position = corners[7];
            m_DebugBoxVertexes[15].Color = Color.Yellow;

            m_DebugBoxVertexes[16].Position = corners[0];
            m_DebugBoxVertexes[16].Color = Color.Yellow;
            m_DebugBoxVertexes[17].Position = corners[4];
            m_DebugBoxVertexes[17].Color = Color.Yellow;

            m_DebugBoxVertexes[18].Position = corners[1];
            m_DebugBoxVertexes[18].Color = Color.Yellow;
            m_DebugBoxVertexes[19].Position = corners[5];
            m_DebugBoxVertexes[19].Color = Color.Yellow;

            m_DebugBoxVertexes[20].Position = corners[2];
            m_DebugBoxVertexes[20].Color = Color.Yellow;
            m_DebugBoxVertexes[21].Position = corners[6];
            m_DebugBoxVertexes[21].Color = Color.Yellow;

            m_DebugBoxVertexes[22].Position = corners[3];
            m_DebugBoxVertexes[22].Color = Color.Yellow;
            m_DebugBoxVertexes[23].Position = corners[7];
            m_DebugBoxVertexes[23].Color = Color.Yellow;

            Debug.DebugDrawer.DrawLines(device, m_DebugBoxVertexes);
        }

        public static void DrawDebugOBB(GraphicsDevice device, CollisionBox box)
        {
            if (box != null)
            {
                Vector3[] corners = box.GetCorners();

                m_DebugBoxVertexes[0].Position = corners[0];
                m_DebugBoxVertexes[0].Color = Color.Yellow;
                m_DebugBoxVertexes[1].Position = corners[1];
                m_DebugBoxVertexes[1].Color = Color.Yellow;

                m_DebugBoxVertexes[2].Position = corners[1];
                m_DebugBoxVertexes[2].Color = Color.Yellow;
                m_DebugBoxVertexes[3].Position = corners[3];
                m_DebugBoxVertexes[3].Color = Color.Yellow;

                m_DebugBoxVertexes[4].Position = corners[0];
                m_DebugBoxVertexes[4].Color = Color.Yellow;
                m_DebugBoxVertexes[5].Position = corners[2];
                m_DebugBoxVertexes[5].Color = Color.Yellow;

                m_DebugBoxVertexes[6].Position = corners[2];
                m_DebugBoxVertexes[6].Color = Color.Yellow;
                m_DebugBoxVertexes[7].Position = corners[3];
                m_DebugBoxVertexes[7].Color = Color.Yellow;

                m_DebugBoxVertexes[8].Position = corners[4];
                m_DebugBoxVertexes[8].Color = Color.Yellow;
                m_DebugBoxVertexes[9].Position = corners[5];
                m_DebugBoxVertexes[9].Color = Color.Yellow;

                m_DebugBoxVertexes[10].Position = corners[4];
                m_DebugBoxVertexes[10].Color = Color.Yellow;
                m_DebugBoxVertexes[11].Position = corners[6];
                m_DebugBoxVertexes[11].Color = Color.Yellow;

                m_DebugBoxVertexes[12].Position = corners[5];
                m_DebugBoxVertexes[12].Color = Color.Yellow;
                m_DebugBoxVertexes[13].Position = corners[7];
                m_DebugBoxVertexes[13].Color = Color.Yellow;

                m_DebugBoxVertexes[14].Position = corners[6];
                m_DebugBoxVertexes[14].Color = Color.Yellow;
                m_DebugBoxVertexes[15].Position = corners[7];
                m_DebugBoxVertexes[15].Color = Color.Yellow;

                m_DebugBoxVertexes[16].Position = corners[0];
                m_DebugBoxVertexes[16].Color = Color.Yellow;
                m_DebugBoxVertexes[17].Position = corners[4];
                m_DebugBoxVertexes[17].Color = Color.Yellow;

                m_DebugBoxVertexes[18].Position = corners[1];
                m_DebugBoxVertexes[18].Color = Color.Yellow;
                m_DebugBoxVertexes[19].Position = corners[5];
                m_DebugBoxVertexes[19].Color = Color.Yellow;

                m_DebugBoxVertexes[20].Position = corners[2];
                m_DebugBoxVertexes[20].Color = Color.Yellow;
                m_DebugBoxVertexes[21].Position = corners[6];
                m_DebugBoxVertexes[21].Color = Color.Yellow;

                m_DebugBoxVertexes[22].Position = corners[3];
                m_DebugBoxVertexes[22].Color = Color.Yellow;
                m_DebugBoxVertexes[23].Position = corners[7];
                m_DebugBoxVertexes[23].Color = Color.Yellow;

                Debug.DebugDrawer.DrawLines(device, m_DebugBoxVertexes);
            }
        }
    }
}
