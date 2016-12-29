using System.Collections.Generic;
using NUnit.Framework;
using Library.Models;

namespace LibraryTest.Models
{
    [TestFixture]
    public class PatronTest
    {
        const int Id = 101;
        const string Name = "Joe";
        const string Barcode1 = "QA123:1";
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
            patron.CheckOut(Barcode1);
            Assert.That(patron.Holdings, Is.EqualTo(new List<string> { Barcode1 }));
        }

        [Test]
        public void CheckIn()
        {
            patron.CheckOut(Barcode1);
            patron.CheckIn(Barcode1);
            Assert.That(patron.Holdings, Has.Count.EqualTo(0));
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
