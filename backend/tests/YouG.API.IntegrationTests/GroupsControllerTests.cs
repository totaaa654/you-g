using System.Net;
using YouG.Application.Features.Groups.Dtos;
using YouG.Domain.Enums;

namespace YouG.API.IntegrationTests;

public class GroupsControllerTests(IntegrationTestWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateGroup_Authenticated_Returns201WithCreatorAsSoleAdminMember()
    {
        await RegisterAndAuthenticateAsync();

        var createResponse = await PostJsonAsync("/api/v1/groups", new { Name = "Friday Crew", Description = "Weekly hangout" });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var group = await ReadAsAsync<GroupDto>(createResponse);
        Assert.Equal("Friday Crew", group.Name);
        Assert.Equal(1, group.MemberCount);

        var membersResponse = await Client.GetAsync($"/api/v1/groups/{group.Id}/members");
        Assert.Equal(HttpStatusCode.OK, membersResponse.StatusCode);
        var members = await ReadAsAsync<List<GroupMemberDto>>(membersResponse);
        var member = Assert.Single(members);
        Assert.Equal(GroupRole.Admin, member.Role);
    }

    [Fact]
    public async Task CreateGroup_WithoutAuthentication_Returns401()
    {
        var response = await PostJsonAsync("/api/v1/groups", new { Name = "No Auth Group", Description = (string?)null });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetGroupById_NonMember_Returns404NotForbidden()
    {
        await RegisterAndAuthenticateAsync();
        var createResponse = await PostJsonAsync("/api/v1/groups", new { Name = "Private Group", Description = (string?)null });
        var group = await ReadAsAsync<GroupDto>(createResponse);

        // A second, unrelated user should get 404 (not 403) - matches the "can't distinguish
        // doesn't-exist from can't-see" authorization decision (docs/04-API-DESIGN.md Section 4).
        await RegisterAndAuthenticateAsync();
        var response = await Client.GetAsync($"/api/v1/groups/{group.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMyGroups_ReturnsOnlyGroupsTheCallerBelongsTo()
    {
        await RegisterAndAuthenticateAsync();
        var createResponse = await PostJsonAsync("/api/v1/groups", new { Name = "My Own Group", Description = (string?)null });
        var myGroup = await ReadAsAsync<GroupDto>(createResponse);

        var response = await Client.GetAsync("/api/v1/groups");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var groups = await ReadAsAsync<List<GroupDto>>(response);
        Assert.Contains(groups, g => g.Id == myGroup.Id);
    }

    [Fact]
    public async Task JoinViaAdminCreatedInviteLink_JoinsInstantly()
    {
        await RegisterAndAuthenticateAsync();
        var createResponse = await PostJsonAsync("/api/v1/groups", new { Name = "Admin Invite Group", Description = (string?)null });
        var group = await ReadAsAsync<GroupDto>(createResponse);

        var inviteResponse = await Client.PostAsync($"/api/v1/groups/{group.Id}/invite-links", null);
        Assert.Equal(HttpStatusCode.Created, inviteResponse.StatusCode);
        var invite = await ReadAsAsync<InviteLinkDto>(inviteResponse);

        await RegisterAndAuthenticateAsync();
        var joinResponse = await Client.PostAsync($"/api/v1/groups/join/{invite.Code}", null);

        Assert.Equal(HttpStatusCode.OK, joinResponse.StatusCode);
        var result = await ReadAsAsync<JoinGroupResultDto>(joinResponse);
        Assert.True(result.Joined);
        Assert.Equal(2, result.Group!.MemberCount);
    }

    [Fact]
    public async Task JoinViaMemberCreatedInviteLink_CreatesPendingRequestUntilAdminApproves()
    {
        var adminAuth = await RegisterAndAuthenticateAsync();
        var createResponse = await PostJsonAsync("/api/v1/groups", new { Name = "Member Invite Group", Description = (string?)null });
        var group = await ReadAsAsync<GroupDto>(createResponse);

        // Admin creates an invite link so a second user can join as a regular member.
        var adminInviteResponse = await Client.PostAsync($"/api/v1/groups/{group.Id}/invite-links", null);
        var adminInvite = await ReadAsAsync<InviteLinkDto>(adminInviteResponse);

        await RegisterAndAuthenticateAsync();
        await Client.PostAsync($"/api/v1/groups/join/{adminInvite.Code}", null);

        // That regular member (not the admin) creates their own invite link.
        var memberInviteResponse = await Client.PostAsync($"/api/v1/groups/{group.Id}/invite-links", null);
        Assert.Equal(HttpStatusCode.Created, memberInviteResponse.StatusCode);
        var memberInvite = await ReadAsAsync<InviteLinkDto>(memberInviteResponse);

        await RegisterAndAuthenticateAsync();
        var joinResponse = await Client.PostAsync($"/api/v1/groups/join/{memberInvite.Code}", null);

        Assert.Equal(HttpStatusCode.OK, joinResponse.StatusCode);
        var joinResult = await ReadAsAsync<JoinGroupResultDto>(joinResponse);
        Assert.False(joinResult.Joined);

        // Admin sees the pending request and approves it.
        AuthenticateAs(adminAuth);
        var requestsResponse = await Client.GetAsync($"/api/v1/groups/{group.Id}/join-requests");
        Assert.Equal(HttpStatusCode.OK, requestsResponse.StatusCode);
        var pending = await ReadAsAsync<List<GroupJoinRequestDto>>(requestsResponse);
        var request = Assert.Single(pending);

        var respondResponse = await PutJsonAsync(
            $"/api/v1/groups/{group.Id}/join-requests/{request.Id}", new { Status = "Accepted" });
        Assert.Equal(HttpStatusCode.NoContent, respondResponse.StatusCode);

        var membersResponse = await Client.GetAsync($"/api/v1/groups/{group.Id}/members");
        var members = await ReadAsAsync<List<GroupMemberDto>>(membersResponse);
        Assert.Equal(3, members.Count); // admin + member + newly-approved requester
    }

    [Fact]
    public async Task LeaveGroup_RemovesCallerFromMembers()
    {
        await RegisterAndAuthenticateAsync();
        var createResponse = await PostJsonAsync("/api/v1/groups", new { Name = "Leave Test Group", Description = (string?)null });
        var group = await ReadAsAsync<GroupDto>(createResponse);

        var inviteResponse = await Client.PostAsync($"/api/v1/groups/{group.Id}/invite-links", null);
        var invite = await ReadAsAsync<InviteLinkDto>(inviteResponse);

        await RegisterAndAuthenticateAsync();
        await Client.PostAsync($"/api/v1/groups/join/{invite.Code}", null);

        var leaveResponse = await Client.DeleteAsync($"/api/v1/groups/{group.Id}/members/me");
        Assert.Equal(HttpStatusCode.NoContent, leaveResponse.StatusCode);

        var getAfterLeave = await Client.GetAsync($"/api/v1/groups/{group.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterLeave.StatusCode);
    }
}
