using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using QuoteApi.Filters;
using QuoteApi.Models;

namespace QuoteApi.Controllers
{
    public class SingleQuotesController : ApiController
    {
        private QuoteApiContext db = new QuoteApiContext();

        // GET: api/SingleQuotes
        public IQueryable<SingleQuote> GetSingleQuotes()
        {
            return db.SingleQuotes;
        }

        // GET: api/SingleQuotes/5
        [ResponseType(typeof(SingleQuote))]
        public IHttpActionResult GetSingleQuote(int id)
        {
            SingleQuote singleQuote = db.SingleQuotes.Find(id);
            if (singleQuote == null)
            {
                return NotFound();
            }

            return Ok(singleQuote);
        }

        // PUT: api/SingleQuotes/5
        //[ValidateHttpAntiForgeryToken]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSingleQuote(int id, SingleQuote singleQuote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != singleQuote.SingleQuoteId)
            {
                return BadRequest();
            }

            db.Entry(singleQuote).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SingleQuoteExists(id))
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

        // POST: api/SingleQuotes
        [ResponseType(typeof(SingleQuote))]
        public IHttpActionResult PostSingleQuote(SingleQuote singleQuote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SingleQuotes.Add(singleQuote);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = singleQuote.SingleQuoteId }, singleQuote);
        }

        // DELETE: api/SingleQuotes/5
        [ResponseType(typeof(SingleQuote))]
        public IHttpActionResult DeleteSingleQuote(int id)
        {
            SingleQuote singleQuote = db.SingleQuotes.Find(id);
            if (singleQuote == null)
            {
                return NotFound();
            }

            db.SingleQuotes.Remove(singleQuote);
            db.SaveChanges();

            return Ok(singleQuote);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SingleQuoteExists(int id)
        {
            return db.SingleQuotes.Count(e => e.SingleQuoteId == id) > 0;
        }
    }
}