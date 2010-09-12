using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Debug
{
    using Common;
    using Common.Primitives;
    using Physics;

    public abstract class DebugDrawer
    {
        static BasicEffect Effect;

        static VertexDeclaration VertexDeclaration;

        static VertexPositionColor[] m_DebugBoxVertexes = new VertexPositionColor[24];

        public static void Initialize(GraphicsDevice device)
        {
            VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);

            Effect = new BasicEffect(device, null);
            Effect.VertexColorEnabled = true;
        }

        public static void DrawLines(GraphicsDevice device, VertexPositionColor[] points)
        {
            if (points != null && points.Length > 0)
            {
                DrawLines(device, points, points.Length);
            }
        }

        public static void DrawLines(GraphicsDevice device, VertexPositionColor[] points, int length)
        {
            device.VertexDeclaration = VertexDeclaration;

            bool fogEnable = device.RenderState.FogEnable;
            FillMode fillMode = device.RenderState.FillMode;

            device.RenderState.FogEnable = false;
            device.RenderState.FillMode = FillMode.WireFrame;

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
                    length / 2);

                pass.End();
            }

            DebugDrawer.Effect.End();

            device.VertexDeclaration = null;

            device.RenderState.FogEnable = fogEnable;
            device.RenderState.FillMode = fillMode;
        }

        public static void DrawDebugBoxCorners(GraphicsDevice device, Vector3[] corners, Color color)
        {
            m_DebugBoxVertexes[0].Position = corners[0];
            m_DebugBoxVertexes[0].Color = color;
            m_DebugBoxVertexes[1].Position = corners[1];
            m_DebugBoxVertexes[1].Color = color;

            m_DebugBoxVertexes[2].Position = corners[1];
            m_DebugBoxVertexes[2].Color = color;
            m_DebugBoxVertexes[3].Position = corners[3];
            m_DebugBoxVertexes[3].Color = color;

            m_DebugBoxVertexes[4].Position = corners[0];
            m_DebugBoxVertexes[4].Color = color;
            m_DebugBoxVertexes[5].Position = corners[2];
            m_DebugBoxVertexes[5].Color = color;

            m_DebugBoxVertexes[6].Position = corners[2];
            m_DebugBoxVertexes[6].Color = color;
            m_DebugBoxVertexes[7].Position = corners[3];
            m_DebugBoxVertexes[7].Color = color;

            m_DebugBoxVertexes[8].Position = corners[4];
            m_DebugBoxVertexes[8].Color = color;
            m_DebugBoxVertexes[9].Position = corners[5];
            m_DebugBoxVertexes[9].Color = color;

            m_DebugBoxVertexes[10].Position = corners[4];
            m_DebugBoxVertexes[10].Color = color;
            m_DebugBoxVertexes[11].Position = corners[6];
            m_DebugBoxVertexes[11].Color = color;

            m_DebugBoxVertexes[12].Position = corners[5];
            m_DebugBoxVertexes[12].Color = color;
            m_DebugBoxVertexes[13].Position = corners[7];
            m_DebugBoxVertexes[13].Color = color;

            m_DebugBoxVertexes[14].Position = corners[6];
            m_DebugBoxVertexes[14].Color = color;
            m_DebugBoxVertexes[15].Position = corners[7];
            m_DebugBoxVertexes[15].Color = color;

            m_DebugBoxVertexes[16].Position = corners[0];
            m_DebugBoxVertexes[16].Color = color;
            m_DebugBoxVertexes[17].Position = corners[4];
            m_DebugBoxVertexes[17].Color = color;

            m_DebugBoxVertexes[18].Position = corners[1];
            m_DebugBoxVertexes[18].Color = color;
            m_DebugBoxVertexes[19].Position = corners[5];
            m_DebugBoxVertexes[19].Color = color;

            m_DebugBoxVertexes[20].Position = corners[2];
            m_DebugBoxVertexes[20].Color = color;
            m_DebugBoxVertexes[21].Position = corners[6];
            m_DebugBoxVertexes[21].Color = color;

            m_DebugBoxVertexes[22].Position = corners[3];
            m_DebugBoxVertexes[22].Color = color;
            m_DebugBoxVertexes[23].Position = corners[7];
            m_DebugBoxVertexes[23].Color = color;

            Debug.DebugDrawer.DrawLines(device, m_DebugBoxVertexes, 24);
        }

        public static void DrawDebugTriangle(GraphicsDevice device, Triangle triangle)
        {
            DrawDebugTriangle(device, triangle, Color.Red);
        }
        public static void DrawDebugTriangle(GraphicsDevice device, Triangle triangle, Color color)
        {
            m_DebugBoxVertexes[0].Position = triangle.Point1;
            m_DebugBoxVertexes[0].Color = color;
            m_DebugBoxVertexes[1].Position = triangle.Point2;
            m_DebugBoxVertexes[1].Color = color;

            m_DebugBoxVertexes[2].Position = triangle.Point1;
            m_DebugBoxVertexes[2].Color = color;
            m_DebugBoxVertexes[3].Position = triangle.Point3;
            m_DebugBoxVertexes[3].Color = color;

            m_DebugBoxVertexes[4].Position = triangle.Point2;
            m_DebugBoxVertexes[4].Color = color;
            m_DebugBoxVertexes[5].Position = triangle.Point3;
            m_DebugBoxVertexes[5].Color = color;

            Debug.DebugDrawer.DrawLines(device, m_DebugBoxVertexes, 6);
        }

        public static void DrawDebugAABB(GraphicsDevice device, BoundingBox aabb)
        {
            DrawDebugAABB(device, aabb, Color.Blue);
        }
        public static void DrawDebugAABB(GraphicsDevice device, BoundingBox aabb, Color color)
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

            DrawDebugBoxCorners(device, corners, color);
        }

        public static void DrawDebugOBB(GraphicsDevice device, CollisionBox obb)
        {
            DrawDebugOBB(device, obb, Color.Green);
        }
        public static void DrawDebugOBB(GraphicsDevice device, CollisionBox obb, Color color)
        {
            if (obb != null)
            {
                Vector3[] corners = obb.GetCorners();

                DrawDebugBoxCorners(device, corners, color);
            }
        }

        public static void DrawDebugPhysicObject(GraphicsDevice device, IPhysicObject iPhysicObject)
        {
            //Primitiva
            CollisionPrimitive pr = iPhysicObject.GetPrimitive();

            //Transformación
            Matrix transform = pr.Transform;

            //Posición y orientación
            Vector3 position = transform.Translation;
            Vector3 up = transform.Up + position;
            Vector3 forward = transform.Forward + position;
            Vector3 right = transform.Right + position;

            //Velocidad
            Vector3 velocity = pr.Velocity + position;

            //Aceleración
            Vector3 acceleration = pr.Acceleration + position;

            m_DebugBoxVertexes[0].Position = position;
            m_DebugBoxVertexes[0].Color = Color.Red;
            m_DebugBoxVertexes[1].Position = up;
            m_DebugBoxVertexes[1].Color = Color.Red;

            m_DebugBoxVertexes[2].Position = position;
            m_DebugBoxVertexes[2].Color = Color.Green;
            m_DebugBoxVertexes[3].Position = forward;
            m_DebugBoxVertexes[3].Color = Color.Green;

            m_DebugBoxVertexes[4].Position = position;
            m_DebugBoxVertexes[4].Color = Color.Blue;
            m_DebugBoxVertexes[5].Position = right;
            m_DebugBoxVertexes[5].Color = Color.Blue;

            m_DebugBoxVertexes[6].Position = position;
            m_DebugBoxVertexes[6].Color = Color.White;
            m_DebugBoxVertexes[7].Position = velocity;
            m_DebugBoxVertexes[7].Color = Color.White;

            m_DebugBoxVertexes[8].Position = position;
            m_DebugBoxVertexes[8].Color = Color.Cyan;
            m_DebugBoxVertexes[9].Position = acceleration;
            m_DebugBoxVertexes[9].Color = Color.Cyan;

            Debug.DebugDrawer.DrawLines(device, m_DebugBoxVertexes, 10);
        }

        public static void DrawDebugEdge(GraphicsDevice device, Vector3 edge)
        {
            DrawDebugEdge(device, edge, Color.Cyan);
        }
        public static void DrawDebugEdge(GraphicsDevice device, Vector3 edge, Color color)
        {
            m_DebugBoxVertexes[0].Position = edge;
            m_DebugBoxVertexes[0].Color = color;
            m_DebugBoxVertexes[1].Position = Vector3.Zero;
            m_DebugBoxVertexes[1].Color = color;

            Debug.DebugDrawer.DrawLines(device, m_DebugBoxVertexes, 2);
        }
    }
}
