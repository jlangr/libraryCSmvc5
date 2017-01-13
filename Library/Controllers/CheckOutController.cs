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
        IRepository<Branch> branchRepo = new EntityRepository<Branch>(db => db.Branches);
        IRepository<Holding> holdingRepo = new EntityRepository<Holding>(db => db.Holdings);
        IRepository<Patron> patronRepo = new EntityRepository<Patron>(db => db.Patrons);

        public CheckOutController(IRepository<Branch> branchRepo, IRepository<Holding> holdingRepo, IRepository<Patron> patronRepo)
        {
            this.branchRepo = branchRepo;
            this.holdingRepo = holdingRepo;
            this.patronRepo = patronRepo;
        }

        // GET: CheckOut
        public ActionResult Index()
        {
            var model = new CheckOutViewModel { BranchesViewList = new List<Branch>(branchRepo.GetAll()) }; // TODO remove checked-out branch
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CheckOutViewModel checkout)
        {
            // TODO is this needed here?
            //if (!ModelState.IsValid)
            //    return View(checkout);

            checkout.BranchesViewList = new List<Branch>(branchRepo.GetAll()); // TODO remove checked-out branch

            var patron = patronRepo.GetByID(checkout.PatronId);
            if (patron == null)
            {
                ModelState.AddModelError("CheckOut", "Invalid patron ID.");
                return View(checkout);
            }

            if (!Holding.IsBarcodeValid(checkout.Barcode))
            {
                ModelState.AddModelError("CheckOut", "Invalid holding barcode format.");
                return View(checkout);
            }

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, checkout.Barcode);
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
            patron.CheckOut(holding.Id);
            patronRepo.Save(patron);
            return RedirectToAction("Index");
        }
    }
}