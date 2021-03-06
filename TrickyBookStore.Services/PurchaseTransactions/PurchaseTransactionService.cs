﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TrickyBookStore.Models;
using TrickyBookStore.Services.Books;

namespace TrickyBookStore.Services.PurchaseTransactions
{
    internal class PurchaseTransactionService : IPurchaseTransactionService
    {
        IBookService BookService { get; }

        public PurchaseTransactionService(IBookService bookService)
        {
            BookService = bookService;
        }

        public IList<PurchaseTransaction> GetPurchaseTransactions(long customerId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            var transactionList = Store.PurchaseTransactions.Data
                .Where(purchaseTransaction =>
                    purchaseTransaction.CustomerId == customerId
                    && purchaseTransaction.CreatedDate >= fromDate
                    && purchaseTransaction.CreatedDate <= toDate)
                .ToList();

            var books = BookService
                .GetBooks(transactionList
                    .Select(tran => tran.BookId)
                .ToArray());
            
            transactionList = transactionList
                .Join(books,
                    tran => tran.BookId,
                    book => book.Id,
                    (tran, book) => 
                    {
                        tran.Book = book;
                        return tran;
                    }).ToList();
            return transactionList;
        }
    }
}
