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

Suffix naming conventions:
-----------------
1. Entity - no suffix (e.g. Budget, Wallet)
2. Controllers - Controller suffix (e.g. WalletControler, BudgetController)
5. Rest (if rest will be used) - Rest suffix (e.g. WalletRest, BudgetRest)

Project modularity:
-----------------
ExpenseManagerSolution will be devided into following projects:

1. ExpenseManager.Entity - this project will contain only DB entities
2. ExpenseManager.BusinessLogic - this project will contain common buissiness logic used in FrontEnt project
3. ExpenseManager.FrontEnd - controlles, views, js, html and css
