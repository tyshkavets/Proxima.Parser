using NUnit.Framework;

namespace Proxima.Parser;

public class ParserTest
{
    [Test]
    public void Test()
    {
  /*      var result = ProgramParser.ParseProgram("mov ax, bx");

        var result2 = ProgramParser.ParseProgram(@"mov ax, bx
add ax, bx");
        var result3 = ProgramParser.ParseProgram(@".test:  mov ax, bx");
*/
        var result4 = ProgramParser.ParseProgram(@".test:  mov ax, bx # MOV BX TO AX
.anotherLabel: add ax, bx # SETS BX TO BX + AX
");
        var result5 = ProgramParser.ParseProgram(@"mov ax, bx # Test
.addition:
add ax, bx # interesting
");
        Assert.Pass();
    }
}