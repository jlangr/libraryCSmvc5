using System;
using NUnit.Framework;
using Library.Models;
using Library.Models.Repositories;

namespace LibraryTests.LibraryTest.Models
{
    [TestFixture]
    public class InMemoryRepositoryTest
    {
        private X x;
        private InMemoryRepository<X> repo;

        [Serializable] class X : Identifiable {
            public int Id { get; set; }
            public string Name { get; set; }
        };
        
        [SetUp]
        public void Initialize()
        {
            x = new X();
            repo = new InMemoryRepository<X>();
        }

        [Test]
        public void InitialIdIs1()
        {
            var id = repo.Create(x);

            Assert.That(id, Is.EqualTo(1));
        }

        [Test]
        public void IdIncrementsOnCreate()
        {
            repo.Create(x);

            int id = repo.Create(x);

            Assert.That(id, Is.EqualTo(2));
        }

        [Test]
        public void RetrievedInstanceNotSameAsCreated()
        {
            var id = repo.Create(x);

            X retrieved = repo.GetByID(id);

            Assert.That(retrieved, Is.Not.SameAs(x));
        }

        [Test]
        public void FindsUsingLambda()
        {
            X alpha = new X { Name = "alpha" };
            X beta = new X { Name = "beta" };
            repo.Create(alpha);
            int betaId = repo.Create(beta);

            X retrieved = repo.Get(x => x.Name == "beta");

            Assert.That(retrieved.Id, Is.EqualTo(betaId));
        }


        [Test]
        public void ClearsRepo()
        {
            repo.Create(x);

            repo.Clear();

            Assert.That(repo.GetAll(), Is.Empty);
        }
    }
}
