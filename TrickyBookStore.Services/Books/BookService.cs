using System.Collections.Generic;
using System.Linq;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Books
{
    internal class BookService : IBookService
    {
        public IList<Book> GetBooks(params long[] ids)
        {
            return Store.Books.Data.Where(book => ids.Contains(book.Id)).ToList();
        }
    }
}
