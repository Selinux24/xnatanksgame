using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Physics;
using CustomProcessors;
using GameComponents;
using GameComponents.Physics;
using GameComponents.Scenery;
using GameComponents.Vehicles.Animation;
using GameComponents.Camera;
using GameComponents.Components.Particles;

namespace GameComponents.Vehicles
{
    /// <summary>
    /// Componente tanque
    /// </summary>
    public partial class TankGameComponent : DrawableGameComponent, IVehicleController
    {
        // Administrador de contenidos
        protected ContentManager contentManager;
        // Escenario
        protected SceneryGameComponent scenery = null;
        // Modelo
        protected Model model;

        // Piloto automático
        private AutoPilot m_Autopilot = new AutoPilot();

        // Posición
        private Vector3 m_Position = Vector3.Zero;
        // Rotación
        private Quaternion m_Rotation = Quaternion.Identity;
        // Escala
        private float m_Scale = 1f;

        // Velocidad de traslación
        private float m_Velocity = 0f;
        // Velocidad de rotación a aplicar
        private float m_AngularVelocity = 0f;
        // Vector velocidad
        private Vector3 m_Direction = Vector3.Forward;
        // Dirección de movimiento del tanque
        private MovingDirections m_MovingDirection = MovingDirections.Forward;
        // Inclinación
        private Quaternion m_Inclination = Quaternion.Identity;

        // Velocidad máxima que puede alcanzar el tanque hacia delante
        protected float m_MaxForwardVelocity = 0f;
        // Velocidad máxima que puede alcanzar el tanque marcha atrás
        protected float m_MaxBackwardVelocity = 0f;
        // Modificador de aceleración
        protected float m_AccelerationModifier = 0f;
        // Modificador de frenado
        protected float m_BrakeModifier = 0f;
        // Velocidad angular
        protected float m_AngularVelocityModifier = 0f;

        // Indica que el vehículo se ha inicializado
        protected bool m_Initialized = false;
        // Indica si la posición ha sido modificada desde la última actualización
        protected bool m_PositionHasChanged = false;
        // Indica si la rotación ha sido modificada desde la última actualización
        protected bool m_RotationHasChanged = false;
        // Indica si la escala ha sido modificada desde la último actualización
        protected bool m_ScaleHasChanged = false;

        // Lista de transformaciones del modelo
        protected Matrix[] m_BoneTransforms;
        // Controlador de animación
        protected AnimationController m_AnimationController = new AnimationController();
        // Lista de posibles posiciones de jugador
        protected List<PlayerPosition> m_PlayerControlList = new List<PlayerPosition>();
        // Posición actual del jugador en el modelo
        protected PlayerPosition m_CurrentPlayerControl = null;
        // Indica si el tanque tiene el foco
        public bool HasFocus = false;

        // test
        int frames = 0;
        int maxframes = 50;

        private ParticleSystem smokePlumeParticles;

        /// <summary>
        /// Obtiene la información de primitivas del modelo
        /// </summary>
        public TriangleInfo TriangleInfo
        {
            get
            {
                return model.Tag as TriangleInfo;
            }
        }

        /// <summary>
        /// Obtiene o establece la posición
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }
        /// <summary>
        /// Obtiene o establece la rotación
        /// </summary>
        public virtual Quaternion Rotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                m_Rotation = value;

                m_Direction = Vector3.Transform(Vector3.Forward, m_Rotation);

                m_RotationHasChanged = true;
            }
        }
        /// <summary>
        /// Obtiene o establece la escala
        /// </summary>
        public virtual float Scale
        {
            get
            {
                return m_Scale;
            }
            set
            {
                m_Scale = value;
            }
        }

        /// <summary>
        /// Obtiene la matriz mundo actual del modelo
        /// </summary>
        public virtual Matrix CurrentTransform
        {
            get
            {
                return GetTransform();
            }
        }
        /// <summary>
        /// Obtiene la vista actual desde el modelo
        /// </summary>
        public virtual Matrix CurrentView
        {
            get
            {
                return GetView();
            }
        }
        /// <summary>
        /// Obtiene la matriz de transformación del modelo
        /// </summary>
        /// <returns>Devuelve la matriz de transformación del modelo</returns>
        protected virtual Matrix GetTransform()
        {
            Matrix world = Matrix.CreateScale(this.Scale);
            world *= Matrix.CreateFromQuaternion(this.Rotation);
            world *= Matrix.CreateFromQuaternion(this.m_Inclination);
            world *= Matrix.CreateTranslation(this.Position);

            return world;
        }
        /// <summary>
        /// Obtiene la matriz de vista desde la posición actual del modelo
        /// </summary>
        /// <returns>Devuelve la matriz de vista desde la posición actual del modelo</returns>
        protected virtual Matrix GetView()
        {
            return m_CurrentPlayerControl.GetViewMatrix(this.m_AnimationController, this.GetTransform());
        }

        /// <summary>
        /// Obtiene la cantidad de velocidad
        /// </summary>
        public virtual float Velocity
        {
            get
            {
                return m_Velocity;
            }
        }
        /// <summary>
        /// Obtiene el vector de dirección actual del vehículo
        /// </summary>
        public virtual Vector3 Direction
        {
            get
            {
                return m_Direction;
            }
        }
        /// <summary>
        /// Obtiene la dirección relativa del vehículo
        /// </summary>
        public virtual MovingDirections MovingDirection
        {
            get
            {
                return m_MovingDirection;
            }
        }
        /// <summary>
        /// Obtiene si el objeto está avanzando
        /// </summary>
        public virtual bool IsAdvancing
        {
            get
            {
                return (m_MovingDirection == MovingDirections.Forward);
            }
        }
        /// <summary>
        /// Indica si el vehículo está detenido
        /// </summary>
        public virtual bool IsStatic
        {
            get
            {
                return (m_Velocity == 0.0f);
            }
        }
        /// <summary>
        /// Obtiene la caja orientada con la transformación aplicada
        /// </summary>
        public virtual OrientedBoundingBox TransformedOBB
        {
            get
            {
                OrientedBoundingBox obb = this.TriangleInfo.OBB;

                obb.Transforms = GetTransform();

                return obb;
            }
        }
        /// <summary>
        /// Obtiene la esfera circundante del modelo con la transformación aplicada
        /// </summary>
        public virtual BoundingSphere TransformedBSph
        {
            get
            {
                BoundingSphere sphere = this.TriangleInfo.BSph;

                Vector3 center = Vector3.Transform(sphere.Center, GetTransform());
                float radius = sphere.Radius;

                return new BoundingSphere(center, radius);
            }
        }

        /// <summary>
        /// Obtiene el piloto automático
        /// </summary>
        public AutoPilot AutoPilot
        {
            get
            {
                return m_Autopilot;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public TankGameComponent(Game game)
            : base(game)
        {
            contentManager = (ContentManager)game.Services.GetService(typeof(ContentManager));
            scenery = (SceneryGameComponent)game.Services.GetService(typeof(SceneryGameComponent));

            smokePlumeParticles = new SmokePlumeParticleSystem(game, contentManager);
            smokePlumeParticles.DrawOrder = 100;

            game.Components.Add(smokePlumeParticles);
        }

        /// <summary>
        /// Carga de contenido gráfico
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }
        /// <summary>
        /// Actualiza el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_Autopilot.Enabled)
            {
                // Piloto automático
                m_Autopilot.UpdateAutoPilot(this);
            }

            // Actualizar la rotación
            this.UpdateRotation(gameTime);

            // Actualizar la posición
            this.UpdatePosition(gameTime);

            // Establecer la visibilidad del vehículo
            this.Visible = this.TransformedBSph.Intersects(BaseCameraGameComponent.gLODHighFrustum);

            smokePlumeParticles.Visible = this.Visible;

            if (!m_Initialized)
            {
                // Actualizar inclinación y altura
                this.UpdateWithScenery(gameTime);

                m_Initialized = true;
            }
            else if (this.Visible)
            {
                if (m_PositionHasChanged || m_RotationHasChanged || m_ScaleHasChanged)
                {
                    // Actualizar inclinación y altura
                    this.UpdateWithScenery(gameTime);
                }

                // Añadiendo humo
                smokePlumeParticles.AddParticle(this.Position, Vector3.Zero);
            }
            else
            {
                frames++;

                if (frames >= maxframes)
                {
                    // Actualizar inclinación y altura
                    this.UpdateWithScenery(gameTime);

                    frames = 0;
                }
            }

            // Controlador de animación
            m_AnimationController.Update(gameTime);

            m_PositionHasChanged = false;
            m_RotationHasChanged = false;
            m_ScaleHasChanged = false;
        }
        /// <summary>
        /// Dibujar
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (model != null)
            {
                Matrix modelTransform = this.GetTransform();

                m_AnimationController.CopyAbsoluteBoneTransformsTo(model, m_BoneTransforms);

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        //SceneryEnvironment.Fog.SetFogToEffect(effect);
                        //SceneryEnvironment.Ambient.SetAmbientToEffect(effect);

                        effect.View = BaseCameraGameComponent.gViewMatrix;
                        effect.Projection = BaseCameraGameComponent.gGlobalProjectionMatrix;
                        effect.World = m_BoneTransforms[mesh.ParentBone.Index] * modelTransform;
                    }

                    mesh.Draw();
                }
            }

            smokePlumeParticles.SetCamera(
                BaseCameraGameComponent.gViewMatrix,
                BaseCameraGameComponent.gGlobalProjectionMatrix);
        }

        /// <summary>
        /// Actualiza el vehículo con el escenario
        /// </summary>
        protected virtual void UpdateWithScenery(GameTime gameTime)
        {
            Triangle? tri = null;
            Vector3? point = null;
            float? distance = null;

            if (scenery.Intersects(m_Position.X, m_Position.Z, out tri, out point, out distance))
            {
                // Establecer la posición definitiva
                m_Position = point.Value;

                // Obtener la normal actual
                Vector3 currentNormal = Matrix.CreateFromQuaternion(m_Rotation).Up;

                // Obtener la normal del triángulo
                Vector3 newNormal = tri.Value.Normal;

                // Calcular inclinación a aplicar
                Quaternion newInclination = Quaternion.Identity;
                Vector3 axis = Vector3.Normalize(Vector3.Cross(currentNormal, newNormal));
                float angle = (float)Math.Acos(Vector3.Dot(currentNormal, newNormal));
                if (angle != 0.0f)
                {
                    newInclination = Quaternion.CreateFromAxisAngle(axis, angle);
                }

                // Obtener el factor para aplicar a la inclinación
                float slerpFactor = this.CalcInclinationSlerpFactor();

                // Establecer la interpolación entre la inclinación actual y la nueva
                m_Inclination = Quaternion.Slerp(m_Inclination, newInclination, slerpFactor);
            }
        }
        /// <summary>
        /// Obtiene el factor para aplicar la inclinación según la velocidad
        /// </summary>
        protected virtual float CalcInclinationSlerpFactor()
        {
            if (m_Velocity == 0f)
            {
                return 1f;
            }
            else
            {
                if (m_MovingDirection == MovingDirections.Forward)
                {
                    return m_Velocity / m_MaxForwardVelocity / 25f;
                }
                else if (m_MovingDirection == MovingDirections.Backward)
                {
                    return m_Velocity / m_MaxBackwardVelocity / 25f;
                }
                else
                {
                    return 0.05f;
                }
            }
        }

        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        public virtual void Accelerate()
        {
            this.AddToVelocity(m_AccelerationModifier);
        }
        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        /// <param name="amount">Cantidad de movimiento a añadir</param>
        public virtual void Accelerate(float amount)
        {
            this.AddToVelocity(amount);
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        public virtual void Brake()
        {
            this.AddToVelocity(-m_BrakeModifier);
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        /// <param name="amount">Cantidad de movimiento a disminuir</param>
        public virtual void Brake(float amount)
        {
            this.AddToVelocity(-amount);
        }
        /// <summary>
        /// Establece la velocidad de traslación del modelo
        /// </summary>
        /// <param name="velocity">Velocidad</param>
        protected virtual void SetVelocity(float velocity)
        {
            m_Velocity = velocity;

            //UpdatePosition();
        }
        /// <summary>
        /// Añade la cantidad especificada a la velocidad de traslación del modelo
        /// </summary>
        /// <param name="velocity">Velocidad</param>
        protected virtual void AddToVelocity(float velocity)
        {
            if (this.IsAdvancing)
            {
                m_Velocity = MathHelper.Clamp(m_Velocity + velocity, 0.0f, m_MaxForwardVelocity);
            }
            else
            {
                m_Velocity = MathHelper.Clamp(m_Velocity + velocity, 0.0f, m_MaxBackwardVelocity);
            }

            //UpdatePosition();
        }
        /// <summary>
        /// Actualiza la posición con respecto a la velocidad y el vector dirección
        /// </summary>
        protected virtual void UpdatePosition(GameTime gameTime)
        {
            if (m_Velocity != 0f)
            {
                // Calcular la velocidad a aplicar este paso
                float timedVelocity = m_Velocity / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (!float.IsInfinity(timedVelocity))
                {
                    // Trasladar la posición
                    m_Position += Vector3.Multiply(m_Direction, timedVelocity);

                    // La posición ha sido modificada
                    m_PositionHasChanged = true;
                }
            }
        }
        /// <summary>
        /// Modifica la dirección de movimiento
        /// </summary>
        public virtual void ChangeDirection()
        {
            if (this.IsStatic)
            {
                m_Direction = Vector3.Negate(m_Direction);

                if (m_MovingDirection == MovingDirections.Forward)
                {
                    m_MovingDirection = MovingDirections.Backward;
                }
                else
                {
                    m_MovingDirection = MovingDirections.Forward;
                }
            }
        }

        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        public virtual void TurnLeft()
        {
            m_AngularVelocity = m_AngularVelocityModifier;

            //UpdateRotation();
        }
        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnLeft(float angle)
        {
            m_AngularVelocity = MathHelper.Clamp(angle, -m_AngularVelocityModifier, m_AngularVelocityModifier);

            //UpdateRotation();
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        public virtual void TurnRight()
        {
            m_AngularVelocity = -m_AngularVelocityModifier;

            //UpdateRotation();
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnRight(float angle)
        {
            m_AngularVelocity = MathHelper.Clamp(-angle, -m_AngularVelocityModifier, m_AngularVelocityModifier);

            //UpdateRotation();
        }
        /// <summary>
        /// Actualizar la rotación
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected virtual void UpdateRotation(GameTime gameTime)
        {
            if (m_AngularVelocity != 0f)
            {
                // Obtener el ángulo a aplicar
                float timedAngularVelocity = m_AngularVelocity / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (!float.IsInfinity(timedAngularVelocity))
                {
                    // Calcular el Quaternion de la rotación parcial
                    Quaternion partialRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, timedAngularVelocity);

                    // Modificar la rotación
                    m_Rotation = Quaternion.Multiply(m_Rotation, partialRotation);

                    // Modificar la dirección
                    m_Direction = Vector3.Transform(m_Direction, partialRotation);

                    // Indicar que la rotación ha sido modificada
                    m_RotationHasChanged = true;

                    // Inicializar la velocidad angular
                    m_AngularVelocity = 0f;
                }
            }
        }

        /// <summary>
        /// Ejecuta la reacción del tanque a colisiones
        /// </summary>
        public virtual void Reaction(IPhysicObject other)
        {
            // TODO: Reacción de colisión. No debe modificarse directamente porque los cuerpos se superponen con la reacción
            IVehicleController vehicle = other as IVehicleController;
            if (vehicle != null)
            {
                // Dirección de la reacción
                Vector3 reactionDirection = Vector3.Normalize(m_Position - vehicle.Position);

                if (vehicle.Velocity > 0.0f)
                {
                    // Reacción
                    m_Position += Vector3.Multiply(reactionDirection, vehicle.Velocity);
                }
                else
                {
                    m_Position += Vector3.Multiply(reactionDirection, 0.01f);
                }

                m_Velocity /= 2.0f;
            }
            else
            {
                m_Velocity = 0.0f;
            }
        }

        /// <summary>
        /// Establecer el siguiente controlador de jugador
        /// </summary>
        public void SetNextPlayerPosition()
        {
            int index = m_PlayerControlList.IndexOf(m_CurrentPlayerControl);
            if (index == m_PlayerControlList.Count - 1)
            {
                m_CurrentPlayerControl = m_PlayerControlList[0];
            }
            else
            {
                m_CurrentPlayerControl = m_PlayerControlList[index + 1];
            }
        }
        /// <summary>
        /// Establecer el anterior controlador de jugador
        /// </summary>
        public void SetPreviousPlayerControl()
        {
            int index = m_PlayerControlList.IndexOf(m_CurrentPlayerControl);
            if (index == 0)
            {
                m_CurrentPlayerControl = m_PlayerControlList[m_PlayerControlList.Count - 1];
            }
            else
            {
                m_CurrentPlayerControl = m_PlayerControlList[index - 1];
            }
        }
    }
}