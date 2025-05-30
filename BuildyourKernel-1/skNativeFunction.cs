using BuildyourKernel_1.Plugins.CareerHistory;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildyourKernel_1
{
     public   class SKNativeFunction
    {
        public async Task run(Kernel kernel)
        {
            kernel.ImportPluginFromType<CareerTrackerPlugin>();

            Console.WriteLine("Enter Job Title: ");
            string title = Console.ReadLine();

            Console.WriteLine("Enter Company Name: ");
            string companyName = Console.ReadLine();

            var validRanks = new HashSet<string> { "Employee", "Supervisor", "Manager" };
            string rank = String.Empty;

            while(!validRanks.Contains(rank))
            {
                Console.WriteLine("Enter Rank (Employee, Supervisor, Manager): ");
                rank = Console.ReadLine();
                if (!validRanks.Contains(rank))
                {
                    Console.WriteLine("Invalid rank. Please enter a valid rank.");
                }
            }

            var result = await kernel.InvokeAsync("CareerTrackerPlugin", "AddToCareerHistoryList",
                new()
                {
                    { "title", title },
                    { "company", companyName },
                    {
                "rank", rank } }
                );

            Console.WriteLine(result);

            var result1 = await kernel.InvokeAsync("CareerTrackerPlugin", "GetCareerhistory");
            Console.WriteLine(result1);
        }
    }
}
