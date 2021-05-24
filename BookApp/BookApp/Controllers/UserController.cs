using BookApp.Helper;
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
    [RoutePrefix("api/account")]
    [EnableCors(origins: "*", headers: "accept,Auth-Key", methods: "*")]
    public class UserController : ApiController
    {
        private IBookService BookService;
        private IUserService UserService;

        public UserController() {
        }
        public UserController(IUserService userService, IBookService bookService) {
            UserService = userService;
            BookService = bookService;
        }

        [HttpGet]
        [Route("GetUserById")]
        public HttpResponseMessage GetUserById(Guid userId) {
            if (userId == null || userId == Guid.Empty)
                throw new APIException() {
                    ErrorDescription = "Bad Request. Provide valid userId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            User user = UserService.GetUserById(userId); // Changed "var" to "User" to be explicit.
            if (user != null)
                return Request.CreateResponse(HttpStatusCode.OK, user, JsonFormatter);
            else
                throw new APIDataException(4, "No user found", HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("CreateUser")]
        public HttpResponseMessage SaveUser([FromBody]User user) {
            if (user == null)
                throw new APIException() {
                    ErrorDescription = "Bad Request. Provide valid user object. Object can't be null.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            user = UserService.AddUser(user);
            if (user != null)
                return Request.CreateResponse(HttpStatusCode.OK, user, JsonFormatter);
            else
                throw new APIDataException(5, "Error Saving User", HttpStatusCode.NotFound);
        }

        [HttpPut]
        [Route("UpdateUser")]
        public HttpResponseMessage UpdateUser([FromBody]User user) {
            if (user == null)
                throw new APIException() {
                    ErrorDescription = "Bad Request. Provide valid user object. Object can't be null.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            user = UserService.UpdateUser(user);
            if (user != null)
                return Request.CreateResponse(HttpStatusCode.OK, user, JsonFormatter);
            else
                throw new APIDataException(6, "Error Updating User", HttpStatusCode.NotFound);
        }

        [HttpDelete] // This was HttpPost, now HttpDelete. Stays consistent with HTTP verb usage.
        [Route("DeleteUser")]
        public HttpResponseMessage DeleteUser([FromBody]Guid userId) {
            if (userId == null || userId == Guid.Empty)
                throw new APIException() {
                    ErrorDescription = "Bad Request. Provide valid userId guid. Can't be empty guid.",
                    HttpStatus = HttpStatusCode.BadRequest
                };
            User user = UserService.GetUserById(userId); // Changed "var" to "User" to be explicit.
            if (user != null) {
                bool result = UserService.DeleteUser(user); // Changed "var" to "bool" to be explicit.
                if (result)
                    return Request.CreateResponse(HttpStatusCode.OK, "Book was deleted", JsonFormatter);
                else
                    throw new APIDataException(7, "Error Deleting User", HttpStatusCode.NotFound);
            } else
                throw new APIDataException(4, "No user found", HttpStatusCode.NotFound);
        }

        protected JsonMediaTypeFormatter JsonFormatter {
            get {
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
