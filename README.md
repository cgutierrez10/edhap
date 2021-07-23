Will implement dual account budgetting software. One account line is actual and one account line is budgetted.

Planning to make this as a rolling window budgetting tool rather than a fixed interval budget.

Otherwise broadly following common budgeting layout and workflow.

Basic files laying out the data table structure for budgeting software.

Note: nunit tests added. No json configs to enable visual studio code to run these tests directly. Will have to use dotnet test CLI options. Using nunit means 'dotnet test --filter fullyqualifiedname~edhapTests.[testclass]' is the way to run a single test, or 'dotnet test' to simply run the whole set.
