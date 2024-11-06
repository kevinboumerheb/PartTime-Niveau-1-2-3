using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xrm.Tooling.Connector;
using Kev.Xrm.Service;
using Kev.Xrm.EntityWrappers;
using Kev.Xrm.Utilities;

namespace Kev.Xrm.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Specify the log file path relative to the application's directory
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string correctDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;
            string logFilePath = Path.Combine(correctDirectory, "log.txt");
            
            // Clear the log file at the beginning of the program
            File.WriteAllText(logFilePath, ""); // Clear the log file


            // Connect to Dynamics 365
            string connectionString = "AuthType=Office365;" +
                                      "Username=Kevin@xj7c0.onmicrosoft.com;" +
                                      "Password=3196546136Ipad12345:);" +
                                      "Url=https://orgb7665afa.crm.dynamics.com/;";

            // Initialize the CrmServiceClient
            CrmServiceClient service = new CrmServiceClient(connectionString);
            
            if (service.IsReady)
            {
                //LogAndDisplayHelper.LogAndDisplay("Connected to Dynamics 365 successfully.", logFilePath);

                ContactService contactService = new ContactService(service, service);

                // 1. Retrieve the list of contacts
                List<Contact> contacts = contactService.RetrieveContacts();

                // 2. Filter active contacts
                List<Contact> activeContacts = contacts
                    .Where(c => c.StateCode != null && (int)c.StateCode == 0)
                    .ToList();

                // 3. (OPTIONAL) Filter inactive contacts
                List<Contact> inactiveContacts = contacts
                    .Where(c => c.StateCode != null && (int)c.StateCode == 1)
                    .ToList();

                // 4. Display the count of active contacts
                LogAndDisplayHelper.LogAndDisplay($"Number of active contacts: {activeContacts.Count}", logFilePath);

                // 5. (OPTIONAL) Display the count of inactive contacts
                LogAndDisplayHelper.LogAndDisplay($"Number of inactive contacts: {inactiveContacts.Count}", logFilePath);

                // 6. Display the count of contacts with age > 18
                int adultContactsCount = activeContacts.Count(c => c.Contains("birthdate") && contactService.CalculateAge((DateTime)c["birthdate"]) > 18);
                LogAndDisplayHelper.LogAndDisplay($"Number of contacts with age > 18: {adultContactsCount}", logFilePath);

                // 7. (OPTIONAL) Display the count of contacts with age < 18
                int minorContactsCount = activeContacts.Count(c => c.Contains("birthdate") && contactService.CalculateAge((DateTime)c["birthdate"]) < 18);
                LogAndDisplayHelper.LogAndDisplay($"Number of contacts with age < 18: {minorContactsCount}", logFilePath);

                // 8. Display each contact type dynamically with counts
                contactService.DisplayContactTypes(activeContacts, logFilePath);

                // 9. Update Contacts 
                //contactService.UpdateContactsWithCityLookup(activeContacts);

                // 9. Update Contacts with ExecuteMultipleRequest
                contactService.UpdateContactsWithCityLookupUsingExecuteMultiple(activeContacts, logFilePath);

                // 9. Create Contacts with ExecuteMultipleRequest and measure execution time
                contactService.RunComparison(logFilePath);

                LogAndDisplayHelper.LogAndDisplay("Process completed.", logFilePath);
            }
            else
            {
                LogAndDisplayHelper.LogAndDisplay("Connection failed.", logFilePath);
            }
        }
    }
}