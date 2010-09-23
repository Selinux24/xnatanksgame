using Microsoft.Xna.Framework;

namespace Common
{
    /// <summary>
    /// Objeto
    /// </summary>
    public interface IObject
    {
        /// <summary>
        /// Obtiene la posición del objeto
        /// </summary>
        Vector3 Position { get; }
    }
}
