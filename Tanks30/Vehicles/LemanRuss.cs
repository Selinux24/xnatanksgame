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

        AnimationAxis m_BATTLECANNON; string _BATTLECANNON = "BattleCannon";
        Animation m_BATTLECANNONBASE; string _BATTLECANNONBASE = "BattleCannonBase";
        AnimationAxis m_HULL_CANNON; string _HULL_CANNON = "LaserCannon";
        Animation m_HULL_CANNON_BASE; string _HULL_CANNON_BASE = "LaserCannonBase";
        AnimationAxis m_LEFT_BOLTER; string _LEFT_BOLTER = "LeftHeavyBolter";
        Animation m_LEFT_BOLTER_BASE; string _LEFT_BOLTER_BASE = "LeftHeavyBolterBase";
        AnimationAxis m_RIGHT_BOLTER; string _RIGHT_BOLTER = "RightHeavyBolter";
        Animation m_RIGHT_BOLTER_BASE; string _RIGHT_BOLTER_BASE = "RightHeavyBolterBase";
        AnimationAxis m_TURRET_HACTH; string _TURRET_HACTH = "TurretHatch";
        AnimationAxis m_DRIVER_HACTH; string _DRIVER_HACTH = "DriverHatch";
        AnimationAxis m_LEFT_HACTH; string _LEFT_HACTH = "LeftHatch";
        AnimationAxis m_RIGHT_HACTH; string _RIGHT_HACTH = "RightHatch";

        #endregion

        #region Posiciones del jugador

        PlayerPosition m_DRIVER; string _DRIVER = "Driver";
        PlayerPosition m_BATTLECANNONGUNNER; string _BATTLECANNONGUNNER = "BattleCannonGunner";
        PlayerPosition m_LASERCANNONGUNNER; string _LASERCANNONGUNNER = "LaserCannonGunner";
        PlayerPosition m_LEFTBOLTERGUNNER; string _LEFTBOLTERGUNNER = "LeftHeavyBolterGunner";
        PlayerPosition m_RIGHTBOLTERGUNNER; string _RIGHTBOLTERGUNNER = "RightHeavyBolterGunner";

        #endregion

        #region Armas

        private Weapon m_BattleCannon = null; string _BattleCannon = "BattleCannon";
        private Weapon m_LaserCannon = null; string _LaserCannon = "LaserCannon";
        private Weapon m_LeftHeavyBolter = null; string _LeftHeavyBolter = "LeftHeavyBolter";
        private Weapon m_RightHeavyBolter = null; string _RightHeavyBolter = "RightHeavyBolter";

        #endregion

        #region Teclas

        Keys m_MoveForwardKey = Keys.W;
        Keys m_MoveBackwardKey = Keys.S;
        Keys m_RotateLeftTankKey = Keys.A;
        Keys m_RotateRightTankKey = Keys.D;
        Keys m_ChangeDirectionKey = Keys.R;
        Keys m_TurretHatchKey = Keys.O;
        bool m_TurretHatchAction = false;
        Keys m_DriverHatchKey = Keys.O;
        bool m_DriverHatchAction = false;
        Keys m_LeftHatchKey = Keys.O;
        bool m_LeftHatchAction = false;
        Keys m_RightHatchKey = Keys.O;
        bool m_RightHatchAction = false;
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
