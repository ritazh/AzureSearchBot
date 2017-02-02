using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcmaBotDTMF.Models;

namespace UcmaBotDTMF.Repositories
{
    public class NewActiveCallRepository
    {
        private static List<ActiveCallModel> activeCallls;

        static NewActiveCallRepository()
        {
            activeCallls  = new List<ActiveCallModel>();
        }
        public static void Add(ActiveCallModel activeCall)
        {
            activeCallls.Add(activeCall);
        }
        public static List<ActiveCallModel> GetActiveCalls()
        {
            return activeCallls.OrderBy(active => active.CallTime).ToList();
        }
        public static ActiveCallModel GetActiveCall(string uri)
        {
            return activeCallls.FirstOrDefault(active => active.SipUri.ToLower() == uri.ToLower());
        }

        public static void DeleteCall(string displayName)
        {
            var call = activeCallls.FirstOrDefault(act => act.DisplayName == displayName);

            if (call == null)
                return;

            activeCallls.Remove(call);
        }

    }
}
