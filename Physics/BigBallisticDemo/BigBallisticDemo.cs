using System;
using System.Collections.Generic;
using DrawingComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics;

namespace BigBallisticDemo
{
    /// <summary>
    /// Demostración de disparos
    /// </summary>
    class BigBallisticDemo : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Número máximo de balas en la simulación
        /// </summary>
        private const int _AmmoRounds = 100;
        /// <summary>
        /// Número de cajas en la simulación
        /// </summary>
        private const int _Boxes = 30;

        private const float _TerrainSize = 200f;

        /// <summary>
        /// Dispositivo gráfico
        /// </summary>
        protected GraphicsDeviceManager Graphics;
        /// <summary>
        /// Controlador de físicas
        /// </summary>
        protected PhysicsController Physics;

        /// <summary>
        /// Colección de cajas para dibujar
        /// </summary>
        private List<CubeGameComponent> m_Cubes = new List<CubeGameComponent>();
        /// <summary>
        /// El tanque 1
        /// </summary>
        private Tank m_Tank_1 = null;
        /// <summary>
        /// El tanque 2
        /// </summary>
        private Tank m_Tank_2 = null;
        /// <summary>
        /// Tipo actual de munición
        /// </summary>
        private ShotType m_CurrentShotType_1 = ShotType.Laser;
        /// <summary>
        /// Tipo actual de munición
        /// </summary>
        private ShotType m_CurrentShotType_2 = ShotType.Laser;

        /// <summary>
        /// Constructor
        /// </summary>
        public BigBallisticDemo()
            : base()
        {
            this.Graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.Physics = new PhysicsController();
        }

        /// <summary>
        /// Inicialización
        /// </summary>
        protected override void Initialize()
        {
            // Actualizar la proyección
            GlobalMatrices.UpdateProjection(this.GraphicsDevice);

            // Cámara
            CameraGameComponent camera = new CameraGameComponent(this);
            camera.Position = new Vector3(50.0f, 10.0f, 50.0f);
            camera.KeyBoardSensibility = 50;
            this.Components.Add(camera);

            // Suelo
            float size = _TerrainSize * 0.5f;
            Vector3 NO = new Vector3(-size, -20, size);
            Vector3 NE = new Vector3(size, 0, size);
            Vector3 SO = new Vector3(-size, 0, -size);
            Vector3 SE = new Vector3(size, -10, -size);
            Triangle tr1 = new Triangle(NO, SO, NE);
            Triangle tr2 = new Triangle(NE, SO, SE);
            this.Physics.RegisterTerrain(new CollisionTriangleSoup(new Triangle[] { tr1, tr2 }, float.PositiveInfinity));
            FloorGameComponent floor = new FloorGameComponent(this, new Vector3[] { NO, NE, SO, SE }, @"floor");
            floor.Position = new Vector3(0f, 0f, 0f);
            this.Components.Add(floor);

            // Inicializa las balas
            List<AmmoRound> roundList = new List<AmmoRound>();
            for (int i = 0; i < _AmmoRounds; i++)
            {
                AmmoRound round = new AmmoRound(ShotType.UnUsed, 0f, 0f);
                this.Physics.RegisterAmmoData(round);
                roundList.Add(round);
            }

            AmmoDrawer ammoDrawer = new AmmoDrawer(this);
            ammoDrawer.Rounds = roundList.ToArray();
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
                cube.Register(this.Physics);
                this.m_Cubes.Add(cube);
                this.Components.Add(cube);
            }

            // El tanque del jugador 1
            m_Tank_1 = new Tank(this);
            this.Components.Add(m_Tank_1);

            // El tanque del jugador 2
            m_Tank_2 = new Tank(this);
            this.Components.Add(m_Tank_2);

            base.Initialize();

            m_Tank_1.Register(this.Physics);
            m_Tank_2.Register(this.Physics);

            this.Reset();
        }
        /// <summary>
        /// Actualiza la aplicación
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected override void Update(GameTime gameTime)
        {
            // Fin de captura de salida de usuario
            InputHandler.Begin();

            // Salir
            if (InputHandler.KeyPressed(Keys.Escape))
            {
                this.Exit();
            }

            // Reinicio
            if (InputHandler.KeyPressed(Keys.R))
            {
                this.Reset();
            }

            #region Tanque 1

            // Captura de disparo del tanque 1
            if (InputHandler.KeyPressed(Keys.Enter))
            {
                this.Fire(gameTime, this.m_Tank_1, this.m_CurrentShotType_1);
            }

            // Cambios de munición del tanque 1
            if (InputHandler.KeyPressed(Keys.D1))
            {
                m_CurrentShotType_1 = ShotType.HeavyBolter;
            }

            if (InputHandler.KeyPressed(Keys.D2))
            {
                m_CurrentShotType_1 = ShotType.FlameThrower;
            }

            if (InputHandler.KeyPressed(Keys.D3))
            {
                m_CurrentShotType_1 = ShotType.Artillery;
            }

            if (InputHandler.KeyPressed(Keys.D4))
            {
                m_CurrentShotType_1 = ShotType.Laser;
            }

            // Movimiento del tanque 1
            if (InputHandler.KeyPressing(Keys.Up))
            {
                m_Tank_1.GoForward(5f);
            }

            if (InputHandler.KeyPressing(Keys.Down))
            {
                m_Tank_1.GoBackward(5f);
            }

            if (InputHandler.KeyPressing(Keys.Left))
            {
                m_Tank_1.TurnLeft(0.01f);
            }

            if (InputHandler.KeyPressing(Keys.Right))
            {
                m_Tank_1.TurnRight(0.01f);
            }

            #endregion

            #region Tanque 2

            // Captura de disparo del tanque 2
            if (InputHandler.KeyPressed(Keys.Space))
            {
                this.Fire(gameTime, this.m_Tank_2, this.m_CurrentShotType_2);
            }

            // Cambios de munición del tanque 2
            if (InputHandler.KeyPressed(Keys.D7))
            {
                m_CurrentShotType_2 = ShotType.HeavyBolter;
            }

            if (InputHandler.KeyPressed(Keys.D8))
            {
                m_CurrentShotType_2 = ShotType.FlameThrower;
            }

            if (InputHandler.KeyPressed(Keys.D9))
            {
                m_CurrentShotType_2 = ShotType.Artillery;
            }

            if (InputHandler.KeyPressed(Keys.D0))
            {
                m_CurrentShotType_2 = ShotType.Laser;
            }

            // Movimiento del tanque 2
            if (InputHandler.KeyPressing(Keys.I))
            {
                m_Tank_2.GoForward(5f);
            }

            if (InputHandler.KeyPressing(Keys.K))
            {
                m_Tank_2.GoBackward(5f);
            }

            if (InputHandler.KeyPressing(Keys.J))
            {
                m_Tank_2.TurnLeft(0.01f);
            }

            if (InputHandler.KeyPressing(Keys.L))
            {
                m_Tank_2.TurnRight(0.01f);
            }

            #endregion

            this.Physics.Update(gameTime);

            // Fin de captura de entrada de usuario
            InputHandler.End();

            // Actualización base
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
        /// Disparar una bala
        /// </summary>
        private void Fire(GameTime gameTime, Tank tank, ShotType shotType)
        {
            if (tank.CanFire(gameTime, shotType))
            {
                Vector3 direction = tank.Transform.Forward;
                Vector3 position = tank.Transform.Translation + ((direction * 3f) + Vector3.Up);

                this.Physics.Fire(position, direction, shotType);
            }
        }
        /// <summary>
        /// Inicializar la posición de los cuerpos
        /// </summary>
        private void Reset()
        {
            this.Physics.Reset();

            float tankArea = _TerrainSize * 0.9f * 0.5f;

            // Tanque 1
            m_Tank_1.SetState(new Vector3(tankArea, 1f, tankArea), Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(45f)));

            // Tanque 2
            m_Tank_2.SetState(new Vector3(-tankArea, 1f, -tankArea), Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(225f)));

            // Inicializar las cajas
            float boxArea = _TerrainSize * 0.8f * 0.5f;
            Random rnd = new Random(DateTime.Now.Millisecond);
            foreach (CubeGameComponent box in this.m_Cubes)
            {
                float x = ((float)rnd.NextDouble() * boxArea * 2f) - boxArea;
                float y = 5f;
                float z = ((float)rnd.NextDouble() * boxArea * 2f) - boxArea;
                float yaw = (float)rnd.NextDouble() * 0.5f;
                float pitch = (float)rnd.NextDouble() * 0.5f;
                float roll = (float)rnd.NextDouble() * 0.5f;

                box.SetState(new Vector3(x, y, z), Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll));
            }
        }
    }
}
