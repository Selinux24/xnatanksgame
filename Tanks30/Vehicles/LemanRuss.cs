using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Vehicles
{
    using Common.Helpers;
    using GameComponents.Vehicles;
    using GameComponents.Vehicles.Animations;

    public partial class LemanRuss : Vehicle
    {
        #region Incialización del control de animación

        AnimationAxis m_BattleCannon;
        Animation m_BattleCannonBase;

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
        public LemanRuss(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Inicializa los componentes gráficos
        /// </summary>
        protected override void LoadContent()
        {
            this.ComponentInfoName = "LemanRuss.xml";

            base.LoadContent();

            #region Controlador de animación

            m_BattleCannon = (AnimationAxis)this.GetAnimation("BattleCannon");
            m_BattleCannonBase = (Animation)this.GetAnimation("BattleCannonBase");

            #endregion

            #region Controladores de posición

            m_Driver = this.GetPlayerPosition("Driver");
            m_BattleCannonGunner = this.GetPlayerPosition("BattleCannonGunner");

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
            this.m_BattleCannon.Rotate(pitch);
            this.m_BattleCannonBase.Rotate(yaw);
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
            }
            if (position == Player.BattleCannonGunner)
            {
                m_CurrentPlayerControl = m_BattleCannonGunner;
            }
        }
    }
}
