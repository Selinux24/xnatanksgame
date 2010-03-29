using Common.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Physics;

namespace PhysicsUnitTests
{
    [TestClass()]
    public class IntersectionTestsTest
    {
        [TestMethod()]
        public void SphereAndTriTest()
        {
            bool intersect = false;

            CollisionSphere sphere = new CollisionSphere(1f, 1f);

            Triangle triangleHAU = new Triangle(new Vector3(0, 1, 10), new Vector3(-10, 1, -10), new Vector3(10, 1, -10));
            Triangle triangleHBU = new Triangle(new Vector3(0, 0, 10), new Vector3(-10, 0, -10), new Vector3(10, 0, -10));
            Triangle triangleHCU = new Triangle(new Vector3(0, -1, 10), new Vector3(-10, -1, -10), new Vector3(10, -1, -10));

            Triangle triangleHAD = new Triangle(new Vector3(0, 1, 10), new Vector3(10, 1, -10), new Vector3(-10, 1, -10));
            Triangle triangleHBD = new Triangle(new Vector3(0, 0, 10), new Vector3(10, 0, -10), new Vector3(-10, 0, -10));
            Triangle triangleHCD = new Triangle(new Vector3(0, -1, 10), new Vector3(10, -1, -10), new Vector3(-10, -1, -10));


            sphere.SetPosition(new Vector3(0, 2, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAU);
            Assert.AreEqual(true, intersect, "Error en Triángulo sobre 0 normal UP, contacto perfecto sobre plano");
            sphere.SetPosition(new Vector3(0, 0, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAU);
            Assert.AreEqual(true, intersect, "Error en Triángulo sobre 0 normal UP, contacto perfecto bajo plano");
            sphere.SetPosition(new Vector3(0, 1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAU);
            Assert.AreEqual(true, intersect, "Error en Triángulo sobre 0 normal UP, contacto en mitad de la esfera");
            sphere.SetPosition(new Vector3(0, 3, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAU);
            Assert.AreEqual(false, intersect, "Error en Triángulo sobre 0 normal UP, sin contacto sobre el plano");
            sphere.SetPosition(new Vector3(0, -1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAU);
            Assert.AreEqual(false, intersect, "Error en Triángulo sobre 0 normal UP, sin contacto bajo el plano");

            sphere.SetPosition(new Vector3(0, 2, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAD);
            Assert.AreEqual(true, intersect, "Error en Triángulo sobre 0 normal DOWN, contacto perfecto sobre plano");
            sphere.SetPosition(new Vector3(0, 0, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAD);
            Assert.AreEqual(true, intersect, "Error en Triángulo sobre 0 normal DOWN, contacto perfecto bajo plano");
            sphere.SetPosition(new Vector3(0, 1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAD);
            Assert.AreEqual(true, intersect, "Error en Triángulo sobre 0 normal DOWN, contacto en mitad de la esfera");
            sphere.SetPosition(new Vector3(0, 3, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAD);
            Assert.AreEqual(false, intersect, "Error en Triángulo sobre 0 normal DOWN, sin contacto sobre el plano");
            sphere.SetPosition(new Vector3(0, -1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHAD);
            Assert.AreEqual(false, intersect, "Error en Triángulo sobre 0 normal DOWN, sin contacto bajo el plano");



            sphere.SetPosition(new Vector3(0, 1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBU);
            Assert.AreEqual(true, intersect, "Error en Triángulo en 0 normal UP, contacto perfecto sobre plano");
            sphere.SetPosition(new Vector3(0, -1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBU);
            Assert.AreEqual(true, intersect, "Error en Triángulo en 0 normal UP, contacto perfecto bajo plano");
            sphere.SetPosition(new Vector3(0, 0, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBU);
            Assert.AreEqual(true, intersect, "Error en Triángulo en 0 normal UP, contacto en mitad de la esfera");
            sphere.SetPosition(new Vector3(0, 2, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBU);
            Assert.AreEqual(false, intersect, "Error en Triángulo en 0 normal UP, sin contacto sobre el plano");
            sphere.SetPosition(new Vector3(0, -2, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBU);
            Assert.AreEqual(false, intersect, "Error en Triángulo en 0 normal UP, sin contacto bajo el plano");

            sphere.SetPosition(new Vector3(0, 1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBD);
            Assert.AreEqual(true, intersect, "Error en Triángulo en 0 normal DOWN, contacto perfecto sobre plano");
            sphere.SetPosition(new Vector3(0, -1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBD);
            Assert.AreEqual(true, intersect, "Error en Triángulo en 0 normal DOWN, contacto perfecto bajo plano");
            sphere.SetPosition(new Vector3(0, 0, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBD);
            Assert.AreEqual(true, intersect, "Error en Triángulo en 0 normal DOWN, contacto en mitad de la esfera");
            sphere.SetPosition(new Vector3(0, 2, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBD);
            Assert.AreEqual(false, intersect, "Error en Triángulo en 0 normal DOWN, sin contacto sobre el plano");
            sphere.SetPosition(new Vector3(0, -2, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBD);
            Assert.AreEqual(false, intersect, "Error en Triángulo en 0 normal DOWN, sin contacto bajo el plano");



            sphere.SetPosition(new Vector3(0, 0, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCU);
            Assert.AreEqual(true, intersect, "Error en Triángulo en -1 normal UP, contacto perfecto sobre plano");
            sphere.SetPosition(new Vector3(0, -2, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCU);
            Assert.AreEqual(true, intersect, "Error en Triángulo en -1 normal UP, contacto perfecto bajo plano");
            sphere.SetPosition(new Vector3(0, -1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCU);
            Assert.AreEqual(true, intersect, "Error en Triángulo en -1 normal UP, contacto en mitad de la esfera");
            sphere.SetPosition(new Vector3(0, 1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCU);
            Assert.AreEqual(false, intersect, "Error en Triángulo en -1 normal UP, sin contacto sobre el plano");
            sphere.SetPosition(new Vector3(0, -3, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCU);
            Assert.AreEqual(false, intersect, "Error en Triángulo en -1 normal UP, sin contacto bajo el plano");

            sphere.SetPosition(new Vector3(0, 0, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCD);
            Assert.AreEqual(true, intersect, "Error en Triángulo en -1 normal DOWN, contacto perfecto sobre plano");
            sphere.SetPosition(new Vector3(0, -2, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCD);
            Assert.AreEqual(true, intersect, "Error en Triángulo en -1 normal DOWN, contacto perfecto bajo plano");
            sphere.SetPosition(new Vector3(0, -1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCD);
            Assert.AreEqual(true, intersect, "Error en Triángulo en -1 normal DOWN, contacto en mitad de la esfera");
            sphere.SetPosition(new Vector3(0, 1, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCD);
            Assert.AreEqual(false, intersect, "Error en Triángulo en -1 normal DOWN, sin contacto sobre el plano");
            sphere.SetPosition(new Vector3(0, -3, 0));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHCD);
            Assert.AreEqual(false, intersect, "Error en Triángulo en -1 normal DOWN, sin contacto bajo el plano");


            sphere.SetPosition(new Vector3(-9, 0, 9));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBU);
            Assert.AreEqual(false, intersect, "Error en Triángulo en -1 normal DOWN, contacto perfecto sobre plano");
            sphere.SetPosition(new Vector3(9, 0, 9));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBU);
            Assert.AreEqual(false, intersect, "Error en Triángulo en -1 normal DOWN, contacto perfecto sobre plano");
            sphere.SetPosition(new Vector3(0, 0, -12));
            intersect = IntersectionTests.SphereAndTri(sphere, triangleHBU);
            Assert.AreEqual(false, intersect, "Error en Triángulo en -1 normal DOWN, contacto perfecto sobre plano");
        }
    }
}
