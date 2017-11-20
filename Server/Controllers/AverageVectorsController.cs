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
    public class AverageVectorsController : ApiController
    {
        private FaceRecognitionDatabaseEntities db = new FaceRecognitionDatabaseEntities();

        // GET: api/AverageVectors
        public IQueryable<AverageVector> GetAverageVectors()
        {
            return db.AverageVectors;
        }

        // GET: api/AverageVectors/5
        [ResponseType(typeof(AverageVector))]
        public async Task<IHttpActionResult> GetAverageVector(int id)
        {
            AverageVector averageVector = await db.AverageVectors.FindAsync(id);
            if (averageVector == null)
            {
                return NotFound();
            }

            return Ok(averageVector);
        }

        // PUT: api/AverageVectors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAverageVector(int id, AverageVector averageVector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != averageVector.ID)
            {
                return BadRequest();
            }

            db.Entry(averageVector).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AverageVectorExists(id))
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

        // POST: api/AverageVectors
        [ResponseType(typeof(AverageVector))]
        public async Task<IHttpActionResult> PostAverageVector(AverageVector averageVector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AverageVectors.Add(averageVector);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = averageVector.ID }, averageVector);
        }

        // DELETE: api/AverageVectors/5
        [ResponseType(typeof(AverageVector))]
        public async Task<IHttpActionResult> DeleteAverageVector(int id)
        {
            AverageVector averageVector = await db.AverageVectors.FindAsync(id);
            if (averageVector == null)
            {
                return NotFound();
            }

            db.AverageVectors.Remove(averageVector);
            await db.SaveChangesAsync();

            return Ok(averageVector);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AverageVectorExists(int id)
        {
            return db.AverageVectors.Count(e => e.ID == id) > 0;
        }
    }
}