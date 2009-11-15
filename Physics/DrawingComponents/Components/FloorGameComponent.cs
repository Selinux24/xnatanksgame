using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace DrawingComponents
{
    /// <summary>
    /// Componente que dibuja un suelo
    /// </summary>
    public class FloorGameComponent : DrawableGameComponent
    {
        // Matriz de transformación del componente
        Matrix transform = Matrix.Identity;
        // Posición
        Vector3 m_Position = Vector3.Zero;
        // Orientación
        Quaternion m_Rotation = Quaternion.Identity;

        // Nombre de la textura del suelo
        string textureName;
        // Textura
        Texture2D texture;

        // Esquina Noreste
        VertexPositionTexture NorthEast;
        // Esquina Noroeste
        VertexPositionTexture NorthWest;
        // Esquina Sureste
        VertexPositionTexture SouthEast;
        // Esquina Suroeste
        VertexPositionTexture SouthWest;

        //AABB Envolvente
        BoundingBox m_Box;

        // Efecto
        BasicEffect effect;
        // Declaración
        VertexDeclaration decal;
        // Vértices
        VertexBuffer vertexBuffer;
        // Indices
        IndexBuffer indexBuffer;

        /// <summary>
        /// Obtiene o establece la posición
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }
        /// <summary>
        /// Obtiene o establece la orientación
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                m_Rotation = value;
            }
        }
        /// <summary>
        /// Obtiene el AABB envolvente
        /// </summary>
        public BoundingBox Box
        {
            get
            {
                return m_Box;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        /// <param name="side">Lado del suelo</param>
        /// <param name="textureAssetName">Nombre de la textura</param>
        public FloorGameComponent(Game game, float side, string textureAssetName)
            : base(game)
        {
            float edge = side / 2f;

            NorthEast = new VertexPositionTexture(new Vector3(-edge, 0, -edge), new Vector2(0, 0));
            NorthWest = new VertexPositionTexture(new Vector3(edge, 0, -edge), new Vector2(1, 0));
            SouthEast = new VertexPositionTexture(new Vector3(-edge, 0, edge), new Vector2(0, 1));
            SouthWest = new VertexPositionTexture(new Vector3(edge, 0, edge), new Vector2(1, 1));

            textureName = textureAssetName;

            this.m_Box = new BoundingBox(NorthEast.Position, SouthWest.Position);
        }

        public FloorGameComponent(Game game, Vector3[] list, string textureAssetName)
            : base(game)
        {
            NorthEast = new VertexPositionTexture(list[0], new Vector2(0, 0));
            NorthWest = new VertexPositionTexture(list[1], new Vector2(1, 0));
            SouthEast = new VertexPositionTexture(list[2], new Vector2(0, 1));
            SouthWest = new VertexPositionTexture(list[3], new Vector2(1, 1));

            textureName = textureAssetName;

            this.m_Box = new BoundingBox(NorthEast.Position, SouthWest.Position);
        }

        /// <summary>
        /// Carga el contenido
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            texture = this.Game.Content.Load<Texture2D>(textureName);

            this.LoadFloor();
        }
        /// <summary>
        /// Actualiza el estado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            transform = Matrix.CreateFromQuaternion(m_Rotation) * Matrix.CreateTranslation(m_Position);
        }
        /// <summary>
        /// Dibuja el contenido
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            DrawFloor();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Carga un suelo
        /// </summary>
        private void LoadFloor()
        {
            effect = new BasicEffect(GraphicsDevice, null);

            decal = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements);

            List<VertexPositionTexture> points = new List<VertexPositionTexture>();

            points.Add(NorthEast);
            points.Add(NorthWest);
            points.Add(SouthEast);
            points.Add(SouthWest);

            vertexBuffer = new VertexBuffer(
                GraphicsDevice,
                VertexPositionTexture.SizeInBytes * points.Count,
                BufferUsage.WriteOnly);

            vertexBuffer.SetData<VertexPositionTexture>(points.ToArray());

            List<Int16> indexes = new List<Int16>();

            //Up
            indexes.Add(0);
            indexes.Add(1);
            indexes.Add(2);
            indexes.Add(1);
            indexes.Add(3);
            indexes.Add(2);

            indexBuffer = new IndexBuffer(
                GraphicsDevice,
                sizeof(Int16) * indexes.Count,
                BufferUsage.None,
                IndexElementSize.SixteenBits);

            indexBuffer.SetData<Int16>(indexes.ToArray());
        }
        /// <summary>
        /// Dibuja un suelo
        /// </summary>
        private void DrawFloor()
        {
            GraphicsDevice.VertexDeclaration = decal;
            GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionTexture.SizeInBytes);
            GraphicsDevice.Indices = indexBuffer;

            effect.World = transform * GlobalMatrices.World;
            effect.View = GlobalMatrices.View;
            effect.Projection = GlobalMatrices.Projection;

            effect.Texture = texture;
            effect.TextureEnabled = true;

            effect.Begin();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                GraphicsDevice.RenderState.CullMode = CullMode.None;
                GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

                pass.End();
            }

            effect.End();
        }
    }
}


