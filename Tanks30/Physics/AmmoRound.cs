using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Una bala
    /// </summary>
    public class AmmoRound : CollisionSphere , IPhysicObject
    {
        /// <summary>
        /// Indica si el proyectil está activo
        /// </summary>
        private bool m_Active = false;
        /// <summary>
        /// Posición original del disparo
        /// </summary>
        private Vector3 m_OriginalPosition;
        /// <summary>
        /// Rango del disparo
        /// </summary>
        private float m_Range = 0f;
        /// <summary>
        /// Indica si la colisión del proyectil genera explosión
        /// </summary>
        private bool m_GenerateExplosion = false;

        /// <summary>
        /// Obtiene si la colisión del proyectil genera explosión
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
        /// <param name="position">Posición inicial</param>
        /// <param name="direction">Dirección</param>
        /// <param name="appliedGravity">Gravedad aplicada</param>
        /// <param name="radius">Radio del proyectil</param>
        /// <param name="generateExplosion">Indica si generará explosión</param>
        public void Fire(float mass, float range, Vector3 position, Vector3 direction, Vector3 appliedGravity, float radius, bool generateExplosion)
        {
            // Establecer la posición de origen
            this.m_OriginalPosition = position;

            // Establecer las propiedades del proyectil
            this.SetMass(mass);
            this.SetPosition(position);
            this.SetVelocity(direction);
            this.SetAcceleration(appliedGravity);

            this.Radius = radius;
            this.m_Range = range;
            this.m_GenerateExplosion = generateExplosion;
          
            // Rebote
            this.SetDamping(0.99f, 0.8f);

            // Establecer la inercia
            this.SetIntertia(0.4f * mass * radius * radius);
            
            // Este objeto no puede dormir
            this.CanSleep = false;

            // Activar
            this.m_Active = true;
        }
        /// <summary>
        /// Desactiva la bala
        /// </summary>
        public void Deactivate()
        {
            this.m_Active = false;
        }

        /// <summary>
        /// Obtiene la primitiva de colisión
        /// </summary>
        /// <returns>Devuelve la primitiva de colisión del objeto</returns>
        public CollisionPrimitive GetPrimitive()
        {
            return this;
        }
        /// <summary>
        /// Obtiene la primitiva de colisión que contacta con el objeto especificado
        /// </summary>
        /// <param name="physicObject">Objeto físico</param>
        /// <returns>Devuelve la primitiva de colisión que contacta con el objeto especificado</returns>
        public CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            if(physicObject != null)
            {
                BoundingSphere thisSph = this.GetSPH();
                BoundingSphere sph = physicObject.GetSPH();

                if (thisSph.Intersects(sph))
                {
                    return this;
                }                
            }

            return null;
        }
        /// <summary>
        /// Obtiene el AABB que contiene al objeto
        /// </summary>
        /// <returns>Devuelve el AABB que contiene al objeto</returns>
        public BoundingBox GetAABB()
        {
            return BoundingBox.CreateFromSphere(this.GetSPH());
        }
        /// <summary>
        /// Obtiene la esfera que contiene al objeto
        /// </summary>
        /// <returns>Devuelve la esfera que contiene al objeto</returns>
        public BoundingSphere GetSPH()
        {
            return new BoundingSphere(this.Position, this.Radius);
        }
        /// <summary>
        /// Obtiene si el objeto está activo
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            if (this.m_Active)
            {
                // Obtener la distancia recorrida
                float distance = Math.Abs(Vector3.Distance(this.m_OriginalPosition, this.Position));
                if (distance > this.m_Range)
                {
                    // Está fuera de rango
                    this.m_Active = false;
                }
            }

            return this.m_Active;
        }
        /// <summary>
        /// Evento que se produce cuando el objeto ha sido contactado
        /// </summary>
        public event ObjectInContactDelegate OnObjectContacted;
        /// <summary>
        /// Disparador del evento de contacto
        /// </summary>
        /// <param name="obj">Objeto contactado</param>
        public void Contacted(IPhysicObject obj)
        {
            this.m_Active = false;

            if(this.OnObjectContacted != null)
            {
                this.OnObjectContacted(obj);
            }
        }
    }
}