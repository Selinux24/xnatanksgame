﻿using Microsoft.Xna.Framework;

namespace GameComponents.Buildings
{
    using Physics;

    public partial class Building: IPhysicObject
    {
        /// <summary>
        /// Primitiva de colisión
        /// </summary>
        private CollisionPrimitive m_CollisionPrimitive = null;
    
        /// <summary>
        /// Indica si el vehículo está detenido
        /// </summary>
        public virtual bool IsStatic
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Obtiene la primitiva de colisión del vehículo
        /// </summary>
        /// <returns>Devuelve la primitiva de colisión del vehículo</returns>
        public virtual CollisionPrimitive Primitive
        {
            get
            {
                return this.m_CollisionPrimitive;
            }
        }
        /// <summary>
        /// Obtiene la primitiva de colisión del vehículo
        /// </summary>
        /// <param name="physicObject">Objeto físico</param>
        /// <returns>Devuelve la primitiva de colisión del vehículo</returns>
        public virtual CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            if (physicObject != null)
            {
                // Obtener las esferas circundantes y detectar colisión potencial
                BoundingSphere thisSPH = this.SPH;
                BoundingSphere otherSph = physicObject.SPH;
                if (thisSPH.Intersects(otherSph))
                {
                    return this.m_CollisionPrimitive;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene la caja alineada con los ejes que circunda el vehículo, en coordenadas del mundo
        /// </summary>
        /// <returns>Devuelve una caja alineada con los ejes</returns>
        public virtual BoundingBox AABB
        {
            get
            {
            return this.m_CollisionPrimitive.AABB;
            }
        }
        /// <summary>
        /// Obtiene la esfera que circunda el vehículo, en coordenadas del mundo
        /// </summary>
        /// <returns>Devuelve una esfera</returns>
        public virtual BoundingSphere SPH
        {
            get
            {
                return this.m_CollisionPrimitive.SPH;
            }
        }
        /// <summary>
        /// Obtiene si el vehículo está estático
        /// </summary>
        /// <returns>Devuelve verdadero si está estático</returns>
        public virtual bool IsActive
        {
            get
            {
                return this.m_CollisionPrimitive.IsAwake;
            }
        }
        /// <summary>
        /// Integra el cuerpo del vehículo en el tiempo
        /// </summary>
        /// <param name="time">Cantidad de tiempo</param>
        public void Integrate(float time)
        {
            if (this.m_CollisionPrimitive != null)
            {
                this.m_CollisionPrimitive.Integrate(time);
            }
        }

        /// <summary>
        /// Evento que se produce al ser contactado por otro objeto
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
        /// Cuando el vehículo es contactado por otro, se notifica el causante del contacto
        /// </summary>
        /// <param name="obj">Objeto que ha contactado con el vehículo actual</param>
        public void SetContactedWith(IPhysicObject obj)
        {
            if (this.Contacted != null)
            {
                this.Contacted(obj);
            }
        }
        /// <summary>
        /// Disparador del evento de activación
        /// </summary>
        private void FireActivated()
        {
            if (this.Activated != null)
            {
                this.Activated(this);
            }
        }
        /// <summary>
        /// Disparador del evento de desactivación
        /// </summary>
        private void FireDeactivated()
        {
            if (this.Deactivated != null)
            {
                this.Deactivated(this);
            }
        }

        /// <summary>
        /// Establece la posición inicial
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="orientation">Orientación</param>
        public virtual void SetInitialState(Vector3 position, Quaternion orientation)
        {
            if (this.m_CollisionPrimitive != null)
            {
                this.m_CollisionPrimitive.SetInitialState(position, orientation);
            }
        }

    }
}
