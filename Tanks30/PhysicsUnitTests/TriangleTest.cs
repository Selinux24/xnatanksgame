using Common.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace PhysicsUnitTests
{
    [TestClass()]
    public class TriangleTest
    {
        [TestMethod()]
        public void ClosestPointInTrinagleTest()
        {
            Triangle tri;
            Vector3 point;
            Vector3 expectedPoint;

            // En origen
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, 0, 0));
            expectedPoint = new Vector3(0, 0, 0);
            Assert.AreEqual(expectedPoint, point, "En origen incorrecto");

            // En el plano
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, 0, 9));
            expectedPoint = new Vector3(0, 0, 9);
            Assert.AreEqual(expectedPoint, point, "Punto en el plano incorrecto");
            // Sobre el plano
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, 1, 9));
            expectedPoint = new Vector3(0, 0, 9);
            Assert.AreEqual(expectedPoint, point, "Punto sobre el plano incorrecto");
            // Bajo el plano
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, -1, 9));
            expectedPoint = new Vector3(0, 0, 9);
            Assert.AreEqual(expectedPoint, point, "Punto bajo el plano incorrecto");

            // En el plano punto 1
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, 0, 11));
            expectedPoint = new Vector3(0, 0, 10);
            Assert.AreEqual(expectedPoint, point, "Punto 1 en el plano incorrecto");
            // Sobre el plano punto 1
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, 1, 11));
            expectedPoint = new Vector3(0, 0, 10);
            Assert.AreEqual(expectedPoint, point, "Punto 1 sobre el plano incorrecto");
            // Bajo el plano punto 1
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, -1, 11));
            expectedPoint = new Vector3(0, 0, 10);
            Assert.AreEqual(expectedPoint, point, "Punto 1 bajo el plano incorrecto");

            // En el plano punto 2
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(-12, 0, -12));
            expectedPoint = new Vector3(-10, 0, -10);
            Assert.AreEqual(expectedPoint, point, "Punto 2 en el plano incorrecto");
            // Sobre el plano punto 2
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(-12, 1, -12));
            expectedPoint = new Vector3(-10, 0, -10);
            Assert.AreEqual(expectedPoint, point, "Punto 2 sobre el plano incorrecto");
            // Bajo el plano punto 2
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(-12, -1, -12));
            expectedPoint = new Vector3(-10, 0, -10);
            Assert.AreEqual(expectedPoint, point, "Punto 2 bajo el plano incorrecto");

            // En el plano punto 3
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(12, 0, -12));
            expectedPoint = new Vector3(10, 0, -10);
            Assert.AreEqual(expectedPoint, point, "Punto 3 en el plano incorrecto");
            // Sobre el plano punto 3
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(12, 1, -12));
            expectedPoint = new Vector3(10, 0, -10);
            Assert.AreEqual(expectedPoint, point, "Punto 3 sobre el plano incorrecto");
            // Bajo el plano punto 3
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(12, -1, -12));
            expectedPoint = new Vector3(10, 0, -10);
            Assert.AreEqual(expectedPoint, point, "Punto 3 bajo el plano incorrecto");

            // En el plano lado 1
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(-10, 0, 0));
            expectedPoint = new Vector3(-6, 0, -2);
            Assert.AreEqual(expectedPoint, point, "Lado 1 en el plano incorrecto");
            // Sobre el plano lado 1
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(-10, 1, 0));
            expectedPoint = new Vector3(-6, 0, -2);
            Assert.AreEqual(expectedPoint, point, "Lado 1 sobre el plano incorrecto");
            // Bajo el plano lado 1
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(-10, -1, 0));
            expectedPoint = new Vector3(-6, 0, -2);
            Assert.AreEqual(expectedPoint, point, "Lado 1 bajo el plano incorrecto");

            // En el plano lado 2
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(10, 0, 0));
            expectedPoint = new Vector3(6, 0, -2);
            Assert.AreEqual(expectedPoint, point, "Lado 2 en el plano incorrecto");
            // Sobre el plano lado 2
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(10, 1, 0));
            expectedPoint = new Vector3(6, 0, -2);
            Assert.AreEqual(expectedPoint, point, "Lado 2 sobre el plano incorrecto");
            // Bajo el plano lado 2
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(10, -1, 0));
            expectedPoint = new Vector3(6, 0, -2);
            Assert.AreEqual(expectedPoint, point, "Lado 2 bajo el plano incorrecto");

            // En el plano lado 3
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, 0, -12));
            expectedPoint = new Vector3(0, 0, -10);
            Assert.AreEqual(expectedPoint, point, "Lado 3 en el plano incorrecto");
            // Sobre el plano lado 3
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, 1, -12));
            expectedPoint = new Vector3(0, 0, -10);
            Assert.AreEqual(expectedPoint, point, "Lado 3 sobre el plano incorrecto");
            // Bajo el plano lado 3
            tri = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            point = Triangle.ClosestPointInTriangle(tri, new Vector3(0, -1, -12));
            expectedPoint = new Vector3(0, 0, -10);
            Assert.AreEqual(expectedPoint, point, "Lado 3 bajo el plano incorrecto");
        }
    }
}
