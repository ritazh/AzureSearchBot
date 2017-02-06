using System;
using System.Collections.Generic;
using System.Linq;
using UcmaBotDtmf.Models;

namespace UcmaBotDtmf.Repositories
{
    public class ActiveCallRepository
    {
        private List<ActiveCallModel> activeCallls = new List<ActiveCallModel>();
        public void Add(ActiveCallModel activeCall)
        {
            activeCallls.Add(activeCall);
        }
        public List<ActiveCallModel> GetActiveCalls()
        {
            return activeCallls.OrderBy(active => active.CallTime).ToList();
        }

        public void DeleteCall(string displayName)
        {
            var call = activeCallls.FirstOrDefault(act => act.DisplayName == displayName);

            if (call == null)
                return;

            activeCallls.Remove(call);
        }

    }
}
