using System.Collections.Generic;
using System.Web.Mvc;
using Library.Models;
using Library.Models.Repositories;
using Library.Util;

namespace Library.Controllers
{
    public class CheckOutController : Controller
    {
        public const string ModelKey = "CheckOut";
        IRepository<Branch> branchRepo;
        IRepository<Holding> holdingRepo;
        IRepository<Patron> patronRepo;

        public CheckOutController()
        {
            branchRepo = new EntityRepository<Branch>(db => db.Branches);
            holdingRepo = new EntityRepository<Holding>(db => db.Holdings);
            patronRepo = new EntityRepository<Patron>(db => db.Patrons);
        }

        public CheckOutController(IRepository<Branch> branchRepo, IRepository<Holding> holdingRepo, IRepository<Patron> patronRepo)
        {
            this.branchRepo = branchRepo;
            this.holdingRepo = holdingRepo;
            this.patronRepo = patronRepo;
        }

        // GET: CheckOut
        public ActionResult Index()
        {
            var model = new CheckOutViewModel { BranchesViewList = new List<Branch>(BranchRepositoryExtensions.GetAll(branchRepo)) };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CheckOutViewModel checkout)
        {
            if (!ModelState.IsValid)
                return View(checkout);

            checkout.BranchesViewList = new List<Branch>(BranchRepositoryExtensions.GetAll(branchRepo)); 

            var patron = patronRepo.GetByID(checkout.PatronId);
            if (patron == null)
            {
                ModelState.AddModelError(ModelKey, "Invalid patron ID.");
                return View(checkout);
            }

            if (!Holding.IsBarcodeValid(checkout.Barcode))
            {
                ModelState.AddModelError(ModelKey, "Invalid holding barcode format.");
                return View(checkout);
            }

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, checkout.Barcode);
            if (holding == null)
            {
                ModelState.AddModelError(ModelKey, "Invalid holding barcode.");
                return View(checkout);
            }
            if (holding.IsCheckedOut)
            {
                ModelState.AddModelError(ModelKey, "Holding is already checked out.");
                return View(checkout);
            }

            // TODO policy?
            holding.CheckOut(TimeService.Now, checkout.PatronId, new BookCheckoutPolicy());
            holdingRepo.Save(holding);

            return RedirectToAction("Index");
        }
    }
}