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
        private CollisionPrimitive m_CollisionPrimitive = null;

        /// <summary>
        /// Piloto automático
        /// </summary>
        private AutoPilot m_Autopilot = new AutoPilot();
        /// <summary>
        /// Motor
        /// </summary>
        private VehicleEngine Engine = new VehicleEngine();

        /// <summary>
        /// Obtiene la cantidad de velocidad
        /// </summary>
        public virtual float Velocity
        {
            get
            {
                return this.m_CollisionPrimitive.Velocity.Length();
            }
        }
        /// <summary>
        /// Obtiene el vector de dirección actual del vehículo
        /// </summary>
        public virtual Vector3 Direction
        {
            get
            {
                return Vector3.Normalize(this.m_CollisionPrimitive.Transform.Forward);
            }
        }
        /// <summary>
        /// Obtiene el vector de dirección contraria actual del vehículo
        /// </summary>
        public virtual Vector3 BackwardDirection
        {
            get
            {
                return Vector3.Normalize(this.m_CollisionPrimitive.Transform.Backward);
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
        /// Indica si el motor está encendido
        /// </summary>
        public virtual bool IsEngineStarted
        {
            get
            {
                return this.Engine.Active;
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
        /// Obtiene la primitiva de colisión del vehículo
        /// </summary>
        /// <returns>Devuelve la primitiva de colisión del vehículo</returns>
        public virtual CollisionPrimitive Primitive
        {
            get
            {
                return this.m_CollisionPrimitive;
            }
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
                BoundingSphere thisSPH = this.SPH;
                BoundingSphere otherSph = physicObject.SPH;
                if (thisSPH.Intersects(otherSph))
                {
                    return this.m_CollisionPrimitive;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene la caja alineada con los ejes que circunda el vehículo, en coordenadas del mundo
        /// </summary>
        /// <returns>Devuelve una caja alineada con los ejes</returns>
        public virtual BoundingBox AABB
        {
            get
            {
                return this.m_CollisionPrimitive.AABB;
            }
        }
        /// <summary>
        /// Obtiene la esfera que circunda el vehículo, en coordenadas del mundo
        /// </summary>
        /// <returns>Devuelve una esfera</returns>
        public virtual BoundingSphere SPH
        {
            get
            {
                return this.m_CollisionPrimitive.SPH;
            }
        }
        /// <summary>
        /// Obtiene si el vehículo está estático
        /// </summary>
        /// <returns>Devuelve verdadero si está estático</returns>
        public virtual bool IsActive
        {
            get
            {
                return this.m_CollisionPrimitive.IsAwake;
            }
        }
        /// <summary>
        /// Integra el cuerpo del vehículo en el tiempo
        /// </summary>
        /// <param name="time">Cantidad de tiempo</param>
        public void Integrate(float time)
        {
            if (this.m_CollisionPrimitive != null)
            {
                this.m_CollisionPrimitive.Integrate(time);
            }
        }

        /// <summary>
        /// Establece la posición inicial
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="orientation">Orientación</param>
        public virtual void SetInitialState(Vector3 position, Quaternion orientation)
        {
            if (this.m_CollisionPrimitive != null)
            {
                this.m_CollisionPrimitive.SetInitialState(position, orientation);
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
                this.StartMoving(this, this.m_CollisionPrimitive.Velocity, this.m_CollisionPrimitive.Acceleration);
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
                this.Accelerating(this, this.m_CollisionPrimitive.Velocity, this.m_CollisionPrimitive.Acceleration);
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
                this.Braking(this, this.m_CollisionPrimitive.Velocity, this.m_CollisionPrimitive.Acceleration);
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
                this.StopMoving(this, this.m_CollisionPrimitive.Velocity, this.m_CollisionPrimitive.Acceleration);
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
            if (!this.IsDestroyed)
            {
                this.Engine.Start();

                this.FireStartMoving();
            }
        }
        /// <summary>
        /// Apaga el motor
        /// </summary>
        public void StopEngine()
        {
            if (!this.IsDestroyed)
            {
                this.Engine.Stop();

                this.FireStopMoving();
            }
        }

        /// <summary>
        /// Cambia la dirección del vehículo
        /// </summary>
        /// <param name="movingDirection">Dirección</param>
        public void ChangeDirection(MovingDirections movingDirection)
        {
            this.Engine.ChangeDirection(movingDirection);
        }

        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Accelerate(GameTime gameTime)
        {
            if (!this.IsDestroyed)
            {
                this.Engine.Accelerate(gameTime);
            }
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Brake(GameTime gameTime)
        {
            if (!this.IsDestroyed)
            {
                this.Engine.Decelerate(gameTime);
            }
        }

        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void TurnLeft(GameTime gameTime)
        {
            if (!this.IsDestroyed)
            {
                this.Engine.TurnLeft(gameTime);
            }
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void TurnRight(GameTime gameTime)
        {
            if (!this.IsDestroyed)
            {
                this.Engine.TurnRight(gameTime);
            }
        }

        /// <summary>
        /// Gira el modelo hacia arriba
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void TurnUp(GameTime gameTime)
        {
            if (!this.IsDestroyed)
            {
                this.Engine.TurnUp(gameTime);
            }
        }
        /// <summary>
        /// Gira el modelo hacia abajo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void TurnDown(GameTime gameTime)
        {
            if (!this.IsDestroyed)
            {
                this.Engine.TurnDown(gameTime);
            }
        }

        /// <summary>
        /// Mueve el vehículo hacia arriba
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Elevate(GameTime gameTime)
        {
            if (!this.IsDestroyed)
            {
                this.Engine.Elevate(gameTime);
            }
        }
        /// <summary>
        /// Mueve el vehículo hacia abajo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Descend(GameTime gameTime)
        {
            if (!this.IsDestroyed)
            {
                this.Engine.Descend(gameTime);
            }
        }
    }
}
