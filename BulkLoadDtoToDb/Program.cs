using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace BulkLoadDtoToDb
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var dbService = new DatabaseService(configuration);

            var employeesToLoad = new List<Loan>()
            {
                new Loan() {LoanId = 101, PrincipalRemaining = 10.12M, Status = "Live"},
                new Loan() { LoanId = 101, PrincipalRemaining = 10.12M, Status = "Live" }
            };

            dbService.UploadData(employeesToLoad);
            

        }
    }
}
