using System.Linq;
using Xunit;

// TODO: Make the existing tests pass
// TODO: Add missing matching tests
// TODO: Add subscription removal tests

namespace AbcArbitrage.Homework.Routing
{
    public class MessageRouterTests
    {
        private readonly SubscriptionIndex _subscriptionIndex;
        private readonly MessageRouter _router;

        public MessageRouterTests()
        {
            _subscriptionIndex = new SubscriptionIndex();
            _router = new MessageRouter(_subscriptionIndex);
        }

        [Fact]
        public void ShouldIncludeSingleMatchingSubscription()
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<SimpleMessages.ExchangeAdded>(clientId),
            });

            // Act
            var clientIds = _router.GetConsumers(new SimpleMessages.ExchangeAdded()).ToList();

            // Assert
            Assert.Equal(new[] { clientId }, clientIds);
        }

        [Fact]
        public void ShouldIncludeMatchingClientForTwoMessages()
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<SimpleMessages.ExchangeAdded>(clientId),
                Subscription.Of<SimpleMessages.ExchangeTradingPhaseChanged>(clientId),
            });

            // Act
            var clientIdsForMessage1 = _router.GetConsumers(new SimpleMessages.ExchangeAdded()).ToList();
            var clientIdsForMessage2 = _router.GetConsumers(new SimpleMessages.ExchangeTradingPhaseChanged()).ToList();

            // Assert
            Assert.Equal(new[] { clientId }, clientIdsForMessage1);
            Assert.Equal(new[] { clientId }, clientIdsForMessage2);
        }

        [Fact]
        public void ShouldExcludeSubscriptionWithOtherMessageType()
        {
            // Arrange
            var clientId1 = new ClientId("Client.1");
            var clientId2 = new ClientId("Client.2");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<SimpleMessages.ExchangeAdded>(clientId1),
                Subscription.Of<SimpleMessages.ExchangeTradingPhaseChanged>(clientId2),
            });

            // Act
            var clientIds = _router.GetConsumers(new SimpleMessages.ExchangeAdded()).ToList();

            // Assert
            Assert.Equal(new[] { clientId1 }, clientIds);
        }

        [Fact]
        public void ShouldExcludeSingleSubscriptionWithOtherMessageType()
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<SimpleMessages.ExchangeAdded>(clientId),
            });

            // Act
            var clientIds = _router.GetConsumers(new SimpleMessages.ExchangeTradingPhaseChanged()).ToList();

            // Assert
            Assert.Empty(clientIds);
        }

        [Fact]
        public void ShouldIncludeSingleMatchingRoutableSubscription()
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<RoutableMessages.PriceUpdated>(clientId, new ContentPattern("NASDAQ")),
            });

            var routableMessage = new RoutableMessages.PriceUpdated { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            // Act
            var clientIds = _router.GetConsumers(routableMessage).ToList();

            // Assert
            Assert.Equal(new[] { clientId }, clientIds);
        }

        [Fact]
        public void ShouldIncludeRoutableSubscriptionsForTwoClients()
        {
            // Arrange
            var clientId1 = new ClientId("Client.1");
            var clientId2 = new ClientId("Client.2");
            var clientId3 = new ClientId("Client.3");

            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<RoutableMessages.PriceUpdated>(clientId1, new ContentPattern("NASDAQ", "*")),
                Subscription.Of<RoutableMessages.PriceUpdated>(clientId2, new ContentPattern("NYSE", "*")),
                Subscription.Of<RoutableMessages.PriceUpdated>(clientId3, new ContentPattern("NASDAQ", "*")),
            });

            var routableMessage = new RoutableMessages.PriceUpdated { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            // Act
            var clientIds = _router.GetConsumers(routableMessage).ToList();

            // Assert
            Assert.Equal(new[] { clientId1, clientId3 }, clientIds);
        }

        [Fact]
        public void ShouldIncludeMatchingRoutableSubscription()
        {
            // Arrange
            var clientId1 = new ClientId("Client.1");
            var clientId2 = new ClientId("Client.2");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<RoutableMessages.PriceUpdated>(clientId1, new ContentPattern("NASDAQ")),
                Subscription.Of<RoutableMessages.PriceUpdated>(clientId2, new ContentPattern("NYSE")),
            });

            var routableMessage = new RoutableMessages.PriceUpdated { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            // Act
            var clientIds = _router.GetConsumers(routableMessage).ToList();

            // Assert
            Assert.Equal(new[] { clientId1 }, clientIds);
        }

        [Fact]
        public void ShouldExcludeRoutableSubscriptionWithOtherMessageType()
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<RoutableMessages.InstrumentAdded>(clientId, new ContentPattern("9")),
            });

            var routableMessage = new RoutableMessages.InstrumentDelisted { ExchangeId = 9 };

            // Act
            var clientIds = _router.GetConsumers(routableMessage).ToList();

            // Assert
            Assert.Empty(clientIds);
        }

        [Fact]
        public void ShouldExcludeRoutableSubscriptionWithOtherContent()
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<RoutableMessages.PriceUpdated>(clientId, new ContentPattern("NASDAQ")),
            });

            var routableMessage = new RoutableMessages.PriceUpdated { ExchangeCode = "NYSE" };

            // Act
            var clientIds = _router.GetConsumers(routableMessage).ToList();

            // Assert
            Assert.Empty(clientIds);
        }

        [Theory]
        [InlineData("NASDAQ")]
        [InlineData("NASDAQ.MSFT")]
        [InlineData("*")]
        [InlineData("*.MSFT")]
        [InlineData("*.*")]
        public void ShouldIncludeMatchingRoutableSubscriptionWithPattern(string contentPattern)
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<RoutableMessages.PriceUpdated>(clientId, ContentPattern.Split(contentPattern)),
            });

            var routableMessage = new RoutableMessages.PriceUpdated { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            // Act
            var clientIds = _router.GetConsumers(routableMessage).ToList();

            // Assert
            Assert.Equal(new[] { clientId }, clientIds);
        }

        [Theory]
        [InlineData("NASDAQ")]
        [InlineData("NASDAQ.42")]
        [InlineData("NASDAQ.*.*.*.MSFT")]
        [InlineData("NASDAQ.42.TECH.L.MSFT")]
        [InlineData("*")]
        [InlineData("*.*.*.*.MSFT")]
        [InlineData("*.*")]
        [InlineData("*.42.*.*.*")]
        [InlineData("*.*.*.*.*")]
        public void ShouldIncludeMatchingRoutableSubscriptionWithLongPattern(string contentPattern)
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            var subscription = Subscription.Of<RoutableMessages.InstrumentConnected>(clientId, ContentPattern.Split(contentPattern));

            var otherClientId = new ClientId("Client.2");
            var otherSubscription = Subscription.Of<RoutableMessages.InstrumentConnected>(otherClientId, ContentPattern.Split("NYSE.*.*.*.*"));

            _subscriptionIndex.AddSubscriptions(new[] { subscription, otherSubscription });

            var routableMessage = new RoutableMessages.InstrumentConnected { ExchangeCode = "NASDAQ", ProviderId = 42, Sector = "TECH", SymbolRangeStart = 'L', Symbol = "MSFT" };

            // Act
            var clientIds = _router.GetConsumers(routableMessage).ToList();

            // Assert
            Assert.Equal(new[] { clientId }, clientIds);
        }

        [Theory]
        [InlineData("NYSE")]
        [InlineData("NASDAQ.AMZN")]
        [InlineData("NYSE.MSFT")]
        [InlineData("*.AMZN")]
        [InlineData("NYSE.*")]
        [InlineData("*.NASDAQ")]
        [InlineData("MSFT.NASDAQ")]
        public void ShouldExcludeSingleRoutableSubscriptionWithPattern(string contentPattern)
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<RoutableMessages.PriceUpdated>(clientId, ContentPattern.Split(contentPattern)),
            });

            var routableMessage = new RoutableMessages.PriceUpdated { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            // Act
            var clientIds = _router.GetConsumers(routableMessage).ToList();

            // Assert
            Assert.Empty(clientIds);
        }

        [Fact]
        public void ShouldSupportContentWithEmptyValue()
        {
            // Arrange
            var clientId = new ClientId("Client.1");
            _subscriptionIndex.AddSubscriptions(new[]
            {
                Subscription.Of<RoutableMessages.TradingHalted>(clientId, ContentPattern.Split("NASDAQ.*.*")),
            });

            var routableMessage = new RoutableMessages.TradingHalted { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            // Act
            var clientIds = _router.GetConsumers(routableMessage).ToList();

            // Assert
            Assert.Equal(new[] { clientId }, clientIds);
        }

        [Fact]
        public void Subscriptions_Remove_MessageType_Add_Single_Remove_Single()
        {
            var clientId = new ClientId("Client.1");
            Subscription subscription = Subscription.Of<RoutableMessages.TradingHalted>(clientId);
            _subscriptionIndex.AddSubscriptions(new[]
            {
               subscription
            });

            _subscriptionIndex.RemoveSubscriptions(new[]
            {
               subscription
            });

            var routableMessage = new RoutableMessages.TradingHalted { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            var clientIds = _router.GetConsumers(routableMessage).ToList();

            Assert.Empty(clientIds);
        }

        [Fact]
        public void Subscriptions_Remove_MessageType_Add_Multi_Remove_Single()
        {
            var clientId = new ClientId("Client.1");
            Subscription subscription_PriceUpdated = Subscription.Of<RoutableMessages.PriceUpdated>(clientId);
            Subscription subscription_TradingHalted = Subscription.Of<RoutableMessages.TradingHalted>(clientId);
            _subscriptionIndex.AddSubscriptions(new[]
            {
               subscription_TradingHalted,
               subscription_PriceUpdated
            });

            _subscriptionIndex.RemoveSubscriptions(new[]
            {
               subscription_TradingHalted
            });

            var routableMessage = new RoutableMessages.PriceUpdated { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            var clientIds = _router.GetConsumers(routableMessage).ToList();

            Assert.Equal(new[] { clientId }, clientIds);
        }

        [Fact]
        public void Subscriptions_Remove_MessageType_Add_Multi_Remove_Multi()
        {
            var clientId = new ClientId("Client.1");
            Subscription subscription_PriceUpdated = Subscription.Of<RoutableMessages.PriceUpdated>(clientId);
            Subscription subscription_TradingHalted = Subscription.Of<RoutableMessages.TradingHalted>(clientId);
            _subscriptionIndex.AddSubscriptions(new[]
            {
               subscription_TradingHalted,
               subscription_PriceUpdated
            });

            _subscriptionIndex.RemoveSubscriptions(new[]
            {
               subscription_TradingHalted,
                subscription_PriceUpdated
            });

            var routableMessage = new RoutableMessages.PriceUpdated { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            var clientIds = _router.GetConsumers(routableMessage).ToList();

            Assert.Empty(clientIds);
        }

        [Theory]
        [InlineData("NYSE")]
        [InlineData("NASDAQ.AMZN")]
        [InlineData("NYSE.MSFT")]
        [InlineData("*.AMZN")]
        [InlineData("NYSE.*")]
        [InlineData("*.NASDAQ")]
        [InlineData("MSFT.NASDAQ")]
        public void Subscriptions_Remove_MessageTypeContentPattern_Add_Single_Remove_Single(string contentPattern)
        {
            var clientId = new ClientId("Client.1");
            Subscription subscription = Subscription.Of<RoutableMessages.TradingHalted>(clientId, ContentPattern.Split(contentPattern));
            _subscriptionIndex.AddSubscriptions(new[]
            {
               subscription
            });

            _subscriptionIndex.RemoveSubscriptions(new[]
            {
               subscription
            });

            var routableMessage = new RoutableMessages.TradingHalted { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            var clientIds = _router.GetConsumers(routableMessage).ToList();

            Assert.Empty(clientIds);
        }

        [Theory]
        [InlineData("*","*")]
        [InlineData("NASDAQ.AMZN", "NASDAQ")]
        [InlineData("NASDAQ.AMZN", "NASDAQ.*")]
        [InlineData("NYSE.MSFT","NYSE.MSFT.TECH")]
        [InlineData("*.AMZN","NASDAQ,AMZN")]
        [InlineData("NYSE.*","NYSE.MSFT.TECH")]
        [InlineData("*.NASDAQ", "*")]   
        public void Subscriptions_Remove_MessageTypeContentPattern_Add_Multi_Remove_Single(string contentPattern_add, string contentPattern_add_remove)
        {
            var clientId = new ClientId("Client.1");
            Subscription subscription_add = Subscription.Of<RoutableMessages.TradingHalted>(clientId, ContentPattern.Split(contentPattern_add));

            Subscription subscription_remove = Subscription.Of<RoutableMessages.TradingHalted>(clientId, ContentPattern.Split(contentPattern_add_remove));

            _subscriptionIndex.AddSubscriptions(new[]
            {
               subscription_add
            });

            _subscriptionIndex.RemoveSubscriptions(new[]
            {
               subscription_remove
            });

            var routableMessage = new RoutableMessages.TradingHalted { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            var clientIds = _router.GetConsumers(routableMessage).ToList();

            Assert.Empty(clientIds);
        }

        [Theory]
        [InlineData("*", "NYSE")]
        [InlineData("NASDAQ.MSFT", "NYSE.MSFT")]
        [InlineData("*.MSFT", "NYSE,AMZN")]
        [InlineData("*.*", "NYSE.MSFT.TECH")]
        [InlineData("NASDAQ.*", "NYSE,*")]
        public void Subscriptions_Remove_MessageTypeContentPattern_Add_Multi_Remove_Single_2(string contentPattern_add, string contentPattern_add_remove)
        {
            var clientId = new ClientId("Client.1");
            Subscription subscription_add = Subscription.Of<RoutableMessages.TradingHalted>(clientId, ContentPattern.Split(contentPattern_add));

            Subscription subscription_remove = Subscription.Of<RoutableMessages.TradingHalted>(clientId, ContentPattern.Split(contentPattern_add_remove));

            _subscriptionIndex.AddSubscriptions(new[]
            {
               subscription_add
            });

            _subscriptionIndex.RemoveSubscriptions(new[]
            {
               subscription_remove
            });

            var routableMessage = new RoutableMessages.TradingHalted { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            var clientIds = _router.GetConsumers(routableMessage).ToList();

            Assert.Equal(new[] { clientId }, clientIds);
        }

        

        [Fact]
        public void Client_Remove_Add_Single_Remove_Single()
        {
            var clientId = new ClientId("Client.1");
            Subscription subscription = Subscription.Of<RoutableMessages.TradingHalted>(clientId, ContentPattern.Split("NASDAQ.*.*"));
            Subscription subscription2 = Subscription.Of<RoutableMessages.PriceUpdated>(clientId, ContentPattern.Split("MSFT.*.*"));
            _subscriptionIndex.AddSubscriptions(new[]
            {
               subscription,subscription2
            });

            _subscriptionIndex.RemoveSubscriptionsForConsumer(clientId);

            var routableMessage = new RoutableMessages.TradingHalted { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            var clientIds = _router.GetConsumers(routableMessage).ToList();

            Assert.Empty(clientIds);

        }

        [Fact]
        public void Client_Remove_Add_Multi_Remove_Single()
        {
            var clientId1 = new ClientId("Client.1");
            var clientId2 = new ClientId("Client.2");
            Subscription clientId1_subscription = Subscription.Of<RoutableMessages.TradingHalted>(clientId1, ContentPattern.Split("NASDAQ.*.*"));
            Subscription clientId1_subscription2 = Subscription.Of<RoutableMessages.PriceUpdated>(clientId1, ContentPattern.Split("MSFT.*.*"));

            Subscription clientId2_subscription = Subscription.Of<RoutableMessages.TradingHalted>(clientId2, ContentPattern.Split("NASDAQ.*.*"));
            Subscription clientId2_subscription2 = Subscription.Of<RoutableMessages.InstrumentAdded>(clientId2, ContentPattern.Split("MSFT.*.*"));

            _subscriptionIndex.AddSubscriptions(new[]
            {
               clientId1_subscription,clientId1_subscription2,clientId2_subscription,clientId2_subscription2
            });

            _subscriptionIndex.RemoveSubscriptionsForConsumer(clientId1);
      
            var routableMessage = new RoutableMessages.TradingHalted { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            var clientIds = _router.GetConsumers(routableMessage).ToList();

            Assert.Equal(new[] { clientId2 }, clientIds);

        }

        [Fact]
        public void Client_Remove_Add_Multi_Remove_Multi()
        {
            var clientId1 = new ClientId("Client.1");
            var clientId2 = new ClientId("Client.2");
            Subscription clientId1_subscription = Subscription.Of<RoutableMessages.TradingHalted>(clientId1, ContentPattern.Split("NASDAQ.*.*"));
            Subscription clientId1_subscription2 = Subscription.Of<RoutableMessages.PriceUpdated>(clientId2, ContentPattern.Split("MSFT.*.*"));

            Subscription clientId2_subscription = Subscription.Of<RoutableMessages.InstrumentDelisted>(clientId1, ContentPattern.Split("NASDAQ.*.*"));
            Subscription clientId2_subscription2 = Subscription.Of<RoutableMessages.InstrumentAdded>(clientId2, ContentPattern.Split("MSFT.*.*"));

            _subscriptionIndex.AddSubscriptions(new[]
            {
               clientId1_subscription,clientId1_subscription2,clientId2_subscription,clientId2_subscription2
            });

            _subscriptionIndex.RemoveSubscriptionsForConsumer(clientId1);
            _subscriptionIndex.RemoveSubscriptionsForConsumer(clientId1);
            var routableMessage = new RoutableMessages.TradingHalted { ExchangeCode = "NASDAQ", Symbol = "MSFT" };

            var clientIds = _router.GetConsumers(routableMessage).ToList();

            Assert.Empty(clientIds);

        }
    }
}
