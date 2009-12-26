using GameComponents;
using GameComponents.Vehicles;
using GameComponents.Vehicles.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tanks.Vehicles
{
    /// <summary>
    /// Un rhino
    /// </summary>
    public partial class Rhino : TankGameComponent
    {
        #region Incializaci�n del control de animaci�n

        AnimationClamped m_BOLTER; string _BOLTER = "Bolter";
        AnimationBase m_BOLTER_BASE; string _BOLTER_BASE = "BolterBase";
        AnimationClamped m_BOLTER_HATCH; string _BOLTER_HATCH = "BolterHatch";
        AnimationClamped m_DRIVER_HATCH; string _DRIVER_HATCH = "DriverHatch";
        AnimationClamped m_RIGHT_HATCH; string _RIGHT_HATCH = "RightHatch";
        AnimationClamped m_LEFT_HATCH; string _LEFT_HATCH = "LeftHatch";
        AnimationClamped m_RIGHT_DOOR; string _RIGHT_DOOR = "RightDoor";
        AnimationClamped m_LEFT_DOOR; string _LEFT_DOOR = "LeftDoor";
        AnimationClamped m_BACK_DOOR; string _BACK_DOOR = "BackDoor";

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
            /// Conductor a cubierto
            /// </summary>
            CoveredDriver,
            /// <summary>
            /// Bolter
            /// </summary>
            Bolter,
            /// <summary>
            /// Bolter a cubierto
            /// </summary>
            CoveredBolter
        }

        PlayerPosition m_DRIVER_POSITION; string _DRIVER_POSITION = "Driver";
        PlayerPosition m_GUNNER_POSITION; string _GUNNER_POSITION = "Gunner";
        PlayerPosition m_GUNNER_COVERED_POSITION; string _GUNNER_COVERED_POSITION = "CoveredGunner";

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

        Keys m_BolterHatchKey = Keys.NumPad7;
        bool m_BolterHatchAction = false;
        Keys m_DriverHatchKey = Keys.NumPad9;
        bool m_DriverHatchAction = false;
        Keys m_BackDoorKey = Keys.NumPad2;
        bool m_BackDoorAction = false;
        Keys m_HatchDoorKey = Keys.NumPad5;
        bool m_HatchDoorAction = false;
        Keys m_RightDoorKey = Keys.NumPad6;
        bool m_RightDoorAction = false;
        Keys m_LeftDoorKey = Keys.NumPad4;
        bool m_LeftDoorAction = false;

        Keys m_ChangeDirectionKey = Keys.R;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public Rhino(Game game)
            : base(game)
        {
            m_MaxForwardVelocity = 2f;
            m_MaxBackwardVelocity = 1f;
            m_AccelerationModifier = m_MaxForwardVelocity / 10f;
            m_BrakeModifier = m_AccelerationModifier * 2f;
            m_AngularVelocityModifier = MathHelper.ToRadians(5f);
        }

        /// <summary>
        /// Inicializa los componentes gr�ficos
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            #region Controlador de animaci�n

            m_BOLTER = (AnimationClamped)this.GetAnimation(_BOLTER);
            m_BOLTER_BASE = this.GetAnimation(_BOLTER_BASE);
            m_BOLTER_HATCH = (AnimationClamped)this.GetAnimation(_BOLTER_HATCH);

            m_DRIVER_HATCH = (AnimationClamped)this.GetAnimation(_DRIVER_HATCH);

            m_RIGHT_DOOR = (AnimationClamped)this.GetAnimation(_RIGHT_DOOR);
            m_LEFT_DOOR = (AnimationClamped)this.GetAnimation(_LEFT_DOOR);

            m_RIGHT_HATCH = (AnimationClamped)this.GetAnimation(_RIGHT_HATCH);
            m_LEFT_HATCH = (AnimationClamped)this.GetAnimation(_LEFT_HATCH);

            m_BACK_DOOR = (AnimationClamped)this.GetAnimation(_BACK_DOOR);

            #endregion

            #region Controladores de posici�n

            m_DRIVER_POSITION = this.GetPlayerPosition(_DRIVER_POSITION);
            m_GUNNER_POSITION = this.GetPlayerPosition(_GUNNER_POSITION);
            m_GUNNER_COVERED_POSITION = this.GetPlayerPosition(_GUNNER_COVERED_POSITION);

            #endregion

            m_CurrentPlayerControl = m_GUNNER_POSITION;
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
                if (m_CurrentPlayerControl == m_DRIVER_POSITION)
                {
                    #region Moving Tank

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

                    #region Rotating Tank

                    if (Keyboard.GetState().IsKeyDown(m_RotateLeftTankKey))
                    {
                        this.TurnLeft();
                    }
                    if (Keyboard.GetState().IsKeyDown(m_RotateRightTankKey))
                    {
                        this.TurnRight();
                    }

                    #endregion

                    #region Direcci�n

                    if (InputHelper.KeyUpEvent(m_ChangeDirectionKey))
                    {
                        this.ChangeDirection();
                    }

                    #endregion

                    #region Driver Hatch

                    if (InputHelper.KeyUpEvent(m_DriverHatchKey))
                    {
                        m_DriverHatchAction = !m_DriverHatchAction;

                        if (m_DriverHatchAction)
                        {
                            this.OpenDriverHatch();
                        }
                        else
                        {
                            this.CloseDriverHatch();
                        }
                    }

                    #endregion
                }
                else if (m_CurrentPlayerControl == m_GUNNER_POSITION)
                {
                    #region Bolter

                    // Marcar los l�mites de movimiento del bolter
                    float maxPitch = 0.05f;
                    float maxYaw = 0.05f;

                    // Obtener el input del rat�n
                    float pitch = MathHelper.Clamp(InputHelper.PitchDelta, -maxPitch, maxPitch);
                    float yaw = MathHelper.Clamp(InputHelper.YawDelta, -maxYaw, maxYaw);

                    // Procesar el input del teclado
                    if (Keyboard.GetState().IsKeyDown(m_RotateLeftBolterKey))
                    {
                        pitch = maxPitch;
                    }
                    if (Keyboard.GetState().IsKeyDown(m_RotateRightBolterKey))
                    {
                        pitch = -maxPitch;
                    }

                    if (Keyboard.GetState().IsKeyDown(m_RotateUpBolterKey))
                    {
                        yaw = maxYaw;
                    }
                    if (Keyboard.GetState().IsKeyDown(m_RotateDownBolterKey))
                    {
                        yaw = -maxYaw;
                    }

                    // Apuntar el bolter
                    this.AimBolter(pitch, yaw);

                    #endregion

                    #region Bolter Hatch

                    if (InputHelper.KeyUpEvent(m_BolterHatchKey))
                    {
                        m_BolterHatchAction = !m_BolterHatchAction;

                        if (m_BolterHatchAction)
                        {
                            this.OpenBolterHatch();
                        }
                        else
                        {
                            this.CloseBolterHatch();
                        }
                    }

                    #endregion
                }
                else if (m_CurrentPlayerControl == m_GUNNER_COVERED_POSITION)
                {
                    #region Back Door

                    if (InputHelper.KeyUpEvent(m_BackDoorKey))
                    {
                        m_BackDoorAction = !m_BackDoorAction;

                        if (m_BackDoorAction)
                        {
                            this.OpenHullBackDoor();
                        }
                        else
                        {
                            this.CloseHullBackDoor();
                        }
                    }

                    #endregion

                    #region Hatch Door

                    if (InputHelper.KeyUpEvent(m_HatchDoorKey))
                    {
                        m_HatchDoorAction = !m_HatchDoorAction;

                        if (m_HatchDoorAction)
                        {
                            this.OpenHullHatch();
                        }
                        else
                        {
                            this.CloseHullHatch();
                        }
                    }

                    #endregion

                    #region Right Door

                    if (InputHelper.KeyUpEvent(m_RightDoorKey))
                    {
                        m_RightDoorAction = !m_RightDoorAction;

                        if (m_RightDoorAction)
                        {
                            this.OpenRightDoor();
                        }
                        else
                        {
                            this.CloseRightDoor();
                        }
                    }

                    #endregion

                    #region Left Door

                    if (InputHelper.KeyUpEvent(m_LeftDoorKey))
                    {
                        m_LeftDoorAction = !m_LeftDoorAction;

                        if (m_LeftDoorAction)
                        {
                            this.OpenLeftDoor();
                        }
                        else
                        {
                            this.CloseLeftDoor();
                        }
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// Apuntar el bolter
        /// </summary>
        /// <param name="pitch">Rotaci�n en Y</param>
        /// <param name="yaw">Rotaci�n en X</param>
        public void AimBolter(float pitch, float yaw)
        {
            this.m_BOLTER.Rotate(pitch);
            this.m_BOLTER_BASE.Rotate(yaw);
        }

        /// <summary>
        /// Abrir escotilla del bolter
        /// </summary>
        public void OpenBolterHatch()
        {
            this.m_BOLTER_HATCH.Begin(false);
        }
        /// <summary>
        /// Cerrar escotilla del bolter
        /// </summary>
        public void CloseBolterHatch()
        {
            this.m_BOLTER_HATCH.Begin(true);
        }

        /// <summary>
        /// Abrir escotilla del conductor
        /// </summary>
        public void OpenDriverHatch()
        {
            this.m_DRIVER_HATCH.Begin(false);
        }
        /// <summary>
        /// Cerrar escotilla del conductor
        /// </summary>
        public void CloseDriverHatch()
        {
            this.m_DRIVER_HATCH.Begin(true);
        }

        /// <summary>
        /// Abrir la puerta de atr�s
        /// </summary>
        public void OpenHullBackDoor()
        {
            this.m_BACK_DOOR.Begin(false);
        }
        /// <summary>
        /// Cerrar la puerta del atr�s
        /// </summary>
        public void CloseHullBackDoor()
        {
            this.m_BACK_DOOR.Begin(true);
        }

        /// <summary>
        /// Abrir la escotilla superior
        /// </summary>
        public void OpenHullHatch()
        {
            this.m_RIGHT_HATCH.Begin(false);
            this.m_LEFT_HATCH.Begin(false);
        }
        /// <summary>
        /// Cerrar la escotilla superior
        /// </summary>
        public void CloseHullHatch()
        {
            this.m_RIGHT_HATCH.Begin(true);
            this.m_LEFT_HATCH.Begin(true);
        }

        /// <summary>
        /// Abrir la puerta derecha
        /// </summary>
        public void OpenRightDoor()
        {
            this.m_RIGHT_DOOR.Begin(false);
        }
        /// <summary>
        /// Cerrar la puerta derecha
        /// </summary>
        public void CloseRightDoor()
        {
            this.m_RIGHT_DOOR.Begin(true);
        }

        /// <summary>
        /// Abrir la puerta izquierda
        /// </summary>
        public void OpenLeftDoor()
        {
            this.m_LEFT_DOOR.Begin(false);
        }
        /// <summary>
        /// Cerrar la puerta izquierda
        /// </summary>
        public void CloseLeftDoor()
        {
            this.m_LEFT_DOOR.Begin(true);
        }

        /// <summary>
        /// Establece la posici�n del jugador
        /// </summary>
        /// <param name="position">Posici�n</param>
        internal void SetPlayerPosition(Player position)
        {
            if (position == Player.Driver)
            {
                m_CurrentPlayerControl = m_DRIVER_POSITION;
            }
            else if (position == Player.CoveredDriver)
            {
                m_CurrentPlayerControl = m_DRIVER_POSITION;
            }
            else if (position == Player.Bolter)
            {
                m_CurrentPlayerControl = m_GUNNER_POSITION;
            }
            else if (position == Player.CoveredBolter)
            {
                m_CurrentPlayerControl = m_GUNNER_COVERED_POSITION;
            }
        }
    }
}