using System;

namespace BigBallisticDemo
{
    static class Program
    {
        /// <summary>
        /// Entrada principal de la aplicaci�n
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

