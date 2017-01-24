using System;
using Library.Util;
using Library.Models.Repositories;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web;

namespace Library.Models.ScanStation
{
    public class ScanStation
    {
        static HttpClient client = new HttpClient(); // Should only be instantiated once!
        const int NoPatron = -1;
        private readonly IClassificationService classificationService;
        private readonly HoldingService holdingService = new HoldingService();
        private readonly int brId;
        private int cur = NoPatron;
        private DateTime cts;
        private IRepository<Patron> patronRepo;

        public ScanStation(int branchId, IClassificationService classificationService)
        {
            this.classificationService = classificationService;
            client.BaseAddress = new Uri("http://localhost:50662");
            brId = branchId;
            patronRepo = new InMemoryRepository<Patron>();
        }

        public Holding AddNewMaterial(string isbn)
        {
            var classification = classificationService.Classification(isbn);
            return holdingService.Add(classification, brId);
        }

        public void AcceptLibraryCard(int patronId)
        {
            cur = patronId;
            cts = TimeService.Now;
        }

        //client.GetAsync(path).ContinueWith(responseMessage =>
        //    {
        //        Console.WriteLine("-response-");
        //        var response = responseMessage.Result;
        //        response.Content.ReadAsAsync<Holding>().ContinueWith(
        //            holdingTask =>
        //            {
        //                Console.WriteLine("-task retrieved-");
        //                Console.WriteLine("retrieved:" + holdingTask.Result.Barcode);
        //            });
        //    });

        private Holding Get(string path)
        {
            var response = client.GetAsync(path).Result;
            if (!response.IsSuccessStatusCode)
            {
                // Log error
                Console.WriteLine("Status: {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Holding>(json);
        }

        // 1/19/2017: who wrote this?
        // 
        // TODO. Fix this mess. We just have to SHIP IT for nwo!!!
        public void AcceptBarcode(string bc)
        {
            var cl = Holding.ClassificationFromBarcode(bc);
            var cn = Holding.CopyNumberFromBarcode(bc);
            var h = Get($"Holdings/Find?classification={cl}&copyNumber={cn}");
            Console.WriteLine("HOLDING => " + h.Barcode);

            //if (h.IsCheckedOut)
            //{
            //    if (cur == NoPatron)
            //    { // ci
            //        bc = h.Barcode;
            //        var patronId = holdingService.Retrieve(bc).HeldByPatronId;
            //        var cis = TimeService.Now;
            //        Material m = null;
            //        m = classificationService.Retrieve(h.Classification);
            //        var fine = m.CheckoutPolicy.FineAmount(h.CheckOutTimestamp.Value, cis);
            //        var p = patronRepo.GetByID(patronId);
            //        p.Fine(fine);
            //        holdingService.CheckIn(cis, bc, brId);
            //        // call check in controller (patronId, bc);
            //    }
            //    else // ck book already cked-out
            //    {
            //        if (h.HeldByPatronId != cur)
            //        {
            //            var bc1 = h.Barcode;
            //            var n = TimeService.Now;
            //            var t = TimeService.Now.AddDays(21);
            //            var f = classificationService.Retrieve(h.Classification).CheckoutPolicy.FineAmount(h.CheckOutTimestamp.Value, n.AddDays(21));
            //            var patron = patronRepo.GetByID(h.HeldByPatronId);
            //            patron.Fine(f);
            //            holdingService.CheckIn(n, bc1, brId);
            //            // co
            //            holdingService.CheckOut(cts, bc1, cur, CheckoutPolicies.BookCheckoutPolicy);
            //            // call check out controller(cur, bc1);
            //            t.AddDays(1);
            //            n = t;
            //            //patronService.CheckIn(patron.Id, bc);
            //        }
            //        else // not checking out book already cked out by other patron
            //        {
            //            // otherwise ignore, already checked out by this patron
            //        }
            //    }
            //}
            //else
            //{
            //    if (cur != NoPatron) // check in book
            //    {
            //        holdingService.CheckOut(cts, h.Barcode, cur, CheckoutPolicies.BookCheckoutPolicy);
            //        // call checkout controller(cur, h.Barcode);
            //    }
            //    else
            //        throw new CheckoutException();
            //}
        }

        public void CompleteCheckout()
        {
            cur = NoPatron;
        }
    }
}
