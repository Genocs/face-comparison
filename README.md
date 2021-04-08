# face-comparison

Web Api service built with .NET Core 5. It allows to compare two images finding similarity score.
The service need both a Azure Blob Storage and Cognitive Services account for face comparison. 

## Azure

The project require to have an Azure subscription with

- Storage account
- Cognitive Services

## All the variables can be setup using environment variables


``` PS
  "AzureStorageConfig": {
    "AccountName": "{{accountName}}",
    "AccountKey": "{{AccountKey}}",
    "ImageContainer": "{{ImageContainer}}",
    "ThumbnailContainer": "{{ThumbnailContainer}}"
  },
  "AzureCognitiveServicesConfig": {
    "Endpoint": "{{Endpoint}}",
    "SubscriptionKey": "{{SubscriptionKey}}"
  }
```

Docker Compose
``` PS
    environment:
      - AzureStorageConfig__AccountName={{accountName}}
      - AzureStorageConfig__AccountKey={{AccountKey}}
      - AzureStorageConfig__ImageContainer={{ImageContainer}}
      - AzureStorageConfig__ThumbnailContainer={{ThumbnailContainer}}      
      - AzureCognitiveServicesConfig__Endpoint={{Endpoint}}
      - AzureCognitiveServicesConfig__SubscriptionKey={{SubscriptionKey}}
  
## Docker image

``` PS
docker build -t genocs/facecomparison -f .\src\Genocs.FaceComparison\Dockerfile .
docker push genocs/facecomparison
```
