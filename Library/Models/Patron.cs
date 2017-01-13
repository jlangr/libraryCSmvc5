using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    [Serializable]
    public class Patron: Identifiable
    {
        public Patron()
        {
            HoldingIds = new List<int>();
        }

        public Patron(int id, string name)
            : this()
        {
            Name = name;
            Id = id;
        }

        [Required, StringLength(25)]
        public string Name { get; set; }
        public int Id { get; set; }
        public decimal Balance { get; set; }

        public void Fine(decimal amount)
        {
            Balance += amount;
        }

        public void CheckOut(int id)
        {
            HoldingIds.Add(id);
        }

        // TODO use a set instead?
        public IList<int> HoldingIds { get; set; }

        public void CheckIn(int holdingId)
        {
            HoldingIds.Remove(holdingId);
        }

        public void Remit(decimal amount)
        {
            Balance -= amount;
        }
    }
}
