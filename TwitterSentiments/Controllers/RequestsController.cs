using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TwitterSentiments.Models;
using TwitterSentiments.App_Start;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace TwitterSentiments.Controllers
{
    public class RequestsController : Controller
    {
        private RequestDbContext db = new RequestDbContext();

        // GET: Requests
        public ActionResult Index()
        {
            return View(db.Requests.ToList());
        }

        // GET: Requests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: Requests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TwitterHandle,Count,Result")] Request request)
        {
            // 1e3e99ee674b4a0c930aaca327eea231
            // sub key for text-analytics
            if (ModelState.IsValid)
            {
                CoreTweetWrapper wrapper = new CoreTweetWrapper();
                RequestManager manager = new RequestManager();

                var score = 0.0;

                // Retrieve the given user's most recent tweets, specified by count.
                var tweetList = wrapper.GetUserTimeline(request.TwitterHandle, request.Count);
                
                foreach(var tweet in tweetList)
                { 
                    // Perform the request and save the raw JSON response
                    var response = manager.MakeRequest(tweet.Text);

                    // Some special characters cause the request to return 400
                    if(response != null)
                    {
                        // Deserialize the JSON to be managed by our JObject
                        var json = (JObject)JsonConvert.DeserializeObject(response);

                        // Grab the root of the JSON document
                        var documents = json.SelectToken("documents");

                        // The "score" token contains the sentiment analysis
                        var value = documents[0].SelectToken("score");

                        // Sum into score to be averaged later.
                        score += Convert.ToDouble(value.ToString());
                    }
                }

                // Save the average score
                request.Result = (score / tweetList.Count);

                // Log results and request into database
                db.Requests.Add(request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(request);
        }

        // GET: Requests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TwitterHandle,Count,Result")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        // GET: Requests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Request request = db.Requests.Find(id);
            db.Requests.Remove(request);
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
