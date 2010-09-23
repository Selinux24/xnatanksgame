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

        private AnimationAxis m_BATTLECANNON; private string _BATTLECANNON = "BattleCannon";
        private Animation m_BATTLECANNONBASE; private string _BATTLECANNONBASE = "BattleCannonBase";
        private AnimationAxis m_HULL_CANNON; private string _HULL_CANNON = "LaserCannon";
        private Animation m_HULL_CANNON_BASE; private string _HULL_CANNON_BASE = "LaserCannonBase";
        private AnimationAxis m_LEFT_BOLTER; private string _LEFT_BOLTER = "LeftHeavyBolter";
        private Animation m_LEFT_BOLTER_BASE; private string _LEFT_BOLTER_BASE = "LeftHeavyBolterBase";
        private AnimationAxis m_RIGHT_BOLTER; private string _RIGHT_BOLTER = "RightHeavyBolter";
        private Animation m_RIGHT_BOLTER_BASE; private string _RIGHT_BOLTER_BASE = "RightHeavyBolterBase";
        private AnimationAxis m_TURRET_HACTH; private string _TURRET_HACTH = "TurretHatch";
        private AnimationAxis m_DRIVER_HACTH; private string _DRIVER_HACTH = "DriverHatch";
        private AnimationAxis m_LEFT_HACTH; private string _LEFT_HACTH = "LeftHatch";
        private AnimationAxis m_RIGHT_HACTH; private string _RIGHT_HACTH = "RightHatch";

        #endregion

        #region Posiciones del jugador

        private PlayerPosition m_DRIVER; private string _DRIVER = "Driver";
        private PlayerPosition m_BATTLECANNONGUNNER; private string _BATTLECANNONGUNNER = "BattleCannonGunner";
        private PlayerPosition m_LASERCANNONGUNNER; private string _LASERCANNONGUNNER = "LaserCannonGunner";
        private PlayerPosition m_LEFTBOLTERGUNNER; private string _LEFTBOLTERGUNNER = "LeftHeavyBolterGunner";
        private PlayerPosition m_RIGHTBOLTERGUNNER; private string _RIGHTBOLTERGUNNER = "RightHeavyBolterGunner";

        #endregion

        #region Armas

        private Weapon m_BattleCannon = null; private string _BattleCannon = "BattleCannon";
        private Weapon m_LaserCannon = null; private string _LaserCannon = "LaserCannon";
        private Weapon m_LeftHeavyBolter = null; private string _LeftHeavyBolter = "LeftHeavyBolter";
        private Weapon m_RightHeavyBolter = null; private string _RightHeavyBolter = "RightHeavyBolter";

        #endregion

        #region Teclas

        private Keys m_StartEngines = Keys.O;
        private Keys m_MoveForwardKey = Keys.W;
        private Keys m_MoveBackwardKey = Keys.S;
        private Keys m_RotateLeftTankKey = Keys.A;
        private Keys m_RotateRightTankKey = Keys.D;
        private Keys m_ChangeDirectionKey = Keys.R;
        private Keys m_TurretHatchKey = Keys.O;
        private bool m_TurretHatchAction = false;
        private Keys m_DriverHatchKey = Keys.O;
        private bool m_DriverHatchAction = false;
        private Keys m_LeftHatchKey = Keys.O;
        private bool m_LeftHatchAction = false;
        private Keys m_RightHatchKey = Keys.O;
        private bool m_RightHatchAction = false;
        private Keys m_AutoPilotKey = Keys.P;

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

            m_BATTLECANNON = (AnimationAxis)this.GetAnimation(_BATTLECANNON);
            m_BATTLECANNONBASE = (Animation)this.GetAnimation(_BATTLECANNONBASE);
            m_HULL_CANNON = (AnimationAxis)this.GetAnimation(_HULL_CANNON);
            m_HULL_CANNON_BASE = (Animation)this.GetAnimation(_HULL_CANNON_BASE);
            m_LEFT_BOLTER = (AnimationAxis)this.GetAnimation(_LEFT_BOLTER);
            m_LEFT_BOLTER_BASE = (AnimationAxis)this.GetAnimation(_LEFT_BOLTER_BASE);
            m_RIGHT_BOLTER = (AnimationAxis)this.GetAnimation(_RIGHT_BOLTER);
            m_RIGHT_BOLTER_BASE = (AnimationAxis)this.GetAnimation(_RIGHT_BOLTER_BASE);
            m_TURRET_HACTH = (AnimationAxis)this.GetAnimation(_TURRET_HACTH);
            m_DRIVER_HACTH = (AnimationAxis)this.GetAnimation(_DRIVER_HACTH);
            m_LEFT_HACTH = (AnimationAxis)this.GetAnimation(_LEFT_HACTH);
            m_RIGHT_HACTH = (AnimationAxis)this.GetAnimation(_RIGHT_HACTH);

            #endregion

            #region Controladores de posición

            m_DRIVER = this.GetPlayerControl(_DRIVER);
            m_BATTLECANNONGUNNER = this.GetPlayerControl(_BATTLECANNONGUNNER);
            m_LASERCANNONGUNNER = this.GetPlayerControl(_LASERCANNONGUNNER);
            m_LEFTBOLTERGUNNER = this.GetPlayerControl(_LEFTBOLTERGUNNER);
            m_RIGHTBOLTERGUNNER = this.GetPlayerControl(_RIGHTBOLTERGUNNER);

            #endregion

            #region Armamento

            this.m_BattleCannon = this.GetWeapon(_BattleCannon);
            this.m_LaserCannon = this.GetWeapon(_LaserCannon);
            this.m_LeftHeavyBolter = this.GetWeapon(_LeftHeavyBolter);
            this.m_RightHeavyBolter = this.GetWeapon(_RightHeavyBolter);

            #endregion

            this.SetPlaterControl(this.m_DRIVER);
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
                if (m_CurrentPlayerControl == m_DRIVER)
                {
                    bool driving = false;

                    #region Motor

                    if (InputHelper.KeyUpEvent(m_StartEngines))
                    {
                        if (!this.IsEngineStarted)
                        {
                            this.StartEngine();
                        }
                        else
                        {
                            this.StopEngine();
                        }
                    }

                    #endregion

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
                if (m_CurrentPlayerControl == m_BATTLECANNONGUNNER)
                {
                    #region BattleCannonGunner

                    // Apuntar
                    this.AimBattleCannon(InputHelper.PitchDelta, InputHelper.YawDelta);

                    //Disparar el bolter
                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        this.Fire();
                    }

                    #endregion

                    #region Turret Hatch

                    if (InputHelper.KeyUpEvent(m_TurretHatchKey))
                    {
                        m_TurretHatchAction = !m_TurretHatchAction;

                        if (m_TurretHatchAction)
                        {
                            this.OpenTurretHatch();
                        }
                        else
                        {
                            this.CloseTurretHatch();
                        }
                    }

                    #endregion
                }
                if (m_CurrentPlayerControl == m_LASERCANNONGUNNER)
                {
                    #region LaserCannonGunner

                    // Apuntar
                    this.AimLaserCannon(InputHelper.PitchDelta, InputHelper.YawDelta);

                    //Disparar el bolter
                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        this.Fire();
                    }

                    #endregion
                }
                if (m_CurrentPlayerControl == m_LEFTBOLTERGUNNER)
                {
                    #region LeftBolter

                    // Apuntar
                    this.AimLeftBolter(InputHelper.PitchDelta, InputHelper.YawDelta);

                    //Disparar el bolter
                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        this.Fire();
                    }

                    #endregion

                    #region Left Hatch

                    if (InputHelper.KeyUpEvent(m_LeftHatchKey))
                    {
                        m_LeftHatchAction = !m_LeftHatchAction;

                        if (m_LeftHatchAction)
                        {
                            this.OpenLeftHatch();
                        }
                        else
                        {
                            this.CloseLeftHatch();
                        }
                    }

                    #endregion
                }
                if (m_CurrentPlayerControl == m_RIGHTBOLTERGUNNER)
                {
                    #region RightBolter

                    // Apuntar
                    this.AimRightBolter(InputHelper.PitchDelta, InputHelper.YawDelta);

                    //Disparar el bolter
                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        this.Fire();
                    }

                    #endregion

                    #region Right Hatch

                    if (InputHelper.KeyUpEvent(m_RightHatchKey))
                    {
                        m_RightHatchAction = !m_RightHatchAction;

                        if (m_RightHatchAction)
                        {
                            this.OpenRightHatch();
                        }
                        else
                        {
                            this.CloseRightHatch();
                        }
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// Apuntar el cañón de batalla pesado
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void AimBattleCannon(float pitch, float yaw)
        {
            this.m_BATTLECANNON.Rotate(pitch);
            this.m_BATTLECANNONBASE.Rotate(yaw);
        }
        /// <summary>
        /// Apuntar el cañón laser
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void AimLaserCannon(float pitch, float yaw)
        {
            this.m_HULL_CANNON.Rotate(pitch);
            this.m_HULL_CANNON_BASE.Rotate(yaw);
        }
        /// <summary>
        /// Apuntar el bolter pesado izquierdo
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void AimLeftBolter(float pitch, float yaw)
        {
            this.m_LEFT_BOLTER.Rotate(pitch);
            this.m_LEFT_BOLTER_BASE.Rotate(yaw);
        }
        /// <summary>
        /// Apuntar el bolter pesado derecho
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void AimRightBolter(float pitch, float yaw)
        {
            this.m_RIGHT_BOLTER.Rotate(pitch);
            this.m_RIGHT_BOLTER_BASE.Rotate(yaw);
        }
        /// <summary>
        /// Abrir escotilla de la torreta
        /// </summary>
        public void OpenTurretHatch()
        {
            this.m_TURRET_HACTH.Begin(false);
        }
        /// <summary>
        /// Cerrar escotilla de la torreta
        /// </summary>
        public void CloseTurretHatch()
        {
            this.m_TURRET_HACTH.Begin(true);
        }
        /// <summary>
        /// Abrir escotilla del conductor
        /// </summary>
        public void OpenDriverHatch()
        {
            this.m_DRIVER_HACTH.Begin(false);
        }
        /// <summary>
        /// Cerrar escotilla del conductor
        /// </summary>
        public void CloseDriverHatch()
        {
            this.m_DRIVER_HACTH.Begin(true);
        }
        /// <summary>
        /// Abrir escotilla izquierda
        /// </summary>
        public void OpenLeftHatch()
        {
            this.m_LEFT_HACTH.Begin(false);
        }
        /// <summary>
        /// Cerrar escotilla izquierda
        /// </summary>
        public void CloseLeftHatch()
        {
            this.m_LEFT_HACTH.Begin(true);
        }
        /// <summary>
        /// Abrir escotilla derecha
        /// </summary>
        public void OpenRightHatch()
        {
            this.m_RIGHT_HACTH.Begin(false);
        }
        /// <summary>
        /// Cerrar escotilla derecha
        /// </summary>
        public void CloseRightHatch()
        {
            this.m_RIGHT_HACTH.Begin(true);
        }

        /// <summary>
        /// La posición del jugador ha cambiado
        /// </summary>
        /// <param name="position">Nueva posición</param>
        protected override void PlayerControlChanged(PlayerPosition position)
        {
            base.PlayerControlChanged(position);

            if (position == this.m_DRIVER)
            {
                this.SelectWeapon(null);
            }
            else if (position == this.m_BATTLECANNONGUNNER)
            {
                this.SelectWeapon(this.m_BattleCannon);
            }
            else if (position == this.m_LASERCANNONGUNNER)
            {
                this.SelectWeapon(this.m_LaserCannon);
            }
            else if (position == this.m_LEFTBOLTERGUNNER)
            {
                this.SelectWeapon(this.m_LeftHeavyBolter);
            }
            else if (position == this.m_RIGHTBOLTERGUNNER)
            {
                this.SelectWeapon(this.m_RightHeavyBolter);
            }
        }
    }
}
