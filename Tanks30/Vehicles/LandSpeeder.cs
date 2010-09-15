using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Vehicles
{
    using Common.Helpers;
    using GameComponents.Animation;
    using GameComponents.Vehicles;
    using GameComponents.Weapons;
    using Physics;
    using GameComponents.Particles;

    public partial class LandSpeeder : Vehicle
    {
        #region Incialización del control de animación

        private AnimationAxis m_PILOT_HEAD; string _PILOT_HEAD = "PilotHead";
        private AnimationAxis m_PILOT_NECK; string _PILOT_NECK = "PilotNeck";
        private AnimationAxis m_FUSION_CANNON; string _FUSION_CANNON = "FusionCannon";
        private AnimationAxis m_FUSION_CANNON_BASE; string _FUSION_CANNON_BASE = "FusionCannonBase";

        #endregion

        #region Posiciones del jugador

        private PlayerPosition m_PILOT; string _PILOT = "Pilot";
        private PlayerPosition m_GUNNER; string _GUNNER = "Gunner";

        #endregion

        #region Armas

        private Weapon m_FussionCannon = null; string _FussionCannon = "FusionCannon";

        #endregion

        #region Emisores de partículas

        private ParticleEmitter m_LeftEngine = null; string _LeftEngine = "LeftEngine";
        private ParticleEmitter m_RightEngine = null; string _RightEngine = "RightEngine";
        private ParticleEmitter m_LeftSmokeEmitter = null; string _LeftSmokeEmitter = "LeftSmokeEmitter";
        private ParticleEmitter m_RightSmokeEmitter = null; string _RightSmokeEmitter = "RightSmokeEmitter";

        #endregion

        #region Teclas

        Keys m_StartEngines = Keys.O;
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
        public LandSpeeder(Game game, string assetsFolder)
            : base(game)
        {
            this.AssetsFolder = assetsFolder;
            this.ComponentInfoName = "LandSpeeder.xml";
        }

        /// <summary>
        /// Inicializa los componentes gráficos
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            #region Controlador de animación

            m_PILOT_HEAD = (AnimationAxis)this.GetAnimation(_PILOT_HEAD);
            m_PILOT_NECK = (AnimationAxis)this.GetAnimation(_PILOT_NECK);
            m_FUSION_CANNON = (AnimationAxis)this.GetAnimation(_FUSION_CANNON);
            m_FUSION_CANNON_BASE = (AnimationAxis)this.GetAnimation(_FUSION_CANNON_BASE);

            #endregion

            #region Controladores de posición

            m_PILOT = this.GetPlayerControl(_PILOT);
            m_GUNNER = this.GetPlayerControl(_GUNNER);

            #endregion

            #region Armamento

            this.m_FussionCannon = this.GetWeapon(_FussionCannon);

            #endregion

            #region Emisores

            this.m_LeftEngine = this.GetParticleEmitter(_LeftEngine);
            this.m_RightEngine = this.GetParticleEmitter(_RightEngine);
            this.m_LeftSmokeEmitter = this.GetParticleEmitter(_LeftSmokeEmitter);
            this.m_RightSmokeEmitter = this.GetParticleEmitter(_RightSmokeEmitter);

            #endregion

            this.SetPlaterControl(this.m_PILOT);
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
                if (m_CurrentPlayerControl == m_PILOT)
                {
                    bool driving = false;

                    #region Motor

                    if (InputHelper.KeyUpEvent(m_StartEngines))
                    {
                        if (!this.Engine.Active)
                        {
                            this.StartEngine();
                        }
                        else
                        {
                            this.StopEngine();
                        }
                    }

                    #endregion

                    #region Look

                    // Vista del piloto
                    this.DriverLook(InputHelper.PitchDelta, InputHelper.YawDelta);

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
                if (m_CurrentPlayerControl == m_GUNNER)
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
        /// Dirigir la vista del conductor
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void DriverLook(float pitch, float yaw)
        {
            this.m_PILOT_HEAD.Rotate(pitch);
            this.m_PILOT_NECK.Rotate(yaw);
        }
        /// <summary>
        /// Apuntar el bolter pesado
        /// </summary>
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
        public void AimFusionCannon(float pitch, float yaw)
        {
            this.m_FUSION_CANNON.Rotate(pitch);
            this.m_FUSION_CANNON_BASE.Rotate(yaw);
        }

        /// <summary>
        /// La posición del jugador ha cambiado
        /// </summary>
        /// <param name="position">Nueva posición</param>
        protected override void PlayerControlChanged(PlayerPosition position)
        {
            base.PlayerControlChanged(position);

            if (position == this.m_PILOT)
            {
                this.SelectWeapon(null);
            }
            else if (position == this.m_GUNNER)
            {
                this.SelectWeapon(this.m_FussionCannon);
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

            this.m_LeftEngine.Active = true;
            this.m_RightEngine.Active = true;
        }

        protected override void OnDamaged()
        {
            base.OnDamaged();

            this.m_LeftSmokeEmitter.Active = true;
            this.m_RightSmokeEmitter.Active = true;
        }
    }
}
