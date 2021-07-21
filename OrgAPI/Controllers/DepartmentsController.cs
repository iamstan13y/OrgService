using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("getById/{id}")]
        public IActionResult GetById(int id)
        {
            var Dept = dbContext.Departments.Where(x => x.Did == id).FirstOrDefault();

            if (Dept != null)
                return Ok(Dept);
            else
                return NotFound();
        }

        [HttpGet("getByName/{dName}")]
        public IActionResult GetByName(string dName)
        {
            var Dept = dbContext.Departments.Where(x => x.DName == dName).FirstOrDefault();

            if (Dept != null)
                return Ok(Dept);
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var Dept = dbContext.Departments.Where(x => x.Did == id).FirstOrDefault();

            if (Dept != null)
            { 
                dbContext.Remove(Dept);
                dbContext.SaveChanges();
                return Ok(Dept);
            }
            else
            {
                return NotFound();
            }
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
        public IActionResult Put(Department D)
        {
            var Dept = dbContext.Departments.Where(x => x.Did == D.Did).AsNoTracking().FirstOrDefault();
            
            if (Dept != null)
            {
                if (ModelState.IsValid)
                {
                    dbContext.Update(D);
                    dbContext.SaveChanges();
                    return Ok(D);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
