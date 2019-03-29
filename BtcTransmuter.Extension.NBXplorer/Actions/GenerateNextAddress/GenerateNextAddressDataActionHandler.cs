using System.Collections.Generic;
using System.Threading.Tasks;
using BtcTransmuter.Abstractions.Actions;
using BtcTransmuter.Data.Entities;
using BtcTransmuter.Extension.NBXplorer.Services;
using NBitcoin;

namespace BtcTransmuter.Extension.NBXplorer.Actions.GenerateNextAddress
{
    public class GenerateNextAddressDataActionHandler : BaseActionHandler<GenerateNextAddressData, BitcoinAddress>
    {
        private readonly NBXplorerClientProvider _nbXplorerClientProvider;
        private readonly NBXplorerPublicWalletProvider _nbXplorerPublicWalletProvider;
        private readonly DerivationStrategyFactoryProvider _derivationStrategyFactoryProvider;
        private readonly DerivationSchemeParser _derivationSchemeParser;
        public override string ActionId => "GenerateNextAddress";
        public override string Name => "Generate Address From HD Wallet";

        public override string Description =>
            "Generate the next available address from an xpub using NBXplorer";

        public override string ViewPartial => "ViewGenerateNextAddressAction";

        public override string ControllerName => "GenerateNextAddress";

        public GenerateNextAddressDataActionHandler()
        {
        }

        public GenerateNextAddressDataActionHandler(
            NBXplorerClientProvider nbXplorerClientProvider,
            NBXplorerPublicWalletProvider nbXplorerPublicWalletProvider,
            DerivationStrategyFactoryProvider derivationStrategyFactoryProvider,
            DerivationSchemeParser derivationSchemeParser)
        {
            _nbXplorerClientProvider = nbXplorerClientProvider;
            _nbXplorerPublicWalletProvider = nbXplorerPublicWalletProvider;
            _derivationStrategyFactoryProvider = derivationStrategyFactoryProvider;
            _derivationSchemeParser = derivationSchemeParser;
        }

        protected override async Task<TypedActionHandlerResult<BitcoinAddress>> Execute(Dictionary<string, object> data, RecipeAction recipeAction,
            GenerateNextAddressData actionData)
        {
            var factory = _derivationStrategyFactoryProvider.GetDerivationStrategyFactory(actionData.CryptoCode);
            var wallet = await _nbXplorerPublicWalletProvider.Get(actionData.CryptoCode,
                _derivationSchemeParser.Parse(factory,
                    actionData.DerivationStrategy));
            var result = await
                wallet.GetNextAddress();
            return new NBXplorerActionHandlerResult<BitcoinAddress>(_nbXplorerClientProvider.GetClient(actionData.CryptoCode))
            {
                Executed = true,
                TypedData = result,
                Result = $"Next address: {result}"
            };
        }
    }
}