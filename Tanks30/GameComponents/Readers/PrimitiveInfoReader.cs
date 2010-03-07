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

            // Leer el n�mero de �ndices
            int indexes = input.ReadInt32();

            for (int index = 0; index < indexes; index++)
            {
                // Leer el �ndice actual
                string currentIndex = input.ReadString();

                // Instanciar la lista de primitivas
                Triangle[] primitiveList = input.ReadObject<Triangle[]>();

                // A�adir las primitivas
                primitiveInfo.AddTriangles(currentIndex, primitiveList);
            }

            primitiveInfo.Update();

            return primitiveInfo;
        }
    }
}
