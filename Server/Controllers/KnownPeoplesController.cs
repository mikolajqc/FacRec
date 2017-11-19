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
    public class KnownPeoplesController : ApiController
    {
        private FaceRecognitionDatabaseEntities3 db = new FaceRecognitionDatabaseEntities3();

        // GET: api/KnownPeoples
        public IQueryable<KnownPeople> GetKnownPeoples()
        {
            return db.KnownPeoples;
        }

        // GET: api/KnownPeoples/5
        [ResponseType(typeof(KnownPeople))]
        public async Task<IHttpActionResult> GetKnownPeople(int id)
        {
            KnownPeople knownPeople = await db.KnownPeoples.FindAsync(id);
            if (knownPeople == null)
            {
                return NotFound();
            }

            return Ok(knownPeople);
        }

        // PUT: api/KnownPeoples/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutKnownPeople(int id, KnownPeople knownPeople)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != knownPeople.Id)
            {
                return BadRequest();
            }

            db.Entry(knownPeople).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KnownPeopleExists(id))
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

        // POST: api/KnownPeoples
        [ResponseType(typeof(KnownPeople))]
        public async Task<IHttpActionResult> PostKnownPeople(KnownPeople knownPeople)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.KnownPeoples.Add(knownPeople);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (KnownPeopleExists(knownPeople.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = knownPeople.Id }, knownPeople);
        }

        // DELETE: api/KnownPeoples/5
        [ResponseType(typeof(KnownPeople))]
        public async Task<IHttpActionResult> DeleteKnownPeople(int id)
        {
            KnownPeople knownPeople = await db.KnownPeoples.FindAsync(id);
            if (knownPeople == null)
            {
                return NotFound();
            }

            db.KnownPeoples.Remove(knownPeople);
            await db.SaveChangesAsync();

            return Ok(knownPeople);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool KnownPeopleExists(int id)
        {
            return db.KnownPeoples.Count(e => e.Id == id) > 0;
        }
    }
}