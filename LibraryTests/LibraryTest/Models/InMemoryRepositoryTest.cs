using Library.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTests.LibraryTest.Models
{
    [TestFixture]
    public class InMemoryRepositoryTest
    {
        private X x;
        private InMemoryRepository<X> repo;

        [Serializable] class X : Identifiable { public int Id { get; set;  } };
        
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
            int id = repo.Create(x);

            X retrieved = repo.GetByID(id);

            Assert.That(retrieved, Is.Not.SameAs(x));
        }
    }
}
