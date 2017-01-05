using System.Diagnostics;
using System.Net;
using System.Web.Mvc;
using Library.Models;
using System.Collections.Generic;
using System;

namespace Library.Controllers
{
    public class HoldingsController : Controller
    {
        IRepository<Holding> repository = new EntityRepository<Holding>(db => db.Holdings);
        IRepository<Branch> branchRepo = new BranchRepository();

        // GET: Holdings
        public ActionResult Index()
        {
            var model = new List<HoldingViewModel>();
            foreach (var holding in repository.GetAll())
                model.Add(new HoldingViewModel(holding) { BranchName = branchRepo.GetByID(holding.BranchId).Name });
            return View(model);
        }

        // GET: Holdings/Details/5
        public ActionResult Details(int? id)
        {
            return Edit(id);
        }

        // GET: Holdings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Holding holding = repository.GetByID(id.Value);
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
            Debug.WriteLine("Creating holding " + holding.Id + " " + holding.Classification);
            if (ModelState.IsValid)
            {
                Debug.WriteLine("yes Create holding " + holding.Id + " " + holding.Classification);
                repository.Create(holding);
                return RedirectToAction("Index");
            }
            Debug.WriteLine("MODEL NOT VALID");
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
                repository.MarkModified(holding);
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
            repository.Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                repository.Dispose();
            base.Dispose(disposing);
        }
    }
}
