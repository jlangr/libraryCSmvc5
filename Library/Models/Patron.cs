using System.Collections.Generic;

namespace Library.Models
{
    public class Patron
    {
        public Patron(int id, string name)
        {
            Holdings = new List<string>();
            Name = name;
            Id = id;
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
        public decimal Balance { get; private set; }

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
