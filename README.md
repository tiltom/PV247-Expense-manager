# PV247-Expense-manager

Name conventions:
-----------------
 - Types and namespaces: UpperCamelCase
 - Interfaces: IUpperCamelCase
 - Type parameters: TUpperCamelCase
 - Methods, properties and events: UpperCamelCase
 - Local variables: lowerCamelCase
 - Local constants: lowerCamelCase
 - Parameters: lowerCamelCase
 - Fields (not private): UpperCamelCase
 - Instance fields (private): _lowerCamelCase
 - Static fields: _lowerCamelCase
 - Constant field: UpperCamelCase
 - Static readonly field: UpperCamelCase
 - Enum member: UpperCamelCase

In the other words naming style conventions are same as ReSharpers (check ReSharper -> Options -> Code Editing-> C# -> Naming Style)

Comments:
-----------------
 - Write comments of all public methods (start with '///', the rest will be created automatically)
 - Add comments within methods only when doing something not clear (statement db.SaveChanges() doesn't need comment)

Suffix naming conventions:
-----------------
1. Entity - no suffix (e.g. Budget, Wallet)
2. Controllers - Controller suffix (e.g. WalletControler, BudgetController)
3. Rest (if rest will be used) - Rest suffix (e.g. WalletRest, BudgetRest)
4. Business logic - Service suffix (e.g. WalletService, BudgetService)
5. Unit tests - ServiceTest suffix (e.g. WalletServiceTest, BudgetServiceTest)

Project modularity:
-----------------
ExpenseManagerSolution will be devided into following projects:

1. ExpenseManager.Entity - this project will contain only DB entities
2. ExpenseManager.BusinessLogic - this project will contain common bussiness logic used in Web project
3. ExpenseManager.BusinessLogic.Test - unit tests for business logic
4. ExpenseManager.Database - contexts, initializers and seeding 
5. ExpenseManager.Web - controlles, views, js, html and css

Class structure:
-----------------
 - Add 'this' keyword when using a member of current class
 - One class per one file, never write two classes into one file
 - Use #region and #endregion keywords for protected and private methods
 - Write constants on the top of a class, then private fields, then properties, then constructor and then methods
 - Order methods by their accessibility: public methods first, then protected ones, and private methods in the end
 - Add white line before properties
 - Never use public fields, use properties
 - Order attributes by their name alphabetically (but Guid on top)

Other rules:
-----------------
 - Controller names should be in singular (e.g. TransactionController, not TransactionsController)
 - Allways commit an application that is built and without an errors and warnings.
 - Use English everywhere (names, comments, commits, ...)
