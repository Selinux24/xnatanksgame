using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Barra de uni�n
    /// </summary>
    public class Rod : ContactGenerator
    {
        /// <summary>
        /// Cuerpo uno
        /// </summary>
        private RigidBody m_BodyOne = null;
        /// <summary>
        /// Cuerpo dos
        /// </summary>
        private RigidBody m_BodyTwo = null;

        /// <summary>
        /// Posici�n de uni�n relativa al cuerpo uno
        /// </summary>
        private Vector3 m_PositionOne = Vector3.Zero;
        /// <summary>
        /// Posici�n de uni�n relativa al cuerpo dos
        /// </summary>
        private Vector3 m_PositionTwo = Vector3.Zero;

        /// <summary>
        /// Longitud de la barra
        /// </summary>
        private float m_Length = 0f;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bodyOne">Cuerpo uno</param>
        /// <param name="positionOne">Posici�n de uni�n relativa al cuerpo uno</param>
        /// <param name="bodyTwo">Cuerpo dos</param>
        /// <param name="positionTwo">Posici�n de uni�n relativa al cuerpo dos</param>
        /// <param name="length">Longitud de la barra</param>
        public Rod(
            ref RigidBody bodyOne, Vector3 positionOne,
            ref RigidBody bodyTwo, Vector3 positionTwo,
            float length)
            : base()
        {
            m_BodyOne = bodyOne;
            m_BodyTwo = bodyTwo;

            m_PositionOne = positionOne;
            m_PositionTwo = positionTwo;

            m_Length = length;
        }

        /// <summary>
        /// A�ade los contactos necesarios para mantener unidos mediante la barra a los cuerpos
        /// </summary>
        /// <param name="contactData">Datos de colisi�n</param>
        /// <param name="limit">L�mite de contactos a a�adir</param>
        /// <returns>Devuelve el n�mero de contacos a�adidos</returns>
        /// <remarks>S�lo a�ade un contacto o ninguno</remarks>
        public override int AddContact(ref CollisionData contactData, int limit)
        {
            if (contactData.HasFreeContacts())
            {
                // Encontrar la longitud actual
                Vector3 positionOneWorld = m_BodyOne.GetPointInWorldSpace(m_PositionOne);
                Vector3 positionTwoWorld = m_BodyTwo.GetPointInWorldSpace(m_PositionTwo);
                float currentLen = Vector3.Distance(positionOneWorld, positionTwoWorld);

                // Comprobar si estamos en extensi�n correcta
                if (currentLen == m_Length)
                {
                    return 0;
                }

                // Rellenar el contacto
                Contact contact = contactData.CurrentContact;

                contact.Bodies[0] = m_BodyOne;
                contact.Bodies[1] = m_BodyTwo;
                contact.ContactPoint = (positionOneWorld + positionTwoWorld) * 0.5f;

                // Calcular la normal
                Vector3 normal = Vector3.Normalize(m_BodyTwo.Position - m_BodyOne.Position);

                // La normal de contacto depende de si hay que extender o contraer para conservar la longitud
                if (currentLen > m_Length)
                {
                    contact.ContactNormal = normal;
                    contact.Penetration = currentLen - m_Length;
                }
                else
                {
                    contact.ContactNormal = Vector3.Negate(normal);
                    contact.Penetration = m_Length - currentLen;
                }

                // Siempre restituci�n 0
                contact.Restitution = 0f;

                contactData.AddContact();

                return 1;
            }

            return 0;
        }
    }
}
