using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    [Serializable]
    public class Patron: Identifiable
    {
        public Patron() { }

        public Patron(int id, string name)
        {
            Holdings = new List<string>();
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

        public void CheckOut(string barcode)
        {
            Holdings.Add(barcode);
        }

        public IList<string> Holdings { get; private set; }

        public void CheckIn(string barcode)
        {
            Holdings.Remove(barcode);
        }

        public void Remit(decimal amount)
        {
            Balance -= amount;
        }
    }
}
