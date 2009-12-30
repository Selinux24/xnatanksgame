using System;
using GameComponents;
using GameComponents.Camera;
using GameComponents.Physics;
using GameComponents.Scenery;
using GameComponents.Vehicles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tanks.Services;
using Tanks.Vehicles;
using System.Collections.Generic;

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
        private VehicleContainerService tankContainer;

        // Tanque actual
        private Vehicle currentTank;

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
        // Cambio al siguiente vehículo
        private Keys m_NextVehicleKey = Keys.Add;
        // Cambio al vehículo anterior
        private Keys m_PreviousVehicleKey = Keys.Subtract;

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

            scenery = new SceneryGameComponent(this);
            scenery.UpdateOrder = 0;
            Components.Add(scenery);
            Services.AddService(typeof(SceneryGameComponent), scenery);

            tankContainer = new VehicleContainerService(this);
            tankContainer.UpdateOrder = 1;
            Services.AddService(tankContainer.GetType(), tankContainer);

            collision = new CollisionManager(this);
            collision.UpdateOrder = 2;
            Components.Add(collision);
            Services.AddService(collision.GetType(), collision);

            camera = new ThirdPersonCameraGameComponent(this);
            camera.UpdateOrder = 3;
            Components.Add(camera);

            skyBox = new SkyBoxGameComponent(this);
            skyBox.UpdateOrder = 4;
            Components.Add(skyBox);

            info = new SceneryInfoGameComponent(this);
            info.UpdateOrder = 5;
            Components.Add(info);

            Random rnd = new Random();

            AddSquadron(TankTypes.LandSpeeder, 3, rnd);
            AddSquadron(TankTypes.LandSpeeder, 3, rnd);
            AddSquadron(TankTypes.LandSpeeder, 3, rnd);
            AddSquadron(TankTypes.LandRaider, 2, rnd);
            AddSquadron(TankTypes.LemanRuss, 3, rnd);
            AddSquadron(TankTypes.LemanRuss, 3, rnd);
            AddSquadron(TankTypes.Rhino, 3, rnd);
            AddSquadron(TankTypes.Rhino, 3, rnd);
            AddSquadron(TankTypes.Rhino, 3, rnd);

            base.Initialize();

            this.SetFocus(tankContainer.Vehicles[0]);
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
        /// Establece el foco en el tanque designado
        /// </summary>
        /// <param name="tank">Tanque</param>
        private void SetFocus(Vehicle tank)
        {
            if (this.currentTank != null)
            {
                // Quitar el foco del tanque anterior
                this.currentTank.HasFocus = false;

                // Indicar que la cámara no seguirá a ningún tanque
                this.camera.ModelToFollow = null;
            }

            if (tank != null)
            {
                // Indicar que la cámara debe seguir al tanque seleccionado
                this.camera.ModelToFollow = tank;

                // Establecer el foco en el tanque
                tank.HasFocus = true;

                // Actualizar el marcador de tanque actual
                this.currentTank = tank;
            }
        }
        /// <summary>
        /// Añade un escuadrón del tipo y la cantidad especificada
        /// </summary>
        /// <param name="type">Tipo de escuadrón</param>
        /// <param name="count">Cantidad de vehículos</param>
        /// <param name="rnd">Aleatorio</param>
        private void AddSquadron(TankTypes type, int count, Random rnd)
        {
            // Punto de posición del escuadrón
            Point where = new Point(rnd.Next(5000) + 5000, rnd.Next(5000) + 5000);

            // Lista de componentes creados
            List<Vehicle> tankList = new List<Vehicle>();

            for (int i = 0; i < count; i++)
            {
                if (type == TankTypes.Rhino)
                {
                    tankList.Add(tankContainer.AddRhino(where));
                }
                else if (type == TankTypes.LandSpeeder)
                {
                    tankList.Add(tankContainer.AddLandSpeeder(where));
                }
                else if (type == TankTypes.LandRaider)
                {
                    tankList.Add(tankContainer.AddLandRaider(where));
                }
                else if (type == TankTypes.LemanRuss)
                {
                    tankList.Add(tankContainer.AddLemanRuss(where));
                }
            }

            // Establecer el destino del piloto automático
            tankList[0].AutoPilot.GoTo(scenery.Center, 50f);
            tankList[0].AutoPilot.Enabled = true;

            for (int i = 1; i < count; i++)
            {
                // Posición de cada tanque relativa al anterior
                tankList[i].Position = tankList[i - 1].Position - Vector3.Multiply(Vector3.One, 10f);
                // Indicar a cada tanque que siga al anterior
                tankList[i].AutoPilot.Follow(tankList[i - 1]);
            }
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
            if (InputHelper.KeyUpEvent(m_NextVehicleKey))
            {
                int index = tankContainer.IndexOf(currentTank);
                if (index >= 0)
                {
                    if (index < tankContainer.Vehicles.Length - 1)
                    {
                        this.SetFocus(tankContainer.Vehicles[index + 1]);
                    }
                    else
                    {
                        this.SetFocus(tankContainer.Vehicles[0]);
                    }
                }
            }

            if (InputHelper.KeyUpEvent(m_PreviousVehicleKey))
            {
                int index = tankContainer.IndexOf(currentTank);
                if (index >= 0)
                {
                    if (index == 0)
                    {
                        this.SetFocus(tankContainer.Vehicles[tankContainer.Vehicles.Length - 1]);
                    }
                    else
                    {
                        this.SetFocus(tankContainer.Vehicles[index - 1]);
                    }
                }
            }

            if (InputHelper.KeyUpEvent(m_NextVehiclePositionKey))
            {
                currentTank.SetNextPlayerPosition();
            }

            foreach (Vehicle tank in tankContainer.Vehicles)
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