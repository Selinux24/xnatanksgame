
namespace TanksDebug
{
    static class Program
    {
        /// <summary>
        /// Entrada principal de la aplicación
        /// </summary>
        static void Main(string[] args)
        {
            using (BigBallisticDemo game = new BigBallisticDemo())
            {
                game.Run();
            }
        }
    }
}

