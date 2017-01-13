using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Library.Models;

namespace LibraryTest.Models
{
    [TestFixture]
    public class PatronTest
    {
        const int Id = 101;
        const string Name = "Joe";
        const int HoldingId = 2;
        private Patron patron;

        [SetUp]
        public void Initialize()
        {
            patron = new Patron(Id, Name);
        }

        [Test]
        public void Create()
        {
            Assert.That(patron.Name, Is.EqualTo(Name));
            Assert.That(patron.Id, Is.EqualTo(Id));
            Assert.That(patron.Balance, Is.EqualTo(0));
        }

        [Test]
        public void CheckOut()
        {
            patron.CheckOut(HoldingId);
            Assert.That(patron.HoldingIds, Is.EqualTo(new List<int> { HoldingId }));
        }

        [Test]
        public void CheckIn()
        {
            patron.CheckOut(HoldingId);
            patron.CheckIn(HoldingId);
            Assert.That(patron.HoldingIds.Any(), Is.EqualTo(false));
        }

        [Test]
        public void FinesIncreaseBalance()
        {
            patron.Fine(0.10m);
            Assert.That(patron.Balance, Is.EqualTo(0.10m));
            patron.Fine(0.10m);
            Assert.That(patron.Balance, Is.EqualTo(0.20m));
        }

        [Test]
        public void RemitReducesBalance()
        {
            patron.Fine(1.10m);
            patron.Remit(0.20m);
            Assert.That(patron.Balance, Is.EqualTo(0.90m));
        }
    }
}
