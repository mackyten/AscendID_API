using AscendID_API.Application.Commands;
using AscendID_API.Application.DTOs;
using AscendID_API.Application.Handlers;
using AscendID_API.Application.Queries;
using AscendID_API.Domain.Entities;
using AscendID_API.Shared.Validators;

namespace AscendID_API.Tests;

public class TempTestRecordEntityTests
{
    [Fact]
    public void NewRecord_IsActive_WithoutUpdateTimestamp()
    {
        var record = new TempTestRecord();

        Assert.True(record.IsActive);
        Assert.Null(record.UpdatedAt);
    }

    [Fact]
    public void Update_ChangesFields_AndStampsUpdatedAt()
    {
        var record = new TempTestRecord { Name = "old", Description = "old" };

        record.Update("new", "desc");

        Assert.Equal("new", record.Name);
        Assert.Equal("desc", record.Description);
        Assert.NotNull(record.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ClearsIsActive()
    {
        var record = new TempTestRecord();

        record.Deactivate();

        Assert.False(record.IsActive);
        Assert.NotNull(record.UpdatedAt);
    }
}

public class TempTestRecordValidatorTests
{
    private readonly CreateTempTestRecordDtoValidator validator = new();

    [Theory]
    [InlineData("Valid name_1-2")]
    [InlineData("a")]
    public void ValidNames_Pass(string name) =>
        Assert.True(validator.Validate(new CreateTempTestRecordDto(name, "")).IsValid);

    [Theory]
    [InlineData("")]
    [InlineData("bad<script>")]
    [InlineData("emoji 😀")]
    public void InvalidNames_Fail(string name) =>
        Assert.False(validator.Validate(new CreateTempTestRecordDto(name, "")).IsValid);

    [Fact]
    public void NameOver100Chars_Fails() =>
        Assert.False(validator.Validate(new CreateTempTestRecordDto(new string('a', 101), "")).IsValid);

    [Fact]
    public void DescriptionOver500Chars_Fails() =>
        Assert.False(validator.Validate(new CreateTempTestRecordDto("ok", new string('x', 501))).IsValid);

    [Fact]
    public void UpdateValidator_AppliesSameNameRule() =>
        Assert.False(new UpdateTempTestRecordDtoValidator()
            .Validate(new UpdateTempTestRecordDto("no/slashes", "")).IsValid);
}

public class TempTestRecordHandlerTests
{
    private readonly FakeTempTestRecordRepository repo = new();

    private async Task<TempTestRecordDto> SeedAsync(string name = "seed") =>
        await new CreateTempTestRecordHandler(repo)
            .Handle(new CreateTempTestRecordCommand(name, "d"), CancellationToken.None);

    [Fact]
    public async Task Create_ReturnsDto_WithGeneratedId()
    {
        var dto = await SeedAsync("first");

        Assert.Equal(1, dto.Id);
        Assert.Equal("first", dto.Name);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenMissing()
    {
        var dto = await new GetTempTestRecordByIdHandler(repo)
            .Handle(new GetTempTestRecordByIdQuery(99), CancellationToken.None);

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetAll_ReturnsEveryRecord()
    {
        await SeedAsync("a");
        await SeedAsync("b");

        var all = await new GetAllTempTestRecordsHandler(repo)
            .Handle(new GetAllTempTestRecordsQuery(), CancellationToken.None);

        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task Update_ChangesRecord()
    {
        var created = await SeedAsync("before");

        var updated = await new UpdateTempTestRecordHandler(repo)
            .Handle(new UpdateTempTestRecordCommand(created.Id, "after", "new"), CancellationToken.None);

        Assert.Equal("after", updated.Name);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task Update_Throws_WhenRecordMissing()
    {
        var handler = new UpdateTempTestRecordHandler(repo);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new UpdateTempTestRecordCommand(42, "x", "y"), CancellationToken.None));
    }

    [Fact]
    public async Task Delete_ReturnsTrueOnce_ThenFalse()
    {
        var created = await SeedAsync();
        var handler = new DeleteTempTestRecordHandler(repo);

        Assert.True(await handler.Handle(new DeleteTempTestRecordCommand(created.Id), CancellationToken.None));
        Assert.False(await handler.Handle(new DeleteTempTestRecordCommand(created.Id), CancellationToken.None));
    }
}
