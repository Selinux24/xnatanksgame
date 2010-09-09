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

        #endregion

        #region Posiciones del jugador

        PlayerPosition m_DRIVER; string _DRIVER = "Driver";
        PlayerPosition m_BATTLECANNONGUNNER; string _BATTLECANNONGUNNER = "BattleCannonGunner";
        PlayerPosition m_LASERCANNONGUNNER; string _LASERCANNONGUNNER = "LaserCannonGunner";
        PlayerPosition m_LEFTBOLTERGUNNER; string _LEFTBOLTERGUNNER = "LeftBolterGunner";
        PlayerPosition m_RIGHTBOLTERGUNNER; string _RIGHTBOLTERGUNNER = "RightBolterGunner";

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

            this.SetPlaterControl(this.m_BATTLECANNONGUNNER);
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

        public void AimLaserCannon(float pitch, float yaw)
        {
            
        }

        public void AimLeftBolter(float pitch, float yaw)
        {

        }

        public void AimRightBolter(float pitch, float yaw)
        {

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
