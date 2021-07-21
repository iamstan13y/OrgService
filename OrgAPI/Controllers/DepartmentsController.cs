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
        public async Task<IActionResult> Get()
        {
            var Depts = await dbContext.Departments
                .Select(x => new Department
                {
                    Did = x.Did,
                    DName = x.DName,
                    Description = x.Description,
                    Employees = x.Employees.Select(y => new Employee
                    {
                        Eid = y.Eid,
                        Name = y.Name,
                        Gender = y.Gender
                    })
                })
                .ToListAsync();

            if (Depts.Count != 0)
                return Ok(Depts);
            else
                return NotFound();
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Dept = await dbContext.Departments.Where(x => x.Did == id).FirstOrDefaultAsync();

            if (Dept != null)
                return Ok(Dept);
            else
                return NotFound();
        }

        [HttpGet("getByIdAndName")]
        public async Task<IActionResult> GetByIdAndName(int id, string dName)
        {
            var Dept = await dbContext.Departments.Where(x => x.Did == id && x.DName == dName).FirstOrDefaultAsync();

            if (Dept != null)
                return Ok(Dept);
            else
                return NotFound();
        }

        [HttpGet("getByName/{dName}")]
        public async Task<IActionResult> GetByName(string dName)
        {
            var Dept = await dbContext.Departments.Where(x => x.DName == dName).FirstOrDefaultAsync();

            if (Dept != null)
                return Ok(Dept);
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Dept = await dbContext.Departments.Where(x => x.Did == id).FirstOrDefaultAsync();

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
        public async Task<IActionResult> Post(Department D)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(D);
                await dbContext.SaveChangesAsync();
                return CreatedAtAction("Get", new { id = D.Did}, D);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(Department D)
        {
            var Dept = await dbContext.Departments
                                .Where(x => x.Did == D.Did)
                                .AsNoTracking().FirstOrDefaultAsync();
            
            if (Dept != null)
            {
                if (ModelState.IsValid)
                {
                    dbContext.Update(D);
                    await dbContext
                        .SaveChangesAsync();
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
