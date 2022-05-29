using RefactorThis.Persistence.Models;
using RefactorThis.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using RefactorThis.Domain.Resources;

namespace RefactorThis.Domain
{
    public class InvoicePaymentService
	{
		private readonly InvoiceRepository _invoiceRepository;

		public InvoicePaymentService(InvoiceRepository invoiceRepository)
		{
			_invoiceRepository = invoiceRepository;
		}

		public string ProcessPayment(string invoiceReference, Payment payment)
		{
			var invoice = _invoiceRepository.GetInvoice(invoiceReference);

            if (invoice == null) throw new InvalidOperationException(ErrorMessages.No_Invoice_Matching);

            var invoiceHavePayments = invoice.HavePayments;
            if (invoice.Amount <= 0)
            {
                if (!invoiceHavePayments)
                {
                    return Responses.Invoice_NoPayment_Needed; 
                }

                throw new InvalidOperationException(ErrorMessages.Invoice_Invalid);
            }

            if (invoice.IsFullyPaid) return Responses.Invoice_Already_FullyPaid;
            
            if (payment.Amount > invoice.UnpaidAmount)
            {
                return invoiceHavePayments ? Responses.Invoice_Payment_Greater_Than_Amount_Remaining : Responses.Invoice_Payment_Greater_Than_Invoice_Amount;
            }

            
            invoice.AddPayment(payment);
            _invoiceRepository.Save(invoice);

            return invoice.IsFullyPaid
                ? (invoiceHavePayments ? Responses.Invoice_Final_Payment_Received : Responses.Invoice_FullyPaid)
                : (invoiceHavePayments ? Responses.Invoice_Partial_Payment_Received : Responses.Invoice_Partially_Paid);
        }
	}
}