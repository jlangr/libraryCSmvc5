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
        HoldingRepository repo;

        [SetUp]
        public void Create()
        {
            repo = new HoldingRepository();
            repo.Clear();
        }

        [Test]
        public void FindByBarcodeReturnsNullWhenNotFound()
        {
            Assert.That(repo.FindByBarcode("AA:1"), Is.Null);
        }

        [Test]
        public void FindByBarcodeReturnsHoldingMatchingClassificationAndCopy()
        {
            var holding = new Holding { Classification = "AA123", CopyNumber = 2 };

            repo.Create(holding);

            Assert.That(repo.FindByBarcode("AA123:2"), Is.EqualTo(holding));
        }
    }
}
