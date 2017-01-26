using System.Collections.Generic;
using System.Linq;

namespace Library.Models.Repositories
{
    public class BranchRepositoryExtensions
    {
        static public IEnumerable<Branch> GetAll(IRepository<Branch> repo)
        {
            return new List<Branch> { Branch.CheckedOutBranch }.Concat(repo.GetAll());
        }
    }
}