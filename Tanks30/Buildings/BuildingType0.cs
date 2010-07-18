using Microsoft.Xna.Framework;

namespace Buildings
{
    using GameComponents.Buildings;

    /// <summary>
    /// Un Edificio
    /// </summary>
    public partial class BuildingType0 : Building
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public BuildingType0(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Inicializa los componentes gráficos
        /// </summary>
        protected override void LoadContent()
        {
            this.ComponentsDirectory = "Content/Buildings/";
            this.ComponentInfoName = "WHBuilding01.xml";

            base.LoadContent();
        }
        /// <summary>
        /// Actualiza el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}