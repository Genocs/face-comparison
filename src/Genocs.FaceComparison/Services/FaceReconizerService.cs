using Genocs.FaceComparison.Options;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Genocs.FaceComparison.Services
{
    public class FaceReconizerService
    {
        private readonly IFaceClient _client;
        private readonly ILogger<FaceReconizerService> _logger;

        public FaceReconizerService(IOptions<AzureCognitiveServicesConfig> config, ILogger<FaceReconizerService> logger)
        {
            _ = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _client = Authenticate(config.Value.Endpoint, config.Value.SubscriptionKey);
        }

        /// <summary>
        /// AUTHENTICATE
        /// Uses subscription key and region to create a client.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        private async Task<List<DetectedFace>> DetectFaceRecognize(string url)
        {
            // Detect faces from image URL. Since only recognizing, use the recognition model 1.
            // We use detection model 2 because we are not retrieving attributes.

            // Recognition model 3 was released in 2020 May.
            // It is recommended since its overall accuracy is improved
            // compared with models 1 and 2.
            IList<DetectedFace> detectedFaces = await _client.Face.DetectWithUrlAsync(url,
                                                                        recognitionModel: RecognitionModel.Recognition03,
                                                                        detectionModel: DetectionModel.Detection03);

            _logger.LogInformation($"{detectedFaces.Count} face(s) detected from image `{Path.GetFileName(url)}`");
            return detectedFaces.ToList();
        }

        /// <summary>
        /// FIND SIMILAR
        /// This example will take an image and find a similar one to it in another image.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<IList<SimilarFace>> FindSimilar(string firstImage, string secondImage)
        {
            _logger.LogInformation("========FIND SIMILAR========");

            List<string> targetImageFileNames = new List<string>
                        {
                            firstImage
                        };

            IList<Guid?> targetFaceIds = new List<Guid?>();
            foreach (var targetImageFileName in targetImageFileNames)
            {
                // Detect faces from target image url.
                var faces = await DetectFaceRecognize(targetImageFileName);
                // Add detected faceId to list of GUIDs.
                targetFaceIds.Add(faces[0].FaceId.Value);
            }

            // Detect faces from source image url.
            IList<DetectedFace> detectedFaces = await DetectFaceRecognize(secondImage);

            // Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
            return await _client.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds);
        }

    }
}
