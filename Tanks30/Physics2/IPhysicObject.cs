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
        /// Obtiene la primitiva del objeto
        /// </summary>
        /// <returns>Devuelve la primitiva del objeto</returns>
        CollisionPrimitive GetPrimitive();
        /// <summary>
        /// Obtiene la esfera que circunda el objeto
        /// </summary>
        /// <returns>Devuelve la esfera que circunda el objeto</returns>
        BoundingSphere GetBoundingSphere();

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
