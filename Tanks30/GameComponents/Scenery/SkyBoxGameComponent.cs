using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameComponents.Camera;

namespace GameComponents.Scenery
{
    /// <summary>
    /// Componente que dibuja el cielo
    /// </summary>
    public partial class SkyBoxGameComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Gestor de contenidos
        ContentManager content;

        // Matriz de proyección del componente de cielo
        Matrix m_ProjectionMatrix;

        // Texturas del cielo
        Texture2D[] m_Textures = new Texture2D[6];
        // Efecto para renderizar el cielo
        Effect m_Effect;

        // Declaración de vértices
        VertexDeclaration m_VertexDeclaration;
        // Buffer de vértices
        VertexBuffer m_Vertices;
        // Buffer de índices
        IndexBuffer m_Indices;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Tiempo de juego</param>
        public SkyBoxGameComponent(Game game)
            : base(game)
        {
            content = (ContentManager)game.Services.GetService(typeof(ContentManager));

            GraphicsDeviceManager deviceManager = (GraphicsDeviceManager)game.Services.GetService(typeof(IGraphicsDeviceManager));
            if (deviceManager != null)
            {
                GraphicsDevice device = deviceManager.GraphicsDevice;

                m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)device.Viewport.Width / (float)device.Viewport.Height,
                    SceneryEnvironment.SkyBox.NearClip,
                    SceneryEnvironment.SkyBox.FarClip);
            }
        }

        /// <summary>
        /// Carga el contenido gráfico del componente
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            m_Textures[0] = content.Load<Texture2D>(@"Content\Skybox\back");
            m_Textures[1] = content.Load<Texture2D>(@"Content\Skybox\front");
            m_Textures[2] = content.Load<Texture2D>(@"Content\Skybox\bottom");
            m_Textures[3] = content.Load<Texture2D>(@"Content\Skybox\top");
            m_Textures[4] = content.Load<Texture2D>(@"Content\Skybox\left");
            m_Textures[5] = content.Load<Texture2D>(@"Content\Skybox\right");

            m_Effect = content.Load<Effect>(@"Content\Skybox\skybox");

            m_VertexDeclaration = new VertexDeclaration(
                this.GraphicsDevice,
                new VertexElement[] {
                    new VertexElement(0,0,VertexElementFormat.Vector3,
                           VertexElementMethod.Default,
                            VertexElementUsage.Position,0),
                    new VertexElement(0,sizeof(float)*3,VertexElementFormat.Vector2,
                           VertexElementMethod.Default,
                            VertexElementUsage.TextureCoordinate,0)});

            VertexPositionTexture[] data = new VertexPositionTexture[4 * 6];

            float mag = SceneryEnvironment.SkyBox.FarClip / 2.0f;

            Vector3 vExtents = new Vector3(mag, mag, mag);
            //back
            data[0].Position = new Vector3(vExtents.X, -vExtents.Y, -vExtents.Z);
            data[0].TextureCoordinate.X = 1.0f; data[0].TextureCoordinate.Y = 1.0f;
            data[1].Position = new Vector3(vExtents.X, vExtents.Y, -vExtents.Z);
            data[1].TextureCoordinate.X = 1.0f; data[1].TextureCoordinate.Y = 0.0f;
            data[2].Position = new Vector3(-vExtents.X, vExtents.Y, -vExtents.Z);
            data[2].TextureCoordinate.X = 0.0f; data[2].TextureCoordinate.Y = 0.0f;
            data[3].Position = new Vector3(-vExtents.X, -vExtents.Y, -vExtents.Z);
            data[3].TextureCoordinate.X = 0.0f; data[3].TextureCoordinate.Y = 1.0f;

            //front
            data[4].Position = new Vector3(-vExtents.X, -vExtents.Y, vExtents.Z);
            data[4].TextureCoordinate.X = 1.0f; data[4].TextureCoordinate.Y = 1.0f;
            data[5].Position = new Vector3(-vExtents.X, vExtents.Y, vExtents.Z);
            data[5].TextureCoordinate.X = 1.0f; data[5].TextureCoordinate.Y = 0.0f;
            data[6].Position = new Vector3(vExtents.X, vExtents.Y, vExtents.Z);
            data[6].TextureCoordinate.X = 0.0f; data[6].TextureCoordinate.Y = 0.0f;
            data[7].Position = new Vector3(vExtents.X, -vExtents.Y, vExtents.Z);
            data[7].TextureCoordinate.X = 0.0f; data[7].TextureCoordinate.Y = 1.0f;

            //bottom
            data[8].Position = new Vector3(-vExtents.X, -vExtents.Y, -vExtents.Z);
            data[8].TextureCoordinate.X = 1.0f; data[8].TextureCoordinate.Y = 0.0f;
            data[9].Position = new Vector3(-vExtents.X, -vExtents.Y, vExtents.Z);
            data[9].TextureCoordinate.X = 1.0f; data[9].TextureCoordinate.Y = 1.0f;
            data[10].Position = new Vector3(vExtents.X, -vExtents.Y, vExtents.Z);
            data[10].TextureCoordinate.X = 0.0f; data[10].TextureCoordinate.Y = 1.0f;
            data[11].Position = new Vector3(vExtents.X, -vExtents.Y, -vExtents.Z);
            data[11].TextureCoordinate.X = 0.0f; data[11].TextureCoordinate.Y = 0.0f;

            //top
            data[12].Position = new Vector3(vExtents.X, vExtents.Y, -vExtents.Z);
            data[12].TextureCoordinate.X = 0.0f; data[12].TextureCoordinate.Y = 0.0f;
            data[13].Position = new Vector3(vExtents.X, vExtents.Y, vExtents.Z);
            data[13].TextureCoordinate.X = 0.0f; data[13].TextureCoordinate.Y = 1.0f;
            data[14].Position = new Vector3(-vExtents.X, vExtents.Y, vExtents.Z);
            data[14].TextureCoordinate.X = 1.0f; data[14].TextureCoordinate.Y = 1.0f;
            data[15].Position = new Vector3(-vExtents.X, vExtents.Y, -vExtents.Z);
            data[15].TextureCoordinate.X = 1.0f; data[15].TextureCoordinate.Y = 0.0f;

            //left
            data[16].Position = new Vector3(-vExtents.X, vExtents.Y, -vExtents.Z);
            data[16].TextureCoordinate.X = 1.0f; data[16].TextureCoordinate.Y = 0.0f;
            data[17].Position = new Vector3(-vExtents.X, vExtents.Y, vExtents.Z);
            data[17].TextureCoordinate.X = 0.0f; data[17].TextureCoordinate.Y = 0.0f;
            data[18].Position = new Vector3(-vExtents.X, -vExtents.Y, vExtents.Z);
            data[18].TextureCoordinate.X = 0.0f; data[18].TextureCoordinate.Y = 1.0f;
            data[19].Position = new Vector3(-vExtents.X, -vExtents.Y, -vExtents.Z);
            data[19].TextureCoordinate.X = 1.0f; data[19].TextureCoordinate.Y = 1.0f;

            //right
            data[20].Position = new Vector3(vExtents.X, -vExtents.Y, -vExtents.Z);
            data[20].TextureCoordinate.X = 0.0f; data[20].TextureCoordinate.Y = 1.0f;
            data[21].Position = new Vector3(vExtents.X, -vExtents.Y, vExtents.Z);
            data[21].TextureCoordinate.X = 1.0f; data[21].TextureCoordinate.Y = 1.0f;
            data[22].Position = new Vector3(vExtents.X, vExtents.Y, vExtents.Z);
            data[22].TextureCoordinate.X = 1.0f; data[22].TextureCoordinate.Y = 0.0f;
            data[23].Position = new Vector3(vExtents.X, vExtents.Y, -vExtents.Z);
            data[23].TextureCoordinate.X = 0.0f; data[23].TextureCoordinate.Y = 0.0f;

            //m_Vertices = new VertexBuffer(
            //    this.GraphicsDevice,
            //    typeof(VertexPositionTexture),
            //    4 * 6,
            //    ResourceUsage.WriteOnly);
            m_Vertices = new VertexBuffer(
                this.GraphicsDevice,
                typeof(VertexPositionTexture),
                4 * 6,
                BufferUsage.WriteOnly);

            m_Vertices.SetData<VertexPositionTexture>(data);

            short[] ib = new short[6 * 6];

            for (int x = 0; x < 6; x++)
            {
                ib[x * 6 + 0] = (short)(x * 4 + 0);
                ib[x * 6 + 2] = (short)(x * 4 + 1);
                ib[x * 6 + 1] = (short)(x * 4 + 2);

                ib[x * 6 + 3] = (short)(x * 4 + 2);
                ib[x * 6 + 5] = (short)(x * 4 + 3);
                ib[x * 6 + 4] = (short)(x * 4 + 0);
            }

            //m_Indices = new IndexBuffer(
            //    this.GraphicsDevice,
            //    typeof(short),
            //    6 * 6,
            //    ResourceUsage.WriteOnly);
            m_Indices = new IndexBuffer(
                this.GraphicsDevice,
                typeof(short),
                6 * 6,
                BufferUsage.WriteOnly);

            m_Indices.SetData<short>(ib);
        }
        /// <summary>
        /// Dibuja el contenido gráfico del juego
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (m_Vertices != null)
            {
                this.GraphicsDevice.VertexDeclaration = m_VertexDeclaration;
                this.GraphicsDevice.Vertices[0].SetSource(m_Vertices, 0, m_VertexDeclaration.GetVertexStrideSize(0));
                this.GraphicsDevice.Indices = m_Indices;

                // Para dibujar el cielo es necesaria la matriz vista actual y la dirección
                Vector3 vCameraDirection = BaseCameraGameComponent.gDirection;
                Matrix viewMatrix = BaseCameraGameComponent.gViewMatrix;
                Matrix projectionMatrix = m_ProjectionMatrix;
                Matrix worldMatrix = Matrix.Identity;

                m_Effect.Parameters["worldViewProjection"].SetValue(worldMatrix * viewMatrix * projectionMatrix);

                m_Effect.Begin();

                for (int x = 0; x < 6; x++)
                {
                    float f = 0;
                    switch (x)
                    {
                        case 0: //back
                            f = Vector3.Dot(vCameraDirection, Vector3.Backward);
                            break;
                        case 1: //front
                            f = Vector3.Dot(vCameraDirection, Vector3.Forward);
                            break;
                        case 2: //down
                            f = Vector3.Dot(vCameraDirection, Vector3.Up);
                            break;
                        case 3: //top
                            f = Vector3.Dot(vCameraDirection, Vector3.Down);
                            break;
                        case 4: //left
                            f = Vector3.Dot(vCameraDirection, Vector3.Right);
                            break;
                        case 5: //right
                            f = Vector3.Dot(vCameraDirection, Vector3.Left);
                            break;
                    }

                    if (f <= 0)
                    {
                        m_Effect.Parameters["baseTexture"].SetValue(m_Textures[x]);

                        foreach (EffectPass pass in m_Effect.CurrentTechnique.Passes)
                        {
                            pass.Begin();

                            this.GraphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                0,
                                x * 4,
                                4,
                                x * 6,
                                2);

                            pass.End();
                        }
                    }
                }

                m_Effect.End();
            }
        }
    }
}


