using Genocs.FaceComparison.Services;
using Genocs.FaceComparison.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Genocs.FaceComparison.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FaceAnalizerController : ControllerBase
{
    private readonly ILogger<FaceAnalizerController> logger;
    private readonly FaceReconizerService faceReconizerService;
    private readonly StorageService storageService;

    public FaceAnalizerController(ILogger<FaceAnalizerController> logger, FaceReconizerService faceReconizerService, StorageService storageService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        var uploadResult = await storageService.UploadFilesAsync(files);
        IList<Microsoft.Azure.CognitiveServices.Vision.Face.Models.SimilarFace> similarResult = await faceReconizerService.FindSimilar(uploadResult.First().URL, uploadResult.Last().URL);


        FaseComparisonResult result = new();
        if (similarResult != null && similarResult.Any())
        {
            result.Confidence = similarResult.OrderByDescending(c => c.Confidence).First().Confidence;
        }
        return result;

    }


    /// <summary>
    /// It allows to compare two images as public available URL
    /// </summary>
    /// <param name="firstUrl">The First Image url</param>
    /// <param name="secondUrl">The Second Image url</param>
    /// <returns>Result</returns>
    [Route("compareresources"), HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<FaseComparisonResult> PostCompareImages([FromQuery] string firstUrl, [FromQuery] string secondUrl)
    {
        if (string.IsNullOrWhiteSpace(firstUrl))
        {
            throw new InvalidCastException("'firstUrl' cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(secondUrl))
        {
            throw new InvalidCastException("'secondUrl' cannot be null or empty");
        }

        IList<Microsoft.Azure.CognitiveServices.Vision.Face.Models.SimilarFace> similarResult = await faceReconizerService.FindSimilar(firstUrl, secondUrl);

        return await Task.Run(() =>
        {
            FaseComparisonResult result = new();
            if (similarResult != null && similarResult.Any())
            {
                result.Confidence = similarResult.OrderByDescending(c => c.Confidence).First().Confidence;
            }
            return result;
        });
    }
}
