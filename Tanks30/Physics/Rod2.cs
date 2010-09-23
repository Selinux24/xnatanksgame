using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Barra de uni�n
    /// </summary>
    public class Rod2 : ContactGenerator
    {
        /// <summary>
        /// Cuerpo uno
        /// </summary>
        private IPhysicObject m_BodyOne = null;
        /// <summary>
        /// Cuerpo dos
        /// </summary>
        private IPhysicObject m_BodyTwo = null;

        /// <summary>
        /// Posici�n de uni�n relativa al cuerpo uno
        /// </summary>
        private Vector3 m_RelativePointOne = Vector3.Zero;
        /// <summary>
        /// Posici�n de uni�n relativa al cuerpo dos
        /// </summary>
        private Vector3 m_RelativePointTwo = Vector3.Zero;
     
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
                    objectOne = this.m_BodyOne.Primitive;
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
                    objectTwo = this.m_BodyTwo.Primitive;
                    if (objectTwo != null)
                    {
                        return objectTwo.GetPointInWorldSpace(this.m_RelativePointTwo);
                    }
                }

                return this.m_RelativePointTwo;
            }
        }

        /// <summary>
        /// Longitud de la barra
        /// </summary>
        private float m_Length = 0f;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bodyOne">Cuerpo uno</param>
        /// <param name="relativePointOne">Posici�n de uni�n relativa al cuerpo uno</param>
        /// <param name="bodyTwo">Cuerpo dos</param>
        /// <param name="relativePointTwo">Posici�n de uni�n relativa al cuerpo dos</param>
        /// <param name="length">Longitud de la barra</param>
        public Rod2(
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
                CollisionPrimitive objectOne = null;
                if (this.m_BodyOne != null)
                {
                    objectOne = this.m_BodyOne.Primitive;
                }

                CollisionPrimitive objectTwo = null;
                if (this.m_BodyTwo != null)
                {
                    objectTwo = this.m_BodyTwo.Primitive;
                }

                // Encontrar la longitud actual
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

                // Comprobar si estamos en extensi�n correcta
                if (currentLen != m_Length)
                {
                    // Rellenar el contacto
                    Contact contact = contactData.CurrentContact;

                    contact.Bodies[0] = objectOne;
                    contact.Bodies[1] = objectTwo;
                    contact.ContactPoint = (positionOneWorld + positionTwoWorld) * 0.5f;

                    // Calcular la normal
                    Vector3 normal = Vector3.Normalize(positionTwo - positionOne);

                    // La normal de contacto depende de si hay que extender o contraer para conservar la longitud
                    if (currentLen > m_Length)
                    {
                        contact.ContactNormal = normal;
                        contact.Penetration = currentLen - m_Length;
                    }
                    else
                    {
                        contact.ContactNormal = Vector3.Negate(normal);
                        contact.Penetration = this.m_Length - currentLen;
                    }

                    // Siempre restituci�n 0
                    contact.Restitution = 0f;

                    contactData.AddContact();

                    return 1;
                }
            }

            return 0;
        }
    }
}
