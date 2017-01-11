using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Models;
using Library.Models.Repositories;
using Library.Util;

namespace Library.Controllers
{
    public class CheckOutController : Controller
    {
        BranchRepository branchRepo = new BranchRepository();
        HoldingRepository holdingRepo = new HoldingRepository();
        IRepository<Patron> patronRepo = new EntityRepository<Patron>(db => db.Patrons);

        public CheckOutController(HoldingRepository holdingRepo, IRepository<Patron> patronRepo)
        {
            this.holdingRepo = holdingRepo;
            this.patronRepo = patronRepo;
        }

        // GET: CheckOut
        public ActionResult Index()
        {
            var model = new CheckOutViewModel { BranchesViewList = new List<Branch>(branchRepo.GetAllPhysical()) };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CheckOutViewModel checkout)
        {
            // TODO is this needed here?
            //if (!ModelState.IsValid)
            //    return View(checkout);

            checkout.BranchesViewList = new List<Branch>(branchRepo.GetAllPhysical());

            if (patronRepo.GetByID(checkout.PatronId) == null)
            {
                ModelState.AddModelError("CheckOut", "Invalid patron ID.");
                return View(checkout);
            }

            if (!Holding.IsBarcodeValid(checkout.Barcode))
            {
                ModelState.AddModelError("CheckOut", "Invalid holding barcode format.");
                return View(checkout);
            }

            var holding = holdingRepo.FindByBarcode(checkout.Barcode);
            if (holding == null)
            {
                ModelState.AddModelError("CheckOut", "Invalid holding barcode.");
                return View(checkout);
            }
            if (holding.IsCheckedOut)
            {
                ModelState.AddModelError("CheckOut", "Holding is already checked out.");
                return View(checkout);
            }

            // TODO policy?
            holding.CheckOut(TimeService.Now, checkout.PatronId, new BookCheckoutPolicy());
            holdingRepo.Save(holding);
            return RedirectToAction("Index");
        }
    }
}