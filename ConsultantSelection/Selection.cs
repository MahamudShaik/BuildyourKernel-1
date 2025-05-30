using ClosedXML.Excel;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantSelection
{
    public class Selection
    {
        public async Task run(Kernel kernel)
        {

            var consultants = ReadConsultants("consultants_1.xlsx");

            string consultantList = string.Join("\n", consultants.Select(c => $"{c.Name}, Skills: {string.Join(", ", c.Skills)}, Cost: {c.CostPerHour}"));

            Console.WriteLine("Enter number of consultants needed:");
            int numberOfConsultants = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter project budget:");
            decimal budget = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter required skills (comma separated):");
            var skillsRequired = Console.ReadLine();

            Console.WriteLine("Enter project duration (in days):");
            int projectDuration = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter project complexity (low, medium, high):");
            string complexity = Console.ReadLine().ToLower();

            // Prepare your prompt
            string prompt = @"
You are a Consultant Selection Assistant.
Select the best consultants from the given list for a new project based on the following criteria:

Consultants must have the required skills.
Consultants must fit within the total project budget.
Prefer more experienced consultants (higher designation) if project complexity is high, look for maximize profit.
Prefer less expensive consultants if project complexity is low to maximize profit.
Total working hours per day = 8 hours.
Project duration = {{$project_duration}} days.
Cost is calculated as: (cost/hour) × 8 × (duration).
Budget = {{$project_budget}}.
Profit = Budget - Total Cost of Consultants.
Profit should not be negative. If it is, return an error indicating that the selection exceeds the budget.
The main objective is to maximize profit while strictly following all the above instructions.

Return the following:
Selected consultants' names
Their designation
Their skills take from the list
Their total cost for the project
A detailed reason why each consultant was selected (based on skills, seniority, and cost)
The overall profit at the end

Consultants List:
{{$consultant_list}}
Requirements:
Skills Needed: {{$skills_required}}
Number of Consultants: {{$number_of_consultants}}
Project Complexity: {{$project_complexity}}
";


            var arguments = new KernelArguments
            {
                ["skills_required"] = skillsRequired,
                ["number_of_consultants"] = numberOfConsultants.ToString(),
                ["project_budget"] = budget.ToString(),
                ["project_duration"] = projectDuration.ToString(),
                ["project_complexity"] = complexity,
                ["consultant_list"] = consultantList
            };

            var result = await kernel.InvokePromptAsync(prompt, arguments);
            Console.WriteLine(result);
        }

        public static List<Consultant> ReadConsultants(string filePath)
        {
            var consultants = new List<Consultant>();
            var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1); // Assuming data is in first sheet
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                consultants.Add(new Consultant
                {
                    Name = row.Cell(1).GetString(),
                    Skills = row.Cell(2).GetString().Split(',').Select(s => s.Trim()).ToList(),
                    Designation = row.Cell(3).GetString(),
                    CostPerHour = decimal.Parse(row.Cell(4).GetString(), CultureInfo.InvariantCulture) // Fix: Parse string to decimal
                });
            }
            return consultants;
        }

        public class Consultant
        {
            public string Name { get; set; }
            public List<string> Skills { get; set; }
            public string Designation { get; set; }
            public decimal CostPerHour { get; set; }
        }
    }
}
