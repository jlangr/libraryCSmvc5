using System;
using System.Collections.Generic;
using NUnit.Framework;
using Library.Util;
using Library.Models;
using Library.Models.ScanStation;
using Library.Models.Repositories;
using Moq;

// TODO wrong place?
namespace LibraryTest.Models
{
    [TestFixture]
    public class ScanStationTest
    {
        const int Branch1Id = 1;
        const int Branch2Id = 2;
        public const string Isbn1 = "ABC";
        public const string Isbn2 = "DEF";
        public const string Classification1 = "QA123";
        public const string Classification2 = "PS987";
        public DateTime CheckoutTime = new DateTime(2018, 1, 15);

        readonly string barcode1 = Holding.GenerateBarcode(Classification1, 1);
        readonly string barcode2 = Holding.GenerateBarcode(Classification2, 1);

        private int patronId1;
        private int patronId2;

        private IClassificationService classificationService;
        private ScanStation scanner;
        private IRepository<Holding> holdingRepo;
        private IRepository<Patron> patronRepo;

        [SetUp]
        public void Initialize()
        {
            holdingRepo = new InMemoryRepository<Holding>();
            patronRepo = new InMemoryRepository<Patron>();

            patronId1 = patronRepo.Create(new Patron { Name = "Anand" });
        }

        [Test]
        public void ScannerCreation()
        {
            scanner = new ScanStation(1, null, null, null);

            Assert.That(scanner.BranchId, Is.EqualTo(1));
        }

        [Test]
        public void AddNewMaterialStoresHoldingWithAssociatedBarcode()
        {
            var classificationService = new Mock<IClassificationService>();
            classificationService.Setup(service => service.Classification(Isbn1))
                .Returns(Classification1);
            scanner = new ScanStation(Branch1Id, classificationService.Object, holdingRepo, patronRepo);

            scanner.AddNewMaterial(Isbn1);

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, barcode1);
            Assert.That(holding.Barcode, Is.EqualTo("QA123:1"));
            Assert.That(holding.HeldByPatronId, Is.Zero);
            Assert.That(holding.BranchId, Is.EqualTo(scanner.BranchId));
        }

        //    [Test]
        //    public void AddSecondNewBookWithSameIsbn()
        //    {
        //        var holding = scanner.AddNewMaterial(Isbn1);
        //        Assert.That(holding.Barcode, Is.EqualTo(Classification1 + ":2"));
        //    }

        [Test]
        public void CheckOutBook()
        {
            var classificationService = new Mock<IClassificationService>();
            classificationService.Setup(service => service.Classification(Isbn1))
                .Returns(Classification1);
            scanner = new ScanStation(Branch1Id, classificationService.Object, holdingRepo, patronRepo);

            scanner.AddNewMaterial(Isbn1);
            TimeService.NextTime = CheckoutTime;
            scanner.AcceptLibraryCard(patronId1);

            scanner.AcceptBarcode(barcode1);
            scanner.CompleteCheckout();

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, barcode1);
            Assert.That(holding.HeldByPatronId, Is.EqualTo(patronId1));
            Assert.That(holding.CheckOutTimestamp, Is.EqualTo(CheckoutTime));
        }

        //    private void SetTimeServiceToLateByDays(string classification, int days)
        //    {
        //        var policy = RetrievePolicy(classification);
        //        TimeService.NextTime = CheckoutTime.AddDays(policy.MaximumCheckoutDays() + days);
        //    }

        //    private CheckoutPolicy RetrievePolicy(string classification)
        //    {
        //        var material = classificationService.Retrieve(classification);
        //        return material.CheckoutPolicy;
        //    }

        private void CheckOut(string barcode)
        {
            TimeService.NextTime = CheckoutTime;
            scanner.AcceptLibraryCard(patronId1);
            scanner.AcceptBarcode(barcode);
            scanner.CompleteCheckout();
        }

        [Test]
        public void CheckInBook()
        {
            var classificationService = new Mock<IClassificationService>();
            classificationService.Setup(service => service.Classification(Isbn1))
                .Returns(Classification1);
            scanner = new ScanStation(Branch1Id, classificationService.Object, holdingRepo, patronRepo);

            scanner.AddNewMaterial(Isbn1);
            CheckOut(barcode1);

            classificationService.Setup(service => service.Retrieve(Classification1))
                .Returns(new Material() { CheckoutPolicy = new BookCheckoutPolicy() });

            scanner.AcceptBarcode(barcode1);

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, barcode1);
            Assert.That(holding.HeldByPatronId, Is.EqualTo(Holding.NoPatron));
            Assert.That(holding.IsCheckedOut, Is.False);
        }

        [Test]
        public void TransfersAreCheckinsToDifferentBranch()
        {
            var classificationService = new Mock<IClassificationService>();
            classificationService.Setup(service => service.Classification(Isbn1))
                .Returns(Classification1);
            scanner = new ScanStation(Branch1Id, classificationService.Object, holdingRepo, patronRepo);

            scanner.AddNewMaterial(Isbn1);
            CheckOut(barcode1);

            var scannerBranch2 = new ScanStation(Branch2Id, classificationService.Object, holdingRepo, patronRepo);

            // for checkin
            classificationService.Setup(service => service.Retrieve(Classification1))
                .Returns(new Material() { CheckoutPolicy = new BookCheckoutPolicy() });

            scannerBranch2.AcceptBarcode(barcode1);

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, barcode1);
            Assert.That(holding.IsCheckedOut, Is.False);
            Assert.That(holding.BranchId, Is.EqualTo(Branch2Id));
        }

        //    [Test]
        //    public void CheckInBookLate()
        //    {
        //        CheckOut(barcode1);

        //        const int daysLate = 2;
        //        SetTimeServiceToLateByDays(Classification1, daysLate);
        //        scanner.AcceptBarcode(barcode1);

        //        var patron = new PatronService().Retrieve(patronId1);
        //        Assert.That(patron.Balance, Is.EqualTo(RetrievePolicy(Classification1).FineAmount(daysLate)));
        //    }

        //    [Test]
        //    public void CannotCheckOutCheckedInBookWithoutPatronScan()
        //    {
        //        Assert.IsFalse(holdingService.IsCheckedOut(barcode1));

        //        Assert.Throws<CheckoutException>(()=>scanner.AcceptBarcode(barcode1));
        //    }

        //    [Test]
        //    public void IgnoreWhenSamePatronRescansAlreadyCheckedOutBook()
        //    {
        //        scanner.AcceptLibraryCard(patronId1);
        //        scanner.AcceptBarcode(barcode1);
        //        scanner.AcceptBarcode(barcode1);
        //        scanner.CompleteCheckout();

        //        AssertHeldBy(barcode1, patronId1);
        //        var patron = patronService.Retrieve(patronId1);
        //        Assert.That(patron.Holdings, Is.EqualTo(new List<string> { barcode1 }));
        //    }

        //    private void AssertHeldBy(string barcode, int patronId)
        //    {
        //        var holding = holdingService.Retrieve(barcode);
        //        Assert.That(holding.HeldByPatronId, Is.EqualTo(patronId));
        //    }

        //    [Test]
        //    public void PatronChecksOutTwoBooks()
        //    {
        //        scanner.AddNewMaterial(Isbn2);

        //        scanner.AcceptLibraryCard(patronId1);

        //        scanner.AcceptBarcode(barcode1);
        //        scanner.AcceptBarcode(barcode2);

        //        scanner.CompleteCheckout();

        //        AssertHeldBy(barcode1, patronId1);
        //        AssertHeldBy(barcode2, patronId1);
        //    }

        //    [Test]
        //    public void TwoPatronsDifferentCopySameBook()
        //    {
        //        scanner.AddNewMaterial(Isbn1);

        //        scanner.AcceptLibraryCard(patronId1);
        //        scanner.AcceptBarcode(barcode1);
        //        scanner.CompleteCheckout();

        //        scanner.AcceptLibraryCard(patronId2);
        //        string barcode1Copy2 = Holding.GenerateBarcode(Classification1, 2);
        //        scanner.AcceptBarcode(barcode1Copy2);
        //        scanner.CompleteCheckout();

        //        AssertHeldBy(barcode1Copy2, patronId2);
        //    }

        //}
    }
}