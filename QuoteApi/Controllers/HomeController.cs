using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using QuoteApi.Models;

namespace QuoteApi.Controllers
{
    public class HomeController : Controller
    {
        private QuoteApiContext db = new QuoteApiContext();

        // GET: Home
        public ActionResult Index()
        {
            return View(db.SingleQuotes.ToList());
        }

        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SingleQuote singleQuote = db.SingleQuotes.Find(id);
            if (singleQuote == null)
            {
                return HttpNotFound();
            }
            return View(singleQuote);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SingleQuoteId,Quote,Self")] SingleQuote singleQuote)
        {
            if (ModelState.IsValid)
            {
                db.SingleQuotes.Add(singleQuote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(singleQuote);
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SingleQuote singleQuote = db.SingleQuotes.Find(id);
            if (singleQuote == null)
            {
                return HttpNotFound();
            }
            return View(singleQuote);
        }

        // POST: Home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SingleQuoteId,Quote,Self")] SingleQuote singleQuote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(singleQuote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(singleQuote);
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SingleQuote singleQuote = db.SingleQuotes.Find(id);
            if (singleQuote == null)
            {
                return HttpNotFound();
            }
            return View(singleQuote);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SingleQuote singleQuote = db.SingleQuotes.Find(id);
            db.SingleQuotes.Remove(singleQuote);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
