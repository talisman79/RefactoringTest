using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LegacyApp.Services;

namespace LegacyApp.CreditProviders
{
    public class CreditLimitProviderFactory
    {
        private readonly IReadOnlyDictionary<string, ICreditLimitProvider> _creditLimitProviders;

        public CreditLimitProviderFactory(IUserCreditService userCreditService)
        {
            var creditLimitProviderType = typeof(ICreditLimitProvider);
            _creditLimitProviders = creditLimitProviderType.Assembly.ExportedTypes
                .Where(x => creditLimitProviderType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x =>
                {
                    var parameterlessCtor = x.GetConstructors().SingleOrDefault(c => c.GetParameters().Length == 0);
                    return parameterlessCtor is not null
                        ? Activator.CreateInstance(x)
                        : Activator.CreateInstance(x, userCreditService);
                })
                .Cast<ICreditLimitProvider>()
                .ToImmutableDictionary(x => x.NameRequirement, x => x);
        }

        public ICreditLimitProvider GetProviderByClientName(string clientName)
        {
            var provider = _creditLimitProviders.GetValueOrDefault(clientName);
            return provider ?? DefaultCreditLimitProvider();
        }

        private ICreditLimitProvider DefaultCreditLimitProvider()
        {
            return _creditLimitProviders[string.Empty]; 
        }
    }
}