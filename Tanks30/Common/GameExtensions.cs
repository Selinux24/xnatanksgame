using Microsoft.Xna.Framework;

namespace Common
{
    /// <summary>
    /// Extensiones
    /// </summary>
    public static class GameExtensions
    {
        /// <summary>
        /// Obtiene el servicio del tipo especificado
        /// </summary>
        /// <typeparam name="T">Tipo de Servicio</typeparam>
        /// <param name="serviceContainer">Contenedor de servicios</param>
        /// <returns>Devuelve el servicio del tipo especificado</returns>
        public static T GetService<T>(this GameServiceContainer serviceContainer)
        {
            return (T)serviceContainer.GetService(typeof(T));
        }
        /// <summary>
        /// Añade el servicio al contenedor de servicios
        /// </summary>
        /// <typeparam name="T">Tipo de Servicio</typeparam>
        /// <param name="serviceContainer">Contenedor de servicios</param>
        /// <param name="gameComponent">Componente</param>
        public static void AddService<T>(this GameServiceContainer serviceContainer, T gameComponent)
        {
            serviceContainer.AddService(typeof(T), gameComponent);
        }
    }
}
