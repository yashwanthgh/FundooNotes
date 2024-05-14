using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer.Notes;
using ModelLayer.NotesModel;
using ModelLayer.Response;
using Newtonsoft.Json;
using RepositoryLayer.Interfaces;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesBL _notes;
        private readonly IDistributedCache _cache;
        private readonly ILogger<NotesController> _logger;

        public NotesController(INotesBL notes, IDistributedCache cache, ILogger<NotesController> logger)
        {
            _notes = notes;
            _cache = cache;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("create-note")]
        public async Task<IActionResult> CreateNote(CreateNoteModel createNote)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);
                await _notes.CreateNote(createNote, userId);
                _logger.LogInformation("Note created!");
                var response = new ResponseStringModel
                {
                    Success = true,
                    Message = "Note Created Successfully",
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
                return Ok(response);
            }
        }

        [Authorize]
        [HttpGet("show-notes")]
        public async Task<IActionResult> DisplayNote()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);
                var key = $"Notes_{userId}";
                await _cache.RemoveAsync(key);
                var cachedNote = await _cache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(cachedNote))
                {
                    var notesList = JsonConvert.DeserializeObject<List<NoteResponse>>(cachedNote);
                    var response = new ResponseDataModel<IEnumerable<NoteResponse>>
                    {
                        Success = true,
                        Message = "Note Fetched Successfully from cache",
                        Data = notesList
                    };
                    return Ok(response);
                }

                var notes = await _notes.GetAllNotes(userId);
                if (notes != null)
                {
                    var serializedNote = JsonConvert.SerializeObject(notes);
                    await _cache.SetStringAsync(key, serializedNote, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                    });

                    var response = new ResponseDataModel<IEnumerable<NoteResponse>>
                    {
                        Success = true,
                        Message = "Note Fetched Successfully from DB",
                        Data = notes
                    };
                    return Ok(response);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpPut("update-using {noteId}")]
        public async Task<IActionResult> UpdateNote(int noteId, CreateNoteModel update)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);
                await _notes.UpdateNote(noteId, userId, update);
                await _cache.RemoveAsync($"Note_{noteId}");
                var response = new ResponseDataModel<NoteResponse>
                {
                    Success = true,
                    Message = "Note updated successfully",
                    Data = null

                };

                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [Authorize]
        [HttpDelete("detele-using {noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            try
            {

                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);
                await _notes.DeleteNote(noteId, userId);

                return Ok(new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "Note deleted successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {

                return NotFound(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpGet("show-archived")]
        public async Task<IActionResult> GetArchivedNotes()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);
                var key = $"Notes_{userId}";
                await _cache.RemoveAsync(key);
                var cachedNote = await _cache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(cachedNote))
                {
                    var notesList = JsonConvert.DeserializeObject<IEnumerable<NoteResponse>>(cachedNote);
                    var response = new ResponseDataModel<IEnumerable<NoteResponse>>
                    {
                        Success = true,
                        Message = "Note Fetched Successfully from cache",
                        Data = notesList
                    };
                    return Ok(response);
                }

                var notes = await _notes.GetAllArchivedNotes(userId);
                if (notes != null)
                {
                    var serializedNote = JsonConvert.SerializeObject(notes);
                    await _cache.SetStringAsync(key, serializedNote, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                    });

                    var response = new ResponseDataModel<IEnumerable<NoteResponse>>
                    {
                        Success = true,
                        Message = "Archived Notes Fetched Successfully from DB",
                        Data = notes
                    };
                    return Ok(response);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpGet("get-notes {noteId}")]
        public async Task<IActionResult> GetNotesByNoteId(int noteId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);
                var key = $"Notes_{userId}";
                await _cache.RemoveAsync(key);
                var cachedNote = await _cache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(cachedNote))
                {
                    var notesList = JsonConvert.DeserializeObject<List<NoteResponse>>(cachedNote);
                    var response = new ResponseDataModel<IEnumerable<NoteResponse>>
                    {
                        Success = true,
                        Message = "Note Fetched Successfully from cache",
                        Data = notesList
                    };
                    return Ok(response);
                }

                var notes = await _notes.GetAllNotebyuserId(noteId, userId);
                if (notes != null)
                {
                    var serializedNote = JsonConvert.SerializeObject(notes);
                    await _cache.SetStringAsync(key, serializedNote, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                    });

                    var response = new ResponseDataModel<NoteResponse>
                    {
                        Success = true,
                        Message = "Note Fetched Successfully from DB",
                        Data = notes
                    };
                    return Ok(response);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
