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
        [Test]
        public void FindByBarcodeReturnsNullWhenNotFound()
        {
            var repo = new HoldingRepository();

            Assert.That(repo.FindByBarcode("AA:1"), Is.Null);
        }
    }
}
