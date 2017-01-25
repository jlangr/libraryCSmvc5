using System;

namespace Library.Models
{
    public class Material
    {
        // TODO clean out ctors
        public Material() { }

        public Material(string classification, string title, string author, string year)
            : this(classification, title, author, year, CheckoutPolicies.BookCheckoutPolicy)
        {
        }

        public Material(string classification, string title, string author, string year, CheckoutPolicy checkoutPolicy)
        {
            Classification = classification;
            Title = title;
            Year = year;
            Author = author;
            CheckoutPolicy = checkoutPolicy;
        }

        public string Director
        {
            get
            {
                return Author;
            }
        }

        public CheckoutPolicy CheckoutPolicy { get; set; }
        public string Title { get; private set; }
        public string Classification { get; private set; }
        public string Author { get; private set; }
        public string Year { get; private set; }
    }
}
