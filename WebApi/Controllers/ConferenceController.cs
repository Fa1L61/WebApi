using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using WebApi.Models;
using WebApi.Context;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private ApplicationContext _dbContext = new ApplicationContext();


        [HttpPost]
        public ActionResult<Conference> CreateConference([FromBody] Conference conference)
        {
            var checker = CheckNullValue(conference);
            var submittedConf = GetCurrentConferenceByUser(conference.AuthorId);
            if (submittedConf.Value == null && conference.AuthorId != default && checker) 
            {
                conference.SubmittedDate = default;
                _dbContext.Conference.Add(conference);
                _dbContext.SaveChanges();
                return conference;
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult<Conference> UpdateConference(Guid id, [FromBody] Conference conference)
        {
            Conference query = _dbContext.Conference.Where(a => a.Id == id && a.IsSubmitted == false ).FirstOrDefault();

            if (query == null) 
                return NotFound();
            else if (query.IsSubmitted == false && conference.IsSubmitted == false) 
            {
                query.Id = id;
                query.Title = conference.Title;
                query.Description = conference.Description;
                query.ActivityType = conference.ActivityType;
                query.Plan = conference.Plan;
                _dbContext.Conference.Update(query);
                _dbContext.SaveChanges();
                return query;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteConference(Guid id)
        {
            var conference = _dbContext.Conference.FirstOrDefault(a => a.Id == id);
            if (conference == null) 
                return NotFound();
            if (conference.IsSubmitted == false)
            {
                _dbContext.Conference.Remove(conference);
                _dbContext.SaveChanges();
                return Ok();
            }
            return NoContent();
        }

        [HttpPost("{id}/submit")]
        public ActionResult SubmitConference(Guid id)
        {
            var conference = _dbContext.Conference.FirstOrDefault(a => a.Id == id);
            if (conference == null) 
                return NotFound();
            if (!conference.IsSubmitted)
            {
                conference.IsSubmitted = true;
                conference.SubmittedDate = DateTime.UtcNow;
                _dbContext.Conference.Update(conference);
                _dbContext.SaveChanges();
                return Ok();
            }
            return NoContent();
        }

        [HttpGet("/applications/submittedDate=")]
        public ActionResult<IEnumerable<Conference>> GetSubmittedConference([FromQuery] DateTime? afterDate, bool isSubmitted)
        {
            var query = _dbContext.Conference.AsEnumerable();

            if (afterDate.HasValue && !isSubmitted)
            {
                query = _dbContext.Conference.Where(a => !a.IsSubmitted && (a.CreatedDate > afterDate));
            }
            else if (afterDate.HasValue && isSubmitted)
            {
                query = _dbContext.Conference.Where(a => a.SubmittedDate > afterDate);
            }
            return query.ToList();
        }
        
        [HttpGet("{authorId}/current")]
        public ActionResult<Conference> GetCurrentConferenceByUser(Guid authorId)
        {
            var conference = _dbContext.Conference.Where(a => a.AuthorId == authorId && !a.IsSubmitted).FirstOrDefault();
            if (conference == null) 
                return NotFound();

            return conference;
        }

        [HttpGet("{id}")]
        public ActionResult<Conference> GetConferenceById(Guid id)
        {
            var conference = _dbContext.Conference.FirstOrDefault(a => a.Id == id);
            if (conference == null) 
                return NotFound();

            return conference;
        }

        [HttpGet("types")]
        public ActionResult<IEnumerable<Activity>> GetActivityTypes()
        {            
            var query = _dbContext.Activity.ToList();

            return query;
        }

        private bool CheckNullValue(Conference conference)
        {
            int checker = 0;
            PropertyInfo[] properties = typeof(Conference).GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(conference);
                var defaultValue = prop.PropertyType.IsValueType ? Activator.CreateInstance(prop.PropertyType) : null;
                if (value == defaultValue || value == "")
                {
                    checker++;
                }
            }
            if (!Enum.IsDefined(typeof(ActivityType), conference.ActivityType))
            {
                checker++;
            }
            if (checker < 4)
            {
                return true;
            }
            return false;
        }
    }
}  