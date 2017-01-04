using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    [Serializable]
    public class Holding: Identifiable
    {
        public const int NoPatron = -1;
        public const int CheckedOutBranchId = -1;

        public Holding() { }

        public Holding(string classification, int copyNumber) 
            : this(classification, copyNumber, CheckedOutBranchId)
        {
        }

        public Holding(string classification, int copyNumber, int branchId)
        {
            CheckOutTimestamp = DateTime.MinValue;
            LastCheckedIn = DateTime.MinValue;
            DueDate = DateTime.MinValue;
            Classification = classification;
            CopyNumber = copyNumber;
            BranchId = branchId;
        }

        public int Id { get; set; }
        public string Classification { get; set; }
        public int CopyNumber { get; set; }
        [NotMapped]
        public CheckoutPolicy CheckoutPolicy { get; set; }
        public DateTime CheckOutTimestamp { get; set; }
        public int BranchId { get; set; }
        public int HeldByPatronId { get; set; }
        public DateTime LastCheckedIn { get; set; }

        [NotMapped]
        public string Barcode
        {
            get
            {
                return GenerateBarcode(Classification, CopyNumber);
            }
        }

        [NotMapped]
        public DateTime DueDate { get; set; }

        [NotMapped]
        public bool IsCheckedOut
        {
            get
            {
                return CheckOutTimestamp != DateTime.MinValue;
            }
        }

        public void CheckIn(DateTime timestamp, int toBranchId)
        {
            LastCheckedIn = timestamp;
            CheckOutTimestamp = DateTime.MinValue;
            HeldByPatronId = NoPatron;
            BranchId = toBranchId;
        }

        public void CheckOut(DateTime timestamp, int patronId, CheckoutPolicy checkoutPolicy)
        {
            CheckOutTimestamp = timestamp;
            HeldByPatronId = patronId;
            CheckoutPolicy = checkoutPolicy;
            CalculateDueDate();
        }

        private void CalculateDueDate()
        {
            DueDate = CheckOutTimestamp.AddDays(CheckoutPolicy.MaximumCheckoutDays());
        }

        public static string GenerateBarcode(string classification, int copyNumber)
        {
            return string.Format("{0}:{1}", classification, copyNumber);
        }

        public static string ClassificationFromBarcode(string barcode)
        {
            var colonIndex = barcode.IndexOf(':');
            return barcode.Substring(0, colonIndex);
        }
    }
}
