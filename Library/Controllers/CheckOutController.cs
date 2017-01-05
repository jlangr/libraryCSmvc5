using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Models;
using Library.Models.Repositories;

namespace Library.Controllers
{
    public class CheckOutController : Controller
    {
        BranchRepository branchRepo = new BranchRepository();
        HoldingRepository holdingRepo = new HoldingRepository();

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
            checkout.BranchesViewList = new List<Branch>(branchRepo.GetAllPhysical());
            if (!ModelState.IsValid)
                return View(checkout);

            // TODO create inmemory repo override that takes a function for criteria
            var holding = holdingRepo.FindByBarcode(checkout.Barcode);
            // TODO time source
            // TODO policy?
            holding.CheckOut(DateTime.Now, checkout.PatronId, new BookCheckoutPolicy());
            holdingRepo.MarkModified(holding);
            return RedirectToAction("Index");
        }
    }
}