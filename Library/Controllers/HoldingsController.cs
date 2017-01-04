using System.Net;
using System.Web.Mvc;
using Library.Models;
using System.Collections.Generic;

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
            {
                //var h = new HoldingViewModel(holding);
                //h.BranchName = branchRepo.GetByID(h.BranchId).Name;
                model.Add(new HoldingViewModel(holding) { BranchName = branchRepo.GetByID(holding.BranchId).Name });
            }
            return View(model);
        }

        // GET: Holdings/Details/5
        public ActionResult Details(int? id)
        {
            return Edit(id);
        }

        // GET: Holdings/Create
        public ActionResult Create()
        {
            var holdingVM = new HoldingViewModel();
            holdingVM.BranchesViewList = new List<Branch>(branchRepo.GetAll());
            return View(holdingVM);
        }

        // POST: Holdings/Create
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
            var viewModel = new HoldingViewModel(holding);
            var branches = branchRepo.GetAll();
            viewModel.BranchesViewList = new List<Branch>(branches);
            return View(viewModel);
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
