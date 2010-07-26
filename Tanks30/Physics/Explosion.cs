using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Explosi�n en dos fases: Implosi�n y Explosi�n
    /// </summary>
    public class Explosion : ForceGenerator
    {
        /// <summary>
        /// Tiempo que la explosi�n ha estado activa
        /// </summary>
        float m_TimePassed = 0f;
        /// <summary>
        /// Indica si la explosi�n est� activa
        /// </summary>
        bool m_ExplosionActive = true;

        /// <summary>
        /// The location of the detonation of the weapon.
        /// Posici�n de la detonaci�n
        /// </summary>
        public Vector3 DetonationCenter;

        /// <summary>
        /// Radio en el que los objetos son atra�dos en la primera fase de la explosi�n
        /// </summary>
        public float ImplosionMaxRadius;
        /// <summary>
        /// Radio en el que los objetos no son atra�dos en la implosi�n, por estar atrapados en el epicentro
        /// </summary>
        public float ImplosionMinRadius;
        /// <summary>
        /// Duraci�n de la fase de la implosi�n
        /// </summary>
        public float ImplosionDuration;
        /// <summary>
        /// La fuerza m�xima que se aplica en la implosi�n
        /// </summary>
        public float ImplosionForce;

        /// <summary>
        /// Velocidad a la que viaja la honda expansiva
        /// </summary>
        /// <remarks>
        /// La anchura de la honda expansiva tiene que ser mayor o igual que la distancia m�xima que pueda alcanzar
        /// Anchura >= Velocidad * Duraci�n
        /// </remarks>
        public float ShockwaveSpeed;
        /// <summary>
        /// Anchura de la honda expansiva
        /// </summary>
        /// <remarks>
        /// Cuanto m�s r�pida sea la honda, m�s ancha debe ser la honda
        /// </remarks>
        public float ShockwaveThickness;
        /// <summary>
        /// Fuerza aplicada en el centro de la honda expansiva sobre un objeto estacionario.
        /// Los objetos que est�n delante o detr�s de la honda obtienen proporcionalmente menos fuerza.
        /// </summary>
        public float PeakConcussionForce;
        /// <summary>
        /// El tiempo que dura la fase de honda expansiva. Cuanto m�s se acerca al final, la honda es menos potente
        /// </summary>
        public float ConcussionDuration;

        /// <summary>
        /// Obtiene si la explosi�n est� activa
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
        /// <param name="duration">Duraci�n</param>
        public override void UpdateForce(ref IPhysicObject obj, float duration)
        {
            // Obtener el cuerpo del objeto
            CollisionPrimitive primitive = obj.GetPrimitive();
            if (primitive != null)
            {
                // Detectar la fase de la explosi�n en la que estamos
                if (this.m_TimePassed <= this.ImplosionDuration)
                {
                    // Fase de implosi�n

                    float distance = Vector3.Distance(primitive.Position, this.DetonationCenter);
                    if (distance > this.ImplosionMinRadius && distance <= this.ImplosionMaxRadius)
                    {
                        // El cuerpo est� en el �rea de implosi�n. Aplicar las fuerzas
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

                    // Intervalo actual de m�xima acci�n de la honda
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
                        // En plena honda expansiva. Se aplican las fuerzas atenuadas s�lo por la duraci�n
                        float relativeTime = 0f;
                        if (m_TimePassed < totalDuration)
                        {
                            relativeTime = 1f - (this.m_TimePassed / totalDuration);
                        }

                        forceMagnitude = this.PeakConcussionForce * relativeTime;
                    }
                    else if (distance < min)
                    {
                        // El objeto ha sido sobrepasado por la honda expansiva. Fuerza m�nimamente atenuada
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
                    // Fin de la explosi�n
                    this.m_ExplosionActive = false;
                }
            }

            this.m_TimePassed += duration;
        }

        /// <summary>
        /// Crea una explosi�n nuclear en la posici�n especificada
        /// </summary>
        /// <param name="position">Posici�n</param>
        /// <returns>Devuelve la explosi�n creada</returns>
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
        /// Crea una explosi�n de artiller�a en la posici�n especificada
        /// </summary>
        /// <param name="position">Posici�n</param>
        /// <returns>Devuelve la explosi�n creada</returns>
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
