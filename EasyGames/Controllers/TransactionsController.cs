using EasyGames.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EasyGames.Controllers
{
    public class TransactionsController : Controller
    {
        private EasyGamesDBEntities db = new EasyGamesDBEntities();

        // GET: Transactions
        public ActionResult Index(int? id = 1)
        {
            ViewBag.id = id;

            var transactions = db.Transactions.Include(t => t.Client).Include(t => t.TransactionType);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Name");
            ViewBag.TransactionTypeID = new SelectList(db.TransactionTypes, "TransactionTypeID", "TransactionTypeName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,Amount,TransactionTypeID,ClientID,Comment")] Transaction transaction, [Bind(Include = "ClientID,Name,Surname,ClientBalance")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                var tempTCID = transaction.ClientID;
                var check = db.Clients.Where(c => tempTCID.Equals(c.ClientID)).FirstOrDefault();
                var tempCID = check.ClientID;

                //Test values
                //ViewBag.TempCID = "TempCID: " + tempCID;
                //ViewBag.TempTCID = "TempTCID: " + tempTCID.ToString();

                //client.ClientID = client.ClientID;
                //client.Name = check.Name;
                //client.Surname = check.Surname;
                client.ClientBalance = check.ClientBalance + transaction.Amount;

                //Test Values
                //ViewBag.CID = "Client ID: " + client.ClientID;
                //ViewBag.Name = "Client Name: " + client.Name.ToString();
                //ViewBag.Surname = "Client Surname: " + client.Surname.ToString();
                //ViewBag.ClientBalance = "Client Balance: " + client.ClientBalance.ToString();

                db.SaveChanges();
                return RedirectToAction("Index", "Main");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Name", transaction.ClientID);
            ViewBag.TransactionTypeID = new SelectList(db.TransactionTypes, "TransactionTypeID", "TransactionTypeName", transaction.TransactionTypeID);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Name", transaction.ClientID);
            ViewBag.TransactionTypeID = new SelectList(db.TransactionTypes, "TransactionTypeID", "TransactionTypeName", transaction.TransactionTypeID);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransactionID,Amount,TransactionTypeID,ClientID,Comment")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Name", transaction.ClientID);
            ViewBag.TransactionTypeID = new SelectList(db.TransactionTypes, "TransactionTypeID", "TransactionTypeName", transaction.TransactionTypeID);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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