using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Delegado del evento de contacto entre objectos físicos
    /// </summary>
    /// <param name="obj">Objeto que ha contactado con el actual</param>
    public delegate void ObjectInContactDelegate(IPhysicObject obj);

    /// <summary>
    /// Interfaz de objeto físico
    /// </summary>
    public interface IPhysicObject
    {
        /// <summary>
        /// Obtiene la primitiva de colisión sin comprobaciones
        /// </summary>
        /// <returns>Devuelve la primitiva de colisión</returns>
        CollisionPrimitive GetPrimitive();
        /// <summary>
        /// Obtiene la primitiva del objeto que contacta al objeto físico especificado
        /// </summary>
        /// <param name="physicObject">Objeto cercano</param>
        /// <returns>Devuelve la primitiva del objeto</returns>
        CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject);
        /// <summary>
        /// Obtiene la caja alineada con los ejes que circunda al objeto
        /// </summary>
        /// <returns>Devuelve una caja alineada con los ejes</returns>
        BoundingBox GetAABB();
        /// <summary>
        /// Obtiene una esfera que circunda al objeto
        /// </summary>
        /// <returns>Devuelve una esfera</returns>
        BoundingSphere GetSPH();
        /// <summary>
        /// Obtiene si el objeto está activo
        /// </summary>
        /// <returns>Devuelve verdadero si el objeto está activo</returns>
        bool IsActive();

        /// <summary>
        /// Integrar el objeto en el tiempo
        /// </summary>
        /// <param name="time">Tiempo</param>
        void Integrate(float time);

        /// <summary>
        /// Ocurre cuando un objeto contacta con el actual
        /// </summary>
        event ObjectInContactDelegate OnObjectContacted;
        /// <summary>
        /// Establece que el objeto actual ha sido contactada por otro
        /// </summary>
        /// <param name="obj">Objeto que ha contactado con el actual</param>
        void Contacted(IPhysicObject obj);
    }
}
