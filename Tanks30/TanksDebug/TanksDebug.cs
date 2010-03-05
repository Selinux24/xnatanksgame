using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TanksDebug
{
    using Common;
    using DrawingComponents;
    using GameComponents.Camera;
    using GameComponents.Vehicles;
    using Physics;
    using Physics.CollideCoarse;

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
        /// Colecci�n de cajas para dibujar
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
        /// Tipo actual de munici�n
        /// </summary>
        private ShotType m_CurrentShotType_1 = ShotType.Laser;
        /// <summary>
        /// Tipo actual de munici�n
        /// </summary>
        private ShotType m_CurrentShotType_2 = ShotType.Laser;

        /// <summary>
        /// Constructor
        /// </summary>
        public BigBallisticDemo()
            : base()
        {
            this.Graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "";
            this.Physics = new PhysicsController();
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

            // C�mara
            this.m_Camera = new CameraGameComponent(this);
            this.m_Camera.UseMouse = true;
            this.m_Camera.Mode = CameraGameComponent.CameraModes.FirstPerson;
            this.Components.Add(this.m_Camera);

            // Suelo
            DemoScenery scn = new DemoScenery(this, _TerrainSize, GlobalTraslation, @"Content/dharma");
            this.Components.Add(scn);
            this.Physics.RegisterEscenery(scn);

            // Inicializa las balas
            List<AmmoRound> roundList = new List<AmmoRound>();
            for (int i = 0; i < _AmmoRounds; i++)
            {
                AmmoRound round = new AmmoRound(ShotType.UnUsed, 0f, 0f);
                //this.Physics.RegisterProyectile(round);
                roundList.Add(round);
            }

            AmmoDrawer ammoDrawer = new AmmoDrawer(this);
            ammoDrawer.Rounds = roundList.ToArray();
            this.Components.Add(ammoDrawer);

            // Inicializa las cajas
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < _Boxes; i++)
            {
                // Inicializa los componentes gr�ficos de las cajas
                float hsX = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                float hsY = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                float hsZ = 0.5f + ((float)rnd.NextDouble() * 1.5f);
                Vector3 min = new Vector3(-hsX, -hsY, -hsZ);
                Vector3 max = new Vector3(hsX, hsY, hsZ);

                CubeGameComponent cube = new CubeGameComponent(this, min, max);
                this.Physics.RegisterVehicle(cube);
                this.m_Cubes.Add(cube);
                this.Components.Add(cube);
            }

            // El tanque del jugador 1
            this.m_Rhino_1 = new Rhino(this);
            this.Components.Add(this.m_Rhino_1);
            this.Physics.RegisterVehicle(m_Rhino_1);

            // El tanque del jugador 2
            this.m_Rhino_2 = new Rhino(this);
            this.Components.Add(this.m_Rhino_2);
            this.Physics.RegisterVehicle(m_Rhino_2);

            // Land speeder 1
            this.m_LandSpeeder_1 = new LandSpeeder(this);
            this.Components.Add(this.m_LandSpeeder_1);
            this.Physics.RegisterVehicle(m_LandSpeeder_1);

            // Land speeder 2
            this.m_LandSpeeder_2 = new LandSpeeder(this);
            this.Components.Add(this.m_LandSpeeder_2);
            this.Physics.RegisterVehicle(m_LandSpeeder_2);

            // Indicar que la c�mara debe seguir al tanque seleccionado
            this.m_Camera.ModelToFollow = this.m_Rhino_1;

            // Establecer el foco en el tanque
            this.m_Rhino_1.HasFocus = true;

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

            // Fin de captura de entrada de usuario
            InputHelper.End();

            // Actualizaci�n base
            base.Update(gameTime);
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
        /// Inicializar la posici�n de los cuerpos
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
                    this.m_Camera.Mode = CameraGameComponent.CameraModes.ThirdPerson;
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
            #region Tanque 1

            // Captura de disparo del tanque 1
            if (InputHelper.KeyDownEvent(Keys.Enter))
            {
                this.Fire(gameTime, this.m_Rhino_1, this.m_CurrentShotType_1);
            }

            if (InputHelper.KeyDownEvent(Keys.Divide))
            {
                m_Rhino_1.SetNextPlayerPosition();
            }

            // Cambios de munici�n del tanque 1
            if (InputHelper.KeyDownEvent(Keys.D1))
            {
                m_CurrentShotType_1 = ShotType.HeavyBolter;
            }

            if (InputHelper.KeyDownEvent(Keys.D2))
            {
                m_CurrentShotType_1 = ShotType.FlameThrower;
            }

            if (InputHelper.KeyDownEvent(Keys.D3))
            {
                m_CurrentShotType_1 = ShotType.Artillery;
            }

            if (InputHelper.KeyDownEvent(Keys.D4))
            {
                m_CurrentShotType_1 = ShotType.Laser;
            }

            // Movimiento del tanque 1
            if (InputHelper.IsKeyDown(Keys.Up))
            {
                m_Rhino_1.Accelerate(gameTime, 50f);
            }

            if (InputHelper.IsKeyDown(Keys.Down))
            {
                m_Rhino_1.Brake(gameTime, 50f);
            }

            if (InputHelper.IsKeyDown(Keys.Left))
            {
                m_Rhino_1.TurnLeft(gameTime, 0.1f);
            }

            if (InputHelper.IsKeyDown(Keys.Right))
            {
                m_Rhino_1.TurnRight(gameTime, 0.1f);
            }

            #endregion

            #region Tanque 2

            // Captura de disparo del tanque 2
            if (InputHelper.KeyDownEvent(Keys.Space))
            {
                this.Fire(gameTime, this.m_Rhino_2, this.m_CurrentShotType_2);
            }

            if (InputHelper.KeyDownEvent(Keys.Tab))
            {
                m_Rhino_1.SetNextPlayerPosition();
            }

            // Cambios de munici�n del tanque 2
            if (InputHelper.KeyDownEvent(Keys.D7))
            {
                m_CurrentShotType_2 = ShotType.HeavyBolter;
            }

            if (InputHelper.KeyDownEvent(Keys.D8))
            {
                m_CurrentShotType_2 = ShotType.FlameThrower;
            }

            if (InputHelper.KeyDownEvent(Keys.D9))
            {
                m_CurrentShotType_2 = ShotType.Artillery;
            }

            if (InputHelper.KeyDownEvent(Keys.D0))
            {
                m_CurrentShotType_2 = ShotType.Laser;
            }

            // Movimiento del tanque 2
            if (InputHelper.IsKeyDown(Keys.I))
            {
                m_Rhino_2.Accelerate(gameTime, 50f);
            }

            if (InputHelper.IsKeyDown(Keys.K))
            {
                m_Rhino_2.Brake(gameTime, 50f);
            }

            if (InputHelper.IsKeyDown(Keys.J))
            {
                m_Rhino_2.TurnLeft(gameTime, 0.1f);
            }

            if (InputHelper.IsKeyDown(Keys.L))
            {
                m_Rhino_2.TurnRight(gameTime, 0.1f);
            }

            #endregion
        }
        /// <summary>
        /// Disparar una bala
        /// </summary>
        private void Fire(GameTime gameTime, Vehicle vehicle, ShotType shotType)
        {
            //if (vehicle.CanFire(gameTime, shotType))
            {
                //Vector3 direction = vehicle.Transform.Forward;
                //Vector3 position = vehicle.CollisionPrimitive.Transform.Translation + ((direction * 3f) + Vector3.Up);

                //this.Physics.Fire(position, direction, shotType);
            }
        }
    }
}
