using System.Net;
using System.Web.Mvc;
using Library.Models;
using System.Collections.Generic;

namespace Library.Controllers
{
    public class HoldingsController : Controller
    {
        IRepository<Holding> repository = new EntityRepository<Holding>(db => db.Holdings);

        // GET: Holdings
        public ActionResult Index()
        {
            return View(repository.GetAll());
        }

        // GET: Holdings/Details/5
        public ActionResult Details(int? id)
        {
            return Edit(id);
        }

        // GET: Holdings/Create
        public ActionResult Create()
        {
            Holding holding = new Models.Holding();
            List<Branch> branches = new List<Branch>
            {
                new Models.Branch() { Id = 1, Name = "Hey" },
                new Models.Branch() { Id = 2, Name = "Toes" }
            };

            holding.BranchesViewList = branches;
            return View(holding);
        }

        // POST: Holdings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Classification,CopyNumber,CheckOutTimestamp,BranchId,HeldByPatronId,LastCheckedIn")] Holding holding)
        {
            if (ModelState.IsValid)
            {
                repository.Create(holding);
                return RedirectToAction("Index");
            }

            return View(holding);
        }

        // GET: Holdings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Holding holding = repository.GetByID(id.Value);
            if (holding == null)
                return HttpNotFound();
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
