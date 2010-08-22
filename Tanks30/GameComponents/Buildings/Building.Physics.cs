using Microsoft.Xna.Framework;

namespace GameComponents.Buildings
{
    using Common.Helpers;
    using Physics;

    public partial class Building: IPhysicObject
    {
        /// <summary>
        /// Primitiva de colisión
        /// </summary>
        private CollisionBox m_OBB = null;
    
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
        /// Obtiene la posición
        /// </summary>
        /// <returns>Devuelve la posición</returns>
        public Vector3 GetPosition()
        {
            return this.Position;
        }
        /// <summary>
        /// Obtiene la orientación
        /// </summary>
        /// <returns>Devuelve la orientación</returns>
        public Quaternion GetOrientation()
        {
            return this.Orientation;
        }
        /// <summary>
        /// Obtiene la primitiva de colisión del vehículo
        /// </summary>
        /// <returns>Devuelve la primitiva de colisión del vehículo</returns>
        public virtual CollisionPrimitive GetPrimitive()
        {
            return this.m_OBB;
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
                BoundingSphere thisSPH = this.GetSPH();
                BoundingSphere otherSph = physicObject.GetSPH();
                if (thisSPH.Intersects(otherSph))
                {
                    return this.m_OBB;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene la caja alineada con los ejes que circunda el vehículo, en coordenadas del mundo
        /// </summary>
        /// <returns>Devuelve una caja alineada con los ejes</returns>
        public virtual BoundingBox GetAABB()
        {
            return this.m_OBB.AABB;
        }
        /// <summary>
        /// Obtiene la esfera que circunda el vehículo, en coordenadas del mundo
        /// </summary>
        /// <returns>Devuelve una esfera</returns>
        public virtual BoundingSphere GetSPH()
        {
            return this.m_OBB.SPH;
        }
        /// <summary>
        /// Obtiene si el vehículo está estático
        /// </summary>
        /// <returns>Devuelve verdadero si está estático</returns>
        public virtual bool IsActive()
        {
            return this.m_OBB.IsAwake;
        }
        /// <summary>
        /// Integra el cuerpo del vehículo en el tiempo
        /// </summary>
        /// <param name="time">Cantidad de tiempo</param>
        public void Integrate(float time)
        {
            if (this.m_OBB != null)
            {
                this.m_OBB.Integrate(time);
            }
        }
        /// <summary>
        /// Evento que se produce al ser contactado por otro objeto
        /// </summary>
        public event ObjectInContactDelegate OnObjectContacted;
        /// <summary>
        /// Cuando el vehículo es contactado por otro, se notifica el causante del contacto
        /// </summary>
        /// <param name="obj">Objeto que ha contactado con el vehículo actual</param>
        public void Contacted(IPhysicObject obj)
        {
            if (this.OnObjectContacted != null)
            {
                this.OnObjectContacted(obj);
            }
        }

        /// <summary>
        /// Establece la posición inicial
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="orientation">Orientación</param>
        public virtual void SetInitialState(Vector3 position, Quaternion orientation)
        {
            if (this.m_OBB != null)
            {
                this.m_OBB.SetInitialState(position, orientation);
            }
        }

    }
}
