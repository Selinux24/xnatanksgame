
namespace Physics
{
    /// <summary>
    /// Interfaz de escenario
    /// </summary>
    public interface IScenery : IPhysicObject
    {
        /// <summary>
        /// Obtiene la altura (Y) en la coordenada especificada
        /// </summary>
        /// <param name="x">Coordenada X</param>
        /// <param name="z">Coordenada Z</param>
        /// <returns>Devuelve el valor de Y o null si las coordenadas no están sobre el terreno</returns>
        float? GetHeigthAtPoint(float x, float z);
    }
}
