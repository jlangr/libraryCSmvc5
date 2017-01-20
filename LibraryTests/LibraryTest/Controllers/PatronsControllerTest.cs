using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using Library.Controllers;
using Library.Models;
using Library.Models.Repositories;

namespace LibraryTests.LibraryTest.Controllers
{
    [TestFixture]
    public class PatronsControllerTest
    {
        private PatronsController controller;
        private InMemoryRepository<Patron> repo;

        [SetUp]
        public void Initialize()
        {
            repo = new InMemoryRepository<Patron>();
            controller = new PatronsController(repo);
        }

        public class Details: PatronsControllerTest
        {
            [Test]
            public void ReturnsNotFoundWhenNoPatronAdded()
            {
                var view = controller.Details(0);

                Assert.That(view, Is.TypeOf<HttpNotFoundResult>());
            }

            [Test]
            public void ReturnsBadRequestErrorWhenIdNull()
            {
                var view = controller.Details(null);

                Assert.That((view as HttpStatusCodeResult).StatusCode, Is.EqualTo(400));
            }

            [Test]
            public void ReturnsViewOnPatronWhenFound()
            {
                var id = repo.Create(new Patron() { Name = "Jeff" }); 

                var view = controller.Details(id);

                var viewPatron = (view as ViewResult).Model as Patron;
                Assert.That(viewPatron.Name, Is.EqualTo("Jeff"));
            }
        }

        public class Holdings: PatronsControllerTest
        {
            [Test]
            public void ReturnsEmptyWhenPatronHasNotCheckedOutBooks()
            {
                var id = repo.Create(new Patron());

                var view = (controller.Holdings(id) as ViewResult).Model as IEnumerable<Holding>;

                Assert.That(!view.Any());
            }
        }
        
        public class Index: PatronsControllerTest
        {
            private InMemoryRepository<Holding> holdingRepo;

            [SetUp]
            public void Initialize()
            {
                holdingRepo = new InMemoryRepository<Holding>();
            }

            [Test]
            public void RetrievesViewOnAllPatrons()
            {
                repo.Create(new Patron { Name = "Alpha" }); 
                repo.Create(new Patron { Name = "Beta" }); 

                var view = controller.Index();

                var patrons = (view as ViewResult).Model as IEnumerable<Patron>;
                Assert.That(patrons.Select(p => p.Name), 
                    Is.EqualTo(new string[] { "Alpha", "Beta" }));
            }
        }

        public class Create: PatronsControllerTest
        {
            [Test]
            public void CreatesPatronWhenModelStateValid()
            {
                var patron = new Patron { Name = "Venkat" };

                controller.Create(patron);

                var retrieved = repo.GetAll().First();
                Assert.That(retrieved.Name, Is.EqualTo("Venkat"));
            }

            [Test]
            public void RedirectsToIndexWhenModelValid()
            {
                var result = controller.Create(new Patron()) as RedirectToRouteResult;

                Assert.That(result.RouteValues["action"], Is.EqualTo("Index"));
            }

            [Test]
            public void AddsNoPatronWhenModelStateInvalid()
            {
                controller.ModelState.AddModelError("", "");

                controller.Create(new Patron());

                Assert.False(repo.GetAll().Any());
            }
        }
    }
}
