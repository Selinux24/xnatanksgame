using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Clase de resolución de contactos
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class ContactResolver
    {
        /// <summary>
        /// Número de iteraciones a realizar al resolver la velocidad
        /// </summary>
        private int m_VelocityIterations = 2048;
        /// <summary>
        /// Número de iteraciones a realizar al resolver la posición
        /// </summary>
        private int m_PositionIterations = 2048;
        /// <summary>
        /// Para evitar inestabilidad, las velocidades menores a este valor son consideradas 0
        /// </summary>
        /// <remarks>Cuanto mayor sea este número, más se notará la interpenetración visualmente</remarks>
        private float m_VelocityEpsilon = 0.01f;
        /// <summary>
        /// Para evitar inestabilidad, las penetraciones menores a este valor se consideran como no interpenetraciones
        /// </summary>
        /// <remarks>Cuanto mayor sea este número, más se notará la interpenetración visualmente</remarks>
        private float m_PositionEpsilon = 0.01f;
        /// <summary>
        /// Almacena el número de iteraciones para resolver la velocidad utilizadas en la última llamada al resolutor de contactos
        /// </summary>
        private int m_VelocityIterationsUsed = 0;
        /// <summary>
        /// Almacena el número de iteraciones para resolver la posición utilizadas en la última llamada al resolutor de contactos
        /// </summary>
        private int m_PositionIterationsUsed = 0;

        /// <summary>
        /// Crea un nuevo resolutor con el número de iteraciones por llamada de resolución
        /// </summary>
        /// <param name="iterations">Número de iteraciones</param>
        public ContactResolver(int iterations)
            : this(iterations, iterations)
        {

        }
        /// <summary>
        /// Crea un nuevo resolutor con el número de iteraciones por llamada de resolución
        /// </summary>
        /// <param name="iterations">Número de iteraciones</param>
        /// <param name="velocityEpsilon">Épsilon para resolver problemas de velocidad</param>
        /// <param name="positionEpsilon">Épsilon para resolver problemas de posición</param>
        public ContactResolver(int iterations, float velocityEpsilon, float positionEpsilon)
            : this(iterations, iterations, velocityEpsilon, positionEpsilon)
        {

        }
        /// <summary>
        /// Crea un nuevo resolutor con el número de iteraciones por llamada de resolución
        /// </summary>
        /// <param name="velocityIterations">Número de iteraciones para resolución de velocidades</param>
        /// <param name="positionIterations">Número de iteraciones para resolución de posición</param>
        public ContactResolver(int velocityIterations, int positionIterations)
        {
            m_VelocityIterations = velocityIterations;
            m_PositionIterations = positionIterations;
        }
        /// <summary>
        /// Crea un nuevo resolutor con el número de iteraciones por llamada de resolución
        /// </summary>
        /// <param name="velocityIterations">Número de iteraciones para resolución de velocidades</param>
        /// <param name="positionIterations">Número de iteraciones para resolución de posición</param>
        /// <param name="velocityEpsilon">Épsilon para resolver problemas de velocidad</param>
        /// <param name="positionEpsilon">Épsilon para resolver problemas de posición</param>
        public ContactResolver(int velocityIterations, int positionIterations, float velocityEpsilon, float positionEpsilon)
        {
            m_VelocityIterations = velocityIterations;
            m_PositionIterations = positionIterations;
            m_VelocityEpsilon = velocityEpsilon;
            m_PositionEpsilon = positionEpsilon;
        }

        /// <summary>
        /// Resuelve una lista de contactos por penetración y velocidad
        /// </summary>
        /// <param name="contacts">Lista de contactos</param>
        /// <param name="duration">La duración del frame anterior para compensar las fuerzas aplicadas</param>
        /// <remarks>Los contactos que no pueden interactuar con el resto, deben resolverse en llamadas separadas, ya que el algoritmo de resolución trabaja mejor con listas pequeñas de contactos</remarks>
        public void ResolveContacts(ref CollisionData contacts, float duration)
        {
            if (contacts != null && contacts.ContactCount > 0)
            {
                if (this.IsValid())
                {
                    // Preparar los contactos para procesarlos
                    this.PrepareContacts(ref contacts, duration);

                    // Resolver problemas de interpenetración con los contactos
                    this.AdjustPositions(ref contacts, duration);

                    // Resolver problemas de velocidad con los contactos
                    this.AdjustVelocities(ref contacts, duration);
                }
            }
        }

        /// <summary>
        /// Obtiene si el resolutor está configurado correctamente
        /// </summary>
        /// <returns>Devuelve verdadero si el resolutor puede continuar</returns>
        private bool IsValid()
        {
            return (m_VelocityIterations > 0) &&
                   (m_PositionIterations > 0) &&
                   (m_PositionEpsilon >= 0.0f) &&
                   (m_PositionEpsilon >= 0.0f);
        }
        /// <summary>
        /// Prepara los contactos para procesarlos
        /// </summary>
        /// <param name="contacts">Lista de contactos</param>
        /// <param name="duration">Duración del frame anterior</param>
        /// <remarks>Esta función simplemente se asegura de que los contactos estén activos y sus datos internos actualizados</remarks>
        private void PrepareContacts(ref CollisionData contacts, float duration)
        {
            for (int i = 0; i < contacts.ContactCount; i++)
            {
                contacts.ContactArray[i].CalculateInternals(duration);
            }
        }
        /// <summary>
        /// Resuelve problemas posicionales de la lista de contactos
        /// </summary>
        /// <param name="contacts">Lista de contactos</param>
        /// <param name="duration">Duración del frame anterior</param>
        private void AdjustPositions(ref CollisionData contacts, float duration)
        {
            Vector3[] linearChange = new Vector3[2];
            Vector3[] angularChange = new Vector3[2];

            this.m_PositionIterationsUsed = 0;

            // Resolver las interpenetraciones por orden de severidad
            while (this.m_PositionIterationsUsed < this.m_PositionIterations)
            {
                // Encontrar la penetración mayor
                float max = this.m_PositionEpsilon;
                int index = contacts.ContactCount;
                for (int i = 0; i < contacts.ContactCount; i++)
                {
                    if (contacts.ContactArray[i].Penetration > max)
                    {
                        max = contacts.ContactArray[i].Penetration;
                        index = i;
                    }
                }

                if (index == contacts.ContactCount)
                {
                    break;
                }

                // Actualizar el estado del contacto
                contacts.ContactArray[index].MatchAwakeState();

                // Resolver la penetración
                contacts.ContactArray[index].ApplyPositionChange(ref linearChange, ref angularChange, max);

                for (int i = 0; i < contacts.ContactCount; i++)
                {
                    for (uint b = 0; b < 2; b++) if (contacts.ContactArray[i].Bodies[b] != null)
                        {
                            for (uint d = 0; d < 2; d++)
                            {
                                if (contacts.ContactArray[i].Bodies[b] == contacts.ContactArray[index].Bodies[d])
                                {
                                    Vector3 deltaPosition = linearChange[d] + Vector3.Cross(angularChange[d], contacts.ContactArray[i].RelativeContactPositions[b]);

                                    contacts.ContactArray[i].Penetration += Vector3.Dot(deltaPosition, contacts.ContactArray[i].ContactNormal) * (b != 0 ? 1 : -1);
                                }
                            }
                        }
                }

                this.m_PositionIterationsUsed++;
            }
        }
        /// <summary>
        /// Resuelve problemas de velocidad con la lista de contactos
        /// </summary>
        /// <param name="contacts">Lista de contactos</param>
        /// <param name="duration">Duración del frame anterior</param>
        private void AdjustVelocities(ref CollisionData contacts, float duration)
        {
            Vector3[] velocityChange = new Vector3[2];
            Vector3[] rotationChange = new Vector3[2];

            this.m_VelocityIterationsUsed = 0;

            // Resolver los impactos por ordende severidad
            while (this.m_VelocityIterationsUsed < this.m_VelocityIterations)
            {
                // Encontrar contactos con magnitud máxima de cambio probable de velocidad
                float max = this.m_VelocityEpsilon;
                int index = contacts.ContactCount;
                for (int i = 0; i < contacts.ContactCount; i++)
                {
                    if (contacts.ContactArray[i].DesiredDeltaVelocity > max)
                    {
                        max = contacts.ContactArray[i].DesiredDeltaVelocity;
                        index = i;
                    }
                }

                if (index == contacts.ContactCount)
                {
                    break;
                }

                // Actualizar el estado del contacto
                contacts.ContactArray[index].MatchAwakeState();

                // Resolver el contacto
                contacts.ContactArray[index].ApplyVelocityChange(ref velocityChange, ref rotationChange);

                for (int i = 0; i < contacts.ContactCount; i++)
                {
                    for (int b = 0; b < 2; b++) if (contacts.ContactArray[i].Bodies[b] != null)
                        {
                            for (int d = 0; d < 2; d++)
                            {
                                if (contacts.ContactArray[i].Bodies[b] == contacts.ContactArray[index].Bodies[d])
                                {
                                    Vector3 deltaVel = velocityChange[d] + Vector3.Cross(rotationChange[d], contacts.ContactArray[i].RelativeContactPositions[b]);

                                    // Si el signo del cambio es negativo, se trata del segundo cuerpo del contacto
                                    contacts.ContactArray[i].ContactVelocity += Matrix3.TransformTranspose(contacts.ContactArray[i].ContactToWorld, deltaVel) * (b != 0 ? -1 : 1);
                                    contacts.ContactArray[i].CalculateDesiredDeltaVelocity(duration);
                                }
                            }
                        }
                }

                this.m_VelocityIterationsUsed++;
            }
        }
    }
}