using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Una bala
    /// </summary>
    public class AmmoRound : CollisionSphere, IPhysicObject
    {
        /// <summary>
        /// Indica si el proyectil est� activo
        /// </summary>
        private bool m_Active = false;
        /// <summary>
        /// Posici�n original del disparo
        /// </summary>
        private Vector3 m_OriginalPosition;
        /// <summary>
        /// Rango del disparo
        /// </summary>
        private float m_Range = 0f;
        /// <summary>
        /// Da�o
        /// </summary>
        private float m_Damage = 0f;
        /// <summary>
        /// Penetraci�n del blindaje
        /// </summary>
        private float m_Penetration = 0f;
        /// <summary>
        /// Indica si la colisi�n del proyectil genera explosi�n
        /// </summary>
        private bool m_GenerateExplosion = false;

        /// <summary>
        /// Da�o
        /// </summary>
        public float Damage
        {
            get
            {
                return this.m_Damage;
            }
        }
        /// <summary>
        /// Penetraci�n
        /// </summary>
        public float Penetration
        {
            get
            {
                return this.m_Penetration;
            }
        }
        /// <summary>
        /// Obtiene si la colisi�n del proyectil genera explosi�n
        /// </summary>
        public bool GenerateExplosion
        {
            get
            {
                return this.m_GenerateExplosion;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="radius">Radio</param>
        /// <param name="mass">Masa</param>
        public AmmoRound()
            : base(1f, 1f)
        {

        }

        /// <summary>
        /// Carga el disparo
        /// </summary>
        /// <param name="mass">Masa del proyectil</param>
        /// <param name="range">Rango</param>
        /// <param name="damage">Da�o</param>
        /// <param name="penetration">Penetraci�n de blindaje</param>
        /// <param name="position">Posici�n inicial</param>
        /// <param name="direction">Direcci�n</param>
        /// <param name="appliedGravity">Gravedad aplicada</param>
        /// <param name="radius">Radio del proyectil</param>
        /// <param name="generateExplosion">Indica si generar� explosi�n</param>
        public void Fire(float mass, float range, float damage, float penetration, Vector3 position, Vector3 direction, Vector3 appliedGravity, float radius, bool generateExplosion)
        {
            // Establecer la posici�n de origen
            this.m_OriginalPosition = position;

            // Establecer las propiedades del proyectil
            this.SetMass(mass);
            this.SetPosition(position);
            this.SetVelocity(direction);
            this.SetAcceleration(appliedGravity);

            this.Radius = radius;
            this.m_Range = range;
            this.m_Damage = damage;
            this.m_Penetration = penetration;
            this.m_GenerateExplosion = generateExplosion;

            // Rebote
            this.SetDamping(0.99f, 0.8f);

            // Establecer la inercia
            this.SetIntertia(0.4f * mass * radius * radius);

            // Este objeto no puede dormir
            this.CanSleep = false;

            // Activar
            this.m_Active = true;

            this.OnActivated();
        }
        /// <summary>
        /// Desactiva la bala
        /// </summary>
        public void Deactivate()
        {
            this.m_Active = false;

            this.OnDeactivated();
        }

        /// <summary>
        /// Obtiene la primitiva de colisi�n
        /// </summary>
        public CollisionPrimitive Primitive
        {
            get
            {
                return this;
            }
        }
        /// <summary>
        /// Obtiene la primitiva de colisi�n que contacta con el objeto especificado
        /// </summary>
        /// <param name="physicObject">Objeto f�sico</param>
        /// <returns>Devuelve la primitiva de colisi�n que contacta con el objeto especificado</returns>
        public CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            if (physicObject != null)
            {
                BoundingSphere thisSph = this.SPH;
                BoundingSphere sph = physicObject.SPH;

                if (thisSph.Intersects(sph))
                {
                    return this;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene si el objeto est� activo
        /// </summary>
        /// <returns></returns>
        public bool IsActive
        {
            get
            {
                if (this.m_Active)
                {
                    // Obtener la distancia recorrida
                    float distance = Math.Abs(Vector3.Distance(this.m_OriginalPosition, this.Position));
                    if (distance > this.m_Range)
                    {
                        // Est� fuera de rango
                        this.m_Active = false;

                        this.OnDeactivated();
                    }
                }

                return this.m_Active;
            }
        }

        /// <summary>
        /// Evento que se produce cuando el objeto ha sido contactado
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
        /// Disparador del evento de contacto
        /// </summary>
        /// <param name="obj">Objeto contactado</param>
        public void SetContactedWith(IPhysicObject obj)
        {
            this.m_Active = false;

            this.OnDeactivated();

            if (this.Contacted != null)
            {
                this.Contacted(obj);
            }
        }
        /// <summary>
        /// Disparador del evento de activaci�n
        /// </summary>
        public void OnActivated()
        {
            if (this.Activated != null)
            {
                this.Activated(this);
            }
        }
        /// <summary>
        /// Disparador del evento de desactivaci�n
        /// </summary>
        public void OnDeactivated()
        {
            if (this.Deactivated != null)
            {
                this.Deactivated(this);
            }
        }
    }
}