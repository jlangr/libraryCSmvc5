using System.Collections.Generic;
using System.Linq;

namespace Library.Models.Repositories
{
    public class BranchRepositoryExtensions
    {
        // TODO test
        static public Branch GetByID(IRepository<Branch> repo, int id)
        {
            if (id == 0) return Branch.CheckedOutBranch;
            return repo.GetByID(id);
        }

        // TODO test
        static public IEnumerable<Branch> GetAll(IRepository<Branch> repo)
        {
            return new List<Branch> { Branch.CheckedOutBranch }.Concat(repo.GetAll());
        }

        // TODO test
        static public IEnumerable<Branch> GetAllPhysical(IRepository<Branch> repo)
        {
            return repo.GetAll();
        }
    }
}