using System.Net;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Models;
using Library.Models.Repositories;

namespace Library.Controllers
{
    public class PatronsController : Controller
    {
        IRepository<Patron> repository;

        public PatronsController()
        {
            repository = new EntityRepository<Patron>(db => db.Patrons);
        }

        public PatronsController(IRepository<Patron> repository)
        {
            this.repository = repository;
        }

        // GET: Patrons
        public ActionResult Index()
        {
            return View(repository.GetAll());
        }

        // GET: Patrons/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patrons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Balance")] Patron patron)
        {
            if (ModelState.IsValid)
            {
                repository.Create(patron);
                return RedirectToAction("Index");
            }
            return View(patron);
        }

        // GET: Patrons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var patron = repository.GetByID(id.Value);
            if (patron == null)
                return HttpNotFound();
            return View(patron);
        }

        public ActionResult Holdings(int id)
        {
            var holdings = new List<Holding>();
            return View(holdings);
        }

        // GET: Patrons/Delete/5
        public ActionResult Delete(int? id)
        {
            return Edit(id);
        }

        // GET: Patrons/Details/5
        public ActionResult Details(int? id)
        {
            return Edit(id);
        }

        // POST: Patrons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Balance")] Patron patron)
        {
            if (ModelState.IsValid)
            {
                repository.Save(patron);
                return RedirectToAction("Index");
            }
            return View(patron);
        }

        // POST: Patrons/Delete/5
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
