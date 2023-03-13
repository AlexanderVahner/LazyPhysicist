# LazyPhysicist

WARNING! THE PROJECT IS UNDER DEVELOPMENT! DO NOT USE IT IN CLINIC!

## Hello, ESAPI!
There are no comments and unit tests yet, but I hope everything will be.
Stay tuned ;)

## ESAPI Version
15.6

## LazyOptimizer --
is a plugin that helps to fill the plan optimizer based on the user's previously created plans.
Plan data is taken from the SQLite database, which is filled in by the PlansCache app.
Optimizer templates are not lazy enough.
The selection of a suitable plan is based on the number of fractions, single dose, treatment machine, 
technique and, most importantly, patient's structures.

## PlansCache --
is a standalone app that fill the SQLite database for LazyOptimizer plugin.