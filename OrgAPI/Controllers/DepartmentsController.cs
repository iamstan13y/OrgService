using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrgDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : Controller
    {
        OrganizationDbContext dbContext;
        UserManager<IdentityUser> userManager; 
        public DepartmentsController(OrganizationDbContext dbContext, UserManager<IdentityUser> _userManager)
        {
            this.dbContext = dbContext;
            userManager = _userManager;
        }

        //#1 Using Lambda Expressions.
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var Depts = await dbContext.Departments
        //        .Select(x => new Department
        //        {
        //            Did = x.Did,
        //            DName = x.DName,
        //            Description = x.Description,
        //            Employees = x.Employees.Select(y => new Employee
        //            {
        //                Eid = y.Eid,
        //                Name = y.Name,
        //                Gender = y.Gender
        //            })
        //        })
        //        .ToListAsync();

        //    if (Depts.Count != 0)
        //        return Ok(Depts);
        //    else
        //        return NotFound();
        //}

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var Depts = await dbContext.Departments.ToListAsync();

            if (Depts.Count != 0)
                return Ok(Depts);
            else
                return NotFound();
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var Depts = await dbContext.Departments.Where(x => x.Did == id).FirstOrDefaultAsync();

                if (Depts != null)
                {
                    var jsonResult = JsonConvert.SerializeObject(
                        Depts,
                        Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                    return Ok(jsonResult);
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong, our clueless engineers are working on it!");
            }
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
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                D.Id = user.Id;
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
