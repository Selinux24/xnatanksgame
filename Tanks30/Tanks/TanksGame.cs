using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tanks
{
    using Common.Drawing;
    using Common.Helpers;
    using GameComponents.Buildings;
    using GameComponents.Camera;
    using GameComponents.MathComponents;
    using GameComponents.Particles;
    using GameComponents.Scenery;
    using GameComponents.Text;
    using GameComponents.Vehicles;
    using GameComponents.Weapons;
    using Physics;
    using Physics.CollideCoarse;
    using Tanks.Services;

    /// <summary>
    /// Juego
    /// </summary>
    public class TanksGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Gestor de dispositivos gráficos
        /// </summary>
        protected GraphicsDeviceManager Graphics;
        /// <summary>
        /// Controlador de físicas
        /// </summary>
        protected PhysicsController Physics = null;
        /// <summary>
        /// Gestor de partículas
        /// </summary>
        private ParticleManager m_ParticleManager = null;
        /// <summary>
        /// Componente de texto
        /// </summary>
        private TextDrawerComponent m_TextDrawer;
        /// <summary>
        /// Terreno
        /// </summary>
        private SceneryGameComponent m_Scenery;
        /// <summary>
        /// Reflejo solar
        /// </summary>
        private LensFlareComponent m_LensFlare;
        /// <summary>
        /// Cámara
        /// </summary>
        private CameraGameComponent m_Camera;
        /// <summary>
        /// Información del terreno
        /// </summary>
        private SceneryInfoGameComponent m_Info;
        /// <summary>
        /// Servicio contenedor de vehículos
        /// </summary>
        private VehicleContainerService m_VehicleContainer;
        /// <summary>
        /// Servicio contenedor de edificios
        /// </summary>
        private BuildingContainerService m_BuilngContainer;

        /// <summary>
        /// Vehículo actual
        /// </summary>
        private Vehicle m_CurrentVehicle;
        /// <summary>
        /// Último vehículo que ha tenido el foco
        /// </summary>
        private Vehicle m_PreviousVehicle;

        #region Teclas

        /// <summary>
        /// Tecla que indica cuándo hay que dibujar los nodos
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
        /// Tecla que activa o desactiva el wireframe
        /// </summary>
        private Keys m_WireFrameKey = Keys.F12;

        /// <summary>
        /// Tecla que cambia a la siguiente posición del vehículo
        /// </summary>
        private Keys m_NextVehiclePositionKey = Keys.Tab;
        /// <summary>
        /// Cambio al siguiente vehículo
        /// </summary>
        private Keys m_NextVehicleKey = Keys.Add;
        /// <summary>
        /// Cambio al vehículo anterior
        /// </summary>
        private Keys m_PreviousVehicleKey = Keys.Subtract;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TanksGame()
        {
            this.Graphics = new GraphicsDeviceManager(this);

            this.Physics = new PhysicsController();
            this.Physics.InitializeProyectiles(100);
            this.Services.AddService(typeof(PhysicsController), this.Physics);

            this.Physics.OnExplosionUpdated += new ExplosionHandler(Physics_OnExplosionUpdated);
            this.Physics.OnProjectileMoved += new AmmoRoundHandler(Physics_OnProjectileMoved);
            this.Physics.OnVehicleMoved += new VehicleHandler(Physics_OnVehicleMoved);
        }

        /// <summary>
        /// Evento de vehículo en movimiento
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        void Physics_OnVehicleMoved(IPhysicObject vehicle)
        {

        }
        /// <summary>
        /// Evento de explosión activa
        /// </summary>
        /// <param name="explosion">Explosión</param>
        void Physics_OnExplosionUpdated(Explosion explosion)
        {
            this.m_ParticleManager.AddParticle(ParticleSystemTypes.Explosion, explosion.DetonationCenter, Vector3.Up * 0.5f);
            this.m_ParticleManager.AddParticle(ParticleSystemTypes.ExplosionSmoke, explosion.DetonationCenter, Vector3.Up * 0.5f);
        }
        /// <summary>
        /// Evento de proyectil en movimiento
        /// </summary>
        /// <param name="obj">Objeto</param>
        /// <param name="position">Posición</param>
        /// <param name="velocity">Velocidad</param>
        void Physics_OnProjectileMoved(AmmoRound ammo)
        {
            this.m_ParticleManager.AddParticle(ParticleSystemTypes.ProjectileTrail, ammo.Position, Vector3.Zero);
        }
        /// <summary>
        /// Se dispara cuando un vehículo ha sido destruído
        /// </summary>
        /// <param name="sender">Vehículo</param>
        /// <param name="e">Argumentos</param>
        void Vehicle_OnVehicleDestroyed(IVehicle sender)
        {
            if (sender != null)
            {
                this.m_ParticleManager.AddParticleGenerator(ParticleSystemTypes.Fire, sender, 15f);
                this.m_ParticleManager.AddParticleGenerator(ParticleSystemTypes.SmokePlume, sender, 60f);
            }
        }
        /// <summary>
        /// Se dispara cuando un vehículo ha sido dañado
        /// </summary>
        /// <param name="sender">Vehículo</param>
        /// <param name="e">Argumentos</param>
        void Vehicle_OnVehicleHeavyDamaged(IVehicle sender)
        {
            if (sender != null)
            {
                this.m_ParticleManager.AddParticleGenerator(ParticleSystemTypes.SmokePlume, sender, 15f);
            }
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

            InputHelper.Initialize(this.GraphicsDevice);

#if DEBUG
            GameComponents.Debug.DebugDrawer.Initialize(this.GraphicsDevice);
#endif
            this.m_Scenery = new SceneryGameComponent(this);
            this.m_Scenery.UpdateOrder = 0;
            this.m_Scenery.DrawOrder = 0;
            this.Components.Add(this.m_Scenery);
            this.Services.AddService(typeof(SceneryGameComponent), this.m_Scenery);

            this.m_VehicleContainer = new VehicleContainerService(this);
            this.m_VehicleContainer.UpdateOrder = 1;
            this.Services.AddService(typeof(VehicleContainerService), this.m_VehicleContainer);

            this.m_BuilngContainer = new BuildingContainerService(this);
            this.m_BuilngContainer.UpdateOrder = 2;
            this.Services.AddService(typeof(BuildingContainerService), this.m_BuilngContainer);

            AmmoDrawer ammoDrawer = new AmmoDrawer(this, @"Content/Steel 1");
            ammoDrawer.Rounds = this.Physics.Projectiles;
            ammoDrawer.UpdateOrder = 3;
            ammoDrawer.DrawOrder = 3;
            this.Components.Add(ammoDrawer);

            Building[] buildings = AddBuildings(BuildingTypes.Type0, 10);

            Vehicle[] squad01 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            //Vehicle[] squad02 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            //Vehicle[] squad03 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            //Vehicle[] squad04 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            //Vehicle[] squad05 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            //Vehicle[] squad06 = AddSquadron(VehicleTypes.LandSpeeder, 3);
            //Vehicle[] squad04 = AddSquadron(VehicleTypes.LandRaider, 2);
            Vehicle[] squad05 = AddSquadron(VehicleTypes.LemanRuss, 3);
            //Vehicle[] squad06 = AddSquadron(VehicleTypes.LemanRuss, 3);
            Vehicle[] squad07 = AddSquadron(VehicleTypes.Rhino, 3);
            //Vehicle[] squad08 = AddSquadron(VehicleTypes.Rhino, 3);
            //Vehicle[] squad09 = AddSquadron(VehicleTypes.Rhino, 3);

            this.m_LensFlare = new LensFlareComponent(this);
            this.m_LensFlare.UpdateOrder = 96;
            this.m_LensFlare.DrawOrder = 96;
            this.Components.Add(this.m_LensFlare);

            this.m_Camera = new CameraGameComponent(this);
            this.m_Camera.UseMouse = true;
            this.m_Camera.Mode = CameraGameComponent.CameraModes.FirstPerson;
            this.m_Camera.UpdateOrder = 98;
            this.Components.Add(this.m_Camera);

            this.m_Info = new SceneryInfoGameComponent(this);
            this.m_Info.UpdateOrder = 99;
            this.m_Info.DrawOrder = 99;
            this.Components.Add(this.m_Info);

            this.m_TextDrawer = new TextDrawerComponent(this);
            this.m_TextDrawer.UpdateOrder = 100;
            this.m_TextDrawer.DrawOrder = 100;
            this.Components.Add(this.m_TextDrawer);

            this.m_ParticleManager = new ParticleManager(this);
            this.Components.Add(this.m_ParticleManager);

            base.Initialize();

            this.Physics.RegisterScenery(this.m_Scenery.Scenery);

            this.InitializeBuildings(buildings);

            this.InitializeSquadron(squad01);
            //this.InitializeSquadron(squad02);
            //this.InitializeSquadron(squad03);
            //this.InitializeSquadron(squad04);
            this.InitializeSquadron(squad05);
            //this.InitializeSquadron(squad06);
            this.InitializeSquadron(squad07);
            //this.InitializeSquadron(squad08);
            //this.InitializeSquadron(squad09);

            this.SetFocus(squad01[0]);

            //squad01[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            //squad02[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            //squad03[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            //squad04[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            //squad05[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            //squad06[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60f);
            //squad07[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            //squad08[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
            //squad09[0].AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 40f);
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

            this.UpdateCamera();

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

            text += "Camara    " + this.m_Camera.GetStatus() + Environment.NewLine;

            this.m_TextDrawer.WriteText(text, 5, 5);

            InputHelper.End();
        }
        /// <summary>
        /// Dibujar los componentes del juego
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected override void Draw(GameTime gameTime)
        {
            this.Graphics.GraphicsDevice.Clear(SceneryEnvironment.Ambient.AtmosphericColor);

            this.Graphics.GraphicsDevice.RenderState.FogColor = SceneryEnvironment.Ambient.AtmosphericColor;
            this.Graphics.GraphicsDevice.RenderState.FogTableMode = FogMode.Linear;
            this.Graphics.GraphicsDevice.RenderState.FogStart = SceneryEnvironment.LevelOfDetail.HighFarClip;
            this.Graphics.GraphicsDevice.RenderState.FogEnd = SceneryEnvironment.LevelOfDetail.LowFarClip;
            this.Graphics.GraphicsDevice.RenderState.FogDensity = 0.5f;
            this.Graphics.GraphicsDevice.RenderState.FogEnable = true;

            base.Draw(gameTime);
        }

        /// <summary>
        /// Establece el foco en el vehículo designado
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        private void SetFocus(Vehicle vehicle)
        {
            if (this.m_CurrentVehicle != null)
            {
                this.m_PreviousVehicle = this.m_CurrentVehicle;

                // Quitar el foco del vehículo anterior
                this.m_CurrentVehicle.HasFocus = false;

                // Indicar que la cámara no seguirá a ningún vehículo
                this.m_Camera.ModelToFollow = null;
            }

            if (vehicle != null)
            {
                // Indicar que la cámara debe seguir al vehículo seleccionado
                this.m_Camera.ModelToFollow = vehicle;

                // Establecer el foco en el vehículo
                vehicle.HasFocus = true;

                // Actualizar el marcador de vehículo actual
                this.m_CurrentVehicle = vehicle;
            }
        }

        /// <summary>
        /// Añadir una lista de edificios
        /// </summary>
        /// <param name="type">Tipo de edificios</param>
        /// <param name="count">Cantidad</param>
        /// <returns>Devuelve la lista de edificios</returns>
        private Building[] AddBuildings(BuildingTypes type, int count)
        {
            // Lista de componentes creados
            List<Building> buildingList = new List<Building>();

            for (int i = 0; i < count; i++)
            {
                if (type == BuildingTypes.Type0)
                {
                    buildingList.Add(m_BuilngContainer.AddType0(Point.Zero));
                }
            }

            return buildingList.ToArray();
        }
        /// <summary>
        /// Inicializa el estado de los edificios
        /// </summary>
        /// <param name="buildings">Lista de edificios</param>
        private void InitializeBuildings(Building[] buildings)
        {
            // Punto de posición del edificio
            foreach (Building building in buildings)
            {
                Vector3 where = new Vector3(RandomComponent.Next(5000) + 5000, 600, RandomComponent.Next(5000) + 5000);

                float? h = this.m_Scenery.Scenery.GetHeigthAtPoint(where.X, where.Z);
                if (h.HasValue)
                {
                    where.Y = h.Value + 1f;
                }

                building.SetInitialState(where, Quaternion.Identity);

                this.Physics.RegisterObject(building);
            }
        }
        /// <summary>
        /// Añade un escuadrón del tipo y la cantidad especificada
        /// </summary>
        /// <param name="type">Tipo de escuadrón</param>
        /// <param name="count">Cantidad de vehículos</param>
        private Vehicle[] AddSquadron(VehicleTypes type, int count)
        {
            // Lista de componentes creados
            List<Vehicle> vehicleList = new List<Vehicle>();

            for (int i = 0; i < count; i++)
            {
                Vehicle newVehicle = null;

                if (type == VehicleTypes.Rhino)
                {
                    newVehicle = m_VehicleContainer.AddRhino(Point.Zero);
                }
                else if (type == VehicleTypes.LandSpeeder)
                {
                    newVehicle = m_VehicleContainer.AddLandSpeeder(Point.Zero);
                }
                else if (type == VehicleTypes.LandRaider)
                {
                    newVehicle = m_VehicleContainer.AddLandRaider(Point.Zero);
                }
                else if (type == VehicleTypes.LemanRuss)
                {
                    newVehicle = m_VehicleContainer.AddLemanRuss(Point.Zero);
                }

                if (newVehicle != null)
                {
                    newVehicle.OnVehicleHeavyDamaged += new VehicleStateHandler(Vehicle_OnVehicleHeavyDamaged);
                    newVehicle.OnVehicleDestroyed += new VehicleStateHandler(Vehicle_OnVehicleDestroyed);

                    vehicleList.Add(newVehicle);
                }
            }

            return vehicleList.ToArray();
        }
        /// <summary>
        /// Inicializa el escuadron especificado
        /// </summary>
        /// <param name="squadron">Escuadrón de vehículos</param>
        private void InitializeSquadron(Vehicle[] squadron)
        {
            // Punto de posición del escuadrón
            Vector3 where = new Vector3(RandomComponent.Next(5000) + 5000, 600, RandomComponent.Next(5000) + 5000);

            float? h = this.m_Scenery.Scenery.GetHeigthAtPoint(where.X, where.Z);
            if (h.HasValue)
            {
                where.Y = h.Value + 1f;
            }

            squadron[0].SetInitialState(where, Quaternion.Identity);

            this.Physics.RegisterObject(squadron[0]);

            for (int i = 1; i < squadron.Length; i++)
            {
                // Posición de cada vehículo relativa al anterior
                Vector3 rWhere = squadron[i - 1].Position - Vector3.Multiply(Vector3.One, 10f);

                float? rh = this.m_Scenery.Scenery.GetHeigthAtPoint(rWhere.X, rWhere.Z);
                if (rh.HasValue)
                {
                    rWhere.Y = rh.Value + 1f;
                }

                squadron[i].SetInitialState(rWhere, Quaternion.Identity);
                // Indicar a cada vehículo que siga al anterior
                //squadron[i].AutoPilot.Follow(squadron[i - 1], 150f);

                this.Physics.RegisterObject(squadron[i]);
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
        /// Actualiza la cámara
        /// </summary>
        private void UpdateCamera()
        {
            // Cambio de cámara
            if (InputHelper.KeyUpEvent(Keys.C))
            {
                if (this.m_Camera.Mode != CameraGameComponent.CameraModes.Free)
                {
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.Free;

                    CameraGameComponent.KeyBoardSensibility = 1000f;

                    this.SetFocus(null);
                }
                else
                {
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.FirstPerson;

                    CameraGameComponent.KeyBoardSensibility = 100f;

                    this.SetFocus(this.m_PreviousVehicle);
                }
            }

            if (this.m_Camera.Mode != CameraGameComponent.CameraModes.Free)
            {
                if (InputHelper.IsKeyDown(Keys.LeftControl))
                {
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.Chase;
                }
                else
                {
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.FirstPerson;
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
                this.m_CurrentVehicle.SetNextPlayerControl();
            }

            //foreach (Vehicle vehicle in this.m_VehicleContainer.Vehicles)
            //{
            //    if (!vehicle.Destroyed)
            //    {
            //        if (vehicle != this.m_CurrentVehicle)
            //        {
            //            if (!vehicle.AutoPilot.Enabled)
            //            {
            //                if (vehicle.AutoPilot.Following)
            //                {
            //                    vehicle.AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 150.0f);
            //                }
            //                else
            //                {
            //                    vehicle.AutoPilot.GoTo(new Vector3(RandomComponent.Next(10000), 0, RandomComponent.Next(10000)), 60.0f);
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}