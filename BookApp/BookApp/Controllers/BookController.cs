using BookApp.Enums;
using BookApp.Helper;
using Interfaces.Repositories;
using Interfaces.Services;
using Models.DomainModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BookApp.Controllers
{
    [RoutePrefix("api/books")]
    [EnableCors(origins: "*", headers: "accept,Auth-Key", methods: "*")]
    public class BookController : ApiController
    {
        private IBookRepository BookRepository;
        private IBookService BookService;

        public BookController()
        {
        }
        public BookController(IBookRepository bookRepository, IBookService bookService)
        {
            BookRepository = bookRepository;
            BookService = bookService;
        }

        [HttpGet]
        [Route("GetBookById")]
        public HttpResponseMessage GetBookById(Guid bookId)
        {
            if (bookId == null || bookId == Guid.Empty)
                throw new APIException()
                {
                    ErrorDescription = "Bad Request. Provide valid bookId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            var book = BookService.GetBookById(bookId);
            if (book != null)
                return Request.CreateResponse(HttpStatusCode.OK, book, JsonFormatter);
            else
                throw new APIDataException((int)BookErrorCode.NoBookFound, "No book found", HttpStatusCode.NotFound);
        }

        /*
         * Moved this to the BookController from the UserController seeing as it has nothing to do
         * with the transformation or retrieval of User data. It only retrieves books from a given account,
         * which appears to be a responsibility better suited for the BookController.
         * This is also more in-line with the BookController given the GetBookById method.
         */
        [HttpGet]
        [Route("GetUserBooks")]
        public HttpResponseMessage GetUserBooks(Guid userId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new APIException()
                {
                    ErrorDescription = "Bad Request. Provide valid userId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            var books = BookService.GetBooksByUserId(userId);
            if (books != null)
                return Request.CreateResponse(HttpStatusCode.OK, books, JsonFormatter);
            else
                throw new APIDataException((int)BookErrorCode.NoBookFound, "No books found", HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("CreateBook")]
        public HttpResponseMessage SaveBook([FromBody]Book book)
        {
            if (book == null)
                throw new APIException()
                {
                    ErrorDescription = "Bad Request. Provide valid book object. Object can't be null.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            book = BookService.AddBook(book);
            if (book != null)
                return Request.CreateResponse(HttpStatusCode.OK, book, JsonFormatter);
            else
                throw new APIDataException((int)BookErrorCode.ErrorSavingBook, "Error Saving Book", HttpStatusCode.NotFound);
        }

        /*
         * Moved this to the BookController from the UserController. Seeing as this method doesn't actually handle
         * user information, but only ties a book to a UserId, it would make more sense to let the BookController handle
         * this interaction rather than the UserController.
         * The Book Controller can already create books without an attached UserId, so it feels more fitting.
         */
        [HttpPost]
        [Route("CreateUserBook")]
        public HttpResponseMessage SaveBook([FromUri]Guid userId, [FromBody]Book book)
        {
            if (book == null)
                throw new APIException()
                {
                    ErrorDescription = "Bad Request. Provide valid book object. Object can't be null.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            if (userId == null || userId == Guid.Empty)
                throw new APIException()
                {
                    ErrorDescription = "Bad Request. Provide valid userId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            BookRepository.Add(book);
            BookRepository.SaveChanges();
            var result = BookRepository.GetBookByID(book.Id);
            if (result != null)
                return Request.CreateResponse(HttpStatusCode.OK, result, JsonFormatter);
            else
                throw new APIDataException((int)BookErrorCode.ErrorSavingBook, "Error Saving Book", HttpStatusCode.NotFound);
        }

        [HttpPut]
        [Route("UpdateBook")]
        public HttpResponseMessage UpdateBook([FromBody]Book book)
        {
            if (book == null)
                throw new APIException()
                {
                    ErrorDescription = "Bad Request. Provide valid book object. Object can't be null.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            book = BookService.UpdateBook(book);
            if (book != null)
                return Request.CreateResponse(HttpStatusCode.OK, book, JsonFormatter);
            else
                throw new APIDataException((int)BookErrorCode.ErrorUpdatingBook, "Error Updating Book", HttpStatusCode.NotFound);
        }

        [HttpDelete] // This was HttpPost, now HttpDelete. Stays consistent with HTTP verb usage.
        [Route("DeleteBook")]
        public HttpResponseMessage DeleteBook([FromBody]Guid bookId)
        {
            if (bookId == null || bookId == Guid.Empty)
                throw new APIException()
                {
                    ErrorDescription = "Bad Request. Provide valid bookId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            var book = BookService.GetBookById(bookId);
            if (book != null)
            {
                var result = BookService.DeleteBook(book);
                if (result)
                    return Request.CreateResponse(HttpStatusCode.OK, "Book was deleted", JsonFormatter);
                else
                    throw new APIDataException((int)BookErrorCode.ErrorDeletingBook, "Error Deleting Book", HttpStatusCode.NotFound);
            }
            else
                throw new APIDataException((int)BookErrorCode.NoBookFound, "No book found", HttpStatusCode.NotFound);
        }

        protected JsonMediaTypeFormatter JsonFormatter
        {
            get
            {
                JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                JsonSerializerSettings json = formatter.SerializerSettings;

                json.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                json.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                json.NullValueHandling = NullValueHandling.Ignore;
                json.Formatting = Formatting.Indented;
                json.ContractResolver = new CamelCasePropertyNamesContractResolver();
                json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                return formatter;
            }

        }

    }
}
