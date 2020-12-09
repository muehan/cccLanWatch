using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Functions
{
    public static class RegistrationApi
    {
        private const string PartitionKey = "REGISTRATION";

        [FunctionName("CreateRegistration")]
        public static async Task<IActionResult> CreateRegistration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "registration")] HttpRequest req,
            [Table("registration", Connection = "AzureWebJobsStorage")] IAsyncCollector<RegistrationTableEntity> registrationTable,
            ILogger log)
        {
            log.LogInformation("Creating a registration");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<RegistrationCreateModel>(requestBody);

            var registration = new Registration(input);
            await registrationTable.AddAsync(registration.ToTableEntity());

            return new OkObjectResult(registration);
        }

        [FunctionName("UpdateRegistration")]
        public static async Task<IActionResult> UpdateRegistration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "registration/{id}")] HttpRequest req,
            [Table("registration", Connection = "AzureWebJobsStorage")] CloudTable registrationTable,
            ILogger log,
            string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<RegistrationUpdateModel>(requestBody);

            if(updated.End == null){
                updated.End = DateTime.UtcNow;
            }

            log.LogInformation($"update registration with id: {id}, End: {updated.End}");

            var findOperation = TableOperation.Retrieve<RegistrationTableEntity>(PartitionKey, id);
            var findResult = await registrationTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new NotFoundResult();
            }

            var existingRow = (RegistrationTableEntity)findResult.Result;

            existingRow.End = updated.End?.ToString("yyyy-MM-ddTHH:mm:ss");

            var replaceOperation = TableOperation.Replace(existingRow);
            await registrationTable.ExecuteAsync(replaceOperation);

            return new OkObjectResult(existingRow.ToRegistraion());
        }

        [FunctionName("GetRegistrations")]
        public static async Task<IActionResult> GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registration")] HttpRequest req,
            [Table("registration", Connection = "AzureWebJobsStorage")] CloudTable registrationTable,
            ILogger log)
        {
            log.LogInformation("Getting all todos");
            var query = new TableQuery<RegistrationTableEntity>();
            var segment = await registrationTable.ExecuteQuerySegmentedAsync(query, null);

            return new OkObjectResult(segment.Select(Mappings.ToRegistraion));
        }

        [FunctionName("GetRegistrationById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registration/{id}")] HttpRequest req,
            [Table("registration", PartitionKey, "{id}", Connection = "AzureWebJobsStorage")] RegistrationTableEntity entity,
            ILogger log,
            string id)
        {
            log.LogInformation("Getting todo item by id");
            if (entity == null)
            {
                log.LogInformation($"item with id: {id} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(entity.ToRegistraion());
        }
    }

    public class RegistrationUpdateModel
    {
        public DateTime? End { get; set; }
    }

    public class RegistrationCreateModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string FromMember { get; set; }
    }

    public class Registration
    {
        public Registration() { }

        public Registration(
            RegistrationCreateModel createModel
        )
        {
            this.Name = createModel.Name;
            this.Type = createModel.Type;
            this.FromMember = createModel.FromMember;
            this.Start = DateTime.UtcNow;
        }

        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public string Name { get; set; }
        public string Type { get; set; }
        public string FromMember { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }

    public class RegistrationTableEntity : TableEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string FromMember { get; set; }
        public DateTime Start { get; set; }
        //https://social.msdn.microsoft.com/Forums/en-US/6b78647b-9117-48d9-90b2-653ce1f34448/table-storage-source-with-nullable-datetime-property-does-not-work-if-property-value-of-first-table?forum=AzureDataFactory
        public string End { get; set; }
    }

    public static class Mappings
    {
        public static RegistrationTableEntity ToTableEntity(
            this Registration registration)
        {
            var entity = new RegistrationTableEntity
            {
                PartitionKey = "REGISTRATION",
                RowKey = registration.Id,
                Name = registration.Name,
                Type = registration.Type,
                FromMember = registration.FromMember,
                Start = registration.Start,
                End = registration.End?.ToString("yyyy-MM-ddTHH:mm:ss"),
            };

            return entity;
        }

        public static Registration ToRegistraion(
            this RegistrationTableEntity entity)
        {
            var registration = new Registration
            {
                Id = entity.RowKey,
                Name = entity.Name,
                Type = entity.Type,
                FromMember = entity.FromMember,
                Start = entity.Start,
                End = DateTime.TryParse(entity.End, out DateTime date) ? date : default(DateTime?),
            };

            return registration;
        }
    }
}