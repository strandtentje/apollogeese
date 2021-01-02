using System;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
    public static class ServiceLookup
    {
        static Map<Service> services = new Map<Service>();

        public static IEnumerable<Service> All => services.Dictionary.Values;

        public static void Register(string v, Service service)
        {
            services[v] = service;
        }

        internal static void Purge(Guid serviceGuid)
        {
            services.Dictionary.Remove(serviceGuid.ToString());
        }

        public static bool TryGet(string serviceID, out Service service)
        {
            if (services.Has(serviceID))
            {
                service = services[serviceID];
                return false;
            }
            else
            {
                service = null;
                return false;
            }
        }
    }
}