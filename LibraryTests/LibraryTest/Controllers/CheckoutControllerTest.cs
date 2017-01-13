using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Library.Controllers;
using Library.Models;
using Library.Models.Repositories;
using Library.Extensions.SystemWebMvcController;
using System;

namespace LibraryTests.LibraryTest.Controllers
{
    [TestFixture]
    public class CheckOutControllerTest
    {
        const string ModelKey = "CheckOut";
        CheckOutController controller;
        IRepository<Holding> holdingRepo;
        IRepository<Patron> patronRepo;
        CheckOutViewModel checkout;
        int branchId;
        private int patronId;

        [SetUp]
        public void Initialize()
        {
            holdingRepo = new InMemoryRepository<Holding>();

            var branchRepo = new InMemoryRepository<Branch>();
            branchId = branchRepo.Create(new Branch() { Name = "b" });

            patronRepo = new InMemoryRepository<Patron>();
            patronId = patronRepo.Create(new Patron { Name = "x" });

            controller = new CheckOutController(branchRepo, holdingRepo, patronRepo);
            checkout = new CheckOutViewModel();
        }

        [Test]
        public void GeneratesErrorWhenPatronIdInvalid()
        {
            var result = controller.Index(checkout) as ViewResult;

            Assert.That(controller.SoleErrorMessage(ModelKey), Is.EqualTo("Invalid patron ID."));
        }

        [Test]
        public void GeneratesErrorWhenNoHoldingFoundForBarcode()
        {
            checkout.PatronId = patronId;
            checkout.Barcode = "NONEXISTENT:1";

            var result = controller.Index(checkout) as ViewResult;

            Assert.That(controller.SoleErrorMessage(ModelKey), Is.EqualTo("Invalid holding barcode."));
        }

        [Test]
        public void GeneratesErrorWhenHoldingAlreadyCheckedOut()
        {
            var holding = new Holding("ABC", 1, branchId);
            holdingRepo.Create(holding);
            checkout.PatronId = patronId;
            checkout.Barcode = holding.Barcode;
            controller.Index(checkout);

            var result = controller.Index(checkout) as ViewResult;

            Assert.That(controller.SoleErrorMessage(ModelKey), Is.EqualTo("Holding is already checked out."));
        }

        [Test]
        public void GeneratesErrorWhenBarcodeHasInvalidFormat()
        {
            checkout.PatronId = patronId;
            checkout.Barcode = "HasNoColon";

            var result = controller.Index(checkout) as ViewResult;

            Assert.That(controller.SoleErrorMessage(ModelKey), Is.EqualTo("Invalid holding barcode format."));
        }

        [Test]
        public void StoresHoldingOnPatronOnSuccess()
        {
            // TODO simplify
            var holding = new Holding { Classification = "ABC", CopyNumber = 1 };
            holding.CheckIn(DateTime.Now, branchId);
            var holdingId = holdingRepo.Create(holding);
            checkout.PatronId = patronId;
            checkout.Barcode = "ABC:1";

            var result = controller.Index(checkout) as ViewResult;

            var patron = patronRepo.GetByID(patronId);
            Assert.That(patron.HoldingIds, Is.EqualTo(new List<int> { holdingId }));
        }

        // shows error when holding is already checked out
        // ? validate patron ID
        // on success:
        //    patron contains holding
        //    holding marked as checked out
        //    redirects back to Index
    }
}
