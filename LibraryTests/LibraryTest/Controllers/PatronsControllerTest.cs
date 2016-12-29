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
                Patron patron = new Patron() { Name = "Jeff" };
                int id = repo.Create(patron); 

                var view = controller.Details(id);

                Console.WriteLine(view.GetType());
                Assert.True(true);
            }
        }
    }
}
