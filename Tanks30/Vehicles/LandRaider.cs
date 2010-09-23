using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Vehicles
{
    using Common.Helpers;
    using GameComponents.Animation;
    using GameComponents.Particles;
    using GameComponents.Vehicles;
    using GameComponents.Weapons;

    public partial class LandRaider : Vehicle
    {
        #region Incialización del control de animación

        private AnimationAxis m_HeavyBolter; private string _HeavyBolter = "HeavyBolter";
        private AnimationAxis m_HeavyBolterBase; private string _HeavyBolterBase = "HeavyBolterBase";
        private AnimationAxis m_LeftLassCannon; private string _LeftLassCannon = "LeftLassCannon";
        private AnimationAxis m_LeftLassCannonBase; private string _LeftLassCannonBase = "LeftLassCannonBase";
        private AnimationAxis m_RightLassCannon; private string _RightLassCannon = "RightLassCannon";
        private AnimationAxis m_RightLassCannonBase; private string _RightLassCannonBase = "RightLassCannonBase";
        private AnimationAxis m_UpperFrontDoor; private string _UpperFrontDoor = "UpperFrontDoor";
        private AnimationAxis m_LowerFrontDoor; private string _LowerFrontDoor = "LowerFrontDoor";
        private AnimationAxis m_Driver; private string _Driver = "Driver";
        private AnimationAxis m_DriverBase; private string _DriverBase = "DriverBase";
        private AnimationAxis m_Commander; private string _Commander = "Commander";
        private AnimationAxis m_CommanderBase; private string _CommanderBase = "CommanderBase";

        #endregion

        #region Posiciones del jugador

        private PlayerPosition m_DriverPosition; private string _DriverPosition = "Driver";
        private PlayerPosition m_CommanderPosition; private string _CommanderPosition = "Commander";
        private PlayerPosition m_TwinLinkedHeavyBolterGunner; private string _TwinLinkedHeavyBolterGunner = "HeavyBolterGunner";
        private PlayerPosition m_LeftLaserCannonGunner; private string _LeftLaserCannonGunner = "LeftLaserCannonGunner";
        private PlayerPosition m_RightLaserCannonGunner; private string _RightLaserCannonGunner = "RightLaserCannonGunner";

        #endregion

        #region Armas

        private Weapon m_TwinLinkedHeavyBolter = null; private string _TwinLinkedHeavyBolter = "TwinLinkedHeavyBolter";
        private Weapon m_LeftTwinLinkedLaserCannon = null; private string _LeftTwinLinkedLaserCannon = "LeftTwinLinkedLaserCannon";
        private Weapon m_RightTwinLinkedLaserCannon = null; private string _RightTwinLinkedLaserCannon = "RightTwinLinkedLaserCannon";

        #endregion

        #region Emisores de partículas

        private ParticleEmitter m_LeftEngine = null; private string _LeftEngine = "LeftEngine";
        private ParticleEmitter m_RightEngine = null; private string _RightEngine = "RightEngine";
        private ParticleEmitter m_LeftCaterpillar = null; private string _LeftCaterpillar = "LeftCaterpillar";
        private ParticleEmitter m_RightCaterpillar = null; private string _RightCaterpillar = "RightCaterpillar";

        #endregion

        #region Teclas

        private Keys m_StartEngines = Keys.O;
        private Keys m_MoveForwardKey = Keys.W;
        private Keys m_MoveBackwardKey = Keys.S;
        private Keys m_RotateLeftTankKey = Keys.A;
        private Keys m_RotateRightTankKey = Keys.D;
        private Keys m_ChangeDirectionKey = Keys.R;
        private Keys m_AutoPilotKey = Keys.P;

        private Keys m_FrontDoorKey = Keys.NumPad8;
        private bool m_FrontDoorAction = false;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public LandRaider(Game game, string assetsFolder)
            : base(game)
        {
            this.AssetsFolder = assetsFolder;
            this.ComponentInfoName = "LandRaider.xml";
        }

        /// <summary>
        /// Inicializa los componentes gráficos
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            #region Controlador de animación

            m_HeavyBolter = (AnimationAxis)this.GetAnimation(_HeavyBolter);
            m_HeavyBolterBase = (AnimationAxis)this.GetAnimation(_HeavyBolterBase);
            m_LeftLassCannon = (AnimationAxis)this.GetAnimation(_LeftLassCannon);
            m_LeftLassCannonBase = (AnimationAxis)this.GetAnimation(_LeftLassCannonBase);
            m_RightLassCannon = (AnimationAxis)this.GetAnimation(_RightLassCannon);
            m_RightLassCannonBase = (AnimationAxis)this.GetAnimation(_RightLassCannonBase);
            m_UpperFrontDoor = (AnimationAxis)this.GetAnimation(_UpperFrontDoor);
            m_LowerFrontDoor = (AnimationAxis)this.GetAnimation(_LowerFrontDoor);
            m_Driver = (AnimationAxis)this.GetAnimation(_Driver);
            m_DriverBase = (AnimationAxis)this.GetAnimation(_DriverBase);
            m_Commander = (AnimationAxis)this.GetAnimation(_Commander);
            m_CommanderBase = (AnimationAxis)this.GetAnimation(_CommanderBase);

            #endregion

            #region Controladores de posición

            m_DriverPosition = this.GetPlayerControl(_DriverPosition);
            m_CommanderPosition = this.GetPlayerControl(_CommanderPosition);
            m_TwinLinkedHeavyBolterGunner = this.GetPlayerControl(_TwinLinkedHeavyBolterGunner);
            m_LeftLaserCannonGunner = this.GetPlayerControl(_LeftLaserCannonGunner);
            m_RightLaserCannonGunner = this.GetPlayerControl(_RightLaserCannonGunner);

            #endregion

            #region Armamento

            this.m_TwinLinkedHeavyBolter = this.GetWeapon(_TwinLinkedHeavyBolter);
            this.m_LeftTwinLinkedLaserCannon = this.GetWeapon(_LeftTwinLinkedLaserCannon);
            this.m_RightTwinLinkedLaserCannon = this.GetWeapon(_RightTwinLinkedLaserCannon);

            #endregion

            #region Emisores

            this.m_LeftEngine = this.GetParticleEmitter(_LeftEngine);
            this.m_RightEngine = this.GetParticleEmitter(_RightEngine);
            this.m_LeftCaterpillar = this.GetParticleEmitter(_LeftCaterpillar);
            this.m_RightCaterpillar = this.GetParticleEmitter(_RightCaterpillar);

            #endregion

            this.SetPlaterControl(this.m_DriverPosition);
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
                if (m_CurrentPlayerControl == m_DriverPosition)
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

                    #region Moving Tank

                    if (InputHelper.IsKeyDown(m_MoveForwardKey))
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
                    if (InputHelper.IsKeyDown(m_MoveBackwardKey))
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

                    if (InputHelper.IsKeyDown(m_RotateLeftTankKey))
                    {
                        driving = true;

                        this.TurnLeft(gameTime);
                    }
                    if (InputHelper.IsKeyDown(m_RotateRightTankKey))
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

                    #region Look

                    // Dirigir la vista
                    this.DriverLook(InputHelper.PitchDelta, InputHelper.YawDelta);

                    #endregion
                }
                if (m_CurrentPlayerControl == m_TwinLinkedHeavyBolterGunner)
                {
                    #region Heavy Bolter

                    // Apuntar el bolter
                    this.AimHeavyBolter(InputHelper.PitchDelta, InputHelper.YawDelta);

                    //Disparar el bolter
                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        this.Fire();
                    }

                    #endregion
                }
                if (m_CurrentPlayerControl == m_LeftLaserCannonGunner)
                {
                    #region Left LassCannon

                    // Apuntar
                    this.AimLeftLassCannon(InputHelper.PitchDelta, InputHelper.YawDelta);

                    //Disparar el bolter
                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        this.Fire();
                    }

                    #endregion
                }
                if (m_CurrentPlayerControl == m_RightLaserCannonGunner)
                {
                    #region Right LassCannon

                    // Apuntar
                    this.AimRightLassCannon(InputHelper.PitchDelta, InputHelper.YawDelta);

                    //Disparar el bolter
                    if (InputHelper.LeftMouseButtonEvent())
                    {
                        this.Fire();
                    }

                    #endregion
                }
                if (m_CurrentPlayerControl == m_CommanderPosition)
                {
                    #region Look

                    // Dirigir la vista
                    this.CommanderLook(InputHelper.PitchDelta, InputHelper.YawDelta);

                    #endregion

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
        /// Dirige la vista del piloto
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void DriverLook(float pitch, float yaw)
        {
            this.m_Driver.Rotate(pitch);
            this.m_DriverBase.Rotate(yaw);
        }
        /// <summary>
        /// Dirige la vista del comandante
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void CommanderLook(float pitch, float yaw)
        {
            this.m_Commander.Rotate(pitch);
            this.m_CommanderBase.Rotate(yaw);
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
        /// La posición del jugador ha cambiado
        /// </summary>
        /// <param name="position">Nueva posición</param>
        protected override void PlayerControlChanged(PlayerPosition position)
        {
            base.PlayerControlChanged(position);

            if (position == this.m_DriverPosition)
            {
                this.SelectWeapon(null);
            }
            else if (position == this.m_CommanderPosition)
            {
                this.SelectWeapon(null);
            }
            else if (position == this.m_TwinLinkedHeavyBolterGunner)
            {
                this.SelectWeapon(this.m_TwinLinkedHeavyBolter);
            }
            else if (position == this.m_LeftLaserCannonGunner)
            {
                this.SelectWeapon(this.m_LeftTwinLinkedLaserCannon);
            }
            else if (position == this.m_RightLaserCannonGunner)
            {
                this.SelectWeapon(this.m_RightTwinLinkedLaserCannon);
            }
        }

        protected override void OnStartMoving()
        {
            base.OnStartMoving();

            this.m_LeftEngine.Active = true;
            this.m_RightEngine.Active = true;
        }

        protected override void OnStopMoving()
        {
            base.OnStopMoving();

            this.m_LeftEngine.Active = false;
            this.m_RightEngine.Active = false;
        }

        protected override void OnAccelerating()
        {
            base.OnAccelerating();

            this.m_LeftCaterpillar.Active = true;
            this.m_RightCaterpillar.Active = true;
        }
    }
}
