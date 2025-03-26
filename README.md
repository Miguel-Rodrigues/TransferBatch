# Transfer batch processor

A simple CLI utility that processes commissions from a transaction spreadsheet

This c# console application calculates the total commissions that should be charged for each
account on a given day, with the following rules:
* Accounts should be charged by 10% of the total value on every transfer
* The transfer with the highest value of the day will not be subject to commission

Given a CSV file with the amount of outbound transfers for each account for a given day, with the
following structure: `<Account_ID>`,`<Transfer_ID>`,`<Total_Transfer_Amount>`


## Build
To build this project you need to have the [.NET 8 SDK](https://dotnet.microsoft.com/download)
installed. Make sure you have an internet connection available to download the nuget dependency
packages during build.

Then you can run the following command in the project root folder:

`dotnet build`

Or you can use the Visual Studio IDE to build the project, by pressing `Ctrl+Shift+B`, or go menu
`Build -> Build Solution`, or press the "Build Solution" button at the Build toolbar..

## Run
After building the project, you can run the application from the command line with the following
command:
```
TransferBatch <Path_to_transfers_file>
```

## Example
Given the input `transfers.csv` file:
```
A10,T1000,100.00
A11,T1001,100.00
A10,T1002,200.00
A10,T1003,300.00
```

The application will output the following result to the console.
```
$ TransferBatch transfers.csv
A10,30
A11,10
```

## CSV generation
For example purposes you use the same application to generate a random CSV file with test transfers
data by running the following command:

```Shell
TransferBatch GenerateSample [<Path_to_file>] [-NumberOfAccounts 10] [-FileSizeLimit 10000] [-NumberOfWorkers <Nr_of_CPUs>]
```

Where the parameters are:
* `Path_to_file`:     The path to the file to be generated.
* `NumberOfAccounts`: The number of accounts to be generated. By default is 10.
* `FileSizeLimit`:    The maximum size of the file in bytes. By default is 100000 bytes (100KB).
* `NumberOfWorkers`:  The number of threads to be used to generate the file. By default is the
                      number of CPUs available in the machine.

## Tests
To run the tests you can use the following command in the project root folder:

`dotnet test`

Or you can use the Visual Studio IDE to run the tests, by pressing `Ctrl+R, A`, or go menu
`Test -> Run All Tests`, or press the top left green play button of the Test Explorer Window.