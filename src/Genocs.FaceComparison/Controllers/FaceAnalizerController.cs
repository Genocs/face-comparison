using Genocs.FaceComparison.Dto;
using Genocs.FaceComparison.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genocs.FaceComparison.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class FaceAnalizerController : ControllerBase
    {
        private readonly FaceReconizerService faceReconizerService;
        private readonly StorageService storageService;

        public FaceAnalizerController(FaceReconizerService faceReconizerService, StorageService storageService)
        {
            this.faceReconizerService = faceReconizerService ?? throw new ArgumentNullException(nameof(faceReconizerService));
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        /// <summary>
        /// It allows to compare at least two images
        /// </summary>
        /// <param name="files">Images files</param>
        /// <returns>Result</returns>
        [Route("compare"), HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<FaseComparisonResult> PostCompareImages([FromForm(Name = "images")] List<IFormFile> files)
        {
            var uploadResult = await this.storageService.UploadFilesAsync(files);
            IList<Microsoft.Azure.CognitiveServices.Vision.Face.Models.SimilarFace> similarResult = await this.faceReconizerService.FindSimilar(uploadResult.First().URL, uploadResult.Last().URL);

            return await Task.Run(() => {
                FaseComparisonResult result = new();
                if (similarResult != null && similarResult.Any())
                {
                    result.Confidence = similarResult.OrderByDescending(c => c.Confidence).First().Confidence;
                }
                return result;
            });
        }
    }
}
