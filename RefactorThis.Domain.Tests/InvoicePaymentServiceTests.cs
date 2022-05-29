using NUnit.Framework;
using RefactorThis.Persistence.Models;
using RefactorThis.Persistence.Repositories;
using System;
using System.Collections.Generic;
using RefactorThis.Domain.Resources;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
	public class InvoicePaymentServiceTests
	{
		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
		{
			var repo = new InvoiceRepository();
            var paymentProcessor = new InvoicePaymentService(repo);

			var payment = new Payment();
			var failureMessage = "";

			try
			{
				var result = paymentProcessor.ProcessPayment("012345", payment);
			}
			catch ( InvalidOperationException e )
			{
				failureMessage = e.Message;
			}

			Assert.AreEqual(ErrorMessages.No_Invoice_Matching, failureMessage);
		}

        [Test]
        public void ProcessPayment_Should_ThrowException_When_InvoiceAmountIsZero_WithPayments()
        {
            var repo = new InvoiceRepository();
            repo.Add(new Invoice(new List<Payment>
            {
                new Payment
                {
                    Amount = 10
                }
            })
            {
				Amount = 0,
				Reference = "012345"
            });
            var paymentProcessor = new InvoicePaymentService(repo);

            var payment = new Payment
            {
				Amount = 10
            };
            var failureMessage = "";

            try
            {
                var result = paymentProcessor.ProcessPayment("012345", payment);
            }
            catch (InvalidOperationException e)
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual(ErrorMessages.Invoice_Invalid, failureMessage);
        }


		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
		{
			var repo = new InvoiceRepository();

			var invoice = new Invoice(null)
			{
				Amount = 0,
                Reference = "012345"
			};

			repo.Add(invoice);

			var paymentProcessor = new InvoicePaymentService(repo);

			var payment = new Payment();
            var result = paymentProcessor.ProcessPayment("012345", payment);

			Assert.AreEqual(Responses.Invoice_NoPayment_Needed, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
		{
			var repo = new InvoiceRepository();

			var invoice = new Invoice(new List<Payment>
            {
                new Payment
                {
                    Amount = 10
                }
            })
			{
				Amount = 10,
				Reference = "012345"
			};
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentService(repo);
            var payment = new Payment();

			var result = paymentProcessor.ProcessPayment("012345", payment);
            Assert.AreEqual(Responses.Invoice_Already_FullyPaid, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
		{
			var repo = new InvoiceRepository();
			var invoice = new Invoice(new List<Payment>
            {
                new Payment
                {
                    Amount = 5
                }
            })
			{
				Amount = 10,
				Reference = "012345"
			};
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentService(repo);
            var payment = new Payment
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment("012345", payment);
            Assert.AreEqual(Responses.Invoice_Payment_Greater_Than_Amount_Remaining, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
		{
			var repo = new InvoiceRepository();
			var invoice = new Invoice
			{
				Amount = 5,
				Reference = "012345"
			};
			repo.Add(invoice);

			var paymentProcessor = new InvoicePaymentService(repo);

			var payment = new Payment
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment("012345", payment);

			Assert.AreEqual(Responses.Invoice_Payment_Greater_Than_Invoice_Amount, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
		{
			var repo = new InvoiceRepository();
			var invoice = new Invoice(new List<Payment>
            {
                new Payment
                {
                    Amount = 5
                }
            })
			{
				Amount = 10,
				Reference = "012345"
			};
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentService(repo);
            var payment = new Payment
			{
				Amount = 5
			};

			var result = paymentProcessor.ProcessPayment("012345", payment);
            Assert.AreEqual(Responses.Invoice_Final_Payment_Received, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
		{
			var repo = new InvoiceRepository();
			var invoice = new Invoice(new List<Payment>
            {
                new Payment
                {
                    Amount = 10
                }
            })
			{
				Amount = 10,
				Reference = "012345"
			};
			repo.Add(invoice);

			var paymentProcessor = new InvoicePaymentService(repo);
            var payment = new Payment
			{
				Amount = 10
			};

			var result = paymentProcessor.ProcessPayment("012345", payment);
            Assert.AreEqual(Responses.Invoice_Already_FullyPaid, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice(new List<Payment>
            {
                new Payment
                {
                    Amount = 5
                }
            })
			{
				Amount = 10,
				Reference = "012345"
			};
			repo.Add(invoice);

			var paymentProcessor = new InvoicePaymentService(repo);
            var payment = new Payment
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment("012345", payment);

			Assert.AreEqual(Responses.Invoice_Partial_Payment_Received, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
		{
			var repo = new InvoiceRepository();
			var invoice = new Invoice
			{
				Amount = 10,
				Reference = "012345"
			};
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentService(repo);

			var payment = new Payment
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment("012345", payment);
            Assert.AreEqual(Responses.Invoice_Partially_Paid, result );
		}
	}
}