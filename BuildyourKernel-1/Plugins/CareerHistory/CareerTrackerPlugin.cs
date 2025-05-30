using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BuildyourKernel_1.Plugins.CareerHistory
{
     public class CareerTrackerPlugin
    {
        [KernelFunction, Description("Get a list of the jobs worked during the user's career")]
        public static string GetCareerhistory()
        {
            string dir = Directory.GetCurrentDirectory();
            string content = File.ReadAllText($"{dir}/data/recentJobs.txt");
            return content;
        }

        [KernelFunction, Description("Add a new job title to the career history list")]
        public static string AddToCareerHistoryList(
            [Description("The job title of the user")] string title,
            [Description("The company")] string company,
            [Description("The rank of the job title")] string rank)
        {
            string content = File.ReadAllText("data/recentjobs.txt");
            var careerHistory = (JsonArray) JsonNode.Parse(content);

            var newJobTitle = new JsonObject
            {
                ["title"] = title,
                ["company"] = company,
                ["rank"] = rank
            };

            careerHistory.Insert(0, newJobTitle);
            
            File.WriteAllText($"data/recentjobs.txt", JsonSerializer.Serialize(
                careerHistory, new JsonSerializerOptions { WriteIndented = true }
                ));

            return $"Added '{title}' to career History";
        }
    }
}
