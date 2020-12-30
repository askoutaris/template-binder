# template-binder
A C# angular like text template binder


# usage

```
// you can create your own pipes and add them here
var pipeTypes = new Type[] {
  typeof(BooleanTextPipe),
  typeof(DatePipe),
  typeof(DecimalPipe),
  typeof(TextPipe),
};

var pipeFactory = new PipeFactoryDefault(pipeTypes);

var template = @"
  The user {FirstName} {LastName} 
  born {DateOfBirth|date:format=o} 
  has logged in {LoginTimes},
  has account balance {AccountBalance|decimal:format=N2},
  {IsActive|booleantext:falseValue=no,trueValue=yes},
  {IsLockedOut|booleantext:falseValue=no,trueValue=yes}";

var binder = new Binder(pipeFactory, template);

var parameters = new Parameter[] {
  new Parameter.Text("FirstName", "David"),
  new Parameter.Text("LastName", "Parker"),
  new Parameter.Date("DateOfBirth", new DateTime(1980,08,15,12,30,0)),
  new Parameter.Number("LoginTimes", 85),
  new Parameter.Number("AccountBalance", 1750.45M),
  new Parameter.Boolean("IsActive",true),
  new Parameter.Boolean("IsLockedOut", false)
};

var text = binder.Bind(parameters);

//The user David Parker 
//born 1980-08-15T12:30:00.0000000 
//has logged in 85,
//has account balance 1.750,45,
//is active: yes,
//is locked out: no
```
