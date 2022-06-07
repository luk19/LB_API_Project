// Title: API Project for US Federal Holidays
// Start Date: June 4th, 2022
// Developer: Luke Browning


// Imported Libraries
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;


namespace ProjectFile
{
    class Program
    {
        
        // static JsonResult GetHolidayData(string dateSelected)  
        static string GetHolidayData(string dateSelected, string myCurrentDirectory)  
        {
            // JObject my_json_object = JObject.Parse(json);
            // Console.WriteLine(my_json_object);
            
            
            // SQLite database connection
            string databaseName = "Luke_Brownings_Database";
            string databasePathName = myCurrentDirectory + "/" + databaseName;
            Console.WriteLine("Opening connection to " + databaseName);
            SQLiteConnection db = new SQLiteConnection("Data Source=" + databasePathName);
            db.Open();

            // Grab DB results with selected date
            dateSelected = dateSelected.Substring(0, 10);
            string selectStatement = "select * from US_FED_HOLIDAYS WHERE Date = '" + dateSelected + "'";
            SQLiteCommand selectCommand = new SQLiteCommand(selectStatement, db);
            SQLiteDataReader dataReader = selectCommand.ExecuteReader();
            DataTable dt = new DataTable("US_FED_HOLIDAYS_TABLE");
            dt.Load(dataReader);
            
            // Iterate through rows in the returned SQL data and determine if selected date is a holiday
            bool wasDataReturned = false;
            string masterString = "";
            foreach(DataRow row in dt.Rows)
            {
                wasDataReturned = true;
                string ID = row["ID"].ToString();
                string Date = row["Date"].ToString();
                string Name = row["Name"].ToString();
                string Description = row["Description"].ToString();
                string Fixed_or_Floating = row["Fixed_or_Floating"].ToString();
                masterString = "{ID: " + ID + ", isHoliday: " + wasDataReturned + ", Date: '" + Date + "', Name: '" + Name + "', Description: '" + Description + "', Fixed_or_Floating: '" + Fixed_or_Floating + "'}";
                Console.WriteLine(masterString);
            }
            if (wasDataReturned)
            {
                Console.WriteLine("SQL command returned data.. Date is a holiday!");
            }
            else
            {
                masterString = "{isHoliday: " + wasDataReturned + ", Date: '" + dateSelected + "'}";
                Console.WriteLine("SQL command returned nothing.. Date is not a holiday!");
            }
            
            // Close database connection
            Console.WriteLine("Closing connection to " + databaseName);
            db.Close();
            
            return masterString;
        }


        static void Main(string[] args)
        {
            // Get current project path
            string myCurrentDirectory = Environment.CurrentDirectory;
            
            // Simple API Build & Run
            var app = WebApplication.CreateBuilder(args).Build();
            app.MapGet("/", () => "Hello World!");
            app.MapGet("/isHoliday/{dateSelected}", (string dateSelected) =>
            {
                string holidayResults = Program.GetHolidayData(dateSelected, myCurrentDirectory);
                return Results.Text(holidayResults, contentType: "application/json");
            });
            app.Run();
        }
    }
}
