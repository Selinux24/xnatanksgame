using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Delegado del evento de contacto entre objectos físicos
    /// </summary>
    /// <param name="obj">Objeto que ha contactado con el actual</param>
    public delegate void ObjectInContactDelegate(IPhysicObject obj);
    /// <summary>
    /// Gestor de eventos de estado de objeto
    /// </summary>
    /// <param name="obj">Objeto</param>
    public delegate void ObjectStateHandler(IPhysicObject obj);

    /// <summary>
    /// Interfaz de objeto físico
    /// </summary>
    public interface IPhysicObject
    {
        /// <summary>
        /// Obtiene la posición del objeto
        /// </summary>
        /// <returns>Devuelve la posición del objeto</returns>
        Vector3 Position { get; }
        /// <summary>
        /// Obtiene el Quaternion del orientación del objeto
        /// </summary>
        /// <returns>Devuelve el Quaternion del orientación del objeto</returns>
        Quaternion Orientation { get; }

        /// <summary>
        /// Obtiene la primitiva de colisión sin comprobaciones
        /// </summary>
        /// <returns>Devuelve la primitiva de colisión</returns>
        CollisionPrimitive Primitive { get; }
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
        BoundingBox AABB { get; }
        /// <summary>
        /// Obtiene una esfera que circunda al objeto
        /// </summary>
        /// <returns>Devuelve una esfera</returns>
        BoundingSphere SPH { get; }
        /// <summary>
        /// Obtiene si el objeto está activo
        /// </summary>
        /// <returns>Devuelve verdadero si el objeto está activo</returns>
        bool IsActive { get; }

        /// <summary>
        /// Integrar el objeto en el tiempo
        /// </summary>
        /// <param name="time">Tiempo</param>
        void Integrate(float time);

        /// <summary>
        /// Ocurre cuando un objeto contacta con el actual
        /// </summary>
        event ObjectInContactDelegate Contacted;
        /// <summary>
        /// Ocurre cuando un objeto se activa
        /// </summary>
        event ObjectStateHandler Activated;
        /// <summary>
        /// Ocurre cuando un objeto se desactiva
        /// </summary>
        event ObjectStateHandler Deactivated;

        /// <summary>
        /// Establece que el objeto actual ha sido contactada por otro
        /// </summary>
        /// <param name="obj">Objeto que ha contactado con el actual</param>
        void SetContactedWith(IPhysicObject obj);
    }
}
