---
layout: default
title: Welcome
nav_order: 1
---

![Image](logo.png)

# About Jube

Jube is open-source transaction and event monitoring software. Jube implements real-time data wrangling, artificial intelligence, decision making and case management. Jube is particularly strong when implemented in fraud and abuse detection use cases.

Data wrangling is real-time. Data wrangling is directed via a series of rules created using either a point-and-click rule builder or an intuitive rule coder. Rules are in-memory matching functions tested against data returned from high-performance cache tables, where datasets are fetched only once for each key that the rules roll up to for each transaction or event processing, with the matches aggregating using a variety of functions. Alternative means of maintaining a lightweight long-term state to facilitate data wrangling is Time To Live (TTL) Counters which are incremented on rule match and then decremented for that incrementation on time-lapse.

Data wrangling return values are independently available for use as features in artificial intelligence training and real-time recall or tested by rules to perform a specific action (e.g., the rejection of a transaction or event). Wrangled values are returned in the real-time response payload and can facilitate a Function as a Service (FaaS) pattern. Response payload data is also stored in an addressable fashion, improving the experience of advanced analytical reporting while also reducing database resource \ compute cost.

Jube is developed statelessly and can support massive horizontal scalability and separation of concerns in the infrastructure.

Jube takes a novel approach to artificial intelligence, ultimately Supervised Learning, yet blending anomaly detection with confirmed class data to ensure datasets of sufficient amounts of class data. Using data archived from its processing, Jube searches for optimal input variables, hidden layers and processing elements. The result is small, optimal, generalised and computationally inexpensive models for efficient real-time recall. The approach taken by Jube allows artificial intelligence's benefits to be available very early in an implementation's lifecycle. It avoids over-fitting models to typology long since passed.

Transaction or event monitoring overlooks the embedding in human-engaging business processes. Jube is real-time, but this does not forgo the need for manual intervention; hence Jube makes comprehensive and highly customisable case management and visualisation intrinsically available in the user interface.

To ensure the segregation of user responsibilities, a user, role and permission model is in place, which controls access to each page within the Jube user interface. Detailed audit logs are available. Any update to a configuration by a user retains a version history,  in which the original is logically deleted and then replaced with the new version.

Jube is multi-tenanted,  allowing a single infrastructure to be shared among many logically isolated entities, maintaining total isolation between tenant data with no loss of function in the user interface.

# Quickstart
Jube runs on commodity Linux. The Quickstart has the following prerequisites:

* .Net 6 SDK.
* PostgreSQL database version 13 onwards.

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

# Community support
For general support, refer to the [documentation](https://jube-home.github.io/jube).

For additional community support, the following resources are available:

* [Book a free a 30 minute demonstration](https://calendly.com/richard-churchman/30min) with the developer.
* Use the #jube:matrix.org [\[Matrix\] Room](https://matrix.to/#/#jube:matrix.org) for support or discussion.
* Use GitHub for bug reports.

Commercial support is available via a GitHub Sponsors monthly tier.

# Reporting Vulnerabilities

Please do not file GitHub issues or post in the #jube:matrix.org [\[Matrix\] Room](https://matrix.to/#/#jube:matrix.org) for security vulnerabilities, as they are public.

Jube takes security issues very seriously. If you have any concerns about Jube or believe you have uncovered a vulnerability, please contact via the e-mail address security@jube.io. In the message, try to describe the issue and, ideally, a way of reproducing it.

Please report any security problems to Jube before disclosing them publicly.

# About Documentation
The [documentation](https://jube-home.github.io/jube) has been drafted to include all features, and there should not be any undocumented know-how.  The documentation adopts an instructional style that will explain most features step-by-step with extensive use of screenshots.  The documentation, where possible, has been written to support an Educate, Demonstrate, Imitate and Practice (EDIP) style.  Given the EDIP style, an excellent approach to using the documentation is:

* Educate: Read the topic from start to finish.
* Demonstrate: Read the topic and pause on each instruction and;
* Imitate: A read of the topic following the instructions step by step.
* Practice: Without referring to the documentation, practice using the topic,  varying parameters as appropriate.

Jube is committed to high-quality instructional documentation and maintains it as part of the overall release methodology.  If documentation is inadequate,  unclear or missing, it would be a great pleasure to receive such feedback as a Github Bug.

Commercial training is available via GitHub Sponsors one-time tier.

# Governance
Jube Holdings Limited is a Cyprus company registered HE404521. Jube Holdings Limited owns Jube and its trademark. Jube is maintained by Jube Operations Limited, a United Kingdom company with registration 14442207. Jube Operations Limited is a wholly owned subsidiary of Jube Holdings Limited. Jube Operations Limited provides training and support services for Jube via Github Sponsors tiers.

# Licence
Jube is distributed under [AGPL-3.0-only](https://www.gnu.org/licenses/agpl-3.0.txt). 
