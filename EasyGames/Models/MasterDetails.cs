using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyGames.Models
{
    public class MasterDetails
    {
        [Key]
        public int MasterID { get; set; }
        public List<Client> ClientList { get; set; }
        public List<Transaction> TransactionList { get; set; }
        public List<TransactionType> TransactionTypeList { get; set; }
    }
}