<h1>LazyPhysicist</h1>

<p>WARNING! THE PROJECT IS UNDER DEVELOPMENT! DO NOT USE IT IN CLINIC!
</p>

<h2>Hello, ESAPI!</h2>
<p>This project is designed to make it easier to solve our daily tasks in radiotherapy planning. And also for practice in C# programming.<br>
There are several ideas for future projects based on my vision of simplifying the planning process.
There are no comments and unit tests yet, but I hope everything will be.</p>

<p>Check out what's already done &darr; and stay tuned ;)</p>

<h2>LazyOptimizer —</h2>

<p>is a plugin that helps to fill the plan optimizer based on the user's previously created plans.
Plan data is taken from the SQLite database, which is filled in by the PlansCache app.<br>
Optimizer templates are not lazy enough.<br>
The selection of a suitable plan is based on the number of fractions, single dose, treatment machine, 
technique and, most importantly, patient's structures.</p>

<p><i>— Why not objective templates?</i> you may ask.<br>
Because they need to be updated quite often, removing irrelevant ones if your planning skills change. With this plugin, this is not necessary. You just make your plans and they will always be up to date for reuse. Also, templates don't always match structures so well.
</p>

<img src="/LazyOptimizer/Resources/example.png" alt="How LazyOptimizer works"/>

<h2>Using LazyOptimizer</h2>
<p>
<b>Standard workflow:</b>
<ul>
	<li>Create a plan, add fields</li>
	<li>Run the plugin</li>
	<li>Choose a plan from those offered</li>
	<li>Click the "Load into plan" button</li>
</ul>
</p>
<p>
	
<b>More details:</b>
</p>
<p>First you need to add a script to Eclipse from <i>Tools > Scripts menu</i>. The plugin can be on the local network, as the user settings and database will be stored in the local folder <i>%APPDATA%\LazyOptimizer</i>.<br>
You also need to install the <a href="https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48">.NET Framework 4.8</a> Runtime.
</p>
<p>Scripts to be approved in  <i>Tools > Script Approvals...</i>
<ul>
	<li>LazyOptimizer.esapi</li>
	<li>PlansCache.exe</li>
	<li>ESAPIInfo.esapi</li>
</ul>
</p>
<p>Create the plan you are going to calculate, add the fields and run this plugin.
</p>
<p>At the first start, it is necessary to fill the database with previous plans by clicking the <b>Check Previous Plans</b> button. The first launch can be long, as all plans for all patients are checked (about a couple of minutes per thousand patients). Only the current user's plans (as accurate as possible) are written to the database. On next launches, only new patients will be checked.
</p>
<p>If plans are found that match the current plan, they will be shown in the list.
Plan Selection Criteria:
<ul>
	<li>The number of factions matches</li>
	<li>Same dose per fraction</li>
	<li>Similar set of structures</li>
	<li>Optionally the same Treatment Machine and Technique</li>
</ul>
</p>
<p>Choose the plan that suits you, match the structures if there are inaccuracies. Structures are matched using the Levenshtein algorithm, the names of the structures do not have to be exactly the same.<br>
If no match is found for a structure, then you can <b>double-click</b> on the structure in the list of <b>undefined structures</b>. The best match will be found in other plans and added.
</p>
<p>There is one more option - <b>Merging</b> the structures of several plans, and averaging the doses, priorities and EUD A parameters in the structures. Activate this mode in the settings if you want to use it.
</p>

<p>You can override the objective priorities for all OARs by clicking the buttons:
<ul>
<li><b>=0</b> — set to zero</li>
<li><b>As Is</b> — as in the plan from the database</li>
<li><b>Set</b> — its value in the field next</li>
</ul>
</p>
<p>Check or uncheck the box for adding NTO.
</p>
<p>Add objectives to the plan by clicking the <b>Load into plan</b> button.
</p>

<h2>PlansCache —</h2>
<p>is a standalone console app that fill the SQLite database for LazyOptimizer plugin.</p>
<img src="/Images/PlansCache_example.png" alt="How PlansCache works"/>

<h2>Tested in ESAPI Versions</h2>
<ul>
  <li>15.6</li>
  <li>16.1</li>
</ul>
