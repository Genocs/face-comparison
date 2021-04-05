using Genocs.FaceComparison.Dto;
using Genocs.FaceComparison.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            var similarResult = await this.faceReconizerService.FindSimilar(uploadResult.First().URL, uploadResult.Last().URL);

            return await Task.Run(() => new FaseComparisonResult() { Confidence = similarResult.First().Confidence });
        }
    }

}
