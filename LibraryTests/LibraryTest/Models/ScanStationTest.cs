using System;
using System.Collections.Generic;
using NUnit.Framework;
using Library.Util;
using Library.Models;

// TODO wrong place?
namespace LibraryTest.Models
{
    //[TestFixture]
    //public class ScanStationTest
    //{
    //    const int Branch1Id = 1;
    //    const int Branch2Id = 2;
    //    public const string Isbn1 = "ABC";
    //    public const string Isbn2 = "DEF";
    //    public const string Classification1 = "QA123";
    //    public const string Classification2 = "PS987";
    //    public DateTime CheckoutTime = DateTime.Now;

    //    readonly string barcode1 = Holding.GenerateBarcode(Classification1, 1);
    //    readonly string barcode2 = Holding.GenerateBarcode(Classification2, 1);

    //    private int patronId1;
    //    private int patronId2;

    //    private IClassificationService classificationService;
    //    private ScanStation scanner;
    //    private HoldingService holdingService;
    //    private PatronService patronService;

    //    [SetUp]
    //    public void Initialize()
    //    {
    //        classificationService = new StubClassificationService();
    //        classificationService.AddBook(Classification1, "T1", "A1", "2001");
    //        classificationService.AddBook(Classification2, "T2", "A2", "2002");

    //        scanner = new ScanStation(Branch1Id, classificationService);

    //        holdingService = new HoldingService();
    //        holdingService.DeleteAllHoldings();

    //        patronService = new PatronService();
    //        PatronService.DeleteAllPatrons();

    //        patronId1 = patronService.Add("Joe");
    //        patronId2 = patronService.Add("Jane");

    //        var holding = scanner.AddNewMaterial(Isbn1);
    //        Assert.That(holding.Barcode, Is.EqualTo(barcode1));
    //    }

    //    [Test]
    //    public void AddNewBook()
    //    {
    //        var holding = new HoldingService().Retrieve(barcode1);

    //        Assert.That(holding.Barcode, Is.EqualTo("QA123:1"));
    //        Assert.That(holding.BranchId, Is.EqualTo(Branch1Id));
    //    }

    //    [Test]
    //    public void AddSecondNewBookWithSameIsbn()
    //    {
    //        var holding = scanner.AddNewMaterial(Isbn1);
    //        Assert.That(holding.Barcode, Is.EqualTo(Classification1 + ":2"));
    //    }

    //    [Test]
    //    public void CheckOutBook()
    //    {
    //        scanner.AddNewMaterial(Isbn1);

    //        TimeService.NextTime = CheckoutTime;
    //        scanner.AcceptLibraryCard(patronId1);
    //        scanner.AcceptBarcode(barcode1);
    //        scanner.CompleteCheckout();

    //        var holding = holdingService.Retrieve(barcode1);
    //        Assert.That(holding.HeldByPatronId, Is.EqualTo(patronId1));
    //        Assert.That(holding.CheckOutTimestamp, Is.EqualTo(CheckoutTime));

    //        var patron = new PatronService().Retrieve(patronId1);
    //        Assert.That(patron.Holdings, Is.EqualTo(new List<string> { barcode1 }));
    //    }

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

    //    [Test]
    //    public void CheckInBook()
    //    {
    //        CheckOut(barcode1);
    //        scanner.AcceptBarcode(barcode1);

    //        var holding = holdingService.Retrieve(barcode1);
    //        Assert.That(holding.HeldByPatronId, Is.EqualTo(Holding.NoPatron));
    //        Assert.That(holding.IsCheckedOut, Is.False);

    //        var patron = new PatronService().Retrieve(patronId1);
    //        Assert.That(patron.Holdings.Count, Is.EqualTo(0));
    //    }

    //    [Test]
    //    public void TransfersAreCheckinsToDifferentBranch()
    //    {
    //        CheckOut(barcode1);

    //        var scannerBranch2 = new ScanStation(Branch2Id, classificationService);
    //        scannerBranch2.AcceptBarcode(barcode1);

    //        var holding = holdingService.Retrieve(barcode1);
    //        Assert.That(holding.IsCheckedOut, Is.False);
    //        Assert.That(holding.BranchId, Is.EqualTo(Branch2Id));
    //    }

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

    //    private void CheckOut(string barcode)
    //    {
    //        TimeService.NextTime = CheckoutTime;
    //        scanner.AcceptLibraryCard(patronId1);
    //        scanner.AcceptBarcode(barcode);
    //        scanner.CompleteCheckout();
    //    }
    //}

    //class StubClassificationService : MasterClassificationService
    //{
    //    public override string Classification(string isbn)
    //    {
    //        if (isbn == ScanStationTest.Isbn1)
    //            return ScanStationTest.Classification1;
    //        if (isbn == ScanStationTest.Isbn2)
    //            return ScanStationTest.Classification2;
    //        return "";
    //    }
    //}
}
