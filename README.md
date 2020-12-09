**ZptSharp** *(formerly ZPT-Sharp)* is a library for MVC-style binding to
HTML and/or XML document templates. It is *attribute-based* and avoids the
need to add ['alien markup'] which make template files hard to read/understand
and impossible for some tools to work-upon.

## Compatibility & dependencies
ZptSharp is compatible with the following framework targets.  The library
is *multi-targeted* to avoid compatibility issues.

* **.NET Framework 4.6.1** and up: `net461`
* **.NET Standard 2.0** and up: `netstandard2.0`
* **.NET Core 2.0** and up *(by virtue of supporting .NET Standard 2.0)*
* **.NET 5** and up *(by virtue of supporting .NET Standard 2.0)*

## Continuous integration status
In this repository the **master** branch represents the latest development code
(some repositories would call this branch *develop*).  The **production** branch
is the latest released version (some repositories would call this branch *master*).
The status badges below show the status of the master branch:

| Environment   | Status |
| ------------- | ------ |
| Windows       | [![AppVeyor (Windows) build status](https://ci.appveyor.com/api/projects/status/apc1gw18xjkr2fn3/branch/master?svg=true)](https://ci.appveyor.com/project/craigfowler/zpt-sharp/branch/master) |
| Linux         | [![Travis (Linux) build status](https://api.travis-ci.org/csf-dev/ZPT-Sharp.svg?branch=master)](https://travis-ci.org/github/csf-dev/ZPT-Sharp) |
| Code quality  | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ZptSharp&metric=alert_status)](https://sonarcloud.io/dashboard?id=ZptSharp) |
| Test coverage | [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=ZptSharp&metric=coverage)](https://sonarcloud.io/dashboard?id=ZptSharp) |