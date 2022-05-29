using RefactorThis.Persistence.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence.Models
{
    public class Invoice
    {
        public Invoice()
        {
            Payments = new List<Payment>();
        }

        public Invoice(List<Payment> initialPayments)
        {
            Payments = initialPayments;
        }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        protected List<Payment> Payments { get; set; }
        public decimal AmountPaid => Payments?.Sum(x => x.Amount) ?? 0;
        public bool IsFullyPaid => Amount == AmountPaid;
        public decimal UnpaidAmount => Amount - AmountPaid;
        public bool HavePayments => Payments?.Any() ?? false;

        public void AddPayment(Payment payment)
        {
            Payments.Add(payment);
        }
    }
}