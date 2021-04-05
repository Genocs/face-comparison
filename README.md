# face-comparison
Web Api service with Azure blob storage and Cognitive Services for face comparison.

## Azure 
The project require to have an Azure subscription with
- Storage account
- Cognitive Services

## Build docker image

docker build -t genocs/facecomparison -f .\src\Genocs.FaceComparison\Dockerfile .
docher push genocs/facecomparison