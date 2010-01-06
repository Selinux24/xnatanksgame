using System;
using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Vehicles
{
    /// <summary>
    /// Control de físicas de un vehículo
    /// </summary>
    public partial class Vehicle : IVehicleController, IPhysicObject
    {
        /// <summary>
        /// Colección de triángulos del modelo
        /// </summary>
        private TriangleInfo m_TriangleInfo = null;
        /// <summary>
        /// Esfera que contiene el vehículo
        /// </summary>
        private BoundingSphere m_Bsph;
        /// <summary>
        /// Primitiva de colisión
        /// </summary>
        private CollisionBox m_OBB = null;

        /// <summary>
        /// Velocidad de traslación
        /// </summary>
        //private float m_Velocity = 0f;
        /// <summary>
        /// Velocidad de rotación a aplicar
        /// </summary>
        //private float m_AngularVelocity = 0f;
        /// <summary>
        /// Vector velocidad
        /// </summary>
        //private Vector3 m_Direction = Vector3.Forward;
        /// <summary>
        /// Inclinación
        /// </summary>
        //private Quaternion m_Inclination = Quaternion.Identity;

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

        // test - Número de frames desde la última actualización con el terreno
        //private int m_FramesSinceLastUpdate = 0;
        // test - Máximo número de frames para actualizar con el terreno
        //private int m_MaxFrames = 12;

        /// <summary>
        /// Piloto automático
        /// </summary>
        private AutoPilot m_Autopilot = new AutoPilot();
        /// <summary>
        /// Dirección de movimiento del tanque
        /// </summary>
        private MovingDirections m_MovingDirection = MovingDirections.Forward;

        /// <summary>
        /// Obtiene la información de primitivas del modelo
        /// </summary>
        public TriangleInfo TriangleInfo
        {
            get
            {
                return this.m_TriangleInfo;
            }
        }

        /// <summary>
        /// Obtiene la cantidad de velocidad
        /// </summary>
        public virtual float Velocity
        {
            get
            {
                //return this.m_Velocity;
                return this.OBB.Body.Velocity.Length();
            }
        }
        /// <summary>
        /// Obtiene el vector de dirección actual del vehículo
        /// </summary>
        public virtual Vector3 Direction
        {
            get
            {
                //return this.m_Direction;
                return Vector3.Normalize(this.OBB.Body.Transform.Forward);
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
        /// Obtiene la esfera circundante del modelo con la transformación aplicada
        /// </summary>
        public BoundingSphere BSph
        {
            get
            {
                return this.m_Bsph;
            }
        }
        /// <summary>
        /// Obtiene la esfera circundante del modelo con la transformación aplicada
        /// </summary>
        public BoundingSphere TransformedBSph
        {
            get
            {
                BoundingSphere sphere = this.BSph;

                Vector3 center = Vector3.Transform(sphere.Center, this.CurrentTransform);
                float radius = sphere.Radius;

                return new BoundingSphere(center, radius);
            }
        }
        /// <summary>
        /// Obtiene la caja orientada con la transformación aplicada
        /// </summary>
        public CollisionBox OBB
        {
            get
            {
                return this.m_OBB;
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
        /// Añade la cantidad especificada a la velocidad de traslación del modelo
        /// </summary>
        /// <param name="velocity">Velocidad</param>
        //protected virtual void AddToVelocity(float velocity)
        //{
        //    if (this.IsAdvancing)
        //    {
        //        this.m_Velocity = MathHelper.Clamp(this.m_Velocity + velocity, 0.0f, this.MaxForwardVelocity);
        //    }
        //    else
        //    {
        //        this.m_Velocity = MathHelper.Clamp(this.m_Velocity + velocity, 0.0f, this.MaxBackwardVelocity);
        //    }
        //}
        /// <summary>
        /// Actualiza la posición con respecto a la velocidad y el vector dirección
        /// </summary>
        //protected virtual void UpdatePosition(GameTime gameTime)
        //{
        //    this.m_Position = CalcPositionValue(gameTime, this.m_Position, this.m_Direction, this.m_Velocity);
        //}
        /// <summary>
        /// Calcula la posición con respecto a la dirección y la velocidad
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="currentPosition">Posición actual</param>
        /// <param name="direction">Dirección</param>
        /// <param name="velocity">Velocidad</param>
        /// <returns>Devuelve la nueva posición</returns>
        //private Vector3 CalcPositionValue(GameTime gameTime, Vector3 currentPosition, Vector3 direction, float velocity)
        //{
        //    Vector3 newPosition = currentPosition;

        //    if (velocity != 0f)
        //    {
        //        // Calcular la velocidad a aplicar este paso
        //        float timedVelocity = velocity / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        //        if (!float.IsInfinity(timedVelocity))
        //        {
        //            // Trasladar la posición
        //            newPosition += Vector3.Multiply(direction, timedVelocity);

        //            // La posición ha sido modificada
        //            this.PositionHasChanged = true;
        //        }
        //    }

        //    return newPosition;
        //}

        /// <summary>
        /// Actualizar la rotación
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        //protected virtual void UpdateRotation(GameTime gameTime)
        //{
        //    if (this.m_AngularVelocity != 0f)
        //    {
        //        // Obtener el ángulo a aplicar
        //        float timedAngularVelocity = this.m_AngularVelocity / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        //        if (!float.IsInfinity(timedAngularVelocity))
        //        {
        //            // Calcular el Quaternion de la rotación parcial
        //            Quaternion partialRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, timedAngularVelocity);

        //            // Modificar la rotación
        //            this.m_Rotation = Quaternion.Multiply(this.m_Rotation, partialRotation);

        //            // Modificar la dirección
        //            this.m_Direction = Vector3.Transform(this.m_Direction, partialRotation);

        //            // Indicar que la rotación ha sido modificada
        //            this.RotationHasChanged = true;
        //        }

        //        // Inicializar la velocidad angular
        //        this.m_AngularVelocity = 0f;
        //    }
        //}
        /// <summary>
        /// Actualiza el vehículo con el escenario
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        //protected virtual void UpdateWithScenery(GameTime gameTime)
        //{
        //    if (!this.Initialized)
        //    {
        //        if (!this.Skimmer)
        //        {
        //            this.UpdateDefaultWithScenery(gameTime);
        //        }
        //        else
        //        {
        //            this.UpdateSkimmerWithScenery(gameTime);
        //        }

        //        this.Initialized = true;
        //    }
        //    else if (!this.Destroyed)
        //    {
        //        if (this.PositionHasChanged || this.RotationHasChanged || this.ScaleHasChanged)
        //        {
        //            if (!this.Skimmer)
        //            {
        //                if (this.Visible)
        //                {
        //                    this.UpdateDefaultWithScenery(gameTime);
        //                }
        //                else
        //                {
        //                    this.m_FramesSinceLastUpdate++;

        //                    if (this.m_FramesSinceLastUpdate >= this.m_MaxFrames)
        //                    {
        //                        this.UpdateDefaultWithScenery(gameTime);

        //                        this.m_FramesSinceLastUpdate = 0;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                this.UpdateSkimmerWithScenery(gameTime);
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// Actualiza el componente con el escenario
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        //protected virtual void UpdateDefaultWithScenery(GameTime gameTime)
        //{
        //    Triangle? tri = null;
        //    Vector3? point = null;
        //    float? distance = null;

        //    if (this.Scenery.Intersects(this.m_Position.X, this.m_Position.Z, out tri, out point, out distance))
        //    {
        //        // Obtener la normal actual
        //        Vector3 currentNormal = Matrix.CreateFromQuaternion(m_Rotation).Up;

        //        // Obtener la normal del triángulo
        //        Vector3 newNormal = tri.Value.Normal;

        //        // Calcular inclinación a aplicar entre las dos normales
        //        Quaternion newInclination = this.CalcInclinationQuaternion(currentNormal, newNormal);

        //        if (!this.Initialized)
        //        {
        //            this.m_Position = point.Value;

        //            this.m_Inclination = newInclination;
        //        }
        //        else
        //        {
        //            // Establecer la posición definitiva
        //            this.m_Position = point.Value;

        //            // Obtener factor de interpolación dependiendo de la velocidad
        //            float slerpFactor = this.CalcInclinationSlerpFactor(25f);

        //            // Establecer la interpolación entre la nueva inclinación y la posición vertical
        //            this.m_Inclination = Quaternion.Slerp(this.m_Inclination, newInclination, slerpFactor);
        //        }
        //    }
        //}
        /// <summary>
        /// Actualiza el componente volador con el escenario
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        //protected virtual void UpdateSkimmerWithScenery(GameTime gameTime)
        //{
        //    Triangle? currentTri = null;
        //    Vector3? currentPoint = null;
        //    float? currentDistance = null;
        //    if (this.Scenery.Intersects(this.m_Position.X, this.m_Position.Z, out currentTri, out currentPoint, out currentDistance))
        //    {
        //        if (!this.Initialized)
        //        {
        //            this.m_Position = currentPoint.Value + (Vector3.Up * this.MinFlightHeight);

        //            this.m_Inclination = Quaternion.Identity;
        //        }
        //        else
        //        {
        //            // Obtener la posición futura
        //            Vector3 futurePosition = this.CalcPositionValue(gameTime, this.m_Position, this.m_Direction, this.m_Velocity);

        //            // Obtener la intersección con la posición futura y el suelo
        //            Triangle? nextTri = null;
        //            Vector3? nextPoint = null;
        //            float? nextDistance = null;
        //            if (this.Scenery.Intersects(futurePosition.X, futurePosition.Z, out nextTri, out nextPoint, out nextDistance))
        //            {
        //                // Altura actual
        //                float currentHeight = (this.m_Position.Y - currentPoint.Value.Y);
        //                float nextHeight = (futurePosition.Y - nextPoint.Value.Y);

        //                float diff = 0;
        //                if (currentHeight < 0)
        //                {
        //                    this.Destroyed = true;
        //                }
        //                else if (currentHeight < this.MinFlightHeight)
        //                {
        //                    // Hay que elevarse rápidamente
        //                    diff = (this.MinFlightHeight - currentHeight) * 0.25f;
        //                }
        //                else if (currentHeight > this.MaxFlightHeight)
        //                {
        //                    // Hay que bajar rápidamente
        //                    diff = (this.MaxFlightHeight - currentHeight) * 0.005f;
        //                }

        //                // Agregar la nueva altura a la posición
        //                this.m_Position.Y += diff;

        //                // Obtener la normal actual
        //                Matrix currentRotation = Matrix.CreateFromQuaternion(m_Rotation);
        //                Vector3 currentNormal = currentRotation.Up;
        //                Vector3 currentAxis = currentRotation.Right;
        //                Vector3 newNormal = Vector3.Up;
        //                float slerpSeed = 0f;
        //                if (Math.Abs(diff) <= 0.1f)
        //                {
        //                    // Sin inclinación
        //                    newNormal = Vector3.Up;
        //                    // Interpolación rápida
        //                    slerpSeed = 25f;
        //                }
        //                else if (diff > 0f)
        //                {
        //                    // Inclinación ascendente
        //                    newNormal = Matrix.CreateFromAxisAngle(currentAxis, this.AscendingAngle).Up;
        //                    // Interpolación rápida
        //                    slerpSeed = 25f;
        //                }
        //                else if (diff < 0f)
        //                {
        //                    // Inclinación descendente
        //                    newNormal = Matrix.CreateFromAxisAngle(currentAxis, -this.DescendingAngle).Up;
        //                    // Interpolación lenta
        //                    slerpSeed = 205f;
        //                }

        //                // Nueva inclinación
        //                Quaternion newInclination = this.CalcInclinationQuaternion(currentNormal, newNormal);

        //                // Obtener factor de interpolación dependiendo de la velocidad
        //                float slerpFactor = this.CalcInclinationSlerpFactor(slerpSeed);

        //                // Establecer la interpolación entre la nueva inclinación y la posición vertical
        //                this.m_Inclination = Quaternion.Slerp(this.m_Inclination, newInclination, slerpFactor);
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// Obtiene el factor para aplicar la inclinación según la velocidad
        /// </summary>
        //protected virtual float CalcInclinationSlerpFactor(float factor)
        //{
        //    if (this.m_Velocity == 0f)
        //    {
        //        return 1f;
        //    }
        //    else
        //    {
        //        if (this.m_MovingDirection == MovingDirections.Forward)
        //        {
        //            return this.m_Velocity / this.MaxForwardVelocity / factor;
        //        }
        //        else if (this.m_MovingDirection == MovingDirections.Backward)
        //        {
        //            return this.m_Velocity / this.MaxBackwardVelocity / factor;
        //        }
        //        else
        //        {
        //            return 0.05f;
        //        }
        //    }
        //}
        /// <summary>
        /// Obtiene el Quaterion de inclinación usando las dos normales especificadas
        /// </summary>
        /// <param name="normal1">Normal 1</param>
        /// <param name="normal2">Normal 2</param>
        /// <returns>Devuelve el cuaternion que representa la inclinación entre las dos normales</returns>
        //protected virtual Quaternion CalcInclinationQuaternion(Vector3 normal1, Vector3 normal2)
        //{
        //    Quaternion inclination = Quaternion.Identity;

        //    // Angulo que forman las dos normales
        //    float angle = (float)Math.Acos(Vector3.Dot(normal1, normal2));
        //    if (angle != 0.0f)
        //    {
        //        // Eje entre las dos normales, la nueva y la actual
        //        Vector3 axis = Vector3.Normalize(Vector3.Cross(normal1, normal2));

        //        // Inclinación
        //        inclination = Quaternion.CreateFromAxisAngle(axis, angle);
        //    }

        //    return inclination;
        //}

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
        /// Obtiene la primitiva de colisión del vehículo
        /// </summary>
        /// <returns>Devuelve la primitiva de colisión del vehículo</returns>
        public virtual CollisionPrimitive GetPrimitive()
        {
            return this.OBB;
        }
        /// <summary>
        /// Obtiene la esfera circundate del vehículo
        /// </summary>
        /// <returns>Devuelve la esfera circundate del vehículo</returns>
        public virtual BoundingSphere GetBoundingSphere()
        {
            return this.TransformedBSph;
        }
        /// <summary>
        /// Integra el cuerpo del vehículo en el tiempo
        /// </summary>
        /// <param name="time">Cantidad de tiempo</param>
        public void Integrate(float time)
        {
            if (this.OBB != null)
            {
                this.OBB.Body.Integrate(time);
                this.OBB.CalculateInternals();
            }
        }

        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        public virtual void Accelerate()
        {
            this.Accelerate(this.AccelerationModifier);
        }
        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        /// <param name="amount">Cantidad de movimiento a añadir</param>
        public virtual void Accelerate(float amount)
        {
            if (!this.Destroyed)
            {
                //this.AddToVelocity(amount);

                if (amount != 0f)
                {
                    if (this.IsAdvancing)
                    {
                        if (this.Velocity < this.MaxForwardVelocity)
                        {
                            this.OBB.Body.Velocity += this.CurrentTransform.Forward * amount;
                        }
                    }
                    else
                    {
                        if (this.Velocity < this.MaxBackwardVelocity)
                        {
                            this.OBB.Body.Velocity += this.CurrentTransform.Backward * amount;
                        }
                    }

                    this.OBB.Body.IsAwake = true;
                }
            }
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        public virtual void Brake()
        {
            this.Brake(this.BrakeModifier);
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        /// <param name="amount">Cantidad de movimiento a disminuir</param>
        public virtual void Brake(float amount)
        {
            if (!this.Destroyed)
            {
                //this.AddToVelocity(-amount);

                if (amount != 0f)
                {
                    if (this.Velocity >= amount)
                    {
                        if (this.IsAdvancing)
                        {
                            this.OBB.Body.Velocity -= this.CurrentTransform.Forward * amount;
                        }
                        else
                        {
                            this.OBB.Body.Velocity -= this.CurrentTransform.Backward * amount;
                        }
                    }

                    this.OBB.Body.IsAwake = true;
                }
            }
        }

        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        public virtual void TurnLeft()
        {
            this.TurnLeft(this.AngularVelocityModifier);
        }
        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnLeft(float angle)
        {
            if (!this.Destroyed)
            {
                //this.m_AngularVelocity = MathHelper.Clamp(angle, -this.AngularVelocityModifier, this.AngularVelocityModifier);

                if (angle != 0f)
                {
                    this.OBB.Body.Orientation *= Quaternion.CreateFromAxisAngle(this.CurrentTransform.Up, angle);

                    this.OBB.Body.IsAwake = true;
                }
            }
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        public virtual void TurnRight()
        {
            this.TurnRight(this.AngularVelocityModifier);
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnRight(float angle)
        {
            if (!this.Destroyed)
            {
                //this.m_AngularVelocity = MathHelper.Clamp(-angle, -this.AngularVelocityModifier, this.AngularVelocityModifier);

                if (angle != 0f)
                {
                    this.OBB.Body.Orientation *= Quaternion.CreateFromAxisAngle(this.CurrentTransform.Up, -angle);

                    this.OBB.Body.IsAwake = true;
                }
            }
        }

        /// <summary>
        /// Mueve el vehículo hacia arriba
        /// </summary>
        public virtual void GoUp()
        {
            this.GoUp(this.AccelerationModifier);
        }
        /// <summary>
        /// Mueve el vehículo hacia arriba la cantidad especificada
        /// </summary>
        /// <param name="amount">Cantidad</param>
        public virtual void GoUp(float amount)
        {
            if (!this.Destroyed)
            {
                if (amount != 0f)
                {
                    this.OBB.Body.Velocity += this.CurrentTransform.Up * amount;

                    this.OBB.Body.IsAwake = true;
                }
            }
        }
        /// <summary>
        /// Mueve el vehículo hacia abajo
        /// </summary>
        public virtual void GoDown()
        {
            this.GoDown(this.AccelerationModifier);
        }
        /// <summary>
        /// Mueve el vehículo hacia abajo la cantidad especificada
        /// </summary>
        /// <param name="amount">Cantidad</param>
        public virtual void GoDown(float amount)
        {
            if (!this.Destroyed)
            {
                if (amount != 0f)
                {
                    this.OBB.Body.Velocity += this.CurrentTransform.Down * amount;

                    this.OBB.Body.IsAwake = true;
                }
            }
        }
    }
}
