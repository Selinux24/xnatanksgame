
namespace Physics
{
    /// <summary>
    /// Generador de fuerzas
    /// </summary>
    public abstract class ForceGenerator
    {
        /// <summary>
        /// Calcula la fuerza y la aplica al objeto especificado
        /// </summary>
        /// <param name="obj">Objeto</param>
        /// <param name="duration">Cantidad de tiempo</param>
        public abstract void UpdateForce(ref IPhysicObject obj, float duration);
    }
}
