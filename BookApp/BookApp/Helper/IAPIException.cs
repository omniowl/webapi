using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace BookApp.Helper {
    /// <summary>
    /// IApiExceptions Interface
    /// </summary>
    public interface IAPIException {
        /// <summary>
        /// ErrorDescription
        /// </summary>
        string ErrorDescription { get; set; }
        /// <summary>
        /// HttpStatus
        /// </summary>
        HttpStatusCode HttpStatus { get; set; }
        /// <summary>
        /// ReasonPhrase
        /// </summary>
        string ReasonPhrase { get; set; }
    }
}