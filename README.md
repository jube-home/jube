![Image](logo.png)

# About Jube

Jube is open-source transaction and event monitoring software. Jube implements real-time data wrangling, artificial intelligence, decision making and case management. Jube is particularly strong when implemented in fraud and abuse detection use cases.

Data wrangling is real-time. Data wrangling is directed via a series of rules created using either a point-and-click rule builder or an intuitive rule coder. Rules are in-memory matching functions tested against data returned from high-performance cache tables, where datasets are fetched only once for each key that the rules roll up to for each transaction or event processing, with the matches aggregating using a variety of functions. Alternative means of maintaining a lightweight long-term state to facilitate data wrangling is Time To Live (TTL) Counters which are incremented on rule match and then decremented for that incrementation on time-lapse.

Data wrangling return values are independently available for use as features in artificial intelligence training and real-time recall or tested by rules to perform a specific action (e.g., the rejection of a transaction or event). Wrangled values are returned in the real-time response payload and can facilitate a Function as a Service (FaaS) pattern. Response payload data is also stored in an addressable fashion, improving the experience of advanced analytical reporting while also reducing database resource \ compute cost.

Jube is developed stateless and can support massive horizontal scalability and separation of concerns in the infrastructure.

Jube takes a novel approach to artificial intelligence, ultimately Supervised Learning, yet blending anomaly detection with confirmed class data to ensure datasets of sufficient amounts of class data. Using data archived from its processing, Jube searches for optimal input variables, hidden layers and processing elements. The result is small, optimal, generalised and computationally inexpensive models for efficient real-time recall. The approach taken by Jube allows artificial intelligence's benefits to be available very early in an implementation's lifecycle. It avoids over-fitting models to typology long since passed.

Transaction or event monitoring overlooks the embedding in human-engaging business processes. Jube is real-time, but this does not forgo the need for manual intervention; hence Jube makes comprehensive and highly customisable case management and visualisation intrinsically available in the user interface.

To ensure the segregation of user responsibilities, a user, role and permission model is in place, which controls access to each page within the Jube user interface. Detailed audit logs are available. Any update to a configuration by a user retains a version history,  in which the original is logically deleted and then replaced with the new version.

Jube is multi-tenanted,  allowing a single infrastructure to be shared among many logically isolated entities, maintaining total isolation between tenant data with no loss of function in the user interface.

# Quickstart
Jube runs on commodity Linux. The Quickstart has the following prerequisites:

* .Net 6 SDK.
* Postgres database version 13 onwards.

Subject to prerequisites, Jube can be up and running in minutes:

```shell
git clone https://github.com/jube-home/jube.git
cd jube/Jube.App
export ConnectionString="Host=<host>;Port=<port>;Database=<defaultdb>;Username=<username>;Password=<password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;SSL Mode=Require;Trust Server Certificate=true;"
export ASPNETCORE_URLS="https://localhost:5001"
dotnet run
```

Change the template value for setting the ConnectionString Environment Variable, then run the shell script as above. Wait for the build to complete, after which observe the welcome message.

Waiting a few moments more will ensure that the embedded Kestrel web server is started correctly.  In a web browser, navigate to the bound URL [https://localhost:5001/](https://localhost:5001/) as per the ASPNETCORE_URLS Environment Variable.

The default user name \ password combination is Administrator \ Administrator,  although the password will be need to be changed on first login.

A more comprehensive installation guide is available in the [Getting Started](https://jube-home.github.io/jube/GettingStarted/) of the [documentation](https://jube-home.github.io/jube).

# Documentation
The [documentation](https://jube-home.github.io/jube) has been drafted to include all features, and there should not be any undocumented know-how.  The documentation adopts an instructional style that will explain most features step-by-step with extensive use of screenshots.  The documentation, where possible, has been written to support an Educate, Demonstrate, Imitate and Practice (EDIP) style.  Given the EDIP style, an excellent approach to using the documentation is:

* Educate: Read the topic from start to finish.
* Demonstrate: Read the topic and pause on each instruction and;
* Imitate: A read of the topic following the instructions step by step.
* Practice: Without referring to the documentation, practice using the topic,  varying parameters as appropriate.

Jube is committed to high-quality instructional documentation and maintains it as part of the overall release methodology.  If documentation is inadequate,  unclear or missing, raise a GitHub Bug.  More generally,  if the software does not perform as per documentation, raise a GitHub Bug.

# Retained Helpdesk and Support Services
A consistent, simple, transparent and cost effective offer:

* Access to Retained Helpdesk is a fee of $990 per month. Retained Helpdesk includes online chat or ticketing surrounding documented features.  Retained Helpdesk offers assured response times between 4 and 12 hours, ranging Critical through Moderate severity.  Access to Retained Helpdesk does not include fees for time spent, which is in addition, at $60 an hour, or part thereof. Access to Retained Helpdesk is a three month commitment, with a three month notice period.
* Support Services which are delivered outside of online chat or ticketing, such as training, integration, customisation, development or go-live support are instructed by Work Order \ Statement of Work, but the fees are the same, being time spent $60 an hour, or part thereof. 
* Support Services are usually delivered remotely for cost efficiency, although where delivered onsite, excludes business class travel expenses.  Onsite delivery of training and go-live support tends to be impactful, but in other work effort, it is a suboptimal use of time.  Where Support Services are delivered onsite, the commitment should not be less than two two weeks,  allowing for some time to handle client commitments where required.
* Given continued and uninterrupted commitment to Retained Helpdesk, all fees are capped for annual increase by no more than the US Retail Prices Index.
* Where access to Retained Helpdesk is not in place,  Support Services as set out above are available, at a higher $180 an hour, or part thereof, instructed by Work Order \ Statement of Work.

Otherwise governed by the Jube Operations Limited Service Terms, in matters relating to Insurances,  Warranties and Limitations.

It is commonplace to conduct development in own fork. Development conducted in own fork and that making a contribution to the public project is at he discretion of the client, yet subject to the terms of the [AGPL-3.0-only](https://www.gnu.org/licenses/agpl-3.0.txt) licence.

Invoicing and Credit Terms by negotiation subject to covenant, but usually monthly in arrears.

For more information, email [support@jube.io](mailto:support@jube.io).

# Reporting Vulnerabilities

Please do not file GitHub issues for security vulnerabilities, as they are public.

Jube takes security issues very seriously. If you have any concerns about Jube or believe you have uncovered a vulnerability, please contact via the e-mail address security@jube.io. In the message, try to describe the issue and, ideally, a way of reproducing it.

Please report any security problems to Jube before disclosing them publicly.

# Governance
Jube Holdings Limited is a Cyprus company registered HE404521. Jube Holdings Limited owns Jube software and Trademarks (registered or otherwise). Jube is maintained by Jube Operations Limited, a United Kingdom company with registration 14442207. Jube Operations Limited is a wholly owned subsidiary of Jube Holdings Limited. Jube Operations Limited provides training and support services for Jube. Jube and "Jooby" (the logo) is a registered trademark in Cyprus. 

# Licence
Jube is distributed under [AGPL-3.0-only](https://www.gnu.org/licenses/agpl-3.0.txt).