using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyGames.Models
{
    public class EasyGamesLists
    {
        public IEnumerable<MasterDetails> GetMasterDetails()
        {
            EasyGamesDBEntities db = new EasyGamesDBEntities();
            MasterDetails masterDetails = new MasterDetails();

            masterDetails.ClientList = db.Clients.ToList();
            masterDetails.TransactionTypeList = db.TransactionTypes.ToList();
            masterDetails.TransactionList = db.Transactions.ToList();

            List<MasterDetails> mainList = new List<MasterDetails>();
            return mainList;
        }
    }
}