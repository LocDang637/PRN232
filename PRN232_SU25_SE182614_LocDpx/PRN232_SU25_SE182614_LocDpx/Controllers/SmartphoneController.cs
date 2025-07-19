using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PRN232_SU25_SE182614_Repository.Models;
using PRN232_SU25_SE182614_Service;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PRN232_SU25_SE182614_LocDpx.Controllers
{
    [Route("api/smartphones")]
    [ApiController]
    [Authorize]
    public class SmartphoneController : ControllerBase
    {
        private readonly ISmartphoneService _smartphoneService;
        private readonly BrandService _brandService;

        public SmartphoneController(ISmartphoneService smartphoneService, BrandService brandService)
        {
            _smartphoneService = smartphoneService;
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                //administrator, moderator: fullCRUD search
                //developer, member: Read + search
                if (!IsAuthorizedRole("administrator", "moderator", "developer", "member"))
                {
                    return StatusCode(403, new { errorCode = "SP40301", message = "Permission denied" });
                }
                var smarphones = await _smartphoneService.GetAllAsync() ?? new List<Smartphone>();
                return Ok(smarphones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = "SP50001", Message = "Internal Server Error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                //administrator, moderator: fullCRUD search
                //developer, member: Read + search
                if (!IsAuthorizedRole("administrator", "moderator"))
                {
                    return StatusCode(403, new { errorCode = "SP40301", message = "Permission denied" });
                }
                var smarphones = await _smartphoneService.GetByIdAsync(id);
                if(smarphones == null)
                {
                    return NotFound(new { errorCode = "SP40401 ", Message = "Resource not found" });
                }
                return Ok(smarphones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = "SP50001", Message = "Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePhone request)
        {
            try
            {
                //administrator, moderator: fullCRUD search
                //developer, member: Read + search
                if (!IsAuthorizedRole("administrator", "moderator"))
                {
                    return StatusCode(403, new { errorCode = "SP40301", message = "Permission denied" });
                }
                var regex = @"^([A-Z0-9][a-zA-Z0-9+]*\s)*([A-Z0-9][a-zA-Z0-9+]*)$";
                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .FirstOrDefault()?.ErrorMessage ?? "Missing/invalid input";
                    return BadRequest(new { errorCode = "HB40001", message = firstError });
                }
                if(request.Stock <= 0)
                {
                    return BadRequest(new { errorCode = "HB40001", message = "Invalid stock" });

                }
                if (request.Price <= 0)
                {
                    return BadRequest(new { errorCode = "HB40001", message = "Invalid price" });

                }
                var brand = await _brandService.GetByIdAsync(request.BrandId);
                if (brand == null)
                {
                    return NotFound(new { errorCode = "SP40401 ", Message = "Brand not found" });
                }
                var created = new Smartphone {
                    BrandId = request.BrandId,
                    Color = request.Color,
                    ModelName = request.ModelName,
                    Price = request.Price,
                    ReleaseDate = request.ReleaseDate,
                    Stock = request.Stock,
                    Storage = request.Storage,
                    
                };
                var result = await _smartphoneService.CreateAsync(created);
                if(result == 0)
                {
                    return StatusCode(500, new { StatusCode = "SP50001", Message = "Internal Server Error" });
                }
                return StatusCode(201, new { Message = "Created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = "SP50001", Message = "Internal Server Error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePhone request)
        {
            try
            {
                //administrator, moderator: fullCRUD search
                //developer, member: Read + search
                if (!IsAuthorizedRole("administrator", "moderator"))
                {
                    return StatusCode(403, new { errorCode = "SP40301", message = "Permission denied" });
                }
                var smartphone = await _smartphoneService.GetByIdAsync(id);
                if (smartphone == null)
                {
                    return NotFound(new { errorCode = "SP40401 ", Message = "Resource not found" });
                }
                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .FirstOrDefault()?.ErrorMessage ?? "Missing/invalid input";
                    return BadRequest(new { errorCode = "HB40001", message = firstError });
                }
                if (request.Stock <= 0)
                {
                    return BadRequest(new { errorCode = "HB40001", message = "Invalid stock" });

                }
                if (request.Price <= 0)
                {
                    return BadRequest(new { errorCode = "HB40001", message = "Invalid price" });

                }
                var brand = await _brandService.GetByIdAsync(request.BrandId);
                if (brand == null)
                {
                    return NotFound(new { errorCode = "SP40401 ", Message = "Brand not found" });
                }
                smartphone.BrandId = request.BrandId;
                smartphone.Stock = request.Stock;
                smartphone.Storage = request.Storage;
                smartphone.Price = request.Price;
                smartphone.Color = request.Color;
                smartphone.ModelName = request.ModelName;
                smartphone.ReleaseDate = request.ReleaseDate;
                var result = await _smartphoneService.UpdateAsync(smartphone);
                if (result == 0)
                {
                    return StatusCode(500, new { StatusCode = "SP50001", Message = "Internal Server Error" });
                }
                return StatusCode(200, new { Message = "Updated resource successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = "SP50001", Message = "Internal Server Error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //administrator, moderator: fullCRUD search
                //developer, member: Read + search
                if (!IsAuthorizedRole("administrator", "moderator"))
                {
                    return StatusCode(403, new { errorCode = "SP40301", message = "Permission denied" });
                }
                var smartphone = await _smartphoneService.GetByIdAsync(id);
                if (smartphone == null)
                {
                    return NotFound(new { errorCode = "SP40401 ", Message = "Resource not found" });
                }
                var result = await _smartphoneService.DeleteAsync(smartphone);
                if (!result)
                {
                    return StatusCode(500, new { StatusCode = "SP50001", Message = "Internal Server Error" });
                }
                return StatusCode(200, new {Message = "Delete resource successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = "SP50001", Message = "Internal Server Error" });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? modelName, [FromQuery] string? storage)
        {
            try
            {
                //administrator, moderator: fullCRUD search
                //developer, member: Read + search
                if (!IsAuthorizedRole("administrator", "moderator", "developer", "member"))
                {
                    return StatusCode(403, new { errorCode = "SP40301", message = "Permission denied" });
                }
                var smarphones = await _smartphoneService.SearchAsync(modelName, storage);
                return Ok(smarphones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = "SP50001", Message = "Internal Server Error" });
            }
        }













        private bool IsAuthorizedRole(params string[] allowedRoles)
        {
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
                return false;

            var roleName = GetRoleName(int.Parse(userRole));
            return allowedRoles.Contains(roleName);
        }

        private static string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "administrator",
                2 => "moderator",
                3 => "developer",
                4 => "member",
                _ => "unknown"
            };
        }
    }

    public class CreatePhone
    {
        [Required(ErrorMessage ="BrandId is required")]
        public int BrandId { get; set; }
        [Required(ErrorMessage = "ModelName is required")]
        [RegularExpression(@"^[A-Z][a-zA-Z]*$", ErrorMessage = "ModelName is not correct format, each word must begin with capital, no number or special characters")]
        public string ModelName { get; set; } = null!;
        [Required(ErrorMessage = "Storage is required")]
        public string? Storage { get; set; }
        [Required(ErrorMessage = "Color is required")]
        public string? Color { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "Stock is required")]
        public int? Stock { get; set; }
        [Required(ErrorMessage = "ReleaseDate is required")]
        public DateOnly? ReleaseDate { get; set; }
    }
}
