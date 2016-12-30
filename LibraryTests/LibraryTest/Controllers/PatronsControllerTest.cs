using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Library.Controllers;
using Library.Models;
using System.Web.Mvc;
using System.Net;

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
                int id = repo.Create(new Patron() { Name = "Jeff" }); 

                var view = controller.Details(id);

                var viewPatron = (view as ViewResult).Model as Patron;
                Assert.That(viewPatron.Name, Is.EqualTo("Jeff"));
            }
        }
        
        public class Index: PatronsControllerTest
        {
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
    }
}
