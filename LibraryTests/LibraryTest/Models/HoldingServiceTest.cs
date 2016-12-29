using System;
using System.Linq;
using NUnit.Framework;
using Library.Models;

namespace LibraryTest.Models
{
    [TestFixture]
    public class HoldingServiceTest
    {
        const int Branch1Id = 1;
        const string Book1Classification = "QA123";
        const string Book2Classification = "VV423";
        const int Book1CopyNumber = 1;
        const int Book2CopyNumber = 1;
        const int PatronId = 101;
        HoldingService service;
        private readonly string book1Barcode = Holding.GenerateBarcode(Book1Classification, Book1CopyNumber);
        private readonly string book2Barcode = Holding.GenerateBarcode(Book2Classification, Book2CopyNumber);

        [SetUp]
        public void Initialize()
        {
            service = new HoldingService
            {
                ClassificationService = new DummyClassificationService()
            };
            service.DeleteAllHoldings();
        }

        [Test]
        public void AddHolding()
        {
            var newHolding = service.Add(Book1Classification, Branch1Id);
            Assert.That(newHolding.CopyNumber, Is.EqualTo(Book1CopyNumber));
            var holding = service.Retrieve(newHolding.Barcode);
            Assert.That(holding.Barcode, Is.EqualTo(newHolding.Barcode));
            Assert.That(holding.BranchId, Is.EqualTo(Branch1Id));
            Assert.IsTrue(holding.CheckoutPolicy is BookCheckoutPolicy);
        }

        [Test]
        public void AddHoldingWithoutBookInfoAdded()
        {
            service.ClassificationService = new RetrieveFailClassificationService();

            Assert.Throws<LibraryException>(() => service.Add("Bad classification", Branch1Id));
        }

        [Test]
        public void AddHoldingWithoutBookInfoAddedWithPostconditionTest()
        {
            service.ClassificationService = new RetrieveFailClassificationService();
            var ex = Assert.Throws<LibraryException>(
                () => service.Add("Bad classification", Branch1Id));

            Assert.That(ex.Message, Is.EqualTo("Invalid classification"));
        }

        [Test]
        public void AddSecondCopyOfBook()
        {
            service.Add(Book1Classification, Branch1Id);
            var secondCopy = service.Add(Book1Classification, Branch1Id);
            Assert.That(secondCopy.CopyNumber, Is.EqualTo(2));
        }

        [Test]
        public void RetrieveHoldings()
        {
            service.Add(Book1Classification, Branch1Id);
            service.Add(Book2Classification, Branch1Id);

            var holdings = service.RetrieveAll();

            Assert.That(
                holdings.Select(h => h.Barcode),
                Is.EqualTo(new string[] { book1Barcode, book2Barcode }));
        }

        [Test]
        public void PersistHoldings()
        {
            var secondService = new HoldingService();
            Assert.IsNull(secondService.Retrieve(book1Barcode));

            service.Add(Book1Classification, Branch1Id);
            var holding = secondService.Retrieve(book1Barcode);
            Assert.That(holding.Barcode, Is.EqualTo(book1Barcode));
        }

        [Test]
        public void CkO()
        {
            service.Add(Book1Classification, Branch1Id);

            Assert.That(service.IsCheckedOut("QA123:1"), Is.False);

            var d = DateTime.Now;
            service.CheckOut(d, book1Barcode, PatronId, CheckoutPolicies.BookCheckoutPolicy);

            Assert.IsTrue(service.IsCheckedOut(book1Barcode));

            var holding = service.Retrieve(book1Barcode);
            Assert.That(holding.IsCheckedOut);
            Assert.That(holding.HeldByPatronId, Is.EqualTo(PatronId));
            Assert.That(holding.CheckOutTimestamp, Is.EqualTo(d));
            Assert.AreSame(CheckoutPolicies.BookCheckoutPolicy, holding.CheckoutPolicy);

            var t = DateTime.Now.AddDays(1);
            service.CheckIn(t, book1Barcode, Branch1Id);

            holding = service.Retrieve(book1Barcode);
            Assert.IsFalse(holding.IsCheckedOut);
            Assert.That(holding.HeldByPatronId, Is.EqualTo(Holding.NoPatron));
        }
    }

    class RetrieveFailClassificationService : DummyClassificationService
    {
        override public Material Retrieve(string classification)
        {
            return null;
        }
    }

    class DummyClassificationService : IClassificationService
    {
        public string Classification(string isbn) { return ""; }

        public void AddBook(string classification, string title, string author, string year)
        {
        }

        virtual public Material Retrieve(string classification)
        {
            return new Material("", "", "", "");
        }

        public void DeleteAllBooks()
        {
        }

        public void AddMovie(string classification, string title, string director, string year)
        {
        }
    }
}
