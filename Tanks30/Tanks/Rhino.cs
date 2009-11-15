using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using GameComponents;
using GameComponents.Vehicles;
using GameComponents.Vehicles.Animation;

namespace Tanks.Vehicles
{
    /// <summary>
    /// Un rhino
    /// </summary>
    public partial class Rhino : TankGameComponent
    {
        #region Incialización del control de animación

        string _Model = @"Content\Rhino";
        AnimationClamped m_BOLTER; string _BOLTER = "HULL_BODY_BOLTER";
        AnimationBase m_BOLTER_BASE; string _BOLTER_BASE = "HULL_BODY_BOLTER_BASE";
        AnimationClamped m_BOLTER_HATCH; string _BOLTER_HATCH = "HULL_BODY_BOLTER_HATCH";
        AnimationClamped m_DRIVER_HATCH; string _DRIVER_HATCH = "HULL_BODY_HATCH_NORTH";
        AnimationClamped m_RIGHT_HATCH; string _RIGHT_HATCH = "HULL_BODY_HATCH_RIGHT";
        AnimationClamped m_LEFT_HATCH; string _LEFT_HATCH = "HULL_BODY_HATCH_LEFT";
        AnimationClamped m_RIGHT_DOOR; string _RIGHT_DOOR = "HULL_RIGHT_DOOR";
        AnimationClamped m_LEFT_DOOR; string _LEFT_DOOR = "HULL_LEFT_DOOR";
        AnimationClamped m_BACK_DOOR; string _BACK_DOOR = "HULL_BODY_BACK_DOOR";

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

        PlayerPosition m_DRIVER_POSITION; string _DRIVER_POSITION = "POSITION_DRIVER";
        PlayerPosition m_GUNNER_POSITION; string _GUNNER_POSITION = "HULL_BODY_BOLTER";
        PlayerPosition m_GUNNER_COVERED_POSITION; string _GUNNER_COVERED_POSITION = "POSITION_GUNNER";

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

        Keys m_BolterHatchKey = Keys.D7;
        bool m_BolterHatchAction = false;
        Keys m_DriverHatchKey = Keys.D9;
        bool m_DriverHatchAction = false;
        Keys m_BackDoorKey = Keys.D2;
        bool m_BackDoorAction = false;
        Keys m_HatchDoorKey = Keys.D5;
        bool m_HatchDoorAction = false;
        Keys m_RightDoorKey = Keys.D6;
        bool m_RightDoorAction = false;
        Keys m_LeftDoorKey = Keys.D4;
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
        /// Inicializa los componentes gráficos
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            model = contentManager.Load<Model>(_Model);

            #region Controlador de animación

            m_BOLTER = new AnimationClamped(model.Bones[_BOLTER]);
            m_BOLTER.Initialize(Vector3.Right, -25, 45);
            m_BOLTER_BASE = new AnimationBase(model.Bones[_BOLTER_BASE]);
            m_BOLTER_BASE.Initialize(Vector3.Up);
            m_BOLTER_HATCH = new AnimationClamped(model.Bones[_BOLTER_HATCH]);
            m_BOLTER_HATCH.Initialize(Vector3.Right, 0, 100);

            m_DRIVER_HATCH = new AnimationClamped(model.Bones[_DRIVER_HATCH]);
            m_DRIVER_HATCH.Initialize(Vector3.Right, 0, 150);

            m_RIGHT_DOOR = new AnimationClamped(model.Bones[_RIGHT_DOOR]);
            m_RIGHT_DOOR.Initialize(Vector3.Forward, 0, 135);
            m_LEFT_DOOR = new AnimationClamped(model.Bones[_LEFT_DOOR]);
            m_LEFT_DOOR.Initialize(Vector3.Forward, -135, 0);

            m_RIGHT_HATCH = new AnimationClamped(model.Bones[_RIGHT_HATCH]);
            m_RIGHT_HATCH.Initialize(Vector3.Forward, 0, 135);
            m_LEFT_HATCH = new AnimationClamped(model.Bones[_LEFT_HATCH]);
            m_LEFT_HATCH.Initialize(Vector3.Forward, -135, 0);

            m_BACK_DOOR = new AnimationClamped(model.Bones[_BACK_DOOR]);
            m_BACK_DOOR.Initialize(Vector3.Right, 0, 90);

            m_AnimationController.Add(m_BOLTER);
            m_AnimationController.Add(m_BOLTER_BASE);
            m_AnimationController.Add(m_BOLTER_HATCH);
            m_AnimationController.Add(m_DRIVER_HATCH);
            m_AnimationController.Add(m_RIGHT_DOOR);
            m_AnimationController.Add(m_LEFT_DOOR);
            m_AnimationController.Add(m_RIGHT_HATCH);
            m_AnimationController.Add(m_LEFT_HATCH);
            m_AnimationController.Add(m_BACK_DOOR);

            #endregion

            #region Controladores de posición

            m_DRIVER_POSITION = new PlayerPosition(model.Bones[_DRIVER_POSITION], new Vector3(0f, 0f, 0.4f));
            m_GUNNER_POSITION = new PlayerPosition(model.Bones[_GUNNER_POSITION], new Vector3(0f, 0.3f, 0.4f));
            m_GUNNER_COVERED_POSITION = new PlayerPosition(model.Bones[_GUNNER_COVERED_POSITION], new Vector3(0f, 0f, 0.4f));

            m_PlayerControlList.Add(m_DRIVER_POSITION);
            m_PlayerControlList.Add(m_GUNNER_POSITION);
            m_PlayerControlList.Add(m_GUNNER_COVERED_POSITION);

            #endregion

            m_CurrentPlayerControl = m_GUNNER_POSITION;

            m_BoneTransforms = new Matrix[model.Bones.Count];
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

                    #region Dirección

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

                    // Marcar los límites de movimiento del bolter
                    float maxPitch = 0.05f;
                    float maxYaw = 0.05f;

                    // Obtener el input del ratón
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
        /// <param name="pitch">Rotación en Y</param>
        /// <param name="yaw">Rotación en X</param>
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
            this.m_BOLTER_HATCH.Begin(0.08f);
        }
        /// <summary>
        /// Cerrar escotilla del bolter
        /// </summary>
        public void CloseBolterHatch()
        {
            this.m_BOLTER_HATCH.Begin(-0.08f);
        }

        /// <summary>
        /// Abrir escotilla del conductor
        /// </summary>
        public void OpenDriverHatch()
        {
            this.m_DRIVER_HATCH.Begin(0.08f);
        }
        /// <summary>
        /// Cerrar escotilla del conductor
        /// </summary>
        public void CloseDriverHatch()
        {
            this.m_DRIVER_HATCH.Begin(-0.08f);
        }

        /// <summary>
        /// Abrir la puerta de atrás
        /// </summary>
        public void OpenHullBackDoor()
        {
            this.m_BACK_DOOR.Begin(0.06f);
        }
        /// <summary>
        /// Cerrar la puerta del atrás
        /// </summary>
        public void CloseHullBackDoor()
        {
            this.m_BACK_DOOR.Begin(-0.01f);
        }

        /// <summary>
        /// Abrir la escotilla superior
        /// </summary>
        public void OpenHullHatch()
        {
            this.m_RIGHT_HATCH.Begin(0.02f);
            this.m_LEFT_HATCH.Begin(-0.02f);
        }
        /// <summary>
        /// Cerrar la escotilla superior
        /// </summary>
        public void CloseHullHatch()
        {
            this.m_RIGHT_HATCH.Begin(-0.02f);
            this.m_LEFT_HATCH.Begin(0.02f);
        }

        /// <summary>
        /// Abrir la puerta derecha
        /// </summary>
        public void OpenRightDoor()
        {
            this.m_RIGHT_DOOR.Begin(0.06f);
        }
        /// <summary>
        /// Cerrar la puerta derecha
        /// </summary>
        public void CloseRightDoor()
        {
            this.m_RIGHT_DOOR.Begin(-0.01f);
        }

        /// <summary>
        /// Abrir la puerta izquierda
        /// </summary>
        public void OpenLeftDoor()
        {
            this.m_LEFT_DOOR.Begin(-0.06f);
        }
        /// <summary>
        /// Cerrar la puerta izquierda
        /// </summary>
        public void CloseLeftDoor()
        {
            this.m_LEFT_DOOR.Begin(0.01f);
        }

        /// <summary>
        /// Establece la posición del jugador
        /// </summary>
        /// <param name="position">Posición</param>
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