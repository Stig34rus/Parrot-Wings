using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PW.Models;

namespace PW.Service
{
    public class WalletService : IDisposable
    {
        public WalletContext _context;
        public WalletService()
        {
            _context = new WalletContext();
        }
        public Wallet GetWalletInfo(string Id)
        {
            var result = _context.Wallets.Where(x => x.Id == Id).FirstOrDefault();
            return result;
        }

        public List<WalletHistory> GetHistory(string Id)
        {
            var result = (from t in _context.Transactions
                          where t.From == Id || t.To == Id
                          join wf in _context.Wallets on t.From equals wf.Id
                          join wt in _context.Wallets on t.To equals wt.Id
                          orderby t.When ascending
                          select new WalletHistory { From = wf.Owner, To = wt.Owner, Value = t.Value, When = t.When}).Distinct().Take(10).ToList();
            return result;
        }

        public bool CreateWallet(Wallet model, ref List<KeyValuePair<string, string>> ErrorList)
        {
            var result = _context.Wallets.Add(model);
            if (_context.SaveChanges() < 0)
            {
                ErrorList.Add(new KeyValuePair<string, string>("DB", "Querry has been crashed"));
                return false;
            }
            return CreateTransaction(new Transaction { From = "ParrotWingsBank", To = result.Id, Value = 500, When = DateTime.Now.ToString() }, ref ErrorList);
        }
        

        public bool CreateTransaction(Transaction model, ref List<KeyValuePair<string, string>> ErrorList)
        {
            Wallet sendersWallet = _context.Wallets.Where(x => x.Id == model.From).First();
            Wallet reciversWallet = _context.Wallets.Where(x => x.Id == model.To).FirstOrDefault();
            if (reciversWallet == null)
            {
                ErrorList.Add(new KeyValuePair<string, string>("To", "This reciever address doesn't exist"));
                return false;
            }
            if (model.From == model.To)
            {
                ErrorList.Add(new KeyValuePair<string, string>("To", "You can't send money yourself"));
                return false;
            }
            if (sendersWallet.Balance < model.Value)
            {
                ErrorList.Add(new KeyValuePair<string, string>("Value", "You don't have enought money"));
                return false;
            }
            _context.Transactions.Add(model);
            if(model.From != "ParrotWingsBank")
                sendersWallet.Balance -= model.Value;
            reciversWallet.Balance += model.Value; 
            if (_context.SaveChanges() > 0)
                return true;
            ErrorList.Add(new KeyValuePair<string, string>("DB", "Querry has been crashed"));
            return false;
        }

        public List<string> GetWalletsList(string val)
        {
            var result = (from t in _context.Wallets orderby t.Owner ascending select t.Owner).Where(x => x.StartsWith(val)).ToList();
            return result;
        }

        public string GetIdByEmail(string email)
        {
            var result = (from t in _context.Wallets where t.Owner == email select t.Id).FirstOrDefault();
            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}