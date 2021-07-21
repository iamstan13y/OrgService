using Microsoft.AspNetCore.Mvc;
using OrgDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : Controller
    {
        OrganizationDbContext dbContext;
        public DepartmentsController(OrganizationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var Depts = dbContext.Departments.ToList();

            if (Depts.Count != 0)
                return Ok(Depts);
            else
                return NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var Dept = dbContext.Departments.Where(x => x.Did == id).FirstOrDefault();

            if (Dept != null)
                return Ok(Dept);
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            var Dept = dbContext.Departments.Where(x => x.Did == id).FirstOrDefault();
            dbContext.Remove(Dept);
            dbContext.SaveChanges();
            return "Record deleted successfully!";
        }

        [HttpPost]
        public IActionResult Post(Department D)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(D);
                dbContext.SaveChanges();
                return CreatedAtAction("Get", new { id = D.Did}, D);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut]
        public string Put(Department D)
        {
            dbContext.Update(D);
            dbContext.SaveChanges();
            return "Updated record successfully!";
        }
    }
}
