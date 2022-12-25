using Moq;
using TNO.EventSystem.Abstractions;
using TNO.Tests.Moq;

#pragma warning disable IDE1006 // Async Suffix isn't needed for test methods.

namespace TNO.EventSystem.Tests
{
   [TestClass]
   [TestCategory(Category.EventSystem)]
   public class EventSystemFacadeTests
   {
      #region Fields
      private readonly EventSystemFacade _sut;
      #endregion
      public EventSystemFacadeTests()
      {
         _sut = new EventSystemFacade();
      }

      #region Tests
      #region Subscribe
      [TestMethod]
      public void Subscribe_ValidHandler_Successful()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed(handler));

         // Act
         bool result = _sut.Subscribe(handler);

         // Assert
         Assert.IsTrue(result);
         Assert.IsTrue(_sut.IsSubscribed(handler));
         handlerMock.VerifyNoOtherCalls();
      }
      [TestMethod]
      public void Subscribe_HandlerAlreadySubscribed_DoesNothing()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         _sut.Subscribe(handler);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handler));

         // Act
         bool result = _sut.Subscribe(handler);

         // Assert
         Assert.IsFalse(result);
         handlerMock.VerifyNoOtherCalls();
      }

      [TestMethod]
      public void Subscribe_ValidDelegate_Successful()
      {
         // Arrange

         // Act
         bool result = _sut.Subscribe<object>(HandlerAction);

         // Assert
         Assert.IsTrue(result);
         Assert.IsTrue(_sut.IsSubscribed<object>(HandlerAction));
      }
      [TestMethod]
      public void Subscribe_DelegateAlreadySubscribed_DoesNothing()
      {
         // Act
         _sut.Subscribe<object>(HandlerAction);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed<object>(HandlerAction));

         // Act
         bool result = _sut.Subscribe<object>(HandlerAction);

         // Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public void SubscribeAll_MulitpleHandlers_Successful()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = CreateMultiHandlerMock(out IEventHandler<int> intHandler);
         IEventHandler<object> handler = handlerMock.Object;

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribedForAny(handler));

         // Act
         bool result = _sut.SubscribeAll(handler);

         // Assert
         Assert.IsTrue(result);
         Assert.IsTrue(_sut.IsSubscribed(handler));
         Assert.IsTrue(_sut.IsSubscribed(intHandler));
         handlerMock.VerifyNoOtherCalls();
      }
      [TestMethod]
      public void SubscribeAll_SomeHandlersAlreadySubscribed_Successful()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = CreateMultiHandlerMock(out IEventHandler<int> intHandler);
         IEventHandler<object> handler = handlerMock.Object;

         _sut.Subscribe(intHandler);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(intHandler));

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed(handler));

         // Act
         bool result = _sut.SubscribeAll(handler);

         // Assert
         Assert.IsTrue(result);
         Assert.IsTrue(_sut.IsSubscribed(handler));
         Assert.IsTrue(_sut.IsSubscribed(intHandler));
         handlerMock.VerifyNoOtherCalls();
      }
      [TestMethod]
      public void SubscribeAll_AllHandlersAlreadySubscribed_DoesNothing()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = CreateMultiHandlerMock(out IEventHandler<int> intHandler);
         IEventHandler<object> handler = handlerMock.Object;

         _sut.SubscribeAll(handler);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handler));
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(intHandler));

         // Act
         bool result = _sut.SubscribeAll(handler);

         // Assert
         Assert.IsFalse(result);
         Assert.IsTrue(_sut.IsSubscribed(handler));
         Assert.IsTrue(_sut.IsSubscribed(intHandler));
         handlerMock.VerifyNoOtherCalls();
      }
      #endregion

      #region Unsubscribe
      [TestMethod]
      public void Unsubscribe_SubscribedHandler_Successful()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         _sut.Subscribe(handler);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handler));

         // Act
         bool result = _sut.Unsubscribe(handler);

         // Assert
         Assert.IsTrue(result);
         Assert.IsFalse(_sut.IsSubscribed(handler));
         handlerMock.VerifyNoOtherCalls();
      }
      [TestMethod]
      public void Unsubscribe_NotSubscribedHandler_DoesNothing()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed(handler));

         // Act
         bool result = _sut.Unsubscribe(handler);

         // Assert
         Assert.IsFalse(result);
         Assert.IsFalse(_sut.IsSubscribed(handler));
         handlerMock.VerifyNoOtherCalls();
      }

      [TestMethod]
      public void Unsubscribe_SubscribedDelegate_Successful()
      {
         // Arrange
         _sut.Subscribe<object>(HandlerAction);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed<object>(HandlerAction));

         // Act
         bool result = _sut.Unsubscribe<object>(HandlerAction);

         // Assert
         Assert.IsTrue(result);
         Assert.IsFalse(_sut.IsSubscribed<object>(HandlerAction));
      }
      [TestMethod]
      public void Unsubscribe_NotSubscribedDelegate_DoesNothing()
      {
         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed<object>(HandlerAction));

         // Act
         bool result = _sut.Unsubscribe<object>(HandlerAction);

         // Assert
         Assert.IsFalse(result);
         Assert.IsFalse(_sut.IsSubscribed<object>(HandlerAction));
      }

      [TestMethod]
      public void UnsubscribeAll_AllHandlersSubscribed_Successful()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = CreateMultiHandlerMock(out IEventHandler<int> intHandler);
         IEventHandler<object> handler = handlerMock.Object;

         _sut.SubscribeAll(handler);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handler));
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(intHandler));

         // Act
         bool result = _sut.UnsubscribeAll(handler);

         // Assert
         Assert.IsTrue(result);
         Assert.IsFalse(_sut.IsSubscribed(handler));
         Assert.IsFalse(_sut.IsSubscribed(intHandler));
         Assert.IsFalse(_sut.IsSubscribedForAny(handler));
         handlerMock.VerifyNoOtherCalls();
      }
      [TestMethod]
      public void UnsubscribeAll_SomeHandlersSubscribed_Successful()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = CreateMultiHandlerMock(out IEventHandler<int> intHandler);
         IEventHandler<object> handler = handlerMock.Object;

         _sut.Subscribe(handler);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handler));
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed(intHandler));

         // Act
         bool result = _sut.UnsubscribeAll(handler);

         // Assert
         Assert.IsTrue(result);
         Assert.IsFalse(_sut.IsSubscribed(handler));
         Assert.IsFalse(_sut.IsSubscribed(intHandler));
         Assert.IsFalse(_sut.IsSubscribedForAny(handler));
         handlerMock.VerifyNoOtherCalls();
      }
      [TestMethod]
      public void UnsubscribeAll_NoHandlersSubscribed_DoesNothing()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = CreateMultiHandlerMock(out IEventHandler<int> intHandler);
         IEventHandler<object> handler = handlerMock.Object;

         // Arrange Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed(handler));
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed(intHandler));

         // Act
         bool result = _sut.UnsubscribeAll(handler);

         // Assert
         Assert.IsFalse(result);
         Assert.IsFalse(_sut.IsSubscribed(handler));
         Assert.IsFalse(_sut.IsSubscribed(intHandler));
         Assert.IsFalse(_sut.IsSubscribedForAny(handler));
         handlerMock.VerifyNoOtherCalls();
      }
      #endregion

      #region Is Subscribed
      [TestMethod]
      public void IsSubscribed_HandlerSubscribed_ReturnsTrue()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed(handler));

         // Act
         _sut.Subscribe(handler);
         bool result = _sut.IsSubscribed(handler);

         // Assert
         Assert.IsTrue(result);
         handlerMock.VerifyNoOtherCalls();
      }
      [TestMethod]
      public void IsSubscribed_HandlerNotSubscribed_ReturnsFalse()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         // Act
         bool result = _sut.IsSubscribed(handler);

         // Assert
         Assert.IsFalse(result);
         handlerMock.VerifyNoOtherCalls();
      }

      [TestMethod]
      public void IsSubscribed_DelegateSubscribed_ReturnsTrue()
      {
         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed<object>(HandlerAction));

         // Act
         _sut.Subscribe<object>(HandlerAction);
         bool result = _sut.IsSubscribed<object>(HandlerAction);

         // Assert
         Assert.IsTrue(result);
      }
      [TestMethod]
      public void IsSubscribed_DelegateNotSubscribed_ReturnsFalse()
      {
         // Act
         bool result = _sut.IsSubscribed<object>(HandlerAction);

         // Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsSubscribedForAny_HandlerSubscribed_ReturnsTrue()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed(handler));

         // Act
         _sut.Subscribe(handler);
         bool result = _sut.IsSubscribedForAny(handler);

         // Assert
         Assert.IsTrue(result);
         handlerMock.VerifyNoOtherCalls();
      }
      [TestMethod]
      public void IsSubscribedForAny_HandlerNotSubscribed_ReturnsFalse()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         // Act
         bool result = _sut.IsSubscribedForAny(handler);

         // Assert
         Assert.IsFalse(result);
         handlerMock.VerifyNoOtherCalls();
      }

      [TestMethod]
      public void AnySubscribersForEvent_HandlerSubscribed_ReturnsTrue()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         _sut.Subscribe(handler);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handler));

         // Act
         bool result = _sut.AnySubscribersForEvent<object>();

         // Assert
         Assert.IsTrue(result);
      }

      [TestMethod]
      public void AnySubscribersForEvent_HandlerNotSubscribed_ReturnsTrue()
      {
         // Arrange
         Mock<IEventHandler<object>> handlerMock = new Mock<IEventHandler<object>>();
         IEventHandler<object> handler = handlerMock.Object;

         // Arrange Assert
         Assert.That.IsInconclusiveIf(_sut.IsSubscribed(handler));

         // Act
         bool result = _sut.AnySubscribersForEvent<object>();

         // Assert
         Assert.IsFalse(result);
      }
      #endregion

      [TestMethod]
      public async Task PublishAsync_NoHandlers_ReturnsFalse()
      {
         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_sut.AnySubscribersForEvent<object>());

         // Act
         bool result = await _sut.PublishAsync(new object());

         // Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public async Task PublishAsync_WithHandler_ReturnsTrue()
      {
         // Arrange
         Mock<IEventHandler<object>> mockHandler = new Mock<IEventHandler<object>>();
         mockHandler.Setup(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
         IEventHandler<object> handler = mockHandler.Object;

         _sut.Subscribe(handler);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handler));

         // Act
         bool result = await _sut.PublishAsync(new object());

         // Assert
         Assert.IsTrue(result);
         mockHandler.VerifyOnce(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()));
         mockHandler.VerifyNoOtherCalls();
      }

      [TestMethod]
      public async Task PublishAsync_WithHandlerAndCancelledToken_ReturnsFalse()
      {
         // Arrange
         Mock<IEventHandler<object>> mockHandler = new Mock<IEventHandler<object>>();
         mockHandler.Setup(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
         IEventHandler<object> handler = mockHandler.Object;

         CancellationTokenSource tokenSource = new CancellationTokenSource();
         tokenSource.Cancel();

         _sut.Subscribe(handler);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handler));
         Assert.That.IsInconclusiveIfNot(tokenSource.IsCancellationRequested);

         // Act
         bool result = await _sut.PublishAsync(new object(), tokenSource.Token);

         // Assert
         Assert.IsFalse(result);
         mockHandler.VerifyNever(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()));
         mockHandler.VerifyNoOtherCalls();
      }

      [TestMethod]
      public async Task PublichAsync_WithMultipleHandlers_AllHandled()
      {
         // Arrange
         Mock<IEventHandler<object>> mockHandlerA = new Mock<IEventHandler<object>>();
         Mock<IEventHandler<object>> mockHandlerB = new Mock<IEventHandler<object>>();

         mockHandlerA.Setup(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
         mockHandlerB.Setup(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

         IEventHandler<object> handlerA = mockHandlerA.Object;
         IEventHandler<object> handlerB = mockHandlerB.Object;

         _sut.Subscribe(handlerA);
         _sut.Subscribe(handlerB);

         // Arrange Assert
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handlerA));
         Assert.That.IsInconclusiveIfNot(_sut.IsSubscribed(handlerB));

         // Act
         bool result = await _sut.PublishAsync(new object());

         // Assert
         Assert.IsTrue(result);

         mockHandlerA.VerifyOnce(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()));
         mockHandlerB.VerifyOnce(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()));

         Assert.That.NoOtherCalls(mockHandlerA, mockHandlerB);
      }
      #endregion

      #region Methods
      private static Mock<IEventHandler<object>> CreateMultiHandlerMock(out IEventHandler<int> intHandler)
      {
         Mock<IEventHandler<int>> intMock = new Mock<IEventHandler<int>>();
         Mock<IEventHandler<object>> objectMock = intMock.As<IEventHandler<object>>();

         intHandler = (IEventHandler<int>)objectMock.Object; // not sure if this is necessary

         return objectMock;
      }

      private static Task HandlerAction(object _) => Task.CompletedTask;
      #endregion
   }
}

#pragma warning restore IDE1006