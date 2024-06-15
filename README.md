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

# Stargazing
Please consider giving the project a GitHub Star.  Thank you in advance!

# Quickstart
Jube runs on commodity Linux. The Quickstart has the following prerequisites:

* .Net 8 Runtime.
* Postgres database version 13 onwards (tested on 15.4 but no significant database development to cause a breaking change).

Subject to prerequisites, Jube can be up and running in minutes:

```shell
git clone https://github.com/jube-home/jube.git
cd jube/Jube.App
export ConnectionString="Host=<host>;Port=<port>;Database=<defaultdb>;Username=<username>;Password=<password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;SSL Mode=Require;Trust Server Certificate=true;"
export ASPNETCORE_URLS="https://localhost:5001"
export JWTKey="IMPORTANT:_ChangeThisKey_~%pvif3KRo!3Mk|1oMC50TvAPi%{mUt<9"B&|>DP|GZy"YYWeVrNUqLQE}mz{L_UsingThisKeyIsDangerous"
dotnet run
```

For security, there is no means to pass configuration values via anything other than Environment Variables,  and the contents of those Environment Variables are never - ever - stored by Jube (which is something the CodeQL security scanner tests for).  

There are sensitive cryptographic values that need to be included at startup. At a minimum the JWTKey value is required:

```shell
export JWTKey="IMPORTANT:_ChangeThisKey_~%pvif3KRo!3Mk|1oMC50TvAPi%{mUt<9"B&|>DP|GZy"YYWeVrNUqLQE}mz{L_UsingThisKeyIsDangerous"
```

The JWTKey value is used to encrypt access tokens providing for API authentication, and therefore user interface authentication.  

While outside of the scope of this installation documentation, other sensitive variables, while optional,  are strongly suggested:

```shell
export PasswordHashingKey="IMPORTANT:_ChangeThisKey_~%pvif3KRo!3Mk|1oMC50TvAPi%{mUt<9"B&|>DP|GZy"YYWeVrNUqLQE}mz{L_UsingThisKeyIsDangerous"
```

It is imperative that the keys be changed from their defaults and kept safe in appropriate storage.  Jube will not start if the keys above are used.

Change the template value for setting the ConnectionString and JWTKey Environment Variables, then run the shell script as above. Wait for the build to complete, after which observe the welcome message.

Waiting a few moments more will ensure that the embedded Kestrel web server is started correctly.  In a web browser, navigate to the bound URL [https://localhost:5001/](https://localhost:5001/) as per the ASPNETCORE_URLS Environment Variable.

The default user name \ password combination is Administrator \ Administrator,  although the password will be need to be changed on first login.

A more comprehensive installation guide is available in the [Getting Started](https://jube-home.github.io/jube/GettingStarted/) of the [documentation](https://jube-home.github.io/jube).

# Documentation
The [documentation](https://jube-home.github.io/jube) has been drafted to include all features, and there should not be any undocumented know-how.  The documentation adopts an instructional style that will explain most features step-by-step with extensive use of screenshots.  

Jube is committed to high-quality instructional documentation and maintains it as part of the overall release methodology.  If documentation is inadequate,  unclear or missing, raise a [Github Issue](https://github.com/jube-home/jube/issues).

# Training
Biannually, a training event is conducted at the [Radisson Blu Hotel in Larnaca, Cyprus](https://www.radissonhotels.com/en-us/hotels/radisson-blu-larnaca). The training program focuses on achieving proficiency in the effective utilization of Jube.  The training is delivered by Richard Churchman, the author of Jube.

The schedule covers a duration of three days, with the length of each day ranging from 6 to 8 hours, depending on the undertaking of Elective Modules. Elective Modules cover in-depth training in advanced administrative concepts using dedicated training servers.  Elective Modules are targeted at technical participants whom are likely to assume overall system administrative responsibility of an implementation of Jube.

Day 1:

* Introduction.
* User Interface.
* HTTP Messaging.
* Models and Payload.
* Inline Functions.
* Abstraction Rules.
* Abstraction Calculations.
* Lists and Dictionaries.
* Activation Rules.
* Elective: Architecture and Caching.
* Elective: Environment Variables.
* Elective: Installation and Log Configuration.

Day 2:

* Suppressions.
* Sanctions Fuzzy Matching.
* Time To Live (TTL) Counters.
* Introduction to Artificial Intelligence (AI).
* Exhaustive AI training.
* Case Management.
* Security.
* Elective: Tracing Transaction Flow and Response Time Analysis.
* Elective: High Availability.
* Elective: Performance Counters.
* Elective: AMQP.

Day 3:

* SQL database discovery.
* Performance Monitoring.
* Visualisation and Reporting.
* Inline Scripts.
* Scores via R Plumber (HTTP).
* Elective: Cache Bottleneck Analysis.
* Elective: Archive Bottleneck Analysis.
* Elective: Multi-Tenancy.
* Elective: Git Definitions Backup and Recovery.

Given a maximum of 8 participants, and a minimum of 4, the total cost of the program is EUR 1395 per attendee, which includes lunch and refreshments. Additionally, participants will have access to four hours of Commercial Support, valid for six months after completion of the program.  The program is available on the subsequent dates:

* In 2024, starting Tuesday December 3rd through end of Thursday December 5th.
* In 2025, starting Tuesday June 3rd through end of Thursday June 5th.

For further details, including detailed training plan, kindly contact [support@jube.io](mailto:support@jube.io). 

Where confidentially considerations exist, the same program can be made available at the client's premises for a daily rate of EUR 800, excluding customary business travel and accommodation costs.

# Support
Free Support is available via [Github Issues](https://github.com/jube-home/jube/issues). 

Commercial support available at a daily rate of EUR 800 prorated. It is uncommon for a client to require more than two days of Commercial Support per month given an active production implementation,  although implementation demands vary depending on client technical proficiency. For further details, please contact [support@jube.io](mailto:support@jube.io).

# Reporting Vulnerabilities

Please do not file GitHub issues for security vulnerabilities, as they are public.

Jube takes security issues very seriously. If you have any concerns about Jube or believe you have uncovered a vulnerability, please contact via the e-mail address security@jube.io. In the message, try to describe the issue and, ideally, a way of reproducing it.

Please report any security problems to Jube before disclosing them publicly.

# Governance
Jube Holdings Limited is a Cyprus company registered HE404521. Jube Holdings Limited owns Jube software and Trademarks (registered or otherwise). Jube is maintained by Jube Operations Limited, a United Kingdom company with registration 14442207. Jube Operations Limited is a wholly owned subsidiary of Jube Holdings Limited. Jube Operations Limited provides training and support services for Jube. Jube and "Jooby" (the logo) is a registered trademark in Cyprus. 

# Licence
Jube is distributed under [AGPL-3.0-or-later](https://www.gnu.org/licenses/agpl-3.0.txt).