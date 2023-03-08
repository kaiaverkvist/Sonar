using System.Security;
using LiteNetLib;
using Sonar.Routing;
using Sonar.Serialization;

namespace Sonar.Tests;

internal class TestNetworkMessage
{
    public string Identifier { get; set; }

    public TestNetworkMessage(string identifier)
    {
        Identifier = identifier;
    }
}


internal class FloatNetworkMessage
{
    public float X { get; set; }

    public FloatNetworkMessage(float x)
    {
        X = x;
    }
}

public class RouterTests
{
    private Router _router;
    private NetPeer _fakeConnection;
    private Client _fakeClient;

    [SetUp]
    public void Setup()
    {
        _router = new Router();
        _fakeClient = new Client(_fakeConnection, new JsonNetworkSerializer());
    }
    
    [Test]
    public void Register_And_Trigger_Test()
    {
        string expectedIdentifier = "123";
        Assert.IsEmpty(_router.GetHandlerDictionary(), "Router handler dictionary must be empty");

        _router.Register<TestNetworkMessage>((sender, message) =>
        {
            TestContext.WriteLine("Triggered test message");
            Assert.That(message.Identifier, Is.EqualTo(expectedIdentifier));
        });
        Assert.IsNotEmpty(_router.GetHandlerDictionary());
        Assert.That(_router.GetHandlerDictionary().Count, Is.EqualTo(1));

        // Create a test network message and try to trigger it.
        TestNetworkMessage testMessage = new TestNetworkMessage(expectedIdentifier);
        int callCount = _router.Trigger(_fakeClient, testMessage);

        // Now check that we triggered an handler.
        Assert.That(callCount, Is.EqualTo(1), "Must have called one (1) handler");
    }

    [Test]
    public void Register_And_Trigger_Test_WithTwo()
    {
        string expectedIdentifier = "123";
        float expectedX = 2.4f;
        Assert.IsEmpty(_router.GetHandlerDictionary(), "Router handler dictionary must be empty");

        _router.Register<TestNetworkMessage>((sender, message) =>
        {
            TestContext.WriteLine("Triggered test message");
            Assert.That(message.Identifier, Is.EqualTo(expectedIdentifier));
        });
        _router.Register<TestNetworkMessage>((sender, message) =>
        {
            TestContext.WriteLine("Triggered test message 2");
            Assert.That(message.Identifier, Is.EqualTo(expectedIdentifier));
        });
        _router.Register<FloatNetworkMessage>((sender, message) =>
        {
            TestContext.WriteLine("Triggered float message");
            Assert.That(message.X, Is.EqualTo(expectedX));
        });
        Assert.IsNotEmpty(_router.GetHandlerDictionary());
        Assert.That(_router.GetHandlerDictionary().Count, Is.EqualTo(2));

        // Create a test network message and try to trigger it.
        TestNetworkMessage testMessage = new TestNetworkMessage(expectedIdentifier);
        int callCount = _router.Trigger(_fakeClient, testMessage);

        // Now check that we triggered an handler.
        Assert.That(callCount, Is.EqualTo(2), "Must have called one (2) handler");

        // Trigger our NO.2 message
        FloatNetworkMessage floatTestMessage = new FloatNetworkMessage(expectedX);
        callCount = _router.Trigger(_fakeClient, floatTestMessage);
        Assert.That(callCount, Is.EqualTo(1), "Must have called one (1) handler");
    }

    [Test]
    public void Trigger_No_Registered()
    {
        Assert.IsEmpty(_router.GetHandlerDictionary(), "Router handler dictionary must be empty");

        TestNetworkMessage testMessage = new TestNetworkMessage("test_xyz");
        int callCount = 0;

        Assert.Throws<SecurityException>(() =>
        {
            callCount = _router.Trigger(_fakeClient, testMessage);
        });
        // Now check that we triggered an handler.
        Assert.That(callCount, Is.EqualTo(0), "Must have called one (0) handler");
    }
    
    [Test]
    public void Test_GetPayloadType()
    {
        _router.Register<TestNetworkMessage>((sender, d) => { });

        TestNetworkMessage testMessage = new TestNetworkMessage("123");
        Type expectedPayloadType = testMessage.GetType();

        string messageJson = "Sonar.Tests.TestNetworkMessage|{\"Identifier\":\"123\"}";
        Type? payloadType = _router.PayloadTypeLookup(messageJson.Split("|")[0]);

        Assert.That(payloadType, Is.EqualTo(expectedPayloadType), "Payload type must be equal to expected type");
    }
}