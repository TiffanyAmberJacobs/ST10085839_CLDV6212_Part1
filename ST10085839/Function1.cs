using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System;


public static class Vaccination
{
    [FunctionName("Vaccination")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "id/{id}")] HttpRequest req,
        string id,
        ILogger log)
    {
        try
        {
            string validIdString = Environment.GetEnvironmentVariable("ValidId");
            var validId = validIdString.Split(',');

            var matchingIdData = validId
                .Select(validId =>
                {
                    var idParts = validId.Split(':');
                    return new { ID = idParts[0], VaccinationData = idParts[1] };
                })
                .FirstOrDefault(v => v.ID.Equals(id));

            if (matchingIdData != null)
            {
                return new OkObjectResult(JsonConvert.SerializeObject(matchingIdData));
            }

            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            log.LogError(ex, "An error occurred.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
