using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HouseServicesSigmaProject.Models;

using System.Net.Http;
using System.IO;

namespace HouseServicesSigmaProject.Controllers
{
    public class ApartmentsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Apartments
        [Authorize(Roles = "Admin, Worker")]  
        public ActionResult Index()
        {
            var apartments = db.Apartments.Include(o => o.porch).Include(o => o.porch.house).Include(o => o.porch.house.street)
                .Include(o => o.porch.house.street.subRegion);
            return View(apartments.ToList());
        }

        // GET: Apartments/Details/5
        [Authorize(Roles = "Admin")]  
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var apartments = db.Apartments.Include(o => o.porch).Include(o => o.porch.house).Include(o => o.porch.house.street)
                .Include(o => o.porch.house.street.subRegion);
            Apartment apartment = null;
            foreach (Apartment a in apartments)
            {
                if (a.ApartmentId == id)
                {
                    apartment = a;
                }
            }
            
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // GET: Apartments/Create
        [Authorize(Roles = "Admin")]  
        public ActionResult Create()
        {
            ViewBag.SubRegionId = new SelectList(db.SubRegions, "SubRegionId", "Name");
            ViewBag.StreetId = new SelectList(db.Streets, "StreetId", "Name");
            ViewBag.HouseId = new SelectList(db.Houses, "HouseId", "HouseNumber");
            ViewBag.PorchId = new SelectList(db.Porches, "PorchId", "PorchNumber");
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  
        public ActionResult Create([Bind(Include = "ApartmentId,ApartmentNumber,OwnerTelephoneNumber,PorchId")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                db.Apartments.Add(apartment);
                db.SaveChanges();
                /*string request = string.Format("http://api.feedgee.com/1.0/listSubscribeOptInNow?apikey=c18c99b2397b4c599a66973df27b3849&list_id={0}&email={1}&phone={2}&mobilecountry=Ukraine&fname=&lname=&names= &values= &optin=FALSE&update_existing=TRUE", "94626", apartment.OwnerTelephoneNumber,"");

                WebRequest req = WebRequest.Create(request);
                WebResponse resp = req.GetResponse();*/
                AddFollover(apartment.OwnerTelephoneNumber);
                return RedirectToAction("Index");
            }


            ViewBag.SubRegionId = new SelectList(db.SubRegions, "SubRegionId", "Name");
            ViewBag.StreetId = new SelectList(db.Streets, "StreetId", "Name");
            ViewBag.HouseId = new SelectList(db.Houses, "HouseId", "HouseNumber");
            ViewBag.PorchId = new SelectList(db.Porches, "PorchId", "PorchNumber");

            return View(apartment);
        }

        private void AddFollover(string eMail)
        {
            string request = string.Format("http://api.feedgee.com/1.0/listSubscribeOptInNow?apikey=c18c99b2397b4c599a66973df27b3849&list_id={0}&email={1}&phone={2}&mobilecountry=Ukraine&fname=&lname=&names= &values= &optin=FALSE&update_existing=TRUE", "94626", eMail, "");

            WebRequest req = WebRequest.Create(request);
            WebResponse resp = req.GetResponse();
        }
      

            [Authorize(Roles = "Admin")]  
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = db.Apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  
        public ActionResult Edit([Bind(Include = "ApartmentId,ApartmentNumber,OwnerTelephoneNumber,PorchId")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(apartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(apartment);
        }

        // GET: Apartments/Delete/5
        [Authorize(Roles = "Admin")]  
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = db.Apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [Authorize(Roles = "Admin")]  
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Apartment apartment = db.Apartments.Find(id);
            db.Apartments.Remove(apartment);
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
        public ActionResult GetPorchesToLoad(int HouseId)
        {
            IEnumerable<Porch> porches = db.Porches.Where(a => a.HouseId == HouseId).ToList();
            ViewBag.porches = porches;

            return PartialView();

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
            ViewBag.ApartmentId = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Apartment apartment = db.Apartments.Find(id);// мы вытащили квартиру по айдишнику  
            if (apartment == null)
            {
                return HttpNotFound();
            }

            

            return View(apartment);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Worker")]
        public ActionResult Send(string textArea, int ApartmentId)
        {
            int id = ApartmentId;
            Apartment apartment = db.Apartments.Find(id);
          
            if (textArea != null)
            {
                // тут нужно все делать 
                // запихивать мыло и создавть рассылку
                string str = textArea; //сюда запишется то значение которое мы написали в большом текстБоксе, я дебажил, все работает  

                SendRequest(textArea, apartment.OwnerTelephoneNumber);

                return RedirectToAction("SendSuccess");
            }
            else
            {
                return RedirectToAction("SendSFail");
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


        private void SendRequest(string text, string eMail)
        {
            string reqList = string.Format("http://api.feedgee.com/1.0/listNew?apikey=c18c99b2397b4c599a66973df27b3849&name={0}", "temp");

            WebRequest req = WebRequest.Create(reqList);
            WebResponse resp = req.GetResponse();
            Stream strList = resp.GetResponseStream();
            StreamReader srList = new StreamReader(strList);
            string sList = srList.ReadToEnd();
            srList.Close();
            string sId = sList.Remove(0, sList.IndexOf("<id>")+4);
            sId = sId.Remove(sId.IndexOf("<"), sId.Length - sId.IndexOf("<"));
            int Id = int.Parse(sId);

            string reqSendCreate = string.Format("http://api.feedgee.com/1.0/listSubscribeOptIn?apikey=c18c99b2397b4c599a66973df27b3849&list_id={0}&emails={1}&phones={2}&optin=FALSE&update_existing=TRUE", sId, eMail, "");
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
            var apartments = db.Apartments.Include(o => o.porch).Include(o => o.porch.house).Include(o => o.porch.house.street)
                .Include(o => o.porch.house.street.subRegion);
            return View(apartments.ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Worker")]
        public ActionResult SendSeveral(string textArea, List<string> Emails)
        {

            
            if (textArea != null)
            {
               SendRequest(textArea, Emails);

                return RedirectToAction("SendSuccess");
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

       
    }
}
