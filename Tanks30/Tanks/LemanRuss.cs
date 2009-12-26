using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tanks
{
    using GameComponents;
    using GameComponents.Vehicles;
    using GameComponents.Vehicles.Animation;

    public partial class LemanRuss : TankGameComponent
    {
        #region Incialización del control de animación

        AnimationClamped m_BattleCannon;
        AnimationBase m_BattleCannonBase;

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

        Keys m_RotateLeftBolterKey = Keys.Left;
        Keys m_RotateRightBolterKey = Keys.Right;
        Keys m_RotateUpBolterKey = Keys.Up;
        Keys m_RotateDownBolterKey = Keys.Down;

        Keys m_ChangeDirectionKey = Keys.R;

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
            this.componentInfoName = "LemanRuss.xml";

            base.LoadContent();

            #region Controlador de animación

            m_BattleCannon = (AnimationClamped)this.GetAnimation("BattleCannon");
            m_BattleCannonBase = (AnimationBase)this.GetAnimation("BattleCannonBase");

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
                    #region Moving

                    if (Keyboard.GetState().IsKeyDown(m_MoveForwardKey))
                    {
                        if (this.IsAdvancing)
                        {
                            this.Accelerate();
                        }
                        else
                        {
                            this.Brake();
                        }
                    }
                    if (Keyboard.GetState().IsKeyDown(m_MoveBackwardKey))
                    {
                        if (this.IsAdvancing)
                        {
                            this.Brake();
                        }
                        else
                        {
                            this.Accelerate();
                        }
                    }

                    #endregion

                    #region Rotating

                    if (Keyboard.GetState().IsKeyDown(m_RotateLeftTankKey))
                    {
                        this.TurnLeft();
                    }
                    if (Keyboard.GetState().IsKeyDown(m_RotateRightTankKey))
                    {
                        this.TurnRight();
                    }

                    #endregion

                    #region Direction

                    if (InputHelper.KeyUpEvent(m_ChangeDirectionKey))
                    {
                        this.ChangeDirection();
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
