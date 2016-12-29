using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Library.Models;

namespace LibraryTest.Models
{
    [TestFixture]
    public class PatronServiceTest
    {
        private PatronService service;
        private int id1;
        private const string Name1 = "name1";
        private const string Name2 = "name2";

        // TODO split into separate fixtures!
        [SetUp]
        public void Initialize()
        {
            PatronService.DeleteAllPatrons();
            service = new PatronService();
            id1 = service.Add(Name1);
        }

        [Test]
        public void AddPatron()
        {
            var patron = service.Retrieve(id1);
            Assert.That(patron.Id, Is.EqualTo(id1));
            Assert.That(patron.Name, Is.EqualTo(Name1));
        }

        [Test]
        public void ReturnsNullWhenPatronIdNotFound()
        {
            Assert.That(service.Retrieve(id1 + 1), Is.Null);
        }

        [Test]
        public void Persistence()
        {
            service = new PatronService();
            Assert.That(service.Retrieve(id1), Is.Not.Null);
        }

        [Test]
        public void AddMultiplePatrons()
        {
            var id2 = service.Add(Name2);

            var patron1 = service.Retrieve(id1);
            AssertPatron(patron1, id1, Name1);

            var patron2 = service.Retrieve(id2);
            AssertPatron(patron2, id2, Name2);
        }

        [Test]
        public void RetrieveAll()
        {
            service.Add(Name2);

            var patrons = service.RetrieveAll();

            Assert.That(patrons.Select(p => p.Name),
                Is.EqualTo(new string[] { Name1, Name2 }));
        }

        [Test]
        public void CheckOutBooks()
        {
            const string barcode = "QA123:1";
            service.CheckOut(id1, barcode);

            var patron = service.Retrieve(id1);
            Assert.That(patron.Holdings, Is.EqualTo(new List<string> { barcode }));
        }

        [Test]
        public void CheckInBooks()
        {
            const string barcode = "QA123:1";
            service.CheckOut(id1, barcode);

            service.CheckIn(id1, barcode);

            var patron = service.Retrieve(id1);
            Assert.That(patron.Holdings, Has.Count.EqualTo(0));
        }

        private static void AssertPatron(Patron patron, int id, string name)
        {
            Assert.That(patron.Id, Is.EqualTo(id));
            Assert.That(patron.Name, Is.EqualTo(name));
        }
    }
}