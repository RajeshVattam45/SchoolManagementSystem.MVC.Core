using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Services;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;
using System.Threading.Tasks;

namespace SchoolManagement.WebAPI.Controllers
{
    
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ExamScheduleApiController : ControllerBase
    {
        private readonly IExamScheduleService _service;

        public ExamScheduleApiController ( IExamScheduleService service )
        {
            _service = service;
        }

        // GET: api/ExamSchedulesApi
        [HttpGet]
        public async Task<IActionResult> Get ( )
        {
            var schedules = await _service.GetExamSchedulesAsync ();
            return Ok ( schedules );
        }

        // GET: api/ExamSchedulesApi/5
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> Get ( int id )
        {
            var schedule = await _service.GetExamScheduleByIdAsync ( id );
            if (schedule == null)
                return NotFound ();
            return Ok ( schedule );
        }

        // POST: api/ExamSchedulesApi
        [HttpPost]
        public async Task<IActionResult> Post ( [FromBody] ExamSchedule schedule )
        {
            if (!ModelState.IsValid)
                return BadRequest ( ModelState );

            await _service.CreateExamScheduleAsync ( schedule );
            return CreatedAtAction ( nameof ( Get ), new { id = schedule.ScheduleId }, schedule );
        }

        //[HttpPost]
        //public async Task<IActionResult> Post ( [FromBody] ExamSchedule schedule )
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest ( ModelState );

        //    try
        //    {
        //        await _service.CreateExamScheduleAsync ( schedule );
        //        return CreatedAtAction ( nameof ( Get ), new { id = schedule.ScheduleId }, schedule );
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return Conflict ( new { message = ex.Message } );
        //    }
        //}

        // PUT: api/ExamSchedulesApi/5
        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Put ( int id, [FromBody] ExamSchedule schedule )
        {
            if (id != schedule.ScheduleId)
                return BadRequest ( "Mismatched Schedule Id" );

            await _service.UpdateExamScheduleAsync ( schedule );
            return NoContent ();
        }

        // DELETE: api/ExamSchedulesApi/5
        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            await _service.DeleteExamScheduleAsync ( id );
            return NoContent ();
        }

        [HttpPost ( "Batch" )]
        public async Task<IActionResult> PostBatch ( [FromBody] ExamScheduleBatchRequest request )
        {
            if (!ModelState.IsValid || request.Schedules == null || !request.Schedules.Any ())
                return BadRequest ( "Invalid request." );

            await _service.ScheduleMultipleExamsAsync ( request );
            return Ok ();
        }
    }
}
