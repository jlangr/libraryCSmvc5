using System.Linq;
using Library.Models;
using Library.Models.Repositories;
using NUnit.Framework;
using System.Collections.Generic;

namespace LibraryTests.LibraryTest.Models.Repositories
{
    // TODO: name? It's not really an extension. Can it be made into one?
    [TestFixture]
    public class BranchRepositoryExtensionsTest
    {
        [Test]
        public void PrependsCheckedOutBranchToListOfAllBranches()
        {
            var branchRepo = new InMemoryRepository<Branch>();
            branchRepo.Create(new Branch { Name = "A" });
            branchRepo.Create(new Branch { Name = "B" });

            var branches = BranchRepositoryExtensions.GetAll(branchRepo);

            Assert.That(branches.First().Name, Is.EqualTo(Branch.CheckedOutBranch.Name));
            Assert.That(branches.Skip(1).Select(b => b.Name), Is.EquivalentTo(new List<string> { "A", "B" }));
        }
    }
}
