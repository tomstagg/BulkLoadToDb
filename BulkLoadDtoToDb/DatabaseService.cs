using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;


namespace BulkLoadDtoToDb
{
    public class DatabaseService
    {
        private string _connectionString;

        public DatabaseService(IConfiguration config)
        {
            _connectionString = config["connectionString"];
        }

        public void UploadData(IEnumerable<Loan> employees)
        {
            using (SqlConnection targetConnection = new SqlConnection(_connectionString))
            {
                targetConnection.Open();

                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Loan));

                DataTable table = new DataTable();

                foreach (PropertyDescriptor property in properties)
                {
                    Console.WriteLine($"{property.Name} with type {Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType}");
                    table.Columns.Add(property.Name,
                        Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }

                //table.Columns.Add("LoanId", typeof(int));
                //table.Columns.Add("PrincipalRemaining",typeof(decimal));
                //table.Columns.Add("Status", typeof(string));

                foreach (var emp in employees)
                {
                    DataRow row = table.NewRow();

                    foreach (PropertyDescriptor property in properties)
                    {
                        row[property.Name] = property.GetValue(emp) ?? DBNull.Value;
                    }

                    //row["LoanId"] = emp.LoanId;
                    //row["PrincipalRemaining"] = emp.PrincipalRemaining;
                    //row["Status"] = emp.Status;
                    table.Rows.Add(row);
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(targetConnection);

                foreach (PropertyDescriptor property in properties)
                {
                    SqlBulkCopyColumnMapping mapping = new SqlBulkCopyColumnMapping(property.Name, property.Name);
                    bulkCopy.ColumnMappings.Add(mapping);
                }

                bulkCopy.DestinationTableName = "dbo.Loan";
                bulkCopy.WriteToServer(table);

                Console.WriteLine("end");

            }
        }


    }
}