using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TanksDebug
{
    using Common;
    using GameComponents.Vehicles;
    using GameComponents.Vehicles.Animations;

    public partial class LandSpeeder : Vehicle
    {
        #region Incialización del control de animación

        AnimationAxis m_FusionCannon;
        AnimationAxis m_FusionCannonBase;

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
            Gunner,
        }

        PlayerPosition m_Driver;
        PlayerPosition m_Gunner;

        #endregion

        #region Teclas

        Keys m_MoveForwardKey = Keys.W;
        Keys m_MoveBackwardKey = Keys.S;
        Keys m_MoveUpKey = Keys.U;
        Keys m_MoveDownKey = Keys.I;
        Keys m_RotateLeftTankKey = Keys.A;
        Keys m_RotateRightTankKey = Keys.D;
        Keys m_ChangeDirectionKey = Keys.R;
        Keys m_AutoPilotKey = Keys.P;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public LandSpeeder(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Inicializa los componentes gráficos
        /// </summary>
        protected override void LoadContent()
        {
            this.ComponentInfoName = "LandSpeeder.xml";

            base.LoadContent();

            #region Controlador de animación

            m_FusionCannon = (AnimationAxis)this.GetAnimation("FusionCannon");
            m_FusionCannonBase = (AnimationAxis)this.GetAnimation("FusionCannonBase");

            #endregion

            #region Controladores de posición

            m_Driver = this.GetPlayerPosition("Driver");
            m_Gunner = this.GetPlayerPosition("Gunner");

            #endregion

            this.SetPlayerPosition(Player.Gunner);
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
                    if (Keyboard.GetState().IsKeyDown(m_MoveUpKey))
                    {
                        driving = true;

                        this.GoUp(gameTime);
                    }
                    if (Keyboard.GetState().IsKeyDown(m_MoveDownKey))
                    {
                        driving = true;

                        this.GoDown(gameTime);
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
                if (m_CurrentPlayerControl == m_Gunner)
                {
                    #region Heavy Bolter

                    // Apuntar el bolter
                    this.AimFusionCannon(InputHelper.PitchDelta, InputHelper.YawDelta);

                    #endregion
                }
            }
        }

        /// <summary>
        /// Apuntar el bolter pesado
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void AimFusionCannon(float pitch, float yaw)
        {
            this.m_FusionCannon.Rotate(pitch);
            this.m_FusionCannonBase.Rotate(yaw);
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
            if (position == Player.Gunner)
            {
                m_CurrentPlayerControl = m_Gunner;
            }
        }
    }
}
