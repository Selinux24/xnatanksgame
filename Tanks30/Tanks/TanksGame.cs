using System;
using System.Collections.Generic;
using GameComponents;
using GameComponents.Camera;
using GameComponents.MathComponents;
using GameComponents.Scenery;
using GameComponents.Text;
using GameComponents.Vehicles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics.CollideCoarse;

namespace Tanks
{
    using Tanks.Services;

    /// <summary>
    /// Juego
    /// </summary>
    public class TanksGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Gestor de dispositivos gr�ficos
        /// </summary>
        protected GraphicsDeviceManager Graphics;
        /// <summary>
        /// Controlador de f�sicas
        /// </summary>
        protected PhysicsController Physics = new PhysicsController();
        /// <summary>
        /// Componente de texto
        /// </summary>
        private TextDrawerComponent m_TextDrawer;
        /// <summary>
        /// Terreno
        /// </summary>
        private SceneryGameComponent m_Scenery;
        /// <summary>
        /// Cielo
        /// </summary>
        private SkyBoxGameComponent m_SkyBox;
        /// <summary>
        /// Jugador
        /// </summary>
        private ThirdPersonCameraGameComponent m_Camera;
        /// <summary>
        /// Informaci�n del terreno
        /// </summary>
        private SceneryInfoGameComponent m_Info;
        /// <summary>
        /// Servicio contenedor de veh�culos
        /// </summary>
        private VehicleContainerService m_VehicleContainer;

        /// <summary>
        /// Veh�culo actual
        /// </summary>
        private Vehicle m_CurrentVehicle;

        #region Teclas

        /// <summary>
        /// Tecla que indica cu�ndo hay que dibujar los nodos
        /// </summary>
        private Keys m_ShowNodesDrawnKey = Keys.F1;
        /// <summary>
        /// Tecla que cambia el LOD de los nodos a dibujar
        /// </summary>
        private Keys m_IncLODKey = Keys.F2;
        /// <summary>
        /// Tecla que cambia el LOD de los nodos a dibujar
        /// </summary>
        private Keys m_DecLODKey = Keys.F3;
        /// <summary>
        /// Tecla que activa o desactiva la niebla
        /// </summary>
        private Keys m_FogKey = Keys.F4;
        /// <summary>
        /// Tecla que activa o desactiva la luz global
        /// </summary>
        private Keys m_GlobalLightningKey = Keys.F5;
        /// <summary>
        /// Tecla que activa o desactiva la luz 1
        /// </summary> 
        private Keys m_Light0Key = Keys.F6;
        /// <summary>
        /// Tecla que activa o desactiva la luz 2
        /// </summary>
        private Keys m_Light1Key = Keys.F7;
        /// <summary>
        /// Tecla que activa o desactiva la luz 3
        /// </summary>
        private Keys m_Light2Key = Keys.F8;
        /// <summary>
        /// Tecla que activa o desactiva el wireframe
        /// </summary>
        private Keys m_WireFrameKey = Keys.F12;

        /// <summary>
        /// Tecla que cambia a la siguiente posici�n del veh�culo
        /// </summary>
        private Keys m_NextVehiclePositionKey = Keys.Tab;
        /// <summary>
        /// Cambio al siguiente veh�culo
        /// </summary>
        private Keys m_NextVehicleKey = Keys.Add;
        /// <summary>
        /// Cambio al veh�culo anterior
        /// </summary>
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
            this.Graphics.PreferredBackBufferWidth = 1024;
            this.Graphics.PreferredBackBufferHeight = 600;
            this.Graphics.PreferMultiSampling = false;
            this.Graphics.IsFullScreen = false;
#else
            this.Graphics.PreferredBackBufferWidth = this.GraphicsDevice.DisplayMode.Width;
            this.Graphics.PreferredBackBufferHeight = this.GraphicsDevice.DisplayMode.Height;
            this.Graphics.PreferredBackBufferFormat = this.GraphicsDevice.DisplayMode.Format;
            this.Graphics.PreferMultiSampling = false;
            this.Graphics.IsFullScreen = true;
#endif

            this.Graphics.ApplyChanges();

            InputHelper.GraphicsDevice = this.Graphics.GraphicsDevice;

            this.m_Scenery = new SceneryGameComponent(this);
            this.m_Scenery.UpdateOrder = 0;
            this.m_Scenery.DrawOrder = 0;
            this.Components.Add(this.m_Scenery);
            this.Services.AddService(typeof(SceneryGameComponent), this.m_Scenery);

            this.m_VehicleContainer = new VehicleContainerService(this);
            this.m_VehicleContainer.UpdateOrder = 1;
            this.Services.AddService(typeof(VehicleContainerService), this.m_VehicleContainer);

            this.m_Camera = new ThirdPersonCameraGameComponent(this);
            this.m_Camera.UpdateOrder = 3;
            this.Components.Add(this.m_Camera);

            this.m_SkyBox = new SkyBoxGameComponent(this);
            this.m_SkyBox.UpdateOrder = 4;
            this.m_SkyBox.DrawOrder = 4;
            this.Components.Add(this.m_SkyBox);

            this.m_Info = new SceneryInfoGameComponent(this);
            this.m_Info.UpdateOrder = 5;
            this.m_Info.DrawOrder = 5;
            this.Components.Add(this.m_Info);

            this.m_TextDrawer = new TextDrawerComponent(this);
            this.m_TextDrawer.UpdateOrder = 100;
            this.m_TextDrawer.DrawOrder = 100;
            this.Components.Add(this.m_TextDrawer);

            Vehicle[] squad01 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            Vehicle[] squad02 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            Vehicle[] squad03 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            Vehicle[] squad04 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            Vehicle[] squad05 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            Vehicle[] squad06 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            //Vehicle[] squad04 = AddSquadron(VehicleTypes.LandRaider, 2);
            //Vehicle[] squad05 = AddSquadron(VehicleTypes.LemanRuss, 3);
            //Vehicle[] squad06 = AddSquadron(VehicleTypes.LemanRuss, 3);
            //Vehicle[] squad07 = AddSquadron(VehicleTypes.Rhino, 3);
            //Vehicle[] squad08 = AddSquadron(VehicleTypes.Rhino, 3);
            //Vehicle[] squad09 = AddSquadron(VehicleTypes.Rhino, 3);

            base.Initialize();

            this.InitializeSquadron(squad01);
            this.InitializeSquadron(squad02);
            this.InitializeSquadron(squad03);
            this.InitializeSquadron(squad04);
            this.InitializeSquadron(squad05);
            this.InitializeSquadron(squad06);
            //this.InitializeSquadron(squad07);
            //this.InitializeSquadron(squad08);
            //this.InitializeSquadron(squad09);

            squad01[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            squad02[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            squad03[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            squad04[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            squad05[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            squad06[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            //squad07[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            //squad08[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            //squad09[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);

            this.SetFocus(squad01[2]);
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

            this.Physics.Update(gameTime);

            base.Update(gameTime);

            this.UpdateEnvironment(gameTime);

            this.UpdateVehicles(gameTime);

            string text = "Vehiculo  " + this.m_CurrentVehicle.Position.ToString() + Environment.NewLine;
            text += "Velocidad " + this.m_CurrentVehicle.Velocity.ToString() + Environment.NewLine;
            text += "Piloto    " + this.m_CurrentVehicle.AutoPilot.Enabled.ToString() + " " + this.m_CurrentVehicle.AutoPilot.Target.ToString() + " " + this.m_CurrentVehicle.AutoPilot.MaximumVelocity.ToString() + Environment.NewLine;
            text += "En Rango  " + this.m_CurrentVehicle.AutoPilot.OnRange.ToString() + " " + this.m_CurrentVehicle.AutoPilot.DistanceToTarget.ToString() + Environment.NewLine;
            if (this.m_Scenery != null)
            {
                text += "Terreno   " + this.m_Scenery.Scenery.Center.ToString() + Environment.NewLine;
            }
            if (this.m_Camera.ModelToFollow != null)
            {
                text += "Camara    " + this.m_Camera.Position.ToString() + Environment.NewLine;
            }

            this.m_TextDrawer.WriteText(text, 5, 5);

            InputHelper.End();
        }
        /// <summary>
        /// Dibujar los componentes del juego
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected override void Draw(GameTime gameTime)
        {
            this.Graphics.GraphicsDevice.Clear(SceneryEnvironment.Ambient.AmbientColor);

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

                // Indicar que la c�mara no seguir� a ning�n tanque
                this.m_Camera.ModelToFollow = null;
            }

            if (tank != null)
            {
                // Indicar que la c�mara debe seguir al tanque seleccionado
                this.m_Camera.ModelToFollow = tank;

                // Establecer el foco en el tanque
                tank.HasFocus = true;

                // Actualizar el marcador de tanque actual
                this.m_CurrentVehicle = tank;
            }
        }
        /// <summary>
        /// A�ade un escuadr�n del tipo y la cantidad especificada
        /// </summary>
        /// <param name="type">Tipo de escuadr�n</param>
        /// <param name="count">Cantidad de veh�culos</param>
        private Vehicle[] AddSquadron(VehicleTypes type, int count)
        {
            // Lista de componentes creados
            List<Vehicle> vehicleList = new List<Vehicle>();

            for (int i = 0; i < count; i++)
            {
                if (type == VehicleTypes.Rhino)
                {
                    vehicleList.Add(m_VehicleContainer.AddRhino(Point.Zero));
                }
                else if (type == VehicleTypes.LandSpeeder)
                {
                    vehicleList.Add(m_VehicleContainer.AddLandSpeeder(Point.Zero));
                }
                else if (type == VehicleTypes.LandRaider)
                {
                    vehicleList.Add(m_VehicleContainer.AddLandRaider(Point.Zero));
                }
                else if (type == VehicleTypes.LemanRuss)
                {
                    vehicleList.Add(m_VehicleContainer.AddLemanRuss(Point.Zero));
                }
            }

            return vehicleList.ToArray();
        }
        /// <summary>
        /// Inicializa el escuadron especificado
        /// </summary>
        /// <param name="vehicleList">Lista de veh�culos</param>
        private void InitializeSquadron(Vehicle[] vehicleList)
        {
            // Punto de posici�n del escuadr�n
            Vector3 where = new Vector3(RandomComponent.Next(5000) + 5000, 600, RandomComponent.Next(5000) + 5000);

            vehicleList[0].Position = where;

            this.Physics.RegisterVehicle(vehicleList[0]);

            for (int i = 1; i < vehicleList.Length; i++)
            {
                // Posici�n de cada tanque relativa al anterior
                vehicleList[i].Position = vehicleList[i - 1].Position - Vector3.Multiply(Vector3.One, 10f);
                // Indicar a cada tanque que siga al anterior
                vehicleList[i].AutoPilot.Follow(vehicleList[i - 1], 150f);

                this.Physics.RegisterVehicle(vehicleList[i]);
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
                m_Info.Visible = !m_Info.Visible;
            }

            if (InputHelper.KeyUpEvent(m_IncLODKey))
            {
                if (m_Info.Lod == LOD.None)
                {
                    m_Info.Lod = LOD.High;
                    m_Scenery.Scenery.LevelOfDetail = LOD.High;
                }
                else if (m_Info.Lod == LOD.High)
                {
                    m_Info.Lod = LOD.Medium;
                    m_Scenery.Scenery.LevelOfDetail = LOD.Medium;
                }
                else if (m_Info.Lod == LOD.Medium)
                {
                    m_Info.Lod = LOD.Low;
                    m_Scenery.Scenery.LevelOfDetail = LOD.Low;
                }
                else if (m_Info.Lod == LOD.Low)
                {
                    m_Info.Lod = LOD.None;
                    m_Scenery.Scenery.LevelOfDetail = LOD.None;
                }
            }

            if (InputHelper.KeyUpEvent(m_DecLODKey))
            {
                if (m_Info.Lod == LOD.Medium)
                {
                    m_Info.Lod = LOD.High;
                    m_Scenery.Scenery.LevelOfDetail = LOD.High;
                }
                else if (m_Info.Lod == LOD.Low)
                {
                    m_Info.Lod = LOD.Medium;
                    m_Scenery.Scenery.LevelOfDetail = LOD.Medium;
                }
                else if (m_Info.Lod == LOD.None)
                {
                    m_Info.Lod = LOD.Low;
                    m_Scenery.Scenery.LevelOfDetail = LOD.Low;
                }
                else if (m_Info.Lod == LOD.High)
                {
                    m_Info.Lod = LOD.None;
                    m_Scenery.Scenery.LevelOfDetail = LOD.None;
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
                if (!vehicle.Destroyed)
                {
                    if (vehicle != this.m_CurrentVehicle)
                    {
                        if (!vehicle.AutoPilot.Enabled)
                        {
                            if (vehicle.AutoPilot.Following)
                            {
                                vehicle.AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 150.0f);
                            }
                            else
                            {
                                vehicle.AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60.0f);
                            }
                        }
                    }
                }
            }
        }
    }
}