using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using Library.Util;
using Library.Models;
using Library.Scanner;
using Library.Models.Repositories;

// TODO remove dependency on web mvc stuff from test and prod packages

namespace LibraryTest.Scanner
{
    [TestFixture]
    public class ScanStationTest
    {
        const string SomeBarcode = "QA123:1";
        readonly DateTime now = DateTime.Now;

        ScanStation scanner;
        IRepository<Holding> holdingRepo = new InMemoryRepository<Holding>();
        IRepository<Patron> patronRepo = new InMemoryRepository<Patron>();
        Mock<IClassificationService> classificationService = new Mock<IClassificationService>();
        int somePatronId;

        [SetUp]
        public void Initialize()
        {
            AlwaysReturnBookMaterial(classificationService);
            somePatronId = patronRepo.Create(new Patron { Name = "Anand" });

            scanner = new ScanStation(1, classificationService.Object, holdingRepo, patronRepo);
        }

        [Test]
        public void BranchIdIsSetFromCtor()
        {
            scanner = new ScanStation(1);

            Assert.That(scanner.BranchId, Is.EqualTo(1));
        }

        [Test]
        public void StoresHoldingAtBranch()
        {
            classificationService.Setup(service => service.Classification("anIsbn")).Returns("AB123");

            scanner.AddNewMaterial("anIsbn");

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, "AB123:1");
            Assert.That(holding.BranchId, Is.EqualTo(scanner.BranchId));
        }

        [Test]
        public void AddSecondNewBookWithSameIsbn()
        {
            classificationService.Setup(service => service.Classification("anIsbn")).Returns("AB123");
            scanner.AddNewMaterial("anIsbn");

            var holding = scanner.AddNewMaterial("anIsbn");

            var holdingBarcodes = holdingRepo.GetAll().Select(h => h.Barcode);
            Assert.That(holdingBarcodes, Is.EquivalentTo(new List<string> { "AB123:1", "AB123:2" }));
        }

        void AlwaysReturnBookMaterial(Mock<IClassificationService> classificationService)
        {
            classificationService.Setup(service => service.Retrieve(Moq.It.IsAny<string>()))
                .Returns(new Material() { CheckoutPolicy = new BookCheckoutPolicy() });
        }

        [Test]
        public void CheckOutBook()
        {
            ScanNewMaterial(SomeBarcode);
            TimeService.NextTime = now;
            scanner.AcceptLibraryCard(somePatronId);

            scanner.AcceptBarcode(SomeBarcode);

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, SomeBarcode);
            Assert.That(holding.HeldByPatronId, Is.EqualTo(somePatronId));
            Assert.That(holding.CheckOutTimestamp, Is.EqualTo(now));
        }

        private void CheckOut(string barcode)
        {
            TimeService.NextTime = now;
            scanner.AcceptLibraryCard(somePatronId);
            scanner.AcceptBarcode(barcode);
            scanner.CompleteCheckout();
        }

        [Test]
        public void CheckInBook()
        {
            ScanNewMaterial(SomeBarcode);
            CheckOut(SomeBarcode);

            scanner.AcceptBarcode(SomeBarcode);

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, SomeBarcode);
            Assert.That(holding.HeldByPatronId, Is.EqualTo(Holding.NoPatron));
            Assert.That(holding.IsCheckedOut, Is.False);
        }

        [Test]
        public void TransferViaCheckinToSecondBranch()
        {
            ScanNewMaterial(SomeBarcode);
            CheckOut(SomeBarcode);
            var newBranchId = scanner.BranchId + 1;
            var scannerBranch2 = new ScanStation(newBranchId, classificationService.Object, holdingRepo, patronRepo);

            scannerBranch2.AcceptBarcode(SomeBarcode);

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, SomeBarcode);
            Assert.That(holding.IsCheckedOut, Is.False);
            Assert.That(holding.BranchId, Is.EqualTo(newBranchId));
        }

        [Test]
        public void CheckInBookLate()
        {
            ScanNewMaterial(SomeBarcode);
            CheckOut(SomeBarcode);
            const int daysLate = 2;
            SetTimeServiceToLateByDays(SomeBarcode, daysLate);

            scanner.AcceptBarcode(SomeBarcode);

            var patron = patronRepo.GetByID(somePatronId);
            Assert.That(patron.Balance, Is.EqualTo(RetrievePolicy(SomeBarcode).FineAmount(daysLate)));
        }

        private void SetTimeServiceToLateByDays(string barcode, int days)
        {
            TimeService.NextTime = now.AddDays(RetrievePolicy(barcode).MaximumCheckoutDays() + days);
        }

        private CheckoutPolicy RetrievePolicy(string barcode)
        {
            var classification = Holding.ClassificationFromBarcode(barcode);
            var material = classificationService.Object.Retrieve(classification);
            return material.CheckoutPolicy;
        }

        [Test]
        public void CannotCheckOutCheckedInBookWithoutPatronScan()
        {
            ScanNewMaterial(SomeBarcode);

            Assert.Throws<CheckoutException>(() => scanner.AcceptBarcode(SomeBarcode));
        }

        [Test]
        public void IgnoreWhenSamePatronRescansAlreadyCheckedOutBook()
        {
            ScanNewMaterial(SomeBarcode);
            scanner.AcceptLibraryCard(somePatronId);
            scanner.AcceptBarcode(SomeBarcode);

            scanner.AcceptBarcode(SomeBarcode);

            Assert.That(GetByBarcode(SomeBarcode).HeldByPatronId, Is.EqualTo(somePatronId));
        }

        Holding GetByBarcode(string barcode)
        {
            return HoldingRepositoryExtensions.FindByBarcode(holdingRepo, barcode);
        }

        [Test]
        public void PatronChecksOutTwoBooks()
        {
            ScanNewMaterial(SomeBarcode);
            ScanNewMaterial("XX123:1");
            scanner.AcceptLibraryCard(somePatronId);

            scanner.AcceptBarcode(SomeBarcode);
            scanner.AcceptBarcode("XX123:1");

            scanner.CompleteCheckout();

            Assert.That(GetByBarcode(SomeBarcode).HeldByPatronId, Is.EqualTo(somePatronId));
            Assert.That(GetByBarcode("XX123:1").HeldByPatronId, Is.EqualTo(somePatronId));
        }

        [Test]
        public void TwoPatronsDifferentCopySameBook()
        {
            ScanNewMaterial(SomeBarcode);
            string barcode1Copy2 = Holding.GenerateBarcode(Holding.ClassificationFromBarcode(SomeBarcode), 2);
            ScanNewMaterial(barcode1Copy2);
            scanner.AcceptLibraryCard(somePatronId);
            scanner.AcceptBarcode(SomeBarcode);
            scanner.CompleteCheckout();

            var patronId2 = patronRepo.Create(new Patron());
            scanner.AcceptLibraryCard(patronId2);
            scanner.AcceptBarcode(barcode1Copy2);

            Assert.That(GetByBarcode(barcode1Copy2).HeldByPatronId, Is.EqualTo(patronId2));
        }

        void ScanNewMaterial(string barcode)
        {
            var classification = Holding.ClassificationFromBarcode(barcode);
            var isbn = "x";
            classificationService.Setup(service => service.Classification(isbn)).Returns(classification);
            scanner.AddNewMaterial(isbn);
        }
    }
}