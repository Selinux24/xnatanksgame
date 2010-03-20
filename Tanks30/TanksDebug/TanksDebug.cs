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
    using GameComponents.Weapons;
    using Physics.CollideCoarse;
    using Vehicles;

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
        /// Tamaño del terreno
        /// </summary>
        private const float _TerrainSize = 300f;

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
        /// Colección de cajas para dibujar
        /// </summary>
        private List<CubeGameComponent> m_Cubes = new List<CubeGameComponent>();
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

            // Cámara
            this.m_Camera = new CameraGameComponent(this);
            this.m_Camera.UseMouse = true;
            this.m_Camera.Mode = CameraGameComponent.CameraModes.FirstPerson;
            this.m_Camera.UpdateOrder = 99;
            this.Components.Add(this.m_Camera);

            AmmoDrawer ammoDrawer = new AmmoDrawer(this, @"Content/dharma");
            ammoDrawer.Rounds = this.Physics.Proyectiles;
            ammoDrawer.UpdateOrder = 3;
            this.Components.Add(ammoDrawer);

            // Inicializa las cajas
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < _Boxes; i++)
            {
                // Inicializa los componentes gráficos de las cajas
                float hsX = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                float hsY = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                float hsZ = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                Vector3 min = new Vector3(-hsX, -hsY, -hsZ);
                Vector3 max = new Vector3(hsX, hsY, hsZ);

                CubeGameComponent cube = new CubeGameComponent(this, min, max);
                this.Physics.RegisterVehicle(cube);
                this.m_Cubes.Add(cube);
                cube.UpdateOrder = 4;
                this.Components.Add(cube);
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

            // Indicar que la cámara debe seguir al tanque seleccionado
            this.m_Camera.ModelToFollow = this.m_Rhino_1;

            // Establecer el foco en el tanque
            this.m_Rhino_1.HasFocus = true;

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
            this.Graphics.GraphicsDevice.Clear(Color.White);

            base.Draw(gameTime);
        }
        /// <summary>
        /// Inicializar la posición de los cuerpos
        /// </summary>
        private void Reset()
        {
            this.Physics.Reset();

            float tankArea = _TerrainSize * 0.9f * 0.5f;

            // Tanque 1
            m_Rhino_1.SetInitialState(new Vector3(tankArea, 20f, tankArea) + GlobalTraslation, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(45f)));

            // Tanque 2
            m_Rhino_2.SetInitialState(new Vector3(-tankArea, 20f, -tankArea) + GlobalTraslation, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(225f)));

            // Land Speeder 1
            m_LandSpeeder_1.SetInitialState(new Vector3(-10, 60f, 10) + GlobalTraslation, Quaternion.Identity);

            // Land Speeder 2
            m_LandSpeeder_2.SetInitialState(new Vector3(10, 60f, -10) + GlobalTraslation, Quaternion.Identity);

            // Inicializar las cajas
            float boxArea = _TerrainSize * 0.8f * 0.5f;
            Random rnd = new Random(DateTime.Now.Millisecond);
            foreach (CubeGameComponent box in this.m_Cubes)
            {
                float x = ((float)rnd.NextDouble() * boxArea * 2f) - boxArea;
                float y = 50f;
                float z = ((float)rnd.NextDouble() * boxArea * 2f) - boxArea;
                float yaw = (float)rnd.NextDouble() * 0.5f;
                float pitch = (float)rnd.NextDouble() * 0.5f;
                float roll = (float)rnd.NextDouble() * 0.5f;

                box.SetState(new Vector3(x, y, z) + GlobalTraslation, Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll));
            }
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
                this.m_Rhino_1.HasFocus = true;
                this.m_Rhino_2.HasFocus = false;
                this.m_LandSpeeder_1.HasFocus = false;
                this.m_LandSpeeder_2.HasFocus = false;

                this.m_Camera.ModelToFollow = this.m_Rhino_1;
            }
            else if (InputHelper.KeyUpEvent(Keys.D2))
            {
                this.m_Rhino_1.HasFocus = false;
                this.m_Rhino_2.HasFocus = true;
                this.m_LandSpeeder_1.HasFocus = false;
                this.m_LandSpeeder_2.HasFocus = false;

                this.m_Camera.ModelToFollow = this.m_Rhino_2;
            }
            else if (InputHelper.KeyUpEvent(Keys.D3))
            {
                this.m_Rhino_1.HasFocus = false;
                this.m_Rhino_2.HasFocus = false;
                this.m_LandSpeeder_1.HasFocus = true;
                this.m_LandSpeeder_2.HasFocus = false;

                this.m_Camera.ModelToFollow = this.m_LandSpeeder_1;
            }
            else if (InputHelper.KeyUpEvent(Keys.D4))
            {
                this.m_Rhino_1.HasFocus = false;
                this.m_Rhino_2.HasFocus = false;
                this.m_LandSpeeder_1.HasFocus = false;
                this.m_LandSpeeder_2.HasFocus = true;

                this.m_Camera.ModelToFollow = this.m_LandSpeeder_2;
            }
        }
    }
}
