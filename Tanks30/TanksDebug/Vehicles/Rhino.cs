using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TanksDebug
{
    using Common;
    using GameComponents.Vehicles;
    using GameComponents.Vehicles.Animations;
    using GameComponents.Weapons;
    using Physics;

    /// <summary>
    /// Un rhino
    /// </summary>
    public partial class Rhino : Vehicle
    {
        #region Incializaci�n del control de animaci�n

        private AnimationAxis m_BOLTER; string _BOLTER = "Bolter";
        private Animation m_BOLTER_BASE; string _BOLTER_BASE = "BolterBase";
        private AnimationAxis m_BOLTER_HATCH; string _BOLTER_HATCH = "BolterHatch";
        private AnimationAxis m_DRIVER_HATCH; string _DRIVER_HATCH = "DriverHatch";
        private AnimationAxis m_RIGHT_HATCH; string _RIGHT_HATCH = "RightHatch";
        private AnimationAxis m_LEFT_HATCH; string _LEFT_HATCH = "LeftHatch";
        private AnimationAxis m_RIGHT_DOOR; string _RIGHT_DOOR = "RightDoor";
        private AnimationAxis m_LEFT_DOOR; string _LEFT_DOOR = "LeftDoor";
        private AnimationAxis m_BACK_DOOR; string _BACK_DOOR = "BackDoor";

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

        private PlayerPosition m_DRIVER_POSITION; string _DRIVER_POSITION = "Driver";
        private PlayerPosition m_GUNNER_POSITION; string _GUNNER_POSITION = "Gunner";
        private PlayerPosition m_GUNNER_COVERED_POSITION; string _GUNNER_COVERED_POSITION = "CoveredGunner";

        #endregion

        #region Armas

        private Weapon m_Bolter = new Weapon()
        {
            Name = "Bolter de asalto",
            Mass = 1f,
            Range = 60f,
            Velocity = 50f,
            AppliedGravity = Constants.FastProyectileGravityForce,
            Radius = 0.1f,
            GenerateExplosion = false,
        };

        #endregion

        #region Teclas

        private Keys m_MoveForwardKey = Keys.W;
        private Keys m_MoveBackwardKey = Keys.S;
        private Keys m_RotateLeftTankKey = Keys.A;
        private Keys m_RotateRightTankKey = Keys.D;
        private Keys m_ChangeDirectionKey = Keys.R;
        private Keys m_AutoPilotKey = Keys.P;

        private Keys m_BolterHatchKey = Keys.NumPad7;
        private bool m_BolterHatchAction = false;
        private Keys m_DriverHatchKey = Keys.NumPad9;
        private bool m_DriverHatchAction = false;
        private Keys m_BackDoorKey = Keys.NumPad2;
        private bool m_BackDoorAction = false;
        private Keys m_HatchDoorKey = Keys.NumPad5;
        private bool m_HatchDoorAction = false;
        private Keys m_RightDoorKey = Keys.NumPad6;
        private bool m_RightDoorAction = false;
        private Keys m_LeftDoorKey = Keys.NumPad4;
        private bool m_LeftDoorAction = false;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public Rhino(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Inicializa los componentes gr�ficos
        /// </summary>
        protected override void LoadContent()
        {
            this.ComponentInfoName = "Rhino.xml";

            base.LoadContent();

            #region Controlador de animaci�n

            m_BOLTER = (AnimationAxis)this.GetAnimation(_BOLTER);
            m_BOLTER_BASE = this.GetAnimation(_BOLTER_BASE);
            m_BOLTER_HATCH = (AnimationAxis)this.GetAnimation(_BOLTER_HATCH);

            m_DRIVER_HATCH = (AnimationAxis)this.GetAnimation(_DRIVER_HATCH);

            m_RIGHT_DOOR = (AnimationAxis)this.GetAnimation(_RIGHT_DOOR);
            m_LEFT_DOOR = (AnimationAxis)this.GetAnimation(_LEFT_DOOR);

            m_RIGHT_HATCH = (AnimationAxis)this.GetAnimation(_RIGHT_HATCH);
            m_LEFT_HATCH = (AnimationAxis)this.GetAnimation(_LEFT_HATCH);

            m_BACK_DOOR = (AnimationAxis)this.GetAnimation(_BACK_DOOR);

            #endregion

            #region Controladores de posici�n

            m_DRIVER_POSITION = this.GetPlayerPosition(_DRIVER_POSITION);
            m_GUNNER_POSITION = this.GetPlayerPosition(_GUNNER_POSITION);
            m_GUNNER_COVERED_POSITION = this.GetPlayerPosition(_GUNNER_COVERED_POSITION);

            #endregion

            this.SetPlayerPosition(Player.Bolter);
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
                if (this.m_CurrentPlayerControl == this.m_DRIVER_POSITION)
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

                    #region Direcci�n

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
                else if (this.m_CurrentPlayerControl == this.m_GUNNER_POSITION)
                {
                    #region Bolter

                    // Apuntar el bolter
                    this.AimBolter(InputHelper.PitchDelta, InputHelper.YawDelta);

                    //Disparar el bolter
                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        this.Fire();
                    }

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
                else if (this.m_CurrentPlayerControl == this.m_GUNNER_COVERED_POSITION)
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
                this.m_CurrentPlayerControl = this.m_DRIVER_POSITION;

                this.SelectWeapon(null);
            }
            else if (position == Player.CoveredDriver)
            {
                this.m_CurrentPlayerControl = this.m_DRIVER_POSITION;

                this.SelectWeapon(null);
            }
            else if (position == Player.Bolter)
            {
                this.m_CurrentPlayerControl = this.m_GUNNER_POSITION;

                this.SelectWeapon(this.m_Bolter);
            }
            else if (position == Player.CoveredBolter)
            {
                this.m_CurrentPlayerControl = this.m_GUNNER_COVERED_POSITION;

                this.SelectWeapon(this.m_Bolter);
            }
        }
    }
}