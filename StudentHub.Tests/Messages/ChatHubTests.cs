using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using StudentHub.API.Models.Dtos;
using StudentHub.Tests.Infrastructure;

namespace StudentHub.Tests.Messages;

[Collection("Integration")]
public class ChatHubTests
{
    private readonly StudentHubApplicationFixture _fixture;

    public ChatHubTests(StudentHubApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Connect_WithoutToken_FailsToStart()
    {
        await using var connection = new HubConnectionBuilder()
            .WithUrl(
                new Uri(_fixture.Factory.Server.BaseAddress!, "chat"),
                options => options.HttpMessageHandlerFactory = _ =>
                    _fixture.Factory.Server.CreateHandler())
            .Build();

        await Assert.ThrowsAnyAsync<Exception>(() => connection.StartAsync());
    }

    [Fact]
    public async Task JoinGroup_AsMember_Succeeds()
    {
        await using var connection = await TestSignalRConnectionFactory.ConnectAsync(
            _fixture.Factory,
            _fixture.Seed.Member);

        var exception = await Record.ExceptionAsync(() =>
            connection.InvokeAsync("JoinGroup", _fixture.Seed.Group.Id));

        Assert.Null(exception);
    }

    [Fact]
    public async Task JoinGroup_AsNonMember_ThrowsHubException()
    {
        await using var connection = await TestSignalRConnectionFactory.ConnectAsync(
            _fixture.Factory,
            _fixture.Seed.Outsider);

        var exception = await Assert.ThrowsAsync<HubException>(() =>
            connection.InvokeAsync("JoinGroup", _fixture.Seed.Group.Id));

        Assert.Contains("access", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SendMessage_AsMember_BroadcastsMessageDtoToGroup()
    {
        const string content = "Broadcast test message";

        await using var sender = await TestSignalRConnectionFactory.ConnectAsync(
            _fixture.Factory,
            _fixture.Seed.Member);

        await using var receiver = await TestSignalRConnectionFactory.ConnectAsync(
            _fixture.Factory,
            _fixture.Seed.Member);

        var received = new TaskCompletionSource<MessageDto>(TaskCreationOptions.RunContinuationsAsynchronously);

        receiver.On<MessageDto>("ReceiveMessage", message => received.TrySetResult(message));

        await sender.InvokeAsync("JoinGroup", _fixture.Seed.Group.Id);
        await receiver.InvokeAsync("JoinGroup", _fixture.Seed.Group.Id);
        await sender.InvokeAsync("SendMessage", _fixture.Seed.Group.Id, content);

        var message = await received.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(content, message.Content);
        Assert.Equal(_fixture.Seed.Member.Username, message.UserName);
        Assert.False(string.IsNullOrWhiteSpace(message.Id));
    }

    [Fact]
    public async Task SendMessage_AsNonMember_ThrowsHubException()
    {
        await using var connection = await TestSignalRConnectionFactory.ConnectAsync(
            _fixture.Factory,
            _fixture.Seed.Outsider);

        var exception = await Assert.ThrowsAsync<HubException>(() =>
            connection.InvokeAsync("SendMessage", _fixture.Seed.Group.Id, "Should fail"));

        Assert.Contains("not a member", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LeaveGroup_RemovesConnectionFromGroupBroadcast()
    {
        const string content = "Message after leave";

        await using var sender = await TestSignalRConnectionFactory.ConnectAsync(
            _fixture.Factory,
            _fixture.Seed.Member);

        await using var leaver = await TestSignalRConnectionFactory.ConnectAsync(
            _fixture.Factory,
            _fixture.Seed.Member);

        var receivedAfterLeave = false;
        leaver.On<MessageDto>("ReceiveMessage", _ => receivedAfterLeave = true);

        await sender.InvokeAsync("JoinGroup", _fixture.Seed.Group.Id);
        await leaver.InvokeAsync("JoinGroup", _fixture.Seed.Group.Id);
        await leaver.InvokeAsync("LeaveGroup", _fixture.Seed.Group.Id);

        await sender.InvokeAsync("SendMessage", _fixture.Seed.Group.Id, content);

        await Task.Delay(300);

        Assert.False(receivedAfterLeave);
    }
}
