using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Vehicles
{
    using Common.Helpers;
    using GameComponents.Animation;
    using GameComponents.Vehicles;
    using GameComponents.Weapons;
    using Physics;

    public partial class LandSpeeder : Vehicle
    {
        #region Incialización del control de animación

        private AnimationAxis m_GunnerFusionCannon;
        private AnimationAxis m_GunnerFusionCannonBase;

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

        private PlayerPosition m_Driver;
        private PlayerPosition m_Gunner;

        #endregion

        #region Armas

        private Weapon m_FussionCannon = null;

        #endregion

        #region Teclas

        private Keys m_MoveForwardKey = Keys.W;
        private Keys m_MoveBackwardKey = Keys.S;
        private Keys m_MoveUpKey = Keys.U;
        private Keys m_MoveDownKey = Keys.I;
        private Keys m_RotateLeftTankKey = Keys.A;
        private Keys m_RotateRightTankKey = Keys.D;
        private Keys m_ChangeDirectionKey = Keys.R;
        private Keys m_AutoPilotKey = Keys.P;

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

            m_GunnerFusionCannon = (AnimationAxis)this.GetAnimation("FusionCannon");
            m_GunnerFusionCannonBase = (AnimationAxis)this.GetAnimation("FusionCannonBase");

            #endregion

            #region Controladores de posición

            m_Driver = this.GetPlayerPosition("Driver");
            m_Gunner = this.GetPlayerPosition("Gunner");

            #endregion

            #region Armamento

            this.m_FussionCannon = this.GetWeapon("FusionCannon");

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
                    #region Fusion Cannon

                    // Apuntar el bolter
                    this.AimFusionCannon(InputHelper.PitchDelta, InputHelper.YawDelta);

                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        this.Fire();
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
        public void AimFusionCannon(float pitch, float yaw)
        {
            this.m_GunnerFusionCannon.Rotate(pitch);
            this.m_GunnerFusionCannonBase.Rotate(yaw);
        }

        /// <summary>
        /// Establece la posición del jugador
        /// </summary>
        /// <param name="position">Posición</param>
        internal void SetPlayerPosition(Player position)
        {
            if (position == Player.Driver)
            {
                this.m_CurrentPlayerControl = this.m_Driver;

                this.SelectWeapon(null);
            }
            if (position == Player.Gunner)
            {
                this.m_CurrentPlayerControl = this.m_Gunner;

                this.SelectWeapon(this.m_FussionCannon);
            }
        }
    }
}
