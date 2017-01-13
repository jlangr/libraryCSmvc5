using Library.Models;
using Library.Models.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTests.LibraryTest.Models.Repositories
{
    [TestFixture, Category("slow")]
    public class HoldingRepositoryTest
    {
        EntityRepository<Holding> repo;

        [SetUp]
        public void Create()
        {
            repo = new EntityRepository<Holding>(db => db.Holdings);
            repo.Clear();
        }

        // TODO belong elsewhere?
        [Test]
        public void FindByBarcodeReturnsNullWhenNotFound()
        {
            Assert.That(HoldingRepositoryExtensions.FindByBarcode(repo, "AA:1"), Is.Null);
        }

        // TODO belong elsewhere?
        [Test]
        public void FindByBarcodeReturnsHoldingMatchingClassificationAndCopy()
        {
            var holding = new Holding { Classification = "AA123", CopyNumber = 2 };

            repo.Create(holding);

            Assert.That(HoldingRepositoryExtensions.FindByBarcode(repo, "AA123:2"), Is.EqualTo(holding));
        }
    }
}
