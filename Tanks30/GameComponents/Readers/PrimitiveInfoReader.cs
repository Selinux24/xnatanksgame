using Microsoft.Xna.Framework.Content;

namespace GameComponents.Readers
{
    using Common.Primitives;

    public class PrimitiveInfoReader : ContentTypeReader<PrimitiveInfo>
    {
        protected override PrimitiveInfo Read(ContentReader input, PrimitiveInfo existingInstance)
        {
            // Instanciar la clase
            PrimitiveInfo primitiveInfo;
            if (existingInstance != null)
            {
                primitiveInfo = existingInstance;
            }
            else
            {
                primitiveInfo = new PrimitiveInfo();
            }

            // Leer el número de índices
            int indexes = input.ReadInt32();

            for (int index = 0; index < indexes; index++)
            {
                // Leer el índice actual
                string currentIndex = input.ReadString();

                // Instanciar la lista de primitivas
                Triangle[] primitiveList = input.ReadObject<Triangle[]>();

                // Añadir las primitivas
                primitiveInfo.AddTriangles(currentIndex, primitiveList);
            }

            primitiveInfo.Update();

            return primitiveInfo;
        }
    }
}
