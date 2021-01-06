using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coffee.GitDependencyResolver
{
    public class PackageMetaTests
    {
        [Test]
        public void GetPackagePath_Nothing()
        {
            var p = PackageMeta.FromNameAndUrl("com.test.package", "https://github.com/mob-sakai/com.test.package.git");
            Assert.AreEqual(p.path, "");
            Assert.AreEqual(p.GetPackagePath("temp"), "temp");
        }

        [Test]
        public void GetPackagePath_Exists()
        {
            var p = PackageMeta.FromNameAndUrl("com.test.package", "https://github.com/mob-sakai/com.test.package.git?path=Packages/com.test.package");
            Assert.AreEqual(p.path, "Packages/com.test.package");
            Assert.AreEqual(p.GetPackagePath("temp"), "temp/Packages/com.test.package");
        }

        [Test]
        public void FromNameAndUrl_Dependancies()
        {
            var p = PackageMeta.FromNameAndUrl("com.test.package", "https://github.com/mob-sakai/com.test.package.git");

            Assert.That(p.dependencies, Is.EqualTo(new PackageMeta[0]));
            Assert.That(p.gitDependencies, Is.EqualTo(new PackageMeta[0]));
        }

        [Test]
        public void FromNameAndUrl_Https()
        {
            var p = PackageMeta.FromNameAndUrl("com.test.package", "https://github.com/mob-sakai/com.test.package.git");

            Assert.AreEqual(p.name, "com.test.package");
            Assert.AreEqual(p.version, new SemVersion(0));
            Assert.AreEqual(p.repository, "https://github.com/mob-sakai/com.test.package.git");
            Assert.AreEqual(p.path, "");
            Assert.AreEqual(p.revision, "");
        }

        [Test]
        public void FromNameAndUrl_Https_Path()
        {
            var p = PackageMeta.FromNameAndUrl("com.test.package", "https://github.com/mob-sakai/com.test.package.git?path=Packages/com.test.package");

            Assert.AreEqual(p.name, "com.test.package");
            Assert.AreEqual(p.version, new SemVersion(0));
            Assert.AreEqual(p.repository, "https://github.com/mob-sakai/com.test.package.git");
            Assert.AreEqual(p.path, "Packages/com.test.package");
            Assert.AreEqual(p.revision, "");
        }

        [Test]
        public void FromNameAndUrl_Https_Path_Rev()
        {
            var p = PackageMeta.FromNameAndUrl("com.test.package", "https://github.com/mob-sakai/com.test.package.git?path=Packages/com.test.package#upm");

            Assert.AreEqual(p.name, "com.test.package");
            Assert.AreEqual(p.version, new SemVersion(0));
            Assert.AreEqual(p.repository, "https://github.com/mob-sakai/com.test.package.git");
            Assert.AreEqual(p.path, "Packages/com.test.package");
            Assert.AreEqual(p.revision, "upm");
        }

        [Test]
        public void FromNameAndUrl_Https_Path_RevVersion()
        {
            var p = PackageMeta.FromNameAndUrl("com.test.package", "https://github.com/mob-sakai/com.test.package.git?path=Packages/com.test.package#1.2.3-preview.4");

            Assert.AreEqual(p.name, "com.test.package");
            Assert.AreEqual(p.version, SemVersion.Parse("1.2.3-preview.4"));
            Assert.AreEqual(p.repository, "https://github.com/mob-sakai/com.test.package.git");
            Assert.AreEqual(p.path, "Packages/com.test.package");
            Assert.AreEqual(p.revision, "1.2.3-preview.4");
        }


        [Test]
        public void FromNameAndUrl_Https_Rev()
        {
            var p = PackageMeta.FromNameAndUrl("com.test.package", "https://github.com/mob-sakai/com.test.package.git#upm");

            Assert.AreEqual(p.name, "com.test.package");
            Assert.AreEqual(p.version, new SemVersion(0));
            Assert.AreEqual(p.repository, "https://github.com/mob-sakai/com.test.package.git");
            Assert.AreEqual(p.path, "");
            Assert.AreEqual(p.revision, "upm");
        }

        [Test]
        public void FromNameAndUrl_Https_RevVersion()
        {
            var p = PackageMeta.FromNameAndUrl("com.test.package", "https://github.com/mob-sakai/com.test.package.git#1.2.3-preview.4");

            Assert.AreEqual(p.name, "com.test.package");
            Assert.AreEqual(p.version, SemVersion.Parse("1.2.3-preview.4"));
            Assert.AreEqual(p.repository, "https://github.com/mob-sakai/com.test.package.git");
            Assert.AreEqual(p.path, "");
            Assert.AreEqual(p.revision, "1.2.3-preview.4");
        }
    }
}
