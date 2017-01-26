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
        IRepository<Holding> holdingRepo;
        IRepository<Patron> patronRepo;
        Mock<IClassificationService> classificationService;
        int somePatronId;

        [SetUp]
        public void Initialize()
        {
            holdingRepo = new InMemoryRepository<Holding>();
            patronRepo = new InMemoryRepository<Patron>();
            classificationService = new Mock<IClassificationService>();
            AlwaysReturnBookMaterial(classificationService);
            somePatronId = patronRepo.Create(new Patron { Name = "x" });

            scanner = new ScanStation(1, classificationService.Object, holdingRepo, patronRepo);
        }

        void AlwaysReturnBookMaterial(Mock<IClassificationService> classificationService)
        {
            classificationService.Setup(service => service.Retrieve(Moq.It.IsAny<string>()))
                .Returns(new Material() { CheckoutPolicy = new BookCheckoutPolicy() });
        }

        void ScanNewMaterial(string barcode)
        {
            var classification = Holding.ClassificationFromBarcode(barcode);
            var isbn = "x";
            classificationService.Setup(service => service.Classification(isbn)).Returns(classification);
            scanner.AddNewMaterial(isbn);
        }

        Holding GetByBarcode(string barcode)
        {
            return HoldingRepositoryExtensions.FindByBarcode(holdingRepo, barcode);
        }

        void CheckOut(string barcode)
        {
            TimeService.NextTime = now;
            scanner.AcceptLibraryCard(somePatronId);
            scanner.AcceptBarcode(barcode);
        }

        public class TestsNotRequiringCheckout : ScanStationTest
        {
            [Test]
            public void BranchIdIsRequiredWhenConstructed()
            {
                var otherScanner = new ScanStation(1);

                Assert.That(otherScanner.BranchId, Is.EqualTo(1));
            }

            [Test]
            public void StoresHoldingAtBranchWhenNewMaterialAdded()
            {
                classificationService.Setup(service => service.Classification("anIsbn")).Returns("AB123");

                scanner.AddNewMaterial("anIsbn");

                Assert.That(GetByBarcode("AB123:1").BranchId, Is.EqualTo(scanner.BranchId));
            }

            [Test]
            public void CopyNumberIncrementedWhenNewMaterialWithSameIsbnAdded()
            {
                classificationService.Setup(service => service.Classification("anIsbn")).Returns("AB123");
                scanner.AddNewMaterial("anIsbn");

                var holding = scanner.AddNewMaterial("anIsbn");

                var holdingBarcodes = holdingRepo.GetAll().Select(h => h.Barcode);
                Assert.That(holdingBarcodes, Is.EquivalentTo(new List<string> { "AB123:1", "AB123:2" }));
            }

            [Test]
            public void ThrowsWhenAttemptingToCheckInCheckedOutBookWithoutPatronScan()
            {
                ScanNewMaterial(SomeBarcode);

                Assert.Throws<CheckoutException>(() => scanner.AcceptBarcode(SomeBarcode));
            }

            [Test]
            public void PatronIdUpdatedWhenLibraryCardAccepted()
            {
                scanner.AcceptLibraryCard(somePatronId);

                Assert.That(scanner.CurrentPatronId, Is.EqualTo(somePatronId));
            }

            [Test]
            public void PatronIdClearedWhenCheckoutCompleted()
            {
                scanner.AcceptLibraryCard(somePatronId);

                scanner.CompleteCheckout();

                Assert.That(scanner.CurrentPatronId, Is.EqualTo(ScanStation.NoPatron));
            }
        }

        public class WhenNewMaterialCheckdOut : ScanStationTest
        {
            [SetUp]
            public void CheckOutNewMaterial()
            {
                ScanNewMaterial(SomeBarcode);
                CheckOut(SomeBarcode);
            }

            [Test]
            public void HeldByPatronIdUpdated()
            {
                Assert.That(GetByBarcode(SomeBarcode).HeldByPatronId, Is.EqualTo(somePatronId));
            }

            [Test]
            public void CheckOutTimestampUpdated()
            {
                Assert.That(GetByBarcode(SomeBarcode).CheckOutTimestamp, Is.EqualTo(now));
            }

            [Test]
            public void IsCheckedOutMarkedTrue()
            {
                Assert.That(GetByBarcode(SomeBarcode).IsCheckedOut, Is.True);
            }

            [Test]
            public void RescanBySamePatronIsIgnored()
            {
                scanner.AcceptBarcode(SomeBarcode);

                Assert.That(GetByBarcode(SomeBarcode).HeldByPatronId, Is.EqualTo(somePatronId));
            }

            [Test]
            public void SecondMaterialCheckedOutAddedToPatron()
            {
                ScanNewMaterial("XX123:1");

                CheckOut("XX123:1");

                Assert.That(GetByBarcode(SomeBarcode).HeldByPatronId, Is.EqualTo(somePatronId));
                Assert.That(GetByBarcode("XX123:1").HeldByPatronId, Is.EqualTo(somePatronId));
            }

            [Test]
            public void SecondPatronCanCheckOutSecondCopyOfSameClassification()
            {
                string barcode1Copy2 = Holding.GenerateBarcode(Holding.ClassificationFromBarcode(SomeBarcode), 2);
                ScanNewMaterial(barcode1Copy2);

                var patronId2 = patronRepo.Create(new Patron());
                scanner.AcceptLibraryCard(patronId2);
                scanner.AcceptBarcode(barcode1Copy2);

                Assert.That(GetByBarcode(barcode1Copy2).HeldByPatronId, Is.EqualTo(patronId2));
            }

            [Test]
            public void CheckInAtSecondBranchResultsInTransfer()
            {
                var newBranchId = scanner.BranchId + 1;
                var scannerBranch2 = new ScanStation(newBranchId, classificationService.Object, holdingRepo, patronRepo);

                scannerBranch2.AcceptBarcode(SomeBarcode);

                Assert.That(GetByBarcode(SomeBarcode).BranchId, Is.EqualTo(newBranchId));
            }

            [Test]
            public void LateCheckInResultsInFine()
            {
                scanner.CompleteCheckout();
                const int daysLate = 2;
                SetTimeServiceToLateForMaterialTypeByDays(SomeBarcode, daysLate);

                scanner.AcceptBarcode(SomeBarcode);

                var patron = patronRepo.GetByID(somePatronId);
                Assert.That(patron.Balance, Is.EqualTo(RetrievePolicy(SomeBarcode).FineAmount(daysLate)));
            }

            private void SetTimeServiceToLateForMaterialTypeByDays(string barcode, int days)
            {
                TimeService.NextTime = now.AddDays(RetrievePolicy(barcode).MaximumCheckoutDays() + days);
            }

            private CheckoutPolicy RetrievePolicy(string barcode)
            {
                var classification = Holding.ClassificationFromBarcode(barcode);
                var material = classificationService.Object.Retrieve(classification);
                return material.CheckoutPolicy;
            }
        }

        public class WhenMaterialCheckedIn : ScanStationTest
        {
            [SetUp]
            public void CheckOutAndCheckInNewMaterial()
            {
                ScanNewMaterial(SomeBarcode);
                CheckOut(SomeBarcode);
                scanner.CompleteCheckout();
                CheckIn(SomeBarcode);
            }

            void CheckIn(string barcode)
            {
                TimeService.NextTime = now;
                scanner.AcceptBarcode(barcode);
            }

            [Test]
            public void PatronCleared()
            {
                Assert.That(GetByBarcode(SomeBarcode).HeldByPatronId, Is.EqualTo(Holding.NoPatron));
            }

            [Test]
            public void HoldingMarkedAsNotCheckedOut()
            {
                Assert.That(GetByBarcode(SomeBarcode).IsCheckedOut, Is.False);
            }

            [Test]
            public void CheckOutTimestampCleared()
            {
                Assert.That(GetByBarcode(SomeBarcode).CheckOutTimestamp, Is.Null);
            }

            [Test]
            public void LastCheckedInTimestampUpdated()
            {
                Assert.That(GetByBarcode(SomeBarcode).LastCheckedIn, Is.EqualTo(now));
            }
        }
    }
}