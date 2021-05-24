# Web API prototype project
This repository stores a REST API project. It is a small prototype that is similar to a real project, that will be relevant for the back-end position.

Below is a few tasks that we have prepared for you. We only expect you to spend around 3 hours on this – not days. The most important is for us to get insight into your understanding/thinking. We ask you to do the following:

1. Fork this repo to your own GitHub account and clone your fork to your machine. Run the application and get an overview over how it is working.
2. Review the code base and think about how it could be improved – especially the general structure and code patterns.
3. Choose to do some relevant changes, accompanied by inline comments explaining the change and why.
4. More overall thoughts/suggestions/ideas for the code or architecture should be added below in this README. This also gives you a chance to mention changes that you did not have time to do in the short timeframe.
5. Push and make a pull request to this repository with your changes.

----

#### Add general thoughts/suggestions/ideas here:
I couldn't properly start the prototype because it seems that Visual Studio for some reason couldn't include the Roslyn compiler in the bin folder. Looking over the solution however, I don't find any really glaring issues as such. The project appears to follow the Repository Pattern and uses Dependency Injection to ensure looser coupling between classes. Also makes it easier to switch in different implementations of the same interfaces.

My changes include:
* Changing the readability of some classes. I am not a fan of using "var" if I can avoid it. There are edge cases where it makes sense to use it, but often readability suffers, when skimming code. Also introduced some "using" statements rather than let namespaces be in the code.
* I moved two methods from the User Controller into the Book Controller. This made sense to me because the actual nature of those methods (creating a book that is bound to an account guid, and a method to get all books associated with an account) did not fit quite so well in the User controller. The Book Controller already has methods to create books not associated with accounts, and retrieving books too. So the distinction seems unnecessary.
* I removed what I considered to be redundant data from the API and IAPI exception classes. They both used an "ErrorCode" field, however all uses of the mentioned exception types only ever gave the error code the same value as the HttpStatus field and thus seemed redundant.
* I introduced two enums for error codes used with the ApiData exception class. Basically, the integers used to signify error codes, were akin to "magic numbers". They could have easily been reused for different error types by mistake, so to alleviate this, two enums were introduced (BookErrorCode/UserErrorCode). Their job is to represent whatever we want the error code to be without needing to care about the error code itself. This makes writing the code less error prone and more consistent. It also makes sure that if we want to change the error code for any reason at any time, we can simply change the associated value in the enum, rather than having to go find the references in code.

What I'd like to have done:
* If I had more time I'd have wanted to change the solution so that it could make use of the `AspNetCore.Http` and `AspNetCore.Mvc` namespaces. This could give me access to the `ProducesResponseTypes` attribute, that ensures better understanding of Method contracts for other developers and ourselves. I'd also be using `async` methods that return `IActionResult` to streamline the code.
* Change the API routing. In the `BookController`, there were originally 4 methods. They each perfectly fit the GET, POST, PUT and DELETE verbs respectively. However, the routing used was explicit rather than implicit. This means that instead of having the routes `GetBookById`, `CreateBook`, `UpdateBook` and `DeleteBook` on the front-end, you'd simply have `api/book/` followed by whatever information you need and so the verb you accompany the route with, will determine what route the API makes use of. But that is more of a streamlining approach and may not be desired from a design perspective. There can be merit in explicit endpoints.
* Change the Book Model to have an Abstract book super class that both book types extend. Both `Book` and `BookExtended` share a lot of fields and so could benefit from a bit of inheritance.