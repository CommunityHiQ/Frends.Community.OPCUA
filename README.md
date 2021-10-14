# Frends.Community.Email

![MyGet](https://img.shields.io/myget/frends-community/v/Frends.Community.OPCUA) [![License: UNLICENSED](https://img.shields.io/badge/License-UNLICENSED-yellow.svg)](https://opensource.org/licenses/UNLICENSED) 

Frends task for reading and writing OPC UA data.

- [Installing](#installing)
- [Tasks](#tasks)
  - [Send Email](#sendemail)
  - [Read Email With IMAP](#reademailwithimap)
  - [Read Email From Exchange Server](#reademailfromexchangeserver)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

Clone the repository and build the package using the `dotnet pack` command.

Tasks
=====

## ReadTags

Reads a number of tags from OPC UA server.

NB! Does not support authentication yet.

## WriteTag

Writes a tag value to OPC UA server.

NB! Does not support authentication yet.

# License

This project is licensed under the MIT License - see the LICENSE file for details

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.OPCUA.git`

Rebuild the project

`dotnet build`

Run Tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version             | Changes                 |
| ---------------------| ---------------------|
| 1.0.0 | Initial version |
