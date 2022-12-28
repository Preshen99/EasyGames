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
    public class MainController : Controller
    {
        private EasyGamesDBEntities db = new EasyGamesDBEntities();

        // GET: Main
        public ActionResult Index(string indexing, string searching, Transaction transaction)
        {
            ViewBag.Name = String.IsNullOrEmpty(indexing) ? "name_desc" : "";
            ViewBag.Surname = String.IsNullOrEmpty(indexing) ? "surname_desc" : "";

            var clients = from c in db.Clients select c;

            if (!String.IsNullOrEmpty(searching))
            {
                clients = clients.Where(c => c.Name.Contains(searching) || c.Surname.Contains(searching));
            }

            switch (indexing)
            {
                case "name_desc":
                    clients = clients.OrderByDescending(c => c.Name);
                    break;
                case "surname_desc":
                    clients = clients.OrderByDescending(c => c.Surname);
                    break;
                default:
                    clients = clients.OrderBy(c => c.Name);
                    break;
            }

            return View(clients.ToList());
        }

        //Testing to see if dynamic balance can be implemented...
        //Not updating db...
        public ActionResult Sum([Bind(Include = "TransactionID,Amount,TransactionTypeID,ClientID,Comment")] Transaction transaction, [Bind(Include = "ClientID,Name,Surname,ClientBalance")] Client client, int? id)
        { 
            var tempTCID = id;
            ViewBag.TCID = tempTCID;
            var check = db.Clients.Where(c => tempTCID == c.ClientID).FirstOrDefault();
            var tempCID = check.ClientID;
            var sumT = db.Transactions.Where(t => tempTCID == t.ClientID).Select(t => t.Amount).Sum();

            //Test values
            ViewBag.TempCID = "TempCID: " + tempCID;
            ViewBag.TempTCID = "TempTCID: " + tempTCID.ToString();
            ViewBag.sumT = "TempCID: " + sumT;

            client.ClientBalance = check.ClientBalance + sumT;
            var total = check.ClientBalance + sumT;

            //////Test Values
            ViewBag.CID = "Client ID: " + client.ClientID;
            ViewBag.Name = "Client Name: " + check.Name;
            ViewBag.Surname = "Client Surname: " + check.Surname;
            ViewBag.ClientBalance = "Client Balance: " + total;

            //string sumQuery = "UPDATE Client SET ClientBalance = @client.ClientBalance WHERE ClientID = @tcid";

            db.Entry(client).State = EntityState.Modified;
            db.SaveChanges();
            //return View();
            return RedirectToAction("index");
        }

        public ActionResult ViewTransactions(int? id, string searching)
        {
            var transactions = db.Transactions.Include(t => t.Client).Include(t => t.TransactionType);

            if (!String.IsNullOrEmpty(searching))
            {
                transactions = transactions.Where(c => c.TransactionType.TransactionTypeName.Contains(searching) || c.Comment.Contains(searching));
            }

            @ViewBag.id = id;

            return View(transactions.ToList());
        }

        public ActionResult EditComment(long? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment([Bind(Include = "TransactionID,Amount,TransactionTypeID,ClientID,Comment")] Transaction transaction)
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

        public ActionResult AddTransaction()
        {
            return View();
        }
    }
}