using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Scenery
{
    /// <summary>
    /// Información en 2D del terreno
    /// </summary>
    public partial class SceneryInfoGameComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Número máximo de vértices de nodo
        private const int _MaxNodeVertexes = 2048;
        // Número máximo de índices de nodo
        private const int _MaxNodeIndexes = _MaxNodeVertexes * 2;
        // Vértices de nodo
        private VertexPositionColor[] m_NodeVertexes = new VertexPositionColor[_MaxNodeVertexes];
        // Índices de nodo
        private short[] m_NodeIndexes = new short[_MaxNodeIndexes];
        // Buffer de vértices
        private VertexBuffer m_NodesVertexBuffer;
        // Buffer de índices
        private IndexBuffer m_NodesIndexBuffer;
        // Declaración de formato de vértices
        private VertexDeclaration m_NodesVertexDeclaration;

        // Efecto para renderizar el componente
        private BasicEffect m_Effect;
        // Escenario al que pertenece el objeto de información
        private SceneryGameComponent m_Scenery;

        // LOD
        private LOD m_Lod = LOD.None;
        /// <summary>
        /// Obtiene o establece el nivel de detalle a mostrar
        /// </summary>
        public LOD Lod
        {
            get
            {
                return m_Lod;
            }
            set
            {
                m_Lod = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public SceneryInfoGameComponent(Game game)
            : base(game)
        {
            m_Scenery = (SceneryGameComponent)game.Services.GetService(typeof(SceneryGameComponent));
        }

        /// <summary>
        /// Inicializar el componente
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Matriz proyección
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(
                0,
                this.GraphicsDevice.Viewport.Width,
                this.GraphicsDevice.Viewport.Height,
                0,
                0.0f,
                1.0f);

            m_Effect = new BasicEffect(this.GraphicsDevice, null);
            m_Effect.View = Matrix.Identity;
            m_Effect.Projection = projectionMatrix;
            m_Effect.World = Matrix.Identity;
            m_Effect.VertexColorEnabled = true;
            m_Effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            m_Effect.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
            m_Effect.LightingEnabled = true;
        }
        /// <summary>
        /// Carga los contenidos gráficos del componente
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            m_NodesVertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionColor.VertexElements);

            //m_NodesVertexBuffer = new VertexBuffer(this.GraphicsDevice,
            //    VertexPositionColor.SizeInBytes * _MaxNodeVertexes,
            //    ResourceUsage.WriteOnly);
            m_NodesVertexBuffer = new VertexBuffer(
                this.GraphicsDevice,
                VertexPositionColor.SizeInBytes * _MaxNodeVertexes,
                BufferUsage.None);

            //m_NodesIndexBuffer = new IndexBuffer(this.GraphicsDevice,
            //    sizeof(short) * _MaxNodeIndexes,
            //    ResourceUsage.WriteOnly,
            //    ResourceManagementMode.Automatic,
            //    IndexElementSize.SixteenBits);
            m_NodesIndexBuffer = new IndexBuffer(this.GraphicsDevice,
                sizeof(short) * _MaxNodeIndexes,
                BufferUsage.WriteOnly,
                IndexElementSize.SixteenBits);
        }
        /// <summary>
        /// Actualiza el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        /// <summary>
        /// Dibujar el componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (m_Lod == LOD.None)
            {
                LODDraw(gameTime, LOD.High);
                LODDraw(gameTime, LOD.Medium);
                LODDraw(gameTime, LOD.Low);
            }
            else
            {
                LODDraw(gameTime, m_Lod);
            }
        }
        /// <summary>
        /// Dibuja nodos por nivel de detalle
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="lod">Nivel de detalle</param>
        private void LODDraw(GameTime gameTime, LOD lod)
        {
            Color nodeColor = Color.White;

            if (lod == LOD.High)
            {
                nodeColor = Color.Red;
            }
            else if (lod == LOD.Medium)
            {
                nodeColor = Color.Orange;
            }
            else if (lod == LOD.Low)
            {
                nodeColor = Color.Green;
            }

            SceneryInfoNodeDrawn[] nodes = m_Scenery.Scenery.GetNodesDrawn(lod);
            int numVerts = ((nodes.Length * 4) > _MaxNodeVertexes) ? _MaxNodeVertexes : (nodes.Length * 4);
            if ((numVerts > 0) && (m_Scenery.Scenery.Width > 2))
            {
                // Inicializar el buffer de vértices
                int n = 0;
                Vector3 pos = new Vector3();
                float scale = (1.0f / m_Scenery.Scenery.Width) * 100.0f;
                for (int i = 0; i <= numVerts - 4; i += 4)
                {
                    SceneryInfoNodeDrawn node = nodes[n++];

                    pos.X = scale * node.UpperLeft.X + 10.0f;
                    pos.Y = scale * node.UpperLeft.Y + 10.0f;
                    pos.Z = 0.0f;
                    m_NodeVertexes[i] = new VertexPositionColor(pos, nodeColor);

                    pos.X = scale * node.LowerRight.X + 10.0f;
                    pos.Y = scale * node.UpperLeft.Y + 10.0f;
                    pos.Z = 0.0f;
                    m_NodeVertexes[i + 1] = new VertexPositionColor(pos, nodeColor);

                    pos.X = scale * node.LowerRight.X + 10.0f;
                    pos.Y = scale * node.LowerRight.Y + 10.0f;
                    pos.Z = 0.0f;
                    m_NodeVertexes[i + 2] = new VertexPositionColor(pos, nodeColor);

                    pos.X = scale * node.UpperLeft.X + 10.0f;
                    pos.Y = scale * node.LowerRight.Y + 10.0f;
                    pos.Z = 0.0f;
                    m_NodeVertexes[i + 3] = new VertexPositionColor(pos, nodeColor);
                }

                // Establecer los vértices en el buffer de vértices
                //m_NodesVertexBuffer.SetData<VertexPositionColor>(
                //    m_NodeVertexes,
                //    0,
                //    numVerts,
                //    SetDataOptions.Discard);
                m_NodesVertexBuffer.SetData<VertexPositionColor>(m_NodeVertexes, 0, numVerts);

                // Crear la lista de líneas para los nodos

                // 4 líneas por nodo y 4 vértices por nodo
                int numLines = numVerts;
                int numIndexes = numLines * 2;
                int v = 0;
                for (int i = 0; i <= numIndexes - 8; i += 8)
                {
                    m_NodeIndexes[i + 0] = (short)(v + 0); m_NodeIndexes[i + 1] = (short)(v + 1);
                    m_NodeIndexes[i + 2] = (short)(v + 1); m_NodeIndexes[i + 3] = (short)(v + 2);
                    m_NodeIndexes[i + 4] = (short)(v + 2); m_NodeIndexes[i + 5] = (short)(v + 3);
                    m_NodeIndexes[i + 6] = (short)(v + 3); m_NodeIndexes[i + 7] = (short)(v + 0);
                    v += 4;
                }

                // Establecer los índices en el buffer de índices
                //m_NodesIndexBuffer.SetData<short>(
                //    m_NodeIndexes,
                //    0,
                //    numIndexes,
                //    SetDataOptions.Discard);
                m_NodesIndexBuffer.SetData<short>(m_NodeIndexes, 0, numIndexes);

                // Dibujar la lista de líneas
                m_Effect.Begin(SaveStateMode.SaveState);

                foreach (EffectPass pass in m_Effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    this.GraphicsDevice.VertexDeclaration = m_NodesVertexDeclaration;
                    this.GraphicsDevice.Vertices[0].SetSource(m_NodesVertexBuffer, 0, VertexPositionColor.SizeInBytes);
                    this.GraphicsDevice.Indices = m_NodesIndexBuffer;

                    this.GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.LineList, 
                        0, 
                        0, 
                        numVerts, 
                        0, 
                        numLines);

                    pass.End();
                }

                m_Effect.End();

                //Vaciar el dispositivo
                this.GraphicsDevice.VertexDeclaration = null;
                this.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
                this.GraphicsDevice.Indices = null;
            }
        }
    }
}