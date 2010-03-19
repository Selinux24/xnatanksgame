
namespace Common.Primitives
{
    /// <summary>
    /// Información de índices de nodo
    /// </summary>
    public class SceneryNodeIndexInfo
    {
        /// <summary>
        /// Cantidad de primitivas para el sector central
        /// </summary>
        public int CenterPrimitiveCount { get; set; }
        /// <summary>
        /// Cantidad de primitivas para cada borde
        /// </summary>
        public int BorderPrimitiveCount { get; set; }
        /// <summary>
        /// Cantidad de primitivas para cada borde de conexión
        /// </summary>
        public int BorderConnectionPrimitiveCount { get; set; }

        /// <summary>
        /// Indice inicial del sector central
        /// </summary>
        public int CenterOffset { get; set; }
        /// <summary>
        /// Indice inicial del borde norte
        /// </summary>
        public int NorthOffset { get; set; }
        /// <summary>
        /// Indice inicial del borde sur
        /// </summary>
        public int SouthOffset { get; set; }
        /// <summary>
        /// Indice inicial del borde este
        /// </summary>
        public int EastOffset { get; set; }
        /// <summary>
        /// Indice inicial del borde oeste
        /// </summary>
        public int WestOffset { get; set; }
        /// <summary>
        /// Indice inicial del borde de conexión norte
        /// </summary>
        public int NorthConnectionOffset { get; set; }
        /// <summary>
        /// Indice inicial del borde de conexión sur
        /// </summary>
        public int SouthConnectionOffset { get; set; }
        /// <summary>
        /// Indice inicial del borde de conexión este
        /// </summary>
        public int EastConnectionOffset { get; set; }
        /// <summary>
        /// Indice inicial del borde de conexión oeste
        /// </summary>
        public int WestConnectionOffset { get; set; }
    }
}
