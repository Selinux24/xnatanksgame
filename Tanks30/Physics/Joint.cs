using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Enlace de uni�n entre dos cuerpos
    /// </summary>
    public class Joint : ContactGenerator
    {
        /// <summary>
        /// Primer cuerpo r�gido que forma parte de la uni�n
        /// </summary>
        private readonly RigidBody m_BodyOne;
        /// <summary>
        /// Segundo cuerpo r�gido que forma parte de la uni�n
        /// </summary>
        private readonly RigidBody m_BodyTwo;

        /// <summary>
        /// Posici�n relativa de la uni�n del primer cuerpo
        /// </summary>
        private readonly Vector3 m_PositionOne;
        /// <summary>
        /// Posici�n relativa de la uni�n del primer cuerpo
        /// </summary>
        private readonly Vector3 m_PositionTwo;

        /// <summary>
        /// M�xima distancia de la uni�n antes de considerar que la uni�n haya sido violada y haya que actuar
        /// </summary>
        /// <remarks>
        /// Normalmente es un peque�o valor epsilon. Puede ser mayor, en cuyo caso se comporta como si una
        /// cuerda inel�stica juntara los cuerpos por sus posiciones de uni�n
        /// </remarks>
        private readonly float m_Error;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="a">Cuerpo primero</param>
        /// <param name="a_pos">Posici�n de la uni�n en ccordenadas del cuerpo primero</param>
        /// <param name="b">Cuerpo segundo</param>
        /// <param name="b_pos">Posici�n de la uni�n en ccordenadas del cuerpo segundo</param>
        /// <param name="error">Distancia m�xima de la uni�n</param>
        public Joint(
            ref RigidBody a, Vector3 a_pos,
            ref RigidBody b, Vector3 b_pos,
            float error)
            : base()
        {
            this.m_BodyOne = a;
            this.m_BodyTwo = b;

            this.m_PositionOne = a_pos;
            this.m_PositionTwo = b_pos;

            this.m_Error = error;
        }

        /// <summary>
        /// Genera los contactos requeridos para restaurar la uni�n si ha sido violada
        /// </summary>
        /// <param name="contactData">Informaci�n de contactos</param>
        /// <param name="limit">L�mite de contactos a generar</param>
        /// <returns>Devuelve el n�mero de contactos generados</returns>
        /// <remarks>Tan solo generar� un contacto o ninguno</remarks>
        public override int AddContact(ref CollisionData contactData, int limit)
        {
            if (contactData.HasFreeContacts())
            {
                // Calcular las posiciones de los puntos de conexi�n en coordenadas del mundo
                Vector3 positionOneWorld = this.m_BodyOne.GetPointInWorldSpace(this.m_PositionOne);
                Vector3 positionTwoWorld = this.m_BodyTwo.GetPointInWorldSpace(this.m_PositionTwo);

                // Calcular la longitud de la uni�n
                float length = Vector3.Distance(positionTwoWorld, positionOneWorld);

                // Check if it is violated
                if (Math.Abs(length) > m_Error)
                {
                    Contact contact = contactData.CurrentContact;

                    contact.Bodies[0] = this.m_BodyOne;
                    contact.Bodies[1] = this.m_BodyTwo;
                    contact.ContactNormal = Vector3.Normalize(positionTwoWorld - positionOneWorld);
                    contact.ContactPoint = (positionOneWorld + positionTwoWorld) * 0.5f;
                    contact.Penetration = length - m_Error;
                    contact.Friction = 1.0f;
                    contact.Restitution = 0;

                    contactData.AddContact();

                    return 1;
                }
            }

            return 0;
        }
    }
}
