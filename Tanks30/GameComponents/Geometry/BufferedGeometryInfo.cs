using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Geometry
{
    /// <summary>
    /// Información de geometría
    /// </summary>
    public class BufferedGeometryInfo
    {
        /// <summary>
        /// Declaración de formato de vértices
        /// </summary>
        public VertexDeclaration VertexDeclaration = null;
        /// <summary>
        /// Vértices
        /// </summary>
        public VertexPositionNormalTexture[] Vertices = null;
        /// <summary>
        /// Indices
        /// </summary>
        public Int16[] Indices = null;
        /// <summary>
        /// Tipo de primitivas a dibujar
        /// </summary>
        public PrimitiveType PrimitiveType = PrimitiveType.TriangleList;
        /// <summary>
        /// Indica si está indexado
        /// </summary>
        public bool Indexed = false;
        /// <summary>
        /// Número de primitivas
        /// </summary>
        public int PrimitiveCount = -1;
        /// <summary>
        /// Textura
        /// </summary>
        public Texture2D Texture = null;
        /// <summary>
        /// Método de relleno de la geometría
        /// </summary>
        public FillMode FillMode = FillMode.Solid;

        /// <summary>
        /// Dibuja la geometría
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="device">Dispositivo</param>
        /// <param name="effect">Effecto</param>
        public void Draw(GameTime gameTime, GraphicsDevice device, BasicEffect effect)
        {
            FillMode prev = device.RenderState.FillMode;
            device.RenderState.FillMode = this.FillMode;

            device.VertexDeclaration = this.VertexDeclaration;

            effect.Texture = this.Texture;
            effect.TextureEnabled = (this.Texture != null);
            effect.VertexColorEnabled = (this.Texture == null);

            effect.PreferPerPixelLighting = true;

            effect.Begin();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                if (this.Indexed)
                {
                    device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                        this.PrimitiveType,
                        this.Vertices,
                        0,
                        this.Vertices.Length,
                        this.Indices,
                        0,
                        this.PrimitiveCount);
                }
                else
                {
                    device.DrawUserPrimitives<VertexPositionNormalTexture>(
                        this.PrimitiveType,
                        this.Vertices,
                        0,
                        this.PrimitiveCount);
                }

                pass.End();
            }

            effect.End();

            device.VertexDeclaration = null;

            device.RenderState.FillMode = prev;
        }
    }
}
