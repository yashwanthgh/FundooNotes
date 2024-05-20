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
using RepositoryLayer.Services;
using System.Security.Claims;
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

                var getNotes = await _notes.CreateNote(createNote, userId);

                // Clear cache for all notes of the user
                var allNotesCacheKey = $"Notes_{userId}";
                await _cache.RemoveAsync(allNotesCacheKey);

                _logger.LogInformation("Note created!");

                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };

                await _cache.SetAsync(allNotesCacheKey,
                       Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(getNotes)),
                       options);

                var response = new ResponseDataModel<IEnumerable<NoteResponse>>
                {
                    Success = true,
                    Message = "Note Created Successfully",
                    Data = getNotes
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
        [HttpGet("showNotes")]
        public async Task<IActionResult> DisplayNote()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("Id");
                int userId = Convert.ToInt32(userIdClaim);

                var key = $"Notes_{userId}";

                // await _cache.RemoveAsync(key);

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
                    await _cache.SetStringAsync(key, serializedNote,
                                                 new DistributedCacheEntryOptions
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

                return Ok(); // Return empty response if no notes found (avoid unnecessary error)
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

                var specificNoteCacheKey = $"Note_{noteId}";
                var cachedNote = await _cache.GetStringAsync(specificNoteCacheKey);

                if (cachedNote != null)
                {
                    var updatedNote = await _notes.UpdateNote(noteId, userId, update);

                    if (updatedNote != null)
                    {
                        var serializedNote = JsonConvert.SerializeObject(updatedNote);

                        await _cache.SetStringAsync(specificNoteCacheKey, serializedNote,
                                                     new DistributedCacheEntryOptions
                                                     {
                                                         SlidingExpiration = TimeSpan.FromMinutes(10)
                                                     });
                    }

                    _logger.LogInformation("Note updated successfully!");

                    var updateResponse = new ResponseDataModel<NoteResponse>
                    {
                        Success = true,
                        Message = "Note updated successfully",
                        Data = updatedNote
                    };

                    return Ok(updateResponse);
                }
                var getNotes = await _notes.UpdateNote(noteId, userId, update);

                _logger.LogInformation("Note updated successfully!");

                var response = new ResponseDataModel<NoteResponse>
                {
                    Success = true,
                    Message = "Note updated successfully",
                    Data = getNotes
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

                var specificNoteCacheKey = $"Note_{noteId}";
                var cachedNote = await _cache.GetStringAsync(specificNoteCacheKey);
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
                    var notesList = JsonConvert.DeserializeObject<IEnumerable<NoteResponse>>(cachedNote);
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
                    var serializedNote = JsonConvert.SerializeObject(notes);
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
                    var noteResponse = JsonConvert.DeserializeObject<NoteResponse>(cachedNote);
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
                    var serializedNote = JsonConvert.SerializeObject(note);
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
