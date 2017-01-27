using Library.ControllerHelpers;
using Library.Models;
using Library.Models.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTests.LibraryTest.ControllerHelpers
{
    [TestFixture]
    public class HoldingsControllerUtilTest
    {
        [Test]
        public void IncrementsCopyNumberUsingCount()
        {
            var holdingRepo = new InMemoryRepository<Holding>();
            holdingRepo.Create(new Holding { Classification = "AB123", CopyNumber = 1 });
            holdingRepo.Create(new Holding { Classification = "AB123", CopyNumber = 2 });
            holdingRepo.Create(new Holding { Classification = "XX123", CopyNumber = 1 });

            var copyNumber = HoldingsControllerUtil.NextAvailableCopyNumber(holdingRepo, "AB123");

            Assert.That(copyNumber, Is.EqualTo(3));
        }
    }
}
