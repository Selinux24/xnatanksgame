using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    using Common;
    using Common.Components;
    using Common.Helpers;
    using Physics;
    using Physics.CollideCoarse;

    /// <summary>
    /// Control de físicas de un vehículo
    /// </summary>
    public partial class Vehicle : IVehicleController, IVehicle
    {
        /// <summary>
        /// Primitiva de colisión
        /// </summary>
        private CollisionBox m_OBB = null;

        /// <summary>
        /// Piloto automático
        /// </summary>
        private AutoPilot m_Autopilot = new AutoPilot();

        /// <summary>
        /// Motor
        /// </summary>
        protected VehicleEngine Engine = new VehicleEngine();

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
        /// Obtiene el vector de dirección contraria actual del vehículo
        /// </summary>
        public virtual Vector3 BackwardDirection
        {
            get
            {
                return Vector3.Normalize(this.m_OBB.Transform.Backward);
            }
        }
        /// <summary>
        /// Obtiene la dirección relativa del vehículo
        /// </summary>
        public virtual MovingDirections MovingDirection
        {
            get
            {
                return this.Engine.MovingDirection;
            }
        }
        /// <summary>
        /// Obtiene si el objeto está avanzando
        /// </summary>
        public virtual bool IsAdvancing
        {
            get
            {
                return (this.Engine.MovingDirection == MovingDirections.Forward);
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
        /// Indica si el vehículo es volador
        /// </summary>
        public bool IsSkimmer
        {
            get
            {
                return (this.Engine.Skimmer);
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
        /// Obtiene la posición
        /// </summary>
        /// <returns>Devuelve la posición</returns>
        public Vector3 GetPosition()
        {
            return this.Position;
        }
        /// <summary>
        /// Obtiene la orientación
        /// </summary>
        /// <returns>Devuelve la orientación</returns>
        public Quaternion GetOrientation()
        {
            return this.Orientation;
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

                if (this.Engine.Skimmer)
                {
                    this.IntegrateSkimmer();
                }
            }
        }
        /// <summary>
        /// Postintegración para vehículos voladores
        /// </summary>
        private void IntegrateSkimmer()
        {
            if (!this.IsDestroyed)
            {
                //Actualizar la altura
                PhysicsController physicsController = this.Game.Services.GetService<PhysicsController>();
                if (physicsController != null)
                {
                    float? height = physicsController.Scenery.GetHeigthAtPoint(this.Position.X, this.Position.Z);
                    if (height.HasValue)
                    {
                        this.Engine.MinFlightHeight = this.Engine.InitialMinFlightHeight + height.Value;
                        this.Engine.MaxFlightHeight = this.Engine.InitialMaxFlightHeight + height.Value;
                    }
                }

                if (this.m_OBB.Position.Y > this.Engine.MaxFlightHeight)
                {
                    //Por encima del techo de vuelo. Bajar a plomo
                    this.m_OBB.SetAcceleration(Constants.GravityForce);
                }
                else if (this.m_OBB.Position.Y < this.Engine.MinFlightHeight)
                {
                    //Por debajo de la altura mínima, subir
                    Vector3 velocity = this.m_OBB.Velocity;
                    Vector3 acceleration = this.m_OBB.Acceleration;

                    if (velocity.Y < 0f)
                    {
                        if (velocity.Y < -1.5f)
                        {
                            //Descendiendo, frenar y ascender
                            velocity.Y += -(velocity.Y * 0.5f);

                            this.m_OBB.SetVelocity(velocity);
                        }
                        else
                        {
                            velocity.Y += 0.5f;
                            acceleration.Y = 0f;

                            this.m_OBB.SetVelocity(velocity);
                            this.m_OBB.SetAcceleration(acceleration);
                        }
                    }
                    else
                    {
                        float distance = this.Engine.MinFlightHeight - this.m_OBB.Position.Y;

                        velocity.Y = 0.5f * distance;
                        acceleration.Y = 0f;

                        this.m_OBB.SetVelocity(velocity);
                        this.m_OBB.SetAcceleration(acceleration);
                    }
                }
                else
                {
                    //En el área de vuelo, bajar despacio
                    this.m_OBB.SetAcceleration(Constants.GravityForce * 0.5f);
                }

                Quaternion currentOrientation = this.m_OBB.Orientation;

                float yaw = 0f;
                float pitch = 0f;
                float roll = 0f;
                currentOrientation.Decompose(out yaw, out pitch, out roll);

                Quaternion newRot = Quaternion.Slerp(this.m_OBB.Orientation, Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f), 0.1f);

                this.m_OBB.SetOrientation(newRot);
            }
            else
            {

                this.m_OBB.SetAcceleration(Constants.GravityForce);
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
        /// Evento que se produce al ser contactado por otro objeto
        /// </summary>
        public event ObjectInContactDelegate Contacted;
        /// <summary>
        /// Ocurre cuando un objeto se activa
        /// </summary>
        public event ObjectStateHandler Activated;
        /// <summary>
        /// Ocurre cuando un objeto se desactiva
        /// </summary>
        public event ObjectStateHandler Deactivated;
        /// <summary>
        /// Evento que se produce cuando el cuerpo empieza a moverse
        /// </summary>
        public event VehicleMovingHandler StartMoving;
        /// <summary>
        /// Evento que se produce cuando el cuerpo acelera
        /// </summary>
        public event VehicleMovingHandler Accelerating;
        /// <summary>
        /// Evento que se produce cuando el cuerpo decelera
        /// </summary>
        public event VehicleMovingHandler Braking;
        /// <summary>
        /// Evento que se produce cuando el cuerpo deja de moverse
        /// </summary>
        public event VehicleMovingHandler StopMoving;

        /// <summary>
        /// Cuando el vehículo es contactado por otro, se notifica el causante del contacto
        /// </summary>
        /// <param name="obj">Objeto que ha contactado con el vehículo actual</param>
        public void SetContactedWith(IPhysicObject obj)
        {
            if (obj is AmmoRound)
            {
                this.TakeDamage(obj as AmmoRound);
            }
            else if (obj is IVehicle)
            {
                //Obtener las velocidades y calcular la magnitud del choque
                if (this.Velocity > 15f)
                {
                    this.TakeDamage(this.Velocity, 0f);
                }
            }
            else if (obj is IScenery)
            {
                //Usar la velocidad del vehículo como magnitud del choque
                if (this.Velocity > 15f)
                {
                    this.TakeDamage(this.Velocity, 0f);
                }
            }

            if (this.Contacted != null)
            {
                this.Contacted(obj);
            }
        }
       
        /// <summary>
        /// Disparador del evento de activación
        /// </summary>
        protected void FireActivated()
        {
            if (this.Activated != null)
            {
                this.Activated(this);
            }

            this.OnActivated();
        }
        /// <summary>
        /// Disparador del evento de desactivación
        /// </summary>
        protected void FireDeactivated()
        {
            if (this.Deactivated != null)
            {
                this.Deactivated(this);
            }

            this.OnDeactivated();
        }
        /// <summary>
        /// Disparador de evento de comienzo de movimiento
        /// </summary>
        protected void FireStartMoving()
        {
            if (StartMoving != null)
            {
                this.StartMoving(this, this.m_OBB.Velocity, this.m_OBB.Acceleration);
            }

            this.OnStartMoving();
        }
        /// <summary>
        /// Disparador de evento de aceleración
        /// </summary>
        protected void FireAccelerating()
        {
            if (Accelerating != null)
            {
                this.Accelerating(this, this.m_OBB.Velocity, this.m_OBB.Acceleration);
            }

            this.OnAccelerating();
        }
        /// <summary>
        /// Disparador de evento de deceleración
        /// </summary>
        protected void FireBraking()
        {
            if (Braking != null)
            {
                this.Braking(this, this.m_OBB.Velocity, this.m_OBB.Acceleration);
            }

            this.OnBraking();
        }
        /// <summary>
        /// Disparador de evento de fin de movimiento
        /// </summary>
        protected void FireStopMoving()
        {
            if (StopMoving != null)
            {
                this.StopMoving(this, this.m_OBB.Velocity, this.m_OBB.Acceleration);
            }

            this.OnStopMoving();
        }

        /// <summary>
        /// Se produce cuando el vehículo se activa
        /// </summary>
        protected virtual void OnActivated()
        {

        }
        /// <summary>
        /// Se produce cuando el vehículo se desactiva
        /// </summary>
        protected virtual void OnDeactivated()
        {

        }
        /// <summary>
        /// Se produce cuando el vehículo empieza a moverse
        /// </summary>
        protected virtual void OnStartMoving()
        {

        }
        /// <summary>
        /// Se produce cuando el vehículo acelera
        /// </summary>
        protected virtual void OnAccelerating()
        {

        }
        /// <summary>
        /// Se produce cuando el vehículo frena
        /// </summary>
        protected virtual void OnBraking()
        {

        }
        /// <summary>
        /// Se produce cuando el vehículo se para
        /// </summary>
        protected virtual void OnStopMoving()
        {

        }

        /// <summary>
        /// Enciende el motor
        /// </summary>
        public void StartEngine()
        {
            this.Engine.Active = true;

            this.FireStartMoving();
        }
        /// <summary>
        /// Apaga el motor
        /// </summary>
        public void StopEngine()
        {
            this.Engine.Active = false;

            this.FireStopMoving();
        }

        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Accelerate(GameTime gameTime)
        {
            this.Accelerate(gameTime, this.Engine.AccelerationModifier);
        }
        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="amount">Cantidad de movimiento a añadir</param>
        public virtual void Accelerate(GameTime gameTime, float amount)
        {
            if (this.Engine.Active && !this.IsDestroyed)
            {
                if (amount != 0f)
                {
                    float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    float toAdd = (amount / time);

                    //Proyección de la velocidad actual sobre la nueva velocidad
                    Vector3 direction = Vector3.Zero;
                    if (this.IsAdvancing)
                    {
                        direction = Vector3.Normalize(this.m_OBB.Transform.Forward);
                    }
                    else
                    {
                        direction = Vector3.Normalize(this.m_OBB.Transform.Backward);
                    }

                    if (this.Velocity < this.Engine.MaxForwardVelocity)
                    {
                        this.m_OBB.SetVelocity(direction * this.Velocity + direction * toAdd);
                    }

                    this.FireAccelerating();
                }
            }
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Brake(GameTime gameTime)
        {
            this.Brake(gameTime, this.Engine.BrakeModifier);
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="amount">Cantidad de movimiento a disminuir</param>
        public virtual void Brake(GameTime gameTime, float amount)
        {
            if (this.Engine.Active && !this.IsDestroyed && !this.IsStatic)
            {
                if (amount != 0f)
                {
                    if (!this.Velocity.IsZero())
                    {
                        Vector3 direction = Vector3.Zero;
                        if (this.IsAdvancing)
                        {
                            direction = Vector3.Normalize(this.m_OBB.Transform.Forward);
                        }
                        else
                        {
                            direction = Vector3.Normalize(this.m_OBB.Transform.Backward);
                        }

                        this.m_OBB.SetVelocity(direction * this.Velocity * amount);

                        this.FireBraking();
                    }
                    else
                    {
                        this.m_OBB.SetVelocity(Vector3.Zero);

                        this.FireBraking();
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
            this.TurnLeft(gameTime, this.Engine.AngularVelocityModifier);
        }
        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnLeft(GameTime gameTime, float angle)
        {
            if (this.Engine.Active && !this.IsDestroyed)
            {
                if (angle != 0f)
                {
                    float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    //Rotación a añadir
                    Quaternion toAdd = Quaternion.CreateFromAxisAngle(this.CurrentTransform.Up, angle / time);

                    //Añadir a la rotación actual
                    this.m_OBB.AddToOrientation(toAdd);

                    if (this.Engine.Skimmer)
                    {
                        this.m_OBB.SetVelocity(Vector3.Transform(this.m_OBB.Velocity, toAdd));
                    }

                    this.FireAccelerating();
                }
            }
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void TurnRight(GameTime gameTime)
        {
            this.TurnRight(gameTime, this.Engine.AngularVelocityModifier);
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnRight(GameTime gameTime, float angle)
        {
            if (this.Engine.Active && !this.IsDestroyed)
            {
                if (angle != 0f)
                {
                    float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    //Rotación a añadir
                    Quaternion toAdd = Quaternion.CreateFromAxisAngle(this.CurrentTransform.Up, -angle / time);

                    //Añadir a la rotación actual
                    this.m_OBB.AddToOrientation(toAdd);

                    if (this.Engine.Skimmer)
                    {
                        this.m_OBB.SetVelocity(Vector3.Transform(this.m_OBB.Velocity, toAdd));
                    }

                    this.FireAccelerating();
                }
            }
        }

        /// <summary>
        /// Mueve el vehículo hacia arriba
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void GoUp(GameTime gameTime)
        {
            this.GoUp(gameTime, this.Engine.AccelerationModifier);
        }
        /// <summary>
        /// Mueve el vehículo hacia arriba la cantidad especificada
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="amount">Cantidad</param>
        public virtual void GoUp(GameTime gameTime, float amount)
        {
            if (this.Engine.Active && !this.IsDestroyed)
            {
                float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                float toAdd = amount / time;

                if (toAdd != 0f)
                {
                    if (this.m_OBB.Position.Y < this.Engine.MaxFlightHeight)
                    {
                        this.m_OBB.AddToVelocity(this.CurrentTransform.Up * toAdd);

                        this.FireAccelerating();
                    }
                }
            }
        }
        /// <summary>
        /// Mueve el vehículo hacia abajo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void GoDown(GameTime gameTime)
        {
            this.GoDown(gameTime, this.Engine.AccelerationModifier);
        }
        /// <summary>
        /// Mueve el vehículo hacia abajo la cantidad especificada
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="amount">Cantidad</param>
        public virtual void GoDown(GameTime gameTime, float amount)
        {
            if (this.Engine.Active && !this.IsDestroyed)
            {
                float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                float toAdd = amount / time;

                if (toAdd != 0f)
                {
                    if (this.m_OBB.Position.Y > this.Engine.MinFlightHeight)
                    {
                        this.m_OBB.AddToVelocity(this.CurrentTransform.Down * toAdd);

                        this.FireAccelerating();
                    }
                }
            }
        }
    }
}
