using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Enlace de uni�n entre dos cuerpos
    /// </summary>
    public class Joint2 : ContactGenerator
    {
        /// <summary>
        /// Primer cuerpo r�gido que forma parte de la uni�n
        /// </summary>
        private readonly IPhysicObject m_BodyOne;
        /// <summary>
        /// Segundo cuerpo r�gido que forma parte de la uni�n
        /// </summary>
        private readonly IPhysicObject m_BodyTwo;

        /// <summary>
        /// Posici�n relativa de la uni�n del primer cuerpo
        /// </summary>
        private readonly Vector3 m_RelativePointOne = Vector3.Zero;
        /// <summary>
        /// Posici�n relativa de la uni�n del primer cuerpo
        /// </summary>
        private readonly Vector3 m_RelativePointTwo = Vector3.Zero;

        /// <summary>
        /// Punto uno en coordenadas del mundo
        /// </summary>
        public Vector3 PointOneWorld
        {
            get
            {
                CollisionPrimitive objectOne = null;
                if (this.m_BodyOne != null)
                {
                    objectOne = this.m_BodyOne.GetPrimitive();
                    if (objectOne != null)
                    {
                        return objectOne.GetPointInWorldSpace(this.m_RelativePointOne);
                    }
                }

                return this.m_RelativePointOne;
            }
        }
        /// <summary>
        /// Punto dos en coordenadas del mundo
        /// </summary>
        public Vector3 PointTwoWorld
        {
            get
            {
                CollisionPrimitive objectTwo = null;
                if (this.m_BodyTwo != null)
                {
                    objectTwo = this.m_BodyTwo.GetPrimitive();
                    if (objectTwo != null)
                    {
                        return objectTwo.GetPointInWorldSpace(this.m_RelativePointTwo);
                    }
                }

                return this.m_RelativePointTwo;
            }
        }

        /// <summary>
        /// M�xima distancia de la uni�n antes de considerar que la uni�n haya sido violada y haya que actuar
        /// </summary>
        /// <remarks>
        /// Normalmente es un peque�o valor epsilon. Puede ser mayor, en cuyo caso se comporta como si una
        /// cuerda inel�stica juntara los cuerpos por sus posiciones de uni�n
        /// </remarks>
        private readonly float m_Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bodyOne">Cuerpo uno</param>
        /// <param name="relativePointOne">Posici�n de uni�n relativa al cuerpo uno</param>
        /// <param name="bodyTwo">Cuerpo dos</param>
        /// <param name="relativePointTwo">Posici�n de uni�n relativa al cuerpo dos</param>
        /// <param name="length">Longitud de la uni�n</param>
        public Joint2(
            IPhysicObject bodyOne, Vector3 relativePointOne,
            IPhysicObject bodyTwo, Vector3 relativePointTwo,
            float length)
            : base()
        {
            this.m_BodyOne = bodyOne;
            this.m_BodyTwo = bodyTwo;

            this.m_RelativePointOne = relativePointOne;
            this.m_RelativePointTwo = relativePointTwo;

            this.m_Length = length;
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
                CollisionPrimitive objectOne = null;
                if (this.m_BodyOne != null)
                {
                    objectOne = this.m_BodyOne.GetPrimitive();
                }

                CollisionPrimitive objectTwo = null;
                if (this.m_BodyTwo != null)
                {
                    objectTwo = this.m_BodyTwo.GetPrimitive();
                }

                Vector3 positionOne = this.m_RelativePointOne;
                Vector3 positionOneWorld = this.m_RelativePointOne;
                if (objectOne != null)
                {
                    positionOne = objectOne.Position;
                    positionOneWorld = objectOne.GetPointInWorldSpace(this.m_RelativePointOne);
                }

                Vector3 positionTwo = this.m_RelativePointTwo;
                Vector3 positionTwoWorld = this.m_RelativePointTwo;
                if (objectTwo != null)
                {
                    positionTwo = objectTwo.Position;
                    positionTwoWorld = objectTwo.GetPointInWorldSpace(this.m_RelativePointTwo);
                }

                float currentLen = Vector3.Distance(positionOneWorld, positionTwoWorld);

                if (Math.Abs(currentLen) > this.m_Length)
                {
                    Contact contact = contactData.CurrentContact;

                    contact.Bodies[0] = objectOne;
                    contact.Bodies[1] = objectTwo;
                    contact.ContactNormal = Vector3.Normalize(positionTwoWorld - positionOneWorld);
                    contact.ContactPoint = (positionOneWorld + positionTwoWorld) * 0.5f;
                    contact.Penetration = currentLen - this.m_Length;
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
