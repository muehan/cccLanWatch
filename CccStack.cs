using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.Storage;

class CccStack : Stack
{
    public CccStack()
    {
        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup("cccresourcegroup");

        // Create an Azure Storage Account
        var storageAccount = new Account("cccstorage", new AccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AccountReplicationType = "LRS",
            AccountTier = "Standard"
        });

        // Create AppService Plan
        var appServicePlan = new Plan("cccappplan", new PlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Kind = "FunctionApp",
            Sku = new PlanSkuArgs
            {
                Tier = "Dynamic",
                Size = "Y1",
            }
        });

        // Create Container
        var privateContainer = new Container("zips", new ContainerArgs
        {
            StorageAccountName = storageAccount.Name,
            ContainerAccessType = "private"
        });

         var publicContainer = new Container("client", new ContainerArgs
        {
            StorageAccountName = storageAccount.Name,
            ContainerAccessType = "private"
        });

        // Create BlobStorage
        var blob = new Blob("zip", new BlobArgs
        {
            StorageAccountName = storageAccount.Name,
            StorageContainerName = privateContainer.Name,
            Type = "Block",
            Source = new FileArchive("./functions/bin/Debug/netcoreapp3.1/publish")
        });

        var codeBlobUrl = SharedAccessSignature.SignedBlobReadUrl(blob, storageAccount);

        // Create FunctionApp
        var app = new FunctionApp("ccc-lan-watch-app", new FunctionAppArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AppServicePlanId = appServicePlan.Id,
            AppSettings =
            {
                {"runtime", "dotnet"},
                {"WEBSITE_RUN_FROM_PACKAGE", codeBlobUrl},
            },
            StorageAccountName = storageAccount.Name,
            StorageAccountAccessKey = storageAccount.PrimaryAccessKey,
            Version = "~3"
        });

        this.Endpoint = Output.Format($"https://{app.DefaultHostname}/api/index");
    }

    [Output]
    public Output<string> Endpoint { get; set; }
}
