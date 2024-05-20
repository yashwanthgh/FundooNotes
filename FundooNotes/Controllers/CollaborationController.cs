using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.CollaborationMadel;
using System.Security.Claims;
using ModelLayer.Response;
using ModelLayer.CollaborationModel;

namespace FundooNotes.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CollaborationController : ControllerBase
    {
        private readonly ICollaborationBL _collaboration;
        private readonly ILogger<CollaborationController> _logger;

        public CollaborationController(ICollaborationBL collaboration, ILogger<CollaborationController> logger)
        {
            _collaboration = collaboration;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("addColleboration/{noteid}")]

        public async Task<IActionResult> AddCollaborator(int noteid, [FromBody] CollaborationCreateModel model)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);
                await _collaboration.AddCollaborator(noteid, model, userId);
                _logger.LogInformation("Collaborator Added");
                var response = new ResponseStringModel
                {
                    Success = true,
                    Message = "Collaboration Successfull",

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
        [HttpDelete("deleteCollaboration/{collaborationId}")]

        public async Task<IActionResult> RemoveCollaborator(int collaborationId)
        {
            try
            {
                await _collaboration.RemoveCollaborator(collaborationId);
                var response = new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "Collaborator removed successfully",
                    Data = null
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid Request {ex.Message}");
                var response = new ResponseDataModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return BadRequest(response);
            }
        }

        [Authorize]
        [HttpGet("getCollaborations")]

        public async Task<IActionResult> GetCollaboration()
        {
            try
            {
                var collab = await _collaboration.GetCollaboration();
                var response = new ResponseDataModel<IEnumerable<CollaborationInfoModel>>
                {
                    Success = true,
                    Message = "Collaborators Fetched Successfully",
                    Data = collab
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid Request {ex.Message}");
                var response = new ResponseStringModel
                {
                    Success = false,
                    Message = ex.Message,

                };
                return Ok(response);

            }
        }
    }
}
