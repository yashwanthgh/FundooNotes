using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer.Notes;
using ModelLayer.NotesModel;
using ModelLayer.Response;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace FundooNotes.Controllers
{
    [Route("api/")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INotesBL _notes;
        private readonly IDistributedCache _cache;
        private readonly ILogger<NoteController> _logger;

        public NoteController(INotesBL notes, IDistributedCache cache, ILogger<NoteController> logger)
        {
            _notes = notes;
            _cache = cache;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("createNote")]
        public async Task<IActionResult> CreateNote(CreateNoteModel createNote)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);

                var newNote = await _notes.CreateNote(createNote, userId);

                _logger.LogInformation("Note created!");

                // Invalidate the user's notes cache
                var userNotesCacheKey = $"Notes_{userId}";
                await _cache.RemoveAsync(userNotesCacheKey);

                var response = new ResponseDataModel<NoteResponse>
                {
                    Success = true,
                    Message = "Note Created Successfully",
                    Data = newNote
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
        [HttpPut("updateNote/{noteId}")]
        public async Task<IActionResult> UpdateNote(int noteId, CreateNoteModel update)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);

                // Invalidate the specific note cache
                var specificNoteCacheKey = $"Note_{noteId}";
                await _cache.RemoveAsync(specificNoteCacheKey);

                // Invalidate the user's notes cache
                var userNotesCacheKey = $"Notes_{userId}";
                await _cache.RemoveAsync(userNotesCacheKey);

                var updatedNote = await _notes.UpdateNote(noteId, userId, update);

                _logger.LogInformation("Note updated successfully!");

                var response = new ResponseDataModel<NoteResponse>
                {
                    Success = true,
                    Message = "Note updated successfully",
                    Data = updatedNote
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
        [HttpDelete("deleteNote/{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);

                // Invalidate the specific note cache
                var specificNoteCacheKey = $"Note_{noteId}";
                await _cache.RemoveAsync(specificNoteCacheKey);

                // Invalidate the user's notes cache
                var userNotesCacheKey = $"Notes_{userId}";
                await _cache.RemoveAsync(userNotesCacheKey);

                await _notes.DeleteNote(noteId, userId);

                _logger.LogInformation($"Note with ID: {noteId} deleted successfully!");

                return Ok(new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "Note deleted successfully",
                    Data = null
                });
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
        [HttpGet("showNotes")]
        public async Task<IActionResult> DisplayNote()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);

                var key = $"Notes_{userId}";

                var cachedNote = await _cache.GetAsync(key);

                if (cachedNote != null)
                {
                    var notesList = JsonSerializer.Deserialize<IEnumerable<NoteResponse>>(cachedNote);

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
                    var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var serializedNote = JsonSerializer.Serialize(notes, jsonSerializerOptions);
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
        [HttpGet("showArchived")]
        public async Task<IActionResult> GetArchivedNotes()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);
                var archivedNotesCacheKey = $"ArchivedNotes_{userId}";

                var cachedNote = await _cache.GetStringAsync(archivedNotesCacheKey);

                if (!string.IsNullOrEmpty(cachedNote))
                {
                    var notesList = JsonSerializer.Deserialize<IEnumerable<NoteResponse>>(cachedNote);
                    var response = new ResponseDataModel<IEnumerable<NoteResponse>>
                    {
                        Success = true,
                        Message = "Archived Notes Fetched Successfully from cache",
                        Data = notesList
                    };
                    return Ok(response);
                }

                var notes = await _notes.GetAllArchivedNotes(userId);

                if (notes != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; 
                    var serializedNote = JsonSerializer.Serialize(  
                        notes, jsonSerializerOptions);
                    await _cache.SetStringAsync(archivedNotesCacheKey, serializedNote,
                        new DistributedCacheEntryOptions
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
        [HttpGet("getNote/{noteId}")]
        public async Task<IActionResult> GetNotesByNoteId(int noteId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);

                var specificNoteCacheKey = $"Note_{noteId}";

                var cachedNote = await _cache.GetStringAsync(specificNoteCacheKey);

                if (!string.IsNullOrEmpty(cachedNote))
                {
                    var noteResponse = JsonSerializer.Deserialize<NoteResponse>(cachedNote);
                    var response = new ResponseDataModel<NoteResponse>
                    {
                        Success = true,
                        Message = "Note Fetched Successfully from cache",
                        Data = noteResponse
                    };
                    return Ok(response);
                }

                var note = await _notes.GetAllNotebyuserId(noteId, userId);

                if (note != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; 
                    var serializedNote = JsonSerializer.Serialize(note, jsonSerializerOptions);
                    await _cache.SetStringAsync(specificNoteCacheKey, serializedNote,
                        new DistributedCacheEntryOptions
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(10)
                        });

                    var response = new ResponseDataModel<NoteResponse>
                    {
                        Success = true,
                        Message = "Note Fetched Successfully from DB",
                        Data = note
                    };

                    return Ok(response);
                }

                return NotFound(new ResponseDataModel<string>
                {
                    Success = false,
                    Message = "Note not found",
                    Data = null
                });
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
