using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using GameComponents;
using GameComponents.Physics;
using GameComponents.Scenery;
using GameComponents.Camera;
using GameComponents.Vehicles;
using GameComponents.Components.Particles;

using Tanks.Services;
using Tanks.Vehicles;
using Physics;

namespace Tanks
{
    /// <summary>
    /// Juego
    /// </summary>
    public class TanksGame : Microsoft.Xna.Framework.Game
    {
        // Gestor de dispositivos gráficos
        GraphicsDeviceManager graphics;
        // Gestor de contenidos
        ContentManager content;

        // Controlador de colisión
        private CollisionManager collision;
        // Terreno
        private SceneryGameComponent scenery;
        // Cielo
        private SkyBoxGameComponent skyBox;
        // Jugador
        private ThirdPersonCameraGameComponent camera;
        // Información del terreno
        private SceneryInfoGameComponent info;
        // Servicio contenedor de tanques
        private TankContainerService tankContainer;

        // Tanque actual
        private Rhino currentTank;

        #region Teclas

        // Tecla que indica cuándo hay que dibujar los nodos
        private Keys m_ShowNodesDrawnKey = Keys.F1;
        // Tecla que cambia el LOD de los nodos a dibujar
        private Keys m_IncLODKey = Keys.F2;
        // Tecla que cambia el LOD de los nodos a dibujar
        private Keys m_DecLODKey = Keys.F3;
        // Tecla que activa o desactiva la niebla
        private Keys m_FogKey = Keys.F4;
        // Tecla que activa o desactiva la luz global
        private Keys m_GlobalLightningKey = Keys.F5;
        // Tecla que activa o desactiva la luz 1
        private Keys m_Light0Key = Keys.F6;
        // Tecla que activa o desactiva la luz 2
        private Keys m_Light1Key = Keys.F7;
        // Tecla que activa o desactiva la luz 3
        private Keys m_Light2Key = Keys.F8;
        // Tecla que activa o desactiva el wireframe
        private Keys m_WireFrameKey = Keys.F12;

        // Tecla que cambia a la siguiente posición del vehículo
        private Keys m_NextVehiclePositionKey = Keys.Tab;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TanksGame()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            this.Services.AddService(content.GetType(), content);

#if DEBUG
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferMultiSampling = false;
            graphics.IsFullScreen = false;
#else
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferMultiSampling = false;
            graphics.IsFullScreen = true;
#endif
        }

        /// <summary>
        /// Inicializar
        /// </summary>
        protected override void Initialize()
        {
            InputHelper.GraphicsDevice = graphics.GraphicsDevice;

            collision = new CollisionManager(this);
            Components.Add(collision);
            Services.AddService(collision.GetType(), collision);

            scenery = new SceneryGameComponent(this);
            Components.Add(scenery);
            Services.AddService(typeof(SceneryGameComponent), scenery);

            info = new SceneryInfoGameComponent(this);
            Components.Add(info);

            camera = new ThirdPersonCameraGameComponent(this);
            Components.Add(camera);

            skyBox = new SkyBoxGameComponent(this);
            Components.Add(skyBox);

            tankContainer = new TankContainerService(this);
            Services.AddService(tankContainer.GetType(), tankContainer);

            Random rnd = new Random();

            for (int i = 0; i < 60; i++)
            {
                Rhino rhino = tankContainer.AddRhino(new Point(rnd.Next(5000) + 5000, rnd.Next(5000) + 5000));
            }

            base.Initialize();

            currentTank = (Rhino)tankContainer.Tanks[0];
            camera.ModelToFollow = currentTank;
            currentTank.HasFocus = true;

            tankContainer.Tanks[0].Position = new Vector3(8000, 0, 8000);
            tankContainer.Tanks[1].Position = currentTank.Position - Vector3.Multiply(Vector3.One, 5f);
            tankContainer.Tanks[2].Position = currentTank.Position - Vector3.Multiply(Vector3.One, 10f);
            tankContainer.Tanks[3].Position = currentTank.Position - Vector3.Multiply(Vector3.One, 15f);

            foreach (TankGameComponent tank in tankContainer.Tanks)
            {
                tank.AutoPilot.GoTo(scenery.Center, 10f);
            }

            tankContainer.Tanks[0].AutoPilot.Enabled = true;
            tankContainer.Tanks[1].AutoPilot.Follow(tankContainer.Tanks[0]);
            tankContainer.Tanks[2].AutoPilot.Follow(tankContainer.Tanks[1]);
            tankContainer.Tanks[3].AutoPilot.Follow(tankContainer.Tanks[2]);
        }

        /// <summary>
        /// Carga de los componentes gráficos
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }
        /// <summary>
        /// Descarga los componentes gráficos
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        /// <summary>
        /// Actualizar los componentes del juego
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            InputHelper.Begin(gameTime);

            base.Update(gameTime);

            this.UpdateEnvironment(gameTime);

            this.UpdateTanks(gameTime);

            InputHelper.End();
        }
        /// <summary>
        /// Dibujar los componentes del juego
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(SceneryEnvironment.Ambient.AmbientColor);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Actualizar elementos del entorno
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        private void UpdateEnvironment(GameTime gameTime)
        {
            if (InputHelper.KeyUpEvent(m_ShowNodesDrawnKey))
            {
                info.Visible = !info.Visible;
            }

            if (InputHelper.KeyUpEvent(m_IncLODKey))
            {
                if (info.Lod == LOD.None)
                {
                    info.Lod = LOD.High;
                    scenery.Lod = LOD.High;
                }
                else if (info.Lod == LOD.High)
                {
                    info.Lod = LOD.Medium;
                    scenery.Lod = LOD.Medium;
                }
                else if (info.Lod == LOD.Medium)
                {
                    info.Lod = LOD.Low;
                    scenery.Lod = LOD.Low;
                }
                else if (info.Lod == LOD.Low)
                {
                    info.Lod = LOD.None;
                    scenery.Lod = LOD.None;
                }
            }

            if (InputHelper.KeyUpEvent(m_DecLODKey))
            {
                if (info.Lod == LOD.Medium)
                {
                    info.Lod = LOD.High;
                    scenery.Lod = LOD.High;
                }
                else if (info.Lod == LOD.Low)
                {
                    info.Lod = LOD.Medium;
                    scenery.Lod = LOD.Medium;
                }
                else if (info.Lod == LOD.None)
                {
                    info.Lod = LOD.Low;
                    scenery.Lod = LOD.Low;
                }
                else if (info.Lod == LOD.High)
                {
                    info.Lod = LOD.None;
                    scenery.Lod = LOD.None;
                }
            }

            if (InputHelper.KeyUpEvent(m_FogKey))
            {
                SceneryEnvironment.Fog.FogEnabled = !SceneryEnvironment.Fog.FogEnabled;
            }

            if (InputHelper.KeyUpEvent(m_GlobalLightningKey))
            {
                SceneryEnvironment.Ambient.LightingEnabled = !SceneryEnvironment.Ambient.LightingEnabled;
            }

            if (InputHelper.KeyUpEvent(m_Light0Key))
            {
                SceneryEnvironment.Ambient.Light0Enable = !SceneryEnvironment.Ambient.Light0Enable;
            }

            if (InputHelper.KeyUpEvent(m_Light1Key))
            {
                SceneryEnvironment.Ambient.Light1Enable = !SceneryEnvironment.Ambient.Light1Enable;
            }

            if (InputHelper.KeyUpEvent(m_Light2Key))
            {
                SceneryEnvironment.Ambient.Light2Enable = !SceneryEnvironment.Ambient.Light2Enable;
            }

            if (InputHelper.KeyUpEvent(m_WireFrameKey))
            {
                if (scenery.FillMode == FillMode.Solid)
                {
                    scenery.FillMode = FillMode.WireFrame;
                }
                else
                {
                    scenery.FillMode = FillMode.Solid;
                }
            }
        }
        /// <summary>
        /// Actualiza los tanques
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        private void UpdateTanks(GameTime gameTime)
        {
            if (InputHelper.KeyUpEvent(m_NextVehiclePositionKey))
            {
                currentTank.SetNextPlayerPosition();
            }

            foreach (TankGameComponent tank in tankContainer.Tanks)
            {
                if ((tank != currentTank) && (!tank.AutoPilot.Enabled))
                {
                    Random rnd = new Random();

                    tank.AutoPilot.GoTo(new Vector3(rnd.Next(16000), 0, rnd.Next(16000)), 1.0f);
                }
            }
        }
    }
}