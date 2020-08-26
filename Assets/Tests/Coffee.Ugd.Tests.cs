using NUnit.Framework;
using UnityEngine;

namespace Coffee.Ugd
{
    public class MathTests
    {
        [Test]
        public void GetX()
        {
            var v = new Vector3(1, 20, 300);
            Assert.AreEqual(Math.GetX(v), 1);
        }

        [Test]
        public void GetY()
        {
            var v = new Vector3(1, 20, 300);
            Assert.AreEqual(Math.GetY(v), 20);
        }

        [Test]
        public void GetZ()
        {
            var v = new Vector3(1, 20, 300);
            Assert.AreEqual(Math.GetZ(v), 300);
        }
    }
}
