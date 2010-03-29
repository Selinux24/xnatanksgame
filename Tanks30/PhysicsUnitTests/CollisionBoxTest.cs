using Physics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace PhysicsUnitTests
{
    [TestClass()]
    public class CollisionBoxTest
    {
        [TestMethod()]
        public void ClosestPointInBoxTest()
        {
            CollisionBox box = new CollisionBox(new Vector3(10, 10, 10), 1);
            Vector3 point;
            Vector3 closestPoint;

            // En origen
            point = CollisionBox.ClosestPointInBox(box, new Vector3(0, 0, 0));
            closestPoint = new Vector3(0, 0, 0);
            Assert.AreEqual(closestPoint, point, "");

            // +++
            point = CollisionBox.ClosestPointInBox(box, new Vector3(10, 10, 10));
            closestPoint = new Vector3(10, 10, 10);
            Assert.AreEqual(closestPoint, point, "+++");
            // +++ Dentro
            point = CollisionBox.ClosestPointInBox(box, new Vector3(8, 8, 8));
            closestPoint = new Vector3(8, 8, 8);
            Assert.AreEqual(closestPoint, point, "+++ Dentro");
            // +++ Fuera
            point = CollisionBox.ClosestPointInBox(box, new Vector3(12, 12, 12));
            closestPoint = new Vector3(10, 10, 10);
            Assert.AreEqual(closestPoint, point, "+++ Fuera");

            // +-+
            point = CollisionBox.ClosestPointInBox(box, new Vector3(10, -10, 10));
            closestPoint = new Vector3(10, -10, 10);
            Assert.AreEqual(closestPoint, point, "+-+");
            // +-+ Dentro
            point = CollisionBox.ClosestPointInBox(box, new Vector3(8, -8, 8));
            closestPoint = new Vector3(8, -8, 8);
            Assert.AreEqual(closestPoint, point, "+-+ Dentro");
            // +-+ Fuera
            point = CollisionBox.ClosestPointInBox(box, new Vector3(12, -12, 12));
            closestPoint = new Vector3(10, -10, 10);
            Assert.AreEqual(closestPoint, point, "+-+ Fuera");

            // ++-
            point = CollisionBox.ClosestPointInBox(box, new Vector3(10, 10, -10));
            closestPoint = new Vector3(10, 10, -10);
            Assert.AreEqual(closestPoint, point, "++-");
            // ++- Dentro
            point = CollisionBox.ClosestPointInBox(box, new Vector3(8, 8, -8));
            closestPoint = new Vector3(8, 8, -8);
            Assert.AreEqual(closestPoint, point, "++- Dentro");
            // ++- Fuera
            point = CollisionBox.ClosestPointInBox(box, new Vector3(12, 12, -12));
            closestPoint = new Vector3(10, 10, -10);
            Assert.AreEqual(closestPoint, point, "++- Fuera");

            // -++
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-10, 10, 10));
            closestPoint = new Vector3(-10, 10, 10);
            Assert.AreEqual(closestPoint, point, "-++");
            // -++ Dentro
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-8, 8, 8));
            closestPoint = new Vector3(-8, 8, 8);
            Assert.AreEqual(closestPoint, point, "-++ Dentro");
            // -++ Fuera
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-12, 12, 12));
            closestPoint = new Vector3(-10, 10, 10);
            Assert.AreEqual(closestPoint, point, "-++ Fuera");

            // --+
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-10, -10, 10));
            closestPoint = new Vector3(-10, -10, 10);
            Assert.AreEqual(closestPoint, point, "--+");
            // --+ Dentro
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-8, -8, 8));
            closestPoint = new Vector3(-8, -8, 8);
            Assert.AreEqual(closestPoint, point, "--+ Dentro");
            // --+ Fuera
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-12, -12, 12));
            closestPoint = new Vector3(-10, -10, 10);
            Assert.AreEqual(closestPoint, point, "--+ Fuera");

            // +--
            point = CollisionBox.ClosestPointInBox(box, new Vector3(10, -10, -10));
            closestPoint = new Vector3(10, -10, -10);
            Assert.AreEqual(closestPoint, point, "+--");
            // +-- Dentro
            point = CollisionBox.ClosestPointInBox(box, new Vector3(8, -8, -8));
            closestPoint = new Vector3(8, -8, -8);
            Assert.AreEqual(closestPoint, point, "+-- Dentro");
            // +-- Fuera
            point = CollisionBox.ClosestPointInBox(box, new Vector3(12, -12, -12));
            closestPoint = new Vector3(10, -10, -10);
            Assert.AreEqual(closestPoint, point, "+-- Fuera");

            // -+-
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-10, 10, -10));
            closestPoint = new Vector3(-10, 10, -10);
            Assert.AreEqual(closestPoint, point, "-+-");
            // -+- Dentro
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-8, 8, -8));
            closestPoint = new Vector3(-8, 8, -8);
            Assert.AreEqual(closestPoint, point, "-+- Dentro");
            // -+- Fuera
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-12, 12, -12));
            closestPoint = new Vector3(-10, 10, -10);
            Assert.AreEqual(closestPoint, point, "-+- Fuera");

            // ---
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-10, -10, -10));
            closestPoint = new Vector3(-10, -10, -10);
            Assert.AreEqual(closestPoint, point, "---");
            // --- Dentro
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-8, -8, -8));
            closestPoint = new Vector3(-8, -8, -8);
            Assert.AreEqual(closestPoint, point, "--- Dentro");
            // --- Fuera
            point = CollisionBox.ClosestPointInBox(box, new Vector3(-12, -12, -12));
            closestPoint = new Vector3(-10, -10, -10);
            Assert.AreEqual(closestPoint, point, "--- Fuera");
        }
    }
}
