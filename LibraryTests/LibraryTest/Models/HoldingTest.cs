using System;
using NUnit.Framework;
using Library.Models;

// TODO add some junk. E.g. try / catch, assert against not null

namespace LibraryTest.Library.Models
{
    [TestFixture]
    public class HoldingTest
    {
        const int PatronId = 101;

        [Test]
        public void CreateWithCommonArguments()
        {
            const int branchId = 10;
            var holding = new Holding("QA123", 2, branchId);
            Assert.That(holding.Barcode, Is.EqualTo("QA123:2"));
            Assert.That(holding.BranchId, Is.EqualTo(branchId));
        }

        [Test]
        public void IsValidBarcodeReturnsFalseWhenItHasNoColon()
        {
            Assert.That(Holding.IsBarcodeValid("ABC"), Is.False);
        }

        [Test]
        public void IsValidBarcodeReturnsFalseWhenItsCopyNumberNotPositiveInt()
        {
            Assert.That(Holding.IsBarcodeValid("ABC:X"), Is.False);
            Assert.That(Holding.IsBarcodeValid("ABC:0"), Is.False);
        }

        [Test]
        public void IsValidBarcodeReturnsFalseWhenItsClassificationIsEmpty()
        {
            Assert.That(Holding.IsBarcodeValid(":1"), Is.False);
        }

        [Test]
        public void IsValidBarcodeReturnsTrueWhenFormattedCorrectly()
        {
            Assert.That(Holding.IsBarcodeValid("ABC:1"));
        }

        [Test]
        public void GenerateBarcode()
        {
            Assert.That(Holding.GenerateBarcode("QA234", 3), Is.EqualTo("QA234:3"));
        }

        [Test]
        public void ClassificationFromBarcode()
        {
            Assert.That(Holding.ClassificationFromBarcode("QA234:3"), Is.EqualTo("QA234"));
        }

        [Test]
        public void CopyNumberFromBarcode()
        {
            Assert.That(Holding.CopyNumberFromBarcode("QA234:3"), Is.EqualTo(3));
        }

        [Test]
        public void Co()
        {
            var holding = new Holding { Classification = "", CopyNumber = 1, BranchId = 1 };
            Assert.IsFalse(holding.IsCheckedOut);
            var now = DateTime.Now;

            var policy = CheckoutPolicies.BookCheckoutPolicy;
            holding.CheckOut(now, PatronId, policy);

            Assert.IsTrue(holding.IsCheckedOut);

            Assert.AreSame(policy, holding.CheckoutPolicy);
            Assert.That(holding.HeldByPatronId, Is.EqualTo(PatronId));

            var dueDate = now.AddDays(policy.MaximumCheckoutDays());
            Assert.That(holding.DueDate, Is.EqualTo(dueDate));

            Assert.That(holding.BranchId, Is.EqualTo(Branch.CheckedOutId));
        }

        [Test]
        public void CheckIn()
        {
            var holding = new Holding { Classification = "X", BranchId = 1, CopyNumber = 1 };
            holding.CheckOut(DateTime.Now, PatronId, CheckoutPolicies.BookCheckoutPolicy);
            var tomorrow = DateTime.Now.AddDays(1);
            const int newBranchId = 2;
            
            holding.CheckIn(tomorrow, newBranchId);
            
            Assert.IsFalse(holding.IsCheckedOut);
            Assert.That(holding.HeldByPatronId, Is.EqualTo(Holding.NoPatron));
            Assert.That(holding.CheckOutTimestamp, Is.Null);
            Assert.That(holding.BranchId, Is.EqualTo(newBranchId));
            Assert.That(holding.LastCheckedIn, Is.EqualTo(tomorrow));
        }
    }
}
