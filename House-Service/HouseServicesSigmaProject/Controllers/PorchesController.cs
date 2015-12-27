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
    public class PorchesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Porches
        [Authorize(Roles = "Admin, Worker")]  
        public ActionResult Index()
        {
            var porches = db.Porches.Include(o => o.house).Include(o => o.house.street).Include(o => o.house.street.subRegion);
            return View(porches.ToList());
        }

        // GET: Porches/Details/5
        [Authorize(Roles = "Admin")]  
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var porches = db.Porches.Include(o => o.house).Include(o => o.house.street).Include(o => o.house.street.subRegion);
            Porch porch = null;
            foreach (Porch p in porches)
            {
                if (p.PorchId == id)
                {
                    porch = p;
                }
            }

          
            List<Apartment> list = db.Apartments.Where(e => e.PorchId == id).ToList();
            ViewBag.count = list.Count;
            if (porch == null)
            {
                return HttpNotFound();
            }
            return View(porch);
        }

        // GET: Porches/Create
        [Authorize(Roles = "Admin")]  
        public ActionResult Create()
        {
            ViewBag.SubRegionId = new SelectList(db.SubRegions, "SubRegionId", "Name");
            ViewBag.StreetId = new SelectList(db.Streets, "StreetId", "Name");
            ViewBag.HouseId = new SelectList(db.Houses, "HouseId", "HouseNumber");
            return View();
        }

        // POST: Porches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  
        public ActionResult Create([Bind(Include = "PorchId,PorchNumber,HouseId")] Porch porch)
        {
            if (ModelState.IsValid)
            {
                db.Porches.Add(porch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SubRegionId = new SelectList(db.SubRegions, "SubRegionId", "Name");
            ViewBag.StreetId = new SelectList(db.Streets, "StreetId", "Name");
            ViewBag.HouseId = new SelectList(db.Houses, "HouseId", "HouseNumber");

            return View(porch);
        }

        // GET: Porches/Edit/5
        [Authorize(Roles = "Admin")]  
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Porch porch = db.Porches.Find(id);
            if (porch == null)
            {
                return HttpNotFound();
            }
            return View(porch);
        }

        // POST: Porches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  
        public ActionResult Edit([Bind(Include = "PorchId,PorchNumber,HouseId")] Porch porch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(porch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(porch);
        }

        // GET: Porches/Delete/5
        [Authorize(Roles = "Admin")]  
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Porch porch = db.Porches.Find(id);
            if (porch == null)
            {
                return HttpNotFound();
            }
            return View(porch);
        }

        // POST: Porches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  
        public ActionResult DeleteConfirmed(int id)
        {
            Porch porch = db.Porches.Find(id);
            db.Porches.Remove(porch);
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

        [Authorize(Roles = "Admin")] 
        public ActionResult GetHousesToLoad(int StreetId)
        {
            IEnumerable<House> houses = db.Houses.Where(a => a.StreetId == StreetId).ToList();
            ViewBag.houses = houses;

            return PartialView();
        }

        [Authorize(Roles = "Admin")] 
        public ActionResult GetStreetsToLoad(int SubRegionId)
        {
            IEnumerable<Street> streets = db.Streets.Where(a => a.SubRegionId == SubRegionId).ToList();
            ViewBag.streets = streets;

            return PartialView();
        }

        [Authorize(Roles = "Admin, Worker")]
        public ActionResult Send(int? id)
        {
            ViewBag.PorchId = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

             
            Porch porch = db.Porches.Find(id); // getting porch by id
            if (porch == null)
            {
                return HttpNotFound();
            }

            return View(porch);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Worker")]
        public ActionResult Send(string textArea, int PorchId)
        {
            int id = PorchId;
            List<Apartment> ApartmentList = db.Apartments.Where(e => e.PorchId == id).ToList(); // all apartments are there
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

            var porches = db.Porches.Include(o => o.house).Include(o => o.house.street).Include(o => o.house.street.subRegion);
            return View(porches.ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Worker")]
        public ActionResult SendSeveral(string textArea, List<int> PorchIdies)
        {
            if(PorchIdies.Count > 0)
            {

                List<string> Emails = new List<string>();
                List<Apartment> Apartments = new List<Apartment>();
                foreach (int id in PorchIdies)
                {
                    List<Apartment> temp = new List<Apartment>();
                    temp = db.Apartments.Where(e => e.PorchId == id).ToList();
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
