using System.Collections.Generic;

namespace Library.Models
{
    public class MasterClassificationService: IClassificationService
    {
        private static readonly IDictionary<string, Material> Materials = new Dictionary<string, Material>();

        public void DeleteAllBooks()
        {
            Materials.Clear();
        }

        public virtual string Classification(string isbn)
        {
            return ""; // would require external lookup
        }

        public void AddMovie(string classification, string title, string director, string year)
        {
            var material = new Material(classification, title, director, year, CheckoutPolicies.MovieCheckoutPolicy);
            Materials[classification] = material;
        }

        public void AddBook(string classification, string title, string author, string year)
        {
            var material = new Material(classification, title, author, year);
            Materials[classification] = material;
        }

        public Material Retrieve(string classification)
        {
            if (!Materials.ContainsKey(classification))
                return null;
            return Materials[classification];
        }
    }
}
