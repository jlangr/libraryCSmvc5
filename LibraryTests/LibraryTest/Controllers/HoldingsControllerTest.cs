﻿using System.Web.Mvc;
using NUnit.Framework;
using Library.Controllers;
using Library.Extensions.SystemWebMvcController;
using Library.Models;
using Library.Models.Repositories;

namespace LibraryTests.LibraryTest.Controllers
{
    [TestFixture]
    public class HoldingsControllerTest
    {
        private HoldingsController controller;
        private IRepository<Holding> holdingRepo;
        private IRepository<Branch> branchRepo;

        [SetUp]
        public void Initialize()
        {
            holdingRepo = new InMemoryRepository<Holding>();
            branchRepo = new InMemoryRepository<Branch>();
            controller = new HoldingsController(holdingRepo, branchRepo);
        }

        [Test]
        public void CreatePersistsHolding()
        {
            controller.Create(new Holding() { Classification = "AB123", CopyNumber = 1 });

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, "AB123:1");
            Assert.That(holding.Barcode, Is.EqualTo("AB123:1"));
        }

        [Test]
        public void CreateReturnsGeneratedId()
        {
            var result = controller.Create(new Holding() { Classification = "AB123", CopyNumber = 1 }) as RedirectToRouteResult;

            var id = (int)result.RouteValues["ID"];
            Assert.That(holdingRepo.GetByID(id).Barcode, Is.EqualTo("AB123:1"));
        }

        [Test]
        public void CreateAssignsCopyNumberWhenNotProvided()
        {
            controller.Create(new Holding() { Classification = "AB123", CopyNumber = 0 });

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, "AB123:1");
            Assert.That(holding.Barcode, Is.EqualTo("AB123:1"));
        }

        [Test]
        public void CreateUsesHighwaterCopyNumberWhenAssigning()
        {
            controller.Create(new Holding() { Classification = "AB123", CopyNumber = 1, HeldByPatronId = 1});

            controller.Create(new Holding() { Classification = "AB123", CopyNumber = 0, HeldByPatronId = 2 });

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, "AB123:2");
            Assert.That(holding.HeldByPatronId, Is.EqualTo(2));
        }

        [Test]
        public void CreateUsesHighwaterOnlyForBooksWithSameClassification()
        {
            controller.Create(new Holding() { Classification = "AB123", CopyNumber = 1, HeldByPatronId = 1});

            controller.Create(new Holding() { Classification = "XX999", CopyNumber = 0, HeldByPatronId = 2 });

            var holding = HoldingRepositoryExtensions.FindByBarcode(holdingRepo, "XX999:1");
            Assert.That(holding.HeldByPatronId, Is.EqualTo(2));
        }

        [Test]
        public void CreateErrorsWhenAddingDuplicateBarcode()
        {
            controller.Create(new Holding() { Classification = "AB123", CopyNumber = 1 });

            var result = controller.Create(new Holding() { Classification = "AB123", CopyNumber = 1 }) as ViewResult;

            Assert.That(controller.SoleErrorMessage(HoldingsController.ModelKey), Is.EqualTo("Duplicate classification / copy number combination."));
        }
    }
}