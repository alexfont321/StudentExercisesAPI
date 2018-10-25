using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using StudentExercisesAPI.Data;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class InstructorController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InstructorController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = @"
             select i.Id,
                    i.FirstName,
                    i.LastName,
                    i.SlackHandle,
                    i.Specialty,
                    i.CohortId
               from Instructor i";

            using (IDbConnection conn = Connection)
            {

                IEnumerable<Instructor> instructors = await conn.QueryAsync<Instructor>(sql);
                return Ok(instructors);
            }

        }


        [HttpGet("{id}", Name = "GetInstructor")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
             select i.Id,
                    i.FirstName,
                    i.LastName,
                    i.SlackHandle,
                    i.Specialty,
                    i.CohortId
               from Instructor i
               where i.id = {id}
               ";

            using (IDbConnection conn = Connection)
            {

                IEnumerable<Instructor> instructors = await conn.QueryAsync<Instructor>(sql);
                return Ok(instructors);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Instructor i)
        {
            string sql = $@"INSERT INTO Instructor 
            (FirstName, LastName, SlackHandle, Specialty, CohortId)
            VALUES
            (
                '{i.FirstName}'
                ,'{i.LastName}'
                ,'{i.SlackHandle}'
                ,'{i.Specialty}',
                '{i.CohortId}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                i.Id = newId;
                return CreatedAtRoute("GetInstructor", new { id = newId }, i);
            }
        }

    }
}
