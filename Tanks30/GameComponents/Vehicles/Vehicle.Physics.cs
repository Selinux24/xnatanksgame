using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    using Common.Helpers;
    using Physics;

    /// <summary>
    /// Control de físicas de un vehículo
    /// </summary>
    public partial class Vehicle : IVehicleController, IPhysicObject
    {
        /// <summary>
        /// Primitiva de colisión
        /// </summary>
        private CollisionBox m_OBB = null;

        /// <summary>
        /// Velocidad máxima que puede alcanzar el tanque hacia delante
        /// </summary> 
        protected float MaxForwardVelocity = 0f;
        /// <summary>
        /// Velocidad máxima que puede alcanzar el tanque marcha atrás
        /// </summary> 
        protected float MaxBackwardVelocity = 0f;
        /// <summary>
        /// Modificador de aceleración
        /// </summary>
        protected float AccelerationModifier = 0f;
        /// <summary>
        /// Modificador de frenado
        /// </summary>
        protected float BrakeModifier = 0f;
        /// <summary>
        /// Velocidad angular
        /// </summary>
        protected float AngularVelocityModifier = 0f;
        /// <summary>
        /// Vehículo volador
        /// </summary>
        protected bool Skimmer = false;
        /// <summary>
        /// Altura de vuelo máxima
        /// </summary>
        protected float MaxFlightHeight = 0f;
        /// <summary>
        /// Altura de vuelo mínima
        /// </summary>
        protected float MinFlightHeight = 0f;
        /// <summary>
        /// Angulo de inclinación del morro en el ascenso
        /// </summary>
        protected float AscendingAngle = 0f;
        /// <summary>
        /// Angulo de inclinación del morro en el descenso
        /// </summary>
        protected float DescendingAngle = 0f;

        /// <summary>
        /// Piloto automático
        /// </summary>
        private AutoPilot m_Autopilot = new AutoPilot();
        /// <summary>
        /// Dirección de movimiento del tanque
        /// </summary>
        private MovingDirections m_MovingDirection = MovingDirections.Forward;

        /// <summary>
        /// Obtiene la cantidad de velocidad
        /// </summary>
        public virtual float Velocity
        {
            get
            {
                return this.m_OBB.Velocity.Length();
            }
        }
        /// <summary>
        /// Obtiene el vector de dirección actual del vehículo
        /// </summary>
        public virtual Vector3 Direction
        {
            get
            {
                return Vector3.Normalize(this.m_OBB.Transform.Forward);
            }
        }
        /// <summary>
        /// Obtiene la dirección relativa del vehículo
        /// </summary>
        public virtual MovingDirections MovingDirection
        {
            get
            {
                return this.m_MovingDirection;
            }
        }
        /// <summary>
        /// Obtiene si el objeto está avanzando
        /// </summary>
        public virtual bool IsAdvancing
        {
            get
            {
                return (this.m_MovingDirection == MovingDirections.Forward);
            }
        }
        /// <summary>
        /// Indica si el vehículo está detenido
        /// </summary>
        public virtual bool IsStatic
        {
            get
            {
                return (this.Velocity <= 1f);
            }
        }

        /// <summary>
        /// Obtiene el piloto automático
        /// </summary>
        public AutoPilot AutoPilot
        {
            get
            {
                return this.m_Autopilot;
            }
        }

        /// <summary>
        /// Modifica la dirección de movimiento
        /// </summary>
        public virtual void ChangeDirection()
        {
            if (this.IsStatic)
            {
                //this.m_Direction = Vector3.Negate(this.m_Direction);

                //if (this.m_MovingDirection == MovingDirections.Forward)
                //{
                //    this.m_MovingDirection = MovingDirections.Backward;
                //}
                //else
                //{
                //    this.m_MovingDirection = MovingDirections.Forward;
                //}
            }
        }

        /// <summary>
        /// Obtiene la primitiva de colisión del vehículo
        /// </summary>
        /// <returns>Devuelve la primitiva de colisión del vehículo</returns>
        public virtual CollisionPrimitive GetPrimitive()
        {
            return this.m_OBB;
        }
        /// <summary>
        /// Obtiene la primitiva de colisión del vehículo
        /// </summary>
        /// <param name="physicObject">Objeto físico</param>
        /// <returns>Devuelve la primitiva de colisión del vehículo</returns>
        public virtual CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            if (physicObject != null)
            {
                // Obtener las esferas circundantes y detectar colisión potencial
                BoundingSphere thisSPH = this.GetSPH();
                BoundingSphere otherSph = physicObject.GetSPH();
                if (thisSPH.Intersects(otherSph))
                {
                    return this.m_OBB;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene la caja alineada con los ejes que circunda el vehículo, en coordenadas del mundo
        /// </summary>
        /// <returns>Devuelve una caja alineada con los ejes</returns>
        public virtual BoundingBox GetAABB()
        {
            return this.m_OBB.AABB;
        }
        /// <summary>
        /// Obtiene la esfera que circunda el vehículo, en coordenadas del mundo
        /// </summary>
        /// <returns>Devuelve una esfera</returns>
        public virtual BoundingSphere GetSPH()
        {
            return this.m_OBB.SPH;
        }
        /// <summary>
        /// Obtiene si el vehículo está estático
        /// </summary>
        /// <returns>Devuelve verdadero si está estático</returns>
        public virtual bool IsActive()
        {
            return this.m_OBB.IsAwake;
        }
        /// <summary>
        /// Integra el cuerpo del vehículo en el tiempo
        /// </summary>
        /// <param name="time">Cantidad de tiempo</param>
        public void Integrate(float time)
        {
            if (this.m_OBB != null)
            {
                this.m_OBB.Integrate(time);

                if (this.Skimmer)
                {
                    Vector3 velocity = this.m_OBB.Velocity;
                    Vector3 acceleration = this.m_OBB.Acceleration;

                    if (this.m_OBB.Position.Y > this.MaxFlightHeight)
                    {
                        acceleration = Constants.GravityForce;
                    }
                    else if (this.m_OBB.Position.Y < this.MinFlightHeight)
                    {
                        if (velocity.Y < 0f)
                        {
                            if (velocity.Y < -1.5f)
                            {
                                //Descendiendo, frenar y ascender
                                velocity.Y += -(velocity.Y * 0.5f);
                            }
                            else
                            {
                                velocity.Y += 0.5f;
                                acceleration.Y = 0f;
                            }
                        }
                        else
                        {
                            float distance = this.MinFlightHeight - this.m_OBB.Position.Y;

                            velocity.Y = 0.5f * distance;
                            acceleration.Y = 0f;
                        }
                    }

                    this.m_OBB.SetVelocity(velocity);
                    this.m_OBB.SetAcceleration(acceleration);
                }
            }
        }
        /// <summary>
        /// Evento que se produce al ser contactado por otro objeto
        /// </summary>
        public event ObjectInContactDelegate OnObjectContacted;
        /// <summary>
        /// Cuando el vehículo es contactado por otro, se notifica el causante del contacto
        /// </summary>
        /// <param name="obj">Objeto que ha contactado con el vehículo actual</param>
        public void Contacted(IPhysicObject obj)
        {
            if (this.OnObjectContacted != null)
            {
                this.OnObjectContacted(obj);
            }
        }

        /// <summary>
        /// Establece la posición inicial
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="orientation">Orientación</param>
        public virtual void SetInitialState(Vector3 position, Quaternion orientation)
        {
            if (this.m_OBB != null)
            {
                this.m_OBB.SetInitialState(position, orientation);
            }
        }

        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Accelerate(GameTime gameTime)
        {
            this.Accelerate(gameTime, this.AccelerationModifier);
        }
        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="amount">Cantidad de movimiento a añadir</param>
        public virtual void Accelerate(GameTime gameTime, float amount)
        {
            if (!this.Destroyed)
            {
                if (amount != 0f)
                {
                    float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (this.IsAdvancing)
                    {
                        if (this.Velocity < this.MaxForwardVelocity)
                        {
                            this.m_OBB.AddToVelocity(this.CurrentTransform.Forward * amount / time);
                        }
                    }
                    else
                    {
                        if (this.Velocity < this.MaxBackwardVelocity)
                        {
                            this.m_OBB.AddToVelocity(this.CurrentTransform.Backward * amount / time);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Brake(GameTime gameTime)
        {
            this.Brake(gameTime, this.BrakeModifier);
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="amount">Cantidad de movimiento a disminuir</param>
        public virtual void Brake(GameTime gameTime, float amount)
        {
            if (!this.Destroyed)
            {
                if (amount != 0f)
                {
                    float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    float toAdd = (0.0001f * amount * time);

                    if (this.Velocity >= toAdd)
                    {
                        if (this.IsAdvancing)
                        {
                            this.m_OBB.AddToVelocity(this.CurrentTransform.Backward * toAdd);
                        }
                        else
                        {
                            this.m_OBB.AddToVelocity(this.CurrentTransform.Forward * toAdd);
                        }
                    }
                    else if (this.Velocity > 0f)
                    {
                        this.m_OBB.SetVelocity(Vector3.Zero);
                    }
                }
            }
        }

        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void TurnLeft(GameTime gameTime)
        {
            this.TurnLeft(gameTime, this.AngularVelocityModifier);
        }
        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnLeft(GameTime gameTime, float angle)
        {
            if (!this.Destroyed)
            {
                if (angle != 0f)
                {
                    float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    //Rotación a añadir
                    Quaternion toAdd = Quaternion.CreateFromAxisAngle(this.CurrentTransform.Up, angle / time);

                    //Añadir a la rotación actual
                    this.m_OBB.AddToOrientation(toAdd);

                    //Actualizar velocidad
                    this.m_OBB.SetVelocity(Vector3.Transform(this.m_OBB.Velocity, toAdd));
                }
            }
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void TurnRight(GameTime gameTime)
        {
            this.TurnRight(gameTime, this.AngularVelocityModifier);
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnRight(GameTime gameTime, float angle)
        {
            if (!this.Destroyed)
            {
                if (angle != 0f)
                {
                    float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    //Rotación a añadir
                    Quaternion toAdd = Quaternion.CreateFromAxisAngle(this.CurrentTransform.Up, -angle / time);

                    //Añadir a la rotación actual
                    this.m_OBB.AddToOrientation(toAdd);

                    //Actualizar velocidad
                    this.m_OBB.SetVelocity(Vector3.Transform(this.m_OBB.Velocity, toAdd));
                }
            }
        }

        /// <summary>
        /// Mueve el vehículo hacia arriba
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void GoUp(GameTime gameTime)
        {
            this.GoUp(gameTime, this.AccelerationModifier);
        }
        /// <summary>
        /// Mueve el vehículo hacia arriba la cantidad especificada
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="amount">Cantidad</param>
        public virtual void GoUp(GameTime gameTime, float amount)
        {
            if (!this.Destroyed)
            {
                if (amount != 0f)
                {
                    float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    this.m_OBB.AddToVelocity(this.CurrentTransform.Up * amount / time);
                }
            }
        }
        /// <summary>
        /// Mueve el vehículo hacia abajo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void GoDown(GameTime gameTime)
        {
            this.GoDown(gameTime, this.AccelerationModifier);
        }
        /// <summary>
        /// Mueve el vehículo hacia abajo la cantidad especificada
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="amount">Cantidad</param>
        public virtual void GoDown(GameTime gameTime, float amount)
        {
            if (!this.Destroyed)
            {
                if (amount != 0f)
                {
                    float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    this.m_OBB.AddToVelocity(this.CurrentTransform.Down * amount / time);
                }
            }
        }
    }
}
