using System.Net;
using System.Web.Mvc;
using Library.Models;
using Library.Models.Repositories;
using System.Collections.Generic;
using System;

namespace Library.Controllers
{
    public class HoldingsController : Controller
    {
        IRepository<Holding> holdingRepo;
        IRepository<Branch> branchRepo;

        public HoldingsController()
        {
            holdingRepo = new EntityRepository<Holding>(db => db.Holdings);
            branchRepo = new EntityRepository<Branch>(db => db.Branches);
        }

        // GET: Holdings
        // TODO test around alternate branch name. Refactor?
        public ActionResult Index()
        {
            var model = new List<HoldingViewModel>();
            foreach (var holding in holdingRepo.GetAll())
                model.Add(new HoldingViewModel(holding) { BranchName = BranchName(holding.BranchId) });
            return View(model);
        }

        // TODO: Where might this go
        string BranchName(int branchId)
        {
            if (branchId == Branch.CheckedOutId)
                return "** checked out **";
            return branchRepo.GetByID(branchId).Name;
        }

        // GET: Holdings/Details/5
        public ActionResult Details(int? id)
        {
            return Edit(id);
        }

        [HttpGet]
        public JsonResult Find(string classification, int copyNumber)
        {
            var barcode = Holding.GenerateBarcode(classification, copyNumber);
            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, barcode);
            if (holding == null)
                return Json(HttpNotFound());
            return Json(holding, JsonRequestBehavior.AllowGet);
        }

        // GET: Holdings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var holding = holdingRepo.GetByID(id.Value);
            if (holding == null)
                return HttpNotFound();
            return ViewWithBranches(holding);
        }

        private ActionResult ViewWithBranches(Holding holding)
        {
            return View(new HoldingViewModel(holding) { BranchesViewList = new List<Branch>(branchRepo.GetAll()) });
        }

        // GET: Holdings/Create
        public ActionResult Create()
        {
            return View(new HoldingViewModel { BranchesViewList = new List<Branch>(branchRepo.GetAll()) });
        }

        // POST: Holdings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Classification,CopyNumber,CheckOutTimestamp,BranchId,HeldByPatronId,LastCheckedIn")] Holding holding)
        {
            if (ModelState.IsValid)
            {
                holdingRepo.Create(holding);
                return RedirectToAction("Index");
            }
            return View(holding);
        }

        // POST: Holdings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Classification,CopyNumber,CheckOutTimestamp,BranchId,HeldByPatronId,LastCheckedIn")] Holding holding)
        {
            if (ModelState.IsValid)
            {
                holdingRepo.Save(holding);
                return RedirectToAction("Index");
            }
            return View(holding);
        }

        // GET: Holdings/Delete/5
        public ActionResult Delete(int? id)
        {
            return Edit(id);
        }

        // POST: Holdings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            holdingRepo.Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                holdingRepo.Dispose();
            base.Dispose(disposing);
        }
    }
}
