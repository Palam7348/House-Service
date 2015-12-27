using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HouseServicesSigmaProject.Models;
using System.IO;

namespace HouseServicesSigmaProject.Controllers
{
    public class SubRegionsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: SubRegions
        [Authorize(Roles = "Admin, Worker")]  
        public ActionResult Index()
        {
            return View(db.SubRegions.ToList());
        }

        // GET: SubRegions/Details/5
        [Authorize(Roles = "Admin")]  
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Apartment> ApartmentList = db.Apartments.Where(e => e.porch.house.street.SubRegionId == id).ToList(); 
            SubRegion subRegion = db.SubRegions.Find(id);
            ViewBag.count = ApartmentList.Count;
            if (subRegion == null)
            {
                return HttpNotFound();
            }
            return View(subRegion);
        }

        // GET: SubRegions/Create
        [Authorize(Roles = "Admin")]  
        public ActionResult Create()
        {
            return View();
        }

        // POST: SubRegions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  
        public ActionResult Create([Bind(Include = "SubRegionId,Name,Password")] SubRegion subRegion)
        {
            if (ModelState.IsValid)
            {
                db.SubRegions.Add(subRegion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(subRegion);
        }

        // GET: SubRegions/Edit/5
        [Authorize(Roles = "Admin")]  
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubRegion subRegion = db.SubRegions.Find(id);
            if (subRegion == null)
            {
                return HttpNotFound();
            }
            return View(subRegion);
        }

        // POST: SubRegions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  
        public ActionResult Edit([Bind(Include = "SubRegionId,Name")] SubRegion subRegion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subRegion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subRegion);
        }

        // GET: SubRegions/Delete/5
        [Authorize(Roles = "Admin")]  
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubRegion subRegion = db.SubRegions.Find(id);
            if (subRegion == null)
            {
                return HttpNotFound();
            }
            return View(subRegion);
        }

        // POST: SubRegions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  
        public ActionResult DeleteConfirmed(int id)
        {
            SubRegion subRegion = db.SubRegions.Find(id);
            db.SubRegions.Remove(subRegion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]  
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "Admin, Worker")]
        public ActionResult Send(int? id)
        {
            ViewBag.SubRegionId = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            
            SubRegion subRegion = db.SubRegions.Find(id); // getting subRegion by id
            if (subRegion == null)
            {
                return HttpNotFound();
            }

            return View(subRegion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Worker")]
        public ActionResult Send(string textArea, int SubRegionId)
        {
            int id = SubRegionId;

            List<Apartment> ApartmentList = db.Apartments.Where(e => e.porch.house.street.SubRegionId == id).ToList(); // all apartments are there
            if (ApartmentList.Count > 0)
            {
                List<string> eMails = new List<string>();
                for (int i = 0; i < ApartmentList.Count; ++i)
                    eMails.Add(ApartmentList[i].OwnerTelephoneNumber);
                if (textArea != null)
                {
                    string str = textArea;

                    SendRequest(textArea, eMails);

                    return RedirectToAction("SendSuccess");
                }
            }
            else
            {
                return RedirectToAction("SendFail");
            }

            return RedirectToAction("Index");
        }

        private void SendRequest(string text, List<string> eMails)
        {
            string reqList = string.Format("http://api.feedgee.com/1.0/listNew?apikey=c18c99b2397b4c599a66973df27b3849&name={0}", "temp");

            WebRequest req = WebRequest.Create(reqList);
            WebResponse resp = req.GetResponse();
            Stream strList = resp.GetResponseStream();
            StreamReader srList = new StreamReader(strList);
            string sList = srList.ReadToEnd();
            srList.Close();
            string sId = sList.Remove(0, sList.IndexOf("<id>") + 4);
            sId = sId.Remove(sId.IndexOf("<"), sId.Length - sId.IndexOf("<"));
            int Id = int.Parse(sId);

            string reqSendCreate = "";
            reqSendCreate += "http://api.feedgee.com/1.0/listSubscribeOptIn?apikey=c18c99b2397b4c599a66973df27b3849&list_id=" + sId + "&emails=";
            for (int i = 0; i < eMails.Count; ++i)
            {
                reqSendCreate += eMails[i];
                if (i != eMails.Count - 1)
                    reqSendCreate += ",";
            }
            reqSendCreate += "&phones=";
            for (int i = 1; i < eMails.Count; ++i)
                reqSendCreate += ",";
            reqSendCreate += "&optin=FALSE&update_existing=TRUE";
            req = WebRequest.Create(reqSendCreate);
            req.GetResponse();

            string reqCampCreate = string.Format("http://api.feedgee.com/1.0/campaignEmailNew?apikey=c18c99b2397b4c599a66973df27b3849&list_ids={0}&subject={1}&html={2}&names={3}&values={4}&isTransactional=FALSE", Id, "hagtyde@gmail.com", text, "", "");
            req = WebRequest.Create(reqCampCreate);
            resp = req.GetResponse();
            strList = resp.GetResponseStream();
            StreamReader srCamp = new StreamReader(strList);
            string Campain = srCamp.ReadToEnd();
            srCamp.Close();

            sId = Campain.Remove(0, Campain.IndexOf("<id>") + 4);
            sId = sId.Remove(sId.IndexOf("<"), sId.Length - sId.IndexOf("<"));
            Id = int.Parse(sId);

            string reqSend = string.Format("http://api.feedgee.com/1.0/campaignEmailSendNow?apikey=c18c99b2397b4c599a66973df27b3849&campaign_Id={0}", Id);
            req = WebRequest.Create(reqSend);
            resp = req.GetResponse();
        }

        [Authorize(Roles = "Admin, Worker")]
        public ActionResult SendSeveral()
        {

            return View(db.SubRegions.ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Worker")]
        public ActionResult SendSeveral(string textArea, List<int> SubRegionIdies)
        {
            if (SubRegionIdies.Count > 0)
            {

                List<string> Emails = new List<string>();
                List<Apartment> Apartments = new List<Apartment>();
                foreach (int id in SubRegionIdies)
                {
                    List<Apartment> temp = new List<Apartment>();
                    temp = db.Apartments.Where(e => e.porch.house.street.SubRegionId == id).ToList();
                    foreach (Apartment apartment in temp)
                    {
                        Emails.Add(apartment.OwnerTelephoneNumber);
                    }
                }


                if (textArea != null)
                {
                    SendRequest(textArea, Emails);

                    return RedirectToAction("SendSuccess");
                }
            }
            else
            {
                return RedirectToAction("SendFail");
            }


            return RedirectToAction("Index");
        }

        public ActionResult SendSuccess()
        {
            return View();
        }

        public ActionResult SendFail()
        {
            return View();
        }
    }
}
