
namespace Physics
{
    /// <summary>
    /// Clase base de generador de contactos entre cuerpos
    /// </summary>
    public abstract class ContactGenerator
    {
        /// <summary>
        /// Genera los contactos entre los cuerpos inclu�dos en el generador de contactos
        /// </summary>
        /// <param name="contactData">Informaci�n de contactos</param>
        /// <param name="limit">L�mite de contactos a generar</param>
        /// <returns>Devuelve el n�mero de contactos generados</returns>
        public abstract int AddContact(ref CollisionData contactData, int limit);
    }
}
