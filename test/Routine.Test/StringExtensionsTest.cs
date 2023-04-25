namespace Routine.Test;

[TestFixture]
public class StringExtensionsTest
{
    [Test]
    public void Test_String_ToUpperInitial()
    {
        Assert.That("lower".ToUpperInitial(), Is.EqualTo("Lower"));
        Assert.That("Upper".ToUpperInitial(), Is.EqualTo("Upper"));
        Assert.That("invariant".ToUpperInitial(), Is.EqualTo("Invariant"));
        Assert.That("i".ToUpperInitial(), Is.EqualTo("I"));
        Assert.That("".ToUpperInitial(), Is.EqualTo(""));
    }

    [Test]
    public void Test_String_ToLowerInitial()
    {
        Assert.That("lower".ToLowerInitial(), Is.EqualTo("lower"));
        Assert.That("Upper".ToLowerInitial(), Is.EqualTo("upper"));
        Assert.That("Invariant".ToLowerInitial(), Is.EqualTo("invariant"));
        Assert.That("I".ToLowerInitial(), Is.EqualTo("i"));
        Assert.That("".ToLowerInitial(), Is.EqualTo(""));
    }

    [Test]
    public void Test_String_SplitCamelCase()
    {
        Assert.That("camelCase".SplitCamelCase(), Is.EqualTo("camel Case"));
        Assert.That("PascalCase".SplitCamelCase(), Is.EqualTo("Pascal Case"));
        Assert.That("PASCALCase".SplitCamelCase(), Is.EqualTo("PASCAL Case"));
        Assert.That("Number00Case".SplitCamelCase(), Is.EqualTo("Number 00 Case"));

        Assert.That("camelCase".SplitCamelCase('-'), Is.EqualTo("camel-Case"));
        Assert.That("PascalCase".SplitCamelCase('-'), Is.EqualTo("Pascal-Case"));
        Assert.That("PASCALCase".SplitCamelCase('-'), Is.EqualTo("PASCAL-Case"));
        Assert.That("Number00Case".SplitCamelCase('-'), Is.EqualTo("Number-00-Case"));
    }

    [Test]
    public void Test_String_SnakeCaseToCamelCase()
    {
        Assert.That("snake_case".SnakeCaseToCamelCase(), Is.EqualTo("snakeCase"));
        Assert.That("snake_Case".SnakeCaseToCamelCase(), Is.EqualTo("snakeCase"));
        Assert.That("Snake_Case".SnakeCaseToCamelCase(), Is.EqualTo("SnakeCase"));
        Assert.That("camelCase".SnakeCaseToCamelCase(), Is.EqualTo("camelCase"));
        Assert.That("SNAKE_case".SnakeCaseToCamelCase(), Is.EqualTo("SNAKECase"));
        Assert.That("snake_00_case".SnakeCaseToCamelCase(), Is.EqualTo("snake00Case"));

        Assert.That("snake-case".SnakeCaseToCamelCase('-'), Is.EqualTo("snakeCase"));
        Assert.That("snake-Case".SnakeCaseToCamelCase('-'), Is.EqualTo("snakeCase"));
        Assert.That("Snake-Case".SnakeCaseToCamelCase('-'), Is.EqualTo("SnakeCase"));
        Assert.That("camelCase".SnakeCaseToCamelCase('-'), Is.EqualTo("camelCase"));
        Assert.That("SNAKE-case".SnakeCaseToCamelCase('-'), Is.EqualTo("SNAKECase"));
        Assert.That("snake-00-case".SnakeCaseToCamelCase('-'), Is.EqualTo("snake00Case"));
    }

    [Test]
    public void Test_String_Before()
    {
        Assert.That("a".Before("."), Is.EqualTo("a"));
        Assert.That("a.b".Before("."), Is.EqualTo("a"));
        Assert.That("a.b.c".Before("."), Is.EqualTo("a"));

        Assert.That("a".BeforeLast("."), Is.EqualTo("a"));
        Assert.That("a.b".BeforeLast("."), Is.EqualTo("a"));
        Assert.That("a.b.c".BeforeLast("."), Is.EqualTo("a.b"));

        Assert.That(".a.a".Before("A", StringComparison.OrdinalIgnoreCase), Is.EqualTo("."));
        Assert.That(".a.a".BeforeLast("A", StringComparison.OrdinalIgnoreCase), Is.EqualTo(".a."));

        Assert.That("ReferenceCoreX".Before("Core"), Is.EqualTo("Reference"));
        Assert.That("ReferenceCore".Before("Core"), Is.EqualTo("Reference"));
    }

    [Test]
    public void Test_String_After()
    {
        Assert.That("a".After("."), Is.EqualTo("a"));
        Assert.That("a.b".After("."), Is.EqualTo("b"));
        Assert.That("a.b.c".After("."), Is.EqualTo("b.c"));

        Assert.That("a".AfterLast("."), Is.EqualTo("a"));
        Assert.That("a.b".AfterLast("."), Is.EqualTo("b"));
        Assert.That("a.b.c".AfterLast("."), Is.EqualTo("c"));

        Assert.That("a.a.".After("A", StringComparison.OrdinalIgnoreCase), Is.EqualTo(".a."));
        Assert.That("a.a.".AfterLast("A", StringComparison.OrdinalIgnoreCase), Is.EqualTo("."));

        Assert.That("CoreReference".After("Core"), Is.EqualTo("Reference"));
        Assert.That("XCoreReference".After("Core"), Is.EqualTo("Reference"));
    }

    [Test]
    public void Test_String_SurroundWith()
    {
        Assert.That("a".SurroundWith("'"), Is.EqualTo("'a'"));
        Assert.That("a".SurroundWith("[", "]"), Is.EqualTo("[a]"));
        Assert.That(((string)null).SurroundWith("'"), Is.EqualTo("''"));
    }

    [Test]
    public void Test_String_Append()
    {
        Assert.That("a".Append(".suffix"), Is.EqualTo("a.suffix"));
    }

    [Test]
    public void Test_String_Prepend()
    {
        Assert.That("a".Prepend("prefix."), Is.EqualTo("prefix.a"));
    }

    [Test]
    public void Test_String_Matches()
    {
        Assert.That("test".Matches(".es."), Is.True);
        Assert.That("test".Matches("es."), Is.False);
        Assert.That("test".Matches(".es"), Is.False);
    }
}
