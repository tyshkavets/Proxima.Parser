namespace Proxima.Parser.Tests;

public class ValidBasicCommandTests
{
    [Test]
    public void SingleCommandDoesNotThrow()
    {
        Assert.DoesNotThrow(() => ProgramParser.ParseProgram("mov ax, bx"));    
    }

    [Test]
    public void MultipleLinesWithCommentsAndLabels()
    {
        var result = ProgramParser.ParseProgram(@".start: mov ax, bx # Test
.addition:
add ax, bx # interesting
");
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Lines, Has.Length.EqualTo(4));
            Assert.That(result.Lines[0].Operation, Is.EqualTo("mov"));
            Assert.That(result.Lines[0].Comment.Trim(), Is.EqualTo("Test"));
            Assert.That(result.Lines[0].Label, Is.EqualTo("start"));
            Assert.That(result.Lines[1].Label, Is.EqualTo("addition"));
        });
    }
}