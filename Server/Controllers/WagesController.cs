using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Server.Models;

namespace Server.Controllers
{
    public class WagesController : ApiController
    {
        private FaceRecognitionDatabaseEntities db = new FaceRecognitionDatabaseEntities();

        // GET: api/Wages
        public IQueryable<Wage> GetWages()
        {
            return db.Wages;
        }

        // GET: api/Wages/5
        [ResponseType(typeof(Wage))]
        public async Task<IHttpActionResult> GetWage(int id)
        {
            Wage wage = await db.Wages.FindAsync(id);
            if (wage == null)
            {
                return NotFound();
            }

            return Ok(wage);
        }

        // PUT: api/Wages/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWage(int id, Wage wage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wage.ID)
            {
                return BadRequest();
            }

            db.Entry(wage).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Wages
        [ResponseType(typeof(Wage))]
        public async Task<IHttpActionResult> PostWage(Wage wage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Wages.Add(wage);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = wage.ID }, wage);
        }

        // DELETE: api/Wages/5
        [ResponseType(typeof(Wage))]
        public async Task<IHttpActionResult> DeleteWage(int id)
        {
            Wage wage = await db.Wages.FindAsync(id);
            if (wage == null)
            {
                return NotFound();
            }

            db.Wages.Remove(wage);
            await db.SaveChangesAsync();

            return Ok(wage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WageExists(int id)
        {
            return db.Wages.Count(e => e.ID == id) > 0;
        }
    }
}