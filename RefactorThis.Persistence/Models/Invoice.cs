using RefactorThis.Persistence.Repositories;
using System.Collections.Generic;

namespace RefactorThis.Persistence.Models
{
    public class Invoice
    {
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public List<Payment> Payments { get; set; }
    }
}