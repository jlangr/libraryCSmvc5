using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class BranchesViewModel
    {
        public string SelectedBranch { get; set; }
        public IEnumerable<Branch> Branches { get; set; }
    }
}