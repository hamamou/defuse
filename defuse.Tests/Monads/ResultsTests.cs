namespace Defuse.Monads.Tests;

public class ResultTests
{
    [Test]
    public void Success_ShouldReturnSuccessResult()
    {
        var result = Result.Success<int>(42);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(42));
            Assert.That(result.Error, Is.Null);
        });
    }

    [Test]
    public void Failure_ShouldReturnFailureResult()
    {
        var result = Result.Failure<int>("Something went wrong");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("Something went wrong"));
            Assert.That(result.Value, Is.EqualTo(0));
        });
    }

    [Test]
    public void ImplicitConversion_FromValueToResult_ShouldSucceed()
    {
        Result<int> result = 99;

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(99));
            Assert.That(result.Error, Is.Null);
        });
    }

    [Test]
    public void ImplicitConversion_FromErrorStringToResult_ShouldFail()
    {
        Result<int> result = "Error occurred";

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("Error occurred"));
            Assert.That(result.Value, Is.EqualTo(0));
        });
    }

    [Test]
    public void SuccessWithNullValue_ShouldSucceedWithNullError()
    {
        var result = Result.Success<string>(null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Error, Is.Null);
            Assert.That(result.Value, Is.Null);
        });
    }

    [Test]
    public void FailureWithNullValue_ShouldReturnFailureWithErrorMessage()
    {
        var result = Result.Failure<string>("Null value failure");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("Null value failure"));
            Assert.That(result.Value, Is.Null);
        });
    }

    [Test]
    public void ImplicitConversion_FromEmptyString_ShouldFail()
    {
        Result<int> result = string.Empty;

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo(string.Empty));
            Assert.That(result.Value, Is.EqualTo(0));
        });
    }
}
