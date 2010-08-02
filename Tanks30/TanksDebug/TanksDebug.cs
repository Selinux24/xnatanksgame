using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TanksDebug
{
    using Common;
    using Common.Helpers;
    using GameComponents.Camera;
    using GameComponents.Vehicles;
    using GameComponents.Weapons;
    using Physics.CollideCoarse;
    using Vehicles;
    using GameComponents.Text;
    using Buildings;
    using GameComponents.Scenery;
    using Physics;
    using GameComponents.Particles;

    /// <summary>
    /// Demostración de disparos
    /// </summary>
    class BigBallisticDemo : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Traslación global
        /// </summary>
        private readonly Vector3 GlobalTraslation = new Vector3(0, 0, 0);

        /// <summary>
        /// Número máximo de balas en la simulación
        /// </summary>
        private const int _AmmoRounds = 100;
        /// <summary>
        /// Número de cajas en la simulación
        /// </summary>
        private const int _Boxes = 20;
        /// <summary>
        /// Número de bolas en la simulación
        /// </summary>
        private const int _Balls = 10;
        /// <summary>
        /// Tamaño del terreno
        /// </summary>
        private const float _TerrainSize = 300f;

        private Color _AmbientColor = SceneryEnvironment.Ambient.AmbientColor;

        /// <summary>
        /// Dispositivo gráfico
        /// </summary>
        protected GraphicsDeviceManager Graphics;
        /// <summary>
        /// Controlador de físicas
        /// </summary>
        protected PhysicsController Physics;

        /// <summary>
        /// Jugador
        /// </summary>
        private CameraGameComponent m_Camera;
        /// <summary>
        /// Componente de texto
        /// </summary>
        private TextDrawerComponent m_Text;
        /// <summary>
        /// Colección de cajas para dibujar
        /// </summary>
        private List<CubeGameComponent> m_Cubes = new List<CubeGameComponent>();
        /// <summary>
        /// Colección de esferas para dibujar
        /// </summary>
        private List<BallGameComponent> m_Balls = new List<BallGameComponent>();
        /// <summary>
        /// Lentes
        /// </summary>
        private LensFlareComponent m_LensFlare = null;
        /// <summary>
        /// Edificio
        /// </summary>
        private BuildingType0 m_Building_1 = null;
        /// <summary>
        /// Edificio
        /// </summary>
        private BuildingType0 m_Building_2 = null;
        /// <summary>
        /// Edificio
        /// </summary>
        private BuildingType0 m_Building_3 = null;
        /// <summary>
        /// El tanque 1
        /// </summary>
        private Rhino m_Rhino_1 = null;
        /// <summary>
        /// El tanque 2
        /// </summary>
        private Rhino m_Rhino_2 = null;
        /// <summary>
        /// Land speeder 1
        /// </summary>
        private LandSpeeder m_LandSpeeder_1 = null;
        /// <summary>
        /// Land speeder 2
        /// </summary>
        private LandSpeeder m_LandSpeeder_2 = null;

        /// <summary>
        /// Vehículo actual
        /// </summary>
        private Vehicle m_CurrentVehicle = null;

        private ParticleManager m_ParticleManager = null;

        private BallGameComponent ball1 = null;
        //private BallGameComponent ball2 = null;
        //private BallGameComponent ball3 = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public BigBallisticDemo()
            : base()
        {
            this.Graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "";

            this.Physics = new PhysicsController();
            this.Physics.InitializeProyectiles(_AmmoRounds);
            this.Services.AddService(typeof(PhysicsController), this.Physics);

            this.Physics.OnExplosionUpdated += new ExplosionHandler(Physics_OnExplosionUpdated);
            this.Physics.OnProjectileMoved += new ObjectMovedHandler(Physics_OnProjectileMoved);
        }

        void Physics_OnProjectileMoved(IPhysicObject obj, Vector3 position, Vector3 velocity)
        {
            this.m_ParticleManager.AddProjectileTrailParticle(position, velocity);
        }

        void Physics_OnExplosionUpdated(Explosion explosion)
        {
            this.m_ParticleManager.AddExplosionParticle(explosion.DetonationCenter, Vector3.Up * 0.5f);
            this.m_ParticleManager.AddExplosionSmokeParticle(explosion.DetonationCenter, Vector3.Up * 0.5f);
        }

        /// <summary>
        /// Inicialización
        /// </summary>
        protected override void Initialize()
        {
            // Inicializar el control de entrada de usuario
            InputHelper.Initialize(this.GraphicsDevice);

            // Actualizar la proyección
            GlobalMatrices.UpdateProjection(this.GraphicsDevice);
#if DEBUG
            // Debug
            GameComponents.Debug.DebugDrawer.Initialize(this.GraphicsDevice);
#endif

            // Suelo
            DemoScenery scn = new DemoScenery(this, _TerrainSize, GlobalTraslation, @"Content/dharma");
            scn.UpdateOrder = 0;
            this.Components.Add(scn);
            this.Physics.RegisterScenery(scn);

            // Reflejo solar
            this.m_LensFlare = new LensFlareComponent(this);
            this.m_LensFlare.DrawOrder = 98;
            this.Components.Add(this.m_LensFlare);

            // Partículas
            this.m_ParticleManager = new ParticleManager(this);
            this.Components.Add(this.m_ParticleManager);

            // Texto
            this.m_Text = new TextDrawerComponent(this);
            this.m_Text.DrawOrder = 99;
            this.Components.Add(this.m_Text);

            // Cámara
            this.m_Camera = new CameraGameComponent(this);
            this.m_Camera.UseMouse = true;
            this.m_Camera.Mode = CameraGameComponent.CameraModes.FirstPerson;
            this.m_Camera.UpdateOrder = 99;
            this.Components.Add(this.m_Camera);

            // Un edificio
            this.m_Building_1 = new BuildingType0(this, @"Content/Buildings/", @"WHBuilding01.xml");
            this.m_Building_1.UpdateOrder = 3;
            this.Components.Add(this.m_Building_1);
            this.Physics.RegisterVehicle(this.m_Building_1);

            // Otro edificio
            this.m_Building_2 = new BuildingType0(this, @"Content/Buildings/", @"WHBuilding02.xml");
            this.m_Building_2.UpdateOrder = 3;
            this.Components.Add(this.m_Building_2);
            this.Physics.RegisterVehicle(this.m_Building_2);

            // Otro edificio
            this.m_Building_3 = new BuildingType0(this, @"Content/Buildings/", @"WHBuilding03.xml");
            this.m_Building_3.UpdateOrder = 3;
            this.Components.Add(this.m_Building_3);
            this.Physics.RegisterVehicle(this.m_Building_3);

            AmmoDrawer ammoDrawer = new AmmoDrawer(this, @"Content/dharma");
            ammoDrawer.Rounds = this.Physics.Projectiles;
            ammoDrawer.UpdateOrder = 3;
            this.Components.Add(ammoDrawer);

            Random rnd = new Random(DateTime.Now.Millisecond);

            // Inicializa las cajas
            for (int i = 0; i < _Boxes; i++)
            {
                // Inicializa los componentes gráficos de las cajas
                float hsX = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                float hsY = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                float hsZ = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                Vector3 min = new Vector3(-hsX, -hsY, -hsZ);
                Vector3 max = new Vector3(hsX, hsY, hsZ);

                CubeGameComponent cube = new CubeGameComponent(this, min, max, (max - min).Length() * 20f);
                this.Physics.RegisterVehicle(cube);
                this.m_Cubes.Add(cube);
                cube.UpdateOrder = 4;
                this.Components.Add(cube);
            }

            // Inicializa las esferas
            for (int i = 0; i < _Balls; i++)
            {
                // Inicializa los componentes gráficos de las esferas
                float radius = 0.5f + ((float)rnd.NextDouble() * 1.5f);

                BallGameComponent ball = new BallGameComponent(this, radius, radius);
                this.Physics.RegisterVehicle(ball);
                this.m_Balls.Add(ball);
                ball.UpdateOrder = 4;
                this.Components.Add(ball);
            }

            // El tanque del jugador 1
            this.m_Rhino_1 = new Rhino(this);
            this.m_Rhino_1.UpdateOrder = 5;
            this.Components.Add(this.m_Rhino_1);
            this.Physics.RegisterVehicle(m_Rhino_1);

            // El tanque del jugador 2
            this.m_Rhino_2 = new Rhino(this);
            this.m_Rhino_2.UpdateOrder = 5;
            this.Components.Add(this.m_Rhino_2);
            this.Physics.RegisterVehicle(m_Rhino_2);

            // Land speeder 1
            this.m_LandSpeeder_1 = new LandSpeeder(this);
            this.m_LandSpeeder_1.UpdateOrder = 5;
            this.Components.Add(this.m_LandSpeeder_1);
            this.Physics.RegisterVehicle(m_LandSpeeder_1);

            // Land speeder 2
            this.m_LandSpeeder_2 = new LandSpeeder(this);
            this.m_LandSpeeder_2.UpdateOrder = 5;
            this.Components.Add(this.m_LandSpeeder_2);
            this.Physics.RegisterVehicle(m_LandSpeeder_2);

            // Prueba
            this.ball1 = new BallGameComponent(this, 1f, 1f);
            this.Components.Add(ball1);
            this.Physics.RegisterVehicle(ball1);

            //this.ball2 = new BallGameComponent(this, 1.5f, 2f);
            //this.Components.Add(ball2);
            //this.Physics.RegisterVehicle(ball2);

            //this.ball3 = new BallGameComponent(this, 2f, 4f);
            //this.Components.Add(ball3);
            //this.Physics.RegisterVehicle(ball3);

            //RodComponent rod1 = new RodComponent(this, this.ball1, Vector3.Up * this.ball1.Sphere.Radius, null, new Vector3(0, 40, 0), 10f);
            //this.Components.Add(rod1);
            //this.Physics.RegisterContactGenerator(rod1.Rod);

            //RodComponent rod2 = new RodComponent(this, this.ball2, Vector3.Up * this.ball2.Sphere.Radius, null, new Vector3(0, 40, 0), 10f);
            //this.Components.Add(rod2);
            //this.Physics.RegisterContactGenerator(rod2.Rod);

            //JointComponent joint1 = new JointComponent(this, this.ball1, Vector3.Down * this.ball1.Sphere.Radius, this.ball3, Vector3.Left * this.ball3.Sphere.Radius, 5f);
            //this.Components.Add(joint1);
            //this.Physics.RegisterContactGenerator(joint1.Joint);

            //JointComponent joint2 = new JointComponent(this, this.ball2, Vector3.Down * this.ball2.Sphere.Radius, this.ball3, Vector3.Right * this.ball3.Sphere.Radius, 5f);
            //this.Components.Add(joint2);
            //this.Physics.RegisterContactGenerator(joint2.Joint);

            Joint2Component rod24 = new Joint2Component(this, this.ball1, Vector3.Up * this.ball1.Radius, null, new Vector3(0, 40, 0), 10f);
            this.Components.Add(rod24);
            this.Physics.RegisterContactGenerator(rod24.Rod);

            // Establecer el foco en el vehículo
            this.SetFocus(this.m_Rhino_1);

            base.Initialize();

            this.Reset();
        }
        /// <summary>
        /// Actualiza la aplicación
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected override void Update(GameTime gameTime)
        {
            // Fin de captura de salida de usuario
            InputHelper.Begin(gameTime);

            // Salir
            if (InputHelper.KeyUpEvent(Keys.Escape))
            {
                this.Exit();
            }

            // Reinicio
            if (InputHelper.KeyUpEvent(Keys.R))
            {
                this.Reset();
            }

            // Cámara
            this.UpdateCamera(gameTime);

            // Vehículos
            this.UpdateVehicles(gameTime);

            // Físicas
            this.Physics.Update(gameTime);

            //this.car.Integrate(gameTime);

            // Actualización base
            base.Update(gameTime);

            // Fin de captura de entrada de usuario
            InputHelper.End();
        }
        /// <summary>
        /// Dibujado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected override void Draw(GameTime gameTime)
        {
            this.Graphics.GraphicsDevice.Clear(_AmbientColor);

            this.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            this.GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            this.GraphicsDevice.RenderState.FogColor = _AmbientColor;
            this.GraphicsDevice.RenderState.FogTableMode = FogMode.Linear;
            this.GraphicsDevice.RenderState.FogStart = 75;
            this.GraphicsDevice.RenderState.FogEnd = 500;
            this.GraphicsDevice.RenderState.FogDensity = 0.25f;
            this.GraphicsDevice.RenderState.FogEnable = true;

            this.m_Text.WriteText("Contactos en uso: " + this.Physics.UsedContacts.ToString(), 5, 5, Color.Red);

            base.Draw(gameTime);
        }
        /// <summary>
        /// Inicializar la posición de los cuerpos
        /// </summary>
        private void Reset()
        {
            this.Physics.Reset();

            float tankArea = _TerrainSize * 0.9f * 0.5f;

            // Edificio
            m_Building_1.SetInitialState(new Vector3(tankArea * 0.75f, 30f, tankArea * 0.75f) + GlobalTraslation, Quaternion.Identity);

            // Edificio
            m_Building_2.SetInitialState(new Vector3(-tankArea * 0.75f, 30f, -tankArea * 0.75f) + GlobalTraslation, Quaternion.Identity);

            // Edificio
            m_Building_3.SetInitialState(new Vector3(-tankArea * 0.65f, 30f, -tankArea * 0.85f) + GlobalTraslation, Quaternion.Identity);

            // Tanque 1
            m_Rhino_1.SetInitialState(new Vector3(tankArea, 20f, tankArea) + GlobalTraslation, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(45f)));

            // Tanque 2
            m_Rhino_2.SetInitialState(new Vector3(-tankArea, 20f, -tankArea) + GlobalTraslation, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(225f)));

            // Land Speeder 1
            m_LandSpeeder_1.SetInitialState(new Vector3(-10, 60f, 10) + GlobalTraslation, Quaternion.Identity);

            // Land Speeder 2
            m_LandSpeeder_2.SetInitialState(new Vector3(10, 60f, -10) + GlobalTraslation, Quaternion.Identity);

            float objectArea = _TerrainSize * 0.8f * 0.5f;
            Random rnd = new Random(DateTime.Now.Millisecond);

            // Inicializar las cajas
            foreach (CubeGameComponent box in this.m_Cubes)
            {
                float x = ((float)rnd.NextDouble() * objectArea * 2f) - objectArea;
                float y = 50f;
                float z = ((float)rnd.NextDouble() * objectArea * 2f) - objectArea;
                float yaw = (float)rnd.NextDouble() * 0.5f;
                float pitch = (float)rnd.NextDouble() * 0.5f;
                float roll = (float)rnd.NextDouble() * 0.5f;

                box.SetState(new Vector3(x, y, z) + GlobalTraslation, Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll));
            }

            // Inicializar las esferas
            foreach (BallGameComponent ball in this.m_Balls)
            {
                float x = ((float)rnd.NextDouble() * objectArea * 2f) - objectArea;
                float y = 50f;
                float z = ((float)rnd.NextDouble() * objectArea * 2f) - objectArea;

                ball.SetPosition(new Vector3(x, y, z) + GlobalTraslation);
            }

            //this.ball1.SetPosition(new Vector3(120, 30, 120));
            this.ball1.SetPosition(new Vector3(5, 35, 5));
            //this.ball2.SetPosition(new Vector3(-15, 39, -20));
            //this.ball3.SetPosition(new Vector3(5, 20, -10));
        }
        /// <summary>
        /// Actualiza la cámara
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        private void UpdateCamera(GameTime gameTime)
        {
            // Cambio de cámara
            if (InputHelper.KeyUpEvent(Keys.C))
            {
                if (this.m_Camera.Mode != CameraGameComponent.CameraModes.Free)
                {
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.Free;
                }
                else
                {
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.FirstPerson;
                }
            }

            if (this.m_Camera.Mode != CameraGameComponent.CameraModes.Free)
            {
                if (InputHelper.IsKeyDown(Keys.LeftControl))
                {
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.ThirdPerson;
                }
                else
                {
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.FirstPerson;
                }
            }
        }
        /// <summary>
        /// Actualiza los vehículos
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        private void UpdateVehicles(GameTime gameTime)
        {
            if (InputHelper.KeyUpEvent(Keys.D1))
            {
                this.SetFocus(this.m_Rhino_1);
            }
            else if (InputHelper.KeyUpEvent(Keys.D2))
            {
                this.SetFocus(this.m_Rhino_2);
            }
            else if (InputHelper.KeyUpEvent(Keys.D3))
            {
                this.SetFocus(this.m_LandSpeeder_1);
            }
            else if (InputHelper.KeyUpEvent(Keys.D4))
            {
                this.SetFocus(this.m_LandSpeeder_2);
            }

            if (InputHelper.KeyUpEvent(Keys.Tab))
            {
                if (this.m_CurrentVehicle != null)
                {
                    this.m_CurrentVehicle.SetNextPlayerPosition();
                }
            }
        }
        /// <summary>
        /// Establece el foco en el vehículo especificado
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        private void SetFocus(Vehicle vehicle)
        {
            if (this.m_CurrentVehicle != null)
            {
                this.m_CurrentVehicle.HasFocus = false;
            }

            this.m_CurrentVehicle = vehicle;

            if (vehicle != null)
            {
                // Indicar que la cámara debe seguir al tanque seleccionado
                this.m_Camera.ModelToFollow = vehicle;

                // Establecer el foco en el tanque
                vehicle.HasFocus = true;
            }
        }
    }
}
