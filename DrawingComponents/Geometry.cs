using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DrawingComponents
{
    /// <summary>
    /// Geometr�a
    /// </summary>
    public abstract class Geometry
    {
        /// <summary>
        /// Efecto
        /// </summary>
        public BasicEffect basicEffect;
        /// <summary>
        /// Declaraci�n de formato de v�rtices
        /// </summary>
        public VertexDeclaration basicEffectVertexDeclaration;

        /// <summary>
        /// Indica si es geometr�a indexada
        /// </summary>
        public bool indexed = false;
        /// <summary>
        /// Tipo de primitivas a dibujar
        /// </summary>
        public PrimitiveType primitiveType = PrimitiveType.TriangleList;
        /// <summary>
        /// N�mero de primitivas
        /// </summary>
        public int primitiveCount = -1;

        /// <summary>
        /// Modo de relleno de las primitivas
        /// </summary>
        public FillMode fillMode = FillMode.Solid;
        /// <summary>
        /// Indica el modo de dibujado de las caras
        /// </summary>
        public CullMode cullMode = CullMode.CullCounterClockwiseFace;
        /// <summary>
        /// Tama�o del punto
        /// </summary>
        public float pointSize = 5f;

        /// <summary>
        /// Dibuja la geometr�a
        /// </summary>
        /// <param name="device">Dispositivo gr�fico</param>
        /// <param name="world">Matriz mundo del objeto</param>
        /// <param name="view">Matriz vista</param>
        /// <param name="projection">Matriz proyecci�n</param>
        public void Draw(GraphicsDevice device, Matrix world, Matrix view, Matrix projection)
        {
            if (indexed)
            {
                DrawIndexBuffer(device, world, view, projection);
            }
            else
            {
                DrawVertexBuffer(device, world, view, projection);
            }
        }
        /// <summary>
        /// Dibuja la geometr�a usando una lista de v�rtices
        /// </summary>
        /// <param name="device">Dispositivo</param>
        /// <param name="world">Matriz mundo del objeto</param>
        /// <param name="view">Matriz vista</param>
        /// <param name="projection">Matriz proyecci�n</param>
        private void DrawVertexBuffer(GraphicsDevice device, Matrix world, Matrix view, Matrix projection)
        {
            FillMode prevFill = device.RenderState.FillMode;
            device.RenderState.FillMode = fillMode;

            CullMode prevCull = device.RenderState.CullMode;
            device.RenderState.CullMode = cullMode;

            this.SetEffectParams(basicEffect);

            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;

            basicEffect.Begin();

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.DrawGeometry(device);
                
                pass.End();
            }

            basicEffect.End();

            device.VertexDeclaration = null;

            device.RenderState.FillMode = prevFill;

            device.RenderState.CullMode = prevCull;
        }
        /// <summary>
        /// Dibuja la geometr�a usando una lista de v�rtices e �ndices
        /// </summary>
        /// <param name="device">Dispositivo</param>
        /// <param name="world">Matriz mundo del objeto</param>
        /// <param name="view">Matriz vista</param>
        /// <param name="projection">Matriz proyecci�n</param>
        private void DrawIndexBuffer(GraphicsDevice device, Matrix world, Matrix view, Matrix projection)
        {
            FillMode prevFill = device.RenderState.FillMode;
            device.RenderState.FillMode = fillMode;

            CullMode prevCull = device.RenderState.CullMode;
            device.RenderState.CullMode = cullMode;

            this.SetEffectParams(basicEffect);

            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;

            basicEffect.Begin();

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.DrawIndexedGeometry(device);

                pass.End();
            }

            basicEffect.End();

            device.VertexDeclaration = null;

            device.RenderState.FillMode = prevFill;

            device.RenderState.CullMode = prevCull;
        }
        /// <summary>
        /// Establece los par�metros de renderizaci�n en el efecto especificado
        /// </summary>
        /// <param name="basicEffect">Efecto</param>
        public abstract void SetEffectParams(BasicEffect basicEffect);
        /// <summary>
        /// Dibuja la geometr�a
        /// </summary>
        /// <param name="device">Dispositivo gr�fico</param>
        public abstract void DrawGeometry(GraphicsDevice device);
        /// <summary>
        /// Dibuja la geometr�a indexada
        /// </summary>
        /// <param name="device">Dispositivo gr�fico</param>
        public abstract void DrawIndexedGeometry(GraphicsDevice device);
    }
    /// <summary>
    /// Geometr�a de v�rtices coloreados
    /// </summary>
    public class GeometryVertexPositionColorInfo : Geometry
    {
        /// <summary>
        /// �ndices
        /// </summary>
        public short[] indices;
        /// <summary>
        /// V�rtices
        /// </summary>
        public VertexPositionColor[] vertices;

        /// <summary>
        /// Establece los par�metros de renderizaci�n en el efecto especificado
        /// </summary>
        /// <param name="basicEffect">Efecto</param>
        public override void SetEffectParams(BasicEffect basicEffect)
        {
            basicEffect.Texture = null;
            basicEffect.TextureEnabled = false;
            basicEffect.VertexColorEnabled = true;
        }
        /// <summary>
        /// Dibuja la geometr�a
        /// </summary>
        /// <param name="device">Dispositivo gr�fico</param>
        public override void DrawGeometry(GraphicsDevice device)
        {
            if (device != null)
            {
                if (vertices != null)
                {
                    device.VertexDeclaration = basicEffectVertexDeclaration;

                    device.DrawUserPrimitives<VertexPositionColor>(
                        primitiveType,
                        vertices,
                        0,
                        primitiveCount);
                }
            }
        }
        /// <summary>
        /// Dibuja la geometr�a indexada
        /// </summary>
        /// <param name="device">Dispositivo gr�fico</param>
        public override void DrawIndexedGeometry(GraphicsDevice device)
        {
            if (device != null)
            {
                if (vertices != null)
                {
                    device.VertexDeclaration = basicEffectVertexDeclaration;

                    device.DrawUserIndexedPrimitives<VertexPositionColor>(
                        primitiveType,
                        vertices,
                        0,
                        vertices.Length,
                        indices,
                        0,
                        primitiveCount);
                }
            }
        }
    }
    /// <summary>
    /// Geometr�a de v�rtices texturizados
    /// </summary>
    public class GeometryVertexNormalTextureInfo : Geometry
    {
        /// <summary>
        /// �ndices
        /// </summary>
        public short[] indices;
        /// <summary>
        /// V�rtices
        /// </summary>
        public VertexPositionNormalTexture[] vertices;
        /// <summary>
        /// Textura
        /// </summary>
        public Texture2D texture;

        /// <summary>
        /// Establece los par�metros de renderizaci�n en el efecto especificado
        /// </summary>
        /// <param name="basicEffect">Efecto</param>
        public override void SetEffectParams(BasicEffect basicEffect)
        {
            basicEffect.Texture = texture;
            basicEffect.TextureEnabled = (texture != null);
            basicEffect.VertexColorEnabled = (texture == null);
        }
        /// <summary>
        /// Dibuja la geometr�a
        /// </summary>
        /// <param name="device">Dispositivo gr�fico</param>
        public override void DrawGeometry(GraphicsDevice device)
        {
            if (device != null)
            {
                if (vertices != null)
                {
                    device.VertexDeclaration = basicEffectVertexDeclaration;

                    device.DrawUserPrimitives<VertexPositionNormalTexture>(
                        primitiveType,
                        vertices,
                        0,
                        primitiveCount);
                }
            }
        }
        /// <summary>
        /// Dibuja la geometr�a indexada
        /// </summary>
        /// <param name="device">Dispositivo gr�fico</param>
        public override void DrawIndexedGeometry(GraphicsDevice device)
        {
            if (device != null)
            {
                if (vertices != null)
                {
                    device.VertexDeclaration = basicEffectVertexDeclaration;

                    device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                        primitiveType,
                        vertices,
                        0,
                        vertices.Length,
                        indices,
                        0,
                        primitiveCount);
                }
            }
        }
    }
}
