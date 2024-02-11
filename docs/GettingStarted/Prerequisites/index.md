---
layout: default
title: Prerequisites
nav_order: 1
parent: Getting Started
---

# Prerequisites

The README file in the head of the project provides for a Quickstart that assumes some ability to provision servers, databases and operating system dependencies:

```shell
git clone https://github.com/jube-home/jube.git
cd jube/Jube.App
export ConnectionString="Host=<host>;Port=<port>;Database=<defaultdb>;Username=<username>;Password=<password>;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;SSL Mode=Require;Trust Server Certificate=true;"
export ASPNETCORE_URLS="https://localhost:5001"
dotnet run
```

This section of the documentation makes no assumption about technical ability and provides a step by step approach to getting started with Jube on [DigitalOcean](https://m.do.co/c/8be72e86abb2) servers.  [DigitalOcean](https://m.do.co/c/8be72e86abb2) is an enterprise hosting provide much like AWS, Azure or Google Cloud Platform.  Digital [DigitalOcean](https://m.do.co/c/8be72e86abb2) has a much simpler and focussed offer,  specifically very well priced high performance Virtual Machines running commodity Linux and Managed PostgreSQL, and when taken together greatly simplify the infrastructure and reduce cost.  [DigitalOcean](https://m.do.co/c/8be72e86abb2) also offer a free $200 credit on being refered from Jube documentation, which is more than enough to complete testing and proof of concept.

Jube has the following prerequisites:

* .Net 8 Runtime.
* Postgres database version 13 onwards (tested on 15.4 but no significant database development to cause a breaking change).

Create an account on [DigitalOcean](https://m.do.co/c/8be72e86abb2).  Once the account is created and navigate to the page as follows:

![Image](ProjectPage.png)

Creating a new project if necessary:

![Image](LocationOfNewProject.png)

The installation assumes two servers as follows:

* General Purpose Droplet (Virtual Machine) 8GB RAM / 2vCPU.
* Managed PostgreSQL Database 4GB RAM / 2vCPU.

Putting aside high availability and storage, this configuration will be more than adequate for the purpose of testing and proof of concept.