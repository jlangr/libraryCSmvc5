using System.Collections.Generic;

namespace Library.Models
{
    public class PatronService
    {
        private static readonly IDictionary<int, Patron> Patrons = new Dictionary<int, Patron>();

        public int Add(string name)
        {
            var id = Patrons.Count + 1;
            var patron = new Patron(id, name);
            Patrons[id] = patron;
            return id;
        }

        public Patron Retrieve(int id)
        {
            if (!Patrons.ContainsKey(id))
                return null;
            return Patrons[id];
        }

        public static void DeleteAllPatrons()
        {
            Patrons.Clear();
        }

        public void CheckOut(int id, string barcode)
        {
            var patron = Retrieve(id);
            patron.CheckOut(barcode);
        }

        public void CheckIn(int id, string barcode)
        {
            var patron = Retrieve(id);
            patron.CheckIn(barcode);
        }

        public ICollection<Patron> RetrieveAll()
        {
            return Patrons.Values;
        }
    }
}
