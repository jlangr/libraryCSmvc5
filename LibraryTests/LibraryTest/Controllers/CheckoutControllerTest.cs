using System.Web.Mvc;
using NUnit.Framework;
using Library.Controllers;
using Library.Models;
using Library.Models.Repositories;
using Library.Extensions.SystemWebMvcController;

namespace LibraryTests.LibraryTest.Controllers
{
    [TestFixture]
    public class CheckOutControllerTest
    {
        const string ModelKey = "CheckOut";
        CheckOutController controller;
        HoldingRepository holdingRepo;
        IRepository<Patron> patronRepo;
        CheckOutViewModel checkout;

        [SetUp]
        public void Initialize()
        {
            holdingRepo = new HoldingRepository();
            patronRepo = new InMemoryRepository<Patron>();
            controller = new CheckOutController(holdingRepo, patronRepo);
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
            var patronId = patronRepo.Create(new Patron { Name = "x" });
            checkout.PatronId = patronId;
            checkout.Barcode = "BAD:1";

            var result = controller.Index(checkout) as ViewResult;

            Assert.That(controller.SoleErrorMessage(ModelKey), Is.EqualTo("Invalid holding barcode."));
        }

        [Test]
        public void GeneratesErrorWhenBarcodeHasInvalidFormat()
        {
            var patronId = patronRepo.Create(new Patron { Name = "x" });
            checkout.PatronId = patronId;
            checkout.Barcode = "HasNoColon";

            var result = controller.Index(checkout) as ViewResult;

            Assert.That(controller.SoleErrorMessage(ModelKey), Is.EqualTo("Invalid holding barcode format."));
        }

        // TODO: don't throw exception when barcode invalid format???
        // shows error when unable to find holding by barcode
        // shows error when holding is already checked out
        // on success:
        //    patron contains holding
        //    redirects back to Index
    }
}
