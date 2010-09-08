using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Vehicles
{
    using Common.Helpers;
    using GameComponents.Animation;
    using GameComponents.Vehicles;
    using GameComponents.Weapons;

    public partial class LemanRuss : Vehicle
    {
        #region Incialización del control de animación

        AnimationAxis m_BATTLECANNON;
        Animation m_BATTLECANNONBASE;

        #endregion

        #region Posiciones del jugador

        /// <summary>
        /// Posiciones posibles del jugador en el modelo
        /// </summary>
        public enum Player
        {
            /// <summary>
            /// Conductor
            /// </summary>
            Driver,
            /// <summary>
            /// Artillero principal
            /// </summary>
            BattleCannonGunner,
        }

        PlayerPosition m_Driver;
        PlayerPosition m_BattleCannonGunner;

        #endregion

        #region Armas

        private Weapon m_BattleCannon = null;
        private Weapon m_LaserCannon = null;
        private Weapon m_LeftHeavyBolter = null;
        private Weapon m_RightHeavyBolter = null;

        #endregion

        #region Teclas

        Keys m_MoveForwardKey = Keys.W;
        Keys m_MoveBackwardKey = Keys.S;
        Keys m_RotateLeftTankKey = Keys.A;
        Keys m_RotateRightTankKey = Keys.D;
        Keys m_ChangeDirectionKey = Keys.R;
        Keys m_AutoPilotKey = Keys.P;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public LemanRuss(Game game, string assetsFolder)
            : base(game)
        {
            this.AssetsFolder = assetsFolder;
            this.ComponentInfoName = "LemanRuss.xml";
        }

        /// <summary>
        /// Inicializa los componentes gráficos
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            #region Controlador de animación

            m_BATTLECANNON = (AnimationAxis)this.GetAnimation("BattleCannon");
            m_BATTLECANNONBASE = (Animation)this.GetAnimation("BattleCannonBase");

            #endregion

            #region Controladores de posición

            m_Driver = this.GetPlayerPosition("Driver");
            m_BattleCannonGunner = this.GetPlayerPosition("BattleCannonGunner");

            #endregion

            #region Armamento

            this.m_BattleCannon = this.GetWeapon("BattleCannon");
            this.m_LaserCannon = this.GetWeapon("LaserCannon");
            this.m_LeftHeavyBolter = this.GetWeapon("LeftHeavyBolter");
            this.m_RightHeavyBolter = this.GetWeapon("RightHeavyBolter");

            #endregion

            this.SetPlayerPosition(Player.BattleCannonGunner);
        }
        /// <summary>
        /// Actualiza el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.HasFocus)
            {
                if (m_CurrentPlayerControl == m_Driver)
                {
                    bool driving = false;

                    #region Moving

                    if (Keyboard.GetState().IsKeyDown(m_MoveForwardKey))
                    {
                        driving = true;

                        if (this.IsAdvancing)
                        {
                            this.Accelerate(gameTime);
                        }
                        else
                        {
                            this.Brake(gameTime);
                        }
                    }
                    if (Keyboard.GetState().IsKeyDown(m_MoveBackwardKey))
                    {
                        driving = true;

                        if (this.IsAdvancing)
                        {
                            this.Brake(gameTime);
                        }
                        else
                        {
                            this.Accelerate(gameTime);
                        }
                    }

                    #endregion

                    #region Rotating

                    if (Keyboard.GetState().IsKeyDown(m_RotateLeftTankKey))
                    {
                        driving = true;

                        this.TurnLeft(gameTime);
                    }
                    if (Keyboard.GetState().IsKeyDown(m_RotateRightTankKey))
                    {
                        driving = true;

                        this.TurnRight(gameTime);
                    }

                    #endregion

                    #region Direction

                    if (InputHelper.KeyUpEvent(m_ChangeDirectionKey))
                    {
                        driving = true;

                        this.ChangeDirection();
                    }

                    #endregion

                    #region Autopilot

                    if (driving)
                    {
                        if (this.AutoPilot.Enabled)
                        {
                            this.AutoPilot.Enabled = false;
                        }
                    }

                    if (InputHelper.KeyUpEvent(m_AutoPilotKey))
                    {
                        this.AutoPilot.Enabled = !this.AutoPilot.Enabled;
                    }

                    #endregion
                }
                if (m_CurrentPlayerControl == m_BattleCannonGunner)
                {
                    #region BattleCannonGunner

                    // Apuntar
                    this.AimBattleCannon(InputHelper.PitchDelta, InputHelper.YawDelta);

                    //Disparar el bolter
                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        //Obtener la posición de la boca del cañón
                        Matrix transform = this.CurrentPlayerControlTransform;
                        Vector3 direction = transform.Forward;
                        Vector3 position = transform.Translation + (transform.Forward * 5f) + (transform.Down);

                        this.Fire(position, direction);
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// Apuntar el bolter pesado
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void AimBattleCannon(float pitch, float yaw)
        {
            this.m_BATTLECANNON.Rotate(pitch);
            this.m_BATTLECANNONBASE.Rotate(yaw);
        }

        /// <summary>
        /// Establece la posición del jugador
        /// </summary>
        /// <param name="position">Posición</param>
        internal void SetPlayerPosition(Player position)
        {
            if (position == Player.Driver)
            {
                m_CurrentPlayerControl = m_Driver;

                this.SelectWeapon(null);
            }
            if (position == Player.BattleCannonGunner)
            {
                m_CurrentPlayerControl = m_BattleCannonGunner;

                this.SelectWeapon(this.m_BattleCannon);
            }
        }
    }
}
