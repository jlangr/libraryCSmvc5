﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class BranchRepository : EntityRepository<Branch>
    {
        public BranchRepository() : base(db => db.Branches) { }

        // TODO test
        override public Branch GetByID(int id)
        {
            if (id == 0) return Branch.CheckedOutBranch;
            return base.GetByID(id);
        }

        // TODO test
        override public IEnumerable<Branch> GetAll()
        {
            return new List<Branch> { Branch.CheckedOutBranch }.Concat(base.GetAll());
        }
    }
}