using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Library.Models
{
    [Serializable]
    public class Holding: Identifiable
    {
        public const int NoPatron = -1;

        public Holding()
            : this("", 1, Branch.CheckedOutId)
        {
        }

        public Holding(string classification, int copyNumber) 
            : this(classification, copyNumber, Branch.CheckedOutId)
        {
        }

        public Holding(string classification, int copyNumber, int branchId)
        {
            CheckOutTimestamp = null;
            LastCheckedIn = null;
            DueDate = null;
            Classification = classification;
            CopyNumber = copyNumber;
            BranchId = branchId;
        }

        public int Id { get; set; }
        public string Classification { get; set; }
        public int CopyNumber { get; set; }
        [NotMapped]
        public CheckoutPolicy CheckoutPolicy { get; set; }
        public DateTime? CheckOutTimestamp { get; set; }
        public DateTime? LastCheckedIn { get; set; }
        public DateTime? DueDate { get; set; }
        public int BranchId { get; set; }
        public int HeldByPatronId { get; set; }

        [NotMapped]
        public string Barcode {
            get { return GenerateBarcode(Classification, CopyNumber); }
        }

        [NotMapped]
        public bool IsCheckedOut
        {
            get { return CheckOutTimestamp != null; }
        }

        public void CheckIn(DateTime timestamp, int toBranchId)
        {
            LastCheckedIn = timestamp;
            CheckOutTimestamp = null;
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
            DueDate = CheckOutTimestamp.Value.AddDays(CheckoutPolicy.MaximumCheckoutDays());
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
