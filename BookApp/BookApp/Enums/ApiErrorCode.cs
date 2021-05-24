using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookApp.Enums
{
    public enum BookErrorCode : int
    {
        NoBookFound = 1,
        ErrorSavingBook = 2,
        ErrorUpdatingBook = 3,
        ErrorDeletingBook = 4
    }

    public enum UserErrorCode : int
    {
        NoUserFound = 4,
        ErrorSavingUser = 5,
        ErrorUpdatingUser = 6,
        ErrorDeletingUser = 7
    }
}