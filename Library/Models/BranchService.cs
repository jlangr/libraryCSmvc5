using System;
using System.Collections.Generic;

namespace Library.Models
{
    public class BranchService
    {
        static readonly IList<string> BranchNamesList = new List<string>();
        int nextId = BranchNamesList.Count + 1;

        public int Add(string name)
        {
            BranchNamesList.Add(name);
            return nextId++;
        }

        public IList<string> BranchNames()
        {
            return BranchNamesList;
        }

        public void DeleteAll()
        {
            BranchNamesList.Clear();
            nextId = 1;
        }

        public string BranchName(int id)
        {
            return BranchNamesList[id - 1];
        }
    }
}
