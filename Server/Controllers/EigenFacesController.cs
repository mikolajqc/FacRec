﻿using System;
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
    public class EigenFacesController : ApiController
    {
        private FaceRecognitionDatabaseEntities db = new FaceRecognitionDatabaseEntities();

        // GET: api/EigenFaces
        public IQueryable<EigenFace> GetEigenFaces()
        {
            return db.EigenFaces;
        }

        // GET: api/EigenFaces/5
        [ResponseType(typeof(EigenFace))]
        public async Task<IHttpActionResult> GetEigenFace(int id)
        {
            EigenFace eigenFace = await db.EigenFaces.FindAsync(id);
            if (eigenFace == null)
            {
                return NotFound();
            }

            return Ok(eigenFace);
        }

        // PUT: api/EigenFaces/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEigenFace(int id, EigenFace eigenFace)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != eigenFace.ID)
            {
                return BadRequest();
            }

            db.Entry(eigenFace).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EigenFaceExists(id))
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

        // POST: api/EigenFaces
        [ResponseType(typeof(EigenFace))]
        public async Task<IHttpActionResult> PostEigenFace(EigenFace eigenFace)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EigenFaces.Add(eigenFace);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = eigenFace.ID }, eigenFace);
        }

        // DELETE: api/EigenFaces/5
        [ResponseType(typeof(EigenFace))]
        public async Task<IHttpActionResult> DeleteEigenFace(int id)
        {
            EigenFace eigenFace = await db.EigenFaces.FindAsync(id);
            if (eigenFace == null)
            {
                return NotFound();
            }

            db.EigenFaces.Remove(eigenFace);
            await db.SaveChangesAsync();

            return Ok(eigenFace);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EigenFaceExists(int id)
        {
            return db.EigenFaces.Count(e => e.ID == id) > 0;
        }
    }
}