using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Vehicles
{
    using Common;
    using GameComponents.Vehicles;
    using GameComponents.Vehicles.Animations;

    public partial class LandRaider : Vehicle
    {
        #region Incialización del control de animación

        AnimationAxis m_HeavyBolter;
        AnimationAxis m_HeavyBolterBase;
        AnimationAxis m_LeftLassCannon;
        AnimationAxis m_LeftLassCannonBase;
        AnimationAxis m_RightLassCannon;
        AnimationAxis m_RightLassCannonBase;
        AnimationAxis m_UpperFrontDoor;
        AnimationAxis m_LowerFrontDoor;

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

            CoveredDriver,
            /// <summary>
            /// Artillero principal
            /// </summary>
            Gunner,

            LeftGunner,

            RightGunner,

            Commander,

            CoveredCommander,
        }

        PlayerPosition m_Driver;
        PlayerPosition m_CoveredDriver;
        PlayerPosition m_Gunner;
        PlayerPosition m_LeftGunner;
        PlayerPosition m_RightGunner;
        PlayerPosition m_Commander;
        PlayerPosition m_CoveredCommander;

        #endregion

        #region Teclas

        Keys m_MoveForwardKey = Keys.W;
        Keys m_MoveBackwardKey = Keys.S;
        Keys m_RotateLeftTankKey = Keys.A;
        Keys m_RotateRightTankKey = Keys.D;
        Keys m_ChangeDirectionKey = Keys.R;
        Keys m_AutoPilotKey = Keys.P;

        Keys m_FrontDoorKey = Keys.NumPad8;
        bool m_FrontDoorAction = false;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public LandRaider(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Inicializa los componentes gráficos
        /// </summary>
        protected override void LoadContent()
        {
            this.ComponentInfoName = "LandRaider.xml";

            base.LoadContent();

            #region Controlador de animación

            m_HeavyBolter = (AnimationAxis)this.GetAnimation("HeavyBolter");
            m_HeavyBolterBase = (AnimationAxis)this.GetAnimation("HeavyBolterBase");
            m_LeftLassCannon = (AnimationAxis)this.GetAnimation("LeftLassCannon");
            m_LeftLassCannonBase = (AnimationAxis)this.GetAnimation("LeftLassCannonBase");
            m_RightLassCannon = (AnimationAxis)this.GetAnimation("RightLassCannon");
            m_RightLassCannonBase = (AnimationAxis)this.GetAnimation("RightLassCannonBase");
            m_UpperFrontDoor = (AnimationAxis)this.GetAnimation("UpperFrontDoor");
            m_LowerFrontDoor = (AnimationAxis)this.GetAnimation("LowerFrontDoor");

            #endregion

            #region Controladores de posición

            m_Driver = this.GetPlayerPosition("Driver");
            m_CoveredDriver = this.GetPlayerPosition("CoveredDriver");
            m_Gunner = this.GetPlayerPosition("Gunner");
            m_LeftGunner = this.GetPlayerPosition("LeftGunner");
            m_RightGunner = this.GetPlayerPosition("RightGunner");
            m_Commander = this.GetPlayerPosition("Commander");
            m_CoveredCommander = this.GetPlayerPosition("CoveredCommander");

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
                if (m_CurrentPlayerControl == m_Driver || m_CurrentPlayerControl == m_CoveredDriver)
                {
                    bool driving = false;

                    #region Moving Tank

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

                    #region Rotating Tank

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
                    this.AimHeavyBolter(InputHelper.PitchDelta, InputHelper.YawDelta);

                    #endregion
                }
                if (m_CurrentPlayerControl == m_LeftGunner)
                {
                    #region Left LassCannon

                    // Apuntar
                    this.AimLeftLassCannon(InputHelper.PitchDelta, InputHelper.YawDelta);

                    #endregion
                }
                if (m_CurrentPlayerControl == m_RightGunner)
                {
                    #region Right LassCannon

                    // Apuntar
                    this.AimRightLassCannon(InputHelper.PitchDelta, InputHelper.YawDelta);

                    #endregion
                }
                if (m_CurrentPlayerControl == m_Commander || m_CurrentPlayerControl == m_CoveredCommander)
                {
                    #region Hatch

                    if (InputHelper.KeyUpEvent(m_FrontDoorKey))
                    {
                        m_FrontDoorAction = !m_FrontDoorAction;

                        if (m_FrontDoorAction)
                        {
                            this.OpenFrontDoor();
                        }
                        else
                        {
                            this.CloseFrontDoor();
                        }
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
        public void AimHeavyBolter(float pitch, float yaw)
        {
            this.m_HeavyBolter.Rotate(pitch);
            this.m_HeavyBolterBase.Rotate(yaw);
        }
        /// <summary>
        /// Apuntar el cañón laser izquierdo
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void AimLeftLassCannon(float pitch, float yaw)
        {
            this.m_LeftLassCannon.Rotate(pitch);
            this.m_LeftLassCannonBase.Rotate(yaw);
        }
        /// <summary>
        /// Apuntar el cañón laser derecho
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void AimRightLassCannon(float pitch, float yaw)
        {
            this.m_RightLassCannon.Rotate(pitch);
            this.m_RightLassCannonBase.Rotate(yaw);
        }

        /// <summary>
        /// Abrir la puerta izquierda
        /// </summary>
        public void OpenFrontDoor()
        {
            this.m_UpperFrontDoor.Begin(false);
            this.m_LowerFrontDoor.Begin(false);
        }
        /// <summary>
        /// Cerrar la puerta izquierda
        /// </summary>
        public void CloseFrontDoor()
        {
            this.m_UpperFrontDoor.Begin(true);
            this.m_LowerFrontDoor.Begin(true);
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
            if (position == Player.CoveredDriver)
            {
                m_CurrentPlayerControl = m_CoveredDriver;
            }
            if (position == Player.Gunner)
            {
                m_CurrentPlayerControl = m_Gunner;
            }
            if (position == Player.LeftGunner)
            {
                m_CurrentPlayerControl = m_LeftGunner;
            }
            if (position == Player.RightGunner)
            {
                m_CurrentPlayerControl = m_RightGunner;
            }
            if (position == Player.Commander)
            {
                m_CurrentPlayerControl = m_Commander;
            }
            if (position == Player.CoveredCommander)
            {
                m_CurrentPlayerControl = m_CoveredCommander;
            }
        }
    }
}
