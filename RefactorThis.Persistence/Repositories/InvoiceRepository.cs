using System;
using System.Collections.Generic;
using System.Linq;
using RefactorThis.Persistence.Interfaces;
using RefactorThis.Persistence.Models;

namespace RefactorThis.Persistence.Repositories
{
    public class InvoiceRepository : IRepository<Invoice>
    {
        private List<Invoice> _invoices;

        public Invoice GetInvoice(string reference)
        {
            return _invoices?.First(x => x.Reference.Equals(reference, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Save(Invoice invoice)
        {
            //saves the invoice to the database
        }

        public void Add(Invoice invoice)
        {
            if (_invoices == null)
                _invoices = new List<Invoice>();
            
            _invoices.Add(invoice);
        }
    }
}