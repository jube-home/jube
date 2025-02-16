---
layout: default
title: Welcome
nav_order: 1
---

![Image](logo.png)

# About Jube

Jube is open-source transaction and event monitoring software. Jube implements real-time data wrangling, artificial
intelligence, decision-making and case management. Jube is particularly strong when implemented in fraud and abuse
detection use cases.

Data wrangling is real-time. Data wrangling is directed via a series of rules created using either a point-and-click
rule builder or an intuitive rule coder. Rules are in-memory matching functions tested against data returned from
high-performance cache tables, where datasets are fetched only once for each key that the rules roll up to for each
transaction or event processing, with the matches aggregating using a variety of functions. Alternative means of
maintaining a lightweight long-term state to facilitate data wrangling is Time To Live (TTL) Counters which are
incremented on rule match and then decremented for that incrementation on time-lapse.

Data wrangling return values are independently available for use as features in artificial intelligence training and
real-time recall or tested by rules to perform a specific action (e.g., the rejection of a transaction or event).
Wrangled values are returned in the real-time response payload and can facilitate a Function as a Service (FaaS)
pattern. Response payload data is also stored in an addressable fashion, improving the experience of advanced analytical
reporting while also reducing database resource \ compute cost.

Jube is developed stateless and can support massive horizontal scalability and separation of concerns in the
infrastructure.

Jube takes a novel approach to artificial intelligence, ultimately Supervised Learning, yet blending anomaly detection
with confirmed class data to ensure datasets of sufficient amounts of class data. Using data archived from its
processing, Jube searches for optimal input variables, hidden layers and processing elements. The result is small,
optimal, generalised and computationally inexpensive models for efficient real-time recall. The approach taken by Jube
allows artificial intelligence's benefits to be available very early in an implementation's lifecycle. It avoids
over-fitting models to typology long since passed.

Transaction or event monitoring overlooks the embedding in human-engaging business processes. Jube is real-time, but
this does not forgo the need for manual intervention; hence Jube makes comprehensive and highly customisable case
management and visualisation intrinsically available in the user interface.

To ensure the segregation of user responsibilities, a user, role and permission model is in place, which controls access
to each page within the Jube user interface. Detailed audit logs are available. Any update to a configuration by a user
retains a version history, in which the original is logically deleted and then replaced with the new version.

Jube is multi-tenanted, allowing a single infrastructure to be shared among many logically isolated entities,
maintaining total isolation between tenant data with no loss of function in the user interface.

# Stargazing

Please consider giving the project a GitHub Star. Thank you in advance!

# Quickstart with the Sandbox

The fastest means to get started is via the [sandbox](https://sandbox.jube.io/Account/Login). The Sandbox offers a fully
functional version of Jube, tailored for learning and training purposes. It leverages Jube's multi-tenancy capabilities
to deliver the experience of an isolated environment, even though it operates on shared resources with logical data
isolation. While the Visualization and Reporting features are limited to three example tables, the overall experience
remains thorough and effective.

# Quickstart with docker compose

A docker compose file is available - it is docker-compose.yml in the root directory - to quickly set up and orchestrate
an installation of Jube, provided Docker is already
installed. This docker compose file creates and configures the following components:

* postgres image and start.
* redis/redis-stack:latest image and start.
* rabbitmq:3-management image and start.
* The building of an image of Jube:
    * Starting an image of Jube for WebAPI Services (API and User Interface).
    * Starting an image of Jube for the Background Jobs.

With the prerequisites in place, Jube can be up and running in just a few minutes:

```shell
git clone https://github.com/jube-home/jube.git
cd jube
docker compose up -d
```

Upon conclusion Jube will be listening on the docker host, on port 5001, hence (http://localhost:5001).

Be sure to update the docker compose file on production use, for appropriate credential creation and management.

# Quickstart with dotnet run

Jube runs on commodity Linux. For running directly, there exists the following prerequisites:

* .Net 9 Runtime.
* Postgres database version 13 onwards (tested on 15.4 but no significant database development to cause a breaking
  change).
* Optional but recommended: Redis version 6 or above (it probably works fine on earlier versions, as the command used
  are basic. RESP
  wire compatible implies that it is possible to use KeyDB, DragonflyDB, Garnet or any RESP compliant wire protocol
  database).

Subject to prerequisites, Jube can be up and running in minutes:

```shell
git clone https://github.com/jube-home/jube.git
cd jube/Jube.App
export ConnectionString="Host=<host>;Port=<port>;Database=<defaultdb>;Username=<username>;Password=<password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;SSL Mode=Require;Trust Server Certificate=true;"
export RedisConnectionString="<host>"
export ASPNETCORE_URLS="https://localhost:5001"
export JWTKey="IMPORTANT:_ChangeThisKey_~%pvif3KRo!3Mk|1oMC50TvAPi%{mUt<9"B&|>DP|GZy"YYWeVrNUqLQE}mz{L_UsingThisKeyIsDangerous"
dotnet run
```

For security, and docker support, there is no means to pass configuration values via anything other than Environment
Variables, and the
contents of those Environment Variables are never - ever - stored by Jube (which is something the CodeQL security
scanner tests for).

The use of Redis is encouraged as it provides a 33% improvement in response times, and a marked improvement in response
time variance contrasted against using Postgres Database. Redis also does not require Cache table indexing jobs, and
while such indexing is automatic on existing data for Postgres, it does create some delay in the creation of Search Keys
retroactively, however by contrast Search Keys in Redis can only be created on a forward only basis and there is no
preexisting data. In general the trade of between Key \ Value Pair in-memory databases and RDMBS durable databases is
not
trivial. In general, the use of Postgres Database is probably the right choice for low volume or cost sensitive
implementations
where the staff and infrastructure complexity costs can't be justified, whereas for any serious real-time implementation
given infrastructure technical capacity, doubtless Redis is the better choice. Setting the Redis Environment Variable to
false will fall back to using the Postgres Database for cache, and is the more simple implementation:

```shell
export Redis="False"
```

There are sensitive cryptographic values that need to be included at startup. At a minimum the JWTKey value is required:

```shell
export JWTKey="IMPORTANT:_ChangeThisKey_~%pvif3KRo!3Mk|1oMC50TvAPi%{mUt<9"B&|>DP|GZy"YYWeVrNUqLQE}mz{L_UsingThisKeyIsDangerous"
```

The JWTKey value is used to encrypt access tokens providing for API authentication, and therefore user interface
authentication.

While outside of the scope of this installation documentation, other sensitive variables, while optional, are strongly
suggested:

```shell
export PasswordHashingKey="IMPORTANT:_ChangeThisKey_~%pvif3KRo!3Mk|1oMC50TvAPi%{mUt<9"B&|>DP|GZy"YYWeVrNUqLQE}mz{L_UsingThisKeyIsDangerous"
```

It is imperative that the keys be changed from their defaults and kept safe in appropriate storage. Jube will not start
if the keys above are used.

Change the template value for setting the ConnectionString and JWTKey Environment Variables, then run the shell script
as above. Wait for the build to complete, after which observe the welcome message.

Waiting a few moments more will ensure that the embedded Kestrel web server is started correctly. In a web browser,
navigate to the bound URL [https://localhost:5001/](https://localhost:5001/) as per the ASPNETCORE_URLS Environment
Variable.

The default username \ password combination is Administrator \ Administrator, although the password will be need to be
changed on first login.

A more comprehensive installation guide is available in
the [Getting Started](https://jube-home.github.io/jube/GettingStarted/) of
the [documentation](https://jube-home.github.io/jube).

# Documentation

The [documentation](https://jube-home.github.io/jube) has been drafted to include all features, and there should not be
any undocumented know-how. The documentation adopts an instructional style that will explain most features step-by-step
with extensive use of screenshots.

The [documentation](https://jube-home.github.io/jube) has been drafted to include all features, and there should not be
any undocumented know-how. The documentation adopts an instructional style that will explain most features step-by-step
with extensive use of screenshots.

Jube is committed to high-quality instructional documentation and maintains it as part of the overall release
methodology. If documentation is inadequate, unclear or missing, raise
a [Github Issue](https://github.com/jube-home/jube/issues).

# Support

Support is available through three channels:

* Ask Jooby, our AI assistant, located in the bottom right-hand corner of the [jube](https://jube.io) website.
* Email at support@jube.io.
* GitHub issues.

Feel free to reach any using any option!

# Training

To further accelerate and ensure positive outcomes, a comprehensive training program is available. This program is
designed to help participants develop the skills needed to effectively implement and manage Jube with confidence.

By the end of the implementation training, participants will:

* Confidently implement and manage Jube within their organization.
* Optimize Jube for performance, scalability, and security.
* Accelerate Jube adoption through hands-on experience and best practices.
* For Developers; Troubleshoot and debug the system effectively.

Explore the [agenda](https://www.jube.io/training/) plan on the [jube](https://www.jube.io) website

# Reporting Vulnerabilities

Please do not file GitHub issues for security vulnerabilities, as they are public.

Jube takes security issues very seriously. If you have any concerns about Jube or believe you have uncovered a
vulnerability, please contact via the e-mail address security@jube.io. In the message, try to describe the issue and,
ideally, a way of reproducing it.

Please report any security problems to Jube before disclosing them publicly.

# Governance

Jube Holdings Limited is a Cyprus company registered HE404521. Jube Holdings Limited owns Jube software and Trademarks (
registered or otherwise). Jube is maintained by Jube Operations Limited, a United Kingdom company with registration
14442207. Jube Operations Limited is a wholly owned subsidiary of Jube Holdings Limited. Jube Operations Limited
          provides training and support services for Jube. Jube and "Jooby" (the logo) is a registered trademark in Cyprus.

# Licence

Jube is distributed under [AGPL-3.0-or-later](https://www.gnu.org/licenses/agpl-3.0.txt).