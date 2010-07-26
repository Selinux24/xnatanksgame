using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Explosión en dos fases: Implosión y Explosión
    /// </summary>
    public class Explosion : ForceGenerator
    {
        /// <summary>
        /// Tiempo que la explosión ha estado activa
        /// </summary>
        float m_TimePassed = 0f;
        /// <summary>
        /// Indica si la explosión está activa
        /// </summary>
        bool m_ExplosionActive = true;

        /// <summary>
        /// The location of the detonation of the weapon.
        /// Posición de la detonación
        /// </summary>
        public Vector3 DetonationCenter;

        /// <summary>
        /// Radio en el que los objetos son atraídos en la primera fase de la explosión
        /// </summary>
        public float ImplosionMaxRadius;
        /// <summary>
        /// Radio en el que los objetos no son atraídos en la implosión, por estar atrapados en el epicentro
        /// </summary>
        public float ImplosionMinRadius;
        /// <summary>
        /// Duración de la fase de la implosión
        /// </summary>
        public float ImplosionDuration;
        /// <summary>
        /// La fuerza máxima que se aplica en la implosión
        /// </summary>
        public float ImplosionForce;

        /// <summary>
        /// Velocidad a la que viaja la honda expansiva
        /// </summary>
        /// <remarks>
        /// La anchura de la honda expansiva tiene que ser mayor o igual que la distancia máxima que pueda alcanzar
        /// Anchura >= Velocidad * Duración
        /// </remarks>
        public float ShockwaveSpeed;
        /// <summary>
        /// Anchura de la honda expansiva
        /// </summary>
        /// <remarks>
        /// Cuanto más rápida sea la honda, más ancha debe ser la honda
        /// </remarks>
        public float ShockwaveThickness;
        /// <summary>
        /// Fuerza aplicada en el centro de la honda expansiva sobre un objeto estacionario.
        /// Los objetos que están delante o detrás de la honda obtienen proporcionalmente menos fuerza.
        /// </summary>
        public float PeakConcussionForce;
        /// <summary>
        /// El tiempo que dura la fase de honda expansiva. Cuanto más se acerca al final, la honda es menos potente
        /// </summary>
        public float ConcussionDuration;

        /// <summary>
        /// Obtiene si la explosión está activa
        /// </summary>
        public bool IsActive
        {
            get
            {
                return m_ExplosionActive;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Explosion()
        {

        }

        /// <summary>
        /// Calcula la fuerza y la aplica al objeto especificado
        /// </summary>
        /// <param name="obj">Objeto</param>
        /// <param name="duration">Duración</param>
        public override void UpdateForce(ref IPhysicObject obj, float duration)
        {
            // Obtener el cuerpo del objeto
            CollisionPrimitive primitive = obj.GetPrimitive();
            if (primitive != null)
            {
                // Detectar la fase de la explosión en la que estamos
                if (this.m_TimePassed <= this.ImplosionDuration)
                {
                    // Fase de implosión

                    float distance = Vector3.Distance(primitive.Position, this.DetonationCenter);
                    if (distance > this.ImplosionMinRadius && distance <= this.ImplosionMaxRadius)
                    {
                        // El cuerpo está en el área de implosión. Aplicar las fuerzas
                        float max = this.ImplosionMaxRadius - this.ImplosionMinRadius;
                        float curr = distance - this.ImplosionMinRadius;
                        float forceMagnitude = curr / max;

                        Vector3 force = Vector3.Normalize(this.DetonationCenter - primitive.Position) * this.ImplosionForce * forceMagnitude;

                        primitive.AddForce(force);
                    }
                }
                else if (this.m_TimePassed <= (this.ImplosionDuration + this.ConcussionDuration))
                {
                    // Honda expansiva

                    // Intervalo actual de máxima acción de la honda
                    float min = this.ShockwaveSpeed * this.m_TimePassed;
                    float max = this.ShockwaveSpeed * (this.m_TimePassed + duration);

                    // Distancia al centro del objeto
                    float distance = Vector3.Distance(primitive.Position, this.DetonationCenter);

                    float totalDuration = this.ConcussionDuration + this.ImplosionDuration;
                    float maxDuration = this.ConcussionDuration;
                    float maxDistance = (this.ShockwaveSpeed * maxDuration) + this.ShockwaveThickness;

                    float forceMagnitude = 0f;
                    if (distance >= min && distance <= max)
                    {
                        // En plena honda expansiva. Se aplican las fuerzas atenuadas sólo por la duración
                        float relativeTime = 0f;
                        if (m_TimePassed < totalDuration)
                        {
                            relativeTime = 1f - (this.m_TimePassed / totalDuration);
                        }

                        forceMagnitude = this.PeakConcussionForce * relativeTime;
                    }
                    else if (distance < min)
                    {
                        // El objeto ha sido sobrepasado por la honda expansiva. Fuerza mínimamente atenuada
                        float relativeTime = 0f;
                        if (this.m_TimePassed < totalDuration)
                        {
                            relativeTime = 1f - (this.m_TimePassed / totalDuration);
                        }

                        forceMagnitude = this.PeakConcussionForce * relativeTime;
                    }
                    else if (distance > max && distance <= maxDistance)
                    {
                        // El objeto no ha sido alcanzado por la honda expansiva. Fuerza atenuada por el tiempo y la distancia
                        float relativeDistance = 0f;
                        if (distance < maxDistance)
                        {
                            relativeDistance = 1f - (distance / maxDistance);
                        }

                        float relativeTime = 0f;
                        if (this.m_TimePassed < totalDuration)
                        {
                            relativeTime = 1f - (this.m_TimePassed / totalDuration);
                        }

                        forceMagnitude = this.PeakConcussionForce * relativeDistance * relativeTime;
                    }

                    if (forceMagnitude > 0f)
                    {
                        Vector3 force = Vector3.Normalize(primitive.Position - this.DetonationCenter) * forceMagnitude;

                        primitive.AddForce(force);
                    }
                }
                else
                {
                    // Fin de la explosión
                    this.m_ExplosionActive = false;
                }
            }

            this.m_TimePassed += duration;
        }

        /// <summary>
        /// Crea una explosión nuclear en la posición especificada
        /// </summary>
        /// <param name="position">Posición</param>
        /// <returns>Devuelve la explosión creada</returns>
        public static Explosion CreateNuclearExplosion(Vector3 position)
        {
            Explosion explosion = new Explosion();

            explosion.DetonationCenter = position;

            explosion.ImplosionMaxRadius = 300f;
            explosion.ImplosionMinRadius = 10f;
            explosion.ImplosionDuration = 60f;
            explosion.ImplosionForce = 5000f;

            explosion.ShockwaveSpeed = 10f;
            explosion.ShockwaveThickness = 200f;
            explosion.PeakConcussionForce = 50000f;
            explosion.ConcussionDuration = 120f;

            return explosion;
        }
        /// <summary>
        /// Crea una explosión de artillería en la posición especificada
        /// </summary>
        /// <param name="position">Posición</param>
        /// <returns>Devuelve la explosión creada</returns>
        public static Explosion CreateArtilleryExplosion(Vector3 position)
        {
            Explosion explosion = new Explosion();

            explosion.DetonationCenter = position;

            explosion.ImplosionMaxRadius = 3f;
            explosion.ImplosionMinRadius = 1f;
            explosion.ImplosionDuration = 1f;
            explosion.ImplosionForce = 1000f;

            explosion.ShockwaveSpeed = 50f;
            explosion.ShockwaveThickness = 2f;
            explosion.PeakConcussionForce = 100000f;
            explosion.ConcussionDuration = 2f;

            return explosion;
        }
    }
}
