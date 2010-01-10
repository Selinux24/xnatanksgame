
namespace Physics
{
    /// <summary>
    /// Clase base de generador de contactos entre cuerpos
    /// </summary>
    public abstract class ContactGenerator
    {
        /// <summary>
        /// Genera los contactos entre los cuerpos incluídos en el generador de contactos
        /// </summary>
        /// <param name="contactData">Información de contactos</param>
        /// <param name="limit">Límite de contactos a generar</param>
        /// <returns>Devuelve el número de contactos generados</returns>
        public abstract int AddContact(ref CollisionData contactData, int limit);
    }
}
