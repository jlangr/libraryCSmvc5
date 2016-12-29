using System.Collections.Generic;
using NUnit.Framework;
using Library.Models;

namespace LibraryTest.Models
{
    [TestFixture]
    public class BranchServiceTest
    {
        private BranchService service;

        [SetUp]
        public void Initialize()
        {
            service = new BranchService();
            service.DeleteAll();
        }
        
        [Test]
        public void AddBranch()
        {
            Assert.That(service.Add("east"), Is.EqualTo(1));
            Assert.That(service.BranchNames(), Is.EqualTo(new List<string> {"east"}));
        }

        [Test]
        public void AddSecondBranchUsesUniqueId()
        {
            Assert.That(service.Add("east"), Is.EqualTo(1));
            Assert.That(new BranchService().Add("west"), Is.EqualTo(2));
            Assert.That(service.BranchNames(), Is.EqualTo(new List<string> { "east", "west" }));
        }

        [Test]
        public void PersistentStorage()
        {
            service.Add("east");
            var secondService = new BranchService();
            Assert.That(secondService.BranchNames(), Is.EqualTo(new List<string> {"east"}));
        }

        [Test]
        public void IdLookup()
        {
            var id = service.Add("east");
            Assert.That(service.BranchName(id), Is.EqualTo("east"));
        }
    }
}
