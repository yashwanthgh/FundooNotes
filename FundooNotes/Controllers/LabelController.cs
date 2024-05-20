using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.NotesModel;
using ModelLayer.Response;
using RepositoryLayer.Entities;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelBL _label;
        private readonly ILogger<LabelController> _logger;

        public LabelController(ILabelBL label, ILogger<LabelController> logger)
        {
            _label = label;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("addLabel")]
        public async Task<IActionResult> AddLabel(CreateLabelModel label)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);
                await _label.CreateLabel(label, userId);
                var response = new ResponseStringModel
                {
                    Success = true,
                    Message = "Label created "

                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseDataModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return NotFound(response);
            }
        }

        [Authorize]
        [HttpDelete("deleteLabel/{labelId}")]
        public async Task<IActionResult> Removelabel(int labelId)
        {
            try
            {
                await _label.DeleteLabel(labelId);
                var response = new ResponseStringModel
                {
                    Success = true,
                    Message = "Label deleted"

                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = $"An error occurred while updating the notes {ex.Message}.",
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpPut("upadteLabel/{labelId}")]
        public async Task<IActionResult> UpdateLabel(CreateLabelModel label, int labelId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);

                await _label.UpdateLabel(label, labelId, userId);
                var response = new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "Label Updated",
                    Data = null

                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = $"An error occurred while updating the notes.{ex.Message}",
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpGet("showLabels")]

        public async Task<IActionResult> GetAllLabelbyId()
        {
            try
            {
                _logger.LogInformation("Get notes by Id");
                var label = await _label.GetAllLabels();
                return Ok(new ResponseDataModel<IEnumerable<Label>>
                {
                    Success = true,
                    Message = "Label retrieved successfully",
                    Data = label
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = $"An error occurred while updating the notes.{ex.Message}",
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpGet("showNotes/{labelId}")]

        public async Task<IActionResult> GetAllNotebyId(int labelId)
        {
            try
            {
                var label = await _label.GetAllNotesbyId(labelId);
                return Ok(new ResponseDataModel<object>
                {
                    Success = true,
                    Message = "Label retrieved successfully",
                    Data = label
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = $"An error occurred while updating the notes.{ex.Message}",
                    Data = null
                });
            }
        }
    }
}
