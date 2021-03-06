using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TanksDebug
{
    using Buildings;
    using Common;
    using Common.Helpers;
    using GameComponents.Camera;
    using GameComponents.Particles;
    using GameComponents.Scenery;
    using GameComponents.Text;
    using GameComponents.Vehicles;
    using GameComponents.Weapons;
    using Physics;
    using Physics.CollideCoarse;
    using Vehicles;

    /// <summary>
    /// Demostraci�n de disparos
    /// </summary>
    class BigBallisticDemo : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Traslaci�n global
        /// </summary>
        private readonly Vector3 GlobalTraslation = new Vector3(0, 0, 0);

        /// <summary>
        /// N�mero m�ximo de balas en la simulaci�n
        /// </summary>
        private const int _AmmoRounds = 100;
        /// <summary>
        /// N�mero de cajas en la simulaci�n
        /// </summary>
        private const int _Boxes = 20;
        /// <summary>
        /// N�mero de bolas en la simulaci�n
        /// </summary>
        private const int _Balls = 10;
        /// <summary>
        /// Tama�o del terreno
        /// </summary>
        private const float _TerrainSize = 300f;

        /// <summary>
        /// Dispositivo gr�fico
        /// </summary>
        protected GraphicsDeviceManager Graphics;
        /// <summary>
        /// Controlador de f�sicas
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
        /// Escenario
        /// </summary>
        private DemoScenery m_Scenery = null;
        /// <summary>
        /// Colecci�n de cajas para dibujar
        /// </summary>
        private List<CubeGameComponent> m_Cubes = new List<CubeGameComponent>();
        /// <summary>
        /// Colecci�n de esferas para dibujar
        /// </summary>
        private List<BallGameComponent> m_Balls = new List<BallGameComponent>();
        /// <summary>
        /// Gestor de part�culas
        /// </summary>
        private ParticleManager m_ParticleManager = null;
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
        /// Tanque Leman Russ
        /// </summary>
        private LemanRuss m_LemanRuss = null;
        /// <summary>
        /// Tanque Land Raider
        /// </summary>
        private LandRaider m_LandRaider = null;

        /// <summary>
        /// Veh�culo actual
        /// </summary>
        private Vehicle m_CurrentVehicle = null;

        /// <summary>
        /// Bola 1
        /// </summary>
        private BallGameComponent ball1 = null;
        /// <summary>
        /// Bola 2
        /// </summary>
        private BallGameComponent ball2 = null;
        /// <summary>
        /// Bola 3
        /// </summary>
        private BallGameComponent ball3 = null;

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
            this.Services.AddService<PhysicsController>(this.Physics);

            this.Physics.ExplosionUpdated += new ExplosionHandler(Physics_OnExplosionUpdated);
            this.Physics.ProjectileMoved += new AmmoRoundHandler(Physics_OnProjectileMoved);
            this.Physics.VehicleMoved += new VehicleHandler(Physics_OnVehicleMoved);

            // Part�culas
            this.m_ParticleManager = new ParticleManager(this);
            this.Services.AddService<ParticleManager>(this.m_ParticleManager);
        }

        /// <summary>
        /// Evento de veh�culo en movimiento
        /// </summary>
        /// <param name="vehicle">Veh�culo</param>
        void Physics_OnVehicleMoved(IPhysicObject vehicle)
        {
            //if (vehicle is Vehicle)
            //{
            //    Vehicle v = vehicle as Vehicle;
            //    if (v.IsSkimmer)
            //    {
            //        this.m_ParticleManager.AddParticle(ParticleSystemTypes.SmokeEngine, v.Position, Vector3.Up * 0.01f);
            //    }
            //    else
            //    {
            //        this.m_ParticleManager.AddParticle(ParticleSystemTypes.Dust, v.Position, Vector3.Up * 0.01f);
            //    }
            //}
        }
        /// <summary>
        /// Evento de explosi�n activa
        /// </summary>
        /// <param name="explosion">Explosi�n</param>
        void Physics_OnExplosionUpdated(Explosion explosion)
        {
            this.m_ParticleManager.AddParticle(ParticleSystemTypes.Explosion, explosion.DetonationCenter, Vector3.Up * 0.5f);
            this.m_ParticleManager.AddParticle(ParticleSystemTypes.ExplosionSmoke, explosion.DetonationCenter, Vector3.Up * 0.5f);
        }
        /// <summary>
        /// Evento de proyectil en movimiento
        /// </summary>
        /// <param name="obj">Objeto</param>
        /// <param name="position">Posici�n</param>
        /// <param name="velocity">Velocidad</param>
        void Physics_OnProjectileMoved(AmmoRound ammo)
        {
            this.m_ParticleManager.AddParticle(ParticleSystemTypes.ProjectileTrail, ammo.Position, Vector3.Zero);
        }

        /// <summary>
        /// Inicializaci�n
        /// </summary>
        protected override void Initialize()
        {
            // Inicializar el control de entrada de usuario
            InputHelper.Initialize(this.GraphicsDevice);

            // Actualizar la proyecci�n
            GlobalMatrices.UpdateProjection(this.GraphicsDevice);
#if DEBUG
            // Debug
            GameComponents.Debug.DebugDrawer.Initialize(this.GraphicsDevice);
#endif

            // Suelo
            this.m_Scenery = new DemoScenery(this, _TerrainSize, GlobalTraslation, @"Content/dharma");
            this.m_Scenery.UpdateOrder = 0;
            this.Components.Add(this.m_Scenery);
            this.Physics.RegisterScenery(this.m_Scenery);

            // Reflejo solar
            this.m_LensFlare = new LensFlareComponent(this);
            this.m_LensFlare.DrawOrder = 98;
            this.Components.Add(this.m_LensFlare);

            // Texto
            this.m_Text = new TextDrawerComponent(this);
            this.m_Text.DrawOrder = 99;
            this.Components.Add(this.m_Text);

            // C�mara
            this.m_Camera = new CameraGameComponent(this);
            this.m_Camera.UseMouse = true;
            this.m_Camera.Forward = Keys.Up;
            this.m_Camera.Backward = Keys.Down;
            this.m_Camera.Left = Keys.Left;
            this.m_Camera.Right = Keys.Right;
            this.m_Camera.Up = Keys.Add;
            this.m_Camera.Down = Keys.Subtract;
            this.m_Camera.Mode = CameraGameComponent.CameraModes.Free;
            this.m_Camera.UpdateOrder = 99;
            this.Components.Add(this.m_Camera);

            // Un edificio
            this.m_Building_1 = new BuildingType0(this, @"Content/Buildings/", @"WHBuilding01.xml");
            this.m_Building_1.UpdateOrder = 3;
            this.Components.Add(this.m_Building_1);
            this.Physics.RegisterObject(this.m_Building_1);

            // Otro edificio
            this.m_Building_2 = new BuildingType0(this, @"Content/Buildings/", @"WHBuilding02.xml");
            this.m_Building_2.UpdateOrder = 3;
            this.Components.Add(this.m_Building_2);
            this.Physics.RegisterObject(this.m_Building_2);

            // Otro edificio
            this.m_Building_3 = new BuildingType0(this, @"Content/Buildings/", @"WHBuilding03.xml");
            this.m_Building_3.UpdateOrder = 3;
            this.Components.Add(this.m_Building_3);
            this.Physics.RegisterObject(this.m_Building_3);

            AmmoDrawer ammoDrawer = new AmmoDrawer(this, @"Content/dharma");
            ammoDrawer.Rounds = this.Physics.Projectiles;
            ammoDrawer.UpdateOrder = 3;
            this.Components.Add(ammoDrawer);

            Random rnd = new Random(DateTime.Now.Millisecond);

            // Inicializa las cajas
            for (int i = 0; i < _Boxes; i++)
            {
                // Inicializa los componentes gr�ficos de las cajas
                float hsX = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                float hsY = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                float hsZ = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                Vector3 min = new Vector3(-hsX, -hsY, -hsZ);
                Vector3 max = new Vector3(hsX, hsY, hsZ);

                CubeGameComponent cube = new CubeGameComponent(this, min, max, (max - min).Length() * 20f);
                this.Physics.RegisterObject(cube);
                this.m_Cubes.Add(cube);
                cube.UpdateOrder = 4;
                this.Components.Add(cube);
            }

            // Inicializa las esferas
            for (int i = 0; i < _Balls; i++)
            {
                // Inicializa los componentes gr�ficos de las esferas
                float radius = 0.5f + ((float)rnd.NextDouble() * 1.5f);

                BallGameComponent ball = new BallGameComponent(this, radius, radius);
                this.Physics.RegisterObject(ball);
                this.m_Balls.Add(ball);
                ball.UpdateOrder = 4;
                this.Components.Add(ball);
            }

            // Land Raider
            this.m_LandRaider = new LandRaider(this, "Content/Vehicles");
            this.m_LandRaider.UpdateOrder = 5;
            this.Components.Add(this.m_LandRaider);
            this.Physics.RegisterObject(m_LandRaider);

            // Leman Russ
            this.m_LemanRuss = new LemanRuss(this, "Content/Vehicles");
            this.m_LemanRuss.UpdateOrder = 5;
            this.Components.Add(this.m_LemanRuss);
            this.Physics.RegisterObject(m_LemanRuss);

            // Land speeder 1
            this.m_LandSpeeder_1 = new LandSpeeder(this, "Content/Vehicles");
            this.m_LandSpeeder_1.UpdateOrder = 5;
            this.Components.Add(this.m_LandSpeeder_1);
            this.Physics.RegisterObject(m_LandSpeeder_1);

            // Land speeder 2
            this.m_LandSpeeder_2 = new LandSpeeder(this, "Content/Vehicles");
            this.m_LandSpeeder_2.UpdateOrder = 5;
            this.Components.Add(this.m_LandSpeeder_2);
            this.Physics.RegisterObject(m_LandSpeeder_2);

            // El tanque del jugador 1
            this.m_Rhino_1 = new Rhino(this, "Content/Vehicles");
            this.m_Rhino_1.UpdateOrder = 5;
            this.Components.Add(this.m_Rhino_1);
            this.Physics.RegisterObject(m_Rhino_1);

            // El tanque del jugador 2
            this.m_Rhino_2 = new Rhino(this, "Content/Vehicles");
            this.m_Rhino_2.UpdateOrder = 5;
            this.Components.Add(this.m_Rhino_2);
            this.Physics.RegisterObject(m_Rhino_2);

            // Prueba
            //this.ball1 = new BallGameComponent(this, 1f, 1f);
            //this.Components.Add(ball1);
            //this.Physics.RegisterVehicle(ball1);

            //this.ball2 = new BallGameComponent(this, 1.5f, 2f);
            //this.Components.Add(ball2);
            //this.Physics.RegisterVehicle(ball2);

            //this.ball3 = new BallGameComponent(this, 2f, 4f);
            //this.Components.Add(ball3);
            //this.Physics.RegisterVehicle(ball3);

            //RodComponent rod1 = new RodComponent(this, this.ball1, Vector3.Up * this.ball1.Radius, null, new Vector3(0, 40, 0), 10f);
            //this.Components.Add(rod1);
            //this.Physics.RegisterContactGenerator(rod1.Rod);

            //RodComponent rod2 = new RodComponent(this, this.ball2, Vector3.Up * this.ball2.Radius, null, new Vector3(0, 40, 0), 10f);
            //this.Components.Add(rod2);
            //this.Physics.RegisterContactGenerator(rod2.Rod);

            //JointComponent joint1 = new JointComponent(this, this.ball1, Vector3.Down * this.ball1.Radius, this.ball3, Vector3.Left * this.ball3.Radius, 5f);
            //this.Components.Add(joint1);
            //this.Physics.RegisterContactGenerator(joint1.Joint);

            //JointComponent joint2 = new JointComponent(this, this.ball2, Vector3.Down * this.ball2.Sphere.Radius, this.ball3, Vector3.Right * this.ball3.Sphere.Radius, 5f);
            //this.Components.Add(joint2);
            //this.Physics.RegisterContactGenerator(joint2.Joint);

            //Joint2Component rod24 = new Joint2Component(this, this.ball1, Vector3.Up * this.ball1.Radius, null, new Vector3(0, 40, 0), 10f);
            //this.Components.Add(rod24);
            //this.Physics.RegisterContactGenerator(rod24.Rod);

            // Establecer el foco en el veh�culo
            this.SetFocus(this.m_Rhino_1);

            base.Initialize();

            this.Reset();
        }
        /// <summary>
        /// Actualiza la aplicaci�n
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

            // C�mara
            this.UpdateCamera(gameTime);

            // Veh�culos
            this.UpdateVehicles(gameTime);

            // F�sicas
            this.Physics.Update(gameTime);

            // Actualizaci�n base
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
            this.Graphics.GraphicsDevice.Clear(SceneryEnvironment.Ambient.AtmosphericColor);

            this.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            this.GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            this.GraphicsDevice.RenderState.FogColor = SceneryEnvironment.Ambient.AtmosphericColor;
            this.GraphicsDevice.RenderState.FogTableMode = FogMode.Linear;
            this.GraphicsDevice.RenderState.FogStart = 75;
            this.GraphicsDevice.RenderState.FogEnd = 500;
            this.GraphicsDevice.RenderState.FogDensity = 0.25f;
            this.GraphicsDevice.RenderState.FogEnable = true;

            this.DrawText();

            base.Draw(gameTime);

#if DEBUG
            if (IntersectionTests.m_DEBUGUSE)
            {
                for (int i = 0; i < IntersectionTests.m_DEBUGAABBCOUNT; i++)
                {
                    GameComponents.Debug.DebugDrawer.DrawDebugAABB(this.GraphicsDevice, IntersectionTests.m_DEBUGAABB[i]);
                }
                IntersectionTests.m_DEBUGAABBCOUNT = 0;

                for (int i = 0; i < IntersectionTests.m_DEBUGTRICOUNT; i++)
                {
                    GameComponents.Debug.DebugDrawer.DrawDebugTriangle(this.GraphicsDevice, IntersectionTests.m_DEBUGTRI[i]);
                }
                IntersectionTests.m_DEBUGTRICOUNT = 0;

                for (int i = 0; i < IntersectionTests.m_DEBUGEDGESCOUNT; i++)
                {
                    GameComponents.Debug.DebugDrawer.DrawDebugEdge(this.GraphicsDevice, IntersectionTests.m_DEBUGEDGES[i]);
                }
                IntersectionTests.m_DEBUGEDGESCOUNT = 0;
            }
#endif
        }
        /// <summary>
        /// Dibuja el texto
        /// </summary>
        private void DrawText()
        {
            string text = "Contactos en uso: " + this.Physics.UsedContacts.ToString() + Environment.NewLine;
            text += "Particulas de humo de motor en uso: " + this.m_ParticleManager.GetUsedParticlesCount(ParticleSystemTypes.SmokeEngine).ToString() + Environment.NewLine;
            text += "Particulas de polvo en uso: " + this.m_ParticleManager.GetUsedParticlesCount(ParticleSystemTypes.Dust).ToString() + Environment.NewLine;
            if (this.m_CurrentVehicle != null)
            {
                text += "POSICION: " + this.m_CurrentVehicle.Position.ToString() + Environment.NewLine;
                text += "VELOCIDAD: " + this.m_CurrentVehicle.Velocity.ToString() + Environment.NewLine;
                text += "HULL: " + this.m_CurrentVehicle.Hull.ToString() + Environment.NewLine;
                text += "ARMR: " + this.m_CurrentVehicle.Armor.ToString() + Environment.NewLine;
            }

            this.m_Text.WriteText(text, 5, 5, Color.Yellow);
        }
        /// <summary>
        /// Inicializar la posici�n de los cuerpos
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

            // Leman Russ
            m_LemanRuss.SetInitialState(new Vector3(tankArea, 20f, -tankArea * 0.5f) + GlobalTraslation, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(125f)));

            // Land Raider
            m_LandRaider.SetInitialState(new Vector3(-tankArea, 20f, tankArea * 0.5f) + GlobalTraslation, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(25f)));

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

            //this.ball1.SetPosition(new Vector3(5, 35, 5));
            //this.ball2.SetPosition(new Vector3(-15, 39, -20));
            //this.ball3.SetPosition(new Vector3(5, 20, -10));
        }
        /// <summary>
        /// Actualiza la c�mara
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        private void UpdateCamera(GameTime gameTime)
        {
            // Cambio de c�mara
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
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.Chase;
                }
                else
                {
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.FirstPerson;
                }
            }
        }
        /// <summary>
        /// Actualiza los veh�culos
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
            else if (InputHelper.KeyUpEvent(Keys.D5))
            {
                this.SetFocus(this.m_LemanRuss);
            }
            else if (InputHelper.KeyUpEvent(Keys.D6))
            {
                this.SetFocus(this.m_LandRaider);
            }

            if (InputHelper.KeyUpEvent(Keys.Tab))
            {
                if (this.m_CurrentVehicle != null)
                {
                    this.m_CurrentVehicle.SetNextPlayerControl();
                }
            }
        }
        /// <summary>
        /// Establece el foco en el veh�culo especificado
        /// </summary>
        /// <param name="vehicle">Veh�culo</param>
        private void SetFocus(Vehicle vehicle)
        {
            if (this.m_CurrentVehicle != null)
            {
                this.m_CurrentVehicle.HasFocus = false;
            }

            this.m_CurrentVehicle = vehicle;

            if (vehicle != null)
            {
                // Indicar que la c�mara debe seguir al tanque seleccionado
                this.m_Camera.ModelToFollow = vehicle;

                // Establecer el foco en el tanque
                vehicle.HasFocus = true;
            }
        }
    }
}
