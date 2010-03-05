using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameComponents.Readers
{
    using Common.Primitives;

    public class PrimitiveListReader : ContentTypeReader<Triangle[]>
    {
        protected override Triangle[] Read(ContentReader input, Triangle[] existingInstance)
        {
            // Leer el n�mero de tri�ngulos
            int primitiveCount = input.ReadInt32();

            // Crear la lista de tri�ngulos
            Triangle[] triangles = new Triangle[primitiveCount];

            // Leer cada uno de los v�rtices de cada tri�ngulo
            for (int primitiveIndex = 0; primitiveIndex < primitiveCount; primitiveIndex++)
            {
                Vector3 vertex1 = input.ReadVector3();
                Vector3 vertex2 = input.ReadVector3();
                Vector3 vertex3 = input.ReadVector3();

                triangles[primitiveIndex] = new Triangle(vertex1, vertex2, vertex3);
            }

            return triangles;
        }
    }
}
