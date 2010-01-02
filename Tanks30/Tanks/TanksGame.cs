using System.Collections.Generic;
using GameComponents;
using GameComponents.Camera;
using GameComponents.MathComponents;
using GameComponents.Physics;
using GameComponents.Scenery;
using GameComponents.Vehicles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tanks
{
    using Tanks.Services;

    /// <summary>
    /// Juego
    /// </summary>
    public class TanksGame : Microsoft.Xna.Framework.Game
    {
        // Gestor de dispositivos gráficos
        protected GraphicsDeviceManager Graphics;

        // Controlador de colisión
        private CollisionManager m_Collision;
        // Terreno
        private SceneryGameComponent m_Scenery;
        // Cielo
        private SkyBoxGameComponent m_SkyBox;
        // Jugador
        private ThirdPersonCameraGameComponent m_Camera;
        // Información del terreno
        private SceneryInfoGameComponent m_Info;
        // Servicio contenedor de vehículos
        private VehicleContainerService m_VehicleContainer;

        // Vehículo actual
        private Vehicle m_CurrentVehicle;

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
            this.Graphics = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Inicializar
        /// </summary>
        protected override void Initialize()
        {
#if DEBUG
            this.Graphics.PreferredBackBufferWidth = 800;
            this.Graphics.PreferredBackBufferHeight = 600;
            this.Graphics.PreferMultiSampling = false;
            this.Graphics.IsFullScreen = false;
#else
            this.Graphics.PreferredBackBufferWidth = this.GraphicsDevice.DisplayMode.Width;
            this.Graphics.PreferredBackBufferHeight = this.GraphicsDevice.DisplayMode.Height;
            this.Graphics.PreferMultiSampling = false;
            this.Graphics.IsFullScreen = true;
#endif

            InputHelper.GraphicsDevice = Graphics.GraphicsDevice;

            m_Scenery = new SceneryGameComponent(this);
            m_Scenery.UpdateOrder = 0;
            Components.Add(m_Scenery);
            Services.AddService(typeof(SceneryGameComponent), m_Scenery);

            m_VehicleContainer = new VehicleContainerService(this);
            m_VehicleContainer.UpdateOrder = 1;
            Services.AddService(m_VehicleContainer.GetType(), m_VehicleContainer);

            m_Collision = new CollisionManager(this);
            m_Collision.UpdateOrder = 2;
            Components.Add(m_Collision);
            Services.AddService(m_Collision.GetType(), m_Collision);

            m_Camera = new ThirdPersonCameraGameComponent(this);
            m_Camera.UpdateOrder = 3;
            Components.Add(m_Camera);

            m_SkyBox = new SkyBoxGameComponent(this);
            m_SkyBox.UpdateOrder = 4;
            Components.Add(m_SkyBox);

            m_Info = new SceneryInfoGameComponent(this);
            m_Info.UpdateOrder = 5;
            Components.Add(m_Info);

            Vehicle[] squad01 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            Vehicle[] squad02 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            Vehicle[] squad03 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            Vehicle[] squad04 = AddSquadron(VehicleTypes.LandRaider, 2);
            Vehicle[] squad05 = AddSquadron(VehicleTypes.LemanRuss, 3);
            Vehicle[] squad06 = AddSquadron(VehicleTypes.LemanRuss, 3);
            Vehicle[] squad07 = AddSquadron(VehicleTypes.Rhino, 3);
            Vehicle[] squad08 = AddSquadron(VehicleTypes.Rhino, 3);
            Vehicle[] squad09 = AddSquadron(VehicleTypes.Rhino, 3);

            squad01[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            squad02[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            squad03[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            squad04[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            squad05[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            squad06[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            squad07[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            squad08[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            squad09[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);

            this.SetFocus(squad01[0]);

            base.Initialize();
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

            this.UpdateVehicles(gameTime);

            InputHelper.End();
        }
        /// <summary>
        /// Dibujar los componentes del juego
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(SceneryEnvironment.Ambient.AmbientColor);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Establece el foco en el tanque designado
        /// </summary>
        /// <param name="tank">Tanque</param>
        private void SetFocus(Vehicle tank)
        {
            if (this.m_CurrentVehicle != null)
            {
                // Quitar el foco del tanque anterior
                this.m_CurrentVehicle.HasFocus = false;

                // Indicar que la cámara no seguirá a ningún tanque
                this.m_Camera.ModelToFollow = null;
            }

            if (tank != null)
            {
                // Indicar que la cámara debe seguir al tanque seleccionado
                this.m_Camera.ModelToFollow = tank;

                // Establecer el foco en el tanque
                tank.HasFocus = true;

                // Actualizar el marcador de tanque actual
                this.m_CurrentVehicle = tank;
            }
        }
        /// <summary>
        /// Añade un escuadrón del tipo y la cantidad especificada
        /// </summary>
        /// <param name="type">Tipo de escuadrón</param>
        /// <param name="count">Cantidad de vehículos</param>
        private Vehicle[] AddSquadron(VehicleTypes type, int count)
        {
            // Punto de posición del escuadrón
            Point where = new Point(RandomComponent.Next(5000) + 5000, RandomComponent.Next(5000) + 5000);

            // Lista de componentes creados
            List<Vehicle> vehicleList = new List<Vehicle>();

            for (int i = 0; i < count; i++)
            {
                if (type == VehicleTypes.Rhino)
                {
                    vehicleList.Add(m_VehicleContainer.AddRhino(where));
                }
                else if (type == VehicleTypes.LandSpeeder)
                {
                    vehicleList.Add(m_VehicleContainer.AddLandSpeeder(where));
                }
                else if (type == VehicleTypes.LandRaider)
                {
                    vehicleList.Add(m_VehicleContainer.AddLandRaider(where));
                }
                else if (type == VehicleTypes.LemanRuss)
                {
                    vehicleList.Add(m_VehicleContainer.AddLemanRuss(where));
                }
            }

            for (int i = 1; i < count; i++)
            {
                // Posición de cada tanque relativa al anterior
                vehicleList[i].Position = vehicleList[i - 1].Position - Vector3.Multiply(Vector3.One, 10f);
                // Indicar a cada tanque que siga al anterior
                vehicleList[i].AutoPilot.Follow(vehicleList[i - 1]);
            }

            return vehicleList.ToArray();
        }
        /// <summary>
        /// Actualizar elementos del entorno
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        private void UpdateEnvironment(GameTime gameTime)
        {
            if (InputHelper.KeyUpEvent(m_ShowNodesDrawnKey))
            {
                m_Info.Visible = !m_Info.Visible;
            }

            if (InputHelper.KeyUpEvent(m_IncLODKey))
            {
                if (m_Info.Lod == LOD.None)
                {
                    m_Info.Lod = LOD.High;
                    m_Scenery.Lod = LOD.High;
                }
                else if (m_Info.Lod == LOD.High)
                {
                    m_Info.Lod = LOD.Medium;
                    m_Scenery.Lod = LOD.Medium;
                }
                else if (m_Info.Lod == LOD.Medium)
                {
                    m_Info.Lod = LOD.Low;
                    m_Scenery.Lod = LOD.Low;
                }
                else if (m_Info.Lod == LOD.Low)
                {
                    m_Info.Lod = LOD.None;
                    m_Scenery.Lod = LOD.None;
                }
            }

            if (InputHelper.KeyUpEvent(m_DecLODKey))
            {
                if (m_Info.Lod == LOD.Medium)
                {
                    m_Info.Lod = LOD.High;
                    m_Scenery.Lod = LOD.High;
                }
                else if (m_Info.Lod == LOD.Low)
                {
                    m_Info.Lod = LOD.Medium;
                    m_Scenery.Lod = LOD.Medium;
                }
                else if (m_Info.Lod == LOD.None)
                {
                    m_Info.Lod = LOD.Low;
                    m_Scenery.Lod = LOD.Low;
                }
                else if (m_Info.Lod == LOD.High)
                {
                    m_Info.Lod = LOD.None;
                    m_Scenery.Lod = LOD.None;
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
                if (m_Scenery.FillMode == FillMode.Solid)
                {
                    m_Scenery.FillMode = FillMode.WireFrame;
                }
                else
                {
                    m_Scenery.FillMode = FillMode.Solid;
                }
            }
        }
        /// <summary>
        /// Actualiza los tanques
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        private void UpdateVehicles(GameTime gameTime)
        {
            if (InputHelper.KeyUpEvent(this.m_NextVehicleKey))
            {
                int index = this.m_VehicleContainer.IndexOf(this.m_CurrentVehicle);
                if (index >= 0)
                {
                    if (index < this.m_VehicleContainer.Vehicles.Length - 1)
                    {
                        this.SetFocus(this.m_VehicleContainer.Vehicles[index + 1]);
                    }
                    else
                    {
                        this.SetFocus(this.m_VehicleContainer.Vehicles[0]);
                    }
                }
            }

            if (InputHelper.KeyUpEvent(this.m_PreviousVehicleKey))
            {
                int index = this.m_VehicleContainer.IndexOf(this.m_CurrentVehicle);
                if (index >= 0)
                {
                    if (index == 0)
                    {
                        this.SetFocus(this.m_VehicleContainer.Vehicles[this.m_VehicleContainer.Vehicles.Length - 1]);
                    }
                    else
                    {
                        this.SetFocus(this.m_VehicleContainer.Vehicles[index - 1]);
                    }
                }
            }

            if (InputHelper.KeyUpEvent(this.m_NextVehiclePositionKey))
            {
                this.m_CurrentVehicle.SetNextPlayerPosition();
            }

            foreach (Vehicle vehicle in this.m_VehicleContainer.Vehicles)
            {
                if ((vehicle != this.m_CurrentVehicle) && (!vehicle.AutoPilot.Enabled))
                {
                    vehicle.AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40.0f);
                }
            }
        }
    }
}